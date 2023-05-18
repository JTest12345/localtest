using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using SLCommonLib.DataBase;
using System.Data.Common;

namespace EICS.Database.LENS
{
    public class WorkResult
    {
        public string LotNo { get; set; }
        public int ProcNo { get; set; }
		public string PlantCD { get; set; }
        public DateTime StartDt { get; set; }
        public DateTime EndDt { get; set; }
        public bool IsCompleted { get; set; }

		private static WorkResult GetData(string lotNo, int procNo, int hostLineCd, string carrierNo, string plantCd)
        {
			List<WorkResult> resultList = GetDatas(lotNo, procNo, hostLineCd, carrierNo, plantCd);

            if (resultList.Count == 0)
            {
                return null;
                //throw new ApplicationException(string.Format("LENSの実績が存在しません。ロット番号:{0} 工程番号:{1}", lotNo, procNo));
            }
			else if (resultList.Count == 1)
			{
				return resultList.Single();
			}
			else
			{
				resultList = resultList.OrderByDescending(r => r.StartDt).ToList();

				return resultList[0];
			}
        }

		private static List<WorkResult> GetDatas(string lotNo, int procNo, int hostLineCd, string carrierNo, string plantCd)//, int? macno, bool isTargetOnlyComplete)
		{
            List<WorkResult> retv = new List<WorkResult>();

            using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.LENS, hostLineCd), "System.Data.SqlClient", false))
            {
				string sql = @" SELECT LotNO, ProcNO, PlantCD, StartDT, EndDT, DelFG, IsCompleted, LastupdDT, CarrierNo
								FROM TnTran WITH (nolock) 
								WHERE (DelFG = 0) ";

                if (!string.IsNullOrEmpty(lotNo))
                {
                    sql += " AND LotNO = @LotNO ";
                    conn.SetParameter("@LotNO", SqlDbType.NVarChar, lotNo);
                }

				if (!string.IsNullOrEmpty(carrierNo))
				{
					sql += " AND CarrierNo = @CarrierNo ";
					conn.SetParameter("@CarrierNo", SqlDbType.NVarChar, carrierNo);
				}

				if (!string.IsNullOrEmpty(plantCd))
				{
					sql += " AND PlantCD = @PlantCd ";
					conn.SetParameter("@PlantCd", SqlDbType.NVarChar, plantCd);
				}

                sql += " AND ProcNO = @ProcNO ";
                conn.SetParameter("@ProcNO", SqlDbType.BigInt, procNo);
  
                conn.Command.CommandText = sql;
                using (DbDataReader rd = conn.GetReader(sql))
                {
                    int ordEndDt = rd.GetOrdinal("EndDT");
                    while (rd.Read())
                    {
                        WorkResult r = new WorkResult();
                        r.LotNo = rd["LotNO"].ToString().Trim();
                        r.ProcNo = Convert.ToInt32(rd["ProcNO"]);
						r.PlantCD = Convert.ToString(rd["PlantCD"]);
                        r.StartDt = Convert.ToDateTime(rd["StartDT"]);
                        if (!rd.IsDBNull(ordEndDt))
                        {
                            r.EndDt = rd.GetDateTime(ordEndDt);
                        }
						r.IsCompleted = Convert.ToBoolean(rd["IsCompleted"]);
                        retv.Add(r);
                    }
                }
            }

            return retv;
        }

		public static bool IsComplete(string lotNo, int procNo, int hostLineCd)
		{
			return IsComplete(lotNo, procNo, hostLineCd, null, null);
		}

		public static bool IsComplete(string lotNo, int procNo, int hostLineCd, string carrierNo, string plantCd)
		{
			SettingInfo setting = SettingInfo.GetSingleton();
			if (!setting.IsMappingMode)
            {
                //マッピング機能無効は完了扱いとする。
                return true;
            }

			WorkResult result = GetData(lotNo, procNo, hostLineCd, carrierNo, plantCd);
			if (result == null)
            {
                return false;
            }
            else
            {
                return result.IsCompleted;
            }
        }
    }
}
