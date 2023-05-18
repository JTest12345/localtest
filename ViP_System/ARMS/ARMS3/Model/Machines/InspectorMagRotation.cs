using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 検査機(車載自動化用) SRAIM
    /// </summary>
    public class InspectorMagRotation : Inspector
    {
        private const int DATA_NO_COL = 0;
        private const int MAG_NO_COL = 3;
        private const int MM_OPENINGCHECKFG_COL = 23;
        private const int SM_OPENINGCHECKFG_COL = 27;
        private const string OPENINGCHECKFG_ON = "1";

        /// <summary>
        /// 始業点検マガジン投入アドレス
        /// </summary>
        public string OpeningCheckMagazineInputWordAddress { get; set; }

        /// <summary>
        /// 始業点検マガジン排出アドレス
        /// </summary>
        public string OpeningCheckMagazineOutputWordAddress { get; set; }

		#region 高効率用

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
        /// コンベア排出予約アドレス
        /// </summary>
        public string OutputReserveBitAddress { get; set; }

        /// <summary>
        /// ローダー側マガジンNo読取完了アドレス
        /// </summary>
        public string LoaderQRReadCompleteBitAddress { get; set; }

        /// <summary>
        /// 開始登録OKBit
        /// </summary>
        public string WorkStartOKBitAddress { get; set; }

        /// <summary>
        /// 開始登録NGBit
        /// </summary>
        public string WorkStartNGBitAddress { get; set; }

        /// <summary>
        /// 完了登録NGBit
        /// </summary>
        public string WorkCompleteNGBitAddress { get; set; }

        /// <summary>
        /// 作業開始登録を本体側でするフラグ
        /// </summary>
        public bool IsWorkStartAutoComplete { get; set; }

        /// <summary>
        /// 作業完了登録を本体側でするフラグ
        /// </summary>
        public bool IsWorkEndAutoComplete { get; set; }

        /// <summary>
        /// 1マガジンデータ長
        /// </summary>
        private const int MAGAZINE_DATA_LENGTH = 32;

		/// <summary>
		/// アンローダーマガジン数
		/// </summary>
		private const int UNLOADER_MAGAZINE_COUNT = 4;

		private const int UNLOADER_DATA_LENGTH = 128;

		private const int DATE_TIME_ADDRESS_LENGTH = 6;

		private const int MAGAZINE_NO_LENGTH = 7;

		private const int WORK_START_ADDRESS_OFFSET = 0;

		private const int WORK_COMPLETE_ADDRESS_OFFSET = 6;

		private const int MAGAZINE_NO_OFFSET = 16;

		#endregion

        protected override void concreteThreadWork()
        {
			try
			{
				if (this.IsRequireOutput() == true)
				{
					if (this.IsAutoLine)
					{
						if (isOpeningCheckMagazineOutput())
						{
							//始業点検完了
							openingCheckComplete();
						}
						else
						{
							//作業完了
							workComplete();
						}
					}
					else
					{
						workCompletehigh();
					}
				}

                if (this.IsWorkStartAutoComplete)
                {
                    if (Plc.GetBit(LoaderQRReadCompleteBitAddress) == PLC.Common.BIT_ON)
                    {
                        workStart();
                    }
                }
                
                CheckMachineLogFile();

                //Nasca不良ファイル取り込み
                //処理を停止しないエラーについては別タスクでポップアップする。
                string errMessage = String.Empty;
                Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, out errMessage);

                if (string.IsNullOrWhiteSpace(errMessage) == false)
                {
                    Task.Factory.StartNew(() =>
                        System.Windows.Forms.MessageBox.Show(errMessage)
                    );
                }

                //仮想マガジン消去要求応答
                ResponseClearMagazineRequest();

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

        public void workStart()
        {
            VirtualMag mag = new VirtualMag();

            string magno = Plc.GetMagazineNo(LMagazineAddress);
            if (string.IsNullOrEmpty(magno) == false)
            {
                mag.MagazineNo = magno;
                mag.LastMagazineNo = magno;
            }
            else
            {
                throw new ApplicationException("[開始登録異常] 検査機搬入マガジンNOの取得に失敗。\n検査機搬入位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
            }
            Magazine svrmag = Magazine.GetCurrent(magno);
            if (svrmag == null) throw new ApplicationException("[開始登録異常] マガジン情報が見つかりません" + magno);

            OutputSysLog(string.Format("[開始処理] 開始 LoaderMagazineNo:{0}", magno));

            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);

            mag.ProcNo = nextproc.ProcNo;
            mag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);

            Order order = CommonApi.GetWorkStartOrder(mag, this.MacNo);

            ArmsApiResponse workResponse = CommonApi.WorkStart(order);
            if (workResponse.IsError)
            {
                Plc.SetBit(this.WorkStartNGBitAddress, 1, PLC.Common.BIT_ON);
                Log.ApiLog.Info(string.Format("[開始登録異常] 装置:{0} 理由:{1}", this.MacNo, workResponse.Message));
            }
            else
            {
                Plc.SetBit(this.WorkStartOKBitAddress, 1, PLC.Common.BIT_ON);
                OutputSysLog(string.Format("[開始処理] 完了 LoaderMagazineNo:{0}", mag.MagazineNo));
            }
        }

        /// <summary>
        /// 作業完了(自動化)
        /// </summary>
        private void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);
            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
                return;
            }

            string oldmagno = Plc.GetMagazineNo(LMagazineAddress);
            if (string.IsNullOrEmpty(oldmagno) == false)
            {
                newMagazine.LastMagazineNo = oldmagno;
            }
            else
            {
                Log.RBLog.Info("検査機搬入側マガジンNOの取得に失敗");
                return;
            }

            //作業IDを取得
            newMagazine.ProcNo = Order.GetLastProcNo(this.MacNo, oldmagno);

            try
            {
                //作業開始完了時間取得
                newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
            }
            catch
            {
                newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
                this.Enqueue(newMagazine, Station.Unloader);
                AsmLot.InsertLotLog("", System.DateTime.Now, "検査機排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください", newMagazine.ProcNo.Value, newMagazine.MagazineNo, true, this.LineNo);
                return;
            }

			//EICSが作成したNasca不良ファイルを登録
			//bool result = RunDefect(newMagazine.LastMagazineNo, this.PlantCd, newMagazine.ProcNo.Value);
			//if (result == false) 
			//{
			//	return;
			//}

            //現在完了工程で削除フラグONかつ規制理由「」が存在する場合、毎回規制を復活させる
            Magazine magazine = Magazine.GetCurrent(newMagazine.LastMagazineNo);
            if (magazine == null) throw new ApplicationException(string.Format("検査機マガジンデータ異常　現在稼働中マガジンがありません:{0}", magazine));
            Restrict[] reslist = Restrict.SearchRestrict(magazine.NascaLotNO, newMagazine.ProcNo, false);
            foreach (Restrict res in reslist)
            {
                //周辺強度による規制理由と同じか確認
                if (res.Reason == WireBonder.MMFile.RESTRICT_REASON_2)
                {
                    //規制を有効化
                    res.DelFg = false;
                    res.Save();
                }
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
		private void workCompletehigh()
		{
            if (this.IsWorkEndAutoComplete)
            {
                VirtualMag ulMag = new VirtualMag();

                string newmagno = Plc.GetMagazineNo(ULMagazineAddress, true);
                if (string.IsNullOrEmpty(newmagno) == false)
                {
                    ulMag.MagazineNo = newmagno;
                    ulMag.LastMagazineNo = newmagno;
                }
                else
                {
                    throw new ApplicationException("[完了登録異常] 検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
                }
                Magazine svrmag = Magazine.GetCurrent(newmagno);
                if (svrmag == null) throw new ApplicationException("[完了登録異常] マガジン情報が見つかりません" + newmagno);

                OutputSysLog(string.Format("[完了処理] 開始 UnLoaderMagazineNo:{0}", newmagno));

                //作業IDを取得
                int procno = Order.GetLastProcNo(this.MacNo, ulMag.MagazineNo);
                ulMag.ProcNo = procno;

                // 直近100ロットの完成リストに存在する場合は何もしない
                if (preCompleteLotQueue.Contains(string.Format("{0},{1}", ulMag.MagazineNo, procno)) == true)
                {
                    return;
                }

                try
                {
                    //作業開始完了時間取得
                    ulMag.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
                    if (ulMag.WorkComplete.HasValue == false)
                    {
                        Log.ApiLog.Info("検査機排出位置のマガジンに完了時間の装置記憶がありません。\nNASCAデータを確認してください" + ulMag.MagazineNo);
                        return;
                    }

                    if (IsWorkStartAutoComplete)
                    {
                        Order startOrder = Order.GetMachineOrder(this.MacNo, svrmag.NascaLotNO);
                        if (startOrder == null)
                        {
                            throw new ApplicationException(string.Format("作業の開始実績が存在しません。手動で開始登録を行った後、装置監視を再開して下さい。LotNo:{0}", svrmag.NascaLotNO));
                        }
                        ulMag.WorkStart = startOrder.WorkStartDt;
                    }
                    else
                    {
                        ulMag.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
                        if (ulMag.WorkStart.HasValue == false)
                        {
                            Log.ApiLog.Info("検査機排出位置のマガジンに開始時間の装置記憶がありません。\nNASCAデータを確認してください" + ulMag.MagazineNo);
                            return;
                        }
                    }
                }
                catch
                {
                    Log.ApiLog.Info("検査機排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください" + ulMag.MagazineNo);
                    return;
                }

                Order order = CommonApi.GetWorkEndOrder(ulMag, this.MacNo, this.LineNo);
                ArmsApiResponse workResponse = CommonApi.WorkEnd(order);
                if (workResponse.IsError)
                {
                    Plc.SetBit(this.WorkCompleteNGBitAddress, 1, PLC.Common.BIT_ON);
                    OutputSysLog(string.Format("[完了登録異常] 理由:{1}", workResponse.Message));
                    return;
                }

                Plc.SetBit(this.OutputReserveBitAddress, 1, PLC.Common.BIT_ON);
                OutputSysLog(string.Format("[完了処理] 完了 UnLoaderMagazineNo:{0}", newmagno));

                //直近100マガジンの完成記憶
                preCompleteLotQueue.Enqueue(string.Format("{0},{1}", ulMag.MagazineNo, procno));
                if (preCompleteLotQueue.Count >= 100)
                {
                    preCompleteLotQueue.Dequeue();
                }
            }
            else
            {
                //アンローダー各位置のマガジン有無情報を全て取得
                string[] magazineExists = Plc.GetBitArray(this.UnloaderReqBitAddressStart, UNLOADER_MAGAZINE_COUNT);

                //アンローダーの全マガジンのデータを全て取得
                string[] raw = Plc.GetBitArray(this.UnloaderMagazineDataAddressStart, UNLOADER_DATA_LENGTH);

                if (magazineExists == PLC.Omron.BIT_READ_TIMEOUT_VALUE_ARRAY || raw == PLC.Omron.BIT_READ_TIMEOUT_VALUE_ARRAY)
                {
                    return;
                }

                for (int i = 0; i < magazineExists.Length; i++)
                {
                    //実体がある位置のデータを小分けにしてマガジン情報に展開
                    if (magazineExists[i] == PLC.Omron.BIT_ON)
                    {
                        VirtualMag mag = parseMagazine(raw, i * MAGAZINE_DATA_LENGTH);
                        if (mag == null)
                        {
                            continue;
                        }

                        //作業IDを取得
                        int procno = Order.GetLastProcNo(this.MacNo, mag.MagazineNo);

                        //直近100ロットの完成リストに存在する場合は何もしない
                        if (preCompleteLotQueue.Contains(string.Format("{0},{1}", mag.MagazineNo, procno)) == true)
                        {
                            continue;
                        }

                        //現在完了工程で削除フラグONかつ規制理由「」が存在する場合、毎回規制を復活させる
                        Magazine magazine = Magazine.GetCurrent(mag.MagazineNo);
                        if (magazine == null) throw new ApplicationException(string.Format("検査機マガジンデータ異常　現在稼働中マガジンがありません:{0}", magazine));
                        Restrict[] reslist = Restrict.SearchRestrict(magazine.NascaLotNO, procno, false);
                        foreach (Restrict res in reslist)
                        {
                            //周辺強度による規制理由と同じか確認
                            if (res.Reason == WireBonder.MMFile.RESTRICT_REASON_2)
                            {
                                //規制を有効化
                                res.DelFg = false;
                                res.Save();
                            }
                        }

                        //Enqueueが先にないとLoc情報が無い
                        this.Enqueue(mag, Station.Unloader);

                        //高効率でArmsWebが作成してしまうので削除
                        this.Dequeue(Station.Loader);

                        //直近100マガジンの完成記憶
                        preCompleteLotQueue.Enqueue(string.Format("{0},{1}", mag.MagazineNo, procno));
                        if (preCompleteLotQueue.Count >= 100)
                        {
                            preCompleteLotQueue.Dequeue();
                        }
                    }
                }
            }
		}

		/// <summary>
		/// アンローダー指定位置のマガジン情報を取得
		/// </summary>
		/// <param name="raw"></param>
		/// <param name="startOffset"></param>
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
				throw new ApplicationException("排出マガジン情報パースエラー offset:" + startOffset, ex);
			}

			VirtualMag mag = new VirtualMag();

			//分割なしマガジン番号を返す
			mag.MagazineNo = Plc.GetMagazineNo(magazineno, true);
			mag.LastMagazineNo = mag.MagazineNo;
			mag.WorkStart = Plc.GetWordsAsDateTime(workstart);
			mag.WorkComplete = Plc.GetWordsAsDateTime(workcomplete);

			if (string.IsNullOrEmpty(mag.MagazineNo) == true)
			{
				throw new ApplicationException("マガジン番号取得失敗");
			}

			if (mag.WorkStart.HasValue == false || mag.WorkComplete.HasValue == false)
			{
				throw new ApplicationException("作業時間取得失敗");
			}

			Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
			if (svrmag == null)
			{
				throw new ApplicationException("マガジン情報が見つかりません" + mag.MagazineNo);
			}

			AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
			Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
			mag.ProcNo = nextproc.ProcNo;

			return mag;
		}

        /// <summary>
        /// 始業点検完了
        /// </summary>
        private void openingCheckComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();
            
            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);
            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
                return;
            }

            List<IMachine> conveyors = LineKeeper.Machines.Where(m => m is OpeningCheckConveyor).ToList();
            if (conveyors.Exists(c => ((OpeningCheckConveyor)c).ParentMacNo == this.MacNo))
            {
                //次装置には検査機MacNoと一致する親装置MacNoが設定されている始業点検CVを設定する
                IMachine conveyor = conveyors.Find(c => ((OpeningCheckConveyor)c).ParentMacNo == this.MacNo);
                newMagazine.NextMachines.Add(conveyor.MacNo);
            }
            else 
            {
                //排出CV
                Log.RBLog.Info(string.Format("搬送する始業点検CVが見つかりませんでした。元装置NO:{0}", this.MacNo));
                newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
            }

            base.Enqueue(newMagazine, Station.Unloader);
        }

        /// <summary>
        /// 始業点検マガジン排出確認
        /// </summary>
        /// <returns></returns>
        private bool isOpeningCheckMagazineOutput()
        {
            if (string.IsNullOrEmpty(OpeningCheckMagazineOutputWordAddress) == true)
            {
                return false;
            }

            decimal retv = Plc.GetWordAsDecimalData(OpeningCheckMagazineOutputWordAddress);
            if (retv == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override bool CanInput(VirtualMag mag)
        {
            bool retv = base.CanInput(mag);
            if (retv == false) return retv;

            IMachine machine = LineKeeper.GetMachine(mag.MacNo);
            if (machine is OpeningCheckConveyor)
            {
                //始業点検コンベア⇒検査機への搬送の場合、PLCに始業点検マガジンである事を教える
                Plc.SetWordAsDecimalData(OpeningCheckMagazineInputWordAddress, 1);
            }
            else
            {
                //始業点検コンベア⇒検査機への搬送の場合、PLCに始業点検マガジンである事を教える
                Plc.SetWordAsDecimalData(OpeningCheckMagazineInputWordAddress, 0);
            }

            return true;
        }

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //アンローダー以外は仮想マガジンを作成しない
            if (station != Station.Unloader && station != Station.EmptyMagazineUnloader)
            {
                return true;
            }
			
            return base.Enqueue(mag, station);
        }

        public override bool IsDischargeMode(VirtualMag mag)
        {
            if (isOpeningCheckMagazineOutput())
            {
                //始業点検マガジン排出が出ている場合、問答無用で抜き取りモードはOFF
                return false;
            }

            return base.IsDischargeMode(mag);
        }

		#region ResponseEmptyMagazineRequest
		/// <summary>
		/// 空マガジンの配置処理
		/// </summary>
		/// <param name="m"></param>
		public override bool ResponseEmptyMagazineRequest()
		{
			//供給禁止状態なら処理しない
			if (this.IsInputForbidden() == true)
			{
				return false;
			}

			if (this.IsRequireEmptyMagazine() == true)
			{
				bool IsDeleteFromMag = false;
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
                    //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                    VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
					if (VirtualMag.IsECKMag(mag.MagazineNo)) return false;
                    if (mag.IsAOIMag()) return false;

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
                            if (bridgeMags.Any(m => Magazine.GetCurrent(m.MagazineNo) == null &&
                                                    VirtualMag.IsECKMag(m.MagazineNo) == false &&
                                                    m.IsAOIMag() == false))
                            {
                                return false;
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
		#endregion

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

        public override void CheckMachineLogFile()
        {
            List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
            foreach (string logFile in logFiles)
            {
                if (MachineLog.IsLotFromFileName(logFile)) continue;

                if (IsStartLogFile(logFile) == false)
                {
                    MachineLog mLog = parseMachineLog(logFile);

                    if (mLog.IsUnknownData)
                    {
                        MachineLog.ChangeFileName(logFile, MachineLog.FILE_UNKNOWN, MachineLog.FILE_UNKNOWN, 0, MachineLog.FILE_UNKNOWN);
                        return;
                    }
                    else if (mLog.IsOpeningCheckFile)
                    {
                        MachineLog.ChangeFileName(logFile, MachineLog.FILE_OPENINGCHECK, MachineLog.FILE_OPENINGCHECK, 0, MachineLog.FILE_OPENINGCHECK);
                        return;
                    }

                    Magazine svrMag = Magazine.GetCurrent(mLog.MagazineNo);
                    if (svrMag == null)
                        throw new ApplicationException(
                            string.Format("装置ログ内のマガジンNo:{0}の稼働中マガジンが存在しません。", mLog.MagazineNo));

                    AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                    int procNo = Process.GetNowProcess(lot).ProcNo;

                    MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo, logFile);
                }
            }
        }

        private MachineLog parseMachineLog(string filepath, int retryCt)
        {
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                throw new ApplicationException("ファイル解析中にエラーが発生しました：" + filepath);
            }

            MachineLog mLog = new MachineLog();
            string magNo = null;

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    int openingCheckFgCol = 0;

                    if (isMMFile(filepath))
                    {
                        openingCheckFgCol = MM_OPENINGCHECKFG_COL;
                    }
                    else if (isSMFile(filepath))
                    {
                        openingCheckFgCol = SM_OPENINGCHECKFG_COL;
                    }
                    else
                    {
                        throw new ApplicationException("SM・MM・OAファイル以外のファイルを検知しました。ファイルの確認を行って下さい：" + filepath);
                    }
                    
                    int lineno = 0;
                    int openingRowCt = 0;

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

                        if (data[openingCheckFgCol].Trim() == OPENINGCHECKFG_ON)
                        {
                            openingRowCt++;
                        }

                        lineno = lineno + 1;
                    }

                    if (lineno == 0)
                    {
                        // データがヘッダのみを想定
                        mLog.IsUnknownData = true;
                        return mLog;
                    }

                    //対象ファイルが始業点検用データかチェック。始業点検と通常のデータが混載している場合は処理不可。
                    if (openingRowCt >= 1)
                    {
                        if (openingRowCt != lineno)
                        {
                            throw new ApplicationException("始業点検データと通常データが混在しています。ファイルの確認を行って下さい。：" + filepath);
                        }
                        else
                        {
                            mLog.IsOpeningCheckFile = true;
                        }
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
        private MachineLog parseMachineLog(string filepath)
        {
            return parseMachineLog(filepath, 0);
        }
                private bool isMMFile(string filepath)
        {
            return Regex.IsMatch(Path.GetFileName(filepath), "^log.*_MM.*$");
        }

        private bool isSMFile(string filepath)
        {
            return Regex.IsMatch(Path.GetFileName(filepath), "^log.*_SM.*$");
        }
    }
}
