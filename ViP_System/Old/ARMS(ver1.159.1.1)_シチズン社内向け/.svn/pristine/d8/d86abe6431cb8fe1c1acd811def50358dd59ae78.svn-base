using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class PastePosition
    {
        public string TypeCd { get; set; }

        public string WorkCd { get; set; }

        public string LeftTypeCd { get; set; }

        public string RightTypeCd { get; set; }

        public static List<PastePosition> GetData(string typecd, string workcd)
        {
            List<PastePosition> retV = new List<PastePosition>();
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    string sql = @" SELECT typecd, workcd, lefttypecd, righttypecd
                                    FROM TmPastePosition WITH(nolock)
                                    WHERE (typecd = @TYPECD) AND (workcd = @WORKCD) AND (delfg = 0)
                                    OPTION(MAXDOP 1) ";
                    cmd.CommandText = sql;
                    cmd.Parameters.Add("@TYPECD", SqlDbType.VarChar).Value = typecd;
                    cmd.Parameters.Add("@WORKCD", SqlDbType.VarChar).Value = workcd;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        int ordTypeCd = rd.GetOrdinal("typecd");
                        int ordWorkCd = rd.GetOrdinal("typecd");
                        int ordLeftTypeCd = rd.GetOrdinal("lefttypecd");
                        int ordRightTypeCd = rd.GetOrdinal("righttypecd");
                        while (rd.Read())
                        {
                            PastePosition pPos = new PastePosition();
                            pPos.TypeCd = SQLite.ParseString(rd[ordTypeCd]);
                            pPos.WorkCd = SQLite.ParseString(rd[ordWorkCd]);
                            pPos.LeftTypeCd = SQLite.ParseString(rd[ordLeftTypeCd]);
                            pPos.RightTypeCd = SQLite.ParseString(rd[ordRightTypeCd]);

                            retV.Add(pPos);
                        }
                    }
                }

                return retV;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }
    }
}
