using ArmsApi.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
    public class PreCutBlendModel
    {
        /// <summary>
        /// 登録作業者
        /// </summary>
        public string EmpCd { get; set; }

        /// <summary>
        /// 読み込みロットリスト
        /// </summary>
        public List<ArmsApi.Model.Magazine> MagList { get; set; }

        public PreCutBlendModel() 
        {
            this.MagList = new List<Magazine>();
        }

        public bool PreCutBlendSubmit(out string msg) 
        {
            List<AsmLot> lots = new List<AsmLot>();
            foreach (Magazine mag in this.MagList)
            {
                AsmLot lot = AsmLot.GetAsmLot(mag.NascaLotNO);
                lots.Add(lot);
            }



            if (WorkChecker.IsCutBlendError(lots.ToArray(), out msg, false))
            {
                return false;
            }

            if (AsmLot.UpdatePreCutBlendNo(this.MagList.Select(m => m.NascaLotNO).ToArray()))
            {
                msg = "";
                return true;
            }
            else 
            {
                msg = "登録失敗";
                return false;
            }
        }
    }
}