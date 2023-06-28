﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

namespace Rhesca_Collection {
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
        /// データ保存先のフォルダ
        /// </summary>
        public static string SaveFolder = ConfigurationManager.AppSettings["SaveFolder"];

        /// <summary>
        /// 測定器の型番_シリアル番号
        /// </summary>
        public static string MeasureSerial = ConfigurationManager.AppSettings["MeasureSerial"];

        /// <summary>
        /// ArmsSystemを使用して機種名、LotNoを取得するかどうか
        /// <para>Arms Systemを使用する = true</para>
        /// </summary>
        public static bool armsSystem = bool.Parse(ConfigurationManager.AppSettings["ArmsSystem"]);

    }
}