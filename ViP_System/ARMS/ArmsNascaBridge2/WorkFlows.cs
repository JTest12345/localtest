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
    public class WorkFlows
    {
		/// <summary>
		/// SLS1で指図の第1作業で保管場所移動しないようにする番号
		/// この番号の場合、OrderMoveの番号に最終作業であれば5、そうでない場合は4を設定する
		/// </summary>
		public const int ORDERMOVE_TIMINGCHANGE_NO = 5;
		public const int ORDERMOVE_WAIT_NO = 4;
		public const int ORDERMOVE_RESTART_NO = 5;

        public int FId { get; set; }
        public int FGroupId { get; set; }
        public string WorkCd { get; set; }
        public int WorkNo { get; set; }
		public int ProcNo { get; set; }
		public int OrderMove { get; set; } 
        public string TypeCd { get; set; }
        public string MagazineDivKb { get; set; }
        public string DbDiceClassKb { get; set; }
        public bool DelFg { get; set; }
        public DateTime LastUpdDt { get; set; }

        public static bool Import()
        {
            try
            {
                WorkFlows[] nascaList = getNascaData();
                string[] types = nascaList.Select(w => w.TypeCd).Distinct().ToArray();

                foreach (string type in types)
                {
                    update(type, nascaList);

                    WorkFlows[] armsList = getData(type);
                    delete(armsList, nascaList);
                }
                return true;
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] WorkFlows Error:" + err.ToString());
                return false;
            }
        }

		private static WorkFlows[] getData(string typecd) 
		{
			List<WorkFlows> retv = new List<WorkFlows>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, workorder, procno, ordermove, magdevidestatus, delfg, lastupddt
								FROM TmWorkFlow WITH (nolock)
								WHERE typecd = @TYPECD ";

				cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typecd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						WorkFlows wf = new WorkFlows();

						wf.TypeCd = typecd;
						wf.WorkNo = SQLite.ParseInt(rd["workorder"]);
						wf.ProcNo = SQLite.ParseInt(rd["procno"]);
						wf.OrderMove = SQLite.ParseInt(rd["ordermove"]);
						wf.MagazineDivKb = SQLite.ParseInt(rd["magdevidestatus"]).ToString();
						wf.DelFg = SQLite.ParseBool(rd["delfg"]);
						wf.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);
						Process p = Process.GetProcess(wf.ProcNo);
						if (p == null) 
						{
							throw new Exception(string.Format("[TmWorkFlow 連携失敗]TmProcessに存在しないProcNoです。ProcNo:{0}", wf.ProcNo));
						}
						wf.DbDiceClassKb = p.DiceClassCd;
						wf.WorkCd = p.WorkCd;
						retv.Add(wf);
					}
				}
			}

			return retv.ToArray();
		}

        private static WorkFlows[] getNascaData() 
        {
            List<WorkFlows> workflows = new List<WorkFlows>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT RTMNFORMGROUP.fcode, RTMNFORMGROUP.fgroup_cd, RvmWORKFLOW.work_no, RvmWORKFLOW.work_cd, NtmSGHJ.MagazineDiv_KB, NtmSGHJ.DbDiceClass_KB, RvmMCONV.mtralbase_cd, RvmWORKFLOW.del_fg, RvmWORKFLOW.lastupd_dt
                                FROM dbo.NtmHMGP WITH (nolock) INNER JOIN
                        dbo.NtmHMGK WITH (nolock) ON dbo.NtmHMGP.MateGroup_CD = dbo.NtmHMGK.MateGroup_CD INNER JOIN
                        dbo.NtmSCKN WITH (nolock) ON dbo.NtmHMGP.Reference_CD = dbo.NtmSCKN.Reference_CD INNER JOIN
                        dbo.RvmWORKFLOW AS RvmWORKFLOW WITH (nolock) INNER JOIN
                        dbo.RvmMCONV AS RvmMCONV WITH (nolock) ON RvmWORKFLOW.material_cd = RvmMCONV.material_cd INNER JOIN
                        ROOTSDB.dbo.RTMNFORMGROUP AS RTMNFORMGROUP WITH (nolock) ON RvmMCONV.workcond_cd = RTMNFORMGROUP.fcode ON 
                        dbo.NtmHMGK.Material_CD = RvmMCONV.material_cd LEFT OUTER JOIN
                        dbo.NtmSGHJ AS NtmSGHJ WITH (nolock) ON RvmWORKFLOW.material_cd = NtmSGHJ.Material_CD AND RvmWORKFLOW.worklin_no = NtmSGHJ.Worklin_NO
                                WHERE (RTMNFORMGROUP.del_fg = '0') AND (RvmMCONV.del_fg = '0') AND (RvmWORKFLOW.del_fg = '0') 
								AND (NtmHMGP.Del_FG = 0) AND (NtmSCKN.Del_FG = 0)
                                AND (RTMNFORMGROUP.fgroup_cd IN ({0})) 
                                AND (RTMNFORMGROUP.fgroupclass_cd = @FGROUPCLSCD) 
								AND (NtmSCKN.section_cd = @SECTIONCD) 
								AND NOT EXISTS (SELECT MateGroup_CD FROM dbo.NtmHMGP AS NtmHMGP2 WITH(nolock) 
												WHERE NtmHMGP.Del_FG = 0 AND MateGroup_CD = 'LED0999' AND NtmHMGP.MateGroup_CD = NtmHMGP2.MateGroup_CD) ";

				cmd.Parameters.Add("@FGROUPCLSCD", SqlDbType.Int).Value = Config.Settings.NascaLineGroupCd;
				cmd.Parameters.Add("@SECTIONCD", SqlDbType.Char).Value = Config.Settings.SectionCd;

                sql = string.Format(sql, string.Join(",", Config.Settings.NascaFGroupCd));

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    int ordFId = rd.GetOrdinal("fcode");
                    int ordFGroupId = rd.GetOrdinal("fgroup_cd");
                    int ordTypeCD = rd.GetOrdinal("mtralbase_cd");
                    int ordWorkNO = rd.GetOrdinal("work_no");
                    int ordWorkCD = rd.GetOrdinal("work_cd");
                    int ordMagazineDivKB = rd.GetOrdinal("MagazineDiv_KB");
                    int ordDbDiceClassKB = rd.GetOrdinal("DbDiceClass_KB");
                    int ordDelFG = rd.GetOrdinal("del_fg");
                    int ordLastUpdDT = rd.GetOrdinal("lastupd_dt");

                    while (rd.Read())
                    {
                        WorkFlows workflow = new WorkFlows();

                        workflow.FId = rd.GetInt32(ordFId);
                        workflow.FGroupId = rd.GetInt32(ordFGroupId);
                        workflow.TypeCd = rd.GetString(ordTypeCD).Trim();
                        workflow.WorkNo = rd.GetInt32(ordWorkNO);
                        workflow.WorkCd = rd.GetString(ordWorkCD).Trim();
                        if (!rd.IsDBNull(ordMagazineDivKB))
                        {
                            workflow.MagazineDivKb = rd.GetString(ordMagazineDivKB).Trim();
                        }
                        if (!rd.IsDBNull(ordDbDiceClassKB))
                        {
                            workflow.DbDiceClassKb = rd.GetString(ordDbDiceClassKB).Trim();
                        }
                        workflow.DelFg = SQLite.ParseBool(rd[ordDelFG]);
                        workflow.LastUpdDt = rd.GetDateTime(ordLastUpdDT);

                        workflows.Add(workflow);
                    }
                }
            }

            return workflows.ToArray();
        }

        private static void update(string typecd, WorkFlows[] workflows)
        {
            //TypeCdの条件で絞った後、TypeCd, FId, WorkNoの順に並び変えて、順にworkOrderを付けていく
            workflows = workflows.Where(w => w.TypeCd == typecd)
                .OrderBy(w => w.TypeCd)
                .ThenBy(w => w.FId)
                .ThenBy(w => w.WorkNo).ToArray();

            int workOrder = 1;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pWorkOrder = cmd.Parameters.Add("@WORKORDER", SqlDbType.Int);
                SqlParameter pProcNo = cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt);
                SqlParameter pOrdermove = cmd.Parameters.Add("@ORDERMOVE", SqlDbType.BigInt);
                SqlParameter pMagdevideStatus = cmd.Parameters.Add("@MAGDEVIDESTATUS", SqlDbType.Int);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                con.Open();

				// 新規作業が間に追加された時、以降の作業も合わせて更新しないといけないが、更新日時がNASCA側で変更されない為、フラグを設ける
				bool isUpdateAfterFlow = false;

                foreach (WorkFlows workflow in workflows)
                {
                    Process proc = null;
                    if (Config.Settings.DiceClassCheckWorkCd.Exists(w => w == workflow.WorkCd))
                    {
                        //DB0003,DB0007はLED,ZDを識別する必要がある為、作業CD、チップ分類CDで特定する
                        proc = Process.GetProcess(workflow.WorkCd, workflow.DbDiceClassKb);
                    }
                    else 
                    {
                        proc = Process.GetProcess(workflow.WorkCd);
                    }
                    if (proc == null) 
                    { 
                        //エラーメール
                        Log.SysLog.Error(string.Format("[TmWorkFlow 連携失敗]TmProcessに存在しないWorkCdです。WorkCd:{0}", workflow.WorkCd));
                        return;
                    }
                    IEnumerable<KeyValuePair<int, int>> orderMoves = Config.Settings.NascaOrderMoveInfo.Where(o => o.Key == workflow.FGroupId);
                    if (orderMoves.Count() == 0) 
                    {
                        //エラーメール
                        Log.SysLog.Error(string.Format("[TmWorkFlow 連携失敗]ArmsConfigに存在しないグループCDです。FGroupID:{0}", workflow.FGroupId));
                        return;
                    }

					int orderMove = 0;
					if (orderMoves.Single().Value == ORDERMOVE_TIMINGCHANGE_NO)
					{
						int workMaxOrder = workflows.Where(w => w.FGroupId == workflow.FGroupId && w.TypeCd == workflow.TypeCd).Max(w => w.WorkNo);
						if (workMaxOrder == workflow.WorkNo)
						{
							orderMove = ORDERMOVE_RESTART_NO;
						}
						else 
						{
							orderMove = ORDERMOVE_WAIT_NO;
						}
					}
					else
					{
						//工程単位(FGroupId)の一番順序が(WorkNo)若い番号のみ番号付与
						int workMinOrder = workflows.Where(w => w.FGroupId == workflow.FGroupId && w.TypeCd == workflow.TypeCd).Min(w => w.WorkNo);
						if (workMinOrder == workflow.WorkNo)
						{
							orderMove = orderMoves.Single().Value;
						}
					}

					if (string.IsNullOrEmpty(workflow.MagazineDivKb)) 
					{
						//NASCAに設定が無い場合は強制的に分割無しへ
						workflow.MagazineDivKb = "0";
					}

                    pType.Value = workflow.TypeCd;
                    pWorkOrder.Value = workOrder;
                    pProcNo.Value = proc.ProcNo;
                    pOrdermove.Value = orderMove;
					pMagdevideStatus.Value = workflow.MagazineDivKb;
                    pDelFg.Value = SQLite.SerializeBool(workflow.DelFg);
                    pLastUpdDt.Value = workflow.LastUpdDt;

                    cmd.CommandText = @" SELECT lastupddt FROM TmWorkFlow WITH(nolock) 
                                            WHERE typecd = @TYPECD AND workorder = @WORKORDER ";

                    object lastupddt = cmd.ExecuteScalar();
                    if (lastupddt == null)
                    {
                        cmd.CommandText = @" INSERT INTO TmWorkFlow(typecd, workorder, procno, ordermove, magdevidestatus, delfg, lastupddt)
                                            VALUES (@TYPECD, @WORKORDER, @PROCNO, @ORDERMOVE, @MAGDEVIDESTATUS, @DELFG, @LASTUPDDT) ";
                        cmd.ExecuteNonQuery();
                    }
                    else 
                    {
                        DateTime current = SQLite.ParseDate(lastupddt) ?? DateTime.MinValue;
                        if (workflow.LastUpdDt > current.AddSeconds(1) || isUpdateAfterFlow)
                        {
                            cmd.CommandText = @"
                                    UPDATE TmWorkFlow SET procno=@PROCNO, ordermove = @ORDERMOVE, magdevidestatus = @MAGDEVIDESTATUS, delfg=@DELFG, lastupddt=@LASTUPDDT
                                    WHERE typecd=@TYPECD AND workorder=@WORKORDER ";
                            cmd.ExecuteNonQuery();

							isUpdateAfterFlow = true;
                        }
                    }

                    workOrder++;
                }
            }
        }

		private static void delete(WorkFlows[] armsList, WorkFlows[] nascaList) 
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pWorkOrder = cmd.Parameters.Add("@WORKORDER", SqlDbType.BigInt);

                foreach (WorkFlows data in armsList)
				{
                    if (nascaList.ToList().Where(n => n.TypeCd == data.TypeCd).Count() >= data.WorkNo)
                    {
                        continue;
                    }

                    //WorkCdでの比較だとNASCA側の行が減った際の行ずれに対応できないため、総行数で判断する。
                    //if (string.IsNullOrEmpty(data.DbDiceClassKb))
                    //{
                    //	if (nascaList.ToList()
                    //			.Exists(n => n.TypeCd == data.TypeCd && n.WorkCd == data.WorkCd))
                    //	{
                    //		continue;
                    //	}
                    //}
                    //else 
                    //{
                    //	if (nascaList.ToList()
                    //			.Exists(n => n.TypeCd == data.TypeCd && n.WorkCd == data.WorkCd && n.DbDiceClassKb == data.DbDiceClassKb))
                    //	{
                    //		continue;
                    //	}
                    //}

                    string sql = " DELETE FROM TmWorkFlow WHERE typecd = @TYPECD AND workorder = @WORKORDER ";

                    pTypeCd.Value = data.TypeCd;
                    pWorkOrder.Value = data.WorkNo;

                    cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}
    }
}