using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    /// <summary>
    /// 基板DM-厚み紐付テーブル
    /// </summary>
    public class SubstrateThicknessRank
    {
        /// <summary>
        /// 厚みランクを取得
        /// </summary>
        /// <param name="datamatrix"></param>
        /// <returns></returns>
        public static string GetThicknessRank(string datamatrix)
        {
            List<string> ranklist = new List<string>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@DataMatrix", System.Data.SqlDbType.NVarChar).Value = datamatrix;

                try
                {
                    con.Open();

                    string sql = @" SELECT thicknessrank
								FROM TnSubstrateThicknessRank WITH(nolock) 
								WHERE datamatrix = @DataMatrix";

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            ranklist.Add(rd["thicknessrank"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("基板DMから厚みランク取得時にエラー発生:基板DM:{0}", datamatrix), ex);
                }
            }

            if (ranklist.Count == 1)
            {
                return ranklist[0];
            }
            else if (ranklist.Count == 0)
            {
                throw new ApplicationException(string.Format("基板DMに厚みランクが紐づいていません。基板DM：{0}", datamatrix));
            }
            else
            {
                throw new ApplicationException(string.Format("基板DMに複数の厚みランクが紐づいています。基板DM：{0}", datamatrix));
            }
        }

        public static void Update(string datamatrix)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();

                    cmd.Parameters.Add("@DATAMATRIX", SqlDbType.NVarChar).Value = datamatrix;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@UPDUSERCD", SqlDbType.Char).Value = "660";

                    //newfgを0に
                    cmd.CommandText = "UPDATE TnSubstrateThicknessRank SET newfg = 0, lastupddt=@UPDDT, updusercd=@UPDUSERCD  WHERE datamatrix=@DATAMATRIX AND newfg=1";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("基板DMと厚みの関連付け情報更新エラー", ex);
                }
            }
        }

        public static void Update(string ringdata, List<string> datamatrix)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@RINGDATA", SqlDbType.NVarChar).Value = ringdata;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@UPDUSERCD", SqlDbType.Char).Value = "660";

                    //前履歴はnewfgを0に
                    cmd.CommandText = "UPDATE TnSubstrateThicknessRank SET newfg = 0, lastupddt=@UPDDT, updusercd=@UPDUSERCD  WHERE ringdata=@RINGDATA AND newfg=1";
                    cmd.ExecuteNonQuery();

                    SqlParameter prmDataMatrix = cmd.Parameters.Add("@DATAMATRIX", SqlDbType.NVarChar);

                    //更新
                    cmd.CommandText = @"
                            UPDATE TnSubstrateThicknessRank SET newfg = 1, ringdata=@RINGDATA, lastupddt=@UPDDT, updusercd=@UPDUSERCD WHERE datamatrix=@DATAMATRIX";

                    foreach (string d in datamatrix)
                    {
                        prmDataMatrix.Value = d;

                        cmd.ExecuteNonQuery();
                    }

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw new ArmsException("基板DMと厚みの関連付け情報更新エラー", ex);
                }
            }
        }
    }
}
