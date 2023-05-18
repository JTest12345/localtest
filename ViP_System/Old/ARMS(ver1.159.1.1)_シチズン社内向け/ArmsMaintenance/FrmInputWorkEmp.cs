using ArmsApi.Model;
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
    public partial class FrmInputWorkEmp : Form
    {
        public FrmInputWorkEmp()
        {
            InitializeComponent();
        }

        private void FrmInputWorkEmp_Load(object sender, EventArgs e)
        {
            //コンボボックス(工程)
            Process[] procList = Process.SearchProcess(null, null, null);
            cmbProcess.ValueMember = "ProcNo";
            cmbProcess.DisplayMember = "InlineProNM";
            cmbProcess.DataSource = procList;
            cmbProcess.SelectedIndex = -1;

            //コンボボックス(勤務帯)
            WorkGroup[] workgroupList = WorkGroup.SearchWorkGroup();
            cmbWorkGroup.ValueMember = "starthour";
            cmbWorkGroup.DisplayMember = "workgroupnm";
            cmbWorkGroup.DataSource = workgroupList;
            cmbWorkGroup.SelectedIndex = -1;

            dtpWorkDt.Value = DateTime.Now;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(this.txtEmpCd.Text))
                {
                    MessageBox.Show("社員番号を入力してください");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                int empcd;
                if (!int.TryParse(this.txtEmpCd.Text, out empcd))
                {
                    MessageBox.Show(this, "社員番号は数字を入力していください。");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                if (this.cmbProcess.SelectedIndex < 0)
                {
                    MessageBox.Show(this, "工程を選択してください。");
                    this.DialogResult = DialogResult.None;
                    return;
                }
                if (this.cmbWorkGroup.SelectedIndex < 0)
                {
                    MessageBox.Show(this, "勤務帯を選択してください。");
                    this.DialogResult = DialogResult.None;
                    return;
                }

                WorkEngagementPeriod w = new WorkEngagementPeriod();
                w.EmpCd = empcd.ToString();
                w.Process = ((Process)cmbProcess.SelectedValue).ProcNo;

                decimal startHour = ((WorkGroup)cmbWorkGroup.SelectedValue).StartHour;
                decimal endHour = ((WorkGroup)cmbWorkGroup.SelectedValue).EndHour;

                DateTime baseDT = new DateTime(this.dtpWorkDt.Value.Year, this.dtpWorkDt.Value.Month, this.dtpWorkDt.Value.Day);
                w.StartDT = baseDT.AddHours((double)startHour);
                w.EndDT = baseDT.AddHours((double)endHour);

                w.Update();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
