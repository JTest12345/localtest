using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Security.Authentication;
using System.Threading;

using FluentFTP;

/*
 * NuGetでFluentFTPパッケージをインストールして使う
 */

namespace Ftp {

    static class FTPclient {

        /// <summary>
        /// 同期処理でFTPアップロードする
        /// </summary>
        /// <param name="ip">サーバーのIPアドレス文字列</param>
        /// <param name="userName">ユーザーネーム</param>
        /// <param name="password">パスワード</param>
        /// <param name="filepath">送信するファイルパス</param>
        public static void UploadFile(string server_ip, string userName, string password, string send_filepath) {

            string filename = Path.GetFileName(send_filepath);

            using (var ftp = new FtpClient(server_ip, userName, password)) {
                ftp.Connect();

                // upload a file to an existing FTP directory
                //ftp.UploadFile(send_filepath, $"/{filename}");

                //upload a file and ensure the FTP directory is created on the server
                ftp.UploadFile(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true);

                //upload a file and ensure the FTP directory is created on the server, verify the file after upload
                //ftp.UploadFile(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            }
        }

        /// <summary>
        /// 非同期処理でFTPアップロードする
        /// </summary>
        /// <param name="server_ip">サーバーのIPアドレス文字列</param>
        /// <param name="userName">ユーザーネーム</param>
        /// <param name="password">パスワード</param>
        /// <param name="send_filepath">送信するファイルパス</param>
        /// <returns></returns>
        public static async Task UploadFileAsync(string server_ip, string userName, string password, string send_filepath) {
            var token = new CancellationToken();
            string filename = Path.GetFileName(send_filepath);

            using (var ftp = new FtpClient(server_ip, userName, password)) {
                await ftp.ConnectAsync(token);

                // upload a file to an existing FTP directory
                //await ftp.UploadFileAsync(send_filepath, $"/{filename}");

                // upload a file and ensure the FTP directory is created on the server
                await ftp.UploadFileAsync(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true);

                // upload a file and ensure the FTP directory is created on the server, verify the file after upload
                //await ftp.UploadFileAsync(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

            }
        }
    }
}
