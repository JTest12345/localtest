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
    /// マウンタ(SIGMA用)
    /// </summary>
    public class Mounter : CifsMachineBase
    {
        private const int TRG_COL_COUNT = 6;
        private const int CSV_COL_COUNT = 30;
        private const int MAT_COL = 1;
        private const int MAT_LOT_COL = 13;

        protected override void concreteThreadWork()
        {
            try
			{
                //運転開始
                operationStart();

                //作業開始
                WorkStart();

                //作業完了
                workComplete();
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
        /// 運転開始
        /// </summary>
        private void operationStart()
        {
            List<MachineLog.TriggerFile3> trgList = MachineLog.TriggerFile3.GetAllFiles(this.LogInputDirectoryPath);
            if(trgList.Count == 0)
            {
                return;
            }

            Profile profile = Profile.GetCurrentDBProfile(this.MacNo);
            if (profile == null)
            {
                throw new ApplicationException(string.Format("SMTローダーに割り付けられているプロファイルを取得できませんでした。装置:{0}", this.MacNo));
            }

            BOM[] bom = Profile.GetBOM(profile.ProfileId);

            //使用されている全部材を取得、SMTローダーに割り付けられているプロファイルと照合
            foreach (MachineLog.TriggerFile3 trg in trgList)
            {
                OutputSysLog(string.Format("[運転開始処理] 開始 mtrファイル取得成功 FileName:{0}", trg.FullName));

                string errmsg = "";
                bool? retv = checkBom(trg, bom, out errmsg);

                if (retv == null || !retv.Value)
                {
                    SendNgFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(trg.FullName), errmsg);
                    OutputSysLog(string.Format("ngファイル出力完了。 トリガファイル:{0}", trg.FullName));
                    throw new ApplicationException(errmsg);
                }
                else
                {
                    SendOkFile(this.LogInputDirectoryPath, Path.GetFileNameWithoutExtension(trg.FullName));
                    OutputSysLog(string.Format("okファイル出力完了。 トリガファイル:{0}", trg.FullName));
                }

                //年月日フォルダに移動
                string backupDir = Path.Combine(this.LogInputDirectoryPath
                , DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                string destPath = Path.Combine(backupDir, Path.GetFileName(trg.FullName));
                if (File.Exists(destPath) == true)
                {
                    File.Delete(destPath);
                }
                File.Move(trg.FullName, destPath);

                OutputSysLog(string.Format("[運転開始処理] 完了"));
            }
        }

        /// <summary>
        /// 照合
        /// </summary>
        /// <param name="trg"></param>
        /// <param name="bom"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        private bool? checkBom(MachineLog.TriggerFile3 trg, BOM[] bom, out string errmsg)
        {
            Material[] matlist = getMaterials(trg, out errmsg);
            if (matlist == null)
            {
                return null;
            }

            string errMsg = string.Empty;
            if (WorkChecker.IsBomError(matlist, bom, out errMsg, null, "", this.MacNo))
            {
                errmsg = string.Format(
                    "BOM照合エラー。 ファイル:{0} 部材品目:{1}",
                    trg.FullName, string.Join(",", matlist.Select(m => m.MaterialCd)));

                return false;
            }

            return true;
        }

        /// <summary>
        /// フィーダ部品段取り情報ファイルから部材情報を取得
        /// </summary>
        /// <param name="contents"></param>
        /// <param name="errMsg"></param>
        /// <returns></returns>
        private Material[] getMaterials(MachineLog.TriggerFile3 trg, out string errMsg)
        {
            List<Material> retv = new List<Material>();

            foreach (string content in trg.FileData)
            {
                string[] fileData = content.Split(',');

                if (fileData.Length != TRG_COL_COUNT)
                {
                    errMsg = string.Format("フィーダ部品段取り情報ファイルに不備があります。 ファイル:{0}", trg.FullName);
                    return null;
                }

                //0 : 問合せ日時 (＝ファイル名の問合せ日時)
                //1 : 部品 ID
                //2 : ロット ID
                //3 : リール ID
                //4 : フィーダ ID
                //5 : フィーダセット位置

                Material m = new Material();
                m.MaterialCd = fileData[MAT_COL];
                retv.Add(m);
            }

            errMsg = "";
            return retv.ToArray();
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            List<MachineLog.FinishedFile4> finList = MachineLog.FinishedFile4.GetAllFiles(this.LogOutputDirectoryPath, this.CsvInputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile4 fin in finList)
            {
                OutputSysLog(string.Format("[完了処理] 開始 Finファイル取得成功 FileName:{0}", fin.FullName_Fin));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[完了処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[完了処理] 工程取得成功 ProcNo:{0}", procno));

                //csvファイルから使用資材を取得し、TnMatRelationに登録
                Material[] frameList = getFrameList(fin.FullName_Csv);

                //実績登録
                EntryOrder(lot, procno, fin.MagNo, fin.WorkStartDt, fin.WorkEndDt, frameList);

                //CSVファイルを年月日フォルダに移動
                string backupDir = Path.Combine(this.CsvInputDirectoryPath
                , DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
                if (!Directory.Exists(backupDir))
                {
                    Directory.CreateDirectory(backupDir);
                }
                string destPath = Path.Combine(backupDir, Path.GetFileName(fin.FullName_Csv));
                if (File.Exists(destPath) == true)
                {
                    File.Delete(destPath);
                }
                File.Move(fin.FullName_Csv, destPath);
                
                //finファイル名をリネーム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                MachineLog.ChangeFileNameCarrier(fin.FullName_Fin, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                OutputSysLog(string.Format("[完了処理] ファイル名変更 from:{0} to:{1}", fin.FullName_Fin, fileName));

                //リフローの実績自動登録
                //マウンタの最終時刻＝開始時刻
                Process nextproc = ArmsApi.Model.Process.GetNextProcess(procno, lot);
                EntryOrder(lot, nextproc.ProcNo, fin.MagNo, DateTime.Now, null, null);
            }
        }

        /// <summary>
        /// csvファイルから使用資材を取得
        /// </summary>
        /// <param name="csvFile"></param>
        /// <returns></returns>
        private Material[] getFrameList(string csvFile)
        {
            List<Material> newlist = new List<Material>();

            string[] contents = File.ReadAllLines(csvFile);

            // ファイルの中身を1行ずつ確認
            foreach (string s in contents)
            {
                string[] fileData = s.Split(',');
                if (fileData.Length <= CSV_COL_COUNT || fileData[MAT_LOT_COL].Trim() == MachineLog.FinishedFile4.CSV_MATLOT)
                {
                    continue;
                }

                string matlotno = fileData[MAT_LOT_COL].Trim();
                Material mat = Material.GetMaterial(null, matlotno);
                newlist.Add(mat);
            }

            return newlist.ToArray();
        }
    }
}
