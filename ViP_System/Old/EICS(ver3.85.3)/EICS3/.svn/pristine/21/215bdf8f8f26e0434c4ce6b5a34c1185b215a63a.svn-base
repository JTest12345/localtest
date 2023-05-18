using EICS.Machine.Base;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.CIFSBase
{
	class AIMachineInfoKOHYOUNG : CIFSBasedMachineAddMPD
	{
		protected virtual string EXT_TRIG_FILE() { return EXT_FIN2_FILE; }

		public override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			MpdProcess(lsetInfo, targetFileIdentity, this.EndFileDir, lotNO, magNO, procNO, equipNO);
		}

		//CIFSBasedMachineAddMPDを継承するRPMachineInfo_MUSASHIも同様の関数必要であるとわかった為、CIFSBasedMachineAddMPDへ移動 2016/5/12 n.yoshi
		//protected override string GetStartableFileIdentity()
		//{
		//	string targetFileIdentity = string.Empty;

		//	if (IsStartableProcess())
		//	{
		//		targetFileIdentity = CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false);
		//	}

		//	return targetFileIdentity;
		//}
	}
}
