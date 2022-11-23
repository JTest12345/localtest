using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class AfterCuringConfirmController : Controller
    {
        //社員コードの入力を行わないようにした
        //public ActionResult Index(string empcd)
        public ActionResult Index()
        {
            //if (string.IsNullOrEmpty(empcd) == false)
            //{
            //    Session["empcd"] = empcd;
            //}

            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                //string code = (string)Session["empcd"];
                //if (string.IsNullOrEmpty(code))
                //{
                //    Session["redirectController"] = "AfterCuringConfirm";
                //    Session["redirectAction"] = "Index";
                //    return RedirectToAction("InputEmpCd", "Home");
                //}

                //m = new AfterCuringConfirmModel(code);
                m = new AfterCuringConfirmModel();
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="lotindex"></param>
        /// <param name="RemoveAll"></param>
        /// <returns></returns>
        public ActionResult Remove(string lotindex, string RemoveAll)
        {
            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (RemoveAll != "1")
            {
                if (lotindex == null)
                {
                    return View("Index", m);
                }
                m.EditTarget = new List<ArmsApi.Model.AsmLot>();
                m.EditTarget.Add(m.LotList[int.Parse(lotindex)]);
            }
            else
            {
                if (m.LotList.Count() <= 0)
                {
                    return View("Index", m);
                }
                m.EditTarget = new List<ArmsApi.Model.AsmLot>();

                foreach (KeyValuePair<int, ArmsApi.Model.AsmLot> lot in m.LotList)
                {
                    m.EditTarget.Add(lot.Value);
                }
            }

            Session["model"] = m;
            return View(m);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult RemoveConfirmed()
        {
            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            foreach (ArmsApi.Model.AsmLot lot in m.EditTarget)
            {
                //aftercuringconfirm の解除
                m.RemoveAfterCuringConfirm(lot);
            }

            Session["model"] = null;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertNew()
        {
            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.EditTarget = new List<ArmsApi.Model.AsmLot>();

            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertNew(string barcode)
        {
            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                if (!string.IsNullOrEmpty(barcode))
                {
                    ArmsApi.Model.AsmLot lot = m.ParseBarCode(barcode);
                    if (lot != null)
                    {
                        string sErrMsg;
                        if (lot.AfterCuringConfirmfg == 1)
                        {
                            TempData["AlertMsg"] = "既に登録されているロットです。";
                        }
                        else
                        {
                            if (m.CheckProcNo(lot.TypeCd, lot.NascaLotNo, out sErrMsg))
                            {
                                m.EditTarget.Add(lot);
                            }
                            else
                            {
                                TempData["AlertMsg"] = sErrMsg;
                            }
                        }
                    }
                    else
                    {
                        TempData["AlertMsg"] = "入力されたロット番号のデータは存在しません";
                    }
                }
                else
                {
                    TempData["AlertMsg"] = "バーコード情報が不正です";
                }

                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertConfirm()
        {
            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                Session["model"] = m;
                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertConfirmed()
        {
            AfterCuringConfirmModel m = Session["model"] as AfterCuringConfirmModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }
            //aftercuringconfim <- 1
            m.UpdateNew();

            Session["model"] = null;
            return RedirectToAction("Index");
        }
    }
}
