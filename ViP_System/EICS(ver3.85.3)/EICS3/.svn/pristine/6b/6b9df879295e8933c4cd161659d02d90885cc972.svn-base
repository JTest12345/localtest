using EICS.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	class MDMachineInfoYAMAMOTO : PLCDDGBasedMachine
	{
		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "E3_10009.01"; }

		protected string PLC_MEMORY_ADDR_PARAM_TRIG = "E3_10000.01";
		protected string PLC_MEMORY_ADDR_RESULT_TRIG = "E3_10000.02";
		protected string PLC_MEMORY_ADDR_RESULT_GET_END = "E3_10010.01";
		protected string PLC_MEMORY_ADDR_PARAM_OK = "E3_10008.01";
		protected string PLC_MEMORY_ADDR_PARAM_NG = "E3_10008.02";

		protected override int GetTimingNo(string chipNm)
		{
			return Constant.TIM_MD;
		}

		protected override void SendHeartBeat(string[] plcMemAddrArray, bool bitVal)
		{
			//山本製MDに対して、ハートビートは知らせない
		}

		protected override void ResetDataRecvReq(string prefixNm, string plcMemAddr)
		{
#if DEBUG || TEST
			return;
#endif
			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
						"【ﾃﾞｰﾀ取得完了ON】prefx:{0} addr:{1}", prefixNm, plcMemAddr));

			plc.SetWordAsDecimalData(plcMemAddr, 1);
		}

		protected override Dictionary<string, string> GetPrefixList(LSETInfo lsetInfo, bool isStartUp, string prefixNm)
		{
			Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, isStartUp, prefixNm, null);

			if (prefixList.Count > 1)
			{
				throw new ApplicationException(string.Format("この装置仕様は2つ以上のprefixに対応していません。prefix数:{0} isStartUp:{1}"
					, prefixList.Count(), isStartUp));
			}
			else if (prefixList.Count == 0)
			{
				throw new ApplicationException(string.Format("TmFileFMTからprefixが取得できませんでした。isStartUp:{0}", isStartUp));
			}

			string key = prefixList.ElementAt(0).Key;
			if (isStartUp)
			{
				prefixList[key] = PLC_MEMORY_ADDR_PARAM_TRIG;
			}
			else
			{
				prefixList[key] = PLC_MEMORY_ADDR_RESULT_TRIG;
			}

			return prefixList;
		}

		protected virtual Dictionary<string, string> GetSystemProcessEndTrig(LSETInfo lsetInfo, bool isStartUp)
		{
			Dictionary<string, string> prefixList = ConnectDB.GetMachineFilePrefix(lsetInfo, 0, isStartUp, null, null);

			if (prefixList.Count > 1)
			{
				throw new ApplicationException(string.Format("この装置仕様は2つ以上のprefixに対応していません。prefix数:{0} isStartUp:{1}"
					, prefixList.Count(), isStartUp));
			}
			else if (prefixList.Count == 0)
			{
				throw new ApplicationException(string.Format("TmFileFMTからprefixが取得できませんでした。isStartUp:{0}", isStartUp));
			}

			string key = prefixList.ElementAt(0).Key;
			if (isStartUp)
			{
				prefixList[key] = PLC_MEMORY_ADDR_PARAM_OK;
			}
			else
			{
				prefixList[key] = PLC_MEMORY_ADDR_RESULT_GET_END;
			}

			return prefixList;
		}

		protected override void ResponseOKFile(bool isStartUp, LSETInfo lset)
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

			//存在する場合はデータ取得要求フラグを落とす
			Dictionary<string, string> prefixList = GetSystemProcessEndTrig(lset, isStartUp);
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

		public override void MachineStopProcess(LSETInfo lsetInfo, bool isStartUp)
		{
			if (CheckForMachineStopFile(isStartUp))
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());

				//存在する場合はデータ取得要求フラグを落とす
				Dictionary<string, string> prefixList = GetPrefixList(lsetInfo, isStartUp, null);
				foreach (KeyValuePair<string, string> prefixAddr in prefixList)
				{
#if DEBUG
					continue;
#endif
					ResetDataRecvReq(prefixAddr.Key, prefixAddr.Value);
				}
			}
		}
	}
}
