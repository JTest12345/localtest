using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;
using System.Xml;
using System.Collections.Specialized;

namespace ArmsWeb.Models
{
    public class EicsTypeChangeModel
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
        /// 変更先タイプ
        /// </summary>
        public string NewType { get; set; }

        /// <summary>
        /// 変更元タイプ
        /// </summary>
        public string OldType { get; set; }

        /// <summary>
        /// 検索結果
        /// </summary>
        public List<string> SearchResult { get; set; }

        public string LotNo { get; set; }


        /// <summary>
        /// タイプの取得
        /// </summary>
        public void GetType(string magno)
        {
            ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magno);
            if (mag == null)
            {
                throw new ApplicationException("マガジンが存在しません：" + magno);
            }

            ArmsApi.Model.AsmLot lot = ArmsApi.Model.AsmLot.GetAsmLot(mag.NascaLotNO);
            if (lot == null)
            {
                throw new ApplicationException("ロットが存在しません：" + magno);
            }

            this.LotNo = lot.NascaLotNo;
            this.NewType = lot.TypeCd;
        }

        /// <summary>
        /// タイプ候補の検索
        /// </summary>
        public void GetSearchResultTypeList(string searchCond)
        {
            string plantModel = ArmsApi.Model.MachineInfo.GetEICSModelNm(this.PlantCd);
            this.SearchResult = ArmsApi.Model.MachineInfo.GetEICSThrowTypeList(plantModel, searchCond);
        }


        public EicsTypeChangeModel(string plantcd)
        {
            this.PlantCd = plantcd;
            this.Mac = MachineInfo.GetMachine(plantcd);
        }

        public bool UpdateEicsType()
        {
            bool retv = false;
            retv = ArmsApi.Model.MachineInfo.UpdateEICSType(this.PlantCd, this.NewType);

            return retv;
        }
    }
}