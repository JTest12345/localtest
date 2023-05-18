using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;


namespace ArmsWeb.Controllers
{
    public class WaferController : Controller
    {
        //
        // GET: /Wafer/
        public ActionResult Index(string plantcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            WaferEditModel m = Session["model"] as WaferEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "Wafer";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new WaferEditModel(code);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }


        /// <summary>
        /// 投入中のウェハー全削除
        /// </summary>
        /// <param name="matdata"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult Remove(string btnSubmit, List<string> selectedWaferList)
        {
            WaferEditModel m = Session["model"] as WaferEditModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (btnSubmit== "装置振替")
            {
                if (selectedWaferList == null || selectedWaferList.Count() == 0)
                {
                    TempData["AlertMsg"] = "カセットが1つも選択されていません。選択してから「装置振替」ボタンを押してください。";
                    return RedirectToAction("Index", m);
                }

                m.EditTarget = new List<ArmsApi.Model.Material>();
                foreach(string s in selectedWaferList)
                {
                    int iKey;
                    if (int.TryParse(s, out iKey) == false) continue;
                    m.EditTarget.Add(m.WaferList[iKey]);
                }
                Session["model"] = m;
                return RedirectToAction("ChangeMachine");
            }

            Session["model"] = m;
            return View(m);
        }

        /// <summary>
        /// </summary>
        /// <param name="matdata"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult RemoveConfirmed()
        {
            WaferEditModel m = Session["model"] as WaferEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.RemoveAllWafer(DateTime.Now);

            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult InsertNew()
        {
            WaferEditModel m = Session["model"] as WaferEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            return View(m);
        }

        public ActionResult InsertConfirm(string barcode)
        {
            WaferEditModel m = Session["model"] as WaferEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                List<ArmsApi.Model.Material> waf = m.ParseWafers(barcode);
                m.EditTarget = waf;
                Session["model"] = m;
                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }


        public ActionResult InsertConfirmed()
        {
            WaferEditModel m = Session["model"] as WaferEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.InsertNew();

            Session["model"] = null;
            return RedirectToAction("Index");
        }
        
        public ActionResult ChangeMachine()
        {
            WaferEditModel m = Session["model"] as WaferEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            return View(m);
        }

        public ActionResult ChangeMachineConfirm(string newplantcd)
        {
            try
            {
                WaferEditModel m = Session["model"] as WaferEditModel;
                if (m == null)
                {
                    return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
                }

                string[] elms = newplantcd.Split(' ');
                string barcodeheader = elms[0] + ' ';
                if (barcodeheader != ArmsApi.Config.MAC_BARCODE_HEADER &&
                    barcodeheader != ArmsApi.Config.MAC_WAFER_BARCODE_HEADER)
                {
                    TempData["AlertMsg"] = $"バーコードヘッダー異常（{ArmsApi.Config.MAC_BARCODE_HEADER}, {ArmsApi.Config.MAC_WAFER_BARCODE_HEADER})";
                    return RedirectToAction("ChangeMachine");
                }

                newplantcd = newplantcd.Replace(barcodeheader, string.Empty);
                
                if (m.Mac.NascaPlantCd == newplantcd)
                {
                    TempData["AlertMsg"] = "振替元装置と同じ設備CDが読み込まれました(別の装置を指定してください)";
                    return RedirectToAction("ChangeMachine");
                }

                m.NewMac = ArmsApi.Model.MachineInfo.GetMachine(newplantcd);
                if (m.NewMac == null)
                {
                    TempData["AlertMsg"] = $"装置情報が見つかりませんでした[設備CD:{newplantcd}]";
                    return RedirectToAction("ChangeMachine");
                }

                // 新規装置への部材投入前チェック (NGの部材が一つでもあれば、中止)
                List<string> errMsgList = new List<string>();
                foreach(ArmsApi.Model.Material waf in m.EditTarget)
                {
                    // 後の取り外し処理時に元の投入日時を参照する為、投入チェック後に必ず元に戻す
                    DateTime? originInputDt = waf.InputDt;
                    waf.InputDt = DateTime.Now;
                    string errMsg;
                    try
                    {
                        if (ArmsApi.Model.WorkChecker.IsErrorBeforeInputMat(waf, m.NewMac, out errMsg))
                        {
                            errMsgList.Add($"ロットNo「{waf.LotNo}」:{errMsg}");
                        }
                    }
                    catch(Exception ex)
                    {
                        errMsgList.Add($"ロットNo「{waf.LotNo}」:{ex.Message}");
                    }
                    finally
                    {
                        waf.InputDt = originInputDt;
                    }
                }
                if (errMsgList.Any() == true)
                {
                    throw new ApplicationException("振替先装置への資材投入前チェックで判定NGが発生しました。\r\n" + string.Join("\r\n", errMsgList));
                }

                m.NewPlantCd = newplantcd;


                Session["model"] = m;
                return View(m);
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("ChangeMachine");
            }            
        }

        public ActionResult ChangeMachineConfirmed()
        {
            try
            {
                WaferEditModel m = Session["model"] as WaferEditModel;
                if (m == null)
                {
                    return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
                }

                m.ChangeMachine();

                string outMsg = string.Empty;
                outMsg += $"下記の内容でカセット情報を振り替えました\r\n\r\n";
                outMsg += $"装置(振替元)：{m.Mac.LongName}\r\n";
                foreach(ArmsApi.Model.Material mat in m.RemoveList)
                {
                    outMsg += $"  {mat.StockerNo} : {mat.LongName}\r\n";
                }
                outMsg += $"\r\n装置(振替先)：{m.NewMac.LongName}\r\n";
                foreach (ArmsApi.Model.Material mat in m.EditTarget)
                {
                    outMsg += $"  {mat.StockerNo} : {mat.LongName}\r\n";
                }

                Session["model"] = null;

                return RedirectToAction("Message", "Home", new { msg = outMsg });
                //return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("Index");
            }
        }

        public ActionResult ChangeMachineCancel()
        {
            WaferEditModel m = Session["model"] as WaferEditModel;
            Session["model"] = null;
            return RedirectToAction("Index");
        }
    }
}
