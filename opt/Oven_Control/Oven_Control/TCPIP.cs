using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;


namespace TcpIp {

    /// <summary>
    /// Clientクラスをインスタンス化して使う
    /// <para>コンストラクターはIPEndPoint</para>
    /// </summary>
    public class Client : TcpClient {
        /// <summary>
        /// TCP/IP接続先のIPEndPoint
        /// </summary>
        public IPEndPoint Local_endpoint { get; private set; }
        /// <summary>
        /// TCP/IP送受信メッセージの文字コード
        /// </summary>
        public readonly Encoding enc = Encoding.UTF8;

        //コンストラクター
        public Client(IPEndPoint localEP) {
            Local_endpoint = localEP;
        }

        /// <summary>
        /// 相手に接続してメッセージ送信
        /// </summary>
        /// <param name="msg"></param>
        public void Send(string msg) {
            NetworkStream stm = null;

            try {
                int timeout = 3000;//接続タイムアウト

                //接続
                Task task = this.ConnectAsync(Local_endpoint.Address, Local_endpoint.Port);

                //標準windows10 OSでは接続タイムアウトは21秒になるので
                //taskの時間で接続タイムアウト3秒にしている
                if (!task.Wait(timeout)) {
                    throw new SocketException(10060);
                }

                //ネットワークストリーム取得
                stm = this.GetStream();
                stm.ReadTimeout = 2000;
                stm.WriteTimeout = 2000;

                //メッセージ送信
                byte[] s_dat = enc.GetBytes(msg);
                stm.Write(s_dat, 0, s_dat.Length);
            }
            catch {
                throw;
            }
            finally {
                //ストリームクローズ
                if (stm != null) { stm.Close(0); }
                //ソケットクローズ
                this.Close();
            }
        }


        /// <summary>
        /// 相手に接続してメッセージ送信＋受け取った応答を返す
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public string Send_receive(string msg) {
            NetworkStream stm = null;

            try {
                int timeout = 3000;//接続タイムアウト

                //接続
                Task task = this.ConnectAsync(Local_endpoint.Address, Local_endpoint.Port);

                //標準windows10 OSでは接続タイムアウトは21秒になるので
                //taskの時間で接続タイムアウト3秒にしている
                if (!task.Wait(timeout)) {
                    throw new SocketException(10060);
                }

                //ネットワークストリーム取得
                stm = this.GetStream();
                stm.ReadTimeout = 5000;
                stm.WriteTimeout = 5000;

                //メッセージ送信
                byte[] s_dat = enc.GetBytes(msg);
                stm.Write(s_dat, 0, s_dat.Length);

                //受信メッセージ格納変数
                byte[] r_dat = new byte[1024];
                int l_dat;
                string r_msg = "";

                do {
                    Array.Clear(r_dat, 0, 1024);

                    //メッセージ受信(ここでTimeoutまで待つ)
                    l_dat = stm.Read(r_dat, 0, r_dat.Length);

                    if (l_dat > 0) {
                        //1byte以上なら文字追加
                        r_msg += enc.GetString(r_dat, 0, l_dat);
                    }
                    else {
                        //0byteの時は切断合図を受信した
                        break;
                    }

                } while (stm.DataAvailable);//取得可能データがある限りループ


                return r_msg;
            }
            catch {
                throw;
            }
            finally {
                //ストリームクローズ
                if (stm != null) { stm.Close(0); }
                //ソケットクローズ
                this.Close();
            }
        }
    }
}
