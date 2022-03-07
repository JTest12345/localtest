﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CejDataAccessCom;


namespace Samples
{
    class FjhTools
    {
        static void Main(string[] args)
        {
			var getvlot = GetVlot();
			var checkvlot = CheckVlot();

			Console.WriteLine(getvlot);
			Console.WriteLine(checkvlot);
            Console.ReadLine();

        }

		private static string GetVlot()
		{
			string ret = string.Empty;

			string vlotno = "V001";
			//Vロット情報取得
			VlotAccess vl = VlotAccess.GetVLotInfo(vlotno);

			//エラー発生
			if (vl.ErrDescription != "")
			{
				ret += vl.ErrDescription + "\r\n";
				return "えらーですよ～";
			}
			//Vlot情報
			string msg = $"VロットNo({vl.VLotNo}) 基板CD({vl.MaterialCd})";
			if (vl.IsVM) { msg += " V溝あり"; } else { msg += " V溝なし"; }
			if (vl.IsBackUp) { msg += " バックアップ対象"; } else { msg += " バックアップ対象外"; }
			ret += msg + "\r\n";

			ret += "4Mロット情報リスト・機種：" + string.Join(",", vl.SeiLotLst.Select(value => value.TypeCd)) + "\r\n";
			ret += "4Mロット情報リスト・ロットNo：" + string.Join(",", vl.SeiLotLst.Select(value => value.SeiLotNo)) + "\r\n";
			ret += "基板情報リスト・ロットNo：" + string.Join(",", vl.MatLotLst.Select(value => value.MatLotNo)) + "\r\n";
			ret += "基板情報リスト・枚数：" + string.Join(",", vl.MatLotLst.Select(value => value.FrameQty)) + "\r\n";
			ret += "4MロットNoリスト：" + string.Join(",", vl.SeiLotNoLst) + "\r\n";

			return ret;

		}

		private static string CheckVlot()
		{
			string ret = string.Empty;

			//VロットNo
			string vlotno = "V001";
			//4MロットNoリスト
			List<string> seilotnolst = new List<string>();
			seilotnolst.Add("S001");
			seilotnolst.Add("S003");
			seilotnolst.Add("S002");

			//VロットNo、4MロットNo整合性チェック
			VlotAccess vl = VlotAccess.CheckVLotInfo(vlotno, seilotnolst);

			//エラー発生
			if (vl.ErrDescription != "")
			{
				ret += vl.ErrDescription;
				return "エラーですね～" + "\r\n";
			}
			//Vlot情報
			string msg = $"VロットNo({vl.VLotNo}) 基板CD({vl.MaterialCd})";
			if (vl.IsVM) { msg += " V溝あり"; } else { msg += " V溝なし"; }
			if (vl.IsBackUp) { msg += " バックアップ対象"; } else { msg += " バックアップ対象外"; }
			ret += msg + "\r\n";

			ret += "4Mロット情報リスト・機種：" + string.Join(",", vl.SeiLotLst.Select(value => value.TypeCd)) + "\r\n";
			ret += "4Mロット情報リスト・ロットNo：" + string.Join(",", vl.SeiLotLst.Select(value => value.SeiLotNo)) + "\r\n";
			ret += "基板情報リスト・ロットNo：" + string.Join(",", vl.MatLotLst.Select(value => value.MatLotNo)) + "\r\n";
			ret += "基板情報リスト・枚数：" + string.Join(",", vl.MatLotLst.Select(value => value.FrameQty)) + "\r\n";
			ret += "4MロットNoリスト：" + string.Join(",", vl.SeiLotNoLst) + "\r\n";
			ret += "Vロット情報ありチェック用なし4MロットNo：" + string.Join(",", vl.ShortLotLst) + "\r\n";
			ret += "Vロット情報と一致しない4MロットNo：" + string.Join(",", vl.ErrLotLst) + "\r\n";

			if (vl.IsSequenceEqual)
			{
				ret += "登録順含めて4Mロットが一致している" + "\r\n";
			}
			else
			{
				ret += "登録順含めて4Mロットが一致していない" + "\r\n";
			}
			if (vl.IsEqual)
			{
				ret += "登録順問わず4Mロットが一致している" + "\r\n";
			}
			else
			{
				ret += "登録順問わず4Mロットが一致していない" + "\r\n";
			}
			if (vl.IsOkOnly)
			{
				ret += "正常4Mロットのみ" + "\r\n";
			}
			else
			{
				ret += "エラー4Mロットが存在する" + "\r\n";
			}

			return ret;
		}
	}

}