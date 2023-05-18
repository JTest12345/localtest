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
	class BBMachineInfo : WBMachineInfo
	{
		public BBMachineInfo()
		{
		}

		public BBMachineInfo(int lineCD, string equipNO)
		{
			this.LineCD = lineCD;
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lineCD);

			WaitForRenameByArmsFG = settingInfoPerLine.GetWaitForRenameByArmsFG(equipNO);
		}

		/// <summary>
		/// マガジンタイミング処理
		/// </summary>
		/// <param name="lsetInfo"></param>
		/// <param name="errMessageList"></param>
		public override void CheckMagazineTiming(LSETInfo lsetInfo, ref List<ErrMessageInfo> errMessageList)
		{
			MagInfo magInfo = new MagInfo();

			try
			{
				bool isNotFoundMEFile = true;

				string runFolderPath = Path.Combine(lsetInfo.InputFolderNM, FOLDER_RUN_NM);
				List<string> fileList = MachineFile.GetPathList(runFolderPath);
				if (fileList.Count == 0) return;

				//MPファイルの更新日時を取得
				string dtFileStamp = KLinkInfo.GetFileStampDT(runFolderPath, Convert.ToString(FileType.MP));//1マガジン単位ファイルのファイルスタンプ取得
				dtFileStamp = Convert.ToString(Convert.ToDateTime(dtFileStamp).AddMinutes(-5));//公開API(装置・時間)の時間として使用(ファイルスタンプ-5分)

				List<string> mmFileList = fileList.Where(f => Regex.IsMatch(Path.GetFileNameWithoutExtension(f), @"^MM.*_.*_.*$")).ToList();

				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
				SettingInfo commonSetting = SettingInfo.GetSingleton();

				if (commonSetting.IsMappingMode)
				{
					//MMファイルからマガジン情報取得
					magInfo = GetMagazineInfo(mmFileList[0]);
				}
				else
				{
					//ARMSからマガジン情報取得
					magInfo = GetMagInfo(lsetInfo, dtFileStamp, false);

					if (magInfo == null)
					{
						return;
					}
				}

				if (magInfo.sNascaLotNO == null)
				{
					Lott.SetTnLOTT(lsetInfo, "不明");
				}
				else
				{
					Lott.SetTnLOTT(lsetInfo, magInfo.sNascaLotNO);
				}
				//#endif 
				//ファイル処理
				foreach (string file in fileList)
				{
					FileInfo fileInfo = new FileInfo(file);
					string dtMeasureDT = Convert.ToString(GetFileName_MeasureDT(fileInfo.Name));

					string fileChar = GetMachineFileChar(fileInfo.FullName, lsetInfo.AssetsNM);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] {0} WB {1}File", lsetInfo.EquipmentNO, fileChar));
					switch (fileChar)
					{
						case "SP":
							List<ErrMessageInfo> errMsgList = DbInput_WB_SPFile_OLD(lsetInfo, magInfo, fileInfo.FullName, dtMeasureDT, Constant.nMagazineTimming);
							errMessageList.AddRange(errMsgList);
							break;
						case "MP":
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
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START] {0} WB {1}File", lsetInfo.EquipmentNO, fileChar));

					//処理済みファイルを保管フォルダへ移動
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} WB {1}FileMove", lsetInfo.EquipmentNO, fileChar));
					string sFileStamp = Convert.ToDateTime(dtFileStamp).ToString("yyyyMMddHHmmss");
					MoveCompleteMachineFile(fileInfo.FullName, lsetInfo, magInfo.sNascaLotNO, sFileStamp);
					log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} WB {1}FileMove", lsetInfo.EquipmentNO, fileChar));
				}

				if (settingInfoPerLine.IsOutputNasFile(lsetInfo.EquipmentNO) && isNotFoundMEFile)
				{
					ErrMessageInfo errMsg = new ErrMessageInfo(string.Format("ロット：{0} エラー情報集計ONですが、MEファイルが存在しません。不良を確認・登録して下さい。", magInfo.sNascaLotNO), Color.Red);
					errMessageList.Add(errMsg);
				}

				//QCNRに異常履歴を挿入
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[START]WB:CheckQC");
				CheckQC(lsetInfo, 5, magInfo.sMaterialCD);//5はWBの意味
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, lsetInfo.EquipmentNO + "/[END]WB:CheckQC");
			}
			catch (Exception err)
			{
				throw;
			}
		}
	}
}
