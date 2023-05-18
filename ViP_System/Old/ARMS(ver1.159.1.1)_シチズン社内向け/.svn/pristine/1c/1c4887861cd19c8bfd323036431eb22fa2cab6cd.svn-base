using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class ResinMixOrderModel
    {
        public ResinMixOrderModel(string magno)
        {
            this.MixReqInfo = new ArmsApi.Model.NJDB.MixReqestInfo();

            this.MixReqInfo.PriorityFg = false;
            this.MixReqInfo.PreFg = false;
            this.Mag = Magazine.GetCurrent(magno);
            this.Lot = AsmLot.GetAsmLot(this.Mag.NascaLotNO);
            this.MixReqInfo.LotCt = 1;
            this.MixReqInfo.MachineCt = 1;
            this.MixReqInfo.UpdUserCd = string.Empty;
            this.MixReqInfo.ExpectCompDt = DateTime.Now;
            this.MixReqInfo.StartDt = null;
            this.MixReqInfo.EndDt = null;
            this.ExpectPlaceCdList = ArmsApi.Config.Settings.ExpectPlaceCdList;
        }

        /// <summary>
        /// 樹脂調合依頼情報
        /// </summary>
        public ArmsApi.Model.NJDB.MixReqestInfo MixReqInfo { get; set; }

        /// <summary>
        /// マガジン情報
        /// </summary
        public Magazine Mag { get; set; }

        /// <summary>
        /// アッセンロット情報
        /// </summary>
        public AsmLot Lot { get; set; }

        /// <summary>
        /// 次のMD作業
        /// </summary>
        public Process NextResinProc { get; set; }

        /// <summary>
        /// 調合場所リスト
        /// </summary>
        public List<string> ExpectPlaceCdList { get; set; }

        /// <summary>
        /// 次MD作業用の樹脂グループCDと調合タイプ名を取得
        /// </summary>
        public bool GetResinGroupAndMixTypeNm(out string msg)
        {
            msg = string.Empty;
            string sMagInfo =
                $"マガジンNo：{this.Mag.MagazineNo}, ロットNo：{this.Lot.NascaLotNo}, 完了工程：{this.Mag.NowCompProcessNm}";

            try
            {
                // 次のMD作業を取得
                this.NextResinProc = Process.GetNextResinProcess(this.Mag.NowCompProcess, this.Lot);
                if (this.NextResinProc == null)
                {
                    msg = $"完了工程以降にMD作業がありません。" + sMagInfo;
                    return false;
                }

                // 次のMD作業用の調合タイプ名を取得
                List<string> unSetMixTypeCodes = new List<string>();
                foreach (string mixTypeCd in this.NextResinProc.MixTypeCd)
                {
                    string mixTypeNm = Resin.GetMixTypeNm(mixTypeCd);
                    if (string.IsNullOrEmpty(mixTypeNm))
                    {
                        unSetMixTypeCodes.Add(mixTypeCd);    
                    }
                }

                if (unSetMixTypeCodes.Any())
                {
                    msg = $"次の樹脂使用作業用の調合タイプCD「{string.Join(",", unSetMixTypeCodes)}」が樹脂調合システムの汎用マスタに設定されていません。"
                            + sMagInfo
                            + $", 次の樹脂使用作業：{this.NextResinProc.InlineProNM}";
                    return false;
                }

                // 次のMD作業用の樹脂グループCDを取得
                List<string> resinGroupCdList = new List<string>();
                foreach (string mixTypeCd in this.NextResinProc.MixTypeCd)
                {
                    resinGroupCdList.AddRange(Resin.GetResinGroupCdList(this.Lot.ResinGpCd, mixTypeCd));
                }
                if (resinGroupCdList.Count == 0)
                {
                    msg = $"次の樹脂使用作業用の樹脂グループがロットに紐づいていません。"
                        + sMagInfo
                        + $", 次の樹脂使用作業：{this.NextResinProc.InlineProNM}, 調合タイプ：{string.Join(",", this.MixReqInfo.MixTypeNm)}";
                    return false;
                }
                else if (resinGroupCdList.Count >= 2)
                {
                    msg = $"次の樹脂使用作業用の樹脂グループがロットに複数紐づいています。樹脂グループ = ({string.Join(",", resinGroupCdList)})"
                        + sMagInfo
                        + $", 次の樹脂使用作業：{this.NextResinProc.InlineProNM}, 調合タイプ：{string.Join(",", this.MixReqInfo.MixTypeNm)}";
                    return false;
                }
                this.MixReqInfo.ResinGroupCd = resinGroupCdList.FirstOrDefault();

                return true;
            }
            catch (Exception ex)
            {
                msg = ex.Message + "\r\n" + sMagInfo;
                return false;
            }
        }

        /// <summary>
        /// 調合依頼情報を登録
        /// </summary>
        public void InsertMixRequest()
        {
            this.MixReqInfo.SecCd = ArmsApi.Config.Settings.ResinDBServerCd;
            this.MixReqInfo.ReqDt = DateTime.Now;
            this.MixReqInfo.ActualPlaceCd = string.Empty;
            this.MixReqInfo.StatusCd = 0;
            this.MixReqInfo.CommentNm = string.Empty;

            this.MixReqInfo.Insert();
        }
    }
}