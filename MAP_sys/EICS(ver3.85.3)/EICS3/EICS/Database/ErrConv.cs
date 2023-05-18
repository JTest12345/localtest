using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
{
	class ErrConv
	{
		public string EquiErr { get; set; }
		public string ClassCD { get; set; }
		public string ItemCD { get; set; }
		public string CauseCD { get; set; }
		public bool NotOutputNasFG { get; set; }
        public long ProcNo { get; set; }

        public static List<ErrConv> GetData(string plantCD, int hostLineCd, long? procNo, bool withProcNo)
		{
			List<ErrConv> mapConvList = new List<ErrConv>();
			using (SqlConnection con = new SqlConnection(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, hostLineCd)))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();

				string sql = @" SELECT EquiErr_NO, NascaErr_NO, DefCause_CD, DefClass_CD, NotOutputNas_FG, ProcNo
					 FROM TmErrConv WITH (nolock) 
					 WHERE (Equipment_NO = @EquipmentNO) ";

				cmd.Parameters.Add("@EquipmentNO", SqlDbType.NVarChar).Value = plantCD;

				cmd.CommandText = sql;
				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					int ordEquiErr = rd.GetOrdinal("EquiErr_NO");
					int ordItemCD = rd.GetOrdinal("NascaErr_NO");
					int ordClassCD = rd.GetOrdinal("DefClass_CD");
					int ordCauseCD = rd.GetOrdinal("DefCause_CD");
					int ordNotOutFg = rd.GetOrdinal("NotOutputNas_FG");
                    int ordProcNo= rd.GetOrdinal("ProcNo");

                    while (rd.Read())
					{
						ErrConv mapConv = new ErrConv();

						mapConv.EquiErr = rd.GetString(ordEquiErr).Trim();
						mapConv.ItemCD = rd.GetString(ordItemCD).Trim();
						mapConv.ClassCD = rd.GetString(ordClassCD).Trim();
						mapConv.CauseCD = rd.GetString(ordCauseCD).Trim();
						mapConv.NotOutputNasFG = rd.GetBoolean(ordNotOutFg);
                        mapConv.ProcNo = rd.GetInt64(ordProcNo);

                        mapConvList.Add(mapConv);
					}
				}
			}

            if(procNo != null && withProcNo == true)
            {
                mapConvList = mapConvList.Where(m => m.ProcNo == procNo).ToList();
            }

			return mapConvList;
		}
	}
}
