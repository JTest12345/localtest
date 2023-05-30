using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Globalization;
using System.Web.Mvc;
using System.Data;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// Product Form Data Search Conditionクラス
    /// <para>製品帳票データ検索条件クラス</para>
    /// </summary>
    public class PFDSearch {

        [Display(Name = "帳票選択")]
        public FormInfo FormInfo { get; set; }

        /// <summary>
        /// データ期間（始め）
        /// </summary>
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "開始日")]
        public string Start { get; set; }

        /// <summary>
        /// データ期間（終わり）
        /// </summary>
        [CheckPeriodAttribute]//自作のバリデーションチェック
        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "終了日")]
        public string End { get; set; }

        /// <summary>
        /// データベースから取得した結果
        /// </summary>
        public DataTable Table { get; set; }

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

                if (delta.Days > 184) {
                    this.ErrorMessage = "期間は6か月以内にして下さい。";
                    return new ValidationResult(this.ErrorMessage);
                }

                return ValidationResult.Success;
            }
        }

        /// <summary>
        /// 帳票リスト(ドロップダウンリスト用)
        /// </summary>
        /// <param name="form_master_list">データベースから取得した帳票一覧</param>
        public List<SelectListItem> create_form_items(List<FormInfo> form_master_list) {

            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "選択して下さい", Value = "" });

            foreach (var fm in form_master_list) {

                string text = $"{fm.FormName}:{fm.FormNo}:rev{fm.FormRev.ToString()}";
                string val = $"{fm.FormName},{fm.FormNo},{fm.FormRev.ToString()}";
                var item = new SelectListItem { Text = text, Value = val };

                if (this.FormInfo != null) {
                    if (fm.FormNo == this.FormInfo.FormNo && fm.FormRev == this.FormInfo.FormRev) {
                        item.Selected = true;
                    }
                }

                list.Add(item);
            }

            return list;
        }

    }
}