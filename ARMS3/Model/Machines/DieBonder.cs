using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi.Model;
using ArmsApi;
using System.IO;
using System.Text.RegularExpressions;
namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ダイボンダー AD830
    /// </summary>
    public class DieBonder : MachineBase
    {
        /// <summary>
        /// ウェハーチェンジャー交換フラグアドレス
        /// </summary>
        public string WaferChangerChangeSensorBitAddress { get; set; }

        /// <summary>
        /// ウェハー段数取得の最初のアドレス
        /// </summary>
        public string WaferBitAddressStart { get; set; }

        /// <summary>
        /// ウェハー段数BITの長さ
        /// </summary>
        private const int WAFER_BIT_LENGTH = 5;

        /// <summary>
        /// 2nd供給要求信号アドレス
        /// </summary>
        public string SecondLoaderReqBitAddress { get; set; }

        /// <summary>
        /// 2nd空マガジン供給要求信号BITアドレス
        /// </summary>
        public string SecondEmpMagLoaderReqBitAddress { get; set; }

        /// <summary>
        /// 作業テーブルセンサー信号アドレス
        /// </summary>
        public string StartWorkTableSensorAddress { get; set; }

        /// <summary>
        /// マガジン取出しフラグ(高効率用)
        /// </summary>
        public string MagazineTakeoutBitAddress { get; set; }

        /// <summary>
        /// アンローダー動作フラグ(高効率用)
        /// </summary>
        public string UnloaderMoveCompleteBitAddress { get; set; }

        public bool IsWaitingMagazineTakeout { get; set; }

		public const string FINISHEDFILE_IDENTITYNAME = "^L";

        public DieBonder() {}

        /// <summary>
        /// メインルーチン
        /// </summary>
        protected override void concreteThreadWork() 
        {
            try
            {
                //ウェハーチェンジャーの交換監視
                if (Plc.GetBit(WaferChangerChangeSensorBitAddress) == Mitsubishi.BIT_ON)
                {
                    WaferChangerChange();
                }

                //作業完了
                if (this.IsRequireOutput() == true)
                {
                    if (this.IsAutoLine)
                    {
                        this.workComplete();
                    }
                    else
                    {
                        //前マガジンの排出を待っている場合は次の完了処理を行わない
                        if (IsWaitingMagazineTakeout == false)
                        {
							WorkCompleteHigh();
                        }

                        if (IsWaitingMagazineTakeout)
                        {
                            //完了位置からマガジンが取り除かれたかのチェック
                            if (this.Plc.GetBit(MagazineTakeoutBitAddress, 1) == Mitsubishi.BIT_ON)
                            {
                                //アンローダーが動作したかのチェック
                                if (this.Plc.GetBit(this.UnloaderMoveCompleteBitAddress, 1) == Mitsubishi.BIT_ON)
                                {
                                    IsWaitingMagazineTakeout = false;
                                    this.Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);
                                    this.Plc.SetBit(UnloaderMoveCompleteBitAddress, 1, Mitsubishi.BIT_OFF);
                                }
                            }
                        }
                    }
                }

                //作業開始
                CheckWorkStart();

				//Nasca不良ファイル取り込み
				Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd);

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

        /// <summary>
        /// ウェハーチェンジャー交換
        /// </summary>
        public virtual void WaferChangerChange()
        {
            VirtualMag lMagazine = this.Peek(Station.Loader);
            if (lMagazine == null)
            {
                //PLCのBITを0に
                Plc.SetBit(WaferChangerChangeSensorBitAddress, 1, Mitsubishi.BIT_OFF);
                //マガジンが無い状態のチェンジャー交換は無視する
                return;
            }

            if (lMagazine.WaferChangerChangeCount.HasValue == false)
            {
                lMagazine.WaferChangerChangeCount = 0;
            }


            //最終のチェンジャー交換から60秒以内の交換信号は無視する
            //センサーの揺らぎ対策
            if (lMagazine.WaferChangerChangeTime.HasValue == true)
            {
                if ((DateTime.Now - lMagazine.WaferChangerChangeTime.Value).TotalSeconds <= 60)
                {
                    //PLCのBITを0に
                    Plc.SetBit(WaferChangerChangeSensorBitAddress, 1, Mitsubishi.BIT_OFF);
                    return;
                }
            }
            
            //インクリメント
            lMagazine.WaferChangerChangeCount += 1;
            lMagazine.WaferChangerChangeTime = DateTime.Now;

            //仮想マガジン更新
            lMagazine.Updatequeue();

            //PLCのBITを0に
            Plc.SetBit(WaferChangerChangeSensorBitAddress, 1, Mitsubishi.BIT_OFF);

            Log.RBLog.Info(string.Format("ウェハーカセットの交換処理:{0}", this.MacNo));
        }
  
        /// <summary>
        /// 自動搬送 作業完了
        /// </summary>
        public virtual void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                //完了処理が終了しているので何もしない
                return;
            }

            VirtualMag newmag = this.Peek(Station.EmptyMagazineLoader);
            VirtualMag oldmag = this.Peek(Station.Loader);

            if (newmag == null)
            {
                return;
            }

            if (oldmag == null)
            {
                return;
            }

			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", oldmag.MagazineNo));

			newmag.LastMagazineNo = oldmag.MagazineNo;
            newmag.ProcNo = oldmag.ProcNo;
            newmag.WorkStart = oldmag.WorkStart;
            newmag.WorkComplete = DateTime.Now;
            newmag.StartWafer = oldmag.StartWafer;
            newmag.WaferChangerChangeCount = oldmag.WaferChangerChangeCount;

            oldmag.InitializeWorkData();
            oldmag.ProcNo = null;

            //終了時ウェハー段数を取得
            newmag.EndWafer = this.GetWaferPlateNo();
            if (newmag.StartWafer == null)
            {
                throw new Exception("開始ウェハー段数が取得できていません。");
            }
            Log.RBLog.Info(string.Format("終了ウェハー段数を設定:{0}", newmag.EndWafer.ToString()));

			Magazine svrMag = Magazine.GetCurrent(oldmag.MagazineNo);
			AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			if (this.LogOutputDirectoryPath != null)
			{
				string lFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
				if (string.IsNullOrEmpty(lFile))
					throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。LotNo:{0}", lot.NascaLotNo));

				List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, newmag.WorkStart.Value, File.GetLastWriteTime(lFile));
				if (lotFiles.Count == 0)
					throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", newmag.WorkStart.Value, newmag.WorkComplete.Value));

				foreach (string lotFile in lotFiles)
				{
					if (IsStartLogFile(lotFile))
						continue;

                    //EICSでリネーム前のファイルを移動させようとしてエラーになる現象がMAPでちょこちょこ発生しているので
                    //その対策としてLファイルを一番最後にリネームするよう改修する 
                    //EICSではLファイルをリネームしていないと全ファイル移動しないような仕組みになっているので
                    //Lファイルを最後にリネームすることで問題が解決する見込み 2019.01.21
                    if (lotFile.Trim() == lFile.Trim())
                        continue;

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, newmag.ProcNo.Value, newmag.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}

                //Lファイルを一番最後にリネーム 2019.01.21
                MachineLog.ChangeFileName(lFile, lot.NascaLotNo, lot.TypeCd, newmag.ProcNo.Value, newmag.MagazineNo);
                OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lFile));
            }

            if (this.Enqueue(newmag, Station.Unloader))
			{
				this.Dequeue(Station.EmptyMagazineLoader);
				this.Dequeue(Station.Loader);

				base.WorkComplete(newmag, this, true);

				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", newmag.MagazineNo));
			}
        }

        /// <summary>
		/// 高生産性 作業完了
		/// </summary>
        public virtual void WorkCompleteHigh()
        {
            VirtualMag oldmag = this.Peek(Station.Loader);
            if (oldmag == null)
            {
                return;
            }
			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", oldmag.MagazineNo));

            //前マガジンの取り除きフラグが残っている場合は削除
            this.Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);

            oldmag.LastMagazineNo = oldmag.MagazineNo;
            oldmag.WorkComplete = DateTime.Now;

            //終了時ウェハー段数を取得
            oldmag.EndWafer = this.GetWaferPlateNo();
            if (oldmag.StartWafer == null)
            {
                throw new Exception("開始ウェハー段数が取得できていません。");
            }
            Log.ApiLog.Info("[完了処理] 終了ウェハー段数を設定:" + oldmag.EndWafer.ToString());

			Magazine svrMag = Magazine.GetCurrent(oldmag.MagazineNo);
			AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

			if (this.LogOutputDirectoryPath != null)
			{
				string lFile = MachineLog.GetEarliestFishishedFile(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
				if (string.IsNullOrEmpty(lFile))
					throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。LotNo:{0}", lot.NascaLotNo));

				List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(this.LogOutputDirectoryPath, oldmag.WorkStart.Value, File.GetLastWriteTime(lFile));
				if (lotFiles.Count == 0)
					throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", oldmag.WorkStart.Value, oldmag.WorkComplete.Value));

				foreach (string lotFile in lotFiles)
				{
					if (IsStartLogFile(lotFile))
						continue;

                    //EICSでリネーム前のファイルを移動させようとしてエラーになる現象がMAPでちょこちょこ発生しているので
                    //その対策としてLファイルを一番最後にリネームするよう改修する 
                    //EICSではLファイルをリネームしていないと全ファイル移動しないような仕組みになっているので
                    //Lファイルを最後にリネームすることで問題が解決する見込み 2019.01.21
                    if (lotFile.Trim() == lFile.Trim())
                        continue;

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, oldmag.ProcNo.Value, oldmag.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}

                //Lファイルを一番最後にリネーム 2019.01.21
                MachineLog.ChangeFileName(lFile, lot.NascaLotNo, lot.TypeCd, oldmag.ProcNo.Value, oldmag.MagazineNo);
                OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lFile));
            }

            if (this.Enqueue(oldmag, Station.Unloader))
			{
				this.Dequeue(Station.Loader);

				IsWaitingMagazineTakeout = true;

				OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", oldmag.MagazineNo));
			}
        }

        /// <summary>
        /// 作業開始 ローダーの先頭マガジンに作業開始時間とウェハー段数を記録
        /// </summary>
        public virtual void CheckWorkStart()
        {
            VirtualMag lMagazine = this.Peek(Station.Loader);
            if (lMagazine == null)
            {
                return;
            }

            if (lMagazine.StartWafer.HasValue == true && lMagazine.StartWafer != 0)
            {
                //すでに開始段数が有る場合は何もしない
                return;
            }

            //開始時間を記録
            VirtualMag ulMag = VirtualMag.GetLastTailMagazine(this.MacNo, Station.Unloader);
            if (ulMag == null)
            {
                lMagazine.WorkStart = DateTime.Now;
                OutputSysLog(string.Format("[開始時間記録] Unloader側に仮想マガジン無しの為、現在時刻:{0}をLoader側マガジンに記録", lMagazine.WorkStart));
            }
            else
            {
                if (lMagazine.WorkStart.HasValue == false || lMagazine.WorkStart.Value <= ulMag.WorkComplete)
                {
                    //1つ前のマガジンで出力されたLファイルの更新日時を完了日時として使用した場合、
                    //それを次マガジンの開始時間として使う事で正確な開始時間が得られる
                    lMagazine.WorkStart = ulMag.WorkComplete;
                    OutputSysLog(string.Format("[開始時間記録] Unloader側の最後尾仮想マガジン完了時刻:{0}をLoader側マガジンに記録", lMagazine.WorkStart));
                }
            }

            //開始時のウェハー段数を記録
            lMagazine.StartWafer = this.GetWaferPlateNo();

            //ウェハーチェンジャーの交換回数を0回に設定
            lMagazine.WaferChangerChangeCount = 0;

            //実仮想マガジン更新
            lMagazine.Updatequeue();

            //TnTranの開始時間も更新する(ローダー側にあるマガジン全て更新)
            Magazine svrMag = Magazine.GetCurrent(lMagazine.MagazineNo);
            Order startOrder = Order.GetMachineOrder(this.MacNo, svrMag.NascaLotNO);
            if (startOrder == null)
            {
                throw new ApplicationException(string.Format("作業の開始実績が存在しませんでしたので、ローダーの先頭マガジンに作業開始時間を更新できませんでした。LotNo:{0}", svrMag.NascaLotNO));
            }

            DateTime workStart = lMagazine.WorkStart.Value;

            List<Order> macOrders = Order.GetMachineOrderStart(this.MacNo, startOrder.WorkStartDt, workStart).Where(r => r.WorkEndDt.HasValue == false).OrderBy(r => r.WorkStartDt).ToList();

            foreach (Order macOrder in macOrders)
            {
                OutputSysLog($"[開始時間記録] Loader側マガジンの開始時刻を更新(TnTran) 更新対象ロット：{macOrder.LotNo} 更新対象工程：{macOrder.ProcNo} 更新前の開始時間：{macOrder.WorkStartDt} 更新後の開始時間：{workStart}");

                macOrder.WorkStartDt = workStart;
                macOrder.DeleteInsert(macOrder.LotNo);

                workStart = workStart.AddSeconds(1);
            }
        }

        #region GetWaferPlateNo

        /// <summary>
        /// ウェハー使用段数を取得
        /// </summary>
        /// <returns></returns>
        public int GetWaferPlateNo()
        {
            int retv = 0;
            string s = Plc.GetBit(WaferBitAddressStart, WAFER_BIT_LENGTH);
            int counter = 0;
            foreach (char c in s)
            {
                int i;

                //各段数を2のn乗として足し合わせ
                if (int.TryParse(c.ToString(), out i) == true)
                {
                    retv += (i * (int)Math.Pow(2, counter));
                    counter++;
                }
                else
                {
                    throw new ApplicationException("ウェハー段数取得時にINTパースエラー発生");
                }
            }

            return retv;
        }
        #endregion

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            if (station == Station.Loader)
            {

                //開始時間を記録
                mag.WorkStart = DateTime.Now;

                //ウェハーチェンジャーの交換回数を0回に設定
                mag.WaferChangerChangeCount = 0;

                ////TODO
                //if (Properties.Settings.Default.UseOvenProfiler)
                //{
                //    //DBオーブンプロファイル予約を設定
                //    LineKeeper.SetDBOvenProfileReserve(this.MacNo, OvenProfiler.GetDBReserveProfileNo(mag));
                      
                      //こっち？
                //    CarrierInfo carrier = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                //    CarrierBase car = (CarrierBase)LineKeeper.GetCarrier(carrier.CarNo);
                //    car.LastDBOvenProfileReserve = OvenProfiler.GetDBReserveProfileNo(mag);
                //}
            }

            Location loc = new Location(this.MacNo, station);
            mag.Enqueue(mag, loc);

            return true;
        }

        /// <summary>
        /// マガジン供給 可否確認
        /// </summary>
        /// <returns>結果</returns>
        public override bool IsRequireInput()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.LoaderReqBitAddress) == true)
            {
                return false;
            }
            //string retv = this.Plc.GetBit(this.LoaderReqBitAddress);
            string retv;
            try
            {
                retv = this.Plc.GetBit(this.LoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.LoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            string retv2 = "0";
            if (!string.IsNullOrEmpty(this.SecondLoaderReqBitAddress))
            {
                try
                {
                    retv2 = this.Plc.GetBit(this.SecondLoaderReqBitAddress);
                }
                catch (Exception ex)
                {
                    OutputSysLog($"PLC通信エラーの為、2nd供給要求OFF扱い。アドレス：『{this.SecondLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                }
            }

            if (retv == Mitsubishi.BIT_ON && retv2 == Mitsubishi.BIT_OFF)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 空マガジン供給 可否確認
        /// </summary>
        /// <returns>結果</returns>
        public override bool IsRequireEmptyMagazine()
        {
            if (string.IsNullOrEmpty(this.EmpMagLoaderReqBitAddress) == true)
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.SecondEmpMagLoaderReqBitAddress) == true)
            {
                return false;
            }

            //string retv = this.Plc.GetBit(this.EmpMagLoaderReqBitAddress);
            //string retv2 = this.Plc.GetBit(this.SecondEmpMagLoaderReqBitAddress);
            string retv;
            string retv2;
            try
            {
                retv = this.Plc.GetBit(this.EmpMagLoaderReqBitAddress);
                retv2 = this.Plc.GetBit(this.SecondEmpMagLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、空供給要求OFF扱い。アドレス：『{this.EmpMagLoaderReqBitAddress}』『{this.SecondEmpMagLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Mitsubishi.BIT_ON && retv2 == Mitsubishi.BIT_OFF)
            {
                return true;
            }

            return false;
        }

		public override bool CanInput(VirtualMag mag)
		{
			// 2015.4.27 車載自動化(2015.GW限定)のDBProfileのロット数に応じてマガジンを供給するモード
			if (Config.Settings.IsDBProfileRequireMode)
			{
				Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
				if (svrmag != null)
				{
					// フレーム搭載機からの移動の場合のみ
					if (svrmag.NowCompProcess == 0)
					{
						AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);

						Profile profile = Profile.GetCurrentDBProfile(this.MacNo);
						if (profile == null)
						{
							return false;
						}
						int profileOrderCt = Profile.GetOrderCountFromProfileName(profile.ProfileNm);

						// 2015.6.1 フレーム搭載機の資材を振り替え予定の装置ProfileでBOMチェック
						Order frameLoadOrder = Order.GetPrevMagazineOrder(lot.NascaLotNo, mag.ProcNo.Value);
						Process frameLoadProc = Process.GetProcess(frameLoadOrder.ProcNo);
						BOM[] bom = Profile.GetBOM(profile.ProfileId);

						string errMsg = string.Empty;
						if (WorkChecker.IsBomError(frameLoadOrder.GetMaterials(), bom, out errMsg, lot, frameLoadProc.WorkCd, frameLoadOrder.MacNo))
						{
							Log.SysLog.Info(string.Format(
								"フレーム搭載後からダイボンダー{0}への搬送はBOM照合エラーの為、対象から除外。 投入ロット:{1} フレーム品目:{2}",
								this.Name, lot.NascaLotNo, string.Join(",", frameLoadOrder.GetMaterials().Select(m => m.MaterialCd))));

							return false;
						}
						//-----------------------------------------------------------------------

						AsmLot[] lots = AsmLot.SearchAsmLot(profile.ProfileId);
						lots = lots.Where(l => l.NascaLotNo != lot.NascaLotNo).ToArray();

						if (lots.Count() < profileOrderCt)
						{
							if (profile.ProfileId != lot.ProfileId)
							{
								Log.ApiLog.Info(
									string.Format("ロット情報を投入装置のProfile情報で更新 ロット番号:{0} 元プロファイルID:{1} 先プロファイルID:{2}", lot.NascaLotNo, lot.ProfileId, profile.ProfileId));

								lot.ProfileId = profile.ProfileId;
								lot.TypeCd = profile.TypeCd;
								lot.BlendCd = profile.BlendCd;
								lot.ResinGpCd = profile.ResinGpCd;
                                lot.ResinGpCd2 = profile.ResinGpCd2;
                                lot.CutBlendCd = profile.CutBlendCd;
								lot.DBThrowDT = profile.DBThrowDt;
                                lot.DieShareSamplingPriority = profile.DieShearSamplingPriority;    //2016.02.24 ダイシェア試験抜取グループ 追加
                                lot.BeforeLifeTestCondCd = profile.BeforeLifeTestCondCd;    //2018.04.02 先行ライフ試験識別 追加
                                lot.IsFullSizeInspection = profile.FullInspectionFg;        //2018.11.27 WB後自動外観検査機投入が全数識別 追加
                                foreach (int proc in profile.InspectionProcs)               //2018.11.27 抜き取り検査対象ロット識別 追加
                                {
                                    Inspection isp = Inspection.GetInspection(lot.NascaLotNo, proc);
                                    if (isp == null)
                                    {
                                        isp = new Inspection();
                                        isp.LotNo = lot.NascaLotNo;
                                        isp.ProcNo = proc;
                                        isp.DeleteInsert();
                                        //抜取り検査フラグON
                                        lot.IsChangePointLot = true;
                                    }
                                }
                                lot.Update();
							}

							return true;
						}
						else
						{
							Log.RBLog.Info(
								string.Format("発行ロット上限超過の為除外 装置:{0} ロット数:{1} プロファイル上限値:{2}", this,Name, lots.Count(), profileOrderCt));

							return false;
						}
					}
				}
			}
			return true;
		}

		/// <summary>
		/// 先頭仮想マガジンの開始時間後の名称変更前Lファイルの出力があればTrue
		/// </summary>
		/// <returns></returns>
        public virtual bool IsFishishedFileOutput()
		{
			if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true))
			{
				VirtualMag lMag = this.Peek(Station.Loader);
				if (lMag == null)
				{
					throw new ApplicationException(string.Format(
						"ローダー側の仮想マガジンが無い状態で傾向管理ファイルが存在します。 除去作業後、監視を再開して下さい。対象フォルダ:{1}", this.LogOutputDirectoryPath));
				}

				if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true, lMag.WorkStart.Value))
				{
					return true;
				}
				else
				{
					throw new ApplicationException(string.Format(
						"ローダー側の先頭マガジンより古い傾向管理ファイルが存在します。 除去作業後、監視を再開して下さい。先頭マガジン開始時間:{0} 対象フォルダ:{1}", lMag.WorkStart.Value, this.LogOutputDirectoryPath));
				}
			}
			else
			{
				return false;
			}
		}

		public bool IsStartLogFile(string fullName)
		{
			string fileName = Path.GetFileNameWithoutExtension(fullName);
			if (Regex.IsMatch(fileName, "^O.*$") || Regex.IsMatch(fileName, "^_O.*$")
				|| Regex.IsMatch(fileName, "^P.*$") || Regex.IsMatch(fileName, "^_P.*$"))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        public bool IsHLogFile(string fullName)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullName);
            if (Regex.IsMatch(fileName, "^H.*$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool IsILogFile(string fullName)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullName);
            if (Regex.IsMatch(fileName, "^I.*$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetHLogFile(List<string> fileList)
        {
            foreach (string file in fileList)
            {
                if (IsHLogFile(file))
                {
                    return file;
                }
            }

            return string.Empty;
        }

        public string GetILogFile(List<string> fileList)
        {
            foreach (string file in fileList)
            {
                if (IsILogFile(file))
                {
                    return file;
                }
            }

            return string.Empty;
        }
    }
}
