using System;
using System.Linq;
using System.Collections.Generic;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Windows.Forms;

namespace Arms3PCD
{
    public class Config
    {
        private const string XML_NAME = "Config.xml";
        private static string configPath = "";


        static Config()
        {
            System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
            string dir = System.IO.Path.GetDirectoryName(asm.GetName().CodeBase);

            string path = Path.Combine(dir, XML_NAME);
            configPath = path;
        }


        public static string Url { get { return loadXml("url"); } }

        public static int Timer { get { return int.Parse(loadXml("timer")); } }

        public static int AlermTimer { get { return int.Parse(loadXml("alermtimer")); } }

        public static bool IsBHT700
        {
            get
            {
                try
                {
                    return bool.Parse(loadXml("BHT700"));
                }
                catch
                {
                    return false;
                }
            }
        }

        public static ProcData[] Procs
        {
            get
            {
                List<ProcData> retv = new List<ProcData>();

                XDocument doc = XDocument.Load(configPath);
                IEnumerable<XElement> procs = doc.Root.Element("process").Elements("watch");

                foreach (XElement elm in procs)
                {
                    ProcData p = new ProcData();
                    p.Name = elm.Attribute("name").Value;
                    p.ProcNo = elm.Attribute("procno").Value;

                    if (elm.Attribute("status").Value == "1")
                    {
                        p.Enabled = true;
                    }
                    else
                    {
                        p.Enabled = false;
                    }

                    retv.Add(p);
                }

                return retv.ToArray();
            }
        }

        //コンベアリスト読み込み

        public static ConveyorData[] Conveyor
        {
            get
            {
                List<ConveyorData> retv = new List<ConveyorData>();
                try
                {
                    XDocument doc = XDocument.Load(configPath);
                    IEnumerable<XElement> conveyors = doc.Root.Element("conveyor").Elements("watch");

                    foreach (XElement elm in conveyors)
                    {
                        ConveyorData c = new ConveyorData();
                        c.cvName = elm.Attribute("name").Value;
                        c.cvNo = elm.Attribute("linecd").Value;

                        retv.Add(c);
                    }
                    return retv.ToArray();
                }
                catch (Exception ex)
                {
                    return retv.ToArray();
                }
            }
        }

        // 装置リスト読み込み

        public static MachineData[] Machines
        {
            get
            {
                List<MachineData> retv = new List<MachineData>();

                XDocument doc = XDocument.Load(configPath);
                IEnumerable<XElement> procs = doc.Root.Element("machine").Elements("watch");

                foreach (XElement elm in procs)
                {
                    MachineData m = new MachineData();
                    m.Number = elm.Attribute("number").Value;
                    m.Name = elm.Attribute("name").Value;
                    m.Unit = elm.Attribute("unit").Value;

                    if (elm.Attribute("status").Value == "1")
                    {
                        m.Enabled = true;
                    }
                    else
                    {
                        m.Enabled = false;
                    }

                    retv.Add(m);
                }

                return retv.ToArray();
            }
        }

        private static string loadXml(string name)
        {
            XDocument doc = XDocument.Load(configPath);
            return doc.Root.Element(name).Value;
        }


        /// <summary>
        /// URL設定保存
        /// </summary>
        /// <param name="newurl"></param>
        public static void UpdateUrl(string newurl)
        {
            XDocument doc = XDocument.Load(configPath);
            doc.Root.Element("url").Value = newurl;
            doc.Save(configPath);
        }


        /// <summary>
        /// 有効工程の設定保存
        /// </summary>
        /// <param name="procs"></param>
        public static void UpdateProcs(ProcData[] procs)
        {
            XDocument doc = XDocument.Load(configPath);
            IEnumerable<XElement> elms = doc.Root.Element("process").Elements("watch");

            foreach (ProcData p in procs)
            {
                IEnumerable<XElement> found = elms.Where(e => e.Attribute("procno").Value == p.ProcNo);

                foreach (XElement elm in found)
                {
                    if (p.Enabled == true)
                    {
                        elm.Attribute("status").Value = "1";
                    }
                    else
                    {
                        elm.Attribute("status").Value = "0";
                    }
                }
            }
            doc.Save(configPath);
        }
        /// <summary>
        /// 有効設備の設定保存
        /// </summary>
        /// <param name="machine"></param>
        public static void UpdateMachines(MachineData[] machines)
        {
            XDocument doc = XDocument.Load(configPath);
           

            IEnumerable<XElement> elms = doc.Root.Element("machine").Elements("watch");
            elms.Remove();

            foreach (MachineData m in machines)
            {
              //IEnumerable<XElement> found = elms.Where(e => e.Attribute("number").Value == m.Number);
              

               // foreach (XElement elm in elms)
              //  {
                    string enable;
                    if (m.Enabled == true)
                    {
                       enable = "1";
                    }
                    else
                    {
                       enable = "0";
                    }
                    XElement elem = new XElement("watch");
                    XAttribute status = new XAttribute("status", enable);
                    XAttribute number = new XAttribute("number", m.Number);
                    XAttribute name = new XAttribute("name", m.Name);
                    XAttribute unit = new XAttribute("unit", m.Unit);
                    elem.Add(status, number, name, unit);
                    doc.Root.Element("machine").Add(elem);

            }
            doc.Save(configPath);
        }
    }
}
