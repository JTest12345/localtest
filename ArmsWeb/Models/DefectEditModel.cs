using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class DefectEditModel
    {
        public DefectEditModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
            Order[] tmpOrders = Order.GetCurrentWorkingOrderInMachine(Mac.MacNo);

            List<Order> newOrders = new List<Order>();   
            foreach (Order o in tmpOrders)
            {
                bool isCutBlend = false;
                //ブレンドされているロット、かつ最終工程以降の工程の場合
                CutBlend[] cbs = CutBlend.SearchBlendRecord(null, o.LotNo, null, false, false);
                if (cbs.Length > 0)
                {
                    AsmLot lot = AsmLot.GetAsmLot(cbs.First().LotNo);

                    //ブレンドロットの最終工程を取得
                    int lastprocno = Order.GetLastProcNoFromLotNo(o.LotNo);
                    Process prevprocess = Process.GetPrevProcess(lastprocno, lot.TypeCd);
                    Process nextprocess = Process.GetNextProcess(prevprocess.ProcNo, lot);

                    if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                    {
                        isCutBlend = true;

                        foreach (CutBlend cb in cbs)
                        {
                            Order newo = new Order();
                            newo.LotNo = cb.LotNo;
                            newo.ProcNo = o.ProcNo;
                            newo.MacNo = o.MacNo;
                            newo.InMagazineNo = o.InMagazineNo;
                            newo.WorkStartDt = o.WorkStartDt;
                            newo.InspectCt = o.InspectCt;
                            newo.InspectEmpCd = o.InspectEmpCd;
                            newOrders.Add(newo);
                        }
                    }
                }

                if(isCutBlend == false)
                {
                    newOrders.Add(o);
                }
            }

            this.Orders = newOrders.OrderBy(o => o.WorkStartDt).ToArray();
        }

        public DefectEditModel(Order order, string typecd)
        {
            this.Orders = new Order[1];
            this.Orders[0] = order;
            this.TypeCd = typecd;
        }

        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        public string TypeCd { get; set; }

        /// <summary>
        /// 状態検査フラグ
        /// </summary>
        public bool IsInspection { get; set; }

        public MachineInfo Mac { get; set; }

        public Order EditTarget { get; set; }

        public DefItem CurrentDefItem { get; set; }

        public Order[] Orders { get; set; }

        public string InitialInspectCtStr { get; set; }

        /// <summary>
        /// カット工程不良予約登録
        /// </summary>
        public bool IsCutPreInputDef { get; set; }

        public string Filter { get; set; }

        public bool HasFilter { get { return !string.IsNullOrEmpty(Filter); } }

        /// <summary>
        /// ブレンドされているロット、かつ最終工程以降の工程の場合のブレンドロット番号
        /// </summary>
        public string BlendLotNo { get; set; }

        /// <summary>
        /// ブレンドされているロット、かつ最終工程以降の工程の場合の子ロットリスト
        /// </summary>
        public List<string> BlendChildLotList { get; set; }

        /// <summary>
        /// 抜き取り検査完了フラグON(IsInspection=trueの場合）
        /// </summary>
        public void UpdateInspectionFg()
        {
            //状態検査完了
            if (this.IsInspection == true)
            {
                Inspection isp = Inspection.GetInspection(EditTarget.NascaLotNo, EditTarget.ProcNo);
                if (isp != null)
                {
                    isp.IsInspected = true;
                    isp.DeleteInsert();
                }
            }
        }

        /// <summary>
        /// Orderの検査者、検査数だけをデータベース更新
        /// </summary>
        public void UpdateEmpCdAndInspectCt()
        {
            int inspectct = EditTarget.InspectCt;
            string empcd = EditTarget.InspectEmpCd;

            string lotno = EditTarget.LotNo;
            int procno = EditTarget.ProcNo;

            //念のため最新情報取り直し
            EditTarget = Order.GetMagazineOrder(EditTarget.LotNo, EditTarget.ProcNo);

            bool isBlendLot = false;
            if (EditTarget == null)
            {
                //ブレンドされているロット、かつ最終工程以降の工程の場合
                CutBlend[] cbs = CutBlend.GetData(lotno);
                if (cbs.Length > 0)
                {
                    AsmLot lot = AsmLot.GetAsmLot(cbs.First().LotNo);

                    //ブレンドロットの最終工程を取得
                    int lastprocno = Order.GetLastProcNoFromLotNo(cbs.First().BlendLotNo);
                    Process prevprocess = Process.GetPrevProcess(lastprocno, lot.TypeCd);
                    Process nextprocess = Process.GetNextProcess(prevprocess.ProcNo, lot);

                    if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                    {
                        isBlendLot = true;
                        BlendLotNo = cbs.First().BlendLotNo;

                        BlendChildLotList = new List<string>();
                        foreach (CutBlend cb in CutBlend.SearchBlendRecord(null, cbs.First().BlendLotNo, null, false, false))
                        {
                            BlendChildLotList.Add(cb.LotNo);
                        }

                        EditTarget = Order.GetMagazineOrder(cbs.First().BlendLotNo, procno);
                    }
                }
            }

            EditTarget.InspectCt = inspectct;
            EditTarget.InspectEmpCd = this.EmpCd;

            EditTarget.DeleteInsert(EditTarget.LotNo);

            if (isBlendLot)
            {
                //ロットを子ロットに戻す
                EditTarget.LotNo = lotno;
            }
        }

        public int GetFinalProcNo()
        {
            Process[] flow = Process.GetWorkFlow(this.TypeCd);
            Process final = flow.Where(p => p.FinalSt == true).FirstOrDefault();
            return final.ProcNo;
        }


        public DefItem[] GetDefItems()
        {
            if (this.IsCutPreInputDef == false)
            {
                DefItem[] defs = Defect.GetAllDefect(this.EditTarget.LotNo, this.TypeCd, EditTarget.ProcNo);
                return defs;
            }
            else
            {
                DefItem[] defs = Defect.GetAllDefect(this.EditTarget.LotNo, this.TypeCd, GetFinalProcNo());
                return defs;
            }
        }

        public Dictionary<string, string> GetCauseCdList(DefItem[] defs)
        {
            Dictionary<string, string> retv = new Dictionary<string, string>();

            foreach (DefItem def in defs)
            {
                if (!retv.Keys.Contains(def.CauseCd))
                {
                    retv.Add(def.CauseCd, def.CauseName);
                }
            }

            return retv;
        }

        /// <summary>
        /// EICSテーブルの
        /// </summary>
        public void UpdateEicsWBAddress(DefItem def, string address, string unit)
        {
            if (string.IsNullOrEmpty(address) && string.IsNullOrEmpty(unit))
            {
                // アドレス, ユニットが両方空白なら何もしない
                return;
            }
            else if (string.IsNullOrEmpty(address) && string.IsNullOrEmpty(unit) == false)
            {
                throw new ApplicationException("ユニットが入力されているのにアドレスが空白です");
            }

            if (string.IsNullOrWhiteSpace(unit))
            {
                unit = "0";
            }

            int unitno;
            if (!int.TryParse(unit, out unitno))
            {
                throw new ApplicationException("ユニットに数値変換できない文字が入力されています。");
            }

            ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(this.EditTarget.NascaLotNo);

            //Defect.UpdateEICSWireBondAddress(this.EditTarget.InMagazineNo, lot, def, address, unit, this.EmpCd);
            ArmsApi.Model.LENS.MacDefect.InsertUpdate(this.EditTarget.InMagazineNo, lot, def, address, unit, this.EmpCd);
        }      
    }
}