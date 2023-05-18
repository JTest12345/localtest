using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.Commons;
using SLCommonLib.DataBase;
using System.Data;


namespace EICS.Database
{
	class PLAJEAddr
	{
		public string LineCD { get; set; }
		public string FrameNO { get; set; }
		public string Year { get; set; }
		public string Month { get; set; }
		public string Day { get; set; }
		public string Hour { get; set; }
		public string Minute { get; set; }
		public string Second { get; set; }
		public string Ar { get; set; }
		public string CF4 { get; set; }
		public string Exhaust { get; set; }
		public string Pf { get; set; }
		public string Pr { get; set; }
		public string Vdc { get; set; }
		public string Vpp { get; set; }
		public string Load { get; set; }
		public string Phase { get; set; }
		public string TrendSummary { get; set; }



		/// <summary>
		/// ログファイル紐付けマスタ[TmPLCFMT]取得
		/// </summary>
		/// <param name="prefixNM">ファイル種類</param>
		/// <param name="modelNM">装置型式</param>
		/// <returns>紐付けマスタ</returns>
		public static List<PLAJEAddr> GetPLAJEAddr(int lineCD)
		{
			System.Data.Common.DbDataReader rd = null;
			List<PLAJEAddr> plaJEAddrList = new List<PLAJEAddr>();

			try
			{
				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
				{
					string sql = @" SELECT TmPLAJEADDR.Inline_CD, TmPLAJEADDR.FrameNoAddr_NM, TmPLAJEADDR.YearAddr_NM, TmPLAJEADDR.MonthAddr_NM, TmPLAJEADDR.DayAddr_NM, TmPLAJEADDR.HourAddr_NM, TmPLAJEADDR.MinuteAddr_NM, TmPLAJEADDR.SecondAddr_NM, 
									TmPLAJEADDR.ArAddr_NM, TmPLAJEADDR.CFAddr_NM, TmPLAJEADDR.ExhaustAddr_NM, TmPLAJEADDR.PfAddr_NM, TmPLAJEADDR.PrAddr_NM, TmPLAJEADDR.VdcAddr_NM, TmPLAJEADDR.VppAddr_NM, TmPLAJEADDR.LoadAddr_NM, TmPLAJEADDR.PhaseAddr_NM, TmPLAJEADDR.TrendSummaryAddr_NM 
	                                FROM TmPLAJEADDR WITH(nolock)
									WHERE (TmPLAJEADDR.Inline_CD = @InlineCD) AND (TmPLAJEADDR.Del_FG = 0) ";

					conn.SetParameter("@InlineCD", SqlDbType.VarChar, lineCD);

					using (rd = conn.GetReader(sql))
					{
						while (rd.Read())
						{
							PLAJEAddr plaJEAddr = new PLAJEAddr();

							plaJEAddr.LineCD = Convert.ToString(rd["Inline_CD"]);
							plaJEAddr.FrameNO = Convert.ToString(rd["FrameNoAddr_NM"]);
							plaJEAddr.Year = Convert.ToString(rd["YearAddr_NM"]);
							plaJEAddr.Month = Convert.ToString(rd["MonthAddr_NM"]);
							plaJEAddr.Day = Convert.ToString(rd["DayAddr_NM"]);
							plaJEAddr.Hour = Convert.ToString(rd["HourAddr_NM"]);
							plaJEAddr.Minute = Convert.ToString(rd["MinuteAddr_NM"]);
							plaJEAddr.Second = Convert.ToString(rd["SecondAddr_NM"]);
							plaJEAddr.Ar = Convert.ToString(rd["ArAddr_NM"]);
							plaJEAddr.CF4 = Convert.ToString(rd["CFAddr_NM"]);
							plaJEAddr.Exhaust = Convert.ToString(rd["ExhaustAddr_NM"]);
							plaJEAddr.Pf = Convert.ToString(rd["PfAddr_NM"]);
							plaJEAddr.Pr = Convert.ToString(rd["PrAddr_NM"]);
							plaJEAddr.Vdc = Convert.ToString(rd["VdcAddr_NM"]);
							plaJEAddr.Vpp = Convert.ToString(rd["VppAddr_NM"]);
							plaJEAddr.Load = Convert.ToString(rd["LoadAddr_NM"]);
							plaJEAddr.Phase = Convert.ToString(rd["PhaseAddr_NM"]);
							plaJEAddr.TrendSummary = Convert.ToString(rd["TrendSummaryAddr_NM"]);


							plaJEAddrList.Add(plaJEAddr);
						}
					}
				}

				return plaJEAddrList;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static List<string> GetPrefixList(LSETInfo lsetInfo)
		{
			System.Data.Common.DbDataReader rd = null;

			List<string> prefixList = new List<string>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
			{
				string sql = @" SELECT TmPLCFMT.Prefix_NM
                                FROM TmPLCFMT WITH(nolock)
                                INNER JOIN TmPRM WITH(nolock) ON TmPRM.QcParam_NO = TmPLCFMT.QcParam_NO
                                WHERE (TmPLCFMT.Model_NM = @ModelNM) AND (TmPLCFMT.Del_FG = 0) 
								Group by TmPLCFMT.Prefix_NM ";

				if (lsetInfo.ChipNM != "" && lsetInfo.ChipNM != null)
				{
					sql += " AND (TmPRM.Chip_NM = @ChipNM) ";
					conn.SetParameter("@ChipNM", SqlDbType.VarChar, lsetInfo.ChipNM);
				}

				conn.SetParameter("@ModelNM", SqlDbType.VarChar, lsetInfo.ModelNM);

				using (rd = conn.GetReader(sql))
				{
					while (rd.Read())
					{
						string prefixNM;
						prefixNM = Convert.ToString(rd["Prefix_NM"]);

						prefixList.Add(prefixNM);
					}
				}
			}

			return prefixList;

		}

	}
}
