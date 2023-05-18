using EICS.Database;
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
	class RPMachineInfoSINWA_QUSPA : CIFSBasedMachine
	{
		protected override int QC_TIMING_NO_ZD() { return Constant.TIM_RP; }
		protected override int QC_TIMING_NO_LED() { return Constant.TIM_RP; }

		protected override string GetStartableFileIdentity()
		{
			if (IsStartableProcess())
			{
				return CIFS.GetLatestProcessableFileIdentity(StartFileDir, EXT_START_FILE, DateIndex, DateLen, false);
			}
			else
			{
				return null;
			}
		}
	}
}
