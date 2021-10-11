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
	/// カットラベル出力先設定
	/// 2015.2.9 車載3次 追加
	/// </summary>
	public class CutLabelFile
	{
		public bool ChangeFg { get; set; }

		public int MacNo { get; set; }
		public string OutputPath { get; set; }
		public bool DelFg { get; set; }
		public string UpdUserCd { get; set; }
		public DateTime LastUpdDt { get; set; }
		public bool IsDefault { get; set; }

		public int OldMacNo { get; set; }

		public static CutLabelFile GetOutputPathSetting(int macno)
		{
			List<CutLabelFile> f = GetData(macno, false, false, false);
			if (f.Count == 0) { return null; }
			else
			{
				return f[0];
			}
		}

		public static CutLabelFile GetDefaultOutputPathSetting()
		{
			List<CutLabelFile> f = GetData(null, false, false, true);
			if (f.Count >= 2)
			{
				throw new ApplicationException("カットラベル出力先で複数の規定値設定がされている為、選択できません。");
			}
			else if (f.Count == 0)
			{
				throw new ApplicationException("カットラベル出力先で規定値の設定がありません。");
			}
			else
			{
				return f.Single();
			}
		}

		public static List<CutLabelFile> GetAllMachineSetting()
		{
			return GetData(null, true, true, false);
		}

		public static List<CutLabelFile> GetData(int? macno, bool includeDelete, bool includeDefault, bool isDefault)
		{
			List<CutLabelFile> retv = new List<CutLabelFile>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			{
				con.Open();

				using (SqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = " SELECT macno, outputpath, delfg, updusercd, lastupddt FROM TmCutLabelFile WHERE 1=1";

					if (macno.HasValue)
					{
						cmd.Parameters.Add("macno", SqlDbType.BigInt).Value = macno.Value;
						cmd.CommandText += " AND (macno = @macno) ";
					}

					if (includeDelete == false)
					{
						cmd.CommandText += " AND (delfg = 0) ";
					}

					if (includeDefault == false)
					{
						cmd.CommandText += " AND (defaultfg = @defaultfg) ";
						cmd.Parameters.Add("@defaultfg", SqlDbType.Bit).Value = isDefault;
					}

					using (SqlDataReader rd = cmd.ExecuteReader())
					{
						while (rd.Read())
						{
							CutLabelFile f = new CutLabelFile();
							f.MacNo = SQLite.ParseInt(rd["macno"]);
							f.OutputPath = rd["outputpath"].ToString().Trim();
							f.DelFg = SQLite.ParseBool(rd["delfg"]);
							f.UpdUserCd = rd["updusercd"].ToString().Trim();
							f.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);
							f.OldMacNo = f.MacNo;
							retv.Add(f);
						}
					}
				}
			}

			return retv;
		}

		public static void InsertUpdate(CutLabelFile data)
		{
			if (data.MacNo == 0)
			{
				throw new ApplicationException("設備の指定が無い為、登録できません。");
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			{
				con.Open();

				using (SqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = @" UPDATE TmCutLabelFile SET
											macno = @macno, outputpath = @outputpath,
											delfg = @delfg, updusercd = @updusercd, lastupddt = @lastupddt, defaultfg = @defaultfg
										 WHERE macno = @oldmacno
										 INSERT INTO TmCutLabelFile (macno, outputpath, updusercd, defaultfg)
										 SELECT @macno, @outputpath, @updusercd, @defaultfg
										 WHERE NOT EXISTS (SELECT * FROM TmCutLabelFile WHERE macno = @macno) ";

					cmd.Parameters.Add("@macno", SqlDbType.BigInt).Value = data.MacNo;
					cmd.Parameters.Add("@outputpath", SqlDbType.NVarChar).Value = data.OutputPath;
					cmd.Parameters.Add("@delfg", SqlDbType.Bit).Value = data.DelFg;
					cmd.Parameters.Add("@updusercd", SqlDbType.NVarChar).Value = data.UpdUserCd;
					cmd.Parameters.Add("@lastupddt", SqlDbType.DateTime).Value = data.LastUpdDt;
					cmd.Parameters.Add("@defaultfg", SqlDbType.Bit).Value = data.IsDefault;

					cmd.Parameters.Add("@oldmacno", SqlDbType.BigInt).Value = data.OldMacNo;

					cmd.ExecuteNonQuery();
				}
			}
		}


	}
}
