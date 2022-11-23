using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class AfterCuringConfirmModel
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public AfterCuringConfirmModel()
        {
            string errMsg = "";
            int row = 0;

            AsmLot[] lots = AsmLot.SerachAfterCuringConfirm();

            this.LotList = new Dictionary<int, AsmLot>();

            //一覧編集
            for (int i = 0; i < lots.Length; i++)
            {
                if (CheckProcNo(lots[i].TypeCd, lots[i].NascaLotNo, out errMsg))
                {
                    this.LotList.Add(row, lots[i]);
                    row += 1;
                }
            }
        }

        public List<AsmLot> EditTarget { get; set; }

        public Dictionary<int, AsmLot> LotList { get; set; }

        /// <summary>
        /// 樹脂キュア硬化後工程確認飛ばし
        /// </summary>
        /// <param name="lot"></param>
        public void RemoveAfterCuringConfirm(AsmLot lot)
        {
            ArmsApi.Model.AsmLot.UpdateAfterCuringConfirm(lot.NascaLotNo, 0);
        }

        /// <summary>
        /// バーコード内容からlot情報を返す。
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        public AsmLot ParseBarCode(string code)
        {
            try
            {
                AsmLot lot = AsmLot.GetAsmLot(code);
                //
                if (lot == null)
                {
                    //ロット番号でない場合、マガジン番号として再検索
                    Magazine mag = Magazine.GetCurrent(code);
                    //TnLot検索
                    if (mag != null)
                    {
                        lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                    }
                }
                return lot;
            }
            catch (Exception ex)
            {
                throw new ApplicationException(ex.Message);
            }
        }

        /// <summary>
        /// TnLot更新（aftercuringconfirm <- 1)
        /// </summary>
        public void UpdateNew()
        {
            foreach (AsmLot w in this.EditTarget)
            {
                ArmsApi.Model.AsmLot.UpdateAfterCuringConfirm(w.NascaLotNo, 1);
            }
        }

        /// <summary>
        /// 入力したロット番号チェック
        /// </summary>
        /// <param name="typecd"></param>
        /// <param name="lotno"></param>
        /// <param name="errmsg"></param>
        /// <returns></returns>
        public bool CheckProcNo(string typecd, string lotno, out string errmsg)
        {
            errmsg = "";

            //TnWorkFlowを検索し、対象工程の１つ前の工程を求める
            Process p = Process.GetPrevProcess(ArmsApi.Config.Settings.AFTER_CURING_CONFIRM, typecd);

            if (p == null)
            {
                errmsg = "ロット番号に誤りがあります。";
                return false;
            }

            //TnTranを検索し、対象データを求める
            Order[] orders = Order.GetOrder(lotno, p.ProcNo);
            if (orders == null)
            {
                //存在しなければＯＫ
                return true;
            }
            else
            {
                foreach(Order o in orders)
                {
                    if (o.WorkEndDt.HasValue)
                    {
                        errmsg = "既に樹脂キュア硬化後確認工程を超えております。";
                        return false;
                    }
                }
            }
            return true; 
       }
    }
}