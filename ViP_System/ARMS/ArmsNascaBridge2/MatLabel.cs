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

    /// <summary>
    /// 原材料ラベルヘッダーのマスタ取り込み
    /// </summary>
    class MatLabel
    {
        private class LabelData
        {
            public string LabelKb { get; set; }
            public string LabelNo { get; set; }
            public string MaterialCd { get; set; }
            public bool DelFg { get; set; }
            public DateTime LastUpdDt { get; set; }
        }

        public static bool Import()
        {
            try
            {
				//TODO 2015.1.31 NtmMLTD.Material_CDにインデックスを付与する等後に対策が必要
				DateTime updfrom = DateTime.Now.AddMonths(-96);
				LabelData[] labels = GetLabelDataList(updfrom);
                updateMatLabel(labels);

				LabelData[] nascaList = GetLabelDataList();
				Material.MatLabel[] armsList = Material.GetMaterialCdFromLabels(true);
				deleteMatLabel(armsList, nascaList);

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] MatLabel Error:" + err.ToString());
                return false;
			}
		}

		private static void updateMatLabel(LabelData[] master)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                SqlParameter prmLabelKb = cmd.Parameters.Add("@LABELKB", SqlDbType.NVarChar);
                SqlParameter prmLabelNo = cmd.Parameters.Add("@LABELNO", SqlDbType.NVarChar);
                SqlParameter prmMatCd = cmd.Parameters.Add("@MATCD", SqlDbType.NVarChar);
                SqlParameter prmDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter prmUpdDt = cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime);

                foreach (LabelData d in master)
                {
                    prmLabelKb.Value = d.LabelKb;
                    prmLabelNo.Value = d.LabelNo;
                    prmMatCd.Value = d.MaterialCd;
                    prmDelFg.Value = SQLite.SerializeBool(d.DelFg);
                    prmUpdDt.Value = d.LastUpdDt;

                    cmd.CommandText = @"
                    SELECT lastupddt FROM TmMatLabel WHERE labelno=@LABELNO AND labelkb=@LABELKB";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd == null)
                    {
                        cmd.CommandText = @"
                        INSERT INTO TmMatLabel(labelkb, labelno, materialcd, delfg, lastupddt)
                        VALUES (@LABELKB, @LABELNO, @MATCD, @DELFG, @UPDDT);";
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (d.LastUpdDt > current)
                        {
                            cmd.CommandText = @"
                            UPDATE TmMatLabel SET materialcd=@MATCD, delfg=@DELFG, lastupddt=@UPDDT
                            WHERE labelkb=@LABELKB AND labelno=@LABELNO";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

		private static void deleteMatLabel(Material.MatLabel[] armsList, LabelData[] nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pLabelKb = cmd.Parameters.Add("@LABELKB", SqlDbType.NVarChar);
				SqlParameter pLabelNo = cmd.Parameters.Add("@LABELNO", SqlDbType.NVarChar);

				foreach (Material.MatLabel data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.LabelKb == data.LabelKb && n.LabelNo == data.LabelNo))
					{
						continue;
					}

					string sql = " DELETE FROM TmMatLabel WHERE labelkb = @LABELKB AND labelno = @LABELNO ";

					pLabelKb.Value = data.LabelKb;
					pLabelNo.Value = data.LabelNo;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}

		/// <summary>
		/// NASCAからラベルマスタ取得
		/// </summary>
		/// <param name="updfrom"></param>
		/// <returns></returns>
		private static LabelData[] GetLabelDataList(DateTime? updfrom)
		{
			List<LabelData> retv = new List<LabelData>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @"
                    select label_kb, nichia_master_no, material_cd, del_fg, lastupd_dt from ntmmltd
                    where material_cd is not null and material_cd <> '' ";

				//if (updfrom.HasValue)
				//{
				//	sql += " and lastupd_dt >= @UPDDT ";
				//	cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = updfrom;
				//}

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						LabelData dt = new LabelData();
						dt.LabelKb = SQLite.ParseString(rd["label_kb"]).Trim();
						dt.LabelNo = SQLite.ParseString(rd["nichia_master_no"]).Trim();
						dt.MaterialCd = SQLite.ParseString(rd["material_cd"]).Trim();
						dt.DelFg = SQLite.ParseBool(rd["del_fg"]);
						dt.LastUpdDt = SQLite.ParseDate(rd["lastupd_dt"]) ?? DateTime.MinValue;

						retv.Add(dt);
					}
				}
			}

			return retv.ToArray();
		}
		private static LabelData[] GetLabelDataList()
		{
			return GetLabelDataList(null);
		}
    }
}
