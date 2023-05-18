namespace ArmsMaintenance
{
    partial class FrmLotTray
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLotTray));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkNewFg = new System.Windows.Forms.CheckBox();
            this.txtTrayNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.ChangeFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TrayNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TypeCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NewFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LastUpdDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OldTrayNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OldLotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
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
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkNewFg);
            this.groupBox1.Controls.Add(this.txtTrayNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(599, 146);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // chkNewFg
            // 
            this.chkNewFg.AutoSize = true;
            this.chkNewFg.Checked = true;
            this.chkNewFg.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkNewFg.Location = new System.Drawing.Point(301, 33);
            this.chkNewFg.Name = "chkNewFg";
            this.chkNewFg.Size = new System.Drawing.Size(114, 16);
            this.chkNewFg.TabIndex = 5;
            this.chkNewFg.Text = "割当て中のみ表示";
            this.chkNewFg.UseVisualStyleBackColor = true;
            // 
            // txtTrayNo
            // 
            this.txtTrayNo.Location = new System.Drawing.Point(143, 61);
            this.txtTrayNo.Name = "txtTrayNo";
            this.txtTrayNo.Size = new System.Drawing.Size(121, 19);
            this.txtTrayNo.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "トレイ番号";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ロット番号";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(143, 31);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(121, 19);
            this.txtLotNo.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(496, 107);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(83, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvItems);
            this.groupBox2.Controls.Add(this.bnItems);
            this.groupBox2.Location = new System.Drawing.Point(12, 164);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(599, 361);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "一覧";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
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
            this.ChangeFg,
            this.TrayNo,
            this.OrderNo,
            this.TypeCd,
            this.LotNo,
            this.NewFg,
            this.LastUpdDt,
            this.OldTrayNo,
            this.OldLotNo});
            this.dgvItems.DataSource = this.bsItems;
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItems.DefaultCellStyle = dataGridViewCellStyle6;
            this.dgvItems.Location = new System.Drawing.Point(6, 18);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowTemplate.Height = 21;
            this.dgvItems.Size = new System.Drawing.Size(587, 312);
            this.dgvItems.TabIndex = 1;
            this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            // 
            // ChangeFg
            // 
            this.ChangeFg.DataPropertyName = "ChangeFg";
            this.ChangeFg.HeaderText = "変更";
            this.ChangeFg.Name = "ChangeFg";
            this.ChangeFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ChangeFg.Width = 40;
            // 
            // TrayNo
            // 
            this.TrayNo.DataPropertyName = "TrayNo";
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Info;
            this.TrayNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.TrayNo.HeaderText = "トレイ番号";
            this.TrayNo.Name = "TrayNo";
            this.TrayNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TrayNo.Width = 80;
            // 
            // OrderNo
            // 
            this.OrderNo.DataPropertyName = "OrderNo";
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Info;
            this.OrderNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.OrderNo.HeaderText = "連番";
            this.OrderNo.Name = "OrderNo";
            this.OrderNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.OrderNo.Width = 75;
            // 
            // TypeCd
            // 
            this.TypeCd.DataPropertyName = "TypeCd";
            this.TypeCd.HeaderText = "タイプ";
            this.TypeCd.Name = "TypeCd";
            this.TypeCd.ReadOnly = true;
            this.TypeCd.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // LotNo
            // 
            this.LotNo.DataPropertyName = "LotNo";
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Info;
            this.LotNo.DefaultCellStyle = dataGridViewCellStyle4;
            this.LotNo.HeaderText = "ロット番号";
            this.LotNo.Name = "LotNo";
            this.LotNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LotNo.Width = 120;
            // 
            // NewFg
            // 
            this.NewFg.DataPropertyName = "NewFg";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Info;
            dataGridViewCellStyle5.NullValue = false;
            this.NewFg.DefaultCellStyle = dataGridViewCellStyle5;
            this.NewFg.HeaderText = "割当て中";
            this.NewFg.Name = "NewFg";
            this.NewFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.NewFg.Width = 60;
            // 
            // LastUpdDt
            // 
            this.LastUpdDt.DataPropertyName = "LastUpdDt";
            this.LastUpdDt.HeaderText = "更新日時";
            this.LastUpdDt.Name = "LastUpdDt";
            this.LastUpdDt.ReadOnly = true;
            this.LastUpdDt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // OldTrayNo
            // 
            this.OldTrayNo.DataPropertyName = "OldTrayNo";
            this.OldTrayNo.HeaderText = "旧トレイ番号";
            this.OldTrayNo.Name = "OldTrayNo";
            this.OldTrayNo.ReadOnly = true;
            this.OldTrayNo.Visible = false;
            // 
            // OldLotNo
            // 
            this.OldLotNo.DataPropertyName = "OldLotNo";
            this.OldLotNo.HeaderText = "旧ロット番号";
            this.OldLotNo.Name = "OldLotNo";
            this.OldLotNo.ReadOnly = true;
            this.OldLotNo.Visible = false;
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
            this.btnAdd,
            this.bindingNavigatorDeleteItem,
            this.btnSave});
            this.bnItems.Location = new System.Drawing.Point(3, 333);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(593, 25);
            this.bnItems.TabIndex = 0;
            this.bnItems.Text = "bindingNavigator1";
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
            this.bindingNavigatorMoveLastItem.Click += new System.EventHandler(this.bindingNavigatorMoveLastItem_Click);
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.RightToLeftAutoMirrorImage = true;
            this.btnAdd.Size = new System.Drawing.Size(75, 22);
            this.btnAdd.Text = "新規追加";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmLotTray
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 537);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmLotTray";
            this.Text = "スパッタ用トレイ割当て情報メンテナンス";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.BindingNavigator bnItems;
		private System.Windows.Forms.ToolStripButton btnAdd;
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
		private System.Windows.Forms.ToolStripButton btnSave;
		private System.Windows.Forms.TextBox txtTrayNo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtLotNo;
		private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.BindingSource bsItems;
        private System.Windows.Forms.CheckBox chkNewFg;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ChangeFg;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrayNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn LotNo;
        private System.Windows.Forms.DataGridViewCheckBoxColumn NewFg;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDt;
        private System.Windows.Forms.DataGridViewTextBoxColumn OldTrayNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn OldLotNo;
	}
}