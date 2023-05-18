using ArmsApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class SubstrateThicknessRankData
    {
        public string CarrierNo { get; set; }
        public string LotNo { get; set; }
        public string SubstrateThicknessRank { get; set; }

        public static void Import()
        {
            try
            {
                //TODO ファイルの拡張子確認 2016.05.19
                List<string> files = MappingData.GetFiles(Config.Settings.MappingDirectoryPath, "mpd", 0);

                foreach (string file in files)
                {
                    List<SubstrateThicknessRankData> substrateThicknessRankDataList = getData(file);
                    update(substrateThicknessRankDataList, true);

                    //MappingData.BackupFiles(Config.Settings.SubstrateThicknessRankDirectoryPath, file);
                }
            }
            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] SubstrateThicknessRankData Error:" + err.ToString());
            }
        }

        private static List<SubstrateThicknessRankData> getData(string file)
        {
            List<SubstrateThicknessRankData> retv = new List<SubstrateThicknessRankData>();
            
            SubstrateThicknessRankData m = new SubstrateThicknessRankData();

            m.CarrierNo = Path.GetFileNameWithoutExtension(file);
            //m.LotNo = ArmsApi.Model.LotCarrier.GetData(m.CarrierNo, true).LotNo;
            if (!string.IsNullOrWhiteSpace(m.CarrierNo))
            {
                m.SubstrateThicknessRank = m.CarrierNo.Substring(12, 1);

                retv.Add(m);
            }

            return retv;
        }

        private static void update(List<SubstrateThicknessRankData> substrateThicknessRankDataList, bool outputFg)
        {
            if (substrateThicknessRankDataList.Count == 0) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                try
                {
                    SqlParameter pCarrier = cmd.Parameters.Add("@CarrierNO", SqlDbType.NVarChar);
                    SqlParameter pThicknessRank = cmd.Parameters.Add("@ThicknessRank", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@UpdUserCD", SqlDbType.Char).Value = "660";
                    SqlParameter pLastupdDT = cmd.Parameters.Add("@LastupdDT", SqlDbType.DateTime);

                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    foreach (SubstrateThicknessRankData substrateThicknessRankData in substrateThicknessRankDataList)
                    {
                        pCarrier.Value = substrateThicknessRankData.CarrierNo;
                        pThicknessRank.Value = string.Empty;
                        pLastupdDT.Value = System.DateTime.Now;

                        cmd.CommandText = @"
                        SELECT lastupddt FROM TnSubstrateThicknessRank 
                        WHERE datamatrix = @CarrierNO";

                        object lastupddt = cmd.ExecuteScalar();
                        if (lastupddt == null)
                        {
                            pThicknessRank.Value = substrateThicknessRankData.SubstrateThicknessRank;
                            pLastupdDT.Value = System.DateTime.Now;

                            cmd.CommandText = @"
                            INSERT
                             INTO TnSubstrateThicknessRank(datamatrix
                                , thicknessrank
                                , UpdUserCD
                                , LastupdDT)
                            values(@CarrierNO
                                , @ThicknessRank
                                , @UpdUserCD
                                , @LastupdDT)";
                            cmd.ExecuteNonQuery();
                        }
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
