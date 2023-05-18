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
    /// 汎用
    /// </summary>
    class General
    {
        /// <summary>
        /// 汎用区分
        /// </summary>
        public enum GeneralGrp
        {
            PartsChangeDefect = 5,
            LittleResinTranCD = 6,
			DBExtractSkipInfo = 9
        }


        /// <summary>汎用CD</summary>
        public string GeneralCD { get; set; }

        /// <summary>汎用名</summary>
        public string GeneralNM { get; set; }

        /// <summary>
        /// 汎用データを取得
        /// </summary>
        /// <param name="generalGrpCD">汎用区分CD</param>
        /// <returns>汎用データ</returns>
        public static List<General> GetGeneralData(object generalGrpCD, int lineCD)
        {
            List<General> generalList = new List<General>();

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCD), "System.Data.SqlClient", false))
            {
                string sql = @" SELECT General_CD, General_NM
                                FROM TmGENERAL WITH(nolock) 
                                WHERE Del_FG = 0 AND (GeneralGrp_CD = @GeneralGrp_CD)";

                conn.SetParameter("GeneralGrp_CD", SqlDbType.Char, generalGrpCD);

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    if (!rd.HasRows) 
                    {
                        throw new Exception(string.Format(Constant.MessageInfo.Message_89, generalGrpCD));
                    }

                    int ordGeneralCD = rd.GetOrdinal("General_CD");
                    int ordGeneralNM = rd.GetOrdinal("General_NM");

                    while (rd.Read())
                    {
                        General general = new General();
                        general.GeneralCD = rd.GetString(ordGeneralCD).Trim();
                        general.GeneralNM = rd.GetString(ordGeneralNM).Trim();
                        generalList.Add(general);
                    }
                }
            }

            return generalList;
        }
    }
}
