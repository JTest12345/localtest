namespace FileIf
{
    partial class FmVipFetchMac
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
            System.Windows.Forms.TreeNode treeNode3 = new System.Windows.Forms.TreeNode("ノード1");
            System.Windows.Forms.TreeNode treeNode4 = new System.Windows.Forms.TreeNode("テスト", new System.Windows.Forms.TreeNode[] {
            treeNode3});
            this.macTreeList = new System.Windows.Forms.TreeView();
            this.fetchConsole = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.btn_fetchfile = new System.Windows.Forms.Button();
            this.btn_AutoFetchFile = new System.Windows.Forms.Button();
            this.cbx_autofetchInterval = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.AutoFecthTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // macTreeList
            // 
            this.macTreeList.Location = new System.Drawing.Point(12, 24);
            this.macTreeList.Name = "macTreeList";
            treeNode3.Name = "ノード1";
            treeNode3.Text = "ノード1";
            treeNode4.Name = "ノード0";
            treeNode4.Text = "テスト";
            this.macTreeList.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            treeNode4});
            this.macTreeList.Size = new System.Drawing.Size(214, 125);
            this.macTreeList.TabIndex = 0;
            this.macTreeList.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeCollapse);
            this.macTreeList.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeView1_BeforeExpand);
            this.macTreeList.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeView1_Click);
            this.macTreeList.MouseDown += new System.Windows.Forms.MouseEventHandler(this.treeView1_MouseDown);
            // 
            // fetchConsole
            // 
            this.fetchConsole.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.fetchConsole.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fetchConsole.Location = new System.Drawing.Point(12, 180);
            this.fetchConsole.Multiline = true;
            this.fetchConsole.Name = "fetchConsole";
            this.fetchConsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.fetchConsole.Size = new System.Drawing.Size(385, 378);
            this.fetchConsole.TabIndex = 5;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 165);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(87, 12);
            this.label4.TabIndex = 4;
            this.label4.Text = "ファイル取得結果";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 12);
            this.label1.TabIndex = 6;
            this.label1.Text = "FTP対象設備";
            // 
            // btn_fetchfile
            // 
            this.btn_fetchfile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_fetchfile.Location = new System.Drawing.Point(237, 121);
            this.btn_fetchfile.Name = "btn_fetchfile";
            this.btn_fetchfile.Size = new System.Drawing.Size(160, 30);
            this.btn_fetchfile.TabIndex = 7;
            this.btn_fetchfile.Text = "いますぐファイル取得";
            this.btn_fetchfile.UseVisualStyleBackColor = true;
            this.btn_fetchfile.Click += new System.EventHandler(this.btn_fetchfile_Click);
            // 
            // btn_AutoFetchFile
            // 
            this.btn_AutoFetchFile.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btn_AutoFetchFile.Location = new System.Drawing.Point(237, 24);
            this.btn_AutoFetchFile.Name = "btn_AutoFetchFile";
            this.btn_AutoFetchFile.Size = new System.Drawing.Size(160, 54);
            this.btn_AutoFetchFile.TabIndex = 8;
            this.btn_AutoFetchFile.Text = "自動ファイル取得開始";
            this.btn_AutoFetchFile.UseVisualStyleBackColor = true;
            this.btn_AutoFetchFile.Click += new System.EventHandler(this.btn_AutoFetchFile_Click);
            // 
            // cbx_autofetchInterval
            // 
            this.cbx_autofetchInterval.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cbx_autofetchInterval.FormattingEnabled = true;
            this.cbx_autofetchInterval.Items.AddRange(new object[] {
            "10",
            "20",
            "30",
            "40",
            "50",
            "60"});
            this.cbx_autofetchInterval.Location = new System.Drawing.Point(316, 84);
            this.cbx_autofetchInterval.Name = "cbx_autofetchInterval";
            this.cbx_autofetchInterval.Size = new System.Drawing.Size(81, 20);
            this.cbx_autofetchInterval.TabIndex = 10;
            this.cbx_autofetchInterval.SelectedIndexChanged += new System.EventHandler(this.cbx_autofetchInterval_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(237, 87);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(73, 12);
            this.label2.TabIndex = 11;
            this.label2.Text = "取得周期(秒)";
            // 
            // AutoFecthTimer
            // 
            this.AutoFecthTimer.Interval = 30000;
            this.AutoFecthTimer.Tick += new System.EventHandler(this.AutoFecthTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 569);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(409, 22);
            this.statusStrip1.TabIndex = 12;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(125, 17);
            this.toolStripStatusLabel1.Text = "ファイル自動取得停止中";
            // 
            // FmVipFetchMac
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(409, 591);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.cbx_autofetchInterval);
            this.Controls.Add(this.btn_AutoFetchFile);
            this.Controls.Add(this.btn_fetchfile);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.fetchConsole);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.macTreeList);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(425, 630);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(425, 630);
            this.Name = "FmVipFetchMac";
            this.Text = "VIP Fetch File";
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView macTreeList;
        private System.Windows.Forms.TextBox fetchConsole;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btn_fetchfile;
        private System.Windows.Forms.Button btn_AutoFetchFile;
        private System.Windows.Forms.ComboBox cbx_autofetchInterval;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Timer AutoFecthTimer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
    }
}