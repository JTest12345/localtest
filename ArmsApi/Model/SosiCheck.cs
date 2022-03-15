using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace ArmsApi.Model
{
    class SosiCheck
    {
        /// <summary>
        /// 設備に割付済みの素子情報を取得
        /// </summary>
        /// <param name="macno"></param>
        /// <param name="materialcd"></param>
        /// <param name="fromdt"></param>
        /// <param name="todt"></param>
        /// <param name="newfg"></param>
        /// <returns></returns>
        public static Material[] GetMatCejKshLst(int? macno, string materialcd, DateTime? fromdt, DateTime? todt, bool newfg)
        {
            DateTime compDate = DateTime.Now;
            if (todt.HasValue)
            {
                compDate = todt.Value;
            }

            List<Material> retv = new List<Material>();

            try
            {
                using (SqlConnection con = new SqlConnection(SQLite.ConStr))
                using (SqlCommand cmd = con.CreateCommand())
                {
                    con.Open();

                    cmd.CommandText = @"
                        SELECT mm.stockerno, mm.materialcd, mm.lotno, mm.startdt, mm.enddt, mm.issampled, mm.ringid, mt.blendcd
                        FROM TnMacMat mm WITH(nolock) INNER JOIN TnMaterials mt WITH(nolock) ON mm.materialcd = mt.materialcd AND mm.lotno = mt.lotno
                        WHERE delfg = 0 AND mm.macno = @MACNO";

                    cmd.Parameters.Add("@MACNO", System.Data.SqlDbType.BigInt).Value = macno;

                    if (!string.IsNullOrEmpty(materialcd))
                    {
                        cmd.CommandText += " AND (mm.materialcd = @MATERIALCD)";
                        cmd.Parameters.Add("@MATERIALCD", SqlDbType.NVarChar).Value = materialcd;
                    }

                    if (fromdt.HasValue)
                    {
                        cmd.CommandText += " AND (mm.enddt is null OR enddt >= @STARTDT)";
                        cmd.Parameters.Add("@STARTDT", SqlDbType.DateTime).Value = fromdt;
                    }

                    if (todt.HasValue)
                    {
                        cmd.CommandText += " AND mm.startdt <= @ENDDT";
                        cmd.Parameters.Add("@ENDDT", SqlDbType.DateTime).Value = compDate;
                    }

                    if (newfg)
                    {
                        cmd.CommandText += " AND mm.enddt is null ";
                    }

                    cmd.CommandText += " ORDER BY mm.materialcd, mm.startdt, mm.stockerno, mm.enddt, mm.lotno";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            Material m = new Material();
                            m.MaterialCd = SQLite.ParseString(rd["materialcd"]);
                            m.LotNo = SQLite.ParseString(rd["lotno"]);
                            m.InputDt = SQLite.ParseDate(rd["startdt"]) ?? DateTime.MinValue;
                            m.RemoveDt = SQLite.ParseDate(rd["enddt"]);
                            m.StockerNo = SQLite.ParseInt(rd["stockerno"]);
                            m.IsTimeSampled = SQLite.ParseBool(rd["issampled"]);
                            m.RingId = SQLite.ParseString(rd["ringid"]);
                            m.BlendCd = SQLite.ParseString(rd["blendcd"]).ToUpper();

                            retv.Add(m);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("投入済み原材料取得エラー", ex);
            }
            return retv.ToArray();
        }
        public class HchRnkCejKsh
        {
            public string CejHchRnk { get; set; }
            public string CejKshCd { get; set; }
        }
        /// <summary>
        /// CEJ機種CDリストから波長ランクリスト取得
        /// </summary>
        /// <param name="cejkshcd"></param>
        /// <returns></returns>
        public static HchRnkCejKsh[] GetHchRnkCejKshList(List<string> cejkshcd)
        {
            List<HchRnkCejKsh> retv = new List<HchRnkCejKsh>();

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.HCHRankConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {

                    con.Open();

                    cmd.CommandText = $@"
                        SELECT CEJ_HCH_RNK
                             , CEJ_KSHCD
                          FROM CEJ_KSH_HCH_MST WITH(nolock)
                         WHERE CEJ_KSHCD IN ('{ string.Join("','", cejkshcd) }')
                         ORDER BY CEJ_HCH_RNK
                          ";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            HchRnkCejKsh h = new HchRnkCejKsh();

                            h.CejHchRnk = SQLite.ParseString(rd["CEJ_HCH_RNK"]).ToUpper();
                            h.CejKshCd = SQLite.ParseString(rd["CEJ_KSHCD"]).ToUpper();

                            retv.Add(h);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("波長ランクCEJ機種取得エラー", ex);
            }
            return retv.ToArray();
        }

        /// <summary>
        /// 製品、素子のコードから波長ランクのリストを取得する
        /// </summary>
        /// <param name="shncd"></param>
        /// <param name="sosicd"></param>
        /// <returns></returns>
        public static string[] GetHchRnkLstFronShn(string shncd, string sosicd)
        {
            List<string> retv = new List<string>();

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.HCHRankConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {

                    con.Open();

                    cmd.CommandText = @"
                        SELECT CEJ_HCH_RNK
                          FROM SHN_SOSI_HCH_MST WITH(NOLOCK)
                         WHERE 1 = 1
                        ";

                    if (!string.IsNullOrEmpty(shncd))
                    {
                        cmd.CommandText += " AND SHNCD = @SHNCD";
                        cmd.Parameters.Add("@SHNCD", SqlDbType.NVarChar).Value = shncd;
                    }
                    if (!string.IsNullOrEmpty(sosicd))
                    {
                        cmd.CommandText += " AND SOSICD = @SOSICD";
                        cmd.Parameters.Add("@SOSICD", SqlDbType.NVarChar).Value = sosicd;
                    }
                     cmd.CommandText += " ORDER BY CEJ_HCH_RNK";

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        while (rd.Read())
                        {
                            retv.Add(SQLite.ParseString(rd["CEJ_HCH_RNK"]).ToUpper());
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("波長ランク取得エラー", ex);
            }
            return retv.ToArray();
        }

        /// <summary>
        /// CEJ機種CDから波長ランク取得
        /// </summary>
        /// <param name="cejkscd"></param>
        /// <returns></returns>
        public static string GetHchRnkFromCejkscd(string cejkscd)
        {
            string retv = "";

            try
            {
                using (SqlConnection con = new SqlConnection(Config.Settings.HCHRankConSTR))
                using (SqlCommand cmd = con.CreateCommand())
                {

                    con.Open();

                    cmd.CommandText = @"
                        SELECT CEJ_HCH_RNK
                          FROM CEJ_KSH_HCH_MST WITH(NOLOCK)
                         WHERE CEJ_KSHCD = @CEJ_KSHCD";

                    cmd.Parameters.Add("@CEJ_KSHCD", SqlDbType.NVarChar).Value = cejkscd;

                    using (SqlDataReader rd = cmd.ExecuteReader())
                    {
                        if (rd.Read())
                        {
                            retv=SQLite.ParseString(rd["CEJ_HCH_RNK"]).ToUpper();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new ArmsException("波長ランク取得エラー", ex);
            }
            return retv;
        }
    }
}
