using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Threading.Tasks;
using System.IO;
using System.Data;
using static Oven_Control.Program;
using System.Web.UI.WebControls;


namespace Oven_Control {

    /// <summary>
    /// 静的クラス
    /// </summary>
    static class OperateFile {

        /// <summary>
        /// ファイル読み書き用文字コード：UTF8
        /// </summary>
        public static Encoding enc { get; private set; } = Encoding.UTF8;

        /// <summary>
        /// ファイル処理失敗した時用の専用エラーログファイル
        /// </summary>
        public static string file_errorlog { get; private set; } = AppFolder + "/Log/file_errorlog.txt";



        /// <summary>
        /// ファイル共有モードで末尾にテキスト追加
        /// <para>throw_exがtrueの時は例外をスルーする</para>
        /// </summary>
        public static void WriteAppend(string path, string text, bool throw_ex = false) {

            try {
                //フォルダが無かったら作る
                string fld = Path.GetDirectoryName(path);
                if (Directory.Exists(fld) == false) {
                    Directory.CreateDirectory(fld);
                }

                //FileMode.Appendはファイルが無かったら新規作成する
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, enc)) {
                    sw.WriteLine(text);
                }
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                WriteAppend(file_errorlog, dt + "\t" + "original_text:" + text);
                if (throw_ex) {
                    throw;
                }
            }
        }



        /// <summary>
        /// 設備番号：IPアドレスの対応データを読み込みます
        /// <para>登録設備が無い時 or 例外時はnullを返します</para>
        /// </summary>
        public static Dictionary<int, IPAddress> Read_MachineInfo() {

            string path = SettingsFolder + "/MachineIP.txt";
            var machine_info = new Dictionary<int, IPAddress>();

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {

                    //1行目を読み込む（使わない）
                    sr.ReadLine();

                    //2行目以降
                    string str;
                    int num;
                    IPAddress ip;
                    while (true) {
                        str = sr.ReadLine();
                        if (str == "" | str == null) {
                            break;
                        }
                        num = int.Parse(str.Split(',')[0]);
                        ip = IPAddress.Parse(str.Split(',')[1]);
                        machine_info.Add(num, ip);
                    }
                }

                if (machine_info.Count == 0) {
                    return null;
                }
                return machine_info;
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                return null;
            }
        }

        /// <summary>
        /// キュア条件を取得する
        /// </summary>
        /// <param name="productname">機種名</param>
        /// <param name="process_code">工程略語(DBCとか)</param>
        /// <returns></returns>
        public static string Get_Recipe_from_ProductRecipeCSV(string productname, string process_code) {

            string path = $@"{RecipeFolder}/{process_code}_Product-Recipe.csv";

            using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
            using (StreamReader sr = new StreamReader(fs, enc)) {

                //1行目を読み込む（使わない）
                sr.ReadLine();

                //2行目以降
                string line, product, recipe;

                while (true) {
                    line = sr.ReadLine();

                    if (string.IsNullOrEmpty(line)) {
                        return null;
                    }

                    product = line.Split(',')[0];
                    recipe = line.Split(',')[1];

                    if (product == productname) {
                        return recipe;
                    }
                }
            }
        }


        /// <summary>
        /// 機種名からキュア条件を読み込みます
        /// <para>例外時は"error":"説明文"のdictionaryを返します</para>
        /// </summary>
        public static Dictionary<string, string> Read_ProductRecipeCSV(Dictionary<string, string> dic, string process = "") {
            /*本当は先に製品-条件の辞書を作成しておいて
            　その辞書から見つけたほうが良いかもしれない。
              最終的には登録しておいた社内データベースから取得したほうがいい*/

            string path = RecipeFolder + $"/{process}Product-Recipe.csv";
            var new_dic = dic;
            List<string> keys = new_dic.Keys.ToList();

            //表作成
            DataTable table = new DataTable($"{process}Product-Recipe");

            // カラム名の追加
            table.Columns.Add("product");
            table.Columns.Add("recipe");

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {

                    //1行目を読み込む（使わない）
                    sr.ReadLine();

                    //2行目以降
                    string str, product, recipe;

                    while (true) {
                        str = sr.ReadLine();
                        if (str == "" | str == null) {
                            break;
                        }
                        product = str.Split(',')[0];
                        recipe = str.Split(',')[1];
                        // Rows.Addメソッドを使ってデータを追加
                        table.Rows.Add(product, recipe);
                    }
                }

                foreach (string key in keys) {
                    // LINQを使ってデータを抽出
                    DataRow[] dRows = table.AsEnumerable()
                    .Where(row => row.Field<string>("product") == key).ToArray();

                    if (dRows.Length == 0) {
                        var error_dic = new Dictionary<string, string>();
                        error_dic.Add("error", $"機種-キュア条件のデータがありません({process}Product-Recipe.csv)\n対象機種：{key}");
                        return error_dic;
                    }
                    else if (dRows.Length > 1) {
                        var error_dic = new Dictionary<string, string>();
                        error_dic.Add("error", $"機種-キュア条件のデータに重複があります({process}Product-Recipe.csv)\n対象機種：{key}");
                        return error_dic;
                    }

                    new_dic[key] = dRows[0][1].ToString();
                }

                return new_dic;
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                var error_dic = new Dictionary<string, string>();
                error_dic.Add("error", $"機種-キュア条件のデータ読み込み時にエラーが発生しました({process}Product-Recipe.csv)\r\n{e.Message}");
                return error_dic;
            }
        }

        /// <summary>
        /// FTP送信するサーバーIP,ユーザーネーム、パスワードを読み込む
        /// <para>無い時はnullを返します</para>
        /// <para>例外時は例外をスローします</para>
        /// </summary>
        public static string[] ReadSeraverIP() {
            string path = SettingsFolder + "/ServerIP.txt";

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {

                    //1行目を読み込む
                    string str = sr.ReadLine();

                    if (str == "" | str == null) {
                        return null;
                    }


                    string[] ret = str.Split(',');
                    return ret;
                }
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                throw;
            }
        }

        /// <summary>
        /// Mail送信する為の情報を読み込む
        /// <para>無い時はnullを返します</para>
        /// <para>例外時は例外をスローします</para>
        /// </summary>
        public static Dictionary<string, string> ReadMailInfo() {
            string path = RecipeFolder + "/mail_to.txt";

            Dictionary<string, string> dic = new Dictionary<string, string>();

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {


                    //1行目を読み込む
                    string line = sr.ReadLine();

                    if (line == "" | line == null) {
                        return null;
                    }

                    string[] str = line.Split(',');

                    dic.Add(str[0], str[1]);

                    //2行目以降
                    while (true) {

                        line = sr.ReadLine();

                        if (line == "" | line == null) {
                            break;
                        }

                        str = line.Split(',');

                        dic.Add(str[0], str[1]);

                    }

                    return dic;

                }
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                throw;
            }
        }


    }
}
