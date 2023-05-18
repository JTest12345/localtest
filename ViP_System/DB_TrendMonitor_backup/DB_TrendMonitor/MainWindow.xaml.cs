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

namespace DB_TrendMonitor
{
    /// <summary>
    /// MainWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Random rand = new Random();
            //折れ線グラフのデータ作成
            double[] xs = Enumerable.Range(0, 100).Select(i => (double)i).ToArray();
            double[] ys = Enumerable.Range(0, 100).Select(i => (double)(i + rand.Next(0, 100) * ((rand.Next(1, 2) == 1) ? 1 : -1))).ToArray();
            double[] ys1 = Enumerable.Range(0, 100).Select(i => Math.Sin(i * 0.1)).ToArray();
            double[] ys2 = Enumerable.Range(0, 100).Select(i => 2 + Math.Cos(i * 0.1)).ToArray();
            double[] ys3 = Enumerable.Range(0, 100).Select(i => 4 + Math.Sin(i * 0.1) * Math.Cos(i * 0.1)).ToArray();
            double[] ys4 = Enumerable.Range(0, 100).Select(i => 1000 + Math.Sin(i * 0.1) * Math.Cos(i * 0.1)).ToArray();

            //グラフの描画
            DrawLine(uxChart1, "折れ線グラフ", xs, ys);

            List<(string legend, double[] xs, double[] ys, int axis)> linedata = new List<(string legend, double[] xs, double[] ys, int axis)>();
            linedata.Add(("legend1", xs, ys1, 0));
            linedata.Add(("legend2", xs, ys2, 0));
            linedata.Add(("legend3", xs, ys3, 0));
            linedata.Add(("legend4", xs, ys4, 1));

            DrawLine(uxChart2, "折れ線グラフ（複数)", linedata);

            //散布図
            DrawScatt(uxChart3, "散布図", xs, ys);

            //円グラフ
            double[] values = { 778, 43, 283, 76, 184 };
            string[] labels = { "C#", "JAVA", "Python", "F#", "PHP" };
            DrawPie(uxChart4, "円グラフ", labels, values);

            //縦棒グラフ
            DrawColumn(uxChart5, "縦棒グラフ", labels, values);

            //横棒グラフ
            DrawBar(uxChart6, "横棒グラフ", labels, values);

            //グルーピングのデータ作成
            string[] items = { "コア数", "クロック数", "スレッド数", "キャッシュ", "価格" };
            List<(string, double[])> datas = new List<(string, double[])>();
            datas.Add(("Core-i7", new double[] { 8, 3.6, 16, 64, 70 }));
            datas.Add(("Core-i5", new double[] { 6, 2.8, 12, 32, 50 }));
            datas.Add(("Core-i3", new double[] { 4, 2.0, 8, 16, 30 }));
            datas.Add(("Pentium", new double[] { 2, 1.8, 4, 8, 10 }));
            datas.Add(("Celeron", new double[] { 2, 1.5, 4, 8, 5 }));

            //棒グラフ（グルーピング）
            DrawGroupColumn(uxChart7, "グループ", items, datas);

            //積み上げグラフ
            DrawStack(uxChart8, "積み上げ", items, datas);

            //レーダーチャート
            DrawRadar(uxChart9, "レーダー", items, datas);

            //箱ひげ図
            DrawPopulations(uxChart10, "箱ひげ", ys);

            //散布図と回帰直線
            DrawRegression(uxChart11, "回帰", xs, ys);

            ///ヒストグラム
            DrawHistgram(uxChart12, "ヒストグラム", ys, 20);

        }

        private void DB_TrendMonitor(object sender, RoutedEventArgs e)
        {
            /*SubWindow sw = new SubWindow();
            sw.Show();*/

            Random rand = new Random();
            //折れ線グラフのデータ作成
            double[] xs = Enumerable.Range(0, 100).Select(i => (double)i).ToArray();
            double[] ys = Enumerable.Range(0, 100).Select(i => (double)(i + rand.Next(0, 100) * ((rand.Next(1, 2) == 1) ? 1 : -1))).ToArray();

            List<DateTime> DateList = new List<DateTime>();
            double[] DL = new double[100];
            for (int index = 0; index < 100; index++)
                DL[index] = DateTime.Now.AddDays(index).ToOADate();

            //グラフの描画 ※X軸もDouble型で指定しなければいけない
            DrawLine(uxChart1, "折れ線グラフ", DL, ys);
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
