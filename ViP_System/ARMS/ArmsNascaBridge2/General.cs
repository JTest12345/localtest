using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;

namespace ArmsNascaBridge2
{
	/// <summary>
	/// ダイシングブレード取り込み+基板マッピングデータ取込対象フラグ取込
	/// </summary>
    public class General
    {
        public string TId { get; set; }
        public string Code { get; set; }
        public string Val { get; set; }
        public string Val2 { get; set; }
        public bool DelFg { get; set; }
        public DateTime LastUpdDt { get; set; }

        private const string TID_DICINGBLADE = "MAPDICINGBLADE";
        private const string TID_MAPPINGIMPORTFRAME = "MAPPINGIMPORTFRAME";

        public static bool Import()
        {
			try
			{
                //ブレード取込
				General[] nascaList = getGeneral();
				updateGeneral(nascaList);
                
				GnlMaster[] armsList = ArmsApi.Model.GnlMaster.Search(TID_DICINGBLADE);
				deleteGeneral(armsList, nascaList);

                //基板マッピング対象基板品目取込
                //General[] nascaMappingList = getMappingGeneral();
                //updateGeneral(nascaList);

                //GnlMaster[] armsMappingList = ArmsApi.Model.GnlMaster.Search(TID_MAPPINGIMPORTFRAME);
                //deleteGeneral(armsMappingList, nascaMappingList);

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] General Error:" + err.ToString());
                return false;
			}
        }

        public static General[] getGeneral() 
        {
            List<General> retv = new List<General>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                        SELECT 
                        material_cd
                        , LabelCheck_VAL
                        , del_fg
                        , lastupd_dt
                        FROM NtmHMHJ(NOLOCK)
                        WHERE LabelCheck_KB=1";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        General ge = new General();
						ge.TId = TID_DICINGBLADE;
						ge.Code = rd["LabelCheck_VAL"].ToString().Trim();
						ge.Val = rd["material_cd"].ToString().Trim();
                        ge.Val2 = "";
                        ge.DelFg = SQLite.ParseBool(rd["del_fg"]);
                        ge.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);

                        retv.Add(ge);
                    }
                }
            }

            return retv.ToArray();
        }

        public static General[] getMappingGeneral()
        {
            List<General> retv = new List<General>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                        SELECT 
                        material_cd
                        , del_fg
                        , lastupd_dt
                        FROM NtmHMHJ(NOLOCK)
                        WHERE Substrate_KB=3";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        General ge = new General();
                        ge.TId = TID_MAPPINGIMPORTFRAME;
                        ge.Code = rd["material_cd"].ToString().Trim();
                        ge.Val = "0";
                        ge.Val2 = "0";
                        ge.DelFg = SQLite.ParseBool(rd["del_fg"]);
                        ge.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);

                        retv.Add(ge);
                    }
                }
            }

            return retv.ToArray();
        }

        public static void updateGeneral(General[] gelist) 
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pTId = cmd.Parameters.Add("@TID", SqlDbType.NVarChar);
                SqlParameter pCode = cmd.Parameters.Add("@CODE", SqlDbType.NVarChar);
                SqlParameter pVal = cmd.Parameters.Add("@VAL", SqlDbType.NVarChar);
                SqlParameter pVal2 = cmd.Parameters.Add("@VAL2", SqlDbType.NVarChar);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                con.Open();

                foreach (General ge in gelist)
                {
                    pTId.Value = ge.TId;
                    pCode.Value = ge.Code;
                    pVal.Value = ge.Val;
                    pVal2.Value = ge.Val2;
                    pDelFg.Value = SQLite.SerializeBool(ge.DelFg);
                    pLastUpdDt.Value = ge.LastUpdDt;

                    cmd.CommandText = @"
                            SELECT updatedt FROM TmGeneral
                            WHERE tid=@TID AND code=@CODE AND val=@VAL AND val2=@VAL2 ";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd == null)
                    {
                        cmd.CommandText = @"
                                INSERT INTO TmGeneral(tid, code, val, val2, delfg, updatedt)
                                VALUES (@TID, @CODE, @VAL, @VAL2, @DELFG, @LASTUPDDT) ";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (ge.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                    UPDATE TmGeneral SET  delfg=@DELFG, updatedt=@LASTUPDDT
                                    WHERE tid=@TID AND code=@CODE AND val=@VAL AND val2=@VAL2 ";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }

        }

		public static void deleteGeneral(ArmsApi.Model.GnlMaster[] armsList, General[] nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTId = cmd.Parameters.Add("@TID", SqlDbType.NVarChar);
				SqlParameter pCode = cmd.Parameters.Add("@CODE", SqlDbType.NVarChar);
				SqlParameter pVal = cmd.Parameters.Add("@VAL", SqlDbType.NVarChar);

				foreach (ArmsApi.Model.GnlMaster data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TId == data.Tid && n.Code == data.Code && n.Val == data.Val))
					{
						continue;
					}

					string sql = " DELETE FROM TmGeneral WHERE tid = @TID AND code = @CODE AND val = @VAL ";

					pTId.Value = data.Tid;
					pCode.Value = data.Code;
					pVal.Value = data.Val;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}
    }
}
