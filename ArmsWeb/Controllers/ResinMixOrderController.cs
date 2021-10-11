using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class ResinMixOrderController : Controller
    {
        //
        // GET: /ResinMixOrder/

        public ActionResult Index()
        {
            ResinMixOrderModel m = Session["model"] as ResinMixOrderModel;
            if (m == null)
            {
                // マガジンNo読込
                string code = (string)Session["magazineno"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "ResinMixOrder";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputMagazineNo", "Home");
                }

                // 稼働中ロットチェック
                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(code);
                if (mag == null)
                {
                    mag = ArmsApi.Model.Magazine.GetCurrent(code + "_#2");

                    if (mag == null)
                    {
                        TempData["AlertMsg"] = "ロット情報が存在しません";
                        Session["redirectController"] = "ResinMixOrder";
                        Session["redirectAction"] = "Index";
                        return RedirectToAction("InputMagazineNo", "Home");
                    }
                }

                m = new ResinMixOrderModel(code);

                // 次のMD作業用の樹脂グループCD取得チェック
                string errMsg;
                bool isOK = m.GetResinGroupAndMixTypeNm(out errMsg);
                if(isOK == false)
                {
                    TempData["AlertMsg"] = errMsg;
                    Session["redirectController"] = "ResinMixOrder";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputMagazineNo", "Home");
                }

                Session["model"] = m;
            }

            // 社員番号読み込み
            if (string.IsNullOrEmpty(m.MixReqInfo.UpdUserCd))
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "ResinMixOrder";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.MixReqInfo.UpdUserCd = code;
                Session["model"] = m;
            }
            
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(bool priorityfg, bool prefg, string resingrouplist, string lotct, string machinect, string linecd, string expectcompdt, string expectplacecd)
        {
            ResinMixOrderModel m = Session["model"] as ResinMixOrderModel;

            // 入力内容チェック
            int nLotCt;
            if(int.TryParse(lotct, out nLotCt) == false)
            {
                TempData["AlertMsg"] = "投入ロット数が数字ではありません。";
                return View(m);
            }
            int nMachineCt;
            if (int.TryParse(machinect, out nMachineCt) == false)
            {
                TempData["AlertMsg"] = "使用装置台数数が数字ではありません。";
                return View(m);
            }
            DateTime dtExpectCompDt;
            if (DateTime.TryParse(expectcompdt, out dtExpectCompDt) == false)
            {
                TempData["AlertMsg"] = "希望完成時間が日時ではありません。\n(例：2000/1/1 12:00:00)";
                return View(m);
            }

            m.MixReqInfo.PriorityFg = priorityfg;
            m.MixReqInfo.PreFg = prefg;
            m.MixReqInfo.LotCt = nLotCt;
            m.MixReqInfo.MachineCt = nMachineCt;
            m.MixReqInfo.LineCd = linecd;
            m.MixReqInfo.ExpectCompDt = dtExpectCompDt;
            m.MixReqInfo.ExpectPlaceCd = expectplacecd;
            
            Session["model"] = m;

            return RedirectToAction("Confirm");
        }

        public ActionResult CancelEdit()
        {
            ResinMixOrderModel m = Session["model"] as ResinMixOrderModel;
            return RedirectToAction("Index");
        }

        public ActionResult Confirm()
        {
            ResinMixOrderModel m = Session["model"] as ResinMixOrderModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Submit()
        {

            ResinMixOrderModel m = Session["model"] as ResinMixOrderModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                m.InsertMixRequest();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "調合依頼登録登録で予期せぬエラー：" + ex.Message });
            }

            return RedirectToAction("Message", "Home", new { msg = "調合依頼を登録しました" });
        }
    }

}
