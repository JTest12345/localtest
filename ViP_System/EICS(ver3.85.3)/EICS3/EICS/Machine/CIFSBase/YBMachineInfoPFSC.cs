using EICS.Database;
using EICS.Machine.Base;
using EICS.Structure;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EICS.Machine.CIFSBase
{
	class YBMachineInfoPFSC : CIFSBasedMachineAddMPD
	{
		protected override int QC_TIMING_NO_ZD() { return Constant.TIM_YB; }
		protected override int QC_TIMING_NO_LED() { return Constant.TIM_YB; }

		protected override int GetTimingNo(string chipNm)
		{
			return Constant.TIM_YB;
		}

		public override void InitFirstLoop(LSETInfo lsetInfo)
		{
			CheckFileFmtFromParamWhenInit(lsetInfo, false);
		}

		public override void CheckFile(LSETInfo lsetInfo)
		{
			base.machineStatus = Constant.MachineStatus.Runtime;

			InitPropAtLoop(lsetInfo);
				
			// 開始時処理関数
			List<ErrMessageInfo> errMsgInfoList = new List<ErrMessageInfo>();
			StartingProcess(lsetInfo, out errMsgInfoList);

			int timingNo = GetTimingNo(lsetInfo.ChipNM);

			// 完了時処理関数
			EndingProcess(lsetInfo, timingNo);

			AdditionProcess(lsetInfo);
		}

		//メインルーチンから呼べる追加プロセス
		protected override void AdditionProcess(LSETInfo lsetInfo)
		{
			BackupMpd(lsetInfo);
		}
	}
}
