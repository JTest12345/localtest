using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using System.ComponentModel;
using System.Diagnostics;

namespace EICS
{
	/// <summary>
	/// HSMS通信
	/// </summary>
	public class ConnectHSMS : IDisposable
	{
		/// <summary>TCP通信インスタンス</summary>
		private TcpClient tcp = null;

		/// <summary></summary>
		private NetworkStream ns = null;

		/// <summary></summary>
		private bool MustEndConnectFG;
		private bool IntervalLinkTestRequestFG;
		private bool RequestableLinkTestFG;

		private DateTime LastLinkTestReqDT;

		private BackgroundWorker bwLinkTestRequester = new BackgroundWorker();

		/// <summary>
		/// コンストラクタ
		/// </summary>
		/// <param name="ipAddressNO"></param>
		public ConnectHSMS(string ipAddressNO, int portNO)
		{
			MustEndConnectFG = false;
			IntervalLinkTestRequestFG = false;
			RequestableLinkTestFG = true;

			tcp = new System.Net.Sockets.TcpClient(ipAddressNO, portNO);
			tcp.Client.ReceiveTimeout = 50000;
			tcp.Client.SendTimeout = 10000;

			//HSMS通信確立要求
			SendWord(GetRequestSelectWord());
		}

		public void DisconnectHSMS()
		{
			//bwLinkTestRequester.CancelAsync();
			if (ns != null) { ns.Close(); ns.Dispose(); }
			if (tcp != null) { tcp.Close(); }

		}

		#region IDisposable メンバ

		public void Dispose()
		{
		}

		#endregion

		/// <summary>
		/// SF受信
		/// </summary>
		/// <returns></returns>
		public ReceiveMessageInfo GetReceiveMessage()
		{
			int resSize = 0;
			int totalReceiveLen = 0;
			byte[] msgLenBytes = new byte[4];
			string strMsgLen = string.Empty;
			List<byte> receiveByteList = new List<byte>();

			ns = tcp.GetStream();

			if (tcp.Available <= 0)
			{
				return null;
			}

			DateTime lastCommunicationTime = DateTime.Now;
			resSize = ns.Read(msgLenBytes, 0, 4);

			if (resSize == 0)
				return null;

			//受信するべきデータ長：メッセージ長の格納部(4byte) + メッセージ長
			int msgLen = ReceiveMessageInfo.GetIndexLength(msgLenBytes) + resSize;


			byte[] receiveBuffer = new byte[msgLen];

			totalReceiveLen += resSize;
			receiveByteList.AddRange(msgLenBytes);

			System.IO.MemoryStream ms = new System.IO.MemoryStream();

			try
			{
				do
				{
					//受信バッファサイズ：受信すべきデータ長 - 受信済みデータ長 = 残りの受信データ長
					receiveBuffer = new byte[msgLen - totalReceiveLen];
					resSize = ns.Read(receiveBuffer, 0, msgLen - totalReceiveLen);

					//受信データが受信バッファサイズに満たない場合、0埋めされてしまう為の対策。今回受信分のデータ長だけをバッファから抜き出す
					receiveBuffer = receiveBuffer.Take(resSize).ToArray();

					//受信データサイズが0byteの場合
					if (resSize == 0)
					{
						TimeSpan duration = DateTime.Now - lastCommunicationTime;

						//前回データを受けてから装置からの送信データが無い期間がタイムアウトに達したかどうか
						if (duration.TotalSeconds >= Constant.TIMEOUT_FROM_STOPPED_TRANSMIT)
						{
							throw new ApplicationException(string.Format(Constant.MessageInfo.Message_106, Constant.TIMEOUT_FROM_STOPPED_TRANSMIT, totalReceiveLen, msgLen));
						}

						continue;
					}


					System.Diagnostics.Debug.WriteLine("受信データ： " + ReceiveMessageInfo.GetAllMessage(receiveBuffer));

					totalReceiveLen += resSize;

					lastCommunicationTime = DateTime.Now; //データを受信した最終時刻を取得

					receiveByteList.AddRange(receiveBuffer);


				} while (totalReceiveLen < msgLen); //受信したデータサイズが受信すべきデータ長以上となったらデータ受信のループから抜ける

				ms.Write(receiveByteList.ToArray(), 0, msgLen);

				return ReceiveMessageInfo.GetReceiveMessage(ms.ToArray());
			}
			catch (SocketException)
			{
				throw new ApplicationException(Constant.MessageInfo.Message_79);
			}
			finally
			{
				if (ms != null) { ms.Close(); ms.Dispose(); }
			}

		}

		/// <summary>
		/// SF送信
		/// </summary>
		/// <param name="ReceiveMessageInfo"></param>
		/// <returns></returns>
		public ReceiveMessageInfo SendMessage(ReceiveMessageInfo rMessageInfo, ref bool firstContactFG)
		{
			switch (rMessageInfo.ReceiveSF)
			{
				case "Select":
					if (rMessageInfo.HeaderByte3 != "0")
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("HSMS通信エラー(SF=Select):{0}", Constant.MessageInfo.Message_81), "SECS通信ログ");
						throw new ApplicationException(Constant.MessageInfo.Message_81);
					}
					rMessageInfo.SendSF = "Select";
					break;
				case "LinkTest":
					byte[] sendWordStr;

#if Debug
                    if (firstContactFG)
                    {
                        sendWordStr = GetS1F13Word();
                        SendWord(sendWordStr);
                        rMessageInfo.SendSF = "S1F13";
                        rMessageInfo.SendAllMessage = ReceiveMessageInfo.GetAllMessage(sendWordStr);
                        firstContactFG = false;
                        return rMessageInfo;
                    }
                    rMessageInfo.SendSF = "LinkTest";	
#else
					if (firstContactFG)
					{
						sendWordStr = GetS1F13Word();
						SendWord(sendWordStr);
						rMessageInfo.SendSF = "S1F13";
						rMessageInfo.SendAllMessage = ReceiveMessageInfo.GetAllMessage(sendWordStr);
						firstContactFG = false;
						return rMessageInfo;
					}

					rMessageInfo.SendSF = "LinkTest";
#endif
					return rMessageInfo;
				case "Separate":
					MustEndConnectFG = true;
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("HSMS通信(SF=Separate):{0}", Constant.MessageInfo.Message_82), "SECS通信ログ");
					throw new ApplicationException(Constant.MessageInfo.Message_82);
					//return rMessageInfo;

				//通信確立要求
				case "S1F13":
					SendWord(GetS1F14Word(rMessageInfo.HeaderVAL));
					rMessageInfo.SendSF = "S1F14";
					return rMessageInfo;

				//通信確立要求応答
				case "S1F14":
					//SendWord(GetS2F37Word(false));
					SendWord(GetS2F37Word(true));
					rMessageInfo.SendSF = "S2F37";
					return rMessageInfo;

				//オンライン確認
				case "S1F1":
					SendWord(GetS1F2Word(rMessageInfo.HeaderVAL));
					rMessageInfo.SendSF = "S1F2";
					break;

				//レポート受信
				case "S6F11":
					SendWord(GetS6F12Word(rMessageInfo.HeaderVAL));
					rMessageInfo.SendSF = "S6F12";

					string msgTypeCD = ReportMessageInfo.GetCeID(rMessageInfo.DataVAL).ToString();

					if (msgTypeCD == "1185" || msgTypeCD == "1186")
					{
						rMessageInfo.MessageJudgeFG = true;
					}
					else
					{
						rMessageInfo.MessageJudgeFG = false;
					}

					return rMessageInfo;

				//アラーム応答
				case "S5F1":
					SendWord(GetS5F2Word(rMessageInfo.HeaderVAL));
					rMessageInfo.SendSF = "S5F2";
					return rMessageInfo;

				//タイムアウト
				case "S9F9":
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("HSMS通信(SF=S9F9):{0}", Constant.MessageInfo.Message_83), "SECS通信ログ");
					throw new ApplicationException(Constant.MessageInfo.Message_83);

				default:
					return rMessageInfo;
			}

			rMessageInfo.LinkTestRequestFG = true;

			return rMessageInfo;
		}

		public void SendWord(byte[] word)
		{
			ns = tcp.GetStream();
			ns.Write(word, 0, word.Length);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="isJudgeResult">送信する判定結果(OK:true/NG or HostSTP:false isSendOKAlwaysがtrueならisJudgeResultの値に関係なく常にtrueが送信される)</param>
		/// <param name="msgTypeCD"></param>
		/// <param name="isSendOKAlways">常に判定結果OKを送信するかどうか(trueの場合、isJudgeResultの値は無視される)</param>
		public void SendJudgeResult(bool isJudgeResult, string msgTypeCD, bool isSendOKAlways)
		{
			if (isSendOKAlways)
			{
				SendWord(GetS2F41Word(true, msgTypeCD));
			}
			else
			{
				SendWord(GetS2F41Word(isJudgeResult, msgTypeCD));
			}
			//if (isJudgeResult == false)
			//{
			//    SendWord(GetRequestSeparateWord());
			//}
		}

		/// <summary>
		/// HSMS通信確立要求
		/// </summary>
		/// <returns></returns>
		private byte[] GetRequestSelectWord()
		{
			byte[] word = new byte[]
            {
                0x00,0x00,0x00,0x0A,    //メッセージ長  ：4byte
                0xFF,0xFF,              //デバイスID    ：2byte
                0x00,                   //ヘッダバイト2 ：1byte
                0x00,                   //ヘッダバイト3 ：1byte
                0x00,                   //Pタイプ       ：1byte
                0x01,                   //Sタイプ       ：1byte
                0x80,0x00,0x00,0x01     //システムバイト：4byte                
            };

			return word;
		}

		/// <summary>
		/// HSMS通信作動確認
		/// </summary>
		/// <returns></returns>
		public byte[] GetRequestLinkTestWord()
		{
			byte[] word = new byte[]
            {
                0x00,0x00,0x00,0x0A,    //メッセージ長  ：4byte
                0xFF,0xFF,              //デバイスID    ：2byte
                0x00,                   //ヘッダバイト2 ：1byte
                0x00,                   //ヘッダバイト3 ：1byte
                0x00,                   //Pタイプ       ：1byte
                0x05,                   //Sタイプ       ：1byte
                0x80,0x00,0x00,0x02     //システムバイト：4byte                
            };

			return word;
		}

		/// <summary>
		/// HSMS通信切断要求
		/// </summary>
		/// <returns></returns>
		public byte[] GetRequestSeparateWord()
		{
			byte[] word = new byte[]
            {
                0x00,0x00,0x00,0x0A,    //メッセージ長  ：4byte
                0xFF,0xFF,              //デバイスID    ：2byte
                0x00,                   //ヘッダバイト2 ：1byte
                0x00,                   //ヘッダバイト3 ：1byte
                0x00,                   //Pタイプ       ：1byte
                0x09,                   //Sタイプ       ：1byte
                0x00,0x00,0x00,0x00     //システムバイト：4byte                
            };

			return word;
		}

		/// <summary>
		/// 通信確立要求応答
		/// </summary>
		private byte[] GetS1F13Word()
		{
			byte[] word = new byte[]
            {
                //メッセージ長  ：4byte
                //ヘッダ        ：10byte
                //データ        ：2byte(リスト0)
                //オリジナル
                0x00,0x00,0x00,0x0C,                                
                0x00,0x01,0x81,0x0D,0x00,0x00,0x00,0x00,0x00,0x03
                ,0x01,0x00                                                                                 

            };

			return word;
		}

		/// <summary>
		/// 通信確立要求応答
		/// </summary>
		private byte[] GetS1F14Word(byte[] rHeaderValue)
		{
			byte[] word = new byte[]
            {
                //メッセージ長  ：4byte
                //ヘッダ        ：10byte
                //データ        ：7byte 
                0x00,0x00,0x00,0x11,                                                                            
                0x00,0x01,0x01,0x0E,0x00,0x00,rHeaderValue[6],rHeaderValue[7],rHeaderValue[8],rHeaderValue[9],  
                0x01,0x02,0x21,0x01,0x00,0x01,0x00                                                                        
            };

			return word;
		}

		/// <summary>
		/// オンライン確認応答
		/// </summary>
		private byte[] GetS1F2Word(byte[] rHeaderValue)
		{
			byte[] word = new byte[]
            {
                //メッセージ長  ：4byte
                //ヘッダ        ：10byte
                //データ        ：2byte  
                0x00,0x00,0x00,0x0C,                                                                            
                0x00,0x01,0x01,0x02,0x00,0x00,rHeaderValue[6],rHeaderValue[7],rHeaderValue[8],rHeaderValue[9],  
                0x01,0x00                                                                                                
            };

			return word;
		}

		/// <summary>
		/// レポート応答
		/// </summary>
		public byte[] GetS6F12Word(byte[] rHeaderValue)
		{
			byte[] word = new byte[]
            {
                //メッセージ長  ：4byte
                //ヘッダ        ：10byte
                //データ        ：3byte
                0x00,0x00,0x00,0x0D,
                0x00,0x01,0x06,0x0C,0x00,0x00,rHeaderValue[6],rHeaderValue[7],rHeaderValue[8],rHeaderValue[9],                                                                                                 
                0x21, 0x01, 0x00
            };

			return word;
		}

		/// <summary>
		/// アラーム応答
		/// </summary>
		public byte[] GetS5F2Word(byte[] rHeaderValue)
		{
			byte[] word = new byte[]
            {                  
                //メッセージ長  ：4byte
                //ヘッダ        ：10byte
                //データ        ：3byte 
                0x00,0x00,0x00,0x0D,
                0x00,0x01,0x05,0x02,0x00,0x00,rHeaderValue[6],rHeaderValue[7],rHeaderValue[8],rHeaderValue[9],                                                                                                           
                0x21, 0x01, 0x00                                                                                         
            };

			return word;
		}

		/// <summary>
		/// リモートコマンド実行要求
		/// </summary>
		public byte[] GetS2F41Word(bool isJudgeResult, string msgTypeCD)
		{
			byte[] word = new byte[]
            {
                //メッセージ長  ：4byte
                //ヘッダ        ：10byte
                //データ        ：14byte
                0x00,0x00,0x00,0x18,                                
                0x00,0x01,0x82,0x29,0x00,0x00,0x00,0x00,0x00,0x22,  
                0x01, 0x02, 0x41, 0x08,                             
                0x43, 0x48, 0x45, 0x43, 0x4b, 0x5f, 0x4f, 0x4b,//OK
                0x01, 0x00
            };

			if (!isJudgeResult && msgTypeCD == "1185")//NG
			{
				word[24] = 0x4e;
				word[25] = 0x47;
			}
			else if (!isJudgeResult && msgTypeCD == "1186")//HOST_STP
			{
				word[18] = 0x48;
				word[19] = 0x4F;
				word[20] = 0x53;
				word[21] = 0x54;
				word[22] = 0x5F;
				word[23] = 0x53;
				word[24] = 0x54;
				word[25] = 0x50;
			}

			return word;
		}

		/// <summary>
		/// レポートCEIDの可/不可を定義する
		/// </summary>
		public byte[] GetS2F37Word(bool flag)
		{
			byte[] word = null;

			if (!flag)
			{
				word = new byte[]
                {
                    //メッセージ長  ：4byte
                    //ヘッダ        ：10byte
                    //データ        ：7byte
                    0x00,0x00,0x00,0x11,                                
                    0x00,0x01,0x82,0x25,0x00,0x00,0x00,0x00,0x00,0x05,  
                    0x01, 0x02, 0x25, 0x01, 0x00, 0x01, 0x00
                };
			}
			else
			{
				word = new byte[]
                {
                    //メッセージ長  ：4byte
                    //ヘッダ        ：10byte
                    //データ        ：15byte
                    0x00,0x00,0x00,0x19,                                
                    0x00,0x01,0x82,0x25,0x00,0x00,0x00,0x00,0x00,0x05,  
                    0x01, 0x02, 0x25, 0x01, 0xff, 0x01, 0x02, 0xa9, 0x02, 0x04, 0xa1, 0xa9, 0x02, 0x04, 0xa2
                };
			}

			return word;
		}

		public bool GetMustEndConnectFG()
		{
			return MustEndConnectFG;
		}

		public bool GetIntervalLinkTestReqFG()
		{
			return IntervalLinkTestRequestFG;
		}

		public void SetIntervalLinkTestReqFG()
		{
			IntervalLinkTestRequestFG = true;
			RequestableLinkTestFG = true;
		}

		public void SendLinkTestRequest()
		{
			if (RequestableLinkTestFG)
			{
				SendWord(GetRequestLinkTestWord());
				LastLinkTestReqDT = DateTime.Now;
				RequestableLinkTestFG = false;
			}
		}

		public bool IsIntervalLinkTestRspWaitingState()
		{
			//LinkTestを要求した日時が格納されている場合、定期的なLinkTestに対する応答待ちである為trueを返す。
			if (LastLinkTestReqDT != null && LastLinkTestReqDT != DateTime.MinValue && RequestableLinkTestFG == false)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public DateTime GetLastLinkTestRequestDT()
		{
			return LastLinkTestReqDT;
		}

		public bool IsCleanLinkTestRsp(ReceiveMessageInfo rMessageInfo)
		{
			//LinkTestレスポンス待ちの状態かどうかをチェック
			if (IsIntervalLinkTestRspWaitingState())
			{
				//LinkTestを最後に送信してから今までの時間を計算
				TimeSpan ts = DateTime.Now - LastLinkTestReqDT;

				//LinkTestを最後に要求してからの時間が閾値オーバーの場合、LinkTest応答に異常ありと判断し、falseを返す。
				if (ts.TotalMilliseconds > 5000)
				{
					return false;
				}

				if (rMessageInfo != null)
				{
					if (rMessageInfo.ReceiveSF == "LinkTest")
					{//受信SFがLinkTestなら、最終LinkTest要求日時の初期化
						//LastLinkTestReqDT = new DateTime();
						//LinkTest送信許可フラグON
						RequestableLinkTestFG = true;
					}
				}
			}
			//LinkTest応答問題無しの意味合いでtrueを返す。 LinkTestを送信しておらず、LinkTestレスポンス待ちで無い場合もtrueを返す。
			return true;
		}



	}
}
