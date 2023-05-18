using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace GetMDLog
{
    class Config
    {
        public static Config Settings;

        private const string SETTING_FILE_NM = "MDLogConfig.xml";
        //public const string DEBUG_SETTING_FILE_NM = @"config\\ArmsConfig.xml";
        public const string SETTING_FILE_FULLPATH = @"C:\ARMS\Config\MDLogConfig.xml";

        public string KVSDPass { get; set; }
        public string VTCFPass { get; set; }
        public List<KeyValuePair<string, List<string>>> MachineList { get; set; }

        public static void LoadSetting()
        {
            //JSON　ローカルConfigから取得する処理
            //Android端末で使用するとドライブの指定がないので使用不可?
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
            //Settings.LocalConnString = "server=VAUTOM1\\SQLExpress;Connect Timeout=30;Application Name=ARMS;UID=inline;PWD=R28uHta;database=ARMS;Max Pool Size=100";
        }
    }
}
