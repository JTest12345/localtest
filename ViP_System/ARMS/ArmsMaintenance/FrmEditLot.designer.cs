namespace ArmsMaintenance
{
    partial class FrmEditLot
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
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.btnAddInspection = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbProc = new System.Windows.Forms.ComboBox();
            this.grdInspections = new System.Windows.Forms.DataGridView();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkLoopCondChange = new System.Windows.Forms.CheckBox();
            this.txtScheduleSelectionStandard = new System.Windows.Forms.TextBox();
            this.txtDBThrowDt = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.numMoveStockCt = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.txtMacGroup = new System.Windows.Forms.TextBox();
            this.chkChangePointLot = new System.Windows.Forms.CheckBox();
            this.chkFullInspection = new System.Windows.Forms.CheckBox();
            this.chkWBMapping = new System.Windows.Forms.CheckBox();
            this.numLotsize = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCutBlendCd = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtResinGr2 = new System.Windows.Forms.TextBox();
            this.txtResinGr = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.chkWarning = new System.Windows.Forms.CheckBox();
            this.txtBlendCd = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.chkLifeTest = new System.Windows.Forms.CheckBox();
            this.chkKHLTest = new System.Windows.Forms.CheckBox();
            this.chkColorTest = new System.Windows.Forms.CheckBox();
            this.txtTypeCd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.txtProfileNm = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtProfileId = new System.Windows.Forms.TextBox();
            this.btnSelectProfile = new System.Windows.Forms.Button();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdInspections)).BeginInit();
            this.groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMoveStockCt)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLotsize)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.btnAddInspection);
            this.groupBox5.Controls.Add(this.label3);
            this.groupBox5.Controls.Add(this.cmbProc);
            this.groupBox5.Controls.Add(this.grdInspections);
            this.groupBox5.Location = new System.Drawing.Point(297, 73);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Size = new System.Drawing.Size(332, 491);
            this.groupBox5.TabIndex = 30;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "検査・抜き取り";
            // 
            // btnAddInspection
            // 
            this.btnAddInspection.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddInspection.Image = global::ArmsMaintenance.Properties.Resources.edit_add;
            this.btnAddInspection.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddInspection.Location = new System.Drawing.Point(229, 29);
            this.btnAddInspection.Name = "btnAddInspection";
            this.btnAddInspection.Size = new System.Drawing.Size(87, 27);
            this.btnAddInspection.TabIndex = 36;
            this.btnAddInspection.Text = "追加";
            this.btnAddInspection.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddInspection.UseVisualStyleBackColor = true;
            this.btnAddInspection.Click += new System.EventHandler(this.btnAddInspection_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 35;
            this.label3.Text = "検査工程";
            // 
            // cmbProc
            // 
            this.cmbProc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProc.FormattingEnabled = true;
            this.cmbProc.Location = new System.Drawing.Point(73, 32);
            this.cmbProc.Name = "cmbProc";
            this.cmbProc.Size = new System.Drawing.Size(150, 22);
            this.cmbProc.TabIndex = 1;
            // 
            // grdInspections
            // 
            this.grdInspections.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdInspections.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdInspections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdInspections.Location = new System.Drawing.Point(6, 67);
            this.grdInspections.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grdInspections.MultiSelect = false;
            this.grdInspections.Name = "grdInspections";
            this.grdInspections.RowHeadersVisible = false;
            this.grdInspections.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.grdInspections.RowTemplate.Height = 21;
            this.grdInspections.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdInspections.Size = new System.Drawing.Size(320, 420);
            this.grdInspections.TabIndex = 0;
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox3.Controls.Add(this.chkLoopCondChange);
            this.groupBox3.Controls.Add(this.txtScheduleSelectionStandard);
            this.groupBox3.Controls.Add(this.txtDBThrowDt);
            this.groupBox3.Controls.Add(this.label11);
            this.groupBox3.Controls.Add(this.label10);
            this.groupBox3.Controls.Add(this.numMoveStockCt);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtMacGroup);
            this.groupBox3.Controls.Add(this.chkChangePointLot);
            this.groupBox3.Controls.Add(this.chkFullInspection);
            this.groupBox3.Controls.Add(this.chkWBMapping);
            this.groupBox3.Controls.Add(this.numLotsize);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.txtCutBlendCd);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtResinGr2);
            this.groupBox3.Controls.Add(this.txtResinGr);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.chkWarning);
            this.groupBox3.Controls.Add(this.txtBlendCd);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.chkLifeTest);
            this.groupBox3.Controls.Add(this.chkKHLTest);
            this.groupBox3.Controls.Add(this.chkColorTest);
            this.groupBox3.Location = new System.Drawing.Point(14, 73);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(276, 524);
            this.groupBox3.TabIndex = 29;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "特性";
            // 
            // chkLoopCondChange
            // 
            this.chkLoopCondChange.AutoSize = true;
            this.chkLoopCondChange.Location = new System.Drawing.Point(9, 498);
            this.chkLoopCondChange.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkLoopCondChange.Name = "chkLoopCondChange";
            this.chkLoopCondChange.Size = new System.Drawing.Size(103, 18);
            this.chkLoopCondChange.TabIndex = 41;
            this.chkLoopCondChange.Text = "ループ条件変更";
            this.chkLoopCondChange.UseVisualStyleBackColor = true;
            // 
            // txtScheduleSelectionStandard
            // 
            this.txtScheduleSelectionStandard.Location = new System.Drawing.Point(10, 267);
            this.txtScheduleSelectionStandard.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtScheduleSelectionStandard.MaxLength = 64;
            this.txtScheduleSelectionStandard.Name = "txtScheduleSelectionStandard";
            this.txtScheduleSelectionStandard.Size = new System.Drawing.Size(255, 22);
            this.txtScheduleSelectionStandard.TabIndex = 40;
            // 
            // txtDBThrowDt
            // 
            this.txtDBThrowDt.Location = new System.Drawing.Point(10, 221);
            this.txtDBThrowDt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDBThrowDt.Name = "txtDBThrowDt";
            this.txtDBThrowDt.Size = new System.Drawing.Size(255, 22);
            this.txtDBThrowDt.TabIndex = 40;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(7, 249);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(79, 14);
            this.label11.TabIndex = 39;
            this.label11.Text = "予定選別規格";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(7, 203);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(63, 14);
            this.label10.TabIndex = 39;
            this.label10.Text = "D/B投入日";
            // 
            // numMoveStockCt
            // 
            this.numMoveStockCt.Location = new System.Drawing.Point(140, 313);
            this.numMoveStockCt.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numMoveStockCt.Name = "numMoveStockCt";
            this.numMoveStockCt.Size = new System.Drawing.Size(122, 22);
            this.numMoveStockCt.TabIndex = 38;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(138, 295);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(79, 14);
            this.label5.TabIndex = 37;
            this.label5.Text = "部門間移動数";
            // 
            // txtMacGroup
            // 
            this.txtMacGroup.Location = new System.Drawing.Point(169, 345);
            this.txtMacGroup.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMacGroup.Name = "txtMacGroup";
            this.txtMacGroup.ReadOnly = true;
            this.txtMacGroup.Size = new System.Drawing.Size(95, 22);
            this.txtMacGroup.TabIndex = 36;
            this.txtMacGroup.Visible = false;
            // 
            // chkChangePointLot
            // 
            this.chkChangePointLot.AutoSize = true;
            this.chkChangePointLot.Location = new System.Drawing.Point(9, 476);
            this.chkChangePointLot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkChangePointLot.Name = "chkChangePointLot";
            this.chkChangePointLot.Size = new System.Drawing.Size(103, 18);
            this.chkChangePointLot.TabIndex = 18;
            this.chkChangePointLot.Text = "抜き取り検査有";
            this.chkChangePointLot.UseVisualStyleBackColor = true;
            // 
            // chkFullInspection
            // 
            this.chkFullInspection.AutoSize = true;
            this.chkFullInspection.Location = new System.Drawing.Point(9, 454);
            this.chkFullInspection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkFullInspection.Name = "chkFullInspection";
            this.chkFullInspection.Size = new System.Drawing.Size(122, 18);
            this.chkFullInspection.TabIndex = 17;
            this.chkFullInspection.Text = "全数自動検査対象";
            this.chkFullInspection.UseVisualStyleBackColor = true;
            // 
            // chkWBMapping
            // 
            this.chkWBMapping.AutoSize = true;
            this.chkWBMapping.Location = new System.Drawing.Point(9, 432);
            this.chkWBMapping.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkWBMapping.Name = "chkWBMapping";
            this.chkWBMapping.Size = new System.Drawing.Size(113, 18);
            this.chkWBMapping.TabIndex = 16;
            this.chkWBMapping.Text = "WBマッピングフラグ";
            this.chkWBMapping.UseVisualStyleBackColor = true;
            // 
            // numLotsize
            // 
            this.numLotsize.Location = new System.Drawing.Point(10, 313);
            this.numLotsize.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numLotsize.Name = "numLotsize";
            this.numLotsize.Size = new System.Drawing.Size(122, 22);
            this.numLotsize.TabIndex = 15;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 295);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 14);
            this.label4.TabIndex = 14;
            this.label4.Text = "指図数量";
            // 
            // txtCutBlendCd
            // 
            this.txtCutBlendCd.Location = new System.Drawing.Point(10, 175);
            this.txtCutBlendCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCutBlendCd.Name = "txtCutBlendCd";
            this.txtCutBlendCd.Size = new System.Drawing.Size(255, 22);
            this.txtCutBlendCd.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 157);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 14);
            this.label7.TabIndex = 12;
            this.label7.Text = "カットブレンドグループ";
            // 
            // txtResinGr2
            // 
            this.txtResinGr2.Location = new System.Drawing.Point(9, 129);
            this.txtResinGr2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtResinGr2.Name = "txtResinGr2";
            this.txtResinGr2.Size = new System.Drawing.Size(255, 22);
            this.txtResinGr2.TabIndex = 11;
            // 
            // txtResinGr
            // 
            this.txtResinGr.Location = new System.Drawing.Point(9, 84);
            this.txtResinGr.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtResinGr.Name = "txtResinGr";
            this.txtResinGr.Size = new System.Drawing.Size(255, 22);
            this.txtResinGr.TabIndex = 11;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(6, 112);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(151, 14);
            this.label12.TabIndex = 10;
            this.label12.Text = "樹脂グループコード(PB実装2)";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 66);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(96, 14);
            this.label6.TabIndex = 10;
            this.label6.Text = "樹脂グループコード";
            // 
            // chkWarning
            // 
            this.chkWarning.AutoSize = true;
            this.chkWarning.Location = new System.Drawing.Point(10, 344);
            this.chkWarning.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkWarning.Name = "chkWarning";
            this.chkWarning.Size = new System.Drawing.Size(50, 18);
            this.chkWarning.TabIndex = 9;
            this.chkWarning.Text = "警告";
            this.chkWarning.UseVisualStyleBackColor = true;
            // 
            // txtBlendCd
            // 
            this.txtBlendCd.Location = new System.Drawing.Point(9, 38);
            this.txtBlendCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBlendCd.Name = "txtBlendCd";
            this.txtBlendCd.Size = new System.Drawing.Size(255, 22);
            this.txtBlendCd.TabIndex = 8;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 20);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 14);
            this.label9.TabIndex = 7;
            this.label9.Text = "ブレンドCD";
            // 
            // chkLifeTest
            // 
            this.chkLifeTest.AutoSize = true;
            this.chkLifeTest.Location = new System.Drawing.Point(9, 388);
            this.chkLifeTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkLifeTest.Name = "chkLifeTest";
            this.chkLifeTest.Size = new System.Drawing.Size(75, 18);
            this.chkLifeTest.TabIndex = 6;
            this.chkLifeTest.Text = "ライフ試験";
            this.chkLifeTest.UseVisualStyleBackColor = true;
            // 
            // chkKHLTest
            // 
            this.chkKHLTest.AutoSize = true;
            this.chkKHLTest.Location = new System.Drawing.Point(9, 410);
            this.chkKHLTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkKHLTest.Name = "chkKHLTest";
            this.chkKHLTest.Size = new System.Drawing.Size(122, 18);
            this.chkKHLTest.TabIndex = 5;
            this.chkKHLTest.Text = "吸湿保管点灯試験";
            this.chkKHLTest.UseVisualStyleBackColor = true;
            // 
            // chkColorTest
            // 
            this.chkColorTest.AutoSize = true;
            this.chkColorTest.Location = new System.Drawing.Point(10, 366);
            this.chkColorTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkColorTest.Name = "chkColorTest";
            this.chkColorTest.Size = new System.Drawing.Size(98, 18);
            this.chkColorTest.TabIndex = 0;
            this.chkColorTest.Text = "先行色調確認";
            this.chkColorTest.UseVisualStyleBackColor = true;
            // 
            // txtTypeCd
            // 
            this.txtTypeCd.Location = new System.Drawing.Point(296, 11);
            this.txtTypeCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTypeCd.Name = "txtTypeCd";
            this.txtTypeCd.ReadOnly = true;
            this.txtTypeCd.Size = new System.Drawing.Size(189, 22);
            this.txtTypeCd.TabIndex = 35;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 14);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 34;
            this.label2.Text = "Type";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(61, 11);
            this.txtLotNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.ReadOnly = true;
            this.txtLotNo.Size = new System.Drawing.Size(159, 22);
            this.txtLotNo.TabIndex = 33;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 14);
            this.label1.TabIndex = 32;
            this.label1.Text = "LotNo";
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUpdate.Image = global::ArmsMaintenance.Properties.Resources.save_all;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.Location = new System.Drawing.Point(526, 570);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(100, 27);
            this.btnUpdate.TabIndex = 31;
            this.btnUpdate.Text = "保存";
            this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // txtProfileNm
            // 
            this.txtProfileNm.Location = new System.Drawing.Point(132, 47);
            this.txtProfileNm.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtProfileNm.Name = "txtProfileNm";
            this.txtProfileNm.ReadOnly = true;
            this.txtProfileNm.Size = new System.Drawing.Size(429, 22);
            this.txtProfileNm.TabIndex = 37;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(15, 50);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(40, 14);
            this.label8.TabIndex = 36;
            this.label8.Text = "Profile";
            // 
            // txtProfileId
            // 
            this.txtProfileId.Location = new System.Drawing.Point(61, 47);
            this.txtProfileId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtProfileId.Name = "txtProfileId";
            this.txtProfileId.ReadOnly = true;
            this.txtProfileId.Size = new System.Drawing.Size(65, 22);
            this.txtProfileId.TabIndex = 38;
            // 
            // btnSelectProfile
            // 
            this.btnSelectProfile.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectProfile.Image = global::ArmsMaintenance.Properties.Resources.viewmag;
            this.btnSelectProfile.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectProfile.Location = new System.Drawing.Point(567, 44);
            this.btnSelectProfile.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectProfile.Name = "btnSelectProfile";
            this.btnSelectProfile.Size = new System.Drawing.Size(62, 27);
            this.btnSelectProfile.TabIndex = 39;
            this.btnSelectProfile.Text = "選択";
            this.btnSelectProfile.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectProfile.UseVisualStyleBackColor = true;
            this.btnSelectProfile.Click += new System.EventHandler(this.btnSelectProfile_Click);
            // 
            // FrmEditLot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(644, 608);
            this.Controls.Add(this.btnSelectProfile);
            this.Controls.Add(this.txtProfileId);
            this.Controls.Add(this.txtProfileNm);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtTypeCd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLotNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox3);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "FrmEditLot";
            this.Text = "FrmEditLot";
            this.Load += new System.EventHandler(this.FrmEditLot_Load);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdInspections)).EndInit();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numMoveStockCt)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numLotsize)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView grdInspections;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtCutBlendCd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtResinGr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.CheckBox chkWarning;
        private System.Windows.Forms.TextBox txtBlendCd;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkLifeTest;
        private System.Windows.Forms.CheckBox chkKHLTest;
        private System.Windows.Forms.CheckBox chkColorTest;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtTypeCd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnAddInspection;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmbProc;
        private System.Windows.Forms.NumericUpDown numLotsize;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkFullInspection;
        private System.Windows.Forms.CheckBox chkWBMapping;
        private System.Windows.Forms.CheckBox chkChangePointLot;
        private System.Windows.Forms.TextBox txtMacGroup;
		private System.Windows.Forms.NumericUpDown numMoveStockCt;
		private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtProfileNm;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtProfileId;
        private System.Windows.Forms.Button btnSelectProfile;
        private System.Windows.Forms.TextBox txtDBThrowDt;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkLoopCondChange;
        private System.Windows.Forms.TextBox txtScheduleSelectionStandard;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtResinGr2;
        private System.Windows.Forms.Label label12;
    }
}