﻿namespace GEICS
{
    partial class F06_DefectRecord
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F06_DefectRecord));
			this.dgvDefect = new System.Windows.Forms.DataGridView();
			this.ChangeFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.LineCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.PlantCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LotNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefAddressNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.UpdateDefAddressNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefUnitNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TargetDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.WorkCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefItemCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefItemNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefCauseCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefCauseNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefClassCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DefClassNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TransactionCD = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.UpdateTransactionCD = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.UpdUserCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.DelFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.LastUpdDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.AddressCompareFG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.bsDefect = new System.Windows.Forms.BindingSource(this.components);
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.label7 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.bnDefect = new System.Windows.Forms.BindingNavigator(this.components);
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
			this.toolPreserve = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.toolLotSearch = new System.Windows.Forms.ToolStripButton();
			this.cmbServer = new SLCommonLib.UCComboBox();
			this.txtPlantCD = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.txtLotNO = new System.Windows.Forms.TextBox();
			this.label3 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.label8 = new System.Windows.Forms.Label();
			this.chkIsTargetDelRecord = new System.Windows.Forms.CheckBox();
			this.txtLineCD = new System.Windows.Forms.TextBox();
			this.label5 = new System.Windows.Forms.Label();
			this.dtsTargetDT = new SLCommonLib.UCDateTimeSelector();
			this.label4 = new System.Windows.Forms.Label();
			this.txtDefItemNM = new System.Windows.Forms.TextBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toolStatus = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsDefect)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnDefect)).BeginInit();
			this.bnDefect.SuspendLayout();
			this.groupBox2.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvDefect
			// 
			this.dgvDefect.AllowUserToAddRows = false;
			this.dgvDefect.AllowUserToResizeRows = false;
			this.dgvDefect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.dgvDefect.AutoGenerateColumns = false;
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvDefect.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
			this.dgvDefect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvDefect.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChangeFG,
            this.LineCD,
            this.PlantCD,
            this.LotNO,
            this.DefAddressNO,
            this.UpdateDefAddressNO,
            this.DefUnitNO,
            this.TargetDT,
            this.WorkCD,
            this.DefItemCD,
            this.DefItemNM,
            this.DefCauseCD,
            this.DefCauseNM,
            this.DefClassCD,
            this.DefClassNM,
            this.TransactionCD,
            this.UpdateTransactionCD,
            this.UpdUserCD,
            this.DelFG,
            this.LastUpdDT,
            this.AddressCompareFG});
			this.dgvDefect.DataSource = this.bsDefect;
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Window;
			dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.ControlText;
			dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
			this.dgvDefect.DefaultCellStyle = dataGridViewCellStyle3;
			this.dgvDefect.Location = new System.Drawing.Point(6, 43);
			this.dgvDefect.Name = "dgvDefect";
			dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
			dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Control;
			dataGridViewCellStyle4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.WindowText;
			dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
			dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
			dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
			this.dgvDefect.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
			this.dgvDefect.RowHeadersVisible = false;
			this.dgvDefect.RowTemplate.Height = 21;
			this.dgvDefect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvDefect.Size = new System.Drawing.Size(978, 446);
			this.dgvDefect.TabIndex = 0;
			this.dgvDefect.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvDefect_CellValueChanged);
			// 
			// ChangeFG
			// 
			this.ChangeFG.HeaderText = "変更";
			this.ChangeFG.Name = "ChangeFG";
			this.ChangeFG.Width = 40;
			// 
			// LineCD
			// 
			this.LineCD.DataPropertyName = "LineCD";
			this.LineCD.HeaderText = "ライン";
			this.LineCD.Name = "LineCD";
			this.LineCD.ReadOnly = true;
			this.LineCD.Width = 50;
			// 
			// PlantCD
			// 
			this.PlantCD.DataPropertyName = "PlantCD";
			this.PlantCD.HeaderText = "設備番号";
			this.PlantCD.Name = "PlantCD";
			this.PlantCD.ReadOnly = true;
			this.PlantCD.Width = 75;
			// 
			// LotNO
			// 
			this.LotNO.DataPropertyName = "LotNO";
			this.LotNO.HeaderText = "ロット番号";
			this.LotNO.Name = "LotNO";
			this.LotNO.ReadOnly = true;
			// 
			// DefAddressNO
			// 
			this.DefAddressNO.DataPropertyName = "DefAddressNO";
			this.DefAddressNO.HeaderText = "アドレス";
			this.DefAddressNO.Name = "DefAddressNO";
			this.DefAddressNO.ReadOnly = true;
			this.DefAddressNO.Width = 60;
			// 
			// UpdateDefAddressNO
			// 
			this.UpdateDefAddressNO.DataPropertyName = "UpdateDefAddressNO";
			this.UpdateDefAddressNO.HeaderText = "アドレス(変更)";
			this.UpdateDefAddressNO.Name = "UpdateDefAddressNO";
			this.UpdateDefAddressNO.Width = 68;
			// 
			// DefUnitNO
			// 
			this.DefUnitNO.DataPropertyName = "DefUnitNO";
			this.DefUnitNO.HeaderText = "ユニット";
			this.DefUnitNO.Name = "DefUnitNO";
			this.DefUnitNO.ReadOnly = true;
			this.DefUnitNO.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.DefUnitNO.Width = 50;
			// 
			// TargetDT
			// 
			this.TargetDT.DataPropertyName = "TargetDT";
			this.TargetDT.HeaderText = "発生日時";
			this.TargetDT.Name = "TargetDT";
			this.TargetDT.ReadOnly = true;
			// 
			// WorkCD
			// 
			this.WorkCD.DataPropertyName = "WorkCD";
			this.WorkCD.HeaderText = "作業CD";
			this.WorkCD.Name = "WorkCD";
			this.WorkCD.ReadOnly = true;
			this.WorkCD.Width = 70;
			// 
			// DefItemCD
			// 
			this.DefItemCD.DataPropertyName = "DefItemCD";
			this.DefItemCD.HeaderText = "不良CD";
			this.DefItemCD.Name = "DefItemCD";
			this.DefItemCD.ReadOnly = true;
			this.DefItemCD.Visible = false;
			// 
			// DefItemNM
			// 
			this.DefItemNM.DataPropertyName = "DefItemNM";
			this.DefItemNM.HeaderText = "不良名";
			this.DefItemNM.Name = "DefItemNM";
			this.DefItemNM.ReadOnly = true;
			this.DefItemNM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// DefCauseCD
			// 
			this.DefCauseCD.DataPropertyName = "DefCauseCD";
			this.DefCauseCD.HeaderText = "不良起因CD";
			this.DefCauseCD.Name = "DefCauseCD";
			this.DefCauseCD.ReadOnly = true;
			this.DefCauseCD.Visible = false;
			// 
			// DefCauseNM
			// 
			this.DefCauseNM.DataPropertyName = "DefCauseNM";
			this.DefCauseNM.HeaderText = "不良起因";
			this.DefCauseNM.Name = "DefCauseNM";
			this.DefCauseNM.ReadOnly = true;
			this.DefCauseNM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// DefClassCD
			// 
			this.DefClassCD.DataPropertyName = "DefClassCD";
			this.DefClassCD.HeaderText = "不良分類CD";
			this.DefClassCD.Name = "DefClassCD";
			this.DefClassCD.ReadOnly = true;
			this.DefClassCD.Visible = false;
			// 
			// DefClassNM
			// 
			this.DefClassNM.DataPropertyName = "DefClassNM";
			this.DefClassNM.HeaderText = "不良分類";
			this.DefClassNM.Name = "DefClassNM";
			this.DefClassNM.ReadOnly = true;
			this.DefClassNM.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			this.DefClassNM.Width = 75;
			// 
			// TransactionCD
			// 
			this.TransactionCD.DataPropertyName = "TranCD";
			this.TransactionCD.HeaderText = "処理CD";
			this.TransactionCD.Name = "TransactionCD";
			this.TransactionCD.ReadOnly = true;
			this.TransactionCD.Width = 85;
			// 
			// UpdateTransactionCD
			// 
			this.UpdateTransactionCD.DataPropertyName = "UpdateTranCD";
			dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Info;
			this.UpdateTransactionCD.DefaultCellStyle = dataGridViewCellStyle2;
			this.UpdateTransactionCD.HeaderText = "処理CD(変更)";
			this.UpdateTransactionCD.Name = "UpdateTransactionCD";
			this.UpdateTransactionCD.Visible = false;
			this.UpdateTransactionCD.Width = 85;
			// 
			// UpdUserCD
			// 
			this.UpdUserCD.DataPropertyName = "UpdUserCD";
			this.UpdUserCD.HeaderText = "更新者";
			this.UpdUserCD.Name = "UpdUserCD";
			this.UpdUserCD.ReadOnly = true;
			// 
			// DelFG
			// 
			this.DelFG.DataPropertyName = "DelFG";
			this.DelFG.HeaderText = "削除";
			this.DelFG.Name = "DelFG";
			this.DelFG.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.DelFG.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// LastUpdDT
			// 
			this.LastUpdDT.DataPropertyName = "LastUpdDT";
			this.LastUpdDT.HeaderText = "更新日時";
			this.LastUpdDT.Name = "LastUpdDT";
			this.LastUpdDT.ReadOnly = true;
			this.LastUpdDT.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// AddressCompareFG
			// 
			this.AddressCompareFG.DataPropertyName = "AddressCompareFG";
			this.AddressCompareFG.HeaderText = "アドレス照合";
			this.AddressCompareFG.Name = "AddressCompareFG";
			this.AddressCompareFG.Visible = false;
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.label7);
			this.groupBox1.Controls.Add(this.label6);
			this.groupBox1.Controls.Add(this.bnDefect);
			this.groupBox1.Controls.Add(this.dgvDefect);
			this.groupBox1.Controls.Add(this.cmbServer);
			this.groupBox1.Location = new System.Drawing.Point(12, 156);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(990, 520);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "不良一覧";
			// 
			// label7
			// 
			this.label7.AutoSize = true;
			this.label7.ForeColor = System.Drawing.Color.Red;
			this.label7.Location = new System.Drawing.Point(6, 28);
			this.label7.Name = "label7";
			this.label7.Size = new System.Drawing.Size(272, 12);
			this.label7.TabIndex = 4;
			this.label7.Text = "※背景色赤はアドレスが間違っている可能性があります。";
			// 
			// label6
			// 
			this.label6.AutoSize = true;
			this.label6.Location = new System.Drawing.Point(762, 20);
			this.label6.Name = "label6";
			this.label6.Size = new System.Drawing.Size(95, 12);
			this.label6.TabIndex = 3;
			this.label6.Text = "保存先中間サーバ";
			// 
			// bnDefect
			// 
			this.bnDefect.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bnDefect.AllowItemReorder = true;
			this.bnDefect.BindingSource = this.bsDefect;
			this.bnDefect.CountItem = this.bindingNavigatorCountItem;
			this.bnDefect.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bnDefect.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bnDefect.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
            this.toolStripSeparator1,
            this.toolLotSearch});
			this.bnDefect.Location = new System.Drawing.Point(3, 492);
			this.bnDefect.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnDefect.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnDefect.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnDefect.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnDefect.Name = "bnDefect";
			this.bnDefect.PositionItem = this.bindingNavigatorPositionItem;
			this.bnDefect.Size = new System.Drawing.Size(984, 25);
			this.bnDefect.TabIndex = 1;
			this.bnDefect.Text = "bindingNavigator1";
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
			// toolPreserve
			// 
			this.toolPreserve.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolPreserve.Enabled = false;
			this.toolPreserve.Image = ((System.Drawing.Image)(resources.GetObject("toolPreserve.Image")));
			this.toolPreserve.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolPreserve.Name = "toolPreserve";
			this.toolPreserve.Size = new System.Drawing.Size(49, 22);
			this.toolPreserve.Text = "保存";
			this.toolPreserve.Click += new System.EventHandler(this.toolPreserve_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// toolLotSearch
			// 
			this.toolLotSearch.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolLotSearch.Image = ((System.Drawing.Image)(resources.GetObject("toolLotSearch.Image")));
			this.toolLotSearch.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.toolLotSearch.Name = "toolLotSearch";
			this.toolLotSearch.Size = new System.Drawing.Size(97, 22);
			this.toolLotSearch.Text = "選択ロット検索";
			this.toolLotSearch.Click += new System.EventHandler(this.toolLotSearch_Click);
			// 
			// cmbServer
			// 
			this.cmbServer.FormattingEnabled = true;
			this.cmbServer.Location = new System.Drawing.Point(863, 17);
			this.cmbServer.Name = "cmbServer";
			this.cmbServer.Size = new System.Drawing.Size(121, 20);
			this.cmbServer.SourceNVC = null;
			this.cmbServer.TabIndex = 2;
			// 
			// txtPlantCD
			// 
			this.txtPlantCD.Location = new System.Drawing.Point(85, 55);
			this.txtPlantCD.Name = "txtPlantCD";
			this.txtPlantCD.Size = new System.Drawing.Size(123, 19);
			this.txtPlantCD.TabIndex = 2;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(26, 58);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "設備番号";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(26, 83);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(53, 12);
			this.label2.TabIndex = 5;
			this.label2.Text = "ロット番号";
			// 
			// txtLotNO
			// 
			this.txtLotNO.Location = new System.Drawing.Point(85, 80);
			this.txtLotNO.Name = "txtLotNO";
			this.txtLotNO.Size = new System.Drawing.Size(123, 19);
			this.txtLotNO.TabIndex = 4;
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(26, 33);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(47, 12);
			this.label3.TabIndex = 7;
			this.label3.Text = "ラインCD";
			// 
			// groupBox2
			// 
			this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox2.Controls.Add(this.label8);
			this.groupBox2.Controls.Add(this.chkIsTargetDelRecord);
			this.groupBox2.Controls.Add(this.txtLineCD);
			this.groupBox2.Controls.Add(this.label5);
			this.groupBox2.Controls.Add(this.dtsTargetDT);
			this.groupBox2.Controls.Add(this.label4);
			this.groupBox2.Controls.Add(this.txtDefItemNM);
			this.groupBox2.Controls.Add(this.btnSearch);
			this.groupBox2.Controls.Add(this.label3);
			this.groupBox2.Controls.Add(this.txtPlantCD);
			this.groupBox2.Controls.Add(this.label1);
			this.groupBox2.Controls.Add(this.label2);
			this.groupBox2.Controls.Add(this.txtLotNO);
			this.groupBox2.Location = new System.Drawing.Point(12, 12);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.Size = new System.Drawing.Size(990, 138);
			this.groupBox2.TabIndex = 8;
			this.groupBox2.TabStop = false;
			this.groupBox2.Text = "検索条件";
			// 
			// label8
			// 
			this.label8.AutoSize = true;
			this.label8.ForeColor = System.Drawing.Color.Red;
			this.label8.Location = new System.Drawing.Point(26, 109);
			this.label8.Name = "label8";
			this.label8.Size = new System.Drawing.Size(257, 12);
			this.label8.TabIndex = 15;
			this.label8.Text = "※保存する場合、ロット番号指定で検索してください。";
			// 
			// chkIsTargetDelRecord
			// 
			this.chkIsTargetDelRecord.AutoSize = true;
			this.chkIsTargetDelRecord.Location = new System.Drawing.Point(239, 83);
			this.chkIsTargetDelRecord.Name = "chkIsTargetDelRecord";
			this.chkIsTargetDelRecord.Size = new System.Drawing.Size(138, 16);
			this.chkIsTargetDelRecord.TabIndex = 14;
			this.chkIsTargetDelRecord.Text = "削除済みレコードを含む";
			this.chkIsTargetDelRecord.UseVisualStyleBackColor = true;
			// 
			// txtLineCD
			// 
			this.txtLineCD.Location = new System.Drawing.Point(85, 30);
			this.txtLineCD.Name = "txtLineCD";
			this.txtLineCD.Size = new System.Drawing.Size(123, 19);
			this.txtLineCD.TabIndex = 13;
			// 
			// label5
			// 
			this.label5.AutoSize = true;
			this.label5.Location = new System.Drawing.Point(237, 58);
			this.label5.Name = "label5";
			this.label5.Size = new System.Drawing.Size(53, 12);
			this.label5.TabIndex = 12;
			this.label5.Text = "発生日時";
			// 
			// dtsTargetDT
			// 
			this.dtsTargetDT.Checked = true;
			this.dtsTargetDT.Conjuction = SLCommonLib.DateConjuction.to;
			this.dtsTargetDT.CustomFormat = "yyyy/MM/dd HH:mm:ss";
			this.dtsTargetDT.EndDate = "2015/08/14 23:59:59";
			this.dtsTargetDT.IsChangeConjuction = false;
			this.dtsTargetDT.Location = new System.Drawing.Point(296, 54);
			this.dtsTargetDT.MaxDateTime = new System.DateTime(9990, 12, 31, 23, 59, 59, 0);
			this.dtsTargetDT.MinDateTime = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
			this.dtsTargetDT.Name = "dtsTargetDT";
			this.dtsTargetDT.Size = new System.Drawing.Size(378, 23);
			this.dtsTargetDT.StartDate = "2015/08/14 00:00:00";
			this.dtsTargetDT.TabIndex = 11;
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(237, 33);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(41, 12);
			this.label4.TabIndex = 10;
			this.label4.Text = "不良名";
			// 
			// txtDefItemNM
			// 
			this.txtDefItemNM.Location = new System.Drawing.Point(296, 29);
			this.txtDefItemNM.Name = "txtDefItemNM";
			this.txtDefItemNM.Size = new System.Drawing.Size(123, 19);
			this.txtDefItemNM.TabIndex = 9;
			// 
			// btnSearch
			// 
			this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
			this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.btnSearch.Location = new System.Drawing.Point(864, 98);
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.Size = new System.Drawing.Size(82, 23);
			this.btnSearch.TabIndex = 8;
			this.btnSearch.Text = "検索";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStatus});
			this.statusStrip.Location = new System.Drawing.Point(0, 679);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.Size = new System.Drawing.Size(1016, 22);
			this.statusStrip.TabIndex = 9;
			this.statusStrip.Text = "statusStrip1";
			// 
			// toolStatus
			// 
			this.toolStatus.Name = "toolStatus";
			this.toolStatus.Size = new System.Drawing.Size(0, 17);
			// 
			// F06_DefectRecord
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(1016, 701);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.Name = "F06_DefectRecord";
			this.Text = "F06 不良履歴";
			this.Load += new System.EventHandler(this.frmDefectData_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvDefect)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsDefect)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bnDefect)).EndInit();
			this.bnDefect.ResumeLayout(false);
			this.bnDefect.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvDefect;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.BindingNavigator bnDefect;
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
        private System.Windows.Forms.TextBox txtPlantCD;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLotNO;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label5;
        private SLCommonLib.UCDateTimeSelector dtsTargetDT;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtDefItemNM;
        private System.Windows.Forms.ToolStripButton toolPreserve;
        private System.Windows.Forms.TextBox txtLineCD;
        private System.Windows.Forms.BindingSource bsDefect;
        private System.Windows.Forms.ToolStripButton toolLotSearch;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private SLCommonLib.UCComboBox cmbServer;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
		private System.Windows.Forms.ToolStripStatusLabel toolStatus;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ChangeFG;
		private System.Windows.Forms.DataGridViewTextBoxColumn LineCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn PlantCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn LotNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefAddressNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn UpdateDefAddressNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefUnitNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn TargetDT;
		private System.Windows.Forms.DataGridViewTextBoxColumn WorkCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefItemCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefItemNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefCauseCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefCauseNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefClassCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn DefClassNM;
		private System.Windows.Forms.DataGridViewComboBoxColumn TransactionCD;
		private System.Windows.Forms.DataGridViewComboBoxColumn UpdateTransactionCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn UpdUserCD;
		private System.Windows.Forms.DataGridViewCheckBoxColumn DelFG;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDT;
		private System.Windows.Forms.DataGridViewCheckBoxColumn AddressCompareFG;
		private System.Windows.Forms.CheckBox chkIsTargetDelRecord;
		private System.Windows.Forms.Label label8;
    }
}