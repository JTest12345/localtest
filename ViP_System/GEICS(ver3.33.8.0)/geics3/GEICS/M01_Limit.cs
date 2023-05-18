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
    public partial class M01_Limit : Form
    {
        /// <summary>
        /// 画面スタイル
        /// </summary>
        private enum FormStyle 
        {
            Normal,
            Input,
            Submit,
        }

        /// <summary>
        /// 機能スタイル
        /// </summary>
        public enum FunctionStyle
        {
            // 読み取り専用
            Read,
            // 編集
            Write,
            // 内規編集
            InnerLimitEdit,
            // 蛍光体シート読み取り専用
            PhosphorSheetRead,
            // 蛍光体シート編集
            PhosphorSheetWrite,
        }

        private FunctionStyle _FunctionStyle;
        
        /// <summary>
        /// 製品型番選択リスト
        /// </summary>
        private List<string> typeSelectList { get; set; }

        /// <summary>
        /// 樹脂Gr選択リスト
        /// </summary>
        private List<string> resinGroupSelectList { get; set; }

        /// <summary>
        /// 管理項目選択リスト
        /// </summary>
        private List<string> qcParamSelectList { get; set; }

        /// <summary>
        /// 樹脂Gr管理フラグ
        /// </summary>
        private bool isResinGroupManageCondition { get; set; }

        private BackgroundWorker bwOutputExcel;

		private bool latestEnableStatustoolExcelOutput;
		private bool latestEnableStatustoolExcelOutputAll;
		private bool latestEnableStatustoolExcelRead;

        private bool viewProgMat { get; set; }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="writeFG">書き込み権限</param>
        public M01_Limit(FunctionStyle style)
        {
            InitializeComponent();

            _FunctionStyle = style;

            typeSelectList = new List<string>();
            qcParamSelectList = new List<string>();
            resinGroupSelectList = new List<string>();

			bwOutputExcel = new BackgroundWorker();
			bwOutputExcel.DoWork += new DoWorkEventHandler(outputExcel);
			bwOutputExcel.WorkerSupportsCancellation = true;
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void frmMasterLimit_Load(object sender, EventArgs e)
        {
            try
            {
                if (this._FunctionStyle == FunctionStyle.Write)
                {
                    ChangeFunctionStyle(FunctionStyle.Write);
                }
                else if (this._FunctionStyle == FunctionStyle.InnerLimitEdit)
                {
                    ChangeFunctionStyle(FunctionStyle.InnerLimitEdit);
                }
                else if (this._FunctionStyle == FunctionStyle.PhosphorSheetRead)
                {
                    ChangeFunctionStyle(FunctionStyle.PhosphorSheetRead);
                }
                else if (this._FunctionStyle == FunctionStyle.PhosphorSheetWrite)
                {
                    ChangeFunctionStyle(FunctionStyle.PhosphorSheetWrite);
                }
                else 
                {
                    ChangeFunctionStyle(FunctionStyle.Read);
                }

                ChangeFormStyle(FormStyle.Normal);

				cmbType.DataSource = ConnectQCIL.GetPLMTypeCD(cbDisplayDisableType.Checked, this.isResinGroupManageCondition, this.chkProgMat.Checked);
                cmbType.Text = string.Empty;
            }
            catch (Exception err) 
            {
                MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        #region フォームツールバー

		private void outputExcel(object sender, DoWorkEventArgs e)
		{
			ExcelControl excel = ExcelControl.GetInstance();
			excel.LimitDataOutput(
                ((ExcelControl.MasterLimitExcelData)e.Argument).data, 
                ((ExcelControl.MasterLimitExcelData)e.Argument).colorFgArray, 
                ((ExcelControl.MasterLimitExcelData)e.Argument).coloringFG, 
                ((ExcelControl.MasterLimitExcelData)e.Argument).typeCDStr,
                ((ExcelControl.MasterLimitExcelData)e.Argument).resinGroupOutputFG
                );
		}

        /// <summary>
        /// Excel出力
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolExcelOutput_Click(object sender, EventArgs e)
        {
			if (cbEnableCellColorAndProtect.Checked)
			{
				if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_62, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
				{
					return;
				}
			}

            try
            {
				DisableExcelMenu();
				SortableBindingList<PlmInfo> sbPlmInfoList = (SortableBindingList<PlmInfo>)bsLimit.DataSource;

				object[,] valArray;
				bool[,] celColorArray;

				GetLimitData(sbPlmInfoList, out valArray, out celColorArray);

				ExcelControl.MasterLimitExcelData exlData;
				exlData.data = valArray;
				exlData.colorFgArray = celColorArray;
				exlData.coloringFG = cbEnableCellColorAndProtect.Checked;
				exlData.typeCDStr = string.Join(",", sbPlmInfoList.Select(s => s.MaterialCD).Distinct().ToArray());
                exlData.resinGroupOutputFG = this.isResinGroupManageCondition;

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
        /// Excel出力(全管理項目)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolExcelOutputAll_Click(object sender, EventArgs e)
        {
            if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_62, "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                return;
            }

            try
            {
				DisableExcelMenu();
                List<ParamInfo> paramList = ConnectQCIL.GetPRMData();
                foreach(ParamInfo paramInfo in paramList)
                {
                    //不良名を取得
					//if (paramInfo.QcParamNO >= 10000 && !((paramInfo.QcParamNO >= 200000) && (paramInfo.QcParamNO < 300000)))
					if(paramInfo.UnManageTrendFG == true)
					{
                        paramInfo.ParameterNM += "(" + ConnectQCIL.GetDefectName(paramInfo.ParameterNM) + ")";
                    }
                }

                object[,] paramData = GetParamData(paramList);

				ExcelControl.MasterLimitExcelData exlData = new ExcelControl.MasterLimitExcelData();
				exlData.data = paramData;
				exlData.coloringFG = false;
                exlData.resinGroupOutputFG = this.isResinGroupManageCondition;
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

                List<PlmInfo> plmList = new List<PlmInfo>();
                using (ExcelControl excel = ExcelControl.GetInstance())
                {
                    plmList = excel.GetLimitData(openFileDialog.FileName);
                    if (plmList.Count == 0)
                    {
                        return;
                    }
                }

                //Excel上に同型番で「ダイス搭載数/装置」の値が違うレコードがあるとエラー停止する
                var mateList = plmList.Select(p => new { MaterialCD = p.MaterialCD, DiceCT = p.DiceCT }).Distinct();
                var errorList = mateList.GroupBy(p => p.MaterialCD).Select(p => new { MaterialCD = p.Key, Num = p.Count() }).Where(p => p.Num > 1);
                if (errorList.Count() != 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_103);
                }

                foreach (PlmInfo plmInfo in plmList)
                {
                    ParamInfo paramInfo = ConnectQCIL.GetPRMData(plmInfo.QcParamNO);

                    //plmInfo.ModelNM = paramInfo.ModelNM;
                    plmInfo.ClassNM = paramInfo.ClassNM;
                    plmInfo.ParameterNM = paramInfo.ParameterNM;
                    plmInfo.ManageNM = paramInfo.ManageNM;
                    plmInfo.TimingNM = paramInfo.TimingNM;
                    plmInfo.Info1NM = paramInfo.Info1NM;
                    plmInfo.Info2NM = paramInfo.Info2NM;
                    plmInfo.Info3NM = paramInfo.Info3NM;
					plmInfo.EquipManageFG = paramInfo.EquipManageFG;
                    plmInfo.ResinGroupManageFG = paramInfo.ResinGroupManageFG;
                }

                bsLimit.DataSource = plmList.ToSortableBindingList();
                ChangeFormStyle(FormStyle.Submit);
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

        #region 検索

        /// <summary>
        /// 管理検索
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnManagementSearch_Click(object sender, EventArgs e)
        {
            try
            {
                using (M03_Parameter MasterParameter = new M03_Parameter(false, true, isResinGroupManageCondition))
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
        /// 管理NOを変更
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtQcParamNO_TextChanged(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtQcParamNO.Text))
            {
                txtParameterNM.Text = string.Empty;
            }
        }

        /// <summary>
        /// 製品型番複数選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnTypeMulti_Click(object sender, EventArgs e)
        {
			List<string> typeList = ConnectQCIL.GetPLMTypeCD(cbDisplayDisableType.Checked, this.isResinGroupManageCondition, this.chkProgMat.Checked);

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
                    qcParamSelectList.Add(txtQcParamNO.Text);
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

                if (cmbResinGroup.Text != string.Empty)
                {
                    resinGroupSelectList.Clear();
                    resinGroupSelectList.Add(cmbResinGroup.Text);
                }


                //閾値一覧出力
                outputLimitData(typeSelectList, qcParamSelectList, resinGroupSelectList);

				ChangeFormStyle(FormStyle.Input);
				//if (typeSelectList.Count != 0 && qcParamSelectList.Count == 0)
				//{
				//    //製品型番のみ検索条件を指定している場合、Excel出力機能を有効にする
				//    ChangeFormStyle(FormStyle.Input);
				//}
				//else
				//{
				//    ChangeFormStyle(FormStyle.Normal);
				//}
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
        /// 閾値一覧出力
        /// </summary>
        /// <param name="typeCD"></param>
        /// <param name="qcParamNO"></param>
        private void outputLimitData(List<string> typeList, List<string> qcParamList, List<string> resinGroupList)
        {
            this.viewProgMat = this.chkProgMat.Checked;
            SortableBindingList<PlmInfo> plmList = ConnectQCIL.GetPLMData(typeList, qcParamList, string.Empty, cbDelFG.Checked, this.isResinGroupManageCondition, resinGroupSelectList, this.viewProgMat)
                .ToSortableBindingList();
            foreach (PlmInfo plmInfo in plmList)
            {
				//ダイス個数を取得
				plmInfo.DiceCT = getDiceCount(plmInfo.MaterialCD);

				//不良名を取得
				//if (plmInfo.QcParamNO >= 10000 && !((plmInfo.QcParamNO >= 200000) && (plmInfo.QcParamNO < 300000)))
				if (plmInfo.UnManageTrendFG == true)
				{
					plmInfo.ParameterNM += "(" + ConnectQCIL.GetDefectName(plmInfo.ParameterNM) + ")";
                }

                //打点数を取得
                int inspectionNO = ConnectQCIL.GetInspectionNO(plmInfo.QcParamNO);
                if (inspectionNO == int.MinValue)
                {
                    continue;
                }
                plmInfo.InspectionNO = inspectionNO;

                int qcnumVAL = ConnectQCIL.GetQCnumVAL(plmInfo.MaterialCD, plmInfo.InspectionNO, 9);
                if (qcnumVAL == int.MinValue)
                {
                    continue;
                }
                plmInfo.QcLinePNT = qcnumVAL;
				plmInfo.QCnumNO = 9;
            }

            bsLimit.DataSource = plmList;
        }


        #endregion

        #region グリッドツールバー

        /// <summary>
        /// 新規追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAdd_Click(object sender, EventArgs e)
        {
            try
            {
                using (M03_Parameter MasterParameter = new M03_Parameter(false, false, isResinGroupManageCondition))
                {
                    MasterParameter.ShowDialog();

                    if (MasterParameter.SelectParamList.Count == 0)
                    {
                        return;
                    }
                    Prm prm = MasterParameter.SelectParamList.Single();

                    if (((SortableBindingList<PlmInfo>)bsLimit.DataSource).Count(p => p.QcParamNO == prm.QcParamNO) != 0)
                    {
                        //すでにリストにある場合は処理停止
                        throw new ApplicationException(Constant.MessageInfo.Message_87);
                    }

                    PlmInfo plmInfo = new PlmInfo();
                    plmInfo.QcParamNO = prm.QcParamNO;
                    plmInfo.ModelNM = prm.ModelNM;
                    plmInfo.ClassNM = prm.ClassNM;
                    //plmInfo.MaterialCD = txtTypeCD.Text;
                    plmInfo.ParameterNM = prm.ParameterNM;
                    plmInfo.ManageNM = prm.ManageNM;
                    //plmInfo.TimingNM = MasterParameter.ParamInfo.TimingNM;

                    Database.PrmInfo prmInfo = Database.PrmInfo.GetData(plmInfo.QcParamNO);
                    
                    plmInfo.Info1NM = prmInfo.Info1;
                    plmInfo.Info2NM = prmInfo.Info2;
                    plmInfo.Info3NM = prmInfo.Info3;
                    plmInfo.ChangeFG = true;
                    bsLimit.Add(plmInfo);

                    ChangeFormStyle(FormStyle.Submit);
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
            foreach (DataGridViewRow dr in dgvItems.Rows)
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
            foreach (DataGridViewRow dr in dgvItems.Rows)
            {
                if (dr.Selected)
                {
                    dr.Cells["DelFG"].Value = false;
                }
            }
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPreserve_Click(object sender, EventArgs e)
        {
			PlmInfo plmErrOut = new PlmInfo();

            dgvItems.EndEdit();

            try
            {
				if (bsLimit.Count == 0)
				{
					throw new ApplicationException(Constant.MessageInfo.Message_96);
				}

				if (DialogResult.OK != MessageBox.Show(Constant.MessageInfo.Message_25, "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
				{
					return;
				}

                List<PlmInfo> plmList = ((SortableBindingList<PlmInfo>)bsLimit.DataSource).Where(p => p.ChangeFG).ToList();
				
                if (plmList.Count == 0)
                {
					throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

				string errMsg = string.Empty;
				if (PlmInfo.HasProblem(plmList, out errMsg))
				{
					F100_MsgBox.Show(errMsg);
					throw new ApplicationException("保存を中止します。");
				}

                //ダイス搭載数/装置を手入力許可にしたので、同型番で「ダイス搭載数/装置」の値が違うレコードがあるとエラーとする
                var mateList = ((SortableBindingList<PlmInfo>)bsLimit.DataSource).Select(p => new { MaterialCD = p.MaterialCD, DiceCT = p.DiceCT }).Distinct();
                var errorList = mateList.GroupBy(p => p.MaterialCD).Select(p => new { MaterialCD = p.Key, Num = p.Count() }).Where(p => p.Num > 1);
                if(errorList.Count() != 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_103);
                }

                using (ConnectQCIL conn = new ConnectQCIL(true, Constant.StrQCIL))
                {
					try
					{
						foreach (PlmInfo plmInfo in plmList)
						{
							plmErrOut = plmInfo;

							if (string.IsNullOrEmpty(plmInfo.ReasonVAL))
							{
								throw new ApplicationException(string.Format(Constant.MessageInfo.Message_91, plmInfo.QcParamNO));
							}

							conn.InsertUpdatePLM(plmInfo);
							conn.InsertPLMHist(plmInfo);

							if (plmInfo.QcLinePNT != null)
							{
								conn.InsertUpdateQCST(plmInfo);
							}

						}

						foreach (var mate in mateList)
						{
                            // 蛍光体シート品目のダイス搭載数が無い為、登録がある場合に限り登録する処理に変更
                            if (string.IsNullOrEmpty(mate.DiceCT) == false)
                            {
                                conn.InsertUpdateDIECT(mate.MaterialCD, Convert.ToInt32(mate.DiceCT));
                            }
						}

						conn.Connection.Commit();
					}
					catch (SqlException err)
					{
						conn.Connection.Rollback();

						throw new ApplicationException(string.Format(
							"ﾏｽﾀ更新処理でｴﾗｰ 設備:{0} ﾀｲﾌﾟ:{1} ﾊﾟﾗﾒﾀNo:{2} ﾊﾟﾗﾒﾀ名:{3} ﾀﾞｲｽ数:{4} 設備No:{5} ｲﾝｽﾍﾟｸｼｮﾝNo:{6} QcNumNo:{7} ｴﾗｰ詳細:{8}"
							, plmErrOut.ModelNM, plmErrOut.MaterialCD, plmErrOut.QcParamNO, plmErrOut.ParameterNM, plmErrOut.DiceCT, plmErrOut.EquipmentNO
							, plmErrOut.InspectionNO, plmErrOut.QCnumNO, err.Message), err);
					}
                }

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
        /// 履歴
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolRevision_Click(object sender, EventArgs e)
        {
            if (dgvItems.SelectedRows.Count == 0 || dgvItems.SelectedRows.Count > 1)
            {
                return;
            }

            string materialCD = dgvItems.SelectedRows[0].Cells["MaterialCD"].Value.ToString();
            string modelNM = dgvItems.SelectedRows[0].Cells["ModelNM"].Value.ToString();
            int qcParamNO = Convert.ToInt32(dgvItems.SelectedRows[0].Cells["QcParamNO"].Value);
			string equipNO = dgvItems.SelectedRows[0].Cells["EquipmentNO"].Value.ToString();
            string resinGroupCD = dgvItems.SelectedRows[0].Cells["ResinGroupCD"].Value.ToString();

            M02_LimitRecord formMasterLimitRecord = new M02_LimitRecord(materialCD, modelNM, qcParamNO, equipNO, resinGroupCD);
            formMasterLimitRecord.ShowDialog();
        }

        /// <summary>
        /// 他ライン転送
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolLineForward_Click(object sender, EventArgs e)
        {
            try
            {
                if (typeSelectList.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_44);
                }

                M06_LimitLineForward formLineForward = new M06_LimitLineForward(typeSelectList);
                formLineForward.ShowDialog();
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
        /// 参照登録
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolReferenceSubmit_Click(object sender, EventArgs e)
        {
			try
			{
				dgvItems.EndEdit();

                //複数型番が一覧表示されているため、選択フラグが付いたレコードで型番が一意にならなければエラーとする
                string materialCD = string.Empty;
                var mateList = ((SortableBindingList<PlmInfo>)bsLimit.DataSource).Where(p => p.ChangeFG).Select(p => new { MaterialCD = p.MaterialCD }).Distinct();
                if (mateList.Count() > 1)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_104);
                }


				List<PlmPrimaryKeyValue> selectPrimaryKeyValueList = new List<PlmPrimaryKeyValue>();
                bool programMaterialCdFG = false;

                //閾値リストの選択フラグが付いたレコードの主キーを取得
                foreach (PlmInfo plmInfo in ((BindingList<PlmInfo>)bsLimit.DataSource).Where(p => p.ChangeFG).ToList())
				{
					PlmPrimaryKeyValue primaryKeyValue = new PlmPrimaryKeyValue();

					primaryKeyValue.modelNM = plmInfo.ModelNM;
					primaryKeyValue.qcParamNo = plmInfo.QcParamNO;
                    materialCD = plmInfo.MaterialCD;
                    programMaterialCdFG = plmInfo.ProgramMaterialCdFG;

					selectPrimaryKeyValueList.Add(primaryKeyValue);
				}
				if (selectPrimaryKeyValueList.Count == 0)
				{
					throw new ApplicationException(Constant.MessageInfo.Message_97);
				}

                M07_LimitReference formLimitReference = new M07_LimitReference(materialCD, selectPrimaryKeyValueList, this._FunctionStyle, programMaterialCdFG);
		
				this.Close();
				this.Dispose();
                
				formLimitReference.ShowDialog();
				
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

        #region 補足情報

        /// <summary>
        /// ダイス個数取得
        /// </summary>
        /// <param name="typeCD"></param>
        /// <returns></returns>
        private string getDiceCount(string typeCD)
        {
            string retv = string.Empty;

            if (string.IsNullOrEmpty(typeCD) == false)
            {
                int diceCount = ConnectQCIL.GetDiceCount(typeCD);
                if (diceCount != int.MinValue)
                {
                    retv = diceCount.ToString();
                }
            }

            return retv;
        }

        /// <summary>
        /// ログファイル紐付け区分出力
        /// </summary>
        private void outputFileFmtStatus(string typeCD)
        {
            if (string.IsNullOrEmpty(typeCD))
            {
				txtlogFile.Text = string.Empty;
                return;
            }

            List<FileFmtTypeInfo> fileFmtTypeList = ConnectQCIL.GetFILEFMTTYPEData(typeCD);
            if (fileFmtTypeList.Count == 0)
            {
                txtlogFile.Text = Constant.MessageInfo.Message_80;
                txtlogFile.ForeColor = Color.Red;
            }
            else
            {
                txtlogFile.Text = Constant.MessageInfo.Message_85;
                txtlogFile.ForeColor = SystemColors.WindowText;
            }
        }

        #endregion

        /// <summary>
        /// グリッドセル値変更時
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dgvItems_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == - 1) 
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
        /// 
        /// </summary>
        /// <param name="plmList"></param>
        /// <returns></returns>
        private void GetLimitData(SortableBindingList<PlmInfo> plmList, out object[,] valArray, out bool[,] celColorArray)
        {
            plmList = plmList.OrderBy(p => p.MaterialCD + "," + p.ModelNM + "," + p.ClassNM + "," + p.ParameterNM).ToSortableBindingList();
            int colCount = 25;
            int parameterIndex = 10;
            if (this.isResinGroupManageCondition == true)
            {
                colCount++;
                parameterIndex++;
            }


            object[,] limitData = new object[plmList.Count, colCount];
			bool[,] colorFgData = new bool[plmList.Count, colCount];
            for (int i = 0; i < plmList.Count; i++)
            {
                limitData[i, 0] = plmList[i].MaterialCD;
                limitData[i, 1] = plmList[i].QcParamNO;
                limitData[i, 2] = plmList[i].ModelNM;
                limitData[i, 3] = plmList[i].ClassNM;
                limitData[i, 4] = plmList[i].ParameterNM;
                limitData[i, 5] = plmList[i].ChipNM;
                limitData[i, 6] = plmList[i].DiceCT;
                limitData[i, 7] = plmList[i].TimingNM;
                limitData[i, 8] = plmList[i].ManageNM;
				limitData[i, 9] = plmList[i].EquipmentNO;
				if (plmList[i].EquipManageFG != 0)
				{
					colorFgData[i, 9] = true;
				}
                if (this.isResinGroupManageCondition == true)
                {
                    limitData[i, 10] = "'" + plmList[i].ResinGroupCD;
                    colorFgData[i, 10] = true;
                }

                limitData[i, parameterIndex + 0] = plmList[i].ParameterMAX;
                limitData[i, parameterIndex + 2] = plmList[i].ParameterMIN;
                limitData[i, parameterIndex + 4] = plmList[i].ParameterVAL;
                limitData[i, parameterIndex + 6] = plmList[i].QcLinePNT;
                limitData[i, parameterIndex + 8] = plmList[i].QcLineMAX;
                limitData[i, parameterIndex + 10] = plmList[i].QcLineMIN;
                limitData[i, parameterIndex + 12] = plmList[i].ParamGetUpperCond;
                limitData[i, parameterIndex + 14] = plmList[i].ParamGetLowerCond;
            }
			valArray = limitData;
			celColorArray = colorFgData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="plmList"></param>
        /// <returns></returns>
        private object[,] GetParamData(List<ParamInfo> paramList)
        {
            paramList = paramList.OrderBy(p => p.ModelNM + "," + p.ClassNM + "," + p.ParameterNM).ToList();

            int colCount = 24;
            if (this.isResinGroupManageCondition == true) { colCount++; }

            object[,] limitData = new object[paramList.Count, colCount];
            for (int i = 0; i < paramList.Count; i++)
            {
                limitData[i, 1] = paramList[i].QcParamNO;
                limitData[i, 2] = paramList[i].ModelNM;
                limitData[i, 3] = paramList[i].ClassNM;
                limitData[i, 4] = paramList[i].ParameterNM;
                limitData[i, 5] = paramList[i].ChipNM;
                limitData[i, 7] = paramList[i].TimingNM;
                limitData[i, 8] = paramList[i].ManageNM;
            }
            return limitData;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        private void ChangeFormStyle(FormStyle style)
        {
            switch (style)
            {
                case FormStyle.Normal:
                    toolExcelOutput.Enabled = false;
                    toolAdd.Enabled = false;
                    break;

                case FormStyle.Input:
                    toolExcelOutput.Enabled = true;
                    toolAdd.Enabled = true;
                    break;

                case FormStyle.Submit:
                    toolExcelOutput.Enabled = false;
                    toolAdd.Enabled = false;
                    break;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="style"></param>
        private void ChangeFunctionStyle(FunctionStyle style)
        {
            switch (style)
            {
                case FunctionStyle.Read:
                    toolAdd.Visible = false;
                    toolExcelRead.Visible = false;
                    toolPreserve.Visible = false;
                    toolReferenceSubmit.Visible = false;
                    toolLineForward.Visible = false;
                    toolDelFgOn.Visible = false;
                    toolDelFgOff.Visible = false;
                    toolLineForward.Visible = false;
                    toolReferenceSubmit.Visible = false;
                    this.Text += Constant.MessageInfo.Message_77;
                    this.isResinGroupManageCondition = false;
                    break;

                case FunctionStyle.Write:
                    toolAdd.Visible = true;
                    toolExcelRead.Visible = true;
                    toolPreserve.Visible = true;
                    toolDelFgOn.Visible = true;
                    toolDelFgOff.Visible = true;
                    toolLineForward.Visible = true;
                    toolReferenceSubmit.Visible = true;
                    this.Text += Constant.MessageInfo.Message_76;
                    this.isResinGroupManageCondition = false;
                    break;

                case FunctionStyle.InnerLimitEdit:
                    toolAdd.Visible = false;
                    toolExcelRead.Visible = false;
                    toolPreserve.Visible = true;
                    toolDelFgOn.Visible = true;
                    toolDelFgOff.Visible = true;
                    toolLineForward.Visible = false;
                    toolReferenceSubmit.Visible = false;

                    foreach (DataGridViewColumn col in dgvItems.Columns)
                    {
                        if (col.Name == "InnerUpperLimit" || col.Name == "InnerLowerLimit"
                            || col.Name == "ChangeFG" || col.Name == "ReasonVAL")
                        {
                            col.ReadOnly = false;
                            col.DefaultCellStyle.BackColor = SystemColors.Info;
                        }
                        else 
                        { 
                            col.ReadOnly = true;
                            col.DefaultCellStyle.BackColor = Color.White;
                        }
                    }

                    this.Text += Constant.MessageInfo.Message_105;
                    this.isResinGroupManageCondition = false;
                    break;

                case FunctionStyle.PhosphorSheetRead:
                    toolAdd.Visible = false;
                    toolExcelRead.Visible = false;
                    toolPreserve.Visible = false;
                    toolReferenceSubmit.Visible = false;
                    toolLineForward.Visible = false;
                    toolDelFgOn.Visible = false;
                    toolDelFgOff.Visible = false;
                    toolLineForward.Visible = false;
                    toolReferenceSubmit.Visible = false;
                    this.Text += Constant.MessageInfo.Message_107;
                    this.label3.Visible = true;
                    this.cmbResinGroup.Visible = true;
                    this.cmbResinGroup.DataSource = ConnectQCIL.GetPLMResinGroupCD();
                    this.cmbResinGroup.Text = string.Empty;
                    this.btnResinGruopMulti.Visible = true;
                    this.isResinGroupManageCondition = true;
                    foreach (DataGridViewColumn col in dgvItems.Columns)
                    {
                        if (col.Name == "ResinGroupCD")
                        {
                            col.Visible = true;
                            col.DefaultCellStyle.BackColor = SystemColors.Info;
                        }
                        if (col.Name == "Info3NM" || col.Name == "DiceCT"
                            || col.Name == "InnerUpperLimit" || col.Name == "InnerLowerLimit" || col.Name == "QcLinePNT"
                            || col.Name == "QcLineMAX" || col.Name == "QcLineMIN")
                        {
                            col.Visible = false;
                        }
                    }
                    break;

                case FunctionStyle.PhosphorSheetWrite:
                    toolAdd.Visible = true;
                    toolExcelRead.Visible = true;
                    toolPreserve.Visible = true;
                    toolDelFgOn.Visible = true;
                    toolDelFgOff.Visible = true;
                    toolLineForward.Visible = true;
                    toolReferenceSubmit.Visible = true;
                    this.Text += Constant.MessageInfo.Message_108;
                    this.label3.Visible = true;
                    this.cmbResinGroup.Visible = true;
                    this.cmbResinGroup.DataSource = ConnectQCIL.GetPLMResinGroupCD();
                    this.cmbResinGroup.Text = string.Empty;
                    this.btnResinGruopMulti.Visible = true;
                    this.isResinGroupManageCondition = true;
                    foreach (DataGridViewColumn col in dgvItems.Columns)
                    {
                        if (col.Name == "ResinGroupCD")
                        {
                            col.Visible = true;
                            col.DefaultCellStyle.BackColor = SystemColors.Info;
                        }
                        if (col.Name == "Info3NM" || col.Name == "DiceCT"
                            || col.Name == "InnerUpperLimit" || col.Name == "InnerLowerLimit" || col.Name == "QcLinePNT"
                            || col.Name == "QcLineMAX" || col.Name == "QcLineMIN")
                        {
                            col.Visible = false;
                        }
                    }
                    break;
            }
            if (Constant.fClient)
            {
                toollblServer.Text = Constant.EmployeeInfo.LoginServerNM;
                toollblEmp.Text = Constant.EmployeeInfo.EmployeeCD;
            }
            else
            {
                statusStrip.Visible = false;
            }
		}

		private void dgvItems_KeyPress(object sender, KeyPressEventArgs e)
		{
			if (e.KeyChar == ' ')
			{
				foreach (DataGridViewRow row in dgvItems.SelectedRows)
				{
					row.Cells["ChangeFG"].Value = !(bool)row.Cells["ChangeFG"].Value;
				}
			}
		}

        private void dgvItems_SelectionChanged(object sender, EventArgs e)
        {
            string materialCD = string.Empty;
            if (dgvItems.SelectedRows.Count == 1)
            {
                materialCD = dgvItems.SelectedRows[0].Cells["MaterialCD"].Value.ToString();
            }

            outputFileFmtStatus(materialCD);
		}

		private void M01_Limit_FormClosed(object sender, FormClosedEventArgs e)
		{

			ExcelControl.CloseProcess();
		}

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

		private void DisableExcelMenu()
		{
			latestEnableStatustoolExcelOutput = toolExcelOutput.Enabled;
			latestEnableStatustoolExcelOutputAll = toolExcelOutputAll.Enabled;
			latestEnableStatustoolExcelRead = toolExcelRead.Enabled;

			toolExcelOutput.Enabled = false;
			toolExcelOutputAll.Enabled = false;
			toolExcelRead.Enabled = false;
		}

		private void ReturnEnableStatusExcelMenu()
		{
			toolExcelOutput.Enabled = latestEnableStatustoolExcelOutput;
			toolExcelOutputAll.Enabled = latestEnableStatustoolExcelOutputAll;
			toolExcelRead.Enabled = latestEnableStatustoolExcelRead;
		}

		private void toolStripButton2_Click(object sender, EventArgs e)
		{
			try
			{
				dgvItems.EndEdit();

				SortableBindingList<PlmInfo> sbPlmInfoList = (SortableBindingList<PlmInfo>)bsLimit.DataSource;
				MasterChk.ChkParamRelatedMasterFromPLM(Constant.EmployeeInfo.ServerInstance, sbPlmInfoList.ToList());

			}
			catch (Exception err)
			{
				MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void cbDisplayDisableType_CheckedChanged(object sender, EventArgs e)
		{
			cmbType.DataSource = ConnectQCIL.GetPLMTypeCD(cbDisplayDisableType.Checked, this.isResinGroupManageCondition, this.chkProgMat.Checked);
			cmbType.Text = string.Empty;
		}

        /// <summary>
        /// 樹脂Gr複数選択
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnResinGruopMulti_Click(object sender, EventArgs e)
        {
            List<string> resinGroupList = ConnectQCIL.GetPLMResinGroupCD();

            M08_MultiSelect formMultiSelect = new M08_MultiSelect(resinGroupList, resinGroupSelectList);
            formMultiSelect.ShowDialog();

            resinGroupSelectList = formMultiSelect.SelectItemList;
            if (resinGroupSelectList.Count == 0)
            {
                btnResinGruopMulti.ImageIndex = 0;
                cmbResinGroup.Enabled = true;
            }
            else
            {
                btnResinGruopMulti.ImageIndex = 1;
                cmbResinGroup.Enabled = false;
                cmbResinGroup.Text = string.Empty;
            }
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {

        }

        private void chkProgMat_CheckedChanged(object sender, EventArgs e)
        {
            cmbType.DataSource = ConnectQCIL.GetPLMTypeCD(cbDisplayDisableType.Checked, this.isResinGroupManageCondition, this.chkProgMat.Checked);
            cmbType.Text = string.Empty;
        }
    }
}
