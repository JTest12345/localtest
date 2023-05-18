using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;

namespace GEICS.Report
{
	class WBEquipVerReport
	{
		public DateTime RecordDT { get; set; }
		public string EquipmentNO { get; set; }
		public string Version { get; set; }

		private static string[] paramHeader;
		private const int COL_COUNT = 3;

		public static List<WBEquipVerReport> GetReportDataFromServer(string connStr, List<string> equipmentList, DateTime startDT, DateTime endDT)
		{
			List<int> lineCDList = ConnectQCIL.GetLineCDList(connStr);
			List<WBEquipVerReport> reportData = new List<WBEquipVerReport>();
			List<WBEquipVerReport> dbData = new List<WBEquipVerReport>();

//            string sql = @" Select Equipment_NO, CONVERT(varchar, Measure_DT, 111) as MeasureDate, SParameter_VAL FROM TnLOG with(nolock)
//							Where Inline_CD = @InlineCD and Measure_DT >= @StartDT and Measure_DT < @EndDT and Equipment_NO = @EquipmentNO and QcParam_NO = @QcParamNO 
//							Group by Convert(varchar, Measure_DT, 111), Equipment_NO, SParameter_VAL ";

						string sql = @" Select Equipment_NO, Measure_DT, SParameter_VAL FROM TnLOG with(nolock)
							Where Inline_CD = @InlineCD and Measure_DT >= @StartDT and Measure_DT < @EndDT and Equipment_NO = @EquipmentNO and QcParam_NO = @QcParamNO ";

			using(DBConnect conn = DBConnect.CreateInstance(connStr, "System.Data.SqlClient", false))
			{
				conn.SetParameter("@StartDT", SqlDbType.DateTime, startDT);
				conn.SetParameter("@EndDT", SqlDbType.DateTime, endDT);
				conn.SetParameter("@QcParamNO", SqlDbType.Int, Constant.WBEquipVerParamNO);

				foreach(int lineCD in lineCDList)
				{
					conn.SetParameter("@InlineCD", SqlDbType.Int, lineCD);

					foreach (string equipmentNO in equipmentList)
					{
						conn.SetParameter("@EquipmentNO", SqlDbType.Char, equipmentNO);	
						
						using(DbDataReader rd = conn.GetReader(sql))
						{
							int ordEquipNO = rd.GetOrdinal("Equipment_NO");
							int ordMeasureDT = rd.GetOrdinal("Measure_DT");
							int ordSParameterVAL = rd.GetOrdinal("SParameter_VAL");

							while (rd.Read())
							{
								WBEquipVerReport record = new WBEquipVerReport();

								record.EquipmentNO = rd.GetString(ordEquipNO).Trim();
								//record.RecordDT = rd.GetString(ordMeasureDT).Trim();
								record.RecordDT = rd.GetDateTime(ordMeasureDT);
								record.Version = rd.GetString(ordSParameterVAL).Trim();

								dbData.Add(record);
							}
						}
					}
				}
			}

			List<WBEquipVerReport> groupingList = new List<WBEquipVerReport>();

			foreach (WBEquipVerReport recordFromDB in dbData)
			{
				int index = reportData.FindIndex(r => r.RecordDT.Date == recordFromDB.RecordDT.Date && r.EquipmentNO == recordFromDB.EquipmentNO && r.Version == recordFromDB.Version);
				if (index >= 0)
				{
					if (reportData[index].RecordDT < recordFromDB.RecordDT)
					{
						reportData[index].RecordDT = recordFromDB.RecordDT;
					}
				}
				else
				{
					reportData.Add(recordFromDB);
				}
			}
			

			return reportData;
		}

		public static object[,] ConvertToExcelData(List<WBEquipVerReport> reportData)
		{
			//エクセルに出力するデータを格納する為の配列宣言(ヘッダは含まない)
			string[,] reportArray = new string[reportData.Count(), COL_COUNT];

			//ロット毎のレコードをforeach内で作っていく
			for(int row = 0; row < reportData.Count; row++)
			{
				reportArray[row, 0] = reportData[row].RecordDT.ToString();
				reportArray[row, 1] = reportData[row].EquipmentNO;
				reportArray[row, 2] = reportData[row].Version;
			}

			return reportArray;
		}

	}
}
