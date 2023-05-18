using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class ResinController : Controller
    {
        //
        // GET: /Resin/

        public ActionResult Index(string plantcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            ResinEditModel m = Session["model"] as ResinEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "Resin";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new ResinEditModel(code);
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
        /// </summary>
        /// <param name="matdata"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult Remove(string resinindex)
        {
            ResinEditModel m = Session["model"] as ResinEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }
            if (resinindex == null)
            {
                return View("Index", m);
            }


            m.EditTarget = m.ResinList[int.Parse(resinindex)];
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
            ResinEditModel m = Session["model"] as ResinEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.RemoveResin(m.EditTarget, DateTime.Now);

            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult InsertNew()
        {
            ResinEditModel m = Session["model"] as ResinEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            return View(m);
        }

        public ActionResult InsertConfirm(string barcode)
        {
            ResinEditModel m = Session["model"] as ResinEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                ArmsApi.Model.Resin rsn = m.ParseResin(barcode);
                m.EditTarget = rsn;
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
            ResinEditModel m = Session["model"] as ResinEditModel;
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
