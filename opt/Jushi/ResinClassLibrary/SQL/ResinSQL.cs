using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;    //データサーバへアクセスするのに使う

using Newtonsoft.Json;
using System.Diagnostics;
using System.Security.Cryptography;

namespace ResinClassLibrary {

    public class ResinSQL : SQL {

        /// <summary>
        /// ResinCupRecipeテーブルのデータクラス
        /// </summary>
        public class ResinCupRecipe_Data {

            /// <summary>
            /// 樹脂カップ番号
            /// </summary>
            [JsonProperty("cupno")]
            public virtual string CupNo { get; set; }

            /// <summary>
            /// 書き込み日時
            /// </summary>
            [JsonProperty("insertat")]
            public virtual DateTimeOffset InsertAt { get; set; }

            /// <summary>
            /// 樹脂カップ番号の連番部分
            /// </summary>
            [JsonProperty("number")]
            public virtual int Number { get; set; }

            /// <summary>
            /// 機種名
            /// </summary>
            [JsonProperty("productName")]
            public virtual string ProductName { get; set; }

            /// <summary>
            /// 波長ランク
            /// </summary>
            [JsonProperty("waveLengthRank")]
            public virtual string WaveLengthRank { get; set; }

            /// <summary>
            /// この樹脂カップを使う製品ロット番号(スラッシュ区切り)
            /// </summary>
            [JsonProperty("lot")]
            public virtual string Lots { get; set; }

            /// <summary>
            /// レシピ内容のJSON文字列
            /// </summary>
            [JsonProperty("recipeContent")]
            public virtual string RecipeContent { get; set; }
        }

        /// <summary>
        /// ResinSenkoLogテーブルのデータクラス
        /// </summary>
        public class ResinSenkoLog_Data {

            /// <summary>
            /// 書き込み日時
            /// </summary>
            [JsonProperty("insertat")]
            public virtual DateTimeOffset InsertAt { get; set; }

            /// <summary>
            /// 書き込みした人
            /// </summary>
            [JsonProperty("insertby")]
            public virtual string InsertBy { get; set; }

            /// <summary>
            /// 機種名
            /// </summary>
            [JsonProperty("productname")]
            public virtual string ProductName { get; set; }

            /// <summary>
            /// 波長ランク
            /// </summary>
            [JsonProperty("wavelengthrank")]
            public virtual string WavelengthRank { get; set; }

            /// <summary>
            /// 樹脂カップ作業手順コード
            /// </summary>
            [JsonProperty("mixtypecd")]
            public virtual string MixTypeCode { get; set; }

            /// <summary>
            /// 樹脂種類タイプ
            /// </summary>
            [JsonProperty("moldtype")]
            public virtual string MoldType { get; set; }

            /// <summary>
            /// 条件
            /// </summary>
            [JsonProperty("conditions")]
            public virtual string Conditions { get; set; }

            /// <summary>
            /// 特殊指定
            /// </summary>
            [JsonProperty("specialdesignation")]
            public virtual string SpecialDesignation { get; set; }

            /// <summary>
            /// 結果/投入指示
            /// </summary>
            [JsonProperty("result")]
            public virtual string ResultOrInput { get; set; }

            /// <summary>
            /// 評価方法
            /// </summary>
            [JsonProperty("method")]
            public virtual string EvaluationMethod { get; set; }

            /// <summary>
            /// 製造場所コード
            /// </summary>
            [JsonProperty("mixtypecd")]
            public virtual string Place { get; set; }

            /// <summary>
            /// ベース樹脂材料(基本は100g)
            /// </summary>
            [JsonProperty("baseamount")]
            public virtual decimal BaseAmount { get; set; }

            /// <summary>
            /// 使用部材情報のJSON文字列
            /// </summary>
            [JsonProperty("buzai")]
            public virtual string Buzai { get; set; }

            /// <summary>
            /// ろ過後使用部材情報のJSON文字列
            /// </summary>
            [JsonProperty("afterfiltbuzai")]
            public virtual string AfterFiltBuzai { get; set; }

            /// <summary>
            /// Excel先行ログの行
            /// </summary>
            [JsonProperty("excelSenkoLogRow")]
            public virtual int ExcelSenkoLogRow { get; set; }
        }

        const string ResinCupRecipe_TABLE = "[dbo].[ResinCupRecipe]";
        const string ResinSenkoLog_TABLE = "[dbo].[ResinSenkoLog]";

        const string cupno_COL = "cupno";
        const string insertat_COL = "insertat";
        const string number_COL = "number";
        const string lotno_COL = "lotno";
        const string recipe_COL = "recipe";

        const string insertby_COL = "insertby";
        const string productname_COL = "productname";
        const string wavelengthrank_COL = "wavelengthrank";
        const string mixtypecd_COL = "mixtypecd";
        const string moldtype_COL = "moldtype";
        const string specialdesignation_COL = "specialdesignation";
        const string conditions_COL = "conditions";
        const string result_COL = "result";
        const string method_COL = "method";
        const string place_COL = "place";
        const string baseamount_COL = "baseamount";
        const string buzai_COL = "buzai";
        const string afterfiltbuzai_COL = "afterfiltbuzai";
        const string excelrow_COL = "excelrow";

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
        /// ResinCupRecipeテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_ResinCupRecipe_Data<T>(string add_where = null, string new_query = null, List<SqlParameter> param_list = null)
            where T : ResinCupRecipe_Data, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {cupno_COL},{insertat_COL},{number_COL},{productname_COL},{wavelengthrank_COL},{lotno_COL},{recipe_COL}
                             FROM {ResinCupRecipe_TABLE} as rcr
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
            var table = read_data(query, param_list);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    CupNo = col.Contains(cupno_COL) ? row[cupno_COL].ToString() : "",
                    InsertAt = col.Contains(insertat_COL) ? (DateTimeOffset)row[insertat_COL] : new DateTimeOffset(new DateTime(1970, 1, 1)),
                    Number = col.Contains(number_COL) ? (int)row[number_COL] : -1,
                    ProductName = col.Contains(productname_COL) ? row[productname_COL].ToString() : "",
                    WaveLengthRank = col.Contains(wavelengthrank_COL) ? row[wavelengthrank_COL].ToString() : "",
                    Lots = col.Contains(lotno_COL) ? row[lotno_COL].ToString() : "",
                    RecipeContent = col.Contains(recipe_COL) ? row[recipe_COL].ToString() : ""
                };

                list.Add(data);
            }

            return list;
        }

        /// <summary>
        /// ResinSenkoLogテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_ResinSenkoLog_Data<T>(string add_where = null, string new_query = null, List<SqlParameter> param_list = null)
            where T : ResinSenkoLog_Data, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {insertat_COL},{insertby_COL},{productname_COL},{wavelengthrank_COL},{mixtypecd_COL},{moldtype_COL},{specialdesignation_COL},
                                  {conditions_COL},{result_COL},{method_COL},{place_COL},{baseamount_COL},{buzai_COL},{afterfiltbuzai_COL}
                             FROM {ResinSenkoLog_TABLE} as rsl
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
            var table = read_data(query, param_list);

            var list = new List<T>();

            //データが1個もない時は空のリスト(Count=0)を返す
            if (table.Rows.Count == 0) { return list; }

            var col = table.Columns;
            for (int i = 0; i < table.Rows.Count; i++) {
                var row = table.Rows[i];

                var data = new T {
                    InsertAt = col.Contains(insertat_COL) ? (DateTimeOffset)row[insertat_COL] : new DateTimeOffset(new DateTime(1970, 1, 1)),
                    InsertBy = col.Contains(insertby_COL) ? row[insertby_COL].ToString() : "",
                    ProductName = col.Contains(productname_COL) ? row[productname_COL].ToString() : "",
                    WavelengthRank = col.Contains(wavelengthrank_COL) ? row[wavelengthrank_COL].ToString() : "",
                    MixTypeCode = col.Contains(mixtypecd_COL) ? row[mixtypecd_COL].ToString() : "",
                    MoldType = col.Contains(moldtype_COL) ? row[moldtype_COL].ToString() : "",
                    SpecialDesignation = col.Contains(specialdesignation_COL) ? row[specialdesignation_COL].ToString() : "",
                    Conditions = col.Contains(conditions_COL) ? row[conditions_COL].ToString() : "",
                    ResultOrInput = col.Contains(result_COL) ? row[result_COL].ToString() : "",
                    EvaluationMethod = col.Contains(method_COL) ? row[method_COL].ToString() : "",
                    Place = col.Contains(place_COL) ? row[place_COL].ToString() : "",
                    BaseAmount = col.Contains(baseamount_COL) ? (decimal)row[baseamount_COL] : -1,
                    Buzai = col.Contains(buzai_COL) ? row[buzai_COL].ToString() : "",
                    AfterFiltBuzai = col.Contains(afterfiltbuzai_COL) ? row[afterfiltbuzai_COL].ToString() : "",
                };

                list.Add(data);
            }

            return list;
        }


        /// <summary>
        /// 本日の樹脂カップ番号の最大番号を取得
        /// </summary>
        /// <param name="place">製造場所(TS、TCとか)</param>
        public int Get_TodayMaxNumber(string place, DateTimeOffset now) {

            var next = now.AddDays(1);

            string query = $@"SELECT MAX({number_COL})
                                FROM {ResinCupRecipe_TABLE}
                               WHERE {cupno_COL} LIKE '{place}%' 
                                 AND {insertat_COL}>='{now.ToString("yyyy/MM/dd 00:00:00 zzz")}' 
                                 AND {insertat_COL}<'{next.ToString("yyyy/MM/dd 00:00:00 zzz")}'
                              ;";

            var table = read_data(query);

            //1個しか返ってこないので
            var val = table.Rows[0].ItemArray[0];

            int num = 0;
            if (string.IsNullOrEmpty(val.ToString()) == false) {
                num = (int)val;
            }

            return num;
        }

        /// <summary>
        /// 樹脂カップレシピリストを取得する
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="date">カップ番号ルールの日付　2022/12/7 ⇒ 221207</param>
        /// <returns></returns>
        public List<T> Get_ResinCupList<T>(string date) where T : ResinCupRecipe_Data, new() {

            string add_where = $@"AND {cupno_COL} LIKE '%{date}%'";

            //データベースからデータ取得
            var list = Get_ResinCupRecipe_Data<T>(add_where);

            return list;
        }


        /// <summary>
        /// 樹脂カップ番号レコード挿入
        /// </summary>
        /// <param name="place">製造場所(TS、TCとか)</param>
        public string Insert_CupNo(string place, Recipe recipe) {

            var now = DateTimeOffset.Now;

            int num = Get_TodayMaxNumber(place, now);

            int next_num = num + 1;

            string cupno = $"{place}{now.ToString("yyMMdd")}{next_num.ToString("d3")}";

            string query = $@"INSERT INTO {ResinCupRecipe_TABLE}
                                     ({cupno_COL},{insertat_COL},{number_COL},{productname_COL},{wavelengthrank_COL},{lotno_COL})
                              VALUES (@{cupno_COL},@{insertat_COL},@{number_COL},@{productname_COL},@{wavelengthrank_COL},@{lotno_COL});";

            var param_list = new List<SqlParameter>();
            param_list.Add(create_sql_param($"@{cupno_COL}", SqlDbType.NVarChar, cupno));
            param_list.Add(create_sql_param($"@{insertat_COL}", SqlDbType.DateTimeOffset, now));
            param_list.Add(create_sql_param($"@{number_COL}", SqlDbType.Int, next_num));
            param_list.Add(create_sql_param($"@{productname_COL}", SqlDbType.NVarChar, recipe.ProductName));
            param_list.Add(create_sql_param($"@{wavelengthrank_COL}", SqlDbType.NVarChar, recipe.WavelengthRank));
            param_list.Add(create_sql_param($"@{lotno_COL}", SqlDbType.NVarChar, string.Join("/", recipe.LotNoList)));

            execute(query, param_list);

            return cupno;
        }

        /// <summary>
        /// レシピ内容を書き込む
        /// </summary>
        /// <param name="cupno"></param>
        /// <param name="recipe_content">レシピ内容の文字列</param>
        public void Update_CupnoRecipe(string cupno, string recipe_content) {

            string query = $@"UPDATE {ResinCupRecipe_TABLE}
                                 SET {recipe_COL}=@{recipe_COL}
                               WHERE {cupno_COL}='{cupno}';";

            var param_list = new List<SqlParameter>();
            param_list.Add(create_sql_param($"@{recipe_COL}", SqlDbType.NVarChar, recipe_content));

            execute(query, param_list);
        }



        /// <summary>
        /// 最新の先行評価ログデータ取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="pn">機種名(CS0209.....とか)</param>
        /// <param name="wr">波長ランク(D3とか)</param>
        /// <param name="mt">樹脂種類(MDとか)</param>
        /// <param name="sd">特殊指定(DIFとか)</param>
        /// <param name="cond">条件(本番確定とか)</param>
        /// <param name="place">製造場所(TS、TCとか)</param>
        /// <returns></returns>
        public T Get_LatestSenkoLogData<T>(string pn, string wr, string mt, string sd, string cond, string place)
            where T : ResinSenkoLog_Data, new() {

            string query = $@"SELECT TOP(1)
                                     {insertat_COL},{insertby_COL},{productname_COL},{wavelengthrank_COL},{mixtypecd_COL},{moldtype_COL},{specialdesignation_COL},
                                     {conditions_COL},{result_COL},{method_COL},{place_COL},{baseamount_COL},{buzai_COL},{afterfiltbuzai_COL}
                                FROM {ResinSenkoLog_TABLE} as rsl
                               WHERE {place_COL}='{place}' AND {productname_COL}='{pn}' AND {wavelengthrank_COL}='{wr}' 
                                 AND {moldtype_COL}='{mt}' AND {specialdesignation_COL}='{sd}' AND {conditions_COL}='{cond}' 
                            ORDER BY {insertat_COL} DESC;";

            var list = Get_ResinSenkoLog_Data<T>(new_query: query);

            if (list.Count == 0) {
                throw new Exception("データベースに先行ログ情報が見つかりませんでした。\n配合情報が分かりません。");
            }

            //1個しかデータは無いのでindex=0で返す
            return list[0];
        }

        /// <summary>
        /// 先行評価ログデータ取得
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> Get_SenkoLogDataList<T>(string product_name = null)
            where T : ResinSenkoLog_Data, new() {


            string add_where = null;

            if (product_name != null) {
                add_where = $@"AND {productname_COL}='{product_name}'";
            }

            var list = Get_ResinSenkoLog_Data<T>(add_where);

            return list;
        }


        public void Insert_SenkoLogData<T>(T data) where T : ResinSenkoLog_Data {

            string query = $@"INSERT INTO {ResinSenkoLog_TABLE}
                              ({insertat_COL},{insertby_COL},{place_COL},{productname_COL},{wavelengthrank_COL},
                               {moldtype_COL},{mixtypecd_COL},{specialdesignation_COL},{conditions_COL},{result_COL},
                               {method_COL},{baseamount_COL},{buzai_COL},{afterfiltbuzai_COL},{excelrow_COL})
                              VALUES(@{insertat_COL},@{insertby_COL},@{place_COL},@{productname_COL},@{wavelengthrank_COL},
                                     @{moldtype_COL},@{mixtypecd_COL},@{specialdesignation_COL},@{conditions_COL},@{result_COL},
                                     @{method_COL},@{baseamount_COL},@{buzai_COL},@{afterfiltbuzai_COL},@{excelrow_COL});";



            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{insertat_COL}", SqlDbType.DateTimeOffset, data.InsertAt));
            param_list.Add(create_sql_param($"@{insertby_COL}", SqlDbType.NVarChar, data.InsertBy));
            param_list.Add(create_sql_param($"@{place_COL}", SqlDbType.NVarChar, data.Place));
            param_list.Add(create_sql_param($"@{productname_COL}", SqlDbType.NVarChar, data.ProductName));
            param_list.Add(create_sql_param($"@{wavelengthrank_COL}", SqlDbType.NVarChar, data.WavelengthRank));
            param_list.Add(create_sql_param($"@{moldtype_COL}", SqlDbType.NVarChar, data.MoldType));
            param_list.Add(create_sql_param($"@{mixtypecd_COL}", SqlDbType.NVarChar, data.MixTypeCode));
            param_list.Add(create_sql_param($"@{specialdesignation_COL}", SqlDbType.NVarChar, data.SpecialDesignation));
            param_list.Add(create_sql_param($"@{conditions_COL}", SqlDbType.NVarChar, data.Conditions));
            param_list.Add(create_sql_param($"@{result_COL}", SqlDbType.NVarChar, data.ResultOrInput));
            param_list.Add(create_sql_param($"@{method_COL}", SqlDbType.NVarChar, data.EvaluationMethod));
            param_list.Add(create_sql_param($"@{baseamount_COL}", SqlDbType.Decimal, data.BaseAmount));
            param_list.Add(create_sql_param($"@{buzai_COL}", SqlDbType.NVarChar, data.Buzai));
            param_list.Add(create_sql_param($"@{afterfiltbuzai_COL}", SqlDbType.NVarChar, data.AfterFiltBuzai));
            param_list.Add(create_sql_param($"@{excelrow_COL}", SqlDbType.Int, data.ExcelSenkoLogRow));

            execute(query, param_list);

        }
    }
}
