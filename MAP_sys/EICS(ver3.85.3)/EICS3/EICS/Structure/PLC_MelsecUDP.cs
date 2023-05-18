using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Structure
{
	public class PLC_MelsecUDP : IDisposable, IPlc
	{
		public static string PLC_TYPE = "MELSEC_UDP";

		private UdpClient udp;

		private List<EICS.Machine.PLCDDGBasedMachine.ExtractExclusion> ExtractExclusionList = new List<Machine.PLCDDGBasedMachine.ExtractExclusion>();

		public string IPAddress { get; set; }
		public int Port { get; set; }

		static object lockobj = new object();

		private const int MAX_DEVICE_READ_POINTS = 600;

		private const int HEADER_BYTE_SIZE = 9;
		private const int DATALEN_START_INDEX = 7;

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public PLC_MelsecUDP(string address, int port, List<Machine.PLCDDGBasedMachine.ExtractExclusion> exclusionList)
		{
			this.udp = new UdpClient(address, port);
			this.udp.Client.ReceiveTimeout = 1000;

			this.IPAddress = address;
			this.Port = port;

			this.ExtractExclusionList = exclusionList;
		}

		#region IDisposable
		/// <summary>
		/// デストラクタ
		/// </summary>
		~PLC_MelsecUDP()
		{
			//アンマネージリソースの解放
			this.Dispose(false);
		}

		/// <summary>
		/// Disposeされたかどうか
		/// </summary>
		private bool disposed;

		protected void Dispose(bool disposing)
		{
			lock (this)
			{
				if (this.disposed)
				{
					//既に呼びだしずみであるならばなんもしない
					return;
				}

				this.disposed = true;

				if (disposing)
				{
					// マネージリソースの解放
				}

				// アンマネージリソースの解放
				udp.Close();
			}
		}

		public void Dispose()
		{
			//マネージリソースおよびアンマネージリソースの解放
			this.Dispose(true);

			//デストラクタを対象外とする
			GC.SuppressFinalize(this);
		}
		#endregion

		#region 定数
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

		/// <summary>
		/// 装置番号WORDアドレスの長さ
		/// </summary>
		public const int MACHINE_NO_WORD_LENGTH = 6;

		/// <summary>
		/// タイムアウト時応答
		/// </summary>
		public const string BIT_READ_TIMEOUT_VALUE = "TIMEOUT";

		public static string[] BIT_READ_TIMEOUT_VALUE_ARRAY = new string[] { "TIMEOUT" };

		#endregion

		#region Command Header

		private static byte[] commandHeader = new byte[] { 0x50, 0x00, 0x00, 0xFF, 0xFF, 0x03, 0x00 };
		private static byte[] responseHeader3E = new byte[] { 0xD0, 0x00, 0x00, 0xFF, 0xFF, 0x03, 0x00 };

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

		private const int RESPONSE_HEADER_LENGTH = 7;

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
			if(ExtractExclusionList.Exists(e => e.DataType == dataType))
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
				retv = GetWordAsFloatData(hexAddressWithDeviceNM, 4).ToString();
			}
			else if (dataType == PLC.DT_BCD32BIT)
			{
				retv = GetDoubleWordAsBCD(hexAddressWithDeviceNM).ToString();
			}
            else if (dataType == PLC.DT_BCD16BIT)
            {
                retv = GetWordAsBCD(hexAddressWithDeviceNM, 1).ToString();
            }
            else if (dataType == PLC.DT_BOOL)
			{
                //string bit = GetBit(hexAddressWithDeviceNM, points);
                string bit = GetBit(hexAddressWithDeviceNM);

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

		#region convertAddressToByteArray

		private byte[] convertAddressToByteArray(string hexAddressWithDeviceNM)
		{
			string header, address;

			if (hexAddressWithDeviceNM.Length == 7)
			{
				address = hexAddressWithDeviceNM.Substring(1).PadLeft(6, '0');
				header = hexAddressWithDeviceNM[0].ToString();
			}
			else if (hexAddressWithDeviceNM.Length == 8)
			{
				address = hexAddressWithDeviceNM.Substring(2);
				header = hexAddressWithDeviceNM.Substring(0, 2);
			}
			else
			{
				throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM + "\r\n\t" + "先頭のメモリ識別子が1桁の場合は7桁、先頭のメモリ識別子が2桁の場合は8桁で設定して下さい。");
			}

			// アドレスの先頭一文字を除いて6桁
			List<byte> retv = new List<byte>();

			for (int i = 2; i <= address.Length; i += 2)
			{
				string sub = address.Substring(address.Length - i, 2);
				retv.Add(Convert.ToByte(sub, 16));
			}

			switch (header)
			{
				case "B":
					retv.Add(Convert.ToByte("A0", 16));
					break;

				case "W":
					retv.Add(Convert.ToByte("B4", 16));
					break;

				case "M":
					retv.Add(Convert.ToByte("90", 16));
					break;

				case "D":
					retv.Add(Convert.ToByte("A8", 16));
					break;

				case "R":
					retv.Add(Convert.ToByte("AF", 16));
					break;

				case "ZR":
					retv.Add(Convert.ToByte("B0", 16));
					break;

                case "L":
                    retv.Add(Convert.ToByte("92", 16));
                    break;

                default:
					throw new ApplicationException("デバイス名が不正です:" + hexAddressWithDeviceNM[0]);
			}

			return retv.ToArray();
		}
		#endregion

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

		private bool checkResponseHeader3E(byte[] plcResponseData)
		{
			byte[] responseHeader = new byte[RESPONSE_HEADER_LENGTH];
			Array.Copy(plcResponseData, 0, responseHeader, 0, RESPONSE_HEADER_LENGTH);

			for (int i = 0; i < responseHeader.Length; i++)
			{
				if (responseHeader[i] != responseHeader3E[i])
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

		public byte[] executeCommand(byte[] bynaryCommand)
		{
			return executeCommand(bynaryCommand, 0);
		}

		public byte[] executeCommand(byte[] bynaryCommand, int tryCt)
		{
			byte[] sendBytes = bynaryCommand;
			byte[] recv;

			try
			{
				//ソケット上限超えを防ぐため、ロックする。
				lock (lockobj)
				{
					System.Net.IPEndPoint remoteEP = null;

					udp.Client.Blocking = true;

					bool needSendCommandFg;

					DateTime firstSendDt = DateTime.Now;
					TimeSpan totalTimeSpan = DateTime.Now - firstSendDt;

					if (udp.Available > 0)
					{
						udp.Receive(ref remoteEP);
					}

					needSendCommandFg = false;
					udp.Send(sendBytes, sendBytes.Length);

					DateTime sendDt = DateTime.Now;
					TimeSpan ts = DateTime.Now - sendDt;

					//ｻﾌﾞﾍｯﾀﾞ＋ｱｸｾｽ経路＋応答ﾃﾞｰﾀ長のサイズがバッファに溜まるまで待機
					//ｻﾌﾞﾍｯﾀﾞ：2byte ｱｸｾｽ経路：5byte　応答ﾃﾞｰﾀ長：2byte

					while (udp.Available < HEADER_BYTE_SIZE)
					{
						Thread.Sleep(10);
						ts = DateTime.Now - sendDt;

						if (ts.TotalMilliseconds > 4000)
						{
							string recvStr = string.Empty;
							if (udp.Available > 0)
							{
								recv = udp.Receive(ref remoteEP);

								foreach (byte bd in recv)
								{
									recvStr += bd.ToString("X2") + " ";
								}
							}

							throw new ApplicationException(
								string.Format("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無くタイムアウト(4000ms)しました。受信ﾊﾞｯﾌｧｻｲｽﾞ:{0} 受信ﾊﾞｯﾌｧﾃﾞｰﾀ:{1}"
								, udp.Available, recvStr));
						}
					}

					recv = udp.Receive(ref remoteEP);

					if (checkResponseHeader3E(recv) == false)
					{
						string headerStr = string.Empty;
						foreach(byte headerByte in responseHeader3E)
						{
							headerStr += headerByte.ToString("X2") + " ";
						}

						string recvStr = string.Empty;
						foreach(byte recvByte in recv)
						{
							recvStr += recvByte.ToString("X2") + " ";
						}

						throw new ApplicationException(string.Format(
							"ﾍｯﾀﾞが合致しないﾃﾞｰﾀを受信しました。ﾍｯﾀﾞ定義:{0} 受信ﾃﾞｰﾀ:{1}", headerStr, recvStr));
					}

					int dataLen = BitConverter.ToInt16(recv, DATALEN_START_INDEX);

					int totalRecvByteSize = HEADER_BYTE_SIZE + dataLen;
					int residualRecvByteSize = totalRecvByteSize - recv.Length;

					//必要なデータサイズに達していれば、受信内容を返す
					if (residualRecvByteSize == 0)
					{
						return recv;
					}

					//残りデータサイズに達するまで待機
					while (udp.Available != residualRecvByteSize)
					{

						ts = DateTime.Now - sendDt;
						System.Threading.Thread.Sleep(10);

						if (ts.TotalMilliseconds > 6000)
						{
							string recvStr = string.Empty;
							if (udp.Available > 0)
							{
								recv = udp.Receive(ref remoteEP);

								foreach (byte bd in recv)
								{
									recvStr += bd.ToString("X2") + " ";
								}
							}

							throw new ApplicationException(string.Format(
								"PLCへのコマンド送信後、残りのデータの受信の為、待機しましたがタイムアウト(6000ms)しました。 受信ﾊﾞｯﾌｧｻｲｽﾞ:{0} 残りﾃﾞｰﾀｻｲｽﾞ:{1} 受信ﾊﾞｯﾌｧﾃﾞｰﾀ:{2}"
								, udp.Available, residualRecvByteSize, recvStr));
						}
					}

					List<byte> recvList = recv.ToList();

					recvList.AddRange(udp.Receive(ref remoteEP).ToList());

					return recvList.ToArray();
				}
			}
			catch (ApplicationException err)
			{
				if (tryCt < 1)
				{
					string logMsg = string.Format("ｺﾏﾝﾄﾞ送信後、4000ms応答が無い為、ｺﾏﾝﾄﾞ再送");
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

					return executeCommand(bynaryCommand, ++tryCt);
				}
				else
				{
					throw;
				}
			}
		}


		//public byte[] executeCommand(byte[] bynaryCommand)
		//{
		//	return executeCommand(bynaryCommand, true);
		//}

		//private byte[] executeCommand(byte[] bynaryCommand, bool canRetry)
		//{
		//	byte[] sendBytes = bynaryCommand;

		//	try
		//	{
		//		//ソケット上限超えを防ぐため、ロックする。
		//		lock (lockobj)
		//		{
		//			System.Net.IPEndPoint remoteEP = null;

		//			udp.Client.Blocking = true;
		//			//udp.Send(sendBytes, sendBytes.Length);

		//			////データを受信する
		//			//return udp.Receive(ref remoteEP);

		//			bool needSendCommandFg;

		//			DateTime firstSendDt = DateTime.Now;
		//			TimeSpan totalTimeSpan = DateTime.Now - firstSendDt;

		//			do
		//			{
		//				while (udp.Available > 0)
		//				{
		//					udp.Receive(ref remoteEP);
		//				}

		//				needSendCommandFg = false;
		//				udp.Send(sendBytes, sendBytes.Length);

		//				DateTime sendDt = DateTime.Now;
		//				TimeSpan ts = DateTime.Now - sendDt;

		//				//受信データがなければ300ms待機した後、コマンド再送
		//				while (udp.Available <= 0)
		//				{
		//					if (ts.TotalMilliseconds < 30)
		//					{
		//						ts = DateTime.Now - sendDt;
		//						System.Threading.Thread.Sleep(10);
		//					}
		//					else
		//					{
		//						needSendCommandFg = true;
		//						Thread.Sleep(300);
		//						break;
		//					}
		//				}

		//				//コマンド再送するも、タイムアウト時間を越えた場合
		//				totalTimeSpan = DateTime.Now - firstSendDt;
		//				if (needSendCommandFg && totalTimeSpan.TotalMilliseconds > 4000)
		//				{
		//					throw new ApplicationException("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無くタイムアウト(4000ms)しました。");
		//				}
		//			} while (needSendCommandFg);

		//			//データを受信する
		//			return udp.Receive(ref remoteEP);
		//		}
		//	}
		//	catch (Exception ex)
		//	{
		//		if (canRetry)
		//		{
		//			return executeCommand(bynaryCommand, false);
		//		}

		//		throw ex;
		//	}
		//}
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
		private string parseWordResponseData(byte[] data)
		{
			try
			{
				if (isExitNormal(data))
				{
					List<byte> outputdata = new List<byte>();

					for (int i = RESPONSE_POS_DATA_START; i < data.Length; i++)
					{
						outputdata.Add(data[i]);
					}
					return Encoding.ASCII.GetString(outputdata.ToArray());
				}
				else
				{
					throw new ApplicationException("PLC異常応答");
				}
			}
			catch (Exception ex)
			{
				//Log.RBLog.Error("PLC WORD Parse error" + ex.ToString());
				throw new ApplicationException("PLC WORD Parse error");
			}
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
                //データ領域が2バイトか4バイトかで分岐。どちらでもなければ異常終了。
                if (data.Length == RESPONSE_POS_DATA_START + 2)
                {
                    //正常終了
                    output = BitConverter.ToInt16(data, RESPONSE_POS_DATA_START);
                }
                else if (data.Length == RESPONSE_POS_DATA_START + 4)
                {
                    //正常終了
                    output = BitConverter.ToInt32(data, RESPONSE_POS_DATA_START);
                }

				
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
			string output;
			bool isSuccess = parseBitResponseData(response, points, out output);
			if (isSuccess == true)
			{
				//Log.SysLog.Info("[SetBit]Address:" + hexAddressWithDeviceNM + " " + data);

				return;
			}
			else
			{
				//異常終了
				//Log.RBLog.Error("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}
		#endregion

		#region GetBit
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns></returns>
		public string GetBit(string hexAddressWithDeviceNM)
		{
			if (isBitKind(hexAddressWithDeviceNM) || !hexAddressWithDeviceNM.Contains('.'))
			{
				return GetBit(hexAddressWithDeviceNM, 1);
			}
			else
			{
				return GetBitByChannel(hexAddressWithDeviceNM);
			}
			//return GetBit(hexAddressWithDeviceNM, 1);
		}

		public string GetBitByChannel(string hexAddressWithDeviceNM)
		{
			string[] memAddr = hexAddressWithDeviceNM.Split('.');

			byte bitNum = Convert.ToByte(memAddr[1], 10);

			string dataMemAddr = memAddr[0];

			byte[] byteData = GetWordRaw(dataMemAddr, 1);

			int decData;
			if (byteData.Length == 1)
			{
                decData = (int)byteData[0];
            }
			else if (byteData.Length == 2)
			{
                decData = (int)GetWordAsDecimalData(dataMemAddr, 1);
			}
			else
			{
				string byteStr = string.Empty;

				foreach (byte bd in byteData)
				{
					byteStr += bd.ToString() + ", ";
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
					, hexAddressWithDeviceNM + "(ﾊﾞｲﾄ長):" + byteData.Length + ", ﾊﾞｲﾄﾃﾞｰﾀ：" + byteStr);

				throw new ApplicationException("想定外のバイト長(3バイト以上)データをPLCから受信しました。");
			}

            string binaryStr = Convert.ToString(decData, 2).PadLeft(16, '0');
            
			return binaryStr.Substring(binaryStr.Length - bitNum, 1);
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

		/// <summary>PLCメモリアドレスがビット領域のデータかどうかを返す</summary>
		/// <param name="addressWithDeviceNm">デバイス識別から始まるメモリアドレス(W000001、B000001)</param>
		/// <returns>ビット領域:true/非ビット領域:false</returns>
		private bool isBitKind(string addressWithDeviceNm)
		{
			string addr = addressWithDeviceNm.ToUpper();

			if(addr.StartsWith("LSTS"))
			{
				return true;
			}
			else if(addr.StartsWith("LSTC"))
			{
				return true;
			}
			else if (addr.StartsWith("LTS"))
			{
				return true;
			}
			else if (addr.StartsWith("LTC"))
			{
				return true;
			}
			else if (addr.StartsWith("STS"))
			{
				return true;
			}
			else if (addr.StartsWith("STC"))
			{
				return true;
			}
			else if (addr.StartsWith("LCS"))
			{
				return true;
			}
			else if (addr.StartsWith("LCC"))
			{
				return true;
			}
			else if (addr.StartsWith("FX"))
			{
				return true;
			}
			else if (addr.StartsWith("FY"))
			{
				return true;
			}
			else if (addr.StartsWith("SM"))
			{
				return true;
			}
			else if (addr.StartsWith("TS"))
			{
				return true;
			}
			else if (addr.StartsWith("TC"))
			{
				return true;
			}
			else if (addr.StartsWith("CS"))
			{
				return true;
			}
			else if (addr.StartsWith("CC"))
			{
				return true;
			}
			else if (addr.StartsWith("SB"))
			{
				return true;
			}
			else if (addr.StartsWith("DX"))
			{
				return true;
			}
			else if (addr.StartsWith("DY"))
			{
				return true;
			}
			else if (addr.StartsWith("X"))
			{
				return true;
			}
			else if (addr.StartsWith("Y"))
			{
				return true;
			}
			else if (addr.StartsWith("M"))
			{
				return true;
			}
			else if (addr.StartsWith("L"))
			{
				return true;
			}
			else if (addr.StartsWith("F"))
			{
				return true;
			}
			else if (addr.StartsWith("V"))
			{
				return true;
			}
			else if (addr.StartsWith("B"))
			{
				return true;
			}
			else if (addr.StartsWith("S"))
			{
				return true;
            }
            else
			{
				return false;
			}

		}

		public string GetBit(string hexAddressWithDeviceNM, int length)
		{
			return GetBit(hexAddressWithDeviceNM, length, 0);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns></returns>
		public string GetBit(string hexAddressWithDeviceNM, int length, int tryCt)
		{
			byte[] subCommand;
			byte[] response = null;
			byte[] cmd = null;
			string errCd = string.Empty;

			try
			{
				bool isBitMemKind = isBitKind(hexAddressWithDeviceNM);

				if (isBitMemKind)
				{
					subCommand = subCommandBitRW;
				}
				else
				{
					subCommand = subCommandWordRW;
				}

				cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommand, length, null);

				//PLCコマンド実行
				response = executeCommand(cmd);
				string outputstring;

				bool isSuccess;

				if (isBitMemKind)
				{
					isSuccess = parseBitResponseData(response, length, out outputstring);
				}
				else
				{
					int output;
					isSuccess = parseWordResponseDataAsDecimalData(response, out output);
					outputstring = output.ToString();
				}

				if (isSuccess == true)
				{
					//Log.SysLog.Info("[GetBit]Address:" + hexAddressWithDeviceNM + " " + outputstring);

					return outputstring;
				}
				else
				{
					//異常終了
					if (response.Length >= 11)
					{
						errCd = string.Format("異常CD受信上位ﾊﾞｲﾄ:{0}/受信下位ﾊﾞｲﾄ:{1}", response[9], response[10]);
					}
					//Log.RBLog.Error("PLC異常応答:" + response);
					throw new ApplicationException(string.Format("PLC異常応答 異常CD:{0}", errCd));
				}
			}
			catch (Exception err)
			{
				if (tryCt >= 3)
				{
					string cmdStr = string.Empty;
					string resStr = string.Empty;

					if (cmd != null)
					{
						foreach (byte cmdByte in cmd)
						{
							cmdStr += string.Format("{0} ", cmdByte.ToString("X2"));
						}
					}

					if (response != null)
					{
						foreach (byte resByte in response)
						{
							resStr += string.Format("{0} ", resByte.ToString("X2"));
						}
					}

					if (response.Length >= 11)
					{
						errCd = string.Format("異常CD受信上位ﾊﾞｲﾄ:{0}/受信下位ﾊﾞｲﾄ:{1}", response[9], response[10]);
					}

					string logMsg = string.Format("GetBit異常応答 発生時：addr:{0} Length:{1}\r\ncmd:{2} 受信ﾃﾞｰﾀ:{3} PLC異常CD:{4}", hexAddressWithDeviceNM, length, cmdStr, resStr, errCd);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
					throw;
				}
				else
				{
					Thread.Sleep(50);
					return GetBit(hexAddressWithDeviceNM, length, ++tryCt);
				}
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

		#region GetWordsAsDateTime

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns>不正応答時はNULL</returns>
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
				try
				{
					string retv = GetBit(hexAddressWithDeviceNM);

					if (retv == exitValue)
					{
						return true;
					}

					if (timeout > 0)
					{
						if ((DateTime.Now - start).TotalSeconds >= timeout)
						{
							return false;
						}
					}

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
			return parseWordResponseData(response);
		}

		#endregion

		#region GetWordAsDecimalData
		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns>タイムアウト時　-1</returns>
		public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength, int tryCt)
		{
			byte[] response = null;
			byte[] cmd = null;
			string errCd = string.Empty;
			try
			{
				cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, wordLength, null);

				//PLCコマンド実行
				response = executeCommand(cmd);
				int output;
				bool isSuccess = parseWordResponseDataAsDecimalData(response, out output);
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
			catch (Exception err)
			{
				if (tryCt >= 3)
				{
					string cmdStr = string.Empty;
					string resStr = string.Empty;

					if (cmd != null)
					{
						foreach (byte cmdByte in cmd)
						{
							cmdStr += string.Format("{0} ", cmdByte.ToString("X2"));
						}
					}

					if (response != null)
					{
						foreach (byte resByte in response)
						{
							resStr += string.Format("{0} ", resByte.ToString("X2"));
						}
					}

					if (response.Length >= 11)
					{
						errCd = string.Format("異常CD受信上位ﾊﾞｲﾄ:{0}/受信下位ﾊﾞｲﾄ:{1}", response[9], response[10]);
					}

					string logMsg = string.Format("GetWordAsDecimalData異常応答 発生時：addr:{0} Length:{1}\r\ncmd:{2} 受信ﾃﾞｰﾀ:{3} PLC異常CD:{4}", hexAddressWithDeviceNM, wordLength, cmdStr, resStr, errCd);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
					throw;
				}
				else
				{
					Thread.Sleep(50);
					return GetWordAsDecimalData(hexAddressWithDeviceNM, wordLength, ++tryCt);
				}
			}
		}

		public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength)
		{
			return GetWordAsDecimalData(hexAddressWithDeviceNM, wordLength, 0);
		}

		public int GetWordAsDecimalData(string hexAddressWithDeviceNM)
		{
			return GetWordAsDecimalData(hexAddressWithDeviceNM, 1, 0);
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

        /// <summary>
        /// BCD形式(4bit毎に数値一桁を表す形式)のデータをint型に変換する。
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="byteLength"></param>
        /// <returns></returns>
        public int GetWordAsBCD(string hexAddressWithDeviceNM, int byteLength)
        {
            int calcResult = 0;
            int halfByteLen = 4;//変換する桁数
        
            byte[] byteArray = GetWordRaw(hexAddressWithDeviceNM, byteLength);

            int digitCt = 0;

            foreach (byte byteData in byteArray)
            {
                string binStr = Convert.ToString(byteData, 2).PadLeft(8, '0');

                //1byte(8bit分)毎に値が取れるが、BCDは4bit毎の変換が必要なので、forﾙｰﾌﾟ2回に分ける
                for (int i = 0; i < 2; i++)
                {
                    //8bit分の文字列中、下位ビットに該当する側から取得する為、indexの計算を(transDigit * (1 - i))としている→i=0で、ｲﾝﾃﾞｸｽ4～7の文字を取得
                    string halfByte = binStr.Substring(halfByteLen * (1 - i), halfByteLen);

                    int valOfBCDDigit = 0;//BCDの一桁毎の数値（BCDの一桁とは変換前の2進4bit分の塊の事）

                    for (int bitDigit = 0; bitDigit < halfByteLen; bitDigit++)
                    {
                        int targetDigitOfBin = halfByteLen - (bitDigit + 1);
                        valOfBCDDigit += Convert.ToInt32(halfByte[targetDigitOfBin].ToString()) * (int)Math.Pow(2, bitDigit); //4bitの塊を10進に変換
                    }

                    //int value = Convert.ToInt32(binStr.Substring(transDigit * (1 - i), transDigit));
                    //int value = Convert.ToInt32(binStr);

                    calcResult += valOfBCDDigit * (int)Math.Pow(10, digitCt + i);
                }
                digitCt += 2;
            }

            return calcResult;
        }

        public int GetDoubleWordAsBCD(string hexAddressWithDeviceNM)
		{
			int calcResult = 0;
			int halfByteLen = 4;//変換する桁数

			byte[] byteArray = GetWordRaw(hexAddressWithDeviceNM, 2);

			int digitCt = 0;

			foreach (byte byteData in byteArray)
			{
				string binStr = Convert.ToString(byteData, 2).PadLeft(8, '0');

				//1byte(8bit分)毎に値が取れるが、BCDは4bit毎の変換が必要なので、forﾙｰﾌﾟ2回に分ける
				for (int i = 0; i < 2; i++)
				{
					//8bit分の文字列中、下位ビットに該当する側から取得する為、indexの計算を(transDigit * (1 - i))としている→i=0で、ｲﾝﾃﾞｸｽ4～7の文字を取得
					string halfByte = binStr.Substring(halfByteLen * (1 - i), halfByteLen);

					int valOfBCDDigit = 0;//BCDの一桁毎の数値（BCDの一桁とは変換前の2進4bit分の塊の事）

					for (int bitDigit = 0; bitDigit < halfByteLen; bitDigit++)
					{
						int targetDigitOfBin = halfByteLen - (bitDigit + 1);
						valOfBCDDigit += Convert.ToInt32(halfByte[targetDigitOfBin].ToString()) * (int)Math.Pow(2, bitDigit); //4bitの塊を10進に変換
					}
					
					//int value = Convert.ToInt32(binStr.Substring(transDigit * (1 - i), transDigit));
					//int value = Convert.ToInt32(binStr);

					calcResult += valOfBCDDigit * (int)Math.Pow(10, digitCt + i);
				}
				digitCt += 2;
			}

			return calcResult;
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
#if DEBUG || TEST
			return;
#endif

			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWrite, subCommandWordRW, 1, BitConverter.GetBytes(Convert.ToInt16(data)));

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
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
				//Log.RBLog.Error("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
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
					//Log.RBLog.Error("PLCエラー応答:" + response);
					throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
				}
			}
			catch (Exception ex)
			{
				//Log.RBLog.Error("日付データへの変換に失敗:res=" + response);
				throw ex;
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
            
            //EXT文字を置換
            qr = qr.Replace(((char)0x03).ToString(), "");

            if (qr.Trim().Split(' ').Length >= 2)
            {
                return qr.Trim().Split(' ')[1];
            }
            else
            {
                return qr.Trim();
            }

            //return qr.Trim();

		}

		#endregion

		/// <summary>
		/// デバイス名付きのアドレスを指定数分だけ移動
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <param name="shiftct">移動数（10進）</param>
		/// <returns></returns>
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

        /// <summary>
        /// 連続したアドレスの情報を文字列として取得
        /// </summary>
        /// <param name="hexAddressWithDeviceNm"></param>
        /// <param name="wordLength"></param>
        /// <param name="removePrefix">半角スペースより前を取り除く(半角スペース複数個は非対応)</param>
        /// <param name="host"></param>
        /// <returns></returns>
        public string GetString(string hexAddressWithDeviceNm, int wordLength)
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
				char[] temp = new char[wordLength];
				for (int i = 0; i <= wordLength / 2; i += 2)
				{
					temp[i] = org[i + 1];
					temp[i + 1] = org[i];
				}
				if (wordLength % 2 != 0)
				{
					temp[wordLength - 1] = org[wordLength];
				}

				str = string.Join(string.Empty, temp);
			}
			else
			{
				str = org.Replace("\0", "").Replace("\r", "").Replace("\n", "");
			}
			return str;
		}

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

    }
}
