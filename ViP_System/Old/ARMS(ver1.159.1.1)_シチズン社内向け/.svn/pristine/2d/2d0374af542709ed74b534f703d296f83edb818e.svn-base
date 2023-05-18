using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
	/// <summary>
	/// 不良判定マスタ取り込み
	/// 41移管2次で検証中
	/// </summary>
	public class DefectJudge
	{
		public string TypeCd { get; set; }
		public int ProcNo { get; set; }
		public string DefItemCD { get; set; }
		public decimal JudgDefectCt { get; set; }
		public bool DelFg { get; set; }
		public DateTime LastUpdDt { get; set; }

		public static void Import()
		{
			try
			{
				string[] types = getTypes();

				foreach (string type in types)
				{
					List<DefectJudge> nascaList = getNascaData(type);
					update(nascaList);

					List<DefectJudge> armsList = getData(type);
					delete(armsList, nascaList);
				}
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] DefectJudge Error:" + err.ToString());
			}
		}

		private static List<DefectJudge> getNascaData(string typeCd)
		{
			List<DefectJudge> retv = new List<DefectJudge>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT NtmFRHT.Material_CD, NtmFRHT.DefItem_CD, NtmFRHT.JudgLin_NO, NtmFRHT.Judg_Defect_CT, NtmFRHT.Del_FG, NtmFRHT.LastUpd_DT
								FROM dbo.NtmFRHT AS NtmFRHT WITH (nolock) 
								INNER JOIN dbo.RvmMCONV AS RvmMCONV WITH (nolock) ON NtmFRHT.Material_CD = RvmMCONV.material_cd 
								INNER JOIN ROOTSDB.dbo.RTMNFORMGROUP AS RTMNFORMGROUP WITH (nolock) ON RvmMCONV.workcond_cd = RTMNFORMGROUP.fcode
								WHERE (RvmMCONV.mtralbase_cd = @TYPECD) AND (RTMNFORMGROUP.fgroupclass_cd = @FGROUPCLSCD) 
								AND (RTMNFORMGROUP.fgroup_cd in ({0})) ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typeCd;
                ////2016.03.01 品目統合一時対応
                //int nascaLineGroupCd = Config.Settings.NascaLineGroupCd;
                //if (Config.Settings.UnificationTargetTypeList.Exists(r => r == typeCd))
                //{
                //    nascaLineGroupCd = FrmBridgeMain.NASCA_LINE_GROUP_CODE_A;
                //}
                //cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = nascaLineGroupCd;
                cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = Config.Settings.NascaLineGroupCd;
				sql = string.Format(sql, string.Join(",", Config.Settings.NascaOrderMoveInfo.Select(o => o.Key)));

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						string materialCd = rd["Material_CD"].ToString().Trim();
						string procGrpCd = materialCd.Substring(materialCd.IndexOf(".") + 1, materialCd.Length - materialCd.IndexOf(".") - 2);

						DefectJudge dj = new DefectJudge();
						dj.TypeCd = typeCd;

						Process p = Process.GetLastProcess(procGrpCd, typeCd, true);
						if (p == null)
						{
							throw new ApplicationException(
								string.Format("作業CDが取得できませんでした。型番:{0} 工程CD:{1}", typeCd, procGrpCd));
						}
						dj.ProcNo = p.ProcNo;

						dj.DefItemCD = rd["DefItem_CD"].ToString().Trim();
						dj.JudgDefectCt = Convert.ToDecimal(rd["Judg_Defect_CT"]);
						dj.DelFg = SQLite.ParseBool(rd["del_fg"]);
						dj.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);
						retv.Add(dj);
					}
				}
			}

			return retv;
		}

		private static List<DefectJudge> getData(string typeCd)
		{
			List<DefectJudge> retv = new List<DefectJudge>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, procno, defitemcd, judgdefectct, delfg, lastupddt
								FROM TmDefectJudge WITH (nolock) WHERE typecd = @TYPECD ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						DefectJudge d = new DefectJudge();

						d.TypeCd = typeCd;
						d.ProcNo = SQLite.ParseInt(rd["procno"]);
						d.DefItemCD = rd["defitemcd"].ToString().Trim();
						d.JudgDefectCt = Convert.ToDecimal(rd["judgdefectct"]);
						d.DelFg = SQLite.ParseBool(rd["delfg"]);
						d.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

						retv.Add(d);
					}
				}
			}

			return retv;
		}

		private static void update(List<DefectJudge> nascaList)
		{
			if (nascaList.Count == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pProcNo = cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt);
				SqlParameter pDefItemCd = cmd.Parameters.Add("@DEFITEMCD", SqlDbType.NVarChar);
				SqlParameter pJudgDefectCt = cmd.Parameters.Add("@JUDGDEFECTCT", SqlDbType.Decimal);
				SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
				SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

				con.Open();

				foreach (DefectJudge nasca in nascaList)
				{
					pType.Value = nasca.TypeCd;
					pProcNo.Value = nasca.ProcNo;
					pDefItemCd.Value = nasca.DefItemCD;
					pJudgDefectCt.Value = nasca.JudgDefectCt;
					pDelFg.Value = SQLite.SerializeBool(nasca.DelFg);
					pLastUpdDt.Value = nasca.LastUpdDt;

					cmd.CommandText = @"
                            SELECT lastupddt FROM TmDefectJudge
                            WHERE typecd=@TYPECD AND procno=@PROCNO AND defitemcd=@DEFITEMCD ";

					object objlastupd = cmd.ExecuteScalar();

					if (objlastupd == null)
					{
						cmd.CommandText = @"
                                INSERT INTO TmDefectJudge(typecd, procno, defitemcd, judgdefectct, delfg, lastupddt)
                                VALUES (@TYPECD, @PROCNO, @DEFITEMCD, @JUDGDEFECTCT, @DELFG, @LASTUPDDT);";
						cmd.ExecuteNonQuery();
						continue;
					}
					else
					{
						DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
						if (nasca.LastUpdDt > current.AddSeconds(1))
						{
							cmd.CommandText = @"
                                      UPDATE TmDefectJudge SET judgdefectct=@JUDGDEFECTCT, delfg=@DELFG, lastupddt=@LASTUPDDT
                                      WHERE typecd = @TYPECD AND procno = @PROCNO AND defitemcd=@DEFITEMCD ";

							cmd.ExecuteNonQuery();
						}
					}
				}
			}

		}

		private static void delete(List<DefectJudge> armsList, List<DefectJudge> nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pProcNo = cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt);
				SqlParameter pDefItemCd = cmd.Parameters.Add("@DEFITEMCD", SqlDbType.NVarChar);

				foreach (DefectJudge data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.ProcNo == data.ProcNo && n.DefItemCD == data.DefItemCD))
					{
						continue;
					}

					string sql = " DELETE FROM TmDefectJudge WHERE typecd = @TYPECD AND procno = @PROCNO AND defitemcd=@DEFITEMCD ";

					pTypeCd.Value = data.TypeCd;
					pProcNo.Value = data.ProcNo;
					pDefItemCd.Value = data.DefItemCD;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
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
                    SELECT DISTINCT typecd FROM TmWorkFlow WITH(nolock)";

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
