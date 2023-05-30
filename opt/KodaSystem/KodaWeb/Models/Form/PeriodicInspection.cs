using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// 定期点検クラス
    /// </summary>
    public class PeriodicInspection : FormsSQL.TnPIFormData {

        #region プロパティ

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string MacName { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Plantcd { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string SerialNo { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Place { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string FormNo { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        [CheckNextAttribute]//自作のバリデーションチェック
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public override DateTime Next { get; set; }

        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}")]
        public override DateTime Limit { get; set; }

        [Display(Name = "社員コード")]
        public string EmployeeCode { get; set; }

        #endregion


        [AttributeUsage(AttributeTargets.Property)]
        public class CheckNextAttribute : ValidationAttribute {

            protected override ValidationResult IsValid(object value, ValidationContext vc) {

                // 比較対象のプロパティの値
                object period_obj = vc.ObjectType.GetProperty("Period").GetValue(vc.ObjectInstance, null);
                object next_obj = vc.ObjectType.GetProperty("Next").GetValue(vc.ObjectInstance, null);

                //ここではSuccessを返す
                //Required属性でnullチェックする
                if (next_obj == null) {
                    return ValidationResult.Success;
                }

                string period = (string)period_obj;
                DateTime next = (DateTime)next_obj;
                //var list = new List<string>() { "M1", "M3", "M6", "Y1" };

                //if (list.Contains(period)) {
                //    if (next.Day != 1) {
                //        this.ErrorMessage = "点検周期が1か月以上の場合は日付は1日を選択して下さい。";
                //        return new ValidationResult(this.ErrorMessage);
                //    }
                //}

                if (period == "D7") {
                    if (next.DayOfWeek != DayOfWeek.Monday) {
                        this.ErrorMessage = "点検周期が1週間の場合は月曜日を選択して下さい。";
                        return new ValidationResult(this.ErrorMessage);
                    }
                }

                return ValidationResult.Success;
            }
        }



        /// <summary>
        /// 登録日/点検開始日/実施期限の設定
        /// </summary>
        public bool Set_DateTime() {

            InsertAt = DateTime.Now;

            DateTime next;
            if (Period == "D1") {
                //点検周期：1日
                next = (DateTime)Next;
                Next = new DateTime(next.Year, next.Month, next.Day, 7, 0, 0);
                //期限は当日まで
                Limit = new DateTime(next.Year, next.Month, next.Day, 23, 59, 59);
            }
            else if (Period == "D7") {
                //点検周期：1週間

                next = (DateTime)Next;
                Next = new DateTime(next.Year, next.Month, next.Day, 7, 0, 0);
                //期限は当日まで
                Limit = new DateTime(next.Year, next.Month, next.Day, 23, 59, 59);
            }
            else if (Period == "M1") {
                //点検周期：1か月
                next = (DateTime)Next;
                Next = new DateTime(next.Year, next.Month, next.Day, 7, 0, 0);
                //期限は1週間以内
                var end = ((DateTime)Next).AddDays(7);
                Limit = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else if (Period == "M3") {
                //点検周期：3か月
                next = (DateTime)Next;
                Next = new DateTime(next.Year, next.Month, next.Day, 7, 0, 0);
                //期限は1週間以内
                var end = ((DateTime)Next).AddDays(7);
                Limit = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else if (Period == "M6") {
                //点検周期：6か月
                next = (DateTime)Next;
                Next = new DateTime(next.Year, next.Month, next.Day, 7, 0, 0);
                //期限は1週間以内
                var end = ((DateTime)Next).AddDays(7);
                Limit = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else if (Period == "Y1") {
                //点検周期：1年
                next = (DateTime)Next;
                Next = new DateTime(next.Year, next.Month, next.Day, 7, 0, 0);
                //期限は2週間以内
                var end = ((DateTime)Next).AddDays(14);
                Limit = new DateTime(end.Year, end.Month, end.Day, 23, 59, 59);
            }
            else {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 点検周期リスト(ドロップダウンリスト用)
        /// </summary>
        public static List<SelectListItem> period_items = new List<SelectListItem>() {
            new SelectListItem { Text = "1日", Value = "D1" },
            new SelectListItem { Text = "1週間", Value = "D7" },
            new SelectListItem { Text = "1か月", Value = "M1",Selected=true},
            new SelectListItem { Text = "3か月", Value = "M3" },
            new SelectListItem { Text = "6か月", Value = "M6" },
            new SelectListItem { Text = "1年", Value = "Y1" }
        };

        /// <summary>
        /// 帳票リスト(ドロップダウンリスト用)
        /// </summary>
        public static List<SelectListItem> create_form_items(List<FormInfo> form_master_list) {

            var list = new List<SelectListItem>();
            list.Add(new SelectListItem { Text = "", Value = "" });

            foreach (var fm in form_master_list) {
                string text = $"{fm.FormName}:{fm.FormNo}";
                string val = fm.FormNo;
                list.Add(new SelectListItem { Text = text, Value = val });
            }

            return list;
        }

        /// <summary>
        /// 製造拠点リスト(ドロップダウンリスト用)
        /// </summary>

        public static List<SelectListItem> manubase_items = new List<SelectListItem>() {
            new SelectListItem { Text = "CET", Value = "CET" ,Selected=true},
            new SelectListItem { Text = "CEJM", Value = "CEJM" },
        };

    }
}