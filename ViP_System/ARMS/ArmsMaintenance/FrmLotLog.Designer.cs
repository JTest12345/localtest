namespace ArmsMaintenance
{
    partial class FrmLotLog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLotLog));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.InDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lineno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProcNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ProcNm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LotNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MagazineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MsgVAL = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.txtLineNo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkError = new System.Windows.Forms.CheckBox();
            this.txtMagazineNo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLogMsg = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtLotNO = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.dtpInTo = new System.Windows.Forms.DateTimePicker();
            this.dtpInFrom = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.表示ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.排出確認モードToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.groupBox2.SuspendLayout();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bnItems);
            this.groupBox1.Controls.Add(this.dgvItems);
            this.groupBox1.Location = new System.Drawing.Point(12, 160);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(852, 433);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一覧";
            // 
            // bnItems
            // 
            this.bnItems.AddNewItem = this.bindingNavigatorAddNewItem;
            this.bnItems.BindingSource = this.bsItems;
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
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem});
            this.bnItems.Location = new System.Drawing.Point(3, 405);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(846, 25);
            this.bnItems.TabIndex = 1;
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
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(29, 22);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "項目の総数";
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
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 25);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "現在の場所";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
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
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.AllowUserToResizeRows = false;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItems.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.InDT,
            this.lineno,
            this.ProcNo,
            this.ProcNm,
            this.LotNO,
            this.MagazineNo,
            this.MsgVAL});
            this.dgvItems.DataSource = this.bsItems;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItems.DefaultCellStyle = dataGridViewCellStyle2;
            this.dgvItems.Location = new System.Drawing.Point(6, 18);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.ReadOnly = true;
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowTemplate.Height = 21;
            this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvItems.Size = new System.Drawing.Size(840, 384);
            this.dgvItems.TabIndex = 0;
            // 
            // InDT
            // 
            this.InDT.DataPropertyName = "InDt";
            this.InDT.Frozen = true;
            this.InDT.HeaderText = "発生日時";
            this.InDT.Name = "InDT";
            this.InDT.ReadOnly = true;
            this.InDT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.InDT.Width = 120;
            // 
            // lineno
            // 
            this.lineno.DataPropertyName = "LineNo";
            this.lineno.Frozen = true;
            this.lineno.HeaderText = "ライン番号";
            this.lineno.Name = "lineno";
            this.lineno.ReadOnly = true;
            this.lineno.Width = 80;
            // 
            // ProcNo
            // 
            this.ProcNo.DataPropertyName = "ProcNo";
            this.ProcNo.Frozen = true;
            this.ProcNo.HeaderText = "工程No";
            this.ProcNo.Name = "ProcNo";
            this.ProcNo.ReadOnly = true;
            this.ProcNo.Visible = false;
            // 
            // ProcNm
            // 
            this.ProcNm.DataPropertyName = "ProcNm";
            this.ProcNm.Frozen = true;
            this.ProcNm.HeaderText = "工程名";
            this.ProcNm.Name = "ProcNm";
            this.ProcNm.ReadOnly = true;
            // 
            // LotNO
            // 
            this.LotNO.DataPropertyName = "LotNo";
            this.LotNO.Frozen = true;
            this.LotNO.HeaderText = "ロット番号";
            this.LotNO.Name = "LotNO";
            this.LotNO.ReadOnly = true;
            this.LotNO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LotNO.Width = 120;
            // 
            // MagazineNo
            // 
            this.MagazineNo.DataPropertyName = "MagazineNo";
            this.MagazineNo.Frozen = true;
            this.MagazineNo.HeaderText = "マガジン番号";
            this.MagazineNo.Name = "MagazineNo";
            this.MagazineNo.ReadOnly = true;
            // 
            // MsgVAL
            // 
            this.MsgVAL.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.MsgVAL.DataPropertyName = "Message";
            this.MsgVAL.Frozen = true;
            this.MsgVAL.HeaderText = "ログ内容";
            this.MsgVAL.Name = "MsgVAL";
            this.MsgVAL.ReadOnly = true;
            this.MsgVAL.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MsgVAL.Width = 597;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.txtLineNo);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.chkError);
            this.groupBox2.Controls.Add(this.txtMagazineNo);
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.txtLogMsg);
            this.groupBox2.Controls.Add(this.label4);
            this.groupBox2.Controls.Add(this.txtLotNO);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.dtpInTo);
            this.groupBox2.Controls.Add(this.dtpInFrom);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Location = new System.Drawing.Point(12, 29);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(852, 125);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "検索条件";
            // 
            // txtLineNo
            // 
            this.txtLineNo.Location = new System.Drawing.Point(83, 30);
            this.txtLineNo.Name = "txtLineNo";
            this.txtLineNo.Size = new System.Drawing.Size(120, 19);
            this.txtLineNo.TabIndex = 15;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(24, 33);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 12);
            this.label6.TabIndex = 14;
            this.label6.Text = "ライン番号";
            // 
            // chkError
            // 
            this.chkError.AutoSize = true;
            this.chkError.Location = new System.Drawing.Point(624, 29);
            this.chkError.Name = "chkError";
            this.chkError.Size = new System.Drawing.Size(96, 16);
            this.chkError.TabIndex = 13;
            this.chkError.Text = "エラーのみ表示";
            this.chkError.UseVisualStyleBackColor = true;
            // 
            // txtMagazineNo
            // 
            this.txtMagazineNo.Location = new System.Drawing.Point(448, 30);
            this.txtMagazineNo.Name = "txtMagazineNo";
            this.txtMagazineNo.Size = new System.Drawing.Size(120, 19);
            this.txtMagazineNo.TabIndex = 12;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(375, 33);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(67, 12);
            this.label5.TabIndex = 11;
            this.label5.Text = "マガジン番号";
            // 
            // txtLogMsg
            // 
            this.txtLogMsg.Location = new System.Drawing.Point(83, 80);
            this.txtLogMsg.Name = "txtLogMsg";
            this.txtLogMsg.Size = new System.Drawing.Size(271, 19);
            this.txtLogMsg.TabIndex = 10;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(24, 83);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(47, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "ログ内容";
            // 
            // txtLotNO
            // 
            this.txtLotNO.Location = new System.Drawing.Point(448, 55);
            this.txtLotNO.Name = "txtLotNO";
            this.txtLotNO.Size = new System.Drawing.Size(120, 19);
            this.txtLotNO.TabIndex = 8;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(210, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "～";
            // 
            // dtpInTo
            // 
            this.dtpInTo.CustomFormat = "yyyy/MM/dd";
            this.dtpInTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInTo.Location = new System.Drawing.Point(233, 55);
            this.dtpInTo.Name = "dtpInTo";
            this.dtpInTo.Size = new System.Drawing.Size(121, 19);
            this.dtpInTo.TabIndex = 6;
            this.dtpInTo.ValueChanged += new System.EventHandler(this.dtpInTo_ValueChanged);
            // 
            // dtpInFrom
            // 
            this.dtpInFrom.CustomFormat = "yyyy/MM/dd";
            this.dtpInFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInFrom.Location = new System.Drawing.Point(83, 55);
            this.dtpInFrom.Name = "dtpInFrom";
            this.dtpInFrom.ShowCheckBox = true;
            this.dtpInFrom.Size = new System.Drawing.Size(121, 19);
            this.dtpInFrom.TabIndex = 5;
            this.dtpInFrom.ValueChanged += new System.EventHandler(this.dtpInFrom_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(24, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "発生日時";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(389, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "ロット番号";
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(743, 87);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(81, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.表示ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(876, 24);
            this.menuStrip1.TabIndex = 4;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 表示ToolStripMenuItem
            // 
            this.表示ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.排出確認モードToolStripMenuItem});
            this.表示ToolStripMenuItem.Name = "表示ToolStripMenuItem";
            this.表示ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.表示ToolStripMenuItem.Text = "表示";
            // 
            // 排出確認モードToolStripMenuItem
            // 
            this.排出確認モードToolStripMenuItem.CheckOnClick = true;
            this.排出確認モードToolStripMenuItem.Name = "排出確認モードToolStripMenuItem";
            this.排出確認モードToolStripMenuItem.Size = new System.Drawing.Size(147, 22);
            this.排出確認モードToolStripMenuItem.Text = "排出確認モード";
            this.排出確認モードToolStripMenuItem.CheckedChanged += new System.EventHandler(this.排出確認モードToolStripMenuItem_CheckedChanged);
            // 
            // FrmLotLog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(876, 605);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmLotLog";
            this.Text = "FrmLotLog";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtLotNO;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dtpInTo;
        private System.Windows.Forms.DateTimePicker dtpInFrom;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLogMsg;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.BindingSource bsItems;
		private System.Windows.Forms.CheckBox chkError;
		private System.Windows.Forms.TextBox txtMagazineNo;
		private System.Windows.Forms.Label label5;
		private System.Windows.Forms.MenuStrip menuStrip1;
		private System.Windows.Forms.ToolStripMenuItem 表示ToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem 排出確認モードToolStripMenuItem;
		private System.Windows.Forms.TextBox txtLineNo;
		private System.Windows.Forms.Label label6;
		private System.Windows.Forms.DataGridViewTextBoxColumn InDT;
		private System.Windows.Forms.DataGridViewTextBoxColumn lineno;
		private System.Windows.Forms.DataGridViewTextBoxColumn ProcNo;
		private System.Windows.Forms.DataGridViewTextBoxColumn ProcNm;
		private System.Windows.Forms.DataGridViewTextBoxColumn LotNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn MagazineNo;
		private System.Windows.Forms.DataGridViewTextBoxColumn MsgVAL;
    }
}