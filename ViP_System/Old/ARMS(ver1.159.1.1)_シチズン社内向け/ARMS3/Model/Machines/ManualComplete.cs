using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    public class ManualComplete : CifsMachineBase
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

            //仮想マガジンのEnqueueが失敗(Unloaderに既に同マガジンが有る)場合でもLoader側の
            //仮想マガジンは削除する。
            this.Enqueue(lMag, Station.Unloader);
            this.Dequeue(Station.Loader);
        }
    }
}
