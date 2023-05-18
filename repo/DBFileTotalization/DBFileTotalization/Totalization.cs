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
            //string FilePathCsv = @"D:\test\file";
            string FilePathCsv = @"E:\I_File\20220904";
            List<string> pcs_data = new List<string>();

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

            string beforemachineno = "";

            double ave_x;
            double ave_y;
            double ave_areatheta;


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
                //パス名からファイル名のみ取得
                filename = Path.GetFileName(name);
                
                //H(pre) or I(post)ファイル
                filetype = filename.Substring(0, 1);

                //号機名
                machineno = filename.Substring(1, 4);
                if(machineno.Substring(0,1) == "0")
                {
                    machineno = machineno.Substring(1, 3);
                }

                //号機名が変わっていたら、ここで集計
                if(beforemachineno != "" && beforemachineno != machineno)
                {
                    //1日のσの平均値を算出
                    ave_x = shigma_x.Average();
                    ave_y = shigma_y.Average();
                    ave_areatheta = shigma_AreaTheta.Average();

                    //DBへデータをInsertする
                    //macno, macname, class, shigma, dbdate

                    shigma_x.Clear();
                    shigma_y.Clear();
                    shigma_AreaTheta.Clear();
                }


                lotno = filename.Substring(filename.IndexOf("_") + 1);
                lotno = lotno.Substring(0, lotno.IndexOf("_"));

                typename = filename;
                for (int i = 1; i <= 2; i++)
                {
                    typename = typename.Substring(typename.IndexOf("_") + 1);
                }
                typename = typename.Substring(0, typename.IndexOf("_"));

                DBtype = filename;
                for (int i = 1; i <= 3; i++)
                {
                    DBtype = DBtype.Substring(DBtype.IndexOf("_") + 1);
                }
                DBtype = DBtype.Substring(0, DBtype.IndexOf("_"));

                //typename = filename.Substring(filename.LastIndexOf("_"));

                pcs_data.Clear();              
                x_data.Clear();
                y_data.Clear();
                AreaTheta_data.Clear();

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
                    //skipのデータを飛ばすため13以上としている H(pre):16,I(post):15
                    if(arr.Length > 13)
                    {
                        //x:6,y:7,Area(H) or theta(I):10
                        x_data.Add(Convert.ToDouble(arr[6]));
                        y_data.Add(Convert.ToDouble(arr[7]));
                        AreaTheta_data.Add(Convert.ToDouble(arr[10]));
                    }
                }
                shigma_x.Add(x_data.PopulationStandardDeviation());
                shigma_y.Add(y_data.PopulationStandardDeviation());
                shigma_AreaTheta.Add(AreaTheta_data.PopulationStandardDeviation());

                //設備の変わり目を判断するために、設備番号を記録
                beforemachineno = machineno;
            }          
        }

        public List<string> Macname_To_Macno(List<string> macname)
        {
            List<string> macno = new List<string>();


            return macno;
        }
    }
}
