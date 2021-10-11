namespace ArmsMaintenance
{
    partial class FrmMacResin
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtMixLimit = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btnApplyEdit = new System.Windows.Forms.Button();
            this.txtMacNo = new System.Windows.Forms.TextBox();
            this.chkRemove = new System.Windows.Forms.CheckBox();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.dtpInputDt = new System.Windows.Forms.DateTimePicker();
            this.txtResinGpCd = new System.Windows.Forms.TextBox();
            this.dtpRemoveDt = new System.Windows.Forms.DateTimePicker();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.txtMixResultId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtMixLimit);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.btnApplyEdit);
            this.groupBox1.Controls.Add(this.txtMacNo);
            this.groupBox1.Controls.Add(this.chkRemove);
            this.groupBox1.Controls.Add(this.txtMachine);
            this.groupBox1.Controls.Add(this.dtpInputDt);
            this.groupBox1.Controls.Add(this.txtResinGpCd);
            this.groupBox1.Controls.Add(this.dtpRemoveDt);
            this.groupBox1.Controls.Add(this.label7);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.txtMixResultId);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(14, 13);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(328, 278);
            this.groupBox1.TabIndex = 43;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "編集";
            // 
            // txtMixLimit
            // 
            this.txtMixLimit.Location = new System.Drawing.Point(107, 142);
            this.txtMixLimit.Name = "txtMixLimit";
            this.txtMixLimit.ReadOnly = true;
            this.txtMixLimit.Size = new System.Drawing.Size(203, 22);
            this.txtMixLimit.TabIndex = 55;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(46, 37);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 32;
            this.label3.Text = "作業号機";
            // 
            // btnApplyEdit
            // 
            this.btnApplyEdit.Location = new System.Drawing.Point(229, 243);
            this.btnApplyEdit.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnApplyEdit.Name = "btnApplyEdit";
            this.btnApplyEdit.Size = new System.Drawing.Size(93, 27);
            this.btnApplyEdit.TabIndex = 54;
            this.btnApplyEdit.Text = "変更反映";
            this.btnApplyEdit.UseVisualStyleBackColor = true;
            this.btnApplyEdit.Click += new System.EventHandler(this.btnApplyEdit_Click);
            // 
            // txtMacNo
            // 
            this.txtMacNo.Location = new System.Drawing.Point(107, 34);
            this.txtMacNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMacNo.Name = "txtMacNo";
            this.txtMacNo.ReadOnly = true;
            this.txtMacNo.Size = new System.Drawing.Size(91, 22);
            this.txtMacNo.TabIndex = 39;
            // 
            // chkRemove
            // 
            this.chkRemove.AutoSize = true;
            this.chkRemove.Location = new System.Drawing.Point(19, 198);
            this.chkRemove.Name = "chkRemove";
            this.chkRemove.Size = new System.Drawing.Size(82, 18);
            this.chkRemove.TabIndex = 44;
            this.chkRemove.Text = "取外し済み";
            this.chkRemove.UseVisualStyleBackColor = true;
            this.chkRemove.CheckedChanged += new System.EventHandler(this.chkRemove_CheckedChanged);
            // 
            // txtMachine
            // 
            this.txtMachine.Location = new System.Drawing.Point(107, 60);
            this.txtMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(203, 22);
            this.txtMachine.TabIndex = 33;
            // 
            // dtpInputDt
            // 
            this.dtpInputDt.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpInputDt.Enabled = false;
            this.dtpInputDt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpInputDt.Location = new System.Drawing.Point(107, 169);
            this.dtpInputDt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpInputDt.Name = "dtpInputDt";
            this.dtpInputDt.Size = new System.Drawing.Size(173, 22);
            this.dtpInputDt.TabIndex = 49;
            // 
            // txtResinGpCd
            // 
            this.txtResinGpCd.Location = new System.Drawing.Point(107, 114);
            this.txtResinGpCd.Name = "txtResinGpCd";
            this.txtResinGpCd.ReadOnly = true;
            this.txtResinGpCd.Size = new System.Drawing.Size(203, 22);
            this.txtResinGpCd.TabIndex = 53;
            // 
            // dtpRemoveDt
            // 
            this.dtpRemoveDt.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpRemoveDt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpRemoveDt.Location = new System.Drawing.Point(107, 195);
            this.dtpRemoveDt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpRemoveDt.Name = "dtpRemoveDt";
            this.dtpRemoveDt.Size = new System.Drawing.Size(173, 22);
            this.dtpRemoveDt.TabIndex = 48;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(48, 173);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(53, 14);
            this.label7.TabIndex = 44;
            this.label7.Text = "割付け日";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(34, 90);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 14);
            this.label2.TabIndex = 42;
            this.label2.Text = "調合結果ID";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(17, 117);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(84, 14);
            this.label6.TabIndex = 43;
            this.label6.Text = "樹脂グループCD";
            // 
            // txtMixResultId
            // 
            this.txtMixResultId.Location = new System.Drawing.Point(107, 87);
            this.txtMixResultId.Name = "txtMixResultId";
            this.txtMixResultId.Size = new System.Drawing.Size(203, 22);
            this.txtMixResultId.TabIndex = 50;
            this.txtMixResultId.Leave += new System.EventHandler(this.txtMixResultId_Leave);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(22, 145);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(79, 14);
            this.label9.TabIndex = 46;
            this.label9.Text = "調合有効期限";
            // 
            // FrmMacResin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 306);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.KeyPreview = true;
            this.Name = "FrmMacResin";
            this.Text = "FrmMacResin";
            this.Load += new System.EventHandler(this.FrmMacResin_Load);
            this.Shown += new System.EventHandler(this.FrmMacResin_Shown);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmMacResin_KeyPress);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtMacNo;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.TextBox txtMixLimit;
        private System.Windows.Forms.Button btnApplyEdit;
        private System.Windows.Forms.CheckBox chkRemove;
        private System.Windows.Forms.DateTimePicker dtpInputDt;
        private System.Windows.Forms.TextBox txtResinGpCd;
        private System.Windows.Forms.DateTimePicker dtpRemoveDt;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtMixResultId;
        private System.Windows.Forms.Label label9;

    }
}