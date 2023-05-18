using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Data;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsNascaBridge2
{
    public class ElasticitySampler
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

                    applyElasticityTestFromNasca(lot);
                }
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] ElasticitySampler Error:" + err.ToString());
			}
		}

        private static void applyElasticityTestFromNasca(AsmLot lot)
        {
            if (lot == null) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo ?? "";
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = lot.TypeCd ?? "";
                cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = Config.ELASTICITY_TEST_CT_LOTCHAR_CD;

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

                int elasticityct;
                if (int.TryParse(val.ToString(), out elasticityct))
                {
                    if (elasticityct > 0)
                    {
                        //最新情報取得して更新
                        lot = AsmLot.GetAsmLot(lot.NascaLotNo);
                        lot.IsElasticityTest = true;
                        lot.Update();

                        LotChar lc = new LotChar();
                        lc.LotCharCd = Config.ELASTICITY_TEST_CT_LOTCHAR_CD;
                        lc.LotCharVal = elasticityct.ToString();
                        lc.DeleteInsert(lot.NascaLotNo);

                        Log.SysLog.Info("弾性率試験特性反映:" + lot.NascaLotNo);
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
