using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmMacResin : Form
    {
        /// <summary>
        /// 該当設備
        /// </summary>
        private MachineInfo machine;

        /// <summary>
        /// 更新前情報
        /// </summary>
        private Resin original;

        /// <summary>
        /// 新規追加時コンストラクタ
        /// </summary>
        /// <param name="machine"></param>
        public FrmMacResin(MachineInfo machine)
        {
            InitializeComponent();
            this.machine = machine;

            this.txtMacNo.Text = machine.MacNo.ToString();
            this.txtMachine.Text = machine.LongName;

            this.dtpInputDt.Value = DateTime.Now;
            this.dtpInputDt.Enabled = true;
            this.dtpRemoveDt.Value = DateTime.Now;
            this.dtpRemoveDt.Enabled = false;
            this.chkRemove.Checked = false;
        }


        /// <summary>
        /// 更新時コンストラクタ
        /// </summary>
        /// <param name="machine"></param>
        /// <param name="oldrecord"></param>
        public FrmMacResin(MachineInfo machine, Resin resin)
        {
            InitializeComponent();

            this.machine = machine;
            this.txtMacNo.Text = machine.MacNo.ToString();
            this.txtMachine.Text = machine.MachineName;

            this.original = resin;

            this.txtMixResultId.Text = resin.MixResultId;
            this.txtResinGpCd.Text = resin.ResinGroupCd;
            this.txtMixLimit.Text = resin.LimitDt.ToString();
            this.dtpInputDt.Value = resin.InputDt.Value;

            if (resin.isRemoved)
            {
                this.chkRemove.Checked = true;
                this.dtpRemoveDt.Enabled = true;
                this.dtpRemoveDt.Value = resin.RemoveDt.Value;
            }
            else
            {
                this.dtpRemoveDt.Enabled = false;
                this.chkRemove.Checked = false;
                this.dtpRemoveDt.Value = DateTime.Now;
            }
        }




        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FrmMacResin_Load(object sender, EventArgs e)
        {
            
        }


        /// <summary>
        /// 調合結果ID手動入力時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtMixResultId_Leave(object sender, EventArgs e)
        {
            loadResin();
        }


        /// <summary>
        /// 更新ボタン押下処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtMixResultId.Text)) return;

                DialogResult res = MessageBox.Show(this, "更新を保存します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (res != DialogResult.OK)
                {
                    return;
                }

                Resin r = Resin.GetResin(int.Parse(this.txtMixResultId.Text));
                if (r == null)
                {
                    MessageBox.Show("調合樹脂情報が存在しません:" + this.txtMixResultId);
                    return;
                }

                r.InputDt = this.dtpInputDt.Value;

                if (chkRemove.Checked) r.RemoveDt = this.dtpRemoveDt.Value;
                else r.RemoveDt = null;

                try
                {
					string msg;
                    bool isError = WorkChecker.IsErrorBeforeInputResin(r, this.machine, out msg, chkRemove.Checked);
                    if (isError)
					{
						MessageBox.Show(msg);
						return;
					}

                    this.machine.DeleteInsertMacResin(r);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("更新エラー:" + ex.Message);
                    return;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void chkRemove_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpRemoveDt.Enabled = this.chkRemove.Checked;
        }

        private void FrmMacResin_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                loadResin();
            }
        }


        private void loadResin()
        {
            Resin r = Resin.GetResin(int.Parse(this.txtMixResultId.Text));
            if (r != null)
            {
                this.txtResinGpCd.Text = r.ResinGroupCd;
                this.txtMixLimit.Text = r.LimitDt.ToString();
            }
        }

        private void FrmMacResin_Shown(object sender, EventArgs e)
        {
            this.txtMixResultId.Focus();
        }

    }
}
