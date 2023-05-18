using ARMS3.Model.Machines;
using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Carriers
{
    /// <summary>
    /// ライン連結橋
    /// </summary>
    public class Bridge : CarrierBase
    {
		/// <summary>
		/// 保持しているマガジン
		/// </summary>
		public override List<VirtualMag> HoldingMagazines
		{
			get
			{
				List<VirtualMag> retv = new List<VirtualMag>();

				if (this.CarNo == 0) return retv;

				List<int> bridgeList = Route.GetMachines(this.CarNo);
				foreach (int macno in bridgeList)
				{
					VirtualMag[] mags = VirtualMag.GetVirtualMag(macno);
					retv.AddRange(mags);
				}

				return retv;
			}
		}

        protected override void concreteThreadWork()
        {
            List<int> machines = Route.GetMachines(this.CarNo);
            foreach(int macno in machines)
            {
                //橋のLoader仮想マガジンを監視

                IMachine bridge = LineKeeper.GetMachine(macno);
                Location loc = bridge.GetLoaderLocation();

                VirtualMag mag = new VirtualMag();
                mag = mag.Peek(loc);

                //送り先橋ロケーションを取得
                int sendbridgeNo = machines.Where(m => m != bridge.MacNo).ToList()[0];
                Location sendLoc = new Location(sendbridgeNo, Station.Unloader);

                IMachine sendbridge = LineKeeper.GetMachine(sendbridgeNo);

                bool isRequireOutput = false;
                if (LineKeeper.GetMachine(sendbridgeNo) is LineBridge2)
                {
                    if (mag != null && ((LineBridge2)LineKeeper.GetMachine(sendbridgeNo)).IsRequireOutput_UsingBaseClass())
                    {
                        isRequireOutput = true;
                    }
                }
                else if (LineKeeper.GetMachine(sendbridgeNo) is LineBridgeBuffer)
                {
                    if (mag != null && ((LineBridgeBuffer)LineKeeper.GetMachine(sendbridgeNo)).IsRequireOutput_UsingBaseClass())
                    {
                        isRequireOutput = true;
                    }
                }
                else
                {
                    if (mag != null && sendbridge.IsRequireOutput())
                    {
                        isRequireOutput = true;
                    }
                }

                if (isRequireOutput == true)
                {
                    //Loaderに仮想マガジンが存在して送り先排出信号がONの場合

                    IMachine machine = LineKeeper.GetMachine(mag.MacNo);
                    Order order = CommonApi.GetWorkEndOrder(mag, machine.MacNo, machine.LineNo);

                    // 【N工場MAP J9・10不具合 修正】
                    // 遠心沈降の空マガジンの場合、次装置を排出CVに更新する
                    // 橋向こうのロボットの腹の上に抱えたときに行き先を再選択
                    if (VirtualMag.IsECKMag(mag.MagazineNo) == true && Magazine.GetCurrent(mag.MagazineNo) == null)
                    {
                        mag.NextMachines.Clear();
                        mag.NextMachines.Add(Route.GetDischargeConveyor(sendLoc.MacNo));
                    }

                    //仮想マガジン移動
                    mag.Enqueue(mag, sendLoc);
                    mag.Dequeue(loc);                    
                }
            }
        }
    }
}
