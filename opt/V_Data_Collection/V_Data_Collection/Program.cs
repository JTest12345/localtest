using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace V_Data_Collection {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainForm());
        }

        /// <summary>
        /// このアプリケーション(.exe)があるフォルダ
        /// </summary>
        public static string AppFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        /// <summary>
        /// LM-1000からのcsvファイルを受信するフォルダ
        /// </summary>
        public static string DataFolder = ConfigurationManager.AppSettings["datafolder"];

        /// <summary>
        /// 測定データを保存するフォルダ
        /// </summary>
        public static string SaveFolder = ConfigurationManager.AppSettings["savefolder"];

        /// <summary>
        /// 生データ移動先フォルダ
        /// </summary>
        public static string RawFolder = $@"{AppFolder}\raw_data";

    }

}
