using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi;

namespace ArmsWeb.Controllers
{
    public class GrinderRingController : Controller
    {
        //
        // GET: /GrinderRing/

		public ActionResult Index()
		{
			GrinderRingModel m = Session["model"] as GrinderRingModel;
			if (m == null)
			{
				string empcd = (string)Session["empcd"];
				if (string.IsNullOrEmpty(empcd))
				{
					//社員番号読み込み
					Session["redirectController"] = "GrinderRing";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputEmpCd", "Home");
				}
				m = new GrinderRingModel(empcd);
				Session["model"] = m;
			}

			if (string.IsNullOrEmpty(m.GrinderRingNo))
			{
				string grinderRingNo = (string)Session["ringdata"];
				if (string.IsNullOrEmpty(grinderRingNo))
				{
					//ロット番号読み込み
					Session["redirectController"] = "GrinderRing";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputRing", "Home");
				}
				m.GrinderRingNo = grinderRingNo;
				Session["model"] = m;
			}

			Session["model"] = m;
			return View(m);
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Index(string txtRingNo, FormCollection fc)
		{
			GrinderRingModel m = Session["model"] as GrinderRingModel;

			//基板DM読み込み
			if (!string.IsNullOrEmpty(txtRingNo))
			{
				string ringNo = txtRingNo;

				//重複読込チェック
				if (m.RingNoList.Where(dm => dm == ringNo).Count() >= 1)
				{
					TempData["AlertMsg"] = "既に読み込み済みの基板/角リングDMです";
					return View(m);
				}

				m.RingNoList.Add(ringNo);
			}
			else
			{
				TempData["AlertMsg"] = "基板/角リングDMを読み込んでください";
				return View(m);
			}

			Session["model"] = m;
			return View(m);
		}

		public ActionResult Remove()
		{
			GrinderRingModel m = Session["model"] as GrinderRingModel;

            //ARMS4のLENS組込までの臨時対応。TnCussetteにデータがあればそちら優先。最終的にはGetCarrierNoの方だけになる。
            List<ArmsApi.Model.Cassette> cassetteList = ArmsApi.Model.Cassette.GetCassetteList(null, null, m.GrinderRingNo, 1);
            if(cassetteList.Count() > 0)
            {
                m.RingNoList = cassetteList.Select(c => c.CassetteNo).ToList();
            }
            else
            {
                string lotno = ArmsApi.Model.LotCarrier.GetLotNoFromRingNo(m.GrinderRingNo);
                string[] carrierNoArray = ArmsApi.Model.LotCarrier.GetCarrierNo(lotno, true, m.GrinderRingNo);
                m.RingNoList = carrierNoArray.ToList();
            }
			return View(m);
		}

		public ActionResult RemoveConfirmed()
		{
			GrinderRingModel m = Session["model"] as GrinderRingModel;

			foreach (string carrierNo in m.RingNoList)
			{
                //ARMS4のLENS組込までの臨時対応。TnCussetteがある場合そちらを優先。最終的にはTnLotCarrirのみに統合。
                ArmsApi.Model.Cassette ct = ArmsApi.Model.Cassette.GetCassette(null, carrierNo, 1);
                if (string.IsNullOrWhiteSpace(ct.CassetteNo) == false)
                {
                    ct.RingNo = null;
                    ct.Lastupddt = DateTime.Now;

                    ct.Update();
                }
                else
                {
                    ArmsApi.Model.LotCarrier lc = ArmsApi.Model.LotCarrier.GetData(carrierNo, true, true);

                    lc.RingNo = null;
                    lc.EmpCd = m.EmpCd;

                    lc.UpdateRingNo(true);
                }
			}

			return RedirectToAction("Message", "Home", new { msg = "研削用リング/IPSトレイの関連付け解除が完了しました" });
		}

		public ActionResult Confirm()
        {
			GrinderRingModel m = Session["model"] as GrinderRingModel;
            if (m == null)
            {
				return RedirectToAction("Message", "Home", new { msg = "GrinderRingModelが見つかりません" });
            }

			try
			{
			}
			catch (Exception ex)
			{
				return RedirectToAction("Message", "Home", new { msg = ex.Message });
			}

            Session["model"] = m;
            return View(m);
        }

		public ActionResult Confirmed()
		{
			GrinderRingModel m = Session["model"] as GrinderRingModel;

			if (m == null)
			{
				return RedirectToAction("Message", "Home", new { msg = "LotCarrierModelが見つかりません" });
			}

			try
			{
				//ロットとキャリアをキーにしてGrinderRingNoをアップデートする
				foreach(string carrierNo in m.RingNoList)
				{
                    //ARMS4のLENS組込までの臨時対応。TnCussetteがある場合そちらを優先。最終的にはTnLotCarrirのみに統合。
                    ArmsApi.Model.Cassette ct = ArmsApi.Model.Cassette.GetCassette(null, carrierNo, 1);
                    if (string.IsNullOrWhiteSpace(ct.CassetteNo) == false)
                    {
                        ct.RingNo = m.GrinderRingNo;
                        ct.Lastupddt = DateTime.Now;

                        ct.Update();
                    }
                    else
                    {
                        ArmsApi.Model.LotCarrier lc = ArmsApi.Model.LotCarrier.GetData(carrierNo, true, true);

                        lc.RingNo = m.GrinderRingNo;
                        lc.EmpCd = m.EmpCd;

                        lc.UpdateRingNo(true);
                    }

				}
			}
			catch (Exception ex)
			{
				return RedirectToAction("Message", "Home", new { msg = "ロットと基板DM/キャリアQRの関連付け登録で予期せぬエラー：" + ex.Message });
			}

			return RedirectToAction("Message", "Home", new { msg = "ロットと基板DM/キャリアQRの関連付け登録が完了しました" });
		}
	}
}
