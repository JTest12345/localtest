using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
{
    class PrmSummary
    {
        public int QcParamNO { get; set; }
        public string TypeCD { get; set; }
        public string SummaryKB { get; set; }
        public Summary SummaryInfo { get; set; }
        public int DataCT { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }

        public enum Summary 
        {
            All,
            Head,
            HeadOut,
			Last,
        }

        public static PrmSummary GetSingleData(int qcParamNo, string typeCd, int serverLineCd)
        {
            List<PrmSummary> data = GetData(qcParamNo, typeCd, serverLineCd);
            if (data.Count == 0)
            {
                return null;
            }
            else 
            {
                return data.Single();
            }
        }

        public static List<PrmSummary> GetData(int? qcParamNo, string typeCd, int serverLineCd)
        {
            List<PrmSummary> summaryList = new List<PrmSummary>();

			using (SqlConnection conn = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, serverLineCd)))
			using (SqlCommand cmd = conn.CreateCommand())
			{
                conn.Open();

                string sql = @" SELECT QcParam_NO, Type_CD, Summary_KB, Data_CT, Del_FG, UpdUser_CD, LastUpd_DT 
                                FROM TmPRMSUMMARY WITH(nolock) 
                                WHERE Del_FG = 0 ";

                if (qcParamNo.HasValue) 
                {
                    sql += " AND QcParam_NO = @QcParamNO ";
                    cmd.Parameters.Add("@QcParamNO", SqlDbType.Int).Value = qcParamNo.Value;
                }

                if (!string.IsNullOrEmpty(typeCd))
                {
                    sql += " AND Type_CD = @TypeCD ";
                    cmd.Parameters.Add("@TypeCD", SqlDbType.NVarChar).Value = typeCd;
                }

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader()) 
                {
                    while (rd.Read()) 
                    {
                        PrmSummary summary = new PrmSummary();

                        summary.QcParamNO =  Convert.ToInt32(rd["QcParam_NO"]);
                        summary.TypeCD = rd["Type_CD"].ToString().Trim();
                        summary.SummaryKB = rd["Summary_KB"].ToString().Trim();
                        summary.DataCT = Convert.ToInt32(rd["Data_CT"]);
                        summary.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                        summary.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
                        summary.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);

                        if (summary.SummaryKB == "1")
                        {
                            summary.SummaryInfo = Summary.All;
                        }
                        else if (summary.SummaryKB == "2")
                        {
                            summary.SummaryInfo = Summary.Head;
                        }
                        else if (summary.SummaryKB == "3")
                        {
                            summary.SummaryInfo = Summary.HeadOut;
                        }
						else if (summary.SummaryKB == "4")
						{
							summary.SummaryInfo = Summary.Last;
						}
						else
						{
							summary.SummaryInfo = Summary.All;
						}

                        summaryList.Add(summary);
                    }   
                }
            }

            return summaryList;
        }

        public static string[] ConvertToSummaryData(string[] fileContents, PrmSummary prmSummary) 
        {
            string[] retv;

            if (prmSummary.SummaryInfo == Summary.All)
            {
                //全て
                retv = fileContents;
            }
            else if (prmSummary.SummaryInfo == Summary.Head)
            {
                //先頭n行のみ
                retv = fileContents.Take(prmSummary.DataCT).ToArray();
            }
            else if (prmSummary.SummaryInfo == Summary.HeadOut)
            {
                //先頭n行除外
                retv = fileContents.Skip(prmSummary.DataCT).ToArray();
            }
			else if (prmSummary.SummaryInfo == Summary.Last)
			{
				//ラストn行のみ
				retv = fileContents.Reverse().Take(prmSummary.DataCT).ToArray();
			}
			else
			{
				throw new ApplicationException(string.Format("項目集計マスタに想定外の集計区分が設定されています。集計区分:{0}", prmSummary.ToString()));
			}

            return retv.ToArray();
        }

		public static string[] ConvertToSummaryData(string[] fileContents, int qcParamNo, string typeCd, int serverLineCd)
		{
			PrmSummary prmSummary = GetSingleData(qcParamNo, typeCd, serverLineCd);

			if (prmSummary == null)
			{
				return fileContents;
			}
			else
			{
				return ConvertToSummaryData(fileContents, prmSummary);
			}
		}
    }
}
