using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ArmsApi;
using ArmsApi.Model;
using System.Data.SqlClient;
using System.Data;

namespace ArmsNascaBridge2
{
    /// <summary>
	/// PDA条件マスタ(樹脂有効期限)
    /// </summary>
    public class DBResinLife
    {
        public string TypeCd { get; set; }
        public string ResinMaterialCd { get; set; }
        public int LifeFromUnpacktoWorkStart { get; set; }
        public int LifeFromUnpacktoWorkEnd { get; set; }
        public int LifeFromInput { get; set; }
        public int ForbiddenFromUnpacktoInput { get; set; }
        public bool DelFg { get; set; }
        public DateTime LastUpdDt { get; set; }

        public static bool Import()
        {
			try
			{
				string[] types = getTypes();
				foreach (string type in types)
				{
					DBResinLife[] mslist = GetPDJK(type);
					updateDBResinLife(mslist);

					//2014.7.23 41移管2次で検証中
					Material.ResinLife[] armsList = Material.getResinLifes(type, true);
					deleteDBResinLife(armsList, mslist);
				}

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] DBResinLife Error:" + err.ToString());
                return false;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="typecd"></param>
        /// <returns></returns>
        public static DBResinLife[] GetPDJK(string typecd)
        {
            List<DBResinLife> retv = new List<DBResinLife>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = typecd + "." + Config.Settings.MaterialCodeSurfix;

                //2016.03.01 品目統合一時対応
                int nascaLineGroupCd = Config.Settings.NascaLineGroupCd;
                if (Config.Settings.UnificationTargetTypeList.Exists(r => r == typecd))
                {
                    nascaLineGroupCd = FrmBridgeMain.NASCA_LINE_GROUP_CODE_A;
                }

                cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = nascaLineGroupCd;
				cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typecd;

                cmd.CommandText = @"
                        SELECT
                          p.Material2_CD
                        , p.Condition_KB
                        , p.General_VAL
                        , p.Check_FG
                        , p.del_fg
                        , p.lastupd_dt
                        , p.General2_VAL
                        FROM NtmPDJK p (NOLOCK)
						inner join ROOTSDB.dbo.RtmMCONV m (NOLOCK) ON p.Material_CD = m.material_cd 
						inner join ROOTSDB.dbo.RTMNFORMGROUP fg (NOLOCK) ON m.workcond_cd = fg.fcode
                        WHERE p.del_fg = 0 AND p.Condition_KB in (4, 5) AND fg.fgroupclass_cd = @FGROUPCLSCD AND m.mtralbase_cd = @TYPECD";

				//Material_cd LIKE @MATCD

                using (SqlDataReader rd = cmd.ExecuteReader())
                {

                    while (rd.Read())
                    {
                        string resinmatcd = rd["Material2_CD"].ToString().Trim();
                        int conditionkb = Convert.ToInt32(rd["Condition_KB"]);
                        bool checkfg = SQLite.ParseBool(rd["Check_FG"]);

                        int value2 = 0;
                        if (int.TryParse(Convert.ToString(rd["General2_VAL"]), out value2) == false)
                        {
                        }

                        int targetIndex 
                            = retv.FindIndex(r => r.TypeCd == typecd && r.ResinMaterialCd == resinmatcd);
                        if (targetIndex == -1)
                        {
                            DBResinLife pd = new DBResinLife();
                            pd.TypeCd = typecd;
                            pd.ResinMaterialCd = resinmatcd;

                            pd = GetValue(conditionkb, checkfg, Convert.ToInt32(rd["General_VAL"]), pd, value2);

                            pd.DelFg = SQLite.ParseBool(rd["del_fg"]);
                            pd.LastUpdDt = System.DateTime.Now;
                            retv.Add(pd);
                        }
                        else
                        {
                            retv[targetIndex] 
                                = GetValue(conditionkb, checkfg, Convert.ToInt32(rd["General_VAL"]), retv[targetIndex], value2);
                        }
                    }
                }
            }

            return retv.ToArray();
        }

        private static DBResinLife GetValue(int conditionkb, bool checkfg, int value, DBResinLife pd, int value2)
        {
            switch (conditionkb) 
            {
                case 4:  // PDA条件マスタ(投入後期限)
                    pd.LifeFromInput = value;
                    break;
                case 5:  // PDA条件マスタ(開封後期限)
                    pd.LifeFromUnpacktoWorkStart = value;
                    if (checkfg)
                    {
                        pd.LifeFromUnpacktoWorkEnd = value;
                    }
                    else 
                    {
                        pd.LifeFromUnpacktoWorkEnd = 0;
                    }
                    pd.ForbiddenFromUnpacktoInput = value2;
                    break;
            }

            return pd;
        }

        private static void updateDBResinLife(DBResinLife[] pdlist)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pResinMaterialCd = cmd.Parameters.Add("@RESINMATERIALCD", SqlDbType.NVarChar);
                SqlParameter pLifeFromUnpacktoWorkStart = cmd.Parameters.Add("@LIFEFROMUNPACKTOWORKSTART", SqlDbType.BigInt);
                SqlParameter pLifeFromUnpacktoWorkEnd = cmd.Parameters.Add("@LIFEFROMUNPACKTOWORKEND", SqlDbType.BigInt);
                SqlParameter pLifeFromInput = cmd.Parameters.Add("@LIFEFROMINPUT", SqlDbType.BigInt);
                SqlParameter pForbiddenFromUnpacktoInput = cmd.Parameters.Add("@FORBIDDENFROMUNPACKTOINPUT", SqlDbType.BigInt);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                con.Open();

                foreach (DBResinLife pd in pdlist)
                {
                    pType.Value = pd.TypeCd;
                    pResinMaterialCd.Value = pd.ResinMaterialCd;
                    pLifeFromUnpacktoWorkStart.Value = pd.LifeFromUnpacktoWorkStart;
                    pLifeFromUnpacktoWorkEnd.Value = pd.LifeFromUnpacktoWorkEnd;
                    pLifeFromInput.Value = pd.LifeFromInput;
                    pForbiddenFromUnpacktoInput.Value = pd.ForbiddenFromUnpacktoInput;
                    pDelFg.Value = SQLite.SerializeBool(pd.DelFg);
                    pLastUpdDt.Value = pd.LastUpdDt;

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TmDBResinLife
                            WHERE typecd=@TYPECD AND resinmaterialcd=@RESINMATERIALCD ";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd == null)
                    {
                        cmd.CommandText = @"
                                INSERT INTO TmDBResinLife(
                                        typecd, 
                                        resinmaterialcd, 
                                        lifefromunpacktoworkstart, 
                                        lifefromunpacktoworkend, 
                                        lifefrominput, 
                                        forbiddenfromunpacktoinput, 
                                        delfg, 
                                        lastupddt
                                        )
                                VALUES (
                                        @TYPECD, 
                                        @RESINMATERIALCD, 
                                        @LIFEFROMUNPACKTOWORKSTART, 
                                        @LIFEFROMUNPACKTOWORKEND, 
                                        @LIFEFROMINPUT,
                                        @FORBIDDENFROMUNPACKTOINPUT, 
                                        @DELFG, 
                                        @LASTUPDDT
                                        ) ";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (pd.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                    UPDATE TmDBResinLife 
                                    SET 
                                        lifefromunpacktoworkstart=@LIFEFROMUNPACKTOWORKSTART, 
                                        lifefromunpacktoworkend=@LIFEFROMUNPACKTOWORKEND, 
                                        lifefrominput=@LIFEFROMINPUT, 
                                        forbiddenfromunpacktoinput = @FORBIDDENFROMUNPACKTOINPUT, 
                                        delfg=@DELFG, 
                                        lastupddt=@LASTUPDDT
                                    WHERE typecd=@TYPECD AND resinmaterialcd=@RESINMATERIALCD ";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

		private static void deleteDBResinLife(Material.ResinLife[] armsList, DBResinLife[] nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pResinMaterialCd = cmd.Parameters.Add("@RESINMATERIALCD", SqlDbType.NVarChar);

				foreach (Material.ResinLife data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.ResinMaterialCd == data.ResinMaterialCd))
					{
						continue;
					}

					string sql = " DELETE FROM TmDBResinLife WHERE typecd = @TYPECD AND resinmaterialcd = @RESINMATERIALCD ";

					pTypeCd.Value = data.TypeCd;
					pResinMaterialCd.Value = data.ResinMaterialCd;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}

		}
    }
}
