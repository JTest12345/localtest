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
    public partial class M13_FileFmtWirebonder : Form
    {
        /// <summary>
        /// 装置型式選択リスト
        /// </summary>
        private List<string> ModelSelectList { get; set; }

        /// <summary>
        /// 管理NO選択リスト
        /// </summary>
        private List<string> QcParamSelectList { get; set; }

        /// <summary>
        /// ファイル紐付番号選択リスト
        /// </summary>
        private List<string> FileFmtSelectList { get; set; }

        /// <summary>
        /// 検索文字選択リスト
        /// </summary>
        private List<string> SearchNmSelectList { get; set; }

        /// <summary>
        /// 最後に並び替えした列No
        /// </summary>
        private int OldSortColumnIndex { get; set; }

        /// <summary>
        /// 直前に行った並び替え方向 (1：昇順, -1：降順)
        /// </summary>
        private bool AscFg { get; set; }


        public M13_FileFmtWirebonder()
        {
            InitializeComponent();

            ModelSelectList = new List<string>();
            QcParamSelectList = new List<string>();
            FileFmtSelectList = new List<string>();
            SearchNmSelectList = new List<string>();

            OldSortColumnIndex = -1;
            AscFg = false;
        }

        private void M13_FileFmtWirebond_Load(object sender, EventArgs e)
        {
            toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
            toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;

            bsItems.DataSource = new List<Database.MachineLogFormat.WirebonderFile>();
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
                int? qcParamNo = null;
                if (!string.IsNullOrEmpty(txtQcParamNO.Text))
                {
                    int result;
                    if (!int.TryParse(txtQcParamNO.Text, out result))
                    {
                        throw new ApplicationException("管理Noには数値を入力して下さい。");
                    }
                    qcParamNo = result;
                }

                int? fileFmtNo = null;
                if (txtFileFmt.Enabled && !string.IsNullOrEmpty(txtFileFmt.Text.Trim()))
                {
                    int result;
                    if (!int.TryParse(txtFileFmt.Text, out result))
                    {
                        throw new ApplicationException("紐付け番号には数値を入力して下さい。");
                    }
                    fileFmtNo = result;
                }

                List<string> cModelSelectList = new List<string>(ModelSelectList);

                List<string> cQcParamSelectList = new List<string>(QcParamSelectList);

                List<string> cFileFmtSelectList = new List<string>(FileFmtSelectList);

                List<string> cSearchNmSelectList = new List<string>(SearchNmSelectList);


                // テキストボックスが有効 且つ 文字が入力されている場合、検索対象に含む
                if (txtModelNM.Enabled && !string.IsNullOrEmpty(txtModelNM.Text.Trim()))
                {
                    cModelSelectList.Add(txtModelNM.Text.Trim());
                }
                if (txtQcParamNO.Enabled && qcParamNo != null)
                {
                    cQcParamSelectList.Add(qcParamNo.ToString());
                }
                if (txtFileFmt.Enabled && fileFmtNo != null)
                {
                    cFileFmtSelectList.Add(fileFmtNo.ToString());
                }
                if (txtSearchNm.Enabled && !string.IsNullOrEmpty(txtSearchNm.Text.Trim()))
                {
                    cSearchNmSelectList.Add(txtSearchNm.Text.Trim());
                }

                List<Database.MachineLogFormat.WirebonderFile> formatList
                    = Database.MachineLogFormat.WirebonderFile.GetData(cQcParamSelectList, cModelSelectList, null, cFileFmtSelectList, cSearchNmSelectList);

                //List<Database.MachineLogFormat.WirebonderFile> formatList 
                //    = Database.MachineLogFormat.WirebonderFile.GetData(qcParamNo, txtModelNM.Text);
                bsItems.DataSource = formatList;

                // 最後に「一括削除+保存」ボタンを無効にする (復旧する場合は、画面を開き直す)
                this.btnAllDelete_and_Save.Enabled = false;
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

        private void btnQcParamSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (M03_Parameter MasterParameter = new M03_Parameter(false, false, null))
                {
                    MasterParameter.ShowDialog();

                    if (MasterParameter.SelectParamList != null)
                    {
                        txtQcParamNO.Text = MasterParameter.SelectParamList.Single().QcParamNO.ToString();
                        txtParameterNM.Text = MasterParameter.SelectParamList.Single().ParameterNM;
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
            Database.MachineLogFormat.WirebonderFile newItem = new Database.MachineLogFormat.WirebonderFile();
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
            bsItems.EndEdit();
            dgvItems.EndEdit();

            try
            {
                List<Database.MachineLogFormat.WirebonderFile> changeList 
                    = ((List<Database.MachineLogFormat.WirebonderFile>)bsItems.DataSource).Where(p => p.ChangeFG).ToList();
                if (changeList.Count() == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                foreach (Database.MachineLogFormat.WirebonderFile change in changeList)
                {
                    if (string.IsNullOrEmpty(change.PrefixNM)) change.PrefixNM = change.OldPrefixNM;
                    if (change.QcParamNO == 0) change.QcParamNO = change.OldQcParamNO;
                    if (change.FileFmtNO == 0) change.FileFmtNO = change.OldFileFmtNO;
                    if (string.IsNullOrEmpty(change.ModelNM)) change.ModelNM = change.OldModelNM;
                    
                    Database.MachineLogFormat.WirebonderFile.InsertUpdate(change);
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
        /// 一括削除
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAllDelete_Click(object sender, EventArgs e)
        {
            try
            {
                dgvItems.EndEdit();

                List<Database.MachineLogFormat.WirebonderFile> deleteList
                    = ((List<Database.MachineLogFormat.WirebonderFile>)bsItems.DataSource).ToList();
                if (deleteList.Count() == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_9);
                }

                // 警告メッセージ (レコードを物理削除する為、念入りに確認する。)

                string Msg = "現在画面に表示されている全データを物理削除します。物理削除したデータは復旧できません。\r\n" +
                             "表示データを再確認し、全て削除して問題ないか再確認を行って下さい。\r\n\r\n" +
                             " 重要： チェックされていないレコードも物理削除します。 ";

                if (DialogResult.OK != MessageBox.Show(Msg, "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }

                Msg = " このメッセージのOKボタンを押すと物理削除されます。\r\n" +
                    " 本当によろしいですか？\r\n\r\n" +
                    " ① チェック無しのレコードを含む全レコードが「物理削除」されます。\r\n" +
                    " ② 物理削除したレコードは「復旧できません。」";

                if (DialogResult.OK != MessageBox.Show(Msg, "最終確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    return;
                }

                foreach (Database.MachineLogFormat.WirebonderFile delete in deleteList)
                {
                    Database.MachineLogFormat.WirebonderFile.Delete(delete);
                }

                MessageBox.Show("物理削除が完了しました。\r\n 削除レコード数：" + deleteList.Count + "レコード", "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

                Database.MachineLogFormat.WirebonderFile newItem = new Database.MachineLogFormat.WirebonderFile();
                newItem.ChangeFG = true;

                newItem.PrefixNM = clipChars[0].ToString().Trim();
                newItem.OldPrefixNM = newItem.PrefixNM;

                int qcParamNo;
                if (!int.TryParse(clipChars[1].ToString().Trim(), out qcParamNo))
                {
                    throw new Exception(string.Format("管理Noには数値を入力して下さい。値:{0}", clipChars[1]));
                }
                newItem.QcParamNO = qcParamNo;
                newItem.OldQcParamNO = newItem.QcParamNO;

                int fileFmtNo;
                if (!int.TryParse(clipChars[2].ToString().Trim(), out fileFmtNo))
                {
                    throw new Exception(string.Format("紐付け番号には数値を入力して下さい。値:{0}", clipChars[2]));
                }
                newItem.FileFmtNO = fileFmtNo;
                newItem.OldFileFmtNO = newItem.FileFmtNO;

                newItem.ModelNM = clipChars[3].ToString().Trim();
                newItem.OldModelNM = newItem.ModelNM;

                int functionNo;
                if (!int.TryParse(clipChars[4].ToString().Trim(), out functionNo))
                {
                    throw new Exception(string.Format("取得方法番号には数値を入力して下さい。値:{0}", clipChars[4]));
                }
                newItem.FunctionNO = functionNo;

                newItem.SearchNM = clipChars[5].ToString().Trim();

                int searchNo;
                if (!int.TryParse(clipChars[6].ToString().Trim(), out searchNo))
                {
                    throw new Exception(string.Format("検索位置には数値を入力して下さい。値:{0}", clipChars[6]));
                }
                newItem.SearchNO = searchNo;

                int commaNo;
                if (!int.TryParse(clipChars[7].ToString().Trim(), out commaNo))
                {
                    throw new Exception(string.Format("検索位置(カンマ)には数値を入力して下さい。値:{0}", clipChars[7]));
                }
                newItem.CommaNO = commaNo;
                
                int delFg = 0;
                if (int.TryParse(clipChars[8].ToString().Trim(), out delFg))
                {
                    newItem.DelFG = Convert.ToBoolean(delFg);
                }
                else 
                {
                    newItem.DelFG = Convert.ToBoolean(clipChars[8].ToString().Trim());
                }

                newItem.UpdUserCD = clipChars[9].ToString().Trim();

                DateTime lastUpdDt;
                if (!DateTime.TryParse(clipChars[10].ToString().Trim(), out lastUpdDt))
                {
                    throw new Exception(string.Format("更新日時には日付型を入力して下さい。値:{0}", clipChars[10]));
                }
                newItem.LastUpdDT = lastUpdDt;

                bsItems.Add(newItem);
            }
        }

        #region 複数選択ボタン

        private void BtnTextUpdate(Button tempBtn, TextBox tempTxt, List<string> CurrentSelectList)
        {
            if (CurrentSelectList.Count == 0)
            {
                tempBtn.ImageIndex = 0;
                tempTxt.Enabled = true;
            }
            else
            {
                tempBtn.ImageIndex = 1;
                tempTxt.Enabled = false;
                tempTxt.Text = string.Empty;
            }
        }

        private void btnModelMulti_Click(object sender, EventArgs e)
        {
            List<string> ElementList = ConnectQCIL.GetElementbyString("Model_NM", "TmFILEFMT_WB");

            M08_MultiSelect formMultiSelect = new M08_MultiSelect(ElementList, ModelSelectList);
            formMultiSelect.ShowDialog();

            ModelSelectList = formMultiSelect.SelectItemList;

            BtnTextUpdate(btnModelMulti, txtModelNM, ModelSelectList);
        }

        private void btnQcParamMulti_Click(object sender, EventArgs e)
        {
            NameValueCollection NVC = ConnectQCIL.GetPrmFromFilefmtWB();

            M08_MultiSelect formMultiSelect = new M08_MultiSelect(NVC, QcParamSelectList);
            formMultiSelect.ShowDialog();

            QcParamSelectList = formMultiSelect.SelectItemList;
            BtnTextUpdate(btnQcParamMulti, txtQcParamNO, QcParamSelectList);
            if (QcParamSelectList.Count > 0)
            {
                txtParameterNM.Text = "";
            }
        }

        private void btnFileFmtMulti_Click(object sender, EventArgs e)
        {
            List<string> ElementList = ConnectQCIL.GetElementbyInt("FileFmt_NO", "TmFILEFMT_WB");

            M08_MultiSelect formMultiSelect = new M08_MultiSelect(ElementList, FileFmtSelectList);
            formMultiSelect.ShowDialog();

            FileFmtSelectList = formMultiSelect.SelectItemList;

            BtnTextUpdate(btnFileFmtMulti, txtFileFmt, FileFmtSelectList);
        }

        private void btnSearchNmMulti_Click(object sender, EventArgs e)
        {
            List<string> ElementList = ConnectQCIL.GetElementbyString("Search_NM", "TmFILEFMT_WB");

            M08_MultiSelect formMultiSelect = new M08_MultiSelect(ElementList, SearchNmSelectList);
            formMultiSelect.ShowDialog();

            SearchNmSelectList = formMultiSelect.SelectItemList;
            BtnTextUpdate(btnSearchNmMulti, txtSearchNm, SearchNmSelectList);
        }

        #endregion

        #region レコードの並び替え

        private void dgvItems_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.ColumnIndex < 0 || e.ColumnIndex >= dgvItems.ColumnCount)
            {
                return;
            }

            List<Database.MachineLogFormat.WirebonderFile> sortList
                    = ((List<Database.MachineLogFormat.WirebonderFile>)bsItems.DataSource).ToList();

            if (sortList.Count == 0)
            {
                return;
            }

            // 直前にクリックした列と同じ
            if (OldSortColumnIndex == e.ColumnIndex)
            {
                // 昇順 ⇒ 降順 ⇒ 昇順と切り替え
                AscFg = !AscFg;
            }
            else
            {
                // 操作列を記録。最初は昇順
                OldSortColumnIndex = e.ColumnIndex;
                AscFg = true;
            }
            
            var name = dgvItems.Columns[e.ColumnIndex].Name;
            if (AscFg)
            {
                // 昇順
                sortList = sortList.OrderBy(l => l.GetType().GetProperty(name).GetValue(l, null)).ToList();
            }
            else
            {
                // 降順
                sortList = sortList.OrderByDescending(l => l.GetType().GetProperty(name).GetValue(l, null)).ToList();
            }

            bsItems.DataSource = sortList;
            
        }

    

        #endregion

        private void txtQcParamNO_Leave(object sender, EventArgs e)
        {
            int oQcParam_NO;

            if (!string.IsNullOrEmpty(this.txtQcParamNO.Text) && int.TryParse(this.txtQcParamNO.Text, out oQcParam_NO))
            {
                this.txtParameterNM.Text = ConnectQCIL.GetPRMData(oQcParam_NO).ParameterNM;
            }
            else
            {
                this.txtParameterNM.Text = "";
            }
        }

        private void btnAllDelete_and_Save_Click(object sender, EventArgs e)
        {
            bsItems.EndEdit();
            dgvItems.EndEdit();

            try
            {
                List<Database.MachineLogFormat.WirebonderFile> changeList
                    = ((List<Database.MachineLogFormat.WirebonderFile>)bsItems.DataSource).ToList();
                if (changeList.Count() == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                // 画面上の紐付け番号の重複削除 + 各紐付け番号のレコード数カウント(画面上 + データベース登録済み)
                List<string> FileFmtList = new List<string>();
                Dictionary<string, int> RecordCount = new Dictionary<string, int>();
                Dictionary<string, int> DatabaseCount = new Dictionary<string, int>();
                foreach (Database.MachineLogFormat.WirebonderFile change in changeList)
                {
                    int FileFmtNo = change.FileFmtNO;
                    string sFileFmtNo = FileFmtNo.ToString();

                    if (!FileFmtList.Exists(f => f == sFileFmtNo))
                    {
                        FileFmtList.Add(sFileFmtNo);
                        RecordCount.Add(sFileFmtNo, 1);
                        DatabaseCount.Add(sFileFmtNo, 0);
                    }
                    else
                    {
                        RecordCount[sFileFmtNo]++;
                    }
                }

                // データベース上の紐付け番号のレコード取得 + レコード数記録
                List<Database.MachineLogFormat.WirebonderFile> deleteList = new List<GEICS.Database.MachineLogFormat.WirebonderFile>();
                foreach (string sFileFmtNo in FileFmtList)
                {
                    int FileFmtNo;
                    if(int.TryParse(sFileFmtNo, out FileFmtNo))
                    {
                        List<Database.MachineLogFormat.WirebonderFile> dList
                                    = Database.MachineLogFormat.WirebonderFile.GetData(null,null,null, FileFmtNo);
                        DatabaseCount[sFileFmtNo] = dList.Count;

                        deleteList.AddRange(dList);
                    }
                }


                // 警告メッセージ (レコードを物理削除する為、念入りに確認する。)
                string Msg = string.Format("画面上にある紐付け番号の全データを物理削除した後に\r\n画面上のデータを新規登録します。\r\n\r\n" +
                                           "【重要】物理削除したデータは復旧できません。\r\n" +
                                           "            画面上のレコードに不足がない事を再確認して下さい。\r\n" +
                                           "            チェック無しのレコードも物理削除・新規登録対象です。\r\n\r\n" +
                                           "■レコード数\r\n" +
                                           "{0,12}{1,10}{2,10}{3,12}\r\n", "紐付け番号", "物理削除", "新規登録","増減");

                foreach (string sFileFmtNo in FileFmtList)
                {
                    int Diff = RecordCount[sFileFmtNo] - DatabaseCount[sFileFmtNo];
                    string sDiff = Diff == 0 ? "±" + Diff : (Diff > 0 ? "＋" + Diff.ToString() : "－" + Math.Abs(Diff).ToString());

                    Msg += string.Format("{0,20}{1,16}{2,18}{3,14}\r\n", 
                        sFileFmtNo, DatabaseCount[sFileFmtNo], RecordCount[sFileFmtNo], sDiff);
                }

                if (DialogResult.OK != MessageBox.Show(Msg, "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }

                Msg = " このメッセージのOKボタンを押すと処理が進みます。\r\n" +
                      " 本当によろしいですか？";

                if (DialogResult.OK != MessageBox.Show(Msg, "最終確認", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning))
                {
                    return;
                }

                foreach (Database.MachineLogFormat.WirebonderFile change in changeList)
                {
                    change.OldPrefixNM = change.PrefixNM;
                    change.OldQcParamNO = change.QcParamNO;
                    change.OldFileFmtNO = change.FileFmtNO;
                    change.OldModelNM = change.ModelNM;

                    Database.MachineLogFormat.WirebonderFile.InsertUpdate(change);
                }

                foreach (Database.MachineLogFormat.WirebonderFile delete in deleteList)
                {
                    // 削除対象レコードの内、新規登録レコードに存在しないレコードを検索
                    if (!changeList.Exists(c =>
                        c.PrefixNM == delete.PrefixNM && c.QcParamNO == delete.QcParamNO &&
                        c.FileFmtNO == delete.FileFmtNO && c.ModelNM == delete.ModelNM))
                    {
                        // 削除 (レコード物理削除)
                        Database.MachineLogFormat.WirebonderFile.Delete(delete);
                    }
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

    }
}
