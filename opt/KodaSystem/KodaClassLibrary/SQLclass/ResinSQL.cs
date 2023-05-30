using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;//プロパティの属性値付けるのに使う
using System.Data;
using System.Data.SqlClient;    //データサーバへアクセスするのに使う

using Newtonsoft.Json;

using static KodaClassLibrary.UtilFuncs;

namespace KodaClassLibrary {

    /// <summary>
    /// KODAデータベースに接続して樹脂管理データを扱うクラス
    /// </summary>
    public class ResinSQL : KodaSQL {

        /// <summary>
        /// 樹脂沈降時間(ResinSettlingTime)テーブルのデータクラス
        /// </summary>
        public class ResinSettlingTimeData {

            /// <summary>
            /// 機種名
            /// </summary>
            [Display(Name = "機種名")]
            public virtual string Typecd { get; set; }

            /// <summary>
            /// ロット番号(10桁)
            /// </summary>
            [Display(Name = "10桁ロット番号")]
            public virtual string LotNo10 { get; set; }

            /// <summary>
            /// ロット番号(18桁)
            /// </summary>
            [Display(Name = "18桁ロット番号")]
            public virtual string LotNo18 { get; set; }

            /// <summary>
            /// 製造拠点
            /// </summary>
            [Display(Name = "製造拠点")]
            public virtual string ManuBase { get; set; }

            /// <summary>
            /// 設備設置場所
            /// </summary>
            [Display(Name = "場所")]
            public virtual string Place { get; set; }

            /// <summary>
            /// 沈降棚名
            /// </summary>
            [Display(Name = "棚名")]
            public virtual string Shelf { get; set; }

            /// <summary>
            /// 沈降棚内の保管エリア
            /// </summary>
            [Display(Name = "保管エリア")]
            public virtual string Area { get; set; }

            /// <summary>
            /// 投入日時
            /// </summary>
            [Display(Name = "投入日時")]
            public virtual DateTime InTime { get; set; }

            /// <summary>
            /// 投入作業者
            /// </summary>
            [Display(Name = "投入作業者")]
            public virtual string InputBy { get; set; }

            /// <summary>
            /// 最短取り出し日時
            /// </summary>
            [Display(Name = "最短取り出し日時")]
            public virtual DateTime MinTime { get; set; }

            /// <summary>
            /// キュア投入期限
            /// </summary>
            [Display(Name = "キュア投入期限")]
            public virtual DateTime MaxTime { get; set; }

            /// <summary>
            /// 取り出し日時
            /// </summary>
            [Display(Name = "取り出し日時")]
            public virtual DateTime? OutTime { get; set; }

            /// <summary>
            /// 取り出し作業者 or 登録者
            /// </summary>
            [Display(Name = "取り出し作業者")]
            public virtual string OutputBy { get; set; }

            /// <summary>
            /// ロットの状態
            /// <para>0 : 沈降中</para>
            /// <para>1 : 取り出し完了</para>
            /// <para>2 : エリア移動</para>
            /// </summary>
            [Display(Name = "ロット状態")]
            public virtual int Status { get; set; }
        }

        const string ResinSettlingTime_TABLE = "[dbo].[ResinSettlingTime]";

        const string typecd_COL = "typecd";
        const string lotno10_COL = "lotno10";
        const string lotno18_COL = "lotno18";
        const string manubase_COL = "manubase";
        const string place_COL = "place";
        const string shelf_COL = "shelf";
        const string area_COL = "area";
        const string intime_COL = "intime";
        const string inputby_COL = "inputby";
        const string mintime_COL = "mintime";
        const string maxtime_COL = "maxtime";
        const string outtime_COL = "outtime";
        const string outputby_COL = "outputby";
        const string status_COL = "status";

        /// <summary>
        /// コンストラクタ(データベースへの接続情報を渡す)
        /// </summary>
        public ResinSQL(string data_source, string db_name, string user_id, string pass, int timeout)
            : base(data_source, db_name, user_id, pass, timeout) {

        }

        /// <summary>
        /// コンストラクタ(データベースへの接続文字列を渡す）
        /// </summary>
        public ResinSQL(string con_str)
            : base(con_str) {

        }

        /// <summary>
        /// ResinSettlingTimeテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_ResinSettlingTimeData<T>(string add_where = null, string new_query = null)
            where T : ResinSettlingTimeData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {typecd_COL},{lotno10_COL},{lotno18_COL},
                                  {manubase_COL},{place_COL},{shelf_COL},{area_COL},
                                  {intime_COL},{inputby_COL},{mintime_COL},{maxtime_COL},{outtime_COL},{outputby_COL},{status_COL}
                              FROM {ResinSettlingTime_TABLE} as rst
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
                    Typecd = col.Contains(typecd_COL) ? row[typecd_COL].ToString() : "",
                    LotNo10 = col.Contains(lotno10_COL) ? row[lotno10_COL].ToString() : "",
                    LotNo18 = col.Contains(lotno18_COL) ? row[lotno18_COL].ToString() : "",
                    ManuBase = col.Contains(manubase_COL) ? row[manubase_COL].ToString() : "",
                    Place = col.Contains(place_COL) ? row[place_COL].ToString() : "",
                    Shelf = col.Contains(shelf_COL) ? row[shelf_COL].ToString() : "",
                    Area = col.Contains(area_COL) ? row[area_COL].ToString() : "",
                    InTime = col.Contains(intime_COL) ? (DateTime)row[intime_COL] : new DateTime(1970, 1, 1),
                    InputBy = col.Contains(inputby_COL) ? row[inputby_COL].ToString() : "",
                    MinTime = col.Contains(mintime_COL) ? (DateTime)row[mintime_COL] : new DateTime(1970, 1, 1),
                    MaxTime = col.Contains(maxtime_COL) ? (DateTime)row[maxtime_COL] : new DateTime(1970, 1, 1),
                    OutTime = col.Contains(outtime_COL) ? row[outtime_COL] as DateTime? : null,
                    OutputBy = col.Contains(outputby_COL) ? row[outputby_COL].ToString() : "",
                    Status = col.Contains(status_COL) ? (int)row[status_COL] : -1,
                };

                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// 沈降実施中の機種データを取得する
        /// </summary>
        public List<T> Get_LotList<T>(string manubase, string place)
            where T : ResinSettlingTimeData, new() {

            string add_where = $@"AND {manubase_COL}='{manubase}' 
                                  AND {place_COL}='{place}' 
                                  AND {status_COL}=0";

            //データベースからデータ取得
            var list = Get_ResinSettlingTimeData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 沈降実施中の機種データを取得する
        /// </summary>
        public List<T> Get_LotList<T>(string manubase, string place, string shelf, string area)
            where T : ResinSettlingTimeData, new() {

            string add_where = $@"AND {manubase_COL}='{manubase}' 
                                  AND {place_COL}='{place}' 
                                  AND {shelf_COL}='{shelf}' 
                                  AND {area_COL}='{area}' 
                                  AND {status_COL}=0";

            //データベースからデータ取得
            var list = Get_ResinSettlingTimeData<T>(add_where);

            return list;
        }

        /// <summary>
        /// 沈降実施中の機種データを取得する
        /// </summary>
        public T Get_Lot<T>(string typecd, string lotno10)
            where T : ResinSettlingTimeData, new() {

            string add_where = $@"AND {typecd_COL}='{typecd}' 
                                  AND {lotno10_COL}='{lotno10}' 
                                  AND {status_COL}=0";

            //データベースからデータ取得
            var list = Get_ResinSettlingTimeData<T>(add_where);

            if (list.Count > 1) {
                //システムで登録している限り発生しないはず
                throw new Exception("同じLotが複数登録されています。おかしいです。");
            }

            if (list.Count == 0) {
                return null;
            }

            return list[0];
        }



        /// <summary>
        /// 沈降完了した機種データを取得する
        /// </summary>
        public List<T> Get_Lot<T>(DateTime start, DateTime end)
            where T : ResinSettlingTimeData, new() {

            string add_where = $@"AND {outtime_COL}>'{start.ToString("yyyy/MM/dd 00:00:00")}' 
                                  AND {outtime_COL}<'{end.ToString("yyyy/MM/dd 23:59:59.999")}' 
                                  AND {status_COL}!=0";

            //データベースからデータ取得
            var list = Get_ResinSettlingTimeData<T>(add_where);

            return list;
        }

        /// <summary>
        /// エリア移動した機種データを取得する
        /// </summary>
        public T Get_MoveLot<T>(string typecd, string lotno10)
            where T : ResinSettlingTimeData, new() {

            string add_where = $@"AND {typecd_COL}='{typecd}' 
                                  AND {lotno10_COL}='{lotno10}' 
                                  AND {status_COL}=2";

            //データベースからデータ取得
            var list = Get_ResinSettlingTimeData<T>(add_where);

            if (list.Count > 1) {
                //システムで登録している限り発生しないはず
                throw new Exception("同じLotが複数回移動されています。おかしいです。");
            }

            if (list.Count == 0) {
                return null;
            }

            return list[0];
        }




        /// <summary>
        /// 投入Lotを登録する
        /// </summary>
        public void Insert_Lot<T>(T rst)
           where T : ResinSettlingTimeData, new() {

            string query = $@"INSERT INTO {ResinSettlingTime_TABLE}
                              ({typecd_COL},{lotno10_COL},{lotno18_COL},{status_COL},
                               {manubase_COL},{place_COL},{shelf_COL},{area_COL},
                               {intime_COL},{inputby_COL},{mintime_COL},{maxtime_COL}) 
                              VALUES(@{typecd_COL},@{lotno10_COL},@{lotno18_COL},@{status_COL},
                                     @{manubase_COL},@{place_COL},@{shelf_COL},@{area_COL},
                                     @{intime_COL},@{inputby_COL},@{mintime_COL},@{maxtime_COL})";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{typecd_COL}", SqlDbType.VarChar, rst.Typecd));
            param_list.Add(create_sql_param($"@{lotno10_COL}", SqlDbType.VarChar, rst.LotNo10));
            param_list.Add(create_sql_param($"@{lotno18_COL}", SqlDbType.VarChar, rst.LotNo18));
            param_list.Add(create_sql_param($"@{status_COL}", SqlDbType.Int, 0));
            param_list.Add(create_sql_param($"@{manubase_COL}", SqlDbType.VarChar, rst.ManuBase));
            param_list.Add(create_sql_param($"@{place_COL}", SqlDbType.NVarChar, rst.Place));
            param_list.Add(create_sql_param($"@{shelf_COL}", SqlDbType.NVarChar, rst.Shelf));
            param_list.Add(create_sql_param($"@{area_COL}", SqlDbType.NVarChar, rst.Area));
            param_list.Add(create_sql_param($"@{intime_COL}", SqlDbType.DateTime, rst.InTime));
            param_list.Add(create_sql_param($"@{inputby_COL}", SqlDbType.NVarChar, rst.InputBy));
            param_list.Add(create_sql_param($"@{mintime_COL}", SqlDbType.DateTime, rst.MinTime));
            param_list.Add(create_sql_param($"@{maxtime_COL}", SqlDbType.DateTime, rst.MaxTime));

            execute(query, param_list);
        }

        /// <summary>
        /// 沈降中Lotの取り出し情報を更新する
        /// </summary>
        public void Update_Lot<T>(T rst, int status)
           where T : ResinSettlingTimeData, new() {

            string query = $@"UPDATE {ResinSettlingTime_TABLE}
                              SET {outtime_COL}=@{outtime_COL},
                                  {outputby_COL}=@{outputby_COL},
                                  {status_COL}=@{status_COL}
                              WHERE {typecd_COL}='{rst.Typecd}' AND {lotno10_COL}='{rst.LotNo10}' AND {status_COL}=0
                             ";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{outtime_COL}", SqlDbType.DateTime, rst.OutTime));
            param_list.Add(create_sql_param($"@{outputby_COL}", SqlDbType.NVarChar,rst.OutputBy));
            param_list.Add(create_sql_param($"@{status_COL}", SqlDbType.Int, status));

            execute(query, param_list);
        }

    }
}
