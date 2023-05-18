using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace GEICS
{
    public partial class M07_LimitReference : Form
    {
		/// <summary>選択した参照型番のリスト</summary>
		public List<string> typeSelectList { get; set; }

        private string TargetTypeCD = string.Empty;
		private List<PlmPrimaryKeyValue> primaryKeyValueList;
        private M01_Limit.FunctionStyle m01style;
        private bool? isProgramNmMatCd;

		public M07_LimitReference(string targetTypeCD, List<PlmPrimaryKeyValue> plmPrimaryKeyValueList, M01_Limit.FunctionStyle m01FunctionStyle, bool? prmProgramNmMatCd)
        {
            InitializeComponent();
            TargetTypeCD = targetTypeCD;
			typeSelectList = new List<string>();
			primaryKeyValueList = plmPrimaryKeyValueList;
            m01style = m01FunctionStyle;
            isProgramNmMatCd = prmProgramNmMatCd;
        }

        /// <summary>
        /// フォームロード
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void M07_LimitReference_Load(object sender, EventArgs e)
        {
            try
            {
                toolTypeCD.Text = TargetTypeCD;
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
        /// 保存
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolPreserve_Click(object sender, EventArgs e)
        {
            grdItems.EndEdit();

            try
            {
                if (txtReason.Text == string.Empty) 
                {
                    throw new ApplicationException(string.Format(Constant.MessageInfo.Message_91, "ALL"));
                }

                //参照元閾値取得
                List<PlmInfo> orignalPlmList = ConnectQCIL.GetPLMData(toolTypeCD.Text, true);

				orignalPlmList = GetQCnumVal(orignalPlmList);

				List<PlmInfo> targetPlmList = new List<PlmInfo>();
				//選択された参照元の閾値情報に限定
				foreach (PlmPrimaryKeyValue primaryKeyValue in primaryKeyValueList)
				{
					targetPlmList.AddRange(orignalPlmList.Where(plm => plm.ModelNM == primaryKeyValue.modelNM && plm.QcParamNO == primaryKeyValue.qcParamNo));
				}

                //保存先閾値対象取得
				if (bsItems.Count == 0) 
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }
                List<string> pTypeList = ((BindingList<MultiSelect>)bsItems.List)
                    .Where(t => t.SelectFG).Select(t => t.ItemNM).ToList();


                foreach (string pType in pTypeList)
                {
                    //対象製品型番の閾値を参照登録
					ReferenceLimit(toolTypeCD.Text, pType, targetPlmList);
                }

                MessageBox.Show(Constant.MessageInfo.Message_63, "Infomation", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

		private List<PlmInfo> GetQCnumVal(List<PlmInfo> plmList)
		{
			foreach (PlmInfo plmInfo in plmList)
			{
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

			return plmList;
		}

        /// <summary>
        /// 閾値を参照登録
        /// </summary>
        /// <param name="type"></param>
        /// <param name="plmList"></param>
		private void ReferenceLimit(string fromType, string toType, List<PlmInfo> plmList) 
        {
            int diceCT = ConnectQCIL.GetDiceCount(fromType);

            using (ConnectQCIL conn = new ConnectQCIL(true, Constant.StrQCIL))
            {
                try
                {
					if (ConnectQCIL.GetDiceCount(toType) == int.MinValue)
					{
						conn.InsertUpdateDIECT(toType, Convert.ToInt32(diceCT));
					}

                    foreach (PlmInfo plmInfo in plmList)
                    {
                        plmInfo.MaterialCD = toType;
                        plmInfo.ReasonVAL = txtReason.Text;

                        conn.InsertUpdatePLM(plmInfo);
						conn.InsertPLMHist(plmInfo);

                        if (plmInfo.QcLinePNT != null)
                        {
                            conn.InsertUpdateQCST(plmInfo);
                        }
                    }

                    conn.Connection.Commit();
                }
                catch (SqlException err)
                {
                    conn.Connection.Rollback();
					throw new ApplicationException(err.Message, err);
                }
            }
        }

        /// <summary>
        /// 新規追加
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void toolAdd_Click(object sender, EventArgs e)
        {
            bsItems.Add(new MultiSelect(true, string.Empty));
        }

		/// <summary>型番参照</summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		private void toolTypeRef_Click(object sender, EventArgs e)
		{
			grdItems.EndEdit();

            bool isResinGroupManageCondition = false;
            if (m01style == M01_Limit.FunctionStyle.PhosphorSheetRead || m01style == M01_Limit.FunctionStyle.PhosphorSheetWrite)
            {
                isResinGroupManageCondition = true;
            }
            List<string> allTypeList = ConnectQCIL.GetPLMTypeCD(true, isResinGroupManageCondition, this.isProgramNmMatCd);
			List<string> typeList = allTypeList;

			//参照登録元型番を型番参照のリストから型番を除外
			typeList.Remove(TargetTypeCD);

			//登録対象のリストでチェックが付いている型番を型番参照のリストから型番を除外
			foreach (string type in bsItems.Cast<MultiSelect>().Where(b => b.SelectFG == true).Select(b => b.ItemNM).ToList())
			{
				if (allTypeList.Exists(atl => atl == type))
				{
					typeList.Remove(type);
				}
			}

			M08_MultiSelect formMultiSelect = new M08_MultiSelect(typeList, new List<string>());
			formMultiSelect.ShowDialog();

			typeSelectList = formMultiSelect.SelectItemList;

			//登録対象リスト、参照登録リスト両方でチェックが外れていた場合、登録対象リストから型番を除外
			foreach (string type in bsItems.Cast<MultiSelect>().Where(b => b.SelectFG == false).Select(b => b.ItemNM).ToList())
			{
				int index = bsItems.Cast<MultiSelect>().ToList().FindIndex(b => b.ItemNM == type);
				bsItems.List.RemoveAt(index);
			}

			foreach (string type in typeSelectList)
			{
				bsItems.Add(new MultiSelect(true, type));
			}
		}

		/// <summary>登録対象のリストに追加するべきタイプかどうか調べる</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsAddTargetType(string type)
		{
			if(IsSelectType(type))
			{
				if(!IsThereTypeTargetList(type))
				{
					return true;
				}
			}
			return false;
		}

		/// <summary>型番参照のリストで選択されたタイプかどうかを調べる</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsSelectType(string type)
		{
			//選択済みタイプリストにtypeが存在しない場合
			if (typeSelectList.Where(tsl => tsl == type).Count() == 0)
			{
				return false;
			}
			return true;
		}

		/// <summary>登録対象のリストにタイプが存在するか調べる</summary>
		/// <param name="type"></param>
		/// <returns></returns>
		private bool IsThereTypeTargetList(string type)
		{
			if (bsItems.Count == 0)
			{
				return false;
			}

			//M07のタイプリストにtypeが存在しない場合
			if (bsItems.Cast<MultiSelect>().Where(bs => bs.ItemNM == type).Count() == 0)
			{
				return false;
			}
			return true;
		}

		private void toolExcelLoad_Click(object sender, EventArgs e)
		{
			try
			{
				if (DialogResult.OK != openFileDialog.ShowDialog())
				{
					return;
				}

				List<string> targetTypeList = new List<string>();
				using (ExcelControl excel = ExcelControl.GetInstance())
				{
					targetTypeList = excel.GetTypeList(openFileDialog.FileName);
					if (targetTypeList.Count == 0)
					{
						return;
					}
				}

				foreach (string typeCD in targetTypeList)
				{
					bsItems.Add(new MultiSelect(true, typeCD));
				}
			}
			catch (Exception err)
			{
				MessageBox.Show(string.Format("{0}\r\n{1}", err.Message, err.StackTrace), "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		/// <summary>
		/// データ貼り付け
		/// </summary>
		private void inputData()
		{
			//クリップボードのデータを取得
			IDataObject data = Clipboard.GetDataObject();
			object clipboardObject = data.GetData(DataFormats.Text);

			string[] rowsClipboardObject = Regex.Split(clipboardObject.ToString(), "\r\n");

			inputData(rowsClipboardObject);
		}

		/// <summary>
		/// データ貼り付け
		/// </summary>
		/// <param name="rowsClipboardObject"></param>
		private void inputData(string[] rowsClipboardObject)
		{
			//int rowIndex = this.grdItems.CurrentCell.RowIndex;

			try
			{
				bsItems.Clear();


				for (int i = 0; i < rowsClipboardObject.Length; i++)
				{
					if (rowsClipboardObject[i] != "")
					{
						string[] cellsClipboardObject = Regex.Split(rowsClipboardObject[i], "\t");

						if (cellsClipboardObject.Length >= 2)
						{
							MessageBox.Show(Constant.MessageInfo.Message_100, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Error);
							return;
						}

						bsItems.Add(new MultiSelect(true, rowsClipboardObject[i].Trim()));

						//bool newRowFG = false;

						//DataRow dr = null;
						//try
						//{
						//    dr = this.dataSet.MultiInfo.Rows[rowIndex];
						//}
						//catch
						//{
						//    newRowFG = true;
						//    dr = this.dataSet.MultiInfo.NewRow();
						//}

						//dr["Code"] = cellsClipboardObject[0].ToString().Trim();

						//if (newRowFG)
						//{
						//    this.dataSet.MultiInfo.Rows.Add(dr);
						//}

						//rowIndex++;
					}
				}

				this.bsItems.EndEdit();
				this.grdItems.EndEdit();
			}
			catch
			{
				MessageBox.Show(Constant.MessageInfo.Message_100, "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}
		}

		private void toolPaste_Click(object sender, EventArgs e)
		{
			inputData();
		}

		private void M07_LimitReference_FormClosing(object sender, FormClosingEventArgs e)
		{
            M01_Limit formMasterLimit = new M01_Limit(m01style);
            //M01_Limit formMasterLimit = null;
            //if (Constant.EmployeeInfo.UserFunctionList.Exists(u => u.FunctionCD == Convert.ToString(Constant.Function.F001)))
            //{
            //    formMasterLimit = new M01_Limit(M01_Limit.FunctionStyle.Write);
            //}
            //else 
            //{
            //    formMasterLimit = new M01_Limit(M01_Limit.FunctionStyle.Read);
            //}            

            this.Dispose();
			formMasterLimit.ShowDialog();
		}

    }
}
