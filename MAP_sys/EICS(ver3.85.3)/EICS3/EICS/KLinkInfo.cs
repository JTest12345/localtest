using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Linq;

namespace EICS
{
    public class KLinkInfo
    {
        public const int BIT_ON = 1;
        public const int BIT_OFF = 0;

        /// <summary>
        /// KISSの起動確認(音あり)
        /// </summary>
        /// <returns></returns>
        public static bool CheckKISS()
        {
            bool flg = true;
            //string sExeName = @"C:\Program Files\KAIJO\KISS\Kiss.exe";

            if (Process.GetProcessesByName("Kiss").Length == 0)
            {
                flg = false;
            }
            return flg;
        }

        /// <summary>
        /// BlackJumboDogの起動確認(音あり)
        /// </summary>
        /// <returns></returns>
        public static bool CheckBlackJumboDog()
        {
            bool flg = true;
            //string sExeName = @"C:\Program Files\SapporoWorks\BlackJumboDog\BlackJumboDog.exe";

            if (Process.GetProcessesByName("BlackJumboDog").Length == 0)
            {
                flg = false;
            }
            return flg;
        }

        /// <summary>
        /// EICSの多重起動確認
        /// </summary>
        /// <returns></returns>
        public static bool CheckEICS()
        {
            bool flg = false;

            //アプリの多重起動を防ぐ 
            // 同じ実行ファイル名のプロセスは起動しない
            // 注) パス違いでも「同名」実行ファイルは起動しない！
            if (Process.GetProcessesByName(
                    Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                flg = true;
            }
            return flg;
        }

		public static bool CheckLENS()
		{
			bool flg = false;

			if (Process.GetProcessesByName("NAMI").Length >= 1 || Process.GetProcessesByName("LENS2").Length >= 1)
			{
				flg = true;
			}

			return flg;
		}

        //------------------------------------------------------------------------
        // 機能	:指定デバイス 連続同値書込み SET関数
        //       書込み出来なかった場合は、"Error"を返す。
        // 引数	:1)IP Address
        //	     2)Port No
        //	     3)アドレス(EM40050,LR10000etc)
        //       4)連続して書き込む値
        //       5)3)から連続して4)を書き込むの個数
        //       6)サフィックス
        // CMD  :"WRS| |デバイス種別|デバイス番号|データ形式| |個数| |書込み文字"
        // CMD例: WRS EM40050.U 3 0 0 0
        //------------------------------------------------------------------------
        public string KLINK_SetKV_WRS(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, int nset, int ncount, string suffix)
        {
            return KLINK_SetKV_WRS(ref tcp, ref ns, host, port, adr, nset, ncount, suffix, 0);
        }

        private string KLINK_SetKV_WRS(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, int nset, int ncount, string suffix, int retryCT)
        {
#if DEBUG
			return "";
#else
            //System.Net.Sockets.TcpClient tcp = null;
            if (retryCT > 5)
            {
                return "Error";
            }

            if (OpenTcp(ref tcp, host, port) != true)
            {
				AlertLog alertLog = AlertLog.GetInstance();

				string sMessage = string.Format("接続出来ませんでした。設備/ｼｰｹﾝｻ側の電源が付いているか、ﾈｯﾄﾜｰｸ接続可能な状態かを確認下さい。接続先:{0} / ﾎﾟｰﾄ:{1}", host, port);
				alertLog.logMessageQue.Enqueue(sMessage);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
				F01_MachineWatch.spMachine.Play();
                return "Error";
            }
            string sset = "";//書き込むデータ 0を4つ書き込む場合：0 0 0 0 
            for (int i = 0; i < ncount; i++)
            {
                sset = sset + " " + nset.ToString();
            }

            string sendMsg = "WRS " + adr + suffix + " " + ncount + sset + "\r";
            //System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            ns = tcp.GetStream();

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
            ns.Write(sendBytes, 0, sendBytes.Length);

            //サーバーから送られたデータを受信する
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[10000];
            int resSize;
            try
            {
                DateTime sendDt = DateTime.Now;
                TimeSpan ts = DateTime.Now - sendDt;

                while (ns.DataAvailable == false)
                {
                    Thread.Sleep(10);

                    ts = DateTime.Now - sendDt;
                    if (ts.TotalMilliseconds > 4000)
                    {
                        throw new ApplicationException(
                                string.Format("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無くタイムアウト(4000ms)しました。(WRS)"));
                    }
                }

                do
                {
                    //データの一部を受信する
                    resSize = ns.Read(resBytes, 0, resBytes.Length);//☆★
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        F01_MachineWatch.spMachine.Play();
                        string sMessage = "Readが0を返した時はサーバーが切断したと判断";
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

                        //tcp.Close(); ns.Close();
                        retryCT++;
                        return KLINK_SetKV_WRS(ref tcp, ref ns, host, port, adr, nset, ncount, suffix, retryCT);
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);

            }
            catch (IOException ex)//基になっている Socket が閉じています。
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                //転送接続からデータを読み取れません:既存の接続はリモートﾎｽﾄに強制的に切断されました。
                return "Error";
            }
            catch (ObjectDisposedException ex)
            {
                
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                return "Error";
            }
            //終了処理---------------------------------------------------------------------------------------
            finally
            {
                if (ms != null) { ms.Close(); }
                //if (tcp != null) { tcp.Close(); }
                //if (ns != null) { ns.Close(); }
            }

            string resMsg = System.Text.Encoding.UTF8.GetString(ms.ToArray());

            return resMsg;
#endif
        }

		public static string SetKV_WRS(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, int nset, int ncount, string suffix)
		{
			return SetKV_WRS(ref tcp, ref ns, host, port, adr, nset, ncount, suffix, 0);
		}

		private static string SetKV_WRS(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, int nset, int ncount, string suffix, int retryCT)
		{
			if (retryCT > 5)
			{
				return "Error";
			}

			if (KLinkInfo.Open(ref tcp, host, port) != true)
			{
				AlertLog alertLog = AlertLog.GetInstance();

				string sMessage = string.Format("接続出来ませんでした。設備/ｼｰｹﾝｻ側の電源が付いているか、ﾈｯﾄﾜｰｸ接続可能な状態かを確認下さい。接続先:{0} / ﾎﾟｰﾄ:{1}", host, port);
				alertLog.logMessageQue.Enqueue(sMessage);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
				F01_MachineWatch.spMachine.Play();
				return "Error";
			}
			string sset = "";//書き込むデータ 0を4つ書き込む場合：0 0 0 0 
			for (int i = 0; i < ncount; i++)
			{
				sset = sset + " " + nset.ToString();
			}

			string sendMsg = "WRS " + adr + suffix + " " + ncount + sset + "\r";
			//System.Net.Sockets.NetworkStream ns = tcp.GetStream();
			ns = tcp.GetStream();

			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
			ns.Write(sendBytes, 0, sendBytes.Length);

			//サーバーから送られたデータを受信する
			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[10000];
			int resSize;
			try
			{
				do
				{
					//データの一部を受信する
					resSize = ns.Read(resBytes, 0, resBytes.Length);//☆★
					//Readが0を返した時はサーバーが切断したと判断
					if (resSize == 0)
					{
						F01_MachineWatch.spMachine.Play();
						string sMessage = "Readが0を返した時はサーバーが切断したと判断";
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);

						//tcp.Close(); ns.Close();
						retryCT++;
						return SetKV_WRS(ref tcp, ref ns, host, port, adr, nset, ncount, suffix, retryCT);
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

			}
			catch (IOException ex)//基になっている Socket が閉じています。
			{
				F01_MachineWatch.spMachine.Play();
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
				//転送接続からデータを読み取れません:既存の接続はリモートﾎｽﾄに強制的に切断されました。
				return "Error";
			}
			catch (ObjectDisposedException ex)
			{

				F01_MachineWatch.spMachine.Play();
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
				return "Error";
			}
			//終了処理---------------------------------------------------------------------------------------
			finally
			{
				if (ms != null) { ms.Close(); }
				//if (tcp != null) { tcp.Close(); }
				//if (ns != null) { ns.Close(); }
			}

			string resMsg = System.Text.Encoding.UTF8.GetString(ms.ToArray());

			return resMsg;
		}

        //------------------------------------------------------------------------
        // 機能	    :指定デバイス データ取得 GET関数
        //		     指定したアドレスからデータを取得して返す。
        //           取得出来なかった場合は、"Error"を返す。
        // 引数	    :1)IP Address
        //           2)Port No
        //	         3)アドレス(EM40050,LR10000etc)
        //           4)サフィックス
        // CMD      :"RD| |デバイス種別|デバイス番号|データ形式"
        // CMD例    : RD EM40050.U
        //------------------------------------------------------------------------
        public string KLINK_GetKV_RD(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, string suffix)
        {
/*#if DEBUG
			return "";
#else*/
			StackFrame callerFrame = new StackFrame(1);
			//メソッド名
			string methodName = callerFrame.GetMethod().Name;
			//クラス名
			string className = callerFrame.GetMethod().ReflectedType.FullName;


            if (OpenTcp(ref tcp, host, port) != true)
            {
				AlertLog alertLog = AlertLog.GetInstance();

				string sMessage = string.Format("接続出来ませんでした。設備/ｼｰｹﾝｻ側の電源が付いているか、ﾈｯﾄﾜｰｸ接続可能な状態かを確認下さい。接続先:{0} / ﾎﾟｰﾄ:{1}", host, port);
				alertLog.logMessageQue.Enqueue(sMessage);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
				F01_MachineWatch.sp.Play();
                return "Error";
            }

            string sendMsg = "RD " + adr + suffix + "\r";//.U=10進数16bit符号なし
            //System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            ns = tcp.GetStream();

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
            ns.Write(sendBytes, 0, sendBytes.Length);

            //サーバーから送られたデータを受信する
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize;
            try
            {
                DateTime sendDt = DateTime.Now;
                TimeSpan ts = DateTime.Now - sendDt;

                while (ns.DataAvailable == false)
                {
                    Thread.Sleep(10);

                    ts = DateTime.Now - sendDt;
                    if (ts.TotalMilliseconds > 4000)
                    {
                        throw new ApplicationException(
                                string.Format("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無くタイムアウト(4000ms)しました。(RD)"));
                    }
                }

                do
                {
                    resSize = ns.Read(resBytes, 0, resBytes.Length);//☆★
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        F01_MachineWatch.spMachine.Play();
                        string sMessage = "Readが0を返した時はサーバーが切断したと判断";
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        return "Error";
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);
            }
            catch (IOException ex)//基になっている Socket が閉じています。
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);

                //転送接続からデータを読み取れません:既存の接続はリモートﾎｽﾄに強制的に切断されました。
                return "Error";
            }
            catch (ObjectDisposedException ex)
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);

                return "Error";
            }
            //終了処理---------------------------------------------------------------------------------------
            finally
            {
                if (ms != null) { ms.Close(); }
                //if (tcp != null) { tcp.Close(); }
                //if (ns != null) { ns.Close(); }
            }
            string resMsg = System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();

			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, resMsg);

            return resMsg;
//#endif
        }

        //------------------------------------------------------------------------
        // 機能	:指定デバイス 連続データ取得 GET関数
        //		 指定したアドレスから、指定個数、データを取得して返す。
        //       取得出来なかった場合は、"Error"を返す。
        // 引数	:1)IP Address
        //	     2)Port No
        //       3)アドレス(EM40050,LR10000etc)
        //       4)個数
        //       5)サフィックス
        // CMD  :"RDS| |デバイス種別|デバイス番号|データ形式"
        // CMD例: RD EM40050.U
        //------------------------------------------------------------------------
        public string KLINK_GetKV_RDS(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, int cnt, string suffix)
        {
/*#if DEBUG
			return "";
#else*/

            if (OpenTcp(ref tcp, host, port) != true)
            {
                F01_MachineWatch.spMachine.Play();
                //MessageBox.Show("接続出来ませんでした。シーケンサ側の電源が付いている事を確認下さい。");
                return "Error";
            }

            string sendMsg = "RDS " + adr + suffix + " " + cnt + "\r";
            ns = tcp.GetStream();

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
            ns.Write(sendBytes, 0, sendBytes.Length);

            //サーバーから送られたデータを受信する
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[10000];
            int resSize;
            try
            {
                DateTime sendDt = DateTime.Now;
                TimeSpan ts = DateTime.Now - sendDt;

                while (ns.DataAvailable == false)
                {
                    Thread.Sleep(10);

                    ts = DateTime.Now - sendDt;
                    if (ts.TotalMilliseconds > 4000)
                    {
                        throw new ApplicationException(
                                string.Format("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無くタイムアウト(4000ms)しました。(RDS)"));
                    }
                }

                do
                {
                    resSize = ns.Read(resBytes, 0, resBytes.Length);//☆★
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        F01_MachineWatch.spMachine.Play();
                        string sMessage = "Readが0を返した時はサーバーが切断したと判断";
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        return "Error";
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);
            }
            catch (IOException ex)//基になっている Socket が閉じています。
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                //転送接続からデータを読み取れません:既存の接続はリモートﾎｽﾄに強制的に切断されました。
                return "Error";
            }
            catch (ObjectDisposedException ex)
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                return "Error";
            }
            //終了処理---------------------------------------------------------------------------------------
            finally
            {
                if (ms != null) { ms.Close(); }
                //if (tcp != null) { tcp.Close(); }
                //if (ns != null) { ns.Close(); }
            }
            string resMsg = System.Text.Encoding.UTF8.GetString(ms.ToArray());
            return resMsg;
//#endif
        }


        //TYPE文字列を入れると上位リンクコマンドWRSS用に文字列を生成する
        ///////////////////////////////////////////////////////////////////
        //引数
        //例：「NSSW206A1」の場合
        ///////////////////////////////////////////////////////////////////
        //[0]NS
        //   N:上位8bit→ASCIIで78→8bitshiftで19885
        //   S:下位8bit→ASCIIで83                  →20051
        //[1]SW                                     →21335
        //[2]20                                     →12848
        //[3]6A                                     →13889
        //[4]1                                      →12544 
        ///////////////////////////////////////////////////////////////////
        //戻り値
        //「20051 21335 12848 13889 12544 」最後にスペースは必要。
        ///////////////////////////////////////////////////////////////////
        public string GetSendMsg_WRSS(string sType)
        {
#if DEBUG
			return "";
#else
            int num = (sType.Length / 2) + (sType.Length % 2);//上位・下位8bitに一文字、計2文字でnum=1
            int[] nsend = new int[num];

            byte[] bwork = System.Text.Encoding.UTF8.GetBytes(sType);
            int wsend = 0, wsend2 = 0;//sendに入れる前のwork領域
            int nCnt = 0, nCnt2 = 0;
            string swork = "";

            for (int i = 0; i < sType.Length; i++)
            {
                wsend = Convert.ToInt32(bwork[i]);
                //上位8bitの場合
                if (i % 2 == 0)
                {
                    wsend2 = wsend << 8;//8bit Shift
                }
                else//下位8bitの場合
                {
                    wsend2 = wsend2 + wsend;
                }
                nCnt += 1;

                if (nCnt % 2 == 0 || i == sType.Length - 1)
                {
                    nsend[nCnt2] = wsend2;
                    nCnt2 += 1;
                }
            }

            for (int i = 0; i < num; i++)
            {
                swork = swork + nsend[i] + " ";
            }
            //swork = swork + " ";

            return swork;
#endif
        }

        public string BytesStringToAscii(string bytesString)
        {
            //不要文字列削除
            string swork = "";
            swork = bytesString.Replace(" ", "");
            swork = swork.Replace("\r\n", "");
            bytesString = swork;

            List<byte> bytes = new List<byte>();
            for (int i = 0; i < bytesString.Length - 1; i = i + 2)
            {
                string word = bytesString.Substring(i, 2);

                byte b;
                if (byte.TryParse(word, System.Globalization.NumberStyles.HexNumber, null, out b) == true)
                {
                    bytes.Add(b);
                }
            }

            String retv = Encoding.ASCII.GetString(bytes.ToArray());
            return retv;
        }

        //------------------------------------------------------------------------
        // 機能	:指定デバイス 連続ビット書込み SET関数
        //       書込み出来なかった場合は、"Error"を返す。
        // 引数	:1)IP Address
        //	     2)Port No
        //	     3)アドレス(EM40050,LR10000etc)
        //       4)連続して書き込む値
        //       5)サフィックス
        // CMD  :"WRS| |デバイス種別|デバイス番号|データ形式| |個数| |書込み文字"
        // CMD例: WRS EM40050.U 3 0 0 0
        //------------------------------------------------------------------------
        public string KLINK_SetKV_WRSS(ref TcpClient tcp, ref NetworkStream ns, string host, int port, string adr, string sset, string suffix)
        {
#if DEBUG
			return "";
#else
            //System.Net.Sockets.TcpClient tcp = null;

            if (OpenTcp(ref tcp, host, port) != true)
            {
				AlertLog alertLog = AlertLog.GetInstance();

				string sMessage = string.Format("接続出来ませんでした。設備/ｼｰｹﾝｻ側の電源が付いているか、ﾈｯﾄﾜｰｸ接続可能な状態かを確認下さい。接続先:{0} / ﾎﾟｰﾄ:{1}", host, port);
				alertLog.logMessageQue.Enqueue(sMessage);
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
				F01_MachineWatch.spMachine.Play();
                return "Error";
            }
            int ncount; //書き込み数 = 文字列中のスペースの数を数える
            string sw;  //引数文字列からスペースを抜いた文字列

            //スペースを抜く
            if (sset == null)
            {
                return "Error";
            }

            sw = sset.Replace(" ", "");
            //書込み数 = スペースの数取得
            ncount = sset.Length - sw.Length;

            //最後のスペース削除=エラーになる為
            sset = sset.Substring(0, sset.Length - 1);

            string sendMsg = "WRS " + adr + suffix + " " + ncount + " " + sset + "\r";//.U=10進数16bit符号なし
            //System.Net.Sockets.NetworkStream ns = tcp.GetStream();
            ns = tcp.GetStream();

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendMsg);
            ns.Write(sendBytes, 0, sendBytes.Length);

            //サーバーから送られたデータを受信する
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[10000];
            int resSize;
            try
            {
                DateTime sendDt = DateTime.Now;
                TimeSpan ts = DateTime.Now - sendDt;

                while (ns.DataAvailable == false)
                {
                    Thread.Sleep(10);

                    ts = DateTime.Now - sendDt;
                    if (ts.TotalMilliseconds > 4000)
                    {
                        throw new ApplicationException(
                                string.Format("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無くタイムアウト(4000ms)しました。(WRS)"));
                    }
                }

                do
                {
                    //データの一部を受信する
                    resSize = ns.Read(resBytes, 0, resBytes.Length);//☆★
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        F01_MachineWatch.sp.Play();
                        string sMessage = "Readが0を返した時はサーバーが切断したと判断";
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                        return "Error";
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);

            }
            catch (IOException ex)//基になっている Socket が閉じています。
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                //転送接続からデータを読み取れません:既存の接続はリモートﾎｽﾄに強制的に切断されました。
                return "Error";
            }
            catch (ObjectDisposedException ex)
            {
                F01_MachineWatch.spMachine.Play();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
                return "Error";
            }
            //終了処理---------------------------------------------------------------------------------------
            finally
            {
                if (ms != null) { ms.Close(); }
                //if (tcp != null) { tcp.Close(); }
                //if (ns != null) { ns.Close(); }
            }

            string resMsg = System.Text.Encoding.UTF8.GetString(ms.ToArray());

            return resMsg;
#endif
        }

        #region KEYENCEシーケンサ系 処理(内製機用=A/I,M/D,C/F)
        public bool OpenTcp(ref TcpClient tcp, string host, int port)
        {
/*#if DEBUG
			return true;
#else*/
            string sMessage = "";
            try
            {
                //シーケンサ側の電源が付いていない場合等、→catch (SocketException ex) TimeOutエラー
                if (tcp == null)
                {
                    tcp = new System.Net.Sockets.TcpClient(host, port);
					//sMessage = "[OPEN tcp]";
					//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage); //logファイルに残る回数が多すぎ、ログ時間と[Open tcp]以外の情報が無く内容が有意義で無い為、削除 2013/10/29 n.yoshimoto
                }
                else if (tcp.Connected == false)
                {
                    tcp = new System.Net.Sockets.TcpClient(host, port);
                    //sMessage = "[OPEN Connected]";
                    //log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
                }
            }
            catch (SocketException ex)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message); 
                //Log.Logger.Info(ex.SocketErrorCode.ToString());

                return false;
            }
            return true;
//#endif
        }

		public static bool Open(ref TcpClient tcp, string host, int port)
		{
			string sMessage = "";
			try
			{
				//シーケンサ側の電源が付いていない場合等、→catch (SocketException ex) TimeOutエラー
				if (tcp == null)
				{
					tcp = new System.Net.Sockets.TcpClient(host, port);
					//sMessage = "[OPEN tcp]";
					//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage); //logファイルに残る回数が多すぎ、ログ時間と[Open tcp]以外の情報が無く内容が有意義で無い為、削除 2013/10/29 n.yoshimoto
				}
				else if (tcp.Connected == false)
				{
					tcp = new System.Net.Sockets.TcpClient(host, port);
					sMessage = "[OPEN Connected]";
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, sMessage);
				}
			}
			catch (SocketException ex)
			{
				//Log.Logger.Info(ex.SocketErrorCode.ToString());

				return false;
			}
			return true;
		}

        #endregion

        /// <summary>
        /// 「シリンジ毎のマッピング(before)」を「メモリに書き込める形(after)」へ変換する
        /// </summary>
        /// <param name="before"></param>
        /// <param name="after"></param>
        public void ExchangeZFMemory(string before, ref string[] after)
        {
            int nZFNum;
            if (before.Length % 32 == 0)
            {
                nZFNum = (before.Length / 32);
            }
            else
            {
                nZFNum = (before.Length / 32) + 1;
            }
            int nCnt;
            int nAddCnt = 0;//1メモリ
            string[,] sMem = new string[nZFNum, 16];//デバッグ用として
            double[] dMem = new double[nZFNum];
            string[] recordArray = new string[] { };

            recordArray = before.Split(' ');
            nCnt = recordArray.Length;
            for (int i = 0; i < nCnt; i++)
            {
                for (int k = 0; k < 16; k++)
                {
                    if (recordArray[i] != "")//最後がカンマなら値なし
                    {
                        sMem[nAddCnt, k] = recordArray[i];//デバッグ用として
                        if (recordArray[i] != "0")
                        {
                            dMem[nAddCnt] = dMem[nAddCnt] + Math.Pow(2, k);//実際にMoldに書き込む値
                        }
                    }
                    i += 1;
                    if (i == nCnt)//実質の終了条件
                        break;
                }
                i -= 1;//余分なインクリメントを戻す
                nAddCnt += 1;
            }

            //一度に送れる上限の1000で分割する。許容=2000*16*シリンジ数4=128000
            for (int i = 0; i < nZFNum; i++)
            {
                if (i < 1000)
                {
                    after[0] = after[0] + Convert.ToString(dMem[i]) + " ";//SP区切り;
                }
                else
                {
                    after[1] = after[1] + Convert.ToString(dMem[i]) + " ";//SP区切り;
                }
            }
        }

        //1マガジン単位のファイル数をカウント
        public static SortedList<int, DateTime> CntLogFile(string swpath, string sprefix)
        {
            int nCnt = 0;
            SortedList<int, DateTime> SortedList = new SortedList<int, DateTime>();

            foreach (string swfname in System.IO.Directory.GetFiles(swpath))
            {
                if (swfname.Substring(swfname.LastIndexOf("\\") + 1, sprefix.Length) == sprefix)
                {
                    SortedList.Add(nCnt, System.IO.File.GetLastWriteTime(swfname));
                    nCnt++;
                }
            }

            IOrderedEnumerable<DateTime> retv2 = SortedList.Values.OrderBy(s => s);
            SortedList<int, DateTime> retv = new SortedList<int, DateTime>();

            int i = 0;
            foreach (DateTime dt in retv2)
            {
                retv.Add(i, dt);
                i++;

            }

            return retv;
        }

        //<--Start 1分毎にﾀﾞｲﾎﾞﾝﾀﾞｰ装置接続 2010/10/13 Y.Matsushima 
        public static void Kickbatch(LSETInfo lsetInfo)
        {
            //バッチファイル(ネットワークドライブの割り当て)実行
            string sExeName = @"C:\QCIL\Component\bat\NETUSE_" + lsetInfo.EquipmentNO + ".bat";
            Process proc = new Process();
            proc.StartInfo.FileName = sExeName;
            proc.Start();       //プロセススタート
            proc.WaitForExit(5000); //プロセス完了待ち
        }

        //最新1マガジン分以外のファイルはストック(データベース登録なし)とする
        public static void StockLogFile(string sFromDir, string sToDir, DateTime dtTurning)
        {
            string sCreateYM = "";
            string smovepath = "";
            string sfilenm = "";

            sCreateYM = Convert.ToString(dtTurning).Substring(0, 4) + Convert.ToString(dtTurning).Substring(5, 2);
            foreach (string swfname in System.IO.Directory.GetFiles(sFromDir))
            {
                smovepath = sToDir;
                sfilenm = swfname.Substring(swfname.LastIndexOf("\\") + 1, swfname.Length - (swfname.LastIndexOf("\\") + 1));

                //ターニングポイントより過去のファイルは保管する
                if (System.IO.File.GetLastWriteTime(swfname) <= dtTurning.AddSeconds(1))//
                //if (System.IO.File.GetCreationTime(swfname) <= dtTurning)
                {
                    smovepath = sToDir + sCreateYM + "\\Stock\\";
                    if (System.IO.Directory.Exists(smovepath) == false)
                    {
                        System.IO.Directory.CreateDirectory(smovepath);
                    }
                    smovepath = smovepath + sfilenm;
                    if (System.IO.File.Exists(smovepath) == false)
                    {
                        System.IO.File.Move(swfname, smovepath);//保管フォルダへ移動
                    }
                    else
                    {
                        System.IO.File.Delete(swfname);
                    }
                }
            }
        }

        /// <summary>
        /// Lot単位ファイルのファイル時間スタンプ取得
        /// </summary>
        /// <param name="swpath"></param>
        /// <param name="sprefix"></param>
        /// <returns></returns>
        public static string GetFileStampDT(string swpath, string sprefix)
        {
            string wdt = "";
            foreach (string swfname in System.IO.Directory.GetFiles(swpath))
            {
                if (swfname.Substring(swfname.LastIndexOf("\\") + 1, sprefix.Length) == sprefix)
                {
                    wdt = Convert.ToString(File.GetLastWriteTime(swfname));
                    break;
                }
            }
            return wdt;
        }

        /// <summary>
        /// Lot単位ファイルのファイル時間スタンプ取得
        /// </summary>
        /// <param name="swpath"></param>
        /// <param name="sprefix"></param>
        /// <returns></returns>
        public static string GetFileStampDT2(string swpath, string sprefix)
        {
            string wdt = "";
            foreach (string swfname in System.IO.Directory.GetFiles(swpath))
            {
                if (swfname.Contains(sprefix))
                {
                    wdt = Convert.ToString(File.GetLastWriteTime(swfname));
                    break;
                }
            }
            return wdt;
        }

        //1マガジン単位のファイル数をカウント
        public static SortedList<int, DateTime> CntLogFile2(string swpath, string sprefix)
        {
            int nCnt = 0;
            SortedList<int, DateTime> SortedList = new SortedList<int, DateTime>();

            foreach (string swfname in System.IO.Directory.GetFiles(swpath))
            {
                if (swfname.Contains(sprefix))
                {
                    SortedList.Add(nCnt, System.IO.File.GetLastWriteTime(swfname));
                    nCnt++;
                }
            }

            IOrderedEnumerable<DateTime> retv2 = SortedList.Values.OrderBy(s => s);
            SortedList<int, DateTime> retv = new SortedList<int, DateTime>();

            int i = 0;
            foreach (DateTime dt in retv2)
            {
                retv.Add(i, dt);
                i++;

            }

            return retv;
        }

        public static int GetDBLotIndex(List<ArmsLotInfo> rtnArmsLotInfo)
        {
            int nIndex = -1;//LotにマガジンNoしか入っていない場合-1を返す

            for (int i = 0; i < rtnArmsLotInfo.Count; i++)
            {
                if (rtnArmsLotInfo[i].LotNO.Trim() !=
                    rtnArmsLotInfo[i].InMag.Trim())
                {
                    nIndex = i;//最初に発見した方(開始日時が早い)を採用する。
                    break;
                }
            }

            return nIndex;
        }

        public static int GetEarlyLotIndex(List<ArmsLotInfo> rtnArmsLotInfo)
        {
			bool hasCompLot = false;
			int nIndex = 0;

			List<ArmsLotInfo> compLotList = rtnArmsLotInfo.Where(r => string.IsNullOrEmpty(r.EndDT) == false).ToList();

			if (compLotList.Count() > 0)
			{
				hasCompLot = true;
				nIndex = compLotList.FindIndex(r => string.IsNullOrEmpty(r.EndDT) == false);
			}

			if (hasCompLot)
			{
				// 完成済み実績が含まれている場合は、完成済実績のリストの中で早い実績のインデックスを取得
				for (int i = 0; i < compLotList.Count; i++)
				{
					if (Convert.ToDateTime(compLotList[nIndex].StartDT) > Convert.ToDateTime(compLotList[i].StartDT))
					{
						nIndex = i;
					}
				}
				nIndex = rtnArmsLotInfo.IndexOf(compLotList[nIndex]);
			}
			else
			{
				// 完成済み実績が存在しない場合、従来通り取得実績の中で最も開始時間の早い実績のインデックスを取得
				for (int i = 0; i < rtnArmsLotInfo.Count; i++)
				{
					if (Convert.ToDateTime(rtnArmsLotInfo[nIndex].StartDT) > Convert.ToDateTime(rtnArmsLotInfo[i].StartDT))
					{
						nIndex = i;
					}
				}
			}

            return nIndex;
        }

        public static bool fCheckWBContinueFile(string sFileType, int nTimmingMode)
        {
            bool fCheck = true;
            if (nTimmingMode == Constant.nStartTimming)
            {
                //マガジンタイミングのファイルの場合はTrue(continue:無視する)
                switch (sFileType)
                {
                    case "SP":
                        fCheck = false;
                        break;
                }
            }
            else
            {
                fCheck = false;
            }
            return fCheck;
        }

        #region 日付とintシリアル値の変換

        /// <summary>
        /// 日付データ変換
        /// </summary>
        /// <param name="dt"></param>
        /// <returns></returns>
        public string SerializeDate(DateTime? dt)
        {
            if (dt.HasValue)
            {
                string str = dt.Value.ToString("yyyyMMddHHmm");
                return long.Parse(str).ToString();
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 日付データからDateTimeへ変換
        /// </summary>
        /// <param name="serial"></param>
        /// <returns>非long値はnull</returns>
        public static DateTime? ParseDate(object serial)
        {
            long? longSerial = serial as long?;
            if (longSerial.HasValue == true)
            {
                return ParseDate(longSerial.Value);
            }
            else
            {
                return null;
            }
        }


        /// <summary>
        /// 日付データからDateTimeへ変換
        /// </summary>
        /// <param name="serial"></param>
        /// <returns></returns>
        public static DateTime ParseDate(long serial)
        {
            string str = serial.ToString();

            if (str.Length == 12)
            {
                return DateTime.ParseExact(str, "yyyyMMddHHmm", null);
            }

            else throw new ApplicationException("不正なDateTime値です:" + str);

        }
        #endregion

        /// <summary>
        /// Lot単位ファイルのマガジンNo取得
        /// </summary>
        /// <param name="swpath"></param>
        /// <param name="sprefix"></param>
        /// <returns></returns>
        public static string GetDCMagazineNo(string swpath, string sprefix)
        {
            long first, second;
            string sWork;
            string sMagazineNo = "";
            string[] textArray = new string[] { };
            string[] recordArray = new string[] { };

            foreach (string swfname in System.IO.Directory.GetFiles(swpath))
            {
                if (swfname.Contains(sprefix))
                {
                    first = 0;
                    second = -1;
                    //装置からﾌｧｲﾙが転送中の場合は、Sleepで待つ。ﾌｧｲﾙｻｲｽﾞが同じになった(=転送終了)場合、ﾙｰﾌﾟを抜ける
                    while (first != second)
                    {
                        System.IO.FileInfo fi = new System.IO.FileInfo(swfname);
                        first = fi.Length;

                        System.Threading.Thread.Sleep(500);
                        if (System.IO.File.Exists(swfname) == false)
                        {
                            //sMessage = EquiInfoDB.sAssetsNM + "/" + EquiInfoDB.sMachinSeqNO + "/" + swfname + "が見つかりません。";
                            //Log.Logger.Info(sMessage);
                            //return;
                        }

                        System.IO.FileInfo fi2 = new System.IO.FileInfo(swfname);
                        second = fi2.Length;
                    }
                    //0KBのﾌｧｲﾙは削除して次へ。
                    if (second == 0)
                    {
                        System.IO.File.Delete(swfname);
                        continue;
                    }
                    using (System.IO.StreamReader textFile = new System.IO.StreamReader(swfname, System.Text.Encoding.Default))
                    {

                        sWork = textFile.ReadToEnd();

                        textFile.Close();
                    }

                    textArray = sWork.Split('\n');
                    int nRowCnt = 0;
                    foreach (string srecord in textArray)
                    {
                        nRowCnt += 1;
                        recordArray = srecord.Split(',');
                        if (nRowCnt < 3 || recordArray[0] == "")//「1,2行目」「Logの最終行」は無視
                        {
                            continue;
                        }

                        sMagazineNo = recordArray[3].Trim();//ﾏｶﾞｼﾞﾝNo取得
                        sMagazineNo = sMagazineNo.Replace("\r", "");//余計な文字削除
                        sMagazineNo = sMagazineNo.Replace("\"", "");//余計な文字削除
                        sMagazineNo = sMagazineNo.Replace("30 ", "");//余計な文字削除
                    }
                    break;
                }
            }

            return sMagazineNo;
        }
    }
}
