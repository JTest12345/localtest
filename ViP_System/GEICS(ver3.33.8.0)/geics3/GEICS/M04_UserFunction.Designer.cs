namespace GEICS
{
    partial class M04_UserFunction
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(M04_UserFunction));
			this.dgvEmployee = new System.Windows.Forms.DataGridView();
			this.Employee_CD = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.employeeNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dsUserFunctionBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dsUserFunction = new GEICS.dsUserFunction();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.btnSearch = new System.Windows.Forms.Button();
			this.txtEmployeeNM = new System.Windows.Forms.TextBox();
			this.txtEmployeeCD = new System.Windows.Forms.TextBox();
			this.label2 = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.groupBox2 = new System.Windows.Forms.GroupBox();
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
			this.tvFunction = new System.Windows.Forms.TreeView();
			this.tvHoldFunction = new System.Windows.Forms.TreeView();
			this.btnDelete = new System.Windows.Forms.Button();
			this.btnAdd = new System.Windows.Forms.Button();
			this.groupBox3 = new System.Windows.Forms.GroupBox();
			this.groupBox4 = new System.Windows.Forms.GroupBox();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.toollblStatusText = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServerhed = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblServer = new System.Windows.Forms.ToolStripStatusLabel();
			this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
			this.toollblEmp = new System.Windows.Forms.ToolStripStatusLabel();
			((System.ComponentModel.ISupportInitialize)(this.dgvEmployee)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsUserFunctionBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsUserFunction)).BeginInit();
			this.groupBox1.SuspendLayout();
			this.groupBox2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).BeginInit();
			this.bindingNavigator1.SuspendLayout();
			this.groupBox3.SuspendLayout();
			this.groupBox4.SuspendLayout();
			this.statusStrip.SuspendLayout();
			this.SuspendLayout();
			// 
			// dgvEmployee
			// 
			this.dgvEmployee.AllowUserToAddRows = false;
			this.dgvEmployee.AllowUserToResizeRows = false;
			resources.ApplyResources(this.dgvEmployee, "dgvEmployee");
			this.dgvEmployee.AutoGenerateColumns = false;
			this.dgvEmployee.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvEmployee.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Employee_CD,
            this.employeeNMDataGridViewTextBoxColumn});
			this.dgvEmployee.DataSource = this.dsUserFunctionBindingSource;
			this.dgvEmployee.MultiSelect = false;
			this.dgvEmployee.Name = "dgvEmployee";
			this.dgvEmployee.ReadOnly = true;
			this.dgvEmployee.RowHeadersVisible = false;
			this.dgvEmployee.RowTemplate.Height = 21;
			this.dgvEmployee.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.dgvEmployee.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvEmployee_CellDoubleClick);
			this.dgvEmployee.SelectionChanged += new System.EventHandler(this.dgvEmployee_SelectionChanged);
			// 
			// Employee_CD
			// 
			this.Employee_CD.DataPropertyName = "Employee_CD";
			resources.ApplyResources(this.Employee_CD, "Employee_CD");
			this.Employee_CD.Name = "Employee_CD";
			this.Employee_CD.ReadOnly = true;
			// 
			// employeeNMDataGridViewTextBoxColumn
			// 
			this.employeeNMDataGridViewTextBoxColumn.DataPropertyName = "Employee_NM";
			resources.ApplyResources(this.employeeNMDataGridViewTextBoxColumn, "employeeNMDataGridViewTextBoxColumn");
			this.employeeNMDataGridViewTextBoxColumn.Name = "employeeNMDataGridViewTextBoxColumn";
			this.employeeNMDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// dsUserFunctionBindingSource
			// 
			this.dsUserFunctionBindingSource.DataMember = "dtEmployee";
			this.dsUserFunctionBindingSource.DataSource = this.dsUserFunction;
			// 
			// dsUserFunction
			// 
			this.dsUserFunction.DataSetName = "dsUserFunction";
			this.dsUserFunction.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// groupBox1
			// 
			this.groupBox1.Controls.Add(this.btnSearch);
			this.groupBox1.Controls.Add(this.txtEmployeeNM);
			this.groupBox1.Controls.Add(this.txtEmployeeCD);
			this.groupBox1.Controls.Add(this.label2);
			this.groupBox1.Controls.Add(this.label1);
			resources.ApplyResources(this.groupBox1, "groupBox1");
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
			// txtEmployeeNM
			// 
			resources.ApplyResources(this.txtEmployeeNM, "txtEmployeeNM");
			this.txtEmployeeNM.Name = "txtEmployeeNM";
			// 
			// txtEmployeeCD
			// 
			resources.ApplyResources(this.txtEmployeeCD, "txtEmployeeCD");
			this.txtEmployeeCD.Name = "txtEmployeeCD";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// groupBox2
			// 
			resources.ApplyResources(this.groupBox2, "groupBox2");
			this.groupBox2.Controls.Add(this.bindingNavigator1);
			this.groupBox2.Controls.Add(this.dgvEmployee);
			this.groupBox2.FlatStyle = System.Windows.Forms.FlatStyle.System;
			this.groupBox2.Name = "groupBox2";
			this.groupBox2.TabStop = false;
			// 
			// bindingNavigator1
			// 
			this.bindingNavigator1.AddNewItem = this.bindingNavigatorAddNewItem;
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
			// tvFunction
			// 
			resources.ApplyResources(this.tvFunction, "tvFunction");
			this.tvFunction.Name = "tvFunction";
			this.tvFunction.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvFunction_AfterSelect);
			// 
			// tvHoldFunction
			// 
			resources.ApplyResources(this.tvHoldFunction, "tvHoldFunction");
			this.tvHoldFunction.Name = "tvHoldFunction";
			this.tvHoldFunction.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.tvHoldFunction_AfterSelect);
			// 
			// btnDelete
			// 
			resources.ApplyResources(this.btnDelete, "btnDelete");
			this.btnDelete.Name = "btnDelete";
			this.btnDelete.UseVisualStyleBackColor = true;
			this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
			// 
			// btnAdd
			// 
			resources.ApplyResources(this.btnAdd, "btnAdd");
			this.btnAdd.Name = "btnAdd";
			this.btnAdd.UseVisualStyleBackColor = true;
			this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
			// 
			// groupBox3
			// 
			resources.ApplyResources(this.groupBox3, "groupBox3");
			this.groupBox3.Controls.Add(this.tvFunction);
			this.groupBox3.Name = "groupBox3";
			this.groupBox3.TabStop = false;
			// 
			// groupBox4
			// 
			resources.ApplyResources(this.groupBox4, "groupBox4");
			this.groupBox4.Controls.Add(this.tvHoldFunction);
			this.groupBox4.Name = "groupBox4";
			this.groupBox4.TabStop = false;
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
			// M04_UserFunction
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.statusStrip);
			this.Controls.Add(this.groupBox4);
			this.Controls.Add(this.groupBox3);
			this.Controls.Add(this.btnAdd);
			this.Controls.Add(this.btnDelete);
			this.Controls.Add(this.groupBox2);
			this.Controls.Add(this.groupBox1);
			this.MaximizeBox = false;
			this.Name = "M04_UserFunction";
			this.Load += new System.EventHandler(this.frmUserFunction_Load);
			((System.ComponentModel.ISupportInitialize)(this.dgvEmployee)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsUserFunctionBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsUserFunction)).EndInit();
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.groupBox2.ResumeLayout(false);
			this.groupBox2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.bindingNavigator1)).EndInit();
			this.bindingNavigator1.ResumeLayout(false);
			this.bindingNavigator1.PerformLayout();
			this.groupBox3.ResumeLayout(false);
			this.groupBox4.ResumeLayout(false);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvEmployee;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.TextBox txtEmployeeNM;
        private System.Windows.Forms.TextBox txtEmployeeCD;
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
        private System.Windows.Forms.TreeView tvFunction;
        private System.Windows.Forms.TreeView tvHoldFunction;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.BindingSource dsUserFunctionBindingSource;
        private dsUserFunction dsUserFunction;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toollblStatusText;
        private System.Windows.Forms.ToolStripStatusLabel toollblServerhed;
        private System.Windows.Forms.ToolStripStatusLabel toollblServer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toollblEmp;
        private System.Windows.Forms.DataGridViewTextBoxColumn Employee_CD;
        private System.Windows.Forms.DataGridViewTextBoxColumn employeeNMDataGridViewTextBoxColumn;
    }
}