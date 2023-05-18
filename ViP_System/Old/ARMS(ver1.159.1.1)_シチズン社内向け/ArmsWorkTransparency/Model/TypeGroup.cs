using ArmsApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsWorkTransparency.Model
{
    public class TypeGroup
    {
        /// <summary>
        /// まとめ型番取得(まとめ型番が登録されていない場合は詳細型番を返す)
        /// </summary>
        /// <param name="typeCd"></param>
        /// <returns></returns>
        public static List<string> GetGroupCode(string typeCd)
        {
            List<string> retv = new List<string>();
            
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @" SELECT distinct TypeGroup_CD FROM TmTypeGroup WITH(nolock) WHERE Del_FG = 0 AND Type_CD = @TYPECD ";

                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(rd["TypeGroup_CD"].ToString());
                        //object typeGroupCd = cmd.ExecuteScalar();
                        //if (typeGroupCd == null)
                        //{
                        //    return typeCd;
                        //    //throw new ApplicationException($"まとめ型番の設定が無い型番です。型番:{ typeCd }");
                        //}
                        //else
                        //{
                        //    return typeGroupCd.ToString();
                        //}
                    }
                }
            }
            return retv.Distinct().ToList();
        }
    }
}
