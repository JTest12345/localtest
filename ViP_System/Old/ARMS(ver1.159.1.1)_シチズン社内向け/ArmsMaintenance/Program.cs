using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArmsMaintenance
{
    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            if (ArmsApi.Model.Software.CanActivateSoftware(ArmsApi.Model.Software.Soft.Maintenance) == false)
            {
                MessageBox.Show("管理されていないPCではソフトウェアを起動できません。\r\nこのPCで使用する場合はシステム管理者に連絡して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            try
            {
                //ネットワークサーバの設定ファイルの更新日時が新しい場合、ローカルにコピーする
                bool configUpdatefg = false;
                try
                {
                    configUpdatefg = ArmsApi.Config.CopySettingFileFromServer(ArmsApi.Config.Settings.OriginalConfigFileStorageFolderPath, 0);
                }
                catch
                {
                    //ファイルコピーに失敗した場合は、確認メッセージを表示し、そのまま起動できるように対応
                    if (DialogResult.OK != MessageBox.Show(string.Format("ネットワークサーバの設定ファイルコピーに失敗しました。このまま起動してもよろしいですか？"), "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                    {
                        return;
                    }
                }

                //設定ファイルリロード
                ArmsApi.Config.Settings = null;
                ArmsApi.Config.LoadSetting();

                //TmServerに記録する
                ArmsApi.Model.Software.UpdateArmsMaintenanceSystemVersion(Application.ProductVersion, configUpdatefg);

                //設定ファイルのバージョンとClickOnceのバージョンに不整合がある場合はエラー終了
                string[] clickOnceVersion = Application.ProductVersion.Split('.');
                string[] configVersion = ArmsApi.Config.Settings.ArmsMaintenanceSystemVersion.Split('.');
                if (clickOnceVersion[0] != configVersion[0] ||
                    clickOnceVersion[1] != configVersion[1] ||
                    clickOnceVersion[2] != configVersion[2])
                {
                    MessageBox.Show("設定ファイルのシステムバージョンとインストールしたシステムバージョンが異なるため起動できません。\r\nインストールしたシステムもしくは設定ファイルに誤りがないか確認してください。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new FrmMagazineMainte());
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
