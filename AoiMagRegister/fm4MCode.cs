using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace AoiMagBuilder
{
    public partial class fm4MCode : Form
    {
        public FrmAoiMag fmm;

        public fm4MCode()
        {
            InitializeComponent();
        }

        private void bt_clear4mcd_Click(object sender, EventArgs e)
        {
            txt_4Mcode.Text = "";
        }

        private void bt_Ok4mcd_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txt_4Mcode.Text) != true)
            {
                var receiveData = txt_4Mcode.Text.Split(',');

                if (receiveData.Length < 10)
                {
                    MessageBox.Show("入力された4Mの部材コード体系に問題があります");
                    return;
                }
                else
                {
                    fmm.ReceiveData = receiveData;
                }

                this.Hide();
            }
        }

        private void bt_Cancel4mcd_Click(object sender, EventArgs e)
        {
            this.Hide();
        }

        private void fm4MCode_VisibleChanged(object sender, EventArgs e)
        {
            txt_4Mcode.Text = "";
            txt_4Mcode.Focus();
        }
    }
}
