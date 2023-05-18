using ScottPlot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Data;
using ExcelDataReader;
using System.Data.SqlClient;


namespace DB_TrendMonitor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            string filePath = "";
            filePath = @"D:\test\DB傾向監視.xlsm";
            //filePath = @"\\SVFILE2\fileserver\301OEM\01.MAP\05 DB\201 OEM製造技術課 ＤＢ工程\システム\DB傾向監視.xlsm";

            InitializeComponent();

            if (!System.IO.File.Exists(filePath))
            {
                // ファイルが存在しない
                MessageBox.Show("Path: " + filePath +"にファイルがありません." + "\r\n" + "傾向監視ファイル保存先を確認してください.");
                Environment.Exit(0);
                //return;
            }
            
            GetSheetName(filePath);
        }

        private List<GraphData> GetGraphData(string macno)
        {
            List<GraphData> GD = new List<GraphData>();

            string constr;
            constr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                cmd.CommandText = "SELECT dbdate, shigma FROM TnDBTrend where macno like @macno order by dbdate asc";
                cmd.Parameters.Add(new SqlParameter("@macno", macno));
                //部分一致検索用


                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        GraphData t = new GraphData();
                        t.x_data = SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue;
                        t.y_data = SQLite.ParseSingle(reader["shigma"]);

                        GD.Add(t);
                    }
                }
                cmd.Parameters.Clear();
            }

            //GD.Add(new GraphData { x_data = DateTime.Now, y_data = "3" });
            return GD;
            
        }

        private void DB_TrendMonitor(object sender, RoutedEventArgs e)
        {
            /*SubWindow sw = new SubWindow();
            sw.Show();*/

            /*Random rand = new Random();
            //折れ線グラフのデータ作成
            double[] xs = Enumerable.Range(0, 100).Select(i => (double)i).ToArray();
            double[] ys = Enumerable.Range(0, 100).Select(i => (double)(i + rand.Next(0, 100) * ((rand.Next(1, 2) == 1) ? 1 : -1))).ToArray();

            List<DateTime> DateList = new List<DateTime>();
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();

            double[] DL = new double[100];
            for (int index = 0; index < 100; index++)
                DL[index] = DateTime.Now.AddDays(index).ToOADate();

            for (int index = 0; index < 100; index++)
            {
                xList.Add(DateTime.Now.AddDays(index).ToOADate());
                yList.Add(index);
            }

                //グラフの描画 ※X軸もDouble型で指定しなければいけない
                DrawLine2(uxChart1, "折れ線グラフ", xList, yList);*/

            // 現在実行しているアセンブリのファイル情報を取得する


            string filePath = "";
            string sheetname = "";
            filePath = @"D:\test\DB傾向監視.xlsm";

            //コンボボックスを選択していない場合
            if (excelsheet.SelectedItem == null)
            {
                MessageBox.Show("シート名が選択されていません.");
                return;
            }
            sheetname = excelsheet.SelectedItem.ToString();

            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("Path: " + filePath + "にファイルがありません." + "\r\n" + "傾向監視ファイル保存先を確認してください.");
                return;
            }

            //ReaderSample(filePath, sheetname);

            //全データ読込
            //AsDataSetSample(filePath, sheetname);
            AsDataDBset(filePath, sheetname);

            /*System.IO.FileStream stream = System.IO.File.Open(filePath, FileMode.Open, FileAccess.Read);

            // Excelファイル読み込む(2007 format; *.xlsx)
            ExcelDataReader.IExcelDataReader excelReader = ExcelDataReader.ExcelReaderFactory.CreateOpenXmlReader(stream);

            System.Data.DataSet result = null;
            result = excelReader.AsDataSet();
            // Excelファイルを閉じる
            excelReader.Close();*/


        }

        //データベースからデータを抽出してグラフへ描画
        private void AsDataDBset(string filepath, string sheetname)
        {
            //string moji;
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            List<double> UpperList = new List<double>();
            List<double> LowerList = new List<double>();

            List<String> machineList = new List<string>();


            //20220512変数名を配列にする
            WpfPlot[] charts = new[] { uxChart1 };//, uxChart2, uxChart3, uxChart4, uxChart5, uxChart6, uxChart7, uxChart8 };
            string machinename = "";

            List<(string legend, List<double> xs, List<double> ys, int axis)> linedata = new List<(string legend, List<double> xs, List<double> ys, int axis)>();


            string constr;
            constr = "server=VAUTOM4\\SQLExpress;Connect Timeout=30;Application Name=PMMS;UID=inline;PWD=R28uHta;database=PMMS;Max Pool Size=100";

            using (SqlConnection con = new SqlConnection(constr))
            using (SqlCommand cmd = con.CreateCommand())
            {
                con.Open();

                //設備リストを取得
                cmd.CommandText = "SELECT Distinct macno FROM TnDBTrend order by macno asc";
                
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {             
                        machineList.Add(SQLite.ParseString(reader["macno"]));
                    }
                }
                cmd.Parameters.Clear();

                //20220722未実装　コンボボックスで選択された指定のデータクラスに関してのみデータを抽出するように追加

                //設備1台ごとに日付データを抽出する
                foreach(var maclist in machineList) 
                {
                    xList.Clear();
                    yList.Clear();

                    cmd.CommandText = "SELECT macname, dbdate, shigma FROM TnDBTrend where class = @shname and macno like @macno order by dbdate asc";
                    cmd.Parameters.Add(new SqlParameter("@shname", sheetname));
                    cmd.Parameters.Add(new SqlParameter("@macno", maclist));


                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            machinename = SQLite.ParseString(reader["macname"]);
                            xList.Add((SQLite.ParseDate(reader["dbdate"]) ?? DateTime.MinValue).ToOADate());
                            yList.Add(SQLite.ParseSingle(reader["shigma"]));
                        }
                    }
                    cmd.Parameters.Clear();

                    var tempx = new List<double>(xList);
                    var tempy = new List<double>(yList);

                    linedata.Add((machinename, tempx, tempy, 0));

                }
            }

            //linedata.Add(("上限値", xList, UpperList, 0));
            //linedata.Add(("下限値", xList, LowerList, 0));

            DrawLine2(uxChart1, "DB傾向監視(σ)", linedata);
            
        }

        private void DB_Single(object sender, RoutedEventArgs e)
        {
            
            string filePath = "";
            string sheetname = "";
            filePath = @"D:\test\DB傾向監視.xlsm";

            //コンボボックスを選択していない場合
            if (excelsheet.SelectedItem == null)
            {
                MessageBox.Show("シート名が選択されていません.");
                return;
            }
            sheetname = excelsheet.SelectedItem.ToString();

            if (!System.IO.File.Exists(filePath))
            {
                MessageBox.Show("Path: " + filePath + "にファイルがありません." + "\r\n" + "傾向監視ファイル保存先を確認してください.");
                return;
            }

            //ReaderSample(filePath, sheetname);

            //全データ読込
            AsDataSetDBSingle(filePath, sheetname);
        

        }

        //指定シートの全データを1行ごとに読込
        private void ReaderSample(string filepath, string sheetname)
        {
            string moji;
            //ファイルの読み取り開始
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsb") || filepath.EndsWith(".xlsm"))
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

                for (int i = 0; i < reader.ResultsCount; i++)
                {
                    //シート名を指定
                    if (reader.Name != sheetname)
                    {
                        //次のシートへ移動
                        reader.NextResult();
                        continue;
                    }

                    while (reader.Read())
                    {
                        //1行毎に情報を取得 ※行全体は値取得しているが、1セル区切りで列をシフトしていってデータ取得している
                        for (int col = 0; col < reader.FieldCount; col++)
                        {
                            if (reader.GetValue(col) != null)
                            {
                                moji = reader.GetValue(col).ToString();
                            }
                            //セルの入力文字を読み取り
                            //Debug.WriteLine(reader.GetValue(col));
                        }
                    }
                }

                reader.Close();
            }
        }

        //指定シートの全データをセルごとに読込
        private void AsDataSetSample(string filepath, string sheetname)
        {
            //string moji;
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            List<double> UpperList = new List<double>();
            List<double> LowerList = new List<double>();

            //ファイルの読み取り開始
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsb") || filepath.EndsWith(".xlsm"))
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

                //シート名を指定
                var worksheet = dataset.Tables[sheetname];

                //20220512変数名を配列にする
                WpfPlot[] charts = new[] { uxChart1 };//, uxChart2, uxChart3, uxChart4, uxChart5, uxChart6, uxChart7, uxChart8 };
                string machinename="";

                List<(string legend, List<double> xs, List<double> ys, int axis)> linedata = new List<(string legend, List<double> xs, List<double> ys, int axis)>();
                //linedata.Add(("legend1", xs, ys1, 0));

                if (worksheet is null)
                {
                    MessageBox.Show("指定されたワークシート名が存在しません。");
                }
                else
                {
                    //セルの入力文字を読み取り
                    //何行目に何号機があるかを検索してその行数だけを取得すればよい
                    //for (var row = 0; row < worksheet.Rows.Count; row++)
                    for (var row = 13; row <= 30; row++)
                    {
                        xList.Clear();
                        yList.Clear();
                        UpperList.Clear();
                        LowerList.Clear();
                        for (var col = 3; col < worksheet.Columns.Count; col++)
                        {
                            if (worksheet.Rows[row][col].ToString() != "" && worksheet.Rows[12][col].ToString() != "")
                            {
                                machinename = worksheet.Rows[row][2].ToString();

                                    //日付 固定で12行目
                                    xList.Add((double)worksheet.Rows[12][col]);
                                    //傾向監視σ値
                                    yList.Add((double)worksheet.Rows[row][col]);
                                //UpperList.Add(10);
                                //LowerList.Add(1);
                            }
                            //Debug.WriteLine(worksheet.Rows[row][col]);
                        }
                        if(xList.Count > 0) 
                        {
                            var tempx = new List<double>(xList);
                            var tempy = new List<double>(yList);
                            //DrawLine2(charts[row-13], machinename, xList, yList);
                            linedata.Add((machinename, tempx, tempy, 0));
                        }
                    }
                }
                reader.Close();
                //linedata.Add(("上限値", xList, UpperList, 0));
                //linedata.Add(("下限値", xList, LowerList, 0));

                DrawLine2(uxChart1, "折れ線グラフ（複数)", linedata);
            }
        }

        private void AsDataSetDBSingle(string filepath, string sheetname)
        {
            //string moji;
            List<double> xList = new List<double>();
            List<double> yList = new List<double>();
            List<double> UpperList = new List<double>();
            List<double> LowerList = new List<double>();

            //ファイルの読み取り開始
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsb") || filepath.EndsWith(".xlsm"))
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

                //シート名を指定
                var worksheet = dataset.Tables[sheetname];

                //20220512変数名を配列にする
                WpfPlot[] charts = new[] { uxChart2, uxChart3, uxChart4, uxChart5, uxChart6, uxChart7, uxChart8, uxChart9 };
                string machinename = "";

                List<(string legend, List<double> xs, List<double> ys, int axis)> linedata = new List<(string legend, List<double> xs, List<double> ys, int axis)>();
                //linedata.Add(("legend1", xs, ys1, 0));

                if (worksheet is null)
                {
                    MessageBox.Show("指定されたワークシート名が存在しません。");
                }
                else
                {
                    //セルの入力文字を読み取り
                    //何行目に何号機があるかを検索してその行数だけを取得すればよい
                    //for (var row = 0; row < worksheet.Rows.Count; row++)
                    for (var row = 13; row <= 20; row++)
                    {
                        xList.Clear();
                        yList.Clear();
                        UpperList.Clear();
                        LowerList.Clear();
                        for (var col = 3; col < worksheet.Columns.Count; col++)
                        {
                            if (worksheet.Rows[row][col].ToString() != "" && worksheet.Rows[12][col].ToString() != "")
                            {
                                machinename = worksheet.Rows[row][2].ToString();

                                //日付 固定で12行目
                                xList.Add((double)worksheet.Rows[12][col]);
                                //傾向監視σ値
                                yList.Add((double)worksheet.Rows[row][col]);
                                //UpperList.Add(10);
                                //LowerList.Add(1);
                            }
                            //Debug.WriteLine(worksheet.Rows[row][col]);
                        }
                        if (xList.Count > 0)
                        {
                            //var tempx = new List<double>(xList);
                            //var tempy = new List<double>(yList);
                            DrawLine2(charts[row-13], machinename, xList, yList);
                            //linedata.Add((machinename, tempx, tempy, 0));
                        }
                    }
                }
                reader.Close();
                //linedata.Add(("上限値", xList, UpperList, 0));
                //linedata.Add(("下限値", xList, LowerList, 0));

                //DrawLine2(uxChart1, "折れ線グラフ（複数)", linedata);
            }
        }

        private void GetSheetName(string filepath)
        {
            
            //ファイルの読み取り開始
            //Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            using (FileStream stream = File.Open(filepath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IExcelDataReader reader;

                //ファイルの拡張子を確認
                if (filepath.EndsWith(".xls") || filepath.EndsWith(".xlsx") || filepath.EndsWith(".xlsb") || filepath.EndsWith(".xlsm"))
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

                //コンボボックス
                var name = dataset.Tables.OfType<DataTable>().Select(c => c.TableName).ToArray();
                //シート名をコンボボックスへ追加
                excelsheet.ItemsSource = name;

                reader.Close();
            }
        }

        private void DrawLine2(WpfPlot chart, string title, List<double> xs, List<double> ys)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            chart.Plot.AddSignalXY(xs.ToArray(), ys.ToArray());
            //X軸を日付に変更
            chart.Plot.XAxis.DateTimeFormat(true);
            
            chart.Render();
        }

        private void DrawLine2(WpfPlot chart, string title, List<(string legend, List<double> xs, List<double> ys, int axis)> datas)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);

            foreach (var data in datas)
            {
                var sig = chart.Plot.AddSignalXY(data.xs.ToArray(), data.ys.ToArray(), label: data.legend);
                sig.YAxisIndex = data.axis;
            }
            //data.axis に１つでも1以上があれば2軸を表示
            chart.Plot.YAxis2.Ticks(datas.Count(i => i.axis > 0) > 0);

            chart.Plot.Legend(location: Alignment.UpperRight);
            //X軸を日付に変更
            chart.Plot.XAxis.DateTimeFormat(true);
            chart.Render();
        }

        

        /// <summary>
        /// 折れ線グラフ
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        private void DrawLine(WpfPlot chart, string title, double[] xs, double[] ys)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            chart.Plot.AddSignalXY(xs, ys);
            //X軸を日付に変更
            chart.Plot.XAxis.DateTimeFormat(true);
            chart.Render();
        }

        /// <summary>
        /// 折れ線グラフ（複数）
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="datas"></param>
        private void DrawLine(WpfPlot chart, string title, List<(string legend, double[] xs, double[] ys, int axis)> datas)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);

            foreach (var data in datas)
            {
                var sig = chart.Plot.AddSignalXY(data.xs, data.ys, label: data.legend);
                sig.YAxisIndex = data.axis;
            }
            //data.axis に１つでも1以上があれば2軸を表示
            chart.Plot.YAxis2.Ticks(datas.Count(i => i.axis > 0) > 0);

            chart.Plot.Legend(location: Alignment.UpperRight);
            chart.Render();
        }

        /// <summary>
        /// 散布図
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        private void DrawScatt(WpfPlot chart, string title, double[] xs, double[] ys)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            chart.Plot.AddScatter(xs, ys, lineWidth: 0);
            chart.Render();
        }

        /// <summary>
        /// 円グラフ
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        /// <param name="values"></param>
        private void DrawPie(WpfPlot chart, string title, string[] labels, double[] values)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            var pie = chart.Plot.AddPie(values);
            pie.SliceLabels = labels;
            pie.ShowLabels = true;
            chart.Render();
        }

        /// <summary>
        /// 縦棒グラフ
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        /// <param name="ys"></param>
        private void DrawColumn(WpfPlot chart, string title, string[] labels, double[] ys)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            chart.Plot.AddBar(ys, DataGen.Consecutive(ys.Length));
            chart.Plot.XTicks(labels);
            chart.Render();

        }

        /// <summary>
        /// 横棒グラフ
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        /// <param name="ys"></param>
        private void DrawBar(WpfPlot chart, string title, string[] labels, double[] ys)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            var bar = chart.Plot.AddBar(ys, DataGen.Consecutive(ys.Length));
            bar.Orientation = ScottPlot.Orientation.Horizontal;
            chart.Plot.YTicks(labels);
            chart.Render();

        }

        /// <summary>
        /// 箱ひげ図
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="ys"></param>
        private void DrawPopulations(WpfPlot chart, string title, double[] ys)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            chart.Plot.AddPopulation(new ScottPlot.Statistics.Population(ys));
            chart.Plot.XAxis.Ticks(false);
            chart.Render();

        }

        /// <summary>
        /// 散布図と回帰直線
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="xs"></param>
        /// <param name="ys"></param>
        private void DrawRegression(WpfPlot chart, string title, double[] xs, double[] ys)
        {

            var model = new ScottPlot.Statistics.LinearRegressionLine(xs, ys);

            chart.Plot.Clear();
            chart.Plot.Title((title != "") ? title : "Y = {model.slope:0.0000}x + {model.offset:0.0}\nR² = {model.rSquared:0.0000}");
            chart.Plot.AddScatter(xs, ys, lineWidth: 0);
            chart.Plot.AddLine(model.slope, model.offset, (xs.Min(), xs.Max()), lineWidth: 2);
            chart.Render();
        }

        /// <summary>
        /// 縦棒グラフ（グループ）
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        /// <param name="values"></param>
        private void DrawGroupColumn(WpfPlot chart, string title, string[] labels, List<(string legend, double[] ys)> values)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);

            var datas = Enumerable.Range(0, values[0].ys.Length).Select(y => Enumerable.Range(0, values.Count).Select(x => values[x].ys[y]).ToArray()).ToArray();
            chart.Plot.AddBarGroups(values.Select(i => i.legend).ToArray(), labels, datas, null);
            chart.Plot.Legend(location: Alignment.UpperRight);
            chart.Render();
        }

        /// <summary>
        /// 積み上げグラフ
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        /// <param name="values"></param>
        private void DrawStack(WpfPlot chart, string title, string[] labels, List<(string legend, double[] ys)> values)
        {
            //チャートエリアのクリア
            chart.Plot.Clear();

            //タイトルの設定
            chart.Plot.Title(title);

            //X軸のデータを生成
            var x = Enumerable.Range(0, labels.Length).Select(i => (double)i).ToArray();

            //積み上げ用配列の初期化と合計計算
            var sum = values.Select(i => i.ys.Sum()).ToArray();

            //積み上げグラフ作成（トップから順に描画）
            for (int n = 0; n < values.Count; n++)
            {
                var bar = chart.Plot.AddBar(sum.ToArray(), x);
                bar.Label = labels[n];
                Enumerable.Range(0, sum.Length).Select(i => sum[i] = sum[i] - values[i].ys[n]).ToArray();
            }

            //凡例と軸の設定
            chart.Plot.Legend(location: Alignment.UpperRight);
            chart.Plot.XTicks(values.Select(i => i.legend).ToArray());

            //レンダリング
            chart.Render();
        }

        /// <summary>
        /// レーダーチャート
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="labels"></param>
        /// <param name="values"></param>

        private void DrawRadar(WpfPlot chart, string title, string[] labels, List<(string legend, double[] ys)> values)
        {
            chart.Plot.Clear();
            chart.Plot.Title(title);
            double[,] plots = new double[values.Count, values[0].ys.Length];

            Enumerable.Range(0, plots.GetLength(0)).Select(i => Enumerable.Range(0, plots.GetLength(1)).Select(j => plots[i, j] = values[i].ys[j]).ToArray()).ToArray();

            var radar = chart.Plot.AddRadar(plots);
            radar.CategoryLabels = labels;
            radar.OutlineWidth = 3;
            radar.GroupLabels = values.Select(i => i.legend).ToArray();
            chart.Plot.Legend(location: Alignment.UpperRight);
            chart.Plot.Grid();
            chart.Render();
        }

        /// <summary>
        /// ヒストグラム
        /// </summary>
        /// <param name="chart"></param>
        /// <param name="title"></param>
        /// <param name="ys"></param>
        /// <param name="backet"></param>
        private void DrawHistgram(WpfPlot chart, string title, double[] ys, int backet = 10)
        {
            double max = ys.Max();
            double min = ys.Min();
            var bin = Math.Abs(max - min) / backet;

            (double[] counts, double[] binEdges) = ScottPlot.Statistics.Common.Histogram(ys, min: min, max: max, binSize: bin);
            double[] leftEdges = binEdges.Take(binEdges.Length - 1).ToArray();

            chart.Plot.Clear();
            chart.Plot.Title(title);
            var bar = chart.Plot.AddBar(values: counts, positions: leftEdges);
            bar.BarWidth = bin;
            chart.Render();
        }       
    }
}
