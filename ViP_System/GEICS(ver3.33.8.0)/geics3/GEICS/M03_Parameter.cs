using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLCommonLib.Commons;
using GEICS.Database;
namespace GEICS
{
    public partial class M03_Parameter : Form
    {
        /// <summary>
        /// 編集モードフラグ
        /// </summary>
        private bool IsEdit = false;

        /// <summary>
        /// 複数選択フラグ
        /// </summary>
        private bool IsMultiSelect = false;

        /// <summary>
        /// 樹脂Gr管理フラグ
        /// </summary>
        private bool? IsResinGroupManageCondition;

        public M03_Parameter(bool isEdit, bool isMultiSelect, bool? isResinGroupManageCondition)
        {
            InitializeComponent();

            this.IsEdit = isEdit;
            this.IsMultiSelect = isMultiSelect;
            this.IsResinGroupManageCondition = isResinGroupManageCondition;
        }
        public M03_Parameter(bool isEdit)
        {
            InitializeComponent();

            this.IsEdit = isEdit;
            this.IsMultiSelect = false;
        }
        public List<Prm> SelectParamList { get; set; }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
				string modelNM = txtModelNM.Text.Trim();
                string parameterNM = txtParameterNM.Text.Trim();

                int qcParamNO = int.MinValue;
                if (!string.IsNullOrEmpty(txtQcParamNO.Text))
                {
                    qcParamNO = Convert.ToInt32(txtQcParamNO.Text);
                }

                bool? progMatFg = null;
                if (this.rdoOnlyNormal.Checked)
                {
                    progMatFg = false;
                }
                else if (this.rdoOnlyProgramNm.Checked)
                {
                    progMatFg = true;
                }

                List<Prm> prmList = Prm.GetData(modelNM, parameterNM, qcParamNO, IsResinGroupManageCondition, progMatFg);

                // NASCAから作業名を取得
                List<WorkInfo> workList = ConnectNASCA.GetNascaWork(null, null, true);
                foreach(var item in prmList)
                {
                    if (string.IsNullOrEmpty(item.CauseWorkCD)) continue;
                    var work = workList.FirstOrDefault(l => l.WorkCD == item.CauseWorkCD);
                    if (work == null) continue;
                    item.CauseWorkNM = work.WorkNMWithCD;
                }

                bsParameter.DataSource = prmList.ToSortableBindingList();
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch(Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 選択/未選択 切替
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvItems_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            try
            {
                if (IsEdit)
                {
                    return;
                }

                if (!IsMultiSelect)
                { 
                    SortableBindingList<Prm> paramList = (SortableBindingList<Prm>)bsParameter.DataSource;
                    paramList = paramList.Where(p => p.IsCheck).ToSortableBindingList();
                    foreach (Prm paramInfo in paramList)
                    {
                        paramInfo.IsCheck = false;
                    }
                    dgvItems.Refresh();
                }

                DataGridViewCheckBoxCell targetCell = ((DataGridViewCheckBoxCell)dgvItems.Rows[e.RowIndex].Cells["CheckFG"]);
                if (Convert.ToBoolean(targetCell.Value))
                {
                    targetCell.Value = false;
                }
                else
                {
                    targetCell.Value = true;
                }
                dgvItems.EndEdit();
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// データを戻す
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDataReturn_Click(object sender, EventArgs e)
        {
            try
            {
                if (dgvItems.Rows.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_9);
                }

                SortableBindingList<Prm> paramList = (SortableBindingList<Prm>)bsParameter.DataSource;
                paramList = paramList.Where(p => p.IsCheck).ToSortableBindingList();
                if (paramList.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_9);
                }

                this.SelectParamList = paramList.ToList();

                this.Close();
                this.Dispose();
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

        /// <summary>
        /// 管理NOを数値入力のみ許可
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQcParamNO_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (char.IsControl(e.KeyChar)) 
            {
                return;
            }
            if (!char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        /// <summary>
        /// FormLoad
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M03_Parameter_Load(object sender, EventArgs e)
        {
            if (IsEdit)
            {
                bnParameterEdit.Visible = true;
                bnParameter.Visible = false;
                dgvItems.ReadOnly = false;
            }
            else 
            {
                bnParameterEdit.Visible = false;
                bnParameter.Visible = true;
				CauseAssetsNM.Visible = false;
                dgvItems.ReadOnly = true;
            }

            rdoOnlyNormal.Checked = false;
            rdoOnlyProgramNm.Checked = false;
            rdoBoth.Checked = true;

			dgvItems.MultiSelect = IsMultiSelect;
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != MessageBox.Show("保存してよろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }

            try
            {
                dgvItems.EndEdit();

                List<Prm> paramList = getCheckData();

				////////////////////////////////////////////////////////////////////////
				//TmPRM登録時に設備指定必須フラグとTmPLMの設備指定状況をチェックして不整合が無いかを確認する処理
				//なかなかややこしいので後回し

				//List<string> paramNoList = (List<string>)paramList.Select(p => p.QcParamNO.ToString()).ToList();

				//List<string> modelNmList = paramList.Select(p => p.ModelNM).Distinct().ToList();

				//foreach(string modelNM in modelNmList)
				//{
				//    ConnectQCIL.GetPLMTypeCD();
				//}

				//ConnectQCIL.GetPLMData(

				//PlmInfo.CheckEquipManageParam();
				////////////////////////////////////////////////////////////////////////


                Prm.InsertUpdate(paramList);

                MessageBox.Show("保存完了しました。", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message + "\r\n" + err.StackTrace, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private List<Prm> getCheckData()
        {
            return ((SortableBindingList<Prm>)bsParameter.List).Where(p => p.IsCheck).ToList();
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1) { return; }
            if (e.ColumnIndex == dgvItems.Columns["CheckFG"].Index) { return; }

            dgvItems.Rows[e.RowIndex].Cells["CheckFG"].Value = true;
        }

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == dgvItems.Columns["CauseAssetsNM"].Index) 
            {
                M11_General formGene = new M11_General(General.GROUPCD_ASSETS);
                formGene.ShowDialog();

                if (formGene.SelectGenerals.Count == 0)
                {
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = string.Empty;
                }
                else 
                {
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = string.Join(",", formGene.SelectGenerals.Select(g => g.GeneralNM).ToArray());
                }
            }
            else if (e.ColumnIndex == dgvItems.Columns["CauseWorkNM"].Index)
            {
                var frm = new F82_SelectNascaWork();
                frm.ShowDialog();

                if (frm.SelectWork != null)
                {
                    dgvItems.Rows[e.RowIndex].Cells["CauseWorkCD"].Value = frm.SelectWork.WorkCD;
                    dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = frm.SelectWork.WorkNMWithCD;
                }
            }
            else if (e.ColumnIndex == dgvItems.Columns["LimitOverWhenFlowNotPossibleFG"].Index ||
                    e.ColumnIndex == dgvItems.Columns["LimitOverWhenMachineNotPossibleFG"].Index)
            {
                DataGridViewCheckBoxCell targetCell = ((DataGridViewCheckBoxCell)this.dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex]);
                if (Convert.ToBoolean(targetCell.Value))
                {
                    targetCell.Value = false;
                }
                else
                {
                    targetCell.Value = true;
                }
            }
        }

		//検索無しで貼り付け時に例外発生するので対策する(2014/6/6 n.yoshimoto)
		private void toolPaste_Click(object sender, EventArgs e)
		{
			if (bsParameter.Count == 0)
			{
				return;
			}

			string[] clipDatas = Clipboard.GetText().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
			if (clipDatas.Count() == 0)
			{
				return;
			}

			SortableBindingList<Prm> multiSelectList = (SortableBindingList<Prm>)bsParameter.DataSource;

			foreach (string clipData in clipDatas)
			{
				if (string.IsNullOrEmpty(clipData)) { continue; }
				int qcParamNO;
				if (int.TryParse(clipData, out qcParamNO))
				{
					if (multiSelectList.ToList().Exists(m => m.QcParamNO == qcParamNO))
					{
						Prm targetRecord = multiSelectList.First(m => m.QcParamNO == qcParamNO);

						if (targetRecord != null)
						{
							targetRecord.IsCheck = true;
						}
					}
				}
			}

			bsParameter.DataSource = multiSelectList.OrderByDescending(m => m.IsCheck).ToSortableBindingList();
		}

		private void toolAllSelect_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow dr in dgvItems.Rows)
			{
				dr.Cells["CheckFG"].Value = true;
			}
		}

		private void toolAllCancel_Click(object sender, EventArgs e)
		{
			foreach (DataGridViewRow dr in dgvItems.Rows)
			{
				dr.Cells["CheckFG"].Value = false;
			}
		}
    }
}
