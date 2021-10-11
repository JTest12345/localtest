using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;
using System.IO;

namespace ArmsWeb.Models
{
    public class CutLabelCompareModel
    {
        /// <summary>
        /// 装置
        /// </summary>
        public string PlantCd { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public MachineInfo Mac { get; set; }

        /// <summary>
        /// カットラベル
        /// </summary>
        public string CutLabel { get; set; }

        /// <summary>
        /// 判定結果
        /// </summary>
        public bool IsOK { get; set; }

        /// <summary>
        /// エラー内容
        /// </summary>
        public string Msg { get; set; }

        public CutLabelCompareModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public void Compare(string cutLabel)
        {
            string errMsg;
            IsOK = CutBlend.CompareCutLabel(cutLabel, this.Mac.MacNo, out errMsg);
            Msg = errMsg;
        }

    }
}