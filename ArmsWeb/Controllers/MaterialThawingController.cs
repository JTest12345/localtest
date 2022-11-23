using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;
using ArmsApi.Model;

namespace ArmsWeb.Controllers
{
    /// <summary>
    /// 解凍処理
    /// </summary>
    public class MaterialThawingController : Controller
    {
        public ActionResult Index(string empcd)
        {
            if (string.IsNullOrEmpty(empcd) == false)
            {
                Session["empcd"] = empcd;
            }

            MaterialThawingModel m = Session["model"] as MaterialThawingModel;
            if (m == null)
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "MaterialThawing";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m = new MaterialThawingModel(code);
            }

            Session["model"] = m;
            return View(m);
        }

        /// <summary>
        /// キャンセル 
        /// </summary>
        /// <returns></returns>
        public ActionResult CancelEdit()
        {
            Session["model"] = null;
            return RedirectToAction("Index");
        }

        [AcceptVerbs(HttpVerbs.Post)]

        /// <summary>
        /// 入力
        /// </summary>
        /// <param name="txtQRCode"></param>
        /// <param name="fc"></param>
        /// <returns></returns>
        public ActionResult Index(string txtQRCode, FormCollection fc)
        {
            MaterialThawingModel m = Session["model"] as MaterialThawingModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            try
            {
                //QRコード分割・TnMaterial存在チェック
                ArmsApi.Model.Material mat = m.ParseMatBarCode(txtQRCode);

                //QRコードチェック
                m.CheckQRCode(mat.TypeCd, mat.LotNo);

                m.EditTarget.Add(mat);

                return RedirectToAction("Confirm", m);
            }
            catch (ApplicationException ex)
            {
                TempData["AlertMsg"] = ex.Message;
                return RedirectToAction("Index", m);
            }
        }

        /// <summary>
        /// 確認 
        /// </summary>
        /// <returns></returns>
        public ActionResult Confirm()
        {
            MaterialThawingModel m = Session["model"] as MaterialThawingModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "MaterialThawingModelが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        /// <summary>
        /// 登録 
        /// </summary>
        /// <returns></returns>
        public ActionResult Submit()
        {
            MaterialThawingModel m = Session["model"] as MaterialThawingModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "MaterialThawingModelが見つかりません" });
            }

            try
            {
                string msg;
                bool success = m.InsertUpdateMatCond(out msg);

                if (!success)
                {
                    return RedirectToAction("Message", "Home", new { msg = "エラー：" + msg });
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "解凍登録で予期せぬエラー：" + ex.Message });
            }

            string messageStr = "解凍登録しました。";

            Session["empcd"] = "";
            return RedirectToAction("Message", "Home", new { msg = messageStr });
        }
    }
}
