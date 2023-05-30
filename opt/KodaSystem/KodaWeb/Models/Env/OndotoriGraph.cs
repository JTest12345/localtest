using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using KodaClassLibrary;

namespace KodaWeb.Models {
    public class OndotoriGraph : OndotoriSQL.OndotoriData {

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "開始日")]
        public DateTime Start { get; set; }

        [CheckPeriodAttribute]//自作のバリデーションチェック
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "終了日")]
        public DateTime End { get; set; }

        [Required(ErrorMessage = "子機選択は必須です。")]
        public override string RemoteName { get; set; }

        /// <summary>
        /// グラフにセットする用
        /// </summary>
        public string GraphDataJson { get; set; }

        /// <summary>
        /// ツリーに表示する用
        /// </summary>
        public string TreeViewJson { get; set; }


        public class TreeJson {

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("nodes", NullValueHandling = NullValueHandling.Ignore)]
            public List<TreeJson> Nodes { get; set; }
        }

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


                DateTime start = DateTime.Parse(start_obj.ToString());
                DateTime end = DateTime.Parse(end_obj.ToString());

                TimeSpan delta = end - start;

                // 条件に応じて検証結果を返す
                if (delta.Days < 0) {
                    this.ErrorMessage = "開始日-終了日が逆です。";
                    return new ValidationResult(this.ErrorMessage);
                }

                if (delta.Days > 30) {
                    this.ErrorMessage = "期間は1か月以内にして下さい。";
                    return new ValidationResult(this.ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }

        /// <summary>
        /// ツリー表示用のJSON作成する
        /// </summary>
        public static string Get_TreeJson(List<OndotoriGraph> og_list) {

            var tj_list = new List<TreeJson>();

            //親機の数だけループ
            foreach (var _base in og_list.GroupBy(x => x.BaseName)) {

                var tj_base = new TreeJson() { Text = _base.Key, Nodes = new List<TreeJson>() };

                //グループの数だけループ
                foreach (var _group in _base.GroupBy(x => x.GroupName)) {

                    var tj_group = new TreeJson() { Text = _group.Key, Nodes = new List<TreeJson>() };

                    //子機の数だけループ
                    foreach (var _remote in _group.GroupBy(x => x.RemoteName)) {

                        var tj_remote = new TreeJson() { Text = _remote.Key };

                        tj_group.Nodes.Add(tj_remote);
                    }

                    tj_base.Nodes.Add(tj_group);
                }

                tj_list.Add(tj_base);
            }

            string json = JsonConvert.SerializeObject(tj_list, Formatting.Indented);

            return json;
        }


        public List<Color> Colors = new List<Color>() {
            Color.Red,Color.Green,Color.Blue,Color.DeepPink,Color.Orange,
            Color.DodgerBlue,Color.DarkViolet,Color.SaddleBrown,Color.YellowGreen,Color.Crimson
        };


    }
}