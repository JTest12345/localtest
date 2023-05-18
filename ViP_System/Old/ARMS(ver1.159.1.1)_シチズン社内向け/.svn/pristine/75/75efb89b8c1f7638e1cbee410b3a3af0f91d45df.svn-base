using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;
using ArmsApi;

namespace ArmsNascaBridge2
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {            
            string commandLine = "";
            if (args.Length != 0)
            {
                commandLine = args[0].ToUpper();
            }

            if (string.IsNullOrWhiteSpace(commandLine) || commandLine != FrmBridgeMain.MANUAL_MODE)
            {
                if (ArmsApi.Model.Software.CanActivateSoftware(ArmsApi.Model.Software.Soft.NascaBridge) == false)
                {
                    MessageBox.Show("管理されていないPCではソフトウェアを起動できません。\r\nこのPCで使用する場合はシステム管理者に連絡して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }
            }

            Mutex mutexObject;

            try
            {
                // ミューテックスを生成する
                mutexObject = new Mutex(false, "ARMSNascaBridge2InstaceMutex");
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
                    Application.Run(FrmBridgeMain.GetInstance(commandLine));
                }
                finally
                {
                    Log.SysLog.Info("NascaBridge2 MutexDispose Start");

                    // ミューテックスを解放する
                    mutexObject.ReleaseMutex();
                    // ミューテックスを破棄する
                    mutexObject.Close();

                    Log.SysLog.Info("NascaBridge2 MutexDispose End");
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
