using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ArmsApi.Model;

namespace ArmsMaintenance
{
	public partial class FrmCutLabelSetting : Form
	{
		public FrmCutLabelSetting()
		{
			InitializeComponent();
		}

		private int defaultMachineNo = 99999;
		private string defaultMachineName = "<規定値>";

		private void FrmCutLabelSetting_Load(object sender, EventArgs e)
		{
			List<MachineInfo> cutMachines
				= MachineInfo.GetMachineList(false).Where(m => m.ClassName.Contains("カット")).ToList();

            cutMachines.AddRange(MachineInfo.GetMachineList(false).Where(m => m.ClassName.Contains("ダイサー")).ToList());

			MachineInfo defaultMachine = new MachineInfo();
			defaultMachine.MacNo = defaultMachineNo;
			defaultMachine.MachineName = defaultMachineName;
			cutMachines.Insert(0, defaultMachine);

			DataGridViewComboBoxColumn macColumn = ((DataGridViewComboBoxColumn)grdItems.Columns["Machine"]);
			macColumn.DataSource = cutMachines;
			macColumn.ValueMember = "MacNo";
			macColumn.DisplayMember = "MachineName";

			bsItems.DataSource = CutLabelFile.GetAllMachineSetting();
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			CutLabelFile newItem = new CutLabelFile();
			newItem.MacNo = ((MachineInfo)((DataGridViewComboBoxColumn)grdItems.Columns["Machine"]).Items[0]).MacNo;
			newItem.ChangeFg = true;
			newItem.LastUpdDt = DateTime.Now;
			bsItems.Add(newItem);
		}

		private void grdItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex == -1)
			{
				return;
			}

			if (grdItems.Columns["ChangeFg"].Index == e.ColumnIndex)
			{
				return;
			}

			((DataGridViewCheckBoxCell)grdItems.Rows[e.RowIndex].Cells["ChangeFg"]).Value = true;

			grdItems.EndEdit();

		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			grdItems.EndEdit();

			try
			{
				List<CutLabelFile> changeList
					= ((List<CutLabelFile>)bsItems.DataSource).Where(f => f.ChangeFg).ToList();
				if (changeList.Count == 0)
				{
					return;
				}

				foreach (CutLabelFile change in changeList)
				{
					if (change.OldMacNo == 0) change.OldMacNo = change.MacNo;

					if (change.MacNo == this.defaultMachineNo)
					{
						change.IsDefault = true;
					}

					if (string.IsNullOrEmpty(change.UpdUserCd))
					{
						throw new ApplicationException("更新者CDで空白の行があります。");
					}

                    int num;
                    if (!int.TryParse(change.UpdUserCd, out num))
                    {
                        throw new ApplicationException("更新者CDは数字を入力してください。");
                    }
                    change.UpdUserCd = num.ToString();

					CutLabelFile.InsertUpdate(change);
				}

				MessageBox.Show("保存完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (ApplicationException err)
			{
				MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
			}
			catch (Exception err)
			{
				MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}

        }
	}
}
