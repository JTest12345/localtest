using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;

namespace EICS.Database
{
	class Lott
	{
		public int PassedLineCD { get; set; }
		public string PassedAssetsNM { get; set; }
		public string PassedEquipmentNO { get; set; }
		public string ChipNM { get; set; }
		public string LotNO { get; set; }

		//TnLOTT(Lotがどの装置を通ったかを記憶するテーブル)に情報を記憶する
		public static void SetTnLOTT(LSETInfo lsetInfo, string sLot)
		{
			string BaseSql = " INSERT INTO TnLOTT(Inline_CD, NascaLot_NO, Equipment_NO, Assets_NM)"
						+ " VALUES(@InlineCD, @NascaLotNO, @EquipmentNO, @AssetsNM) ";//※Measure_DTは初期値でGETDATE()「ｻｰﾊﾞｰが持っている現在時間」を指定済み

			
			string sql = string.Format(BaseSql, lsetInfo.InlineCD, sLot.Trim(), lsetInfo.EquipmentNO.Trim(), lsetInfo.AssetsNM.Trim());
#if Debug
            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnStringForDebug(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
#else
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lsetInfo.InlineCD), "System.Data.SqlClient", false))
#endif
			{
				conn.SetParameter("@InlineCD", SqlDbType.Int, lsetInfo.InlineCD);
				conn.SetParameter("@NascaLotNO", SqlDbType.VarChar, sLot.Trim());
				conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, lsetInfo.EquipmentNO.Trim());
				conn.SetParameter("@AssetsNM", SqlDbType.NVarChar, lsetInfo.AssetsNM.Trim());

				try
				{
					conn.ExecuteScalar(sql);
				}
				catch (Exception ex)
				{
					string sMsg = lsetInfo.InlineCD + "/" + DateTime.Now + "/[原因]" + ex.Message + "/ネットワーク停止<--Start";
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMsg);
				}
			}
		}

		public static List<Lott> GetData(int dbServLineCD, string assetsNM, string lotNO)
		{
			List<Lott> lottList = new List<Lott>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, dbServLineCD), "System.Data.SqlClient", false))
			{
				string sql = @" SELECT Inline_CD, NascaLot_NO, Equipment_NO, Measure_DT, Assets_NM
								FROM TnLOTT WITH(nolock)
								WHERE NascaLot_NO = @LotNO and Assets_NM = @AssetsNM OPTION(MAXDOP 1) ";

				conn.SetParameter("@LotNO", SqlDbType.VarChar, lotNO);
				conn.SetParameter("@AssetsNM", SqlDbType.NVarChar, assetsNM);

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int ordInLineCD = rd.GetOrdinal("InLine_CD");
					int ordNascaLotNO = rd.GetOrdinal("NascaLot_NO");
					int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
					int ordAssetsNM = rd.GetOrdinal("Assets_NM");

					while (rd.Read())
					{
						Lott lot = new Lott();

						lot.PassedLineCD = rd.GetInt32(ordInLineCD);
						lot.LotNO = rd.GetString(ordNascaLotNO);
						lot.PassedEquipmentNO = rd.GetString(ordEquipmentNO);
						lot.PassedAssetsNM = rd.GetString(ordAssetsNM);

						lottList.Add(lot);
					}
				}

			}

			return lottList;
		}

		public static List<int> GetPassedLine(int dbServLineCD, string assetsNM, string lotNO)
		{
			List<int> retv = new List<int>();

			List<Lott> LottList = GetData(dbServLineCD, assetsNM, lotNO);
			if (LottList.Count == 0) { return retv; }

			retv = LottList.Select(l => l.PassedLineCD).Distinct().ToList();
			return retv;
		}
	}
}
