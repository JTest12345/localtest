using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class CassetteTransferController : Controller
    {
        //
        // GET: /CassetteTransfer/
        public ActionResult Index()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo)
        {
            CassetteTransferModel m = null;
            try
            {
                m = new CassetteTransferModel(txtMagNo);
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return View(m);
            }

            Session["model"] = m;
            return RedirectToAction("InputProcInfo");
        }

        public ActionResult InputProcInfo()
        {
            CassetteTransferModel m = Session["model"] as CassetteTransferModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputProcInfo(string procindex)
        {
            CassetteTransferModel m = Session["model"] as CassetteTransferModel;
            m.selectedprocno = Convert.ToInt32(procindex);
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }


        public ActionResult InputCassette(string txtdetachCassNo, string txtattachCassNo)
        {
            CassetteTransferModel m = Session["model"] as CassetteTransferModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }
            if (m.selectedprocno == 0)
            {
                return RedirectToAction("Message", "Home", new { msg = "作業選択後に[選択]ボタン押下して、[次へ]を押下してください。" });
            }
            
            if (!string.IsNullOrEmpty(txtdetachCassNo))
            {
                m.precassetteno = txtdetachCassNo;
            }
            if (!string.IsNullOrEmpty(txtattachCassNo))
            {
                m.prenextcassetteno = txtattachCassNo;
            }
            
            if (!string.IsNullOrEmpty(txtdetachCassNo) && !string.IsNullOrEmpty(txtattachCassNo))
            {
                string[] detcst = txtdetachCassNo.Split(' ');
                string[] atacst = txtattachCassNo.Split(' ');

                CassetteTransferModel.CassetteCng cassetteCng = new CassetteTransferModel.CassetteCng();
                if (detcst.Length >= 3) 
                {
                    //cassetteCng.cassetteno = detcst[2];
                    cassetteCng.cassetteno = txtdetachCassNo;
                }
                //else if (detcst.Length == 4)
                //{
                //    //cassetteCng.cassetteno = detcst[3];
                //    cassetteCng.cassetteno = txtdetachCassNo;
                //}
                else {
                    return RedirectToAction("Message", "Home", new { msg = "フォーマットエラー：リングIDを読み込んでください。" });
                }

                if (atacst.Length >= 3)
                {
                    //cassetteCng.nextcassetteno = atacst[2];
                    cassetteCng.nextcassetteno = txtattachCassNo;
                }
                //else if (atacst.Length == 4)
                //{
                //    //cassetteCng.nextcassetteno = atacst[3];
                //    cassetteCng.nextcassetteno = txtattachCassNo;
                //}
                else
                {
                    return RedirectToAction("Message", "Home", new { msg = "フォーマットエラー：リングIDを読み込んでください。" });
                }

                m.listCassetteCng.Add(cassetteCng);

                m.precassetteno = "";
                m.prenextcassetteno = "";
            }
            Session["model"] = m;

            return View(m);
        }

        public ActionResult Submit()
        {
            CassetteTransferModel m = Session["model"] as CassetteTransferModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            //登録
            try
            {
                m.UpdateDB();
            }
            catch (ApplicationException ex)
            {
                return RedirectToAction("Message", "Home", new { msg = ex.Message});
            }

            //return View(m);
            return RedirectToAction("Message", "Home", new { msg = "登録完了しました。" });
        }
    }
}
