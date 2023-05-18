using ArmsApi.Model;
using EICS.Arms;
using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	class FCMachineInfo : CIFSBasedMachine
	{
		protected string NonDefectiveProductExtractTargetFile = "ProductionControlBond";

		protected override int QC_TIMING_NO_ZD() { return Constant.TIM_ZDFC; }
		protected override int QC_TIMING_NO_LED() { return Constant.TIM_LEDFC; }

		//追加処理
		protected override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

            if (settingInfoPerLine.GetBMCount(lsetInfo.EquipmentNO))
            {
                //不良数集計の処理
                OutputNasFileProcess(lsetInfo.InlineCD, lotNO, magNO, equipNO, targetFileIdentity);
            }            
		}

        //メインルーチンから呼べる追加プロセス
        protected override void AdditionProcess(LSETInfo lsetInfo)
        {
            BackupMpd(lsetInfo);
        }

		private void OutputNasFileProcess(int lineCD, string lotNO, string magNO, string equipNO, string targetFileIdentity)
		{
			int defectCt = CalcDefect(lineCD, lotNO, targetFileIdentity);

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			List<NascaDefectFile.DefectQtyInfo> defectInfoList = new List<NascaDefectFile.DefectQtyInfo>();

			NascaDefectFile.DefectQtyInfo defectInfo = new NascaDefectFile.DefectQtyInfo();

			defectInfo.CauseCd = settingInfoPerLine.BMCauseCD;
			defectInfo.ClassCd = settingInfoPerLine.BMClassCD;
			defectInfo.ErrCd = settingInfoPerLine.BMDefectCD;
			defectInfo.Qty = defectCt;

			if (defectCt > 0)
			{
				defectInfoList.Add(defectInfo);
			}

			//不良情報を受け取ってファイル出力
			NascaDefectFile.Create(lotNO, magNO, equipNO, lineCD, defectInfoList);
		}

		private int CalcDefect(int lineCD, string lotNO, string targetFileIdentity)
		{
			//投入数取得
			EICS.Arms.AsmLot asmLot = EICS.Arms.AsmLot.GetAsmLot(lineCD, lotNO);
			Profile[] profArray = Profile.SearchProfiles(lineCD, asmLot.ProfileId, null, false, true);

			if (profArray.Count() == 0)
			{
				throw new ApplicationException(string.Format(
					"profileID:{0}でﾌﾟﾛﾌｧｲﾙ情報の取得が出来ませんでした。ﾛｯﾄ:{1}", asmLot.ProfileId, lotNO));
			}

			int inputPkg = profArray[0].LotSize;
			
			//実装数取得
			int okPkg = GetOkPkg(targetFileIdentity);

			//投入数 - 実装数で不良数取得
			int defectCt = inputPkg - okPkg;

			return defectCt;
		}

		private int GetOkPkg(string targetFileIdentity)
		{
			string[] txtArray;

			string fileSearchPattern = Common.GetSearchPatternStr(DateIndex, targetFileIdentity, true) + NonDefectiveProductExtractTargetFile + @"_.*\." + EXT_END_FILE();

			List<string> fileNmList = Common.GetFiles(EndFileDir, fileSearchPattern);

			if (fileNmList.Count == 0)
			{
				string errMsg = string.Format(
					"ﾌｧｲﾙが存在しません。 {0}/{1}/{2} 監視ﾊﾟｽ:{3} ﾌｧｲﾙ種:{4} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{5}"
					, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, EndFileDir, NonDefectiveProductExtractTargetFile, fileSearchPattern);

				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, errMsg);

				throw new ApplicationException(errMsg);
			}
			else if (fileNmList.Count > 1)
			{
				string errMsg = string.Format("同一のファイル種類、年月日時分秒文字列でファイルが複数抽出されました。 検索Dir:{0} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{1}",
					EndFileDir, fileSearchPattern);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN, errMsg);

				throw new ApplicationException(errMsg);

			}
			//日付文字列とファイル種類が同じで複数ファイルが取れたらどうする？

			string fileNm = fileNmList.Single();
			string filePath = Path.Combine(EndFileDir, fileNm);

			if (MachineFile.TryGetTxt(filePath, LF, out txtArray, EncodeStr, 0) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙ内容取得失敗 ﾌｧｲﾙﾊﾟｽ:{0} 行区切り文字{1}", filePath, @LF));
			}

			string[] dataArray = MachineFile.GetData(txtArray, 0, false, null, null);

			if(dataArray.Count() != 1)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙから実装数の取得が出来ませんでした。(ﾃﾞｰﾀ数が1つに限定して取得出来なかった) ﾃﾞｰﾀ数:{0}", dataArray.Count()));
			}

			int okPkgCt;

			if (int.TryParse(dataArray[0], out okPkgCt) == false)
			{
				throw new ApplicationException(
					string.Format("ﾌｧｲﾙのﾃﾞｰﾀが数値変換できませんでした。 変換対象ﾃﾞｰﾀ:{0}", dataArray[0]));
			}

			return okPkgCt;
		}

		public override bool StartingProcess(LSETInfo lsetInfo, out List<ErrMessageInfo> errMsgList)
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

					List<FILEFMTInfo> checkTargetFileFmtList = new List<FILEFMTInfo>(fileFmtList);
					checkTargetFileFmtList.AddRange(ConnectDB.GetFILEFMTData(null, lsetInfo, false));

					FileFmtWithPlm.CheckAllFileFmtFromParamMaster(plmPerTypeModelChipList, lsetInfo, false, checkTargetFileFmtList, null);

					List<FileFmtWithPlm> allFileFmtWithPlmList = FileFmtWithPlm.GetData(lsetInfo, true, plmPerTypeModelChipList, fileFmtList, null);

					// .wstファイル群の取得
					List<string> chkTargetFileList = Common.GetFiles(StartFileDir, EXT_START_FILE);

					//暫定版で判定OKﾌｧｲﾙを強制出力 2015/12/2 吉本
					//log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
					//CIFS.OutputResultFile(StartFileDir, targetFileIdentity, string.Empty, true);

					// ファイル内の装置パラメタチェック
					foreach (string chkTargetFilePath in chkTargetFileList)
					{
						string fileNm = Path.GetFileName(chkTargetFilePath);
						string[] fileNmElement = Path.GetFileNameWithoutExtension(fileNm).Split(FileNmSplitter);

						if (fileNmElement.Count() <= FileKindIndexInFileNm)
						{
							fileNmElement = new string[] { Path.GetFileNameWithoutExtension(fileNm), string.Empty };
						}

						string fileKind = fileNmElement[FileKindIndexInFileNm];

						List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, fileKind, true);

						FileScan fileScan;

						List<FileFmtWithPlm> fileFmtWithPlmList = allFileFmtWithPlmList.Where(f => f.FileFmt.PrefixNM == fileKind).ToList();
						fileFmtWithPlmList = fileFmtWithPlmList.FindAll(f => plmPerTypeModelChipList.Select(p => p.QcParamNO).Contains(f.Plm.QcParamNO));

						if (fileScanList.Count == 0 && fileFmtWithPlmList.Count == 0)
						{
							continue;
						}
						else if (fileScanList.Count > 0 && fileFmtWithPlmList.Count == 0)
						{
							throw new ApplicationException(
								string.Format("TmFileScanﾏｽﾀ設定済みですがTmFILEFMT未設定です。ﾏｽﾀ管理者に連絡して下さい。設備:{0} ﾌｧｲﾙ種類:{1}",
								lsetInfo.ModelNM, fileKind));
						}
						else if (fileScanList.Count == 0 && fileFmtWithPlmList.Count > 0)
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

						//ファイル名から日時を取得
						DateTime? dtFromFileNm = CIFS.GetDateFromFileNm(fileNm, DateIndex, DateLen);

						if (dtFromFileNm.HasValue == false)
						{
							throw new ApplicationException(
								string.Format("ファイル名から日時の取得に失敗 設備:{0} {1} ﾌｧｲﾙ:{2} 日付取得開始Index:{3}　日付文字長さ:{4}"
								, lsetInfo.ModelNM, lsetInfo.EquipmentNO, fileNm, DateIndex, DateLen));
							//continue;
						}

						if (doneDt < dtFromFileNm.Value)
						{
							doneDt = dtFromFileNm.Value;
						}

						string filePath = Path.Combine(StartFileDir, fileNm);

						MachineFile macFile = new MachineFile();
						if (macFile.CheckMachineFile_SpeedUp(fileFmtWithPlmList, filePath, LF, lsetInfo.TypeCD, string.Empty, string.Empty, dtFromFileNm.Value
							, fileScan, lsetInfo, lsetInfo.ChipNM, null, ref errMsgList, EncodeStr, ref logList) == false)
						{
							continue;
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));

					}

					// 判定結果の出力前にファイルのバックアップ処理を移動
					chkTargetFileList.Add(Path.Combine(StartFileDir, string.Join(".", targetFileIdentity, EXT_TRIG_FILE())));

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
					// 処理したファイルの移動
					CIFS.BackupDoneFiles(chkTargetFileList, StartFileDir, string.Empty, doneDt);

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

						CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false); //暫定版で判定結果ﾌｧｲﾙ出力を一時コメントアウト 2015/12/2 吉本
					}
					else
					{
						//暫定版で判定結果ﾌｧｲﾙ出力を一時コメントアウト 2015/12/2 吉本
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
						CIFS.OutputResultFile(StartFileDir, targetFileIdentity, string.Empty, true);
					}

					//foreach (Log log in logList)
					//{
					//	ConnectDB.InsertTnLOG(log, lsetInfo);
					//}

					chkTargetFileList.Add(Path.Combine(StartFileDir, string.Join(".", targetFileIdentity, EXT_TRIG_FILE())));

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
					// 処理したファイルの移動
					CIFS.BackupDoneFiles(chkTargetFileList, StartFileDir, string.Empty, doneDt);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

					return true;
				}

				return false;
			}
			catch (Exception err)
			{
				if (string.IsNullOrEmpty(targetFileIdentity))
				{
					string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるSTOP出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}}\r\nStackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
					CIFS.OutputStopFile(StartFileDir, DateTime.Now.ToString("yyyyMMddHHmmss"), errMsg, false);
				}
				else
				{
					string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるNG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\nStackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
					CIFS.OutputResultFile(StartFileDir, targetFileIdentity, errMsg, false);
				}

				throw;
			}
		}
	}
}
