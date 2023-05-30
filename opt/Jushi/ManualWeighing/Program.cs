using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;
using System.Configuration;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;

using Newtonsoft.Json;

using ResinClassLibrary;
using ResinVBClassLibrary;

namespace ManualWeighing {

    static class Program {

        /// <summary>
        /// このソフトのバージョン
        /// </summary>
        public static string AppVersion { get; } = "ver1.5.2  2022.12.07";

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {

            try {
                SystemFileFolderPath = ConfigurationManager.AppSettings["SystemFileFolderPath"];

                //このアプリの設定を取得する
                AppSettings = Setting.Get_AppSetting();
            }
            catch (Exception ex) {
                MessageBox.Show($"初期処理に失敗しました。\n終了します。\n\n{ex.ToString()}");
                Application.Exit();
            }

            /*ソフトを多重に起動させない設定*/
            //Mutex名を決める（必ずアプリケーション固有の文字列に変更すること！）
            string mutexName = "ManualWeighing_App";

            //Mutex名の先頭に「Global\」を付けて、Global Mutexにする
            mutexName = "Global\\" + mutexName;

            //すべてのユーザーにフルコントロールを許可するMutexSecurityを作成する
            MutexAccessRule rule = new MutexAccessRule(new SecurityIdentifier(WellKnownSidType.WorldSid, null), MutexRights.FullControl, AccessControlType.Allow);
            MutexSecurity mutexSecurity = new MutexSecurity();
            mutexSecurity.AddAccessRule(rule);
            //Mutexオブジェクトを作成する
            bool createdNew;
            Mutex mutex = new Mutex(false, mutexName, out createdNew, mutexSecurity);

            bool hasHandle = false;
            try {
                try {
                    //ミューテックスの所有権を要求する
                    hasHandle = mutex.WaitOne(0, false);
                }
                //.NET Framework 2.0以降の場合
                catch (AbandonedMutexException) {
                    //別のアプリケーションがミューテックスを解放しないで終了した時
                    hasHandle = true;
                }
                //ミューテックスを得られたか調べる
                if (hasHandle == false) {
                    //得られなかった場合は、すでに起動していると判断して終了
                    MessageBox.Show("既にソフトが起動しています。", "情報", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                //はじめからMainメソッドにあったコードを実行
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            finally {
                if (hasHandle) {
                    //ミューテックスを解放する
                    mutex.ReleaseMutex();
                }
                mutex.Close();
            }
        }

        /// <summary>
        /// このアプリで使う各種設定
        /// </summary>
        public static Setting AppSettings { get; set; }

        /// <summary>
        /// システムファイル(樹脂配合システムに関係するマスタファイル)があるフォルダパス
        /// </summary>
        public static string SystemFileFolderPath { get; set; }

        /// <summary>
        /// ラベルプリンターの名前
        /// </summary>
        public static string LabelPrinterName { get; set; }

        /// <summary>
        /// 計量結果記載する
        /// </summary>
        public static void WriteResult(Recipe recipe, string empcd) {

            try {
                string now = DateTime.Now.ToString("yyyyMMdd");

                //フォルダが無ければ作る
                var folder = $@"{AppSettings.ResultFolderPath}";
                if (Directory.Exists(folder) == false) {
                    Directory.CreateDirectory(folder);
                }

                //ファイルが無ければ作る
                string path = $@"{folder}\{DateTime.Now.ToString("yyyy_MM")}.csv";
                if (File.Exists(path) == false) {
                    string header = "カップ番号,配合手段,樹脂種類,機種名,LotNo,部材名,部材LotNo,目標値[g],結果[g],開始時間,終了時間,作業者\n";
                    using (StreamWriter sw = new StreamWriter(path, false, Encoding.UTF8)) {
                        sw.Write(header);
                    }
                }


                //書き込みテキスト作成
                string base_text = $"{recipe.CupNo},{recipe.FlowMode},{recipe.MoldType}/{recipe.MixTypeCode},{recipe.ProductName},{string.Join("/", recipe.LotNoList)}";
                string log_text = "";
                foreach (int key in recipe.ManualMixResult.Keys.OrderBy(x => x)) {
                    var buzai = recipe.ManualMixResult[key];
                    log_text += $"{base_text},{buzai.Name},{buzai.LotNo},{buzai.Amount},{buzai.ResultAmount},{buzai.StartTime.ToString("yyyy/MM/dd HH:mm:ss")},{buzai.EndTime.ToString("yyyy/MM/dd HH:mm:ss")},{empcd}\n";
                }

                //書き込み
                using (FileStream fs = new FileStream(path, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (StreamWriter sw = new StreamWriter(fs, Encoding.UTF8)) {
                    sw.Write(log_text);
                }
            }
            catch (Exception ex) {
                throw new Exception($"結果ログ保存失敗しました。\n{ex.Message}");
            }
        }


    }


    /// <summary>
    /// このアプリの設定クラス
    /// </summary>
    public class Setting {

        /// <summary>
        /// 設定を保存するファイル
        /// </summary>
        public static string filepath = @"./settings\app-setting.json";

        /// <summary>
        /// 手配合に関するデータがあるベースとなるフォルダ
        /// </summary>
        [JsonProperty("manual-folderPath")]
        public string ManualFolderPath { get; set; }


        /// <summary>
        /// レシピが保存されているフォルダ
        /// </summary>
        [JsonIgnore]
        public string RecipeFolderPath {
            get {
                return $@"{ManualFolderPath}\Dispatchings";
            }
        }

        /// <summary>
        /// 既に作成した樹脂カップのレシピを置いておくフォルダ
        /// </summary>
        [JsonIgnore]
        public string AlreadyMadeRecipeFolderPath {
            get {
                return $@"{RecipeFolderPath}\already-made";
            }
        }

        /// <summary>
        /// 配合結果ファイルを保存するフォルダ
        /// </summary>
        [JsonIgnore]
        public string ResultFolderPath {
            get {
                return $@"{ManualFolderPath}\Result";
            }
        }

        /// <summary>
        /// 電子天秤の設備番号
        /// </summary>
        [JsonProperty("balance-plantcd")]
        public string BalancePlantcd { get; set; }

        /// <summary>
        /// 電子天秤のシリアルポート
        /// </summary>
        [JsonProperty("balance-serialPort")]
        public string BalanceSerialPortName { get; set; }

        /// <summary>
        /// 印刷するラベルフォーマットのファイルパス
        /// </summary>
        [JsonIgnore]
        public string LabelFormatFilePath {
            get {
                return $@"./settings\label\{LabelFormatFileName}";
            }
        }

        /// <summary>
        /// 印刷するラベルフォーマットのファイル名
        /// </summary>
        [JsonProperty("labelFormat-fileName")]
        public string LabelFormatFileName { get; set; }

        /// <summary>
        /// プリンター種類
        /// <para>bpac or zpl</para>
        /// </summary>
        [JsonProperty("printer")]
        public string Printer { get; set; }

        /// <summary>
        /// ドアチェックなどで使用するIOボードのデバイスの名前(CONTEC)
        /// <para>"DIO000"とか</para>
        /// </summary>
        [JsonProperty("dio-deviceName")]
        public string DioDeviceName { get; set; }

        /// <summary>
        /// ドアチェックで使用するセンサーがデバイスに入力しているビット
        /// <para> bitno = 8 → 入力ポート1のbit0の値</para>
        /// </summary>
        [JsonProperty("doorCheck-bitno")]
        public short DoorCheckBitNo { get; set; }

        /// <summary>
        /// 天秤安定時間(秒)
        /// </summary>
        [JsonProperty("stable-second")]
        public int StableSecond { get; set; }

        /// <summary>
        /// 手攪拌時間(秒)
        /// </summary>
        [JsonProperty("manualMix-second")]
        public int ManualMixSecond { get; set; }

        /// <summary>
        /// 排気時間(秒)・・・KSF使用時に使う
        /// </summary>
        [JsonProperty("exhaust-second")]
        public int ExhaustSecond { get; set; }

        /// <summary>
        /// 総重量判定するかどうか
        /// <para>する = true</para>
        /// </summary>
        [JsonProperty("check-grossWeight")]
        public bool CheckGrossWeight { get; set; }

        /// <summary>
        /// 総重量判定許容誤差(g)
        /// </summary>
        [JsonProperty("grossWeight-allowableErrorGram")]
        public decimal GrossWeightAllowableErrorGram { get; set; }

        /// <summary>
        /// 風速計のシリアルポート
        /// </summary>
        [JsonProperty("kanomax-serialPort")]
        public string kanomaxSerialPortName { get; set; }



        /// <summary>
        /// AppSettingを取得する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static Setting Get_AppSetting() {

            string json;
            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (StreamReader sr = new StreamReader(fs, Encoding.UTF8)) {
                json = sr.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<Setting>(json);
        }

        /// <summary>
        /// AppSettingを保存する
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public void Save_AppSetting() {

            string json = JsonConvert.SerializeObject(this, Formatting.Indented);

            using (FileStream fs = new FileStream(filepath, FileMode.Create, FileAccess.Write, FileShare.ReadWrite))
            using (var sw = new StreamWriter(fs, Encoding.UTF8)) {
                sw.Write(json);
            }
        }
    }
}
