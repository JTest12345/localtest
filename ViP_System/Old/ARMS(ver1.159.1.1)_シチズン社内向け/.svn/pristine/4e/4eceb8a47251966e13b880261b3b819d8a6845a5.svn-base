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
	/// DBウェハー水洗浄後有効期限
	/// 2015.5.14 3in1高効率ラインのシステム変更依頼1(水洗浄)
	/// </summary>
	public class DBWaferWashedLife
	{
		public string TypeCd { get; set; }
		public string WaferTypeCd { get; set; }
		public int DbWaferWashedLife { get; set; }
		public bool DelFg { get; set; }
		public DateTime LastUpdDt { get; set; }

		public static bool Import()
		{
			try
			{
				string[] types = getTypes();
				foreach (string type in types)
				{
					List<DBWaferWashedLife> nascaList = getNascaData(type);
					update(nascaList);

					List<DBWaferWashedLife> armsList = getData(type);
					delete(armsList, nascaList);
				}

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] DBWaferWashedLife Error:" + err.ToString());
                return false;
			}
		}

		public static List<DBWaferWashedLife> getNascaData(string typecd)
		{
			List<DBWaferWashedLife> retv = new List<DBWaferWashedLife>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				//cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = typecd + "." + Config.Settings.MaterialCodeSurfix;
                //MAPでは未使用のためコメントアウト。他品種で必要な場合はここを復活する。
                ////2016.03.01 品目統合一時対応
                //int nascaLineGroupCd = Config.Settings.NascaLineGroupCd;
                //if (Config.Settings.UnificationTargetTypeList.Exists(r => r == typecd))
                //{
                //    nascaLineGroupCd = FrmBridgeMain.NASCA_LINE_GROUP_CODE_A;
                //}
                //cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = nascaLineGroupCd;
                cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = Config.Settings.NascaLineGroupCd;
				cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typecd;

				string sql = @" SELECT p.Material_CD, p.Type_CD, p.General_VAL, p.Del_FG, p.LastUpd_DT
							FROM NtmPDJK p (NOLOCK)
							inner join ROOTSDB.dbo.RtmMCONV m (NOLOCK) ON p.Material_CD = m.material_cd 
							inner join ROOTSDB.dbo.RTMNFORMGROUP fg (NOLOCK) ON m.workcond_cd = fg.fcode
							WHERE Condition_KB in (22) AND fg.fgroupclass_cd = @FGROUPCLSCD AND m.mtralbase_cd = @TYPECD ";

				//Material_cd LIKE @MATCD

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						DBWaferWashedLife item = new DBWaferWashedLife();

						item.TypeCd = typecd;
						item.WaferTypeCd = rd["Type_CD"].ToString().Trim();
						item.DbWaferWashedLife = Convert.ToInt32(rd["General_VAL"]);
						item.DelFg = SQLite.ParseBool(rd["Del_FG"]);
						item.LastUpdDt = Convert.ToDateTime(rd["LastUpd_DT"]);

						retv.Add(item);
					}
				}
			}

			return retv;
		}

		private static List<DBWaferWashedLife> getData(string typeCd)
		{
			List<DBWaferWashedLife> retv = new List<DBWaferWashedLife>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, wafertypecd, lifefromwashend, delfg, lastupddt
								FROM TmDBWaferWashedLife WITH (nolock) WHERE typecd = @TYPECD ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						DBWaferWashedLife d = new DBWaferWashedLife();

						d.TypeCd = typeCd;
						d.WaferTypeCd = rd["wafertypecd"].ToString().Trim();
						d.DbWaferWashedLife = Convert.ToInt32(rd["lifefromwashend"]);
						d.DelFg = SQLite.ParseBool(rd["delfg"]);
						d.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

						retv.Add(d);
					}
				}
			}

			return retv;
		}

		private static void update(List<DBWaferWashedLife> nascaList)
		{
			if (nascaList.Count == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pWaferTypeCd = cmd.Parameters.Add("@WAFERTYPECD", SqlDbType.NVarChar);

				SqlParameter pLifeFromWashEnd = cmd.Parameters.Add("@LIFEFROMWASHEND", SqlDbType.BigInt);
				SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
				SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

				con.Open();

				foreach (DBWaferWashedLife nasca in nascaList)
				{
					pType.Value = nasca.TypeCd;
					pWaferTypeCd.Value = nasca.WaferTypeCd;
					pLifeFromWashEnd.Value = nasca.DbWaferWashedLife;
					pDelFg.Value = SQLite.SerializeBool(nasca.DelFg);
					pLastUpdDt.Value = nasca.LastUpdDt;

					cmd.CommandText = @"
                            SELECT lastupddt FROM TmDBWaferWashedLife
                            WHERE typecd=@TYPECD AND wafertypecd=@WAFERTYPECD ";

					object objlastupd = cmd.ExecuteScalar();

					if (objlastupd == null)
					{
						cmd.CommandText = @"
                                INSERT INTO TmDBWaferWashedLife(typecd, wafertypecd, lifefromwashend, delfg, lastupddt)
                                VALUES (@TYPECD, @WAFERTYPECD, @LIFEFROMWASHEND, @DELFG, @LASTUPDDT);";
						cmd.ExecuteNonQuery();
						continue;
					}
					else
					{
						DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
						if (nasca.LastUpdDt > current.AddSeconds(1))
						{
							cmd.CommandText = @"
                                      UPDATE TmDBWaferWashedLife SET lifefromwashend=@LIFEFROMWASHEND, delfg=@DELFG, lastupddt=@LASTUPDDT
                                      WHERE typecd = @TYPECD AND wafertypecd = @WAFERTYPECD ";

							cmd.ExecuteNonQuery();
						}
					}
				}
			}
		}

		private static void delete(List<DBWaferWashedLife> armsList, List<DBWaferWashedLife> nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pWaferTypeCd = cmd.Parameters.Add("@WAFERTYPECD", SqlDbType.NVarChar);

				foreach (DBWaferWashedLife data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.WaferTypeCd == data.WaferTypeCd))
					{
						continue;
					}

					string sql = " DELETE FROM TmDBWaferWashedLife WHERE typecd = @TYPECD AND wafertypecd = @WAFERTYPECD ";

					pTypeCd.Value = data.TypeCd;
					pWaferTypeCd.Value = data.WaferTypeCd;

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
                    SELECT DISTINCT typecd FROM TmWorkFlow with (nolock)";

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
