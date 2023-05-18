namespace GEICS
{
    partial class frmMasterSetting
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMasterSetting));
			this.cmbbType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cmbbLineNo = new System.Windows.Forms.ComboBox();
			this.label23 = new System.Windows.Forms.Label();
			this.btnOutputExcel = new System.Windows.Forms.Button();
			this.btnReadExcel = new System.Windows.Forms.Button();
			this.btnEntry = new System.Windows.Forms.Button();
			this.btnOutputForm = new System.Windows.Forms.Button();
			this.txtbOutputDate = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.gvMasterSetting = new System.Windows.Forms.DataGridView();
			this.Revision = new System.Windows.Forms.DataGridViewButtonColumn();
			this.col2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.col3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col4 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col5 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Col6 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col7 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col8 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col9 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Info1_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Info2_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Info3_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col10 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col11 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col12 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.col13 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Col14 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Col15 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Col16 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Col17 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Col18 = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dSFGDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
			this.tvPLMExBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dsTmPLMExBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dsTmPLMEx = new GEICS.dsTmPLMEx();
			this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
			this.label3 = new System.Windows.Forms.Label();
			this.txtDiceCount = new System.Windows.Forms.TextBox();
			this.label4 = new System.Windows.Forms.Label();
			this.label5 = new System.Windows.Forms.Label();
			this.txtType = new System.Windows.Forms.TextBox();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toollblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServerhed = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServer = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblEmp = new System.Windows.Forms.ToolStripStatusLabel();
			this.dsTmQCST = new GEICS.dsTmQCST();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.bindingNavigator1 = new System.Windows.Forms.BindingNavigator(this.components);
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
			((System.ComponentModel.ISupportInitialize)(this.gvMasterSetting)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.tvPLMExBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsTmPLMExBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsTmPLMEx)).BeginInit();
			this.statusStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.dsTmQCST)).BeginInit();
			this.groupBox1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
			this.bindingNavigator1.SuspendLayout();
			this.SuspendLayout();
			// 
			// cmbbType
			// 
			this.cmbbType.FormattingEnabled = true;
			resources.ApplyResources(this.cmbbType, "cmbbType");
			this.cmbbType.Name = "cmbbType";
			this.cmbbType.SelectedIndexChanged += new System.EventHandler(this.cmbbType_SelectedIndexChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// cmbbLineNo
			// 
			resources.ApplyResources(this.cmbbLineNo, "cmbbLineNo");
			this.cmbbLineNo.FormattingEnabled = true;
			this.cmbbLineNo.Name = "cmbbLineNo";
			this.cmbbLineNo.SelectedIndexChanged += new System.EventHandler(this.cmbbLineNo_SelectedIndexChanged);
			// 
			// label23
			// 
			resources.ApplyResources(this.label23, "label23");
			this.label23.Name = "label23";
			// 
			// btnOutputExcel
			// 
			resources.ApplyResources(this.btnOutputExcel, "btnOutputExcel");
			this.btnOutputExcel.Name = "btnOutputExcel";
			this.btnOutputExcel.UseVisualStyleBackColor = true;
			this.btnOutputExcel.Click += new System.EventHandler(this.btnOutputExcel_Click);
			// 
			// btnReadExcel
			// 
			resources.ApplyResources(this.btnReadExcel, "btnReadExcel");
			this.btnReadExcel.Name = "btnReadExcel";
			this.btnReadExcel.UseVisualStyleBackColor = true;
			this.btnReadExcel.Click += new System.EventHandler(this.btnReadExcel_Click);
			// 
			// btnEntry
			// 
			resources.ApplyResources(this.btnEntry, "btnEntry");
			this.btnEntry.Name = "btnEntry";
			this.btnEntry.UseVisualStyleBackColor = true;
			this.btnEntry.Click += new System.EventHandler(this.btnEntry_Click);
			// 
			// btnOutputForm
			// 
			resources.ApplyResources(this.btnOutputForm, "btnOutputForm");
			this.btnOutputForm.Name = "btnOutputForm";
			this.btnOutputForm.UseVisualStyleBackColor = true;
			this.btnOutputForm.Click += new System.EventHandler(this.btnOutputForm_Click);
			// 
			// txtbOutputDate
			// 
			resources.ApplyResources(this.txtbOutputDate, "txtbOutputDate");
			this.txtbOutputDate.Name = "txtbOutputDate";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// gvMasterSetting
			// 
			this.gvMasterSetting.AllowUserToAddRows = false;
			this.gvMasterSetting.AllowUserToDeleteRows = false;
			resources.ApplyResources(this.gvMasterSetting, "gvMasterSetting");
			this.gvMasterSetting.AutoGenerateColumns = false;
			this.gvMasterSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.gvMasterSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Revision,
            this.col2,
            this.col3,
            this.col4,
            this.col5,
            this.Col6,
            this.col7,
            this.col8,
            this.col9,
            this.Info1_NM,
            this.Info2_NM,
            this.Info3_NM,
            this.col10,
            this.col11,
            this.col12,
            this.col13,
            this.Col14,
            this.Col15,
            this.Col16,
            this.Col17,
            this.Col18,
            this.dSFGDataGridViewCheckBoxColumn});
			this.gvMasterSetting.DataSource = this.tvPLMExBindingSource;
			this.gvMasterSetting.Name = "gvMasterSetting";
			this.gvMasterSetting.ReadOnly = true;
			this.gvMasterSetting.RowTemplate.Height = 21;
			this.gvMasterSetting.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvMasterSetting_CellContentClick);
			// 
			// Revision
			// 
			this.Revision.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			resources.ApplyResources(this.Revision, "Revision");
			this.Revision.Name = "Revision";
			this.Revision.ReadOnly = true;
			this.Revision.Text = "確認";
			this.Revision.UseColumnTextForButtonValue = true;
			// 
			// col2
			// 
			this.col2.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.col2.DataPropertyName = "Edit_FG";
			resources.ApplyResources(this.col2, "col2");
			this.col2.Name = "col2";
			this.col2.ReadOnly = true;
			this.col2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
			this.col2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
			// 
			// col3
			// 
			this.col3.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.col3.DataPropertyName = "Rev";
			resources.ApplyResources(this.col3, "col3");
			this.col3.Name = "col3";
			this.col3.ReadOnly = true;
			// 
			// col4
			// 
			this.col4.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.col4.DataPropertyName = "QcParam_NO";
			resources.ApplyResources(this.col4, "col4");
			this.col4.Name = "col4";
			this.col4.ReadOnly = true;
			// 
			// col5
			// 
			this.col5.DataPropertyName = "Model_NM";
			resources.ApplyResources(this.col5, "col5");
			this.col5.Name = "col5";
			this.col5.ReadOnly = true;
			// 
			// Col6
			// 
			this.Col6.DataPropertyName = "Material_CD";
			resources.ApplyResources(this.Col6, "Col6");
			this.Col6.Name = "Col6";
			this.Col6.ReadOnly = true;
			// 
			// col7
			// 
			this.col7.DataPropertyName = "Class_NM";
			resources.ApplyResources(this.col7, "col7");
			this.col7.Name = "col7";
			this.col7.ReadOnly = true;
			// 
			// col8
			// 
			this.col8.DataPropertyName = "Parameter_NM";
			resources.ApplyResources(this.col8, "col8");
			this.col8.Name = "col8";
			this.col8.ReadOnly = true;
			// 
			// col9
			// 
			this.col9.DataPropertyName = "Manage_NM";
			resources.ApplyResources(this.col9, "col9");
			this.col9.Name = "col9";
			this.col9.ReadOnly = true;
			// 
			// Info1_NM
			// 
			this.Info1_NM.DataPropertyName = "Info1_NM";
			resources.ApplyResources(this.Info1_NM, "Info1_NM");
			this.Info1_NM.Name = "Info1_NM";
			this.Info1_NM.ReadOnly = true;
			// 
			// Info2_NM
			// 
			this.Info2_NM.DataPropertyName = "Info2_NM";
			resources.ApplyResources(this.Info2_NM, "Info2_NM");
			this.Info2_NM.Name = "Info2_NM";
			this.Info2_NM.ReadOnly = true;
			// 
			// Info3_NM
			// 
			this.Info3_NM.DataPropertyName = "Info3_NM";
			resources.ApplyResources(this.Info3_NM, "Info3_NM");
			this.Info3_NM.Name = "Info3_NM";
			this.Info3_NM.ReadOnly = true;
			// 
			// col10
			// 
			this.col10.DataPropertyName = "Timing_NM";
			resources.ApplyResources(this.col10, "col10");
			this.col10.Name = "col10";
			this.col10.ReadOnly = true;
			// 
			// col11
			// 
			this.col11.DataPropertyName = "Parameter_MAX";
			resources.ApplyResources(this.col11, "col11");
			this.col11.Name = "col11";
			this.col11.ReadOnly = true;
			// 
			// col12
			// 
			this.col12.DataPropertyName = "Parameter_MIN";
			resources.ApplyResources(this.col12, "col12");
			this.col12.Name = "col12";
			this.col12.ReadOnly = true;
			// 
			// col13
			// 
			this.col13.DataPropertyName = "Parameter_VAL";
			resources.ApplyResources(this.col13, "col13");
			this.col13.Name = "col13";
			this.col13.ReadOnly = true;
			// 
			// Col14
			// 
			this.Col14.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.Col14.DataPropertyName = "QcLine_PNT";
			resources.ApplyResources(this.Col14, "Col14");
			this.Col14.Name = "Col14";
			this.Col14.ReadOnly = true;
			// 
			// Col15
			// 
			this.Col15.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.Col15.DataPropertyName = "QcLine_MAX";
			resources.ApplyResources(this.Col15, "Col15");
			this.Col15.Name = "Col15";
			this.Col15.ReadOnly = true;
			// 
			// Col16
			// 
			this.Col16.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
			this.Col16.DataPropertyName = "QcLine_MIN";
			resources.ApplyResources(this.Col16, "Col16");
			this.Col16.Name = "Col16";
			this.Col16.ReadOnly = true;
			// 
			// Col17
			// 
			this.Col17.DataPropertyName = "Reason_NM";
			resources.ApplyResources(this.Col17, "Col17");
			this.Col17.Name = "Col17";
			this.Col17.ReadOnly = true;
			// 
			// Col18
			// 
			this.Col18.DataPropertyName = "UpdUser_CD";
			resources.ApplyResources(this.Col18, "Col18");
			this.Col18.Name = "Col18";
			this.Col18.ReadOnly = true;
			// 
			// dSFGDataGridViewCheckBoxColumn
			// 
			this.dSFGDataGridViewCheckBoxColumn.DataPropertyName = "DS_FG";
			resources.ApplyResources(this.dSFGDataGridViewCheckBoxColumn, "dSFGDataGridViewCheckBoxColumn");
			this.dSFGDataGridViewCheckBoxColumn.Name = "dSFGDataGridViewCheckBoxColumn";
			this.dSFGDataGridViewCheckBoxColumn.ReadOnly = true;
			// 
			// tvPLMExBindingSource
			// 
			this.tvPLMExBindingSource.DataMember = "TvPLMEx";
			this.tvPLMExBindingSource.DataSource = this.dsTmPLMExBindingSource;
			// 
			// dsTmPLMExBindingSource
			// 
			this.dsTmPLMExBindingSource.DataSource = this.dsTmPLMEx;
			this.dsTmPLMExBindingSource.Position = 0;
			// 
			// dsTmPLMEx
			// 
			this.dsTmPLMEx.DataSetName = "dsTmPLMEx";
			this.dsTmPLMEx.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// openFileDialog1
			// 
			this.openFileDialog1.FileName = "openFileDialog1";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// txtDiceCount
			// 
			resources.ApplyResources(this.txtDiceCount, "txtDiceCount");
			this.txtDiceCount.Name = "txtDiceCount";
			this.txtDiceCount.ReadOnly = true;
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// txtType
			// 
			resources.ApplyResources(this.txtType, "txtType");
			this.txtType.Name = "txtType";
			this.txtType.ReadOnly = true;
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
			// dsTmQCST
			// 
			this.dsTmQCST.DataSetName = "dsTmQCST";
			this.dsTmQCST.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.bindingNavigator1);
			this.groupBox1.Controls.Add(this.gvMasterSetting);
			resources.ApplyResources(this.groupBox1, "groupBox1");
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.TabStop = false;
			// 
			// bindingNavigator1
			// 
			this.bindingNavigator1.AddNewItem = this.bindingNavigatorAddNewItem;
			this.bindingNavigator1.BindingSource = this.tvPLMExBindingSource;
			this.bindingNavigator1.CountItem = this.bindingNavigatorCountItem;
			this.bindingNavigator1.DeleteItem = this.bindingNavigatorDeleteItem;
			resources.ApplyResources(this.bindingNavigator1, "bindingNavigator1");
			this.bindingNavigator1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
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
			this.bindingNavigator1.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
			this.bindingNavigator1.MoveLastItem = this.bindingNavigatorMoveLastItem;
			this.bindingNavigator1.MoveNextItem = this.bindingNavigatorMoveNextItem;
			this.bindingNavigator1.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
			this.bindingNavigator1.Name = "bindingNavigator1";
			this.bindingNavigator1.PositionItem = this.bindingNavigatorPositionItem;
			// 
			// bindingNavigatorAddNewItem
			// 
			this.bindingNavigatorAddNewItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.bindingNavigatorAddNewItem, "bindingNavigatorAddNewItem");
			this.bindingNavigatorAddNewItem.Name = "bindingNavigatorAddNewItem";
			// 
			// bindingNavigatorCountItem
			// 
			this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
			resources.ApplyResources(this.bindingNavigatorCountItem, "bindingNavigatorCountItem");
			// 
			// bindingNavigatorDeleteItem
			// 
			this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			resources.ApplyResources(this.bindingNavigatorDeleteItem, "bindingNavigatorDeleteItem");
			this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
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
			// frmMasterSetting
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.label5);
			this.Controls.Add(this.txtType);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.txtDiceCount);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.txtbOutputDate);
			this.Controls.Add(this.btnOutputForm);
			this.Controls.Add(this.btnEntry);
			this.Controls.Add(this.btnReadExcel);
			this.Controls.Add(this.btnOutputExcel);
			this.Controls.Add(this.cmbbType);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.cmbbLineNo);
			this.Controls.Add(this.label23);
			this.Name = "frmMasterSetting";
			this.Load += new System.EventHandler(this.frmMasterSetting_Load);
			((System.ComponentModel.ISupportInitialize)(this.gvMasterSetting)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.tvPLMExBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsTmPLMExBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsTmPLMEx)).EndInit();
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.dsTmQCST)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
			this.bindingNavigator1.ResumeLayout(false);
			this.bindingNavigator1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmbbType;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbbLineNo;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button btnOutputExcel;
        private System.Windows.Forms.Button btnReadExcel;
        private System.Windows.Forms.Button btnEntry;
        private System.Windows.Forms.Button btnOutputForm;
        private System.Windows.Forms.TextBox txtbOutputDate;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView gvMasterSetting;
        private System.Windows.Forms.BindingSource dsTmPLMExBindingSource;
        private dsTmPLMEx dsTmPLMEx;
        private System.Windows.Forms.BindingSource tvPLMExBindingSource;
        private System.Windows.Forms.OpenFileDialog openFileDialog1;
        private dsTmQCST dsTmQCST;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtDiceCount;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtType;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toollblServerhed;
        private System.Windows.Forms.ToolStripStatusLabel toollblServer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toollblEmp;
        private System.Windows.Forms.ToolStripStatusLabel toollblStatusText;
        private System.Windows.Forms.DataGridViewButtonColumn Revision;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col2;
        private System.Windows.Forms.DataGridViewTextBoxColumn col3;
        private System.Windows.Forms.DataGridViewTextBoxColumn col4;
        private System.Windows.Forms.DataGridViewTextBoxColumn col5;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col6;
        private System.Windows.Forms.DataGridViewTextBoxColumn col7;
        private System.Windows.Forms.DataGridViewTextBoxColumn col8;
        private System.Windows.Forms.DataGridViewTextBoxColumn col9;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info1_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info2_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info3_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn col10;
        private System.Windows.Forms.DataGridViewTextBoxColumn col11;
        private System.Windows.Forms.DataGridViewTextBoxColumn col12;
        private System.Windows.Forms.DataGridViewTextBoxColumn col13;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col14;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col15;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col16;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col17;
        private System.Windows.Forms.DataGridViewTextBoxColumn Col18;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dSFGDataGridViewCheckBoxColumn;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.BindingNavigator bindingNavigator1;
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
    }
}