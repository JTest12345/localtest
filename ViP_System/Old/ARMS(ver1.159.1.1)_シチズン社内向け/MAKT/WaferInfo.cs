using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using ArmsApi;



namespace MAKT
{
    class WaferInfo
    {
        public static void Crawl()
        {
            var marks = LotMarkData.Search(null, null, null, null, null, false);

            AsmLot lot = null;
            SortedList<int, MachineInfo> procList = new SortedList<int,MachineInfo>();
            bool isComplete = false;
            int i = 1;
            foreach (var m in marks)
            {
                try
                {
                    if (lot == null || lot.NascaLotNo != m.LotNo)
                    {
                        lot = AsmLot.GetAsmLot(m.LotNo);
                        procList = GetDBOrderAndMachines(lot.TypeCd, lot.NascaLotNo, out isComplete);
                    }

                    foreach (var p in procList)
                    {
                        var wafers = GetWaferLot(lot, (long)m.MarkData, m.Row, m.ProcNo, p.Key, p.Value);

                        using (MAKTEntities ent = new MAKTEntities())
                        {
                            foreach (var w in wafers)
                            {
                                var exists = (from d in ent.TnMaterial
                                              where d.markdata == w.markdata && d.lotno == lot.NascaLotNo && d.typecd == lot.TypeCd && d.materialcd == w.materialcd && d.matlotno == w.matlotno
                                              select d).FirstOrDefault();

                                if (exists != null)
                                {
                                    continue;
                                }
                                else
                                {
                                    ent.TnMaterial.Add(w);
                                }
                            }
                            ent.SaveChanges();
                        }
                    }

                    if (isComplete)
                    {
                        //ウェハー集計フラグをON
                        m.WaferCollectFg = true;
                        m.Update();
                    }

                    FrmMAKTMain.AppendLog("wafer:" + i.ToString() + " / " + marks.Count.ToString() + " " + m.LotNo);
                    i++;
                }
                catch (Exception ex)
                {
                    Log.SysLog.Error("MAKT wafer trace error:" + m.LotNo + ":" + m.MarkData + ":" + ex.ToString());
                }
            }

        }


        private static SortedList<int, MachineInfo> GetDBOrderAndMachines(string typeCd, string lotNo, out bool isCompleteWaferTrace)
        {
            SortedList<int, MachineInfo> retv = new SortedList<int, MachineInfo>();

            Order[] orders = Order.GetOrder(lotNo);
            foreach (var o in orders)
            {
                var mat = o.GetMaterials();
                var usedwafer = mat.Where(m => m.IsWafer == true);

                if (usedwafer.Count() >= 1)
                {
                    MachineInfo mac = MachineInfo.GetMachine(o.MacNo);
                    retv.Add(o.ProcNo, mac);
                }
            }
            if (orders.Where(o => o.ProcNo == Process.GetLastProcess(typeCd).ProcNo).Count() >= 1)
            //if (orders.Where(o => Process.GetProcess(o.ProcNo).FinalSt == true).Count() >= 1)
            {
                isCompleteWaferTrace = true;
            }
            else if (orders.Max(o => o.WorkStartDt) <= DateTime.Now.AddMonths(-3))
            {
                //最終の作業履歴が3か月以上前なら何らかの問題で最終工程まで到達しないロットと判断してウェハートレース終了
                isCompleteWaferTrace = true;
            }
            else
            {
                isCompleteWaferTrace = false;
            }

            return retv;
        }
        
        /// <summary>
        /// 現在完了している全工程のウェハーリスト取得
        /// 最終工程までの履歴が揃っていた場合はisCompleteWaferTrace=true
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="row"></param>
        /// <param name="markingProcNo"></param>
        /// <param name="isCompleteWaferTrace"></param>
        /// <returns></returns>
        public static List<TnMaterial> GetWaferLot(AsmLot lot, long markdata, int row, int markingProcNo, int dbProcNo, MachineInfo machine)
        {
            List<TnMaterial> retv = new List<TnMaterial>();

            Order dbOrder = Order.GetOrder(lot.NascaLotNo, dbProcNo).First();
            var mat = dbOrder.GetMaterials();
            var usedwafer = mat.Where(m => m.IsWafer == true);

            if (usedwafer.Count() >= 1)
            {
                var logs = searchByAsmLot(lot, markingProcNo, dbOrder.ProcNo, row);

                //ARMSのウェハー情報を正として全部登録する。
                //ログと一致しているウェハーにはフラグON
                foreach (var w in usedwafer)
                {
                    TnMaterial d = new TnMaterial();
                    d.lotno = lot.NascaLotNo;
                    d.typecd = lot.TypeCd;
                    d.plantcd = machine.NascaPlantCd;
                    d.materialcd = w.MaterialCd;
                    d.matlotno = w.LotNo;
                    d.lastupddt = DateTime.Now;
                    d.markdata = markdata;

                    if (logs.Where(l => l.LotNo == d.lotno).Count() >= 1)
                    {
                        d.importantfg = 1;
                    }
                    else
                    {
                        d.importantfg = 0;
                    }

                    retv.Add(d);
                }
            }

            return retv;
        }

        private static Material[] searchByAsmLot(AsmLot lot, int markingProcNo, int diebondProcNo, int rowno)
        {
            List<Material> retv = new List<Material>();

            //指定工程との間に反転がある場合は段数を反転
            bool isReverse = Process.IsReverseFramePlacement(lot.NascaLotNo, markingProcNo, diebondProcNo);
            if (isReverse == true)
            {
                int? step = ArmsApi.Model.LENS.Mag.GetMagStep(lot.TypeCd);
                int? stepcd = ArmsApi.Model.LENS.Mag.GetMagStepCd(lot.TypeCd);
                if (step == null || stepcd == null)
                {
                    throw new ApplicationException("LENS TmMagマスタにStepCdの設定がありません:" + lot.TypeCd);
                }
                rowno = Magazine.ReverseRow(rowno, step.Value, stepcd.Value);
            }


            Order[] orders = Order.GetOrder(lot.NascaLotNo, diebondProcNo);
            if (orders.Length == 0)
            {
                return new Material[0];
            }
            MachineInfo mac = MachineInfo.GetMachine(orders[0].MacNo);
            DateTime workEndDt = orders[0].WorkEndDt.Value;

            DBWaferLog[] logs = DBWaferLog.GetAllLogs(mac.NascaPlantCd, workEndDt.AddDays(-1), workEndDt);
            logs = logs.OrderByDescending(l => l.LogDT).ToArray();

            bool isLotChange = false;
            foreach (var log in logs)
            {
                if (log.KB == DBWaferLog.LogKB.ロット完成 && log.AsmLotNo != lot.NascaLotNo)
                {
                    //別ロットの完成レコード以降のウェハー交換については段数判定しない
                    isLotChange = true;
                }

                if (log.KB == DBWaferLog.LogKB.ウェハー段数変更_自動 || log.KB == DBWaferLog.LogKB.ウェハー段数変更_手動)
                {
                    if (isLotChange)
                    {
                        //ロット交換を挟んでいた場合は最初に見つかったウェハーだけで終了
                        retv.Add(log.Wafer);
                        break;
                    }

                    if (log.MagRowNo > rowno)
                    {
                        //検索段数以降の交換ログは無視する
                        continue;
                    }

                    if (log.MagRowNo < rowno)
                    {
                        //検索段数より前の交換ログが見つかった場合も終了
                        retv.Add(log.Wafer);
                        break;
                    }

                    if (log.MagRowNo == rowno)
                    {
                        //検索段数と同一段数の交換ログなら前半に別ウェハーが使われているので続行
                        retv.Add(log.Wafer);
                    }
                }
            }

            return retv.ToArray();
        }

    }
}
