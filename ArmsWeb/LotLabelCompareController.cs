using ArmsApi.Model;
using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class LotLabelCompareController : Controller
    {
        public ActionResult Index()
        {
            LotLabelCompareModel m = Session["model"] as LotLabelCompareModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "LotLabelCompare";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new LotLabelCompareModel(plantcd);
                Session["model"] = m;
            }

            string empcd = (string)Session["empcd"];
            if (string.IsNullOrEmpty(empcd))
            {
                Session["redirectController"] = "LotLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputEmpCd", "Home");
            }

            m.EmpCd = empcd;
            Session["model"] = m;

            string magno = (string)Session["magazineno"];
            if (string.IsNullOrEmpty(magno))
            {
                //マガジン読み込み
                Session["redirectController"] = "LotLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputMagazineNo", "Home");
            }

            #region マガジンデータ存在チェック
            ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno);

            if (mag == null)
            {
                TempData["AlertMsg"] = "ロット情報が存在しません";
                //マガジン読み込み
                Session["redirectController"] = "LotLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputMagazineNo", "Home");
            }
            #endregion

            string msg;
            bool isOk = m.CheckBeforeStart(mag, out msg);

            if (!isOk)
            {
                TempData["AlertMsg"] = msg;
                //マガジン読み込み
                Session["redirectController"] = "LotLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputMagazineNo", "Home");
            }

            m.MagNo = magno;
            Session["model"] = m;

            string lotno = (string)Session["lotno"];
            if (string.IsNullOrEmpty(lotno))
            {
                //ロット管理表読み込み
                Session["redirectController"] = "LotLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputLotNo", "Home");
            }
            m.LotNo = lotno;
            m.Compare(mag);
            if (!m.IsOK)
            {
                TempData["AlertMsg"] = m.Msg;
                Session["redirectController"] = "LotLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputLotNo", "Home");
            }

            Session["model"] = m;

            return View(m);
        }

        public ActionResult CancelEdit()
        {
            //キャンセル時はマガジン読み込み画面へ遷移
            Session["magno"] = null;
            Session["lotno"] = null;

            Session["redirectController"] = "LotLabelCompare";
            Session["redirectAction"] = "Index";
            return RedirectToAction("InputMagazineNo", "Home");
        }

        public ActionResult Submit()
        {
            LotLabelCompareModel m = Session["model"] as LotLabelCompareModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "LotLabelCompareModelが見つかりません" });
            }

            ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(m.MagNo);
            m.Compare(mag);
            if (!m.IsOK)
            {
                TempData["AlertMsg"] = m.Msg;
                return View("index",m);
            }

            //実績登録
            string msg;
                bool isOk = m.EndMag(mag, out msg, false);

            if (!isOk)
            {
                return RedirectToAction("Message", "Home", new { msg = msg });
            }

            Session["model"] = m;
            return View("Finish", m);
        }
    }
}