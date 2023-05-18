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
	/// PDA条件マスタ取り込み
	/// 2015.1.1 3in1改修 WorkCD, PlantCD, Material2CD取り込み追加 26取り込み追加
	/// </summary>
    public class TypeCondition
    {
        public string TypeCd { get; set; }
        public string CondCd { get; set; }
        public int LineNo { get; set; }
        public string CondVal { get; set; }
        public bool DelFg { get; set; }
        public DateTime LastUpdDt { get; set; }
		public string WorkCd { get; set; }
		public string PlantCd { get; set; }
		public string Material2Cd { get; set; }


        public static bool Import()
        {
			try
			{
				string[] types = getTypes();
				foreach (string type in types)
				{
					TypeCondition[] nascaList = getPDJK(type);
					updateTypeCondition(nascaList);

					TypeCondition[] armsList = getData(type);
					deleteTypeCondition(armsList, nascaList);
				}

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] TypeCondition Error : " + err.ToString());
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

        private static TypeCondition[] getPDJK(string typecd)
        {
            List<TypeCondition> retv = new List<TypeCondition>();

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
                        p.Condition_KB
                        , p.ConditionLin_NO
                        , p.General_VAL
                        , p.Del_FG
                        , p.LastUpd_DT
                        , p.Work_CD
                        , p.Plant_CD
						, p.Material2_CD
						, p.LotChar_CD
                        FROM NtmPDJK p (NOLOCK)
						inner join ROOTSDB.dbo.RtmMCONV m (NOLOCK) ON p.Material_CD = m.material_cd 
						inner join ROOTSDB.dbo.RTMNFORMGROUP fg (NOLOCK) ON m.workcond_cd = fg.fcode
                        WHERE p.Condition_KB in (3, 8, 15, 26, 28) AND fg.fgroupclass_cd = @FGROUPCLSCD AND m.mtralbase_cd = @TYPECD ";
						//3=オーブン硬化条件 8=プラズマ洗浄値/リング設備 15=カサ/メタルマスク/製造プログラム条件
						//26=資材投入条件 28=遠心沈降機(プログラムNo)

				//Material_cd LIKE @MATCD

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        TypeCondition tc = new TypeCondition();

                        tc.TypeCd = typecd;
                        tc.CondCd = rd["Condition_KB"].ToString().Trim();

						// 2015.7.28 オーブン硬化条件取得方法変更
						//if (tc.CondCd == "3" && rd["LotChar_CD"].ToString().Trim() == LotChar.AUTOLINEOVENPROF_LOTCHARCD)
						//{
						//	tc.CondCd = "OVENPROF";
						//}

                        tc.LineNo = Convert.ToInt32(rd["ConditionLin_NO"]);
                        tc.CondVal = rd["General_VAL"].ToString().Trim();
                        tc.DelFg = SQLite.ParseBool(rd["Del_fg"]);
                        tc.LastUpdDt = Convert.ToDateTime(rd["LastUpd_DT"]);
						tc.WorkCd = rd["Work_CD"].ToString().Trim();
						tc.PlantCd = rd["Plant_CD"].ToString().Trim();
						tc.Material2Cd = rd["Material2_CD"].ToString().Trim();

                        retv.Add(tc);
                    }
                }
            }

            return retv.ToArray();
        }

		private static TypeCondition[] getData(string typecd) 
		{
			List<TypeCondition> retv = new List<TypeCondition>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, condcd, [lineno], condval, delfg, lastupddt, workcd, plantcd, materialcd
									FROM TmTypeCond WITH (nolock)
									WHERE typecd = @TYPECD AND condcd in ('3', '8', '15', '26', '28') ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

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
						tc.PlantCd = rd["plantcd"].ToString().Trim();
						tc.Material2Cd = rd["materialcd"].ToString().Trim();
						retv.Add(tc);
					}
				}
			}

			return retv.ToArray();
		}

        private static void updateTypeCondition(TypeCondition[] tclist)
        {
			if (tclist.Count() == 0) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pCondCd = cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar);
                SqlParameter pLineNo = cmd.Parameters.Add("@LINENO", SqlDbType.BigInt);
                SqlParameter pCondVal = cmd.Parameters.Add("@CONDVAL", SqlDbType.NVarChar);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);
				SqlParameter pWorkCd = cmd.Parameters.Add("@WORKCD", SqlDbType.NVarChar);
				SqlParameter pPlantCd = cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar);
				SqlParameter pMatCd = cmd.Parameters.Add("@MATERIALCD", SqlDbType.NVarChar);// 3in1対応 n.yoshimoto 2015/1/14

                con.Open();

                foreach (TypeCondition tc in tclist)
                {
                    pType.Value = tc.TypeCd;
                    pCondCd.Value = tc.CondCd;
                    pLineNo.Value = tc.LineNo;
                    pCondVal.Value = tc.CondVal;
                    pDelFg.Value = SQLite.SerializeBool(tc.DelFg);
                    pLastUpdDt.Value = tc.LastUpdDt;

					// 3in1対応 n.yoshimoto 2015/1/14
					if (tc.WorkCd == "0" || string.IsNullOrEmpty(tc.WorkCd))
					{ 
						pWorkCd.Value = DBNull.Value; 
					}
					else
					{
						pWorkCd.Value = tc.WorkCd;
					}
					if (tc.PlantCd == "0" || string.IsNullOrEmpty(tc.PlantCd))
					{
						pPlantCd.Value = DBNull.Value;
					}
					else
					{
						pPlantCd.Value = tc.PlantCd;
					}
					if (tc.Material2Cd == "0" || string.IsNullOrEmpty(tc.Material2Cd))
					{
						pMatCd.Value = DBNull.Value;
					}
					else
					{
						pMatCd.Value = tc.Material2Cd;
					}

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TmTypeCond
                            WHERE typecd=@TYPECD AND condcd=@CONDCD AND [lineno]=@LINENO ";

                    object objlastupd = cmd.ExecuteScalar();
                    if (objlastupd == null)
					{// 3in1対応 n.yoshimoto 2015/1/14
                        cmd.CommandText = @"
                                INSERT INTO TmTypeCond(typecd, condcd, [lineno], condval, delfg, lastupddt, workcd, plantcd, materialcd)
                                VALUES (@TYPECD, @CONDCD, @LINENO, @CONDVAL, @DELFG, @LASTUPDDT, @WORKCD, @PLANTCD, @MATERIALCD) ";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (tc.LastUpdDt > current.AddSeconds(1))
                        {// 3in1対応 n.yoshimoto 2015/1/14
                            cmd.CommandText = @"
                                    UPDATE TmTypeCond SET condval=@CONDVAL, delfg=@DELFG, lastupddt=@LASTUPDDT, workcd=@WORKCD, plantcd=@PLANTCD, materialcd=@MATERIALCD 
                                    WHERE typecd=@TYPECD AND condcd=@CONDCD AND [lineno]=@LINENO ";
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

		private static void deleteTypeCondition(TypeCondition[] armsList, TypeCondition[] nascaList) 
		{
			if (armsList.Count() == 0) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pCondCd = cmd.Parameters.Add("@CONDCD", SqlDbType.NVarChar);
				SqlParameter pCondVal = cmd.Parameters.Add("@CONDVAL", SqlDbType.NVarChar);

				foreach (TypeCondition data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.CondCd == data.CondCd && n.CondVal == data.CondVal)) 
					{
						continue;
					}

					string sql = " DELETE FROM TmTypeCond WHERE typecd = @TYPECD AND condcd = @CONDCD AND condval = @CONDVAL ";

					pTypeCd.Value = data.TypeCd;
					pCondCd.Value = data.CondCd;
					pCondVal.Value = data.CondVal;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}
    }
}
