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

namespace KodaClassLibrary {

    public static class FTPclient {

        private const string koda_host = "koda.cej.citizen.co.jp";
        private const string kodaftp_pass = "id-12345";
        private const string kodaftp_user_particle = "particle|koda-ftp";


        /// <summary>
        /// 同期処理でFTPアップロードする
        /// </summary>
        /// <param name="host">ホスト名 or サーバーのIPアドレス文字列</param>
        /// <param name="userName">ユーザーネーム</param>
        /// <param name="password">パスワード</param>
        /// <param name="filepath">送信するファイルパス</param>
        public static string UploadFile(string host, string userName, string password, string send_filepath) {

            string filename = Path.GetFileName(send_filepath);

            using (var ftp = new FtpClient(host, userName, password)) {
                ftp.Connect();

                // upload a file to an existing FTP directory
                //ftp.UploadFile(send_filepath, $"/{filename}");

                //upload a file and ensure the FTP directory is created on the server
                var status = ftp.UploadFile(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true);

                //upload a file and ensure the FTP directory is created on the server, verify the file after upload
                //ftp.UploadFile(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

                return status.ToString();
            }

        }

        /// <summary>
        /// 非同期処理でFTPアップロードする
        /// </summary>
        /// <param name="host">ホスト名 or サーバーのIPアドレス文字列</param>
        /// <param name="userName">ユーザーネーム</param>
        /// <param name="password">パスワード</param>
        /// <param name="send_filepath">送信するファイルパス</param>
        /// <returns></returns>
        public static async Task<string> UploadFileAsync(string host, string userName, string password, string send_filepath) {
            var token = new CancellationToken();
            string filename = Path.GetFileName(send_filepath);

            using (var ftp = new FtpClient(host, userName, password)) {
                await ftp.ConnectAsync(token);

                // upload a file to an existing FTP directory
                //await ftp.UploadFileAsync(send_filepath, $"/{filename}");

                // upload a file and ensure the FTP directory is created on the server
                var status = await ftp.UploadFileAsync(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true);

                // upload a file and ensure the FTP directory is created on the server, verify the file after upload
                //await ftp.UploadFileAsync(send_filepath, $"/{filename}", FtpRemoteExists.Overwrite, true, FtpVerify.Retry);

                return status.ToString();
            }
        }

        /// <summary>
        /// KodaSystemへファイルを同期処理でFTP送信します。
        /// <para>送信成功したファイルは削除されます。</para>
        /// <para>エラーが発生しても継続して処理します。</para>
        /// </summary>
        /// <param name="path_list">FTP送信するファイルパス配列</param>
        /// <returns>エラー発生辞書->key:エラー発生ファイルパス value:ファイル処理中に発生した例外</returns>
        public static Dictionary<string, Exception> FtpSend_ParticleFile_toKoda(List<string> path_list) {

            var error_dic = new Dictionary<string, Exception>();

            foreach (string path in path_list) {

                try {
                    //ファイルをFTP送信
                    string status = FTPclient.UploadFile(koda_host, kodaftp_user_particle, kodaftp_pass, path);

                    if (status == "Success") {

                        //jsonファイル削除
                        int cnt = 0;
                        while (true) {
                            try {
                                File.Delete(path);
                                break;
                            }
                            catch (IOException) {
                                Thread.Sleep(1000);//早すぎると失敗するかもなので
                                cnt += 1;
                                if (cnt > 10) { throw; }
                            }
                        }
                    }
                }
                catch (Exception ex) {
                    error_dic.Add(path, ex);
                    continue;
                }

            }

            return error_dic;
        }








    }
}
