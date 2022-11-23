using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;
/// <summary>
/// 富士情報　新規作成　2022/5
/// 実績収集作業場所
/// </summary>
namespace ArmsApi.Model
{
    public class POPSgyubsh
    {
        public static string GetPOPSgyubsh(string typecd)
        {
            string retv = "";

            using (SqlConnection con = new SqlConnection(Config.Settings.SCMLocalConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"SELECT S_SGYU_BSH_CD FROM M_SW_ROUTE(nolock) 
                WHERE BHNCD = @BHNCD AND S_SHIKAKARI_CD = @S_SHIKAKARI_CD AND S_SGYU_BSH_CD like @SGYU_BSH_CD AND M_SGYU_BSH_CD like @SGYU_BSH_CD";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@BHNCD", System.Data.SqlDbType.VarChar).Value = typecd;
                cmd.Parameters.Add("@S_SHIKAKARI_CD", System.Data.SqlDbType.VarChar).Value = typecd;
                cmd.Parameters.Add("@SGYU_BSH_CD", System.Data.SqlDbType.VarChar).Value = ArmsApi.Config.Settings.MANU_SGYUK_CD + "%";

                con.Open();

                try
                {
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            retv = rd["S_SGYU_BSH_CD"].ToString();
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("4Mシステムから作業場所の取得でエラーが発生しました。" + ex.ToString());
                }
            }
            return retv;
        }
    }
}
