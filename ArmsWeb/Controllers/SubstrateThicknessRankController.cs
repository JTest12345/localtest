using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class SubstrateThicknessRankController : Controller
    {
        public ActionResult Index()
        {
            SubstrateThicknessRankModel m = Session["model"] as SubstrateThicknessRankModel;
            if (m == null)
            {
                string ringdata = (string)Session["ringdata"];
                if (string.IsNullOrEmpty(ringdata))
                {
                    //リングデータ読み込み
                    Session["redirectController"] = "SubstrateThicknessRank";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputRing", "Home");
                }

                m = new SubstrateThicknessRankModel(ringdata);
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtDataMatrix, FormCollection fc)
        {
            SubstrateThicknessRankModel m = Session["model"] as SubstrateThicknessRankModel;
            
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
            return RedirectToAction("Index", "Home");
        }

        public ActionResult Confirm()
        {
            SubstrateThicknessRankModel m = Session["model"] as SubstrateThicknessRankModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "SubstrateThicknessRankModelが見つかりません" });
            }

            string msg = string.Empty;
            if (!m.CheckSubstrateThicknessRank(out msg))
            {
                return RedirectToAction("Message", "Home", new { msg = "照合NG：基板の厚みランクが異なります。" + msg });
            }
            else
            {
                m.Update();
                return RedirectToAction("Message", "Home", new { msg = "照合OK" });
            }
        }
    }
}