namespace ArmsMaintenance
{
    partial class FrmMacMat
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
            this.txtMacNo = new System.Windows.Forms.TextBox();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnQR = new System.Windows.Forms.Button();
            this.txtQR = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numStocker = new System.Windows.Forms.NumericUpDown();
            this.btnApplyEdit = new System.Windows.Forms.Button();
            this.txtMaterialNm = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.chkRemove = new System.Windows.Forms.CheckBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnSelectMat = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.txtLotNO = new System.Windows.Forms.TextBox();
            this.txtMaterialCd = new System.Windows.Forms.TextBox();
            this.dtpRemoveDt = new System.Windows.Forms.DateTimePicker();
            this.dtpInputDt = new System.Windows.Forms.DateTimePicker();
            this.chkRindId = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStocker)).BeginInit();
            this.SuspendLayout();
            // 
            // txtMacNo
            // 
            this.txtMacNo.Location = new System.Drawing.Point(99, 80);
            this.txtMacNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMacNo.Name = "txtMacNo";
            this.txtMacNo.ReadOnly = true;
            this.txtMacNo.Size = new System.Drawing.Size(89, 22);
            this.txtMacNo.TabIndex = 39;
            // 
            // txtMachine
            // 
            this.txtMachine.Location = new System.Drawing.Point(99, 106);
            this.txtMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(185, 22);
            this.txtMachine.TabIndex = 33;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(35, 83);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "作業号機";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnQR);
            this.groupBox1.Controls.Add(this.txtQR);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtMacNo);
            this.groupBox1.Controls.Add(this.numStocker);
            this.groupBox1.Controls.Add(this.txtMachine);
            this.groupBox1.Controls.Add(this.btnApplyEdit);
            this.groupBox1.Controls.Add(this.txtMaterialNm);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.label8);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.chkRindId);
            this.groupBox1.Controls.Add(this.chkRemove);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.btnSelectMat);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.txtLotNO);
            this.groupBox1.Controls.Add(this.txtMaterialCd);
            this.groupBox1.Controls.Add(this.dtpRemoveDt);
            this.groupBox1.Controls.Add(this.dtpInputDt);
            this.groupBox1.Location = new System.Drawing.Point(14, 14);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(335, 340);
            this.groupBox1.TabIndex = 40;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "更新内容";
            // 
            // btnQR
            // 
            this.btnQR.Location = new System.Drawing.Point(270, 45);
            this.btnQR.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnQR.Name = "btnQR";
            this.btnQR.Size = new System.Drawing.Size(59, 27);
            this.btnQR.TabIndex = 58;
            this.btnQR.Text = "読込";
            this.btnQR.UseVisualStyleBackColor = true;
            this.btnQR.Click += new System.EventHandler(this.btnQR_Click);
            // 
            // txtQR
            // 
            this.txtQR.Location = new System.Drawing.Point(37, 48);
            this.txtQR.Name = "txtQR";
            this.txtQR.Size = new System.Drawing.Size(227, 22);
            this.txtQR.TabIndex = 57;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 51);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(23, 14);
            this.label4.TabIndex = 56;
            this.label4.Text = "QR";
            // 
            // numStocker
            // 
            this.numStocker.Location = new System.Drawing.Point(99, 134);
            this.numStocker.Name = "numStocker";
            this.numStocker.Size = new System.Drawing.Size(82, 22);
            this.numStocker.TabIndex = 55;
            this.numStocker.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            // 
            // btnApplyEdit
            // 
            this.btnApplyEdit.Location = new System.Drawing.Point(227, 306);
            this.btnApplyEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnApplyEdit.Name = "btnApplyEdit";
            this.btnApplyEdit.Size = new System.Drawing.Size(93, 27);
            this.btnApplyEdit.TabIndex = 54;
            this.btnApplyEdit.Text = "変更反映";
            this.btnApplyEdit.UseVisualStyleBackColor = true;
            this.btnApplyEdit.Click += new System.EventHandler(this.btnApplyEdit_Click);
            // 
            // txtMaterialNm
            // 
            this.txtMaterialNm.Location = new System.Drawing.Point(99, 189);
            this.txtMaterialNm.Name = "txtMaterialNm";
            this.txtMaterialNm.ReadOnly = true;
            this.txtMaterialNm.Size = new System.Drawing.Size(203, 22);
            this.txtMaterialNm.TabIndex = 53;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(43, 136);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(50, 14);
            this.label1.TabIndex = 41;
            this.label1.Text = "ストッカー";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(47, 220);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(46, 14);
            this.label8.TabIndex = 52;
            this.label8.Text = "ロットNo";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(43, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 14);
            this.label2.TabIndex = 42;
            this.label2.Text = "品目CD";
            // 
            // chkRemove
            // 
            this.chkRemove.AutoSize = true;
            this.chkRemove.Location = new System.Drawing.Point(11, 273);
            this.chkRemove.Name = "chkRemove";
            this.chkRemove.Size = new System.Drawing.Size(82, 18);
            this.chkRemove.TabIndex = 44;
            this.chkRemove.Text = "取外し済み";
            this.chkRemove.UseVisualStyleBackColor = true;
            this.chkRemove.CheckedChanged += new System.EventHandler(this.chkRemove_CheckedChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(38, 192);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(55, 14);
            this.label6.TabIndex = 43;
            this.label6.Text = "原材料名";
            // 
            // btnSelectMat
            // 
            this.btnSelectMat.Location = new System.Drawing.Point(243, 160);
            this.btnSelectMat.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectMat.Name = "btnSelectMat";
            this.btnSelectMat.Size = new System.Drawing.Size(59, 27);
            this.btnSelectMat.TabIndex = 41;
            this.btnSelectMat.Text = "選択";
            this.btnSelectMat.UseVisualStyleBackColor = true;
            this.btnSelectMat.Click += new System.EventHandler(this.btnSelectMat_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(40, 248);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 14);
            this.label7.TabIndex = 44;
            this.label7.Text = "割付け日";
            // 
            // txtLotNO
            // 
            this.txtLotNO.Location = new System.Drawing.Point(99, 217);
            this.txtLotNO.Name = "txtLotNO";
            this.txtLotNO.ReadOnly = true;
            this.txtLotNO.Size = new System.Drawing.Size(203, 22);
            this.txtLotNO.TabIndex = 51;
            // 
            // txtMaterialCd
            // 
            this.txtMaterialCd.Location = new System.Drawing.Point(99, 161);
            this.txtMaterialCd.Name = "txtMaterialCd";
            this.txtMaterialCd.ReadOnly = true;
            this.txtMaterialCd.Size = new System.Drawing.Size(136, 22);
            this.txtMaterialCd.TabIndex = 50;
            // 
            // dtpRemoveDt
            // 
            this.dtpRemoveDt.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpRemoveDt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpRemoveDt.Location = new System.Drawing.Point(99, 270);
            this.dtpRemoveDt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpRemoveDt.Name = "dtpRemoveDt";
            this.dtpRemoveDt.Size = new System.Drawing.Size(173, 22);
            this.dtpRemoveDt.TabIndex = 48;
            // 
            // dtpInputDt
            // 
            this.dtpInputDt.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpInputDt.Enabled = false;
            this.dtpInputDt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInputDt.Location = new System.Drawing.Point(99, 244);
            this.dtpInputDt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpInputDt.Name = "dtpInputDt";
            this.dtpInputDt.Size = new System.Drawing.Size(173, 22);
            this.dtpInputDt.TabIndex = 49;
            // 
            // chkRindId
            // 
            this.chkRindId.AutoSize = true;
            this.chkRindId.Location = new System.Drawing.Point(243, 18);
            this.chkRindId.Name = "chkRindId";
            this.chkRindId.Size = new System.Drawing.Size(63, 18);
            this.chkRindId.TabIndex = 44;
            this.chkRindId.Text = "リングID";
            this.chkRindId.UseVisualStyleBackColor = true;
            this.chkRindId.CheckedChanged += new System.EventHandler(this.chkRemove_CheckedChanged);
            // 
            // FrmMacMat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(363, 366);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.KeyPreview = true;
            this.Name = "FrmMacMat";
            this.Text = "FrmMacMat";
            this.Load += new System.EventHandler(this.FrmMacMat_Load);
            this.Shown += new System.EventHandler(this.FrmMacMat_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmMacMat_KeyPress);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numStocker)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtMacNo;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox chkRemove;
        private System.Windows.Forms.Button btnSelectMat;
        private System.Windows.Forms.TextBox txtLotNO;
        private System.Windows.Forms.TextBox txtMaterialCd;
        private System.Windows.Forms.DateTimePicker dtpInputDt;
        private System.Windows.Forms.DateTimePicker dtpRemoveDt;
        private System.Windows.Forms.TextBox txtMaterialNm;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Button btnApplyEdit;
        private System.Windows.Forms.NumericUpDown numStocker;
        private System.Windows.Forms.TextBox txtQR;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnQR;
        private System.Windows.Forms.CheckBox chkRindId;
    }
}