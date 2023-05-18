//#if DEBUG
//using ArmsApi.local.nichia.naweb_dev;
//#else
//using ArmsApi.local.nichia.naweb;
//#endif
using ArmsApi.local.nichia.naweb;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ArmsApi.Model.NASCA
{
    public delegate void ExportLogEventHandler(string msg);
    public delegate void ExportLotCompleteEventHandler(string lotno, string msg);
    public delegate void ExportLotErrorEventHandler(string lotno, string procnm, string sendcmd, string receivemsg);

    public class Exporter
    {
        private const int DB_PROC_NO = 1;
        private string LOT_RECORD_MUTEX_NAME;

        /// <summary>
        /// MAPバルクオフダイサー識別
        /// </summary>
        private const string MAP_BULK_OFF_DICER_LOTCHAR_CD = "P0000084";
        private const string MAP_BULK_OFF_DICER_LISTVAL = "16";
        private const string MAP_BULK_OFF_DICER_PLANTCD_LOTCHAR_CD = "P0000061";

        public const string SYNC_COMPLETE_STRING = "連携完了";

        /// <summary>
        /// PDA用のFromToRootsフォルダを使うかのフラグ
        /// </summary>
        public bool IsSecondary { get; set; }

		/// <summary>
		/// 連携最終作業を指定する時に設定
		/// </summary>
		public Process TargetEndProcess { get; set; }

        #region Singleton
        /// <summary>
        /// singleton
        /// </summary>
        private static Exporter instance;

        /// <summary>
        /// PDA連携用の2ndインスタンス
        /// </summary>
        private static Exporter instance2;


        private Exporter(bool isSecondary)
        {
            this.IsSecondary = isSecondary;

            if (IsSecondary)
            {
                LOT_RECORD_MUTEX_NAME = "ArmsExporterMutex2";
            }
            else
            {
                LOT_RECORD_MUTEX_NAME = "ArmsExporterMutex1";
            }

            this.asmlotQueue = new Queue<AsmLot>();
            this.cutblendQueue = new Queue<string>();
        }


        public static Exporter GetInstance()
        {
            return GetInstance(false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="isSecondary"></param>
        /// <returns></returns>
        public static Exporter GetInstance(bool isSecondary)
        {
            if (isSecondary == false)
            {
                if (instance == null) instance = new Exporter(isSecondary);
                return instance;
            }
            else
            {
                if (instance2 == null) instance2 = new Exporter(isSecondary);
                return instance2;
            }
        }

        #endregion

        #region 処理キュー関連

        private System.Threading.Mutex queueMutex = new System.Threading.Mutex(false, "armsExporeterQueueMutex");

        private Queue<string> cutblendQueue;

        private Queue<AsmLot> asmlotQueue;

        public void Enqueue(string cutblendlot)
        {
            if (this.cutblendQueue.Contains(cutblendlot)) return;
            else
            {
                try
                {
                    queueMutex.WaitOne();
                    cutblendQueue.Enqueue(cutblendlot);
                }
                finally
                {
                    queueMutex.ReleaseMutex();
                }
            }
        }

        public void Enqueue(AsmLot lot)
        {
            if (this.asmlotQueue.Contains(lot)) return;
            else
            {
                try
                {
                    queueMutex.WaitOne();
                    asmlotQueue.Enqueue(lot);
                }
                finally
                {
                    queueMutex.ReleaseMutex();
                }
            }
        }
        #endregion

        public void SyncAllQueue()
        {
            if (IsRunning == false)
            {
                IsRunning = true;
                Action act = new Action(sweepQueue);
                act.BeginInvoke(new AsyncCallback(queueSweepComplete), null);
            }
        }

        private void queueSweepComplete(IAsyncResult res)
        {
            this.IsRunning = false;
        }

        #region sweepQueue

        private void sweepQueue()
        {
            StopRequest = false;

            try
            {
                AsmLot lot;
                while (asmlotQueue.Count > 0)
                {
                    try
                    {
                        queueMutex.WaitOne();
                        lot = asmlotQueue.Dequeue();
                    }
                    finally
                    {
                        queueMutex.ReleaseMutex();
                    }

                    if (lot != null)
                    {
                        NASCAResponse res = SendAsmLotAllProc(lot);
                        if (OnLotComplete != null) OnLotComplete(lot.NascaLotNo, res.OriginalMessage);
                    }

                    if (StopRequest == true) return;
                }

                string cutblendlot = null;
                while (cutblendQueue.Count > 0)
                {
                    try
                    {
                        queueMutex.WaitOne();
                        cutblendlot = cutblendQueue.Dequeue();
                    }
                    finally
                    {
                        queueMutex.ReleaseMutex();
                    }

                    if (cutblendlot != null)
                    {
                        if (Config.GetLineType == Config.LineTypes.MEL_MAP)
                        {
							NASCAResponse res = SendAllDataForMelMap(cutblendlot);
							if (OnLotComplete != null) OnLotComplete(cutblendlot, res.OriginalMessage);
                        }
                        else if (Config.GetLineType == Config.LineTypes.MEL_MPL || Config.GetLineType == Config.LineTypes.MEL_83385
                            || Config.GetLineType == Config.LineTypes.MEL_COB || Config.GetLineType == Config.LineTypes.MEL_NTSV)
                        {
                            if (Config.GetLineType == Config.LineTypes.MEL_NTSV)
                            {
                                NASCAResponse res = SendAllDataForMel2(cutblendlot, false);
                                if (OnLotComplete != null) OnLotComplete(cutblendlot, res.OriginalMessage);
                            }
                            else
                            {
                                NASCAResponse res = SendAllDataForMel2(cutblendlot, true);
                                if (OnLotComplete != null) OnLotComplete(cutblendlot, res.OriginalMessage);
                            }

                        }
                        else
						{
							NASCAResponse res = SendAllData(cutblendlot);
							if (OnLotComplete != null) OnLotComplete(cutblendlot, res.OriginalMessage);
						}

                    }

                    if (StopRequest == true) return;
                }

                if (OnComplete != null) OnComplete(this, EventArgs.Empty);
            }
            finally
            {
                IsRunning = false;
            }
        }
        #endregion

        public bool StopRequest { get; set; }

        public bool IsRunning { get; private set; }

        public event ExportLogEventHandler OnStatusUpdate;

        public event ExportLotCompleteEventHandler OnLotComplete;

        public event EventHandler OnComplete;

        public event ExportLotErrorEventHandler OnError;

        protected void logact(string msg)
        {
            Log.SysLog.Info(msg);
            if (OnStatusUpdate != null) OnStatusUpdate(msg);
        }

        #region OrderデータNASCA連携関係
        /// <summary>
        /// 工程作業履歴をNASCAに登録、期間中の装置材料割り付けレコードも全て登録
        /// 成功時はisNascaEnd=trueでorder更新
        /// </summary>
        /// <param name="ord"></param>
        /// <param name="tempLotNo"></param>
        /// <returns></returns>
        public NASCAResponse SendOrderToNasca(AsmLot lot, Process p, bool isAvailableWaitProcess, string sendNascaLotNo)
        {
            Mutex lotmut = new Mutex(false, LOT_RECORD_MUTEX_NAME);
            if (lotmut.WaitOne(1000))
            {
                try
                {
                    if (lot == null) return NASCAResponse.GetNGResponse("ロット情報が見つかりません");

                    // 流動規制のレコードが1レコード以上、存在する場合、連携異常扱いとする
                    Restrict[] recList = Restrict.SearchRestrict(lot.NascaLotNo, p.ProcNo, true);
                    if (recList.Length > 0)
                    {
                        string recMsg = null;
                        foreach (Restrict rec in recList)
                        {
                            if(!string.IsNullOrEmpty(recMsg)){ recMsg += ","; }
                            recMsg += rec.Reason.Trim();
                        }

                        return NASCAResponse.GetNGResponse("流動規制が解除されていません。規制理由：" + recMsg);
                    }


                    Order[] orders = Order.GetOrder(lot.NascaLotNo, p.ProcNo);
                    if (orders.Length == 0) return NASCAResponse.GetOKResponse();

                    if (orders.Where(o => !o.IsNascaEnd && o.IsComplete && !o.IsNascaRunning).Count() == 0)
                    {
                        return NASCAResponse.GetOKResponse();
                    }

                    //マガジン分割状態で完成する工程の場合、両マガジンが完了状態でなければ連携不可
                    Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);
                    if (dst == Process.MagazineDevideStatus.Double || dst == Process.MagazineDevideStatus.SingleToDouble)
                    {
                        if (orders.Where(o => o.IsComplete).Count() < 2)
                        {
                            return NASCAResponse.GetOKResponse();
                        }
                    }

                    int ordermove = Process.GetOrderMove(lot.TypeCd, p.ProcNo);

					//NASCA連携不要工程
					if (ordermove == -1)
					{
						foreach (Order o in orders)
						{
							o.IsNascaEnd = true;
							o.DeleteInsert(o.LotNo);
						}
						return NASCAResponse.GetOKResponse();
					}
					
					//NASCA連携待機工程 ※SLS1ブレイク作業(5)まで連携を待つ仕様
					if (ordermove == 4 && isAvailableWaitProcess)
					{
						return NASCAResponse.GetOKResponse();
					}
					else if (ordermove == 4 && isAvailableWaitProcess == false)
					{
						CutBlend[] cutblendList = CutBlend.SearchBlendRecord(null, sendNascaLotNo, null, false, false);
						if (cutblendList != null && cutblendList.ToList().Exists(c => Order.GetMagazineOrder(c.LotNo, p.ProcNo).IsNascaEnd 
							&& Order.GetMagazineOrder(c.LotNo, p.ProcNo).MacNo == orders[0].MacNo)) 
						{
							//同装置での作業実績した
							return NASCAResponse.GetOKResponse();
						}

						//カット指図発行が終わって呼び出された時を想定して待機解除するが、
						//ordermove発行はさせない為に、0にする
						ordermove = 0;
					}

                    #region ordermove発行

                    //指図発行必要工程
                    if (ordermove != 0)
                    {
                        if (orders.Where(o => o.IsNascaStart != true).Count() >= 1)
                        {
                            NppNelOrderMoveParamInfo orderMoveInfo; string orderMoveString;
                            //最初の工程でLotsizeありの場合
                            if (ordermove == 1 && lot.LotSize != 0)
                            {
                                orderMoveInfo 
                                    = NascaPubApi.GetOrderMoveParamInfo(lot.NascaLotNo, ordermove, lot.ProfileId, orders[0].WorkStartDt, lot.LotSize, out orderMoveString);
                                //ordercmd = Spider.GetOrderCommand(lot.NascaLotNo, lot.ProfileId.ToString(), ordermove, orders[0].WorkStartDt, lot.LotSize);
                            }
                            else
                            {
                                orderMoveInfo
                                    = NascaPubApi.GetOrderMoveParamInfo(lot.NascaLotNo, ordermove, lot.ProfileId, orders[0].WorkStartDt, out orderMoveString);
                                //string ordercmd = Spider.GetOrderCommand(lot.NascaLotNo, lot.ProfileId.ToString(), ordermove, orders[0].WorkStartDt);
                            }

                            logact("NASCA SEND:" + orderMoveString);
                            //NASCAResponse orderres = Spider.SendNASCACommandToSpider(orderMoveString, 0, this.IsSecondary, LOT_RECORD_MUTEX_NAME);
                            NascaPubApi api = NascaPubApi.GetInstance();
                            NASCAResponse orderRes = api.WriteOrderMove(orderMoveInfo, true);
                            logact("NASCA RECEIVE:" + orderRes.OriginalMessage);

                            if (orderRes.Status != NASCAStatus.OK)
                            {
                                if (OnError != null) OnError(lot.NascaLotNo, Process.GetProcess(orders[0].ProcNo).InlineProNM, orderMoveString, orderRes.OriginalMessage);
                                return orderRes;
                            }
                            else
                            {
                                foreach (Order o in orders)
                                {
                                    o.IsNascaStart = true;
                                    o.DeleteInsert(o.LotNo);
                                }
                            }
                        }
                    }
                    #endregion

                    //同工程、自作業より前作業にマガジン分割作業が存在する、実績が1件の場合、2件目を作成する
                    List<Process> prevWorkflows = Process.GetPrevProcess(lot.TypeCd, p.ProcGroupCd, p.WorkOrder).ToList();
                    if (prevWorkflows.Exists(w => Process.GetMagazineDevideStatus(lot, w.ProcNo) == Process.MagazineDevideStatus.Double
                                               || Process.GetMagazineDevideStatus(lot, w.ProcNo) == Process.MagazineDevideStatus.SingleToDouble))
                    {
                        if (orders.Length == 1)
                        {
                            orders[0].DevidedMagazineSeqNo = 1;
                            orders[0].NascaDummmyOrderNo = 1;

                            Order[] doubleOrders = Order.GetOrder(lot.NascaLotNo, p.ProcNo);
                            doubleOrders[0].DevidedMagazineSeqNo = 2;
                            doubleOrders[0].NascaDummmyOrderNo = 2;

                            List<Order> dOrders = orders.ToList();
                            dOrders.Add(doubleOrders[0]);

                            orders = dOrders.ToArray();
                        }
                    }

                    NASCAResponse res = NASCAResponse.GetNGResponse("アッセンロット連携異常");
                    foreach (Order o in orders)
                    {
                        if (o.IsNascaEnd) continue;
						if (o.IsNascaRunning) continue;

						if (p.IsNascaDefect == true && o.IsDefectEnd == false)
						{
							return NASCAResponse.GetNGResponse("ARMSの不良登録が未完了です。");
						}

						try
						{
                            updateNascaRunningStatus(o, true);

							#region 検査数　実績2つの場合は最大値を設定
							int inspectct = 0;

							if (orders.Where(r => r.InspectCt < 0).Count() >= 1)
							{
								//全数検査になっているレコードがあれば全数扱い
								inspectct = -1;
							}
							else
							{
								inspectct = orders.Max(r => r.InspectCt);
							}
							#endregion

							//コメント
							string comment = string.Join(" ", orders.Select(r => r.Comment).ToArray());

                            #region 樹脂・原材料・不良情報・金型取得

                            MachineInfo machine = MachineInfo.GetMachine(o.MacNo, true);
							Process proc = Process.GetProcess(o.ProcNo);

							Material[] materials = o.GetMaterials();
							Resin[] resins = o.GetResins();
							Defect def = Defect.GetDefect(lot.NascaLotNo, proc.ProcNo);

                            //金型
                            WorkCondition[] cond = machine.GetWorkConditions(o.WorkStartDt, o.WorkEndDt);

                            string[] parts = null;
                            if (cond != null && cond.Length > 0)
                            {
                                try
                                {
                                    parts = cond.Where(c => c.CondCd == CutBlend.PRESSDIE_WORKCOND_CD).Select(c => c.CondVal).ToArray();

                                    if (parts.Length == 0)
                                        parts = null;
                                }
                                catch (Exception ex)
                                {
                                    Log.SysLog.Fatal("NASCA連携時　金型取得エラー　金型無しで継続:" + o.LotNo + ":" + o.MacNo + ":" + ex.ToString());
                                    parts = null;
                                }
                            }

                            #endregion

                            string mnfctString;
							NppNelMnfctParamInfo mnfctInfo = NascaPubApi.GetMnfctParamInfo(sendNascaLotNo,
								(o.DevidedMagazineSeqNo == 0 ? "" : o.DevidedMagazineSeqNo.ToString()), //マガジン連番0なら空白
								machine.NascaPlantCd, o.WorkStartDt, o.WorkEndDt,
                                parts, def.DefItems.ToArray(), materials,
								resins, o.TranStartEmpCd, o.TranCompEmpCd,
								comment, o.InspectEmpCd, inspectct, out mnfctString);

							logact("NASCA SEND:" + mnfctString);
							NascaPubApi api = NASCA.NascaPubApi.GetInstance();
							res = api.WriteMnfctResult(mnfctInfo, true);
							logact("NASCA RECEIVE:" + res.OriginalMessage);

							if (res.Status == NASCAStatus.OK)
							{
								updateNascaEndStatus(o, true);

								//最終工程で稼働中フラグがTrueの場合、マガジンとロット切り離し
								bool isLastProc = Process.IsLastProcess(p, lot.TypeCd);
								Magazine[] mags = Magazine.GetMagazine(o.LotNo, true);
                                if (mags.Count() == 0)
                                {
                                    mags = Magazine.GetMagazine(o.NascaLotNo, true);
                                }
								if (isLastProc && mags.Count() != 0 && o.NascaDummmyOrderNo != 1)
								{
									//ロットに紐付き、有効な全キャリアの解放 ※バグ持ちで有効に働かないので一旦コメントアウト2016.11.15
                                    //NppCarrierLotInfo[] carriers = api.GetCarrierData(o.LotNo, true);
                                    //foreach (NppCarrierLotInfo carrier in carriers)
                                    //{
                                    //    if (carrier.DelFG == false && carrier.OperateFG)
                                    //        api.ReleaseCarrierNo(o.LotNo, carrier.CarrierNO, true);
                                    //}

									Magazine.UpdateNewFgOff(mags.Single().MagazineNo);	
								}

                                //DMCのカセット用処理。（有効なカセットの紐付解放）
                                List<string> listcassettes = Cassette.GetCassette(o.LotNo, 1);
                                if (isLastProc && listcassettes.Count() != 0)
                                {
                                    Cassette.DeleteNewFgOff(o.LotNo);
                                }
							}
							else
							{
								if (OnError != null) OnError(lot.NascaLotNo, proc.InlineProNM, mnfctString, res.OriginalMessage);
							}

							if (res.Status != NASCAStatus.OK)
							{
								//配信先切り分け 2012/08/28 SGA32 Sasaki
								Log.SysLog.Fatal("アッセンロット連携異常:" + lot.NascaLotNo + ":" + p.InlineProNM + ":" + res.Message);
								return res;
							}
						}
						finally 
						{
                            updateNascaRunningStatus(o, false);
						}
                    }

                    return res;
                }
                finally
                {
                    lotmut.ReleaseMutex();
                    lotmut.Close();				
                }
            }
            else
            {
                lotmut.Close();
                if (StopRequest) return NASCAResponse.GetOKResponse();
                Thread.Sleep(1000);
                return SendOrderToNasca(lot, p);
            }
        }
		public NASCAResponse SendOrderToNasca(AsmLot lot, Process p) 
		{
			return SendOrderToNasca(lot, p, true, lot.NascaLotNo);
		}

		/// <summary>
		/// isNascaRunningのステータス更新
		/// </summary>
		/// <param name="o"></param>
		/// <param name="lotno"></param>
		/// <param name="isNascaRuning"></param>
		private void updateNascaRunningStatus(Order o, bool isNascaRuning)
		{
			string argLotNo = o.LotNo;
			string runningLotNo = o.LotNo;

			if (o.NascaDummmyOrderNo == 1 || o.NascaDummmyOrderNo == 2)
			{
				//NASCA連携用に分割データを生成している場合は対象を通常ロット(#無し)に変更
				runningLotNo = o.NascaLotNo;
			}

			o.IsNascaRunning = isNascaRuning;
			o.DeleteInsert(runningLotNo);

			//インスタンスのロット番号を元に戻す
			o.LotNo = argLotNo;
		}

		private void updateNascaEndStatus(Order o, bool isNascaEnd) 
		{
			string argLotNo = o.LotNo;
			string endLotNo = o.LotNo;

			if (o.NascaDummmyOrderNo == 1)
			{
				//分割データの1つ目は何もしない
				return;
			}
			else if (o.NascaDummmyOrderNo == 2)
			{
				//NASCA連携用に分割データを生成している場合は対象を通常ロット(#無し)に変更
				endLotNo = o.NascaLotNo;
			}

			o.IsNascaEnd = isNascaEnd;
			o.DeleteInsert(endLotNo);

			//インスタンスのロット番号を元に戻す
			o.LotNo = argLotNo;
		}

        #region ExportLotChar

        public bool ExportLotChar(string lotNo, string typecd)
        {
            try
            {
                LotChar[] lclist = WorkCondition.GetLotCharFromLot(lotNo);

                NascaPubApi api = NASCA.NascaPubApi.GetInstance();
                foreach (LotChar lc in lclist)
                {
                    // ライフ試験数特性(T0000001)のみ、特性付与しない(カット指図発行時にNASCA側で試験数の多いアッセンロットの特性を引き継ぐ処理がある為)
                    if (lc.LotCharCd == LotChar.LIFETESTCT_LOTCHARCD)
                        continue;

                    api.WriteLotChar(lotNo, typecd, lc.LotCharCd, lc.LotCharVal, lc.ListVal, true, true);
                    logact("特性付与:" + lotNo + ":" + lc.LotCharCd + ":" + lc.LotCharVal + ":" + lc.ListVal);
                }

                Inspection[] isplist = Inspection.GetInspections(lotNo);
                foreach (Inspection isp in isplist)
                {
                    if (isp.ProcNo == DB_PROC_NO)
                    {
                        api.WriteLotChar(lotNo, typecd, Importer.DB_INSPECITON_END, "", "1", true, true);
                        logact("特性付与:" + lotNo + ":" + Importer.DB_INSPECITON_END + "::1");

                        api.WriteLotChar(lotNo, typecd, Importer.DB_INSPECTION_NEED, "", "1", true, true);
                        logact("特性付与:" + lotNo + ":" + Importer.DB_INSPECTION_NEED + "::1");
                    }

                    if (isp.ProcNo == Config.Settings.WBInspectionProcNo)
                    {
                        api.WriteLotChar(lotNo, typecd, Importer.WB_INSPECTION_NEED, "", "1", true, true);
                        logact("特性付与:" + lotNo + ":" + Importer.WB_INSPECTION_NEED + "::1");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.SysLog.Fatal(ex.ToString());
                return false;
            }
        }
        #endregion

        #endregion

        #region アッセンロットの未連携データを全て送信

        public NASCAResponse SendAsmLotAllProc(AsmLot lot)
        {
            return SendAsmLotAllProc(lot, false, true, lot.NascaLotNo);
        }

		public NASCAResponse SendAsmLotAllProc(AsmLot lot, bool isBackgroundProcess)
		{
			return SendAsmLotAllProc(lot, isBackgroundProcess, true, lot.NascaLotNo);
		}

        public NASCAResponse SendAsmLotAllProc(AsmLot lot, bool isBackgroundProcess, bool isAvailableWaitProcess, string sendNascaLotNo)
        {
            Process[] workFlow = Process.GetWorkFlow(lot.TypeCd);
            Order[] orders = Order.GetOrder(lot.NascaLotNo).OrderBy(o => o.WorkStartDt).ToArray();

            Log.SysLog.Info("アッセンロット連携:" + lot.NascaLotNo);

            //ロット特性更新はバックグラウンドプロセスでは行わない
            if (lot.IsNascaLotCharEnd == false && !isBackgroundProcess)
            {
                logact("ロット特性更新処理開始:" + lot.NascaLotNo);
                bool isSuccess = ExportLotChar(lot.NascaLotNo, lot.TypeCd);

                if (isSuccess == true)
                {
                    logact("ロット特性更新処理終了:" + lot.NascaLotNo);
                }
                else
                {
                    return NASCAResponse.GetNGResponse("ロット特性更新処理失敗" + lot.NascaLotNo);
                }
            }

			//連携最終作業を設定しているが、作業順に存在しない場合は連携無し
			if (TargetEndProcess != null && workFlow.Where(w => w.ProcNo == TargetEndProcess.ProcNo).Count() == 0)
				return NASCAResponse.GetOKResponse();

            foreach (Process p in workFlow)
            {
                //最終工程はここでは連携無し(MAP高効率,SLS2だけ例外。最終工程がカットではない）
				if (p.FinalSt) continue;

                //最終工程以降もここでは連携無し
                Process finalProc = workFlow.Where(r => r.FinalSt == true).FirstOrDefault();
                if (finalProc != null && finalProc.WorkOrder < p.WorkOrder)
                    continue;

                //外部バッチでの自動連携の場合、最新工程の連携なし
                if (isBackgroundProcess)
                {
                    #region 最新工程判定
                    Process nextProc = Process.GetNextProcess(p.ProcNo, lot);
                    if (nextProc == null) continue;

                    Order[] nextOrder = Order.GetOrder(lot.NascaLotNo, nextProc.ProcNo);

                    if (nextOrder.Length == 0 || nextOrder.Where(o => o.IsComplete == false).Count() >= 1) continue;
                    #endregion

                    #region 排他制御　一瞬でも他プロセスと競合すれば処理終了

                    Mutex lotmut = new Mutex(false, LOT_RECORD_MUTEX_NAME);
                    try
                    {
                        if (lotmut.WaitOne(1))
                        {
                            lotmut.ReleaseMutex();
                        }
                        else
                        {
                            //一瞬でも他プロセスと競合したら処理終了
                            return NASCAResponse.GetOKResponse();
                        }
                    }
                    finally
                    {
                        lotmut.Close();
                    }

                    #endregion
                }

				NASCAResponse res = SendOrderToNasca(lot, p, isAvailableWaitProcess, sendNascaLotNo);
				if (res.Status != NASCAStatus.OK)
				{
					//配信先切り分け 2012/08/28 SGA32 Sasaki
					Log.SysLog.Fatal("アッセンロット連携異常:" + lot.NascaLotNo + ":" + p.InlineProNM + ":" + res.Message);
					return res;
				}

				//連携最終作業を設定している場合、その作業で処理を抜ける
				if (TargetEndProcess != null && TargetEndProcess.ProcNo == p.ProcNo) break;

				if (StopRequest) return res;
			}
            logact("アッセンロット連携完了:" + lot.NascaLotNo);

            return NASCAResponse.GetOKResponse();
        }
        #endregion

        #region SendAllData (NASCA連携)

        /// <summary>
        /// 前工程から全ての情報を連携する
        /// </summary>
        /// <param name="cutblendLot"></param>
        /// <returns></returns>
        public NASCAResponse SendAllData(string cutblendLot)
        {
            logact("カットブレンド連携開始" + cutblendLot);
            CutBlend[] cutblendList = CutBlend.SearchBlendRecord(null, cutblendLot, null, false, false);
            AsmLot firstLot = AsmLot.GetAsmLot(cutblendList[0].LotNo);
            Process[] workFlow = Process.GetWorkFlow(firstLot.TypeCd);
            Process cutProcess = workFlow.Where(p => p.FinalSt == true).FirstOrDefault();
            //カットブレンド指図は必ず1つの想定
            Order cutOrder = Order.GetMagazineOrder(cutblendLot, cutProcess.ProcNo);


            //完了登録済みなら未判定でOK
            if (cutOrder.IsNascaEnd == true)
            {
                //最終ステータス後の工程があればそれを連携
                if (workFlow.Where(r => r.WorkOrder > cutProcess.WorkOrder).Count() != 0)
                {
                    return SendAllDataAfterFinal(workFlow, cutblendLot, cutProcess);
                }
                else
                {
                    logact("連携済みのためスキップ" + cutblendLot);
                    return NASCAResponse.GetOKResponse();
                }
            }

            //カットラベル照合関係
			if (Config.Settings.HasCutLabelCompare)
			{
				if (Config.GetLineType == Config.LineTypes.NEL_SV || Config.GetLineType == Config.LineTypes.MEL_SV)
				{
					if (CutBlend.IsCutLabelCompareOK(cutblendLot) == false)
					{
						logact("カットラベル照合未完了のためスキップ" + cutblendLot);
						return NASCAResponse.GetNGResponse("カットラベル照合未完了のためスキップ");
					}
				}
			}

            #region 各アッセンロットの実績連携

            //アッセンロットの全工程を連携
            foreach (CutBlend cb in cutblendList)
            {
                AsmLot lot = AsmLot.GetAsmLot(cb.LotNo);
                if (lot == null) return NASCAResponse.GetNGResponse("ロットが見つかりません:" + cb.LotNo);

                NASCAResponse asmres = SendAsmLotAllProc(lot);
                if (asmres.Status != NASCAStatus.OK)
                {
                    return asmres;
                }

                if (StopRequest) return asmres;
            }
            #endregion

            #region 各アッセンロットのカット工程移動 orderwork

            //各アッセンロットをカット工程に移動
            foreach (CutBlend cb in cutblendList)
            {
                if (cb.IsNascaEnd) continue;

                string orderMoveString;
                NppNelOrderMoveParamInfo orderMoveInfo = NascaPubApi.GetOrderMoveParamInfo(cb.LotNo, 3, firstLot.ProfileId, cb.StartDt, out orderMoveString);
                logact("NASCA SEND:" + orderMoveString);
                NascaPubApi cbstartApi = NascaPubApi.GetInstance();
                NASCAResponse cbstartRes = cbstartApi.WriteOrderMove(orderMoveInfo, true);
                logact("NASCA RECEIVE:" + cbstartRes.OriginalMessage);
                if (cbstartRes.Status != NASCAStatus.OK)
                {
                    if (OnError != null) OnError(cb.LotNo, "カット", orderMoveString, cbstartRes.OriginalMessage);
                    return cbstartRes;
                }
                else
                {
                    cb.IsNascaEnd = true;
                    cb.DeleteInsert();
                }

                if (StopRequest) return cbstartRes;
            }
            #endregion

            #region カット指図発行コマンド cutblend

            //カット指図発行
            if (cutOrder.IsNascaStart == false)
            {
                logact("カット指図発行:" + cutblendLot);
                string orderString;
                NppNelCutBlendOrdParamInfo orderInfo = NascaPubApi.GetCutBlendOrderParamInfo(cutblendList, out orderString);
                logact("NASCA SEND:" + orderString);
                NascaPubApi api = NascaPubApi.GetInstance();
                NASCAResponse startRes = api.WriteCutBlendOrder(orderInfo, true);

                logact("NASCA RECEIVE:" + startRes.OriginalMessage);

                if (startRes.Status != NASCAStatus.OK)
                {
                    if (OnError != null) OnError(cutOrder.LotNo, "カット", orderString, startRes.OriginalMessage);
                    return startRes;
                }

                //開始成功でSQLite更新
                cutOrder.IsNascaStart = true;
                cutOrder.DeleteInsert(cutOrder.LotNo);
                logact("カット指図発行完了:" + cutblendLot);

                if (StopRequest) return startRes;
            }
            #endregion

			#region 各アッセンロットの待機工程実績連携

			//アッセンロットの全工程を連携
			foreach (CutBlend cb in cutblendList)
			{
				AsmLot lot = AsmLot.GetAsmLot(cb.LotNo);
				if (lot == null) return NASCAResponse.GetNGResponse("ロットが見つかりません:" + cb.LotNo);

				NASCAResponse asmres = SendAsmLotAllProc(lot, false, false, cb.BlendLotNo);
				if (asmres.Status != NASCAStatus.OK)
				{
					return asmres;
				}

				if (StopRequest) return asmres;
			}
			#endregion

            #region カット完了コマンド(mnfct)

            //カット完成
            //if (cutOrder.IsNascaDefectEnd == false)
            //{
                MachineInfo machine = MachineInfo.GetMachine(cutOrder.MacNo);
                WorkCondition[] cond = machine.GetWorkConditions(cutOrder.WorkStartDt, cutOrder.WorkEndDt);

                string[] parts;
                if (cond == null || cond.Length == 0)
                {
                    parts = new string[0];
                }
                else
                {
                    try
                    {
                        parts = cond.Where(c => c.CondCd == CutBlend.PRESSDIE_WORKCOND_CD).Select(c => c.CondVal).ToArray();
                    }
                    catch (Exception ex)
                    {
                        Log.SysLog.Fatal("カット送信時　金型取得エラー　金型無しで継続:" + cutOrder.LotNo + ":" + ex.ToString());
                        parts = new string[0];
                    }
                }

                Defect def = Defect.GetDefect(cutblendLot, cutOrder.ProcNo);
                Material[] materials = cutOrder.GetMaterials();

                logact("カット完了登録:" + cutblendLot);
                string mnfctString;



                string magno = null;
                if (Config.GetLineType == Config.LineTypes.NEL_MAP || Config.GetLineType == Config.LineTypes.MEL_MAP)
                {
                    //2016.3.10 MAP品目統合に於いてCT工程のマガジン区分(NASCA)がマガジン単位になるが、
                    //自動搬送#1～#8でカット実績を連携すると実績のマガジン番号が空白になって
                    //以後のPDA処理が出来なくなるため、MAPのカット連携の時のみマガジン番号を1にするよう
                    //緊急の突貫対応。(古川S許可済み!) yuasa
                    magno = "1";
                }
                
                NppNelMnfctParamInfo mnfctInfo = NascaPubApi.GetMnfctParamInfo(cutblendLot, magno, machine.NascaPlantCd,
                    cutOrder.WorkStartDt, cutOrder.WorkEndDt, parts, def.DefItems.ToArray(),
                    materials, null, cutOrder.TranStartEmpCd, cutOrder.TranCompEmpCd, cutOrder.Comment, cutOrder.InspectEmpCd, cutOrder.InspectCt, out mnfctString);

                logact("NASCA SEND:" + mnfctString);
                NascaPubApi endApi = NascaPubApi.GetInstance();
                NASCAResponse endRes = endApi.WriteMnfctResult(mnfctInfo, true);
                logact("NASCA SEND:" + endRes.OriginalMessage);

                if (endRes.Status != NASCAStatus.OK)
                {
                    if (OnError != null) OnError(cutblendLot, "カット", mnfctString, endRes.OriginalMessage);
                    return endRes;
                }
                //cutOrder.IsNascaDefectEnd = true;
                cutOrder.DeleteInsert(cutOrder.LotNo);
                logact("カット完了:" + cutblendLot);

                if (StopRequest) return endRes;
            //}
            #endregion

            //ロット特性の全更新
            bool success = ExportCutLotChar(cutblendLot, firstLot.TypeCd, cutblendList, logact);
            if (success)
            {
                //完了成功でSQLite更新
                cutOrder.IsNascaEnd = true;
                cutOrder.DeleteInsert(cutOrder.LotNo);
            }
            else
            {
                Log.SysLog.Fatal("カット完了失敗:" + cutblendLot);
                return NASCAResponse.GetNGResponse("ロット特性更新失敗:" + cutblendLot);
            }
            logact("カット完了成功:" + cutblendLot);


            #region  最終ステータス後の工程があればそれを連携
            if (workFlow.Where(r => r.WorkOrder > cutProcess.WorkOrder).Count() != 0)
            {
                return SendAllDataAfterFinal(workFlow, cutblendLot, cutProcess);
            }
            #endregion

            return NASCAResponse.GetOKResponse();
        }

        private NASCAResponse SendAllDataAfterFinal(Process[] workFlow, string cutblendLot, Process cutProcess)
        {
            int sendCount = 0;
            foreach (Process p in workFlow)
            {
                if (p.WorkOrder <= cutProcess.WorkOrder)
                    continue;

                Order order = Order.GetMagazineOrder(cutblendLot, p.ProcNo);
                if (order.IsNascaEnd == false)
                {
                    sendCount++;

                    MachineInfo machine = MachineInfo.GetMachine(order.MacNo);
                    WorkCondition[] cond = machine.GetWorkConditions(order.WorkStartDt, order.WorkEndDt);

                    string[] parts;
                    if (cond == null || cond.Length == 0)
                    {
                        parts = new string[0];
                    }
                    else
                    {
                        try
                        {
                            parts = cond.Select(c => c.CondVal).ToArray();
                        }
                        catch (Exception ex)
                        {
                            Log.SysLog.Fatal(p.InlineProNM + "送信時　製造条件取得エラー　製造条件無しで継続:" + cutblendLot + ":" + ex.ToString());
                            parts = new string[0];
                        }
                    }

                    Defect def = Defect.GetDefect(cutblendLot, order.ProcNo);
                    Material[] materials = order.GetMaterials();

                    string mnfctString;

                    string magno = null;
                    if (Config.GetLineType == Config.LineTypes.NEL_MAP || Config.GetLineType == Config.LineTypes.MEL_MAP)
                    {
                        //2016.3.10 MAP品目統合に於いてCT工程のマガジン区分(NASCA)がマガジン単位になるが、
                        //自動搬送#1～#8でカット実績を連携すると実績のマガジン番号が空白になって
                        //以後のPDA処理が出来なくなるため、MAPのカット連携の時のみマガジン番号を1にするよう
                        //緊急の突貫対応。(古川S許可済み!) yuasa
                        magno = "1";
                    }

                    NppNelMnfctParamInfo mnfctInfoAfterFinal = NascaPubApi.GetMnfctParamInfo(cutblendLot, magno, machine.NascaPlantCd,
                    order.WorkStartDt, order.WorkEndDt, parts, def.DefItems.ToArray(),
                    materials, null, order.TranStartEmpCd, order.TranCompEmpCd, order.Comment, order.InspectEmpCd, order.InspectCt, out mnfctString);

                    logact("NASCA SEND:" + mnfctString);
                    NascaPubApi endApi = NascaPubApi.GetInstance();
                    NASCAResponse endRes = endApi.WriteMnfctResult(mnfctInfoAfterFinal, true);
                    logact("NASCA SEND:" + endRes.OriginalMessage);

                    if (endRes.Status != NASCAStatus.OK)
                    {
                        if (OnError != null) OnError(cutblendLot, p.InlineProNM, mnfctString, endRes.OriginalMessage);
                        return endRes;
                    }
                    order.IsNascaEnd = true;
                    order.DeleteInsert(order.LotNo);
                    logact(p.InlineProNM + "完了:" + cutblendLot);
                }
            }

            if (sendCount == 0)
            {
                logact("連携済みのためスキップ" + cutblendLot);
            }

            return NASCAResponse.GetOKResponse();
        }

        #endregion

        #region SendAllDataForMelMap

        /// <summary>
        /// 前工程から全ての情報を連携する
        /// MAP高効率専用　カット指図発行
        /// </summary>
        /// <param name="cutblendLot"></param>
        /// <returns></returns>
        public NASCAResponse SendAllDataForMelMap(string lotno)
        {
            logact("NASCA連携開始" + lotno);
            AsmLot lot = AsmLot.GetAsmLot(lotno);
            Process[] workFlow = Process.GetWorkFlow(lot.TypeCd);
            Process finalProcess = workFlow.Where(p => p.WorkOrder == workFlow.Max(w => w.WorkOrder)).FirstOrDefault();
            //Process finalProcess = workFlow.Where(p => p.FinalSt == true).FirstOrDefault();

            Order[] finalOrder = Order.GetOrder(lotno, finalProcess.ProcNo);

            if (finalOrder.Length == 0 || finalOrder[0].IsNascaEnd == true)
            {
                logact("連携済みのためスキップ" + lotno);
                return NASCAResponse.GetOKResponse();
            }

            if (finalOrder.Where(o => o.IsComplete == false).Count() >= 1)
            {
                logact("未完了のためスキップ" + lotno);
                return NASCAResponse.GetOKResponse();
            }

            NASCAResponse asmres = SendAsmLotAllProc(lot);
            if (asmres.Status != NASCAStatus.OK)
            {
                return asmres;
            }

            if (StopRequest) return asmres;

            // ロットマーキング作業のMD工程→CT工程移動対応
            // 最終作業のorderMoveが0以外の時、カット指図の発行を行わない。(ここで中断)
            int ordermove = Process.GetOrderMove(lot.TypeCd, finalProcess.ProcNo);
            if (ordermove != 0) return asmres;


            //string cbstartCmd = Spider.GetOrderCommand(lot.NascaLotNo, lot.ProfileId.ToString(), 2, DateTime.Now);
            string orderMoveString; 
            NppNelOrderMoveParamInfo orderMoveInfo = NascaPubApi.GetOrderMoveParamInfo(lot.NascaLotNo, 2, lot.ProfileId, DateTime.Now, out orderMoveString);

            logact("NASCA SEND:" + orderMoveString);
            //NASCAResponse cbstartRes = Spider.SendNASCACommandToSpider(orderMoveString, 0, this.IsSecondary, LOT_RECORD_MUTEX_NAME);
            NascaPubApi api = NascaPubApi.GetInstance();
            NASCAResponse cbstartRes = api.WriteOrderMove(orderMoveInfo, true);
            logact("NASCA RECEIVE:" + cbstartRes.OriginalMessage);
            if (cbstartRes.Status != NASCAStatus.OK)
            {
                if (OnError != null) OnError(lot.NascaLotNo, "カット", orderMoveString, cbstartRes.OriginalMessage);
                return cbstartRes;
            }

            logact("MAP高効率ロット完了成功:" + lotno);

            return NASCAResponse.GetOKResponse();
        }

        #endregion

        #region SendAllDataForMel2

        /// <summary>
        /// 前工程から全ての情報を連携する
        /// カットブレンド作業が無い(TnCutBlend未使用)のライン用。
        /// isOrderMoveAtLastフラグがTrueだと全連携終わった後に次工程に指図未発行移動をする。
        /// 指図未発行移動はSLS2(MPL, 83/385/COB)高効率用。NTSVはCT工程まで連携するので移動無し。
        /// </summary>
        /// <param name="cutblendLot"></param>
        /// <returns></returns>
        public NASCAResponse SendAllDataForMel2(string lotno, bool isOrderMoveAtLast)
        {
			logact("NASCA連携開始" + lotno);
			AsmLot lot = AsmLot.GetAsmLot(lotno);
			Process[] workFlow = Process.GetWorkFlow(lot.TypeCd);
            Process finalProcess = workFlow.Where(p => p.WorkOrder == workFlow.Max(w => w.WorkOrder)).FirstOrDefault();
            //Process finalProcess = workFlow.Where(p => p.FinalSt == true).FirstOrDefault();

			Order[] finalOrder = Order.GetOrder(lotno, finalProcess.ProcNo);

			if (finalOrder.Length == 0 || finalOrder[0].IsNascaEnd == true)
			{
				logact("連携済みのためスキップ" + lotno);
				return NASCAResponse.GetOKResponse();
			}

			if (finalOrder.Where(o => o.IsComplete == false).Count() >= 1)
			{
				logact("未完了のためスキップ" + lotno);
				return NASCAResponse.GetOKResponse();
			}

			NASCAResponse asmres = SendAsmLotAllProc(lot);
			if (asmres.Status != NASCAStatus.OK)
			{
				return asmres;
			}

			if (StopRequest) return asmres;
            
            if (isOrderMoveAtLast == false) //最後の保管場所移動をしない場合はここまで。
            {
                logact("高効率ロット完了成功:" + lotno);
                return asmres; 
            }

            string orderMoveString;
			NppNelOrderMoveParamInfo orderMoveInfo = NascaPubApi.GetOrderMoveParamInfo(lot.NascaLotNo, 3, lot.ProfileId, DateTime.Now, out orderMoveString);

			logact("NASCA SEND:" + orderMoveString);
			NascaPubApi api = NascaPubApi.GetInstance();
			NASCAResponse cbstartRes = api.WriteOrderMove(orderMoveInfo, true);
			logact("NASCA RECEIVE:" + cbstartRes.OriginalMessage);
			if (cbstartRes.Status != NASCAStatus.OK)
			{
				if (OnError != null) OnError(lot.NascaLotNo, "カット", orderMoveString, cbstartRes.OriginalMessage);
				return cbstartRes;
			}

			logact("高効率ロット完了成功:" + lotno);

			return NASCAResponse.GetOKResponse();
		}

		#endregion

        #region ExportCutLotChar

        /// <summary>
        /// ロット特性連携（カット工程版　全アッセンロットの資材、不良からも特性判定)
        /// </summary>
        /// <param name="blendlotNo"></param>
        /// <param name="typecd"></param>
        /// <param name="blendlist"></param>
        /// <param name="logact"></param>
        /// <returns></returns>
        public bool ExportCutLotChar(string blendlotNo, string typecd, CutBlend[] blendlist, Action<string> logact)
        {
            try
            {
                LotChar[] lclist = WorkCondition.GetLotCharFromLot(blendlotNo);

                NascaPubApi api = NASCA.NascaPubApi.GetInstance();

                foreach (LotChar lc in lclist)
                {
                    // ライフ試験数特性(T0000001)のみ、特性付与しない(カット指図発行時にNASCA側で試験数の多いアッセンロットの特性を引き継ぐ処理がある為)
                    if (lc.LotCharCd == LotChar.LIFETESTCT_LOTCHARCD)
                        continue;

                    api.WriteLotChar(blendlotNo, typecd, lc.LotCharCd, lc.LotCharVal, lc.ListVal, true, true);
                    logact("特性付与:" + blendlotNo + ":" + lc.LotCharCd + ":" + lc.LotCharVal + ":" + lc.ListVal);
                }


                //MAPライン判定
                if (Config.GetLineType == Config.LineTypes.NEL_MAP)
                {
                    api.WriteLotChar(blendlotNo, typecd, MAP_BULK_OFF_DICER_LOTCHAR_CD, "", MAP_BULK_OFF_DICER_LISTVAL, true, true);
                    CutBlend[] cbs = CutBlend.SearchBlendRecord(null, blendlotNo, null, false, false);
                    if (cbs.Length >= 1)
                    {
                        MachineInfo m = MachineInfo.GetMachine(cbs[0].MacNo);
                        if (m != null) api.WriteLotChar(blendlotNo, typecd, MAP_BULK_OFF_DICER_PLANTCD_LOTCHAR_CD, m.NascaPlantCd, "", true, true);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Log.SysLog.Fatal(ex.ToString());
                return false;
            }
        }
        #endregion
    }
}
