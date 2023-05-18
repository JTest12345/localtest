using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class CassetteTransfer2to1Controller : Controller
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
            CassetteTransfer2to1Model m = null;
            try
            {
                m = new CassetteTransfer2to1Model(txtMagNo);
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
            CassetteTransfer2to1Model m = Session["model"] as CassetteTransfer2to1Model;
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
            CassetteTransfer2to1Model m = Session["model"] as CassetteTransfer2to1Model;
            m.selectedprocno = Convert.ToInt32(procindex);
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }


        public ActionResult InputCassette(string txtdetachCassNo, string txtattachCassNo, string txtattachPosition)
        {
            CassetteTransfer2to1Model m = Session["model"] as CassetteTransfer2to1Model;
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
            if (!string.IsNullOrEmpty(txtattachPosition))
            {
                m.attachposition = txtattachPosition;
            }

            if (!string.IsNullOrEmpty(txtdetachCassNo) && !string.IsNullOrEmpty(txtattachCassNo) && !string.IsNullOrEmpty(txtattachPosition))
            {
                string[] detcst = txtdetachCassNo.Split(' ');
                string[] atacst = txtattachCassNo.Split(' ');

                CassetteTransfer2to1Model.CassetteCng cassetteCng = new CassetteTransfer2to1Model.CassetteCng();
                if (detcst.Length >= 3) 
                {
                    cassetteCng.cassetteno = txtdetachCassNo;
                }
                else {
                    return RedirectToAction("Message", "Home", new { msg = "フォーマットエラー：リングIDを読み込んでください。" });
                }

                if (atacst.Length >= 3)
                {
                    cassetteCng.nextcassetteno = txtattachCassNo;
                }
                else
                {
                    return RedirectToAction("Message", "Home", new { msg = "フォーマットエラー：リングIDを読み込んでください。" });
                }

                //研削搭載の冶具設置時の左右と最終の左右が違うのでコードは入れ替る。
                if(txtattachPosition == CassetteTransfer2to1Model.POSITION_CD_LEFT)
                {
                    cassetteCng.branchno = CassetteTransfer2to1Model.BRANCH_NO_RIGHT;
                }
                else if(txtattachPosition == CassetteTransfer2to1Model.POSITION_CD_RIGHT)
                {
                    cassetteCng.branchno = CassetteTransfer2to1Model.BRANCH_NO_LEFT;
                }
                else
                {
                    return RedirectToAction("Message", "Home", new { msg = "位置情報コードが不正です。（『MIGI』/『HIDARI』の何れか以外)" });
                }

                m.listCassetteCng.Add(cassetteCng);

                m.precassetteno = "";
                m.prenextcassetteno = "";
                m.attachposition = "";
            }
            Session["model"] = m;

            return View(m);
        }

        public ActionResult Submit()
        {
            CassetteTransfer2to1Model m = Session["model"] as CassetteTransfer2to1Model;
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
