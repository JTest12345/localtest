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
	/// フルオート圧縮成型(CO:COmpression molding [SL47略称参照])
	/// </summary>
	class COMachineInfo : CIFSBasedMachine
	{
		protected override int QC_TIMING_NO_ZD() { return Constant.TIM_CO; }
		protected override int QC_TIMING_NO_LED() { return Constant.TIM_CO; }

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
				if(IsStartableProcess())
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
	}
}
