using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;

namespace EICS.Database
{
	class Tran
	{
		public string PlantCD { get; set; }
		public DateTime StartDT { get; set; }
		public DateTime? EndDT { get; set; }

		public static List<Tran> GetData(int lineCD, string modelNM, string lotNO)
        {
            return GetData(lineCD, modelNM, lotNO, false);
        }

        public static List<Tran> GetData(int lineCD, string modelNM, string lotNO, bool useLikeClassNM)
        {
            List<Tran> tranList = new List<Tran>();
			System.Data.Common.DbDataReader rd = null;

			string sql = @" SELECT m.plantcd, t.startdt, t.enddt
                        FROM TnTran AS t WITH(nolock)
                        INNER JOIN TmMachine AS m WITH(nolock) ON t.macno = m.macno
                        WHERE (t.delfg = 0) and t.lotno = @LotNO ";

            if (useLikeClassNM == true)
            {
                sql += "  and m.clasnm LIKE @ClassNM + '%' ";
            }
            else
            {
                sql += "  and m.clasnm = @ClassNM ";
            }

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
			{
				conn.SetParameter("@LotNO", System.Data.SqlDbType.NVarChar, lotNO);
				conn.SetParameter("@ClassNM", System.Data.SqlDbType.NVarChar, modelNM);

				using (rd = conn.GetReader(sql))
				{
					int ordPlantCD = rd.GetOrdinal("plantcd");
					int ordStartDT = rd.GetOrdinal("startdt");
					int ordEndDT = rd.GetOrdinal("enddt");

					while (rd.Read())
					{
						Tran tranInfo = new Tran();

						tranInfo.PlantCD = rd.GetString(ordPlantCD);

						if (rd.IsDBNull(ordStartDT) == false)
						{
							tranInfo.StartDT = rd.GetDateTime(ordStartDT);
						}
						else
						{
							throw new Exception(string.Format("実績[TnTran]の開始時刻が取得出来ませんでした。 ライン：{0} / 装置：{1} / ロットNO：{2}", lineCD, modelNM, lotNO));
						}

						if (rd.IsDBNull(ordEndDT) == false)
						{
							tranInfo.EndDT = rd.GetDateTime(ordEndDT);
						}
						else
						{
							tranInfo.EndDT = null;
						}

						tranList.Add(tranInfo);
					}
				}
			}

			return tranList;
		}

		public static List<Tran> GetDataByWorkCD(int lineCD, string workCD, string lotNO)
		{
			List<Tran> tranList = new List<Tran>();
			System.Data.Common.DbDataReader rd = null;

			string sql = @" SELECT m.plantcd, t.startdt, t.enddt
                        FROM TnTran AS t WITH(nolock) 
						INNER JOIN TmProcess AS p on t.procno = p.procno
						INNER JOIN TmMachine As m on t.macno = m.macno
                        WHERE (t.delfg = 0) and (m.delfg = 0) and (p.delfg = 0) and t.lotno = @LotNO and p.workcd = @WorkCD ";

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
			{
				conn.SetParameter("@LotNO", System.Data.SqlDbType.NVarChar, lotNO);
				conn.SetParameter("@WorkCD", System.Data.SqlDbType.NVarChar, workCD);
				
				using (rd = conn.GetReader(sql))
				{
					int ordPlantCD = rd.GetOrdinal("plantcd");
					int ordStartDT = rd.GetOrdinal("startdt");
					int ordEndDT = rd.GetOrdinal("enddt");

					while (rd.Read())
					{
						Tran tranInfo = new Tran();

						tranInfo.PlantCD = rd.GetString(ordPlantCD);

						if (rd.IsDBNull(ordStartDT) == false)
						{
							tranInfo.StartDT = rd.GetDateTime(ordStartDT);
						}
						else
						{
							throw new Exception(string.Format("実績[TnTran]の開始時刻が取得出来ませんでした。 ライン：{0} / 作業CD：{1} / ロットNO：{2}", lineCD, workCD, lotNO));
						}

						if (rd.IsDBNull(ordEndDT) == false)
						{
							tranInfo.EndDT = rd.GetDateTime(ordEndDT);
						}
						else
						{
							tranInfo.EndDT = null;
						}

						tranList.Add(tranInfo);
					}
				}
			}

			return tranList;
		}
        
        public static Tran GetWBData(int lineCD, string lotNO)
        {
            List<Tran> tranList = GetData(lineCD, "ワイヤーボンダー", lotNO, true);
            if (tranList == null)
            {
                return null;
            }
            else if (tranList.Count == 1)
            {
                return tranList.Single();
            }
            else
            {
                // FD-MAP品種(検査ありWB作業なし)があるの為、WB実績が無くてもエラー扱いにしない
                return null;
            }
        }
    }
}
