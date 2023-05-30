using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Configuration;
using System.Data;

using Newtonsoft.Json;

using KodaWeb.Models;
using KodaClassLibrary;
using static KodaClassLibrary.UtilFuncs;
using ResinClassLibrary;
using CejDataAccessCom;
using KodaWeb.Models.WebApi;

namespace KodaWeb.Controllers {
    /// <summary>
    /// 工程コントローラー
    /// </summary>
    public class ProcController : Controller {

        /// <summary>
        /// データベース接続文字列取得
        /// <para>web.config内の文字列変えた時に自動で取得しなおされる</para>
        /// </summary>
        private static string proc_connect_string = WebConfigurationManager.ConnectionStrings["ProcDatabase"].ConnectionString;

        KodaClassLibrary.ResinSQL resin_sql = new KodaClassLibrary.ResinSQL(proc_connect_string);


        /// <summary>
        /// ユーザーレベル取得するためのデータベース接続文字列
        /// </summary>
        private static string koda_connect_string = WebConfigurationManager.ConnectionStrings["KodaDatabase"].ConnectionString;

        KodaSQL koda_sql = new KodaSQL(koda_connect_string);


        /// <summary>
        /// 樹脂沈降時間管理の保管場所記載ファイルパス
        /// </summary>
        private readonly string rst_areas_path = System.Web.HttpContext.Current.Server.MapPath("~/App_Data/rst-areas.json");


        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// マガジン情報取得ページ
        /// </summary>
        public ActionResult GetMagLot() {
            return View();
        }

        /// <summary>
        /// マガジン情報取得ページのPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetMagLot(MagInfo mi) {

            if (ModelState.IsValid) {

                try {

                    if (mi.MagNo != null) {
                        //マガジン番号からLotNo取得
                        mi.LotNo18 = UseArmsApi.Get_LotNo18(mi.MagNo);
                    }

                    //18桁LotNoから機種情報取得
                    var lot = UseFjhApi.Get_LotInfo_from_LotNo18(new List<string>() { mi.LotNo18 })[0];

                    if (lot.IsOK) {
                        mi.ProductName = lot.TypeCd;
                        mi.LotNo10 = lot.JitLotNo;
                        mi.VLot = lot.VLotNo;
                    }
                    else {
                        throw new Exception("対象マガジンのロット情報はデータベースにはありませんでした。");
                    }

                }
                catch (Exception ex) {
                    ViewBag.ErrorMsg = ex.Message;
                    mi = new MagInfo { MagNo = mi.MagNo };
                    return View(mi);
                }
            }

            return View(mi);
        }



        /// <summary>
        /// 樹脂沈降時間管理ページ
        /// </summary>
        public ActionResult Rst() {

            //全保管場所取得
            var json = ReadText(rst_areas_path);
            var storage_list = JsonConvert.DeserializeObject<List<ResinSettlingTime>>(json);

            //ツリー表示用のJSON取得
            var tree_json = ResinSettlingTime.Get_TreeJson(storage_list);

            var rst = new ResinSettlingTime {
                TreeViewJson = tree_json,
            };

            ViewBag.ManubaseList = ResinSettlingTime.manubase_items;
            return View(rst);
        }

        /// <summary>
        /// 樹脂沈降時間管理ページのPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Rst(ResinSettlingTime rst) {

            if (ModelState.IsValid) {

                if (rst.Mode != 0) {

                    try {
                        string user_id = GetHankaku(rst.EmployeeCode).Replace("01 ", "");

                        //レベルチェック
                        var level = koda_sql.Get_UserLevel<KodaSQL.TmLevelData>(user_id);
                        if (level.ResinLevel < 2) {
                            throw new Exception("ロット登録/取り出しできる権限がありません。");
                        }

                        //ロット登録処理
                        if (rst.Mode == 1) {
                            var lot = resin_sql.Get_Lot<ResinSettlingTime>(rst.Typecd, rst.LotNo10);
                            if (lot != null) {
                                throw new Exception("このLotは登録されています。");
                            }

                            //ここで18桁Lotの取得
                            //xxxxxxxx

                            //投入時間設定
                            rst.InTime = DateTime.Now;

                            //mintime maxtime設定（本当は機種ごとの気もするが今は固定）
                            rst.MinTime = rst.InTime.AddHours(12);//12時間後
                            rst.MaxTime = rst.InTime.AddHours(90);//90時間後

                            //ver9等キュア炉で沈降させるタイプのもの用（こうしないとすぐに取り出せない）
                            //後でいらなくなるので、今だけ一時的に処置
                            if (rst.Shelf == "キュア炉") {
                                rst.MinTime = rst.InTime;
                            }

                            //登録しようとしているエリア内のロット取得
                            var inlot_list = resin_sql.Get_LotList<ResinSettlingTime>(rst.ManuBase, rst.Place, rst.Shelf, rst.Area);

                            if (inlot_list.Count > 0) {
                                //このエリアのキュア投入期限
                                DateTime area_maxtime = inlot_list.Min(x => x.MaxTime);
                                //1時間の余裕を持たせる
                                area_maxtime = area_maxtime.AddHours(-1);

                                if (rst.MinTime > area_maxtime) {
                                    throw new Exception("このロットを一緒にするとキュア投入期限を超えてしまうロットが存在するため、このエリアには投入できません。");
                                }
                            }

                            //投入者設定
                            rst.InputBy = user_id;

                            //データベースへ登録
                            resin_sql.Insert_Lot(rst);

                            //初期化
                            ModelState.Remove("Typecd");
                            rst.Typecd = null;
                            ModelState.Remove("LotNo10");
                            rst.LotNo10 = null;
                            ModelState.Remove("LotNo18");
                            rst.LotNo18 = null;
                            ModelState.Remove("InputBy");
                            rst.InputBy = null;

                            ViewBag.Msg = "ロット登録しました。";
                        }
                        //取り出し処理
                        else if (rst.Mode == 2) {
                            //取り出しエリア内の取り出されていないロット取得
                            var output_list = resin_sql.Get_LotList<ResinSettlingTime>(rst.ManuBase, rst.Place, rst.Shelf, rst.Area);

                            if (output_list.Count == 0) {
                                throw new Exception("既に取り出されています。");
                            }

                            //取り出し時間
                            var now = DateTime.Now;

                            //エリア内の最短取り出し時間
                            DateTime min_time = output_list.Max(x => x.MinTime);

                            //普通は取り出しボタンが無いので取り出せないが、ブラウザリロードとかでおかしいことがあったのでここでも判断
                            if (now < min_time) {
                                throw new Exception("最短取り出し時間になっていないので取り出しは出来ません。");
                            }

                            //ロット毎にデータベースをUpdate
                            foreach (var outlot in output_list) {
                                outlot.OutTime = now;
                                outlot.OutputBy = user_id;
                                resin_sql.Update_Lot(outlot, 1);
                            }

                            //初期化
                            ModelState.Remove("OutputBy");
                            rst.OutputBy = null;

                            ViewBag.Msg = "取り出し処理しました。";
                        }
                        //保管場所登録処理
                        else if (rst.Mode == 3) {

                            //レベルチェック
                            if (level.ResinLevel < 3) {
                                throw new Exception("保管場所登録できる権限がありません。");
                            }

                            //全保管場所取得
                            var json = ReadText(rst_areas_path);
                            var list = JsonConvert.DeserializeObject<List<ResinSettlingTime>>(json);

                            //登録しようとしている場所と同じ場所が何個あるか数える
                            var cnt = list.Where(x => x.ManuBase == rst.ManuBase && x.Place == rst.Place && x.Shelf == rst.Shelf && x.Area == rst.Area).Count();

                            if (cnt > 0) {
                                throw new Exception("同じ場所が登録されています。");
                            }
                            else {
                                list.Add(rst);

                                //保存用JSON作成
                                var area_list = new List<Dictionary<string, string>>();
                                foreach (var a in list) {
                                    var dic = new Dictionary<string, string> {
                                    { "manubase",a.ManuBase },
                                     { "place",a.Place },
                                     { "shelf",a.Shelf },
                                     { "area",a.Area }
                                };
                                    area_list.Add(dic);
                                }
                                var areas_json = JsonConvert.SerializeObject(area_list, Formatting.Indented, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore });

                                //ファイルに保存
                                OverWriteText(rst_areas_path, areas_json);
                            }

                            ViewBag.Msg = "保管場所登録しました。";
                        }
                        else if (rst.Mode == 4) {
                            //レベルチェック
                            if (level.ResinLevel < 3) {
                                throw new Exception("保管エリア変更できる権限がありません。");
                            }

                            var move_lot = resin_sql.Get_MoveLot<ResinSettlingTime>(rst.Typecd, rst.LotNo10);

                            if (move_lot != null) {
                                throw new Exception("このロットは既に1回移動しているので移動出来ません。");
                            }

                            TempData["rst"] = rst;
                            return RedirectToAction("ChangeRstArea");
                        }
                    }
                    catch (Exception ex) {
                        ViewBag.ErrorMsg = ex.Message;
                        //これをしないと反映されない
                        ModelState.Clear();
                        var new_rst = new ResinSettlingTime() {
                            ManuBase = rst.ManuBase,
                            Place = rst.Place,
                        };
                        rst = new_rst;
                    }

                }

                //全保管場所取得
                var storage_json = ReadText(rst_areas_path);
                var storage_list = JsonConvert.DeserializeObject<List<ResinSettlingTime>>(storage_json);

                //ツリー表示用のJSON取得
                var tree_json = ResinSettlingTime.Get_TreeJson(storage_list);

                //全保管場所オブジェクトツリー取得
                var mbi_list = ResinSettlingTime.Get_ManuBaseInfoList(storage_list);

                //沈降中ロット取得
                var lot_list = resin_sql.Get_LotList<ResinSettlingTime>(rst.ManuBase, rst.Place);

                //全保管場所にLot情報を追加する
                mbi_list = ResinSettlingTime.Update_StorageInfos(mbi_list, lot_list);

                //再設定
                ModelState.Remove("TreeViewJson");
                rst.TreeViewJson = tree_json;
                ModelState.Remove("StorageInfo");
                rst.StorageInfo = mbi_list;
            }

            ViewBag.ManubaseList = ResinSettlingTime.manubase_items;
            return View(rst);
        }

        /// <summary>
        /// 樹脂沈降保管エリア変更ページ
        /// </summary>
        public ActionResult ChangeRstArea() {

            var rst = (ResinSettlingTime)TempData["rst"];

            var lot = resin_sql.Get_Lot<ResinSettlingTime>(rst.Typecd, rst.LotNo10);
            lot.EmployeeCode = rst.EmployeeCode;

            ////全保管場所取得
            var json = ReadText(rst_areas_path);
            var storage_list = JsonConvert.DeserializeObject<List<ResinSettlingTime>>(json);

            List<string> change_area = storage_list.Where(x => x.ManuBase == lot.ManuBase && x.Place == lot.Place && x.Shelf == lot.Shelf).Select(x => x.Area).ToList();

            Dictionary<string, bool> select_area_dic = new Dictionary<string, bool>();
            foreach (string area in change_area) {
                select_area_dic.Add(area, false);
                if (area == lot.Area) {
                    select_area_dic[area] = true;
                }
            }

            ViewBag.SelectAreaDic = select_area_dic;
            return View(lot);
        }

        /// <summary>
        /// 樹脂沈降保管エリア変更ページのPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ChangeRstArea(ResinSettlingTime rst) {

            string user_id = GetHankaku(rst.EmployeeCode).Replace("01 ", "");

            //変更する前に情報取得しておく
            var lot = resin_sql.Get_Lot<ResinSettlingTime>(rst.Typecd, rst.LotNo10);

            //移動前のレコードを更新
            rst.OutTime = DateTime.Now;
            rst.OutputBy = user_id;
            resin_sql.Update_Lot(rst, 2);

            lot.Area = rst.Area;
            //再度データベースへ登録
            resin_sql.Insert_Lot(lot);

            return RedirectToAction("Rst");
        }

        /// <summary>
        /// 樹脂沈降時間結果取得ページ
        /// </summary>
        public ActionResult GetRst() {

            //全保管場所取得
            var storage_json = ReadText(rst_areas_path);
            var storage_list = JsonConvert.DeserializeObject<List<ResinSettlingTime>>(storage_json);

            //検索場所用ドロップダウンリスト作成
            ViewBag.DropDownList = ResinSettlingTime.Create_SearchPlaceItems(storage_list);

            return View();
        }

        /// <summary>
        /// 樹脂沈降時間結果取得ページのPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult GetRst(ResinSettlingTime rst) {

            if (ModelState.IsValid) {

                var rst_list = resin_sql.Get_Lot<ResinSettlingTime>(rst.SearchRst.Start, rst.SearchRst.End);

                //テーブル作成＋列追加(基本データ)
                DataTable table = new DataTable("RstData");

                List<string> cols = new List<string> { "機種", "10桁LotNo", "18桁LotNo",
                    "製造拠点", "場所", "棚", "エリア", "投入時間", "投入作業者",
                    "最短取り出し時間", "キュア投入期限", "取り出し時間", "取り出し作業者"  };

                foreach (string c in cols) {
                    table.Columns.Add(c);
                }

                string csv_text = string.Join(",", cols) + "\n";

                //DataTableに1行追加を繰り返す
                foreach (var data in rst_list.OrderBy(x => x.OutTime)) {

                    //新しく1行データ作成
                    DataRow dr = table.NewRow();

                    dr["機種"] = data.Typecd;
                    dr["10桁LotNo"] = data.LotNo10;
                    dr["18桁LotNo"] = data.LotNo18;

                    dr["製造拠点"] = data.ManuBase;
                    dr["場所"] = data.Place;
                    dr["棚"] = data.Shelf;
                    dr["エリア"] = data.Area;

                    dr["投入時間"] = data.InTime.ToString("yyyy/MM/dd HH:mm:ss");
                    dr["投入作業者"] = data.InputBy;
                    dr["最短取り出し時間"] = data.MinTime.ToString("yyyy/MM/dd HH:mm:ss");
                    dr["キュア投入期限"] = data.MaxTime.ToString("yyyy/MM/dd HH:mm:ss");
                    dr["取り出し時間"] = ((DateTime)data.OutTime).ToString("yyyy/MM/dd HH:mm:ss");
                    dr["取り出し作業者"] = data.OutputBy;

                    //テーブルに1行分データ追加
                    table.Rows.Add(dr);

                    csv_text += string.Join(",", dr.ItemArray) + "\n";
                }

                ViewBag.DataTable = table;

                ViewBag.CsvFileName = $"樹脂沈降時間データ_{rst.SearchRst.Start.ToString("yyyy/MM/dd")}～{rst.SearchRst.End.ToString("yyyy/MM/dd")}";
                ViewBag.CsvText = csv_text;
            }

            //全保管場所取得
            var storage_json = ReadText(rst_areas_path);
            var storage_list = JsonConvert.DeserializeObject<List<ResinSettlingTime>>(storage_json);

            //検索場所用ドロップダウンリスト作成
            ViewBag.DropDownList = ResinSettlingTime.Create_SearchPlaceItems(storage_list);

            return View(rst);
        }


        /// <summary>
        /// 樹脂レシピ取得ページ
        /// </summary>
        public ActionResult GetResinCupRecipeList(string search_date) {

            List<ResinClassLibrary.ResinSQL.ResinCupRecipe_Data> list;

            if (search_date != null) {
                var local_sql = new ResinClassLibrary.ResinSQL(proc_connect_string);

                var sp = search_date.Split('-');
                list = local_sql.Get_ResinCupList<ResinClassLibrary.ResinSQL.ResinCupRecipe_Data>(sp[0].Substring(2, 2) + sp[1] + sp[2]);
            }
            else {
                list = null;
            }

            return View(list);
        }


        /// <summary>
        /// 樹脂先行評価ログ取得ページ
        /// </summary>
        public ActionResult GetResinSenkoLog(string productName) {

            if (string.IsNullOrEmpty(productName) == false) {

                List<ResinClassLibrary.ResinSQL.ResinSenkoLog_Data> list;

                var local_sql = new ResinClassLibrary.ResinSQL(proc_connect_string);

                list = local_sql.Get_SenkoLogDataList<ResinClassLibrary.ResinSQL.ResinSenkoLog_Data>(productName);


                var table = new DataTable("senkolog");
                foreach (var s in new List<string> { "日時", "機種名", "波長ランク", "樹脂種類", "特殊指定", "条件", "指示/結果", "基準配合量" }) {
                    table.Columns.Add(s);
                }

                foreach (var data in list) {

                    //追加する行作成
                    DataRow dr = table.NewRow();

                    //基本情報
                    dr["日時"] = data.InsertAt.ToString("yyyy/MM/dd HH:mm:ss");
                    dr["機種名"] = data.ProductName;
                    dr["波長ランク"] = data.WavelengthRank;
                    dr["樹脂種類"] = data.MoldType;
                    dr["特殊指定"] = data.SpecialDesignation;
                    dr["条件"] = data.Conditions;
                    dr["指示/結果"] = data.ResultOrInput;
                    dr["基準配合量"] = data.BaseAmount.ToString();

                    //部材情報
                    var buzai_list = JsonConvert.DeserializeObject<List<MixBuzai>>(data.Buzai);
                    foreach (var b in buzai_list) {
                        var col_name = $"{b.Fcode}<br/>{b.Name}";

                        if (table.Columns.Contains(col_name) == false) {
                            table.Columns.Add(col_name);
                        }

                        dr[col_name] = $"{b.LotNo}<br/>{b.BaseAmount}g";
                    }

                    //ろ過後部材情報
                    var afterfilt_buzai_list = JsonConvert.DeserializeObject<List<MixBuzai>>(data.AfterFiltBuzai);
                    if (afterfilt_buzai_list != null) {
                        foreach (var ab in afterfilt_buzai_list) {
                            var col_name = $"{ab.Fcode}<br/>{ab.Name}";

                            if (table.Columns.Contains(col_name) == false) {
                                table.Columns.Add(col_name);
                            }

                            dr[col_name] = $"{ab.LotNo}<br/>{ab.PercentOfTotalWeight}%";
                        }
                    }

                    //テーブルに1行分データ追加
                    table.Rows.Add(dr);
                }

                ViewBag.DataTable = table;
            }

            return View();
        }




    }
}