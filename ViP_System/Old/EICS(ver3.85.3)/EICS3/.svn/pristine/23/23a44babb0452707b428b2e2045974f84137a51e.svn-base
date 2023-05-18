using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	class PLAMachineInfo : PLCDDGBasedMachine
	{
		private const string PLC_JUDGE_RESULT_ADDR = "W000250";

		public PLAMachineInfo(LSETInfo lsetInfo, IPlc _plc)
		{
			InitPropAtLoop(lsetInfo);
			//InitPLC(lsetInfo);
			plc = _plc;
		}

		public void InitPLC(LSETInfo lsetInfo)
		{

		}

		public void SendResultProcess(LSETInfo lsetInfo, bool isStartUp)
		{
			if (CheckForMachineStopFile(isStartUp))
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				SendNG(PLC_JUDGE_RESULT_ADDR);
			}
		}

		public void SendNG(string memAddr)
		{
#if DEBUG || TEST
			return;
#else
			plc.SetWordAsDecimalData(memAddr, 1);
#endif
		}

		public void SendOK(string memAddr)
		{
#if DEBUG || TEST
			return;
#else
			plc.SetWordAsDecimalData(memAddr, 0);
#endif
		}

		protected void ResponseOKFile(bool isStartUp, LSETInfo lset)
		{
			List<string> chkDirList = new List<string>();
			List<string> chkTargetFileList = new List<string>();
			List<KeyValuePair<string, List<string>>> moveTargetFileList = new List<KeyValuePair<string,List<string>>>() ;

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

				SendOK(PLC_JUDGE_RESULT_ADDR);


				KeyValuePair<string, List<string>> moveInfo = new KeyValuePair<string,List<string>>(chkDir, chkTargetFileList);

				moveTargetFileList.Add(moveInfo);
			}

			log4netHelper.LogControl.OutputLog(log4netHelper.LogLevel.INFO, string.Format(
				"設備:{0}/{1}/{2}号機【OKﾌｧｲﾙ確認の為、ﾃﾞｰﾀ取得要求OFFと出力ﾌｧｲﾙ移動を開始】 isStartUpFg:{3}", lset.ModelNM, lset.EquipmentNO, lset.MachineSeqNO, isStartUp));
			
			//OKファイルを退避(サブ装置スレッドのファイルも全て移動
			foreach (KeyValuePair<string, List<string>> moveTarget in moveTargetFileList)
			{
				EICS.Structure.CIFS.BackupDoneFiles(moveTarget.Value, moveTarget.Key, string.Empty, DateTime.Now);
			}
		}

		protected override bool IsGetableData(IPlc plc, string plcMemAddr)
		{
			int retv = plc.GetWordAsDecimalData(plcMemAddr, 1);

			if (retv == 0)
			{
				return true;
			}
			else if (retv == 1)
			{
				return false;
			}
			else
			{
				throw new ApplicationException(string.Format("PLCから予期せぬ応答を受信しました。ﾒﾓﾘｱﾄﾞﾚｽ:{0} 応答:{1} 想定する応答:{2}", plcMemAddr, retv, "0 or 1"));
			}

		}
	}
}
