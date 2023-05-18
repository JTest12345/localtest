using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArmsNascaBridge2
{
    public class ReflowSampler
    {
        public static void Import()
        {
            try
            {
                string[] lotlist = getActiveLotList();

                foreach (string lotno in lotlist)
                {
                    AsmLot lot = AsmLot.GetAsmLot(lotno);
                    if (lot == null) continue;

                    applyReflowTestFromNasca(lot);
                    applyReflowTestWirebondFromNasca(lot);
                }
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] ReflowSampler Error:" + err.ToString());
			}
		}

        /// <summary>
        /// リフローパス試験数
        /// </summary>
        /// <param name="lot"></param>
        private static void applyReflowTestFromNasca(AsmLot lot)
        {
            if (lot == null) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo ?? "";
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = lot.TypeCd ?? "";
                cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = Config.REFLOW_TEST_CT_LOTCHAR_CD;

                cmd.CommandText = @"
                    select
                     lotchar_val
                    from nttltna l(nolock)
                    inner join nttltts t(nolock)
                    on l.nascalot_id = t.nascalot_id
                    where type_cd = @TYPECD
                    and nascalot_no = @LOTNO
                    and lotchar_cd = @LOTCHARCD";

                object val = cmd.ExecuteScalar();
                if (val == null) return;

                int reflowct;
                if (int.TryParse(val.ToString(), out reflowct))
                {
                    if (reflowct > 0)
                    {
                        //最新情報取得して更新
                        lot = AsmLot.GetAsmLot(lot.NascaLotNo);
                        lot.IsReflowTest = true;
                        lot.Update();

                        LotChar lc = new LotChar();
                        lc.LotCharCd = Config.REFLOW_TEST_CT_LOTCHAR_CD;
                        lc.LotCharVal = reflowct.ToString();
                        lc.DeleteInsert(lot.NascaLotNo);

                        Log.SysLog.Info("リフローパス試験数特性反映:" + lot.NascaLotNo);
                    }
                }
            }
        }

        /// <summary>
        /// リフローパス試験(WB)数
        /// </summary>
        /// <param name="lot"></param>
        private static void applyReflowTestWirebondFromNasca(AsmLot lot)
        {
            if (lot == null) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo ?? "";
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = lot.TypeCd ?? "";
                cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = Config.REFLOW_TEST_CT_WB_LOTCHAR_CD;

                cmd.CommandText = @"
                    select
                     lotchar_val
                    from nttltna l(nolock)
                    inner join nttltts t(nolock)
                    on l.nascalot_id = t.nascalot_id
                    where type_cd = @TYPECD
                    and nascalot_no = @LOTNO
                    and lotchar_cd = @LOTCHARCD";

                object val = cmd.ExecuteScalar();
                if (val == null) return;

                int reflowct;
                if (int.TryParse(val.ToString(), out reflowct))
                {
                    if (reflowct > 0)
                    {
                        //最新情報取得して更新
                        lot = AsmLot.GetAsmLot(lot.NascaLotNo);
                        lot.IsReflowTestWirebond = true;
                        lot.Update();

                        LotChar lc = new LotChar();
                        lc.LotCharCd = Config.REFLOW_TEST_CT_WB_LOTCHAR_CD;
                        lc.LotCharVal = reflowct.ToString();
                        lc.DeleteInsert(lot.NascaLotNo);

                        Log.SysLog.Info("リフローパス試験(WB)数特性反映:" + lot.NascaLotNo);
                    }
                }
            }
        }

        private static string[] getActiveLotList()
        {
            List<string> retv = new List<string>();

            using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = @"
                    select lotno from tnlot l
                    where not exists(select * from tncutblend b where l.lotno = b.lotno)";

                using (SqlDataReader rd = cmd.ExecuteReader())
                {
                    while (rd.Read())
                    {
                        retv.Add(rd["lotno"].ToString());
                    }
                }
            }

            return retv.ToArray();
        }
    }
}
