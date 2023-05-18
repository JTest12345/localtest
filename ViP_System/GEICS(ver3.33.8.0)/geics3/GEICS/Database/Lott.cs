using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Data;

namespace GEICS.Database
{
    public class Lott
    {
        public static string GetDBMachineNames(int lineno, string lotno)
        {
            return string.Join(",", GetMachineNames(lineno, lotno, "ﾀﾞｲﾎﾞﾝﾀﾞｰ").ToArray());
        }
        public static string GetWBMachineNames(int lineno, string lotno)
        {
            return string.Join(",", GetMachineNames(lineno, lotno, "ﾜｲﾔｰﾎﾞﾝﾀﾞｰ").ToArray());
        }

        private static List<string> GetMachineNames(int lineno, string lotno, string assetsnm)
        {
            List<string> retv = new List<string>();

            using (SqlConnection conn = new SqlConnection(Constant.StrQCIL))
            using (SqlCommand cmd = conn.CreateCommand())
            {
                conn.Open();

                // 変更前
                //string sql = @" SELECT TmEQUI.MachinSeq_NO
                //                FROM dbo.TnLOTT AS TnLOTT WITH (nolock) 
                //                INNER JOIN dbo.TmEQUI AS TmEQUI WITH (nolock) ON TnLOTT.Equipment_NO = TmEQUI.Equipment_NO
                //                WHERE (TmEQUI.Del_FG = 0) AND (TnLOTT.NascaLot_NO = @LOTNO) AND (TmEQUI.Assets_NM = @ASSETSNM) AND (TnLOTT.Inline_CD = @LINENO) ";

                string sql = @" SELECT TmEQUI.MachinSeq_NO
                                FROM dbo.TnLOTT AS TnLOTT WITH (nolock) 
                                INNER JOIN dbo.TmEQUI AS TmEQUI WITH (nolock) ON TnLOTT.Equipment_NO = TmEQUI.Equipment_NO
                                WHERE (TmEQUI.Del_FG = 0) AND (TnLOTT.NascaLot_NO = @LOTNO) AND (TmEQUI.Assets_NM = @ASSETSNM) 
                                AND (TnLOTT.Inline_CD in (SELECT Inline_CD FROM TmLINE))  ";

                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno;
                cmd.Parameters.Add("@ASSETSNM", SqlDbType.NVarChar).Value = assetsnm;
                //cmd.Parameters.Add("@LINENO", SqlDbType.Int).Value = lineno;

                cmd.CommandText = sql;
                using (SqlDataReader rd = cmd.ExecuteReader()) 
                {
                    int ordMachineNM = rd.GetOrdinal("MachinSeq_NO");
                    while(rd.Read())
                    {
                        string machineNM = rd.GetString(ordMachineNM);
                        if (retv.Exists(r => r == machineNM)) 
                        {
                            continue;
                        }

                        retv.Add(machineNM);
                    }
                }
            }

            return retv;
        }
    }
}
