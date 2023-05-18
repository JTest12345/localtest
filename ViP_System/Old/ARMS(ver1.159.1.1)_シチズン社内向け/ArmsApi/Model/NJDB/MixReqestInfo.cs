using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ArmsApi.Model.NJDB
{
    /// <summary>
    /// 調合依頼情報
    /// </summary>
    public class MixReqestInfo
    {
        /// <summary>依頼ID</summary>
        public int ReqId { get; set; }

        /// <summary>部署コード</summary>
        public string SecCd { get; set; }

        /// <summary>依頼日</summary>
        public DateTime ReqDt { get; set; }

        /// <summary>優先フラグ</summary>
        public bool PriorityFg { get; set; }

        /// <summary>樹脂グループCD</summary>
        public string ResinGroupCd { get; set; }

        /// <summary>ロット数</summary>
        public int LotCt { get; set; }

        /// <summary>更新者</summary>
        public string UpdUserCd { get; set; }

        /// <summary>使用ライン</summary>
        public string LineCd { get; set; }

        /// <summary>希望完成時間</summary>
        public DateTime ExpectCompDt { get; set; }

        /// <summary>希望調合場所</summary>
        public string ExpectPlaceCd { get; set; }

        /// <summary>調合場所</summary>
        public string ActualPlaceCd { get; set; }

        /// <summary> 調合ステータス</summary>
        public int StatusCd { get; set; }

        /// <summary> 調合ステータス名</summary>
        public string StatusNm { get; set; }

        /// <summary>調合開始日時</summary>
        public DateTime? StartDt { get; set; }

        /// <summary>調合終了日時 </summary>
        public DateTime? EndDt { get; set; }

        /// <summary>検索用調合終了日時</summary>
        public string SearchEndDt { get; set; }

        /// <summary>調合タイプ名称 </summary>
        public List<string> MixTypeNm { get; set; } = new List<string>();

        /// <summary>使用装置台数</summary>
        public int MachineCt { get; set; }

        /// <summary>先行色調フラグ)</summary>
        public bool PreFg { get; set; }

        /// <summary>コメント </summary>
        public string CommentNm { get; set; }
        
        public void Insert()
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Transaction = con.BeginTransaction();

                string sql = @" INSERT INTO JttReqest(Sec_CD,Req_DT, Priority_FG, ResinGroup_CD, Lot_CT, UpdUser_CD, 
                        Line_CD, ExpectComp_DT, ExpectPlace_CD, ActualPlace_CD, Status_CD, Start_DT, End_DT, LastUpd_DT,
                        MixType_NM, Machine_CT, Pre_FG, Comment_NM) 
                        VALUES(@SECCD, @REQDT, @PRIORITYFG, @RESINGROUPCD, @LOTCT, @UPDUSERCD,
                        @LINECD, @EXPECTCOMPDT, @EXPECTPLACECD, @ACTUALPLACECD, @STATUSCD, @STARTDT, @ENDDT, GETDATE(),
                        @MIXTYPENM, @MACHINECT, @PREFG, @COMMENTNM) ";

                try
                {
                    cmd.Parameters.Add("@SECCD", SqlDbType.NVarChar).Value = this.SecCd;
                    cmd.Parameters.Add("@REQDT", SqlDbType.NVarChar).Value = this.ReqDt;
                    cmd.Parameters.Add("@PRIORITYFG", SqlDbType.Bit).Value = this.PriorityFg;
                    cmd.Parameters.Add("@RESINGROUPCD", SqlDbType.NChar).Value = this.ResinGroupCd;
                    cmd.Parameters.Add("@LOTCT", SqlDbType.Int).Value = this.LotCt;
                    cmd.Parameters.Add("@UPDUSERCD", SqlDbType.NChar).Value = this.UpdUserCd;
                    cmd.Parameters.Add("@LINECD", SqlDbType.NVarChar).Value = this.LineCd ?? string.Empty;
                    cmd.Parameters.Add("@EXPECTCOMPDT", SqlDbType.DateTime).Value = this.ExpectCompDt;
                    cmd.Parameters.Add("@EXPECTPLACECD", SqlDbType.NVarChar).Value = this.ExpectPlaceCd ?? string.Empty;
                    cmd.Parameters.Add("@ACTUALPLACECD", SqlDbType.NVarChar).Value = this.ActualPlaceCd ?? string.Empty;
                    cmd.Parameters.Add("@STATUSCD", SqlDbType.Int).Value = this.StatusCd;
                    cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = (object)this.StartDt ?? DBNull.Value;
                    cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = (object)this.EndDt ?? DBNull.Value;
                    cmd.Parameters.Add("@MIXTYPENM", SqlDbType.NVarChar).Value = string.Join(",", this.MixTypeNm);
                    cmd.Parameters.Add("@MACHINECT", SqlDbType.Int).Value = this.MachineCt;
                    cmd.Parameters.Add("@PREFG", SqlDbType.Bit).Value = this.PreFg;
                    cmd.Parameters.Add("@COMMENTNM", SqlDbType.NVarChar).Value = this.CommentNm ?? string.Empty;

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();

                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
