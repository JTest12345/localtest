using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class CutBlendModel
    {
        public CutBlendModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
            this.BlendList = CutBlend.GetCurrentBlendItems(this.Mac.MacNo);
            this.BlendList = this.BlendList.OrderBy(b => b.StartDt).ToArray();
            this.CurrentBlend = new List<CutBlend>();
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        public MachineInfo Mac { get; set; }
        public CutBlend[] BlendList { get; set; }

        public List<CutBlend> CurrentBlend { get; set; }

        public string BlendLotNo { get; set; }


        /// <summary>
        /// 合計数
        /// </summary>
        public int TotalQty { get; set; }

        /// <summary>
        /// 不良数
        /// </summary>
        public int DefQty { get; set; }

        /// <summary>
        /// 試験数
        /// </summary>
        public int TestQty { get; set; }

        public int ProcNo { get; set; }

        public void FinishBlend()
        {
            CarrierInfo carrier = Route.GetReachable(new Location(Mac.MacNo, Station.Loader));
            if (carrier == null) 
            {
                throw new ApplicationException("TmRouteからライン番号の取得に失敗");
            }

            this.BlendLotNo = CutBlend.CompleteBlend(DateTime.Now, CurrentBlend.ToArray(), carrier.CarNo.ToString().Substring(carrier.CarNo.ToString().Length - 2, 2), this.EmpCd, Mac.IsAutoLine);

            AsmLot lot = AsmLot.GetAsmLot(this.CurrentBlend[0].LotNo);
            Process[] flow = Process.GetWorkFlow(lot.TypeCd);
            Process final = flow.Where(p => p.FinalSt == true).FirstOrDefault();

            //予約登録の不良を登録(※CutBlend.CalcTotalよりも先に呼び出しが必要　※処理の順序入れ替え厳禁)
            ApplyPreDefect(this.CurrentBlend, final.ProcNo, BlendLotNo);

            int total, deftotal, testtotal;
            CutBlend.CalcTotal(this.CurrentBlend.ToArray(), BlendLotNo, out total, out deftotal, out testtotal);
            this.TotalQty = total;
            this.DefQty = deftotal;
            this.TestQty = testtotal;

            //稼働フラグ外す
            foreach (CutBlend cb in this.CurrentBlend)
            {
                Magazine[] mags = Magazine.GetMagazine(cb.MagNo, cb.LotNo, true, null);
                foreach (Magazine mag in mags)
                {
                    Magazine.UpdateNewFgOff(mag.MagazineNo);
                }

                VirtualMag[] vmags = VirtualMag.GetVirtualMag(this.Mac.MacNo.ToString(), ((int)Station.Unloader).ToString(), cb.MagNo);
                foreach (VirtualMag vmag in vmags)
                {
                    vmag.Delete();
                }
            }

            this.ProcNo = final.ProcNo;
        }

        public void ApplyPreDefect(List<CutBlend> blendlist, int procno, string blendlotno)
        {
            List<DefItem> predefs = new List<DefItem>();

            foreach (CutBlend cb in blendlist)
            {
                Defect pre = Defect.GetDefect(cb.LotNo, procno);

                foreach (DefItem p in pre.DefItems)
                {
                    bool found = false;
                    foreach (DefItem m in predefs)
                    {
                        if (p.CauseCd == m.CauseCd && p.ClassCd == m.ClassCd && p.DefectCd == m.DefectCd)
                        {
                            m.DefectCt += p.DefectCt;
                            found = true;
                            break;
                        }
                    }
                    if (!found)
                    {
                        predefs.Add(p);
                    }
                }
            }

            Defect def = new Defect();
            def.DefItems = new List<DefItem>();
            def.DefItems.AddRange(predefs);
            def.LotNo = blendlotno;
            def.ProcNo = procno;
            def.DeleteInsert();
        }

        public string GetMnfctKB(string lotno)
        {
            AsmLot lot = AsmLot.GetAsmLot(lotno);

            if (lot == null) return string.Empty;

            Profile p = Profile.GetProfile(lot.ProfileId);

            if (p == null) return string.Empty;

            return p.MnfctKb;
        }
    }
}