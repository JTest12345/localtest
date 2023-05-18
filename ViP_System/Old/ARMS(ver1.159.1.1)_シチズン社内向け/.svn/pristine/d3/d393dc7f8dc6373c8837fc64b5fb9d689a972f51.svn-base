using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;

namespace ArmsNascaBridge2
{
    public class Defect
    {
        public static bool Import(int? targetPastHours)
        {
			try
			{
				DateTime start = DateTime.Now;
				string[] types = getTypes();
				foreach (string type in types)
				{
					FRMS[] nascaList = getFRMS(type, targetPastHours);
                    updateFRMS(nascaList);

                    if (targetPastHours.HasValue)

                        // NASCA対象データの時間指定が有る場合、以下の物理削除処理はしない
                        // 手動実行では物理削除されないので物理削除したい場合はバッチモードで実行する
                        continue;

					FRMS[] armsList = getData(type);
					deleteFRMS(armsList, nascaList);	
				}

                return true;
			}
			catch(Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] Defect Error:" + err.ToString());
                return false;
			}
        }

        public static bool Import()
        {
            return Import(null);
        }

        private static void updateFRMS(FRMS[] mslist)
        {
            if (mslist.Count() == 0) return;
            
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pMaterialCd = cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar);
                SqlParameter pWorkCd = cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar);
                SqlParameter pDefLinNo = cmd.Parameters.Add("@DEFLINNO", SqlDbType.Int);
                SqlParameter pItemCd = cmd.Parameters.Add("@ITEMCD", SqlDbType.NVarChar);
                SqlParameter pItemNm = cmd.Parameters.Add("@ITEMNM", SqlDbType.NVarChar);
                SqlParameter pCauseCd = cmd.Parameters.Add("@CAUSECD", SqlDbType.NVarChar);
                SqlParameter pCauseNm = cmd.Parameters.Add("@CAUSENM", SqlDbType.NVarChar);
                SqlParameter pClassCd = cmd.Parameters.Add("@CLASSCD", SqlDbType.NVarChar);
                SqlParameter pClassNm = cmd.Parameters.Add("@CLASSNM", SqlDbType.NVarChar);
                SqlParameter pItemNo = cmd.Parameters.Add("@ITEMNO", SqlDbType.BigInt);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                con.Open();

                foreach (FRMS ms in mslist)
                {
                    pType.Value = ms.TypeCd;
                    pMaterialCd.Value = ms.MaterialCd;
                    pWorkCd.Value = ms.WorkCd;
                    pDefLinNo.Value = ms.DefLinNo;
                    pItemCd.Value = ms.ItemCd;
                    pItemNm.Value = ms.ItemNm;
                    pCauseCd.Value = ms.CauseCd;
                    pCauseNm.Value = ms.CauseNm;
                    pClassCd.Value = ms.ClassCd;
                    pClassNm.Value = ms.ClassNm;
                    pItemNo.Value = ms.ItemNo;
                    pDelFg.Value = SQLite.SerializeBool(ms.DelFg);
                    pLastUpdDt.Value = ms.LastUpdDt;

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TmDefect
                            WHERE materialcd=@MATCD AND workcd=@WORKCD AND deflinno=@DEFLINNO";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd == null)
                    {
						if (ms.DelFg) continue;

                        cmd.CommandText = @"
                                INSERT INTO TmDefect(typecd, workcd, deflinno, materialcd, itemcd, causecd, classcd, itemnm, causenm, classnm, itemno, delfg, lastupddt)
                                VALUES (@TYPECD, @WORKCD, @DEFLINNO, @MATCD, @ITEMCD, @CAUSECD, @CLASSCD, @ITEMNM, @CAUSENM, @CLASSNM, @ITEMNO, @DELFG, @LASTUPDDT);";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (ms.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                      UPDATE TmDefect SET itemcd=@ITEMCD, causecd=@CAUSECD, classcd=@CLASSCD,
                                        itemnm=@ITEMNM, causenm=@CAUSENM, classnm=@CLASSNM, itemno=@ITEMNO, delfg=@DELFG, lastupddt=@LASTUPDDT
                                      WHERE materialcd=@MATCD AND workcd=@WORKCD AND deflinno=@DEFLINNO";

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

		private static void deleteFRMS(FRMS[] armsList, FRMS[] nascaList) 
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pMatCd = cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar);
				SqlParameter pWorkCd = cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar);
				SqlParameter pDefLinNo = cmd.Parameters.Add("@DEFLINNO", SqlDbType.Int);

				foreach (FRMS data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.MaterialCd == data.MaterialCd && n.WorkCd == data.WorkCd && n.DefLinNo == data.DefLinNo))
					{
						continue;
					}

					string sql = " DELETE FROM TmDefect WHERE materialcd = @MATCD AND workcd = @WORKCD AND deflinno = @DEFLINNO ";

					pMatCd.Value = data.MaterialCd;
					pWorkCd.Value = data.WorkCd;
					pDefLinNo.Value = data.DefLinNo;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typecd"></param>
        /// <returns></returns>
        private static FRMS[] getFRMS(string typecd, int? targetPastHours)
        {
            List<FRMS> retv = new List<FRMS>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

//                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = typecd + "%" + Config.Settings.MaterialCodeSurfix;

//                cmd.CommandText = @"
//                        SELECT
//                        material_cd
//                        , work_cd
//                        , DefLin_NO
//                        , ntmFRMS.defcause_cd
//                        , defcause_ja
//                        , ntmFRMS.defclass_cd
//                        , defclass_ja
//                        , ntmFRMS.defitem_cd
//                        , defitem_ja
//                        , defitem_no
//                        , NtmFRms.del_fg
//                        , NtmFRMS.lastupd_dt
//                        FROM NtmFRMS(NOLOCK)
//                        INNER JOIN NtmFRKM(NOLOCK)
//                        ON NtmFRMS.defitem_cd= ntmfrkm.defitem_cd
//                        INNER JOIN NtmFRKN(NOLOCK)
//                        ON NtmFRMS.DefCause_CD = NtmFRKN.defCause_Cd
//                        INNER JOIN NtmFRBR(NOLOCK)
//                        ON NtmFRMS.DefClass_Cd = NtmFRBR.DefClass_Cd
//                        WHERE Material_cd LIKE @MATCD";

                cmd.CommandText = @" SELECT NtmFRMS.Material_CD, NtmFRMS.Work_CD, NtmFRMS.DefLin_NO, NtmFRMS.DefCause_CD, 
                                    NtmFRKN.DefCause_JA, NtmFRMS.DefClass_CD, 
                                    NtmFRBR.DefClass_JA, NtmFRMS.DefItem_CD, NtmFRKM.DefItem_JA, NtmFRMS.DefItem_NO, NtmFRMS.Del_FG, NtmFRMS.LastUpd_DT
                                    FROM dbo.NtmFRMS AS NtmFRMS WITH (NOLOCK) 
                                    INNER JOIN dbo.NtmFRKN AS NtmFRKN WITH (NOLOCK) ON NtmFRMS.DefCause_CD = NtmFRKN.DefCause_CD 
                                    INNER JOIN dbo.NtmFRKM AS NtmFRKM WITH (NOLOCK) ON NtmFRMS.DefItem_CD = NtmFRKM.DefItem_CD 
                                    INNER JOIN dbo.NtmFRBR AS NtmFRBR WITH (NOLOCK) ON NtmFRMS.DefClass_CD = NtmFRBR.DefClass_CD 
                                    INNER JOIN dbo.RvmMCONV AS RvmMCONV WITH (NOLOCK) ON NtmFRMS.Material_CD = RvmMCONV.material_cd 
                                    INNER JOIN ROOTSDB.dbo.RTMNFORMGROUP AS RTMNFORMGROUP WITH (nolock) ON RvmMCONV.workcond_cd = RTMNFORMGROUP.fcode
                                    WHERE (RvmMCONV.mtralbase_cd = @TYPECD) AND (RTMNFORMGROUP.fgroupclass_cd = @FGROUPCLSCD) ";

                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typecd;

                //2016.03.01 品目統合一時対応
                int nascaLineGroupCd = Config.Settings.NascaLineGroupCd;
                if (Config.Settings.UnificationTargetTypeList.Exists(r => r == typecd))
                {
                    nascaLineGroupCd = FrmBridgeMain.NASCA_LINE_GROUP_CODE_A;
                }

                cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = nascaLineGroupCd;

                if (targetPastHours.HasValue)
                {
                    cmd.CommandText += $" AND (NtmFRMS.LastUpd_DT >= '{ System.DateTime.Now.AddHours(-targetPastHours.Value) }') ";
                }

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        FRMS fr = new FRMS();

                        fr.TypeCd = typecd;
                        fr.MaterialCd = rd["material_cd"].ToString().Trim();
                        fr.WorkCd = rd["work_cd"].ToString().Trim();
                        fr.DefLinNo = Convert.ToInt32(rd["DefLin_NO"]);
                        fr.ItemCd = rd["defitem_cd"].ToString().Trim();
                        fr.ItemNm = rd["defitem_ja"].ToString().Trim();
                        fr.ClassCd = rd["defclass_cd"].ToString().Trim();
                        fr.ClassNm = rd["defclass_ja"].ToString().Trim();
                        fr.CauseCd = rd["defcause_cd"].ToString().Trim();
                        fr.CauseNm = rd["defcause_ja"].ToString().Trim();
                        fr.ItemNo = Convert.ToInt32(rd["defitem_no"]);
                        fr.DelFg = SQLite.ParseBool(rd["del_fg"]);
                        fr.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);

                        retv.Add(fr);
                    }
                }
            }

            return retv.ToArray();
        }

		private static FRMS[] getData(string typecd) 
		{
			List<FRMS> retv = new List<FRMS>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, workcd, itemcd, causecd, classcd, itemnm, causenm, classnm, itemno, delfg, lastupddt, deflinno, materialcd
								FROM TmDefect WITH (nolock)
								WHERE typecd = @TYPECD ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						FRMS f = new FRMS();

						f.TypeCd = typecd;
						f.WorkCd = rd["workcd"].ToString().Trim();
						f.ItemCd = rd["itemcd"].ToString().Trim();
						f.CauseCd = rd["causecd"].ToString().Trim();
						f.ClassCd = rd["classcd"].ToString().Trim();
						f.DefLinNo = SQLite.ParseInt(rd["deflinno"]);
						f.MaterialCd = rd["materialcd"].ToString().Trim();
						f.DelFg = SQLite.ParseBool(rd["delfg"]);
						f.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

						retv.Add(f);
					}
				}
			}

			return retv.ToArray();
		}

        #region getTypes
        
        private static string[] getTypes()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    SELECT DISTINCT typecd  FROM TmWorkFlow";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(SQLite.ParseString(rd["typecd"]));
                    }
                }
            }

            return retv.ToArray();
        }
        #endregion

        public class FRMS
        {
            public string TypeCd { get; set; }
            public string MaterialCd { get; set; }
            public string WorkCd { get; set; }
            public int DefLinNo { get; set; }
            public string CauseCd { get; set; }
            public string ClassCd { get; set; }
            public string ItemCd { get; set; }
            public string ItemNm { get; set; }
            public string CauseNm { get; set; }
            public string ClassNm { get; set; }
            public int ItemNo { get; set; }
            public bool DelFg { get; set; }
            public DateTime LastUpdDt { get; set; }
        }
    }
}
