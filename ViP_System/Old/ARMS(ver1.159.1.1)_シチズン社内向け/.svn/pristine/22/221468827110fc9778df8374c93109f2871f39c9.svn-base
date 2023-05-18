using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.Windows.Forms;

namespace Arms3PCD
{
    public class ConveyorData
    {
        public string cvNo { get; set; }
        public string cvName { get; set; }
        public string LotNo { get; set; }
        public string TypeCD{ get; set; }
        public string ProcCD { get; set; }
        public string NextProcCD { get; set; }
        public string Reason { get; set; }
        public string ResReason { get; set; }
        public int Mode { get; set; }

        public static ConveyorData[] ParseXML(string xml,int mode)
        {
            List<ConveyorData> retv = new List<ConveyorData>();

            if (string.IsNullOrEmpty(xml)) return retv.ToArray();


            XDocument doc = XDocument.Parse(xml);

            XElement[] lotlist = doc.Root.Elements("lot").ToArray();
            foreach (XElement lotnode in lotlist)
            {
                ConveyorData data = new ConveyorData();
                data.LotNo = lotnode.Element("lotno").Value;
                data.TypeCD = lotnode.Element("type").Value;
                data.NextProcCD = lotnode.Element("proc").Value;
                data.Reason = lotnode.Element("reason").Value;
                data.ResReason = lotnode.Element("res").Value;
                data.Mode = mode; 
                retv.Add(data);
            }

            return retv.ToArray();
        }
    }
}
