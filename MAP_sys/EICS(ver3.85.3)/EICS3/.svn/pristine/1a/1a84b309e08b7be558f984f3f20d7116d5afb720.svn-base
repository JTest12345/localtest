using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EICS.Machine
{
	/// <summary>
	/// 基板裏面洗浄(BC:Board Cleaning [SL47略称記載無し])　PLCからデータを取り込む装置
	/// </summary>
	class BCMachineInfo : PLCDDGBasedMachine
	{
		protected override int GetTimingNo(string chipNm)
		{
			return Constant.TIM_BC;
		}

		protected override string[] PLC_MEMORY_ADDR_HEART_BEAT()
		{
			return new string[]
			{
				"EM40000"
			};
		}

		protected override string PLC_MEMORY_ADDR_MACHINE_STOP() { return "EM40020"; }
	}
}
