using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// マスキング装置（白樹脂塗布） SIGMA用
    /// </summary>
    public class Masking : CifsMachineBase
    {
        protected override void concreteThreadWork()
        {
            try
            {
                //基板作業開始
                base.WorkStart();

                //基板作業完了
                workComplete();

                //マガジン作業完了
                base.MagazineWorkComplete();

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
        /// 基板作業完了
        /// </summary>
        private void workComplete()
        {
            List<MachineLog.FinishedFile6> finList = MachineLog.FinishedFile6.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile6 fin in finList)
            {
                OutputSysLog(string.Format("[完了処理] 開始 Fin2ファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[完了処理] 工程取得成功 ProcNo:{0}", procno));

                //fin2ファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                OutputSysLog(string.Format("[完了処理] 完了"));
            }
        }
    }
}
