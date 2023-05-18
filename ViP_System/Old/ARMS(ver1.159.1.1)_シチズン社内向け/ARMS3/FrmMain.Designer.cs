namespace ARMS3
{
    partial class FrmMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMain));
            this.treeMacList = new System.Windows.Forms.TreeView();
            this.menuMachine = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemMacStart = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMacStop = new System.Windows.Forms.ToolStripMenuItem();
            this.itemMacMagDelete = new System.Windows.Forms.ToolStripMenuItem();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.txtApiLog = new System.Windows.Forms.TextBox();
            this.txtLog = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.lblVersion = new System.Windows.Forms.ToolStripLabel();
            this.btnStartAll = new System.Windows.Forms.ToolStripButton();
            this.btnStopReqAll = new System.Windows.Forms.ToolStripButton();
            this.grbMachine = new System.Windows.Forms.GroupBox();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusMachineThread = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCarrierLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.statusCarrierThread = new System.Windows.Forms.ToolStripStatusLabel();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.treeCarList = new System.Windows.Forms.TreeView();
            this.menuCarrier = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.itemCarStart = new System.Windows.Forms.ToolStripMenuItem();
            this.itemCarStop = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnChangeCVWait = new System.Windows.Forms.ToolStripButton();
            this.txtCVWaitMinute = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.btnTestMode = new System.Windows.Forms.ToolStripButton();
            this.btnSignalDisp = new System.Windows.Forms.ToolStripButton();
            this.menuMachine.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.toolStrip.SuspendLayout();
            this.grbMachine.SuspendLayout();
            this.statusStrip.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.menuCarrier.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeMacList
            // 
            this.treeMacList.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.treeMacList.ContextMenuStrip = this.menuMachine;
            this.treeMacList.Location = new System.Drawing.Point(4, 16);
            this.treeMacList.Margin = new System.Windows.Forms.Padding(4);
            this.treeMacList.Name = "treeMacList";
            this.treeMacList.Size = new System.Drawing.Size(265, 360);
            this.treeMacList.TabIndex = 2;
            // 
            // menuMachine
            // 
            this.menuMachine.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemMacStart,
            this.itemMacStop,
            this.itemMacMagDelete});
            this.menuMachine.Name = "menuMachine";
            this.menuMachine.Size = new System.Drawing.Size(173, 70);
            this.menuMachine.Opening += new System.ComponentModel.CancelEventHandler(this.menuMachine_Opening);
            // 
            // itemMacStart
            // 
            this.itemMacStart.Image = ((System.Drawing.Image)(resources.GetObject("itemMacStart.Image")));
            this.itemMacStart.Name = "itemMacStart";
            this.itemMacStart.Size = new System.Drawing.Size(172, 22);
            this.itemMacStart.Text = "開始";
            this.itemMacStart.Click += new System.EventHandler(this.itemMacStart_Click);
            // 
            // itemMacStop
            // 
            this.itemMacStop.Image = ((System.Drawing.Image)(resources.GetObject("itemMacStop.Image")));
            this.itemMacStop.Name = "itemMacStop";
            this.itemMacStop.Size = new System.Drawing.Size(172, 22);
            this.itemMacStop.Text = "停止";
            this.itemMacStop.Click += new System.EventHandler(this.itemMacStop_Click);
            // 
            // itemMacMagDelete
            // 
            this.itemMacMagDelete.Image = ((System.Drawing.Image)(resources.GetObject("itemMacMagDelete.Image")));
            this.itemMacMagDelete.Name = "itemMacMagDelete";
            this.itemMacMagDelete.Size = new System.Drawing.Size(172, 22);
            this.itemMacMagDelete.Text = "仮想マガジン削除";
            this.itemMacMagDelete.Click += new System.EventHandler(this.itemMacMagDelete_Click);
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 2000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // txtApiLog
            // 
            this.txtApiLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtApiLog.Location = new System.Drawing.Point(4, 298);
            this.txtApiLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtApiLog.Multiline = true;
            this.txtApiLog.Name = "txtApiLog";
            this.txtApiLog.ReadOnly = true;
            this.txtApiLog.Size = new System.Drawing.Size(583, 231);
            this.txtApiLog.TabIndex = 4;
            // 
            // txtLog
            // 
            this.txtLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.txtLog.Location = new System.Drawing.Point(4, 4);
            this.txtLog.Margin = new System.Windows.Forms.Padding(4);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(583, 286);
            this.txtLog.TabIndex = 3;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.tableLayoutPanel1);
            this.groupBox1.Location = new System.Drawing.Point(294, 29);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox1.Size = new System.Drawing.Size(599, 553);
            this.groupBox1.TabIndex = 11;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "動作ログ";
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.Controls.Add(this.txtApiLog, 0, 1);
            this.tableLayoutPanel1.Controls.Add(this.txtLog, 0, 0);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(4, 16);
            this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(4);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 2;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 55.17891F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 44.82109F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(591, 533);
            this.tableLayoutPanel1.TabIndex = 4;
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblVersion,
            this.btnStartAll,
            this.btnStopReqAll});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(906, 25);
            this.toolStrip.TabIndex = 13;
            this.toolStrip.Text = "toolStrip1";
            // 
            // lblVersion
            // 
            this.lblVersion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(29, 22);
            this.lblVersion.Text = "ver.";
            // 
            // btnStartAll
            // 
            this.btnStartAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStartAll.Name = "btnStartAll";
            this.btnStartAll.Size = new System.Drawing.Size(36, 22);
            this.btnStartAll.Text = "開始";
            this.btnStartAll.Click += new System.EventHandler(this.btnStartAll_Click);
            // 
            // btnStopReqAll
            // 
            this.btnStopReqAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnStopReqAll.Name = "btnStopReqAll";
            this.btnStopReqAll.Size = new System.Drawing.Size(60, 22);
            this.btnStopReqAll.Text = "通常停止";
            this.btnStopReqAll.Click += new System.EventHandler(this.btnStopReqAll_Click);
            // 
            // grbMachine
            // 
            this.grbMachine.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.grbMachine.Controls.Add(this.treeMacList);
            this.grbMachine.Location = new System.Drawing.Point(13, 202);
            this.grbMachine.Margin = new System.Windows.Forms.Padding(4);
            this.grbMachine.Name = "grbMachine";
            this.grbMachine.Padding = new System.Windows.Forms.Padding(4);
            this.grbMachine.Size = new System.Drawing.Size(273, 380);
            this.grbMachine.TabIndex = 12;
            this.grbMachine.TabStop = false;
            this.grbMachine.Text = "装置リスト";
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.statusMachineThread,
            this.statusCarrierLabel,
            this.statusCarrierThread});
            this.statusStrip.Location = new System.Drawing.Point(0, 611);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Size = new System.Drawing.Size(906, 23);
            this.statusStrip.TabIndex = 14;
            this.statusStrip.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(56, 18);
            this.toolStripStatusLabel1.Text = "装置監視";
            // 
            // statusMachineThread
            // 
            this.statusMachineThread.ForeColor = System.Drawing.Color.Red;
            this.statusMachineThread.Name = "statusMachineThread";
            this.statusMachineThread.Size = new System.Drawing.Size(32, 18);
            this.statusMachineThread.Text = "停止";
            // 
            // statusCarrierLabel
            // 
            this.statusCarrierLabel.Name = "statusCarrierLabel";
            this.statusCarrierLabel.Size = new System.Drawing.Size(68, 18);
            this.statusCarrierLabel.Text = "搬送機操作";
            // 
            // statusCarrierThread
            // 
            this.statusCarrierThread.ForeColor = System.Drawing.Color.Red;
            this.statusCarrierThread.Name = "statusCarrierThread";
            this.statusCarrierThread.Size = new System.Drawing.Size(32, 18);
            this.statusCarrierThread.Text = "停止";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.treeCarList);
            this.groupBox3.Location = new System.Drawing.Point(13, 29);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(4);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(4);
            this.groupBox3.Size = new System.Drawing.Size(273, 165);
            this.groupBox3.TabIndex = 15;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "搬送機リスト";
            // 
            // treeCarList
            // 
            this.treeCarList.ContextMenuStrip = this.menuCarrier;
            this.treeCarList.Dock = System.Windows.Forms.DockStyle.Fill;
            this.treeCarList.Location = new System.Drawing.Point(4, 16);
            this.treeCarList.Margin = new System.Windows.Forms.Padding(4);
            this.treeCarList.Name = "treeCarList";
            this.treeCarList.Size = new System.Drawing.Size(265, 145);
            this.treeCarList.TabIndex = 2;
            // 
            // menuCarrier
            // 
            this.menuCarrier.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.itemCarStart,
            this.itemCarStop});
            this.menuCarrier.Name = "menuMachine";
            this.menuCarrier.Size = new System.Drawing.Size(101, 48);
            this.menuCarrier.Opening += new System.ComponentModel.CancelEventHandler(this.menuCarrier_Opening);
            // 
            // itemCarStart
            // 
            this.itemCarStart.Image = ((System.Drawing.Image)(resources.GetObject("itemCarStart.Image")));
            this.itemCarStart.Name = "itemCarStart";
            this.itemCarStart.Size = new System.Drawing.Size(100, 22);
            this.itemCarStart.Text = "開始";
            this.itemCarStart.Click += new System.EventHandler(this.itemCarStart_Click);
            // 
            // itemCarStop
            // 
            this.itemCarStop.Image = ((System.Drawing.Image)(resources.GetObject("itemCarStop.Image")));
            this.itemCarStop.Name = "itemCarStop";
            this.itemCarStop.Size = new System.Drawing.Size(100, 22);
            this.itemCarStop.Text = "停止";
            this.itemCarStop.Click += new System.EventHandler(this.itemCarStop_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnChangeCVWait,
            this.txtCVWaitMinute,
            this.toolStripLabel1,
            this.btnTestMode,
            this.btnSignalDisp});
            this.toolStrip1.Location = new System.Drawing.Point(0, 586);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(906, 25);
            this.toolStrip1.TabIndex = 16;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnChangeCVWait
            // 
            this.btnChangeCVWait.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnChangeCVWait.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnChangeCVWait.Image = ((System.Drawing.Image)(resources.GetObject("btnChangeCVWait.Image")));
            this.btnChangeCVWait.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnChangeCVWait.Name = "btnChangeCVWait";
            this.btnChangeCVWait.Size = new System.Drawing.Size(23, 22);
            this.btnChangeCVWait.Text = "変更";
            this.btnChangeCVWait.Visible = false;
            this.btnChangeCVWait.Click += new System.EventHandler(this.btnChangeCVWait_Click);
            // 
            // txtCVWaitMinute
            // 
            this.txtCVWaitMinute.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.txtCVWaitMinute.Name = "txtCVWaitMinute";
            this.txtCVWaitMinute.ReadOnly = true;
            this.txtCVWaitMinute.Size = new System.Drawing.Size(50, 25);
            this.txtCVWaitMinute.TextBoxTextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtCVWaitMinute.Visible = false;
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(118, 22);
            this.toolStripLabel1.Text = "常温CV待機時間(分)";
            this.toolStripLabel1.Visible = false;
            // 
            // btnTestMode
            // 
            this.btnTestMode.Image = ((System.Drawing.Image)(resources.GetObject("btnTestMode.Image")));
            this.btnTestMode.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnTestMode.Name = "btnTestMode";
            this.btnTestMode.Size = new System.Drawing.Size(88, 22);
            this.btnTestMode.Text = "Test Mode";
            this.btnTestMode.Click += new System.EventHandler(this.btnTestMode_Click);
            // 
            // btnSignalDisp
            // 
            this.btnSignalDisp.Image = ((System.Drawing.Image)(resources.GetObject("btnSignalDisp.Image")));
            this.btnSignalDisp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSignalDisp.Name = "btnSignalDisp";
            this.btnSignalDisp.Size = new System.Drawing.Size(76, 22);
            this.btnSignalDisp.Text = "信号確認";
            this.btnSignalDisp.Click += new System.EventHandler(this.btnSignalDisp_Click);
            // 
            // FrmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(906, 634);
            this.Controls.Add(this.grbMachine);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.toolStrip);
            this.Name = "FrmMain";
            this.Text = "ARMS3";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmMain_Load);
            this.menuMachine.ResumeLayout(false);
            this.groupBox1.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.tableLayoutPanel1.PerformLayout();
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.grbMachine.ResumeLayout(false);
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.menuCarrier.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeMacList;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.TextBox txtApiLog;
        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripLabel lblVersion;
        private System.Windows.Forms.ToolStripButton btnStartAll;
        private System.Windows.Forms.ToolStripButton btnStopReqAll;
        private System.Windows.Forms.GroupBox grbMachine;
        private System.Windows.Forms.ContextMenuStrip menuMachine;
        private System.Windows.Forms.ToolStripMenuItem itemMacStart;
        private System.Windows.Forms.ToolStripMenuItem itemMacStop;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel statusMachineThread;
        private System.Windows.Forms.ToolStripStatusLabel statusCarrierLabel;
        private System.Windows.Forms.ToolStripStatusLabel statusCarrierThread;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TreeView treeCarList;
        private System.Windows.Forms.ContextMenuStrip menuCarrier;
        private System.Windows.Forms.ToolStripMenuItem itemCarStart;
        private System.Windows.Forms.ToolStripMenuItem itemCarStop;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton btnChangeCVWait;
        private System.Windows.Forms.ToolStripTextBox txtCVWaitMinute;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnTestMode;
        private System.Windows.Forms.ToolStripMenuItem itemMacMagDelete;
        private System.Windows.Forms.ToolStripButton btnSignalDisp;
    }
}