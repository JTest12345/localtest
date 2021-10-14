using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Samples
{
    class Mailkit
    {
        static void Main(string[] args)
        {
            SendMail("watanabejunic@citizen.co.jp", "TEST", "Regards");
        }


        // メール送信関数 ：引数(宛先アドレス, 件名, 本文)
        static private void SendMail(string strToAddr, string strSubject, string strMessage)
        {
            using (var smtp = new MailKit.Net.Smtp.SmtpClient())
            {
                // メールサーバ接続情報 
                string strHost = "polaris.citizen.co.jp";
                int nPort = 25;
                MailKit.Security.SecureSocketOptions mailSecOpt = MailKit.Security.SecureSocketOptions.None;
                string strUsrId = "user";
                string strUsrPass = "password";
                string strUsrAddr = "no-reply@citizen.co.jp";
                Console.WriteLine(strToAddr + "にメール送信");
                // SMTPサーバに接続
                smtp.Connect(strHost, nPort, mailSecOpt);
                // 認証が不要な場合は、以下をコメントアウト
                //smtp.Authenticate(strUsrId, strUsrPass);
                // 送信するメールを作成
                MimeKit.MimeMessage mail = new MimeKit.MimeMessage();
                MimeKit.BodyBuilder builder = new MimeKit.BodyBuilder();
                mail.From.Add(new MimeKit.MailboxAddress("", strUsrAddr));
                mail.To.Add(new MimeKit.MailboxAddress("", strToAddr));
                mail.Subject = strSubject;
                builder.TextBody = strMessage;
                mail.Body = builder.ToMessageBody();
                // メールを送信
                smtp.Send(mail);
                // SMTPサーバから切断
                smtp.Disconnect(true);
                Console.WriteLine("メール送信完了");
            }
        }
    }
}
