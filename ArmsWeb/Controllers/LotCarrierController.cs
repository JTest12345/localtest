using ArmsApi;
using ArmsWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ArmsWeb.Controllers
{
    public class LotCarrierController : Controller
    {
		public ActionResult Index()
		{
			LotCarrierModel m = Session["model"] as LotCarrierModel;
			if (m == null)
			{
				string empcd = (string)Session["empcd"];
				if (string.IsNullOrEmpty(empcd))
				{
					//社員番号読み込み
					Session["redirectController"] = "LotCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputEmpCd", "Home");
				}
				m = new LotCarrierModel(empcd);
				Session["model"] = m;
			}
		
			if (string.IsNullOrEmpty(m.LotNo))
			{
				string lotno = (string)Session["lotno"];
				if (string.IsNullOrEmpty(lotno))
				{
					//ロット番号読み込み
					Session["redirectController"] = "LotCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputLotNo", "Home");
				}
				m.LotNo = lotno;
				//m = new LotCarrierModel(lotno);
				Session["model"] = m;
			}
		
			
			/*if (string.IsNullOrEmpty(m.MagazineNo))
			{
				string magazineno = (string)Session["magazineno"];
				if (string.IsNullOrEmpty(magazineno))
				{
					//マガジン番号読み込み
					Session["redirectController"] = "LotCarrier";
					Session["redirectAction"] = "Index";
					return RedirectToAction("InputMagazineNo", "Home");
				}

				m.MagazineNo = magazineno;
				Session["model"] = m;
			}*/
			
            Session["model"] = m;
            return View(m);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(string txtDataMatrix, FormCollection fc)
        {
            LotCarrierModel m = Session["model"] as LotCarrierModel;

            //基板DM読み込み
            if (!string.IsNullOrEmpty(txtDataMatrix))
            {
                string datamatrix = txtDataMatrix;

                //重複読込チェック
                if (m.DataMatirxList.Where(dm => dm == datamatrix).Count() >= 1)
                {
                    TempData["AlertMsg"] = "既に読み込み済みの基板DM/キャリアQRです";
                    return View(m);
                }

                m.DataMatirxList.Add(datamatrix);
            }
            else
            {
                TempData["AlertMsg"] = "基板DM/キャリアQRを読み込んでください";
                return View(m);
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult CancelEdit()
        {
            //キャンセル時はロット番号読み込み画面へ遷移
            Session["model"] = null;
            return RedirectToAction("InputLotNo", "Home");
        }

        public ActionResult Confirm()
        {
            LotCarrierModel m = Session["model"] as LotCarrierModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "LotCarrierModelが見つかりません" });
            }

			try
			{
				//指定したロットで稼働中のデータがあるか確認
				m.OperationMagazine = m.GetMagazine();
				//マガジンNo保存
				m.MagazineNo = m.OperationMagazine.MagazineNo;
			}
			catch (Exception ex)
			{
				return RedirectToAction("Message", "Home", new { msg = ex.Message });
			}

			if (!m.ConfirmedMagazineFg)
            {
                //指定したマガジンが別ロットで稼働中の場合、確認メッセージを表示する
                //確認OKの場合は別ロットの稼働中をOFF
                m.CurrentMagazine = m.GetCurrentMagazine();

                if (m.CurrentMagazine != null)
                {
                    Session["model"] = m;
                    return RedirectToAction("ConfirmMagazine", "LotCarrier");
                }
            }

            //基板の厚みランクが異なるデータが読み込まれている場合はエラーとする
            if (Config.Settings.IsFrameThicknessRankCheck)
            {
                try
                {
                    string msg = string.Empty;
                    if (!LotCarrierModel.CheckSubstrateThicknessRank(out msg, m.DataMatirxList))
                    {
                        return RedirectToAction("Message", "Home", new { msg = "ロットと基板DM/キャリアQRの関連付け登録でエラー：基板の厚みランクが異なります。" + msg });
                    }
                }
                catch (Exception ex)
                {
                    return RedirectToAction("Message", "Home", new { msg = "ロットと基板DM/キャリアQRの関連付け登録でエラー：" + ex.Message });
                }

            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult Confirmed()
        {
            LotCarrierModel m = Session["model"] as LotCarrierModel;

            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "LotCarrierModelが見つかりません" });
            }

            try
            {
				bool exsit = m.MappingConfilm();
				//マッピングデータ取込有無確認
				if (exsit)
				{
					//マッピング開始工程取得
					int? procNo = m.getMappingStartProc();

					if (procNo == null)
					{
                        return RedirectToAction("Message", "Home", new { msg = "先頭マッピング工程が取得できません：" + m.LotNo });
					}

                    //マッピングデータ割り付け
                    m.ImportMapping(Convert.ToInt32(procNo));
                }
                else if (ArmsApi.Config.Settings.CreateAllOKMappingForMainte == true)
                {
                    //マッピング開始工程取得
                    int? procNo = m.getMappingStartProc();

                    if (procNo == null)
                    {
                        return RedirectToAction("Message", "Home", new { msg = "先頭マッピング工程が取得できません：" + m.LotNo });
                    }

                    //マッピングデータ割り付け
                    m.CreateAllOKMapping(Convert.ToInt32(procNo));
                }
                m.Insert();
            }
            catch (Exception ex)
            {
                return RedirectToAction("Message", "Home", new { msg = "ロットと基板DM/キャリアQRの関連付け登録で予期せぬエラー：" + ex.Message });
            }

            return RedirectToAction("Message", "Home", new { msg = "ロットと基板DM/キャリアQRの関連付け登録が完了しました" });
        }

        public ActionResult ConfirmMagazine()
        {
            LotCarrierModel m = Session["model"] as LotCarrierModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "LotCarrierModelが見つかりません" });
            }

            Session["model"] = m;
            return View(m);
        }

        public ActionResult ConfirmedMagazine()
        {
            LotCarrierModel m = Session["model"] as LotCarrierModel;
            if (m == null)
            {
                return RedirectToAction("Message", "Home", new { msg = "LotCarrierModelが見つかりません" });
            }

            m.ConfirmedMagazineFg = true;

            return RedirectToAction("Confirm", "LotCarrier");
        }
    }
}