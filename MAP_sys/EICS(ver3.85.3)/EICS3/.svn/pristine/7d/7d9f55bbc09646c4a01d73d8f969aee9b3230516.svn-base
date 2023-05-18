using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;

namespace EICS.Database
{
	public class ErrCommunicate
	{
		public int SendLineCD { get; set; }
        public DateTime AlertDT { get; set; }
		public string EquipmentNO { get; set; }
        public int RecvLineCD { get; set; }
        public string MessageVAL { get; set; }

		public static void Insert(int dbServerLineCD, int sendLineCD, DateTime alertDT, string equipmentNO, int recvLineCD, string errorMsgVAL)
		{
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, dbServerLineCD), "System.Data.SqlClient", false))
			{
				try
				{
                    string sql = @" INSERT INTO TnErrCommunicate
											(SendLine_CD, Alert_DT, ReceiveLine_CD, Equipment_NO, Message_VAL)
									VALUES	(@SendLineCD, @AlertDT, @ReceiveLineCD, @EquipmentNO, @MessageVAL) ";

					conn.SetParameter("@SendLineCD", SqlDbType.Int, Common.GetParameterValue(sendLineCD));
					conn.SetParameter("@AlertDT", SqlDbType.Char, Common.GetParameterValue(alertDT));
					conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, Common.GetParameterValue(equipmentNO));
					conn.SetParameter("@ReceiveLineCD", SqlDbType.Int, Common.GetParameterValue(recvLineCD));
					conn.SetParameter("@MessageVAL", SqlDbType.NVarChar, Common.GetParameterValue(errorMsgVAL));

					conn.ExecuteNonQuery(sql);
				}
				catch (Exception err)
				{
					throw new Exception(err.Message);
				}
			}
		}

		public static List<ErrCommunicate> GetData(int dbServerLineCD, int receiveLineCD)
		{
			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, dbServerLineCD), "System.Data.SqlClient", false))
			{
				List<ErrCommunicate> errcomms = new List<ErrCommunicate>();

				string causeModelChipNM = string.Empty;

                string sql = @" SELECT SendLine_CD, Alert_DT, ReceiveLine_CD, Equipment_NO, Message_VAL
								FROM TnErrCommunicate WITH(NOLOCK) 
								WHERE (ReceiveLine_CD = @ReceiveLineCD) OPTION(MAXDOP 1) ";

				conn.SetParameter("@ReceiveLineCD", SqlDbType.Int, receiveLineCD);

				using (System.Data.Common.DbDataReader rd = conn.GetReader(sql))
				{
                    int ordSendLineCD = rd.GetOrdinal("SendLine_CD");
                    int ordAlertDT = rd.GetOrdinal("Alert_DT");
                    int ordRecvLineCD = rd.GetOrdinal("ReceiveLine_CD");
                    int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
                    int ordMessageVAL = rd.GetOrdinal("Message_VAL");

					while (rd.Read())
					{
						ErrCommunicate errcomm = new ErrCommunicate();

						errcomm.SendLineCD = rd.GetInt32(ordSendLineCD);
						errcomm.AlertDT = rd.GetDateTime(ordAlertDT);
                        errcomm.RecvLineCD = rd.GetInt32(ordRecvLineCD);
						errcomm.EquipmentNO = rd.GetString(ordEquipmentNO);
                        errcomm.MessageVAL = rd.GetString(ordMessageVAL);

                        errcomms.Add(errcomm);
					}
				}

                return errcomms;
			}
		}

		public static void DeleteData(int dbServerLineCD, ErrCommunicate err)
		{
			string sql = @" DELETE FROM TnErrCommunicate WHERE (SendLine_CD = @SendLineCD) and (Alert_DT = @AlertDT) and (ReceiveLine_CD = @ReceiveLineCD)
								and (Equipment_NO = @EquipmentNO) and (Message_VAL = @MessageVAL) ";

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, dbServerLineCD), "System.Data.SqlClient", false))
			{
				conn.SetParameter("@SendLineCD", SqlDbType.Int, err.SendLineCD);
				conn.SetParameter("@AlertDT", SqlDbType.DateTime, err.AlertDT);
				conn.SetParameter("@ReceiveLineCD", SqlDbType.Int, err.RecvLineCD);
				conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, err.EquipmentNO);
				conn.SetParameter("@MessageVAL", SqlDbType.NVarChar, err.MessageVAL);

				conn.ExecuteNonQuery(sql);
			}
		}

		public static void DeleteData(int dbServerLineCD, List<ErrCommunicate> errList)
		{
			foreach(ErrCommunicate err in errList)
			{
				DeleteData(dbServerLineCD, err);
			}
		}

	}
}
