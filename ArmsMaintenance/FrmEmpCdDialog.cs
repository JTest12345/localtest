using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ArmsMaintenance
{
    public partial class FrmEmpCdDialog : Form
    {
        public int EmpId { get; set; }

        public FrmEmpCdDialog()
        {
            InitializeComponent();
        }

        private void btnOK_Click(object sender, EventArgs e)
        {
            int empId = 0;
            if (int.TryParse(txtCode.Text, out empId) == true)
            {
                this.EmpId = empId;
                DialogResult = DialogResult.OK;
                this.Close();
            }
            else
            {
                MessageBox.Show("社員番号には数値を入力して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                this.txtCode.Text = string.Empty;
                return;
            }
        }
    }
}
