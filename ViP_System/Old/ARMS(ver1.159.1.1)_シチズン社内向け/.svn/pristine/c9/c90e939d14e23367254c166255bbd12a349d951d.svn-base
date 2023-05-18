using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class StartMagModel
    {
        public StartMagModel(int? procno, string macgroup)
        {
            try
            {
                VirtualMag[] vmags = VirtualMag.GetVirtualMag(null, ((int)Station.Loader).ToString(), "");

                this.Insts = new List<StartMag>();
                foreach (VirtualMag vmag in vmags)
                {
                    //工程ID不一致も飛ばす
                    if (vmag.ProcNo.HasValue && procno.HasValue && vmag.ProcNo.Value != procno.Value) continue;

                    MachineInfo mac = MachineInfo.GetMachine(vmag.MacNo);

					if (!string.IsNullOrEmpty(macgroup) && !mac.MacGroup.Contains(macgroup))
                    {
                        //macgroupに指定がある場合は条件外の装置も排除
                        continue;
                    }

                    StartMag ins = new StartMag();
                    ins.Mac = mac;
                    ins.Vmag = vmag;
                     Magazine mag = Magazine.GetCurrent(vmag.MagazineNo);
					if (mag == null) continue;

                    ins.Lot = AsmLot.GetAsmLot(mag.NascaLotNO);
					if (ins.Mac.RequestMoveStartMagFg == true)
					{
						this.Insts.Add(ins);
					}
                }
            }
            catch (Exception ex)
            {
                this.Insts = null;
                this.ErrMsg = ex.ToString();
            }
        }

        /// <summary>
        /// 完了待ち状態の仮想マガジンレコード一覧
        /// </summary>
        public List<StartMag> Insts { get; set; }

        public string ErrMsg { get; set; }
    }

    public class StartMag
    {
        public VirtualMag Vmag { get; set; }
        public AsmLot Lot { get; set; }
        public MachineInfo Mac { get; set; }
    }

}