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
	/// <summary>
	/// ライフ試験結果
	/// ※仮カットブレンド登録ロットのみ取得
	/// </summary>
	public class LifeTestResult
	{
		public static void Import()
		{
			try
			{
				AsmLot[] lotlist = getActiveLotList();
                //2015.10.09 車載CT特採対応。仮ブレンドロットについてもライフ試験結果を取得する。
                var tempblendlist = lotlist.Where(l => l.TempCutBlendNo != "").Select(l => new {l.TypeCd, l.TempCutBlendNo}).Distinct().ToArray();
               
				foreach (AsmLot lot in lotlist)
				{ 
					applyLifeTestResultFromNasca(lot.TypeCd, lot.NascaLotNo);
				}

                foreach (var lot in tempblendlist)
                {
                    applyLifeTestResultFromNasca(lot.TypeCd, lot.TempCutBlendNo);
                }		
			}

			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] LifeTestResult Error:" + err.ToString());
			}
		}

		private static void applyLifeTestResultFromNasca(string typecd, string lotno)
		{
			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();
				cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lotno ?? "";
				cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = typecd ?? "";
				cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = ArmsApi.Model.LotChar.LIFETESTRESULTCONDCD_LOTCHARCD;

				cmd.CommandText = @"
                    select
                     lotchar_cd, charval_cd, lotchar_val
                    from nttltna l(nolock)
                    inner join nttltts t(nolock)
                    on l.nascalot_id = t.nascalot_id
                    where type_cd = @TYPECD
                    and nascalot_no = @LOTNO
                    and lotchar_cd = @LOTCHARCD";

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while (rd.Read())
					{
                        ArmsApi.Model.LotChar lc = new ArmsApi.Model.LotChar();
						lc.LotCharCd = rd["lotchar_cd"].ToString().Trim();
						lc.ListVal = rd["charval_cd"].ToString().Trim();
						lc.DeleteInsert(lotno);
					}
				}
			}
		}

		private static AsmLot[] getActiveLotList()
		{
			List<AsmLot> retv = new List<AsmLot>();

			Magazine[] maglist = Magazine.GetMagazine(null, null, true, null);
			foreach (Magazine mag in maglist)
			{
				AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
								
				if (lot == null) continue;
                if (lot.TempCutBlendNo == string.Empty) continue;

				retv.Add(lot);
			}

			return retv.ToArray();
		}

	}
}
