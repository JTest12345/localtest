using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExcelDataReader;
using System.IO;
using System.Data.SqlClient;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

namespace VFCalSearch
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            STPath.Text = @"\\cejfs1.citizen.local\Public\工程共用\LED\Lighting\Ctester\特性選別規格\MAP";
            //STPath.Text = @"D:\test\VFCal";           
        }

        //Form1オブジェクトを保持するためのフィールド
        private static Form1 _form1Instance;

        //Form1オブジェクトを取得、設定するためのプロパティ
        public static Form1 Form1Instance
        {
            get
            {
                return _form1Instance;
            }
            set
            {
                _form1Instance = value;
            }
        }

        public string TextBoxText
        {
            get
            {
                return STPath.Text;
            }
            set
            {
                STPath.Text = value;
            }
        }

        public async void ReadSTFile()
        {
            //int i;
            string filepath = STPath.Text;

            if (System.IO.Directory.Exists(filepath))
            {
                //MessageBox.Show("'" + filepath + "'は存在します。");
            }
            else
            {
                MessageBox.Show("'" + filepath + "'は存在しません。");
                return;
                
            }

            List<string> sheet_list = new List<string>();

            List<string> names = Directory.GetFiles(filepath, "*.xls*").OrderBy(f => f).ToList();
            float count = 1;
            
            progressBar1.Value = 0; // プログレスバーの初期化

            foreach (string name in names)
            {

                var progress = new Progress<float>(x => {
                    progressBar1.Value = (int)x; // 進捗を管理するオブジェクト(progress)の値とプログレスバーの値を紐づけ
                });

                await Task.Run(() => HeavyTask(progress, names.Count, count));
                count += 1;

                AsDataSetSample(name, "");

                if (progressBar1.Value == 100)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show("VfCalチェックが完了しました。", "完了");
                }
            }
        }
        

        public void AsDataSetSample(string filepath, string sheetname)
        {
            //ファイルの読み取り開始
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsb"))
                {
                    reader = ExcelReaderFactory.CreateReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else if (filepath.EndsWith(".csv"))
                {
                    reader = ExcelReaderFactory.CreateCsvReader(stream, new ExcelReaderConfiguration()
                    {
                        //デフォルトのエンコードは西ヨーロッパ言語の為、日本語が文字化けする
                        //オプション設定でエンコードをシフトJISに変更する
                        FallbackEncoding = Encoding.GetEncoding("Shift_JIS")
                    });
                }
                else
                {
                    MessageBox.Show("サポート対象外の拡張子です。");
                    return;
                }

                //全シート全セルを読み取り
                var dataset = reader.AsDataSet();
                string cellstr = "";
                string low = "";
                string vfcal_value = "";

                string typecd = "";
                string stkikaku = "";
                string filename_only = Path.GetFileName(filepath);
                //var worksheet = dataset.Tables[sheetname];
                //var worksheet = dataset.Tables;
                foreach (DataTable tbl in dataset.Tables)
                {
                    for (var j = 8; j < 10; j++)
                    //for (var j = 8; j < tbl.Rows.Count; j++)
                    //foreach (DataRow row in tbl.Rows)
                    {
                        //--- Col -----------
                        for (var i = 0; i < tbl.Columns.Count; i++)
                        {
                            //Console.Write("{0} ", row[i]);
                            //s = row[i].ToString();
                            cellstr = tbl.Rows[j][i].ToString();

                            //Vf Cal検索
                            if(cellstr == "Vf Cal")
                            {
                                low = tbl.Rows[j + 1][i].ToString();
                                //1行下がLowの文字
                                if (low == "Low")
                                {
                                    vfcal_value = tbl.Rows[j + 2][i].ToString();
                                    //2行下がLowの値なので
                                    if(vfcal_value == "")
                                    {
                                        //空白なら未登録リストへ登録
                                        typecd = tbl.Rows[0][2].ToString();
                                        stkikaku = tbl.Rows[2][2].ToString();
                                        VfCal_NGList.Text = VfCal_NGList.Text + filename_only + "___" + typecd + "___" + stkikaku + "\r\n";
                                    }
                                    else 
                                    {
                                        //typecd = tbl.Rows[0][2].ToString();
                                        //stkikaku = tbl.Rows[2][2].ToString();
                                        //VfCal_NGList.Text = VfCal_NGList.Text + typecd + "___" + stkikaku + "\r\n";
                                        break;
                                    }

                                }

                            }
                        }

                    }
                }
       
                reader.Close();
            }
        }

        private void VfCal_Check_Click(object sender, EventArgs e)
        {
            VfCal_NGList.Text = "";
            ReadSTFile();
            //string s = STPath.Text;
        }

        public void HeavyTask(IProgress<float> progress, float filecount, float count)
        {
            float i;
            
            i = 100 / filecount * count;
            //for (var i = 1; i < 100; i++)
            //{
                Thread.Sleep(1);
                progress.Report((int)i); // 進捗を報告
            //}
        }       

        private void GetParamOpen_Click(object sender, EventArgs e)
        {
            GetParam GP = new GetParam();
            // form2を表示
            GP.Show();
        }
    }
}
