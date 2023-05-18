using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;


namespace Arms3PCD
{
    public class LotData
    {
        public string procno { get; set; }
        public string LotNo { get; set; }
        public string TypeCd { get; set; }
        public string MacName { get; set; }
        public string ResinGpCd { get; set; }
        public string MacNumber { get; set; }
        /// <summary>
        /// 開始フラグ
        /// 装置供給信号On・供給CV到達信号On
        /// </summary>
        public bool Startflg { get; set; }
        public WorkState Place { get; set; }

        public enum WorkState
        {
            Start, End
        }

        private const int START = 0;
        private const int END = 1;

        public static LotData[] ParseXML(string xml, WorkState workstate)
        {
            List<LotData> retv = new List<LotData>();

          
            if (string.IsNullOrEmpty(xml)) return retv.ToArray();


            XDocument doc = XDocument.Parse(xml);
            
            XElement[] lotlist = doc.Root.Elements("lot").ToArray();
           
            foreach (XElement lotnode in lotlist)
            {
                LotData data = new LotData();

                data.procno = lotnode.Element("proc").Value;
                data.LotNo = lotnode.Element("lotno").Value;
                data.MacName = lotnode.Element("macname").Value;
                data.TypeCd = lotnode.Element("typecd").Value;
                data.ResinGpCd = lotnode.Element("resin").Value;
                data.MacNumber = lotnode.Element("macno").Value;
                data.Place = workstate;
                retv.Add(data);
            }
           

            return retv.ToArray();
        }
    }
}
