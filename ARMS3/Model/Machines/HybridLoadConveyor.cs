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
    /// 途中投入CV (実Mag・空Mag両方投入可)
    /// </summary>
    public class HybridLoadConveyor : LoadConveyor
    {
        /// <summary>
        /// 排出位置にあるマガジンの種類(実Mag or 空Mag)の情報をPLCに記録する
        /// 0:Magなし
        /// 1:実Mag
        /// 2:空mag
        /// </summary>
        public string UnLoaderMagazineStatusWordAddress { get; set; }

        private int MAGAZINE_STATUS = 1;
        private int EMPTY_MAGAZINE_STATUS = 2;

        public HybridLoadConveyor() { }

        protected override void concreteThreadWork()
        {
            if (isMagazineArrived() == true)
            {
                workComplete();
            }

            if (isMissingReservedMagazine() == true)
            {
                Dequeue(Station.Unloader);
                Dequeue(Station.EmptyMagazineUnloader);
                this.Plc.SetBit(MissingReservedMagazineBitAddress, 1, Mitsubishi.BIT_OFF);
            }

            checkUnLoaderMagazine();
        }

        /// <summary>
        /// 作業完了(排出前の持ち上げ動作と次作業の問い合わせジョブ)
        /// </summary>
        protected override void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            VirtualMag eulMagazine = this.Peek(Station.EmptyMagazineUnloader);
            if (ulMagazine != null || eulMagazine != null)
            {
                return;
            }

            string magazineNo = Plc.GetMagazineNo(QRReaderWordAddress);
            if (magazineNo == string.Empty)
            {
                return;
            }
            OutputSysLog($"{this.Name} 到達マガジンの取得：『{magazineNo}』");

            Magazine svmag = Magazine.GetCurrent(magazineNo);
            VirtualMag newMagazine = new VirtualMag();
            newMagazine.MagazineNo = magazineNo;
            bool isEmptyMagazine = false;

            if (VirtualMag.IsECKMag(magazineNo))
            {
                //稼動中フラグのあるマガジンは行先の変更を行わない
                if (svmag == null)
                {
                    newMagazine.NextMachines.Add(Route.GetDischargeConveyor(this.MacNo));
                }
            }
            else if (svmag != null)
            {
                // 実マガジン
            }
            else
            {
                // 空マガジン
                isEmptyMagazine = true;
            }

            if (isEmptyMagazine == false)
            {
                this.Enqueue(newMagazine, Station.Unloader);
                this.WorkComplete(newMagazine, this, false);
                Plc.SetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress, MAGAZINE_STATUS);
            }
            else
            {
                this.Enqueue(newMagazine, Station.EmptyMagazineUnloader);
                Plc.SetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress, EMPTY_MAGAZINE_STATUS);
            }

            //立ち上げ要求送信
            Plc.SetBit(this.OutputReserveBitAddress, 1, Mitsubishi.BIT_ON);
        }

        /// <summary>
        /// 排出要求確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutput()
        {
            int bitdata = 0;
            try
            {
                bitdata = Plc.GetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、排出要求OFF扱い。アドレス：『{this.UnLoaderMagazineStatusWordAddress}』, エラー内容：{ex.Message}");
                return false;
            }
            if (bitdata != MAGAZINE_STATUS)
            //if (Plc.GetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress) != MAGAZINE_STATUS)
            {
                return false;
            }

            return base.IsRequireOutput();
        }

        /// <summary>
        /// 空マガジン排出要求確認
        /// </summary>
        /// <returns></returns>
        public override bool IsRequireOutputEmptyMagazine()
        {
            int bitdata = 0;
            try
            {
                bitdata = Plc.GetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress);
            }
            catch (Exception ex)
            {
                OutputSysLog($"PLC通信エラーの為、空排出要求OFF扱い。アドレス：『{this.UnLoaderMagazineStatusWordAddress}』, エラー内容：{ex.Message}");
                return false;
            }
            if (bitdata != EMPTY_MAGAZINE_STATUS)
            //if (Plc.GetWordAsDecimalData(this.UnLoaderMagazineStatusWordAddress) != EMPTY_MAGAZINE_STATUS)
            {
                return false;
            }

            return base.IsRequireOutput();
        }

        /// <summary>
        /// UnLoaderの仮想マガジンを排出扱いにするまでの時間 (秒)
        /// </summary>
        public int UnloaderMagazineDischargeCheckCycleSecond { get; set; }

        /// <summary>
        /// 仮想マガジン作成時刻から一定時間経過したUnLoader実マガジンを排出CV行きにする
        /// </summary>
        protected void checkUnLoaderMagazine()
        {
            #region

            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                ulMagazine.LocationId = (int)Station.Unloader;
                PurgeVirtualMagazine(ulMagazine);
            }

            #endregion
        }

        /// <summary>
        /// 引数の仮想マガジンを排出CV行きにする
        /// </summary>
        private void PurgeVirtualMagazine(VirtualMag mag)
        {
            #region

            try
            {
                // 一定時間が経過していない場合は、処理しない
                if ((DateTime.Now - mag.LastUpdDt).TotalSeconds < this.UnloaderMagazineDischargeCheckCycleSecond)
                {
                    return;
                }

                // 処理が不要かどうかを判定 (既に排出CV行きになっているか)
                List<IMachine> macList = mag.NextMachines.Select(m => LineKeeper.GetMachine(m)).ToList();
                bool processfg = false;

                if (macList.Any() == false)
                {
                    // 行先なし ⇒ 処理が必要
                    processfg = true;
                }
                else
                {
                    foreach (IMachine mac in macList)
                    {
                        if ((mac is DischargeConveyor) == false)
                        {
                            // 行先に排出CV以外を含んでいる ⇒ 処理対象
                            processfg = true;
                            break;
                        }
                    }
                }
                if (processfg == false)
                {
                    return;
                }

                // 次装置をクリア + 排出CVを追加 + 更新
                this.OutputSysLog($"時間超過の為、仮想マガジンの次装置を排出CVに変更[位置：「{(Station)Enum.Parse(typeof(Station), mag.LocationId.ToString())}」マガジンNo:「{mag.MagazineNo}」]");
                mag.NextMachines.Clear();
                mag.NextMachines.AddRange(Route.GetDischargeConveyors(this.MacNo));
                mag.Updatequeue();
            }
            catch (Exception ex)
            {
                this.OutputSysLog($"実マガジン排出行き振替時に異常発生した為、処理をスキップ[マガジンNo:{mag.MagazineNo}]理由：{ex.ToString()}");
            }

            #endregion
        }
    }
}
