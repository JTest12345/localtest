using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data;
using System.Data.Common;
using GEICS;
using GEICS.Report;

namespace GEICS
{
    /// <summary>
    /// 汎用
    /// </summary>
    public class General
    {
        public const string GROUPCD_PARTSCHANGEDEFECT = "5";
        public const string GROUPCD_LITTLERESINTRANCD = "6";
        public const string GROUPCD_LINE = "7";
        public const string GROUPCD_ASSETS = "ASSETS";

        public bool IsCheck { get; set; }

        /// <summary>汎用CD</summary>
        public string GeneralCD { get; set; }

        /// <summary>汎用名</summary>
        public string GeneralNM { get; set; }

        /// <summary>
        /// 汎用データを取得
        /// </summary>
        /// <param name="generalGrpCD">汎用区分CD</param>
        /// <returns>汎用データ</returns>
        public static List<General> GetGeneralData(object generalGrpCD)
        {
            List<General> generalList = new List<General>();

			using (DBConnect conn = DBConnect.CreateInstance(Constant.StrQCIL, "System.Data.SqlClient", false))
            {
                string sql = @" SELECT General_CD, General_NM
                                FROM TmGENERAL WITH(nolock) 
                                WHERE Del_FG = 0 AND (GeneralGrp_CD = @GeneralGrp_CD)";

                conn.SetParameter("GeneralGrp_CD", SqlDbType.Char, generalGrpCD);

                using (DbDataReader rd = conn.GetReader(sql))
                {
                    if (!rd.HasRows) 
                    {
						throw new ApplicationException(string.Format("TmGENERALマスタが設定されていません。GeneralGrp_CD:{0}", generalGrpCD));
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

	class Parameter
	{
		public string LotNO { get; set; }
        public string MagazineNO { get; set; }
		public int ParameterIndex { get; set; }
		public string ParameterNM { get; set; }
        public int QcParamNO { get; set; }
		public bool IsDecimalValue { get; set; }
		public object ParameterVAL { get; set; }
        public DateTime MeasureDT { get; set; }
	}

	class ParameterReport
	{
		public List<ParameterPerLotReport> LotInfoList { get; set; }
		public List<Parameter> ParameterList { get; set; }
	}

}
