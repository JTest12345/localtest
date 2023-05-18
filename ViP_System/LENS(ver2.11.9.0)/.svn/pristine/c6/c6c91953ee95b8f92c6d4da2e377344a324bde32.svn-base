using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2_Api
{
	public class WorkFlow
	{
		public string TypeCd { get; set; }
		public int WorkOrder { get; set; }
		public int ProcNo { get; set; }

		//public static bool IsReverseDeploy(int procno)
		//{
		//	List<int> retv = GetReverseDeployProcess(procno);
		//	if (retv.Count == 0)
		//	{
		//		return false;
		//	}
		//	else
		//	{
		//		return true;
		//	}
		//}

//		public static List<int> GetReverseDeployProcess(int? procno)
//		{
//			List<int> retv = new List<int>();

//			using (SqlConnection con = new SqlConnection(Config.Settings.LensConnectionString))
//			using (SqlCommand cmd = con.CreateCommand())
//			{
//				con.Open();

//				string sql = @" SELECT ProcNO, DelFG, LastupdDT 
//								FROM TmRevProc WITH (nolock) ";

//				if (procno.HasValue)
//				{
//					sql += " WHERE (ProcNO = @ProcNO) ";
//					cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = procno.Value;
//				}

//				cmd.CommandText = sql;

//				using (SqlDataReader rd = cmd.ExecuteReader())
//				{
//					while (rd.Read())
//					{
//						int procNo = Convert.ToInt32(rd["ProcNO"]);

//						retv.Add(procNo);
//					}
//				}
//			}

//			return retv;
//		}
	}
}
