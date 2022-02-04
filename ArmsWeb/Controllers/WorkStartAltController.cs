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

            Session["model"] = m;

            if (m.Mac.ClassName.Contains("オーブン") || m.Mac.ClassName.Contains("ｵｰﾌﾞﾝ"))
            {
                return RedirectToAction("Index", "WorkReserve");
            }
            else
            {
                //if (m.Mac.ClassName.Contains("ｶｯﾄ") || m.Mac.ClassName.Contains("カット"))
                //{
                if (MachineInfo.IsCutMachine(m.Mac.ClassName))
                {
                    m.CutBlendList = ArmsApi.Model.CutBlend.GetCurrentBlendItems(m.Mac.MacNo).ToList();
                    if (m.CutBlendList.Count >= 1)
                    {
                        ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(m.CutBlendList[0].MagNo);
                        searchRecommend(mag, m);
                    }                
                }
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
        public ActionResult Index(string txtMagNo, FormCollection fc)
        {
            WorkStartAltModel m = Session["model"] as WorkStartAltModel;

            if (!string.IsNullOrEmpty(txtMagNo))
            {
                string magno;
                int seqNo = 0;

                string[] elms = txtMagNo.Split(' ');

                // Magno Header Check Scripts here .... JuniWatanabe
                // マガジンにヘッダー"A "がない場合の暫定対応
                if (elms.Length == 1)
                {
                    elms = new string[] { "C30", txtMagNo };
                    txtMagNo = "C30 " + txtMagNo;
                }


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

                m.MagList.Add(mag);
            }
            Session["model"] = m;
            return View(m);
        }

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
