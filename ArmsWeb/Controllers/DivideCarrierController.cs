using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class DivideCarrierController : Controller
    {
        //
        // GET: /DivideCarrier/

        public ActionResult Index()
        {
            DivideCarrierModel m = Session["model"] as DivideCarrierModel;

			if (m == null)
			{
				string empCd = (string)Session["empcd"];
				if (string.IsNullOrEmpty(empCd))
				{
					Session["redirectController"] = "DivideCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputEmpCd", "Home");
				}

				m = new DivideCarrierModel(empCd);
				m.EmpCD = empCd;
				Session["model"] = m;
			}

			if (string.IsNullOrEmpty(m.LotNo))
            {
                string lotno = (string)Session["lotno"];
                if (string.IsNullOrEmpty(lotno))
                {
                    //ロット番号読み込み
                    Session["redirectController"] = "DivideCarrier";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputLotNo", "Home");
                }

                m.LotNo = lotno;
                Session["model"] = m;
            }

            //if (string.IsNullOrEmpty(m.MagazineNo))
            //{
            //    string magazineno = (string)Session["magazineno"];
            //    if (string.IsNullOrEmpty(magazineno))
            //    {
            //        //マガジン番号読み込み
            //        Session["redirectController"] = "DivideCarrier";
            //        Session["redirectAction"] = "Index";
            //        return RedirectToAction("InputMagazineNo", "Home");
            //    }

            //    m.MagazineNo = magazineno;
            //    Session["model"] = m;
            //}

			if (string.IsNullOrEmpty(m.OrgCarrierNo))
			{
				string orgCarrierNo = (string)Session["orgcarrierno"];
				if (string.IsNullOrEmpty(orgCarrierNo))
				{
					Session["redirectController"] = "DivideCarrier";
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
					Session["redirectController"] = "DivideCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputNewCarrierNo", "Home");
				}

				m.NewCarrierNo = newCarrierNo;
				Session["model"] = m;
			}

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Submit() 
        {
            DivideCarrierModel m = Session["model"] as DivideCarrierModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                List<string> msg;
                bool success = m.DivideCarrier(out msg);
                if (!success)
                {
                    string rawmsg = string.Join(" ", msg.ToArray());
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + rawmsg });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "完了登録で予期せぬエラー：" + ex.Message });
            }

			Session["model"] = null;
			Session["lotno"] = null;
			Session["magazineno"] = null;

			Session["orgcarrierno"] = null;
			Session["newcarrierno"] = null;
			Session["empcd"] = null;

            m.NewCarrierNo = null;

			//return RedirectToAction("Index");

            return RedirectToAction("Message", "Home", new { msg = "登録を完了しました" });
        }

		public ActionResult CancelEdit()
		{
            Session["model"] = null;
            Session["lotno"] = null;
            Session["magazineno"] = null;

            Session["orgcarrierno"] = null;
            Session["newcarrierno"] = null;

			Session["empcd"] = null;

            return RedirectToAction("Index");
		}
    }
}
