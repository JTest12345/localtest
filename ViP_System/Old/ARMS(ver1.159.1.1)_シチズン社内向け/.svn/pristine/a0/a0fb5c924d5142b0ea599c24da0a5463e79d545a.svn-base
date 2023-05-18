using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// ダイサー(SIGMA用)
    /// </summary>    
    class Dicing2 : MachineBase
    {
        protected override void concreteThreadWork()
        {
            try
            {
                //フルオート完了(finファイルのリネイム、仮想マガジンをUnLoaderに移す)
                magazineWorkComplete();

                //ワーク加工完了(fin2ファイルのリネイム)
                base.SubstrateWorkComplete();
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
        /// マガジン完了時
        /// </summary>
        private void magazineWorkComplete()
        {
            //TnVirtualMagのloader側の仮想マガジンを一つUnLoaderに移す
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }
            VirtualMag oldmag = this.Peek(Station.Loader);
            if (oldmag == null)
            {
                return;
            }

            List<MachineLog.FinishedFile5> finList = MachineLog.FinishedFile5.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }
            if (finList.Count >= 2)
            {
                throw new ApplicationException("finファイルが複数存在するため処理できません。");
            }

            foreach (MachineLog.FinishedFile5 fin in finList)
            {
                if (fin.WorkStartDt == null || fin.WorkEndDt == null)
                {
                    throw new ApplicationException(string.Format("finファイル内から開始/完了時間が取得できませんでした。ファイル名：{0}", fin.FullName));
                }

                VirtualMag newMagazine = this.Peek(Station.Loader);
                newMagazine.WorkStart = fin.WorkStartDt;
                newMagazine.WorkComplete = fin.WorkEndDt;

                OutputSysLog(string.Format("[完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[完了処理] 工程取得成功 ProcNo:{0}", procno));

                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.DataMatrix);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.DataMatrix, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName, fileName));

                //仮想マガジンをunloaderに移動
                this.Enqueue(newMagazine, Station.Unloader);

                OutputSysLog(string.Format("[完了処理] 完了"));
            }
        }
    }
}
