using EICS;
using EICS.Arms;
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
    /// Materials内の何れか1つは該当資材が登録されている必要がある。
    /// NASCA実績ロット特性機能
    /// </summary>
    public class RequireMaterial
    {
        public string LotCharCd1 { get; set; }
        public string LotCharCd2 { get; set; }
        public List<string> Materials { get; set; }

        public RequireMaterial() { this.Materials = new List<string>(); }
    }


    /// <summary>
    /// 製造プロファイル・製造条件関連
    /// </summary>
    public class Profile
    {
        //private static NLog.Logger log = NLog.LogManager.GetCurrentClassLogger();

        #region プロパティ

        /// <summary>
        /// プロファイル番号
        /// </summary>
        public int ProfileId { get; set; }

        /// <summary>
        /// プロファイル名
        /// </summary>
        public string ProfileNm { get; set; }

        /// <summary>
        /// アッセンブリー型番
        /// </summary>
        public string TypeCd { get; set; }

        /// <summary>
        /// ブレンドコード
        /// </summary>
        public string BlendCd { get; set; }

        /// <summary>
        /// 樹脂グループコード
        /// </summary>
        public List<string> ResinGpCd { get; set; }

        /// <summary>
        /// カットブレンド判定文字列
        /// </summary>
        public string CutBlendCd { get; set; }

        /// <summary>
        /// D/B投入日
        /// </summary>
        public string DBThrowDt { get; set; }

        /// <summary>
        /// 狙いランク
        /// </summary>
        public string AimRank { get; set; }

        /// <summary>
        /// 製造区分
        /// </summary>
        public string MnfctKb { get; set; }

        /// <summary>
        /// 指示書番号
        /// </summary>
        public string TrialNo { get; set; }

        /// <summary>
        /// 1ロット数量
        /// </summary>
        public int LotSize { get; set; }

        /// <summary>
        /// 検査工程リスト
        /// </summary>
        public int[] InspectionProcs { get; set; }

        /// <summary>
        /// 現在有効
        /// </summary>
        public bool IsCurrent { get; set; }

        /// <summary>
        /// HV試験対象
        /// </summary>
        public bool HasHvTest { get; set; }

        /// <summary>
        /// ⊿L試験対象
        /// </summary>
        public bool HasDeltaLTest { get; set; }

        /// <summary>
        /// 最終更新日
        /// </summary>
        public DateTime LastUpdDt { get; set; }

        /// <summary>
        /// 削除フラグ
        /// </summary>
        public bool DelFg { get; set; }

		/// <summary>
		/// 先行ライフ試験条件
		/// </summary>
		public string BeforeLifeTestCondCd { get; set; }

        #endregion

        /// <summary>
        /// 対象日時のプロファイルを取得
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
		//public static Profile GetCurrentProfile()
		//{
		//	Profile[] proflist = SearchProfiles(null, null, true, true);
		//	if (proflist == null || proflist.Length == 0)
		//	{
		//		return null;
		//	}
		//	else
		//	{
		//		return proflist[0];
		//	}
		//}

        /// <summary>
        /// プロファイル情報取得(削除フラグ無視）
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
		//public static Profile GetProfile(int lineCD, int profileid)
		//{
		//	return GetProfile(lineCD, profileid, null);
		//}

        /// <summary>
        /// プロファイル情報取得(削除フラグ無視）
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
		//public static Profile GetProfile(int lineCD, int? profileid, string dbThrowDt)
		//{
			//Profile[] proflist = SearchProfiles(lineCD, profileid, dbThrowDt, false, true);

			//if (proflist.Length >= 1)
			//{
			//	return proflist[0];
			//}

			//return null;
		//}



        #region SearchProfiles

        /// <summary>
        /// プロファイル検索
        /// </summary>
        /// <param name="profileid"></param>
        /// <param name="cutBlendCd"></param>
        /// <returns></returns>
		public static Profile[] SearchProfiles(int lineCD, int? profileid, string dbThrowDt, bool onlyCurrent, bool showDeleted)
		{
			List<Profile> retv = new List<Profile>();

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				//現在使用中プロファイルは削除フラグONでも表示

				cmd.CommandText = @"
                    SELECT profileid, profilenm, typecd, blendcd, resingpcd, cutblendcd, dbthrowdt, aimrank, lotsize, mnfctkb, trialno, currentfg, inspection, hvtestfg, deltaltestfg, lastupddt, beforelifetestcondcd
                    FROM TmProfile
                    WHERE
                        1=1";

				if (profileid != null)
				{
					cmd.CommandText += " AND profileid=@PROFILEID";
					cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = profileid;
				}

				if (dbThrowDt != null)
				{
					cmd.CommandText += " AND dbThrowDt=@DBTHROWDT";
					cmd.Parameters.Add("@DBTHROWDT", SqlDbType.NVarChar).Value = dbThrowDt;
				}

				if (onlyCurrent == true)
				{
					cmd.CommandText += " AND currentfg=1";
				}

				if (showDeleted == false)
				{
					cmd.CommandText += " AND delfg=0";
				}

				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					int ordResingpCd = reader.GetOrdinal("resingpcd");

					while (reader.Read())
					{
						Profile p = new Profile();
						p.ProfileId = SQLite.ParseInt(reader["profileid"]);
						p.ProfileNm = SQLite.ParseString(reader["profilenm"]);
						p.TypeCd = SQLite.ParseString(reader["typecd"]);
						p.BlendCd = SQLite.ParseString(reader["blendcd"]);

						p.ResinGpCd = new List<string>();
						if (!reader.IsDBNull(ordResingpCd))
						{
							p.ResinGpCd.AddRange(reader.GetString(ordResingpCd).Split(','));
						}

						p.CutBlendCd = SQLite.ParseString(reader["cutblendcd"]);
						p.DBThrowDt = SQLite.ParseString(reader["dbthrowdt"]);
						p.AimRank = SQLite.ParseString(reader["aimrank"]);
						p.LotSize = SQLite.ParseInt(reader["lotsize"]);
						p.MnfctKb = SQLite.ParseString(reader["mnfctkb"]);
						p.TrialNo = SQLite.ParseString(reader["trialno"]);
						p.IsCurrent = SQLite.ParseBool(reader["currentfg"]);
						p.HasHvTest = SQLite.ParseBool(reader["hvtestfg"]);
						p.HasDeltaLTest = SQLite.ParseBool(reader["deltaltestfg"]);
						p.LastUpdDt = SQLite.ParseDate(reader["lastupddt"]).Value;

						// 2015.6.16 車載高 先行ライフ試験対応
						p.BeforeLifeTestCondCd = SQLite.ParseString(reader["beforelifetestcondcd"]);

						//抜き取り検査工程
						List<int> procs = new List<int>();
						string ispProcs = SQLite.ParseString(reader["inspection"]);
						if (string.IsNullOrEmpty(ispProcs) == false)
						{
							try
							{
								string[] procstr = ispProcs.Split(',');
								foreach (string proc in procstr)
								{
									procs.Add(int.Parse(proc));
								}
							}
							catch (Exception ex)
							{
								throw new Exception("プロファイル検査工程読込異常" + ex.ToString(), ex);
							}
						}
						p.InspectionProcs = procs.ToArray();

						retv.Add(p);
					}
				}
			}

			return retv.ToArray();
		}
        #endregion

    }
}
