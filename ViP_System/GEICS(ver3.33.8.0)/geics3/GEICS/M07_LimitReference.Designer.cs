namespace GEICS
{
    partial class M07_LimitReference
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M07_LimitReference));
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
            this.bindingNavigatorAddNewItem = new System.Windows.Forms.ToolStripButton();
            this.toolPreserve = new System.Windows.Forms.ToolStripButton();
            this.toolAdd = new System.Windows.Forms.ToolStripButton();
            this.toolTypeRef = new System.Windows.Forms.ToolStripButton();
            this.toolPaste = new System.Windows.Forms.ToolStripButton();
            this.toolExcelLoad = new System.Windows.Forms.ToolStripButton();
            this.grdItems = new System.Windows.Forms.DataGridView();
            this.SelectFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ItemNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolTypeCD = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.txtReason = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.openFileDialog = new System.Windows.Forms.OpenFileDialog();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
            this.toolStrip.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.bnItems);
            this.groupBox2.Controls.Add(this.grdItems);
            this.groupBox2.Location = new System.Drawing.Point(12, 118);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(623, 198);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "登録対象";
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
            this.bindingNavigatorAddNewItem,
            this.bindingNavigatorDeleteItem,
            this.toolPreserve,
            this.toolAdd,
            this.toolTypeRef,
            this.toolPaste,
            this.toolExcelLoad});
            this.bnItems.Location = new System.Drawing.Point(3, 170);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(617, 25);
            this.bnItems.TabIndex = 2;
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
            // toolPreserve
            // 
            this.toolPreserve.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolPreserve.Image = ((System.Drawing.Image)(resources.GetObject("toolPreserve.Image")));
            this.toolPreserve.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolPreserve.Name = "toolPreserve";
            this.toolPreserve.Size = new System.Drawing.Size(51, 22);
            this.toolPreserve.Text = "保存";
            this.toolPreserve.Click += new System.EventHandler(this.toolPreserve_Click);
            // 
            // toolAdd
            // 
            this.toolAdd.Image = ((System.Drawing.Image)(resources.GetObject("toolAdd.Image")));
            this.toolAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolAdd.Name = "toolAdd";
            this.toolAdd.Size = new System.Drawing.Size(75, 22);
            this.toolAdd.Text = "新規追加";
            this.toolAdd.Click += new System.EventHandler(this.toolAdd_Click);
            // 
            // toolTypeRef
            // 
            this.toolTypeRef.Image = ((System.Drawing.Image)(resources.GetObject("toolTypeRef.Image")));
            this.toolTypeRef.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolTypeRef.Name = "toolTypeRef";
            this.toolTypeRef.Size = new System.Drawing.Size(75, 22);
            this.toolTypeRef.Text = "型番参照";
            this.toolTypeRef.Click += new System.EventHandler(this.toolTypeRef_Click);
            // 
            // toolPaste
            // 
            this.toolPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolPaste.Image")));
            this.toolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolPaste.Name = "toolPaste";
            this.toolPaste.Size = new System.Drawing.Size(65, 22);
            this.toolPaste.Text = "貼りつけ";
            this.toolPaste.Click += new System.EventHandler(this.toolPaste_Click);
            // 
            // toolExcelLoad
            // 
            this.toolExcelLoad.Image = ((System.Drawing.Image)(resources.GetObject("toolExcelLoad.Image")));
            this.toolExcelLoad.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolExcelLoad.Name = "toolExcelLoad";
            this.toolExcelLoad.Size = new System.Drawing.Size(78, 22);
            this.toolExcelLoad.Text = "Excel読込";
            this.toolExcelLoad.Click += new System.EventHandler(this.toolExcelLoad_Click);
            // 
            // grdItems
            // 
            this.grdItems.AllowUserToAddRows = false;
            this.grdItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdItems.AutoGenerateColumns = false;
            this.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.grdItems.ColumnHeadersVisible = false;
            this.grdItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SelectFG,
            this.ItemNM});
            this.grdItems.DataSource = this.bsItems;
            this.grdItems.Location = new System.Drawing.Point(6, 18);
            this.grdItems.Name = "grdItems";
            this.grdItems.RowHeadersVisible = false;
            this.grdItems.RowTemplate.Height = 21;
            this.grdItems.Size = new System.Drawing.Size(611, 149);
            this.grdItems.TabIndex = 1;
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
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolTypeCD,
            this.toolStripLabel1});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(647, 25);
            this.toolStrip.TabIndex = 4;
            this.toolStrip.Text = "toolStrip1";
            // 
            // toolTypeCD
            // 
            this.toolTypeCD.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolTypeCD.Name = "toolTypeCD";
            this.toolTypeCD.ReadOnly = true;
            this.toolTypeCD.Size = new System.Drawing.Size(100, 25);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(43, 22);
            this.toolStripLabel1.Text = "参照元";
            // 
            // txtReason
            // 
            this.txtReason.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtReason.Location = new System.Drawing.Point(6, 18);
            this.txtReason.MaxLength = 200;
            this.txtReason.Multiline = true;
            this.txtReason.Name = "txtReason";
            this.txtReason.Size = new System.Drawing.Size(611, 51);
            this.txtReason.TabIndex = 7;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtReason);
            this.groupBox1.Location = new System.Drawing.Point(12, 36);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(623, 75);
            this.groupBox1.TabIndex = 9;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "変更理由";
            // 
            // openFileDialog
            // 
            this.openFileDialog.FileName = "openFileDialog";
            // 
            // M07_LimitReference
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(647, 330);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.groupBox2);
            this.Name = "M07_LimitReference";
            this.Text = "M07 参照登録";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.M07_LimitReference_FormClosing);
            this.Load += new System.EventHandler(this.M07_LimitReference_Load);
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView grdItems;
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
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripTextBox toolTypeCD;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton toolPreserve;
        private System.Windows.Forms.TextBox txtReason;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.ToolStripButton toolAdd;
        private System.Windows.Forms.BindingSource bsItems;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SelectFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn ItemNM;
		private System.Windows.Forms.ToolStripButton toolTypeRef;
		private System.Windows.Forms.ToolStripButton toolExcelLoad;
		private System.Windows.Forms.OpenFileDialog openFileDialog;
		private System.Windows.Forms.ToolStripButton toolPaste;
    }
}