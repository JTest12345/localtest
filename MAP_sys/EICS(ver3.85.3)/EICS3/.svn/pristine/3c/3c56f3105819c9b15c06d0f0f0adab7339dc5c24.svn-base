using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;

namespace EICS.Database
{
	class Serv
	{
		public int InLineCD { get; set; }
		public string ServerNO { get; set; }
		public string DatabaseNM { get; set; }
		public bool MainServerFG { get; set; }
		public bool SubServerFG { get; set; }
		public bool DelFG { get; set; }
		public DateTime LastUpdDT { get; set; }

		DBConnect Connection;

		public Serv(DBConnect conn)
		{
			this.Connection = conn;
		}

		public Serv()
		{
		}

		public static List<Serv> GetData(int lineCD)
		{
			List<Serv> servList = new List<Serv>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
			{
				string sql = @" Select TmServ.InLine_CD, Server_NO, Database_NM
								From TmServ with(nolock)
								Where SubServer_FG = '1' And TmServ.Del_FG = '0' ";

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int InLineCD_Ord = rd.GetOrdinal("InLine_CD");
					int ServerNO_Ord = rd.GetOrdinal("Server_NO");
					int DatabaseNM_Ord = rd.GetOrdinal("Database_NM");

					while (rd.Read())
					{
						Serv serv = new Serv();

						serv.InLineCD = rd.GetInt32(InLineCD_Ord);
						serv.ServerNO = rd.GetString(ServerNO_Ord);
						serv.DatabaseNM = rd.GetString(DatabaseNM_Ord);

						servList.Add(serv);
					}
				}
			}
			return servList;
		}

		public static string GetIPAddrFromLineCD(int lineCD)
		{
			string ipAddr = string.Empty;
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
			{
				string sql = @" Select Server_NO
								From TmServ with(nolock)
								Where Del_FG = '0' ";

				using (DbDataReader rd = conn.GetReader(sql))
				{
					//int InLineCD_Ord = rd.GetOrdinal("InLine_CD");
					int ServerNO_Ord = rd.GetOrdinal("Server_NO");
					//int DatabaseNM_Ord = rd.GetOrdinal("Database_NM");

					while (rd.Read())
					{
						ipAddr = rd.GetString(ServerNO_Ord);
					}
				}
			}
			return ipAddr;
		}

		public static string GetDBServerAddr(int serverLineCD, int lineCD)
		{
			try
			{
				string dbServerAddr = string.Empty;
				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, serverLineCD), "System.Data.SqlClient", false))
				{
					string sql = @" Select DBServerAddr_VAL
								From TmServ with(nolock)
								Where Del_FG = '0' And InLine_CD = @LineCD Option (MAXDOP 1) ";

					conn.SetParameter("@LineCD", SqlDbType.Int, lineCD);

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int DBServerAddrVAL_Ord = rd.GetOrdinal("DBServerAddr_VAL");

						while (rd.Read())
						{
							dbServerAddr = rd.GetString(DBServerAddrVAL_Ord);
						}
					}
				}
				return dbServerAddr;
			}
			catch (Exception err)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, err.Message);
				throw new Exception(err.Message);
			}
		}
	}
}
