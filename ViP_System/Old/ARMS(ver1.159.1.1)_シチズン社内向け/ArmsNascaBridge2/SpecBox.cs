using ArmsApi;
using ArmsApi.Model;
using ArmsApi.Model.SQDB;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class SpecBox
    {
        public static void Import()
        {
            try
            {
                AsmLot[] lotlist = getActiveLotList();
                Log.SysLog.Info("[ArmsNascaBridge2] SpecBox LoopStart:" + DateTime.Now.ToString());
                foreach (AsmLot lot in lotlist)
                {
                    getMnggrInfoFromNasca(lot);
                    lot.Update();
                }
            }

            catch (Exception err)
            {
                Log.SysLog.Error("[ArmsNascaBridge2] SpecBox Error:" + err.ToString());
            }
        }
        
        private static AsmLot[] getActiveLotList()
        {
            List<AsmLot> retv = new List<AsmLot>();

            Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);
            foreach (Magazine mag in maglist)
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot != null) retv.Add(lot);
            }

            return retv.ToArray();
        }

        /// <summary>
        /// NASCAからスペックボックスを取得。ロットのレコードがあれば、trueを返す。
        /// </summary>
        private static bool getMnggrInfoFromNasca(AsmLot lot)
        {
            bool retv = false;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo;
                cmd.Parameters.Add("@MATECD", SqlDbType.Char).Value = lot.TypeCd + ".%";

                cmd.CommandText = @"
                    SELECT TOP 1 tbl2.mnggr_id, tbl2.mnggr_nm
                    FROM [SC00_RELAY].[dbo].[NttSPBX] AS tbl1 WITH(NOLOCK)
                        INNER JOIN [dbo].[VM_SIMMNGGR] AS tbl2 WITH(NOLOCK)
                            ON tbl1.Mnggr_ID = tbl2.mnggr_id
                    WHERE tbl1.RootsLot_NO = @LOTNO
                        AND tbl1.Material_CD LIKE @MATECD
                        AND tbl1.Del_FG = 0
                        AND tbl1.Operate_FG = 1
                    ORDER BY Spec_DT DESC
                    OPTION(MAXDOP 1)";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    if (rd.Read())
                    {
                        lot.MnggrId = SQLite.ParseNullableInt(rd["mnggr_id"]);
                        lot.MnggrNm = SQLite.ParseString(rd["mnggr_nm"]);

                        retv = true;
                    }
                }
            }

            return retv;
        }
    }
}
