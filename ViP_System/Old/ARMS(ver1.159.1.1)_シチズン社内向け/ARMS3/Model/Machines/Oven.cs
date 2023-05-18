using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// オーブン
    /// </summary>
    public class Oven : MachineBase
    {
        /// <summary>
        /// 供給アドレス
        /// </summary>
        public SortedList<int, string> LoaderReqBitAddressList { get; set; }

        /// <summary>
        /// 排出アドレス
        /// </summary>
        public SortedList<int, string> UnloaderReqBitAddressList { get; set; }

        /// <summary>
        /// 供給FromToポイントリスト
        /// </summary>
        public SortedList<int, string> LoaderPointList { get; set; }

        /// <summary>
        /// 排出FromToポイントリスト
        /// </summary>
        public SortedList<int, string> UnloaderPointList { get; set; }

        public SortedList<int, Station> LoaderList { get; set; }

        public SortedList<int, Station> UnloaderList { get; set; }

        /// <summary>
        /// 現在のプロファイルアドレス
        /// </summary>
        public string CurrentProfileWordAddress { get; set; }

        /// <summary>
        /// プロファイル変更時にインターロックアドレス
        /// </summary>
        public string ChangeProfileInterlockBitAddress { get; set; }

        /// <summary>
        /// プロファイルOK、NG判定アドレス
        /// </summary>
        public string ProfileStatusAddress { get; set; }

        /// <summary>
        /// オーブンプロファイルNG時の値
        /// </summary>
        protected const int PROFILE_NG = 1;

        /// <summary>
        /// 硬化状態    0：待機  1：硬化中
        /// </summary>
        public string OvenProcessStatusWordAddress { get; set; }

        /// <summary>
        /// オーブン硬化状態の値
        /// </summary>
        protected const int OVEN_WORK = 1;

        /// <summary>
        /// 硬化状態を参照して、開始実績を登録する機能を使用するかどうか
        /// </summary>
        public bool IsUseOvenProcessStatus { get; set; }

        // MachineBaseクラスに移動
        ///// <summary>
        ///// 全扉閉中 (Robot3の搬送処理で使用)   0：開  1：閉
        ///// </summary>
        //public string OvenAllDoorCloseBitAddress { get; set; }

        /// <summary>
        /// オーブンの扉の開閉状態  (Robot3の搬送処理で使用) 0：閉  1：開
        /// </summary>
        public string OvenDoorOpenStatusBitAddress { get; set; }

        /// <summary>
        /// 停電時オーブン運転有   1：有り
        /// </summary>
        public string PowerCutsBitAddress { get; set; }

        public Oven()
            : base()
        {
            this.LoaderReqBitAddressList = new SortedList<int, string>();
            this.UnloaderReqBitAddressList = new SortedList<int, string>();
            this.LoaderPointList = new SortedList<int, string>();
            this.UnloaderPointList = new SortedList<int, string>();
            this.LoaderList = new SortedList<int, Station>();
            this.UnloaderList = new SortedList<int, Station>();

            this.LoaderList.Add(1, Station.Loader1);
            this.LoaderList.Add(2, Station.Loader2);
            this.LoaderList.Add(3, Station.Loader3);

            this.UnloaderList.Add(1, Station.Unloader1);
            this.UnloaderList.Add(2, Station.Unloader2);
            this.UnloaderList.Add(3, Station.Unloader3);
        }

        protected override void concreteThreadWork()
        {
            if (IsRequireOutput() == true)
            {
                workComplete();
            }

            // オーブン硬化中の場合、開始実績を登録
            if (IsRequireOvenProcessStatus() == true)
            {
                workStart();
            }

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();

            // 停電時オーブン運転確認
            checkOvenPowerCut();
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            try
            {
                foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
                {
                    string retv = Plc.GetBit(kv.Value);
                    if (retv == Mitsubishi.BIT_ON)
                    {
                        VirtualMag mag = this.Peek(this.LoaderList[kv.Key]);
                        if (mag != null)
                        {
                            //作業開始完了時間取得
                            mag.LastMagazineNo = mag.MagazineNo;
                            //mag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                            mag.WorkComplete = GetWorkCompleteTime();
                            //mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                            mag.WorkStart = GetWorkStartTime();

                            //プロファイルNGの判定
                            if (Plc.GetWordAsDecimalData(this.ProfileStatusAddress) == PROFILE_NG)
                            {
                                mag.PurgeReason = "オーブンプロファイル異常で排出";
                            }

                            //アンローダー側の仮想マガジンを先に全削除
                            //後から来たマガジン情報を優先
                            while (this.Dequeue(this.UnloaderList[kv.Key]) != null) { }

                            this.Enqueue(mag, this.UnloaderList[kv.Key]);
                            this.Dequeue(this.LoaderList[kv.Key]);

                            this.WorkComplete(mag, this, true);
                        }
                        else if (this.Peek(this.UnloaderList[kv.Key]) == null)
                        {
                            //アンローダーにも仮想マガジンが無い場合は空マガジンで埋めたと見做して排出を設定
                            mag = new VirtualMag();
                            mag.MagazineNo = VirtualMag.UNKNOWN_MAGNO;
                            mag.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
							mag.PurgeReason = "オーブン架空マガジンの排出";
                            this.Enqueue(mag, this.UnloaderList[kv.Key]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.RBLog.Info("", ex);
            }
        }

        /// <summary>
        /// 作業開始登録
        /// </summary>
        /// <param name="plc"></param>
        private void workStart()
        {
            //Log.SysLog.Info(this.Name + ": 作業開始登録処理");

            // オーブン内の全ロットの開始登録が正常にできたかどうか
            bool Success = true;

            MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
            Process process = null;

            // ローダーの全仮想マガジンに対して処理する。
            foreach (KeyValuePair<int, Station> kv in this.LoaderList)
            {
                // 仮想マガジンの確認
                VirtualMag vmag = this.Peek(this.LoaderList[kv.Key]);
                if (vmag == null) continue;

                process = Process.GetProcess(vmag.ProcNo.Value);
                    
                // 稼働中マガジンの確認
                Magazine mag = Magazine.GetCurrent(vmag.MagazineNo);
                if (mag == null) continue;

                AsmLot lot = mag != null ? AsmLot.GetAsmLot(mag.NascaLotNO) : null;

                // 作業開始実績の確認
                Order[] temps = Order.GetOrder(mag.NascaLotNO, vmag.ProcNo.Value);
                if (temps.Length > 0) continue;

                // 作業実績作成
                Order order = CommonApi.GetWorkStartOrder(vmag, this.MacNo);

                //ロットNo・作業開始時間取得
                order.LotNo = mag.NascaLotNO;
                //order.WorkStartDt = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                order.WorkStartDt = GetWorkStartTime();

                // 開始前の各種特性チェック、原材料チェック
                Log.ApiLog.Info(string.Format("[投入前判定開始]{0}-{1}", this.Name, order.LotNo));
                string msg;
                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, process, out msg);
                if (isError)
                {
                    Success = false;
                    Log.ApiLog.Info(string.Format("[投入前判定完了-結果NG]{0}-{1}: {2}", this.Name, order.LotNo, msg));
                    continue;
                }
                else
                {
                    Log.ApiLog.Info(string.Format("[投入前判定完了-結果OK]{0}-{1}", this.Name, order.LotNo));
                }

                //作業開始登録
                Log.ApiLog.Info(string.Format("[作業開始登録開始]{0}：LotNo={1}, Procno={2}", this.Name, order.LotNo, order.ProcNo));
                try
                {
                    order.DeleteInsert(order.LotNo);
                    Log.ApiLog.Info(string.Format("[作業開始登録完了]{0}：LotNo={1}, Procno={2}", this.Name, order.LotNo, order.ProcNo));
                }
                catch (Exception ex)
                {
                    Log.ApiLog.Info(string.Format("[作業開始登録失敗]{0}：LotNo={1}, Procno={2} \r\n{3}", this.Name, order.LotNo, order.ProcNo, ex.Message));
                    Success = false;
                }                    
            }

            if (Success)
            {
                // 硬化状態信号を停止にする。
                Plc.SetWordAsDecimalData(this.OvenProcessStatusWordAddress, 0);
                //Log.SysLog.Info( this.Name + ": 硬化状態信号(" + this.OvenProcessStatusWordAddress + ")を1→0へ");
            }
            else
            {
                throw new ApplicationException(string.Format("[作業開始処理失敗]{0}:画面上のログを確認して下さい。", this.Name));
            }

        }
        

        #region IsRequireOutput

        /// <summary>
        /// マガジン排出　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            //排出要求信号の確認
            if (this.UnloaderReqBitAddressList == null)
            {
                return false;
            }

            if (this.UnloaderReqBitAddressList.Count == 0)
            {
                return false;
            }

            
            //string retv = Plc.GetBit(this.UnloaderReqBitAddressList[1], UnloaderReqBitAddressList.Count);
            string retv;
            try
            {
                retv = Plc.GetBit(this.UnloaderReqBitAddressList[1], UnloaderReqBitAddressList.Count);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.UnloaderReqBitAddressList[1]}』, エラー内容：{ex.Message}");
                return false;
            }

            foreach (char bit in retv)
            {
                if (bit.ToString() == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region IsRequireInput

        /// <summary>
        /// マガジン供給　可否確認
        /// </summary>
        /// <returns></returns>
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

            //string retv = Plc.GetBit(this.LoaderReqBitAddressList[1], LoaderReqBitAddressList.Count);
            string retv;
            try
            {
                retv = Plc.GetBit(this.LoaderReqBitAddressList[1], LoaderReqBitAddressList.Count);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.LoaderReqBitAddressList[1]}』, エラー内容：{ex.Message}");
                return false;
            }

            foreach (char bit in retv)
            {
                if (bit.ToString() == Mitsubishi.BIT_ON)
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region IsRequireOvenProcessStatus

        /// <summary>
        /// マガジン硬化中　可否確認
        /// </summary>
        /// <returns></returns>
        private bool IsRequireOvenProcessStatus()
        {
            // 硬化中フラグ使用確認
            if (!this.IsUseOvenProcessStatus)
            {
                return false;
            }

            // 硬化中信号の設定確認
            if (this.OvenProcessStatusWordAddress == null)
            {
                return false;
            }

            // 硬化中信号の確認
            int ovenStatus = Plc.GetWordAsDecimalData(this.OvenProcessStatusWordAddress);

            if (ovenStatus == OVEN_WORK)
            {
                return true;
            }

            return false;
        }
        #endregion

        #region GetFromToCode

        public override string GetFromToCode(Station station)
        {
            foreach (KeyValuePair<int, Station> kv in LoaderList)
            {
                if (kv.Value == station)
                {
                    return this.LoaderPointList[kv.Key];
                }
            }

            foreach (KeyValuePair<int, Station> kv in UnloaderList)
            {
                if (kv.Value == station)
                {
                    return this.UnloaderPointList[kv.Key];
                }
            }

            throw new ApplicationException("定義外のStationのGetFromToCode");
        }
        #endregion
        
        #region GetFromBufferCode        
        /// <summary>
        /// 排出側バッファ位置取得 (Robot3用 ステーション指定)
        /// </summary>
        /// <returns></returns>
        public override string GetFromToBufferCode(Station station)
        {
            foreach (KeyValuePair<int, Station> kv in LoaderList)
            {
                if (kv.Value == station)
                {
                    return kv.Key.ToString();
                }
            }

            foreach (KeyValuePair<int, Station> kv in UnloaderList)
            {
                if (kv.Value == station)
                {
                    return kv.Key.ToString();
                }
            }
            throw new ApplicationException("定義外のStationのGetFromToBufferCode");
        }
        #endregion

        /// <summary>
        /// 現在動作中のプロファイル取得
        /// </summary>
        /// <returns></returns>
        public int GetCurrentProfile()
        {
            if (string.IsNullOrEmpty(this.CurrentProfileWordAddress))
            {
                return 0;
            }

            return Plc.GetWordAsDecimalData(this.CurrentProfileWordAddress);
        }

        /// <summary>
        /// プロファイル変更のインタロック
        /// </summary>
        /// <param name="on"></param>
        /// <param name="plc"></param>
        public void SetInterlock(bool on)
        {
            if (string.IsNullOrEmpty(this.ChangeProfileInterlockBitAddress))
            {
                return;
            }

            if (on == true)
            {
                Plc.SetBit(this.ChangeProfileInterlockBitAddress, 1, Mitsubishi.BIT_ON);
            }
            else
            {
                Plc.SetBit(this.ChangeProfileInterlockBitAddress, 1, Mitsubishi.BIT_OFF);
            }
        }

        #region GetLoaderLocation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Location GetLoaderLocation()
        {
            foreach (KeyValuePair<int, string> kv in this.LoaderReqBitAddressList)
            {
                string retv = Plc.GetBit(kv.Value);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, this.LoaderList[kv.Key]);
                }
            }

            //投入先が無ければ排出コンベヤを返す
            Log.RBLog.Info("オーブンで投入先が埋まった状態で投入場所の問い合わせ発生。排出コンベヤをセット");
            IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
            return dischargeConveyor.GetLoaderLocation();
        }

        #endregion

        #region GetUnloaderLocation

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override Location GetUnLoaderLocation()
        {
            foreach (KeyValuePair<int, string> kv in this.UnloaderReqBitAddressList)
            {
                string retv = Plc.GetBit(kv.Value);

                if (retv == Mitsubishi.BIT_ON)
                {
                    return new Location(this.MacNo, this.UnloaderList[kv.Key]);
                }
            }

            //投入先が無ければnull
            Log.RBLog.Info("オーブンで排出元が無い状態で排出元の問い合わせ発生。");
            return null;
        }
        #endregion

        #region IsDoorOpenStatus
        /// <summary>
        /// オーブンの扉が開いているか
        /// </summary>
        /// <returns></returns>
        public bool IsDoorOpenStatus()
        {
            //扉開信号の確認
            if (string.IsNullOrWhiteSpace(this.OvenDoorOpenStatusBitAddress) == true)
            {
                return false;
            }

            string retv = Plc.GetBit(this.OvenDoorOpenStatusBitAddress);
            
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }

            return false;
        }
        #endregion
        
        #region GetRequireBitData
        public override string GetRequireBitData(Station st, string buffercode)
        {
            foreach (KeyValuePair<int, Station> kv in LoaderList)
            {
                if(kv.Value == st)
                {
                    return Plc.GetBit(LoaderReqBitAddressList[kv.Key]);
                }
            }

            foreach (KeyValuePair<int, Station> kv in UnloaderList)
            {
                if (kv.Value == st)
                {
                    return Plc.GetBit(UnloaderReqBitAddressList[kv.Key]);
                }
            }

            throw new ApplicationException($"定義外のStationのGetRequireBitData：{st.ToString()}");
        }
        #endregion

        #region checkOvenPowerCut
        private void checkOvenPowerCut()
        {
            if (string.IsNullOrWhiteSpace(this.PowerCutsBitAddress) == true)
            {
                return;
            }

            // 信号がONの場合、エラー表示 + 監視停止
            if (this.Plc.GetBit(this.PowerCutsBitAddress) == Mitsubishi.BIT_ON)
            {
                string errMsg = $"『停電時オーブン運転有』の信号がONになりました。オーブン制御盤のエラー画面を確認して下さい。";
                FrmErrHandle frmErr = new FrmErrHandle(errMsg, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw new Exception(errMsg);
                }
            }
        }
        #endregion
    }
}
