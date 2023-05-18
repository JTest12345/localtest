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
    public partial class FrmCVWaitInput : Form
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public FrmCVWaitInput()
        {
            InitializeComponent();
        }

        /// <summary>
        /// FormLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmCVWaitInput_Load(object sender, EventArgs e)
        {
			//this.nudCVWaitMinutes.Value = Properties.Settings.Default.MoldConvayorWaitMinutes;
        }

        /// <summary>
        /// 設定変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApply_Click(object sender, EventArgs e)
        {
			//Properties.Settings.Default.MoldConvayorWaitMinutes = Convert.ToInt32(this.nudCVWaitMinutes.Value);
			//Properties.Settings.Default.Save();
        }
    }
}
