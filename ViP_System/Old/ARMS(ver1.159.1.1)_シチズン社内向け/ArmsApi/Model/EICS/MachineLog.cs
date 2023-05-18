using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ArmsApi.Model.EICS
{
    public class MachineLog
    {
        /// <summary>
        /// 傾向監視ログの数値取得値を取得
        /// </summary>
        /// <param name="plantCd"></param>
        /// <param name="qcParamNo"></param>
        /// <param name="NascaLotNo"></param>
        /// <returns></returns>
        public static decimal? GetNumericValue(int inlineCd, string plantCd, int qcParamNo, string NascaLotNo)
        {
            using (SqlConnection con = new SqlConnection(Config.Settings.QCILConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = $@" SELECT DParameter_VAL FROM TnLog WITH(nolock) 
                                        WHERE Del_FG = 0 
                                        AND Inline_CD = @InlineCd AND Equipment_NO = @EquipmentNo AND QcParam_NO = @QcParamNo AND NascaLot_NO = @NascaLotNo ";

                cmd.Parameters.Add("@InlineCd", SqlDbType.Int).Value = inlineCd;
                cmd.Parameters.Add("@EquipmentNo", SqlDbType.NVarChar).Value = plantCd;
                cmd.Parameters.Add("@QcParamNo", SqlDbType.Int).Value = qcParamNo;
                cmd.Parameters.Add("@NascaLotNo", SqlDbType.VarChar).Value = NascaLotNo;

                object paramValue = cmd.ExecuteScalar();
                if (paramValue == null)
                {
                    return null;
                }
                else
                {
                    return Convert.ToDecimal(paramValue);
                }
            }
        }

        public static decimal? GetGoldWireUseCount(string machineLogPath, DateTime date, string lotNo)
        {
            string dateFolderName = Path.Combine(date.ToString("yyyyMM"), "Bind");

            string[] lotDir = Directory.GetDirectories(Path.Combine(machineLogPath, dateFolderName), lotNo);
            if (lotDir == null)
            {
                return null;
            }
            string[] file = DirectoryHelper.GetFiles(lotDir.FirstOrDefault(), "^MP.*$");
            if (file.Count() == 0)
            {
                return null;
            }

            string[] fileLines = File.ReadAllLines(file.FirstOrDefault(), Encoding.Default);
            IEnumerable<string> line = fileLines.Where(f => Regex.IsMatch(f, @"^.*ワイヤ消費量.*$"));
            if (line.Count() == 0)
                return null;
            //throw new ApplicationException("[算出不可] ファイル内容「ワイヤ消費量」取得失敗");

            decimal usedWireCt = 0;
            if (decimal.TryParse(line.FirstOrDefault().Split(',')[4], out usedWireCt) == false)
                return null;
            //throw new ApplicationException("[算出不可] ファイル内容「ワイヤ消費量」数値取得失敗");

            return usedWireCt;
        }
    }
}
