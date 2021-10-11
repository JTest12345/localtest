using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsApi;
using ARMS3.Model.PLC;


namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 天井にロボットが載って移載機と一体化した遠心沈降機
    /// ダミーマガジンは装置が自動で対応
    /// </summary>
    public class ECK4 : MachineBase
    {
        public string IpAddress { get; set; }
        public int Port { get; set; }

        // 201602/24～26検証内容 永尾追加

        /// <summary>
        /// マガジン交換のWorkCD
        /// 遠沈Mag供給時に次の工程がこの作業かどうかでPLCへ送信する信号「沈降機工程有無」を変える
        /// </summary>
        const string MAG_EXCHANGE_WORK_CD = "MD0103";

        /// <summary>
        /// マガジン移載前待機のWorkCD
        /// 1. この作業なら自動完了登録を行う
        /// 2. 遠沈Mag供給時に次の工程がこの作業かどうかでPLCへ送信する信号「沈降機工程有無」を変える
        /// </summary>
        const string MAG_PRE_EXCHANGE_WORK_CD = "BUFFER";

        /// <summary>
        /// 供給要求
        /// </summary>
        public SortedList<int, string> LoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 供給FromTo
        /// </summary>
        public SortedList<int, string> LoaderPointList { get; set; }
        
        /// <summary>
        /// アンローダーリスト
        /// </summary>
        public List<Station> UnloaderStations { get; set; }

        /// <summary>
        /// 合体している移載機のMacNo
        /// </summary>
        public int CombinedMagExchangerMacNo { get; set; }

        // 201602/24～26検証内容 永尾追加

        /// <summary>
        /// 供給するマガジンの沈降機工程有無をPC ⇒ PLCへ送信する
        /// 0:沈降機工程有
        /// 1:沈降機工程無(移載のみ)
        /// </summary>
        public string MagECKProcessHasWordAddress { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public const string PC_READY = "B0FF";

        MachinePLC mplc = MachinePLC.GetInstance();

        public ECK4()
            : base()
        {
            this.LoaderPointList = new SortedList<int, string>();
            this.LoaderReqBitAddressList = new SortedList<int, string>();
            
            this.UnloaderStations = new List<Station>();
            this.UnloaderStations.Add(Station.Unloader1);
            this.UnloaderStations.Add(Station.Unloader2);
            this.UnloaderStations.Add(Station.Unloader3);
            this.UnloaderStations.Add(Station.Unloader4);
            this.UnloaderStations.Add(Station.Unloader5);
            this.UnloaderStations.Add(Station.Unloader6);
            this.UnloaderStations.Add(Station.Unloader7);

        }

        protected override void concreteThreadWork() 
        {
            //if (this.IsRequireOutput() == false)
            //{
            workComplete();
            //}
            
            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        public void Pc_Ready()
        {
            //上位リンクモード
            this.mplc.SetBit(PC_READY, MachinePLC.BIT_ON, this.IpAddress, this.Port);
        }

        #region workComplete
        private void workComplete()
        {
            IMachine exc = LineKeeper.GetMachine(this.CombinedMagExchangerMacNo);
            if (exc == null)
            {
                throw new ApplicationException("ECK4：移載機参照エラー");
            }

            foreach (Station st in this.UnloaderStations)
            {
                VirtualMag ulMagazine = this.Peek(st);

                if (ulMagazine != null) continue;
                if (IsULMagazineExists(st) == false) continue;

                string newmagno = getMagNo(st, 0);
                VirtualMag magExVmag = exc.Peek(Station.Loader);
                if (newmagno == "" || (magExVmag != null && newmagno == magExVmag.MagazineNo))
                {
                    //マガジン移載機のローダー側仮想マガジンと一致している場合は何もしない
                    //遠沈上部のロボットへ移動指示を出した段階で仮想マガジンを移してあるため、指示直後～実際にロボットが掴むまでの間の状態
                    continue;
                }

                VirtualMag newmag = new VirtualMag();

                if (string.IsNullOrEmpty(newmagno) == false)
                {
                    newmag.MagazineNo = newmagno;
                    newmag.LastMagazineNo = newmagno;
                }
                else
                {
                    Log.RBLog.Info("遠心沈降機 排出マガジンNOの取得に失敗: " + st.ToString());
                    return;
                }

                ////作業開始完了時間取得
                //try
                //{
                //    newmag.WorkComplete = getWorkCompleteTime(st).Value;
                //}
                //catch
                //{
                //    throw new ApplicationException(string.Format("遠心沈降機 作業完了時間取得失敗:{0}", this.MacNo));
                //}

                //try
                //{
                //    newmag.WorkStart = getWorkStartTime(st).Value;
                //}
                //catch
                //{
                //    throw new ApplicationException(string.Format("遠心沈降機 作業開始時間取得失敗:{0}", this.MacNo));
                //}

                ////作業IDを取得
                //newmag.ProcNo = Order.GetLastProcNoWithDevidedMag(this.MacNo, newmag.MagazineNo);
                //this.Enqueue(newmag, st);
                //base.WorkComplete(newmag, this, true);


                ////移載前待機作業があれば自動で完了登録を行う
                //Magazine svrmag = Magazine.GetCurrent(newmag.MagazineNo);
                //AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                //var nextproc = Process.GetProcess(newmag.ProcNo.Value);

                ////完了時にエラーが出た場合はnextprocが更新されないので異常な状態になる。
                ////処理必要

                ////var nextproc = Process.GetNextProcess(newmag.ProcNo.Value, lot);
                //if (nextproc.WorkCd.ToUpper() == "BUFFER")
                //{
                //    newmag.ProcNo = nextproc.ProcNo;
                //    newmag.WorkStart = newmag.WorkComplete.Value;
                //    newmag.NextMachines.Clear();
                //    newmag.Updatequeue();
                //    base.WorkComplete(newmag, this, true);

                //    //workComplete内で次作業にされるので再び戻す
                //    newmag.ProcNo = nextproc.ProcNo;
                //    newmag.Updatequeue();
                //}

                int? MagKb = getMagKb(st);

                if (MagKb == 0)
                {
                    // 0:沈降機工程有 =  沈降作業完了登録 + 仮想マガジン作成 ( ⇒ 移載前作業開始完了登録 + 仮想マガジン更新)

                    //作業開始完了時間取得
                    try
                    {
                        newmag.WorkComplete = getWorkCompleteTime(st).Value;
                    }
                    catch
                    {
                        throw new ApplicationException(string.Format("遠心沈降機 作業完了時間取得失敗:{0}, 位置= {1}", this.MacNo, st.ToString()));
                    }

                    try
                    {
                        newmag.WorkStart = getWorkStartTime(st).Value;
                    }
                    catch
                    {
                        throw new ApplicationException(string.Format("遠心沈降機 作業開始時間取得失敗:{0}, 位置= {1}", this.MacNo, st.ToString()));
                    }

                    //作業IDを取得
                    newmag.ProcNo = Order.GetLastProcNoWithDevidedMag(this.MacNo, newmag.MagazineNo);
                    this.Enqueue(newmag, st);
                    base.WorkComplete(newmag, this, true);


                    //移載前待機作業があれば自動で完了登録を行う
                    Magazine svrmag = Magazine.GetCurrent(newmag.MagazineNo);
                    AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                    var nextproc = Process.GetProcess(newmag.ProcNo.Value);
                    if (nextproc.WorkCd.ToUpper() == "BUFFER")
                    {
                        newmag.ProcNo = nextproc.ProcNo;
                        newmag.WorkStart = newmag.WorkComplete.Value;
                        newmag.NextMachines.Clear();
                        newmag.Updatequeue();
                        base.WorkComplete(newmag, this, true);

                        //workComplete内で次作業にされるので再び戻す (工程No = 現在完了工程)
                        //移載部へマガジンが移動する時に工程Noが次の工程Noに更新される
                        newmag.ProcNo = nextproc.ProcNo;
                        newmag.Updatequeue();
                    }
                    else
                    {
                        //workComplete内で次作業にされるので再び戻す (工程No = 現在完了工程)
                        //移載部へマガジンが移動する時に工程Noが次の工程Noに更新される
                        newmag.ProcNo = svrmag.NowCompProcess;
                        newmag.Updatequeue();
                    }
                }

                else if(MagKb == 1)
                {
                    // 1:沈降機工程無 =  (移載前作業開始完了登録 ⇒ ) 仮想マガジン作成 

                    Magazine svrmag = Magazine.GetCurrent(newmag.MagazineNo);
                    AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);

                    newmag.ProcNo = svrmag.NowCompProcess;
                    newmag.WorkStart = DateTime.Now;
                    newmag.WorkComplete = DateTime.Now;

                    //次工程が移載前待機作業であれば自動で完了登録を行う
                    var nextproc = Process.GetNextProcess(svrmag.NowCompProcess, lot);
                    if (nextproc.WorkCd.ToUpper() == "BUFFER")
                    {
                        newmag.ProcNo = nextproc.ProcNo;
                        base.WorkComplete(newmag, this, true);

                        //workComplete内で次作業にされるので再び戻す (工程No = 現在完了工程)
                        //移載部へマガジンが移動する時に工程Noが次の工程Noに更新される
                        newmag.ProcNo = nextproc.ProcNo;
                    }

                    // 仮想マガジン生成
                    this.Enqueue(newmag, st);

                }
                else
                {
                    Log.RBLog.Info("沈降後マガジン置き場の沈降機工程有無の取得に失敗: " + this.MacNo + " - " + st.ToString());
                    return;
                }
            }
        }

        #endregion

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
            if (this.UnloaderStations.Contains(station) == false)
            {

                return true;
            }

            return base.Enqueue(mag, station);
        }

        #region CanInput

        /// <summary>
        /// 投入要求　可否確認
        /// </summary>
        /// <param name="mag"></param>
        /// <returns>結果</returns>
        public override bool CanInput(VirtualMag mag)
        {
            bool retv = base.CanInput(mag);
            if (retv == false) return retv;

            IMachine exc = LineKeeper.GetMachine(this.CombinedMagExchangerMacNo);
            if (exc == null)
            {
                throw new ApplicationException("ECK4：移載機参照エラー");
            }

            #region 投入規制チェック (分割ロットの場合、相方が別の遠沈自動機にいるかどうかチェック)

            Process.MagazineDevideStatus mst = Process.GetMagazineDevideStatus(mag.MagazineNo, mag.ProcNo.Value);
            if (mst == Process.MagazineDevideStatus.Double || mst == Process.MagazineDevideStatus.DoubleToSingle)
            {
                Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
                if (svrMag == null) return false;
                AsmLot asmLot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                if (asmLot == null) return false;
                
                // 自分の装置以外の遠沈自動機(沈降部)のリストを作成
                List<IMachine> otherInputECK4List = LineKeeper.Machines
                                                .Where(m => m is ECK4)
                                                .Where(m => m.MacNo != this.MacNo)
                                                .ToList();

                // 規制チェック (同じロットのマガジンが他の装置にある場合
                foreach (IMachine mac in otherInputECK4List)
                {
                    if (((ECK4)mac).ExistsPairMagazine(svrMag) == true)
                    {
                        return false;
                    }
                }

                // 供給要求を立てている遠沈前バッファが1つだけの場合、装置内に相方が投入中でなければ、投入不可とする
                //  ( 投入後、他の供給位置が解放されず、装置が詰まってしまう為)
                if (this.GetCanInputBufferCount((ECK4MagExchanger)exc) <= 1)
                {
                    if (this.ExistsPairMagazine(svrMag) == false)
                    {
                        return false;
                    }
                }
                
            }

            #endregion

            // Ver 1.156.0.3 判定追加 仮想マガジンの装置がこの遠心沈降機と同じリニア上にある場合のみにする
            Location from = new Location(mag.MacNo, Station.Unloader);
            Location to = new Location(this.MacNo, Station.Loader);
            if (Route.IsMyReachable(from, to) == true)
            {
                var nextproc = Process.GetProcess(mag.ProcNo.Value);

                if (nextproc.WorkCd == MAG_EXCHANGE_WORK_CD || nextproc.WorkCd == MAG_PRE_EXCHANGE_WORK_CD)
                {
                    // 次の工程 = 遠沈作業以外
                    Plc.SetWordAsDecimalData(MagECKProcessHasWordAddress, 1);
                    Log.RBLog.Info($"{this.Name} 装置へ沈降機工程有無信号送信 値：1, マガジンNo：{mag.MagazineNo}, 次作業：{nextproc.InlineProNM}" +
                                   $"from：{mag.MacNo}, to：{this.MacNo}");
                }
                else
                {
                    // 次の工程 = 遠沈作業
                    Plc.SetWordAsDecimalData(MagECKProcessHasWordAddress, 0);
                    Log.RBLog.Info($"{this.Name} 装置へ沈降機工程有無信号送信 値：0, マガジンNo：{mag.MagazineNo}, 次作業：{nextproc.InlineProNM}" +
                                   $"from：{mag.MacNo}, to：{this.MacNo}");
                }
            }

            return retv;

        }

        #endregion

        #region IsRequireOutput

        public override bool IsRequireOutput()
        {
            //この装置は搬送ロボットに対しては一切排出要求を出さない。
            //一体化している移載機ロボットが完成マガジンを扱う
            return false;
        }
        #endregion

        #region IsRequireInput

        public override bool IsRequireInput()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            //供給要求信号の確認
            if (this.LoaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.LoaderReqBitAddressList.Count == 0)
            {
                return false;
            }


            foreach (string address in this.LoaderReqBitAddressList.Values)
            {
                //string retv = this.Plc.GetBit(address);
                string retv;
                try
                {
                    retv = this.Plc.GetBit(address);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{address}』, エラー内容：{ex.Message}");
                    retv = Mitsubishi.BIT_OFF;
                }

                if (retv == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 投入要求確認 (バッファ位置指定)
        /// </summary>
        /// <returns>結果</returns>
        public bool IsRequireInput(string loaderPoint)
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            //投入要求信号の確認
            if (this.LoaderPointList == null)
            {
                return false;
            }

            if (this.LoaderPointList.Count == 0)
            {
                return false;
            }

            foreach (KeyValuePair<int, string> kv in this.LoaderPointList)
            {
                if(loaderPoint == kv.Value)
                {
                    if(Plc.GetBit(this.LoaderReqBitAddressList[kv.Key]) == Mitsubishi.BIT_ON)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
        #endregion

        #region ResponseEmptyMagazineRequest

        public override bool ResponseEmptyMagazineRequest()
        {
            return false;
        }

        #endregion

        #region GetFromToCode

        public override string GetFromToCode(Station station)
        {
            switch (station)
            {
                case Station.Loader:
                    foreach (KeyValuePair<int, string> kv in this.LoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return this.LoaderPointList[kv.Key];
                        }
                    }
                    return this.LoaderPointList[1];
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }
        #endregion

        #region マガジン情報取得
        public bool IsLMagazineExists(Station st, int key)
        {
            string magno = getMagNo(st, key);
            if (string.IsNullOrEmpty(magno))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public bool IsULMagazineExists(Station st)
        {
            string magno = getMagNo(st, 0);
            if (string.IsNullOrEmpty(magno))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        
        private string getMagNo(Station st, int key)
        {
            switch (st)
            {
                case Station.Loader:
                    switch (key)
                    {
                        case 1:
                            return mplc.GetMagazineNo("W26E0", true, this.IpAddress, this.Port, 10);

                        case 2:
                            return mplc.GetMagazineNo("W2700", true, this.IpAddress, this.Port, 10);

                        case 3:
                            return mplc.GetMagazineNo("W2720", true, this.IpAddress, this.Port, 10);

                        case 4:
                            return mplc.GetMagazineNo("W2740", true, this.IpAddress, this.Port, 10);

                        default:
                            throw new ApplicationException($"定義外のStation:『{st.ToString()}』+バッファ位置:『{key}』でgetMagNo関数が呼び出されました。");
                    }

                case Station.Unloader1:
                    return mplc.GetMagazineNo("W2660", true, this.IpAddress, this.Port, 10);

                case Station.Unloader2:
                    return mplc.GetMagazineNo("W2680", true, this.IpAddress, this.Port, 10);

                case Station.Unloader3:
                    return mplc.GetMagazineNo("W26A0", true, this.IpAddress, this.Port, 10);

                case Station.Unloader4:
                    return mplc.GetMagazineNo("W26C0", true, this.IpAddress, this.Port, 10);

                case Station.Unloader5:
                    return mplc.GetMagazineNo("W2760", true, this.IpAddress, this.Port, 10);

                case Station.Unloader6:
                    return mplc.GetMagazineNo("W2780", true, this.IpAddress, this.Port, 10);

                case Station.Unloader7:
                    return mplc.GetMagazineNo("W27A0", true, this.IpAddress, this.Port, 10);

                default:
                    throw new ApplicationException($"定義外のStation:『{st.ToString()}』+バッファ位置:『{key}』でgetMagNo関数が呼び出されました。");
            }
        }

        private DateTime? getWorkStartTime(Station st)
        {
            switch (st)
            {
                case Station.Unloader1:
                    return mplc.GetWordsAsDateTime("W266A", this.IpAddress, this.Port);

                case Station.Unloader2:
                    return mplc.GetWordsAsDateTime("W268A", this.IpAddress, this.Port);

                case Station.Unloader3:
                    return mplc.GetWordsAsDateTime("W26AA", this.IpAddress, this.Port);

                case Station.Unloader4:
                    return mplc.GetWordsAsDateTime("W26CA", this.IpAddress, this.Port);

                case Station.Unloader5:
                    return mplc.GetWordsAsDateTime("W276A", this.IpAddress, this.Port);

                case Station.Unloader6:
                    return mplc.GetWordsAsDateTime("W278A", this.IpAddress, this.Port);

                case Station.Unloader7:
                    return mplc.GetWordsAsDateTime("W27AA", this.IpAddress, this.Port);

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }
        }

        private DateTime? getWorkCompleteTime(Station st)
        {
            switch (st)
            {
                case Station.Unloader1:
                    return mplc.GetWordsAsDateTime("W2670", this.IpAddress, this.Port);

                case Station.Unloader2:
                    return mplc.GetWordsAsDateTime("W2690", this.IpAddress, this.Port);

                case Station.Unloader3:
                    return mplc.GetWordsAsDateTime("W26B0", this.IpAddress, this.Port);

                case Station.Unloader4:
                    return mplc.GetWordsAsDateTime("W26D0", this.IpAddress, this.Port);

                case Station.Unloader5:
                    return mplc.GetWordsAsDateTime("W2770", this.IpAddress, this.Port);

                case Station.Unloader6:
                    return mplc.GetWordsAsDateTime("W2790", this.IpAddress, this.Port);

                case Station.Unloader7:
                    return mplc.GetWordsAsDateTime("W27B0", this.IpAddress, this.Port);

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }
        }

        // 201602/24～26検証内容 永尾追加
        private int? getMagKb(Station st)
        {
            switch (st)
            {
                case Station.Unloader1:
                    return mplc.GetWordAsDecimalData("W2677", this.IpAddress, this.Port);
                    
                case Station.Unloader2:
                    return mplc.GetWordAsDecimalData("W2697", this.IpAddress, this.Port);

                case Station.Unloader3:
                    return mplc.GetWordAsDecimalData("W26B7", this.IpAddress, this.Port);

                case Station.Unloader4:
                    return mplc.GetWordAsDecimalData("W26D7", this.IpAddress, this.Port);

                case Station.Unloader5:
                    return mplc.GetWordAsDecimalData("W2777", this.IpAddress, this.Port);

                case Station.Unloader6:
                    return mplc.GetWordAsDecimalData("W2797", this.IpAddress, this.Port);

                case Station.Unloader7:
                    return mplc.GetWordAsDecimalData("W27B7", this.IpAddress, this.Port);

                default:
                    throw new ApplicationException("定義外のStation:" + st.ToString());
            }
        }
        #endregion

        public static bool IsMagExchangeWorkCd(string workCd)
        {
            if (workCd == MAG_EXCHANGE_WORK_CD)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region GetFromBufferCode        
        /// <summary>
        /// 搬送先バッファ位置取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetFromToBufferCode(Station station)
        {
            switch (station)
            {
                case Station.Loader:
                    foreach (KeyValuePair<int, string> kv in this.LoaderReqBitAddressList)
                    {
                        if (Plc.GetBit(kv.Value) == Mitsubishi.BIT_ON)
                        {
                            return kv.Key.ToString();
                        }
                    }
                    return null;

                default:
                    throw new ApplicationException($"{this.Name}：定義外のStation:{station.ToString()}のGetFromToCodeの要求を検知しました。");
            }

        }
        #endregion

        #region GetRequireBitData
        /// <summary>
        /// 供給要求BITデータ取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetRequireBitData(Station st, string buffercode)
        {
            int bc;
            if (int.TryParse(buffercode, out bc) == false)
            {
                bc = 1;
            }

            switch (st)
            {
                case Station.Loader:
                    return this.Plc.GetBit(this.LoaderReqBitAddressList[bc]);

                default:
                    throw new ApplicationException($"{this.Name}：GetRequireBitDataで定義されていないStation:{st.ToString()}の要求を検知しました。lineconfigを確認して下さい。");
            }
        }
        #endregion

        /// <summary>
        /// 遠沈前置場の搬送可能なバッファ数を返す
        /// </summary>
        /// <returns></returns>
        private int GetCanInputBufferCount(ECK4MagExchanger exc)
        {
            int retV = 0;
            //供給要求信号の確認
            if (this.LoaderReqBitAddressList == null)
            {
                return retV;
            }

            if (this.LoaderReqBitAddressList.Count == 0)
            {
                return retV;
            }

            int existsCt = 0;
            foreach (KeyValuePair<int, string> kv in this.LoaderReqBitAddressList)
            {
                if (this.IsLMagazineExists(Station.Loader, kv.Key) == true)
                {
                    existsCt++;
                }
            }

            if (exc.IsExchangeLoaderMagazineExists() == true)
            {
                existsCt++;
            }

            // 搬送可能数 = 遠沈前置場数 - (遠沈前置場の空き数 + 移載機Loaderの空き数)
            retV = this.LoaderReqBitAddressList.Count() - existsCt;

            return retV;
        }

        public bool ExistsPairMagazine(Magazine mag)
        {
            AsmLot asmLot1 = AsmLot.GetAsmLot(mag.NascaLotNO);

            // 条件1 相方が他の装置の作業開始中実績を持たない
            Order[] orderList = Order.SearchOrder(null, null, this.MacNo, true, false);
            foreach (Order o in orderList)
            {
                // ロットNo(分割No含む)が同じ作業実績は無視
                if (o.LotNo == mag.NascaLotNO) continue;

                AsmLot asmLot2 = AsmLot.GetAsmLot(o.LotNo);
                if (asmLot2 == null) continue;

                // 他の装置に同じNascaLotNoの作業中実績があれば、投入NGとする
                if (asmLot1.NascaLotNo == asmLot2.NascaLotNo)
                {
                    return true;
                }
            }

            // 条件2 相方が他の装置の仮想マガジン『UnLoader(1～7)』にある
            foreach (var st in this.UnloaderStations)
            {
                var othermag = this.Peek(st);

                if (othermag == null) continue;
                // 同じマガジンNoの仮想マガジンは無視
                if (othermag.MagazineNo == mag.MagazineNo) continue;

                Magazine svrMag2 = Magazine.GetCurrent(othermag.MagazineNo);
                if (svrMag2 == null) continue;
                AsmLot asmLot2 = AsmLot.GetAsmLot(svrMag2.NascaLotNO);
                if (asmLot2 == null) continue;

                // 他の装置に同じNascaLotNoの仮想マガジンがあれば、投入NGとする
                if (asmLot1.NascaLotNo == asmLot2.NascaLotNo)
                {
                    return true;
                }
            }

            return false;
        } 
    }
}
