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
using ArmsApi;
using ARMS3.Model;
using ARMS3.Model.PLC;
using System.Data.SqlClient;
using System.Threading;


namespace ARMS3.FakeVIPline
{
    public partial class FrmAoiMag : Form
    {

        // Keywords Dict
        Dictionary<string, string> Keys = new Dictionary<string, string>() {
            { "magno", "" } ,
            { "fcode", "" } ,
            { "lotno", "" } ,
        };

        // Clear CheckBox Setting
        string[] conf;


        public FrmAoiMag()
        {
            InitializeComponent();
            ReadIniFile();
            InitCheckBoxes();
        }


        private void btn_setdata_Click(object sender, EventArgs e)
        {
            try
            {   
                string msg;
                if (!CheckKeyWordsAndValue(out msg))
                {
                    MessageBox.Show(msg);
                    return;
                }
                
                MessageBox.Show("完了しました");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            btn_clearTxtBox.Focus();
        }


        private void txt_bdvol_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            //0～9と、バックスペース以外の時は、イベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }


        public bool CheckKeyWordsAndValue(out string msg)
        {
            foreach (var item in Keys)
            {
                if (string.IsNullOrEmpty(item.Value))
                {
                    if (item.Key == "mag")
                    {
                        if (!txt_magno.Text.Contains(item.Value))
                        {
                            msg = "マガジンNOの入力が不正です";
                            txt_magno.Focus();
                            return false;
                        }
                    }
                    else if (item.Key == "fcode")
                    {
                        if (!txt_matcode.Text.Contains(item.Value))
                        {
                            msg = "基板部材コードの入力が不正です";
                            txt_matcode.Focus();
                            return false;
                        }
                    }
                    else if (item.Key == "lotno")
                    {
                        if (!txt_matlot.Text.Contains(item.Value))
                        {
                            msg = "基板ロットの入力が不正です";
                            txt_matlot.Focus();
                            return false;
                        }
                    }
                }
            }

            if (string.IsNullOrEmpty(txt_bdvol.Text))
            {
                msg = "基板数量が不正です";
                txt_bdvol.Focus();
                return false;
            }

            if (int.Parse(txt_bdvol.Text) < 1)
            {
                msg = "基板数量が不正です";
                txt_bdvol.Focus();
                return false;
            }

            msg = "";
            return true;
        }


        private void btn_clearTxtBox_Click(object sender, EventArgs e)
        {
            if (checkBox1.Checked) txt_matcode.Text = "";
            if (checkBox2.Checked) txt_matlot.Text = "";
            if (checkBox3.Checked) txt_magno.Text = "";
            if (checkBox4.Checked) txt_bdvol.Text = "";
            if (checkBox5.Checked) txt_typecd.Text = "";
        }


        private void ReadIniFile()
        {
            try
            {
                conf = File.ReadAllText(@"C:\ARMS\Config\aoibuilder.ini").Split(',');
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }    


        private void SaveIniFile()
        {
            try
            {
                var configstr = "";

                if (checkBox1.Checked)
                {
                    configstr += "1,";
                }
                else
                {
                    configstr += "0,";
                }

                if (checkBox2.Checked)
                {
                    configstr += "1,";
                }
                else
                {
                    configstr += "0,";
                }

                if (checkBox3.Checked)
                {
                    configstr += "1,";
                }
                else
                {
                    configstr += "0,";
                }

                if (checkBox4.Checked)
                {
                    configstr += "1,";
                }
                else
                {
                    configstr += "0,";
                }

                if (checkBox5.Checked)
                {
                    configstr += "1,";
                }
                else
                {
                    configstr += "0,";
                }


                File.WriteAllText(@"C:\ARMS\Config\aoibuilder.ini", configstr);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.ToString());
            }
        }
        
        private void InitCheckBoxes()
        {
            if (conf[0] == "1")
            {
                checkBox1.Checked = true;
            }
            else
            {
                checkBox1.Checked = false;
            }

            if (conf[1] == "1")
            {
                checkBox2.Checked = true;
            }
            else
            {
                checkBox2.Checked = false;
            }

            if (conf[2] == "1")
            {
                checkBox3.Checked = true;
            }
            else
            {
                checkBox3.Checked = false;
            }

            if (conf[3] == "1")
            {
                checkBox4.Checked = true;
            }
            else
            {
                checkBox4.Checked = false;
            }

            if (conf[4] == "1")
            {
                checkBox5.Checked = true;
            }
            else
            {
                checkBox5.Checked = false;
            }
        }

        private void FrmAoiMag_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveIniFile();
        }

        private void txt_bdvol_KeyPress_1(object sender, KeyPressEventArgs e)
        {
            //0～9と、バックスペース以外の時は、イベントをキャンセルする
            if ((e.KeyChar < '0' || '9' < e.KeyChar) && e.KeyChar != '\b')
            {
                e.Handled = true;
            }
        }

    }
}
