using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Arms
{
	public class LotTray
	{
		public string TrayNo { get; set; }
		public string LotNo { get; set; }
		public int OrderNo { get; set; }

        public static List<LotTray> GetCurrentFromMultipleServer(int lineCD, string lotNo, string trayNo, int? orderNo, bool referMultiServerFG)
        {
            List<LotTray> list = GetRelateTray(lineCD, lotNo, trayNo, orderNo);

            SettingInfo commonSettingInfo = SettingInfo.GetSingleton();
            
            // 他の指定サーバからもデータを取得する。
            if (referMultiServerFG)
            {
                foreach (string serverNm in commonSettingInfo.ArmsServerList)
                {
                    list.AddRange(GetRelateTray(serverNm, lotNo, trayNo, orderNo));
                }
            }

            return list;
        }

        public static List<LotTray> GetRelateTray(int lineCD, string lotNo, string trayNo, int? orderNo)
        {
            return GetRelateTray(lotNo, trayNo, orderNo, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD));
        }

        public static List<LotTray> GetRelateTray(string serverNm, string lotNo, string trayNo, int? orderNo)
        {
            return GetRelateTray(lotNo, trayNo, orderNo, ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, serverNm));
        }

        public static List<LotTray> GetRelateTray(string lotNo, string trayNo, int? orderNo, string constr)
		{
			List<LotTray> lotTrayList = new List<LotTray>();

			using (SqlConnection con = new SqlConnection(constr))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT trayno, lotno, orderno
                                                FROM TnLotTray WITH(nolock)
                                                WHERE newfg = 1 ";

				if (string.IsNullOrEmpty(lotNo) == false)
				{
					sql += " AND lotno = @LotNo ";
					cmd.Parameters.Add("@LotNo", SqlDbType.NVarChar).Value = lotNo;
				}

				if (string.IsNullOrEmpty(trayNo) == false)
				{
					sql += " AND trayno = @TrayNo ";
					cmd.Parameters.Add("@TrayNo", SqlDbType.NVarChar).Value = trayNo;
				}

                if (orderNo.HasValue)
                {
                    sql += " AND orderno = @OrderNo ";
                    cmd.Parameters.Add("@OrderNo", SqlDbType.Int).Value = orderNo.Value;
                }

				cmd.CommandText = sql;
				SqlDataReader rd = cmd.ExecuteReader();
				while (rd.Read())
				{
					LotTray tray = new LotTray();

					tray.TrayNo = rd["trayno"].ToString().Trim();
					tray.LotNo = rd["lotno"].ToString().Trim();
					tray.OrderNo = Convert.ToInt32(rd["orderno"]);

					lotTrayList.Add(tray);
				}
			}

			return lotTrayList;
		}
	}
}
