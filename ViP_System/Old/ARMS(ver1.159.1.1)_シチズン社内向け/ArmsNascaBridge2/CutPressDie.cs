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
    class CutPressDie
    {
		public string TypeCd { get; set; }
        public string CutPlantCd { get; set; }
        public string CutWorkCd { get; set; }
        public bool DelFg { get; set; }
        public DateTime LastUpdDt { get; set; }

		private const string CONDCD_PRESSDIE = "PRESSDIE";

        public static bool Import()
        {
			try
			{
				DateTime start = DateTime.Now;
				string[] types = getTypes();

				foreach (string type in types)
				{
					CutPressDie[] dieList = getPressDieList(type);
					updatePressDie(type, dieList);

					//2014.7.23 41移管2次で検証中
					TypeCondition[] armsList = getData(type);
					deletePressDie(armsList, dieList);
				}

                return true;
			}
			catch(Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] CutPressDie Error:" + err.ToString());
                return false;
			}
        }

        private static void updatePressDie(string typecd, CutPressDie[] dieList)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                SqlParameter pLineNo = cmd.Parameters.Add("@LINENO", SqlDbType.BigInt);
                SqlParameter pDieNo = cmd.Parameters.Add("@DIENO", SqlDbType.NVarChar);
                SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pWorkCd = cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Bit);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                foreach (CutPressDie die in dieList)
                {
                    pDieNo.Value = die.CutPlantCd;
                    pLineNo.Value = 0;
                    pTypeCd.Value = typecd;
                    pWorkCd.Value = die.CutWorkCd;
                    pDelFg.Value = die.DelFg;
                    pLastUpdDt.Value = die.LastUpdDt;

                    cmd.CommandText = @"
                        SELECT lastupddt FROM TmTypeCond 
                        WHERE condcd = 'PRESSDIE' 
                        AND typecd = @TYPECD
                        AND workcd = @WORKCD
                        AND condval = @DIENO";

                    object lastupddt = cmd.ExecuteScalar();
                    if (lastupddt == null)
                    {
                        cmd.Transaction = con.BeginTransaction();
                        try
                        {
                            cmd.CommandText = "SELECT MAX([lineno]) FROM TmTypeCond";
                            object objmaxno = cmd.ExecuteScalar();
                            int maxno = 1;
                            if (objmaxno != null)
                            {
                                int.TryParse(objmaxno.ToString(), out maxno);
                            }
                            pLineNo.Value = maxno + 1;

                            cmd.CommandText = @"
                            INSERT INTO TmTypeCond(typecd, condcd, [lineno], condval,delfg, lastupddt, workcd)
                            VALUES(@TYPECD, 'PRESSDIE',@LINENO, @DIENO, @DELFG, @LASTUPDDT, @WORKCD)";

                            cmd.ExecuteNonQuery();

                            cmd.Transaction.Commit();
                        }
                        catch
                        {
                            cmd.Transaction.Rollback();
                        }
                    }
                    else 
                    {
                        DateTime current = SQLite.ParseDate(lastupddt) ?? DateTime.MinValue;
                        if (die.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                    UPDATE TmTypeCond SET delfg = @DELFG, lastupddt = @LASTUPDDT
                                    WHERE condcd = 'PRESSDIE' AND typecd = @TYPECD AND workcd = @WORKCD AND condval = @DIENO ";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

		private static void deletePressDie(TypeCondition[] armsList, CutPressDie[] nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pCondCd = cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar);
				SqlParameter pCondVal = cmd.Parameters.Add("@CONDVAL", SqlDbType.NVarChar);
                SqlParameter pWorkCd = cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar);

                foreach (TypeCondition data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.CutPlantCd == data.CondVal && n.CutWorkCd == data.WorkCd))
					{
						continue;
					}

					string sql = " DELETE FROM TmTypeCond WHERE typecd = @TYPECD AND condcd = @CONDCD AND workcd {0} AND condval = @CONDVAL ";

					pTypeCd.Value = data.TypeCd;
					pCondCd.Value = CONDCD_PRESSDIE;
					pCondVal.Value = data.CondVal;
                    if (string.IsNullOrEmpty(data.WorkCd) == true)
                    {
                        sql = string.Format(sql, " IS NULL ");
                        pWorkCd.Value = DBNull.Value;
                    }
                    else
                    {
                        sql = string.Format(sql, " = @WORKCD ");
                        pWorkCd.Value = data.WorkCd;
                    }

                    cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}

		private static TypeCondition[] getData(string typecd)
		{
			List<TypeCondition> retv = new List<TypeCondition>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, condcd, [lineno], condval, delfg, lastupddt, workcd
									FROM TmTypeCond WITH (nolock)
									WHERE typecd = @TYPECD AND condcd = @CONDCD ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;
				cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar).Value = CONDCD_PRESSDIE;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						TypeCondition tc = new TypeCondition();

						tc.TypeCd = typecd;
						tc.CondCd = rd["condcd"].ToString().Trim();
						tc.LineNo = SQLite.ParseInt(rd["lineno"]);
						tc.CondVal = rd["condval"].ToString().Trim();
						tc.DelFg = SQLite.ParseBool(rd["delfg"]);
						tc.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);
                        tc.WorkCd = rd["workcd"].ToString().Trim();

                        retv.Add(tc);
					}
				}
			}

			return retv.ToArray();
		}

        private static CutPressDie[] getPressDieList(string typecd)
        {
            List<CutPressDie> retv = new List<CutPressDie>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //cmd.CommandText = @" SELECT NtmSGST.Plant_CD, NtmSGST.Del_FG,  NtmSGST.LastUpd_DT
                //                     FROM dbo.NtmSGST AS NtmSGST WITH (nolock) 
                //                        INNER JOIN ROOTSDB.dbo.RTMMCONV AS RTMMCONV WITH (nolock) ON NtmSGST.Material_CD = RTMMCONV.material_cd 
                //                        INNER JOIN ROOTSDB.dbo.RTMNFORMGROUP AS RTMNFORMGROUP WITH (nolock) ON RTMMCONV.workcond_cd = RTMNFORMGROUP.fcode
                //                     WHERE (RTMMCONV.del_fg = '0') AND (RTMNFORMGROUP.del_fg = '0')
                //                        AND (RTMMCONV.mtralbase_cd = @MATERIALCD) AND (RTMNFORMGROUP.fgroup_cd = 70004) ";

                cmd.CommandText = @" SELECT NtmSGST.Plant_CD, NtmSGST.Work_CD, NtmSGST.Del_FG, NtmSGST.LastUpd_DT
                                    FROM dbo.NtmSGST AS NtmSGST WITH (nolock) 
                                        INNER JOIN ROOTSDB.dbo.RTMMCONV AS RTMMCONV WITH (nolock) ON NtmSGST.Material_CD = RTMMCONV.material_cd 
                                        INNER JOIN ROOTSDB.dbo.RTMNFORMGROUP AS RTMNFORMGROUP WITH (nolock) ON RTMMCONV.workcond_cd = RTMNFORMGROUP.fcode
                                        INNER JOIN dbo.NtmSTBI AS NtmSTBI WITH (nolock) ON NtmSGST.Plant_CD = NtmSTBI.Plant_CD
                                    WHERE (RTMMCONV.del_fg = '0') AND (RTMNFORMGROUP.del_fg = '0') AND (NtmSTBI.Del_FG = 0)
                                        AND (RTMMCONV.mtralbase_cd = @MATERIALCD) AND (NtmSTBI.PlantClas_CD = 'KMO')
                                        AND (RTMNFORMGROUP.fgroupclass_cd = @FGROUPCLSCD) ";

                cmd.Parameters.Add("@MATERIALCD", SqlDbType.Char).Value = typecd;
                cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = Config.Settings.NascaLineGroupCd;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    int ordPlantCD = rd.GetOrdinal("Plant_CD");
                    int ordWorkCD = rd.GetOrdinal("Work_CD");
                    int ordDelFG = rd.GetOrdinal("Del_FG");
                    int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");

                    while (rd.Read())
                    {
                        try
                        {
                            CutPressDie cut = new CutPressDie();
							cut.TypeCd = typecd;
                            cut.CutPlantCd = rd.GetString(ordPlantCD).Trim();
                            cut.CutWorkCd = rd.GetString(ordWorkCD).Trim();
                            cut.DelFg = SQLite.ParseBool(rd[ordDelFG]);
                            cut.LastUpdDt = rd.GetDateTime(ordLastUpdDT);

                            //自動化、高効率など全ての品目で削除フラグがたっていれば削除対象とする
                            if (retv.Exists(r => r.CutPlantCd == cut.CutPlantCd))
                            {
                                int index = retv.FindIndex(r => r.CutPlantCd == cut.CutPlantCd);
                                if (!cut.DelFg) 
                                {
                                    retv[index] = cut; 
                                }
                            }
                            else
                            {
                                retv.Add(cut);
                            }
                        }
                        catch { }
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

    }
}
