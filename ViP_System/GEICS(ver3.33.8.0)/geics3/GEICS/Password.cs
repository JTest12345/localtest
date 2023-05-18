using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEICS
{
    public partial class Password : Form
    {
        string _sText = "";
        public Password()
        {
            InitializeComponent();
        }

        private void btnEICSStop_Click(object sender, EventArgs e)
        {
            if (txtbPassword.Text.Trim().ToUpper() == "CLOSE")
            {
                this.DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                this.DialogResult = DialogResult.Cancel;
                MessageBox.Show(Constant.MessageInfo.Message_3);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
		}
    }
}
