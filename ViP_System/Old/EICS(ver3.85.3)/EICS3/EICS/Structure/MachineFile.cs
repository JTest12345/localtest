using EICS.Database;
using EICS.Machine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Structure
{
	public class MachineFile
	{
		private const int LOT_ON_FILENAME_INDEX = 1;
		private const int TYPE_ON_FILENAME_INDEX = 2;
		private const int PROC_ON_FILENAME_INDEX = 3;
		private const int MAGAZINE_ON_FILENAME_INDEX = 4;

		private const int HEADER_COL_INDEX = 0;
		private const int HEADER_ROW_INDEX = 0;
		public const char TEXT_SPLITTER = ',';

		/// <summary>ARMSによりファイルに情報付与された場合にロットNoが_区切りで付与される位置（インデックス情報）</summary>
		public const int FILE_UNKNOWN_LOT_NAME_INDEX = 2;
		/// <summary>ARMSによりロットNoが特定できずにファイル名にロットNo不明の意味でロットNo付与位置に付与される文字列</summary>
		public const string FILE_UNKNOWN_LOT_NAME = "UNKNOWN";

		public const string NULL_Str = "NULL";

		private static int GetDataStartRowIndex()
		{
			return HEADER_ROW_INDEX + 1;
		}

		private static int GetDataStartColIndex()
		{
			return HEADER_COL_INDEX + 1;
		}

		public static char GetTextSplitter(char? txtSplitter)
		{
			if (txtSplitter.HasValue)
			{
				return txtSplitter.Value;
			}
			else
			{
				return TEXT_SPLITTER;
			}
		}

		public static bool IsThereLotNoInFileName(string filePath)
		{
			FileInfo fi = new FileInfo(filePath);

			if (fi.Exists)
			{
				// SDファイル名定義(log000_SD***_Lot_Type_Proc.csv)を想定して要素数で付与済か判断する 
				string[] nameChar = Path.GetFileNameWithoutExtension(fi.Name).Split('_');
				if (nameChar.Count() < 5)
				{
					return false;
				}
				else { return true; }
			}
			else
			{
				return false;
			}
		}

		public static string GetLotFromFileName(string filePath)
		{
			return GetLotFromFileName(filePath, null);
		}

		public static string GetLotFromFileName(string filePath, int? indexOffset)
		{
			int offset = 0;

			if (indexOffset.HasValue)
			{
				offset = indexOffset.Value;
			}

			if (IsThereLotNoInFileName(filePath) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名にﾛｯﾄ情報がありません。ﾌｧｲﾙﾊﾟｽ:{0}", filePath));
			}

			string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');
			return nameChar[LOT_ON_FILENAME_INDEX + offset].Trim();
		}

		public static bool IsUnknownFile(FileInfo mmFile)
		{
			string[] nameChar = Path.GetFileNameWithoutExtension(mmFile.FullName).Split('_');

			if (nameChar[FILE_UNKNOWN_LOT_NAME_INDEX].ToUpper() == FILE_UNKNOWN_LOT_NAME)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public static string GetTypeFromFileName(string filePath)
		{
			return GetTypeFromFileName(filePath, null);
		}

		public static string GetTypeFromFileName(string filePath, int? indexOffset)
		{
			int offset = 0;

			if (indexOffset.HasValue)
			{
				offset = indexOffset.Value;
			}

			if (IsThereLotNoInFileName(filePath) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名にﾛｯﾄ情報がありません。ﾌｧｲﾙﾊﾟｽ:{0}", filePath));
			}

			string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');

			return nameChar[TYPE_ON_FILENAME_INDEX + offset].Trim();
		}

		public static int GetProcFromFileName(string filePath)
		{
			return GetProcFromFileName(filePath, null);
		}

		public static int GetProcFromFileName(string filePath, int? indexOffset)
		{
			int offset = 0;

			if (indexOffset.HasValue)
			{
				offset = indexOffset.Value;
			}

			int proc;
			if (IsThereLotNoInFileName(filePath) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名にﾛｯﾄ情報がありません。ﾌｧｲﾙﾊﾟｽ:{0}", filePath));
			}

			string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');

			if (int.TryParse(nameChar[PROC_ON_FILENAME_INDEX].Trim(), out proc) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名から取得したprocNo文字列が数値変換出来ません。ﾌｧｲﾙ:{0} 変換対象:{1} ｲﾝﾃﾞｸｽ:{2}"
					, filePath, nameChar[PROC_ON_FILENAME_INDEX + offset], PROC_ON_FILENAME_INDEX));
			}

			return proc;
		}

		public static string GetMagazineNoFromFileName(string filePath)
		{
			return GetMagazineNoFromFileName(filePath, null);
		}

		public static string GetMagazineNoFromFileName(string filePath, int? indexOffset)
		{
			int offset = 0;

			if (indexOffset.HasValue)
			{
				offset = indexOffset.Value;
			}

			if (IsThereLotNoInFileName(filePath) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名にﾛｯﾄ情報がありません。ﾌｧｲﾙﾊﾟｽ:{0}", filePath));
			}

			string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');
			return nameChar[MAGAZINE_ON_FILENAME_INDEX + offset].Trim();
		}

		public static List<string> GetHeaderList(string[] txtArray, bool headerScanInRowFg)
		{
			return GetHeaderList(txtArray, headerScanInRowFg, TEXT_SPLITTER);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="fileTxtArray"></param>
		/// <param name="headerScanInRowFg"></param>
		/// <param name="splitter"></param>
		/// <returns></returns>
		public static List<string> GetHeaderList(string[] txtArray, bool headerScanInRowFg, char splitter)
		{
			try
			{
				List<string> headerNmList = new List<string>();

				if (headerScanInRowFg)  //行方向にパラメタ名を走査
				{
					string[] headerNmElm = txtArray[HEADER_ROW_INDEX].Split(splitter);

					headerNmList = headerNmElm.ToList();

					for (int i = 0; i < headerNmList.Count; i++)
					{
						headerNmList[i] = headerNmList[i].ToUpper().Trim();
					}
				}
				else //列方向にパラメタ名を走査
				{
					foreach (string lineTxt in txtArray)
					{
						string lineTxtTmp = lineTxt.ToUpper().Trim();

						string[] lineTxtElm = lineTxtTmp.Split(splitter);

						headerNmList.Add(lineTxtElm[HEADER_COL_INDEX].Trim());
					}
				}

				return headerNmList;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// テキストファイル内のヘッダにおける指定したヘッダ名の0始まりのインデックスを返す
		/// 存在しない場合は-1を返す
		/// </summary>
		/// <param name="txtArray"></param>
		/// <param name="headerNm"></param>
		/// <param name="headerScanInRowFg"></param>
		/// <returns></returns>
		public static int GetHeaderIndex(string[] txtArray, string headerNm, bool headerScanInRowFg)
		{
			try
			{
				List<string> headerList = GetHeaderList(txtArray, headerScanInRowFg);

				int index = headerList.IndexOf(headerNm);

				return index;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static string GetData(string[] txtArray, string targetHeaderNm, bool lastGetFg, bool headerScanInRowFg)
		{
			return GetData(txtArray, targetHeaderNm, lastGetFg, headerScanInRowFg, string.Empty, null);
		}

		/// <summary>
		/// ファイル内の指定したヘッダのインデックスの値を取得する
		/// </summary>
		/// <param name="txtArray">ファイル内容</param>
		/// <param name="targetHeaderIndex">取得対象値の該当するヘッダ</param>
		/// <param name="lastGetFg">ファイルの末尾から取得するかどうか</param>
		/// <param name="headerScanInRowFg">ヘッダが行か列のどちらに存在するか</param>
		/// <returns></returns>
		public static string GetData(string[] txtArray, string targetHeaderNm, bool lastGetFg, bool headerScanInRowFg, string targetData, int? targetDataColIndex)
		{
			try
			{
				if (string.IsNullOrEmpty(targetHeaderNm))
				{
					return string.Empty;
				}

				int targetHeaderIndex = GetHeaderIndex(txtArray, targetHeaderNm, headerScanInRowFg);

				string[] targetDataArray = GetData(txtArray, targetHeaderIndex, headerScanInRowFg, targetData, targetDataColIndex);

				if (targetDataArray.Length == 0)
				{
					return string.Empty;
				}

				if (lastGetFg)
				{
					return targetDataArray.Last();
				}
				else
				{
					return targetDataArray.First();
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static string[] GetData(string[] txtArray, int headerIndex, bool headerScanInRowFg, string targetData, int? targetDataColIndex)
		{
			return GetData(txtArray, TEXT_SPLITTER, headerIndex, headerScanInRowFg, targetData, targetDataColIndex);
		}

		/// <summary>
		/// ファイル内のテキストデータとヘッダのインデックス、ヘッダが行方向に並んでいるかを与えると
		/// 対象ヘッダのデータ部を返す
		/// </summary>
		/// <param name="txtArray"></param>
		/// <param name="headerIndex"></param>
		/// <param name="headerRowScanFg"></param>
		/// <returns></returns>
		public static string[] GetData(string[] txtArray, char splitter, int headerIndex, bool headerScanInRowFg, string targetData, int? targetDataColIndex)
		{
			try
			{
				string[] dataArray;
				List<string> dataList = new List<string>();

				if (headerScanInRowFg)  //headerIndex番目の列をデータとして取得
				{
					txtArray = txtArray.Skip(GetDataStartRowIndex()).ToArray();

					foreach (string lineTxt in txtArray)
					{
						if (string.IsNullOrEmpty(lineTxt))
						{
							continue;
						}
						string[] lineTxtElm = lineTxt.Split(splitter);

						if (string.IsNullOrEmpty(targetData) == false && targetDataColIndex.HasValue)
						{
							if (lineTxtElm[targetDataColIndex.Value] != targetData)
							{
								continue;
							}
						}

						if (lineTxtElm.Length > headerIndex)
						{
							if (lineTxtElm[headerIndex].Trim() == PLC.OUTPUT_NULL_Str || lineTxtElm[headerIndex].Trim().ToUpper() == NULL_Str)
							{
								continue;
							}

							dataList.Add(lineTxtElm[headerIndex].Trim());
						}
						else
						{
							continue;
							throw new ApplicationException(
								string.Format("ﾌｧｲﾙから取得されたﾃｷｽﾄの要素数が不足しています。 ﾃｷｽﾄ内容:{0}", lineTxt));
						}
					}
					dataArray = dataList.ToArray();
				}
				else //headerIndex番目の行をデータとして取得
				{
					string[] dataElmWithHeader = txtArray[headerIndex].Split(splitter);
					dataArray = dataElmWithHeader.Skip(GetDataStartColIndex()).Select(val => val.Trim()).ToArray();
				}

				return dataArray;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static bool TryGetTxt(string filePath, string lineSplitter, out string[] result, string encodeStr)
		{
			return TryGetTxt(filePath, lineSplitter, out result, encodeStr, 0);
		}

		public static bool TryGetTxt(string filePath, string lineSplitter, out string[] result, string encodeStr, int retryCt)
		{
			try
			{
				if (File.Exists(filePath))
				{
					try
					{
						if (string.IsNullOrEmpty(encodeStr))
						{
							encodeStr = "Shift_JIS";
						}

						using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding(encodeStr)))
						{
							result = sr.ReadToEnd().Split(new string[] { lineSplitter }, StringSplitOptions.None);
							sr.Close();
						}
					}
					catch (Exception err)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
							string.Format("ファイル読み込み時にエラー発生の為、装置処理を初めから再スタート 対象ファイル：{0}\r\n例外Msg：{1}\r\n例外StackTrace{2}", filePath, err.Message, err.StackTrace));
						result = null;
						return false;
					}
				}
				else
				{
					retryCt++;

					if (retryCt > 5)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
								string.Format("ﾌｧｲﾙの存在が確認出来ませんでした。 ﾌｧｲﾙﾊﾟｽ：{0}", filePath));

						result = null;
						return false;
					}
					else
					{
						Thread.Sleep(300);
						return TryGetTxt(filePath, lineSplitter, out result, encodeStr, retryCt);
					}
				}

				return true;
			}
			catch (Exception err)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
				string.Format("ﾌｧｲﾙ読み込み時に例外発生 ﾌｧｲﾙﾊﾟｽ：{0}\r\n例外Msg：{1}\r\n例外StackTrace{2}", filePath, err.Message, err.StackTrace));

				throw;
			}
		}

		public static bool TryGetElementFromFileNm(string fileNm, char splitter, int targetIndex, int? needElementCt, out string typeCD)
		{
			try
			{
				string[] fileNmElement = fileNm.Split(splitter);

				if (fileNmElement.Count() >= targetIndex)
				{
					if (needElementCt.HasValue)
					{
						if (fileNmElement.Count() == needElementCt.Value)
						{
							typeCD = fileNmElement[targetIndex];
							return true;
						}
						else
						{
							typeCD = null;
							return false;
						}
					}
					else
					{
						typeCD = fileNmElement[targetIndex];
						return true;
					}
				}
				else
				{
					typeCD = null;
					return false;
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// ﾌｧｲﾙ内ﾃｷｽﾄ(各要素はｶﾝﾏ区切り)で行ごとのﾃﾞｰﾀをstring配列に格納したﾃﾞｰﾀから対象列のﾃﾞｰﾀのﾘｽﾄを返す関数
		/// 指定列の指定値を除外する機能あり
		/// </summary>
		/// <param name="strLineArray">ﾌｧｲﾙ内ﾃｷｽﾄ(各要素はｶﾝﾏ区切り)で行ごとに配列に格納したﾃﾞｰﾀ</param>
		/// <param name="dataStartRowIndex">ﾃﾞｰﾀが入っている行ｲﾝﾃﾞｸｽ（ﾍｯﾀﾞなどを飛ばして参照する為に必要）</param>
		/// <param name="keikouFgColIndex">傾向管理列のｲﾝﾃﾞｸｽ（ﾃﾞｰﾀ除外処理に必要）</param>
		/// <param name="keikouFgStrForSkip">傾向管理列において除外判断する為の除外値</param>
		/// <param name="wbMappingFg">WBﾏｯﾋﾟﾝｸﾞの有効無効を表すﾌﾗｸﾞ</param>
		/// <param name="changeFgColIndex">変化点ﾌﾗｸﾞ列のｲﾝﾃﾞｸｽ（ﾃﾞｰﾀ除外処理に必要）</param>
		/// <param name="changeFgStrForSkip">変化点ﾌﾗｸﾞ列において除外判断する為の除外値</param>
		/// <param name="targetColIndex">ﾃﾞｰﾀ取得対象の列ｲﾝﾃﾞｸｽ</param>
		/// <param name="targetColStrForSkip">ﾃﾞｰﾀ取得対象列において除外対象となる値</param>
		/// <param name="dayColIndex">日付列ｲﾝﾃﾞｸｽ</param>
		/// <param name="timeColIndex">時間列ｲﾝﾃﾞｸｽ</param>
		/// <param name="dateTimeStr">日付列、時間列から取得した日時文字列を受け取る為の引数</param>
		/// <returns></returns>
		public static List<string> GetDataListFromStrArray(string[] strLineArray, int dataStartRowIndex, int keikouFgColIndex, string keikouFgStrForSkip,
			bool wbMappingFg, int changeFgColIndex, string changeFgStrForSkip, int targetColIndex, string targetColStrForSkip, int dayColIndex, int timeColIndex, out string dateTimeStr)
		{
			dateTimeStr = string.Empty;

			FileValueInfo fileValueInfo = new FileValueInfo();
			List<double> valueList = new List<double>();
			List<string> dataList = new List<string>();

			for (int i = dataStartRowIndex; i < strLineArray.Length; i++)
			{
				if (strLineArray[i] == "")
				{
					//空白行の場合、次へ
					continue;
				}

				string[] fileValue = strLineArray[i].Split(',');
				if (fileValue[keikouFgColIndex].Trim().ToUpper() == keikouFgStrForSkip.Trim().ToUpper())
				{
					//傾向管理しないデータの場合、次へ
					continue;
				}
				if (wbMappingFg)
				{
					if (changeFgColIndex >= 0)
					{
						if (fileValue[changeFgColIndex].Trim().ToUpper() == changeFgStrForSkip.Trim().ToUpper())
						{
							//変化点フラグOFFの場合、次へ
							continue;
						}
					}
				}
				if (fileValue[targetColIndex].Trim().ToUpper() == targetColStrForSkip.Trim().ToUpper())
				{
					continue;
				}

				double measureVAL = 0;
				string txtVal = fileValue[targetColIndex].Trim();

				if (!double.TryParse(txtVal, out measureVAL))
				{
					//測定値が数値以外の場合、次へ
					continue;
				}

				dateTimeStr = Convert.ToString(Convert.ToDateTime(fileValue[dayColIndex] + " " + fileValue[timeColIndex]));

				dataList.Add(txtVal);
			}

			return dataList;
		}

		public static string GetCalculatedResult(Plm plmInfo, string[] sampledData, int lineCD)
		{
			return GetCalculatedResult(plmInfo, sampledData, null, false, lineCD, null, null, null);
		}

		//public static string GetCalculatedResult(Plm plmInfo, string[] sampledData, bool getOnly1Data)
		//{
		//	return GetCalculatedResult(plmInfo, sampledData, null, getOnly1Data, null, null, null, null);
		//}

		/// <summary>
		/// σから母集団のσを計算する時に使用可能
		/// </summary>
		/// <param name="plmInfo"></param>
		/// <param name="sampledData"></param>
		/// <param name="sampledData2"></param>
		/// <returns></returns>
		public static string GetCalculatedResult(Plm plmInfo, string[] sampledData, string[] sampledData2, int lineCD)
		{
			return GetCalculatedResult(plmInfo, sampledData, sampledData2, false, lineCD, null, null, null);
		}

		//public static string GetCalculatedResult(Plm plmInfo, string[] sampledData, string[] sampledData2, bool getOnly1Data)
		//{
		//	GetCalculatedResult
		//}

		public static string GetCalculatedResult(Plm plmInfo, string[] sampledData, string[] sampledData2, bool getOnly1Data, int? lineCD, string equipmentNO, string measureDT, int? refQcParamNO)
		{
			try
			{
				double calclatedData;

				if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.AVE.ToString())
				{
					calclatedData = calcAvg(sampledData);
					//return calclatedData.ToString();
				}
				else if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.SIGMA.ToString())
				{
					double fileAveVAL = calcAvg(sampledData);
					calclatedData = calcSigma(sampledData, fileAveVAL);
					//return calclatedData.ToString();
				}
				else if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.SIGMA2USIGMA.ToString())
				{
					calclatedData = calcUSigmaFromSigma(sampledData, sampledData2);

					//return calclatedData.ToString();
				}
				else if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.MAX.ToString())
				{
					calclatedData = calcMax(sampledData);
					//return calclatedData.ToString();
				}
				else if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.MIN.ToString())
				{
					calclatedData = calcMin(sampledData);
					//return calclatedData.ToString();
				}
				else if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.DELTA.ToString())
				{
					if (lineCD.HasValue == false || refQcParamNO.HasValue == false || string.IsNullOrEmpty(measureDT) || string.IsNullOrEmpty(equipmentNO))
					{
						throw new ApplicationException(string.Format(
							"ﾗｲﾝCD、設備CD、測定日時、前回値参照用QcParamNOの何れかの指定が欠けている為、前回値差分集計の計算出来ません。ﾗｲﾝCD:{0} 設備CD:{1} 測定日時:{2} refQcParamNO:{3}",
							lineCD.Value, equipmentNO, measureDT, refQcParamNO.Value));
					}

					double currentData = calcAvg(sampledData);
					calclatedData = calcDeltaBetweenPreviousAndCurrent(lineCD.Value, equipmentNO, measureDT, refQcParamNO.Value, currentData);
					//return calclatedData.ToString();
				}
				else if (plmInfo.TotalKB.ToUpper() == Constant.CalcType.SP.ToString())
				{
					if (lineCD.HasValue == false || refQcParamNO.HasValue == false || string.IsNullOrEmpty(measureDT) || string.IsNullOrEmpty(equipmentNO))
					{
						throw new ApplicationException(string.Format(
							"ﾗｲﾝCD、設備CD、測定日時、前回値参照用QcParamNOの何れかの指定が欠けている為、前回値差分集計の計算出来ません。ﾗｲﾝCD:{0} 設備CD:{1} 測定日時:{2} refQcParamNO:{3}",
							lineCD.Value, equipmentNO, measureDT, refQcParamNO.Value));
					}

					double currentData = calcAvg(sampledData);
					calclatedData = calcDeltaBetweenPreviousAndCurrent(lineCD.Value, equipmentNO, measureDT, refQcParamNO.Value, currentData);
					//return calclatedData.ToString();
				}
				else
				{
					if (sampledData.Count() == 1 && string.IsNullOrEmpty(plmInfo.ParameterVAL))
					{
						if (sampledData[0] == Structure.MachineFile.NULL_Str)
						{
							return sampledData[0];
						}

						if (double.TryParse(sampledData[0], out calclatedData) == false)
						{
							throw new ApplicationException(
								string.Format("パラメタが数値変換出来ません。ﾊﾟﾗﾒﾀ番号:{0}　ﾊﾟﾗﾒﾀ名:{1}　変換対象:{2}"
								, plmInfo.QcParamNO, plmInfo.ParameterNM, sampledData[0]));
						}
						//return calclatedData.ToString();
					}
					else if (sampledData.Count() == 1 && string.IsNullOrEmpty(plmInfo.ParameterVAL) == false)
					{
						//マスタと一致しているかの対象パラメタ値を取得
						if (double.TryParse(sampledData[0], out calclatedData))
						{
						}
						else
						{
							return sampledData[0];
						}
					}
					else if (sampledData.Count() > 1 && string.IsNullOrEmpty(plmInfo.ParameterVAL) == false)
					{
						throw new ApplicationException(
							string.Format("パラメタ一致照合の対象ですが、ファイルから取得したデータが複数存在します。ﾊﾟﾗﾒﾀ番号：{0} ﾊﾟﾗﾒﾀ名：{1}"
							, plmInfo.QcParamNO, plmInfo.ParameterNM));
					}
					else if (getOnly1Data)
					{
						if (sampledData.Count() > 1 && string.IsNullOrEmpty(plmInfo.ParameterVAL))
						{
							throw new ApplicationException(string.Format("集計処理対象外のﾏｽﾀ設定ですが、ﾌｧｲﾙから複数の値が抽出されました。ﾏｽﾀ担当者へ連絡して下さい。ﾊﾟﾗﾒﾀ番号：{0} ﾊﾟﾗﾒﾀ名：{1}"
								, plmInfo.QcParamNO, plmInfo.ParameterNM));

						}
						else if (sampledData.Count() == 0)
						{
							throw new ApplicationException(
								string.Format("ﾌｧｲﾙからﾃﾞｰﾀが抽出されませんでした。ﾊﾟﾗﾒﾀ番号：{0} ﾊﾟﾗﾒﾀ名：{1}"
								, plmInfo.QcParamNO, plmInfo.ParameterNM));
						}
						else
						{
							throw new ApplicationException(
								string.Format("ﾌｧｲﾙ内から1ﾃﾞｰﾀのみが取得出来る想定の処理で複数行のﾃﾞｰﾀが存在しました。ﾊﾟﾗﾒﾀ番号：{0} ﾊﾟﾗﾒﾀ名：{1}"
								, plmInfo.QcParamNO, plmInfo.ParameterNM));
							//return sampledData[0];
						}
					}
					else
					{
						if (double.TryParse(sampledData[0], out calclatedData))
						{
						}
						else
						{
							return sampledData[0];
						}
					}
				}

				double unitChangedData;

				unitChangedData = CalcChangeUnit(lineCD.Value, plmInfo.QcParamNO, calclatedData);

				return unitChangedData.ToString();
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// 算出情報を基にファイル値を計算
		/// </summary>
		/// <returns></returns>
		public static double CalcChangeUnit(int lineCD, int qcParamNO, double targetVAL)
		{
			double rChangeUnitVAL = 0;

			string unitVAL = Convert.ToString(ConnectDB.GetPRMElement("ChangeUnit_VAL", qcParamNO, lineCD));
			if (unitVAL == "" || unitVAL == "0" || unitVAL == "1")
			{
				return targetVAL;
			}

			int calcVAL = Convert.ToInt32(unitVAL.Substring(1, unitVAL.Length - 1));

			if (unitVAL.Substring(0, 1) == "/")
			{
				rChangeUnitVAL = targetVAL / calcVAL;
			}
			else if (unitVAL.Substring(0, 1) == "*")
			{
				rChangeUnitVAL = targetVAL * calcVAL;
			}
			else
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_122, lineCD, qcParamNO, unitVAL));
			}

			return rChangeUnitVAL;
		}


		private static double calcDeltaBetweenPreviousAndCurrent(int lineCD, string equipmentNO, string measureDT, int refQcParamNO, double currentVal)
		{
			try
			{
				double retv;

				//前回取得値(同装置での前ロット取得値)を取得
				decimal backLotVAL = ConnectDB.GetTnLOG_DParam(lineCD, equipmentNO, measureDT, refQcParamNO, 1);

				if (backLotVAL == decimal.MinValue)
				{
					//前回取得値が存在しなかった場合(初期ロット)、異常判定はしないので0を格納
					retv = 0;
				}
				else
				{
					//前回取得値との差異を格納
					retv = Math.Abs(Convert.ToDouble(backLotVAL) - currentVal);
				}
				return retv;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		#region ファイルから取得したデータ集計
		private static double calcSigma(string[] dataArray, double avg)
		{
			try
			{
				double xsum = 0;
				double x2sum = 0;
				double dCnt = dataArray.Length;
				//  念のためのエラーチェック
				if (dCnt == 0) return 0.0;

				foreach (string dataStr in dataArray)
				{
					double data;
					if (double.TryParse(dataStr, out data) == false)
					{
						dCnt--;
						continue;
					}

					xsum += data;
					x2sum += data * data;
				}

				//  Excel STDEVA()の定義
				double sigma = Math.Sqrt((dCnt * x2sum - (xsum * xsum)) / (dCnt * (dCnt - 1)));

				return sigma;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		private static double calcUSigmaFromSigma(string[] strSigmaDataArray, string[] strAveDataArray)
		{
			try
			{
				int sampleNum = strSigmaDataArray.Length;
				if (sampleNum != strAveDataArray.Length)
				{
					throw new ApplicationException(string.Format("σ値標本(標本数:{0})と平均値標本(標本数:{1})の数が一致しません。", sampleNum, strAveDataArray.Length));
				}

				double[] aveDataArray = new double[sampleNum];
				double[] sigmaDataArray = new double[sampleNum];
				double totalAve = 0;

				//計算に使う値が文字列で渡される為、数値変換
				for (int i = 0; i < sampleNum; i++)
				{
					double convertedAveVal;
					double convertedSigmaVal;

					if (double.TryParse(strAveDataArray[i], out convertedAveVal) == false)
					{
						throw new ApplicationException(string.Format("平均値文字列が数値変換出来ません。変換対象:{0} index:{1}", strAveDataArray[i], i));
					}
					aveDataArray[i] = convertedAveVal;
					if (double.TryParse(strSigmaDataArray[i], out convertedSigmaVal) == false)
					{
						throw new ApplicationException(string.Format("σ値文字列が数値変換出来ません。変換対象:{0} index:{1}", strSigmaDataArray[i], i));
					}
					sigmaDataArray[i] = convertedSigmaVal;
				}

				//LINQのSumは情報落ち誤差が無いみたいなので、List化してSum() 2015/9/8 nyoshimoto
				totalAve = aveDataArray.ToList().Sum() / sampleNum;

				//分子部分の計算（分母はsample数）
				List<double> calcTempList = new List<double>();
				for (int i = 0; i < sampleNum; i++)
				{
					double dispersion = Math.Pow(sigmaDataArray[i], 2);
					double offset = Math.Pow(aveDataArray[i] - totalAve, 2);

					calcTempList.Add(dispersion + offset);
				}

				double calcResult = Math.Sqrt(calcTempList.Sum() / sampleNum);

				return calcResult;

			}
			catch (Exception err)
			{
				throw;
			}
		}

		private static double calcAvg(string[] dataArray)
		{
			try
			{
				double retv = 0.0;
				double dCnt = dataArray.Length;
				//  念のためのエラーチェック
				if (dCnt == 0) return 0.0;

				foreach (string dataStr in dataArray)
				{
					double data;

					if (double.TryParse(dataStr, out data) == false)
					{
						dCnt--;
						continue;
					}

					retv += data;
				}

				retv = retv / dCnt;

				return retv;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		private static double calcAvg(List<double> dataList)
		{
			try
			{
				double retv = 0.0;
				double dCnt = dataList.Count;
				//  念のためのエラーチェック
				if (dCnt == 0) return 0.0;

				foreach (double data in dataList)
				{
					retv += data;
				}

				retv = retv / dCnt;

				return retv;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		private static double calcMin(string[] dataArray)
		{
			try
			{
				double wdrtn = double.MaxValue;

				foreach (string dataStr in dataArray)
				{
					double data;
					if (double.TryParse(dataStr, out data) == false)
					{
						continue;
					}

					if (wdrtn > data)
					{
						wdrtn = data;
					}
				}
				return wdrtn;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		private static double calcMin(List<double> dataList)
		{
			try
			{
				double wdrtn = double.MaxValue;

				foreach (double data in dataList)
				{
					if (wdrtn > data)
					{
						wdrtn = data;
					}
				}
				return wdrtn;
			}
			catch (Exception err)
			{
				throw;
			}
		}


		private static double calcMax(string[] dataArray)
		{
			try
			{
				double wdrtn = double.MinValue;

				foreach (string dataStr in dataArray)
				{
					double data;
					if (double.TryParse(dataStr, out data) == false)
					{
						continue;
					}

					if (wdrtn < data)
					{
						wdrtn = data;
					}
				}
				return wdrtn;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		private static double calcMax(List<double> dataList)
		{
			try
			{
				double wdrtn = double.MinValue;

				foreach (double data in dataList)
				{
					if (wdrtn < data)
					{
						wdrtn = data;
					}
				}
				return wdrtn;
			}
			catch (Exception err)
			{
				throw;
			}
		}
		#endregion


		public static FileInfo Search(string targetDirectoryPath, string prefixNm)
		{
			List<string> filePathList = GetPathList(targetDirectoryPath, prefixNm, true);
			if (filePathList.Count == 0) return null;

			//見つかったファイルの内、更新日時が一番新しいファイルを返す
			if (File.Exists(filePathList.Last()))
			{
				try
				{
					using (FileStream fs = File.Open(filePathList.Last(), FileMode.Open, FileAccess.ReadWrite))
					{
					}

					return new FileInfo(filePathList.Last());
				}
				catch (Exception err)
				{
					return null;
				}
			}
			else
			{
				return null;
			}
		}

		/// <summary>
		/// 設備ファイルの取得
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="equipmentInfo">設備情報</param>
		/// <returns>スタートファイルリスト</returns>
		public static List<string> GetPathList(string machineDirPath, string filePrefix)
		{
			try
			{
				List<FileInfo> sortFileList = new List<FileInfo>();
				List<string> rFileList = new List<string>();

				//string[] machineFileList = Directory.GetFiles(machineDirPath, "*" + filePrefix + "*");
				List<string> machineFileList = Common.GetFiles(machineDirPath, filePrefix + ".*");

				foreach (string machineFile in machineFileList)
				{
					FileInfo fileInfo;
					if (File.Exists(machineFile))
					{
						fileInfo = new FileInfo(machineFile);
					}
					else
					{
						continue;
					}

					if (fileInfo.Name.Substring(0, 4) == "____")
					{
						continue;
					}

					//0KBファイルを削除
					if (fileInfo.Length == 0)
					{
						if (File.Exists(machineFile))
						{
							fileInfo.Delete();
						}
						else
						{
							continue;
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_44, fileInfo.FullName));
						continue;
					}

					//ファイルが転送中かの確認
					if (!CheckFileAccess(fileInfo.FullName))
					{
						continue;
					}

					sortFileList.Add(fileInfo);
				}

				//更新日時順にファイルを並び変える
				IEnumerable<FileInfo> sFileList = sortFileList.OrderBy(s => s.LastWriteTime);
				foreach (FileInfo sFile in sFileList)
				{
					rFileList.Add(sFile.FullName);
				}

				return rFileList;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// ファイルが転送中かの確認
		/// </summary>
		/// <param name="targetFilePath"></param>
		/// <returns></returns>
		public static bool CheckFileAccess(string targetFilePath)
		{
			long first = 0;
			long second = -1;

			//装置からﾌｧｲﾙが転送中の場合は、Sleepで待つ。ﾌｧｲﾙｻｲｽﾞが同じになった(=転送終了)場合、ﾙｰﾌﾟを抜ける
			while (first != second)
			{
				FileInfo fi = new FileInfo(targetFilePath);

				first = fi.Length;

				Thread.Sleep(500);
				if (File.Exists(targetFilePath) == false)
				{
					return false;
				}

				FileInfo fi2 = new FileInfo(targetFilePath);
				second = fi2.Length;
			}

			return true;
		}

		/// <summary>
		/// 設備ファイルの取得
		/// </summary>
		/// <param name="path">ファイルパス</param>
		/// <param name="equipmentInfo">設備情報</param>
		/// <returns>スタートファイルリスト</returns>
		public static List<string> GetPathList(string machineDirPath, string filePrefix, bool waitAddLotNoToFileName)
		{
			List<FileInfo> sortFileList = new List<FileInfo>();
			List<string> rFileList = new List<string>();

			List<string> machineFileList = Common.GetFiles(machineDirPath, ".*" + filePrefix + ".*");

			foreach (string machineFile in machineFileList)
			{
				if (waitAddLotNoToFileName)
				{
					SettingInfo commonSetting = SettingInfo.GetSingleton();

					int loopCt = 0;

					bool isThereLotNoInFileName = MachineFile.IsThereLotNoInFileName(machineFile);

					if (isThereLotNoInFileName == false)
					{
						continue;
					}
				}

				FileInfo fileInfo;
				if (File.Exists(machineFile))
				{
					fileInfo = new FileInfo(machineFile);
				}
				else
				{
					continue;
				}

				if (fileInfo.Name.Substring(0, 4) == "____")
				{
					continue;
				}

				//0KBファイルを削除
				if (fileInfo.Length == 0)
				{
					if (File.Exists(machineFile))
					{
						fileInfo.Delete();
					}
					else
					{
						continue;
					}

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(Constant.MessageInfo.Message_44, fileInfo.FullName));
					continue;
				}

				//ファイルが転送中かの確認
				if (!CheckFileAccess(fileInfo.FullName))
				{
					continue;
				}

				sortFileList.Add(fileInfo);
			}

			//更新日時順にファイルを並び変える
			IEnumerable<FileInfo> sFileList = sortFileList.OrderBy(s => s.LastWriteTime);
			foreach (FileInfo sFile in sFileList)
			{
				rFileList.Add(sFile.FullName);
			}

			return rFileList;
		}

		public static List<string> GetPathList(string machineDirPath)
		{
			return MachineFile.GetPathList(machineDirPath, ".*");
		}

		public static bool CheckMachineFile(string filePath, string splitter, string typeCD, string lotNO, string magNO, DateTime dt, FileScan fileScan, LSETInfo lsetInfo, string chipNm, ref List<ErrMessageInfo> errMsgList, string encodeStr, ref List<Log> logList)
		{
			return CheckMachineFile(filePath, splitter, typeCD, lotNO, magNO, dt, fileScan, lsetInfo, chipNm, ref errMsgList, encodeStr, string.Empty, null, ref logList);
		}

        public bool CheckMachineFile_SpeedUp(List<FileFmtWithPlm> fileFmtWithPlmList, string filePath, string splitter, string typeCD, string lotNO, string magNO, DateTime dt, FileScan fileScan, LSETInfo lsetInfo, string chipNm, string[] withoutvalues, ref List<ErrMessageInfo> errMsgList, string encodeStr, ref List<Log> logList)
        {
            return CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, splitter, null, typeCD, lotNO, magNO, dt, fileScan, lsetInfo, chipNm, withoutvalues
                , ref errMsgList, encodeStr, ref logList, true);
        }

        public bool CheckMachineFile_SpeedUp(List<FileFmtWithPlm> fileFmtWithPlmList, string filePath, string splitter, string typeCD, string lotNO, string magNO, DateTime dt, FileScan fileScan, LSETInfo lsetInfo, string chipNm, string[] withoutvalues, ref List<ErrMessageInfo> errMsgList, string encodeStr, ref List<Log> logList, bool isErrorSound)
		{
			return CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, splitter, null, typeCD, lotNO, magNO, dt, fileScan, lsetInfo, chipNm, withoutvalues
                , ref errMsgList, encodeStr, ref logList, isErrorSound);
		}

		public bool CheckMachineFile_SpeedUp(List<FileFmtWithPlm> fileFmtWithPlmList, string filePath, string splitter, char? itemSplitter, string typeCD, string lotNO, string magNO, DateTime dt, FileScan fileScan, LSETInfo lsetInfo, string chipNm, string[] withoutvalues, ref List<ErrMessageInfo> errMsgList, string encodeStr, ref List<Log> logList, bool isErrorSound)
		{
			string[] txtArray;

			if (MachineFile.TryGetTxt(filePath, splitter, out txtArray, encodeStr, 0) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ内容取得失敗 ﾌｧｲﾙﾊﾟｽ:{0} 行区切り文字:{1}", filePath, @splitter));
			}

			List<string> txtList = new List<string>();
			foreach (string lineTxt in txtArray)
			{
				if (lineTxt.Contains(MachineFile.GetTextSplitter(itemSplitter)))
					txtList.Add(lineTxt);
			}

			return CheckMachineFile_SpeedUp(fileFmtWithPlmList, txtList, itemSplitter, typeCD, lotNO, magNO, dt, fileScan, lsetInfo, chipNm, withoutvalues, ref errMsgList, string.Empty, null, ref logList, isErrorSound);
		}

		public static bool CheckMachineFile(string filePath, string lineSplitter, string typeCD, string lotNO, string magNO, DateTime dt, FileScan fileScan, LSETInfo lsetInfo, string chipNm, ref List<ErrMessageInfo> errMsgList, string encodeStr, string targetData, int? targetDataColIndex, ref List<Log> logList)
		{
			return CheckMachineFile(filePath, lineSplitter, null, typeCD, lotNO, magNO, dt, fileScan, lsetInfo, chipNm, ref errMsgList, encodeStr, targetData, targetDataColIndex, ref logList);
		}


		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="lineSplitter"></param>
		/// <param name="typeCD"></param>
		/// <param name="lotNO"></param>
		/// <param name="magNO"></param>
		/// <param name="dt"></param>
		/// <param name="fileScan"></param>
		/// <param name="lsetInfo"></param>
		/// <param name="errMsgList"></param>
		/// <returns>判定処理が行われていない場合、false</returns>
		public static bool CheckMachineFile(string filePath, string lineSplitter, char? itemSplitter, string typeCD, string lotNO, string magNO, DateTime dt
			, FileScan fileScan, LSETInfo lsetInfo, string chipNm, ref List<ErrMessageInfo> errMsgList, string encodeStr, string targetData
			, int? targetDataColIndex, ref List<Log> logList)
		{
			DateTime lastDt = DateTime.Now;

			List<string> exclusionList = new List<string>();

			string headerNm;
			try
			{
				string[] txtArray;

				if (MachineFile.TryGetTxt(filePath, lineSplitter, out txtArray, encodeStr, 0) == false)
				{
					throw new ApplicationException(
						string.Format("ﾌｧｲﾙ内容取得失敗 ﾌｧｲﾙﾊﾟｽ:{0} 行区切り文字:{1}", filePath, @lineSplitter));
				}

				List<string> headerList = new List<string>();

				if (itemSplitter.HasValue == false)
				{
					headerList = MachineFile.GetHeaderList(txtArray, fileScan.HeaderScanInRowFG);
				}
				else
				{
					headerList = MachineFile.GetHeaderList(txtArray, fileScan.HeaderScanInRowFG, itemSplitter.Value);
				}

				List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(fileScan.PrefixNM, lsetInfo, fileScan.StartUpFG);

				foreach (FILEFMTInfo fileFmt in fileFmtList)
				{
					string valStr = string.Empty;

					Plm plmInfo = new Plm();
					if (lsetInfo.ReferMultiServerFG)
					{
						plmInfo = Plm.GetCurrentFromMultipleServer(lsetInfo.InlineCD, typeCD, lsetInfo.ModelNM, fileFmt.QCParamNO, lsetInfo.EquipmentNO, false, chipNm, lsetInfo.ReferMultiServerFG);
					}
					else
					{
						plmInfo = Plm.GetData(lsetInfo.InlineCD, typeCD, lsetInfo.ModelNM, fileFmt.QCParamNO, lsetInfo.EquipmentNO, false, chipNm);
					}

					if (plmInfo == null)
					{
						//設定されていない場合、装置処理停止
						string message = string.Format(Constant.MessageInfo.Message_28, typeCD, fileFmt.QCParamNO, fileFmt.ParameterNM);
						throw new ApplicationException(message);
					}

					headerNm = fileFmt.HeaderNM.ToUpper();

					//ファイルの走査方向に従ってファイル内容中をパラメタ名称で検索をかける
					int headerIndex = headerList.FindIndex(h => h == headerNm);

					if (headerIndex < 0)
					{
						throw new ApplicationException(string.Format("ﾌｧｲﾙからのﾃﾞｰﾀ取得時に異常発生(ﾌｧｲﾙ内にﾍｯﾀﾞが見つかりませんでした。) ﾌｧｲﾙﾊﾟｽ:{0} ﾊﾟﾗﾒﾀ番号:{1} ﾍｯﾀﾞ:{2}", filePath, fileFmt.QCParamNO, fileFmt.HeaderNM));
					}

					string[] dataArray;
					try
					{
						//ファイルからデータ取得
						if (itemSplitter.HasValue)
						{
							dataArray = MachineFile.GetData(txtArray, itemSplitter.Value, headerIndex, fileScan.HeaderScanInRowFG, targetData, targetDataColIndex);
						}
						else
						{
							dataArray = MachineFile.GetData(txtArray, headerIndex, fileScan.HeaderScanInRowFG, targetData, targetDataColIndex);
						}

						if (dataArray.Count() == 0)
						{
							exclusionList.Add(string.Format("ﾍｯﾀﾞ名：{0}", headerNm));
							continue;
						}
					}
					catch (Exception err)
					{
						throw new ApplicationException(string.Format("ﾌｧｲﾙからのﾃﾞｰﾀ取得時に異常発生:(例外内容:{0}) ﾌｧｲﾙﾊﾟｽ:{1} ﾊﾟﾗﾒﾀ番号:{2} ﾍｯﾀﾞ:{3}", err.Message, filePath, fileFmt.QCParamNO, fileFmt.HeaderNM), err);
					}

					//データサンプリング(指定位置・数量のデータに限定)
					string[] sampledData = PrmSummary.ConvertToSummaryData(dataArray, fileFmt.QCParamNO, typeCD, lsetInfo.InlineCD);

					//データ集計・計算処理
					string calculatedValStr;
					try
					{
						calculatedValStr = MachineFile.GetCalculatedResult(plmInfo, sampledData, null, false, lsetInfo.InlineCD, lsetInfo.EquipmentNO, dt.ToString(), plmInfo.RefQcParamNO);
					}
					catch (Exception err)
					{
						throw new ApplicationException(string.Format("{0} ﾍｯﾀﾞ名(ﾌｧｲﾙ):{1}", err.Message, fileFmt.HeaderNM), err);
					}

					MachineBase.OutputErr(lsetInfo, plmInfo, typeCD, magNO, lotNO, calculatedValStr, dt.ToString(), ref errMsgList, ref logList);
				}

				if (exclusionList.Count > 0)
				{
					string logMsg = string.Format("設備:{0} 設備CD:{1} 号機:{2} 空行・除外値除去後に抽出出来るﾃﾞｰﾀが存在しませんでした。ﾌｧｲﾙﾊﾟｽ:{3}\r\n{4}"
						, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, filePath, string.Join(" , ", exclusionList));

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				}

				return true;
			}
			catch (Exception err)
			{
				throw new ApplicationException(string.Format("ﾌｧｲﾙ内容の判定にて異常有り ﾌｧｲﾙﾊﾟｽ：{0}", filePath) + err.Message, err);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="filePath"></param>
		/// <param name="splitter"></param>
		/// <param name="typeCD"></param>
		/// <param name="lotNO"></param>
		/// <param name="magNO"></param>
		/// <param name="dt"></param>
		/// <param name="fileScan"></param>
		/// <param name="lsetInfo"></param>
		/// <param name="errMsgList"></param>
		/// <returns>判定処理が行われていない場合、false</returns>
		public bool CheckMachineFile_SpeedUp(List<FileFmtWithPlm> fileFmtWithPlmList, IEnumerable<string> fileContents, char? itemSplitter
			, string typeCD, string lotNO, string magNO, DateTime dt, FileScan fileScan, LSETInfo lsetInfo, string chipNm, string[] withoutvalues, ref List<ErrMessageInfo> errMsgList
			, string targetData, int? targetDataColIndex, ref List<Log> logList, bool isErrorSound)
		{
			List<string> exclusionList = new List<string>();

			List<Log> localLogList = new List<Log>();
			List<ErrMessageInfo> localErrMsgList = new List<ErrMessageInfo>();

			try
			{
				List<string> headerList = new List<string>();// MachineFile.GetHeaderList(fileContents.ToArray(), fileScan.HeaderScanInRowFG);

				if (itemSplitter.HasValue == false)
				{
					headerList = MachineFile.GetHeaderList(fileContents.ToArray(), fileScan.HeaderScanInRowFG);
				}
				else
				{
					headerList = MachineFile.GetHeaderList(fileContents.ToArray(), fileScan.HeaderScanInRowFG, itemSplitter.Value);
				}

				object lockErrObj = new object();
				object lockLogObj = new object();
				object lockPlmObj = new object();

				/////////////////////////////////////////////////////////////////////////
				try
				{
                    //検証時はParallel.ForEachでのデバッグは難しいのでforeachの方を使用しても可
                    //foreachのままリリースした場合、マルチコア・マルチスレッド処理による高速化の恩恵が受けられなくなるので要注意

                    //Parallel.ForEach(fileFmtWithPlmList, fileFmtWithPlm =>
                    foreach (FileFmtWithPlm fileFmtWithPlm in fileFmtWithPlmList)
                    /////////////////////////////////////////////////////////////////////////
                    {

						EICS.Machine.MachineBase.JudgeResult jr = new MachineBase.JudgeResult();

						List<Log> paralocalLogList = new List<Log>();
						List<ErrMessageInfo> paralocalErrMsgList = new List<ErrMessageInfo>();

						string headerNm;
						string valStr = string.Empty;

						FILEFMTInfo fileFmt = fileFmtWithPlm.FileFmt;
						Plm plmInfo = fileFmtWithPlm.Plm;

						if (plmInfo == null)
						{
							//設定されていない場合、装置処理停止
							string message = string.Format(Constant.MessageInfo.Message_28, typeCD, fileFmt.QCParamNO, fileFmt.ParameterNM);
							throw new Exception(message);
						}

						headerNm = fileFmt.HeaderNM.ToUpper();

						//ファイルの走査方向に従ってファイル内容中をパラメタ名称で検索をかける
						int headerIndex = headerList.FindIndex(h => h == headerNm);

						if (headerIndex < 0)
						{
							throw new ApplicationException(string.Format("ﾌｧｲﾙからのﾃﾞｰﾀ取得時に異常発生(ﾌｧｲﾙ内にﾍｯﾀﾞが見つかりませんでした。) ﾊﾟﾗﾒﾀ番号:{0} ﾍｯﾀﾞ:{1}", fileFmt.QCParamNO, fileFmt.HeaderNM));
						}

						string[] dataArray;
						try
						{
							//ファイルからデータ取得
							//dataArray = MachineFile.GetData(fileContents.ToArray(), headerIndex, fileScan.HeaderScanInRowFG, targetData, targetDataColIndex);
							if (itemSplitter.HasValue)
							{
								dataArray = MachineFile.GetData(fileContents.ToArray(), itemSplitter.Value, headerIndex, fileScan.HeaderScanInRowFG, targetData, targetDataColIndex);
							}
							else
							{
								dataArray = MachineFile.GetData(fileContents.ToArray(), headerIndex, fileScan.HeaderScanInRowFG, targetData, targetDataColIndex);
							}

							if (dataArray.Count() == 0)
							{
								exclusionList.Add(string.Format("ﾍｯﾀﾞ名：{0}", headerNm));
							}
						}
						catch (Exception err)
						{
							throw new ApplicationException(string.Format("ﾌｧｲﾙからのﾃﾞｰﾀ取得時に異常発生:(例外内容:{0}) ﾊﾟﾗﾒﾀ番号:{1} ﾍｯﾀﾞ:{2}", err.Message, fileFmt.QCParamNO, fileFmt.HeaderNM), err);
						}

                        // 取得データを修正する。
                        if (withoutvalues != null && withoutvalues.Length > 0)
                        {
                            dataArray = dataArray.Where(d => withoutvalues.Contains(d) == false).ToArray();
                            if (dataArray.Length == 0)
                            {
                                dataArray = new string[] { "0" };
                                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "加工後のファイル取得データが空になったので0を格納します。");
                            }
                        }

                        if (fileScan.StartUpFG == true && dataArray.Count() > 0)
						{
							string registerData = dataArray[0];
							//差分集計が必要なら前回値取得して差分計算する仕組みを追加
							//plmInfo.RefQcParamNO
							if (plmInfo.TotalKB == Constant.SP_CALC && plmInfo.RefQcParamNO.HasValue && plmInfo.RefQcParamNO.Value != 0)
							{
								decimal dValue;
								if (decimal.TryParse(dataArray[0], out dValue) == false)//sValueが0に近い値の場合、失敗する為その対応
								{
									throw new ApplicationException($"ﾊﾟﾗﾒﾀ番号:『{plmInfo.QcParamNO}』 :ファイルから取得した値の数値変換に失敗した為、0を格納します。変換対象:『{dataArray[0]}』");
								}

								registerData = CalcGap(lsetInfo, dt, dValue, plmInfo.RefQcParamNO.Value);
							}
							else
							{
								registerData = MachineFile.GetCalculatedResult(plmInfo, dataArray.Take(1).ToArray(), null, false, lsetInfo.InlineCD, lsetInfo.EquipmentNO, dt.ToString(), plmInfo.RefQcParamNO);
							}
							//QcParamNOがあるパラメタのRefQcParamNOに設定されている場合、前回値集計で前回値を参照出来るようにする為にデータベース登録が必要となる
							bool needRegisterData = fileFmtWithPlmList.Exists(f => f.Plm.RefQcParamNO == plmInfo.QcParamNO);

							jr = MachineBase.JudgeParam(lsetInfo, plmInfo, typeCD, magNO, lotNO, registerData, dt.ToString(), paralocalErrMsgList, paralocalLogList, needRegisterData, isErrorSound);
						}
						else if (fileScan.StartUpFG == false && dataArray.Count() > 0)
						{
							//データサンプリング(指定位置・数量のデータに限定)
							string[] sampledData = PrmSummary.ConvertToSummaryData(dataArray, fileFmt.QCParamNO, typeCD, lsetInfo.InlineCD);

							//データ集計・計算処理
							string calculatedValStr;
							try
							{
								calculatedValStr = MachineFile.GetCalculatedResult(plmInfo, sampledData, null, false, lsetInfo.InlineCD, lsetInfo.EquipmentNO, dt.ToString(), plmInfo.RefQcParamNO);
							}
							catch (Exception err)
							{
								throw new ApplicationException(string.Format("{0} ﾍｯﾀﾞ名(ﾌｧｲﾙ):{1}", err.Message, fileFmt.HeaderNM), err);
							}

							jr = MachineBase.JudgeParam(lsetInfo, plmInfo, typeCD, magNO, lotNO, calculatedValStr, dt.ToString(), paralocalErrMsgList, paralocalLogList, true, isErrorSound);
						}
						else
						{
							string dataGetErrMsg = string.Format("type:{0} model:{1} qcParamNo:{2}でﾌｧｲﾙからﾃﾞｰﾀが取得できません。", typeCD, lsetInfo.ModelNM, fileFmt.QCParamNO);
							ErrMessageInfo errMsgInfo = new ErrMessageInfo(dataGetErrMsg, System.Drawing.Color.Red);

							jr.ErrMessageList = new List<ErrMessageInfo>();
							jr.ErrMessageList.Add(errMsgInfo);
							jr.LogList = new List<Log>();
						}

						if (jr.ErrMessageList.Count > 0)
						{
							lock (lockErrObj)
							{
								localErrMsgList.AddRange(jr.ErrMessageList);
							}
						}

						if (jr.LogList.Count > 0)
						{
							lock (lockLogObj)
							{
								localLogList.AddRange(jr.LogList);
							}
						}
                    }//foreachの時はこっちの閉じかっこ使う
                //});//Parallel.ForEachの時はこっちの閉じかっこ使う
				}
				//catch(Exception ex)//foreachの時はこっちのcatchを使う
				//{
				//	throw new ApplicationException(ex.ToString());
				//}
				catch (AggregateException ex)
				{
					if (localErrMsgList.Count > 0)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "並列処理中に例外発生。例外発生までの間に確認されたエラーを表示します。");
						foreach (ErrMessageInfo localErrMsgInfo in localErrMsgList)
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, localErrMsgInfo.MessageVAL);
						}
					}

					string errMsg = string.Format("\r\n並列化処理中 ｴﾗｰ件数：{0}\r\n", ex.InnerExceptions.Count);
					int errCt = 0;
					foreach (var err in ex.InnerExceptions)
					{
						errMsg += string.Format("ｴﾗｰNo:{0} ｴﾗｰ内容:{1}\r\nｽﾀｯｸﾄﾚｰｽ:{2}\r\n\r\n", errCt, err.Message, err.StackTrace);
					}
					throw new ApplicationException(errMsg);
				}

				logList.AddRange(localLogList);
				errMsgList.AddRange(localErrMsgList);

				if (exclusionList.Count > 0)
				{
					string logMsg = string.Format("設備:{0} 設備CD:{1} 号機:{2} 空行・除外値除去後に抽出出来るﾃﾞｰﾀが存在しませんでした。\r\n{3}"
						, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, string.Join(" , ", exclusionList));

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg);
				}

				return true;
			}
			catch (Exception err)
			{
				throw new Exception(string.Format("ﾌｧｲﾙ内容の判定にて異常有り ：") + err.Message, err);
			}
		}
        
		private string CalcGap(LSETInfo lsetInfo, DateTime measureDt, decimal currentVal, int refQcParamNO)
		{
			string calculatedValStr;
			//前回取得値(同装置での前ロット取得値)を取得
			decimal backLotVAL = ConnectDB.GetTnLOG_DParam(lsetInfo.InlineCD, lsetInfo.EquipmentNO, measureDt.ToString(), refQcParamNO, 1);

			if (backLotVAL == decimal.MinValue)
			{
				//前回取得値が存在しなかった場合(初期ロット)、異常判定はしないので0を格納
				calculatedValStr = "0";
			}
			else
			{
				//前回取得値との差異を格納
				calculatedValStr = Convert.ToString(Math.Abs(backLotVAL - currentVal));
			}
			return calculatedValStr;
		}
	}
}
