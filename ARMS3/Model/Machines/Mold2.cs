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
	/// [廃止] ECK2(マガジン全段仕様)の運用に伴い、分割マガジンを使用する事が無くなった為。
    /// SIDEVIEW 304D用の空遠心沈降マガジンを供給するMold
    /// </summary>
    public class Mold2 : Mold
    {
        protected override void concreteThreadWork()
        {
            if (base.IsRequireOutput() == true)
            {
                base.workComplete();
            }

            //仮想マガジン消去要求応答
            base.ResponseClearMagazineRequest();
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
        
        /// <summary>
        /// 空マガジン配置
        /// </summary>
        /// <returns>結果</returns>
        public override bool ResponseEmptyMagazineRequest()
        {
            //遠心沈降用改良マガジン+マガジン移載機対応

            //供給側は途中投入CV側の移動処理
            //排出側は移載機側の空マガジン要求処理で行うのでモールド機は処理無し
            //仮想マガジンだけを作成する
            if (string.IsNullOrEmpty(this.EULMagazineAddress)) return false;

            VirtualMag mag = Peek(Station.EmptyMagazineUnloader);
            if (mag != null) return false;

            if (this.IsRequireOutputEmptyMagazine())
            {
                string magno = Plc.GetMagazineNo(EULMagazineAddress);

                if (string.IsNullOrEmpty(magno) == false)
                {
                    Magazine svrmag = Magazine.GetCurrent(magno);
                    if (svrmag != null)
                    {
                        //現在稼働中フラグのあるマガジンなら無視する
                        return false;
                    }

                    mag = new VirtualMag();
                    mag.MagazineNo = magno;
                    mag.LastMagazineNo = magno;
                    Enqueue(mag, Station.EmptyMagazineUnloader);
                }
                else
                {
                    Log.RBLog.Info("モールド空マガジン排出位置のマガジンNO取得に失敗");
                    return false;
                }

                // 排出コンベアの状態確認
                IMachine dischargeConveyor = LineKeeper.GetMachine(Route.GetDischargeConveyor(this.MacNo));
                if (dischargeConveyor.IsRequireInput() == true)
                {
                    Location from = new Location(this.MacNo, Station.EmptyMagazineUnloader);
                    Location to = new Location(dischargeConveyor.MacNo, Station.Loader);

                    LineKeeper.MoveFromTo(from, to, true, true, false);
                    return true;
                }
                else
                {
                    //空マガジンをどこにも置けない場合
                    Dequeue(Station.EmptyMagazineUnloader);
                    return false;
                }
            }
            return false;
        }
    }
}
