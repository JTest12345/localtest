using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;

namespace ARMS3.Model.Machines
{
    public class MAPBoardDischargeConveyor : MachineBase
    {
        protected override void concreteThreadWork()
        {
            //空マガジンコンベヤは自動で持ち上げを実施するため何も行わない。
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            //何もしない
            return true;
        }
    }
}
