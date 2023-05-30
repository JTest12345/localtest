using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;
using System.Threading;
using System.Security.AccessControl;
using System.Security.Principal;

namespace Oven_Control {
    static class Program {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main() {

            //Mutex名を決める（必ずアプリケーション固有の文字列に変更すること！）
            string mutexName = "Oven_Control_App";

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
        /// このアプリケーション(.exe)があるフォルダ
        /// </summary>
        public static string AppFolder = System.IO.Path.GetDirectoryName(Application.ExecutablePath);

        public static string SettingsFolder = AppFolder + "/Settings";
        public static string FtpFolder = AppFolder + "/Ftp";
        public static string RecipeFolder = ConfigurationManager.AppSettings["RecipeFolder"];
        public static string SmtpServer = ConfigurationManager.AppSettings["SmtpServer"];
        public static int SmtpPort = int.Parse(ConfigurationManager.AppSettings["SmtpPort"]);
        public static string FromMail = ConfigurationManager.AppSettings["FromMail"];


        /// <summary>
        /// trueの場合はFTPで上位システムへログファイルを転送する
        /// </summary>
        public static bool use_ftp { get; set; } = Boolean.Parse(ConfigurationManager.AppSettings["UseFTP"]);

        /// <summary>
        /// trueの場合は温度上下限異常時にMail送信する
        /// </summary>
        public static bool use_mail { get; set; } = Boolean.Parse(ConfigurationManager.AppSettings["UseMail"]);

        /// <summary>
        /// ArmsSystemを使用して機種名、LotNoを取得するかどうか
        /// <para>Arms Systemを使用する = true</para>
        /// </summary>
        public static bool armsSystem = bool.Parse(ConfigurationManager.AppSettings["ArmsSystem"]);



    }
}
