using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ArmsWeb.Models
{
    public class ResinMeasurementTrayCompareModel
    {
        public ResinMeasurementTrayCompareModel(string plantcd)
        {
            this.PlantCd = plantcd;
        }

        /// <summary>
        /// 設備番号
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// トレイのラベル
        /// </summary>
        public string TrayNo { get; set; }

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

            if (this.PlantCd.ToUpper().Trim() != this.TrayNo.ToUpper().Trim())
            {
                IsOK = false;
                errMsg = $"照合NG：設備番号が一致しません 設備「{this.PlantCd}」、トレイ「{this.TrayNo}」";
            }
            else
            {
                IsOK = true;
                errMsg = "照合OK。";
            }

            Msg = errMsg;
        }
    }
}