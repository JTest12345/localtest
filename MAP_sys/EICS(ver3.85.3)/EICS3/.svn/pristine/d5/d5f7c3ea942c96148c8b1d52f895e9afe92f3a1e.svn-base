using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.ComponentModel;
using EICS.Database;
using System.Net;
using System.Net.Sockets;

namespace EICS
{
	class ErrorCommunicator
	{
		const int SEND_MAX_MSGLENGTH = 4095;

		BackgroundWorker ErrorSender = new BackgroundWorker();
		BackgroundWorker ErrorReceiver = new BackgroundWorker();

		public class ErrorInfo
		{
			public int DBServLineCD { get; set; }
			public DateTime AlertDT { get; set; }
			public LSETInfo DiscovererLsetInfo { get; set; }
			public string LotNO { get; set; }
			public string MagNO { get; set; }
			public int QcParamNO { get; set; }
			public string ErrorMessage { get; set; }
		}

		private static ErrorCommunicator logInstance;
		public Queue<ErrorInfo> logMessageQue;
		static public System.Object lockThis = new System.Object();

		public static ErrorInfo GetContactError(int dbServLineCD, DateTime alertDT, LSETInfo lsetInfo, string lotNO, string magazineNO, int qcParamNO, string errorMessage)
		{
			ErrorInfo contactError = new ErrorInfo();

			contactError.DBServLineCD = dbServLineCD;
			contactError.AlertDT = alertDT;
			contactError.DiscovererLsetInfo = lsetInfo;
			contactError.LotNO = lotNO;
			contactError.MagNO = magazineNO;
			contactError.QcParamNO = qcParamNO;
			contactError.ErrorMessage = errorMessage;

			return contactError;
		}

		private ErrorCommunicator()
		{
			logMessageQue = new Queue<ErrorInfo>();
		}

		public static ErrorCommunicator GetInstance()
		{
			if (logInstance == null)
			{
				logInstance = new ErrorCommunicator();
			}

			return logInstance;
		}

		public static void SendError(ErrorInfo errInfo)
		{
			//ロットNOでTnLOTTを調べ工程との照合をかける
			List<string> causeAssetsNMList = Prm.GetCauseAssetsNMList(errInfo.DBServLineCD, errInfo.QcParamNO);

			foreach (string causeAssetsNM in causeAssetsNMList)
			{
				List<int> receiveLineCDList = (Lott.GetPassedLine(errInfo.DBServLineCD, causeAssetsNM, errInfo.LotNO));

				foreach (int receiveLineCD in receiveLineCDList)
				{
					//起因の装置が自ラインの場合、遠隔通知は必要なし
					if (errInfo.DBServLineCD == receiveLineCD)
					{
						continue;
					}

					List<Lott> lottList = Lott.GetData(errInfo.DBServLineCD, causeAssetsNM, errInfo.LotNO);
					if (lottList.Count == 0)
					{
						continue;
					}

					Lott lott = lottList.First();

					string message = string.Format("遠隔通知エラー[line:{0} {1} {2}]⇒[line:{3} {4} {5}] 内容:{6}", errInfo.DBServLineCD, errInfo.DiscovererLsetInfo.ModelNM, errInfo.DiscovererLsetInfo.EquipmentNO, lott.PassedLineCD, lott.PassedAssetsNM, lott.PassedEquipmentNO, errInfo.ErrorMessage);

					List<CommunicateLine> comLineList = CommunicateLine.GetData(errInfo.DBServLineCD);

					//関連ライン全てに遠隔通知
					foreach (CommunicateLine comLine in comLineList)
					{
						ErrCommunicate.Insert(errInfo.DBServLineCD, comLine.HostLineCD, errInfo.AlertDT, lott.PassedEquipmentNO, comLine.RemoteLineCD, errInfo.ErrorMessage);
					}
				}
			}
		}

		private void SendMessage()
		{
		}

		//private void SendMessageOnTcp(string ipAddr, string message)
		//{
		//    SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

		//    using (TcpClient tcp = new TcpClient(ipAddr, commonSettingInfo.SendPort))
		//    {
		//        NetworkStream ns = tcp.GetStream();


		//        byte[] byteMsg = System.Text.Encoding.UTF8.GetBytes(string.Format("{0}\0", message));

		//        if (byteMsg.Length > SEND_MAX_MSGLENGTH)
		//        {
		//            throw new ApplicationException(string.Format(Constant.MessageInfo.Message_150, byteMsg.Length, SEND_MAX_MSGLENGTH));
		//        }

		//        byte[] byteSize = System.Text.Encoding.UTF8.GetBytes(string.Format("{0:x3}", byteMsg.Length));

		//        List<byte> sendByteList = byteSize.ToList();

		//        sendByteList.AddRange(byteMsg);

		//        ns.Write(sendByteList.ToArray(), 0, sendByteList.Count);
		//    }
		//}

		public static List<ErrCommunicate> GetReceiveError(int dbServerLineCD, int receiveLineCD)
		{
			List<ErrCommunicate> retv = new List<ErrCommunicate>();

			List<ErrCommunicate> errComList = ErrCommunicate.GetData(dbServerLineCD, receiveLineCD);

			foreach(ErrCommunicate errCom in errComList)
			{
				retv.Add(errCom);
			}
			return retv;
		}

		//private void ReceiveMessageOnTcp()
		//{
		//    //ListenするIPアドレスを決める
		//    IPAddress ipAdd = Dns.GetHostEntry("localhost").AddressList[0];

		//    SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

		//    //TcpListenerオブジェクトを作成する
		//    TcpListener listener = new TcpListener(ipAdd, commonSettingInfo.ReceivePort);

		//    //Listenを開始する
		//    listener.Start();
		//    Console.WriteLine("Listenを開始しました({0}:{1})。", ((IPEndPoint)listener.LocalEndpoint).Address, ((IPEndPoint)listener.LocalEndpoint).Port);

		//    //接続要求があったら受け入れる
		//    TcpClient client = listener.AcceptTcpClient();
		//    Console.WriteLine("クライアント({0}:{1})と接続しました。", ((IPEndPoint)client.Client.RemoteEndPoint).Address, ((IPEndPoint)client.Client.RemoteEndPoint).Port);

		//    //NetworkStreamを取得
		//    NetworkStream ns = client.GetStream();

		//    //クライアントから送られたデータを受信する
		//    System.Text.Encoding enc = System.Text.Encoding.UTF8;
		//    System.IO.MemoryStream ms = new System.IO.MemoryStream();
		//    byte[] resBytes = new byte[4095];
		//    List<byte> receiveByteList = new List<byte>();

		//    do
		//    {
		//        //ヘッダ（メッセージ長を取得）
		//        int resSize = ns.Read(resBytes, 0, 3);

		//        int msgLen = ReceiveMessageInfo.GetIndexLength(resBytes);

		//        byte[] receiveBuffer;

		//        //一つのメッセージ受信で受信した総データ長
		//        int totalReceiveLen = 0;

		//        DateTime lastCommunicationTime = DateTime.Now;
		//        try
		//        {
		//            do
		//            {
		//                //受信バッファサイズ：受信すべきデータ長 - 受信済みデータ長 = 残りの受信データ長
		//                receiveBuffer = new byte[msgLen - totalReceiveLen];
		//                resSize = ns.Read(receiveBuffer, 0, msgLen - totalReceiveLen);

		//                //受信データが受信バッファサイズに満たない場合、0埋めされてしまう為の対策。今回受信分のデータ長だけをバッファから抜き出す
		//                receiveBuffer = receiveBuffer.Take(resSize).ToArray();

		//                //受信データサイズが0byteの場合
		//                if (resSize == 0)
		//                {
		//                    TimeSpan duration = DateTime.Now - lastCommunicationTime;

		//                    //前回データを受けてから装置からの送信データが無い期間がタイムアウトに達したかどうか
		//                    if (duration.TotalSeconds >= Constant.TIMEOUT_FROM_STOPPED_TRANSMIT)
		//                    {
		//                        throw new ApplicationException(string.Format(Constant.MessageInfo.Message_106, Constant.TIMEOUT_FROM_STOPPED_TRANSMIT, totalReceiveLen, msgLen));
		//                    }

		//                    continue;
		//                }


		//                System.Diagnostics.Debug.WriteLine("受信データ： " + ReceiveMessageInfo.GetAllMessage(receiveBuffer));

		//                totalReceiveLen += resSize;

		//                lastCommunicationTime = DateTime.Now; //データを受信した最終時刻を取得

		//                receiveByteList.AddRange(receiveBuffer);


		//            } while (totalReceiveLen < msgLen); //受信したデータサイズが受信すべきデータ長以上となったらデータ受信のループから抜ける

		//            ms.Write(receiveByteList.ToArray(), 0, msgLen);

		//            string outputMsg = System.Text.Encoding.UTF8.GetString(ms.ToArray());

		//        }
		//        catch (SocketException)
		//        {
		//            throw new ApplicationException(Constant.MessageInfo.Message_79);
		//        }
		//        finally
		//        {
		//            if (ms != null) { ms.Close(); ms.Dispose(); }
		//        }
		//        //受信したデータを蓄積する
		//        ms.Write(resBytes, 0, resSize);
		//    } while (ns.DataAvailable);

		//    //受信したデータを文字列に変換
		//    string recvData = enc.GetString(ms.ToArray());
		//    ms.Close();
		//    client.Close();
		//    listener.Stop();
		//    ns.Close();

		//    string[] recvDataArray = recvData.Split('\n');

		//    Thread.Sleep(3000);
		//}

		/// <summary>
		/// 登録先のラインCD
		/// </summary>
		/// <param name="errRegistedLineCD"></param>
		/// <param name="qcParamNO"></param>
		/// <returns></returns>
		public static bool IsMustSendToOtherLineError(int errRegistedLineCD, int qcParamNO)
		{
			if (string.IsNullOrEmpty(Prm.GetCauseAssetsNM(errRegistedLineCD, qcParamNO)))
			{	//PRMのCauseModel_NMが未設定ならエラーの遠隔通知機能がOFFのパラメータと認識
				return false;
			}
			else
			{	//CauseModel_NMに何らかの値が設定されている場合、エラーの遠隔通知機能がONのパラメータと認識
				return true;
			}

		}

		//private void Check
	}
}
