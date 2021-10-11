using System;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;
namespace SocketTest
{
    class SocketServerSample
    {
        // 実行関数
        static void SocketServer(string[] args)
        {
            // serverソケットを生成する。
            using (var server = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // ipはローカルでポートは9999でlisten待機する。
                server.Bind(new IPEndPoint(IPAddress.Any, 9999));
                server.Listen(20);
                // コンソールに出力
                Console.WriteLine("Server Start... Listen port 9999...");
                try
                {
                    while (true)
                    {
                        // 多重接続を許すためにThreadpoolを利用してマルチスレッド環境を作る。
                        ThreadPool.QueueUserWorkItem(c =>
                        {
                            // クライアントソケット
                            Socket client = (Socket)c;
                            try
                            {
                                // 無限ループでメッセージを待機する。
                                while (true)
                                {
                                    // 始めにデータ長さを受け取る4byteを宣言する。
                                    var data = new byte[4];
                                    // pythonでlittleエンディアンで値を受信する。bigエンディアンとlittleエンディアンは配列の順番が逆なのでreverseする。
                                    client.Receive(data, 4, SocketFlags.None);
                                    Array.Reverse(data);
                                    // データの長さでbyte配列を生成する。
                                    data = new byte[BitConverter.ToInt32(data, 0)];
                                    // データを受信する。
                                    client.Receive(data, data.Length, SocketFlags.None);

                                    // byteをUTF8エンコードでstringタイプで変換する。
                                    var msg = Encoding.UTF8.GetString(data);
                                    // データをコンソールに出力する。
                                    Console.WriteLine(msg);
                                    // メッセージでechoを文字に付ける。
                                    msg = "C# server echo : " + msg;
                                    // データをUTF8エンコードでbyte形式で変換する。
                                    data = Encoding.UTF8.GetBytes(msg);
                                    // データの長さをクライアントで転送する。
                                    client.Send(BitConverter.GetBytes(data.Length));
                                    // データを転送する。
                                    client.Send(data, data.Length, SocketFlags.None);
                                }
                            }
                            catch (Exception)
                            {
                                // Exceptionが発生すれば(予期しない接続終了)client socketを閉める。
                                client.Close();
                            }
                            // serverでclientが接続すればThreadPoolでThreadが生成されました。
                        }, server.Accept());
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
            // いずれかのキーを押下すると終了。
            Console.WriteLine("Press any key...");
            Console.ReadLine();
        }
    }
}