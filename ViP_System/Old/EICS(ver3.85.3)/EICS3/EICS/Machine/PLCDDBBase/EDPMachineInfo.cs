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
	/// 電着(EDP:ElectroDePosition [SL47略称参照])   PLCからデータを取得する装置
	/// </summary>
	class EDPMachineInfo : PLCDDGBasedMachine
	{
		new protected virtual string[] PLC_MEMORY_ADDR_HEART_BEAT() 
		{
			return new string[]
			{
				"W000300",
				"W000301"
			}; 
		}

		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "W000314"; }

		public bool GetSubEnableBit()
		{
#if DEBUG
			return false;
#endif

			string bitStr = plc.GetBit("B001540");

			if (bitStr == "1")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
#if TEST
			lsetInfo.InputFolderNM = @"C:\qcil\data\test";
			lsetInfo.IPAddressNO = "172.21.56.53";
#endif
			try
			{
				base.machineStatus = Constant.MachineStatus.Runtime;

				InitPropAtLoop(lsetInfo);
				InitPLC(lsetInfo);
				//問題発生時は装置停止

				if (lsetInfo.MainThreadFG)
				{
					//ハートビート Hレベル
					SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), true);

					CreateFileProcess(lsetInfo, true);
					CreateFileProcess(lsetInfo, false);
				}

				StartingProcess(lsetInfo);

				MachineStopProcess(lsetInfo, true);

				int timingNo;
				if (lsetInfo.ChipNM == "白電着")
				{
					timingNo = Constant.TIM_WEDP;
				}
				else if (lsetInfo.ChipNM == "電着")
				{
					timingNo = Constant.TIM_EDP;
				}
				else
				{
					throw new NotImplementedException();
				}

				EndingProcess(lsetInfo, timingNo);

				MachineStopProcess(lsetInfo, false);

				if (lsetInfo.MainThreadFG)
				{
					ResponseOKFile(true, lsetInfo);
					ResponseOKFile(false, lsetInfo);
					//ハートビート Lレベル
					SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), false);
				}
			}
			catch (Exception err)
			{
				//装置停止処理
				SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
				throw;
			}
		}

		protected override List<string> GetSubGrpOutputDir(LSETInfo lsetInfo, bool isStartUpFile, string fileIdentity)
		{
			//サブ装置スレッド用のディレクトリ取得
			List<LSETInfo> lsetList = ConnectDB.GetLSETData(lsetInfo.InlineCD, string.Empty);
			List<LSETInfo> subGrpEquipList = new List<LSETInfo>();
			List<string> subGrpOutputDirList = new List<string>();
			SettingInfo settingInfoPerLine = SettingInfo.GetSettingInfoPerLine(lsetInfo.InlineCD);

			if (string.IsNullOrEmpty(lsetInfo.ThreadGrpCD) == false)
			{
				subGrpEquipList = lsetList.Where(l => l.ThreadGrpCD == lsetInfo.ThreadGrpCD && l.MainThreadFG == false).ToList();
			}

			if (isStartUpFile)
			{
				foreach (LSETInfo subGrpEquip in subGrpEquipList)
				{
#if DEBUG
					subGrpEquip.InputFolderNM = @"C:\QCIL\DATA";
#endif
					string outputSubDir = string.Empty;
					outputSubDir = Path.Combine(subGrpEquip.InputFolderNM, settingInfoPerLine.GetStartFileDirNm(subGrpEquip.EquipmentNO));

					if (GetSubEnableBit())
					{
						subGrpOutputDirList.Add(outputSubDir);
					}
					else
					{
						if (string.IsNullOrEmpty(fileIdentity) == false)
						{
							CIFS.OutputResultFile(outputSubDir, fileIdentity, string.Empty, true);
						}
					}
				}
			}
			else
			{
				foreach (LSETInfo subGrpEquip in subGrpEquipList)
				{
#if DEBUG
					subGrpEquip.InputFolderNM = @"C:\QCIL\DATA";
#endif
					string outputSubDir = string.Empty;

					outputSubDir = Path.Combine(subGrpEquip.InputFolderNM, settingInfoPerLine.GetEndFileDirNm(subGrpEquip.EquipmentNO));

					if (GetSubEnableBit())
					{
						subGrpOutputDirList.Add(outputSubDir);
					}
					else
					{
						if (string.IsNullOrEmpty(fileIdentity) == false)
						{
							CIFS.OutputResultFile(outputSubDir, fileIdentity, string.Empty, true);
						}
					}
				}
			}


			return subGrpOutputDirList;
		}
	}
}
