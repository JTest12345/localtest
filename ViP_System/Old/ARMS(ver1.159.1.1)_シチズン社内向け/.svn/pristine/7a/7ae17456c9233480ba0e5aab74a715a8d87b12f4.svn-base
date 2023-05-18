using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// カット
    /// </summary>
    public class Cut : MachineBase
    {
        /// <summary>
        /// 反転ユニット通過済設定用アドレス
        /// </summary>
        public string IsMagReverseWorkBitAddress { get; set; }

        /// <summary>
        /// メインルーチン
        /// </summary>
        protected override void concreteThreadWork()
        {
			if (this.IsAutoLine)
			{
				//カット機の場合、空マガジン排出要求を処理
				if (this.IsRequireOutputEmptyMagazine() == true)
				{
					workComplete();
				}
			}
			else
			{
				if (this.IsRequireOutput() == true)
				{
					workCompletehigh();
				}
			}

            //仮想マガジン消去要求応答
            ResponseClearMagazineRequest();
        }

        /// <summary>
        /// 作業完了処理(自動化)
        /// </summary>
        private void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.EmptyMagazineUnloader);
            if (ulMagazine != null)
            {
                return;
            }

            VirtualMag lMagazine = this.Peek(Station.Loader);
            if (lMagazine == null)
            {
                return;
            }

            Log.ApiLog.Info("CT作業完了処理");

            lMagazine.LastMagazineNo = lMagazine.MagazineNo;
            lMagazine.WorkComplete = DateTime.Now;

            //搭載向けの移動時には次工程IDを0にセット。
            //空マガジン移動時にもNASCA開始登録が正常に動くようにする対策
            lMagazine.ProcNo = 0;

            this.Enqueue(lMagazine, Station.EmptyMagazineUnloader);
            this.Dequeue(Station.Loader);
        }

		/// <summary>
		/// 作業完了処理(高効率)
		/// </summary>
		private void workCompletehigh()
		{
			//カットは全部のマガジンを一度に完成
			VirtualMag[] mags = VirtualMag.GetVirtualMag(this.MacNo, ((int)Station.Loader));
			foreach (VirtualMag mag in mags)
			{
				mag.WorkComplete = DateTime.Now;
				mag.LastMagazineNo = mag.MagazineNo;

				this.Enqueue(mag, Station.Unloader);

				this.Dequeue(Station.Loader);
			}

			//要求信号を手動で立ち下げ
			setOutPutBitOff();
		}

		/// <summary>
		/// 排出要求の立ち下げ
		/// </summary>
		/// <returns></returns>
		private void setOutPutBitOff()
		{
			Plc.SetBit(this.UnLoaderReqBitAddress, 1, Mitsubishi.BIT_OFF);
		}


        /// <summary>
        /// 反転ユニット通過済設定用アドレスを設定
        /// </summary>
        /// <param name="plc"></param>
        /// <param name="flag"></param>
        public void SetMagReverseWorkBitAddress(bool flag)
        {
            if (flag)
            {
                this.Plc.SetBit(IsMagReverseWorkBitAddress, 1, Mitsubishi.BIT_ON);
            }
            else
            {
                this.Plc.SetBit(IsMagReverseWorkBitAddress, 1, Mitsubishi.BIT_OFF);
            }
        }

        /// <summary>
        /// 仮想マガジン作成
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="station"></param>
        public override bool Enqueue(VirtualMag mag, Station station)
        {
            if (station == Station.Loader)
            {
                //MAPラインの場合、搬送時に仮ラベル印字
                if (Config.GetLineType == Config.LineTypes.NEL_MAP)
                {
                    if (!string.IsNullOrEmpty(Config.Settings.CutPreLabelDir))
                    {
                        try
                        {
                            CutBlend[] cblist = CutBlend.GetCurrentBlendItems(this.MacNo)
								.OrderByDescending(c => c.StartDt).ToArray();

                            foreach (CutBlend cb in cblist)
                            {
                                if (cb.MagNo == mag.MagazineNo)
                                {
                                    CutBlend.PrintMapPreCutLabel(
                                        this.MacNo, cb.LotNo, cb.MagNo);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Log.RBLog.Error("MAP ダイシング前ラベル印字エラー:" + ex.ToString());
                        }
                    }
                }
            }
            return base.Enqueue(mag, station);
        }

        /// <summary>
        /// マガジン供給(マガジン指定)　可否確認
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public override bool CanInput(VirtualMag mag)
        {
            bool retv = base.CanInput(mag);
            if (retv == false) return retv;

            if (Config.GetLineType == Config.LineTypes.NEL_MAP)
            {
                //作業順に移載機(反転)を含む場合、反転ユニット通過フラグをTrue
                Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);

                List<Process> workfrow = Process.GetWorkFlow(svrlot.TypeCd).ToList();
                if (workfrow.Exists(w => w.WorkCd == Config.Settings.MagExchangerReverseWorkCd))
                {
                    //757系通常フローを想定
                    SetMagReverseWorkBitAddress(true);
                }
                else
                {
                    //557,657,157系通常フローを想定
                    SetMagReverseWorkBitAddress(false);
                }
            }

            return base.CanInput(mag);
        }

        /// <summary>
        /// 空マガジン配置
        /// カット機の場合は排出先を探す
        /// </summary>
        /// <param name="robo"></param>
        public override bool ResponseEmptyMagazineRequest()
        {
            while (this.IsRequireOutputEmptyMagazine() == true)
            {
                VirtualMag emptyMag = Peek(Station.EmptyMagazineUnloader);
                if (emptyMag == null)
                {
                    //仮想マガジンが無い場合は処理しない
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
                    return false;
                }
            }

            return false;
        }
    }
}
