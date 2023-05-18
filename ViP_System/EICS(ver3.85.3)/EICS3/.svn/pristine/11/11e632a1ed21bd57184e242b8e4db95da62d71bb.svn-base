using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EICS
{
    /// <summary>
    /// レポートメッセージ
    /// </summary>
    public class ReportMessageInfo
    {
		const string LIST_TAG = "<List>";
		public const string HSMS_LOG_FILE_EXT = "hsms";

        /// <summary>パラメータ値</summary>
        public object ParameterVAL { get; set; }

        /// <summary>パラメータバイト長</summary>
        public int ParameterLength { get; set; }

        /// <summary>子パラメータリスト</summary>
        public List<ReportMessageInfo> MultiValueList { get; set; }

        /// <summary>
        /// CEID取得
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <returns>CEID</returns>
        public static int GetCeID(byte[] message)
        {
            string ceID = string.Format("{0}{1}", message[8].ToString("X"), message[9].ToString("X"));
            return Convert.ToInt32(ceID, 16);
        }

		public static string GetMagazineNO(byte[] message)
		{
			byte[] tmpMessage;
			int magazineStrLen;

			try
			{
				tmpMessage = message.Skip(18).ToArray();

				magazineStrLen = tmpMessage[1];

				tmpMessage = tmpMessage.Skip(2).ToArray();

				string magazineStr = Encoding.GetEncoding("Shift_JIS").GetString(tmpMessage.Take(magazineStrLen).ToArray());

				if (magazineStr.StartsWith(Constant.DISCRIMINATION_MAGAZINE))
				{
					return magazineStr.Replace(Constant.DISCRIMINATION_MAGAZINE, "").Trim();
				}
				else if (magazineStr.StartsWith(Constant.DISCRIMINATION_LOT))
				{
					return magazineStr.Replace(Constant.DISCRIMINATION_LOT, "").Trim();
				}
				else
				{
					string errMsg = string.Format(Constant.MessageInfo.Message_113, Encoding.GetEncoding("Shift_JIS").GetString(message), magazineStrLen, magazineStr);
					throw new ApplicationException(errMsg);
				}
			}
			catch (Exception err)
			{
				return "unknown";
			}
		}

        /// <summary>
        /// パラメータ取得
        /// </summary>
        /// <param name="message">受信メッセージ</param>
        /// <returns>パラメータ</returns>
        public static byte[] GetParameterData(byte[] message, bool isIncludeMagazineNO) 
        {
			message = message.Skip(18).ToArray();

            if (isIncludeMagazineNO == true)
			{
				int magazineStrLen = message[1];
				message = message.Skip(magazineStrLen + 2).ToArray();
			}

            return message;

        }

		public static List<ReportMessageInfo> GetReportData(byte[] parameters, int listCount, string logFilePath)
		{
			return GetReportData(parameters, listCount, 0, logFilePath);
		}

		/// <summary>
		/// レポートデータを取得（ログ出力版）
		/// </summary>
		/// <param name="parameters">パラメータ</param>
		/// <returns>レポートデータ</returns>
		public static List<ReportMessageInfo> GetReportData(byte[] parameters, int listCount, int listDepth, string logFilePath)
		{
			try
			{
				List<ReportMessageInfo> reportList = new List<ReportMessageInfo>();
				int paramCount;
				int dataStartIndex = 0;// 2;

				//パラメータ数を取得
				if (listCount == 0)//初回呼び出し時はparamCountは
				{
					paramCount = parameters[1];
					dataStartIndex = 2;
				}
				else
					paramCount = listCount;

				//System.Diagnostics.Debug.WriteLine("リスト数(paramCount)：" + paramCount);

				//パラメータ分繰り返し
				for (int paramIndex = 0; paramIndex < paramCount; paramIndex++)
				{
					ReportMessageInfo reportInfo = new ReportMessageInfo();

					//パラメータ値長を取得
					int byteCount = parameters[dataStartIndex + 1];

					//パラメータ長を取得
					reportInfo.ParameterLength = byteCount + 2;

					int itemStartIndex = dataStartIndex + 2;
					byte[] item = parameters.Skip(itemStartIndex).Take(byteCount).ToArray();

					string dataType = parameters[dataStartIndex].ToString("X");

					switch (dataType)
					{
						//ASCII文字
						case "41":
							reportInfo.ParameterVAL = Encoding.ASCII.GetString(item);
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(ASCII)：{0}", reportInfo.ParameterVAL), "SECS通信ログ");

							break;

						//マルチバイト文字
						case "49":
							string charSet = string.Format("{0}{1}", parameters.Skip(dataStartIndex + 2).Take(1).ToArray()[0].ToString("X2"),
								parameters.Skip(dataStartIndex + 3).Take(1).ToArray()[0].ToString("X2"));

							if (charSet == "0008")//Shift_JIS
							{
								item = item.Skip(2).Take(byteCount - 2).ToArray();

								reportInfo.ParameterVAL = Encoding.GetEncoding("Shift_JIS").GetString(item);

								log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(Shift_JIS)：{0}", reportInfo.ParameterVAL), "SECS通信ログ");
							}

							break;

						//符号無し整数(2ﾊﾞｲﾄ)
						case "A9":
							List<string> paramList = new List<string>();
							for (int i = 0; i < item.Length; i += 2)
							{
								//※バイト配列順序を反転させる
								paramList.Add(BitConverter.ToUInt16(item.Skip(i).Take(2).Reverse().ToArray(), 0).ToString());
							}
							reportInfo.ParameterVAL = string.Join(",", paramList.ToArray());

							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(UINT 2byte)：{0}", reportInfo.ParameterVAL), "SECS通信ログ");

							break;

						//符号無し整数(1ﾊﾞｲﾄ)
						case "A5":
							paramList = new List<string>();
							for (int i = 0; i < item.Length; i++)
							{
								paramList.Add(item[i].ToString());
							}
							reportInfo.ParameterVAL = string.Join(",", paramList.ToArray());

							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(UINT 1byte)：{0}", reportInfo.ParameterVAL), "SECS通信ログ");

							break;

						//符号無し整数(4ﾊﾞｲﾄ)
						case "91":
							paramList = new List<string>();
							for (int i = 0; i < item.Length; i += 4)
							{
								//※Single値に変換する際はバイト配列順序を反転させる
								paramList.Add(BitConverter.ToSingle(item.Skip(i).Take(4).Reverse().ToArray(), 0).ToString());
							}
							reportInfo.ParameterVAL = string.Join(",", paramList.ToArray());

							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(UINT 4byte)：{0}", reportInfo.ParameterVAL), "SECS通信ログ");

							break;

						//符号あり整数(4ﾊﾞｲﾄ)
						case "71":
							paramList = new List<string>();
							for (int i = 0; i < item.Length; i += 4)
							{
								//※Single値に変換する際はバイト配列順序を反転させる
								paramList.Add(BitConverter.ToInt32(item.Skip(i).Take(4).Reverse().ToArray(), 0).ToString());
							}
							reportInfo.ParameterVAL = string.Join(",", paramList.ToArray());

							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(INT 4byte)：{0}", reportInfo.ParameterVAL), "SECS通信ログ");

							break;

						//リスト
						default:
							string strListSize = parameters[dataStartIndex + 1].ToString("D");
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("パラメータログ(リスト)：リスト数{0}", strListSize), "SECS通信ログ");
							//hsmsLog.Add(GetContainIndentLogMsg(string.Format("{0} {1}", LIST_TAG, strListSize), listDepth));

							using (StreamWriter sw = new StreamWriter(logFilePath, true, SettingInfo.GetEncode()))
							{
								sw.WriteLine(GetContainIndentLogMsg(string.Format("{0} {1}", LIST_TAG, strListSize), listDepth));
							}
							//子リストを取得
							reportInfo.MultiValueList = GetReportData(parameters.Skip(dataStartIndex + 2).ToArray(), parameters[dataStartIndex + 1], listDepth + 1, logFilePath);

							//子リスト長の分進める
							//dataStartIndex = itemStartIndex + reportInfo.MultiValueList.Sum(m => m.ParameterLength); //改修前
							dataStartIndex += GetAllChildListLen(reportInfo.MultiValueList);
							reportList.Add(reportInfo);

							continue;
					}

					if (reportInfo.ParameterVAL != null)
					{
						using (StreamWriter sw = new StreamWriter(logFilePath, true, SettingInfo.GetEncode()))
						{
							sw.WriteLine(GetContainIndentLogMsg((string)reportInfo.ParameterVAL, listDepth));
						}
					}

					//パラメータ長の分進める
					dataStartIndex = itemStartIndex + byteCount;
					reportList.Add(reportInfo);
				}

				return reportList;
			}
			catch (Exception err)
			{
				throw new Exception(err.Message);
			}
		}

		private static string GetContainIndentLogMsg(string rawMsg, int depth)
		{
			for (int i = 0; i < depth; i++)
			{
				rawMsg = "\t" + rawMsg;
			}
			return rawMsg;
		}

		private static int GetAllChildListLen(List<ReportMessageInfo> targetList)
		{
			int allLength = 0;
			int listCount = 0;

			foreach (ReportMessageInfo targetReportInfo in targetList)
			{
				listCount++;
				if (targetReportInfo.MultiValueList != null)
				{
					allLength += GetAllChildListLen(targetReportInfo.MultiValueList);
				}
				else
				{
					allLength += targetReportInfo.ParameterLength;
				}
			}
			return allLength + 2;//リストの要素数を示した部分のバイト数（2バイト）を足して戻り値を返す。
		}

		public void OutputLogData(string filePath, List<string> logDataList)
		{
            using (StreamWriter sw = new StreamWriter(filePath, false, Encoding.GetEncoding("Shift_JIS")))
            {
                foreach (string logData in logDataList)
                {
                    sw.WriteLine(logData);
                }
            }
		}

    }
}
