using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// オーブン(高効率)
    /// </summary>
    public class ManualOven : MachineBase
    {
        protected override void concreteThreadWork()
        {
            try
            {
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

        /// <summary>
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            List<VirtualMag> list 
                = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), string.Empty).ToList();

            //全仮想マガジンの内、一番新しい時間を全マガジンの開始時間とする
            DateTime workstart = DateTime.MinValue;
            foreach (VirtualMag mag in list)
            {
                if (mag.WorkStart.HasValue && mag.WorkStart >= workstart)
                {
                    workstart = mag.WorkStart.Value;
                }
            }

            foreach (VirtualMag mag in list)
            {
                mag.WorkStart = workstart;
                mag.WorkComplete = DateTime.Now;
                mag.LastMagazineNo = mag.MagazineNo;

                mag.Enqueue(mag, new Location(this.MacNo,Station.Unloader));
                mag.Dequeue(new Location(this.MacNo, Station.Loader));

                ////Enqueueに成功した場合のみ
                //this.SendCompleteJobData(mag);                
            }
        }
    }
}
