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
    public partial class FrmWaferCheck : Form
    {
        public FrmWaferCheck()
        {
            InitializeComponent();
        }


        private void FrmWaferCheck_Load(object sender, EventArgs e)
        {
            ArmsApi.Model.MachineInfo[] maclist = ArmsApi.Model.MachineInfo.GetMachineList(false);
            foreach (ArmsApi.Model.MachineInfo m in maclist)
            {
                if (m.HasWaferChanger)
                {
                    this.cmbMachineList.Items.Add(new KeyValuePair<string, string>(m.NascaPlantCd, m.LongName));
                }
            }
        }



        private void setTimeFromLotNo(string plantcd)
        {
            ArmsApi.Model.MachineInfo mac = ArmsApi.Model.MachineInfo.GetMachine(plantcd);

            Order[] ord = Order.SearchOrder(this.txtLotNo.Text, null, mac.MacNo, false, false);

            DateTime? complt = null;
            if (ord.Length >= 0)
            {
                complt = ord[0].WorkEndDt;
            }

            if (complt.HasValue== false)
            {
                MessageBox.Show("ロットが見つかりません");
                return;
            }

            this.dateTimePicker2.Value = complt.Value;
            this.dateTimePicker1.Value = complt.Value.AddHours(-2);

        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.cmbMachineList.Text) == true)
            {
                return;
            }

            string machineID = ((KeyValuePair<string, string>)this.cmbMachineList.SelectedItem).Key;

            if (rdoLotSearch.Checked == true)
            {
                setTimeFromLotNo(machineID);
            }

            DBWaferLog[] mergedArray = DBWaferLog.GetAllLogs(machineID, this.dateTimePicker1.Value, this.dateTimePicker2.Value);

            this.dataGridView1.DataSource = mergedArray.OrderBy(key => key.LogDT).ToArray();

            this.dataGridView1.Columns["KB"].DisplayIndex = 1;
            this.dataGridView1.Columns["KB"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.dataGridView1.Columns["KB"].HeaderText = "区分";
            this.dataGridView1.Columns["KB"].SortMode = DataGridViewColumnSortMode.Automatic;

            this.dataGridView1.Columns["Message"].DisplayIndex = 3;
            this.dataGridView1.Columns["Message"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCellsExceptHeader;
            this.dataGridView1.Columns["Message"].HeaderText = "ログ";

            this.dataGridView1.Columns["SheetNO"].DisplayIndex = 2;
            this.dataGridView1.Columns["SheetNO"].AutoSizeMode = DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.dataGridView1.Columns["SheetNO"].HeaderText = "ウェハー段数";
        }

        private void txtLotNo_TextChanged(object sender, EventArgs e)
        {

        }
    }
}
