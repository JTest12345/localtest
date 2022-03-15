using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class DefectSubStController : Controller
    {
        //
        // GET: /Defect/
        public ActionResult Index(string plantcd, string empcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            if (string.IsNullOrEmpty(empcd) == false)
            {
                Session["empcd"] = empcd;
            }

            DefectSubStEditModel m = Session["model"] as DefectSubStEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "DefectSubSt";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new DefectSubStEditModel(code);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "DefectSubSt";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = code;
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

        public ActionResult Select(int? orderindex)
        {
            DefectSubStEditModel m = Session["model"] as DefectSubStEditModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (orderindex.HasValue)
            {
                m.EditTarget = m.Orders[orderindex.Value];
            }

            if (m.EditTarget == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "検査対象の指図が選択されていません" });
            }

            if (m.TypeCd == null)
            {
                m.TypeCd = ArmsApi.Model.AsmLot.GetAsmLot(m.EditTarget.NascaLotNo).TypeCd;
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult InputDef(string classcd, string causecd, string defectcd, string qty)
        {
            DefectSubStEditModel m = Session["model"] as DefectSubStEditModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.CurrentDefItem = m.GetDefItems().Where(d => d.CauseCd == causecd && d.ClassCd == classcd && d.DefectCd == defectcd).FirstOrDefault();
            if (m.CurrentDefItem == null)
            {
                return RedirectToAction("Select");
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputDef(string classcd, string causecd, string defectcd, string qty, string address, string unit)
        {
            DefectSubStEditModel m = Session["model"] as DefectSubStEditModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            int ct;
            if (int.TryParse(qty, out ct) == false)
            {
                TempData["AlertMsg"] = "不良数は数字を入力してください";
                return View(m);
            }

            ArmsApi.Model.DefItem[] defs = m.GetDefItems();
            ArmsApi.Model.DefItem di = defs.Where(d => d.CauseCd == causecd && d.ClassCd == classcd && d.DefectCd == defectcd).FirstOrDefault();
            if (di == null)
            {
                TempData["AlertMsg"] = "不良明細が見つかりません";
                return View(m);
            }

            di.DefectCt = ct;

            ArmsApi.Model.Defect defect = new ArmsApi.Model.Defect();
            defect.LotNo = m.EditTarget.NascaLotNo;
            defect.DefItems = new List<ArmsApi.Model.DefItem>(defs);
            defect.ProcNo = m.EditTarget.ProcNo;
            defect.MagazineNo = m.EditTarget.InMagazineNo;

            //変更前不良枚数取得
            ArmsApi.Model.Magazine mags = ArmsApi.Model.Magazine.GetMagazine(defect.LotNo);
            m.FailureBdQty = defect.GetDefectCtSubSt(defect.LotNo, defect.ProcNo);
            mags.FrameQty += m.FailureBdQty;

            //入力数量チェック
            if (!defect.CheckDefectSubSt(mags.FrameQty))
            {
                TempData["AlertMsg"] = "不良枚数が基板枚数を超えています:";
                return View(m);
            }

            //EICSのWB不良アドレス更新
            try
            {
                m.UpdateEicsWBAddress(di, address, unit);
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = "更新失敗:" + ex.Message;
                return View(m);
            }

            //defect更新
            defect.DeleteInsertSubSt(mags.FrameQty);

            Session["empcd"] = "";
            return View("Select", m);
        }

        public ActionResult ReturnCutBlend()
        {
            Session["model"] = Session["cutBlendModel"];
            return RedirectToAction("Finish", "CutBlend");
        }
    }
}
