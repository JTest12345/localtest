using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.LAMS
{
    public class HeartBeat
    {
        public DateTime Date { get; set; }

        public int Available { get; set; }

        public int Working { get; set; }

        private static List<HeartBeat> GetMachineData(string plantCd, DateTime fromDt, DateTime toDt)
        {
            List<HeartBeat> retv = new List<HeartBeat>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LAMSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                string sql = @" SELECT date, available, working
                         FROM TnHeartBeat WITH(nolock)
                         WHERE (plantcd = @PlantCd) AND (date >= @FromDt) AND (date < @ToDt)";

                cmd.Parameters.Add("@PlantCd", SqlDbType.NVarChar).Value = plantCd;
                cmd.Parameters.Add("@FromDt", SqlDbType.DateTime).Value = fromDt;
                cmd.Parameters.Add("@ToDt", SqlDbType.DateTime).Value = toDt;

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        HeartBeat h = new HeartBeat();

                        h.Date = Convert.ToDateTime(rd["date"]);
                        h.Available = Convert.ToInt32(rd["available"]);
                        h.Working = Convert.ToInt32(rd["working"]);

                        retv.Add(h);
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// 指定設備の停止時間を取得
        /// </summary>
        /// <param name="plantCd">設備番号</param>
        /// <param name="fromDt">停止対象範囲開始</param>
        /// <param name="toDt">停止対象範囲終了</param>
        /// <returns></returns>
        public static double GetMachineStopHour(string plantCd, DateTime fromDt, DateTime toDt)
        {
            double retv = 0;

            DateTime? beforeRecoredDt = null;
            int? beforeAvailable = null;

            List<HeartBeat> dataList = GetMachineData(plantCd, fromDt, toDt);
            foreach(HeartBeat data in dataList)
            {
                if (beforeAvailable.HasValue == true)
                {
                    if (beforeAvailable.Value == 1 && data.Available == 1)
                    {
                        beforeAvailable = data.Available;
                        beforeRecoredDt = data.Date;

                        continue;
                    }

                    TimeSpan span = data.Date - beforeRecoredDt.Value;
                    retv += span.TotalHours;
                }

                beforeAvailable = data.Available;
                beforeRecoredDt = data.Date;
            }

            return retv;
        }

    }
}
