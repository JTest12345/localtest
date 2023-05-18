using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace GEICS.Database
{
	public class Prm
	{
        public bool IsCheck { get; set; }

		public string ServerNM { get; set; }
		public string TypeGroup { get; set; }
		public int QcParamNO { get; set; }
		public string ModelNM { get; set; }
        public string ChipNM { get; set; }
		public string ClassNM { get; set; }
        public string DieKB { get; set; }
		public string ParameterNM { get; set; }
        public string ManageNM { get; set; }
        public int TimingNO { get; set; }
        public string ChangeUnitVAL { get; set; }
        public string TotalKB { get; set; }
		public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }
        public int RefQcParamNO { get; set; }
        public string CauseAssetsNM { get; set; }
		public int EquipManageFG { get; set; }
		public bool UnManageTrendFG { get; set; }
		public bool WithoutFileFmtFG { get; set; }
        public int ResinGroupManageFG { get; set; }
        public bool LimitOverWhenFlowNotPossibleFG { get; set; }
        public bool LimitOverWhenMachineNotPossibleFG { get; set; }
        public string CauseWorkCD { get; set; }
        public string CauseWorkNM { get; set; }
        public bool ProgramMaterialCdFG { get; set; }

        public static List<Prm> GetData(string serverNM, string connStr)
		{
			List<Prm> paramList = new List<Prm>();

			try
			{
				using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
				{
					string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Model_NM, TmPRM.Class_NM, TmPRM.Parameter_NM, TmPRM.Del_FG,
									TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG
                                    FROM TmPRM with(nolock) ";

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordUnManageFG = rd.GetOrdinal("UnManageTrend_FG");
						int ordWithoutFG = rd.GetOrdinal("WithoutFileFmt_FG");

						while (rd.Read())
						{
							Prm paramInfo = new Prm();
							paramInfo.ServerNM = serverNM;
							paramInfo.TypeGroup = ConnectQCIL.GetTypeGroup(serverNM);
							paramInfo.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
							paramInfo.ModelNM = rd.GetString(rd.GetOrdinal("Model_NM"));
							paramInfo.ClassNM = rd.GetString(rd.GetOrdinal("Class_NM"));
							paramInfo.ParameterNM = rd.GetString(rd.GetOrdinal("Parameter_NM"));
							paramInfo.UnManageTrendFG = rd.GetBoolean(ordUnManageFG);
							paramInfo.WithoutFileFmtFG = rd.GetBoolean(ordWithoutFG);

							paramList.Add(paramInfo);
						}
					}
				}

				return paramList;
			}
			catch (Exception err)
			{
				throw err;
			}
		}

        public static List<Prm> GetDataFromParamNo(string serverNM, string connStr, int? qcParamNo)
		{
			List<Prm> paramList = new List<Prm>();

			try
			{
				using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
				{
					string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Model_NM, TmPRM.Class_NM, TmPRM.Parameter_NM, TmPRM.Del_FG,
									TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG
                                    FROM TmPRM with(nolock) Where 1=1 ";

					if (qcParamNo.HasValue)
					{
						sql += @" AND TmPRM.QcParam_NO = @ParamNO ";
						conn.SetParameter("@ParamNO", DbType.Int32, qcParamNo.Value);
					}

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordUnManageFG = rd.GetOrdinal("UnManageTrend_FG");
						int ordWithoutFG = rd.GetOrdinal("WithoutFileFmt_FG");

						while (rd.Read())
						{
							Prm paramInfo = new Prm();
							paramInfo.ServerNM = serverNM;

							if (string.IsNullOrEmpty(serverNM) == false)
							{
								paramInfo.TypeGroup = ConnectQCIL.GetTypeGroup(serverNM);
							}
							paramInfo.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));
							paramInfo.ModelNM = rd.GetString(rd.GetOrdinal("Model_NM"));
							paramInfo.ClassNM = rd.GetString(rd.GetOrdinal("Class_NM"));
							paramInfo.ParameterNM = rd.GetString(rd.GetOrdinal("Parameter_NM"));
							paramInfo.UnManageTrendFG = rd.GetBoolean(ordUnManageFG);
							paramInfo.WithoutFileFmtFG = rd.GetBoolean(ordWithoutFG);

							paramList.Add(paramInfo);
						}
					}
				}

				return paramList;
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		public static List<Prm> GetData(string modelNM, string parameterNM, int qcParamNO)
		{
			return GetData(modelNM, parameterNM, qcParamNO, null, null);
        }

        public static List<Prm> GetData(string modelNM, string parameterNM, int qcParamNO, bool? resinGroupManage_FG, bool? progMatFg)
        {
            return GetData(modelNM, parameterNM, qcParamNO, null, resinGroupManage_FG, progMatFg);
        }

        public static List<Prm> GetData(string modelNM, string parameterNM, int qcParamNO, string classNm, bool? resinGroupManage_FG, bool? progMatFg = null)
        {
            List<Prm> paramList = new List<Prm>();

            using (DBConnect conn = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT QcParam_NO, Model_NM, Chip_NM, Class_NM, Die_KB, Parameter_NM, Manage_NM, Timing_NO, ChangeUnit_VAL, Total_KB, Del_FG, UpdUser_CD, 
                                  LastUpd_DT, RefQcParam_NO, CauseAssets_NM, EquipManage_FG, UnManageTrend_FG, WithoutFileFmt_FG, ResinGroupManage_FG,
                                  LimitOverWhenFlowNotPossible_FG, LimitOverWhenMachineNotPossible_FG, CauseWork_CD, ProgramMaterialCd_FG
                                FROM TmPRM with(nolock) 
                                WHERE 1=1 ";

				if (!string.IsNullOrEmpty(modelNM))
				{
					sql += " AND (TmPRM.Model_NM like @ModelNM) ";
					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNM + "%");
				}
                if (!string.IsNullOrEmpty(parameterNM))
                {
                    sql += " AND (TmPRM.Parameter_NM like @ParameterNM) ";
                    conn.SetParameter("@ParameterNM", SqlDbType.VarChar, parameterNM + "%");
                }
                if (qcParamNO != int.MinValue)
                {
                    sql += " AND (TmPRM.QcParam_NO = @QcParamNO) ";
                    conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);
                }
				if (!string.IsNullOrEmpty(classNm))
				{
					sql += " AND (TmPRM.Class_NM = @Class_NM) ";
					conn.SetParameter("@Class_NM", SqlDbType.VarChar, classNm);
                }
                if (resinGroupManage_FG.HasValue)
                {
                    sql += " AND (TmPRM.ResinGroupManage_FG = @ResinGroupManageFG) ";
                    conn.SetParameter("@ResinGroupManageFG", SqlDbType.Int, Common.ParseBoolToInt(resinGroupManage_FG.Value));
                }
                if (progMatFg.HasValue)
                {
                    sql += " AND (TmPRM.ProgramMaterialCd_FG = @ProgramMaterialCdFG) ";
                    conn.SetParameter("@ProgramMaterialCdFG", SqlDbType.Bit, progMatFg.Value);
                }


                using (DbDataReader rd = conn.GetReader(sql))
                {
                    int ordChipNM = rd.GetOrdinal("Chip_NM");
                    int ordDieKB = rd.GetOrdinal("Die_KB");
                    int ordTotalKB = rd.GetOrdinal("Total_KB");
                    int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
                    int ordCauseAssetsNM = rd.GetOrdinal("CauseAssets_NM");
					int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
					int ordModelNM = rd.GetOrdinal("Model_NM");
					int ordClassNM = rd.GetOrdinal("Class_NM");
					int ordParamNM = rd.GetOrdinal("Parameter_NM");
					int ordMngNM = rd.GetOrdinal("Manage_NM");
					int ordTimingNO = rd.GetOrdinal("Timing_NO");

					int ordUnManageFG = rd.GetOrdinal("UnManageTrend_FG");
					int ordWithoutFG = rd.GetOrdinal("WithoutFileFmt_FG");
                    int ordResinGroupManageFG = rd.GetOrdinal("ResinGroupManage_FG");
                    int ordLimitOverWhenFlowNotPossibleFG = rd.GetOrdinal("LimitOverWhenFlowNotPossible_FG");
                    int ordLimitOverWhenMachineNotPossibleFG = rd.GetOrdinal("LimitOverWhenMachineNotPossible_FG");
                    int ordCauseWorkCD = rd.GetOrdinal("CauseWork_CD");
                    int ordProgramMaterialCdFG = rd.GetOrdinal("ProgramMaterialCd_FG");

                    while (rd.Read())
					{
						Prm prm = new Prm();
						prm.QcParamNO = rd.GetInt32(rd.GetOrdinal("QcParam_NO"));

						if (!rd.IsDBNull(ordModelNM))
						{
							prm.ModelNM = rd.GetString(ordModelNM);
						}

						if (!rd.IsDBNull(ordChipNM))
						{
							prm.ChipNM = rd.GetString(ordChipNM);
						}

						if (!rd.IsDBNull(ordClassNM))
						{
							prm.ClassNM = rd.GetString(ordClassNM);
						}

						if (!rd.IsDBNull(ordDieKB))
						{
							prm.DieKB = rd.GetString(ordDieKB);
						}

						if (!rd.IsDBNull(ordParamNM))
						{
							prm.ParameterNM = rd.GetString(ordParamNM);
						}

						if (!rd.IsDBNull(ordMngNM))
						{
							prm.ManageNM = rd.GetString(ordMngNM);
						}

						if (!rd.IsDBNull(ordTimingNO))
						{
							prm.TimingNO = rd.GetInt32(ordTimingNO);
						}

                        prm.ChangeUnitVAL = rd.GetString(rd.GetOrdinal("ChangeUnit_VAL"));
						prm.EquipManageFG = rd.GetInt32(ordEquipManageFG);

                        if (!rd.IsDBNull(ordTotalKB))
                        {
                            prm.TotalKB = rd.GetString(ordTotalKB);
                        }

                        prm.DelFG = rd.GetBoolean(rd.GetOrdinal("Del_FG"));
                        prm.UpdUserCD = rd.GetString(rd.GetOrdinal("UpdUser_CD"));
                        prm.LastUpdDT = rd.GetDateTime(rd.GetOrdinal("LastUpd_DT"));

                        if (!rd.IsDBNull(ordRefQcParamNO))
                        {
                            prm.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
                        }

                        if (!rd.IsDBNull(ordCauseAssetsNM))
                        {
                            prm.CauseAssetsNM = rd.GetString(ordCauseAssetsNM);
                        }

						prm.UnManageTrendFG = rd.GetBoolean(ordUnManageFG);
						prm.WithoutFileFmtFG = rd.GetBoolean(ordWithoutFG);
                        prm.ResinGroupManageFG = rd.GetInt32(ordResinGroupManageFG);
                        prm.LimitOverWhenFlowNotPossibleFG = rd.GetBoolean(ordLimitOverWhenFlowNotPossibleFG);
                        prm.LimitOverWhenMachineNotPossibleFG = rd.GetBoolean(ordLimitOverWhenMachineNotPossibleFG);
                        if (!rd.IsDBNull(ordCauseWorkCD))
                        {
                            prm.CauseWorkCD = rd.GetString(ordCauseWorkCD);
                        }
                        prm.ProgramMaterialCdFG = rd.GetBoolean(ordProgramMaterialCdFG);

                        paramList.Add(prm);
                    }
                }
            }

            return paramList;
        }

        public static void InsertUpdate(List<Prm> prms)
        {
            bool chgUpdUserCd = !string.IsNullOrEmpty(Constant.EmployeeInfo.EmployeeCD);
            foreach (Prm p in prms)
            {
                if (chgUpdUserCd) p.UpdUserCD = Constant.EmployeeInfo.EmployeeCD;
                InsertUpdate(p);
            }
        }
        public static void InsertUpdate(Prm prm) 
        {
            using (SqlConnection con = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" UPDATE TmPRM
                                SET Model_NM = @ModelNM, Chip_NM = @ChipNM, Class_NM = @ClassNM, Die_KB = @DieKB, Parameter_NM = @ParameterNM,
									Manage_NM = @ManageNM, Timing_NO = @TimingNO, ChangeUnit_VAL = @ChangeUnitVAL, Total_KB = @TotalKB,
									Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT, RefQcParam_NO = @RefQcParamNO,
									CauseAssets_NM = @CauseAssetsNM, EquipManage_FG = @EquipManageFG, UnManageTrend_FG = @UnManageTrendFG,
									WithoutFileFmt_FG = @WithoutFileFmtFG, ResinGroupManage_FG = @ResinGroupManageFG,
                                    LimitOverWhenFlowNotPossible_FG = @LimitOverWhenFlowNotPossibleFG, 
                                    LimitOverWhenMachineNotPossible_FG = @LimitOverWhenMachineNotPossibleFG,
                                    CauseWork_CD = @CauseWorkCD
                                WHERE QcParam_NO = @QcParamNO
                                INSERT INTO TmPRM (QcParam_NO, Model_NM, Chip_NM, Class_NM, Die_KB, Parameter_NM, Manage_NM, Timing_NO, ChangeUnit_VAL, Total_KB, 
                                                    Del_FG, UpdUser_CD, LastUpd_DT, RefQcParam_NO, CauseAssets_NM, EquipManage_FG, UnManageTrend_FG, WithoutFileFmt_FG,
                                                    ResinGroupManage_FG, LimitOverWhenFlowNotPossible_FG, LimitOverWhenMachineNotPossible_FG, CauseWork_CD)
                                SELECT @QcParamNO, @ModelNM, @ChipNM, @ClassNM, @DieKB, @ParameterNM, @ManageNM, @TimingNO, @ChangeUnitVAL, @TotalKB, 
                                        @DelFG, @UpdUserCD, @LastUpdDT, @RefQcParamNO, @CauseAssetsNM, @EquipManageFG, @UnManageTrendFG, @WithoutFileFmtFG,
                                        @ResinGroupManageFG, @LimitOverWhenFlowNotPossibleFG, @LimitOverWhenMachineNotPossibleFG, @CauseWorkCD
                                WHERE NOT EXISTS 
                                (SELECT * FROM TmPRM
                                 WHERE QcParam_NO = @QcParamNO) ";
                cmd.CommandText = sql;

                cmd.Parameters.Add("@QcParamNO", SqlDbType.Int).Value = prm.QcParamNO;
                cmd.Parameters.Add("@ModelNM", SqlDbType.VarChar).Value = prm.ModelNM;
				cmd.Parameters.Add("@ChipNM", SqlDbType.VarChar).Value = prm.ChipNM ?? (object)DBNull.Value; ;
                cmd.Parameters.Add("@ClassNM", SqlDbType.VarChar).Value = prm.ClassNM;
				cmd.Parameters.Add("@DieKB", SqlDbType.Char).Value = prm.DieKB ?? (object)DBNull.Value;
                cmd.Parameters.Add("@ParameterNM", SqlDbType.VarChar).Value = prm.ParameterNM;
                cmd.Parameters.Add("@ManageNM", SqlDbType.VarChar).Value = prm.ManageNM;
                cmd.Parameters.Add("@TimingNO", SqlDbType.Int).Value = prm.TimingNO;
                cmd.Parameters.Add("@ChangeUnitVAL", SqlDbType.Char).Value = prm.ChangeUnitVAL;
                cmd.Parameters.Add("@TotalKB", SqlDbType.Char).Value = prm.TotalKB ?? (object)DBNull.Value;
                cmd.Parameters.Add("@DelFG", SqlDbType.Bit).Value = prm.DelFG;
                cmd.Parameters.Add("@UpdUserCD", SqlDbType.Char).Value = prm.UpdUserCD;
                cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = System.DateTime.Now;
				cmd.Parameters.Add("@UnManageTrendFG", SqlDbType.Bit).Value = prm.UnManageTrendFG;
				cmd.Parameters.Add("@WithoutFileFmtFG", SqlDbType.Bit).Value = prm.WithoutFileFmtFG;

				if (prm.EquipManageFG != 0)
				{
					prm.EquipManageFG = 1;
				}
				cmd.Parameters.Add("@EquipManageFG", SqlDbType.Int).Value = prm.EquipManageFG;

                if (prm.ResinGroupManageFG != 0)
                {
                    prm.ResinGroupManageFG = 1;
                }
                cmd.Parameters.Add("@ResinGroupManageFG", SqlDbType.Int).Value = prm.ResinGroupManageFG;

                //SqlParameter paramRefQcParamNO = cmd.Parameters.Add("@RefQcParamNO", SqlDbType.Int);
                if (prm.RefQcParamNO == 0)
				{
					cmd.Parameters.Add("@RefQcParamNO", SqlDbType.Int).Value = DBNull.Value;
					//paramRefQcParamNO.Value = DBNull.Value;
				}
                else
                {
					cmd.Parameters.Add("@RefQcParamNO", SqlDbType.Int).Value = prm.RefQcParamNO;
					//paramRefQcParamNO.Value = prm.RefQcParamNO;
                }

                cmd.Parameters.Add("@CauseAssetsNM", SqlDbType.NVarChar).Value = prm.CauseAssetsNM ?? (object)DBNull.Value;
                cmd.Parameters.Add("@LimitOverWhenFlowNotPossibleFG", SqlDbType.Bit).Value = prm.LimitOverWhenMachineNotPossibleFG;
                cmd.Parameters.Add("@LimitOverWhenMachineNotPossibleFG", SqlDbType.Bit).Value = prm.LimitOverWhenMachineNotPossibleFG;
                cmd.Parameters.Add("@CauseWorkCD", SqlDbType.NVarChar).Value = prm.CauseWorkCD ?? (object)DBNull.Value;

                cmd.ExecuteNonQuery();
            }
        }
	}

    public class PrmInfo 
    {
        public string Info1 { get; set; }
        public string Info2 { get; set; }
        public string Info3 { get; set; }

        public static PrmInfo GetData(int qcparamno)
        {
            PrmInfo prm = null;

            using (SqlConnection con = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT Info1_NM, Info2_NM, Info3_NM FROM TmPRMInfo WITH(nolock)
                                WHERE QcParam_NO = @QcParamNO ";

                cmd.Parameters.Add("QcParamNO", SqlDbType.Int).Value = qcparamno;

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader()) 
                {
                    int ordInfo1NM = rd.GetOrdinal("Info1_NM");
                    int ordInfo2NM = rd.GetOrdinal("Info2_NM");
                    int ordInfo3NM = rd.GetOrdinal("Info3_NM");

                    while(rd.Read())
                    {
                        prm = new PrmInfo();

                        prm.Info1 = rd.GetString(ordInfo1NM);
                        prm.Info2 = rd.GetString(ordInfo2NM);
                        prm.Info3 = rd.GetString(ordInfo3NM);
                    }
                }
            }
            return prm;
        }
    }

    public class PrmSummary 
    {
        public bool ChangeFG { get; set; }

        public int QcParamNO { get; set; }
        public string TypeCD { get; set; }
        public string SummaryKB { get; set; }
        public int AnyRowCT { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }

        public int OldQcParamNO { get; set; }
        public string OldTypeCD { get; set; }

        public enum Summary 
        {
            All,
            Head,
            HeadOut,
        }
        
        public static List<PrmSummary> GetData(List<int> qcParamNo, List<string> typeCd, bool includeDelete)
        {
            List<PrmSummary> summaryList = new List<PrmSummary>();

            using (SqlConnection conn = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

				string sql = @" SELECT QcParam_NO, Type_CD, Summary_KB, Data_CT, Del_FG, UpdUser_CD, LastUpd_DT 
                                FROM TmPRMSUMMARY WITH(nolock) 
                                WHERE 1=1 ";

                if (includeDelete == false) 
                {
                    sql += " AND Del_FG = 0 ";
                }

                if (qcParamNo.Count != 0) 
                {
                    sql += string.Format(" AND QcParam_NO in ({0}) "
                        , string.Join(", ", qcParamNo.ConvertAll(q => q.ToString()).ToArray()));
                }

                if (typeCd.Count != 0)
                {
                    sql += string.Format(" AND Type_CD in ({0}) "
                        , string.Join(",", typeCd.Select(t => string.Format("'{0}'", t)).ToArray()));
                }

                cmd.CommandText = sql;

                using (SqlDataReader rd = cmd.ExecuteReader()) 
                {
                    while (rd.Read()) 
                    {
                        PrmSummary summary = new PrmSummary();

                        summary.QcParamNO =  Convert.ToInt32(rd["QcParam_NO"]);
                        summary.TypeCD = rd["Type_CD"].ToString().Trim();
                        summary.SummaryKB = rd["Summary_KB"].ToString().Trim();
						summary.AnyRowCT = Convert.ToInt32(rd["Data_CT"]);
                        summary.DelFG = Convert.ToBoolean(rd["Del_FG"]);
                        summary.UpdUserCD = rd["UpdUser_CD"].ToString().Trim();
                        summary.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);

                        summary.OldTypeCD = summary.TypeCD;
                        summary.OldQcParamNO = summary.QcParamNO;

                        summaryList.Add(summary);
                    }   
                }
            }

            return summaryList;
        }

        public static void InsertUpdate(PrmSummary data) 
        {
            if (data.QcParamNO == 0 || string.IsNullOrEmpty(data.TypeCD))
            {
                throw new ApplicationException(string.Format(Constant.MessageInfo.Message_106, data.TypeCD, data.QcParamNO));
            }

            using (SqlConnection conn = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                string sql = @" UPDATE TmPRMSUMMARY SET
                                    Type_CD = @TypeCD, QcParam_NO = @QcParamNO, Summary_KB = @SummaryKB, Data_CT = @DataCT,
                                    Del_FG = @DelFG, UpdUser_CD = @UpdUserCD, LastUpd_DT = @LastUpdDT
                                WHERE Type_CD = @OldTypeCD AND QcParam_NO = @OldQcParamNO
                                INSERT INTO TmPRMSUMMARY (Type_CD, QcParam_NO, Summary_KB, Data_CT, UpdUser_CD)
                                    SELECT @TypeCD, @QcParamNO, @SummaryKB, @AnyRowCT, @UpdUserCD
                                    WHERE NOT EXISTS (SELECT * FROM TmPRMSUMMARY WHERE Type_CD = @TypeCD AND QcParam_NO = @QcParamNO) ";

                cmd.Parameters.Add("QcParamNO", SqlDbType.Int).Value = data.QcParamNO;
                cmd.Parameters.Add("@TypeCD", SqlDbType.Char).Value = data.TypeCD;
                cmd.Parameters.Add("@SummaryKB", SqlDbType.NVarChar).Value = data.SummaryKB;
				cmd.Parameters.Add("@DataCT", SqlDbType.Int).Value = data.AnyRowCT;
                cmd.Parameters.Add("@DelFG", SqlDbType.Bit).Value = data.DelFG;
                cmd.Parameters.Add("@UpdUserCD", SqlDbType.NVarChar).Value = data.UpdUserCD;
                cmd.Parameters.Add("@LastUpdDT", SqlDbType.DateTime).Value = System.DateTime.Now;

                cmd.Parameters.Add("@OldTypeCD", SqlDbType.NVarChar).Value = data.OldTypeCD;
                cmd.Parameters.Add("@OldQcParamNO", SqlDbType.Int).Value = data.OldQcParamNO;

                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
