using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using log4net;

namespace EICS
{
    public class PLC
    {
		public const string DT_STR = "STR";
		public const string DT_BINARY = "BIN";
		public const string DT_DEC_32BIT = "DEC_32BIT";
		public const string DT_DEC_32BIT_REV = "DEC_32BIT_REV";
		public const string DT_DEC_16BIT = "DEC_16BIT";
		public const string DT_UDEC_32BIT = "UDEC_32BIT";
		public const string DT_UDEC_16BIT = "UDEC_16BIT";
		public const string DT_HEX_32BIT = "HEX_32BIT";
		public const string DT_HEX_16BIT = "HEX_16BIT";
		public const string DT_BOOL = "BOOL";
		public const string DT_DOUBLE = "DOUBLE";
		public const string DT_FLOAT = "FLOAT_32BIT";
		public const string DT_FLOAT_16BIT = "FLOAT_16BIT";
		public const string DT_BCD32BIT = "BCD_32BIT";
		public const string DT_BCD16BIT = "BCD_16BIT";

        public static int GetDataSizeFromDataType(string DataTypeCD)
        {
            int retV = 1;

            if (DataTypeCD == PLC.DT_DEC_32BIT ||
				DataTypeCD == PLC.DT_DEC_32BIT_REV ||
                DataTypeCD == PLC.DT_UDEC_32BIT ||
                DataTypeCD == PLC.DT_HEX_32BIT ||
                DataTypeCD == PLC.DT_BCD32BIT ||
                DataTypeCD == PLC.DT_FLOAT)
            {
                retV = 2;
            }

            return retV;
        }

		public const string OUTPUT_NULL_Str = "NULL";

		public string HostAddressNO { get; set; }
		public int PortNO { get; set; }

		

		//private static Mutex mut = new Mutex(false, "PLCMutex");
		//private static Mutex mut2 = new Mutex(false, "PLC2ndMutex");

        /// <summary>
        /// Singletonインスタンス
        /// </summary>
		//private static PLC instance;

        /// <summary>
        /// 2ndインスタンス
        /// </summary>
		//private static PLC instance2;

        /// <summary>
        /// コンストラクタ
        /// </summary>
		public PLC(string hostAddressNO, int portNO)
		{
			HostAddressNO = hostAddressNO;
			PortNO = portNO;

			tcp = new System.Net.Sockets.TcpClient(hostAddressNO, portNO);

		}

        ~PLC()
        {
            if (ns != null)
            {
                ns.Close();
            }

			//if (ns2 != null)
			//{
			//    ns2.Close();
			//}

            if (tcp != null)
            {
                tcp.Close();
            }

			//if (tcp2 != null)
			//{
			//    tcp2.Close();
			//}
        }

		public bool ConnectedPLC()
		{
			return tcp.Connected;
		}

		public void ConnectPLC()
		{
			tcp = new System.Net.Sockets.TcpClient(HostAddressNO, PortNO);
		}


		//public static PLC GetInstance(string sIpAddress)
		//{
		//    if (instance == null)
		//    {
		//        instance = new PLC();
		//        instance.InstanceNo = 1;
		//        instance.host = sIpAddress;
		//    }

		//    return instance;
		//}

		//public static PLC Get2ndInstance(string sIpAddress)
		//{

		//    if (instance2 == null)
		//    {
		//        instance2 = new PLC();
		//        instance2.InstanceNo = 2;
		//        instance2.host = sIpAddress;
		//    }

		//    return instance2;
		//}

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

        /// <summary>
        /// タイムアウト時応答
        /// </summary>
        public const string BIT_READ_TIMEOUT_VALUE = "TIMEOUT";

        /// <summary>
        /// TCPソケット
        /// </summary>
        private System.Net.Sockets.TcpClient tcp;

        /// <summary>
        /// 2ndソケット
        /// </summary>
		//private static System.Net.Sockets.TcpClient tcp2;

        /// <summary>
        /// ネットワークストリーム
        /// </summary>
        private static System.Net.Sockets.NetworkStream ns = null;

        /// <summary>
        /// 2ndネットワークストリーム
        /// </summary>
        private static System.Net.Sockets.NetworkStream ns2 = null;


        #region ADDRESS
        //■PC→プラズマ/////////////////////////////////////////
        /// <summary>
        /// パラメータ書込み完了フラグ
        /// </summary>
        public const string SET_PARAM_SETTING_OK = "B000200";
        /// <summary>
        /// 工程確認(ON＝DB後, OFF＝DBｵｰﾌﾞﾝ後) 
        /// </summary>
        public const string SET_PROCESS_TEACH = "B000201";
        /// <summary>
        /// 判定結果待ち受けフラグEE
        /// </summary>
        public const string SET_CHECK_EE_FLG = "B000211";
        /// <summary>
        /// 判定結果待ち受けフラグJE
        /// </summary>
        public const string SET_CHECK_JE_FLG = "B000212";
        /// <summary>
        /// 判定結果待ち受けフラグLE
        /// </summary>
        public const string SET_CHECK_LE_FLG = "B000213";
        /// <summary>
        /// RF入射
        /// </summary>
        public const string SET_PARAM_RF = "W000200";
        /// <summary>
        /// 処理時間
        /// </summary>
        public const string SET_PARAM_TIME = "W000202";
        /// <summary>
        /// Arガス流量
        /// </summary>
        public const string SET_PARAM_AR = "W000204";
        /// <summary>
        /// 真空度設定
        /// </summary>
        public const string SET_PARAM_VACUUM = "W000206";
        /// <summary>
        /// 放電開始圧力設定
        /// </summary>
        public const string SET_PARAM_DISCHARGE = "W000208";

        /// <summary>
        /// 判定結果(OK = 1, NG = 0)
        /// </summary>
        public const string SET_JUDGE = "W000210";

        /// <summary>
        /// EEファイル判定結果(OK = 1, NG = 0)
        /// </summary>
        public const string SET_EE_JUDGE = "W000211";

        /// <summary>
        /// JEファイル判定結果(OK = 1, NG = 0)
        /// </summary>
        public const string SET_JE_JUDGE = "W000212";

        /// <summary>
        /// LEファイル判定結果(OK = 1, NG = 0)
        /// </summary>
        public const string SET_LE_JUDGE = "W000212";

        //■プラズマ→PC/////////////////////////////////////////
        /// <summary>
        /// マガジンＮｏ書込み完了フラグ
        /// </summary>
        public const string GET_MAGAZINE_SETTING_OK = "B000300";

        /// <summary>
        /// 装置運転中(ON = 有効, OFF = 無効)
        /// </summary>
        public const string GET_PLA_READY = "B00030E";

        /// <summary>
        /// 傾向管理有効無効(ON = 有効. OFF = 無効)
        /// </summary>
        public const string GET_PLA_TRENDSWITCH = "B00030F";

        /// <summary>
        /// マガジンＮｏ(W000300～000305) ASCII12文字分
        /// </summary>
        public const string GET_MAGAZINE = "W000300";

        #endregion

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

        #region convertAddressToByteArray

        private byte[] convertAddressToByteArray(string hexAddressWithDeviceNM)
        {
            if (hexAddressWithDeviceNM.Length != 7)
            {
                throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
            }

            // アドレスの先頭一文字を除いて6桁
            string address = hexAddressWithDeviceNM.Substring(1).PadLeft(6, '0');
            List<byte> retv = new List<byte>();

            for (int i = 2; i <= address.Length; i += 2)
            {
                string sub = address.Substring(address.Length - i, 2);
                retv.Add(Convert.ToByte(sub, 16));
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
            Mutex currentMutex = null;

			//if (InstanceNo == 1)
			//{
			//    string host = instance.host;
			//    int port = 1025;

			//    currentMutex = mut;
			//    currentNS = ns;

			//    if (tcp == null)
			//    {
			//        tcp = new System.Net.Sockets.TcpClient(host, port);
			//    }
			//    else if (tcp.Connected == false)
			//    {
			//        tcp = new System.Net.Sockets.TcpClient(host, port);
			//    }

			//    currentTcp = tcp;
			//}
			//else if (InstanceNo == 2)//未使用
			//{
			//    string host = instance2.host;
			//    int port = 1025;

			//    currentMutex = mut2;
			//    currentNS = ns2;

			//    if (tcp2 == null)
			//    {
			//        tcp2 = new TcpClient(host, port);
			//    }
			//    else if (tcp2.Connected == false)
			//    {
			//        tcp2 = new TcpClient(host, port);
			//    }

			//    currentTcp = tcp2;
			//}

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
                        //Log.WriteError("PLC通信切断発生");
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
                //currentMutex.ReleaseMutex();
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
        public bool SetBit(string hexAddressWithDeviceNM, int points, string data)
        {
            byte[] writeData = parseStringDataToPLCBitData(data);
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWrite, subCommandBitRW, points, writeData);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            string output;
            bool isSuccess = parseBitResponseData(response, points, out output);
            if (isSuccess == true)
            {
                return true;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return false;
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

        #endregion

        #region GetWordAsDecimalData
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns>タイムアウト時　-1</returns>
        public int GetWordAsDecimalData(string hexAddressWithDeviceNM)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandWordRW, 1, null);

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



        #endregion

        #region SetWordAsDecimalData

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandWrite, subCommandWordRW, 1, BitConverter.GetBytes(Convert.ToInt16(data)));

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            string output;

            //応答解析はBITとして処理
            bool isSuccess = parseBitResponseData(response, 1, out output);
            if (isSuccess == true)
            {
                return true;
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + response);
                return false;
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



        #endregion

        #region GetMagazineNo

        public string GetMagazineNo(string hexAddressWithDeviceNm)
        {
            string qr = string.Empty;

            string org = this.GetWord(hexAddressWithDeviceNm, MAGAZINE_NO_WORD_LENGTH).Trim();
            qr = org;

            //Null文字を置換
            qr = qr.Replace("\0", "");

            if (qr.Trim().Split(' ').Length >= 2)
            {
                return qr.Trim().Split(' ')[1].Replace("","");
            }
            else
            {
                return qr.Trim();
            }

        }

        #endregion

        /// <summary>
        /// PLCコマンド受付状態
        /// </summary>
        /// <returns></returns>
        public bool IsPLCReadyToCommand()
        {
            //string retv = GetBit(STAT_ROBOT_COMMAND_READY);
            string retv = GetBit(GET_PLA_READY);

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
