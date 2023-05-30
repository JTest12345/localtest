using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static M_Com.Program;

namespace M_Com {
    public class Standard {

        public string file_errorlog { get; private set; } = AppFolder + "/Log/file_errorlog.txt";

        /// <summary>
        /// ファイル共有モードで末尾にテキスト追加
        /// </summary>
        private void WriteAppend(string path, string text, bool throw_ex = false) {

            try {
                //フォルダが無かったら作る
                string fld = Path.GetDirectoryName(path);
                if (Directory.Exists(fld) == false) {
                    Directory.CreateDirectory(fld);
                }

                //FileMode.Appendはファイルが無かったら新規作成する
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
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
        /// 機種に応じた条件ファイルパスを取得する
        /// </summary>
        private string get_file_path(string product_name) {

            //データ保存先フォルダの1個上階層のフォルダ
            string parentPath = Path.GetDirectoryName(SaveFolder);

            //そこにある機種-マスタファイル一覧表
            string path = $"{parentPath}/system_file/{Process}_Product-Standard.csv";

            try {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

                    //1行目読み込み（使わない）
                    string line = sr.ReadLine();

                    while (true) {
                        line = sr.ReadLine();

                        if (line == null || line == "") {
                            return null;
                        }

                        string[] str_array = line.Split(',');

                        if (str_array[0] == product_name) {
                            //マスタファイルのファイルパスを返す
                            return $"{parentPath}/system_file/{str_array[1]}.csv";
                        }
                    }
                }
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                return null;
            }
        }



        /// <summary>
        /// 上下限などの規格値を取得する
        /// <para>マスタが無い時はnullを返す</para>
        /// </summary>
        public Dictionary<string, StandardValue> get_product_standard(string product_name) {

            string file_path = get_file_path(product_name);

            //マスタが無ければnullを返す
            if (file_path == null) {
                return null;
            }

            var dic = new Dictionary<string, StandardValue>();

            try {
                using (FileStream fs = new FileStream(file_path, FileMode.Open, FileAccess.Read, FileShare.Read))
                using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {

                    //1行目読み込み（使わない）
                    string line = sr.ReadLine();

                    while (true) {
                        line = sr.ReadLine();

                        if (line == null || line == "") {
                            break;
                        }

                        string[] str_array = line.Split(',');

                        string item = $"{str_array[0]}_{str_array[1]} {str_array[2]}";//[1]と[2]の間に半角スペース有り
                        //例⇒            Blue-Anode     ボール径            X

                        double upper = double.Parse(str_array[3]);
                        double lower = double.Parse(str_array[4]);
                        int cnt = int.Parse(str_array[5]);

                        var sv = new StandardValue(upper, lower, cnt);

                        dic.Add(item, sv);

                    }
                }
                return dic;
            }
            catch (Exception e) {
                string dt = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff");
                WriteAppend(file_errorlog, dt + "\t" + e.Message);
                return null;
            }
        }


        public class StandardValue {

            /// <summary>
            /// 測定数の基準値
            /// <para>初期値 = 0</para>
            /// </summary>
            public int Count { get; private set; }

            /// <summary>
            /// 規格上限値
            /// <para>初期値 = 9999</para>
            /// </summary>
            public double Upper { get; private set; }

            /// <summary>
            /// 規格下限値
            /// <para>初期値 = 0</para>
            /// </summary>
            public double Lower { get; private set; }


            /// <summary>
            /// コンストラクタ
            /// </summary>
            public StandardValue(double upper = 9999, double lower = 0, int count = 0) {
                Upper = upper;
                Lower = lower;
                Count = count;
            }
        }


    }
}
