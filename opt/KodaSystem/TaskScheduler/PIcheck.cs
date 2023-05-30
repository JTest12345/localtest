using System;
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

    public static class PIcheck {

        /// <summary>
        /// 定期点検の開始日、実施期限チェックをする
        /// </summary>
        public static void CheckPI() {

            FormsSQL form_sql = new FormsSQL(DBConStrDic["PIcheck"]);

            var now = DateTime.Now;

            string path = $"{now.ToString("yyyy-MM-dd")}.csv";
            string fullpath = Path.GetFullPath(path);
            try {

                var exp_list = form_sql.Get_ExpiredPI<FormsSQL.TnPIFormData>(now);

                var new_list = form_sql.Get_NewStartPI<FormsSQL.TnPIFormData>(now);

                //csvファイル作成＋書き込み
                using (var fs = new FileStream(fullpath, FileMode.Append, FileAccess.Write, FileShare.ReadWrite))
                using (var sw = new StreamWriter(fs, Encoding.UTF8)) {

                    if (exp_list.Count != 0) {
                        sw.WriteLine("【実施期限が過ぎている点検】");
                        sw.WriteLine("ID,設備型式,設備番号,製造拠点,場所,点検周期,点検開始日,実施期限");
                        foreach (var exp in exp_list) {
                            string text = $"{exp.ID.ToString()},{exp.MacName},{exp.Plantcd},{exp.ManuBase},{exp.Place},";
                            text += $"{FormsSQL.TnPIFormData.period_dic[exp.Period]},{exp.Next},{exp.Limit}";
                            sw.WriteLine(text);
                        }
                        sw.WriteLine();
                    }

                    if (new_list.Count != 0) {
                        sw.WriteLine("【点検開始日になった点検】");
                        sw.WriteLine("ID,設備型式,設備番号,製造拠点,場所,点検周期,点検開始日,実施期限");
                        foreach (var n in new_list) {
                            string text = $"{n.ID.ToString()},{n.MacName},{n.Plantcd},{n.ManuBase},{n.Place},";
                            text += $"{FormsSQL.TnPIFormData.period_dic[n.Period]},{n.Next},{n.Limit}";
                            sw.WriteLine(text);
                        }
                        sw.WriteLine();
                    }
                }


                //何もないならメールしない
                if (exp_list.Count == 0 && new_list.Count == 0) {
                    //何もしない
                }
                else {
                    var dic = Get_MailTo();
                    var mail = new Mail("定期点検チェック", "3-3F_CR@citizen.co.jp", dic, "設備定期点検チェック");
                    string mailbody = "関係各位\n\n";

                    if (exp_list.Count != 0) {
                        mailbody += "【実施期限が過ぎている点検】\n";

                        foreach (var ge in exp_list.GroupBy(x => new { x.ManuBase, x.Place }).ToList()) {
                            mailbody += $"{ge.Key.ManuBase}\t{ge.Key.Place}\t{ge.Count()}件\n";
                        }
                        mailbody += "\n";
                    }


                    if (new_list.Count != 0) {
                        mailbody += "【点検開始日になった点検】\n";

                        foreach (var gn in new_list.GroupBy(x => new { x.ManuBase, x.Place }).ToList()) {
                            mailbody += $"{gn.Key.ManuBase}\t{gn.Key.Place}\t{gn.Count()}件\n";
                        }
                        mailbody += "\n";
                    }

                    mail.mailText = mailbody;

                    mail.attachFile = new List<string>() { fullpath };

                    var task = mail.send_async("polaris.citizen.co.jp", 25);
                    task.Wait();
                }
            }
            catch (Exception ex) {
                var dic = Get_MailTo();
                var mail = new Mail("定期点検チェック", "3-3F_CR@citizen.co.jp", dic, "【エラー】設備定期点検チェック");
                string mailbody = "関係各位\n\n";

                mailbody += "設備定期点検チェックでエラーが発生しました。\n\n";
                mailbody += "【エラー内容】\n";
                mailbody += $"{ex.ToString()}\n";

                mail.mailText = mailbody;

                var task = mail.send_async("polaris.citizen.co.jp", 25);
                task.Wait();
            }
            finally {
                if (File.Exists(fullpath)) {
                    //ファイル消す
                    File.Delete(fullpath);
                }
            }

        }

        /// <summary>
        /// メール宛先を読み込む
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> Get_MailTo() {

            string json = ReadText($@"{MailDir}\picheck_mail-to.json");
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return dic;
        }


    }
}
