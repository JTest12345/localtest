using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;    //データサーバへアクセスするのに使う

namespace KodaClassLibrary {

    /// <summary>
    /// データベースへ接続してSQLでデータを扱う基本クラス
    /// </summary>
    public class SQL {

        /// <summary>
        /// 接続文字列を分かりやすく作るために使用
        /// </summary>
        protected SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();

        /// <summary>
        /// データベースへの接続オブジェクト
        /// </summary>
        private SqlConnection conn = null;

        /// <summary>
        /// データベースへの接続文字列
        /// </summary>
        private string connect_string = "";

        public string ConnectString { get { return connect_string; } }


        /// <summary>
        /// コンストラクタ(使用しない)
        /// </summary>
        public SQL() {
        }

        /// <summary>
        /// コンストラクタ(データベースへの接続文字列を渡す）
        /// </summary>
        public SQL(string con_str) {
            connect_string = con_str;
        }

        /// <summary>
        /// コンストラクタ(データベースへの接続情報を渡す)
        /// </summary>
        public SQL(string data_source, string db_name, string user_id, string pass, int timeout) {
            builder.DataSource = data_source;     // 接続先のSQL Server
            builder.InitialCatalog = db_name;     // データベースの名前
            builder.UserID = user_id;             // 接続ユーザー名
            builder.Password = pass;              // 接続パスワード
            builder.ConnectTimeout = timeout;     // 接続タイムアウトの秒数(s)
        }

        /// <summary>
        /// データベースから読み取ったデータをデータテーブルで返す
        /// </summary>
        /// <param name="query">SQLクエリ文</param>
        /// <exception cref="Exception"></exception>
        protected DataTable read_data(string query) {

            DataTable table = new DataTable("Table");

            string connectionString;
            if (string.IsNullOrEmpty(connect_string)) {
                connectionString = builder.ConnectionString;
            }
            else {
                connectionString = connect_string;
            }

            //接続オブジェクト作成
            using (conn = new SqlConnection(connectionString)) {

                //送信コマンド作成
                SqlCommand command = new SqlCommand(query, conn);

                //接続実行
                conn.Open();

                // データベースにクエリを送信して取得したデータを
                // まとめてデータテーブルに入れる
                var adapter = new SqlDataAdapter(command);
                adapter.Fill(table);
            }

            return table;
        }

        /// <summary>
        /// データベースに対してクエリを実行する
        /// </summary>
        /// <param name="query">SQLクエリ文</param>
        /// <exception cref="Exception"></exception>
        protected void execute(string query) {

            string connectionString;
            if (string.IsNullOrEmpty(connect_string)) {
                connectionString = builder.ConnectionString;
            }
            else {
                connectionString = connect_string;
            }

            //接続オブジェクト作成
            using (conn = new SqlConnection(connectionString)) {

                //送信コマンド作成
                SqlCommand command = new SqlCommand(query, conn);

                //接続実行
                conn.Open();

                //SQL実行
                int n = command.ExecuteNonQuery();
            }
        }


        protected void execute(string query, List<SqlParameter> param_list) {

            string connectionString;
            if (string.IsNullOrEmpty(connect_string)) {
                connectionString = builder.ConnectionString;
            }
            else {
                connectionString = connect_string;
            }

            //接続オブジェクト作成
            using (conn = new SqlConnection(connectionString)) {

                //送信コマンド作成
                SqlCommand command = new SqlCommand(query, conn);

                foreach (var p in param_list) {
                    command.Parameters.Add(p);
                }

                //接続実行
                conn.Open();

                //SQL実行
                int n = command.ExecuteNonQuery();
            }
        }

        protected SqlParameter create_sql_param(string name, SqlDbType type, object value) {

            var param = new SqlParameter();
            param.ParameterName = name;
            param.SqlDbType = type;
            param.Direction = ParameterDirection.Input;

            if (value == null) {
                param.Value = DBNull.Value;
            }
            else {
                param.Value = value;
            }

            return param;
        }

    }

}
