using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LENS2_Api;
using System.Threading;
namespace LENS2
{
	public class MachineLog
	{
		//public const string ALL_MACHINELOG_DIR_NAME = "MachineLog";

		/// <summary>
		/// 履歴用ファイルをコピー作成
		/// </summary>
		private static void copyKeepRecord(string fromfilepath, string todirectorypath, string lotno, int retryct)
		{
			if (System.IO.Directory.Exists(fromfilepath))
			{
				throw new ApplicationException(string.Format("指定されたファイルパスは存在しません。FilePath:{0}", fromfilepath));
			}
			FileInfo file = new FileInfo(fromfilepath);

			string toDirectory = Path.Combine(todirectorypath, lotno);
			if (!System.IO.Directory.Exists(toDirectory))
			{
				System.IO.Directory.CreateDirectory(toDirectory);
			}

			try
			{
				file.CopyTo(Path.Combine(toDirectory, file.Name), true);
			}
			catch (IOException)
			{
				if (retryct == Config.Settings.FileAccessRetryCount)
				{
					throw new ApplicationException(string.Format("履歴用ファイルの作成に失敗しました。 ロット番号:{0} ファイル元:{1}", lotno, fromfilepath));
				}
				copyKeepRecord(fromfilepath, todirectorypath, lotno, retryct++);
			}
		}
		public static void CopyKeepRecord(string fromfilepath, string todirectorypath, string lotno)
		{
			copyKeepRecord(fromfilepath, todirectorypath, lotno, 0);
		}

		/// <summary>
		/// 指定フォルダのファイル中で最も更新時間（ファイル名中に記載の日時）が新しいファイルパスを取得
		/// </summary>
		/// <param name="folderPath"></param>
		/// <param name="startIndexFromMM">MM文字のインデックスから日時文字列までの文字数</param>
		/// <returns></returns>
		public static FileInfo GetLatest(string folderPath, string fileKind, int startIndexFromMM, int retryCt)
		{
			if (!System.IO.Directory.Exists(folderPath))
			{
				Machine.MachineBase.CommonHideLog(string.Format("フォルダが見つかりません。パス:{0}", folderPath));
				return null;
			}

			List<string> fileList = LENS2_Api.Directory.GetFiles(folderPath, fileKind + ".*");
			if (fileList.Count == 0)
			{
				if (retryCt >= 10)
				{
					return null;
				}
				else 
				{
					retryCt = retryCt + 1;
					Thread.Sleep(1000);
					return GetLatest(folderPath, fileKind, startIndexFromMM, retryCt);
				}
			}

			DateTime[] sortedCreateTime = new DateTime[fileList.Count];

			List<FileInfo> fileInfoList = new List<FileInfo>();

			// ファイル名から日付文字列取得
			foreach (string swfname in fileList)
			{
				fileInfoList.Add(new System.IO.FileInfo(swfname));
			}

			fileInfoList = fileInfoList.OrderByDescending(f => f.LastWriteTime).ToList();
			//// ファイル名からMMを検索して、最初に見つかったインデックスからの10文字(年月日日時)で配列並び替え
			//fileInfoList = fileInfoList.OrderByDescending(f => DateTime.ParseExact(f.Name.Substring(f.Name.IndexOf(fileKind) + startIndexFromMM, 10), "yyMMddHHmm", null)).ToList();
			//if (fileInfoList.Count == 0) 
			//{
			//	Machine.MachineBase.CommonHideLog(string.Format("ファイル名に年月日時分の情報を持つ{0}ファイルが見つかりません。フォルダパス：{1}", fileKind, folderPath));
			//	return null;
			//}

			// ファイル名中の時間が最も遅いファイルパスを取得
			return fileInfoList.First();
		}
		public static FileInfo GetLatest(string folderPath, string fileKind, int startIndexFromMM) 
		{
			return GetLatest(folderPath, fileKind, startIndexFromMM, 0);
		}

		protected string getMagazineQR(string[] fileValue, int magazineCol)
		{
			string retv = string.Empty;
			for (int targetMagRow = fileValue.Count() - 1; targetMagRow >= 0; targetMagRow--)
			{
				if (string.IsNullOrEmpty(fileValue[targetMagRow]))
				{//空白行の場合、手前の行をサーチする
					continue;
				}

				string[] fileValueLine = fileValue[targetMagRow].Split(',');

				retv = fileValueLine[magazineCol].Trim().Replace("\"", "");
                retv = retv.Replace("\r", "");

				if (string.IsNullOrWhiteSpace(retv))
				{//マガジン・ロットNo列がnullか空白なら手前の行をサーチする
					continue;
				}
				else
				{//何らかの文字列が入っていた場合、サーチ終了
					break;
				}
			}
			return retv;
		}

		///// <summary>
		///// 不必要ファイル(1番新しい更新日時のファイル以外)の移動
		///// ※内製機に限る
		///// </summary>
		///// <param name="topath"></param>
		//public static void MoveUnnecessary(string fromdirpath, string fileprefix) 
		//{
		//	if (System.IO.Directory.Exists(fromdirpath))
		//	{
		//		throw new ApplicationException(string.Format("指定されたディレクトリパスは存在しません。DirectoryPath:{0}", fromdirpath));
		//	}

		//	DirectoryInfo dir = new DirectoryInfo(fromdirpath);
		//	FileInfo[] files = dir.GetFiles("*" + fileprefix + "*");

		//	IEnumerable<FileInfo> unnecessaryFiles = files.OrderByDescending(f => f.LastWriteTime).Skip(1);
		//	foreach (FileInfo file in unnecessaryFiles)
		//	{
		//		file.MoveTo(Path.Combine(Path.Combine(file.DirectoryName, "reserve"), file.Name));
		//	}
		//}

		///// <summary>
		///// 最新ファイルの取得
		///// </summary>
		///// <param name="fromdirpath"></param>
		///// <param name="fileprefix"></param>
		//public static FileInfo GetLatest(string fromdirpath, string fileprefix) 
		//{
		//	if (!System.IO.Directory.Exists(fromdirpath))
		//	{
		//		throw new ApplicationException(string.Format("指定されたディレクトリパスは存在しません。DirectoryPath:{0}", fromdirpath));
		//	}

		//	DirectoryInfo dir = new DirectoryInfo(fromdirpath);
		//	FileInfo[] files = dir.GetFiles("*" + fileprefix + "*");
		//	if (files.Count() == 0)
		//	{
		//		return null;
		//	}

		//	FileInfo latestFile = files.OrderByDescending(f => f.LastWriteTime).First();
		//	return latestFile;
		//}

		///// <summary>
		///// 完了ファイルの移動(EICS引き渡し)
		///// </summary>
		//private static void moveCompleteFile(string fromfilepath, string lotno, int retryct)
		//{
		//	if (System.IO.Directory.Exists(fromfilepath))
		//	{
		//		throw new ApplicationException(string.Format("指定されたファイルパスは存在しません。FilePath:{0}", fromfilepath));
		//	}

		//	copyKeepRecordFile(fromfilepath, lotno);

		//	FileInfo file = new FileInfo(fromfilepath);

		//	string toDirectory = Path.Combine(file.DirectoryName, "Done");

		//	try
		//	{
		//		file.MoveTo(Path.Combine(toDirectory, file.Name));
		//	}
		//	catch (IOException)
		//	{
		//		if (retryct == Config.Settings.FileAccessRetryCount)
		//		{
		//			throw new ApplicationException(string.Format("完了ファイルの移動に失敗しました。 ロット番号:{0} ファイル元:{1}", lotno, fromfilepath));
		//		}
		//		moveCompleteFile(fromfilepath, lotno, retryct++);
		//	}
		//}
		//public static void MoveCompleteFile(string fromfilepath, string lotno) 
		//{
		//	moveCompleteFile(fromfilepath, lotno, 0);
		//}
	}
}
