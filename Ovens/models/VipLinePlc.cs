using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Oskas;
using Oskas.Functions.Plcs;
using System.Timers;
using System.Collections.Concurrent;
using Newtonsoft.Json;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;


namespace Ovens.models
{
    //
    // VIP自動搬送ラインPLC 取得データ
    //
    [Serializable]
    public class OvenPlcData
    {
        public int Status { get; set; }
        public String LastDtHub { get; set; }
        public List<DateTime> PlcDateTimes { get; set; }
        public OvenData[] OvenDat { get; set; }
    }

    [Serializable]
    public class OvenData
    {
        public List<float> SpValue { get; set; }
        public List<float> PvValue { get; set; }
        public List<string> MacNo { get; set; }
        public List<float> StdevPlus { get; set; }
        public List<float> StdevMinus { get; set; }
        public List<int> Ng1Flag { get; set; }
        public List<int> Ng2Flag { get; set; }
        public List<int> OvenBusy { get; set; }
    }

    //
    // VIP自動搬送ラインPLC
    //
    public class VipLinePlc// : IDisposable, IOVENPLC
    {
        //
        // Task Timer設定
        //
        Timer fetchTimer; //データ取得用
        Timer DbTimer; //DB Insert用
        public bool taskEnabled { get; set; } = true;
        public bool OnGoingFetchTask { get; set; } = false;
        public bool OnGoingDbTask { get; set; } = false;

        //
        // データ取得サイクル(秒)
        //
        public const int fetchCycle = 10;

        //
        // Task動作条件変数
        //
        long pingFailCount = 0; //PINGが連続してNGの場合カウントアップ
        long plcDateFailCount = 0; //取得日付データが前回データと同一の場合カウントアップ
        public int pingFailLimit { get; set; }
        public int plcDateFailLimit { get; set; }
        public TimeSpan insertThresholdTime { get; set; } = new TimeSpan(0, 0, 0, 9);

        //
        // Oven Data
        //
        ConcurrentQueue<OvenPlcData> ovenData = new ConcurrentQueue<OvenPlcData>();
        OvenPlcData ovnPlcDat = new OvenPlcData();
        TnOvenData db = new TnOvenData(DateTime.Parse("2000/1/1 0:0:0"), "", "", "");
        Dictionary<string, string> ovenGetData = new Dictionary<string, string>();
        bool ovnPlcDatLock = false;

        //
        // Action for console
        //
        Action<string, int> consoleAction;
        public void InitConsoleAction(Action<string, int> act)
        {
            consoleAction = act;
        }

        //
        // Oven PLC実体化
        //
        public string Macno { get; set; }
        public string IPAddress { get; set; }
        public int Port { get; set; }
        public int PingFailLimit { get; set; }
        public int PlcDateFailLimit { get; set; }

        public Mitsubishi Plc { get; set; }
        public List<NOven> NOvens = new List<NOven>();
        public int NumOven;

        public string DateTimeAddress { get; set; } = "D000010";

        public DateTime PlcDateTime { get; set; }
        public DateTime PreDateTime { get; set; } = DateTime.Parse("2021/1/1 00:00:00");

        //
        // Oven Status
        //
        public int FetchStatus { get; set; }
        public DateTime lastInsertWithNoError { get; set; } = DateTime.Parse("2021/1/1 00:00:00");
        public DateTime lastInsertWithError { get; set; } = DateTime.Parse("2021/1/1 00:00:00");

        public VipLinePlc(OvenClient plc)
        {
            this.Macno = plc.header_ovn.macno;
            this.IPAddress = plc.plc.ip_address;
            this.Port = plc.plc.port;
            this.PingFailLimit = plc.plc.stoptask.ping;
            this.PlcDateFailLimit = plc.plc.stoptask.plcdate;
            Plc = new Mitsubishi(IPAddress, Port);
            // PLCが持っているオーブンを実体化
            var index = 0;
            foreach (var oven in plc.oven)
            {
                NOvens.Add(new NOven(
                    index,
                    oven.oven_id,
                    oven.sp_address,
                    oven.pv_address,
                    oven.ng1_address,
                    oven.ng2_address,
                    oven.macno_address,
                    oven.error_plus_addres,
                    oven.error_minus_address,
                    oven.oven_busy_address));
                index++;
            }
            //Oven数
            NumOven = plc.oven.Count();
            //Ovenデータ用格納オブジェクト初期化
            InitOvenDat();

            ////////////////////////////////
            ///Timer割り込み
            ////////////////////////////////
            fetchTimer = new Timer(fetchCycle * 1000/*msec*/);
            fetchTimer.Elapsed += FetchTimerTask;
            fetchTimer.Enabled = true;

            DbTimer = new Timer(55 * 1000/*msec*/);
            DbTimer.Elapsed += DbTimerTask;
            DbTimer.Enabled = true;

        }

        //
        //Ovenデータ用格納オブジェクト初期化
        //
        public void InitOvenDat()
        {
            ovnPlcDat.PlcDateTimes = new List<DateTime>();
            ovnPlcDat.OvenDat = new OvenData[NumOven];
            for (int i = 0; i < NumOven; i++)
            {
                ovnPlcDat.OvenDat[i] = new OvenData();
                ovnPlcDat.OvenDat[i].SpValue = new List<float>();
                ovnPlcDat.OvenDat[i].PvValue = new List<float>();
                ovnPlcDat.OvenDat[i].Ng1Flag = new List<int>();
                ovnPlcDat.OvenDat[i].Ng2Flag = new List<int>();
                ovnPlcDat.OvenDat[i].MacNo = new List<string>();
                ovnPlcDat.OvenDat[i].StdevPlus = new List<float>();
                ovnPlcDat.OvenDat[i].StdevMinus = new List<float>();
                ovnPlcDat.OvenDat[i].OvenBusy = new List<int>();
            }
        }


        ~VipLinePlc()
        /// <summary>
        /// デストラクタ
        /// </summary>
        {
            //アンマネージリソースの解放
            this.Dispose(false);
        }

        // <summary>
        // Disposeされたかどうか
        // </summary>
        private bool disposed;

        protected void Dispose(bool disposing)
        {
            lock (this)
            {
                if (this.disposed)
                {
                    //既に呼びだしずみであるならばなんもしない
                    return;
                }

                this.disposed = true;

                if (disposing)
                {
                    // マネージリソースの解放
                }

                // アンマネージリソースの解放
                Plc.Dispose();
            }
        }

        public void Dispose()
        {
            //マネージリソースおよびアンマネージリソースの解放
            this.Dispose(true);

            //デストラクタを対象外とする
            GC.SuppressFinalize(this);
        }

        public bool isDisposed()
        {
            return disposed;
        }

        public bool Ping()
        {
            try
            {
                var host = IPAddress;
                var timeout = 100;
                var retryTimes = 3;
                return Plc.Ping(host, timeout, retryTimes);
            }
            catch (Exception)
            {
                return false;
            }
        }

        public void SetTimerEnable()
        {
            fetchTimer.Enabled = true;
            DbTimer.Enabled = true;
            taskEnabled = true;
        }

        public void SetTimerDisable()
        {
            fetchTimer.Enabled = false;
            DbTimer.Enabled = false;
            taskEnabled = false;
        }

        private void FetchTimerTask(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                OnGoingFetchTask = true;
                fetchTimer.Enabled = false;
                if (FmOvens.habEnable)
                {
                    this.RoutineFetchTask();
                }
                else
                {
                    //Ovenデータ用格納オブジェクト初期化
                    InitOvenDat();
                }
                fetchTimer.Enabled = true;
            }
            catch
            {
                fetchTimer.Enabled = false;
            }
            finally
            {
                OnGoingFetchTask = false;
                GC.Collect();
            }
        }

        private void DbTimerTask(Object source, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                OnGoingDbTask = true;
                DbTimer.Enabled = false;
                if (FmOvens.habEnable)
                {
                    var dt = DateTime.Now;
                    int iSecond = dt.Second;
                    System.Threading.Thread.Sleep((61 - iSecond) * 1000);
                    this.RoutineDbTask();
                }
                DbTimer.Enabled = true;
            }
            catch
            {
                DbTimer.Enabled = false;
                fetchTimer.Enabled = false;
            }
            finally
            {
                OnGoingDbTask = false;
                GC.Collect();
            }
        }

        //public void RunTask(bool cmdrun)
        //{
        //    if (cmdrun)
        //    {
        //        SetTimerEnable();
        //    }
        //    else
        //    {
        //        SetTimerDisable();
        //    }
        //}

        private async void RoutineFetchTask()
        {
            FetchStatus = 0;

            await Task.Run(() =>
            {
                try
                {
                    ovnPlcDatLock = true;

                    // Console出力用msg初期化
                    var msg = "■■■■■■■■■■■■■■■■■■■■■" + Macno + "■■■■■■■■■■■■■■■■■■■■■\r\n";

                    //
                    // PING確認
                    // 
                    if (!this.Ping())
                    {
                        FetchStatus = 1;
                        if (pingFailCount < PingFailLimit) pingFailCount++;
                        msg += "Ping: Fail\r\n";
                        consoleAction(msg, Cnslcnf.msg_warn);
                        return;
                    }
                    else
                    {
                        pingFailCount = 0;
                        msg += "Ping: Pass\r\n";
                    }

                    //
                    // PLC DateTime取得処理
                    //
                    var plcdt = new DateTime();
                    var dt = string.Empty;
                    var dthub = DateTime.Now;
                    if (GetPlcDataTime(ref plcdt, ref dt))
                    {
                        msg += dt + "\r\n";
                        if (plcdt == PreDateTime)
                        {
                            FetchStatus = 2;
                            if (plcDateFailCount < plcDateFailLimit) plcDateFailCount++;
                            msg += "PLCの日時が前回と同じです。" + "\r\n";
                            consoleAction(msg, Cnslcnf.msg_warn);
                            return;
                        }
                        else
                        {
                            PlcDateTime = plcdt;
                            ovnPlcDat.PlcDateTimes.Add(PlcDateTime);
                            ovnPlcDat.LastDtHub = dthub.ToString("G");
                            plcDateFailCount = 0;
                        }
                    }
                    else
                    {
                        FetchStatus = 3;
                        msg += dt + "\r\n";
                        msg += "PLCの日時データ取得時に問題が発生しました。" + "\r\n";
                        consoleAction(msg, Cnslcnf.msg_error);
                        return;
                    }

                    //
                    // データ取得処理
                    //
                    if (!this.GetAllOvenDatas(ref msg))
                    {
                        FetchStatus = 3;
                        msg += "OVENデータ取得時に問題が発生しました。" + "\r\n";
                        consoleAction(msg, Cnslcnf.msg_error);
                        return;
                    }


                    //
                    //  取得したOVNデータをコンソール出力処理
                    //

                    consoleAction(msg, Cnslcnf.msg_info);

                }
                catch (Exception e)
                {
                    FetchStatus = 4;
                    consoleAction(e.ToString(), 10);
                    // 一応こうしておく↓
                    consoleAction($"【{Macno}】動作を停止します。", Cnslcnf.msg_error);
                    SetTimerDisable();
                }
                finally
                {
                    //var dt = DateTime.Now;
                    //var spanDtWithNoError = dt - lastInsertWithNoError;
                    //var spanDtWithError = dt - lastInsertWithError;
                    //// 
                    ////  FIFOに取得データ格納
                    ////
                    //if (FetchStatus == 0)
                    //{
                    //    if (spanDtWithNoError > insertThresholdTime)
                    //    {
                    //        ovenData.Enqueue(ovnPlcDat);
                    //        lastInsertWithNoError = dt;
                    //    }
                    //}
                    //else
                    //{
                    //    if ((spanDtWithNoError > insertThresholdTime) && (spanDtWithError > insertThresholdTime))
                    //    {
                    //        ovenData.Enqueue(ovnPlcDat);
                    //        lastInsertWithNoError = dt;
                    //    }
                    //}


                    ////
                    //// FIFOからデータ取出し⇒Josn Parse⇒DB登録
                    ////
                    //while (ovenData.Count() != 0)
                    //{
                    //    OvenPlcData data;
                    //    string json = "{}";
                    //    if (ovenData.TryDequeue(out data))
                    //    {
                    //        // json Parse
                    //        json = JsonConvert.SerializeObject(data, Formatting.Indented);
                    //    }

                    //    // DB処理
                    //    try
                    //    {
                    //        var error_flg = "true";
                    //        if (FetchStatus == 0) { error_flg = "false"; }
                    //        db = new TnOvenData(dt, macno, json, error_flg);

                    //        var msg = string.Empty;
                    //        if (!db.InsertTnOvenData(ref msg))
                    //        {
                    //            consoleAction(msg + "\r\n", Cnslcnf.msg_error);
                    //        }
                    //    }
                    //    catch (Exception)
                    //    {
                    //        ovenData.Enqueue(data);
                    //        break;
                    //    }
                    //}

                    //
                    // 連続不具合の確認⇒TimerDisable
                    //
                    if ((pingFailCount > PingFailLimit))
                    {
                        consoleAction($"【{Macno}】PING不通が規定回数を超えた為、動作を停止します。", Cnslcnf.msg_error);
                        SetTimerDisable();
                        FetchStatus = 4;
                    }

                    if ((pingFailCount > PingFailLimit) || (plcDateFailCount > PlcDateFailLimit))
                    {
                        consoleAction($"【{Macno}】データ更新の無い状態が規定回数を超えた為、動作を停止します。", Cnslcnf.msg_error);
                        SetTimerDisable();
                        FetchStatus = 4;
                    }

                    //
                    // PLCの現在の状態をセット
                    //
                    PreDateTime = PlcDateTime;

                    ovnPlcDatLock = false;
                }
            });
        }

        private async void RoutineDbTask()
        {
            try
            {
                await Task.Run(() =>
                {
                    OnGoingDbTask = true;

                    var dt = DateTime.Now;

                    if (ovnPlcDat.PlcDateTimes.Count != 0)
                    {
                        ovnPlcDat.Status = 0;
                    }
                    else
                    {
                        ovnPlcDat.Status = 1;
                    }

                    var OPD = new OvenPlcData();
                    //OPD.Status = ovnPlcDat.Status;
                    //OPD.LastDtHub = ovnPlcDat.LastDtHub;
                    //OPD.PlcDateTimes = ovnPlcDat.PlcDateTimes;
                    //OPD.OvenDat = new OvenData[NumOven];
                    //var i = 0;
                    //foreach (var oven in ovnPlcDat.OvenDat)
                    //{
                    //    OPD.OvenDat[i] = oven.DeepClone();
                    //    i++;
                    //}
                    OPD = ovnPlcDat.DeepClone();

                    //
                    //初期化処理
                    //
                    int elapsed = 0;
                    while ((ovnPlcDatLock) && (elapsed < TimeSpan.FromSeconds(10).TotalMilliseconds))
                    {
                        System.Threading.Thread.Sleep(1000);
                        elapsed += 1000;
                        //Console.WriteLine($"+1000");
                    }
                    InitOvenDat();  //初期化


                    //var spanDtWithNoError = dt - lastInsertWithNoError;
                    //var spanDtWithError = dt - lastInsertWithError;
                    //// 
                    ////  FIFOに取得データ格納
                    ////
                    //if (FetchStatus == 0)
                    //{
                    //    if (spanDtWithNoError > insertThresholdTime)
                    //    {
                    //        ovenData.Enqueue(ovnPlcDat);
                    //        lastInsertWithNoError = dt;
                    //    }
                    //}
                    //else
                    //{
                    //    if ((spanDtWithNoError > insertThresholdTime) && (spanDtWithError > insertThresholdTime))
                    //    {
                    //        ovenData.Enqueue(ovnPlcDat);
                    //        lastInsertWithNoError = dt;
                    //    }
                    //}

                    // 
                    //  FIFOに取得データ格納
                    //
                    //ovenData.Enqueue(ovnPlcDat);


                    //
                    // FIFOからデータ取出し⇒Josn Parse⇒DB登録
                    //
                    //while (ovenData.Count() != 0)
                    //{
                    //    OvenPlcData data;
                    //    string json = "{}";
                    //    try
                    //    {
                    //        if (ovenData.TryDequeue(out data))
                    //        {
                    //            var settings = new JsonSerializerSettings
                    //            {
                    //                NullValueHandling = NullValueHandling.Ignore

                    //            };
                    //            // json Parse
                    //            json = JsonConvert.SerializeObject(data, Formatting.Indented, settings);
                    //        }

                    //        // DB処理
                    //        var error_flg = "true";
                    //        if (FetchStatus == 0) { error_flg = "false"; }
                    //        db = new TnOvenData(dt, Macno, json, error_flg);

                    //        var msg = string.Empty;
                    //        //if (!db.InsertTnOvenData(ref msg))
                    //        if (!db.InsertTnOvenData())
                    //        {
                    //            msg = "DB登録が失敗しました";
                    //            consoleAction(msg + "\r\n", Cnslcnf.msg_error);
                    //        }
                    //        }
                    //    catch (Exception)
                    //    {
                    //        //ovenData.;
                    //        break;
                    //    }

                    string json = "{}";
                    var settings = new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore

                    };
                    // json Parse
                    json = JsonConvert.SerializeObject(OPD, Formatting.Indented, settings);


                    // DB処理
                    var error_flg = "true";
                    if (FetchStatus == 0) { error_flg = "false"; }
                    db = new TnOvenData(dt, Macno, json, error_flg);

                    var msg = string.Empty;
                    //if (!db.InsertTnOvenData(ref msg))
                    if (!db.InsertTnOvenData())
                    {
                        msg = "DB登録が失敗しました";
                        consoleAction(msg + "\r\n", Cnslcnf.msg_error);
                    }
                    else
                    {
                        var cnt = OPD.PlcDateTimes.Count();
                        consoleAction(Macno + "から" + cnt + "件のデータをDB登録しました\r\n", Cnslcnf.msg_info);
                    }
                });
            }
            catch (Exception ex)
            {
                consoleAction(ex.ToString() + "\r\n", Cnslcnf.msg_error);
            }
            finally
            {
                //int elapsed = 0;
                //while ((ovnPlcDatLock) && (elapsed < TimeSpan.FromSeconds(10).TotalMilliseconds))
                //{
                //    System.Threading.Thread.Sleep(1000);
                //    elapsed += 1000;
                //    //Console.WriteLine($"+1000");
                //}
                //InitOvenDat();  //初期化
            }

        }

        //拾ってきたけど結局使わなかった
        //public bool RetryUntilSuccessOrTimeout(Func<bool> task, TimeSpan timeSpan)
        //{
        //    bool success = false;
        //    int elapsed = 0;
        //    while ((!success) && (elapsed < timeSpan.TotalMilliseconds))
        //    {
        //        System.Threading.Thread.Sleep(1000);
        //        elapsed += 1000;
        //        success = task();
        //    }
        //    return success;
        //}


        private bool GetPlcDataTime(ref DateTime PlcDateTime, ref string dt)
        {
            try
            {
                var plcMemAddr = string.Empty;
                if (!Mitsubishi.isHexFormAddress(DateTimeAddress))
                {
                    var hexaddress = string.Empty;
                    if (Mitsubishi.ExchangeDecAddressDevtoHex(DateTimeAddress, ref hexaddress))
                    {
                        plcMemAddr = hexaddress;
                    }
                }
                PlcDateTime = Plc.GetWordsAsDateTime(plcMemAddr);
                dt = Plc.GetWordsAsDateTime(plcMemAddr).ToString();
                return true;
            }
            catch (Exception ex)
            {
                dt = ex.ToString();
                return false;
            }

        }

        private bool GetAllOvenDatas(ref string dbgmsg)
        {
            try
            {
                foreach (var oven in NOvens)
                {
                    dbgmsg += "【" + oven.OVEN_NAME + "】";
                    ovenGetData = new Dictionary<string, string>();
                    if (oven.GetOvenData(Plc, ref ovenGetData))
                    {
                        foreach (var data in ovenGetData)
                        {
                            switch (data.Key)
                            {
                                case "SP":
                                    ovnPlcDat.OvenDat[oven.INDEX].SpValue.Add(float.Parse(data.Value));
                                    break;
                                case "PV":
                                    ovnPlcDat.OvenDat[oven.INDEX].PvValue.Add(float.Parse(data.Value));
                                    break;
                                case "NG1":
                                    ovnPlcDat.OvenDat[oven.INDEX].Ng1Flag.Add(int.Parse(data.Value));
                                    break;
                                case "NG2":
                                    ovnPlcDat.OvenDat[oven.INDEX].Ng2Flag.Add(int.Parse(data.Value));
                                    break;
                                case "MACNO":
                                    ovnPlcDat.OvenDat[oven.INDEX].MacNo.Add(data.Value);
                                    break;
                                case "STDEV_PLUS":
                                    ovnPlcDat.OvenDat[oven.INDEX].StdevPlus.Add(float.Parse(data.Value));
                                    break;
                                case "STDEV_MINUS":
                                    ovnPlcDat.OvenDat[oven.INDEX].StdevMinus.Add(float.Parse(data.Value));
                                    break;
                                case "OVEN_BUSY":
                                    ovnPlcDat.OvenDat[oven.INDEX].OvenBusy.Add(int.Parse(data.Value));
                                    break;
                            }

                            dbgmsg += data.Key + ": " + data.Value + " ";
                        }
                        dbgmsg += "\r\n";
                    }
                    else
                    {
                        //
                        dbgmsg = "データ取得失敗" + "\r\n";
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                dbgmsg = ex.ToString();
                return false;
            }
        }

    }

    //
    // VIP自動搬送ラインオーブン
    //
    public class NOven
    {
        public int INDEX { get; set; }                //インデックス
        public string OVEN_NAME { get; set; }           //オーブン名称
        public string SP_ADDRESS { get; set; }          //オーブン温度目標値
        public string PV_ADDRESS { get; set; }          //オーブン温度現在値
        public string NG1_ADDRESS { get; set; }         //NG要因１（偏差）
        public string NG2_ADDRESS { get; set; }         //NG要因２（停止）
        public string MACNO_ADDRESS { get; set; }       //オーブン設備番号
        public string ERROR_PLUS_ADDRESS { get; set; }  //偏差限界+
        public string ERROR_MINUS_ADDRESS { get; set; } //偏差限界-
        public string OVEN_BUSY_ADDRESS { get; set; }   //オーブン稼働中

        Dictionary<string, int> strdataln_dict = new Dictionary<string, int>();
        Dictionary<string, string> strdatalbl_dict = new Dictionary<string, string>();
        Dictionary<string, string> bitdata_dict = new Dictionary<string, string>();

        public NOven(
            int INDEX,
            string OVEN_NAME,
            string SP_ADDRESS,
            string PV_ADDRESS,
            string NG1_ADDRESS,
            string NG2_ADDRESS,
            string MACNO_ADDRESS,
            string ERROR_PLUS_ADDRESS,
            string ERROR_MINUS_ADDRESS,
            string OVEN_BUSY_ADDRESS)
        {
            this.INDEX = INDEX;
            this.OVEN_NAME = OVEN_NAME;
            this.SP_ADDRESS = SP_ADDRESS;
            this.PV_ADDRESS = PV_ADDRESS;
            this.NG1_ADDRESS = NG1_ADDRESS;
            this.NG2_ADDRESS = NG2_ADDRESS;
            this.MACNO_ADDRESS = MACNO_ADDRESS;
            this.ERROR_PLUS_ADDRESS = ERROR_PLUS_ADDRESS;
            this.ERROR_MINUS_ADDRESS = ERROR_MINUS_ADDRESS;
            this.OVEN_BUSY_ADDRESS = OVEN_BUSY_ADDRESS;
        }

        public bool GetOvenData(Mitsubishi Plc, ref Dictionary<string, string> dData)
        {
            try
            {
                strdataln_dict = new Dictionary<string, int>
                {
                    { SP_ADDRESS, 1 },
                    { PV_ADDRESS, 1 },
                    { MACNO_ADDRESS, 1 },
                    { ERROR_PLUS_ADDRESS, 1 },
                    { ERROR_MINUS_ADDRESS, 1 }
                };
                strdatalbl_dict = new Dictionary<string, string>
                {
                    { SP_ADDRESS, "SP" },
                    { PV_ADDRESS, "PV" },
                    { MACNO_ADDRESS, "MACNO" },
                    { ERROR_PLUS_ADDRESS, "STDEV_PLUS" },
                    { ERROR_MINUS_ADDRESS, "STDEV_MINUS" }
                };
                bitdata_dict = new Dictionary<string, string>
                {
                    { NG1_ADDRESS, "NG1" },
                    { NG2_ADDRESS, "NG2" },
                    { OVEN_BUSY_ADDRESS, "OVEN_BUSY" }
                };

                foreach (var data in strdataln_dict)
                {
                    var plcMemAddr = data.Key;
                    if (!Mitsubishi.isHexFormAddress(plcMemAddr))
                    {
                        var hexaddress = string.Empty;
                        if (Mitsubishi.ExchangeDecAddressDevtoHex(plcMemAddr, ref hexaddress))
                        {
                            plcMemAddr = hexaddress;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    var ret = Plc.GetWordAsDecimalData(plcMemAddr, data.Value);
                    if (ret == -1) return false;
                    dData.Add(strdatalbl_dict[data.Key], ret.ToString());
                    System.Threading.Thread.Sleep(10);
                }

                foreach (var data in bitdata_dict)
                {
                    var plcMemAddr = data.Key;
                    if (!Mitsubishi.isHexFormAddress(plcMemAddr))
                    {
                        var hexaddress = string.Empty;
                        if (Mitsubishi.ExchangeDecAddressDevtoHex(plcMemAddr, ref hexaddress))
                        {
                            plcMemAddr = hexaddress;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    dData.Add(data.Value, Plc.GetBit(plcMemAddr, 1));
                    System.Threading.Thread.Sleep(10);
                }

                return true;
            }
            catch (Exception)
            {

                return false;
            }

        }

    }

    public static class ObjectExtension
    {
        // ディープコピーの複製を作る拡張メソッド
        public static T DeepClone<T>(this T src)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                var binaryFormatter
                  = new System.Runtime.Serialization
                        .Formatters.Binary.BinaryFormatter();
                binaryFormatter.Serialize(memoryStream, src); // シリアライズ
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);
                return (T)binaryFormatter.Deserialize(memoryStream); // デシリアライズ
            }
        }
    }
}