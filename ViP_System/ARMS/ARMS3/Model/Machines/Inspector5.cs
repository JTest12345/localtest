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
    /// チルト(SIGMA用)
    /// </summary>
    public class Inspector5 : CifsMachineBase
    {
        //private const int TRS_COL_COUNT = 13;
        //private const int BIN_COL = 11;
        //private const int MARKING_COL = 12;
        //private const int MAPDATA_COL = 3;
        //private const string NG_DATA = "1";

        protected override void concreteThreadWork()
        {
            try
            {
                //作業開始
                base.WorkStart();

                //作業完了
                //base.WorkComplete();

                //排出トレイトレースデータ
                traceComplete();
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
        /// 作業完了(排出トレイトレースデータ処理)
        /// </summary>
        public void traceComplete()
        {
            List<MachineLog.FinishedFile8> finList = MachineLog.FinishedFile8.GetAllFiles(this.LogOutputDirectoryPath);
            if (finList.Count == 0)
            {
                return;
            }

            foreach (MachineLog.FinishedFile8 fin in finList)
            {
                OutputSysLog(string.Format("[排出トレイトレースデータ処理] 開始 fin3ファイル取得成功 FileName:{0}", fin.FullName_Fin));

                AsmLot lot = AsmLot.GetAsmLot(fin.NascaLotNo);
                OutputSysLog(string.Format("[排出トレイトレースデータ処理] ロット取得成功 LotNo:{0}", lot.NascaLotNo));

                //工程取得
                int procno = ArmsApi.Model.MachineInfo.GetProcNo(this.MacNo);
                OutputSysLog(string.Format("[排出トレイトレースデータ処理] 工程取得成功 ProcNo:{0}", procno));

                #region LENS2で行うよう変更 2016.06.13
                //Process proc = Process.GetProcess(procno);
                //if (proc == null) throw new ApplicationException("作業情報が見つかりません:" + procno);

                ////trsファイルから排出トレイトレースデータを取得し、エラー内容をTnErrTraceに登録
                //List<ErrTrace> traceList = new List<ErrTrace>();
                //string mappingdata = string.Empty;
                //getTraceabilityData(fin.FullName_Trs, lot.NascaLotNo, procno, lot.TypeCd, proc.WorkCd, ref traceList, ref mappingdata);
                //foreach (ErrTrace trace in traceList)
                //{
                //    //TODO 2016.05.25 この判定でいいのか？
                //    if (trace.InspectionResult == NG_DATA)
                //    {
                //        trace.Insert();
                //    }
                //}

                ////trsファイルを年月日フォルダに移動
                //string backupDir = Path.Combine(this.LogOutputDirectoryPath
                //, DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
                //if (!Directory.Exists(backupDir))
                //{
                //    Directory.CreateDirectory(backupDir);
                //}
                //string destPath = Path.Combine(backupDir, Path.GetFileName(fin.FullName_Trs));
                //if (File.Exists(destPath) == true)
                //{
                //    File.Delete(destPath);
                //}
                //File.Move(fin.FullName_Trs, destPath);

                ////fin3ファイルを消す
                //File.Delete(fin.FullName_Fin);
                #endregion

                //fin3ファイルをリネイム
                //基板DM_ロット番号_タイプ_procno_マガジンNO.拡張子
                MachineLog.ChangeFileNameCarrier(fin.FullName_Fin, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo, fin.CarrierNo);
                string fileName = string.Format("{0}_{1}_{2}_{3}_{4}", fin.CarrierNo, lot.NascaLotNo, lot.TypeCd, procno, fin.MagNo);
                OutputSysLog(string.Format("[排出トレイトレースデータ処理] ファイル名変更 from:{0} to:{1}", fin.FullName_Fin, fileName));

                OutputSysLog(string.Format("[排出トレイトレースデータ処理] 完了"));
            }
        }

        //private void getTraceabilityData(string trsFile, string lotNo, int procno, string typecd, string workcd, ref List<ErrTrace> errlist, ref string mappingdata)
        //{
        //    StringBuilder sb = new StringBuilder();
        //    bool first = true;

        //    string[] contents = File.ReadAllLines(trsFile);

        //    // ファイルの中身を1行ずつ確認
        //    foreach (string s in contents)
        //    {
        //        string[] fileData = s.Split(',');
        //        if (fileData.Length <= TRS_COL_COUNT)
        //        {
        //            continue;
        //        }

        //        //Mappingデータ生成
        //        if (!first) sb.Append(",");
        //        sb.Append(fileData[MAPDATA_COL].Trim());
        //        first = false;

        //        ErrTrace err = new ErrTrace();
        //        err.LotNo = lotNo;
        //        err.ErrProcNo = procno;
        //        err.InspectionResult = fileData[BIN_COL].Trim();

        //        DefItem def = ArmsApi.Model.Defect.GetErrConv(this.PlantCd, typecd, workcd, err.InspectionResult);
        //        string nascaErrNm = string.Empty;
        //        string causeNm = string.Empty;
        //        string classNm = string.Empty;
        //        ArmsApi.Model.Defect.GetDefectNm(typecd, workcd, def, ref nascaErrNm, ref causeNm, ref classNm);
        //        string errDetail = "【不良起因名】" + causeNm + "【不良分類名】" + classNm + "【不良項目名】" + nascaErrNm;

        //        err.ErrDetail = errDetail;
        //        err.SideMarking = fileData[MARKING_COL].Trim();
        //        //err.Contactmiss = fileData[CONTACTMISS_COL].Trim();
        //        errlist.Add(err);
        //    }

        //    mappingdata = sb.ToString();
        //}
    }
}
