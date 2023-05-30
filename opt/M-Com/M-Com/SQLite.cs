using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data.SQLite;

/*
 * SQLiteにデータ保存するため
 * Nugetで"System.Data.SQLite.Core"をインストールして使う
 */

namespace M_Com {
    class SQLite {

        //SQliteで使える値のデータ型
        //NULL      NULL値
        //INTEGER   符号付整数。1, 2, 3, 4, 6, or 8 バイトで格納
        //REAL      浮動小数点数。8バイトで格納
        //TEXT      テキスト。UTF-8, UTF-16BE or UTF-16-LEのいずれかで格納
        //BLOB      Binary Large OBject。入力データをそのまま格納

        //SQliteで使えるカラムのデータ型
        //TEXT      データ型が文字列 CHAR 、 CLOB 、 TEXT のいずれかを含む場合 TEXT 型となります
        //NUMERIC   それ以外は NUMERIC 型となります
        //INTEGER　 データ型が文字列 INT を含む場合 INTEGER 型となります。
        //REAL      データ型が文字列 REAL 、 FLOA 、 DOUB のいずれかを含む場合 REAL 型となります。
        //NONE      データ型が文字列 BLOB を含む場合、またデータ型が指定されなかった場合は NONE 型となります。


        /// <summary>
        /// 接続文字列作成しやすくするために使う
        /// </summary>
        private SQLiteConnectionStringBuilder builder = new SQLiteConnectionStringBuilder();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="filepath">データベースの名前（SQLiteの場合はファイルパス）</param>
        public SQLite(string filepath) {

            builder.DataSource = filepath;
            builder.Version = 3;
        }

        /// <summary>
        /// テーブルを作成する
        /// </summary>
        public void create_table(string tableName, Dictionary<string, string> column_dic) {

            List<string> keys = column_dic.Keys.ToList();

            string column_name = "";

            for (int i = 0; i < column_dic.Count; i++) {

                column_name += keys[i] + " " + column_dic[keys[i]];

                if (i != column_dic.Count - 1) {
                    column_name += ",";
                }
            }

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                string sql = $"create table {tableName} ({column_name})";

                SQLiteCommand com = new SQLiteCommand(sql, con);

                con.Open();

                //データベースを変更するSQLにはExecuteNonQueryを使う
                com.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// テーブルを削除する
        /// </summary>
        /// <param name="tableName"></param>
        public void drop_table(string tableName) {

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                string sql = $"drop table {tableName}";

                SQLiteCommand com = new SQLiteCommand(sql, con);

                con.Open();

                //データベースを変更するSQLにはExecuteNonQueryを使う
                com.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 測定データを1レコード分データ追加する
        /// </summary>
        public void insert_data(AddSqlData asd) {
            string tableName = "DATA";

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                //SQL文作成
                string sql = $"insert into {tableName} values({asd.InsertString})";

                SQLiteCommand com = new SQLiteCommand(sql, con);

                con.Open();

                //データベースを変更するSQLにはExecuteNonQueryを使う
                com.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// 測定データをデータベースに追加する
        /// </summary>
        /// <param name="asd_list"></param>
        public void insert_data(List<AddSqlData> asd_list) {
            string tableName = "DATA";

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                con.Open();

                //usingステートメントを使えばSqlTransactionの明示的なRollbackは不要
                //transactionはセーブポイント　異常時はtransaction.Rollbackして最初の状態に戻すが
                //usingを使用しているので書かなくていい　tryの時はcatchのところに書く
                using (var transaction = con.BeginTransaction()) {

                    asd_list.ForEach(asd =>
                    {
                        //SQL文作成
                        string sql = $"insert into {tableName} values({asd.InsertString})";

                        SQLiteCommand com = new SQLiteCommand(sql, con);

                        //データベースを変更するSQLにはExecuteNonQueryを使う
                        com.ExecuteNonQuery();
                    });

                    //データを確定させる
                    transaction.Commit();
                }
            }
        }


        /// <summary>
        /// 統計データを追加する
        /// </summary>
        /// <param name="assd"></param>
        public void insert_data(AddSqlStatData assd) {
            string tableName = "STATDATA";

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                con.Open();

                //usingステートメントを使えばSqlTransactionの明示的なRollbackは不要
                //transactionはセーブポイント　異常時はtransaction.Rollbackして最初の状態に戻すが
                //usingを使用しているので書かなくていい　tryの時はcatchのところに書く
                using (var transaction = con.BeginTransaction()) {

                    assd.ItemStatDic.Keys.ToList().ForEach(key =>
                    {
                        //SQL文作成
                        string sql = $"insert into {tableName} values({assd.get_InsertString(key)})";

                        SQLiteCommand com = new SQLiteCommand(sql, con);

                        //データベースを変更するSQLにはExecuteNonQueryを使う
                        com.ExecuteNonQuery();
                    });

                    //データを確定させる
                    transaction.Commit();
                }
            }
        }



        public void update_data(string tableName) {
            //まず更新の対象となるデータを WHERE 句の条件式を使って指定します。
            //条件式に一致するデータが複数の場合は、複数のデータがそれぞれ更新されることになります。 
            //WHERE を省略した場合はテーブルに含まれる全てのデータが更新されます。

            //次にどの値を更新するのかをカラム名と値で指定します。
            //複数のカラムの値を一度に更新することができます。

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                string sql = $"update {tableName} set id=4,name = 'ｎａｋａｊｉｍａ　' where id = 5";

                SQLiteCommand com = new SQLiteCommand(sql, con);

                con.Open();

                com.ExecuteNonQuery();

            }
        }

        public void delete_data(string tableName) {

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                string sql = $"delete from {tableName} where id=2";

                SQLiteCommand com = new SQLiteCommand(sql, con);

                con.Open();

                com.ExecuteNonQuery();
            }
        }


        public void select_data() {

            using (var con = new SQLiteConnection(builder.ConnectionString)) {

                string sql = "select DateTime, Item from DATA;";

                SQLiteCommand com = new SQLiteCommand(sql, con);

                con.Open();

                //データベースにクエリを送信して、取得した結果を処理
                //1行1ずつ読み込む方法
                using (var reader = com.ExecuteReader()) {

                    string s = "";
                    while (reader.Read()) {

                        s += reader["DateTime"] + "," + reader["Item"] + "\n";
                    }
                }
            }
        }


        /// <summary>
        /// 測定データをデータベースに追加する時に使う
        /// </summary>
        public class AddSqlData {

            public string Product { get; private set; }

            public string LotNo { get; private set; }

            public string OperatorNo { get; private set; }

            public int Machine { get; private set; }

            public string DateTime { get; private set; }

            public string Place { get; private set; }

            public string Item { get; private set; }

            public double Data { get; private set; }

            public string Judge { get; private set; }

            public string MeasureSerial { get; private set; }

            /// <summary>
            /// 測定データを追加する時に使う
            /// </summary>
            public string InsertString {
                get {
                    //string型は''で囲む
                    //機種　LotNo　測定者　設備No　日時　測定箇所　測定項目　測定値　判定　メジャリングシリアル
                    string s = $"'{Product}','{LotNo}','{OperatorNo}',{Machine},'{DateTime}','{Place}','{Item}',{Data},'{Judge}','{MeasureSerial}'";
                    return s;
                }
            }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="product"></param>
            /// <param name="lotno"></param>
            /// <param name="ope"></param>
            /// <param name="machine"></param>
            /// <param name="dt"></param>
            /// <param name="place"></param>
            /// <param name="item"></param>
            /// <param name="data"></param>
            /// <param name="judge"></param>
            public AddSqlData(string product, string lotno, string ope, string machine, string dt, string place, string item, string data, string judge, string measure_serial) {
                Product = product;
                LotNo = lotno;
                OperatorNo = ope;
                Machine = int.Parse(machine);
                DateTime = dt;
                Place = place;
                Item = item;
                Data = double.Parse(data);
                Judge = judge;
                MeasureSerial = measure_serial;
            }
        }

        /// <summary>
        /// 統計データをデータベースに追加する時に使う
        /// </summary>
        public class AddSqlStatData {

            public string Product { get; private set; }

            public string LotNo { get; private set; }

            public string OperatorNo { get; private set; }

            public int Machine { get; private set; }

            public string DateTime { get; private set; }

            public Dictionary<string, SampleData> ItemStatDic { get; private set; }

            /// <summary>
            /// コンストラクタ
            /// </summary>
            /// <param name="product"></param>
            /// <param name="lotno"></param>
            /// <param name="ope"></param>
            /// <param name="machine"></param>
            /// <param name="dt"></param>
            /// <param name="item_stat_dic"></param>
            public AddSqlStatData(string product, string lotno, string ope, string machine, string dt, Dictionary<string, SampleData> item_stat_dic) {
                Product = product;
                LotNo = lotno;
                OperatorNo = ope;
                Machine = int.Parse(machine);
                DateTime = dt;
                ItemStatDic = item_stat_dic;
            }

            /// <summary>
            /// 統計データを追加する時に使うSQL分のvalueの部分を返す
            /// </summary>
            /// <param name="key">ItemStatDicのキー</param>
            /// <returns></returns>
            public string get_InsertString(string key) {

                //string型は''で囲む
                string s = "";
                string place;
                string item;

                //機種,LotNo,測定者,設備No,日時,
                s += $"'{Product}','{LotNo}','{OperatorNo}',{Machine},'{DateTime}',";

                //測定箇所,測定項目,
                place = key.Split('_')[0];
                item = key.Split('_')[1];
                s += $"'{place}','{item}',";

                //測定数,最大,平均,最小,レンジ,
                s += $"{ItemStatDic[key].Count},{ItemStatDic[key].Max},{ItemStatDic[key].Ave},{ItemStatDic[key].Min},{ItemStatDic[key].Range},";

                //σ,
                if (ItemStatDic[key].Sigma == 0 || double.IsNaN(ItemStatDic[key].Sigma)) {

                    s += "null,";
                }
                else {
                    s += $"{ItemStatDic[key].Sigma},";
                }

                //Cpk
                if (double.IsNaN(ItemStatDic[key].Get_Cpk())) {
                    s += $"null";
                }
                else {
                    s += $"{ItemStatDic[key].Get_Cpk()}";
                }

                //機種　LotNo　測定者　設備No　日時　測定箇所　測定項目　測定数　最大　平均　最小　レンジ　σ　Cpk
                return s;
            }
        }






















    }
}
