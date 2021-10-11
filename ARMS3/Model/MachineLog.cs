using ArmsApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ARMS3.Model
{
	public class MachineLog
	{
		public const string FILE_UNKNOWN = "UNKNOWN";
        public const string FILE_OPENINGCHECK = "OPENINGCHECK";
		public const string MPD_FILE_EXT = "mpd";
        public const int FINISHED_RENAME_ELEMENT_NUM = 4;

		public string MagazineNo { get; set; }

		public bool IsUnknownData { get; set; }

        public bool IsOpeningCheckFile { get; set; }

        /// <summary>
        /// 装置ログ出力フォルダのファイルの内、最終更新日が新しい1つを返す
        /// ファイルが全く無い場合はnull
        /// </summary>
        /// <param name="mmFileDir"></param>
        /// <returns></returns>
        public static string GetNewestFile(string targetDirectoryPath, string prefix)
		{
			System.IO.DirectoryInfo dir = new System.IO.DirectoryInfo(targetDirectoryPath);
			if (dir.Exists == false)
			{
				throw new ApplicationException(string.Format("{0}ファイル保存ディレクトリが見つかりません:{1}", prefix, targetDirectoryPath));
			}

			System.IO.FileInfo[] files = dir.GetFiles();
			if (files.Count() == 0)
			{
				return null;
			}

			List<FileInfo> prefixFiles = new List<FileInfo>();
			foreach(FileInfo file in files)
			{
				string fileName = file.Name;
				if (file.Name.ToUpper().StartsWith("LOG")) 
				{
					fileName = string.Join("", file.Name.Skip(file.Name.IndexOf("_") + 1));
				}
				if (fileName.ToUpper().StartsWith(prefix) && file.Extension.ToUpper() == ".CSV")
				{
					prefixFiles.Add(file);
				}
			}
			if (prefixFiles.Count == 0)
			{
				return null;
			}

			return prefixFiles.OrderBy(p => p.CreationTime).Last().FullName;

			//[完了]2014.10.15 コードレビュー指摘
			//作成日時順に並び変える、ファイル名の先頭に"log"の文字があったら、1要素目は飛ばしてPrefix判定を行う。
			
			//System.IO.FileInfo top = files.Where(f => f.Name.ToUpper().StartsWith(prefix) && f.Extension.ToUpper() == ".CSV").FirstOrDefault();

			//if (top != null)
			//{
			//	return top.FullName;
			//}
			//else
			//{
			//	return null;
			//}
		}

		/// <summary>
		/// 装置ログファイル名から更新日付を取得
		/// 外観検査機、モールド機のみ使用可能(※ファイル名の定義が同じ装置ログ)
		/// </summary>
		/// <returns></returns>
		public static DateTime GetUpdateDate(string targetFilePath, int startIndex, int length) 
		{
			string fileName = Path.GetFileNameWithoutExtension(targetFilePath);
			string dateString = fileName.Substring(startIndex, length);

			DateTime updateDate;
			if (!DateTime.TryParse(dateString, out updateDate))
			{
				throw new ApplicationException(
					string.Format("ファイル名から更新日付の取得ができませんでした。ファイルパス:{0}", targetFilePath));
			}
			return updateDate;
		}

		//SLP2対応:n.yoshi
		/// <summary>
		/// ファイル名のアンダーバー以降を削除（SLP2で付き始めた_設備CDを削除する為の処理)
		/// </summary>
		/// <param name="filePath"></param>
		/// <returns>リネームを行った場合、リネーム後のファイルパスを返す。リネーム未実施の場合はnull</returns>
		public static string RemoveFileNmAfterUnderBar(string filePath)
		{
			string fileNm = Path.GetFileName(filePath);

			//MPD、FINファイルに付与されている"_設備CD"を除去する。他装置と同様にMPDはロットNo、マガジン、タイプ付与のリネームはしない。
			//σ・WBとSLP2・BBのmpdﾌｧｲﾙ出力仕様が違う為、先に検証済みのσ・WB仕様に合わせる n.yoshi
			if (!MachineLog.IsLotFromFileName(filePath) && fileNm.Contains('_'))
			{
				string fileExt = Path.GetExtension(filePath);
				string destFileNm = string.Format("{0}{1}", fileNm.Remove(fileNm.IndexOf('_')), fileExt);
				string destFilePath = Path.Combine(Path.GetDirectoryName(filePath), destFileNm);

                if (File.Exists(destFilePath))
                {
                    throw new ApplicationException(
                        string.Format("ﾌｧｲﾙﾘﾈｰﾑを試行しましたが、既に試行と同一名称のﾘﾈｰﾑ済みﾌｧｲﾙが存在する為、ﾘﾈｰﾑを行えませんでした。"
                    + "何れかのﾌｧｲﾙが異常な出力と考えられる為、確認しﾒﾝﾃﾅﾝｽして下さい。ﾘﾈｰﾑ対象ﾌｧｲﾙﾊﾟｽ：{0} / ﾘﾈｰﾑ後ﾌｧｲﾙﾊﾟｽ：{1}", filePath, destFilePath));
                }

				File.Move(filePath, destFilePath);
				Log.SysLog.Info(string.Format("[完了処理] ファイル名称変更 FileName:{0}⇒{1}", filePath, destFilePath));

				return destFilePath;
			}

			return null;
		}

		public static void ChangeFileName(string targetFilePath, string lotNo, string typeCd, int procNo) 
		{
			ChangeFileName(targetFilePath, lotNo, typeCd, procNo, string.Empty, 0, null);
		}
		public static void ChangeFileName(string targetFilePath, string lotNo, string typeCd, int procNo, string magazineNo) 
		{
			ChangeFileName(targetFilePath, lotNo, typeCd, procNo, magazineNo, 0, null);
		}
		public static void ChangeFileName(string targetFilePath, string lotNo, string typeCd, int procNo, string magazineNo, string changeFileNameHeader)
		{
			ChangeFileName(targetFilePath, lotNo, typeCd, procNo, magazineNo, 0, changeFileNameHeader);
		}
        public static void ChangeFileNameCarrier(string targetFilePath, string lotNo, string typeCd, int procNo, string magazineNo, string carrieNo)
        {
            ChangeFileName(targetFilePath, lotNo, typeCd, procNo, magazineNo, 0, null);
        }
		public static void ChangeFileName(string targetFilePath, string lotNo, string typeCd, int procNo, string magazineNo, int retryCt, string changeFileNameHeader) 
		{
			if (retryCt >= 5)
			{
				throw new ApplicationException(
					string.Format("ファイルのリネームに失敗しました。ファイルが編集可能か確認して下さい。ファイルパス:{0}", targetFilePath));
			}

			if (!File.Exists(targetFilePath)) 
			{
				throw new ApplicationException(
					string.Format("存在しないファイルにロット情報を付与しようとしました。ファイルパス:{0}", targetFilePath));
			}
			FileInfo file = new FileInfo(targetFilePath);

			if (IsLotFromFileName(file.FullName)) 
			{
				//既に付与済みの場合は何もしない
				return;
			}

			string fileNameHeader = Path.GetFileNameWithoutExtension(targetFilePath);
			if (string.IsNullOrWhiteSpace(changeFileNameHeader) == false) 
			{
				fileNameHeader = changeFileNameHeader;			
			}

			string fileName = string.Empty;
			if (string.IsNullOrEmpty(magazineNo))
			{
				fileName = string.Format("{0}_{1}_{2}_{3}{4}", fileNameHeader, lotNo, typeCd, procNo, file.Extension);
			}
			else 
			{
				fileName = string.Format("{0}_{1}_{2}_{3}_{4}{5}", fileNameHeader, lotNo, typeCd, procNo, magazineNo, file.Extension);
			}

			try
			{
				file.MoveTo(Path.Combine(file.DirectoryName, fileName));
			}
			catch(IOException)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				ChangeFileName(targetFilePath, lotNo, typeCd, procNo, magazineNo, retryCt, changeFileNameHeader);
				return;
			}
		}

		/// <summary>
		/// 正規表現で指定したパス下のファイル一覧を取得する。
		/// extentionSearchPatternには拡張子の.(ピリオド)は含む必要無し
		/// Directory.GetFiles()ではファイル名に任意の文字を含むファイル一覧の取得が出来ない為、自作
		/// </summary>
		/// <param name="path">Directory.GetFiles()のpathと同じ</param>
		/// <param name="fileNameSearchPattern">ファイル名部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <param name="extentionSearchPattern">拡張子部分における正規表現パターン(Regexで使用出来る正規表現)</param>
        /// <param name="retryCt">リトライ回数</param>
		/// <returns></returns>
        public static List<string> GetFiles(string path, string fileNameSearchPattern, string extentionSearchPattern, int retryCt)
		{
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                throw new ApplicationException(string.Format("該当ファイルが存在しません。リトライ回数が上限に達しました。検索フォルダ:{0} 検索条件:{1}"
                    , path, fileNameSearchPattern + "." + extentionSearchPattern));
            }

			string[] pathArray = Directory.GetFiles(path, "*.*");

			Regex regex = new Regex(fileNameSearchPattern + "[.]" + extentionSearchPattern);

            List<string> retv = pathArray.Where(p => regex.IsMatch(p)).ToList();

            if (retv.Count == 0)
            {
                Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return GetFiles(path, fileNameSearchPattern, extentionSearchPattern, retryCt);
            }

            return retv;
		}

		/// <summary>
		/// 正規表現で指定したパス下のファイル一覧を取得する。
		/// .は任意の一文字 *は手前の文字の0文字以上の +は手前の文字の1文字以上の、?は手前の文字の0または1文字以上の繰り返し
		/// Directory.GetFiles()ではファイル名に任意の文字を含むファイル一覧の取得が出来ない為、自作
		/// </summary>
		/// <param name="path">Directory.GetFiles()のpathと同じ</param>
		/// <param name="fileNameSearchPattern">ファイル名部分における正規表現パターン(Regexで使用出来る正規表現)</param>
		/// <param name="extentionSearchPattern">拡張子部分における正規表現パターン(Regexで使用出来る正規表現)</param>
        /// <param name="retryCt">リトライ回数</param>
		/// <returns></returns>
        public static List<string> GetFiles(string path, string searchPattern, int retryCt)
		{
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                throw new ApplicationException(string.Format("該当ファイルが存在しません。リトライ回数が上限に達しました。検索フォルダ:{0} 検索条件:{1}"
                    , path, searchPattern));
            }

			if (Directory.Exists(path) == false) 
			{
				Directory.CreateDirectory(path);
			}

			string[] pathArray = Directory.GetFiles(path, "*.*");

			Regex regex = new Regex(searchPattern);

            List<string> retv = pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();

            if (retv.Count == 0)
            {
                Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return GetFiles(path, searchPattern, retryCt);
            }

            return retv;
		}
        
        /// <summary>
        /// 正規表現で指定したパス下のファイル一覧を取得する。(リトライなし)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<string> GetFilesByBackwardMatching(string path, string searchPattern)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string[] pathArray = Directory.GetFiles(path, "*.*");

            Regex regex = new Regex("^.*." + searchPattern + "$");

            return pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
        }

        /// <summary>
        /// 正規表現で指定したパス下のファイル一覧を取得する。(リトライなし)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<string> GetFiles(string path, string searchPattern)
        {
            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string[] pathArray = Directory.GetFiles(path, "*.*");

            Regex regex = new Regex(searchPattern);

            return pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
        }

        public static List<string> GetFiles(string path)
		{
			if (Directory.Exists(path) == false)
			{
				Directory.CreateDirectory(path);
			}

			string[] pathArray = Directory.GetFiles(path, "*.*");
			return pathArray.ToList();
		}

        /// <summary>
        /// ファイル名変更済確認(ロット + タイプ + 工程NO + マガジンNO付与)
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static bool IsLotFromFileName(string filePath)
        {
            // ファイル名定義(MM***_Lot_Type_Proc_MagazineNo.CSV)を想定して要素数で付与済か判断する 
            string[] nameChar = Path.GetFileNameWithoutExtension(filePath).Split('_');
            if (nameChar.Count() < 5)
            {
                return false;
            }
            else { return true; }
        }

        /// <summary>
        /// 正規表現で指定したパス下のファイル一覧を取得する。(日付順に並べ替える。リトライなし)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<string> GetFilesOrderByLastAccessTime_NotRetry(string path, string searchPattern)
        {
            Dictionary<string, DateTime> retv = new Dictionary<string, DateTime>();

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string[] pathArray = Directory.GetFiles(path, "*.*");

            Regex regex = new Regex("^.*." + searchPattern + "$");

            List<string> files = pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
            foreach (string file in files)
            {
                DateTime outputDate = File.GetLastAccessTime(file);
                retv.Add(file, outputDate);
            }

            return retv.OrderBy(r => r.Value).Select(r => r.Key).ToList();
        }

        /// <summary>
        /// 正規表現で指定したパス下のファイル一覧を取得する。(日付降順に並べ替える。リトライなし)
        /// </summary>
        /// <param name="path"></param>
        /// <param name="searchPattern"></param>
        /// <returns></returns>
        public static List<string> GetFilesOrderByDescendingLastAccessTime_NotRetry(string path, string searchPattern)
        {
            Dictionary<string, DateTime> retv = new Dictionary<string, DateTime>();

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string[] pathArray = Directory.GetFiles(path, "*.*");

            Regex regex = new Regex("^.*." + searchPattern + "$");

            List<string> files = pathArray.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();
            foreach (string file in files)
            {
                DateTime outputDate = File.GetLastAccessTime(file);
                retv.Add(file, outputDate);
            }

            return retv.OrderByDescending(r => r.Value).Select(r => r.Key).ToList();
        }


        #region ※CIFS専用

        public class FinishedFile
		{
			public const string FINISHEDFILE_IDENTITYNAME = "fin";

			public DateTime? WorkStartDt { get; set; }
			public DateTime? WorkEndDt { get; set; }
			public DateTime? LastUpdDt { get; set; }
			public string FullName { get; set; }

			public static FinishedFile GetLastFile(string directoryPath)
			{
				List<string> finFiles = GetFiles(directoryPath, FINISHEDFILE_IDENTITYNAME, 0);
				if (finFiles.Count == 0)
				{
					return null;
				}

				IOrderedEnumerable<string> descfinFiles = finFiles.OrderByDescending(f => File.GetLastWriteTime(f));

				FinishedFile fin = new FinishedFile();

				fin.FullName = descfinFiles.First();
				string[] content = File.ReadAllLines(fin.FullName);

				fin.LastUpdDt = File.GetLastWriteTime(fin.FullName);

				DateTime startDt; DateTime endDt;

				string fileValue = content[1].Split(',')[1];
                string date = fileValue.Substring(0, 4) + "/" + fileValue.Substring(4, 2) + "/" + fileValue.Substring(6, 2) + " " +
                    fileValue.Substring(8, 2) + ":" + fileValue.Substring(10, 2) + ":" + fileValue.Substring(12, 2);
				if (DateTime.TryParse(date, out startDt) == false)
				{
					throw new ApplicationException(string.Format("finファイル内の開始日時文字が不正です。 読込文字:{0}", fileValue));
				}
				fin.WorkStartDt = startDt;

				fileValue = content[3].Split(',')[1];
                date = fileValue.Substring(0, 4) + "/" + fileValue.Substring(4, 2) + "/" + fileValue.Substring(6, 2) + " " +
                    fileValue.Substring(8, 2) + ":" + fileValue.Substring(10, 2) + ":" + fileValue.Substring(12, 2);
				if (DateTime.TryParse(date, out endDt) == false)
				{
					throw new ApplicationException(string.Format("finファイル内の完了日時文字が不正です。 読込文字:{0}", fileValue));
				}
				fin.WorkEndDt = endDt;

				return fin;
			}

            public static List<FinishedFile> GetAllFiles(string directoryPath) 
            {
                List<FinishedFile> retv = new List<FinishedFile>();

                List<string> finFiles = GetFilesByBackwardMatching(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count == 0)
                {
                    return null;
                }
            
                foreach(string file in finFiles)
                {
                    if (MachineLog.IsLotFromFileName(file)) continue;

                    FinishedFile fin = new FinishedFile();

				    fin.FullName = file;
				    string[] content = File.ReadAllLines(fin.FullName);

					fin.LastUpdDt = File.GetLastWriteTime(fin.FullName);

				    DateTime startDt; DateTime endDt;

					try
					{
						string fileValue = content[1].Split(',')[1];
						if (string.IsNullOrWhiteSpace(fileValue))
						{
							fin.WorkStartDt = null;
						}
						else
						{
							string date = fileValue.Substring(0, 4) + "/" + fileValue.Substring(4, 2) + "/" + fileValue.Substring(6, 2) + " " +
								fileValue.Substring(8, 2) + ":" + fileValue.Substring(10, 2) + ":" + fileValue.Substring(12, 2);
							if (DateTime.TryParse(date, out startDt) == false)
							{
								throw new ApplicationException(string.Format("finファイル内の開始日時文字が不正です。 読込文字:{0}", fileValue));
							}
							fin.WorkStartDt = startDt;
						}
					}
					catch(Exception err)
					{
						Log.SysLog.Info(string.Format("開始日時が取得できなかった為、NULL値を代入しました。 エラーメッセージ:{0}", err.Message));
						fin.WorkStartDt = null;
					}

					try
					{
						string fileValue = content[3].Split(',')[1];
						if (string.IsNullOrWhiteSpace(fileValue))
						{
							fin.WorkEndDt = null;
						}
						else
						{
							string date = fileValue.Substring(0, 4) + "/" + fileValue.Substring(4, 2) + "/" + fileValue.Substring(6, 2) + " " +
								fileValue.Substring(8, 2) + ":" + fileValue.Substring(10, 2) + ":" + fileValue.Substring(12, 2);
							if (DateTime.TryParse(date, out endDt) == false)
							{
								throw new ApplicationException(string.Format("finファイル内の完了日時文字が不正です。 読込文字:{0}", fileValue));
							}
							fin.WorkEndDt = endDt;
						}
					}
					catch (Exception err)
					{
						Log.SysLog.Info(string.Format("完了日時が取得できなかった為、NULL値を代入しました。 エラーメッセージ:{0}", err.Message));
						fin.WorkEndDt = null;
					}

                    retv.Add(fin);
                }

				return retv;
            }
		}

		#endregion

        #region ※CIFS専用(fin2) 完了

        public class FinishedFile6
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin2";

            public string FullName { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public static List<FinishedFile6> GetAllFiles(string directoryPath)
            {
                List<FinishedFile6> retv = new List<FinishedFile6>();

                List<string> finFiles = GetFilesByBackwardMatching(directoryPath, FINISHEDFILE_IDENTITYNAME);

                foreach (string file in finFiles)
                {
                    string carrierNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                    if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                    }

                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    FinishedFile6 fin = new FinishedFile6();

                    fin.CarrierNo = carrierNo;
                    fin.NascaLotNo = lotCarrier.LotNo;
                    fin.MagNo = mag.MagazineNo;
                    fin.FullName = file;

                    retv.Add(fin);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(trg) 開始

        public class TriggerFile
        {
            public const string TRIGGERFILE_IDENTITYNAME = "trg";

            public string FullName { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }
            public DateTime LastUpdDt { get; set; }

            public static List<TriggerFile> GetAllFiles(string directoryPath)
            {
                List<TriggerFile> retv = new List<TriggerFile>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string carrierNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                    if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                    }

                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    TriggerFile trg = new TriggerFile();

                    trg.CarrierNo = carrierNo;
                    trg.NascaLotNo = lotCarrier.LotNo;
                    trg.MagNo = mag.MagazineNo;
                    trg.FullName = file;
                    trg.LastUpdDt = File.GetLastWriteTime(file);

                    retv.Add(trg);
                }

                return retv;
            }


        }

        #endregion

        #region ※CIFS専用(fin・wed) 完了

        public class FinishedFile2
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin";
            public const string WORKENDFILE_IDENTITYNAME = "wed";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }
            public string FullName_Fin { get; set; }
            public string FullName_Wed { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public static List<FinishedFile2> GetAllFiles(string directoryPath)
            {
                List<FinishedFile2> retv = new List<FinishedFile2>();

                List<string> finFiles = GetFilesOrderByLastAccessTime_NotRetry(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count != 0)
                {
                    List<string> wedFiles = GetFiles(directoryPath, WORKENDFILE_IDENTITYNAME, 0);
                    if (wedFiles.Count != 0)
                    {
                        foreach (string file in finFiles)
                        {
                            string carrierNo = Path.GetFileNameWithoutExtension(file);

                            //リネイム済みは除外
                            if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                            {
                                continue;
                            }

                            string wedFullName = getWedFile(wedFiles, carrierNo, directoryPath);

                            ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                            if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                            {
                                throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                            }

                            ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                            if (mags.Count() == 0)
                            {
                                throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                            }
                            ArmsApi.Model.Magazine mag = mags.Single();

                            FinishedFile2 fin = new FinishedFile2();

                            fin.CarrierNo = carrierNo;
                            fin.NascaLotNo = lotCarrier.LotNo;
                            fin.MagNo = mag.MagazineNo;
                            fin.FullName_Fin = file;
                            fin.FullName_Wed = wedFullName;
                            string[] content = File.ReadAllLines(fin.FullName_Fin);

                            DateTime? startDt = null;
                            DateTime? endDt = null;

                            // ファイルの中身を1行ずつ確認
                            foreach (string s in content)
                            {
                                string[] fileData = s.Split(',');
                                if (fileData.Length <= 1)
                                {
                                    continue;
                                }

                                // 開始時刻
                                if (fileData[0] == FIN_FILE_START)
                                {
                                    startDt = getDate(fileData[1].Trim(), "開始日時");
                                }
                                // 完了時刻
                                if (fileData[0] == FIN_FILE_END)
                                {
                                    endDt = getDate(fileData[1].Trim(), "完了日時");
                                }
                            }

                            fin.WorkStartDt = startDt;
                            fin.WorkEndDt = endDt;

                            retv.Add(fin);
                        }
                    }
                }

                return retv;
            }

            /// <summary>
            /// 対象のwedファイル取得
            /// </summary>
            /// <param name="wedFiles"></param>
            /// <param name="carrierNo"></param>
            /// <param name="directoryPath"></param>
            /// <returns></returns>
            private static string getWedFile(List<string> wedFiles, string carrierNo, string directoryPath)
            {
                string searchPattern = carrierNo + "[.]" + WORKENDFILE_IDENTITYNAME;
                Regex regex = new Regex(searchPattern);

                List<string> list = wedFiles.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();

                if (list.Count != 0)
                {
                    return list[0];
                }
                else
                {
                    throw new ApplicationException(string.Format("傾向管理ファイルが見つかりませんでした。 検索フォルダ:{0} キャリア番号:{1}", directoryPath, carrierNo));
                }
            }
        }

        #endregion

        /// <summary>
        /// 日時取得
        /// </summary>
        /// <param name="fileValue"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        private static DateTime? getDate(string fileValue, string msg)
        {
            DateTime? retv = null;

            try
            {
                if (string.IsNullOrWhiteSpace(fileValue))
                {
                    retv = null;
                }
                else
                {
                    DateTime dt;

                    string date = fileValue.Substring(0, 4) + "/" + fileValue.Substring(4, 2) + "/" + fileValue.Substring(6, 2) + " " +
                        fileValue.Substring(8, 2) + ":" + fileValue.Substring(10, 2) + ":" + fileValue.Substring(12, 2);
                    if (DateTime.TryParse(date, out dt) == false)
                    {
                        throw new ApplicationException(string.Format("finファイル内の{0}文字が不正です。 読込文字:{1}", msg, fileValue));
                    }
                    retv = dt;
                }
            }
            catch (Exception err)
            {
                Log.SysLog.Info(string.Format("{0}が取得できなかった為、NULL値を代入しました。 エラーメッセージ:{1}", msg, err.Message));
                retv = null;
            }

            return retv;
        }

        public static DateTime? GetDateTimeFromFileName(string filename)
        {
            return getDate(filename, "更新日時");
        }

        #region ※CIFS専用(ダイボンダー AD8380) 完了

        public class FinishedFile3
        {
            public const string FINISHEDFILE_IDENTITYNAME = "out";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";

            public string FullName { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }

            public static List<FinishedFile3> GetAllFiles(string directoryPath)
            {
                List<FinishedFile3> retv = new List<FinishedFile3>();

                List<string> outFiles = GetFilesByBackwardMatching(directoryPath, FINISHEDFILE_IDENTITYNAME);

                foreach (string file in outFiles)
                {
                    string carrierNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                    if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                    }

                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    FinishedFile3 fin = new FinishedFile3();
                    fin.CarrierNo = carrierNo;
                    fin.NascaLotNo = lotCarrier.LotNo;
                    fin.MagNo = mag.MagazineNo;
                    fin.FullName = file;

                    string[] content = File.ReadAllLines(fin.FullName);
                    DateTime? startDt = null;
                    DateTime? endDt = null;

                    // ファイルの中身を1行ずつ確認
                    foreach (string s in content)
                    {
                        string[] fileData = s.Split(',');
                        if (fileData.Length <= 1)
                        {
                            continue;
                        }

                        // 開始時刻
                        if (fileData[0] == FIN_FILE_START)
                        {
                            startDt = getDate(fileData[1].Trim(), "開始日時");
                        }
                        // 完了時刻
                        if (fileData[0] == FIN_FILE_END)
                        {
                            endDt = getDate(fileData[1].Trim(), "完了日時");
                        }
                    }

                    fin.WorkStartDt = startDt;
                    fin.WorkEndDt = endDt;

                    retv.Add(fin);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(ダイボンダー AD838L) 開始

        public class TriggerFile2
        {
            public const string TRIGGERFILE_IDENTITYNAME = "DM";

            public string FullName { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public static List<TriggerFile2> GetAllFiles(string directoryPath)
            {
                List<TriggerFile2> retv = new List<TriggerFile2>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string carrierNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                    if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                    }

                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    TriggerFile2 trg = new TriggerFile2();

                    trg.CarrierNo = carrierNo;
                    trg.NascaLotNo = lotCarrier.LotNo;
                    trg.MagNo = mag.MagazineNo;
                    trg.FullName = file;

                    retv.Add(trg);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(fin・csv) 完了

        public class FinishedFile4
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin";
            public const string CSVFILE_IDENTITYNAME = "csv";
            public const string CSV_DATAMATRIX = "Board ID";
            public const string CSV_MATLOT = "Parts Lot ID";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }
            public string FullName_Fin { get; set; }
            public string FullName_Csv { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public static List<FinishedFile4> GetAllFiles(string directoryPath, string csvDirectoryPath)
            {
                List<FinishedFile4> retv = new List<FinishedFile4>();

                List<string> finFiles = GetFilesByBackwardMatching(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count != 0)
                {
                    List<string> csvFiles = GetFiles(csvDirectoryPath, CSVFILE_IDENTITYNAME, 0);
                    if (csvFiles.Count != 0)
                    {
                        foreach (string file in finFiles)
                        {
                            string carrierNo = Path.GetFileNameWithoutExtension(file);

                            //リネイム済みは除外
                            if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                            {
                                continue;
                            }

                            string csvFullName = getCsvFile(csvFiles, carrierNo, directoryPath);

                            ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                            if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                            {
                                throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                            }

                            ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                            if (mags.Count() == 0)
                            {
                                throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                            }
                            ArmsApi.Model.Magazine mag = mags.Single();

                            FinishedFile4 fin = new FinishedFile4();

                            fin.CarrierNo = carrierNo;
                            fin.NascaLotNo = lotCarrier.LotNo;
                            fin.MagNo = mag.MagazineNo;
                            fin.FullName_Fin = file;
                            fin.FullName_Csv = csvFullName;
                            string[] content = File.ReadAllLines(fin.FullName_Fin);

                            DateTime? startDt = null;
                            DateTime? endDt = null;

                            // ファイルの中身を1行ずつ確認
                            foreach (string s in content)
                            {
                                string[] fileData = s.Split(',');
                                if (fileData.Length <= 1)
                                {
                                    continue;
                                }

                                // 開始時刻
                                if (fileData[0] == FIN_FILE_START)
                                {
                                    startDt = getDate(fileData[1].Trim(), "開始日時");
                                }
                                // 完了時刻
                                if (fileData[0] == FIN_FILE_END)
                                {
                                    endDt = getDate(fileData[1].Trim(), "完了日時");
                                }
                            }

                            fin.WorkStartDt = startDt;
                            fin.WorkEndDt = endDt;

                            retv.Add(fin);
                        }
                    }
                }

                return retv;
            }

            private static string getCsvFile(List<string> csvFiles, string carrierNo, string directoryPath)
            {
                foreach (string csvFile in csvFiles)
                {
                    string[] contents = File.ReadAllLines(csvFile);

                    // ファイルの中身を1行ずつ確認
                    foreach (string s in contents)
                    {
                        string[] fileData = s.Split(',');
                        if (fileData.Length <= 1)
                        {
                            continue;
                        }

                        if (fileData[0] == CSV_DATAMATRIX)
                        {
                            string datamatrix = fileData[1].Trim();

                            if (carrierNo == datamatrix)
                            {
                                return csvFile;
                            }
                        }
                    }
                }

                throw new ApplicationException(string.Format("トレーサビリティファイルが見つかりませんでした。 検索フォルダ:{0} 基板DM:{1}", directoryPath, carrierNo));
            }
        }

        #endregion

        #region ※CIFS専用(mtr) 開始

        public class TriggerFile3
        {
            public const string TRIGGERFILE_IDENTITYNAME = "mtr";

            public string FullName { get; set; }
            public string[] FileData { get; set; }

            public static List<TriggerFile3> GetAllFiles(string directoryPath)
            {
                List<TriggerFile3> retv = new List<TriggerFile3>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string[] contents = File.ReadAllLines(file);

                    TriggerFile3 trg = new TriggerFile3();

                    trg.FullName = file;
                    trg.FileData = contents;

                    retv.Add(trg);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(fin) 完了

        public class FinishedFile5
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";

            public string FullName { get; set; }
            public string DataMatrix { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }

			public static List<FinishedFile5> GetAllFiles(string directoryPath)
			{
				List<string> finFiles = GetFilesByBackwardMatching(directoryPath, FINISHEDFILE_IDENTITYNAME);
				return GetAllFiles(finFiles);
			}

            public static List<FinishedFile5> GetAllFiles(List<string> finFiles)
            {
                List<FinishedFile5> retv = new List<FinishedFile5>();

                foreach (string file in finFiles)
                {
                    string dataMatrix = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (dataMatrix.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(dataMatrix, true);
                    if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 基板DM:{0}", dataMatrix));
                    }

                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    FinishedFile5 fin = new FinishedFile5();

                    fin.DataMatrix = dataMatrix;
                    fin.NascaLotNo = lotCarrier.LotNo;
                    fin.MagNo = mag.MagazineNo;
                    fin.FullName = file;

                    string[] content = File.ReadAllLines(fin.FullName);
                    DateTime? startDt = null;
                    DateTime? endDt = null;

                    // ファイルの中身を1行ずつ確認
                    foreach (string s in content)
                    {
                        string[] fileData = s.Split(',');
                        if (fileData.Length <= 1)
                        {
                            continue;
                        }

                        // 開始時刻
                        if (fileData[0] == FIN_FILE_START)
                        {
                            startDt = getDate(fileData[1].Trim(), "開始日時");
                        }
                        // 完了時刻
                        if (fileData[0] == FIN_FILE_END)
                        {
                            endDt = getDate(fileData[1].Trim(), "完了日時");
                        }
                    }

                    fin.WorkStartDt = startDt;
                    fin.WorkEndDt = endDt;

                    retv.Add(fin);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(レンズ実装) 開始

        public class TriggerFile4
        {
            /// <summary>
            /// TODO 2016.04.07 ファイルの拡張子未定
            /// </summary>
            public const string TRIGGERFILE_IDENTITYNAME = "???";

            public string FullName { get; set; }
            public string TrayDataMatrix { get; set; }
            public string NascaLotNo { get; set; }
            public string MaterialCd { get; set; }

            public static List<TriggerFile4> GetAllFiles(string directoryPath)
            {
                List<TriggerFile4> retv = new List<TriggerFile4>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string trayDM = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (trayDM.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    string lotNo = ArmsApi.Model.LotTrayDataMatrix.GetLotNo(trayDM, true);
                    if (string.IsNullOrWhiteSpace(lotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 トレイDM:{0}", trayDM));
                    }

                    TriggerFile4 trg = new TriggerFile4();

                    trg.FullName = file;
                    trg.TrayDataMatrix = trayDM;
                    trg.NascaLotNo = lotNo;

                    retv.Add(trg);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(trg) 開始

        public class TriggerFile5
        {
            public const string TRIGGERFILE_IDENTITYNAME = "trg";

            public string FullName { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }
            public DateTime LastUpdDt { get; set; }

            public static List<TriggerFile> GetAllFiles(string directoryPath)
            {
                List<TriggerFile> retv = new List<TriggerFile>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string carrierNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                    if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                    }

                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotCarrier.LotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotCarrier.LotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    TriggerFile trg = new TriggerFile();

                    trg.CarrierNo = carrierNo;
                    trg.NascaLotNo = lotCarrier.LotNo;
                    trg.MagNo = mag.MagazineNo;
                    trg.FullName = file;
                    trg.LastUpdDt = File.GetLastWriteTime(file);

                    retv.Add(trg);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(trg) 開始 (TnCassette, TnLotCarrier両テーブルに紐付が無いか確認)

        public class TriggerFile6
        {
            public const string TRIGGERFILE_IDENTITYNAME = "trg";

            public string FullName { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }
            public DateTime LastUpdDt { get; set; }

            public static List<TriggerFile6> GetAllFiles(string directoryPath)
            {
                List<TriggerFile6> retv = new List<TriggerFile6>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string carrierNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    string lotNo = ArmsApi.Model.LotCarrier.GetLotNo(carrierNo, null, true, false).ToList().FirstOrDefault();
                    if (string.IsNullOrWhiteSpace(lotNo))
                    {
                        lotNo = ArmsApi.Model.LotCarrier.GetLotNoFromRingNo(carrierNo, false);
                    }

                    if (string.IsNullOrWhiteSpace(lotNo))
                    {
                        throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                    }
                    ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotNo, true);
                    if (mags.Count() == 0)
                    {
                        throw new ApplicationException("マガジンが見つかりません:" + lotNo);
                    }
                    ArmsApi.Model.Magazine mag = mags.Single();

                    TriggerFile6 trg = new TriggerFile6();

                    trg.CarrierNo = carrierNo;
                    trg.NascaLotNo = lotNo;
                    trg.MagNo = mag.MagazineNo;
                    trg.FullName = file;
                    trg.LastUpdDt = File.GetLastWriteTime(file);

                    retv.Add(trg);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(trg) 開始 (マガジンQR読込パターン)

        public class TriggerFile7
        {
            public const string TRIGGERFILE_IDENTITYNAME = "trg";

            public string FullName { get; set; }
            public ArmsApi.Model.AsmLot Lot { get; set; }
            public ArmsApi.Model.Magazine Mag { get; set; }
            public DateTime LastUpdDt { get; set; }

            public static List<TriggerFile7> GetAllFiles(string directoryPath)
            {
                List<TriggerFile7> retv = new List<TriggerFile7>();

                List<string> files = GetFilesByBackwardMatching(directoryPath, TRIGGERFILE_IDENTITYNAME);

                foreach (string file in files)
                {
                    string magNo = Path.GetFileNameWithoutExtension(file);

                    //リネイム済みは除外
                    if (magNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                    {
                        continue;
                    }

                    string[] elmMag = magNo.Split(' ');
                    if(elmMag.Count() > 1)
                    {
                        magNo = elmMag[1];
                    }

                    ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magNo);
                    if (mag == null)
                    {
                        throw new ApplicationException($"稼働中マガジンが見つかりませんでした。マガジンNo：『{magNo}』");
                    }

                    ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(mag.NascaLotNO);

                    if (lot == null)
                    {
                        throw new ApplicationException($"ロット情報の取得に失敗しました。マガジンNo：『{magNo}』");
                    }

                    TriggerFile7 trg = new TriggerFile7();
                    
                    trg.Lot = lot;
                    trg.Mag = mag;
                    trg.FullName = file;
                    trg.LastUpdDt = File.GetLastWriteTime(file);

                    retv.Add(trg);
                }

                return retv;
            }
        }

        #endregion

        #region ※CIFS専用(fin・wed) 完了

        public class FinishedFile7
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin";
            public const string WORKENDFILE_IDENTITYNAME = "wed";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }
            public string FullName_Fin { get; set; }
            public string FullName_Wed { get; set; }
            public string NascaLotNo { get; set; }
            public string TypeCd { get; set; }
            public string MagNo { get; set; }

            public static List<FinishedFile7> GetAllFiles(string directoryPath, string magazineNo)
            {
                List<FinishedFile7> retv = new List<FinishedFile7>();

                List<string> finFiles = GetFilesOrderByLastAccessTime_NotRetry(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count != 0)
                {
                    List<string> wedFiles = GetFiles(directoryPath, WORKENDFILE_IDENTITYNAME, 0);
                    if (wedFiles.Count != 0)
                    {
                        foreach (string file in finFiles)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(file);

                            //リネイム済みは除外
                            if (fileName.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                            {
                                continue;
                            }

                            string wedFullName = getWedFile(wedFiles, fileName, directoryPath);

                            ArmsApi.Model.Magazine svrmag = ArmsApi.Model.Magazine.GetCurrent(magazineNo);
                            if (svrmag == null) throw new ApplicationException("ArmsSvr マガジン情報が見つかりません:" + magazineNo);
                            ArmsApi.Model.AsmLot svrlot = ArmsApi.Model.AsmLot.GetAsmLot(svrmag.NascaLotNO);
                            if (svrlot == null) throw new ApplicationException("ArmsSvr ロット情報が見つかりません:" + svrmag.NascaLotNO);

                            FinishedFile7 fin = new FinishedFile7();

                            fin.NascaLotNo = svrlot.NascaLotNo;
                            fin.TypeCd = svrlot.TypeCd;
                            fin.MagNo = magazineNo;
                            fin.FullName_Fin = file;
                            fin.FullName_Wed = wedFullName;
                            string[] content = File.ReadAllLines(fin.FullName_Fin);

                            DateTime? startDt = null;
                            DateTime? endDt = null;

                            // ファイルの中身を1行ずつ確認
                            foreach (string s in content)
                            {
                                string[] fileData = s.Split(',');
                                if (fileData.Length <= 1)
                                {
                                    continue;
                                }

                                // 開始時刻
                                if (fileData[0] == FIN_FILE_START)
                                {
                                    startDt = getDate(fileData[1].Trim(), "開始日時");
                                }
                                // 完了時刻
                                if (fileData[0] == FIN_FILE_END)
                                {
                                    endDt = getDate(fileData[1].Trim(), "完了日時");
                                }
                            }

                            fin.WorkStartDt = startDt;
                            fin.WorkEndDt = endDt;

                            retv.Add(fin);
                        }
                    }
                }

                return retv;
            }

            /// <summary>
            /// 対象のwedファイル取得
            /// </summary>
            /// <param name="wedFiles"></param>
            /// <param name="carrierNo"></param>
            /// <param name="directoryPath"></param>
            /// <returns></returns>
            private static string getWedFile(List<string> wedFiles, string fileNm, string directoryPath)
            {
                string searchPattern = fileNm + "[.]" + WORKENDFILE_IDENTITYNAME;
                Regex regex = new Regex(searchPattern);

                List<string> list = wedFiles.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();

                if (list.Count != 0)
                {
                    return list[0];
                }
                else
                {
                    throw new ApplicationException(string.Format("傾向管理ファイルが見つかりませんでした。 検索フォルダ:{0} ファイル名:{1}", directoryPath, fileNm));
                }
            }
        }

        #endregion

        #region ※CIFS専用(fin3・trs) 完了

        public class FinishedFile8
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin3";
            public const string TRACEFILE_IDENTITYNAME = "trs";

            public const string FIN_FILE_LOT = "LOTNO";

            public string FullName_Fin { get; set; }
            public string FullName_Trs { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public static List<FinishedFile8> GetAllFiles(string directoryPath)
            {
                List<FinishedFile8> retv = new List<FinishedFile8>();

                List<string> finFiles = GetFilesByBackwardMatching(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count != 0)
                {
                    foreach (string file in finFiles)
                    {
                        string carrierNo = Path.GetFileNameWithoutExtension(file);

                        //リネイム済みは除外
                        if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                        {
                            continue;
                        }

                        List<string> trsFiles = GetFiles(directoryPath, TRACEFILE_IDENTITYNAME, 0);

                        string trsFullName = getTrsFile(trsFiles, carrierNo, directoryPath);

                        string[] content = File.ReadAllLines(file);
                        string lotno = string.Empty;
                        // ファイルの中身を1行ずつ確認
                        foreach (string s in content)
                        {
                            string[] fileData = s.Split(',');
                            if (fileData.Length <= 1)
                            {
                                continue;
                            }

                            // ロットNO
                            if (fileData[0] == FIN_FILE_LOT)
                            {
                                lotno = fileData[1].Trim();
                            }
                        }

                        ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotno, true);
                        if (mags.Count() == 0)
                        {
                            throw new ApplicationException("マガジンが見つかりません:" + lotno);
                        }
                        ArmsApi.Model.Magazine mag = mags.Single();

                        FinishedFile8 fin = new FinishedFile8();

                        fin.CarrierNo = carrierNo;
                        fin.NascaLotNo = lotno;
                        fin.MagNo = mag.MagazineNo;
                        fin.FullName_Fin = file;
                        fin.FullName_Trs = trsFullName;

                        retv.Add(fin);
                    }
                }

                return retv;
            }

            private static string getTrsFile(List<string> trsFiles, string carrierNo, string directoryPath)
            {
                string searchPattern = carrierNo + "[.]" + TRACEFILE_IDENTITYNAME;
                Regex regex = new Regex(searchPattern);

                List<string> list = trsFiles.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();

                if (list.Count != 0)
                {
                    return list[0];
                }
                else
                {
                    throw new ApplicationException(string.Format("排出トレイトレースファイルが見つかりませんでした。 検索フォルダ:{0} ファイル名:{1}", directoryPath, carrierNo));
                }
            }
        }

        #endregion

        #region ※CIFS専用(fin・wed) 完了 (TnCassette, TnLotCarrier両テーブルに紐付が無いか確認)

        public class FinishedFile9
        {
            public const string FINISHEDFILE_IDENTITYNAME = "fin";
            public const string WORKENDFILE_IDENTITYNAME = "wed";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }
            public string FullName_Fin { get; set; }
            public string FullName_Wed { get; set; }
            public string CarrierNo { get; set; }
            public string NascaLotNo { get; set; }
            public string MagNo { get; set; }

            public static List<FinishedFile9> GetAllFiles(string directoryPath)
            {
                List<FinishedFile9> retv = new List<FinishedFile9>();

                List<string> finFiles = GetFilesOrderByLastAccessTime_NotRetry(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count != 0)
                {
                    List<string> wedFiles = GetFiles(directoryPath, WORKENDFILE_IDENTITYNAME, 0);
                    if (wedFiles.Count != 0)
                    {
                        foreach (string file in finFiles)
                        {
                            string carrierNo = Path.GetFileNameWithoutExtension(file);

                            //リネイム済みは除外
                            if (carrierNo.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                            {
                                continue;
                            }

                            string wedFullName = getWedFile(wedFiles, carrierNo, directoryPath);

                            //ArmsApi.Model.LotCarrier lotCarrier = ArmsApi.Model.LotCarrier.GetData(carrierNo, true);
                            //if (string.IsNullOrWhiteSpace(lotCarrier.LotNo))
                            //{
                            //    throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                            //}

                            string lotNo = ArmsApi.Model.LotCarrier.GetLotNo(carrierNo, null, true, false).ToList().FirstOrDefault();
                            if (string.IsNullOrWhiteSpace(lotNo))
                            {
                                lotNo = ArmsApi.Model.LotCarrier.GetLotNoFromRingNo(carrierNo, false);
                            }

                            if (string.IsNullOrWhiteSpace(lotNo))
                            {
                                throw new ApplicationException(string.Format("ロットNOが取得できませんでした。 キャリア番号:{0}", carrierNo));
                            }
                            
                            ArmsApi.Model.Magazine[] mags = ArmsApi.Model.Magazine.GetMagazine(lotNo, true);
                            if (mags.Count() == 0)
                            {
                                throw new ApplicationException("マガジンが見つかりません:" + lotNo);
                            }
                            ArmsApi.Model.Magazine mag = mags.Single();

                            FinishedFile9 fin = new FinishedFile9();

                            fin.CarrierNo = carrierNo;
                            fin.NascaLotNo = lotNo;
                            fin.MagNo = mag.MagazineNo;
                            fin.FullName_Fin = file;
                            fin.FullName_Wed = wedFullName;
                            string[] content = File.ReadAllLines(fin.FullName_Fin);

                            DateTime? startDt = null;
                            DateTime? endDt = null;

                            // ファイルの中身を1行ずつ確認
                            foreach (string s in content)
                            {
                                string[] fileData = s.Split(',');
                                if (fileData.Length <= 1)
                                {
                                    continue;
                                }

                                // 開始時刻
                                if (fileData[0] == FIN_FILE_START)
                                {
                                    startDt = getDate(fileData[1].Trim(), "開始日時");
                                }
                                // 完了時刻
                                if (fileData[0] == FIN_FILE_END)
                                {
                                    endDt = getDate(fileData[1].Trim(), "完了日時");
                                }
                            }

                            fin.WorkStartDt = startDt;
                            fin.WorkEndDt = endDt;

                            retv.Add(fin);
                        }
                    }
                }

                return retv;
            }


            /// <summary>
            /// 対象のwedファイル取得
            /// </summary>
            /// <param name="wedFiles"></param>
            /// <param name="carrierNo"></param>
            /// <param name="directoryPath"></param>
            /// <returns></returns>
            private static string getWedFile(List<string> wedFiles, string carrierNo, string directoryPath)
            {
                string searchPattern = carrierNo + "[.]" + WORKENDFILE_IDENTITYNAME;
                Regex regex = new Regex(searchPattern);

                List<string> list = wedFiles.Where(p => regex.IsMatch(Path.GetFileName(p))).ToList();

                if (list.Count != 0)
                {
                    return list[0];
                }
                else
                {
                    throw new ApplicationException(string.Format("傾向管理ファイルが見つかりませんでした。 検索フォルダ:{0} キャリア番号:{1}", directoryPath, carrierNo));
                }
            }
        }

        #region CIFS用(蟻酸リフロー)。マガジン単位のtrgファイル。ファイル名にマガジンQRコード有り。Finファイル内にロード・アンロード時間あり。
        public class FinishedFile10
        {

            public const string FINISHEDFILE_IDENTITYNAME = "fin";
            public const string WORKENDFILE_IDENTITYNAME = "wed";
            public const string FIN_FILE_START = "LDTIME";
            public const string FIN_FILE_END = "ULDTIME";
            public const string FIN_FILE_LDMAGNO = "LD2D";
            public const string FIN_FILE_ULDMAGNO = "ULD2D";

            public DateTime? WorkStartDt { get; set; }
            public DateTime? WorkEndDt { get; set; }
            public string FullName_Fin { get; set; }
            public string NascaLotNo { get; set; }
            public string TypeCd { get; set; }
            public string UnloaderMagNo { get; set; }
            public string LoaderMagNo { get; set; }

            public static List<FinishedFile10> GetAllFiles(string directoryPath)
            {
                List<FinishedFile10> retv = new List<FinishedFile10>();

                List<string> finFiles = GetFilesOrderByLastAccessTime_NotRetry(directoryPath, FINISHEDFILE_IDENTITYNAME);
                if (finFiles.Count != 0)
                {
                    List<string> wedFiles = GetFiles(directoryPath, WORKENDFILE_IDENTITYNAME, 0);
                    if (wedFiles.Count != 0)
                    {
                        foreach (string file in finFiles)
                        {
                            string fileName = Path.GetFileNameWithoutExtension(file);
                            //リネイム済みは除外
                            if (fileName.Split('_').Length >= FINISHED_RENAME_ELEMENT_NUM)
                            {
                                continue;
                            }

                            FinishedFile10 fin = new FinishedFile10();

                            string[] contents = File.ReadAllLines(file);

                            foreach (string s in contents)
                            {
                                string[] fileData = s.Split(',');
                                if (fileData.Length <= 1)
                                {
                                    continue;
                                }

                                //ローダー側マガジン
                                if (fileData[0] == FIN_FILE_LDMAGNO)
                                {
                                    fin.LoaderMagNo = fileData[1].Trim();
                                    string[] magElm = fin.LoaderMagNo.Split(' ');
                                    if(magElm.Length > 0)
                                    {
                                        fin.LoaderMagNo = magElm[1];
                                    }
                                    continue;
                                }

                                //アンローダー側マガジン
                                if (fileData[0] == FIN_FILE_ULDMAGNO)
                                {
                                    fin.UnloaderMagNo = fileData[1].Trim();
                                    string[] magElm = fin.UnloaderMagNo.Split(' ');
                                    if (magElm.Length > 0)
                                    {
                                        fin.UnloaderMagNo = magElm[1];
                                    }
                                    continue;
                                }

                                // 開始時刻
                                if (fileData[0] == FIN_FILE_START)
                                {
                                    fin.WorkStartDt = getDate(fileData[1].Trim(), "開始日時");
                                    continue;
                                }
                                // 完了時刻
                                if (fileData[0] == FIN_FILE_END)
                                {
                                    fin.WorkEndDt = getDate(fileData[1].Trim(), "完了日時");
                                    continue;
                                }
                            }

                            if(string.IsNullOrWhiteSpace(fin.LoaderMagNo) || string.IsNullOrWhiteSpace(fin.UnloaderMagNo)
                                || fin.WorkEndDt == null || fin.WorkStartDt == null)
                            {
                                throw new ApplicationException($"finファイルからマガジン・開始・完了時間の何れかが取得できません。装置ファイルを退避し、装置動作について装置担当者に確認を行ってください。対象ファイル：『{file}』");
                            }

                            ArmsApi.Model.Magazine svrmag = ArmsApi.Model.Magazine.GetCurrent(fin.LoaderMagNo);
                            if (svrmag == null) throw new ApplicationException("ArmsSvr マガジン情報が見つかりません:" + fin.LoaderMagNo);
                            ArmsApi.Model.AsmLot svrlot = ArmsApi.Model.AsmLot.GetAsmLot(svrmag.NascaLotNO);
                            if (svrlot == null) throw new ApplicationException("ArmsSvr ロット情報が見つかりません:" + svrmag.NascaLotNO);

                            fin.NascaLotNo = svrlot.NascaLotNo;
                            fin.TypeCd = svrlot.TypeCd;
                            fin.FullName_Fin = file;
                            
                            retv.Add(fin);
                        }
                    }
                }

                return retv;
            }
        }
        #endregion

        #endregion
        public static bool IsFishishedOutput(string directoryPath, string fileKey, int retryCt, bool isLotUnCompleteFile, DateTime? fileStampFromDt)
        {
            return IsFishishedOutput(directoryPath, fileKey, retryCt, isLotUnCompleteFile, fileStampFromDt, true);
        }

        /// <summary>
        /// 1ロットの全ログファイルが出力完了したか確認 (finファイル等出力存在確認)
        /// isWating == trueの場合のみArmsConfigの設定に沿ってリトライを実施。
        /// </summary>
        /// <param name="directoryPath"></param>
        /// <returns></returns>
        public static bool IsFishishedOutput(string directoryPath, string fileKey, int retryCt, bool isLotUnCompleteFile, DateTime? fileStampFromDt, bool isWaiting)
		{

			if (retryCt >= Config.Settings.FinishedFileAccessRetryCt && isWaiting == true)
			{
				return false;
			}

            List<string> finFiles = GetFiles(directoryPath, fileKey);

			if (isLotUnCompleteFile)
			{
				finFiles = finFiles.Where(f => IsLotFromFileName(f) == false).ToList();
			}

			if (fileStampFromDt.HasValue)
			{
				finFiles = finFiles.Where(f => File.GetLastWriteTime(f) >= fileStampFromDt.Value).ToList();
			}
            

			if (finFiles.Count == 0 && isWaiting == true)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				return IsFishishedOutput(directoryPath, fileKey, retryCt, isLotUnCompleteFile, fileStampFromDt, isWaiting);
            }
            else if (finFiles.Count == 0 && isWaiting == false)
            {
                return false;
            }
			else
			{
				return true;
			}
            

		}

		public static bool IsFishishedOutput(string directoryPath)
		{
			return IsFishishedOutput(directoryPath, string.Format("^.*{0}$", FinishedFile.FINISHEDFILE_IDENTITYNAME), 0, false, null);
		}

		public static bool IsFishishedOutput(string directoryPath, string fileKey)
		{
			return IsFishishedOutput(directoryPath, fileKey, 0, false, null);
		}

		public static bool IsFishishedOutput(string directoryPath, bool isLotUnCompleteFile)
		{
			return IsFishishedOutput(directoryPath, string.Format("^.*{0}$", FinishedFile.FINISHEDFILE_IDENTITYNAME), 0, isLotUnCompleteFile, null);
		}

        public static bool IsFishishedOutput(string directoryPath, bool isLotUnCompleteFile, bool isWaiting)
        {
            return IsFishishedOutput(directoryPath, string.Format("^.*{0}$", FinishedFile.FINISHEDFILE_IDENTITYNAME), 0, isLotUnCompleteFile, null, isWaiting);
        }

        public static bool IsFishishedOutput(string directoryPath, bool isLotUnCompleteFile, DateTime fileStampFromDt)
		{
			return IsFishishedOutput(directoryPath, string.Format("^.*{0}$", FinishedFile.FINISHEDFILE_IDENTITYNAME), 0, isLotUnCompleteFile, fileStampFromDt);
		}

		public static bool IsFishishedOutput(string directoryPath, string fileKey, bool isLotUnCompleteFile)
		{
			return IsFishishedOutput(directoryPath, fileKey, 0, isLotUnCompleteFile, null);
		}

		public static bool IsFishishedOutput(string directoryPath, string fileKey, bool isLotUnCompleteFile, DateTime fileStampFromDt)
		{
			return IsFishishedOutput(directoryPath, fileKey, 0, isLotUnCompleteFile, fileStampFromDt);
		}

		/// <summary>
		/// 最古の完了トリガファイルを取得
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="fileKey"></param>
		/// <param name="isLotUnCompleteFile"></param>
		/// <param name="retryCt"></param>
		/// <returns></returns>
		private static string GetEarliestFishishedFile(string directoryPath, string fileKey, bool isLotUnCompleteFile, int retryCt) 
		{
			if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
			{
				return null;
			}

            List<string> finFiles = GetFiles(directoryPath, fileKey);

			if (isLotUnCompleteFile)
			{
				finFiles = finFiles.Where(f => IsLotFromFileName(f) == false).ToList();
			}

			if (finFiles.Count == 0)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				return GetEarliestFishishedFile(directoryPath, fileKey, isLotUnCompleteFile, retryCt);
			}
			else
			{
				return finFiles.OrderBy(f => File.GetLastWriteTime(f)).First();
			}
		}

		/// <summary>
		/// 最古の完了トリガファイルを取得
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="fileKey"></param>
		/// <param name="isLotUnCompleteFile"></param>
		/// <returns></returns>
		public static string GetEarliestFishishedFile(string directoryPath, string fileKey, bool isLotUnCompleteFile) 
		{
			return GetEarliestFishishedFile(directoryPath, fileKey, isLotUnCompleteFile, 0);
		}

		/// <summary>
		/// 1ロットの全ログファイルを取得
		/// </summary>
		/// <param name="directoryPath"></param>
		/// <param name="startDt"></param>
		/// <param name="endDt"></param>
		/// <returns></returns>
		public static List<string> GetLotFiles(string directoryPath, DateTime startDt, DateTime endDt, int retryCt)
		{
			if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
			{
				throw new ApplicationException(string.Format("該当ファイルが存在しません。リトライ回数が上限に達しました。検索フォルダ:{0} 検索範囲 開始:{1} 完了:{2}"
					, directoryPath, startDt, endDt));
			}

			Dictionary<string, DateTime> retv = new Dictionary<string, DateTime>();

			List<string> files = GetFiles(directoryPath);
			foreach (string file in files)
			{
				if (File.Exists(file) == false)
				{
					if (File.Exists(string.Format("_{0}", file)))
					{
						throw new ApplicationException(
							string.Format("装置パラメータのチェックが終わっていないか、存在しないファイルを参照しようとしてます。 ファイルパス:{0}", file));
					}
				}

				string fileName = Path.GetFileNameWithoutExtension(file);
				string[] fileNameChars = fileName.Split('_');

                //2016/7/12 ファイル名から日付取るのじゃなくて、最終更新日時から取得して並び替えるようにする
                DateTime outputDate = File.GetLastWriteTime(file);
                
                //string date = fileNameChars[0].Substring(0, 4) + "/" + fileNameChars[0].Substring(4, 2) + "/" + fileNameChars[0].Substring(6, 2) + " " +
                //    fileNameChars[0].Substring(8, 2) + ":" + fileNameChars[0].Substring(10, 2) + ":" + fileNameChars[0].Substring(12, 2);
                //if (DateTime.TryParse(date, out outputDate) == false)
                //{
                //    throw new ApplicationException(string.Format("日付型に変換できないファイル名が付いています ファイルパス:{0}", fileNameChars[0]));
                //}

				startDt = startDt.AddMilliseconds(-startDt.Millisecond);

				if (outputDate >= startDt && outputDate <= endDt)
				{
					retv.Add(file, outputDate);
				}
			}

			if (retv.Count == 0)
			{
				Thread.Sleep(1000);
				retryCt = retryCt + 1;
				return GetLotFiles(directoryPath, startDt, endDt, retryCt);
			}

			return retv.OrderBy(r => r.Value).Select(r => r.Key).ToList();
		}
		public static List<string> GetLotFiles(string directoryPath, DateTime startDt, DateTime endDt)
		{
			return GetLotFiles(directoryPath, startDt, endDt, 0);
		}

		public static List<string> GetLotFilesFromFileStamp(string directoryPath, DateTime startDt, DateTime endDt)
		{
			List<string> retv = new List<string>();

			List<string> files = GetFiles(directoryPath);

			//ファイルを更新日時順に並び変えて、処理のトリガファイルを末尾にして返す
			files = files.OrderBy(f => File.GetLastWriteTime(f)).ToList();

			foreach (string file in files)
			{
				if (File.Exists(string.Format("_{0}", file)))
				{
					throw new ApplicationException(
						string.Format("装置パラメータのチェックが終わっていないか、存在しないファイルを参照しようとしてます。 ファイルパス:{0}", file));
				}

				DateTime lastWriteDt = File.GetLastWriteTime(file);

				if (lastWriteDt >= startDt && lastWriteDt <= endDt)
				{
					retv.Add(file);
				}
			}

			return retv;
		}

        public static void BackupFile()
        {
        }

	}
}
