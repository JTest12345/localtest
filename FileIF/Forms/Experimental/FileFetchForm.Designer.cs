namespace FileIf
{
    partial class FileFetchForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.cmb_pcat = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.cmb_macname = new System.Windows.Forms.ComboBox();
            this.cmb_fetchfile = new System.Windows.Forms.ComboBox();
            this.btn_fetchfile = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.tmpFileWatchTimer = new System.Windows.Forms.Timer(this.components);
            this.fetchConsole = new System.Windows.Forms.TextBox();
            this.rb_getLatestFile = new System.Windows.Forms.RadioButton();
            this.rb_getSelectedFile = new System.Windows.Forms.RadioButton();
            this.cbx_cleanFtpFold = new System.Windows.Forms.CheckBox();
            this.cbx_usebat = new System.Windows.Forms.CheckBox();
            this.cbx_autofetch = new System.Windows.Forms.CheckBox();
            this.cbx_autofetchInterval = new System.Windows.Forms.ComboBox();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.AutoFecthTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 17);
            this.label1.TabIndex = 0;
            this.label1.Text = "工程カテゴリ";
            // 
            // cmb_pcat
            // 
            this.cmb_pcat.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_pcat.FormattingEnabled = true;
            this.cmb_pcat.Location = new System.Drawing.Point(96, 11);
            this.cmb_pcat.Name = "cmb_pcat";
            this.cmb_pcat.Size = new System.Drawing.Size(142, 25);
            this.cmb_pcat.TabIndex = 1;
            this.cmb_pcat.SelectedIndexChanged += new System.EventHandler(this.cmb_pcat_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(10, 48);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(41, 17);
            this.label2.TabIndex = 0;
            this.label2.Text = "設備名";
            // 
            // cmb_macname
            // 
            this.cmb_macname.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_macname.FormattingEnabled = true;
            this.cmb_macname.Location = new System.Drawing.Point(96, 43);
            this.cmb_macname.Name = "cmb_macname";
            this.cmb_macname.Size = new System.Drawing.Size(142, 25);
            this.cmb_macname.TabIndex = 1;
            this.cmb_macname.SelectionChangeCommitted += new System.EventHandler(this.cmb_macname_SelectionChangeCommitted);
            // 
            // cmb_fetchfile
            // 
            this.cmb_fetchfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_fetchfile.FormattingEnabled = true;
            this.cmb_fetchfile.Location = new System.Drawing.Point(176, 158);
            this.cmb_fetchfile.Name = "cmb_fetchfile";
            this.cmb_fetchfile.Size = new System.Drawing.Size(142, 25);
            this.cmb_fetchfile.TabIndex = 1;
            this.cmb_fetchfile.SelectedIndexChanged += new System.EventHandler(this.cmb_fetchfile_SelectedIndexChanged);
            // 
            // btn_fetchfile
            // 
            this.btn_fetchfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_fetchfile.Location = new System.Drawing.Point(270, 11);
            this.btn_fetchfile.Name = "btn_fetchfile";
            this.btn_fetchfile.Size = new System.Drawing.Size(142, 57);
            this.btn_fetchfile.TabIndex = 2;
            this.btn_fetchfile.Text = "ファイル取得";
            this.btn_fetchfile.UseVisualStyleBackColor = true;
            this.btn_fetchfile.Click += new System.EventHandler(this.btn_fetchfile_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 223);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "ファイル取得結果";
            // 
            // tmpFileWatchTimer
            // 
            this.tmpFileWatchTimer.Interval = 2000;
            this.tmpFileWatchTimer.Tick += new System.EventHandler(this.tmpFileWatchTimer_Tick);
            // 
            // fetchConsole
            // 
            this.fetchConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fetchConsole.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fetchConsole.Location = new System.Drawing.Point(12, 243);
            this.fetchConsole.Multiline = true;
            this.fetchConsole.Name = "fetchConsole";
            this.fetchConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fetchConsole.Size = new System.Drawing.Size(400, 317);
            this.fetchConsole.TabIndex = 3;
            // 
            // rb_getLatestFile
            // 
            this.rb_getLatestFile.AutoSize = true;
            this.rb_getLatestFile.Location = new System.Drawing.Point(12, 133);
            this.rb_getLatestFile.Name = "rb_getLatestFile";
            this.rb_getLatestFile.Size = new System.Drawing.Size(136, 21);
            this.rb_getLatestFile.TabIndex = 4;
            this.rb_getLatestFile.Text = "最新のファイルを取得";
            this.rb_getLatestFile.UseVisualStyleBackColor = true;
            this.rb_getLatestFile.CheckedChanged += new System.EventHandler(this.getfileradio_CheckedChanged);
            // 
            // rb_getSelectedFile
            // 
            this.rb_getSelectedFile.AutoSize = true;
            this.rb_getSelectedFile.Location = new System.Drawing.Point(12, 158);
            this.rb_getSelectedFile.Name = "rb_getSelectedFile";
            this.rb_getSelectedFile.Size = new System.Drawing.Size(158, 21);
            this.rb_getSelectedFile.TabIndex = 5;
            this.rb_getSelectedFile.Text = "ファイル名を指定して取得";
            this.rb_getSelectedFile.UseVisualStyleBackColor = true;
            this.rb_getSelectedFile.CheckedChanged += new System.EventHandler(this.getfileradio_CheckedChanged);
            // 
            // cbx_cleanFtpFold
            // 
            this.cbx_cleanFtpFold.AutoSize = true;
            this.cbx_cleanFtpFold.Location = new System.Drawing.Point(12, 189);
            this.cbx_cleanFtpFold.Name = "cbx_cleanFtpFold";
            this.cbx_cleanFtpFold.Size = new System.Drawing.Size(388, 21);
            this.cbx_cleanFtpFold.TabIndex = 6;
            this.cbx_cleanFtpFold.Text = "ファイル取得が成功した場合、FTPフォルダ内のファイルを全て削除する";
            this.cbx_cleanFtpFold.UseVisualStyleBackColor = true;
            // 
            // cbx_usebat
            // 
            this.cbx_usebat.AutoSize = true;
            this.cbx_usebat.Location = new System.Drawing.Point(12, 79);
            this.cbx_usebat.Name = "cbx_usebat";
            this.cbx_usebat.Size = new System.Drawing.Size(317, 21);
            this.cbx_usebat.TabIndex = 7;
            this.cbx_usebat.Text = "ftpスクリプトファイルによる規定動作にてファイルを取得";
            this.cbx_usebat.UseVisualStyleBackColor = true;
            // 
            // cbx_autofetch
            // 
            this.cbx_autofetch.AutoSize = true;
            this.cbx_autofetch.Location = new System.Drawing.Point(12, 106);
            this.cbx_autofetch.Name = "cbx_autofetch";
            this.cbx_autofetch.Size = new System.Drawing.Size(180, 21);
            this.cbx_autofetch.TabIndex = 8;
            this.cbx_autofetch.Text = "ファイルを自動取得：周期(分)";
            this.cbx_autofetch.UseVisualStyleBackColor = true;
            this.cbx_autofetch.CheckedChanged += new System.EventHandler(this.cbx_autofetch_CheckedChanged);
            // 
            // cbx_autofetchInterval
            // 
            this.cbx_autofetchInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_autofetchInterval.FormattingEnabled = true;
            this.cbx_autofetchInterval.Items.AddRange(new object[] {
            "1",
            "2",
            "3"});
            this.cbx_autofetchInterval.Location = new System.Drawing.Point(198, 102);
            this.cbx_autofetchInterval.Name = "cbx_autofetchInterval";
            this.cbx_autofetchInterval.Size = new System.Drawing.Size(70, 25);
            this.cbx_autofetchInterval.TabIndex = 9;
            this.cbx_autofetchInterval.SelectedIndexChanged += new System.EventHandler(this.comboBox1_SelectedIndexChanged);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 569);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(424, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(125, 17);
            this.toolStripStatusLabel1.Text = "ファイル自動取得停止中";
            // 
            // AutoFecthTimer
            // 
            this.AutoFecthTimer.Interval = 1000;
            this.AutoFecthTimer.Tick += new System.EventHandler(this.AutoFecthTimer_Tick);
            // 
            // FileFetchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 591);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.cbx_autofetchInterval);
            this.Controls.Add(this.cbx_autofetch);
            this.Controls.Add(this.cbx_usebat);
            this.Controls.Add(this.cbx_cleanFtpFold);
            this.Controls.Add(this.rb_getSelectedFile);
            this.Controls.Add(this.rb_getLatestFile);
            this.Controls.Add(this.fetchConsole);
            this.Controls.Add(this.btn_fetchfile);
            this.Controls.Add(this.cmb_fetchfile);
            this.Controls.Add(this.cmb_macname);
            this.Controls.Add(this.cmb_pcat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(440, 630);
            this.Name = "FileFetchForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FileFetch";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_pcat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_macname;
        private System.Windows.Forms.ComboBox cmb_fetchfile;
        private System.Windows.Forms.Button btn_fetchfile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Timer tmpFileWatchTimer;
        private System.Windows.Forms.TextBox fetchConsole;
        private System.Windows.Forms.RadioButton rb_getLatestFile;
        private System.Windows.Forms.RadioButton rb_getSelectedFile;
        private System.Windows.Forms.CheckBox cbx_cleanFtpFold;
        private System.Windows.Forms.CheckBox cbx_usebat;
        private System.Windows.Forms.CheckBox cbx_autofetch;
        private System.Windows.Forms.ComboBox cbx_autofetchInterval;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer AutoFecthTimer;
    }
}