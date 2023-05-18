using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ExcelDataReader;
using System.Threading;

namespace VFCalSearch
{
    public partial class GetParam : Form
    {
        public GetParam()
        {
            InitializeComponent();
        }

        private void GetAllParam_Click(object sender, EventArgs e)
        {
            ReadParamFile();
        }

        public async void ReadParamFile()
        {
            //Form1のインスタンスの作成
            Form1 formdata = new Form1();
            //Form1Instanceに代入
            Form1.Form1Instance = formdata;
            //Form1の表示
            //formdata.Show();
            //int i;
            string filepath = formdata.TextBoxText;

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
                
                if(progressBar1.Value == 100)
                {
                    progressBar1.Value = 0;
                    MessageBox.Show("データ抽出が完了しました。", "抽出完了");
                }
            }
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
                string high = "";
                string min_low = "10000";
                string max_high = "-10000";

                string low_Ra = "";
                string high_Ra = "";
                string min_low_Ra = "10000";
                string max_high_Ra = "-10000";

                string low_R9 = "";
                string high_R9 = "";
                string min_low_R9 = "10000";
                string max_high_R9 = "-10000";

                bool numjudge;
                float f_judge;

                int offset = 0;
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
                            min_low = "10000";
                            max_high = "-10000";

                            
                             min_low_Ra = "10000";
                            max_high_Ra = "-10000";

                         min_low_R9 = "10000";
                           max_high_R9 = "-10000";
                            //Console.Write("{0} ", row[i]);
                            //s = row[i].ToString();
                            cellstr = tbl.Rows[j][i].ToString();

                            //Vf Cal検索
                            if (cellstr == "光束")
                            {
                                if(tbl.Rows[j][i + 4].ToString() == "Ra")
                                {
                                    offset = 2;
                                }
                                //for (var k = j + 2; k < tbl.Rows.Count; k++)
                                for (var k = j + 2; k < 270; k++)
                                {
                                    if (tbl.Rows[k][2].ToString() != "")
                                    {
                                        //光束規格値
                                        low = tbl.Rows[k][i].ToString();
                                        high = tbl.Rows[k][i + 1].ToString();

                                        //Ra規格値
                                        low_Ra = tbl.Rows[k][i + 2 + offset].ToString();
                                        high_Ra = tbl.Rows[k][i + 3 + offset].ToString();

                                        //R9規格値
                                        low_R9 = tbl.Rows[k][i + 4 + offset].ToString();
                                        high_R9 = tbl.Rows[k][i + 5 + offset].ToString();
                                    }
                                    else break;

                                    if(low == "" && low_Ra == "" && low_R9 == "")
                                    {
                                        break;
                                    }

                                    //光束規格判定
                                    numjudge = float.TryParse(low, out f_judge);
                                    if (numjudge == true && low != "")
                                    {
                                        if (float.Parse(low) < float.Parse(min_low))
                                        {
                                            min_low = low;
                                            
                                        }
                                    }

                                    numjudge = float.TryParse(high, out f_judge);
                                    if (numjudge == true && high != "")
                                    {
                                        if (float.Parse(high) > float.Parse(max_high))
                                        {
                                            max_high = high;

                                        }
                                    }

                                    //Ra規格判定
                                    numjudge = float.TryParse(low_Ra, out f_judge);
                                    if (numjudge == true && low_Ra != "")
                                    {
                                        if (float.Parse(low_Ra) < float.Parse(min_low_Ra))
                                        {
                                            min_low_Ra = low_Ra;

                                        }
                                    }

                                    numjudge = float.TryParse(high_Ra, out f_judge);
                                    if (numjudge == true && high_Ra != "")
                                    {
                                        if (float.Parse(high_Ra) > float.Parse(max_high_Ra))
                                        {
                                            max_high_Ra = high_Ra;

                                        }
                                    }

                                    //R9規格判定
                                    numjudge = float.TryParse(low_R9, out f_judge);
                                    if (numjudge == true && low_R9 != "")
                                    {
                                        if (float.Parse(low_R9) < float.Parse(min_low_R9))
                                        {
                                            min_low_R9 = low_R9;

                                        }
                                    }

                                    numjudge = float.TryParse(high_R9, out f_judge);
                                    if (numjudge == true && high_R9 != "")
                                    {
                                        if (float.Parse(high_R9) > float.Parse(max_high_R9))
                                        {
                                            max_high_R9 = high_R9;

                                        }
                                    }

                                }
                                offset = 0;

                                //20221118 規格がなかったやつはなしになるように設定
                                if (min_low == "10000") min_low = "下限なし";
                                if (max_high == "-10000") max_high = "上限なし";
                                if (min_low_Ra == "10000") min_low_Ra = "下限なし";
                                if (max_high_Ra == "-10000") max_high_Ra = "上限なし";
                                if (min_low_R9 == "10000") min_low_R9 = "下限なし";
                                if (max_high_R9 == "-10000") max_high_R9 = "上限なし";
                                //textへ登録
                                typecd = tbl.Rows[0][2].ToString();
                                stkikaku = tbl.Rows[2][2].ToString();
                                paramtext.Text = paramtext.Text + filename_only + "_" + typecd + "_" + stkikaku + "_" + min_low + "_" + max_high + "_" + min_low_Ra + "_" + max_high_Ra + "_" + min_low_R9 + "_" + max_high_R9 + "\r\n";
                                break;
                            }
                        }
                        //Console.WriteLine("");
                    }
                }           
                reader.Close();
            }
        }
    }
}
