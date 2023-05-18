using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi.Model;

namespace ArmsWeb.Controllers
{
    public class EicsTypeChangeController : Controller
    {
        //
        // GET: /EicsTypeChange/

        public ActionResult Index(string plantcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            EicsTypeChangeModel m = Session["model"] as EicsTypeChangeModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "EicsTypeChange";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new EicsTypeChangeModel(code);
                Session["model"] = m;
            }
            return View(m);
        }

        [HttpPost]

        public ActionResult Index(string btnMag, string btnType, string btnSelect, string barcode, string searchStr, string typeList)
        {
            EicsTypeChangeModel m = Session["model"] as EicsTypeChangeModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "EicsTypeChangeModelが見つかりません" });
            }

            if (!String.IsNullOrEmpty(btnMag) || !String.IsNullOrEmpty(barcode))
            {
                string magno;
                int seqNo = 0;

                string[] elms = barcode.Split(' ');
                if (elms.Length == 2 && barcode.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno = elms[1];
                }
                else if (elms.Length == 4 && barcode.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno = elms[1];
                    //seqNo = int.Parse(elms[2]);
                    //magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqNo);
                }
                else if (barcode.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    magno = barcode.Replace(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE, "");
                }
                else
                {
                    TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    return View(m);
                }

                try
                {
                    m.GetType(magno);
                }
                catch
                {
                    return RedirectToAction("Message", "Home", new { msg = "マガジン情報が見つかりません" });
                }

                return RedirectToAction("ChangeConfirm", m);
               
            }

            else if (!String.IsNullOrEmpty(btnType) || !String.IsNullOrEmpty(searchStr))
            {
                m.GetSearchResultTypeList(searchStr);
                return View(m);
            }

            //else if (btnSubmit == "btnSelect")
            else if (!String.IsNullOrEmpty(btnSelect))
            {
                m.NewType = typeList;
                return RedirectToAction("ChangeConfirm", m);
            }

            
            return View(m);
        }

        public ActionResult ChangeConfirm(string btnChange)
        {
            EicsTypeChangeModel m = Session["model"] as EicsTypeChangeModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "EicsTypeChangeModelが見つかりません" });
            }
            else if (m.NewType == String.Empty)
            {
                return RedirectToAction("Message", "Home", new { msg = "タイプ情報が見つかりません" });
            }

            if (string.IsNullOrEmpty(m.PlantCd) == false)
            {
                m.OldType = ArmsApi.Model.MachineInfo.GetCurrentEICSTypeCd(m.Mac);

                if (string.IsNullOrEmpty(m.OldType) == true)
                {
                    return RedirectToAction("Message", "Home", new { msg = "EICS設定からタイプが取得できません" });
                }

            }

            if (!String.IsNullOrEmpty(btnChange))
            {
                if (m.UpdateEicsType() == false)
                {
                    return RedirectToAction("Message", "Home", new { msg = "EICS設定の更新に失敗しました" });
                }
                else
                {
                    return RedirectToAction("Message", "Home", new { msg = "設定の更新が完了しました" });
                }
            }
            
            return View(m);
        }

    }
}
