using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api.ARMS
{
    public class Restrict
    {
        /// <summary>
        /// 周辺強度試験のための排出
        /// </summary>
        public const string RESTRICT_REASON_WIRE_2 = "WBエラーに対する周辺強度実施の為";
        public const string RESTRICT_REASON_DIESHEARSAMPLE = "ダイシェア抜き取り対象ロットの為";
        
        /// <summary>
        /// マッピング不一致の為の排出
        /// </summary>
        public const string RESTRICT_REASON_MD_MAPPING_NG = "マッピング照合NGの為、流動禁止";

        public const string TESTMODE_DIESHEAR = "DIESHEAR";
        public const string TESTMODE_PULL = "PULL";
        public const string TESTMODE_SHEAR = "SHEAR";

        public const string INSPECT_WORKCD = "WB0004";

        public const string RESTRICT_REASON_KB_NOTCHECK_WORK_START = "作業開始規制チェック対象外";

        string _lotno;
        /// <summary>
        /// ロット番号
        /// </summary>
        public string LotNo { get { return _lotno; } set { _lotno = WorkMagazine.MagLotToNascaLot(value); } }

        /// <summary>
        /// 投入規制工程
        /// </summary>
        public int ProcNo { get; set; }

        /// <summary>
        /// 規制理由
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool DelFg { get; set; }

        /// <summary>
        /// 最終更新日
        /// </summary>
        public DateTime LastUpdDt { get; set; }

        /// <summary>
        /// 規制理由区分
        /// </summary>
        public string ReasonKb { get; set; }

        #region SearchRestrict

        /// <summary>
        /// 流動規制情報検索
        /// </summary>
        /// <param name="lotno">null許可</param>
        /// <param name="procno">工程ID</param>
        /// <param name="onlyActive">DelFg除くフラグ</param>
        /// <returns></returns>
        public static Restrict[] SearchRestrict(string lotno, int? procno, bool onlyActive)
        {
            return SearchRestrict(lotno, procno, onlyActive, SQLite.ArmsConStr, null);
        }

        /// <summary>
        /// 流動規制情報検索
        /// </summary>
        /// <param name="lotno">null許可</param>
        /// <param name="procno">工程ID</param>
        /// <param name="onlyActive">DelFg除くフラグ</param>
        /// <returns></returns>
        public static Restrict[] SearchRestrict(string lotno, int? procno, bool onlyActive, string constr, string reason)
        {
            //マガジン分割対応
            lotno = WorkMagazine.MagLotToNascaLot(lotno);

            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            List<Restrict> retv = new List<Restrict>();

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT lotno, procno, reason, delfg, lastupddt, reasonkb FROM TnRestrict WHERE 1=1 ";

                if (onlyActive)
                {
                    cmd.CommandText += " AND delfg=0";
                }

                if (lotno != null)
                {
                    cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotno;
                    cmd.CommandText += " AND lotno = @LOTNO";
                }

                if (procno.HasValue)
                {
                    cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procno;
                    cmd.CommandText += " AND procno=@PROCNO";
                }

                if (string.IsNullOrEmpty(reason) == false)
                {
                    cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = reason;
                    cmd.CommandText += " AND reason = @REASON";
                }

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Restrict r = new Restrict();
                        r.LotNo = SQLite.ParseString(rd["lotno"]);
                        r.ProcNo = SQLite.ParseInt(rd["procno"]);
                        r.Reason = SQLite.ParseString(rd["reason"]);
                        r.DelFg = SQLite.ParseBool(rd["delfg"]);
                        r.LastUpdDt = SQLite.ParseDate(rd["lastupddt"]) ?? DateTime.MinValue;
                        r.ReasonKb = SQLite.ParseString(rd["reasonkb"]);

                        retv.Add(r);
                    }
                }

            }

            return retv.ToArray();
        }

        public static Restrict[] SearchRestrict(bool onlyActive, string reason)
        {
            return SearchRestrict(null, null, onlyActive, SQLite.ArmsConStr, reason);
        }
        #endregion

        public void Save()
        {
            Save(LENS2_Api.Config.Settings.ArmsConnectionString);
        }

        /// <summary>
        /// Insert or Update
        /// </summary>
        public void Save(string constr)
        {
            //ライン受渡しに使われるため呼び出し先全てにconstrの受け渡し必要
            if (string.IsNullOrEmpty(LotNo)) return;

            Restrict[] exists = SearchRestrict(LotNo, ProcNo, false, constr, null);
            foreach (Restrict r in exists)
            {
                if (r.Reason == this.Reason)
                {
                    //規制理由まで全く同じものが見つかった場合はUPDATEして終了
                    r.LastUpdDt = DateTime.Now;
                    r.DelFg = this.DelFg;
                    r.ReasonKb = this.ReasonKb;
                    r.updateDB(constr);
                    return;
                }
            }

            //既存レコードが無ければ新規Insert
            this.insertDB(constr);
        }


        #region Insert/Update
        
        /// <summary>
        /// Update
        /// </summary>
        private void updateDB(string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"
                        UPDATE TnRestrict SET lastupddt=@UPDDT, delfg=@DELFG, reasonkb=@REASONKB 
                        WHERE lotno=@LOTNO AND procno=@PROCNO AND reason=@REASON";

                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.ProcNo;
                cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = this.Reason;
                cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.DelFg);
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@REASONKB", SqlDbType.NVarChar).Value = (object)this.ReasonKb ?? DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }


        /// <summary>
        /// Insert
        /// </summary>
        private void insertDB(string constr)
        {
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"
                        INSERT TnRestrict(lotno, procno, reason, delfg, lastupddt, reasonkb)
                        VALUES (@LOTNO, @PROCNO, @REASON, @DELFG, @UPDDT, @REASONKB)";

                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = this.LotNo;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = this.ProcNo;
                cmd.Parameters.Add("@REASON", SqlDbType.NVarChar).Value = this.Reason;
                cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(this.DelFg);
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@REASONKB", SqlDbType.NVarChar).Value = (object)this.ReasonKb ?? DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }

        #endregion
    }
}
