using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi.Model;

namespace ArmsWeb.Controllers
{
    public class LotViewController : Controller
    {
        //
        // GET: /LotView/
        public ActionResult Index(string magno)
        {
            if (!string.IsNullOrEmpty(magno))
            {

                string magno2;
                int seqNo = 0;

                string[] elms = magno.Split(' ');

                //////////////////////////////////////////
                // 照明合理化マガジン（ヘッダーなし）対応
                // 2022.03.03 Junichi Watanabe
                if (elms.Length == 1)
                {
                    elms = new string[] { "C30", magno };
                    magno = "C30 " + magno;
                }
                //////////////////////////////////////////

                if (elms.Length == 2 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno2 = elms[1];
                }
                else if (elms.Length == 4 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno2 = elms[1];
                    seqNo = int.Parse(elms[2]);
                    magno2 = ArmsApi.Model.Order.NascaLotToMagLot(magno2, seqNo);
                }
                else if (magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    magno2 = magno.Replace(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE, "");
                }
                else
                {
                    //20220701 MOD START
                    //TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    TempData["AlertMsg"] = "マガジンバーコード/ロット番号を読み込んでください";
                    //20220701 MOD END
                    return View();
                }

                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno2);

                //20220701 ADD START
                if (mag == null)
                {
                    //マガジン番号で取得できない場合、入力されたコードをロット番号として再度検索
                    ArmsApi.Model.Magazine mag2 = ArmsApi.Model.Magazine.GetMagazine(magno2);
                    if (mag2 != null)
                    {
                        mag = ArmsApi.Model.Magazine.GetCurrent(mag2.MagazineNo);
                    }
                }
                //20220701 ADD END

                if (mag == null)
                {
                    if (seqNo >= 1)
                    {
                        seqNo = 0;
                        magno2 = ArmsApi.Model.Order.MagLotToNascaLot(magno2);
                        mag = ArmsApi.Model.Magazine.GetCurrent(magno2);
                        if (mag == null)
                        {
                            TempData["AlertMsg"] = "ロット情報が存在しません";
                            return View();
                        }
                    }

                    else
                    {
                        TempData["AlertMsg"] = "ロット情報が存在しません";
                        return View();
                    }
                }

                ArmsWeb.Models.LotViewModel m = new Models.LotViewModel(mag.MagazineNo);
                Session["model"] = m;
                return View(m);
            }
            else
            {
                return View();
            }
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }


        public ActionResult FindOther()
        {
            LotViewModel m = Session["model"] as LotViewModel;
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FindOther(string magno)
        {
            LotViewModel m = Session["model"] as LotViewModel;

            if (!string.IsNullOrEmpty(magno))
            {
                string magno2;
                int seqNo = 0;

                string[] elms = magno.Split(' ');

                // 照明合理化マガジン（ヘッダーなし）対応
                // 2022.03.03 Junichi Watanabe
                if (elms.Length == 1)
                {
                    elms = new string[] { "C30", magno };
                    magno = "C30 " + magno;
                }

                if (elms.Length == 2 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno2 = elms[1];
                }
                else if (elms.Length == 4 && magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno2 = elms[1];
                    seqNo = int.Parse(elms[2]);
                    magno2 = ArmsApi.Model.Order.NascaLotToMagLot(magno2, seqNo);
                }
                else if (magno.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    magno2 = magno.Replace(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE, "");
                }
                else
                {
                    TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    return View();
                }

                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno2);

                if (mag == null)
                {
                    TempData["AlertMsg"] = "ロット情報が存在しません";
                    return View();
                }

                if (m == null)
                {
                    m = new Models.LotViewModel(mag.MagazineNo);
                }
                else if (m.IsMatched == true)
                {
                    m = new Models.LotViewModel(mag.MagazineNo);
                }
                else
                {
                    Magazine other = m.FindOtherMag(m.Magazine);
                    if (other == null)
                    {
                        TempData["AlertMsg"] = m.Magazine + "の分割マガジンが見つかりません";
                        return View(m);
                    }
                    else if (m.MagNo == magno2)
                    {
                        Session["model"] = m;
                        return View(m);
                    }
                    else if (other.MagazineNo != magno2)
                    {
                        Session["model"] = m;
                        TempData["AlertMsg"] = "分割マガジンが一致しません";
                        return View(m);
                    }
                    else if (other.MagazineNo == magno2)
                    {
                        m.IsMatched = true;
                        Session["model"] = m;
                        return View(m);

                    }
                }

                Session["model"] = m;
                return View(m);
            }
            else
            {
                return View();
            }
        }

        public ActionResult DefInput(bool isInspection)
        {
            LotViewModel m = Session["model"] as LotViewModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            AsmLot lot = AsmLot.GetAsmLot(m.LotNo);
            Magazine mag = Magazine.GetCurrent(m.MagNo);

            Order ord = Order.GetNextMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);
            if (ord == null)
            {
                ord = Order.GetMagazineOrder(mag.NascaLotNO, mag.NowCompProcess);

                if (ord == null)
                {
                    TempData["AlertMsg"] = "作業中の指図がありません";
                    return RedirectToAction("Index");
                }
            }

            DefectEditModel dm = new DefectEditModel(ord, lot.TypeCd);
            Session["model"] = dm;
            return RedirectToAction("InputQty", "Defect", new { orderindex = 0, isInspection = isInspection });
        }

        public ActionResult PsTesterLink()
        {
            LotViewModel m = Session["model"] as LotViewModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            List<KeyValuePair<int, string>?> cfglst = ArmsApi.Config.Settings.PSTesterLinkInfo;
            if (cfglst == null || cfglst.Count == 0)
            {
                TempData["AlertMsg"] = "強度試験用Configが設定されていません";
                return RedirectToAction("Index");
            }

            KeyValuePair<int, string>? cfg = cfglst.Where(c => c.Value.Value != null).First();
            if (cfg == null)
            {
                TempData["AlertMsg"] = "強度試験用Configが設定されていません";
                return RedirectToAction("Index");
            }

            m.WBOrder = Order.GetMagazineOrder(m.LotNo, cfg.Value.Key);
            if (m.WBOrder == null)
            {
                TempData["AlertMsg"] = "WB工程の実績が存在しません";
                return RedirectToAction("Index");
            }

            m.WBMachine = MachineInfo.GetMachine(m.WBOrder.MacNo);

            Session["model"] = m;
            return View(m);
        }


        public ActionResult PsTesterLinkDone()
        {
            LotViewModel m = Session["model"] as LotViewModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.Magazine.WritePSTesterFile();

            DefectEditModel dm = new DefectEditModel(m.WBOrder, m.Lot.TypeCd);
            Session["model"] = dm;
            return RedirectToAction("InputQty", "Defect", new { orderindex = 0, isInspection = false });
        }

        public ActionResult BdTesterLinkDone()
        {
            LotViewModel m = Session["model"] as LotViewModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.Magazine.WriteBDTesterFile();

            DefectEditModel dm = new DefectEditModel(m.WBOrder, m.Lot.TypeCd);
            Session["model"] = dm;
            return RedirectToAction("InputQty", "Defect", new { orderindex = 0, isInspection = false });
        }

        public ActionResult FilmTesterLinkDone()
        {
            LotViewModel m = Session["model"] as LotViewModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.Magazine.WriteFilmTesterFile();

            DefectEditModel dm = new DefectEditModel(m.WBOrder, m.Lot.TypeCd);
            Session["model"] = dm;
            return RedirectToAction("InputQty", "Defect", new { orderindex = 0, isInspection = false });
        }
    }
}
