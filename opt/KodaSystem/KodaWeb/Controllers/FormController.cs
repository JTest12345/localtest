using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data;

using Newtonsoft.Json;

using KodaWeb.Models;

using KodaClassLibrary;
using static KodaClassLibrary.UtilFuncs;
using KodaWeb.Models.WebApi;

namespace KodaWeb.Controllers {

    public class FormController : Controller {

        /// <summary>
        /// データベース接続文字列取得
        /// <para>web.config内の文字列変えた時に自動で取得しなおされる</para>
        /// </summary>
        private static string form_connect_string = WebConfigurationManager.ConnectionStrings["KodaDatabase"].ConnectionString;

        FormsSQL form_sql = new FormsSQL(form_connect_string);

        /// <summary>
        /// 工程帳票マスタへのリンクURL
        /// </summary>
        public const string kodamm_url = "http://ap-koda.cej.citizen.co.jp/kodamm";

        /// <summary>
        /// 製品/工程帳票へのリンクURLのベース部分
        /// </summary>
        public const string kodain_url = "http://ap-koda.cej.citizen.co.jp/kodain/lotinfo/";

        /// <summary>
        /// 定期点検帳票へのリンクURLのベース部分
        /// </summary>
        public const string kodapif_url = "http://ap-koda.cej.citizen.co.jp/kodapif/";


        /// <summary>
        /// 修理記録の画像保管場所フォルダパス
        /// </summary>
        private readonly string rr_image_fld = System.Web.HttpContext.Current.Server.MapPath("~/Images/repair_record");


        /// <summary>
        /// 帳票システムホームページ
        /// </summary>
        public ActionResult Index() {
            ViewBag.FormMasterUrl = kodamm_url;
            return View();
        }

        /// <summary>
        /// 製品帳票入力画面へのリンクさせるためのページ
        /// </summary>
        public ActionResult PFLogin() {
            return View();
        }

        /// <summary>
        /// 製品帳票入力画面へのリンクさせるためのページの[Post]処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PFLogin(PFLogin p) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {
                string link_url = kodain_url;

                string login_user = GetHankaku(p.EmployeeCode).Replace("01 ", "");

                //マガジン番号がある場合
                if (p.MagNo != null) {
                    try {
                        //マガジン番号からLotNo取得
                        p.LotNo18 = UseArmsApi.Get_LotNo18(p.MagNo);

                        //製品/工程帳票データテーブルから取得
                        var pf_new = form_sql.Get_FormTran_from_Lotno18<PFLogin>(p.LotNo18);

                        link_url += $"{login_user}::{pf_new.Typecd}::{pf_new.LotNo18}::{pf_new.ProcNo}::{pf_new.Plantcd}";
                        return Redirect(link_url);
                    }
                    catch (Exception ex) {
                        ViewBag.ErrorMsg = ex.Message;
                        return View(p);
                    }
                }
                //18桁ロット番号がある場合
                else if (p.LotNo18 != null) {
                    try {
                        var pf_new = form_sql.Get_FormTran_from_Lotno18<PFLogin>(p.LotNo18);

                        link_url += $"{login_user}::{pf_new.Typecd}::{pf_new.LotNo18}::{pf_new.ProcNo}::{pf_new.Plantcd}";
                        return Redirect(link_url);
                    }
                    catch (Exception ex) {
                        ViewBag.ErrorMsg = ex.Message;
                        return View(p);
                    }
                }
                //設備番号がある場合
                else if (p.Plantcd != null) {
                    try {
                        //設備内マガジンが分かるリストを取得
                        var vmags = UseArmsApi.Get_MagNo_from_plantcd(p.Plantcd);

                        var dic = new Dictionary<string, string>();
                        foreach (var m in vmags) {

                            p.MagNo = m.MagazineNo;

                            //マガジン番号からLotNo取得
                            try {
                                p.LotNo18 = UseArmsApi.Get_LotNo18(p.MagNo);
                            }
                            catch (Exception e) {
                                ViewBag.ErrorMsg += $"マガジン番号 {p.MagNo}：{e.Message}\n";
                                continue;
                            }

                            //製品/工程帳票データテーブルから取得
                            PFLogin pf_new;
                            try {
                                pf_new = form_sql.Get_FormTran_from_Lotno18<PFLogin>(p.LotNo18);
                            }
                            catch (Exception e) {
                                ViewBag.ErrorMsg += $"ロット番号 {p.LotNo18}：{e.Message}\n";
                                continue;
                            }


                            var link = $"{link_url}" +
                                $"{login_user}::{pf_new.Typecd}::{pf_new.LotNo18}::{pf_new.ProcNo}::{pf_new.Plantcd}";

                            string key = link;
                            string val = $"機種：{pf_new.Typecd},LotNo：{pf_new.LotNo18}," +
                                $"設備番号：{pf_new.Plantcd},マガジン番号：{p.MagNo}," +
                                $"作業開始時間：{((DateTime)m.WorkStart).ToString("yyyy/MM/dd HH:mm:ss")}";

                            dic.Add(key, val);
                        }

                        //帳票入力するLotが無い場合は、そのまま戻す
                        if (dic.Count == 0) {
                            return View(p);
                        }

                        ViewBag.LinkUrlDic = dic;
                        return View(p);
                    }
                    catch (Exception ex) {
                        ViewBag.ErrorMsg = ex.Message;
                        return View(p);
                    }
                }
                //作業単位ロットがある場合
                else if (p.WorkUnitID != null) {
                    try {
                        var pfl_list = form_sql.Get_FormTranList_from_WorkUnitID<PFLogin>(p.WorkUnitID);

                        //作業単位ロットのリストを作成（重複無し）
                        var wui_list = new List<string>();
                        foreach (var pfl in pfl_list) {
                            if (!wui_list.Contains(pfl.WorkUnitID)) {
                                wui_list.Add(pfl.WorkUnitID);
                            }
                        }

                        var url_dic = new Dictionary<string, string>();
                        foreach (var wui in wui_list) {

                            var pfls = pfl_list.Where(x => x.WorkUnitID == wui).ToList<PFLogin>();

                            int procno = pfls[0].ProcNo;
                            string plantcd = pfls[0].Plantcd;

                            string val = $"作業単位ロット：{wui},";
                            foreach (var pfl in pfls) {
                                val += $"機種：{pfl.Typecd}　LotNo：{pfl.LotNo18},";
                            }

                            val = val.Trim(',');

                            var link = $"{link_url}" +
                               $"{login_user}::all::wuid::{procno}::{plantcd}::{wui}";

                            url_dic.Add(link, val);
                        }

                        ViewBag.LinkUrlDic = url_dic;
                        return View(p);

                    }
                    catch (Exception ex) {
                        ViewBag.ErrorMsg = ex.Message;
                        return View(p);
                    }
                }

            }

            return View(p);
        }

        /// <summary>
        /// 帳票未完了ロット一覧ページ
        /// <para>新システム使用機種のみに対応している</para>
        /// </summary>
        /// <returns></returns>
        public ActionResult NotClosePFList() {

            //未完了帳票リスト
            var list = form_sql.Get_NotClosePFList<FormsSQL.TnFormTranData>();

            var lotinfo_list = new List<ApiLotInfo>();

            //ロット情報取得
            foreach (var lotno in list.Select(x => x.LotNo18).ToList()) {
                //18桁ロットからロット情報取得
                var lotinfo = ApiLotInfo.Get_LotInfo_from_LotNo18(lotno);

                //対象のロット番号の帳票未完了の工程番号取得
                int procno = list.Where(x => x.LotNo18 == lotno).First().ProcNo;

                foreach (var proc in new List<LotInfo.Process> { lotinfo.PreProcess, lotinfo.NowProcess, lotinfo.NextProcess }) {
                    if (proc != null) {
                        if (proc.ProcessNo == procno) {
                            proc.ClosePF = false;
                        }
                        else {
                            proc.ClosePF = true;
                        }
                    }
                }

                lotinfo_list.Add(lotinfo);
            }

            return View(lotinfo_list.OrderBy(x=>x.PreProcess.ProcessNo).ToList());
        }


        /// <summary>
        /// 帳票データ検索ページ
        /// </summary>
        public ActionResult GetPFD() {

            var pfds = new PFDSearch();
            var now = DateTime.Now;
            pfds.Start = now.AddMonths(-1).ToString("yyyy-MM-dd");
            pfds.End = now.ToString("yyyy-MM-dd");

            //データベースから帳票一覧取得
            var form_master_list = form_sql.Get_FormMasterList<FormInfo>();

            //製品/工程帳票のリストだけにする
            form_master_list = form_master_list.Where(m => m.Usefor == "PRO").ToList();

            //ビューに渡す為にViewBagに設定
            ViewBag.FormList = pfds.create_form_items(form_master_list);

            return View(pfds);
        }

        /// <summary>
        /// 帳票データ検索ページの[Post]処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPFD(PFDSearch pfds) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {

                try {
                    //データベースから結果取得
                    pfds.Table = FormData.Get_PFD(form_sql, pfds.FormInfo, pfds.Start, pfds.End);

                    //別のアクションに渡す為、一旦TempDataに格納
                    TempData["SearchResult"] = pfds;

                    return RedirectToAction("ViewPFD");
                }
                catch (Exception ex) {
                    TempData["msg"] = ex.Message;
                    return RedirectToAction("GetPFD");
                }
            }

            //データベースから帳票一覧取得
            var form_master_list = form_sql.Get_FormMasterList<FormInfo>();

            //製品/工程帳票のリストだけにする
            form_master_list = form_master_list.Where(m => m.Usefor == "PRO").ToList();

            //ビューに渡す為にViewBagに設定
            ViewBag.FormList = pfds.create_form_items(form_master_list);

            return View(pfds);

        }

        /// <summary>
        /// 帳票データ検索結果表示ページ
        /// </summary>
        public ActionResult ViewPFD() {
            var pfds = (PFDSearch)TempData["SearchResult"];

            //ダウンロード用のcsvテキスト作成
            var table = pfds.Table;

            //ヘッダリスト作成
            var header_list = new List<string>();
            foreach (DataColumn col in table.Columns) {
                header_list.Add(col.ColumnName);
            }

            string csv_text = "";
            csv_text += $"帳票名,{pfds.FormInfo.FormName}\n";
            csv_text += $"帳票番号,{pfds.FormInfo.FormNo},{pfds.FormInfo.FormRev.ToString()}版\n";
            csv_text += $"期間,{pfds.Start.Replace("-", "/")}～{pfds.End.Replace("-", "/")}\n";
            csv_text += "\n";

            //ヘッダ文字列作成
            string h1 = "";
            string h2 = "";
            foreach (string name in header_list) {
                //\nで分割
                string[] del = { "\n" };
                var array = name.Split(del, StringSplitOptions.RemoveEmptyEntries);

                if (array.Length == 1) {
                    h1 += ",";
                    h2 += $"{array[0]},";
                }
                if (array.Length == 2) {
                    h1 += $"{array[0]},";
                    h2 += $"{array[1]},";
                }
            }
            h1.Trim(',');
            h2.Trim(',');

            csv_text += $"{h1}\n{h2}\n";

            //データ文字列作成
            foreach (DataRow row in table.Rows) {

                var data_list = new List<string>();

                foreach (string col in header_list) {
                    string data = row[col].ToString();
                    data_list.Add(data);
                }

                csv_text += string.Join(",", data_list) + "\n";
            }

            ViewBag.CsvText = csv_text;

            ViewBag.CsvName = $"{pfds.FormInfo.FormName}_{pfds.FormInfo.FormNo}_rev{pfds.FormInfo.FormRev.ToString()}_{pfds.Start}_to_{pfds.End}.csv";

            return View(pfds);
        }

        /// <summary>
        /// 製品/工程帳票修正用の画面に移動させる
        /// </summary>
        public ActionResult ModifyPF(string typecd, string lotno, string procno, string plantcd, string workunitid, string empcd) {

            string link_url = kodain_url;
            if (string.IsNullOrEmpty(workunitid)) {
                link_url += $"{empcd}::{typecd}::{lotno}::{procno}::{plantcd}::::admin";
            }
            else {
                link_url += $"{empcd}::all::wuid::{procno}::{plantcd}::{workunitid}::admin";
            }

            return Redirect(link_url);

        }


        /// <summary>
        /// 製品/工程帳票作成ページ
        /// <para>旧システム使用の為、帳票が自動で挿入されない機種に使う(全て新システム使用になればいらない)</para>
        /// </summary>
        public ActionResult CreatePFTran() {

            var pf = new PFforOldSystem();

            //データベースから帳票一覧取得
            var form_master_list = form_sql.Get_FormMasterList<FormInfo>(true);

            //製品/工程帳票のリストだけにする
            form_master_list = form_master_list.Where(m => m.Usefor == "PRO").ToList();

            //ビューに渡す為にViewBagに設定
            ViewBag.FormList = pf.create_form_items(form_master_list);

            //ビューに渡す為にViewBagに製造拠点一覧を設定
            ViewBag.ManubaseList = pf.create_manubase_items();

            return View(pf);
        }

        /// <summary>
        /// 製品/工程帳票作成ページのPOST処理
        /// <para>旧システム使用の為、帳票が自動で挿入されない機種に使う(全て新システム使用になればいらない)</para>
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreatePFTran(PFforOldSystem pf) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {

                var fm_info = form_sql.Get_FormMaster<FormInfo>(pf.FormNo, pf.FormRev);
                pf.Workcd = fm_info.FormNo.Split('-')[1];
                pf.COJ = fm_info.COJ;

                var tran_list = form_sql.Get_OldSystePFList<PFforOldSystem>(pf.Typecd, pf.LotNo18);
                if (tran_list.Count == 0) {
                    pf.ProcNo = -1;
                }
                else {
                    int procno = tran_list.Min(x => x.ProcNo);
                    pf.ProcNo = procno - 1;
                }

                pf.InsertAt = DateTime.Now;
                pf.InsertBy = "KODA";
                form_sql.Insert_PF(pf);

                return RedirectToAction("OldSystemPFList");
            }

            //データベースから帳票一覧取得
            var form_master_list = form_sql.Get_FormMasterList<FormInfo>(true);

            //製品/工程帳票のリストだけにする
            form_master_list = form_master_list.Where(m => m.Usefor == "PRO").ToList();

            //ビューに渡す為にViewBagに設定
            ViewBag.FormList = pf.create_form_items(form_master_list);

            //ビューに渡す為にViewBagに製造拠点一覧を設定
            ViewBag.ManubaseList = pf.create_manubase_items();

            return View(pf);
        }

        /// <summary>
        /// 製品/工程帳票リスト選択ページ
        /// <para>旧システム使用の為、帳票が自動で挿入されない機種に使う(全て新システム使用になればいらない)</para>
        /// </summary>
        public ActionResult OldSystemPFList() {

            var fm_list = form_sql.Get_FormMasterList<FormInfo>(true);
            var pf_list = form_sql.Get_OldSystemNotClosePFList<PFforOldSystem>();

            foreach (var li in pf_list) {
                li.LinkUrl = $"{kodain_url}login_user::{li.Typecd}::{li.LotNo18}::{li.ProcNo}::{li.Plantcd}";
                li.FormName = fm_list.Where(x => x.FormNo == li.FormNo).First().FormName;
            }

            ViewBag.Msg = TempData["msg"];
            ViewBag.ErrMsg = TempData["errmsg"];
            return View(pf_list);
        }

        /// <summary>
        /// 旧システム使用の対象機種/ロット/工程番号の製品/工程帳票を消す
        /// <para>間違って何回も登録してしまうことがあるため</para>
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="lotno"></param>
        /// <param name="procno"></param>
        /// <param name="empcd"></param>
        /// <returns></returns>
        public ActionResult OldSystemDeletePF(string typecd, string lotno, int procno, string empcd) {

            string login_user = GetHankaku(empcd).Replace("01 ", "");

            try {
                var level = form_sql.Get_UserLevel<FormsSQL.TmLevelData>(login_user);

                if (level.FormsLevel > 2) {

                    form_sql.Delete_NotClosePF(typecd, lotno, procno);

                    TempData["msg"] = $"削除しました。";
                    return Redirect("OldSystemPFList");
                }
                else {
                    throw new Exception("削除出来る権限がありません。");
                }
            }
            catch (Exception ex) {
                TempData["errmsg"] = $"削除失敗しました。{ex.Message}";
                return Redirect("OldSystemPFList");
            }
        }



        /// <summary>
        /// 定期点検登録ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult RegisterPI() {

            //データベースから帳票一覧取得
            var form_master_list = form_sql.Get_FormMasterList<FormInfo>(latest_only: true);

            //定期点検用のリストだけにする
            form_master_list = form_master_list.Where(m => m.Usefor == "PI").ToList();

            //ビューに渡す為にViewBagに帳票一覧を設定
            ViewBag.FormList = PeriodicInspection.create_form_items(form_master_list);

            //ビューに渡す為にViewBagに点検期間一覧を設定
            ViewBag.PeriodList = PeriodicInspection.period_items;

            //ビューに渡す為にViewBagに製造拠点一覧を設定
            ViewBag.ManubaseList = PeriodicInspection.manubase_items;

            return View();
        }

        /// <summary>
        /// 定期点検登録ページの[Post]処理
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterPI(PeriodicInspection pi) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {

                bool set_ok = pi.Set_DateTime();
                if (set_ok) {

                    try {

                        var list = form_sql.Get_NotClosePI<PeriodicInspection>();
                        int cnt = list.Count(x => x.Plantcd == pi.Plantcd && x.FormNo == pi.FormNo && x.Period == pi.Period && x.Next == pi.Next);

                        if (cnt > 0) {
                            throw new Exception("既に同じ内容が登録されています。");
                        }

                        //定期点検テーブルに登録する
                        form_sql.Insert_PI(pi);

                        ViewBag.Finish = "登録完了しました。";

                    }
                    catch (Exception ex) {
                        ViewBag.ErrorMsg = $"登録失敗しました。\n{ex.Message}";
                    }
                }
                else {
                    ViewBag.ErrorMsg = "期限設定に失敗した為、登録出来ませんでした。";
                }
            }

            //データベースから帳票一覧取得
            var form_master_list = form_sql.Get_FormMasterList<FormInfo>(latest_only: true);

            //定期点検用のリストだけにする
            form_master_list = form_master_list.Where(m => m.Usefor == "PI").ToList();

            //ビューに渡す為にViewBagに帳票一覧を設定
            ViewBag.FormList = PeriodicInspection.create_form_items(form_master_list);

            //ビューに渡す為にViewBagに点検期間一覧を設定
            ViewBag.PeriodList = PeriodicInspection.period_items;

            //ビューに渡す為にViewBagに製造拠点一覧を設定
            ViewBag.ManubaseList = PeriodicInspection.manubase_items;

            return View(pi);
        }


        /// <summary>
        /// 未実施定期点検一覧ページ
        /// </summary>
        public ActionResult PIList() {

            var list = form_sql.Get_NotClosePI<PeriodicInspection>();

            ViewBag.Msg = TempData["msg"];
            ViewBag.PI_INPUT_URL = kodapif_url;

            return View(list);
        }

        /// <summary>
        /// 未実施定期点検の入力画面にリンクさせる
        /// </summary>
        public ActionResult PIInput(int id, string empcd) {

            string login_user = GetHankaku(empcd).Replace("01 ", "");

            var link_url = $"{kodapif_url}{login_user}::{id}";
            return Redirect(link_url);
        }

        /// <summary>
        /// 未実施定期点検削除ページ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public ActionResult PIDelete(int id, string empcd) {

            //社員コードチェック
            try {
                string login_user = GetHankaku(empcd).Replace("01 ", "");
                var level = form_sql.Get_UserLevel<FormsSQL.TmLevelData>(login_user);

                if (level.FormsLevel < 3) {
                    throw new Exception("削除出来る権限がありません。");
                }
                var pi = form_sql.Get_NotClosePI<PeriodicInspection>(id);
                pi.EmployeeCode = login_user;
                return View(pi);

            }
            catch (Exception ex) {
                //別のアクションに渡す為、一旦TempDataに格納
                TempData["msg"] = $"{ex.Message}\nID={id}の削除ページに移動出来ません。";
                return RedirectToAction("PIList");
            }
        }

        /// <summary>
        /// 未実施定期点検削除ページのPOST処理
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PIDelete(PeriodicInspection pi) {

            try {
                form_sql.Delete_PI(pi.ID, pi.EmployeeCode);
                TempData["msg"] = $"ID:{pi.ID}の帳票を削除しました。";
            }
            catch (Exception ex) {
                TempData["msg"] = ex.Message;
            }
            return RedirectToAction("PIList");
        }

        /// <summary>
        /// 定期点検帳票データ検索ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult GetPIFD() {

            //ビューに渡す為にViewBagに点検期間一覧を設定
            ViewBag.PeriodList = PeriodicInspection.period_items;

            //ビューに渡す為にViewBagに製造拠点一覧を設定
            ViewBag.ManubaseList = PeriodicInspection.manubase_items;

            var pi = new PeriodicInspection();
            ViewBag.Msg = TempData["msg"];
            return View(pi);
        }

        /// <summary>
        /// 定期点検帳票データ検索ページのPOST処理
        /// </summary>
        /// <param name="pi"></param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetPIFD(PeriodicInspection pi) {

            if (pi.FormNo == null) {

                try {

                    var list = form_sql.Get_PIFormList<PeriodicInspection>(pi.ManuBase, pi.Period);

                    //FormNoとFormRevでグループ化して、各グループの最初のものだけを取得
                    //FormNoとFormRevの重複しない組み合わせが欲しい
                    var group_list = list.GroupBy(x => new { x.FormNo, x.FormRev }).Select(g => g.First());


                    //ドロップダウンリスト用のリスト作成
                    var drop_list = new List<SelectListItem>();
                    drop_list.Add(new SelectListItem { Text = "", Value = "" });
                    foreach (var item in group_list) {

                        var fi = form_sql.Get_FormMaster<FormInfo>(item.FormNo, (int)item.FormRev);

                        string text = $"{fi.FormName}:{fi.FormNo}:rev{fi.FormRev.ToString()} 使用開始：{((DateTime)fi.UpdateAt).ToString("yyyy/MM/dd HH:mm:ss")}";
                        string val = $"{fi.FormName},{fi.FormNo},{fi.FormRev}";

                        drop_list.Add(new SelectListItem { Text = text, Value = val });

                    }

                    //ビューに渡す為にViewBagに帳票一覧を設定
                    ViewBag.FormList = (IEnumerable<SelectListItem>)drop_list;
                    return View(pi);
                }
                catch (Exception ex) {
                    TempData["msg"] = ex.Message;
                    return RedirectToAction("GetPIFD");
                }
            }
            else {
                //データ取得
                var table = FormData.Get_PIFD(form_sql, pi);
                TempData["Table"] = table;
                TempData["PI"] = pi;
                return RedirectToAction("ViewPIFD");
            }

        }

        /// <summary>
        /// 定期点検帳票データ検索結果表示ページ
        /// </summary>
        /// <returns></returns>
        public ActionResult ViewPIFD() {
            var pi = (PeriodicInspection)TempData["PI"];
            var table = (DataTable)TempData["Table"];
            ViewBag.DataTable = table;

            //ダウンロード用のcsvテキスト作成
            //ヘッダリスト作成
            var header_list = new List<string>();
            foreach (DataColumn col in table.Columns) {
                header_list.Add(col.ColumnName);
            }

            string csv_text = "";
            csv_text += $"帳票名,{pi.FormName}\n";
            csv_text += $"帳票番号,{pi.FormNo},{pi.FormRev.ToString()}版\n";
            csv_text += $"製造拠点,{pi.ManuBase}\n";
            csv_text += $"点検周期,{PeriodicInspection.period_dic[pi.Period]}\n";
            csv_text += "\n";

            //ヘッダ文字列作成
            string h1 = "";
            string h2 = "";
            foreach (string name in header_list) {
                //\nで分割
                string[] del = { "\n" };
                var array = name.Split(del, StringSplitOptions.RemoveEmptyEntries);

                if (array.Length == 1) {
                    h1 += ",";
                    h2 += $"{array[0]},";
                }
                if (array.Length == 2) {
                    h1 += $"{array[0]},";
                    h2 += $"{array[1]},";
                }
            }
            h1.Trim(',');
            h2.Trim(',');

            csv_text += $"{h1}\n{h2}\n";

            //データ文字列作成
            foreach (DataRow row in table.Rows) {

                var data_list = new List<string>();

                foreach (string col in header_list) {
                    string data = row[col].ToString();
                    data_list.Add(data);
                }

                csv_text += string.Join(",", data_list) + "\n";
            }

            ViewBag.CsvText = csv_text;

            ViewBag.CsvName = $"{pi.FormName}_{pi.FormNo}_rev{pi.FormRev.ToString()}.csv";

            return View(pi);
        }

        /// <summary>
        /// 定期点検帳票修正用の画面に移動させる
        /// </summary>
        public ActionResult ModifyPIF(string form_id, string empcd) {

            string link_url = $"{kodapif_url}{empcd}::{form_id}::admin";

            return Redirect(link_url);
        }


        /// <summary>
        /// 修理記録登録ページ
        /// <para>修理記録登録⇒Register Repair Record</para>
        /// </summary>
        public ActionResult RegisterRR() {
            return View();
        }

        /// <summary>
        /// 修理記録登録ページのPOST処理
        /// <para>修理記録登録⇒Register Repair Record</para>
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegisterRR(RegisterRR rrr) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {
                try {
                    rrr.InsertBy = GetHankaku(rrr.InsertBy).Replace("01 ", "");
                    int id = form_sql.Insert_RR(rrr);

                    //画像保存
                    if (rrr.PostedFiles[0] != null) {

                        var save_fld = $@"{rr_image_fld}\id-{id}\register";
                        //フォルダ無かったら作る
                        if (Directory.Exists(save_fld) == false) {
                            Directory.CreateDirectory(save_fld);
                        }

                        //ファイル保存　ファイル名は1.jpg,2.jpg...とか
                        int i = 1;
                        foreach (var file in rrr.PostedFiles) {
                            var ext = Path.GetExtension(file.FileName);
                            string path = $@"{save_fld}\{i}{ext}";
                            file.SaveAs(path);
                            i += 1;
                        }

                        //フォルダセキュリティ変更
                        UtilFuncs.ChangeDirectoryAccessControl(save_fld);
                    }

                    TempData["msg"] = $"ID={id}で登録完了しました。";

                    //メール送信
                    string mailto_path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/mail-to/repair_record.json");
                    var mailto = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadText(mailto_path));
                    string subject = $"修理記録登録(ID={id})";
                    string mail_text = "新しい修理記録が登録されました。\n\n";
                    mail_text += $"装置：{rrr.MacName}({rrr.Plantcd})\n【不具合】\n{rrr.Failure}\n【備考】\n{rrr.Remarks1}\n\n";
                    mail_text += $@"https://koda.cej.citizen.co.jp/kodaweb/Form/CompleteRR/{id}";
                    Mail.MailSend_from_Koda(mailto, subject, mail_text);

                    return RedirectToAction("RRList");
                }
                catch (Exception ex) {
                    ViewBag.ErrorMsg = ex.Message;
                    return View(rrr);
                }
            }
            return View();
        }

        /// <summary>
        /// 修理記録一覧ページ
        /// </summary>
        public ActionResult RRList() {
            var list = form_sql.Get_RRData<CompleteRR>();

            //ダウンロード用のcsvテキスト作成
            var csv_text = "ID,作業,設備型式,設備番号,件名,登録日時,修理完了\n";

            foreach (var data in list) {
                csv_text += $"{data.ID},";
                csv_text += data.UpdateAt == null ? "入力," : "閲覧,";
                csv_text += $"\"{data.MacName}\",";
                csv_text += $"\"{data.Plantcd}\",";
                csv_text += $"\"{data.Title}\",";
                csv_text += $"{data.InsertAt},";
                csv_text += $"{data.UpdateAt}\n";
            }

            ViewBag.CsvText = csv_text;

            ViewBag.CsvName = $"修理記録一覧_{DateTime.Now.ToString("yyyy_MM_dd")}.csv";

            ViewBag.Msg = TempData["msg"];
            return View(list);
        }

        /// <summary>
        /// 修理記録完了ページ
        /// <para>修理記録完了⇒Complete Repair Record</para>
        /// </summary>
        public ActionResult CompleteRR(int id) {
            var crr = form_sql.Get_RRData<CompleteRR>(id);

            //故障登録時画像ファイルパス設定
            var register_image_fld = $@"{rr_image_fld}\id-{id}\register";
            if (Directory.Exists(register_image_fld)) {
                var files = Directory.GetFiles(register_image_fld);

                crr.RegisterImagePathList = new List<string>();

                foreach (var file in files) {
                    var path = file.Replace(System.Web.HttpContext.Current.Server.MapPath("~/"), "");
                    crr.RegisterImagePathList.Add($"../../{path}");
                }
            }
            //修理完了時画像ファイルパス設定
            var complete_image_fld = $@"{rr_image_fld}\id-{id}\complete";
            if (Directory.Exists(complete_image_fld)) {
                var files = Directory.GetFiles(complete_image_fld);

                crr.CompleteImagePathList = new List<string>();

                foreach (var file in files) {
                    var path = file.Replace(System.Web.HttpContext.Current.Server.MapPath("~/"), "");
                    crr.CompleteImagePathList.Add($"../../{path}");
                }
            }

            return View(crr);
        }

        /// <summary>
        /// 修理記録完了ページのPOST処理
        /// <para>修理記録完了⇒Complete Repair Record</para>
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CompleteRR(CompleteRR crr) {

            //IsValidがfalseだったらmodelの値にエラーがある
            if (ModelState.IsValid) {
                try {
                    crr.UpdateBy = GetHankaku(crr.UpdateBy).Replace("01 ", "");
                    form_sql.Update_RR(crr);
                }
                catch (Exception ex) {
                    ViewBag.ErrorMsg = ex.Message;
                    return View(crr);
                }

                //画像保存
                if (crr.PostedFiles[0] != null) {

                    var save_fld = $@"{rr_image_fld}\id-{crr.ID.ToString()}\complete";
                    //フォルダ無かったら作る
                    if (Directory.Exists(save_fld) == false) {
                        Directory.CreateDirectory(save_fld);
                    }

                    //ファイル保存　ファイル名は1.jpg,2.jpg...とか
                    int i = 1;
                    foreach (var file in crr.PostedFiles) {
                        var ext = Path.GetExtension(file.FileName);
                        string path = $@"{save_fld}\{i}{ext}";
                        file.SaveAs(path);
                        i += 1;
                    }

                    //フォルダセキュリティ変更
                    UtilFuncs.ChangeDirectoryAccessControl(save_fld);
                }

                TempData["msg"] = $"ID={crr.ID}の修理完了しました。";

                //メール送信
                string mailto_path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/mail-to/repair_record.json");
                var mailto = JsonConvert.DeserializeObject<Dictionary<string, string>>(ReadText(mailto_path));
                string subject = $"修理完了(ID={crr.ID})";
                string mail_text = $"ID={crr.ID}の修理が完了しました。\n\n";
                mail_text += $"装置：{crr.MacName}({crr.Plantcd})\n【原因】\n{crr.Cause}\n【処置内容】\n{crr.Treatment}\n【備考】\n{crr.Remarks2}\n\n";
                mail_text += $@"https://koda.cej.citizen.co.jp/kodaweb/Form/CompleteRR/{crr.ID}";
                Mail.MailSend_from_Koda(mailto, subject, mail_text);

                return RedirectToAction("RRList");
            }
            return View(crr);
        }


    }
}