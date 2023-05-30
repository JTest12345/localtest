using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using Newtonsoft.Json;

using KodaClassLibrary;

namespace KodaWeb.Models {

    public class ParticleGraph : ParticleSQL.ParticleData {

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
        /// ツリー表示用のJSON作成する
        /// </summary>
        public static string Get_TreeJson(List<ParticleGraph> pg_list) {

            var tj_list = new List<TreeJson>();

            //拠点の数だけループ
            foreach (var _base in pg_list.GroupBy(x => x.ManuBase)) {

                var tj_base = new TreeJson() { Text = _base.Key, Nodes = new List<TreeJson>() };

                //場所の数だけループ
                foreach (var _place in _base.GroupBy(x => x.Place)) {

                    var tj_place = new TreeJson() { Text = _place.Key, Nodes = new List<TreeJson>() };

                    //センサー名の数だけループ
                    foreach (var _sensor in _place.GroupBy(x => x.SensorName)) {

                        var tj_sensor = new TreeJson() { Text = _sensor.Key };

                        tj_place.Nodes.Add(tj_sensor);
                    }

                    tj_base.Nodes.Add(tj_place);
                }

                tj_list.Add(tj_base);
            }

            string json = JsonConvert.SerializeObject(tj_list, Formatting.Indented);

            return json;
        }


    }

}