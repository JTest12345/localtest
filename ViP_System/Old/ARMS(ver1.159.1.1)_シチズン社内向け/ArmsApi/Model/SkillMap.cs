using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class SkillMap
    {
        public static bool HasSkill(string usercd, string workcd)
        {
            List<DateTime> expirydtlist = new List<DateTime>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@UserCD", System.Data.SqlDbType.NVarChar).Value = usercd;
                cmd.Parameters.Add("@WorkCD", System.Data.SqlDbType.NVarChar).Value = workcd;

                try
                {
                    con.Open();

                    string sql = @" SELECT expirydt
								FROM TnSkillMap WITH(nolock) 
								WHERE usercd = @UserCD 
                                AND workcd = @WorkCD";

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            expirydtlist.Add(SQLite.ParseDate(rd["expirydt"]).Value);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("スキルマップ認定データ取得時にエラー発生:作業者:{0} 作業:{1}", usercd, workcd), ex);
                }
            }

            bool retv = false;
            foreach (DateTime expirydt in expirydtlist)
            {
                if (expirydt > DateTime.Now)
                {
                    retv = true;
                    break;
                }
            }
            return retv;
        }
    }
}
