using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model.PLC
{
    /// <summary>
    /// キーエンス UDP接続
    /// </summary>
    public class Keyence : IDisposable, IPLC
    {
        private UdpClient udp;

        public string IPAddress { get; set; }
        public int Port { get; set; }

        static object lockobj = new object();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="address"></param>
        /// <param name="port"></param>
        public Keyence(string address, int port)
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
        ~Keyence()
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
        public const int MAGAZINE_NO_WORD_LENGTH = 10;

        /// <summary>
		/// 装置番号WORDアドレスの長さ
		/// </summary>
		public const int MACHINE_NO_WORD_LENGTH = 6;

        public static string[] BIT_READ_TIMEOUT_VALUE_ARRAY = new string[] { "TIMEOUT" };


        /// <summary>
        /// PLC応答から終了コード取得
        /// </summary>
        /// <param name="responseData"></param>
        /// <returns></returns>
        private bool isExitNormal(byte[] plcResponseData)
        {
            string res = System.Text.Encoding.UTF8.GetString(plcResponseData).Trim();
            {
                if (res == "OK")
                {
                    return true;
                }

                if (res == "E0")
                {
                    return false;
                }

                if (res == "E1")
                {
                    return false;
                }
            }

            return true;
        }

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
            string command = CommandType.WR.ToString() + " " + hexAddressWithDeviceNM + " " + data + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand(byteCmd);
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
            return string.Join("", GetBitArray(hexAddressWithDeviceNM, length)).Trim();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="hexAddressWithDeviceNM"></param>
        /// <param name="points"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public string[] GetBitArray(string hexAddressWithDeviceNM, int points)
        {
            string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + " " + points.ToString() + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd);

            if (isExitNormal(res) == true)
            {
                return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + command);
                return BIT_READ_TIMEOUT_VALUE_ARRAY;
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
            return string.Join("", GetWordArray(hexAddressWithDeviceNM, length)).Trim();
        }

        public string[] GetWordArray(string hexAddressWithDeviceNM, int points)
        {
            string command = CommandType.RDS.ToString() + " " + hexAddressWithDeviceNM + ".U" + " " + points.ToString() + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            byte[] res = executeCommand(byteCmd);

            if (isExitNormal(res) == true)
            {
                return System.Text.Encoding.UTF8.GetString(res).Replace("\r\n", "").Split(' ');
            }
            else
            {
                //異常終了
                //Log.WriteError("PLCエラー応答:" + command);
                return BIT_READ_TIMEOUT_VALUE_ARRAY;
            }
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

        #region GetMagazineNo

        public string GetMagazineNo(string hexAddressWithDeviceNm)
        {
            return GetMagazineNo(hexAddressWithDeviceNm, true);
        }

        public string GetMagazineNo(string hexAddressWithDeviceNm, int wordLength)
        {
            return GetMagazineNo(hexAddressWithDeviceNm, true, wordLength);
        }

        /// <summary>
        /// 不正応答時は空白
        /// </summary>
        /// <param name="hexAddressWithDeviceNm"></param>
        /// <param name="host"></param>
        /// <returns></returns>
        public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided)
        {
            return GetMagazineNo(hexAddressWithDeviceNm, notDevided, MAGAZINE_NO_WORD_LENGTH);
        }

        public string GetMagazineNo(string hexAddressWithDeviceNm, bool notDevided, int wordLength)
        {
            string qr = string.Empty;

            string[] org = this.GetBitArray(hexAddressWithDeviceNm, wordLength);
            if (org == BIT_READ_TIMEOUT_VALUE_ARRAY)
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

                magno = Order.NascaLotToMagLot(magno, seqno);
                return magno;
            }
            else
            {
                return qr.Trim();
            }
        }

        #endregion

        #region GetMachineNo

        public string GetMachineNo(string hexAddressWithDeviceNm)
        {
            return GetMachineNo(hexAddressWithDeviceNm, MAGAZINE_NO_WORD_LENGTH);
        }

        public string GetMachineNo(string hexAddressWithDeviceNm, int wordLength)
        {
            string qr = string.Empty;

            string[] org = this.GetBitArray(hexAddressWithDeviceNm, wordLength);
            if (org == BIT_READ_TIMEOUT_VALUE_ARRAY)
            {
                return string.Empty;
            }

            return GetMachineNo(org);
        }

        public string GetMachineNo(string[] plcResponseBitArray)
        {
            string qr = string.Empty;

            foreach (string orgs in plcResponseBitArray)
            {
                qr += System.Text.Encoding.UTF8.GetString(BitConverter.GetBytes(int.Parse(orgs)));
            }

            //Null文字を置換
            qr = qr.Replace("\0", "");
            string[] elms = qr.Trim().Split(' ');

            if (elms.Length >= 2)
            {
                return elms[1];
            }
            else
            {
                return qr.Trim();
            }
        }
        #endregion

        #region GetString

        public string GetString(string hexAddressWithDeviceNm, int wordLength)
        {
            return GetString(hexAddressWithDeviceNm, wordLength, false);
        }
        public string GetString(string hexAddressWithDeviceNm, int wordLength, bool isBigEndian)
        {
            string retv = string.Empty;

            string[] org = this.GetBitArray(hexAddressWithDeviceNm, wordLength);

            foreach (string s in org)
            {
                var bit = BitConverter.GetBytes(int.Parse(s));
                if (isBigEndian)
                {
                    bit = bit.Reverse().ToArray();
                }
                retv += System.Text.Encoding.UTF8.GetString(bit);
            }

            //Null文字を置換
            retv = retv.Replace("\0", "");
            return retv;
        }

        #endregion

        #region GetWordAsDecimalData

        public int GetWordAsDecimalData(string hexAddressWithDeviceNM)
        {
            return GetWordAsDecimalData(hexAddressWithDeviceNM, 1);
        }

        public int GetWordAsDecimalData(string hexAddressWithDeviceNM, int wordLength)
        {
            return GetWordAsDecimalDataNullInclude(hexAddressWithDeviceNM).Value;
        }

        public int? GetWordAsDecimalDataNullInclude(string hexAddressWithDeviceNM)
        {
            int? retv = null;
            try
            {
                retv = int.Parse(GetWord(hexAddressWithDeviceNM));
            }
            catch
            {
                return null;
            }

            return retv;
        }

        #endregion

        #region SetWordAsDecimalData

        public void SetWordAsDecimalData(string hexAddressWithDeviceNM, int data)
        {
            string sendData;

            sendData = data.ToString("X2");

            string command = string.Format("WRS {0}{1} {2} {3}\r", hexAddressWithDeviceNM, ".H", 1, sendData);

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand(byteCmd);
        }

        #endregion

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

            byte[] bs = new byte[2];
            string cmdWrite = null;
            for (int i = 0; i < byteStr.Length; i += 2)
            {
                if (i > 0)
                {
                    cmdWrite += " ";
                }

                Array.Copy(byteStr, i, bs, 0, 2);
                //cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).Reverse().ToArray());
                cmdWrite += string.Join("", bs.Select(b => b.ToString("X").PadLeft(2, '0')).ToArray());
            }

            string command = CommandType.WRS.ToString() + " " + hexAddressWithDeviceNM + ".H " + (byteStr.Length / 2).ToString() + " " + cmdWrite + "\r";

            byte[] byteCmd = System.Text.Encoding.UTF8.GetBytes(command);

            executeCommand(byteCmd);

            return true;
        }

        #endregion

        #region GetWordsAsDateTime

        public DateTime GetWordsAsDateTime(string hexAddressWithDeviceNM)
        {
            DateTime? retv = GetWordsAsDateTimeNullInclude(hexAddressWithDeviceNM).Value;
            if (retv.HasValue == false)
            {
                //異常終了
                throw new ApplicationException("PLCから日付データの読出しに失敗:" + hexAddressWithDeviceNM);
            }

            return retv.Value;
        }

        public DateTime? GetWordsAsDateTimeNullInclude(string hexAddressWithDeviceNM)
        {
            string[] res = this.GetBitArray(hexAddressWithDeviceNM, 6);

            return GetWordsAsDateTime(res);
        }

        #endregion

        #region WatchBit

        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue)
        {
            return WatchBit(hexAddressWithDeviceNM, timeout, exitValue, null);
        }

        public bool WatchBit(string hexAddressWithDeviceNM, int timeout, string exitValue, Action act)
        {
            DateTime start = DateTime.Now;
            while (true)
            {
                try
                {
                    string retv = GetBit(hexAddressWithDeviceNM);

                    if (retv == exitValue)
                    {
                        if (act != null) act();
                        return true;
                    }

                    if (timeout > 0)
                    {
                        if ((DateTime.Now - start).TotalSeconds >= timeout)
                        {
                            if (act != null) act();
                            return false;
                        }
                    }
                    if (act != null) act();

                    Thread.Sleep(100);
                }
                catch (Exception ex)
                {
                    Log.RBLog.Error("WatchBit PLC通信エラー発生　10秒待機後に監視継続:" + ex.ToString());
                    Thread.Sleep(10000); //10秒待機
                }
            }
        }

        #endregion

        public bool Ping(string host, int timeout, int retryTimes)
        {
            // Pingオブジェクトの作成
            System.Net.NetworkInformation.Ping ping = new System.Net.NetworkInformation.Ping();
            // Pingを送信する(）
            System.Net.NetworkInformation.PingReply reply = ping.Send(host, timeout);
            // 結果を取得
            bool ret = false;

            try
            {
                if (reply.Status == System.Net.NetworkInformation.IPStatus.Success)
                {
                    ret = true;
                }
                else
                {
                    if (retryTimes <= 2)
                    {
                        return Ping(host, timeout, ++retryTimes);
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            finally
            {
                ping.Dispose();
            }

            return ret;
        }
    }
}
