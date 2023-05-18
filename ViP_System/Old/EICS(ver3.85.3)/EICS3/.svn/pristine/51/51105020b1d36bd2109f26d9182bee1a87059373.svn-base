using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Drawing;
using EICS.Structure;
using System.Threading;

namespace EICS
{
    public class PLC_Omron : IDisposable, IPlc
    {
		private const int ChannelByteLength = 2;
		private const int WordByteSize = 2;
		private const int FINS_COM_FRAME_COMMAND_INDEX = 10;
		private const int FINS_COM_FRAME_COMMAND_BYTESIZE = 2;
		private const int FINS_COM_FRAME_IO_MEM_TYPE_INDEX = 12;
		private const int FINS_COM_FRAME_READPOINT_INDEX = 16;
		private const int FINS_COM_FRAME_READPOINT_BYTESIZE = 2;
		private const int FINS_COM_FRAME_READ_COMMAND_SIZE = 18;
		private const int FINS_RES_FRAME_BASE_BYTESIZE = 14;


		public static string PLC_TYPE = "OMRON";
		/// <summary>ホストPC IPアドレス</summary>
		private string Host_IP { get; set; }
        /// <summary>PLC IPアドレス</summary>
		private string PLC_IP { get; set; }
        /// <summary>中間サーバノード</summary>
        private byte _fromNode;
        /// <summary>PLCノード</summary>
        private byte _toNode;
        /// <summary>UDP通信インスタンス</summary>
        private System.Net.Sockets.UdpClient udp = null;
        /// <summary>PLCポート</summary>
        private const int PLC_PORT_NO = 9600;

		static object lockobj = new object();

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

        public PLC_Omron(string hostIP, string plcIP, byte fromNode, byte toNode, int receiveTimeout)
        {
			this.Host_IP = hostIP;
			this.PLC_IP = plcIP;
            this._fromNode = fromNode;
            this._toNode = toNode;

			udp = new System.Net.Sockets.UdpClient(plcIP, PLC_PORT_NO);
			udp.Client.ReceiveTimeout = receiveTimeout;
        }

        #region IDisposable メンバ

        public void Dispose()
        {
            if (udp != null)
            {
                udp.Close();
            }
        }

        #endregion

		public static string GetNodeAddress(string ipAddress)
		{
			return ipAddress.Substring(ipAddress.LastIndexOf(".") + 1, ipAddress.Length - ipAddress.LastIndexOf(".") - 1);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="hexAddressWithDeviceNM"></param>
		/// <returns></returns>
		public bool GetBool(string hexAddressWithDeviceNM)
		{
			string recvStr = GetBit(hexAddressWithDeviceNM);

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

        /// <summary>
        /// データメモリを変更する
        /// </summary>
        /// <param name="status"></param>
        /// <param name="errMessageList"></param>
        public void ChangeFrameSupplyStatus(Constant.PlcMemory address, Constant.FrameSupplyStatus status, ref List<ErrMessageInfo> errMessageList) 
        {
            try
            {
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                //ヘッダ
                byte[] sendBytes1 = new byte[]
                    { 
                        0x80,           //ICF(インフォメーション・コントロール・フィールド)
                        0x00,           //RSV(システム予約)                                 
                        0x02,           //GCT(許容ｹﾞｰﾄｳｪｲ通過数)
                        0x00            //DNA(送信先ﾈｯﾄﾜｰｸｱﾄﾞﾚｽ)
                    };

                //送信先ノード
                byte[] sendBytes2 = new byte[] 
                    { 
                        this._toNode    //DA1(送信先ﾉｰﾄﾞｱﾄﾞﾚｽ)      ※PLC側ノード
                    };

                //送信先アドレス
                byte[] sendBytes3 = new byte[] 
                    { 
                        0x00,           //DA2(送信先号機ｱﾄﾞﾚｽ)
                        0x00            //SNA(送信元ﾈｯﾄﾜｰｸｱﾄﾞﾚｽ)
                    };

                //送信元ノード
                byte[] sendBytes4 = new byte[] 
                    { 
                        this._fromNode  //SA1(送信元ﾉｰﾄﾞｱﾄﾞﾚｽ)      ※PC側ノード
                    };

                //送信元アドレス
                byte[] sendBytes5 = new byte[] 
                    { 
                        0x00,           //SA2(送信元号機ｱﾄﾞﾚｽ)
                        0x00            //SID(サービスID)
                    };

                //ヘッダ
                byte[] sendBytes6 = new byte[] 
                    { 
                        0x01,           //ｺﾏﾝﾄﾞｺｰﾄﾞ1
                        0x02,           //ｺﾏﾝﾄﾞｺｰﾄﾞ2                ※書込は02
                        0x82            //I/Oﾒﾓﾘ種別
                    };

                //DMアドレス
                byte[] sendBytes7;
                if (address == Constant.PlcMemory.D900)
                {
                    sendBytes7 = new byte[] 
                    { 
                        0x03,           //ｱﾄﾞﾚｽ1                    ※D900は384、※D910は38E
                        0x84,           //ｱﾄﾞﾚｽ2
                        0x00            //ｱﾄﾞﾚｽ3
                    };
                }
                else
                {
                    sendBytes7 = new byte[] 
                    { 
                        0x03,           //ｱﾄﾞﾚｽ1                    ※D900は384、※D910は38E
                        0x8E,           //ｱﾄﾞﾚｽ2
                        0x00            //ｱﾄﾞﾚｽ3
                    };
                }

                //書込内容
                byte[] sendBytes8;
                if (status == Constant.FrameSupplyStatus.NG)
                {
                    sendBytes8 = new byte[] 
                    { 
                        0x00,           //要素数1
                        0x01,           //要素数2
                        0x00,           //書込データ1
                        0x00            //書込データ2
                    };
                }
                else
                {
                    sendBytes8 = new byte[] 
                    { 
                        0x00,           //要素数1
                        0x01,           //要素数2
                        0x00,           //書込データ1
                        0x01            //書込データ2
                    };
                }
                                        
                ms.Write(sendBytes1, 0, sendBytes1.Length);
                ms.Write(sendBytes2, 0, sendBytes2.Length);
                ms.Write(sendBytes3, 0, sendBytes3.Length);
                ms.Write(sendBytes4, 0, sendBytes4.Length);
                ms.Write(sendBytes5, 0, sendBytes5.Length);
                ms.Write(sendBytes6, 0, sendBytes6.Length);
                ms.Write(sendBytes7, 0, sendBytes7.Length);
                ms.Write(sendBytes8, 0, sendBytes8.Length);

                byte[] sendBytes = ms.ToArray();
                ms.Close();

                //データを送信する
                udp.Send(sendBytes, sendBytes.Length);

                //データを受信する
                System.Net.IPEndPoint remoteEP = null;

                byte[] rcvBytes1 = udp.Receive(ref remoteEP);
            }

            catch(Exception)
            {
                //命令が送信できなかった場合、メッセージ表示
                string message = string.Format(Constant.MessageInfo.Message_43, this,PLC_IP, Convert.ToString(address));                ErrMessageInfo errMessageInfo = new ErrMessageInfo(message, Color.Red);
                errMessageList.Add(errMessageInfo);

                F01_MachineWatch.spMachine.PlayLooping();
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, message);
            }
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

            byte[] recv;
            byte[] sendBytes = bynaryCommand;
			System.Net.IPEndPoint remoteEP = null;

            System.Threading.Mutex mutex = new System.Threading.Mutex(false, $"OmronPlcAccessMutex_{PLC_IP.Trim()}");

            try
			{
                //ARMSとの同時通信を規制するためにMutexでロックする。1秒待機しても解除されなかったら例外エラー。
                if (mutex.WaitOne(1000, false) == true)
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
                            if (ts.TotalMilliseconds < 30)
                            {
                                ts = DateTime.Now - sendDt;
                                System.Threading.Thread.Sleep(10);
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
                            mutex.ReleaseMutex();
                            throw new ApplicationException("PLCへのデータ送信がタイムアウトしました。");
                        }

                    } while (needSendCommandFg);

                    //データを受信する
                    recv = udp.Receive(ref remoteEP);
                    mutex.ReleaseMutex();

                    return recv;
                }
                else
                {
                    throw new ApplicationException($"OmronPLCへのデータ送信で他プロセスのロックが1秒以上解除されませんでした。IP:{PLC_IP}");
                }
			}
			catch (Exception ex)
			{

                mutex.Close();
                if (canRetry)
				{
					return executeCommand(bynaryCommand, false);
				}

				throw ex;
			}
            finally
            {
                mutex.Close();
            }
        }

		private byte[] executeCommandWithSizeCheck(byte[] bynaryCommand)
		{
			return executeReadCommandWithSizeCheck(bynaryCommand, true);
		}

		private byte[] executeReadCommandWithSizeCheck(byte[] bynaryCommand, bool canRetry)
		{
			byte[] sendBytes = bynaryCommand;
			System.Net.IPEndPoint remoteEP = null;
			byte[] recv;

            System.Threading.Mutex mutex = new System.Threading.Mutex(false, $"OmronPlcAccessMutex_{PLC_IP.Trim()}");


            try
			{
                //ARMSとの同時通信を規制するためにMutexでロックする。1秒待機しても解除されなかったら例外エラー。
                if (mutex.WaitOne(1000, false) == true)
                {
					udp.Client.Blocking = true;

					DateTime firstSendDt = DateTime.Now;
					TimeSpan totalTimeSpan = DateTime.Now - firstSendDt;

					if (udp.Available > 0)
					{
						udp.Receive(ref remoteEP);
					}

					udp.Send(sendBytes, sendBytes.Length);

					DateTime sendDt = DateTime.Now;
					TimeSpan ts = DateTime.Now - sendDt;

					byte hexMemType = sendBytes[FINS_COM_FRAME_IO_MEM_TYPE_INDEX];

					int pointByteSize = GetDataLen(hexMemType);
					
					int needByteLen = 0;
					if (sendBytes.Length == FINS_COM_FRAME_READ_COMMAND_SIZE)
					{
						byte[] readPtSection = sendBytes.Skip(FINS_COM_FRAME_READPOINT_INDEX).Take(FINS_COM_FRAME_READPOINT_BYTESIZE).Reverse().ToArray();
						int readPoint = BitConverter.ToUInt16(readPtSection, 0);
						needByteLen = readPoint * pointByteSize + FINS_RES_FRAME_BASE_BYTESIZE;
					}
					else
					{
						throw new ApplicationException(string.Format("不正なｺﾏﾝﾄﾞ送信です。(ｺﾏﾝﾄﾞｻｲｽﾞが想定されるｻｲｽﾞ:{0}と合致しませんでした。) " +
							"ｺﾏﾝﾄﾞ:{1}", FINS_COM_FRAME_READ_COMMAND_SIZE, string.Join(" ", sendBytes)));
					}

					while (udp.Available != needByteLen)
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
                            mutex.ReleaseMutex();
                            throw new ApplicationException(
								string.Format("PLCへのコマンド送信後、送信先からのコマンドに対する応答が無い(もしくは必要な受信ｻｲｽﾞに達しないまま)タイムアウト(4000ms)しました。受信ﾊﾞｯﾌｧｻｲｽﾞ:{0} 受信ﾊﾞｯﾌｧﾃﾞｰﾀ:{1}"
								, udp.Available, recvStr));
						}
					}

					recv = udp.Receive(ref remoteEP);
                    mutex.ReleaseMutex();

					if (recv[FINS_COM_FRAME_COMMAND_INDEX] == sendBytes[FINS_COM_FRAME_COMMAND_INDEX]
						&& recv[FINS_COM_FRAME_COMMAND_INDEX + 1] == sendBytes[FINS_COM_FRAME_COMMAND_INDEX + 1])
					{
						return recv;
					}
					else
					{
						throw new ApplicationException(string.Format(
							"送信ｺﾏﾝﾄﾞｺｰﾄﾞと受信ﾃﾞｰﾀ内のｺﾏﾝﾄﾞｺｰﾄﾞが一致しませんでした。(両ﾊﾞｲﾄ列共 10,11ﾊﾞｲﾄ目がｺﾏﾝﾄﾞｺｰﾄﾞ)送信内容:{0} / 受信内容:{1}",
							string.Join(" ", sendBytes), string.Join(" ", recv)));
					}
				}
                else
                {
                    throw new ApplicationException($"OmronPLCへのデータ送信で他プロセスのロックが1秒以上解除されませんでした。IP:{PLC_IP}");
                }
            }
			catch (Exception ex)
			{

                mutex.Close();
                if (canRetry)
				{
					return executeCommand(bynaryCommand, false);
				}

				throw ex;
			}
            finally
            {
                mutex.Close();
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
			retv.Add(Convert.ToByte(GetNode(this.PLC_IP)));
			retv.AddRange(commandDestinationAddress);
			retv.Add(Convert.ToByte(GetNode(this.Host_IP)));
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
				byte[] readData = data.Skip(RESPONSE_POS_DATA_START).ToArray();
				//正常終了
				if (readData.Length == 1)
				{
					output = BitConverter.ToInt16(readData.Take(1).Reverse().ToArray(), 0);
				}
				else if (readData.Length == 2)
				{
					output = BitConverter.ToInt16(readData.Take(2).Reverse().ToArray(), 0);
				}
				else if (readData.Length == 4)
				{
					output = BitConverter.ToInt32(readData.Take(4).Reverse().ToArray(), 0);
				}
				else
				{
					return false;
				}

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
						if (i + 2 <= rData.Length)
						{
							outputdata.Add(rData[i + 1]);
						}
						outputdata.Add(rData[i]);
					}
					return Encoding.ASCII.GetString(outputdata.ToArray());
				}
				else
				{
					throw new ApplicationException(string.Format("PLC異常応答： data = {0}", string.Join(", ", data)));
				}
			}
			catch (Exception ex)
			{
				throw new ApplicationException(string.Format("PLC WORD Parse error: data = {0}\r\n{1}", string.Join(", ", data), ex.Message));
			}
		}
		#endregion

		#region convertAddressToByteArray

		private byte[] convertAddressToByteArray(string hexAddressWithDeviceNM, DataType dataType)
		{
			List<byte> retv = new List<byte>();

			string allInOneAddress = string.Empty, bank;

			string memoryType = hexAddressWithDeviceNM.Substring(0, 2);
			switch (memoryType)
			{
				case "DM":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
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
					retv.Add(Convert.ToByte("30", 16));
					break;
				case "WR":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					retv.Add(Convert.ToByte("31", 16));
					break;
				case "HM":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					retv.Add(Convert.ToByte("32", 16));
					break;
				case "AR":
					allInOneAddress = hexAddressWithDeviceNM.Substring(2);
					retv.Add(Convert.ToByte("33", 16));
					break;
				//default:
					//throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
			}

			memoryType = hexAddressWithDeviceNM.Substring(0, 1);

			if (hexAddressWithDeviceNM.StartsWith("EM") == false)
			{
				switch (memoryType)
				{
					case "E":
						string[] memoryAddrItem = hexAddressWithDeviceNM.Split('_');
						string memTypeHex;
						bank = memoryAddrItem[0].Remove(0, 1);
						string bankOffset = "C0";

						allInOneAddress = hexAddressWithDeviceNM.Substring(3);
						//bank = hexAddressWithDeviceNM.Substring(2, 1);

						if (dataType == DataType.Bit)
						{
							memTypeHex = "20";
						}
						else if (dataType == DataType.Channel)
						{
							memTypeHex = "A0";
						}
						else
						{
							throw new ApplicationException("不正なPLCアドレスです(DataTypeがBit,Channelの何れにも該当しません。):" + hexAddressWithDeviceNM);
						}

						if (bank.Length == 1)
						{
							//バンクが1桁の場合、20(EMﾊﾞﾝｸ0～F ﾋﾞｯﾄ) or A0(EMﾊﾞﾝｸ0～F ﾁｬﾈﾙ)
							memTypeHex = (Convert.ToInt32(memTypeHex, 16) + Convert.ToInt32(bank, 16)).ToString("X");
						}
						else if (bank.Length == 2)
						{
							//バンクが2桁の場合、20(EMﾊﾞﾝｸ10～18 ﾋﾞｯﾄ) or A0(EMﾊﾞﾝｸ10～18 ﾁｬﾈﾙ)にC0(ﾊﾞﾝｸｵﾌｾｯﾄ)を足すと桁上がりする為
							memTypeHex = (Convert.ToInt32(memTypeHex, 16) + Convert.ToInt32(bank.Substring(1), 16) + (Convert.ToInt32(bankOffset, 16)) & 0xff).ToString("X");
						}
						else
						{
							throw new ApplicationException("不正なPLCアドレスです(ﾊﾞﾝｸを示す部分の桁数が2桁を超え想定外です):" + hexAddressWithDeviceNM);
						}

						//retv.Add(Convert.ToByte(memTypeHex));
						retv.Add((byte)Convert.ToInt32(memTypeHex, 16));						
						break;
					default:
						//throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
						break;
				}
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

		/// <summary>
		/// I/Oメモリ種別(Hex)から1要素あたりのデータ長を返す
		/// </summary>
		/// <param name="hexIOMemType"></param>
		/// <returns></returns>
		private int GetDataLen(byte hexIOMemType)
		{
			#region I/Oメモリ種別毎のデータ長リスト
			//  16進数    10進数   1要素のデータ長(バイト)
			/////////////////////////////////////
			//      02         2         1
			//      06         6         1
			//      07         7         1
			//      09         9         1
			//      0A        10         1
			//      20        32         1
			//      21        33         1
			//      22        34         1
			//      23        35         1
			//      24        36         1
			//      25        37         1
			//      26        38         1
			//      27        39         1
			//      28        40         1
			//      29        41         1
			//      2A        42         1
			//      2B        43         1
			//      2C        44         1
			//      2D        45         1
			//      2E        46         1
			//      2F        47         1
			//      30        48         1
			//      31        49         1
			//      32        50         1
			//      33        51         1
			//      46        70         1
			/////////////////////////////////////
			//      50        80         2
			//      51        81         2
			//      52        82         2
			//      53        83         2
			//      54        84         2
			//      55        85         2
			//      56        86         2
			//      57        87         2
			//      58        88         2
			//      59        89         2
			//      5A        90         2
			//      5B        91         2
			//      5C        92         2
			//      5D        93         2
			//      5E        94         2
			//      5F        95         2
			//      60        96         2
			//      61        97         2
			//      62        98         2
			//      63        99         2
			//      64       100         2
			//      65       101         2
			//      66       102         2
			//      67       103         2
			//      68       104         2
			//      82       130         2
			//      89       137         2
			//      98       152         2
			//      A0       160         2
			//      A1       161         2
			//      A2       162         2
			//      A3       163         2
			//      A4       164         2
			//      A5       165         2
			//      A6       166         2
			//      A7       167         2
			//      A8       168         2
			//      A9       169         2
			//      AA       170         2
			//      AB       171         2
			//      AC       172         2
			//      AD       173         2
			//      AE       174         2
			//      AF       175         2
			//      B0       176         2
			//      B1       177         2
			//      B2       178         2
			//      B3       179         2
			//      BC       188         2
			//      BC       188         2
			/////////////////////////////////////
			//      DC       220         4
			/////////////////////////////////////
			//      E0       224         1
			//      E1       225         1
			//      E2       226         1
			//      E3       227         1
			//      E4       228         1
			//      E5       229         1
			//      E6       230         1
			//      E7       231         1
			//      E8       232         1
			#endregion

			if (hexIOMemType < 0x50)
			{
				return 1;
			}
			else if (hexIOMemType < 0xDC)
			{
				return 2;
			}
			else if (hexIOMemType < 0xE0)
			{
				return 4;
			}
			else
			{
				return 1;
			}
		}

		public void SetBit(string hexAddressWithDeviceNM, int points, string data)
		{
			//string logMsg = string.Format("SetBit() addr:{0} length:{1} data:{2}", hexAddressWithDeviceNM, points, data);
			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

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
				throw new ApplicationException("PLC異常応答");
			}
		}

		#region GetBit

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
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, hexAddressWithDeviceNM + "(ﾊﾞｲﾄﾃﾞｰﾀ):" + byteData[0].ToString() + "," + byteData[1].ToString());
				decData = (int)byteData[1];
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

			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, hexAddressWithDeviceNM + ":" + decData.ToString());

			//byte data = GetWordRaw(dataMemAddr, 1)[1];
			byte data = Convert.ToByte(decData);

			string binaryStr = Convert.ToString(data, 2).PadLeft(16, '0');

			return binaryStr.Substring(binaryStr.Length - bitNum, 1);

			//return (data & bitNum).ToString();
		}

		public string GetBit(string hexAddressWithDeviceNM)
		{
			// ビットでのデータ取得が何故か出来ない（PLCコマンドも確認したが特に問題なく、Channel経由だとデータ取得可能なので、
			// GetBitByChannel()を作りワード単位でデータを取って、アドレスで指定されたビットを抜き出すように処理修正 ARMSではHｱﾄﾞﾚｽでのﾋﾞｯﾄ取得実績は有る模様  2016/3/10 吉本）
			//return GetBitByChannel(hexAddressWithDeviceNM);
			//return GetBit(hexAddressWithDeviceNM, 1);
			if (hexAddressWithDeviceNM.StartsWith("E") && hexAddressWithDeviceNM.StartsWith("EM") == false)
			{
				return GetBitByChannel(hexAddressWithDeviceNM);
			}
			else
			{
				return GetBit(hexAddressWithDeviceNM, 1);
			}
		}

		// ビットでのデータ取得が何故か出来ない（PLCコマンドも確認したが特に問題なく、Channel経由だとデータ取得可能なので、2016/3/10 吉本）
		public string GetBit(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Bit, 1, null);

			//PLCコマンド実行
			byte[] response = executeCommand(cmd);
			string outputstring;
			bool isSuccess = parseBitResponseData(response, length, out outputstring);
			if (isSuccess == true)
			{
				//if (outputstring == BIT_ON) 
				//{
				//	string logMsg = string.Format("GetBit() addr:{0} length:{1} response:{2}", hexAddressWithDeviceNM, length, string.Join(", ", response));
				//	log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				//}
				return outputstring;
			}
			else
			{
				//異常終了
				throw new ApplicationException("PLC異常応答");
			}
		}

		public string[] GetBitArray(string hexAddressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Bit, length, null);

			byte[] res = executeCommand(cmd);

			if (isExitNormal(res) == true)
			{
				return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
			}
			else
			{
				//異常終了
				//Log.WriteError("PLCエラー応答:" + command);
				return PLC_Omron.BIT_READ_TIMEOUT_VALUE_ARRAY;
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
			byte[] response = executeCommandWithSizeCheck(cmd);

			//string logMsg = string.Format("GetWord() addr:{0} length:{1} response:{2}", hexAddressWithDeviceNM, length, string.Join(", ", response));
			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

			return parseWordResponseData(response);
		}

		#endregion

		#region GetWordsAsDateTime

		public DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Channel, 6, null);

			//PLCコマンド実行
			byte[] response = executeCommandWithSizeCheck(cmd);

			try
			{
				if (isExitNormal(response))
				{
					if (response.Length != 26)
					{
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
					throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
				}
			}
			catch (Exception ex)
			{
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
			//string logMsg = string.Format("SetWordAsDecimalData() addr:{0} data:{1}", hexAddressWithDeviceNM, data);
			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

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
            bool isSuccess;
            int output;

            int tryCt = 1;
            do
            {
                //PLCコマンド実行
				byte[] response = executeCommandWithSizeCheck(cmd);

				//string logMsg = string.Format("GetWordAsDecimalData() addr:{0} Length:{1} response:{2}", hexAddressWithDeviceNM, wordLength, string.Join(", ", response));
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

                isSuccess = parseWordResponseDataAsDecimalData(response, out output);

            } while (isSuccess == false && tryCt++ < 3);

            if (isSuccess)
            {
                return output;
            }
            else
            {
                //異常終了
                throw new ApplicationException("PLC異常応答");
            }
		}

		public float GetWordAsFloatData(string hexAddressWithDeviceNM)
		{
			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandReadCode, DataType.Channel, 2, null);
			bool isSuccess;
			float output;

			int tryCt = 1;
			do
			{
				//PLCコマンド実行
				byte[] response = executeCommandWithSizeCheck(cmd);

				//string logMsg = string.Format("GetWordAsFloatData() addr:{0} Length:{1} response:{2}", hexAddressWithDeviceNM, 2, string.Join(", ", response));
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

				isSuccess = parseWordResponseDataAsSingleData(response, out output);

			} while (isSuccess == false && tryCt++ < 3);

			if (isSuccess)
			{
				return output;
			}
			else
			{
				//異常終了
				throw new ApplicationException("PLC異常応答");
			}
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
				byte[] targetData = data.Skip(RESPONSE_POS_DATA_START).ToArray();

				//山本製MDからSingleでデータ取得する際に上位2バイトと下位2バイトをそれぞれで上下1バイト入れ替えをする必要がある
				byte[] upper2ByteData = targetData.Take(2).Reverse().ToArray();
				byte[] lower2ByteData = targetData.Skip(2).Reverse().ToArray();

				//byte[] swapedUpperLowerData = 
				
				byte[] mergedArray = new byte[4];
				//マージする配列のデータをコピーする
				Array.Copy(upper2ByteData, mergedArray, 2);
				Array.Copy(lower2ByteData, 0, mergedArray, 2, 2);

				//正常終了
				output = BitConverter.ToSingle(mergedArray, 0);
				return true;
			}

			//異常終了
			output = 0;
			return false;
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

		///// <summary>
		///// 不正応答時は空白
		///// </summary>
		///// <param name="hexAddressWithDeviceNm"></param>
		///// <param name="host"></param>
		///// <returns></returns>
		//public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided)
		//{
		//	string qr = string.Empty;

		//	string[] org = this.GetBitArray(hexAddressWithDeviceNm, MAGAZINE_NO_WORD_LENGTH);
		//	if (org == PLC_Omron.BIT_READ_TIMEOUT_VALUE_ARRAY)
		//	{
		//		return string.Empty;
		//	}

		//	return GetMagazineNo(org, notDevided);
		//}

		///// <summary>
		///// PLCが読み取ったマガジン番号を返す。
		///// 要素数4の場合は分割マガジン番号を返す
		///// </summary>
		///// <param name="plcResponseBitArray"></param>
		///// <returns></returns>
		//public string GetMagazineNo(string[] plcResponseBitArray, bool notDevided)
		//{
		//	string qr = string.Empty;

		//	foreach (string orgs in plcResponseBitArray)
		//	{
		//		qr += System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(int.Parse(orgs)));
		//	}

		//	//Null文字を置換
		//	qr = qr.Replace("\0", "");
		//	string[] elms = qr.Trim().Split(' ');

		//	if (elms.Length == 2 || (elms.Length >= 3 && notDevided))
		//	{
		//		return elms[1];
		//	}
		//	else if (elms.Length >= 3)
		//	{
		//		string magno = elms[1];
		//		int seqno = int.Parse(elms[2]);

		//		magno = ArmsApi.Model.Order.NascaLotToMagLot(magno, seqno);
		//		return magno;
		//	}
		//	else
		//	{
		//		return qr.Trim();
		//	}
		//}

		#endregion

		public string GetMachineNo(string hexAddressWithDeviceNm)
		{
			throw new NotImplementedException();
		}

		public string GetMachineNo(string hexAddressWithDeviceNm, int wordLength)
		{
			throw new NotImplementedException();
		}

		public string GetDataAsString(string hexAddressWithDeviceNM, int points, string dataType)
		{
			if (dataType == PLC.DT_BINARY)
			{
				return GetBit(hexAddressWithDeviceNM);
			}
			else if (dataType == PLC.DT_DEC_32BIT)
			{
				int recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 2);

				return recvVal.ToString();
			}
			else if (dataType == PLC.DT_DEC_16BIT)
			{
				int recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
				return recvVal.ToString();
			}
			else if (dataType == PLC.DT_UDEC_32BIT)
			{
				int recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 2);

				return ((uint)recvVal).ToString();
			}
			else if (dataType == PLC.DT_UDEC_16BIT)
			{
				int recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
				return ((uint)recvVal).ToString();
			}
			else if (dataType == PLC.DT_HEX_32BIT)
			{
				string recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 2).ToString("X8");
				return recvVal;
			}
			else if (dataType == PLC.DT_HEX_16BIT)
			{
				string recvVal = GetWordAsDecimalData(hexAddressWithDeviceNM, 1).ToString("X4");
				return recvVal;
			}
			else if (dataType == PLC.DT_FLOAT)
			{
				return GetWordAsFloatData(hexAddressWithDeviceNM).ToString();
			}
			else if (dataType == PLC.DT_FLOAT_16BIT)
			{
				return GetWordAsFloatData(hexAddressWithDeviceNM).ToString();
			}
			else if (dataType == PLC.DT_BCD32BIT)
			{
				return GetWordAsBCD(hexAddressWithDeviceNM, 2, false);
			}
			else if (dataType == PLC.DT_BCD16BIT)
			{
				return GetWordAsBCD(hexAddressWithDeviceNM, 1, false);
			}
			else if (dataType == PLC.DT_BOOL)
			{
				string bit = GetBit(hexAddressWithDeviceNM);

				if (bit == BIT_ON.ToString())
				{
					return "TRUE";
				}
				else if (bit == BIT_OFF.ToString())
				{
					return "FALSE";
				}
				else
				{
					throw new ApplicationException(string.Format(
						"PLCから予期せぬ応答を取得しました 応答内容:{0} ｱﾄﾞﾚｽ:{1} 長さ:{2} ﾃﾞｰﾀﾀｲﾌﾟ:{3}", bit, hexAddressWithDeviceNM, points, dataType));
				}
			}
			else if (dataType == PLC.DT_STR)
			{
				return GetString(hexAddressWithDeviceNM, points, true, false);
			}
			else
			{
				throw new ApplicationException(string.Format(
					"ﾏｽﾀにﾃﾞｰﾀﾀｲﾌﾟが指定されていません ｱﾄﾞﾚｽ:{0} 長さ:{1} ﾃﾞｰﾀﾀｲﾌﾟ:{2}", hexAddressWithDeviceNM, points, dataType));
			}
			return string.Empty;
		}

		public string GetWordAsBCD(string addressWithDeviceNM, int wordLen, bool endianReverse)
		{
			string result = string.Empty;
			//int calcResult = 0;
			int halfByteLen = 4;//変換する桁数

			byte[] byteArray = GetWordRaw(addressWithDeviceNM, wordLen);

			int digitCt = 0;

			foreach (byte byteData in byteArray)
			{
				string binStr = Convert.ToString(byteData, 16).PadLeft(2, '0');

				if (endianReverse)
				{
					result = binStr + result;
				}
				else
				{
					result += binStr;
				}
			}

			result = result.TrimStart('0');

			if (string.IsNullOrEmpty(result))
			{
				result = "0";
			}

			return result;

		}

		public byte[] GetWordRaw(string addressWithDeviceNM, int length)
		{
			byte[] cmd = createCommand(addressWithDeviceNM, commandReadCode, DataType.Channel, length, null);

			//PLCコマンド実行
			byte[] response;
			int tryCt = 0;
			int recvSize;

			do{
				response = executeCommandWithSizeCheck(cmd);
				tryCt++;
				recvSize = RESPONSE_POS_DATA_START + ChannelByteLength * length;

				Thread.Sleep(50);
			} while (response.Length != recvSize && tryCt < 10);

			if (response.Length != recvSize)
			{
				throw new ApplicationException(string.Format("PLCエラー応答発生 要求ﾃﾞｰﾀｻｲｽﾞ:{0}に対して受信ｻｲｽﾞ：{1}が異常です。", recvSize, response.Length));
			}

			if (isExitNormal(response))
			{
				//byte[] output = new byte[response.Length - RESPONSE_POS_DATA_START];
				byte[] output = new byte[ChannelByteLength * length];
				//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("outputLen:{0} responseLen:{1}", output.Length, response.Length));
				Array.Copy(response, RESPONSE_POS_DATA_START, output, 0, output.Length);

				return output;
			}
			else
			{
				throw new ApplicationException("PLCエラー応答発生");
			}
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

			//string logMsg = string.Format("GetString() addr:{0} Length:{1}", hexAddressWithDeviceNM, wordLength);
			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

            string org = this.GetWord(hexAddressWithDeviceNM, wordLength).Trim();

			if (doSwapFg)
			{
				int tempLen = wordLength;
				if (wordLength % WordByteSize != 0)
				{
					//奇数要素での初期化にならないように偶数にする
					tempLen++;
				}

				char[] temp = new char[tempLen];
				for (int i = 0; i < wordLength; i += WordByteSize)
				{
					temp[i] = org[i + 1];
					temp[i + 1] = org[i];
				}
				if (wordLength % WordByteSize != 0)
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

		#region SetString

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

			//string logMsg = string.Format("SetString() addr:{0} data:{1}", hexAddressWithDeviceNM, data);
			//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);

			byte[] byteStr = Encoding.GetEncoding(encStr).GetBytes(data);

			List<byte> byteList = byteStr.ToList();
			if (Math.IEEERemainder(byteStr.Length, 2) != 0)
			{
				byteList.Add(0x00);
				byteStr = byteList.ToArray();
			}

			byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWriteCode, DataType.Channel, byteStr.Count() / 2, byteStr);
			string logStr = string.Empty;
			//foreach (byte cmdbyte in cmd)
			//{
			//	logStr += Convert.ToString(cmdbyte);
			//}
			//Log.SysLog.Info(string.Format("送信コマンド(⇒PLC):{0}", logStr));

			//logStr = string.Empty;
			//foreach (byte cmdbyte in byteStr)
			//{
			//	logStr += Convert.ToString(cmdbyte);
			//}
			//Log.SysLog.Info(string.Format("送信データ:{0}", logStr));

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

    }
}
