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
	public partial class FrmSelectProcess : Form
	{
		public FrmSelectProcess()
		{
			InitializeComponent();
		}

		public FrmSelectProcess(Process activateProcess, bool isMultiSelect) 
		{
			InitializeComponent();

			ActivateProcess = activateProcess;

			IsMultiSelect = isMultiSelect;
		}

		public Process SelectProcess = null;

		public Process ActivateProcess = null;

		private bool IsMultiSelect = false;

		private void FrmSelectProcess_Load(object sender, EventArgs e)
		{
			if (IsMultiSelect)
			{
				btnAllSelect.Visible = true;
				btnAllCancel.Visible = true;
			}
			else 
			{
				btnAllSelect.Visible = false;
				btnAllCancel.Visible = false;
			}

			List<Process> procList = new List<Process>();

			int[] procNoList = Process.GetWorkFlowProcNo();
			foreach (int procNo in procNoList)
			{
				Process p = Process.GetProcess(procNo);
				procList.Add(p);
			}

			bsItems.DataSource = procList;

			if (ActivateProcess != null) 
			{
				foreach(DataGridViewRow row in dgvItems.Rows)
				{
					if (ActivateProcess.ProcNo == Convert.ToInt32(row.Cells["ProcNo"].Value))
					{
						row.Cells["SelectFg"].Value = true;
						break;
					}
				}
				SelectProcess = ActivateProcess;
			}
		}

		private void btnSubmit_Click(object sender, EventArgs e)
		{
			dgvItems.EndEdit();

			SelectProcess = null;

			foreach (DataGridViewRow row in dgvItems.Rows) 
			{
				if (Convert.ToBoolean(row.Cells["SelectFg"].Value) == true) 
				{
					List<Process> procList = (List<Process>)bsItems.DataSource;
					SelectProcess = procList[row.Index];
				}
			}

			this.Dispose();
			this.Close();
		}

		private void btnAllCancel_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in dgvItems.Rows)
			{
				row.Cells["SelectFg"].Value = false;
			}
		}

		private void btnAllSelect_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow row in dgvItems.Rows)
			{
				row.Cells["SelectFg"].Value = true;
			}
		}

		private void dgvItems_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
		{
			if (e.RowIndex == -1) return;

			if (Convert.ToBoolean(dgvItems.Rows[e.RowIndex].Cells["SelectFg"].Value) == true)
			{
				dgvItems.Rows[e.RowIndex].Cells["SelectFg"].Value = false;
			}
			else
			{
				dgvItems.Rows[e.RowIndex].Cells["SelectFg"].Value = true;
				if (IsMultiSelect == false)
				{
					foreach(DataGridViewRow row in dgvItems.Rows)
					{
						if (row.Index == e.RowIndex) continue;
						row.Cells["SelectFg"].Value = false;
					}
				}
			}

		}
	}
}
