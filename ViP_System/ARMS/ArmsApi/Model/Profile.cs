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
    /// BOM情報 投入可能資材
    /// </summary>
    public class BOM
    {
        public string LotCharCd { get; set; }
        public string MaterialCd { get; set; }

        private string lotcharNm;
        public string LotCharName
        {
            get
            {
                if (lotcharNm == null)
                {
                    lotcharNm = tryGetLotCharName(this.LotCharCd);
                }
                return lotcharNm;
            }
        }

        private string materialNm;
        public string MaterialName
        {
            get
            {
                if (materialNm == null)
                {
                    materialNm = tryGetMaterialName(this.MaterialCd);
                }
                return materialNm;
            }
        }

        public BOM(string lotcharcd, string matcd)
        {
            this.LotCharCd = lotcharcd;
            this.MaterialCd = matcd;
        }

        #region tryGetLotCharName

        /// <summary>
        /// NASCAからロット特性コード取得
        /// NASCA接続エラー時は空白を返す
        /// </summary>
        /// <param name="lotcharcd"></param>
        /// <returns></returns>
        private string tryGetLotCharName(string lotcharcd)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = lotcharcd;
                    cmd.CommandText = "SELECT LotChar_ja FROM ntmlttk WHERE lotchar_cd = @LOTCHARCD";

                    return (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("BOMロット特性名称取得エラー:" + lotcharcd + ex.ToString());
                return string.Empty;
            }
        }
        #endregion

        private string tryGetMaterialName(string materialcd)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@MATERIALCD", SqlDbType.NVarChar).Value = materialcd;
                    cmd.CommandText = "SELECT TOP 1 materialnm FROM TnMaterials WHERE materialcd = @MATERIALCD";

                    return (cmd.ExecuteScalar() ?? string.Empty).ToString().Trim();
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("BOM原材料名称取得エラー:" + materialcd + ex.ToString());
                return string.Empty;
            }
        }
    }

    /// <summary>
    /// Materials内の何れか1つは該当資材が登録されている必要がある。
    /// NASCA実績ロット特性機能
    /// </summary>
    public class RequireMaterial
    {
        public string LotCharCd1 { get; set; }
        public string LotCharCd2 { get; set; }
		public string LotCharCd3 { get; set; }
		public string LotCharCd4 { get; set; }

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
        /// 樹脂グループコード2
        /// </summary>
        public List<string> ResinGpCd2 { get; set; }

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

        /// <summary>
        /// ダイシェア試験抜取グループ
        /// </summary>
        public string DieShearSamplingPriority { get; set; }

        /// <summary>
        /// 予定選別規格
        /// </summary>
        public string ScheduleSelectionStandard { get; set; }

        /// <summary>
        /// ラインNo(プロファイル名から取得)
        /// </summary>
        public string LineNo { get; set; }

        /// <summary>
        /// WB後自動外観検査機投入が全数のフラグ
        /// </summary>
        public bool FullInspectionFg { get; set; }

        #endregion

        #region GetBOM

        /// <summary>
        /// プロファイルに関連づいた原材料コードリストを取得
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
        public static BOM[] GetBOM(int profileid)
        {
            List<BOM> retv = new List<BOM>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT lotcharcd, materialcd
                        FROM TmBom
                        WHERE
                          profileid=@PROFILEID AND delfg=0";

                    cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = profileid;

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            string lotcharcd = SQLite.ParseString(reader["lotcharcd"]);
                            string matcd = SQLite.ParseString(reader["materialcd"]);

                            retv.Add(new BOM(lotcharcd, matcd));
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return retv.ToArray();
        }

        #endregion

        #region GetRequiredMaterials

        /// <summary>
        /// NASCA実績ロット特性機能
        /// 各工程終了時の最低必要資材を取得
        /// </summary>
        /// <param name="lot"></param>
        /// <param name="workcd"></param>
        /// <returns></returns>
        public static RequireMaterial[] GetRequiredMaterials(AsmLot lot, string workcd)
        {
            BOM[] bom = GetBOM(lot.ProfileId);

            List<RequireMaterial> retv = new List<RequireMaterial>();
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                                    SELECT lotcharcd, lotcharcd2, lotcharcd3, lotcharcd4
                                    FROM TmMatRequire with(nolock)
                                    WHERE
                                      (typecd = 0 OR typecd=@TYPECD) AND workcd=@WORKCD AND delfg=0";


                    cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.NVarChar).Value = lot.TypeCd;
                    cmd.Parameters.Add("@WORKCD", System.Data.SqlDbType.NVarChar).Value = (object)workcd ?? DBNull.Value;


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            RequireMaterial req = new RequireMaterial();
                            req.LotCharCd1 = SQLite.ParseString(reader["lotcharcd"]);
                            req.LotCharCd2 = SQLite.ParseString(reader["lotcharcd2"]);
                            req.LotCharCd3 = SQLite.ParseString(reader["lotcharcd3"]);
                            req.LotCharCd4 = SQLite.ParseString(reader["lotcharcd4"]);
                            req.Materials.AddRange(bom.Where(b => b.LotCharCd == req.LotCharCd1)
                                                .Select(b => b.MaterialCd));

                            req.Materials.AddRange(bom.Where(b => b.LotCharCd == req.LotCharCd2)
                                                .Select(b => b.MaterialCd));

                            req.Materials.AddRange(bom.Where(b => b.LotCharCd == req.LotCharCd3)
                                                .Select(b => b.MaterialCd));

                            req.Materials.AddRange(bom.Where(b => b.LotCharCd == req.LotCharCd4)
                                                .Select(b => b.MaterialCd));

                            if (req.Materials.Count > 0)
                            {
                                retv.Add(req);
                            }
                        }
                    }
                }
                return retv.ToArray();
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw new ArmsException("実績ロット特性取得エラー", ex);
            }
        }
        #endregion

        /// <summary>
        /// 対象日時のプロファイルを取得
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static Profile GetCurrentProfile()
        {
            Profile[] proflist = SearchProfiles(null, null, true, true);
            if (proflist == null || proflist.Length == 0)
            {
                return null;
            }
            else
            {
                return proflist[0];
            }
        }

        /// <summary>
        /// プロファイル情報取得(削除フラグ無視）
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
        public static Profile GetProfile(int profileid)
        {
            return GetProfile(profileid, null);
        }


        /// <summary>
        /// プロファイル情報取得(削除フラグ無視）
        /// </summary>
        /// <param name="profileid"></param>
        /// <returns></returns>
        public static Profile GetProfile(int? profileid, string dbThrowDt)
        {
            Profile[] proflist = SearchProfiles(profileid, dbThrowDt, false, true);

            if (proflist.Length >= 1)
            {
                return proflist[0];
            }

            return null;
        }

        public static Profile GetProfile(string typeCd, string dbThrowDt)
        {
            Profile[] proflist = SearchProfiles(null, dbThrowDt, false, false);

            proflist = proflist.Where(p => p.TypeCd == typeCd && p.DelFg == false).ToArray();
            if (proflist.Count() == 0)
                return null;
            else if (proflist.Count() >= 2)
                throw new ApplicationException(
                    string.Format("複数のプロファイルデータが存在します。型番:{0} D/B投入日{1}", typeCd, dbThrowDt));

            return proflist.Single();
        }

        #region SetActive

        public static void SetActive(int profileid)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = profileid;

                    cmd.CommandText = @"
                        update TmProfile SET currentfg = 0
                        WHERE profileid <> @PROFILEID";
                    cmd.ExecuteNonQuery();

                    cmd.CommandText = @"
                        update TmProfile SET currentfg = 1
                        WHERE profileid = @PROFILEID";
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error("アクティブプロファイル更新失敗" + ex.ToString());
                throw ex;
            }
        }
        #endregion

        #region GetCurrentDBProfile

        /// <summary>
        /// DBプロファイルで有効フラグのついたものを取得
        /// </summary>
        /// <param name="macno"></param>
        /// <returns></returns>
        public static Profile GetCurrentDBProfile(int macno)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;

                    //現在使用中プロファイルは削除フラグONでも表示
                    cmd.CommandText = @"
                        SELECT profileid
                        FROM TmDBProfile
                        WHERE
                           macno=@MACNO";

                    object o = cmd.ExecuteScalar();
                    if (o == null)
                    {
                        return null;
                    }

                    int profileid = Convert.ToInt32(o);
                    return Profile.GetProfile(profileid);
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }
        }

        #endregion

        #region SetCurrentDBProfile

        public static void SetCurrentDBProfile(int macno, int profileid)
        {

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macno;
                    cmd.Parameters.Add("@PROFILEID", SqlDbType.BigInt).Value = profileid;
                    cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    con.Open();

                    //現在使用中プロファイルは削除フラグONでも表示
                    cmd.CommandText = @"
                        UPDATE TmDBProfile
                        SET profileid = @PROFILEID
                        WHERE
                           macno=@MACNO";

                    cmd.ExecuteNonQuery();
                    Log.SysLog.Info("DBプロファイル更新:" + macno + ":" + profileid);
                }
                catch (Exception ex)
                {
                    throw new ArmsException(ex.ToString());
                }
            }
        }
        #endregion

        #region SearchProfiles

        /// <summary>
        /// プロファイル検索
        /// </summary>
        /// <param name="profileid"></param>
        /// <param name="cutBlendCd"></param>
        /// <returns></returns>
        public static Profile[] SearchProfiles(int? profileid, string dbThrowDt, bool onlyCurrent, bool showDeleted)
        {
            List<Profile> retv = new List<Profile>();
            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    //現在使用中プロファイルは削除フラグONでも表示

                    // 2015.6.16 車載高 先行ライフ試験対応
                    //					cmd.CommandText = @"
                    //                        SELECT profileid, profilenm, typecd, blendcd, resingpcd, cutblendcd, dbthrowdt, aimrank, lotsize, mnfctkb, trialno, currentfg, inspection, hvtestfg, deltaltestfg, lastupddt
                    //                        FROM TmProfile
                    //                        WHERE
                    //                           1=1";

                    cmd.CommandText = @"
                        SELECT profileid, profilenm, typecd, blendcd, resingpcd, resingpcd2, cutblendcd, dbthrowdt, aimrank, lotsize, mnfctkb, trialno, currentfg
                            , inspection, hvtestfg, deltaltestfg, lastupddt, beforelifetestcondcd, dieshearsamplingpriority, scheduleselectionstandard, fullinspectionfg
                        FROM TmProfile WITH(NOLOCK)
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
                        int ordResingpCd2 = reader.GetOrdinal("resingpcd2");

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

                            p.ResinGpCd2 = new List<string>();
                            if (!reader.IsDBNull(ordResingpCd2))
                            {
                                p.ResinGpCd2.AddRange(reader.GetString(ordResingpCd2).Split(','));
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
                            p.BeforeLifeTestCondCd = SQLite.ParseString(reader["beforelifetestcondcd"]);
                            p.DieShearSamplingPriority = SQLite.ParseString(reader["dieshearsamplingpriority"]);

                            // Ver1.99.0 予定選別規格 追加
                            p.ScheduleSelectionStandard = SQLite.ParseString(reader["scheduleselectionstandard"]);

                            // <!--プロファイル名からラインNoを取得
                            p.LineNo = GetLineNoFromProfileName(p.ProfileNm);
                            //  -->

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
                                    Log.SysLog.Error("プロファイル検査工程読込異常" + ex.ToString());
                                }
                            }
                            p.InspectionProcs = procs.ToArray();

                            p.FullInspectionFg = SQLite.ParseBool(reader["fullinspectionfg"]);

                            retv.Add(p);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.SysLog.Error(ex);
                throw ex;
            }

            return retv.ToArray();
        }
        #endregion

        #region UpdateProfile(未使用)

        //public static void UpdateProfile(Profile prof, BOM[] bomlist, string constr)
        //{
        //    //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
        //    using (SqlConnection con = new SqlConnection(constr))
        //    using (SqlCommand cmd = con.CreateCommand())
        //    {
        //        con.Open();

        //        cmd.Parameters.Add("@PROFID", SqlDbType.BigInt).Value = prof.ProfileId;
        //        cmd.Parameters.Add("@NAME", SqlDbType.NVarChar).Value = prof.ProfileNm;
        //        cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = prof.TypeCd;
        //        cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = prof.BlendCd;

        //        //2015.2.12 複数樹脂グループ投入対応
        //        if (prof.ResinGpCd == null || prof.ResinGpCd.Count == 0)
        //        {
        //            cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = System.DBNull.Value;
        //        }
        //        else
        //        {
        //            cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = string.Join(",", prof.ResinGpCd);
        //        }

        //        //cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = prof.ResinGpCd;

        //        cmd.Parameters.Add("@CUTBLENDCD", SqlDbType.NVarChar).Value = prof.CutBlendCd;
        //        cmd.Parameters.Add("@DBTHROWDT", SqlDbType.NVarChar).Value = prof.DBThrowDt;
        //        cmd.Parameters.Add("@LOTSIZE", SqlDbType.BigInt).Value = prof.LotSize;
        //        cmd.Parameters.Add("@AIMRANK", SqlDbType.NVarChar).Value = prof.AimRank;
        //        cmd.Parameters.Add("@MNFCTKB", SqlDbType.NVarChar).Value = prof.MnfctKb;
        //        cmd.Parameters.Add("@TRIALNO", SqlDbType.NVarChar).Value = prof.TrialNo;
        //        cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = prof.DelFg;
        //        cmd.Parameters.Add("@HVTESTFG", SqlDbType.Int).Value = prof.HasHvTest;
        //        cmd.Parameters.Add("@DELTALFG", SqlDbType.Int).Value = prof.HasDeltaLTest;
        //        cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = prof.LastUpdDt;

        //        // 2015.2.12 複数樹脂グループ投入対応
        //        cmd.Parameters.Add("@BEFORELIFETESTCONDCD", SqlDbType.NVarChar).Value = prof.BeforeLifeTestCondCd;

        //        //抜取り検査工程はカンマ区切り文字列へ変換
        //        string inspections = null;
        //        if (prof.InspectionProcs != null)
        //        {
        //            foreach (int isp in prof.InspectionProcs)
        //            {
        //                if (inspections != null) inspections += ",";
        //                inspections += isp.ToString();
        //            }
        //        }
        //        cmd.Parameters.Add("@INSPECTION", SqlDbType.NVarChar).Value = inspections ?? (object)DBNull.Value;

        //        cmd.CommandText = @"
        //                    SELECT lastupddt FROM TmProfile WHERE profileid=@PROFID";

        //        object objlastupd = cmd.ExecuteScalar();



        //        if (objlastupd == null)
        //        {
        //            // 2015.6.16 車載高 先行ライフ試験対応
        //            //					cmd.CommandText = @"
        //            //                        INSERT INTO TmProfile(profileid, profilenm, typecd, blendcd, resingpcd, cutblendcd, dbthrowdt, lotsize,aimrank, mnfctkb, trialno, inspection, delfg, lastupddt, hvtestfg, deltaltestfg)
        //            //                        VALUES (@PROFID, @NAME, @TYPECD, @BLENDCD, @RESINGPCD, @CUTBLENDCD, @DBTHROWDT, @LOTSIZE, @AIMRANK, @MNFCTKB, @TRIALNO, @INSPECTION, @DELFG, @UPDDT, @HVTESTFG, @DELTALFG);";

        //            cmd.CommandText = @"
        //                        INSERT INTO TmProfile(profileid, profilenm, typecd, blendcd, resingpcd, cutblendcd, delfg, aimrank, lotsize, mnfctkb, trialno, inspection, lastupddt, hvtestfg, deltaltestfg, dbthrowdt, beforelifetestcondcd)
        //                        VALUES (@PROFID, @NAME, @TYPECD, @BLENDCD, @RESINGPCD, @CUTBLENDCD, @DELFG, @AIMRANK, @LOTSIZE, @MNFCTKB, @TRIALNO, @INSPECTION, @UPDDT, @HVTESTFG, @DELTALFG, @DBTHROWDT, @BEFORELIFETESTCONDCD);";

        //            cmd.ExecuteNonQuery();

        //            updateBOM(prof.ProfileId, bomlist, constr);
        //            return;
        //        }
        //        else
        //        {
        //            DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
        //            if (prof.LastUpdDt > current)
        //            {
        //                cmd.CommandText = @"
        //                            UPDATE TmProfile SET profilenm=@NAME, typecd=@TYPECD, blendcd=@BLENDCD, resingpcd=@RESINGPCD
        //                             , cutblendcd=@CUTBLENDCD, dbthrowdt=@DBTHROWDT, lotsize=@LOTSIZE, delfg=@DELFG, lastupddt=@UPDDT, aimrank=@AIMRANK, mnfctkb=@MNFCTKB, trialno=@TRIALNO, inspection=@INSPECTION
        //                             , hvtestfg=@HVTESTFG, deltaltestfg=@DELTALFG
        //                            WHERE profileid=@PROFID";
        //                cmd.ExecuteNonQuery();
        //                updateBOM(prof.ProfileId, bomlist, constr);
        //            }
        //        }
        //    }
        //}

        #endregion

        #region updateBOM

        /// <summary>
        /// DELETE-INSERT直接呼ばない
        /// </summary>
        /// <param name="profileid"></param>
        /// <param name="bomlist"></param>
        private static void updateBOM(int profileid, BOM[] bomlist, string constr)
        {
            //ライン受渡しも使うため、呼び出し先全てにConstrの受け渡し必要
            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Transaction = con.BeginTransaction();

                try
                {
                    cmd.Parameters.Add("@PROFID", SqlDbType.BigInt).Value = profileid;
                    cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = 0;
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                    cmd.CommandText = @"DELETE FROM TmBOM WHERE profileid=@PROFID";
                    cmd.ExecuteNonQuery();

                    SqlParameter prmLotchar = cmd.Parameters.Add("@LOTCHARCD", SqlDbType.NVarChar);
                    SqlParameter prmMatCd = cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar);

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

		public static int GetOrderCountFromProfileName(string profileNm)
		{
			int retv = 0;

			// 頭から "【" と "：" まで進んで、そこまで切り飛ばし
			//TS7-154U-NSSW064A-010-b5/WXY-1-448007,449007 自動化ライン#02 4/30【02：78ロット】
			int startIndex = profileNm.IndexOf("【", 0);
			int colonIndex = profileNm.IndexOf("：", startIndex);
			if (colonIndex == -1)
			{
                // コロン (：) が半角文字 の対応
                colonIndex = profileNm.IndexOf(":", startIndex);
			}
            if (colonIndex == -1)
            {
                throw new ApplicationException($"プロファイル名に全角半角のコロン(：)が含まれていません。" +
                                               $"プロファイル名:{profileNm}");
            }

            string orderCountChar = string.Join("", profileNm.Skip(colonIndex + 1));

			// 頭から "ロ"以降は切り飛ばし
			//TS7-154U-NSSW064A-010-b5/WXY-1-448007,449007 自動化ライン#02 4/30【02：78ロット】
			int endIndex = orderCountChar.IndexOf("ロ", 0);
            if (endIndex == -1)
            {
                // ロットの文字 = 半角文字 の対応
                endIndex = orderCountChar.IndexOf("ﾛ", 0);
            }
            if (endIndex == -1)
            {
                throw new ApplicationException($"プロファイル名に全角半角の『ロット』が含まれていません。" +
                                               $"プロファイル名:{profileNm}");
            }
            orderCountChar = string.Join("", orderCountChar.Take(endIndex));

			// 最終"78"を取得
			//TS7-154U-NSSW064A-010-b5/WXY-1-448007,449007 自動化ライン#02 4/30【02：78ロット】
			if (int.TryParse(orderCountChar, out retv) == false)
			{
				throw new Exception(string.Format("プロファイル名の規定ロット数が数値ではありません。プロファイル名:{0}", profileNm));
			}

			return retv;
		}

        // <!--【改修1.127.0】 GetMnfctKbListFromProfile, GetLineNoFromProfileName追加
        public static string[] GetMnfctKbListFromProfile()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    SELECT DISTINCT mnfctkb FROM TmProfile ORDER BY mnfctkb";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(SQLite.ParseString(rd["mnfctkb"]));
                    }
                }
            }

            return retv.ToArray();
        }

        public static string GetLineNoFromProfileName(string profileNm)
        {
            string retv = string.Empty;

            // 頭から "【" まで進んで、そこまで切り飛ばし
            //TS7-154U-NSSW064A-010-b5/WXY-1-448007,449007 自動化ライン#02 4/30【02：78ロット】
            int startIndex = profileNm.IndexOf("【", 0);

            // "【"が無いプロファイルはLineNo = なしとする
            if (startIndex == -1) return retv;

            string lineNoChar = string.Join("", profileNm.Skip(startIndex + 1));

            // 頭から ":"以降は切り飛ばし
            //TS7-154U-NSSW064A-010-b5/WXY-1-448007,449007 自動化ライン#02 4/30【02：78ロット】
            int endIndex = lineNoChar.IndexOf(":", 0);
            if (endIndex == -1) endIndex = lineNoChar.IndexOf("：", 0);
            lineNoChar = string.Join("", lineNoChar.Take(endIndex));

            // プロファイル名が次のような場合、更に"【"以前を切り飛ばす
            // NS2W266GCRTB-70A71_P482b52e61/Zm_173502_(2018/01/15) 6号機(HB) 自動搬送ライン-1【SGAF-180014】【先行色調】【#6:10ロット】
            // lineNoChar = "SGAF-180014】【#6"
            startIndex = lineNoChar.LastIndexOf("【");
            if (startIndex != -1)
            {
                lineNoChar = string.Join("", lineNoChar.Skip(startIndex + 1));
            }

            // "【"と":"の間が空白の場合、ラインNo = "高生産性"とする
            if (string.IsNullOrWhiteSpace(lineNoChar) == true)
            {
                retv = Config.LINENAME_HIGHLINE;
            }
            else
            {
                retv = lineNoChar;
            }

            return retv;
        }
    }
}
