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
            //20220822 ADD START
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;

            //マガジン入替ありでもキャンセルで一覧が出たほうがいいのでマガジン入替なしを削除する。
            //if (!m.IsNeedMagazineChange)
            //{
            m.VirtualMags.Clear();
            //キャンセル後、一覧 → 決定ボタン押下で確認画面へ遷移するのを防止するためクリア
            m.UnloaderMagNo = null;    
            Session["model"] = m;
            return RedirectToAction("UnloaderMag");
            //}
            //20220822 ADD END

            //20220822 DEL START
            //遷移する画面を UnloaderMag としたため削除（通常処理では Index.cshtml には遷移していない？）
            //Session["model"] = null;
            //return RedirectToAction("Index");
            //20220822 DEL END
        }

        public ActionResult CancelUnloaderMagEdit()
        {
            //Session["model"] = null;

            //20220822 ADD START
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            m.VirtualMags.Clear();
            Session["model"] = m;
            //20220822 ADD END

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

                //////////////////////////////////////////
                // 照明合理化マガジン（ヘッダーなし）対応
                // 2022.03.03 Junichi Watanabe
                if (elms.Length == 1)
                {
                    elms = new string[] { "C30", txtMagNo };
                    raw = "C30 " + txtMagNo;
                }
                //////////////////////////////////////////

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

                //////////////////////////////////////////
                // 照明合理化マガジン（ヘッダーなし）対応
                // 2022.03.03 Junichi Watanabe
                if (elms.Length == 1)
                {
                    //20220831 ADD START
                    //マガジンコードチェック
                    if (!m.CheckMagno(raw))
                    {
                        TempData["AlertMsg"] = "マガジンコードではありません";
                        return View(m);
                    }
                    //20220831 ADD END

                    elms = new string[] { "C30", txtMagNo };
                    raw = "C30 " + txtMagNo;
                }
                //////////////////////////////////////////

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

            //20220822 FJH ADD START
            //未選択で「決定」ボタンを押下するとシステムエラーになるので回避
            if (ulmagazine == null)
            {
                TempData["AlertMsg"] = "完了するロットを選択してください";
                return View(m);
            }
            //20220822 FJH ADD END

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
            //20220822 FJH ADD START
            //マガジン入替なしで、単一選択の場合は、基板不良枚数入力へ遷移させる
            else
            {
                if (m.MagList.Count == 1)
                {
                    m.TypeCd = ArmsApi.Model.AsmLot.GetAsmLot(m.MagList[0].NascaLotNO).TypeCd;
                    return RedirectToAction("InputDef");
                }
            }
            //20220822 FJH ADD END

            //return RedirectToAction("Submit");
            return RedirectToAction("UnloaderMagConfirm");
            //return View(m);
        }

        public ActionResult Confirm()
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            #region FJH ADD 
            //不良枚数取得
            ArmsApi.Model.Defect defect = new ArmsApi.Model.Defect();
            m.FailureBdQty = defect.GetDefectCtSubSt(m.MagList[0].NascaLotNO, (int)m.VirtualMags.ElementAt(0).Value.ProcNo);
            #endregion 

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

        //20220822 FJH MOD START
        //引数追加
        //public ActionResult Submit()
        public ActionResult Submit(string kbn)
        //20220822 FJH MOD END
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            //20220627 ADD START
            Session["workunitid"] = null;
            //20220627 ADD END

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

            //20220627 ADD START
            Session["workunitid"] = m.WorkUnitId;
            if (m.WorkUnitId != null)
            {
                Session["URL"] = m.URL;
            }
            //20220627 ADD END

            Session["empcd"] = "";
            Session["completemsg"] = m.Comment;

            //20220822 FJH MOD START
            //引数の内容で、画面遷移を変更する
            //return RedirectToAction("Message", "Home", new { msg = "作業を完了しました\r\n", displayPrintLabelButtonFg = displayPrintLabelButtonFg });
            if (string.IsNullOrEmpty(kbn))
            {
                return RedirectToAction("Message", "Home", new { msg = "作業を完了しました\r\n", displayPrintLabelButtonFg = displayPrintLabelButtonFg });
            }
            else
            {
                return RedirectToAction("Message", new { msg = "作業を完了しました\r\n", displayPrintLabelButtonFg = displayPrintLabelButtonFg });
            }
            //20220822 FJH MOD END
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

        #region FJH ADD 
        //FJH ADD START
        public ActionResult Select()
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            //TnTranを使用して「ProcNo」を取得する
            //m.ProcNo = ArmsApi.Model.Order.GetLastProcNoFromLotNo(m.MagList[0].NascaLotNO);
            m.TypeCd = ArmsApi.Model.AsmLot.GetAsmLot(m.MagList[0].NascaLotNO).TypeCd;

            Session["model"] = m;
            return View(m);
        }

        public ActionResult InputDef(string classcd, string causecd, string defectcd, string qty)
        {
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;

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
            WorkEndAltModel m = Session["model"] as WorkEndAltModel;

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
            defect.LotNo = m.MagList[0].NascaLotNO;
            defect.DefItems = new List<ArmsApi.Model.DefItem>(defs);
            //defect.ProcNo = m.ProcNo;
            defect.ProcNo = (int)m.VirtualMags.ElementAt(0).Value.ProcNo;
            defect.MagazineNo = m.MagList[0].MagazineNo;

            //数量チェック
            if (!defect.CheckDefectSubSt(null))
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

            //Defect更新
            defect.DeleteInsertSubSt(null);

            //20220822 DEL START
            //キャンセルで社員コード入力画面に遷移するので削除する。
            //Session["empcd"] = "";
            //20220822 DEL END
            return View("Select", m);
        }

        //20220822 FJH ADD START
        //WorkEndAlt専用Message表示用
        public ActionResult Message(string msg, string displayPrintLabelButtonFg)
        {
            if (msg == null) msg = "";

            if (Session["completemsg"] != null)
            {
                msg += Session["completemsg"];
            }

            if (msg.Contains("\r\n"))
            {
                msg = msg.Replace("\r\n", "<br/>");
            }

            ViewData["msg"] = msg;

            if (string.IsNullOrWhiteSpace(displayPrintLabelButtonFg) == false &&
                Convert.ToBoolean(displayPrintLabelButtonFg) == true)
            {
                ViewData["displayPrintLabelButton"] = "true";
            }

            if (Session["workunitid"] != null)
            {
                ViewData["workunitid"] = Session["workunitid"];
                ViewData["URL"] = Session["URL"];
            }
            return View();
        }
        //20220822 FJH ADD END

        //FJH ADD END
        #endregion 
    }

}
