namespace GEICS
{
    partial class M02_LimitRecord
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M02_LimitRecord));
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.dgvItems = new System.Windows.Forms.DataGridView();
			this.RevNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QcParamNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ModelNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MaterialCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ClassNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ParameterNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ManageNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Info1NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Info2NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Info3NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.TimingNM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.EquipmentNO = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ParameterMAX = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.InnerUpperLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ParameterMIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.InnerLowerLimit = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ParameterVAL = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QcLinePNT = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QcLineMAX = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.QcLineMIN = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.ReasonVAL = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.UpdUserCD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.LastUpdDT = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.bsLimitRecord = new System.Windows.Forms.BindingSource(this.components);
			this.bnLimitRecord = new System.Windows.Forms.BindingNavigator(this.components);
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
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bsLimitRecord)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.bnLimitRecord)).BeginInit();
			this.bnLimitRecord.SuspendLayout();
			this.SuspendLayout();
			// 
			// groupBox1
			// 
			this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.groupBox1.Controls.Add(this.dgvItems);
			this.groupBox1.Controls.Add(this.bnLimitRecord);
			this.groupBox1.Location = new System.Drawing.Point(12, 12);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(857, 413);
			this.groupBox1.TabIndex = 1;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "履歴一覧";
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
            this.RevNO,
            this.QcParamNO,
            this.ModelNM,
            this.MaterialCD,
            this.ClassNM,
            this.ParameterNM,
            this.ManageNM,
            this.Info1NM,
            this.Info2NM,
            this.Info3NM,
            this.TimingNM,
            this.EquipmentNO,
            this.ParameterMAX,
            this.InnerUpperLimit,
            this.ParameterMIN,
            this.InnerLowerLimit,
            this.ParameterVAL,
            this.QcLinePNT,
            this.QcLineMAX,
            this.QcLineMIN,
            this.ReasonVAL,
            this.UpdUserCD,
            this.LastUpdDT});
			this.dgvItems.DataSource = this.bsLimitRecord;
			this.dgvItems.Location = new System.Drawing.Point(6, 18);
			this.dgvItems.MultiSelect = false;
			this.dgvItems.Name = "dgvItems";
			this.dgvItems.ReadOnly = true;
			this.dgvItems.RowHeadersVisible = false;
			this.dgvItems.RowTemplate.Height = 21;
			this.dgvItems.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvItems.Size = new System.Drawing.Size(845, 364);
			this.dgvItems.TabIndex = 3;
			// 
			// RevNO
			// 
			this.RevNO.DataPropertyName = "RevNO";
			this.RevNO.HeaderText = "履歴NO";
			this.RevNO.Name = "RevNO";
			this.RevNO.ReadOnly = true;
			this.RevNO.Width = 70;
			// 
			// QcParamNO
			// 
			this.QcParamNO.DataPropertyName = "QcParamNO";
			this.QcParamNO.HeaderText = "管理No";
			this.QcParamNO.Name = "QcParamNO";
			this.QcParamNO.ReadOnly = true;
			this.QcParamNO.Width = 80;
			// 
			// ModelNM
			// 
			this.ModelNM.DataPropertyName = "ModelNM";
			this.ModelNM.HeaderText = "装置型式";
			this.ModelNM.Name = "ModelNM";
			this.ModelNM.ReadOnly = true;
			this.ModelNM.Width = 80;
			// 
			// MaterialCD
			// 
			this.MaterialCD.DataPropertyName = "MaterialCD";
			this.MaterialCD.HeaderText = "製品型番";
			this.MaterialCD.Name = "MaterialCD";
			this.MaterialCD.ReadOnly = true;
			// 
			// ClassNM
			// 
			this.ClassNM.DataPropertyName = "ClassNM";
			this.ClassNM.HeaderText = "分類";
			this.ClassNM.Name = "ClassNM";
			this.ClassNM.ReadOnly = true;
			// 
			// ParameterNM
			// 
			this.ParameterNM.DataPropertyName = "ParameterNM";
			this.ParameterNM.HeaderText = "管理名";
			this.ParameterNM.Name = "ParameterNM";
			this.ParameterNM.ReadOnly = true;
			this.ParameterNM.Width = 120;
			// 
			// ManageNM
			// 
			this.ManageNM.DataPropertyName = "ManageNM";
			this.ManageNM.HeaderText = "管理方法";
			this.ManageNM.Name = "ManageNM";
			this.ManageNM.ReadOnly = true;
			this.ManageNM.Width = 80;
			// 
			// Info1NM
			// 
			this.Info1NM.DataPropertyName = "Info1NM";
			this.Info1NM.HeaderText = "補足情報1";
			this.Info1NM.Name = "Info1NM";
			this.Info1NM.ReadOnly = true;
			this.Info1NM.Width = 85;
			// 
			// Info2NM
			// 
			this.Info2NM.DataPropertyName = "Info2NM";
			this.Info2NM.HeaderText = "補足情報2";
			this.Info2NM.Name = "Info2NM";
			this.Info2NM.ReadOnly = true;
			// 
			// Info3NM
			// 
			this.Info3NM.DataPropertyName = "Info3NM";
			this.Info3NM.HeaderText = "表示";
			this.Info3NM.Name = "Info3NM";
			this.Info3NM.ReadOnly = true;
			this.Info3NM.Width = 80;
			// 
			// TimingNM
			// 
			this.TimingNM.DataPropertyName = "TimingNM";
			this.TimingNM.HeaderText = "タイミング";
			this.TimingNM.Name = "TimingNM";
			this.TimingNM.ReadOnly = true;
			this.TimingNM.Width = 80;
			// 
			// EquipmentNO
			// 
			this.EquipmentNO.DataPropertyName = "EquipmentNO";
			this.EquipmentNO.HeaderText = "設備CD";
			this.EquipmentNO.Name = "EquipmentNO";
			this.EquipmentNO.ReadOnly = true;
			// 
			// ParameterMAX
			// 
			this.ParameterMAX.DataPropertyName = "ParameterMAX";
			this.ParameterMAX.HeaderText = "最大";
			this.ParameterMAX.Name = "ParameterMAX";
			this.ParameterMAX.ReadOnly = true;
			this.ParameterMAX.Width = 80;
			// 
			// InnerUpperLimit
			// 
			this.InnerUpperLimit.DataPropertyName = "InnerUpperLimit";
			this.InnerUpperLimit.HeaderText = "最大(内規)";
			this.InnerUpperLimit.Name = "InnerUpperLimit";
			this.InnerUpperLimit.ReadOnly = true;
			// 
			// ParameterMIN
			// 
			this.ParameterMIN.DataPropertyName = "ParameterMIN";
			this.ParameterMIN.HeaderText = "最小";
			this.ParameterMIN.Name = "ParameterMIN";
			this.ParameterMIN.ReadOnly = true;
			this.ParameterMIN.Width = 80;
			// 
			// InnerLowerLimit
			// 
			this.InnerLowerLimit.DataPropertyName = "InnerLowerLimit";
			this.InnerLowerLimit.HeaderText = "最小(内規)";
			this.InnerLowerLimit.Name = "InnerLowerLimit";
			this.InnerLowerLimit.ReadOnly = true;
			// 
			// ParameterVAL
			// 
			this.ParameterVAL.DataPropertyName = "ParameterVAL";
			this.ParameterVAL.HeaderText = "文字列";
			this.ParameterVAL.Name = "ParameterVAL";
			this.ParameterVAL.ReadOnly = true;
			// 
			// QcLinePNT
			// 
			this.QcLinePNT.DataPropertyName = "QcLinePNT";
			this.QcLinePNT.HeaderText = "*点連続";
			this.QcLinePNT.Name = "QcLinePNT";
			this.QcLinePNT.ReadOnly = true;
			this.QcLinePNT.Width = 80;
			// 
			// QcLineMAX
			// 
			this.QcLineMAX.DataPropertyName = "QcLineMAX";
			this.QcLineMAX.HeaderText = "最大";
			this.QcLineMAX.Name = "QcLineMAX";
			this.QcLineMAX.ReadOnly = true;
			this.QcLineMAX.Width = 80;
			// 
			// QcLineMIN
			// 
			this.QcLineMIN.DataPropertyName = "QcLineMIN";
			this.QcLineMIN.HeaderText = "最小";
			this.QcLineMIN.Name = "QcLineMIN";
			this.QcLineMIN.ReadOnly = true;
			this.QcLineMIN.Width = 80;
			// 
			// ReasonVAL
			// 
			this.ReasonVAL.DataPropertyName = "ReasonVAL";
			this.ReasonVAL.HeaderText = "変更理由";
			this.ReasonVAL.Name = "ReasonVAL";
			this.ReasonVAL.ReadOnly = true;
			// 
			// UpdUserCD
			// 
			this.UpdUserCD.DataPropertyName = "UpdUserCD";
			this.UpdUserCD.HeaderText = "更新者CD";
			this.UpdUserCD.Name = "UpdUserCD";
			this.UpdUserCD.ReadOnly = true;
			// 
			// LastUpdDT
			// 
			this.LastUpdDT.DataPropertyName = "LastUpdDT";
			this.LastUpdDT.HeaderText = "更新日時";
			this.LastUpdDT.Name = "LastUpdDT";
			this.LastUpdDT.ReadOnly = true;
			// 
			// bnLimitRecord
			// 
			this.bnLimitRecord.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bnLimitRecord.BindingSource = this.bsLimitRecord;
			this.bnLimitRecord.CountItem = this.bindingNavigatorCountItem;
			this.bnLimitRecord.DeleteItem = this.bindingNavigatorDeleteItem;
			this.bnLimitRecord.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.bnLimitRecord.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
			this.bnLimitRecord.Location = new System.Drawing.Point(3, 385);
			this.bnLimitRecord.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bnLimitRecord.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bnLimitRecord.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bnLimitRecord.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bnLimitRecord.Name = "bnLimitRecord";
			this.bnLimitRecord.PositionItem = this.bindingNavigatorPositionItem;
			this.bnLimitRecord.Size = new System.Drawing.Size(851, 25);
			this.bnLimitRecord.TabIndex = 1;
			this.bnLimitRecord.Text = "bindingNavigator1";
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
			// M02_LimitRecord
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(881, 437);
			this.Controls.Add(this.groupBox1);
			this.Name = "M02_LimitRecord";
			this.Text = "マスタメンテ - 閾値履歴";
			this.Load += new System.EventHandler(this.frmMasterLimitRecord_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bsLimitRecord)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.bnLimitRecord)).EndInit();
			this.bnLimitRecord.ResumeLayout(false);
			this.bnLimitRecord.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.BindingNavigator bnLimitRecord;
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
        private System.Windows.Forms.DataGridView dgvItems;
		private System.Windows.Forms.BindingSource bsLimitRecord;
		private System.Windows.Forms.DataGridViewTextBoxColumn RevNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn QcParamNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn ModelNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn MaterialCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn ClassNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn ParameterNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn ManageNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn Info1NM;
		private System.Windows.Forms.DataGridViewTextBoxColumn Info2NM;
		private System.Windows.Forms.DataGridViewTextBoxColumn Info3NM;
		private System.Windows.Forms.DataGridViewTextBoxColumn TimingNM;
		private System.Windows.Forms.DataGridViewTextBoxColumn EquipmentNO;
		private System.Windows.Forms.DataGridViewTextBoxColumn ParameterMAX;
		private System.Windows.Forms.DataGridViewTextBoxColumn InnerUpperLimit;
		private System.Windows.Forms.DataGridViewTextBoxColumn ParameterMIN;
		private System.Windows.Forms.DataGridViewTextBoxColumn InnerLowerLimit;
		private System.Windows.Forms.DataGridViewTextBoxColumn ParameterVAL;
		private System.Windows.Forms.DataGridViewTextBoxColumn QcLinePNT;
		private System.Windows.Forms.DataGridViewTextBoxColumn QcLineMAX;
		private System.Windows.Forms.DataGridViewTextBoxColumn QcLineMIN;
		private System.Windows.Forms.DataGridViewTextBoxColumn ReasonVAL;
		private System.Windows.Forms.DataGridViewTextBoxColumn UpdUserCD;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDT;
    }
}