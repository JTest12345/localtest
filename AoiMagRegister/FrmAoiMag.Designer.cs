namespace AoiMagBuilder
{
    partial class FrmAoiMag
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
            this.txt_magno = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_typecd = new System.Windows.Forms.TextBox();
            this.txt_matcode = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_matlot = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.btn_setdata = new System.Windows.Forms.Button();
            this.txt_bdvol = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.btn_clearTxtBox = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.checkBox3 = new System.Windows.Forms.CheckBox();
            this.checkBox4 = new System.Windows.Forms.CheckBox();
            this.checkBox5 = new System.Windows.Forms.CheckBox();
            this.label5 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_magno
            // 
            this.txt_magno.Location = new System.Drawing.Point(296, 91);
            this.txt_magno.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_magno.Name = "txt_magno";
            this.txt_magno.Size = new System.Drawing.Size(259, 25);
            this.txt_magno.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(196, 93);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(84, 18);
            this.label1.TabIndex = 1;
            this.label1.Text = "*マガジンNo:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(231, 166);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(49, 18);
            this.label2.TabIndex = 1;
            this.label2.Text = "製品名:";
            // 
            // txt_typecd
            // 
            this.txt_typecd.Location = new System.Drawing.Point(296, 163);
            this.txt_typecd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_typecd.Name = "txt_typecd";
            this.txt_typecd.Size = new System.Drawing.Size(259, 25);
            this.txt_typecd.TabIndex = 5;
            // 
            // txt_matcode
            // 
            this.txt_matcode.Location = new System.Drawing.Point(296, 16);
            this.txt_matcode.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_matcode.Name = "txt_matcode";
            this.txt_matcode.Size = new System.Drawing.Size(259, 25);
            this.txt_matcode.TabIndex = 1;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(176, 22);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 18);
            this.label3.TabIndex = 1;
            this.label3.Text = "*基板部材コード:";
            // 
            // txt_matlot
            // 
            this.txt_matlot.Location = new System.Drawing.Point(296, 54);
            this.txt_matlot.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_matlot.Name = "txt_matlot";
            this.txt_matlot.Size = new System.Drawing.Size(259, 25);
            this.txt_matlot.TabIndex = 2;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(208, 57);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(72, 18);
            this.label4.TabIndex = 1;
            this.label4.Text = "*ロットNo:";
            // 
            // btn_setdata
            // 
            this.btn_setdata.Location = new System.Drawing.Point(618, 59);
            this.btn_setdata.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_setdata.Name = "btn_setdata";
            this.btn_setdata.Size = new System.Drawing.Size(138, 79);
            this.btn_setdata.TabIndex = 6;
            this.btn_setdata.Text = "データベース転送";
            this.btn_setdata.UseVisualStyleBackColor = true;
            this.btn_setdata.Click += new System.EventHandler(this.btn_setdata_Click);
            // 
            // txt_bdvol
            // 
            this.txt_bdvol.Location = new System.Drawing.Point(296, 128);
            this.txt_bdvol.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_bdvol.Name = "txt_bdvol";
            this.txt_bdvol.Size = new System.Drawing.Size(259, 25);
            this.txt_bdvol.TabIndex = 4;
            this.txt_bdvol.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.txt_bdvol_KeyPress_1);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(212, 131);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(68, 18);
            this.label7.TabIndex = 1;
            this.label7.Text = "*基板枚数:";
            // 
            // btn_clearTxtBox
            // 
            this.btn_clearTxtBox.Location = new System.Drawing.Point(24, 128);
            this.btn_clearTxtBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btn_clearTxtBox.Name = "btn_clearTxtBox";
            this.btn_clearTxtBox.Size = new System.Drawing.Size(138, 47);
            this.btn_clearTxtBox.TabIndex = 0;
            this.btn_clearTxtBox.Text = "クリア";
            this.btn_clearTxtBox.UseVisualStyleBackColor = true;
            this.btn_clearTxtBox.Click += new System.EventHandler(this.btn_clearTxtBox_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(566, 22);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(15, 14);
            this.checkBox1.TabIndex = 7;
            this.checkBox1.UseVisualStyleBackColor = true;
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(566, 59);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(15, 14);
            this.checkBox2.TabIndex = 8;
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // checkBox3
            // 
            this.checkBox3.AutoSize = true;
            this.checkBox3.Location = new System.Drawing.Point(566, 97);
            this.checkBox3.Name = "checkBox3";
            this.checkBox3.Size = new System.Drawing.Size(15, 14);
            this.checkBox3.TabIndex = 9;
            this.checkBox3.UseVisualStyleBackColor = true;
            // 
            // checkBox4
            // 
            this.checkBox4.AutoSize = true;
            this.checkBox4.Location = new System.Drawing.Point(566, 135);
            this.checkBox4.Name = "checkBox4";
            this.checkBox4.Size = new System.Drawing.Size(15, 14);
            this.checkBox4.TabIndex = 10;
            this.checkBox4.UseVisualStyleBackColor = true;
            // 
            // checkBox5
            // 
            this.checkBox5.AutoSize = true;
            this.checkBox5.Location = new System.Drawing.Point(566, 169);
            this.checkBox5.Name = "checkBox5";
            this.checkBox5.Size = new System.Drawing.Size(15, 14);
            this.checkBox5.TabIndex = 11;
            this.checkBox5.UseVisualStyleBackColor = true;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("メイリオ", 6.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(563, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(34, 14);
            this.label5.TabIndex = 1;
            this.label5.Text = "クリア";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 22);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(138, 70);
            this.button1.TabIndex = 12;
            this.button1.Text = "基板4Mコード読込";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 201);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(774, 22);
            this.statusStrip1.TabIndex = 13;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // FrmAoiMag
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(774, 223);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.checkBox5);
            this.Controls.Add(this.checkBox4);
            this.Controls.Add(this.checkBox3);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btn_clearTxtBox);
            this.Controls.Add(this.btn_setdata);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_bdvol);
            this.Controls.Add(this.txt_matlot);
            this.Controls.Add(this.txt_matcode);
            this.Controls.Add(this.txt_typecd);
            this.Controls.Add(this.txt_magno);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(790, 262);
            this.MinimumSize = new System.Drawing.Size(790, 262);
            this.Name = "FrmAoiMag";
            this.Text = "AOI Builder";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmAoiMag_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
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
        private System.Windows.Forms.Button btn_setdata;
        private System.Windows.Forms.TextBox txt_bdvol;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button btn_clearTxtBox;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.CheckBox checkBox2;
        private System.Windows.Forms.CheckBox checkBox3;
        private System.Windows.Forms.CheckBox checkBox4;
        private System.Windows.Forms.CheckBox checkBox5;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}