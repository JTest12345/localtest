using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;


namespace Arms3PCD
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
        }

        private void btnGo_Click(object sender, EventArgs e)
        {
            go();
        }

        private void go()
        {
            //string empcd = "";

            string raw = this.txtEmpCd.Text;
            if (string.IsNullOrEmpty(raw)) return;

            string[] splt = raw.Split(' ');
            if (splt.Length == 2 && splt[0] == "01")
            {
                //empcd = splt[1];
            }
            else
            {
                MessageBox.Show("不正なコードです");
                return;
            }

            this.txtEmpCd.Text = "";
            FrmLotList lst = new FrmLotList();
            //lst.EmpCd = empcd;
            this.Hide();
            lst.ShowDialog();
            this.Close();
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void FrmLogin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                go();
            }
        }


        private void FrmLogin_Load(object sender, EventArgs e)
        {

        }
    }
}