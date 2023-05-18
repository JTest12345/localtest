using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ArmsApi.Model;

namespace ArmsWeb.Models
{
    public class EicsTypeChange
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
        /// 検索条件
        /// </summary>
        public string SearchCond { get; set; }

        /// <summary>
        /// 検索結果
        /// </summary>
        public List<string> SearchResult { get; set; } 
        

    }
}