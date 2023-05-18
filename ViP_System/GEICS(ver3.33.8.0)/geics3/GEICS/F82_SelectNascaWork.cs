using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLCommonLib.Commons;

namespace GEICS
{
    public partial class F82_SelectNascaWork : Form
    {
        public WorkInfo SelectWork { get; set; }

        public List<WorkInfo> workList { get; set; } 

        public F82_SelectNascaWork()
        {
            InitializeComponent();
        }

        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                this.workList = ConnectNASCA.GetNascaWork(this.txtWorkCD.Text, this.txtWorkNM.Text, false);
                this.bsItems.DataSource = workList.ToSortableBindingList();
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void toolReturnData_Click(object sender, EventArgs e)
        {
            this.SelectWork = this.workList?.FirstOrDefault(l => l.SelectFG);
            if (this.SelectWork == null)
            {
                this.SelectWork = new WorkInfo() { WorkCD = string.Empty, WorkNM = string.Empty };
            }
            
            this.Close();
            this.Dispose();
        }

        /// <summary>
        /// 全選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllSelect_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in this.grdItems.Rows)
            {
                dr.Cells["SelectFG"].Value = true;
            }
        }

        /// <summary>
        /// 全解除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllCansel_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in this.grdItems.Rows)
            {
                dr.Cells["SelectFG"].Value = false;
            }
        }        

        private void grdItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                if (e.ColumnIndex == this.grdItems.Columns["SelectFG"].Index)
                {
                    for(int i = 0; i < this.grdItems.Rows.Count; i++)
                    {
                        DataGridViewCheckBoxCell targetCell = ((DataGridViewCheckBoxCell)this.grdItems.Rows[i].Cells["SelectFG"]);
                        if (Convert.ToBoolean(targetCell.Value) || i != e.RowIndex)
                        {
                            targetCell.Value = false;
                        }
                        else
                        {
                            targetCell.Value = true;
                        }
                    }
                }
                this.grdItems.EndEdit();
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
