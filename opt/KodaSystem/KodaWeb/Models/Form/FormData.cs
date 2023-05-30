using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;    //データサーバへアクセスするのに使う

using Newtonsoft.Json;//Newtonsoft.Jsonをインストールして使う
using Newtonsoft.Json.Linq;

using System.ComponentModel.DataAnnotations;

using KodaWeb.Controllers;
using KodaClassLibrary;


namespace KodaWeb.Models {

    /// <summary>
    /// 製品帳票データクラス
    /// </summary>
    public class ProductFormData : FormsSQL.TnFormTranData {

        /// <summary>
        /// 帳票データのJson文字列
        /// </summary>
        public List<string> FormDataJsonList { get; set; }

    }


    /// <summary>
    /// 定期点検帳票データクラス
    /// </summary>
    public class PIFormData : FormsSQL.TnPIFormData {

        /// <summary>
        /// 帳票データのJson文字列
        /// </summary>
        public List<string> FormDataJsonList { get; set; }

    }

    /// <summary>
    /// 帳票データ処理クラス
    /// </summary>
    public static class FormData {



        /// <summary>
        /// 製品帳票データを取得する
        /// </summary>
        public static DataTable Get_PFD(FormsSQL sql, FormInfo form_info, string start, string end) {

            //データベース接続オブジェクト作成
            //var sql = new FormsSQL(FormController.connect_string);

            //データベースから帳票情報取得
            var fi = sql.Get_FormMaster<FormInfo>(form_info.FormNo, form_info.FormRev);
            fi = Split_CojJson(fi);

            //テーブル作成＋列追加(基本データ+修正リンク用列)
            DataTable table = new DataTable("FormData");
            List<string> cols = new List<string> { "機種", "LotNo", "設備番号", "工程番号", "作業単位Lot", "実施日", "作業者" };
            foreach (string c in cols) {
                table.Columns.Add(c);
            }

            //列追加(帳票データ)
            for (int i = 0; i < fi.DataNameDicList.Count; i++) {
                fi.DataNameDicList[i].Values.ToList().ForEach(name =>
                {
                    //
                    table.Columns.Add($"項目{(i + 1).ToString()}\n{name}");
                });
            }

            //データベースから製品帳票データ取得
            var pfd_list = sql.Get_ProductFormData<ProductFormData>(fi.FormNo, fi.FormRev, start, end);

            //DataTableに1行追加を繰り返す
            foreach (ProductFormData obj in pfd_list) {

                var pfd = Set_FormDataJsonList(obj);

                //新しく1行データ作成
                DataRow dr = table.NewRow();

                dr["機種"] = pfd.Typecd;
                dr["LotNo"] = pfd.LotNo18;
                dr["設備番号"] = pfd.Plantcd;
                dr["工程番号"] = pfd.ProcNo;
                dr["作業単位Lot"] = pfd.WorkUnitID;
                dr["実施日"] = ((DateTime)pfd.UpdateAt).ToString("yyyy/MM/dd HH:mm:ss");
                dr["作業者"] = pfd.UpdateBy;

                //項目ごとのループ
                for (int i = 0; i < pfd.FormDataJsonList.Count; i++) {

                    //デシリアライズ
                    var root = JObject.Parse(pfd.FormDataJsonList[i]);

                    //データ毎のループ
                    foreach (var pair in fi.DataNameDicList[i]) {
                        string[] key_array = pair.Key.Split('>');

                        string colname = $"項目{(i + 1).ToString()}\n{pair.Value}";
                        string value = "";
                        try {
                            if (key_array.Length == 1) {
                                value = root[key_array[0]].ToString();
                                //valueがリストの時があるので、ここで処理する
                                value = value.Replace("\r\n", "");
                                value = value.Replace("[", "").Replace("]", "");
                                value = value.Replace("\"", "").Replace(" ", "");
                                value = value.Replace(",", "、");

                                //valueがチェックボックスの結果だとTrue or Falseなので
                                //変換する
                                if (value == "True") {
                                    value = "✔";
                                }
                                else if (value == "False") {
                                    value = "-";
                                }

                                //何も値が無いなら"-"にする
                                if (value == "") {
                                    value = "-";
                                }
                            }
                            else if (key_array.Length == 2) {
                                var dt = (DateTime)root[key_array[0]][key_array[1]];

                                //今のところLengthが2になるのは日時だけなので
                                //valueをutcからjapan or chinaに時刻変換する
                                if (pfd.ManuBase == "CET") {
                                    value = Convert_DateTime_from_UTC(dt, "japan");
                                }
                                else if (pfd.ManuBase == "CEJM") {
                                    value = Convert_DateTime_from_UTC(dt, "china");
                                }
                                else {
                                    value = dt.ToString();
                                }

                            }

                            dr[colname] = value;
                        }
                        catch (Exception ex) {
                            //rootの中に指定のkeyの項目が無い場合にエラーになるので"-"を入れておく
                            //requireでないデータはformDataに入っていないかもしれない
                            dr[colname] = "-";
                        }
                    }

                }

                //テーブルに1行分データ追加
                table.Rows.Add(dr);
            }

            return table;

        }

        /// <summary>
        /// 定期点検帳票データを取得する
        /// </summary>
        public static DataTable Get_PIFD(FormsSQL sql, PeriodicInspection pi) {

            //データベース接続オブジェクト作成
            //var sql = new FormsSQL(FormController.connect_string);

            //データベースから帳票情報取得
            var fi = sql.Get_FormMaster<FormInfo>(pi.FormNo, (int)pi.FormRev);
            fi = Split_CojJson(fi);

            //テーブル作成＋列追加(基本データ)
            DataTable table = new DataTable("FormData");
            List<string> cols = new List<string> { "ID", "設備型式", "設備番号", "シリアル番号", "場所", "実施日", "作業者" };
            foreach (string c in cols) {
                table.Columns.Add(c);
            }

            //列追加(帳票データ)
            for (int i = 0; i < fi.DataNameDicList.Count; i++) {
                fi.DataNameDicList[i].Values.ToList().ForEach(name =>
                {
                    table.Columns.Add($"項目{(i + 1).ToString()}\n{name}");
                });
            }

            //データベースから製品帳票データ取得
            var pifd_list = sql.Get_PIFormData<PIFormData>(pi.FormNo, (int)pi.FormRev, pi.ManuBase, pi.Period);

            //DataTableに1行追加を繰り返す
            foreach (PIFormData obj in pifd_list) {

                var pifd = Set_FormDataJsonList(obj);

                //新しく1行データ作成
                DataRow dr = table.NewRow();
                dr["ID"] = pifd.ID;
                dr["設備型式"] = pifd.MacName;
                dr["設備番号"] = pifd.Plantcd;
                dr["シリアル番号"] = pifd.SerialNo;
                dr["場所"] = pifd.Place;
                dr["実施日"] = ((DateTime)pifd.UpdateAt).ToString("yyyy/MM/dd HH:mm:ss");
                dr["作業者"] = pifd.UpdateBy;

                //項目ごとのループ
                for (int i = 0; i < pifd.FormDataJsonList.Count; i++) {

                    //デシリアライズ
                    var root = JObject.Parse(pifd.FormDataJsonList[i]);

                    //データ毎のループ
                    foreach (var pair in fi.DataNameDicList[i]) {
                        string[] key_array = pair.Key.Split('>');

                        string colname = $"項目{(i + 1).ToString()}\n{pair.Value}";
                        string value = "";
                        try {
                            if (key_array.Length == 1) {
                                value = root[key_array[0]].ToString();
                                //valueがリストの時があるので、ここで処理する
                                value = value.Replace("\r\n", "");
                                value = value.Replace("[", "").Replace("]", "");
                                value = value.Replace("\"", "").Replace(" ", "");
                                value = value.Replace(",", "、");

                                //valueがチェックボックスの結果だとTrue or Falseなので
                                //変換する
                                if (value == "True") {
                                    value = "✔";
                                }
                                else if (value == "False") {
                                    value = "-";
                                }

                                //何も値が無いなら"-"にする
                                if (value == "") {
                                    value = "-";
                                }
                            }
                            else if (key_array.Length == 2) {
                                var dt = (DateTime)root[key_array[0]][key_array[1]];

                                //今のところLengthが2になるのは日時だけなので
                                //valueをutcからjapan or chinaに時刻変換する
                                if (pi.ManuBase == "CET") {
                                    value = Convert_DateTime_from_UTC(dt, "japan");
                                }
                                else if (pi.ManuBase == "CEJM") {
                                    value = Convert_DateTime_from_UTC(dt, "china");
                                }
                                else {
                                    value = dt.ToString();
                                }

                            }

                            dr[colname] = value;
                        }
                        catch (Exception ex) {
                            //rootの中に指定のkeyの項目が無い場合にエラーになるので"-"を入れておく
                            //requireでないデータはformDataに入っていないかもしれない
                            dr[colname] = "-";
                        }
                    }

                }

                //テーブルに1行分データ追加
                table.Rows.Add(dr);
            }

            return table;

        }

        /// <summary>
        /// COJのJSONからデータ名一覧を取得してセットする
        /// </summary>
        /// <param name="fi"></param>
        /// <returns></returns>
        public static FormInfo Split_CojJson(FormInfo fi) {

            string json = fi.COJ;
            var root = JObject.Parse(json);

            var js_list = new List<string>();
            var ui_list = new List<string>();
            var dnd_list = new List<string>();

            root["cejObject"]["coList"].ToList().ForEach(obj =>
            {
                //JSONスキーマ
                js_list.Add(obj["schema"].ToString());
                //UIスキーマ
                ui_list.Add(obj["uiSchema"].ToString());
                //データ名対応一覧
                dnd_list.Add(obj["dataNames"].ToString());
            });

            fi.JsonSchema = js_list;
            fi.UiSchema = ui_list;
            fi.DataNameDicList = FormData.Get_DataNameDicList(dnd_list);

            return fi;
        }

        /// <summary>
        /// 帳票データのJSONリストをセットする(製品/工程帳票)
        /// </summary>
        public static ProductFormData Set_FormDataJsonList(ProductFormData pfd) {

            //デシリアライズ
            var root = JObject.Parse(pfd.COJ);

            var form_data_json_list = new List<string>();

            //cejObject、coListはJSON仕様により固定
            root["cejObject"]["coList"].ToList().ForEach(obj =>
            {
                form_data_json_list.Add(obj["formData"].ToString());
            });

            pfd.FormDataJsonList = form_data_json_list;

            return pfd;
        }

        /// <summary>
        /// 帳票データのJSONリストをセットする(定期点検帳票)
        /// </summary>
        public static PIFormData Set_FormDataJsonList(PIFormData pifd) {

            //デシリアライズ
            var root = JObject.Parse(pifd.COJ);

            var form_data_json_list = new List<string>();

            //cejObject、coListはJSON仕様により固定
            root["cejObject"]["coList"].ToList().ForEach(obj =>
            {
                form_data_json_list.Add(obj["formData"].ToString());
            });

            pifd.FormDataJsonList = form_data_json_list;

            return pifd;
        }




        /// <summary>
        /// データ名対応一覧からデータ名辞書のリストを取得する
        /// </summary>
        /// <param name="datanames_list">JSONデータの"dataNames"の部分</param>
        public static List<Dictionary<string, string>> Get_DataNameDicList(List<string> datanames_list) {

            var dnd_list = new List<Dictionary<string, string>>();

            try {

                for (int i = 0; i < datanames_list.Count; i++) {
                    var dataname_dic = new Dictionary<string, string>();

                    // dataNamesのJSON文字列をデシリアライズ
                    var json = datanames_list[i];
                    var obj = JObject.Parse(json);

                    foreach (var o in obj) {
                        dataname_dic.Add(o.Key, o.Value.ToString());
                    }

                    dnd_list.Add(dataname_dic);
                }

                return dnd_list;
            }
            catch (Exception ex) {
                throw new Exception("データ名辞書のリストの作成に失敗しました。");
            }

        }


        /// <summary>
        /// UTC時刻から日本または中国時間に変換する
        /// <para>引数2は local="japan" or "china"</para>
        /// <para>どちらでもない場合は例外を出す</para>
        /// </summary>
        /// <returns></returns>
        public static string Convert_DateTime_from_UTC(DateTime utc, string local) {

            if (local == "japan") {
                TimeZoneInfo jstZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");
                DateTime jst = TimeZoneInfo.ConvertTimeFromUtc(utc, jstZoneInfo);
                return jst.ToString();
            }
            else if (local == "china") {
                TimeZoneInfo cstZoneInfo = TimeZoneInfo.FindSystemTimeZoneById("China Standard Time");
                DateTime cst = TimeZoneInfo.ConvertTimeFromUtc(utc, cstZoneInfo);
                return cst.ToString();
            }
            else {
                throw new Exception("local引数が無効な値です。");
            }
        }


    }


}
