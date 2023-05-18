using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class HomeController : Controller
    {
        //
        // GET: /Home/
        public ActionResult Index(string id, string empcd)
        {
            Session.Clear();
            if (!string.IsNullOrEmpty(empcd)) Session["empcd"] = empcd;
            if (!string.IsNullOrEmpty(id)) Session["plantcd"] = id;
            return View();
        }


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

            return View();
        }

        public ActionResult PrintLabel()
        {
            WorkEndModel m = Session["model"] as WorkEndModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            Session["completemsg"] = null;

            string msg = "ラベル印字完了";
            try
            {
                string blendlotno = m.BlendLotList.First().Value.NascaLotNo;
                CutBlend[] currentBlend = CutBlend.SearchBlendRecord(null, blendlotno, null, false, false);

                CutBlend.PrintCutLabel(currentBlend, blendlotno, true);
            }
            catch (Exception ex)
            {
                msg = "ラベル印字失敗:" + ex.Message;
            }

            return RedirectToAction("Message", "Home", new { msg = msg });
        }

        public ActionResult InputPlantCd()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputPlantCd(string plantcd)
        {
            if (plantcd.StartsWith(ArmsApi.Config.MAC_BARCODE_HEADER) == false 
                && plantcd.StartsWith(ArmsApi.Config.MAC_COMP_BARCODE_HEADER) == false)
            {
                TempData["AlertMsg"] = "バーコードヘッダー異常（07, 36)";
                return View();
            }

            plantcd = plantcd.Replace(ArmsApi.Config.MAC_BARCODE_HEADER, string.Empty);
            plantcd = plantcd.Replace(ArmsApi.Config.MAC_COMP_BARCODE_HEADER, string.Empty);

            ArmsApi.Model.MachineInfo mac = ArmsApi.Model.MachineInfo.GetMachine(plantcd);
            if (mac == null)
            {
                TempData["AlertMsg"] = "装置情報が見つかりませんでした";
                return View();
            }

            Session["plantcd"] = plantcd;

            string action = (string)Session["redirectAction"];
            string controller = (string)Session["redirectController"];

            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
            {
                return RedirectToAction(action, controller);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        public ActionResult InputEmpCd()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputEmpCd(string empcd)
        {
            if (empcd.StartsWith(ArmsApi.Config.EMP_BARCODE_HEADER) == false)
            {
                TempData["AlertMsg"] = "バーコードヘッダー異常（01 )";
                return View();
            }

            empcd = empcd.Replace(ArmsApi.Config.EMP_BARCODE_HEADER, string.Empty);
            int empcdint;
            if (!int.TryParse(empcd, out empcdint))
            {
                TempData["AlertMsg"] = "社員番号は数字を入力してください";
                return View();
            }

            Session["empcd"] = empcd;

            string action = (string)Session["redirectAction"];
            string controller = (string)Session["redirectController"];

            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
            {
                return RedirectToAction(action, controller);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }


        /// <summary>
        /// 高効率ライン完了通知用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carrierNo"></param>
        /// <param name="macgroup"></param>
        /// <returns></returns>
        public ActionResult MelInst(string id, string macgroup)
        {
            ArmsWeb.Models.MelInstModel m = new Models.MelInstModel(int.Parse(id), macgroup);
            return View(m);
        }

        public ActionResult DBPreOvnPLInst()
        {
            ArmsWeb.Models.DBPreOvnPLInstModel m = new Models.DBPreOvnPLInstModel();
            return View(m);
        }

        public ActionResult InputLotNo()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputLotNo(string lotno)
        {
            string originallotno = lotno;

            string[] elms = lotno.Split(' ');
            if (elms.Length >= 2)
            {
                lotno = elms[1];
            }
            elms = originallotno.Split(',');
            if (elms.Length == 5)
            {
                //ライフラベルを想定
                //ロットNo,型番,数量,日時,
                lotno = elms[0];
            }

            Session["lotno"] = lotno;

            string action = (string)Session["redirectAction"];
            string controller = (string)Session["redirectController"];

            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
            {
                return RedirectToAction(action, controller);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult InputMagazineNo()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputMagazineNo(string magazineno)
        {
            string[] elms = magazineno.Split(' ');
            if (elms.Length >= 2)
            {
                magazineno = elms[1];
            }

            Session["magazineno"] = magazineno;

            string action = (string)Session["redirectAction"];
            string controller = (string)Session["redirectController"];

            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
            {
                return RedirectToAction(action, controller);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        public ActionResult InputRing()
        {
            return View();
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InputRing(string ringdata)
        {
            Session["ringdata"] = ringdata;

            string action = (string)Session["redirectAction"];
            string controller = (string)Session["redirectController"];

            if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
            {
                return RedirectToAction(action, controller);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

		public ActionResult InputCurrentCarrierNo()
		{
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InputCurrentCarrierNo(string orgcarrierno)
		{
			Session["orgcarrierno"] = orgcarrierno;

			string action = (string)Session["redirectAction"];
			string controller = (string)Session["redirectController"];

			if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
			{
				return RedirectToAction(action, controller);
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

		public ActionResult InputNewCarrierNo()
		{
			return View();
		}

		[AcceptVerbs(HttpVerbs.Post)]
		public ActionResult InputNewCarrierNo(string newcarrierno)
		{
			Session["newcarrierno"] = newcarrierno;

			string action = (string)Session["redirectAction"];
			string controller = (string)Session["redirectController"];

			if (!string.IsNullOrEmpty(action) && !string.IsNullOrEmpty(controller))
			{
				return RedirectToAction(action, controller);
			}
			else
			{
				return RedirectToAction("Index");
			}
		}

        /// <summary>
        /// 新型自動搬送ライン開始通知用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carrierNo"></param>
        /// <param name="macgroup"></param>
        /// <returns></returns>
        public ActionResult StartMag(string id, string macgroup)
        {
            ArmsWeb.Models.StartMagModel m = new Models.StartMagModel(int.Parse(id), macgroup);
            return View(m);
        }

        /// <summary>
        /// 設備リスト一覧取得用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carrierNo"></param>
        /// <param name="macgroup"></param>
        /// <returns></returns>
        public ActionResult MachineList()
        {
            ArmsWeb.Models.MachineListModel m = new Models.MachineListModel();
            return View(m);
        }

        /// <summary>
        /// コンベアリスト一覧取得用
        /// </summary>
        /// <param name="id"></param>
        /// <param name="carrierNo"></param>
        /// <param name="macgroup"></param>
        /// <returns></returns>
        public ActionResult ConveyorLotList(string id, string num)
        {
            //error = true;
            ArmsWeb.Models.ConveyorLotListModel m = new Models.ConveyorLotListModel(id, int.Parse(num));
            return View(m);
        }

    }
}
