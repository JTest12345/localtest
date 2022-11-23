using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi.Model;

namespace ArmsWeb.Controllers
{
    public class CutBlendController : Controller
    {
		//
		// GET: /CutBlend/

		public ActionResult Index(string plantcd, string empcd)
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
				if(string.IsNullOrWhiteSpace(plantcd))
	                plantcd = (string)Session["plantcd"];

                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "CutBlend";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new CutBlendModel(plantcd);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
				if(string.IsNullOrWhiteSpace(empcd))
	                empcd = (string)Session["empcd"];

                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "CutBlend";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = empcd;
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }


        public ActionResult PreInputDefect()
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "CutBlend";
                    Session["redirectAction"] = "PreInputDefect";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new CutBlendModel(plantcd);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string empcd = (string)Session["empcd"];
                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "CutBlend";
                    Session["redirectAction"] = "PreInputDefect";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = empcd;
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PreInputDefect(string blenditem)
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (blenditem == null)
            {
                TempData["AlertMsg"] = "入力対象が選択されていません";
                return RedirectToAction("Index", m);
            }


            int idx = int.Parse(blenditem);
            CutBlend cb = m.BlendList[idx];


            AsmLot lot = AsmLot.GetAsmLot(cb.LotNo);
            Process[] flow = Process.GetWorkFlow(lot.TypeCd);

            Order[] orders = Order.GetOrder(lot.NascaLotNo);

            //IsCutPreInputDefの場合は指図の工程情報は使われないので先頭を渡す
            //TnMagの関連はこの時点で外れているためNowCompProcessは使用できない
            DefectEditModel dm = new DefectEditModel(orders[0], lot.TypeCd);
            dm.IsCutPreInputDef = true;
            Session["model"] = dm;
            dm.EditTarget = orders[0];
            return RedirectToAction("Select", "Defect", new { orderindex = 0, isInspection = false });
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult Confirm(string[] blenditem)
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (blenditem == null)
            {
                TempData["AlertMsg"] = "ブレンド対象が選択されていません";
                return RedirectToAction("Index", m);
            }

            if (blenditem.Length > ArmsApi.Config.Settings.CutBlendMagazineCt)
            {
                TempData["AlertMsg"] = "カットブレンド最大数を超えています";
                return RedirectToAction("Index", m);
            }


            m.CurrentBlend.Clear();
            foreach (string str in blenditem)
            {
                int idx = int.Parse(str);

                m.CurrentBlend.Add(m.BlendList[idx]);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Confirmed()
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                m.FinishBlend();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "カット完了エラー：" + ex.Message });
            }

            Session["model"] = m;
            return View("Finish", m);
        }

        public ActionResult Finish()
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["empcd"] = "";
            Session["model"] = m;
            return View("Finish", m);
        }

        public ActionResult PrintLabel()
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                CutBlend.PrintCutLabel(m.CurrentBlend.ToArray(), m.BlendLotNo);
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = "ラベル印字失敗:" + ex.Message;
                return View("Finish", m);
            }

            TempData["AlertMsg"] = "ラベル印字完了";
            return View("Finish", m);
        }

        public ActionResult InputDefect()
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Order ord = Order.GetMagazineOrder(m.BlendLotNo,m.ProcNo);
            AsmLot lot = AsmLot.GetAsmLot(m.CurrentBlend[0].LotNo);
            DefectEditModel defmodel = new DefectEditModel(ord, lot.TypeCd);
            defmodel.EmpCd = m.EmpCd;

            Session["model"] = defmodel;

            //不良入力画面に遷移した後、このモデルを使用して当画面に戻る
            Session["cutBlendModel"] = m;

            return RedirectToAction("InputQty", "Defect", new { orderindex = 0, isInspection=false});
        }
    }
}
