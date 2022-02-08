using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FIFJsonBuilder
{
    public partial class fm_input : Form
    {
        public fm_main fmmain;

        public fm_input()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Visible=false;
        }

        private void button1_Click(object sender, EventArgs e)
        {

            if (fmmain != null)
            {
                fmmain.ShareString += "," + tb_sharestring.Text;
            }
            tb_sharestring.Text = "";
            this.Visible = false;
        }

    }
}
