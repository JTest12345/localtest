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
    public class FirstArticle
    {
        public static void Import()
        {
            try
            {
                //string[] lotlist = getActiveLotList();
				AsmLot[] lotlist = getActiveLotList();

                //foreach (string lotno in lotlist)
				foreach (AsmLot lot in lotlist)
                {
                    //AsmLot lot = AsmLot.GetAsmLot(lotno);
                    if (lot == null) continue;

                    applyFirstArticleFromNasca(lot);
                }
			}
			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] FirstArticle Error:" + err.ToString());
			}
		}

        /// <summary>
        /// 初品識別
        /// </summary>
        /// <param name="lot"></param>
        private static void applyFirstArticleFromNasca(AsmLot lot)
        {
            if (lot == null) return;

            using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();
                cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo ?? "";
                cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = lot.TypeCd ?? "";
                cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = Config.FIRST_ARTICLE_LOTCHAR_CD;

                cmd.CommandText = @"
                    select
                     charval_cd
                    from nttltna l(nolock)
                    inner join nttltts t(nolock)
                    on l.nascalot_id = t.nascalot_id
                    where type_cd = @TYPECD
                    and nascalot_no = @LOTNO
                    and lotchar_cd = @LOTCHARCD";

                object val = cmd.ExecuteScalar();
                if (val == null) return;

                ArmsApi.Model.LotChar lc = new ArmsApi.Model.LotChar();
                lc.LotCharCd = Config.FIRST_ARTICLE_LOTCHAR_CD;
                lc.ListVal = val.ToString();
                lc.DeleteInsert(lot.NascaLotNo);

                Log.SysLog.Info("初品識別特性反映:" + lot.NascaLotNo);
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

//		private static string[] getActiveLotList()
//		{
//			List<string> retv = new List<string>();

//			using (SqlConnection con = new SqlConnection(Config.Settings.LocalConnString))
//			using (SqlCommand cmd = con.CreateCommand())
//			{
//				con.Open();

//				cmd.CommandText = @"
//                    select lotno from tnlot l with(nolock)
//                    where not exists(select * from tncutblend b where l.lotno = b.lotno)";

//				using (SqlDataReader rd = cmd.ExecuteReader())
//				{
//					while (rd.Read())
//					{
//						retv.Add(rd["lotno"].ToString());
//					}
//				}
//			}

//			return retv.ToArray();
//		}
    }
}
