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

        public string typecd;

        public int procno;

        public int NewMagFrameQty { get; set; } = 0;

        public int FailureBdQty { get; set; } = 0;

        public string UnloaderMagNo;

        public string empcd;

        public Magazine mag;

        public AsmLot lot;

        public WorkEndAltModel wem;

        public WorkEnd(string plantcd, string empcd, string magno, string ulmagno)
        {
            this.plantcd = plantcd;
            this.empcd = empcd;
            this.magno = magno;
            this.UnloaderMagNo = ulmagno;

            //製品名・Lotno取得
            this.mag = Magazine.GetCurrent(magno);
            this.lotno = this.mag.NascaLotNO;
            this.typecd = AsmLot.GetAsmLot(lotno).TypeCd;

            //工程No取得
            lot = AsmLot.GetAsmLot(lotno);
            this.procno = Process.GetNowProcess(lot.NascaLotNo).ProcNo;

            //上記で十分だったため不用コード化
            //var curp = Process.GetWorkFlow(lot.TypeCd, "", false);
            //if (curp[0].ProcNo == Process.GetNowProcess(lot.NascaLotNo).ProcNo)
            //{
            //    //初工程用処理
            //    this.procno = mag.NowCompProcess;
            //}
            //else
            //{
            //    //初工程以外処理
            //    Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
            //    this.procno = p.ProcNo;
            //}
        }

        public WorkEnd(string plantcd, string empcd, string magno, string ulmagno, string lotno = "")
        {
            this.plantcd = plantcd;
            this.empcd = empcd;
            this.magno = magno;
            this.UnloaderMagNo = ulmagno;

            //製品名・Lotno取得
            if (string.IsNullOrEmpty(magno))
            {
                this.mag = Magazine.GetCurrent(magno);
                this.lotno = this.mag.NascaLotNO;
                this.typecd = AsmLot.GetAsmLot(lotno).TypeCd;
            }
            if (string.IsNullOrEmpty(lotno))
            {
                this.lotno = lotno;
                this.typecd = AsmLot.GetAsmLot(this.lotno).TypeCd;
            }

            //工程No取得
            lot = AsmLot.GetAsmLot(lotno);
            this.procno = Process.GetNowProcess(lot.NascaLotNo).ProcNo;

            //上記で十分だったため不用コード化
            //var curp = Process.GetWorkFlow(lot.TypeCd, "", false);
            //if (curp[0].ProcNo == Process.GetNowProcess(lot.NascaLotNo).ProcNo)
            //{
            //    //初工程用処理
            //    this.procno = mag.NowCompProcess;
            //}
            //else
            //{
            //    //初工程以外処理
            //    Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
            //    this.procno = p.ProcNo;
            //}
        }

        public bool RegisterDefects(out string msg, Dictionary<string, int> Defectdict)
        {
            try
            {
                //不良リスト取得
                DefItem[] defs = Defect.GetAllDefectSubSt(lotno, typecd, procno);

                //不良内訳計上
                foreach (var defect in Defectdict)
                {
                    DefItem di = defs.Where(d => d.DefectCd == defect.Key).FirstOrDefault();
                    if (di != null)
                    {
                        if (!InputDef(plantcd, out msg, di.ClassCd, di.CauseCd, di.DefectCd, defect.Value.ToString(), null, null))
                        {
                            return false;
                        }
                    }
                }

                //不良基板数計上
                this.FailureBdQty = Defectdict.Count;
                
                msg = "";
                return true;
            }
            catch(Exception ex)
            {
                msg = "不良項目の登録が失敗しています:" + ex.Message;
                return false;
            }

        }


        public List<Magazine> GetMagazines(string plantcd)
        {
            wem = new WorkEndAltModel(plantcd);
            return wem.getUnloaderMag(plantcd);
        }

        public bool End(out string msg)
        {
            this.mag = Magazine.GetCurrent(magno);

            if (mag == null)
            {
                msg = "対象の実マガジンは存在しません";
                return false;
            }

            this.lotno = mag.NascaLotNO;
            this.lot = AsmLot.GetAsmLot(lotno);

            if (lot == null)
            {
                msg = "対象のロットは存在しません";
                return false;
            }

            if (plantcd == "")
            {
                msg = "設備コード(plantcd)が必要です";
                return false;
            }

            try
            {
                wem = new WorkEndAltModel(plantcd);
                wem.MagList = new List<Magazine>();
                wem.EmpCd = this.empcd;

                // すべてのロードされている仮想マガジンを強制的にアンロード
                MachineInfo m = MachineInfo.GetMachine(plantcd);
                List<VirtualMag> vmags = VirtualMag.GetVirtualMag(m.MacNo, ((int)Station.Loader)).ToList();
                foreach (var mag in vmags)
                {
                    mag.CurrentLocation = new Location(m.MacNo, Station.Loader);
                    mag.Dequeue(mag.CurrentLocation);
                    mag.CurrentLocation.Station = Station.Unloader;
                    mag.LastMagazineNo = mag.MagazineNo;
                    mag.Enqueue(mag, mag.CurrentLocation);
                }

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
                        msg = "Unloader位置に一致する仮想マガジンが見つかりません lastMag:" + mgz;
                        return false;
                    }
                    wem.AddMagazine(svrmag, vmag);
                }

                wem.UnloaderMagNo = UnloaderMagNo;

                // 20220328 JuniWatanabe
                // NewMagFrameQtyの強制入力はしない
                // DefectDicからの計上のみにする
                //if (NewMagFrameQty != 0)
                //{
                //    wem.NewMagFrameQty = NewMagFrameQty;
                //}
                //else
                //{
                //    wem.NewMagFrameQty = wem.MagList[0].FrameQty;
                //}

                // DefectDicからの不良基板数計上
                wem.NewMagFrameQty = 0;
                wem.FailureBdQty = FailureBdQty;

                var msgs = new List<string>();
                if (!wem.WorkEnd(out msgs))
                {
                    msg = msgs[0];
                    return false;
                }

                msg = "";
                return true;
            }
            catch (Exception e)
            {
                msg = e.ToString();
                return false;
            }
        }


        public bool InputDef(string plantcd, out string msg, string classcd, string causecd, string defectcd, string qty, string address, string unit)
        {
            int ct;
            if (int.TryParse(qty, out ct) == false)
            {
                msg = "不良数は数字を入力してください";
                return false;
            }

            ArmsApi.Model.DefItem[] defs = Defect.GetAllDefectSubSt(this.lotno, this.typecd, this.procno);
            ArmsApi.Model.DefItem di = defs.Where(d => d.CauseCd == causecd && d.ClassCd == classcd && d.DefectCd == defectcd).FirstOrDefault();
            if (di == null)
            {
                msg = "不良明細が見つかりません";
            }

            di.DefectCt = ct;

            ArmsApi.Model.Defect defect = new ArmsApi.Model.Defect();
            defect.LotNo = this.lotno;
            defect.DefItems = new List<ArmsApi.Model.DefItem>(defs);
            defect.ProcNo = this.procno;
            defect.MagazineNo = this.magno;

            //数量チェック
            if (!defect.CheckDefectSubSt(null))
            {
                msg = "不良枚数が基板枚数を超えています";
                return false;
            }

            //EICSのWB不良アドレス更新
            //wem.UpdateEicsWBAddress(di, address, unit);


            //Defect更新
            defect.DeleteInsertSubSt(null);
            msg = "";
            return true;

        }
    }
}
