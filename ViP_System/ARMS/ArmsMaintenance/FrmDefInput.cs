using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using ArmsApi.Model;
using ArmsApi;

namespace ArmsMaintenance
{
    public partial class FrmDefInput : Form
    {
        private SortedList<DefItem, int> OriginalDefectList;
        private DefItem[] DefectList;
        private DefItem[] FilterdList;
        private MachineInfo m;
        private Order[] orders = new Order[0];
        private Order selected = null;
        
        /// <summary>
        /// 主要不良の入力ボタン
        /// </summary>
        private List<Button> primalListButtons;

        /// <summary>
        /// 主要不良の数量テキストボックス
        /// </summary>
        private List<TextBox> primalListTextBoxes;

		/// <summary>
		/// 主要不良の分類ラジオボタン
		/// </summary>
		private List<RadioButton> primalClassRdoButtons;

        private Button[] macButtons;

		private Button[] chipButtons;

		private const int MAX_MACBUTTON = 11;

		private const int MAX_CHIPBUTTON = 12;

        /// <summary>
        /// 装置選択画面に渡す装置リスト
        /// </summary>
        private List<MachineInfoGroup> macInfoGroupList { get; set; }

        public FrmDefInput()
        {
            InitializeComponent();

			macButtons = new Button[MAX_MACBUTTON];
            macButtons[0] = btnMac1;
            macButtons[1] = btnMac2;
            macButtons[2] = btnMac3;
            macButtons[3] = btnMac4;
            macButtons[4] = btnMac5;
            macButtons[5] = btnMac6;
			macButtons[6] = btnMac7;
			macButtons[7] = btnMac8;
			macButtons[8] = btnMac9;
			macButtons[9] = btnMac10;
			macButtons[10] = btnMac11;

			//chipButtons = new Button[MAX_CHIPBUTTON];
			//chipButtons[0] = btnChip1;
			//chipButtons[1] = btnChip2;
			//chipButtons[2] = btnChip3;
			//chipButtons[3] = btnChip4;
			//chipButtons[4] = btnChip5;
			//chipButtons[5] = btnChip6;
			//chipButtons[6] = btnChip7;
			//chipButtons[7] = btnChip8;
			//chipButtons[8] = btnChip9;
			//chipButtons[9] = btnChip10;
			//chipButtons[10] = btnChip11;
			//chipButtons[11] = btnChip12;
        }

        #region 不良登録関連

        /// <summary>
        /// 不良登録グリッドの初期化
        /// </summary>
        private void setDefectGrid(string lotno)
        {
            if (string.IsNullOrEmpty(lotno))
            {
                this.DefectList = new DefItem[0];
                this.grdDefect.DataSource = this.DefectList;
                BindPrimalDefect();
                return;
            }

            Order o = this.orders.Where(i => i.LotNo == lotno).FirstOrDefault();
            AsmLot lot = AsmLot.GetAsmLot(lotno);

            if (o == selected) return;
            selected = o;　//nullでも選択する

            if (o == null)
            {
                this.DefectList = new DefItem[0];
                this.grdDefect.DataSource = this.DefectList;
                return;
            }

            this.txtEmpCd.Text = o.InspectEmpCd;
            if (o.InspectCt < 0)
            {
                this.rdoInspectAll.Checked = true;
                this.rdoInspectSample.Checked = false;
                this.txtInspectCt.Text = "";
            }
            else
            {
                this.rdoInspectAll.Checked = false;
                this.rdoInspectSample.Checked = true;
                if (o.InspectCt == 0)
                {
                    int? defaultct = Defect.GetDefaultInspectCtMaster(lot.TypeCd, o.ProcNo);
                    if (defaultct == null)
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
                    this.txtInspectCt.Text = o.InspectCt.ToString();
                }
            }

            DefItem[] org = Defect.GetAllDefect(lot, o.ProcNo);

            if (org == null || org.Length == 0)
            {
                this.DefectList = org;
                this.grdDefect.DataSource = null;
                return;
            }

            this.txtCauseNm.Text = "";
            this.txtClassNm.Text = "";
            this.txtDefectNm.Text = "";
            this.numDefectCt.Value = 0;

            this.DefectList = org.OrderBy(i => i.OrderNo).ToArray();
            this.grdDefect.DataSource = this.DefectList;

            this.OriginalDefectList = new SortedList<DefItem,int>();
            //オリジナル値をコピー保存
            foreach (DefItem def in this.DefectList)
            {
                this.OriginalDefectList.Add(def, def.DefectCt);
            }

            this.grdDefect.Columns["OrderNo"].Visible = false;
            this.grdDefect.Columns["CauseCd"].Visible = false;
            this.grdDefect.Columns["ClassCd"].Visible = false;
            this.grdDefect.Columns["DefectCd"].Visible = false;
            
            this.grdDefect.Columns["CauseName"].ReadOnly = true;
            this.grdDefect.Columns["CauseName"].HeaderText = "起因";
            this.grdDefect.Columns["CauseName"].DisplayIndex = 4;
            
            this.grdDefect.Columns["ClassName"].ReadOnly = true;
            this.grdDefect.Columns["ClassName"].HeaderText = "分類";
            this.grdDefect.Columns["ClassName"].DisplayIndex = 3;

            this.grdDefect.Columns["DefectName"].ReadOnly = true;
            this.grdDefect.Columns["DefectName"].HeaderText = "名称";
            this.grdDefect.Columns["DefectName"].DisplayIndex = 1;

            this.grdDefect.Columns["DefectCt"].HeaderText = "数量";
            this.grdDefect.Columns["DefectCt"].ReadOnly = true;
            this.grdDefect.Columns["DefectCt"].DisplayIndex = 2;

            countAndSetDefectTotal();

            this.cmbCause.Items.Clear();
            this.cmbCause.Items.AddRange(this.DefectList.Select(d => d.CauseName).Distinct().ToArray());

            this.cmbClass.Items.Clear();
            this.cmbClass.Items.AddRange(this.DefectList.Select(d => d.ClassName).Distinct().ToArray());
        }

        private void updateDefect(string lotno)
        {
            if (selected == null) return;

            Defect def = new Defect();
            def.LotNo = selected.LotNo;
            def.ProcNo = selected.ProcNo;
            def.DefItems = new List<DefItem>();
            def.DefItems.AddRange(this.DefectList);
            foreach (DefItem itm in def.DefItems)
            {
                if (itm.DefectCt > 0) updateEICSAddress(def.LotNo, itm);
            }

            def.DeleteInsert();

            this.DefectList = new DefItem[0];
            this.grdDefect.DataSource = this.DefectList;
            this.selected = null;
        }
		
        /// <summary>
        /// EICSアドレス更新用のダイアログ呼び出し
        /// </summary>
        /// <param name="lotno"></param>
        /// <param name="def"></param>
        private void updateEICSAddress(string lotno, DefItem def)
        {
            AsmLot lot = AsmLot.GetAsmLot(lotno);
            Magazine[] mags = Magazine.GetMagazine(null, lotno, true, null);

            if (lot == null || mags.Length == 0)
            {
                throw new ApplicationException("EICSアドレス更新異常：ロット情報またはマガジン情報が存在しません ");
            }

            KeyValuePair<DefItem, int> org = this.OriginalDefectList.Where(o => o.Key.CauseCd == def.CauseCd && o.Key.ClassCd == def.ClassCd && o.Key.DefectCd == def.DefectCd).FirstOrDefault();
            if (def.DefectCt > org.Value)
            {
                FrmDefAddressInput frm = new FrmDefAddressInput(mags[0].MagazineNo, def, lot, this.txtEmpCd.Text);
                DialogResult dres = frm.ShowDialog(this);
                if (dres != DialogResult.OK)
                {
                    updateEICSAddress(lotno, def);
                }
            }
        }
		
        /// <summary>
        /// 不良合計数のカウント
        /// </summary>
        private void countAndSetDefectTotal()
        {
            int total = 0;

            if (this.DefectList != null)
            {
                foreach (DefItem item in this.DefectList)
                {
                    total += item.DefectCt;
                }
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

        #region 号機選択ボタンクリック
        
        private void btnSelectMachine_Click(object sender, EventArgs e)
        {
            try
            {
                FrmSelectMachine frmmac = new FrmSelectMachine();
                DialogResult res = frmmac.ShowDialog();

                if (res == DialogResult.OK)
                {
                    MachineInfo m = frmmac.SelectedMachine;
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                    this.m = m;
                    this.lstLotNo.Items.Clear();
                    this.selected = null;
                    this.DefectList = new DefItem[0];
                    this.grdDefect.DataSource = this.DefectList;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        private void reloadLotList()
        {
            if (this.m == null)
            {
                setDefectGrid(null);
                return;
            }

            Order[] orders = Order.GetCurrentWorkingOrderInMachine(m.MacNo).OrderBy(o => o.WorkStartDt).ToArray();
            this.orders = orders;

            List<ListLot> listLots = new List<ListLot>();
            foreach (Order o in orders)
            {
                if (Inspection.GetInspection(o.LotNo, o.ProcNo) == null)
                {
                    listLots.Add(new ListLot(o.LotNo, o.LotNo));
                }
                else 
                {
                    //検査設定のあるロットの場合は識別する為に*を頭文字に付与
                    listLots.Add(new ListLot(o.LotNo, "*" + o.LotNo));
                }
            }

			lstLotNo.ValueMember = "Key";
			lstLotNo.DisplayMember = "Text";
            lstLotNo.DataSource = listLots;

            if (this.lstLotNo.Items.Count >= 1)
            {
                this.lstLotNo.SelectedIndex = 0;
                setDefectGrid(this.lstLotNo.SelectedValue.ToString());
            }

            BindPrimalDefect();
        }

        private void reloadMacMatList()
        {
            setMatGrid();

            setResinGrid();
        }

        private void FrmDefInput_Load(object sender, EventArgs e)
        {
			// 装置ボタン設定
            int?[] MacList = Config.Settings.FrmDefInputMacNoList;
			for (int i = 0; i < MAX_MACBUTTON; i++)
            {
                if (MacList.Length <= i)
                {
                    macButtons[i].Tag = null;
                    macButtons[i].Text = "";
                    continue;
                }
                if (MacList[i] == null)
                {
                    macButtons[i].Tag = null;
                    macButtons[i].Text = "";
                    continue;
                }
                MachineInfo m = MachineInfo.GetMachine(MacList[i].Value);
                macButtons[i].Tag = m;
                macButtons[i].Text = m.MachineName; 
            }

            if (Config.Settings.FrmDefInputLineAndMachineClassList != null)
            {
                this.macInfoGroupList = MachineInfoGroup.GetMachineInfoGroupList(Config.Settings.FrmDefInputLineAndMachineClassList);
                // 装置ボタンを非表示にする
                for (int i = 0; i < MAX_MACBUTTON; i++)
                {
                    macButtons[i].Visible = false;
                }
            }
            else
            {
                // 装置選択ボタンを非表示にする
                this.btnMachineSelect.Visible = false;
            }

            this.primalListButtons = new List<Button>();
            this.primalListTextBoxes = new List<TextBox>();
			this.primalClassRdoButtons = new List<RadioButton>();

            #region 主要不良入力用コントロール配列作成
            
            this.primalListButtons.Add(btnPrime1);
            this.primalListButtons.Add(btnPrime2);
            this.primalListButtons.Add(btnPrime3);
            this.primalListButtons.Add(btnPrime4);
            this.primalListButtons.Add(btnPrime5);
            this.primalListButtons.Add(btnPrime6);
            this.primalListButtons.Add(btnPrime7);
            this.primalListButtons.Add(btnPrime8);
            this.primalListButtons.Add(btnPrime9);

            this.primalListTextBoxes.Add(txtPrime1);
            this.primalListTextBoxes.Add(txtPrime2);
            this.primalListTextBoxes.Add(txtPrime3);
            this.primalListTextBoxes.Add(txtPrime4);
            this.primalListTextBoxes.Add(txtPrime5);
            this.primalListTextBoxes.Add(txtPrime6);
            this.primalListTextBoxes.Add(txtPrime7);
            this.primalListTextBoxes.Add(txtPrime8);
            this.primalListTextBoxes.Add(txtPrime9);

			this.primalClassRdoButtons.Add(rdoClass1);
			this.primalClassRdoButtons.Add(rdoClass2);
			this.primalClassRdoButtons.Add(rdoClass3);
			this.primalClassRdoButtons.Add(rdoClass4);
			this.primalClassRdoButtons.Add(rdoClass5);
			this.primalClassRdoButtons.Add(rdoClass6);
			this.primalClassRdoButtons.Add(rdoClass7);
			this.primalClassRdoButtons.Add(rdoClass8);

            #endregion
        }

        private void btnUpdateDefect_Click(object sender, EventArgs e)
        {
            if (this.selected == null)
            {
                return;
            }

            DialogResult res = MessageBox.Show(this, "不良情報を更新します", "確認", MessageBoxButtons.OKCancel,
                MessageBoxIcon.Exclamation, MessageBoxDefaultButton.Button2);

            int empcd = 0;
            if (string.IsNullOrEmpty(this.txtEmpCd.Text))
            {
                MessageBox.Show("検査者を入力してください");
                return;
            }
            else
            {
                if (!int.TryParse(this.txtEmpCd.Text, out empcd))
                {
                    MessageBox.Show("検査者は数字を入力してください。");
                    return;
                }


                //他からの更新の可能性があるので再度とり直し
                Order ord = Order.GetMagazineOrder(selected.LotNo, selected.ProcNo);
                ord.InspectEmpCd = empcd.ToString();

                if (rdoInspectAll.Checked == true)
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
                        MessageBox.Show("検査数を入力してください");
                        return;
                    }
                }

                ord.DeleteInsert(ord.LotNo);
            }

            if (this.grdDefect.SelectedRows.Count != 0)
            {
                DefItem current = (DefItem)this.grdDefect.SelectedRows[0].DataBoundItem;
                if (current.DefectCt != numDefectCt.Value)
                {
                    current.DefectCt = (int)numDefectCt.Value;
                }
            }

            if (res != DialogResult.OK) return;
            this.grdDefect.EndEdit();
            updateDefect(this.selected.LotNo);
            reloadLotList();
        }

		private void lstLotNo_SelectedValueChanged(object sender, EventArgs e)
		{
			if (this.lstLotNo.Items.Count >= 1 && this.lstLotNo.SelectedIndex != 0)
			{
				MessageBox.Show(this.lstLotNo.SelectedValue.ToString() + "の不良を入力します");
			}

			if (this.lstLotNo.Items.Count >= 1)
			{
				setDefectGrid(this.lstLotNo.SelectedValue.ToString());
				BindPrimalDefect();
				this.grdDefect.ClearSelection();

				// チップボタン設定
				AsmLot lot = AsmLot.GetAsmLot(this.lstLotNo.SelectedValue.ToString());
				DefItem[] defItems = Defect.GetDefectMaster(lot.TypeCd);

				var clsCdList = defItems.Select(d => d.ClassName).GroupBy(d => d).Select(g => new { g.Key });
				if (clsCdList.Count() > MAX_CHIPBUTTON) 
				{
					throw new ApplicationException(string.Format("チップボタンの制限数を超えて表示しようとしています。システム管理者に問い合わせて下さい。製品型番:{0}", lot.TypeCd));
				}

				int i = 0;
				foreach (var clsCd in clsCdList)
				{
					this.primalClassRdoButtons[i].Text = clsCd.Key;
					i++;
				}
			}
		}

        #region グリッドの選択列変更処理　grdDefect_SelectionChanged(object sender, EventArgs e)

        /// <summary>
        /// グリッドの選択列変更処理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void grdDefect_SelectionChanged(object sender, EventArgs e)
        {           
            if (this.grdDefect.SelectedRows.Count == 0)
            {
                ///何もない場合はクリア
                this.txtCauseNm.Text = "";
                this.txtClassNm.Text = "";
                this.txtDefectNm.Text = "";
                this.numDefectCt.Value = 0;
                return;
            }
            else
            {
                //既存の入力値をソースに反映
                string causenm = this.txtCauseNm.Text;
                string classnm = this.txtClassNm.Text;
                string defectnm = this.txtDefectNm.Text;
                int defectct = (int)this.numDefectCt.Value;

                DefItem itm = this.DefectList.Where(d => d.DefectName == defectnm && d.ClassName == classnm && d.CauseName == causenm).FirstOrDefault();

                if (itm != null)
                {
                    itm.DefectCt = defectct;
                }
            }

            DefItem current = (DefItem)this.grdDefect.SelectedRows[0].DataBoundItem;

            this.txtCauseNm.Text = current.CauseName;
            this.txtClassNm.Text = current.ClassName;
            this.txtDefectNm.Text = current.DefectName;
            this.numDefectCt.Value = current.DefectCt;
            countAndSetDefectTotal();
        }
        #endregion

        #region 詳細入力ページ　フィルターコンボボックス
        
        private void cmbCause_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterDefect();
        }

        private void cmbClass_SelectedIndexChanged(object sender, EventArgs e)
        {
            filterDefect();
        }

        private void filterDefect()
        {
            string causenm = this.cmbCause.Text;
            string classnm = this.cmbClass.Text;

            if (!string.IsNullOrEmpty(causenm))
            {
                this.FilterdList = this.DefectList.Where(d => d.CauseName == causenm).ToArray();
            }
            else
            {
                this.FilterdList = this.DefectList;
            }

            if (!string.IsNullOrEmpty(classnm))
            {
                this.FilterdList = this.FilterdList.Where(d => d.ClassName == classnm).ToArray();
            }

            this.grdDefect.DataSource = FilterdList;
        }
        #endregion

        #region 分類選択ラジオボタン

        private void rdoClassChk_Click(object sender, EventArgs e)
        {
			BindPrimalDefect();
        }
        #endregion

        private void btnPrimal_Click(object sender, EventArgs e)
        {
            int idx = this.primalListButtons.IndexOf((Button)sender);

            int defct = int.Parse(this.primalListTextBoxes[idx].Text);
            this.primalListTextBoxes[idx].Text = (++defct).ToString();
            this.primalListTextBoxes[idx].Focus();
        }

        private void BindPrimalDefect()
        {
            if (this.DefectList == null) return;

			string classnm = "";

			foreach (RadioButton rdo in this.primalClassRdoButtons)
			{
				if (rdo.Checked == true)
				{
					classnm = rdo.Text;
					break;
				}
			}

            int i = 0;

            foreach (DefItem def in this.DefectList)
            {
                if (def.ClassName == classnm)
                {
                    this.primalListButtons[i].Text = def.DefectName + "\r" + def.CauseName;
                    this.primalListButtons[i].Enabled = true;
                    this.primalListTextBoxes[i].DataBindings.Clear();
                    this.primalListTextBoxes[i].DataBindings.Add("Text", def, "DefectCt");
                    this.primalListTextBoxes[i].Enabled = true;
                    compareOriginal(this.primalListTextBoxes[i]);
                    i += 1;
                    if (i > 8) break;
                }
            }

            //足りない項目分の入力BoxをDisable
            for (int j = i; j <= 8; j++)
            {
                this.primalListButtons[j].Text = "";
                this.primalListButtons[j].Enabled = false;
                this.primalListTextBoxes[j].DataBindings.Clear();
                this.primalListTextBoxes[j].Text = "";
                this.primalListTextBoxes[j].Enabled = false;
            }
        }

        private void txtPrime1_Validated(object sender, EventArgs e)
        {
            countAndSetDefectTotal();
            TextBox txt = (TextBox)sender;
            compareOriginal(txt);
        }

        /// <summary>
        /// 初期値を比較して変更箇所を赤表示
        /// </summary>
        /// <param name="txt"></param>
        private void compareOriginal(TextBox txt)
        {
            if (txt.DataBindings.Count >= 1)
            {
                DefItem def = txt.DataBindings[0].DataSource as DefItem;
                KeyValuePair<DefItem, int> org = this.OriginalDefectList.Where(o => o.Key.CauseCd == def.CauseCd && o.Key.ClassCd == def.ClassCd && o.Key.DefectCd == def.DefectCd).FirstOrDefault();
                if (def.DefectCt != org.Value)
                {
                    txt.ForeColor = Color.Red;
                }
                else
                {
                    txt.ForeColor = Color.Black;
                }
            }
        }

        private void tabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.grdDefect.SelectedRows.Count != 0)
            {
                DefItem current = (DefItem)this.grdDefect.SelectedRows[0].DataBoundItem;
                if (current.DefectCt != numDefectCt.Value)
                {
                    current.DefectCt = (int)numDefectCt.Value;
                }

                this.grdDefect.ClearSelection();
            }

            BindPrimalDefect();
            filterDefect();
        }

        private void rdoInspectSample_CheckedChanged(object sender, EventArgs e)
        {
            this.rdoInspectAll.Checked = !rdoInspectSample.Checked;
            this.txtInspectCt.Enabled = rdoInspectSample.Checked;
        }

        #region テンキー　btnNum*_Click
        
        private void btnNum0_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("0");
            SendKeys.Flush();
        }

        private void btnNum1_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("1");
            SendKeys.Flush();
        }

        private void btnNum2_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("2");
            SendKeys.Flush();
        }

        private void btnNum3_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("3");
            SendKeys.Flush();
        }

        private void btnNum4_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("4");
            SendKeys.Flush();
        }

        private void btnNum5_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("5");
            SendKeys.Flush();
        }

        private void btnNum6_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("6");
            SendKeys.Flush();
        }

        private void btnNum7_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("7");
            SendKeys.Flush();
        }

        private void btnNum8_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("8");
            SendKeys.Flush();
        }

        private void btnNum9_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("9");
            SendKeys.Flush();
        }

        private void btnNumDel_Click(object sender, EventArgs e)
        {
            SendKeys.SendWait("{BACKSPACE}");
            SendKeys.Flush();
        }

        #endregion

        private void btnMac_Click(object sender, EventArgs e)
        {
            if (((Button)sender).Tag != null)
            {
                MachineInfo m = ((Button)sender).Tag as MachineInfo;
                if (m == null) return;

                try
                {
                    this.txtMachine.Text = m.LongName;
                    this.txtMacNo.Text = m.MacNo.ToString();
                    this.m = m;

                    this.selected = null;
                    this.DefectList = new DefItem[0];
                    this.grdDefect.DataSource = this.DefectList;

                    reloadLotList();

                    reloadMacMatList();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

		private void btnChip_Click(object sender, EventArgs e)
		{
			//if (string.IsNullOrEmpty(((Button)sender).Text) == false)
			//{
			//	BindPrimalDefect(((Button)sender).Text);
			//}
		}

        private void btnUnselect_Click(object sender, EventArgs e)
        {
            this.txtMachine.Text = string.Empty;
            this.txtMacNo.Text = string.Empty;
            this.m = null;
            this.lstLotNo.DataSource = new List<ListLot>();
            //this.lstLotNo.Items.Clear();
            this.selected = null;
            this.DefectList = new DefItem[0];
            this.grdDefect.DataSource = this.DefectList;

            reloadLotList();
        }

        #region 試験データ出力ボタン

        private void btnExport_Click(object sender, EventArgs e)
        {
            if (lstLotNo.Items.Count == 0)
            {
                MessageBox.Show("投入中ロットが無い為、出力できません。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (lstLotNo.SelectedIndex == -1)
            {
                return;
            }

            string lotNo = this.lstLotNo.SelectedValue.ToString().Trim();

            Magazine[] mags = Magazine.GetMagazine(lotNo, true);
            if (mags.Count() == 0)
            {
                MessageBox.Show(string.Format("稼働中のマガジン情報が存在しません。LotNo:{0}", lotNo));
                return;
            }
            
            mags.Single().RecordAGVPSTester();

            mags.Single().WritePSTesterFile();

            MessageBox.Show(this, "正常に終了しました");
        }

        private void btnBallExport_Click(object sender, EventArgs e)
        {
            if (lstLotNo.Items.Count == 0)
            {
                MessageBox.Show("投入中ロットが無い為、出力できません。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (lstLotNo.SelectedIndex == -1)
            {
                return;
            }

            string lotNo = this.lstLotNo.SelectedValue.ToString().Trim();

            Magazine[] mags = Magazine.GetMagazine(lotNo, true);
            if (mags.Count() == 0)
            {
                MessageBox.Show("稼働中のマガジン情報が存在しません。LotNo:{0}", lotNo);
                return;
            }
            mags.Single().WriteBDTesterFile();

            MessageBox.Show(this, "正常に終了しました");
        }

        private void btnFilmExport_Click(object sender, EventArgs e)
        {
            if (lstLotNo.Items.Count == 0)
            {
                MessageBox.Show("投入中ロットが無い為、出力できません。", "Exclamation", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
                return;
            }

            if (lstLotNo.SelectedIndex == -1)
            {
                return;
            }

            string lotNo = this.lstLotNo.SelectedValue.ToString().Trim();

            Magazine[] mags = Magazine.GetMagazine(lotNo, true);
            if (mags.Count() == 0)
            {
                MessageBox.Show("稼働中のマガジン情報が存在しません。LotNo:{0}", lotNo);
                return;
            }
            mags.Single().WriteFilmTesterFile();

            MessageBox.Show(this, "正常に終了しました");
        }

        #endregion

        private void btnMatChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdMatValues.SelectedRows.Count == 0)
                {
                    return;
                }

                Material mat = (Material)this.grdMatValues.SelectedRows[0].DataBoundItem;

                DialogResult res = DialogResult.Cancel;
                res = new FrmMacMat(this.m).ShowDialog();
                if (res == DialogResult.OK)
                {
                    mat.RemoveDt = DateTime.Now.AddSeconds(-1);
                    m.DeleteInsertMacMat(mat);

                    setMatGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnMatEdit_Click(object sender, EventArgs e)
        {
            DialogResult res = DialogResult.Cancel;

            Material mat = (Material)this.grdMatValues.SelectedRows[0].DataBoundItem;
            res = new FrmMacMat(this.m, mat).ShowDialog();
            if (res == DialogResult.OK)
            {
                setMatGrid();
            }
        }

        private void btnResinEdit_Click(object sender, EventArgs e)
        {
            DialogResult res = DialogResult.Cancel;

            Resin resin = (Resin)this.grdResinValues.SelectedRows[0].DataBoundItem;
            res = new FrmMacResin(this.m, resin).ShowDialog();
            if (res == DialogResult.OK)
            {
                setResinGrid();
            }
        }

        private void btnResinChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.grdResinValues.SelectedRows.Count == 0)
                {
                    return;
                }

                Resin resin = (Resin)this.grdResinValues.SelectedRows[0].DataBoundItem;

                DialogResult res = DialogResult.Cancel;
                res = new FrmMacResin(this.m).ShowDialog();
                if (res == DialogResult.OK)
                {
                    resin.RemoveDt = DateTime.Now.AddSeconds(-1);
                    m.DeleteInsertMacResin(resin);

                    setResinGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void btnWaferChange_Click(object sender, EventArgs e)
        {
            try
            {
                if (this.m == null) return;

                if (DialogResult.OK !=  MessageBox.Show("ウェハーカセットの交換作業を行います。よろしいですか？", "Infomation", MessageBoxButtons.OKCancel, MessageBoxIcon.Information))
                {
                    return;
                }

                Material[] matlist = m.GetMaterials(DateTime.Now, DateTime.Now);
                foreach (Material mat in matlist)
                {
                    if (mat.IsWafer != true)
                    {
                        continue;
                    }

                    mat.RemoveDt = DateTime.Now.AddSeconds(-1);
                    m.DeleteInsertMacMat(mat);
                }

                DialogResult res = DialogResult.Cancel;
                res = new FrmMacWafer(this.m).ShowDialog();

                if (res == DialogResult.OK)
                {
                    setMatGrid();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }

        private void setMatGrid()
        {
            if (this.m == null) return;

            this.grdMatValues.DataSource = m.GetMaterials();

            foreach (DataGridViewColumn col in grdMatValues.Columns)
            {
                switch (col.Name)
                {
                    case "MaterialCd":
                        col.HeaderText = "原材料CD";
                        break;

                    case "MaterialNm":
                        col.HeaderText = "原材料名";
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット";
                        break;

                    case "StockerNo":
                        col.HeaderText = "ストッカー";
                        break;

                    case "StartDt":
                    case "InputDt":
                        col.HeaderText = "投入日";
                        break;

                    case "EndDt":
                    case "RemoveDt":
                        col.HeaderText = "取外し日";
                        break;

                    case "LimitDt":
                        col.HeaderText = "期限";
                        break;

                    case "MixResultId":
                        col.HeaderText = "調合ID";
                        break;

                    case "ResinGroupCd":
                        col.HeaderText = "樹脂Gr";
                        break;

                    case "CondCd":
                        col.HeaderText = "特性CD";
                        break;

                    case "CondVal":
                        col.HeaderText = "特性値";
                        break;

                    case "CondName":
                        col.HeaderText = "特性名称";
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }

            if (this.grdMatValues.SelectedRows.Count == 0)
            {
                this.btnMatEdit.Enabled = false;
                //if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = false;
            }
            else
            {
                this.btnMatEdit.Enabled = true;
                //if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = true;
            }
        }

        private void setResinGrid()
        {
            if (this.m == null) return;

            this.grdResinValues.DataSource = m.GetResins();

            foreach (DataGridViewColumn col in grdResinValues.Columns)
            {
                switch (col.Name)
                {
                    case "MaterialCd":
                        col.HeaderText = "原材料CD";
                        break;

                    case "MaterialNm":
                        col.HeaderText = "原材料名";
                        break;

                    case "LotNo":
                        col.HeaderText = "ロット";
                        break;

                    case "StockerNo":
                        col.HeaderText = "ストッカー";
                        break;

                    case "StartDt":
                    case "InputDt":
                        col.HeaderText = "投入日";
                        break;

                    case "EndDt":
                    case "RemoveDt":
                        col.HeaderText = "取外し日";
                        break;

                    case "LimitDt":
                        col.HeaderText = "期限";
                        break;

                    case "MixResultId":
                        col.HeaderText = "調合ID";
                        break;

                    case "ResinGroupCd":
                        col.HeaderText = "樹脂Gr";
                        break;

                    case "CondCd":
                        col.HeaderText = "特性CD";
                        break;

                    case "CondVal":
                        col.HeaderText = "特性値";
                        break;

                    case "CondName":
                        col.HeaderText = "特性名称";
                        break;

                    default:
                        col.Visible = false;
                        break;
                }
            }

            if (this.grdResinValues.SelectedRows.Count == 0)
            {
                this.btnResinEdit.Enabled = false;
                //if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = false;
            }
            else
            {
                this.btnResinEdit.Enabled = true;
                //if (this.dataType == FrmMacValueDataType.Material) this.btnDeleteMat.Enabled = true;
            }
        }

        private void grdMatValues_SelectionChanged(object sender, EventArgs e)
        {
            if (this.grdMatValues.SelectedRows.Count == 0)
            {
                this.btnMatEdit.Enabled = false;
            }
            else
            {
                this.btnMatEdit.Enabled = true;
            }
        }

        private void btnMachineSelect_Click(object sender, EventArgs e)
        {
            try
            {
                int macno;
                if (int.TryParse(this.txtMacNo.Text, out macno) == false)
                {
                    macno = 0;
                }

                FrmSelectMachineButtons frm = new FrmSelectMachineButtons(macno, this.macInfoGroupList);
                frm.ShowDialog();

                MachineInfo m = frm.SelectedMacInfo;

                if (m == null) return;

                this.txtMachine.Text = m.LongName;
                this.txtMacNo.Text = m.MacNo.ToString();
                this.m = m;

                this.selected = null;
                this.DefectList = new DefItem[0];
                this.grdDefect.DataSource = this.DefectList;

                reloadLotList();

                reloadMacMatList();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }

    public class NotSelectableButton : Button
    {
        public NotSelectableButton()
        {
            this.SetStyle(ControlStyles.Selectable, false);
        }
    }

    public class ListLot
    {
        public string Key { get; set; }
        public string Text { get; set; }

        public ListLot(string key, string text) 
        {
            this.Key = key;
            this.Text = text;
        }
    }
}
