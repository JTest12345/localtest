using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class CutLabelCompareController : Controller
    {
        //
        // GET: /CutLabelCompare/
        public ActionResult Index()
        {
            CutLabelCompareModel m = Session["model"] as CutLabelCompareModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "CutLabelCompare";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new CutLabelCompareModel(plantcd);
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtLabel, FormCollection fc)
        {
            CutLabelCompareModel m = Session["model"] as CutLabelCompareModel;
            if (!string.IsNullOrEmpty(txtLabel))
            {
                m.Compare(txtLabel);
            }
            Session["model"] = m;
            return View(m);
        }

    }
}
