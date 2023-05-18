using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Arms
{
	class CutBlend
	{
		public string LotNO { get; set; }
		public Int64 MacNO { get; set; }
		public string BlendLotNO { get; set; }
		public DateTime? StartDT { get; set; }
		public DateTime? EndDT { get; set; }
		public string MagNO { get; set; }
		//public bool NascaEndFG { get; set; }
		//public bool NascaStartFG { get; set; }
		//public bool DelFG { get; set; }
		//public DateTime LastUpdDT { get; set; }

		public static List<CutBlend> GetData(int lineCD, string blendLotNo, string plantCd)
		{
			List<CutBlend> cutBlendList = new List<CutBlend>();
			try
			{
				using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.ARMS, lineCD), "System.Data.SqlClient", false))
				{
					string sql = @"SELECT TnCutBlend.lotno, TnCutBlend.macno, TnCutBlend.blendlotno, TnCutBlend.delfg, TmMachine.plantcd, TnCutBlend.magno,
									TnCutBlend.startdt, TnCutBlend.enddt
							FROM TnCutBlend WITH(INDEX(IX_BlendList)) INNER JOIN TmMachine ON TnCutBlend.macno = TmMachine.macno
							WHERE TnCutBlend.delfg = 0 and TmMachine.delfg = 0 and TnCutBlend.blendlotno = @BlendLot and TmMachine.plantcd = @PlantCD ";

					conn.SetParameter("@BlendLot", SqlDbType.NVarChar, blendLotNo);
					conn.SetParameter("@PlantCD", SqlDbType.NVarChar, plantCd);

					using (DbDataReader rd = conn.GetReader(sql))
					{
						int ordLotNo = rd.GetOrdinal("lotno");
						int ordMacNo = rd.GetOrdinal("macno");
						int ordBlendLot = rd.GetOrdinal("blendlotno");
						//int ordPlantCd = rd.GetOrdinal("plantcd");
						int ordStartDt = rd.GetOrdinal("startdt");
						int ordEndDt = rd.GetOrdinal("enddt");
						int ordMagNo = rd.GetOrdinal("magno");

						while (rd.Read())
						{
							CutBlend cutBlend = new CutBlend();

							cutBlend.LotNO = rd.GetString(ordLotNo).Trim();
							cutBlend.MacNO = rd.GetInt64(ordMacNo);
							cutBlend.BlendLotNO = rd.GetString(ordBlendLot).Trim();

							if (rd.IsDBNull(ordStartDt) == false)
							{
								cutBlend.StartDT = rd.GetDateTime(ordStartDt);
							}

							if (rd.IsDBNull(ordEndDt) == false)
							{
								cutBlend.EndDT = rd.GetDateTime(ordEndDt);
							}

							cutBlend.MagNO = rd.GetString(ordMagNo).Trim();

							cutBlendList.Add(cutBlend);
						}
					}

				}

				return cutBlendList;
			}
			catch (Exception)
			{
				throw;
			}
		}
	}
}
