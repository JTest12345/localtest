using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;

namespace ARMS3.Model.Machines
{
    public class MAPBoardMagazineLoadConveyor : MachineBase
    {
        protected override void concreteThreadWork()
        {
            if (base.IsRequireOutput() == true)
            {
                workComplete();
            }
        }

        private void workComplete()
        {
            VirtualMag ulMagazine = Peek(Station.Unloader);
            if (ulMagazine != null) return;

            VirtualMag mag = new VirtualMag();
            mag.MagazineNo = VirtualMag.MAP_AOI_MAGAZINE;

            //アオイ基板は先ず行先排出CVで登録し読込後に行先決定
            mag.NextMachines.Add(Route.GetAoiDischargeConveyor(this.MacNo));

            this.Enqueue(mag, Station.Unloader);
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            if (station != Station.Unloader)
            {
                //何もしない
                return true;
            }
            return base.Enqueue(mag, station);
        }
    }
}
