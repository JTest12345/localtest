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
    public class ManualECK : MachineBase
    {
        /// <summary>
        /// 直近100ロットの完成履歴
        /// </summary>
        public Queue<string> preCompleteLotQueue = new Queue<string>();

        /// <summary>
        /// 回転プログラム番号
        /// </summary>
        public string ProgramNoAddress { get; set; }

        #region CPMChamber

        public class CPMChamber
        {
            public int ChamberNo { get; set; }
            public string ReqBitAddress { get; set; }
            public string StartOKBitAddress { get; set; }
            public string StartNGBitAddress { get; set; }
            public string CompleteOKBitAddress { get; set; }
            public string CompleteNGBitAddress { get; set; }

            /// <summary>
            /// 中判マガジン判定ビット
            /// </summary>
            public string MediumSizeLotBitAddress { get; set; }

            /// <summary>
            /// 大判マガジン判定ビット
            /// </summary>
            public string LargeSizeLotBitAddress { get; set; }

            public string MagazineAddress { get; set; }

            /// <summary>
            /// 開始登録OK、NG
            /// </summary>
            public bool IsNASCAStartOK { get; set; }

            /// <summary>
            /// 完了登録OK、NG
            /// </summary>
            public bool IsNASCACompleteOK { get; set; }

            /// <summary>
            /// 基板サイズ
            /// </summary>
            public WorkCondition.MapFrameSize Size { get; set; }

            public CPMChamber(int chamberNo)
            {
                this.ChamberNo = chamberNo;
            }
        }
        #endregion

        public List<CPMChamber> ChamberList { get; set; }

        public string StartReqBitAddress { get; set; }

        /// <summary>
        /// プログラム番号のBIT長
        /// </summary>
        private const int PROGRAM_NO_BIT_LENGTH = 8;

        /// <summary>
        /// 中判用ダミーマガジン
        /// </summary>
        private const string MID_SIZE_DUMMY_MAG = "DUMMY";

        /// <summary>
        /// 大判用ダミーマガジン
        /// </summary>
        private const string LARGE_SIZE_DUMMY_MAG = "DUMMY1";

        protected override void concreteThreadWork()
        {
            if (Plc.GetBit(StartReqBitAddress) == Mitsubishi.BIT_ON)
            {
                WorkStart();
            }

            if (this.IsRequireOutput() == true)
            {
                workComplete();
            }
        }

        #region WorkStart

        /// <summary>
        /// 作業開始登録
        /// </summary>
        /// <param name="plc"></param>
        public void WorkStart()
        {
            //同一周回内で1ロットは1回しか登録を行わない
            SortedList<string, bool> exitslot = new SortedList<string, bool>();

            try
            {
                string prgNo = Plc.GetString(this.ProgramNoAddress, PROGRAM_NO_BIT_LENGTH);

                DateTime? workstart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                if (workstart.HasValue == false)
                {
                    throw new ApplicationException("遠心沈降機　開始時間取得失敗:" + this.MacNo);
                }

                foreach (CPMChamber c in this.ChamberList)
                {
					//2014.8.5 湯浅君の要望で現在流動中のsizeはLargeしかない為、強制設定してマスタ設定手間を無くす。
					c.Size = WorkCondition.MapFrameSize.Large;

                    //実体マガジンの有無チェック
                    if (Plc.GetBit(c.ReqBitAddress) == MachinePLC.BIT_OFF)
                    {
                        c.IsNASCAStartOK = true;
                        c.Size = WorkCondition.MapFrameSize.None;
                        continue;
                    }

                    //Prefixの11_ 13_を判断するため、マガジン番号ではなく文字列として取得
                    string mg = Plc.GetString(c.MagazineAddress, MachinePLC.MAGAZINE_NO_WORD_LENGTH);

					//if (mg == MID_SIZE_DUMMY_MAG)
					//{
					//	Log.ApiLog.Info("チャンバー：" + c.ChamberNo.ToString() + "中判ダミー");
					//	c.IsNASCAStartOK = true;
					//	c.Size = WorkCondition.MapFrameSize.Medium;
					//}
					if (mg == LARGE_SIZE_DUMMY_MAG)
					{
						Log.ApiLog.Info("チャンバー：" + c.ChamberNo.ToString() + "大判ダミー");
						c.IsNASCAStartOK = true;
						//c.Size = WorkCondition.MapFrameSize.Large;
					}
                    else if (mg.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                    {
                        Log.ApiLog.Info(string.Format("遠心沈降機 開始登録(高効率)", mg));

                        #region MAP高効率用開始登録
                        string[] elms = mg.Trim().Split(' ');

                        //マガジン番号をロット番号の形にとり直し(ロット番号・ロット_#連番の内、稼働中ロットがある方)
                        mg = "";
                        Magazine svrmag = Magazine.GetCurrent(elms[1]);
                        if (svrmag == null)
                        {
                            int magSeqNo;
                            if (int.TryParse(elms[2], out magSeqNo) == true)
                            {
                                svrmag = Magazine.GetCurrent(Order.NascaLotToMagLot(elms[1], magSeqNo));
                            }
                        }
                        if (svrmag != null)
                        {
                            mg = svrmag.MagazineNo;
                        }

                        //AsmLot asmLot = AsmLot.GetAsmLot(elms[1]);
                        //Process proc = Process.GetProcess("MD0081");

                        //Process.MagazineDevideStatus st = Process.GetMagazineDevideStatus(asmLot, proc.ProcNo);
                        //if (st == Process.MagazineDevideStatus.Double || st == Process.MagazineDevideStatus.SingleToDouble)
                        //{
                        //    //マガジン番号をロット_#連番の形にとり直し
                        //    mg = Plc.GetMagazineNo(c.MagazineAddress, false);
                        //}
                        //else
                        //{
                        //    //マガジン番号をロット番号だけの形にとり直し
                        //    mg = Plc.GetMagazineNo(c.MagazineAddress, true);
                        //}
                        
                        string errmsg = string.Empty;
                        if (!exitslot.Keys.Contains(mg))
                        {
                            //ARMS開始
                            c.IsNASCAStartOK = WorkStartInlineLot(mg, prgNo, out errmsg);
                            exitslot.Add(mg, c.IsNASCAStartOK);
                        }
                        else
                        {
                            //既に開始登録済みのロットは前回の結果を引き継ぐ
                            c.IsNASCAStartOK = exitslot[mg];
                        }

                        if (c.IsNASCAStartOK)
                        {
                            Log.ApiLog.Info("NASCA開始登録OK: " + this.MacNo + ":" + mg + ":" + prgNo);
                        }
                        else
                        {
                            Log.ApiLog.Info("NASCA開始登録NG: " + this.MacNo + ":" + mg + ":" + prgNo + ":" + errmsg);
                        }

                        #endregion
                    }
                    else if (mg.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE)) 
                    {
                        Log.ApiLog.Info(string.Format("遠心沈降機 開始登録(高効率)", mg));

                        #region SIDEVIEW304D用開始登録

                        string[] elms = mg.Trim().Split(' ');
                        mg = Magazine.GetCurrent(elms[1]).NascaLotNO;

						//c.Size = WorkCondition.MapFrameSize.Large;

                        string errmsg = string.Empty;
                        if (!exitslot.Keys.Contains(mg))
                        {
                            //ARMS開始
                            c.IsNASCAStartOK = WorkStartInlineLot(mg, prgNo, out errmsg);
                            exitslot.Add(mg, c.IsNASCAStartOK);
                        }
                        else
                        {
                            //既に開始登録済みのロットは前回の結果を引き継ぐ
                            c.IsNASCAStartOK = exitslot[mg];
                        }

                        if (c.IsNASCAStartOK)
                        {
                            Log.ApiLog.Info("NASCA開始登録OK: " + this.MacNo + ":" + mg + ":" + prgNo);
                        }
                        else
                        {
                            Log.ApiLog.Info("NASCA開始登録NG: " + this.MacNo + ":" + mg + ":" + errmsg);
                        }

                        #endregion
                    }
                    else
                    {
                        Log.ApiLog.Info(string.Format("遠心沈降機 開始登録(アウトライン)", mg));

                        #region アウトライン用開始登録
                        //マガジン番号をロット番号だけの形にとり直し
                        mg = Plc.GetMagazineNo(c.MagazineAddress, true);

                        c.IsNASCAStartOK = false;
                        c.Size = WorkCondition.MapFrameSize.None;

                        MachineInfo svrmac = MachineInfo.GetMachine(this.MacNo);
                        if (svrmac == null)
                        {
                            Log.ApiLog.Info("Svr装置情報が見つかりません: " + this.MacNo);
                            continue;
                        }

                        //フレームサイズを参照サーバーから取得
                        WorkCondition.MapFrameSize nascaSize = WorkCondition.GetMapFrameSizeForOutlineLot(mg);

                        if (nascaSize == WorkCondition.MapFrameSize.None)
                        {
                            //取得失敗の場合、仮に中判として装置側には設定する。
                            //未設定では運転がかからないため
                            c.Size = WorkCondition.MapFrameSize.Medium;

                            //NASCA開始NG扱い
                            c.IsNASCAStartOK = false;
                            Log.ApiLog.Info("NASCAフレームサイズマスタ取得異常: " + this.MacNo + ":" + mg + ":" + prgNo);
                            continue;
                        }
                        c.Size = nascaSize;

                        string errmsg = string.Empty;
                        if (!exitslot.Keys.Contains(mg))
                        {
                            //NASCA更新
                            ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
                            c.IsNASCAStartOK = api.NppEditMeasureRstInfo(mg, svrmac.NascaPlantCd,
                                workstart.Value.ToString(), "", prgNo, false, true, out errmsg);
                            exitslot.Add(mg, c.IsNASCAStartOK);
                        }
                        else
                        {
                            //既に開始登録済みのロットは前回の結果を引き継ぐ
                            c.IsNASCAStartOK = exitslot[mg];
                        }

                        if (c.IsNASCAStartOK)
                        {
                            Log.ApiLog.Info("NASCA開始登録OK: " + this.MacNo + ":" + mg + ":" + prgNo);
                        }
                        else
                        {
                            Log.ApiLog.Info("NASCA開始登録NG: " + this.MacNo + ":" + mg + ":" + prgNo + ":" + errmsg);
                        }
                        #endregion
                    }
                }

                //  NASCA開始OK、NGをPLCに書き込み
                foreach (CPMChamber c in this.ChamberList)
                {
                    if (c.IsNASCAStartOK == true)
                    {
                        if (c.Size == WorkCondition.MapFrameSize.Large)
                        {
                            Log.ApiLog.Info("チャンバー：" + c.ChamberNo.ToString() + "大判マガジン判定セット");
                            Plc.SetBit(c.LargeSizeLotBitAddress, 1, MachinePLC.BIT_ON);
                        }
                        else if (c.Size == WorkCondition.MapFrameSize.Medium)
                        {
                            Log.ApiLog.Info("チャンバー：" + c.ChamberNo.ToString() + "中判マガジン判定セット");
                            Plc.SetBit(c.MediumSizeLotBitAddress, 1, MachinePLC.BIT_ON);
                        }

                        Plc.SetBit(c.StartOKBitAddress, 1, MachinePLC.BIT_ON);
                    }
                    else
                    {
                        Plc.SetBit(c.StartNGBitAddress, 1, MachinePLC.BIT_ON);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("遠心沈降機 作業開始エラー:", ex);
                throw ex;
            }
        }
        #endregion

        #region WorkComplete

        private void workComplete()
        {
            //同一周回内で1ロットは1回しか登録を行わない
            SortedList<string, bool> exitslot = new SortedList<string, bool>();

            try
            {
                string prgNo = Plc.GetString(this.ProgramNoAddress, PROGRAM_NO_BIT_LENGTH);

                DateTime? workstart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                if (workstart.HasValue == false)
                {
                    throw new ApplicationException("遠心沈降機　開始時間取得失敗:" + this.MacNo);
                }

                DateTime? workcomplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                if (workcomplete.HasValue == false)
                {
                    throw new ApplicationException("遠心沈降機　完了時間取得失敗:" + this.MacNo);
                }

                foreach (CPMChamber c in this.ChamberList)
                {
                    //実体マガジンの有無チェック
                    if (Plc.GetBit(c.ReqBitAddress) == MachinePLC.BIT_OFF)
                    {
                        c.IsNASCACompleteOK = true;
                        continue;
                    }

                    string mg = Plc.GetString(c.MagazineAddress, MachinePLC.MAGAZINE_NO_WORD_LENGTH);

                    if (mg == LARGE_SIZE_DUMMY_MAG || mg == MID_SIZE_DUMMY_MAG)
                    {
                        c.IsNASCACompleteOK = true;
                    }
                    else if (mg.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                    {
                        #region 高効率用完了登録 ロット番号をPLCから取得

                        c.IsNASCACompleteOK = true;

                        string[] elms = mg.Trim().Split(' ');

                        //マガジン番号をロット番号の形にとり直し(ロット番号・ロット_#連番の内、稼働中ロットがある方)
                        mg = "";
                        Magazine svrmag = Magazine.GetCurrent(elms[1]);
                        if (svrmag == null)
                        {
                            int magSeqNo;
                            if (int.TryParse(elms[2], out magSeqNo) == true)
                            {
                                svrmag = Magazine.GetCurrent(Order.NascaLotToMagLot(elms[1], magSeqNo));
                            }
                        }
                        if (svrmag != null)
                        {
                            mg = svrmag.MagazineNo;
                        }

                        //AsmLot asmLot = AsmLot.GetAsmLot(elms[1]);
                        //Process proc = Process.GetProcess("MD0081");
                        //Process.MagazineDevideStatus st = Process.GetMagazineDevideStatus(asmLot, proc.ProcNo);
                        //if (st == Process.MagazineDevideStatus.Double || st == Process.MagazineDevideStatus.SingleToDouble)
                        //{
                        //    //マガジン番号をロット_#連番の形にとり直し
                        //    mg = Plc.GetMagazineNo(c.MagazineAddress, false);
                        //}
                        //else
                        //{
                        //    //マガジン番号をロット番号だけの形にとり直し
                        //    mg = Plc.GetMagazineNo(c.MagazineAddress, true);
                        //}

						OutputSysLog(string.Format("[完了処理] 開始 MagazineNo:{0}", mg));

                        //完了処理
                        string errmsg = string.Empty;
                        if (WorkEndInlineLot(mg, out errmsg))
                        {
                            Log.ApiLog.Info("NASCA完了登録OK: " + this.MacNo + ":" + mg);
                            c.IsNASCACompleteOK = true;
                        }
                        else
                        {
                            Log.ApiLog.Info("NASCA完了登録OK: " + this.MacNo + ":" + mg + ":" + errmsg);
                            c.IsNASCACompleteOK = false;
                            continue;
                        }

						OutputSysLog(string.Format("[完了処理] 完了 MagazineNo:{0}", mg));

                        #endregion
                    }
                    else if (mg.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
                    {
                        #region SIDEVIEW304D用完了登録 マガジン番号をPLCから取得

                        c.IsNASCACompleteOK = true;

                        string[] elms = mg.Trim().Split(' ');
                        mg = Magazine.GetCurrent(elms[1]).NascaLotNO;

                        //完了処理
                        string errmsg = string.Empty;
                        if (WorkEndInlineLot(mg, out errmsg))
                        {
                            Log.ApiLog.Info("NASCA完了登録OK: " + this.MacNo + ":" + mg);
                            c.IsNASCACompleteOK = true;
                        }
                        else 
                        {
                            Log.ApiLog.Info("NASCA完了登録NG: " + this.MacNo + ":" + mg + ":" + errmsg);
                            c.IsNASCACompleteOK = false;
                            continue;
                        }

                        #endregion
                    }
                    else
                    {
                        #region アウトライン用完了登録

                        mg = Plc.GetMagazineNo(c.MagazineAddress, true);

                        MachineInfo svrmac = MachineInfo.GetMachine(this.MacNo);
                        if (svrmac == null)
                        {
                            Log.ApiLog.Info("Svr装置情報が見つかりません: " + this.MacNo);
                            c.IsNASCACompleteOK = false;
                            continue;
                        }

                        string errMsg = string.Empty;
                        if (!exitslot.Keys.Contains(mg))
                        {
                            //NASCA更新
                            ArmsApi.Model.NASCA.NascaPubApi api = ArmsApi.Model.NASCA.NascaPubApi.GetInstance();
                            c.IsNASCACompleteOK = api.NppEditMeasureRstInfo(mg, svrmac.NascaPlantCd,
                                    workstart.Value.ToString(), workcomplete.Value.ToString(), prgNo, true, true, out errMsg);
                            exitslot.Add(mg, c.IsNASCACompleteOK);
                        }
                        else
                        {
                            c.IsNASCACompleteOK = exitslot[mg];
                        }

                        if (c.IsNASCACompleteOK)
                        {
                            Log.ApiLog.Info("NASCA完了登録OK: " + this.MacNo + ":" + mg);
                        }
                        else
                        {
                            Log.ApiLog.Info("NASCA完了登録NG: " + this.MacNo + ":" + mg + ":" + errMsg);
                        }

                        #endregion
                    }
                }

                //  完了OK,NGをPLCに書き込み
                foreach (CPMChamber c in this.ChamberList)
                {
                    if (c.IsNASCACompleteOK == true)
                    {
                        Plc.SetBit(c.CompleteOKBitAddress, 1, MachinePLC.BIT_ON);
                    }
                    else
                    {
                        Plc.SetBit(c.CompleteNGBitAddress, 1, MachinePLC.BIT_ON);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.ApiLog.Error("遠心沈降機 作業完了エラー:" + ex.ToString(), ex);
                throw ex;
            }
        }

        #endregion

        #region WorkStartInlineLot

        /// <summary>
        /// 作業開始登録
        /// </summary>
        /// <param name="plc"></param>
        public bool WorkStartInlineLot(string lotno, string programNo, out string errMsg)
        {
            AsmLot lot = AsmLot.GetAsmLot(lotno);
            if (lot == null)
            {
                errMsg = "Svrロットが見つかりません:" + lotno;
                return false;
            }

			//if (WorkCondition.CompareECKProgramToNasca(lot.TypeCd, this.MacNo, programNo) == false)
			//{
			//	errMsg = "プログラム照合不一致";
			//	return false;
			//}

			//2014.7.23 41移管2次で検証中
			if (WorkCondition.CompareECKProgram(lot.TypeCd, this.MacNo, programNo) == false)
			{
				errMsg = "プログラム照合不一致";
				return false;
			}

            try
            {
                //SIDEVIEW 304D対応 TnMag取得方法をMagazineNoからLotNoへ
                //Magazine mag = Magazine.GetCurrent(lotno);
                //if (mag == null)
                //{
                //    errMsg = "Svrマガジンが見つかりません:" + lotno;
                //    return false;
                //}

                Magazine[] mags = Magazine.GetMagazine(lotno, true);
                if (mags.Count() == 0)
                {
                    errMsg = "Svrマガジンが見つかりません:" + lotno;
                    return false;
                }
                Magazine mag = mags.Single();

                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    errMsg = "次工程情報が見つかりません:" + mag.NowCompProcess;
                    return false;
                }

                MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
                if (machine == null)
                {
                    errMsg = "Svr装置情報が見つかりません:" + this.MacNo;
                    return false;
                }

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
				order.IsDefectEnd = true;

                bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
                if (!isError)
                {
                    order.DeleteInsert(order.LotNo);
                    return true;
                }
                else
                {
                    // ダミー実績を登録した際は、削除 + 巻き戻し
                    if (insertDummyTranFg == true)
                    {
                        Order.ReturnDummyWork(mag, dummyprocno, nowprocno);
                    }

                    return false;
                }
            }
            catch (Exception ex)
            {
                errMsg = "ARMS開始登録で不明なエラーが発生:" + ex.ToString();
                return false;
            }
        }
        #endregion

        #region WorkEndInlineLot

        /// <summary>
        /// 作業完了登録
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        public bool WorkEndInlineLot(string lotno, out string errMsg) 
        {
            AsmLot lot = AsmLot.GetAsmLot(lotno);
            if (lot == null)
            {
                errMsg = "Svrロットが見つかりません:" + lotno;
                return false;
            }

            MachineInfo machine = MachineInfo.GetMachine(this.MacNo);
            if (machine == null)
            {
                errMsg = "Svr装置情報が見つかりません:" + this.MacNo;
                return false;
            }

            Magazine[] mags = Magazine.GetMagazine(lotno, true);
            if (mags.Count() == 0)
            {
                errMsg = "Svrマガジンが見つかりません:" + lotno;
                return false;
            }
            Magazine mag = mags.Single();

            Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
            if (p == null)
            {
                errMsg = "次工程情報が見つかりません:" + mag.NowCompProcess;
                return false;
            }

			Order order = Order.GetMagazineOrder(lotno, p.ProcNo);
			if (order == null)
			{
				throw new ApplicationException(string.Format("開始実績が存在しません。MagazineNo:{0}", lotno));
			}

			VirtualMag vMag = new VirtualMag();
			vMag.MagazineNo = mag.MagazineNo;
			vMag.LastMagazineNo = mag.MagazineNo;
			vMag.WorkStart = order.WorkStartDt;
			vMag.WorkComplete = DateTime.Now;
			vMag.ProcNo = p.ProcNo;
			
			if (this.WorkComplete(vMag, this, true))
			{
				errMsg = "";
				return true;
			}
			else 
			{
				errMsg = string.Format("[完了処理] 登録失敗 MagazineNo:{0}", lotno);
				return false;
			}
        }
        #endregion

		//private WorkCondition.MapFrameSize GetFrameSizeForInline(string mg)
		//{
		//	string lotno = mg.Replace(AsmLot.PREFIX_DEVIDED_INLINE_LOT, "");
		//	Order[] orders = Order.GetOrder(lotno);

		//	foreach (Order o in orders)
		//	{
		//		Material[] matlist = o.GetMaterials();

		//		foreach (Material mat in matlist)
		//		{
		//			WorkCondition.MapFrameSize size = WorkCondition.GetMapFrameSize(mat.MaterialCd);
		//			if (size != WorkCondition.MapFrameSize.None)
		//			{
		//				return size;
		//			}
		//		}
		//	}

		//	return WorkCondition.MapFrameSize.None;
		//}

        /// <summary>
        /// アンローダー指定位置のマガジン情報を取得
        /// </summary>
        /// <param name="raw"></param>
        /// <param name="startOffset"></param>
        /// <param name="plc"></param>
        /// <returns></returns>
        public VirtualMag parseMagazine(string rawMgNo, DateTime start, DateTime complete)
        {
            string lotno;
            int seqno;

            if (rawMgNo.StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
            {
                lotno = rawMgNo.Split(' ')[1];
                seqno = int.Parse(rawMgNo.Split(' ')[2]);
            }
            else if (rawMgNo.StartsWith(AsmLot.PREFIX_INLINE_LOT))
            {
                lotno = rawMgNo.Split(' ')[1];
                seqno = 0;
            }
            else if (rawMgNo.StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE)) 
            {
                lotno = Magazine.GetCurrent(rawMgNo.Split(' ')[1]).NascaLotNO;
                seqno = Order.ParseMagSeqNo(lotno);
            }
            else
            {
                lotno = rawMgNo;
                seqno = 0;
            }

            AsmLot asmLot = AsmLot.GetAsmLot(lotno);
            Process proc = Process.GetProcess("MD0081");

            Process.MagazineDevideStatus st = Process.GetMagazineDevideStatus(asmLot, proc.ProcNo);
            if (st == Process.MagazineDevideStatus.Double || st == Process.MagazineDevideStatus.SingleToDouble)
            {
                lotno = Order.NascaLotToMagLot(lotno, seqno);
            }

            VirtualMag mag = new VirtualMag();

            Magazine svrmag = Magazine.GetCurrent(lotno);
            if (svrmag == null)
            {
                //分割なしマガジンも検索
                svrmag = Magazine.GetCurrent(Order.MagLotToNascaLot(lotno));
                if (svrmag == null)
                {
                    throw new ApplicationException("マガジン情報が見つかりません" + lotno);
                }
                else
                {
                    mag.MagazineNo = svrmag.MagazineNo;
                    mag.LastMagazineNo = svrmag.MagazineNo;
                }
            }

            mag.MagazineNo = lotno;
            mag.LastMagazineNo = mag.MagazineNo;
            mag.WorkStart = start;
            mag.WorkComplete = complete;

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
            mag.ProcNo = nextproc.ProcNo;

            return mag;
        }
    }
}
