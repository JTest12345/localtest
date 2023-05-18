using LENS2_Api;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LENS2
{
	class CommonProcess
	{
		public const string NOT_FOUND_MACHINELOG_MSG =
									"監視フォルダに{0}ファイルが見つかりません。\r\n監視フォルダ:{1}\r\n" +
									"何らかの理由により装置の出力ファイルが見つからない為、メンテナンスを行って下さい。\r\n\r\n";

		public const string HOW_TO_MACHINELOG_MAINTE_MSG =
									"【メンテナンス方法】\r\n" +
									"メニューのデータメンテナンスから『装置・ファイル取得要求メンテ』画面を呼び出し\r\n" +
									"この装置の{0}ファイル取得要求を0にして下さい。\r\n";

		public static string GetLabelValueFromFileContents(string[] fileLineValueList, int dataStartRow, int targetCol)
		{
			string labelValue = string.Empty;

			for (int targetRow = fileLineValueList.Count() - 1; targetRow >= dataStartRow; targetRow--)
			{
				if (string.IsNullOrEmpty(fileLineValueList[targetRow]))
				{
					//空白行の場合、手前の行をサーチする
					continue;
				}

				string[] fileLineValue = fileLineValueList[targetRow].Split(',');
				labelValue = fileLineValue[targetCol].Trim();

				if (string.IsNullOrEmpty(labelValue))
				{
					//マガジン・ロットNo列がnullか空白なら手前の行をサーチする
					continue;
				}
				else
				{
					//何らかの文字列が入っていた場合、サーチ終了
					break;
				}
			}

			return labelValue;
		}

		/// <summary>
		/// ロックされていないファイルの移動処理
		/// ロックされている場合、指定回数リトライする。
		/// </summary>
		/// <param name="fromFileFullPath"></param>
		/// <param name="toFileFullPath"></param>
		/// <param name="retryNum"></param>
		/// <returns></returns>
		public static bool MoveUnlockFile(string fromFileFullPath, string toFileFullPath, int retryNum)
		{
			if (retryNum < 1)
			{
				retryNum = 1;
			}

			for (int tryNum = 0; tryNum < retryNum; tryNum++)
			{
				try
				{
					File.Move(fromFileFullPath, toFileFullPath);

					return true;
				}
				catch (IOException)
				{
					continue;
				}
			}
			return false;
		}

		public static bool MoveOldFiles(string fileKind, string fromDirPath, string toDirPath, int dateTimeStartIndex, int dateTimeStringLen)
		{
			/* TODO開発中
			string sfname = "";
			string sWork = "";
			string sFileType = "";
			string[] textArray = new string[] { };
			
			string[] fileNameArray = Directory.GetFiles(fromDirPath, "*" + fileKind + "*");
			int fileNum = fileNameArray.Length;

			string[] sortedCreateTime = GetSortedDateTimeStringListFromFileName(fileNameArray, dateTimeStartIndex, dateTimeStringLen);

			//発見ファイル数から最新1ファイルを除いてループ
			for (int fileIndex = 0; fileIndex < fileNum - 1; fileIndex++)
			{
				//日時分毎のファイルフルパスリストを取得
				string[] fileNameArrayPerDT = Directory.GetFiles(fromDirPath, "*" + fileKind + sortedCreateTime[fileIndex] + "*");

				foreach (string fileFullPath in fileNameArrayPerDT)
				{
					FileInfo file = new FileInfo(fileFullPath);

					//ファイル名に付いている日付を確認して、最新ファイル以外は未登録場所へ移動する。
					string moveToFullPath = Path.Combine(toDirPath, file.Name);

					if (!Directory.Exists(toDirPath))
					{
						Directory.CreateDirectory(toDirPath);
					}
					else
					{
						if (File.Exists(moveToFullPath))
						{
							//移動済みﾌｧｲﾙは削除して次へ
							File.Delete(fileFullPath);
							continue;
						}
					}

					MoveUnlockFile(fileFullPath, moveToFullPath, RETRYNUM);

				}
			}
			 */
			throw new NotImplementedException();
		}

		private static string[] GetSortedDateTimeStringListFromFileName(string[] fileNameArray, int dateTimeStartIndex, int dateTimeStringLen)
		{
			int fileNum = fileNameArray.Length;
			string[] sortedCreateTime = new string[fileNum];

			int fileIndex = 0;

			//ファイル名から日付文字列取得
			foreach (string fileFullPath in fileNameArray)
			{
				FileInfo file = new FileInfo(fileFullPath);

				sortedCreateTime[fileIndex] = file.Name.Substring(dateTimeStartIndex, dateTimeStringLen);//ﾌｧｲﾙ名に付加されている日付文字列取得
				fileIndex++;
			}
			Array.Sort(sortedCreateTime);

			return sortedCreateTime;
		}

		/// <summary>
		/// ファイルからヘッダを除いた内容を取得
		/// </summary>
		/// <param name="fileFullPath"></param>
		/// <param name="valueStartLine">ヘッダを除いた内容が開始される行数</param>
		/// <param name="retryNum">ファイルアクセスの最大リトライ回数</param>
		/// <returns>ヘッダを除いた指定ファイル内容、ヘッダを除いた内容開始行が空白、空だった場合とファイルアクセス出来なかった場合はnull</returns>
		public static string[] GetFileContentsWithoutHeader(string fileFullPath, int valueStartLine, int retryNum)
		{
			/* TODO開発中
			if (retryNum < 1)
			{
				retryNum = 1;
			}

			string[] fileContentsWithoutHeader = null;

			for (int tryNum = 0; tryNum < retryNum; tryNum++)
			{
				try
				{
					using (System.IO.StreamReader sr = new System.IO.StreamReader(fileFullPath, FILEENCODE))
					{
						string[] fileContents = sr.ReadToEnd().Split('\n');
						
						int skipLineNum = valueStartLine - 1;

						fileContentsWithoutHeader = fileContents.Skip(skipLineNum).ToArray();
					}
				}
				catch (IOException)
				{
					continue;
				}
			}

			if (fileContentsWithoutHeader == null)
			{
				//ファイルアクセスが行えなかった事をログ出力
				return null;
			}

			if (string.IsNullOrEmpty(fileContentsWithoutHeader[valueStartLine].Trim()))
			{
				File.Delete(fileFullPath);

				//ファイルの中身が空白である事をログ出力
				return null;
			}
			else
			{
				return fileContentsWithoutHeader;
			}
			 */
			throw new NotImplementedException();
		}
	}
}
