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
	public class TestSampler
	{
        /// <summary>
        /// ライフ試験数
        /// </summary>
        private const string LIFETESTCT_LOTCHARCD = "T0000001";

        /// <summary>
        /// カットブレンドの文字数限界値{データベース： ARMS.dbo.TnLot.CutBlendCd NVarChar(64)}
        /// </summary>
        private const int CUTBLENDCD_MAXLENGTH = 64;

        public static void Import()
		{
			try
			{
				GnlMaster[] lotcharList = GnlMaster.GetTestSampleLotChar();
				AsmLot[] lotlist = getActiveLotList();

                foreach (GnlMaster lotchar in lotcharList)
                {
                    foreach (AsmLot lot in lotlist)
                    {
                        foreach (ArmsApi.Model.LotChar lc in getLifeTestFromNasca(lotchar.Code, lot))
                        {
                            lc.DeleteInsert(lot.NascaLotNo);
                        }
                    }
                }

                if (Config.Settings.ExamsMngInfoSrchCondList != null)
                {
                    List<string> DenyCutBlendTypeList = ExamsMngInfo.GetExamsMngInfo_TypeCD(Config.Settings.ExamsMngInfoSrchCondList);

                    foreach (AsmLot lot in lotlist)
                    {
                        // 試験管理データベースの検索条件設定がある場合にカットブレンド不可チェック
                        if (checkDenyCutBlend(DenyCutBlendTypeList, lot) == true)
                        {
                            if (lot.CutBlendCd.Contains(lot.NascaLotNo) == false)
                            {
                                // カットブレンドグループに固有の値で上書きする
                                string addString = $"_{lot.NascaLotNo}";
                                if (lot.CutBlendCd.Length + addString.Length > CUTBLENDCD_MAXLENGTH)
                                    lot.CutBlendCd = lot.CutBlendCd.Substring(0, CUTBLENDCD_MAXLENGTH - addString.Length);
                                lot.CutBlendCd += addString;
                                lot.Update();
                                Log.SysLog.Info("[NascaBridge2] カットブレンドグループ変更:ロットNo=" + lot.NascaLotNo + ",カットブレンドグループ=" + lot.CutBlendCd);
                            }
                        }
                    }
                }
            }

			catch (Exception err)
			{
				Log.SysLog.Error("[ArmsNascaBridge2] TestSampler Error:" + err.ToString());
			}
		}

		private static List<ArmsApi.Model.LotChar> getLifeTestFromNasca(string lotcharcd, AsmLot lot)
		{
            List<ArmsApi.Model.LotChar> lcList = new List<ArmsApi.Model.LotChar>();

			using (SqlConnection con = new SqlConnection(Config.Settings.NASCAConSTR))
			using (SqlCommand cmd = con.CreateCommand())
			{
				con.Open();
				cmd.Parameters.Add("@LOTNO", SqlDbType.VarChar).Value = lot.NascaLotNo ?? "";
				cmd.Parameters.Add("@TYPECD", SqlDbType.Char).Value = lot.TypeCd ?? "";
				cmd.Parameters.Add("@LOTCHARCD", SqlDbType.Char).Value = lotcharcd;

				cmd.CommandText = @"
                    select
                     lotchar_cd, lotchar_val
                    from nttltna l(nolock)
                    inner join nttltts t(nolock)
                    on l.nascalot_id = t.nascalot_id
                    where type_cd = @TYPECD
                    and nascalot_no = @LOTNO
                    and lotchar_cd = @LOTCHARCD";

				using (SqlDataReader rd = cmd.ExecuteReader())
				{
					while(rd.Read())
					{
						int testct;
						if (int.TryParse(rd["lotchar_val"].ToString().Trim(), out testct))
						{
							if (testct > 0)
							{
                                ArmsApi.Model.LotChar lc = new ArmsApi.Model.LotChar();
								lc.LotCharCd = rd["lotchar_cd"].ToString().ToString();
								lc.LotCharVal = testct.ToString();
                                lcList.Add(lc);
							}
						}
					}
				}
			}

            return lcList;
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

        private static bool checkDenyCutBlend(List<string> DenyCutBlendTypeList, AsmLot lot)
        {
            if (DenyCutBlendTypeList.Contains(lot.TypeCd) == false)
            {
                return false;
            }

            List<ArmsApi.Model.LotChar> LifeTestCtList = getLifeTestFromNasca(LIFETESTCT_LOTCHARCD, lot);

            if (LifeTestCtList.Count == 0)
            {
                return false;
            }

            foreach (ArmsApi.Model.LotChar lc in LifeTestCtList)
            {
                int testct;
                if(int.TryParse(lc.LotCharVal, out testct))
                {
                    if(testct > 0)
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
