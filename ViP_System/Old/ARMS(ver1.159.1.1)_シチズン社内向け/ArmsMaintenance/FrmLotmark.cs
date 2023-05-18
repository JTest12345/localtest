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
	public partial class FrmLotmark : Form
	{
		public FrmLotmark()
		{
			InitializeComponent();
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrEmpty(txtLotNo.Text.Trim()) && string.IsNullOrEmpty(txtWorkMagazineNo.Text.Trim()))
			{
				MessageBox.Show("検索条件を最低一つは指定して下さい。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				return;
			}

			string lotNo = string.Empty;
			if (string.IsNullOrEmpty(txtLotNo.Text.Trim()) == false)
			{
				string[] lotNoChars = txtLotNo.Text.Split(' ');
				if (lotNoChars.Count() >= 2)
				{
					lotNo = lotNoChars[1];
				}
				else
				{
					lotNo = lotNoChars[0];
				}
			}

			string magNo = string.Empty;
			if (string.IsNullOrEmpty(txtWorkMagazineNo.Text.Trim()) == false)
			{
				string[] magNoChars = txtWorkMagazineNo.Text.Split(' ');
				if (magNoChars.Count() >= 2)
				{
					magNo = magNoChars[1];
				}
				else
				{
					magNo = magNoChars[0];
				}
			}

			List<LotMarkData> lotmarkDataList = LotMarkData.Search(lotNo, magNo);
			bsItems.DataSource = lotmarkDataList;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
			string lotNo = string.Empty;
			if (dgvItems.Rows.Count != 0) 
			{
				lotNo = dgvItems.Rows[0].Cells["LotNo"].Value.ToString().Trim();
			}

			LotMarkData lotmarkData = new LotMarkData();
			lotmarkData.ChangeFg = true;
			lotmarkData.LotNo = lotNo;
			lotmarkData.StockerNo = 0;
			lotmarkData.WorkDt = System.DateTime.Now;
			lotmarkData.MarkData = 0;
			lotmarkData.ManualFg = true;
			bsItems.Add(lotmarkData);
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			dgvItems.EndEdit();

			try
			{
				List<LotMarkData> changeList
					= ((List<LotMarkData>)bsItems.DataSource).Where(f => f.ChangeFg).ToList();
				if (changeList.Count == 0)
				{
					return;
				}

				foreach (LotMarkData change in changeList)
				{
					if (LotMarkData.HasMarkData(change.LotNo, change.Row) == true)
					{
						MessageBox.Show(
							string.Format("既に同一データが登録されている為、登録できません。 ロット番号:{0} マガジン段数:{1}", change.LotNo, change.Row), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						continue;
					}

					if (LotMarkData.HasMarkData(change.LotNo, change.MarkData) == true) 
					{
						MessageBox.Show(
							string.Format("既に同一データが登録されている為、登録できません。 ロット番号:{0} 印字文字:{1}", change.LotNo, change.MarkData), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						continue;
					}

					LotMarkData.InsertUpdate(change);
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

		private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
		{
			if (e.RowIndex == -1)
			{
				return;
			}

			if (dgvItems.Columns["ChangeFg"].Index == e.ColumnIndex)
			{
				return;
			}

			((DataGridViewCheckBoxCell)dgvItems.Rows[e.RowIndex].Cells["ChangeFg"]).Value = true;
			((DataGridViewCheckBoxCell)dgvItems.Rows[e.RowIndex].Cells["ManualFg"]).Value = true;

			dgvItems.EndEdit();
		}

		private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
		{

		}
	}
}
