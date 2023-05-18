using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.IO;
using MathNet.Numerics.Statistics;
using System.Configuration;
using System.Data.SqlClient;

namespace DBFileTotalization
{
    class Totalization
    {
        private static readonly Encoding Enc = Encoding.GetEncoding("shift_jis");
        public void ReadData() 
        {
            string constr = ConfigurationManager.AppSettings["PMMSDBconnect"];

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
            int filenum = 0;

            List<double> shigma_x = new List<double>(); ;
            List<double> shigma_y = new List<double>(); ;
            List<double> shigma_AreaTheta = new List<double>(); ;

            string beforemachineno = "";
            string beforefiletype = "";

            double ave_x;
            double ave_y;
            double ave_areatheta;

            string Armsmacno;


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

                //最後のファイルになった場合、先に足さないと、次の処理で条件に入らない
                filenum = filenum + 1;

                //最後の1ファイルのみ別処理
                if(filenum == names.Count)
                {
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
                        if (arr.Length > 13)
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

                    //1日のσの平均値を算出
                    ave_x = shigma_x.Average();
                    ave_y = shigma_y.Average();
                    ave_areatheta = shigma_AreaTheta.Average();

                    //DBへデータをInsertする
                    //macno, macname, class, shigma, dbdate

                    shigma_x.Clear();
                    shigma_y.Clear();
                    shigma_AreaTheta.Clear();

                    break;
                }//最後の1ファイルのみ別処理終わり


                //ロット名
                lotno = filename.Substring(filename.IndexOf("_") + 1);
                lotno = lotno.Substring(0, lotno.IndexOf("_"));
                //品種名
                typename = filename;
                for (int i = 1; i <= 2; i++)
                {
                    typename = typename.Substring(typename.IndexOf("_") + 1);
                }
                typename = typename.Substring(0, typename.IndexOf("_"));

                //ZD or LED
                DBtype = filename;
                for (int i = 1; i <= 3; i++)
                {
                    DBtype = DBtype.Substring(DBtype.IndexOf("_") + 1);
                }
                DBtype = DBtype.Substring(0, DBtype.IndexOf("_"));


                //号機名が変わっている　もしくは　ファイルタイプが変わっていたら、ここで集計
                if (beforemachineno != "" && beforefiletype != "" && (beforemachineno != machineno || beforefiletype != filetype))
                {
                    //1日のσの平均値を算出
                    ave_x = shigma_x.Average();
                    ave_y = shigma_y.Average();
                    ave_areatheta = shigma_AreaTheta.Average();

                    //DBへデータをInsertする
                    //macno, macname, class, shigma, dbdate

                    //classはfiletypeから判断

                    //Armsのデータベースからmacnoを取得
                    Armsmacno = Macname_To_Macno(machineno, DBtype);

                    shigma_x.Clear();
                    shigma_y.Clear();
                    shigma_AreaTheta.Clear();
                }

                
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
                beforefiletype = filetype;


            }          
        }

        public string Macname_To_Macno(string plantnm, string DBtype)
        {
            string constr = ConfigurationManager.AppSettings["ARMSDBconnect"];

            string macno = "";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //ZD
                if(DBtype == "1") 
                {
                    cmd.CommandText = "SELECT macno FROM TmMachine where macno like @macno and plantnm like @plantnm and plantcd like @plantcd";
                    cmd.Parameters.Add(new SqlParameter("@macno", "%01%"));
                    cmd.Parameters.Add(new SqlParameter("@plantnm", "C" + plantnm));
                    cmd.Parameters.Add(new SqlParameter("@plantcd", "%DB%"));
                }
                //LED
                else
                {
                    cmd.CommandText = "SELECT macno FROM TmMachine where macno like @macno and plantnm like @plantnm and plantcd like @plantcd";
                    cmd.Parameters.Add(new SqlParameter("@macno", "%04%"));
                    cmd.Parameters.Add(new SqlParameter("@plantnm", "C" + plantnm));
                    cmd.Parameters.Add(new SqlParameter("@plantcd", "%DB%"));
                }
                               
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        macno = SQLite.ParseString(reader["macno"]);
                        break;
                    }
                }
                cmd.Parameters.Clear();
            }

            return macno;
        }
    }
}
