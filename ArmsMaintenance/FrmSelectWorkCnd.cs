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
    public partial class FrmSelectWorkCnd : Form
    {
        public WorkCondition SelectedCnd { get; set; }

        public FrmSelectWorkCnd()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string code = this.txtCode.Text;
                string name = this.txtName.Text;

                if (code == string.Empty) code = null;
                if (name == string.Empty) name = null;

                WorkCondition[] conditions = WorkCondition.SearchCondition(code, null, name, true);

                this.grdConditions.DataSource = conditions;
                this.grdConditions.Columns["StartDt"].Visible = false;
                this.grdConditions.Columns["EndDt"].Visible = false;
                this.grdConditions.Columns["CondVal"].Visible = false;
                this.grdConditions.Columns["CondCd"].HeaderText = "コード";
                this.grdConditions.Columns["CondName"].HeaderText = "名称";

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnApply_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdConditions.SelectedRows.Count == 0)
                {
                    return;
                }

                SelectedCnd = (WorkCondition)this.grdConditions.SelectedRows[0].DataBoundItem;
                this.DialogResult = DialogResult.OK;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

    }
}
