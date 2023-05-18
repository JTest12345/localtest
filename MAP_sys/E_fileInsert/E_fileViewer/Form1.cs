using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
using System.Runtime.InteropServices;
using System.Data.SqlClient;

namespace E_fileViewer
{
    public partial class Form1 : Form
    {
        private static readonly Encoding Enc = Encoding.GetEncoding("shift_jis");

        //タイムアウト秒
        public static int TimeoutSeconds { get; set; } = 1;

        public Form1()
        {
            InitializeComponent();
            //日付設定　昨日分を更新
            string ymd = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            EFilePass.Text = @"\\CE05006\E_File\" + ymd;
            //EFilePass.Text = @"D:\test\file";
            datestart.Value = new DateTime(DateTime.Now.AddDays(-600).Year, DateTime.Now.Month, DateTime.Now.Day);
        }

        private void GetEData_button_Click(object sender, EventArgs e)
        {
            List<string> ALL_EFile = new List<string>();
            //Program obj = new Program();

            int startd = int.Parse(datestart.Value.ToString("yyyyMMdd"));
            int endd = int.Parse(dateend.Value.ToString("yyyyMMdd"));
            DateTime testd = datestart.Value;

            //パスがなかったら、1秒で次のアドレスを見に行くような仕様
            if (DirectoryExists(EFilePass.Text))
            {
                ALL_EFile = GetALL_EFile(EFilePass.Text);
            }
            ReadData(ALL_EFile);
        }

        public static bool DirectoryExists(string path)
        {
            return TimeoutCore(() => Directory.Exists(path));
        }

        private static bool TimeoutCore(Func<bool> existFunction)
        {
            try
            {
                var source = new CancellationTokenSource();
                source.CancelAfter(TimeoutSeconds * 1000);
                var task = Task.Factory.StartNew(() => existFunction(), source.Token);
                task.Wait(source.Token);
                return task.Result;
            }
            catch (OperationCanceledException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        /// <summary>
        /// Eファイル全てのフルパスを取得
        /// </summary>
        /// <param name="pass"></param>
        /// <returns></returns>
        public List<string> GetALL_EFile(string pass)
        {
            string folderFrom = pass; //コピー元のフォルダー
            string yyyymmdd;
            yyyymmdd = DateTime.Now.AddDays(0).ToString("yyyyMMdd");
            //string folderTo = @"\\svfile7\境川\工程データ\DB\E_file\" + yyyymmdd;
            string filePattern = "E*";

            DateTime filetime;
            int namelength;

            var foldersFrom = new System.Collections.Generic.List<string>();
            var foldersTo = new System.Collections.Generic.List<string>();

            string foldername;

            foreach (string fileName in System.IO.Directory.GetFiles(folderFrom, filePattern, System.IO.SearchOption.AllDirectories))
            {
                string folder = System.IO.Path.GetDirectoryName(fileName);
                foldername = Path.GetFileName(folder);
                if (foldersFrom.Contains(folder + "\\" + filePattern))
                {
                    continue;
                }

                filetime = File.GetLastWriteTime(fileName);
                namelength = Path.GetFileName(fileName).Length;

                //名称の長さが30文字以上　※正常完了しなかったロットは12文字くらい、正常ロットは50文字くらい
                if (namelength >= 30 && datestart.Value <= filetime && dateend.Value >= filetime)
                {
                    //ファイル名称入りのフルパスでアドレス指定
                    foldersFrom.Add(fileName);
                }
            }

            return foldersFrom;

        }

        public void ReadData(List<string> ALL_EFile)
        {
            List<string> row_data = new List<string>();

            string[] arr;
            List<string> ErrorCode = new List<string>();
            List<string> ErrorName = new List<string>();
            var ErrorCount = new Dictionary<string, int>();

            string filename;
            string filetype;
            string machineno;
            string lotno;
            string typename;
            string DBtype;
            int filenum = 0;
            bool data_exist;
            DateTime filetime;

            string codemoji;

            //昨日の日付
            //string dt = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");

            //初期の書き方　デフォルトで昇順っぽい
            //string[] names = Directory.GetFiles(FilePathCsv, "*");

            //昇順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderBy(f => f).ToList();
            //降順
            //List<string> names = Directory.GetFiles(FilePathCsv, "*").OrderByDescending(f => f).ToList();

            foreach (string name in ALL_EFile)
            {
                ErrorCode.Clear();
                ErrorName.Clear();
                ErrorCount.Clear();
                row_data.Clear();

                //パス名からファイル名のみ取得
                filename = Path.GetFileName(name);
                filetime = File.GetLastWriteTime(name);

                //H(pre) or I(post)ファイル
                filetype = filename.Substring(0, 1);

                //号機名
                machineno = filename.Substring(1, 4);
                if (machineno.Substring(0, 1) == "0")
                {
                    machineno = machineno.Substring(1, 3);
                }

                //最後のファイルになった場合、先に足さないと、次の処理で条件に入らない
                //filenum = filenum + 1;

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

                //1行ずつ読込
                using (var sr = new StreamReader(name, Enc))
                {
                    while (sr.Peek() > -1)
                    {
                        row_data.Add(sr.ReadLine());
                    }
                }

                foreach (string data in row_data)
                {
                    arr = data.Split(',');
                    //skipのデータを飛ばすため8以上としている
                    if (arr.Length >= 8)
                    {
                        if (arr[8].IndexOf("@@") > 0)
                        {
                            codemoji = arr[8].Remove(0, arr[8].IndexOf("@@"));
                            codemoji = codemoji.Remove(codemoji.LastIndexOf("@@"));
                            codemoji = codemoji.Replace("@@", "");
                        }
                        else
                            codemoji = arr[6];
                        
                        //ErrorCode:6,ErrorName:8 20230213 arr[6]だとエラーコードが0の時があり、確実ではない
                        //ErrorCode.Add(arr[6]);
                        ErrorCode.Add(codemoji);
                        ErrorName.Add(arr[8]);

                        data_exist = false;
                        foreach (KeyValuePair<string, int> dicItem in ErrorCount)
                        {
                            if(dicItem.Key == codemoji)
                            {
                                ErrorCount[codemoji] += 1;
                                /*
                                if(ErrorCount[arr[6]] >=100)
                                {
                                    data_exist = true;
                                }
                                */
                                data_exist = true;
                                break;
                            }
                        }

                        //初回はカウントをキーとカウント1をいれる
                        if (data_exist == false)
                            ErrorCount.Add(codemoji, 1);
                    }
                }
                foreach (KeyValuePair<string, int> dicItem2 in ErrorCount)
                {
                    //データビューへ挿入
                    //dataGridView1.Rows.Add(lotno, machineno, DBtype, typename, dicItem.Key, dicItem.Value);

                    //DBへデータ格納
                    LotInsert(lotno, machineno, DBtype, typename, dicItem2.Key, dicItem2.Value, filetime);
                }
            }
        }

        public void LotInsert(string lotno, string machineno, string DBtype, string typename, string errorcode, int errorcount, DateTime filetime)
        {
            if (DBtype == "1") DBtype = "ZD";
            else if (DBtype == "4") DBtype = "LED";

            string constr = "server=svopt01;Connect Timeout=30;Application Name=OEMTest;UID=sa;PWD=admin_4121opt;database=OEMTest;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = cmd.CommandText = @"
                            INSERT
                             INTO TnDBErrorHistory(lotno
	                            , machineno
                                , zdled
                                , typecd
                                , errorcode
                                , errorcount
                                , lastupddt)
                            values(@lotno
	                            , @machineno
                                , @zdled
                                , @typecd
                                , @errorcode
                                , @errorcount
                                , @lastupddt)";

                cmd.Parameters.Add(new SqlParameter("@lotno", lotno));
                cmd.Parameters.Add(new SqlParameter("@machineno", machineno));
                cmd.Parameters.Add(new SqlParameter("@zdled", DBtype));
                cmd.Parameters.Add(new SqlParameter("@typecd", typename));//20220518 mix_first_list[0].ledrank
                cmd.Parameters.Add(new SqlParameter("@errorcode", errorcode));
                cmd.Parameters.Add(new SqlParameter("@errorcount", errorcount));
                cmd.Parameters.Add(new SqlParameter("@lastupddt", filetime));

                try 
                {
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    return;
                    //throw;
                }
                

                con.Close();
            }
        }

    }
}
