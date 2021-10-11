namespace ArmsMaintenance
{
    partial class FrmHistoryView
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmHistoryView));
            this.grdHistory = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rdoEditDt = new System.Windows.Forms.RadioButton();
            this.rdoLotNo = new System.Windows.Forms.RadioButton();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dtpEditTo = new System.Windows.Forms.DateTimePicker();
            this.dtpEditFrom = new System.Windows.Forms.DateTimePicker();
            this.btnCheckOk = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdHistory)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdHistory
            // 
            this.grdHistory.AllowUserToAddRows = false;
            this.grdHistory.AllowUserToDeleteRows = false;
            this.grdHistory.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdHistory.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdHistory.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdHistory.Location = new System.Drawing.Point(13, 119);
            this.grdHistory.Name = "grdHistory";
            this.grdHistory.ReadOnly = true;
            this.grdHistory.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.grdHistory.RowTemplate.Height = 21;
            this.grdHistory.Size = new System.Drawing.Size(917, 351);
            this.grdHistory.TabIndex = 39;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rdoEditDt);
            this.groupBox1.Controls.Add(this.rdoLotNo);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.dtpEditTo);
            this.groupBox1.Controls.Add(this.dtpEditFrom);
            this.groupBox1.Location = new System.Drawing.Point(13, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(597, 100);
            this.groupBox1.TabIndex = 38;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // rdoEditDt
            // 
            this.rdoEditDt.AutoSize = true;
            this.rdoEditDt.Location = new System.Drawing.Point(19, 60);
            this.rdoEditDt.Name = "rdoEditDt";
            this.rdoEditDt.Size = new System.Drawing.Size(73, 18);
            this.rdoEditDt.TabIndex = 43;
            this.rdoEditDt.Text = "更新日時";
            this.rdoEditDt.UseVisualStyleBackColor = true;
            // 
            // rdoLotNo
            // 
            this.rdoLotNo.AutoSize = true;
            this.rdoLotNo.Checked = true;
            this.rdoLotNo.Location = new System.Drawing.Point(19, 28);
            this.rdoLotNo.Name = "rdoLotNo";
            this.rdoLotNo.Size = new System.Drawing.Size(58, 18);
            this.rdoLotNo.TabIndex = 43;
            this.rdoLotNo.TabStop = true;
            this.rdoLotNo.Text = "LotNo";
            this.rdoLotNo.UseVisualStyleBackColor = true;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(98, 27);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(263, 22);
            this.txtLotNo.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(273, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(16, 14);
            this.label1.TabIndex = 38;
            this.label1.Text = "~";
            // 
            // btnSearch
            // 
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.Location = new System.Drawing.Point(495, 58);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(89, 27);
            this.btnSearch.TabIndex = 38;
            this.btnSearch.Text = "検索";
            this.btnSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dtpEditTo
            // 
            this.dtpEditTo.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpEditTo.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEditTo.Location = new System.Drawing.Point(295, 58);
            this.dtpEditTo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpEditTo.Name = "dtpEditTo";
            this.dtpEditTo.Size = new System.Drawing.Size(169, 22);
            this.dtpEditTo.TabIndex = 39;
            // 
            // dtpEditFrom
            // 
            this.dtpEditFrom.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpEditFrom.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEditFrom.Location = new System.Drawing.Point(98, 58);
            this.dtpEditFrom.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpEditFrom.Name = "dtpEditFrom";
            this.dtpEditFrom.Size = new System.Drawing.Size(169, 22);
            this.dtpEditFrom.TabIndex = 37;
            // 
            // btnCheckOk
            // 
            this.btnCheckOk.Enabled = false;
            this.btnCheckOk.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnCheckOk.Image = ((System.Drawing.Image)(resources.GetObject("btnCheckOk.Image")));
            this.btnCheckOk.Location = new System.Drawing.Point(792, 87);
            this.btnCheckOk.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnCheckOk.Name = "btnCheckOk";
            this.btnCheckOk.Size = new System.Drawing.Size(138, 27);
            this.btnCheckOk.TabIndex = 40;
            this.btnCheckOk.Text = "確認済みチェック";
            this.btnCheckOk.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnCheckOk.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnCheckOk.UseVisualStyleBackColor = true;
            this.btnCheckOk.Click += new System.EventHandler(this.btnCheckOk_Click);
            // 
            // FrmHistoryView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(943, 483);
            this.Controls.Add(this.btnCheckOk);
            this.Controls.Add(this.grdHistory);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmHistoryView";
            this.Text = "データ修正履歴照会";
            this.Load += new System.EventHandler(this.FrmHistoryView_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdHistory)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdHistory;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DateTimePicker dtpEditTo;
        private System.Windows.Forms.DateTimePicker dtpEditFrom;
        private System.Windows.Forms.RadioButton rdoLotNo;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.RadioButton rdoEditDt;
        private System.Windows.Forms.Button btnCheckOk;
    }
}