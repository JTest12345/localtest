using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Runtime.InteropServices;
using System.Text.RegularExpressions;


namespace ARMSLogAnalysis
{
    public partial class Form1 : Form
    {

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, string lParam);

        [DllImport("User32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int uMsg, int wParam, int lParam);

        const int EM_REPLACESEL = 0x00C2;

        public Form1()
        {
            InitializeComponent();
        }

        //List格納定義
        string[][] loadCSV(string filePath)
        {
            var list = new List<string[]>();
            StreamReader reader =
            new StreamReader(filePath, System.Text.Encoding.GetEncoding("utf-8"));
            while (reader.Peek() >= 0) list.Add(reader.ReadLine().Split(','));
            reader.Close();
            return list.ToArray();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //最初に設備リストをCSVファイルから読み出し、配列に格納する。
            string[][] CSVList;
            string filePath = @"C:\ALA\MachineList\MAP自動搬送ライン(シチズン)_装置番号割付_201710201.csv";
            int i = 0, j = 0;
            //int sizetest = 0;
            string targetpath = @"C:\ALA\#1\";

            //CSVファイル読込
            CSVList = loadCSV(filePath);

            openFileDialog1.Filter = "All files(*.*)|*.*";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }

            System.IO.File.Copy(openFileDialog1.FileName, targetpath + openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 8, 8), true);

            textBox1.Text = targetpath + openFileDialog1.FileName.Substring(openFileDialog1.FileName.Length - 8, 8);//openFileDialog1.FileName;//選択したファイル名のパスがFileName
            string line, str = "";
            StreamReader sr = new StreamReader(textBox1.Text, Encoding.Default);
            if (File.Exists(textBox1.Text))
            {
                while ((line = sr.ReadLine()) != null) //テキストファイルを一行づつ読み込む
                {
                    //ここにテキストファイルから特定の設備番号があった場合、数値を文字に置き換えて表示する
                    //line=「変換後の文字列」
                    for (i = 0; i < CSVList.GetLength(0); i++)
                    {
                        //sizetest = CSVList.GetLength(0);
                        if (CSVList[i][0] != "" && line.Contains(CSVList[i][0]) == true)
                        {
                            //CSVファイル内に5列目まで入力が無いセルがあると、範囲外を参照しエラーとなる。[4]注意
                            line = line.Replace(CSVList[i][0],CSVList[i][4]);
                            //break;//ブレイクすると、対象行内にいくつか置換文字があった場合に置換されない
                        }
                    }
                    str = str + line + "\r\n";
                }
                textBox2.Text = str;
            }
            sr.Close();
        }

        private void button2_Click(object sender, EventArgs e)
        {

            textBox4.ResetText();
            string text = textBox2.Text;
            string search = textBox3.Text;
            //StringBuilder sb = new StringBuilder();

            //stringbuilder使用
            /*for (int i = 0; i < textBox2.Lines.Length; i++)
            {
                if (textBox2.Lines[i].Contains(search) == true)
                sb.Append(textBox2.Lines[i] +"\r\n");
            }
            textBox4.Text = sb.ToString();
            */

            //sendmessageの関数使用
           for (int i = 0; i < textBox2.Lines.Length; i++)
           {
               if (textBox2.Lines[i].Contains(search) == true)
               {
                   SendMessage(textBox4.Handle, EM_REPLACESEL, 1,
                       textBox2.Lines[i] + "\r\n");
               }

           }

           //textBox4.Text = sb.ToString();

            /*MatchCollection matchedObjects = Regex.Matches(textBox2.Text, search);

            foreach (Match matched in matchedObjects)
            {
                textBox4.Text += textBox4.Text + matched.Value;
            }
            */


            //20180627別処理コード、×動作しない
            /*System.IO.StringReader rs = new System.IO.StringReader(textBox2.Text);

            while (rs.Peek() > -1)
            {
                //一行読み込んで表示する
                //Console.WriteLine(rs.ReadLine());
                if (rs.ReadLine().Contains(search) == true)
                    textBox4.Text += (rs.ReadLine()) + "\r\n";

            }

            rs.Close();
            */

            //int pos = text.IndexOf(search);
            //textBox4.Text += string.Format("{0:d} 文字目で該当しました。\r\n", pos);


        }

        private void button3_Click(object sender, EventArgs e)
        {
            //Form2クラスのインスタンスを作成する
            Form2 f = new Form2();
            //Form2を表示する
            //ここではモードレスフォームとして表示する
            f.Show();

        }
        //#1ライン
        private void button4_Click(object sender, EventArgs e)
        {
            //最初に設備リストをCSVファイルから読み出し、配列に格納する。
            string[][] CSVList;
            string filePath = @"C:\ALA\MachineList\MAP自動搬送ライン(シチズン)_装置番号割付_201710201.csv";
            int i = 0, j = 0;
            string sourcepath = @"\\172.24.17.11\NEL\BIN\LOG";
            string targetpath = @"C:\ALA\#1";

            //CSVファイル読込
            CSVList = loadCSV(filePath);

            // コピー先のディレクトリがなければ作成する
            if (!System.IO.Directory.Exists(targetpath))
            {
                System.IO.Directory.CreateDirectory(targetpath);
                System.IO.File.SetAttributes(targetpath, System.IO.File.GetAttributes(targetpath));
                //bOverwrite = true;
            }



            //ファイル表示前に保存元⇒ローカルPC一時保管へ保存
            //本番
            System.IO.File.Copy(@"\\172.24.17.11\NEL\BIN\LOG\MOVELOG" + System.DateTime.Today.ToString("yyyyMMdd"), targetpath + @"\MOVELOG" + System.DateTime.Today.ToString("yyyyMMdd"), true);

            //練習
            //System.IO.File.Copy(@"C:\Users\jnk-nkmr\Downloads\ARMSLog\MOVELOG" + System.DateTime.Today.ToString("yyyyMMdd"), targetpath + @"\MOVELOG" + System.DateTime.Today.ToString("yyyyMMdd"), true);

            /*openFileDialog1.Filter = "All files(*.*)|*.*";
            if (openFileDialog1.ShowDialog() != DialogResult.OK)
            {
                return;
            }*/
            textBox1.Text = targetpath + @"\MOVELOG" + System.DateTime.Today.ToString("yyyyMMdd");//openFileDialog1.FileName;
            string line, str = "";
            StreamReader sr = new StreamReader(textBox1.Text, Encoding.Default);
            if (File.Exists(textBox1.Text))
            {
                while ((line = sr.ReadLine()) != null) //テキストファイルを一行づつ読み込む
                {
                    //ここにテキストファイルから特定の設備番号があった場合、数値を文字に置き換えて表示する
                    //line=「変換後の文字列」
                    for (i = 0; i < CSVList.GetLength(0); i++)
                    {
                        //sizetest = CSVList.GetLength(0);
                        if (CSVList[i][0] != "" && line.Contains(CSVList[i][0]) == true)
                        {
                            //CSVファイル内に5列目まで入力が無いセルがあると、範囲外を参照しエラーとなる。[4]注意
                            line = line.Replace(CSVList[i][0], CSVList[i][4]);
                            //break;//ブレイクすると、対象行内にいくつか置換文字があった場合に置換されない
                        }
                    }
                    str = str + line + "\r\n";
                }
                textBox2.Text = str;
            }
            sr.Close();
        }
        //#2ライン
        private void button5_Click(object sender, EventArgs e)
        {

        }
        //J1J2ライン
        private void button6_Click(object sender, EventArgs e)
        {

        }
        //J3J4ライン
        private void button7_Click(object sender, EventArgs e)
        {

        }
    }
}
