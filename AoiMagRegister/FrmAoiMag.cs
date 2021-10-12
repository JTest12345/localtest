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
using ArmsCejApi.Model;


namespace AoiBuilder
{
    public partial class FrmAoiMag : Form
    {

        // Keywords Dict
        Dictionary<string, string> Keys = new Dictionary<string, string>() {
            { "magno", "C30 " } ,
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

                //基板枚数
                var pcbnum = int.Parse(txt_bdvol.Text);

                List<PcbMark.PcbMarkF> pmds = new List<PcbMark.PcbMarkF>();
                for (int i=0; i<pcbnum; i++)
                {
                    PcbMark.PcbMarkF pmd = new PcbMark.PcbMarkF
                    {
                        PcbNo = i + 1,
                        PcbLotNo = txt_matlot.Text,
                        Markstr = ""
                    };
                    pmds.Add(pmd);
                }

                PcbMark.PcbMarkM pmm = new PcbMark.PcbMarkM
                {
                    MagNo = txt_magno.Text.Replace("C30 ", ""),
                    TypeCd = "",
                    MaterialCd = txt_matcode.Text,
                    WorkStDt = DateTime.Now,
                    WorkEndDt = DateTime.Now,
                    PcbMarks = pmds
                };

                string errmsg = PcbMark.InsertPcbMark(pmm);
                if (errmsg != "")
                {
                    MessageBox.Show(errmsg);
                }
                else
                {
                    MessageBox.Show("登録完了しました");
                }

                // MessageBox.Show("登録完了しました");
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
                if (item.Key == "magno")
                {
                    if (string.IsNullOrEmpty(txt_magno.Text))
                    {
                        msg = "マガジンNOの入力が不正です";
                        txt_magno.Focus();
                        return false;
                    }
                    else if (string.IsNullOrEmpty(item.Value) == false)
                    {
                        if (txt_magno.Text.Contains(item.Value) == false)
                        {
                            msg = "マガジンNOの入力が不正です";
                            txt_magno.Focus();
                            return false;
                        }
                    }
                }
                else if (item.Key == "fcode")
                {
                    if (string.IsNullOrEmpty(txt_matcode.Text))
                    {
                        msg = "基板部材コードの入力が不正です";
                        txt_matcode.Focus();
                        return false;
                    }
                    else if (string.IsNullOrEmpty(item.Value) == false)
                    {
                        if (txt_matcode.Text.Contains(item.Value) == false)
                        {
                            msg = "基板部材コードの入力が不正です";
                            txt_matcode.Focus();
                            return false;
                        }
                    }
                }
                else if (item.Key == "lotno")
                {
                    if (string.IsNullOrEmpty(txt_matlot.Text))
                    {
                        msg = "基板ロットの入力が不正です";
                        txt_matlot.Focus();
                        return false;
                    }
                    else if (string.IsNullOrEmpty(item.Value) == false)
                    {
                        if (txt_matlot.Text.Contains(item.Value) == false)
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
