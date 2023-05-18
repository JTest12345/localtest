using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;

namespace GEICS.Report
{
	class ParameterPerLotReport
	{
		private const int PARAM_START_COL = 5;
		private static Dictionary<int , KeyValuePair<string, bool>> paramHeader;

		public string MaterialCD { get; set; }
		public string LotNO { get; set; }
		public string MachineSeqNO { get; set; }
		public int LineCD { get; set; }
		public DateTime MeasureDT { get; set; }

		public static ParameterReport GetReportDataFromServer(string connStr, List<string> equipmentList, List<int> paramNoList, DateTime startDT, DateTime endDT, bool isSingleMagazineData)
		{
			try
			{
				List<int> lineCDList = ConnectQCIL.GetLineCDList(connStr);
				List<ParameterPerLotReport> lotList = new List<ParameterPerLotReport>();
				List<Parameter> parameterList = new List<Parameter>();

				string sql = @" SELECT dbo.TnLOG.Inline_CD, dbo.TnLOG.Measure_DT, dbo.TnLOG.Material_CD, dbo.TnLOG.NascaLot_NO, dbo.TnLOG.DParameter_VAL, dbo.TnLOG.SParameter_VAL,
								   dbo.TmPRM.QcParam_NO, dbo.TmPRM.Parameter_NM, dbo.TmEQUI.MachinSeq_NO, TnLOG.Magazine_NO
							FROM dbo.TnLOG WITH(nolock) 
                                Inner Join dbo.TmPRM WITH(nolock) ON dbo.TnLOG.QcParam_NO = dbo.TmPRM.QcParam_NO
								INNER JOIN dbo.TmEQUI WITH(nolock) ON TnLOG.Equipment_NO = TmEQUI.Equipment_NO
							WHERE Measure_DT >= @StartDT and Measure_DT < @EndDT 
							and TnLog.Del_FG = 0 ";

                if (paramNoList != null && paramNoList.Any())
                {
                    sql += $" AND TnLog.QcParam_NO IN ('{ string.Join("','", paramNoList.Select(p => p.ToString()).ToArray())}') ";
                }

                if (equipmentList != null && equipmentList.Any())
                {
                    sql += $" AND TnLog.Equipment_NO IN ('{ string.Join("','", equipmentList.ToArray())}') ";
                }

                if (lineCDList != null && lineCDList.Any())
                {
                    sql += $" AND TnLog.Inline_CD IN ('{ string.Join("','", lineCDList.Select(p => p.ToString()).ToArray())}') ";
                }

                sql += " ORDER BY QcParam_NO ";

                int paramIndex = 0; int qcParamNo = 0;
                using (DBConnect conn = DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
				{
					conn.SetParameter("@StartDT", SqlDbType.DateTime, startDT);
					conn.SetParameter("@EndDT", SqlDbType.DateTime, endDT);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        int ordMachineSeqNO = rd.GetOrdinal("MachinSeq_NO");
                        int ordMeasureDT = rd.GetOrdinal("Measure_DT");
                        int ordSParameterVAL = rd.GetOrdinal("SParameter_VAL");
                        int ordDparameterVAL = rd.GetOrdinal("DParameter_VAL");
                        int ordInlineCD = rd.GetOrdinal("Inline_CD");
                        int ordMaterialCD = rd.GetOrdinal("Material_CD");
                        int ordNascaLotNO = rd.GetOrdinal("NascaLot_NO");
                        int ordParameterNM = rd.GetOrdinal("Parameter_NM");
                        int ordMagazineNO = rd.GetOrdinal("Magazine_NO");
                        int ordQcParamNO = rd.GetOrdinal("QcParam_NO");
                        while (rd.Read())
                        {
                            ParameterPerLotReport lotRecord = new ParameterPerLotReport();
                            Parameter paramRecord = new Parameter();
                            
                            lotRecord.MachineSeqNO = rd.GetString(ordMachineSeqNO).Trim();
                            lotRecord.MeasureDT = rd.GetDateTime(ordMeasureDT);
                            lotRecord.LineCD = rd.GetInt32(ordInlineCD);
                            lotRecord.MaterialCD = rd.GetString(ordMaterialCD).Trim();
                            lotRecord.LotNO = rd.GetString(ordNascaLotNO);

                            paramRecord.LotNO = lotRecord.LotNO;
                            paramRecord.MagazineNO = rd.GetString(ordMagazineNO).Trim();
                            paramRecord.MeasureDT = rd.GetDateTime(ordMeasureDT);
                            paramRecord.ParameterNM = rd.GetString(ordParameterNM).Trim();
                            paramRecord.QcParamNO = rd.GetInt32(ordQcParamNO);

                            if (rd.IsDBNull(ordDparameterVAL) == false)
                            {
                                paramRecord.ParameterVAL = rd.GetDecimal(ordDparameterVAL);
                                paramRecord.IsDecimalValue = true;
                            }
                            else if (rd.IsDBNull(ordSParameterVAL) == false)
                            {
                                paramRecord.ParameterVAL = rd.GetString(ordSParameterVAL).Trim();
                                paramRecord.IsDecimalValue = false;
                            }
                            else
                            {
                                paramRecord.ParameterVAL = null;
                                paramRecord.IsDecimalValue = false;
                            }

                            if (qcParamNo != 0 && qcParamNo != paramRecord.QcParamNO)
                            {
                                paramIndex = paramIndex + 1;
                            }
                            paramRecord.ParameterIndex = paramIndex;
                            
                            if (lotList.Exists(l => l.LineCD == lotRecord.LineCD && l.LotNO == lotRecord.LotNO && l.MachineSeqNO == lotRecord.MachineSeqNO
                                && l.MaterialCD == lotRecord.MaterialCD && l.MeasureDT == lotRecord.MeasureDT) == false)
                                //if (lotList.Exists(l => l.LineCD == lotRecord.LineCD && l.LotNO == lotRecord.LotNO && l.MachineSeqNO == lotRecord.MachineSeqNO
                                //&& l.MaterialCD == lotRecord.MaterialCD) == false)
                            {
                                //if (lotRecord.LotNO == "H189KF32100")
                                //{
                                //    continue;
                                //}

                                lotList.Add(lotRecord);
                            }

                            qcParamNo = paramRecord.QcParamNO;

                            parameterList.Add(paramRecord);
                        }
                    }


                    //foreach (int paramNO in paramNoList)
                    //{
                    //	bool isThereParamData = false;

                    //	conn.SetParameter("@QcParamNO", SqlDbType.Int, paramNO);

                    //	foreach (int lineCD in lineCDList)
                    //	{
                    //		conn.SetParameter("@InlineCD", SqlDbType.Int, lineCD);

                    //		foreach (string equipmentNO in equipmentList)
                    //		{
                    //			conn.SetParameter("@EquipmentNO", SqlDbType.Char, equipmentNO);

                    //			using (DbDataReader rd = conn.GetReader(sql))
                    //			{
                    //				int ordMachineSeqNO = rd.GetOrdinal("MachinSeq_NO");
                    //				int ordMeasureDT = rd.GetOrdinal("Measure_DT");
                    //				int ordSParameterVAL = rd.GetOrdinal("SParameter_VAL");
                    //				int ordDparameterVAL = rd.GetOrdinal("DParameter_VAL");
                    //				int ordInlineCD = rd.GetOrdinal("Inline_CD");
                    //				int ordMaterialCD = rd.GetOrdinal("Material_CD");
                    //				int ordNascaLotNO = rd.GetOrdinal("NascaLot_NO");
                    //				int ordParameterNM = rd.GetOrdinal("Parameter_NM");
                    //				int ordMagazineNO = rd.GetOrdinal("Magazine_NO");
                    //				while (rd.Read())
                    //				{
                    //					ParameterPerLotReport lotRecord = new ParameterPerLotReport();
                    //					Parameter paramRecord = new Parameter();

                    //					lotRecord.MachineSeqNO = rd.GetString(ordMachineSeqNO).Trim();
                    //					lotRecord.MeasureDT = rd.GetDateTime(ordMeasureDT);
                    //					lotRecord.LineCD = rd.GetInt32(ordInlineCD);
                    //					lotRecord.MaterialCD = rd.GetString(ordMaterialCD);
                    //					lotRecord.LotNO = rd.GetString(ordNascaLotNO);

                    //					paramRecord.LotNO = lotRecord.LotNO;
                    //					paramRecord.MagazineNO = rd.GetString(ordMagazineNO).Trim();
                    //					paramRecord.MeasureDT = rd.GetDateTime(ordMeasureDT);
                    //					paramRecord.ParameterIndex = paramIndex;
                    //					paramRecord.ParameterNM = rd.GetString(ordParameterNM).Trim();

                    //					if (rd.IsDBNull(ordDparameterVAL) == false)
                    //					{
                    //						paramRecord.ParameterVAL = rd.GetDecimal(ordDparameterVAL);
                    //						paramRecord.IsDecimalValue = true;
                    //					}
                    //					else if (rd.IsDBNull(ordSParameterVAL) == false)
                    //					{
                    //						paramRecord.ParameterVAL = rd.GetString(ordSParameterVAL).Trim();
                    //						paramRecord.IsDecimalValue = false;
                    //					}
                    //					else
                    //					{
                    //						paramRecord.ParameterVAL = null;
                    //						paramRecord.IsDecimalValue = false;
                    //					}

                    //					if (lotList.Exists(l => l.LineCD == lotRecord.LineCD && l.LotNO == lotRecord.LotNO && l.MachineSeqNO == lotRecord.MachineSeqNO
                    //						&& l.MaterialCD == lotRecord.MaterialCD && l.MeasureDT == lotRecord.MeasureDT) == false)
                    //					{
                    //						lotList.Add(lotRecord);
                    //					}

                    //					parameterList.Add(paramRecord);
                    //					isThereParamData = true;
                    //				}
                    //			}
                    //		}
                    //	}
                    //	if (isThereParamData)
                    //	{
                    //		paramIndex++;
                    //	}
                    //}
                }

                // 分割マガジンは先頭マガジンデータのみに絞る
                if (isSingleMagazineData)
				{
					parameterList = parameterList.OrderBy(p => p.MeasureDT).ToList();
					foreach (IGrouping<string, string> lot in lotList.Select(l => l.LotNO).GroupBy(l => l))
					{
						string singleMagazineNo = string.Empty;
						IEnumerable<IGrouping<string, string>> maglist = parameterList.Where(p => p.LotNO == lot.Key).Select(p => p.MagazineNO).GroupBy(p => p);
						if (maglist.Count() >= 2)
						{
							singleMagazineNo = maglist.FirstOrDefault().Key;
						}

						if (string.IsNullOrEmpty(singleMagazineNo) == false)
						{
							parameterList.RemoveAll(p => p.LotNO == lot.Key && singleMagazineNo != p.MagazineNO);
						}
					}
				}

				ParameterReport paramReport = new ParameterReport();
				paramReport.LotInfoList = lotList;
				paramReport.ParameterList = parameterList;

				return paramReport;
			}
			catch (Exception err)
			{
				throw new Exception(err.Message, err);
			}
		}

		public static object[,] ConvertToExcelData(ParameterReport reportData)
		{
			string nowMate = string.Empty, nowLot = string.Empty, nowMac = string.Empty, nowLine = string.Empty, nowMeasure = string.Empty;

			try
			{
				InitParamHeaderData(reportData.ParameterList);

				int paramNum = reportData.ParameterList.Select(p => p.ParameterIndex).Distinct().ToList().Count;

				//エクセルに出力するデータを格納する為の配列宣言(ヘッダは含まない)
				object[,] reportArray = new object[reportData.LotInfoList.Count(), PARAM_START_COL + paramHeader.Count()];

				//object[,] reportArray = new object[reportData.LotInfoList.Count(), PARAM_START_COL + paramNum];

				//reportData.LotInfoList = reportData.LotInfoList.OrderBy(r => r.MaterialCD).ThenBy(r => r.LotNO) //並べ替え不要と言う事で、とりあえずコメントアウト

				int row = 0;

				//ロット毎のレコードをforeach内で作っていく
				foreach (ParameterPerLotReport lotInfo in reportData.LotInfoList)
				{
					nowMate = lotInfo.MaterialCD;
					nowLot = lotInfo.LotNO;
					nowMac = lotInfo.MachineSeqNO;
					nowLine = lotInfo.LineCD.ToString();
					nowMeasure = lotInfo.MeasureDT.ToString();

					reportArray[row, 0] = nowMate;
					reportArray[row, 1] = nowLot;
					reportArray[row, 2] = nowMac;
					reportArray[row, 3] = nowLine;
					reportArray[row, 4] = nowMeasure;

					List<Parameter> paramInfoList = reportData.ParameterList.Where(r => r.LotNO == lotInfo.LotNO && r.MeasureDT == lotInfo.MeasureDT).ToList();

					//ロット毎に全パラメタの値を取得
					foreach (Parameter paramInfo in paramInfoList)
					{
						int col = PARAM_START_COL + paramInfo.ParameterIndex;//パラメタ取得時にパラメタ名と紐付ける形で0始まりのインデックスを振っているので、それを列指定に使用

						//if (paramInfo.IsDecimalValue)
						//{
						//    decimal value = Convert.ToDecimal(paramInfo.ParameterVAL);
						//    reportArray[row, col] = value;
						//}
						//else
						//{
						//    reportArray[row, col] = paramInfo.ParameterVAL.ToString();
						//}
						reportArray[row, col] = paramInfo.ParameterVAL;
					}

					row++;
				}

				return reportArray;
			}
			catch (Exception err)
			{
				throw new Exception(
					string.Format("ｴﾗｰ:{0} ﾀｲﾌﾟ:{1}, ﾛｯﾄ:{2}, 号機:{3}, ﾗｲﾝ:{4}, 日時:{5}", err.Message, nowMate, nowLot, nowMac, nowLine, nowMeasure), err);
			}
		}

		/// <summary>
		/// パラメタの名前をヘッダ出力用の配列に格納する
		/// </summary>
		/// <param name="paramInfoList"></param>
		private static void InitParamHeaderData(List<Parameter> paramInfoList)
		{
			try
			{
				string debug = string.Empty;
				foreach (Parameter para in paramInfoList)
				{
					debug += string.Format("{0} | {1} | {2} | {3}\r\n", para.LotNO, para.MagazineNO, para.ParameterIndex, para.ParameterNM);
				}

				List<int> paramIndexList = paramInfoList.Select(p => p.ParameterIndex).Distinct().ToList();

				paramHeader = new Dictionary<int, KeyValuePair<string, bool>>();

				for (int index = 0; index < paramIndexList.Count; index++)
				{
					string headerNM = paramInfoList.Where(p => p.ParameterIndex == index).Select(p => p.ParameterNM).First();
					bool isDecimalValue = paramInfoList.Where(p => p.ParameterIndex == index).Select(p => p.IsDecimalValue).First();

					if (paramHeader.ContainsKey(index) == false)
					{
						KeyValuePair<string, bool> kvp = new KeyValuePair<string, bool>(headerNM, isDecimalValue);
						paramHeader.Add(index, kvp);
					}
				}
			}
			catch (Exception err)
			{
				throw new Exception(err.Message, err);
			}

			//foreach (int paramIndex in paramIndexList)
			//{
			//    //パラメタのインデックスが一致するパラメタ名を取得
			//    paramHeader[paramIndex] = paramInfoList.Where(p => p.ParameterIndex == paramIndex).Select(p => p.ParameterNM).First();
			//}
		}

		public static Dictionary<int, KeyValuePair<string, bool>> GetParamHeaderData()
		{
			return paramHeader;
		}
	}
}
