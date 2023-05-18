using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class ReservedOrder
    {

        #region プロパティ
        /// <summary>
        /// ロット情報
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 搬入マガジンNO
        /// </summary>
        public string InMagazineNo { get; set; }

        /// <summary>
        /// 装置NO
        /// </summary>
        public int MacNo { get; set; }

        /// <summary>
        /// 予約日
        /// </summary>
        public DateTime ReservedDt { get; set; }

        /// <summary>
        /// 工程ID
        /// </summary>
        public int ProcNo { get; set; }

        /// <summary>
        /// 工程作業者
        /// </summary>
        public string EmpCd { get; set; }
        #endregion

        #region DeleteInsert

        /// <summary>
        /// ロット番号をキーに予約更新
        /// </summary>
        public void DeleteInsert()
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@PROC", SqlDbType.BigInt).Value = this.ProcNo;
                cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = this.MacNo;
                cmd.Parameters.Add("@INMAG", SqlDbType.NVarChar).Value = this.InMagazineNo ?? "";
                cmd.Parameters.Add("@RESERVEDDT", SqlDbType.DateTime).Value = this.ReservedDt;
                cmd.Parameters.Add("@EMPCD", SqlDbType.NVarChar).Value = this.EmpCd ?? "";

                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    //前履歴は削除
                    cmd.CommandText = "DELETE FROM TnReservedTran WHERE lotno=@LOTNO";
                    cmd.ExecuteNonQuery();

                    //新規Insert
                    cmd.CommandText = @"
                            INSERT
                             INTO TnReservedTran(lotno
	                            , procno
	                            , macno
	                            , inmag
	                            , reserveddt
                                , empcd)
                            values(@LOTNO
	                            , @PROC
	                            , @MACNO
	                            , @INMAG
	                            , @RESERVEDDT
                                , @EMPCD)";

                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    cmd.Transaction.Rollback();
                    throw ex;
                }
            }
        }
        #endregion

        #region GetReserveList
        /// <summary>
        /// 装置で予約されているロットを全て取得
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static ReservedOrder[] GetReserveList(int macno)
        {
            List<ReservedOrder> retv = new List<ReservedOrder>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                        SELECT 
                          t.lotno , 
                          t.procno , 
                          t.macno , 
                          t.inmag , 
                          t.reserveddt , 
                          t.empcd
                        FROM 
                          TnReservedTran t 
                        WHERE 
                          t.macno = @MACNO";

                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {

                        while (reader.Read())
                        {
                            ReservedOrder o = new ReservedOrder();
                            o.LotNo = SQLite.ParseString(reader["lotno"]);
                            o.MacNo = SQLite.ParseInt(reader["macno"]);
                            o.ProcNo = SQLite.ParseInt(reader["procno"]);
                            o.InMagazineNo = SQLite.ParseString(reader["inmag"]);
                            o.ReservedDt = SQLite.ParseDate(reader["reserveddt"]) ?? DateTime.MinValue;
                            o.EmpCd = SQLite.ParseString(reader["empcd"]);

                            retv.Add(o);
                        }
                    }

                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }
        #endregion

        #region Delete
        /// <summary>
        /// ロット番号単位で予約解除
        /// </summary>
        public void Delete()
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                try
                {
                    con.Open();
                    cmd.CommandText = "DELETE FROM TnReservedTran WHERE lotno=@LOTNO";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }
        public static void Delete(string lotno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                try
                {
                    con.Open();
                    cmd.CommandText = "DELETE FROM TnReservedTran WHERE lotno=@LOTNO";
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Log.SysLog.Info(ex.ToString());
                    throw ex;
                }
            }
        }
        #endregion

        /// <summary>
        /// 装置の予約内容を全クリア
        /// </summary>
        /// <param name="macno"></param>
        public static void ClearReserveList(int macno)
        {
            ReservedOrder[] rolist = ReservedOrder.GetReserveList(macno);
            foreach (ReservedOrder ro in rolist)
            {
                ro.Delete();
            }
        }
    }
}
