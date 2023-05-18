using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Machine
{
	public class CIFSBasedMachine : MachineBase
	{
		// 定数
		protected virtual int QC_TIMING_NO_ZD() { return 0; }
		protected virtual int QC_TIMING_NO_LED() { return 0; }
		public const string EXT_START_FILE = "wst";
		protected virtual string EXT_END_FILE() { return "wed"; }
		public const string EXT_MPD_FILE = "mpd";

		protected const string EXT_FIN_FILE = "fin";
		protected const string EXT_FIN2_FILE = "fin2";

		protected virtual string EXT_TRIG_FILE() { return EXT_FIN_FILE; }
		protected virtual string EXT_MPDTRIG_FILE() { return EXT_FIN_FILE; }
		protected const int ERR_MSG_MAX_LEN = 255;
		/// <summary>LineFeed(改行文字)</summary>
		public const string LF = "\r\n";
		public const int FILEKIND_INDEX_IN_FILENM = 1;
		public const int IDENTITY_INDEX_IN_FILENM = 0;
		public const int LOTNO_INDEX_IN_FILENM = 1;
		public const int TYPECD_INDEX_IN_FILENM = 2;
		public const int PROC_INDEX_IN_FILENM = 3;
		public const int MAGNO_INDEX_IN_FILENM = 4;
		public const int CHK_TIME_SPAN = 3000;

		// プロパティ
		public char FileNmSplitter { get; set; }
		public int FileKindIndexInFileNm { get; set; }
		public int DateIndex { get; set; }
		public int DateLen { get; set; }
		public int EndFileIdLen { get; set; }
		public int IdentityIndexInFileNm { get; set; }
		public int TypeCdIndexInFileNm { get; set; }
		public int LotNoIndexInFileNm { get; set; }
		public int MagNoIndexInFileNm { get; set; }
		public int ProcNoIndexInFileNm { get; set; }
		public string EndFileDir { get; set; }
		public string StartFileDir { get; set; }
		public string EncodeStr { get; set; }
		public string AvailableAddress { get; set; }

        DateTime lastGetLimitMaster = new DateTime();

        public class CheckResult
		{
			public List<Log> RegistrationData;
			public List<ErrMessageInfo> ErrorInfo;

			public CheckResult()
			{
				this.RegistrationData = new List<Log>();
				this.ErrorInfo = new List<ErrMessageInfo>();
			}
		}

		public struct FileNameAdditionInfo
		{
			public string Identity;
			public string TypeCd;
			public string LotNo;
			public string MagNo;
			public int ProcNo;
		}

		void InitErrorMessageList()
		{
		}

		public CIFSBasedMachine()
		{
			if (base.errorMessageList == null)
			{
				base.errorMessageList = new List<ErrMessageInfo>();
			}
			
			InitPropAtFirstTime();
		}
		public override void InitFirstLoop(LSETInfo lsetInfo)
		{
			//完了時ファイルスキャンマスタチェック
			List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, false);
			CheckFileScanMaster(lsetInfo, fileScanList);

			CheckFileFmtFromParamWhenInit(lsetInfo, false);
		}

		protected void InitPropAtFirstTime()
		{
			FileNmSplitter = CIFS.FILE_NM_SPLITTER;
			FileKindIndexInFileNm = FILEKIND_INDEX_IN_FILENM;

			IdentityIndexInFileNm = IDENTITY_INDEX_IN_FILENM;
			TypeCdIndexInFileNm = TYPECD_INDEX_IN_FILENM;
			LotNoIndexInFileNm = LOTNO_INDEX_IN_FILENM;
			MagNoIndexInFileNm = MAGNO_INDEX_IN_FILENM;
			ProcNoIndexInFileNm = PROC_INDEX_IN_FILENM;
		}

		public void InitPropAtLoop(LSETInfo lsetInfo)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			
			DateIndex = settingInfoPerLine.GetDateStrIndex(lsetInfo.EquipmentNO);
			DateLen = settingInfoPerLine.GetDateStrLen(lsetInfo.EquipmentNO);
			EndFileIdLen = settingInfoPerLine.GetEndIdLen(lsetInfo.EquipmentNO);

			StartFileDir = Path.Combine(lsetInfo.InputFolderNM, settingInfoPerLine.GetStartFileDirNm(lsetInfo.EquipmentNO));

			if (Directory.Exists(StartFileDir) == false)
			{
				Directory.CreateDirectory(StartFileDir);
			}
			
			EndFileDir = Path.Combine(lsetInfo.InputFolderNM, settingInfoPerLine.GetEndFileDirNm(lsetInfo.EquipmentNO));

			if (Directory.Exists(EndFileDir) == false)
			{
				Directory.CreateDirectory(EndFileDir);
			}

			EncodeStr = settingInfoPerLine.GetEncodeStr(lsetInfo.EquipmentNO);

			//SelectTypeCD = settingInfoPerLine.GetMaterialCD(lsetInfo.EquipmentNO);
			//SelectChipNM = settingInfoPerLine.GetChipNM(lsetInfo.EquipmentNO);
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				base.machineStatus = Constant.MachineStatus.Runtime;

                InitPropAtLoop(lsetInfo);

                // 開始時処理関数
                StartingProcess(lsetInfo);

                int timingNo = GetTimingNo(lsetInfo.ChipNM);

                // 完了時処理関数
                EndingProcess(lsetInfo, timingNo);

                AdditionProcess(lsetInfo);
            }
			catch (Exception err)
			{
				throw;
			}
		}

        protected virtual void AdditionProcess(LSETInfo lsetInfo)
        {
        }

		protected virtual int GetTimingNo(string chipNm)
		{
			int timingNo;
			if (Common.IsLEDChip(chipNm))
			{
				timingNo = QC_TIMING_NO_LED();
			}
			else
			{
				timingNo = QC_TIMING_NO_ZD();
			}

			return timingNo;
		}

		protected bool IsStartableProcess()
		{
			return CIFS.IsFileOutputDone(StartFileDir, CHK_TIME_SPAN);
		}

		protected virtual string GetStartableFileDtIdentity()
		{
			return CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_TRIG_FILE(), DateIndex, DateLen, false);
		}

		protected virtual string GetStartableFileIdentity()
		{
			return CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_TRIG_FILE(), DateIndex, DateLen, false);
		}

		public virtual bool StartingProcess(LSETInfo lsetInfo)
		{
			List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

			return StartingProcess(lsetInfo, out errMsgList);
		}

		public virtual bool StartingProcess(LSETInfo lsetInfo, out List<ErrMessageInfo> errMsgList)
		{
			string targetFileIdentity = string.Empty;
			try
			{
				List<Log> logList = new List<Log>();

				errMsgList = new List<ErrMessageInfo>();
				
				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				DateTime doneDt = DateTime.MinValue;

				//処理開始して良いかトリガを確認
				targetFileIdentity = GetStartableFileIdentity();

				//処理可能な出力ファイルが有れば、ファイル毎に処理する関数
				if (string.IsNullOrEmpty(targetFileIdentity) == false)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 トリガ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

					List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, lsetInfo.TypeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

					List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(null, lsetInfo, true);

                    //閾値に開始時、完了時、装置内区分(EquipPart_ID)の識別が無くすべての閾値を取得した上で閾値の上位確認を行う。
                    FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, false);             

					List<FileFmtWithPlm> allFileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, true, plmPerTypeModelChipList, fileFmtList, null);

					// .wstファイル群の取得
					List<string> chkTargetFileList = Common.GetFiles(StartFileDir, EXT_START_FILE);

					List<FileScan> allStartUpFileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, true);

					// ファイル内の装置パラメタチェック
					foreach (string chkTargetFilePath in chkTargetFileList)
					{
						string fileNm = Path.GetFileName(chkTargetFilePath);
						string[] fileNmElement = Path.GetFileNameWithoutExtension(fileNm).Split(FileNmSplitter);

						string fileKind = string.Empty;

						if (FileKindIndexInFileNm < 0)
						{
						}
						else if (fileNmElement.Count() <= FileKindIndexInFileNm)
						{
							fileNmElement = new string[] { Path.GetFileNameWithoutExtension(fileNm), string.Empty };
							fileKind = fileNmElement[FileKindIndexInFileNm];
						}
						else
						{
							fileKind = fileNmElement[FileKindIndexInFileNm];
						}

						List<FileScan> fileScanList = allStartUpFileScanList.FindAll(f => f.PrefixNM == fileKind).ToList();
						FileScan fileScan;
						//List<FileFmt> fileFmtList = FileFmt.GetData(lsetInfo, true, fileKind, null);

						List<FileFmtWithPlm> fileFmtWithPlmList = allFileFmtWithPlmList.Where(f => f.FileFmt.PrefixNM == fileKind).ToList();
						fileFmtWithPlmList = fileFmtWithPlmList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.Plm.QcParamNO));

						if (fileScanList.Count == 0 && fileFmtWithPlmList.Count == 0)
						{
							continue;
						}
						else if (fileScanList.Count > 0 && fileFmtWithPlmList.Count == 0)
						{
							throw new ApplicationException(
								$"TmFileScanﾏｽﾀ設定済みですが閾値未設定(TmPLM、TmFILEFMT)です。ﾏｽﾀ管理者に連絡して下さい。設備:{lsetInfo.ModelNM} ﾌｧｲﾙ種類:{fileKind}");
						}
						else if (fileScanList.Count == 0 && fileFmtWithPlmList.Count > 0)
						{
							throw new ApplicationException(
								string.Format("TmFILEFMTﾏｽﾀ設定済みですがTmFileScan未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
								lsetInfo.ModelNM, fileKind));
						}

						if (fileScanList.Count == 1)
						{
							fileScan = fileScanList.Single();
						}
						else
						{
							throw new ApplicationException(
								string.Format("TmFileScanから複数のﾏｽﾀが取得されました。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}", lsetInfo.ModelNM, fileKind));
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
							, $"開始時処理[START] {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} {Path.GetFileName(chkTargetFilePath)}File");

						string filePath = Path.Combine(StartFileDir, fileNm);

						DateTime? dtFromFile = File.GetCreationTime(filePath);

						if (doneDt < dtFromFile.Value)
						{
							doneDt = dtFromFile.Value;
						}

						MachineFile macFile = new MachineFile();
						if (macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, LF, lsetInfo.TypeCD, string.Empty, string.Empty, dtFromFile.Value, fileScan, lsetInfo, lsetInfo.ChipNM, null, ref errMsgList, EncodeStr, ref logList) == false)
						{
							continue;
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
							, $"開始時処理[End] {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} {Path.GetFileName(chkTargetFilePath)}File");

					}

					if (doneDt != DateTime.MinValue)
					{
						chkTargetFileList.AddRange(GetBackupTargetStartFiles(targetFileIdentity));

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
													, $"開始時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileIdentity}");

						// 処理したファイルの移動
						BackupDoneStartFiles(chkTargetFileList, StartFileDir, string.Empty, doneDt);

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
													, $"開始時処理 完了 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileIdentity}");

						//判定結果出力
						if (errMsgList.Count > 0)
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
								, $"開始時処理 判定結果NG出力 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileIdentity}");

							base.errorMessageList.AddRange(errMsgList);

							string errMsg = string.Empty;

							foreach (ErrMessageInfo errMsgInfo in errMsgList)
							{
								errMsg += errMsgInfo.MessageVAL + "\r\n";
							}

							CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
						}
						else
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
								, $"開始時処理 判定結果OK出力 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileIdentity}");
							CIFS.OutputResultFile(StartFileDir, targetFileIdentity, string.Empty, true);
						}

						return true;
					}
				}
				return false;
			}
			catch (Exception err)
			{
				if (string.IsNullOrEmpty(targetFileIdentity))
				{
					string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
						, $"開始時処理 例外発生によるSTOP出力 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileIdentity}\r\n StackTrace:{err.StackTrace}");
					CIFS.OutputStopFile(StartFileDir, DateTime.Now.ToString("yyyyMMddHHmmss"), errMsg, false);
				}
				else
				{
					string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
						, $"開始時処理 例外発生によるNG出力 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileIdentity}\r\n StackTrace:{err.StackTrace}");
					CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
				}

				throw;
			}
		}

        public virtual void StartingProcess(LSETInfo lsetInfo, List<FileFmtWithPlm> fileFmtWithPlmList)
        {
            List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

            string targetFileIdentity = string.Empty;
            try
            {
                List<Log> logList = new List<Log>();
                DateTime doneDt = DateTime.MinValue;
                CheckResult chkResult = new CheckResult();

                SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

                //処理開始して良いかトリガを確認
                targetFileIdentity = GetStartableFileIdentity();

                //処理可能な出力ファイルが有れば、ファイル毎に処理する関数
                if (string.IsNullOrEmpty(targetFileIdentity) == false)
                {
                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 トリガ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                    // .wstファイル群の取得
                    List<string> chkTargetFileList = Common.GetFiles(StartFileDir, EXT_START_FILE);

                    // ファイル内の装置パラメタチェック
                    foreach (string chkTargetFilePath in chkTargetFileList)
                    {
                        string fileNm = Path.GetFileName(chkTargetFilePath);
                        string[] fileNmElement = Path.GetFileNameWithoutExtension(fileNm).Split(FileNmSplitter);

                        string fileKind = string.Empty;

                        if (FileKindIndexInFileNm < 0)
                        {
                        }
                        else if (fileNmElement.Count() <= FileKindIndexInFileNm)
                        {
                            fileNmElement = new string[] { Path.GetFileNameWithoutExtension(fileNm), string.Empty };
                            fileKind = fileNmElement[FileKindIndexInFileNm];
                        }
                        else
                        {
                            fileKind = fileNmElement[FileKindIndexInFileNm];
                        }

                        List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, fileKind, true);
                        FileScan fileScan;

                        List<FileFmt> fileFmtList = FileFmt.GetData(lsetInfo, true, fileKind, null);

                        if (fileScanList.Count == 0 && fileFmtList.Count == 0)
                        {
                            continue;
                        }
                        else if (fileScanList.Count > 0 && fileFmtList.Count == 0)
                        {
                            throw new ApplicationException(
                                string.Format("TmFileScanﾏｽﾀ設定済みですがTmFILEFMT未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
                                lsetInfo.ModelNM, fileKind));
                        }
                        else if (fileScanList.Count == 0 && fileFmtList.Count > 0)
                        {
                            throw new ApplicationException(
                                string.Format("TmFILEFMTﾏｽﾀ設定済みですがTmFileScan未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
                                lsetInfo.ModelNM, fileKind));
                        }
                        else if (fileScanList.Count == 1)
                        {
                            fileScan = fileScanList.Single();
                        }
                        else
                        {
                            throw new ApplicationException(
                                string.Format("TmFileScanから複数のﾏｽﾀが取得されました。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}", lsetInfo.ModelNM, fileKind));
                        }

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[START] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));

                        string filePath = Path.Combine(StartFileDir, fileNm);

                        DateTime? dtFromFile = File.GetCreationTime(filePath);

                        if (doneDt < dtFromFile.Value)
                        {
                            doneDt = dtFromFile.Value;
                        }

                        MachineFile macFile = new MachineFile();
                        if (macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, LF, lsetInfo.TypeCD, string.Empty, string.Empty, dtFromFile.Value, fileScan, lsetInfo, lsetInfo.ChipNM, null, ref errMsgList, EncodeStr, ref logList) == false)
                        {
                            continue;
                        }
                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));
                    }

                    if (doneDt != DateTime.MinValue)
                    {
                        chkTargetFileList.AddRange(GetBackupTargetStartFiles(targetFileIdentity));

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                        // 処理したファイルの移動
                        BackupDoneStartFiles(chkTargetFileList, StartFileDir, string.Empty, doneDt);

                        log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

                        //判定結果出力
                        if (errMsgList.Count > 0)
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果NG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                            base.errorMessageList.AddRange(errMsgList);

                            string errMsg = string.Empty;

                            foreach (ErrMessageInfo errMsgInfo in errMsgList)
                            {
                                errMsg += errMsgInfo.MessageVAL + "\r\n";
                            }

                            CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
                        }
                        else
                        {
                            log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                            CIFS.OutputResultFile(StartFileDir, targetFileIdentity, string.Empty, true);
                        }
                    }
                }
            }
            catch (Exception err)
            {
                if (string.IsNullOrEmpty(targetFileIdentity))
                {
                    string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるSTOP出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\n StackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
                    CIFS.OutputStopFile(StartFileDir, DateTime.Now.ToString("yyyyMMddHHmmss"), errMsg, false);
                }
                else
                {
                    string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

                    log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるNG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\n StackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
                    CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
                }

                throw;
            }
        }

        protected virtual List<string> GetBackupTargetStartFiles(string targetFileIdnetity)
		{
			List<string> targetFiles = new List<string>();

			targetFiles.Add(Path.Combine(StartFileDir, string.Join(".", targetFileIdnetity, EXT_TRIG_FILE())));
			return targetFiles;
		}

		protected virtual void BackupDoneStartFiles(List<string> chkTargetFileList, string targetFileDir, string lotNO, DateTime doneDt)
		{
			// 処理したファイルの移動
			CIFS.BackupDoneFiles(chkTargetFileList, targetFileDir, lotNO, doneDt);
		}

		protected virtual void BackupDoneEndFiles(List<string> fileListInEndDir, string destDir, string lotNO, DateTime doneDt)
		{
			CIFS.BackupDoneFiles(fileListInEndDir, destDir, lotNO, doneDt);
		}

		protected virtual void EndingProcess(LSETInfo lsetInfo, int? timingNo) 
		{
			EndingProcess(lsetInfo, timingNo, true);
		}

        protected virtual void EndingProcess(LSETInfo lsetInfo, int? timingNo, bool isAvailable)
        {
            EndingProcess(lsetInfo, timingNo, isAvailable, true);
        }

        protected virtual void EndingProcess(LSETInfo lsetInfo, int? timingNo, bool isAvailable, bool isNeedAddedInfo)
		{

			List<Log> logList = new List<Log>();
			List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

			bool doneFG = false;
			string typeCD = string.Empty;

			if (isAvailable == false) 
			{
				if (string.IsNullOrEmpty(CIFS.GetLatestProcessableFileIdentity(EndFileDir, CIFS.EXT_OK_FILE, DateIndex, EndFileIdLen, true)) == false)
				{
					//既に.OKファイルが存在する場合は作成しない
					return;
				}

				Thread.Sleep(3000);
				string dateStr = System.DateTime.Now.ToString("yyyyMMddHHmmss");
				CIFS.OutputResultFile(EndFileDir, dateStr, string.Empty, true);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, dateStr));
				return;
			}

			// ロット、タイプ、工程Noがファイル名に付与される。付与された事がEICSで処理していいというトリガ。
			// 出力ファイルの存在チェック関数呼び出し。
            // ファイルのリネーム情報を無視する場合はファイル名リネーム自体を無視してファイル取得。
			string targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(EndFileDir, EXT_TRIG_FILE(), DateIndex, EndFileIdLen, isNeedAddedInfo);
			FileNameAdditionInfo fileNmAddInfo = new FileNameAdditionInfo();

			//処理可能な出力ファイルが有れば、ファイル毎に処理する関数
			if(string.IsNullOrEmpty(targetFileIdentity) == false)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾄﾘｶﾞ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

				List<string> trigFilePathList = Common.GetFiles(EndFileDir, string.Format("{0}.*\\.{1}", targetFileIdentity, EXT_TRIG_FILE()));
				if(trigFilePathList.Count > 1)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.FATAL,
						string.Format("同一のﾌｧｲﾙ種類、出力ﾀｲﾐﾝｸﾞ識別文字列(日付orDM)でﾌｧｲﾙが複数抽出されました。 監視Dir:{0}　出力ﾀｲﾐﾝｸﾞ識別文字列:{1}",
						EndFileDir, targetFileIdentity));
				}

                string trigFilePath = trigFilePathList.Single();

                string trigFileNm = Path.GetFileNameWithoutExtension(trigFilePath);

                DateTime dt = File.GetLastWriteTime(trigFilePath);

                if(isNeedAddedInfo)
                {
                    fileNmAddInfo = GetInfoFromFileNm(trigFilePath, FileNmSplitter
                           , this.IdentityIndexInFileNm, this.TypeCdIndexInFileNm, this.LotNoIndexInFileNm, this.MagNoIndexInFileNm, this.ProcNoIndexInFileNm);
                }
                else
                {
                    fileNmAddInfo = getInfoFromOther(trigFilePath);
                }

                if (IsFinishedLENSProcess(fileNmAddInfo.LotNo, fileNmAddInfo.ProcNo, lsetInfo.InlineCD, targetFileIdentity, lsetInfo.EquipmentNO) == false)
                {
                    return;
                }

                AdditionEndProcess(lsetInfo, targetFileIdentity, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, fileNmAddInfo.ProcNo, lsetInfo.EquipmentNO);

                CheckResult chkResult = CheckMachineEndFile(lsetInfo, fileNmAddInfo.TypeCd, fileNmAddInfo.LotNo, fileNmAddInfo.MagNo, dt, targetFileIdentity);

                //処理対象ファイルの種類毎の情報を取得
                List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, false);

                //通常は完了時の判定結果ファイル出力は不要。
                //20160428現在、例外的に完了時の判定結果ファイル出力が必要なEDPMachineInfo、BWMMachineInfo以外は
                //OutputResultの中でファイル出力しないようにしている 2016/4/28 n.yoshimoto

                OutputResult(lsetInfo, targetFileIdentity, EndFileDir, chkResult, false);

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                BackupFileEndTiming(targetFileIdentity, fileNmAddInfo.LotNo, dt);

                log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                doneFG = true;

			}

			if(timingNo.HasValue && doneFG && string.IsNullOrEmpty(fileNmAddInfo.TypeCd) == false)
			{
				CheckQC(lsetInfo, timingNo.Value, fileNmAddInfo.TypeCd);
			}
		}

        protected virtual FileNameAdditionInfo getInfoFromOther(string filePath)
        {
            return new FileNameAdditionInfo();
            //オーバーライド用。必要に応じて各派生クラスでオーバーライドを作成する。
        }


        public static FileNameAdditionInfo GetInfoFromFileNm(string filePath, char fileNmSplitter, int identityIndex, int typeCdIndex, int lotNoIndex, int magNoIndex, int procNoIndex)
		{
			FileNameAdditionInfo fileNmAddInfo = new FileNameAdditionInfo();

			string fileNm = Path.GetFileNameWithoutExtension(filePath);

			string procNOStr;
			if (MachineFile.TryGetElementFromFileNm(fileNm, fileNmSplitter, identityIndex, null, out fileNmAddInfo.Identity) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名から識別文字列の取得が出来ませんでした。ﾌｧｲﾙ名:{0}　区切り文字:{1}　識別文字列を取得するｲﾝﾃﾞｸｽ:{2}", filePath, fileNmSplitter, identityIndex));
			}

			if (MachineFile.TryGetElementFromFileNm(fileNm, fileNmSplitter, typeCdIndex, null, out fileNmAddInfo.TypeCd) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名からﾀｲﾌﾟCDの取得が出来ませんでした。ﾌｧｲﾙ名:{0}　区切り文字:{1}　ﾀｲﾌﾟCDを取得するｲﾝﾃﾞｸｽ:{2}", filePath, fileNmSplitter, typeCdIndex));
			}

			if (MachineFile.TryGetElementFromFileNm(fileNm, fileNmSplitter, lotNoIndex, null, out fileNmAddInfo.LotNo) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名からﾛｯﾄNoの取得が出来ませんでした。ﾌｧｲﾙ名:{0}　区切り文字:{1}　ﾛｯﾄNoを取得するｲﾝﾃﾞｸｽ:{2}", filePath, fileNmSplitter, lotNoIndex));
			}

			if (MachineFile.TryGetElementFromFileNm(fileNm, fileNmSplitter, magNoIndex, null, out fileNmAddInfo.MagNo) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名からﾏｶﾞｼﾞﾝNoの取得が出来ませんでした。ﾌｧｲﾙ名:{0}　区切り文字:{1}　ﾏｶﾞｼﾞﾝNoを取得するｲﾝﾃﾞｸｽ:{2}", filePath, fileNmSplitter, magNoIndex));
			}

			if (MachineFile.TryGetElementFromFileNm(fileNm, fileNmSplitter, procNoIndex, null, out procNOStr) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ名から工程Noの取得が出来ませんでした。ﾌｧｲﾙ名:{0}　区切り文字:{1}　工程Noを取得するｲﾝﾃﾞｸｽ:{2}", filePath, fileNmSplitter, procNoIndex));
			}

			if (int.TryParse(procNOStr, out fileNmAddInfo.ProcNo) == false)
			{
				throw new ApplicationException(string.Format("ﾌｧｲﾙ名から取得した工程Noが数値に変換できません。数値変換対象:{0}", procNOStr));
			}

			return fileNmAddInfo;
		}

		protected virtual List<string> GetBackupTargetFile(string targetFileIdentity)
		{
			List<string> fileListInEndDir = Common.GetFiles(EndFileDir, string.Format("{0}.*", targetFileIdentity));
			fileListInEndDir = fileListInEndDir
				.Where(f => Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_OK_FILE) && Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_NG_FILE)).ToList();

			return fileListInEndDir;
		}

        protected virtual CheckResult CheckMachineEndFile(LSETInfo lsetInfo, string typeCD, string lotNO, string magNO, DateTime dt, string targetFileIdentity)
        {
            return CheckMachineEndFile(lsetInfo, typeCD, lotNO, magNO, dt, targetFileIdentity, null, null, true);
        }

        protected virtual CheckResult CheckMachineEndFile(LSETInfo lsetInfo, string typeCD, string lotNO, string magNO, DateTime dt, string targetFileIdentity, string prefixNM, bool isErrorSound)
        {
            return CheckMachineEndFile(lsetInfo, typeCD, lotNO, magNO, dt, targetFileIdentity, null, prefixNM, true);
        }

        // Ver 3.70.0 引数追加 → string resingroupcd (樹脂グループ)  virtualを削除。virtualは一つ上の樹脂グループ無し版の関数が引き継ぐ
        protected CheckResult CheckMachineEndFile(LSETInfo lsetInfo, string typeCD, string lotNO, string magNO, DateTime dt, string targetFileIdentity, string resingroupcd, string prefixNM, bool isErrorSound)
		{
			lsetInfo.TypeCD = typeCD;

			CheckResult chkResult = new CheckResult();

			//処理対象ファイルの種類毎の情報を取得
			List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, false);

			CheckFileScanMaster(lsetInfo, fileScanList);

			List<FILEFMTInfo> fileFmtList = ConnectDB.GetFILEFMTData(prefixNM, lsetInfo, false);

            List<Plm> plmPerTypeModelChipList = Plm.GetDatas(lsetInfo.InlineCD, typeCD, lsetInfo.ModelNM, null, false, lsetInfo.ChipNM, false);

            //閾値に開始時、完了時、装置内区分(EquipPart_ID)の識別が無くすべての閾値を取得した上で閾値の上位確認を行う
            FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, false);
            
            List<FileFmtWithPlm> allFileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, false, plmPerTypeModelChipList, fileFmtList, resingroupcd);
            
			foreach (FileScan fileScan in fileScanList)
			{
                //樹脂でチェックする場合（色調自動測定機）では判定時にファイルのリネームをしない場合があるのでPrefixの後のアンダースコアは条件にしない。
                string fileSearchPattern = string.Empty;
                if (string.IsNullOrWhiteSpace(resingroupcd) == true)
                {
                    fileSearchPattern = Common.GetSearchPatternStr(DateIndex, targetFileIdentity, true) + fileScan.PrefixNM + @"_.*\." + EXT_END_FILE();
                }
                else
                {
                    fileSearchPattern = Common.GetSearchPatternStr(DateIndex, targetFileIdentity, true) + fileScan.PrefixNM + @".*\." + EXT_END_FILE();
                }

                List<string> fileNmList = Common.GetFiles(EndFileDir, fileSearchPattern);

				if (fileNmList.Count == 0)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
						"ﾌｧｲﾙが存在しない為ｽｷｯﾌﾟ {0}/{1}/{2} 監視ﾊﾟｽ:{3} ﾌｧｲﾙ種:{4} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{5}"
						, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, EndFileDir, fileScan.PrefixNM, fileSearchPattern));

					continue;
				}
				else if (fileNmList.Count > 1)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN,
						string.Format("同一のﾌｧｲﾙ種類、識別文字列(日付orDM)でﾌｧｲﾙが複数抽出されました。 検索Dir:{0} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{1}",
						EndFileDir, fileSearchPattern));
				}
				//日付文字列とファイル種類が同じで複数ファイルが取れたらどうする？

				string fileNm = fileNmList.Single();
				string filePath = Path.Combine(EndFileDir, fileNm);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理[START] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));

				List<FileFmtWithPlm> fileFmtWithPlmList = allFileFmtWithPlmList.Where(f => f.FileFmt.PrefixNM == fileScan.PrefixNM).ToList();
				fileFmtWithPlmList = fileFmtWithPlmList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.Plm.QcParamNO));

				if (fileFmtWithPlmList.Count == 0)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, $"{lsetInfo.ModelNM}/{fileScan.PrefixNM}の閾値マスタが存在しない為、処理スキップ");
					continue;
				}

				MachineFile macFile = new MachineFile();
				if (macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, LF, typeCD, lotNO, magNO, dt
					, fileScan, lsetInfo, lsetInfo.ChipNM, getWithOutValues(), ref chkResult.ErrorInfo, EncodeStr, ref chkResult.RegistrationData, isErrorSound) == false)
				{
					continue;
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));
			}

			return chkResult;
		}

        /// <summary>
        /// CheckMachineEndFile関数内で使用。集計対象外の値を取得する
        /// </summary>
        protected virtual string[] getWithOutValues()
        {
            // ファイルから取得したデータを加工する。オーバーライド先で修正
            return null;
        }

        protected void CheckFileScanMaster(LSETInfo lsetInfo, List<FileScan> fileScanList)
		{
			List<FileFmt> fileFmtList = FileFmt.GetData(lsetInfo, true, null, null);

			if (fileScanList.Count > 0 && fileFmtList.Count == 0)
			{
				throw new ApplicationException(
					string.Format("TmFileScanﾏｽﾀ設定済みですがTmFILEFMT未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0}", lsetInfo.ModelNM));
			}
			else if (fileScanList.Count == 0 && fileFmtList.Count > 0)
			{
				throw new ApplicationException(
					string.Format("TmFILEFMTﾏｽﾀ設定済みですがTmFileScan未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0}", lsetInfo.ModelNM));
			}
		}

		//2016/4/28 この関数はまだ開始時の出力で使えてないので使うように将来修正 n.yoshimoto
		protected virtual void OutputResult(LSETInfo lsetInfo, string targetFileIdentity, string outputPath, CheckResult chkResult, bool isStartTiming)
		{
			string timingMsg = string.Empty;

			if (isStartTiming)
			{
				timingMsg = "開始";
			}
			else
			{
				timingMsg = "完了";
			}

			//判定結果出力

			//BBSQの検証時に完了時OKファイルが出力されていない事が原因で、装置側トリガが落ちていないことが判明
			//19ラインのレーザスクライブではOKファイルが完了時に出ており、下記の完了時にEDP、BWM以外は出力しないという根拠不明
			//BBSQの事もあるのでひとまず、下記はコメントアウト
			//if (isStartTiming == false)
			//{
			//	//完了時はEDPMachineInfo、BWMMachineInfoを除き判定結果ファイルの出力は行わない 2016/4/28 n.yoshimoto
			//}
			//else if (chkResult.ErrorInfo.Count > 0)

			if (chkResult.ErrorInfo.Count > 0)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0}時処理 判定結果NG出力 {1}/{2}/{3} ﾄﾘｶﾞﾌｧｲﾙ識別:{4} ", timingMsg, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
				base.errorMessageList.AddRange(chkResult.ErrorInfo);

				string errMsg = string.Empty;

				foreach (ErrMessageInfo errMsgInfo in chkResult.ErrorInfo)
				{
					errMsg += errMsgInfo.MessageVAL + "\r\n";
				}

                //BBSQ等のPLC連動ソフトで問題があったので巻き戻し
                ////完了時処理では装置へのOK/NG応答は不要なためファイル出力無し
                //if(isStartTiming == true)
                CIFS.OutputResultFile(outputPath, targetFileIdentity, errMsg, false);
			}
			else
            {
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0}時処理 判定結果OK出力 {1}/{2}/{3} ﾄﾘｶﾞﾌｧｲﾙ識別:{4} ", timingMsg, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
                
                ////BBSQ等のPLC連動ソフトで問題があったので巻き戻し
                ////完了時処理では装置へのOK/NG応答は不要なためファイル出力無し
                //if (isStartTiming == true)
                CIFS.OutputResultFile(outputPath, targetFileIdentity, string.Empty, true);
			}

			if (isStartTiming == false)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
					, string.Format("完了時処理 閾値判定結果のDB登録開始 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} "
					, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

				foreach (Log log in chkResult.RegistrationData)
				{
					ConnectDB.InsertTnLOG(log, lsetInfo);
				}

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
					, string.Format("完了時処理 閾値判定結果のDB登録完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} "
					, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
			}
		}

		public virtual void BackupFileEndTiming(string targetFileIdentity, string lotNo, DateTime dt)
		{
			// 完了フォルダ内のファイル群の取得
			List<string> backupFileList = GetBackupTargetFile(targetFileIdentity);

			BackupDoneEndFiles(backupFileList, EndFileDir, lotNo, dt);
		}

		/// <summary>
		/// 19ライン立ち上げ時のCIFSのベース仕様にLENS処理待ち関数追加（19ライン立ち上げ時のCIFSベース仕様としては常にtrueを返す）継承する側の装置で必要に応じて中身記述
		/// </summary>
		/// <param name="lotNo"></param>
		/// <param name="procNo"></param>
		/// <param name="lineCD"></param>
		/// <returns></returns>
		protected virtual bool IsFinishedLENSProcess(string lotNo, int procNo, int lineCD, string carrierNo, string plantCd)
		{
			return true;
		}

		void SendStopSignalToMachine()
		{

		}

		protected virtual bool IsFinishedFile(string fileDir)
		{
			List<string> filePathList = new List<string>();
		
			filePathList.AddRange(Common.GetFiles(fileDir, ".*\\." + CIFS.EXT_FIN_FILE));

			if (filePathList.Count() > 0)
			{
				return true;
			}
			else 
			{
				return false;
			}
		}

		protected virtual bool IsFinishedFile(string fileDir, string ext)
		{
			List<string> filePathList = new List<string>();

			filePathList.AddRange(Common.GetFiles(fileDir, string.Format(".*\\.{0}$", ext)));

			if (filePathList.Count() > 0)
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		protected virtual void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			//追加処理が必要な装置のオーバーライド用（派生クラスでオーバーライドする)
		}

        //参照先を見ていたらわざわざ指定していなかったprocnoを全部もってたので引数を変えてこのオーバーロードは廃止
        //public static void CountDefect(string filePath, int lineCD, string lotNo, string magNo, string plantCD, string targetFileIdentity, bool withProcNo)
        //{
        //    CountDefect(filePath, lineCD, lotNo, magNo, plantCD, null, targetFileIdentity, withProcNo);
        //}

        /// <summary>
        /// カンマ区切りでパッケージの状態CDが記録された(20160304 吉本/本関数作成時に想定しているのはmpdファイル)ファイルから不良集計する関数
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="lotNo"></param>
        /// <param name="typeCd"></param>
        /// <param name="modelNm"></param>
        /// <param name="procNo"></param>
        public static void CountDefect(string filePath, int lineCD, string lotNo, string magNo, string plantCD, long procNo, string targetFileIdentity, bool withProcNo)
		{
			List<NascaDefectFile.DefectQtyInfo> nascaDefectCtList = new List<NascaDefectFile.DefectQtyInfo>();

			Dictionary<string, int> defectCtList = GetDefectCount(filePath);

			//typeCd, modelNm, procNoからパッケージ状態CD⇒NASCA不良・分類・起因CDへの変換マスタを取得
			List<ErrConv> errConvList = ErrConv.GetData(plantCD, lineCD, procNo, withProcNo);

			foreach (KeyValuePair<string, int> defectInfo in defectCtList)
			{
				if (errConvList.Count(e => e.EquiErr == defectInfo.Key) != 1)
				{
					throw new ApplicationException(string.Format(
						"ﾏｽﾀ(EICS.TmErrConv)から取得した情報が1件(存在しないか複数ある為)に絞れません。ﾏｽﾀ担当へ連絡下さい。 設備:{0} ﾛｯﾄ:{1} 設備出力不良CD:{2}", plantCD, lotNo, defectInfo.Key));
				}

				ErrConv errConvMaster = errConvList.Single(e => e.EquiErr == defectInfo.Key);

				if (errConvMaster.NotOutputNasFG)
				{
					continue;
				}

				NascaDefectFile.DefectQtyInfo nascaDefectCt = new NascaDefectFile.DefectQtyInfo();

				nascaDefectCt.ClassCd = errConvMaster.ClassCD;
				nascaDefectCt.CauseCd = errConvMaster.CauseCD;
				nascaDefectCt.ErrCd = errConvMaster.ItemCD;
				nascaDefectCt.Qty = defectInfo.Value;

				nascaDefectCtList.Add(nascaDefectCt);
			}

			//nasファイルへ書き出し
			NascaDefectFile.Create(lotNo, magNo, plantCD, lineCD, procNo, nascaDefectCtList, targetFileIdentity);
		}

		private static Dictionary<string, int> GetDefectCount(string filePath)
		{
			//ファイルから全パッケージの状態CDのリスト取得
			string[] fileTxt;

			if (MachineFile.TryGetTxt(filePath, LF, out fileTxt, "ASCII") == false)
			{
				throw new ApplicationException(string.Format("ﾌｧｲﾙを読み取れませんでした。ﾌｧｲﾙﾊﾟｽ:{0}", filePath));
			}

            //mpdファイルが複数行に分かれていても全部処理されるように変更。20170822
            List<string> machineDefectCd = new List<string>();
            for (int i = 0; i < fileTxt.Length; i++)
            {
                if(String.IsNullOrWhiteSpace(fileTxt[i]) == false)
                {
                    machineDefectCd.AddRange(fileTxt[i].Split(',').ToList());
                }
            }

			//各不良の集計
			Dictionary<string, int> defectCtList = new Dictionary<string, int>();

			List<string> defectList = machineDefectCd.Distinct().ToList();

			foreach (string defect in defectList)
			{
				if (defect != Constant.MAP_AI_OK)
				{
					int defectCt = machineDefectCd.Count(m => m == defect);
					defectCtList.Add(defect, defectCt);
				}
			}

			return defectCtList;
		}

		public static void OutputResultFile(LSETInfo lsetInfo, List<ErrMessageInfo> errMsgList, string targetDir, string targetFileID, bool isStartup)
		{
			string processTimingStr;

			if(isStartup)
			{
				processTimingStr = "開始時";
			}
			else
			{
				processTimingStr = "完了時";
			}

			//判定結果出力
			if (errMsgList.Count > 0)
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
					, $"{processTimingStr}処理 判定結果NG出力 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileID} ");

				string errMsg = string.Empty;

				foreach (ErrMessageInfo errMsgInfo in errMsgList)
				{
					errMsg += errMsgInfo.MessageVAL + "\r\n";
				}

				CIFS.OutputResultFile(targetDir, targetFileID, errMsg, false);
			}
			else
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO
					, $"{processTimingStr} 判定結果OK出力 {lsetInfo.ModelNM}/{lsetInfo.EquipmentNO}/{lsetInfo.MachineSeqNO} ﾄﾘｶﾞﾌｧｲﾙ識別:{targetFileID} ");
				CIFS.OutputResultFile(targetDir, targetFileID, string.Empty, true);
			}
		}
    }
}
