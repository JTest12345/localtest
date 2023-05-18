using ArmsApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ArmsWorkTransparency.Model.PLC
{
	/// <summary>
	/// オムロンPLC UDP接続 Binary通信
	/// </summary>
	public class Omron : IDisposable, IPLC
	{
		private UdpClient udp;

		public string IPAddress { get; set; }
		public int Port { get; set; }
		public string HostIPAddress { get; set; }

		static object lockobj = new object();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		public Omron(string address, int port, string host)
		{
			this.udp = new UdpClient(address, port);
			this.udp.Client.ReceiveTimeout = 1000;

			this.IPAddress = address;
			this.Port = port;
			this.HostIPAddress = host;
		}

		#region IDisposable
		/// <summary>
		/// デストラクタ
		/// </summary>
		~Omron()
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
		public const int MAGAZINE_NO_WORD_LENGTH = 8;

		/// <summary>
		/// PLC応答 実データ開始位置
		/// </summary>
		private const int RESPONSE_POS_DATA_START = 14;

		/// <summary>
		/// PLC応答 終了コード位置
		/// </summary>
		private const int RESPONSE_POS_EXIT_CODE_START = 12;

		/// <summary>
		/// PLC応答 終了コード長さ
		/// </summary>
		private const int RESPONSE_POS_EXIT_CODE_LENGTH = 2;

		/// <summary>
		/// PLC応答 正常終了コード
		/// </summary>
		byte[] responseExitCodeNormal = new byte[] { 0x00, 0x00 };

		/// <summary>
		/// タイムアウト時応答
		/// </summary>
		public const string BIT_READ_TIMEOUT_VALUE = "TIMEOUT";

		public static string[] BIT_READ_TIMEOUT_VALUE_ARRAY = new string[] { "TIMEOUT" };

		public enum DataType
		{
			Channel,
			Bit
		}

		#region Command Header

		/// <summary>
		/// コマンド(ヘッダ1)
		/// </summary>
		private static byte[] commandHeader1 = new byte[] 
		{
			0x80,	//ICF(インフォメーション・コントロール・フィールド)
			0x00,   //RSV(システム予約)                                 
			0x02,   //GCT(許容ｹﾞｰﾄｳｪｲ通過数)
			0x00    //DNA(送信先ﾈｯﾄﾜｰｸｱﾄﾞﾚｽ)
		};

		/// <summary>
		/// コマンド(送信先アドレス)
		/// </summary>
		private static byte[] commandDestinationAddress = new byte[] 
		{
			0x00,   //DA2(送信先号機ｱﾄﾞﾚｽ)
			0x00    //SNA(送信元ﾈｯﾄﾜｰｸｱﾄﾞﾚｽ)
		};

		/// <summary>
		/// コマンド(送信元アドレス)
		/// </summary>
		private static byte[] commandSourceAddress = new byte[] 
		{
			0x00,   //SA2(送信元号機ｱﾄﾞﾚｽ)
			0x00    //SID(サービスID)
		};

		/// <summary>
		/// コマンド(書込用)
		/// </summary>
		private static byte[] commandWriteCode = new byte[] 
        { 
            0x01,　//ｺﾏﾝﾄﾞｺｰﾄﾞ1
            0x02   //ｺﾏﾝﾄﾞｺｰﾄﾞ2                
        };

		/// <summary>
		/// コマンド(読出用)
		/// </summary>
		private static byte[] commandReadCode = new byte[] 
        { 
            0x01,	//ｺﾏﾝﾄﾞｺｰﾄﾞ1
            0x01	//ｺﾏﾝﾄﾞｺｰﾄﾞ2                
        };

		/// <summary>
		/// コマンド(I/Oメモリ種別：DM)
		/// </summary>
		private static byte[] commandDMMemory = new byte[] 
        { 
            0x82              
        };

		/// <summary>
		/// コマンド(I/Oメモリ種別：EM)
		/// </summary>
		private static byte[] commandEMMemory = new byte[] 
        { 
            0xA0              
        };

		#endregion

		#region executeCommand コマンド実行して生の結果を返す

		public byte[] executeCommand(byte[] bynaryCommand)
		{
			return executeCommand(bynaryCommand, true);
		}

		private byte[] executeCommand(byte[] bynaryCommand, bool canRetry)
		{
			byte[] sendBytes = bynaryCommand;
			System.Net.IPEndPoint remoteEP = null;

			try
			{
				//ソケット上限超えを防ぐため、ロックする。
				lock (lockobj)
				{
					udp.Client.Blocking = true;
					bool needSendCommandFg;

					DateTime firstSendDt = DateTime.Now;
					TimeSpan totalTimeSpan = DateTime.Now - firstSendDt;

					do
					{
						while (udp.Available > 0)
						{
							udp.Receive(ref remoteEP);
						}

						needSendCommandFg = false;
						udp.Send(sendBytes, sendBytes.Length);

						DateTime sendDt = DateTime.Now;
						TimeSpan ts = DateTime.Now - sendDt;

						//受信データがなければ300ms待機した後、コマンド再送
						while (udp.Available <= 0)
						{
							if (ts.TotalMilliseconds < 300)
							{
								ts = DateTime.Now - sendDt;
								System.Threading.Thread.Sleep(50);
							}
							else
							{
								needSendCommandFg = true;
								break;
							}
						}

						//コマンド再送するも、タイムアウト時間を越えた場合
						totalTimeSpan = DateTime.Now - firstSendDt;
						if (needSendCommandFg && totalTimeSpan.TotalMilliseconds > 3000)
						{
							throw new ApplicationException("PLCへのデータ送信がタイムアウトしました。");
						}
					} while (needSendCommandFg);

					//データを受信する
					return udp.Receive(ref remoteEP);
				}
			}
			catch (Exception ex)
			{
				if (canRetry)
				{
					return executeCommand(bynaryCommand, false);
				}

				throw ex;
			}
		}
		#endregion

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
		private byte[] createCommand(string hexAddressWithDeviceNM, byte[] commandType, DataType dataType, int points, byte[] writeData)
		{
			List<byte> retv = new List<byte>();

			retv.AddRange(commandHeader1);
			retv.Add(Convert.ToByte(GetNode(this.IPAddress)));
			retv.AddRange(commandDestinationAddress);
			retv.Add(Convert.ToByte(GetNode(this.HostIPAddress)));
			retv.AddRange(commandSourceAddress);
			retv.AddRange(commandType);
			retv.AddRange(convertAddressToByteArray(hexAddressWithDeviceNM, dataType));

			retv.AddRange(BitConverter.GetBytes(Convert.ToInt16(points)).Reverse());

			if (writeData != null)
			{
				retv.AddRange(writeData);
			}

			return retv.ToArray();
		}

		#endregion

		private string GetNode(string ipAddress)
		{
			return ipAddress.Substring(ipAddress.LastIndexOf(".") + 1, ipAddress.Length - ipAddress.LastIndexOf(".") - 1);
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

			retv.Add(Convert.ToByte(Convert.ToInt16(s)));

			return retv.ToArray();
		}

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

					output = data[RESPONSE_POS_DATA_START].ToString();
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
				output = BitConverter.ToInt16(data.Skip(RESPONSE_POS_DATA_START).Take(2).Reverse().ToArray(), 0);
				return true;
			}

			//異常終了
			output = 0;
			return false;
		}

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

					byte[] rData = data.Skip(RESPONSE_POS_DATA_START).ToArray();

					for (int i = 0; i < rData.Length; i += 2)
					{
						outputdata.Add(rData[i + 1]);
						outputdata.Add(rData[i]);
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

		#region convertAddressToByteArray

		private byte[] convertAddressToByteArray(string hexAddressWithDeviceNM, DataType dataType)
		{
			List<byte> retv = new List<byte>();

			string header, allInOneAddress, bank;

			string memoryType = hexAddressWithDeviceNM.Substring(0, 2);
			switch (memoryType)
			{
				case "DM":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					header = hexAddressWithDeviceNM.Substring(0, 2);
					if (dataType == DataType.Channel)
					{
						retv.Add(Convert.ToByte("82", 16));
					}
					else
					{
						retv.Add(Convert.ToByte("02", 16));
					}
					break;
				case "EM":
					allInOneAddress = hexAddressWithDeviceNM.Substring(3);
					header = hexAddressWithDeviceNM.Substring(0, 2);
					bank = hexAddressWithDeviceNM.Substring(2, 1);
					if (bank == "0") 
						retv.Add(Convert.ToByte("A0", 16));
					else if (bank == "1")
						retv.Add(Convert.ToByte("A1", 16));
					else if (bank == "2")
						retv.Add(Convert.ToByte("A2", 16));
					else if (bank == "3")
						retv.Add(Convert.ToByte("A3", 16));
					else
						throw new ApplicationException("想定されていないバンクアドレスが指定されています:" + hexAddressWithDeviceNM);
					
					break;
				case "CH":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					header = hexAddressWithDeviceNM.Substring(0, 2);
					retv.Add(Convert.ToByte("30", 16));
					break;
				case "WR":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					header = hexAddressWithDeviceNM.Substring(0, 2);
					retv.Add(Convert.ToByte("31", 16));
					break;
				case "HM":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					header = hexAddressWithDeviceNM.Substring(0, 2);
					retv.Add(Convert.ToByte("32", 16));
					break;
				case "AR":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					header = hexAddressWithDeviceNM.Substring(0, 2);
					retv.Add(Convert.ToByte("33", 16));
					break;
				default:
					throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
			}

			string address = allInOneAddress.Split('.')[0];

			byte[] addressByte;
			addressByte = BitConverter.GetBytes(Convert.ToInt16(address));

			retv.AddRange(addressByte.Reverse());

			byte addrFooter;
			if (dataType == DataType.Channel)
			{
				addrFooter = 0x00;
			}
			else
			{
				addrFooter = Convert.ToByte(allInOneAddress.Split('.')[1]);
			}

			retv.Add(addrFooter);

			return retv.ToArray();
		}
		#endregion

		public void SetBit(string hexAddressWithDeviceNM, int points, string data)
		{
			byte[] writeData = parseStringDataToPLCBitData(data);
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWriteCode, DataType.Bit, 1, writeData);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			string output;
			bool isSuccess = parseBitResponseData(response, points, out output);
			if (isSuccess == true)
			{
				return;
			}
			else
			{
				//異常終了
				//Log.SysLog.Info("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}

		#region GetBit

		public string GetBit(string hexAddressWithDeviceNM)
		{
			return GetBit(hexAddressWithDeviceNM, 1);
		}

		public string GetBit(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Bit, 1, null);

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
				//Log.RBLog.Info("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}
		public string[] GetBitArray(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Bit, length, null);

			byte[] res = executeCommand(cmd);

			if (isExitNormal(res) == true)
			{
				res = res.Skip(RESPONSE_POS_DATA_START).Take(2).Reverse().ToArray();
				return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
			}
			else
			{
				//異常終了
				//Log.WriteError("PLCエラー応答:" + command);
				return Omron.BIT_READ_TIMEOUT_VALUE_ARRAY;
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
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Channel, length, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			return parseWordResponseData(response);
		}

		#endregion

		#region GetWordsAsDateTime

		public DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Channel, 6, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);

			try
			{
				if (isExitNormal(response))
				{
					if (response.Length != 26)
					{
						//Log.RBLog.Info("PLCエラー応答:" + response);
						throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
					}

					Byte[] yearBytes = response.Skip(RESPONSE_POS_DATA_START).Take(2).ToArray();
					int year = BitConverter.ToInt16(yearBytes.Reverse().ToArray(), 0);

					Byte[] monthBytes = response.Skip(RESPONSE_POS_DATA_START + 2).Take(2).ToArray();
					int month = BitConverter.ToInt16(monthBytes.Reverse().ToArray(), 0);

					Byte[] dayBytes = response.Skip(RESPONSE_POS_DATA_START + 4).Take(2).ToArray();
					int day = BitConverter.ToInt16(dayBytes.Reverse().ToArray(), 0);

					Byte[] hourBytes = response.Skip(RESPONSE_POS_DATA_START + 6).Take(2).ToArray();
					int hour = BitConverter.ToInt16(hourBytes.Reverse().ToArray(), 0);

					Byte[] minuteBytes = response.Skip(RESPONSE_POS_DATA_START + 8).Take(2).ToArray();
					int minute = BitConverter.ToInt16(minuteBytes.Reverse().ToArray(), 0);

					Byte[] secondBytes = response.Skip(RESPONSE_POS_DATA_START + 10).Take(2).ToArray();
					int second = BitConverter.ToInt16(secondBytes.Reverse().ToArray(), 0);

					return new DateTime(year, month, day, hour, minute, second);
				}
				else
				{
					//異常終了
					//Log.RBLog.Info("PLCエラー応答:" + response);
					throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
				}
			}
			catch (Exception ex)
			{
				//Log.RBLog.Error("日付データへの変換に失敗:res=" + response);
				throw ex;
			}
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

		public void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWriteCode, DataType.Channel, 1, BitConverter.GetBytes(Convert.ToInt16(data)).Reverse().ToArray());

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

		public int GetWordAsDecimalData(string hexAddressWithDeviceNM)
		{
			return GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
		}
		public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Channel, wordLength, null);

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
				//Log.RBLog.Error("PLC異常応答:" + response);
				throw new ApplicationException("PLC異常応答");
			}
		}

		#region GetMagazineNo

		public string GetMagazineNo(string hexAddressWithDeviceNm)
		{
			return GetMagazineNo(hexAddressWithDeviceNm, MAGAZINE_NO_WORD_LENGTH);
		}

		public string GetMagazineNo(string hexAddressWithDeviceNm, int wordLength)
		{
			string qr = string.Empty;

			string org = this.GetWord(hexAddressWithDeviceNm, wordLength).Trim();
			qr = org;

			//Null文字を置換
			qr = qr.Replace("\0", "");

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
		}

		/// <summary>
		/// 不正応答時は空白
		/// </summary>
		/// <param name="hexAddressWithDeviceNm"></param>
		/// <param name="host"></param>
		/// <returns></returns>
		public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided)
		{
			string qr = string.Empty;

			string[] org = this.GetBitArray(hexAddressWithDeviceNm, MAGAZINE_NO_WORD_LENGTH);
			if (org == Omron.BIT_READ_TIMEOUT_VALUE_ARRAY)
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

				magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqno);
				return magno;
			}
			else
			{
				return qr.Trim();
			}
		}

		#endregion

		public string GetMachineNo(string hexAddressWithDeviceNm)
		{
			throw new NotImplementedException();
		}

		public string GetMachineNo(string hexAddressWithDeviceNm, int wordLength)
		{
			throw new NotImplementedException();
		}

		public string GetString(string hexAddressWithDeviceNm, int wordLength)
		{
			throw new NotImplementedException();
		}

		#region SetString

		public bool SetString(string hexAddressWithDeviceNM, string data)
		{
			byte[] byteStr = Encoding.ASCII.GetBytes(data);
			List<byte> byteList = byteStr.ToList();
			if (Math.IEEERemainder(byteStr.Length, 2) != 0)
			{
				byteList.Add(0x00);
				byteStr = byteList.ToArray();
			}

			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWriteCode, DataType.Channel, byteStr.Count() / 2, byteStr);
			string logStr = string.Empty;
			foreach (byte cmdbyte in cmd)
			{
				logStr += Convert.ToString(cmdbyte);
			}
			//Log.SysLog.Info(string.Format("送信コマンド(ARMS⇒PLC):{0}", logStr));

			logStr = string.Empty;
			foreach (byte cmdbyte in byteStr)
			{
				logStr += Convert.ToString(cmdbyte);
			}
			//Log.SysLog.Info(string.Format("LMマーキングバイト列:{0}", logStr));

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

		#endregion

		//private string getMyIpAddress()
		//{
		//	string hostname = Dns.GetHostName();
		//	IPAddress[] adrList = Dns.GetHostAddresses(hostname);

		//	IEnumerable<IPAddress> ipAddress = adrList.Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
		//	if (ipAddress.Count() == 0)
		//	{
		//		return string.Empty;
		//	}
		//	else 
		//	{
		//		return ipAddress.Single().ToString();
		//	}
		//}

		public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue)
		{
			throw new NotImplementedException();
		}
	}
}
