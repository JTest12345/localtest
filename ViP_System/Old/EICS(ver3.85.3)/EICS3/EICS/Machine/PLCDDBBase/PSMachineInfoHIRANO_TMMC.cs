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
	/// <summary>
	/// 塗工機(PS:Phosphor Sheet [SL47略称記載参照])　PLCからデータを取り込む装置
	/// </summary>
	class PSMachineInfoHIRANO_TMMC : PLCDDGBasedMachine
	{

		private const int ERROR_MSG_MAX_LEN = 200;
		private const string NG_MSG_START_ADDR = "W000200";
		private const string OK_RESULT_ADDR = "B000002";
		private const string NG_RESULT_ADDR = "B000003";
		private const string COMP_VERIFY_ADDR = "B000001";
		private const string NG_MSG_FOR_MACHINE = "EICS閾値エラーによる装置停止";

		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "B000004"; }

		public override void CheckFile(LSETInfo lsetInfo)
		{
#if TEST
			lsetInfo.InputFolderNM = @"C:\qcil\data\test";
			lsetInfo.IPAddressNO = "172.21.56.53";
#endif
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			string plcEnc = settingInfoPerLine.GetPLCEncode(lsetInfo.EquipmentNO);

			try
			{
				base.machineStatus = Constant.MachineStatus.Runtime;

				InitPropAtLoop(lsetInfo);
				InitPLC(lsetInfo);
				//問題発生時は装置停止
#if TEST
#else
				if (lsetInfo.MainThreadFG)
				{
					CreateFileProcess(lsetInfo, true);
				}

				StartingProcess(lsetInfo);

				MachineStopProcess(lsetInfo, true);
#endif

#if TEST
#else
				ResponseOKFile(true, lsetInfo);
#endif
			}
			catch (Exception err)
			{
				//装置側がサイクル停止に対応していない為、停止命令は除去 2015/10/22 n.yoshimoto
				//plc.SetString(NG_MSG_START_ADDR, "EICS異常停止による装置停止", plcEnc);
				//Thread.Sleep(100);
				//装置停止処理
				//SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
				throw;
			}
		}

		protected override bool IsGetableData(IPlc plc, string plcMemAddr)
		{
#if DEBUG
			return false;
#endif
            string retv = plc.GetBit(plcMemAddr);

			if (retv == "0")
			{
				return false;
			}
			else if (retv == "1")
			{
				return true;
			}
			else
			{
				throw new ApplicationException(string.Format("PLCから予期せぬ応答を受信しました。ﾒﾓﾘｱﾄﾞﾚｽ:{0} 応答:{1} 想定する応答:{2}", plcMemAddr, retv, "0 or 1"));
			}

		}

		public void SendMachineStop(string memAddr)
		{
#if DEBUG || TEST
			return;
#else
			plc.SetBit(memAddr, 1, "1");
#endif
		}

		public override void MachineStopProcess(LSETInfo lsetInfo, bool isStartUp)
		{
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);
			string plcEnc = settingInfoPerLine.GetPLCEncode(lsetInfo.EquipmentNO);

			string errMsg = string.Empty;

			if (CheckForMachineStopFile(isStartUp, out errMsg))
			{
				base.machineStatus = Constant.MachineStatus.Stop;

				//errMsg = errMsg.Replace("\r", "").Replace("\n", "");

				plc.SetString(NG_MSG_START_ADDR, NG_MSG_FOR_MACHINE, plcEnc);

				Thread.Sleep(100);

				plc.SetBit(NG_RESULT_ADDR, 1, "1");

				Thread.Sleep(100);

				plc.SetBit(COMP_VERIFY_ADDR, 1, "0");
			}
		}

		protected void ResponseOKFile(bool isStartUp, LSETInfo lset)
		{
			List<string> chkDirList = new List<string>();
			List<string> chkTargetFileList = new List<string>();
			List<KeyValuePair<string, List<string>>> moveTargetFileList = new List<KeyValuePair<string, List<string>>>();

			if (isStartUp)
			{
				chkDirList.Add(StartFileDir);
			}
			else
			{
				chkDirList.Add(EndFileDir);
			}

			chkDirList.AddRange(GetSubGrpOutputDir(lset, isStartUp, string.Empty));

			//移動対象を抽出
			foreach (string chkDir in chkDirList)
			{
				//OKファイルがあるかチェック
				chkTargetFileList = Common.GetFiles(chkDir, EICS.Structure.CIFS.EXT_OK_FILE);
				if (chkTargetFileList.Count == 0) return;

				KeyValuePair<string, List<string>> moveInfo = new KeyValuePair<string, List<string>>(chkDir, chkTargetFileList);

				moveTargetFileList.Add(moveInfo);
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
				"設備:{0}/{1}/{2}号機【OKﾌｧｲﾙ確認の為、ﾃﾞｰﾀ取得要求OFFと出力ﾌｧｲﾙ移動を開始】 isStartUpFg:{3}", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO, isStartUp));

			if (moveTargetFileList.Count > 0)
			{
				plc.SetBit(OK_RESULT_ADDR, 1, "1");
			}

			//存在する場合はデータ取得要求フラグを落とす
			Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lset, 0, isStartUp);
			foreach (KeyValuePair<string, string> prefixAddr in prefixList)
			{
				ResetDataRecvReq(prefixAddr.Key, prefixAddr.Value);
			}

			//OKファイルを退避(サブ装置スレッドのファイルも全て移動
			foreach (KeyValuePair<string, List<string>> moveTarget in moveTargetFileList)
			{
				EICS.Structure.CIFS.BackupDoneFiles(moveTarget.Value, moveTarget.Key, string.Empty, DateTime.Now);
			}
		}

		protected void ResetDataRecvReq(string prefixNm, string plcMemAddr)
		{
#if DEBUG || TEST
			return;
#endif

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
						"【ﾃﾞｰﾀ取得要求OFF】prefx:{0} addr:{1}", prefixNm, plcMemAddr));

			plc.SetBit(plcMemAddr, 1, "0");
		}

		public bool CheckForMachineStopFile(bool isStartUp, out string errMsg)
		{
			errMsg = string.Empty;
			List<string> filePathList = new List<string>();
			string targetFilePath;


			if (isStartUp == true)
			{
				//開始フォルダに.NGファイルリスト追加
				filePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + CIFS.EXT_NG_FILE));
				//開始フォルダに.STOPファイルリスト追加
				filePathList.AddRange(Common.GetFiles(StartFileDir, ".*\\." + CIFS.EXT_STOP_FILE));

				if (filePathList.Count() > 0)
				{
					filePathList.Sort();

					targetFilePath = filePathList.Last();

					using (StreamReader sr = new StreamReader(targetFilePath))
					{
						char[] buffer = new char[ERROR_MSG_MAX_LEN];
						sr.Read(buffer, 0, ERROR_MSG_MAX_LEN);

						errMsg = string.Join("", buffer);
					}

					CIFS.BackupDoneFiles(filePathList, StartFileDir, string.Empty, DateTime.Now);
					return true;
				}
			}

			return false;
		}
	}
}
