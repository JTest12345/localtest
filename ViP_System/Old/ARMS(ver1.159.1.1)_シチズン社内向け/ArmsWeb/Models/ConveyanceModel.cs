using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
    public class ConveyanceModel
    {
        /// <summary>
        /// 搬送作業の分類
        /// </summary>
        private const string CONVEYANCE_PROC = "CONVEY";

        public ConveyanceModel(string empcd)
        {
            MagList = new List<Magazine>();

            this.EmpCd = empcd;
        }

        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        public List<Magazine> MagList { get; set; }


        /// <summary>
        /// 搬送作業開始・完了登録
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool WorkStartEndConveyance(out string errMsg)
        {
            foreach (ArmsApi.Model.Magazine mag in MagList)
            {
                //念のため最新情報取得
                ArmsApi.Model.Magazine mag2 = Magazine.GetCurrent(mag.MagazineNo);
                if (mag2 == null)
                {
                    errMsg = "マガジン情報が見つかりません:" + mag.MagazineNo;
                    return false;
                }

                bool isSuccecc = endMag(mag2, out errMsg);
                if (!isSuccecc)
                {
                    return false;
                }
            }
            
            errMsg = "";
            return true;
        }

        private bool endMag(Magazine mag, out string msg)
        {
            try
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                if (lot == null)
                {
                    msg = "ロット情報が存在しません";
                    return false;
                }

                Process p = Process.GetNextProcess(mag.NowCompProcess, lot);
                if (p == null)
                {
                    msg = "工程情報が存在しません";
                    return false;
                }

                if (p.ProcGroupCd != CONVEYANCE_PROC)
                {
                    msg = "工程が対象工程(搬送作業)ではありません";
                    return false;
                }

                if (p.AutoUpdMachineNo == null)
                {
                    msg = "設備情報が登録されていません";
                    return false;
                }

                Order order = new Order();
                order.LotNo = mag.NascaLotNO;
                order.ProcNo = p.ProcNo;
                order.InMagazineNo = mag.MagazineNo;
                order.OutMagazineNo = mag.MagazineNo;
                order.MacNo = p.AutoUpdMachineNo.Value;
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

                order.DeleteInsert(order.LotNo);
                
                msg = "";
                return true;
            }
            catch (Exception ex)
            {
                msg = "エラーが発生したため処理を中断しました " + ex.ToString();
                return false;
            }
        }
    }
}