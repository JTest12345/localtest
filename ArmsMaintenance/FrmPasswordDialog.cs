using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ArmsMaintenance
{
    public partial class FrmPasswordDialog : Form
    {
        public string Password { get; set; }
        public FrmPasswordDialog(string password, string msg)
        {

            InitializeComponent();

            this.Password = password;
            this.txtMessage.Text = msg;
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            if (this.Password.ToUpper() == this.txtCode.Text.ToUpper())
            {
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else 
            {
                this.txtCode.Text = string.Empty;
                return;
            }


        }
    }
}
