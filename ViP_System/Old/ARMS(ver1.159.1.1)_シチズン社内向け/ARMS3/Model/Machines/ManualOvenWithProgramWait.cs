using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsApi;

namespace ARMS3.Model.Machines
{
    class ManualOvenWithProgramWait : MachineBase
    {
        protected override void concreteThreadWork()
        {
            try
            {
                updateProgramWait();

                if (IsRequireOutput() == true)
                {
                    workComplete();
                }
            }
            catch (Exception ex)
            {
                FrmErrHandle frmErr = new FrmErrHandle(ex.Message, this.MacNo);
                frmErr.ShowDialog();
            }
        }

        /// <summary>
        /// 排出要求　可否確認
        /// </summary>
        /// <returns>結果</returns>
        public override bool IsRequireOutput()
        {
            VirtualMag peekmag = this.Peek(Station.Loader);
            if (peekmag == null)
            {
                return false;
            }

            //全仮想マガジンのプログラム時間経過後に排出要求ON
            List<VirtualMag> maglist
                = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), string.Empty).ToList();

            bool isNotComplete = false;
            foreach (VirtualMag mag in maglist)
            {
                if (mag.ProgramTotalMinutes.HasValue == false)
                {
                    throw new ApplicationException("プログラム時間の無いマガジンがオーブン内に存在します");
                }

                if ((DateTime.Now - mag.WorkStart.Value).TotalMinutes < (double)mag.ProgramTotalMinutes)
                {
                    isNotComplete = true;
                }
            }

            if (isNotComplete)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void updateProgramWait()
        {
            List<VirtualMag> list
                = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), string.Empty).ToList();

            //常温待機作業の自動完了登録
            foreach (VirtualMag mag in list)
            {
                if (mag.WorkComplete == null)
                {
                    //常温待機作業の自動完了登録
                    Magazine svrmag = Magazine.GetCurrent(mag.MagazineNo);
                    if (svrmag == null) throw new ApplicationException("[完了登録異常] マガジン情報が見つかりません" + mag.MagazineNo);
                    AsmLot svrlot = AsmLot.GetAsmLot(svrmag.NascaLotNO);
                    double? time = getWaitTime(svrlot.TypeCd);

                    if (time.HasValue)
                    {
                        time = Math.Abs(time.Value);
                        mag.WorkComplete = mag.WorkStart.Value.AddMinutes(time.Value + 1);
                        mag.LastMagazineNo = mag.MagazineNo;
                        mag.CurrentLocation = new Location(this.MacNo, Station.Loader);
                        Order order = CommonApi.GetWorkEndOrder(mag, this.MacNo, this.LineNo);

                        ArmsApiResponse workResponse = CommonApi.WorkEnd(order);
                        if (workResponse.IsError)
                        {
							throw new ApplicationException(
								string.Format("[完了登録異常] 常温待機実績登録後にエラー発生 MagazineNo:{0} 内容:{1}", mag.MagazineNo, workResponse.Message));
                        }

                        //完了情報取り直し
                        svrmag = Magazine.GetCurrent(mag.MagazineNo);
                        Process nextproc = Process.GetNextProcess(svrmag.NowCompProcess, svrlot);
                        mag.ProcNo = nextproc.ProcNo;
						mag.WorkStart = mag.WorkComplete.Value.AddMinutes(1);

						workStart(svrmag, nextproc, mag.WorkStart.Value);
                    }
                    else
                    {
                        mag.WorkComplete = DateTime.Now;
                        mag.CurrentLocation = new Location(this.MacNo, Station.Loader);
                    }

                    //作業完了時間を更新済みに
                    mag.Updatequeue();
                }
            }
        }

        private void workStart(Magazine mag, Process p, DateTime startDt)
        {
            AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
            MachineInfo machine = MachineInfo.GetMachine(this.MacNo);

            Order order = new Order();
            order.LotNo = mag.NascaLotNO;
            order.ProcNo = p.ProcNo;
            order.InMagazineNo = mag.MagazineNo;
            order.MacNo = this.MacNo;
            order.WorkStartDt = startDt;
            order.WorkEndDt = null;
            order.TranStartEmpCd = "660";
            order.TranCompEmpCd = "660";

            if (p.IsNascaDefect)
            {
                order.IsDefectEnd = false;
            }
            else
            {
                order.IsDefectEnd = true;
            }

            string errMsg;

            bool isError = WorkChecker.IsErrorBeforeStartWork(lot, machine, order, p, out errMsg);
            if (!isError)
            {
                order.DeleteInsert(order.LotNo);
            }
            else
            {
                throw new ApplicationException(
					string.Format("[開始登録異常] 常温待機後のオーブン自動開始登録でエラー発生 MagazineNo:{0} 内容:{1}", mag.MagazineNo, errMsg));
            }

            Log.ApiLog.Info("ManualOven auto start COMPLETE:" + this.MacNo);
        }


        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            List<VirtualMag> list
                = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), string.Empty).ToList();

            //全仮想マガジンの内、一番新しい時間を全マガジンの開始時間とする
            DateTime workstart = DateTime.MinValue;
            bool isProgramWaitComplete = true;
            foreach (VirtualMag mag in list)
            {
                if (mag.WorkStart.HasValue && mag.WorkStart >= workstart)
                {
                    workstart = mag.WorkStart.Value;
                }

                //常温待機が完了している場合はLoader側で完了時間が入っているハズ
                if (mag.WorkComplete == null)
                {
                    isProgramWaitComplete = false;
                }
            }

            //常温待機作業が完了済みでない場合は何もしない
            if (isProgramWaitComplete == false) return;

            //作業完了
            foreach (VirtualMag mag in list)
            {


                mag.WorkStart = workstart;
                mag.WorkComplete = DateTime.Now;
                mag.LastMagazineNo = mag.MagazineNo;

                mag.Enqueue(mag, new Location(this.MacNo, Station.Unloader));
                mag.Dequeue(new Location(this.MacNo, Station.Loader));
            }
        }


        /// <summary>
        /// 常温待機時間取得
        /// </summary>
        /// <returns></returns>
        private double? getWaitTime(string typeCD)//, string startWorkCd, TimeLimit.JudgeKb startWorkKb, string endWorkCd, TimeLimit.JudgeKb endWorkKb)
        {
            //待機時間取得
            //TimeLimit[] limits = TimeLimit.GetLimits(typeCD, startWorkCd, startWorkKb, endWorkCd, endWorkKb);
            TimeLimit[] limits = TimeLimit.GetLimits(typeCD, Config.Settings.MoldConveyorWaitStartWorkCd, Config.Settings.MoldConveyorWaitStartWorkKb,
                Config.Settings.MoldConveyorWaitEndWorkCd, Config.Settings.MoldConveyorWaitEndWorkKb);

            //マイナス時間を取得
            limits = limits.Where(l => l.EffectLimit < 0).ToArray();
            if (limits.Length == 0)
            {
                return null;
            }
            if (limits.Length > 1)
            {
                throw new ApplicationException(string.Format("MD常温待機時間の設定が2件以上存在します。型番:{0}", typeCD));
            }

            return (double)limits.Single().EffectLimit;
        }
    }
}
