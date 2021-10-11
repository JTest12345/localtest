using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi.Model;

namespace ArmsWeb.Controllers
{
    public class WorkEndAltController : Controller
    {
        //
        // GET: /WorkEnd/
        public ActionResult Index()
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "WorkEndAlt";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WorkEndAltModel(plantcd);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string empcd = (string)Session["empcd"];
                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "WorkEndAlt";
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

        public ActionResult CancelUnloaderMagEdit()
        {
            //Session["model"] = null;
            return RedirectToAction("UnloaderMag");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtMagNo, FormCollection fc)
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;

            if (m.IsNeedMagazineChange == true)
            {
                TempData["AlertMsg"] = "この工程は１マガジンずつ完了する必要があります";
                return View(m);
            }

            if (!string.IsNullOrEmpty(txtMagNo))
            {
                //13 で始まるロット番号コードを読んだ場合はマガジン入れ替え無しと判断する
                bool isSameUnloaderMag = false;

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
                    //13 で始まるロット番号コードを読んだ場合はマガジン入れ替え無しと判断する
                    isSameUnloaderMag = true;
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

                ArmsApi.Model.VirtualMag[] vmags =
                    ArmsApi.Model.VirtualMag.GetVirtualMag(m.Mac.MacNo.ToString(), ((int)ArmsApi.Model.Station.Unloader).ToString(), string.Empty);

                ArmsApi.Model.VirtualMag vmag = vmags.Where(vm => vm.LastMagazineNo == mag.MagazineNo).FirstOrDefault();
                if (vmag == null)
                {
                    TempData["AlertMsg"] = "Unloader位置に一致する仮想マガジンが見つかりません lastMag:" + mag.MagazineNo;
                    return View(m);
                }
                m.AddMagazine(mag, vmag);

                if (isSameUnloaderMag)
                {
                    m.UnloaderMagNo = mag.MagazineNo;
                }
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult InputUnloaderMag(string txtMagNo, FormCollection fc)
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null) return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });

            //マガジン入れ替えが発生しない工程はこの画面を飛ばす
            //既に前画面でUnloaderMagNoが入っている場合もこの画面を飛ばす（13_のコードを読む高効率ラインの場合）
            if (m.IsNeedMagazineChange == false || !string.IsNullOrEmpty(m.UnloaderMagNo))
            {
                return RedirectToAction("Confirm");
            }

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

                ArmsApi.Model.Magazine curmag = ArmsApi.Model.Magazine.GetCurrent(magno);
                if (curmag != null)
                {
                    TempData["AlertMsg"] = string.Format("指定したマガジンNo「{1}」は、既に他の稼働中ロットに紐づいています。ロットNo = 「{0}」", curmag.NascaLotNO, magno);
                    return View(m);
                }

                m.UnloaderMagNo = magno;

                // 2021.08.02 なぜか下記のコードなしでも動作するようになった。。。原因わからん
                //////////////////////////////////////////////
                /// 2021.07.28 Junichi Watanabe 追加コード
                /// 
                //MachineInfo mc = MachineInfo.GetMachine(m.PlantCd);
                //List<VirtualMag> vmags = VirtualMag.GetVirtualMag(mc.MacNo, ((int)Station.EmptyMagazineLoader)).ToList();
                //if (vmags.Count == 0)
                //{
                //    vmags = VirtualMag.GetVirtualMag(mc.MacNo, ((int)Station.Unloader)).ToList();
                //    if (vmags.Count == 1)
                //    {
                //        var vm = vmags[0];
                //        vm.CurrentLocation = new Location(mc.MacNo, Station.EmptyMagazineLoader);
                //        vm.LastMagazineNo = vm.MagazineNo;
                //        vm.MagazineNo = magno;
                //        vm.Enqueue(vm, vm.CurrentLocation);
                //    }
                //    else
                //    {
                //        RedirectToAction("Message", "Home", new { msg = "仮想マガジン(Unloder)の状態が不正です" });
                //    }
                //}
                // ここまで
                //////////////////////////////////////////////

            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult UnloaderMag(string plantcd, string empcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            if (string.IsNullOrEmpty(empcd) == false)
            {
                Session["empcd"] = empcd;
            }

            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "WorkEndAlt";
                    Session["redirectAction"] = "UnloaderMag";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WorkEndAltModel(code);
                m.IsCompleteUnloaderMagazine = true;

                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["model"] = m;
                    Session["redirectController"] = "WorkEndAlt";
                    Session["redirectAction"] = "UnloaderMag";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = code;
                Session["model"] = m;
            }

            m.MagList = m.getUnloaderMag(m.PlantCd);

            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnloaderMag(string[] ulmagazine)
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }


            m.MagList = new List<ArmsApi.Model.Magazine>();
            m.NeedInspectionWhenCompleteLotList = new List<string>();

            ArmsApi.Model.VirtualMag[] vmags =
                ArmsApi.Model.VirtualMag.GetVirtualMag(m.Mac.MacNo.ToString(), ((int)ArmsApi.Model.Station.Unloader).ToString(), string.Empty);

            List<ArmsApi.Model.Magazine> svrmags = new List<ArmsApi.Model.Magazine>();
            foreach (string mag in ulmagazine)
            {
                ArmsApi.Model.Magazine svrmag = ArmsApi.Model.Magazine.GetCurrent(mag);

                //ブレンドされているロット、かつ最終工程以降の工程の開始の場合
                CutBlend[] cbs = CutBlend.GetData(mag);
                if (cbs.Length > 0)
                {
                    AsmLot lot = AsmLot.GetAsmLot(mag);
                    int lastprocno = Order.GetLastProcNoFromLotNo(cbs.First().BlendLotNo);
                    Process prevproc = Process.GetPrevProcess(lastprocno, lot.TypeCd);
                    Process nextprocess = Process.GetNextProcess(prevproc.ProcNo, lot);

                    if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                    {
                        svrmag = new Magazine();
                        svrmag.MagazineNo = mag;
                        svrmag.NascaLotNO = mag;
                        svrmag.NowCompProcess = prevproc.ProcNo;

                        ArmsApi.Model.AsmLot blendlot = lot;
                        blendlot.NascaLotNo = cbs.First().BlendLotNo;
                        m.BlendLotList.Add(mag, blendlot);
                    }
                }

                ArmsApi.Model.VirtualMag vmag = vmags.Where(vm => vm.LastMagazineNo == mag).FirstOrDefault();
                if (vmag == null)
                {
                    TempData["AlertMsg"] = "Unloader位置に一致する仮想マガジンが見つかりません lastMag:" + mag;
                    return View(m);
                }
                m.AddMagazine(svrmag, vmag);
            }

            if (m.IsNeedMagazineChange && m.MagList.Count >= 2)
            {
                return RedirectToAction("Message", "Home", new { msg = "UL側プレート読み込み必要なマガジンがあります 1マガジンずつ完了してください" });
            }

            Session["model"] = m;

            if (m.IsNeedMagazineChange)
            {
                return RedirectToAction("InputUnloaderMag");
            }

            return RedirectToAction("Submit");
            //return RedirectToAction("UnloaderMagConfirm");
            //return View(m);
        }

        public ActionResult Confirm()
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult UnloaderMagConfirm()
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult FormInfo() //Submit()
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
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
                return RedirectToAction("Message", "Home", new { msg = "完了登録で予期せぬエラー：" + ex.Message });
            }

            string displayPrintLabelButtonFg = "false";
            if (m.BlendLotList.Count > 0)
            {
                displayPrintLabelButtonFg = "true";
            }

            if (m.NeedInspectionWhenCompleteLotList.Count > 0)
            {
                string messageStr = "下記ﾛｯﾄは状態検査が必要です。\r\n";

                foreach (string targetLot in m.NeedInspectionWhenCompleteLotList.Distinct())
                {
                    messageStr += string.Format("{0}\r\n", targetLot);
                }

                return RedirectToAction("Message", "Home", new { msg = messageStr, displayPrintLabelButtonFg = displayPrintLabelButtonFg });
            }

            if (m.NeedAutoInspectionNextProc)
            {
                return RedirectToAction("Message", "Home", new { msg = "検査機への投入が必要です", displayPrintLabelButtonFg = displayPrintLabelButtonFg });
            }



            // 帳票入力リンク対応　juniwatanabe

            m.CurrentProcess = ArmsApi.Model.Process.GetNowProcess(m.MagList[0].NascaLotNO);

            Session["empcd"] = "";
            if (m.Comment.Contains("\r\n"))
            {
                m.Comment = m.Comment.Replace("\r\n", "<br/>");
            }
            Session["completemsg"] = m.Comment;
            //return RedirectToAction("Message", "Home", new { msg = "作業を完了しました\r\n", displayPrintLabelButtonFg = displayPrintLabelButtonFg });

            Session["model"] = m;
            return View(m);
        }


        /// <summary>
        /// 基板不良入力対応　juniwatanabe
        /// </summary>
        /// <returns></returns>

        public ActionResult InputFailureBdQty(int? txtFileBdQty, FormCollection fc)
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (txtFileBdQty > 0)
            {
                m.FailureBdQty = (int)txtFileBdQty;
            }

            Session["model"] = m;
            return View(m);
        }
    }

}
