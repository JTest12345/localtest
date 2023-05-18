using EICS.Machine.Base;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace EICS.Machine.CIFSBase
{
	class ScrewMDMachineInfo : MDMachineInfo
	{
		private const int HEART_BEAT_ADDR__MAX_LEN = 10;
		public override void CheckFile(LSETInfo lsetInfo)
		{
			try
			{
				InitFileErrorState();

				CheckDirectory(lsetInfo);

				base.machineStatus = Constant.MachineStatus.Runtime;
				tcp = null;
				ns = null;

				if (IPAddress == null)
					IPAddress = lsetInfo.IPAddressNO;

				SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(this.LineCD);
				SettingInfo settingInfoCommon = SettingInfo.GetSingleton();

				//マッピング有効ラインの場合、LENS2起動確認
				if (settingInfoCommon.IsMappingMode)
				{
					if (!KLinkInfo.CheckLENS())
					{
						throw new ApplicationException(string.Format("LENS2を起動して下さい。", this.LineCD));
					}
				}

				if (SendHeartBeat(lsetInfo, 1, 10) == false)
				{
					return;
				}

				#region 装置運転開始時のパラメータ管理

				CIFSBasedMachineAddMPD cifsInst = new CIFSBasedMachineAddMPD();

				cifsInst.InitPropAtLoop(lsetInfo);

				List<ErrMessageInfo> errMsgList = null;

				bool doneWstChk = cifsInst.StartingProcess(lsetInfo, out errMsgList);

				if (doneWstChk && errMsgList != null)
				{
					bool wstChkResult = true;
					if (errMsgList.Count() > 0)
					{
						wstChkResult = false;
						this.errorMessageList.AddRange(errMsgList);
					}
					
					CheckPlcParameter(lsetInfo, ref errorMessageList, wstChkResult);
				}
				else
				{
					//wstファイルのチェックが行われてない場合は、PLCの判定も判定結果送信も何もしない。
				}

				//対象ファイル種類を取得
				Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, true);
				foreach (KeyValuePair<string, string> prefix in prefixList)
				{
					if (string.IsNullOrWhiteSpace(prefix.Key) && string.IsNullOrWhiteSpace(prefix.Value))
						continue;

					FileProcess(lsetInfo, prefix.Key, prefix.Value, true);
				}

				#endregion

				prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, false);
				foreach (KeyValuePair<string, string> prefix in prefixList)
				{
					if (string.IsNullOrWhiteSpace(prefix.Key) && string.IsNullOrWhiteSpace(prefix.Value))
						continue;

					FileProcess(lsetInfo, prefix.Key, prefix.Value, false);
				}

				if (SendHeartBeat(lsetInfo, 0, 10) == false)
				{
					return;
				}
			}
			finally
			{
				if (ns != null) { ns.Close(); }
				if (tcp != null) { tcp.Close(); }
			}
		}

		protected override void FileProcess(LSETInfo lsetInfo, string prefixNm, string prefixAddr, bool startUpFG)
		{
			AlertLog alertLog = AlertLog.GetInstance();

			string resMsg = string.Empty;
			KLinkInfo kLinkInfo = new KLinkInfo();
			SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			MagInfo magInfo = new MagInfo();

			bool waitForRenameByArmsFg = settingInfoPerLine.GetWaitForRenameByArmsFG(lsetInfo.EquipmentNO);

			//ファイル取得要求
			resMsg = kLinkInfo.KLINK_GetKV_RD(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				string logMsg = string.Format("装置からのﾌｧｲﾙ取得要求の確認時に異常発生 設備CD:{0} ﾌｧｲﾙ識別:{1} ｱﾄﾞﾚｽ:{2} startUpFg:{3}", lsetInfo.EquipmentNO, prefixNm, prefixAddr, startUpFG);
				alertLog.logMessageQue.Enqueue(logMsg);

				base.machineStatus = Constant.MachineStatus.Stop;
				return;
			}
			if (Convert.ToInt16(resMsg.ToString().Trim()) == 1)//ONの場合
			{
				string logMsg = string.Format("ﾌｧｲﾙ取得要求確認 設備CD:{0} ﾌｧｲﾙ識別:{1} ｱﾄﾞﾚｽ:{2} startUpFg:{3}", lsetInfo.EquipmentNO, prefixNm, prefixAddr, startUpFG);
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, logMsg, lsetInfo.EquipmentNO);

				if (startUpFG == false && lsetInfo.EquipInfo.WaitForRenameByArmsFG)
				{
					FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
					int loopCt = 0;

					while (file == null && loopCt < 100)
					{
						Thread.Sleep(settingInfoCommon.MachineLogOutWaitmSec);
						file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
						loopCt++;
					}

					if (file == null)
					{
						throw new ApplicationException(string.Format(
							"ARMSによるﾌｧｲﾙﾘﾈｰﾑを{0}ms {1}回、待機しましたが完了しませんでした。ARMSでﾄﾗﾌﾞﾙが無いか(監視状態の確認含む)確認して下さい。" +
							"ARMSが問題無い場合 ﾘﾈｰﾑ待機設定が不要か、ﾘﾈｰﾑ完了している場合 EICSの問題の可能性が有ります。ｼｽﾃﾑ担当者へ連絡して下さい。監視ﾊﾟｽ:{2} ﾌｧｲﾙ識別:{3}"
							, settingInfoCommon.MachineLogOutWaitmSec, loopCt, lsetInfo.InputFolderNM, prefixNm));
					}

					magInfo.sNascaLotNO = MachineFile.GetLotFromFileName(file.FullName, 1);
					magInfo.sMagazineNO = MachineFile.GetMagazineNoFromFileName(file.FullName, 1);
					magInfo.sMaterialCD = MachineFile.GetTypeFromFileName(file.FullName, 1);
				}
				else
				{
					magInfo.sMaterialCD = lsetInfo.TypeCD;
					magInfo.sMagazineNO = string.Empty;
					magInfo.sNascaLotNO = string.Empty;
				}

				//MDMachineInfo.csでは関数末尾に存在する処理。
				//ScrewMDMachineInfo.csをSLPからマージしてくる際に
				//SLPソースではMDMachineInfo.csのFileProcess()を変更していたがマージに依る影響範囲を抑える為
				//ScrewMDMachineInfo.csでFileProcess()をoverrideして処理内容をSLPのMDMachineInfo.csと同じ変更を適用
				resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, prefixAddr, 0, 1, Constant.ssuffix_U);
				if (resMsg == "Error")
				{
					base.machineStatus = Constant.MachineStatus.Stop;
					return;
				}
				log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format("plantCD:{0} □MD欠損調査 {1}:{2}取得要求立ち下げ", lsetInfo.EquipmentNO, prefixNm, prefixAddr), lsetInfo.EquipmentNO);

				if ((prefixNm == SDFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
					|| (waitForRenameByArmsFg && startUpFG == false))
				{
					FileInfo file = MachineFile.Search(lsetInfo.InputFolderNM, prefixNm);
					if (file == null)
					{
						return;
					}

					if (!MachineFile.IsThereLotNoInFileName(file.FullName))
					{
						//SDファイル名にロット情報が付与されていない間は待機
						return;
					}

					if (prefixNm == SDFile.FILE_PREFIX && settingInfoCommon.IsMappingMode)
					{
						//log000_454254_UNKNOWN_UNKNOWN_0.csvファイル名の時はヘッダのみのﾌｧｲﾙなのでLENS処理は完了させたことにして後の処理で削除等させる
						if (SDFile.IsUnknownFile(file) == false)
						{
							SDFile sdFile = new SDFile(file);
							if (Database.LENS.WorkResult.IsComplete(sdFile.LotNo, sdFile.ProcNo, this.LineCD) == false)
							{
								//LENSの処理が完了していない間は待機
								return;
							}
						}
					}
				}

				//「受付準備OKをOFF」
				resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, 0, 11, Constant.ssuffix_U);
				if (resMsg == "Error")
				{
					base.machineStatus = Constant.MachineStatus.Stop;
					return;
				}

				if (settingInfoPerLine.LineType == Constant.LineType.Out.ToString() && IsStopTargetFileKind(prefixNm))
				{
					//アウトラインはPRファイルとORファイルのみ異常判定する為、他ファイルはReserveへ移動 //BTS.2126により改修(ORファイル追加)
					MoveReserveFolderFiles(lsetInfo.InputFolderNM);
				}
				else
				{
					//ログファイル処理
					FileDistribution(ref tcp, ref ns, prefixAddr, lsetInfo, prefixNm, magInfo, ref base.errorMessageList);
				}

				if ((settingInfoPerLine.LineType == Constant.LineType.Out.ToString() && base.errorMessageList.Count != 0) || IsStopTargetFileKind(prefixNm))
				{
					//ファイル処理で異常判定があれば装置停止要求
					kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Runnnin, 1, 1, Constant.ssuffix_U);
				}
			}
		}

		public override bool SendHeartBeat(LSETInfo lsetInfo, int heartState, int len)
		{
			SettingInfo settingInfoCommon = SettingInfo.GetSingleton();
			
			KLinkInfo kLinkInfo = new KLinkInfo();

			int sendLen = len;

			if (settingInfoCommon.IsMappingMode == false)
			{
				sendLen = HEART_BEAT_ADDR__MAX_LEN;
			}

				//「受付準備OKをOFF」
				string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, heartState, sendLen, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				return false;
			}
			
			return true;
		}

	}
}
