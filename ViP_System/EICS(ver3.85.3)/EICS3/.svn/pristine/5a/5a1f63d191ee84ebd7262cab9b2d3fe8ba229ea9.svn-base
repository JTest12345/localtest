using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database.NASCA
{
    public class Section
    {
        public static string GetSectionCd(string sectionNm, int hostLineCd)
        {
            using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.NASCA, hostLineCd)))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT scode
                                FROM dbo.RvMSECTION
                                WHERE (sname = @SECTIONNM) AND (void_p = 0)
                                OPTION(MAXDOP 1)";

                cmd.Parameters.Add("@SECTIONNM", System.Data.SqlDbType.NVarChar).Value = sectionNm;
                cmd.CommandText = sql;

                var sectionCd = cmd.ExecuteScalar();
                if (sectionCd == null)
                {
                    throw new ApplicationException($"部署マスタに該当部署が存在しません。SectionNm = {sectionNm}");
                }
                else
                {
                    return Convert.ToString(sectionCd);
                }
            }

        }

    }
}
