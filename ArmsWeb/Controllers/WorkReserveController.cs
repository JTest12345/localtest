using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class WorkReserveController : Controller
    {
        //
        // GET: /WorkReserve/
        public ActionResult Index()
        {
            WorkStartModel m = Session["model"] as WorkStartModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "WorkReserve";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WorkStartModel(plantcd);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string empcd = (string)Session["empcd"];
                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "WorkReserve";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = empcd;
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo, FormCollection fc)
        {
            WorkStartModel m = Session["model"] as WorkStartModel;

            if (!string.IsNullOrEmpty(txtMagNo))
            {
                string magno;
                int seqNo = 0;

                string[] elms = txtMagNo.Split(' ');

                if (elms.Length == 2 && txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno = elms[1];
                }
                else if (elms.Length == 4 && txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno = elms[1];
                    seqNo = int.Parse(elms[2]);
                    magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqNo);
                }
                else if (txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE)) 
                {
                    magno = txtMagNo.Replace(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE, "");
                }
                else
                {
                    TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    return View(m);
                }

                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno);

                if (mag == null)
                {
                    if (seqNo >= 1)
                    {
                        seqNo = 0;
                        magno = ArmsApi.Model.Order.MagLotToNascaLot(magno);
                        mag = ArmsApi.Model.Magazine.GetCurrent(magno);
                        if (mag == null)
                        {
                            TempData["AlertMsg"] = "ロット情報が存在しません";
                            return View(m);
                        }
                    }

                    else
                    {
                        TempData["AlertMsg"] = "ロット情報が存在しません";
                        return View(m);
                    }
                }

                string msg;
                bool isOk = m.CheckBeforeStart(mag, out msg);

                if (!isOk)
                {
                    TempData["AlertMsg"] = msg;
                    return View(m);
                }

                //重複読込チェック
                if (m.MagList.Where(em => em.MagazineNo == magno).Count() >= 1)
                {
                    TempData["AlertMsg"] = "既に読み込み済みのマガジンです";
                    return View(m);
                }

                ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    TempData["AlertMsg"] = "ロット情報が存在しません";
                    return View(m);
                }

                ArmsApi.Model.Process p = ArmsApi.Model.Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    TempData["AlertMsg"] = "工程情報が存在しません";
                    return View(m);
                }

                ArmsApi.Model.ReservedOrder ro = new ArmsApi.Model.ReservedOrder();
                ro.LotNo = mag.NascaLotNO;
                ro.InMagazineNo = mag.MagazineNo;
                ro.MacNo = m.Mac.MacNo;
                ro.ProcNo = p.ProcNo;
                ro.ReservedDt = DateTime.Now;
                ro.DeleteInsert();
            }
            Session["model"] = m;
            return View(m);
        }


        public ActionResult Confirm()
        {
            WorkStartModel m = Session["model"] as WorkStartModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "セッション情報が不正です" });
            }

            ArmsApi.Model.ReservedOrder[] ros = ArmsApi.Model.ReservedOrder.GetReserveList(m.Mac.MacNo);
            m.MagList.Clear();
			m.NeedInspectionWhenStartLotList.Clear();
			m.DieShearSamplingLotList.Clear();

            foreach (ArmsApi.Model.ReservedOrder ro in ros)
            {
                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(ro.InMagazineNo);
                m.MagList.Add(mag);
            }
            Session["model"] = m;
            return RedirectToAction("Confirm", "WorkStart");
        }

        public ActionResult Clear()
        {
            WorkStartModel m = Session["model"] as WorkStartModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "セッション情報が不正です" });
            }
            return View(m);
        }

        public ActionResult ClearConfirm()
        {
            WorkStartModel m = Session["model"] as WorkStartModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "セッション情報が不正です" });
            }

            ArmsApi.Model.ReservedOrder.ClearReserveList(m.Mac.MacNo);
            return RedirectToAction("Message", "Home", new { msg = "装置の予約登録を全て解除しました" });
        }
    }
}
