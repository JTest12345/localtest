using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ギ酸リフロー装置
    /// </summary>
    public class FormicAcidReflow : MachineBase
    {
        #region 定数

        /// <summary>
        /// プログラム取得用のCondCd
        /// </summary>
        private const string PROGRAM_COND_CD = "15";

        #endregion

        #region PLCアドレス定義
        /// <summary>
        /// 1基板毎のアドレスオフセット量
        /// </summary>
        private const int FRAME_ADDR_OFFSET =100;

        /// <summary>
        /// 投入側マガジンNo取得アドレス (ZR60000)
        /// </summary>
        private const string IN_MAG_NO_ADDR = "ZR00EA60";
        /// <summary>
        /// 投入側マガジンNo取得アドレス (ZR60050)
        /// </summary>
        private const string OUT_MAG_NO_ADDR = "ZR00EA92";
        private const int MAG_NO_ADDR_LENGTH = 12;

        /// <summary>
        /// プログラム取得アドレス (ZR60100)
        /// </summary>
        private const string PROGRAM_ADDR = "ZR00EAC4";
        private const int PROGRAM_ADDR_LENGTH = 12;


        /// <summary>
        /// アドレスのヘッダ
        /// </summary>
        private const string ADDR_HEADER = "ZR";
        /// <summary>
        /// 基板の開始時刻アドレス(年) 基板1枚目
        /// </summary>
        private const int WORK_START_YEAR_ADDR_START = 62130;
        /// <summary>
        /// 基板の完了時刻アドレス(年) 基板1枚目
        /// </summary>
        private const int WORK_END_YEAR_ADDR_START = 62180;
        /// <summary>
        /// 基板情報(DMコード)アドレス 基板1枚目
        /// </summary>
        private const int DATAMATRIX_ADDR_START = 62100;
        private const int DATAMATRIX_ADDR_LENGTH = 12;
        /// <summary>
        /// 基板のキャリア投入位置 基板1枚目
        /// </summary>
        private const int CARRIER_POS_ADDR_START = 62190;

        /// <summary>
        /// 基板枚数上限
        /// </summary>
        private const int MAX_FRAME_CT = 40;

        private const string MACHINE_READY_BIT_ADDR = "B000240";
        private const string INPUT_REQ_BIT_ADDR = "B000200";
        private const string STOCK_REQ_BIT_ADDR = "B000210";

        private const string INPUT_OK_BIT_ADDR = "B000220";
        private const string INPUT_NG_BIT_ADDR = "B000221";
        private const string STOCK_RECIVE_BIT_ADDR = "B000230";

        #endregion

        public FormicAcidReflow() { }

        /// <summary>
        /// メインルーチン
        /// </summary>
        protected override void concreteThreadWork()
        {
            WorkComplete();

            WorkStart();
        }

        #region 要求信号判定

        private bool IsInputRequest()
        {
            if (this.Plc.GetBit(INPUT_REQ_BIT_ADDR) == PLC.Common.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool IsStockRequest()
        {
            if (this.Plc.GetBit(STOCK_REQ_BIT_ADDR) == PLC.Common.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        /// <summary>
        /// 高生産性 作業完了
        /// </summary>
        public void WorkStart()
        {
            if (IsInputRequest() == false) return;

            try
            {
                string magno = this.Plc.GetWord(IN_MAG_NO_ADDR, MAG_NO_ADDR_LENGTH).Replace("\0", "");
                if (string.IsNullOrWhiteSpace(magno))
                {
                    throw new ApplicationException($"投入マガジンの取得に失敗しました。IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス『{IN_MAG_NO_ADDR}』");
                }
                string[] elms = magno.Split(' ');
                if(elms.Length > 1)
                {
                    magno = elms[1];
                }
                Magazine svrmag = Magazine.GetCurrent(magno);
                if (svrmag == null)
                {
                    throw new ApplicationException($"稼働中ではないマガジンです。マガジンNo『{magno}』");
                }

                string progNo = this.Plc.GetWord(PROGRAM_ADDR, PROGRAM_ADDR_LENGTH).Replace("\0", "");
                if (string.IsNullOrWhiteSpace(progNo))
                {
                    throw new ApplicationException($"製造プログラムの取得に失敗しました。IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス:『{PROGRAM_ADDR}』");
                }

                AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                if (lot == null)
                {
                    throw new ApplicationException($"アッセンロット情報が見つかりません。ロットNo『{svrmag.NascaLotNO}』");
                }
                Process p = Process.GetNextProcess(svrmag.NowCompProcess, lot);
                if (p == null)
                {
                    throw new ApplicationException($"次工程情報が見つかりません。ロットNo『{lot.NascaLotNo}』,現完了工程No『{svrmag.NowCompProcess}』");
                }

                // この時点での現在完了工程と次作業( = ダミー実績候補の作業)を記録 → 作業規制NG時のダミー実績削除処理に使用
                int nowprocno = svrmag.NowCompProcess;
                int dummyprocno = p.ProcNo;
                // 次作業のダミー実績登録判定 + 判定OK時にダミー実績登録
                bool insertDummyTranFg = dummyTranCheckAndInsert(lot, svrmag, ref p);

                string condVal = WorkCondition.GetTypeCondVal(lot.TypeCd, PROGRAM_COND_CD, p.WorkCd);
                if (condVal != progNo)
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(svrmag, dummyprocno, nowprocno);
                    }
                    throw new ApplicationException($"プログラム照合不一致,プログラム名『{progNo}』,条件値『{condVal}』");
                }

                bool isInsertOrder = true;
                Order[] currentOrders = Order.SearchOrder(lot.NascaLotNo, p.ProcNo, null, false, false);
                foreach(Order o in currentOrders)
                {
                    if(o.WorkEndDt.HasValue)
                    {
                        // ダミー実績を登録した際は、削除 + 巻き戻し
                        if (insertDummyTranFg == true)
                        {
                            Order.ReturnDummyWork(svrmag, dummyprocno, nowprocno);
                        }
                        // 完了済み実績がある場合、NG
                        throw new ApplicationException($"完了実績が既に登録されています。ロットNo『{o.LotNo}』,工程No『{o.ProcNo}』");
                    }
                    else if(o.MacNo != this.MacNo)
                    {
                        // ダミー実績を登録した際は、削除 + 巻き戻し
                        if (insertDummyTranFg == true)
                        {
                            Order.ReturnDummyWork(svrmag, dummyprocno, nowprocno);
                        }
                        // 別の装置での開始実績がある場合、NG
                        throw new ApplicationException($"別装置の開始実績が既に登録されています。ロットNo『{o.LotNo}』,工程No『{o.ProcNo}』,装置No『{o.MacNo}』");
                    }
                    else
                    {
                        // 同じ装置の開始実績がある場合は、実績の登録処理はしない
                        isInsertOrder = false;
                    }
                }

                // 以下、開始実績を新規登録
                Order order = new Order();
                order.LotNo = svrmag.NascaLotNO;
                order.ProcNo = p.ProcNo;
                order.InMagazineNo = svrmag.MagazineNo;
                order.MacNo = this.MacNo;
                order.WorkStartDt = DateTime.Now;
                order.WorkEndDt = null;
                order.TranStartEmpCd = "660";
                order.TranCompEmpCd = "660";
                order.IsDefectEnd = true;

                MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
                if (machine == null)
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(svrmag, dummyprocno, nowprocno);
                    }
                    throw new ApplicationException($"装置情報が見つかりません。装置番号『{this.MacNo}』");
                }

                string errMsg;
                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
                if (!isError)
                {
                    if (isInsertOrder == true)
                    {
                        order.DeleteInsert(order.LotNo);
                        this.OutputApiLog($"[開始処理] 正常完了『{svrmag.NascaLotNO}』");
                    }
                    else
                    {
                        this.OutputApiLog($"[開始処理] 開始済みの為実績登録はスキップ『{svrmag.NascaLotNO}』");
                    }

                    //NASCA開始OKをPLCに書き込み
                    Plc.SetBit(INPUT_OK_BIT_ADDR, 1, PLC.Common.BIT_ON);
                    this.OutputSysLog($"NASCA開始OK信号にON書込み：IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス『{INPUT_OK_BIT_ADDR}』");
                }
                else
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(svrmag, dummyprocno, nowprocno);
                    }
                    //NASCA開始NGをPLCに書き込み
                    throw new ApplicationException(errMsg);
                }
            }
            catch (ApplicationException ex)
            {
                //NASCA開始NGをPLCに書き込み
                Plc.SetBit(INPUT_NG_BIT_ADDR, 1, PLC.Common.BIT_ON);
                this.OutputSysLog($"NASCA開始NG信号にON書込み：IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス『{INPUT_NG_BIT_ADDR}』");
                throw new ApplicationException($"[開始登録チェックNG] 理由『{ex.Message}』");
            }
            catch (Exception ex)
            {
                //NASCA開始NGをPLCに書き込み
                Plc.SetBit(INPUT_NG_BIT_ADDR, 1, PLC.Common.BIT_ON);
                this.OutputSysLog($"NASCA開始NG信号にON書込み：IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス『{INPUT_NG_BIT_ADDR}』");
                throw new Exception($"[開始登録異常] 理由『{ex.Message}』");
            }

        }

        /// <summary>
        /// 高生産性 作業完了
        /// </summary>
        public void WorkComplete()
        {
            if (IsStockRequest() == false) return;
            
            // アドレスからマガジンNoを取得
            string newmagno = this.Plc.GetWord(OUT_MAG_NO_ADDR, MAG_NO_ADDR_LENGTH).Replace("\0", "");
            if (string.IsNullOrWhiteSpace(newmagno))
            {
                throw new ApplicationException($"投入マガジンの取得に失敗しました。IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス『{OUT_MAG_NO_ADDR}』");
            }
            string[] elms = newmagno.Split(' ');
            if (elms.Length > 1)
            {
                newmagno = elms[1];
            }
            Magazine svrmag = Magazine.GetCurrent(newmagno);
            if (svrmag == null)
            {
                throw new ApplicationException($"稼働中ではないマガジンです。マガジンNo『{newmagno}』");
            }

            AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            if (lot == null)
            {
                throw new ApplicationException($"アッセンロット情報が見つかりません。ロットNo『{svrmag.NascaLotNO}』");
            }

            List<StockerData> sdList = GetStockerData();
            if (sdList.Count == 0)
            {
                throw new ApplicationException($"搭載情報の取得に失敗しました。IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』");
            }

            VirtualMag mag = new VirtualMag();
            VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Unloader));

            //既にキュー内に存在するかを確認
            bool found = false;
            foreach (VirtualMag exist in mags)
            {
                if (exist.MagazineNo == mag.MagazineNo)
                {
                    // 登録済みの仮想マガジン情報を使用する。
                    mag = exist;
                    found = true;
                }
            }

            if (found == false)
            {
                // 初回登録時
                mag.MagazineNo = newmagno;
                mag.LastMagazineNo = newmagno;

                Process p = Process.GetNextProcess(svrmag.NowCompProcess, lot);
                mag.ProcNo = p.ProcNo;

                mag.WorkStart = StockerData.GetWorkStartDt(sdList);
                mag.WorkComplete = StockerData.GetWorkEndDt(sdList);
                
                //段数データ等を登録
                RegisterCarrierData(sdList, mag, lot);

                this.Enqueue(mag, Station.Unloader);
            }
            else
            {
                // 同一の仮想マガジン登録済みの場合
                mag.WorkComplete = StockerData.GetWorkEndDt(sdList);
                
                //段数データ等を登録
                RegisterCarrierData(sdList, mag, lot);

                mag.Updatequeue();
            }


            //受信完了ONをPLCに書き込み
            this.OutputSysLog($"受信完了ON書込み：IPアドレス『{this.Plc.IPAddress}』,ポート『{this.Plc.Port}』,アドレス『{STOCK_RECIVE_BIT_ADDR}』");
            this.Plc.SetBit(STOCK_RECIVE_BIT_ADDR, 1, Mitsubishi.BIT_ON);

            this.OutputApiLog($"[完了処理] 正常完了『{svrmag.NascaLotNO}』");
        }

        #region GetStockerData

        private List<StockerData> GetStockerData()
        {
            List<StockerData> retv = new List<StockerData>();

            for (int i = 0; i < MAX_FRAME_CT; i++)
            {
                StockerData sd = new StockerData();

                // 基板DMを取得して0ならcontinue
                sd.DataMatrix = StockerData.GetDateMatrix(this.Plc, DATAMATRIX_ADDR_START, DATAMATRIX_ADDR_LENGTH, i);
                if(string.IsNullOrWhiteSpace(sd.DataMatrix) == true || sd.DataMatrix == "0")
                {
                    continue;
                }
                
                sd.CarrierPos = this.Plc.GetWordAsDecimalData(StockerData.ToAdr(CARRIER_POS_ADDR_START, i)); 

                sd.WorkStartDt = StockerData.GetDateTime(this.Plc, WORK_START_YEAR_ADDR_START, i);

                sd.WorkEndDt = StockerData.GetDateTime(this.Plc, WORK_END_YEAR_ADDR_START, i);

                sd.step = i + 1;
                                
                // 開始時刻と完了時刻の一方がNullならThrow
                if(sd.WorkStartDt.HasValue == false)
                {
                    throw new ApplicationException($"基板情報(DMコード)が入っているのに開始時間のデータがありません。Mg段数『{(i + 1).ToString()}』");
                }
                if (sd.WorkEndDt.HasValue == false)
                {
                    throw new ApplicationException($"基板情報(DMコード)が入っているのに終了時間のデータがありません。Mg段数『{(i + 1).ToString()}』");
                }

                retv.Add(sd);
            }

            return retv;
        }
        #endregion

        public class StockerData
        {
            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }
            public string DataMatrix { get; set; }
            public decimal CarrierPos { get; set; }
            public int step { get; set; }

            #region 
            public StockerData() { }
            
            public static string ToAdr(int baseAddress, int frameNo)
            {
                return ADDR_HEADER
                    + (baseAddress + (frameNo * FRAME_ADDR_OFFSET)).ToString("X6");
            }

            public static DateTime? GetDateTime(IPLC plc, int yearAddress, int frameNo)
            {
                string adr = ToAdr(yearAddress, frameNo);
                try
                {
                    DateTime? dt = plc.GetWordsAsDateTime(adr);
                    return dt;
                }
                catch (Exception)
                {
                    // Log.SysLog.Info("蟻酸リフロー装置の作業時間取得エラー:" + frameNo.ToString());
                    //Log.SysLog.Info("応急処置としてNULLをセット");
                    return null;
                }
            }

            public static string GetDateMatrix(IPLC plc, int baseAddress, int length, int frameNo)
            {
                string adr = ToAdr(baseAddress, frameNo);
                try
                {
                    string retv = plc.GetWord(adr, length).Replace("\0", "");
                    return retv;
                }
                catch (Exception)
                {
                    return null;
                }
            }

            #endregion

            public static DateTime GetWorkStartDt(List<StockerData> sdlist)
            {
                //return sdlist.Where(d => d.WorkStartDt.HasValue).OrderBy(d => d.WorkStartDt.Value).First().WorkStartDt.Value;
                return sdlist.OrderBy(d => d.WorkStartDt.Value).First().WorkStartDt.Value;
            }

            public static DateTime GetWorkEndDt(List<StockerData> sdlist)
            {
                //return sdlist.Where(d => d.WorkEndDt.HasValue).OrderByDescending(d => d.WorkEndDt.Value).First().WorkEndDt.Value;
                return sdlist.OrderByDescending(d => d.WorkEndDt.Value).First().WorkEndDt.Value;
            }

        }

        /// <summary>
        /// 基板段数及びポジションデータをデータベースに登録
        /// </summary>
        /// <param name="startAddr"></param>
        /// <param name="distance"></param>
        /// <param name="MaxCt"></param>
        /// <returns></returns>
        private void RegisterCarrierData(List<StockerData> stockDataList, VirtualMag mag, AsmLot lot)
        {
            //int loadStepCd = ArmsApi.Model.LENS.Mag.GetMagStepCd(lot.TypeCd).Value;

            CarrireWorkData regData = new CarrireWorkData();
            regData.LotNo = lot.NascaLotNo;
            regData.ProcNo = Convert.ToInt32(mag.ProcNo);
            regData.Delfg = 0;
            regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            foreach(StockerData data in stockDataList)
            {
                regData.CarrierNo = data.DataMatrix;

                int step = data.step;

                //装置によって積み方が違ってマスタで対応できないので無しに変更。（レポーター側で対応する）
                ////段数情報はPLCから抜く場合元々搭載しない段もカウントしてしまうので、それを除外して集計する。
                ////loadStepCdは1が偶数段のみ、2が奇数段のみ
                //if (loadStepCd == 1)
                //{
                //    step = step / 2;
                //}
                //else if (loadStepCd == 2)
                //{
                //    step = (step + 1) / 2;
                //}
                //else if (loadStepCd != 3)
                //{
                //    continue;
                //}


                //段数登録
                regData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;
                regData.Value = Convert.ToString(step);
                regData.InsertUpdate();

                //キャリア投入位置を設定
                regData.Infoid = CarrireWorkData.IN_SURFACE_ADDR_INFOCD;
                regData.Value = Convert.ToString(data.CarrierPos);
                regData.InsertUpdate();
            }            
        }
    }
}
