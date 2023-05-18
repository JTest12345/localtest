using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class ConveyanceController : Controller
    {
        public ActionResult Index()
        {
            ConveyanceModel m = Session["model"] as ConveyanceModel;
            if (m == null)
            {
                string empcd = (string)Session["empcd"];
                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "Conveyance";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m = new ConveyanceModel(empcd);
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo, FormCollection fc)
        {
            ConveyanceModel m = Session["model"] as ConveyanceModel;

            if (!string.IsNullOrEmpty(txtMagNo))
            {
                string magno;
                int seqNo = 0;

                string[] elms = txtMagNo.Split(' ');
                if (elms.Length == 2 && txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno = elms[1];
                }
                else if (elms.Length == 4 && txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno = elms[1];
                    seqNo = int.Parse(elms[2]);
                    magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqNo);
                }
                else if (txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    magno = txtMagNo.Replace(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE, "");
                }
                else
                {
                    TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    return View(m);
                }

                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno);

                if (mag == null)
                {
                    if (seqNo >= 1)
                    {
                        seqNo = 0;
                        magno = ArmsApi.Model.Order.MagLotToNascaLot(magno);
                        mag = ArmsApi.Model.Magazine.GetCurrent(magno);
                        if (mag == null)
                        {
                            TempData["AlertMsg"] = "ロット情報が存在しません";
                            return View(m);
                        }
                    }

                    else
                    {
                        TempData["AlertMsg"] = "ロット情報が存在しません";
                        return View(m);
                    }
                }

                //重複読込チェック
                if (m.MagList.Where(em => em.MagazineNo == magno).Count() >= 1)
                {
                    TempData["AlertMsg"] = "既に読み込み済みのマガジンです";
                    return View(m);
                }

                m.MagList.Add(mag);
            }
            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult Confirm()
        {
            ConveyanceModel m = Session["model"] as ConveyanceModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "ConveyanceModelが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Submit()
        {
            ConveyanceModel m = Session["model"] as ConveyanceModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "ConveyanceModelが見つかりません" });
            }

            try
            {
                string msg;
                bool success = m.WorkStartEndConveyance(out msg);

                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "搬送実績登録で予期せぬエラー：" + ex.Message });
            }

            string messageStr = "搬送実績登録が完了しました。";
            return RedirectToAction("Message", "Home", new { msg = messageStr });
        }
    }
}