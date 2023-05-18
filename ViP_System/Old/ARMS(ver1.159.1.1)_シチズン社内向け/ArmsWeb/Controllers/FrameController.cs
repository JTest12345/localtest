using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsApi.Model;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class FrameController : Controller
    {
        //
        // GET: /Frame/

        public ActionResult Index(string stocker, string plantcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            FrameEditModel m = Session["model"] as FrameEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "Frame";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new FrameEditModel(code);
            }

            if (!string.IsNullOrEmpty(stocker))
            {
                int? stockerNo = MachineInfo.GetStockerNo(stocker);
                if (stockerNo == null)
                {
                    TempData["AlertMsg"] = "ストッカー情報が不正です:" + stocker;
                    return RedirectToAction("Index");
                }

                Material mat = m.FrameList.Where(f => f.StockerNo == stockerNo).FirstOrDefault();
                if (mat == null)
                {
                    return RedirectToAction("InsertNew", new { stocker = stocker });
                }
                else
                {
                    return RedirectToAction("Remove", new { stocker = stocker });
                }
            }


            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }


        /// <summary>
        /// 投入中のウェハー全削除
        /// </summary>
        /// <param name="matdata"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        /// 
        public ActionResult Remove(string stocker)
        {
            FrameEditModel m = Session["model"] as FrameEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }
            if (string.IsNullOrEmpty(stocker))
            {
                TempData["AlertMsg"] = "ストッカー情報を読み込んでください";
                return RedirectToAction("Index");
            }

            int? stockerNo = MachineInfo.GetStockerNo(stocker);
            if (stockerNo == null)
            {
                TempData["AlertMsg"] = "ストッカー情報が不正です:" + stocker;
                return RedirectToAction("Index");
            }

            m.StockerNo = stockerNo.Value;

            Material mat = m.FrameList.Where(f => f.StockerNo == stockerNo).FirstOrDefault();
            if (mat == null)
            {
                TempData["AlertMsg"] = "ストッカーに割り付いているフレームが存在しません";
                return RedirectToAction("Index");
            }
            m.EditTarget = mat;
            Session["model"] = m;
            return View(m);
        }

        /// <summary>
        /// </summary>
        /// <param name="matdata"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult RemoveConfirmed()
        {
            FrameEditModel m = Session["model"] as FrameEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.Remove();

            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult InsertNew(string stocker)
        {
            FrameEditModel m = Session["model"] as FrameEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }
            if (m.StockerNo == 0)
            {
                if (string.IsNullOrEmpty(stocker))
                {
                    TempData["AlertMsg"] = "ストッカー情報を読み込んでください";
                    return RedirectToAction("Index");
                }

                int? stockerNo = MachineInfo.GetStockerNo(stocker);
                if (stockerNo == null)
                {
                    TempData["AlertMsg"] = "ストッカー情報が不正です:" + stocker;
                    return RedirectToAction("Index");
                }

                m.StockerNo = stockerNo.Value;
            }

            Material mat = m.FrameList.Where(f => f.StockerNo == m.StockerNo).FirstOrDefault();
            if (mat != null)
            {
                TempData["AlertMsg"] = "先にフレームを取り外してください";
                return RedirectToAction("Index");
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult InsertConfirm(string barcode)
        {
            FrameEditModel m = Session["model"] as FrameEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                Material frame = m.ParseFrame(barcode, m.StockerNo);
                m.EditTarget = frame;
                Session["model"] = m;
                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }


        public ActionResult InsertConfirmed()
        {
            FrameEditModel m = Session["model"] as FrameEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.InsertNew();

            Session["model"] = null;
            return RedirectToAction("Index");
        }
    }
}
