using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using SLCommonLib.Commons;
using System.Data.SqlClient;
using GEICS.Database;
using GEICS.Function;

namespace GEICS
{
    public partial class M17_PlcFileConv : Form
    {

        #region 変数

        /// <summary>
        /// 管理項目選択リスト
        /// </summary>
        private List<string> qcParamSelectList { get; set; }

        private BackgroundWorker bwOutputExcel;

        private bool latestEnableStatustoolExcelOutput;
        private bool latestEnableStatustoolExcelOutputAll;
        private bool latestEnableStatustoolExcelRead;


        #endregion

        #region コンストラクタ・フォームロード

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public M17_PlcFileConv()
        {
            InitializeComponent();
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M17_PlcFileConv_Load(object sender, EventArgs e)
        {
            qcParamSelectList = new List<string>();

            // データ形式列の設定
            DataGridViewComboBoxColumn column = (DataGridViewComboBoxColumn)grdItems.Columns["DataTypeCD"];
            column.DataSource = PlcFileConv.DataTypeMaster;
            column.DisplayMember = "NM";
            column.ValueMember = "CD";

            bwOutputExcel = new BackgroundWorker();
            bwOutputExcel.DoWork += new DoWorkEventHandler(outputExcel);
            bwOutputExcel.WorkerSupportsCancellation = true;

        }

        #endregion

        #region 検索条件指定

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

                        this.qcParamSelectList = MasterParameter.SelectParamList.Select(p => p.QcParamNO).ToList().ConvertAll(i => i.ToString());
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
            M08_MultiSelect formMultiSelect = new M08_MultiSelect(qcParamSelectList, qcParamSelectList);
            formMultiSelect.ShowDialog();

            qcParamSelectList = formMultiSelect.SelectItemList;
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
        /// 管理NOを変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQcParamNO_Leave(object sender, EventArgs e)
        {
            int outQcParamNO;

            if (string.IsNullOrEmpty(txtQcParamNO.Text))
            {
                txtParameterNM.Text = string.Empty;
            }
            else if(int.TryParse(txtQcParamNO.Text, out outQcParamNO))
            {
                List<Prm> prmList = Prm.GetData(null, null, outQcParamNO);
                if (prmList.Count == 1)
                {
                    txtParameterNM.Text = prmList.Single().ParameterNM.Trim();

                    qcParamSelectList.Clear();
                    btnQcParamMulti.ImageIndex = 0;
                }
            }
        }

        #endregion

        /// <summary>
        /// 検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSearch_Click(object sender, EventArgs e)
        {
            try
            {
                string sModel_NM = txtModelNM.Text.Trim();
                string sPrefix_NM = txtPrefixNM.Text.Trim();

                List<int> qcParamList = new List<int>();
                foreach (string s in qcParamSelectList)
                {
                    int i = Convert.ToInt32(s);
                    qcParamList.Add(i);
                }
                if (!string.IsNullOrEmpty(txtQcParamNO.Text))
                {
                    int temp;
                    if (int.TryParse(txtQcParamNO.Text.Trim(), out temp))
                    {
                        qcParamList.Add(temp);
                    }
                }

                List<PlcFileConv> pfcList = PlcFileConv.GetDatawithPRM(sModel_NM, qcParamList, sPrefix_NM, cbDelFG.Checked);

                bsPlcFileConv.DataSource = pfcList;
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        #region グリッドセル操作

        /// <summary>
        /// グリッドセル値変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                return;
            }

            if (grdItems.Columns["IsCheck"].Index == e.ColumnIndex)
            {
                return;
            }

            ((DataGridViewCheckBoxCell)grdItems.Rows[e.RowIndex].Cells["IsCheck"]).Value = true;

            grdItems.EndEdit();
        }

        /// <summary>
        /// キーを押した時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdItems_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ')
            {
                foreach (DataGridViewRow row in grdItems.SelectedRows)
                {
                    row.Cells["IsCheck"].Value = !(bool)row.Cells["IsCheck"].Value;
                }
            }
        }

        #endregion

        #region グリッドセルオプションボタン

        /// <summary>
        /// 新規追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (M03_Parameter MasterParameter = new M03_Parameter(false, false, null))
                {
                    MasterParameter.ShowDialog();

                    if (MasterParameter.SelectParamList.Count == 0)
                    {
                        return;
                    }
                    Prm prm = MasterParameter.SelectParamList.Single();

                    PlcFileConv pfc = new PlcFileConv();
                    pfc.QcModelNM = prm.ModelNM;
                    pfc.QcParamNO = prm.QcParamNO;
                    pfc.ClassNM = prm.ClassNM;
                    pfc.ParameterNM = prm.ParameterNM;
                    pfc.OrderNO = 1;
                    pfc.DataTypeCD = PlcFileConv.DataTypeMaster[0].CD;

                    bsPlcFileConv.Add(pfc);
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
        /// 削除フラグON
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDelFgOn_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (dr.Selected)
                {
                    dr.Cells["DelFG"].Value = true;
                }
            }
        }

        /// <summary>
        /// 削除フラグOFF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolDelFgOff_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow dr in grdItems.Rows)
            {
                if (dr.Selected)
                {
                    dr.Cells["DelFG"].Value = false;
                }
            }
        }

        #endregion

        #region 保存

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPreserve_Click(object sender, EventArgs e)
        {
            try
            {
                grdItems.EndEdit();

                if (bsPlcFileConv.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_25, "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }

                List<PlcFileConv> pfcList = new List<PlcFileConv>();

                List<int> CheckNGRowList = new List<int>();
                foreach (DataGridViewRow row in grdItems.Rows)
                {
                    if (Convert.ToBoolean(row.Cells["IsCheck"].Value))
                    {
                        // 特定の列が登録可能な値であるかをチェック
                        if (RowCheck(row))
                        {
                            pfcList.Add((PlcFileConv)bsPlcFileConv.List[row.Index]);
                        }
                        else
                        {
                            CheckNGRowList.Add(row.Index + 1);
                        }
                    }
                }

                // 登録可能でない行があれば、メッセージ
                if (CheckNGRowList.Count > 0)
                {
                    string msg = "以下の行Noのレコードは、\r\n  「装置形式」「管理No」「ファイル識別文字」「検査位置」\r\nのいずれかが 空欄 または 0 の為、保存できません。\r\n";
                    foreach (int i in CheckNGRowList)
                    {
                        msg += "   " + i.ToString() + "レコード目\r\n";
                    }
                    throw new ApplicationException(msg);
                }

                if (pfcList.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                PlcFileConv.InsertUpdate(pfcList);

                MessageBox.Show(Constant.MessageInfo.Message_75, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (ApplicationException err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);

            }
        }

        /// <summary>
        /// グリッド行の内容確認
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private bool RowCheck(DataGridViewRow row)
        {
            // 「装置形式」列 = 空欄
            if (string.IsNullOrEmpty(Convert.ToString(row.Cells["ModelNM"].Value)))
            {
                return false;
            }
            // 「管理No」列 = 0
            if (Convert.ToInt32(row.Cells["QcParamNO"].Value) == 0)
            {
                return false;
            }
            // 「ファイル識別文字」列 = 空欄
            if (string.IsNullOrEmpty(Convert.ToString(row.Cells["PrefixNM"].Value)))
            {
                return false;
            }
            // 「検索位置」列 = 0
            if (Convert.ToInt32(row.Cells["OrderNO"].Value) == 0)
            {
                return false;
            }

            return true;
        }

        #endregion

        #region フォームツールバー

        private void timerExcelOutputProgress_Tick(object sender, EventArgs e)
        {
            ExcelControl xlControl = ExcelControl.GetInstance();
            pbExcel.Value = xlControl.GetProgressRate();
            if (pbExcel.Value >= 100)
            {
                ReturnEnableStatusExcelMenu();
                pbExcel.Value = 0;
                timer1.Stop();
            }
        }

        private void outputExcel(object sender, DoWorkEventArgs e)
        {
            ExcelControl excel = ExcelControl.GetInstance();
            excel.PlcFileConvDataOutput(((ExcelControl.MasterPlcFileConvExcelData)e.Argument).data, ((ExcelControl.MasterPlcFileConvExcelData)e.Argument).colorFgArray);
        }

        private void DisableExcelMenu()
        {
            latestEnableStatustoolExcelOutput = toolExcelOutput.Enabled;
            latestEnableStatustoolExcelRead = toolExcelRead.Enabled;

            toolExcelOutput.Enabled = false;
            toolExcelRead.Enabled = false;
        }

        private void ReturnEnableStatusExcelMenu()
        {
            toolExcelOutput.Enabled = latestEnableStatustoolExcelOutput;
            toolExcelRead.Enabled = latestEnableStatustoolExcelRead;
        }

        /// <summary>
        /// Excel出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolExcelOutput_Click(object sender, EventArgs e)
        {
            try
            {
                DisableExcelMenu();

                List<PlcFileConv> pfcList = new List<PlcFileConv>();
                foreach (DataGridViewRow row in grdItems.Rows)
                {
                    pfcList.Add((PlcFileConv)bsPlcFileConv.List[row.Index]);
                }

                object[,] valArray;
                bool[] celColorArray;

                GetPlcFileConvData(pfcList, out valArray, out celColorArray);

                ExcelControl.MasterPlcFileConvExcelData exlData;
                exlData.data = valArray;
                exlData.colorFgArray = celColorArray;

                bwOutputExcel.RunWorkerAsync(exlData);

                timer1.Start();

            }
            catch (ApplicationException err)
            {
                ReturnEnableStatusExcelMenu();
                MessageBox.Show(err.Message, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
            }
            catch (Exception err)
            {
                ReturnEnableStatusExcelMenu();
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plmList"></param>
        /// <returns></returns>
        private void GetPlcFileConvData(List<PlcFileConv> pfcList, out object[,] valArray, out bool[] celColorArray)
        {
            pfcList = pfcList.Where(p => p.DelFG == false).OrderBy(p => p.PrefixNM + "," + p.ModelNM + "," + p.QcParamNO + "," + p.ParameterNM).ToList();

            object[,] limitData = new object[pfcList.Count, 9];
            bool[] colorFgData = new bool[9];
            for (int i = 0; i < pfcList.Count; i++)
            {
                limitData[i, 0] = pfcList[i].QcParamNO;
                limitData[i, 1] = pfcList[i].ParameterNM;
                limitData[i, 2] = pfcList[i].ModelNM;
                limitData[i, 3] = pfcList[i].PrefixNM;
                limitData[i, 4] = pfcList[i].HeaderNM;
                limitData[i, 5] = pfcList[i].OrderNO;
                limitData[i, 6] = pfcList[i].PlcADDR;
                limitData[i, 7] = pfcList[i].DataLEN;
                limitData[i, 8] = PlcFileConv.GetDataType_NM(pfcList[i].DataTypeCD);
            }
            colorFgData[0] = true;
            colorFgData[2] = true;
            colorFgData[3] = true;
            colorFgData[4] = true;
            colorFgData[5] = true;
            colorFgData[6] = true;
            colorFgData[7] = true;
            colorFgData[8] = true;

            valArray = limitData;
            celColorArray = colorFgData;
        }

        /// <summary>
        /// Excel読込(通常)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolExcelRead_Click(object sender, EventArgs e)
        {
            try
            {
                if (DialogResult.OK != openFileDialog.ShowDialog())
                {
                    return;
                }

                object[,] dataList;
                using (ExcelControl excel = ExcelControl.GetInstance())
                {
                    dataList = excel.GetPlcFileConvData(openFileDialog.FileName);
                    if (dataList.GetLength(0) == 0)
                    {
                        return;
                    }
                }

                // 取り込みデータをリストに追加 + 内容チェック
                List<PlcFileConv> pfcList = new List<PlcFileConv>();
                List<string> errMsgList = new List<string>();
                for (int rowIndex = 1; rowIndex <= dataList.GetLength(0); rowIndex++)
                {
                    int rowStartIndex = ExcelControl.TPL04_DATASTART_ROW;
                    bool errorfg = false;
                    int rowNo = rowIndex + rowStartIndex - 1;

                    string qcparamno = Convert.ToString(dataList[rowIndex,1]);
                    string modelnm = Convert.ToString(dataList[rowIndex,3]);
                    string prefixnm = Convert.ToString(dataList[rowIndex,4]);
                    string headernm = Convert.ToString(dataList[rowIndex,5]);
                    string orderno = Convert.ToString(dataList[rowIndex,6]);
                    string plcaddr = Convert.ToString(dataList[rowIndex,7]);
                    string datalen = Convert.ToString(dataList[rowIndex,8]);
                    string datatypenm = Convert.ToString(dataList[rowIndex,9]);

                    if (string.IsNullOrEmpty(qcparamno) ||
                        string.IsNullOrEmpty(modelnm) ||
                        string.IsNullOrEmpty(prefixnm) ||
                        string.IsNullOrEmpty(orderno))
                    {
                        errorfg = true;
                        errMsgList.Add(Convert.ToString(rowNo) + "行目：必須入力項目の管理No、装置形式、ファイル識別文字、検査位置の何れかが入力されていません。");
                    }


                    int intQcParamNO = 0;
                    if (!int.TryParse(qcparamno, out intQcParamNO) || intQcParamNO == 0)
                    {
                        errorfg = true;
                        errMsgList.Add(Convert.ToString(rowNo) + "行目：管理NOに 数字以外 または 0 が入力されています。");
                    }

                    string qcmodelnm = Constant.VOID_STRING;
                    string classnm = Constant.VOID_STRING;
                    string parameternm = Constant.VOID_STRING;
                    List<Prm> prmList = Prm.GetData(null, null, intQcParamNO);
                    if (prmList.Count != 0)
                    {
                        qcmodelnm = prmList[0].ModelNM;
                        classnm = prmList[0].ClassNM;
                        parameternm = prmList[0].ParameterNM;
                    }
                    else
                    {
                        errorfg = true;
                        errMsgList.Add(Convert.ToString(rowNo) + "行目：管理Noがマスタに登録されていません。");
                    }

                    int intOrderNO = 0;
                    if (!int.TryParse(orderno, out intOrderNO) || intOrderNO == 0)
                    {
                        errorfg = true;
                        errMsgList.Add(Convert.ToString(rowNo) + "行目：検査位置に 数字以外 または 0 が入力されています。");
                    }

                    int intDataLEN = 0;
                    if (!int.TryParse(datalen, out intDataLEN) || intDataLEN == 0)
                    {
                        errorfg = true;
                        errMsgList.Add(Convert.ToString(rowNo) + "行目：データ文字数に 数字以外 または 0 が入力されています。");
                    }

                    string datatypecd = PlcFileConv.GetDataType_CD(datatypenm);
                    if (string.IsNullOrEmpty(datatypecd))
                    {
                        errorfg = true;
                        errMsgList.Add(Convert.ToString(rowNo) + "行目：データ形式が不適切な値です。");
                    }

                    if (!errorfg)
                    {
                        PlcFileConv pfc = new PlcFileConv();
                        pfc.IsCheck = true;
                        pfc.QcParamNO = intQcParamNO;
                        pfc.QcModelNM = qcmodelnm;
                        pfc.ClassNM = classnm;
                        pfc.ParameterNM = parameternm;
                        pfc.ModelNM = modelnm;
                        pfc.PrefixNM = prefixnm;
                        pfc.HeaderNM = headernm;
                        pfc.OrderNO = intOrderNO;
                        pfc.PlcADDR = plcaddr;
                        pfc.DataLEN = intDataLEN;
                        pfc.DataTypeCD = datatypecd;
                        pfcList.Add(pfc);
                    }

                }
                
                bsPlcFileConv.DataSource = pfcList.ToSortableBindingList();

                if (errMsgList.Count != 0)
                {
                    string errMsg = string.Join("\r\n", errMsgList.ToArray());
                    MessageBox.Show("取り込んだデータに不備があります。\r\n\r\n" + errMsg);
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

        #endregion

    }
}
