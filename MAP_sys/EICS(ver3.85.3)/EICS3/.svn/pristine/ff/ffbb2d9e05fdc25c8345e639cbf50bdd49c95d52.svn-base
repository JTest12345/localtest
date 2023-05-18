using EICS.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	class HPMachineInfo : PLCDDGBasedMachine
	{
		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "EM50020"; }

		protected override string[] PLC_MEMORY_ADDR_HEART_BEAT()
		{
			return new string[]
			{
				"EM50000",
			};
		}

		protected override int GetTimingNo(string chipNm)
		{
			return Constant.TIM_MD;
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
#if TEST
#else
				//ハートビート Hレベル
				SendHeartBeat(PLC_MEMORY_ADDR_HEART_BEAT(), true);

				if (lsetInfo.MainThreadFG)
				{
					CreateFileProcess(lsetInfo, true);
				}

				StartingProcess(lsetInfo);

				MachineStopProcess(lsetInfo, true);
#endif

#if TEST
#else
				//処理の分解を行う
				ResponseOKFile(true, lsetInfo);

#endif
			}
			catch (Exception err)
			{
				//装置停止処理
				//SendMachineStop(PLC_MEMORY_ADDR_MACHINE_STOP());
				throw;
			}
		}
	}
}
