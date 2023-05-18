using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using ScottPlot; // DataGenに使います


namespace DB_Monitor
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            // サンプル用のデータです。
            double[] pointX = { 1.0, 2.0, 3.0, 4.0, 5.0 };
            double[] pointY = { 1.0, 2.0, 3.0, 4.0, 5.0 };

            // グラフにタイトルを付けます
            formsPlot1.Plot.Title("タイトル");

            // X軸, Y軸に名前を付けます
            formsPlot1.Plot.XLabel("時間");
            formsPlot1.Plot.YLabel("値");

            // 線付きの散布図としてデータをセットします。凡例もあわせて設定します。
            formsPlot1.Plot.AddScatter(pointX, pointY, label: "漢字");
            

            // Legend(凡例)の表示を指定します(指定しないと表示されません)
            formsPlot1.Plot.Legend();

            // ScottPlotのコントロールに描画(表示)します。
            formsPlot1.Render();

            

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
            chart.Render();
        }

        private void button1_Click(object sender, EventArgs e)
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
            //DrawLine(uxChart1, "折れ線グラフ", xs, ys);
        }
    }
}
