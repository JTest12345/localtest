using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Data;
using System.Data.SqlClient;    //データサーバへアクセスするのに使う
using System.ComponentModel.DataAnnotations;//プロパティの属性値付けるのに使う

using Newtonsoft.Json;

namespace KodaClassLibrary {

    /// <summary>
    /// KODAデータベースに接続してパーティクルデータを扱うクラス
    /// </summary>
    public class ParticleSQL : KodaSQL {

        /// <summary>
        /// パーティクルテーブルのデータクラス
        /// </summary>
        public class ParticleData {

            /// <summary>
            /// 製造拠点
            /// </summary>
            [Display(Name = "製造拠点")]
            [JsonProperty("manubase")]
            public virtual string ManuBase { get; set; }

            /// <summary>
            /// 場所
            /// </summary>
            [Display(Name = "場所")]
            [JsonProperty("place")]
            public virtual string Place { get; set; }

            /// <summary>
            /// センサー名（設置場所名）
            /// </summary>
            [Display(Name = "センサー設置場所")]
            [JsonProperty("sensor-name")]
            public virtual string SensorName { get; set; }

            /// <summary>
            /// センサー型式
            /// </summary>
            [Display(Name = "センサー型式")]
            [JsonProperty("sensor-model")]
            public virtual string SensorModel { get; set; }

            /// <summary>
            /// センサーシリアル番号
            /// </summary>
            [Display(Name = "センサーシリアル番号")]
            [JsonProperty("sensor-serial")]
            public virtual string SensorSerial { get; set; }

            /// <summary>
            /// 日時
            /// </summary>
            [Display(Name = "日時")]
            [JsonProperty("time")]
            public virtual DateTimeOffset Time { get; set; }

            /// <summary>
            /// 0.3μm粒子カウント数
            /// </summary>
            [Display(Name = "0.3μm粒子")]
            [JsonProperty("particle-03")]
            public virtual int Particle03 { get; set; }

            /// <summary>
            /// 0.3μm粒子カウント数
            /// </summary>
            [Display(Name = "0.5μm粒子")]
            [JsonProperty("particle-05")]
            public virtual int Particle05 { get; set; }

            /// <summary>
            /// 0.3μm粒子カウント数
            /// </summary>
            [Display(Name = "1.0μm粒子")]
            [JsonProperty("particle-1")]
            public virtual int Particle1 { get; set; }

            /// <summary>
            /// 温度
            /// </summary>
            [Display(Name = "温度")]
            [JsonProperty("temperature")]
            public virtual double? Temperature { get; set; }

            /// <summary>
            /// 湿度
            /// </summary>
            [Display(Name = "湿度")]
            [JsonProperty("humidity")]
            public virtual double? Humidity { get; set; }

            /// <summary>
            /// 露点温度
            /// </summary>
            [Display(Name = "露点温度")]
            [JsonProperty("dewpoint")]
            public virtual double? DewPoint { get; set; }

            /// <summary>
            /// 規格外発生したかどうか
            /// <para>NG発生した時はtrue</para>
            /// </summary>
            [Display(Name = "規格外発生")]
            [JsonProperty("isng")]
            public virtual bool IsNG { get; set; }

            /// <summary>
            /// NGメッセージ
            /// </summary>
            [Display(Name = "NGメッセージ")]
            [JsonProperty("ng-msg")]
            public virtual string NgMsg { get; set; }
        }

        const string Particle_TABLE = "[dbo].[Particle]";

        const string manubase_COL = "manubase";
        const string place_COL = "place";
        const string sensorname_COL = "sensorname";
        const string sensormodel_COL = "sensormodel";
        const string sensorserial_COL = "sensorserial";
        const string time_COL = "time";
        const string par03_COL = "par03";
        const string par05_COL = "par05";
        const string par1_COL = "par1";
        const string temp_COL = "temp";
        const string hum_COL = "hum";
        const string dew_COL = "dew";
        const string isng_COL = "isng";
        const string ngmsg_COL = "ngmsg";


        /// <summary>
        /// コンストラクタ(データベースへの接続情報を渡す)
        /// </summary>
        public ParticleSQL(string data_source, string db_name, string user_id, string pass, int timeout)
            : base(data_source, db_name, user_id, pass, timeout) {

        }

        /// <summary>
        /// コンストラクタ(データベースへの接続文字列を渡す）
        /// </summary>
        public ParticleSQL(string con_str)
            : base(con_str) {

        }

        /// <summary>
        /// パーティクルテーブルからデータを読み取る共通メソッド
        /// </summary>
        /// <param name="add_where"></param>
        /// <returns></returns>
        private List<T> Get_ParticleData<T>(string add_where = null, string new_query = null)
            where T : ParticleData, new() {

            string query;
            if (new_query == null) {
                query = $@"SELECT {manubase_COL},{place_COL},{sensorname_COL},{sensormodel_COL},{sensorserial_COL},
                                  {time_COL},{par03_COL},{par05_COL},{par1_COL},{temp_COL},{hum_COL},{dew_COL},{isng_COL},{ngmsg_COL}
                              FROM {Particle_TABLE} as pt
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
                    ManuBase = col.Contains(manubase_COL) ? row[manubase_COL].ToString() : "",
                    Place = col.Contains(place_COL) ? row[place_COL].ToString() : "",
                    SensorName = col.Contains(sensorname_COL) ? row[sensorname_COL].ToString() : "",
                    SensorModel = col.Contains(sensormodel_COL) ? row[sensormodel_COL].ToString() : "",
                    SensorSerial = col.Contains(sensorserial_COL) ? row[sensorserial_COL].ToString() : "",
                    Time = col.Contains(time_COL) ? (DateTimeOffset)row[time_COL] : new DateTimeOffset(new DateTime(1970, 1, 1, 0, 0, 0)),
                    Particle03 = col.Contains(par03_COL) ? (int)row[par03_COL] : -1,
                    Particle05 = col.Contains(par05_COL) ? (int)row[par05_COL] : -1,
                    Particle1 = col.Contains(par1_COL) ? (int)row[par1_COL] : -1,
                    Temperature = col.Contains(temp_COL) ? row[temp_COL] as double? : null,
                    Humidity = col.Contains(hum_COL) ? row[hum_COL] as double? : null,
                    DewPoint = col.Contains(dew_COL) ? row[dew_COL] as double? : null,
                    IsNG = col.Contains(isng_COL) ? (bool)row[isng_COL] : false,
                    NgMsg = col.Contains(ngmsg_COL) ? row[ngmsg_COL].ToString() : "",
                };

                list.Add(data);
            }

            return list;
        }


        /// <summary>
        /// 重複しないセンサーのリストを取得する
        /// </summary>
        public List<T> Get_SensorList<T>()
            where T : ParticleData, new() {

            string new_query = $@"SELECT DISTINCT {manubase_COL},{place_COL},{sensorname_COL}
                                  FROM {Particle_TABLE}";

            //データベースからデータ取得
            var list = Get_ParticleData<T>(new_query: new_query);

            return list;
        }

        /// <summary>
        /// 指定したセンサーの指定した期間(日付)のデータを取得する
        /// </summary>
        public List<T> Get_DataList<T>(T obj, DateTime start, DateTime end)
            where T : ParticleData, new() {

            string add_where = $@"AND {manubase_COL}='{obj.ManuBase}'
                                  AND {place_COL}='{obj.Place}'
                                  AND {sensorname_COL}='{obj.SensorName}'
　　　　　　　　　　　　　　　　　AND {time_COL}>'{start.ToString("yyyy/MM/dd 00:00:00 +9:00")}' AND {time_COL}<'{end.ToString("yyyy/MM/dd 23:59:59 +9:00")}'
                                  ORDER BY {time_COL}";

            //データベースからデータ取得
            var list = Get_ParticleData<T>(add_where);

            return list;
        }

        /// <summary>
        /// データベースのParticleテーブルに書き込む
        /// </summary>
        public void Insert_CurrentData(ParticleData pd) {

            string query = $@"INSERT INTO {Particle_TABLE}
                                     ({manubase_COL},{place_COL},{sensorname_COL},{sensormodel_COL},{sensorserial_COL},
                                      {time_COL},{par03_COL},{par05_COL},{par1_COL},{temp_COL},{hum_COL},{dew_COL},{isng_COL},{ngmsg_COL})
                              VALUES (@{manubase_COL},@{place_COL},@{sensorname_COL},@{sensormodel_COL},@{sensorserial_COL},
                                      @{time_COL},@{par03_COL},@{par05_COL},@{par1_COL},@{temp_COL},@{hum_COL},@{dew_COL},@{isng_COL},@{ngmsg_COL})";

            var param_list = new List<SqlParameter>();

            param_list.Add(create_sql_param($"@{manubase_COL}", SqlDbType.NVarChar, pd.ManuBase));
            param_list.Add(create_sql_param($"@{place_COL}", SqlDbType.NVarChar, pd.Place));
            param_list.Add(create_sql_param($"@{sensorname_COL}", SqlDbType.NVarChar, pd.SensorName));
            param_list.Add(create_sql_param($"@{sensormodel_COL}", SqlDbType.NVarChar, pd.SensorModel));
            param_list.Add(create_sql_param($"@{sensorserial_COL}", SqlDbType.NVarChar, pd.SensorSerial));

            param_list.Add(create_sql_param($"@{time_COL}", SqlDbType.DateTimeOffset, pd.Time));
            param_list.Add(create_sql_param($"@{par03_COL}", SqlDbType.Int, pd.Particle03));
            param_list.Add(create_sql_param($"@{par05_COL}", SqlDbType.Int, pd.Particle05));
            param_list.Add(create_sql_param($"@{par1_COL}", SqlDbType.Int, pd.Particle1));

            param_list.Add(create_sql_param($"@{temp_COL}", SqlDbType.Float, pd.Temperature));
            param_list.Add(create_sql_param($"@{hum_COL}", SqlDbType.Float, pd.Humidity));
            param_list.Add(create_sql_param($"@{dew_COL}", SqlDbType.Float, pd.DewPoint));

            param_list.Add(create_sql_param($"@{isng_COL}", SqlDbType.Bit, pd.IsNG));
            param_list.Add(create_sql_param($"@{ngmsg_COL}", SqlDbType.NVarChar, pd.NgMsg));

            execute(query, param_list);

        }


    }
}

