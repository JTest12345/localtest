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
	/// <summary>
	/// ブレイク(BK:BreaK [SL47略称参照])
	/// </summary>
	class BKMachineInfo : CIFSBasedMachine
	{
		protected override int QC_TIMING_NO_ZD() { return Constant.TIM_BK; }
		protected override int QC_TIMING_NO_LED() { return Constant.TIM_BK; }

		private const int TARGET_DATA_COL_INDEX = 0;
		private const int TARGET_DATA_START_NO = 1;

		public override bool StartingProcess(LSETInfo lsetInfo)
		{
			string targetFileIdentity = string.Empty;
			try
			{
				List<Log> logList = new List<Log>();

				List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				DateTime doneDt = DateTime.MinValue;

				//処理開始して良いかトリガを確認 （装置出力ファイルが1ファイルの場合はトリガファイルが出無い仕様らしいので、IsStartableProcess()でチェック）
				if (IsStartableProcess())
					targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false);

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

						if (fileNmElement.Count() <= FileKindIndexInFileNm)
						{
							fileNmElement = new string[] { Path.GetFileNameWithoutExtension(fileNm), string.Empty };
						}

						string fileKind = fileNmElement[FileKindIndexInFileNm];

						List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, fileKind, true);

						FileScan fileScan;

						if (fileScanList.Count == 0)
						{
							log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
								"TmFILESCANからの取得ﾚｺｰﾄﾞ0の為判定処理ｽｷｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ﾌｧｲﾙ種:{4}"
								, lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, fileKind));
							continue;
						}
						else if (fileScanList.Count == 1)
						{
							fileScan = fileScanList.Single();
						}
						else
						{
							throw new ApplicationException(
								string.Format("TmFileScanから複数のマスタが取得されました。マスタ管理者に連絡して下さい。設備:{0} ファイル種類:{1}", lsetInfo.ModelNM, fileKind));
						}

						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[START] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));

						//ファイル名から日時を取得
						DateTime? dtFromFileNm = CIFS.GetDateFromFileNm(fileNm, DateIndex, DateLen);

						if (dtFromFileNm.HasValue == false)
						{
							throw new ApplicationException(
								string.Format("ファイル名から日時の取得に失敗 設備:{0} {1} ﾌｧｲﾙ:{2} 日付取得開始Index:{3}　日付文字長さ:{4}"
								, lsetInfo.ModelNM, lsetInfo.EquipmentNO, fileNm, DateIndex, DateLen));
						}

						if (doneDt < dtFromFileNm.Value)
						{
							doneDt = dtFromFileNm.Value;
						}

						string filePath = Path.Combine(StartFileDir, fileNm);

						if (MachineFile.CheckMachineFile(filePath, LF, lsetInfo.TypeCD, string.Empty, string.Empty, dtFromFileNm.Value, fileScan, lsetInfo, lsetInfo.ChipNM, ref errMsgList, EncodeStr, ref logList) == false)
						{
							continue;
						}
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(chkTargetFilePath)));

					}

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

					//foreach (Log log in logList)
					//{
					//	ConnectDB.InsertTnLOG(log, lsetInfo);
					//}

					// ※finファイルが出力されない装置不具合が有る為無効に。出力されるようになったら有効にする。
					//chkTargetFileList.Add(Path.Combine(StartFileDir, string.Join(".", targetFileIdentity, EXT_TRIG_FILE)));

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
					// 処理したファイルの移動
					CIFS.BackupDoneFiles(chkTargetFileList, StartFileDir, string.Empty, doneDt);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

					return true;
				}
			}
			catch (Exception err)
			{
				if (string.IsNullOrEmpty(targetFileIdentity))
				{
					string errMsg = string.Format("EICSで開始処理時に予期せぬエラーが発生しました。{0}", err.Message);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるSTOP出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\nStackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
					CIFS.OutputStopFile(StartFileDir, DateTime.Now.ToString("yyyyMMddHHmmss"), errMsg, false);
				}
				else
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 例外発生によるNG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3}\r\nStackTrace:{4}", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity, err.StackTrace));
					CIFS.OutputResultFile(StartFileDir, targetFileIdentity, err.Message, false);
				}

				throw;
			}

			return false;
		}

		//(2015/10/1 吉本)
		//ブレンドロットをアッセンロットに分解して各アッセンロットとファイル内データを紐付ける仕様取り消しとなった為、一先ずコメントアウト。
		//処理内容を確認した結果、baseのEndingProcessと同じ処理で問題無さそうな為、EndingProcessのoverrideをやめた
		//protected override void EndingProcess(LSETInfo lsetInfo, int? timingNo)
		//{
		//	try
		//	{
		//		List<ErrMessageInfo> errMsgList = new List<ErrMessageInfo>();

		//		bool doneFG = false;
		//		string typeCD = string.Empty;

		//		//(2015/8/6 吉本)
		//		// ロット、タイプ、工程Noがファイル名に付与される。付与された事がEICSで処理していいというトリガ。
		//		// 出力ファイルの存在チェック関数呼び出し
		//		string targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(EndFileDir, EXT_TRIG_FILE, DateIndex, DateLen, true);

		//		//ブレンド作業を完了しているタイミングで処理出来るようにする
		//		//（ブレンド作業を実施しているかどうかは問題では無く、ブレンドしている場合に、ブレンド情報を参照出来る事が必要）
		//		//【恐らく、ファイルがあるからと言って処理した場合】ブレンド情報が手入力の為、ブレンド情報を得る前に、
		//		//情報が存在しない事をブレンドをしなかったと判断して処理してしまう。のでそうならないようにする必要有

		//		//処理可能な出力ファイルが有れば、ファイル毎に処理する関数
		//		if (string.IsNullOrEmpty(targetFileIdentity) == false)
		//		{
		//			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 トリガ確認 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

		//			List<string> trigFilePathList = Common.GetFiles(EndFileDir, string.Format("{0}.*\\.{1}", targetFileIdentity, EXT_TRIG_FILE));
		//			if (trigFilePathList.Count > 1)
		//			{
		//				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.FATAL,
		//					string.Format("同一のファイル種類、年月日時分秒文字列でファイルが複数抽出されました。 監視Dir:{0}　年月日時分秒文字列:{1}",
		//					EndFileDir, targetFileIdentity));
		//			}
		//			string trigFilePath = trigFilePathList.Single();
		//			string trigFileNm = Path.GetFileNameWithoutExtension(trigFilePath);

		//			DateTime dt;
		//			if (DateTime.TryParseExact(targetFileIdentity, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dt) == false)
		//			{
		//				throw new ApplicationException(
		//							string.Format("ファイル名から日時の取得が出来ませんでした。ファイル名:{0}", trigFilePathList.Single()));
		//			}

		//			if (MachineFile.TryGetElementFromFileNm(trigFileNm, FileNmSplitter, TypeCdIndexInFileNm, null, out typeCD) == false)
		//			{
		//				throw new ApplicationException(
		//					string.Format("ファイル名からタイプCDの取得が出来ませんでした。ファイル名:{0}　区切り文字:{1}　タイプCDを取得するインデクス:{2}", trigFilePath, FileNmSplitter, TypeCdIndexInFileNm));
		//			}

		//			string lotNO;
		//			if (MachineFile.TryGetElementFromFileNm(trigFileNm, FileNmSplitter, LotNoIndexInFileNm, null, out lotNO) == false)
		//			{
		//				throw new ApplicationException(
		//					string.Format("ファイル名からロットNoの取得が出来ませんでした。ファイル名:{0}　区切り文字:{1}　ロットNoを取得するインデクス:{2}", trigFilePath, FileNmSplitter, LotNoIndexInFileNm));
		//			}

		//			string magNO;
		//			if (MachineFile.TryGetElementFromFileNm(trigFileNm, FileNmSplitter, MagNoIndexInFileNm, null, out magNO) == false)
		//			{
		//				throw new ApplicationException(
		//					string.Format("ファイル名からマガジンNoの取得が出来ませんでした。ファイル名:{0}　区切り文字:{1}　マガジンNoを取得するインデクス:{2}", trigFilePath, FileNmSplitter, MagNoIndexInFileNm));
		//			}

		//			//処理対象ファイルの種類毎の情報を取得
		//			List<FileScan> fileScanList = FileScan.GetDataList(lsetInfo.InlineCD, lsetInfo.ModelNM, null, false);

		//			//ブレンドされたロットNoの取得
		//			//List<CutBlend> cutBlendList = CutBlend.GetData(lsetInfo.InlineCD, lotNO, lsetInfo.EquipmentNO).OrderBy(c => c.StartDT).ToList();

		//			//下記のfileScan内でロット毎の処理を出来れば良いのではないか(2015/8/6 吉本)
		//			foreach (FileScan fileScan in fileScanList)
		//			{
		//				string fileSearchPattern = Common.GetSearchPatternStr(DateIndex, targetFileIdentity, true) + fileScan.PrefixNM + @"_.*\." + EXT_END_FILE;

		//				List<string> fileNmList = Common.GetFiles(EndFileDir, fileSearchPattern);

		//				if (fileNmList.Count == 0)
		//				{
		//					continue;
		//				}
		//				else if (fileNmList.Count > 1)
		//				{
		//					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.WARN,
		//						string.Format("同一のファイル種類、年月日時分秒文字列でファイルが複数抽出されました。 検索Dir:{0} ﾌｧｲﾙ検索ﾊﾟﾀｰﾝ:{1}",
		//						EndFileDir, fileSearchPattern));
		//				}
		//				//日付文字列とファイル種類が同じで複数ファイルが取れたらどうする？

		//				string fileNm = fileNmList.Single();
		//				string filePath = Path.Combine(EndFileDir, fileNm);

		//				//(2015/8/6 吉本)
		//				//タイプ、ロットNo、マガジンNo、タイプ列インデクス、ロットNo列インデクス、マガジンNo列インデクスを必要に応じて取得
		//				//あるいは今回は列インデクスは決め打ちでも良いかと…
		//				//取得したインデクスの数量分、foreachで下にあるMachineFile.CheckMachineFile()を実行
		//				//(2015/10/1 吉本)
		//				//ブレンドロットをアッセンロットに分解して各アッセンロットとファイル内データを紐付ける仕様取り消しとなった為、一先ずコメントアウト。
		//				//foreach (CutBlend cutBlend in cutBlendList)
		//				//{
		//				//	log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理[START] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));

		//				//	string targetData = (cutBlendList.IndexOf(cutBlend) + TARGET_DATA_START_NO).ToString();

		//				//	//(2015/8/6 吉本)
		//				//	//列インデクスの引数を追加して呼び出し
		//				//	if (MachineFile.CheckMachineFile(filePath, LF, typeCD, lotNO, magNO, dt, fileScan, lsetInfo, lsetInfo.ChipNM, ref base.errorMessageList, EncodeStr, targetData, TARGET_DATA_COL_INDEX) == false)
		//				//	{
		//				//		log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理 ﾌｧｲﾙ内容判定処理が未完了のまま終了 {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));
		//				//		continue;
		//				//	}
		//				//	log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時各ﾌｧｲﾙ処理[End] {0}/{1}/{2} {3}File", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, Path.GetFileName(filePath)));
		//				//}

						
		//			}

		//			//判定結果出力
		//			if (errMsgList.Count > 0)
		//			{
		//				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果NG出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
		//				base.errorMessageList.AddRange(errMsgList);

		//				string errMsg = string.Empty;

		//				foreach (ErrMessageInfo errMsgInfo in errMsgList)
		//				{
		//					errMsg += errMsgInfo.MessageVAL + "\r\n";
		//				}

		//				CIFS.OutputResultFile(EndFileDir, targetFileIdentity, errMsg, false);
		//			}
		//			else
		//			{
		//				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("開始時処理 判定結果OK出力 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
		//				CIFS.OutputResultFile(EndFileDir, targetFileIdentity, string.Empty, true);
		//			}

		//			// 完了フォルダ内のファイル群の取得
		//			List<string> fileListInEndDir = Common.GetFiles(EndFileDir, string.Format("{0}*", targetFileIdentity));
		//			fileListInEndDir = fileListInEndDir
		//				.Where(f => Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_OK_FILE) && Path.GetExtension(f) != string.Format(".{0}", CIFS.EXT_NG_FILE)).ToList();

		//			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 ﾌｧｲﾙﾊﾞｯｸｱｯﾌﾟ {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));

		//			CIFS.BackupDoneFiles(fileListInEndDir, EndFileDir, lotNO, dt);

		//			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("完了時処理 完了 {0}/{1}/{2} ﾄﾘｶﾞﾌｧｲﾙ識別:{3} ", lsetInfo.ModelNM, lsetInfo.EquipmentNO, lsetInfo.MachineSeqNO, targetFileIdentity));
		//			doneFG = true;
		//		}

		//		if (timingNo.HasValue && doneFG)
		//		{
		//			CheckQC(lsetInfo, timingNo.Value, typeCD);
		//		}
		//	}
		//	catch (Exception err)
		//	{
		//		throw;
		//	}
		//}
	}
}
