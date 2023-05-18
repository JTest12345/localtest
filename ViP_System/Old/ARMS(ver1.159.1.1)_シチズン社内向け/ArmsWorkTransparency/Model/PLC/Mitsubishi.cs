using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using ArmsApi;
using System.Threading;

namespace ArmsWorkTransparency.Model.PLC
{
    /// <summary>
    /// 三菱PLC UDP接続 Binary通信
    /// </summary>
    public class Mitsubishi : IDisposable, IPLC
    {
        private UdpClient udp;

        public string IPAddress { get; set; }
        public int Port { get; set; }

        static object lockobj = new object();

        private const int MAX_DEVICE_READ_POINTS = 600;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public Mitsubishi(string address, int port) 
        {
			this.udp = new UdpClient(address, port);
            this.udp.Client.ReceiveTimeout = 1000;

            this.IPAddress = address;
            this.Port = port;
        }

        #region IDisposable
        /// <summary>
        /// デストラクタ
        /// </summary>
        ~Mitsubishi()
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
                throw new ApplicationException("不正なPLCアドレスです:" + hexAddressWithDeviceNM);
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

        public byte[] executeCommand(byte[] bynaryCommand)
        {
            return executeCommand(bynaryCommand, true);
        }

        private byte[] executeCommand(byte[] bynaryCommand, bool canRetry)
        {
            byte[] sendBytes = bynaryCommand;

            try
            {
                //ソケット上限超えを防ぐため、ロックする。
                lock (lockobj)
                {
                    udp.Client.Blocking = true;
                    udp.Send(sendBytes, sendBytes.Length);

                    //データを受信する
                    System.Net.IPEndPoint remoteEP = null;
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
            return GetBit(hexAddressWithDeviceNM, 1);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <returns></returns>
        public string GetBit(string hexAddressWithDeviceNM, int length)
        {
            byte[] cmd = createCommand(hexAddressWithDeviceNM, commandRead, subCommandBitRW, length, null);

            //PLCコマンド実行
            byte[] response = executeCommand(cmd);
            string outputstring;
            bool isSuccess = parseBitResponseData(response, length, out outputstring);
            if (isSuccess == true)
            {
                //Log.SysLog.Info("[GetBit]Address:" + hexAddressWithDeviceNM + " " + outputstring);

                return outputstring;
            }
            else
            {
                //異常終了
                //Log.RBLog.Error("PLC異常応答:" + response);
                throw new ApplicationException("PLC異常応答");
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
                return Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY;
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
                //Log.RBLog.Error("PLC異常応答:" + response);
                throw new ApplicationException("PLC異常応答");
            }
        }

		public int GetWordAsDecimalData(string hexAddressWithDeviceNM) 
		{
			return GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
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

		public string GetMachineNo(string hexAddressWithDeviceNm) 
		{
			return GetMachineNo(hexAddressWithDeviceNm, MAGAZINE_NO_WORD_LENGTH);
		}

		public string GetMachineNo(string hexAddressWithDeviceNm, int wordLength)
		{
			string qr = string.Empty;

			string org = this.GetWord(hexAddressWithDeviceNm, wordLength).Trim();
			qr = org;

			//Null文字を置換
			qr = qr.Replace("\0", "");

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
            if (org == Mitsubishi.BIT_READ_TIMEOUT_VALUE_ARRAY)
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

		public bool SetString(string hexAddressWithDeviceNM, string data)
		{
			byte[] byteStr = Encoding.ASCII.GetBytes(data);
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
			Console.WriteLine(string.Format("LMマーキングバイト列:{0}", logStr));

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
