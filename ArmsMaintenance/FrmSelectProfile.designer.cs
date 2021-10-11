namespace ArmsMaintenance
{
    partial class FrmSelectProfile
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
            this.grdProfiles = new System.Windows.Forms.DataGridView();
            this.btnSetActive = new System.Windows.Forms.Button();
            this.grdBOM = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtScheduleSelectionStandard = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtDbThrowDt = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.txtLastUpdDt = new System.Windows.Forms.TextBox();
            this.txtLotSize = new System.Windows.Forms.TextBox();
            this.txtMnfctKb = new System.Windows.Forms.TextBox();
            this.txtInspection = new System.Windows.Forms.TextBox();
            this.txtAimRank = new System.Windows.Forms.TextBox();
            this.txtResinGr = new System.Windows.Forms.TextBox();
            this.txtBlendCd = new System.Windows.Forms.TextBox();
            this.txtTypeCd = new System.Windows.Forms.TextBox();
            this.txtProfileNm = new System.Windows.Forms.TextBox();
            this.txtProfileNo = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtDbThrowDtCondition = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbLineNoCondition = new System.Windows.Forms.ComboBox();
            this.label18 = new System.Windows.Forms.Label();
            this.cmbMnfctKbCondition = new System.Windows.Forms.ComboBox();
            this.label16 = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txtTypeCdCondition = new System.Windows.Forms.TextBox();
            this.txtResinGroupCondition = new System.Windows.Forms.TextBox();
            this.chkNewProfile = new System.Windows.Forms.CheckBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            ((System.ComponentModel.ISupportInitialize)(this.grdProfiles)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBOM)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdProfiles
            // 
            this.grdProfiles.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdProfiles.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdProfiles.ImeMode = System.Windows.Forms.ImeMode.On;
            this.grdProfiles.Location = new System.Drawing.Point(13, 105);
            this.grdProfiles.Name = "grdProfiles";
            this.grdProfiles.RowTemplate.Height = 21;
            this.grdProfiles.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdProfiles.Size = new System.Drawing.Size(671, 161);
            this.grdProfiles.TabIndex = 0;
            this.grdProfiles.SelectionChanged += new System.EventHandler(this.grdProfiles_SelectionChanged);
            // 
            // btnSetActive
            // 
            this.btnSetActive.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSetActive.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSetActive.Location = new System.Drawing.Point(560, 614);
            this.btnSetActive.Name = "btnSetActive";
            this.btnSetActive.Size = new System.Drawing.Size(124, 27);
            this.btnSetActive.TabIndex = 1;
            this.btnSetActive.Text = "有効フラグ設定";
            this.btnSetActive.UseVisualStyleBackColor = true;
            this.btnSetActive.Click += new System.EventHandler(this.btnSetActive_Click);
            // 
            // grdBOM
            // 
            this.grdBOM.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdBOM.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdBOM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdBOM.Location = new System.Drawing.Point(364, 32);
            this.grdBOM.Name = "grdBOM";
            this.grdBOM.RowTemplate.Height = 21;
            this.grdBOM.Size = new System.Drawing.Size(302, 295);
            this.grdBOM.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtScheduleSelectionStandard);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtDbThrowDt);
            this.groupBox1.Controls.Add(this.label12);
            this.groupBox1.Controls.Add(this.txtLastUpdDt);
            this.groupBox1.Controls.Add(this.txtLotSize);
            this.groupBox1.Controls.Add(this.txtMnfctKb);
            this.groupBox1.Controls.Add(this.txtInspection);
            this.groupBox1.Controls.Add(this.txtAimRank);
            this.groupBox1.Controls.Add(this.txtResinGr);
            this.groupBox1.Controls.Add(this.txtBlendCd);
            this.groupBox1.Controls.Add(this.txtTypeCd);
            this.groupBox1.Controls.Add(this.txtProfileNm);
            this.groupBox1.Controls.Add(this.txtProfileNo);
            this.groupBox1.Controls.Add(this.label11);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.grdBOM);
            this.groupBox1.Location = new System.Drawing.Point(12, 276);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(672, 333);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "内容確認";
            // 
            // txtScheduleSelectionStandard
            // 
            this.txtScheduleSelectionStandard.Location = new System.Drawing.Point(85, 301);
            this.txtScheduleSelectionStandard.Name = "txtScheduleSelectionStandard";
            this.txtScheduleSelectionStandard.ReadOnly = true;
            this.txtScheduleSelectionStandard.Size = new System.Drawing.Size(275, 22);
            this.txtScheduleSelectionStandard.TabIndex = 26;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(7, 304);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 14);
            this.label14.TabIndex = 25;
            this.label14.Text = "予定選別規格";
            // 
            // txtDbThrowDt
            // 
            this.txtDbThrowDt.Location = new System.Drawing.Point(85, 273);
            this.txtDbThrowDt.Name = "txtDbThrowDt";
            this.txtDbThrowDt.ReadOnly = true;
            this.txtDbThrowDt.Size = new System.Drawing.Size(275, 22);
            this.txtDbThrowDt.TabIndex = 26;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(7, 276);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(63, 14);
            this.label12.TabIndex = 25;
            this.label12.Text = "D/B投入日";
            // 
            // txtLastUpdDt
            // 
            this.txtLastUpdDt.Location = new System.Drawing.Point(85, 245);
            this.txtLastUpdDt.Name = "txtLastUpdDt";
            this.txtLastUpdDt.ReadOnly = true;
            this.txtLastUpdDt.Size = new System.Drawing.Size(275, 22);
            this.txtLastUpdDt.TabIndex = 24;
            // 
            // txtLotSize
            // 
            this.txtLotSize.Location = new System.Drawing.Point(256, 217);
            this.txtLotSize.Name = "txtLotSize";
            this.txtLotSize.ReadOnly = true;
            this.txtLotSize.Size = new System.Drawing.Size(100, 22);
            this.txtLotSize.TabIndex = 23;
            // 
            // txtMnfctKb
            // 
            this.txtMnfctKb.Location = new System.Drawing.Point(85, 217);
            this.txtMnfctKb.Name = "txtMnfctKb";
            this.txtMnfctKb.ReadOnly = true;
            this.txtMnfctKb.Size = new System.Drawing.Size(100, 22);
            this.txtMnfctKb.TabIndex = 22;
            // 
            // txtInspection
            // 
            this.txtInspection.Location = new System.Drawing.Point(85, 189);
            this.txtInspection.Name = "txtInspection";
            this.txtInspection.ReadOnly = true;
            this.txtInspection.Size = new System.Drawing.Size(274, 22);
            this.txtInspection.TabIndex = 21;
            // 
            // txtAimRank
            // 
            this.txtAimRank.Location = new System.Drawing.Point(85, 161);
            this.txtAimRank.Name = "txtAimRank";
            this.txtAimRank.ReadOnly = true;
            this.txtAimRank.Size = new System.Drawing.Size(274, 22);
            this.txtAimRank.TabIndex = 20;
            // 
            // txtResinGr
            // 
            this.txtResinGr.Location = new System.Drawing.Point(85, 133);
            this.txtResinGr.Name = "txtResinGr";
            this.txtResinGr.ReadOnly = true;
            this.txtResinGr.Size = new System.Drawing.Size(274, 22);
            this.txtResinGr.TabIndex = 19;
            // 
            // txtBlendCd
            // 
            this.txtBlendCd.Location = new System.Drawing.Point(85, 105);
            this.txtBlendCd.Name = "txtBlendCd";
            this.txtBlendCd.ReadOnly = true;
            this.txtBlendCd.Size = new System.Drawing.Size(274, 22);
            this.txtBlendCd.TabIndex = 18;
            // 
            // txtTypeCd
            // 
            this.txtTypeCd.Location = new System.Drawing.Point(85, 77);
            this.txtTypeCd.Name = "txtTypeCd";
            this.txtTypeCd.ReadOnly = true;
            this.txtTypeCd.Size = new System.Drawing.Size(274, 22);
            this.txtTypeCd.TabIndex = 17;
            // 
            // txtProfileNm
            // 
            this.txtProfileNm.Location = new System.Drawing.Point(85, 49);
            this.txtProfileNm.Name = "txtProfileNm";
            this.txtProfileNm.ReadOnly = true;
            this.txtProfileNm.Size = new System.Drawing.Size(274, 22);
            this.txtProfileNm.TabIndex = 16;
            // 
            // txtProfileNo
            // 
            this.txtProfileNo.Location = new System.Drawing.Point(85, 21);
            this.txtProfileNo.Name = "txtProfileNo";
            this.txtProfileNo.ReadOnly = true;
            this.txtProfileNo.Size = new System.Drawing.Size(274, 22);
            this.txtProfileNo.TabIndex = 15;
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(361, 15);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(32, 14);
            this.label11.TabIndex = 14;
            this.label11.Text = "BOM";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(191, 220);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(60, 14);
            this.label10.TabIndex = 13;
            this.label10.Text = "ロットサイズ";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(7, 248);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(67, 14);
            this.label9.TabIndex = 12;
            this.label9.Text = "最終更新日";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(7, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(55, 14);
            this.label8.TabIndex = 11;
            this.label8.Text = "量産区分";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 192);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 14);
            this.label7.TabIndex = 10;
            this.label7.Text = "検査工程";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(54, 14);
            this.label6.TabIndex = 9;
            this.label6.Text = "狙いランク";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 108);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 14);
            this.label5.TabIndex = 8;
            this.label5.Text = "ブレンドCD";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 136);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 14);
            this.label4.TabIndex = 7;
            this.label4.Text = "樹脂Gr";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 52);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(31, 14);
            this.label3.TabIndex = 6;
            this.label3.Text = "名称";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(7, 80);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(31, 14);
            this.label2.TabIndex = 5;
            this.label2.Text = "型番";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 24);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(46, 14);
            this.label1.TabIndex = 4;
            this.label1.Text = "管理No";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(305, 18);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(63, 14);
            this.label13.TabIndex = 26;
            this.label13.Text = "D/B投入日";
            // 
            // txtDbThrowDtCondition
            // 
            this.txtDbThrowDtCondition.Location = new System.Drawing.Point(374, 15);
            this.txtDbThrowDtCondition.Name = "txtDbThrowDtCondition";
            this.txtDbThrowDtCondition.Size = new System.Drawing.Size(176, 22);
            this.txtDbThrowDtCondition.TabIndex = 27;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(569, 28);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(84, 23);
            this.btnSearch.TabIndex = 28;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.cmbLineNoCondition);
            this.groupBox2.Controls.Add(this.label18);
            this.groupBox2.Controls.Add(this.cmbMnfctKbCondition);
            this.groupBox2.Controls.Add(this.label16);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.label17);
            this.groupBox2.Controls.Add(this.txtTypeCdCondition);
            this.groupBox2.Controls.Add(this.txtResinGroupCondition);
            this.groupBox2.Controls.Add(this.chkNewProfile);
            this.groupBox2.Controls.Add(this.label13);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Controls.Add(this.txtDbThrowDtCondition);
            this.groupBox2.Location = new System.Drawing.Point(24, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(671, 99);
            this.groupBox2.TabIndex = 29;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "検索条件";
            // 
            // cmbLineNoCondition
            // 
            this.cmbLineNoCondition.FormattingEnabled = true;
            this.cmbLineNoCondition.Location = new System.Drawing.Point(374, 43);
            this.cmbLineNoCondition.Name = "cmbLineNoCondition";
            this.cmbLineNoCondition.Size = new System.Drawing.Size(176, 22);
            this.cmbLineNoCondition.TabIndex = 38;
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(305, 46);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(33, 14);
            this.label18.TabIndex = 37;
            this.label18.Text = "ライン";
            // 
            // cmbMnfctKbCondition
            // 
            this.cmbMnfctKbCondition.FormattingEnabled = true;
            this.cmbMnfctKbCondition.Items.AddRange(new object[] {
            "",
            "なし",
            "量産",
            "試作",
            "量産試作",
            "製造中止",
            "初期流動品",
            "出荷不可品"});
            this.cmbMnfctKbCondition.Location = new System.Drawing.Point(95, 71);
            this.cmbMnfctKbCondition.Name = "cmbMnfctKbCondition";
            this.cmbMnfctKbCondition.Size = new System.Drawing.Size(176, 22);
            this.cmbMnfctKbCondition.TabIndex = 36;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(27, 74);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 14);
            this.label16.TabIndex = 31;
            this.label16.Text = "量産区分";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(26, 46);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(69, 14);
            this.label15.TabIndex = 32;
            this.label15.Text = "樹脂グループ";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(26, 18);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(31, 14);
            this.label17.TabIndex = 33;
            this.label17.Text = "型番";
            // 
            // txtTypeCdCondition
            // 
            this.txtTypeCdCondition.Location = new System.Drawing.Point(95, 15);
            this.txtTypeCdCondition.Name = "txtTypeCdCondition";
            this.txtTypeCdCondition.Size = new System.Drawing.Size(176, 22);
            this.txtTypeCdCondition.TabIndex = 34;
            // 
            // txtResinGroupCondition
            // 
            this.txtResinGroupCondition.Location = new System.Drawing.Point(95, 43);
            this.txtResinGroupCondition.Name = "txtResinGroupCondition";
            this.txtResinGroupCondition.Size = new System.Drawing.Size(176, 22);
            this.txtResinGroupCondition.TabIndex = 35;
            // 
            // chkNewProfile
            // 
            this.chkNewProfile.AutoSize = true;
            this.chkNewProfile.Location = new System.Drawing.Point(308, 73);
            this.chkNewProfile.Name = "chkNewProfile";
            this.chkNewProfile.Size = new System.Drawing.Size(193, 18);
            this.chkNewProfile.TabIndex = 29;
            this.chkNewProfile.Text = "現在流動中のプロファイルのみ表示";
            this.chkNewProfile.UseVisualStyleBackColor = true;
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel});
            this.statusStrip.Location = new System.Drawing.Point(0, 647);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(695, 22);
            this.statusStrip.TabIndex = 30;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel
            // 
            this.toolStripStatusLabel.Name = "toolStripStatusLabel";
            this.toolStripStatusLabel.Size = new System.Drawing.Size(0, 17);
            // 
            // FrmSelectProfile
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 669);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnSetActive);
            this.Controls.Add(this.grdProfiles);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmSelectProfile";
            this.Text = "プロファイル選択";
            this.Load += new System.EventHandler(this.FrmSelectProfile_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdProfiles)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdBOM)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grdProfiles;
        private System.Windows.Forms.Button btnSetActive;
        private System.Windows.Forms.DataGridView grdBOM;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLastUpdDt;
        private System.Windows.Forms.TextBox txtLotSize;
        private System.Windows.Forms.TextBox txtMnfctKb;
        private System.Windows.Forms.TextBox txtInspection;
        private System.Windows.Forms.TextBox txtAimRank;
        private System.Windows.Forms.TextBox txtResinGr;
        private System.Windows.Forms.TextBox txtBlendCd;
        private System.Windows.Forms.TextBox txtTypeCd;
        private System.Windows.Forms.TextBox txtProfileNm;
        private System.Windows.Forms.TextBox txtProfileNo;
        private System.Windows.Forms.TextBox txtDbThrowDt;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtDbThrowDtCondition;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox chkNewProfile;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel;
        private System.Windows.Forms.TextBox txtScheduleSelectionStandard;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.ComboBox cmbLineNoCondition;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.ComboBox cmbMnfctKbCondition;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtTypeCdCondition;
        private System.Windows.Forms.TextBox txtResinGroupCondition;
    }
}