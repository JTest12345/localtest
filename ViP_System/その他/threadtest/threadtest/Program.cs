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
    public static class Program
    {
        
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
