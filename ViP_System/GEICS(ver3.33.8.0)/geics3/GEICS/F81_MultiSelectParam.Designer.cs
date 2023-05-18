namespace GEICS
{
	partial class F81_MultiSelectParam
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F81_MultiSelectParam));
			this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
			this.bsItems = new System.Windows.Forms.BindingSource(this.components);
			this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
			this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
			this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolReturnData = new System.Windows.Forms.ToolStripButton();
			this.toolAllSelect = new System.Windows.Forms.ToolStripButton();
			this.toolAllCansel = new System.Windows.Forms.ToolStripButton();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.grdItems = new System.Windows.Forms.DataGridView();
			this.SelectFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.TypeGroup = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ModelNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ClassNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QcParamNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ParameterNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.cmbClassCond = new System.Windows.Forms.ComboBox();
			this.cmbParamCond = new System.Windows.Forms.ComboBox();
			this.cmbParamNMCond = new System.Windows.Forms.ComboBox();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label4 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
			this.bnItems.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
			// 
			// bnItems
			// 
			this.bnItems.AddNewItem = null;
			this.bnItems.BindingSource = this.bsItems;
			this.bnItems.CountItem = this.bindingNavigatorCountItem;
			this.bnItems.DeleteItem = null;
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
            this.toolReturnData,
            this.toolAllSelect,
            this.toolAllCansel});
			this.bnItems.Location = new System.Drawing.Point(3, 437);
			this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnItems.Name = "bnItems";
			this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
			this.bnItems.Size = new System.Drawing.Size(751, 25);
			this.bnItems.TabIndex = 1;
			this.bnItems.Text = "bindingNavigator1";
			// 
			// bindingNavigatorCountItem
			// 
			this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
			this.bindingNavigatorCountItem.Size = new System.Drawing.Size(27, 22);
			this.bindingNavigatorCountItem.Text = "/ {0}";
			this.bindingNavigatorCountItem.ToolTipText = "項目の総数";
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
			this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 21);
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
			// toolReturnData
			// 
			this.toolReturnData.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolReturnData.Image = ((System.Drawing.Image)(resources.GetObject("toolReturnData.Image")));
			this.toolReturnData.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolReturnData.Name = "toolReturnData";
			this.toolReturnData.Size = new System.Drawing.Size(84, 22);
			this.toolReturnData.Text = "データを戻す";
			this.toolReturnData.Click += new System.EventHandler(this.toolReturnData_Click);
			// 
			// toolAllSelect
			// 
			this.toolAllSelect.Image = ((System.Drawing.Image)(resources.GetObject("toolAllSelect.Image")));
			this.toolAllSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolAllSelect.Name = "toolAllSelect";
			this.toolAllSelect.Size = new System.Drawing.Size(61, 22);
			this.toolAllSelect.Text = "全選択";
			this.toolAllSelect.Click += new System.EventHandler(this.toolAllSelect_Click);
			// 
			// toolAllCansel
			// 
			this.toolAllCansel.Image = ((System.Drawing.Image)(resources.GetObject("toolAllCansel.Image")));
			this.toolAllCansel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolAllCansel.Name = "toolAllCansel";
			this.toolAllCansel.Size = new System.Drawing.Size(61, 22);
			this.toolAllCansel.Text = "全解除";
			this.toolAllCansel.Click += new System.EventHandler(this.toolAllCansel_Click);
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.bnItems);
			this.groupBox1.Controls.Add(this.grdItems);
			this.groupBox1.Location = new System.Drawing.Point(12, 103);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(757, 465);
			this.groupBox1.TabIndex = 3;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "一覧";
			// 
			// grdItems
			// 
			this.grdItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.grdItems.AutoGenerateColumns = false;
			this.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.grdItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectFG,
            this.TypeGroup,
            this.ModelNM,
            this.ClassNM,
            this.QcParamNO,
            this.ParameterNM});
			this.grdItems.DataSource = this.bsItems;
			this.grdItems.Location = new System.Drawing.Point(6, 18);
			this.grdItems.Name = "grdItems";
			this.grdItems.ReadOnly = true;
			this.grdItems.RowTemplate.Height = 21;
			this.grdItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.grdItems.Size = new System.Drawing.Size(745, 416);
			this.grdItems.TabIndex = 0;
			this.grdItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdItems_CellDoubleClick);
			this.grdItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdItems_KeyDown);
			// 
			// SelectFG
			// 
			this.SelectFG.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.SelectFG.DataPropertyName = "SelectFG";
			this.SelectFG.HeaderText = "";
			this.SelectFG.Name = "SelectFG";
			this.SelectFG.ReadOnly = true;
			this.SelectFG.Width = 21;
			// 
			// TypeGroup
			// 
			this.TypeGroup.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.TypeGroup.DataPropertyName = "TypeGroup";
			this.TypeGroup.HeaderText = "品種";
			this.TypeGroup.Name = "TypeGroup";
			this.TypeGroup.ReadOnly = true;
			this.TypeGroup.Width = 54;
			// 
			// ModelNM
			// 
			this.ModelNM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.ModelNM.DataPropertyName = "ModelNM";
			this.ModelNM.HeaderText = "装置種";
			this.ModelNM.Name = "ModelNM";
			this.ModelNM.ReadOnly = true;
			this.ModelNM.Width = 66;
			// 
			// ClassNM
			// 
			this.ClassNM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.ClassNM.DataPropertyName = "ClassNM";
			this.ClassNM.HeaderText = "分類";
			this.ClassNM.Name = "ClassNM";
			this.ClassNM.ReadOnly = true;
			this.ClassNM.Width = 54;
			// 
			// QcParamNO
			// 
			this.QcParamNO.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.QcParamNO.DataPropertyName = "QcParamNO";
			this.QcParamNO.HeaderText = "管理No";
			this.QcParamNO.Name = "QcParamNO";
			this.QcParamNO.ReadOnly = true;
			this.QcParamNO.Width = 78;
			// 
			// ParameterNM
			// 
			this.ParameterNM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
			this.ParameterNM.DataPropertyName = "ParameterNM";
			this.ParameterNM.HeaderText = "管理名";
			this.ParameterNM.Name = "ParameterNM";
			this.ParameterNM.ReadOnly = true;
			this.ParameterNM.Width = 66;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(210, 25);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "管理No";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(6, 24);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(29, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "分類";
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(417, 25);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(41, 12);
			this.label3.TabIndex = 8;
			this.label3.Text = "管理名";
			// 
			// cmbClassCond
			// 
			this.cmbClassCond.FormattingEnabled = true;
			this.cmbClassCond.Location = new System.Drawing.Point(41, 21);
			this.cmbClassCond.Name = "cmbClassCond";
			this.cmbClassCond.Size = new System.Drawing.Size(157, 20);
			this.cmbClassCond.TabIndex = 10;
			this.cmbClassCond.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbClassCond_KeyDown);
			// 
			// cmbParamCond
			// 
			this.cmbParamCond.FormattingEnabled = true;
			this.cmbParamCond.Location = new System.Drawing.Point(269, 21);
			this.cmbParamCond.Name = "cmbParamCond";
			this.cmbParamCond.Size = new System.Drawing.Size(135, 20);
			this.cmbParamCond.TabIndex = 11;
			this.cmbParamCond.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbParamCond_KeyDown);
			// 
			// cmbParamNMCond
			// 
			this.cmbParamNMCond.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.cmbParamNMCond.FormattingEnabled = true;
			this.cmbParamNMCond.Location = new System.Drawing.Point(464, 22);
			this.cmbParamNMCond.Name = "cmbParamNMCond";
			this.cmbParamNMCond.Size = new System.Drawing.Size(285, 20);
			this.cmbParamNMCond.TabIndex = 12;
			this.cmbParamNMCond.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbParamNMCond_KeyDown);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.cmbParamNMCond);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.cmbParamCond);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.cmbClassCond);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(757, 85);
			this.groupBox2.TabIndex = 13;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "絞り込み条件";
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(527, 61);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(224, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "※絞り込み条件は部分一致による絞り込み可";
			// 
			// F81_MultiSelectParam
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(781, 580);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "F81_MultiSelectParam";
			this.Text = "F81_MultiSelectParam";
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
			this.bnItems.ResumeLayout(false);
			this.bnItems.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.BindingNavigator bnItems;
		private System.Windows.Forms.BindingSource bsItems;
		private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
		private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
		private System.Windows.Forms.ToolStripButton toolReturnData;
		private System.Windows.Forms.ToolStripButton toolAllSelect;
		private System.Windows.Forms.ToolStripButton toolAllCansel;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.DataGridView grdItems;
		private System.Windows.Forms.DataGridViewCheckBoxColumn SelectFG;
		private System.Windows.Forms.DataGridViewTextBoxColumn TypeGroup;
		private System.Windows.Forms.DataGridViewTextBoxColumn ModelNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn ClassNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn QcParamNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn ParameterNM;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbClassCond;
		private System.Windows.Forms.ComboBox cmbParamCond;
		private System.Windows.Forms.ComboBox cmbParamNMCond;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.Label label4;
	}
}