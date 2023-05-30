using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


using static Dage_Collection.OperateFile;


namespace Dage_Collection {


    static class Statistics {

        /// <summary>
        /// データの平均値を取得する関数
        /// <para>引数には空白など無い数値文字列リストを渡す</para>
        /// <para>例外時にはDouble.NANを返す</para>
        /// </summary>
        public static double GetAverage(List<string> strList) {

            if (strList.Count == 0) {
                return Double.NaN;
            }

            double sum = 0;

            try {
                foreach (string s in strList) {
                    sum += double.Parse(s);
                }

                double ave = sum / strList.Count;
                return ave;
            }
            catch {
                return Double.NaN;
            }
        }

        /// <summary>
        /// データの最大値を取得する関数
        /// <para>引数には空白など無い数値文字列リストを渡す</para>
        /// </summary>
        public static double GetMax(List<string> strList) {

            if (strList.Count == 0) {
                return Double.NaN;
            }

            double max = -999;

            try {
                foreach (string s in strList) {

                    if (max < double.Parse(s)) {
                        max = double.Parse(s);
                    }
                }

                return max;
            }
            catch {
                return Double.NaN;
            }
        }

        /// <summary>
        /// データの最小値を取得する関数
        /// <para>引数には空白など無い数値文字列リストを渡す</para>
        /// </summary>
        public static double GetMin(List<string> strList) {

            if (strList.Count == 0) {
                return Double.NaN;
            }

            double min = 10000;

            try {
                foreach (string s in strList) {

                    if (min > double.Parse(s)) {
                        min = double.Parse(s);
                    }
                }

                return min;
            }
            catch {
                return Double.NaN;
            }
        }


        /// <summary>
        /// データの標準偏差を取得する関数(ExcelのSTDEV関数)
        /// <para>引数には空白など無い数値文字列リストを渡す</para>
        /// <para>データが1個の時 or 例外時はDouble.NaNを返す</para>
        /// </summary>
        public static double GetSigma(List<string> strList) {

            if (strList.Count == 0) {
                return Double.NaN;
            }

            double ave = GetAverage(strList);
            double temp_sigma = 0;

            try {

                foreach (string s in strList) {
                    temp_sigma += Math.Pow(double.Parse(s) - ave, 2);
                }

                double sigma = Math.Sqrt(temp_sigma / (strList.Count-1));
                return sigma;
            }
            catch {
                return Double.NaN;
            }
        }



    }



    static class TrendConstant {

        /// <summary>
        /// 傾向値管理のA2を求める
        /// </summary>
        public static double Get_A2(string chip_place) {
            double[] A2 = new double[11];

            A2[2] = 1.88; // n=2
            A2[3] = 1.023;
            A2[4] = 0.729;
            A2[5] = 0.577; // n=5
            A2[6] = 0.483;
            A2[7] = 0.419;
            A2[8] = 0.373;
            A2[9] = 0.337;
            A2[10] = 0.308; // n=10

            if (chip_place.Contains("DS-")) {
                return A2[10];
            }
            else if (chip_place.Contains("BS-")) {
                return A2[5];
            }
            else if (chip_place.Contains("PC-")) {
                return A2[5];
            }
            else {
                return Double.NaN;
            }
        }

        /// <summary>
        /// 傾向値管理のD3を求める
        /// </summary>
        public static double Get_D3(string chip_place) {
            double[] D3 = new double[11];

            D3[2] = 0; // n=2
            D3[3] = 0;
            D3[4] = 0;
            D3[5] = 0; // n=5
            D3[6] = 0;
            D3[7] = 0.076;
            D3[8] = 0.136;
            D3[9] = 0.184;
            D3[10] = 0.223; // n=10

            if (chip_place.Contains("DS-")) {
                return D3[10];
            }
            else if (chip_place.Contains("BS-")) {
                return D3[5];
            }
            else if (chip_place.Contains("PC-")) {
                return D3[5];
            }
            else {
                return Double.NaN;
            }
        }

        /// <summary>
        /// 傾向値管理のD4を求める
        /// </summary>
        public static double Get_D4(string chip_place) {
            double[] D4 = new double[11];

            D4[2] = 3.267; // n=2
            D4[3] = 2.575;
            D4[4] = 2.282;
            D4[5] = 2.114;
            D4[6] = 2.004;
            D4[7] = 1.924;
            D4[8] = 1.864;
            D4[9] = 1.816;
            D4[10] = 1.777; // n=10

            if (chip_place.Contains("DS-")) {
                return D4[10];
            }
            else if (chip_place.Contains("BS-")) {
                return D4[5];
            }
            else if (chip_place.Contains("PC-")) {
                return D4[5];
            }
            else {
                return Double.NaN;
            }
        }
    }





    class Trend {

        public string TrendFolder { get; private set; }

        public string Product { get; private set; }

        public string Bonder { get; private set; }

        public List<string> OldTrend_Xlist { get; private set; }

        public List<string> OldTrend_Rlist { get; private set; }


        //コンストラクター
        public Trend(string folder_path, string product, string bonder) {
            TrendFolder = folder_path;
            Product = product;
            Bonder = bonder;
        }


        /// <summary>
        /// 過去のX,Rデータを読み取る
        /// <para>読み取ったデータはプロパティに代入</para>
        /// <para>プロパティ：OldTrend_Xlist,OldTrend_Rlist</para>
        /// <para>データが無い場合、読み取り失敗した場合はfalseを返す</para>
        /// </summary>
        private bool ReadOldTrend(string filepath) {

            string line;        // 1行分データ読み込みしたものを一時的に格納する変数
            string[] strArray;  // 1行分データを分割して一時的に代入するための配列

            string[] X_Array = new string[61]; // 平均値保管配列
            string[] R_Array = new string[61]; // レンジ保管配列
            int[] LineNum_Array = new int[61]; // データ行保管配列

            int row;     //現在読み込んでいる行番号
            int cnt = 0; //配列の要素数カウント用

            try {
                using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read)) {
                    using (StreamReader sr = new StreamReader(fs, enc)) {

                        //1行目読み込み（1行目はヘッダーなので使わない）
                        line = sr.ReadLine();
                        row = 1;

                        //過去データ読み込み処理
                        while (true) {
                            line = sr.ReadLine();

                            //読み取りが無ければループ抜ける
                            if (line == null) { break; }

                            if (line != "") {
                                // 1行分データを分割して配列へ格納
                                strArray = line.Split(',');

                                if (strArray[0] == Product && strArray[2] == Bonder) {
                                    // 保管配列にデータ格納
                                    X_Array[cnt] = strArray[4];
                                    R_Array[cnt] = strArray[5];
                                    LineNum_Array[cnt] = row;

                                    cnt += 1;
                                }
                            }
                            row += 1;
                        }
                    }
                }

                //対象機種、設備のデータが無い場合
                if (cnt == 0) {
                    return false;
                }

                //データ数が50より大きい場合
                if (cnt > 50) {
                    int del_num = cnt - 51;

                    // 機種&装置が同じもののデータが50個以上ある場合は古いもの消去
                    for (int i = del_num; i >= 0; i += -1) {
                        X_Array[i] = "";
                        R_Array[i] = "";
                        Delete_Line(filepath, LineNum_Array[i]); // 古いデータ消去
                    }
                }

                //空白等を除いた過去傾向値データをプロパティにセット
                OldTrend_Xlist = new List<string>();
                OldTrend_Rlist = new List<string>();
                for (int j = 0; j < X_Array.Length; j++) {
                    //空白ではない かつ nullではないなら
                    if (X_Array[j] != "" && X_Array[j] != null) {
                        OldTrend_Xlist.Add(X_Array[j]);
                        OldTrend_Rlist.Add(R_Array[j]);
                    }
                }

                return true;
            }
            catch (Exception e) {
                MessageBox.Show(e.Message + "\n" + e.ToString(), "警告", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return false;
            }
        }

        /// <summary>
        /// 過去の傾向値管理データ確認
        /// <para>傾向値管理有り、傾向値データ有りの場合はtrueを返す</para>
        /// </summary>
        public bool Check_OldTrend(string chip_place) {

            //Trendフォルダが無かったら作る
            if (Directory.Exists(TrendFolder) == false) {
                Directory.CreateDirectory(TrendFolder);
            }

            //過去の傾向値データファイルが見つからなければ終了
            string filename = chip_place + "_trend.csv";
            string filepath = FileExist(TrendFolder, filename);
            if (filepath == null) {
                MessageBox.Show("傾向値管理のデータはありません。\nこの測定箇所は初めてです。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return false;
            }

            //傾向値　過去X,Rデータ読み取り
            if (ReadOldTrend(filepath) == false) {
                return false;
            }

            return true;
        }

    }

}




