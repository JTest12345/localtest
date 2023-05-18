using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;

namespace EICS.Database
{
    /// <summary>
    /// 不良(PDA入力)
    /// </summary>
    class Defect
    {
        /// <summary>ラインNO</summary>
        public int LineNO { get; set; }

        /// <summary>設備CD</summary>
        public string PlantCD { get; set; }

        /// <summary>ロット番号</summary>
        public string LotNO { get; set; }

        /// <summary>アドレスNO</summary>
        public int DefAddressNO { get; set; }

        /// <summary>ユニットNO</summary>
        public string DefUnitNO { get; set; }

        /// <summary>
        /// 不良データを取得
        /// </summary>
        /// <param name="lineNO">ラインNO</param>
        /// <param name="plantCD">設備CD</param>
        /// <param name="lotNO">ロットNO</param>
        /// <param name="addressNO">アドレスNO</param>
        /// <param name="unitNO">ユニットNO</param>
        /// <returns>不良データ</returns>
        public static List<Defect> GetDefectData(int severLineNO, int? lineNO, string plantCD, string lotNO, string addressNO, string unitNO, string defectCD)
        {
            List<Defect> defectList = new List<Defect>();

			using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, severLineNO, lineNO), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT Line_CD, Plant_CD, Lot_NO, DefAddress_NO, DefUnit_NO, Target_DT, Work_CD, DefItem_CD, DefItem_NM, DefCause_CD, 
                                  DefCause_NM, DefClass_CD, DefClass_NM, Tran_CD, UpdateTran_CD, UpdUser_CD, LastUpd_DT
                                FROM TnDEFECT WITH(nolock)
                                WHERE (Del_FG = 0) AND (Plant_CD = @Plant_CD) 
                                AND (Lot_NO = @Lot_NO) ";

				if (lineNO.HasValue)
				{
					sql += " AND (Line_CD = @Line_CD) ";
					conn.SetParameter("@Line_CD", SqlDbType.Int, lineNO);
				}

                conn.SetParameter("@Plant_CD", SqlDbType.Char, plantCD);
                conn.SetParameter("@Lot_NO", SqlDbType.VarChar, lotNO);

                if (addressNO != string.Empty)
                {
                    sql += " AND (DefAddress_NO = @DefAddress_NO) ";
                    conn.SetParameter("@DefAddress_NO", SqlDbType.VarChar, addressNO);
                }
                if (unitNO != string.Empty)
                {
                    sql += " AND (DefUnit_NO = @DefUnit_NO) ";
                    conn.SetParameter("@DefUnit_NO", SqlDbType.VarChar, unitNO);
                }

                if (defectCD != string.Empty)
                {
                    sql += " AND (DefItem_CD = @DefItem_CD) ";
                    conn.SetParameter("@DefItem_CD", SqlDbType.Char, defectCD);
                }
                
                using (DbDataReader rd = conn.GetReader(sql))
                {
                    int ordLineNO = rd.GetOrdinal("Line_CD");
                    int ordPlantCD = rd.GetOrdinal("Plant_CD");
                    int ordLotNO = rd.GetOrdinal("Lot_NO");
                    int ordAddressNO = rd.GetOrdinal("DefAddress_NO");
                    int ordUnitNO = rd.GetOrdinal("DefUnit_NO");

                    while (rd.Read())
                    {
                        Defect defect = new Defect();

                        defect.LineNO = rd.GetInt32(ordLineNO);
                        defect.PlantCD = rd.GetString(ordPlantCD).Trim();
                        defect.LotNO = rd.GetString(ordLotNO);
                        defect.DefAddressNO = Convert.ToInt32(rd.GetString(ordAddressNO));
                        defect.DefUnitNO = rd.GetString(ordUnitNO);

                        defectList.Add(defect);
                    }
                }
            }

            return defectList;

        }

        public static List<Defect> GetDefectData(int lineNO, string plantCD, string lotNO, string defectCD) 
        {
            return GetDefectData(lineNO, null, plantCD, lotNO, string.Empty, string.Empty, defectCD);
        }
    }
}
