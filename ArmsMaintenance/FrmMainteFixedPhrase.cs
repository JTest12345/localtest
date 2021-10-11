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
	public partial class FrmMainteFixedPhrase : Form
	{
		public FrmMainteFixedPhrase()
		{
			InitializeComponent();
		}

		private void FrmMainteFixedPhrase_Load(object sender, EventArgs e)
		{
			try
            {
                List<FixedPhrase> dataList = FixedPhrase.GetDataMaintenance(true);
                bsItems.DataSource = dataList;
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

		private void btnSave_Click(object sender, EventArgs e)
		{
			grdItems.EndEdit();

            try
            {
                List<FixedPhrase> changeList
                    = ((List<FixedPhrase>)bsItems.DataSource).Where(f => f.ChangeFg).ToList();
                if (changeList.Count == 0)
                {
					return;
                }

				foreach (FixedPhrase change in changeList)
                {
					if (string.IsNullOrEmpty(change.OldText)) change.OldText = change.Text;

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

					FixedPhrase.InsertUpdate(change);
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

		private void btnAdd_Click(object sender, EventArgs e)
		{
			FixedPhrase newItem = new FixedPhrase();
			newItem.TextKb = "DataMaintenance";
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
	}
}
