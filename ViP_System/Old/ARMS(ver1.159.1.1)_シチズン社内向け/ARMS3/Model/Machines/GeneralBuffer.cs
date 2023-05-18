using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.Carriers;
namespace ARMS3.Model.Machines
{
    /// <summary>
    /// バッファ
    /// </summary>
    public class GeneralBuffer : MachineBase
    {
        protected override void concreteThreadWork()
        {
            VirtualMag ulMagazine = Peek(Station.Unloader);
            if (ulMagazine == null) return;

            //排出要求OFFになったら仮想マガジンを消してしまう
            if (this.IsRequireOutput() == false)
            {
                this.Dequeue(Station.Unloader);
            }
        }

        /// <summary>
        /// 空マガジン供給　可否確認
        /// </summary>
        /// <returns></returns>
        public override bool ResponseEmptyMagazineRequest()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);

            //仮想マガジンが無いのに排出要求があるなら排出CVへ捨ててしまう
            if (ulMagazine == null && this.IsRequireOutput())
            {
                //Log.SysLog.Info("Unloader仮想マガジン無し,排出信号ONの時、排出CVへ");

                IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));

                Location from = new Location(this.MacNo, Station.Unloader);
                Location to = new Location(dischargeConveyor.MacNo, Station.Loader);

                LineKeeper.MoveFromTo(from, to, false, true, false);
                
                return true;
            }

            return false;
        }

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //ローダーに供給された瞬間にUnloaderにEnqueueしてしまう。
            if (station == Station.Loader)
            {
                station = Station.Unloader;
            }

            mag.LastMagazineNo = mag.MagazineNo;
            mag.WorkComplete = DateTime.Now;

            base.Enqueue(mag, station);

            base.WorkComplete(mag, this, true);

            return true;
        }
    }
}
