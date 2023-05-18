using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ArmsWeb.Models;

namespace ArmsWeb.Controllers
{
    public class DefectController : Controller
    {
        //
        // GET: /Defect/
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

            DefectEditModel m = Session["model"] as DefectEditModel;
            if (m == null)
            {
                string code = (string)Session["plantcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "Defect";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputPlantCd", "Home");
                }

                m = new DefectEditModel(code);
                Session["model"] = m;
            }

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string code = (string)Session["empcd"];
                if (string.IsNullOrEmpty(code))
                {
                    Session["redirectController"] = "Defect";
                    Session["redirectAction"] = "Index";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = code;
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

        public ActionResult InputQty(int? orderindex, bool? isInspection)
        {
            DefectEditModel m = Session["model"] as DefectEditModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            if (isInspection.HasValue)
            {
                m.IsInspection = isInspection.Value;
            }
            if (orderindex.HasValue)
            {
                m.EditTarget = m.Orders[orderindex.Value];
            }

            if (m.EditTarget == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "検査対象の指図が選択されていません" });
            }
            
            //カット完了画面からの移行の場合は先にタイプコードがセットされている(AsmLotは存在しない)
            if (m.TypeCd == null)
            {
                m.TypeCd = ArmsApi.Model.AsmLot.GetAsmLot(m.EditTarget.NascaLotNo).TypeCd;
            }

            Session["model"] = m;

            if (string.IsNullOrEmpty(m.EmpCd))
            {
                string empcd = (string)Session["empcd"];
                if (string.IsNullOrEmpty(empcd))
                {
                    Session["redirectController"] = "Defect";
                    Session["redirectAction"] = "InputQty";
                    return RedirectToAction("InputEmpCd", "Home");
                }

                m.EmpCd = empcd;
                Session["model"] = m;
            }

            #region 検査数入力テキストボックスの初期値設定
            
            if (m.EditTarget.InspectCt < 0)
            {
                m.InitialInspectCtStr = "";
            }
            else if (m.EditTarget.InspectCt > 0)
            {
                m.InitialInspectCtStr = m.EditTarget.InspectCt.ToString();
            }
            else if (m.EditTarget.InspectCt == 0)
            {
                int? defaultct = ArmsApi.Model.Defect.GetDefaultInspectCtMaster(m.TypeCd, m.EditTarget.ProcNo);
                if (defaultct.HasValue)
                {
                    m.InitialInspectCtStr = defaultct.Value.ToString();
                }
                else
                {
                    m.InitialInspectCtStr = "";
                }
            }
            #endregion

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Select(string inspectkb, string inspectct, string filter)
        {
            DefectEditModel m = Session["model"] as DefectEditModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "引き継ぎデータが見つかりません" });
            }

            m.Filter = filter;
            if (!string.IsNullOrEmpty(inspectkb))
            {
                if (inspectkb == "all")
                {
                    m.EditTarget.InspectCt = -1;
                }
                else
                {
                    int ct;
                    if (int.TryParse(inspectct, out ct) == false)
                    {
                        TempData["AlertMsg"] = "検査数は数字を入力してください";
                        return RedirectToAction("InputQty");
                    }
                    m.EditTarget.InspectCt = ct;
                }
                //この時点で検査者は更新してしまう
                m.UpdateEmpCdAndInspectCt();
                m.UpdateInspectionFg();
                Session["model"] = m;
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult InputDef(string classcd, string causecd, string defectcd, string qty)
        {
            DefectEditModel m = Session["model"] as DefectEditModel;
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
            DefectEditModel m = Session["model"] as DefectEditModel;
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
            ArmsApi.Model.Defect defect = new ArmsApi.Model.Defect();
            defect.LotNo = m.EditTarget.NascaLotNo;
            defect.DefItems = new List<ArmsApi.Model.DefItem>(defs);

            if (m.IsCutPreInputDef)
            {
                defect.ProcNo = m.GetFinalProcNo();
            }
            else
            {
                defect.ProcNo = m.EditTarget.ProcNo;
            }
            defect.DeleteInsert();

            //ブレンドされているロット、かつ最終工程以降の工程の場合、ブレンドロットの不良も更新する
            if (string.IsNullOrWhiteSpace(m.BlendLotNo) == false)
            {
                ArmsApi.Model.Defect defP = new ArmsApi.Model.Defect();
                defP.LotNo = m.BlendLotNo;
                defP.ProcNo = m.EditTarget.ProcNo;
                defP.DefItems = new List<ArmsApi.Model.DefItem>();

                foreach (string blendChildLotNo in m.BlendChildLotList)
                {
                    ArmsApi.Model.DefItem[] childDefs = ArmsApi.Model.Defect.GetAllDefect(blendChildLotNo, m.TypeCd, m.EditTarget.ProcNo);

                    foreach (ArmsApi.Model.DefItem childDef in childDefs)
                    {
                        ArmsApi.Model.DefItem d = defP.DefItems.Where(r => r.CauseCd == childDef.CauseCd &&
                                                r.ClassCd == childDef.ClassCd &&
                                                r.DefectCd == childDef.DefectCd).FirstOrDefault();
                        if (d == null)
                        {
                            d = new ArmsApi.Model.DefItem();
                            d.CauseCd = childDef.CauseCd;
                            d.ClassCd = childDef.ClassCd;
                            d.DefectCd = childDef.DefectCd;
                            d.DefectCt = childDef.DefectCt;

                            defP.DefItems.Add(d);
                        }
                        else
                        {
                            d.DefectCt += childDef.DefectCt;
                        }
                    }
                }

                defP.DeleteInsert();
            }


            Session["empcd"] = "";
            return View("Select", m);
        }

        public ActionResult ReturnCutBlend()
        {
            Session["model"] = Session["cutBlendModel"];
            return RedirectToAction("Finish", "CutBlend");
        }
    }
}
