using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
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


		public static List<Equi> GetEquipmentInfo(int lineCD, string equipNO)
		{
			return GetEquipmentInfo(lineCD, null, equipNO, null, null);
		}

		public static List<Equi> GetEquipmentInfo(int lineCD, string equipNO, string modelNM, bool? delFG)
		{
			return GetEquipmentInfo(lineCD, null, equipNO, modelNM, delFG);
		}

		public static List<Equi> GetEquipmentInfo(int lineCD, int? assetsCD)
		{
			return GetEquipmentInfo(lineCD, assetsCD, null, null, null);
		}

		public static List<Equi> GetEquipmentInfo(int lineCD, int? assetsCD, string equipNO)
		{
			return GetEquipmentInfo(lineCD, assetsCD, equipNO, null, null);
		}

		public static List<Equi> GetEquipmentInfo(int lineCD, int? assetsCD, string equipNO, string modelNM)
		{
			return GetEquipmentInfo(lineCD, assetsCD, equipNO, modelNM, null);
		}

		/// <summary>
		/// 装置情報を取得
		/// </summary>
		/// <returns></returns>
		public static List<Equi> GetEquipmentInfo(int lineCD, int? assetsCD, string equipNO, string modelNM, bool? delFG)
		{
			List<Equi> equiList = new List<Equi>();

			try
			{
				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
				{
					string sql = @" SELECT Equipment_NO, Model_NM, Assets_NM, MachinSeq_NO, Del_FG
                                    FROM dbo.TmEQUI AS TmEQUI WITH(nolock) WHERE 1=1 ";

					if (assetsCD != null)
					{
						sql += @" AND TmEQUI.Assets_CD = @AssetsCD ";// +assetsCD;
						conn.SetParameter("@AssetsCD", SqlDbType.Int, assetsCD);
					}

					if (equipNO != null)
					{
						sql += @" AND TmEQUI.Equipment_NO = @EquipmentNO ";
						conn.SetParameter("@EquipmentNO", SqlDbType.NVarChar, equipNO);
					}

					if (modelNM != null)
					{
						sql += @" AND TmEQUI.Model_NM = @ModelNM ";
						conn.SetParameter("@ModelNM", SqlDbType.NVarChar, modelNM);
					}

					if (delFG != null)
					{
						sql += @" AND TmEQUI.Del_FG = @DelFG ";
						conn.SetParameter("@DelFG", SqlDbType.Bit, delFG.Value);
					}

					sql += @" Option(MAXDOP 1) ";

					conn.Command.CommandText = sql;

					using (DbDataReader rd = conn.Command.ExecuteReader())
					{
						int ordEquipmentNO = rd.GetOrdinal("Equipment_NO");
						int ordModelNM = rd.GetOrdinal("Model_NM");
						int ordAssetsNM = rd.GetOrdinal("Assets_NM");
						int ordMachineSeqNO = rd.GetOrdinal("MachinSeq_NO");
						int ordDelFG = rd.GetOrdinal("Del_FG");

						while (rd.Read())
						{
							Equi equi = new Equi();

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
				throw;
			}
		}

		private string GetModel(string connStr, string sEquiNo)
		{
			string sModel = "";
			string BaseSql = "SELECT Model_NM FROM TmEQUI WITH(NOLOCK) Where Equipment_NO='{0}'";

			string sqlCmdTxt = string.Format(BaseSql, sEquiNo);

			using (DBConnect connect = DBConnect.CreateInstance(connStr, false))
			{
				connect.Command.CommandText = sqlCmdTxt;
				using(DbDataReader reader = connect.Command.ExecuteReader())

				while (reader.Read())
				{
					sModel = Convert.ToString(reader["Model_NM"]).Trim();  //型式
				}
			}
			return sModel;
		}

        /// <summary>
        /// delfgを0にする
        /// </summary>
        /// <param name="inlineCd"></param>
        /// <param name="plantCd"></param>
        /// <param name="modelNm"></param>
        public static void UpdateDelFgOff(int inlineCd, string plantCd, string modelNm)
        {
            updateDelFg(inlineCd, plantCd, modelNm, false);
        }

        /// <summary>
        /// delfgを1にする
        /// </summary>
        /// <param name="inlineCd"></param>
        /// <param name="plantCd"></param>
        /// <param name="modelNm"></param>
        public static void UpdateDelFgOn(int inlineCd, string plantCd, string modelNm)
        {
            updateDelFg(inlineCd, plantCd, modelNm, true);
        }

        /// <summary>
        /// delfgを更新する
        /// </summary>
        /// <param name="inlineCd"></param>
        /// <param name="plantCd"></param>
        /// <param name="newModel"></param>
        /// <param name="delfg"></param>
        private static void updateDelFg(int inlineCd, string plantCd, string modelNm, bool delfg)
        {
            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, inlineCd), "System.Data.SqlClient", false))
            {
                string sql = @" UPDATE TmEQUI
                            SET Del_FG = @DELFG
                            WHERE Equipment_NO = @EQUIPMENTNO ";

                if (string.IsNullOrWhiteSpace(modelNm) == false)
                {
                    sql += " AND Model_NM = @MODELNM ";
                    conn.SetParameter("@MODELNM", SqlDbType.VarChar, modelNm);
                }
                conn.SetParameter("@DELFG", SqlDbType.Bit, delfg);
                conn.SetParameter("@EQUIPMENTNO", SqlDbType.VarChar, plantCd);

                conn.ExecuteNonQuery(sql);
            }
        }
    }
}
