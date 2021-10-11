using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ARMS3
{
    public partial class FrmConfirm : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmConfirm()
        {
            InitializeComponent();
        }

        /// <summary>認証完了フラグ</summary>
        public bool ConfirmCompleteFG { get; set; }

        /// <summary>
        /// OK
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            if (txtPassword.Text.ToUpper() == "CLOSE")
            {
                ConfirmCompleteFG = true;

                this.Close();
                this.Dispose();
            }
            else 
            {
                txtPassword.Text = string.Empty;
            }
        }

        /// <summary>
        /// Cancel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }
    }
}
