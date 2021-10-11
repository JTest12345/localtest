namespace ArmsMaintenance
{
	partial class FrmSelectProcess
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectProcess));
			this.dgvItems = new System.Windows.Forms.DataGridView();
			this.SelectFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ProcNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ProcNm = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bsItems = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
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
			this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.btnSubmit = new System.Windows.Forms.ToolStripButton();
			this.btnAllCancel = new System.Windows.Forms.ToolStripButton();
			this.btnAllSelect = new System.Windows.Forms.ToolStripButton();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
			this.bnItems.SuspendLayout();
			this.SuspendLayout();
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
            this.SelectFg,
            this.ProcNo,
            this.ProcNm});
			this.dgvItems.DataSource = this.bsItems;
			this.dgvItems.Location = new System.Drawing.Point(6, 18);
			this.dgvItems.Name = "dgvItems";
			this.dgvItems.RowHeadersVisible = false;
			this.dgvItems.RowTemplate.Height = 21;
			this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvItems.Size = new System.Drawing.Size(361, 472);
			this.dgvItems.TabIndex = 0;
			this.dgvItems.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvItems_CellMouseDoubleClick);
			// 
			// SelectFg
			// 
			this.SelectFg.HeaderText = "選択";
			this.SelectFg.Name = "SelectFg";
			this.SelectFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.SelectFg.Width = 40;
			// 
			// ProcNo
			// 
			this.ProcNo.DataPropertyName = "ProcNo";
			this.ProcNo.HeaderText = "工程No";
			this.ProcNo.Name = "ProcNo";
			this.ProcNo.Visible = false;
			// 
			// ProcNm
			// 
			this.ProcNm.DataPropertyName = "InlineProNM";
			this.ProcNm.HeaderText = "工程名";
			this.ProcNm.Name = "ProcNm";
			this.ProcNm.ReadOnly = true;
			this.ProcNm.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.ProcNm.Width = 300;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.bnItems);
			this.groupBox1.Controls.Add(this.dgvItems);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(373, 521);
			this.groupBox1.TabIndex = 1;
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
            this.bindingNavigatorDeleteItem,
            this.btnSubmit,
            this.btnAllCancel,
            this.btnAllSelect});
			this.bnItems.Location = new System.Drawing.Point(3, 493);
			this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnItems.Name = "bnItems";
			this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
			this.bnItems.Size = new System.Drawing.Size(367, 25);
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
			this.bindingNavigatorCountItem.Size = new System.Drawing.Size(38, 22);
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
			// btnSubmit
			// 
			this.btnSubmit.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.btnSubmit.Image = ((System.Drawing.Image)(resources.GetObject("btnSubmit.Image")));
			this.btnSubmit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnSubmit.Name = "btnSubmit";
			this.btnSubmit.Size = new System.Drawing.Size(52, 22);
			this.btnSubmit.Text = "決定";
			this.btnSubmit.Click += new System.EventHandler(this.btnSubmit_Click);
			// 
			// btnAllCancel
			// 
			this.btnAllCancel.Image = ((System.Drawing.Image)(resources.GetObject("btnAllCancel.Image")));
			this.btnAllCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnAllCancel.Name = "btnAllCancel";
			this.btnAllCancel.Size = new System.Drawing.Size(64, 22);
			this.btnAllCancel.Text = "全解除";
			this.btnAllCancel.Click += new System.EventHandler(this.btnAllCancel_Click);
			// 
			// btnAllSelect
			// 
			this.btnAllSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnAllSelect.Image")));
			this.btnAllSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.btnAllSelect.Name = "btnAllSelect";
			this.btnAllSelect.Size = new System.Drawing.Size(64, 22);
			this.btnAllSelect.Text = "全選択";
			this.btnAllSelect.Click += new System.EventHandler(this.btnAllSelect_Click);
			// 
			// FrmSelectProcess
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(397, 545);
			this.Controls.Add(this.groupBox1);
			this.Name = "FrmSelectProcess";
			this.Text = "工程選択";
			this.Load += new System.EventHandler(this.FrmSelectProcess_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
			this.bnItems.ResumeLayout(false);
			this.bnItems.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView dgvItems;
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
		private System.Windows.Forms.ToolStripButton btnSubmit;
		private System.Windows.Forms.BindingSource bsItems;
		private System.Windows.Forms.ToolStripButton btnAllCancel;
		private System.Windows.Forms.ToolStripButton btnAllSelect;
		private System.Windows.Forms.DataGridViewCheckBoxColumn SelectFg;
		private System.Windows.Forms.DataGridViewTextBoxColumn ProcNo;
		private System.Windows.Forms.DataGridViewTextBoxColumn ProcNm;
	}
}