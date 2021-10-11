using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    public class MAPCompltDischargeConveyor : MachineBase
    {
        protected override void concreteThreadWork()
        {
            //何も行わない
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //何もしない
            return true;
        }
    }
}
