namespace GEICS
{
    partial class M12_FileFmtTypeWirebonder
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M12_FileFmtTypeWirebonder));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle13 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle14 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle15 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle16 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle17 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle18 = new System.Windows.Forms.DataGridViewCellStyle();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.cmbType = new SLCommonLib.UCComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
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
			this.btnSave = new System.Windows.Forms.ToolStripButton();
			this.toolAdd = new System.Windows.Forms.ToolStripButton();
			this.toolPaste = new System.Windows.Forms.ToolStripButton();
			this.dgvItems = new System.Windows.Forms.DataGridView();
			this.ChangeFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.OldTypeCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TypeCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FileFmtNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.FrameNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DelFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.UpdUserCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastUpdDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toollblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServerhed = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServer = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblEmp = new System.Windows.Forms.ToolStripStatusLabel();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
			this.bnItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.btnSearch);
			this.groupBox1.Controls.Add(this.cmbType);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(633, 100);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "検索条件";
			// 
			// btnSearch
			// 
			this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
			this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
			this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSearch.Location = new System.Drawing.Point(522, 61);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(88, 23);
			this.btnSearch.TabIndex = 3;
			this.btnSearch.Text = "検索";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// cmbType
			// 
			this.cmbType.FormattingEnabled = true;
			this.cmbType.Location = new System.Drawing.Point(80, 28);
			this.cmbType.MaxDropDownItems = 15;
			this.cmbType.Name = "cmbType";
			this.cmbType.Size = new System.Drawing.Size(139, 20);
			this.cmbType.SourceNVC = null;
			this.cmbType.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(21, 31);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 0;
			this.label1.Text = "製品型番";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.bnItems);
			this.groupBox2.Controls.Add(this.dgvItems);
			this.groupBox2.Location = new System.Drawing.Point(12, 118);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(633, 479);
			this.groupBox2.TabIndex = 1;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "一覧";
			// 
			// bnItems
			// 
			this.bnItems.AddNewItem = null;
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
            this.bindingNavigatorDeleteItem,
            this.btnSave,
            this.toolAdd,
            this.toolPaste});
			this.bnItems.Location = new System.Drawing.Point(3, 451);
			this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnItems.Name = "bnItems";
			this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
			this.bnItems.Size = new System.Drawing.Size(627, 25);
			this.bnItems.TabIndex = 3;
			this.bnItems.Text = "bindingNavigator1";
			// 
			// bindingNavigatorCountItem
			// 
			this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
			this.bindingNavigatorCountItem.Size = new System.Drawing.Size(27, 22);
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
			// btnSave
			// 
			this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
			this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(49, 22);
			this.btnSave.Text = "保存";
			this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
			// 
			// toolAdd
			// 
			this.toolAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolAdd.Image")));
			this.toolAdd.Name = "toolAdd";
			this.toolAdd.RightToLeftAutoMirrorImage = true;
			this.toolAdd.Size = new System.Drawing.Size(73, 22);
			this.toolAdd.Text = "新規追加";
			this.toolAdd.Click += new System.EventHandler(this.toolAdd_Click);
			// 
			// toolPaste
			// 
			this.toolPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolPaste.Image")));
			this.toolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolPaste.Name = "toolPaste";
			this.toolPaste.Size = new System.Drawing.Size(67, 22);
			this.toolPaste.Text = "貼り付け";
			this.toolPaste.Click += new System.EventHandler(this.toolPaste_Click);
			// 
			// dgvItems
			// 
			this.dgvItems.AllowUserToAddRows = false;
			this.dgvItems.AllowUserToResizeRows = false;
			this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.dgvItems.AutoGenerateColumns = false;
			this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChangeFG,
            this.OldTypeCD,
            this.TypeCD,
            this.FileFmtNO,
            this.FrameNO,
            this.DelFG,
            this.UpdUserCD,
            this.LastUpdDT});
			this.dgvItems.DataSource = this.bsItems;
			this.dgvItems.Location = new System.Drawing.Point(9, 18);
			this.dgvItems.Name = "dgvItems";
			this.dgvItems.RowHeadersVisible = false;
			this.dgvItems.RowTemplate.Height = 21;
			this.dgvItems.Size = new System.Drawing.Size(621, 430);
			this.dgvItems.TabIndex = 0;
			this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
			// 
			// ChangeFG
			// 
			this.ChangeFG.DataPropertyName = "ChangeFG";
			dataGridViewCellStyle13.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle13.BackColor = System.Drawing.SystemColors.Info;
			dataGridViewCellStyle13.NullValue = false;
			this.ChangeFG.DefaultCellStyle = dataGridViewCellStyle13;
			this.ChangeFG.HeaderText = "変更";
			this.ChangeFG.Name = "ChangeFG";
			this.ChangeFG.Width = 40;
			// 
			// OldTypeCD
			// 
			this.OldTypeCD.DataPropertyName = "TypeCD";
			this.OldTypeCD.HeaderText = "旧製品型番";
			this.OldTypeCD.Name = "OldTypeCD";
			this.OldTypeCD.Visible = false;
			// 
			// TypeCD
			// 
			this.TypeCD.DataPropertyName = "TypeCD";
			dataGridViewCellStyle14.BackColor = System.Drawing.SystemColors.Info;
			this.TypeCD.DefaultCellStyle = dataGridViewCellStyle14;
			this.TypeCD.HeaderText = "製品型番";
			this.TypeCD.Name = "TypeCD";
			this.TypeCD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.TypeCD.Width = 150;
			// 
			// FileFmtNO
			// 
			this.FileFmtNO.DataPropertyName = "FileFmtNO";
			dataGridViewCellStyle15.BackColor = System.Drawing.SystemColors.Info;
			this.FileFmtNO.DefaultCellStyle = dataGridViewCellStyle15;
			this.FileFmtNO.HeaderText = "紐付け番号";
			this.FileFmtNO.Name = "FileFmtNO";
			this.FileFmtNO.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.FileFmtNO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.FileFmtNO.Width = 80;
			// 
			// FrameNO
			// 
			this.FrameNO.DataPropertyName = "FrameNO";
			dataGridViewCellStyle16.BackColor = System.Drawing.SystemColors.Info;
			this.FrameNO.DefaultCellStyle = dataGridViewCellStyle16;
			this.FrameNO.HeaderText = "基板番号";
			this.FrameNO.Name = "FrameNO";
			this.FrameNO.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.FrameNO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.FrameNO.Width = 80;
			// 
			// DelFG
			// 
			this.DelFG.DataPropertyName = "DelFG";
			dataGridViewCellStyle17.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle17.BackColor = System.Drawing.SystemColors.Info;
			dataGridViewCellStyle17.NullValue = false;
			this.DelFG.DefaultCellStyle = dataGridViewCellStyle17;
			this.DelFG.HeaderText = "削除";
			this.DelFG.Name = "DelFG";
			this.DelFG.Width = 40;
			// 
			// UpdUserCD
			// 
			this.UpdUserCD.DataPropertyName = "UpdUserCD";
			dataGridViewCellStyle18.BackColor = System.Drawing.SystemColors.Info;
			this.UpdUserCD.DefaultCellStyle = dataGridViewCellStyle18;
			this.UpdUserCD.HeaderText = "更新者CD";
			this.UpdUserCD.Name = "UpdUserCD";
			this.UpdUserCD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.UpdUserCD.Width = 80;
			// 
			// LastUpdDT
			// 
			this.LastUpdDT.DataPropertyName = "LastUpdDT";
			this.LastUpdDT.HeaderText = "更新日";
			this.LastUpdDT.Name = "LastUpdDT";
			this.LastUpdDT.ReadOnly = true;
			this.LastUpdDT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toollblStatusText,
            this.toollblServerhed,
            this.toollblServer,
            this.toolStripStatusLabel1,
            this.toollblEmp});
			this.statusStrip.Location = new System.Drawing.Point(0, 605);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(657, 22);
			this.statusStrip.TabIndex = 62;
			this.statusStrip.Text = "statusStrip1";
			// 
			// toollblStatusText
			// 
			this.toollblStatusText.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Right;
			this.toollblStatusText.Name = "toollblStatusText";
			this.toollblStatusText.Size = new System.Drawing.Size(510, 17);
			this.toollblStatusText.Spring = true;
			// 
			// toollblServerhed
			// 
			this.toollblServerhed.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toollblServerhed.Name = "toollblServerhed";
			this.toollblServerhed.Size = new System.Drawing.Size(75, 17);
			this.toollblServerhed.Text = "ログインサーバ";
			// 
			// toollblServer
			// 
			this.toollblServer.Name = "toollblServer";
			this.toollblServer.Size = new System.Drawing.Size(0, 17);
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			this.toolStripStatusLabel1.RightToLeftAutoMirrorImage = true;
			this.toolStripStatusLabel1.Size = new System.Drawing.Size(57, 17);
			this.toolStripStatusLabel1.Text = "ログイン者";
			// 
			// toollblEmp
			// 
			this.toollblEmp.Name = "toollblEmp";
			this.toollblEmp.Size = new System.Drawing.Size(0, 17);
			// 
			// M12_FileFmtTypeWirebonder
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(657, 627);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "M12_FileFmtTypeWirebonder";
			this.Text = "M12 マスタメンテナンス - WBファイル内管理項目紐付け方法 製品型番別";
			this.Load += new System.EventHandler(this.M12_FileFmtType_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
			this.bnItems.ResumeLayout(false);
			this.bnItems.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private SLCommonLib.UCComboBox cmbType;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.BindingSource bsItems;
        private System.Windows.Forms.BindingNavigator bnItems;
        private System.Windows.Forms.ToolStripButton toolAdd;
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
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toollblStatusText;
        private System.Windows.Forms.ToolStripStatusLabel toollblServerhed;
        private System.Windows.Forms.ToolStripStatusLabel toollblServer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toollblEmp;
        private System.Windows.Forms.ToolStripButton toolPaste;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ChangeFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn OldTypeCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn FileFmtNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn FrameNO;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DelFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdUserCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDT;
    }
}