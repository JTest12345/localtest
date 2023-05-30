using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using static Dage_Collection.Program;

namespace Dage_Collection {
    static class OperateFile {

        /// <summary>
        /// ファイル読み書き用文字コード：UTF8
        /// </summary>
        public static Encoding enc { get; private set; } = Encoding.UTF8;



        /// <summary>
        /// テキストファイルの指定行を読み込み
        /// </summary>
        public static string Read_Line(string filepath, int lineNum) {
            string[] lines;

            try {
                // 全行読み込み
                lines = File.ReadAllLines(filepath, enc);
                return lines[lineNum - 1];
            }
            catch {
                throw;
            }
        }

        /// <summary>
        /// テキストファイルの指定行目を書き換える
        /// </summary>
        public static void Write_Line(string filepath, int lineNum, string write_text) {
            string[] lines;
            string new_text = "";

            try {
                // 全行読み込み
                lines = File.ReadAllLines(filepath, enc);
                // 指定行の内容書き換え
                lines[lineNum - 1] = write_text;

                for (int i = 0; i < lines.Length; i++) {
                    new_text += lines[i];

                    //最後の行以外改行付ける
                    if (i < lines.Length - 1)
                        new_text += "\n";
                }

                // ファイルへ書き込み
                File.WriteAllText(filepath, new_text, enc);
            }
            catch {
                throw;
            }
        }


        /// <summary>
        /// 指定のフォルダに特定の文字列があるファイル名があるか調べて、見つかれば最初のファイル名を1つだけ返す
        /// <para>見つからない時はnullを返す</para>
        /// </summary>
        public static string FileExist(string dir_name, string find_name) {
            string[] filelist = Directory.GetFiles(dir_name, find_name);

            if (filelist.Length == 0) {
                return null;
            }

            return filelist[0];
        }


        /// <summary>
        /// テキストファイルの指定行目を削除する
        /// </summary>
        public static bool Delete_Line(string filepath, int lineNum) {
            string[] lines;
            string writeText = "";

            try {
                //全行読み込み
                lines = File.ReadAllLines(filepath, enc);

                //再書き込みテキスト作成
                for (int i = 0; i < lines.Length; i++) {
                    if (i == lineNum) {
                    }
                    else {
                        writeText += lines[i] + "\n";
                    }
                } 

                File.WriteAllText(filepath, writeText, enc); //ファイルへ書き込み
                return true;
            }
            catch {
                throw;
            }
        }




        /// <summary>
        /// ファイルに測定結果データを保存する
        /// </summary>
        public static void DataSave(string filepath, string text) {

            // フォルダが無かったら作る
            string fld = Path.GetDirectoryName(filepath);
            if (Directory.Exists(fld) == false) {
                Directory.CreateDirectory(fld);
            }

            // ファイルが無かったらファイルを作成してヘッダーを書く
            if (File.Exists(filepath) == false) {
                string header = "機種,LotNo,測定箇所,強度,破壊モード,コメント,設備,作業者,日時,テスト装置,テストグループ\n";
                File.AppendAllText(filepath, header, enc);
            }

            //テキストを追加
            File.AppendAllText(filepath, text, enc);
        }

        /// <summary>
        /// 傾向値管理データを保存する
        /// </summary>
        public static void TrendDataSave(string filepath, string text) {

            // フォルダが無かったら作る
            string fld = Path.GetDirectoryName(filepath);
            if (Directory.Exists(fld) == false) {
                Directory.CreateDirectory(fld);
            }

            // ファイルが無かったらファイルを作成してヘッダーを書く
            if (File.Exists(filepath) == false) {
                string header = "機種,LotNo,設備,日時,X,R,N\n";
                File.AppendAllText(filepath, header, enc);
            }

            //テキストを追加
            File.AppendAllText(filepath, text, enc);
        }

        /// <summary>
        /// データ修正履歴を保存する
        /// </summary>
        public static void ChangeLogSave(string filepath, string text) {

            // ファイルが無かったらファイルを作成してヘッダーを書く
            if (File.Exists(filepath) == false) {
                string header = "日時,機種,LotNo,測定箇所,強度,破壊モード,コメント,,強度,破壊モード,コメント,変更内容,修正理由\n";
                File.AppendAllText(filepath, header, enc);
            }

            //テキストを追加
            File.AppendAllText(filepath, text, enc);
        }



        /// <summary>
        /// 機種ごとの規格下限値を保存する
        /// </summary>
        /// <param name="filename">機種名.csvの形</param>
        /// <param name="key">例：BS-LED-Anode</param>
        /// <param name="value">例：25gf</param>
        public static void Save_StandardData(string filename, string key, string value) {

            // フォルダが無かったら作る
            if (Directory.Exists(StandardFolder) == false) {
                Directory.CreateDirectory(StandardFolder);
            }

            //ファイル探してファイルパス取得
            string filepath = FileExist(StandardFolder, filename);


            //ファイルが見つかったら
            if (filepath != null) {
                Dictionary<string, string> dic = new Dictionary<string, string>();

                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {

                    string line;
                    string[] strArray;
                    while (true) {

                        line = sr.ReadLine();

                        //読み取りが無ければループ抜ける
                        if (line == null) { break; }

                        if (line != "") {

                            //1行分データを分割して配列へ格納
                            strArray = line.Split(',');
                            //辞書に追加
                            dic.Add(strArray[0], strArray[1]);
                        }
                    }
                }

                if (dic.ContainsKey(key)) {
                    dic[key] = value;
                }
                else {
                    dic.Add(key, value);
                }

                //ファイル消す
                File.Delete(filepath);


                string new_text = "";

                foreach (string s in dic.Keys) {
                    new_text += $"{s},{dic[s]}\n";
                }

                //ファイル新規作成で書き込み
                File.AppendAllText(filepath, new_text, enc);

            }
            else {
                filepath = $"{StandardFolder}/{filename}";

                //ファイル新規作成で書き込み
                File.AppendAllText(filepath, $"{key},{value}", enc);
            }
        }





        /// <summary>
        /// 機種ごとの規格下限値を確認する
        /// <para>ファイルが無い時、例外発生時はnullを返す</para>
        /// </summary>
        /// <param name="filename">機種名.csvの形</param>
        public static Dictionary<string, string> Read_StandardData(string filename) {
            string filepath = $"{StandardFolder}/{filename}";

            try {
                //ファイルが無ければ終了
                if (File.Exists(filepath) == false) {
                    return null;
                }

                Dictionary<string, string> dic = new Dictionary<string, string>();

                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, enc)) {

                    string line;
                    string[] strArray;

                    while (true) {

                        line = sr.ReadLine();

                        //読み取りが無ければループ抜ける
                        if (line == null) { break; }

                        if (line != "") {

                            // 1行分データを分割して配列へ格納
                            strArray = line.Split(',');

                            dic.Add(strArray[0], strArray[1]);
                        }
                    }

                    return dic;
                }
            }
            catch {
                return null;
            }
        }
    }
}


