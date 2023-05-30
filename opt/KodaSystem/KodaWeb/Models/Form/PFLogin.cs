using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// Product Form Loginクラス
    /// <para>製品帳票入力ログインクラス</para>
    /// </summary>
    public class PFLogin : FormsSQL.TnFormTranData {

        [Required(ErrorMessage = "{0}は必須です。")]
        [Display(Name = "社員コード")]
        public string EmployeeCode { get; set; }

        [CheckInput]//自作のバリデーションチェック
        [Display(Name = "マガジン番号")]
        public string MagNo { get; set; }


        [AttributeUsage(AttributeTargets.Property)]
        public class CheckInputAttribute : ValidationAttribute {

            protected override ValidationResult IsValid(object value, ValidationContext vc) {

                // 比較対象のプロパティの値
                //object lotno10 = vc.ObjectType.GetProperty("LotNo10").GetValue(vc.ObjectInstance, null);
                object lotno18 = vc.ObjectType.GetProperty("LotNo18").GetValue(vc.ObjectInstance, null);
                object magno = vc.ObjectType.GetProperty("MagNo").GetValue(vc.ObjectInstance, null);
                object plantcd = vc.ObjectType.GetProperty("Plantcd").GetValue(vc.ObjectInstance, null);
                object workunit_id = vc.ObjectType.GetProperty("WorkUnitID").GetValue(vc.ObjectInstance, null);

                this.ErrorMessage = "QRコードはどれか1つのみ入力して下さい。";
                //どれか1つだけ入力されている場合のみsuccessを返す
                int cnt = 0;
                var obj_list = new List<object>() { lotno18, magno, plantcd, workunit_id };
                foreach (var obj in obj_list) {
                    if (obj != null) {
                        cnt += 1;
                    }
                }

                if (cnt == 1) {
                    return ValidationResult.Success;
                }
                else {
                    return new ValidationResult(this.ErrorMessage);
                }
            }
        }
    }
}