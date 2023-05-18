using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using ARMS3.Model.PLC;
using System.Text.RegularExpressions;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ワイヤーボンダー
    /// </summary>
    public class WireBonder : MachineBase
    {
		///// <summary>
		///// MMファイルを待ってその内容で完了登録するモード
		///// </summary>
		//public bool IsMMFileMode
		//{
		//	get
		//	{
		//		if (!string.IsNullOrEmpty(MMFilePath))
		//		{
		//			return true;
		//		}
		//		else
		//		{
		//			return false;
		//		}
		//	}
		//}

		///// <summary>
		///// MMファイル保存パス
		///// </summary>
		//public string MMFilePath { get; set; }

        /// <summary>
        /// 対応する抜き取り検査工程（MMファイル処理用）
        /// </summary>
        public int InspectProcNo { get; set; }

        /// <summary>
        /// 2nd供給要求信号アドレス
        /// </summary>
        public string SecondLoaderReqBitAddress { get; set; }

        /// <summary>
        /// 2nd空マガジン供給要求信号BITアドレス
        /// </summary>
        public string SecondEmpMagLoaderReqBitAddress { get; set; }

        /// <summary>
        /// マガジン取出しフラグ(高効率用)
        /// </summary>
        public string MagazineTakeoutBitAddress { get; set; }

        /// <summary>
        /// アンローダー動作フラグ(高効率用)
        /// </summary>
        public string UnloaderMoveCompleteBitAddress { get; set; }

        public bool IsWaitingMagazineTakeout { get; set; }

        protected override void concreteThreadWork()
        {
            try
            {         
                if (this.IsAutoLine)
                {
                    if (this.IsRequireOutput() == true)
                    {
                        workComplete();
                    }
                }
                else
                {
                    if (this.IsRequireOutput() == true)
                    {
                        //前マガジンの排出を待っている場合は次の完了処理を行わない
                        if (IsWaitingMagazineTakeout == false)
                        {
                            workCompleteHigh();
                        }

                        if (IsWaitingMagazineTakeout)
                        {
                            //完了位置からマガジンが取り除かれたかのチェック
                            if (this.Plc.GetBit(MagazineTakeoutBitAddress, 1) == Mitsubishi.BIT_ON)
                            {
                                //Unloaderが動作したかのチェック
                                if (this.Plc.GetBit(this.UnloaderMoveCompleteBitAddress, 1) == Mitsubishi.BIT_ON)
                                {
                                    IsWaitingMagazineTakeout = false;
                                    this.Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);
                                    this.Plc.SetBit(UnloaderMoveCompleteBitAddress, 1, Mitsubishi.BIT_OFF);
                                }
                            }
                        }
                    }
					if (Config.Settings.IsMappingMode)
                    //if (IsMMFileMode)
                    {
                        //Unloaderに仮想マガジンが移った後
                        mmComplete();
                    }
                    else
                    {
                        //EmptyMagazineLoaderの仮想マガジンを削除
                        this.ClearVirtualMagazines(Station.EmptyMagazineLoader);
                    }
                }

                //作業開始
                checkWorkStart();

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
        /// 作業完了(自動化)
        /// </summary>
        private void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }
            VirtualMag newMagazine = this.Peek(Station.EmptyMagazineLoader);
			if (newMagazine == null)
			{
				return;
			}
			
			VirtualMag oldMagazine = this.Peek(Station.Loader);
            if (oldMagazine == null)
            {
                return;
            }

			OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", oldMagazine.MagazineNo));

			//if (IsMMFileMode)
			if (Config.Settings.IsMappingMode)
			{
				//LENS処理が完了している場合は完了登録処理へ
				Magazine svroldmag = Magazine.GetCurrent(oldMagazine.MagazineNo);
				if (ArmsApi.Model.LENS.WorkResult.IsComplete(svroldmag.NascaLotNO, oldMagazine.ProcNo.Value))
				{
					newMagazine.LastMagazineNo = oldMagazine.MagazineNo;
					newMagazine.ProcNo = oldMagazine.ProcNo;
					newMagazine.WorkStart = oldMagazine.WorkStart;
					newMagazine.WorkComplete = DateTime.Now;

					oldMagazine.InitializeWorkData();
					oldMagazine.ProcNo = null;

					this.Enqueue(newMagazine, Station.Unloader);
					this.Enqueue(oldMagazine, Station.EmptyMagazineUnloader);

					this.Dequeue(Station.EmptyMagazineLoader);
					this.Dequeue(Station.Loader);

					this.WorkComplete(newMagazine, this, true);

					return;
				}

				//MMファイル情報取得
				MMFile mm = MMFile.GetMMFileInfo(this.LogOutputDirectoryPath, oldMagazine.MagazineNo, this.MacNo);
				if (mm == null)
				{
					Log.RBLog.Info(string.Format("MMファイル到着待ち mag:{0}", newMagazine.MagazineNo));
					return;
				}

				if (mm.MagNo != newMagazine.MagazineNo)
				{
					throw new ApplicationException("MMファイル内のマガジンNoと仮想マガジンで不一致発生");
				}

				if (MMFile.IsLotFromFileName(mm.FileFullPath) == false)
				{
					//検査機流動フラグを立てる。
					if (mm.IsContains1)
					{
						mm.SetInspectFg(InspectProcNo);
					}

					//検査後に排出するための排出フラグ（削除フラグON状態）をセットする。
					if (mm.IsContains2)
					{
						mm.SetRistrictToAfterInspectionProcess(InspectProcNo);
					}

					//排出フラグと理由「WBエラーに対する検査の為」を設定する。
					if (mm.IsContains3)
					{
						mm.SetRistrict(oldMagazine.ProcNo.Value);
						//Log.RBLog.Info("WBエラーの為の検査フラグ設定" + mm.MagNo);
					}

					//MMファイル内のスキップ数を不良数として登録
					if (mm.SkipCt >= 1)
					{
						if (!Defect.InsertWireMMFileSkipError(oldMagazine.MagazineNo, mm.SkipCt, oldMagazine.ProcNo.Value))
						{
							mm.SetRistrictToSkipError(oldMagazine.ProcNo.Value, mm.SkipCt);
						}
					}

					//MMファイル内のバッドマーク数を不良数として登録
					if (mm.BadmarkCt >= 1) 
					{
						if (!Defect.InsertWireMMFileBadmarkError(oldMagazine.MagazineNo, mm.BadmarkCt, oldMagazine.ProcNo.Value))
						{
							//mm.SetRistrictToSkipError(oldMagazine.ProcNo.Value, mm.SkipCt);
						}
					}

					//mm.ChangeCompleteFileName();

					Magazine svrmag = Magazine.GetCurrent(oldMagazine.MagazineNo);
					if (svrmag == null) throw new ApplicationException("稼働中マガジン情報が見つかりません:" + oldMagazine.MagazineNo);
					AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
					if (svrlot == null) throw new ApplicationException("ロット情報が見つかりません:" + svrmag.NascaLotNO);

					OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", svrlot.NascaLotNo));

					if (this.LogOutputDirectoryPath != null)
					{
						List<string> lotFiles = GetLotFiles(this.LogOutputDirectoryPath, oldMagazine.WorkStart.Value, DateTime.Now);
						foreach (string lotFile in lotFiles)
						{
							MachineLog.ChangeFileName(lotFile, svrlot.NascaLotNo, svrlot.TypeCd, oldMagazine.ProcNo.Value, mm.MagNo);
							OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
						}
					}
				}

				return;
			}

            newMagazine.LastMagazineNo = oldMagazine.MagazineNo;
            newMagazine.ProcNo = oldMagazine.ProcNo;
            newMagazine.WorkStart = oldMagazine.WorkStart;
            newMagazine.WorkComplete = DateTime.Now;

            oldMagazine.InitializeWorkData();
            oldMagazine.ProcNo = null;

            this.Enqueue(newMagazine, Station.Unloader);
            this.Enqueue(oldMagazine, Station.EmptyMagazineUnloader);

            this.Dequeue(Station.EmptyMagazineLoader);
            this.Dequeue(Station.Loader);

            this.WorkComplete(newMagazine, this, true);

			OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", newMagazine.MagazineNo));
        }

        /// <summary>
        /// 作業完了(高効率)
        /// </summary>
        private void workCompleteHigh()
        {
            VirtualMag unloadermag = this.Peek(Station.Unloader);
            if (unloadermag != null)
            {
                return;
            }

            VirtualMag oldmag = this.Peek(Station.Loader);
            if (oldmag == null)
            {
                return;
            }

            //if (IsMMFileMode)
			if (Config.Settings.IsMappingMode)
            {
                //MMファイル情報取得
				MMFile mm = MMFile.GetMMFileInfo(this.LogOutputDirectoryPath, oldmag.MagazineNo, this.MacNo);
                if (mm == null)
                {
                    Log.ApiLog.Info("MMファイル到着待ち mag:" + oldmag.MagazineNo);
                    return;
                }

                //検査機流動フラグを立てる。
                if (mm.IsContains1)
                {
                    mm.SetInspectFg(InspectProcNo);
                }
            }

            //前マガジンの取り除きフラグが残っている場合は削除
            this.Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);

            oldmag.LastMagazineNo = oldmag.MagazineNo;
            oldmag.WorkComplete = DateTime.Now;

            this.Enqueue(oldmag, Station.Unloader);
            this.Dequeue(Station.Loader);

            IsWaitingMagazineTakeout = true;            
        }

        /// <summary>
        /// MMファイル処理(高効率用)
        /// Unloaderへ仮想マガジンを移した後、EmptyMagazineLoaderに仮想マガジンができれば処理開始
        /// </summary>
        private void mmComplete() 
        {
            VirtualMag emploadermag = this.Peek(Station.EmptyMagazineLoader);
            if (emploadermag == null) 
            {
                return;
            }

            VirtualMag ulmag = this.Peek(Station.Unloader);
            if (ulmag != null) 
            {
                return;
            }
			OutputSysLog(string.Format("[完了処理] 開始 EmptyMagazineLoaderMagazineNo:{0}", emploadermag.MagazineNo));

            //MMファイル情報取得
			MMFile mm = MMFile.GetMMFileInfo(this.LogOutputDirectoryPath, emploadermag.MagazineNo, this.MacNo);
            if (mm == null)
            {
                Log.ApiLog.Info("MMファイル到着待ち mag:" + emploadermag.MagazineNo);
                return;
            }

            if (mm.MagNo != emploadermag.MagazineNo)
            {
                throw new ApplicationException("MMファイル内のマガジンNoと仮想マガジンで不一致発生 macno:" + this.MacNo);
            }

            Magazine svrmag = Magazine.GetCurrent(emploadermag.MagazineNo);
            if (svrmag == null) throw new ApplicationException("ArmsSvr マガジン情報が見つかりません:" + emploadermag.MagazineNo);
            AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            if (svrlot == null) throw new ApplicationException("ArmsSvr ロット情報が見つかりません:" + svrmag.NascaLotNO);
			OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", svrlot.NascaLotNo));

            //検査後に排出するための排出フラグ（削除フラグON状態）をセットする。
            if (mm.IsContains2)
            {
                mm.SetRistrictToAfterInspectionProcess(InspectProcNo);
            }

            //排出フラグと理由「WBエラーに対する検査の為」を設定する。
            if (mm.IsContains3)
            {
                mm.SetRistrict(emploadermag.ProcNo.Value);
                //Log.ApiLog.Info("WBエラーの為の検査フラグ設定" + mm.MagNo);
            }

            //MMファイル内のスキップ数を不良数として登録
            if (mm.SkipCt >= 1)
            {
				if (!Defect.InsertWireMMFileSkipError(emploadermag.MagazineNo, mm.SkipCt, emploadermag.ProcNo.Value)) 
				{
					mm.SetRistrictToSkipError(emploadermag.ProcNo.Value, mm.SkipCt);
				}
            }

			//MMファイル内のバッドマーク数を不良数として登録
			if (mm.BadmarkCt >= 1)
			{
				if (!Defect.InsertWireMMFileBadmarkError(emploadermag.MagazineNo, mm.BadmarkCt, emploadermag.ProcNo.Value))
				{
					//mm.SetRistrictToSkipError(oldMagazine.ProcNo.Value, mm.SkipCt);
				}
			}

			//mm.ChangeCompleteFileName();

			if (this.LogOutputDirectoryPath != null)
			{
				List<string> lotFiles = GetLotFiles(this.LogOutputDirectoryPath, emploadermag.WorkStart.Value, emploadermag.WorkComplete.Value);
				foreach (string lotFile in lotFiles)
				{
					MachineLog.ChangeFileName(lotFile, svrlot.NascaLotNo, svrlot.TypeCd, emploadermag.ProcNo.Value, emploadermag.MagazineNo);
					OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
				}
			}

            this.Dequeue(Station.EmptyMagazineLoader);
			OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", emploadermag.MagazineNo));
        }

        /// <summary>
        /// 作業開始 ローダーの先頭マガジンに作業開始時間を記録
        /// </summary>
        private void checkWorkStart()
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
            lMagazine.WorkStart = DateTime.Now;

            //開始時のウェハー段数を仮に記録
            //この記録は開始時間を二度更新しない為のフラグにだけ用いて、ULの仮想マガジンには転記しない
            lMagazine.StartWafer = 1;

            //実仮想マガジン更新
            lMagazine.Updatequeue();
        }

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
            //string retv = Plc.GetBit(this.LoaderReqBitAddress);
            //string retv2 = Plc.GetBit(this.SecondLoaderReqBitAddress);
            string retv;
            string retv2;
            try
            {
                retv = Plc.GetBit(this.LoaderReqBitAddress);
                retv2 = Plc.GetBit(this.SecondLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、供給要求OFF扱い。アドレス：『{this.LoaderReqBitAddress}』『{this.SecondLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Mitsubishi.BIT_ON && retv2 == Mitsubishi.BIT_OFF)
            {
                return true;
            }
            return false;
        }

        public override bool IsRequireEmptyMagazine()
        {
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (string.IsNullOrEmpty(this.EmpMagLoaderReqBitAddress) == true)
            {
                return false;
            }
            if (string.IsNullOrEmpty(this.SecondEmpMagLoaderReqBitAddress) == true)
            {
                return false;
            }

            //string retv = Plc.GetBit(this.EmpMagLoaderReqBitAddress);
            //string retv2 = Plc.GetBit(this.SecondEmpMagLoaderReqBitAddress);
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

        public override bool ResponseEmptyMagazineRequest()
        {

            //供給禁止状態なら処理しない
            if (this.IsInputForbidden() == true)
            {
                return false;
            }

            if (this.IsRequireEmptyMagazine() == true)
            {
                Location from = null;
                List<LineBridge> bridgeList = LineKeeper.Machines.Where(m => m is LineBridge).Select(m => (LineBridge)m).ToList();
                bool IsDeleteFromMag = false;

                //自装置の空マガジンを使用
                if (this.IsRequireOutputEmptyMagazine() == true)
                {
                    from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    IsDeleteFromMag = true;
                }
                //ライン連結橋の空マガジンを使用
                else if (bridgeList.Count() > 0)
                {
                    foreach (LineBridge bridge in bridgeList)
                    {
                        if (bridge.IsRequireOutputEmptyMagazine() == false) continue;
                        //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                        VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
                        if (VirtualMag.IsECKMag(mag.MagazineNo)) continue;
                        if (mag.IsAOIMag()) continue;

                        if (mag.NextMachines.Any() == true)
                        {
                            // 空マガジンの仮想マガジンの次装置(A)が入力されており、(A)の装置Noと自装置と違う場合、
                            // (A)装置の空Mag要求がONの場合は、そのマガジンは(A)装置専用とする為、自装置は要求しない
                            IMachine nmac = LineKeeper.GetMachine(mag.NextMachines.First());
                            if (nmac != null && nmac.MacNo != this.MacNo && nmac.IsRequireEmptyMagazine() == true)
                            {
                                continue;
                            }
                        }

                        from = bridge.GetUnLoaderLocation();
                        IsDeleteFromMag = true;

                        break;
                    }
                }
                //空マガジン投入CVの空マガジンを使用
                if (from == null)
                {
                    if (base.IsRequireOutputEmptyMagazine())
                    {
                        //自装置の空マガジン待ち(完了処理待ち)
                        return false;
                    }

                    //空マガジン投入CVの状態確認
                    List<int> emptyMagazineLoadConveyorMacNoList = Route.GetEmptyMagazineLoadConveyors(this.MacNo);
                    foreach (int macNo in emptyMagazineLoadConveyorMacNoList)
                    {
                        IMachine empMagLoadConveyor = LineKeeper.GetMachine(macNo);
                        if (empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
                        {
                            CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                            CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
                            if (fromCar.CarNo != toCar.CarNo)
                            {
                                //空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする
                                List<VirtualMag> bridgeMags = new List<VirtualMag>();

                                List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge || m is RobotQRReader).ToList();
                                foreach (IMachine b in bridges)
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

                            // 循環式投入CVは空マガジン排出時に仮想マガジンを作成するので
                            // 橋と同じ扱いにする (仮想マガジンを削除する)
                            if (empMagLoadConveyor is LoadConveyorMagRotation)
                            {
                                IsDeleteFromMag = true;
                            }
                        }
                    }
                    if (from == null)
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

        #region class MMFile

        public class MMFile
        {
			public const string FILE_PREFIX = "MM";

            public class MMData
            {
                public string Address { get; set; }
                public string Unit { get; set; }
                public string Code { get; set; }
            }

            /// <summary>
            /// 通常の排出
            /// </summary>
            public const string RESTRICT_REASON = "WBエラーに対する検査の為";

            /// <summary>
            /// 周辺強度試験のための排出
            /// </summary>
            public const string RESTRICT_REASON_2 = "WBエラーに対する周辺強度実施の為";

			/// <summary>
			/// DB不良数がSKIP数を超過したことによる状況調査の為の排出
			/// </summary>
			public const string RESTRICT_REASON_3 = "DB不良数+部材交換免責数+立ち上げ免責数がSKIP数を超過したことによる状況調査の為";

            //MMファイル内容
            private const int DATA_NO_COL = 0;
            private const int MAG_NO_COL = 3;
            private const int ADDRESS_NO_COL = 4;
            private const int ERROR_CD_COL = 5;
            private const int UNIT_NO_COL = 6;
            private const int TREAT_CD_COL = 7;

            /// <summary>
            /// 判定CD列のOK時出力文字列
            /// </summary>
            private const string OK_ERROR_CD = "0x0000";

			private const int FILENAME_UPDDT_STARTINDEX = 6;
			private const int FILENAME_UPDDT_LENGTH = 11;

			///// <summary>
			///// DONEフォルダパス
			///// </summary>
			//private const string DONE_DIR_NAME = "DONE";

            /// <summary>
            /// MMファイル装置吐き出し位置
            /// </summary>
            public string BaseDir { get; set; }

			///// <summary>
			///// 処理済みファイル移動先
			///// </summary>
			//public string DoneDir { get; set; }

            /// <summary>
            /// 処理対象ファイルフルパス
            /// </summary>
            public string FileFullPath { get; set; }

            /// <summary>
            /// スキップエラー数
            /// </summary>
            public int SkipCt { get; set; }

			/// <summary>
			/// バッドマーク数
			/// </summary>
			public int BadmarkCt { get; set; }

            /// <summary>
            /// MMファイル内マガジン番号　UL側マガジンと一致
            /// </summary>
            public string MagNo { get; set; }

            public bool IsContains1 { get; set; }
            public bool IsContains2 { get; set; }
            public bool IsContains3 { get; set; }
			//public bool IsContains4 { get; set; }

            /// <summary>
            /// LD側マガジン番号
            /// </summary>
            public string OldMagNo { get; set; }
            public string LotNo { get; set; }
            public string TypeNo { get; set; }
			public int ProcNo { get; set; }

            /// <summary>
            /// 複数ユニット（ワイヤー本数2本以上）で同一不良をカウントしないためのリスト
            /// </summary>
            public List<MMData> DataList { get; set; }

			///// <summary>
			///// 処置コード4のアドレスリスト
			///// EICS側で無条件にモールドマッピング必要
			///// </summary>
			//public List<MMData> TreatAddressList { get; set; }

			public static MMFile GetMMFileInfo(string mmFileDir, string oldMagNO, int macno)
            {
                MMFile mm = new MMFile();

                mm.BaseDir = mmFileDir;
                System.IO.DirectoryInfo bdir = new System.IO.DirectoryInfo(mmFileDir);
				//mm.DoneDir = System.IO.Path.Combine(bdir.Parent.FullName, DONE_DIR_NAME);

				string path = MachineLog.GetNewestFile(mmFileDir, MMFile.FILE_PREFIX);
                //ファイルが無い場合はNULLを返す
                if (string.IsNullOrEmpty(path)) return null;

                mm.FileFullPath = path;
                mm.OldMagNo = oldMagNO;

                Magazine svrmag = Magazine.GetCurrent(oldMagNO);
                if (svrmag == null) throw new ApplicationException("MMファイル処理エラー 現在稼働中マガジン情報が存在しません:" + oldMagNO);

                AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                if (lot == null) throw new ApplicationException("MMファイル処理エラー Svrロット情報が存在しません:" + oldMagNO);

                mm.LotNo = lot.NascaLotNo;
                mm.TypeNo = lot.TypeCd;
				mm.ProcNo = Order.GetLastProcNoFromLot(macno, lot.NascaLotNo);

				try
                {
                    mm.parseMMFile(mm.FileFullPath);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException(string.Format("MMファイル解析エラー発生 ファイルパス:{0} エラー内容:{1}", mm.FileFullPath, ex.Message));
                }

                return mm;
            }

			#region ChangeCompleteFileName

			public void ChangeCompleteFileName()
			{
				System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(BaseDir);
				System.IO.FileInfo[] files = dir.GetFiles();

				foreach (System.IO.FileInfo file in files)
				{
					//アンダーバーから始まるファイルは次マガジンの途中ファイルなので無視する
					if (file.Name.StartsWith("_")) continue;

					try
					{
						if (file.FullName == FileFullPath)
						{
							string orgnm = System.IO.Path.GetFileNameWithoutExtension(file.Name);
							//今回判定対象のMMファイルは末尾にロット、タイプ、作業NOを付与
							string newfilenm = orgnm + "_" + LotNo + "_" + TypeNo + "_" + ProcNo + file.Extension;
							string destfile = System.IO.Path.Combine(BaseDir, newfilenm);

							file.MoveTo(destfile);
						}
						else
						{
							//今回判定の対象外とならなかったMMファイルはゴミとしてそのままのファイル名
						}
					}
					catch (Exception ex)
					{
						Log.RBLog.Info(string.Format("MMファイルの名称変更失敗 {0}{1}", file.Name, ex.ToString()));
					}
				}
			}

			///// <summary>
			///// 既存のファイルを全てDoneへ移動
			///// </summary>
			//public void MoveDoneDir()
			//{
			//	if (!System.IO.Directory.Exists(DoneDir)) System.IO.Directory.CreateDirectory(DoneDir);

			//	System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(BaseDir);
			//	System.IO.FileInfo[] files = dir.GetFiles();

			//	foreach (System.IO.FileInfo file in files)
			//	{
			//		//アンダーバーから始まるファイルは次マガジンの途中ファイルなので無視する
			//		if (file.Name.StartsWith("_")) continue;

			//		try
			//		{
			//			if (file.FullName == FileFullPath)
			//			{
			//				string orgnm = System.IO.Path.GetFileNameWithoutExtension(file.Name);
			//				//今回判定対象のMMファイルは末尾にロット、タイプを付与してDONEへ
			//				string newfilenm = orgnm + "_" + LotNo + "_" + TypeNo + file.Extension;
			//				string destfile = System.IO.Path.Combine(DoneDir, newfilenm);

			//				file.MoveTo(destfile);
			//			}
			//			else
			//			{
			//				//今回判定の対象外とならなかったMMファイルはゴミとしてそのままのファイル名でDONEへ
			//				string destfile = System.IO.Path.Combine(DoneDir, file.Name);
			//				file.MoveTo(destfile);
			//			}
			//		}
			//		catch (Exception ex)
			//		{
			//			Log.RBLog.Info(string.Format("MMファイルのDONEフォルダ移動失敗 {0}{1}", file.Name, ex.ToString()));
			//		}
			//	}
			//}

			#endregion

            #region parseMMFile

            /// <summary>
            /// MMファイルの内容を解析
            /// </summary>
            /// <param name="filepath"></param>
            /// <param name="magno"></param>
            /// <param name="isContains1"></param>
            /// <param name="isContains3"></param>
            private void parseMMFile(string filepath)
            {
                IsContains1 = false;
                IsContains2 = false;
                IsContains3 = false;
				//IsContains4 = false;

                SkipCt = 0;
				BadmarkCt = 0;

                MagNo = null;

                //データ初期化
                this.DataList = new List<MMData>();

                //処置番号4のリスト初期化
                //this.TreatAddressList = new List<MMData>();

                GnlMaster[] skipErrorList = GnlMaster.GetWbSkipError();

                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    while (sr.Peek() > 0)
                    {
                        string line = sr.ReadLine();
                        if (string.IsNullOrEmpty(line)) continue;

                        //CSV各要素分解
                        string[] data = line.Split(',');

                        //DATA_NOが数字変換できない行は飛ばす
                        int datano;
                        if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;

                        if (MagNo == null)
                        {
                            string[] magStr = data[MAG_NO_COL].Split(' ');
                            if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
                            {
                                //自動化用の30_を取り除き
                                MagNo = magStr[1];
                            }
                            else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
                            {
                                //高効率用の11_を取り除き
                                MagNo = magStr[1];
                            }
                            else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT)) 
                            {
                                //高効率用の13_を取り除き
                                MagNo = magStr[1];
                            }
                            else
                            {
                                MagNo = data[MAG_NO_COL];
                            }
                        }

                        string unitno = data[UNIT_NO_COL].Trim();
                        string address = data[ADDRESS_NO_COL].Trim();
                        string errcd = data[ERROR_CD_COL].Trim();

                        if (data[TREAT_CD_COL].Trim() == "1") IsContains1 = true;
                        if (data[TREAT_CD_COL].Trim() == "2") IsContains2 = true;
                        if (data[TREAT_CD_COL].Trim() == "3") IsContains3 = true;

                        if (Config.Settings.IsInspectorNgMarking && data[TREAT_CD_COL].Trim() != "0")
                        {
                            // 検査機でNGマーキングするラインの場合、WB不良が発生していれば検査機流動する
                            IsContains1 = true;
                        }

                        if (Defect.IsWireMMFileBadmarkErrorCode(errcd))
						{
							//同一アドレスで必ずユニット番号は若い順に出力される前提
							var exists = DataList.Where(d => d.Address == address && d.Code != OK_ERROR_CD);
							if (exists.Count() == 0)
							{
								BadmarkCt++;
							}
						}

						if (Defect.IsWireMMFileSkipErrorCode(errcd, skipErrorList))
						{
							//同一アドレスで必ずユニット番号は若い順に出力される前提
							var exists = DataList.Where(d => d.Address == address && d.Code != OK_ERROR_CD);
							if (exists.Count() == 0)
							{
								//スキップエラー数加算
								SkipCt++;
							}
						}

                        //処理済みリストに追加
                        this.DataList.Add(new MMData() { Address = address, Code = errcd, Unit = unitno });
                    }

                    if (MagNo == null) throw new ApplicationException("MMファイルにマガジン番号が存在しません:" + filepath);
                }
            }
            #endregion

			//#region getNewestFile

			///// <summary>
			///// MM*.CSVファイルの内、最終更新日が新しい1つを返す
			///// ファイルが全く無い場合はnull
			///// </summary>
			///// <param name="mmFileDir"></param>
			///// <returns></returns>
			//private static string getNewestFile(string mmFileDir)
			//{
			//	System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(mmFileDir);
			//	if (dir.Exists == false) throw new ApplicationException("MMファイル保存ディレクトリが見つかりません:" + mmFileDir);

			//	System.IO.FileInfo[] files = dir.GetFiles();

			//	System.IO.FileInfo top = files.Where(f => f.Name.ToUpper().StartsWith("MM") && f.Extension.ToUpper() == ".CSV").FirstOrDefault();

			//	if (top != null)
			//	{
			//		return top.FullName;
			//	}
			//	else
			//	{
			//		return null;
			//	}
			//}
			//#endregion

            /// <summary>
            /// MMファイル内容で流動規制情報を作成する
            /// </summary>
            /// <param name="procno"></param>
            public void SetRistrict(int procno)
            {
                Restrict r = new Restrict();
                r.LotNo = this.LotNo;
                r.ProcNo = procno;
                r.Reason = RESTRICT_REASON;
                r.DelFg = false;
                r.Save();

				Log.ApiLog.Info(string.Format("[流動規制設定] ロットNO:{0} 工程NO:{1} 規制理由:{2}", LotNo, procno, RESTRICT_REASON));
			}

            /// <summary>
            /// 検査機後に周辺検査のための排出（流動規制）を予約する
            /// 検査機への投入時点では判定されないように、削除フラグONでセットする。
            /// 検査機の完了処理内で復活される
            /// </summary>
            /// <param name="procno"></param>
            public void SetRistrictToAfterInspectionProcess(int procno)
            {
                Restrict r = new Restrict();
                r.LotNo = this.LotNo;
                r.ProcNo = procno;
                r.Reason = RESTRICT_REASON_2;
                //検査機への投入時点では判定されないように、削除フラグONでセットする。
                //検査機の完了処理内で復活される
                r.DelFg = true;
                r.Save();

				Log.ApiLog.Info(string.Format("[流動規制設定] ロットNO:{0} 工程NO:{1} 規制理由:{2}", LotNo, procno, RESTRICT_REASON_2));
			}

			/// <summary>
			/// DB不良数がSKIP数を超過したことによる状況調査の為
			/// </summary>
			/// <param name="procno"></param>
			public void SetRistrictToSkipError(int procno, int skipct)
			{
				//エラー表示用の取得(DB不良数)
				AsmLot lot = AsmLot.GetAsmLot(this.LotNo);
				int dbDefectCt = Defect.GetDefectCountOfPassedProcess(lot, procno);

				int matChangeCt = Defect.GetMaterialChangeCt(lot.NascaLotNo, procno);

				string message = string.Format("[流動規制設定] ロットNO:{0} 工程NO:{1} 規制理由:{2} DB不良:{3} 部材交換+立ち上げ:{4} SKIP:{5}",
					LotNo, procno, RESTRICT_REASON_3, dbDefectCt, matChangeCt, skipct);

				Restrict r = new Restrict();
				r.LotNo = this.LotNo;
				r.ProcNo = procno;
				r.Reason = message;
				r.DelFg = false;
				r.Save();

				Log.ApiLog.Info(message);
			}

            /// <summary>
            /// 抜取り検査フラグ（検査機流動フラグ）をセットする
            /// </summary>
            /// <param name="procno"></param>
            public void SetInspectFg(int inspectProcno)
            {
                Inspection isp = new Inspection();
                isp.LotNo = this.LotNo;
                isp.IsInspected = false;
                isp.ProcNo = inspectProcno;
                isp.DeleteInsert();

                Log.RBLog.Info(string.Format("MMファイル内容から検査抜き取り工程設定:{0}:{1}", LotNo, inspectProcno.ToString()));
            }

			///// <summary>
			///// マッピング検査フラグをセットする
			///// </summary>
			//public void SetMappingInspectionFg()
			//{
			//	AsmLot lot = AsmLot.GetAsmLot(LotNo);
			//	lot.IsMappingInspection = true;
			//	lot.Update();

			//	Log.RBLog.Info(string.Format("MMファイル内容からマッピング検査設定:{0}", LotNo));
			//}

			/// <summary>
			/// ファイル名変更済確認(ロット + タイプ + 工程NO付与)
			/// </summary>
			/// <param name="filePath"></param>
			/// <returns></returns>
			public static bool IsLotFromFileName(string filePath)
			{
				// MMファイル名定義(MM***_Lot_Type_Proc.CSV)を想定して要素数で付与済か判断する 
				string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');
				if (nameChar.Count() < 4)
				{
					return false;
				}
				else { return true; }
			}
        }

        #endregion

		/// <summary>
		/// 1ロットの全ログファイルを取得
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="startDt"></param>
		/// <param name="endDt"></param>
		/// <returns></returns>
		public static List<string> GetLotFiles(string directoryPath, DateTime startDt, DateTime endDt)
		{
			List<string> retv = new List<string>();

			List<string> files = MachineLog.GetFiles(directoryPath);

			//ファイルを更新日時順に並び変えて、処理のトリガファイルを末尾にして返す
			files = files.OrderBy(f => File.GetLastWriteTime(f)).ToList();

			foreach (string file in files)
			{
				if (File.Exists(file) == false)
				{
					if (File.Exists(string.Format("_{0}", file)))
					{
						throw new ApplicationException(
							string.Format("装置パラメータのチェックが終わっていないか、存在しないファイルを参照しようとしてます。 ファイルパス:{0}", file));
					}
				}

				if (Regex.IsMatch(Path.GetFileNameWithoutExtension(file), "^_.*$"))
				{
					continue;
				}

				string fileName = Path.GetFileNameWithoutExtension(file);

				DateTime outputDate; string date = string.Empty;
				date = fileName.Substring(6, 2) + "/" + fileName.Substring(8, 2) + "/" + fileName.Substring(10, 2) + " " +
					fileName.Substring(12, 2) + ":" + fileName.Substring(14, 2) + ":" + fileName.Substring(16, 2);

				if (DateTime.TryParse(date, out outputDate) == false)
				{
					throw new ApplicationException(string.Format("日付型に変換できないファイル名が付いています ファイルパス:{0}", fileName));
				}

				if (outputDate >= startDt && outputDate <= endDt)
				{
					retv.Add(file);
				}
			}

			return retv;
		}
    }
}
