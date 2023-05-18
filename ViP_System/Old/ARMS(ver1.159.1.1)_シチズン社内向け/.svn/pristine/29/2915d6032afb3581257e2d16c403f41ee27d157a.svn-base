using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Threading;
using ARMS3.Model.PLC;
using System.Text.RegularExpressions;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// モールド
    /// </summary>
    public class Mold : MachineBase
    {
        /// <summary>
        /// 直近100ロットの完成履歴
        /// </summary>
        public Queue<string> preCompleteLotQueue = new Queue<string>();

        /// <summary>
        /// アンローダーマガジン有無確認の最初のアドレス
        /// </summary>
        public string UnloaderReqBitAddressStart { get; set; }

        /// <summary>
        /// マガジン情報全体の開始アドレス
        /// </summary>
        public string UnloaderMagazineDataAddressStart { get; set; }

        /// <summary>
        /// 終了時排出マガジンNo取得アドレス
        /// </summary>
        public string ULMagazineAddress { get; set; }

        /// <summary>
        /// 開始時マガジンNo取得アドレス
        /// </summary>
        public string LMagazineAddress { get; set; }

        /// <summary>
        /// 空マガジン排出STのマガジンNo取得アドレス
        /// </summary>
        public string EULMagazineAddress { get; set; }

        /// <summary>
        /// ローダー側マガジンNo取得アドレス(追加)
        /// </summary>
        public string LoaderMagazineAddress { get; set; }

        /// <summary>
        /// ローダー側マガジンNo読取完了アドレス(追加)
        /// </summary>
        public string LoaderQRReadCompleteBitAddress { get; set; }

        /// <summary>
        /// ローダー側空マガジン搭載数量アドレス
        /// </summary>
        public string ELMagazineCountAddress { get; set; }

        /// <summary>
        /// ローダー側実マガジン搭載数量アドレス
        /// </summary>
        public string LMagazineCountAddress { get; set; }

        /// <summary>
        /// アンローダー側空マガジン搭載数量アドレス
        /// </summary>
        public string ULMagazineCountAddress { get; set; }

        /// <summary>
        /// アンローダー側実マガジン搭載数量アドレス
        /// </summary>
        public string EULMagazineCountAddress { get; set; }

        #region アンローダーマガジン情報読み込みアドレス定数

        private const int UNLOADER_DATA_LENGTH = 128;

        private const int WORK_START_ADDRESS_OFFSET = 0;

        private const int WORK_COMPLETE_ADDRESS_OFFSET = 6;

        private const int MAGAZINE_NO_OFFSET = 16;

        private const int MAGAZINE_NO_LENGTH = 7;

        private const int DATE_TIME_ADDRESS_LENGTH = 6;

        #endregion

        /// <summary>
        /// アンローダーマガジン数
        /// </summary>
        private const int UNLOADER_MAGAZINE_COUNT = 4;

        /// <summary>
        /// 1マガジンデータ長
        /// </summary>
        private const int MAGAZINE_DATA_LENGTH = 32;

        /// <summary>
        /// NASCA開始OK登録Bit
        /// </summary>
        public string NASCA_OK_BIT { get; set; }

        /// <summary>
        /// NASCA開始NG登録Bit
        /// </summary>
        public string NASCA_NG_BIT { get; set; }

        /// <summary>
        /// NASCA開始NGワード
        /// </summary>
        public string NASCA_NG_REASON_WORD { get; set; }

		///// <summary>
		///// SDファイル保存パス
		///// </summary>
		//public string SDFilePath { get; set; }

		/// <summary>
		/// QRReder有/無
		/// </summary>
		public bool IsQRReader { get; set; }

        /// <summary>
        /// マガジン番号WORDアドレスの長さ(自動化)
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH_AUTO = 6;

        #region 装置ログ関連

        private const int DATA_NO_COL = 0;
		private const int MAG_NO_COL = 3;

        /// <summary>
        /// 各ファイルの傾向管理フラグ列
        /// </summary>
        /// <returns></returns>
        private const int SM_KEIKOUFG_COL_INDEX = 7;
        private const int AM_KEIKOUFG_COL_INDEX =14;
        private const int EM_KEIKOUFG_COL_INDEX =28;
        private const int SD_KEIKOUFG_COL_INDEX =18;

        #endregion


        /// <summary>
        /// 吐出圧力
        /// </summary>
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_1 = "W3FC0";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_2 = "W3FC1";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_3 = "W3FC2";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_4 = "W3FC3";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_5 = "W3FC4";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_6 = "W3FC5";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_7 = "W3FC6";
        private const string DISCHARGE_PRESSURE_DATA_ADDRESS_8 = "W3FC7";

        //private const int DISCHARGE_PRESSURE_DATA_WORD_LENGTH = 8;

        /// <summary>
        /// 設備番号
        /// </summary>
        private const string PLANT_CD_ADDRESS = "W3FB0";
        private const int PLANT_CD_WORD_LENGTH = 4;

        /// <summary>
        /// 変更吐出圧力
        /// </summary>
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_1 = "W3FE0";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_2 = "W3FE1";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_3 = "W3FE2";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_4 = "W3FE3";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_5 = "W3FE4";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_6 = "W3FE5";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_7 = "W3FE6";
        private const string RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_8 = "W3FE7";

        /// <summary>
        /// 吐出圧力転送完了
        /// </summary>
        private const string RESULT_SEND_COMPLETE = "B3FD4";

        /// <summary>
        /// 設備番号照合OK
        /// </summary>
        private const string PLANT_VERIFICATION_OK_ADDRESS = "B3FD2";
        /// <summary>
        /// 設備番号照合NG
        /// </summary>
        private const string PLANT_VERIFICATION_NG_ADDRESS = "B3FD3";

        /// <summary>
        /// 設備番号照合結果ファイル保存パスがリセットされているかフラグ
        /// </summary>


        public Mold() { }

        protected override void concreteThreadWork()
        {
            try
            {
                //はじめに1回だけ設備番号照合結果ファイルのごみデータをoldに移動する
                if (IsInitialized == false)
                {
                    IsInitialized = true;
                    //設備番号照合結果ファイルのごみデータをoldに移動する
                    moveOldFilePlantVerification();
                }

                if (this.IsAutoLine)
                {
                    if (base.IsRequireOutput() == true)
                    {
                        workComplete();
                    }
                }
                else
                {
                    if (base.IsRequireOutput() == true)
                    {
                        if (IsQRReader)
                        {
                            workCompletehigh();
                        }
                        else
                        {
                            workCompletehigh2();
                        }
                    }

                    workStart();
                }

                CheckMachineLogFile();
                //if (Config.Settings.IsMappingMode)
                //{
                //	// SDファイル処理
                //	SdComplete();
                //}

                // 仮想マガジン消去要求応答
                ResponseClearMagazineRequest();

                #region MD反射材3D測定機用の処理
                //接続要求時
                if (base.IsRequireConnection() == true)
                {
                    base.ConnectionProcess();
                }

                //出力要求時
                if (base.IsRequireReadDischargePressureData() == true)
                {
                    readDischargePressureData();
                }

                //測定器からの結果書込完了時
                if (ArmsApi.Model.MoldDischargePressure.IsDataReceived(this.PlantCd) == true)
                {
                    sendDischargePressureData();
                }

                //切断要求時
                if (base.IsRequireDisconnect() == true)
                {
                    base.DisconnectionProcess();
                }

                //設備番号照合結果ファイル処理
                checkPlantVerificationFile();
                #endregion
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw new Exception(ex.Message, ex);
                }
            }
        }

        #region workComplete

        /// <summary>
        /// 作業完了
        /// </summary>
        public void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress, MAGAZINE_NO_WORD_LENGTH_AUTO);
            Log.RBLog.Info($"{this.Name} 完了登録処理開始");

            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info($"{this.Name} 排出マガジンNOの取得に失敗 [取得アドレス：{ULMagazineAddress}]");
                return;
            }

            string oldmagno = Plc.GetMagazineNo(LMagazineAddress, MAGAZINE_NO_WORD_LENGTH_AUTO);
            if (string.IsNullOrEmpty(oldmagno) == false)
            {
                newMagazine.LastMagazineNo = oldmagno;
            }
            else
            {
                Log.RBLog.Info($"{this.Name} 搬入側マガジンNOの取得に失敗 [取得アドレス：{LMagazineAddress}]");
                return;
            }

			//作業IDを取得
			newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, oldmagno);

            //作業開始完了時間取得
            try
            {
                newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                throw new ApplicationException($"{this.Name} 作業完了時間の取得に失敗 [取得アドレス：{this.WorkCompleteTimeAddress}]");
            }

            try
            {
                newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
            }
            catch
            {
                throw new ApplicationException($"{this.Name} 作業開始時間の取得に失敗 [取得アドレス：{this.WorkStartTimeAddress}]");
            }

            VirtualMag oldmag = new VirtualMag();
            oldmag.MagazineNo = oldmagno;
            oldmag.LastMagazineNo = oldmagno;

			this.Enqueue(oldmag, Station.EmptyMagazineUnloader);
            this.Enqueue(newMagazine, Station.Unloader);

            this.WorkComplete(newMagazine, this, true);
        }

        /// <summary>
        /// 作業完了(高効率)
        /// </summary>
        public void workCompletehigh()
        {
            // アンローダー各位置のマガジン有無情報を全て取得
            string[] magazineExists = Plc.GetBitArray(this.UnloaderReqBitAddressStart, UNLOADER_MAGAZINE_COUNT);

            // アンローダーの全マガジンのデータを全て取得
            string[] raw = Plc.GetBitArray(this.UnloaderMagazineDataAddressStart, UNLOADER_DATA_LENGTH);

            if (magazineExists == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY || raw == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY)
            {
                return;
            }

            for (int i = 0; i < magazineExists.Length; i++)
            {
                // 実体がある位置のデータを小分けにしてマガジン情報に展開
                if (magazineExists[i] == Mitsubishi.BIT_ON)
                {
                    VirtualMag mag = parseMagazine(raw, i * MAGAZINE_DATA_LENGTH);
                    if (mag == null)
                    {
                        continue;
                    }

                    Magazine curMag = Magazine.GetCurrent(mag.MagazineNo);

					// 作業IDを取得
					int procno = Order.GetLastProcNo(this.MacNo, mag.MagazineNo);

                    // 直近100ロットの完成リストに存在する場合は何もしない
                    //if (preCompleteLotQueue.Contains(string.Format("{0},{1}", mag.MagazineNo, procno)) == true)
                    if (preCompleteLotQueue.Contains(string.Format("{0},{1}", curMag.NascaLotNO, procno)) == true)
                    {
                        continue;
                    }

                    // Enqueueが先にないとLoc情報が無い
                    this.Enqueue(mag, Station.Unloader);

                    // 高効率でArmsWebが作成してしまうので削除
                    this.Dequeue(Station.Loader);

                    // 直近100マガジンの完成記憶
                    //preCompleteLotQueue.Enqueue(string.Format("{0},{1}", mag.MagazineNo, procno));
                    preCompleteLotQueue.Enqueue(string.Format("{0},{1}", curMag.NascaLotNO, procno));
                    if (preCompleteLotQueue.Count >= 100)
                    {
                        preCompleteLotQueue.Dequeue();
                    }
                }
            }
        }

		/// <summary>
		/// 作業完了(高効率) QRリーダー無し
		/// </summary>
		public void workCompletehigh2()
		{
			// アンローダー各位置のマガジン有無情報を全て取得
			string[] magazineExists = Plc.GetBitArray(this.UnloaderReqBitAddressStart, UNLOADER_MAGAZINE_COUNT);

			// アンローダーの全マガジンのデータを全て取得
			string[] raw = Plc.GetBitArray(this.UnloaderMagazineDataAddressStart, UNLOADER_DATA_LENGTH);

			if (magazineExists == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY || raw == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY)
			{
				return;
			}

			for (int i = 0; i < magazineExists.Length; i++)
			{
				// 実体がある位置のデータを小分けにしてマガジン情報に展開
				if (magazineExists[i] == Mitsubishi.BIT_ON)
				{
					VirtualMag mag = parseMagazine(raw, i * MAGAZINE_DATA_LENGTH);
					if (mag == null)
					{
						continue;
					}

					// 作業IDを取得
					int procno = Order.GetLastProcNo(this.MacNo, mag.MagazineNo);

					List<VirtualMag> ulMagazines = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Unloader)).ToList();
					if (ulMagazines.Where(m => m.WorkStart.Value == mag.WorkStart && m.WorkComplete.Value == mag.WorkComplete).Count() != 0)
					{
						continue;
					}

					if (this.Enqueue(mag, Station.Unloader))
					{
						// 高効率でArmsWebが作成してしまうので削除
						this.Dequeue(Station.Loader);
					}
				}
			}
		}

        #endregion

		/// <summary>
        /// 作業開始登録
        /// </summary>
        /// <param name="plc"></param>
        private void workStart()
        {
            //開始処理（NASCA自動登録）
            if (string.IsNullOrEmpty(this.LoaderQRReadCompleteBitAddress) == true)
            {
                return;
            }

            try
            {
                if (Plc.GetBit(this.LoaderQRReadCompleteBitAddress) != Mitsubishi.BIT_ON)
                {
                    return;
                }

                this.OutputApiLog("作業開始登録処理開始:");

                // 開始時は必ず1マガジン想定
                string mgno = Plc.GetMagazineNo(LoaderMagazineAddress, true);

                Magazine mag = Magazine.GetCurrent(mgno);
                if (mag == null) 
                {
                    throw new Exception($"{this.Name} [開始登録異常発生]稼働中ではないマガジンです。マガジンNo:『{mgno}』");
                }

                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                MachineInfo machine = MachineInfo.GetMachine(this.MacNo);

                // この時点での現在完了工程と次作業( = ダミー実績候補の作業)を記録 → 作業規制NG時のダミー実績削除処理に使用
                int nowprocno = mag.NowCompProcess;
                int dummyprocno = p.ProcNo;
                // 次作業のダミー実績登録判定 + 判定OK時にダミー実績登録
                bool insertDummyTranFg = dummyTranCheckAndInsert(lot, mag, ref p);

                Order order = new Order();
                order.LotNo = mag.NascaLotNO;
                order.ProcNo = p.ProcNo;
                order.InMagazineNo = mag.MagazineNo;
                order.MacNo = this.MacNo;
                order.WorkStartDt = DateTime.Now;
                order.WorkEndDt = null;
                order.TranStartEmpCd = "660";
                order.TranCompEmpCd = "660";

				if (p.IsNascaDefect)
				{
					order.IsDefectEnd = false;
				}
				else
				{
					order.IsDefectEnd = true;
				}

                string errMsg;
                
                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
                if (!isError)
                {
                    Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
                    if (dst == Process.MagazineDevideStatus.SingleToDouble)
                    {
                        mag.NewFg = false;
                        mag.Update();

                        order.DevidedMagazineSeqNo = 1;
                        order.DeleteInsert(order.LotNo);

                        string org = mag.MagazineNo;

                        mag.NascaLotNO = order.LotNo;
                        mag.MagazineNo = org + "_#1";
                        mag.NewFg = true;
                        mag.Update();

                        order.DevidedMagazineSeqNo = 2;
                        order.DeleteInsert(order.LotNo);

                        mag.NascaLotNO = order.LotNo;
                        mag.MagazineNo = org + "_#2";
                        mag.Update();
                    }
                    else
                    {
                        order.DeleteInsert(order.LotNo);
                    }

                    //NASCA開始登録OKをPLCに書き込み
                    Plc.SetBit(NASCA_OK_BIT, 1, Mitsubishi.BIT_ON);
                }
                else
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                    }

                    //NASCA開始NG理由をPLCに書き込み
                    //現在は固定で「1」を書き込み
                    Plc.SetBit(NASCA_NG_REASON_WORD, 1, "1");

                    //NASCA開始登録NGをPLCに書き込み
                    Plc.SetBit(NASCA_NG_BIT, 1, Mitsubishi.BIT_ON);

                    this.OutputApiLog($"[開始登録異常発生] 理由:『{errMsg}』");
                }

                this.OutputApiLog("開始登録処理完了:" + this.MacNo);
            }
            catch (Exception ex)
            {
                //NASCA開始NG理由をPLCに書き込み
                //現在は固定で「1」を書き込み
                Plc.SetBit(NASCA_NG_REASON_WORD, 1, "1");

                //NASCA開始登録NGをPLCに書き込み
                Plc.SetBit(NASCA_NG_BIT, 1, Mitsubishi.BIT_ON);

                Log.ApiLog.Error($"{this.Name} [開始登録異常発生]理由:", ex);
                throw ex;
            }
        }

        /// <summary>
        /// アンローダー指定位置のマガジン情報を取得
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="startOffset"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        private VirtualMag parseMagazine(string[] raw, int startOffset)
        {
            string[] workstart = new string[DATE_TIME_ADDRESS_LENGTH];
            string[] workcomplete = new string[DATE_TIME_ADDRESS_LENGTH];
            string[] magazineno = new string[MAGAZINE_NO_LENGTH];

            try
            {
                Array.Copy(raw, startOffset + WORK_START_ADDRESS_OFFSET, workstart, 0, DATE_TIME_ADDRESS_LENGTH);
                Array.Copy(raw, startOffset + WORK_COMPLETE_ADDRESS_OFFSET, workcomplete, 0, DATE_TIME_ADDRESS_LENGTH);
                Array.Copy(raw, startOffset + MAGAZINE_NO_OFFSET, magazineno, 0, MAGAZINE_NO_LENGTH);

            }
            catch (Exception ex)
            {
                Log.ApiLog.Error($"{this.Name} モールド（新）の排出マガジン情報パースエラー オフセット:{startOffset}", ex);
                return null;
            }

            VirtualMag mag = new VirtualMag();

            if (IsQRReader)
            {
                //分割なしマガジン番号を返す
                mag.MagazineNo = Plc.GetMagazineNo(magazineno, true);
            }
            else
            {
                VirtualMag lMag = this.Peek(Station.Loader);
                if (lMag == null)
                    return null;

                mag.MagazineNo = lMag.MagazineNo;
            }

            mag.LastMagazineNo = mag.MagazineNo;
            mag.WorkStart = Plc.GetWordsAsDateTime(workstart);
            mag.WorkComplete = Plc.GetWordsAsDateTime(workcomplete);

            if (string.IsNullOrEmpty(mag.MagazineNo) == true)
            {
                this.OutputApiLog("排出マガジン番号取得失敗");
                return null;
            }

            if (mag.WorkStart.HasValue == false || mag.WorkComplete.HasValue == false)
            {
                this.OutputApiLog("作業時間(開始 or 完了)取得失敗");
                return null;
            }

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            if (svrmag == null)
            {
                //分割マガジンも検索
                Order ord = Order.SearchOrder(mag.MagazineNo, null, this.MacNo, true, false).OrderBy(m => m.DevidedMagazineSeqNo).FirstOrDefault();

                if (ord != null)
                {
                    svrmag = Magazine.GetCurrent(ord.LotNo);
                }

                // 自動搬送用プレート対策
                if (svrmag == null)
                {
                    // ---- 一旦、ロットNoを取得後にモールド作業未完了のロットNoを取得
                    // 高生産性ライン用ラベル使用時はmag.MagazineNoの中身は単ロットのロットNoが入るので、
                    // 必ず#2の処理でも#1でヒットしてしまう。
                    svrmag = Magazine.GetCurrent(mag.MagazineNo + "_#1");
                    if (svrmag == null)
                    {
                        svrmag = Magazine.GetCurrent(mag.MagazineNo + "_#2");
                    }
                    //if (tmpmag != null)
                    //{
                    //    ord = Order.SearchOrder(tmpmag.NascaLotNO, null, this.MacNo, true, false).OrderBy(m => m.DevidedMagazineSeqNo).FirstOrDefault();
                    //    Magazine[] curMag = Magazine.GetMagazine(ord.LotNo, true);
                    //    if (curMag.Length == 1)
                    //    {
                    //        svrmag = curMag.Single();
                    //    }
                    //    else
                    //    {
                    //        svrmag = null;
                    //    }
                    //}
                }

                if (svrmag == null)
                {
                    //分割状態か判定して、分割の完了履歴も存在しないなら異常
                    ord = Order.SearchOrder(mag.MagazineNo, null, this.MacNo, false, false).OrderBy(m => m.DevidedMagazineSeqNo).FirstOrDefault();
                    if (ord == null)
                    {
                        throw new ApplicationException($"{this.Name} 稼働中ではないマガジンです。マガジンNo:『{mag.MagazineNo}』");
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    mag.MagazineNo = svrmag.MagazineNo;
                    mag.LastMagazineNo = svrmag.MagazineNo;
                }
            }

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
            mag.ProcNo = nextproc.ProcNo;

            return mag;
        }

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //アンローダー以外は何もしない
            if (station != Station.Unloader && station != Station.EmptyMagazineUnloader)
            {
                return true;
            }

            return base.Enqueue(mag, station);
        }
		
		/// <summary>
		/// 空マガジン配置
		/// </summary>
		/// <returns>結果</returns>
		public override bool ResponseEmptyMagazineRequest()
		{
			//if (Config.GetLineType == Config.LineTypes.NEL_MAP)
			//{
			//    //遠心沈降用改良マガジン+マガジン移載機対応

			//    //供給側は途中投入CV側の移動処理
			//    //排出側は移載機側の空マガジン要求処理で行うのでモールド機は処理無し
			//    //仮想マガジンだけを作成する
			//    if (string.IsNullOrEmpty(this.EULMagazineAddress)) return false;

			//    VirtualMag mag = Peek(Station.EmptyMagazineUnloader);
			//    if (mag != null) return false;

			//    if (this.IsRequireOutputEmptyMagazine())
			//    {
			//        string magno = Plc.GetMagazineNo(EULMagazineAddress);
			//        if (string.IsNullOrEmpty(magno) == false)
			//        {
			//            mag = new VirtualMag();
			//            mag.MagazineNo = magno;
			//            mag.LastMagazineNo = magno;
			//            Enqueue(mag, Station.EmptyMagazineUnloader);
			//        }
			//        else
			//        {
			//            Log.RBLog.Info("モールド空マガジン排出位置のマガジンNO取得に失敗");
			//        }
			//    }
			//    return false;
			//}
			//else
			//{
			//    return base.ResponseEmptyMagazineRequest();
			//}
			if (Config.GetLineType == Config.LineTypes.NEL_MAP)
			{
				//遠心沈降用改良マガジン+マガジン移載機対応

				//供給側は途中投入CV側の移動処理
				//排出側は移載機側の空マガジン要求処理で行うのでモールド機は処理無し

				//空マガジンが満杯になる手前まで行って、移載機側が埋まっている場合は排出CVへ捨てる
				if (IsRequireOutputEmptyMagazine())
				{
					if ((GetMagazineCount(Station.EmptyMagazineUnloader) >= UNLOADER_MAGAZINE_COUNT - 1))
					{
						var exs = LineKeeper.Machines
							.Where(m => m is MagExchanger)
							.Where(m => m.IsRequireEmptyMagazine());

						if (exs.Count() >= 1)
						{
							//移動できる場合はここでは何もしない。MagExchangerのResponseEmptyMagazineで処理される
							return false;
						}
						else
						{
							//移動できない場合は排出CVへ捨てる
							Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
							IMachine conveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
							Location to = conveyor.GetLoaderLocation();
							LineKeeper.MoveFromTo(from, to, true, true, false);
						}
					}
				}
				return false;
			}
			else
			{
				bool IsDeleteFromMag = false;

				//供給禁止状態なら処理しない
				if (this.IsInputForbidden() == true)
				{
					return false;
				}

				if (this.IsRequireEmptyMagazine() == true)
				{
					Location from = null;
					LineBridge bridge = LineKeeper.GetReachBridge(this.MacNo);

					//自装置の空マガジンを使用
					if (this.IsRequireOutputEmptyMagazine() == true)
					{
						from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
						IsDeleteFromMag = true;
					}
					//ライン連結橋の空マガジンを使用
					else if (bridge != null && bridge.IsRequireOutputEmptyMagazine())
					{
						//先頭が遠心沈降マガジンなら処理しない
						VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
						if (VirtualMag.IsECKMag(mag.MagazineNo)) return false;

						from = bridge.GetUnLoaderLocation();
						IsDeleteFromMag = true;
					}
					//空マガジン投入CVの空マガジンを使用
					else
					{
						if (base.IsRequireOutputEmptyMagazine())
						{
							//自装置の空マガジン待ち(完了処理待ち)
							return false;
						}

						//空マガジン投入CVの状態確認
						IMachine empMagLoadConveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
						if (empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
						{
							CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
							CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
							if (fromCar.CarNo != toCar.CarNo)
							{
								//空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする
								List<VirtualMag> bridgeMags = new List<VirtualMag>();

								List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge).ToList();
								foreach (LineBridge b in bridges)
								{
									//橋内のすべての仮想マガジンを取得
									bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
								}
								if (bridgeMags.Count != 0)
								{
									if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
									{
										return false;
									}
								}
							}

							from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
							IsDeleteFromMag = false;
						}
						else
						{
							//空マガジン投入CVにマガジンが無い場合
							return false;
						}
					}

					Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
					LineKeeper.MoveFromTo(from, to, IsDeleteFromMag, true, false);

					return true;
				}
				else
				{
					return false;
				}
			}
		}

        /// <summary>
        /// 空マガジン排出要求
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutputEmptyMagazine()
        {
            //空マガジン排出信号ON、仮想マガジン有り(EmptyMagazineUnloader)
            if (!base.IsRequireOutputEmptyMagazine())
            {
                return false;
            }

            if (this.IsAutoLine)
            {
                VirtualMag mag = Peek(Station.EmptyMagazineUnloader);
                if (mag == null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 配置マガジン数取得
        /// </summary>
        /// <param name="st"></param>
        /// <returns></returns>
        public int GetMagazineCount(Station st) 
        {
            int magCount = 0;

            try
            {
                switch (st)
                {
                    case Station.EmptyMagazineLoader:
                        magCount = this.Plc.GetWordAsDecimalData(this.ELMagazineCountAddress);
                        break;

                    case Station.Loader:
                        magCount = this.Plc.GetWordAsDecimalData(this.LMagazineCountAddress);
                        break;

                    case Station.EmptyMagazineUnloader:
                        magCount = this.Plc.GetWordAsDecimalData(this.EULMagazineCountAddress);
                        break;

                    case Station.Unloader:
                        magCount = this.Plc.GetWordAsDecimalData(this.ULMagazineCountAddress);
                        break;
                }
                return magCount;
            }
            catch (Exception) 
            {
                Log.SysLog.Error($"{this.Name} MD機マガジン数の取得に失敗した為、0セットしました。 ステーション:{st.ToString()}");
                return 0;
            }
        }

		#region [削除] 出力されているファイル全てリネーム対象にする事で処理の見直し
		//public void SdComplete()
		//{
		//	if (string.IsNullOrWhiteSpace(SDFilePath)) 
		//	{
		//		return;
		//	}

		//	SDFile sd = SDFile.GetSDFileInfo(SDFilePath);
		//	if (sd == null)
		//	{
		//		return;
		//	}
		//	if (sd.IsLotFromFileName(sd.FileFullPath) == false)
		//	{
		//		// ファイル名称変更(ロット + タイプ + 工程NO付与)
		//		sd.ChangeCompleteFileName(sd.FileFullPath, sd.LotNo, sd.TypeNo, sd.ProcNo);
		//	}

		//	if (sd.IsUnknownData)
		//	{
		//		// SDファイル内容がヘッダのみの空ファイルを想定　ファイル名のみUNKNOWNに変更、以降の処理をスルーしてEICSに削除処理を任せる
		//		return;
		//	}

		//	if (Config.Settings.IsMappingMode)
		//	{
		//		if (ArmsApi.Model.LENS.WorkResult.IsComplete(sd.LotNo, sd.ProcNo) == false)
		//		{
		//			// LENS2処理待ち
		//			Log.ApiLog.Info(string.Format("LENS処理待ち mag:{0}", sd.MagNo));
		//			return;
		//		}
		//	}
		//}

		//public class SDFile
		//{
		//	public const string FILE_PREFIX = "SD";
		//	public const string FILE_UNKNOWN = "UNKNOWN";

		//	private const int FILENAME_UPDDT_STARTINDEX = 9;
		//	private const int FILENAME_UPDDT_LENGTH = 10;



		//	private const int DATA_START_ROW = 3;

		//	/// <summary>
		//	/// SDファイル装置吐き出し位置
		//	/// </summary>
		//	public string BaseDir { get; set; }

		//	/// <summary>
		//	/// 処理対象ファイルフルパス
		//	/// </summary>
		//	public string FileFullPath { get; set; }

		//	/// <summary>
		//	/// SDファイル内マガジン番号　UL側マガジンと一致
		//	/// </summary>
		//	public string MagNo { get; set; }
		//	public string LotNo { get; set; }
		//	public string TypeNo { get; set; }
		//	public int ProcNo { get; set; }
		//	public bool IsUnknownData { get; set; }

		//	public static SDFile GetSDFileInfo(string sdFileDir)
		//	{
		//		SDFile sd = new SDFile();

		//		sd.BaseDir = sdFileDir;
		//		DirectoryInfo bdir = new DirectoryInfo(sdFileDir);

		//		string path = getNewestFileName(sdFileDir);
		//		//ファイルが無い場合はNULLを返す
		//		if (string.IsNullOrEmpty(path)) return null;

		//		sd.FileFullPath = path;

		//		try
		//		{
		//			sd.parseSDFile(sd.FileFullPath);
		//		}
		//		catch (Exception ex)
		//		{
		//			throw new ApplicationException(string.Format("SDファイル解析エラー発生 ファイルパス:{0} エラー内容:{1}", sd.FileFullPath, ex.Message));
		//		}

		//		if (sd.IsUnknownData)
		//		{
		//			sd.LotNo = FILE_UNKNOWN;
		//			sd.TypeNo = FILE_UNKNOWN;
		//			sd.ProcNo = 0;
		//		}
		//		else
		//		{
					
		//			Magazine svrmag = Magazine.GetCurrent(sd.MagNo);
		//			if (svrmag == null)
		//			{
		//				return null;
		//				//throw new ApplicationException("SDファイル処理エラー 現在稼働中マガジンが存在しません:" + sd.MagNo);
		//			}

		//			AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
		//			if (lot == null) throw new ApplicationException("SDファイル処理エラー ロット情報が存在しません:" + sd.MagNo);

		//			sd.LotNo = lot.NascaLotNo;
		//			sd.TypeNo = lot.TypeCd;
		//			sd.ProcNo = Process.GetNowProcess(lot).ProcNo;
		//		}
		//		return sd;
		//	}

		//	/// <summary>
		//	/// MMファイルの内容を解析
		//	/// </summary>
		//	/// <param name="filepath"></param>
		//	/// <param name="magno"></param>
		//	/// <param name="isContains1"></param>
		//	/// <param name="isContains3"></param>
		//	private void parseSDFile(string filepath)
		//	{
		//		string magNo = null;

		//		try
		//		{
		//			using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
		//			{
		//				int lineno = 0;
		//				while (sr.Peek() > 0)
		//				{
		//					string line = sr.ReadLine();

		//					line = line.Replace("\"", "");

		//					if (string.IsNullOrEmpty(line)) continue;

		//					//CSV各要素分解
		//					string[] data = line.Split(',');

		//					//DATA_NOが数字変換できない行は飛ばす
		//					int datano;
		//					if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;
							
		//					string[] magStr = data[MAG_NO_COL].Split(' ');
		//					if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
		//					{
		//						//自動化用の30_を取り除き
		//						magNo = magStr[1];
		//					}
		//					else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
		//					{
		//						//高効率用の11_を取り除き
		//						magNo = magStr[1];
		//					}
		//					else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
		//					{
		//						//高効率用の13_を取り除き
		//						magNo = magStr[1];
		//					}
		//					else
		//					{
		//						magNo = data[MAG_NO_COL];
		//					}
							
		//					lineno = lineno + 1;
		//				}

		//				if (lineno == 0)
		//				{
		//					// データがヘッダのみを想定
		//					this.IsUnknownData = true;
		//					return;
		//				}

		//				if (magNo == null) throw new ApplicationException("MMファイルにマガジン番号が存在しません:" + filepath);
		//			}

		//			this.MagNo = magNo;
		//		}
		//		catch (IOException)
		//		{
		//			Thread.Sleep(5000);
		//			parseSDFile(filepath);
		//		}
		//	}

		//	/// <summary>
		//	/// SD*.CSVファイルの内、最終更新日が新しい1つを返す
		//	/// ファイルが全く無い場合はnull
		//	/// </summary>
		//	/// <param name="mmFileDir"></param>
		//	/// <returns></returns>
		//	private static string getNewestFileName(string mmFileDir)
		//	{
		//		string filePath = MachineLog.GetNewestFile(mmFileDir, SDFile.FILE_PREFIX);

		//		return filePath;
		//	}

		//	/// <summary>
		//	/// SDファイル処理完了確認 
		//	/// </summary>
		//	/// <param name="filePath"></param>
		//	/// <returns></returns>
		//	public bool IsLotFromFileName(string mmFilePath)
		//	{
		//		//名称変更済かで確認(ロット + タイプ + 工程NO付与)
		//		string[] nameChar = Path.GetFileNameWithoutExtension(mmFilePath).Split('_');
		//		if (nameChar.Count() < 5)
		//		{
		//			return false;
		//		}
		//		else { return true; }
		//	}

		//	/// <summary>
		//	/// SDファイル名称変更(ロット + タイプ + 工程NO付与)
		//	/// </summary>
		//	/// <param name="mmFilePath"></param>
		//	/// <param name="lotNo"></param>
		//	/// <param name="typeCd"></param>
		//	public void ChangeCompleteFileName(string mmFilePath, string lotNo, string typeCd, int procNo)
		//	{
		//		MachineLog.ChangeFileName(mmFilePath, LotNo, typeCd, procNo);
		//	}
		//}
		#endregion

		public void CheckMachineLogFile() 
		{
			List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
			foreach(string logFile in logFiles)
			{
                if (IsRenamedFile(logFile)) continue;

                if (IsQRReader)
				{
					if (IsStartLogFile(logFile) == false)
					{
						MachineLog mLog = parseMachineLog(logFile);
						if (mLog.IsUnknownData) 
						{
							MachineLog.ChangeFileName(logFile, MachineLog.FILE_UNKNOWN, MachineLog.FILE_UNKNOWN, 0, MachineLog.FILE_UNKNOWN);
							return;
						}
						
						Magazine svrMag = Magazine.GetCurrent(mLog.MagazineNo);
                        if (svrMag == null)
                        {
                            svrMag = Magazine.GetCurrent(mLog.MagazineNo + "_#2");
                        }
						if (svrMag == null)
							throw new ApplicationException($"{this.Name} 装置ログ内のマガジンNo:{mLog.MagazineNo}の稼働中マガジンが存在しません。");

						AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                        //int procNo = Process.GetNowProcess(lot).ProcNo;
                        Order[] o = Order.SearchOrder(svrMag.NascaLotNO, null, this.MacNo, true, false);
                        if (o.Length == 0)
                            throw new ApplicationException(
                                $"{this.Name} 装置ログ内のマガジンNo:{mLog.MagazineNo}の稼働中ロット：{svrMag.NascaLotNO}の作業中実績が存在しません。");
                        int procNo = o[0].ProcNo;

						MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);
					}
				}
				else 
				{
					// <QRリーダーが付いていない場合の装置ログとロットの紐付け方法>
					// Unloader部仮想マガジン有りの場合、格納されている開始、完了内に装置ログが紐付けばそれを採用
					// Unloader部仮想マガジン無し又は上記条件に紐付かない場合、Loader部先頭仮想マガジンと紐付け
					if (IsStartLogFile(logFile) == false)
					{
						VirtualMag lMag = this.Peek(Station.Loader);
						if (lMag == null)
						{
							throw new ApplicationException($"{this.Name} 装置ログ{logFile}とロットを紐付け中に問題が発生しました。ローダー部に仮想マガジンが存在しません。作業開始登録がされているか確認して下さい。");
						}
						Magazine svrMag = Magazine.GetCurrent(lMag.MagazineNo);
						AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
						int procNo = Process.GetNowProcess(lot).ProcNo;
						MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);

						//DateTime logFileDt = File.GetLastWriteTime(logFile);
						//Magazine svrMag = null;

						//VirtualMag ulMag = this.Peek(Station.Unloader);
						//if (ulMag == null)
						//{
						//	VirtualMag lMag = this.Peek(Station.Loader);
						//	if (lMag == null)
						//	{
						//		throw new ApplicationException(string.Format("装置ログ{0}とロットを紐付け中に問題が発生しました。ローダー部に仮想マガジンが存在しません。作業開始登録がされているか確認して下さい。", logFile));
						//	}
						//	svrMag = Magazine.GetCurrent(lMag.MagazineNo);
						//}
						//else
						//{
						//	if (logFileDt >= ulMag.WorkStart.Value && logFileDt <= ulMag.WorkComplete.Value)
						//	{
						//		svrMag = Magazine.GetCurrent(ulMag.MagazineNo);
						//	}
						//	else 
						//	{
						//		VirtualMag lMag = this.Peek(Station.Loader);
						//		if (lMag == null)
						//		{
						//			throw new ApplicationException(string.Format("装置ログ{0}とロットを紐付け中に問題が発生しました。ローダー部に仮想マガジンが存在しません。作業開始登録がされているか確認して下さい。", logFile));
						//		}
						//		svrMag = Magazine.GetCurrent(lMag.MagazineNo);
						//	}
						//}

						//AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
						//int procNo = Process.GetNowProcess(lot).ProcNo;
						//MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);
					}
					//DateTime logFileDt = File.GetLastWriteTime(logFile);
					//Order order = Order.GetMachineOrder(this.MacNo, logFileDt);
					//if (order == null) 	
					//{
					//	throw new ApplicationException(
					//		string.Format("装置ログ(日時:{0})が実績に紐づかない為、ロットを特定できません。作業開始した実績がある事を確認後、無ければファイル除去をし、再開して下さい。 装置ログパス:{1}", logFileDt, logFile));
					//}
					//AsmLot lot = AsmLot.GetAsmLot(order.NascaLotNo);

					//MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, order.ProcNo, order.InMagazineNo);
				}
			}	
		}

		/// <summary>
		/// 装置ログファイル処理完了確認 
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns></returns>
		public bool IsLotFromFileName(string filePath)
		{
			//名称変更済かで確認(ロット + タイプ + 工程NO付与)
			string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');
			if (nameChar.Count() < 5)
			{
				return false;
			}
			else { return true; }
		}

        /// <summary>
        /// 装置ログファイルの内容を解析
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="magno"></param>
        /// <param name="isContains1"></param>
        /// <param name="isContains3"></param>
        public static MachineLog parseMachineLog(string filepath, int retryCt)
		{
			if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
			{
				throw new ApplicationException("ファイル解析中にエラーが発生しました：" + filepath);
			}

			MachineLog mLog = new MachineLog();
			string magNo = null;

			try
			{
                //装置ログの\rがなくなったら削除する機能(Start)
                //[緊急対応：削除予定]ログファイル内の[\r"]⇒["]に置換してファイルに書き戻す処理追加
                List<byte> byteList = new List<byte>();
                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite))
                {
                    byte[] bs = new byte[fs.Length];

                    fs.Read(bs, 0, bs.Length);

                    for (int i = 0; i < bs.Length; i++)
                    {
                        if ((i < bs.Length - 1) && bs[i] == 0x0D && bs[i + 1] == 0x22)
                        {
                            continue;
                        }

                        byteList.Add(bs[i]);
                    }
                }
                using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.ReadWrite))
                {
                    fs.Write(byteList.ToArray(), 0, byteList.ToArray().Length);
                }
                //装置ログの\rがなくなったら削除する機能(end)

                //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                //filepath = @"C:\QCIL\data\MD\S08571(257号機)\log000_AM1809010112.csv";
                string filenm = Path.GetFileName(filepath);

                //リネーム対象は完了のファイルのみ
                int keikoufgColumn = 0;
                if (filenm.Contains("SM") == true)
                {
                    keikoufgColumn = SM_KEIKOUFG_COL_INDEX;
                }
                else if (filenm.Contains("AM") == true)
                {
                    keikoufgColumn = AM_KEIKOUFG_COL_INDEX;
                }
                else if (filenm.Contains("EM") == true)
                {
                    keikoufgColumn = EM_KEIKOUFG_COL_INDEX;
                }
                else if (filenm.Contains("SD") == true)
                {
                    keikoufgColumn = SD_KEIKOUFG_COL_INDEX;
                }
                //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                    {
                        int lineno = 0;
                        while (sr.Peek() > 0)
                        {
                            string line = sr.ReadLine();

                            line = line.Replace("\"", "");

                            if (string.IsNullOrEmpty(line)) continue;

                            //CSV各要素分解
                            string[] data = line.Split(',');
                            
                            //DATA_NOが数字変換できない行は飛ばす
                            int datano;
                            if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;

                            //<--SGA-IM0000007627 MD装置SDカード内不要データ抽出
                            //傾向管理フラグが0の行は飛ばす
                            if (Convert.ToInt32(data[keikoufgColumn]) == 0) continue;
                            //-->SGA-IM0000007627 MD装置SDカード内不要データ抽出
                            
                            string[] magStr = data[MAG_NO_COL].Split(' ');
                            if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
                            {
                                //自動化用の30_を取り除き
                                magNo = magStr[1];
                            }
                            else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
                            {
                                //高効率用の11_を取り除き
                                magNo = magStr[1];
                            }
                            else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                            {
                                //高効率用の13_を取り除き
                                magNo = magStr[1];
                            }
                            else
                            {
                                magNo = data[MAG_NO_COL];
                            }

                            lineno = lineno + 1;
                        }

                        if (lineno == 0)
                        {
                            // データがヘッダのみを想定
                            mLog.IsUnknownData = true;
                            return mLog;
                        }

                        if (magNo == null) throw new ApplicationException("装置ログにマガジン番号が存在しません:" + filepath);
                    }

				mLog.MagazineNo = magNo;
				return mLog;
			}
			catch (IOException)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				return parseMachineLog(filepath, retryCt);
			}	
		}
        public static MachineLog parseMachineLog(string filepath) 
		{
			return parseMachineLog(filepath, 0);
		}

		public static bool IsStartLogFile(string fullName)
		{
			string fileName = Path.GetFileNameWithoutExtension(fullName);
			if (Regex.IsMatch(fileName, "^log..._OR.*$") || Regex.IsMatch(fileName, "^log..._PR.*$")
				|| Regex.IsMatch(fileName, "^log..._SF.*$") || Regex.IsMatch(fileName, "^log..._EF.*$"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// 吐出圧力取得
        /// </summary>
        private void readDischargePressureData()
        {
            this.OutputApiLog($"吐出圧力取得処理開始");

            //設備番号
            string plantCd = Plc.GetString(PLANT_CD_ADDRESS, PLANT_CD_WORD_LENGTH, true);
            this.OutputApiLog($"[{this.PlantCd}]装置から取得した設備番号:{plantCd}");
            if (string.IsNullOrWhiteSpace(plantCd) == true)
            {
                throw new ApplicationException($"[{this.PlantCd}]設備番号を取得できませんでした。");
            }

            //吐出圧力
            string d1 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_1);
            string d2 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_2);
            string d3 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_3);
            string d4 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_4);
            string d5 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_5);
            string d6 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_6);
            string d7 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_7);
            string d8 = Plc.GetBit(DISCHARGE_PRESSURE_DATA_ADDRESS_8);
            if (string.IsNullOrWhiteSpace(d1) == true ||
                string.IsNullOrWhiteSpace(d2) == true ||
                string.IsNullOrWhiteSpace(d3) == true ||
                string.IsNullOrWhiteSpace(d4) == true ||
                string.IsNullOrWhiteSpace(d5) == true ||
                string.IsNullOrWhiteSpace(d6) == true ||
                string.IsNullOrWhiteSpace(d7) == true ||
                string.IsNullOrWhiteSpace(d8) == true)
            {
                throw new ApplicationException($"[{this.PlantCd}]吐出圧力値を取得できませんでした。");
            }

            decimal dischargePressureData1 = Convert.ToDecimal(d1);
            decimal dischargePressureData2 = Convert.ToDecimal(d2);
            decimal dischargePressureData3 = Convert.ToDecimal(d3);
            decimal dischargePressureData4 = Convert.ToDecimal(d4);
            decimal dischargePressureData5 = Convert.ToDecimal(d5);
            decimal dischargePressureData6 = Convert.ToDecimal(d6);
            decimal dischargePressureData7 = Convert.ToDecimal(d7);
            decimal dischargePressureData8 = Convert.ToDecimal(d8);

            //Plc.GetMagazineNo(DISCHARGE_PRESSURE_DATA_ADDRESS, DISCHARGE_PRESSURE_DATA_WORD_LENGTH);
            this.OutputApiLog($"[{this.PlantCd}]装置から取得した吐出圧力:「{Convert.ToInt32(dischargePressureData1)},{Convert.ToInt32(dischargePressureData2)},{Convert.ToInt32(dischargePressureData3)},{Convert.ToInt32(dischargePressureData4)},{Convert.ToInt32(dischargePressureData5)},{Convert.ToInt32(dischargePressureData6)},{Convert.ToInt32(dischargePressureData7)},{Convert.ToInt32(dischargePressureData8)}」");

            //吐出圧力記録テーブルデータへ記録
            ArmsApi.Model.MoldDischargePressure.Insert(plantCd, dischargePressureData1, dischargePressureData2, dischargePressureData3
                , dischargePressureData4, dischargePressureData5, dischargePressureData6, dischargePressureData7, dischargePressureData8);

            //取得要求信号をOFF
            this.OutputApiLog($"取得要求信号をOFF");
            Plc.SetBit(this.ReadDischargePressureDataReqBitAddress, 1, Keyence.BIT_OFF);

            this.OutputApiLog($"吐出圧力取得処理完了");
        }

        /// <summary>
        /// 変更吐出圧力送信
        /// </summary>
        private void sendDischargePressureData()
        {
            this.OutputApiLog($"変更吐出圧力送信処理開始");

            //算出した吐出圧力値書込
            List<decimal> dischargePressureData = ArmsApi.Model.MoldDischargePressure.GetMeasurementData(this.PlantCd);
            if(dischargePressureData == null ||
                dischargePressureData.Count != 8)
            {
                throw new ApplicationException($"[{this.PlantCd}]算出した吐出圧力値を取得できませんでした。");
            }

            this.OutputApiLog($"算出した吐出圧力値書込:{this.PlantCd}、吐出圧力「{Convert.ToInt32(dischargePressureData[0])},{Convert.ToInt32(dischargePressureData[1])},{Convert.ToInt32(dischargePressureData[2])},{Convert.ToInt32(dischargePressureData[3])},{Convert.ToInt32(dischargePressureData[4])},{Convert.ToInt32(dischargePressureData[5])},{Convert.ToInt32(dischargePressureData[6])},{Convert.ToInt32(dischargePressureData[7])}」");
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_1, Convert.ToInt32(dischargePressureData[0]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_2, Convert.ToInt32(dischargePressureData[1]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_3, Convert.ToInt32(dischargePressureData[2]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_4, Convert.ToInt32(dischargePressureData[3]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_5, Convert.ToInt32(dischargePressureData[4]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_6, Convert.ToInt32(dischargePressureData[5]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_7, Convert.ToInt32(dischargePressureData[6]));
            Plc.SetWordAsDecimalData(RESULT_DISCHARGE_PRESSURE_DATA_ADDRESS_8, Convert.ToInt32(dischargePressureData[7]));

            //吐出圧力記録テーブルデータの送信完了フラグをON
            ArmsApi.Model.MoldDischargePressure.MoldSendingDone(this.PlantCd);

            //吐出圧力転送完了信号をON
            this.OutputApiLog($"吐出圧力転送完了信号をON");
            Plc.SetBit(RESULT_SEND_COMPLETE, 1, Keyence.BIT_ON);

            this.OutputApiLog($"変更吐出圧力送信処理完了");
        }

        /// <summary>
        /// 設備番号照合結果ファイル処理
        /// </summary>
        private void checkPlantVerificationFile()
        {
            MachineInfo machine = MachineInfo.GetMachine(this.PlantCd);
            if (machine == null)
            {
                throw new ApplicationException($"{this.Name} 装置マスタに対象の装置が見つかりません。設備番号：{this.PlantCd}");
            }
            else if (string.IsNullOrWhiteSpace(machine.VerificationLogOutputDirectoryPath) == true)
            {
                //設定が無い場合は何もしないよう修正
                return;
                //throw new ApplicationException($"装置マスタに照合結果ログ出力先フォルダの設定(verificationlogoutputdirectorypath)がされていません。設備番号：{this.PlantCd}");
            }
            else
            {
                List<string> logFiles = MachineLog.GetFilesOrderByDescendingLastAccessTime_NotRetry(machine.VerificationLogOutputDirectoryPath, "^*" + this.PlantCd + ".*$");

                if (logFiles.Count >= 1)
                {
                    //日付の一番新しいファイルのみ処理しDoneフォルダへ移動。それ以外は別フォルダへ移動。
                    bool firstfg = true;
                    foreach (string filepath in logFiles)
                    {
                        if (firstfg == true)
                        {
                            this.OutputApiLog($"設備番号照合結果ファイル処理開始 ファイル：{filepath}");
                            bool? isOk = null;
                            using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                            {
                                while (sr.Peek() > 0)
                                {
                                    string line = sr.ReadLine().Trim();
                                    if (line.ToUpper() == "OK")
                                    {
                                        this.OutputApiLog($"OK送信");
                                        isOk = true;
                                        Plc.SetBit(PLANT_VERIFICATION_NG_ADDRESS, 1, Keyence.BIT_OFF);
                                        Plc.SetBit(PLANT_VERIFICATION_OK_ADDRESS, 1, Keyence.BIT_ON);
                                        break;
                                    }
                                    else if (line.ToUpper() == "NG")
                                    {
                                        this.OutputApiLog($"NG送信");
                                        isOk = false;
                                        Plc.SetBit(PLANT_VERIFICATION_OK_ADDRESS, 1, Keyence.BIT_OFF);
                                        Plc.SetBit(PLANT_VERIFICATION_NG_ADDRESS, 1, Keyence.BIT_ON);
                                        break;
                                    }
                                }
                            }

                            if (isOk.HasValue == false)
                            {
                                throw new ApplicationException($"照合結果ログファイルの内容が正しくありません。ファイル：{filepath}");
                            }

                            string destPath = Path.Combine(machine.VerificationLogOutputDirectoryPath, "done");
                            if (Directory.Exists(destPath) == false)
                            {
                                Directory.CreateDirectory(destPath);
                            }
                            string destFullPath = Path.Combine(destPath, Path.GetFileName(filepath));
                            if (File.Exists(destFullPath) == true)
                            {
                                //移動先のフォルダに同じファイル名が存在する場合エラーとなるので、先に削除する
                                File.Delete(destFullPath);
                            }
                            File.Move(filepath, destFullPath);

                            firstfg = false;

                            this.OutputApiLog($"設備番号照合結果ファイル処理完了");
                        }
                        else
                        {
                            string destPath = Path.Combine(machine.VerificationLogOutputDirectoryPath, "old");
                            if (Directory.Exists(destPath) == false)
                            {
                                Directory.CreateDirectory(destPath);
                            }
                            string destFullPath = Path.Combine(destPath, Path.GetFileName(filepath));
                            if (File.Exists(destFullPath) == true)
                            {
                                //移動先のフォルダに同じファイル名が存在する場合エラーとなるので、先に削除する
                                File.Delete(destFullPath);
                            }
                            File.Move(filepath, destFullPath);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 設備番号照合結果ファイルのごみデータをoldに移動する
        /// </summary>
        private void moveOldFilePlantVerification()
        {
            MachineInfo machine = MachineInfo.GetMachine(this.PlantCd);
            if (machine == null)
            {
                return;
            }
            if (string.IsNullOrWhiteSpace(machine.VerificationLogOutputDirectoryPath) == true)
            {
                return;
            }

            List<string> logFiles = MachineLog.GetFiles(machine.VerificationLogOutputDirectoryPath);
            foreach (string filepath in logFiles)
            {
                string destPath = Path.Combine(machine.VerificationLogOutputDirectoryPath, "old");
                if (Directory.Exists(destPath) == false)
                {
                    Directory.CreateDirectory(destPath);
                }
                string destFullPath = Path.Combine(destPath, Path.GetFileName(filepath));
                if (File.Exists(destFullPath) == true)
                {
                    //移動先のフォルダに同じファイル名が存在する場合エラーとなるので、先に削除する
                    File.Delete(destFullPath);
                }
                File.Move(filepath, destFullPath);
            }
        }
    }
}
