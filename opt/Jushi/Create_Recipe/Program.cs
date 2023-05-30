using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

using Newtonsoft.Json;

namespace Create_Recipe {

    public static class Program {

        /// <summary>
        /// このソフトのバージョン
        /// </summary>
        public static string AppVersion { get; } = "ver1.3.0  2023.1.23";

        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {

            //初期設定(config file)
            Place = ConfigurationManager.AppSettings["Place"];
            KodaConStr = ConfigurationManager.AppSettings["KodaConStr"];
            SystemFileFolderPath = ConfigurationManager.AppSettings["SystemFileFolderPath"];

            try {
                CheckAppVersion();

                AppUsePath = UsePath.ReadSetting();
                Copy_LabelFile();
            }
            catch (Exception ex) {
                MessageBox.Show(ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Stop);
                return;
            }

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());

        }

        /// <summary>
        /// このアプリケーション(.exe)があるフォルダ
        /// </summary>
        public static string AppFolder = Path.GetDirectoryName(Application.ExecutablePath);

        /// <summary>
        /// このアプリケーションで使用するパス
        /// </summary>
        public static UsePath AppUsePath { get; set; }

        /// <summary>
        /// KODA接続文字列
        /// </summary>
        public static string KodaConStr { get; set; }

        /// <summary>
        /// システムファイル(樹脂配合システムに関係するマスタファイル)があるフォルダパス
        /// </summary>
        public static string SystemFileFolderPath { get; set; }

        /// <summary>
        /// 製造場所(TCとか)
        /// </summary>
        public static string Place { get; set; }

        /// <summary>
        /// 最新版ソフトかどうか確認する
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void CheckAppVersion() {
#if DEBUG
#else
            using (var fs = new FileStream($@"{SystemFileFolderPath}\Create_Recipe_latest-version.txt", FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.UTF8)) {
                string line = sr.ReadLine();
                if (line != AppVersion) {
                    throw new Exception($"このソフトは最新版ではありません。\n最新版を入手して下さい。\n\n最新版：{line}\n\nソフトを終了します。");
                }
            }
#endif
        }

        /// <summary>
        /// 最新ラベルファイルを取得する
        /// </summary>
        public static void Copy_LabelFile() {
            string label_folder = $@"{SystemFileFolderPath}\label";

            var files = Directory.GetFiles(label_folder, "*.lbx");

            string dest_folder = $@"./label";
            if (Directory.Exists(dest_folder) == false) {
                Directory.CreateDirectory(dest_folder);
            }

            foreach (var f in files) {
                string dest_filename = $@"{dest_folder}\{Path.GetFileName(f)}";
                File.Copy(f, dest_filename, true);
            }

        }
    }

    public class UsePath {

        /// <summary>
        /// 手配合レシピ保存先
        /// <para>電子天秤配合ソフトがレシピを読み取るフォルダ</para>
        /// </summary>
        [JsonProperty("manualRecipe-saveFolderPath")]
        public string ManualRecipe_SaveFolderPath { get; set; }

        /// <summary>
        /// 既に手配合した樹脂カップのレシピを置いておくフォルダ
        /// </summary>
        [JsonIgnore]
        public string AlreadyMadeRecipeFolderPath {
            get {
                return $@"{ManualRecipe_SaveFolderPath}\already-made";
            }
        }

        /// <summary>
        /// 自動配合レシピ保存先フォルダ
        /// <para>配合自動機がレシピを読み取る</para>
        /// </summary>
        [JsonProperty("autoMachine-saveFolderPath")]
        public AutoRecipe_SaveFolderPath AutoMachine_SaveFolderPath { get; set; }

        /// <summary>
        /// 配合自動機が残量を記録するファイル
        /// </summary>
        [JsonProperty("autoMachine-remainingWeight-filePath")]
        public string AutoMachine_RemainingWeight_FilePath { get; set; }

        /// <summary>
        /// 先行評価ログがデータベース化されているかどうか
        /// <para>true = 先行評価ログはデータベース</para>
        /// </summary>
        [JsonProperty("senkolog-database")]
        public bool SenkologDatabase { get; set; }

        /// <summary>
        /// 使用する先行ログファイル
        /// </summary>
        [JsonProperty("senkoLog-filePath")]
        public string SenkoLog_FilePath { get; set; }


        public class AutoRecipe_SaveFolderPath {

            /// <summary>
            /// 配合手段がAUTOの時のレシピ保存先フォルダ
            /// </summary>
            [JsonProperty("auto-saveFolderPath")]
            public string Auto_SaveFolderPath { get; set; }

            /// <summary>
            /// 配合手段がADD、CUT_INの時のレシピ保存先フォルダ
            /// </summary>
            [JsonProperty("add-saveFolderPath")]
            public string Add_SaveFolderPath { get; set; }
        }

        /// <summary>
        /// ファイルからパスを読み込みクラスを返す
        /// </summary>
        public static UsePath ReadSetting() {

            string path = $@"{Program.SystemFileFolderPath}\{Program.Place}_path-setting.json";

            string json = "";
            using (var fs = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            using (var sr = new StreamReader(fs, Encoding.UTF8)) {
                json = sr.ReadToEnd();
            }

            var obj = JsonConvert.DeserializeObject<UsePath>(json);

            return obj;
        }

        /// <summary>
        /// 設定ファイルが更新されたか確認する
        /// <para>true : 更新された</para>
        /// </summary>
        public static bool CheckUpdateSetting() {

            string path = $@"{Program.SystemFileFolderPath}\{Program.Place}_path-setting.json";

            //設定ファイルの更新日時確認
            var updatetime = File.GetLastWriteTime(path).ToString("yyyy/MM/dd HH:mm:ss");
            string last_updatetime = ConfigurationManager.AppSettings["PlaceSettingLastUpdate"];
            if (updatetime == last_updatetime) {
                return false;
            }

            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings["PlaceSettingLastUpdate"].Value = updatetime;
            config.Save();

            return true;
        }

    }
}
