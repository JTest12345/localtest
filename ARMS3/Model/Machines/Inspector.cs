using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using System.IO;
using System.Threading;
using System.Text.RegularExpressions;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 検査機
    /// </summary>
    public class Inspector : MachineBase
    {
        #region 装置ログ関連

        private const int DATA_NO_COL = 0;
        private const int MAG_NO_COL = 3;

        #endregion

        /// <summary>
        /// 供給マガジンNo取得アドレス
        /// </summary>
        public string LMagazineAddress { get; set; }

        /// <summary>
        /// 終了時排出マガジンNo取得アドレス
        /// </summary>
        public string ULMagazineAddress { get; set; }

        /// <summary>
        /// 直前の完成マガジンNO
        /// </summary>
        public string preCompleteMagNo { get; set; }

		/// <summary>
		/// MMファイル保存パス
		/// </summary>
		public string MMFilePath { get; set; }

        public Inspector() {}

        protected override void concreteThreadWork()
        {
            if (this.IsRequireOutput() == true)
            {
                if (this.IsAutoLine)
                {
                    workComplete();
                }
                else
                {
                    workCompletehigh();
                }               
            }

            // MMファイル処理
            CheckMachineLogFile();
            //mmComplete();

            // 仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);

            if (string.IsNullOrEmpty(newmagno) == false)
            {
                newMagazine.MagazineNo = newmagno;
                newMagazine.LastMagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
                return;
            }
			Magazine svrmag = Magazine.GetCurrent(newmagno);

            //作業IDを取得（DB後、WB後、MD後で値が違うため都度問合せ）
            int currentProcNo = int.MinValue;
            try
            {
                currentProcNo = Order.GetLastProcNo(this.MacNo, newMagazine.MagazineNo);
                newMagazine.ProcNo = currentProcNo;
            }
            catch(Exception ex)
            {
                throw new ApplicationException("次作業問合せエラー:" + ex.Message + ":" + ex.StackTrace);
            }

            //次作業を取り損ねた場合は排出行き
            if (currentProcNo == int.MinValue)
            {
                newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
                this.Enqueue(newMagazine, Station.Unloader);

                //Log.WriteDischargeLog(mag.MagazineNo, "検査機排出時にNASCA工程ID取得失敗\nNASCAデータを確認してください");
                AsmLot.InsertLotLog("", System.DateTime.Now, "検査機排出時にNASCA工程ID取得失敗\nNASCAデータを確認してください", currentProcNo, newMagazine.MagazineNo, true, this.LineNo);
                return;
            }

			try
			{
				//作業開始完了時間取得
				newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
			}
			catch
			{
				newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
				this.Enqueue(newMagazine, Station.Unloader);
				AsmLot.InsertLotLog("", System.DateTime.Now, "検査機排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください", currentProcNo, newMagazine.MagazineNo, true, this.LineNo);
				return;
			}

            //EICSが作成したNasca不良ファイルを登録
			//Defect.ImportNascaDefectFile(newMagazine.MagazineNo, this.PlantCd, newMagazine.ProcNo.Value);
			bool result = RunDefect(newMagazine.MagazineNo, this.PlantCd, newMagazine.ProcNo.Value);
			if (result == false) 
			{
				return;
			}

            //現在完了工程で削除フラグONかつ規制理由「」が存在する場合、毎回規制を復活させる
            Magazine magazine = Magazine.GetCurrent(newMagazine.MagazineNo);
            if (magazine == null) throw new ApplicationException(string.Format("検査機マガジンデータ異常　現在稼働中マガジンがありません:{0}", magazine));
            Restrict[] reslist = Restrict.SearchRestrict(magazine.NascaLotNO, currentProcNo, false);
            foreach (Restrict res in reslist)
            {
                //周辺強度による規制理由と同じか確認
                if (res.Reason == WireBonder.MMFile.RESTRICT_REASON_2)
                {
                    //規制を有効化
                    res.DelFg = false;
                    res.Save();
                }
            }

            this.Enqueue(newMagazine, Station.Unloader);
            this.WorkComplete(newMagazine, this, true);
        }

        /// <summary>
        /// 作業完了(高効率)
        /// </summary>
        private void workCompletehigh() 
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag newMagazine = new VirtualMag();

            //キュー順序入れ替わりの可能性があるのでPLCから最新の情報を取得
            string newmagno = Plc.GetMagazineNo(ULMagazineAddress);
            if (string.IsNullOrEmpty(newmagno) == false)
            {
                //直前の完成マガジンと比較
                //一致の場合はアンローダー上で完了登録をしたと判断して排出要求無視
                if (this.preCompleteMagNo == newmagno)
                {
                    return;
                }

                newMagazine.MagazineNo = newmagno;
                newMagazine.LastMagazineNo = newmagno;
            }
            else
            {
                Log.RBLog.Info("検査機排出マガジンNOの取得に失敗。\n検査機排出位置のマガジンは装置に作業記録がありません。\n手動で取り除いてください。");
                return;
            }

            //作業IDを取得（DB後、WB後、MD後で値が違うため都度問合せ）
            int currentProcNo = int.MinValue;
            try
            {
                currentProcNo = Order.GetLastProcNo(this.MacNo, newMagazine.MagazineNo);
                newMagazine.ProcNo = currentProcNo;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("次作業問合せエラー:" + ex.Message + ":" + ex.StackTrace);
            }

			try
			{
				//作業開始完了時間取得
				newMagazine.WorkComplete = Plc.GetWordsAsDateTime(this.WorkCompleteTimeAddress);
				newMagazine.WorkStart = Plc.GetWordsAsDateTime(this.WorkStartTimeAddress);
			}
			catch
			{
				newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
				this.Enqueue(newMagazine, Station.Unloader);
				AsmLot.InsertLotLog("", System.DateTime.Now, "検査機排出位置のマガジンに開始・完了時間の装置記憶がありません。\nNASCAデータを確認してください", currentProcNo, newMagazine.MagazineNo, true, this.LineNo);
				return;
			}

            //EICSが作成したNasca不良ファイルを登録
			if (Config.Settings.ImportNascaDefect == true)
			{
				RunDefect(newMagazine.MagazineNo, this.PlantCd, newMagazine.ProcNo.Value);
				//Defect.ImportNascaDefectFile(newMagazine.MagazineNo, this.PlantCd, newMagazine.ProcNo.Value);
			}

            //Enqueueが先にないとLoc情報が無い
            this.Enqueue(newMagazine, Station.Unloader);

            //最終の完成ロット情報として記録
            preCompleteMagNo = newMagazine.MagazineNo;
        }

		//public void mmComplete()
		//{
		//	//MMファイル処理
		//	MMFile mm = MMFile.GetMMFileInfo(MMFilePath);
		//	if (mm == null)
		//	{
		//		return;
		//	}

		//	if (mm.IsLotFromFileName(mm.FileFullPath) == false)
		//	{
		//		// ファイル名称変更(ロット + タイプ + 工程NO付与)
		//		mm.ChangeCompleteFileName(mm.FileFullPath, mm.LotNo, mm.TypeNo, mm.ProcNo);
		//	}

		//	if (mm.IsUnknownData)
		//	{
		//		// MMファイル内容がヘッダのみの空ファイルを想定　ファイル名のみUNKNOWNに変更、以降の処理をスルーしてEICSに削除処理を任せる
		//		return;
		//	}

		//	if (Config.Settings.IsMappingMode)
		//	{
		//		if (ArmsApi.Model.LENS.WorkResult.IsComplete(mm.LotNo, mm.ProcNo) == false)
		//		{
		//			// LENS2処理待ち
		//			Log.ApiLog.Info(string.Format("LENS処理待ち mag:{0}", mm.MagNo));
		//			return;
		//		}
		//	}
		//}

        public virtual void CheckMachineLogFile()
        {
            List<string> logFiles = MachineLog.GetFiles(this.LogOutputDirectoryPath);
            foreach (string logFile in logFiles)
            {
                if (MachineLog.IsLotFromFileName(logFile)) continue;

                if (IsStartLogFile(logFile) == false)
                {
                    MachineLog mLog = parseMachineLog(logFile);
                    if (mLog.IsUnknownData)
                    {
                        MachineLog.ChangeFileName(logFile, MachineLog.FILE_UNKNOWN, MachineLog.FILE_UNKNOWN, 0, MachineLog.FILE_UNKNOWN);
                        return;
                    }

                    Magazine svrMag = Magazine.GetCurrent(mLog.MagazineNo);
                    if (svrMag == null)
                        throw new ApplicationException(
                            string.Format("装置ログ内のマガジンNo:{0}の稼働中マガジンが存在しません。", mLog.MagazineNo));

                    AsmLot lot = AsmLot.GetAsmLot(svrMag.NascaLotNO);
                    int procNo = Process.GetNowProcess(lot).ProcNo;

                    MachineLog.ChangeFileName(logFile, lot.NascaLotNo, lot.TypeCd, procNo, svrMag.MagazineNo);
                }
            }
        }

        public bool IsStartLogFile(string fullName)
        {
            string fileName = Path.GetFileNameWithoutExtension(fullName);
            if (Regex.IsMatch(fileName, "^log..._OA.*$"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 装置ログファイルの内容を解析
        /// </summary>
        /// <param name="filepath"></param>
        /// <param name="magno"></param>
        /// <param name="isContains1"></param>
        /// <param name="isContains3"></param>
        protected MachineLog parseMachineLog(string filepath, int retryCt)
        {
            if (retryCt >= Config.Settings.FinishedFileAccessRetryCt)
            {
                throw new ApplicationException("ファイル解析中にエラーが発生しました：" + filepath);
            }

            MachineLog mLog = new MachineLog();
            string magNo = null;

            try
            {
                using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
                {
                    int lineno = 0;
                    while (sr.Peek() > 0)
                    {
                        string line = sr.ReadLine();

                        line = line.Replace("\"", "");

                        if (string.IsNullOrEmpty(line)) continue;

                        //CSV各要素分解
                        string[] data = line.Split(',');

                        //DATA_NOが数字変換できない行は飛ばす
                        int datano;
                        if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;

                        string[] magStr = data[MAG_NO_COL].Split(' ');
                        if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
                        {
                            //自動化用の30_を取り除き
                            magNo = magStr[1];
                        }
                        else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
                        {
                            //高効率用の11_を取り除き
                            magNo = magStr[1];
                        }
                        else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
                        {
                            //高効率用の13_を取り除き
                            magNo = magStr[1];
                        }
                        else
                        {
                            magNo = data[MAG_NO_COL];
                        }

                        lineno = lineno + 1;
                    }

                    if (lineno == 0)
                    {
                        // データがヘッダのみを想定
                        mLog.IsUnknownData = true;
                        return mLog;
                    }

                    if (magNo == null) throw new ApplicationException("装置ログにマガジン番号が存在しません:" + filepath);
                }

                mLog.MagazineNo = magNo;
                return mLog;
            }
            catch (IOException)
            {
                Thread.Sleep(1000);
                retryCt = retryCt + 1;
                return parseMachineLog(filepath, retryCt);
            }
        }

        protected MachineLog parseMachineLog(string filepath)
        {
            return parseMachineLog(filepath, 0);
        }


        /// <summary>
        /// 不良登録処理
        /// </summary>
        /// <param name="mag"></param>
        public bool RunDefect(string magazineNo, string plantCd, int procNo)
		{
			if (Config.Settings.ImportNascaDefect == true)
			{
				if (Directory.Exists(Config.Settings.NascaDefectFilePath) == false)
				{
					Directory.CreateDirectory(Config.Settings.NascaDefectFilePath);
				}

				string[] files = DirectoryHelper.GetFiles(Config.Settings.NascaDefectFilePath, string.Format("^.*{0}_{1}.*$", magazineNo, plantCd));
				//string[] files = System.IO.Directory.GetFiles(Properties.Settings.Default.MapFilePath, "*" + magazineNo + "_" + plantCd + "*");

				string path = null;
				//本日0時以降に作られたデータが対象
				DateTime lastWrite = DateTime.Now.Date;
				foreach (string file in files)
				{
					FileInfo fi = new FileInfo(file);
					if (lastWrite >= fi.LastWriteTime)
					{
						continue;
					}
					path = file;
					lastWrite = fi.LastWriteTime;
				}

				if (string.IsNullOrEmpty(path) == true)
				{
					Log.RBLog.Info("外観検査結果データ待ち mag:" + magazineNo);
					return false;
				}

				Magazine svrMagazine = Magazine.GetCurrent(magazineNo);

				bool result = InsertDefect(path, svrMagazine.NascaLotNO, procNo);
				if (result == true)
				{
					try
					{
						//完了ファイルの移動
						if (Directory.Exists(Config.Settings.NascaDefectFileDonePath) == false)
						{
							Directory.CreateDirectory(Config.Settings.NascaDefectFileDonePath);
						}

						string destPath = Path.Combine(Config.Settings.NascaDefectFileDonePath, Path.GetFileName(path));
						if (File.Exists(destPath) == true)
						{
							File.Delete(destPath);
						}
						File.Move(path, destPath);

						//残ったゴミファイルの掃除
						foreach (string file in files)
						{
							if (File.Exists(file))
							{
								File.Delete(file);
							}
						}
						return true;
					}
					catch
					{
						Log.RBLog.Info("外観検査データの完了フォルダへの移動時にエラー発生。\nNASCA登録は正常完了しているため処理続行。\n" + path);
						return true;
						//ファイル移動に関するエラーは握りつぶす
					}
				}
				else
				{
					throw new ApplicationException("検査結果登録でNASCAエラー発生");
				}
			}

			return true;
		}

		/// <summary>
		/// EICSが作成したNasca不良ファイルを登録
		/// </summary>
		/// <param name="inspectionDataPath"></param>
		public bool InsertDefect(string inspectionDataPath, string lotNO, int procNO)
		{
			List<DefItem> defectList = new List<DefItem>();

			try
			{
				using (StreamReader sr = new StreamReader(inspectionDataPath))
				{
					while (sr.Peek() >= 0)
					{
						string line = sr.ReadLine();
						if (string.IsNullOrEmpty(line))
						{
							continue;
						}

						//各行は4項目
						string[] data = line.Trim().Split(',');
						if (data.Length != 4)
						{
							Log.SysLog.Info("不正な検査データです:" + inspectionDataPath);
							continue;
						}

						//4列目は数字
						int count;
						if (int.TryParse(data[3], out count) == false)
						{
							Log.SysLog.Info("不正な検査データです:" + inspectionDataPath);
							continue;
						}

						DefItem defect = new DefItem(procNO, data[2], data[1], data[0], count);
						defectList.Add(defect);
					}
				}

				Defect d = new Defect();
				d.DefItems = defectList;
				d.LotNo = lotNO;
				d.ProcNo = procNO;
				d.DeleteInsert();

				return true;
			}
			catch (Exception ex)
			{
				Log.RBLog.Error("検査結果書き込みエラー", ex);
				return false;
			}
		}

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //実マガジンのアンローダー以外は何もしない
			if (station != Station.Unloader && station != Station.EmptyMagazineUnloader)
			{
                return true;
            }

            return base.Enqueue(mag, station);
        }

		public class MMFile
		{
			public const string FILE_PREFIX = "MM";
			public const string FILE_UNKNOWN = "UNKNOWN";
			private const int FILENAME_UPDDT_STARTINDEX = 9;
			private const int FILENAME_UPDDT_LENGTH = 10;

			private const int DATA_NO_COL = 0;
			private const int MAG_NO_COL = 3;

			private const int DATA_START_ROW = 3;

			/// <summary>
			/// MMファイル装置吐き出し位置
			/// </summary>
			public string BaseDir { get; set; }

			/// <summary>
			/// 処理対象ファイルフルパス
			/// </summary>
			public string FileFullPath { get; set; }

			/// <summary>
			/// MMファイル内マガジン番号　UL側マガジンと一致
			/// </summary>
			public string MagNo { get; set; }
			public string LotNo { get; set; }
			public string TypeNo { get; set; }
			public int ProcNo { get; set; }
			public bool IsUnknownData { get; set; }

			public static MMFile GetMMFileInfo(string mmFileDir)
			{
				MMFile mm = new MMFile();

				mm.BaseDir = mmFileDir;
				DirectoryInfo bdir = new System.IO.DirectoryInfo(mmFileDir);

				string path = getNewestFileName(mmFileDir);
				//ファイルが無い場合はNULLを返す
				if (string.IsNullOrEmpty(path)) return null;

				mm.FileFullPath = path;

				try
				{
					mm.parseMMFile(mm.FileFullPath);
				}
				catch (Exception ex)
				{
					throw new ApplicationException(string.Format("MMファイル解析エラー発生 ファイルパス:{0} エラー内容:{1}", mm.FileFullPath, ex.Message));
				}

				if (mm.IsUnknownData)
				{
					mm.LotNo = FILE_UNKNOWN;
					mm.TypeNo = FILE_UNKNOWN;
					mm.ProcNo = 0;
				}
				else
				{
					Magazine svrmag = Magazine.GetCurrent(mm.MagNo);
					if (svrmag == null) throw new ApplicationException("MMファイル処理エラー 現在稼働中マガジン情報が存在しません:" + mm.MagNo);

					AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
					if (lot == null) throw new ApplicationException("MMファイル処理エラー Svrロット情報が存在しません:" + mm.MagNo);

					mm.LotNo = lot.NascaLotNo;
					mm.TypeNo = lot.TypeCd;
					mm.ProcNo = Process.GetNowProcess(lot).ProcNo;
				}

				return mm;
			}

			/// <summary>
			/// MMファイルの内容を解析
			/// </summary>
			/// <param name="filepath"></param>
			/// <param name="magno"></param>
			/// <param name="isContains1"></param>
			/// <param name="isContains3"></param>
			private void parseMMFile(string filepath)
			{
				string magNo = null;

				try
				{
					using (System.IO.StreamReader sr = new System.IO.StreamReader(filepath))
					{
						int lineno = 0;
						while (sr.Peek() > 0)
						{
							string line = sr.ReadLine();
							line = line.Replace("\"", "");

							if (string.IsNullOrEmpty(line)) continue;

							//CSV各要素分解
							string[] data = line.Split(',');

							//DATA_NOが数字変換できない行は飛ばす
							int datano;
							if (int.TryParse(data[DATA_NO_COL], out datano) == false) continue;

							//if (magNo == null)
							//{
								string[] magStr = data[MAG_NO_COL].Split(' ');
								if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_MAGAZINE))
								{
									//自動化用の30_を取り除き
									magNo = magStr[1];
								}
								else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_INLINE_LOT))
								{
									//高効率用の11_を取り除き
									magNo = magStr[1];
								}
								else if (data[MAG_NO_COL].StartsWith(AsmLot.PREFIX_DEVIDED_INLINE_LOT))
								{
									//高効率用の13_を取り除き
									magNo = magStr[1];
								}
								else
								{
									magNo = data[MAG_NO_COL];
								}
							//}

							lineno = lineno + 1;
						}

						if (lineno == 0)
						{
							// データがヘッダのみを想定
							this.IsUnknownData = true;
							return;
						}

						if (magNo == null) throw new ApplicationException("MMファイルにマガジン番号が存在しません:" + filepath);
					}

					this.MagNo = magNo;
				}
				catch (IOException)
				{
					Thread.Sleep(5000);
					parseMMFile(filepath);
				}
			}

			/// <summary>
			/// MM*.CSVファイルの内、最終更新日が新しい1つを返す
			/// ファイルが全く無い場合はnull
			/// </summary>
			/// <param name="mmFileDir"></param>
			/// <returns></returns>
			private static string getNewestFileName(string mmFileDir)
			{
				string filePath = MachineLog.GetNewestFile(mmFileDir, MMFile.FILE_PREFIX);

				return filePath;
			}

			/// <summary>
			/// MMファイル処理完了確認 
			/// </summary>
			/// <param name="filePath"></param>
			/// <returns></returns>
			public bool IsLotFromFileName(string mmFilePath)
			{
				//名称変更済かで確認(ロット + タイプ + 工程NO付与)
				string[] nameChar = Path.GetFileNameWithoutExtension(mmFilePath).Split('_');
				if (nameChar.Count() < 5)
				{
					return false;
				}
				else { return true; }
			}

			/// <summary>
			/// MMファイル名称変更(ロット + タイプ + 工程NO付与)
			/// </summary>
			/// <param name="mmFilePath"></param>
			/// <param name="lotNo"></param>
			/// <param name="typeCd"></param>
			public void ChangeCompleteFileName(string mmFilePath, string lotNo, string typeCd, int procNo)
			{
				MachineLog.ChangeFileName(mmFilePath, LotNo, typeCd, procNo);
			}
		}
    }
}
