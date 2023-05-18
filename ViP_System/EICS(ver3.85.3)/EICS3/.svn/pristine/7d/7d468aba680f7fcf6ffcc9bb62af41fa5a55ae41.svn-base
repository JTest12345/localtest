using EICS.Machine.Base;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.CIFSBase
{
	class RPMachineInfoMUSASHI : CIFSBasedMachineAddMPD
	{
		protected override string EXT_TRIG_FILE() { return EXT_FIN_FILE; }

		public override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			List<string> targetFileNmList = CIFS.GetProcessableTriggerFileNmList(EndFileDir, EXT_MPD_FILE + "$", false);

			List<string> targetDMList = new List<string>();

			foreach (string targetFileNm in targetFileNmList)
			{
				targetDMList.Add(Path.GetFileNameWithoutExtension(targetFileNm));
			}

			foreach(string targetFileId in targetDMList)
			{
				MpdProcess(lsetInfo, targetFileId, this.EndFileDir, lotNO, magNO, procNO, equipNO);
			}
		}
	}
}
