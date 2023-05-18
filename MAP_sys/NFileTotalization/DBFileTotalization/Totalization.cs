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
            string FilePathCsv = @"\\cejfs0\ToCEJFS1(X線データ）\DB_data\N_File";
            //string FilePathCsv = @"D:\test\DB2021";
            List<string> pcs_data = new List<string>();
            //string resultpass = @"D:\test\allpara_db.txt";
            string resultpass = @"\\SVFILE2\fileserver\301OEM\01.MAP\05 DB\201 OEM製造技術課 ＤＢ工程\システム\Nファイル\auto_bond_backup_pkg.csv";
            string resultpass2 = @"\\SVFILE2\fileserver\301OEM\01.MAP\05 DB\201 OEM製造技術課 ＤＢ工程\システム\Nファイル\Edit_Bonding_Map.csv";
            string today = DateTime.Now.ToString("yyyyMMdd");

            string[] lines;

            if (File.Exists(resultpass))
            {
                lines = File.ReadAllLines(resultpass);

                //excelで読み込めるように、1048576データ到達前にファイルを変える
                if (lines.Length >= 1000000)
                {
                    resultpass = @"\\SVFILE2\fileserver\301OEM\01.MAP\05 DB\201 OEM製造技術課 ＤＢ工程\システム\Nファイル\auto_bond_backup_pkg_" + today.ToString() + ".csv";
                }

            }

            if (File.Exists(resultpass2))
            {
                lines = File.ReadAllLines(resultpass2);

                if (lines.Length >= 1000000)
                {
                    resultpass2 = @"\\SVFILE2\fileserver\301OEM\01.MAP\05 DB\201 OEM製造技術課 ＤＢ工程\システム\Nファイル\Edit_Bonding_Map_" + today.ToString() + ".csv";
                }
            }
                

            string[] arr;

            string filename;
            string filetype;
            DateTime filetime;
            string machineno;
            string lotno;
            string typename;
            string DBtype;

            int data_count = 0;
            
            string searchword = "auto bond backup pkg file";
            string searchword2 = "[Toggle] Edit Bonding Map";
            //マガジンの高さの設定を変更しているログ 20220912           

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
                        //auto bond backup pkg file
                        if (arr[2].Contains(searchword))
                        {
                            File.AppendAllText(resultpass, lotno + "," + typename + "," + machineno + "," + filetype + "," + arr[0] + "," + arr[1] + "," + arr[2] + "\r\n");
                        }
                        //[Toggle] Edit Bonding Map
                        else if (arr[2].Contains(searchword2))
                        {
                            File.AppendAllText(resultpass2, lotno + "," + typename + "," + machineno + "," + filetype + "," + arr[0] + "," + arr[1] + "," + arr[2] + "\r\n");
                        }
                    }
                }
            }

            
            string folderTo = @"\\cejfs0\ToCEJFS1(X線データ）\DB_data\N_File";
            //cejfs0のNファイル削除
            DirectoryInfo directory = new DirectoryInfo(folderTo);

            foreach (FileInfo file in directory.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in directory.EnumerateDirectories())
            {
                dir.Delete(true);
            }

        }      
    }
}
