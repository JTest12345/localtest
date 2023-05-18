using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Collections.Specialized;

namespace GEICS
{
    public partial class M05_StandardText : Form
    {
        public M05_StandardText()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmStandardText_Load(object sender, EventArgs e)
        {
            toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
            toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;

            try
            {
                //装置分類取得
                NameValueCollection nvc = ConnectQCIL.GetAssets();

                //コンボボックス装置分類設定
                cmbAssets.SourceNVC = nvc;

                //グリッドコンボボックス装置分類設定
                for (int i = 0; i < nvc.Count; i++)
                {
                    ((DataGridViewComboBoxColumn)dgvStandardText.Columns["Assets_NM"]).Items.Add(nvc.Get(i));
                }
            }
            catch (Exception err) 
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);           
            }
        }

        /// <summary>
        /// 検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            dsStandardText.dtFNM.Clear();
            string assetsNM = cmbAssets.Text.Trim();

            try
            {
                //定型文取得
                List<FNMInfo> fnmList = ConnectQCIL.GetFNM(assetsNM);

                foreach (FNMInfo fnmInfo in fnmList)
                {
                    DataRow dr = dsStandardText.dtFNM.NewRow();
                    dr[dsStandardText.dtFNM.Assets_NMColumn] = fnmInfo.AssetsNM;
                    dr[dsStandardText.dtFNM.Fixed_NOColumn] = fnmInfo.FixedNO;
                    dr[dsStandardText.dtFNM.Fixed_NMColumn] = fnmInfo.FixedNM;
                    dr[dsStandardText.dtFNM.Del_FGColumn] = fnmInfo.DelFG;
                    dr[dsStandardText.dtFNM.UpdUser_CDColumn] = fnmInfo.UpdUserCD;
                    dr[dsStandardText.dtFNM.LastUpd_DTColumn] = fnmInfo.LastUpdDT;
                    dsStandardText.dtFNM.Rows.Add(dr);
                }

                dsStandardText.dtFNM.AcceptChanges();
            }
            catch (Exception err) 
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);           
            }
        }

        /// <summary>
        /// 新規追加ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolbtnAdd_Click(object sender, EventArgs e)
        {
            int newRowIndex = dgvStandardText.Rows.GetLastRow(DataGridViewElementStates.Displayed);
            dgvStandardText.Rows[newRowIndex].Cells["Assets_NM"].ReadOnly = false;

			dgvStandardText.Rows[newRowIndex].Cells["updUserCDDataGridViewTextBoxColumn"].Value = Constant.EmployeeInfo.EmployeeCD;
			dgvStandardText.Rows[newRowIndex].Cells["lastUpdDTDataGridViewTextBoxColumn"].Value = DateTime.Now;
        }

        /// <summary>
        /// 保存ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolbtnPreserve_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_25, "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }

            dgvStandardText.EndEdit();
            dsStandardTextBindingSource.EndEdit();

            try
            {
                //追加
                dsStandardText.dtFNMDataTable dt = (dsStandardText.dtFNMDataTable)dsStandardText.dtFNM.GetChanges(DataRowState.Added);
                if (dt != null && dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        //新規FixedNOを取得
                        int fixedNO = ConnectQCIL.GetFNMFixedNOMAX(Convert.ToString(dr[dt.Assets_NMColumn]));

                        ConnectQCIL.InsertFNM(Convert.ToString(dr[dt.Assets_NMColumn]), fixedNO,
                            Convert.ToString(dr[dt.Fixed_NMColumn]), Constant.EmployeeInfo.EmployeeCD);
                    }
                }

                //変更
                dt = (dsStandardText.dtFNMDataTable)dsStandardText.dtFNM.GetChanges(DataRowState.Modified);
                if (dt != null && dt.Rows.Count != 0)
                {
                    foreach (DataRow dr in dt.Rows)
                    {
                        ConnectQCIL.UpdateFNM(Convert.ToString(dr[dt.Assets_NMColumn]), Convert.ToInt32(dr[dt.Fixed_NOColumn]),
                            Convert.ToString(dr[dt.Fixed_NMColumn]), Constant.EmployeeInfo.EmployeeCD, Convert.ToBoolean(dr[dt.Del_FGColumn]));
                    }
                }

                dsStandardText.dtFNM.AcceptChanges();
                MessageBox.Show(Constant.MessageInfo.Message_75, "Info", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception err) 
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
