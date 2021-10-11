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
    public partial class FrmMacCnd : Form
    {
        private MachineInfo machine;

        /// <summary>
        /// 更新前情報
        /// </summary>
        private WorkCondition original;


        /// <summary>
        /// 新規追加時コンストラクタ
        /// </summary>
        /// <param name="machine"></param>
        public FrmMacCnd(MachineInfo machine)
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
        /// <param name="cnd"></param>
        public FrmMacCnd(MachineInfo machine, WorkCondition cnd)
        {
            InitializeComponent();
            this.machine = machine;
            this.original = cnd;
            this.txtMacNo.Text = machine.MacNo.ToString();
            this.txtMachine.Text = machine.LongName;

            this.txtCondCd.Text = cnd.CondCd;
            this.txtCondName.Text = cnd.CondName;
            this.txtCondVal.Text = cnd.CondVal;
            this.dtpInputDt.Value = cnd.StartDt;

            if (cnd.EndDt.HasValue)
            {
                this.chkRemove.Checked = true;
                this.dtpRemoveDt.Enabled = true;
                this.dtpRemoveDt.Value = cnd.EndDt.Value;
            }
            else
            {
                this.dtpRemoveDt.Enabled = false;
                this.chkRemove.Checked = false;
                this.dtpRemoveDt.Value = DateTime.Now;
            }
        }

        private void FrmCondition_Load(object sender, EventArgs e)
        {

        }

        private void chkRemove_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpRemoveDt.Enabled = chkRemove.Checked;
        }


        /// <summary>
        /// 更新ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnApplyEdit_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtCondCd.Text)) return;

                DialogResult res = MessageBox.Show(this, "更新を保存します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Exclamation);
                if (res != DialogResult.OK)
                {
                    return;
                }

                WorkCondition c = WorkCondition.GetCondition(this.txtCondCd.Text);
                if (c == null)
                {
                    MessageBox.Show("製造条件情報が存在しません:" + this.txtCondCd.Text);
                    return;
                }

                c.CondVal = this.txtCondVal.Text;
                c.StartDt = this.dtpInputDt.Value;

                if (chkRemove.Checked) c.EndDt = this.dtpRemoveDt.Value;
                else c.EndDt = null;

                try
                {
                    this.machine.DeleteInsertWorkCond(c);
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

        private void btnSelectCond_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectWorkCnd frm = new FrmSelectWorkCnd();
                DialogResult res = frm.ShowDialog();

                if (res == DialogResult.OK)
                {
                    this.txtCondCd.Text = frm.SelectedCnd.CondCd;
                    this.txtCondName.Text = frm.SelectedCnd.CondName;
                    this.txtCondVal.Text = "";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
