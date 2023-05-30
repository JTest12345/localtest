using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KodaClassLibrary {

    /// <summary>
    /// KODAデータベースに接続してKODAシステム全体に関わるデータを扱うクラス
    /// </summary>
    public class KodaSQL : SQL {

        public class TmLevelData {

            /// <summary>
            /// ユーザーID
            /// </summary>
            public virtual string UserID { get; set; }

            /// <summary>
            /// 帳票システムのレベル
            /// </summary>
            public virtual int FormsLevel { get; set; }


            /// <summary>
            /// 樹脂工程管理のレベル
            /// </summary>
            public virtual int ResinLevel { get; set; }
        }

        const string TmLevel_TABLE = "[dbo].[TmLevel]";

        const string userid_COL = "userid";
        const string forms_COL = "forms";
        const string resin_COL = "resin";

        /// <summary>
        /// コンストラクタ(使用しない)
        /// </summary>
        public KodaSQL() {
            builder.DataSource = @"vautom3\SQLEXpress";     // 接続先のSQL Server
            builder.UserID = "inline";                      // 接続ユーザー名
            builder.Password = "R28uHta";                   // 接続パスワード
            builder.InitialCatalog = "KODA";                // データベースの名前
            builder.ConnectTimeout = 5;                     // 接続タイムアウトの秒数(s) デフォルトは 15 秒
        }

        /// <summary>
        /// コンストラクタ(データベースへの接続情報を渡す)
        /// </summary>
        public KodaSQL(string data_source, string db_name, string user_id, string pass, int timeout)
            : base(data_source, db_name, user_id, pass, timeout) {

        }

        /// <summary>
        /// コンストラクタ(データベースへの接続文字列を渡す）
        /// </summary>
        public KodaSQL(string con_str)
            : base(con_str) {

        }

        /// <summary>
        /// システムレベルマスタテーブルからデータを読み取る共通メソッド
        /// </summary>
        public List<T> Get_TmLevelData<T>(string add_where, string new_query = null)
            where T : TmLevelData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {userid_COL},{forms_COL},{resin_COL}
                              FROM {TmLevel_TABLE}
                              WHERE 1=1 ";
            }
            else {
                query = new_query;
            }

            //条件があれば付け加える
            if (add_where != null) {
                query += add_where;
            }

            //データベースからデータ取得
            var table = read_data(query);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    UserID = col.Contains(userid_COL) ? row[userid_COL].ToString() : "",
                    FormsLevel = col.Contains(forms_COL) ? (int)row[forms_COL] : 1,
                    ResinLevel = col.Contains(resin_COL) ? (int)row[resin_COL] : 1,

                };

                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// 指定したシステムレベル以下のユーザーを取得する
        /// </summary>
        /// <returns></returns>
        public List<T> Get_Users<T>(T level)
            where T : TmLevelData, new() {

            string add_where = $@"AND (
                                        {forms_COL}<={level.FormsLevel}
                                     OR {resin_COL}<={level.ResinLevel}
                                      )";

            //データベースからデータ取得
            var list = Get_TmLevelData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 社員コードからユーザーレベルを取得する
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public T Get_UserLevel<T>(string user)
            where T : TmLevelData, new() {

            string add_where = $@"AND {userid_COL}='{user}'";

            //データベースからデータ取得
            var list = Get_TmLevelData<T>(add_where);

            //データが1個ではない時はおかしい
            if (list.Count == 0) {
                throw new Exception("この社員コードの登録はありませんでした。");
            }
            else if (list.Count > 1) {
                throw new Exception("この社員コードの登録が2個以上ありました。\nシステム管理者に連絡して下さい。");
            }

            //1個しかないはずなのでindex=0で処理する
            return list[0];
        }

        /// <summary>
        /// システムのユーザーレベルを変更する
        /// </summary>
        /// <param name="user"></param>
        /// <param name="forms_level"></param>
        public void Update_UserLevel(TmLevelData tld) {

            string query = $@"UPDATE {TmLevel_TABLE}
                              SET {forms_COL}={tld.FormsLevel},
                                  {resin_COL}={tld.ResinLevel}
                              WHERE {userid_COL}='{tld.UserID}'";

            execute(query);
        }

        /// <summary>
        ///ユーザーを新規登録する(levelは全部1)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="forms_level"></param>
        public void Insert_UserLevel(string userid) {

            string query = $@"INSERT INTO {TmLevel_TABLE} ({userid_COL},{forms_COL},{resin_COL})
                              VALUES('{userid}',1,1)";

            execute(query);
        }
    }

}
