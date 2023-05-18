using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ARMS3.Model.PLC;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// MultiLoaderプラズマ(高効率)
    /// </summary>
    public class ManualMultiLoadPlasma : MachineBase
    {
        protected override void concreteThreadWork()
        {
            try
            {
                if (this.IsRequireOutput() == true)
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
        /// 作業完了
        /// </summary>
        private void workComplete()
        {
            Location loaderLocation = new Location(this.MacNo, Station.Loader);
        
            List<VirtualMag> list 
                = VirtualMag.GetVirtualMag(this.MacNo.ToString(), ((int)Station.Loader).ToString(), string.Empty).ToList();

            //プラズマは全部のマガジンを一度に完成
            foreach (VirtualMag mag in list)
            {
                mag.WorkComplete = DateTime.Now;
                mag.LastMagazineNo = mag.MagazineNo;

                ////Enqueue成功時
                //this.SendCompleteJobData(mag);
                mag.Enqueue(mag, new Location(this.MacNo, Station.Unloader));
                mag.Dequeue(loaderLocation);
            }

            //要求信号を手動で立ち下げ
            setOutPutBitOff();
        }
        
        /// <summary>
        /// 排出要求の立ち下げ
        /// </summary>
        /// <returns></returns>
        private bool setOutPutBitOff()
        {
            //排出要求信号の確認
            if (string.IsNullOrEmpty(this.UnLoaderReqBitAddress) == true)
            {
                return false;
            }

            this.Plc.SetBit(this.UnLoaderReqBitAddress, 1, Mitsubishi.BIT_OFF);

            return true;
        }
    }
}
