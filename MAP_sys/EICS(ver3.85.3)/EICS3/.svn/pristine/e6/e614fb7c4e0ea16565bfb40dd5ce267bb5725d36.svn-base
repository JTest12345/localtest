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
	class MDMachineInfoSINWA_QUSPA : CIFSBasedMachine
	{
		protected int QC_TIMING_NO_JUSHIWAKU() { return Constant.TIM_FMD; }
		protected int QC_TIMING_NO_INNER() { return Constant.TIM_INMD; }
		protected int QC_TIMING_NO_OUTER() { return Constant.TIM_OUTMD;}
		protected int QC_TIMING_NO_JUSHIWAKU_OUTER() { return Constant.TIM_OUTFMD;}
		protected int QC_TIMING_NO_JUSHIWAKU_INDIVIDUAL() { return Constant.TIM_CHIPFMD;}


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

		protected override int GetTimingNo(string chipNm)
		{
			int timingNo;
			if (chipNm == "樹脂枠") // 先で樹脂枠などのチップNmとタイミングNoの紐付けマスタを作って決め打ちをやめる
			{
				timingNo = QC_TIMING_NO_JUSHIWAKU();
			}
			else if (chipNm == "IM")
			{
				timingNo = QC_TIMING_NO_INNER();
			}
			else if (chipNm == "OM")
			{
				timingNo = QC_TIMING_NO_OUTER();
			}
			else if (chipNm == "外周樹脂枠")
			{
				timingNo = QC_TIMING_NO_JUSHIWAKU_OUTER();
			}
			else if (chipNm == "個片樹脂枠")
			{ 
				timingNo = QC_TIMING_NO_JUSHIWAKU_INDIVIDUAL();
			}
			else
			{
				throw new ApplicationException("不明なchipNmが選択されました。");
			}

			return timingNo;
		}
	}
}
