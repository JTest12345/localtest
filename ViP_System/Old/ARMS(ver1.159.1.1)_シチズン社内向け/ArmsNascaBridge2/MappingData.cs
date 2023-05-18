using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class MappingData
    {
        public string CarrierNo { get; set; }
        public string LotNo { get; set; }
        public string ResultValue { get; set; }

        public static void Import()
        {
            try
			{
                //マッピングデータの取り込み
                List<string> files = GetFiles(Config.Settings.MappingDirectoryPath, "mpd", 0);
                foreach (string file in files)
                {
                    List<MappingData> mappingDataList = getMappingData(file);
                    update(mappingDataList, true);

                    BackupFiles(Config.Settings.MappingDirectoryPath, file);
                }

                //TODO 個片DMの取り込み 2016.05.16

            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] MappingData Error:" + err.ToString());
            }
        }

        private static List<MappingData> getMappingData(string file)
        {
            List<MappingData> retv = new List<MappingData>();
            
            MappingData m = new MappingData();

            m.CarrierNo = Path.GetFileNameWithoutExtension(file);
            m.LotNo = ArmsApi.Model.LotCarrier.GetData(m.CarrierNo, true).LotNo;
            if (!string.IsNullOrWhiteSpace(m.LotNo))
            {
                m.ResultValue = File.ReadAllText(file, Encoding.Default);

                retv.Add(m);
            }

            return retv;
        }

        public static List<string> GetFiles(string path, string searchPattern, int retryCt)
        {
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                return new List<string>();
            }

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string[] pathArray = Directory.GetFiles(path, "*.*");

            Regex regex = new Regex(searchPattern);

            List<string> retv = pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();

            if (retv.Count == 0)
            {
                Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return GetFiles(path, searchPattern, retryCt);
            }

            return retv;
        }

        public static void BackupFiles(string path, string file)
        {
            string backupDir = Path.Combine(path
                , DateTime.Now.Year.ToString(), DateTime.Now.Month.ToString(), DateTime.Now.Day.ToString());
            if (!Directory.Exists(backupDir))
            {
                Directory.CreateDirectory(backupDir);
            }
            string destPath = Path.Combine(backupDir, Path.GetFileName(file));
            if (File.Exists(destPath) == true)
            {
                File.Delete(destPath);
            }
            File.Move(file, destPath);
        }

        private static void update(List<MappingData> mappingDataList, bool outputFg)
        {
            if (mappingDataList.Count == 0) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.LENSConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    SqlParameter pHistory = cmd.Parameters.Add("@HistoryID", SqlDbType.Int);
                    SqlParameter pLot = cmd.Parameters.Add("@LotNO", SqlDbType.NVarChar);
                    SqlParameter pCarrier = cmd.Parameters.Add("@CarrierNO", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@ProcNO", SqlDbType.BigInt).Value = -999;
                    SqlParameter pResultValue = cmd.Parameters.Add("@ResultValue", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@UpdUserCD", SqlDbType.Char).Value = "660";
                    SqlParameter pLastupdDT = cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime);
                    cmd.Parameters.Add("@OutputFG", SqlDbType.Int).Value = SQLite.SerializeBool(outputFg);
                    cmd.Parameters.Add("@NewFG", SqlDbType.Bit).Value = true;

                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    foreach (MappingData mappingData in mappingDataList)
                    {
                        pLot.Value = mappingData.LotNo;
                        pCarrier.Value = mappingData.CarrierNo;
                        pResultValue.Value = mappingData.ResultValue;

                        cmd.CommandText = @"
                            SELECT HistoryID FROM TnMapResult
                            WHERE LotNO = @LotNO 
                                AND ProcNO = @ProcNO 
                                AND CarrierNO = @CarrierNO 
                                AND OutputFG = @OutputFG
                                AND NewFG = 1 ";

                        object historyID = cmd.ExecuteScalar();

                        pLastupdDT.Value = System.DateTime.Now;

                        if (historyID == null)
                        {
                            pHistory.Value = 1;
                        }
                        else
                        {
                            //前履歴のNewFGを0に
                            cmd.CommandText = @" 
                            UPDATE 
                                TnMapResult 
                            SET NewFG = 0
							WHERE LotNO = @LotNO 
                                AND ProcNO = @ProcNO 
                                AND CarrierNO = @CarrierNO 
                                AND OutputFG = @OutputFG
                                AND NewFG = 1 ";

                            cmd.ExecuteNonQuery();

                            pHistory.Value = Convert.ToInt32(historyID) + 1;
                        }

                        cmd.CommandText = @"
                            INSERT
                             INTO TnMapResult(HistoryID
                                , LotNO
	                            , CarrierNO
	                            , ProcNO
	                            , ResultValue
                                , OutputFG
                                , UpdUserCD
                                , LastupdDT
                                , NewFG)
                            values(@HistoryID
                                , @LotNO
	                            , @CarrierNO
	                            , @ProcNO
	                            , @ResultValue
                                , @OutputFG
                                , @UpdUserCD
                                , @LastupdDT
                                , @NewFG)";
                        cmd.ExecuteNonQuery();
                    }
                    cmd.Transaction.Commit();
                }
                catch (Exception ex)
                {
                    cmd.Transaction.Rollback();
                    throw ex;
                }
            }
        }
    }
}
