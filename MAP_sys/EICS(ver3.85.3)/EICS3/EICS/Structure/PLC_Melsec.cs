using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using EICS.Structure;

namespace EICS
{
	public class PLC_Melsec : IDisposable, IPlc
	{
		private const int WORDSIZE = 2;

		public static string PLC_TYPE = "MELSEC";

        public string HostAddressNO { get; set; }
        public int PortNO { get; set; }

		private List<EICS.Machine.PLCDDGBasedMachine.ExtractExclusion> ExtractExclusionList = new List<Machine.PLCDDGBasedMachine.ExtractExclusion>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public PLC_Melsec(string hostAddressNO, int portNO, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList) 
        {
			this.HostAddressNO = hostAddressNO;
			this.PortNO = portNO;

            this.tcp = new System.Net.Sockets.TcpClient(hostAddressNO, portNO);

			this.ExtractExclusionList = exclusionList;

        }

        ~PLC_Melsec()
        {
            Dispose();
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

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns></returns>
		public bool GetBool(string hexAddressWithDeviceNM)
		{
			string recvStr = GetBit(hexAddressWithDeviceNM, 1);

			if (recvStr == BIT_ON)
			{
				return true;
			}
			else if (recvStr == BIT_OFF)
			{
				return false;
			}
			else
			{
				throw new ApplicationException(string.Format("PLCからのデータ受信で問題発生 PLCメモリ：{0} 受信データ：{1}", hexAddressWithDeviceNM, recvStr));
			}
		}

        public bool ConnectedPLC()
        {
            return tcp.Connected;
        }

        public void ConnectPLC()
        {
            tcp = new System.Net.Sockets.TcpClient(HostAddressNO, PortNO);
        }

        public int InstanceNo { get; set; }

        public string host { get; set; }

        /// <summary>
        /// PLCのBIT型ONの場合の値
        /// </summary>
        public const string BIT_ON = "1";

        /// <summary>
        /// PLCのBIT型OFFの場合の値
        /// </summary>
        public const string BIT_OFF = "0";

        /// <summary>
        /// マガジン番号WORDアドレスの長さ
        /// </summary>
        public const int MAGAZINE_NO_WORD_LENGTH = 6;

		private const int MAX_DEVICE_READ_POINTS = 600;
        /// <summary>
        /// タイムアウト時応答
        /// </summary>
        public const string BIT_READ_TIMEOUT_VALUE = "TIMEOUT";

        /// <summary>
        /// TCPソケット
        /// </summary>
        private System.Net.Sockets.TcpClient tcp;

        /// <summary>
        /// ネットワークストリーム
        /// </summary>
        private System.Net.Sockets.NetworkStream ns = null;

        #region Command Header

        private static byte[] commandHeader = new byte[] { 0x50, 0x00, 0x00, 0xFF, 0xFF, 0x03, 0x00 };

        /// <summary>
        /// CPU待ち時間
        /// </summary>
        byte[] cputimer = new byte[] { 0x10, 0x00 };

        /// <summary>
        /// 読出し用
        /// </summary>
        private static byte[] commandRead = new byte[] { 0x01, 0x04 };

        /// <summary>
        /// 書き込み用
        /// </summary>
        private static byte[] commandWrite = new byte[] { 0x01, 0x14 };

        /// <summary>
        /// BIT単位
        /// </summary>
        /// 
        byte[] subCommandBitRW = new byte[] { 0x01, 0x00 };

        /// <summary>
        /// WORD単位
        /// </summary>
        byte[] subCommandWordRW = new byte[] { 0x00, 0x00 };


        /// <summary>
        /// PLC応答 正常終了コード
        /// </summary>
        byte[] responseExitCodeNormal = new byte[] { 0x00, 0x00 };

        /// <summary>
        /// PLC応答 終了コード位置
        /// </summary>
        private const int RESPONSE_POS_EXIT_CODE_START = 9;

        /// <summary>
        /// PLC応答 終了コード長さ
        /// </summary>
        private const int RESPONSE_POS_EXIT_CODE_LENGTH = 2;

        /// <summary>
        /// PLC応答 実データ開始位置
        /// </summary>
        private const int RESPONSE_POS_DATA_START = 11;

        #endregion

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

        #region convertAddressToByteArray

        private byte[] convertAddressToByteArray(string hexAddressWithDeviceNM)
        {
            string address;
            
            List<byte> retv = new List<byte>();

            if (hexAddressWithDeviceNM[0] == 'D')
            {
                if (hexAddressWithDeviceNM.Length != 5)
                {
                    throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
                }
                int iaddress = Convert.ToInt32(hexAddressWithDeviceNM.Substring(1));

                address = iaddress.ToString("X4").PadLeft(6, '0');

                for (int i = 2; i <= address.Length; i += 2)
                {
                    retv.Add(Convert.ToByte(address.Substring(address.Length -i, 2), 16));
                }
            }
            else
            {
                if (hexAddressWithDeviceNM.Length != 7)
                {
                    throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
                }
                // アドレスの先頭一文字を除いて6桁
                address = hexAddressWithDeviceNM.Substring(1).PadLeft(6, '0');

                for (int i = 2; i <= address.Length; i += 2)
                {
                    string sub = address.Substring(address.Length - i, 2);
                    retv.Add(Convert.ToByte(sub, 16));
                }
            }

            switch (hexAddressWithDeviceNM[0])
            {
                case 'B':
                    retv.Add(Convert.ToByte("A0", 16));
                    break;

                case 'W':
                    retv.Add(Convert.ToByte("B4", 16));
                    break;

                case 'M':
                    retv.Add(Convert.ToByte("90", 16));
                    break;

                case 'D':
                    retv.Add(Convert.ToByte("A8", 16));
                    break;

                default:
                    throw new ApplicationException("デバイス名が不正です:" + hexAddressWithDeviceNM[0]);
            }

            return retv.ToArray();
        }
        #endregion

		public bool SetString(string hexAddressWithDeviceNM, string data)
		{
			return SetString(hexAddressWithDeviceNM, data, "ASCII");
		}

        /// <summary>
        /// 
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

			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWrite, subCommandWordRW, byteStr.Count() / 2, byteStr);
			string logStr = string.Empty;
			foreach (byte cmdbyte in cmd)
			{
				logStr += Convert.ToString(cmdbyte);
			}
			Console.WriteLine(string.Format("送信コマンド(EICS⇒PLC):{0}", logStr));
			logStr = string.Empty;
			foreach (byte cmdbyte in byteStr)
			{
				logStr += Convert.ToString(cmdbyte);
			}
			Console.WriteLine(string.Format("送信文字列:{0}", logStr));

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);

			System.Threading.Thread.Sleep(100);

			string output;

			//応答解析はBitとして処理
			bool isSuccess = parseBitResponseData(response, 1, out output);
			if (isSuccess == true)
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
        /// PLC応答から終了コード取得
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        private bool isExitNormal(byte[] plcResponseData)
        {
            byte[] exitcode = new byte[RESPONSE_POS_EXIT_CODE_LENGTH];
            Array.Copy(plcResponseData, RESPONSE_POS_EXIT_CODE_START, exitcode, 0, RESPONSE_POS_EXIT_CODE_LENGTH);

            for (int i = 0; i < exitcode.Length; i++)
            {
                if (exitcode[i] != responseExitCodeNormal[i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 1バイトの16進表記上位下位を1アドレスとするデータに変換
        /// 元データが奇数の場合は最後に0x0を付け足す
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private static byte[] parseStringDataToPLCBitData(string s)
        {
            if (string.IsNullOrEmpty(s) == true)
            {
                return new byte[0];
            }

            List<byte> retv = new List<byte>();

            for (int i = 0; i <= s.Length; i += 2)
            {
                if (s.Length == (i + 1))
                {
                    retv.Add(Convert.ToByte(s.Substring(i, 1) + "0", 16));
                }
                else
                {
                    retv.Add(Convert.ToByte(s.Substring(i, 2), 16));
                }
            }

            return retv.ToArray();
        }

        #region createCommand
        /// <summary>
        /// PLC送信用コマンド作成
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="command">READ/WRITE</param>
        /// <param name="subCommand">BIT/WORD</param>
        /// <param name="points"></param>
        /// <param name="data">読込時はNULL</param>
        /// <returns></returns>
        private byte[] createCommand(string hexAddressWithDeviceNM, byte[] mainCommand, byte[] subCommand, int points, byte[] writeData)
        {
            List<byte> command2 = new List<byte>();
            command2.AddRange(cputimer);
            command2.AddRange(mainCommand);
            command2.AddRange(subCommand);
            command2.AddRange(convertAddressToByteArray(hexAddressWithDeviceNM));
            command2.AddRange(BitConverter.GetBytes(Convert.ToInt16(points)));
            if (writeData != null)
            {
                command2.AddRange(writeData);
            }

            List<byte> retv = new List<byte>();
            retv.AddRange(commandHeader);
            Int16 cmd2Length = Convert.ToInt16(command2.Count);
            retv.AddRange(BitConverter.GetBytes(cmd2Length));
            retv.AddRange(command2);

            return retv.ToArray();
        }

        #endregion

        #region executeCommand コマンド実行して生の結果を返す

        /// <summary>
        /// PLCにASCIIコマンド送信
        /// </summary>
        /// <param name="asciiCommand"></param>
        /// <returns></returns>
        public byte[] executeCommand(byte[] bynaryCommand)
        {
            TcpClient currentTcp = tcp;
            NetworkStream currentNS = null;

            try
            {
                //currentMutex.WaitOne();
                currentNS = currentTcp.GetStream();

                byte[] sendBytes = bynaryCommand;

                currentNS.Write(sendBytes, 0, sendBytes.Length);

                //サーバーから送られたデータを受信する
                System.IO.MemoryStream ms = new System.IO.MemoryStream();
                byte[] resBytes = new byte[256];
                int resSize;
                do
                {
                    //データの一部を受信する
                    resSize = currentNS.Read(resBytes, 0, resBytes.Length);
                    //Readが0を返した時はサーバーが切断したと判断
                    if (resSize == 0)
                    {
                        return new byte[0];
                    }
                    ms.Write(resBytes, 0, resSize);
                } while (currentNS.DataAvailable);

                byte[] res = ms.ToArray();
                ms.Close();

                return res;
            }
            finally
            {
            }
        }
        #endregion

        #region parseBitResponseData

        /// <summary>
        /// PLC返信データを解析
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool parseBitResponseData(byte[] data, int length, out string output)
        {
            output = "";

            if (isExitNormal(data))
            {
                if (length == 1)
                {
                    if (data.Length <= RESPONSE_POS_DATA_START)
                    {
                        output = "0";
                        return true;
                    }

                    //1点を要求した場合は16または0が帰ってくる
                    //MELSECの仕様
                    if (data[RESPONSE_POS_DATA_START] >= 15)
                    {
                        output = "1";
                        return true;
                    }
                    else
                    {
                        output = "0";
                        return true;
                    }
                }
                else
                {
                    for (int i = RESPONSE_POS_DATA_START; i < data.Length; i++)
                    {
                        output += Convert.ToString(data[i], 16).PadLeft(2, '0');
                    }

                    if (length < output.Length)
                    {
                        output = output.Substring(0, length);
                    }
                }
                return true;
            }

            //異常終了
            return false;
        }
        #endregion

        #region parseWordResponseData

        /// <summary>
        /// PLC返信データを解析
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool parseWordResponseData(byte[] data, out string output)
        {
            output = "";

            if (isExitNormal(data))
            {
                List<byte> outputdata = new List<byte>();

                for (int i = RESPONSE_POS_DATA_START; i < data.Length; i++)
                {
                    outputdata.Add(data[i]);
                }
                output = Encoding.ASCII.GetString(outputdata.ToArray());

                return true;
            }

            //異常終了
            output = "";
            return false;
        }
        #endregion

        #region parseWordResponseDataAsDecimalData

        /// <summary>
        /// PLC返信データを解析
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool parseWordResponseDataAsDecimalData(byte[] data, out int output)
        {
            output = 0;


            if (isExitNormal(data))
            {
                //正常終了
                output = BitConverter.ToInt16(data, RESPONSE_POS_DATA_START);
                return true;
            }

            //異常終了
            output = 0;
            return false;
        }

        /// <summary>
        /// PLC返信データを解析(Int16かInt32を選択して変換)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool parseWordResponseDataAsDecimalData(byte[] data, out int output, int convertBit)
        {
            output = 0;


            if (isExitNormal(data))
            {
                switch(convertBit)
                {
                    case 16:
                    //正常終了
                        output = BitConverter.ToInt16(data, RESPONSE_POS_DATA_START);
                        return true;

                    case 32:
                        output = BitConverter.ToInt32(data, RESPONSE_POS_DATA_START);
                        return true;

                    default:
                        throw new Exception(Constant.MessageInfo.Message_123);
                }
            }

            //異常終了
            output = 0;
            return false;
        }

		/// <summary>
		/// PLC返信データを解析(double)
		/// </summary>
		/// <param name="data"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		private bool parseWordResponseDataAsDecimalData(byte[] data, out double output)
		{
			output = 0;


			if (isExitNormal(data))
			{
				//正常終了
				output = BitConverter.ToDouble(data, RESPONSE_POS_DATA_START);
				return true;
			}

			//異常終了
			output = 0;
			return false;
		}

		/// <summary>
		/// PLC返信データを解析
		/// </summary>
		/// <param name="data"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		private bool parseWordResponseDataAsSingleData(byte[] data, out float output)
		{
			output = 0;


			if (isExitNormal(data))
			{
				//正常終了
				output = BitConverter.ToSingle(data, RESPONSE_POS_DATA_START);
				return true;
			}

			//異常終了
			output = 0;
			return false;
		}

		/// <summary>
		/// PLC返信データを解析
		/// </summary>
		/// <param name="data"></param>
		/// <param name="output"></param>
		/// <returns></returns>
		private bool parseWordResponseDataAsDoubleData(byte[] data, out double output)
		{
			output = 0;

			if (isExitNormal(data))
			{
				//正常終了
				output = BitConverter.ToDouble(data, RESPONSE_POS_DATA_START);
				return true;
			}

			//異常終了
			output = 0;
			return false;
		}
        /// <summary>
        /// PLC返信データを解析
        /// </summary>
        /// <param name="data"></param>
        /// <param name="output"></param>
        /// <returns></returns>
        private bool parseWordResponseDataAsDateTime(byte[] data, out int output)
        {
            output = 0;

            byte[] doubleByte = new byte[2];


            if (isExitNormal(data))
            {
                //正常終了
                output = Convert.ToInt32(data[RESPONSE_POS_DATA_START]);
                output += Convert.ToInt32(data[RESPONSE_POS_DATA_START + 1]) * 16 * 16;
                return true;
            }

            //異常終了
            output = 0;
            return false;
        }

        #endregion

        #region ByteStringToAscii
        /// <summary>
        /// HexASCIIコード文字列を2文字単位で文字に変換
        /// </summary>
        /// <param name="bytesString"></param>
        /// <returns></returns>
        public string BytesStringToAscii(string bytesString)
        {
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

        #endregion

        #region SetBit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SetBit(string hexAddressWithDeviceNM, int points, string data)
        {
            byte[] writeData = parseStringDataToPLCBitData(data);
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWrite, subCommandBitRW, points, writeData);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
			System.Threading.Thread.Sleep(100);
            string output;
            bool isSuccess = parseBitResponseData(response, points, out output);
            if (isSuccess == true)
            {
                return;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return;
            }
        }
        #endregion

        #region GetBit
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public string GetBit(string hexAddressWithDeviceNM)
        {
            return GetBit(hexAddressWithDeviceNM, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public string GetBit(string hexAddressWithDeviceNM, int length)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandBitRW, length, null);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            string outputstring;
            bool isSuccess = parseBitResponseData(response, length, out outputstring);
            if (isSuccess == true)
            {
                return outputstring;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return BIT_READ_TIMEOUT_VALUE;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <param name="points"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		public string[] GetBitArray(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandBitRW, length, null);

			byte[] res = executeCommand(cmd);

			if (isExitNormal(res) == true)
			{
				return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
			}
			else
			{
				//異常終了
				//Log.WriteError("PLCエラー応答:" + command);
				return PLC_MelsecUDP.BIT_READ_TIMEOUT_VALUE_ARRAY;
			}
		}

        #endregion

        #region WatchBit

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="timeout">タイムアウトまでの秒数（sec) 0または負なら無限ループ</param>
        /// <param name="exitValue">監視を抜ける条件</param>
        /// <returns>タイムアウト時false</returns>
        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue)
        {

            DateTime start = DateTime.Now;
            while (true)
            {
                string retv = GetBit(hexAddressWithDeviceNM);

                if (retv == PLC.BIT_READ_TIMEOUT_VALUE)
                {
                    //Log.WriteError("PLC通信エラー発生　監視継続");
                    Thread.Sleep(10000); //10秒待機
                }
                else
                {
                    if (retv == exitValue)
                    {
                        return true;
                    }
                }

                if (timeout > 0)
                {
                    if ((DateTime.Now - start).Seconds >= timeout)
                    {
                        return false;
                    }
                }

                //待機中はMutexを開放
                Thread.Sleep(50);
            }

        }
        #endregion

        #region GetWord
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public string GetWord(string hexAddressWithDeviceNM)
        {
            return GetWord(hexAddressWithDeviceNM, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public string GetWord(string hexAddressWithDeviceNM, int length)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);


            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            string outputstring;
            bool isSuccess = parseWordResponseData(response, out outputstring);
            if (isSuccess == true)
            {
                return outputstring;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return BIT_READ_TIMEOUT_VALUE;
            }
        }

		//public string GetRawData(string hexAddressWithDeviceNM, bool isLittleEndian)
		//{
		//    return GetRawData(hexAddressWithDeviceNM, 1, isLittleEndian);
		//}

		//public string GetRawData(string hexAddressWithDeviceNM, int length, bool isLittleEndian)
		//{
		//    byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);


		//    //PLCコマンド実行
		//    byte[] response = executeCommand(cmd);
		//    string outputstring = string.Empty;

		//    int startIndex, endIndex;
		//    int addVal = 1;

		//    if (isLittleEndian)
		//    {
		//        startIndex = response.Length;
		//        endIndex = RESPONSE_POS_DATA_START;
		//        addVal = -1;
		//    }
		//    else
		//    {
		//        startIndex = RESPONSE_POS_DATA_START;
		//        endIndex = response.Length;
		//    }

		//    if (isExitNormal(response))
		//    {
		//        for (int i = startIndex; i < endIndex; i+= addVal)
		//        {
		//            outputstring += response[i].ToString("D2");
		//        }

		//        return outputstring;
		//    }
		//    else
		//    {
		//        //異常終了
		//        //Log.WriteError("PLCエラー応答:" + response);
		//        return BIT_READ_TIMEOUT_VALUE;
		//    }            
		//}

		public string GetHex(string hexAddressWithDeviceNM)
		{
			return GetHexArray(hexAddressWithDeviceNM, 1)[0];
		}

		public string[] GetHexArray(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			List<string> outputstring = new List<string>();

			if (isExitNormal(response))
			{
				for (int i = RESPONSE_POS_DATA_START; i < RESPONSE_POS_DATA_START + length; i++)
				{
					outputstring.Add(response[i].ToString("XX"));
				}

				return outputstring.ToArray();
			}
			else
			{
				outputstring = new List<string>();

				outputstring.Add(BIT_READ_TIMEOUT_VALUE);
				//異常終了
				//Log.WriteError("PLCエラー応答:" + response);
				return outputstring.ToArray();
			}
		}

        #endregion

        #region GetWordAsDecimalData
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
		public int GetWordAsDecimalData(string hexAddressWithDeviceNM)
		{
			return GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
		}

        public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, wordLength, null);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            int output;
            bool isSuccess = parseWordResponseDataAsDecimalData(response, out output);
            if (isSuccess == true)
            {
                return output;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return 0;
            }
        }

		public float GetWordAsFloatData(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			float output;
			bool isSuccess = parseWordResponseDataAsSingleData(response, out output);
			if (isSuccess == true)
			{
				return output;
			}
			else
			{
				//異常終了
				//Log.RBLog.Error("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}

		public double GetWordAsDoubleData(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			double output;
			bool isSuccess = parseWordResponseDataAsDoubleData(response, out output);
			if (isSuccess == true)
			{
				return output;
			}
			else
			{
				//異常終了
				//Log.RBLog.Error("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}

		public float GetDobleWordAsFloatData(string hexAddressWithDeviceNM)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, 2, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			float output;
			bool isSuccess = parseWordResponseDataAsSingleData(response, out output);
			if (isSuccess == true)
			{
				return output;
			}
			else
			{
				//異常終了
				//Log.RBLog.Error("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}

        public double GetDoubleWordAsDoubleData(string hexAddressWithDeviceNM, int length)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            float output;
            bool isSuccess = parseWordResponseDataAsSingleData(response, out output);
            if (isSuccess == true)
            {
                return output;
            }
            else
            {
                //異常終了
                //Log.RBLog.Error("PLC異常応答:" + response);
                throw new ApplicationException("PLC異常応答");
            }
        }

		public int GetDoubleWordAsBCD(string hexAddressWithDeviceNM)
		{
			int calcResult = 0;
			int transDigit = 4;//変換する桁数

			byte[] byteArray = GetWordRaw(hexAddressWithDeviceNM, 2);

			int digitCt = 0;

			foreach (byte byteData in byteArray)
			{
				string binStr = Convert.ToString(byteData, 2).PadLeft(8, '0');

				for (int i = 0; i < 2; i++)
				{
					//8bit分の文字列中、下位ビットに該当する側から取得する為、indexの計算を(transDigit * (1 - i))としている
					int value = Convert.ToInt32(binStr.Substring(transDigit * (1 - i), transDigit));

					calcResult += value * (int)Math.Pow(10, digitCt + i);
				}
				digitCt += 2;
			}

			return calcResult;
		}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public int GetDoubleWordAsDecimalData(string hexAddressWithDeviceNM)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, 2, null);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            int output;
            bool isSuccess = parseWordResponseDataAsDecimalData(response, out output, 32);
            if (isSuccess == true)
            {
                return output;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return 0;
            }
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns>タイムアウト時　-1</returns>
        //public double GetWordAsDoubleData(string hexAddressWithDeviceNM)
        //{
        //    int output1, output2;

        //    byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, 1, null);

        //    //PLCコマンド実行
        //    byte[] response = executeCommand(cmd);
        //    bool isSuccess1 = parseWordResponseDataAsDecimalData(response, out output1);

        //    response = executeCommand(cmd);
        //    cmd = createCommand(GetNextAddress(hexAddressWithDeviceNM), commandRead, subCommandWordRW, 1, null);

        //    bool isSuccess2 = parseWordResponseDataAsDecimalData(response, out output2);

        //    if (isSuccess1 == true && isSuccess2 == true)
        //    {
        //        //output1,output2の順番に気をつける 2013/8/9 n.yoshimoto
        //        List<byte> byteList = new List<byte>();
        //        byteList.Add(Convert.ToByte(output1)); 
        //        byteList.Add(Convert.ToByte(output2));

        //        return BitConverter.ToDouble(byteList.ToArray(), 0);
        //    }
        //    else
        //    {
        //        //異常終了
        //        //Log.WriteError("PLCエラー応答:" + response);
        //        return 0;
        //    }
        //}

		public string ShiftAddress(string hexAddressWithDeviceNM, int shiftct)
		{
			//ヘッダー部とアドレス部（16進変換）に分割
			string header, hexaddress;

			if (hexAddressWithDeviceNM.Length == 7)
			{
				hexaddress = hexAddressWithDeviceNM.Substring(1).PadLeft(6, '0');
				header = hexAddressWithDeviceNM[0].ToString();
			}
			else if (hexAddressWithDeviceNM.Length == 8)
			{
				hexaddress = hexAddressWithDeviceNM.Substring(2);
				header = hexAddressWithDeviceNM.Substring(0, 2);
			}
			else
			{
				throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
			}


			//addressは10進
			long address = Convert.ToInt64(hexaddress, 16);
			address += shiftct;
			return header + address.ToString("X6");
		}

		#region GetWordRaw
		public byte[] GetWordRaw(string hexAddressWithDeviceNM, int length)
		{
			#region 最大読取点数を超える場合はループ実行
			if (length > MAX_DEVICE_READ_POINTS)
			{
				List<byte> retv = new List<byte>();

				string currentAddress = hexAddressWithDeviceNM;

				//最大読取点数分だけループしてコマンド実行
				for (int i = 0; i < length / MAX_DEVICE_READ_POINTS; i++)
				{
					retv.AddRange(GetWordRaw(currentAddress, MAX_DEVICE_READ_POINTS));
					currentAddress = ShiftAddress(hexAddressWithDeviceNM, MAX_DEVICE_READ_POINTS);
				}

				//余りがある場合は最後の読取点からあまり分だけ追加で読取
				if (length % MAX_DEVICE_READ_POINTS > 0)
				{
					retv.AddRange(GetWordRaw(currentAddress, length % MAX_DEVICE_READ_POINTS));
				}

				return retv.ToArray();
			}
			#endregion

			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, length, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			if (isExitNormal(response))
			{
				byte[] output = new byte[response.Length - RESPONSE_POS_DATA_START];
				Array.Copy(response, RESPONSE_POS_DATA_START, output, 0, output.Length);

				return output;
			}
			else
			{
				throw new ApplicationException("PLCエラー応答発生");
			}
		}

		#endregion

        public string GetNextAddress(string hexAddressWithDeviceNM)
        {
            string address = hexAddressWithDeviceNM;

            if (hexAddressWithDeviceNM[0] == 'D')
            {
                int iaddress = Convert.ToInt32(hexAddressWithDeviceNM.Substring(1)) + 1;
                address = string.Format("D{0}",iaddress);
            }
            else if (hexAddressWithDeviceNM[0] == 'B' || hexAddressWithDeviceNM[0] == 'W' || hexAddressWithDeviceNM[0] == 'M')
            {
                int iaddress = Convert.ToInt32(hexAddressWithDeviceNM.Substring(1), 16) + 1;
                address = string.Format("{0}{1}", hexAddressWithDeviceNM[0], iaddress.ToString("X6"));
            }

            return address;
        }

        #endregion

        #region SetWordAsDecimalData

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWrite, subCommandWordRW, 1, BitConverter.GetBytes(Convert.ToInt16(data)));

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
			System.Threading.Thread.Sleep(100);
            string output;

            //応答解析はBITとして処理
            bool isSuccess = parseBitResponseData(response, 1, out output);
            if (isSuccess == true)
            {
                return;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return;
            }
        }
        #endregion

        #region GetWordsAsDateTime

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, 6, null);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);

            try
            {
                if (isExitNormal(response))
                {
                    if (response.Length != 23)
                    {
                        throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
                    }
                    int year = BitConverter.ToInt16(response, RESPONSE_POS_DATA_START);
                    int month = BitConverter.ToInt16(response, RESPONSE_POS_DATA_START + 2);
                    int day = BitConverter.ToInt16(response, RESPONSE_POS_DATA_START + 4);
                    int hour = BitConverter.ToInt16(response, RESPONSE_POS_DATA_START + 6);
                    int minute = BitConverter.ToInt16(response, RESPONSE_POS_DATA_START + 8);
                    int second = BitConverter.ToInt16(response, RESPONSE_POS_DATA_START + 10);

                    return new DateTime(year, month, day, hour, minute, second);
                }
                else
                {
                    //異常終了
                    //Log.WriteError("PLCエラー応答:" + response);
                    throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
                }
            }
            catch (Exception ex)
            {
                //Log.WriteError("日付データへの変換に失敗:res=" + response);
                throw ex;
            }
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
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM, 2).ToString();
			}
			else if (dataType == PLC.DT_DEC_32BIT_REV)
			{
				retv = GetDoubleWordAsDecimalData(hexAddressWithDeviceNM).ToString();
			}
			else if (dataType == PLC.DT_DEC_16BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM).ToString();
			}
			else if (dataType == PLC.DT_HEX_32BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM, 2).ToString("X8");
			}
			else if (dataType == PLC.DT_HEX_16BIT)
			{
				retv = GetWordAsDecimalData(hexAddressWithDeviceNM).ToString("X4");
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
                retv = GetWordAsDoubleData(hexAddressWithDeviceNM, 4).ToString();
			}
			else if (dataType == PLC.DT_BCD32BIT)
			{
				retv = GetDoubleWordAsBCD(hexAddressWithDeviceNM).ToString();
			}
			else if (dataType == PLC.DT_BOOL)
			{
				string bit = GetBit(hexAddressWithDeviceNM, points);

				if (bit == BIT_ON)
				{
					retv = "TRUE";
				}
				else if (bit == BIT_OFF)
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
				retv = GetWord(hexAddressWithDeviceNM, points);
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

        #endregion

        #region GetMagazineNo

        public string GetMagazineNo(string hexAddressWithDeviceNm, int magazineNoDeviceLen)
        {
            string qr = string.Empty;

			string org = this.GetWord(hexAddressWithDeviceNm, magazineNoDeviceLen).Trim();
            qr = org;

            //Null文字を置換
            qr = qr.Replace("\0", "").Replace("\"", "").Replace("\r", "");

			return qr.Trim();

			//if (qr.Trim().Split(' ').Length >= 2)
			//{
			//	return qr.Trim().Split(' ')[1].Replace("","");
			//}
			//else
			//{
			//	return qr.Trim();
			//}

        }

        #endregion


        /// <summary>
        /// 連続したアドレスの情報を文字列として取得
        /// ※引数isBigEndianで上位下位反転させる機能は未実装
        /// </summary>
        /// <param name="hexAddressWithDeviceNm"></param>
        /// <param name="wordLength"></param>
        /// <param name="removePrefix">半角スペースより前を取り除く(半角スペース複数個は非対応)</param>
        /// <param name="host"></param>
        /// <returns></returns>
        public string GetString(string hexAddressWithDeviceNm, int wordLength, bool isBigEndian)
		{
			string retv = string.Empty;

			string[] org = this.GetBitArray(hexAddressWithDeviceNm, wordLength);

			foreach (string s in org)
			{
				retv += System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(int.Parse(s)));
			}

			//Null文字を置換
			retv = retv.Replace("\0", "");
			return retv;
		}

        /// <summary>
        /// 連続したアドレスの情報を文字列として取得
        /// ※引数isBigEndianで上位下位反転させる機能は未実装
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="wordLength"></param>
        /// <param name="doSwapFg"></param>
        /// <param name="isBigEndian"></param>
        /// <returns></returns>
        public string GetString(string hexAddressWithDeviceNM, int wordLength, bool doSwapFg, bool isBigEndian)
		{
			string str;

			string logMsg = string.Format("GetString() addr:{0} Length:{1}", hexAddressWithDeviceNM, wordLength);
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

			string org = this.GetWord(hexAddressWithDeviceNM, wordLength).Trim();

			if (doSwapFg)
			{
				int tempLen = wordLength;
				if (wordLength % WORDSIZE != 0)
				{
					//奇数要素での初期化にならないように偶数にする
					tempLen++;
				}

				char[] temp = new char[tempLen];
				for (int i = 0; i < wordLength; i += WORDSIZE)
				{
					temp[i] = org[i + 1];
					temp[i + 1] = org[i];
				}
				if (wordLength % WORDSIZE != 0)
				{
					temp[wordLength - 1] = org[wordLength];
				}

				str = string.Join(string.Empty, temp);
			}
			else
			{
				str = org;
			}
			return str.Replace("\0", "").Replace("\r", "").Replace("\n", "");

		}

        /// <summary>
        /// PLCコマンド受付状態
        /// </summary>
        /// <returns></returns>
        public bool GetBitState(string address)
        {
            //string retv = GetBit(STAT_ROBOT_COMMAND_READY);
			string retv = GetBit(address);

            if (retv == BIT_READ_TIMEOUT_VALUE)
            {
                return false;
            }
            else
            {
                if (retv == BIT_ON)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
