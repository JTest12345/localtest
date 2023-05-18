using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace EICS
{
    public partial class F04_Password : Form
    {
        private bool passCompleteFG = false;
        public bool PassCompleteFG 
        {
            get { return passCompleteFG; }
        }

        public F04_Password()
        {
            InitializeComponent();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.Trim().ToUpper() == "EICS3")
            {
                passCompleteFG = true;
                this.Close();
            }
            else 
            {
                txtPassword.Text = "";
            }
        }
    }
}
