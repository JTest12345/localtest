using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Oskas
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ARMSInstanceMutex");
            if (mutex.WaitOne(0, false) == false)
            {
                MessageBox.Show("多重起動はできません。");
                return;
            }

            try
            {
                Application.Run(new fmMain());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
