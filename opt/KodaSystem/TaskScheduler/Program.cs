using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using KodaClassLibrary;
using static KodaClassLibrary.UtilFuncs;
using MimeKit;

namespace TaskScheduler {
    class Program {

        public static string AppDir { get; } = AppDomain.CurrentDomain.SetupInformation.ApplicationBase;

        public static string SettingDir { get; } = $@"{AppDir}\settings";

        public static string MailDir { get; } = $@"{SettingDir}\mail";

        public static Dictionary<string, string> DBConStrDic { get; } = Get_dbConStr();

        public static Dictionary<string, string> FtpFldPathDic { get; } = Get_ftpFldPath();

        static void Main(string[] args) {


            if (args.Contains("PIcheck")) {
                PIcheck.CheckPI();
            }
            else if (args.Contains("RTR500BC")) {
                OndotoriFtpData.Get_RTR500BC_Data();
            }
            else if (args.Contains("Particle")) {
                ParticleFtpData.Get_Particle_Data();
            }
            else if (args.Contains("debug")) {


               //string rr_con = @"Data Source=vautom4\SQLEXpress; Initial Catalog=KODA;Connect Timeout=5; User ID=inline; Password=R28uHta";
                //FormsSQL rr_sql = new FormsSQL(rr_con);

                //var list = rr_sql.Get_RRData<FormsSQL.RepairRecordData>();

                //string rr3_con = @"Data Source=vautom3\SQLEXpress; Initial Catalog=KODA;Connect Timeout=5; User ID=inline; Password=R28uHta";
                //FormsSQL rr3_sql = new FormsSQL(rr3_con);

                //foreach(var data in list) {

                //    rr3_sql.Insert_RR(data);
                //    rr3_sql.Update_RR(data);
                //}


            }




        }

        /// <summary>
        /// データベースへの接続文字列を読み込む
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> Get_dbConStr() {

            string json = ReadText($@"{SettingDir}\db-constr.json");
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return dic;
        }

        /// <summary>
        /// FTPに使用しているフォルダを読み込む
        /// </summary>
        /// <returns></returns>
        public static Dictionary<string, string> Get_ftpFldPath() {

            string json = ReadText($@"{SettingDir}\ftp-fldpath.json");
            Dictionary<string, string> dic = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);

            return dic;
        }
    }
}
