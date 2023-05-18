using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi;
using ArmsApi.Model;
using ARMS3.Model.Carriers;

namespace ARMS3.Model.Machines
{
    /// <summary>
    /// 選択搭載型のFrameloaderで搭載済みのマガジンが置かれる場所
    /// </summary>
    class FrameLoader3ULBuffer : MachineBase
    {
        public int PrevBufferMacNo { get; set; }

        protected override void concreteThreadWork()
        {
            if (this.IsRequireOutput() == true)
            {
                this.workComplete();
            }
        }

        private void workComplete()
        {
            VirtualMag ulMagazine = this.Peek(Station.Unloader);
            if (ulMagazine != null)
            {
                return;
            }

            IMachine prevBuffer = LineKeeper.GetMachine(this.PrevBufferMacNo);

			//排出要求がある場合は一つ手前のバッファから仮想マガジンを移動
			VirtualMag vmag = prevBuffer.Peek(Station.Unloader);
			if (vmag != null && vmag.NextMachines != null && vmag.NextMachines.Count() > 0)
			{
                this.Enqueue(vmag, Station.Unloader);
                prevBuffer.Dequeue(Station.Unloader);
            }

            //作業完了登録は最初の位置で終わっているので重複しては行わない
        }
    }
}
