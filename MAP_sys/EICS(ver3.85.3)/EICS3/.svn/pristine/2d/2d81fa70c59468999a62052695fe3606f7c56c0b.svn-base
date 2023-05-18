using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Text.RegularExpressions;
using System.Net;
using EICS.Database;

namespace EICS
{
    public class Common
    {
		public const string PLC_CARRIER_NO_HEADER = "31 ";

		private static string[] NichiaLM32Array = {"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", 
												   "A", "C", "D", "E", "F", "G", "H", "J", "K", "L",
												   "M", "N", "P", "Q", "R", "T", "U", "V", "W", "X",
												   "Y", "Z"};

		public static List<string> Nichia32Array = new string[]
														{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", 
														 "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
														 "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
														 "U", "V"}.ToList();

        private static List<string> Nichia36Array = new string[]
														{"0", "1", "2", "3", "4", "5", "6", "7", "8", "9", 
														 "A", "B", "C", "D", "E", "F", "G", "H", "I", "J",
														 "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T",
														 "U", "V", "W", "X", "Y", "Z"}.ToList();

		
        public static object GetParameterValue(object targetValue)
        {
            if (targetValue == null || targetValue.ToString() == "")
            {
                return DBNull.Value;
            }
            else
            {
                return targetValue;
            }
        }


        public static string Join(string separator, byte[] value) 
        {
            return Join(separator, value, int.MinValue);
        }
        public static string Join(string separator, byte[] value, int format)
        {
            string rValue = string.Empty;
            foreach(byte val in value)
            {           
                switch(format)
                {
                    case int.MinValue:
                        rValue += val + separator;
                        break;
                    case 16:
                        rValue += val.ToString("X") + separator;
                        break;
                }
            }
            return rValue;
        }

		private static int GetDBYear(string lotNO)
		{
			int year;
			int startIndex = 1;
			int length = 2;

			if (!int.TryParse(lotNO.Substring(startIndex, length), out year))
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_116, lotNO, startIndex + 1, length, lotNO.Substring(startIndex, length)));
			}
			
			return year;
		}

		private static int GetDBMonth(string lotNO)
		{
			int month;
			int startIndex = 3;
			int length = 1;
			
			month = Nichia32Array.IndexOf(lotNO.Substring(startIndex, length).ToUpper());

			if (month < 1 || month > 12)
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_116, lotNO, startIndex + 1, length, lotNO.Substring(startIndex, length)));
			}
			
			return month;
		}

		private static int GetDBDay(string lotNO)
		{
			int day;
			int startIndex = 4;
			int length = 1;

			day = Nichia32Array.IndexOf(lotNO.Substring(startIndex, length).ToUpper());

			if (day < 1 || day > 31)
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_116, lotNO, startIndex + 1, length, lotNO.Substring(startIndex, length)));
			}
			
			return day;
		}

		private static string Dec2BaseNichia32OneDigit(string sDec)
		{
			int dec;

			if (!int.TryParse(sDec, out dec))
			{
				return null;
			}

			return Dec2BaseNichia32OneDisit(dec);
		}

		private static string Dec2BaseNichia32OneDisit(int Dec)
		{
			if (Dec >= 32)
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_117, Dec));
			}

			return NichiaLM32Array[Dec];
		}

		//車載の自動化/非自動化で処理を切り分けた為、下記の関数はコメントアウト
        //public static string GetLotMarkingNO(string lotNO) 
        //{
        //    string slotMarkingNO = string.Empty;
        //    string sPartialLotNO = string.Empty;

        //    slotMarkingNO += NichiaLM32Array[GetDBYear(lotNO)];
        //    slotMarkingNO += NichiaLM32Array[GetDBMonth(lotNO)];
        //    slotMarkingNO += NichiaLM32Array[GetDBDay(lotNO)];

        //    sPartialLotNO = Dec2BaseNichia32OneDigit(lotNO.Substring(6, 2));
        //    if (string.IsNullOrEmpty(sPartialLotNO))
        //    {
        //        throw new Exception(string.Format(Constant.MessageInfo.Message_118, lotNO.Substring(6, 2)));
        //    }

        //    slotMarkingNO += sPartialLotNO;
        //    slotMarkingNO += lotNO.Substring(8, 1);

        //    return slotMarkingNO;
        //}

        private static int Nichia36OneDigit2Dec(string str)
        {
            bool isCorrespond = false;
            int i;
            for (i = 0; i < 36; i++)
            {
                if (Nichia36Array[i] == str)
                {
                    isCorrespond = true;
                    break;
                }
            }

            if (isCorrespond)
            {
                return i;
            }
            throw new Exception(string.Format("36進数から10進数へ変換できませんでした｡:{0}", str));
        }

        //返り値をマーキング文字の文字列からクラスに変更。（完全連番はシリアルNoが必要なため）
        public static LotMark GetLotMarkingNO(int lineCD, string lotNo, Constant.TypeGroup typeGrp, Constant.LineType lineType, bool fullSerialNoModeFG, int markingDigit)
        {
            LotMark LotMarkData = new LotMark();

			LotMarkData.MarkNo = string.Empty;

            //2016.3.14湯浅 完全連番フラグ(FullSerialNoModeFG)で処理分岐

            if (fullSerialNoModeFG == true)
            {
                LotMarkData = LotNoConv.GetFullSerialNoMarikingChar(lotNo, typeGrp, lineCD, markingDigit);
            }
            else
            {
                LotMarkData.LotNo = lotNo;
                LotMarkData.SerialNo = 0;

                LotMarkData.MarkNo += NichiaLM32Array[GetDBYear(lotNo)];
                LotMarkData.MarkNo += NichiaLM32Array[GetDBMonth(lotNo)];
                LotMarkData.MarkNo += NichiaLM32Array[GetDBDay(lotNo)];

                LotMarkData.MarkNo += GetMarkingSerialNo(lineCD, lotNo, typeGrp, lineType);
            }

            return LotMarkData;
        }

        public static LotMark GetLotMarkingNO(int lineCD, string lotNo, Constant.TypeGroup typeGrp, Constant.LineType lineType)
        {
            return GetLotMarkingNO(lineCD, lotNo, typeGrp, lineType, false, 0);
        }

		private static string GetMarkingSerialNo(int lineCD, string lotNo, Constant.TypeGroup typeGrp, Constant.LineType lineType)
		{
			string markingSerialNo = string.Empty;

			switch (typeGrp)
			{
				case Constant.TypeGroup.AutoMotive:
				case Constant.TypeGroup.MPL:
				case Constant.TypeGroup.x83:
				case Constant.TypeGroup.Map:
				case Constant.TypeGroup.x19:
					if (lineType == Constant.LineType.Auto || lineType == Constant.LineType.Hybrid)
					{
						markingSerialNo = GetMarkingSerialNoForGAAuto(lotNo);
					}
					else
					{
						SettingInfo commonSettingInfo = SettingInfo.GetSingleton();

						string convTarget = lotNo.Substring(commonSettingInfo.LotNoConvStartIndex,  commonSettingInfo.LotNoConvLen);
						string convertID = commonSettingInfo.LotNoConvertID;

						markingSerialNo = LotNoConv.GetConvertedLotNo(lineCD, convertID, convTarget);
					}
					break;
				default:
					throw new ApplicationException(string.Format("ロットNoからマーキングロットNoへの変換が未定義です。システム担当者へ連絡して下さい。 ロットNo：{0} 設定ファイルタイプGr： {1} 設定ファイルラインタイプ：{2}", lotNo, typeGrp, lineType));
					//break;
			}

			if (string.IsNullOrEmpty(markingSerialNo))
			{
				throw new ApplicationException(string.Format("ロット連番に当たる番号からマーキング用の文字列取得が行えませんでした。システム担当者へ連絡して下さい。 ロットNo：{0}", lotNo));
			}

			return markingSerialNo;
		}

		private static string GetMarkingSerialNoForGANonAuto(string lotNo)
		{


			//ロットNoの7,8,9桁目の連番を取得
			string sSerialNo = lotNo.Substring(6, 3);
			int iSerialNo;

			if (!int.TryParse(sSerialNo, out iSerialNo))
			{
				throw new Exception(string.Format("ロットNoを数値変換できませんでした｡ 変換対象:{0} ロットNo：{1}", sSerialNo, lotNo));
			}

			string markingSerialNo = IntToBase32(iSerialNo);

			markingSerialNo = markingSerialNo.PadLeft(2, '0');

			return markingSerialNo;
		}

		private static string GetMarkingSerialNoForGAAuto(string lotNo)
		{
			string markingSerialNo = string.Empty;

			int lotNo1;
			string sLotNo1 = lotNo.Substring(10, 1);

			if (!int.TryParse(sLotNo1, out lotNo1))
			{
				throw new Exception(string.Format("ロットNoを数値変換できませんでした｡ 変換対象:{0} ロットNo：{1}", sLotNo1, lotNo));
			}

			string sLotNo2 = lotNo.Substring(9, 1).ToUpper();

			int ilotNo = Nichia36OneDigit2Dec(sLotNo2) * 10 + lotNo1;

			markingSerialNo = IntToBase32(ilotNo);

			if (string.IsNullOrEmpty(markingSerialNo))
			{
				throw new Exception(string.Format(Constant.MessageInfo.Message_118, lotNo.Substring(6, 2)));
			}

			markingSerialNo = markingSerialNo.PadLeft(2, '0');

			return markingSerialNo;
		}

        private static string IntToBase32(int b)
        {
            string result = string.Empty;

            long amari = 0;
            while (true)
            {
				if (b < NichiaLM32Array.Length)
                {
                    result = NichiaLM32Array[b] + result;
                    break;
                }
				amari = b % NichiaLM32Array.Length;
				b = b / NichiaLM32Array.Length;
                result = NichiaLM32Array[amari] + result;
            }
            return result;
        }


		/// <summary>
		/// 正規表現で指定したパス下のファイル一覧を取得する。
		/// extentionSearchPatternには拡張子の.(ピリオド)は含む必要無し
		/// Directory.GetFiles()ではファイル名に任意の文字を含むファイル一覧の取得が出来ない為、自作
		/// </summary>
		/// <param name="path">Directory.GetFiles()のpathと同じ</param>
		/// <param name="fileNameSearchPattern">ファイル名部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <param name="extentionSearchPattern">拡張子部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <returns></returns>
		public static List<string> GetFiles(string path, string fileNameSearchPattern, string extentionSearchPattern)
		{
			string[] pathArray = Directory.GetFiles(path, "*.*");

			Regex regex = new Regex(fileNameSearchPattern + "[.]" + extentionSearchPattern);

			return pathArray.Where(p => regex.IsMatch(p)).ToList();
		}

		/// <summary>
		/// 正規表現で指定したパス下のファイル一覧を取得する。
		/// .は任意の一文字 *は手前の文字の0文字以上の +は手前の文字の1文字以上の、?は手前の文字の0または1文字以上の繰り返し
		/// Directory.GetFiles()ではファイル名に任意の文字を含むファイル一覧の取得が出来ない為、自作
		/// </summary>
		/// <param name="path">Directory.GetFiles()のpathと同じ</param>
		/// <param name="fileNameSearchPattern">ファイル名部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <param name="extentionSearchPattern">拡張子部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <returns></returns>
		public static List<string> GetFiles(string path, string searchPattern)
		{
			string[] pathArray = Directory.GetFiles(path, "*.*");

			Regex regex = new Regex(searchPattern);

			return pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="targetIndex"></param>
		/// <param name="targetLen"></param>
		/// <param name="targetStr"></param>
		/// <param name="anyStrAfterFg">指定文字列の後に任意文字列が付く事を許すか</param>
		/// <returns></returns>
		public static string GetSearchPatternStr(int targetIndex, string targetStr, bool anyStrAfterFg)
		{
			string searchPatternStr = string.Empty;

			for (int i = 0; i < targetIndex; i++)
			{
				searchPatternStr += ".";
			}

			searchPatternStr += targetStr;

			if (anyStrAfterFg)
			{
				searchPatternStr += ".*";
			}

			return searchPatternStr;
		}

		public static bool IsLEDChip(string chipNM)
		{
			if (chipNM.ToUpper().Contains(Constant.Chip.LED.ToString()) || chipNM.ToUpper().Contains(Constant.Chip.RED.ToString()) ||
				chipNM.ToUpper().Contains(Constant.Chip.GREEN.ToString()) || chipNM.ToUpper().Contains(Constant.Chip.BLUE.ToString()))
			{
				return true;
			}
			else
			{
				return false;
			}
		}

        /// <summary>
        /// 指定したディレクトリ内の全ファイル/ディレクトリを削除する
        /// </summary>
        /// <param name="dirPath"></param>
        public static void ClearDirectory(string dirPath)
        {
            string[] allFile = Directory.GetFiles(dirPath, "*.*", SearchOption.AllDirectories);
            foreach(string file in allFile)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"ClearDirectoryによるファイル削除実行：{file}");
                File.Delete(file);
            }

            string[] allDir = Directory.GetDirectories(dirPath, "*", SearchOption.AllDirectories);
            foreach(string dir in allDir)
            {
                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"ClearDirectoryによるディレクトリ削除実行：{dir}");
                Directory.Delete(dir);
            }
        }
    }
}
