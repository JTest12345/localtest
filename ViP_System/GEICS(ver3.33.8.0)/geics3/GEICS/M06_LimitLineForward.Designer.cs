namespace GEICS
{
    partial class M06_LimitLineForward
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M06_LimitLineForward));
			this.grdLine = new System.Windows.Forms.DataGridView();
			this.LineSelectFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ServerCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ServerNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DatabaseNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bsLine = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bnLine = new System.Windows.Forms.BindingNavigator(this.components);
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
			this.toolForward = new System.Windows.Forms.ToolStripButton();
			this.toolAllSelect = new System.Windows.Forms.ToolStripButton();
			this.toolAllCancel = new System.Windows.Forms.ToolStripButton();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.grdTargetType = new System.Windows.Forms.DataGridView();
			this.SelectFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.ItemNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bsTargetType = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.txtReason = new System.Windows.Forms.TextBox();
			((System.ComponentModel.ISupportInitialize)(this.grdLine)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsLine)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnLine)).BeginInit();
			this.bnLine.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdTargetType)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsTargetType)).BeginInit();
			this.groupBox3.SuspendLayout();
			this.SuspendLayout();
			// 
			// grdLine
			// 
			this.grdLine.AllowUserToAddRows = false;
			this.grdLine.AllowUserToResizeRows = false;
			this.grdLine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdLine.AutoGenerateColumns = false;
			this.grdLine.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.grdLine.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LineSelectFG,
            this.ServerCD,
            this.ServerNM,
            this.DatabaseNM});
			this.grdLine.DataSource = this.bsLine;
			this.grdLine.Location = new System.Drawing.Point(6, 18);
			this.grdLine.Name = "grdLine";
			this.grdLine.RowHeadersVisible = false;
			this.grdLine.RowTemplate.Height = 21;
			this.grdLine.Size = new System.Drawing.Size(344, 337);
			this.grdLine.TabIndex = 0;
			this.grdLine.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdLine_CellDoubleClick);
			// 
			// LineSelectFG
			// 
			this.LineSelectFG.DataPropertyName = "SelectFG";
			this.LineSelectFG.HeaderText = "";
			this.LineSelectFG.Name = "LineSelectFG";
			this.LineSelectFG.Width = 40;
			// 
			// ServerCD
			// 
			this.ServerCD.DataPropertyName = "ServerCD";
			this.ServerCD.HeaderText = "";
			this.ServerCD.Name = "ServerCD";
			this.ServerCD.ReadOnly = true;
			this.ServerCD.Visible = false;
			// 
			// ServerNM
			// 
			this.ServerNM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ServerNM.DataPropertyName = "ServerNM";
			this.ServerNM.HeaderText = "";
			this.ServerNM.Name = "ServerNM";
			this.ServerNM.ReadOnly = true;
			// 
			// DatabaseNM
			// 
			this.DatabaseNM.DataPropertyName = "DatabaseNM";
			this.DatabaseNM.HeaderText = "";
			this.DatabaseNM.Name = "DatabaseNM";
			this.DatabaseNM.Visible = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.bnLine);
			this.groupBox1.Controls.Add(this.grdLine);
			this.groupBox1.Location = new System.Drawing.Point(326, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(356, 386);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "ライン";
			// 
			// bnLine
			// 
			this.bnLine.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bnLine.BindingSource = this.bsLine;
			this.bnLine.CountItem = this.bindingNavigatorCountItem;
			this.bnLine.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bnLine.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bnLine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.toolForward,
            this.toolAllSelect,
            this.toolAllCancel});
			this.bnLine.Location = new System.Drawing.Point(3, 358);
			this.bnLine.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnLine.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnLine.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnLine.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnLine.Name = "bnLine";
			this.bnLine.PositionItem = this.bindingNavigatorPositionItem;
			this.bnLine.Size = new System.Drawing.Size(350, 25);
			this.bnLine.TabIndex = 1;
			this.bnLine.Text = "bindingNavigator1";
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
			// toolForward
			// 
			this.toolForward.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolForward.Image = ((System.Drawing.Image)(resources.GetObject("toolForward.Image")));
			this.toolForward.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolForward.Name = "toolForward";
			this.toolForward.Size = new System.Drawing.Size(49, 22);
			this.toolForward.Text = "転送";
			this.toolForward.Click += new System.EventHandler(this.toolForward_Click);
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
			// toolAllCancel
			// 
			this.toolAllCancel.Image = ((System.Drawing.Image)(resources.GetObject("toolAllCancel.Image")));
			this.toolAllCancel.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolAllCancel.Name = "toolAllCancel";
			this.toolAllCancel.Size = new System.Drawing.Size(61, 22);
			this.toolAllCancel.Text = "全解除";
			this.toolAllCancel.Click += new System.EventHandler(this.toolAllCancel_Click);
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.grdTargetType);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(308, 305);
			this.groupBox2.TabIndex = 2;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "転送対象";
			// 
			// grdTargetType
			// 
			this.grdTargetType.AllowUserToAddRows = false;
			this.grdTargetType.AllowUserToResizeRows = false;
			this.grdTargetType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.grdTargetType.AutoGenerateColumns = false;
			this.grdTargetType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
			this.grdTargetType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectFG,
            this.ItemNM});
			this.grdTargetType.DataSource = this.bsTargetType;
			this.grdTargetType.Location = new System.Drawing.Point(6, 18);
			this.grdTargetType.Name = "grdTargetType";
			this.grdTargetType.RowHeadersVisible = false;
			this.grdTargetType.RowTemplate.Height = 21;
			this.grdTargetType.Size = new System.Drawing.Size(296, 281);
			this.grdTargetType.TabIndex = 0;
			this.grdTargetType.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdTargetType_CellDoubleClick);
			// 
			// SelectFG
			// 
			this.SelectFG.DataPropertyName = "SelectFG";
			this.SelectFG.HeaderText = "";
			this.SelectFG.Name = "SelectFG";
			this.SelectFG.Width = 40;
			// 
			// ItemNM
			// 
			this.ItemNM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
			this.ItemNM.DataPropertyName = "ItemNM";
			this.ItemNM.HeaderText = "";
			this.ItemNM.Name = "ItemNM";
			this.ItemNM.ReadOnly = true;
			// 
			// groupBox3
			// 
			this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox3.Controls.Add(this.txtReason);
			this.groupBox3.Location = new System.Drawing.Point(12, 323);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.Size = new System.Drawing.Size(308, 75);
			this.groupBox3.TabIndex = 10;
			this.groupBox3.TabStop = false;
			this.groupBox3.Text = "変更理由";
			// 
			// txtReason
			// 
			this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtReason.Location = new System.Drawing.Point(6, 18);
			this.txtReason.MaxLength = 200;
			this.txtReason.Multiline = true;
			this.txtReason.Name = "txtReason";
			this.txtReason.Size = new System.Drawing.Size(296, 51);
			this.txtReason.TabIndex = 7;
			// 
			// M06_LimitLineForward
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(694, 410);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "M06_LimitLineForward";
			this.Text = "M06 他ライン転送";
			this.Load += new System.EventHandler(this.M06_LimitLineForward_Load);
			((System.ComponentModel.ISupportInitialize)(this.grdLine)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsLine)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnLine)).EndInit();
			this.bnLine.ResumeLayout(false);
			this.bnLine.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grdTargetType)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsTargetType)).EndInit();
			this.groupBox3.ResumeLayout(false);
			this.groupBox3.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdLine;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.BindingNavigator bnLine;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ToolStripButton toolForward;
        private System.Windows.Forms.BindingSource bsTargetType;
        private System.Windows.Forms.DataGridView grdTargetType;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNM;
        private System.Windows.Forms.BindingSource bsLine;
        private System.Windows.Forms.ToolStripButton toolAllSelect;
        private System.Windows.Forms.ToolStripButton toolAllCancel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.DataGridViewCheckBoxColumn LineSelectFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServerCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn ServerNM;
        private System.Windows.Forms.DataGridViewTextBoxColumn DatabaseNM;
    }
}