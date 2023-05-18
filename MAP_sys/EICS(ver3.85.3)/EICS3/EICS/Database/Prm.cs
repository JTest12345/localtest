using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;

namespace EICS.Database
{
	public class Prm
	{
		public int ParamNO { get; set; }
		public string ModelNM { get; set; }
		public string WorkCD { get; set; }
		public string ClassNM { get; set; }
		public string DieKB { get; set; }
		public string ParamNM { get; set; }
		public string ManageNM { get; set; }
		public int TimingNO { get; set; }
		public string ChangeUnitVAL { get; set; }
		public string TotalKB { get; set; }
		public bool DelFG { get; set; }
		public int? RefParamNO { get; set; }
		public string CauseAssetsNM { get; set; }
		public string CauseChipNM { get; set; }
		public int EquipManageFG { get; set; }
		public bool UnManageTrendFG { get; set; }
		public bool WithoutFileFmtFG { get; set; }

		/// <summary>
		/// 管理項目マスタ[TmPRM](要素)取得
		/// </summary>
		/// <param name="elementNM">戻り値フィールド名</param>
		/// <param name="qcParamNO">管理NO</param>
		/// <returns>フィールド値</returns>
		public static string GetCauseAssetsNM(int serverLineCD, int qcParamNO)
		{
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, serverLineCD), "System.Data.SqlClient", false))
			{
				string causeAssetsNM = string.Empty;

				string sql = @" SELECT CauseAssets_NM 
								FROM TmPRM WITH(NOLOCK) 
								WHERE (Del_FG = 0) AND (QcParam_NO = @QcParamNO) OPTION(MAXDOP 1) ";

				conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNO);

				System.Data.Common.DbDataReader rd = null;

				using (rd = conn.GetReader(sql))
				{
					int ordCauseAssetsNM = rd.GetOrdinal("CauseAssets_NM");

					while (rd.Read())
					{
						if (rd.IsDBNull(ordCauseAssetsNM) == false)
						{
							causeAssetsNM = rd.GetString(ordCauseAssetsNM);
						}
					}
				}

				return causeAssetsNM;

			}
		}

		public static List<string> GetCauseAssetsNMList(int serverLineCD, int qcParamNO)
		{
			string causeAssetsNM = GetCauseAssetsNM(serverLineCD, qcParamNO);
			List<string> causeAssetsNMList = new List<string>();

			//List<Prm> causeModelList = new List<Prm>();

			if (causeAssetsNM.Contains(","))
			{
				causeAssetsNMList = causeAssetsNM.Split(',').ToList();

				//foreach (string causeModelChip in causeAssetsNMList)
				//{
				//    causeModelList.Add(DivideModelAndChip(serverLineCD, qcParamNO, causeModelChip));
				//}
			}
			else
			{
				causeAssetsNMList.Add(causeAssetsNM);
				//causeModelList.Add(DivideModelAndChip(serverLineCD, qcParamNO, causeAssetsNM));
			}

			//return causeModelList;
			return causeAssetsNMList;
		}

		public static Prm GetData(int lineCD, int paramNo, string modelNm, string workCd)
		{
			List<Prm> prmList = new List<Prm>();

			prmList = GetListData(lineCD, paramNo, modelNm, workCd);

			if (prmList.Count == 1)
			{
				return prmList.Single();
			}
			else if (prmList.Count > 1)
			{
				throw new ApplicationException(string.Format("パラメータマスタTmPrmにキー重複があります。マスタ設定者へ確認してください。 パラメータ番号：{0} ", paramNo));
			}
			else
			{
				return null;
			}
		}

		public static List<Prm> GetListData(int lineCD, int? paramNo, string modelNm, string workCd)
		{
			List<Prm> prmList = new List<Prm>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Model_NM, TmPRM.Chip_NM, TmPRM.Class_NM, TmPRM.Die_KB, TmPRM.Parameter_NM, TmPRM.Manage_NM,
									TmPRM.Timing_NO, TmPRM.ChangeUnit_VAL, TmPRM.Total_KB, TmPRM.Del_FG, TmPRM.RefQcParam_NO, TmPRM.CauseAssets_NM,
									TmPRM.EquipManage_FG, TmPRM.UnManageTrend_FG, TmPRM.WithoutFileFmt_FG
								FROM TmPRM WITH(NOLOCK) 
								WHERE (TmPRM.Del_FG = 0) ";

				if (paramNo.HasValue)
				{
					sql += @" AND (TmPRM.QcParam_NO = @QcParamNO) ";
					conn.SetParameter("@QcParamNO", SqlDbType.Int, paramNo);
				}

				if (string.IsNullOrEmpty(modelNm) == false)
				{
					sql += @" AND (TmPRM.Model_NM = @ModelNM) ";
					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
				}

				if (string.IsNullOrEmpty(workCd) == false)
				{
					sql += @" AND (TmPRM.Chip_NM = @ChipNM) ";
					conn.SetParameter("@ChipNM", SqlDbType.VarChar, workCd);
				}

				using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
				{
					int ordParamNO = rd.GetOrdinal("QcParam_NO");
					int ordModelNM = rd.GetOrdinal("Model_NM");
					int ordChipNM = rd.GetOrdinal("Chip_NM");
					int ordClassNM = rd.GetOrdinal("Class_NM");
					int ordDieKB = rd.GetOrdinal("Die_KB");
					int ordParamNM = rd.GetOrdinal("Parameter_NM");
					int ordManageNM = rd.GetOrdinal("Manage_NM");
					int ordTimingNO = rd.GetOrdinal("Timing_NO");
					int ordChangeUnitVAL = rd.GetOrdinal("ChangeUnit_VAL");
					int ordTotalKB = rd.GetOrdinal("Total_KB");
					int ordDelFG = rd.GetOrdinal("Del_FG");
					int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
					int ordCauseAssetsNM = rd.GetOrdinal("CauseAssets_NM");
					int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
					int ordUnManageTrendFG = rd.GetOrdinal("UnManageTrend_FG");
					int ordWithoutFileFmtFG = rd.GetOrdinal("WithoutFileFmt_FG");

					while (rd.Read())
					{
						Prm prm = new Prm();

						prm.ParamNO = rd.GetInt32(ordParamNO);
						prm.ModelNM = rd.GetString(ordModelNM).Trim();
						prm.ParamNM = rd.GetString(ordParamNM).Trim();

                        if (rd.IsDBNull(ordChipNM) == false)
                        {
                            prm.WorkCD = rd.GetString(ordChipNM).Trim();
                        }
						
						prm.ManageNM = rd.GetString(ordManageNM).Trim();
						prm.TimingNO = rd.GetInt32(ordTimingNO);
						prm.ChangeUnitVAL = rd.GetString(ordChangeUnitVAL).Trim();

						if (rd.IsDBNull(ordDieKB) == false)
						{
                            prm.DieKB = rd.GetString(ordDieKB).Trim();
                        }
						else
						{
							prm.DieKB = null;
						}

						if (rd.IsDBNull(ordClassNM) == false)
						{
							prm.ClassNM = rd.GetString(ordClassNM).Trim();
						}
						else
						{
							prm.ClassNM = null;
						}

						if (rd.IsDBNull(ordTotalKB) == false)
						{
							prm.TotalKB = rd.GetString(ordTotalKB).Trim();
						}
						else
						{
							prm.TotalKB = null;
						}

						prm.DelFG = rd.GetBoolean(ordDelFG);

						if (rd.IsDBNull(ordRefQcParamNO) == false)
						{
							prm.RefParamNO = rd.GetInt32(ordRefQcParamNO);
						}
						else
						{
							prm.RefParamNO = null;
						}

						if (rd.IsDBNull(ordCauseAssetsNM) == false)
						{
							prm.CauseAssetsNM = rd.GetString(ordCauseAssetsNM);
						}
						else
						{
							prm.CauseAssetsNM = null;
						}

						prm.EquipManageFG = rd.GetInt32(ordEquipManageFG);

						prm.UnManageTrendFG = rd.GetBoolean(ordUnManageTrendFG);
						prm.WithoutFileFmtFG = rd.GetBoolean(ordWithoutFileFmtFG);
						
						prmList.Add(prm);
					}
				}
			}

			return prmList;
		}

		public static Prm GetSendPrm(int lineCD, List<int> paramNoList, string workCd)
		{
			List<Prm> sendPrmList = new List<Prm>();

			paramNoList = paramNoList.Distinct().ToList();

			foreach(int paramNo in paramNoList)
			{
				Prm prm = Prm.GetData(lineCD, paramNo, string.Empty, workCd);
				if (prm == null)
				{
					continue;
				}
				sendPrmList.Add(prm);
			}

			if (sendPrmList.Count == 1)
			{
				return sendPrmList.Single();
			}
			else if (sendPrmList.Count >= 2)
			{
				throw new ApplicationException(string.Format("パラメータが複数取得されました。マスタ設定者へ確認してください。　パラメータ番号：{0} 作業CD：{1}", string.Join(", ", paramNoList), workCd));
			}
			else
			{
				throw new ApplicationException(string.Format("パラメータ(名称or値)が見つかりませんでした。マスタ設定者へ確認してください。　パラメータ番号：{0} 作業CD：{1}", string.Join(", ", paramNoList), workCd));
			}
		}

        /// <summary>
        /// QcParamNoで指定した群の中から最大のDieKBを返す。
        /// null値と数値が混ざる可能性のある群は本関数ではエラーとする。
        /// </summary>
        /// <param name="lineCD"></param>
        /// <param name="paramNoList"></param>
        /// <param name="identifier"></param>
        /// <returns></returns>
        public static int? GetMaxDieKB(int lineCD, List<int> paramNoList, string identifier)
        {
            List<string> dieKBList = new List<string>();

            foreach (int paramNo in paramNoList)
            {
                Prm prm = GetData(lineCD, paramNo, null, identifier);
                dieKBList.Add(prm.DieKB);
            }

            if (dieKBList.Count == 0)
                return null;

            dieKBList = dieKBList.Where(d => string.IsNullOrEmpty(d) == false && d != "共通").Distinct().ToList();

            if (dieKBList.Count == 0)
                return null;

            int retv = 0;
            foreach (string dieKB in dieKBList)
            {
                if (retv < int.Parse(dieKB))
                {
                    retv = int.Parse(dieKB);
                }
            }

            return retv;
        }
    }
}
