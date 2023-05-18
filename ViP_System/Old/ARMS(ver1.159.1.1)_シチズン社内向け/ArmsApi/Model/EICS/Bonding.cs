using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.EICS
{
    public class Bonding
    {
        public string TypeCd { get; set; }

        public int ProcNo { get; set; }

        /// <summary>
        /// ﾁｯﾌﾟ搭載数
        /// </summary>
        public int ChipBondCt { get; set; }

        /// <summary>
        /// 1ﾁｯﾌﾟのDB樹脂塗布数
        /// </summary>
        public int? DbDispenseCt { get; set; }

        
        public static Bonding GetSetting(string typeCd, int procNo)
        {
            Bonding retv = null;
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = $@" SELECT Type_CD, Proc_NO, ChipBond_CT, DbDispense_CT
                                        FROM TmBonding WITH (nolock)
                                        WHERE (Del_FG = 0) ";

                cmd.Parameters.Add("@TypeCd", SqlDbType.NVarChar).Value = typeCd;
                cmd.Parameters.Add("@ProcNo", SqlDbType.Int).Value = procNo;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    int ordDbDispenseCt = rd.GetOrdinal("DbDispense_CT");

                    while (rd.Read())
                    {
                        retv = new Bonding();
                        retv.ChipBondCt = Convert.ToInt32(rd["ChipBond_CT"]);
                        if (rd.IsDBNull(ordDbDispenseCt) == false)
                        {
                            retv.DbDispenseCt = rd.GetInt32(ordDbDispenseCt);
                        }
                    }
                }
            }

            return retv;
        }
    }
}
