using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class ChangeCarrierController : Controller
    {
        //
        // GET: /ChangeCarrier/

        public ActionResult Index()
        {			
			ChangeCarrierModel m = Session["model"] as ChangeCarrierModel;
			if (m == null)
			{
				string empcd = (string)Session["empcd"];
				if (string.IsNullOrEmpty(empcd))
				{
					//社員番号読み込み
					Session["redirectController"] = "ChangeCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputEmpCd", "Home");
				}

				m = new ChangeCarrierModel(empcd);
				Session["model"] = m;
			}

			if (string.IsNullOrEmpty(m.LotNo))
			{
				string lotno = (string)Session["lotno"];
				if (string.IsNullOrEmpty(lotno))
				{
					//ロット番号読み込み
					Session["redirectController"] = "ChangeCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputLotNo", "Home");
				}

				m.LotNo = lotno;
				Session["model"] = m;
			}

			if (string.IsNullOrEmpty(m.OrgCarrierNo))
			{
				string orgCarrierNo = (string)Session["orgcarrierno"];
				if (string.IsNullOrEmpty(orgCarrierNo))
				{
					Session["redirectController"] = "ChangeCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputCurrentCarrierNo", "Home");
				}
				
				m.OrgCarrierNo = orgCarrierNo;
				Session["model"] = m;
			}

			if (string.IsNullOrEmpty(m.NewCarrierNo))
			{
				string newCarrierNo = (string)Session["newcarrierno"];
				if (string.IsNullOrEmpty(newCarrierNo))
				{
					Session["redirectController"] = "ChangeCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputNewCarrierNo", "Home");
				}

				m.NewCarrierNo = newCarrierNo;
				Session["model"] = m;
			}

			Session["model"] = m;
			return View(m);
        }

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult Index(string txtDataMatrix, FormCollection fc)
		{
			ChangeCarrierModel m = Session["model"] as ChangeCarrierModel;

			////基板DM読み込み
			//if (!string.IsNullOrEmpty(txtDataMatrix))
			//{
//				string datamatrix = txtDataMatrix;

//				//重複読込チェック
				//if (m.DataMatirxList.Where(dm => dm == datamatrix).Count() >= 1)
				//{
				//	TempData["AlertMsg"] = "既に読み込み済みの基板DM/キャリアQRです";
				//	return View(m);
//				}

//				m.DataMatirxList.Add(datamatrix);
			//}
			//else
			//{
//				TempData["AlertMsg"] = "基板DM/キャリアQRを読み込んでください";
				//return View(m);
			//}

			Session["model"] = m;
			return View(m);
		}

		public ActionResult Submit()
		{
			try
			{
				ChangeCarrierModel m = Session["model"] as ChangeCarrierModel;
				if (m == null)
				{
					return RedirectToAction("Message", "Home", new { msg = "ChangeCarrierModelが見つかりません" });
				}

				//指定したロットで稼働中のデータがあるか確認
				m.ChangeCarrier();

			}
			catch (Exception err)
			{
				return RedirectToAction("Message", "Home", new { msg = "ロットとキャリアQR関連付け変更でエラー：" +  err.Message});
			}

			Session["model"] = null;
			Session["empcd"] = null;
			Session["lotno"] = null;
			Session["orgcarrierno"] = null;
			Session["newcarrierno"] = null;

			return RedirectToAction("Message", "Home", new { msg = "登録を完了しました" });
			//return RedirectToAction("InputLotNo", "Home");
		}

		public ActionResult CancelEdit()
		{
			//キャンセル時はロット番号読み込み画面へ遷移
			Session["model"] = null;
			Session["empcd"] = null;
			Session["lotno"] = null;
			Session["orgcarrierno"] = null;
			Session["newcarrierno"] = null;

			return RedirectToAction("Index");
		}
    }
}
