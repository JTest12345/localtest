using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
{
	public class PlcFileConv
	{
		public const string GENERAL_GRP_CD = "PLCFILE";

		public const string IDENTCD_LOTNO = "IDENTLOT";
		public const string IDENTCD_MEASUREDT = "IDENTMDT";
		public const string IDENTCD_MAGNO = "IDENTMAG";
		public const string IDENTCD_TYPE = "IDENTTYP";

		public string ModelNM { get; set; }
        public int? QcParamNO { get; set; }
		public string PrefixNM { get; set; }
		public string HeaderNM { get; set; }
		public int OrderNO { get; set; }
		public string PlcADDR { get; set; }
		public int DataLen { get; set; }
		public string DataTypeCD { get; set; }
		public string IdentifyCD { get; set; }
		public bool DelFG { get; set; }
		public DateTime LastUpdDT { get; set; }

        public Plm Plm { get; set; }

		public static string GetGeneralNm(int lineCd, string genCd)
		{
			List<General> genList = General.GetGeneralData(GENERAL_GRP_CD, lineCd);

			List<string> genNmList = genList.Where(g => g.GeneralCD == genCd).Select(g => g.GeneralNM).ToList();

			string genNm = string.Empty;
			if (genNmList.Count == 1)
			{
				genNm = genNmList.Single();
			}				

			return genNm;
		}

		public static List<PlcFileConv> GetDataList(int lineCd, string modelNm, string prefixNm)
		{
			return GetDataList(lineCd, modelNm, prefixNm, null, null);
		}

		public static List<PlcFileConv> GetDataList(int lineCd, string modelNm, string prefixNm, string headerNm, int? orderNo)
		{
			try
			{
				List<PlcFileConv> retv = new List<PlcFileConv>();

				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), "System.Data.SqlClient", false))
				{
                    string sql = @" SELECT TmPlcFileConv.Model_NM, TmPlcFileConv.QcParam_NO, TmPlcFileConv.Prefix_NM, TmPlcFileConv.Header_NM, TmPlcFileConv.Order_NO, TmPlcFileConv.Plc_ADDR,
										TmPlcFileConv.Data_LEN, TmPlcFileConv.DataType_CD, TmPlcFileConv.Identify_CD, TmPlcFileConv.Del_FG, TmPlcFileConv.LastUpd_DT
									FROM TmPlcFileConv WITH(nolock) 
									WHERE (TmPlcFileConv.Model_NM = @ModelNM) 
									AND (TmPlcFileConv.Del_FG = 0) ";

					if (string.IsNullOrEmpty(prefixNm) == false)
					{
						sql += " AND (TmPlcFileConv.Prefix_NM = @PrefixNM) ";
						conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNm);
					}

					if (string.IsNullOrEmpty(headerNm) == false)
					{
						sql += " AND (TmPlcFileConv.Header_NM = @HeaderNM) ";
						conn.SetParameter("@HeaderNM", SqlDbType.VarChar, headerNm);
					}

					if (orderNo.HasValue)
					{
						sql += " AND (TmPlcFileConv.Order_NM = @OrderNM) ";
						conn.SetParameter("@OrderNM", SqlDbType.VarChar, orderNo.Value);
					}

					conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordModelNM = rd.GetOrdinal("Model_NM");
                        int ordQcParamNO = rd.GetOrdinal("QcParam_NO");
                        int ordPrefixNM = rd.GetOrdinal("Prefix_NM");
						int ordHeaderNM = rd.GetOrdinal("Header_NM");
						int ordOrderNO = rd.GetOrdinal("Order_NO");
						int ordPlcADDR = rd.GetOrdinal("Plc_ADDR");
						int ordDataLEN = rd.GetOrdinal("Data_LEN");
						int ordDataTypeCD = rd.GetOrdinal("DataType_CD");
						int ordIdentifyCD = rd.GetOrdinal("Identify_CD");
						int ordDelFG = rd.GetOrdinal("Del_FG");
						int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");

						while (rd.Read())
						{
							PlcFileConv pfc = new PlcFileConv();
							pfc.ModelNM = rd.GetString(ordModelNM).Trim();

							if (rd.IsDBNull(ordQcParamNO))
							{
								pfc.QcParamNO = null;
							}
							else
							{
								pfc.QcParamNO = rd.GetInt32(ordQcParamNO);
							}
                            pfc.PrefixNM = rd.GetString(ordPrefixNM).Trim();
							pfc.HeaderNM = rd.GetString(ordHeaderNM).Trim();
							pfc.OrderNO = rd.GetInt32(ordOrderNO);
							pfc.PlcADDR = rd.GetString(ordPlcADDR).Trim();
							pfc.DataLen = rd.GetInt32(ordDataLEN);
							pfc.DataTypeCD = rd.GetString(ordDataTypeCD).Trim();

							if (rd.IsDBNull(ordIdentifyCD))
							{
								pfc.IdentifyCD = string.Empty;
							}
							else
							{
								pfc.IdentifyCD = rd.GetString(ordIdentifyCD).Trim();
							}
							pfc.DelFG = rd.GetBoolean(ordDelFG);
							pfc.LastUpdDT = rd.GetDateTime(ordLastUpdDT);

							retv.Add(pfc);
						}
					}
				}

				return retv;
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}
}
