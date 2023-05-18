using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace EICS.Structure
{
	class CIFS
	{
		public const string EXT_OK_FILE = "OK";
		public const string EXT_NG_FILE = "NG";
		public const string EXT_STOP_FILE = "STOP";
		public const string EXT_FIN_FILE = "fin";
		public const string EXT_FIN2_FILE = "fin2";
		public const string EXT_AD830_FIN_FILE = "out"; //AD838L改修時にAD830の改修依頼忘れらしく、finで出てこないので .outにも対応 2016/6/6 n.yoshi
		public const char FILE_NM_SPLITTER = '_';
		private const string DESELECT_IDENTIFIER = "_";
		private const string TRIG_FILE_NM_FORMAT = ".+\\.{0}$";
		protected const int ERR_MSG_MAX_LEN = 200;

		/// <summary>
		/// dirPath直下のextStrを拡張子に持ち処理可能な状態のトリガファイルが存在するかどうかを返す
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="extStr"></param>
		/// <param name="isNeedAddedInfo">ファイル名へのロットNo、工程Noなどの付加情報を必要とするかどうか</param>
		/// <returns></returns>
		public static List<string> GetProcessableTriggerFileNmList(string dirPath, string extStr, bool isNeedAddedInfo)
		{
			try
			{
				if (extStr.Contains('.'))
				{
					extStr = extStr.Replace(".", "");
				}

				List<string> trigFileList = Common.GetFiles(dirPath, string.Format(TRIG_FILE_NM_FORMAT, extStr));

				List<string> processableTrigFileList = new List<string>();
				foreach (string fileNm in trigFileList)
				{
					//ファイルが処理可能なものかどうかチェック
					if (IsProcessableFile(fileNm, isNeedAddedInfo))
					{
						//処理可能な奴ならリストに追加
						processableTrigFileList.Add(fileNm);
					}
				}

				return processableTrigFileList;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// dirPath直下のextStrを拡張子に持つファイル中で最後のファイル群を示す識別文字を返す
		/// 引数で指定した文字列を昇順で並べ、最も最後となるファイルを取得する。
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="extStr"></param>
		/// <param name="dateIndex"></param>
		/// <param name="dateLen"></param>
		/// <returns></returns>
		public static string GetLatestProcessableFileIdentity(string dirPath, string extStr, int extractIndex, int extractLen, bool isNeedAddedInfo)
		{
			try
			{
				//コードレビュー指摘・修正後コード
				DirectoryInfo di = new DirectoryInfo(dirPath);

                extStr = extStr.Replace(".", "");
                Regex regex = new Regex(extStr);

				FileInfo[] fiArray = di.GetFiles().OrderByDescending(d => d.CreationTime).Where(r => regex.IsMatch(r.Name)).ToArray();

				foreach (FileInfo fi in fiArray)
				{
					if (IsProcessableFile(fi.Name, isNeedAddedInfo))
					{
						return fi.Name.Substring(extractIndex, extractLen);
					}
				}

				return null;

				//コードレビュー指摘・修正前コード
				//Dictionary<DateTime, string> processableTrigDtIdDict = new Dictionary<DateTime, string>();
				//List<string> processableTrigList = GetProcessableTriggerFileNmList(dirPath, extStr, isNeedAddedInfo);

				//if (processableTrigList.Count == 0)
				//{
				//	return string.Empty;
				//}

				//foreach (string filePath in processableTrigList)
				//{
				//	DateTime creationTime = File.GetCreationTime(Path.Combine(dirPath, filePath));
				//	string fileNm = Path.GetFileName(filePath);
				//	string fileId = fileNm.Substring(extractIndex, extractLen);

				//	if (processableTrigDtIdDict.ContainsKey(creationTime) == false)
				//	{
				//		processableTrigDtIdDict.Add(creationTime, fileId);
				//	}
				//}

				//return processableTrigDtIdDict.OrderByDescending(p => p.Key).Select(p => p.Value).ToList().First();
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static DateTime? GetDateFromFileNm(string fileNm, int dateIndex, int dateLen)
		{
			try
			{
				DateTime dt;
				string dateStr = Path.GetFileNameWithoutExtension(fileNm).Substring(dateIndex, dateLen);
				//if (DateTime.TryParse(dateStr, out dt) == false)
				if (DateTime.TryParseExact(dateStr, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt) == false)
				{
					return null;
				}
				return dt;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static string GetDateStrFromFileNm(string fileNm, string extStr, int dateIndex, int dateLen)
		{
			try
			{
				DateTime dt;
				string dateStr = Path.GetFileNameWithoutExtension(fileNm).Substring(dateIndex, dateLen);
				//if (DateTime.TryParse(dateStr, out dt) == false)
				if (DateTime.TryParseExact(dateStr, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt) == false)
				{
					return null;
				}
				return dateStr;
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// 指定した時間経過後の監視対象ディレクトリ内のファイル増加有無を返す
		/// </summary>
		/// <param name="watchDir">ファイル増加を監視するディレクトリ</param>
		/// <param name="chkTimeSpan">ファイル増加の監視間隔</param>
		/// <returns>True:増加あり/False:増加無しor減少</returns>
		public static bool IsFileOutputDone(string watchDir, int chkTimeSpan)
		{
			try
			{
				int fileCt = Directory.GetFiles(watchDir).Count();

				if (fileCt == 0)
				{
					return false;
				}

				Thread.Sleep(chkTimeSpan);

				if (fileCt >= Directory.GetFiles(watchDir).Count())
				{
					return true;
				}
				else
				{
					return false;
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// dirPath直下のextStrを拡張子に持つファイルが処理可能なファイルか返す
		/// </summary>
		/// <param name="dirPat"></param>
		/// <param name="extStr"></param>
		/// <returns></returns>
		protected static bool IsProcessableFile(string fileNm, bool isNeedAddedInfo)
		{
			try
			{
				if (fileNm.StartsWith(DESELECT_IDENTIFIER))
				{
					return false;
				}

				if (isNeedAddedInfo)
				{
					// ファイル名定義(20150706153423_FileKind_Lot_Type_Proc.[extStr])を想定して要素数で付与済か判断する 
					string[] nameChar = Path.GetFileNameWithoutExtension(fileNm).Split(FILE_NM_SPLITTER);
					if (nameChar.Count() < 5)
					{
						return false;
					}
					else { return true; }
				}
				else
				{
					return true;
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static void OutputResultFile(string dirPath, string fileNm, string msg, bool resultOK)
		{
			OutputResultFile(dirPath, fileNm, msg, resultOK, false, ERR_MSG_MAX_LEN);
		}

		public static void OutputResultFile(string dirPath, string fileNm, string msg, bool resultOK, int msgMaxLen)
		{
			OutputResultFile(dirPath, fileNm, msg, resultOK, false, msgMaxLen);
		}

		public static void OutputResultFile(string dirPath, string fileNm, string msg, bool resultOK, bool needLowerCase)
		{
			OutputResultFile(dirPath, fileNm, msg, resultOK, needLowerCase, ERR_MSG_MAX_LEN);
		}

		/// <summary>
		/// 指定フォルダにファイル名.拡張子の空ファイルを作成
		/// 拡張子はEXT_OK_FILE、EXT_NG_FILE
		/// </summary>
		/// <param name="dirPath"></param>
		/// <param name="fileNm"></param>
		/// <param name="resultOK"></param>
		public static void OutputResultFile(string dirPath, string fileNm, string msg, bool resultOK, bool needLowerCase, int msgMaxLen)
		{　//TDKのMD装置が大文字のOK、NGを受け付けないので切り替えられるように対応(2015/8/8 n.yoshimoto)
			try
			{
				//fileNm = Path.GetFileNameWithoutExtension(fileNm);

				Regex regex = new Regex("[.]$[^.]*", RegexOptions.IgnoreCase);

				fileNm = regex.Replace(Path.GetFileName(fileNm), string.Empty);

				if (resultOK)
				{
					string ext = EXT_OK_FILE;

					if (needLowerCase)
					{
						ext = ext.ToLower();
					}

					fileNm = string.Join(".", fileNm, ext);
				}
				else
				{
					string ext = EXT_NG_FILE;

					if (needLowerCase)
					{
						ext = ext.ToLower();
					}

					fileNm = string.Join(".", fileNm, ext);
				}

				string outputFileFullPath = Path.Combine(dirPath, fileNm);

				if (File.Exists(outputFileFullPath) == false)
				{
					using (StreamWriter textFile = File.CreateText(outputFileFullPath))
					{
						if (msg.Length > msgMaxLen)
						{
							msg = msg.Substring(0, msgMaxLen);
						}

						textFile.WriteLine(msg);
						textFile.Close();
					}
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
					string.Format("判定結果ファイル出力完了 : パス：{0}", outputFileFullPath));
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static void OutputStopFile(string dirPath, string fileNm, string msg, bool isOutputException)
		{
			OutputStopFile(dirPath, fileNm, msg, isOutputException, false, ERR_MSG_MAX_LEN);
		}

		public static void OutputStopFile(string dirPath, string fileNm, string msg, bool isOutputException, int msgMaxLen)
		{
			OutputStopFile(dirPath, fileNm, msg, isOutputException, false, msgMaxLen);
		}

		public static void OutputStopFile(string dirPath, string fileNm, string msg, bool isOutputException, bool needLowerCase)
		{
			OutputStopFile(dirPath, fileNm, msg, isOutputException, needLowerCase, ERR_MSG_MAX_LEN);
		}

		public static void OutputStopFile(string dirPath, string fileNm, string msg, bool isOutputException, bool needLowerCase, int msgMaxLen)
		{
			try
			{
				fileNm = Path.GetFileNameWithoutExtension(fileNm);

				string ext = EXT_STOP_FILE;

				if (needLowerCase)
				{
					ext = ext.ToLower();
				}

				fileNm = string.Join(".", fileNm, ext);

				string outputFileFullPath = Path.Combine(dirPath, fileNm);

				using (StreamWriter textFile = File.CreateText(outputFileFullPath))
				{
					if (msg.Length > msgMaxLen)
					{
						msg = msg.Substring(0, msgMaxLen);
					}

					textFile.WriteLine(msg);
					textFile.Close();
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
					string.Format("停止ファイル出力完了 : パス：{0}", outputFileFullPath));
			}
			catch (Exception err)
			{
				if (isOutputException)
				{
					throw;
				}
				else
				{
				}
			}
		}

		/// <summary>
		/// ファイル群を移動する。移動するファイルのファイル名は次のフォーマットを満たさなくてはならない。
		/// ①_②_③あるいは①_②_③_④
		/// ①：年月日が識別できる文字列　②：ファイル種類　③：ロットNO　④：タイプ
		/// 移動先のtoBaseDir下にキーファイル年月フォルダを作成し③からロットNOが分かればロットNo名のフォルダ、分からなければ日時分秒名のフォルダを作りその中にバックアップ
		/// </summary>
		/// <param name="fromPath"></param>
		/// <param name="toBaseDir"></param>
		public static void BackupDoneFiles(List<string> fromPath, string toBaseDir, string lotNo, DateTime dt)
		{
			try
			{
				string childDirNm;
				string yearDirNm = dt.ToString("yyyy");
				string monthDirNm = dt.ToString("MM");

				if ((string.IsNullOrEmpty(lotNo) || lotNo.ToUpper() == "UNKNOWN"))
				{
					childDirNm = dt.ToString("dd");
				}
				else
				{
					childDirNm = lotNo;
				}

				string backupDir = Path.Combine(toBaseDir, yearDirNm, monthDirNm, childDirNm);

				foreach (string filePath in fromPath)
				{
					if (File.Exists(filePath) == false)
					{
						continue;
					}

					if (Directory.Exists(backupDir) == false)
					{
						Directory.CreateDirectory(backupDir);
					}
					
					string destPath = Path.Combine(backupDir, Path.GetFileName(filePath));
					if (File.Exists(destPath))
					{
						File.Delete(destPath);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
							string.Format("[BackupDoneFiles] 移動先に同名ファイルが存在した為、移動先ファイルを削除。ファイルパス:{0}", destPath));

					}
					File.Move(filePath, destPath);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
							string.Format("[BackupDoneFiles] 移動完了 移動元:{0} 移動先:{1}", filePath, destPath));
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}

		public static void CopyDoneFiles(List<string> fromPath, string toBaseDir, string lotNo, DateTime dt)
		{
			try
			{
				string childDirNm;
				string yearMonthDirNm = dt.ToString("yyyyMM");

				if ((string.IsNullOrEmpty(lotNo) || lotNo.ToUpper() == "UNKNOWN"))
				{
					childDirNm = dt.ToString("dd-HH");
				}
				else
				{
					childDirNm = lotNo;
				}

				string backupDir = Path.Combine(toBaseDir, yearMonthDirNm, childDirNm);

				foreach (string filePath in fromPath)
				{
					if (Directory.Exists(backupDir) == false)
					{
						Directory.CreateDirectory(backupDir);
					}

					string destPath = Path.Combine(backupDir, Path.GetFileName(filePath));
					if (File.Exists(destPath))
					{
						File.Delete(destPath);
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
							string.Format("[BackupDoneFiles] コピー先に同名ファイルが存在した為、コピー先ファイルを削除。ファイルパス:{0}", destPath));

					}

					File.Copy(filePath, destPath);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO,
							string.Format("[BackupDoneFiles] コピー完了 移動元:{0} 移動先:{1}", filePath, destPath));
				}
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}
}
