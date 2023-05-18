using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArmsNascaBridge3
{
	static class Program
	{
		/// <summary>
		/// アプリケーションのメイン エントリ ポイントです。
		/// </summary>
		[STAThread]
		static void Main(string[] args)
		{
			if (ArmsApi.Model.Software.CanActivateSoftware(ArmsApi.Model.Software.Soft.NascaBridge) == false)
			{
				MessageBox.Show("管理されていないPCではソフトウェアを起動できません。\r\nこのPCで使用する場合はシステム管理者に連絡して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

            Mutex mutexObject;

            try
            {
                // ミューテックスを生成する
                mutexObject = new Mutex(false, "ARMSNascaBridge3InstaceMutex");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                // グローバル・ミューテックスによる多重起動禁止
                Console.WriteLine("多重起動禁止1");
                return;
            }

            // ミューテックスを取得する
            if (mutexObject.WaitOne(0, false))
            {
                try
                {
                    Application.Run(FrmBridgeMain.GetInstance());
                }
                finally
                {
                    // ミューテックスを解放する
                    mutexObject.ReleaseMutex();
                    // ミューテックスを破棄する
                    mutexObject.Close();
                }
            }
            else
            {
                //終了
                Console.WriteLine("多重起動禁止2");
                // ミューテックスを破棄する
                mutexObject.Close();
                return;
            }
        }
	}
}
