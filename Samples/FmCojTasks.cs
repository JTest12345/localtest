using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oskas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace FileIf
{

    class Test
    {
        static void _Main(string[] args)
        {

            var avi = new TaskFile_avi01();
            avi.ReadInFile(0, "C:\\Users\\jn-wtnb\\Desktop\\V501 1-1-5.txt");


            // コンテキストからCOJを取得する
            var assm = Assembly.GetExecutingAssembly();
            string dataStr;
            // "fileContext/csvsample.json"のStreamを取得する
            using (var stream = assm.GetManifestResourceStream("Samples.fileContext.cojsample_gaikan.json"))
            {
                var dataContext = new StreamReader(stream);
                dataStr = dataContext.ReadToEnd();
            }
            var test = JsonConvert.DeserializeObject<CojIF.AVI01_01>(dataStr);

            Console.ReadKey();
        }
    }


        class TaskFile_avi01 : IFile // avi01ファイル
    {
        public string fileId { get; set; } = "_avi01.csv";
        public string kishulot { get; set; }
        public string tanto { get; set; }
        public string jyoken { get; set; }
        public string setubi { get; set; }
        public string kaisijikan { get; set; }
        public string shuryoujikan { get; set; }
        public string kensajikan { get; set; }
        public string hpk { get; set; }
        public string budomari { get; set; }
        public string tounyusu { get; set; }
        public string ryouhinsu { get; set; }
        public string furyousu { get; set; }
        public List<string[]> furyomeisai { get; set; }


        public void ReadInFile(int taskid, string filepath)
        {
            furyomeisai = new List<string[]>();
            Tasks_Common tcommons = new Tasks_Common();
            string msg;
            try
            {
                // ヘッダー情報読取
                kishulot = CommonFuncs.GetIniValue(filepath, "Hedder", "機種LOT");
                tanto = CommonFuncs.GetIniValue(filepath, "Hedder", "担当");
                jyoken = CommonFuncs.GetIniValue(filepath, "Hedder", "条件");
                setubi = CommonFuncs.GetIniValue(filepath, "Hedder", "設備");
                kaisijikan = CommonFuncs.GetIniValue(filepath, "Hedder", "開始時間");
                shuryoujikan = CommonFuncs.GetIniValue(filepath, "Hedder", "終了時間");
                kensajikan = CommonFuncs.GetIniValue(filepath, "Hedder", "検査時間");
                hpk = CommonFuncs.GetIniValue(filepath, "Hedder", "h/k");
                budomari = CommonFuncs.GetIniValue(filepath, "Hedder", "歩留");
                tounyusu = CommonFuncs.GetIniValue(filepath, "Hedder", "投入数");
                ryouhinsu = CommonFuncs.GetIniValue(filepath, "Hedder", "良品数");
                furyousu = CommonFuncs.GetIniValue(filepath, "Hedder", "不良数");

                // 不良項目読取
                var contents = new List<string>();
                if (!CommonFuncs.ReadTextFileLine(filepath, ref contents, "UTF-16"))
                {
                    Console.WriteLine("不良項目の読取が失敗しました");
                }

                foreach (var item in contents)
                {
                    if (item.Contains("不良明細="))
                    {
                        var fmeisai = item.Replace("不良明細=", "").Split(',');
                        furyomeisai.Add(fmeisai);
                    }
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine("不良項目の読取が失敗しました");
            }
            
        }
    }


}
