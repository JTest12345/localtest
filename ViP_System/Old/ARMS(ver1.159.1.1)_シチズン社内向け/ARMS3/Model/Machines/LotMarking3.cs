using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// ロットマーキング機　大隆精機製の汎用（マガジンToマガジン）
	/// ※装置改造が間に合わなかった為に高生産性ラインの量試はこのクラスで運用する
	/// </summary>
	public class LotMarking3: LotMarking
	{
		protected override void concreteThreadWork()
		{
			this.WorkCompleteHigh();
		}

		public override void WorkCompleteHigh()
		{
			VirtualMag lMag = this.Peek(Station.Loader);
			if (lMag == null)
			{
				return;
			}

			lMag.LastMagazineNo = lMag.MagazineNo;

			if (this.Enqueue(lMag, Station.Unloader))
			{
				this.Dequeue(Station.Loader);
			}
		}
	}
}
