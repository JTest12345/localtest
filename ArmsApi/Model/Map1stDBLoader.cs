using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ArmsApi.Model
{
    public class Map1stDBLoader
    {
        public int MacNo { get; set; }
        public int MagCt { get; set; }
        public int FrameCt { get; set; }
        public DateTime? WarningDt { get; set; }
        public bool IgnoreFg { get; set; }

        public void Update()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.Int).Value = this.MacNo;
                    cmd.Parameters.Add("@MAGCT", System.Data.SqlDbType.Int).Value = this.MagCt;
                    cmd.Parameters.Add("@FRAMECT", System.Data.SqlDbType.Int).Value = this.FrameCt;
                    cmd.Parameters.Add("@WARNINGDT", SqlDbType.DateTime).Value = (object)WarningDt ?? DBNull.Value;
                    cmd.Parameters.Add("@IGNOREFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IgnoreFg);

                    cmd.CommandText = @"DELETE FROM TnMap1stDBLoader WHERE macno=@MACNO";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        INSERT TnMap1stDBLoader(macno, magct, framect, warningdt, ignorefg)
                        VALUES(@MACNO, @MAGCT, @FRAMECT, @WARNINGDT, @IGNOREFG)";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("Map1stDBLoaderError:" + ex.ToString(), ex);
            }
        }

        #region Delete

        public void Delete()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.Int).Value = this.MacNo;

                    cmd.CommandText = @"DELETE FROM TnMap1stDBLoader WHERE macno=@MACNO";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("Map1stDBLoaderError:" + ex.ToString(), ex);
            }
        }
        #endregion

        public static Map1stDBLoader GetRecord(int macno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT macno, magct, framect, warningdt, ignorefg FROM TnMap1stDBLoader
                        WHERE
                          macno = @MACNO";

                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.Int).Value = macno;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            Map1stDBLoader retv = new Map1stDBLoader();
                            retv.MacNo = macno;
                            retv.MagCt = SQLite.ParseInt(rd["magct"]);
                            retv.FrameCt = SQLite.ParseInt(rd["framect"]);
                            retv.WarningDt = SQLite.ParseDate(rd["warningdt"]);
                            retv.IgnoreFg = SQLite.ParseBool(rd["ignorefg"]);

                            //MachineInfo mac = MachineInfo.GetMachine(retv.MacNo);
                            //if (mac != null) retv.MacName = mac.MachineName;

                            return retv;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ArmsException("Map1stDBLoaderError:" + ex.ToString(), ex);
            }
        }

        public static Map1stDBLoader[] GetAllRecord()
        {
            List<Map1stDBLoader> retlist = new List<Map1stDBLoader>();
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT macno, magct, framect, warningdt, ignorefg FROM TnMap1stDBLoader";


                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Map1stDBLoader retv = new Map1stDBLoader();
                            retv.MacNo = SQLite.ParseInt(rd["macno"]);
                            retv.MagCt = SQLite.ParseInt(rd["magct"]);
                            retv.FrameCt = SQLite.ParseInt(rd["framect"]);
                            retv.WarningDt = SQLite.ParseDate(rd["warningdt"]);
                            retv.IgnoreFg = SQLite.ParseBool(rd["ignorefg"]);

                            //Machine mac = Machine.GetMachine(retv.MacNo);
                            //if (mac != null) retv.MacName = mac.MachineName;

                            retlist.Add(retv);
                        }
                    }
                }

                return retlist.ToArray();
            }
            catch (Exception ex)
            {
                throw new ArmsException("Map1stDBLoaderError:" + ex.ToString(), ex);
            }
        }

        public static int GetAlertMinutes(int framect)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT max(alertminutes) lim FROM TmMapFrameBaking
                        WHERE
                          remainframect<=@REMAINCT";

                    cmd.Parameters.Add("@REMAINCT", System.Data.SqlDbType.Int).Value = framect;

                    object o = cmd.ExecuteScalar();
                    if (o == null)
                    {
                        return 0;
                    }

                    return SQLite.ParseInt(o);
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("GetBakingMasterError:" + ex.ToString(), ex);
            }
        }

        /// <summary>
        /// 新規にベーキング開始時の処理
        /// 一番優先度の高いダイボンダーの警告フラグをリセットする
        /// </summary>
        public static void NewBakingStart()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        UPDATE TnMap1stDBLoader SET ignorefg = 1
                        WHERE
                          macno = (SELECT TOP 1 macno FROM TnMap1stDBLoader ORDER BY warningdt)";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("NewBakingStartError:" + ex.ToString(), ex);
            }
        }
    }
}
