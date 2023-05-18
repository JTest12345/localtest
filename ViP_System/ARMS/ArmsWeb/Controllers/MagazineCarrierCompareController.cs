using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class MagazineCarrierCompareController : Controller
    {
        public ActionResult Index()
        {
            MagazineCarrierCompareModel m = Session["model"] as MagazineCarrierCompareModel;
            if (m == null)
            {
                string magazineno = (string)Session["magazineno"];
                if (string.IsNullOrEmpty(magazineno))
                {
                    //マガジン番号読み込み
                    Session["redirectController"] = "MagazineCarrierCompare";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputMagazineNo", "Home");
                }

                m = new MagazineCarrierCompareModel(magazineno);
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtDataMatrix, FormCollection fc)
        {
            MagazineCarrierCompareModel m = Session["model"] as MagazineCarrierCompareModel;

            //基板DM読み込み
            if (!string.IsNullOrEmpty(txtDataMatrix))
            {
                string datamatrix = txtDataMatrix;
                
                //重複読込チェック
                if (m.DataMatirxList.Where(dm => dm == datamatrix).Count() >= 1)
                {
                    TempData["AlertMsg"] = "既に読み込み済みの基板DM/キャリアQRです";
                    return View(m);
                }

                m.DataMatirxList.Add(datamatrix);
            }
            else
            {
                TempData["AlertMsg"] = "基板DM/キャリアQRを読み込んでください";
                return View(m);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            //キャンセル時はマガジン番号読み込み画面へ遷移
            Session["model"] = null;
            return RedirectToAction("InputMagazineNo", "Home");
        }

        public ActionResult Confirm()
        {
            MagazineCarrierCompareModel m = Session["model"] as MagazineCarrierCompareModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "MagazineCarrierCompareModelが見つかりません" });
            }

            m.Compare();

            Session["model"] = m;
            return View(m);
        }
    }
}