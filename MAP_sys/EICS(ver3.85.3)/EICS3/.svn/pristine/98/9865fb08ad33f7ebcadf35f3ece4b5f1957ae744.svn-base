using SLCommonLib.DataBase;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Database
{
    public class PlcAddrInfo
    {
        public string ModelNM { get; set; }
        public string PrefixNM { get; set; }
        public string StartONADDR { get; set; }
        public string PrgNmADDR { get; set; }
        public int PrgNmLEN { get; set; }
        public string PrmOKADDR { get; set; }
        public string PrmNGADDR { get; set; }
        public bool DelFG { get; set; }
        public string UpdUserCD { get; set; }
        public DateTime LastUpdDT { get; set; }

        public static PlcAddrInfo GetData(int lineCd, string modelNm, string prefixNm)
        {
            try
            {
                List<PlcAddrInfo> PlcAddrList = new List<PlcAddrInfo>();

                using (DBConnect conn = DBConnect.CreateInstance(ConnectDB.getConnString(Constant.DBConnectGroup.EICSDB, lineCd), "System.Data.SqlClient", false))
                {
                    string sql = @" SELECT PA.Model_NM, PA.Prefix_NM, PA.StartON_ADDR, PA.PrgNm_ADDR, PA.PrgNm_LEN,
                                            PA.PrmOK_ADDR, PA.PrmNG_ADDR, PA.Del_FG, PA.UpdUser_CD, PA.LastUpd_DT
									FROM TmPlcADDRESS AS PA WITH(NOLOCK) 
									WHERE (PA.Model_NM = @ModelNM)
                                    AND (PA.Prefix_NM = @PrefixNM) 
									AND (PA.Del_FG = 0) ";

                    conn.SetParameter("@ModelNM", SqlDbType.VarChar, modelNm);
                    conn.SetParameter("@PrefixNM", SqlDbType.VarChar, prefixNm);

                    using (DbDataReader rd = conn.GetReader(sql))
                    {
                        int ordModelNM = rd.GetOrdinal("Model_NM");
                        int ordPrefixNM = rd.GetOrdinal("Prefix_NM");
                        int ordStartONADDR = rd.GetOrdinal("StartON_ADDR");
                        int ordPrgNmADDR = rd.GetOrdinal("PrgNm_ADDR");
                        int ordPrgNmLEN = rd.GetOrdinal("PrgNm_LEN");
                        int ordPrmOKADDR = rd.GetOrdinal("PrmOK_ADDR");
                        int ordPrmNGADDR = rd.GetOrdinal("PrmNG_ADDR");
                        int ordDelFG = rd.GetOrdinal("Del_FG");
                        int ordUpdUserCD = rd.GetOrdinal("UpdUser_CD");
                        int ordLastUpdDT = rd.GetOrdinal("LastUpd_DT");

                        while (rd.Read())
                        {
                            PlcAddrInfo PlcAddr = new PlcAddrInfo();
                            PlcAddr.ModelNM = rd.GetString(ordModelNM).Trim();
                            PlcAddr.PrefixNM = rd.GetString(ordPrefixNM).Trim();
                            PlcAddr.StartONADDR = rd.GetString(ordStartONADDR).Trim();
                            PlcAddr.PrgNmADDR = rd.GetString(ordPrgNmADDR).Trim();
                            PlcAddr.PrgNmLEN = rd.GetInt32(ordPrgNmLEN);
                            PlcAddr.PrmOKADDR = rd.GetString(ordPrmOKADDR).Trim();
                            PlcAddr.PrmNGADDR = rd.GetString(ordPrmNGADDR).Trim();
                            PlcAddr.DelFG = rd.GetBoolean(ordDelFG);
                            PlcAddr.UpdUserCD = rd.GetString(ordUpdUserCD).Trim();
                            PlcAddr.LastUpdDT = rd.GetDateTime(ordLastUpdDT);

                            PlcAddrList.Add(PlcAddr);
                        }
                    }
                }

                if (PlcAddrList.Count == 1)
                {
                    return PlcAddrList.Single();
                }
                else if (PlcAddrList.Count > 1)
                {
                    throw new ApplicationException(string.Format("マスタテーブルTmPlcAddressにキー重複があります。マスタ設定者へ確認してください。 Model_NM={0}, Prefix_NM={1} ", modelNm, prefixNm));
                }
                else
                {
                    return null;
                }
            }
            catch (Exception err)
            {
                throw;
            }
        }
    }
}
