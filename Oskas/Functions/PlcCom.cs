using System;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Oskas
{
    public class PlcCom
    {
        public bool CheckSocketConnect(string ipaddress, int portno, ref string globalmsg)
        {
            try
            {
                Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IAsyncResult result = socket.BeginConnect(ipaddress, portno, null, null);
                // タイムアウトを3秒に設定
                bool success = result.AsyncWaitHandle.WaitOne(3000, true);

                if (socket.Connected)
                {
                    socket.EndConnect(result);
                    return true;
                }
                else
                {
                    // NOTE, MUST CLOSE THE SOCKET
                    socket.Close();
                    globalmsg += "PLCに接続できません。接続条件を確認してください。";
                    return false;
                }
            }
            catch(Exception ex)
            {
                globalmsg += ex.Message;
                return false;
            }

        }


        public bool OpenPlcPortTest(string ipaddress, int portno, ref string globalmsg)
        {
            try
            {
                //上位リンクのポートをオープン
                System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipaddress, portno);
                System.Net.Sockets.NetworkStream ns = tcp.GetStream();
                ns.ReadTimeout = 1000;
                ns.WriteTimeout = 1000;
                //上位リンクのポートをクローズ
                ns.Close();
                tcp.Close();
                globalmsg += "オープンクローズは正常に完了しました";
                return true;
            }
            catch (Exception ex)
            {
                globalmsg += ex.Message;
                return false;
            }
        }


        public bool DeviceRead(string ipOrHost, int port, string devtyp, int devNo, ref string globalmsg)
        {
            try
            {
                //var sw = new System.Diagnostics.Stopwatch();
                string sendMsg = "RD " + devtyp + devNo.ToString() + ".U\r";

                System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
                System.Net.Sockets.NetworkStream ns = tcp.GetStream();
                ns.ReadTimeout = 3000;
                ns.WriteTimeout = 3000;

                //sw.Start();
                Encoding enc = Encoding.ASCII;
                byte[] sendBytes = enc.GetBytes(sendMsg + '\r');
                ns.Write(sendBytes, 0, sendBytes.Length);
                //MessageBox.Show(sendMsg);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize = 0;
                do
                {
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    if (resSize == 0)
                    {
                        globalmsg += "PLCとの接続が切断しました";
                        return false;
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

                string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                ms.Close();
                //sw.Stop();
                //TimeSpan ts = sw.Elapsed;
                //MessageBox.Show(ts.Seconds + "s " + ts.Milliseconds + "ms"); ;

                resMsg = resMsg.TrimEnd('\n');
                globalmsg += "読込結果：" + resMsg;

                ns.Close();
                tcp.Close();

                return true;

            }
            catch(Exception ex)
            {
                globalmsg += ex.Message;
                return false;
            }

 
        }

        public bool DeviceWrite(string ipOrHost, int port, string devtyp, int DmNo, int data, ref string globalmsg)
        {
            try
            {
                //var sw = new System.Diagnostics.Stopwatch();
                //string sendMsg = "WR DM5000.U 123\r";
                string sendMsg = "WR " + devtyp + DmNo.ToString() + ".U " + data.ToString() + "\r";

                System.Net.Sockets.TcpClient tcp = new System.Net.Sockets.TcpClient(ipOrHost, port);
                System.Net.Sockets.NetworkStream ns = tcp.GetStream();
                ns.ReadTimeout = 3000;
                ns.WriteTimeout = 3000;

                //sw.Start();
                Encoding enc = Encoding.ASCII;
                byte[] sendBytes = enc.GetBytes(sendMsg + '\r');
                ns.Write(sendBytes, 0, sendBytes.Length);

                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize = 0;
                do
                {
                    resSize = ns.Read(resBytes, 0, resBytes.Length);

                    if (resSize == 0)
                    {
                        globalmsg += "PLCとの接続が切断しました";
                        return false;
                    }

                    ms.Write(resBytes, 0, resSize);

                } while (ns.DataAvailable || resBytes[resSize - 1] != '\n');

                string resMsg = enc.GetString(ms.GetBuffer(), 0, (int)ms.Length);
                ms.Close();
                //sw.Stop();
                //TimeSpan ts = sw.Elapsed;
                //MessageBox.Show(ts.Seconds + "s " + ts.Milliseconds + "ms"); ;

                ns.Close();
                tcp.Close();

                if (resMsg == "OK\r\n")
                {
                    resMsg = resMsg.TrimEnd('\n');
                    globalmsg += "デバイスに書き込み完了しました";
                    return true;
                }
                else
                {
                    globalmsg += "PLCの戻り値に異常があります";
                    return false;
                }

            }
            catch (Exception ex)
            {
                globalmsg += ex.Message;
                return false;
            }

        }
    }
}
