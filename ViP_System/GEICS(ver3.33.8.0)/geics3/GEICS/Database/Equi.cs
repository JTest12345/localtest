using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Collections.Specialized;

namespace GEICS.Database
{
	class Equi
	{
		public string ServerNM { get; set; }
		public string EquipmentNO { get; set; }
		public string ModelNM { get; set; }
		public string AssetsNM { get; set; }
		public string MachineSeqNO { get; set; }
		public bool DelFG { get; set; }
		public int AssetsCD { get; set; }


		public static List<Equi> GetEquipmentInfo(string serverNM, string connStr, string equipNO)
		{
			return GetEquipmentInfo(serverNM, connStr, null, equipNO, null, null);
		}

		public static List<Equi> GetEquipmentInfo(string serverNM, string connStr, string equipNO, string modelNM, bool? delFG)
		{
			return GetEquipmentInfo(serverNM, connStr, null, equipNO, modelNM, delFG);
		}

		public static List<Equi> GetEquipmentInfo(string serverNM, string connStr, int? assetsCD)
		{
			return GetEquipmentInfo(serverNM, connStr, assetsCD, null, null, null);
		}

		public static List<Equi> GetEquipmentInfo(string serverNM, string connStr, int? assetsCD, string equipNO)
		{
			return GetEquipmentInfo(serverNM, connStr, assetsCD, equipNO, null, null);
		}

		public static List<Equi> GetEquipmentInfo(string serverNM, string connStr, int? assetsCD, string equipNO, string modelNM)
		{
			return GetEquipmentInfo(serverNM, connStr, assetsCD, equipNO, modelNM, null);
		}

		/// <summary>
		/// 装置情報を取得
		/// </summary>
		/// <returns></returns>
		public static List<Equi> GetEquipmentInfo(string serverNM, string connStr, int? assetsCD, string equipNO, string modelNM, bool? delFG)
		{
			SqlDataReader rd = null;
			List<Equi> equiList = new List<Equi>();

			try
			{
				using (IConnection conn = NascaConnection.CreateInstance(connStr, false))
				{
					string sql = @" SELECT Equipment_NO, Model_NM, Assets_NM, MachinSeq_NO, Del_FG
                                    FROM dbo.TmEQUI AS TmEQUI WITH(nolock) WHERE 1=1 ";

					if (assetsCD != null)
					{
						sql += @" AND TmEQUI.Assets_CD = @AssetsCD ";// +assetsCD;
						conn.Command.Parameters.Add(new SqlParameter("@AssetsCD", assetsCD));
					}

					if (equipNO != null)
					{
						sql += @" AND TmEQUI.Equipment_NO = @EquipmentNO ";
						conn.Command.Parameters.Add(new SqlParameter("@EquipmentNO", equipNO));
					}

					if (modelNM != null)
					{
						sql += @" AND TmEQUI.Model_NM = @ModelNM ";
						conn.Command.Parameters.Add(new SqlParameter("@ModelNM", modelNM));
					}

					if (delFG != null)
					{
						sql += @" AND TmEQUI.Del_FG = @DelFG ";
						conn.Command.Parameters.Add(new SqlParameter("@DelFG", delFG.Value));
					}

					sql += @" Option(MAXDOP 1) ";

					conn.Command.CommandText = sql;

					using (rd = conn.Command.ExecuteReader())
					{
						int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
						int ordModelNM = rd.GetOrdinal("Model_NM");
						int ordAssetsNM = rd.GetOrdinal("Assets_NM");
						int ordMachineSeqNO = rd.GetOrdinal("MachinSeq_NO");
						int ordDelFG = rd.GetOrdinal("Del_FG");
						
						while (rd.Read())
						{
							Equi equi = new Equi();

							equi.ServerNM = serverNM;
							equi.EquipmentNO = rd.GetString(ordEquipmentNO);
							equi.ModelNM = rd.GetString(ordModelNM);
							equi.AssetsNM = rd.GetString(ordAssetsNM);
							equi.MachineSeqNO = rd.GetString(ordMachineSeqNO);
							equi.DelFG = rd.GetBoolean(ordDelFG);

							equiList.Add(equi);
						}
					}
				}

				return equiList;
			}
			catch (Exception err)
			{
				throw err;
			}
		}

		private string GetModel(string sEquiNo)
		{
			string sModel = "";
			string BaseSql = "SELECT Model_NM FROM TmEQUI WITH(NOLOCK) Where Equipment_NO='{0}'";

			string sqlCmdTxt = string.Format(BaseSql, sEquiNo);

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				SqlDataReader reader = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					reader = connect.Command.ExecuteReader();

					while (reader.Read())
					{
						sModel = Convert.ToString(reader["Model_NM"]).Trim();  //型式
					}
				}
				finally
				{
					if (reader != null) reader.Close();
					connect.Close();
				}
			}
			return sModel;
		}
	}
}
