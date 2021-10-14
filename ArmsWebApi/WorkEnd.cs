using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsWeb.Models;

namespace ArmsWebApi
{
    public class WorkEnd
    {
        public string plantcd;

        public string magno;

        public string lotno;

        public int NewMagFrameQty;

        public string UnloaderMagNo;

        public Magazine mag;

        public AsmLot lot;

        public WorkEndAltModel wem;

        public WorkEnd(string plantcd, string magno, string ulmagno, int NewMagFrameQty)
        {
            this.plantcd = plantcd;
            this.magno = magno;
            this.UnloaderMagNo = ulmagno;
            this.NewMagFrameQty = NewMagFrameQty;
            this.mag = Magazine.GetCurrent(magno);
            this.lot = AsmLot.GetAsmLot(lotno);
            this.wem = new WorkEndAltModel(plantcd);
            if (mag != null)
            {
                this.lotno = mag.NascaLotNO;
            }
        }

        public bool End(out string msg)
        {
            wem = new WorkEndAltModel(plantcd);

            wem.MagList = wem.getUnloaderMag(wem.PlantCd);

            ArmsApi.Model.VirtualMag[] vmgzs =
                ArmsApi.Model.VirtualMag.GetVirtualMag(wem.Mac.MacNo.ToString(), ((int)ArmsApi.Model.Station.Unloader).ToString(), string.Empty);

            var ulmagazine = new List<string> { mag.MagazineNo };
            List<ArmsApi.Model.Magazine> svrmags = new List<ArmsApi.Model.Magazine>();
            foreach (string mgz in ulmagazine)
            {
                ArmsApi.Model.Magazine svrmag = ArmsApi.Model.Magazine.GetCurrent(mgz);

                //ブレンドされているロット、かつ最終工程以降の工程の開始の場合
                CutBlend[] cbs = CutBlend.GetData(mgz);
                if (cbs.Length > 0)
                {
                    AsmLot lot = AsmLot.GetAsmLot(mgz);
                    int lastprocno = Order.GetLastProcNoFromLotNo(cbs.First().BlendLotNo);
                    Process prevproc = Process.GetPrevProcess(lastprocno, lot.TypeCd);
                    Process nextprocess = Process.GetNextProcess(prevproc.ProcNo, lot);

                    if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                    {
                        svrmag = new Magazine();
                        svrmag.MagazineNo = mgz;
                        svrmag.NascaLotNO = mgz;
                        svrmag.NowCompProcess = prevproc.ProcNo;

                        ArmsApi.Model.AsmLot blendlot = lot;
                        blendlot.NascaLotNo = cbs.First().BlendLotNo;
                        wem.BlendLotList.Add(mgz, blendlot);
                    }
                }

                ArmsApi.Model.VirtualMag vmag = vmgzs.Where(vm => vm.LastMagazineNo == mgz).FirstOrDefault();
                if (vmag == null)
                {
                    msg = "Unloader位置に一致する仮想マガジンが見つかりません lastMag:" + vmag.MagazineNo;
                    return false;
                }
                wem.AddMagazine(svrmag, vmag);
            }

            wem.UnloaderMagNo = UnloaderMagNo;
            wem.NewMagFrameQty = NewMagFrameQty;

            var msgs = new List<string>();
            if (!wem.WorkEnd(out msgs))
            {
                msg = msgs[0];
                return false;
            }

            msg = "";
            return true;
        }
    }
}
