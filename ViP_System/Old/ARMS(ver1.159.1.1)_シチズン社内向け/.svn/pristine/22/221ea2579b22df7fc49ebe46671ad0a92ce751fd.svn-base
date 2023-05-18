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
	/// ダイシェア抜き取り対象型番取り込み
	/// 2015.1.1 3in1新規構築
	/// </summary>
	public class DieShearSampler
	{
		public string TypeCd { get; set; }
		public bool DelFg { get; set; }
		public DateTime LastUpdDt { get; set; }

		public static bool Import()
		{
			try
			{
				string[] types = getTypes();

				foreach (string type in types)
				{
					List<DieShearSampler> pstesterList = getPsTesterData(type);
					update(pstesterList);

					List<DieShearSampler> armsList = getData(type);
					delete(armsList, pstesterList);
				}

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] DieShearSampler Error:" + err.ToString());
                return false;
			}
		}

		private static List<DieShearSampler> getPsTesterData(string typeCd)
		{
			List<DieShearSampler> retv = new List<DieShearSampler>();

			using (SqlConnection con = new SqlConnection(Config.Settings.PSTesterConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT Name_Char, Ment_DT, Del_FG
								FROM dbo.T_MST_COMMON with(nolock)
								WHERE (Name_kind = 'RedType') ";

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						DieShearSampler d = new DieShearSampler();
						d.TypeCd = rd["Name_Char"].ToString().Trim();
						d.LastUpdDt = Convert.ToDateTime(rd["Ment_DT"]);
						d.DelFg = SQLite.ParseBool(rd["Del_FG"]);
						retv.Add(d);
					}
				}
			}

			return retv;
		}

		private static List<DieShearSampler> getData(string typeCd)
		{
			List<DieShearSampler> retv = new List<DieShearSampler>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT code, delfg, updatedt
								FROM TmGeneral WITH (nolock) WHERE tid = @TID AND code = @TYPECD ";

				cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = GnlMaster.TID_DIESHEARSAMPLETYPE;
				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						DieShearSampler d = new DieShearSampler();

						d.TypeCd = typeCd;
						d.DelFg = SQLite.ParseBool(rd["delfg"]);
						d.LastUpdDt = Convert.ToDateTime(rd["updatedt"]);

						retv.Add(d);
					}
				}
			}

			return retv;
		}

		private static void update(List<DieShearSampler> pstesterList)
		{
			if (pstesterList.Count == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = GnlMaster.TID_DIESHEARSAMPLETYPE;

				SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
				SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

				con.Open();

				foreach (DieShearSampler pstester in pstesterList)
				{
					pType.Value = pstester.TypeCd;
					pDelFg.Value = SQLite.SerializeBool(pstester.DelFg);
					pLastUpdDt.Value = pstester.LastUpdDt;

					cmd.CommandText = @"
                            SELECT updatedt FROM TmGeneral
                            WHERE tid = @TID AND code = @TYPECD ";

					object objlastupd = cmd.ExecuteScalar();

					if (objlastupd == null)
					{
						cmd.CommandText = @"
                                INSERT INTO TmGeneral(tid, code, val, val2, delfg, updatedt)
                                VALUES (@TID, @TYPECD, @TYPECD, '', @DELFG, @LASTUPDDT);";
						cmd.ExecuteNonQuery();
						continue;
					}
					else
					{
						DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
						if (pstester.LastUpdDt > current.AddSeconds(1))
						{
							cmd.CommandText = @"
                                      UPDATE TmGeneral SET delfg=@DELFG, updatedt=@LASTUPDDT
                                      WHERE tid = @TID AND code = @TYPECD ";

							cmd.ExecuteNonQuery();
						}
					}
				}
			}

		}

		private static void delete(List<DieShearSampler> armsList, List<DieShearSampler> nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = GnlMaster.TID_DIESHEARSAMPLETYPE;

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);

				foreach (DieShearSampler data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd))
					{
						continue;
					}

					string sql = " DELETE FROM TmGeneral WHERE tid = @TID AND code = @TYPECD ";

					pTypeCd.Value = data.TypeCd;

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
                    SELECT DISTINCT typecd  FROM TmWorkFlow WITH(nolock) ";

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
