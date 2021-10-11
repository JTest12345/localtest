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
    public partial class FrmSelectMachine : Form
    {

        public MachineInfo SelectedMachine { get; set; }
        public string SelectedMachineNo { get; set; }
        public string SelectedMachineName { get; set; }


        public FrmSelectMachine()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                MachineInfo[] machineList = null;
                if (rdoInline.Checked)
                {
                    machineList = MachineInfo.SearchMachine(null, null, true, false, true, null, null, null);
                }
                else
                {
                    machineList = MachineInfo.SearchMachine(null, null, false, true, true, null, null, null);
                }
                if (machineList == null) return;

                //全装置リスト取得
                MachineInfo[] filterd = machineList.Where(m => m.LongName.Contains(this.txtCond.Text)).ToArray();

                this.listBox1.DataSource = filterd;
                this.listBox1.DisplayMember = "LongName";
                this.listBox1.ValueMember = "MacNo";
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
                this.SelectedMachine = (MachineInfo)this.listBox1.SelectedItem;
                this.SelectedMachineName = this.listBox1.SelectedItem.ToString();
                this.SelectedMachineNo = this.listBox1.SelectedValue.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
