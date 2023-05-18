namespace GEICS
{
    partial class M05_StandardText
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M05_StandardText));
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
			this.cmbAssets = new SLCommonLib.UCComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.dgvStandardText = new System.Windows.Forms.DataGridView();
			this.Assets_NM = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.fixedNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.fixedNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.delFGDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.updUserCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.lastUpdDTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dsStandardTextBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dsStandardText = new GEICS.dsStandardText();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
			this.bindingNavigator = new System.Windows.Forms.BindingNavigator(this.components);
			this.toolbtnAdd = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
			this.toolbtnDelete = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
			this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
			this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.toolbtnPreserve = new System.Windows.Forms.ToolStripButton();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toollblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServerhed = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServer = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblEmp = new System.Windows.Forms.ToolStripStatusLabel();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvStandardText)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsStandardTextBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsStandardText)).BeginInit();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigator)).BeginInit();
			this.bindingNavigator.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbAssets
			// 
			this.cmbAssets.FormattingEnabled = true;
			resources.ApplyResources(this.cmbAssets, "cmbAssets");
			this.cmbAssets.Name = "cmbAssets";
			this.cmbAssets.SourceNVC = null;
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBox1
			// 
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Controls.Add(this.btnSearch);
			this.groupBox1.Controls.Add(this.cmbAssets);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// btnSearch
			// 
			resources.ApplyResources(this.btnSearch, "btnSearch");
			this.btnSearch.Name = "btnSearch";
			this.btnSearch.UseVisualStyleBackColor = true;
			this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
			// 
			// dgvStandardText
			// 
			this.dgvStandardText.AllowUserToAddRows = false;
			this.dgvStandardText.AllowUserToResizeRows = false;
			resources.ApplyResources(this.dgvStandardText, "dgvStandardText");
			this.dgvStandardText.AutoGenerateColumns = false;
			this.dgvStandardText.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvStandardText.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Assets_NM,
            this.fixedNODataGridViewTextBoxColumn,
            this.fixedNMDataGridViewTextBoxColumn,
            this.delFGDataGridViewCheckBoxColumn,
            this.updUserCDDataGridViewTextBoxColumn,
            this.lastUpdDTDataGridViewTextBoxColumn});
			this.dgvStandardText.DataSource = this.dsStandardTextBindingSource;
			this.dgvStandardText.MultiSelect = false;
			this.dgvStandardText.Name = "dgvStandardText";
			this.dgvStandardText.RowHeadersVisible = false;
			this.dgvStandardText.RowTemplate.Height = 21;
			this.dgvStandardText.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			// 
			// Assets_NM
			// 
			this.Assets_NM.DataPropertyName = "Assets_NM";
			resources.ApplyResources(this.Assets_NM, "Assets_NM");
			this.Assets_NM.Name = "Assets_NM";
			this.Assets_NM.ReadOnly = true;
			this.Assets_NM.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			// 
			// fixedNODataGridViewTextBoxColumn
			// 
			this.fixedNODataGridViewTextBoxColumn.DataPropertyName = "Fixed_NO";
			resources.ApplyResources(this.fixedNODataGridViewTextBoxColumn, "fixedNODataGridViewTextBoxColumn");
			this.fixedNODataGridViewTextBoxColumn.Name = "fixedNODataGridViewTextBoxColumn";
			this.fixedNODataGridViewTextBoxColumn.ReadOnly = true;
			this.fixedNODataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// fixedNMDataGridViewTextBoxColumn
			// 
			this.fixedNMDataGridViewTextBoxColumn.DataPropertyName = "Fixed_NM";
			dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Info;
			this.fixedNMDataGridViewTextBoxColumn.DefaultCellStyle = dataGridViewCellStyle5;
			resources.ApplyResources(this.fixedNMDataGridViewTextBoxColumn, "fixedNMDataGridViewTextBoxColumn");
			this.fixedNMDataGridViewTextBoxColumn.Name = "fixedNMDataGridViewTextBoxColumn";
			this.fixedNMDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// delFGDataGridViewCheckBoxColumn
			// 
			this.delFGDataGridViewCheckBoxColumn.DataPropertyName = "Del_FG";
			dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Info;
			dataGridViewCellStyle6.NullValue = false;
			this.delFGDataGridViewCheckBoxColumn.DefaultCellStyle = dataGridViewCellStyle6;
			resources.ApplyResources(this.delFGDataGridViewCheckBoxColumn, "delFGDataGridViewCheckBoxColumn");
			this.delFGDataGridViewCheckBoxColumn.Name = "delFGDataGridViewCheckBoxColumn";
			// 
			// updUserCDDataGridViewTextBoxColumn
			// 
			this.updUserCDDataGridViewTextBoxColumn.DataPropertyName = "UpdUser_CD";
			resources.ApplyResources(this.updUserCDDataGridViewTextBoxColumn, "updUserCDDataGridViewTextBoxColumn");
			this.updUserCDDataGridViewTextBoxColumn.Name = "updUserCDDataGridViewTextBoxColumn";
			this.updUserCDDataGridViewTextBoxColumn.ReadOnly = true;
			this.updUserCDDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// lastUpdDTDataGridViewTextBoxColumn
			// 
			this.lastUpdDTDataGridViewTextBoxColumn.DataPropertyName = "LastUpd_DT";
			resources.ApplyResources(this.lastUpdDTDataGridViewTextBoxColumn, "lastUpdDTDataGridViewTextBoxColumn");
			this.lastUpdDTDataGridViewTextBoxColumn.Name = "lastUpdDTDataGridViewTextBoxColumn";
			this.lastUpdDTDataGridViewTextBoxColumn.ReadOnly = true;
			this.lastUpdDTDataGridViewTextBoxColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
			// 
			// dsStandardTextBindingSource
			// 
			this.dsStandardTextBindingSource.DataMember = "dtFNM";
			this.dsStandardTextBindingSource.DataSource = this.dsStandardText;
			// 
			// dsStandardText
			// 
			this.dsStandardText.DataSetName = "dsStandardText";
			this.dsStandardText.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.bindingNavigator);
			this.groupBox2.Controls.Add(this.dgvStandardText);
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// bindingNavigator
			// 
			this.bindingNavigator.AddNewItem = this.toolbtnAdd;
			this.bindingNavigator.BindingSource = this.dsStandardTextBindingSource;
			this.bindingNavigator.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigator.DeleteItem = this.toolbtnDelete;
			resources.ApplyResources(this.bindingNavigator, "bindingNavigator");
			this.bindingNavigator.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.toolbtnAdd,
            this.toolbtnDelete,
            this.toolbtnPreserve});
			this.bindingNavigator.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigator.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigator.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigator.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigator.Name = "bindingNavigator";
			this.bindingNavigator.PositionItem = this.bindingNavigatorPositionItem;
			// 
			// toolbtnAdd
			// 
			resources.ApplyResources(this.toolbtnAdd, "toolbtnAdd");
			this.toolbtnAdd.Name = "toolbtnAdd";
			this.toolbtnAdd.Click += new System.EventHandler(this.toolbtnAdd_Click);
			// 
			// bindingNavigatorCountItem
			// 
			this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
			resources.ApplyResources(this.bindingNavigatorCountItem, "bindingNavigatorCountItem");
			// 
			// toolbtnDelete
			// 
			resources.ApplyResources(this.toolbtnDelete, "toolbtnDelete");
			this.toolbtnDelete.Name = "toolbtnDelete";
			// 
			// bindingNavigatorMoveFirstItem
			// 
			this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.bindingNavigatorMoveFirstItem, "bindingNavigatorMoveFirstItem");
			this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
			// 
			// bindingNavigatorMovePreviousItem
			// 
			this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.bindingNavigatorMovePreviousItem, "bindingNavigatorMovePreviousItem");
			this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
			// 
			// bindingNavigatorSeparator
			// 
			this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
			resources.ApplyResources(this.bindingNavigatorSeparator, "bindingNavigatorSeparator");
			// 
			// bindingNavigatorPositionItem
			// 
			resources.ApplyResources(this.bindingNavigatorPositionItem, "bindingNavigatorPositionItem");
			this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
			// 
			// bindingNavigatorSeparator1
			// 
			this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
			resources.ApplyResources(this.bindingNavigatorSeparator1, "bindingNavigatorSeparator1");
			// 
			// bindingNavigatorMoveNextItem
			// 
			this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.bindingNavigatorMoveNextItem, "bindingNavigatorMoveNextItem");
			this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
			// 
			// bindingNavigatorMoveLastItem
			// 
			this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.bindingNavigatorMoveLastItem, "bindingNavigatorMoveLastItem");
			this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
			// 
			// bindingNavigatorSeparator2
			// 
			this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
			resources.ApplyResources(this.bindingNavigatorSeparator2, "bindingNavigatorSeparator2");
			// 
			// toolbtnPreserve
			// 
			this.toolbtnPreserve.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			resources.ApplyResources(this.toolbtnPreserve, "toolbtnPreserve");
			this.toolbtnPreserve.Name = "toolbtnPreserve";
			this.toolbtnPreserve.Click += new System.EventHandler(this.toolbtnPreserve_Click);
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toollblStatusText,
            this.toollblServerhed,
            this.toollblServer,
            this.toolStripStatusLabel1,
            this.toollblEmp});
			resources.ApplyResources(this.statusStrip, "statusStrip");
			this.statusStrip.Name = "statusStrip";
			// 
			// toollblStatusText
			// 
			this.toollblStatusText.Name = "toollblStatusText";
			resources.ApplyResources(this.toollblStatusText, "toollblStatusText");
			this.toollblStatusText.Spring = true;
			// 
			// toollblServerhed
			// 
			this.toollblServerhed.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toollblServerhed.Name = "toollblServerhed";
			resources.ApplyResources(this.toollblServerhed, "toollblServerhed");
			// 
			// toollblServer
			// 
			this.toollblServer.Name = "toollblServer";
			resources.ApplyResources(this.toollblServer, "toollblServer");
			// 
			// toolStripStatusLabel1
			// 
			this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
			this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
			resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
			// 
			// toollblEmp
			// 
			this.toollblEmp.Name = "toollblEmp";
			resources.ApplyResources(this.toollblEmp, "toollblEmp");
			// 
			// M05_StandardText
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.Name = "M05_StandardText";
			this.Load += new System.EventHandler(this.frmStandardText_Load);
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dgvStandardText)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsStandardTextBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsStandardText)).EndInit();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigator)).EndInit();
			this.bindingNavigator.ResumeLayout(false);
			this.bindingNavigator.PerformLayout();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private SLCommonLib.UCComboBox cmbAssets;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvStandardText;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.BindingNavigator bindingNavigator;
        private System.Windows.Forms.ToolStripButton toolbtnAdd;
        private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
        private System.Windows.Forms.ToolStripButton toolbtnDelete;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
        private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
        private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
        private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
        private System.Windows.Forms.ToolStripButton toolbtnPreserve;
        private System.Windows.Forms.BindingSource dsStandardTextBindingSource;
        private dsStandardText dsStandardText;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toollblStatusText;
        private System.Windows.Forms.ToolStripStatusLabel toollblServerhed;
        private System.Windows.Forms.ToolStripStatusLabel toollblServer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toollblEmp;
        private System.Windows.Forms.DataGridViewComboBoxColumn Assets_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn fixedNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fixedNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn delFGDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn updUserCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdDTDataGridViewTextBoxColumn;
    }
}