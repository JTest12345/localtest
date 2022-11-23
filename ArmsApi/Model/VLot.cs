using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;

namespace ArmsApi.Model
{
    public class VLot
    {
        /// <summary>
        /// VロットNo
        /// </summary>
        public string VLotNo { get; set; }

        /// <summary>
        /// 基板CD
        /// </summary>
        public string MaterialCd { get; set; }

        /// <summary>
        /// V溝工程あり
        /// </summary>
        public bool IsVM { get; set; }

        /// <summary>
        /// バックアップ対象
        /// </summary>
        public bool IsBackUp { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        public string ErrDescription { get; set; }

        /// <summary>
        /// 4Mロット情報リスト
        /// </summary>
        public List<SeiLot> SeiLotLst { get; set; } = new List<SeiLot>();

        /// <summary>
        /// 4Mロット情報
        /// </summary>
        public class SeiLot
        {
            public string SeiLotNo;
            public string TypeCd;
            public int ProcNo;
            public int FrameQty;
            //分割前の4Mロット番号
            public string SeiLotNoOrg;

            public SeiLot()
            {
                SeiLotNo = "";
                TypeCd = "";
                FrameQty = 0;
            }
        }

        /// <summary>
        /// 基板ロット情報リスト
        /// </summary>
        public List<MatLot> MatLotLst { get; set; } = new List<MatLot>();

        /// <summary>
        /// 基板ロット情報
        /// </summary>
        public class MatLot
        {
            public string MatLotNo;
            public int FrameQty;

            public MatLot()
            {
                MatLotNo = "";
                FrameQty = 0;
            }
        }

        public static VLot GetVLotInfo(string vlotno)
        {
            VLot retv = new VLot();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
 
                    cmd.Parameters.Add("@VLOTNO", SqlDbType.NVarChar).Value = vlotno;

                    if (!GetVLot(cmd, ref retv))
                    {
                        return retv;
                    }
                    if (string.IsNullOrEmpty(retv.VLotNo))
                    {
                        retv.ErrDescription = "TnVLotが存在しません。";
                        return retv;
                    }

                    if (!Get4MLot(cmd, ref retv))
                    {
                        return retv;
                    }
                    if (!GetMatLot(cmd, ref retv))
                    {
                        return retv;
                    }
                }
            }
            catch (Exception ex)
            {
                retv.ErrDescription = ex.Message;
            }

            return retv;
        }

        /// <summary>
        /// TnVlot取得
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="vl"></param>
        /// <returns></returns>
        static bool GetVLot(SqlCommand cmd, ref VLot vl)
        {
            bool retv = true;

            string sql = $@" SELECT vlotno, materialcd, isvm, isbackup
                            FROM TnVLot(nolock) WHERE vlotno = @VLOTNO";


            cmd.CommandText = sql;

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                if (rd.Read())
                {
                    vl.VLotNo = rd["vlotno"].ToString().TrimEnd();
                    vl.MaterialCd = rd["materialcd"].ToString().TrimEnd();
                    vl.IsVM = Convert.ToBoolean(rd["isvm"]);
                    vl.IsBackUp = Convert.ToBoolean(rd["isbackup"]);
                }
            }

            return retv;
        }

        /// <summary>
        /// TnTnVLot4M取得
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="vl"></param>
        /// <returns></returns>
        static bool Get4MLot(SqlCommand cmd, ref VLot vl)
        {
            bool retv = true;

            string sql = $@" SELECT vlotno, typecd, seilotno
                    FROM TnVLot4M(nolock) WHERE vlotno = @VLOTNO ORDER BY seqno";

            cmd.CommandText = sql;

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    SeiLot sl = new SeiLot();
                    sl.TypeCd = rd["typecd"].ToString().TrimEnd();
                    sl.SeiLotNo = rd["seilotno"].ToString().TrimEnd();
                    vl.SeiLotLst.Add(sl);
                }
            }

            return retv;
        }

        /// <summary>
        ///TnVLotMat取得
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="vl"></param>
        /// <returns></returns>
        static bool GetMatLot(SqlCommand cmd, ref VLot vl)
        {
            bool retv = true;

            string sql = $@" SELECT vlotno, seqno, matlotno, frameqty
                    FROM TnVLotMat(nolock)WHERE vlotno = @VLOTNO ORDER BY seqno";

            cmd.CommandText = sql;

            using (SqlDataReader rd = cmd.ExecuteReader())
            {
                while (rd.Read())
                {
                    MatLot ml = new MatLot();
                    ml.MatLotNo = rd["matlotno"].ToString().TrimEnd();
                    ml.FrameQty = Convert.ToInt32(rd["frameqty"]);
                    vl.MatLotLst.Add(ml);
                }
            }

            return retv;
        }

        /// <summary>
        /// TnTnVLot4M取得
        /// </summary>
        /// <param name="cmd"></param>
        /// <param name="vl"></param>
        /// <returns></returns>
        public static VLot GetVLotFrom4MLot(string seilotno)
        {
            VLot retv = new VLot();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = $@" SELECT v.vlotno, v.materialcd, v.isvm, v.isbackup, s.typecd, s.seilotno, m.matlotno
                    FROM TnVLot4M(nolock) s INNER JOIN TnVLot(nolock) v ON s.vlotno = v.vlotno Left JOIN TnMatRelation(nolock) m ON s.vlotno = m.lotno WHERE seilotno = @SEILOTNO";

                cmd.Parameters.Add("@SEILOTNO", SqlDbType.NVarChar).Value = seilotno;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        if (string.IsNullOrEmpty(retv.VLotNo))
                        {
                            retv.VLotNo = rd["vlotno"].ToString().TrimEnd();
                            retv.MaterialCd = rd["materialcd"].ToString().TrimEnd();
                            retv.IsVM = Convert.ToBoolean(rd["isvm"]);
                            retv.IsBackUp = Convert.ToBoolean(rd["isbackup"]);

                            SeiLot sl = new SeiLot();
                            sl.TypeCd = rd["typecd"].ToString().TrimEnd();
                            sl.SeiLotNo = rd["seilotno"].ToString().TrimEnd();
                            retv.SeiLotLst.Add(sl);
                        }

                        MatLot ml = new MatLot();
                        ml.MatLotNo = rd["matlotno"].ToString().TrimEnd();
                        retv.MatLotLst.Add(ml);
                    }
                }
            }

            return retv;
        }
    }
}
