namespace ArmsMaintenance
{
    partial class FrmNascaSync
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmNascaSync));
            this.btnSync = new System.Windows.Forms.Button();
            this.grdCutLot = new System.Windows.Forms.DataGridView();
            this.colBlend = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.最新の情報に更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.grdAsmLot = new System.Windows.Forms.DataGridView();
            this.dataGridViewCheckBoxColumn1 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.label3 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.btnInput = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMacGroup = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnInputMultiLot = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.panel1 = new System.Windows.Forms.Panel();
            this.rbAllAsmLot = new System.Windows.Forms.RadioButton();
            this.rbTargetAsmLot = new System.Windows.Forms.RadioButton();
            this.btnSelectProcess = new System.Windows.Forms.Button();
            this.chkTargetEndProcess = new System.Windows.Forms.CheckBox();
            this.btnStopReq = new System.Windows.Forms.Button();
            this.txtErrLog = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdCutLot)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAsmLot)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnSync
            // 
            this.btnSync.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSync.Location = new System.Drawing.Point(614, 344);
            this.btnSync.Name = "btnSync";
            this.btnSync.Size = new System.Drawing.Size(142, 25);
            this.btnSync.TabIndex = 7;
            this.btnSync.Text = "NASCA連携";
            this.btnSync.UseVisualStyleBackColor = true;
            this.btnSync.Click += new System.EventHandler(this.btnSync_Click);
            // 
            // grdCutLot
            // 
            this.grdCutLot.AllowUserToAddRows = false;
            this.grdCutLot.AllowUserToDeleteRows = false;
            this.grdCutLot.AllowUserToResizeRows = false;
            this.grdCutLot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdCutLot.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdCutLot.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdCutLot.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colBlend});
            this.grdCutLot.Location = new System.Drawing.Point(7, 55);
            this.grdCutLot.Name = "grdCutLot";
            this.grdCutLot.RowTemplate.Height = 21;
            this.grdCutLot.Size = new System.Drawing.Size(345, 243);
            this.grdCutLot.TabIndex = 6;
            // 
            // colBlend
            // 
            this.colBlend.HeaderText = "選択";
            this.colBlend.Name = "colBlend";
            this.colBlend.Width = 37;
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(16, 406);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(740, 95);
            this.txtLog.TabIndex = 9;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.最新の情報に更新ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(775, 26);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 最新の情報に更新ToolStripMenuItem
            // 
            this.最新の情報に更新ToolStripMenuItem.Name = "最新の情報に更新ToolStripMenuItem";
            this.最新の情報に更新ToolStripMenuItem.Size = new System.Drawing.Size(116, 22);
            this.最新の情報に更新ToolStripMenuItem.Text = "最新の情報に更新";
            this.最新の情報に更新ToolStripMenuItem.Click += new System.EventHandler(this.最新の情報に更新ToolStripMenuItem_Click);
            // 
            // grdAsmLot
            // 
            this.grdAsmLot.AllowUserToAddRows = false;
            this.grdAsmLot.AllowUserToDeleteRows = false;
            this.grdAsmLot.AllowUserToResizeRows = false;
            this.grdAsmLot.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdAsmLot.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdAsmLot.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdAsmLot.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewCheckBoxColumn1});
            this.grdAsmLot.Location = new System.Drawing.Point(6, 77);
            this.grdAsmLot.Name = "grdAsmLot";
            this.grdAsmLot.RowTemplate.Height = 21;
            this.grdAsmLot.Size = new System.Drawing.Size(369, 221);
            this.grdAsmLot.TabIndex = 12;
            // 
            // dataGridViewCheckBoxColumn1
            // 
            this.dataGridViewCheckBoxColumn1.HeaderText = "選択";
            this.dataGridViewCheckBoxColumn1.Name = "dataGridViewCheckBoxColumn1";
            this.dataGridViewCheckBoxColumn1.Width = 37;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(85, 218);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 16;
            this.label3.Text = "ロット番号";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(119, 52);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(149, 22);
            this.txtLotNo.TabIndex = 15;
            this.txtLotNo.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txtLotNo_KeyDown);
            // 
            // btnInput
            // 
            this.btnInput.Location = new System.Drawing.Point(299, 50);
            this.btnInput.Name = "btnInput";
            this.btnInput.Size = new System.Drawing.Size(75, 25);
            this.btnInput.TabIndex = 14;
            this.btnInput.Text = "入力";
            this.btnInput.UseVisualStyleBackColor = true;
            this.btnInput.Click += new System.EventHandler(this.btnInput_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMacGroup);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.grdCutLot);
            this.groupBox1.Location = new System.Drawing.Point(16, 29);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(359, 309);
            this.groupBox1.TabIndex = 17;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "CT工程実績情報連携";
            // 
            // txtMacGroup
            // 
            this.txtMacGroup.Location = new System.Drawing.Point(46, 27);
            this.txtMacGroup.Name = "txtMacGroup";
            this.txtMacGroup.Size = new System.Drawing.Size(184, 22);
            this.txtMacGroup.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 31);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(33, 14);
            this.label4.TabIndex = 17;
            this.label4.Text = "ライン";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnInputMultiLot);
            this.groupBox2.Controls.Add(this.txtLotNo);
            this.groupBox2.Controls.Add(this.panel1);
            this.groupBox2.Controls.Add(this.btnSelectProcess);
            this.groupBox2.Controls.Add(this.chkTargetEndProcess);
            this.groupBox2.Controls.Add(this.grdAsmLot);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnInput);
            this.groupBox2.Location = new System.Drawing.Point(381, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(382, 309);
            this.groupBox2.TabIndex = 18;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "MD以前工程情報連携";
            // 
            // btnInputMultiLot
            // 
            this.btnInputMultiLot.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnInputMultiLot.ImageIndex = 1;
            this.btnInputMultiLot.ImageList = this.imageList1;
            this.btnInputMultiLot.Location = new System.Drawing.Point(273, 52);
            this.btnInputMultiLot.Name = "btnInputMultiLot";
            this.btnInputMultiLot.Size = new System.Drawing.Size(22, 22);
            this.btnInputMultiLot.TabIndex = 72;
            this.btnInputMultiLot.UseVisualStyleBackColor = true;
            this.btnInputMultiLot.Click += new System.EventHandler(this.btnInputMultiLot_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ACT_MULTISELECT_IN.png");
            this.imageList1.Images.SetKeyName(1, "ACT_MULTISELECT_EP.png");
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.rbAllAsmLot);
            this.panel1.Controls.Add(this.rbTargetAsmLot);
            this.panel1.Location = new System.Drawing.Point(7, 19);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(152, 55);
            this.panel1.TabIndex = 71;
            // 
            // rbAllAsmLot
            // 
            this.rbAllAsmLot.AutoSize = true;
            this.rbAllAsmLot.Location = new System.Drawing.Point(5, 4);
            this.rbAllAsmLot.Name = "rbAllAsmLot";
            this.rbAllAsmLot.Size = new System.Drawing.Size(142, 18);
            this.rbAllAsmLot.TabIndex = 19;
            this.rbAllAsmLot.TabStop = true;
            this.rbAllAsmLot.Text = "流動中の全アッセンロット";
            this.rbAllAsmLot.UseVisualStyleBackColor = true;
            this.rbAllAsmLot.CheckedChanged += new System.EventHandler(this.rbAllAsmLot_CheckedChanged);
            // 
            // rbTargetAsmLot
            // 
            this.rbTargetAsmLot.AutoSize = true;
            this.rbTargetAsmLot.Checked = true;
            this.rbTargetAsmLot.Location = new System.Drawing.Point(5, 34);
            this.rbTargetAsmLot.Name = "rbTargetAsmLot";
            this.rbTargetAsmLot.Size = new System.Drawing.Size(108, 18);
            this.rbTargetAsmLot.TabIndex = 69;
            this.rbTargetAsmLot.TabStop = true;
            this.rbTargetAsmLot.Text = "指定アッセンロット";
            this.rbTargetAsmLot.UseVisualStyleBackColor = true;
            // 
            // btnSelectProcess
            // 
            this.btnSelectProcess.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectProcess.ImageIndex = 1;
            this.btnSelectProcess.ImageList = this.imageList1;
            this.btnSelectProcess.Location = new System.Drawing.Point(327, 21);
            this.btnSelectProcess.Name = "btnSelectProcess";
            this.btnSelectProcess.Size = new System.Drawing.Size(22, 22);
            this.btnSelectProcess.TabIndex = 68;
            this.btnSelectProcess.UseVisualStyleBackColor = true;
            this.btnSelectProcess.Click += new System.EventHandler(this.btnSelectProcess_Click);
            // 
            // chkTargetEndProcess
            // 
            this.chkTargetEndProcess.AutoSize = true;
            this.chkTargetEndProcess.Location = new System.Drawing.Point(197, 24);
            this.chkTargetEndProcess.Name = "chkTargetEndProcess";
            this.chkTargetEndProcess.Size = new System.Drawing.Size(131, 18);
            this.chkTargetEndProcess.TabIndex = 18;
            this.chkTargetEndProcess.Text = "連携終了作業を指定";
            this.chkTargetEndProcess.UseVisualStyleBackColor = true;
            // 
            // btnStopReq
            // 
            this.btnStopReq.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnStopReq.Location = new System.Drawing.Point(466, 344);
            this.btnStopReq.Name = "btnStopReq";
            this.btnStopReq.Size = new System.Drawing.Size(142, 25);
            this.btnStopReq.TabIndex = 19;
            this.btnStopReq.Text = "連携中断";
            this.btnStopReq.UseVisualStyleBackColor = true;
            this.btnStopReq.Click += new System.EventHandler(this.btnStopReq_Click);
            // 
            // txtErrLog
            // 
            this.txtErrLog.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtErrLog.Location = new System.Drawing.Point(16, 530);
            this.txtErrLog.Multiline = true;
            this.txtErrLog.Name = "txtErrLog";
            this.txtErrLog.ReadOnly = true;
            this.txtErrLog.Size = new System.Drawing.Size(740, 115);
            this.txtErrLog.TabIndex = 20;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(20, 513);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 14);
            this.label1.TabIndex = 21;
            this.label1.Text = "連携エラー";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(20, 385);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 14);
            this.label2.TabIndex = 22;
            this.label2.Text = "連携ログ";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(386, 371);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(357, 28);
            this.label6.TabIndex = 25;
            this.label6.Text = "※作業指定なしで連携ボタンを押すと最新の完了工程まで連携されます。\r\n　　実績入力など間違いがないかご確認のうえ実行して下さい。";
            // 
            // FrmNascaSync
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 657);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtErrLog);
            this.Controls.Add(this.btnStopReq);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.btnSync);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(750, 650);
            this.Name = "FrmNascaSync";
            this.Text = "FrmNascaSync";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmNascaSync_FormClosing);
            this.Load += new System.EventHandler(this.FrmNascaSync_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdCutLot)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdAsmLot)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSync;
        private System.Windows.Forms.DataGridView grdCutLot;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colBlend;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 最新の情報に更新ToolStripMenuItem;
        private System.Windows.Forms.DataGridView grdAsmLot;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dataGridViewCheckBoxColumn1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.Button btnInput;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Button btnStopReq;
        private System.Windows.Forms.TextBox txtErrLog;
        private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox txtMacGroup;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.CheckBox chkTargetEndProcess;
		private System.Windows.Forms.ImageList imageList1;
		private System.Windows.Forms.Button btnSelectProcess;
		private System.Windows.Forms.RadioButton rbTargetAsmLot;
		private System.Windows.Forms.RadioButton rbAllAsmLot;
		private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button btnInputMultiLot;
        private System.Windows.Forms.Label label6;
    }
}