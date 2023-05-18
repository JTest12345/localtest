using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using ArmsApi;
using ArmsApi.Model;

namespace ArmsNascaBridge2
{
    public class DefectCt
    {
        private const string GNLMASTER_TID = "DEFCT";

        public static bool Import()
        {
			try
			{
				DateTime start = DateTime.Now;
				string[] types = getTypes();
				foreach (string type in types)
				{
					try
					{
						HMHJ[] mst = HMHJ.GetHMHJ(type);

						foreach (HMHJ m in mst)
						{
							updateGnlMaster(m);
						}

						//2014.7.23 41移管2次で検証中
						GnlMaster[] armsList = ArmsApi.Model.GnlMaster.Search(GNLMASTER_TID, type);
						deleteGnlMaster(armsList, mst);
					}
					catch (Exception ex)
					{
						throw new Exception("不良数マスタ取得エラー:" + type + ex.ToString());
					}
				}

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] DefectCt Error:" + err.ToString());
                return false;
			}
        }

        private static void updateGnlMaster(HMHJ mst)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Transaction = con.BeginTransaction();

                try
                {
                    cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = GNLMASTER_TID;
                    cmd.Parameters.Add("@CODE", SqlDbType.NVarChar).Value = mst.TypeCd;
                    cmd.Parameters.Add("@V1", SqlDbType.NVarChar).Value = mst.ProcNo.ToString();
                    cmd.Parameters.Add("@V2", SqlDbType.NVarChar).Value = mst.DefCt.ToString();
                    cmd.Parameters.Add("@DELFG", SqlDbType.Int).Value = SQLite.SerializeBool(mst.DelFg);
                    cmd.Parameters.Add("@UPDDT", SqlDbType.DateTime).Value = mst.LastUpdDt;

                    cmd.CommandText = @"
                    SELECT updatedt FROM TMGeneral
                    WHERE tid=@TID AND code=@CODE AND val=@V1";

                    DateTime? upddt = SQLite.ParseDate(cmd.ExecuteScalar());

                    if (upddt == null)
                    {
                        cmd.CommandText = @"
                        INSERT INTO TmGeneral(tid, code, val, val2, delfg, updatedt)
                        VALUES(@TID, @CODE, @V1, @V2, @DELFG, @UPDDT)";
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                    }
                    else if (mst.LastUpdDt > upddt)
                    {
                        cmd.CommandText = @"
                        UPDATE TmGeneral SET val2 = @V2, delfg=@DELFG, updatedt=@UPDDT
                        WHERE tid=@TID AND code=@CODE AND val=@V1";
                        cmd.ExecuteNonQuery();
                        cmd.Transaction.Commit();
                    }
                }
                catch (Exception)
                {
                    cmd.Transaction.Rollback();
                    throw;
                }

            }
        }

		private static void deleteGnlMaster(GnlMaster[] armsList, HMHJ[] nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTId = cmd.Parameters.Add("@TID", SqlDbType.NVarChar);
				SqlParameter pCode = cmd.Parameters.Add("@CODE", SqlDbType.NVarChar);
				SqlParameter pVal = cmd.Parameters.Add("@VAL", SqlDbType.NVarChar);
				SqlParameter pVal2 = cmd.Parameters.Add("@VAL2", SqlDbType.NVarChar);

				foreach (GnlMaster data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.Code && n.ProcNo == int.Parse(data.Val) && n.DefCt == int.Parse(data.Val2)))
					{
						continue;
					}

					string sql = " DELETE FROM TmGeneral WHERE tid = @TID AND code = @CODE AND val = @VAL AND val2 = @VAL2 ";

					pTId.Value = GNLMASTER_TID;
					pCode.Value = data.Code;
					pVal.Value = data.Val;
					pVal2.Value = data.Val2;

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

//		private static HMHJ[] getData() 
//		{
//			List<HMHJ> retv = new List<HMHJ>();

//			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
//			using (SqlCommand cmd = con.CreateCommand())
//			{
//				con.Open();

//				string sql = @" SELECT tid, code, val, val2, delfg, updatedt
//								FROM TmGeneral WITH (nolock) WHERE tid = @TID ";

//				cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = GNLMASTER_TID;

//				cmd.CommandText = sql;
//				using (SqlDataReader rd = cmd.ExecuteReader())
//				{
//					while (rd.Read())
//					{
//						HMHJ h = new HMHJ();

//						h.TypeCd = rd["code"].ToString().Trim();
//						h.ProcNo = Convert.ToInt32(rd["val"]);
//						h.DefCt = Convert.ToInt32(rd["val2"]);
//						h.DelFg = Convert.ToBoolean(rd["delfg"]);
//						h.LastUpdDt = Convert.ToDateTime(rd["updatedt"]);

//						retv.Add(h);
//					}
//				}
//			}

//			return retv.ToArray();
//		}

        public class HMHJ
        {
            public string MaterialCd { get; set; }
            public string TypeCd { get; set; }
            public int ProcNo { get; set; }
            public int DefCt { get; set; }
            public bool DelFg { get; set; }
            public DateTime LastUpdDt { get; set; }

            #region GetHMHJ
            
            public static HMHJ[] GetHMHJ(string typecd)
            {
                List<HMHJ> retv = new List<HMHJ>();

				List<Process> workFlows = Process.GetWorkFlow(typecd).ToList();

                using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();
                    cmd.CommandText = @"
                        SELECT h.material_cd, h.inspect_kb, h.inspect_ct, h.del_fg, h.lastupd_dt 
						FROM ntmhmhj h (NOLOCK)
						inner join ROOTSDB.dbo.RtmMCONV m (NOLOCK) ON h.Material_CD = m.material_cd 
						inner join ROOTSDB.dbo.RTMNFORMGROUP fg (NOLOCK) ON m.workcond_cd = fg.fcode
                        WHERE h.del_fg = 0 AND fg.fgroupclass_cd = @FGROUPCLSCD AND m.mtralbase_cd = @TYPECD ";

					//material_cd LIKE @MATCD

                    //cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = typecd + "." + Config.Settings.MaterialCodeSurfix;
					cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typecd;

                    //2016.03.01 品目統合一時対応
                    int nascaLineGroupCd = Config.Settings.NascaLineGroupCd;
                    if (Config.Settings.UnificationTargetTypeList.Exists(r => r == typecd))
                    {
                        nascaLineGroupCd = FrmBridgeMain.NASCA_LINE_GROUP_CODE_A;
                    }

                    cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = nascaLineGroupCd;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            //品目コードからNASCAのワークフロー取得
                            string matcd = SQLite.ParseString(rd["material_cd"]).Trim();
                            string[] workcdList = getNascaWorkCdList(matcd);

                            //対象の全ワークフローの工程に対してマスタ設定
                            foreach (string workcd in workcdList)
                            {
                                //Process p = Process.GetProcess(workcd);

                                Process[] proclist = Process.SearchProcess(null, workcd, null, false);
                                foreach (Process proc in proclist)
                                {
                                    if (!workFlows.Exists(f => f.ProcNo == proc.ProcNo)) 
                                    {
                                        continue;
                                    }
                                    //if (p == null) continue;

                                    HMHJ m = new HMHJ
                                    {
                                        MaterialCd = matcd,
                                        TypeCd = typecd,
                                        DefCt = SQLite.ParseInt(rd["inspect_ct"]),
                                        ProcNo = proc.ProcNo,
                                        DelFg = SQLite.ParseBool(rd["del_fg"]),
                                        LastUpdDt = SQLite.ParseDate(rd["lastupd_dt"]) ?? DateTime.MinValue
                                    };

                                    retv.Add(m);
                                }
                            }

                        }
                    }
                }

                return retv.ToArray();
            }

            #endregion

            #region getNascaWorkCdList

            /// <summary>
            /// 品目コードに関連付いた作業コードをRvmWorkFlowから取得
            /// </summary>
            /// <param name="materialCd"></param>
            /// <returns></returns>
            private static string[] getNascaWorkCdList(string materialCd)
            {
                try
                {
                    List<string> retv = new List<string>();

                    using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
                    using (SqlCommand cmd = con.CreateCommand())
                    {
                        con.Open();
                        cmd.CommandText = @"
                             select work_cd from  Rvmworkflow
                             where del_fg = 0 and material_cd = @MATCD";

                        cmd.Parameters.Add("@MATCD", SqlDbType.Char).Value = materialCd;

                        using (SqlDataReader rd = cmd.ExecuteReader())
                        {
                            while (rd.Read())
                            {
                                retv.Add(SQLite.ParseString(rd["work_cd"]).Trim());
                            }
                        }
                    }

                    return retv.ToArray();
                }
                catch(Exception ex)
                {
                    Log.SysLog.Error("NASCA　WorkCd取得エラー:" + materialCd);
                    return new string[0];
                }
            }
            #endregion
        }
    }
}
