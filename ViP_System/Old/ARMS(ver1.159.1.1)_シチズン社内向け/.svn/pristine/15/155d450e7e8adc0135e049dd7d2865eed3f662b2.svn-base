using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model.PSTESTER
{
    public class WorkResult
    {
        public string LotNo { get; set; }

        public int FunctionId { get; set; }

        public int ChipId { get; set; }

        public int PositionId { get; set; }

        public bool NgFg { get; set; }

        public static List<WorkResult> GetData(string lotNo, int functionId)
        {
            List<WorkResult> retv = new List<WorkResult>();

            using (SqlConnection con = new SqlConnection(Config.Settings.PSTesterConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.CommandText = @" SELECT T_LotNo.LotNo, T_Measurement.FunctionID, T_MeasurementItem.ChipID, T_MeasurementItem.PositionID, T_MeasurementItem.NG_FG
                                     FROM dbo.T_Measurement AS T_Measurement WITH (nolock) 
                                     INNER JOIN dbo.T_LotNo AS T_LotNo WITH (nolock) ON T_Measurement.LotNoID = T_LotNo.LotNoID 
                                     INNER JOIN dbo.T_MeasurementItem AS T_MeasurementItem WITH (nolock) ON T_Measurement.MeasurementID = T_MeasurementItem.MeasurementID
                                     WHERE T_LotNo.LotNo = @LOTNO
                                     AND T_Measurement.FunctionID = @FUNCTIONID ";

                //AND T_LotNo.NgFlag = 0
                //AND T_MeasurementItem.NG_FG = 0
                
                cmd.Parameters.Add("@LOTNO", SqlDbType.NVarChar).Value = lotNo;
                cmd.Parameters.Add("@FUNCTIONID", SqlDbType.Int).Value = functionId;
                
                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        WorkResult r = new WorkResult();

                        r.LotNo = rd["LotNo"].ToString().Trim();
                        r.FunctionId = SQLite.ParseInt(rd["FunctionID"]);
                        r.ChipId = SQLite.ParseInt(rd["ChipID"]);
                        r.PositionId = SQLite.ParseInt(rd["PositionID"]);
                        r.NgFg = SQLite.ParseBool(rd["NG_FG"]);

                        retv.Add(r);
                    }
                }
            }

            return retv;
        }

        /// <summary>
        /// 測定結果が合格か判断する
        /// </summary>
        /// <param name="lotNo"></param>
        /// <param name="functionId"></param>
        /// <returns></returns>
        public static bool IsResultPassing(string lotNo, int functionId)
        {
            List<WorkResult> list = GetData(lotNo, functionId);
            if (list.Count == 0)
            {
                return false;
            }

            if (list.Exists(l => l.NgFg == true))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
    }
}
