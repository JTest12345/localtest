using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using System.Data.SqlClient;    //データサーバへアクセスするのに使う


using System.Data;
using System.ComponentModel.DataAnnotations;//プロパティの属性値付けるのに使う

namespace KodaClassLibrary {

    /// <summary>
    /// KODAデータベースに接続しておんどとりデータを扱うクラス
    /// </summary>
    public class OndotoriSQL : KodaSQL {

        /// <summary>
        /// Ondotoriテーブルのデータクラス
        /// </summary>
        public class OndotoriData {

            /// <summary>
            /// 親機名
            /// </summary>
            [Display(Name = "親機名")]
            public virtual string BaseName { get; set; }

            /// <summary>
            /// 親機型式
            /// </summary>
            [Display(Name = "親機型式")]
            public virtual string BaseModel { get; set; }

            /// <summary>
            /// 親機シリアル番号
            /// </summary>
            [Display(Name = "親機シリアル番号")]
            public virtual string BaseSerial { get; set; }

            /// <summary>
            /// グループ名
            /// </summary>
            [Display(Name = "グループ名")]
            public virtual string GroupName { get; set; }

            /// <summary>
            /// 子機名
            /// </summary>
            [Display(Name = "子機名")]
            public virtual string RemoteName { get; set; }

            /// <summary>
            /// 子機型式
            /// </summary>
            [Display(Name = "子機型式")]

            public virtual string RemoteModel { get; set; }

            /// <summary>
            /// 子機シリアル番号
            /// </summary>
            [Display(Name = "子機シリアル番号")]

            public virtual string RemoteSerial { get; set; }

            /// <summary>
            /// 電波強度
            /// </summary>
            [Display(Name = "電波強度")]
            public virtual int Rssi { get; set; }

            /// <summary>
            /// 日時
            /// </summary>
            [Display(Name = "日時")]
            public virtual DateTime Time { get; set; }

            /// <summary>
            /// 現在値の時刻（世界協定時刻(UTC) 1970年1月1日からの経過秒数）
            /// </summary>
            [Display(Name = "UnixTime")]
            public virtual long UnixTime { get; set; }

            /// <summary>
            /// 温度
            /// </summary>
            [Display(Name = "温度")]
            public virtual double? Temperature { get; set; }

            /// <summary>
            /// エラー
            /// </summary>
            public virtual string Error { get; set; }

            /// <summary>
            /// 温度の単位　Cはセルシウス
            /// </summary>
            public virtual string Unit { get; set; }

            /// <summary>
            /// 電池残量
            /// </summary>
            [Display(Name = "電池残量")]
            public virtual int? Battery { get; set; }

            /// <summary>
            /// 情報が書いてあるXML
            /// </summary>
            public virtual string Xml { get; set; }

        }

        const string Ondotori_TABLE = "[dbo].[Ondotori]";

        const string basename_COL = "basename";
        const string basemodel_COL = "basemodel";
        const string baseserial_COL = "baseserial";
        const string groupname_COL = "groupname";
        const string remotename_COL = "remotename";
        const string remotemodel_COL = "remotemodel";
        const string remoteserial_COL = "remoteserial";
        const string rssi_COL = "rssi";
        const string time_COL = "time";
        const string unixtime_COL = "unixtime";
        const string temp_COL = "temp";
        const string error_COL = "error";
        const string unit_COL = "unit";
        const string batt_COL = "batt";
        const string xml_COL = "xml";

        /// <summary>
        /// コンストラクタ(データベースへの接続情報を渡す)
        /// </summary>
        public OndotoriSQL(string data_source, string db_name, string user_id, string pass, int timeout)
            : base(data_source, db_name, user_id, pass, timeout) {

        }

        /// <summary>
        /// コンストラクタ(データベースへの接続文字列を渡す）
        /// </summary>
        public OndotoriSQL(string con_str)
            : base(con_str) {

        }

        /// <summary>
        /// おんどとりテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_OndotoriData<T>(string add_where = null, string new_query = null)
            where T : OndotoriData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {basename_COL},{basemodel_COL},{baseserial_COL},{groupname_COL},
                                  {remotename_COL},{remotemodel_COL},{remoteserial_COL},
                                  {rssi_COL},{time_COL},{unixtime_COL},{temp_COL},{error_COL},{unit_COL},
                                  {batt_COL}
                              FROM {Ondotori_TABLE} as ot
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
                    BaseName = col.Contains(basename_COL) ? row[basename_COL].ToString() : "",
                    BaseModel = col.Contains(basemodel_COL) ? row[basemodel_COL].ToString() : "",
                    BaseSerial = col.Contains(baseserial_COL) ? row[baseserial_COL].ToString() : "",
                    GroupName = col.Contains(groupname_COL) ? row[groupname_COL].ToString() : "",
                    RemoteName = col.Contains(remotename_COL) ? row[remotename_COL].ToString() : "",
                    RemoteModel = col.Contains(remotemodel_COL) ? row[remotemodel_COL].ToString() : "",
                    RemoteSerial = col.Contains(remoteserial_COL) ? row[remoteserial_COL].ToString() : "",
                    Rssi = col.Contains(rssi_COL) ? (int)row[rssi_COL] : -1,
                    Time = col.Contains(time_COL) ? (DateTime)row[time_COL] : new DateTime(1970, 1, 1),
                    UnixTime = col.Contains(unixtime_COL) ? (long)row[unixtime_COL] : 0,
                    Temperature = col.Contains(temp_COL) ? row[temp_COL] as double? : null,
                    Error = col.Contains(error_COL) ? row[error_COL].ToString() : "",
                    Unit = col.Contains(unit_COL) ? row[unit_COL].ToString() : "",
                    Battery = col.Contains(batt_COL) ? row[batt_COL] as int? : null,
                };

                //Local時刻のプロパティ設定
                data.Time = DateTime.SpecifyKind(data.Time, DateTimeKind.Local);

                list.Add(data);

            }

            return list;
        }

        /// <summary>
        /// 重複しない子機のリストを取得する
        /// </summary>
        public List<T> Get_RemoteList<T>()
            where T : OndotoriData, new() {

            string new_query = $@"SELECT DISTINCT {basename_COL},{groupname_COL},{remotename_COL}
                                  FROM {Ondotori_TABLE}";

            //データベースからデータ取得
            var list = Get_OndotoriData<T>(new_query: new_query);

            return list;
        }

        /// <summary>
        /// 指定した子機の指定した期間(日付)のデータを取得する
        /// </summary>
        public List<T> Get_DataList<T>(T obj, DateTime start, DateTime end)
            where T : OndotoriData, new() {

            string add_where = $@"AND {basename_COL}='{obj.BaseName}'
                                  AND {groupname_COL}='{obj.GroupName}'
                                  AND {remotename_COL}='{obj.RemoteName}'
　　　　　　　　　　　　　　　　　AND {time_COL}>'{start.ToString("yyyy/MM/dd 00:00:00")}' AND {time_COL}<'{end.ToString("yyyy/MM/dd 23:59:59")}'
                                  ORDER BY {time_COL}";

            //データベースからデータ取得
            var list = Get_OndotoriData<T>(add_where);

            return list;
        }

        /// <summary>
        /// データベースのOndotoriテーブルに書き込む
        /// </summary>
        public void Insert_CurrentData(RTR500BC.XMLObject obj) {

            string query = $@"INSERT INTO {Ondotori_TABLE}
                                          ({basename_COL},{basemodel_COL},{baseserial_COL},
                                           {groupname_COL},{remotename_COL},{remotemodel_COL},{remoteserial_COL},
                                           {rssi_COL},{time_COL},{unixtime_COL},{temp_COL},{error_COL},{unit_COL},{batt_COL})
                              VALUES";

            var list = new List<string>();

            foreach (var gr in obj.Root.Group) {
                foreach (var rem in gr.Remote) {

                    string str = $@"('{obj.Root.Base.Name}','{obj.Root.Base.Model}','{obj.Root.Base.Serial}','{gr.Name}','{rem.Name}','{rem.Model}','{rem.Serial}',{rem.Rssi},";

                    var cur = rem.CH[0].Current;

                    str += $@"'{cur.TimeStr}',";

                    //データベースに書き込むデータは分単位にする
                    //2022-01-01 12:34:56のデータだった場合
                    //2022-01-01 12:34:00～12:34:59のデータが既にあるかチェックする
                    //TimeStrには2022-01-01 12:34:56の書式で入っているはず
                    string mi = cur.TimeStr.Substring(0, cur.TimeStr.Length - 2);
                    string check_query = $@"SELECT * 
                                            FROM {Ondotori_TABLE}
                                            WHERE {basename_COL}='{obj.Root.Base.Name}' and {groupname_COL}='{gr.Name}'
                                              and {remotename_COL}='{rem.Name}'
                                              and {time_COL} BETWEEN '{mi}00' and '{mi}59'";

                    //既に同じ時間（分）のデータがあれば次のデータへ
                    DataTable table = read_data(check_query);
                    if (table.Rows.Count != 0) {
                        continue;
                    }

                    //UTC unix time
                    str += $@"{cur.UnixTime},";

                    //温度またはエラーメッセージ
                    //測定単位、バッテリー
                    if (cur.Value.Valid) {
                        str += $@"{double.Parse(cur.Value.ValueStr)},NULL,'{cur.Unit}',{int.Parse(cur.Batt)}";
                    }
                    else {
                        str += $@"NULL,'{cur.Value.ValueStr}',NULL,NULL";
                    }

                    //xmlテキスト
                    //str += $@"'{obj.XmlText}')";

                    str += ")";

                    list.Add(str);
                }
            }

            if (list.Count == 0) {
                return;
            }

            query += String.Join(",", list);
            query += ";";

            execute(query);

        }

    }
}

