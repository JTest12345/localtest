using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;

namespace EICS.Database
{
	public class CommunicateLine
	{
		public int HostLineCD { get; set; }
		public int RemoteLineCD { get; set; }

		public static List<CommunicateLine> GetData(int hostLineCD)
		{
			List<CommunicateLine> comms = new List<CommunicateLine>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, hostLineCD), "System.Data.SqlClient", false))
			{
                string sql = @" SELECT HostLine_CD, RemoteLine_CD
							FROM TmCommunicateLine WITH(nolock) 
							WHERE HostLine_CD = @HostLineCD AND TmCommunicateLine.Del_FG = 0 OPTION(MAXDOP 1) ";

				conn.SetParameter("@HostLineCD", SqlDbType.Int, hostLineCD);

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int ord_HostLineCD = rd.GetOrdinal("HostLine_CD");
					int ord_RemoteLineCD = rd.GetOrdinal("RemoteLine_CD");

					while (rd.Read())
					{
						CommunicateLine comm = new CommunicateLine();

						comm.HostLineCD = rd.GetInt32(ord_HostLineCD);
						comm.RemoteLineCD = rd.GetInt32(ord_RemoteLineCD);

                        comms.Add(comm);
					}
				}
			}
			return comms;

		}
	}
}
