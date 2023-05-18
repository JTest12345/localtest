namespace GEICS
{
	partial class F80_MultiSelectPlant
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F80_MultiSelectPlant));
			this.grdItems = new System.Windows.Forms.DataGridView();
			this.SelectFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ServerNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AssetsNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MachineSeqNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EquipmentNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bsItems = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
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
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.cmbAssetsNMCond = new System.Windows.Forms.ComboBox();
			this.cmbPlantCDCond = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
			this.bnItems.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.SuspendLayout();
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
            this.ServerNM,
            this.AssetsNM,
            this.MachineSeqNO,
            this.EquipmentNO});
			this.grdItems.DataSource = this.bsItems;
			this.grdItems.Location = new System.Drawing.Point(6, 18);
			this.grdItems.Name = "grdItems";
			this.grdItems.ReadOnly = true;
			this.grdItems.RowTemplate.Height = 21;
			this.grdItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.grdItems.Size = new System.Drawing.Size(647, 418);
			this.grdItems.TabIndex = 0;
			this.grdItems.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdItems_CellDoubleClick);
			this.grdItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdItems_KeyDown);
			// 
			// SelectFG
			// 
			this.SelectFG.DataPropertyName = "SelectFG";
			this.SelectFG.HeaderText = "";
			this.SelectFG.Name = "SelectFG";
			this.SelectFG.ReadOnly = true;
			// 
			// ServerNM
			// 
			this.ServerNM.DataPropertyName = "ServerNM";
			this.ServerNM.HeaderText = "サーバ";
			this.ServerNM.Name = "ServerNM";
			this.ServerNM.ReadOnly = true;
			// 
			// AssetsNM
			// 
			this.AssetsNM.DataPropertyName = "AssetsNM";
			this.AssetsNM.HeaderText = "装置種";
			this.AssetsNM.Name = "AssetsNM";
			this.AssetsNM.ReadOnly = true;
			// 
			// MachineSeqNO
			// 
			this.MachineSeqNO.DataPropertyName = "MachineSeqNO";
			this.MachineSeqNO.HeaderText = "号機";
			this.MachineSeqNO.Name = "MachineSeqNO";
			this.MachineSeqNO.ReadOnly = true;
			// 
			// EquipmentNO
			// 
			this.EquipmentNO.DataPropertyName = "EquipmentNO";
			this.EquipmentNO.HeaderText = "設備CD";
			this.EquipmentNO.Name = "EquipmentNO";
			this.EquipmentNO.ReadOnly = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.bnItems);
			this.groupBox1.Controls.Add(this.grdItems);
			this.groupBox1.Location = new System.Drawing.Point(12, 97);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(659, 467);
			this.groupBox1.TabIndex = 2;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "一覧";
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
			this.bnItems.Location = new System.Drawing.Point(3, 439);
			this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnItems.Name = "bnItems";
			this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
			this.bnItems.Size = new System.Drawing.Size(653, 25);
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
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(242, 21);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(69, 12);
			this.label1.TabIndex = 4;
			this.label1.Text = "設備CD検索";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(9, 21);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(41, 12);
			this.label2.TabIndex = 6;
			this.label2.Text = "装置種";
			// 
			// cmbAssetsNMCond
			// 
			this.cmbAssetsNMCond.FormattingEnabled = true;
			this.cmbAssetsNMCond.Location = new System.Drawing.Point(56, 18);
			this.cmbAssetsNMCond.Name = "cmbAssetsNMCond";
			this.cmbAssetsNMCond.Size = new System.Drawing.Size(180, 20);
			this.cmbAssetsNMCond.TabIndex = 7;
			this.cmbAssetsNMCond.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbAssetsNMCond_KeyDown);
			// 
			// cmbPlantCDCond
			// 
			this.cmbPlantCDCond.FormattingEnabled = true;
			this.cmbPlantCDCond.Location = new System.Drawing.Point(317, 18);
			this.cmbPlantCDCond.Name = "cmbPlantCDCond";
			this.cmbPlantCDCond.Size = new System.Drawing.Size(106, 20);
			this.cmbPlantCDCond.TabIndex = 8;
			this.cmbPlantCDCond.KeyDown += new System.Windows.Forms.KeyEventHandler(this.cmbPlantCDCond_KeyDown);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(429, 55);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(224, 12);
			this.label3.TabIndex = 9;
			this.label3.Text = "※絞り込み条件は部分一致による絞り込み可";
			// 
			// groupBox2
			// 
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.cmbAssetsNMCond);
			this.groupBox2.Controls.Add(this.cmbPlantCDCond);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(659, 79);
			this.groupBox2.TabIndex = 10;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "絞り込み条件";
			// 
			// F80_MultiSelectPlant
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(683, 576);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "F80_MultiSelectPlant";
			this.Text = "F80_MultiSelectPlant";
			((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
			this.bnItems.ResumeLayout(false);
			this.bnItems.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView grdItems;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.BindingNavigator bnItems;
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
		private System.Windows.Forms.BindingSource bsItems;
		private System.Windows.Forms.DataGridViewCheckBoxColumn SelectFG;
		private System.Windows.Forms.DataGridViewTextBoxColumn ServerNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn AssetsNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn MachineSeqNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn EquipmentNO;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox cmbAssetsNMCond;
		private System.Windows.Forms.ComboBox cmbPlantCDCond;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.GroupBox groupBox2;
	}
}