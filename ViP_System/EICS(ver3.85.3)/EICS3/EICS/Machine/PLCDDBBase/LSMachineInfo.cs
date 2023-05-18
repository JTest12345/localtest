using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	/// <summary>
	/// レーザースクライブ（LS:Laser Scribe [SL47略称記載無し])  PLCからデータを取り込む装置
	/// </summary>
	class LSMachineInfo : PLCDDGBasedMachine
	{
		protected override int GetTimingNo(string chipNm)
		{
			return Constant.TIM_LS;
		}

		protected override string[] PLC_MEMORY_ADDR_HEART_BEAT()
		{
			return new string[]
			{
				"W001B00",
				"W001B01"
			};
		}

		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "W001B14"; }
	}
}
