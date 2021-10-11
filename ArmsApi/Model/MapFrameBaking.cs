using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ArmsApi.Model
{
    public class MapFrameBaking
    {
        /// <summary>
        /// 基板ロット番号
        /// </summary>
        public string FrameLotNo { get; set; }

        /// <summary>
        /// マガジンインデックス情報
        /// </summary>
        public string MagIndex { get; set; }

        /// <summary>
        /// 作業中DB装置番号
        /// </summary>
        public int? CurrentWorkingMacNo { get; set; }

        /// <summary>
        /// 作業中DB号機名称
        /// </summary>
        public string CurrentWorkingMacName { get; private set; }

        /// <summary>
        /// 有効期限
        /// </summary>
        public DateTime LimitDt { get; set; }

        /// <summary>
        /// ベーキング回数
        /// </summary>
        public int BakingCt { get; set; }

        /// <summary>
        /// ベーキング履歴無効フラグ
        /// </summary>
        public bool IgnoreLastBakingFg { get; set; }

        public void Update()
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.Parameters.Add("@FRAMELOTNO", System.Data.SqlDbType.NVarChar).Value = this.FrameLotNo;
                    cmd.Parameters.Add("@MAGINDEX", System.Data.SqlDbType.NVarChar).Value = this.MagIndex;
                    cmd.Parameters.Add("@MACNO", SqlDbType.Int).Value = (object)this.CurrentWorkingMacNo ?? DBNull.Value;
                    cmd.Parameters.Add("@LIMITDT", SqlDbType.DateTime).Value = this.LimitDt;
                    cmd.Parameters.Add("@BAKINGCT", SqlDbType.Int).Value = this.BakingCt;
                    cmd.Parameters.Add("@IGNOREFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.IgnoreLastBakingFg);

                    cmd.CommandText = @"DELETE FROM TnMapFrameBaking WHERE framelotno=@FRAMELOTNO AND magindex=@MAGINDEX";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        INSERT TnMapFrameBaking(framelotno, magindex, currentworkingmacno, limitdt, bakingct, ignorefg)
                        VALUES(@FRAMELOTNO, @MAGINDEX, @MACNO, @LIMITDT, @BAKINGCT, @IGNOREFG)";

                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("GetBakingRecordError:" + ex.ToString(), ex);
            }

        }

        public static MapFrameBaking[] GetNotIgnoredList()
        {
            List<MapFrameBaking> retlist = new List<MapFrameBaking>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT framelotno, magindex, currentworkingmacno, limitdt, bakingct, ignorefg FROM TnMapFrameBaking
                        WHERE
                          ignorefg = 0";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            MapFrameBaking retv = new MapFrameBaking();
                            retv.FrameLotNo = SQLite.ParseString(rd["framelotno"]);
                            retv.MagIndex = SQLite.ParseString(rd["magindex"]);
                            if (rd["currentworkingmacno"] == DBNull.Value)
                            {
                                retv.CurrentWorkingMacNo = null;
                                retv.CurrentWorkingMacName = null;
                            }
                            else
                            {
                                retv.CurrentWorkingMacNo = SQLite.ParseInt(rd["currentworkingmacno"]);
                                MachineInfo mac = MachineInfo.GetMachine(retv.CurrentWorkingMacNo.Value);
                                if (mac != null) retv.CurrentWorkingMacName = mac.MachineName;
                            }

                            retv.BakingCt = SQLite.ParseInt(rd["bakingct"]);
                            retv.LimitDt = SQLite.ParseDate(rd["limitdt"]) ?? DateTime.MinValue;
                            retv.IgnoreLastBakingFg = SQLite.ParseBool(rd["ignorefg"]);


                            retlist.Add(retv);
                        }
                    }
                }

                return retlist.ToArray();
            }
            catch (Exception ex)
            {
                throw new ArmsException("GetBakingRecordError:" + ex.ToString(), ex);
            }
        }

        public static MapFrameBaking GetBakingRecord(string framelotno, string magindex)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT framelotno, magindex, currentworkingmacno, limitdt, bakingct, ignorefg FROM TnMapFrameBaking
                        WHERE
                          framelotno=@FRAMELOTNO AND magindex=@MAGINDEX";

                    cmd.Parameters.Add("@FRAMELOTNO", System.Data.SqlDbType.NVarChar).Value = framelotno;
                    cmd.Parameters.Add("@MAGINDEX", System.Data.SqlDbType.NVarChar).Value = magindex ?? "";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            MapFrameBaking retv = new MapFrameBaking();
                            retv.FrameLotNo = framelotno;
                            retv.MagIndex = magindex;
                            if (rd["currentworkingmacno"] == DBNull.Value)
                            {
                                retv.CurrentWorkingMacNo = null;
                                retv.CurrentWorkingMacName = null;
                            }
                            else
                            {
                                retv.CurrentWorkingMacNo = SQLite.ParseInt(rd["currentworkingmacno"]);
                                MachineInfo mac = MachineInfo.GetMachine(retv.CurrentWorkingMacNo.Value);
                                if (mac != null) retv.CurrentWorkingMacName = mac.MachineName;
                            }

                            retv.BakingCt = SQLite.ParseInt(rd["bakingct"]);
                            retv.LimitDt = SQLite.ParseDate(rd["limitdt"]) ?? DateTime.MinValue;
                            retv.IgnoreLastBakingFg = SQLite.ParseBool(rd["ignorefg"]);

                            return retv;
                        }
                    }
                }

                return null;
            }
            catch (Exception ex)
            {
                throw new ArmsException("GetBakingRecordError:" + ex.ToString(), ex);
            }
        }

        public static int GetBakingLimitMinutes()
        {
            //当面は２４時間固定
            //将来的には品目等をキーに変更の可能性あり
            return 1440;
        }

        /// <summary>
        /// フレーム内容枚数に応じて、何分前までにDB投入が必要かの制限時間取得
        /// </summary>
        /// <returns></returns>
        public static int GetInputLimitMinutes(int framect)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT max(inputlimitminutes) lim FROM TmMapFrameBaking
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

        public static int GetBakingProfileNo()
        {
            return 4;
        }
    }
}
