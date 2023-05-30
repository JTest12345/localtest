using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data;

using KodaWeb.Models;

using KodaWeb;
using KodaClassLibrary;
using static KodaClassLibrary.UtilFuncs;

namespace KodaWeb.Controllers {
    public class HomeController : Controller {

        /// <summary>
        /// データベース接続文字列取得
        /// <para>web.config内の文字列変えた時に自動で取得しなおされる</para>
        /// </summary>
        public static string connect_string = WebConfigurationManager.ConnectionStrings["KodaDatabase"].ConnectionString;

        KodaSQL koda_sql = new KodaSQL(connect_string);

        /// <summary>
        /// ホームページ
        /// </summary>
        public ActionResult Index() {
            return View();
        }

        /// <summary>
        /// ユーザーレベル登録/変更画面
        /// </summary>
        public ActionResult UpdateUserLevel() {
            return View();
        }

        /// <summary>
        /// ユーザーレベル登録/変更画面のPOST処理
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult UpdateUserLevel(UserLevel ul) {

            //状態クリア
            ModelState.Clear();

            if (ul.Mode == 1) {
                try {
                    string userid = GetHankaku(ul.NewRegisteredPerson.UserID).Replace("01 ", "");
                    koda_sql.Insert_UserLevel(userid);
                    ViewBag.Msg = "新規登録しました。";
                }
                catch (Exception ex) {
                    ViewBag.ErrMsg = $"新規登録失敗しました。既に登録してある人かもしれません。{ex.Message}";
                }
                ul.UpdatedPerson = null;
                ul.NewRegisteredPerson = null;
            }
            else if (ul.Mode == 2) {
                //対象者のレベル取得
                ul.UpdatedPerson = koda_sql.Get_UserLevel<KodaSQL.TmLevelData>(ul.UpdatedPerson.UserID);
                //ドロップダウンリスト作成
                if (ul.UpdatedPerson.FormsLevel <= ul.RegisterPerson.FormsLevel) {
                    ViewBag.FormLevelList = ul.Create_UserLeverList(ul.UpdatedPerson.FormsLevel, ul.RegisterPerson.FormsLevel);
                }
                if (ul.UpdatedPerson.ResinLevel <= ul.RegisterPerson.ResinLevel) {
                    ViewBag.ResinLevelList = ul.Create_UserLeverList(ul.UpdatedPerson.ResinLevel, ul.RegisterPerson.ResinLevel);
                }
                ul.NewRegisteredPerson = null;
            }
            else if (ul.Mode == 3) {
                try {
                    //対象者のレベル変更
                    koda_sql.Update_UserLevel(ul.UpdatedPerson);
                    ViewBag.Msg = "レベル変更しました。";
                }
                catch (Exception ex) {
                    ViewBag.ErrMsg = $"レベル変更失敗しました。{ex.Message}";
                }
                ul.UpdatedPerson = null;
                ul.NewRegisteredPerson = null;
            }

            try {
                ul.RegisterPerson = koda_sql.Get_UserLevel<KodaSQL.TmLevelData>(ul.RegisterPerson.UserID);

                var user_list = koda_sql.Get_Users(ul.RegisterPerson);
                ViewBag.UserList = UserLevel.Create_UserList(user_list);
            }
            catch (Exception ex) {
                ViewBag.ErrMsg = $"{ex.Message}";
                ul = null;
            }
            
            return View(ul);
        }
    }
}