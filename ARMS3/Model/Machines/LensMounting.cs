using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// レンズ実装(SIGMA用)
    /// </summary>
    class LensMounting : CifsMachineBase
    {
        ///// <summary>
        ///// TODO 2016.04.07 キャビ番号の文字位置
        ///// </summary>
        //private const int CAVITY_START_INDEX = -1;

        ///// <summary>
        ///// TODO 2016.04.07 キャビ番号の文字数
        ///// </summary>
        //private const int CAVITY_LENGTH = 2;

        protected override void concreteThreadWork() 
        {
            try
            {
                //資材トレイ照合
                //workStart();

                //マガジン作業完了
                base.MagazineWorkComplete();

                //trgファイル名をリネーム
                base.WorkStart();

                //fin2ファイル名をリネーム
                base.SubstrateWorkComplete();

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
        /// 作業開始(資材トレイ照合)
        /// </summary>
        //private void workStart()
        //{
        //    List<MachineLog.TriggerFile4> trgList = MachineLog.TriggerFile4.GetAllFiles(this.LogInputDirectoryPath);
        //    if (trgList.Count == 0)
        //    {
        //        return;
        //    }

        //    foreach (MachineLog.TriggerFile4 trg in trgList)
        //    {
        //        OutputSysLog(string.Format("[資材トレイ開始処理] 開始 トリガファイル取得成功 FileName:{0}", trg.FullName));

        //        AsmLot lot = AsmLot.GetAsmLot(trg.NascaLotNo);
        //        OutputSysLog(string.Format("[資材トレイ開始処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

        //        //キャビ番号取得
        //        string cavityno = lot.NascaLotNo.Substring(CAVITY_START_INDEX, CAVITY_LENGTH);
        //        OutputSysLog(string.Format("[資材トレイ開始処理] キャビ番号成功 キャビ番号:{0}", cavityno));

        //        //未完了のロット取得
        //        List<Material> preMatlist = MachineInfo.GetLotList(this.MacNo);
        //        OutputSysLog(string.Format("[資材トレイ開始処理] 未完了のロット取得成功 LotNo:{0}", string.Join(",", preMatlist.Select(r => r.LotNo))));

        //        //キャビ番号照合
        //        bool isMatch = true;
        //        foreach (Material premat in preMatlist)
        //        {
        //            string precavityno = premat.LotNo.Substring(CAVITY_START_INDEX, CAVITY_LENGTH);
        //            if (cavityno != precavityno)
        //            {
        //                isMatch = false;
        //                break;
        //            }
        //        }

        //        if (!isMatch)
        //        {
        //            //ngファイル送信
        //            string errmsg = "キャビの混載";
        //            SendNgFile(this.LogInputDirectoryPath, trg.TrayDataMatrix, errmsg);
        //            OutputSysLog(string.Format("[資材トレイ開始処理] キャビの混載。 ngファイル出力完了。 トリガファイル:{0}", trg.FullName));
        //            throw new ApplicationException(errmsg);
        //        }
        //        else
        //        {
        //            //レンズはロットデータで資材が一意に定まるためこの仕様となっています。
        //            Material[] matlist = Material.GetMaterials(null, null, lot.NascaLotNo, false);
                                        
        //            //MacMatに登録
        //            MachineInfo mac = MachineInfo.GetMachine(this.MacNo);
        //            if (mac != null)
        //            {
        //                //未完了のロットを完了に
        //                foreach (Material premat in preMatlist)
        //                {
        //                    premat.RemoveDt = DateTime.Now;
        //                    mac.DeleteInsertMacMat(premat);
        //                }

        //                //今回のロットを登録
        //                foreach (Material mat in matlist)
        //                {
        //                    mat.StockerNo = 0;
        //                    mat.IsTimeSampled = false;
        //                    mac.DeleteInsertMacMat(mat);
        //                }
        //            }

        //            //okファイル送信
        //            SendOkFile(this.LogInputDirectoryPath, trg.TrayDataMatrix);
        //            OutputSysLog(string.Format("okファイル出力完了。 トリガファイル:{0}", trg.FullName));
        //        }

        //        OutputSysLog(string.Format("[資材トレイ開始処理] 完了"));
        //    }
        //}
    }
}
