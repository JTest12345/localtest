using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;
using ArmsApi;
using ArmsApi.Model;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 途中投入CV
    /// </summary>
    public class LoadConveyor : MachineBase
    {
        /// <summary>
        /// コンベア排出予約アドレス
        /// </summary>
        public string OutputReserveBitAddress { get; set; }

        /// <summary>
        /// QR読み込み結果先頭アドレス
        /// </summary>
        public string QRReaderWordAddress { get; set; }

        /// <summary>
        /// マガジン到達アドレス
        /// </summary>
        public string MagazineArriveBitAddress { get; set; }

        /// <summary>
        /// 予約済みマガジン消失フラグ
        /// </summary>
        public string MissingReservedMagazineBitAddress { get; set; }

        public LoadConveyor() {}

        protected override void concreteThreadWork() 
        {
            if (isMagazineArrived() == true)
            {
                workComplete();
            }

            if (isMissingReservedMagazine() == true)
            {
                Dequeue(Station.Unloader);
                this.Plc.SetBit(MissingReservedMagazineBitAddress, 1, Mitsubishi.BIT_OFF);
            }
        }

        /// <summary>
        /// 作業完了(排出前の持ち上げ動作と次作業の問い合わせジョブ)
        /// </summary>
        protected virtual void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            //作業完了は行わない
            VirtualMag newMagazine = new VirtualMag();
            newMagazine.MagazineNo = Plc.GetMagazineNo(QRReaderWordAddress);
            if (newMagazine.MagazineNo == string.Empty)
            {
                return;
            }
            OutputSysLog($"{this.Name} 到達マガジンの取得：『{newMagazine.MagazineNo}』");
            //遠心沈降用の改良マガジン判定　改良マガジンなら排出行きで仮想を作成してMoveFromToで振り替え
            if (VirtualMag.IsECKMag(newMagazine.MagazineNo))
            {
                //稼動中フラグのあるマガジンは行先の変更を行わない
                Magazine svmag = Magazine.GetCurrent(newMagazine.MagazineNo);
                if (svmag == null)
                {
                    newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
                }
            }

            this.Enqueue(newMagazine, Station.Unloader);

            //立ち上げ要求送信
            Plc.SetBit(this.OutputReserveBitAddress, 1, Mitsubishi.BIT_ON);

            this.WorkComplete(newMagazine, this, false);
        }

        /// <summary>
        /// マガジン到達時
        /// </summary>
        /// <returns></returns>
        protected bool isMagazineArrived()
        {
            string retv = this.Plc.GetBit(MagazineArriveBitAddress);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// CVにあるマガジンを手で取った時
        /// </summary>
        /// <returns></returns>
        protected bool isMissingReservedMagazine()
        {
            string retv = this.Plc.GetBit(MissingReservedMagazineBitAddress);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
