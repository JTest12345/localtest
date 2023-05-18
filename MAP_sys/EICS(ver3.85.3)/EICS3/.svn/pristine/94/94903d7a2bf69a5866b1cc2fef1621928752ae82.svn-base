using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace EICS.Machine
{
	class DBMachineInfoNMC : DBMachineInfo
	{
		public const string ASSETS_NM = "Die Bonder";

		/// <summary>
		/// ファイルチェック(NMC)
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <returns>装置処理状態ステータス</returns>
		public void CheckFile(LSETInfo lsetInfo, ref Constant.MachineStatus machineStatus, ref List<ErrMessageInfo> errMessageList)
		{
			if (!Directory.Exists(lsetInfo.InputFolderNM))
			{
				//監視フォルダが存在しない場合、装置処理停止
				throw new Exception(string.Format(Constant.MessageInfo.Message_39, lsetInfo.InputFolderNM));
			}

			if (machineStatus == Constant.MachineStatus.Wait)
			{
				//初回起動時に溜まっているスタートファイルを[Reserve]フォルダに移動する
				string[] files = Directory.GetFiles(lsetInfo.InputFolderNM);
				foreach (string file in files)
				{
					//移動先の[Reserve]フォルダ名を生成
					FileInfo sFileInfo = new FileInfo(file);
					string dateDirNM = sFileInfo.LastWriteTime.ToString("yyyy/MM").Replace("/", "");
					string moveToFilePath = Path.Combine(sFileInfo.DirectoryName, dateDirNM);
					moveToFilePath = Path.Combine(moveToFilePath, "reserve");
					moveToFilePath = Path.Combine(moveToFilePath, sFileInfo.Name);

					MoveMachineFileNMC(file, moveToFilePath);
				}
				machineStatus = Constant.MachineStatus.Runtime;
			}

			//string a = Constant.SettingInfo.PackagePC;


			List<string> magazineIDList = GetMagazineIDData(false, lsetInfo.InputFolderNM, MachineTiming.Start, false);
			foreach (string magazineID in magazineIDList)
			{
				//1Set揃っているか確認
				CheckStartFileExist(lsetInfo.InputFolderNM, magazineID, null, false, string.Empty);

				//スタートタイミングの処理
				CheckStartTiming(lsetInfo, magazineID, ref errMessageList);
			}

			if (magazineIDList.Count > 0)   //2012/06/28追加 T.Sasaki　上で処理を変更したのでif分追加
			{
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ファイル作成前");
				//判定ファイルの作成
				if (errMessageList.Count != 0)
				{
					CreateJudgeFile(lsetInfo.InputFolderNM, 0);   //0=NG
				}
				else
				{
					CreateJudgeFile(lsetInfo.InputFolderNM, 1);   //1=OK
				}
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, "ファイル作成後");
			}

		}

		/// <summary>
		/// スタートタイミング処理(NMC)
		/// </summary>
		/// <param name="lsetInfo">装置情報</param>
		/// <param name="magazineID">マガジンID</param>
		/// <param name="errMessageList">異常内容リスト</param>
		/// <returns>装置処理状態ステータス</returns>
		private new void CheckStartTiming(LSETInfo lsetInfo, string magazineID, ref List<ErrMessageInfo> errMessageList)
		{
			//[O:装置設定パラメータ] / [P:Package data name] / [M:装置設定パラメータ拡張]

			MagInfo magInfo = new MagInfo();
			magInfo.sMagazineNO = "";
			magInfo.sNascaLotNO = null;
			SettingInfo settingInfo = SettingInfo.GetSettingInfoPerLine(this.LineCD);
			magInfo.sMaterialCD = settingInfo.GetMaterialCD(lsetInfo.EquipmentNO);


			List<string> sFileList = GetMachineStartFile(lsetInfo.InputFolderNM, magazineID);
			foreach (string sFile in sFileList)
			{
				//ファイル内処理(スタートタイミングでは異常判定だけ行い、管理項目内容はDB登録しない)
				string sFileType = GetMachineFileChar(sFile, lsetInfo.AssetsNM);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));
				switch (sFileType)
				{
					case "O":
						DbInput_DB_OFile(lsetInfo, magInfo, sFile, Constant.nStartTimmingNMC, ref errMessageList);
						break;
					case "P":
						DbInput_DB_PFile(lsetInfo, magInfo, sFile, Constant.nStartTimmingNMC, ref errMessageList);
						break;
					case "M":
						DbInput_DB_MFile(lsetInfo, magInfo, sFile, Constant.nStartTimmingNMC, ref errMessageList);
						break;
				}
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}File", lsetInfo.EquipmentNO, sFileType));

				//移動先の年月フォルダ名を生成
				FileInfo sFileInfo = new FileInfo(sFile);
				string dateDirNM = sFileInfo.LastWriteTime.ToString("yyyy/MM").Replace("/", "");
				string moveToFilePath = Path.Combine(Path.Combine(sFileInfo.DirectoryName, dateDirNM), sFileInfo.Name);

				//処理済みファイルを移動する(装置フォルダ→中間サーバ年月フォルダ)
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[START]{0} DB {1}FileMove", lsetInfo.EquipmentNO, sFileType));
				MoveMachineFileNMC(sFileInfo.FullName, moveToFilePath);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("[END]{0} DB {1}FileMove", lsetInfo.EquipmentNO, sFileType));
			}
		}

	}
}
