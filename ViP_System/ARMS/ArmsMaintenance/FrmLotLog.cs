using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
	/// <summary>
	/// 2015.2.10 車載3次 マガジン番号、工程、エラーフラグ、ライン番号追加
	/// </summary>
    public partial class FrmLotLog : Form
    {
        public FrmLotLog()
        {
            InitializeComponent();
        }

		private bool isOutputCheckMode = false;

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            DateTime? fromdt = null; DateTime? todt = null;
            if (dtpInFrom.Checked)
            {
                fromdt = dtpInFrom.Value.Date;
                todt = dtpInTo.Value;
            }

            List<AsmLotLog> logs 
				= AsmLot.GetLotLog(txtLotNO.Text.Trim(), fromdt, todt, txtLogMsg.Text.Trim(),
									txtMagazineNo.Text.Trim(), chkError.Checked, txtLineNo.Text.Trim());

			if (this.isOutputCheckMode) 
			{
				// 排出確認モードの場合、現時刻から1時間前までの履歴を降順で表示する
				logs = logs.Where(l => l.InDt >= System.DateTime.Now.AddHours(-1)).OrderByDescending(l => l.InDt).ToList();
			}

            bsItems.DataSource = logs;
        }

        private void dtpInFrom_ValueChanged(object sender, EventArgs e)
        {
            if (!dtpInFrom.Checked)
            {
                dtpInTo.Enabled = false;
            }
            else 
            {
                dtpInTo.Enabled = true;
            }
        }

        private void dtpInTo_ValueChanged(object sender, EventArgs e)
        {
            dtpInFrom.Checked = true;
		}

		private void 排出確認モードToolStripMenuItem_CheckedChanged(object sender, EventArgs e)
		{
			if (((ToolStripMenuItem)sender).Checked)
			{
				dtpInFrom.Checked = false;
				this.isOutputCheckMode = true;
			}
			else
			{
				dtpInFrom.Checked = true;
				this.isOutputCheckMode = false;
			}
			dtpInFrom_ValueChanged(sender, e);
		}
    }
}
