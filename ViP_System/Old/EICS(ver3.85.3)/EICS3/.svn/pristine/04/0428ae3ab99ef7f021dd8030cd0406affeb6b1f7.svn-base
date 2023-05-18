using EICS.Database;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	/// <summary>
	/// 反射材ﾎﾟｯﾃｨﾝｸﾞ(RP:reflector potting [SL47略称参照])
	/// </summary>
	class RPMachineInfo : MDMachineInfo
	{
		protected override int QC_TIMING_NO() { return 25; }
		protected override int SM_KEIKOUFG_COL_INDEX() { return 6; }

		public override bool SendHeartBeat(LSETInfo lsetInfo, int heartState, int len)
		{
#if Debug
#else		
			KLinkInfo kLinkInfo = new KLinkInfo();
			//「受付準備OKをOFF」
			string resMsg = kLinkInfo.KLINK_SetKV_WRS(ref tcp, ref ns, lsetInfo.IPAddressNO, Constant.MACHINE_PORT, Constant.TRG_Send_Restarting, heartState, len, Constant.ssuffix_U);
			if (resMsg == "Error")
			{
				base.machineStatus = Constant.MachineStatus.Stop;
				return false;
			}
#endif
			return true;
		}
	}
}
