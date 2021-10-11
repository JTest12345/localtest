namespace ArmsMaintenance
{
    partial class FrmLotTrace
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLotTrace));
            this.rdoLotSearch = new System.Windows.Forms.RadioButton();
            this.grdLotList = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtAsmLotNo = new System.Windows.Forms.TextBox();
            this.rdoAsmLot = new System.Windows.Forms.RadioButton();
            this.btnSelectAsmLot = new System.Windows.Forms.Button();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnSelectMat = new System.Windows.Forms.Button();
            this.rdoResinSearch = new System.Windows.Forms.RadioButton();
            this.btnSearch = new System.Windows.Forms.Button();
            this.txtMixResultId = new System.Windows.Forms.TextBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.ckbRestrictRelease = new System.Windows.Forms.CheckBox();
            this.txtRestrictEmp = new System.Windows.Forms.TextBox();
            this.cmbRestrictProc = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.btnAddRestrict = new System.Windows.Forms.Button();
            this.txtRestrictNote = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdLotList)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // rdoLotSearch
            // 
            this.rdoLotSearch.AutoSize = true;
            this.rdoLotSearch.Checked = true;
            this.rdoLotSearch.Location = new System.Drawing.Point(6, 53);
            this.rdoLotSearch.Name = "rdoLotSearch";
            this.rdoLotSearch.Size = new System.Drawing.Size(73, 18);
            this.rdoLotSearch.TabIndex = 18;
            this.rdoLotSearch.TabStop = true;
            this.rdoLotSearch.Text = "資材ロット";
            this.rdoLotSearch.UseVisualStyleBackColor = true;
            // 
            // grdLotList
            // 
            this.grdLotList.AllowUserToAddRows = false;
            this.grdLotList.AllowUserToDeleteRows = false;
            this.grdLotList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdLotList.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdLotList.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.grdLotList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdLotList.Location = new System.Drawing.Point(10, 130);
            this.grdLotList.Name = "grdLotList";
            this.grdLotList.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.grdLotList.RowTemplate.Height = 21;
            this.grdLotList.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.CellSelect;
            this.grdLotList.Size = new System.Drawing.Size(555, 308);
            this.grdLotList.TabIndex = 16;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtAsmLotNo);
            this.groupBox1.Controls.Add(this.rdoAsmLot);
            this.groupBox1.Controls.Add(this.btnSelectAsmLot);
            this.groupBox1.Controls.Add(this.btnSelectMat);
            this.groupBox1.Controls.Add(this.rdoResinSearch);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Controls.Add(this.txtMixResultId);
            this.groupBox1.Controls.Add(this.rdoLotSearch);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(553, 112);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // txtAsmLotNo
            // 
            this.txtAsmLotNo.Location = new System.Drawing.Point(114, 83);
            this.txtAsmLotNo.Name = "txtAsmLotNo";
            this.txtAsmLotNo.Size = new System.Drawing.Size(216, 22);
            this.txtAsmLotNo.TabIndex = 68;
            // 
            // rdoAsmLot
            // 
            this.rdoAsmLot.AutoSize = true;
            this.rdoAsmLot.Location = new System.Drawing.Point(6, 84);
            this.rdoAsmLot.Name = "rdoAsmLot";
            this.rdoAsmLot.Size = new System.Drawing.Size(84, 18);
            this.rdoAsmLot.TabIndex = 67;
            this.rdoAsmLot.Text = "アッセンロット";
            this.rdoAsmLot.UseVisualStyleBackColor = true;
            // 
            // btnSelectAsmLot
            // 
            this.btnSelectAsmLot.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectAsmLot.ImageIndex = 1;
            this.btnSelectAsmLot.ImageList = this.imageList1;
            this.btnSelectAsmLot.Location = new System.Drawing.Point(336, 83);
            this.btnSelectAsmLot.Name = "btnSelectAsmLot";
            this.btnSelectAsmLot.Size = new System.Drawing.Size(22, 22);
            this.btnSelectAsmLot.TabIndex = 66;
            this.btnSelectAsmLot.UseVisualStyleBackColor = true;
            this.btnSelectAsmLot.Click += new System.EventHandler(this.btnSelectAsmLot_Click);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ACT_MULTISELECT_IN.png");
            this.imageList1.Images.SetKeyName(1, "ACT_MULTISELECT_EP.png");
            // 
            // btnSelectMat
            // 
            this.btnSelectMat.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectMat.Image = global::ArmsMaintenance.Properties.Resources.viewmag;
            this.btnSelectMat.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectMat.Location = new System.Drawing.Point(114, 47);
            this.btnSelectMat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectMat.Name = "btnSelectMat";
            this.btnSelectMat.Size = new System.Drawing.Size(102, 31);
            this.btnSelectMat.TabIndex = 64;
            this.btnSelectMat.Text = "原材料選択";
            this.btnSelectMat.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectMat.UseVisualStyleBackColor = true;
            this.btnSelectMat.Click += new System.EventHandler(this.btnSelectMat_Click);
            // 
            // rdoResinSearch
            // 
            this.rdoResinSearch.AutoSize = true;
            this.rdoResinSearch.Location = new System.Drawing.Point(6, 21);
            this.rdoResinSearch.Name = "rdoResinSearch";
            this.rdoResinSearch.Size = new System.Drawing.Size(85, 18);
            this.rdoResinSearch.TabIndex = 21;
            this.rdoResinSearch.Text = "樹脂調合ID";
            this.rdoResinSearch.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSearch.Location = new System.Drawing.Point(461, 80);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(0);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(80, 27);
            this.btnSearch.TabIndex = 41;
            this.btnSearch.Text = "検索";
            this.btnSearch.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // txtMixResultId
            // 
            this.txtMixResultId.Location = new System.Drawing.Point(114, 20);
            this.txtMixResultId.Name = "txtMixResultId";
            this.txtMixResultId.Size = new System.Drawing.Size(216, 22);
            this.txtMixResultId.TabIndex = 22;
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox2.Controls.Add(this.ckbRestrictRelease);
            this.groupBox2.Controls.Add(this.txtRestrictEmp);
            this.groupBox2.Controls.Add(this.cmbRestrictProc);
            this.groupBox2.Controls.Add(this.label2);
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.btnAddRestrict);
            this.groupBox2.Controls.Add(this.txtRestrictNote);
            this.groupBox2.Controls.Add(this.label1);
            this.groupBox2.Location = new System.Drawing.Point(12, 444);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(526, 180);
            this.groupBox2.TabIndex = 22;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "流動規制";
            // 
            // ckbRestrictRelease
            // 
            this.ckbRestrictRelease.AutoSize = true;
            this.ckbRestrictRelease.Location = new System.Drawing.Point(6, 156);
            this.ckbRestrictRelease.Name = "ckbRestrictRelease";
            this.ckbRestrictRelease.Size = new System.Drawing.Size(74, 18);
            this.ckbRestrictRelease.TabIndex = 46;
            this.ckbRestrictRelease.Text = "解除制限";
            this.ckbRestrictRelease.UseVisualStyleBackColor = true;
            // 
            // txtRestrictEmp
            // 
            this.txtRestrictEmp.Location = new System.Drawing.Point(147, 152);
            this.txtRestrictEmp.Name = "txtRestrictEmp";
            this.txtRestrictEmp.Size = new System.Drawing.Size(127, 22);
            this.txtRestrictEmp.TabIndex = 45;
            // 
            // cmbRestrictProc
            // 
            this.cmbRestrictProc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbRestrictProc.FormattingEnabled = true;
            this.cmbRestrictProc.Location = new System.Drawing.Point(91, 21);
            this.cmbRestrictProc.Name = "cmbRestrictProc";
            this.cmbRestrictProc.Size = new System.Drawing.Size(184, 22);
            this.cmbRestrictProc.TabIndex = 44;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(93, 156);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 14);
            this.label2.TabIndex = 43;
            this.label2.Text = "社員番号";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(79, 14);
            this.label3.TabIndex = 43;
            this.label3.Text = "投入規制工程";
            // 
            // btnAddRestrict
            // 
            this.btnAddRestrict.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnAddRestrict.Image = ((System.Drawing.Image)(resources.GetObject("btnAddRestrict.Image")));
            this.btnAddRestrict.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnAddRestrict.Location = new System.Drawing.Point(362, 150);
            this.btnAddRestrict.Margin = new System.Windows.Forms.Padding(0);
            this.btnAddRestrict.Name = "btnAddRestrict";
            this.btnAddRestrict.Size = new System.Drawing.Size(158, 27);
            this.btnAddRestrict.TabIndex = 42;
            this.btnAddRestrict.Text = "選択ロットを流動規制";
            this.btnAddRestrict.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnAddRestrict.UseVisualStyleBackColor = true;
            this.btnAddRestrict.Click += new System.EventHandler(this.btnAddRestrict_Click);
            // 
            // txtRestrictNote
            // 
            this.txtRestrictNote.Location = new System.Drawing.Point(9, 66);
            this.txtRestrictNote.Multiline = true;
            this.txtRestrictNote.Name = "txtRestrictNote";
            this.txtRestrictNote.Size = new System.Drawing.Size(511, 81);
            this.txtRestrictNote.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 49);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "規制理由";
            // 
            // FrmLotTrace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 636);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.grdLotList);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmLotTrace";
            this.Text = "ロットトレース/一括規制";
            this.Load += new System.EventHandler(this.FrmLotTrace_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdLotList)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton rdoLotSearch;
        private System.Windows.Forms.DataGridView grdLotList;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rdoResinSearch;
        private System.Windows.Forms.TextBox txtMixResultId;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btnAddRestrict;
        private System.Windows.Forms.TextBox txtRestrictNote;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnSelectMat;
        private System.Windows.Forms.ComboBox cmbRestrictProc;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAsmLotNo;
        private System.Windows.Forms.RadioButton rdoAsmLot;
        private System.Windows.Forms.Button btnSelectAsmLot;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.TextBox txtRestrictEmp;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox ckbRestrictRelease;
    }
}