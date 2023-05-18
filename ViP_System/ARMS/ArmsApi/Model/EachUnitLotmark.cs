using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class EachUnitLotmark
    {
        public long XAddress { get; set; }
        public long YAddress { get; set; }
        public string CarrierNo { get; set; }
        public string EachUnitLotmarkData { get; set; }

        public static string GetEachUnitLotmark(long xaddress, long yaddress, string carrierNo)
        {
            List<string> datalist = new List<string>();

             using (SqlConnection con = new SqlConnection(SQLite.ConStr))
             using (SqlCommand cmd = con.CreateCommand())
             {
                 cmd.Parameters.Add("@CarrierNO", System.Data.SqlDbType.NVarChar).Value = carrierNo;
                 cmd.Parameters.Add("@XAddress", System.Data.SqlDbType.BigInt).Value = xaddress;
                 cmd.Parameters.Add("@YAddress", System.Data.SqlDbType.BigInt).Value = yaddress;
          
                 try
                 {
                     con.Open();

                     string sql = @" SELECT eachunitlotmarkdata
								FROM TnEachUnitLotmark WITH(nolock) 
								WHERE carrierno = @CarrierNO 
                                AND xaddress = @XAddress
                                AND yaddress = @YAddress";

                     cmd.CommandText = sql;
                     using (SqlDataReader rd = cmd.ExecuteReader())
                     {
                         while (rd.Read())
                         {
                             string eachunitlotmarkdata = rd["eachunitlotmarkdata"].ToString().Trim();
                             datalist.Add(eachunitlotmarkdata);
                         }
                     }
                 }
                 catch (Exception ex)
                 {
                     throw new ArmsException(string.Format("個片DMデータ取得時にエラー発生:キャリア番号:{0}", carrierNo), ex);
                 }
             }

             if (datalist.Count == 1)
             {
                 return datalist[0];
             }
             else if (datalist.Count == 0)
             {
                 throw new ApplicationException(string.Format("キャリア番号に個片DMが紐づいていません。キャリア番号：{0}", carrierNo));
             }
             else
             {
                 throw new ApplicationException(string.Format("キャリア番号に複数の個片DMが紐づいています。キャリア番号：{0}", carrierNo));
             }
        }
    }
}
