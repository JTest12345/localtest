using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.Carriers;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 常温待機CV
    /// </summary>
    public class MoldConveyor : MachineBase
    {
        /// <summary>
        /// 予約済みマガジン消失フラグ
        /// </summary>
        public string MissingReservedMagazineBitAddress { get; set; }

        /// <summary>
        /// 最終のプロファイル予約に使ったマガジン番号
        /// </summary>
        private string lastOvenProfileReserveMag;

        /// <summary>
        /// コンベア排出予約アドレス
        /// </summary>
        public string OutputReserveBitAddress { get; set; }

        /// <summary>
        /// マガジン到達アドレス
        /// </summary>
        public string MagazineArriveBitAddress { get; set; }

        protected override void concreteThreadWork()
        {
            //作業完了処理
            if (this.IsAutoLine)
            {
                workComplete();

                //手動取り除き処理
                if (isMissingReservedMagazine() == true)
                {
                    Dequeue(Station.Unloader);
                    Plc.SetBit(MissingReservedMagazineBitAddress, 1, Mitsubishi.BIT_OFF);
                }
            }
            else 
            {
                workCompletehigh();
            }

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 作業完了(自動化)
        /// 先頭マガジンの作業経過時間を確認。
        /// タイマーを超過していれば排出予約。
        /// </summary>
        private void workComplete()
        {
            VirtualMag unloaderMag = this.Peek(Station.Unloader);
            if (unloaderMag != null)
            {
                return;
            }

            VirtualMag lMagazine = this.Peek(Station.Loader);
            if (lMagazine == null)
            {
                return;
            }

            //if (Properties.Settings.Default.UseOvenProfiler)
            //{
            //    //MDオーブンプロファイル予約
            //    if (this.lastOvenProfileReserveMag != lMagazine.MagazineNo)
            //    {
            //        Magazine svrmag = Magazine.GetCurrent(lMagazine.MagazineNo);
            //        AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
            //        double waittime = getWaitTime(lot.TypeCd, Config.Settings.MoldConveyorWaitStartWorkCd, Config.Settings.MoldConveyorWaitStartWorkKb,
            //        Config.Settings.MoldConveyorWaitEndWorkCd, Config.Settings.MoldConveyorWaitEndWorkKb);

            //        if ((DateTime.Now - lMagazine.WorkStart.Value).TotalMinutes >= waittime - OvenProfiler.MoldCVTopMagazineReserveOffsetMinutes)
            //        {
            //            //オーブン自動切り替え設定
            //            CarrierInfo carrier = Route.GetReachable(new Location(this.MacNo, Station.Loader));
            //            CarrierBase car = (CarrierBase)LineKeeper.GetCarrier(carrier.CarNo);

            //            car.LastMDOvenProfileReserve = OvenProfiler.GetMDReserveProfileNo(lMagazine);
            //            this.lastOvenProfileReserveMag = lMagazine.MagazineNo;
            //        }
            //    }
            //}
            
            //設定時間経過時の処理
            if (isCompleteWait(lMagazine)) 
            {
                bool isArrived = isMagazineArrived();
                if (isArrived == true)
                {
                    lMagazine.LastMagazineNo = lMagazine.MagazineNo;
                    lMagazine.WorkComplete = DateTime.Now;

                    this.Enqueue(lMagazine, Station.Unloader);
                    this.Dequeue(Station.Loader);

                    //排出予約ON
                    Plc.SetBit(this.OutputReserveBitAddress, 1, Mitsubishi.BIT_ON);
                    this.WorkComplete(lMagazine, this, true);
                }
            }
        }

        /// <summary>
        /// 作業完了(高効率)
        /// タイマーを超過していれば排出予約。
        /// </summary>
        public void workCompletehigh()
        {
            VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Loader));
            
            //人搬送の常温待機は順不同で複数完成許可
            foreach (VirtualMag mag in mags)
            {
                if (isCompleteWait(mag) == true)
                {
                    mag.WorkComplete = DateTime.Now;
                    mag.LastMagazineNo = mag.MagazineNo;

                    Enqueue(mag, Station.Unloader);
                    mag.Delete();
                }
            }
        }

        /// <summary>
        /// 待機時間監視
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public bool isCompleteWait(VirtualMag mag)
        {
            if (mag.WorkStart.HasValue == false)
            {
                mag.WorkStart = DateTime.Now;
                mag.CurrentLocation = new Location(this.MacNo, Station.Loader);
                mag.Updatequeue();
            }

            //if (mag.ProgramTotalMinutes.HasValue == false)
            //{
            //    throw new ApplicationException("プログラム時間の無いマガジンがオーブン内に存在します");
            //}

            Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
            AsmLot lot = AsmLot.GetAsmLot(svrmag.NascaLotNO);

            if (mag.ProgramTotalMinutes.HasValue == false || mag.ProgramTotalMinutes.Value == 0)
            {
                //待機時間取得
                //double waitTime = getWaitTime(lot.TypeCd, Config.Settings.MoldConveyorWaitStartWorkCd, Config.Settings.MoldConveyorWaitStartWorkKb,
                //    Config.Settings.MoldConveyorWaitEndWorkCd, Config.Settings.MoldConveyorWaitEndWorkKb);
				//mag.ProgramTotalMinutes = Convert.ToInt32(getWaitTime(lot.TypeCd, Config.Settings.MoldConveyorWaitStartWorkCd, Config.Settings.MoldConveyorWaitStartWorkKb,
				//	Config.Settings.MoldConveyorWaitEndWorkCd, Config.Settings.MoldConveyorWaitEndWorkKb));
				mag.ProgramTotalMinutes = Convert.ToInt32(getWaitTime(lot.TypeCd));
				
				mag.CurrentLocation = new Location(this.MacNo, Station.Loader);
				mag.Updatequeue();
            }
            
            //設定時間経過時の処理
            if ((DateTime.Now - mag.WorkStart.Value).TotalMinutes >= Math.Abs(mag.ProgramTotalMinutes.Value))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 待機時間取得
        /// </summary>
        /// <returns></returns>
        private double getWaitTime(string typeCD)//, string startWorkCd, TimeLimit.JudgeKb startWorkKb, string endWorkCd, TimeLimit.JudgeKb endWorkKb)
        {
            //待機時間取得
            //TimeLimit[] limits = TimeLimit.GetLimits(typeCD, startWorkCd, startWorkKb, endWorkCd, endWorkKb);
			TimeLimit[] limits = TimeLimit.GetLimits(typeCD, Config.Settings.MoldConveyorWaitStartWorkCd, Config.Settings.MoldConveyorWaitStartWorkKb,
				Config.Settings.MoldConveyorWaitEndWorkCd, Config.Settings.MoldConveyorWaitEndWorkKb);

            //マイナス時間を取得
            limits = limits.Where(l => l.EffectLimit < 0).ToArray();
            if (limits.Length == 0) 
            {
                throw new ApplicationException(string.Format("MD常温待機時間の設定が存在しません。型番:{0}", typeCD));
            }
            if (limits.Length > 1)
            {
                throw new ApplicationException(string.Format("MD常温待機時間の設定が2件以上存在します。型番:{0}", typeCD));
            }

            return (double)limits.Single().EffectLimit;

        }

        /// <summary>
        /// CVにあるマガジンを手で取った時
        /// </summary>
        /// <returns></returns>
        public bool isMissingReservedMagazine()
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

        /// <summary>
        /// マガジン存在確認
        /// </summary>
        /// <returns></returns>
        public bool isMagazineArrived()
        {
            string retv = Plc.GetBit(MagazineArriveBitAddress);
            if (retv == Mitsubishi.BIT_ON)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

		public override bool CanInput(VirtualMag mag)
		{
			VirtualMag lMag = this.Peek(Station.Loader);
			if (lMag == null)
			{
				//待機中マガジンが無い場合、投入OK
				return true;
			}
			else
			{
				//待機中マガジンが有る場合、先頭マガジンの待機時間と一致すれば投入OK
				Magazine inputMag = Magazine.GetCurrent(mag.MagazineNo);
				if (inputMag == null)
				{
					return false;
				}
				AsmLot lot = AsmLot.GetAsmLot(inputMag.NascaLotNO);
				double inputMagTime = getWaitTime(lot.TypeCd);

				Magazine svrlMag = Magazine.GetCurrent(lMag.MagazineNo);
				if (svrlMag == null)
				{
					return false;
				}
				lot = AsmLot.GetAsmLot(svrlMag.NascaLotNO);
				double lMagTime = getWaitTime(lot.TypeCd);

				if (inputMagTime == lMagTime)
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
}
