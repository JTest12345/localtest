using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 自動化ライン外への排出CV
    /// </summary>
    public class AutoLineOutConveyor : MachineBase
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
