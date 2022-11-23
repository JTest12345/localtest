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
                //20220701 ADD START
                if (!string.IsNullOrEmpty(ArmsApi.Config.Settings.PlantCDLabelCompareDC))
                {
                    Session["plantcd"] = ArmsApi.Config.Settings.PlantCDLabelCompareDC;
                }
                //20220701 ADD END

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
                return RedirectToAction("InputLotNo", "BlendLabelCompare");
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
                    //富士情報　変更　start
                    TempData["AlertMsg"] = "既に読み込み済みのロット管理表ロットです";
                    //TempData["AlertMsg"] = "既に読み込み済みの構成ロット（子ロット）です";
                    //富士情報　変更　end
                    return View(m);
                }

                m.AsmLotList.Add(asmlotno);
            }
            else
            {
                //富士情報　変更　start
                TempData["AlertMsg"] = "ロット管理表ロットを読み込んでください";
                //TempData["AlertMsg"] = "構成ロット（子ロット）を読み込んでください";
                //富士情報　変更　end
                return View(m);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            //キャンセル時は設備番号読み込み画面へ遷移 -> 社員入力画面へ遷移
            Session["model"] = null;
            //20220701 MOD START
            //Session["plantcd"] = null;
            if (string.IsNullOrEmpty(ArmsApi.Config.Settings.PlantCDLabelCompareDC))
            {
                Session["plantcd"] = null;
            }
            //20220701 MOD END
            Session["empcd"] = null;
            Session["lotno"] = null;

            Session["redirectController"] = "BlendLabelCompare";
            Session["redirectAction"] = "Index";
            //20220701 MOD START
            //return RedirectToAction("InputPlantCd", "Home");
            if (string.IsNullOrEmpty(ArmsApi.Config.Settings.PlantCDLabelCompareDC))
            {
                return RedirectToAction("InputPlantCd", "Home");
            }
            else
            {
                return RedirectToAction("InputEmpCd", "Home");
            }
            //20220701 MOD END
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
        public ActionResult InputLotNo()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputLotNo(string lotno)
        {
            //string originallotno = lotno;

            //string[] elms = lotno.Split(' ');
            //if (elms.Length >= 2)
            //{
            //    lotno = elms[1];
            //}
            //elms = originallotno.Split(',');
            //if (elms.Length == 5)
            //{
            //    //ライフラベルを想定
            //    //ロットNo,型番,数量,日時,
            //    lotno = elms[0];
            //}

            Session["lotno"] = lotno;

            //string action = (string)Session["redirectAction"];
            //string controller = (string)Session["redirectController"];

            //if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
            //{
            //    return RedirectToAction(action, controller);
            //}
            //else
            //{
                return RedirectToAction("Index");
            //}
        }
    }
}