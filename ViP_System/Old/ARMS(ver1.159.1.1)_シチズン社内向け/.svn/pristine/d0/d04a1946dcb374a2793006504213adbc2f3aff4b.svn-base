using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class MapDicingBladeController : Controller
    {
        //
        // GET: /MapDicingBlade/
        public ActionResult Index()
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "MapDicingBlade";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new MaterialEditModel(plantcd);

                m.EditTarget = new List<ArmsApi.Model.Material>();

                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult InsertNew(string placecd)
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (string.IsNullOrEmpty(placecd))
            {
                return RedirectToAction("Message", "Home", new { msg = "取りつけ位置が指定されていません" });
            }
            m.EditTarget = new List<ArmsApi.Model.Material>();

            m.MapDicingBladePlaceCd = placecd;

            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertNew(string placecd, string barcode)
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                if (string.IsNullOrEmpty(barcode) == false)
                {
                    ArmsApi.Model.Material mat = m.ParseMatBarCode(barcode);
                    string errMsg;
                    if (ArmsApi.Model.WorkChecker.IsErrorBeforeInputMat(mat, m.Mac, out errMsg))
                    {
                        return RedirectToAction("Message", "Home", new { msg = errMsg });
                    }
                    m.EditTarget = new List<ArmsApi.Model.Material>();
                    m.EditTarget.Add(mat);
                }

                if (string.IsNullOrEmpty(placecd) == false)
                {
                    m.MapDicingBladePlaceCd = placecd;
                }

                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }
    }
}
