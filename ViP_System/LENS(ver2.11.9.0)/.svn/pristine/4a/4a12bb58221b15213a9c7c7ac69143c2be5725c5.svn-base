using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
	public class MapConv
	{
		public static Dictionary<string, string> GetData(string originalcd, string convertcd)
		{
			Dictionary<string, string> retv = new Dictionary<string, string>();
			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT OriginalValue, ConvertValue 
					 FROM TmMapConv WITH (nolock) 
					 WHERE (DelFG = 0) AND (OriginalCD = @OriginalCD) AND (ConvertCD = @ConvertCD) ";

				cmd.Parameters.Add("@OriginalCD", SqlDbType.NVarChar).Value = originalcd;
				cmd.Parameters.Add("@ConvertCD", SqlDbType.NVarChar).Value = convertcd;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						retv.Add(rd["OriginalValue"].ToString().Trim(), rd["ConvertValue"].ToString().Trim());
					}
				}
			}
			return retv;
		}
	}
}
