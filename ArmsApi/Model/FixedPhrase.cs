using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace ArmsApi.Model
{
	/// <summary>
	/// 定型文
	/// 2015.2.9 車載3次 追加
	/// </summary>
	public class FixedPhrase
	{
		public bool ChangeFg { get; set; }

		public string TextKb { get; set; }
		public string Text { get; set; }
		public bool	DelFg { get; set; }
		public string UpdUserCd { get; set; }
		public DateTime LastUpdDt { get; set; }

		public string OldText { get; set; }

		private static List<FixedPhrase> GetData(string textkb, bool includeDelete) 
		{
			List<FixedPhrase> retv = new List<FixedPhrase>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			{
				con.Open();

				using (SqlCommand cmd = con.CreateCommand()) 
				{
					cmd.CommandText = " SELECT textkb, text, delfg, updusercd, lastupddt FROM TmFixedPhrase WHERE textkb = @TEXTKB ";

					cmd.Parameters.Add("@TEXTKB", SqlDbType.NVarChar).Value = textkb;
					if (includeDelete == false)
					{
						cmd.CommandText += " AND (delfg = 0) ";
					}

					using (SqlDataReader rd = cmd.ExecuteReader()) 
					{
						while(rd.Read())
						{
							FixedPhrase f = new FixedPhrase();
							f.TextKb = rd["textkb"].ToString().Trim();
							f.Text = rd["text"].ToString().Trim();
							f.DelFg = SQLite.ParseBool(rd["delfg"]);
							f.UpdUserCd = rd["updusercd"].ToString().Trim();
							f.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

							f.OldText = f.Text;

							retv.Add(f);
						}
					}
				}
			}

			return retv;
		}

		/// <summary>
		/// データメンテナンス定型文取得
		/// </summary>
		/// <returns></returns>
		public static List<FixedPhrase> GetDataMaintenance(bool includeDelete) 
		{
			return GetData("DataMaintenance", includeDelete);
		}

		public static void InsertUpdate(FixedPhrase data)
		{
			if (string.IsNullOrEmpty(data.TextKb) || string.IsNullOrEmpty(data.Text))
			{
				throw new ApplicationException("定型文を空白で登録できません。");
			}

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			{
				con.Open();

				using (SqlCommand cmd = con.CreateCommand())
				{
					cmd.CommandText = @" UPDATE TmFixedPhrase SET
											text = @text,  
											delfg = @delfg, updusercd = @updusercd, lastupddt = @lastupddt 
										 WHERE textkb = @textkb AND text = @oldtext
										 INSERT INTO TmFixedPhrase (textkb, text, updusercd)
										 SELECT @textkb, @text, @updusercd
										 WHERE NOT EXISTS (SELECT * FROM TmFixedPhrase WHERE textkb = @textkb AND text = @text) ";

					cmd.Parameters.Add("@textkb", SqlDbType.NVarChar).Value = data.TextKb;
					cmd.Parameters.Add("@text", SqlDbType.NVarChar).Value = data.Text;
					cmd.Parameters.Add("@delfg", SqlDbType.Bit).Value = data.DelFg;
					cmd.Parameters.Add("@updusercd", SqlDbType.NVarChar).Value = data.UpdUserCd;
					cmd.Parameters.Add("@lastupddt", SqlDbType.DateTime).Value = data.LastUpdDt;

					cmd.Parameters.Add("@oldtext", SqlDbType.NVarChar).Value = data.OldText;

					cmd.ExecuteNonQuery();
				}
			}
		}
	}
}
