namespace ArmsMaintenance
{
    partial class FrmCutBlendWorkView
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
            this.grdWork = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label2 = new System.Windows.Forms.Label();
            this.chkWorkEnd = new System.Windows.Forms.CheckBox();
            this.dtpWorkEndTo = new System.Windows.Forms.DateTimePicker();
            this.dtpWorkEndFrom = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.chkWorkStart = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dtpWorkStartTo = new System.Windows.Forms.DateTimePicker();
            this.dtpWorkStartFrom = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.txtMacNo = new System.Windows.Forms.TextBox();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.btnSelectMachine = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdWork)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdWork
            // 
            this.grdWork.AllowUserToAddRows = false;
            this.grdWork.AllowUserToDeleteRows = false;
            this.grdWork.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdWork.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdWork.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdWork.Location = new System.Drawing.Point(13, 139);
            this.grdWork.Name = "grdWork";
            this.grdWork.ReadOnly = true;
            this.grdWork.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.grdWork.RowTemplate.Height = 21;
            this.grdWork.Size = new System.Drawing.Size(917, 330);
            this.grdWork.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkWorkEnd);
            this.groupBox1.Controls.Add(this.dtpWorkEndTo);
            this.groupBox1.Controls.Add(this.dtpWorkEndFrom);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.chkWorkStart);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.dtpWorkStartTo);
            this.groupBox1.Controls.Add(this.dtpWorkStartFrom);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtMacNo);
            this.groupBox1.Controls.Add(this.txtMachine);
            this.groupBox1.Controls.Add(this.btnSelectMachine);
            this.groupBox1.Location = new System.Drawing.Point(13, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(597, 109);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(267, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(10, 12);
            this.label2.TabIndex = 41;
            this.label2.Text = "~";
            // 
            // chkWorkEnd
            // 
            this.chkWorkEnd.AutoSize = true;
            this.chkWorkEnd.Location = new System.Drawing.Point(13, 80);
            this.chkWorkEnd.Name = "chkWorkEnd";
            this.chkWorkEnd.Size = new System.Drawing.Size(72, 16);
            this.chkWorkEnd.TabIndex = 42;
            this.chkWorkEnd.Text = "完了時間";
            this.chkWorkEnd.UseVisualStyleBackColor = true;
            this.chkWorkEnd.CheckedChanged += new System.EventHandler(this.chkWorkEnd_CheckedChanged);
            // 
            // dtpWorkEndTo
            // 
            this.dtpWorkEndTo.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkEndTo.Enabled = false;
            this.dtpWorkEndTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkEndTo.Location = new System.Drawing.Point(289, 77);
            this.dtpWorkEndTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkEndTo.Name = "dtpWorkEndTo";
            this.dtpWorkEndTo.Size = new System.Drawing.Size(169, 19);
            this.dtpWorkEndTo.TabIndex = 43;
            // 
            // dtpWorkEndFrom
            // 
            this.dtpWorkEndFrom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkEndFrom.Enabled = false;
            this.dtpWorkEndFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkEndFrom.Location = new System.Drawing.Point(92, 77);
            this.dtpWorkEndFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkEndFrom.Name = "dtpWorkEndFrom";
            this.dtpWorkEndFrom.Size = new System.Drawing.Size(169, 19);
            this.dtpWorkEndFrom.TabIndex = 40;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(267, 55);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(10, 12);
            this.label1.TabIndex = 38;
            this.label1.Text = "~";
            // 
            // chkWorkStart
            // 
            this.chkWorkStart.AutoSize = true;
            this.chkWorkStart.Checked = true;
            this.chkWorkStart.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkWorkStart.Location = new System.Drawing.Point(13, 54);
            this.chkWorkStart.Name = "chkWorkStart";
            this.chkWorkStart.Size = new System.Drawing.Size(72, 16);
            this.chkWorkStart.TabIndex = 38;
            this.chkWorkStart.Text = "開始時間";
            this.chkWorkStart.UseVisualStyleBackColor = true;
            this.chkWorkStart.CheckedChanged += new System.EventHandler(this.chkWorkStart_CheckedChanged);
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(483, 75);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(96, 27);
            this.btnSearch.TabIndex = 38;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dtpWorkStartTo
            // 
            this.dtpWorkStartTo.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkStartTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkStartTo.Location = new System.Drawing.Point(289, 51);
            this.dtpWorkStartTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkStartTo.Name = "dtpWorkStartTo";
            this.dtpWorkStartTo.Size = new System.Drawing.Size(169, 19);
            this.dtpWorkStartTo.TabIndex = 39;
            // 
            // dtpWorkStartFrom
            // 
            this.dtpWorkStartFrom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkStartFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkStartFrom.Location = new System.Drawing.Point(92, 51);
            this.dtpWorkStartFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkStartFrom.Name = "dtpWorkStartFrom";
            this.dtpWorkStartFrom.Size = new System.Drawing.Size(169, 19);
            this.dtpWorkStartFrom.TabIndex = 37;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 32;
            this.label3.Text = "作業号機";
            // 
            // txtMacNo
            // 
            this.txtMacNo.Location = new System.Drawing.Point(71, 20);
            this.txtMacNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMacNo.Name = "txtMacNo";
            this.txtMacNo.ReadOnly = true;
            this.txtMacNo.Size = new System.Drawing.Size(89, 19);
            this.txtMacNo.TabIndex = 35;
            // 
            // txtMachine
            // 
            this.txtMachine.Location = new System.Drawing.Point(167, 20);
            this.txtMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(185, 19);
            this.txtMachine.TabIndex = 33;
            // 
            // btnSelectMachine
            // 
            this.btnSelectMachine.Location = new System.Drawing.Point(358, 17);
            this.btnSelectMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectMachine.Name = "btnSelectMachine";
            this.btnSelectMachine.Size = new System.Drawing.Size(59, 27);
            this.btnSelectMachine.TabIndex = 34;
            this.btnSelectMachine.Text = "選択";
            this.btnSelectMachine.UseVisualStyleBackColor = true;
            this.btnSelectMachine.Click += new System.EventHandler(this.btnSelectMachine_Click);
            // 
            // FrmCutBlendWorkView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 481);
            this.Controls.Add(this.grdWork);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmCutBlendWorkView";
            this.Text = "カット工程作業実績照会";
            ((System.ComponentModel.ISupportInitialize)(this.grdWork)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdWork;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkWorkEnd;
        private System.Windows.Forms.DateTimePicker dtpWorkEndTo;
        private System.Windows.Forms.DateTimePicker dtpWorkEndFrom;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.CheckBox chkWorkStart;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DateTimePicker dtpWorkStartTo;
        private System.Windows.Forms.DateTimePicker dtpWorkStartFrom;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMacNo;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.Button btnSelectMachine;
    }
}