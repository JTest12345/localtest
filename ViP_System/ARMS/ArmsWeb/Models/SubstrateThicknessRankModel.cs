using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace ArmsWeb.Models
{
    public class SubstrateThicknessRankModel
    {
        public SubstrateThicknessRankModel(string ringdata)
        {
            DataMatirxList = new List<string>();

            this.RingData = ringdata;
        }

        public string RingData { get; set; }

        /// <summary>
        /// 基板DM
        /// </summary>
        public List<string> DataMatirxList { get; set; }

        /// <summary>
        /// 厚みランクが混ざっていないか照合
        /// </summary>
        /// <returns></returns>
        public bool CheckSubstrateThicknessRank(out string msg)
        {
            List<string> thicknessRankList = new List<string>();

            StringBuilder sb = new StringBuilder();
            bool first = true;

            foreach (string datamatrix in this.DataMatirxList)
            {
                string thicknessRank = getSubstrateThicknessRank(datamatrix);
                if (!thicknessRankList.Exists(r => r == thicknessRank))
                {
                    thicknessRankList.Add(thicknessRank);
                }

                if (!first) sb.Append(",");
                sb.Append(datamatrix + ":" + thicknessRank);
                first = false;
            }

            msg = sb.ToString();
            if (thicknessRankList.Count >= 2)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string getSubstrateThicknessRank(string datamatrix)
        {
            return ArmsApi.Model.SubstrateThicknessRank.GetThicknessRank(datamatrix);
        }

        /// <summary>
        /// リングデータと基板DMの紐付け登録
        /// </summary>
        public void Update()
        {
            ArmsApi.Model.SubstrateThicknessRank.Update(this.RingData, this.DataMatirxList);
        }
    }
}