namespace FIFDebugTools
{
    partial class ArmsTranController
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
            this.debugMonitor = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label39 = new System.Windows.Forms.Label();
            this.label17 = new System.Windows.Forms.Label();
            this.txt_dbcnt_lotno = new System.Windows.Forms.TextBox();
            this.btn_inslot = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txt_bdqty = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txt_magno = new System.Windows.Forms.TextBox();
            this.txt_debugmagheader = new System.Windows.Forms.TextBox();
            this.txt_jushicd = new System.Windows.Forms.TextBox();
            this.txt_blendcd = new System.Windows.Forms.TextBox();
            this.txt_profileno = new System.Windows.Forms.TextBox();
            this.cmb_typecd = new System.Windows.Forms.ComboBox();
            this.btn_deltrans = new System.Windows.Forms.Button();
            this.btn_dellot = new System.Windows.Forms.Button();
            this.btn_instarans = new System.Windows.Forms.Button();
            this.txt_processname = new System.Windows.Forms.TextBox();
            this.label35 = new System.Windows.Forms.Label();
            this.label22 = new System.Windows.Forms.Label();
            this.btn_reset_proc = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.btn_inc_proc = new System.Windows.Forms.Button();
            this.cb_procno = new System.Windows.Forms.ComboBox();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // debugMonitor
            // 
            this.debugMonitor.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.debugMonitor.Location = new System.Drawing.Point(9, 229);
            this.debugMonitor.MaxLength = 0;
            this.debugMonitor.Multiline = true;
            this.debugMonitor.Name = "debugMonitor";
            this.debugMonitor.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.debugMonitor.Size = new System.Drawing.Size(463, 350);
            this.debugMonitor.TabIndex = 4;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 214);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(112, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "データベース操作モニタ";
            // 
            // label39
            // 
            this.label39.AutoSize = true;
            this.label39.Location = new System.Drawing.Point(6, 25);
            this.label39.Name = "label39";
            this.label39.Size = new System.Drawing.Size(42, 12);
            this.label39.TabIndex = 56;
            this.label39.Text = "Typecd";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(8, 49);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(37, 12);
            this.label17.TabIndex = 56;
            this.label17.Text = "LotNo.";
            // 
            // txt_dbcnt_lotno
            // 
            this.txt_dbcnt_lotno.Location = new System.Drawing.Point(54, 47);
            this.txt_dbcnt_lotno.Name = "txt_dbcnt_lotno";
            this.txt_dbcnt_lotno.Size = new System.Drawing.Size(221, 19);
            this.txt_dbcnt_lotno.TabIndex = 55;
            // 
            // btn_inslot
            // 
            this.btn_inslot.Location = new System.Drawing.Point(168, 72);
            this.btn_inslot.Name = "btn_inslot";
            this.btn_inslot.Size = new System.Drawing.Size(107, 20);
            this.btn_inslot.TabIndex = 2;
            this.btn_inslot.Text = "ロット挿入";
            this.btn_inslot.UseVisualStyleBackColor = true;
            this.btn_inslot.Click += new System.EventHandler(this.btn_inslot_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label6);
            this.groupBox6.Controls.Add(this.label7);
            this.groupBox6.Controls.Add(this.label2);
            this.groupBox6.Controls.Add(this.label5);
            this.groupBox6.Controls.Add(this.label1);
            this.groupBox6.Controls.Add(this.txt_bdqty);
            this.groupBox6.Controls.Add(this.label3);
            this.groupBox6.Controls.Add(this.txt_magno);
            this.groupBox6.Controls.Add(this.txt_debugmagheader);
            this.groupBox6.Controls.Add(this.txt_jushicd);
            this.groupBox6.Controls.Add(this.txt_blendcd);
            this.groupBox6.Controls.Add(this.txt_profileno);
            this.groupBox6.Controls.Add(this.cmb_typecd);
            this.groupBox6.Controls.Add(this.btn_deltrans);
            this.groupBox6.Controls.Add(this.btn_dellot);
            this.groupBox6.Controls.Add(this.btn_instarans);
            this.groupBox6.Controls.Add(this.btn_inslot);
            this.groupBox6.Controls.Add(this.txt_dbcnt_lotno);
            this.groupBox6.Controls.Add(this.label17);
            this.groupBox6.Controls.Add(this.label39);
            this.groupBox6.Controls.Add(this.txt_processname);
            this.groupBox6.Controls.Add(this.label35);
            this.groupBox6.Controls.Add(this.label22);
            this.groupBox6.Controls.Add(this.btn_reset_proc);
            this.groupBox6.Controls.Add(this.button1);
            this.groupBox6.Controls.Add(this.btn_inc_proc);
            this.groupBox6.Controls.Add(this.cb_procno);
            this.groupBox6.Location = new System.Drawing.Point(10, 12);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(462, 192);
            this.groupBox6.TabIndex = 54;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "ダミーデータ挿入／削除";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(286, 164);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(53, 12);
            this.label6.TabIndex = 70;
            this.label6.Text = "基板枚数";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(286, 138);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(70, 12);
            this.label7.TabIndex = 70;
            this.label7.Text = "Load MagNo.";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(286, 111);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(74, 12);
            this.label2.TabIndex = 71;
            this.label2.Text = "Mag接頭文字";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(286, 83);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(45, 12);
            this.label5.TabIndex = 72;
            this.label5.Text = "樹脂CD";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(286, 54);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 12);
            this.label1.TabIndex = 73;
            this.label1.Text = "ブレンドCD";
            // 
            // txt_bdqty
            // 
            this.txt_bdqty.Location = new System.Drawing.Point(362, 160);
            this.txt_bdqty.Name = "txt_bdqty";
            this.txt_bdqty.Size = new System.Drawing.Size(90, 19);
            this.txt_bdqty.TabIndex = 65;
            this.txt_bdqty.Text = "25";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(287, 26);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(57, 12);
            this.label3.TabIndex = 74;
            this.label3.Text = "プロファイル";
            // 
            // txt_magno
            // 
            this.txt_magno.Location = new System.Drawing.Point(362, 134);
            this.txt_magno.Name = "txt_magno";
            this.txt_magno.Size = new System.Drawing.Size(90, 19);
            this.txt_magno.TabIndex = 65;
            this.txt_magno.Text = "00001";
            // 
            // txt_debugmagheader
            // 
            this.txt_debugmagheader.Location = new System.Drawing.Point(362, 107);
            this.txt_debugmagheader.Name = "txt_debugmagheader";
            this.txt_debugmagheader.Size = new System.Drawing.Size(90, 19);
            this.txt_debugmagheader.TabIndex = 66;
            // 
            // txt_jushicd
            // 
            this.txt_jushicd.Enabled = false;
            this.txt_jushicd.Location = new System.Drawing.Point(362, 79);
            this.txt_jushicd.Name = "txt_jushicd";
            this.txt_jushicd.Size = new System.Drawing.Size(90, 19);
            this.txt_jushicd.TabIndex = 67;
            this.txt_jushicd.Text = "NA";
            // 
            // txt_blendcd
            // 
            this.txt_blendcd.Enabled = false;
            this.txt_blendcd.Location = new System.Drawing.Point(362, 50);
            this.txt_blendcd.Name = "txt_blendcd";
            this.txt_blendcd.Size = new System.Drawing.Size(90, 19);
            this.txt_blendcd.TabIndex = 68;
            this.txt_blendcd.Text = "NA";
            // 
            // txt_profileno
            // 
            this.txt_profileno.Enabled = false;
            this.txt_profileno.Location = new System.Drawing.Point(362, 22);
            this.txt_profileno.Name = "txt_profileno";
            this.txt_profileno.Size = new System.Drawing.Size(90, 19);
            this.txt_profileno.TabIndex = 69;
            this.txt_profileno.Text = "1";
            // 
            // cmb_typecd
            // 
            this.cmb_typecd.FormattingEnabled = true;
            this.cmb_typecd.Items.AddRange(new object[] {
            "CLUHANKAN-SIM",
            "CLUKANSEI-SIM"});
            this.cmb_typecd.Location = new System.Drawing.Point(54, 21);
            this.cmb_typecd.Name = "cmb_typecd";
            this.cmb_typecd.Size = new System.Drawing.Size(221, 20);
            this.cmb_typecd.TabIndex = 58;
            this.cmb_typecd.SelectedIndexChanged += new System.EventHandler(this.cmb_typecd_SelectedIndexChanged);
            // 
            // btn_deltrans
            // 
            this.btn_deltrans.Location = new System.Drawing.Point(54, 161);
            this.btn_deltrans.Name = "btn_deltrans";
            this.btn_deltrans.Size = new System.Drawing.Size(74, 21);
            this.btn_deltrans.TabIndex = 57;
            this.btn_deltrans.Text = "実績削除";
            this.btn_deltrans.UseVisualStyleBackColor = true;
            this.btn_deltrans.Click += new System.EventHandler(this.btn_deltrans_Click);
            // 
            // btn_dellot
            // 
            this.btn_dellot.Location = new System.Drawing.Point(54, 74);
            this.btn_dellot.Name = "btn_dellot";
            this.btn_dellot.Size = new System.Drawing.Size(74, 21);
            this.btn_dellot.TabIndex = 57;
            this.btn_dellot.Text = "ロット削除";
            this.btn_dellot.UseVisualStyleBackColor = true;
            this.btn_dellot.Click += new System.EventHandler(this.btn_dellot_Click);
            // 
            // btn_instarans
            // 
            this.btn_instarans.Enabled = false;
            this.btn_instarans.Location = new System.Drawing.Point(168, 162);
            this.btn_instarans.Name = "btn_instarans";
            this.btn_instarans.Size = new System.Drawing.Size(107, 20);
            this.btn_instarans.TabIndex = 2;
            this.btn_instarans.Text = "デバック準備";
            this.btn_instarans.UseVisualStyleBackColor = true;
            this.btn_instarans.Click += new System.EventHandler(this.btn_instarans_Click);
            // 
            // txt_processname
            // 
            this.txt_processname.Enabled = false;
            this.txt_processname.Location = new System.Drawing.Point(54, 134);
            this.txt_processname.Name = "txt_processname";
            this.txt_processname.Size = new System.Drawing.Size(153, 19);
            this.txt_processname.TabIndex = 54;
            // 
            // label35
            // 
            this.label35.AutoSize = true;
            this.label35.Location = new System.Drawing.Point(6, 131);
            this.label35.Name = "label35";
            this.label35.Size = new System.Drawing.Size(41, 12);
            this.label35.TabIndex = 4;
            this.label35.Text = "工程名";
            // 
            // label22
            // 
            this.label22.AutoSize = true;
            this.label22.Location = new System.Drawing.Point(6, 109);
            this.label22.Name = "label22";
            this.label22.Size = new System.Drawing.Size(39, 12);
            this.label22.TabIndex = 4;
            this.label22.Text = "procno";
            // 
            // btn_reset_proc
            // 
            this.btn_reset_proc.Location = new System.Drawing.Point(215, 130);
            this.btn_reset_proc.Name = "btn_reset_proc";
            this.btn_reset_proc.Size = new System.Drawing.Size(60, 23);
            this.btn_reset_proc.TabIndex = 1;
            this.btn_reset_proc.Text = "Reset";
            this.btn_reset_proc.UseVisualStyleBackColor = true;
            this.btn_reset_proc.Click += new System.EventHandler(this.btn_reset_proc_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(215, 104);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(27, 23);
            this.button1.TabIndex = 1;
            this.button1.Text = "<";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.btn_dec_proc_Click);
            // 
            // btn_inc_proc
            // 
            this.btn_inc_proc.Location = new System.Drawing.Point(248, 104);
            this.btn_inc_proc.Name = "btn_inc_proc";
            this.btn_inc_proc.Size = new System.Drawing.Size(27, 23);
            this.btn_inc_proc.TabIndex = 1;
            this.btn_inc_proc.Text = ">";
            this.btn_inc_proc.UseVisualStyleBackColor = true;
            this.btn_inc_proc.Click += new System.EventHandler(this.btn_inc_proc_Click);
            // 
            // cb_procno
            // 
            this.cb_procno.FormattingEnabled = true;
            this.cb_procno.Location = new System.Drawing.Point(54, 106);
            this.cb_procno.Name = "cb_procno";
            this.cb_procno.Size = new System.Drawing.Size(153, 20);
            this.cb_procno.TabIndex = 0;
            this.cb_procno.SelectedIndexChanged += new System.EventHandler(this.cb_procno_SelectedIndexChanged);
            // 
            // ArmsTranController
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(484, 591);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.debugMonitor);
            this.MaximumSize = new System.Drawing.Size(500, 1200);
            this.Name = "ArmsTranController";
            this.Text = "ArmsTranController";
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox debugMonitor;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btn_inslot;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.Button btn_reset_proc;
        private System.Windows.Forms.Button btn_inc_proc;
        private System.Windows.Forms.ComboBox cb_procno;
        private System.Windows.Forms.Label label35;
        private System.Windows.Forms.TextBox txt_processname;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txt_dbcnt_lotno;
        private System.Windows.Forms.Label label39;
        private System.Windows.Forms.Button btn_dellot;
        private System.Windows.Forms.ComboBox cmb_typecd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txt_magno;
        private System.Windows.Forms.TextBox txt_debugmagheader;
        private System.Windows.Forms.TextBox txt_jushicd;
        private System.Windows.Forms.TextBox txt_blendcd;
        private System.Windows.Forms.TextBox txt_profileno;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txt_bdqty;
        private System.Windows.Forms.Button btn_deltrans;
        private System.Windows.Forms.Button btn_instarans;
    }
}