using ArmsApi;
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
    public partial class FrmMachineOperator : Form
    {
        public FrmMachineOperator()
        {
            InitializeComponent();
        }

        private void FrmMachineOperator_Load(object sender, EventArgs e)
        {
            Search(this.chkMonitorOnly.Checked);
        }

        public void Search(bool isMonitor)
        {
            if (Config.Settings.OperationMachineGroupShowKey == null)
            {
                MessageBox.Show("自動登録の装置作業者設定を使用する場合は、設定ファイル(ArmsConfig.xml)のOperationMachineGroupShowKeyに表示させたいキー文字を設定して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            bsItems.Clear();
            List<MachineInfo.Operator> list = MachineInfo.GetOperators(isMonitor, Config.Settings.OperationMachineGroupShowKey);
            bsItems.DataSource = list;

            foreach(DataGridViewRow row in grdItems.Rows)
            {
                if (HasOverWork(Convert.ToDateTime(row.Cells["LastUpdDt"].Value), Convert.ToInt32(row.Cells["WarningMinuteTm"].Value)))
                {
                    row.DefaultCellStyle.BackColor = Color.MistyRose;
                }
            }         
        }

        private void chkMonitorOnly_CheckedChanged(object sender, EventArgs e)
        {
            Search(this.chkMonitorOnly.Checked);
        }

        private void grdItems_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            string selectGroupCd = grdItems.Rows[e.RowIndex].Cells["OperationMacgroupCd"].Value.ToString().Trim();

            FrmOperationMachine frm = new FrmOperationMachine(selectGroupCd);
            frm.ShowDialog();
        }

        private void grdItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            try
            {
                txtQRCode.Text += e.KeyChar.ToString();

                if (e.KeyChar == (char)Keys.Enter)
                {
                    string[] qrCodeChars = txtQRCode.Text.Split(' ');

                    txtQRCode.Text = string.Empty;

                    string empCd = string.Empty;
                    if (qrCodeChars.Count() >= 2)
                    {
                        empCd = qrCodeChars[1];
                    }
                    else
                    {
                        empCd = qrCodeChars[0];
                    }

                    int newEmpCd = 0;
                    if (int.TryParse(empCd, out newEmpCd) == false)
                    {
                        throw new ApplicationException($"社員CDの読込で数値ではない文字を読みました。読込文字{txtQRCode.Text}");
                    }

                    using (var armsDB = new ArmsApi.Model.DataContext.ARMSDataContext(Config.Settings.LocalConnString))
                    {
                        if (armsDB.TmEmployee.Where(emp => emp.empcode == newEmpCd).Count() == 0)
                        {
                            throw new ApplicationException($"社員マスタに存在しない社員CDです。読込文字{newEmpCd}");
                        }
                    }

                    string nowEmpCd = grdItems.SelectedRows[0].Cells["OperationEmpCd"].Value.ToString();
                    if (string.IsNullOrEmpty(nowEmpCd))
                    {
                        nowEmpCd = "未設定";
                    }

                    if (DialogResult.OK != MessageBox.Show($"「{nowEmpCd}」から「{newEmpCd}」に変更します。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                    {
                        return;
                    }

                    string operationMacGroupCd = grdItems.SelectedRows[0].Cells["OperationMacgroupCd"].Value.ToString();

                    MachineInfo.EditMachineOperator(operationMacGroupCd, newEmpCd);

                    Search(chkMonitorOnly.AutoCheck);
                }
            }           
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void grdItems_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                // Enterボタン押下で(QR改行文字を想定)選択行が勝手に下に移動する動作をキャンセルさせる
                e.Handled = true;
            }
        }

        public bool HasOverWork(DateTime lastUpdDt, int limitMinutesTm)
        {
            DateTime limitDt = lastUpdDt.AddMinutes(limitMinutesTm);
            if (System.DateTime.Now >= limitDt)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
