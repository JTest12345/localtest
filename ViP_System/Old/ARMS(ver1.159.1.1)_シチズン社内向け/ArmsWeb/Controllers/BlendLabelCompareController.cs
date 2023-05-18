using ArmsApi.Model;
using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class BlendLabelCompareController : Controller
    {
        public ActionResult Index()
        {
            BlendLabelCompareModel m = Session["model"] as BlendLabelCompareModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "BlendLabelCompare";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new BlendLabelCompareModel(plantcd);
                Session["model"] = m;
            }

            string empcd = (string)Session["empcd"];
            if (string.IsNullOrEmpty(empcd))
            {
                Session["redirectController"] = "BlendLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputEmpCd", "Home");
            }

            m.EmpCd = empcd;
            Session["model"] = m;

            string blendlotno = (string)Session["lotno"];
            if (string.IsNullOrEmpty(blendlotno))
            {
                //親ロット読み込み
                Session["redirectController"] = "BlendLabelCompare";
                Session["redirectAction"] = "Index";
                return RedirectToAction("InputLotNo", "Home");
            }
            m.BlendLotNo = blendlotno;

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtAsmLotNo, FormCollection fc)
        {
            BlendLabelCompareModel m = Session["model"] as BlendLabelCompareModel;

            //構成ロット（子ロット全て）読み込み
            if (!string.IsNullOrEmpty(txtAsmLotNo))
            {
                string asmlotno = txtAsmLotNo;
                string[] elms = asmlotno.Split(' ');
                if (elms.Length >= 2)
                {
                    //13 ロット番号
                    asmlotno = elms[1];
                }
                elms = asmlotno.Split(',');
                if (elms.Length >= 4)
                {
                    //ロットNo,型番,数量,日時
                    asmlotno = elms[0];
                }

                //重複読込チェック
                if (m.AsmLotList.Where(dm => dm == asmlotno).Count() >= 1)
                {
                    TempData["AlertMsg"] = "既に読み込み済みの構成ロット（子ロット）です";
                    return View(m);
                }

                m.AsmLotList.Add(asmlotno);
            }
            else
            {
                TempData["AlertMsg"] = "構成ロット（子ロット）を読み込んでください";
                return View(m);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            //キャンセル時は設備番号読み込み画面へ遷移
            Session["model"] = null;
            Session["plantcd"] = null;
            Session["empcd"] = null;
            Session["lotno"] = null;

            Session["redirectController"] = "BlendLabelCompare";
            Session["redirectAction"] = "Index";
            return RedirectToAction("InputPlantCd", "Home");
        }

        public ActionResult Confirm()
        {
            BlendLabelCompareModel m = Session["model"] as BlendLabelCompareModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "BlendLabelCompareModelが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Confirmed()
        {
            BlendLabelCompareModel m = Session["model"] as BlendLabelCompareModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "BlendLabelCompareModelが見つかりません" });
            }

            m.Compare();

            if (m.IsOK)
            {
                //実績登録
                string msg;
                bool isOk = m.EndMag(out msg);

                if (!isOk)
                {
                    return RedirectToAction("Message", "Home", new { msg = msg });
                }
            }


            Session["model"] = m;
            return View("Finish", m);
        }

        public ActionResult Finish()
        {
            BlendLabelCompareModel m = Session["model"] as BlendLabelCompareModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "BlendLabelCompareModelが見つかりません" });
            }

            Session["model"] = m;
            return View("Finish", m);
        }
    }
}