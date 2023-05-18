using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;

namespace GEICS.Database
{
	class Qtim
	{
		public int TimingNo { get; set; }
		public string TimingNm { get; set; }

		public static Dictionary<string, int> GetQtimList()
		{
			Dictionary<string, int> qtimList = new Dictionary<string, int>();
			//閾値マスタにあるInline_CDのみ表示
			//string sqlCmdTxt = "Select DISTINCT Timing_NM From TmQtim WITH(NOLOCK) ORDER BY Timing_NM  ASC";
			string sqlCmdTxt = "Select DISTINCT Timing_NO, Timing_NM From TmQtim WITH(NOLOCK)";
			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				try
				{
					connect.Command.CommandText = sqlCmdTxt;

					using (SqlDataReader rd = connect.Command.ExecuteReader())
					{
						int ordNo = rd.GetOrdinal("Timing_NO");
						int ordNm = rd.GetOrdinal("Timing_NM");

						while (rd.Read())
						{
							int qtimNo = rd.GetInt32(ordNo);
							string qtimNm = rd.GetString(ordNm);

							if (ContainsKeyAndVal(qtimList, qtimNm, qtimNo) == false)
							{
								qtimList.Add(qtimNm, qtimNo);
							}
						}
					}
				}
				finally
				{
					connect.Close();
				}
			}

			return qtimList;
		}

		private static bool ContainsKeyAndVal(Dictionary<string, int> qtimList, string key, int val)
		{

			if (qtimList != null && qtimList.Count > 0 && qtimList.Where(q => q.Key == key && q.Value == val).ToDictionary(q => q.Key).Count > 0)
			{
				return true;
			}
			
			return false;
		}
	}
}
