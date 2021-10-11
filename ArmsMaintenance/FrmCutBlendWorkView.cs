using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsMaintenance
{
    public partial class FrmCutBlendWorkView : Form
    {
        public FrmCutBlendWorkView()
        {
            InitializeComponent();
            dt = new DataTable();
            dt.Columns.Add("アッセンロット");
            dt.Columns.Add("装置");
            dt.Columns.Add("開始", typeof(DateTime));
            dt.Columns.Add("完了", typeof(DateTime));
            dt.Columns.Add("搬入Mg");
            dt.Columns.Add("ブレンドロット");
            dt.Columns.Add("NASCA連携済");
            this.grdWork.DataSource = dt;
        }

        DataTable dt;
        MachineInfo machine;

        private void btnSelectMachine_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMachine frmmac = new FrmSelectMachine();
                DialogResult res = frmmac.ShowDialog();

                if (res == DialogResult.OK)
                {
                    MachineInfo m = frmmac.SelectedMachine;
                    this.machine = m;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.txtMacNo.Text))
            {
                MessageBox.Show("装置を選択してください");
                return;
            }

            int macno = 0;
            int.TryParse(this.txtMacNo.Text, out macno);

            DateTime? startfrom = (this.chkWorkStart.Checked) ? (DateTime?)dtpWorkStartFrom.Value : null;
            DateTime? startto = (this.chkWorkStart.Checked) ? (DateTime?)dtpWorkStartTo.Value : null;

            DateTime? endfrom = (this.chkWorkEnd.Checked) ? (DateTime?)dtpWorkEndFrom.Value : null;
            DateTime? endto = (this.chkWorkEnd.Checked) ? (DateTime?)dtpWorkEndTo.Value : null;


            CutBlend[] blends = CutBlend.SearchBlendRecord(null, null, macno, false, false, startfrom, startto, endfrom, endto);

            if (blends.Length >= 1)
            {
                blends = blends.OrderBy(b => b.StartDt).ToArray();
            }

            dt.Clear();
            int i = 1;
            foreach (CutBlend b in blends)
            {
                this.dt.Rows.Add(
                    new object[]{
                        b.LotNo, b.MacNo, b.StartDt, b.EndDt, 
                        b.MagNo, b.BlendLotNo, b.IsNascaEnd});

                i++;
                if (i >= 500)
                {
                    MessageBox.Show("検索結果が500件を超えた為中断");
                    break;
                }
            }

        }

        private void chkWorkStart_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpWorkStartFrom.Enabled = this.chkWorkStart.Checked;
            this.dtpWorkStartTo.Enabled = this.chkWorkStart.Checked;
        }

        private void chkWorkEnd_CheckedChanged(object sender, EventArgs e)
        {
            this.dtpWorkEndFrom.Enabled = this.chkWorkEnd.Checked;
            this.dtpWorkEndTo.Enabled = this.chkWorkEnd.Checked;
        }
    }
}
