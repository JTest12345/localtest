using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class MaterialController : Controller
    {
        //
        // GET: /Material/
        public ActionResult Index(string plantcd)
        {
            if (string.IsNullOrEmpty(plantcd) == false)
            {
                Session["plantcd"] = plantcd;
            }

            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "Material";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new MaterialEditModel(code);
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
        /// matdata=GGコード_ロット番号の形
        /// </summary>
        /// <param name="matdata"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult Remove(string matindex, string RemoveAll)
        //public ActionResult Remove(string matindex)
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            //取り外しボタンと全取り外しボタン(新規)に処理分岐　2016.11.5 湯浅

            if (RemoveAll != "1")
            {
                if (matindex == null)
                {
                    return View("Index", m);
                }
                m.EditTarget = new List<ArmsApi.Model.Material>();
                m.EditTarget.Add(m.MatList[int.Parse(matindex)]);
            }
            else
            {
                if (m.MatList.Count() <= 0)
                {
                    return View("Index", m);
                }
                m.EditTarget = new List<ArmsApi.Model.Material>();

                foreach (KeyValuePair<int, ArmsApi.Model.Material> mat in m.MatList)
                {
                    m.EditTarget.Add(mat.Value);
                }
            }
            //switch (submitbutton)
            //{
            //    case "Each":
            //        if (matindex == null)
            //        {
            //            return View("Index", m);
            //        }
            //        m.EditTarget = new List<ArmsApi.Model.Material>();
            //        m.EditTarget.Add(m.MatList[int.Parse(matindex)]);

            //        break;

            //    case "All":
            //        if (m.MatList.Count() <= 0)
            //{
            //    return View("Index", m);
            //}
            //m.EditTarget = new List<ArmsApi.Model.Material>();

            //foreach (KeyValuePair<int, ArmsApi.Model.Material> mat in m.MatList)
            //{
            //    m.EditTarget.Add(mat.Value);
            //}
            //        break;
            //}

            //Session["model"] = m;
            //return View(m);


            //if (matindex == null)
            //{
            //    return View("Index", m);
            //}
            //m.EditTarget = new List<ArmsApi.Model.Material>();

            //m.EditTarget.Add(m.MatList[int.Parse(matindex)]);
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
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            foreach (ArmsApi.Model.Material mat in m.EditTarget)
            {
                m.RemoveMaterial(mat, DateTime.Now);
            }

            Session["model"] = null;
            return RedirectToAction("Index");
        }

        public ActionResult InsertNew()
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.EditTarget = new List<ArmsApi.Model.Material>();

            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InsertNew(string barcode, bool isRingID)
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                ArmsApi.Model.Material mat = m.ParseMatBarCode(barcode, isRingID);
                string errMsg;
                if (ArmsApi.Model.WorkChecker.IsErrorBeforeInputMat(mat, m.Mac, out errMsg))
                {
                    return RedirectToAction("Message", "Home", new { msg = errMsg });
                }

                m.EditTarget.Add(mat);

                // 「リングID」のチェックIDを引継ぎ
                m.isCheckedRingID = isRingID;

                //富士情報　start
                //登録する部材ロットががバックアップ工程対象の半完成品のロットかチェックする
                //部材ロットから枝番を排除する
                string lotno = mat.LotNo.Substring(0, mat.LotNo.LastIndexOf("-"));
                lotno = lotno.Substring(0, lotno.LastIndexOf("-"));
                ArmsApi.Model.VLot vl = ArmsApi.Model.VLot.GetVLotFrom4MLot(lotno);
                if (vl.IsBackUp)
                {
                    TempData["AlertMsg"] += $"\r\n{lotno}:V溝無しLDバックアップ工程対象ロットです。";
                }
                //富士情報　end

                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }

        public ActionResult InsertConfirm(string barcode)
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                //ArmsApi.Model.Material mat = m.ParseMatBarCode(barcode);
                //string errMsg;
                //if (ArmsApi.Model.WorkChecker.IsErrorBeforeInputMat(mat, m.Mac, out errMsg))
                //{
                //    return RedirectToAction("Message", "Home", new { msg = errMsg });
                //}

                //m.EditTarget.Add(mat);
                Session["model"] = m;
                return View(m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("InsertNew", m);
            }
        }

        public ActionResult InsertConfirmed(FormCollection fc)
        {
            MaterialEditModel m = Session["model"] as MaterialEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            List<string> exceptMags = new List<string>();
            foreach (string key in fc.Keys)
            {
                exceptMags.Add(fc[key]);
            }

            m.InsertNew(exceptMags);

            Session["model"] = null;
            return RedirectToAction("Index");
        }
    }
}
