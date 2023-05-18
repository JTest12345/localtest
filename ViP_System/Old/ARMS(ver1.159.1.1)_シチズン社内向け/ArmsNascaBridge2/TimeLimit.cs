using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsNascaBridge2
{
    class TimeLimit
    {
        public static bool Import()
        {
			try
			{
				DateTime start = DateTime.Now;
				string[] types = getTypes();

				foreach (string type in types)
				{
                    //2016.03.01 品目統合一時対応
                    string matCdSurfix = Config.Settings.AsmMatCdSurfix;
                    if (Config.Settings.UnificationTargetTypeList.Exists(r => r == type))
                    {
                        matCdSurfix = FrmBridgeMain.ASM_MAT_CODE_SURFIX_A;
                    }

                    NascaSGKS[] sgksList = getSGKS(type, matCdSurfix);
					updateTimeLimit(type, sgksList);

					//2014.7.23 41移管2次で検証中
					ArmsApi.Model.TimeLimit[] armsList = getData(type);
					deleteTimeLimit(armsList, sgksList);
				}

                return true;
			}
			catch(Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] TimeLimit:" + err.ToString());
                return false;
			}
        }


        private static void updateTimeLimit(string typecd, NascaSGKS[] sgksList)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
                SqlParameter pChkWorkCd = cmd.Parameters.Add("@CHKWORKCD", SqlDbType.NVarChar);
                SqlParameter pChkWorkKb = cmd.Parameters.Add("@CHKWORKKB", SqlDbType.NVarChar);
                SqlParameter pTgtWorkCd = cmd.Parameters.Add("@TGTWORKCD", SqlDbType.NVarChar);
                SqlParameter pTgtWorkKb = cmd.Parameters.Add("@TGTWORKKB", SqlDbType.NVarChar);
                SqlParameter pChkLinNo = cmd.Parameters.Add("CHKLINNO", SqlDbType.Int);
                SqlParameter pAttention = cmd.Parameters.Add("@ATTENTION", SqlDbType.BigInt);
                SqlParameter pEffect = cmd.Parameters.Add("@EFFECT", SqlDbType.BigInt);
                SqlParameter pWarning = cmd.Parameters.Add("@WARNING", SqlDbType.BigInt);
                SqlParameter pLineNo = cmd.Parameters.Add("@LINENO", SqlDbType.BigInt);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);

                cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;

                foreach (NascaSGKS sgks in sgksList)
                {
                    pLineNo.Value = 0;
                    pDelFg.Value = 0;
                    pChkWorkCd.Value = sgks.ChkWorkCd;
                    pChkWorkKb.Value = sgks.ChkWorkKb;
                    pTgtWorkCd.Value = sgks.TgtWorkCd;
                    pTgtWorkKb.Value = sgks.TgtWorkKb;
                    pChkLinNo.Value = sgks.ChkLinNo;
                    pAttention.Value = sgks.Attention;
                    pEffect.Value = sgks.Effect;
                    pWarning.Value = sgks.Warning;

                    cmd.CommandText = @"
                    SELECT lastupddt FROM TmTimeLimit
                    WHERE typecd = @TYPECD 
                    AND chkworkcd = @CHKWORKCD AND chkworkkb = @CHKWORKKB
                    AND chklinno = @CHKLINNO";
                    //AND tgtworkkb = @TGTWORKKB AND tgtworkcd = @TGTWORKCD";

                    object objDt = cmd.ExecuteScalar();

                    if (objDt == null)
                    {
                        if (sgks.DelFg == true) continue;

                        #region Insert
                        
                        try
                        {
                            cmd.Transaction = con.BeginTransaction();
                            cmd.CommandText = "SELECT MAX([lineno]) FROM TmTimeLimit";
                            object objmaxno = cmd.ExecuteScalar();
                            int maxno = 1;
                            if (objmaxno != null)
                            {
                                int.TryParse(objmaxno.ToString(), out maxno);
                            }
                            pLineNo.Value = maxno + 1;


                            cmd.CommandText = @"
                            INSERT INTO TmTimeLimit(typecd, chkworkcd, chkworkkb, [lineno], tgtworkcd, tgtworkkb,
                            attension, warning, effect, delfg, lastupddt, chklinno)
                            VALUES(@TYPECD, @CHKWORKCD, @CHKWORKKB, @LINENO, @TGTWORKCD, @TGTWORKKB, @ATTENTION,
                            @WARNING, @EFFECT, 0, @LASTUPDDT, @CHKLINNO)";
                            cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                        }
                        catch(Exception err)
                        {
                            cmd.Transaction.Rollback();
                        }
                        #endregion

                        continue;
                    }
                    else
                    {
                        DateTime? existdt = SQLite.ParseDate(objDt);
                        if (existdt != null && existdt.Value.AddMinutes(1) >= sgks.LastUpdDt)
                        {
                            continue;
                        }

                        pDelFg.Value = SQLite.SerializeBool(sgks.DelFg);

                        #region Update
                        
                        try
                        {
                            cmd.Transaction = con.BeginTransaction();
                            cmd.CommandText = "SELECT MAX([lineno]) FROM TmTimeLimit";
                            object objmaxno = cmd.ExecuteScalar();
                            int maxno = 1;
                            if (objmaxno != null)
                            {
                                int.TryParse(objmaxno.ToString(), out maxno);
                            }
                            pLineNo.Value = maxno + 1;

                            cmd.CommandText = @"
                            UPDATE TmTimeLimit SET 
                                typecd=@TYPECD, chkworkcd=@CHKWORKCD, chkworkkb=@CHKWORKKB,
                                [lineno]=@LINENO, tgtworkcd=@TGTWORKCD, tgtworkkb=@TGTWORKKB,
                                attension=@ATTENTION, warning=@WARNING, effect=@EFFECT,
                                delfg=@DELFG, lastupddt=@LASTUPDDT
                            WHERE typecd=@TYPECD AND chkworkcd=@CHKWORKCD AND chkworkkb=@CHKWORKKB
							AND chklinno = @CHKLINNO";
//                            AND tgtworkkb=@TGTWORKKB AND tgtworkcd=@TGTWORKCD";

                            cmd.ExecuteNonQuery();
                            cmd.Transaction.Commit();
                        }
                        catch
                        {
                            cmd.Transaction.Rollback();
                        }
                        #endregion
                    }
                }
            }
        }

		private static void deleteTimeLimit(ArmsApi.Model.TimeLimit[] armsList, NascaSGKS[] nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pChkWorkCd = cmd.Parameters.Add("@CHKWORKCD", SqlDbType.NVarChar);
				SqlParameter pChkWorkKb = cmd.Parameters.Add("@CHKWORKKB", SqlDbType.NVarChar);
				SqlParameter pChkLinNo = cmd.Parameters.Add("@CHKLINNO", SqlDbType.Int);

				foreach (ArmsApi.Model.TimeLimit data in armsList)
				{
					string armsChkKb = ArmsApi.Model.TimeLimit.GetNascaJudgeKb(data.ChkKb);

					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.ChkWorkCd == data.ChkWorkCd
							&& n.ChkWorkKb == armsChkKb && n.ChkLinNo == data.ChkLinNo))
					{
						continue;
					}

					string sql = @" DELETE FROM TmTimeLimit WHERE typecd = @TYPECD AND chkworkcd = @CHKWORKCD 
									AND chkworkkb = @CHKWORKKB AND chklinno = @CHKLINNO ";

					pTypeCd.Value = data.TypeCd;
					pChkWorkCd.Value = data.ChkWorkCd;
					pChkWorkKb.Value = armsChkKb;
					pChkLinNo.Value = data.ChkLinNo;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}

        private static NascaSGKS[] getSGKS(string typecd, string matcdsurfix)
        {
            List<NascaSGKS> retv = new List<NascaSGKS>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @"
                        select
                         ChkMate_cd
                        , ChkWork_CD
                        , ChkWork_Kb
                        , ChkLin_NO
                        , TargeWork_cd
                        , TargeWork_Kb
                        , Attention_Dt
                        , Warning_dt
                        , Effect_dt
                        , lastupd_dt
                        , Del_fg
                        From ntmsgks
                        where Del_fg = 0 AND ChkMate_cd LIKE @MATCD";

                cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = typecd + ".%" + matcdsurfix;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        NascaSGKS sg = new NascaSGKS();
						sg.TypeCd = typecd;
                        sg.ChkWorkCd = rd["ChkWork_CD"].ToString().Trim();
                        sg.ChkWorkKb = rd["ChkWork_Kb"].ToString().Trim();
                        sg.TgtWorkCd = rd["TargeWork_cd"].ToString().Trim();
                        sg.TgtWorkKb = rd["TargeWork_Kb"].ToString().Trim();
                        sg.ChkLinNo = Convert.ToInt32(rd["ChkLin_NO"]);
                        sg.Attention = Convert.ToInt32(rd["Attention_Dt"]);
                        sg.Warning = Convert.ToInt32(rd["Warning_dt"]);
                        sg.Effect = Convert.ToInt32(rd["Effect_dt"]);
                        sg.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);
                        sg.DelFg = SQLite.ParseBool(rd["Del_fg"]);
                        retv.Add(sg);
                    }
                }
            }

            return retv.ToArray();
        }

		private static ArmsApi.Model.TimeLimit[] getData(string typecd)
		{
			List<ArmsApi.Model.TimeLimit> retv = new List<ArmsApi.Model.TimeLimit>();

			using (SqlConnection con = new SqlConnection(SQLite.ConStr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

				string sql = @" 
				SELECT 
					typecd , 
					chkworkcd , 
					chkworkkb ,
					tgtworkcd ,
					tgtworkkb , 
					attension , 
					warning , 
					effect ,
					chklinno
				FROM 
					TmTimeLimit WITH(nolock)
				WHERE 
					typecd = @TYPECD ";

				cmd.CommandText = sql;
				using (SqlDataReader reader = cmd.ExecuteReader())
				{
					while (reader.Read())
					{
						ArmsApi.Model.TimeLimit l = new ArmsApi.Model.TimeLimit();
						l.TypeCd = SQLite.ParseString(reader["typecd"]);

						l.ChkWorkCd = SQLite.ParseString(reader["chkworkcd"]);
						string ckb = SQLite.ParseString(reader["chkworkkb"]);
						if (ckb == ArmsApi.Model.TimeLimit.JUDGEKB_WORKSTART) l.ChkKb = ArmsApi.Model.TimeLimit.JudgeKb.Start;
						else if (ckb == ArmsApi.Model.TimeLimit.JUDGEKB_WORKEND) l.ChkKb = ArmsApi.Model.TimeLimit.JudgeKb.End;
						else throw new Exception("作業監視マスタの開始完了区分設定が不正です:" + typecd + ":" + ckb);

						l.TgtWorkCd = SQLite.ParseString(reader["tgtworkcd"]);
						string tkb = SQLite.ParseString(reader["tgtworkkb"]);
						if (tkb == ArmsApi.Model.TimeLimit.JUDGEKB_WORKSTART) l.TgtKb = ArmsApi.Model.TimeLimit.JudgeKb.Start;
						else if (tkb == ArmsApi.Model.TimeLimit.JUDGEKB_WORKEND) l.TgtKb = ArmsApi.Model.TimeLimit.JudgeKb.End;
						else throw new Exception("作業監視マスタの開始完了区分設定が不正です:" + typecd + ":" + tkb);

						l.AttensionBefore = SQLite.ParseInt(reader["attension"]);
						l.WarningBefore = SQLite.ParseInt(reader["warning"]);
						l.EffectLimit = SQLite.ParseInt(reader["effect"]);

						l.ChkLinNo = SQLite.ParseInt(reader["chklinno"]);

						retv.Add(l);
					}
				}
			}

			return retv.ToArray();
		}

        private class NascaSGKS
        {
			public string TypeCd { get; set; }
            public string ChkWorkCd { get; set; }
            public string ChkWorkKb { get; set; }
            public string TgtWorkCd { get; set; }
            public string TgtWorkKb { get; set; }
            public int ChkLinNo { get; set; }
            public int Attention { get; set; }
            public int Warning { get; set; }
            public int Effect { get; set; }
            public DateTime LastUpdDt { get; set; }
            public bool DelFg { get; set; }
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
                    while(rd.Read())
                    {
                        retv.Add(SQLite.ParseString(rd["typecd"]));
                    }
                }
            }

            return retv.ToArray();
        }
        #endregion
    }
}
