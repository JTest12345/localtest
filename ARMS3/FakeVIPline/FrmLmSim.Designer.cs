namespace ARMS3.FakeVIPline
{
    partial class FrmLmSim
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
            this.txt_magno = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_typecd = new System.Windows.Forms.TextBox();
            this.txt_matcode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_matlot = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_retcode = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.btn_setdata = new System.Windows.Forms.Button();
            this.btn_upload = new System.Windows.Forms.Button();
            this.txt_bdvol = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_magno
            // 
            this.txt_magno.Location = new System.Drawing.Point(111, 14);
            this.txt_magno.Name = "txt_magno";
            this.txt_magno.Size = new System.Drawing.Size(117, 19);
            this.txt_magno.TabIndex = 0;
            this.txt_magno.Text = "C30 M00001";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(46, 18);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "マガジンNo:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(61, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(43, 12);
            this.label2.TabIndex = 1;
            this.label2.Text = "製品名:";
            // 
            // txt_typecd
            // 
            this.txt_typecd.Location = new System.Drawing.Point(111, 39);
            this.txt_typecd.Name = "txt_typecd";
            this.txt_typecd.Size = new System.Drawing.Size(117, 19);
            this.txt_typecd.TabIndex = 0;
            this.txt_typecd.Text = "CL-A160-1W9-S4";
            // 
            // txt_matcode
            // 
            this.txt_matcode.Location = new System.Drawing.Point(111, 64);
            this.txt_matcode.Name = "txt_matcode";
            this.txt_matcode.Size = new System.Drawing.Size(117, 19);
            this.txt_matcode.TabIndex = 0;
            this.txt_matcode.Text = "284-A0XXXX";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 68);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(82, 12);
            this.label3.TabIndex = 1;
            this.label3.Text = "基板部材コード:";
            // 
            // txt_matlot
            // 
            this.txt_matlot.Location = new System.Drawing.Point(111, 89);
            this.txt_matlot.Name = "txt_matlot";
            this.txt_matlot.Size = new System.Drawing.Size(117, 19);
            this.txt_matlot.TabIndex = 0;
            this.txt_matlot.Text = "KIBANLOTNO001";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(60, 92);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 12);
            this.label4.TabIndex = 1;
            this.label4.Text = "ロットNo:";
            // 
            // txt_retcode
            // 
            this.txt_retcode.Location = new System.Drawing.Point(164, 29);
            this.txt_retcode.Name = "txt_retcode";
            this.txt_retcode.Size = new System.Drawing.Size(38, 19);
            this.txt_retcode.TabIndex = 0;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(103, 32);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 12);
            this.label5.TabIndex = 1;
            this.label5.Text = "返信コード:";
            // 
            // btn_setdata
            // 
            this.btn_setdata.Location = new System.Drawing.Point(13, 139);
            this.btn_setdata.Name = "btn_setdata";
            this.btn_setdata.Size = new System.Drawing.Size(215, 23);
            this.btn_setdata.TabIndex = 2;
            this.btn_setdata.Text = "PLC転送";
            this.btn_setdata.UseVisualStyleBackColor = true;
            this.btn_setdata.Click += new System.EventHandler(this.btn_setdata_Click);
            // 
            // btn_upload
            // 
            this.btn_upload.Location = new System.Drawing.Point(12, 27);
            this.btn_upload.Name = "btn_upload";
            this.btn_upload.Size = new System.Drawing.Size(79, 23);
            this.btn_upload.TabIndex = 2;
            this.btn_upload.Text = "出力";
            this.btn_upload.UseVisualStyleBackColor = true;
            this.btn_upload.Click += new System.EventHandler(this.btn_upload_Click);
            // 
            // txt_bdvol
            // 
            this.txt_bdvol.Location = new System.Drawing.Point(111, 114);
            this.txt_bdvol.Name = "txt_bdvol";
            this.txt_bdvol.Size = new System.Drawing.Size(117, 19);
            this.txt_bdvol.TabIndex = 0;
            this.txt_bdvol.Text = "32";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(50, 117);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 12);
            this.label7.TabIndex = 1;
            this.label7.Text = "基板枚数:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btn_upload);
            this.groupBox1.Controls.Add(this.txt_retcode);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Location = new System.Drawing.Point(13, 178);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(215, 66);
            this.groupBox1.TabIndex = 3;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "上位通信要求";
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 1000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // FrmLmSim
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(241, 254);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btn_setdata);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_bdvol);
            this.Controls.Add(this.txt_matlot);
            this.Controls.Add(this.txt_matcode);
            this.Controls.Add(this.txt_typecd);
            this.Controls.Add(this.txt_magno);
            this.Name = "FrmLmSim";
            this.Text = "捺印機SIM";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_magno;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_typecd;
        private System.Windows.Forms.TextBox txt_matcode;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_matlot;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_retcode;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button btn_setdata;
        private System.Windows.Forms.Button btn_upload;
        private System.Windows.Forms.TextBox txt_bdvol;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Timer timer1;
    }
}