using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using KodaClassLibrary;

namespace KodaWeb.Models {

    public class UserLevel {

        /// <summary>
        /// 登録する人
        /// </summary>
        public KodaSQL.TmLevelData RegisterPerson { get; set; }

        /// <summary>
        /// 新規登録される人
        /// </summary>
        public KodaSQL.TmLevelData NewRegisteredPerson { get; set; }

        /// <summary>
        /// 更新される人
        /// </summary>
        public KodaSQL.TmLevelData UpdatedPerson { get; set; }

        /// <summary>
        /// Post時の処理モード
        /// <para>管理者の社員コード入力完了：0</para>
        /// <para>新規登録：1</para>
        /// <para>対象者のレベル表示：2</para>
        /// <para>対象者のレベル更新：3</para>
        /// </summary>
        public int Mode { get; set; }

        /// <summary>
        /// レベル選択ドロップダウンリスト作成する
        /// </summary>
        public List<SelectListItem> Create_UserLeverList(int nowlevel, int maxlevel) {

            var level_list = new List<SelectListItem>();

            for (int i = 1; i <= maxlevel; i++) {
                if (i == nowlevel) {
                    level_list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString(), Selected = true });

                }
                else {
                    level_list.Add(new SelectListItem { Text = i.ToString(), Value = i.ToString() });
                }
            }

            return level_list;
        }
        public static List<SelectListItem> Create_UserList(List<KodaSQL.TmLevelData> data_list) {

            var level_list = new List<SelectListItem>();

            for (int i = 0; i < data_list.Count; i++) {
                if (i == 0) {
                    level_list.Add(new SelectListItem { Text = data_list[i].UserID, Value = data_list[i].UserID, Selected = true });
                }
                else {
                    level_list.Add(new SelectListItem { Text = data_list[i].UserID, Value = data_list[i].UserID });
                }
            }

            return level_list;
        }

    }
}