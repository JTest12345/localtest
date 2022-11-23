using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi.Model;

namespace ArmsWeb.Controllers
{
    public class WorkStartAltController : Controller
    {
        //
        // GET: /WorkStart/

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

            WorkStartAltModel m = Session["model"] as WorkStartAltModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "WorkStartAlt";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WorkStartAltModel(code);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "WorkStartAlt";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = code;
                Session["model"] = m;
            }

            //20220627 ADD START
            //設備番号をキーに初工程かどうかを判定する
            if (!string.IsNullOrEmpty(m.PlantCd))
            {
                if (ArmsApi.Model.MachineInfo.IsFirstSt(m.PlantCd))
                {
                    //初工程
                    m.InputItem = "計画ロット読み込み";
                }
                else
                {
                    //以外
                    m.InputItem = "投入マガジン読み込み";
                }
                
                //マガジンコードとロット番号を照合する工程かどうかを判定
                m.IsLotNoChkProc = ArmsApi.Model.MachineInfo.IsLotNoChkProc(m.PlantCd);
            }
            //20220627 ADD END

            Session["model"] = m;

            if (m.Mac.ClassName.Contains("オーブン") || m.Mac.ClassName.Contains("ｵｰﾌﾞﾝ"))
            {
                return RedirectToAction("Index", "WorkReserve");
            }
            else
            {
                //if (m.Mac.ClassName.Contains("ｶｯﾄ") || m.Mac.ClassName.Contains("カット"))
                //{
                //if (MachineInfo.IsCutMachine(m.Mac.ClassName))
                //{
                    m.CutBlendList = ArmsApi.Model.CutBlend.GetCurrentBlendItems(m.Mac.MacNo).ToList();
                    if (m.CutBlendList.Count >= 1)
                    {
                        ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(m.CutBlendList[0].MagNo);
                        searchRecommend(mag, m);
                    }                
                //}
                //}

                return View(m);
            }
        }

        /// <summary>
        /// カット工程　ブレンド可能ロット検索機能
        /// </summary>
        private void searchRecommend(ArmsApi.Model.Magazine first, WorkStartAltModel m)
        {
            try
            {
                AsmLot firstlot = AsmLot.GetAsmLot(first.NascaLotNO);

                //最終工程以外では本機能は無効
                if (Process.GetNextProcess(first.NowCompProcess, firstlot).FinalSt == false) return;

                List<string> recommend = new List<string>();

                Magazine[] maglist = Magazine.GetMagazine(null, null, true, string.Join(",", first.ResinGr));
                foreach (Magazine mag in maglist)
                {
                    //＊＊＊＊＊＊未検証コード。resingpcd2でカットブレンドチェックするならば下記の動作検証が必要。
                    //ただしResinGr2のプロパティ自体がnewインスタンスしか返ってこない可能性があるので要確認。2018/5/29湯浅/川口
                    //if(first.ResinGr2 != null && first.ResinGr2.Count() > 0)
                    //{
                    //    if (string.Join(",", first.ResinGr2) != string.Join(",", mag.ResinGr2)) continue;
                    //}
                    //＊＊＊＊＊＊ここまで
                    
                    bool found = false;
                    foreach (Magazine exist in m.MagList)
                    {
                        if (mag.MagazineNo == exist.MagazineNo) found = true;
                    }

                    foreach (CutBlend cb in m.CutBlendList)
                    {
                        if (cb.MagNo == mag.MagazineNo) found = true;
                    }

                    if (found) continue;

                    if (mag.NowCompProcess == first.NowCompProcess)
                    {
                        AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                        string msg;
                        bool isError = WorkChecker.IsCutBlendError(new AsmLot[] { firstlot, lot }, out msg);
                        if (isError == false)
                        {
                            recommend.Add(lot.NascaLotNo);
                        }
                    }
                }

                foreach (string str in recommend)
                {
                    m.RecommendList.Add(str);
                }
            }
            catch
            {
                //何もしない
            }
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]
        //20220627 MOD START
        public ActionResult Index(string txtMagNo, FormCollection fc)
        //public ActionResult Index(string txtMagNo, string txtLotNo, FormCollection fc)
        //20220627 MOD END
        {
            WorkStartAltModel m = Session["model"] as WorkStartAltModel;

            if (!string.IsNullOrEmpty(txtMagNo))
            {
                string magno;
                int seqNo = 0;

                string[] elms = txtMagNo.Split(' ');

                //////////////////////////////////////////
                // 照明合理化マガジン（ヘッダーなし）対応
                // 2022.03.03 Junichi Watanabe
                if (elms.Length == 1)
                {
                    elms = new string[] { "C30", txtMagNo };
                    txtMagNo = "C30 " + txtMagNo;
                }
                //////////////////////////////////////////

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

                // TmWorkflow上の第1作業の装置の時、ロットNo = マガジンNoの仮ロットを作成する
                int firstprocno;
                if (mag == null && m.IsFirstProcess(out firstprocno) == true)
                {
                    //重複読込チェック
                    if (m.MagList.Where(em => em.MagazineNo == magno).Count() >= 1)
                    {
                        TempData["AlertMsg"] = "既に読み込み済みのマガジンです";
                        return View(m);
                    }

                    if (txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_INLINE_LOT) 
                        || txtMagNo.StartsWith(ArmsApi.Model.AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                    {
                        TempData["AlertMsg"] = "第1作業の空マガジン開始処理でマガジンラベルが読み込まれています。";
                        return View(m);
                    }

                    // 空マガジンの仮レコードを読み込み済みに追加。開始登録処理にてレコードを登録する。
                    mag = new ArmsApi.Model.Magazine();
                    mag.NascaLotNO = magno;
                    mag.MagazineNo = magno;
                    mag.NowCompProcess = firstprocno;

                    m.MagList.Add(mag);
                    Session["model"] = m;
                    return View(m);
                }

                if (mag == null)
                {
                    //富士情報 追加　start
                    //合理化はロット管理表のロット番号読めるように対応
                    seqNo = 1;
                    //富士情報 追加　end

                    if (seqNo >= 1)
                    {
                        seqNo = 0;
                        magno = ArmsApi.Model.Order.MagLotToNascaLot(magno);
                        
                        //ブレンドされているロット、かつ最終工程以降の工程の開始の場合
                        CutBlend[] cbs = CutBlend.GetData(magno);
                        if (cbs.Length > 0)
                        {
                            AsmLot blendlot = null;
                            AsmLot lot = AsmLot.GetAsmLot(cbs.First().LotNo);

                            //ブレンドロットの最終工程を取得
                            int lastprocno = Order.GetLastProcNoFromLotNo(cbs.First().BlendLotNo);
                            Process nextprocess = Process.GetNextProcess(lastprocno, lot);

                            if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                            {
                                //ブレンドロットはTnLotに存在しないので、アッセンのロットで取得しロット番号を置き換える
                                blendlot = lot;
                                blendlot.NascaLotNo = cbs.First().BlendLotNo;
                            }

                            if (blendlot != null)
                            {
                                //重複読込チェック
                                if (m.BlendLotList.Where(b => b.Key == magno).Count() >= 1)
                                {
                                    TempData["AlertMsg"] = "既に読み込み済みのマガジンです";
                                    return View(m);
                                }
                                m.BlendLotList.Add(magno, blendlot);
                            }

                            //magは子ロットが入る
                            mag = new Magazine();
                            mag.NascaLotNO = magno;
                            mag.MagazineNo = magno;
                            mag.NowCompProcess = lastprocno;
                        }
                        else
                        {
                            mag = ArmsApi.Model.Magazine.GetCurrent(magno);
                        }

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

                if (WorkStartEndModel.IsDoubleToSingleProcess(mag) == true)
                {
                    //WorkStartEndを使ってもらう仕様
                    TempData["AlertMsg"] = "マガジン統合工程は対応していません";
                    return View(m);
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

                //20220627 ADD START
                //既に開始済の場合はエラーとする
                isOk = m.CheckInputDuplication(mag, out msg);
                if (!isOk)
                {
                    TempData["AlertMsg"] = msg;
                    return View(m);
                }

                ////ロット番号照合工程の場合
                if (m.IsLotNoChkProc)
                {
                    //    //ロット番号未入力はエラー
                    //    if (string.IsNullOrEmpty(txtLotNo))
                    //    {
                    //        TempData["AlertMsg"] = "計画ロット番号を入力してください";
                    //        return View(m);
                    //    }
                    //    //マガジン番号とロット番号チェック
                    //    if (!ArmsApi.Model.Magazine.IsLotnoChkToMag(magno, txtLotNo))
                    //    {
                    //        TempData["AlertMsg"] = "入力した計画ロット番号が正しくありません";
                    //        return View(m);
                    //    }
                    m.magazineno = magno;
                    Session["mag"] = mag;
                    Session["model"] = m;
                    Session["redirectController"] = "WorkStartAlt";
                    Session["redirectAction"] = "Compare";
                    return RedirectToAction("InputLotNo", "Home");
                }
                //20220627 ADD END

                m.MagList.Add(mag);
            }
            Session["model"] = m;
            return View(m);
        }

        //20220627 ADD START
        public ActionResult Compare(string lotno)
        {
            WorkStartAltModel m = Session["model"] as WorkStartAltModel;
            ArmsApi.Model.Magazine mag = Session["mag"] as ArmsApi.Model.Magazine;

            if (string.IsNullOrEmpty(lotno) == false)
            {
                Session["lotno"] = lotno;
            }
            string code = (string)Session["lotno"];
            if (string.IsNullOrEmpty(code))
            {
                TempData["AlertMsg"] = "計画ロット番号を入力してください";
                Session["redirectController"] = "WorkStartAlt";
                Session["redirectAction"] = "Compare";
                return RedirectToAction("InputLotNo", "Home");
            }

            m.lotno = (string)Session["lotno"];

            //マガジン番号とロット番号チェック
            if (!ArmsApi.Model.Magazine.IsLotnoChkToMag(m.magazineno, m.lotno))
            {
                TempData["AlertMsg"] = "計画ロット番号照合エラー";
                Session["redirectController"] = "WorkStartAlt";
                Session["redirectAction"] = "Compare";
                return RedirectToAction("InputLotNo", "Home");
            }

            m.MagList.Add(mag);
            Session["model"] = m;
            return RedirectToAction("Index");
        }
        //20220627 ADD END

        public ActionResult Confirm()
        {
            WorkStartAltModel m = Session["model"] as WorkStartAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "WorkStartAltModelが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }


        public ActionResult Submit()
        {
            WorkStartAltModel m = Session["model"] as WorkStartAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "WorkStartModelが見つかりません" });
            }

            //20220627 ADD START
            Session["workunitid"] = null;
            //20220627 ADD END

            try
            {
                string msg;
                bool success = m.WorkStart(out msg);

                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "開始登録で予期せぬエラー：" + ex.Message });
            }

            //20220627 ADD START
            Session["workunitid"] = m.WorkUnitId;
            if (m.WorkUnitId != null)
            {
                Session["URL"] = m.URL; 
            }
            //20220627 ADD END

            //状態検査とダイシェアのメッセージが両立するように修正。　2015.9.23 湯浅
            string messageStr = "作業開始しました。";

            if (m.NeedInspectionWhenStartLotList.Count > 0)
            {
                messageStr += "下記ﾛｯﾄは状態検査が必要です。\r\n";

                foreach (string targetLot in m.NeedInspectionWhenStartLotList.Distinct())
                {
                    messageStr += string.Format("{0}\r\n", targetLot);
                }
            }

            if (m.DieShearSamplingLotList.Count > 0)
            {
                messageStr += "下記ﾛｯﾄはﾀﾞｲｼｪｱ対象です。\r\n";

                foreach (string targetLot in m.DieShearSamplingLotList.Distinct())
                {
                    messageStr += string.Format("{0}\r\n", targetLot);
                }
            }

            //富士情報　追加　start
            //20220413 ADD START
            if (!string.IsNullOrEmpty(m.Comment2))
            {
                messageStr += string.Format("\r\n{0}", m.Comment2);
            }
            //20220413 ADD END
            if (!string.IsNullOrEmpty(m.Comment))
            {
                messageStr += string.Format("\r\n{0}", m.Comment);
            }
            //富士情報　追加　end

            Session["empcd"] = "";
            return RedirectToAction("Message", "Home", new { msg = messageStr });
        }

        public ActionResult FormInfo() //Submit()
        {
            WorkStartAltModel m = Session["model"] as WorkStartAltModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "WorkStartAltModelが見つかりません" });
            }

            try
            {
                string msg;
                bool success = m.WorkStart(out msg);

                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "開始登録で予期せぬエラー：" + ex.Message });
            }

            //状態検査とダイシェアのメッセージが両立するように修正。　2015.9.23 湯浅
            string messageStr = "作業開始しました。";

            if (m.NeedInspectionWhenStartLotList.Count > 0)
            {
                messageStr += "下記ﾛｯﾄは状態検査が必要です。\r\n";

                foreach (string targetLot in m.NeedInspectionWhenStartLotList.Distinct())
                {
                    messageStr += string.Format("{0}\r\n", targetLot);
                }
            }

            if (m.DieShearSamplingLotList.Count > 0)
            {
                messageStr += "下記ﾛｯﾄはﾀﾞｲｼｪｱ対象です。\r\n";

                foreach (string targetLot in m.DieShearSamplingLotList.Distinct())
                {
                    messageStr += string.Format("{0}\r\n", targetLot);
                }
            }

            m.CurrentProcess = ArmsApi.Model.Process.GetNowProcess(m.MagList[0].NascaLotNO);

            Session["empcd"] = "";
           // return RedirectToAction("Form", "Home", new { msg = messageStr });

            Session["model"] = m;
            return View(m);

        }

    }
}
