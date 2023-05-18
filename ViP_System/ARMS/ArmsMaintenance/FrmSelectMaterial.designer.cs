namespace ArmsMaintenance
{
    partial class FrmSelectMaterial
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSelectMaterial));
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.txtMatNm = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.grdMaterials = new System.Windows.Forms.DataGridView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.btnSelectLot = new System.Windows.Forms.Button();
            this.rdoLotSearch = new System.Windows.Forms.RadioButton();
            this.rdoAsmLot = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.txtAsmLot = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.cmbDBProc = new System.Windows.Forms.ComboBox();
            this.cmbCurrentProc = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.numMagRow = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterials)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMagRow)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(34, 30);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(55, 14);
            this.label1.TabIndex = 9;
            this.label1.Text = "原材料名";
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(421, 500);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(104, 28);
            this.btnApply.TabIndex = 8;
            this.btnApply.Text = "反映";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // txtMatNm
            // 
            this.txtMatNm.Location = new System.Drawing.Point(95, 27);
            this.txtMatNm.Name = "txtMatNm";
            this.txtMatNm.Size = new System.Drawing.Size(241, 22);
            this.txtMatNm.TabIndex = 7;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(375, 52);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(90, 28);
            this.btnSearch.TabIndex = 5;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 59);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 14);
            this.label2.TabIndex = 11;
            this.label2.Text = "ロットNo";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(95, 56);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(241, 22);
            this.txtLotNo.TabIndex = 10;
            // 
            // grdMaterials
            // 
            this.grdMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdMaterials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMaterials.Location = new System.Drawing.Point(16, 214);
            this.grdMaterials.MultiSelect = false;
            this.grdMaterials.Name = "grdMaterials";
            this.grdMaterials.ReadOnly = true;
            this.grdMaterials.RowTemplate.Height = 21;
            this.grdMaterials.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdMaterials.Size = new System.Drawing.Size(509, 280);
            this.grdMaterials.TabIndex = 12;
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "ACT_MULTISELECT_IN.png");
            this.imageList1.Images.SetKeyName(1, "ACT_MULTISELECT_EP.png");
            // 
            // btnSelectLot
            // 
            this.btnSelectLot.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectLot.ImageIndex = 1;
            this.btnSelectLot.ImageList = this.imageList1;
            this.btnSelectLot.Location = new System.Drawing.Point(337, 56);
            this.btnSelectLot.Name = "btnSelectLot";
            this.btnSelectLot.Size = new System.Drawing.Size(22, 22);
            this.btnSelectLot.TabIndex = 67;
            this.btnSelectLot.UseVisualStyleBackColor = true;
            this.btnSelectLot.Visible = false;
            this.btnSelectLot.Click += new System.EventHandler(this.btnSelectLot_Click);
            // 
            // rdoLotSearch
            // 
            this.rdoLotSearch.AutoSize = true;
            this.rdoLotSearch.Checked = true;
            this.rdoLotSearch.Location = new System.Drawing.Point(16, 9);
            this.rdoLotSearch.Name = "rdoLotSearch";
            this.rdoLotSearch.Size = new System.Drawing.Size(73, 18);
            this.rdoLotSearch.TabIndex = 68;
            this.rdoLotSearch.TabStop = true;
            this.rdoLotSearch.Text = "資材ロット";
            this.rdoLotSearch.UseVisualStyleBackColor = true;
            // 
            // rdoAsmLot
            // 
            this.rdoAsmLot.AutoSize = true;
            this.rdoAsmLot.Location = new System.Drawing.Point(16, 84);
            this.rdoAsmLot.Name = "rdoAsmLot";
            this.rdoAsmLot.Size = new System.Drawing.Size(143, 18);
            this.rdoAsmLot.TabIndex = 69;
            this.rdoAsmLot.Text = "アッセンロット⇒ウェハ検索";
            this.rdoAsmLot.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(43, 108);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(66, 14);
            this.label3.TabIndex = 70;
            this.label3.Text = "アッセンロット";
            // 
            // txtAsmLot
            // 
            this.txtAsmLot.Location = new System.Drawing.Point(142, 105);
            this.txtAsmLot.Name = "txtAsmLot";
            this.txtAsmLot.Size = new System.Drawing.Size(241, 22);
            this.txtAsmLot.TabIndex = 71;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(43, 135);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(93, 14);
            this.label4.TabIndex = 72;
            this.label4.Text = "製品マガジン段数";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(43, 163);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 14);
            this.label5.TabIndex = 74;
            this.label5.Text = "搭載工程";
            // 
            // cmbDBProc
            // 
            this.cmbDBProc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbDBProc.FormattingEnabled = true;
            this.cmbDBProc.Location = new System.Drawing.Point(142, 160);
            this.cmbDBProc.Name = "cmbDBProc";
            this.cmbDBProc.Size = new System.Drawing.Size(184, 22);
            this.cmbDBProc.TabIndex = 75;
            // 
            // cmbCurrentProc
            // 
            this.cmbCurrentProc.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrentProc.FormattingEnabled = true;
            this.cmbCurrentProc.Location = new System.Drawing.Point(142, 186);
            this.cmbCurrentProc.Name = "cmbCurrentProc";
            this.cmbCurrentProc.Size = new System.Drawing.Size(184, 22);
            this.cmbCurrentProc.TabIndex = 76;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(43, 189);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 14);
            this.label6.TabIndex = 77;
            this.label6.Text = "現工程";
            // 
            // numMagRow
            // 
            this.numMagRow.Location = new System.Drawing.Point(142, 133);
            this.numMagRow.Name = "numMagRow";
            this.numMagRow.Size = new System.Drawing.Size(79, 22);
            this.numMagRow.TabIndex = 78;
            // 
            // FrmSelectMaterial
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(541, 540);
            this.Controls.Add(this.numMagRow);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.cmbCurrentProc);
            this.Controls.Add(this.cmbDBProc);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtAsmLot);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rdoAsmLot);
            this.Controls.Add(this.rdoLotSearch);
            this.Controls.Add(this.btnSelectLot);
            this.Controls.Add(this.grdMaterials);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLotNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.txtMatNm);
            this.Controls.Add(this.btnSearch);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmSelectMaterial";
            this.Text = "FrmSelectMaterial";
            this.Load += new System.EventHandler(this.FrmSelectMaterial_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterials)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numMagRow)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.TextBox txtMatNm;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.DataGridView grdMaterials;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.Button btnSelectLot;
        private System.Windows.Forms.RadioButton rdoLotSearch;
        private System.Windows.Forms.RadioButton rdoAsmLot;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtAsmLot;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.ComboBox cmbDBProc;
        private System.Windows.Forms.ComboBox cmbCurrentProc;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown numMagRow;

    }
}