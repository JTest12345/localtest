namespace GEICS
{
    partial class M16_ParameterSummary
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M16_ParameterSummary));
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
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
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnQcParamMulti = new System.Windows.Forms.Button();
            this.btnTypeMulti = new System.Windows.Forms.Button();
            this.txtParameterNM = new System.Windows.Forms.TextBox();
            this.btnQcParamSearch = new System.Windows.Forms.Button();
            this.txtQcParamNO = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbType = new SLCommonLib.UCComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.toolPaste = new System.Windows.Forms.ToolStripButton();
            this.ChangeFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.TypeCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QcParamNO = new System.Windows.Forms.DataGridViewButtonColumn();
            this.SummaryKB = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.AnyRowCT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DelFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UpdUserCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastUpdDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            this.groupBox2.SuspendLayout();
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
            this.ChangeFG,
            this.TypeCD,
            this.QcParamNO,
            this.SummaryKB,
            this.AnyRowCT,
            this.DelFG,
            this.UpdUserCD,
            this.LastUpdDT});
            this.dgvItems.DataSource = this.bsItems;
            this.dgvItems.Location = new System.Drawing.Point(6, 18);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowTemplate.Height = 21;
            this.dgvItems.Size = new System.Drawing.Size(647, 361);
            this.dgvItems.TabIndex = 0;
            this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            this.dgvItems.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bnItems);
            this.groupBox1.Controls.Add(this.dgvItems);
            this.groupBox1.Location = new System.Drawing.Point(12, 129);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(659, 410);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一覧";
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
            this.btnSave,
            this.toolPaste});
            this.bnItems.Location = new System.Drawing.Point(3, 382);
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
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.RightToLeftAutoMirrorImage = true;
            this.btnAdd.Size = new System.Drawing.Size(76, 22);
            this.btnAdd.Text = "新規追加";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
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
            this.btnSave.Size = new System.Drawing.Size(52, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.btnSearch);
            this.groupBox2.Controls.Add(this.btnQcParamMulti);
            this.groupBox2.Controls.Add(this.btnTypeMulti);
            this.groupBox2.Controls.Add(this.txtParameterNM);
            this.groupBox2.Controls.Add(this.btnQcParamSearch);
            this.groupBox2.Controls.Add(this.txtQcParamNO);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Controls.Add(this.cmbType);
            this.groupBox2.Location = new System.Drawing.Point(12, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(659, 111);
            this.groupBox2.TabIndex = 19;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "検索条件";
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "ACT_MULTISELECT_EP.png");
            this.imageList.Images.SetKeyName(1, "ACT_MULTISELECT_IN.png");
            // 
            // btnQcParamMulti
            // 
            this.btnQcParamMulti.ImageIndex = 0;
            this.btnQcParamMulti.ImageList = this.imageList;
            this.btnQcParamMulti.Location = new System.Drawing.Point(353, 59);
            this.btnQcParamMulti.Name = "btnQcParamMulti";
            this.btnQcParamMulti.Size = new System.Drawing.Size(33, 23);
            this.btnQcParamMulti.TabIndex = 18;
            this.btnQcParamMulti.UseVisualStyleBackColor = true;
            this.btnQcParamMulti.Click += new System.EventHandler(this.btnQcParamMulti_Click);
            // 
            // btnTypeMulti
            // 
            this.btnTypeMulti.ImageIndex = 0;
            this.btnTypeMulti.ImageList = this.imageList;
            this.btnTypeMulti.Location = new System.Drawing.Point(232, 29);
            this.btnTypeMulti.Name = "btnTypeMulti";
            this.btnTypeMulti.Size = new System.Drawing.Size(33, 23);
            this.btnTypeMulti.TabIndex = 17;
            this.btnTypeMulti.UseVisualStyleBackColor = true;
            this.btnTypeMulti.Click += new System.EventHandler(this.btnTypeMulti_Click);
            // 
            // txtParameterNM
            // 
            this.txtParameterNM.Location = new System.Drawing.Point(135, 61);
            this.txtParameterNM.Name = "txtParameterNM";
            this.txtParameterNM.ReadOnly = true;
            this.txtParameterNM.Size = new System.Drawing.Size(177, 19);
            this.txtParameterNM.TabIndex = 16;
            // 
            // btnQcParamSearch
            // 
            this.btnQcParamSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnQcParamSearch.Image")));
            this.btnQcParamSearch.Location = new System.Drawing.Point(318, 59);
            this.btnQcParamSearch.Name = "btnQcParamSearch";
            this.btnQcParamSearch.Size = new System.Drawing.Size(33, 23);
            this.btnQcParamSearch.TabIndex = 15;
            this.btnQcParamSearch.UseVisualStyleBackColor = true;
            this.btnQcParamSearch.Click += new System.EventHandler(this.btnQcParamSearch_Click);
            // 
            // txtQcParamNO
            // 
            this.txtQcParamNO.Location = new System.Drawing.Point(87, 61);
            this.txtQcParamNO.Name = "txtQcParamNO";
            this.txtQcParamNO.Size = new System.Drawing.Size(42, 19);
            this.txtQcParamNO.TabIndex = 14;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(28, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 13;
            this.label2.Text = "管理";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(28, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "製品型番";
            // 
            // cmbType
            // 
            this.cmbType.FormattingEnabled = true;
            this.cmbType.Location = new System.Drawing.Point(87, 31);
            this.cmbType.MaxDropDownItems = 15;
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(139, 20);
            this.cmbType.SourceNVC = null;
            this.cmbType.TabIndex = 11;
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(555, 73);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(88, 23);
            this.btnSearch.TabIndex = 19;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // toolPaste
            // 
            this.toolPaste.Image = ((System.Drawing.Image)(resources.GetObject("toolPaste.Image")));
            this.toolPaste.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolPaste.Name = "toolPaste";
            this.toolPaste.Size = new System.Drawing.Size(76, 22);
            this.toolPaste.Text = "貼り付け";
            this.toolPaste.Click += new System.EventHandler(this.toolPaste_Click);
            // 
            // ChangeFG
            // 
            this.ChangeFG.DataPropertyName = "ChangeFG";
            this.ChangeFG.HeaderText = "変更";
            this.ChangeFG.Name = "ChangeFG";
            this.ChangeFG.Width = 40;
            // 
            // TypeCD
            // 
            this.TypeCD.DataPropertyName = "TypeCD";
            this.TypeCD.HeaderText = "型番";
            this.TypeCD.Name = "TypeCD";
            this.TypeCD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TypeCD.Width = 150;
            // 
            // QcParamNO
            // 
            this.QcParamNO.DataPropertyName = "QcParamNO";
            this.QcParamNO.HeaderText = "管理No";
            this.QcParamNO.Name = "QcParamNO";
            this.QcParamNO.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.QcParamNO.Width = 70;
            // 
            // SummaryKB
            // 
            this.SummaryKB.DataPropertyName = "SummaryKB";
            this.SummaryKB.HeaderText = "集計方法";
            this.SummaryKB.Name = "SummaryKB";
            this.SummaryKB.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.SummaryKB.Width = 70;
            // 
            // AnyRowCT
            // 
            this.AnyRowCT.DataPropertyName = "AnyRowCT";
            this.AnyRowCT.HeaderText = "n数";
            this.AnyRowCT.Name = "AnyRowCT";
            this.AnyRowCT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.AnyRowCT.Width = 70;
            // 
            // DelFG
            // 
            this.DelFG.DataPropertyName = "DelFG";
            this.DelFG.HeaderText = "削除";
            this.DelFG.Name = "DelFG";
            this.DelFG.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DelFG.Width = 40;
            // 
            // UpdUserCD
            // 
            this.UpdUserCD.DataPropertyName = "UpdUserCD";
            this.UpdUserCD.HeaderText = "更新者CD";
            this.UpdUserCD.Name = "UpdUserCD";
            this.UpdUserCD.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.UpdUserCD.Width = 80;
            // 
            // LastUpdDT
            // 
            this.LastUpdDT.DataPropertyName = "LastUpdDT";
            this.LastUpdDT.HeaderText = "更新日時";
            this.LastUpdDT.Name = "LastUpdDT";
            this.LastUpdDT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // M16_ParameterSummary
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(683, 551);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "M16_ParameterSummary";
            this.Text = "マスタメンテナンス - 閾値項目集計方法";
            this.Load += new System.EventHandler(this.M16_ParameterSummary_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvItems;
        private System.Windows.Forms.GroupBox groupBox1;
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
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.BindingSource bsItems;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.Button btnQcParamMulti;
        private System.Windows.Forms.Button btnTypeMulti;
        private System.Windows.Forms.TextBox txtParameterNM;
        private System.Windows.Forms.Button btnQcParamSearch;
        private System.Windows.Forms.TextBox txtQcParamNO;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private SLCommonLib.UCComboBox cmbType;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ToolStripButton toolPaste;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ChangeFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn TypeCD;
        private System.Windows.Forms.DataGridViewButtonColumn QcParamNO;
        private System.Windows.Forms.DataGridViewTextBoxColumn SummaryKB;
        private System.Windows.Forms.DataGridViewTextBoxColumn AnyRowCT;
        private System.Windows.Forms.DataGridViewCheckBoxColumn DelFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdUserCD;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDT;
    }
}