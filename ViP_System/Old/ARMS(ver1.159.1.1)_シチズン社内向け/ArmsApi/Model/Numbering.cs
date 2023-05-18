using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsApi.Model
{
    class Numbering
    {
        public enum Category
        {
            AsmLot,
            CutLot,
            AsmLotHigh,
        }


        /// <summary>
        /// ライン番号 + 日付(yyyyMMdd) + 連番(3桁)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetNewAsmLotNo(DateTime dt, string lineNo)
        {
            string yy = dt.Year.ToString().Substring(2, 2);
            string m = dt.Month.ToString("X");

            int day = dt.Day;
            string d = (day < 10) ? day.ToString() : ((char)('A' + day - 10)).ToString();

            int uniqNo = GetNewUniqNumber(Category.AsmLot, int.Parse(dt.ToString("yyyyMMdd")), lineNo);

            int low = uniqNo % 10;
            int high = uniqNo / 10;
            string hstr = (high < 10) ? high.ToString() : ((char)('A' + high - 10)).ToString();

            string num = hstr + low.ToString();

            string lotno = Config.Settings.AsmLot1 + yy + m + d + string.Format(Config.Settings.AsmLot6789, lineNo) + num;
            lotno = lotno.Replace('I', 'i');
            return lotno;
        }

        /// <summary>
        /// ライン番号 + 日付(yyyyMMdd) + 連番(3桁)
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetNewAsmLotNoHigh(DateTime dt, string lineNo)
        {
            string yy = dt.Year.ToString().Substring(2, 2);
            string m = dt.Month.ToString("X");

            int day = dt.Day;
            string d = (day < 10) ? day.ToString() : ((char)('A' + day - 10)).ToString();

            int uniqNo = GetNewUniqNumber(Category.AsmLotHigh, int.Parse(dt.ToString("yyyyMMdd")), lineNo);

            string num = uniqNo.ToString("000");

            // 部署コード = 自動搬送用の設定項目「AsmLot6789」を流用 (1文字目 = 部署CD)
            string secStr = string.Format(Config.Settings.AsmLot6789, lineNo).Substring(0, 1);

            string lotno = Config.Settings.AsmLot1 + yy + m + d + secStr + num + "00";
            lotno = lotno.Replace('I', 'i');
            return lotno;
        }

        /// <summary>
        /// カットブレンド
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public static string GetNewCutBlendLotNo(DateTime dt, string lineno, bool isAutoLine)
        {
            string yy = dt.ToString().Substring(2, 2);
            string m = dt.Month.ToString("X");

            int day = dt.Day;
            string d = (day < 10) ? day.ToString() : ((char)('A' + day - 10)).ToString();

            int uniqNo = GetNewUniqNumber(Category.CutLot, int.Parse(dt.ToString("yyyyMMdd")), lineno);

            if (isAutoLine == true)
            {
                int low = uniqNo % 10;
                int high = uniqNo / 10;
                string hstr = (high < 10) ? high.ToString() : ((char)('A' + high - 10)).ToString();

                string num = hstr + low.ToString();
                string lotno = Config.Settings.CutBlend12 + yy + m + d + string.Format(Config.Settings.CutBlend789A, lineno) + num;
                lotno = lotno.Replace('I', 'i');
                return lotno;
            }
            else
            {
                int low = uniqNo % 100;
                int high = uniqNo / 100;
                string hstr = (high < 10) ? high.ToString() : ((char)('A' + high - 10)).ToString();
                string num = hstr + low.ToString("00");

                string lotno = Config.Settings.CutBlend12 + yy + m + d + Config.Settings.CutBlendOut789 + num;
                lotno = lotno.Replace('I', 'i');
                return lotno;
            }
        }


        /// <summary>
        /// 採番テーブルから新しい番号を取得し、
        /// 最終取得番号を更新
        /// </summary>
        /// <param name="ctg"></param>
        /// <param name="version"></param>
        /// <returns></returns>
        public static int GetNewUniqNumber(Category ctg, long version, string lineno)
        {
            using (SqlConnection con = new SqlConnection(SQLite.ConStr))
            using (SqlCommand cmd = con.CreateCommand())
            {

                try
                {
                    con.Open();
                    cmd.Transaction = con.BeginTransaction();

                    cmd.Parameters.Add("@CATEGORY", SqlDbType.NVarChar).Value = ctg.ToString();
                    cmd.Parameters.Add("@VERSION", SqlDbType.BigInt).Value = version;
                    cmd.Parameters.Add("@LASTUPDDT", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@LINENO", SqlDbType.NVarChar).Value = lineno;

                    cmd.CommandText = "SELECT lastno FROM TnNumbering WHERE category=@CATEGORY AND version=@VERSION AND [lineno]=@LINENO ";
                    object objlastno = cmd.ExecuteScalar();

                    if (objlastno == null)
                    {
                        //採番履歴が無い場合はArmsConfig(LotDefaultSerialNumber or CutLotDefaultSerialNumber)の設定を返す
                        //その設定も無ければ1を返す
                        int defaultNumber = 1;
						if (ctg == Category.AsmLot || ctg == Category.AsmLotHigh)
						{
							if (Config.Settings.LotDefaultSerialNumber != null)
							{
								List<KeyValuePair<string, int>> serialNumberList
									= Config.Settings.LotDefaultSerialNumber.Where(l => l.Key == lineno).ToList();
								if (serialNumberList.Count != 0)
								{
									defaultNumber = serialNumberList.Single().Value;
								}
							}
						}
                        else if (ctg == Category.CutLot)
                        {
                            if (Config.Settings.CutLotDefaultSerialNumber != null)
                            {
                                List<KeyValuePair<string, int>> serialNumberList
                                    = Config.Settings.CutLotDefaultSerialNumber.Where(l => l.Key == lineno).ToList();
                                if (serialNumberList.Count != 0)
                                {
                                    defaultNumber = serialNumberList.Single().Value;
                                }
                            }
                        }
                        cmd.Parameters.Add("@LASTNO", SqlDbType.BigInt).Value = defaultNumber;

						cmd.CommandText = @"
						INSERT INTO TnNumbering(category, version, [lineno], lastno, lastupddt)
						VALUES(@CATEGORY, @VERSION, @LINENO, @LASTNO, @LASTUPDDT)";
						cmd.ExecuteNonQuery();
						cmd.Transaction.Commit();
						return defaultNumber;
                    }

                    int lastno = SQLite.ParseInt(objlastno);
                    lastno += 1;

                    cmd.Parameters.Add("@LASTNO", SqlDbType.BigInt).Value = lastno;

                    cmd.CommandText = @"
                        UPDATE TnNumbering 
                        SET lastno=@LASTNO, lastupddt=@LASTUPDDT
                        WHERE category=@CATEGORY AND version=@VERSION AND [lineno]=@LINENO";
                    cmd.ExecuteNonQuery();
                    cmd.Transaction.Commit();

                    return lastno;
                }
                catch (Exception ex)
                {
                    if (cmd.Transaction != null)
                    {
                        cmd.Transaction.Rollback();
                    }
                    throw new ArmsException("採番処理エラー:" + ctg.ToString() + ":" + version.ToString(), ex);
                }
            }
        }
    }
}
