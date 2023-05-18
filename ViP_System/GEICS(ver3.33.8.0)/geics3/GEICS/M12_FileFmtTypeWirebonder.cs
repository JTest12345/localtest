using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GEICS
{
    public partial class M12_FileFmtTypeWirebonder : Form
    {
        public M12_FileFmtTypeWirebonder()
        {
            InitializeComponent();
        }
        
        private void M12_FileFmtType_Load(object sender, EventArgs e)
        {
            toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
            toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;

            cmbType.DataSource = ConnectQCIL.GetPLMTypeCD(true);
            cmbType.Text = string.Empty;

            bsItems.DataSource = new List<FileFmtTypeInfo>();
        }

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            List<FileFmtTypeInfo> typeList = Database.MachineLogFormat.WirebonderFile.GetTypeClassData(cmbType.Text);
            bsItems.DataSource = typeList;
        }

        #region データグリッド

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
        /// 新規追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAdd_Click(object sender, EventArgs e)
        {
            FileFmtTypeInfo newItem = new FileFmtTypeInfo();
            newItem.ChangeFG = true;
            newItem.UpdUserCD = Constant.EmployeeInfo.EmployeeCD;
            newItem.LastUpdDT = DateTime.Now;
            bsItems.Add(newItem);
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
                List<FileFmtTypeInfo> changeList = ((List<FileFmtTypeInfo>)bsItems.DataSource).Where(p => p.ChangeFG).ToList();
                if (changeList.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                foreach(FileFmtTypeInfo change in changeList)
                {
                    if (string.IsNullOrEmpty(change.OldTypeCD)) change.OldTypeCD = change.TypeCD;

                    Database.MachineLogFormat.WirebonderFile.InsertUpdateTypeClass(change);
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

        #endregion

        /// <summary>
        /// 貼り付け
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPaste_Click(object sender, EventArgs e)
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

                FileFmtTypeInfo newItem = new FileFmtTypeInfo();
                newItem.ChangeFG = true;
                newItem.TypeCD = clipChars[0].ToString().Trim();

                int fileFmtNo;
                if (!int.TryParse(clipChars[1].ToString().Trim(), out fileFmtNo))
                {
                    throw new Exception(string.Format("紐付け番号には数値を入力して下さい。値:{0}", clipChars[1]));
                }
                newItem.FileFmtNO = fileFmtNo;

                int frameNo;
                if (!int.TryParse(clipChars[2].ToString().Trim(), out frameNo))
                {
                    throw new Exception(string.Format("基板番号には数値を入力して下さい。値:{0}", clipChars[2]));
                }
                newItem.FrameNO= frameNo;

                int delFg = 0;
                if (int.TryParse(clipChars[3].ToString().Trim(), out delFg))
                {
                    newItem.DelFG = Convert.ToBoolean(delFg);
                }
                else
                {
                    newItem.DelFG = Convert.ToBoolean(clipChars[3].ToString().Trim());
                }
                                
                newItem.UpdUserCD = clipChars[4].ToString().Trim();

                DateTime lastUpdDt;
                if (!DateTime.TryParse(clipChars[5].ToString().Trim(), out lastUpdDt))
                {
                    throw new Exception(string.Format("更新日時には日付型を入力して下さい。値:{0}", clipChars[5]));
                }
                newItem.LastUpdDT = lastUpdDt;

                bsItems.Add(newItem);
            }
        }
    }
}
