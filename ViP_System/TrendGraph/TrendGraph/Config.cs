using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace TrendGraph
{
    public class Config
    {
        public static Config Settings;

        private const string SETTING_FILE_NM = "ArmsConfig.xml";
        //public const string DEBUG_SETTING_FILE_NM = @"config\\ArmsConfig.xml";
        public const string SETTING_FILE_FULLPATH = @"C:\ARMS\Config\ArmsConfig.xml";

        public string LocalConnString { get; set; }

        public static void LoadSetting()
        {
            try
            {
                if (Settings == null)
                {
                    string raw;

                    string settingfolderpath = @"C:\ARMS\Config\";

                    raw = File.ReadAllText(Path.Combine(settingfolderpath, SETTING_FILE_NM), Encoding.UTF8);
                  
                    Settings = JsonConvert.DeserializeObject<Config>(raw);
                }
            }
            catch (Exception err)
            {
                Settings = new Config();
            }
        }
    }
}