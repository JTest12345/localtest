using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;
using System.Text.RegularExpressions;

namespace ArmsNascaBridge
{
    public class Profiles
    {
        #region 定数
        
        /// <summary>
        /// 品目
        /// </summary>
        private const int MATCD_ROW = 2;

        /// <summary>
        /// 1ロット数量
        /// </summary>
        private const int LOT_SIZE_ROW = 9;

        /// <summary>
        /// ブレンドコード
        /// </summary>
        private const int BLENDCD_ROW = 11;

        /// <summary>
        /// 量産区分
        /// </summary>
        private const int MNFCTKB_ROW = 10;

        /// <summary>
        /// 指示書番号
        /// </summary>
        private const int TRIALNO_ROW = 12;

        /// <summary>
        /// ロット特性
        /// </summary>
        private const int LOTCHAR_ROW = 502;

        /// <summary>
        /// ダイボンド投入日ロット特性
        /// </summary>
        private const string DBDATE_LOTCHARCD = "P0000110";

        /// <summary>
        /// カットブレンドグループロット特性
        /// </summary>
        private const string CUTBLENDGROUP_LOTCHARCD = "P0000191";

        /// <summary>
        /// 樹脂グループ
        /// </summary>
        private const string RESINGPCD_LOTCHARCD = "P0000007";

        /// <summary>
        /// 樹脂グループ
        /// </summary>
        private const string RESINGPCD2_LOTCHARCD = "P0000264";

        /// <summary>
        /// 狙いランク
        /// </summary>
        private const string AIMRANK_LOTCHARCD = "P0000001";

        /// <summary>
        /// 量産試作指示書番号
        /// </summary>
        private const string TRIALNO_LOTCHARCD = "P0000025";

        /// <summary>
        /// WB後外観検査(,1の部分で有無の判定）
        /// </summary>
        private const string WB_INSPECTION_LOTCHARCD = "P0000144,1";

        /// <summary>
        /// WB後自動外観検査機投入の有無 全数(SLS2用)(,3の部分で有無の判定）
        /// </summary>
        private const string WB_INSPECTION_ALL_LOTCHARCD = "P0000144,3";

        #endregion

        public class BOM
        {
            public string LotCharCd { get; set; }
            public string MaterialCd { get; set; }
        }

        public int ProfileId { get; set; }
        public string Name { get; set; }
        public string TypeCd { get; set; }
        public int LotSize { get; set; }
        public string BlendCd { get; set; }
        public List<string> ResinGpCd { get; set; }
        public List<string> ResinGpCd2 { get; set; }
        public string CutBlendCd { get; set; }
        public string DBThrowDt { get; set; }
        public string AimRank { get; set; }
        public string MnfctKB { get; set; }
        public string TrialNo { get; set; }
        public bool HvTestFg { get; set; }
        public bool DeltaLtestFg { get; set; }
        public string Inspection { get; set; }
        public bool FullInspectionFg { get; set; }
        public DateTime LastUpdDt { get; set; }
        public int delfg { get; set; }
        public BOM[] BomList { get; set; }
        public string DieShearSamplingPriority { get; set; }

		/// <summary>先行ライフ試験条件特性値</summary>
		public string BeforeLifeTestCondCd { get; set; }

        /// <summary>予定選別規格</summary>
        public string ScheduleSelectionStandard { get; set; }

        private class PRFD
        {
            public int LineNo { get; set; }
            public string Item { get; set; }
            public bool DelFg { get; set; }
        }

        #region getPRFD
        
        /// <summary>
        /// プロファイル詳細取得
        /// </summary>
        /// <param name="profileId"></param>
        /// <returns></returns>
        private static PRFD[] getPRFD(int profileId)
        {
            List<PRFD> retv = new List<PRFD>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@PROFILEID", SqlDbType.Int).Value = profileId;
                cmd.CommandText = "SELECT item_no,item_val, del_fg FROM NttPRFD WHERE profile_no = @PROFILEID";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        PRFD pd = new PRFD();
                        pd.LineNo = Convert.ToInt32(rd["item_no"]);
                        pd.Item = rd["item_val"].ToString().Trim();
                        pd.DelFg = SQLite.ParseBool(rd["del_fg"]);

                        retv.Add(pd);
                    }
                }
            }

            return retv.ToArray();
        }
        #endregion

        #region getProfile

        private static Profiles getProfile(int profileId, List<string> LotCharCdList, SortedList<int, string> MnfctKbList)
        {
            Profiles retv = new Profiles();
            retv.ProfileId = profileId;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@PROFILEID", SqlDbType.Int).Value = profileId;

                cmd.CommandText = "SELECT profile_ja, Explain_nm, lastupd_dt, del_fg FROM NttPRFL WHERE profile_no=@PROFILEID";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        retv.Name = rd["profile_ja"].ToString().Trim() + " " + rd["Explain_nm"].ToString().Trim();
                        retv.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);
                        retv.delfg = Convert.ToInt32(rd["del_fg"]);
                    }
                }
            }

            PRFD[] prfdList = getPRFD(profileId);
            try
            {
                string matcd = prfdList.Where(p => p.LineNo == MATCD_ROW).First().Item;
				if (matcd.EndsWith(Config.Settings.MaterialCodeSurfix.Replace("%", "").Replace("_", "")) == false)
				{
					return null;
				}

                retv.TypeCd = matcd.Split('.').First();
            }
            catch (Exception)
            {
                return null;
            }
           
            retv.LotSize = int.Parse(prfdList.Where(p => p.LineNo == LOT_SIZE_ROW).First().Item);
            retv.BlendCd = prfdList.Where(p => p.LineNo == BLENDCD_ROW).First().Item;
            retv.CutBlendCd = getCutBlendGroup(prfdList);
            retv.DBThrowDt = getDBDate(prfdList);
            retv.ResinGpCd = GetResinGpCd(prfdList);
            retv.ResinGpCd2 = GetResinGpCd2(prfdList);
            retv.AimRank = GetAimRank(prfdList);
            retv.BomList = getBOM(prfdList, LotCharCdList);
            retv.TrialNo = GetTrialNo(prfdList);
            retv.Inspection = GetInspection(prfdList);
            retv.FullInspectionFg = GetFullInspection(prfdList);
            retv.DeltaLtestFg = GetDeltaLTestFg(prfdList);
            retv.HvTestFg = GetHvTestFg(prfdList);
			retv.BeforeLifeTestCondCd = GetBeforeLifeTestCondCd(prfdList);
            retv.DieShearSamplingPriority = GetDieShearSamplingPriority(prfdList);
            //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応
            // Ver1.99.0 予定選別規格 追加
            List<string> ScheduleSelectionStandardList = GetScheduleSelectionStandard(prfdList);
            retv.ScheduleSelectionStandard = string.Join(",", ScheduleSelectionStandardList);
            //-->Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応

            // <-- Ver.1.133.0 プロファイルの製造区分の取得方法を変更 (NASCAの設定内容をそのままにする)
            retv.MnfctKB = "";
            string kb = prfdList.Where(p => p.LineNo == MNFCTKB_ROW).First().Item;
            int iKb;
            if (int.TryParse(kb, out iKb) == true)
            {
                string mnfctKb;
                if (MnfctKbList.TryGetValue(iKb, out mnfctKb) == true)
                {
                    retv.MnfctKB = mnfctKb;
                }
            }
            // --> Ver.1.133.0 プロファイルの製造区分の取得方法を変更 (NASCAの設定内容をそのままにする)

            return retv;

        }
        #endregion

        #region GetTrialNo
        
        private static string GetTrialNo(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(TRIALNO_LOTCHARCD))
                {
                    return pd.Item.Split(',').Last();
                }
            }

            return string.Empty;
        }
        #endregion

        #region GetHvTestFg
        
        private static bool GetHvTestFg(PRFD[] prfdList)
        {
            string cd = Config.HV_TEST_CT_LOTCHAR_CD;

            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(cd))
                {
                    return true;
                }
            }

            return false;
        }

        #endregion

        #region GetDeltaLTestFg
                
        private static bool GetDeltaLTestFg(PRFD[] prfdList)
        {
            string cd = Config.DELTA_L_TEST_CT_LOTCHAR_CD;

            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(cd))
                {
                    return true;
                }
            }

            return false;
        }
        #endregion

        #region GetAimRank

        private static string GetAimRank(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(AIMRANK_LOTCHARCD))
                {
                    string cd = pd.Item.Split(',')[1];

                    using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.Parameters.Add("@CD", SqlDbType.Char).Value = AIMRANK_LOTCHARCD;
                        cmd.Parameters.Add("@LSTCD", SqlDbType.Char).Value = cd;
                        cmd.CommandText = "SELECT lotchar_val FROM NtmLTTL WHERE lotchar_cd = @CD AND charval_cd =@LSTCD";

                        return (cmd.ExecuteScalar() ?? "").ToString().Trim();
                    }
                }
            }

            return string.Empty;
        }
        #endregion

        #region GetResinGpCd
        
        private static List<string> GetResinGpCd(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {

				if (pd.Item.StartsWith(RESINGPCD_LOTCHARCD))
				{
                    //※フィールド値の定義が変更されたら改修が必要
                    //想定:P0000007,,,0,00,123456(樹脂グループ1つめ),654321(樹脂グループ2つめ)
                    return pd.Item.Split(',').Skip(5).Select(r => r.Trim()).ToList();
				}
			}

            return null;
        }

        private static List<string> GetResinGpCd2(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(RESINGPCD2_LOTCHARCD))
                {
                    //※フィールド値の定義が変更されたら改修が必要
                    //想定:P0000007,,,0,00,123456(樹脂グループ1つめ),654321(樹脂グループ2つめ)
                    return pd.Item.Split(',').Skip(5).Select(r => r.Trim()).ToList();
                }
            }

            return null;
        }
        #endregion

        #region GetInspection

        private static string GetInspection(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(WB_INSPECTION_LOTCHARCD) ||
                    pd.Item.StartsWith(WB_INSPECTION_ALL_LOTCHARCD))
                {
                    return Config.Settings.WBInspectionProcNo.ToString();
                }
            }
            return string.Empty;
        }

        #endregion

        #region GetFullInspection

        private static bool GetFullInspection(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(WB_INSPECTION_ALL_LOTCHARCD))
                {
                    return true;
                }
            }
            return false;
        }

        #endregion

        #region getBOM

        /// <summary>
        /// プロファイル内容から原材料情報抽出
        /// </summary>
        /// <param name="prfdList"></param>
        /// <returns></returns>
        private static BOM[] getBOM(PRFD[] prfdList, List<string> LotCharCdList)
        {
            List<BOM> retv = new List<BOM>();

            foreach (PRFD pd in prfdList)
            {
                string[] item = pd.Item.Split(',');

                if (item.Length >= 5)
                {
                    if (string.IsNullOrEmpty(item[2]) == false)
                    {
                        //// 2015/09/15 永尾追加。
                        //// Import関数の最初にリストを取得する。
                        if (LotCharCdList.Exists(l => l == item[0]))
                        {
                            BOM b = new BOM();
                            b.LotCharCd = item[0];
                            b.MaterialCd = item[2];

                            //重複データは無視
                            if (retv.Any(r => r.LotCharCd == b.LotCharCd && r.MaterialCd == b.MaterialCd)) continue;

                            retv.Add(b);
                        }
                    }
                }
            }

            return retv.ToArray();
        }
        #endregion

        #region getDBDate
        
        private static string getDBDate(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(DBDATE_LOTCHARCD))
                {
                    return pd.Item.Split(',').Last();
                }
            }

            return string.Empty;
        }
        #endregion

        #region getCutBlendGroup

        private static string getCutBlendGroup(PRFD[] prfdList) 
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(CUTBLENDGROUP_LOTCHARCD))
                {
                    return pd.Item.Split(',').Last();
                }
            }

            return string.Empty;
        }
        #endregion

		#region getBeforeLifeTestCondCd

		private static string GetBeforeLifeTestCondCd(PRFD[] prfdList)
		{
			foreach (PRFD pd in prfdList)
			{
				if (pd.Item.StartsWith(ArmsApi.Model.LotChar.BEFORELIFETESTCONDCD_LOTCHARCD))
				{
					return pd.Item.Split(',')[1];
				}
			}

			return string.Empty;
		}

		#endregion

        #region GetDieShearSamplingPriority

        private static string GetDieShearSamplingPriority(PRFD[] prfdList)
        {
            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(ArmsApi.Model.LotChar.DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD))
                {
                    return pd.Item.Split(',').Last();
                }
            }

            return string.Empty;
        }

        #endregion

        #region GetScheduleSelectionStandard

        //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応
        //private static string GetScheduleSelectionStandard(PRFD[] prfdList)
        private static List<string> GetScheduleSelectionStandard(PRFD[] prfdList)
        {
            List<string> retv = new List<string>();

            foreach (PRFD pd in prfdList)
            {
                if (pd.Item.StartsWith(ArmsApi.Model.LotChar.SCHEDULE_SELECTION_STANDARD))
                {
                    retv.Add(pd.Item.Split(',').Last());
                }
            }

            return retv;
        }
        //-->Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応

        #endregion

        public static void Import()
        {
            try
            {
                List<string> LotCharCdList = ArmsApi.Model.NASCA.Importer.GetLotCharCdList();
                SortedList<int, string> MnfctKbList = ArmsApi.Model.NASCA.Importer.GetMnfctKb();

                //取り込むラインをループ
                foreach (string lineSuffix in Config.Settings.NascaPRFDItemValLike)
                {
                    int[] idlist = GetProfileIds(lineSuffix);
                    
                    foreach (int id in idlist)
                    {
						try
						{
                            Profiles prof = getProfile(id, LotCharCdList, MnfctKbList);
							if (prof != null)
							{
								updateProfile(prof);
							}
						}
						catch (Exception err) 
						{
							Log.SysLog.Error("[ArmsNascaBridge] Profiles Error:" + err.ToString());
							throw err;
						}
                    }
                }
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge] Profiles Error:" + err.ToString());
			}
        }

        #region ProfileのIDリスト取得
        
        private static int[] GetProfileIds(string lineSuffix) 
        {
            List<int> retv = new List<int>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@ITEMLINENO", SqlDbType.Int).Value = Config.Settings.NascaPRFDItemLineNo;
                cmd.Parameters.Add("@ITEMVAL", SqlDbType.NVarChar).Value = lineSuffix + "%";
                cmd.Parameters.Add("@MATCDLINENO", SqlDbType.Int).Value = MATCD_ROW;
                //cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar).Value = "%" + Config.Settings.MaterialCodeSurfix;
                cmd.Parameters.Add("@SECTIONCD", SqlDbType.Int).Value = int.Parse(Config.Settings.SectionCd);
				cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = Config.Settings.NascaLineGroupCd;
                cmd.Parameters.Add("@FROM", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-14);

                cmd.CommandText = @"
                    SELECT
                     distinct profile_no
                    FROM NttPRFD d (NOLOCK)
                    WHERE item_no=501
                    AND itemlin_no=@ITEMLINENO
                    AND item_val LIKE @ITEMVAL
                    AND lastupd_dt >= @FROM
                    AND EXISTS (
	                    SELECT
	                     *
	                    FROM NttPRFD s (NOLOCK)
						inner join ROOTSDB.dbo.RtmMCONV m (NOLOCK) ON s.Item_VAL = m.material_cd 
						inner join ROOTSDB.dbo.RTMNFORMGROUP fg (NOLOCK) ON m.workcond_cd = fg.fcode
	                    WHERE s.profile_no = d.profile_no
						and m.del_fg = '0' and fg.del_fg = '0'
						and s.item_no = @MATCDLINENO
	                    and fg.fgroupclass_cd = @FGROUPCLSCD)
                    AND EXISTS (
   	                    SELECT * FROM NttPRFL l (NOLOCK)
                        INNER JOIN NtmSCKN s (NOLOCK) ON s.reference_cd = l.reference_cd
	                    where l.profile_no  = d.profile_No
	                    and s.section_cd = @SECTIONCD
	                 )";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(Convert.ToInt32(rd[0]));
                    }
                }
            }

            return retv.ToArray();

        }
        #endregion

        public static void updateProfile(Profiles prof)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@PROFID", SqlDbType.BigInt).Value = prof.ProfileId;
                cmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = prof.Name;
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = prof.TypeCd;
                cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = prof.BlendCd;
				if (prof.ResinGpCd == null || prof.ResinGpCd.Count == 0)
				{
					cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = System.DBNull.Value;
				}
				else
				{
					cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = string.Join(",", prof.ResinGpCd);
				}

                if (prof.ResinGpCd2 == null || prof.ResinGpCd2.Count == 0)
                {
                    cmd.Parameters.Add("@RESINGPCD2", SqlDbType.NVarChar).Value = System.DBNull.Value;
                }
                else
                {
                    cmd.Parameters.Add("@RESINGPCD2", SqlDbType.NVarChar).Value = string.Join(",", prof.ResinGpCd2);
                }

                cmd.Parameters.Add("@CUTBLENDCD", SqlDbType.NVarChar).Value = prof.CutBlendCd;
                cmd.Parameters.Add("@DBTHROWDT", SqlDbType.NVarChar).Value = prof.DBThrowDt;
                cmd.Parameters.Add("@LOTSIZE", SqlDbType.BigInt).Value = prof.LotSize;
                cmd.Parameters.Add("@AIMRANK", SqlDbType.NVarChar).Value = prof.AimRank;
                cmd.Parameters.Add("@MNFCTKB", SqlDbType.NVarChar).Value = prof.MnfctKB;
                cmd.Parameters.Add("@TRIALNO", SqlDbType.NVarChar).Value = prof.TrialNo;
                cmd.Parameters.Add("@INSPECTION", SqlDbType.NVarChar).Value = prof.Inspection;
                cmd.Parameters.Add("@FULLINSPECTIONFG", SqlDbType.Int).Value = prof.FullInspectionFg;
                cmd.Parameters.Add("@HVTESTFG", SqlDbType.Int).Value = prof.HvTestFg;
                cmd.Parameters.Add("@DELTALFG", SqlDbType.Int).Value = prof.DeltaLtestFg;
                cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = prof.delfg;
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = prof.LastUpdDt;

				cmd.Parameters.Add("@BEFORELIFETESTCONDCD", SqlDbType.NVarChar).Value = prof.BeforeLifeTestCondCd;
                cmd.Parameters.Add("@DIESHARESAMPLINGPRIORITY", SqlDbType.NVarChar).Value = prof.DieShearSamplingPriority;
                cmd.Parameters.Add("@SCHEDULESELECTIONSTANDARD", SqlDbType.NVarChar).Value = prof.ScheduleSelectionStandard;

                cmd.CommandText = @"
                    SELECT lastupddt FROM TmProfile WHERE profileid=@PROFID";

                object objlastupd = cmd.ExecuteScalar();

                if (objlastupd == null)
                {
					cmd.CommandText = @"
                        INSERT INTO TmProfile(profileid, profilenm, typecd, blendcd, resingpcd, resingpcd2, cutblendcd, dbthrowdt, lotsize,aimrank, mnfctkb, trialno, inspection, hvtestfg, deltaltestfg, delfg, lastupddt, beforelifetestcondcd, dieshearsamplingpriority, scheduleselectionstandard, fullinspectionfg)
                        VALUES (@PROFID, @NAME, @TYPECD, @BLENDCD, @RESINGPCD, @RESINGPCD2, @CUTBLENDCD, @DBTHROWDT, @LOTSIZE, @AIMRANK, @MNFCTKB, @TRIALNO, @INSPECTION, @HVTESTFG, @DELTALFG, @DELFG, @UPDDT, @BEFORELIFETESTCONDCD, @DIESHARESAMPLINGPRIORITY, @SCHEDULESELECTIONSTANDARD, @FULLINSPECTIONFG);";

                    cmd.ExecuteNonQuery();

                    updateBOM(prof.ProfileId, prof.BomList);
                    return;
                }
                else
                {
                    DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                    if (prof.LastUpdDt > current)
                    {
						cmd.CommandText = @"
                            UPDATE TmProfile SET profilenm=@NAME, typecd=@TYPECD, blendcd=@BLENDCD, resingpcd=@RESINGPCD, resingpcd2=@RESINGPCD2
                             , cutblendcd=@CUTBLENDCD, dbthrowdt=@DBTHROWDT, lotsize=@LOTSIZE, delfg=@DELFG, lastupddt=@UPDDT, aimrank=@AIMRANK, mnfctkb=@MNFCTKB, trialno=@TRIALNO, inspection=@INSPECTION
                             , hvtestfg=@HVTESTFG, deltaltestfg=@DELTALFG, beforelifetestcondcd=@BEFORELIFETESTCONDCD, dieshearsamplingpriority=@DIESHARESAMPLINGPRIORITY, scheduleselectionstandard=@SCHEDULESELECTIONSTANDARD, fullinspectionfg=@FULLINSPECTIONFG
                            WHERE profileid=@PROFID";

                        cmd.ExecuteNonQuery();
                        updateBOM(prof.ProfileId, prof.BomList);
                    }
                }
            }
        }

        #region updateBOM
        
        /// <summary>
        /// DELETE-INSERT直接呼ばない
        /// </summary>
        /// <param name="profileid"></param>
        /// <param name="bomlist"></param>
        private static void updateBOM(int profileid, BOM[] bomlist)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Transaction = con.BeginTransaction();

                try
                {
                    cmd.Parameters.Add("@PROFID", SqlDbType.BigInt).Value = profileid;
                    SqlParameter prmLotchar = cmd.Parameters.Add("@LOTCHARCD", SqlDbType.NVarChar);
                    SqlParameter prmMatCd = cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    prmLotchar.Value = "";
                    prmMatCd.Value = "";

                    cmd.CommandText = @"DELETE FROM TmBOM WHERE profileid=@PROFID";
                    cmd.ExecuteNonQuery();

                    foreach (BOM b in bomlist)
                    {

                        prmLotchar.Value = b.LotCharCd;
                        prmMatCd.Value = b.MaterialCd;

                        cmd.CommandText = @"
                        INSERT INTO TmBOM(profileid, lotcharcd, materialcd, delfg, lastupddt)
                        VALUES (@PROFID, @LOTCHARCD, @MATCD,@DELFG, @UPDDT);";
                        cmd.ExecuteNonQuery();
                    }


                    cmd.Transaction.Commit();
                }
                catch
                {
                    cmd.Transaction.Rollback();
                    throw;
                }
            }

        }
        #endregion                
    }
}
