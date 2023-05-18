namespace ArmsMaintenance
{
    partial class FrmCutBlendMainte
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCutBlendMainte));
            this.btnUpdateDefect = new System.Windows.Forms.Button();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.treeAsmLot = new System.Windows.Forms.TreeView();
            this.btnPrintLabel = new System.Windows.Forms.Button();
            this.treeCutBlend = new System.Windows.Forms.TreeView();
            this.txtMacGroup = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnDelete = new System.Windows.Forms.ToolStripButton();
            this.btnPrintLabel2 = new System.Windows.Forms.Button();
            this.txtPastLot = new System.Windows.Forms.TextBox();
            this.rdoLine = new System.Windows.Forms.RadioButton();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.rdoPastLot = new System.Windows.Forms.RadioButton();
            this.grpTranInfo = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.dtpWorkEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpWorkStart = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.txtCutComment = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.cmbLotNo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtInspectCt = new System.Windows.Forms.TextBox();
            this.rdoInspectSample = new System.Windows.Forms.RadioButton();
            this.rdoInspectAll = new System.Windows.Forms.RadioButton();
            this.txtEmpCd = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtDefTotal = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.grdDefect = new System.Windows.Forms.DataGridView();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.treeWork = new System.Windows.Forms.TreeView();
            this.chkNascaStart = new System.Windows.Forms.CheckBox();
            this.chkNascaEnd = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.grpTranInfo.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDefect)).BeginInit();
            this.groupBox5.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnUpdateDefect
            // 
            this.btnUpdateDefect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUpdateDefect.Image = global::ArmsMaintenance.Properties.Resources.kword;
            this.btnUpdateDefect.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdateDefect.Location = new System.Drawing.Point(827, 91);
            this.btnUpdateDefect.Name = "btnUpdateDefect";
            this.btnUpdateDefect.Size = new System.Drawing.Size(146, 27);
            this.btnUpdateDefect.TabIndex = 32;
            this.btnUpdateDefect.Text = "実績修正";
            this.btnUpdateDefect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdateDefect.UseVisualStyleBackColor = true;
            this.btnUpdateDefect.Click += new System.EventHandler(this.btnUpdateDefect_Click);
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(422, 15);
            this.txtLotNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.ReadOnly = true;
            this.txtLotNo.Size = new System.Drawing.Size(185, 22);
            this.txtLotNo.TabIndex = 29;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(334, 21);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(82, 14);
            this.label1.TabIndex = 28;
            this.label1.Text = "ブレンドロットNo";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox1.Controls.Add(this.treeAsmLot);
            this.groupBox1.Location = new System.Drawing.Point(18, 324);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(246, 193);
            this.groupBox1.TabIndex = 30;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ブレンド情報";
            // 
            // treeAsmLot
            // 
            this.treeAsmLot.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeAsmLot.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeAsmLot.FullRowSelect = true;
            this.treeAsmLot.HideSelection = false;
            this.treeAsmLot.ItemHeight = 15;
            this.treeAsmLot.Location = new System.Drawing.Point(3, 18);
            this.treeAsmLot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeAsmLot.Name = "treeAsmLot";
            this.treeAsmLot.Size = new System.Drawing.Size(240, 172);
            this.treeAsmLot.TabIndex = 33;
            // 
            // btnPrintLabel
            // 
            this.btnPrintLabel.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrintLabel.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintLabel.Image")));
            this.btnPrintLabel.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintLabel.Location = new System.Drawing.Point(607, 51);
            this.btnPrintLabel.Name = "btnPrintLabel";
            this.btnPrintLabel.Size = new System.Drawing.Size(146, 27);
            this.btnPrintLabel.TabIndex = 31;
            this.btnPrintLabel.Text = "カットラベル再印刷";
            this.btnPrintLabel.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrintLabel.UseVisualStyleBackColor = true;
            this.btnPrintLabel.Click += new System.EventHandler(this.btnPrintLabel_Click);
            // 
            // treeCutBlend
            // 
            this.treeCutBlend.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeCutBlend.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeCutBlend.FullRowSelect = true;
            this.treeCutBlend.HideSelection = false;
            this.treeCutBlend.ItemHeight = 15;
            this.treeCutBlend.Location = new System.Drawing.Point(6, 20);
            this.treeCutBlend.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeCutBlend.Name = "treeCutBlend";
            this.treeCutBlend.Size = new System.Drawing.Size(234, 162);
            this.treeCutBlend.TabIndex = 32;
            this.treeCutBlend.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeCutBlend_AfterSelect);
            // 
            // txtMacGroup
            // 
            this.txtMacGroup.Location = new System.Drawing.Point(87, 13);
            this.txtMacGroup.Name = "txtMacGroup";
            this.txtMacGroup.Size = new System.Drawing.Size(133, 22);
            this.txtMacGroup.TabIndex = 39;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(234, 12);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(65, 23);
            this.btnSearch.TabIndex = 41;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.toolStrip1);
            this.groupBox3.Controls.Add(this.treeCutBlend);
            this.groupBox3.Location = new System.Drawing.Point(21, 114);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(246, 212);
            this.groupBox3.TabIndex = 17;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "ブレンドロット一覧";
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnDelete});
            this.toolStrip1.Location = new System.Drawing.Point(3, 184);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(240, 25);
            this.toolStrip1.TabIndex = 33;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnDelete
            // 
            this.btnDelete.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnDelete.Image = ((System.Drawing.Image)(resources.GetObject("btnDelete.Image")));
            this.btnDelete.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(52, 22);
            this.btnDelete.Text = "削除";
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // btnPrintLabel2
            // 
            this.btnPrintLabel2.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnPrintLabel2.Image = ((System.Drawing.Image)(resources.GetObject("btnPrintLabel2.Image")));
            this.btnPrintLabel2.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnPrintLabel2.Location = new System.Drawing.Point(769, 51);
            this.btnPrintLabel2.Name = "btnPrintLabel2";
            this.btnPrintLabel2.Size = new System.Drawing.Size(170, 27);
            this.btnPrintLabel2.TabIndex = 31;
            this.btnPrintLabel2.Text = "検査・照合用ラベル印刷";
            this.btnPrintLabel2.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnPrintLabel2.UseVisualStyleBackColor = true;
            this.btnPrintLabel2.Click += new System.EventHandler(this.btnPrintLabel2_Click);
            // 
            // txtPastLot
            // 
            this.txtPastLot.Enabled = false;
            this.txtPastLot.Location = new System.Drawing.Point(87, 45);
            this.txtPastLot.Name = "txtPastLot";
            this.txtPastLot.Size = new System.Drawing.Size(133, 22);
            this.txtPastLot.TabIndex = 39;
            // 
            // rdoLine
            // 
            this.rdoLine.AutoSize = true;
            this.rdoLine.Checked = true;
            this.rdoLine.Location = new System.Drawing.Point(9, 16);
            this.rdoLine.Name = "rdoLine";
            this.rdoLine.Size = new System.Drawing.Size(51, 18);
            this.rdoLine.TabIndex = 43;
            this.rdoLine.TabStop = true;
            this.rdoLine.Text = "ライン";
            this.rdoLine.UseVisualStyleBackColor = true;
            this.rdoLine.CheckedChanged += new System.EventHandler(this.rdoSearchCond_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.rdoPastLot);
            this.groupBox4.Controls.Add(this.rdoLine);
            this.groupBox4.Controls.Add(this.btnSearch);
            this.groupBox4.Controls.Add(this.txtMacGroup);
            this.groupBox4.Controls.Add(this.txtPastLot);
            this.groupBox4.Location = new System.Drawing.Point(18, 5);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(305, 72);
            this.groupBox4.TabIndex = 16;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "検索条件";
            // 
            // rdoPastLot
            // 
            this.rdoPastLot.AutoSize = true;
            this.rdoPastLot.Location = new System.Drawing.Point(9, 46);
            this.rdoPastLot.Name = "rdoPastLot";
            this.rdoPastLot.Size = new System.Drawing.Size(73, 18);
            this.rdoPastLot.TabIndex = 43;
            this.rdoPastLot.Text = "過去ロット";
            this.rdoPastLot.UseVisualStyleBackColor = true;
            this.rdoPastLot.CheckedChanged += new System.EventHandler(this.rdoSearchCond_CheckedChanged);
            // 
            // grpTranInfo
            // 
            this.grpTranInfo.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grpTranInfo.Controls.Add(this.label2);
            this.grpTranInfo.Controls.Add(this.dtpWorkEnd);
            this.grpTranInfo.Controls.Add(this.label5);
            this.grpTranInfo.Controls.Add(this.dtpWorkStart);
            this.grpTranInfo.Controls.Add(this.label4);
            this.grpTranInfo.Controls.Add(this.txtCutComment);
            this.grpTranInfo.Controls.Add(this.groupBox2);
            this.grpTranInfo.Location = new System.Drawing.Point(418, 114);
            this.grpTranInfo.Name = "grpTranInfo";
            this.grpTranInfo.Size = new System.Drawing.Size(601, 400);
            this.grpTranInfo.TabIndex = 39;
            this.grpTranInfo.TabStop = false;
            this.grpTranInfo.Text = "実績情報";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(271, 9);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(88, 14);
            this.label2.TabIndex = 48;
            this.label2.Text = "カット実績コメント";
            // 
            // dtpWorkEnd
            // 
            this.dtpWorkEnd.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkEnd.Location = new System.Drawing.Point(66, 52);
            this.dtpWorkEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkEnd.Name = "dtpWorkEnd";
            this.dtpWorkEnd.Size = new System.Drawing.Size(189, 22);
            this.dtpWorkEnd.TabIndex = 47;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(5, 56);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 14);
            this.label5.TabIndex = 46;
            this.label5.Text = "完了時間";
            // 
            // dtpWorkStart
            // 
            this.dtpWorkStart.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkStart.Location = new System.Drawing.Point(66, 26);
            this.dtpWorkStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkStart.Name = "dtpWorkStart";
            this.dtpWorkStart.Size = new System.Drawing.Size(189, 22);
            this.dtpWorkStart.TabIndex = 45;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(5, 29);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 14);
            this.label4.TabIndex = 44;
            this.label4.Text = "開始時間";
            // 
            // txtCutComment
            // 
            this.txtCutComment.Location = new System.Drawing.Point(273, 26);
            this.txtCutComment.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCutComment.Multiline = true;
            this.txtCutComment.Name = "txtCutComment";
            this.txtCutComment.Size = new System.Drawing.Size(317, 48);
            this.txtCutComment.TabIndex = 43;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.cmbLotNo);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.txtEmpCd);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtDefTotal);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.grdDefect);
            this.groupBox2.Location = new System.Drawing.Point(6, 78);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(584, 322);
            this.groupBox2.TabIndex = 42;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "不良";
            // 
            // cmbLotNo
            // 
            this.cmbLotNo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbLotNo.FormattingEnabled = true;
            this.cmbLotNo.Location = new System.Drawing.Point(55, 23);
            this.cmbLotNo.Name = "cmbLotNo";
            this.cmbLotNo.Size = new System.Drawing.Size(185, 22);
            this.cmbLotNo.TabIndex = 18;
            this.cmbLotNo.SelectionChangeCommitted += new System.EventHandler(this.cmbLotNo_SelectionChangeCommitted);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 14);
            this.label3.TabIndex = 17;
            this.label3.Text = "ロットNo";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtInspectCt);
            this.groupBox6.Controls.Add(this.rdoInspectSample);
            this.groupBox6.Controls.Add(this.rdoInspectAll);
            this.groupBox6.Location = new System.Drawing.Point(359, 42);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(204, 39);
            this.groupBox6.TabIndex = 16;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "検査数";
            // 
            // txtInspectCt
            // 
            this.txtInspectCt.Location = new System.Drawing.Point(131, 11);
            this.txtInspectCt.Name = "txtInspectCt";
            this.txtInspectCt.Size = new System.Drawing.Size(67, 22);
            this.txtInspectCt.TabIndex = 17;
            // 
            // rdoInspectSample
            // 
            this.rdoInspectSample.AutoSize = true;
            this.rdoInspectSample.Checked = true;
            this.rdoInspectSample.Location = new System.Drawing.Point(68, 13);
            this.rdoInspectSample.Name = "rdoInspectSample";
            this.rdoInspectSample.Size = new System.Drawing.Size(57, 18);
            this.rdoInspectSample.TabIndex = 1;
            this.rdoInspectSample.TabStop = true;
            this.rdoInspectSample.Text = "抜取り";
            this.rdoInspectSample.UseVisualStyleBackColor = true;
            // 
            // rdoInspectAll
            // 
            this.rdoInspectAll.AutoSize = true;
            this.rdoInspectAll.Location = new System.Drawing.Point(6, 13);
            this.rdoInspectAll.Name = "rdoInspectAll";
            this.rdoInspectAll.Size = new System.Drawing.Size(49, 18);
            this.rdoInspectAll.TabIndex = 0;
            this.rdoInspectAll.Text = "全数";
            this.rdoInspectAll.UseVisualStyleBackColor = true;
            // 
            // txtEmpCd
            // 
            this.txtEmpCd.Location = new System.Drawing.Point(218, 54);
            this.txtEmpCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtEmpCd.Name = "txtEmpCd";
            this.txtEmpCd.Size = new System.Drawing.Size(104, 22);
            this.txtEmpCd.TabIndex = 12;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(169, 57);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 14);
            this.label15.TabIndex = 11;
            this.label15.Text = "検査者";
            // 
            // txtDefTotal
            // 
            this.txtDefTotal.Location = new System.Drawing.Point(46, 54);
            this.txtDefTotal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDefTotal.Name = "txtDefTotal";
            this.txtDefTotal.ReadOnly = true;
            this.txtDefTotal.Size = new System.Drawing.Size(100, 22);
            this.txtDefTotal.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(9, 57);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 14);
            this.label8.TabIndex = 7;
            this.label8.Text = "合計";
            // 
            // grdDefect
            // 
            this.grdDefect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDefect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdDefect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDefect.Location = new System.Drawing.Point(6, 89);
            this.grdDefect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grdDefect.Name = "grdDefect";
            this.grdDefect.RowTemplate.Height = 21;
            this.grdDefect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDefect.Size = new System.Drawing.Size(572, 229);
            this.grdDefect.TabIndex = 1;
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.treeWork);
            this.groupBox5.Location = new System.Drawing.Point(270, 114);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(144, 400);
            this.groupBox5.TabIndex = 40;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "作業名";
            // 
            // treeWork
            // 
            this.treeWork.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.treeWork.FullRowSelect = true;
            this.treeWork.HideSelection = false;
            this.treeWork.Location = new System.Drawing.Point(6, 20);
            this.treeWork.Name = "treeWork";
            this.treeWork.Size = new System.Drawing.Size(132, 377);
            this.treeWork.TabIndex = 0;
            this.treeWork.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeWork_AfterSelect);
            // 
            // chkNascaStart
            // 
            this.chkNascaStart.AutoSize = true;
            this.chkNascaStart.Location = new System.Drawing.Point(744, 15);
            this.chkNascaStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkNascaStart.Name = "chkNascaStart";
            this.chkNascaStart.Size = new System.Drawing.Size(112, 18);
            this.chkNascaStart.TabIndex = 41;
            this.chkNascaStart.Text = "NASCA開始連携";
            this.chkNascaStart.UseVisualStyleBackColor = true;
            // 
            // chkNascaEnd
            // 
            this.chkNascaEnd.AutoSize = true;
            this.chkNascaEnd.Location = new System.Drawing.Point(861, 15);
            this.chkNascaEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkNascaEnd.Name = "chkNascaEnd";
            this.chkNascaEnd.Size = new System.Drawing.Size(112, 18);
            this.chkNascaEnd.TabIndex = 40;
            this.chkNascaEnd.Text = "NASCA完了連携";
            this.chkNascaEnd.UseVisualStyleBackColor = true;
            // 
            // FrmCutBlendMainte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1020, 529);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.chkNascaStart);
            this.Controls.Add(this.chkNascaEnd);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.btnUpdateDefect);
            this.Controls.Add(this.btnPrintLabel2);
            this.Controls.Add(this.btnPrintLabel);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtLotNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.grpTranInfo);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmCutBlendMainte";
            this.Text = "FrmCutBlendMainte";
            this.Load += new System.EventHandler(this.FrmCutBlendMainte_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.grpTranInfo.ResumeLayout(false);
            this.grpTranInfo.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDefect)).EndInit();
            this.groupBox5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnPrintLabel;
        private System.Windows.Forms.Button btnUpdateDefect;
        private System.Windows.Forms.TreeView treeCutBlend;
        private System.Windows.Forms.TreeView treeAsmLot;
		private System.Windows.Forms.TextBox txtMacGroup;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.GroupBox groupBox3;
		private System.Windows.Forms.ToolStrip toolStrip1;
		private System.Windows.Forms.ToolStripButton btnDelete;
        private System.Windows.Forms.Button btnPrintLabel2;
        private System.Windows.Forms.TextBox txtPastLot;
        private System.Windows.Forms.RadioButton rdoLine;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.RadioButton rdoPastLot;
        private System.Windows.Forms.GroupBox grpTranInfo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker dtpWorkEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpWorkStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCutComment;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.TextBox txtInspectCt;
        private System.Windows.Forms.RadioButton rdoInspectSample;
        private System.Windows.Forms.RadioButton rdoInspectAll;
        private System.Windows.Forms.TextBox txtEmpCd;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtDefTotal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView grdDefect;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.TreeView treeWork;
        private System.Windows.Forms.ComboBox cmbLotNo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkNascaStart;
        private System.Windows.Forms.CheckBox chkNascaEnd;
    }
}