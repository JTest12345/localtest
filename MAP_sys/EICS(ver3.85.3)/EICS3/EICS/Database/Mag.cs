using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace EICS.Database
{
	class Mag
	{
		public string LotNO { get; set; }
		public string MagNO { get; set; }
		public long InlineProcNO { get; set; }
		public int NewFG { get; set; }
		public DateTime LastUpdDT { get; set; }

		public static bool IsThereData(string serverNO, string keyNO, bool isMagazineNO)
		{
			List<Mag> magList = new List<Mag>();
			Mag mag = new Mag();

#if Debug
			using (DBConnect conn =
				DBConnect.CreateInstance(string.Format(EICS.Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", serverNO).Replace("ARMS", "ARMS_SV"), "System.Data.SqlClient", false))
#else
			using (DBConnect conn =
				DBConnect.CreateInstance(string.Format(EICS.Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", serverNO), "System.Data.SqlClient", false))
#endif
			{
				string sql = @" Select lotno, magno From TnMag Where newfg = '1' ";

				if (isMagazineNO)
				{
					sql += @" And magno = @MagNO ";
					conn.SetParameter("@MagNO", System.Data.SqlDbType.NVarChar, keyNO);
				}
				else
				{
					sql += @" And lotno = @LotNO ";
					conn.SetParameter("@LotNO", System.Data.SqlDbType.NVarChar, keyNO);
				}

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int LotNO_Ord = rd.GetOrdinal("lotno");
					int MagNO_Ord = rd.GetOrdinal("magno");

					while (rd.Read())
					{
						mag.LotNO = rd.GetString(LotNO_Ord);
						mag.MagNO = rd.GetString(MagNO_Ord);

						magList.Add(mag);
					}

					if (magList.Count == 1)
					{
						return true;
					}
					else if (magList.Count == 0)
					{
						return false;
					}
					else
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_91, serverNO, mag.LotNO, mag.MagNO));
					}
				}
			}
		}

		public List<Mag> GetEndDataFromProcNO(string serverNO, int procNo)
		{
			List<Mag> magList = new List<Mag>();
			Mag mag = new Mag();

#if Debug
			using (DBConnect conn =
				DBConnect.CreateInstance(string.Format(EICS.Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", serverNO).Replace("ARMS", "ARMS_SV"), "System.Data.SqlClient", false))
#else
			using (DBConnect conn =
				DBConnect.CreateInstance(string.Format(EICS.Properties.Settings.Default.ConnectionString_ARMSDB, "inline", "R28uHta", serverNO), "System.Data.SqlClient", false))
#endif
			{
				string sql = @" Select lotno, magno From TnMag With(nolock) Where newfg = '1' and inlineprocno = @ProcNO ";

				conn.SetParameter("@ProcNO", System.Data.SqlDbType.Int, procNo);

				using (DbDataReader rd = conn.GetReader(sql))
				{
					int LotNO_Ord = rd.GetOrdinal("lotno");
					int MagNO_Ord = rd.GetOrdinal("magno");

					while (rd.Read())
					{
						mag.LotNO = rd.GetString(LotNO_Ord);
						mag.MagNO = rd.GetString(MagNO_Ord);

						magList.Add(mag);
					}

					if (magList.Count >= 0)
					{
						return magList;
					}
					else
					{
						throw new Exception(string.Format(Constant.MessageInfo.Message_91, serverNO, mag.LotNO, mag.MagNO));
					}
				}
			}
		}

	}
}

