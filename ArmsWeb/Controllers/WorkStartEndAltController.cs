using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class WorkStartEndAltController : Controller
    {
        //
        // GET: /WorkStartEnd/
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

            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "WorkStartEndAlt";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WorkStartEndAltModel(code);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "WorkStartEndAlt";
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

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo, FormCollection fc)
        {
            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;

            //開始完了の場合、マガジン入れ替えに伴う1マガジン毎の読み込み判定はスキップする
            if (!string.IsNullOrEmpty(txtMagNo))
            {
                string raw = txtMagNo;
                string[] elms = raw.Split(' ');

                string magno;
                int seqNo = 0;
                #region バーコードヘッダー部判定

                if (elms.Length == 2 && raw.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno = elms[1];
                }
                else if (elms.Length == 2 && raw.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    magno = elms[1];
                }
                else if (elms.Length == 4 && raw.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno = elms[1];
                    seqNo = int.Parse(elms[2]);
                    magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqNo);
                }
                else
                {
                    TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    return View(m);
                }
                #endregion

                ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno);

                #region マガジンデータ存在チェック

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
                #endregion

                string msg;
                bool isOk = m.CheckBeforeStart(mag, out msg);

                if (!isOk)
                {
                    TempData["AlertMsg"] = msg;
                    return View(m);
                }

                m.LastReadMag = mag;

                //開始完了の場合は仮想マガジンも処理しない
                m.AddMagazine(mag);
            }

            Session["model"] = m;

            //分割開始対応
            if (WorkStartEndAltModel.IsDoubleToSingleProcess(m.LastReadMag) == true)
            {
                return RedirectToAction("FindOther");
            }
            else if (m.Mac.MagazineChgFg == true)
            {
                return RedirectToAction("InputUnloaderMag");
            }
            else
            {
                return View(m);
            }
        }

        public ActionResult FindOther()
        {
            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FindOther(string txtMagNo, FormCollection fc)
        {
            #region FindOTher

            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

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
                    TempData["AlertMsg"] = "ロット情報が存在しません";
                    return View(m);
                }

                if (m.FindOtherMag(m.LastReadMag).MagazineNo != mag.MagazineNo)
                {
                    TempData["AlertMsg"] = "分割マガジンが一致しません";
                    return View(m);
                }

                string msg;
                bool isOk = m.CheckBeforeStart(mag, out msg);

                if (!isOk)
                {
                    TempData["AlertMsg"] = msg;
                    return View(m);
                }

                m.LastReadMag = mag;
                m.MagList.Add(mag);
            }
            #endregion

            Session["model"] = m;
            return RedirectToAction("InputUnloaderMag");
        }
        
        public ActionResult InputUnloaderMag(string txtMagNo, FormCollection fc)
        {
            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;
            if (m == null) return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });

            if (!string.IsNullOrEmpty(txtMagNo))
            {
                string raw = txtMagNo;
                string[] elms = raw.Split(' ');

                string magno;
                int seqNo = 0;

                #region バーコードヘッダー部判定
                if (elms.Length == 2 && raw.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT))
                {
                    magno = elms[1];
                }
                else if (elms.Length == 2 && raw.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_MAGAZINE))
                {
                    magno = elms[1];
                }

                else if (elms.Length == 4 && raw.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                {
                    magno = elms[1];
                    seqNo = int.Parse(elms[2]);
                    magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqNo);

                    #region マガジン統合作業時のバグ対応
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

                    if (WorkStartEndAltModel.IsDoubleToSingleProcess(mag) == true)
                    {
                        magno = elms[1];
                    }
                    #endregion
                }
                else
                {
                    TempData["AlertMsg"] = "マガジンバーコードを読み込んでください";
                    return View(m);
                }
                #endregion

                m.UnloaderMagNo = magno;
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Confirm()
        {
            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult FormInfo() //Submit()
        {
            WorkStartEndAltModel m = Session["model"] as WorkStartEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                List<string> msg;
                bool success = m.WorkEnd(out msg);

                if (!success)
                {
                    string rawmsg = string.Join(" ", msg.ToArray());
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + rawmsg });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "開始完了登録で予期せぬエラー：" + ex.Message });
            }

			if (m.NeedInspectionWhenCompleteLotList.Count > 0)
			{
				string messageStr = "下記ﾛｯﾄは状態検査が必要です。\r\n";

				foreach (string targetLot in m.NeedInspectionWhenCompleteLotList.Distinct())
				{
					messageStr += string.Format("{0}\r\n", targetLot);
				}

				return RedirectToAction("Message", "Home", new { msg = messageStr });
			}



            // 帳票入力リンク対応　juniwatanabe

            try
            {
                m.CurrentProcess = ArmsApi.Model.Process.GetNowProcess(m.MagList[0].NascaLotNO);
                bool success = ArmsApi.Model.FORMS.ProccessForms.UpdateMacInfo(m.TypeCd, m.MagList[0].NascaLotNO, m.CurrentProcess.ProcNo, m.PlantCd, m.Mac.MacNo, m.EmpCd);

                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "帳票開始情報がアップデートできませんでした。" });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "帳票開始情報がアップデートで予期せぬエラー：" + ex.Message });
            }

            Session["empcd"] = "";
            // return RedirectToAction("Form", "Home", new { msg = messageStr });

            Session["model"] = m;
            return View(m);
        }
    }
}
