using EICS.Database;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace EICS.Machine
{
	class BBMachineInfo2 : BBMachineInfo
	{
		private const string FOLDER_IN_NM = "Input";
		private const string FOLDER_OUT_NM = "Output";
		private const int RESULT_MSG_LEN = 200;
		private const string FILE_SEARCH_PATTERN = "^M.[0-9]{16}_.*";

		public BBMachineInfo2(LSETInfo lsetInfo)
		{
			this.LineCD = lsetInfo.InlineCD;
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);

			WaitForRenameByArmsFG = settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO);

			CheckDirectory(lsetInfo);

			string inputDirPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_IN_NM);
			if (!Directory.Exists(inputDirPath))
			{
				Directory.CreateDirectory(inputDirPath);
			}

			string outputDirPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_OUT_NM);
			if (!Directory.Exists(outputDirPath))
			{
				Directory.CreateDirectory(outputDirPath);
			}
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);
				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				bool isOutputCIFSResult = settingInfoPerLine.IsOutputCIFSResult(lsetInfo.EquipmentNO);

#if Debug
            //開発者が確認する場合、保存先を変える
			lsetInfo.InputFolderNM = @"C:\QCIL\data\WB\" + lsetInfo.EquipmentNO + @"\";
#else
				if (settingInfo.KissFG == "ON")
				{
					if (!KLinkInfo.CheckKISS())
					{
						F01_MachineWatch.sp.PlayLooping();
						ErrMessageInfo errMessageInfo = new ErrMessageInfo(string.Format(Constant.MessageInfo.Message_6, this.LineCD), Color.Red);
						base.errorMessageList.Add(errMessageInfo);
						base.machineStatus = Constant.MachineStatus.Stop;
						return;
					}
				}
#endif

				CheckInput(lsetInfo, ref base.errorMessageList);

				BackupMpd(lsetInfo, FOLDER_OUT_NM);

				CheckOutput(lsetInfo, ref base.errorMessageList);

			}
			catch (Exception err)
			{
				throw;
			}

			base.machineStatus = Constant.MachineStatus.Runtime;
			return;
		}

		/// <summary>
		/// スタートタイミング処理
		/// </summary>
		/// <param name="lsetInfo"></param>
		/// <param name="errMessageList"></param>
		public void CheckInput(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			MagInfo magInfo = new MagInfo();
			magInfo.sMagazineNO = "";
			magInfo.sNascaLotNO = null;

			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);
			magInfo.sMaterialCD = settingInfoPerLine.GetMaterialCD(lsetInfo.EquipmentNO);

			try
			{
				string sFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_IN_NM);
				List<string> fileList = MachineFile.GetPathList(sFolderPath, string.Format("^{0}", "^" + Convert.ToString(FileType.SP)));

				DateTime? latestMeasureDT = null;
				string latestWstFile = string.Empty;

				List<DateTime> measureDtList = new List<DateTime>();


				List<string> wstFileList = Common.GetFiles(sFolderPath, string.Format(".*{0}$", CIFSBasedMachine.EXT_START_FILE));

				if (wstFileList.Count == 0)
				{
					return;
				}

				latestWstFile = wstFileList.OrderByDescending(w => Path.GetFileName(w)).ToList()[0];

				foreach (string wstFile in wstFileList)
				{
					FileInfo fileInfo = new FileInfo(wstFile);

					DateTime dt;

					string fileNm = Path.GetFileNameWithoutExtension(fileInfo.Name);
					if (DateTime.TryParseExact(fileNm, "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.AssumeLocal, out dt) == false)
					{
						throw new ApplicationException(string.Format(
							"wstファイル名をyyyyMMddHHmmssの形式で日時変換出来ませんでした。変換対象:{0}", fileNm));
					}

					measureDtList.Add(dt);
				}

				latestMeasureDT = measureDtList.OrderByDescending(m => m).ToList()[0];


				foreach (string sFile in fileList)
				{
					FileInfo fileInfo = new FileInfo(sFile);
					//if (fileInfo.Name.Substring(0, 1) == "_")
					//{
					//	continue;
					//}

					DateTime dtMeasureDT = GetFileName_MeasureDT(fileInfo.Name);

					if (latestMeasureDT.HasValue && latestMeasureDT.Value != dtMeasureDT)
					{
						continue;
					}

					//SPファイル処理
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}File", lsetInfo.EquipmentNO, "SP"));
					List<ErrMessageInfo> errMsgList = DbInput_WB_SPFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT.ToString("yyyy/MM/dd HH:mm:ss"), Constant.nStartTimming);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}File", lsetInfo.EquipmentNO, "SP"));

					errMessageList.AddRange(errMsgList);


					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}", lsetInfo.EquipmentNO, "Output CIFS Result"));
					OutputCIFSResult(latestWstFile, errMsgList, RESULT_MSG_LEN);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}", lsetInfo.EquipmentNO, "Output CIFS Result"));
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove file:{2}", lsetInfo.EquipmentNO, "SP", sFile));
					MoveCompleteMachineFile(fileInfo, dtMeasureDT);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}FileMove file:{2}", lsetInfo.EquipmentNO, "SP", sFile));

					continue;
						//CIFS仕様の場合はファイルリネームはせず、処理後に即ファイルを移動する。

					//処理済みファイル名を変更する
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove", lsetInfo.EquipmentNO, "SP"));
					//ChangeCompleteMachineFile(fileInfo.FullName, ref errMessageList);
					MoveCompleteMachineFile(fileInfo, dtMeasureDT);

					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}FileMove", lsetInfo.EquipmentNO, "SP"));
				}


				fileList = MachineFile.GetPathList(sFolderPath, string.Format("^{0}", "^" + Convert.ToString(FileType.SP)));

				if (fileList.Count() > 0)
				{
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("警告 {0} WB 全wstﾌｧｲﾙ処理後もﾌｧｲﾙ名の日時がwstに対応しないSPﾌｧｲﾙが残っています。", lsetInfo.EquipmentNO));

					foreach (string sFile in fileList)
					{
						log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("{0} WB 未処理ﾌｧｲﾙ{1}", lsetInfo.EquipmentNO, sFile));
					}
				}

			}
			catch (Exception err)
			{
				throw;
			}
		}

		/// <summary>
		/// 完了時出力ファイル処理
		/// </summary>
		/// <param name="lsetInfo"></param>
		/// <param name="errMessageList"></param>
		public void CheckOutput(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			MagInfo magInfo = new MagInfo();

			try
			{
				string folderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_OUT_NM);
				//M○□□□YYMMDDHHmmSSから始まるARMSリネーム済みファイルを抽出
				List<string> fileList = MachineFile.GetPathList(folderPath, FILE_SEARCH_PATTERN);
				if (fileList.Count == 0) return;


				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				SettingInfo commonSetting = SettingInfo.GetSingleton();
				
				//#endif 
				//ファイル処理
				foreach (string file in fileList)
				{
					CIFSBasedMachine.FileNameAdditionInfo fileNmAddInfo = CIFSBasedMachine.GetInfoFromFileNm(file, CIFS.FILE_NM_SPLITTER, CIFSBasedMachine.IDENTITY_INDEX_IN_FILENM
						, CIFSBasedMachine.TYPECD_INDEX_IN_FILENM, CIFSBasedMachine.LOTNO_INDEX_IN_FILENM
						, CIFSBasedMachine.MAGNO_INDEX_IN_FILENM, CIFSBasedMachine.PROC_INDEX_IN_FILENM);

					magInfo.sNascaLotNO = fileNmAddInfo.LotNo;
					magInfo.sMagazineNO = fileNmAddInfo.MagNo;
					magInfo.sMaterialCD = fileNmAddInfo.TypeCd;
					magInfo.ProcNO = fileNmAddInfo.ProcNo;

					FileInfo fileInfo = new FileInfo(file);
					string dtMeasureDT = Convert.ToString(GetFileName_MeasureDT(fileInfo.Name));

					string fileChar = GetMachineFileChar(fileInfo.FullName, lsetInfo.AssetsNM);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}File", lsetInfo.EquipmentNO, fileChar));
					switch (fileChar)
					{
						case "SP":
							List<ErrMessageInfo> errMsgList = DbInput_WB_SPFile_OLD(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, Constant.nMagazineTimming);
							errMessageList.AddRange(errMsgList);
							break;
						case "MP":

							if (magInfo.sNascaLotNO == null)
							{
								Lott.SetTnLOTT(lsetInfo, "不明");
							}
							else
							{
								Lott.SetTnLOTT(lsetInfo, magInfo.sNascaLotNO);
							}

							DbInput_WB_MPFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, ref errMessageList);
							break;
						case "ML":
							DbInput_WB_MLFile(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, ref errMessageList);
							break;
						case "MM":
							//処理無し
							break;
						case "ME":
							//処理無し
							break;
					}
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]  {0} WB {1}File", lsetInfo.EquipmentNO, fileChar));

					//処理済みファイルを保管フォルダへ移動
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove", lsetInfo.EquipmentNO, fileChar));
					string sFileStamp = Convert.ToDateTime(dtMeasureDT).ToString("yyyyMMddHHmmss");
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, sFileStamp);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]  {0} WB {1}FileMove", lsetInfo.EquipmentNO, fileChar));
				}

				//QCNRに異常履歴を挿入
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]WB:CheckQC");
				CheckQC(lsetInfo, Constant.TIM_BB, magInfo.sMaterialCD);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]  WB:CheckQC");
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}
}
