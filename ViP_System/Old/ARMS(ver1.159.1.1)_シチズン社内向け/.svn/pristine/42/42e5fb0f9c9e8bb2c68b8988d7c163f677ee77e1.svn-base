using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge
{
	/// <summary>
	/// ブレンド詳細マスタ取り込み(NtmBDSS)
	/// </summary>
	public class Blend
	{
		public string WaferMaterialCd { get; set; }
		public string BlendCd { get; set; }
		public int BlendLinNo { get; set; }
		public string SupplyId { get; set; }
		public string WorkCd { get; set; }
		public bool DelFg { get; set; }
		public DateTime LastUpdDt { get; set; }

		public static void Import()
		{
			try
			{
				List<string> blendCdList = getProfileBlendCd();
				foreach (string blendCd in blendCdList) 
				{
					List<Blend> nascaList = getNascaData(blendCd);
					update(nascaList);

					List<Blend> armsList = getData(blendCd);
					delete(armsList, nascaList);
				}
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] Blend Error:" + err.ToString());
			}
		}

		private static List<Blend> getNascaData(string blendcd)
		{
			List<Blend> retv = new List<Blend>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT Blend_CD, BlendLin_NO, WfrMate_CD, Supply_ID, Work_CD, Del_FG, LastUpd_DT
						FROM dbo.NtmBDSS WITH (nolock)
						WHERE Del_FG = 0 AND Blend_CD = @BLENDCD ";

				cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar).Value = blendcd;

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						Blend b = new Blend();

						b.BlendCd = blendcd;
						b.BlendLinNo = Convert.ToInt32(rd["BlendLin_NO"]);
						b.WaferMaterialCd = rd["WfrMate_CD"].ToString().Trim();
						b.SupplyId = rd["Supply_ID"].ToString().Trim();
						b.WorkCd = rd["Work_CD"].ToString().Trim();
						b.DelFg = Convert.ToBoolean(rd["Del_FG"]);
						b.LastUpdDt = Convert.ToDateTime(rd["LastUpd_DT"]);

						retv.Add(b);
					}
				}
			}

			return retv;
		}

		private static List<Blend> getData(string blendcd)
		{
			List<Blend> retv = new List<Blend>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT blendcd, blendlinno, wafermaterialcd, supplyid, workcd, delfg, lastupddt
								FROM TmBlend with(nolock) WHERE blendcd = @BlendCd ";

				cmd.Parameters.Add("@BlendCd", SqlDbType.NVarChar).Value = blendcd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						Blend b = new Blend();

						b.BlendCd = blendcd;
						b.BlendLinNo = Convert.ToInt32(rd["blendlinno"]);
						b.WaferMaterialCd = rd["wafermaterialcd"].ToString().Trim();
						b.SupplyId = rd["supplyid"].ToString().Trim();
						b.WorkCd = rd["workcd"].ToString().Trim();
						b.DelFg = Convert.ToBoolean(rd["delfg"]);
						b.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

						retv.Add(b);
					}
				}
			}

			return retv;
		}

		private static List<string> getProfileBlendCd()
		{
			List<string> retv = new List<string>(); 

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT blendcd 
							 FROM TmProfile WITH(nolock) 
							 WHERE (delfg = 0) 
							 GROUP BY blendcd ";

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader()) 
				{
					while (rd.Read()) 
					{
						retv.Add(rd["blendcd"].ToString().Trim());
					}
				}				
			}

			return retv;
		}

		private static void update(List<Blend> nascaList)
		{
			if (nascaList.Count == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				SqlParameter pBlendCd = cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar);
				SqlParameter pBlendLinNo = cmd.Parameters.Add("@BLENDLINNO", SqlDbType.Int);

				SqlParameter pWaferMaterialCd = cmd.Parameters.Add("@WAFERMATERIALCD", SqlDbType.NVarChar);
				SqlParameter pSupplyId = cmd.Parameters.Add("@SUPPLYID", SqlDbType.NVarChar);
				SqlParameter pWorkCd = cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar);

				SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
				SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

				con.Open();

				foreach (Blend nasca in nascaList)
				{
					pBlendCd.Value = nasca.BlendCd;
					pBlendLinNo.Value = nasca.BlendLinNo;
					pWaferMaterialCd.Value = nasca.WaferMaterialCd;
					pSupplyId.Value = nasca.SupplyId;
					pWorkCd.Value = nasca.WorkCd;
					pDelFg.Value = SQLite.SerializeBool(nasca.DelFg);
					pLastUpdDt.Value = nasca.LastUpdDt;

					cmd.CommandText = @"
                            SELECT lastupddt FROM TmBlend
                            WHERE blendcd=@BLENDCD AND blendlinno=@BLENDLINNO ";

					object objlastupd = cmd.ExecuteScalar();

					if (objlastupd == null)
					{
						cmd.CommandText = @"
                                INSERT INTO TmBlend(blendcd, blendlinno, wafermaterialcd, supplyid, workcd, delfg, lastupddt)
                                VALUES (@BLENDCD, @BLENDLINNO, @WAFERMATERIALCD, @SUPPLYID, @WORKCD, @DELFG, @LASTUPDDT);";
						cmd.ExecuteNonQuery();
						continue;
					}
					else
					{
						DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
						if (nasca.LastUpdDt > current.AddSeconds(1))
						{
							cmd.CommandText = @"
                                      UPDATE TmBlend SET wafermaterialcd=@WAFERMATERIALCD, supplyid=@SUPPLYID, workcd=@WORKCD, delfg=@DELFG, lastupddt=@LASTUPDDT
                                      WHERE blendcd = @BLENDCD AND blendlinno = @BLENDLINNO ";

							cmd.ExecuteNonQuery();
						}
					}
				}
			}
		}

		private static void delete(List<Blend> armsList, List<Blend> nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pBlendCd = cmd.Parameters.Add("@BLENDCD", SqlDbType.NVarChar);
				SqlParameter pBlendLinNo = cmd.Parameters.Add("@BLENDLINNO", SqlDbType.Int);

				foreach (Blend data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.BlendCd == data.BlendCd && n.BlendLinNo == data.BlendLinNo))
					{
						continue;
					}

					string sql = " DELETE FROM TmBlend WHERE blendcd = @BLENDCD AND blendlinno = @BLENDLINNO ";

					pBlendCd.Value = data.BlendCd;
					pBlendLinNo.Value = data.BlendLinNo;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}
