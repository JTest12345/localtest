using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace GEICS.Database
{
    public class Log
    {
        public static string GetMagazineNO(string lotno, string equipmentno, int lineno, DateTime startDT, DateTime endDT)
        {
            string retv = string.Empty;

            using (SqlConnection conn = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

				string sql = @" SELECT TOP 1 Magazine_NO FROM TnLOG WITH(nolock)
								WHERE NascaLot_NO = @LOTNO AND Equipment_NO = @EQUINO AND Inline_CD = @LINECD 
								AND Measure_DT >= @StartMeasureDT AND Measure_DT < @EndMeasureDT
								AND Magazine_NO <> '' AND Magazine_NO IS NOT NULL ";
				

                cmd.Parameters.Add("@LOTNO",SqlDbType.VarChar).Value = lotno;
                cmd.Parameters.Add("@EQUINO", SqlDbType.Char).Value = equipmentno;
                cmd.Parameters.Add("@LINECD", SqlDbType.Int).Value = lineno;
				cmd.Parameters.Add("@StartMeasureDT", SqlDbType.DateTime).Value = startDT;
				cmd.Parameters.Add("@EndMeasureDT", SqlDbType.DateTime).Value = endDT;

                cmd.CommandText = sql;
                object magno = cmd.ExecuteScalar();
                if (magno == null || magno == System.DBNull.Value)
                {
                    return "";
                }
                else 
                {
                    return magno.ToString().Trim();
                }
            }
        }
    }
}
