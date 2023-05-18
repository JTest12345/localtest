using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using System.Text;

namespace ArmsWeb.Controllers
{
    public class CassetteRelationController : Controller
    {
        //
        // GET: /CassetteRelation/

        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo)
        {
            CasseteRelationModel m = null;
            try
            {
                m = new CasseteRelationModel(txtMagNo);
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return View(m);
            }

            Session["model"] = m;
            return RedirectToAction("InputCassette");
        }

        public ActionResult InputCassette(string txtCassNo)
        {
            CasseteRelationModel m = Session["model"] as CasseteRelationModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (!string.IsNullOrEmpty(txtCassNo))
            {
                string[] cst = txtCassNo.Split(' ');

                if (cst.Length >= 3)
                {
                    //m.listCassetteNo.Add(cst[2]);
                    m.listCassetteNo.Add(txtCassNo);//メーカー違いでリング番号重複が起きそうなので修正。
                }
                else
                {
                    return RedirectToAction("Message", "Home", new { msg = "フォーマットエラー：リングIDを読み込んでください。" });
                }
            }
            return View(m);
        }

        public ActionResult Submit()
        {
            CasseteRelationModel m = Session["model"] as CasseteRelationModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                //登録
                m.UpdateDB();
            }
            catch (ApplicationException ex)
            {
                return RedirectToAction("Message", "Home", new { msg = ex.Message });
            }

            //return View(m);
            return RedirectToAction("Message", "Home", new { msg = "登録完了しました。" });
        }
    }
}
