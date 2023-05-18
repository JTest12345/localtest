namespace ArmsMaintenance
{
    partial class FrmPSTesterExport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPSTesterExport));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnBallExport = new System.Windows.Forms.Button();
            this.txtTypeCd = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btnExport = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtWBMacNm = new System.Windows.Forms.TextBox();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.txtMagNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btnFilmExport = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.btnFilmExport);
            this.groupBox1.Controls.Add(this.btnBallExport);
            this.groupBox1.Controls.Add(this.txtTypeCd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.btnExport);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtWBMacNm);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.txtMagNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(12, 15);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(290, 198);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "マガジン情報編集";
            // 
            // btnBallExport
            // 
            this.btnBallExport.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnBallExport.Image = ((System.Drawing.Image)(resources.GetObject("btnBallExport.Image")));
            this.btnBallExport.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnBallExport.Location = new System.Drawing.Point(8, 131);
            this.btnBallExport.Name = "btnBallExport";
            this.btnBallExport.Size = new System.Drawing.Size(130, 26);
            this.btnBallExport.TabIndex = 12;
            this.btnBallExport.Text = "ボール径データ出力";
            this.btnBallExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnBallExport.UseVisualStyleBackColor = true;
            this.btnBallExport.Click += new System.EventHandler(this.btnBallExport_Click);
            // 
            // txtTypeCd
            // 
            this.txtTypeCd.Location = new System.Drawing.Point(96, 78);
            this.txtTypeCd.Name = "txtTypeCd";
            this.txtTypeCd.ReadOnly = true;
            this.txtTypeCd.Size = new System.Drawing.Size(177, 19);
            this.txtTypeCd.TabIndex = 11;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(6, 81);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(68, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "NASCA型番";
            // 
            // btnExport
            // 
            this.btnExport.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnExport.Image = ((System.Drawing.Image)(resources.GetObject("btnExport.Image")));
            this.btnExport.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnExport.Location = new System.Drawing.Point(143, 131);
            this.btnExport.Name = "btnExport";
            this.btnExport.Size = new System.Drawing.Size(130, 26);
            this.btnExport.TabIndex = 9;
            this.btnExport.Text = "強度データ出力";
            this.btnExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnExport.UseVisualStyleBackColor = true;
            this.btnExport.Click += new System.EventHandler(this.btnExport_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(6, 109);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(46, 12);
            this.label3.TabIndex = 5;
            this.label3.Text = "WB号機";
            // 
            // txtWBMacNm
            // 
            this.txtWBMacNm.Location = new System.Drawing.Point(96, 106);
            this.txtWBMacNm.Name = "txtWBMacNm";
            this.txtWBMacNm.ReadOnly = true;
            this.txtWBMacNm.Size = new System.Drawing.Size(177, 19);
            this.txtWBMacNm.TabIndex = 4;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(96, 50);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.ReadOnly = true;
            this.txtLotNo.Size = new System.Drawing.Size(177, 19);
            this.txtLotNo.TabIndex = 3;
            // 
            // txtMagNo
            // 
            this.txtMagNo.Location = new System.Drawing.Point(96, 22);
            this.txtMagNo.Name = "txtMagNo";
            this.txtMagNo.ReadOnly = true;
            this.txtMagNo.Size = new System.Drawing.Size(177, 19);
            this.txtMagNo.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(6, 53);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(82, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "NASCAロットNo";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "マガジンNo";
            // 
            // btnFilmExport
            // 
            this.btnFilmExport.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnFilmExport.Image = ((System.Drawing.Image)(resources.GetObject("btnFilmExport.Image")));
            this.btnFilmExport.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnFilmExport.Location = new System.Drawing.Point(8, 163);
            this.btnFilmExport.Name = "btnFilmExport";
            this.btnFilmExport.Size = new System.Drawing.Size(130, 26);
            this.btnFilmExport.TabIndex = 12;
            this.btnFilmExport.Text = "膜厚データ出力";
            this.btnFilmExport.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnFilmExport.UseVisualStyleBackColor = true;
            this.btnFilmExport.Click += new System.EventHandler(this.btnFilmExport_Click);
            // 
            // FrmPSTesterExport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(314, 227);
            this.Controls.Add(this.groupBox1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "FrmPSTesterExport";
            this.Text = "強度データ出力";
            this.Load += new System.EventHandler(this.FrmPSTesterExport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtTypeCd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btnExport;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtWBMacNm;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.TextBox txtMagNo;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnBallExport;
        private System.Windows.Forms.Button btnFilmExport;
    }
}