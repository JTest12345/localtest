using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace PLC
{
	public class Keyence : IPlc, IDisposable
	{
		private static Mutex mutex = new Mutex(false, "PLC_Mutex");

		/// <summary>
		/// PLCのBIT型ONの場合の値
		/// </summary>
		public const string BIT_ON = "1";

		/// <summary>
		/// PLCのBIT型OFFの場合の値
		/// </summary>
		public const string BIT_OFF = "0";

		public const int SENDABLE_LENGTH_AT_ONCE = 1000;

		/// <summary>
		/// TCPクライアント
		/// </summary>
		private System.Net.Sockets.TcpClient tcp;
		//private static System.Net.Sockets.UdpClient udp;

		/// <summary>
		/// ネットワークストリーム
		/// </summary>
		private static System.Net.Sockets.NetworkStream ns = null;

		public string HostAddressNO { get; set; }
		public int PortNO { get; set; }

		//private enum CommandType
		//{
		//	RD,
		//	RDS,
		//	WR,
		//	WRS,
		//}

		//上位リンク KEYENCEコマンド サフィックス
		public const string Suffix_U = ".U";                     //上位リンク 10進数16ビット符号 なし ｼｰｹﾝｻ側EM*****は16ビット
		public const string Suffix_S = ".S";                     //上位リンク 10進数16ビット符号 あり
		public const string Suffix_H = ".H";                     //上位リンク 16進数16ビット
		public const string Suffix_D = ".D";                     //上位リンク 10進数32ビット符号 なし ｼｰｹﾝｻ側EM*****は16ビットなので、注意が必要
		public const string Suffix_L = ".L";                     //上位リンク 10進数32ビット符号 あり

		public Keyence(string ipAddress, int port)
		{
			this.HostAddressNO = ipAddress;
			this.PortNO = port;
		}

		public void ConnectPLC()
		{
			if (tcp != null && tcp.Connected)
			{
				return;
			}

			tcp = new System.Net.Sockets.TcpClient(this.HostAddressNO, this.PortNO);
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

		public bool GetBitForAI_MD(string memoryAddressNO)
		{
			if (string.IsNullOrEmpty(memoryAddressNO))
			{
				throw new ApplicationException(string.Format("アドレスが指定されていません。読込アドレス:{0}", memoryAddressNO));
			}

			string data;
			string sendCommand = string.Format("RD {0}\r", memoryAddressNO);

			ConnectPLC();

			mutex.WaitOne();
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

				int intRecvData;

				if (int.TryParse(data, out intRecvData) == false)
				{
					return false;
				}
				if (intRecvData == 1)
				{
					return true;
				}
				else if (intRecvData == 0)
				{
					return false;
				}
				else
				{
					throw new ApplicationException(
						string.Format("装置から想定していないデータを取得しました。アドレス：{0}/データ：{1}", memoryAddressNO, data));
				}

			}
			finally
			{
				mutex.ReleaseMutex();
				if (ms != null) { ms.Close(); }
			}
		}

		public bool GetBit(string memoryAddressNO)
		{
			if (string.IsNullOrEmpty(memoryAddressNO))
			{
				throw new ApplicationException(string.Format("アドレスが指定されていません。読込アドレス:{0}", memoryAddressNO));
			}

			string data;
			string sendCommand = string.Format("RD {0}\r", memoryAddressNO);

			ConnectPLC();

			mutex.WaitOne();
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

				int intRecvData;

				if (int.TryParse(data, out intRecvData) == false)
				{
					throw new ApplicationException(
						string.Format("装置から想定していないデータを取得しました。アドレス：{0}/データ：{1}", memoryAddressNO, data));
				}
				if (intRecvData == 1)
				{
					return true;
				}
				else if (intRecvData == 0)
				{
					return false;
				}
				else
				{
					throw new ApplicationException(
						string.Format("装置から想定していないデータを取得しました。アドレス：{0}/データ：{1}", memoryAddressNO, data));
				}

			}
			finally
			{
				mutex.ReleaseMutex();
				if (ms != null) { ms.Close(); }
			}
		}

		public void SetBit(string memoryAddressNO, bool statusFG)
		{
			if (string.IsNullOrEmpty(memoryAddressNO))
			{
				throw new ApplicationException(string.Format("アドレスが指定されていません。書込アドレス:{0}", memoryAddressNO));
			}

			string sendCommand = string.Format("WRS {0}{1} {2} {3}\r",
				memoryAddressNO, Suffix_U, 1, Convert.ToInt16(statusFG));

			ConnectPLC();

			mutex.WaitOne();
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

				//return System.Text.Encoding.UTF8.GetString(ms.ToArray());
			}
			finally
			{
				mutex.ReleaseMutex();
				if (ms != null) { ms.Close(); }
			}
		}

		public void SendMultiValue(string memoryAddress, string[] valueArray, string suffix)
		{
			int[] intArray = new int[valueArray.Length];

			for (int i = 0; i < intArray.Length; i++)
			{
				if (int.TryParse(valueArray[i], out intArray[i]) == false)
				{
					throw new ApplicationException(string.Format("マッピングデータ(シリンジ別)を数値変換出来ませんでした。書き込みアドレス:{0} / アドレス:{1} / 変換元データ:{2}", memoryAddress, i, valueArray[i]));
				}
			}

			SendMultiValue(memoryAddress, intArray, suffix);
		}

		public void SendMultiValue(string memoryAddress, int[] valueArray, string suffix)
		{

			if (string.IsNullOrEmpty(memoryAddress))
			{
				throw new ApplicationException(string.Format("アドレスが指定されていません。書込アドレス:{0}", memoryAddress));
			}

			ConnectPLC();

			int memoryAddressNo = int.Parse(Regex.Replace(memoryAddress, "[^0-9]", string.Empty));
			string addressKind = memoryAddress.Replace(memoryAddressNo.ToString(), string.Empty);

			while (valueArray.Length > 0)
			{
				int takeLength = SENDABLE_LENGTH_AT_ONCE;
				if (valueArray.Length < takeLength)
				{
					takeLength = valueArray.Length;
				}

				string sendData = string.Join(" ", valueArray.Take(takeLength));

				valueArray = valueArray.Skip(takeLength).ToArray();

				string sendCommand = string.Format("WRS {0}{1} {2} {3}\r", memoryAddress, suffix, takeLength, sendData);

				memoryAddressNo += takeLength;
				memoryAddress = string.Format("{0}{1}", addressKind, memoryAddressNo);

				mutex.WaitOne();
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

					//return System.Text.Encoding.UTF8.GetString(ms.ToArray());
				}
				finally
				{
					mutex.ReleaseMutex();
					if (ms != null) { ms.Close(); }
				}
			}
		}

		public string Send1ByteData(string memoryAddressNO, int sendByteData)
		{
			int length;
			string sendData;

			sendData = sendByteData.ToString("X2");

			string sendCommand = string.Format("WRS {0}{1} {2} {3}\r", memoryAddressNO, Suffix_U, 1, sendData);

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
			if (string.IsNullOrEmpty(memoryAddressNO))
			{
				throw new ApplicationException(string.Format("アドレスが指定されていません。書込アドレス:{0}", memoryAddressNO));
			}

			int length;

			sendData = GetSendMsg_WRS(sendData, true, out length);

			string sendCommand = string.Format("WRS {0}{1} {2} {3}\r",
				memoryAddressNO, Suffix_U, length, sendData);

			ConnectPLC();

			mutex.WaitOne();
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
				mutex.ReleaseMutex();
				if (ms != null) { ms.Close(); }
			}
		}

		public string GetData(string Addr, int num)
		{
			return GetData(Addr, num, string.Empty, false);
		}

		public string GetData(string memoryAddressNO, int length, string suffix, bool isConvB2A)
		{
			if (string.IsNullOrEmpty(memoryAddressNO))
			{
				throw new ApplicationException(string.Format("アドレスが指定されていません。読込アドレス:{0}", memoryAddressNO));
			}

			string dataStr;
			string sendCommand = string.Format("RDS {0}{1} {2}\r", memoryAddressNO, suffix, length);

			ConnectPLC();

			mutex.WaitOne();
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
				mutex.ReleaseMutex();
				if (ms != null) { ms.Close(); }
			}
		}

		public string GetData(string memoryAddressNO, int length, string suffix)
		{
			return GetData(memoryAddressNO, length, suffix, false);
		}

		//public string Send1ByteData(string memoryAddressNO, int sendByteData)
		//{
		//	int length;
		//	string sendData;

		//	sendData = sendByteData.ToString("X2");

		//	string sendCommand = string.Format("WRS {0}{1} {2} {3}\r", memoryAddressNO, ssuffix_H, 1, sendData);

		//	ns = tcp.GetStream();
		//	byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
		//	ns.Write(sendBytes, 0, sendBytes.Length);

		//	System.IO.MemoryStream ms = new System.IO.MemoryStream();
		//	byte[] resBytes = new byte[10000];
		//	try
		//	{
		//		do
		//		{
		//			int resSize = ns.Read(resBytes, 0, resBytes.Length);
		//			if (resSize == 0)
		//			{
		//				throw new Exception();
		//			}
		//			ms.Write(resBytes, 0, resSize);
		//		} while (ns.DataAvailable);

		//		return System.Text.Encoding.UTF8.GetString(ms.ToArray());
		//	}
		//	finally
		//	{
		//		if (ms != null) { ms.Close(); }
		//	}
		//}

		//public string GetWord(string memoryAddressNO, int length)
		//{
		//	string sendCommand = string.Format("RDS {0}{1} {2}\r", memoryAddressNO, ssuffix_U, length);
		//	ns = tcp.GetStream();

		//	byte[] sendBytes = System.Text.Encoding.UTF8.GetBytes(sendCommand);
		//	ns.Write(sendBytes, 0, sendBytes.Length);

		//	System.IO.MemoryStream ms = new System.IO.MemoryStream();
		//	byte[] resBytes = new byte[256];
		//	int resSize;
		//	try
		//	{
		//		do
		//		{
		//			resSize = ns.Read(resBytes, 0, resBytes.Length);
		//			if (resSize == 0)
		//			{
		//				throw new ApplicationException();
		//			}
		//			ms.Write(resBytes, 0, resSize);
		//		} while (ns.DataAvailable);

		//		return System.Text.Encoding.UTF8.GetString(ms.ToArray()).Trim();
		//		//return Convert.ToBoolean();
		//	}
		//	catch (IOException ex)//基になっている Socket が閉じています。
		//	{
		//		//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
		//		//転送接続からデータを読み取れません:既存の接続はリモートﾎｽﾄに強制的に切断されました。
		//		return "Error";
		//	}
		//	catch (ObjectDisposedException ex)
		//	{
		//		//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, ex.Message);
		//		return "Error";
		//	}
		//	finally
		//	{
		//		if (ms != null) { ms.Close(); }
		//	}
		//}

		///// <summary>
		///// PLC応答から終了コード取得
		///// </summary>
		///// <param name="responseData"></param>
		///// <returns></returns>
		//private bool isExitNormal(byte[] plcResponseData)
		//{
		//	/*
		//	string res = System.Text.Encoding.UTF8.GetString(plcResponseData).Trim();
		//	{
		//		switch (res)
		//		{
		//			case "E0":
		//				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_109, res));
		//				break;
		//			case "E1":
		//				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_110, res));
		//				break;
		//			case "E4":
		//				throw new ApplicationException(string.Format(Constant.MessageInfo.Message_111, res));
		//				break;
		//			default:
		//				return true;
		//				break;
		//		}
		//	}
		//	 */
		//	throw new NotImplementedException();
		//}



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
		private string GetSendMsg_WRS(string sData, bool isLittleEndian, out int length)
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
			//swork = swork + " ";

			length = num;

			// 末尾の" "は不要なため削除
			return swork.Trim();
		}

		private string BytesStringToAscii(string bytesString)
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
	}
}
