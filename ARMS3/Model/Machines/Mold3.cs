using ArmsApi;
using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
	/// <summary>
	/// MAP自動化で通常マガジンを供給するモールド
	/// </summary>
	public class Mold3 : Mold
	{
		protected override void concreteThreadWork()
		{
			if (base.IsRequireOutput() == true)
			{
				base.workComplete();
			}

			CheckMachineLogFile();
			//if (Config.Settings.IsMappingMode)
			//{
			//	// SDファイル処理
			//	SdComplete();
			//}

			// 仮想マガジン消去要求応答
			ResponseClearMagazineRequest();
		}

		/// <summary>
        /// 空マガジン排出要求
        /// </summary>
        /// <returns></returns>
		public override bool IsRequireOutputEmptyMagazine()
		{
			return base.IsRequireOutputEmptyMagazine();
		}

		/// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
		public override bool Enqueue(VirtualMag mag, Station station)
		{
			return base.Enqueue(mag, station);
		}

		public override bool ResponseEmptyMagazineRequest()
		{
			//供給禁止状態なら処理しない
			if (this.IsInputForbidden() == true)
			{
				return false;
			}

			if (this.IsRequireEmptyMagazine() == true)
			{
				bool IsDeleteFromMag = false;
				Location from = null;

                // 【N工場MAP J9・10不具合 修正】
                //List<LineBridge> bridgeList = LineKeeper.GetReachBridges(this.MacNo);
                List<LineBridge> bridgeList = LineKeeper.Machines.Where(m => m is LineBridge).Select(m => (LineBridge)m).ToList();

                //自装置の空マガジンを使用
                if (this.IsRequireOutputEmptyMagazine() == true)
                {
                    from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    IsDeleteFromMag = true;
                }
                //ライン連結橋の空マガジンを使用
                else if (bridgeList.Count() > 0)
                {
                    foreach (LineBridge bridge in bridgeList)
                    {
                        if (bridge.IsRequireOutputEmptyMagazine() == false) continue;
                        //先頭が遠心沈降マガジン or アオイ基板マガジンなら処理しない
                        VirtualMag mag = bridge.Peek(bridge.GetUnLoaderLocation().Station);
                        if (VirtualMag.IsECKMag(mag.MagazineNo)) continue;
                        if (mag.IsAOIMag()) continue;

                        // 【N工場MAP J9・10不具合 修正】
                        if (mag.NextMachines.Any() == true)
                        {
                            // 空マガジンの仮想マガジンの次装置(A)が入力されており、(A)の装置Noと自装置と違う場合、
                            // (A)装置の空Mag要求がONの場合は、そのマガジンは(A)装置専用とする為、自装置は要求しない
                            IMachine nmac = LineKeeper.GetMachine(mag.NextMachines.First());
                            if (nmac != null && nmac.MacNo != this.MacNo && nmac.IsRequireEmptyMagazine() == true)
                            {
                                continue;
                            }
                        }
                        from = bridge.GetUnLoaderLocation();
                        IsDeleteFromMag = true;

                        break;
                    }
                }
                //空マガジン投入CVの空マガジンを使用
                if (from == null)
                {
                    if (base.IsRequireOutputEmptyMagazine())
                    {
                        //自装置の空マガジン待ち(完了処理待ち)
                        return false;
                    }

                    //空マガジン投入CVの状態確認
                    List<int> emptyMagazineLoadConveyorMacNoList = Route.GetEmptyMagazineLoadConveyors(this.MacNo);
                    foreach (int macNo in emptyMagazineLoadConveyorMacNoList)
                    {
                        IMachine empMagLoadConveyor = LineKeeper.GetMachine(macNo);
                        if (empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
                        {
                            CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                            CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
                            if (fromCar.CarNo != toCar.CarNo)
                            {
                                //空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする
                                List<VirtualMag> bridgeMags = new List<VirtualMag>();

                                List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge || m is RobotQRReader).ToList();
                                foreach (IMachine b in bridges)
                                {
                                    //橋内のすべての仮想マガジンを取得
                                    bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
                                }
                                if (bridgeMags.Any(m => Magazine.GetCurrent(m.MagazineNo) == null &&
                                                        VirtualMag.IsECKMag(m.MagazineNo) == false &&
                                                        m.IsAOIMag() == false))
                                {
                                    return false;
                                }
                            }

                            from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
                        }
                    }
                    if (from == null)
                    {
                        //空マガジン投入CVにマガジンが無い場合
                        return false;
                    }

                    //               //空マガジン投入CVの状態確認
                    //               IMachine empMagLoadConveyor = LineKeeper.GetMachine(Route.GetEmptyMagazineLoadConveyor(this.MacNo));
                    //if (empMagLoadConveyor.IsRequireOutputEmptyMagazine() == true)
                    //{
                    //	CarrierInfo fromCar = Route.GetReachable(new Location(this.MacNo, Station.Loader));
                    //	CarrierInfo toCar = Route.GetReachable(new Location(empMagLoadConveyor.MacNo, Station.Loader));
                    //	if (fromCar.CarNo != toCar.CarNo)
                    //	{
                    //		//空マガジン投入CVが自ラインでは無い場合、橋に空マガジンが無いか確認し、有れば搬送しないようにする
                    //		List<VirtualMag> bridgeMags = new List<VirtualMag>();

                    //		List<IMachine> bridges = LineKeeper.Machines.Where(m => m is LineBridge).ToList();
                    //		foreach (LineBridge b in bridges)
                    //		{
                    //			//橋内のすべての仮想マガジンを取得
                    //			bridgeMags.AddRange(VirtualMag.GetVirtualMag(b.MacNo));
                    //		}
                    //		if (bridgeMags.Count != 0)
                    //		{
                    //			if (bridgeMags.Where(m => Magazine.GetCurrent(m.MagazineNo) == null).Count() != 0)
                    //			{
                    //				return false;
                    //			}
                    //		}
                    //	}

                    //	from = new Location(empMagLoadConveyor.MacNo, Station.EmptyMagazineUnloader);
                    //	IsDeleteFromMag = false;
                    //}
                    //else
                    //{
                    //	//空マガジン投入CVにマガジンが無い場合
                    //	return false;
                    //}
                }

                Location to = new Location(this.MacNo, Station.EmptyMagazineLoader);
				LineKeeper.MoveFromTo(from, to, IsDeleteFromMag, true, false);

				return true;
			}
			else
			{
				return false;
			}
		}
	}
}
