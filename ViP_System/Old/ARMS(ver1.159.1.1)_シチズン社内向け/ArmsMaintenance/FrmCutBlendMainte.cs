using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using System.Data.SqlClient;

namespace ArmsMaintenance
{
    public partial class FrmCutBlendMainte : Form
    {
        private DefItem[] DefectList;

        private CutBlend[] BlendList;

        public string cutlotno;
        public string typecd;

        public Process proc;

        public FrmCutBlendMainte()
        {
            InitializeComponent();
        }

        #region 不良登録関連
        /// <summary>
        /// 不良登録グリッドの初期化
        /// </summary>
        private void setDefectGrid()
        {
            DefItem[] org = Defect.GetAllDefect(this.cutlotno, this.typecd, this.proc.ProcNo);

            if (org == null || org.Length == 0)
            {
                this.grdDefect.DataSource = null;
                return;
            }

            this.DefectList = org.OrderBy(i => i.OrderNo).ToArray();
            this.grdDefect.DataSource = this.DefectList;

            this.grdDefect.Columns["OrderNo"].Visible = false;
            this.grdDefect.Columns["CauseCd"].Visible = false;
            this.grdDefect.Columns["CauseName"].ReadOnly = true;
            this.grdDefect.Columns["ClassCd"].Visible = false;
            this.grdDefect.Columns["ClassName"].ReadOnly = true;
            this.grdDefect.Columns["DefectCd"].ReadOnly = true;
            this.grdDefect.Columns["DefectName"].ReadOnly = true;
            this.grdDefect.Columns["DefectCt"].DefaultCellStyle.BackColor = Color.Yellow;

            countAndSetDefectTotal();
        }


        private void updateDefect()
        {
            Defect def = new Defect();
            def.LotNo = this.cutlotno;
            def.ProcNo = this.proc.ProcNo;
            def.DefItems = new List<DefItem>();

            foreach (DataGridViewRow row in this.grdDefect.Rows)
            {
                DefItem d = new DefItem();
                d.CauseCd = row.Cells["causecd"].Value.ToString();
                d.ClassCd = row.Cells["classcd"].Value.ToString();
                d.DefectCd = row.Cells["defectcd"].Value.ToString();
                d.DefectCt = Convert.ToInt32(row.Cells["defectct"].Value);

                if (d.DefectCt >= 1)
                {
                    def.DefItems.Add(d);
                }
            }

            def.DeleteInsert();


            //子ロットが選択されている場合は、子ロットと親ロットに対して不良登録の記録を残す
            if (this.txtLotNo.Text != this.cutlotno)
            {
                Defect defP = new Defect();
                defP.LotNo = this.txtLotNo.Text;
                defP.ProcNo = this.proc.ProcNo;
                defP.DefItems = new List<DefItem>();

                foreach (CutBlend cb in this.BlendList)
                {
                    DefItem[] org = Defect.GetAllDefect(cb.LotNo, this.typecd, this.proc.ProcNo);

                    foreach (DefItem orgd in org)
                    {
                        DefItem d = defP.DefItems.Where(r => r.CauseCd == orgd.CauseCd &&
                                                r.ClassCd == orgd.ClassCd &&
                                                r.DefectCd == orgd.DefectCd).FirstOrDefault();
                        if (d == null)
                        {
                            d = new DefItem();
                            d.CauseCd = orgd.CauseCd;
                            d.ClassCd = orgd.ClassCd;
                            d.DefectCd = orgd.DefectCd;
                            d.DefectCt = orgd.DefectCt;

                            defP.DefItems.Add(d);
                        }
                        else
                        {
                            d.DefectCt += orgd.DefectCt;
                        }
                    }
                }

                defP.DeleteInsert();
            }
        }

        /// <summary>
        /// 不良合計数のカウント
        /// </summary>
        private void countAndSetDefectTotal()
        {
            int total = 0;

            foreach (DefItem item in this.DefectList)
            {
                total += item.DefectCt;
            }

            this.txtDefTotal.Text = total.ToString();
        }

        /// <summary>
        /// 不良グリッドのフォーマットエラー
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdDefect_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("不良数の値が不正です");
        }

        private void grdDefect_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            countAndSetDefectTotal();
        }
        #endregion

        private void FrmCutBlendMainte_Load(object sender, EventArgs e)
        {
            // フォルダの設定無し or 設定フォルダが存在しない時、ボタンを消す。
            if (string.IsNullOrWhiteSpace(ArmsApi.Config.Settings.OutLabelOutputDirectoryPath) == true ||
                System.IO.Directory.Exists(ArmsApi.Config.Settings.OutLabelOutputDirectoryPath) == false)
            {
                this.btnPrintLabel2.Visible = false;
            }

			txtMacGroup.Text = Properties.Settings.Default.SelectMacGroup;

			SearchBlendLot();
        }

		private void SearchBlendLot() 
		{
			this.treeCutBlend.Nodes.Clear();

            //CutBlend[] all = CutBlend.SearchBlendRecord(null, null, null, false, true);
            CutBlend[] all;
            if (this.rdoPastLot.Checked && string.IsNullOrWhiteSpace(this.txtPastLot.Text.Trim()) == false)
            {
                all = CutBlend.SearchBlendRecord(null, this.txtPastLot.Text.Trim(), null, false, false, null, null, null, null, true);
            }
            else
            {
                all = CutBlend.SearchBlendRecord(null, null, null, false, true);
            }
            if (all.Length == 0) 
			{
				treeCutBlend.Nodes.Clear();
				return;
			}

			if (string.IsNullOrEmpty(txtMacGroup.Text) == false)
			{
				all = all.Where(a => AsmLot.GetAsmLot(a.LotNo) != null 
					&& AsmLot.GetAsmLot(a.LotNo).MacGroup.Contains(txtMacGroup.Text)).ToArray();
			}

			string[] treenode = all.OrderBy(c => c.EndDt).Select(c => c.BlendLotNo).Distinct().ToArray();
			this.treeCutBlend.Nodes.Clear();
			foreach (string node in treenode)
			{
				if (node != null && !string.IsNullOrEmpty(node))
				{
					this.treeCutBlend.Nodes.Add(node);
				}
			}
		}

        private void setRecord()
        {
            //this.BlendList = CutBlend.SearchBlendRecord(null, this.cutlotno, null, false, true);
            bool isNascaNotEndOnly = true;
            if (this.rdoPastLot.Checked && string.IsNullOrWhiteSpace(this.txtPastLot.Text.Trim()) == false)
            {
                // 過去ロット検索時は、NASCA連携済みのレコードも対象にする
                isNascaNotEndOnly = false;
            }
            this.BlendList = CutBlend.SearchBlendRecord(null, this.cutlotno, null, false, isNascaNotEndOnly);
            this.treeAsmLot.Nodes.Clear();

            this.cmbLotNo.Items.Clear();
            this.cmbLotNo.Items.Add(this.cutlotno);
            foreach (CutBlend cb in this.BlendList)
            {
                this.treeAsmLot.Nodes.Add(cb.MagNo + " : " + cb.LotNo + " [" + cb.StartDt.ToString("MM/dd HH:mm") + "]");

                if (this.cmbLotNo.Items.Contains(cb.LotNo) == false)
                    this.cmbLotNo.Items.Add(cb.LotNo);
            }
            this.cmbLotNo.SelectedIndex = 0;

            AsmLot lot = AsmLot.GetAsmLot(this.BlendList[0].LotNo);
            this.typecd = lot.TypeCd;

            Process[] flow = Process.GetWorkFlow(this.typecd);
            Process final = flow.Where(p => p.FinalSt == true).FirstOrDefault();
            this.proc = final;

            setOrder();
            
            //作業名の表示
            this.treeWork.Nodes.Clear();
            bool isTargetFg = false;
            foreach (Process p in flow)
            {
                if (p.ProcNo == this.proc.ProcNo)
                    isTargetFg = true;

                if (isTargetFg == true)
                    this.treeWork.Nodes.Add(p.ProcNo.ToString(), p.InlineProNM);
            }

            setDefectGrid();
        }

        private void setOrder()
        {
            Order ord = Order.GetMagazineOrder(this.txtLotNo.Text, this.proc.ProcNo);
            if (ord != null)
            {
                this.chkNascaStart.Checked = ord.IsNascaStart;
                this.chkNascaEnd.Checked = ord.IsNascaEnd;

                this.txtEmpCd.Text = ord.InspectEmpCd;
                if (ord.InspectCt < 0)
                {
                    this.rdoInspectAll.Checked = true;
                    this.rdoInspectSample.Checked = false;
                    this.txtInspectCt.Text = "";
                }
                else
                {

                    //検査数が0の場合はマスタ参照
                    if (ord.InspectCt == 0)
                    {
                        int? defaultct = Defect.GetDefaultInspectCtMaster(this.typecd, ord.ProcNo);
                        if (defaultct.HasValue == false)
                        {
                            this.txtInspectCt.Text = "";
                        }
                        else
                        {
                            this.txtInspectCt.Text = defaultct.ToString();
                        }
                    }
                    else
                    {
                        this.txtInspectCt.Text = ord.InspectCt.ToString();
                    }
                    this.rdoInspectAll.Checked = false;
                    this.rdoInspectSample.Checked = true;
                }

                this.txtCutComment.Text = ord.Comment;
                this.dtpWorkStart.Value = ord.WorkStartDt;
                this.dtpWorkStart.Enabled = true;

                if (ord.WorkEndDt.HasValue)
                {
                    this.dtpWorkEnd.Value = ord.WorkEndDt.Value;
                    this.dtpWorkEnd.Enabled = true;
                }
                else
                {
                    this.dtpWorkEnd.Enabled = false;
                    this.dtpWorkEnd.Value = DateTime.MinValue;
                }
            }
            else
            {
                this.dtpWorkStart.Enabled = false;
                this.dtpWorkStart.Value = DateTime.Now;
                this.dtpWorkEnd.Enabled = false;
                this.dtpWorkEnd.Value = DateTime.Now;
            }
        }

        private void treeCutBlend_AfterSelect(object sender, TreeViewEventArgs e)
        {
            this.cutlotno = e.Node.Text; 

            this.txtDefTotal.Text = "0";
            this.txtLotNo.Text = this.cutlotno;
            setRecord();

            // 過去ロット(NASCA連携済みロット)が選択された場合、「実績修正」ボタンを無効にする
            List<CutBlend> cutlotinfolist = CutBlend.SearchBlendRecord(null, this.cutlotno, null, false, false).ToList();



            /*
            if (cutlotinfolist.Exists(c => c.IsNascaEnd == true))
            {
                this.btnUpdateDefect.Enabled = false;
            }
            else
            {
                this.btnUpdateDefect.Enabled = true;
            }
            */

        }

        private void grdDefect_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void btnUpdateDefect_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(this.cutlotno)) return;
            try
            {
                int total = 0;
                foreach (DefItem item in this.DefectList)
                {
                    total += item.DefectCt;
                }

                int empcd = 0;
                if (total > 0)
                {
                    if (string.IsNullOrEmpty(this.txtEmpCd.Text))
                    {
                        MessageBox.Show(this, "検査結果がある場合は検査者の社員番号を入力してください。");
                        return;
                    }

                    if (!int.TryParse(this.txtEmpCd.Text, out empcd))
                    {
                        MessageBox.Show(this, "社員番号は数字を入力してください。");
                        return;
                    }

                }

                Order ord = Order.GetMagazineOrder(this.txtLotNo.Text, this.proc.ProcNo);
                if (ord == null)
                {
                    MessageBox.Show(this, "実績が存在しないため編集できません。");
                    return;
                }

                //不良数に変更があった不良のリスト作成
                List<DefItem> DefectChangeList = GetDefectChnageList();

                if (DefectChangeList.Count != 0)
                {
                    //不良一覧(変更部)を確認画面に渡す。
                    // 戻り値 [true] : OKボタンが押された
                    //        [false]: Cancel/画面を閉じた
                    bool ret = FrmConfirmation.ShowForm(DefectChangeList); ;

                    //確認画面で[OK]を押さなかった場合
                    if (ret == false)
                    {
                        return;
                    }
                }
                else
                {
                    DialogResult res = MessageBox.Show(this, "保存しますか?", "確認",
                        MessageBoxButtons.OKCancel, MessageBoxIcon.Question, MessageBoxDefaultButton.Button2);

                    if (res != DialogResult.OK)
                    {
                        return;
                    }
                }

                ord.InspectEmpCd = empcd.ToString();
                ord.Comment = this.txtCutComment.Text;
                if (this.dtpWorkEnd.Enabled)
                {
                    ord.WorkEndDt = this.dtpWorkEnd.Value;
                }
                if (this.dtpWorkStart.Enabled)
                {
                    ord.WorkStartDt = this.dtpWorkStart.Value;
                }


                if (this.rdoInspectAll.Checked == true)
                {
                    ord.InspectCt = -1;
                }
                else
                {
                    int inspectct;
                    if (int.TryParse(this.txtInspectCt.Text, out inspectct))
                    {
                        ord.InspectCt = inspectct;
                    }
                    else
                    {
                        MessageBox.Show(this, "検査数は数字を入力してください。");
                        return;
                    }
                }

                if (ord.InspectCt != 0 && string.IsNullOrEmpty(this.txtEmpCd.Text))
                {
                    MessageBox.Show(this, "検査数がある場合は検査者の社員番号を入力してください。");
                    return;
                }

                if (!string.IsNullOrEmpty(this.txtEmpCd.Text))
                {
                    int num;
                    if (!int.TryParse(this.txtEmpCd.Text, out num))
                    {
                        MessageBox.Show(this, "検査者の社員番号は数字を入力してください。");
                        return;
                    }
                    ord.InspectEmpCd = num.ToString();
                }

                ord.IsNascaStart = this.chkNascaStart.Checked;
                ord.IsNascaEnd = this.chkNascaEnd.Checked;

                ord.DeleteInsert(ord.LotNo, true);

                updateDefect();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
                return;
            }

            if (this.chkNascaEnd.Checked == true)
            {
                MessageBox.Show(this, "登録完了\nこのロットはNASCA連携済みです。\nNASCA側の履歴も修正してください");
            }
            else {
                MessageBox.Show(this, "登録完了");
            }


        }

        /// <summary>
        /// 不良数を変更した不良のリスト作成
        /// </summary>
        /// <returns></returns>
        private List<DefItem> GetDefectChnageList()
        {
            List<DefItem> DefectChangeList = new List<DefItem>();

            //変更前の不良数を取得
            DefItem[] orgDefectList = Defect.GetAllDefect(this.cutlotno, this.typecd, this.proc.ProcNo);

            this.grdDefect.EndEdit();
            Defect def = new Defect();
            def.LotNo = this.cutlotno;
            def.ProcNo = this.proc.ProcNo;
            def.DefItems = new List<DefItem>();

            //変更前後の不良数を比較して、違いがあればDefectChangeList[不良リスト(変更部)]に追加
            //  [変更前]this.orgDefectList
            //  [変更後]this.grdDefect.Rows
            int beforeDefectCt = 0; //変更前の不良数
            foreach (DataGridViewRow row in this.grdDefect.Rows)
            {

                DefItem d = new DefItem();
                d.CauseCd = row.Cells["causecd"].Value.ToString();
                d.CauseName = row.Cells["causename"].Value.ToString();
                d.ClassCd = row.Cells["classcd"].Value.ToString();
                d.ClassName = row.Cells["classname"].Value.ToString();
                d.DefectCd = row.Cells["defectcd"].Value.ToString();
                d.DefectName = row.Cells["defectname"].Value.ToString();
                d.DefectCt = Convert.ToInt32(row.Cells["defectct"].Value);

                DefItem beforeDef = orgDefectList.Where(r => r.CauseCd == d.CauseCd
                                                            && r.ClassCd == d.ClassCd
                                                            && r.DefectCd == d.DefectCd).FirstOrDefault();
                if (beforeDef != null)
                {
                    beforeDefectCt = beforeDef.DefectCt;
                }

                //変更前後の不良数を比較し、変更があった場合、不良変更リストに追加
                if (d.DefectCt != beforeDefectCt)
                {
                    DefectChangeList.Add(d);
                }
            }

            return DefectChangeList;
        }


        private void btnPrintLabel_Click(object sender, EventArgs e)
        {
            if (this.BlendList == null || this.BlendList.Length == 0 || string.IsNullOrEmpty(this.txtLotNo.Text))
            {
                return;
            }

            try
            {
                bool afterFinalStFg = false;
                AsmLot lot = AsmLot.GetAsmLot(this.BlendList.First().LotNo);
                Process lastp = Process.GetLastProcess(lot.TypeCd);
                if (Process.IsFinalStAfterProcess(lastp, lot.TypeCd) == true)
                {
                    afterFinalStFg = true;
                }

                CutBlend.PrintCutLabel(this.BlendList, this.txtLotNo.Text, afterFinalStFg);
                MessageBox.Show("印刷完了");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void rdoInspectAll_CheckedChanged(object sender, EventArgs e)
        {
            this.rdoInspectAll.Checked = !rdoInspectSample.Checked;
            this.txtInspectCt.Enabled = rdoInspectSample.Checked;
        }

		private void btnSearch_Click(object sender, EventArgs e)
		{
			SearchBlendLot();

			Properties.Settings.Default.SelectMacGroup = txtMacGroup.Text;

            // 各構成フォームを初期化
            this.cutlotno = null;
            this.grdDefect.DataSource = null;
            this.treeAsmLot.Nodes.Clear();
            this.treeWork.Nodes.Clear();
            this.cmbLotNo.Items.Clear();
            this.txtCutComment.Text = null;
            this.txtLotNo.Text = null;
            this.dtpWorkStart.Value = DateTime.Now;
            this.dtpWorkEnd.Value = DateTime.Now;
            this.txtEmpCd.Text = null;
            this.txtDefTotal.Text = null;
            this.rdoInspectAll.Checked = false;
            this.rdoInspectSample.Checked = true;
            this.txtInspectCt.Text = null;
        }

        private void btnDelete_Click(object sender, EventArgs e)
		{
            if (treeCutBlend.SelectedNode == null) return;

            if (DialogResult.OK != MessageBox.Show("削除してよろしいですか？", "Question", MessageBoxButtons.OKCancel, MessageBoxIcon.Question))
            {
                return;
            }

            try
            {
                CutBlend.Delete(treeCutBlend.SelectedNode.Text);
                SearchBlendLot();
                MessageBox.Show("削除が完了しました。");
            }
            catch (Exception err)
            {
                MessageBox.Show(string.Format("削除に失敗しました。理由:{0} 場所:{1}", err.Message, err.StackTrace));
            }
        }

        private void btnPrintLabel2_Click(object sender, EventArgs e)
        {
            if (this.BlendList == null || this.BlendList.Length == 0 || string.IsNullOrEmpty(this.txtLotNo.Text))
            {
                return;
            }

            try
            {
                string directoryPath = ArmsApi.Config.Settings.OutLabelOutputDirectoryPath;
                if (directoryPath.EndsWith("\\") == false)
                {
                    directoryPath += "\\";
                }
                if (System.IO.Directory.Exists(directoryPath) == false)
                {
                    throw new ApplicationException($"指定のファイル出力先フォルダが存在しません。\r\n出力先『{directoryPath}』");
                }

                AsmLot firstlot = AsmLot.GetAsmLot(this.BlendList[0].LotNo);
                // 出力ファイルのフォーマット = 「ロットNo タイプ 樹脂Gr.csv」
                string fileName = $"{this.txtLotNo.Text} {firstlot.TypeCd} {string.Join(",",firstlot.ResinGpCd)}.csv";
                // 空のファイルを作成する
                using (System.IO.StreamWriter sw = new System.IO.StreamWriter(System.IO.Path.Combine(directoryPath, fileName), false, Encoding.GetEncoding("shift_jis")))
                {
                }

                MessageBox.Show($"検査・照合用印刷完了\r\n印刷用ファイル出力先『{directoryPath}』");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void rdoSearchCond_CheckedChanged(object sender, EventArgs e)
        {
            if ( this.rdoLine.Checked == true)
            {
                this.txtMacGroup.Enabled = true;
                this.txtPastLot.Enabled = false;
            }
            else if (this.rdoPastLot.Checked == true)
            {
                this.txtMacGroup.Enabled = false;
                this.txtPastLot.Enabled = true;
            }

        }

        private void treeWork_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (this.proc != null)
            {
                if (this.proc.ProcNo.ToString() == e.Node.Name)
                {
                    return;
                }
            }

            this.proc = Process.GetProcess(int.Parse(e.Node.Name));

            setOrder();
            setDefectGrid();
        }

        private void cmbLotNo_SelectionChangeCommitted(object sender, EventArgs e)
        {
            this.cutlotno = this.cmbLotNo.SelectedItem.ToString();
            this.txtDefTotal.Text = "0";

            setDefectGrid();
        }
    }
}
