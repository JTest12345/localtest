using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Oskas.Functions.Plcs
{
    public class _KeyenceTcp
    {
        /// <summary>
        /// PLCのBIT型ONの場合の値
        /// </summary>
        public const string BIT_ON = "1";

        /// <summary>
        /// PLCのBIT型OFFの場合の値
        /// </summary>
        public const string BIT_OFF = "0";

        /// <summary>
        /// タイムアウト時応答
        /// </summary>
        public const string BIT_READ_TIMEOUT_VALUE = "TIMEOUT";

        public static string[] BIT_READ_TIMEOUT_VALUE_ARRAY = new string[] { "TIMEOUT" };

        public int InstanceNo { get; set; }

        private static Mutex mut = new Mutex(false, "PLCMutex");

        /// <summary>
        /// Singletonインスタンス
        /// </summary>
        private static _KeyenceTcp instance;

        /// <summary>
        /// 
        /// </summary>
        private SortedList<string, TcpClient> clients;

        private SortedList<string, System.Net.Sockets.NetworkStream> streams;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        private _KeyenceTcp() 
        {
            this.clients = new SortedList<string, TcpClient>();
            this.streams = new SortedList<string, NetworkStream>();
        }

        ~_KeyenceTcp()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            if (ns != null)
            {
                ns.Close();
            }

            if (tcp != null)
            {
                tcp.Close();
            }

            foreach (KeyValuePair<string, NetworkStream> kv in this.streams)
            {
                kv.Value.Close();
            }

            foreach (KeyValuePair<string, TcpClient> kv in this.clients)
            {
                kv.Value.Close();
            }
        }

        public static _KeyenceTcp GetInstance()
        {
            if (instance == null)
            {
                instance = new _KeyenceTcp();
                instance.InstanceNo = 1;
            }
            return instance;
        }

        private TcpClient getClient(string host, int port)
        {
            TcpClient retv = null;

            foreach (KeyValuePair<string, TcpClient> kv in this.clients)
            {
                if (kv.Key == host)
                {
                    retv = kv.Value;
                    break;
                }
            }

            if (retv != null)
            {
                if (retv.Connected == true)
                {
                    return retv;
                }
                else
                {
					retv.Close();
                    retv = new TcpClient(host, port);
					this.clients[host] = retv;
					this.streams[host] = retv.GetStream();

                    return retv;
                }
            }

            TcpClient client = new TcpClient(host, port);
            try
            {
                this.clients.Add(host, client);
                this.streams.Add(host, client.GetStream());
            }
            catch { }
            return client;
        }

		private NetworkStream getStream(string host)
		{
			for (int i = 0; i < this.streams.Count; i++)
			{
				if (this.streams.Keys[i] == host)
				{
					return this.streams.Values[i];
				}
			}

			this.streams[host] = this.clients[host].GetStream();
			return this.streams[host];
		}

        /// <summary>
        /// マガジン番号WORDアドレスの長さ
        /// 13 Nxxxxxxxxxxx 1 1
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH = 10;

		/// <summary>
		/// 装置番号WORDアドレスの長さ
		/// </summary>
		public const int MACHINE_NO_WORD_LENGTH = 6;

        /// <summary>
        /// TCPソケット
        /// </summary>
        private static System.Net.Sockets.TcpClient tcp;

        /// <summary>
        /// ネットワークストリーム
        /// </summary>
        private static System.Net.Sockets.NetworkStream ns = null;

        /// <summary>
        /// PLC応答から終了コード取得
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        private bool isExitNormal(byte[] plcResponseData)
        {
            string res = System.Text.Encoding.UTF8.GetString(plcResponseData).Trim();
            {
                if (res == "OK")
                {
                    return true;
                }

                if (res == "E0")
                {
                    return false;
                }

                if (res == "E1")
                {
                    return false;
                }
            }

            return true;
        }

        private enum CommandType
        {
            RD,
            RDS,
            WR,
            WRS,
        }

        public bool Ping(string host, int timeout, int retryTimes)
        {
            // Pingオブジェクトの作成
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            // Pingを送信する(）
            System.Net.NetworkInformation.PingReply reply = ping.Send(host, timeout);
            // 結果を取得
            bool ret = false;

            try
            {
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    ret = true;
                }
                else
                {
                    if (retryTimes <= 2)
                    {
                        return Ping(host, timeout, ++retryTimes);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                ping.Dispose();
            }

            return ret;
        }

        #region executeCommand コマンド実行して生の結果を返す

        /// <summary>
        /// PLCにASCIIコマンド送信
        /// </summary>
        /// <param name="asciiCommand"></param>
        /// <returns></returns>
        public byte[] executeCommand(byte[] bynaryCommand, string host, int port)
        {
            DateTime constart = DateTime.Now;

            //死活監視 3連続で50msのPingに応答しない場合は応答停止
            if (Ping(host, 50, 1) == false)
            {
                //LineKeeper.DisavailableHost(host);
                return System.Text.Encoding.UTF8.GetBytes("E0");
            }

            tcp = getClient(host, port);

			try
			{
                mut.WaitOne();

				try
				{
					ns = this.getStream(host);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("[Error] MachinePLC.executeCommand getStream内で異常発生 host:{0} port{1}",
						host, port));
				}

                byte[] sendBytes = bynaryCommand;

				try
				{
					ns.Write(sendBytes, 0, sendBytes.Length);
				}
				catch (Exception)
				{
					throw new Exception(string.Format("[Error] MachinePLC.executeCommand ns.Writeで異常発生 host:{0} port{1} bynaryCommand:{2}",
						host, port, string.Join(" ", bynaryCommand)));
				}

				//サーバーから送られたデータを受信する
				System.IO.MemoryStream ms = new System.IO.MemoryStream();
				byte[] resBytes = new byte[256];
				int resSize;

				try
				{
					do
					{
						//データの一部を受信する
						resSize = ns.Read(resBytes, 0, resBytes.Length);
						//Readが0を返した時はサーバーが切断したと判断
						if (resSize == 0)
						{
							//Log.SysLog.Error("PLC通信切断発生");
							throw new ApplicationException("PLC通信切断発生");
						}
						ms.Write(resBytes, 0, resSize);
						Thread.Sleep(1);
					} while (ns.DataAvailable);

					byte[] res = ms.ToArray();
					ms.Close();

					return res;
				}
				catch (Exception)
				{
					//throw new Exception(string.Format("[Error] MachinePLC.executeCommand ns.Readで異常発生 host:{0} port{1} resBytes:{2}",
					//	host, port, string.Join(" ", resBytes)));
					throw new Exception(string.Format("装置との通信に失敗しました。 装置の電源がONになっているか確認後、再開して下さい。IPAddress:{0} Port{1} 応答:{2}",
						host, port, string.Join(" ", resBytes)));
				}
			}
			//catch (Exception)
			//{
			//	throw new Exception("PLC不正応答のため装置処理停止:" + host);
			//	//Log.SysLog.Error("PLC不正応答のため装置無効設定:" + host);
			//	//LineKeeper.DisavailableHost(host);
			//	//return new byte[] { 0 };
			//}
            finally
            {
                mut.ReleaseMutex();
                //ns.Close();
                //tcp.Close();
                //ns.Dispose();
            }
        }
        #endregion

        #region SetBit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetBit(string hexAddressWithDeviceNM, string data, string host, int port)
        {

            string command = CommandType.WR.ToString() + " " + hexAddressWithDeviceNM + " " + data + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd, host, port);

            if (isExitNormal(res) == true)
            {
                return true;
            }
            else
            {
                //異常終了
                //Log.SysLog.Error("PLCエラー応答:" + command);
                return false;
            }
        }
        #endregion

        #region GetBit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetBit(string hexAddressWithDeviceNM, string address, int port)
        {
            string command = CommandType.RD.ToString() + " " + hexAddressWithDeviceNM + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd, address, port);

            if (isExitNormal(res) == true)
            {
                return System.Text.Encoding.UTF8.GetString(res).Trim();
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + command);
                return _KeyenceTcp.BIT_READ_TIMEOUT_VALUE;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetWord(string hexAddressWithDeviceNM, string address, int port)
        {
            string command = CommandType.RD.ToString() + " " + hexAddressWithDeviceNM + ".D\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd, address, port);

            if (isExitNormal(res) == true)
            {
                return System.Text.Encoding.UTF8.GetString(res).Trim();
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + command);
                return _KeyenceTcp.BIT_READ_TIMEOUT_VALUE;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string[] GetWord(string hexAddressWithDeviceNM, int points, string address, int port)
        {
            string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + ".U"  + " " + points.ToString() + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd, address, port);

            if (isExitNormal(res) == true)
            {
                return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + command);
                return _KeyenceTcp.BIT_READ_TIMEOUT_VALUE_ARRAY;
            }
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string[] GetBit(string hexAddressWithDeviceNM, int points, string address, int port)
        {
            string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + " " + points.ToString() + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd, address, port);

            if (isExitNormal(res) == true)
            {
                return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + command);
                return _KeyenceTcp.BIT_READ_TIMEOUT_VALUE_ARRAY;
            }
        }



        #endregion

        #region GetWordsAsDateTime

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>不正応答時はNULL</returns>
        public DateTime? GetWordsAsDateTime(string hexAddressWithDeviceNM, string host, int port)
        {
            string[] res = this.GetBit(hexAddressWithDeviceNM, 6, host, port);

            return GetWordsAsDateTime(res);
        }

        public DateTime? GetWordsAsDateTime(string[] res)
        {
            DateTime? retv = null;
            try
            {
                int year = int.Parse(res[0]);
                int month = int.Parse(res[1]);
                int day = int.Parse(res[2]);
                int hour = int.Parse(res[3]);
                int minute = int.Parse(res[4]);
                int sec = int.Parse(res[5]);

                retv = new DateTime(year, month, day, hour, minute, sec);

            }
            catch
            {
                return null;
            }

            return retv;
        }

        #endregion
        
        #region SetWordAsDecimalData
        public void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data, string host, int port)
        {
            Send1ByteData(hexAddressWithDeviceNM, data, host, port);
        }

        public void Send1ByteData(string memoryAddressNO, int sendByteData, string host, int port)
        {
            //int length;
            string sendData;

            sendData = sendByteData.ToString("X2");

            string command = string.Format("WRS {0}{1} {2} {3}\r", memoryAddressNO, ".H", 1, sendData);

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand(byteCmd, host, port);

            //tcp = getClient(host, port);
            //try
            //{
            //    mut.WaitOne();


            //    ns = tcp.GetStream();
            //    byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
            //    ns.Write(sendBytes, 0, sendBytes.Length);

            //    System.IO.MemoryStream ms = new System.IO.MemoryStream();
            //    byte[] resBytes = new byte[10000];
            //    try
            //    {
            //        do
            //        {
            //            int resSize = ns.Read(resBytes, 0, resBytes.Length);
            //            if (resSize == 0)
            //            {
            //                throw new Exception();
            //            }
            //            ms.Write(resBytes, 0, resSize);
            //        } while (ns.DataAvailable);

            //        return System.Text.Encoding.UTF8.GetString(ms.ToArray());
            //    }
            //    finally
            //    {
            //        if (ms != null) { ms.Close(); }
            //    }
            //}
            //finally
            //{
            //    mut.ReleaseMutex();
            //    //ns.Close();
            //    //tcp.Close();
            //    //ns.Dispose();
            //}
        }

        #endregion

        #region GetWordsAsDateTimeAsDecimalData

        public int GetWordAsDecimalData(string hexAddressWithDeviceNM, string address, int port)
        {
            int retv = 0;

            string data = GetWord(hexAddressWithDeviceNM, address, port);
            if (int.TryParse(data, out retv))
            {
                return retv;
            }
            else
            {
                // TIMEOUTを想定
                return -1;
            }
        }


        #endregion

        #region GetMagazineNo

        /// <summary>
        /// 不正応答時は空白
        /// </summary>
        /// <param name="hexAddressWithDeviceNm"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided, string host, int port, int wordLength)
        {
            string qr = string.Empty;

            string[] org = this.GetBit(hexAddressWithDeviceNm, wordLength, host, port);
            if (org == _KeyenceTcp.BIT_READ_TIMEOUT_VALUE_ARRAY)
            {
                return string.Empty;
            }

            return GetMagazineNo(org, notDevided);
        }
       
        /// <summary>
        /// PLCが読み取ったマガジン番号を返す。
        /// 要素数4の場合は分割マガジン番号を返す
        /// </summary>
        /// <param name="plcResponseBitArray"></param>
        /// <returns></returns>
        public string GetMagazineNo(string[] plcResponseBitArray, bool notDevided)
        {
            string qr = string.Empty;

            foreach (string orgs in plcResponseBitArray)
            {
                qr += System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(int.Parse(orgs)));
            }

            //Null文字を置換
            qr = qr.Replace("\0", "");
            string[] elms = qr.Trim().Split(' ');

            if (elms.Length == 2 || (elms.Length >= 3 && notDevided))
            {
                return elms[1];
            }
            else if (elms.Length >= 3)
            {
                string magno = elms[1];
                int seqno = int.Parse(elms[2]);

                //magno = Order.NascaLotToMagLot(magno, seqno);
                return magno;
            }
            else
            {
                return qr.Trim();
            }
        }

		public string GetMachineNo(string hexAddressWithDeviceNm, string host, int port, int wordLength)
		{
			string qr = string.Empty;

			string[] org = this.GetBit(hexAddressWithDeviceNm, wordLength, host, port);
			if (org == _KeyenceTcp.BIT_READ_TIMEOUT_VALUE_ARRAY)
			{
				return string.Empty;
			}

			return GetMachineNo(org);
		}

		public string GetMachineNo(string[] plcResponseBitArray)
		{
			string qr = string.Empty;

			foreach (string orgs in plcResponseBitArray)
			{
				qr += System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(int.Parse(orgs)));
			}

			//Null文字を置換
			qr = qr.Replace("\0", "");
			string[] elms = qr.Trim().Split(' ');

			if (elms.Length >= 2)
			{
				return elms[1];
			}
			else
			{
				return qr.Trim();
			}
		}

        /// <summary>
        /// 連続したアドレスの情報を文字列として取得
        /// </summary>
        /// <param name="hexAddressWithDeviceNm"></param>
        /// <param name="wordLength"></param>
        /// <param name="removePrefix">半角スペースより前を取り除く(半角スペース複数個は非対応)</param>
        /// <param name="host"></param>
        /// <returns></returns>
        public string GetString(string hexAddressWithDeviceNm, int wordLength, string host, int port, bool isBigEndian)
        {
            string retv = string.Empty;

            string[] org = this.GetBit(hexAddressWithDeviceNm, wordLength, host, port);

            foreach (string s in org)
            {
                var bit = BitConverter.GetBytes(int.Parse(s));
                if(isBigEndian)
                {
                    bit = bit.Reverse().ToArray();
                }
                retv += System.Text.Encoding.UTF8.GetString(bit);
            }

            //Null文字を置換
            retv = retv.Replace("\0", "");
            return retv;
        }

        public void SetString(string hexAddressWithDeviceNm, string ascii, string host, int port)
        {
            byte[] byteStr = Encoding.ASCII.GetBytes(ascii);

            List<byte> byteList = byteStr.ToList();

            if (Math.IEEERemainder(byteStr.Length, 2) != 0)
            {
                byteList.Add(0x00);
                byteStr = byteList.ToArray();
            }

            byte[] bs = new byte[2];
            string cmdWrite = null;
            for (int i = 0; i < byteStr.Length; i += 2)
            {
                if (i > 0)
                {
                    cmdWrite += " ";
                }

                Array.Copy(byteStr, i, bs, 0, 2);
                //cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).Reverse().ToArray());
                cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).ToArray());
            }

            string command = CommandType.WRS.ToString() + " " + hexAddressWithDeviceNm + ".H " + (byteStr.Length / 2).ToString() + " " + cmdWrite + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand(byteCmd, host, port);
        }

        public void SetString(string hexAddressWithDeviceNm, string setdata, string host, int port, string encoding)
        {
            Encoding sjisEnc = Encoding.GetEncoding(encoding);
            byte[] byteStr = sjisEnc.GetBytes(setdata);

            List<byte> byteList = byteStr.ToList();

            if (Math.IEEERemainder(byteStr.Length, 2) != 0)
            {
                byteList.Add(0x00);
                byteStr = byteList.ToArray();
            }

            byte[] bs = new byte[2];
            string cmdWrite = null;
            for (int i = 0; i < byteStr.Length; i += 2)
            {
                if (i > 0)
                {
                    cmdWrite += " ";
                }

                Array.Copy(byteStr, i, bs, 0, 2);
                //cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).Reverse().ToArray());
                cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).ToArray());
            }

            string command = CommandType.WRS.ToString() + " " + hexAddressWithDeviceNm + ".H " + (byteStr.Length / 2).ToString() + " " + cmdWrite + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand(byteCmd, host, port);
        }


        #endregion

        #region WatchBit
        /// <summary>
        /// 指定したアドレスが指定値を満たすまで、待機する。act指定時は、ループごとにact処理を実行 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="timeout">タイムアウトまでの秒数（sec) 0または負なら無限ループ</param>
        /// <param name="exitValue">監視を抜ける条件</param>
        /// <returns>タイムアウト時false</returns>
        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue, string address, int port)
        {
            return WatchBit(hexAddressWithDeviceNM, timeout, exitValue, address, port);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="timeout">タイムアウトまでの秒数（sec) 0または負なら無限ループ</param>
        /// <param name="exitValue">監視を抜ける条件</param>
        /// <returns>タイムアウト時false</returns>
        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue, string address, int port, Action act)
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                try
                {
                    string retv = GetBit(hexAddressWithDeviceNM, address, port);

                    if (retv == exitValue)
                    {
                        if (act != null) act();
                        return true;
                    }

                    if (timeout > 0)
                    {
                        if ((DateTime.Now - start).TotalSeconds >= timeout)
                        {
                            if (act != null) act();
                            return false;
                        }
                    }
                    if (act != null) act();

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    //Log.RBLog.Error("WatchBit PLC通信エラー発生　10秒待機後に監視継続:" + ex.ToString());
                    Thread.Sleep(10000); //10秒待機
                }
            }
        }
        #endregion
    }
}
