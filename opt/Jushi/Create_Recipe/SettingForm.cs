using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Configuration;

using static Create_Recipe.Program;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Create_Recipe {

    public partial class SettingForm : Form {

        public SettingForm() {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロードイベント
        /// </summary>
        private void SettingForm_Load(object sender, EventArgs e) {

            //コンボボックス初期化
            foreach (var pair in MainForm.place_dic) {
                var s = $"{pair.Key}:{pair.Value}";
                place_comboBox.Items.Add(s);

                if (pair.Key == Place) {
                    place_comboBox.SelectedIndex = place_comboBox.Items.IndexOf(s);
                }
            }

            manual_recipe_folderpath_label.Text = AppUsePath.ManualRecipe_SaveFolderPath;
            auto_recipe_folderpath_label.Text = AppUsePath.AutoMachine_SaveFolderPath.Auto_SaveFolderPath;
            add_recipe_folderpath_label.Text = AppUsePath.AutoMachine_SaveFolderPath.Add_SaveFolderPath;
            setting_remaining_weight_filepath_label.Text = AppUsePath.AutoMachine_RemainingWeight_FilePath;
            senko_log_filepath_label.Text = AppUsePath.SenkoLog_FilePath;
            if (AppUsePath.SenkologDatabase) {
                senko_log_title_label.Text = "先行評価ログはデータベース";
            }
            else {
                senko_log_title_label.Text = "先行評価ログはExcel";
            }


            system_folder_path_label.Text = SystemFileFolderPath;
            koda_constr_label.Text = $"{KodaConStr.Split(';')[0]}  {KodaConStr.Split(';')[1]}";
        }

        /// <summary>
        /// Vドライブは"V:"ではアクセスできないので書き換える
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public string Convert_Vdrive(string path) {
            string new_path = path;
            //Vドライブは"V:"ではアクセスできないので書き換える
            if (path.Substring(0, 2) == "V:") {
                new_path = path.Replace("V:", "\\\\svfile2\\fileserver");
            }
            return new_path;
        }

        /// <summary>
        /// コンボボックス選択時処理
        /// </summary>
        private void place_comboBox_SelectedIndexChanged(object sender, EventArgs e) {

            var cb = (System.Windows.Forms.ComboBox)sender;

            string place = cb.SelectedItem.ToString().Split(':')[0];

            if (place != Place) {
                Place = place;

                //製造場所をconfigファイルに保存
                try {
                    Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                    config.AppSettings.Settings["Place"].Value = Place;
                    config.Save();
                }
                catch {
                    MessageBox.Show($"コンフィグファイルに保存するのを失敗しました。", "保存失敗", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                }

                AppUsePath = UsePath.ReadSetting();
                manual_recipe_folderpath_label.Text = AppUsePath.ManualRecipe_SaveFolderPath;
                auto_recipe_folderpath_label.Text = AppUsePath.AutoMachine_SaveFolderPath.Auto_SaveFolderPath;
                add_recipe_folderpath_label.Text = AppUsePath.AutoMachine_SaveFolderPath.Add_SaveFolderPath;
                setting_remaining_weight_filepath_label.Text = AppUsePath.AutoMachine_RemainingWeight_FilePath;
                senko_log_filepath_label.Text = AppUsePath.SenkoLog_FilePath;
                if (AppUsePath.SenkologDatabase) {
                    senko_log_title_label.Text = "先行評価ログはデータベース";
                    senko_log_filepath_label.Font = new Font(senko_log_filepath_label.Font, FontStyle.Strikeout);
                }
                else {
                    senko_log_title_label.Text = "先行評価ログはExcel";
                    senko_log_filepath_label.Font = new Font(senko_log_filepath_label.Font, FontStyle.Regular);
                }

                MessageBox.Show("製造場所変更しました。", "変更完了", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
  
    }
}
