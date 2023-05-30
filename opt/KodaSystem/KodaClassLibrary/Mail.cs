using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*
 * NuGetでMailKitをインストールして使う
 */

namespace KodaClassLibrary {

    //メールクラス
    public class Mail {

        /// <summary>
        /// メール本体部分
        /// </summary>
        private MimeKit.MimeMessage mail = new MimeKit.MimeMessage();

        /// <summary>
        /// メール本文
        /// </summary>
        public string mailText { get; set; } = "";

        /// <summary>
        /// 添付ファイル
        /// </summary>
        public List<string> attachFile { get; set; } = null;

        /// <summary>
        /// 添付ファイルをCloseするためにリストを使う
        /// </summary>
        private List<FileStream> attachfile_stream;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="from_name">差出人の名前</param>
        /// <param name="from_address">差出人のアドレス</param>
        /// <param name="mail_to">{key=名前,value=アドレス}</param>
        public Mail(string from_name, string from_address, Dictionary<string, string> mail_to, string subject = "") {
            //差出人の設定
            mail.From.Add(new MimeKit.MailboxAddress(from_name, from_address));

            //宛先の設定
            mail_to.Keys.ToList().ForEach(key =>
            {
                mail.To.Add(new MimeKit.MailboxAddress(key, mail_to[key]));
            });

            //件名設定
            mail.Subject = subject;

        }

        private void make_mail() {

            //メール中身
            var multipart = new MimeKit.Multipart("mixed");

            //テキスト部分を作成
            var textPart = new MimeKit.TextPart(MimeKit.Text.TextFormat.Text);
            textPart.Text = mailText;

            // メール中身にテキスト部分を付ける
            multipart.Add(textPart);

            //添付ファイルをCloseするためにリストを使う
            attachfile_stream = null;

            if (attachFile != null) {
                //メール中身に添付ファイルを付ける
                try {
                    attachfile_stream = new List<FileStream>();

                    attachFile.ForEach(path =>
                    {
                        var mime_type = MimeKit.MimeTypes.GetMimeType(path);
                        FileStream fs = File.Open(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);

                        var attachment = new MimeKit.MimePart(mime_type) {
                            Content = new MimeKit.MimeContent(fs),
                            ContentDisposition = new MimeKit.ContentDisposition(),
                            ContentTransferEncoding = MimeKit.ContentEncoding.Base64,
                            FileName = Path.GetFileName(path)
                        };

                        attachfile_stream.Add(fs);

                        multipart.Add(attachment);
                    });
                }
                catch (Exception ex) {
                    if (attachfile_stream != null) {
                        foreach (var att in attachfile_stream) {
                            if (att != null)
                                att.Close();//メール添付に使用したファイルは全て閉じる
                        }
                    }
                    throw new Exception("メールの作成に失敗しました。");
                }
            }

            //MimeMessageを完成させる
            mail.Body = multipart;
        }

        /// <summary>
        /// メールを送る処理(同期)
        /// <para>先にmailTextプロパティで本文追加しとく</para>
        /// <para>先にattachFileプロパティで添付ファイル追加しとく</para>
        /// </summary>
        /// <param name="host">SMTPサーバの名前</param>
        /// <param name="port">SMTPサーバのポート番号</param>
        public void send(string host, int port) {

            //メール作成
            make_mail();

            //ここから下はSMTPサーバに接続してメールを送信する処理
            using (var client = new MailKit.Net.Smtp.SmtpClient()) {

                try {
                    client.Connect(host, port); //接続(指定のSMTPサーバ)

                    //SMTPサーバがユーザー認証を必要としない場合は、次の行は不要
                    //client.Authenticate(userName, password) '認証

                    client.Send(mail); // 送信

                    client.Disconnect(true); // 切断
                }
                catch (Exception ex) {
                    throw new Exception("SMTPサーバに接続してメールを送信する処理に失敗しました。");
                }

                finally {
                    if (attachfile_stream != null) {
                        foreach (var att in attachfile_stream) {
                            if (att != null)
                                att.Close();//メール添付に使用したファイルは全て閉じる
                        }
                    }
                }
            }
        }

        /// <summary>
        /// メールを送る処理(非同期)
        /// <para>先にmailTextプロパティで本文追加しとく</para>
        /// <para>先にattachFileプロパティで添付ファイル追加しとく</para>
        /// </summary>
        /// <param name="host">SMTPサーバの名前</param>
        /// <param name="port">SMTPサーバのポート番号</param>
        public async Task send_async(string host, int port) {

            //メール作成
            make_mail();

            //ここから下はSMTPサーバに接続してメールを送信する処理
            using (var client = new MailKit.Net.Smtp.SmtpClient()) {

                try {
                    await client.ConnectAsync(host, port); //接続(指定のSMTPサーバ)
                    //client.Connect(host, port); //接続(指定のSMTPサーバ)

                    //SMTPサーバがユーザー認証を必要としない場合は、次の行は不要
                    //Await client.AuthenticateAsync(userName, password) '認証

                    await client.SendAsync(mail); // 送信
                    //client.Send(mail); // 送信

                    await client.DisconnectAsync(true); // 切断
                    //client.Disconnect(true); // 切断
                }
                catch (Exception ex) {
                    throw new Exception("SMTPサーバに接続してメールを送信する処理に失敗しました。");
                }

                finally {
                    if (attachfile_stream != null) {
                        foreach (var att in attachfile_stream) {
                            if (att != null)
                                att.Close();//メール添付に使用したファイルは全て閉じる
                        }
                    }
                }
            }
        }

        /// <summary>
        /// KODAシステムからメールを送る(同期)
        /// </summary>
        /// <param name="mail_to">メール宛先</param>
        /// <param name="subject">件名</param>
        /// <param name="mail_text">本文</param>
        /// <param name="attach_files">添付ファイル</param>
        public static void MailSend_from_Koda(Dictionary<string, string> mail_to, string subject, string mail_text, List<string> attach_files = null) {

            var mail = new Mail("KODA", "koda-system@citizen.co.jp", mail_to, subject);
            mail.mailText = mail_text;
            mail.attachFile = attach_files;
            mail.send("polaris.citizen.co.jp", 25);
        }

    }
}
