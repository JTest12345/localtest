using System.Runtime.CompilerServices;
using System.Net;
using System.ComponentModel;
using System.Diagnostics;
using System;
using System.Windows.Forms;
using System.Threading;
using System.Threading.Tasks;

namespace threadtest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();

            button1.Click += eventButton1Click;

            //Controls.AddRange(new Control[] { progressBar1, button1 });
        }

        public void HeavyTask(IProgress<float> progress)
        {
            for (var i = 1; i < 100; i++)
            {
                Thread.Sleep(10);
                progress.Report(i/3); // 進捗を報告
            }
        }

        private async void eventButton1Click(Object s, EventArgs e)
        {
            progressBar1.Value = 0; // プログレスバーの初期化

            var progress = new Progress<float>(x => {
                progressBar1.Value = (int)x; // 進捗を管理するオブジェクト(progress)の値とプログレスバーの値を紐づけ
            });

            await Task.Run(() => HeavyTask(progress));
        }

        
    }
}
