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
    public partial class FrmOperationMachine : Form
    {
        private string SelectOperationMacgroupCd { get; set; }

        public FrmOperationMachine(string selectOperationMacgroupCd)
        {
            InitializeComponent();

            this.SelectOperationMacgroupCd = selectOperationMacgroupCd;
        }

        private void FrmOperationMachine_Load(object sender, EventArgs e)
        {
            search();
        }

        private void search()
        {
            if (Config.Settings.OperationMachineGroupShowKey == null)
            {
                MessageBox.Show("自動登録の装置作業者設定を使用する場合は、設定ファイル(ArmsConfig.xml)のOperationMachineGroupShowKeyに表示させたいキー文字を設定して下さい。", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MachineInfo.Operator item = MachineInfo.GetOperator(this.SelectOperationMacgroupCd, false, Config.Settings.OperationMachineGroupShowKey);
            this.txtWarningMinuteTm.Text = item.WarningMinuteTm.ToString();
            chkMonitorOnly.Checked = item.MonitorFg;

            MachineInfo[] machines = MachineInfo.GetOperationMachines(this.SelectOperationMacgroupCd);

            bsItems.DataSource = machines;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            grdItems.EndEdit();

            if (DialogResult.OK != MessageBox.Show("保存してよろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }

            try
            {
                int warningMinuteTm = 0;
                if (int.TryParse(txtWarningMinuteTm.Text, out warningMinuteTm) == false)
                {
                    throw new ApplicationException("時間設定には数値を入力してください。");
                }

                MachineInfo.EditMachineOperatorSetting(chkMonitorOnly.Checked, txtWarningMinuteTm.Text, this.SelectOperationMacgroupCd);

                foreach (DataGridViewRow row in grdItems.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["AddFG"].Value) == true)
                    {
                        MachineInfo.AddToOperationMacGroup(row.Cells["PlantCd"].Value.ToString(), this.SelectOperationMacgroupCd);
                    }
                    else if (Convert.ToBoolean(row.Cells["DeleteFG"].Value) == true)
                    {
                        MachineInfo.RemoveFromOperationMacGroup(row.Cells["PlantCd"].Value.ToString());
                    }
                }

                search();
                MessageBox.Show("保存完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            FrmSelectMachine frm = new FrmSelectMachine();
            frm.ShowDialog();

            if (frm.SelectedMachine == null)
                return;

            List<MachineInfo> machineList = ((MachineInfo[])bsItems.DataSource).ToList();
            if (machineList.Exists(m => m.NascaPlantCd == frm.SelectedMachine.NascaPlantCd))
            {
                MessageBox.Show("既に同設備が存在します。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (string.IsNullOrEmpty(frm.SelectedMachine.OperationMacGroupCd) == false)
            {
                MessageBox.Show($"既に{frm.SelectedMachine.OperationMacGroupCd}グループに属しています。削除後、再度追加をしてください。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            frm.SelectedMachine.OperationMacGroupCd = this.SelectOperationMacgroupCd;
            machineList.Add(frm.SelectedMachine);

            bsItems.DataSource = machineList.ToArray();

            int newRowIndex = grdItems.RowCount - 1;
            grdItems.Rows[newRowIndex].Cells["AddFG"].Value = true;
            grdItems.EndEdit();
        }
    }
}
