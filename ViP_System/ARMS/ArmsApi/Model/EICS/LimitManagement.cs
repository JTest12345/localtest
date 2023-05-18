using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.EICS
{
    public class LimitManagement
    {
        public static decimal? GetUpperLimit(string modelNm, int qcParamNo, string materialCd)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = $@" SELECT Parameter_MAX FROM TmPLM WITH(nolock) 
                                        WHERE Del_FG = 0 AND QcParam_NO = @QcParamNo AND Material_CD = @MaterialCd AND Model_NM = @ModelNm ";

                cmd.Parameters.Add("@QcParamNo", SqlDbType.Int).Value = qcParamNo;
                cmd.Parameters.Add("@MaterialCd", SqlDbType.NVarChar).Value = materialCd;
                cmd.Parameters.Add("@ModelNm", SqlDbType.NVarChar).Value = modelNm;

                object maxLimit = cmd.ExecuteScalar();
                if (maxLimit == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToDecimal(maxLimit);
                }
            }
        }
    }
}
