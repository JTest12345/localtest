﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using static TaskScheduler.Program;
using KodaClassLibrary;
using static KodaClassLibrary.UtilFuncs;

namespace TaskScheduler {

    /// <summary>
    /// FTPで送られて来たファイルを処理するクラス
    /// </summary>
    public static class OndotoriFtpData {

        /// <summary>
        /// FTPファイルを読み込んでデータベースに書き込む
        /// </summary>
        public static void Get_RTR500BC_Data() {

            //データベース操作オブジェクト
            var ondotori_sql = new OndotoriSQL(DBConStrDic["Ondotori"]);

            string[] filepaths;

            try {
                filepaths = Directory.GetFiles(FtpFldPathDic["RTR500BC"], "*.xml");
            }
            catch (Exception ex) {
                SendMail(ex.ToString());
                return;
            }

            if (filepaths.Length == 0) {
                return;
            }

            for (int i = 0; i < filepaths.Length; i++) {

                string path = filepaths[i];

                try {
                    //ファイル読み込んでオブジェクト取得
                    var obj = RTR500BC.GetObj_from_xmlFile(path);

                    //nullではない時は送信テストファイルなので何もしない
                    if (obj.Root.Sample == null) {
                        ondotori_sql.Insert_CurrentData(obj);
                    }
                }
                catch (Exception ex) {
                    string str = $"{i}/{filepaths.Length}の時にエラー発生";
                    SendMail($"{str}\n{ex.ToString()}");
                    //処理終了
                    return;
                }

                //xmlファイル削除
                int cnt = 0;
                while (true) {
                    try {
                        File.Delete(path);
                        break;
                    }
                    catch (IOException) {
                        Thread.Sleep(1000);//早すぎると失敗するかもなので
                        cnt += 1;
                        if (cnt > 10) { break; }
                    }
                }

            }
        }

        public static void SendMail(string text) {
            var dic = Get_MailTo();
            //var mail = new Mail("KODAシステムエラー", "3-3F_CR@citizen.co.jp", dic, "【エラー】おんどとりデータ定期処理");
            string mailbody = "関係各位\n\n";

            mailbody += "おんどとりデータ定期処理でエラーが発生しました。\n\n";
            mailbody += "【エラー内容】\n";
            mailbody += $"{text}\n";

            //mail.mailText = mailbody;

            Mail.MailSend_from_Koda(dic, "【エラー】おんどとりデータ定期処理", mailbody);

            //var task = mail.send_async("polaris.citizen.co.jp", 25);
            //task.Wait();
        }

        /// <summary>
        /// メール宛先を読み込む
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> Get_MailTo() {

            string json = ReadText($@"{MailDir}\ondotori_mail-to.json");
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return dic;
        }

    }
}
