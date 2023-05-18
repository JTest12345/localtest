using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// SLS1 ダイシング
	/// PDAで作業開始登録後、直ぐに完了登録できるように
	/// </summary>
	public class Dicing : MachineBase
	{
		protected override void concreteThreadWork()
		{
			workCompletehigh();
		}

		private void workCompletehigh()
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
