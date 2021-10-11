using ARMS3.Model.PLC;
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
    /// ダイボンダー AD838L(SIGMA用)
    /// </summary>
    public class DieBonder7 : DieBonder2
    {
        protected override void concreteThreadWork()
        {
            try
            {
                //DMファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                this.substrateStart();

                //作業完了
                if (this.IsRequireOutput() == true)
                {
                    this.WorkCompleteHigh();
                }

                //作業開始
                base.checkWorkStart();

                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                this.workComplete();

                //Nasca不良ファイル取り込み
                Defect.ImportNascaDefectFile(this.MacNo, this.PlantCd, true);
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

            //前マガジンの取り除きフラグが残っている場合は削除
            Plc.SetBit(MagazineTakeoutBitAddress, 1, Mitsubishi.BIT_OFF);
            Plc.SetBit(UnloaderMoveCompleteBitAddress, 1, Mitsubishi.BIT_OFF);

            //Log.ApiLog.Info("DB作業完了処理");
            oldmag.LastMagazineNo = oldmag.MagazineNo;
            oldmag.WorkComplete = DateTime.Now;
            oldmag.StartWafer = 0;
            oldmag.EndWafer = 0;

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
        /// 基板完了
        /// </summary>
        private void workComplete()
        {
            List<MachineLog.FinishedFile5> finList = MachineLog.FinishedFile5.GetAllFiles(this.AltMapOutputDirectoryPath);
            List<MachineLog.FinishedFile3> outList = MachineLog.FinishedFile3.GetAllFiles(this.AltMapOutputDirectoryPath);
            if (finList.Count == 0 && outList.Count == 0)
            {
                return;
            }

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

        public override bool IsRequireOutput()
        {
            if (this.Plc.GetBit(MagazineTakeoutBitAddress, 1) == Mitsubishi.BIT_ON &&
                this.Plc.GetBit(this.UnloaderMoveCompleteBitAddress, 1) == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

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
    }
}
