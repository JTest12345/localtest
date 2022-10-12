using System;
using System.Collections.Generic;
using System.Net;
using System.IO;
namespace FTPConnector
{
    class Program
    {
        // FTPに接続するユーザID
        private string id = "test";
        // FTPに接続するパスワード
        private string pwd = "";
        public Program()
        {
            // ローカルのファイルをFTPサーバにアップロードする関数を呼び出す。
            // d:\ftptest\uploadフォルダからFTPサーバにアップロードする。
            // UploadFileList("ftp://127.0.0.1", @"C:\temp\ftp\client");
            // FTPサーバからファイルをダウンロードする関数を呼び出す。
            // FTPサーバからd:\ftptest\downloadにダウンロードする。
            DownloadFileList("ftp://127.0.0.1", @"C:\temp\ftp\client");
        }
        // FTPサーバに接続関数
        private FtpWebResponse Connect(String url, string method, Action<FtpWebRequest> action = null)
        {
            // WebRequestクラスを利用してFTPサーバに接続する。(FtpWebRequestに変換)
            var request = WebRequest.Create(url) as FtpWebRequest;
            // Binaryタイプで使う。
            request.UseBinary = false;
            // FTPメソッド設定(下記に別途で説明)
            request.Method = method;
            // ログイン認証
            request.Credentials = new NetworkCredential(id, pwd);
            // callback関数を呼び出す。
            if (action != null)
            {
                action(request);
            }
            // FTPに接続してWebResponseクラスを取得する。(FtpWebResponseに変換)
            return request.GetResponse() as FtpWebResponse;
        }
        // アップロード関数
        private void UploadFileList(String url, string source)
        {
            // アップロードするパスのタイプをチェックする。
            var attr = File.GetAttributes(source);
            // もし、ディレクトリなら
            if ((attr & FileAttributes.Directory) == FileAttributes.Directory)
            {
                // ディレクトリ情報を取得する。
                DirectoryInfo dir = new DirectoryInfo(source);
                // ディレクトリ中のファイルリストを取得する。
                foreach (var item in dir.GetFiles())
                {
                    // ファイルをアップロードする。(再帰的な呼び出す)
                    UploadFileList(url + "/" + item.Name, item.FullName);
                }
                // ディレクトリ中の派生ディレクトリリストを取得する。
                foreach (var item in dir.GetDirectories())
                {
                    try
                    {
                        // FTPサーバにディレクトリを生成する。
                        Connect(url + "/" + item.Name, WebRequestMethods.Ftp.MakeDirectory).Close();
                    }
                    catch (WebException)
                    {
                        // FTPサーバぶ同じ名のディレクトリが存在すればエラーが発生する。
                    }
                    // ディレクトリをアップロードする(再帰的な呼び出す)
                    UploadFileList(url + "/" + item.Name, item.FullName);
                }
            }
            else
            {
                // ディレクトリではなくファイルの場合、ファイルのストリームを取得する。
                using (var fs = File.OpenRead(source))
                {
                    // ファイルをアップロードする。
                    Connect(url, WebRequestMethods.Ftp.UploadFile, (req) =>
                    {
                        // ファイルのサイズを設定
                        req.ContentLength = fs.Length;
                        // GetResponse()が呼び出す前にrequestストリームにファイルbinaryを格納する。
                        using (var stream = req.GetRequestStream())
                        {
                            // アップロードする。
                            fs.CopyTo(stream);
                        }
                    }).Close();
                    // responseオブジェクトのリソースを返却
                }
            }
        }
        // ダウンロードする関数
        private void DownloadFileList(string url, string target)
        {
            // fileリストバッファ
            var list = new List<String>();
            // FTPサーバに接続してファイルとディレクトリストを取得する。
            using (var res = Connect(url, WebRequestMethods.Ftp.ListDirectory))
            {
                // FTPサーバのストリームを取得(リストがテキストみたいなバイナリだ。)
                using (var stream = res.GetResponseStream())
                {
                    // データがバイナリなどでバイナリリーダを取得する。
                    using (var rd = new StreamReader(stream))
                    {
                        // 無限ループでバイナリリーダがNULLなら分岐で止まる。
                        while (true)
                        {
                            // binary結果で改行(\r\n)の区分でファイルリストを取得する。
                            string buf = rd.ReadLine();
                            // nullならリスト検索が終了
                            if (string.IsNullOrWhiteSpace(buf))
                            {
                                break;
                            }
                            // ファイルやディレクトリ情報をバッファに格納
                            list.Add(buf);
                        }
                    }
                }
            }
            // ディレクトリとファイルリストを繰り返す。
            foreach (var item in list)
            {
                try
                {
                    // ファイルをダウンロードする。
                    using (var res = Connect(url + "/" + item, WebRequestMethods.Ftp.DownloadFile))
                    {
                        // ストリームを取得する。
                        using (var stream = res.GetResponseStream())
                        {
                            // streamでファイルを作成する。
                            using (var fs = File.Create(target + "\\" + item))
                            {
                                // ファイル作成する。
                                stream.CopyTo(fs);
                            }
                        }
                    }
                }
                catch (WebException)
                {
                    // ファイルならダウンロードができるけどディレクトリならダウンロードするものではないのでエラーが発生する。
                    // ローカルディレクトリ生成
                    Directory.CreateDirectory(target + "\\" + item);
                    // ディレクトリなら再帰的な方法でファイルリストを探索する。
                    DownloadFileList(url + "/" + item, target + "\\" + item);
                }
            }
        }
        // 実行関数
        static void Main(string[] args)
        {
            // プログラムを実行
            new Program();
            // いずれかのキーを押下するとプログラム終了
            Console.WriteLine("Press any key...");
            Console.ReadKey();
        }
    }
}
