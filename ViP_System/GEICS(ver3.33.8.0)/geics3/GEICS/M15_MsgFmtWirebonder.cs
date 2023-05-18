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
    public partial class M15_MsgFmtWirebonder : Form
    {
        public M15_MsgFmtWirebonder()
        {
            InitializeComponent();
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

                List<Database.MachineLogFormat.WirebonderSecs> formatList
                    = Database.MachineLogFormat.WirebonderSecs.GetData(txtModelNM.Text.Trim(), qcParamNo);
                bsItems.DataSource = formatList;
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
        private void toolAdd_Click(object sender, EventArgs e)
        {
            Database.MachineLogFormat.WirebonderSecs newItem = new Database.MachineLogFormat.WirebonderSecs();
            newItem.ChangeFG = true;
            newItem.UpdUserCD = Constant.EmployeeInfo.EmployeeCD;
            newItem.LastUpdDT = DateTime.Now;
            bsItems.Add(newItem);
        }

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

                Database.MachineLogFormat.WirebonderFile newItem = new Database.MachineLogFormat.WirebonderFile();
                newItem.ChangeFG = true;

                newItem.PrefixNM = clipChars[0].ToString().Trim();

                int qcParamNo;
                if (!int.TryParse(clipChars[1].ToString().Trim(), out qcParamNo))
                {
                    throw new Exception(string.Format("管理Noには数値を入力して下さい。値:{0}", clipChars[1]));
                }
                newItem.QcParamNO = qcParamNo;

                int fileFmtNo;
                if (!int.TryParse(clipChars[2].ToString().Trim(), out fileFmtNo))
                {
                    throw new Exception(string.Format("紐付け番号には数値を入力して下さい。値:{0}", clipChars[2]));
                }
                newItem.FileFmtNO = fileFmtNo;

                newItem.ModelNM = clipChars[3].ToString().Trim();

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

                newItem.DelFG = Convert.ToBoolean(int.Parse(clipChars[8].ToString().Trim()));
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
                List<Database.MachineLogFormat.WirebonderSecs> changeList
                    = ((List<Database.MachineLogFormat.WirebonderSecs>)bsItems.DataSource).Where(p => p.ChangeFG).ToList();
                if (changeList.Count == 0)
                {
                    throw new ApplicationException(Constant.MessageInfo.Message_96);
                }

                foreach (Database.MachineLogFormat.WirebonderSecs change in changeList)
                {
                    if (string.IsNullOrEmpty(change.FmtTypeCD)) change.FmtTypeCD = change.OldFmtTypeCD;
                    if (change.FmtNO == 0) change.FmtNO = change.OldFmtNO;
                    if (string.IsNullOrEmpty(change.ModelNM)) change.ModelNM = change.OldModelNM;
                    if (change.QcParamNO == 0) change.QcParamNO = change.OldQcParamNO;

                    Database.MachineLogFormat.WirebonderSecs.InsertUpdate(change);
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
