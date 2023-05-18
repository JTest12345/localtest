using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;

namespace GEICS.Database
{
	class Plm
	{
		public int QcParamNO { get; set; }
		public int? RefQcParamNO { get; set; }
		public string MaterialCD { get; set; }
		public string EquipmentNO { get; set; }
		public string ManageNM { get; set; }
		public string ModelNM { get; set; }
		public string ParameterNM { get; set; }
		public string TotalKB { get; set; }
		public string DieKB { get; set; }

		public decimal ParameterMAX { get; set; }
		public decimal ParameterMIN { get; set; }
		public string ParameterVAL
		{
			get { return parameterVAL; }
			set { this.parameterVAL = value; }
		}
		private string parameterVAL = "";

		public decimal QcLineMAX { get; set; }
		public decimal QcLineMIN { get; set; }
		public decimal AimLineVAL { get; set; }
		public decimal AimRateVAL { get; set; }
		public bool DsFG { get; set; }

		public decimal? InnerUpperLimit { get; set; }
		public decimal? InnerLowerLimit { get; set; }

		public int EquipManageFG { get; set; }

        public string ClassNM { get; set; }

        public string ServerNM { get; set; }
        public string TypeGroup { get; set; }

        public static List<Plm> GetData(string conStr, string modelNm)
		{
			return GetData(conStr, string.Empty, modelNm, string.Empty);
		}

		public static List<Plm> GetData(string conStr, string typeCd, string modelNm, string chipNm)
		{
			List<Plm> retv = new List<Plm>();

			using (DBConnect con = SLCommonLib.DataBase.DBConnect.CreateInstance(conStr, "System.Data.SqlClient", false))
			{
				retv = GetDatas(con, typeCd, modelNm, null, true, chipNm, null);
			}

			return retv;

		}

		public static List<Plm> GetData(DBConnect conn, string typeCd, string modelNm, string chipNm)
		{
			if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm))
			{
				throw new ApplicationException(
					string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1}", typeCd, modelNm));
			}

			return GetDatas(conn, typeCd, modelNm, null, true, chipNm, null);
		}

		public static Plm GetData(DBConnect conn, int lineCd, string typeCd, string modelNm, int qcParamNo, bool isIncludeNascaParam)
		{
			if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
			{
				throw new ApplicationException(
					string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
			}

			List<Plm> plmList = GetDatas(conn, typeCd, modelNm, qcParamNo, isIncludeNascaParam, string.Empty, null);
			if (plmList.Count == 0)
			{
				return null;
			}
			else
			{
				return plmList.Single();
			}
		}

		public static Plm GetData(DBConnect conn, string typeCd, string modelNm, int qcParamNo, bool isIncludeNascaParam, string chipNM)
		{
			if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
			{
				throw new ApplicationException(
					string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
			}

			List<Plm> plmList = GetDatas(conn, typeCd, modelNm, qcParamNo, isIncludeNascaParam, chipNM, null);
			if (plmList.Count == 0)
			{
				return null;
			}
			else
			{
				return plmList.Single();
			}
		}

		public static Plm GetData(DBConnect conn, string typeCd, string modelNm, int qcParamNo, string chipNm)
		{
			if (string.IsNullOrEmpty(typeCd) || string.IsNullOrEmpty(modelNm) || qcParamNo == 0)
			{
				throw new ApplicationException(
					string.Format("Plm.GetData()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} QcParamNo:{2}", typeCd, modelNm, qcParamNo));
			}

			List<Plm> plmList = GetDatas(conn, typeCd, modelNm, qcParamNo, false, chipNm, null);
			if (plmList.Count == 0)
			{
				return null;
			}
			else
			{
				return plmList.Single();
			}
		}

		public static List<Plm> GetParamNoList(string conStr, string modelNm)
		{
			List<Plm> retv = new List<Plm>();

			using (DBConnect con = SLCommonLib.DataBase.DBConnect.CreateInstance(conStr, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TmPLM.QcParam_NO
                            FROM TmPLM WITH(nolock) WHERE 1=1 ";

				if (!string.IsNullOrEmpty(modelNm))
				{
					sql += " AND (TmPLM.Model_NM = @ModelNM) ";
					con.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
				}

				sql += " GROUP BY TmPLM.QcParam_NO ";

				using (DbDataReader rd = con.GetReader(sql))
				{
					int ordParamNo = rd.GetOrdinal("QcParam_NO");


					while (rd.Read())
					{
						Plm p = new Plm();
						p.QcParamNO = rd.GetInt32(ordParamNo);

						retv.Add(p);
					}
				}
			}

			return retv;
		}

		public static List<Plm> GetData(string typeCd, string equipNo, int qcParamNo)
		{
			List<Plm> paramList = new List<Plm>();

			using (DBConnect con = SLCommonLib.DataBase.DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TmPLM.QcParam_NO, Material_CD, Parameter_VAL, Parameter_MAX, Parameter_MIN
                            FROM TmPLM WITH(nolock) WHERE 1=1 ";

				if (!string.IsNullOrEmpty(equipNo))
				{
					sql += " AND (TmPLM.Equipment_NO = @EquipmentNO) ";
					con.SetParameter("@EquipmentNO", SqlDbType.VarChar, equipNo);
				}

				if (!string.IsNullOrEmpty(typeCd))
				{
					sql += " AND (TmPLM.Material_CD = @MaterialCD) ";
					con.SetParameter("@MaterialCD", SqlDbType.VarChar, typeCd);
				}

				sql += " AND (TmPLM.QcParam_NO = @QcParamNO) ";
				con.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo);

				using (DbDataReader rd = con.GetReader(sql))
				{
					int ordParamNo = rd.GetOrdinal("QcParam_NO");

					//int ordDieKB = rd.GetOrdinal("Die_KB");
					//int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
					int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
					//int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
					int ordMaterialCD = rd.GetOrdinal("Material_CD");

					while (rd.Read())
					{
						Plm p = new Plm();
						p.QcParamNO = rd.GetInt32(ordParamNo);

						p.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
						//p.ManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
						//p.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
						//p.TotalKB = Convert.ToString(rd["Total_KB"]).Trim();
						p.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();
						//p.EquipManageFG = rd.GetInt32(ordEquipManageFG);
						p.MaterialCD = rd.GetString(ordMaterialCD).Trim();

						//if (!rd.IsDBNull(ordRefQcParamNO))
						//{
						//    p.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
						//}
						//else
						//{
						//    p.RefQcParamNO = null;
						//}

						//if (!rd.IsDBNull(ordDieKB))
						//{
						//    p.DieKB = rd.GetString(ordDieKB).Trim();
						//}

						p.ParameterVAL = Convert.ToString(rd["Parameter_VAL"]).Trim();
						if (rd["Parameter_MAX"] != System.DBNull.Value)
						{
							p.ParameterMAX = Convert.ToDecimal(rd["Parameter_MAX"]);
						}
						if (rd["Parameter_MIN"] != System.DBNull.Value)
						{
							p.ParameterMIN = Convert.ToDecimal(rd["Parameter_MIN"]);
						}
						//p.DsFG = Convert.ToBoolean(rd["DS_FG"]);

						//if (rd["InnerUpperLimit"] != System.DBNull.Value)
						//{
						//    p.InnerUpperLimit = Convert.ToDecimal(rd["InnerUpperLimit"]);
						//}
						//if (rd["InnerLowerLimit"] != System.DBNull.Value)
						//{
						//    p.InnerLowerLimit = Convert.ToDecimal(rd["InnerLowerLimit"]);
						//}

						paramList.Add(p);
					}
				}
			}

			return paramList;
		}
		public static List<Plm> GetDatas(DBConnect conn, string typeCd, string modelNm, bool isIncludeNascaParam)
		{
			if (string.IsNullOrEmpty(typeCd))
			{
				throw new ApplicationException(
					string.Format("Plm.GetDatas()で必須条件が選択されずに関数呼び出しが発生 TypeCd:{0} ModelNm:{1} ", typeCd, modelNm));
			}

			return GetDatas(conn, typeCd, modelNm, null, isIncludeNascaParam, string.Empty, null);
		}

        public static List<Plm> GetDatas(string connStr, string modelNm, string serverNm)
        {
            return GetDatas(SLCommonLib.DataBase.DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false), null, modelNm, null, true, null, serverNm);
        }

		public static List<Plm> GetDatas(DBConnect conn, string typeCd, string modelNm, int? qcParamNo, bool isIncludeNascaParam, string chipNm, string serverNm)
		{
			List<Plm> retv = new List<Plm>();

			string sql = @" SELECT TmPRM.QcParam_NO, TmPRM.Manage_NM, TmPRM.Parameter_NM, TmPRM.Total_KB, TmPRM.Die_KB, TmPLM.Parameter_MAX, 
								TmPLM.Parameter_MIN, TmPLM.Parameter_VAL, TmPLM.DS_FG, TmPRM.RefQcParam_NO, TmPLM.Equipment_NO, TmPRM.EquipManage_FG, 
                                TmPLM.InnerUpperLimit, TmPLM.InnerLowerLimit, TmPLM.Material_CD, TmPRM.Class_NM, TmPLM.Model_NM
                            FROM TmPLM WITH(nolock) 
                            INNER JOIN TmPRM WITH(nolock) ON TmPLM.QcParam_NO = TmPRM.QcParam_NO 
                            WHERE (TmPLM.Del_FG = 0) AND (TmPRM.Del_FG = 0) ";

			if (qcParamNo.HasValue)
			{
				sql += " AND (TmPLM.QcParam_NO = @QcParamNO) ";
				conn.SetParameter("@QcParamNO", SqlDbType.Int, qcParamNo.Value);
			}

			if (isIncludeNascaParam == false)
			{
				sql += " AND (TmPRM.UnManageTrend_FG = 0) ";
			}

			if (!string.IsNullOrEmpty(chipNm))
			{
				sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
				conn.SetParameter("@ChipNM", SqlDbType.VarChar, chipNm);
			}

			if (!string.IsNullOrEmpty(modelNm))
			{
				sql += " AND (TmPLM.Model_NM = @ModelNM) ";
				conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
			}

			if (!string.IsNullOrEmpty(typeCd))
			{
				sql += " AND (TmPLM.Material_CD = @MaterialCD) ";
				conn.SetParameter("@MaterialCD", SqlDbType.Char, typeCd);
			}

			using (DbDataReader rd = conn.GetReader(sql))
			{
				string ordDieKB = "Die_KB";
				int ordRefQcParamNO = rd.GetOrdinal("RefQcParam_NO");
				int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
				int ordEquipManageFG = rd.GetOrdinal("EquipManage_FG");
				int ordMaterialCD = rd.GetOrdinal("Material_CD");

				while (rd.Read())
				{
					Plm p = new Plm();
					p.QcParamNO = Convert.ToInt32(rd["QcParam_NO"]);
					p.ManageNM = Convert.ToString(rd["Manage_NM"]).Trim();
					p.ParameterNM = Convert.ToString(rd["Parameter_NM"]).Trim();
					p.TotalKB = Convert.ToString(rd["Total_KB"]).Trim();
					p.EquipmentNO = rd.GetString(ordEquipmentNO).Trim();
					p.EquipManageFG = rd.GetInt32(ordEquipManageFG);
					p.MaterialCD = rd.GetString(ordMaterialCD).Trim();

					if (!rd.IsDBNull(ordRefQcParamNO))
					{
						p.RefQcParamNO = rd.GetInt32(ordRefQcParamNO);
					}
					else
					{
						p.RefQcParamNO = null;
					}

					if (!rd.IsDBNull(rd.GetOrdinal(ordDieKB)))
					{
						p.DieKB = rd.GetString(rd.GetOrdinal(ordDieKB)).Trim();
					}

					p.ParameterVAL = Convert.ToString(rd["Parameter_VAL"]).Trim();
					if (rd["Parameter_MAX"] != System.DBNull.Value)
					{
						p.ParameterMAX = Convert.ToDecimal(rd["Parameter_MAX"]);
					}
					if (rd["Parameter_MIN"] != System.DBNull.Value)
					{
						p.ParameterMIN = Convert.ToDecimal(rd["Parameter_MIN"]);
					}
					p.DsFG = Convert.ToBoolean(rd["DS_FG"]);

					if (rd["InnerUpperLimit"] != System.DBNull.Value)
					{
						p.InnerUpperLimit = Convert.ToDecimal(rd["InnerUpperLimit"]);
					}
					if (rd["InnerLowerLimit"] != System.DBNull.Value)
					{
						p.InnerLowerLimit = Convert.ToDecimal(rd["InnerLowerLimit"]);
					}
                    p.ClassNM = Convert.ToString(rd["Class_NM"]);

                    if (string.IsNullOrEmpty(serverNm) == false)
                    {
                        p.ServerNM = serverNm;
                        p.TypeGroup = ConnectQCIL.GetTypeGroup(serverNm);
                    }
                    p.ModelNM = rd["Model_NM"].ToString().Trim();
                    retv.Add(p);
				}
			}

			return retv;
		}
	}
}
