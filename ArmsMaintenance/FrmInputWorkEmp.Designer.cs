namespace ArmsMaintenance
{
    partial class FrmInputWorkEmp
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmInputWorkEmp));
            this.txtEmpCd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.btnSave = new System.Windows.Forms.Button();
            this.cmbProcess = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.dtpWorkDt = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbWorkGroup = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // txtEmpCd
            // 
            this.txtEmpCd.Location = new System.Drawing.Point(93, 23);
            this.txtEmpCd.Name = "txtEmpCd";
            this.txtEmpCd.Size = new System.Drawing.Size(165, 19);
            this.txtEmpCd.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(27, 26);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "社員番号";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(27, 142);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(29, 12);
            this.label12.TabIndex = 20;
            this.label12.Text = "工程";
            // 
            // btnSave
            // 
            this.btnSave.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSave.Location = new System.Drawing.Point(145, 184);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(107, 27);
            this.btnSave.TabIndex = 22;
            this.btnSave.Text = "保存";
            this.btnSave.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSave.UseVisualStyleBackColor = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // cmbProcess
            // 
            this.cmbProcess.FormattingEnabled = true;
            this.cmbProcess.Location = new System.Drawing.Point(93, 138);
            this.cmbProcess.Name = "cmbProcess";
            this.cmbProcess.Size = new System.Drawing.Size(165, 20);
            this.cmbProcess.TabIndex = 27;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(27, 61);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(41, 12);
            this.label7.TabIndex = 50;
            this.label7.Text = "作業日";
            // 
            // dtpWorkDt
            // 
            this.dtpWorkDt.CustomFormat = "yyyy/MM/dd";
            this.dtpWorkDt.Enabled = false;
            this.dtpWorkDt.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkDt.Location = new System.Drawing.Point(93, 58);
            this.dtpWorkDt.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkDt.Name = "dtpWorkDt";
            this.dtpWorkDt.Size = new System.Drawing.Size(165, 19);
            this.dtpWorkDt.TabIndex = 51;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(27, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(41, 12);
            this.label1.TabIndex = 52;
            this.label1.Text = "勤務帯";
            // 
            // cmbWorkGroup
            // 
            this.cmbWorkGroup.FormattingEnabled = true;
            this.cmbWorkGroup.Location = new System.Drawing.Point(93, 96);
            this.cmbWorkGroup.Name = "cmbWorkGroup";
            this.cmbWorkGroup.Size = new System.Drawing.Size(165, 20);
            this.cmbWorkGroup.TabIndex = 53;
            // 
            // FrmInputWorkEmp
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 223);
            this.Controls.Add(this.cmbWorkGroup);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dtpWorkDt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.cmbProcess);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtEmpCd);
            this.Controls.Add(this.label5);
            this.Name = "FrmInputWorkEmp";
            this.Text = "作業者登録";
            this.Load += new System.EventHandler(this.FrmInputWorkEmp_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtEmpCd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.ComboBox cmbProcess;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.DateTimePicker dtpWorkDt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmbWorkGroup;
    }
}