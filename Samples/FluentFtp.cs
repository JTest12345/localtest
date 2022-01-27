﻿using FluentFTP;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;


namespace FluentFTPtest
{
    public class FluetFtpTst
    {
        static void _Main()
        {
            //var ftptest = new FluentFtp();

            DownloadFileExample.DownloadFile();

            Console.WriteLine("Done!");
            Console.ReadKey();
        }
    }


    internal static class DownloadFileExample
    {

        public static void DownloadFile()
        {
            try
            {
                using (var ftp = new FtpClient("127.0.0.1", "test", ""))
                {
                    ftp.ConnectTimeout = 5000;
                    ftp.Connect();

                    // download a file and ensure the local directory is created
                    ftp.DownloadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto_ftp.csv", @"tmp\00002_bto.csv");

                    // download a file and ensure the local directory is created, verify the file after download
                    // ftp.DownloadFile(@"D:\Github\FluentFTP\README.md", "/public_html/temp/README.md", FtpLocalExists.Overwrite, FtpVerify.Retry);

                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
            }
            
        }


        // Asyncはトークンが必要みたい、サンプルの何処かにある？(https://github.com/robinrodricks/FluentFTP/tree/master/FluentFTP.CSharpExamples)

        //public static async Task DownloadFileAsync()
        //{
        //    var token = new CancellationToken();
        //    using (var ftp = new FtpClient("127.0.0.1", "test", ""))
        //    {
        //        await ftp.ConnectAsync(token);

        //        // download a file and ensure the local directory is created
        //        await ftp.DownloadFileAsync(@"D:\Github\FluentFTP\README.md", "/public_html/temp/README.md");

        //        // download a file and ensure the local directory is created, verify the file after download
        //        await ftp.DownloadFileAsync(@"D:\Github\FluentFTP\README.md", "/public_html/temp/README.md", FtpLocalExists.Overwrite, FtpVerify.Retry);

        //    }
        //}

    }


    public partial class FluentFtp
    {
        public FluentFtp()
        {
            FtpClient client = new FtpClient();

            client.Host = "127.0.0.1";
            client.Port = 21;
            // 資格情報の設定
            client.Credentials = new NetworkCredential("test", "");
            // 要求の完了後に接続を閉じる
            client.SocketKeepAlive = false;
            // Explicit設定
            client.EncryptionMode = FtpEncryptionMode.Explicit;
            // プロトコルはTls
            client.SslProtocols = SslProtocols.Tls;
            // 接続タイムアウトを5秒に設定
            client.ConnectTimeout = 5000;
            // 証明書の内容を確認しない
            client.ValidateCertificate += new FtpSslValidation(OnValidateCertificate);

            try
            {
                // 接続
                client.Connect();
                // ファイルのアップロード
                client.UploadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto.csv", "00002_bto.csv");

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                // 切断
                client.Disconnect();
                // 解放
                client.Dispose();
            }
        }

        private void OnValidateCertificate(FtpClient control, FtpSslValidationEventArgs e)
        {
            // 証明書の内容を確認しない
            e.Accept = true;
        }
    }
}