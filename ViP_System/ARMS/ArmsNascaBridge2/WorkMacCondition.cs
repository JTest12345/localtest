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
	/// 作業設備マスタ取り込み
	/// </summary>
	public class WorkMacCondition
	{
		public string PlantCd { get; set; }
		public string TypeCd { get; set; }
		public int ProcNo { get; set; }
		public bool DelFg { get; set; }
		public DateTime LastUpdDt { get; set; }

		public static bool Import()
		{
			try
			{
				MachineInfo[] machines = MachineInfo.GetMachineList(false);
				foreach (MachineInfo mac in machines)
				{
					List<WorkMacCondition> nascaList = getNascaData(mac.NascaPlantCd);
					update(nascaList);

					List<WorkMacCondition> armsList = getData(mac.NascaPlantCd);
					delete(armsList, nascaList);


                    // 2016.3.25湯浅追加　作業設備マスタに禁止設備設定が追加されたため、関数を追加。
                    List<WorkMacCondition> nascaListDeny = getNascaDataDenyMacType(mac.NascaPlantCd);
                    updateDenyMacType(nascaListDeny);

                    List<WorkMacCondition> armsListDeny = getDataDenyMacType(mac.NascaPlantCd);
                    deleteDenyMacType(armsListDeny, nascaListDeny);
				}

                return true;
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] WorkMacCondition Error:" + err.ToString());
                return false;
			}
		}

		private static List<WorkMacCondition> getNascaData(string plantCd)
		{
			List<WorkMacCondition> retv = new List<WorkMacCondition>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

                // 2016.3.25湯浅追加：NtmSGSTが投入可能設備と禁止設備設定を兼ねるようになった為、WorkPlantKbnでフィルタリング。(可能は1)
				string sql = @" SELECT Plant_CD, Material_CD, Work_CD, Reference_CD, Del_FG, UpdUser_CD, LastUpd_DT
								FROM dbo.NtmSGST WITH (nolock) 
								WHERE (Plant_CD = @PLANTCD)
                                AND (WorkPlantKbn = 1)";

				cmd.Parameters.Add("@PLANTCD", SqlDbType.Char).Value = plantCd;

				cmd.CommandText = sql;

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
                        string materialCd = rd["Material_CD"].ToString().Trim();
                        if (materialCd.IndexOf(".") == -1)
                        {   
                            // 2016.3.25湯浅コメントアウト（特に該当エラーで不具合にならないし誰も気にしていないため）
                            // Log.SysLog.Info(string.Format("品目CDに加工状態が付いていません。 MaterialCD:{0}", materialCd));
                            continue;
                        }

                        // -- 2015/9/15 永尾追加 同一のWork_CDで複数のProcNoがある時、先頭のProcNoしか取り込まない不具合を修正。
                        // -- GetProcessList関数を新規作成
                        string workcd = rd["Work_CD"].ToString().Trim();
                        Process[] pList = Process.GetProcessList(workcd);
                        if (pList.Length == 0)
                        {
                            Log.SysLog.Info(string.Format("ARMSで存在しない作業CDです。 MaterialCD:{0} PlantCD:{1} WorkCD:{2}", materialCd, rd["Plant_CD"].ToString().Trim(), workcd));
                            continue;
                        }

                        
                        foreach (Process p in pList)
                        {
                            WorkMacCondition c = new WorkMacCondition();
                            c.PlantCd = rd["Plant_CD"].ToString().Trim();
                            c.TypeCd = materialCd.Substring(0, materialCd.IndexOf("."));

                            c.DelFg = SQLite.ParseBool(rd["del_fg"]);
                            c.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);
                            c.ProcNo = p.ProcNo;
                            retv.Add(c);
                        }
					}
				}
			}

			return retv;
		}

		private static List<WorkMacCondition> getData(string plantCd)
		{
			List<WorkMacCondition> retv = new List<WorkMacCondition>();

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT typecd, procno, plantcd, delfg, lastupddt
								FROM TmWorkMacCond WITH (nolock) WHERE plantcd = @PLANTCD ";

				cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						WorkMacCondition d = new WorkMacCondition();

						d.TypeCd = rd["typecd"].ToString().Trim();
						d.ProcNo = SQLite.ParseInt(rd["procno"]);
						d.PlantCd = rd["plantcd"].ToString().Trim();
						d.DelFg = SQLite.ParseBool(rd["delfg"]);
						d.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

						retv.Add(d);
					}
				}
			}

			return retv;
		}

		private static void update(List<WorkMacCondition> nascaList)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pPlantCd = cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar);
				SqlParameter pProcNo = cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt);
				SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
				SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

				con.Open();

				foreach (WorkMacCondition nasca in nascaList)
				{
					pType.Value = nasca.TypeCd;
					pPlantCd.Value = nasca.PlantCd;
					pProcNo.Value = nasca.ProcNo;
					pDelFg.Value = SQLite.SerializeBool(nasca.DelFg);
					pLastUpdDt.Value = nasca.LastUpdDt;

					cmd.CommandText = @"
                            SELECT lastupddt FROM TmWorkMacCond
                            WHERE typecd=@TYPECD AND procno=@PROCNO AND plantcd=@PLANTCD ";

					object objlastupd = cmd.ExecuteScalar();

					if (objlastupd == null)
					{
						if (nasca.DelFg) continue;

						cmd.CommandText = @"
                                INSERT INTO TmWorkMacCond(typecd, procno, plantcd, delfg, lastupddt)
                                VALUES (@TYPECD, @PROCNO, @PLANTCD, @DELFG, @LASTUPDDT);";
						cmd.ExecuteNonQuery();
						continue;
					}
					else
					{
						DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
						if (nasca.LastUpdDt > current.AddSeconds(1))
						{
							cmd.CommandText = @"
                                      UPDATE TmWorkMacCond SET delfg=@DELFG, lastupddt=@LASTUPDDT
                                      WHERE typecd = @TYPECD AND procno = @PROCNO AND plantcd = @PLANTCD ";

							cmd.ExecuteNonQuery();
						}
					}
				}
			}

		}

		private static void delete(List<WorkMacCondition> armsList, List<WorkMacCondition> nascaList)
		{
			if (armsList.Count() == 0) return;

			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
				SqlParameter pProcNo = cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt);
				SqlParameter pPlantCd = cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar);

				foreach (WorkMacCondition data in armsList)
				{
					if (nascaList.ToList()
						.Exists(n => n.TypeCd == data.TypeCd && n.ProcNo == data.ProcNo && n.PlantCd == data.PlantCd))
					{
						continue;
					}

					string sql = " DELETE FROM TmWorkMacCond WHERE typecd = @TYPECD AND procno = @PROCNO AND plantcd = @PLANTCD ";

					pTypeCd.Value = data.TypeCd;
					pProcNo.Value = data.ProcNo;
					pPlantCd.Value = data.PlantCd;

					cmd.CommandText = sql;
					cmd.ExecuteNonQuery();
				}
			}
		}

        /// <summary>
        /// NASCAの作業設備マスタ(NtmSGST)の禁止マスタ取得関数。可能マスタと比較してworkCdがない。2016.3.25湯浅作成。
        /// </summary>
        /// <param name="plantCd"></param>
        /// <returns></returns>
        private static List<WorkMacCondition> getNascaDataDenyMacType(string plantCd)
        {
            List<WorkMacCondition> retv = new List<WorkMacCondition>();

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT Plant_CD, Material_CD, Reference_CD, Del_FG, UpdUser_CD, LastUpd_DT
								FROM dbo.NtmSGST WITH (nolock) 
								WHERE (Plant_CD = @PLANTCD)
                                AND (WorkPlantKbn = 2)";

                cmd.Parameters.Add("@PLANTCD", SqlDbType.Char).Value = plantCd;

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        string materialCd = rd["Material_CD"].ToString().Trim();
                        if (materialCd.IndexOf(".") == -1)
                        {
                            continue;
                        }

                        WorkMacCondition c = new WorkMacCondition();
                        c.PlantCd = rd["Plant_CD"].ToString().Trim();
                        c.TypeCd = materialCd.Substring(0, materialCd.IndexOf("."));

                        c.DelFg = SQLite.ParseBool(rd["del_fg"]);
                        c.LastUpdDt = Convert.ToDateTime(rd["lastupd_dt"]);
                        retv.Add(c);
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// ARMSの投入禁止マスタ(TmDenyMacType取得関数。ARMSのTmMachineの設備が対象。2016.3.25湯浅作成。
        /// </summary>
        /// <param name="plantCd"></param>
        /// <returns></returns>
        private static List<WorkMacCondition> getDataDenyMacType(string plantCd)
        {
            List<WorkMacCondition> retv = new List<WorkMacCondition>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT typecd, plantcd, delfg, lastupddt
								FROM TmDenyMacType WITH (nolock) WHERE plantcd = @PLANTCD ";

                cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar).Value = plantCd;

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        WorkMacCondition d = new WorkMacCondition();

                        d.TypeCd = rd["typecd"].ToString().Trim();
                        d.PlantCd = rd["plantcd"].ToString().Trim();
                        d.DelFg = SQLite.ParseBool(rd["delfg"]);
                        d.LastUpdDt = Convert.ToDateTime(rd["lastupddt"]);

                        retv.Add(d);
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// NASCA投入禁止マスタのARMS側更新マスタ。 2016.3.25湯浅作成。
        /// </summary>
        /// <param name="nascaList"></param>
        private static void updateDenyMacType(List<WorkMacCondition> nascaList)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                SqlParameter pType = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pPlantCd = cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar);
                SqlParameter pDelFg = cmd.Parameters.Add("@DELFG", SqlDbType.Int);
                SqlParameter pLastUpdDt = cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime);

                con.Open();

                foreach (WorkMacCondition nasca in nascaList)
                {
                    pType.Value = nasca.TypeCd;
                    pPlantCd.Value = nasca.PlantCd;
                    pDelFg.Value = SQLite.SerializeBool(nasca.DelFg);
                    pLastUpdDt.Value = nasca.LastUpdDt;

                    cmd.CommandText = @"
                            SELECT lastupddt FROM TmDenyMacType
                            WHERE typecd=@TYPECD AND plantcd=@PLANTCD ";

                    object objlastupd = cmd.ExecuteScalar();

                    if (objlastupd == null)
                    {
                        if (nasca.DelFg) continue;

                        cmd.CommandText = @"
                                INSERT INTO TmDenyMacType(typecd, plantcd, delfg, lastupddt)
                                VALUES (@TYPECD, @PLANTCD, @DELFG, @LASTUPDDT);";
                        cmd.ExecuteNonQuery();
                        continue;
                    }
                    else
                    {
                        DateTime current = SQLite.ParseDate(objlastupd) ?? DateTime.MinValue;
                        if (nasca.LastUpdDt > current.AddSeconds(1))
                        {
                            cmd.CommandText = @"
                                      UPDATE TmDenyMacType SET delfg=@DELFG, lastupddt=@LASTUPDDT
                                      WHERE typecd = @TYPECD AND plantcd = @PLANTCD ";

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 禁止マスタのNASCA側物理削除対策。2016.3.25湯浅作成。
        /// </summary>
        /// <param name="armsList"></param>
        /// <param name="nascaList"></param>
        private static void deleteDenyMacType(List<WorkMacCondition> armsList, List<WorkMacCondition> nascaList)
        {
            if (armsList.Count() == 0) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                SqlParameter pTypeCd = cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar);
                SqlParameter pPlantCd = cmd.Parameters.Add("@PLANTCD", SqlDbType.NVarChar);

                foreach (WorkMacCondition data in armsList)
                {
                    if (nascaList.ToList()
                        .Exists(n => n.TypeCd == data.TypeCd && n.PlantCd == data.PlantCd))
                    {
                        continue;
                    }

                    string sql = " DELETE FROM TmDenyMacType WHERE typecd = @TYPECD AND plantcd = @PLANTCD ";

                    pTypeCd.Value = data.TypeCd;
                    pPlantCd.Value = data.PlantCd;

                    cmd.CommandText = sql;
                    cmd.ExecuteNonQuery();
                }
            }
        }
	}
}