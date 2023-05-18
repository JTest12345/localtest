using System;
using System.Linq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace Arms3PCD
{
    public partial class FrmSetting : Form
    {
        public FrmSetting()
        {
            InitializeComponent();
        }

        private void FrmSetting_Load(object sender, EventArgs e)
        {
            this.txtUrl.Text = Config.Url;

            this.lstProc.Items.Clear();
            ProcData[] procs = Config.Procs;
            foreach (ProcData p in procs)
            {
                ListViewItem itm = this.lstProc.Items
                    .Add(new ListViewItem(new string[] { p.ProcNo, p.Name }));

                itm.Checked = p.Enabled;
            }
        }


        private ProcData[] getProcDataFromListView()
        {
            List<ProcData> retv = new List<ProcData>();

            foreach (ListViewItem item in this.lstProc.Items)
            {
                ProcData p = new ProcData();
                p.Enabled = item.Checked;
                p.ProcNo = item.Text;

                retv.Add(p);
            }

            return retv.ToArray();
        }

        private void menuSave_Click(object sender, EventArgs e)
        {
            Config.UpdateUrl(this.txtUrl.Text);
            Config.UpdateProcs(getProcDataFromListView());
            this.Close();

        }

        private void menuCancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lstProc_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}