using ArmsApi;
using ArmsApi.Model;
using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class PreCutBlendController : Controller
    {
		//
		// GET: /PreCutBlend/

		public ActionResult Index(string empcd)
        {
            PreCutBlendModel m = Session["model"] as PreCutBlendModel;

            if (m == null)
            {
                m = new PreCutBlendModel();
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
				if(string.IsNullOrWhiteSpace(empcd))
	                empcd = (string)Session["empcd"];

                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "PreCutBlend";
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
            PreCutBlendModel m = Session["model"] as PreCutBlendModel;

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

                //重複読込チェック
                if (m.MagList.Where(em => em.MagazineNo == magno).Count() >= 1)
                {
                    TempData["AlertMsg"] = "既に読み込み済みのロットです";
                    return View(m);
                }
                m.MagList.Add(mag);

                //カットブレンド済みチェック
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);

                if (lot.IsBeforeLifeTest == false) 
                {
                    TempData["AlertMsg"] = "仮カットブレンド対象ロットではありません。";
                }

                if (lot.TempCutBlendNo != String.Empty)
                {
                    TempData["AlertMsg"] = "既に仮カットブレンド済みのロットです。組み換えを行う場合はこのまま続行して下さい。";
                }
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Confirm()
        {
            PreCutBlendModel m = Session["model"] as PreCutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "PreCutBlendModelが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Submit()
        {
            PreCutBlendModel m = Session["model"] as PreCutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "PreCutBlendModelが見つかりません" });
            }

            try
            {
                string msg;

                bool success = m.PreCutBlendSubmit(out msg);
                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                }
                
                //bool success = m.WorkStart(out msg);

                //if (!success)
                //{
                //    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                //}
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "仮ブレンド登録で予期せぬエラー：" + ex.Message });
            }

            //if (m.NeedInspectionWhenStart)
            //{
            //    return RedirectToAction("Message", "Home", new { msg = "状態検査が必要です" });
            //}

            //if (m.IsDieShearSampling)
            //{
            //    return RedirectToAction("Message", "Home", new { msg = "作業開始しました。 ダイシェア対象ロットです。" });
            //}

            Session["model"] = m;
            return View("Finish", m);

            //return RedirectToAction("Message", "Home", new { msg = "登録完了しました。" });
        }

        public ActionResult Finish()
        {
            CutBlendModel m = Session["model"] as CutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View("Finish", m);
        }

        public ActionResult PrintLabel()
        {
            PreCutBlendModel m = Session["model"] as PreCutBlendModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                List<AsmLot> lots = new List<AsmLot>();
                foreach (Magazine mag in m.MagList)
                {
                    AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                    lots.Add(lot);
                }

                CutBlend.PrintGaPreCutLabel(lots.ToArray(), lots[0].TempCutBlendNo);
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = "ラベル印字失敗:" + ex.Message;
                return View("Finish", m);
            }

            TempData["AlertMsg"] = "ラベル印字完了";
            return View("Finish", m);
        }

    }
}
