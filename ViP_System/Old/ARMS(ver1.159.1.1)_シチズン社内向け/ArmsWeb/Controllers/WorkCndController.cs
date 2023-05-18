using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class WorkCndController : Controller
    {
        //
        // GET: /WorkCnd/

        public ActionResult Index(string plantcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            WorkCndEditModel m = Session["model"] as WorkCndEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "WorkCnd";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WorkCndEditModel(code);
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
        public ActionResult Remove(string cndindex)
        {
            WorkCndEditModel m = Session["model"] as WorkCndEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (cndindex == null)
            {
                return View("Index", m);
            }


            m.EditTarget = m.Conditions[int.Parse(cndindex)];
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
            WorkCndEditModel m = Session["model"] as WorkCndEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.RemoveWorkCnd(m.EditTarget, DateTime.Now);

            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult InsertNew()
        {
            WorkCndEditModel m = Session["model"] as WorkCndEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            return View(m);
        }

        public ActionResult InsertConfirm(string barcode, string condindex)
        {
            WorkCndEditModel m = Session["model"] as WorkCndEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                //2014.11.7 41マスタ移管2次改修でリスト選択式に変更
                if (condindex == null)
                {
                    return RedirectToAction("Message", "Home", new { msg = "条件が選択されていません" });
                }
                ArmsApi.Model.WorkCondition cnd = m.ParseWorkCond(barcode, int.Parse(condindex));
                m.EditTarget = cnd;
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
            WorkCndEditModel m = Session["model"] as WorkCndEditModel;
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
