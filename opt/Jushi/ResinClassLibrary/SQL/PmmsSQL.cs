//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//using System.Data;
//using System.Data.SqlClient;    //データサーバへアクセスするのに使う

//using Newtonsoft.Json;

//namespace ResinClassLibrary {

//    /// <summary>
//    /// KODAデータベースに接続してパーティクルデータを扱うクラス
//    /// </summary>
//    public class PmmsSQL : SQL {

//        /// <summary>
//        /// PMMSテーブルのデータクラス
//        /// </summary>
//        public class TnPMMSTran_CEJ_Data {

//            /// <summary>
//            /// カップ番号
//            /// </summary>
//            [JsonProperty("cupno")]
//            public virtual string CupNo { get; set; }

//            /// <summary>
//            /// PMMSで使用する工程番号
//            /// <para>自動配合機を使う場合は0 __ 使わない場合は1</para>
//            /// </summary>
//            [JsonProperty("procno")]
//            public virtual int ProcessNo { get; set; }

//            /// <summary>
//            /// 機種名
//            /// </summary>
//            [JsonProperty("productnm")]
//            public virtual string ProductName { get; set; }

//            /// <summary>
//            /// 波長ランク
//            /// </summary>
//            [JsonProperty("ledrank")]
//            public virtual string WavelengthRank { get; set; }

//            /// <summary>
//            /// 配合手段
//            /// </summary>
//            [JsonProperty("kubun")]
//            public virtual string FlowMode { get; set; }

//            /// <summary>
//            /// 結果
//            /// </summary>
//            [JsonProperty("result")]
//            public virtual string Result { get; set; }

//            /// <summary>
//            /// 樹脂カップ作業工程コード
//            /// </summary>
//            [JsonProperty("mixtypecd")]
//            public virtual string MixTypeCode { get; set; }

//            /// <summary>
//            /// 成形機
//            /// </summary>
//            [JsonProperty("seikeiki")]
//            public virtual string Machine { get; set; }

//            /// <summary>
//            /// 設備番号
//            /// </summary>
//            [JsonProperty("macno")]
//            public virtual string Plantcd { get; set; }

//            /// <summary>
//            /// 変化点
//            /// </summary>
//            [JsonProperty("henkaten")]
//            public virtual string Henkaten { get; set; }

//            /// <summary>
//            /// レシピファイル名
//            /// </summary>
//            [JsonProperty("recipefilenm")]
//            public virtual string RecipeFileName { get; set; }

//            /// <summary>
//            /// このカップで使う基板枚数
//            /// </summary>
//            [JsonProperty("bdqty")]
//            public virtual int? BdQty { get; set; }

//            /// <summary>
//            /// この樹脂カップ番号を作製した回数
//            /// <para>初めて作製 = 1</para>
//            /// </summary>
//            [JsonProperty("trynm")]
//            public virtual int? TryNum { get; set; }

//            /// <summary>
//            /// ？
//            /// </summary>
//            [JsonProperty("dtend")]
//            public virtual DateTime? End { get; set; }

//            /// <summary>
//            /// 社員コード
//            /// </summary>
//            [JsonProperty("employee")]
//            public virtual string Employee { get; set; }

//            /// <summary>
//            /// ？
//            /// </summary>
//            [JsonProperty("initialdtend")]
//            public virtual DateTime? InitialEnd { get; set; }

//            /// <summary>
//            /// 樹脂グループコード(ARMSに合わせて存在するだけで、特に使っていない)
//            /// </summary>
//            [JsonProperty("resingroupcd")]
//            public virtual string ResinGroupCode { get; set; }

//            /// <summary>
//            /// ロット番号(10桁or18桁)をカンマ区切りで並べた文字列
//            /// <para>パターン①22929VD001,22929VD002,22929VD003</para>
//            /// <para>パターン②520220929130000001,520220929130000002,520220929130000003</para>
//            /// <para>パターン③520220929130000001,22929VD003,520220929130000003</para>
//            /// </summary>
//            [JsonProperty("lotno")]
//            public virtual string LotNo { get; set; }
//        }

//        /// <summary>
//        /// TmWorkRegulationテーブルのデータクラス
//        /// </summary>
//        public class TmWorkRegulationData {

//            /// <summary>
//            /// カップ番号
//            /// </summary>
//            [JsonProperty("cupno")]
//            public virtual string CupNo { get; set; }

//            /// <summary>
//            /// 基準工程
//            /// </summary>
//            [JsonProperty("procfrom")]
//            public virtual int ProcessFrom { get; set; }

//            /// <summary>
//            /// 次工程
//            /// </summary>
//            [JsonProperty("procto")]
//            public virtual int ProcessTo { get; set; }

//            /// <summary>
//            /// 基準工程実施してから次工程開始が出来るようになるまでの時間(分)
//            /// <para>この時間が経過しないとPMMSで次工程の作業登録ができない</para>
//            /// <para>テーブルの"fromwaittime"に対応</para>
//            /// </summary>
//            [JsonProperty("fromwaittime")]
//            public virtual int ForbiddenMinute { get; set; }

//            /// <summary>
//            /// 基準工程実施してから次工程完了までの時間(分)
//            /// <para>この時間以内にPMMSで作業登録しないと、この部材は使えなくなる</para>
//            /// <para>テーブルの"fromtoendtime"に対応</para>
//            /// </summary>
//            [JsonProperty("fromtoendtime")]
//            public virtual int LimitMinute { get; set; }
//        }

//        const string TnPMMSTran_CEJ_TABLE = "[dbo].[TnPMMSTran_CEJ]";
//        const string TmWorkRegulation_TABLE = "[dbo].[TmWorkRegulation]";

//        const string cupno_COL = "cupno";
//        const string procno_COL = "procno";
//        const string productnm_COL = "productnm";
//        const string ledrank_COL = "ledrank";
//        const string kubun_COL = "kubun";
//        const string result_COL = "result";
//        const string mixtypecd_COL = "mixtypecd";
//        const string seikeiki_COL = "seikeiki";
//        const string macno_COL = "macno";
//        const string henkaten_COL = "henkaten";
//        const string recipefilenm_COL = "recipefilenm";
//        const string bdqty_COL = "bdqty";
//        const string trynm_COL = "trynm";
//        const string dtend_COL = "dtend";
//        const string Employee_COL = "Employee";
//        const string initialdtend_COL = "initialdtend";
//        const string resingroupcd_COL = "resingroupcd";
//        const string lotno_COL = "lotno";

//        const string procfrom_COL = "procfrom";
//        const string procto_COL = "procto";
//        const string fromwaittime_COL = "fromwaittime";
//        const string fromtoendtime_COL = "fromtoendtime";


//        /// <summary>
//        /// コンストラクタ(データベースへの接続情報を渡す)
//        /// </summary>
//        public PmmsSQL(string data_source, string db_name, string user_id, string pass, int timeout)
//            : base(data_source, db_name, user_id, pass, timeout) {

//        }

//        /// <summary>
//        /// コンストラクタ(データベースへの接続文字列を渡す）
//        /// </summary>
//        public PmmsSQL(string con_str)
//            : base(con_str) {

//        }

//        /// <summary>
//        /// TnPMMSTran_CEJテーブルからデータを読み取る共通メソッド
//        /// </summary>
//        /// <param name="add_where"></param>
//        /// <returns></returns>
//        private List<T> Get_PmmsData_from_TnPMMSTran_CEJ<T>(string add_where = null, string new_query = null)
//            where T : TnPMMSTran_CEJ_Data, new() {

//            string query;
//            if (new_query == null) {
//                query = $@"SELECT {cupno_COL},{procno_COL},{productnm_COL},{ledrank_COL},{kubun_COL},{result_COL},
//                                  {mixtypecd_COL},{seikeiki_COL},{macno_COL},{henkaten_COL},{recipefilenm_COL},{bdqty_COL},
//                                  {trynm_COL},{dtend_COL},{Employee_COL},{initialdtend_COL},{resingroupcd_COL},{lotno_COL}
//                             FROM {TnPMMSTran_CEJ_TABLE} as tnpt_cej
//                             WHERE 1=1 ";
//            }
//            else {
//                query = new_query;
//            }

//            //条件があれば付け加える
//            if (add_where != null) {
//                query += add_where;
//            }

//            //データベースからデータ取得
//            var table = read_data(query);

//            var list = new List<T>();

//            //データが1個もない時は空のリスト(Count=0)を返す
//            if (table.Rows.Count == 0) { return list; }

//            var col = table.Columns;
//            for (int i = 0; i < table.Rows.Count; i++) {
//                var row = table.Rows[i];

//                var data = new T {
//                    CupNo = col.Contains(cupno_COL) ? row[cupno_COL].ToString() : "",
//                    ProcessNo = col.Contains(procno_COL) ? (int)row[procno_COL] : -1,
//                    ProductName = col.Contains(productnm_COL) ? row[productnm_COL].ToString() : "",
//                    WavelengthRank = col.Contains(ledrank_COL) ? row[ledrank_COL].ToString() : "",
//                    FlowMode = col.Contains(kubun_COL) ? row[kubun_COL].ToString() : "",
//                    Result = col.Contains(result_COL) ? row[result_COL].ToString() : "",
//                    MixTypeCode = col.Contains(mixtypecd_COL) ? row[mixtypecd_COL].ToString() : "",
//                    Machine = col.Contains(seikeiki_COL) ? row[seikeiki_COL].ToString() : "",
//                    Plantcd = col.Contains(macno_COL) ? row[macno_COL].ToString() : "",
//                    Henkaten = col.Contains(henkaten_COL) ? row[henkaten_COL].ToString() : "",
//                    RecipeFileName = col.Contains(recipefilenm_COL) ? row[recipefilenm_COL].ToString() : "",
//                    BdQty = col.Contains(bdqty_COL) ? row[bdqty_COL] as int? : null,
//                    TryNum = col.Contains(trynm_COL) ? row[trynm_COL] as int? : null,
//                    End = col.Contains(dtend_COL) ? row[dtend_COL] as DateTime? : null,
//                    Employee = col.Contains(Employee_COL) ? row[Employee_COL].ToString() : "",
//                    InitialEnd = col.Contains(initialdtend_COL) ? row[initialdtend_COL] as DateTime? : null,
//                    ResinGroupCode= col.Contains(resingroupcd_COL) ? row[resingroupcd_COL].ToString() : "",
//                    LotNo = col.Contains(lotno_COL) ? row[lotno_COL].ToString() : "",
//                };

//                list.Add(data);
//            }

//            return list;
//        }

//        private List<T> Get_TmWorkRegulationData<T>(string add_where = null, string new_query = null)
//            where T : TmWorkRegulationData, new() {

//            string query;
//            if (new_query == null) {
//                query = $@"SELECT {cupno_COL},{procfrom_COL},{procto_COL},{fromwaittime_COL},{fromtoendtime_COL}
//                             FROM {TmWorkRegulation_TABLE} as tm_wr
//                            WHERE 1=1 ";
//            }
//            else {
//                query = new_query;
//            }

//            //条件があれば付け加える
//            if (add_where != null) {
//                query += add_where;
//            }

//            //データベースからデータ取得
//            var table = read_data(query);

//            var list = new List<T>();

//            //データが1個もない時は空のリスト(Count=0)を返す
//            if (table.Rows.Count == 0) { return list; }

//            var col = table.Columns;
//            for (int i = 0; i < table.Rows.Count; i++) {
//                var row = table.Rows[i];

//                var data = new T {
//                    CupNo = col.Contains(cupno_COL) ? row[cupno_COL].ToString() : "",
//                    ProcessFrom = col.Contains(procfrom_COL) ? (int)row[procfrom_COL] : -1,
//                    ProcessTo = col.Contains(procto_COL) ? (int)row[procto_COL] : -1,
//                    ForbiddenMinute = col.Contains(fromwaittime_COL) ? (int)row[fromwaittime_COL] : -1,
//                    LimitMinute = col.Contains(fromtoendtime_COL) ? (int)row[fromtoendtime_COL] : -1,
//                };

//                list.Add(data);
//            }

//            return list;
//        }

//        /// <summary>
//        /// 同じカップを何回作製したか取得する(条件：cupno AND process no)
//        /// <para>作製していない ⇒ 0</para>
//        /// <para>作製したことがある ⇒ 1,2,3,...</para>
//        /// </summary>
//        public int Get_CupTryNum(Recipe recipe) {

//            string add_where = $@"AND {cupno_COL}='{recipe.CupNo}' AND {procno_COL}={recipe.ProcessNo}";

//            var list = Get_PmmsData_from_TnPMMSTran_CEJ<TnPMMSTran_CEJ_Data>(add_where);

//            if (list.Count == 0) {
//                return 0;
//            }
//            else {

//                //1個しかないので
//                var num = list[0].TryNum;

//                if (num == null) {
//                    return 0;
//                }
//                else {
//                    return (int)num;
//                }
//            }
//        }

//        /// <summary>
//        /// 指定カップ番号の作業規制時間を取得する
//        /// </summary>
//        /// <param name="cupno"></param>
//        /// <returns></returns>
//        public List<CupWorkRegulation> Get_CupWorkRegulation(string cupno) {

//            string add_where = $@"AND {cupno_COL}='{cupno}'";

//            var list = Get_TmWorkRegulationData<CupWorkRegulation>(add_where);

//            return list;
//        }

//        /// <summary>
//        /// TnPMMSTran_CEJテーブルに樹脂カップ履歴を書き込む
//        /// </summary>
//        public void Insert_CupTranData_to_TnPMMSTran_CEJ(TnPMMSTran_CEJ_Data data) {

//            string query = $@"INSERT INTO {TnPMMSTran_CEJ_TABLE}
//                                     ({cupno_COL},{procno_COL},{productnm_COL},{ledrank_COL},{kubun_COL},{result_COL},
//                                      {mixtypecd_COL},{seikeiki_COL},{macno_COL},{henkaten_COL},{recipefilenm_COL},{bdqty_COL},
//                                      {trynm_COL},{dtend_COL},{Employee_COL},{initialdtend_COL},{resingroupcd_COL},{lotno_COL})
//                              VALUES (@{cupno_COL},@{procno_COL},@{productnm_COL},@{ledrank_COL},@{kubun_COL},@{result_COL},
//                                      @{mixtypecd_COL},@{seikeiki_COL},@{macno_COL},@{henkaten_COL},@{recipefilenm_COL},@{bdqty_COL},
//                                      @{trynm_COL},@{dtend_COL},@{Employee_COL},@{initialdtend_COL},@{resingroupcd_COL},@{lotno_COL})";

//            var param_list = new List<SqlParameter>();

//            param_list.Add(create_sql_param($"@{cupno_COL}", SqlDbType.NVarChar, data.CupNo));
//            param_list.Add(create_sql_param($"@{procno_COL}", SqlDbType.Int, data.ProcessNo));
//            param_list.Add(create_sql_param($"@{productnm_COL}", SqlDbType.NVarChar, data.ProductName));
//            param_list.Add(create_sql_param($"@{ledrank_COL}", SqlDbType.NVarChar, data.WavelengthRank));
//            param_list.Add(create_sql_param($"@{kubun_COL}", SqlDbType.NVarChar, data.FlowMode));
//            param_list.Add(create_sql_param($"@{result_COL}", SqlDbType.NVarChar, data.Result));
//            param_list.Add(create_sql_param($"@{mixtypecd_COL}", SqlDbType.NVarChar, data.MixTypeCode));
//            param_list.Add(create_sql_param($"@{seikeiki_COL}", SqlDbType.NVarChar, data.Machine));
//            param_list.Add(create_sql_param($"@{macno_COL}", SqlDbType.NVarChar, data.Plantcd));
//            param_list.Add(create_sql_param($"@{henkaten_COL}", SqlDbType.NVarChar, data.Henkaten));
//            param_list.Add(create_sql_param($"@{recipefilenm_COL}", SqlDbType.NVarChar, data.RecipeFileName));
//            param_list.Add(create_sql_param($"@{bdqty_COL}", SqlDbType.Int, data.BdQty));
//            param_list.Add(create_sql_param($"@{trynm_COL}", SqlDbType.Int, data.TryNum));
//            param_list.Add(create_sql_param($"@{dtend_COL}", SqlDbType.DateTime, data.End));
//            param_list.Add(create_sql_param($"@{Employee_COL}", SqlDbType.NVarChar, data.Employee));
//            param_list.Add(create_sql_param($"@{initialdtend_COL}", SqlDbType.DateTime, data.InitialEnd));
//            param_list.Add(create_sql_param($"@{resingroupcd_COL}", SqlDbType.NVarChar, data.ResinGroupCode));
//            param_list.Add(create_sql_param($"@{lotno_COL}", SqlDbType.NVarChar, data.LotNo));

//            execute(query, param_list);
//        }

//        /// <summary>
//        /// TnPMMSTran_CEJテーブルのカップ情報を更新する
//        /// </summary>
//        public void Update_CupTranData_to_TnPMMSTran_CEJ(TnPMMSTran_CEJ_Data data) {

//            string query = $@"UPDATE {TnPMMSTran_CEJ_TABLE}
//                              SET {kubun_COL}=@{kubun_COL},
//                                  {result_COL}=@{result_COL},
//                                  {macno_COL}=@{macno_COL},
//                                  {trynm_COL}=@{trynm_COL},
//                                  {dtend_COL}=@{dtend_COL},
//                                  {Employee_COL}=@{Employee_COL}
//                              WHERE {cupno_COL}='{data.CupNo}' AND {procno_COL}={data.ProcessNo} AND {mixtypecd_COL}='{data.MixTypeCode}'";

//            var param_list = new List<SqlParameter>();

//            param_list.Add(create_sql_param($"@{kubun_COL}", SqlDbType.NVarChar, data.FlowMode));
//            param_list.Add(create_sql_param($"@{result_COL}", SqlDbType.NVarChar, data.Result));
//            param_list.Add(create_sql_param($"@{macno_COL}", SqlDbType.NVarChar, data.Plantcd));
//            param_list.Add(create_sql_param($"@{trynm_COL}", SqlDbType.Int, data.TryNum));
//            param_list.Add(create_sql_param($"@{dtend_COL}", SqlDbType.DateTime, data.End));
//            param_list.Add(create_sql_param($"@{Employee_COL}", SqlDbType.NVarChar, data.Employee));

//            execute(query, param_list);
//        }

//        /// <summary>
//        /// TmWorkRegulationテーブルに樹脂カップの作業時間規制を書き込む
//        /// </summary>
//        public void Insert_CupTimeInfo_to_TmWorkRegulation<T>(List<T> list)
//            where T : TmWorkRegulationData {

//            string query = $@"INSERT INTO {TmWorkRegulation_TABLE}
//                                     ({cupno_COL},{procfrom_COL},{procto_COL},{fromwaittime_COL},{fromtoendtime_COL})
//                              VALUES ";

//            var param_list = new List<SqlParameter>();


//            for (int i = 0; i < list.Count; i++) {

//                var data = list[i];

//                query += $@"(@{cupno_COL}{i},@{procfrom_COL}{i},@{procto_COL}{i},@{fromwaittime_COL}{i},@{fromtoendtime_COL}{i})";

//                param_list.Add(create_sql_param($"@{cupno_COL}{i}", SqlDbType.NVarChar, data.CupNo));
//                param_list.Add(create_sql_param($"@{procfrom_COL}{i}", SqlDbType.Int, data.ProcessFrom));
//                param_list.Add(create_sql_param($"@{procto_COL}{i}", SqlDbType.Int, data.ProcessTo));
//                param_list.Add(create_sql_param($"@{fromwaittime_COL}{i}", SqlDbType.Int, data.ForbiddenMinute));
//                param_list.Add(create_sql_param($"@{fromtoendtime_COL}{i}", SqlDbType.Int, data.LimitMinute));

//                if (i < list.Count - 1) {
//                    query += ",";
//                }
//            }

//            execute(query, param_list);
//        }
//    }
//}

