namespace ArmsMaintenance
{
    partial class FrmRestrict
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmRestrict));
            this.btnDelete = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.chkProc = new System.Windows.Forms.CheckBox();
            this.chkOnlyActive = new System.Windows.Forms.CheckBox();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.cmbProcess = new System.Windows.Forms.ComboBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.grdValues = new System.Windows.Forms.DataGridView();
            this.ColChk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdValues)).BeginInit();
            this.SuspendLayout();
            // 
            // btnDelete
            // 
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnDelete.Image = global::ArmsMaintenance.Properties.Resources.delete;
            this.btnDelete.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnDelete.Location = new System.Drawing.Point(625, 47);
            this.btnDelete.Margin = new System.Windows.Forms.Padding(0);
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.Size = new System.Drawing.Size(113, 31);
            this.btnDelete.TabIndex = 49;
            this.btnDelete.Text = "解除";
            this.btnDelete.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkProc);
            this.groupBox1.Controls.Add(this.chkOnlyActive);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.cmbProcess);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(472, 78);
            this.groupBox1.TabIndex = 47;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // chkProc
            // 
            this.chkProc.AutoSize = true;
            this.chkProc.Location = new System.Drawing.Point(6, 46);
            this.chkProc.Name = "chkProc";
            this.chkProc.Size = new System.Drawing.Size(98, 18);
            this.chkProc.TabIndex = 44;
            this.chkProc.Text = "投入規制工程";
            this.chkProc.UseVisualStyleBackColor = true;
            // 
            // chkOnlyActive
            // 
            this.chkOnlyActive.AutoSize = true;
            this.chkOnlyActive.Checked = true;
            this.chkOnlyActive.CheckState = System.Windows.Forms.CheckState.Checked;
            this.chkOnlyActive.Location = new System.Drawing.Point(330, 18);
            this.chkOnlyActive.Name = "chkOnlyActive";
            this.chkOnlyActive.Size = new System.Drawing.Size(107, 18);
            this.chkOnlyActive.TabIndex = 43;
            this.chkOnlyActive.Text = "規制中のみ表示";
            this.chkOnlyActive.UseVisualStyleBackColor = true;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(110, 16);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(214, 22);
            this.txtLotNo.TabIndex = 42;
            // 
            // cmbProcess
            // 
            this.cmbProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbProcess.FormattingEnabled = true;
            this.cmbProcess.Location = new System.Drawing.Point(110, 44);
            this.cmbProcess.Name = "cmbProcess";
            this.cmbProcess.Size = new System.Drawing.Size(234, 22);
            this.cmbProcess.TabIndex = 41;
            // 
            // btnSearch
            // 
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.Location = new System.Drawing.Point(374, 41);
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
            this.label3.Location = new System.Drawing.Point(64, 19);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(40, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "LotNo";
            // 
            // grdValues
            // 
            this.grdValues.AllowUserToAddRows = false;
            this.grdValues.AllowUserToDeleteRows = false;
            this.grdValues.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdValues.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdValues.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdValues.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColChk});
            this.grdValues.Location = new System.Drawing.Point(12, 96);
            this.grdValues.Name = "grdValues";
            this.grdValues.RowTemplate.Height = 21;
            this.grdValues.Size = new System.Drawing.Size(726, 458);
            this.grdValues.TabIndex = 68;
            // 
            // ColChk
            // 
            this.ColChk.Frozen = true;
            this.ColChk.HeaderText = "選択";
            this.ColChk.Name = "ColChk";
            this.ColChk.Width = 37;
            // 
            // FrmRestrict
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(750, 566);
            this.Controls.Add(this.grdValues);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmRestrict";
            this.Text = "流動規制管理";
            this.Load += new System.EventHandler(this.FrmRestrict_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdValues)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.ComboBox cmbProcess;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkOnlyActive;
        private System.Windows.Forms.DataGridView grdValues;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColChk;
        private System.Windows.Forms.CheckBox chkProc;
    }
}