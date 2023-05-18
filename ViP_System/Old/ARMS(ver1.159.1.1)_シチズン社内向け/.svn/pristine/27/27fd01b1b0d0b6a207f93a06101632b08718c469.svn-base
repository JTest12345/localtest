using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// SMT検査機(SIGMA用)
    /// </summary>
    public class Inspector4 : CifsMachineBase
    {
        protected override void concreteThreadWork()
        {
            try
            {
                //作業開始
                workStart();

                //作業完了
                workComplete();

                //EICSが作成したNasca不良ファイルを登録
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

        /// <summary>
        /// 作業開始
        /// </summary>
        private void workStart()
        {
            List<MachineLog.TriggerFile> trgList = MachineLog.TriggerFile.GetAllFiles(this.LogInputDirectoryPath);
            if (trgList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.TriggerFile trg in trgList)
            {
                OutputSysLog(string.Format("[開始処理] 開始 trgファイル取得成功 FileName:{0}", trg.FullName));

                AsmLot lot = AsmLot.GetAsmLot(trg.NascaLotNo);
                OutputSysLog(string.Format("[開始処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[開始処理] 工程取得成功 ProcNo:{0}", procno));

                //trgファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(trg.FullName, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo, trg.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", trg.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, trg.MagNo);
                OutputSysLog(string.Format("[開始処理] ファイル名変更 from:{0} to:{1}", trg.FullName, fileName));

                OutputSysLog(string.Format("[開始処理] 完了"));

                //はんだ印刷の実績自動登録
                //はんだ印刷後検査の最初の開始時刻＝完了時刻
                //リフローの実績自動登録
                //リフロー後検査の開始時刻＝完了時刻
                Process prevproc = ArmsApi.Model.Process.GetPrevProcess(procno, lot.TypeCd);
                EntryOrder(lot, prevproc.ProcNo, trg.MagNo, null, DateTime.Now, null);
            }
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            List<MachineLog.FinishedFile2> finList = MachineLog.FinishedFile2.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile2 fin in finList)
            {
                OutputSysLog(string.Format("[完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName_Fin));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));
          
                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[完了処理] 工程取得成功 ProcNo:{0}", procno));

                //実績登録
                EntryOrder(lot, procno, fin.MagNo, fin.WorkStartDt, fin.WorkEndDt, null);

                //fin及びwedファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName_Wed, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[開始処理] ファイル名変更 from:{0} to:{1}", fin.FullName_Wed, fileName));
                MachineLog.ChangeFileNameCarrier(fin.FullName_Fin, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                OutputSysLog(string.Format("[開始処理] ファイル名変更 from:{0} to:{1}", fin.FullName_Fin, fileName));

                OutputSysLog(string.Format("[完了処理] 完了"));
            }
        }
    }
}
