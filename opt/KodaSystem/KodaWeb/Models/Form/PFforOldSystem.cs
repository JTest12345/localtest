using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.ComponentModel.DataAnnotations;

using KodaClassLibrary;

namespace KodaWeb.Models {

    /// <summary>
    /// 旧システム使用機種で帳票システムを使いたいために作成したクラス
    /// </summary>
    public class PFforOldSystem : FormsSQL.TnFormTranData {

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Typecd { get; set; }

        [Required(ErrorMessage = "{0}は必須です。")]
        public override string Plantcd { get; set; }

        /// <summary>
        /// 製品/工程帳票へのリンクURL
        /// </summary>
        public string LinkUrl { get; set; }

        /// <summary>
        /// 帳票名
        /// </summary>
        [Display(Name = "帳票名")]
        public string FormName { get; set; }

        /// <summary>
        /// 製造拠点リスト(ドロップダウンリスト用)
        /// </summary>

        public List<SelectListItem> create_manubase_items() {

            var list = new List<SelectListItem>();

            foreach (var mb in new List<string> { "CET", "CEJM" }) {

                SelectListItem item;
                if (mb == this.ManuBase) {
                    item = new SelectListItem { Text = mb, Value = mb, Selected = true };
                }
                else {
                    item = new SelectListItem { Text = mb, Value = mb };
                }

                list.Add(item);
            }

            return list;
        }

        /// <summary>
        /// 帳票リスト(ドロップダウンリスト用)
        /// </summary>
        /// <param name="form_master_list">データベースから取得した帳票一覧</param>
        public List<SelectListItem> create_form_items(List<FormInfo> form_master_list) {

            var list = new List<SelectListItem>();

            list.Add(new SelectListItem { Text = "選択して下さい", Value = "" });

            foreach (var fm in form_master_list) {

                string text = $"{fm.FormName}：{fm.FormNo}";
                string val = $"{fm.FormName},{fm.FormNo},{fm.FormRev.ToString()}";

                SelectListItem item;
                if (fm.FormNo == this.FormNo && fm.FormRev == this.FormRev) {
                    item = new SelectListItem { Text = text, Value = val, Selected = true };
                }
                else {
                    item = new SelectListItem { Text = text, Value = val };
                }

                list.Add(item);
            }

            return list;
        }


    }

}