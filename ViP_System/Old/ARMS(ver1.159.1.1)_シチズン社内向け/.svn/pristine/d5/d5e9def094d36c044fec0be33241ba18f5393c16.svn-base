using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class DBPreOvnPLInstModel
    {
        private const string PL_WORK_CD = "DB0027";

        public DBPreOvnPLInstModel()
        {
            this.Insts = new List<DBPreOvnPLInst>();
            try
            {
                Process plProc = Process.GetProcess(PL_WORK_CD);

                TimeLimit[] limits = TimeLimit.GetLimits(null, plProc.ProcNo, false);
                SortedList<string, TimeLimit> includeTypes = new SortedList<string, TimeLimit>();
                List<int> includeProcs = new List<int>();

                foreach (var lim in limits)
                {
                    //マイナス時間を持っている設定しか考慮しない
                    if (lim.EffectLimit < 0)
                    {
                        includeTypes.Add(lim.TypeCd, lim);
                    }
                }

                Magazine[] mags = Magazine.GetMagazine(true);

                foreach (Magazine m in mags)
                {
                    AsmLot lot = AsmLot.GetAsmLot(m.NascaLotNO);

                    //監視タイプに含まれていれば処理無し
                    if (includeTypes.Keys.Contains(lot.TypeCd) == false) continue;

                    //硬化前プラズマ開始ロットは通知なし
                    Order ord = Order.GetMagazineOrder(lot.NascaLotNo, plProc.ProcNo);
                    if (ord != null)
                    {
                        continue;
                    }

                    TimeLimit limit = includeTypes[lot.TypeCd];
                    Process dbproc = limit.TgtProc;

                    ord = Order.GetMagazineOrder(lot.NascaLotNo, dbproc.ProcNo);

                    //DB完了前ロットは通知なし
                    if (ord == null || ord.WorkEndDt.HasValue == false)
                    {
                        continue;
                    }

                    //ダイボンド終了時刻から現時刻が時間監視のマイナス値以下なら通知なし
                    if ((DateTime.Now - ord.WorkEndDt.Value).TotalMinutes <= Math.Abs(limit.EffectLimit))
                    {
                        continue;
                    }

                    DBPreOvnPLInst ins = new DBPreOvnPLInst();
                    ins.Lot = lot;
                    ins.MagNo = m.MagazineNo;
                    ins.ProcNo = m.NowCompProcess;

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
        /// 6時間経過レコードのリスト
        /// </summary>
        public List<DBPreOvnPLInst> Insts { get; set; }

        public string ErrMsg { get; set; }
    }

    public class DBPreOvnPLInst
    {
        public int ProcNo { get; set; }
        public string MagNo { get; set; }
        public AsmLot Lot { get; set; }
    }

}