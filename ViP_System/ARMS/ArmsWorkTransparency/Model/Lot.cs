using ArmsApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsWorkTransparency.Model
{
    public class Lot
    {
        public string TypeCd { get; set; }

        public string LotNo { get; set; }

        public decimal EicsLogCt { get; set; }

        public Lot(string typeCd, string lotNo)
        {
            this.TypeCd = typeCd;
            this.LotNo = lotNo;
        }

        public static decimal? GetUseCount(int macNo, int procNo, string typeCd, string matGroupCd)
        {
            decimal? retv = null;

            using (SqlConnection con = new SqlConnection(Config.Settings.LAMSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macNo;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;
                cmd.Parameters.Add("@MATGROUPCD", SqlDbType.Char).Value = matGroupCd;

                string sql = @" SELECT lotusect FROM TnLotUseMaterial WITH(nolock) 
                                    WHERE macno = @MACNO AND procno = @PROCNO AND typecd = @TYPECD AND matgroupcd = @MATGROUPCD ";

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv = Convert.ToDecimal(rd["lotusect"]);
                    }
                }
            }

            return retv;
        }

        public static void UpdateUseCount(int macNo, int procNo, string typeCd, string matGroupCd, decimal useCt)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.LAMSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@MACNO", SqlDbType.BigInt).Value = macNo;
                cmd.Parameters.Add("@PROCNO", SqlDbType.BigInt).Value = procNo;
                cmd.Parameters.Add("@TYPECD", SqlDbType.NVarChar).Value = typeCd;
                cmd.Parameters.Add("@MATGROUPCD", SqlDbType.Char).Value = matGroupCd;

                cmd.Parameters.Add("@LOTUSECT", SqlDbType.Decimal).Value = useCt;

                string sql = @" SELECT lastupddt FROM TnLotUseMaterial
                                    WHERE macno = @MACNO AND procno = @PROCNO AND typecd = @TYPECD AND matgroupcd = @MATGROUPCD ";

                cmd.CommandText = sql;
                object lastUpdDt = cmd.ExecuteScalar();
                if (lastUpdDt == null)
                {
                    sql = @" INSERT INTO TnLotUseMaterial(macno, procno, typecd, matgroupcd, lotusect) VALUES(@MACNO, @PROCNO, @TYPECD, @MATGROUPCD, @LOTUSECT) ";
                }
                else
                {
                    sql = @" UPDATE TnLotUseMaterial SET lotusect = @LOTUSECT, lastupddt = getdate()
                                    WHERE macno = @MACNO AND procno = @PROCNO AND typecd = @TYPECD AND matgroupcd = @MATGROUPCD ";

                }
                cmd.CommandText = sql;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
