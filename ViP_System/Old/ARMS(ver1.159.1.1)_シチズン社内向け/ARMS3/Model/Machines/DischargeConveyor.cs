using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 排出CV
    /// </summary>
    public class DischargeConveyor : MachineBase
    {
        public DischargeConveyor()
        {

        }   
        protected override void concreteThreadWork()
        {
            // 何も行わない
        }

        public override bool Enqueue(VirtualMag mag, Station station)
        {
            if (string.IsNullOrEmpty(mag.MagazineNo))
			{
				return false;
			}
            
            // 2015.2.10 車載3次 稼働中マガジンを排出する場合はロットログ保存
            Magazine svrMag = Magazine.GetCurrent(mag.MagazineNo);
			if (svrMag == null) 
			{
                return false;
			}
			if (svrMag.NewFg)
			{
                AsmLot.InsertLotLog(svrMag.NascaLotNO, System.DateTime.Now, mag.PurgeReason, svrMag.NowCompProcess, mag.MagazineNo, false, this.LineNo);
            }
            return true;
        }
    }
}
