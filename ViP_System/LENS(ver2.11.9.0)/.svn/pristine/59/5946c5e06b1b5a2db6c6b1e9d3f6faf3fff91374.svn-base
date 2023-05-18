using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LENS2
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

#if ONLY_TOOLS
			Application.Run(new F03_MachineDefectMainte());
#else

			System.Threading.Mutex mutex = new System.Threading.Mutex(false, "LENSInstanceMutex");
			if (mutex.WaitOne(0, false) == false)
			{
				MessageBox.Show("多重起動はできません。");
				return;
			}

			try
			{
				Application.Run(new F01_Watch());
			}
			finally
			{
				mutex.ReleaseMutex();
			}
#endif
		}
	}
}
