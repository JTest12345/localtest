using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class LotViewModel
    {
        public LotViewModel(string magno)
        {
            this.MagNo = magno;

            this.Magazine = Magazine.GetCurrent(magno);

            if (this.Magazine != null)
            {
                LotNo = this.Magazine.NascaLotNO;
                this.LotLog = AsmLot.GetLotLog(LotNo);
                this.Lot = AsmLot.GetAsmLot(this.Magazine.NascaLotNO);

                //TODO マガジン分割に対応させるならFirstではなく、正確に分割マガジンの情報で取得
                Order lastorder = Order.GetOrder(this.Magazine.NascaLotNO, Magazine.NowCompProcess).FirstOrDefault();
                if (lastorder != null)
                {
                    this.LastMachine = MachineInfo.GetMachine(lastorder.MacNo);
                }

                // 狙いランク取得
                this.Profile = Profile.GetProfile(this.Lot.ProfileId);

                // 各種有効期限取得
                this.LcrList = GetTimeLimitFromLot(this.Lot);

                // 投入禁止期限取得
                this.LcrList_FI = GetTimeLimitForbiddenInputFromLot(this.Lot);

                // 次投入ライン
                //20220711 MOD START（次工程を表示するように変更）
                //string msg;
                //this.NextLine = Magazine.GetNextLine(this.Magazine, out msg);
                string msg = string.Empty;
                Process prc = Process.GetNextProcess(Magazine.NowCompProcess, this.Lot);
                if (prc != null)
                {
                    Magazine.NextProcess = prc.ProcNo;
                    this.NextLine = prc.ProcNo + " (" + Magazine.NextProcessNm + ")";
                }
                else
                {
                    this.NextLine = null;
                }
                //20220711 MOD END
                this.NextLineComment = msg;
            }
            else
            {
                isNotFound = true;
            }
        }

        public bool isNotFound { get; set; }

        public string MagNo { get; set; }

        /// <summary>
        /// FindOther照合用
        /// </summary>
        public bool IsMatched { get; set; }

        public Magazine Magazine { get; set; }

        public string LotNo { get; set; }

        public AsmLot Lot { get; set; }

        public MachineInfo WBMachine { get; set; }

        public Order WBOrder { get; set; }

        public SortedList<DateTime, string> LotLog { get; set; }

        public MachineInfo LastMachine { get; set; }

        public Profile Profile { get; set; }

        public LimitCheckResult[] LcrList { get; set; }

        public List<LimitCheckResult> LcrList_FI { get; set; }

        private KeyValuePair<GnlMaster, int>? nextLineInfo { get; set; }

        public string NextLine { get; set; }

        public string NextLineComment { get; set; }

        /// <summary>
        /// 分割マガジンの片割れ表示
        /// </summary>
        /// <param name="mag"></param>
        /// <returns></returns>
        public Magazine FindOtherMag(Magazine mag)
        {
            string lotno = Order.MagLotToNascaLot(mag.NascaLotNO);
            Magazine[] mags = Magazine.GetMagazine(null, lotno, true, null);
            return mags.Where(m => m.MagazineNo != mag.MagazineNo).FirstOrDefault();
        }


        /// <summary>
        /// 次作業装置として表示する内容
        /// </summary>
        public string NextGroupString
        {
            get
            {
                if (Lot == null || Lot.MacGroup == null || Lot.MacGroup.Count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    List<string> retv = new List<string>();
                    foreach (string grp in Lot.MacGroup)
                    {
                        GnlMaster[] master = ArmsApi.Model.GnlMaster.Search("macgroup", grp, null, null);
                        if (master.Length >= 1)
                        {
                            retv.Add(master[0].Val);
                        }
                        else
                        {
                            //装置グループマスターが無い場合はMacGroupの文字をそのまま出力
                            retv.Add(grp);
                        }
                    }

                    return string.Join(", ", retv);
                }
            }
        }

        /// <summary>
        /// 作業監視中リストとして表示する内容
        /// </summary>
        public LimitCheckResult[] GetTimeLimitFromLot(AsmLot lot)
        {
            // 各種有効期限取得
            TimeLimit[] limitList = TimeLimit.GetLimits(lot.TypeCd, null);
            LimitCheckResult[] lcrList = TimeLimit.CheckTimeLimit(lot, limitList, null, true, null);

            return lcrList;
        }

        /// <summary>
        /// 作業監視中(投入禁止)リストとして表示する内容
        /// </summary>
        public List<LimitCheckResult> GetTimeLimitForbiddenInputFromLot(AsmLot lot)
        {
            // 各種有効期限取得
            List<LimitCheckResult> LcrList = new List<LimitCheckResult>();
            TimeLimit[] limitList = TimeLimit.GetLimits(lot.TypeCd, null);
            foreach(TimeLimit limit in limitList)
            {
                //ここではマイナス作業時間のみ判定する
                if (limit.EffectLimit　>= 0) continue;

                //対象工程の実績確認
                Order chkOrd = Order.GetMagazineOrder(lot.NascaLotNo, limit.ChkProc.ProcNo);
                // 対象区分 = Start →  実績がある場合は、表示なし
                if (limit.ChkKb == TimeLimit.JudgeKb.Start)
                {
                    if (chkOrd != null) continue;
                }
                // 対象区分 = End →  完了時刻がある場合は、表示なし
                else
                {
                    if (chkOrd != null && chkOrd.WorkEndDt.HasValue == true) continue;
                }

                // 監視工程の実績確認
                Order tgtOrd = Order.GetMagazineOrder(lot.NascaLotNo, limit.TgtProc.ProcNo);
                DateTime tgtDt;
                // 監視区分 = Start →  実績がない場合は、表示なし
                if (limit.TgtKb == TimeLimit.JudgeKb.Start)
                {
                    if (tgtOrd == null) continue;
                    tgtDt = tgtOrd.WorkStartDt;
                }
                // 監視区分 = End →  完了時刻がない場合は、表示なし
                else
                {
                    if (tgtOrd == null || tgtOrd.WorkEndDt.HasValue == false) continue;
                    tgtDt = tgtOrd.WorkEndDt.Value;
                }
                
                LimitCheckResult lcr = new LimitCheckResult(lot.NascaLotNo, limit, tgtDt.AddMinutes(Math.Abs(limit.EffectLimit)), LimitCheckResult.ResultKb.Normal);
                LcrList.Add(lcr);
            }

            return LcrList;
        }        
    }
}