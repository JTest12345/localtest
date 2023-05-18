using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace EICS.Arms
{
	class GnlMaster
	{
		public string Tid { get; set; }
		public string Code { get; set; }
		public string Val { get; set; }
		public string Val2 { get; set; }

		public static GnlMaster[] Search(string tid, string code, string val, string val2, int lineCD)
		{
			List<GnlMaster> retv = new List<GnlMaster>();

			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();
				cmd.CommandText = @"SELECT tid, code, val, val2 FROM TmGeneral
                    WHERE delfg=0";

				if (tid != null)
				{
					cmd.CommandText += " AND tid=@TID";
					cmd.Parameters.Add("@TID", SqlDbType.NVarChar).Value = tid;
				}

				if (code != null)
				{
					cmd.CommandText += " AND code=@CODE";
					cmd.Parameters.Add("@CODE", SqlDbType.NVarChar).Value = code;
				}

				if (val != null)
				{
					cmd.CommandText += " AND val=@VAL";
					cmd.Parameters.Add("@VAL", SqlDbType.NVarChar).Value = val;
				}

				if (val2 != null)
				{
					cmd.CommandText += " AND val2=@VAL2";
					cmd.Parameters.Add("@VAL2", SqlDbType.NVarChar).Value = val2;
				}

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
						GnlMaster gnl = new GnlMaster();
						gnl.Tid = rd["tid"].ToString();
						gnl.Code = rd["code"].ToString();
						gnl.Val = rd["val"].ToString();
						gnl.Val2 = rd["val2"].ToString();
						retv.Add(gnl);
					}
				}
			}

			return retv.ToArray();
		}
	}
}
