using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using System.Text;

namespace EICS
{
    #region 2015/9/16 追加分 パラメータ管理機能
    public class KeyenceDeviceMasterList
    {
        public string DeviceNM;     // デバイス識別
        public string Front;        // 先頭アドレス
        public string Last;         // 最後尾アドレス
        public string Suffix;       // データ型
        public int ReadLimit1;      // ビット/.U/.S/.Hの読取個数の限界値
        public int ReadLimit2;      // .D/.Lの連続データの読取個数の限界値
        public int Digit;           // アドレス番号の桁数

        public KeyenceDeviceMasterList(string p1, string p2, string p3, string p4, int p5, int p6, int p7)
        {
            DeviceNM = p1;
            Front = p2;
            Last = p3;
            Suffix = p4;
            ReadLimit1 = p5;
            ReadLimit2 = p6;
            Digit = p7;
        }
    }
    #endregion
    
    public class PLC_Keyence : IDisposable, IPlc
	{
		public static string PLC_TYPE = "KEYENCE";

		public string HostAddressNO { get; set; }
		public int PortNO { get; set; }

		private List<EICS.Machine.PLCDDGBasedMachine.ExtractExclusion> ExtractExclusionList = new List<Machine.PLCDDGBasedMachine.ExtractExclusion>();

		/// <summary>
		/// PLCのBIT型ONの場合の値
		/// </summary>
		public const int BIT_ON = 1;

		/// <summary>
		/// PLCのBIT型OFFの場合の値
		/// </summary>
		public const int BIT_OFF = 0;

		/// <summary>
		/// TCPクライアント
		/// </summary>
		private System.Net.Sockets.TcpClient tcp;
		private System.Net.Sockets.UdpClient udp;

		private enum CommandType
		{
			RD,
			RDS,
			WR,
			WRS,
		}

        public const string BIT_READ_TIMEOUT_VALUE = "TIMEOUT";

        #region 2015/9/16 追加分 パラメータ管理機能

        // KV7500・KV7300の定義
        public static List<KeyenceDeviceMasterList> DEVICEMASTERLIST_7500 = new List<KeyenceDeviceMasterList>(){
           new KeyenceDeviceMasterList("R", "00000", "99915", "", 1000, 500, 5),
           new KeyenceDeviceMasterList("B", "0000", "7FFF", "", 1000, 500, 4),
           new KeyenceDeviceMasterList("MR", "00000", "399915", "", 1000, 500, 5),
           new KeyenceDeviceMasterList("LR", "00000", "99915", "", 1000, 500, 5),
           new KeyenceDeviceMasterList("CR", "0000", "7915", "", 1000, 500, 4),
           new KeyenceDeviceMasterList("VB", "0000", "F9FF", "", 1000, 500, 4),
           new KeyenceDeviceMasterList("DM", "00000", "65534", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("EM", "00000", "65534", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("FM", "00000", "32767", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("ZF", "00000", "524287", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("W", "0000", "7FFF", ".U", 1000, 500, 4),
           new KeyenceDeviceMasterList("TM", "000", "511", ".U", 512, 256, 3),
           new KeyenceDeviceMasterList("ZF", "01", "12", ".U", 12, 12, 2),
           new KeyenceDeviceMasterList("T", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("TC", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("TS", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("C", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("CC", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("CS", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("AT", "0", "7", ".D", 8, 8, 1),
           new KeyenceDeviceMasterList("CM", "0000", "5999", ".U", 1000, 500, 4),
           new KeyenceDeviceMasterList("VM", "00000", "50999", ".U", 1000, 500, 5)
        };
        // KV5500の定義
        public static List<KeyenceDeviceMasterList> DEVICEMASTERLIST_5500 = new List<KeyenceDeviceMasterList>(){
           new KeyenceDeviceMasterList("R", "00000", "99915", "", 1000, 500, 5),
           new KeyenceDeviceMasterList("B", "0000", "3FFF", "", 1000, 500, 4),
           new KeyenceDeviceMasterList("MR", "00000", "99915", "", 1000, 500, 5),
           new KeyenceDeviceMasterList("LR", "00000", "99915", "", 1000, 500, 5),
           new KeyenceDeviceMasterList("CR", "0000", "3915", "", 1000, 500, 4),
           new KeyenceDeviceMasterList("VB", "0000", "3FFF", "", 1000, 500, 4),
           new KeyenceDeviceMasterList("DM", "00000", "65534", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("EM", "00000", "65534", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("FM", "00000", "32767", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("ZF", "00000", "131071", ".U", 1000, 500, 5),
           new KeyenceDeviceMasterList("W", "0000", "3FFF", ".U", 1000, 500, 4),
           new KeyenceDeviceMasterList("TM", "000", "511", ".U", 512, 256, 3),
           new KeyenceDeviceMasterList("Z", "01", "12", ".U", 12, 12, 2),
           new KeyenceDeviceMasterList("T", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("TC", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("TS", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("C", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("CC", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("CS", "0000", "3999", ".D", 120, 120, 4),
           new KeyenceDeviceMasterList("CTH", "0", "1", ".D", 2, 2, 1),
           new KeyenceDeviceMasterList("CTC", "0", "3", ".D", 4, 4, 1),
           new KeyenceDeviceMasterList("AT", "0", "7", ".D", 8, 8, 1),
           new KeyenceDeviceMasterList("CM", "0000", "5999", ".U", 1000, 500, 4),
           new KeyenceDeviceMasterList("VM", "00000", "49999", ".U", 1000, 500, 5)
        };


        #endregion

		/// <summary>
		/// 内部未実装
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns></returns>
		public bool GetBool(string hexAddressWithDeviceNM)
		{
			string data;
			string sendCommand = string.Format("RD {0}\r", hexAddressWithDeviceNM);
			ns = tcp.GetStream();

			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[256];
			int resSize;
			try
			{
				do
				{
					resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				data = System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();

				if (Convert.ToInt16(data) == 1)
				{
					return true;
				}
				else if (Convert.ToInt16(data) == 0)
				{
					return false;
				}
				else
				{
					throw new Exception(string.Format(Constant.MessageInfo.Message_120, data));
				}

			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        //public string[] GetBit(string hexAddressWithDeviceNM, int points)
        //{
        //    string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + " " + points.ToString() + "\r";

        //    byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

        //    byte[] res = executeCommand1(byteCmd);

        //    if (isExitNormal(res) == true)
        //    {
        //        return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
        //    }
        //    else
        //    {
        //        //異常終了
        //        //Log.WriteError("PLCエラー応答:" + command);
        //        return BIT_READ_TIMEOUT_VALUE_ARRAY;
        //    }
        //}

        public void SetBit(string hexAddressWithDeviceNM, int points, string data)
		{
			SetBit(hexAddressWithDeviceNM, points, data, string.Empty);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <param name="points"></param>
		/// <param name="data">"0"か"1"</param>
		public bool SetBit(string hexAddressWithDeviceNM, int points, string data, string encStr)
		{
			string commandKindStr = string.Empty;
			if (data == "1")
			{
				commandKindStr = "STS";
			}
			else if (data == "0")
			{
				commandKindStr = "RSS";
			}
			else
			{
				throw new ApplicationException(string.Format(@"SetBit()の引数:dataは""0""もしくは""1""で無ければなりません。data=""{0}""", data));
			}

			string sendCommand = string.Format("{0} {1} {2}\r", commandKindStr, hexAddressWithDeviceNM, points);

			ns = tcp.GetStream();

			if (string.IsNullOrEmpty(encStr))
			{
				encStr = "ASCII";
			}

			byte[] sendBytes =  Encoding.GetEncoding(encStr).GetBytes(sendCommand);

			byte[] res = executeCommand1(sendBytes);

			if (isExitNormal(res) == true)
			{
				return true;
			}
			else
			{
				//異常終了
				return false;
			}
		}


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="wordLength"></param>
        /// <param name="doSwapFg"></param>
        /// <returns></returns>
        public string GetString(string hexAddressWithDeviceNM, int wordLength, bool doSwapFg, bool isBigEndian)
        {
            string retv = GetStringWithoutReplace(hexAddressWithDeviceNM, wordLength, doSwapFg, isBigEndian);

            return retv.Replace("\r", "").Replace("\n", "");
        }

        public string GetStringWithoutReplace(string hexAddressWithDeviceNM, int wordLength, bool doSwapFg, bool isBigEndian)
        {
            //string recvVal = GetWord(hexAddressWithDeviceNM, wordLength);

            string recvVal = GetData(hexAddressWithDeviceNM, wordLength, ".U", false);

			string[] strItemArray = recvVal.Split(' ');

			List<byte> byteList = new List<byte>();
			foreach (string strItem in strItemArray)
			{
				int convVal;
				if (int.TryParse(strItem, out convVal) == false)
				{
					throw new ApplicationException(
						string.Format("取得ﾃﾞｰﾀが数値変換出来ませんでした。取得ﾃﾞｰﾀ:{0}", strItem));
				}

                var bit = BitConverter.GetBytes(convVal);
                if (isBigEndian)
                {
                    bit = bit.Reverse().ToArray();
                }
                byteList.AddRange(bit);
            }

			if (doSwapFg)
			{
				int byteLen = byteList.Count;
				byte[] temp = new byte[byteLen];
				for (int i = 0; i < byteLen; i += 2)
				{
					temp[i] = byteList[i + 1];
					temp[i + 1] = byteList[i];
				}
				if (byteLen % 2 != 0)
				{
					temp[byteLen - 1] = byteList[byteLen];
				}

				byteList = temp.ToList();
			}
			else
			{
			}

			String retv = Encoding.ASCII.GetString(byteList.ToArray());

			retv = retv.Replace("\0", "").Trim();

			return retv;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string GetString2(string hexAddressWithDeviceNM, int points)
        {
            string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + ".H " + points.ToString() + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            string str = Encoding.ASCII.GetString(executeCommand2(byteCmd));
            //スペース除去して全結合
            str = str.Replace(" ", "").Trim();

            StringBuilder sb = new StringBuilder();

            //2文字ずつASCII変換
            for (int i = 0; i < str.Length; i += 2)
            {
                short s = Convert.ToInt16(str.Substring(i, 2), 16);
                sb.Append(Convert.ToChar(s));
            }


            return sb.ToString().Trim('\0');
        }

        public string GetString_AS_ShiftJIS(string hexAddressWithDeviceNM, int points)
        {
            string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + ".H " + points.ToString() + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            Encoding sjisEnc = Encoding.GetEncoding("Shift_JIS");

            string str = sjisEnc.GetString(executeCommand2(byteCmd));

            //スペース除去して全結合
            str = str.Replace(" ", "").Trim();

            //2文字ずつbyte型配列に格納
            List<byte> bList = new List<byte>();
            for (int i = 0; i < str.Length; i += 2)
            {
                string ss = str.Substring(i, 2);
                int bNumber = Convert.ToInt32(ss, 16);
                byte b = Convert.ToByte(bNumber);
                bList.Add(b);
            }

            // 再度Shift_JIS変換
            str = sjisEnc.GetString(bList.ToArray());


            return str.Replace("\0", "").Trim();

        }

		/// <summary>
		/// 文字列送信(ASCII)
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public bool SetString(string hexAddressWithDeviceNM, string data)
		{
			return SetString(hexAddressWithDeviceNM, data, "ASCII");
		}

        /// <summary>
        /// 文字列送信
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="data"></param>
        /// <returns></returns>
		public bool SetString(string hexAddressWithDeviceNM, string data, string encStr)
		{
			if (string.IsNullOrEmpty(encStr))
			{
				encStr = "ASCII";
			}

			byte[] byteStr = Encoding.GetEncoding(encStr).GetBytes(data);

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
                cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).ToArray());
            }

			string command = CommandType.WRS.ToString() + " " + hexAddressWithDeviceNM + ".H " + (byteStr.Length / 2).ToString() + " " + cmdWrite + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand1(byteCmd);
        
			return true;
		}


		public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength)
		{
			return GetWordAsDecimalData(hexAddressWithDeviceNM, wordLength, "");
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <param name="wordLength"></param>
		/// <returns></returns>
		public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength, string suffix)
		{
			int retv;
			string recvData = GetData(hexAddressWithDeviceNM, wordLength, suffix, false);

			if (int.TryParse(recvData, out retv) == false)
			{
				throw new ApplicationException(
					string.Format("PLCからの取得値が数値変換出来ません。 取得値:{0}　PLCﾒﾓﾘ:{1}　取得長:{2}", recvData, hexAddressWithDeviceNM, wordLength));
			}

			return retv;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <param name="data"></param>
		public void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
		{
			Send1ByteData(hexAddressWithDeviceNM, data);
		}

		public float GetWordAsFloatData(string hexAddressWithDeviceNM, int len)
		{
			float retv;

			string recvData = GetData(hexAddressWithDeviceNM, len, ".H", false);

			List<byte> byt = new List<byte>();

			if (len <= 2)
			{
				recvData = recvData.PadLeft(8, '0');
			}
			for (int i = 0; i < recvData.Length / 2; i++)
			{

				byt.Add(Convert.ToByte(recvData.Substring(2 * i, 2), 16));
			}

			retv = BitConverter.ToSingle(byt.ToArray(), 0);

			return retv;
		}

		/// <summary>
		/// ネットワークストリーム
		/// </summary>
		private System.Net.Sockets.NetworkStream ns = null;

		public PLC_Keyence(string hostAddressNO, int portNO)
		{
			tcp = new System.Net.Sockets.TcpClient(hostAddressNO, portNO);
			udp = new System.Net.Sockets.UdpClient(hostAddressNO, portNO);
		}

		public bool ConnectedPLC()
		{
			return tcp.Connected;
		}

		public void ConnectPLC()
		{
			tcp = new System.Net.Sockets.TcpClient(HostAddressNO, PortNO);
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
		}

		public string SetBit(string memoryAddressNO, bool statusFG)
		{
			string sendCommand = string.Format("WRS {0}{1} {2} {3}\r",
				memoryAddressNO, Constant.ssuffix_U, 1, Convert.ToInt16(statusFG));

			ns = tcp.GetStream();
			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[10000];
			try
			{
				do
				{
					int resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				return System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetBit2(string hexAddressWithDeviceNM, string data)
        {

            string command = CommandType.WR.ToString() + " " + hexAddressWithDeviceNM + " " + data + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand1(byteCmd);

            if (isExitNormal(res) == true)
            {
                return true;
            }
            else
            {
                //異常終了
                return false;
            }
        }

		public string Send1ByteData(string memoryAddressNO, int sendByteData)
		{
			int length;
			string sendData;

			sendData = sendByteData.ToString("X2");

			string sendCommand = string.Format("WRS {0}{1} {2} {3}\r", memoryAddressNO, Constant.ssuffix_H, 1, sendData);

			ns = tcp.GetStream();
			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[10000];
			try
			{
				do
				{
					int resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				return System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

		public string SendString(string memoryAddressNO, string sendData)
		{
			int length;

			sendData = GetSendMsg_WRS(sendData, true, out length);

			string sendCommand = string.Format("WRS {0}{1} {2} {3}\r",
				memoryAddressNO, Constant.ssuffix_U, length, sendData);

			ns = tcp.GetStream();
			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[10000];
			try
			{
				do
				{
					int resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				return System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

        /// <summary>
        /// 内部未実装
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public string GetBit(string hexAddressWithDeviceNM, int length)
        {
            string sendCommand = string.Format("RD {0}\r", hexAddressWithDeviceNM);
            ns = tcp.GetStream();

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
            ns.Write(sendBytes, 0, sendBytes.Length);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize;
            try
            {
                do
                {
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    if (resSize == 0)
                    {
                        throw new Exception($"PLCアドレス「{hexAddressWithDeviceNM}」の取得に失敗しました。");
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);

                return System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();
            }
            finally
            {
                if (ms != null) { ms.Close(); }
            }
        }

        public string GetBit(string memoryAddressNO)
		{
			string data;
			string sendCommand = string.Format("RD {0}\r", memoryAddressNO);
			ns = tcp.GetStream();

			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[256];
			int resSize;
			try
			{
				do
				{
					resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				data = System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();

				if (Convert.ToInt16(data) == 1)
				{
					return "1";
				}
				else if (Convert.ToInt16(data) == 0)
				{
					return "0";
				}
				else
				{
					throw new Exception(string.Format(Constant.MessageInfo.Message_120, data));
				}

			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public string GetWord(string hexAddressWithDeviceNM)
		{
			return GetWord(hexAddressWithDeviceNM, 1);
		}

		public string GetWord(string memoryAddressNO, int length)
		{
			string sendCommand = string.Format("RDS {0}{1} {2}\r", memoryAddressNO, Constant.ssuffix_U, length);
			ns = tcp.GetStream();

			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[256];
			int resSize;
			try
			{
				do
				{
					resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				return System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();
			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

		public byte[] ExecuteCommand(byte[] bynaryCommand)
		{
			return ExecuteCommand(bynaryCommand, true);
		}

		private byte[] ExecuteCommand(byte[] binaryCommand, bool canRetry)
		{
			byte[] sendBytes = binaryCommand;

			try
			{
				udp.Client.Blocking = true;
				udp.Send(sendBytes, sendBytes.Length);

				System.Net.IPEndPoint remoteEP = null;
				return udp.Receive(ref remoteEP);
			}
			catch (Exception error)
			{
				if (canRetry)
				{
					return ExecuteCommand(binaryCommand, false);
				}
				throw error;
			}
		}

		/// <summary>
		/// PLC応答から終了コード取得
		/// </summary>
		/// <param name="responseData"></param>
		/// <returns></returns>
		private bool isExitNormal(byte[] plcResponseData)
		{
			string res = System.Text.Encoding.UTF8.GetString(plcResponseData).Trim();
			{
				switch (res)
				{
					case "E0":
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_109, res));
						break;
					case "E1":
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_110, res));
						break;
					case "E4":
						throw new ApplicationException(string.Format(Constant.MessageInfo.Message_111, res));
						break;
					default:
						return true;
						break;
				}
			}


		}

		public string GetData(string memoryAddressNO, int length, string suffix, bool isConvB2A)
		{
			string dataStr;
			string sendCommand = string.Format("RDS {0}{1} {2}\r", memoryAddressNO, suffix, length);
			ns = tcp.GetStream();

			byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
			ns.Write(sendBytes, 0, sendBytes.Length);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();
			byte[] resBytes = new byte[256];
			int resSize;
			try
			{
				do
				{
					resSize = ns.Read(resBytes, 0, resBytes.Length);
					if (resSize == 0)
					{
						throw new Exception();
					}
					ms.Write(resBytes, 0, resSize);
				} while (ns.DataAvailable);

				dataStr = Encoding.UTF8.GetString(ms.ToArray());

				if (isConvB2A)
				{
					dataStr = (BytesStringToAscii(dataStr)).Replace("\r\n", "").Trim();
				}
				else
				{
					dataStr = dataStr.Replace("\r\n", "").Trim();
				}

				return dataStr;
			}
			finally
			{
				if (ms != null) { ms.Close(); }
			}
		}

		public string GetData(string Addr, int num)
		{
			return GetData(Addr, num, string.Empty, false);
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
		public string GetSendMsg_WRS(string sData, bool isLittleEndian, out int length)
		{
			int num = (sData.Length / 2) + (sData.Length % 2);//上位・下位8bitに一文字、計2文字でnum=1
			int[] nsend = new int[num];

			byte[] bwork = System.Text.Encoding.UTF8.GetBytes(sData);
			int wsend = 0, wsend2 = 0;//sendに入れる前のwork領域
			int nCnt = 0, nCnt2 = 0;
			string swork = "";

			for (int i = 0; i < sData.Length; i++)
			{
				wsend = System.Convert.ToInt32(bwork[i]);
				if (isLittleEndian)
				{
					//上位8bitの場合
					if (i % 2 == 0)
					{
						wsend2 = wsend << 8;//8bit Shift
					}
					else//下位8bitの場合
					{
						wsend2 = wsend2 + wsend;
					}
				}
				else
				{
					//上位8bitの場合
					if (i % 2 == 0)
					{
						wsend2 = wsend2 + wsend;
					}
					else//下位8bitの場合
					{
						wsend2 = wsend2 + (wsend << 8);//8bit Shift						
					}
				}
				nCnt += 1;

				if (nCnt % 2 == 0 || i == sData.Length - 1)
				{
					nsend[nCnt2] = wsend2;
					nCnt2 += 1;
					wsend2 = 0;
				}
			}

			for (int i = 0; i < num; i++)
			{
				swork = swork + nsend[i] + " ";
			}

			length = num;

			// 末尾の" "は不要なため削除
			return swork.Trim();
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

        #region executeCommand 1 & 2 コマンド実行して生の結果を返す

        object lockobj = new object();

        /// <summary>
        /// PLCにASCIIコマンド送信
        /// </summary>
        /// <param name="asciiCommand"></param>
        /// <returns></returns>
        public byte[] executeCommand2(byte[] bynaryCommand)
        {
            DateTime constart = DateTime.Now;

            try
            {
                //ソケット上限超えを防ぐため、ロックする。
                lock (lockobj)
                {
                    udp.Client.Blocking = true;
                    udp.Send(bynaryCommand, bynaryCommand.Length);

                    //データを受信する
                    System.Net.IPEndPoint remoteEP = null;
                    return udp.Receive(ref remoteEP);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        /// <summary>
        /// PLCにASCIIコマンド送信
        /// </summary>
        /// <param name="asciiCommand"></param>
        /// <returns></returns>
        public byte[] executeCommand1(byte[] bynaryCommand)
        {
            DateTime constart = DateTime.Now;

            ////死活監視 3連続で50msのPingに応答しない場合は応答停止
            //if (Ping(host, 50, 1) == false)
            //{
            //    return System.Text.Encoding.UTF8.GetBytes("E0");
            //}

            try
            {
                byte[] sendBytes = bynaryCommand;

                ns.Write(sendBytes, 0, sendBytes.Length);

                //サーバーから送られたデータを受信する
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize;
                do
                {
                    //データの一部を受信する
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        return new byte[0];
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
                return new byte[] { 0 };
            }
            finally
            {
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
        public byte[] GetDataAsByteArray(string hexAddressWithDeviceNM)
        {
            // コマンド例 "RD DM30001\r"
            string command = string.Format("{0} {1}\r", CommandType.RD.ToString(), hexAddressWithDeviceNM);

            // コマンドをPLC送信用に変換します。(バイト形式)
            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            // 送信用コマンドを元にPLCからデータを取得します。(空白区切りのバイト形式)
            return executeCommand2(byteCmd);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public byte[] GetDataAsByteArray(string hexAddressWithDeviceNM, int points)
        {
            // コマンド例 "RDS DM30001 1000\r"
            string command = string.Format("{0} {1} {2}\r", CommandType.RDS.ToString(), hexAddressWithDeviceNM, points.ToString());

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            return executeCommand2(byteCmd);
        }

        public byte[] GetDataAsByteArray(string hexAddressWithDeviceNM, int points, string suffix)
        {
            // コマンド例 "RDS DM30001.U 1000\r"
            string command = string.Format("{0} {1}{2} {3}\r", CommandType.RDS.ToString(), hexAddressWithDeviceNM, suffix, points.ToString());

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            return executeCommand2(byteCmd);
        }

        #endregion


        public decimal[] GetDataAsIntArray(string hexAddressWithDeviceNM, int points, string dataType)
        {
            decimal[] retV = new decimal[points];

            // -- 取得の流れ
            // ①バイトデータを取得
            // ② バイト型を文字列に変換 (アスキーコード変換) ※ 各要素は空白区切りで格納される。
            // ③ 空白区切りの文字列をInt型の配列に格納し直す。

            byte[] b;

            if (dataType == PLC.DT_BINARY)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points);
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    retV[i] = int.Parse(strlst[i]);
                }
            }
            else if (dataType == PLC.DT_DEC_32BIT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points, ".L");
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    retV[i] = Convert.ToInt32(strlst[i]);
                }
            }
            else if (dataType == PLC.DT_DEC_16BIT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points, ".S");
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    retV[i] = Convert.ToInt16(strlst[i]);
                }
            }
            else if (dataType == PLC.DT_UDEC_32BIT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points, ".D");
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    retV[i] = Convert.ToUInt32(strlst[i]);
                }
            }
            else if (dataType == PLC.DT_UDEC_16BIT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points, ".U");
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    retV[i] = Convert.ToUInt16(strlst[i]);
                }
            }
            else if (dataType == PLC.DT_HEX_32BIT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points * 2, ".H");
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                string[] s = new string[2];
                for (int i = 0; i < points; i++)
                {
                    Array.Copy(strlst, i * 2, s, 0, 2);
                    string s2 = (s[1] + s[0]).Replace("\r\n", "");
                    retV[i] = Convert.ToUInt32(s2, 16);
                }
            }
            else if (dataType == PLC.DT_HEX_16BIT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points, ".H");
                string str = Encoding.ASCII.GetString(b);
                string[] strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    string s = strlst[i].Replace("\r\n", "");
                    retV[i] = Convert.ToUInt32(s, 16);
                }
            }
            else if (dataType == PLC.DT_FLOAT)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points, ".H");
                string str = Encoding.ASCII.GetString(b);
                string[] strlst = str.Split(' ');
                string[] s = new string[2];

                for (int i = 0; i < points; i++)
                {
                    Array.Copy(strlst, i * 2, s, 0, 2);
                    string s2 = (s[1] + s[0]).Replace("\r\n", "");
                    retV[i] = Convert.ToDecimal(s2);
                }
            }
            else if (dataType == PLC.DT_FLOAT_16BIT)
            {
                throw new ApplicationException(string.Format(
                    "ﾃﾞｰﾀﾀｲﾌﾟの取得処理がソフトに実装されていません。 ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
            }
            else if (dataType == PLC.DT_BCD32BIT)
            {
                throw new ApplicationException(string.Format(
                    "ﾃﾞｰﾀﾀｲﾌﾟの取得処理がソフトに実装されていません。 ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
            }
            else if (dataType == PLC.DT_BOOL)
            {
                b = GetDataAsByteArray(hexAddressWithDeviceNM, points);
                string str = Encoding.ASCII.GetString(b);
                var strlst = str.Split(' ');
                for (int i = 0; i < points; i++)
                {
                    retV[i] = int.Parse(strlst[i]);
                }
            }
            //else if (dataType == PLC.DT_STR)
            //{
            //    return GetString(hexAddressWithDeviceNM, points, true);
            //}
            else
            {
                throw new ApplicationException(string.Format(
                    "ﾏｽﾀにﾃﾞｰﾀﾀｲﾌﾟが指定されていません ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
            }

            return retV;
        }

		public string GetDataAsString(string hexAddressWithDeviceNM, int points, string dataType)
		{
			string retv;

			if (dataType == PLC.DT_BINARY)
			{
				return GetBit(hexAddressWithDeviceNM, points);
			}
			else if (dataType == PLC.DT_DEC_32BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM, 1, ".L").ToString();
			}
			else if (dataType == PLC.DT_DEC_16BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM, 1, ".S").ToString();
			}
            else if (dataType == PLC.DT_UDEC_32BIT)
            {
                int recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 1, ".D");

                return ((uint)recvVal).ToString();
            }
            else if (dataType == PLC.DT_UDEC_16BIT)
            {
                int recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 1, ".U");
                return ((uint)recvVal).ToString();
            }
            else if (dataType == PLC.DT_HEX_32BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM, 2, ".H").ToString("X8");
			}
			else if (dataType == PLC.DT_HEX_16BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM, 1, ".H").ToString("X4");
			}
			else if (dataType == PLC.DT_FLOAT)
			{
				retv = GetWordAsFloatData(hexAddressWithDeviceNM, 2).ToString();
			}
			else if (dataType == PLC.DT_FLOAT_16BIT)
			{
				retv = GetWordAsFloatData(hexAddressWithDeviceNM, 1).ToString();
			}
			else if (dataType == PLC.DT_DOUBLE)
			{
				throw new ApplicationException(string.Format(
					"ﾃﾞｰﾀﾀｲﾌﾟの取得処理がソフトに実装されていません。 ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
			}
			else if (dataType == PLC.DT_BCD32BIT)
			{
				throw new ApplicationException(string.Format(
					"ﾃﾞｰﾀﾀｲﾌﾟの取得処理がソフトに実装されていません。 ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
			}
			else if (dataType == PLC.DT_BOOL)
			{
				string bit = GetBit(hexAddressWithDeviceNM, points);

				if (bit == BIT_ON.ToString())
				{
					retv = "TRUE";
				}
				else if (bit == BIT_OFF.ToString())
				{
					retv = "FALSE";
				}
				else
				{
					throw new ApplicationException(string.Format(
						"PLCから予期せぬ応答を取得しました 応答内容:{0} ｱﾄﾞﾚｽ:{1} 長さ:{2} ﾃﾞｰﾀﾀｲﾌﾟ:{3}", bit, hexAddressWithDeviceNM, points, dataType));
				}
			}
			else if (dataType == PLC.DT_STR)
			{
                //retv = GetWord(hexAddressWithDeviceNM, points);
                retv = GetString(hexAddressWithDeviceNM, points, false, false);
            }
			else
			{
				throw new ApplicationException(string.Format(
					"ﾏｽﾀにﾃﾞｰﾀﾀｲﾌﾟが指定されていません ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
			}

			if (IsExcludePlcValue(dataType, retv))
			{
				return PLC.OUTPUT_NULL_Str;
			}
			else
			{
				return retv;
			}
		}

		/// <summary>
		/// PLCからの取得データが除外対象値かどうかチェック
		/// </summary>
		/// <param name="dataType">PLCからの取得データのデータの種類</param>
		/// <param name="getData">PLCからの取得値</param>
		/// <returns></returns>
		private bool IsExcludePlcValue(string dataType, string getData)
		{
			if (ExtractExclusionList == null || ExtractExclusionList.Count == 0)
			{
				return false;
			}

			//dataType指定で抽出除外設定されているか確認
			if (ExtractExclusionList.Exists(e => e.DataType == dataType))
			{
				List<EICS.Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList = ExtractExclusionList.Where(e => e.DataType == dataType).ToList();

				foreach (EICS.Machine.PLCDDGBasedMachine.ExtractExclusion exclusion in exclusionList)
				{
					if (exclusion.ExceptionValue == getData)
					{
						return true;
					}
				}
				return false;
			}//dataTypeの指定無しで抽出除外設定されているか確認
			else if (ExtractExclusionList.Exists(e => e.DataType == string.Empty))
			{
				List<EICS.Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList = ExtractExclusionList.Where(e => e.DataType == string.Empty).ToList();

				foreach (EICS.Machine.PLCDDGBasedMachine.ExtractExclusion exclusion in exclusionList)
				{
					if (exclusion.ExceptionValue == getData)
					{
						return true;
					}
				}
				return false;
			}
			else
			{
				return false;
			}
		}
        /// <summary>
        /// 不正応答時は空白
        /// </summary>
        /// <param name="hexAddressWithDeviceNm"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided, int wordLength)
        {
            string qr = string.Empty;

            string org = this.GetBit(hexAddressWithDeviceNm, wordLength);
            if (org == BIT_READ_TIMEOUT_VALUE)
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
        public string GetMagazineNo(string qr, bool notDevided)
        {
            //string qr = string.Empty;

            //foreach (string orgs in plcResponseBitArray)
            //{
            //    qr += System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(int.Parse(orgs)));
            //}

            //Null文字を置換
            qr = qr.Replace("\0", "");
            string[] elms = qr.Trim().Split(' ');

            if (elms.Length == 2 || (elms.Length >= 3 && notDevided))
            {
                return elms[1];
            }
            //else if (elms.Length >= 3)
            //{
            //    string magno = elms[1];
            //    int seqno = int.Parse(elms[2]);

            //    magno = Order.NascaLotToMagLot(magno, seqno);
            //    return magno;
            //}
            else
            {
                return qr.Trim();
            }
        }

        public string GetMagazineNo(string hexAddressWithDeviceNm, int wordLength)
        {
            return GetMagazineNo(hexAddressWithDeviceNm, true, wordLength);
        }

        #region GetWordsAsDateTime

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>不正応答時はNULL</returns>
        public DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM)
        {
            string sendCommand = string.Format("RD {0} {1}\r", hexAddressWithDeviceNM, 6);
            ns = tcp.GetStream();

            byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
            ns.Write(sendBytes, 0, sendBytes.Length);

            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            byte[] resBytes = new byte[256];
            int resSize;
            try
            {
                do
                {
                    resSize = ns.Read(resBytes, 0, resBytes.Length);
                    if (resSize == 0)
                    {
                        throw new Exception();
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (ns.DataAvailable);

                string[] res = System.Text.Encoding.UTF8.GetString(ms.ToArray()).Replace("\r\n", "").Split(' ');

                try
                {
                    int year = int.Parse(res[0]);
                    int month = int.Parse(res[1]);
                    int day = int.Parse(res[2]);
                    int hour = int.Parse(res[3]);
                    int minute = int.Parse(res[4]);
                    int sec = int.Parse(res[5]);

                    return new DateTime(year, month, day, hour, minute, sec);
                }
                catch
                {
                    throw new ApplicationException($"日時取得失敗 アドレス:{hexAddressWithDeviceNM}");
                }
            }
            finally
            {
                if (ms != null) { ms.Close(); }
            }
        }

        #endregion
    }
}
