using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ダイボンダー AD838Lのバッファを利用するタイプ(NTSV用)
    /// </summary>
    public class DieBonder6 : DieBonder 
    {
        protected override void concreteThreadWork()
		{
			try
			{
                //ウェハーチェンジャーの交換監視
                if (Plc.GetBit(WaferChangerChangeSensorBitAddress) == PLC.Common.BIT_ON)
                {
                    WaferChangerChange();
                }

                //DMファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                //this.substrateComplete();
                //this.workStart();
                this.substrateStart();

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
                
                CheckWorkStart();

                //Nasca不良ファイル取り込み(2016/9/28 基板毎に変更 fukatani）
                Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);

                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                //this.substrateStart();
                frameWorkComplete();

                //仮想マガジン消去要求応答
                ResponseClearMagazineRequest();
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();

                if (frmErr.Method == ErrorHandleMethod.None)
                {
                    throw;
                }
            }
        }

        public override void WorkCompleteHigh()
        {
            VirtualMag oldmag = Peek(Station.Loader);

            if (oldmag == null)
            {
                return;
            }

            OutputSysLog(string.Format("[完了処理] 開始 LoaderMagazineNo:{0}", oldmag.MagazineNo));

            ////前マガジンの取り除きフラグが残っている場合は削除
            //Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);
            //Plc.SetBit(UnloaderMoveCompleteBitAddress, 1, Mitsubishi.BIT_OFF);

            //Log.ApiLog.Info("DB作業完了処理");
            oldmag.LastMagazineNo = oldmag.MagazineNo;
            oldmag.WorkComplete = DateTime.Now;

            //終了時ウェハー段数を取得
            oldmag.EndWafer = GetWaferPlateNo();
            if (oldmag.StartWafer == null)
            {
                throw new Exception("開始ウェハー段数が取得できていません。");
            }
            OutputApiLog("終了ウェハー段数を設定:" + oldmag.EndWafer.ToString());

            Magazine svrMag = Magazine.GetCurrent(oldmag.MagazineNo);
            AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
            OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

            if (LogOutputDirectoryPath != null)
            {
                string lFile = MachineLog.GetEarliestFishishedFile(LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true);
                if (string.IsNullOrEmpty(lFile))
                    throw new ApplicationException(string.Format("排出信号を検知しましたが、Lファイルが存在しません。LotNo:{0}", lot.NascaLotNo));

                oldmag.WorkComplete = File.GetLastWriteTime(lFile);

                List<string> lotFiles = MachineLog.GetLotFilesFromFileStamp(LogOutputDirectoryPath, oldmag.WorkStart.Value, File.GetLastWriteTime(lFile));
                if (lotFiles.Count == 0)
                    throw new ApplicationException(string.Format("排出信号を検知しましたが、開始～完了の間に出力された傾向管理ファイルが存在しません。 開始:{0} 完了:{1}", oldmag.WorkStart.Value, oldmag.WorkComplete.Value));

                foreach (string lotFile in lotFiles)
                {
                    if (IsStartLogFile(lotFile))
                        continue;

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, oldmag.ProcNo.Value, oldmag.MagazineNo);
                    OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
                }
            }

            if (Enqueue(oldmag, Station.Unloader))
            {
                Dequeue(Station.Loader);

                IsWaitingMagazineTakeout = true;

                OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", oldmag.MagazineNo));
            }
        }
    
        /// <summary>
        /// マガジン完了
        /// </summary>
        public override void workComplete()
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
            OutputSysLog(string.Format("終了ウェハー段数を設定:{0}", newmag.EndWafer.ToString()));

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

                    MachineLog.ChangeFileName(lotFile, lot.NascaLotNo, lot.TypeCd, newmag.ProcNo.Value, newmag.MagazineNo);
                    OutputSysLog(string.Format("[完了処理] ロットファイル名称変更 FileName:{0}", lotFile));
                }
            }

            if (this.Enqueue(newmag, Station.Unloader))
            {
                this.Enqueue(oldmag, Station.EmptyMagazineUnloader);

                this.Dequeue(Station.EmptyMagazineLoader);
                this.Dequeue(Station.Loader);

                base.WorkComplete(newmag, this, true);

                OutputSysLog(string.Format("[完了処理] 完了 UnloaderMagazineNo:{0}", newmag.MagazineNo));
            }
        }

        /// <summary>
        /// 基板完了
        /// </summary>
        private void frameWorkComplete()
        {
            List<MachineLog.FinishedFile5> finList = MachineLog.FinishedFile5.GetAllFiles(this.AltMapOutputDirectoryPath);
            List<MachineLog.FinishedFile3> outList = MachineLog.FinishedFile3.GetAllFiles(this.AltMapOutputDirectoryPath);
            if (finList.Count == 0 && outList.Count == 0)
            {
                return;
            }

            CarrireWorkData stepData = new CarrireWorkData();
            stepData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            foreach (MachineLog.FinishedFile5 fin in finList)
            {
                OutputSysLog(string.Format("[基板完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = Process.GetNowProcess(lot).ProcNo;
                OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.DataMatrix);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.DataMatrix, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                //段数情報を登録
                stepData.LotNo = lot.NascaLotNo;
                stepData.ProcNo = procno;
                stepData.CarrierNo = fin.DataMatrix;
                stepData.Delfg = 0;
                stepData.RegisterMagazineStepWithAutotCalc();

                OutputSysLog(string.Format("[基板完了処理] 完了"));
            }
            foreach (MachineLog.FinishedFile3 outf in outList)
            {
                OutputSysLog(string.Format("[基板完了処理] 開始 outファイル取得成功 FileName:{0}", outf.FullName));

                AsmLot lot = AsmLot.GetAsmLot(outf.NascaLotNo);
                OutputSysLog(string.Format("[基板完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = Process.GetNowProcess(lot).ProcNo;
                OutputSysLog(string.Format("[基板完了処理] 工程取得成功 ProcNo:{0}", procno));

                //outファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(outf.FullName, lot.NascaLotNo, lot.TypeCd, procno, outf.MagNo, outf.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", outf.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, outf.MagNo);
                OutputSysLog(string.Format("[基板完了処理] ファイル名変更 from:{0} to:{1}", outf.FullName, fileName));

                //段数情報を登録
                stepData.LotNo = lot.NascaLotNo;
                stepData.ProcNo = procno;
                stepData.CarrierNo = outf.CarrierNo;
                stepData.Delfg = 0;
                stepData.RegisterMagazineStepWithAutotCalc();

                OutputSysLog(string.Format("[基板完了処理] 完了"));
            }
        }

        /// <summary>
        /// DMファイル名をリネーム
        /// </summary>
        private void substrateStart()
        {
            List<MachineLog.TriggerFile2> trgList = MachineLog.TriggerFile2.GetAllFiles(this.AltMapInputDirectoryPath);
            if (trgList.Count == 0)
            {
                return;
            }

            CarrireWorkData stepData = new CarrireWorkData();
            stepData.Infoid = CarrireWorkData.MAGAZINE_STEP_INFOCD;

            foreach (MachineLog.TriggerFile2 trg in trgList)
            {
                OutputSysLog(string.Format("[開始処理] 開始 DMファイル取得成功 FileName:{0}", trg.FullName));

                AsmLot lot = AsmLot.GetAsmLot(trg.NascaLotNo);
                OutputSysLog(string.Format("[開始処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = Process.GetNowProcess(lot).ProcNo;
                OutputSysLog(string.Format("[開始処理] 工程取得成功 ProcNo:{0}", procno));

                //DMファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(trg.FullName, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo, trg.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", trg.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo);
                OutputSysLog(string.Format("[開始処理] ファイル名変更 from:{0} to:{1}", trg.FullName, fileName));

                OutputSysLog(string.Format("[開始処理] 完了"));
            }
        }

        //public override bool IsRequireOutput()
        //{
        //    //if (IsReady() && IsFishishedFileOutput())
        //    //{
        //    //    return true;
        //    //}
        //    //else
        //    //{
        //    //    return false;
        //    //}
        //    if (this.Plc.GetBit(MagazineTakeoutBitAddress, 1) == Mitsubishi.BIT_ON &&
        //        this.Plc.GetBit(this.UnloaderMoveCompleteBitAddress, 1) == Mitsubishi.BIT_ON)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        /// <summary>
        /// 先頭仮想マガジンの開始時間後の名称変更前Lファイルの出力があればTrue
        /// </summary>
        /// <returns></returns>
        public override bool IsFishishedFileOutput()
        {
            if (MachineLog.IsFishishedOutput(this.LogOutputDirectoryPath, FINISHEDFILE_IDENTITYNAME, true))
            {
                VirtualMag lMag = this.Peek(Station.Loader);
                if (lMag == null)
                {
                    throw new ApplicationException(string.Format(
                        "ローダー側の仮想マガジンが無い状態で傾向管理ファイルが存在します。 除去作業後、監視を再開して下さい。対象フォルダ:{0}", this.LogOutputDirectoryPath));
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

        /// <summary>
        /// 空マガジン排出要求
        /// ※要求だけでマガジンを取りに行くと装置のシンクロOFFの時に
        /// まだアンローダーへデータが移り変わっていない(稼働中)マガジンを取りにいってしまうので未稼働になった時に返り値Trueを返す 
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutputEmptyMagazine()
        {
            if (string.IsNullOrEmpty(this.EmpMagUnLoaderReqBitAddress) == true)
            {
                return false;
            }

            string retv;
            try
            {
                retv = Plc.GetBit(this.EmpMagUnLoaderReqBitAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、空排出要求OFF扱い。アドレス：『{this.EmpMagUnLoaderReqBitAddress}』, エラー内容：{ex.Message}");
                return false;
            }

            if (retv == Mitsubishi.BIT_READ_TIMEOUT_VALUE)
            {
                return false;
            }

            if (retv == Mitsubishi.BIT_ON)
            {
                VirtualMag mag = this.Peek(Station.EmptyMagazineUnloader);
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
                return false;
            }
        }

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
                Location from = null;

                List<LineBridge> bridgeList = LineKeeper.Machines.Where(m => m is LineBridge).Select(m => (LineBridge)m).ToList();

                //自装置の空マガジンを使用
                if (this.IsRequireOutputEmptyMagazine() == true)
                {
                    from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                }
                //ライン連結橋の空マガジンを使用
                else if (bridgeList.Count() > 0)
                {
                    foreach (LineBridge bridge in bridgeList)
                    {
                        if (bridge.IsRequireOutputEmptyMagazine() == false) continue;
                        //先頭が遠心沈降マガジンなら処理しない
                        VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
                        if (VirtualMag.IsECKMag(mag.MagazineNo)) continue;

                        // 【N工場MAP J9・10不具合 修正】
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

                        break;
                    }
                }
                //空マガジン投入CVの空マガジンを使用
                if (from == null)
                {
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
                                if (bridgeMags.Count != 0)
                                {
                                    //if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
                                    if (bridgeMags.Any(m => Magazine.GetCurrent(m.MagazineNo) == null && VirtualMag.IsECKMag(m.MagazineNo) == false))
                                    {
                                        return false;
                                    }
                                }
                            }

                            from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
                        }
                    }
                    if (from == null)
                    {
                        //空マガジン投入CVにマガジンが無い場合
                        return false;
                    }
                }

                Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);

                LineKeeper.MoveFromTo(from, to, true, true, false);

                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
