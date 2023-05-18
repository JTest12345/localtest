/*************************************************************************************
 * システム名     : 流動検証サポートシステム
 *  
 * 処理名         : FVDC300 設備切替
 * 
 * 概略           : 設備の切替を簡単に可能とする
 * 
 * 作成           : 2020/01/28 SLA2.Uchida
 * 
 * 修正履歴       : 
 ************************************************************************************/

using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace FVDC
{
    public partial class FVDC300 : Form
    {
        private FVDC310 FVDC310         = new FVDC310();
        private DsSqlData dsGridView    = new DsSqlData();
        private int SelRow              = -1;

        public FVDC300()
        {
            InitializeComponent();            
        }

        /// <summary>
        /// 画面がロードされたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FVDC300_Load(object sender, EventArgs e)
        {
            /// 設備テーブルに定義されているラインを検索する
            string WhereSql                         = " WHERE (plantcd <> '') AND (plantcd <> '-') AND (NOT([lineno] IS NULL)) ORDER BY [lineno] ";
            DsName dsMacLine                        = new DsName();
            CommonRead objCommonRead                = new CommonRead();
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                objCommonRead.NameRead(sqlInfo, "TmMachine", "[lineno]", "[lineno]", WhereSql, true, ref dsMacLine);
            }
            string macgroup                         = "";
            this.dsLine.Name.Rows.Clear();
            this.dsLine.Name.Rows.Add("", "");
            for (int i = 0; i < dsMacLine.Name.Rows.Count; i++)
            {
                string strLine                      = dsMacLine.Name[i].Key_CD.Trim().Replace("J1","J01").Replace("J2", "J02").Replace("J3", "J03").Replace("J4", "J04").Replace("J5", "J05").Replace("J6", "J06").Replace("J7", "J07").Replace("J8", "J08").Replace("J9", "J09");
                strLine                             = strLine.Replace("JA", "J10").Replace("JB", "J11").Replace("JC", "J12").Replace("JD", "J13").Replace("JE", "J14").Replace("JF", "J15").Replace("JG", "J16").Replace("JH", "J17").Replace("JI", "J18").Replace("JJ", "J19").Replace("JK", "J20");
                strLine                             = strLine.Replace("JL", "J21").Replace("JM", "J22").Replace("JN", "J23").Replace("JO", "J24").Replace("JP", "J25").Replace("JQ", "J26").Replace("JR", "J27").Replace("JS", "J28").Replace("JT", "J29").Replace("JU", "J30").Replace("JV", "J31");
               
                if  (!macgroup.Contains(strLine))
                {
                    macgroup                        += "," + strLine;
                    this.dsLine.Name.Rows.Add(strLine ,strLine);
                }
            }
            this.cmbLine.DataSource                 = this.dsLine;
            this.dsFVDC300.Machine.Rows.Clear();
            this.dataGridView.DataSource            = this.dsFVDC300;
            Transfer.dsDeviceID                     = new DsFree();
        }
        /// <summary>
        /// 検索を押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnFind_Click(object sender, EventArgs e)
        {
            string LineNM                           = this.cmbLine.SelectedValue.ToString();
            /// 入力が無いとき処理しない
            if ((LineNM == "")
                && (this.txtDeviceID.Text.Trim() == "")
                && (Transfer.dsDeviceID.List.Rows.Count == 0)) return;

            CommonRead objCommonRead                = new CommonRead();
            CheckBox chkTableNM                     = new CheckBox();
            string WhereSql                         = "";
            string KeyNM                            = "";
            string EmpList                          = "@";

            /// 対象ラインの選択が無いとき
            if (LineNM == "")
            {
                switch (this.lblDeviceID.Text)
                {
                    case "設備番号":
                        KeyNM                       = "plantcd";
                        if (Transfer.dsDeviceID.List.Rows.Count == 0)
                        {
                            WhereSql                = " = '" + this.txtDeviceID.Text.Trim() + "' ";
                        }
                        else
                        {
                            for (int i = 0; i < Transfer.dsDeviceID.List.Rows.Count; i++)
                            {
                                EmpList             += ",'" + Transfer.dsDeviceID.List[i].Name + "'";
                            }
                            EmpList                 = EmpList.Replace("@,", "").Replace("@", "");
                            WhereSql                = " IN(" + EmpList + ") ";
                        }
                        break;
                    case "号　機":
                        KeyNM                       = "plantnm";
                        WhereSql                    = " = '" + this.txtDeviceID.Text.Trim() + "' "
                                                    + " AND plantcd <> '' AND plantcd <> '-' "
                                                    + " AND macgroup <> '9999' ";
                        break;
                    case "マシン№":
                        KeyNM                       = "macno";
                        if (Transfer.dsDeviceID.List.Rows.Count == 0)
                        {
                            WhereSql                = " = " + this.txtDeviceID.Text.Trim() + " ";
                        }
                        else
                        {
                            EmpList                 = "@";
                            for (int i = 0; i < Transfer.dsDeviceID.List.Rows.Count; i++)
                            {
                                EmpList             += "," + Transfer.dsDeviceID.List[i].Name;
                            }
                            EmpList                 = EmpList.Replace("@,", "").Replace("@", "");
                            WhereSql                = " IN(" + EmpList + ") "
                                                    + " AND plantcd <> '' AND plantcd <> '-' ";
                        }
                        /// 設備番号を取得する
                        DsName dsEmpList            = new DsName();
                        using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
                        {
                            if (objCommonRead.NameRead(sqlInfo, "TmMachine", "plantcd", "plantcd", "WHERE macno" + WhereSql, true, ref dsEmpList))
                            {
                                EmpList             = "@";
                                for (int i = 0; i < dsEmpList.Name.Rows.Count; i++)
                                {
                                    EmpList         += ",'" + dsEmpList.Name[i].Data_NM + "'";
                                }
                                EmpList             = EmpList.Replace("@,", "").Replace("@", "");
                                KeyNM               = "plantcd";
                                WhereSql            = " IN(" + EmpList + ") ";
                            }
                        }
                        break;
                }
            }
            /// 対象ラインを選択したとき
            else
            {
                /// 高生産性
                if (LineNM.Length > 3)
                {
                    KeyNM                           = "macgroup";
                    WhereSql                        = " = '9999' "
                                                    //+ " AND [lineno] = '" + LineNM + "'"
                                                    + " AND plantcd <> '' AND plantcd <> '-' ";
                }
                /// Jobショップ
                else if (LineNM.Contains("J"))
                {
                    KeyNM                           = "[lineno]";
                    string RepLine                  = LineNM.Replace("J10", "JA").Replace("J11", "JB").Replace("J12", "JC").Replace("J13", "JD").Replace("J14", "JE").Replace("J15", "JF").Replace("J16", "JG").Replace("J17", "JH").Replace("J18", "JI").Replace("J19", "JJ").Replace("J20", "JK");
                    RepLine                         = RepLine.Replace("J21", "JL").Replace("J22", "JM").Replace("J23", "JN").Replace("J24", "JO").Replace("J25", "JP").Replace("J26", "JQ").Replace("J27", "JR").Replace("J28", "JR").Replace("J29", "JT").Replace("J30", "JU").Replace("J31", "JV").Replace("0", "");
                    WhereSql                        = " = '" + RepLine + "' "
                                                    + " AND plantcd <> '' AND plantcd <> '-' ";
                }
                /// 旧自動搬送
                else
                {
                    KeyNM                           = "[lineno]";
                    WhereSql                        = " = '" + Convert.ToInt32(LineNM).ToString("00") + "' "
                                                    + " AND plantcd <> '' AND plantcd <> '-' ";
                }
            }

            EmpList                                 = "@";
            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                this.dsGridView                          = new DsSqlData();
                if (objCommonRead.SqlDataSetCreate(sqlInfo, "ARMS", "TmMachine", ref chkTableNM, ref this.dsGridView))
                {
                    /// 設備情報を読み込む
                    WhereSql                        += " ORDER BY plantcd, macno";
                    if (objCommonRead.SqlDataFind(sqlInfo, "参照", "TmMachine", KeyNM, WhereSql, ref this.dsGridView))
                    {
                        for (int i = 0; i < this.dsGridView.LIST.Rows.Count; i++)
                        {
                            if (!EmpList.Contains(this.dsGridView.LIST[i]["plantcd"].ToString()))
                            {
                                EmpList             += ",'" + this.dsGridView.LIST[i]["plantcd"].ToString() + "'";
                            }
                        }
                    }
                    /// 読込できなかった高生産性設備を取得する
                    EmpList                         = EmpList.Replace("@,", "").Replace("@", "");
                    if ((KeyNM != "plantcd")
                        && (LineNM != "9999")
                        && (LineNM.Length < 4)
                        && (EmpList != ""))
                    {
                        WhereSql                    = " IN(" + EmpList + ") AND macgroup = '9999' ";
                        if (objCommonRead.SqlDataFind(sqlInfo, "参照", "TmMachine", "plantcd", WhereSql, ref this.dsGridView))
                        {
                        }
                    }
                    /// グリッド表示用編集
                    this.dsFVDC300.Machine.Rows.Clear();
                    for (int i = 0; i < this.dsGridView.LIST.Rows.Count; i++)
                    {
                        /// 削除されていないレコードのとき
                        if (!Convert.ToBoolean(this.dsGridView.LIST[i]["delfg"]))
                        {
                            /// 高生産性コメント編集
                            string AddComment       = "";
                            if ((!this.dsGridView.LIST[i]["clasnm"].ToString().Contains("高"))
                                && (this.dsGridView.LIST[i]["macgroup"].ToString() == "9999"))
                            {
                                AddComment          = "（高）";
                            }
                            /// 自動搬送コメント編集
                            else
                            {
                                /// 工程CDを検索し追加する名称を決定                           
                                string procno       = "";
                                WhereSql            = " WHERE macno = N'" + this.dsGridView.LIST[i]["macno"].ToString() + "' ";
                                objCommonRead.OnceRead(sqlInfo, "TmProMac", "procno", WhereSql, ref procno);
                                string procnm       = "";
                                for (int j = 0; j < Transfer.dsProcess.Name.Rows.Count; j++)
                                {
                                    if (procno == Transfer.dsProcess.Name[j].Key_CD)
                                    {
                                        procnm      = Transfer.dsProcess.Name[j].Data_NM;
                                        break;
                                    }
                                }
                                if (procnm != "")
                                {
                                    AddComment      = "（" + procnm + "）";
                                }
                            }
                            /// グリッドデータ編集
                            this.dsFVDC300.Machine.Rows.Add(
                                this.dsGridView.LIST[i]["plantcd"].ToString(),
                                this.dsGridView.LIST[i]["plantnm"].ToString(),
                                this.dsGridView.LIST[i]["clasnm"].ToString() + AddComment,
                                this.dsGridView.LIST[i]["macno"].ToString(),
                                this.dsGridView.LIST[i]["clasnm"].ToString(),
                                this.dsGridView.LIST[i]["macno"].ToString());
                        }
                    }
                    this.dataGridView.DataSource    = this.dsFVDC300;
                }
            }
        }

        /// <summary>
        /// グリッドの右クリックメニューを表示する直前
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsDataGridView_Opening(object sender, CancelEventArgs e)
        {
            if (SelRow == -1) return;
            string plantcd                          = this.dataGridView["plantcd", SelRow].Value.ToString();
            string AddType                          = "";
            string OldClasnm                        = "";
            this.cmsDataGridView.Items.Clear();
            int ListCT                              = 0;

            CommonRead objCommonRead                = new CommonRead();

            using (IConnection sqlInfo = ServerConnection.CreateInstance(Constant.StrARMSDB, false))
            {
                for (int i = 0; i < dsGridView.LIST.Rows.Count; i++)
                {
                    if (dsGridView.LIST[i]["plantcd"].ToString() == plantcd)
                    {
                        /// 高生産性コメント編集
                        AddType = "";
                        if ((dsGridView.LIST[i]["macgroup"].ToString() == "9999")
                            && (!dsGridView.LIST[i]["clasnm"].ToString().Contains("高")))
                        {
                            AddType                 = "（高）";
                        }
                        /// 自動搬送コメント編集
                        else
                        {
                            /// 工程CDを検索し追加する名称を決定
                            string procno           = "";
                            string WhereSql         = " WHERE macno = N'" + this.dsGridView.LIST[i]["macno"].ToString() + "' ";
                            objCommonRead.OnceRead(sqlInfo, "TmProMac", "procno", WhereSql, ref procno);
                            string procnm           = "";
                            for (int j = 0; j < Transfer.dsProcess.Name.Rows.Count; j++)
                            {
                                if (procno == Transfer.dsProcess.Name[j].Key_CD)
                                {
                                    procnm          = Transfer.dsProcess.Name[j].Data_NM;
                                    break;
                                }
                            }
                            if (procnm != "")
                            {
                                AddType             = "（" + procnm + "）";
                            }
                        }
                        /// 選択リストに追加
                        ToolStripMenuItem tsmItem   = new ToolStripMenuItem();
                        tsmItem.Name                = dsGridView.LIST[i]["macno"].ToString();
                        tsmItem.Text                = dsGridView.LIST[i]["clasnm"].ToString() + AddType;
                        /// 誤登録データへ切替出来ないように名称が同じデータは無視する
                        if (OldClasnm != tsmItem.Text)
                        {
                            ListCT++;
                            OldClasnm               = tsmItem.Text;
                            this.cmsDataGridView.Items.AddRange(new ToolStripItem[] { tsmItem });
                        }
                    }
                }
            }
            this.cmsDataGridView.Size               = new System.Drawing.Size(200, ListCT * 22 + 4);
        }

        /// <summary>
        /// グリッドの右クリックメニューを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmsDataGridView_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            if (SelRow == -1) return;

            this.dataGridView["macno_NEW", SelRow].Value        = e.ClickedItem.Name;
            this.dataGridView["clasnm_NEW", SelRow].Value       = e.ClickedItem.Text;

            if (this.dataGridView["macno_OLD", SelRow].Value.ToString() == e.ClickedItem.Name)
            {
                this.dataGridView["TextAction", SelRow].Value   = "";
            }
            else
            {
                this.dataGridView["TextAction", SelRow].Value   = "切替";
            }
            this.dataGridView.UpdateCellValue(0, SelRow);
            this.dataGridView.UpdateCellValue(3, SelRow);
            this.dataGridView.UpdateCellValue(4, SelRow);
            this.dataGridView.RefreshEdit();
            this.dataGridView.EndEdit();
            this.dataGridView.Refresh();
            this.dataGridView.Update();
        }

        /// <summary>
        /// グリッド位置の取得
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            SelRow                      = e.RowIndex;
        }

        private void dataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            SelRow                      = e.RowIndex;
        }

        
        /// <summary>
        /// 複数選択ボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnSelect_Click(object sender, EventArgs e)
        {
            /// 詳細画面を表示します。
            FVDC310.ShowDialog();
            
            try
            {
                if (Transfer.dsDeviceID.List.Rows.Count > 0)
                {
                    this.txtDeviceID.Enabled    = false;
                    this.txtDeviceID.Text       = "";
                }
                else
                {
                    this.txtDeviceID.Enabled    = true;
                }
            }
            catch
            {
                this.txtDeviceID.Enabled        = true;
            }
        }
        /// <summary>
        /// 設備番号をダブルクリックしたとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lblDeviceID_DoubleClick(object sender, EventArgs e)
        {
            switch (this.lblDeviceID.Text)
            {
                case "設備番号":
                    /// 設備番号の複数選択をしていないとき
                    if (this.txtDeviceID.Enabled)
                    {
                        this.lblDeviceID.Text   = "号　機";
                        this.btnSelect.Enabled  = false;
                        Transfer.dsDeviceID     = new DsFree();
                    }
                    break;
                case "号　機":
                    this.lblDeviceID.Text       = "マシン№";
                    this.btnSelect.Enabled      = true;
                    break;
                case "マシン№":
                    this.lblDeviceID.Text       = "設備番号";
                    this.btnSelect.Enabled      = true;
                    break;
            }
        }

        /// <summary>
        /// ＯＫボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnOK_Click(object sender, EventArgs e)
        {
            int UpdateCnt                       = 0;
            CommonIO objCommonIO                = new CommonIO();
            for (int i = 0; i < this.dataGridView.Rows.Count; i++)
            {
                if (this.dataGridView["TextAction", i].Value == null) { }
                else if (this.dataGridView["TextAction", i].Value.ToString() == "切替")
                {
                    /// 削除フラグを切り替える
                    string UpdateChar           = " delfg = 1 ";
                    string WhereChar            = " WHERE macno = " + this.dataGridView["macno_OLD", i].Value.ToString() + " ";
                    if (objCommonIO.Update(Constant.StrARMSDB, "TmMachine", UpdateChar, WhereChar))
                    {
                        UpdateChar              = " delfg = 0 ";
                        WhereChar               = " WHERE macno = " + this.dataGridView["macno_NEW", i].Value.ToString() + " ";
                        if (objCommonIO.Update(Constant.StrARMSDB, "TmMachine", UpdateChar, WhereChar))
                        {
                            UpdateCnt++;
                        }
                    }
                }
            }
            if (UpdateCnt > 0)
            {
                /// グリッドの情報を最新に変更する
                this.btnFind_Click(null, null);
            }
        }

        /// <summary>
        /// キャンセルボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.dsFVDC300.Machine.Rows.Clear();
            this.Close();
        }

        /// <summary>
        /// 対象ラインを選択したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmbLine_SelectedIndexChanged(object sender, EventArgs e)
        {
            /// 何か選択したとき
            if (this.cmbLine.SelectedIndex > 0)
            {
                this.lblDeviceID.Text           = "設備番号";
                this.btnSelect.Enabled          = true;
                this.txtDeviceID.Text           = "";
                Transfer.dsDeviceID             = new DsFree();
            }
        }

        /// <summary>
        /// 設備番号の入力が有ったとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void txtDeviceID_TextChanged(object sender, EventArgs e)
        {
            if (this.lblDeviceID.Text.Trim() == "") return;

            this.cmbLine.SelectedIndex          = 0;

            switch (this.lblDeviceID.Text)
            {
                case "設備番号":
                    this.txtDeviceID.Text       = this.txtDeviceID.Text.Trim().ToUpper().Replace("SLC-", "").Replace("Ｓ", "S").Replace("ｓ", "S").Replace("０", "0").Replace("SO", "S0");
                    break;
                case "号　機":
                    break;
                case "マシン№":
                    DataCast objCast            = new DataCast();
                    this.txtDeviceID.Text       = objCast.NumericChange(this.txtDeviceID.Text).ToString();
                    break;
            }
        }
        /// <summary>
        /// クリアボタンを押したとき
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            this.dsFVDC300.Machine.Rows.Clear();
            this.dataGridView.DataSource        = this.dsFVDC300;
            this.cmbLine.SelectedIndex          = 0;
            this.lblDeviceID.Text               = "設備番号";
            this.btnSelect.Enabled              = true;
            this.txtDeviceID.Text               = "";
            Transfer.dsDeviceID                 = new DsFree();
        }
    }
}
