using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine.PLCDDBBase
{
	class DCMachineInfoDISCO : PLCDDGBasedMachine
	{
		protected override string[] PLC_MEMORY_ADDR_HEART_BEAT()
		{
			return new string[]
			{
				"",
				""
			};
		}

		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return ""; }
	}
}
