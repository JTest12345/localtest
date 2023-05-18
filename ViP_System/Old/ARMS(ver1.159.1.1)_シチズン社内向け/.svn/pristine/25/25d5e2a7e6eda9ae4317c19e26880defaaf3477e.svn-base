using ArmsApi.Model;
using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
	public class TransferCarrierController : Controller
	{
		//
		// GET: /ChangeCarrier/


		public ActionResult Index(string txtRingNo, string txtNextRingNo)
		{
			TransferCarrierModel m = Session["model"] as TransferCarrierModel;
			if (m == null)
			{
				m = new TransferCarrierModel();
				m.RingList = new List<TransferCarrierModel.RingCmb>();
			}

			if (!string.IsNullOrEmpty(txtRingNo) && !string.IsNullOrEmpty(txtNextRingNo))
			{
				try
				{
					m.InputCheck(txtRingNo, txtNextRingNo);
				}
				catch (Exception ex)
				{
					return RedirectToAction("Message", "home", new { msg = ex.Message });
				}
			}

			Session["model"] = m;

			return View(m);
		}

		public ActionResult Confirm()
		{
			TransferCarrierModel m = Session["model"] as TransferCarrierModel;
			if (m == null)
			{
				return RedirectToAction("Message", "Home", new { msg = "TransferCarrierModelが見つかりません" });
			}

			try
			{
				//m.RingTransfer();
			}

			catch (Exception err)
			{
				return RedirectToAction("Message", "Home", new { msg = "ロットとキャリアQR関連付け変更でエラー：" + err.Message });
			}

			Session["model"] = null;
			Session["empcd"] = null;
			Session["lotno"] = null;
			Session["ringno"] = null;
			Session["nextringno"] = null;

			return RedirectToAction("Message", "Home", new { msg = "登録を完了しました" });
		}

		public ActionResult CancelEdit()
		{
			//キャンセル時はロット番号読み込み画面へ遷移
			Session["model"] = null;
			Session["empcd"] = null;
			Session["lotno"] = null;
			Session["ringno"] = null;
			Session["nextringno"] = null;

			return RedirectToAction("Index");
		}
	}

		
}
