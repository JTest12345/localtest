using ArmsApi.Model;
using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class ResinMeasurementTrayCompareController : Controller
    {
        public ActionResult Index()
        {
            ResinMeasurementTrayCompareModel m = Session["model"] as ResinMeasurementTrayCompareModel;
            if (m == null)
            {
                string plantcd = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(plantcd))
                {
                    Session["redirectController"] = "ResinMeasurementTrayCompare";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new ResinMeasurementTrayCompareModel(plantcd);
                Session["model"] = m;
            }

            return View(m);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtTrayNo, FormCollection fc)
        {
            ResinMeasurementTrayCompareModel m = Session["model"] as ResinMeasurementTrayCompareModel;

            if (!string.IsNullOrEmpty(txtTrayNo))
            {
                string plantcd = txtTrayNo.Trim();

                if (plantcd.StartsWith(ArmsApi.Config.MAC_BARCODE_HEADER) == false
                && plantcd.StartsWith(ArmsApi.Config.MAC_COMP_BARCODE_HEADER) == false)
                {
                    TempData["AlertMsg"] = "バーコードヘッダー異常（07, 36)";
                    return View(m);
                }

                plantcd = plantcd.Replace(ArmsApi.Config.MAC_BARCODE_HEADER, string.Empty);
                plantcd = plantcd.Replace(ArmsApi.Config.MAC_COMP_BARCODE_HEADER, string.Empty);

                m.TrayNo = plantcd;
            }
            else
            {
                TempData["AlertMsg"] = "トレイのラベルを読み込んでください";
                return View(m);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            //キャンセル時は設備番号読み込み画面へ遷移
            Session["model"] = null;
            Session["plantcd"] = null;

            Session["redirectController"] = "ResinMeasurementTrayCompare";
            Session["redirectAction"] = "Index";
            return RedirectToAction("InputPlantCd", "Home");
        }

        public ActionResult Compare()
        {
            ResinMeasurementTrayCompareModel m = Session["model"] as ResinMeasurementTrayCompareModel;

            if (string.IsNullOrEmpty(m.TrayNo))
            {
                TempData["AlertMsg"] = "トレイのラベルを読み込んでください";
                return View(m);
            }

            m.Compare();

            string msg = m.Msg;

            MachineInfo machine = MachineInfo.GetMachine(m.PlantCd);
            if (machine == null)
            {
                msg += $"装置マスタに対象の装置が見つかりません。設備番号：{m.PlantCd}";
            }
            else
            {
                if (string.IsNullOrWhiteSpace(machine.VerificationLogOutputDirectoryPath) == true)
                {
                    msg += $"装置マスタに照合結果ログ出力先フォルダの設定(verificationlogoutputdirectorypath)がされていません。";
                }
                else
                {
                    //照合結果ファイル出力
                    outputCompareResultFile(machine.VerificationLogOutputDirectoryPath, m.PlantCd, m.IsOK);
                }
            }

            return RedirectToAction("Message", "Home", new { msg = msg });
        }

        private void outputCompareResultFile(string pathNm, string plantCd, bool isOk)
        {
            if (Directory.Exists(pathNm) == false)
            {
                Directory.CreateDirectory(pathNm);
            }

            string fileNm = DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + plantCd + ".txt";

            StreamWriter sw = new StreamWriter(Path.Combine(pathNm, fileNm), true, Encoding.ASCII);
            string writeVal = "NG";
            if (isOk)
            {
                writeVal = "OK";
            }
            sw.Write(writeVal);
            sw.Close();
        }
    }
}