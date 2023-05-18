using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Windows.Forms;

namespace HDDChecker
{
    class Program
    {
        static void Main(string[] args)
        {
            ManagementObject mo =
    new ManagementObject("Win32_LogicalDisk=\"E:\"");
            //C:のドライブの容量を取得する
            //ulong ts = (ulong)mo.Properties["Size"].Value / (1024*1024*1024);
            //Console.WriteLine("C:ドライブのサイズは{0}GBです。", ts);

            //C:のドライブの空き容量を取得する
            ulong fs = (ulong)mo.Properties["FreeSpace"].Value / (1024 * 1024 * 1024);
            //Console.WriteLine("C:ドライブの空き容量は{0}GBです。", fs);

            if(fs <= 10)
            {
                MessageBox.Show("注意：ドライブの空き容量が10GB以下です.\n空き容量：" + fs.ToString() + "GB");
            }           
                mo.Dispose();
            
        }
    }
}
