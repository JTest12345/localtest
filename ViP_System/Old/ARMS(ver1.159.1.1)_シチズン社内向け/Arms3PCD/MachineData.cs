using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
namespace Arms3PCD
{
    public class MachineData
    {
        public string Number { get; set; }
        public string Name { get; set; }
        public string Unit { get; set; }
        public bool Enabled { get; set; }

        public static MachineData[] ParseXML(string xml)
        {
            List<MachineData> retv = new List<MachineData>();

            if (string.IsNullOrEmpty(xml)) return retv.ToArray();


            XDocument doc = XDocument.Parse(xml);

            XElement[] lotlist = doc.Root.Elements("machine").ToArray();
            foreach (XElement lotnode in lotlist)
            {
                MachineData data = new MachineData();

                data.Number = lotnode.Element("macno").Value;
                data.Name = lotnode.Element("classname").Value;
                data.Unit = lotnode.Element("macname").Value;
                retv.Add(data);
            }

            return retv.ToArray();
        }

    }



}
