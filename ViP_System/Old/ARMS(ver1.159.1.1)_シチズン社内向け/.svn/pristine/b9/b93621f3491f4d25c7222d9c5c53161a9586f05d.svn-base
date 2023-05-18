using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArmsApi.Model;
using System.Data;
using System.Data.SqlClient;

namespace MAKT
{
    class FrameInfo
    {
        public string LotNo { get; set; }
        public int Row { get; set; }
        public int StockerNo { get; set; }
        public decimal MarkData { get; set; }
        public DateTime WorkDt { get; set; }
        public bool ManualFg { get; set; }
        public bool FrameCollectFg { get; set; }
        public bool WaferCollectFg { get; set; }
        public int WaferCollectCt { get; set; }
        public string TypeCd { get; set; }
        public DateTime LastUpdDt { get; set; }

        public static void Crawl()
        {
            var marks = LotMarkData.Search(null, null, null, null, false, null);

            int i = 1;
            foreach (var m in marks)
            {
                AsmLot lot = AsmLot.GetAsmLot(m.LotNo);
                if (lot == null) continue;

                FrameInfo f = new FrameInfo();
                f.LotNo = m.LotNo;
                f.Row = m.Row;
                f.StockerNo = m.StockerNo;
                f.WorkDt = m.WorkDt;
                f.WaferCollectFg = m.WaferCollectFg;
                f.FrameCollectFg = true;
                f.MarkData = m.MarkData;
                f.ManualFg = m.ManualFg;
                f.TypeCd = lot.TypeCd;

                //トレースDB更新
                f.Commit();

                var mat = GetFrameLot(lot, f.StockerNo, m);

                using (MAKTEntities ent = new MAKTEntities())
                {
                    try
                    {
                        foreach (var w in mat)
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
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        foreach (var errors in ex.EntityValidationErrors)
                        {
                            foreach (var error in errors.ValidationErrors)
                            {
                                System.Diagnostics.Trace.WriteLine(error.ErrorMessage);    // VisualStudioの出力に表示
                            }
                        }
                        throw ex;
                    }
                }


                //フレーム集計フラグをON
                m.FrameCollectFg = true;
                m.Update();

                FrmMAKTMain.AppendLog(i.ToString() + " / " + marks.Count.ToString() + " " + f.LotNo);
                i++;
            }
        }

        public void Commit()
        {
            using (MAKTEntities ent = new MAKTEntities())
            {
                var m = (from d in ent.TnMarkingLog
                        where d.lotno == this.LotNo && d.row == this.Row
                        select d).FirstOrDefault();

                if (m != null)
                {
                    m.flamecollectfg = SQLite.SerializeBool(this.FrameCollectFg);
                    m.lastupddt = DateTime.Now;

                    ent.SaveChanges();
                }
                else
                {

                    try
                    {
                        TnMarkingLog mk = new TnMarkingLog();
                        mk.lotno = this.LotNo;
                        mk.row = this.Row;
                        mk.stockerno = this.StockerNo;
                        mk.markdata = (long)this.MarkData;
                        mk.workdt = this.WorkDt;
                        mk.manualfg = SQLite.SerializeBool(this.ManualFg);
                        mk.flamecollectfg = 1;
                        mk.wafercollectfg = 0;
                        mk.typecd = this.TypeCd;
                        mk.lastupddt = DateTime.Now;
                        mk.materialcd = "";
                        mk.matlotno = "";
                        ent.TnMarkingLog.Add(mk);
                        ent.SaveChanges();
                    }
                    catch (System.Data.Entity.Validation.DbEntityValidationException ex)
                    {
                        foreach (var errors in ex.EntityValidationErrors)
                        {
                            foreach (var error in errors.ValidationErrors)
                            {
                                System.Diagnostics.Trace.WriteLine(error.ErrorMessage);    // VisualStudioの出力に表示
                            }
                        }
                        throw ex;
                    }

                }
            }
        }


        public static List<TnMaterial> GetFrameLot(AsmLot lot, int stockerno, LotMarkData mark)
        {
            List<TnMaterial> retv = new List<TnMaterial>();

            Order[] orders = Order.GetOrder(lot.NascaLotNo);

            Order ord = null;
            Material[] mat = null;
            foreach (var o in orders)
            {
                //原材料としてフレームが使われている指図を特定する
                mat = o.GetMaterials();
                if (mat.Where(m => m.IsFrame).Count() >= 1)
                {
                    ord = o;
                    break;
                }
            }

            if (ord == null) return retv;

            MachineInfo machine = MachineInfo.GetMachine(ord.MacNo);

            foreach (Material m in mat)
            {
                TnMaterial d = new TnMaterial();
                d.lotno = lot.NascaLotNo;
                d.typecd = lot.TypeCd;
                if (machine != null)
                {
                    d.plantcd = machine.NascaPlantCd;
                }
                else
                {
                    d.plantcd = string.Empty;
                }
                
                d.materialcd = m.MaterialCd;
                d.matlotno = m.LotNo;
                d.lastupddt = DateTime.Now;
                d.markdata = (long)mark.MarkData;

                //重要フラグON

                //同一アッセンロットに同じ材料が複数回登録される問題の対応
                if (retv.Find(r => r.materialcd == d.materialcd && r.matlotno == d.matlotno) == null)
                {
                    retv.Add(d);
                }


            }

            return retv;

        }
    }
}
