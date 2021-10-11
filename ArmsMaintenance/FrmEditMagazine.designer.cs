namespace ArmsMaintenance
{
    partial class FrmEditMagazine
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEditMagazine));
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtTypeCd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.chkActive = new System.Windows.Forms.CheckBox();
            this.cmbNowCompProcess = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.txtResinGpCd = new System.Windows.Forms.TextBox();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.txtMagNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 58);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(60, 14);
            this.label1.TabIndex = 0;
            this.label1.Text = "マガジンNo";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtTypeCd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnUpdate);
            this.groupBox1.Controls.Add(this.chkActive);
            this.groupBox1.Controls.Add(this.cmbNowCompProcess);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtResinGpCd);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.txtMagNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(291, 237);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "マガジン情報編集";
            // 
            // txtTypeCd
            // 
            this.txtTypeCd.Location = new System.Drawing.Point(96, 111);
            this.txtTypeCd.Name = "txtTypeCd";
            this.txtTypeCd.ReadOnly = true;
            this.txtTypeCd.Size = new System.Drawing.Size(177, 22);
            this.txtTypeCd.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 114);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(69, 14);
            this.label5.TabIndex = 10;
            this.label5.Text = "NASCA型番";
            // 
            // btnUpdate
            // 
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUpdate.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdate.Image")));
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.Location = new System.Drawing.Point(198, 195);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(75, 26);
            this.btnUpdate.TabIndex = 9;
            this.btnUpdate.Text = "更新";
            this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // chkActive
            // 
            this.chkActive.AutoSize = true;
            this.chkActive.Location = new System.Drawing.Point(9, 31);
            this.chkActive.Name = "chkActive";
            this.chkActive.Size = new System.Drawing.Size(86, 18);
            this.chkActive.TabIndex = 8;
            this.chkActive.Text = "現在稼働中";
            this.chkActive.UseVisualStyleBackColor = true;
            // 
            // cmbNowCompProcess
            // 
            this.cmbNowCompProcess.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbNowCompProcess.FormattingEnabled = true;
            this.cmbNowCompProcess.Location = new System.Drawing.Point(96, 167);
            this.cmbNowCompProcess.Name = "cmbNowCompProcess";
            this.cmbNowCompProcess.Size = new System.Drawing.Size(177, 22);
            this.cmbNowCompProcess.TabIndex = 7;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(6, 170);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(79, 14);
            this.label4.TabIndex = 6;
            this.label4.Text = "現在完了工程";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 142);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(84, 14);
            this.label3.TabIndex = 5;
            this.label3.Text = "樹脂グループCD";
            // 
            // txtResinGpCd
            // 
            this.txtResinGpCd.Location = new System.Drawing.Point(96, 139);
            this.txtResinGpCd.Name = "txtResinGpCd";
            this.txtResinGpCd.ReadOnly = true;
            this.txtResinGpCd.Size = new System.Drawing.Size(177, 22);
            this.txtResinGpCd.TabIndex = 4;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(96, 83);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.ReadOnly = true;
            this.txtLotNo.Size = new System.Drawing.Size(177, 22);
            this.txtLotNo.TabIndex = 3;
            // 
            // txtMagNo
            // 
            this.txtMagNo.Location = new System.Drawing.Point(96, 55);
            this.txtMagNo.Name = "txtMagNo";
            this.txtMagNo.Size = new System.Drawing.Size(177, 22);
            this.txtMagNo.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 86);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(84, 14);
            this.label2.TabIndex = 1;
            this.label2.Text = "NASCAロットNo";
            // 
            // FrmEditMagazine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(315, 261);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmEditMagazine";
            this.Text = "FrmEditMagazine";
            this.Load += new System.EventHandler(this.FrmEditMagazine_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtResinGpCd;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.TextBox txtMagNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmbNowCompProcess;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.CheckBox chkActive;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.TextBox txtTypeCd;
        private System.Windows.Forms.Label label5;
    }
}