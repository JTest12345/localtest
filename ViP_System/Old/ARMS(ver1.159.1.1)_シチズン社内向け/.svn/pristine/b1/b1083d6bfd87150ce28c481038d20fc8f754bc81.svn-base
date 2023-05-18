using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArmsApi.Model.NASCA
{
    public class Importer
    {
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
        /// 樹脂グループコード(PB実装2)
        /// </summary>
        private const string RESINGPCD2_LOTCHARCD = "P0000264";

        /// <summary>
        /// 狙いランク
        /// </summary>
        private const string AIMRANK_LOTCHARCD = "P0000001";

        /// <summary>
        /// ライフ試験
        /// </summary>
        private const string LIFETEST_LOTCHARCD = "T0000001";

        /// <summary>
        /// 先行色調
        /// </summary>
        private const string COLORTEST_LOTCARCD = "P0000031";

        /// <summary>
        /// 吸湿保管点灯
        /// </summary>
        private const string KHLTEST_LOTCHARCD = "T0000036";

        /// <summary>
        /// フレーム識別ID
        /// </summary>
        private const string FRAMETYPE_LOTCHARCD = "M0000013";

        /// <summary>
        /// フレーム識別ID(BardMark)
        /// </summary>
        private const string FRAMETYPE_BADMARK_CHARVALCD = "23";

        public const string DB_INSPECTION_NEED = "P0000147"; //1=必要 2=不要

        public const string DB_INSPECITON_END = "T0000074"; // 1=終了 2=未検査

        public const string WB_INSPECTION_NEED = "P0000144"; //1=必要 2=不要 3=全数

        /// <summary>
        /// 研削厚み判定資材種類
        /// </summary>
        public const string GR_THICKNESS_DECISION_MATKIND_LOTCHARCD = "P0000238"; //0=基板　1=チップ


        #region GetMnfctInstNo

        public static List<string> GetMnfctInstNo(string lotno, string sectioncd)
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                cmd.Parameters.Add("@SECTIONCD", SqlDbType.Char).Value = sectioncd;
                cmd.CommandText = @"
                             SELECT
                         RTTORDH.mnfctinst_no
                         FROM   RVTORDH RTTORDH
                         INNER JOIN  RVMMAT RTMMAT ON  RTTORDH.material_cd = RTMMAT.material_cd 
                         INNER JOIN  dbo.NttSSHJ  NttSSHJ ON RTTORDH.mnfctinst_no = NttSSHJ.MnfctInst_NO
                         INNER JOIN  RVTREPAYSCH  RTTREPAYSCH  ON RTTORDH.repaysch_no = RTTREPAYSCH.repaysch_no
                         WHERE  (RTTORDH.del_fg = '0' ) 
                         AND (NttSSHJ.Del_FG = 0 ) 	
                         AND (RTTREPAYSCH.section_cd = @SECTIONCD)
                         and (NttSSHJ.Lot_no = @LOTNO)";

                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(rd[0].ToString().Trim());
                    }
                }
            }

            return retv;
        }
        #endregion

        #region SetDefectRecord

        public static void SetDefectRecord(string rslno, string lotno, int procno, Action<string> logact)
        {
            logact("不良情報" + lotno + ":" + rslno);

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@RSLNO", SqlDbType.Char).Value = rslno;

                cmd.CommandText = @"
                    select DefCause_CD, DefClass_cd, defItem_cd, defect_ct from NttSJFR
                    where mnfctrsl_no = @RSLNO";

                con.Open();

                Defect d = new Defect();
                d.LotNo = lotno;
                d.ProcNo = procno;
                d.DefItems = new List<DefItem>();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        DefItem di = new DefItem();
                        di.CauseCd = SQLite.ParseString(rd["defcause_cd"]).Trim();
                        di.ClassCd = SQLite.ParseString(rd["defclass_cd"]).Trim();
                        di.DefectCd = SQLite.ParseString(rd["defItem_cd"]).Trim();
                        di.DefectCt = (int)Convert.ToDecimal(rd["defect_ct"]);
                        d.DefItems.Add(di);
                    }
                }
                d.DeleteInsert();
            }
        }
        #endregion

        #region SetTranRecord

        public static void SetTranRecord(string mnfctinstno, string lotno, Action<string> logact)
        {
            logact("作業情報" + lotno + ":" + mnfctinstno);
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                cmd.Parameters.Add("@INSTNO", SqlDbType.Char).Value = mnfctinstno;

                cmd.CommandText = @"
                    select t.mnfctrsl_no, t.mnfctinst_no, o.work_cd, plant_cd, s.start_dt, s.complt_dt  from rvttranh t 
                    inner join rvtordwork o 
                    on t.mnfctinst_no = o.mnfctinst_no
                    and t.worklin_no = o.worklin_no
                    inner join nttsjsb s
                    on s.mnfctrsl_no = t.mnfctrsl_no
                    where t.mnfctinst_no = @INSTNO
                    and o.mnfctinst_no = @INSTNO
                    and t.del_fg = 0
                    and o.del_fg = 0
                    and s.del_fg =0
                    and s.plantlin_no = 1";

                con.Open();

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string rslno = rd["mnfctrsl_no"].ToString();
                        Order ord = new Order();
                        ord.LotNo = lotno;
                        ord.IsNascaStart = true;
                        ord.WorkStartDt = Convert.ToDateTime(rd["start_dt"]);

                        try
                        {
                            ord.WorkEndDt = Convert.ToDateTime(rd["complt_dt"]);
                        }
                        catch { }

                        ord.ProcNo = getProcNo(rd["work_cd"].ToString().Trim());
                        ord.MacNo = getmacno(rd["plant_cd"].ToString().Trim());
                        //ord.IsNascaCommentEnd = true;
                        if (ord.WorkEndDt.HasValue)
                        {
                            //ord.IsNascaDefectEnd = true;
                            ord.IsNascaEnd = true;
                        }

                        Order exists = Order.GetMagazineOrder(ord.LotNo, ord.ProcNo);
                        if (exists != null && exists.IsNascaEnd)
                        {
                            continue;
                        }

                        ord.DeleteInsert(ord.LotNo);

                        //不良情報
                        SetDefectRecord(rslno, ord.LotNo, ord.ProcNo, logact);
                    }
                }
            }
        }
        #endregion

        #region getProcNo

        private static int getProcNo(string workcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                string sql = @"
                       select procno FROM TmProcess WHERE workcd = @WORKCD";
                #endregion

                cmd.CommandText = sql;
                cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar).Value = workcd;

                con.Open();

                object ret = cmd.ExecuteScalar();

                return Convert.ToInt32(ret);
            }
        }
        #endregion

        #region GetMacNo


        private static int getmacno(string plantcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                string sql = @"
                       select macno FROM TmMachine WHERE plantcd=@PLANTCD";

                cmd.CommandText = sql;
                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantcd;

                con.Open();

                object ret = cmd.ExecuteScalar();

                int r = Convert.ToInt32(ret);
                if (r == 0)
                {
                    Console.WriteLine(plantcd);
                    r = 99999;
                }

                return r;
            }
        }
        #endregion

        #region ImportAsmLot

        /// <summary>
        /// TnLotのみTnTran、TnMagは更新なし
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="logact"></param>
        public static void ImportAsmLot(string lotno, Action<string> logact)
        {
            if (lotno.Trim() == string.Empty) return;

            logact("アッセンロット取り込み開始");
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;

                    //cmd.CommandText = @"
                    //    select nascalot_id, nascalot_no, type_cd, lastupd_dt
                    //    from nttltna with(nolock) where nascalot_no = @LOTNO
                    //    order by lastupd_dt desc option(maxdop 1)";

                    cmd.CommandText = @" SELECT dbo.NttSSHJ.Lot_NO AS nascalot_no, dbo.NttLTNA.NascaLot_ID, dbo.RvmMCONV.mtralbase_cd as type_cd
                        FROM dbo.NttSSHJ with(nolock)
                        INNER JOIN dbo.RvtORDH with(nolock) ON dbo.NttSSHJ.MnfctInst_NO = dbo.RvtORDH.mnfctinst_no 
                        INNER JOIN dbo.RvmMCONV with(nolock) ON dbo.RvtORDH.material_cd = dbo.RvmMCONV.material_cd 
                        INNER JOIN dbo.NttLTNA with(nolock) ON dbo.RvmMCONV.mtralbase_cd = dbo.NttLTNA.Type_CD 
                        AND dbo.NttSSHJ.Lot_NO = dbo.NttLTNA.NascaLot_NO
                        WHERE(dbo.RvtORDH.del_fg = '0') AND(dbo.NttSSHJ.Del_FG = 0) 
                        AND (dbo.NttSSHJ.Lot_NO = @LOTNO) AND (dbo.RvmMCONV.del_fg = '0') ";

                    con.Open();
                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            int nascalotid = Convert.ToInt32(rd["nascalot_id"]);

                            AsmLot lot = new AsmLot();

                            lot.TypeCd = rd["type_cd"].ToString().Trim();
                            lot.NascaLotNo = rd["nascalot_no"].ToString().Trim();
                            lot.TempLotNo = lot.NascaLotNo;

                            lot.ResinGpCd = new List<string>();
                            string resinGroup = GetLotCharVal(nascalotid, RESINGPCD_LOTCHARCD);
                            if (!string.IsNullOrWhiteSpace(resinGroup))
                            {
                                lot.ResinGpCd.AddRange(resinGroup.Split(','));
                            }

                            lot.ResinGpCd2 = new List<string>();
                            string resinGroup2 = GetLotCharVal(nascalotid, RESINGPCD2_LOTCHARCD);
                            if (!string.IsNullOrWhiteSpace(resinGroup2))
                            {
                                lot.ResinGpCd2.AddRange(resinGroup2.Split(','));
                            }

                            lot.CutBlendCd = GetLotCharVal(nascalotid, CUTBLENDGROUP_LOTCHARCD);
                            lot.DBThrowDT = GetLotCharVal(nascalotid, DBDATE_LOTCHARCD);
                            lot.ProfileId = GetProfileNo(lot.TypeCd, lot.DBThrowDT);
                            lot.BlendCd = GetBlendCd(lot.NascaLotNo);
                            lot.IsLifeTest = IsLifetest(nascalotid);
                            lot.IsColorTest = IsColortest(nascalotid);
                            lot.IsKHLTest = IsKHLTest(nascalotid);
                            lot.IsBadMarkFrame = IsBadMarkFrame(nascalotid);
                            lot.IsFullSizeInspection = IsFullInspection(nascalotid);

							lot.BeforeLifeTestCondCd = GetLotCharListCd(nascalotid, LotChar.BEFORELIFETESTCONDCD_LOTCHARCD);
                            lot.DieShareSamplingPriority = GetLotCharVal(nascalotid, LotChar.DIE_SHARE_SAMPLING_PRIORITY_LOTCHARCD);

                            //<--Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応 
                            Profile prof = Profile.GetProfile(lot.ProfileId);
                            if (prof != null)
                            {
                                lot.ScheduleSelectionStandard = prof.ScheduleSelectionStandard;
                            }
                            //-->Ver.1.122.2 【不具合対応】予定選別規格の複数取り込み対応 

                            lot.Update();

                            bool updated = setInspections(nascalotid, lotno);
                            if (updated)
                            {
                                lot.IsChangePointLot = true;
                                lot.Update();
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("NASCAロットデータ取得エラー:" + lotno, ex);
                }
            }
            logact("型番、ロットID取得完了");
        }

        /// <summary>
        /// TnLot+TnTran+TnMagレコード
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="lineno"></param>
        /// <param name="sectioncd"></param>
        /// <param name="logact"></param>
        public static void ImportAsmLot(string lotno, string sectioncd, Action<string> logact)
        {
            //ロット情報取り込み
            ImportAsmLot(lotno, logact);

            AsmLot lot = AsmLot.GetAsmLot(lotno);

            //研削厚み取込処理対象のサーバなら厚み対象品種データを取得 2016.12.3
            if (Config.Settings.ImportMatThickness)
            {
                string thicknessDesitionMat_Kb = GetLotCharListCd(lot.TypeCd, lot.NascaLotNo, GR_THICKNESS_DECISION_MATKIND_LOTCHARCD);

                if (!string.IsNullOrWhiteSpace(thicknessDesitionMat_Kb))
                {
                    LotChar lotChar = new LotChar();
                    lotChar.ListVal = thicknessDesitionMat_Kb;
                    lotChar.LotCharCd = GR_THICKNESS_DECISION_MATKIND_LOTCHARCD;
                    lotChar.DeleteInsert(lot.NascaLotNo, Config.Settings.LocalConnString);
                }
            }

            //高効率のみダミー実績(第一作業)を追加　⇒　限定をやめる 2018.06.06
            //やめた理由：自動搬送の場合は既に対象工程の実績があるので処理されない。
            //if (Config.GetLineType == Config.LineTypes.MEL_MAP || Config.GetLineType == Config.LineTypes.MEL_SV
            //    || Config.GetLineType == Config.LineTypes.MEL_GAM
            //    || Config.GetLineType == Config.LineTypes.MEL_19
            //    || Config.GetLineType == Config.LineTypes.MEL_MPL || Config.GetLineType == Config.LineTypes.MEL_83385
            //    || Config.GetLineType == Config.LineTypes.MEL_COB
            //    || Config.GetLineType == Config.LineTypes.MEL_SIGMA
            //    || Config.GetLineType == Config.LineTypes.MEL_NTSV || Config.GetLineType == Config.LineTypes.MEL_VOYAGER
            //    || Config.GetLineType == Config.LineTypes.MEL_KIRAMEKI || Config.GetLineType == Config.LineTypes.MEL_CSP
            //    || Config.GetLineType == Config.LineTypes.MEL_093)
            //{
                Process firstProc = Process.GetFirstProcess(lot.TypeCd);

                Order ord = new Order();
                ord.LotNo = lotno;
                ord.IsNascaStart = true;
                ord.WorkStartDt = DateTime.Now;
                ord.WorkEndDt = DateTime.Now;
                ord.ProcNo = firstProc.ProcNo;
                ord.MacNo = 0;
                ord.IsDefectEnd = true;
                ord.IsNascaEnd = true;
                ord.IsAutoImport = true;

                Order exists = Order.GetMagazineOrder(ord.LotNo, ord.ProcNo);
                if (exists == null)
                {
                    // 2015.4.18 リードフレーム成型日管理機能追加　
                    // 第一作業がフレーム搭載作業の場合、リードフレーム成型日管理の為、フレーム資材をTnMatRelationへ取り込む
                    if (firstProc.WorkCd == "DB0009" || firstProc.WorkCd == "ZFC0001" || firstProc.WorkCd == "BB0004" || firstProc.WorkCd == "FC0005")
                    {
                        Material[] matlots = Material.GetNascaFrameMaterials(ord.LotNo);
                        ord.UpdateMaterialRelation(matlots);
                    }

                    ord.DeleteInsert(ord.LotNo);
                    logact(string.Format("高効率　工程:{0}レコード作成", firstProc.ProcNo));

                }
            //}

            //自動化のみ実績を取り込む　⇒　処理をやめる 2018.06.06
            //やめた理由：昔データベースが中間PC毎に分かれていたときの処理の名残り。
            //          　別の中間PCでの実績をコピーするための機能。
            //            今はデータベースはライン間で1つなのでこの処理の必要はない。
            //if (Config.GetLineType == Config.LineTypes.NEL_SV || Config.GetLineType == Config.LineTypes.NEL_MAP
            //    || Config.GetLineType == Config.LineTypes.NEL_GAM)
            //{
            //    List<string> inst = GetMnfctInstNo(lotno, sectioncd);
            //    foreach (string no in inst)
            //    {
            //        SetTranRecord(no, lotno, logact);
            //    }
            //    logact("TnTranレコード更新");
            //}

            setMagazine(lotno, logact);
            logact("TnMag情報更新");


        }
        #endregion

        #region GetProfileNo

        public static int GetProfileNo(string typecd, string dbThrowDt)
        {
            Profile p = Profile.GetProfile(typecd, dbThrowDt);
            if (p == null)
            {
                return 0;
            }
            else
            {
                return p.ProfileId;
            }

            //Profile p = Profile.GetProfile(null, dbThrowDt);
            //if (p == null)
            //{
            //	return 0;
            //}
            //if (p.TypeCd == typecd)
            //{
            //	return p.ProfileId;
            //}
            //else
            //{
            //	return 0;
            //}

        }
        #endregion

        #region istest判定
        private static bool IsKHLTest(int lotid)
        {
            string val = GetLotCharVal(lotid, KHLTEST_LOTCHARCD);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            else if (val == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsLifetest(int lotid)
        {
            string val = GetLotCharVal(lotid, LIFETEST_LOTCHARCD);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            else if (val == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsColortest(int lotid)
        {
            string val = GetLotCharListVal(lotid, COLORTEST_LOTCARCD);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            else if (val.StartsWith("OFF"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private static bool IsBadMarkFrame(int lotid)
        {
            string val = GetLotCharListCd(lotid, FRAMETYPE_LOTCHARCD, FRAMETYPE_BADMARK_CHARVALCD);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            else if (val == "0")
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion

        private static bool IsFullInspection(int lotid)
        {
            string val = GetLotCharListCd(lotid, LotChar.WB_INSPECTION_LOTCHARCD);
            if (string.IsNullOrEmpty(val))
            {
                return false;
            }
            if (val == "3")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 状態検査登録
        /// </summary>
        /// <param name="lotid"></param>
        /// <param name="lotno"></param>
        private static bool setInspections(int lotid, string lotno)
        {
            bool retv = false;

            string val = GetLotCharListVal(lotid, DB_INSPECTION_NEED);
            if (val == "有")
            {
                Inspection isp = Inspection.GetInspection(lotno, Config.Settings.DBInspectionProcNo);
                if (isp == null)
                {
                    isp = new Inspection();
                    isp.LotNo = lotno;
                    isp.ProcNo = Config.Settings.DBInspectionProcNo;
                    isp.DeleteInsert();
                    retv = true;
                }
            }

            val = GetLotCharListVal(lotid, WB_INSPECTION_NEED);
            if (val == "有" || val == "全数(SLS2用)")
            {
                Inspection isp = Inspection.GetInspection(lotno, Config.Settings.WBInspectionProcNo);
                if (isp == null)
                {
                    isp = new Inspection();
                    isp.LotNo = lotno;
                    isp.ProcNo = Config.Settings.WBInspectionProcNo;
                    isp.DeleteInsert();
                    retv = true;
                }
            }

            return retv;
        }


        #region setMagazine

        private static void setMagazine(string lotno, Action<string> logact)
        {
            logact("マガジン情報:" + lotno);

            Magazine[] exists = Magazine.GetMagazine(null, lotno, true, null);
            if (exists.Length >= 1)
            {
                return;
            }

            AsmLot lot = AsmLot.GetAsmLot(lotno);
            int firstProcNo = Process.GetFirstProcess(lot.TypeCd).ProcNo;

            Magazine mag = new Magazine();
            mag.MagazineNo = lotno;
            mag.NascaLotNO = lotno;
            mag.NewFg = true;
            mag.NowCompProcess = firstProcNo;
            mag.Update();
        }
        #endregion

        #region GetMagList

        private static List<Magazine> GetMagList(int linecd)
        {
            List<Magazine> retv = new List<Magazine>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    cmd.Parameters.Add("@LINENO", SqlDbType.Int).Value = linecd;
                    cmd.CommandText = @"
                        select l.nascalot_id, l.nascalot_no, magazine_no, now_comp_process
                        from nttinml(NOLOCK) m inner join nttltna(NOLOCK) l
                        on m.nascalot_id = l.nascalot_id
                        where inline_cd = @LINENO
                        and new_Fg = 1
                        and del_fg = 0";

                    con.Open();

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Magazine mag = new Magazine();
                            mag.MagazineNo = rd["magazine_no"].ToString().Trim();
                            mag.NewFg = true;
                            mag.NowCompProcess = Convert.ToInt32(rd["now_comp_process"]);
                            mag.NascaLotNO = rd["nascalot_no"].ToString().Trim();
                            mag.Update();

                            retv.Add(mag);
                        }
                    }
                }
                catch (Exception ex)
                {
                    throw new ArmsException("NASCAロットデータ取得エラー:" + linecd, ex);
                }
            }

            return retv;
        }
        #endregion



        private static string GetLotCharListVal(int lotnoid, string lotchar_cd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                string sql = @"
                   select l.lotchar_val from nttltts s inner join ntmlttl l
                    on s.charval_cd  =  l.charval_cd and s.lotchar_cd = l.lotchar_cd
                      where nascalot_id = @LOTID 
                     and s.lotchar_cd = @LOTCHARCD";
                #endregion

                cmd.CommandText = sql;
                cmd.Parameters.Add("@LOTID", System.Data.SqlDbType.Int).Value = lotnoid;
                cmd.Parameters.Add("@LOTCHARCD", System.Data.SqlDbType.Char).Value = lotchar_cd;

                con.Open();

                return (cmd.ExecuteScalar() ?? "").ToString().Trim();
            }
        }

        private static string GetLotCharVal(int lotnoid, string lotchar_cd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                string sql = @"
                   select lotchar_val from nttltts s 
                    where nascalot_id = @LOTID 
                     and s.lotchar_cd = @LOTCHARCD";
                #endregion

                cmd.CommandText = sql;
                cmd.Parameters.Add("@LOTID", System.Data.SqlDbType.Int).Value = lotnoid;
                cmd.Parameters.Add("@LOTCHARCD", System.Data.SqlDbType.Char).Value = lotchar_cd;

                con.Open();

                return (cmd.ExecuteScalar() ?? "").ToString().Trim();
            }
        }

        private static string GetLotCharListCd(int lotnoid, string lotcharcd)
        {
            return GetLotCharListCd(lotnoid, lotcharcd, null);
        }
        private static string GetLotCharListCd(int lotnoid, string lotcharcd, string charvalcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT NttLTTS.CharVal_CD
                            FROM dbo.NttLTNA AS NttLTNA WITH (nolock) 
                            INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTTS.NascaLot_ID 
                            WHERE (NttLTTS.LotChar_CD = @LOTCHARCD) 
                            AND (NttLTNA.nascalot_id = @LOTID) ";

                // INNER JOIN dbo.NttLTRS AS NttLTRS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTRS.NascaLot_ID

                if (string.IsNullOrEmpty(charvalcd) == false)
                {
                    sql += " AND (NttLTTS.CharVal_CD = @CHARVALCD) ";
                    cmd.Parameters.Add("@CHARVALCD", System.Data.SqlDbType.Char).Value = charvalcd;
                }

                cmd.Parameters.Add("@LOTID", System.Data.SqlDbType.Int).Value = lotnoid;
                cmd.Parameters.Add("@LOTCHARCD", System.Data.SqlDbType.Char).Value = lotcharcd;

                sql += " OPTION(MAXDOP 1) ";

                cmd.CommandText = sql;

                return (cmd.ExecuteScalar() ?? "").ToString().Trim();
            }
        }

        /// <summary>
        /// 型番・ロット・特性コードからロット特性リスト値を取得する関数
        /// </summary>
        /// <param name="typced"></param>
        /// <param name="lotno"></param>
        /// <param name="lotcharcd"></param>
        /// <returns></returns>
        public static string GetLotCharListCd(string typecd, string lotno, string lotcharcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT NttLTTS.CharVal_CD
                            FROM dbo.NttLTNA AS NttLTNA WITH (nolock) 
                            INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTTS.NascaLot_ID 
                            WHERE (NttLTTS.LotChar_CD = @LOTCHARCD) 
                            AND (NttLTNA.NascaLot_NO = @LOTNO) AND (NttLTNA.Type_CD = @TYPECD)";

                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.NVarChar).Value = lotno;
                cmd.Parameters.Add("@LOTCHARCD", System.Data.SqlDbType.Char).Value = lotcharcd;
                cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.Char).Value = typecd;

                sql += " OPTION(MAXDOP 1) ";

                cmd.CommandText = sql;

                object tempRetv = cmd.ExecuteScalar();
                if (tempRetv == null || tempRetv == DBNull.Value)
                {
                    return string.Empty;
                }
                else
                {
                    return tempRetv.ToString().Trim();
                }
            }
        }

        public static LotChar GetLotCharVal(string lotno, string lotcharcd)
        {
            return GetLotCharVal(lotno, null, lotcharcd);
        }

        public static LotChar GetLotCharVal(string lotno, string typecd, string lotcharcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT NttLTTS.LotChar_VAL
                            FROM dbo.NttLTNA AS NttLTNA WITH (nolock) 
                            INNER JOIN dbo.NttLTTS AS NttLTTS WITH (nolock) ON NttLTNA.NascaLot_ID = NttLTTS.NascaLot_ID
                            WHERE (NttLTTS.LotChar_CD = @LOTCHARCD) 
                            AND (NttLTNA.NascaLot_NO = @LOTNO)";

                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.VarChar).Value = lotno;
                cmd.Parameters.Add("@LOTCHARCD", System.Data.SqlDbType.Char).Value = lotcharcd;

                if (string.IsNullOrWhiteSpace(typecd) == false)
                {
                    sql += @" AND (NttLTNA.Type_CD = @TYPECD) ";
                    cmd.Parameters.Add("@TYPECD", System.Data.SqlDbType.VarChar).Value = typecd;
                }

                sql += " OPTION(MAXDOP 1) ";

                cmd.CommandText = sql;

                string charval = (cmd.ExecuteScalar() ?? "").ToString().Trim();

                LotChar lc = new LotChar();
                lc.LotCharCd = lotcharcd;
                lc.LotCharVal = charval;

                return lc;
            }
        }

        /// <summary>
        /// 2015/8/19作成(永尾)
        /// 引数の特性CDがNASCA上にあれば、そのまま返す。無ければ""を返す。
        /// </summary>
        /// <param name="lotcharcd"></param>
        public static string GetLotCharCd(string lotcharcd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT LotChar_CD
                            FROM dbo.NtmLTTK WITH (nolock) 
                            WHERE (LotChar_CD = @LOTCHARCD) ";

                cmd.Parameters.Add("@LOTCHARCD", System.Data.SqlDbType.Char).Value = lotcharcd;

                sql += " OPTION(MAXDOP 1) ";

                cmd.CommandText = sql;

                return (cmd.ExecuteScalar() ?? "").ToString().Trim();
            }
        }

        /// <summary>
        /// 2015/9/15作成(永尾)
        /// NASCA上の全ロット特性CDを返す。  処理速度UP対応
        /// </summary>
        /// <param name="lotcharcd"></param>
        public static List<string> GetLotCharCdList()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT LotChar_CD
                            FROM dbo.NtmLTTK WITH (nolock)  
                            OPTION(MAXDOP 1) ";

                cmd.CommandText = sql.Replace("\r\n", "");

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        retv.Add(SQLite.ParseString(reader["LotChar_CD"]));
                    }
                }

                return retv;
            }
        }

        #region GetBlendCd

        private static string GetBlendCd(string lotno)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                #region SQL

                string sql = @"
               select distinct Blend_cd from nttsshj(NOLOCK)
                where (lot_no = @LOTNO) and (Del_FG = 0)";

                #endregion

                cmd.CommandText = sql;
                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.VarChar).Value = lotno;

                con.Open();

                return cmd.ExecuteScalar().ToString().Trim();
            }
        }
        #endregion

        /// <summary>
        /// チップの厚み情報をNASCAから取得する関数
        /// </summary>
        /// <param name="w"></param>
        public static float GetThicknessAve(string lotno)
        {
            float thicknessAve;

            using (var db = new ArmsApi.Model.DataContext.NASCADataContext(Config.Settings.NASCAConSTR))
            {
                List<float> thickneccAveList = db.NttDITK.Where(r => r.RootsLot_NO == lotno && r.Del_FG == false).Select(r => r.Thickness_AVE).ToList();

                if (thickneccAveList == null || thickneccAveList.Count != 1)
                {
                    throw new Exception(string.Format("ウェハの厚み情報取得に失敗しました。ロット：{0}\n", lotno));
                }
                else
                {
                    thicknessAve = thickneccAveList[0];
                    return thicknessAve;
                }
            }
        }
        
        public static string GetRingID(string rootsLotNo)
        {
            using (var db = new ArmsApi.Model.DataContext.NASCADataContext(Config.Settings.NASCAConSTR))
            {
                var rings = db.NttRING.Where(r => r.Del_FG == false && r.RootsLot_NO == rootsLotNo);
                if (rings.Count() == 0 || rings.Count() >= 2)
                {
                    Log.SysLog.Info($"リングと資材の紐付がないか、複数紐付がある為、リングIDを特定できません。ロット：{rootsLotNo}");
                    return null;
                }
                else
                {
                    Log.SysLog.Info(rings.Single().Ring_ID);
                    return rings.Single().Ring_ID;
                }
            }
        }

        #region GetLotTranData

        /// <summary>
        /// 指定ロットの作業実績を取得
        /// </summary>
        /// <param name="w"></param>
        public static List<NascaTranData> GetLotTranData(string lotno)
        {
            List<NascaTranData> retV = new List<NascaTranData>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT nttsshj.Lot_NO, RvtORDWORK.mnfctinst_no, RvtORDWORK.worklin_no, RvtORDWORK.work_cd, 
                                RvtTRANH.start_dt, RvtTRANH.complt_dt
                                FROM NttSSHJ WITH(NOLOCK)
                                INNER JOIN RvtORDWORK WITH(NOLOCK)
                                ON NttSSHJ.MnfctInst_NO = RvtORDWORK.mnfctinst_no
                                LEFT OUTER JOIN RvtTRANH WITH(NOLOCK)
                                ON RvtORDWORK.mnfctinst_no = RvtTRANH.mnfctinst_no
                                AND RvtORDWORK.worklin_no = RvtTRANH.worklin_no
                                AND RvtTRANH.del_fg = '0'
                                WHERE NttSSHJ.Lot_NO = @LOTNO 
                                AND NttSSHJ.Del_FG = 0 AND RvtORDWORK.del_fg = '0'
                                ORDER BY RvtORDWORK.mnfctinst_no, RvtORDWORK.worklin_no
                                OPTION(MAXDOP 1)";

                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.VarChar).Value = lotno;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    int ordLotNo = rd.GetOrdinal("Lot_NO");
                    int ordMnfctInstNo = rd.GetOrdinal("mnfctinst_no");
                    int ordWorkLinNo = rd.GetOrdinal("worklin_no");
                    int ordWorkCd = rd.GetOrdinal("work_cd");
                    int ordStartDt = rd.GetOrdinal("start_dt");
                    int ordCompltDt = rd.GetOrdinal("complt_dt");

                    while (rd.Read())
                    {
                        NascaTranData trandata = new NascaTranData();

                        trandata.LotNo = SQLite.ParseString(rd[ordLotNo]).Trim();
                        trandata.MnfctInstNo = SQLite.ParseString(rd[ordMnfctInstNo]).Trim();
                        trandata.WorkLinNo = SQLite.ParseInt(rd[ordWorkLinNo]);
                        trandata.WorkCd = SQLite.ParseString(rd[ordWorkCd]).Trim();
                        trandata.StartDt = SQLite.ParseDate(rd[ordStartDt]);
                        trandata.CompltDt = SQLite.ParseDate(rd[ordCompltDt]);

                        retV.Add(trandata);
                    }
                }

                return retV;
            }
        }

        #endregion

        #region GetTypeFromLotNo

        /// <summary>
        /// 指定ロットのタイプCDを取得
        /// </summary>
        /// <param name="w"></param>
        public static string GetTypeFromLotNo(string lotno)
        {
            List<NascaTranData> retV = new List<NascaTranData>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {

                con.Open();

                string sql = @"SELECT TOP 1 RvmMCONV.mtralbase_cd
                                FROM NttSSHJ WITH(NOLOCK)
                                INNER JOIN RvtORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no
                                INNER JOIN RvmMCONV WITH(NOLOCK) ON RvtORDH.material_cd = RvmMCONV.material_cd
                                WHERE NttSSHJ.Lot_NO = @LOTNO 
                                AND NttSSHJ.Del_FG = 0 AND RvtORDH.del_fg = '0' AND RvmMCONV.del_fg = '0'
                                OPTION(MAXDOP 1)";

                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.VarChar).Value = lotno;

                cmd.CommandText = sql;

                string typeCD = SQLite.ParseString((cmd.ExecuteScalar() ?? ""));
                if (string.IsNullOrWhiteSpace(typeCD) == true)
                {
                    return null;
                }
                else
                {
                    return typeCD.Trim();
                }
            }
        }

        /// <summary>
        /// 指定ロットのタイプCDを取得
        /// </summary>
        /// <param name="w"></param>
        public static string GetMaterialCdFromLotNo(string lotno)
        {
            List<NascaTranData> retV = new List<NascaTranData>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {

                con.Open();

                string sql = @"SELECT TOP 1 RvtORDH.material_cd
                                FROM NttSSHJ WITH(NOLOCK)
                                INNER JOIN RvtORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no
                                WHERE NttSSHJ.Lot_NO = @LOTNO 
                                AND NttSSHJ.Del_FG = 0 AND RvtORDH.del_fg = '0'
                                ORDER BY NttSSHJ.mnfctinst_no
                                OPTION(MAXDOP 1)";

                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.VarChar).Value = lotno;

                cmd.CommandText = sql;

                string materialCd = SQLite.ParseString((cmd.ExecuteScalar() ?? ""));
                if (string.IsNullOrWhiteSpace(materialCd) == true)
                {
                    return null;
                }
                else
                {
                    return materialCd.Trim();
                }
            }
        }

        #endregion

        public static SortedList<int, string> GetMnfctKb()
        {
            SortedList<int, string> retv = new SortedList<int, string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT general_cd, general_ja FROM RvmGENPARTD WITH(NOLOCK) WHERE general_kb = 31 OPTION(MAXDOP 1)";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        int code = Convert.ToInt32(rd["general_cd"]);
                        string ja = rd["general_ja"].ToString().Trim();
                        retv.Add(code, ja);
                    }
                }
            }

            return retv;
        }
    }
    public class NascaTranData
    {
        public string LotNo { get; set; }
        public string MnfctInstNo { get; set; }
        public int WorkLinNo { get; set; }
        public string WorkCd { get; set; }
        public DateTime? StartDt { get; set; }
        public DateTime? CompltDt { get; set; }
    }
}
