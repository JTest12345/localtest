using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GEICS.Database;

namespace GEICS
{
    public partial class M16_ParameterSummary : Form
    {
        /// <summary>
        /// 製品型番選択リスト
        /// </summary>
        private List<string> typeSelectList { get; set; }

        /// <summary>
        /// 管理項目選択リスト
        /// </summary>
        private List<int> qcParamSelectList { get; set; }

        public M16_ParameterSummary()
        {
            InitializeComponent();

            typeSelectList = new List<string>();
            qcParamSelectList = new List<int>();
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                if (!string.IsNullOrEmpty(txtQcParamNO.Text))
                {
                    qcParamSelectList.Clear();
                    qcParamSelectList.Add(int.Parse(txtQcParamNO.Text));
                }

                if (cmbType.Enabled && cmbType.Text == string.Empty)
                {
                    typeSelectList.Clear();

                }

                if (cmbType.Text != string.Empty)
                {
                    typeSelectList.Clear();
                    typeSelectList.Add(cmbType.Text);
                }

                bsItems.DataSource = PrmSummary.GetData(qcParamSelectList, typeSelectList, true);
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

        private void M16_ParameterSummary_Load(object sender, EventArgs e)
        {
            cmbType.DataSource = ConnectQCIL.GetPLMTypeCD(true);
            cmbType.Text = string.Empty;

        }

        /// <summary>
        /// 製品型番複数選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTypeMulti_Click(object sender, EventArgs e)
        {
            List<string> typeList = ConnectQCIL.GetPLMTypeCD(true);

            M08_MultiSelect formMultiSelect = new M08_MultiSelect(typeList, typeSelectList);
            formMultiSelect.ShowDialog();

            typeSelectList = formMultiSelect.SelectItemList;
            if (typeSelectList.Count == 0)
            {
                btnTypeMulti.ImageIndex = 0;
                cmbType.Enabled = true;
            }
            else
            {
                btnTypeMulti.ImageIndex = 1;
                cmbType.Enabled = false;
                cmbType.Text = string.Empty;
            }
        }

        /// <summary>
        /// 管理検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQcParamSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (M03_Parameter MasterParameter = new M03_Parameter(false, true, null))
                {
                    MasterParameter.ShowDialog();

                    if (MasterParameter.SelectParamList != null)
                    {
                        if (MasterParameter.SelectParamList.Count == 1)
                        {
                            txtQcParamNO.Text = MasterParameter.SelectParamList.Single().QcParamNO.ToString();
                            txtParameterNM.Text = MasterParameter.SelectParamList.Single().ParameterNM;
                        }

                        this.qcParamSelectList = MasterParameter.SelectParamList.Select(p => p.QcParamNO).ToList();
                    }
                }
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
        /// 管理項目複数選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnQcParamMulti_Click(object sender, EventArgs e)
        {
            M08_MultiSelect formMultiSelect = new M08_MultiSelect(qcParamSelectList.ConvertAll(q => q.ToString()), qcParamSelectList.ConvertAll(q => q.ToString()));
            formMultiSelect.ShowDialog();

            qcParamSelectList = formMultiSelect.SelectItemList.ConvertAll(q => int.Parse(q));
            if (qcParamSelectList.Count == 0)
            {
                btnQcParamMulti.ImageIndex = 0;
                txtQcParamNO.Enabled = true;
                txtParameterNM.Enabled = true;
            }
            else
            {
                btnQcParamMulti.ImageIndex = 1;
                txtQcParamNO.Enabled = true;
                txtParameterNM.Enabled = true;
                txtQcParamNO.Text = string.Empty;
                txtParameterNM.Text = string.Empty;
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSave_Click(object sender, EventArgs e)
        {
            dgvItems.EndEdit();

            try
            {
                List<PrmSummary> changeList
                    = ((List<PrmSummary>)bsItems.DataSource).Where(p => p.ChangeFG).ToList();
                if (changeList.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                foreach (PrmSummary change in changeList)
                {
                    if (string.IsNullOrEmpty(change.OldTypeCD)) change.OldTypeCD = change.TypeCD;
                    if (change.OldQcParamNO == 0) change.OldQcParamNO = change.QcParamNO;
                    PrmSummary.InsertUpdate(change);
                }

                MessageBox.Show(Constant.MessageInfo.Message_75, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
        /// 追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            Database.PrmSummary newItem = new Database.PrmSummary();
            newItem.ChangeFG = true;
            newItem.UpdUserCD = Constant.EmployeeInfo.EmployeeCD;
            newItem.LastUpdDT = DateTime.Now;
            bsItems.Add(newItem);
        }

        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            if (dgvItems.Columns["ChangeFG"].Index == e.ColumnIndex)
            {
                return;
            }

            ((DataGridViewCheckBoxCell)dgvItems.Rows[e.RowIndex].Cells["ChangeFG"]).Value = true;

            dgvItems.EndEdit();
        }

        /// <summary>
        /// 貼り付け
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPaste_Click(object sender, EventArgs e)
        {
            try
            {
                string[] clipDatas = Clipboard.GetText().Split(new string[] { Environment.NewLine }, StringSplitOptions.None);
                if (clipDatas.Count() == 0)
                {
                    return;
                }

                foreach (string clipData in clipDatas)
                {
                    if (string.IsNullOrEmpty(clipData)) { continue; }

                    string[] clipChars = clipData.Split('\t');

                    if (clipChars.Count() != (dgvItems.ColumnCount-1))
                    {
                        throw new ApplicationException("対応していないデータ(列数が違う等)を貼り付けようとしています。");
                    }

                    Database.PrmSummary newItem = new Database.PrmSummary();
                    newItem.ChangeFG = true;

                    newItem.TypeCD = clipChars[0].ToString().Trim();

                    int qcParamNo;
                    if (!int.TryParse(clipChars[1].ToString().Trim(), out qcParamNo))
                    {
                        throw new ApplicationException(string.Format("管理Noには数値を入力して下さい。値:{0}", clipChars[1]));
                    }
                    newItem.QcParamNO = qcParamNo;

                    newItem.SummaryKB = clipChars[2].ToString().Trim();

                    int anyRowCt;
                    if (!int.TryParse(clipChars[3].ToString().Trim(), out anyRowCt))
                    {
                        throw new ApplicationException(string.Format("n数には数値を入力して下さい。値:{0}", clipChars[3]));
                    }
                    newItem.AnyRowCT = anyRowCt;

                    newItem.DelFG = Convert.ToBoolean(int.Parse(clipChars[4].ToString().Trim()));
                    newItem.UpdUserCD = clipChars[5].ToString().Trim();

                    DateTime lastUpdDt;
                    if (!DateTime.TryParse(clipChars[6].ToString().Trim(), out lastUpdDt))
                    {
                        throw new ApplicationException(string.Format("更新日時には日付型を入力して下さい。値:{0}", clipChars[6]));
                    }
                    newItem.LastUpdDT = lastUpdDt;

                    bsItems.Add(newItem);
                }
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

        private void dgvItems_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dgvItems.Columns["QcParamNO"].Index == e.ColumnIndex)
            {
                using (M03_Parameter MasterParameter = new M03_Parameter(false, false, null))
                {
                    MasterParameter.ShowDialog();

                    if (MasterParameter.SelectParamList != null)
                    {
                        dgvItems.Rows[e.RowIndex].Cells[e.ColumnIndex].Value = MasterParameter.SelectParamList.Single().QcParamNO;
                    }
                }
            }
        }
    }
}
