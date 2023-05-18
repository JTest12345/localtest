namespace GEICS
{
    partial class M11_General
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M11_General));
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bnGeneral = new System.Windows.Forms.BindingNavigator(this.components);
			this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
			this.bsGeneral = new System.Windows.Forms.BindingSource(this.components);
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
			this.toolDataReturn = new System.Windows.Forms.ToolStripButton();
			this.dgvItems = new System.Windows.Forms.DataGridView();
			this.CheckFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.GeneralCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.GeneralNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnGeneral)).BeginInit();
			this.bnGeneral.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsGeneral)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
			this.SuspendLayout();
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.bnGeneral);
			this.groupBox2.Controls.Add(this.dgvItems);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(447, 297);
			this.groupBox2.TabIndex = 7;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "項目";
			// 
			// bnGeneral
			// 
			this.bnGeneral.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bnGeneral.BindingSource = this.bsGeneral;
			this.bnGeneral.CountItem = this.bindingNavigatorCountItem;
			this.bnGeneral.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bnGeneral.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bnGeneral.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.toolDataReturn});
			this.bnGeneral.Location = new System.Drawing.Point(3, 269);
			this.bnGeneral.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnGeneral.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnGeneral.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnGeneral.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnGeneral.Name = "bnGeneral";
			this.bnGeneral.PositionItem = this.bindingNavigatorPositionItem;
			this.bnGeneral.Size = new System.Drawing.Size(441, 25);
			this.bnGeneral.TabIndex = 1;
			this.bnGeneral.Text = "bindingNavigator1";
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
			// toolDataReturn
			// 
			this.toolDataReturn.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolDataReturn.Image = ((System.Drawing.Image)(resources.GetObject("toolDataReturn.Image")));
			this.toolDataReturn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolDataReturn.Name = "toolDataReturn";
			this.toolDataReturn.Size = new System.Drawing.Size(108, 22);
			this.toolDataReturn.Text = "選択データを戻す";
			this.toolDataReturn.Click += new System.EventHandler(this.toolDataReturn_Click);
			// 
			// dgvItems
			// 
			this.dgvItems.AllowUserToAddRows = false;
			this.dgvItems.AllowUserToDeleteRows = false;
			this.dgvItems.AllowUserToResizeRows = false;
			this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvItems.AutoGenerateColumns = false;
			this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CheckFG,
            this.GeneralCD,
            this.GeneralNM});
			this.dgvItems.DataSource = this.bsGeneral;
			this.dgvItems.Location = new System.Drawing.Point(6, 18);
			this.dgvItems.MultiSelect = false;
			this.dgvItems.Name = "dgvItems";
			this.dgvItems.ReadOnly = true;
			this.dgvItems.RowHeadersVisible = false;
			this.dgvItems.RowTemplate.Height = 21;
			this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvItems.Size = new System.Drawing.Size(435, 248);
			this.dgvItems.TabIndex = 0;
			this.dgvItems.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvItems_CellMouseDoubleClick);
			// 
			// CheckFG
			// 
			this.CheckFG.DataPropertyName = "IsCheck";
			this.CheckFG.HeaderText = "";
			this.CheckFG.Name = "CheckFG";
			this.CheckFG.ReadOnly = true;
			this.CheckFG.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.CheckFG.Width = 40;
			// 
			// GeneralCD
			// 
			this.GeneralCD.DataPropertyName = "GeneralCD";
			this.GeneralCD.HeaderText = "汎用CD";
			this.GeneralCD.Name = "GeneralCD";
			this.GeneralCD.ReadOnly = true;
			this.GeneralCD.Width = 80;
			// 
			// GeneralNM
			// 
			this.GeneralNM.DataPropertyName = "GeneralNM";
			this.GeneralNM.HeaderText = "汎用名";
			this.GeneralNM.Name = "GeneralNM";
			this.GeneralNM.ReadOnly = true;
			this.GeneralNM.Width = 200;
			// 
			// M11_General
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(471, 321);
			this.Controls.Add(this.groupBox2);
			this.Name = "M11_General";
			this.Text = "M11 マスタメンテナンス-汎用項目";
			this.Load += new System.EventHandler(this.M11_General_Load);
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnGeneral)).EndInit();
			this.bnGeneral.ResumeLayout(false);
			this.bnGeneral.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bsGeneral)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.BindingNavigator bnGeneral;
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
        private System.Windows.Forms.ToolStripButton toolDataReturn;
        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.BindingSource bsGeneral;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CheckFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn GeneralCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn GeneralNM;
    }
}