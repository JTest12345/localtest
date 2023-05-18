using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class Resin
    {
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType()) return false;
            Resin mat = (Resin)obj;
            if (this.MixResultId == mat.MixResultId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <summary>
        /// 調合結果ID
        /// </summary>
        public string MixResultId { get; set; }

        /// <summary>
        /// 樹脂グループ
        /// </summary>
        public string ResinGroupCd { get; set; }

        /// <summary>
        /// 装置投入日
        /// </summary>
        public DateTime? InputDt { get; set; }

        /// <summary>
        /// 装置取り外し日時
        /// </summary>
        public DateTime? RemoveDt { get; set; }

        /// <summary>
        /// 保管場所CD
        /// </summary>
        public string BinCD { get; set; }

        /// <summary>
        /// 装置取り外し済みフラグ
        /// </summary>
        public bool isRemoved
        {
            get
            {
                return RemoveDt.HasValue == true ? true : false;
            }
        }

        /// <summary>
        /// 使用期限
        /// </summary>
        public DateTime LimitDt { get; set; }

        /// <summary>
        /// NASCA割付開始処理済み
        /// </summary>
        public bool IsNascaStart { get; set; }

        /// <summary>
        /// NASCA取外処理済み
        /// </summary>
        public bool IsNascaEnd { get; set; }

		/// <summary>
		/// 調合タイプ
		/// </summary>
		public string MixTypeCd { get; set; }

        /// <summary>
        /// 脱泡後有効期限
        /// </summary>
        public DateTime? StirringLimitDt { get; set; }

        public static Resin GetResin(int mixresultId)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT mixresultid, resingroupcd, uselimit, bincd, mixtypecd, stirringlimitdt
                        FROM TnResinMix WITH(NOLOCK)
                        WHERE 
                          mixresultid = @MIXRESULTID";

                    cmd.Parameters.Add("@MIXRESULTID", SqlDbType.BigInt).Value = mixresultId;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            Resin r = new Resin();
                            r.MixResultId = SQLite.ParseString(reader["mixresultid"]);
                            r.ResinGroupCd = SQLite.ParseString(reader["resingroupcd"]);
                            r.BinCD = SQLite.ParseString(reader["bincd"]);
                            r.LimitDt = SQLite.ParseDate(reader["uselimit"]) ?? DateTime.MaxValue.AddYears(-2);
							r.MixTypeCd = reader["mixtypecd"].ToString().Trim();
                            r.StirringLimitDt = SQLite.ParseDate(reader["stirringlimitdt"]);
                            return r;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                //LogManager.GetCurrentClassLogger().Error(ex);
                throw ex;
            }

            return null;
        }

        /// <summary>
        /// 使用中の樹脂グループの内、指定調合タイプと一致するモノを返す。
        /// </summary>
        public static List<string> GetResinGroupCdList(List<string> resingroupcdlist, string mixtypecd)
        {
            List<string> retV = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT DISTINCT ResinGroup_CD
                        FROM JtmMIXRATE WITH(NOLOCK)
                        WHERE Del_FG = 0 AND MixType_CD = @MIXTYPECD  
                        AND UseMode_CD = '3' ";  //UseMode_CD= 3 使用中

                    cmd.Parameters.Add("@MIXTYPECD", SqlDbType.Char).Value = mixtypecd;

                    cmd.CommandText += $" AND ResinGroup_CD IN ('{string.Join("','", resingroupcdlist)}') ";

                    cmd.CommandText += " OPTION(MAXDOP 1) ";

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string resinGroupCd = SQLite.ParseString(reader["ResinGroup_CD"]);
                            retV.Add(resinGroupCd.Trim());
                        }
                    }
                }
                return retV;
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex.ToString());
                throw ex;
            }
        }

        /// <summary>
        /// 調合タイプ名を取得
        /// </summary>
        public static string GetMixTypeNm(string mixtypecd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    con.Open();
                    cmd.CommandText = @"
                        SELECT General_NM
                        FROM JtmGENPARTD WITH(NOLOCK)
                        WHERE Del_FG = 0 AND General_KB = 1
                        AND General_CD = @MIXTYPECD
                        OPTION(MAXDOP 1) ";

                    cmd.Parameters.Add("@MIXTYPECD", SqlDbType.Char).Value = mixtypecd;

                    object generalNm = cmd.ExecuteScalar();
                    if(generalNm == null)
                    {
                        return null;
                    }
                    else
                    {
                        return Convert.ToString(generalNm);
                    }
                }
                catch (Exception ex)
                {
                    Log.SysLog.Error(ex.ToString());
                    throw ex;
                }
            }
        }
    }
}
