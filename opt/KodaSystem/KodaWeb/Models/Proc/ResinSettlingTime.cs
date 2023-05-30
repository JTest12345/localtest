using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using Newtonsoft.Json;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// 樹脂沈降時間管理クラス
    /// </summary>
    public class ResinSettlingTime : ResinSQL.ResinSettlingTimeData {

        [Display(Name = "社員コード")]
        public string EmployeeCode { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string ManuBase { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Place { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Shelf { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Area { get; set; }

        /// <summary>
        /// ボタン押した時の動作モード
        /// <para>0：表示</para>
        /// <para>1：ロット登録</para>
        /// <para>2：取り出し</para>
        /// <para>3：保管場所登録</para>
        /// <para>4：保管エリア変更</para>
        /// </summary>
        [JsonIgnore]
        public int Mode { get; set; }

        /// <summary>
        /// ツリーに表示する用
        /// </summary>
        [JsonIgnore]
        public string TreeViewJson { get; set; }

        /// <summary>
        /// 全保管情報
        /// </summary>
        [JsonIgnore]
        public List<ManubaseInfo> StorageInfo { get; set; }

        /// <summary>
        /// 製造拠点リスト(ドロップダウンリスト用)
        /// </summary>
        public static List<SelectListItem> manubase_items = new List<SelectListItem>() {
            new SelectListItem { Text = "CET", Value = "CET" ,Selected=true},
            new SelectListItem { Text = "CEJM", Value = "CEJM" },
        };

        [JsonIgnore]
        public SearchRst SearchRst { get; set; }

        /// <summary>
        /// ツリー情報を扱うクラス
        /// </summary>
        public class TreeJson {

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("nodes", NullValueHandling = NullValueHandling.Ignore)]
            public List<TreeJson> Nodes { get; set; }
        }

        /// <summary>
        /// ツリー表示用のJSON作成する
        /// </summary>
        public static string Get_TreeJson(List<ResinSettlingTime> rst_list) {

            var tj_list = new List<TreeJson>();

            //拠点の数だけループ
            foreach (var _base in rst_list.GroupBy(x => x.ManuBase)) {

                var tj_base = new TreeJson() { Text = _base.Key, Nodes = new List<TreeJson>() };

                //場所の数だけループ
                foreach (var _place in _base.GroupBy(x => x.Place)) {

                    var tj_place = new TreeJson() { Text = _place.Key, Nodes = new List<TreeJson>() };

                    //棚の数だけループ
                    foreach (var _shelf in _place.GroupBy(x => x.Shelf)) {

                        var tj_shelf = new TreeJson() { Text = _shelf.Key, Nodes = new List<TreeJson>() };

                        //エリアの数だけループ
                        foreach (var _area in _shelf.GroupBy(x => x.Area)) {

                            var tj_area = new TreeJson() { Text = _area.Key };
                            tj_shelf.Nodes.Add(tj_area);
                        }

                        tj_place.Nodes.Add(tj_shelf);
                    }

                    tj_base.Nodes.Add(tj_place);
                }

                tj_list.Add(tj_base);
            }

            string json = JsonConvert.SerializeObject(tj_list, Formatting.Indented);

            return json;
        }


        /// <summary>
        /// 拠点情報リスト作成（オブジェクトツリー）
        /// </summary>
        public static List<ManubaseInfo> Get_ManuBaseInfoList(List<ResinSettlingTime> rst_list) {

            var mbi_list = new List<ManubaseInfo>();

            //拠点の数だけループ
            foreach (var _base in rst_list.GroupBy(x => x.ManuBase)) {

                var mbi = new ManubaseInfo() { ManubaseName = _base.Key, PlaceInfos = new List<PlaceInfo>() };

                //場所の数だけループ
                foreach (var _place in _base.GroupBy(x => x.Place)) {

                    var pi = new PlaceInfo() { PlaceName = _place.Key, ShelfInfos = new List<ShelfInfo>() };

                    //棚の数だけループ
                    foreach (var _shelf in _place.GroupBy(x => x.Shelf)) {

                        var si = new ShelfInfo() { ShelfName = _shelf.Key, AreaInfos = new List<AreaInfo>() };

                        //エリアの数だけループ
                        foreach (var _area in _shelf.GroupBy(x => x.Area)) {

                            var ai = new AreaInfo() { AreaName = _area.Key, RstList = new List<ResinSettlingTime>() };
                            si.AreaInfos.Add(ai);
                        }

                        pi.ShelfInfos.Add(si);
                    }

                    mbi.PlaceInfos.Add(pi);
                }

                mbi_list.Add(mbi);
            }

            return mbi_list;
        }
 
        /// <summary>
        /// 検索場所リスト作成(ドロップダウンリスト用)
        /// </summary>
        public static List<SelectListItem> Create_SearchPlaceItems(List<ResinSettlingTime> rst_list) {

            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "", Value = "" });

            //拠点の数だけループ
            foreach (var _base in rst_list.GroupBy(x => x.ManuBase)) {

                //場所の数だけループ
                foreach (var _place in _base.GroupBy(x => x.Place)) {
                    string text = $"{_base.Key} {_place.Key}";
                    string val = $"{_base.Key},{_place.Key}";
                    list.Add(new SelectListItem { Text = text, Value = val });
                }
            }

            return list;
        }

        /// <summary>
        /// 保管場所にロットを追加する
        /// </summary>
        public static List<ManubaseInfo> Update_StorageInfos(List<ManubaseInfo> mbi_list, List<ResinSettlingTime> lot_list) {

            //全保管場所でループ
            foreach (var mbi in mbi_list) {
                foreach (var pi in mbi.PlaceInfos) {
                    foreach (var si in pi.ShelfInfos) {
                        foreach (var ai in si.AreaInfos) {

                            //初期化して再度追加しなおす
                            ai.RstList = new List<ResinSettlingTime>();

                            //Lot毎でループ
                            foreach (var rst in lot_list) {

                                //エリア情報とロットの保管場所が一致したら
                                if (rst.ManuBase == mbi.ManubaseName && rst.Place == pi.PlaceName
                                    && rst.Shelf == si.ShelfName && rst.Area == ai.AreaName) {
                                    //エリアにロット追加
                                    ai.RstList.Add(rst);
                                }
                            }

                            //エリア内にLotがあるなら最短取り出し時間、キュア投入期限、初Lotの投入時間を設定
                            if (ai.RstList.Count != 0) {
                                ai.AreaMaxDateTime = ai.RstList.Min(x => x.MaxTime);
                                ai.AreaMinDateTime = ai.RstList.Max(x => x.MinTime);
                                ai.AreaFirstInTime = ai.RstList.Min(x => x.InTime);
                            }
                        }
                    }
                }
            }

            return mbi_list;
        }

    }


    public class ManubaseInfo {

        public string ManubaseName { get; set; }

        /// <summary>
        /// 拠点内の各場所情報
        /// </summary>
        public List<PlaceInfo> PlaceInfos { get; set; }
    }

    public class PlaceInfo {

        public string PlaceName { get; set; }

        /// <summary>
        /// 場所内の各棚情報
        /// </summary>
        public List<ShelfInfo> ShelfInfos { get; set; }
    }

    public class ShelfInfo {

        public string ShelfName { get; set; }

        /// <summary>
        /// 棚内の各エリア情報
        /// </summary>
        public List<AreaInfo> AreaInfos { get; set; }
    }

    /// <summary>
    /// エリア情報を扱うクラス
    /// </summary>
    public class AreaInfo {

        public string AreaName { get; set; }

        public List<ResinSettlingTime> RstList { get; set; }

        public DateTime? AreaMaxDateTime { get; set; }

        public DateTime? AreaMinDateTime { get; set; }

        public DateTime? AreaFirstInTime { get; set; }
    }

    public class SearchRst {

        /// <summary>
        /// 結果検索の開始日時
        /// </summary>
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "検索開始日時")]
        [JsonIgnore]
        public DateTime Start { get; set; }

        /// <summary>
        /// 結果検索の終了日時
        /// </summary>
        [CheckPeriodAttribute]//自作のバリデーションチェック
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "検索終了日時")]
        [JsonIgnore]
        public DateTime End { get; set; }

        /// <summary>
        /// 検索期間のバリデーションチェック
        /// </summary>
        [AttributeUsage(AttributeTargets.Property)]
        public class CheckPeriodAttribute : ValidationAttribute {

            protected override ValidationResult IsValid(object value, ValidationContext vc) {

                // 比較対象のプロパティの値
                object start_obj = vc.ObjectType.GetProperty("Start").GetValue(vc.ObjectInstance, null);
                object end_obj = vc.ObjectType.GetProperty("End").GetValue(vc.ObjectInstance, null);

                //nullの場合はsuccessを返す
                //Required属性でエラーにさせる
                if (start_obj == null | end_obj == null) {
                    return ValidationResult.Success;
                }

                DateTime start = (DateTime)start_obj;
                DateTime end = (DateTime)end_obj;

                TimeSpan delta = end - start;

                // 条件に応じて検証結果を返す
                if (delta.Days < 0) {
                    this.ErrorMessage = "開始日-終了日が逆です。";
                    return new ValidationResult(this.ErrorMessage);
                }

                if (delta.Days > 184) {
                    this.ErrorMessage = "期間は6か月以内にして下さい。";
                    return new ValidationResult(this.ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }
    }

}