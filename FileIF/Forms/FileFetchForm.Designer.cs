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
            this.label3 = new System.Windows.Forms.Label();
            this.cmb_fetchfile = new System.Windows.Forms.ComboBox();
            this.btn_fetchfile = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.fetchConsole = new System.Windows.Forms.TextBox();
            this.tmpFileWatchTimer = new System.Windows.Forms.Timer(this.components);
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
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 80);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(74, 17);
            this.label3.TabIndex = 0;
            this.label3.Text = "取得ファイル";
            // 
            // cmb_fetchfile
            // 
            this.cmb_fetchfile.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmb_fetchfile.FormattingEnabled = true;
            this.cmb_fetchfile.Location = new System.Drawing.Point(96, 76);
            this.cmb_fetchfile.Name = "cmb_fetchfile";
            this.cmb_fetchfile.Size = new System.Drawing.Size(142, 25);
            this.cmb_fetchfile.TabIndex = 1;
            this.cmb_fetchfile.SelectedIndexChanged += new System.EventHandler(this.cmb_fetchfile_SelectedIndexChanged);
            // 
            // btn_fetchfile
            // 
            this.btn_fetchfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_fetchfile.Location = new System.Drawing.Point(267, 13);
            this.btn_fetchfile.Name = "btn_fetchfile";
            this.btn_fetchfile.Size = new System.Drawing.Size(142, 84);
            this.btn_fetchfile.TabIndex = 2;
            this.btn_fetchfile.Text = "ファイル取得";
            this.btn_fetchfile.UseVisualStyleBackColor = true;
            this.btn_fetchfile.Click += new System.EventHandler(this.btn_fetchfile_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(10, 114);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(96, 17);
            this.label4.TabIndex = 0;
            this.label4.Text = "ファイル取得結果";
            // 
            // fetchConsole
            // 
            this.fetchConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fetchConsole.Location = new System.Drawing.Point(12, 134);
            this.fetchConsole.Multiline = true;
            this.fetchConsole.Name = "fetchConsole";
            this.fetchConsole.Size = new System.Drawing.Size(400, 445);
            this.fetchConsole.TabIndex = 3;
            // 
            // tmpFileWatchTimer
            // 
            this.tmpFileWatchTimer.Interval = 2000;
            this.tmpFileWatchTimer.Tick += new System.EventHandler(this.tmpFileWatchTimer_Tick);
            // 
            // FileFetchForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(424, 591);
            this.Controls.Add(this.fetchConsole);
            this.Controls.Add(this.btn_fetchfile);
            this.Controls.Add(this.cmb_fetchfile);
            this.Controls.Add(this.cmb_macname);
            this.Controls.Add(this.cmb_pcat);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MinimumSize = new System.Drawing.Size(440, 630);
            this.Name = "FileFetchForm";
            this.Text = "FileFetch";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox cmb_pcat;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox cmb_macname;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.ComboBox cmb_fetchfile;
        private System.Windows.Forms.Button btn_fetchfile;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox fetchConsole;
        private System.Windows.Forms.Timer tmpFileWatchTimer;
    }
}