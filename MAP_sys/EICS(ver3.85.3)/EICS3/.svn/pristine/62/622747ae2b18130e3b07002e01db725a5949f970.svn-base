using EICS.Machine.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.CIFSBase
{
	/// <summary>
	/// レンズ実装装置  wedファイルが出てきて、mpdファイルの退避をせんといかんのだとか…
	/// </summary>
	class LHAMachineInfo : CIFSBasedMachineAddMPD
	{
		protected virtual string EXT_TRIG_FILE() { return EXT_FIN2_FILE; }

		public override void AdditionEndProcess(LSETInfo lsetInfo, string targetFileIdentity, string lotNO, string magNO, int procNO, string equipNO)
		{
			MpdProcess(lsetInfo, targetFileIdentity, this.EndFileDir, lotNO, magNO, procNO, equipNO);
		}
	}
}
