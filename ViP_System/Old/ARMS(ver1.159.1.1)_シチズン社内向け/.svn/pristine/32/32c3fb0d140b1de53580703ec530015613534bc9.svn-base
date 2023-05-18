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
	public partial class FrmLotTray : Form
	{
        public class LotTrayViewData
        {
            public bool ChangeFg { get; set; }
            public string TrayNo { get; set; }
            public int? OrderNo { get; set; }
            public string TypeCd { get; set; }
            public string LotNo { get; set; }
            public bool NewFg { get; set; }
            public DateTime LastUpdDt { get; set; }
            public string OldTrayNo { get; set; }
            public string OldLotNo { get; set; }
        }

		public FrmLotTray()
		{
			InitializeComponent();
		}

		private void btnSearch_Click(object sender, EventArgs e)
		{
			if (string.IsNullOrWhiteSpace(txtLotNo.Text.Trim()) && string.IsNullOrWhiteSpace(txtTrayNo.Text.Trim()) && !chkNewFg.Checked)
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

			string trayNo = string.Empty;
			if (string.IsNullOrEmpty(txtTrayNo.Text.Trim()) == false)
			{
				string[] trayNoChars = txtTrayNo.Text.Split(' ');
				if (trayNoChars.Count() >= 2)
				{
					trayNo = trayNoChars[1];
				}
				else
				{
					trayNo = trayNoChars[0];
				}
			}

            List<AsmLot.LotTray> lotTrayDataList = AsmLot.GetRelateTray(trayNo, null, lotNo, chkNewFg.Checked, false);
            List<LotTrayViewData> lotTrayViewDataList = new List<LotTrayViewData>();
            foreach (AsmLot.LotTray lott in lotTrayDataList)
            {
                LotTrayViewData lottrayviewData = new LotTrayViewData();
                lottrayviewData.ChangeFg = false;
                lottrayviewData.TrayNo = lott.TrayNo;
                lottrayviewData.OrderNo = lott.OrderNo;
                lottrayviewData.LotNo = lott.LotNo;
                lottrayviewData.NewFg = lott.NewFg;
                lottrayviewData.LastUpdDt = lott.LastUpdDt;
                lottrayviewData.OldTrayNo = lott.TrayNo;
                lottrayviewData.OldLotNo = lott.LotNo;
                AsmLot lot = AsmLot.GetAsmLot(lott.LotNo);
                if (lot != null)
                {
                    lottrayviewData.TypeCd = lot.TypeCd;
                }

                lotTrayViewDataList.Add(lottrayviewData);
            }

			bsItems.DataSource = lotTrayViewDataList;
		}

		private void btnAdd_Click(object sender, EventArgs e)
		{
            LotTrayViewData lottrayviewData = new LotTrayViewData();
            lottrayviewData.ChangeFg = true;
            lottrayviewData.TrayNo = string.Empty;
            lottrayviewData.OrderNo = 0;
            lottrayviewData.TypeCd = string.Empty;
            lottrayviewData.LotNo = string.Empty;
            lottrayviewData.NewFg = true;
            lottrayviewData.LastUpdDt = System.DateTime.Now;
            lottrayviewData.OldTrayNo = string.Empty;
            lottrayviewData.OldLotNo = string.Empty;

            List<LotTrayViewData> dataList = new List<LotTrayViewData>();

            if (bsItems.DataSource != null)
            {
                dataList = ((List<LotTrayViewData>)bsItems.DataSource).ToList();
            }

            dataList.Add(lottrayviewData);

            bsItems.DataSource = dataList;
		}

		private void btnSave_Click(object sender, EventArgs e)
		{
			dgvItems.EndEdit();

			try
			{
                List<LotTrayViewData> changeList
                    = ((List<LotTrayViewData>)bsItems.DataSource).Where(f => f.ChangeFg).ToList();
				if (changeList.Count == 0)
				{
                    MessageBox.Show(
                            string.Format("「変更欄」の列にチェックがついているレコードがありません。"), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
					return;
				}

                List<LotTrayViewData> baseList
                    = ((List<LotTrayViewData>)bsItems.DataSource).ToList();
                if (baseList.Where(f => f.ChangeFg).ToList().Count == 0)
                {
                    MessageBox.Show(
                            string.Format("「変更欄」の列にチェックがついているレコードがありません。"), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                    return;
                }

                //foreach (LotTrayViewData change in changeList)
                foreach (LotTrayViewData change in baseList)
                {
                    if (!change.ChangeFg)
                    {
                        continue;
                    }

					if (string.IsNullOrEmpty(change.TrayNo))
					{
						MessageBox.Show(
                            string.Format("「トレイ番号」の列が空白の為、登録できません。"), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						continue;
					}

					if (!change.OrderNo.HasValue || change.OrderNo.Value == 0) 
					{
						MessageBox.Show(
							string.Format("「連番」の列が空白または「0」の為、登録できません。"), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
						continue;
					}

                    if (string.IsNullOrEmpty(change.LotNo))
                    {
                        MessageBox.Show(
                            string.Format("「ロット番号」の列が空白の為、登録できません。"), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                        continue;
                    }

                    if (!string.IsNullOrWhiteSpace(change.OldTrayNo) && !string.IsNullOrWhiteSpace(change.OldLotNo))
                    {
                        AsmLot.DissolveTray(change.OldLotNo, change.OldTrayNo, null, false);
                    }

                    AsmLot.UpdateRelateTray(change.LotNo, change.TrayNo, change.OrderNo.Value, change.NewFg, false);

                    change.OldLotNo = change.LotNo;
                    change.OldTrayNo = change.TrayNo;
                    change.ChangeFg = false;

				}

                bsItems.DataSource = baseList;

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

            if (dgvItems.Columns["LotNo"].Index == e.ColumnIndex)
            {
                object lotNo = dgvItems.Rows[e.RowIndex].Cells["LotNo"].Value;
                string slotNo = lotNo != null ? lotNo.ToString() : string.Empty;
                string typeCd = string.Empty;
                if (!string.IsNullOrEmpty(slotNo) && !string.IsNullOrWhiteSpace(slotNo))
                {
                    AsmLot lot = AsmLot.GetAsmLot(slotNo);
                    typeCd = lot != null ? lot.TypeCd : string.Empty;
                }
                dgvItems.Rows[e.RowIndex].Cells["TypeCd"].Value = typeCd;
            }

            ((DataGridViewCheckBoxCell)dgvItems.Rows[e.RowIndex].Cells["ChangeFg"]).Value = true;

            dgvItems.EndEdit();
		}

		private void bindingNavigatorMoveLastItem_Click(object sender, EventArgs e)
		{

		}
	}
}
