using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class ErrTrace
    {
        /// <summary>
        /// ロットNO
        /// </summary>
        public string LotNo { get; set; }

        /// <summary>
        /// 検査結果
        /// </summary>
        public string InspectionResult { get; set; }

        /// <summary>
        /// 側面ロットマーキング
        /// </summary>
        public string SideMarking { get; set; }

        /// <summary>
        /// 裏面ロットマーキング
        /// </summary>
        public string BackSideMarking { get; set; }

        /// <summary>
        /// コンタクトミス
        /// </summary>
        public string Contactmiss { get; set; }

        public int? XAddress { get; set; }

        public int? YAddress { get; set; }

        public string ErrDetail { get; set; }

        public int ErrProcNo { get; set; }
        
        /// <summary>
        /// インライン工程名
        /// </summary>
        public string ErrInlineProNM { get; set; }

        public void Insert()
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @" INSERT INTO TnErrTrace(lotno, backsidemarking, sidemarking, xaddress, yaddress, errdetail, errprocno) VALUES
									(@LotNO, @BackSideMarking, @SideMarking, @XAddress, @YAddress, @ErrDetail, @ErrProcNo) ";

                cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@BackSideMarking", SqlDbType.NVarChar).Value = this.BackSideMarking ?? (object)DBNull.Value;
                cmd.Parameters.Add("@SideMarking", SqlDbType.NVarChar).Value = this.SideMarking ?? (object)DBNull.Value;
                cmd.Parameters.Add("@XAddress", SqlDbType.BigInt).Value = this.XAddress ?? (object)DBNull.Value;
                cmd.Parameters.Add("@YAddress", SqlDbType.BigInt).Value = this.YAddress ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ErrDetail", SqlDbType.NVarChar).Value = this.ErrDetail;
                cmd.Parameters.Add("@ErrProcNo", SqlDbType.BigInt).Value = this.ErrProcNo;

                cmd.ExecuteNonQuery();
            }
        }

        public static ErrTrace[] GetDatas(string lotno, string sidemarking, string backsidemarking)
        {
            List<ErrTrace> retv = new List<ErrTrace>();

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT lotno, backsidemarking, sidemarking, xaddress, yaddress, errdetail, errprocno
								FROM TnErrTrace WITH (nolock) 
								WHERE 1=1 ";

                if (!string.IsNullOrEmpty(lotno))
                {
                    sql += " AND (lotno = @LotNO) ";
                    cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar).Value = lotno;
                }

                if (!string.IsNullOrEmpty(sidemarking))
                {
                    sql += " AND (backsidemarking = @BackSideMarking) ";
                    cmd.Parameters.Add("@BackSideMarking", SqlDbType.NVarChar).Value = sidemarking;
                }

                if (!string.IsNullOrEmpty(backsidemarking))
                {
                    sql += " AND (sidemarking = @SideMarking) ";
                    cmd.Parameters.Add("@SideMarking", SqlDbType.NVarChar).Value = backsidemarking;
                }

                cmd.CommandText = sql;
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ErrTrace e = new ErrTrace();

                        e.LotNo = SQLite.ParseString(reader["lotno"]);
                        e.BackSideMarking = SQLite.ParseString(reader["backsidemarking"]);
                        e.SideMarking = SQLite.ParseString(reader["sidemarking"]);
                        e.XAddress = SQLite.ParseNullableInt(reader["xaddress"]);
                        e.YAddress = SQLite.ParseNullableInt(reader["yaddress"]);
                        e.ErrDetail = SQLite.ParseString(reader["errdetail"]);
                        e.ErrProcNo = SQLite.ParseInt(reader["errprocno"]);
                        e.ErrInlineProNM = Process.GetProcess(e.ErrProcNo).InlineProNM;

                        retv.Add(e);
                    }
                }
            }

            return retv.ToArray();
        }
    }
}
