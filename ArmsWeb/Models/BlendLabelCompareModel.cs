using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
    public class BlendLabelCompareModel
    {
        public BlendLabelCompareModel(string plantcd)
        {
            AsmLotList = new List<string>();

            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
        }

        /// <summary>
        /// 設備番号
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 作業者
        /// </summary>
        public string EmpCd { get; set; }

        public MachineInfo Mac { get; set; }

        /// <summary>
        /// 親ロット
        /// </summary>
        public string BlendLotNo { get; set; }

        /// <summary>
        /// 構成ロット（子ロット）
        /// </summary>
        public List<string> AsmLotList { get; set; }

        /// <summary>
        /// 判定結果
        /// </summary>
        public bool IsOK { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        public string Msg { get; set; }


        /// <summary>
        /// 照合
        /// </summary>
        /// <returns></returns>
        public void Compare()
        {
            string errMsg;
            IsOK = false;

            ArmsApi.Model.CutBlend[] lst = ArmsApi.Model.CutBlend.SearchBlendRecord(null, BlendLotNo, null, false, false);

            if (lst == null || lst.Count() == 0)
            {
                IsOK = false;
                errMsg = "照合NG：親ロット情報が存在しません";
            }
            else
            {
                List<string> asmLotList = lst.Select(r => r.LotNo).ToList();

                List<string> notExistList = new List<string>();
                foreach (string asmLotno in this.AsmLotList)
                {
                    if (!asmLotList.Exists(r => r == asmLotno))
                    {
                        notExistList.Add(asmLotno);
                    }
                }

                List<string> notExistList_DB = new List<string>();
                foreach (string asmLotno in asmLotList)
                {
                    if (!this.AsmLotList.Exists(r => r == asmLotno))
                    {
                        notExistList_DB.Add(asmLotno);
                    }
                }

                if (notExistList.Count == 0 && notExistList_DB.Count == 0)
                {
                    IsOK = true;
                    errMsg = "照合OK。";
                }
                else
                {
                    IsOK = false;
                    errMsg = "照合NG";
                    if (notExistList.Count != 0)
                    {
                        errMsg += string.Format("\r\n{0}は紐付けされていません。", string.Join(",", notExistList));
                    }
                    if (notExistList_DB.Count != 0)
                    {
                        errMsg += string.Format("\r\n{0}が照合対象として読み込まれていません。", string.Join(",", notExistList_DB));
                    }
                }
            }


            Msg = errMsg;
        }

        /// <summary>
        /// 作業開始前チェック
        /// </summary>
        /// <param name="mag"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool EndMag(out string msg)
        {
            try
            {
                AsmLot lot = null;
                foreach (string lotno in AsmLotList)
                {
                    lot = AsmLot.GetAsmLot(lotno);
                    if (lot == null)
                    {
                        msg = "ロット情報が存在しません：ロットNo『" + lotno + "』";
                        return false;
                    }
                }

                //ブレンドロットの最終工程を取得
                int nowCompProcess = Order.GetLastProcNoFromLotNo(BlendLotNo);

                Process p = Process.GetNextProcess(nowCompProcess, lot);
                if (p == null)
                {
                    msg = "工程情報が存在しません：工程No『" + nowCompProcess + "』";
                    return false;
                }

                Process.MagazineDevideStatus dst = Process.GetMagazineDevideStatus(lot, p.ProcNo);

                Order order = Order.GetMagazineOrder(BlendLotNo, p.ProcNo);
                if (order == null || dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    order = new Order();
                    order.LotNo = BlendLotNo;
                    order.ProcNo = p.ProcNo;
                }

                order.MacNo = this.Mac.MacNo;
                order.WorkStartDt = DateTime.Now;
                order.WorkEndDt = DateTime.Now;

                order.TranStartEmpCd = this.EmpCd;
                order.TranCompEmpCd = this.EmpCd;
                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }

                if (p.IsNascaDefect)
                {
                    order.IsDefectEnd = false;
                }
                else
                {
                    order.IsDefectEnd = true;
                }
                string errMsg;

                //IsErrorBeforeStartは分割ロット番号で確認する
                bool isStartError = WorkChecker.IsErrorBeforeStartWork(lot, Mac, order, lot.NascaLotNo, p, out errMsg);

                if (dst == Process.MagazineDevideStatus.DoubleToSingle)
                {
                    order.LotNo = Order.MagLotToNascaLot(order.LotNo);
                }

                if (isStartError)
                {
                    msg = errMsg;
                    return false;
                }

                bool isError = WorkChecker.IsErrorWorkComplete(order, Mac, lot, out errMsg);

                if (isError)
                {
                    msg = errMsg + " ロットは完了しましたが警告状態になっています。";
                    lot.IsWarning = true;
                    lot.Update();
                    order.Comment += errMsg;
                    order.DeleteInsert(order.LotNo);

                    return false;
                }
                else
                {
                    order.DeleteInsert(order.LotNo);

                    msg = "";

                    return true;
                }
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }
    }
}