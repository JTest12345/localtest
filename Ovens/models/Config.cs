using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Timers;

namespace Ovens
{
    //
    // Config File(ovnSrvConfig.json) Model
    //

    public class Header
    {
        public string obj_type { get; set; }
        public string division { get; set; }
        public string site { get; set; }
        public string discription { get; set; }
        public string macno { get; set; }
    }

    public class HeaderOvn
    {
        public string discription { get; set; }
        public string macno { get; set; }
    }

    public class PingSettings
    {
        public int time_out_ms { get; set; }
        public int retry { get; set; }
    }

    public class StopTask
    {
        public int ping { get; set; }
        public int plcdate { get; set; }
    }

    public class Plc
    {
        public string ip_address { get; set; }
        public int port { get; set; }
        public string type { get; set; }
        public string datetime_address { get; set; }
        public PingSettings ping_settings { get; set; }
        public StopTask stoptask { get; set; }
    }

    public class Oven
    {
        public string oven_id { get; set; }
        public string sp_address { get; set; }
        public string pv_address { get; set; }
        public string ng1_address { get; set; }
        public string ng2_address { get; set; }
        public string macno_address { get; set; }
        public string error_plus_addres { get; set; }
        public string error_minus_address { get; set; }
        public string oven_busy_address { get; set; }
    }

    public class OvenClient
    {
        public HeaderOvn header_ovn { get; set; }
        public Plc plc { get; set; }
        public List<Oven> oven { get; set; }
    }

    public class CeObject
    {
        public Header header { get; set; }
        public List<OvenClient> oven_client { get; set; }
    }

    public class OvenConfig
    {
        public CeObject ceObject { get; set; }
    }

    public class OvenConfigFuncs
    {
        static public OvenConfig ovnSrvConfig()
        {
            //ConfigJson読込
            string FilePath = @"C:\Oskas\ovnsrv\ovnSrvConfig.json";
            return JsonConvert.DeserializeObject<OvenConfig>(JsonFileReader(FilePath));
        }

        static public string JsonFileReader(string FilePath)
        {
            StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding("utf-8"));
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }
    }


}
