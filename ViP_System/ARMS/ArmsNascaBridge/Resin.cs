using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;
using ArmsApi.Model.NJDB;

namespace ArmsNascaBridge
{
    /// <summary>
    /// 樹脂取り込み
    /// 2015.2.9 3in1 MixTypeCD取り込み追加
    /// </summary>
    public class Resin
    {
        public int MixResultId { get; set; }
        public string ResinGpCd { get; set; }
        public DateTime UseLimit { get; set; }
        public DateTime LastUpdDt { get; set; }
        public DateTime? StirringLimitDt { get; set; }
        public string MixTypeCd { get; set; }

        public bool KakudatsuFg { get; set; }

        public static void Import()
        {
            try
            {
				using (var armsDB = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
				using (var resinDB = new ArmsApi.Model.DataContext.NJDBDataContext(Config.Settings.ResinDBConSTR))
				{
					string[] typelist = getTypes();

					System.Diagnostics.Debug.Write(string.Join("',\r\n'", typelist));

					foreach (string typecd in typelist)
					{
						Resin[] resinList = getResin(typecd);

						foreach (Resin resin in resinList)
						{
							if (resin.UseLimit > DateTime.Now)
							{
								IEnumerable<MixMaterial> resinMatList = MixMaterial.GetAllMaterial(resinDB, resin.MixResultId);

								MixMaterial.UpdateResinMix(armsDB, resinMatList);
							}

							updateResin(resin);
						}
					}
				}
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge] Resin Error:" + err.ToString());
            }
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

        private static void updateResin(Resin resin)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MIXRESULTID", SqlDbType.BigInt).Value = resin.MixResultId;
                cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = resin.ResinGpCd;
                cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@BINCD", SqlDbType.NVarChar).Value = Config.Settings.ResinBinCd;
                cmd.Parameters.Add("@LIMITDT", SqlDbType.DateTime).Value = resin.UseLimit;

                cmd.Parameters.Add("@MIXTYPECD", SqlDbType.NVarChar).Value = resin.MixTypeCd;
                cmd.Parameters.Add("@STIRRINGLIMITDT", SqlDbType.DateTime).Value = SQLite.GetParameterValue(resin.StirringLimitDt);


                cmd.CommandText = @"
                    SELECT lastupddt FROM TnResinMix
                    WHERE mixresultid=@MIXRESULTID";

                object objlastupd = cmd.ExecuteScalar();

                if (objlastupd == null)
                {
                    // 作業チェックON + 全作業未完了の場合は、取り込まない
                    if (resin.KakudatsuFg == true && IsCompleteAllWorks(resin) == false)
                    {
                        return;
                    }

                    cmd.CommandText = @"
                        INSERT INTO TnResinMix(mixresultid, resingroupcd, uselimit, bincd, lastupddt, mixtypecd, stirringlimitdt)
                        VALUES (@MIXRESULTID, @RESINGPCD, @LIMITDT, @BINCD, @UPDDT, @MIXTYPECD, @STIRRINGLIMITDT);";
                    cmd.ExecuteNonQuery();
                    return;
                }
                else
                {
                    DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                    if (resin.LastUpdDt > current)
                    {
                        cmd.CommandText = @"
                            UPDATE TnResinMix SET resingroupcd=@RESINGPCD, uselimit=@LIMITDT, bincd=@BINCD, lastupddt=@UPDDT, mixtypecd=@MIXTYPECD
                                              , stirringlimitdt=@STIRRINGLIMITDT
                            WHERE mixresultid=@MIXRESULTID";

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        private static Resin[] getResin(string typecd)
        {
            List<Resin> retv = new List<Resin>();

            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"
                    SELECT
                     mixresult_id
                    , resingroup_cd
                    , mixture_dt
                    , start_dt
                    , end_dt
                    , mixvalidity_tm
                    , mixvalidity_kb
                    , jttmixresult.lastupd_dt
					, jtmmixrate.MixType_CD
                    , jtmmixrate.Kakudatsu_FG
                    FROM JttMIXRESULT WITH(NOLOCK)
                    INNER JOIN JtmMIXRATE WITH(NOLOCK)
                    ON JttMIXRESULT.mixrate_id = JtmMIXRATE.mixrate_id
                    WHERE mixture_dt >= @MIXDT
                    AND EXISTS (
	                    SELECT
	                     *
	                    FROM JtmTYPECONV WITH(NOLOCK)
	                    WHERE JtmTYPECONV.type_cd=JtmMIXRATE.type_Cd
	                    AND nascaType_CD = @TYPECD)";

                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typecd;
                cmd.Parameters.Add("@MIXDT", SqlDbType.DateTime).Value = DateTime.Now.AddDays(-10);


                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        Resin r = new Resin();
                        r.MixResultId = Convert.ToInt32(rd["mixresult_id"]);
                        r.ResinGpCd = rd["resingroup_cd"].ToString().Trim();

                        DateTime start = Convert.ToDateTime(rd["start_dt"]);
                        DateTime end = Convert.ToDateTime(rd["end_dt"]);
                        int validtm = Convert.ToInt32(rd["mixvalidity_tm"]);
                        int kb = Convert.ToInt32(rd["mixvalidity_kb"]);
                        r.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);

                        if (kb == 1)
                        {
                            r.UseLimit = start.AddMinutes(validtm);
                        }
                        else
                        {
                            r.UseLimit = end.AddMinutes(validtm);
                        }

                        r.MixTypeCd = rd["MixType_CD"].ToString().Trim();
                        r.KakudatsuFg = Convert.ToBoolean(rd["Kakudatsu_FG"]);

                        // NADB01.dbo.JttJSTGの更新日時と脱泡後有効期限の情報を追加
                        // NJDB02で取得した更新日時をNADB01の取得更新日時で上書き
                        r = AddJstgInfoNasca(r);

                        retv.Add(r);
                    }
                }
            }
            return retv.ToArray();
        }

        private static bool IsNascaAsmThrow(int resultId)
        {
            bool retv = false;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" SELECT AsmThrow_FG FROM JttJSTG WITH(nolock) WHERE (Del_FG = 0) Result_ID = @RESULTID ";

                cmd.Parameters.Add("@RESULTID", SqlDbType.Int).Value = resultId;

                object isAsmThrow = cmd.ExecuteScalar();
                if (isAsmThrow == null || isAsmThrow == System.DBNull.Value)
                {
                    retv = false;
                }
                else
                {
                    retv = Convert.ToBoolean(isAsmThrow);
                }
            }

            return retv;
        }

        // NASCAのJttJSTGにレコードがあるなら、更新日時と脱泡後有効期限の情報を追加する。
        private static Resin AddJstgInfoNasca(Resin resin)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" SELECT LastUpd_DT, StirringLimit_DT 
                                     FROM JttJSTG WITH(nolock) 
                                     WHERE Del_FG = 0 AND Result_ID = @RESULTID 
                                     OPTION(MAXDOP 1) ";

                cmd.Parameters.Add("@RESULTID", SqlDbType.Int).Value = resin.MixResultId;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        resin.LastUpdDt = SQLite.ParseDate(rd["LastUpd_DT"]).Value;
                        resin.StirringLimitDt = SQLite.ParseDate(rd["StirringLimit_DT"]);
                    }
                }
            }
            return resin;            
        }

        private static bool IsCompleteAllWorks(Resin resin)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.ResinDBConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MIXRESULTID", SqlDbType.BigInt).Value = resin.MixResultId;
                cmd.Parameters.Add("@RESINGPCD", SqlDbType.NVarChar).Value = resin.ResinGpCd;

                cmd.CommandText = @"
                        SELECT TOP 1 JtmWORKFROW.WorkFrow_ID
                        FROM JtmWORKFROW 
                            LEFT OUTER JOIN JttWORKRESULT 
                                ON JtmWORKFROW.WorkFrow_ID = JttWORKRESULT.WorkFrow_ID
                                AND JttWORKRESULT.Del_FG = 0
                                AND JttWORKRESULT.MixResult_ID = @MIXRESULTID
                        WHERE 
                            JtmWORKFROW.Del_FG = 0 AND
                            JtmWORKFROW.ResinGroup_CD = @RESINGPCD AND
                            JttWORKRESULT.WorkFrow_ID IS NULL";

                object objworkflow = cmd.ExecuteScalar();
                if (objworkflow != null)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
        }
    }
}
