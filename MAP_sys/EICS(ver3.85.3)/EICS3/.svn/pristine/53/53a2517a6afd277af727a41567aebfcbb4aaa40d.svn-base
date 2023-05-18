using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EICS
{
    /// <summary>
    /// 受信メッセージ
    /// </summary>
    public class ReceiveMessageInfo
    {
        /// <summary>メッセージ長(ヘッダ + データ)</summary>
        public byte[] LengthVAL { get; set; }

        /// <summary>メッセージ長インデックス</summary>
        public int LengthIndexVAL { get; set; }

        /// <summary>ヘッダ3</summary>
        public string HeaderByte3 { get; set; }

        /// <summary>Pタイプ(プレゼンテーションタイプ)</summary>
        public string Ptype { get; set; }

        /// <summary>Sタイプ(セッションタイプ)</summary>
        public string Stype { get; set; }

        /// <summary>ヘッダ内容</summary>
        public byte[] HeaderVAL { get; set; }

        /// <summary>データ内容</summary>
        public byte[] DataVAL { get; set; }

        /// <summary>受信SF</summary>
        public string ReceiveSF { get; set; }

        /// <summary>送信SF</summary>
        public string SendSF { get; set; }

        /// <summary>判定する必要がある受信メッセージ</summary>
        public bool MessageJudgeFG { get; set; }

        /// <summary>リンクテストをする必要のある受信メッセージ</summary>
        public bool LinkTestRequestFG { get; set; }

        public string AllMessage { get; set; }

        public string SendAllMessage { get; set; }

        /// <summary>
        /// 受信情報を取得
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <returns>受信情報</returns>
        public static ReceiveMessageInfo GetReceiveMessage(byte[] message) 
        {
            ReceiveMessageInfo rMessageInfo = new ReceiveMessageInfo();

            rMessageInfo.ReceiveSF = GetStreamFunctionCD(message);
            rMessageInfo.HeaderByte3 = message[7].ToString("X");
            rMessageInfo.Ptype = message[8].ToString("X");
            rMessageInfo.Stype = message[9].ToString("X");
            rMessageInfo.LengthVAL = GetLength(message);
            rMessageInfo.LengthIndexVAL = GetIndexLength(rMessageInfo.LengthVAL);
            rMessageInfo.HeaderVAL = GetHeader(message);
            rMessageInfo.DataVAL
                = GetData(message,
                rMessageInfo.LengthVAL.Length + rMessageInfo.HeaderVAL.Length,
                rMessageInfo.LengthIndexVAL - rMessageInfo.HeaderVAL.Length);

            rMessageInfo.AllMessage = GetAllMessage(message);
            return rMessageInfo;
        }

        /// <summary>
        /// SFCD取得
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <returns>SFCD</returns>
        public static string GetStreamFunctionCD(byte[] message)
        {
            string sfCD = string.Empty;
            switch(message[9].ToString("X"))
            {
                case "2":
                    sfCD = "Select";
                    break;
                case "6":
                    sfCD = "LinkTest";
                    break;
                case "9":
                    sfCD = "Separate";
                    break;
                default:
                    string stream = message[6].ToString("X");
                    sfCD = string.Format("S{0}F{1}", stream.Substring(stream.Length - 1, 1), message[7]);
                    break;
            }
            return sfCD;
        }

        /// <summary>
        /// メッセージ長取得(ヘッダ + データ)
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <returns>メッセージ長</returns>
        private static byte[] GetLength(byte[] message)
        {
            return message.Take(4).ToArray();
        }

        /// <summary>
        /// メッセージ長インデックス取得
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public static int GetIndexLength(byte[] message) 
        {
            //return Convert.ToInt32(Common.Join("", message.ToArray(), 16), 16); 

			string lengthStr = string.Empty;

			foreach (byte byteData in message)
			{
				lengthStr += byteData.ToString("X2");
			}

			int length = Convert.ToInt32(lengthStr, 16);

			return length;
        }

        /// <summary>
        /// ヘッダ内容取得
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <returns>ヘッダ</returns>
        private static byte[] GetHeader(byte[] message)
        {
            return message.Skip(4).Take(10).ToArray();
        }

        /// <summary>
        /// データ内容取得
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <param name="startIndex">開始位置</param>
        /// <param name="dataCount">データ数</param>
        /// <returns>データ内容</returns>
        private static byte[] GetData(byte[] message, int startIndex, int dataCount)
        {
            return message.Skip(startIndex).Take(dataCount).ToArray();
        }

        public static string GetAllMessage(byte[] message) 
        {
            string allMessage = string.Empty;
            foreach (byte m in message) 
            {
                allMessage += Convert.ToString(m,16) + " ";
            }
            return allMessage;
        }
    }

}
