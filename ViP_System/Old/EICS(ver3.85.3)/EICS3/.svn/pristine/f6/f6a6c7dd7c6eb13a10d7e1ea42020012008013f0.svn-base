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
	/// 基板重量測定(BWM:Board Weight Measure [SL47略称記載無し])　PLCからデータを取り込む装置
	/// </summary>
	class BWMMachineInfo : PLCDDGBasedMachine
	{
		protected override int GetTimingNo(string chipNm)
		{
			return Constant.TIM_BWM;
		}

		public int EndFileCreateCt { get; set; }
		public bool LatestUnloadReqBit { get; set; }

		public BWMMachineInfo()
		{
			EndFileCreateCt = 0;
			LatestUnloadReqBit = false;
		}

		new protected virtual string[] PLC_MEMORY_ADDR_HEART_BEAT()
		{
			return new string[]
			{
				"W000200",
				"W000201"
			};
		}

		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "W000214"; }

		public bool GetUnloadReqBit()
		{
#if DEBUG
			return false;
#endif

			string bitStr = plc.GetBit("B000004");

			if (bitStr == "1")
			{
				return true;
			}
			else
			{
				return false;
			}
		}

		public bool GetSubEnableBit()
		{
#if DEBUG
			return false;
#endif

			string bitStr = plc.GetBit("B000042");

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
			try
			{
#if DEBUG
				lsetInfo.InputFolderNM = @"C:\QCIL\data\";
				lsetInfo.IPAddressNO = "";
#endif
				base.machineStatus = Constant.MachineStatus.Runtime;


				InitPropAtLoop(lsetInfo);
				InitPLC(lsetInfo);
				//問題発生時は装置停止

				bool available = IsAvailable(AvailableAddress);

				if (lsetInfo.MainThreadFG)
				{
					//ハートビート Hレベル
					SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), true);

					if (available)
					{
						CreateFileProcess(lsetInfo, true);


						bool unloadReqBit = GetUnloadReqBit(); //ﾃﾞｰﾀ取得要求OFF後に再立ち上がりしているっぽいので、そこへの応急処置 2015/8/20 n.yoshimoto　装置側対応も立ち消え？応急処置が恒久処置になる予感

						if (unloadReqBit && EndFileCreateCt == 0)//ﾃﾞｰﾀ取得要求OFF後に再立ち上がりしているっぽいので、そこへの応急処置 2015/8/20 n.yoshimoto
						{
							CreateFileProcess(lsetInfo, false);
							EndFileCreateCt++;//ﾃﾞｰﾀ取得要求OFF後に再立ち上がりしているっぽいので、そこへの応急処置 2015/8/20 n.yoshimoto
						}

						if (LatestUnloadReqBit == true && unloadReqBit == false)//ﾃﾞｰﾀ取得要求OFF後に再立ち上がりしているっぽいので、そこへの応急処置 2015/8/20 n.yoshimoto
						{
							EndFileCreateCt = 0;//ﾃﾞｰﾀ取得要求OFF後に再立ち上がりしているっぽいので、そこへの応急処置 2015/8/20 n.yoshimoto
						}
						LatestUnloadReqBit = unloadReqBit;//ﾃﾞｰﾀ取得要求OFF後に再立ち上がりしているっぽいので、そこへの応急処置 2015/8/20 n.yoshimoto
					}
				}

				StartingProcess(lsetInfo);

				MachineStopProcess(lsetInfo, true);


				EndingProcess(lsetInfo, GetTimingNo(lsetInfo.ChipNM), available);

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
#if DEBUG
				throw;
#endif
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
