using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database.NASCA
{
    public class Lot
    {
        public static string GetTypeCd(string lotNo, int hostLineCd)
        {
            using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.NASCA, hostLineCd)))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @"SELECT TOP 1 RvmMCONV.mtralbase_cd
                                FROM NttSSHJ WITH(NOLOCK)
                                INNER JOIN RvtORDH WITH(NOLOCK) ON NttSSHJ.MnfctInst_NO = RvtORDH.mnfctinst_no
                                INNER JOIN RvmMCONV WITH(NOLOCK) ON RvtORDH.material_cd = RvmMCONV.material_cd
                                WHERE NttSSHJ.Lot_NO = @LOTNO 
                                AND NttSSHJ.Del_FG = 0 AND RvtORDH.del_fg = '0' AND RvmMCONV.del_fg = '0'
                                OPTION(MAXDOP 1)";

                cmd.Parameters.Add("@LOTNO", System.Data.SqlDbType.VarChar).Value = lotNo;
                cmd.CommandText = sql;

                string typeCD = Convert.ToString(cmd.ExecuteScalar() ?? "");
                if (string.IsNullOrWhiteSpace(typeCD) == true)
                {
                    return null;
                }
                else
                {
                    return typeCD.Trim();
                }
            }

        }

    }
}
