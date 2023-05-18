using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
    public partial class FrmRestrict : Form
    {
        public FrmRestrict()
        {
            InitializeComponent();
        }

        private void FrmRestrict_Load(object sender, EventArgs e)
        {
            Process[] procs = Process.GetWorkflowProcess();
            this.cmbProcess.DataSource = procs;
            this.cmbProcess.DisplayMember = "InlineProNM";
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            Process p = this.cmbProcess.SelectedItem as Process;
            int procno = p.ProcNo;

            string lotno = null;
            if (!string.IsNullOrEmpty(this.txtLotNo.Text))
            {
                lotno = this.txtLotNo.Text;
            }

            int? schProc = procno;
            if (chkProc.Checked == false) schProc = null;
            Restrict[] rlist = Restrict.SearchRestrict(lotno, schProc, this.chkOnlyActive.Checked);

            this.grdValues.DataSource = rlist;

            #region グリッド表示列変更

            foreach (DataGridViewColumn col in this.grdValues.Columns)
            {
                switch (col.Name)
                {
                    case "ColChk":
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット番号";
                        col.ReadOnly = true;
                        break;

                    case "ProcNm":
                        col.HeaderText = "投入規制工程";
                        col.ReadOnly = true;
                        break;

                    case "Reason":
                        col.HeaderText = "規制理由";
                        col.ReadOnly = true;
                        break;

                    case "DelFg":
                        col.HeaderText = "解除フラグ";
                        col.ReadOnly = true;
                        break;

                    case "LastUpdDt":
                        col.HeaderText = "更新日";
                        col.ReadOnly = true;
                        break;

                    case "ReasonKb":
                        col.HeaderText = "規制理由区分";
                        col.ReadOnly = true;
                        break;

                    case "RestrictReleaseFg":
                        col.HeaderText = "解除制限フラグ";
                        col.ReadOnly = true;
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }
            #endregion



        }

        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (this.grdValues.Rows.Count == 0)
            {
                return;
            }

            FrmEmpCdDialog f = new FrmEmpCdDialog();
            if (f.ShowDialog() != DialogResult.OK) return;          
            
            List<Restrict> rlist = new List<Restrict>();

            foreach (DataGridViewRow row in this.grdValues.Rows)
            {
                object cell = row.Cells["ColChk"].Value;

                if ((bool?)cell == true)
                {
                    if ((bool)row.Cells["RestrictReleaseFg"].Value == true 
                        && f.EmpId != Convert.ToInt32(row.Cells["UpdUserCd"].Value))
                    {
                        MessageBox.Show("規制した社員番号と一致しません");
                        return;
                    }
                    Restrict r = row.DataBoundItem as Restrict;
                    if (r == null) continue;
                    rlist.Add(r);
                }
            }

            if (rlist.Count >= 1)
            {
                DialogResult res  = MessageBox.Show(this, "規制を解除します", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Asterisk, MessageBoxDefaultButton.Button2);
                if (res != DialogResult.OK) return;
            }

            // 代表ロットが✔されている場合、波及ロットも解除対象に含める
            //→代表ロットが自ロットと一致している場合のみ波及ロットの規制を解除する
            if (rlist.Exists(r => string.IsNullOrEmpty(r.RepresentativeLotNo) == false && r.LotNo == r.RepresentativeLotNo))
            {
                DialogResult res = MessageBox.Show("波及ロットも存在するので全て規制解除します。よろしいですか？", "確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Information);
                if (res != DialogResult.OK) return;

                List<string> rLots = rlist
                    .Where(r => string.IsNullOrEmpty(r.RepresentativeLotNo) == false)
                    .Select(r => r.RepresentativeLotNo).ToList();

                foreach (string rLot in rLots)
                {
                    List<Restrict> spreadList = Restrict.GetRestrictFromRepresentativeLotNo(rLot);
                    foreach (Restrict spread in spreadList)
                    {
                        if (rlist.Exists(r => r.LotNo == spread.LotNo))
                            continue;

                        rlist.Add(spread);
                    }
                }
            }

            foreach (Restrict r in rlist)
            {
                r.DelFg = true;
                r.UpdUserCd = Convert.ToString(f.EmpId);
                r.Save();
            }

            MessageBox.Show(this, "解除完了");
            this.grdValues.DataSource = null;

        }
    }
}
