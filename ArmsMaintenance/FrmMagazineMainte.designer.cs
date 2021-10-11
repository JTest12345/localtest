namespace ArmsMaintenance
{
    partial class FrmMagazineMainte
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMagazineMainte));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTypeNo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtResinGrp = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.chkLifeTestResultInfo = new System.Windows.Forms.CheckBox();
            this.label8 = new System.Windows.Forms.Label();
            this.cmbProcess = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtDbThrowDT = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtLineCD = new System.Windows.Forms.TextBox();
            this.txtProfileId = new System.Windows.Forms.TextBox();
            this.btnProfileSelect = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.txtProfileNm = new System.Windows.Forms.TextBox();
            this.numInterval = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.chkAutoUpdate = new System.Windows.Forms.CheckBox();
            this.chkOnlyActive = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtMagNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.grdMagazine = new System.Windows.Forms.DataGridView();
            this.SelectFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DieShearTestFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NascaLotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MagazineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NowCompProcessNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ResinGr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.NowCompProcess = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CutblendCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DBThrowDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProfileNm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MacGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TempCutBlendNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LifeTestResult = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BlendCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ScheduleSelectionStandard = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MnggrId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMacMat = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMacResin = new System.Windows.Forms.ToolStripMenuItem();
            this.toolMacCond = new System.Windows.Forms.ToolStripMenuItem();
            this.プロファイル選択ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nASCA連携ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.カット完了品NASCA連携ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.nASCAロット取り込みToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.設備マスタ編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.作業履歴照会ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.作業履歴照会ToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolCutWorkView = new System.Windows.Forms.ToolStripMenuItem();
            this.dBウェハー確認ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cSV作業実績出力ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ロットエラー履歴照会ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ロット進捗確認ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.toolTouchPanelDefectMenu = new System.Windows.Forms.ToolStripMenuItem();
            this.toolManualInstruction = new System.Windows.Forms.ToolStripMenuItem();
            this.mAPダイボンド前ベーキング管理ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.メンテナンス履歴照会ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolRestrict = new System.Windows.Forms.ToolStripMenuItem();
            this.ロットトレース一括規制ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.仮想マガジンToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.データメンテナンス定型文設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.カットラベル印刷設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.マーキング情報編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.スパッタ用トレイ割当て情報ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.資材在庫数編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lblVersion = new System.Windows.Forms.ToolStripTextBox();
            this.toolMachineOperator = new System.Windows.Forms.ToolStripMenuItem();
            this.非定常メンテToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ロット情報全削除 = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.toolRowMaxCount = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolAllSelect = new System.Windows.Forms.ToolStripButton();
            this.toolAllCancel = new System.Windows.Forms.ToolStripButton();
            this.btnPSTester = new System.Windows.Forms.Button();
            this.btnEditCutBlend = new System.Windows.Forms.Button();
            this.btnCutBlend = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnInput = new System.Windows.Forms.Button();
            this.button7 = new System.Windows.Forms.Button();
            this.btnAddRow = new System.Windows.Forms.Button();
            this.timer2 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMagazine)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtTypeNo);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.txtResinGrp);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.chkLifeTestResultInfo);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.cmbProcess);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtDbThrowDT);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtLineCD);
            this.groupBox1.Controls.Add(this.txtProfileId);
            this.groupBox1.Controls.Add(this.btnProfileSelect);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.txtProfileNm);
            this.groupBox1.Controls.Add(this.numInterval);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.chkAutoUpdate);
            this.groupBox1.Controls.Add(this.chkOnlyActive);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtMagNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(14, 32);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.groupBox1.Size = new System.Drawing.Size(958, 148);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // txtTypeNo
            // 
            this.txtTypeNo.Location = new System.Drawing.Point(87, 24);
            this.txtTypeNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtTypeNo.Name = "txtTypeNo";
            this.txtTypeNo.Size = new System.Drawing.Size(189, 22);
            this.txtTypeNo.TabIndex = 30;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(16, 27);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(31, 14);
            this.label9.TabIndex = 29;
            this.label9.Text = "型番";
            // 
            // txtResinGrp
            // 
            this.txtResinGrp.Location = new System.Drawing.Point(87, 114);
            this.txtResinGrp.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtResinGrp.Name = "txtResinGrp";
            this.txtResinGrp.Size = new System.Drawing.Size(129, 22);
            this.txtResinGrp.TabIndex = 12;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(14, 117);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(69, 14);
            this.label3.TabIndex = 11;
            this.label3.Text = "樹脂グループ";
            // 
            // chkLifeTestResultInfo
            // 
            this.chkLifeTestResultInfo.AutoSize = true;
            this.chkLifeTestResultInfo.Location = new System.Drawing.Point(669, 80);
            this.chkLifeTestResultInfo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkLifeTestResultInfo.Name = "chkLifeTestResultInfo";
            this.chkLifeTestResultInfo.Size = new System.Drawing.Size(144, 18);
            this.chkLifeTestResultInfo.TabIndex = 28;
            this.chkLifeTestResultInfo.Text = "ライフ試験待ち状況表示";
            this.chkLifeTestResultInfo.UseVisualStyleBackColor = true;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(303, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 14);
            this.label8.TabIndex = 27;
            this.label8.Text = "完了工程";
            // 
            // cmbProcess
            // 
            this.cmbProcess.FormattingEnabled = true;
            this.cmbProcess.Location = new System.Drawing.Point(378, 55);
            this.cmbProcess.Name = "cmbProcess";
            this.cmbProcess.Size = new System.Drawing.Size(233, 22);
            this.cmbProcess.TabIndex = 26;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(403, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(63, 14);
            this.label7.TabIndex = 25;
            this.label7.Text = "D/B投入日";
            // 
            // txtDbThrowDT
            // 
            this.txtDbThrowDT.Location = new System.Drawing.Point(472, 114);
            this.txtDbThrowDT.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDbThrowDT.Name = "txtDbThrowDT";
            this.txtDbThrowDT.Size = new System.Drawing.Size(204, 22);
            this.txtDbThrowDT.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(222, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(33, 14);
            this.label6.TabIndex = 23;
            this.label6.Text = "ライン";
            // 
            // txtLineCD
            // 
            this.txtLineCD.Location = new System.Drawing.Point(261, 114);
            this.txtLineCD.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLineCD.Name = "txtLineCD";
            this.txtLineCD.Size = new System.Drawing.Size(136, 22);
            this.txtLineCD.TabIndex = 22;
            // 
            // txtProfileId
            // 
            this.txtProfileId.Location = new System.Drawing.Point(87, 84);
            this.txtProfileId.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtProfileId.Name = "txtProfileId";
            this.txtProfileId.Size = new System.Drawing.Size(66, 22);
            this.txtProfileId.TabIndex = 21;
            // 
            // btnProfileSelect
            // 
            this.btnProfileSelect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnProfileSelect.Image = global::ArmsMaintenance.Properties.Resources.viewmag;
            this.btnProfileSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnProfileSelect.Location = new System.Drawing.Point(619, 83);
            this.btnProfileSelect.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnProfileSelect.Name = "btnProfileSelect";
            this.btnProfileSelect.Size = new System.Drawing.Size(22, 22);
            this.btnProfileSelect.TabIndex = 20;
            this.btnProfileSelect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnProfileSelect.UseVisualStyleBackColor = true;
            this.btnProfileSelect.Click += new System.EventHandler(this.btnProfileSelect_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(14, 87);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(59, 14);
            this.label5.TabIndex = 19;
            this.label5.Text = "プロファイル";
            // 
            // txtProfileNm
            // 
            this.txtProfileNm.Location = new System.Drawing.Point(157, 84);
            this.txtProfileNm.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtProfileNm.Name = "txtProfileNm";
            this.txtProfileNm.ReadOnly = true;
            this.txtProfileNm.Size = new System.Drawing.Size(456, 22);
            this.txtProfileNm.TabIndex = 18;
            // 
            // numInterval
            // 
            this.numInterval.Location = new System.Drawing.Point(749, 22);
            this.numInterval.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.numInterval.Name = "numInterval";
            this.numInterval.Size = new System.Drawing.Size(57, 22);
            this.numInterval.TabIndex = 17;
            this.numInterval.Value = new decimal(new int[] {
            60,
            0,
            0,
            0});
            this.numInterval.ValueChanged += new System.EventHandler(this.numInterval_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(812, 25);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(19, 14);
            this.label4.TabIndex = 16;
            this.label4.Text = "秒";
            // 
            // chkAutoUpdate
            // 
            this.chkAutoUpdate.AutoSize = true;
            this.chkAutoUpdate.Location = new System.Drawing.Point(669, 23);
            this.chkAutoUpdate.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkAutoUpdate.Name = "chkAutoUpdate";
            this.chkAutoUpdate.Size = new System.Drawing.Size(74, 18);
            this.chkAutoUpdate.TabIndex = 14;
            this.chkAutoUpdate.Text = "自動更新";
            this.chkAutoUpdate.UseVisualStyleBackColor = true;
            this.chkAutoUpdate.CheckedChanged += new System.EventHandler(this.chkAutoUpdate_CheckedChanged);
            // 
            // chkOnlyActive
            // 
            this.chkOnlyActive.AutoSize = true;
            this.chkOnlyActive.Checked = true;
            this.chkOnlyActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnlyActive.Location = new System.Drawing.Point(669, 54);
            this.chkOnlyActive.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.chkOnlyActive.Name = "chkOnlyActive";
            this.chkOnlyActive.Size = new System.Drawing.Size(107, 18);
            this.chkOnlyActive.TabIndex = 13;
            this.chkOnlyActive.Text = "稼働中のみ表示";
            this.chkOnlyActive.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Image = global::ArmsMaintenance.Properties.Resources.go_down_10;
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.Location = new System.Drawing.Point(848, 111);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(87, 27);
            this.btnSearch.TabIndex = 2;
            this.btnSearch.Text = "検索";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtMagNo
            // 
            this.txtMagNo.Location = new System.Drawing.Point(87, 54);
            this.txtMagNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMagNo.Name = "txtMagNo";
            this.txtMagNo.Size = new System.Drawing.Size(189, 22);
            this.txtMagNo.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(60, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "マガジンNo";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(378, 24);
            this.txtLotNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(233, 22);
            this.txtLotNo.TabIndex = 7;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(318, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 14);
            this.label1.TabIndex = 6;
            this.label1.Text = "LotNo";
            // 
            // grdMagazine
            // 
            this.grdMagazine.AllowUserToAddRows = false;
            this.grdMagazine.AllowUserToDeleteRows = false;
            this.grdMagazine.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Honeydew;
            this.grdMagazine.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            this.grdMagazine.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.grdMagazine.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            this.grdMagazine.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMagazine.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectFg,
            this.DieShearTestFg,
            this.NascaLotNo,
            this.MagazineNo,
            this.TypeCd,
            this.NowCompProcessNM,
            this.ResinGr,
            this.NewFg,
            this.NowCompProcess,
            this.CutblendCd,
            this.DBThrowDt,
            this.ProfileNm,
            this.MacGroup,
            this.TempCutBlendNo,
            this.LifeTestResult,
            this.BlendCd,
            this.ScheduleSelectionStandard,
            this.MnggrId});
            this.grdMagazine.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.grdMagazine.Location = new System.Drawing.Point(6, 17);
            this.grdMagazine.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.grdMagazine.Name = "grdMagazine";
            this.grdMagazine.ReadOnly = true;
            this.grdMagazine.RowTemplate.Height = 21;
            this.grdMagazine.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdMagazine.Size = new System.Drawing.Size(946, 348);
            this.grdMagazine.TabIndex = 1;
            this.grdMagazine.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdMagazine_CellMouseDoubleClick);
            // 
            // SelectFg
            // 
            this.SelectFg.DataPropertyName = "SelectFg";
            this.SelectFg.HeaderText = "選択";
            this.SelectFg.Name = "SelectFg";
            this.SelectFg.ReadOnly = true;
            this.SelectFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SelectFg.Width = 40;
            // 
            // DieShearTestFg
            // 
            this.DieShearTestFg.DataPropertyName = "DieShearTestFg";
            this.DieShearTestFg.HeaderText = "D";
            this.DieShearTestFg.Name = "DieShearTestFg";
            this.DieShearTestFg.ReadOnly = true;
            this.DieShearTestFg.Width = 40;
            // 
            // NascaLotNo
            // 
            this.NascaLotNo.DataPropertyName = "NascaLotNo";
            this.NascaLotNo.HeaderText = "NASCAロット番号";
            this.NascaLotNo.Name = "NascaLotNo";
            this.NascaLotNo.ReadOnly = true;
            this.NascaLotNo.Width = 120;
            // 
            // MagazineNo
            // 
            this.MagazineNo.DataPropertyName = "MagazineNo";
            this.MagazineNo.HeaderText = "マガジンNo";
            this.MagazineNo.Name = "MagazineNo";
            this.MagazineNo.ReadOnly = true;
            this.MagazineNo.Width = 80;
            // 
            // TypeCd
            // 
            this.TypeCd.HeaderText = "タイプ";
            this.TypeCd.Name = "TypeCd";
            this.TypeCd.ReadOnly = true;
            // 
            // NowCompProcessNM
            // 
            this.NowCompProcessNM.DataPropertyName = "NowCompProcessNm";
            this.NowCompProcessNM.HeaderText = "完了工程";
            this.NowCompProcessNM.Name = "NowCompProcessNM";
            this.NowCompProcessNM.ReadOnly = true;
            this.NowCompProcessNM.Width = 120;
            // 
            // ResinGr
            // 
            this.ResinGr.DataPropertyName = "ResinGr";
            this.ResinGr.HeaderText = "樹脂グループ";
            this.ResinGr.Name = "ResinGr";
            this.ResinGr.ReadOnly = true;
            this.ResinGr.Width = 84;
            // 
            // NewFg
            // 
            this.NewFg.DataPropertyName = "NewFg";
            this.NewFg.HeaderText = "稼働中";
            this.NewFg.IndeterminateValue = "NewFg";
            this.NewFg.Name = "NewFg";
            this.NewFg.ReadOnly = true;
            this.NewFg.Width = 50;
            // 
            // NowCompProcess
            // 
            this.NowCompProcess.DataPropertyName = "NowCompProcess";
            this.NowCompProcess.HeaderText = "Column7";
            this.NowCompProcess.Name = "NowCompProcess";
            this.NowCompProcess.ReadOnly = true;
            this.NowCompProcess.Visible = false;
            // 
            // CutblendCd
            // 
            this.CutblendCd.HeaderText = "カットブレンドグループ";
            this.CutblendCd.Name = "CutblendCd";
            this.CutblendCd.ReadOnly = true;
            this.CutblendCd.Width = 120;
            // 
            // DBThrowDt
            // 
            this.DBThrowDt.HeaderText = "D/B投入日";
            this.DBThrowDt.Name = "DBThrowDt";
            this.DBThrowDt.ReadOnly = true;
            this.DBThrowDt.Width = 84;
            // 
            // ProfileNm
            // 
            this.ProfileNm.DataPropertyName = "ProfileNm";
            this.ProfileNm.HeaderText = "プロファイル";
            this.ProfileNm.Name = "ProfileNm";
            this.ProfileNm.ReadOnly = true;
            this.ProfileNm.Width = 350;
            // 
            // MacGroup
            // 
            this.MacGroup.HeaderText = "ライン";
            this.MacGroup.Name = "MacGroup";
            this.MacGroup.ReadOnly = true;
            this.MacGroup.Width = 80;
            // 
            // TempCutBlendNo
            // 
            this.TempCutBlendNo.DataPropertyName = "TempCutBlendNo";
            this.TempCutBlendNo.HeaderText = "仮カットブレンド";
            this.TempCutBlendNo.Name = "TempCutBlendNo";
            this.TempCutBlendNo.ReadOnly = true;
            this.TempCutBlendNo.Width = 90;
            // 
            // LifeTestResult
            // 
            this.LifeTestResult.DataPropertyName = "LifeTestResult";
            this.LifeTestResult.HeaderText = "ライフ試験結果";
            this.LifeTestResult.Name = "LifeTestResult";
            this.LifeTestResult.ReadOnly = true;
            this.LifeTestResult.Width = 90;
            // 
            // BlendCd
            // 
            this.BlendCd.HeaderText = "ブレンドCD";
            this.BlendCd.Name = "BlendCd";
            this.BlendCd.ReadOnly = true;
            // 
            // ScheduleSelectionStandard
            // 
            this.ScheduleSelectionStandard.HeaderText = "予定選別規格";
            this.ScheduleSelectionStandard.Name = "ScheduleSelectionStandard";
            this.ScheduleSelectionStandard.ReadOnly = true;
            // 
            // MnggrId
            // 
            this.MnggrId.HeaderText = "スペックボックスID";
            this.MnggrId.Name = "MnggrId";
            this.MnggrId.ReadOnly = true;
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItem2,
            this.プロファイル選択ToolStripMenuItem,
            this.nASCA連携ToolStripMenuItem,
            this.作業履歴照会ToolStripMenuItem,
            this.toolStripMenuItem1,
            this.lblVersion,
            this.toolMachineOperator,
            this.非定常メンテToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.RenderMode = System.Windows.Forms.ToolStripRenderMode.Professional;
            this.menuStrip1.Size = new System.Drawing.Size(984, 26);
            this.menuStrip1.TabIndex = 18;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolMacMat,
            this.toolMacResin,
            this.toolMacCond});
            this.toolStripMenuItem2.Image = global::ArmsMaintenance.Properties.Resources.db_update;
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(187, 22);
            this.toolStripMenuItem2.Text = "設備：[資材/条件] 割り付け";
            // 
            // toolMacMat
            // 
            this.toolMacMat.Name = "toolMacMat";
            this.toolMacMat.Size = new System.Drawing.Size(184, 22);
            this.toolMacMat.Text = "設備：資材情報編集";
            this.toolMacMat.Click += new System.EventHandler(this.toolMacMat_Click);
            // 
            // toolMacResin
            // 
            this.toolMacResin.Name = "toolMacResin";
            this.toolMacResin.Size = new System.Drawing.Size(184, 22);
            this.toolMacResin.Text = "設備：樹脂情報編集";
            this.toolMacResin.Click += new System.EventHandler(this.toolMacResin_Click);
            // 
            // toolMacCond
            // 
            this.toolMacCond.Name = "toolMacCond";
            this.toolMacCond.Size = new System.Drawing.Size(184, 22);
            this.toolMacCond.Text = "設備：条件情報編集";
            this.toolMacCond.Click += new System.EventHandler(this.toolMacCond_Click);
            // 
            // プロファイル選択ToolStripMenuItem
            // 
            this.プロファイル選択ToolStripMenuItem.Image = global::ArmsMaintenance.Properties.Resources.viewmag;
            this.プロファイル選択ToolStripMenuItem.Name = "プロファイル選択ToolStripMenuItem";
            this.プロファイル選択ToolStripMenuItem.Size = new System.Drawing.Size(132, 22);
            this.プロファイル選択ToolStripMenuItem.Text = "プロファイル選択";
            this.プロファイル選択ToolStripMenuItem.Click += new System.EventHandler(this.プロファイル選択ToolStripMenuItem_Click);
            // 
            // nASCA連携ToolStripMenuItem
            // 
            this.nASCA連携ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.カット完了品NASCA連携ToolStripMenuItem1,
            this.nASCAロット取り込みToolStripMenuItem,
            this.設備マスタ編集ToolStripMenuItem});
            this.nASCA連携ToolStripMenuItem.Image = global::ArmsMaintenance.Properties.Resources.NppUpdate;
            this.nASCA連携ToolStripMenuItem.Name = "nASCA連携ToolStripMenuItem";
            this.nASCA連携ToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.nASCA連携ToolStripMenuItem.Text = "NASCA連携";
            // 
            // カット完了品NASCA連携ToolStripMenuItem1
            // 
            this.カット完了品NASCA連携ToolStripMenuItem1.Name = "カット完了品NASCA連携ToolStripMenuItem1";
            this.カット完了品NASCA連携ToolStripMenuItem1.Size = new System.Drawing.Size(213, 22);
            this.カット完了品NASCA連携ToolStripMenuItem1.Text = "カット完了品NASCA連携";
            this.カット完了品NASCA連携ToolStripMenuItem1.Click += new System.EventHandler(this.カット完了品NASCA連携ToolStripMenuItem1_Click);
            // 
            // nASCAロット取り込みToolStripMenuItem
            // 
            this.nASCAロット取り込みToolStripMenuItem.Name = "nASCAロット取り込みToolStripMenuItem";
            this.nASCAロット取り込みToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.nASCAロット取り込みToolStripMenuItem.Text = "NASCAロット取り込み";
            this.nASCAロット取り込みToolStripMenuItem.Click += new System.EventHandler(this.nASCAロット取り込みToolStripMenuItem_Click);
            // 
            // 設備マスタ編集ToolStripMenuItem
            // 
            this.設備マスタ編集ToolStripMenuItem.Name = "設備マスタ編集ToolStripMenuItem";
            this.設備マスタ編集ToolStripMenuItem.Size = new System.Drawing.Size(213, 22);
            this.設備マスタ編集ToolStripMenuItem.Text = "設備マスタ編集";
            this.設備マスタ編集ToolStripMenuItem.Click += new System.EventHandler(this.設備マスタ編集ToolStripMenuItem_Click);
            // 
            // 作業履歴照会ToolStripMenuItem
            // 
            this.作業履歴照会ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.作業履歴照会ToolStripMenuItem1,
            this.toolCutWorkView,
            this.dBウェハー確認ToolStripMenuItem,
            this.cSV作業実績出力ToolStripMenuItem,
            this.ロットエラー履歴照会ToolStripMenuItem,
            this.ロット進捗確認ToolStripMenuItem});
            this.作業履歴照会ToolStripMenuItem.Image = global::ArmsMaintenance.Properties.Resources.kword;
            this.作業履歴照会ToolStripMenuItem.Name = "作業履歴照会ToolStripMenuItem";
            this.作業履歴照会ToolStripMenuItem.Size = new System.Drawing.Size(84, 22);
            this.作業履歴照会ToolStripMenuItem.Text = "レポート";
            // 
            // 作業履歴照会ToolStripMenuItem1
            // 
            this.作業履歴照会ToolStripMenuItem1.Name = "作業履歴照会ToolStripMenuItem1";
            this.作業履歴照会ToolStripMenuItem1.Size = new System.Drawing.Size(196, 22);
            this.作業履歴照会ToolStripMenuItem1.Text = "作業履歴照会";
            this.作業履歴照会ToolStripMenuItem1.Click += new System.EventHandler(this.作業履歴照会ToolStripMenuItem1_Click);
            // 
            // toolCutWorkView
            // 
            this.toolCutWorkView.Name = "toolCutWorkView";
            this.toolCutWorkView.Size = new System.Drawing.Size(196, 22);
            this.toolCutWorkView.Text = "カット作業履歴照会";
            this.toolCutWorkView.Click += new System.EventHandler(this.toolCutWorkView_Click);
            // 
            // dBウェハー確認ToolStripMenuItem
            // 
            this.dBウェハー確認ToolStripMenuItem.Name = "dBウェハー確認ToolStripMenuItem";
            this.dBウェハー確認ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.dBウェハー確認ToolStripMenuItem.Text = "DBウェハー確認";
            this.dBウェハー確認ToolStripMenuItem.Click += new System.EventHandler(this.dBウェハー確認ToolStripMenuItem_Click);
            // 
            // cSV作業実績出力ToolStripMenuItem
            // 
            this.cSV作業実績出力ToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("cSV作業実績出力ToolStripMenuItem.Image")));
            this.cSV作業実績出力ToolStripMenuItem.Name = "cSV作業実績出力ToolStripMenuItem";
            this.cSV作業実績出力ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.cSV作業実績出力ToolStripMenuItem.Text = "CSV作業実績出力";
            this.cSV作業実績出力ToolStripMenuItem.Click += new System.EventHandler(this.cSV作業実績出力ToolStripMenuItem_Click);
            // 
            // ロットエラー履歴照会ToolStripMenuItem
            // 
            this.ロットエラー履歴照会ToolStripMenuItem.Name = "ロットエラー履歴照会ToolStripMenuItem";
            this.ロットエラー履歴照会ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.ロットエラー履歴照会ToolStripMenuItem.Text = "ロットエラー履歴照会";
            this.ロットエラー履歴照会ToolStripMenuItem.Click += new System.EventHandler(this.ロットエラー履歴照会ToolStripMenuItem_Click);
            // 
            // ロット進捗確認ToolStripMenuItem
            // 
            this.ロット進捗確認ToolStripMenuItem.Name = "ロット進捗確認ToolStripMenuItem";
            this.ロット進捗確認ToolStripMenuItem.Size = new System.Drawing.Size(196, 22);
            this.ロット進捗確認ToolStripMenuItem.Text = "ロット進捗確認";
            this.ロット進捗確認ToolStripMenuItem.Click += new System.EventHandler(this.ロット進捗確認ToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolTouchPanelDefectMenu,
            this.toolManualInstruction,
            this.mAPダイボンド前ベーキング管理ToolStripMenuItem,
            this.メンテナンス履歴照会ToolStripMenuItem,
            this.toolRestrict,
            this.ロットトレース一括規制ToolStripMenuItem,
            this.debugToolStripMenuItem,
            this.仮想マガジンToolStripMenuItem,
            this.データメンテナンス定型文設定ToolStripMenuItem,
            this.カットラベル印刷設定ToolStripMenuItem,
            this.マーキング情報編集ToolStripMenuItem,
            this.スパッタ用トレイ割当て情報ToolStripMenuItem,
            this.資材在庫数編集ToolStripMenuItem});
            this.toolStripMenuItem1.Image = global::ArmsMaintenance.Properties.Resources.windows_list;
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(84, 22);
            this.toolStripMenuItem1.Text = "サブ画面";
            // 
            // toolTouchPanelDefectMenu
            // 
            this.toolTouchPanelDefectMenu.Name = "toolTouchPanelDefectMenu";
            this.toolTouchPanelDefectMenu.Size = new System.Drawing.Size(257, 22);
            this.toolTouchPanelDefectMenu.Text = "タッチパネル用不良入力画面";
            this.toolTouchPanelDefectMenu.Click += new System.EventHandler(this.toolTouchPanelDefectMenu_Click);
            // 
            // toolManualInstruction
            // 
            this.toolManualInstruction.Name = "toolManualInstruction";
            this.toolManualInstruction.Size = new System.Drawing.Size(257, 22);
            this.toolManualInstruction.Text = "指図手動発行";
            this.toolManualInstruction.Click += new System.EventHandler(this.toolManualInstruction_Click);
            // 
            // mAPダイボンド前ベーキング管理ToolStripMenuItem
            // 
            this.mAPダイボンド前ベーキング管理ToolStripMenuItem.Name = "mAPダイボンド前ベーキング管理ToolStripMenuItem";
            this.mAPダイボンド前ベーキング管理ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.mAPダイボンド前ベーキング管理ToolStripMenuItem.Text = "MAPダイボンド前ベーキング管理";
            this.mAPダイボンド前ベーキング管理ToolStripMenuItem.Click += new System.EventHandler(this.mAPダイボンド前ベーキング管理ToolStripMenuItem_Click);
            // 
            // メンテナンス履歴照会ToolStripMenuItem
            // 
            this.メンテナンス履歴照会ToolStripMenuItem.Name = "メンテナンス履歴照会ToolStripMenuItem";
            this.メンテナンス履歴照会ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.メンテナンス履歴照会ToolStripMenuItem.Text = "メンテナンス履歴照会";
            this.メンテナンス履歴照会ToolStripMenuItem.Click += new System.EventHandler(this.メンテナンス履歴照会ToolStripMenuItem_Click);
            // 
            // toolRestrict
            // 
            this.toolRestrict.Name = "toolRestrict";
            this.toolRestrict.Size = new System.Drawing.Size(257, 22);
            this.toolRestrict.Text = "流動規制情報検索";
            this.toolRestrict.Click += new System.EventHandler(this.toolRestrict_Click);
            // 
            // ロットトレース一括規制ToolStripMenuItem
            // 
            this.ロットトレース一括規制ToolStripMenuItem.Name = "ロットトレース一括規制ToolStripMenuItem";
            this.ロットトレース一括規制ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.ロットトレース一括規制ToolStripMenuItem.Text = "ロットトレース/一括規制";
            this.ロットトレース一括規制ToolStripMenuItem.Click += new System.EventHandler(this.ロットトレース一括規制ToolStripMenuItem_Click);
            // 
            // debugToolStripMenuItem
            // 
            this.debugToolStripMenuItem.Name = "debugToolStripMenuItem";
            this.debugToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.debugToolStripMenuItem.Text = "Debug";
            this.debugToolStripMenuItem.Visible = false;
            this.debugToolStripMenuItem.Click += new System.EventHandler(this.debugToolStripMenuItem_Click);
            // 
            // 仮想マガジンToolStripMenuItem
            // 
            this.仮想マガジンToolStripMenuItem.Name = "仮想マガジンToolStripMenuItem";
            this.仮想マガジンToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.仮想マガジンToolStripMenuItem.Text = "仮想マガジン";
            this.仮想マガジンToolStripMenuItem.Click += new System.EventHandler(this.仮想マガジンToolStripMenuItem_Click);
            // 
            // データメンテナンス定型文設定ToolStripMenuItem
            // 
            this.データメンテナンス定型文設定ToolStripMenuItem.Name = "データメンテナンス定型文設定ToolStripMenuItem";
            this.データメンテナンス定型文設定ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.データメンテナンス定型文設定ToolStripMenuItem.Text = "データメンテナンス定型文設定";
            this.データメンテナンス定型文設定ToolStripMenuItem.Click += new System.EventHandler(this.データメンテナンス定型文設定ToolStripMenuItem_Click);
            // 
            // カットラベル印刷設定ToolStripMenuItem
            // 
            this.カットラベル印刷設定ToolStripMenuItem.Name = "カットラベル印刷設定ToolStripMenuItem";
            this.カットラベル印刷設定ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.カットラベル印刷設定ToolStripMenuItem.Text = "カットラベル印刷設定";
            this.カットラベル印刷設定ToolStripMenuItem.Click += new System.EventHandler(this.カットラベル印刷設定ToolStripMenuItem_Click);
            // 
            // マーキング情報編集ToolStripMenuItem
            // 
            this.マーキング情報編集ToolStripMenuItem.Name = "マーキング情報編集ToolStripMenuItem";
            this.マーキング情報編集ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.マーキング情報編集ToolStripMenuItem.Text = "マーキング情報編集";
            this.マーキング情報編集ToolStripMenuItem.Click += new System.EventHandler(this.マーキング情報編集ToolStripMenuItem_Click);
            // 
            // スパッタ用トレイ割当て情報ToolStripMenuItem
            // 
            this.スパッタ用トレイ割当て情報ToolStripMenuItem.Name = "スパッタ用トレイ割当て情報ToolStripMenuItem";
            this.スパッタ用トレイ割当て情報ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.スパッタ用トレイ割当て情報ToolStripMenuItem.Text = "スパッタ用トレイ割当て情報";
            this.スパッタ用トレイ割当て情報ToolStripMenuItem.Click += new System.EventHandler(this.スパッタ用トレイ割当て情報ToolStripMenuItem_Click);
            // 
            // 資材在庫数編集ToolStripMenuItem
            // 
            this.資材在庫数編集ToolStripMenuItem.Name = "資材在庫数編集ToolStripMenuItem";
            this.資材在庫数編集ToolStripMenuItem.Size = new System.Drawing.Size(257, 22);
            this.資材在庫数編集ToolStripMenuItem.Text = "資材在庫数編集";
            this.資材在庫数編集ToolStripMenuItem.Click += new System.EventHandler(this.資材在庫数編集ToolStripMenuItem_Click);
            // 
            // lblVersion
            // 
            this.lblVersion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblVersion.BackColor = System.Drawing.SystemColors.Control;
            this.lblVersion.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.ReadOnly = true;
            this.lblVersion.Size = new System.Drawing.Size(90, 22);
            this.lblVersion.Text = "ver.";
            this.lblVersion.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // toolMachineOperator
            // 
            this.toolMachineOperator.Image = ((System.Drawing.Image)(resources.GetObject("toolMachineOperator.Image")));
            this.toolMachineOperator.Name = "toolMachineOperator";
            this.toolMachineOperator.Size = new System.Drawing.Size(184, 22);
            this.toolMachineOperator.Text = "作業者設定 (通知：10分毎)";
            this.toolMachineOperator.Click += new System.EventHandler(this.toolMachineOperator_Click);
            // 
            // 非定常メンテToolStripMenuItem
            // 
            this.非定常メンテToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.ロット情報全削除});
            this.非定常メンテToolStripMenuItem.Image = ((System.Drawing.Image)(resources.GetObject("非定常メンテToolStripMenuItem.Image")));
            this.非定常メンテToolStripMenuItem.Name = "非定常メンテToolStripMenuItem";
            this.非定常メンテToolStripMenuItem.Size = new System.Drawing.Size(96, 22);
            this.非定常メンテToolStripMenuItem.Text = "ロット削除";
            // 
            // ロット情報全削除
            // 
            this.ロット情報全削除.Name = "ロット情報全削除";
            this.ロット情報全削除.Size = new System.Drawing.Size(172, 22);
            this.ロット情報全削除.Text = "ロット情報全削除";
            this.ロット情報全削除.Click += new System.EventHandler(this.ロット情報全削除_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.bnItems);
            this.groupBox2.Controls.Add(this.grdMagazine);
            this.groupBox2.Location = new System.Drawing.Point(14, 187);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(958, 397);
            this.groupBox2.TabIndex = 21;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "一覧";
            // 
            // bnItems
            // 
            this.bnItems.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnItems.CountItem = this.bindingNavigatorCountItem;
            this.bnItems.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bnItems.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.toolRowMaxCount,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.toolAllSelect,
            this.toolAllCancel});
            this.bnItems.Location = new System.Drawing.Point(3, 369);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(952, 25);
            this.bnItems.TabIndex = 2;
            this.bnItems.Text = "bindingNavigator1";
            // 
            // bindingNavigatorAddNewItem
            // 
            this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorAddNewItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorAddNewItem.Image")));
            this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
            this.bindingNavigatorAddNewItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorAddNewItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorAddNewItem.Text = "新規追加";
            this.bindingNavigatorAddNewItem.Visible = false;
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(38, 22);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "項目の総数";
            this.bindingNavigatorCountItem.Visible = false;
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "削除";
            this.bindingNavigatorDeleteItem.Visible = false;
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "最初に移動";
            this.bindingNavigatorMoveFirstItem.Visible = false;
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "前に戻る";
            this.bindingNavigatorMovePreviousItem.Visible = false;
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator.Visible = false;
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 25);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "現在の場所";
            this.bindingNavigatorPositionItem.Visible = false;
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            this.bindingNavigatorSeparator1.Visible = false;
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "次に移動";
            this.bindingNavigatorMoveNextItem.Visible = false;
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "最後に移動";
            this.bindingNavigatorMoveLastItem.Visible = false;
            // 
            // toolRowMaxCount
            // 
            this.toolRowMaxCount.Name = "toolRowMaxCount";
            this.toolRowMaxCount.ReadOnly = true;
            this.toolRowMaxCount.Size = new System.Drawing.Size(50, 25);
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolAllSelect
            // 
            this.toolAllSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolAllSelect.Image")));
            this.toolAllSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAllSelect.Name = "toolAllSelect";
            this.toolAllSelect.Size = new System.Drawing.Size(64, 22);
            this.toolAllSelect.Text = "全選択";
            this.toolAllSelect.Click += new System.EventHandler(this.toolAllSelect_Click);
            // 
            // toolAllCancel
            // 
            this.toolAllCancel.Image = ((System.Drawing.Image)(resources.GetObject("toolAllCancel.Image")));
            this.toolAllCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAllCancel.Name = "toolAllCancel";
            this.toolAllCancel.Size = new System.Drawing.Size(64, 22);
            this.toolAllCancel.Text = "全解除";
            this.toolAllCancel.Click += new System.EventHandler(this.toolAllCancel_Click);
            // 
            // btnPSTester
            // 
            this.btnPSTester.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnPSTester.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPSTester.Image = ((System.Drawing.Image)(resources.GetObject("btnPSTester.Image")));
            this.btnPSTester.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPSTester.Location = new System.Drawing.Point(209, 591);
            this.btnPSTester.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnPSTester.Name = "btnPSTester";
            this.btnPSTester.Size = new System.Drawing.Size(120, 27);
            this.btnPSTester.TabIndex = 20;
            this.btnPSTester.Text = "強度データ出力";
            this.btnPSTester.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPSTester.UseVisualStyleBackColor = true;
            this.btnPSTester.Click += new System.EventHandler(this.btnPSTester_Click);
            // 
            // btnEditCutBlend
            // 
            this.btnEditCutBlend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEditCutBlend.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEditCutBlend.Image = global::ArmsMaintenance.Properties.Resources.edit;
            this.btnEditCutBlend.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEditCutBlend.Location = new System.Drawing.Point(578, 592);
            this.btnEditCutBlend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEditCutBlend.Name = "btnEditCutBlend";
            this.btnEditCutBlend.Size = new System.Drawing.Size(132, 27);
            this.btnEditCutBlend.TabIndex = 19;
            this.btnEditCutBlend.Text = "カットブレンド編集";
            this.btnEditCutBlend.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEditCutBlend.UseVisualStyleBackColor = true;
            this.btnEditCutBlend.Click += new System.EventHandler(this.btnEditCutBlend_Click);
            // 
            // btnCutBlend
            // 
            this.btnCutBlend.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnCutBlend.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCutBlend.Image = global::ArmsMaintenance.Properties.Resources.database_connect;
            this.btnCutBlend.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCutBlend.Location = new System.Drawing.Point(441, 592);
            this.btnCutBlend.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnCutBlend.Name = "btnCutBlend";
            this.btnCutBlend.Size = new System.Drawing.Size(131, 27);
            this.btnCutBlend.TabIndex = 17;
            this.btnCutBlend.Text = "カットブレンド完成";
            this.btnCutBlend.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCutBlend.UseVisualStyleBackColor = true;
            this.btnCutBlend.Click += new System.EventHandler(this.btnCutBlend_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEdit.Image = global::ArmsMaintenance.Properties.Resources.kword;
            this.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEdit.Location = new System.Drawing.Point(14, 592);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(87, 27);
            this.btnEdit.TabIndex = 15;
            this.btnEdit.Text = "編集";
            this.btnEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnInput
            // 
            this.btnInput.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnInput.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInput.Image = global::ArmsMaintenance.Properties.Resources.edit;
            this.btnInput.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnInput.Location = new System.Drawing.Point(871, 592);
            this.btnInput.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(101, 27);
            this.btnInput.TabIndex = 14;
            this.btnInput.Text = "実績入力";
            this.btnInput.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // button7
            // 
            this.button7.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button7.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.button7.Image = global::ArmsMaintenance.Properties.Resources.time;
            this.button7.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.button7.Location = new System.Drawing.Point(335, 592);
            this.button7.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(100, 27);
            this.button7.TabIndex = 13;
            this.button7.Text = "時間監視";
            this.button7.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.bnInput_Click);
            // 
            // btnAddRow
            // 
            this.btnAddRow.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnAddRow.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddRow.Image = global::ArmsMaintenance.Properties.Resources.edit_copy_7;
            this.btnAddRow.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddRow.Location = new System.Drawing.Point(107, 592);
            this.btnAddRow.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btnAddRow.Name = "btnAddRow";
            this.btnAddRow.Size = new System.Drawing.Size(96, 27);
            this.btnAddRow.TabIndex = 12;
            this.btnAddRow.Text = "ロット複製";
            this.btnAddRow.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddRow.UseVisualStyleBackColor = true;
            this.btnAddRow.Click += new System.EventHandler(this.btnAddRow_Click);
            // 
            // timer2
            // 
            this.timer2.Tick += new System.EventHandler(this.timer2_Tick);
            // 
            // FrmMagazineMainte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(984, 632);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.btnPSTester);
            this.Controls.Add(this.btnEditCutBlend);
            this.Controls.Add(this.btnCutBlend);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnInput);
            this.Controls.Add(this.button7);
            this.Controls.Add(this.btnAddRow);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.Name = "FrmMagazineMainte";
            this.Text = "ARMSデータメンテナンス";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMagazineMainte_FormClosing);
            this.Load += new System.EventHandler(this.FrmMagazineMainte_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInterval)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMagazine)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtMagNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnAddRow;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.TextBox txtResinGrp;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkAutoUpdate;
        private System.Windows.Forms.CheckBox chkOnlyActive;
        private System.Windows.Forms.NumericUpDown numInterval;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.DataGridView grdMagazine;
        private System.Windows.Forms.Button btnEdit;
        private System.Windows.Forms.Button btnCutBlend;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem nASCA連携ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem カット完了品NASCA連携ToolStripMenuItem1;
        private System.Windows.Forms.Button btnEditCutBlend;
        private System.Windows.Forms.ToolStripMenuItem nASCAロット取り込みToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem プロファイル選択ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 作業履歴照会ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 作業履歴照会ToolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem 設備マスタ編集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem dBウェハー確認ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem toolTouchPanelDefectMenu;
        private System.Windows.Forms.ToolStripMenuItem toolManualInstruction;
        private System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem toolMacMat;
        private System.Windows.Forms.ToolStripMenuItem toolMacResin;
        private System.Windows.Forms.ToolStripMenuItem toolMacCond;
        private System.Windows.Forms.ToolStripMenuItem mAPダイボンド前ベーキング管理ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cSV作業実績出力ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem メンテナンス履歴照会ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem toolCutWorkView;
        private System.Windows.Forms.ToolStripMenuItem toolRestrict;
        private System.Windows.Forms.ToolStripMenuItem ロットトレース一括規制ToolStripMenuItem;
        private System.Windows.Forms.Button btnPSTester;
        private System.Windows.Forms.ToolStripMenuItem debugToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 仮想マガジンToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.BindingNavigator bnItems;
        private System.Windows.Forms.ToolStripButton bindingNavigatorAddNewItem;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton toolAllSelect;
        private System.Windows.Forms.ToolStripButton toolAllCancel;
        private System.Windows.Forms.Button btnProfileSelect;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProfileNm;
		private System.Windows.Forms.TextBox txtProfileId;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtDbThrowDT;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtLineCD;
        private System.Windows.Forms.ToolStripMenuItem ロットエラー履歴照会ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox lblVersion;
        private System.Windows.Forms.ToolStripMenuItem ロット進捗確認ToolStripMenuItem;
        private System.Windows.Forms.ToolStripTextBox toolRowMaxCount;
        private System.Windows.Forms.Label label8;
		private System.Windows.Forms.ComboBox cmbProcess;
		private System.Windows.Forms.ToolStripMenuItem データメンテナンス定型文設定ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem カットラベル印刷設定ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem マーキング情報編集ToolStripMenuItem;
        private System.Windows.Forms.CheckBox chkLifeTestResultInfo;
        private System.Windows.Forms.ToolStripMenuItem スパッタ用トレイ割当て情報ToolStripMenuItem;
        private System.Windows.Forms.TextBox txtTypeNo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ToolStripMenuItem 資材在庫数編集ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectFg;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DieShearTestFg;
        private System.Windows.Forms.DataGridViewTextBoxColumn NascaLotNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn MagazineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn NowCompProcessNM;
        private System.Windows.Forms.DataGridViewTextBoxColumn ResinGr;
        private System.Windows.Forms.DataGridViewCheckBoxColumn NewFg;
        private System.Windows.Forms.DataGridViewTextBoxColumn NowCompProcess;
        private System.Windows.Forms.DataGridViewTextBoxColumn CutblendCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBThrowDt;
        private System.Windows.Forms.DataGridViewTextBoxColumn ProfileNm;
        private System.Windows.Forms.DataGridViewTextBoxColumn MacGroup;
        private System.Windows.Forms.DataGridViewTextBoxColumn TempCutBlendNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn LifeTestResult;
        private System.Windows.Forms.DataGridViewTextBoxColumn BlendCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn ScheduleSelectionStandard;
        private System.Windows.Forms.DataGridViewTextBoxColumn MnggrId;
        private System.Windows.Forms.ToolStripMenuItem toolMachineOperator;
        private System.Windows.Forms.Timer timer2;
        private System.Windows.Forms.ToolStripMenuItem 非定常メンテToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ロット情報全削除;
    }
}