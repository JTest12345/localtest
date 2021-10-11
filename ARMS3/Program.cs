using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARMS3
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            //Configのひな型作成用
            //ArmsApi.Config.Settings.Save();
			//ArmsApi.Config.LoadSetting();

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
                //ネットワークサーバの設定ファイルの更新日時が新しい場合、ローカルにコピーする
                bool configUpdatefg = false;
                bool lineConfigUpdatefg = false;
                //try
                //{
                //    configUpdatefg = ArmsApi.Config.CopySettingFileFromServer(ArmsApi.Config.Settings.OriginalConfigFileStorageFolderPath, 0);
                //    lineConfigUpdatefg = ARMS3.Model.LineConfig.CopySettingFileFromServer(ArmsApi.Config.Settings.OriginalLineFileStorageFolderPath, 0);
                //}
                //catch (Exception ex)
                //{
                //    //ファイルコピーに失敗した場合は、確認メッセージを表示し、そのまま起動できるように対応
                //    if (DialogResult.OK != MessageBox.Show(string.Format("ネットワークサーバの設定ファイルコピーに失敗しました。このまま起動してもよろしいですか？ \r\nエラー内容：{0}", ex.Message), "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                //    {
                //        return;
                //    }
                //}

                //設定ファイルリロード
                ArmsApi.Config.Settings = null;
                ArmsApi.Config.LoadSetting();

                if (args.Count() == 0)
                {
                    //    //TmServerに記録する
                    //    if (ArmsApi.Model.Software.CanActivateSoftware(ArmsApi.Model.Software.Soft.Arms) == false)
                    //    {
                    //        MessageBox.Show("管理されていないPCではソフトウェアを起動できません。\r\nこのPCで使用する場合はシステム管理者に連絡して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //        return;
                    //    }
                    //    else
                    //    {
                    //        ArmsApi.Model.Software.UpdateArmsSystemVersion(Application.ProductVersion, configUpdatefg, lineConfigUpdatefg);
                    //    }

                    //    //設定ファイルのバージョンとClickOnceのバージョンに不整合がある場合はエラー終了
                    //    string[] clickOnceVersion = Application.ProductVersion.Split('.');
                    //    string[] configVersion = ArmsApi.Config.Settings.ArmsSystemVersion.Split('.');
                    //    if (clickOnceVersion[0] != configVersion[0] ||
                    //        clickOnceVersion[1] != configVersion[1] ||
                    //        clickOnceVersion[2] != configVersion[2])
                    //    {
                    //        MessageBox.Show("設定ファイルのシステムバージョンとインストールしたシステムバージョンが異なるため起動できません。\r\nインストールしたシステムもしくは設定ファイルに誤りがないか確認してください。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    //        return;
                    //    }


                    Application.Run(new FrmMain());
                    //Application.Run(new FakeVIPline.FrmFakeNline());
                    //Application.Run(new FakeVIPline.FrmLmSim());
                    //Application.Run(new FakeVIPline.FrmAoiMag());
                    //Application.Run(new FakeVIPline.TestForm());
                }
                else
                {
                    if (args[0].ToUpper() == "TEST")
                        Application.Run(new FrmTestMode());
                }
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
