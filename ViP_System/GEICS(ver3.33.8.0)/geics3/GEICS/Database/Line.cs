using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace GEICS.Database
{
	class Line
	{
		public static List<TmLINEInfo> GetTmLINEInfo(bool packageFG, int lineCD)
		{
			List<TmLINEInfo> wListTmLINEInfo = new List<TmLINEInfo>();

			string sqlCmdTxt = @"SELECT Inline_CD,Inline_NM,Plant_NM,LineCate_NM, NotUseTmQDIW_FG
                                FROM TmLINE
                                Where Del_FG=0";

			using (IConnection connect = NascaConnection.CreateInstance(Constant.StrQCIL, false))
			{
				if (packageFG)
				{
					sqlCmdTxt += " AND (Inline_CD = @Line_CD) ";
					SqlParameter param = new SqlParameter("@Line_CD", SqlDbType.Int);
					param.Value = lineCD;
					connect.Command.Parameters.Add(param);
				}

				SqlDataReader rd = null;
				try
				{
					connect.Command.CommandText = sqlCmdTxt;
					rd = connect.Command.ExecuteReader();

					int ordNotUseTmQDIW = rd.GetOrdinal("NotUseTmQDIW_FG");

					while (rd.Read())
					{
						TmLINEInfo wTmLINEInfo = new TmLINEInfo();
						wTmLINEInfo.Inline_CD = Convert.ToInt32(rd["Inline_CD"]);
						wTmLINEInfo.Inline_NM = Convert.ToString(rd["Inline_NM"]).Trim();
						wTmLINEInfo.Plant_NM = Convert.ToString(rd["Plant_NM"]).Trim();
						wTmLINEInfo.LineCate_NM = Convert.ToString(rd["LineCate_NM"]).Trim();
						wTmLINEInfo.NotUseTmQDIW = rd.GetBoolean(ordNotUseTmQDIW);
						wListTmLINEInfo.Add(wTmLINEInfo);
					}
				}
				finally
				{
					if (rd != null) rd.Close();
					connect.Close();
				}
			}
			return wListTmLINEInfo;
		}
	}
}
