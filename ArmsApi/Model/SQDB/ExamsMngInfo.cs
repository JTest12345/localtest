using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.SQDB
{
    public class ExamsMngInfo
    {
        #region プロパティ
        public string Type_CD { get; set; }

        public int Examination_NO { get; set; }

        public int Sampling_NO { get; set; }

        public string ProductFactory_CD { get; set; }

        public string Sect_CD { get; set; }

        public int Sampling_CT { get; set; }

        public string SampleSubmissionMethod_NM { get; set; }

        public int Judge_KB { get; set; }

        public string Reference_NM { get; set; }

        public string Comment_NM { get; set; }

        public string Reason_NM { get; set; }

        public bool Change_FG { get; set; }

        public bool Edition_FG { get; set; }

        public bool Delete_FG { get; set; }

        public bool Delivery_FG { get; set; }

        public string UpdateUser_CD { get; set; }

        public DateTime LastUpdate_DT { get; set; }

        public DateTime Apply_DT { get; set; }

        #endregion


        public static List<string> GetExamsMngInfo_TypeCD(List<ExamsMngInfoSrch_Condition> ExamsMngInfoSrchCondList)
        {
            List<string> retV = new List<string>();

            if (ExamsMngInfoSrchCondList == null)
            {
                return retV;
            }

            using (var db = new ArmsApi.Model.DataContext.SQDBDataContext(Config.Settings.SQDBConSTR))
            {
                List<ArmsApi.Model.DataContext.MttExamsMngInfo> ExamsMngInfoList = db.MttExamsMngInfo.Where(r => r.Delete_FG == false && r.Edition_FG == false).ToList();
                List<string> typeList = new List<string>();

                // リストの各Where条件(試験・抜き取り)を全て満たすタイプを返す
                // nullの設定を見ない
                List<ExamsMngInfoSrch_Condition> eMISCList = ExamsMngInfoSrchCondList.Where(e => e != null).ToList();
                foreach (ExamsMngInfoSrch_Condition eMISC in eMISCList)
                {
                    // 試験Noと抜き取り方法Noを満たすタイプを抽出する
                    List<string> tempList= ExamsMngInfoList
                        .Where(e => e.Examination_NO == eMISC.ExaminationNO && e.Sampling_NO == eMISC.SamplingNO)
                        .Select(t => t.Type_CD).Distinct().ToList();

                    if(eMISCList.FindIndex(e => e == eMISC) == 0)
                    {
                        // 最初のループは、抽出リストを母体にする
                        typeList = tempList;
                    }
                    else
                    {
                        // 2回目以降は、母体のリストから別条件で抽出したタイプと積集合で抜き取る
                        typeList = tempList.Where(t => typeList.Contains(t) == true).ToList();
                    }
                }

                // 抽出したタイプを返値に追加
                retV.AddRange(typeList);
            }
            return retV;
        } 
    }

    public class ExamsMngInfoSrch_Condition
    {
        /// <summary>
        /// 試験No
        /// </summary>
        public int ExaminationNO { get; set; }

        /// <summary>
        /// 抜き取り方法No
        /// </summary>
        public int SamplingNO { get; set; }

    }
}
