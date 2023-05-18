using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class LotTrayDataMatrix
    {

        /// <summary>
        /// トレイDMからロット番号取得
        /// </summary>
        /// <param name="trayDataMatrix"></param>
        /// <param name="onlyActiveCarrier"></param>
        /// <returns></returns>
        public static string GetLotNo(string trayDataMatrix, bool onlyActiveCarrier)
        {
            List<string> lotlist = new List<string>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@TrayDataMatrix", System.Data.SqlDbType.NVarChar).Value = trayDataMatrix;
                cmd.Parameters.Add("@OperateFG", System.Data.SqlDbType.Int).Value = Convert.ToInt32(onlyActiveCarrier);

                try
                {
                    con.Open();

                    string sql = @" SELECT lotno
								FROM TnLotTrayDataMatrix WITH(nolock) 
								WHERE operatefg = @OperateFG 
                                AND traydatamatrix = @TrayDataMatrix";

                    cmd.CommandText = sql;
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            lotlist.Add(rd["lotno"].ToString().Trim());
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException(string.Format("トレイDMからロット番号取得時にエラー発生:トレイDM:{0}", trayDataMatrix), ex);
                }
            }

            if (lotlist.Count == 1)
            {
                return lotlist[0];
            }
            else if (lotlist.Count == 0)
            {
                throw new ApplicationException(string.Format("トレイDMにロットが紐づいていません。トレイDM：{0}", trayDataMatrix));
            }
            else
            {
                throw new ApplicationException(string.Format("トレイDMに複数のロットが紐づいています。トレイDM：{0}", trayDataMatrix));
            }
        }

        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    string sql = " DELETE FROM TnLotTrayDataMatrix WHERE lotno = @LotNo";
                    cmd.CommandText = sql;

                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@LotNo", System.Data.SqlDbType.NVarChar).Value = lotno;

                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}
