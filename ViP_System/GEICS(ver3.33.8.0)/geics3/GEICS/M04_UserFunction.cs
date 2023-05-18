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
    public partial class M04_UserFunction : Form
    {
        public M04_UserFunction()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmUserFunction_Load(object sender, EventArgs e)
        {
            toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
            toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;
        }

        /// <summary>
        /// 社員検索ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string empCD = txtEmployeeCD.Text.Trim();
            string empNM = txtEmployeeNM.Text;

            try
            {
                if (empCD == "" && empNM == "")
                {
                    throw new Exception(Constant.MessageInfo.Message_66);
                }

                dsUserFunction.dtEmployee.Clear();

                //社員情報を取得
                NameValueCollection nvc = ConnectNASCA.GetEmplpyee(empCD, empNM);
                for (int i = 0; i < nvc.Count; i++)
                {
                    DataRow dr = dsUserFunction.dtEmployee.NewRow();
                    dr[dsUserFunction.dtEmployee.Employee_CDColumn] = nvc.GetKey(i);
                    dr[dsUserFunction.dtEmployee.Employee_NMColumn] = nvc.Get(i);
                    dsUserFunction.dtEmployee.Rows.Add(dr);
                }
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 社員選択時(ダブルクリック)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvEmployee_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                tvHoldFunction.Nodes.Clear();
                tvFunction.Nodes.Clear();

                //保持権限を表示
                List<UserFunctionInfo> ufList = ConnectQCIL.GetUserFunction(Convert.ToString(dgvEmployee.SelectedRows[0].Cells["Employee_CD"].Value));
                foreach (UserFunctionInfo ufInfo in ufList) 
                {
                    tvHoldFunction.Nodes.Add(ufInfo.FunctionCD, ufInfo.FunctionNM); 
                }

                //権限一覧を表示
                NameValueCollection nvc = ConnectQCIL.GetFunction();
                for (int i = 0; i < nvc.Count; i++)
                {
                    if (ufList.Exists(uf => uf.FunctionCD == nvc.GetKey(i)))
                    {
                        //保持している権限の場合次へ
                        continue;
                    }

                    tvFunction.Nodes.Add(nvc.GetKey(i), nvc.Get(i));
                }
            }
            catch(Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 社員選択変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvEmployee_SelectionChanged(object sender, EventArgs e)
        {
            tvFunction.Nodes.Clear();
            btnAdd.Enabled = false;
            tvHoldFunction.Nodes.Clear();
            btnDelete.Enabled = false;
        }

        /// <summary>
        /// 権限一覧選択時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvFunction_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDelete.Enabled = false;
            btnAdd.Enabled = true;
        }

        /// <summary>
        /// 保持権限選択時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tvHoldFunction_AfterSelect(object sender, TreeViewEventArgs e)
        {
            btnDelete.Enabled = true;
            btnAdd.Enabled = false;
        }

        /// <summary>
        /// 権限追加ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnAdd_Click(object sender, EventArgs e)
        {
            if (tvFunction.SelectedNode == null)
            {
                return;
            }

            if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_69, "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }
            try
            {
                string empCD = Convert.ToString(dgvEmployee.SelectedRows[0].Cells["Employee_CD"].Value).Trim();
                string functionCD = tvFunction.SelectedNode.Name;

                //追加
                ConnectQCIL.InsertUserFunction(empCD, functionCD, Constant.EmployeeInfo.EmployeeCD);

                //保持権限再取得
                dgvEmployee_CellDoubleClick(null, null);
            }
            catch (Exception err) 
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 保持権限削除ボタン
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (tvHoldFunction.SelectedNode == null) 
            {
                return;
            }

            if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_70, "Info", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
            {
                return;
            }
            try
            {
                string empCD = Convert.ToString(dgvEmployee.SelectedRows[0].Cells["Employee_CD"].Value).Trim();
                string functionCD = tvHoldFunction.SelectedNode.Name;

                //削除
                ConnectQCIL.DeleteUserFunction(empCD, functionCD);

                //保持権限再取得
                dgvEmployee_CellDoubleClick(null, null);
            }
            catch (Exception err)
            {
                MessageBox.Show(err.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        
    }
}
