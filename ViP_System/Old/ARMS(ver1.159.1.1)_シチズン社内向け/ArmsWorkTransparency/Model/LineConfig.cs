using ArmsWorkTransparency.Model.PLC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ArmsWorkTransparency.Model
{
    class LineConfig
    {
        public XElement Carrier { get; set; }

        public IEnumerable<XElement> MachineList { get; set; }

        public static LineConfig LoadLineConfig(string path)
        {
            LineConfig retv = new LineConfig();
            
            XDocument doc = XDocument.Load(path);

            IEnumerable<XElement> machines = doc.Elements("settings").Elements("machine")
                .Where(mac => string.IsNullOrEmpty(mac.Element("machineNo").Value) == false);
            
            //IEnumerable<XElement> carriers = doc.Elements("settings").Elements("carrier")
            //    .Where(car => string.IsNullOrEmpty(car.Element("carrierNo").Value) == false);
            //retv.Carrier = carriers.SingleOrDefault();

            retv.MachineList = machines;

            return retv;
        }
        
        public class MachineSetting
        {
            public int MacNo { get; set; }

            public List<string> RequreInputAddressList { get; set; }

            public string InputForbiddenBitAddress { get; set; }

            public IPLC Plc { get; set; }

            public IPLC RobotPlc { get; set; }

            public string MachineLogOutputPath { get; set; }
        }

        public static MachineSetting GetMachineSetting(List<LineConfig> lineConfigList, int macNo)
        {
            // 装置設定のあるlineconfigを抽出する
            LineConfig lineConfig = lineConfigList
                .Where(l => l.MachineList.Where(m => int.Parse(m.Element("machineNo").Value) == macNo).SingleOrDefault() != null)
                .SingleOrDefault();

            //if (lineConfig == null)
            //    throw new ApplicationException($"設備設定ファイル(lineconfig.xml)に設定されていない設備が選択されています。製造合理化担当まで連絡して下さい。macno:{macNo}");

            // lineConfigを見ないようにする
            if (lineConfig == null)
            {
                return new MachineSetting();
            }

            // 指定設備の設定を抽出する
            XElement elm = lineConfig.MachineList.Where(m => int.Parse(m.Element("machineNo").Value) == macNo).SingleOrDefault();

            MachineSetting retv = new MachineSetting();
            retv.RequreInputAddressList = new List<string>();

            string hostIpAddress = string.Empty;
            XElement elmHostIpAddress = elm.Element("hostIpAddress");
            if (elmHostIpAddress != null)
            {
                hostIpAddress = elmHostIpAddress.Value;
            }

            XElement plcMakerElm = elm.Element("plcMaker");
            XElement plcAddressElm = elm.Element("plcAddress");
            XElement plcPortElm = elm.Element("plcPort");

            string plcSocket = string.Empty;
            XElement plcSocketElm = elm.Element("plcSocket");
            if (plcSocketElm != null)
            {
                plcSocket = plcSocketElm.Value;
            }
            else
            {
                plcSocket = Socket.Udp.ToString();
            }

            if (plcMakerElm != null && plcAddressElm != null && plcPortElm != null)
            {
                if (!string.IsNullOrEmpty(plcMakerElm.Value) && !string.IsNullOrEmpty(plcAddressElm.Value) && !string.IsNullOrEmpty(plcPortElm.Value))
                {
                    retv.Plc = PLC.Common.GetInstance(plcMakerElm.Value, plcAddressElm.Value, int.Parse(plcPortElm.Value), hostIpAddress, plcSocket);
                }
            }

            XElement elmList = elm.Element("loaderConveyorReqBitAddressList");
            if (elmList != null)
            {
                var elms = elmList.Elements("loaderConveyorReqBitAddress");
                foreach (XElement e in elms)
                {
                    retv.RequreInputAddressList.Add(e.Value);
                }
            }

            XElement inputForbiddenBitAddressElm = elm.Element("inputForbiddenBitAddress");
            if (inputForbiddenBitAddressElm != null)
            {
                retv.InputForbiddenBitAddress = inputForbiddenBitAddressElm.Value;
            }

            //if (lineConfig.Carrier != null)
            //{
            //    retv.RobotPlc = new RelayMachinePLC(lineConfig.Carrier.Element("plcAddress").Value, int.Parse(lineConfig.Carrier.Element("plcPort").Value));
            //}

            XElement elmMachineLogOutputPath = elm.Element("logOutputDirectoryPath");
            if (elmMachineLogOutputPath != null)
            {
                retv.MachineLogOutputPath = elmMachineLogOutputPath.Value;
            }

            return retv;
        }
    }
}
