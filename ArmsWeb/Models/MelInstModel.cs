using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class MelInstModel
    {
        public MelInstModel(int? procno, string macgroup)
        {
            try
            {
                VirtualMag[] vmags = VirtualMag.GetVirtualMag(null, ((int)Station.Unloader).ToString(), "");

                this.Insts = new List<MelInst>();
                foreach (VirtualMag vmag in vmags)
                {
                    //Routeの情報がある（=インライン装置）は無視
                    if (Route.HasRouteInfo(vmag.MacNo)) continue;

                    //工程ID不一致も飛ばす
                    if (vmag.ProcNo.HasValue && procno.HasValue && vmag.ProcNo.Value != procno.Value) continue;

                    MachineInfo mac = MachineInfo.GetMachine(vmag.MacNo);
                    if (!string.IsNullOrEmpty(macgroup) && !mac.MacGroup.Contains(macgroup))
                    {
                        //macgroupに指定がある場合は条件外の装置も排除
                        continue;
                    }

                    MelInst ins = new MelInst();
                    ins.Mac = mac;
                    ins.Vmag = vmag;
                    Magazine mag = Magazine.GetCurrent(vmag.LastMagazineNo);
                    if (mag == null)
                    {
                        mag = Magazine.GetCurrent(vmag.MagazineNo);
                    }
                    ins.Lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                    this.Insts.Add(ins);
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
        public List<MelInst> Insts { get; set; }

        public string ErrMsg { get; set; }
    }

    public class MelInst
    {
        public VirtualMag Vmag { get; set; }
        public AsmLot Lot { get; set; }
        public MachineInfo Mac { get; set; }
    }

}