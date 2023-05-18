using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;
using System.Data;

namespace EICS.Database
{
	class Lset
	{
		public int InlineCD { get; set; }
		public string EquipmentNO { get; set; }
		public string EquipmentCD { get; set; }
		public int ProcessCD { get; set; }
		public string SeqCD { get; set; }
		public string IPAddressNO { get; set; }
		public int PortNO { get; set; }
		public string InputFolderNM { get; set; }
		public bool DelFG { get; set; }
		public string UpdUserCD { get; set; }
		public DateTime LastUpdDT { get; set; }
		public string ThreadGrpCD { get; set; }
		public bool MainThreadFG { get; set; }
		public string EquipPartID { get; set; }
		public string WorkingType_CD { get; set; }
		public bool ArmsTypeNoCheck_FG { get; set; }
        public bool ReferMultiServer_FG { get; set; }
		public bool EnableResultPriorityJudge_FG { get; set; }

		public string WorkingTypeGroup_CD { get; set; }

        public string ChipNM { get; set; }
        
        public static List<Lset> GetLsetData(int inlineCd)
		{
			List<Lset> retv = new List<Lset>();

			try
			{
				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, inlineCd), "System.Data.SqlClient", false))
				{
					string sql = @" SELECT Inline_CD, Equipment_NO, Equipment_CD, Process_CD, Seq_NO, IPAddress_NO, Port_NO,
                                    InputFolder_NM, LoaderAddress_NO, LoaderPlcNode_NO, Del_FG, UpdUser_CD, LastUpd_DT,
                                    ThreadGrp_CD, MainThread_FG, EquipPart_ID, WorkingType_CD, ArmsTypeNoCheck_FG, ReferMultiServer_FG,
									EnableResultPriorityJudge_FG, WorkingTypeGroup_CD, Chip_NM
                                    FROM TmLSET WITH (nolock)
                                    WHERE Inline_CD = @InlineCd AND Del_FG = 0 ";


					conn.SetParameter("@InlineCD", SqlDbType.VarChar, inlineCd);

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordWorkTypeGr = rd.GetOrdinal("WorkingTypeGroup_CD");

						while (rd.Read())
						{
							Lset lsetdata = new Lset();

							lsetdata.InlineCD = Convert.ToInt32(rd["Inline_CD"]);
							lsetdata.EquipmentNO = Convert.ToString(rd["Equipment_NO"]).Trim();
							lsetdata.EquipmentCD = Convert.ToString(rd["Equipment_CD"]).Trim();
							lsetdata.ProcessCD = Convert.ToInt32(rd["Process_CD"]);
							lsetdata.SeqCD = Convert.ToString(rd["Seq_NO"]).Trim();
							lsetdata.IPAddressNO = Convert.ToString(rd["IPAddress_NO"]).Trim();
							lsetdata.InputFolderNM = Convert.ToString(rd["InputFolder_NM"]).Trim();
							lsetdata.DelFG = Convert.ToBoolean(rd["Del_FG"]);
							lsetdata.UpdUserCD = Convert.ToString(rd["UpdUser_CD"]).Trim();
							lsetdata.LastUpdDT = Convert.ToDateTime(rd["LastUpd_DT"]);
							lsetdata.ThreadGrpCD = Convert.ToString(rd["ThreadGrp_CD"]).Trim();
							//lsetdata.MainThreadFG = Convert.ToBoolean(rd["MainThread_FG"]);
							lsetdata.EquipPartID = Convert.ToString(rd["EquipPart_ID"]).Trim();
							lsetdata.WorkingType_CD = Convert.ToString(rd["WorkingType_CD"]).Trim();
							lsetdata.ArmsTypeNoCheck_FG = Convert.ToBoolean(rd["ArmsTypeNoCheck_FG"]);
                            lsetdata.ReferMultiServer_FG = Convert.ToBoolean(rd["ReferMultiServer_FG"]);
							lsetdata.EnableResultPriorityJudge_FG = Convert.ToBoolean(rd["EnableResultPriorityJudge_FG"]);

							if (rd.IsDBNull(ordWorkTypeGr))
							{
								lsetdata.WorkingTypeGroup_CD = null;
							}
							else
							{
								lsetdata.WorkingTypeGroup_CD = rd.GetString(ordWorkTypeGr).Trim();
							}

							int ordPortNO = rd.GetOrdinal("Port_NO");
							if (!rd.IsDBNull(ordPortNO))
							{
								lsetdata.PortNO = Convert.ToInt32(rd.GetString(ordPortNO));
							}

                            lsetdata.ChipNM = Convert.ToString(rd["Chip_NM"]).Trim();

							retv.Add(lsetdata);
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

		public static bool UpdateWorkingType(int inlineCd, string plantCd, string newType, string newTypeGr)
		{
#if TEST
			return false;
#endif

			bool retv = false;

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, inlineCd), "System.Data.SqlClient", false))
			{
				string sql = @" UPDATE TmLSET
                            SET WorkingType_CD = @NEWTYPE, WorkingTypeGroup_CD = @NEWTYPEGR
                            WHERE Inline_CD = @INLINECD AND Equipment_NO = @PLANTCD";

				conn.SetParameter("@INLINECD", SqlDbType.Int, inlineCd);
				conn.SetParameter("@NEWTYPE", SqlDbType.VarChar, newType);
				conn.SetParameter("@PLANTCD", SqlDbType.VarChar, plantCd);
				conn.SetParameter("@NEWTYPEGR", SqlDbType.VarChar, newTypeGr ?? (object)DBNull.Value);

				conn.ExecuteNonQuery(sql);

				retv = true;
			}

			return retv;
		}

        public static bool UpdateChipName(int inlineCd, string plantCd, string chipNm)
        {
            bool retv = false;

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, inlineCd), "System.Data.SqlClient", false))
            {
                string sql = @" UPDATE TmLSET
                            SET Chip_NM = @CHIPNM
                            WHERE Inline_CD = @INLINECD AND Equipment_NO = @PLANTCD ";

                conn.SetParameter("@INLINECD", SqlDbType.Int, inlineCd);
                conn.SetParameter("@PLANTCD", SqlDbType.VarChar, plantCd);
                conn.SetParameter("@CHIPNM", SqlDbType.VarChar, chipNm);

                conn.ExecuteNonQuery(sql);

                retv = true;
            }

            return retv;
        }
    }
}
