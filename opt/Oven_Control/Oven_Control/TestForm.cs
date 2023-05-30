using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

using static Oven_Control.Program;
using Ftp;
using static Oven_Control.OperateFile;


namespace Oven_Control {
    public partial class TestForm : Form {
        public TestForm() {
            InitializeComponent();
            QRread_label.Text = "";
        }


        private void Mon_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Get_Mon();
            //label1.Text = str;
        }

        private void PrgmMon_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Get_PrgmMon();
            //label1.Text = str;
        }

        private void Alarm_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Get_Alarm();
            //label1.Text = str;
        }

        private void KeyProtect_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Get_KeyProtect();
            //label1.Text = str;
        }

        private void Standby_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Set_Mode_Standby();
            //label1.Text = str;
        }

        private void ModeOff_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Set_Mode_OFF();
            //label1.Text = str;
        }

        private void KeyProtectOn_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Set_KeyProtect_ON();
            //label1.Text = str;
        }

        private void KeyProtectOff_Click(object sender, EventArgs e) {
            //Oven oven = new Oven(20, test_endpoint);
            //string str = oven.Set_KeyProtect_OFF();
            //label1.Text = str;
        }

        private void ReadQR_textBox_KeyPress(object sender, KeyPressEventArgs e) {
            QRread_label.Text += CCV((int)e.KeyChar);
        }

        /// <summary>
        /// Control_Character_Visualization '制御文字可視化
        /// </summary>
        private string CCV(int decicode) {
            string str = "";
            string[] code = new string[] {"NUL","SOH","STX","ETX","EOT","ENQ","ACK","BEL","BS","HT",
                                          "LF","VT","FF","CR","SO","SI","DLE","DC1","DC2","DC3",
                                          "DC4","NAK","SYN","ETB","CAN","EM","SUB","ESC","FS","GS",
                                          "RS","US"};
            if (decicode < 32) {
                str = "<" + code[decicode] + ">";
            }
            else if (decicode == 32) {
                str = "<SP>";
            }
            else if (decicode == 127) {
                str = "<DEL>";
            }
            else {
                str = ((char)decicode).ToString();
            }
            return str;
        }

        private void button1_Click(object sender, EventArgs e) {

            //FTPアップロード先の情報読み取り
            string[] ftp_info = ReadSeraverIP();
            if (ftp_info != null) {
                //FTPアップロード
                string zip_path = FtpFolder + "/oven20_20210210 - 153518.zip";
                FTPclient.UploadFile(ftp_info[0], ftp_info[1], ftp_info[2], zip_path);
            }
        }
    }
}
