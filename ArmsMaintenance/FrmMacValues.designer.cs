namespace ArmsMaintenance
{
    partial class FrmMacValues
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMacValues));
            this.grdValues = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnMac11 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac10 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac9 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac8 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac7 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac6 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac5 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac4 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac3 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac2 = new ArmsMaintenance.NotSelectableButton();
            this.btnMac1 = new ArmsMaintenance.NotSelectableButton();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMacNo = new System.Windows.Forms.TextBox();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.dtpWorkEnd = new System.Windows.Forms.DateTimePicker();
            this.btnSelectMachine = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dtpWorkStart = new System.Windows.Forms.DateTimePicker();
            this.btnDeleteAllWafers = new System.Windows.Forms.Button();
            this.btnDeleteMat = new System.Windows.Forms.Button();
            this.btnEdit = new System.Windows.Forms.Button();
            this.btnAddNew = new System.Windows.Forms.Button();
            this.btnAddWafer = new System.Windows.Forms.Button();
            this.btnMachineSelect = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdValues)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdValues
            // 
            this.grdValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdValues.Location = new System.Drawing.Point(16, 166);
            this.grdValues.Name = "grdValues";
            this.grdValues.ReadOnly = true;
            this.grdValues.RowTemplate.Height = 21;
            this.grdValues.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdValues.Size = new System.Drawing.Size(916, 319);
            this.grdValues.TabIndex = 44;
            this.grdValues.SelectionChanged += new System.EventHandler(this.grdMaterials_SelectionChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnMachineSelect);
            this.groupBox1.Controls.Add(this.btnMac11);
            this.groupBox1.Controls.Add(this.btnMac10);
            this.groupBox1.Controls.Add(this.btnMac9);
            this.groupBox1.Controls.Add(this.btnMac8);
            this.groupBox1.Controls.Add(this.btnMac7);
            this.groupBox1.Controls.Add(this.btnMac6);
            this.groupBox1.Controls.Add(this.btnMac5);
            this.groupBox1.Controls.Add(this.btnMac4);
            this.groupBox1.Controls.Add(this.btnMac3);
            this.groupBox1.Controls.Add(this.btnMac2);
            this.groupBox1.Controls.Add(this.btnMac1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtMacNo);
            this.groupBox1.Controls.Add(this.txtMachine);
            this.groupBox1.Controls.Add(this.dtpWorkEnd);
            this.groupBox1.Controls.Add(this.btnSelectMachine);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.dtpWorkStart);
            this.groupBox1.Location = new System.Drawing.Point(16, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(916, 148);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // btnMac11
            // 
            this.btnMac11.Location = new System.Drawing.Point(817, 97);
            this.btnMac11.Name = "btnMac11";
            this.btnMac11.Size = new System.Drawing.Size(91, 32);
            this.btnMac11.TabIndex = 65;
            this.btnMac11.Text = "button12";
            this.btnMac11.UseVisualStyleBackColor = true;
            this.btnMac11.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac10
            // 
            this.btnMac10.Location = new System.Drawing.Point(769, 59);
            this.btnMac10.Name = "btnMac10";
            this.btnMac10.Size = new System.Drawing.Size(91, 32);
            this.btnMac10.TabIndex = 64;
            this.btnMac10.Text = "button12";
            this.btnMac10.UseVisualStyleBackColor = true;
            this.btnMac10.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac9
            // 
            this.btnMac9.Location = new System.Drawing.Point(720, 97);
            this.btnMac9.Name = "btnMac9";
            this.btnMac9.Size = new System.Drawing.Size(91, 32);
            this.btnMac9.TabIndex = 63;
            this.btnMac9.Text = "button12";
            this.btnMac9.UseVisualStyleBackColor = true;
            this.btnMac9.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac8
            // 
            this.btnMac8.Location = new System.Drawing.Point(623, 97);
            this.btnMac8.Name = "btnMac8";
            this.btnMac8.Size = new System.Drawing.Size(91, 32);
            this.btnMac8.TabIndex = 62;
            this.btnMac8.Text = "button12";
            this.btnMac8.UseVisualStyleBackColor = true;
            this.btnMac8.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac7
            // 
            this.btnMac7.Location = new System.Drawing.Point(526, 97);
            this.btnMac7.Name = "btnMac7";
            this.btnMac7.Size = new System.Drawing.Size(91, 32);
            this.btnMac7.TabIndex = 61;
            this.btnMac7.Text = "button13";
            this.btnMac7.UseVisualStyleBackColor = true;
            this.btnMac7.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac6
            // 
            this.btnMac6.Location = new System.Drawing.Point(672, 59);
            this.btnMac6.Name = "btnMac6";
            this.btnMac6.Size = new System.Drawing.Size(91, 32);
            this.btnMac6.TabIndex = 60;
            this.btnMac6.Text = "button12";
            this.btnMac6.UseVisualStyleBackColor = true;
            this.btnMac6.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac5
            // 
            this.btnMac5.Location = new System.Drawing.Point(720, 22);
            this.btnMac5.Name = "btnMac5";
            this.btnMac5.Size = new System.Drawing.Size(91, 32);
            this.btnMac5.TabIndex = 59;
            this.btnMac5.Text = "button12";
            this.btnMac5.UseVisualStyleBackColor = true;
            this.btnMac5.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac4
            // 
            this.btnMac4.Location = new System.Drawing.Point(575, 59);
            this.btnMac4.Name = "btnMac4";
            this.btnMac4.Size = new System.Drawing.Size(91, 32);
            this.btnMac4.TabIndex = 58;
            this.btnMac4.Text = "button12";
            this.btnMac4.UseVisualStyleBackColor = true;
            this.btnMac4.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac3
            // 
            this.btnMac3.Location = new System.Drawing.Point(623, 22);
            this.btnMac3.Name = "btnMac3";
            this.btnMac3.Size = new System.Drawing.Size(91, 32);
            this.btnMac3.TabIndex = 57;
            this.btnMac3.Text = "button12";
            this.btnMac3.UseVisualStyleBackColor = true;
            this.btnMac3.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac2
            // 
            this.btnMac2.Location = new System.Drawing.Point(478, 59);
            this.btnMac2.Name = "btnMac2";
            this.btnMac2.Size = new System.Drawing.Size(91, 32);
            this.btnMac2.TabIndex = 56;
            this.btnMac2.Text = "button13";
            this.btnMac2.UseVisualStyleBackColor = true;
            this.btnMac2.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnMac1
            // 
            this.btnMac1.Location = new System.Drawing.Point(526, 22);
            this.btnMac1.Name = "btnMac1";
            this.btnMac1.Size = new System.Drawing.Size(91, 32);
            this.btnMac1.TabIndex = 55;
            this.btnMac1.Text = "button12";
            this.btnMac1.UseVisualStyleBackColor = true;
            this.btnMac1.Click += new System.EventHandler(this.btnMac_Click);
            // 
            // btnSearch
            // 
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.Location = new System.Drawing.Point(385, 105);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 27);
            this.btnSearch.TabIndex = 40;
            this.btnSearch.Text = "検索";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 35);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "作業号機";
            // 
            // txtMacNo
            // 
            this.txtMacNo.Location = new System.Drawing.Point(69, 32);
            this.txtMacNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMacNo.Name = "txtMacNo";
            this.txtMacNo.ReadOnly = true;
            this.txtMacNo.Size = new System.Drawing.Size(103, 22);
            this.txtMacNo.TabIndex = 39;
            // 
            // txtMachine
            // 
            this.txtMachine.Location = new System.Drawing.Point(178, 32);
            this.txtMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(215, 22);
            this.txtMachine.TabIndex = 33;
            // 
            // dtpWorkEnd
            // 
            this.dtpWorkEnd.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkEnd.Location = new System.Drawing.Point(280, 62);
            this.dtpWorkEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkEnd.Name = "dtpWorkEnd";
            this.dtpWorkEnd.Size = new System.Drawing.Size(180, 22);
            this.dtpWorkEnd.TabIndex = 38;
            // 
            // btnSelectMachine
            // 
            this.btnSelectMachine.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectMachine.Image = global::ArmsMaintenance.Properties.Resources.viewmag;
            this.btnSelectMachine.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectMachine.Location = new System.Drawing.Point(396, 30);
            this.btnSelectMachine.Margin = new System.Windows.Forms.Padding(0);
            this.btnSelectMachine.Name = "btnSelectMachine";
            this.btnSelectMachine.Size = new System.Drawing.Size(69, 27);
            this.btnSelectMachine.TabIndex = 34;
            this.btnSelectMachine.Text = "選択";
            this.btnSelectMachine.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectMachine.UseVisualStyleBackColor = true;
            this.btnSelectMachine.Click += new System.EventHandler(this.btnSelectMachine_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(255, 67);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(19, 14);
            this.label5.TabIndex = 37;
            this.label5.Text = "～";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 67);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 14);
            this.label4.TabIndex = 35;
            this.label4.Text = "開始時間";
            // 
            // dtpWorkStart
            // 
            this.dtpWorkStart.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkStart.Location = new System.Drawing.Point(69, 62);
            this.dtpWorkStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkStart.Name = "dtpWorkStart";
            this.dtpWorkStart.Size = new System.Drawing.Size(180, 22);
            this.dtpWorkStart.TabIndex = 36;
            // 
            // btnDeleteAllWafers
            // 
            this.btnDeleteAllWafers.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteAllWafers.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteAllWafers.Image = global::ArmsMaintenance.Properties.Resources.delete;
            this.btnDeleteAllWafers.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteAllWafers.Location = new System.Drawing.Point(331, 488);
            this.btnDeleteAllWafers.Margin = new System.Windows.Forms.Padding(0);
            this.btnDeleteAllWafers.Name = "btnDeleteAllWafers";
            this.btnDeleteAllWafers.Size = new System.Drawing.Size(151, 31);
            this.btnDeleteAllWafers.TabIndex = 47;
            this.btnDeleteAllWafers.Text = "全カセット割付解除";
            this.btnDeleteAllWafers.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeleteAllWafers.UseVisualStyleBackColor = true;
            this.btnDeleteAllWafers.Click += new System.EventHandler(this.btnDeleteAllWafers_Click);
            // 
            // btnDeleteMat
            // 
            this.btnDeleteMat.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnDeleteMat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDeleteMat.Image = global::ArmsMaintenance.Properties.Resources.delete;
            this.btnDeleteMat.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDeleteMat.Location = new System.Drawing.Point(619, 488);
            this.btnDeleteMat.Margin = new System.Windows.Forms.Padding(0);
            this.btnDeleteMat.Name = "btnDeleteMat";
            this.btnDeleteMat.Size = new System.Drawing.Size(102, 31);
            this.btnDeleteMat.TabIndex = 46;
            this.btnDeleteMat.Text = "割付解除";
            this.btnDeleteMat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDeleteMat.UseVisualStyleBackColor = true;
            this.btnDeleteMat.Click += new System.EventHandler(this.btnDeleteMat_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEdit.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnEdit.Image = global::ArmsMaintenance.Properties.Resources.edit;
            this.btnEdit.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnEdit.Location = new System.Drawing.Point(824, 488);
            this.btnEdit.Margin = new System.Windows.Forms.Padding(0);
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(108, 31);
            this.btnEdit.TabIndex = 45;
            this.btnEdit.Text = "編集";
            this.btnEdit.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnEdit.UseVisualStyleBackColor = true;
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // btnAddNew
            // 
            this.btnAddNew.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddNew.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddNew.Image = ((System.Drawing.Image)(resources.GetObject("btnAddNew.Image")));
            this.btnAddNew.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddNew.Location = new System.Drawing.Point(724, 488);
            this.btnAddNew.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddNew.Name = "btnAddNew";
            this.btnAddNew.Size = new System.Drawing.Size(98, 31);
            this.btnAddNew.TabIndex = 41;
            this.btnAddNew.Text = "新規追加";
            this.btnAddNew.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddNew.UseVisualStyleBackColor = true;
            this.btnAddNew.Click += new System.EventHandler(this.btnAddNew_Click);
            // 
            // btnAddWafer
            // 
            this.btnAddWafer.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnAddWafer.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddWafer.Image = ((System.Drawing.Image)(resources.GetObject("btnAddWafer.Image")));
            this.btnAddWafer.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddWafer.Location = new System.Drawing.Point(485, 488);
            this.btnAddWafer.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddWafer.Name = "btnAddWafer";
            this.btnAddWafer.Size = new System.Drawing.Size(130, 31);
            this.btnAddWafer.TabIndex = 48;
            this.btnAddWafer.Text = "カセット新規追加";
            this.btnAddWafer.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddWafer.UseVisualStyleBackColor = true;
            this.btnAddWafer.Click += new System.EventHandler(this.btnAddWafer_Click);
            // 
            // btnMachineSelect
            // 
            this.btnMachineSelect.Location = new System.Drawing.Point(551, 25);
            this.btnMachineSelect.Name = "btnMachineSelect";
            this.btnMachineSelect.Size = new System.Drawing.Size(115, 37);
            this.btnMachineSelect.TabIndex = 70;
            this.btnMachineSelect.Text = "装置選択";
            this.btnMachineSelect.UseVisualStyleBackColor = true;
            this.btnMachineSelect.Click += new System.EventHandler(this.btnMachineSelect_Click);
            // 
            // FrmMacValues
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(948, 528);
            this.Controls.Add(this.btnAddWafer);
            this.Controls.Add(this.btnDeleteAllWafers);
            this.Controls.Add(this.btnDeleteMat);
            this.Controls.Add(this.btnEdit);
            this.Controls.Add(this.btnAddNew);
            this.Controls.Add(this.grdValues);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmMacValues";
            this.Text = "FrmMacValues";
            this.Load += new System.EventHandler(this.FrmMacValues_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdValues)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdValues;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMacNo;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.DateTimePicker dtpWorkEnd;
        private System.Windows.Forms.Button btnSelectMachine;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.DateTimePicker dtpWorkStart;
        private System.Windows.Forms.Button btnAddNew;
        private System.Windows.Forms.Button btnEdit;
        private NotSelectableButton btnMac2;
        private NotSelectableButton btnMac1;
        private System.Windows.Forms.Button btnDeleteMat;
        private System.Windows.Forms.Button btnDeleteAllWafers;
        private System.Windows.Forms.Button btnAddWafer;
        private NotSelectableButton btnMac6;
        private NotSelectableButton btnMac5;
        private NotSelectableButton btnMac4;
        private NotSelectableButton btnMac3;
		private NotSelectableButton btnMac9;
		private NotSelectableButton btnMac8;
		private NotSelectableButton btnMac7;
		private NotSelectableButton btnMac11;
		private NotSelectableButton btnMac10;
        private System.Windows.Forms.Button btnMachineSelect;
    }
}