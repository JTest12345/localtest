using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MathNet.Numerics.Statistics;

namespace DBFileTotalization
{
    class Totalization
    {
        private static readonly Encoding Enc = Encoding.GetEncoding("shift_jis");


        public void ReadData()
        {
            //string FilePathCsv = @"\\cejfs0\ToCEJFS1(X線データ）\WB_data\matome";
            string FilePathCsv = @"D:\test\DB2021";
            List<string> pcs_data = new List<string>();
            string resultpass = @"D:\test\allpara_db.txt";

            string[] arr;
            List<string> arr_list = new List<string>();
            List<double> x_data = new List<double>();
            List<double> y_data = new List<double>();
            List<double> AreaTheta_data = new List<double>();

            string filename;
            string filetype;
            DateTime filetime;
            string machineno;
            string lotno;
            string typename;
            string DBtype;

            List<double> shigma_x = new List<double>(); ;
            List<double> shigma_y = new List<double>(); ;
            List<double> shigma_AreaTheta = new List<double>(); ;

            int data_count = 0;

            //マガジンの高さの設定を変更しているログ 20220912
            string searchword = "auto bond backup pkg file";

            //昨日の日付
            string dt = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //初期の書き方　デフォルトで昇順っぽい
            //string[] names = Directory.GetFiles(FilePathCsv, "*");

            //ファイル名昇順
            List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => f).ToList();

            //更新日時昇順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => File.GetLastWriteTime(f)).ToList();

            //降順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderByDescending(f => f).ToList();

            foreach (string name in names)
            {
                data_count = data_count + 1;
                //パス名からファイル名のみ取得
                filename = Path.GetFileName(name);
                pcs_data.Clear();

                filetime = File.GetLastWriteTime(name);

                machineno = filename.Substring(1, 4);
                if (machineno.Substring(0, 1) == "0")
                {
                    machineno = machineno.Substring(1, 3);
                }

                lotno = filename.Substring(filename.IndexOf("_") + 1);
                lotno = lotno.Substring(0, lotno.IndexOf("_"));

                typename = filename.Substring(filename.IndexOf("_") + 1);
                typename = typename.Substring(typename.IndexOf("_") + 1);
                typename = typename.Substring(0, typename.IndexOf("_"));

                filetype = filename.Substring(filename.IndexOf("_") + 1);
                filetype = filetype.Substring(filetype.IndexOf("_") + 1);
                filetype = filetype.Substring(filetype.IndexOf("_") + 1);
                filetype = filetype.Substring(0, filetype.IndexOf("_"));

                //1行ずつ読込
                using (var sr = new StreamReader(name, Enc))
                {
                    while (sr.Peek() > -1)
                    {
                        pcs_data.Add(sr.ReadLine());
                    }
                }

                foreach (string data in pcs_data)
                {
                    arr = data.Split(',');

                    //auto bond backup pkg file	ENABLE	->	DISABLE
                    //auto bond backup pkg file	DISABLE	->	ENABLE
                    if (arr.Length >= 3)
                    {
                        if (arr[2].Contains(searchword))
                        {
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + machineno + "," + filetype + "," + arr[0] + "," + arr[1] + "," + arr[2] + "\r\n");
                        }
                    }
                }
            }
        }

        public void ReadDataN()
        {
            //string FilePathCsv = @"\\cejfs0\ToCEJFS1(X線データ）\WB_data\matome";
            string FilePathCsv = @"D:\test\DB2021";
            List<string> pcs_data = new List<string>();
            string resultpass = @"D:\test\allpara_db.txt";
            
            string[] arr;
            List<string> arr_list = new List<string>();
            List<double> x_data = new List<double>();
            List<double> y_data = new List<double>();
            List<double> AreaTheta_data = new List<double>();

            string filename;
            string filetype;
            DateTime filetime;
            string machineno;
            string lotno;
            string typename;
            string DBtype;

            List<double> shigma_x = new List<double>(); ;
            List<double> shigma_y = new List<double>(); ;
            List<double> shigma_AreaTheta = new List<double>(); ;

            int data_count = 0;

            //[Toggle] Edit Bonding Map    EDIT
            //string searchword = "[Toggle] Edit Bonding Map";
            string searchword = "auto bond backup pkg file";

            //昨日の日付
            string dt = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //初期の書き方　デフォルトで昇順っぽい
            //string[] names = Directory.GetFiles(FilePathCsv, "*");

            //ファイル名昇順
            List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => f).ToList();

            //更新日時昇順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => File.GetLastWriteTime(f)).ToList();
            
            //降順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderByDescending(f => f).ToList();

            foreach (string name in names)
            {
                data_count = data_count + 1;
                //パス名からファイル名のみ取得
                filename = Path.GetFileName(name);
                pcs_data.Clear();

                filetime = File.GetLastWriteTime(name);

                machineno = filename.Substring(1, 4);
                if(machineno.Substring(0,1) == "0") 
                {
                    machineno = machineno.Substring(1, 3);
                }

                lotno = filename.Substring(filename.IndexOf("_") + 1);
                lotno = lotno.Substring(0, lotno.IndexOf("_"));

                typename = filename.Substring(filename.IndexOf("_") + 1);
                typename = typename.Substring(typename.IndexOf("_") + 1);
                typename = typename.Substring(0, typename.IndexOf("_"));

                filetype = filename.Substring(filename.IndexOf("_") + 1);
                filetype = filetype.Substring(filetype.IndexOf("_") + 1);
                filetype = filetype.Substring(filetype.IndexOf("_") + 1);
                filetype = filetype.Substring(0, filetype.IndexOf("_"));

                //1行ずつ読込
                using (var sr = new StreamReader(name, Enc))
                {
                    while (sr.Peek() > -1)
                    {
                        pcs_data.Add(sr.ReadLine());
                    }
                }

                foreach (string data in pcs_data)
                {
                    arr = data.Split(',');

                    //auto bond backup pkg file	ENABLE	->	DISABLE
                    //auto bond backup pkg file	DISABLE	->	ENABLE
                    if (arr.Length >= 3)
                    {
                        if (arr[2].Contains(searchword))
                        {                         
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + machineno+ "," + filetype + "," + arr[0] + "," + arr[1] + "," + arr[2] + "\r\n");
                        }                      
                    }
                }
            }
        }
        public void ReadDataML() 
        {
            //string FilePathCsv = @"\\cejfs0\ToCEJFS1(X線データ）\WB_data\matome";
            string FilePathCsv = @"D:\test\2022";
            List<string> pcs_data = new List<string>();
            string resultpassonoff = @"D:\test\resultonoff.txt";
            

            string pre_x;
            string[] arr;
            List<string> arr_list = new List<string>();
            List<double> x_data = new List<double>();
            List<double> y_data = new List<double>();
            List<double> AreaTheta_data = new List<double>();

            string filename;
            string filetype;
            string machineno;
            string lotno;
            string typename;
            string DBtype;

            List<double> shigma_x = new List<double>(); ;
            List<double> shigma_y = new List<double>(); ;
            List<double> shigma_AreaTheta = new List<double>(); ;

            int data_count=0;


            //昨日の日付
            string dt = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //初期の書き方　デフォルトで昇順っぽい
            //string[] names = Directory.GetFiles(FilePathCsv, "*");

            //昇順
            List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => f).ToList();
            //降順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderByDescending(f => f).ToList();

            foreach (string name in names)
            {
                data_count = data_count + 1;
                //パス名からファイル名のみ取得
                filename = Path.GetFileName(name);                
                pcs_data.Clear();

                lotno = filename.Substring(filename.IndexOf("_")+1);
                lotno = lotno.Substring(0,lotno.IndexOf("_"));

                typename = filename.Substring(filename.IndexOf("_") + 1);
                typename = typename.Substring(typename.IndexOf("_") + 1);
                typename = typename.Substring(0, typename.IndexOf("_"));

                //1行ずつ読込
                using (var sr = new StreamReader(name, Enc))
                {
                    while (sr.Peek() > -1)
                    {
                        pcs_data.Add(sr.ReadLine());
                    }
                }
               
                foreach (string data in pcs_data)
                {
                    arr = data.Split(',');
                    //[      ON]    → [     OFF]
                    if (arr.Length >= 5)
                    {

                    

                        if (arr[3].Contains("GAP大エラー検出") && arr[4].Contains("OFF"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3]);
                            File.AppendAllText(resultpassonoff, lotno + "," + typename + "," + arr[3] + "," + arr[4] + "\r\n");
                        }
                        else if(arr[3].Contains("GAP小エラー検出") && arr[4].Contains("OFF"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3]);
                            File.AppendAllText(resultpassonoff, lotno + "," + typename + "," + arr[3] + "," + arr[4] + "\r\n");
                        }
                        //105%確認
                        else if(arr[3].Contains("ARCエラー検出") && arr[4].Contains("OFF"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4]);
                            File.AppendAllText(resultpassonoff, lotno + "," + typename + "," + arr[3] + "," + arr[4] + "\r\n");
                        }
                        else if (arr[3].Contains("U/Lエラー検出") && arr[4].Contains("OFF"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4]);
                            File.AppendAllText(resultpassonoff, lotno + "," + typename + ","  + arr[3] + "," + arr[4] + "\r\n");
                        }
                        else if (arr[3].Contains("WCOエラー検出") && arr[4].Contains("OFF"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4]);
                            File.AppendAllText(resultpassonoff, lotno + "," + typename + "," + arr[3] + "," + arr[4] + "\r\n");
                        }

                    }

                }
                

            }          
        }

        public void ReadDataMP()
        {
            //string FilePathCsv = @"\\cejfs0\ToCEJFS1(X線データ）\WB_data\matome";
            string FilePathCsv = @"D:\test\Filesaki";
            List<string> pcs_data = new List<string>();
            string resultpass = @"D:\test\result.txt";

            string pre_x;
            string[] arr;
            List<string> arr_list = new List<string>();
            List<double> x_data = new List<double>();
            List<double> y_data = new List<double>();
            List<double> AreaTheta_data = new List<double>();

            string filename;
            string filetype;
            string machineno;
            string lotno;
            string typename;
            string DBtype;

            List<double> shigma_x = new List<double>(); ;
            List<double> shigma_y = new List<double>(); ;
            List<double> shigma_AreaTheta = new List<double>(); ;

            int data_count = 0;


            //昨日の日付
            string dt = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //初期の書き方　デフォルトで昇順っぽい
            //string[] names = Directory.GetFiles(FilePathCsv, "*");

            //昇順
            List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => f).ToList();
            //降順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderByDescending(f => f).ToList();

            foreach (string name in names)
            {
                data_count = data_count + 1;
                //パス名からファイル名のみ取得
                filename = Path.GetFileName(name);
                pcs_data.Clear();

                lotno = filename.Substring(filename.IndexOf("_") + 1);
                lotno = lotno.Substring(0, lotno.IndexOf("_"));

                typename = filename.Substring(filename.IndexOf("_") + 1);
                typename = typename.Substring(typename.IndexOf("_") + 1);
                typename = typename.Substring(0, typename.IndexOf("_"));

                //1行ずつ読込
                using (var sr = new StreamReader(name, Enc))
                {
                    while (sr.Peek() > -1)
                    {
                        pcs_data.Add(sr.ReadLine());
                    }
                }

                foreach (string data in pcs_data)
                {
                    arr = data.Split(',');

                    if (arr[0].Contains("S-ERR"))
                    {
                        if (arr[1].Contains("GAP大エラー検出"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3]);
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + arr[1] + "," + arr[3] + "\r\n");
                        }
                        else if (arr[1].Contains("GAP小エラー検出"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3]);
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + arr[1] + "," + arr[3] + "\r\n");
                        }
                        //105%確認
                        else if (arr[1].Contains("ARC  エラー検出"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4]);
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4] + "\r\n");
                        }
                        else if (arr[1].Contains("U/L  エラー検出"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4]);
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4] + "\r\n");
                        }
                        else if (arr[1].Contains("WCO  エラー検出"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4]);
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + arr[1] + "," + arr[3] + "," + arr[4] + "\r\n");
                        }
                        else if (arr[1].Contains("EPD絶縁破壊エラー検出"))
                        {
                            //Console.WriteLine(lotno + "," + typename + "," + arr[1] + "," + arr[4]);
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + arr[1] + "," + arr[4] + "\r\n");
                            break;
                        }
                    }

                }


            }
        }
    }
}
