using System;
using System.IO;
using System.IO.Ports;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

using static ManualWeighing.Program;

namespace ManualWeighing {

    public partial class SettingForm : Form {
        public SettingForm() {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロードイベント
        /// </summary>
        private void SettingForm_Load(object sender, EventArgs e) {

            manual_folderpath_label.Text = AppSettings.ManualFolderPath;
            balance_plantcd_textBox.Text = AppSettings.BalancePlantcd;

            //電子天秤シリアルポート
            int i = 0;
            foreach (string port in SerialPort.GetPortNames()) {
                balance_serialPortName_comboBox.Items.Add(port);
                if (port == AppSettings.BalanceSerialPortName) {
                    balance_serialPortName_comboBox.SelectedIndex = i;
                }
                i += 1;
            }

            //kanomaxシリアルポート 20230308
            i = 0;
            foreach (string port in SerialPort.GetPortNames())
            {
                kanomax_serialPortName_comboBox.Items.Add(port);
                if (port == AppSettings.kanomaxSerialPortName)
                {
                    kanomax_serialPortName_comboBox.SelectedIndex = i;
                }
                i += 1;
            }

            //プリンタ種類
            if (AppSettings.Printer == "bpac") {
                bpac_radioButton.Checked = true;
            }
            else {
                zpl_radioButton.Checked = true;
            }

            //ラベルフォーマット
            var label_paths = Directory.GetFiles(@"./settings\label", "*.lbx");
            int j = 0;
            foreach (string path in label_paths) {
                var name = Path.GetFileName(path);
                label_format_comboBox.Items.Add(name);
                if (name == AppSettings.LabelFormatFileName) {
                    label_format_comboBox.SelectedIndex = j;
                }
                j += 1;
            }

            dio_deviceName_textBox.Text = AppSettings.DioDeviceName;
            doorCheck_bitno_numericUpDown.Value = AppSettings.DoorCheckBitNo;
            stable_second_numericUpDown.Value = AppSettings.StableSecond;
            manualMix_second_numericUpDown.Value = AppSettings.ManualMixSecond;
            exhaust_second_numericUpDown.Value = AppSettings.ExhaustSecond;
            gross_weight_checkBox.Checked = AppSettings.CheckGrossWeight;
            gross_weight_allowableErrorGram_numericUpDown.Value = AppSettings.GrossWeightAllowableErrorGram;
            //pmms_constr_label.Text = $"{PmmsConStr.Split(';')[0]}  {PmmsConStr.Split(';')[1]}";
            system_fldpath_label.Text = SystemFileFolderPath;
        }

        private void ViewChange(Button btn) {
            btn.BackColor = Color.Yellow;
            btn.Update();
            Thread.Sleep(100);
            btn.BackColor = SystemColors.Control;
            btn.Update();
        }

        /// <summary>
        /// Manualフォルダパスの設定
        /// </summary>
        private void setting_manual_folderpath_button_Click(object sender, EventArgs e) {

            //初期表示パス設定
            folderBrowserDialog1.SelectedPath = AppSettings.ManualFolderPath;

            if (folderBrowserDialog1.ShowDialog() == DialogResult.OK) {
                string path = folderBrowserDialog1.SelectedPath;

                path = Convert_Vdrive(path);

                AppSettings.ManualFolderPath = path;

                AppSettings.Save_AppSetting();

                manual_folderpath_label.Text = AppSettings.ManualFolderPath;
            }
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
        /// 電子天秤設備番号設定
        /// </summary>
        private void setting_balance_plantcd_button_Click(object sender, EventArgs e) {
            AppSettings.BalancePlantcd = balance_plantcd_textBox.Text;
            AppSettings.Save_AppSetting();
            ViewChange((Button)sender);
        }

        /// <summary>
        /// 電子天秤通信シリアルポート設定
        /// </summary>
        private void balance_serialPortName_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            AppSettings.BalanceSerialPortName = balance_serialPortName_comboBox.SelectedItem.ToString();
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// プリンター種類設定
        /// </summary>
        private void printer_radioButton_CheckedChanged(object sender, EventArgs e) {
            var rb = (RadioButton)sender;
            if (rb.Checked == false) { return; }

            if (rb.Tag.ToString() == "bpac") {
                AppSettings.Printer = "bpac";
            }
            else {
                AppSettings.Printer = "zpl";
            }
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 印刷するラベルフォーマット設定
        /// </summary>
        private void label_format_comboBox_SelectedIndexChanged(object sender, EventArgs e) {
            AppSettings.LabelFormatFileName = label_format_comboBox.SelectedItem.ToString();
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// IOボードデバイス名設定
        /// </summary>
        private void setting_dio_deviceName_tbutton_Click(object sender, EventArgs e) {
            AppSettings.DioDeviceName = dio_deviceName_textBox.Text;
            AppSettings.Save_AppSetting();
            ViewChange((Button)sender);
        }


        private void setting_doorCheck_bitno_button_Click(object sender, EventArgs e) {

        }

        /// <summary>
        /// ドアチェック用のBitNo設定
        /// </summary>
        private void doorCheck_bitno_numericUpDown_ValueChanged(object sender, EventArgs e) {
            AppSettings.DoorCheckBitNo = (short)doorCheck_bitno_numericUpDown.Value;
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 計量安定時間設定
        /// </summary>
        private void stable_second_numericUpDown_ValueChanged(object sender, EventArgs e) {
            AppSettings.StableSecond = (int)stable_second_numericUpDown.Value;
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 手攪拌時間設定
        /// </summary>
        private void manualMix_second_numericUpDown_ValueChanged(object sender, EventArgs e) {
            AppSettings.ManualMixSecond = (int)manualMix_second_numericUpDown.Value;
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 排気時間設定
        /// </summary>
        private void exhaust_second_numericUpDown_ValueChanged(object sender, EventArgs e) {
            AppSettings.ExhaustSecond = (int)exhaust_second_numericUpDown.Value;
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 総重量判定するか設定
        /// </summary>
        private void gross_weight_checkBox_CheckedChanged(object sender, EventArgs e) {
            AppSettings.CheckGrossWeight = gross_weight_checkBox.Checked;
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 総重量判定の許容誤差設定
        /// </summary>
        private void gross_weight_allowableErrorGram_numericUpDown_ValueChanged(object sender, EventArgs e) {
            AppSettings.GrossWeightAllowableErrorGram = (decimal)gross_weight_allowableErrorGram_numericUpDown.Value;
            AppSettings.Save_AppSetting();
        }

        /// <summary>
        /// 風速計通信シリアルポート設定
        /// </summary>
        private void kanomax_serialPortName_comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.kanomaxSerialPortName = kanomax_serialPortName_comboBox.SelectedItem.ToString();
            AppSettings.Save_AppSetting();
        }
    }
}
