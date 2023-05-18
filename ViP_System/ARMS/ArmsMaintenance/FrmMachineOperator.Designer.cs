namespace ArmsMaintenance
{
    partial class FrmMachineOperator
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMachineOperator));
            this.grdItems = new System.Windows.Forms.DataGridView();
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
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
            this.chkMonitorOnly = new System.Windows.Forms.CheckBox();
            this.OperationMacgroupCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OperationEmpNm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OperationEmpCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WarningMinuteTm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MonitorFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LastUpdDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.txtQRCode = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdItems
            // 
            this.grdItems.AllowUserToAddRows = false;
            this.grdItems.AllowUserToResizeRows = false;
            this.grdItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdItems.AutoGenerateColumns = false;
            this.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OperationMacgroupCd,
            this.OperationEmpNm,
            this.OperationEmpCd,
            this.WarningMinuteTm,
            this.MonitorFG,
            this.LastUpdDt});
            this.grdItems.DataSource = this.bsItems;
            this.grdItems.Location = new System.Drawing.Point(6, 38);
            this.grdItems.Name = "grdItems";
            this.grdItems.ReadOnly = true;
            this.grdItems.RowHeadersVisible = false;
            this.grdItems.RowTemplate.Height = 21;
            this.grdItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdItems.Size = new System.Drawing.Size(658, 391);
            this.grdItems.TabIndex = 0;
            this.grdItems.CellMouseDoubleClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdItems_CellMouseDoubleClick);
            this.grdItems.KeyDown += new System.Windows.Forms.KeyEventHandler(this.grdItems_KeyDown);
            this.grdItems.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.grdItems_KeyPress);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.bnItems);
            this.groupBox1.Controls.Add(this.grdItems);
            this.groupBox1.Location = new System.Drawing.Point(12, 34);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(670, 460);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一覧";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(338, 23);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(306, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "グループ装置の編集・時間設定変更はグループ行をダブルクリック";
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
            this.bnItems.Location = new System.Drawing.Point(3, 432);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(664, 25);
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
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 19);
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
            // chkMonitorOnly
            // 
            this.chkMonitorOnly.AutoSize = true;
            this.chkMonitorOnly.Checked = true;
            this.chkMonitorOnly.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkMonitorOnly.Location = new System.Drawing.Point(35, 12);
            this.chkMonitorOnly.Name = "chkMonitorOnly";
            this.chkMonitorOnly.Size = new System.Drawing.Size(105, 16);
            this.chkMonitorOnly.TabIndex = 2;
            this.chkMonitorOnly.Text = "監視中のみ表示";
            this.chkMonitorOnly.UseVisualStyleBackColor = true;
            this.chkMonitorOnly.CheckedChanged += new System.EventHandler(this.chkMonitorOnly_CheckedChanged);
            // 
            // OperationMacgroupCd
            // 
            this.OperationMacgroupCd.DataPropertyName = "OperationMacgroupCd";
            this.OperationMacgroupCd.HeaderText = "グループ";
            this.OperationMacgroupCd.Name = "OperationMacgroupCd";
            this.OperationMacgroupCd.ReadOnly = true;
            // 
            // OperationEmpNm
            // 
            this.OperationEmpNm.DataPropertyName = "OperationEmpNm";
            this.OperationEmpNm.HeaderText = "作業者名";
            this.OperationEmpNm.Name = "OperationEmpNm";
            this.OperationEmpNm.ReadOnly = true;
            // 
            // OperationEmpCd
            // 
            this.OperationEmpCd.DataPropertyName = "OperationEmpCd";
            this.OperationEmpCd.HeaderText = "社員CD";
            this.OperationEmpCd.Name = "OperationEmpCd";
            this.OperationEmpCd.ReadOnly = true;
            this.OperationEmpCd.Width = 80;
            // 
            // WarningMinuteTm
            // 
            this.WarningMinuteTm.DataPropertyName = "WarningMinuteTm";
            this.WarningMinuteTm.HeaderText = "時間設定(分)";
            this.WarningMinuteTm.Name = "WarningMinuteTm";
            this.WarningMinuteTm.ReadOnly = true;
            // 
            // MonitorFG
            // 
            this.MonitorFG.DataPropertyName = "MonitorFg";
            this.MonitorFG.HeaderText = "監視中";
            this.MonitorFG.Name = "MonitorFG";
            this.MonitorFG.ReadOnly = true;
            this.MonitorFG.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.MonitorFG.Width = 50;
            // 
            // LastUpdDt
            // 
            this.LastUpdDt.DataPropertyName = "LastUpdDt";
            this.LastUpdDt.HeaderText = "更新日時";
            this.LastUpdDt.Name = "LastUpdDt";
            this.LastUpdDt.ReadOnly = true;
            // 
            // txtQRCode
            // 
            this.txtQRCode.Location = new System.Drawing.Point(543, 12);
            this.txtQRCode.Name = "txtQRCode";
            this.txtQRCode.Size = new System.Drawing.Size(139, 19);
            this.txtQRCode.TabIndex = 9;
            this.txtQRCode.Visible = false;
            // 
            // FrmMachineOperator
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(694, 506);
            this.Controls.Add(this.txtQRCode);
            this.Controls.Add(this.chkMonitorOnly);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmMachineOperator";
            this.Text = "装置作業者設定画面";
            this.Load += new System.EventHandler(this.FrmMachineOperator_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grdItems;
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
        private System.Windows.Forms.CheckBox chkMonitorOnly;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.BindingSource bsItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperationMacgroupCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperationEmpNm;
        private System.Windows.Forms.DataGridViewTextBoxColumn OperationEmpCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn WarningMinuteTm;
        private System.Windows.Forms.DataGridViewCheckBoxColumn MonitorFG;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDt;
        private System.Windows.Forms.TextBox txtQRCode;
    }
}