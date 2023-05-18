using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    public class DBWaferLog
    {
        public enum LogKB
        {
            ロット完成,
            ウェハー割り付け,
            ウェハー取り外し,
            カセット交換,
            ウェハー段数変更_手動,
            ウェハー段数変更_自動,
            マガジンロード,
            マガジンアンロード,
            読み込みエラー,
        }

        public DateTime LogDT { get; set; }
        public string Message { get; set; }

        /// <summary>
        /// アッセンロット番号
        /// </summary>
        public string AsmLotNo { get; set; }

        /// <summary>
        /// ウェハー割り付け段数
        /// </summary>
        public int SheetNO { get; set; }

        /// <summary>
        /// ウェハーロット番号
        /// </summary>
        public Material Wafer { get; set; }

        public LogKB KB { get; set; }

        /// <summary>
        /// UL側マガジン段数
        /// </summary>
        public int MagRowNo { get; set; }

        public static DBWaferLog[] GetAllLogs(string plantcd, DateTime from, DateTime to)
        {
            DBWaferLog[] logs1 = GetMnfctRecord(plantcd, from, to);
            DBWaferLog[] logs2 = GetMaterialRecord(plantcd, from, to);
            DBWaferLog[] logs3 = GetQCILWaferLog(plantcd, from, to);

            DBWaferLog[] mergedArray = new DBWaferLog[logs1.Length + logs2.Length + logs3.Length];
            logs1.CopyTo(mergedArray, 0);
            logs2.CopyTo(mergedArray, logs1.Length);
            logs3.CopyTo(mergedArray, logs1.Length + logs2.Length);

            return mergedArray;
        }

        #region GetMnfctRecord

        private static DBWaferLog[] GetMnfctRecord(string plantcd, DateTime from, DateTime to)
        {
            MachineInfo mac = MachineInfo.GetMachine(plantcd);
            Order[] orders = Order.SearchOrder(null, null, null, mac.MacNo, false, false, null, null, from, to);

            List<DBWaferLog> retv = new List<DBWaferLog>();
            foreach (Order o in orders)
            {
                DBWaferLog log = new DBWaferLog();
                log.KB = DBWaferLog.LogKB.ロット完成;
                log.LogDT = o.WorkEndDt.Value;
                log.Message = o.LotNo;
                log.AsmLotNo = o.LotNo;

                retv.Add(log);
            }

            return retv.ToArray();
        }

        #endregion

        #region GetMaterialRecord

        private static DBWaferLog[] GetMaterialRecord(string plantcd, DateTime from, DateTime to)
        {
            MachineInfo mac = MachineInfo.GetMachine(plantcd);
            Material[] matlist = mac.GetMaterials(from, to);

            List<DBWaferLog> retv = new List<DBWaferLog>();

            foreach (Material mat in matlist)
            {
                if (!mat.IsWafer) continue;

                DBWaferLog log = new DBWaferLog();
                log.KB = DBWaferLog.LogKB.ウェハー割り付け;
                log.LogDT = mat.InputDt ?? DateTime.MinValue;
                log.SheetNO = mat.StockerNo;
                log.Message = mat.MaterialCd + " " + mat.LotNo;

                retv.Add(log);

                if (mat.RemoveDt.HasValue)
                {
                    DBWaferLog log2 = new DBWaferLog();
                    log2.KB = DBWaferLog.LogKB.ウェハー取り外し;
                    log2.LogDT = mat.RemoveDt.Value;
                    log2.SheetNO = mat.StockerNo;
                    log2.Message = mat.MaterialCd + " " + mat.LotNo;

                    retv.Add(log2);
                }

            }

            return retv.ToArray();
        }

        #endregion

        #region InsertQCILWaferLog

        public static void InsertQCILWaferLog(string plantcd, DateTime dt, string log)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.Parameters.Add("@PLANTCD", System.Data.SqlDbType.NVarChar).Value = plantcd;
                cmd.Parameters.Add("@DT", System.Data.SqlDbType.DateTime).Value = dt;

                cmd.CommandText = "SELECT plant_cd FROM TnDBWF WHERE plant_cd=@PLANTCD AND Log_DT = @DT";

                //記録済みのログは保存しない
                if (cmd.ExecuteScalar() != null)
                {
                    return;
                }

                cmd.Parameters.Add("@MSG", System.Data.SqlDbType.NVarChar).Value = log;
                cmd.CommandText = "INSERT TnDBWF(plant_cd, log_dt, message) VALUES(@PLANTCD, @DT, @MSG)";
                cmd.ExecuteNonQuery();
            }
        }
        #endregion

        #region GetQCILWaferLog
        public static DBWaferLog[] GetQCILWaferLog(string plantcd, DateTime from, DateTime to)
        {
            List<DBWaferLog> retv = new List<DBWaferLog>();
            MachineInfo mac = MachineInfo.GetMachine(plantcd);
            if (mac == null) return retv.ToArray();

            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //GG除く（内製チップのみ）
                cmd.CommandText = @"
                    SELECT Log_DT, Message FROM TnDBWF
                    where plant_cd = @PLANTCD
                    and Log_DT between @DATEFROM and @DATETO";

                cmd.Parameters.Add("@PLANTCD", System.Data.SqlDbType.Char).Value = mac.MacNo;
                cmd.Parameters.Add("@DATEFROM", System.Data.SqlDbType.DateTime).Value = from;
                cmd.Parameters.Add("@DATETO", System.Data.SqlDbType.DateTime).Value = to;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DBWaferLog log = parseQCILWaferLog(reader.GetDateTime(0), plantcd, reader.GetString(1));

                        retv.Add(log);
                    }
                }
            }


            using (SqlConnection con = new SqlConnection(Config.Settings.MainQCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //GG除く（内製チップのみ）
                cmd.CommandText = @"
                    SELECT Log_DT, Message FROM TnDBWF
                    where plant_cd = @PLANTCD
                    and Log_DT between @DATEFROM and @DATETO";

                cmd.Parameters.Add("@PLANTCD", System.Data.SqlDbType.Char).Value = mac.MacNo;
                cmd.Parameters.Add("@DATEFROM", System.Data.SqlDbType.DateTime).Value = from;
                cmd.Parameters.Add("@DATETO", System.Data.SqlDbType.DateTime).Value = to;

                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        DBWaferLog log = parseQCILWaferLog(reader.GetDateTime(0), plantcd, reader.GetString(1));

                        retv.Add(log);
                    }
                }
            }

            return retv.ToArray();
        }
        
        #endregion

        #region parseQCILWaferLog
        /// <summary>
        /// DB機のウェハーログを解析する
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="log"></param>
        /// <returns></returns>
        private static DBWaferLog parseQCILWaferLog(DateTime dt, string plantcd, string log)
        {
            DBWaferLog retv = new DBWaferLog();
            retv.LogDT = dt;

            string[] elements = log.Split(',');
            if (elements.Length == 0)
            {
                return retv;
            }

            switch (elements[0])
            {
                case "01":
                    retv.KB = DBWaferLog.LogKB.マガジンロード;
                    break;

                case "02":
                    retv.KB = DBWaferLog.LogKB.ウェハー段数変更_自動;
                    break;

                case "03":
                    retv.KB = DBWaferLog.LogKB.カセット交換;
                    break;

                case "04":
                    retv.KB = DBWaferLog.LogKB.ウェハー段数変更_手動;
                    break;

                case "10":
                    retv.KB = DBWaferLog.LogKB.マガジンアンロード;
                    break;

                default:
                    retv.KB = DBWaferLog.LogKB.読み込みエラー;
                    break;
            }

            if (elements.Length >= 3)
            {
                retv.SheetNO = int.Parse(elements[1]);

                int magThrowCt = int.Parse(elements[1]);

                Material wafer = GetWafer(plantcd, dt, magThrowCt);
                if (wafer != null)
                {
                    string waferlot = wafer.MaterialCd + " " + wafer.LotNo;
                    retv.Wafer = wafer;
                    retv.Message = waferlot + "(マガジン段数:" + elements[2] + ")";
                }
                retv.MagRowNo = int.Parse(elements[2]);

            }

            return retv;
        }
        #endregion

        private static Material GetWafer(string plantCd, DateTime changeDT, int throwNo)
        {
            MachineInfo mac = MachineInfo.GetMachine(plantCd);

            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    SELECT
                     TOP 1 macno
                    , stockerno
                    , m.materialcd
                    , m.lotno
                    FROM TnMacMat m
                    INNER JOIN TnMaterials mat
                    ON m.materialcd = mat.materialcd
                    AND m.lotno = mat.lotno
                    WHERE macno = @MACNO
                    AND stockerno = @STOCKERNO
                    and startdt <= @CHANGEDT
                    and mat.iswafer = 1
                    order by startdt desc";

                cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.Int).Value = mac.MacNo;
                cmd.Parameters.Add("@STOCKERNO", System.Data.SqlDbType.Int).Value = throwNo;
                cmd.Parameters.Add("@CHANGEDT", System.Data.SqlDbType.DateTime).Value = changeDT;

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        string materialcd = SQLite.ParseString(rd["materialcd"]);
                        string lotno = SQLite.ParseString(rd["lotno"]);

                        Material m = Material.GetMaterial(materialcd, lotno);
                        return m;
                    }
                }
            }

            return null;
        }


    }
}
