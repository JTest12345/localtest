using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class AoiMagController : Controller
    {
        private const char MAP_FRAME_SEPERATOR = ',';
        private const int MAP_FRAME_ELEMENT_CT = 9;

        //
        // GET: /AoiMag/

        public ActionResult Index()
        {
            AoiMagEditModel m = Session["model"] as AoiMagEditModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "AoiMag";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new AoiMagEditModel(plantcd);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string empcd = (string)Session["empcd"];
                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "AoiMag";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = empcd;
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo, FormCollection fc)
        {
            AoiMagEditModel m = Session["model"] as AoiMagEditModel;
            if (!string.IsNullOrEmpty(txtMagNo))
            {
                //品名,,ロット,,数量,GGコード,,,
                string[] values = txtMagNo.Split(MAP_FRAME_SEPERATOR);
                if (values.Length != MAP_FRAME_ELEMENT_CT)
                {
                    TempData["AlertMsg"] = "MAP基板QRコード内容が不正です";
                    return View(m);
                }

                ArmsApi.Model.Magazine mag = new ArmsApi.Model.Magazine();
                mag.MagazineNo = txtMagNo.Trim() + "_" + DateTime.Now.Ticks.ToString();

                string msg;
                if (m.IsErrorBeforeStart(mag, out msg) == true)
                {
                    TempData["AlertMsg"] = msg;
                    return View(m);
                }

                m.MagList.Add(mag);
            }
            Session["model"] = m;
            return View(m);
        }

        public ActionResult Confirm()
        {
            AoiMagEditModel m = Session["model"] as AoiMagEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "セッション情報が不正です" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Submit()
        {
            AoiMagEditModel m = Session["model"] as AoiMagEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "セッション情報が不正です" });
            }

            try
            {
                string msg;
                
                bool success = m.WorkStart(out msg);

                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "開始登録で予期せぬエラー：" + ex.Message });
            }

            return RedirectToAction("Message", "Home", new { msg = "作業を開始しました" });
        }

    }
}
