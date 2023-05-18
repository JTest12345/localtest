namespace EICS
{
    partial class F01_MachineWatch
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F01_MachineWatch));
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.toolCheckDBConnectInfo = new System.Windows.Forms.ToolStripButton();
            this.toolAllMachineStart = new System.Windows.Forms.ToolStripButton();
            this.toolAllMachineStop = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolLotChain = new System.Windows.Forms.ToolStripButton();
            this.toolTrayEdit = new System.Windows.Forms.ToolStripButton();
            this.toolMachineList = new System.Windows.Forms.ToolStripButton();
            this.btnSilent = new System.Windows.Forms.Button();
            this.tvNGMessage = new System.Windows.Forms.TreeView();
            this.fileSystemWatcher = new System.IO.FileSystemWatcher();
            this.tvOKMessage = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.tvMachine = new System.Windows.Forms.TreeView();
            this.cmsObserve = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmStart = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmStop = new System.Windows.Forms.ToolStripMenuItem();
            this.tmRemoteErrReceiver = new System.Windows.Forms.Timer(this.components);
            this.TmTypeChanger = new System.Windows.Forms.Timer(this.components);
            this.toolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).BeginInit();
            this.cmsObserve.SuspendLayout();
            this.SuspendLayout();
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "kai_re.gif");
            this.imageList.Images.SetKeyName(1, "kai_ye.gif");
            this.imageList.Images.SetKeyName(2, "kai_bl.gif");
            this.imageList.Images.SetKeyName(3, "kai_pi.gif");
            this.imageList.Images.SetKeyName(4, "kai_gr.gif");
            this.imageList.Images.SetKeyName(5, "kai_grye.gif");
            // 
            // statusStrip
            // 
            resources.ApplyResources(this.statusStrip, "statusStrip");
            this.statusStrip.Name = "statusStrip";
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripButton1,
            this.toolCheckDBConnectInfo,
            this.toolAllMachineStart,
            this.toolAllMachineStop,
            this.toolStripSeparator1,
            this.toolLotChain,
            this.toolTrayEdit,
            this.toolMachineList});
            resources.ApplyResources(this.toolStrip, "toolStrip");
            this.toolStrip.Name = "toolStrip";
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolApplicationExit_Click);
            // 
            // toolCheckDBConnectInfo
            // 
            this.toolCheckDBConnectInfo.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            resources.ApplyResources(this.toolCheckDBConnectInfo, "toolCheckDBConnectInfo");
            this.toolCheckDBConnectInfo.Name = "toolCheckDBConnectInfo";
            this.toolCheckDBConnectInfo.Click += new System.EventHandler(this.toolCheckDBConnectInfo_Click);
            // 
            // toolAllMachineStart
            // 
            resources.ApplyResources(this.toolAllMachineStart, "toolAllMachineStart");
            this.toolAllMachineStart.Name = "toolAllMachineStart";
            this.toolAllMachineStart.Click += new System.EventHandler(this.toolAllMachineStart_Click);
            // 
            // toolAllMachineStop
            // 
            resources.ApplyResources(this.toolAllMachineStop, "toolAllMachineStop");
            this.toolAllMachineStop.Name = "toolAllMachineStop";
            this.toolAllMachineStop.Click += new System.EventHandler(this.toolAllMachineStop_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            resources.ApplyResources(this.toolStripSeparator1, "toolStripSeparator1");
            // 
            // toolLotChain
            // 
            resources.ApplyResources(this.toolLotChain, "toolLotChain");
            this.toolLotChain.Name = "toolLotChain";
            this.toolLotChain.Click += new System.EventHandler(this.toolLotChain_Click);
            // 
            // toolTrayEdit
            // 
            this.toolTrayEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolTrayEdit, "toolTrayEdit");
            this.toolTrayEdit.Name = "toolTrayEdit";
            this.toolTrayEdit.Click += new System.EventHandler(this.toolTrayEdit_Click);
            // 
            // toolMachineList
            // 
            resources.ApplyResources(this.toolMachineList, "toolMachineList");
            this.toolMachineList.Name = "toolMachineList";
            this.toolMachineList.Click += new System.EventHandler(this.toolMachineList_Click);
            // 
            // btnSilent
            // 
            resources.ApplyResources(this.btnSilent, "btnSilent");
            this.btnSilent.Name = "btnSilent";
            this.btnSilent.UseVisualStyleBackColor = true;
            this.btnSilent.Click += new System.EventHandler(this.btnSilent_Click);
            // 
            // tvNGMessage
            // 
            resources.ApplyResources(this.tvNGMessage, "tvNGMessage");
            this.tvNGMessage.BackColor = System.Drawing.SystemColors.Control;
            this.tvNGMessage.Name = "tvNGMessage";
            this.tvNGMessage.ShowLines = false;
            this.tvNGMessage.ShowPlusMinus = false;
            this.tvNGMessage.ShowRootLines = false;
            this.tvNGMessage.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvMessage_NodeMouseDoubleClick);
            // 
            // fileSystemWatcher
            // 
            this.fileSystemWatcher.EnableRaisingEvents = true;
            this.fileSystemWatcher.NotifyFilter = System.IO.NotifyFilters.LastWrite;
            this.fileSystemWatcher.Path = "C:\\QCIL\\SettingFiles";
            this.fileSystemWatcher.SynchronizingObject = this;
            this.fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(this.fileSystemWatcher_Changed);
            // 
            // tvOKMessage
            // 
            resources.ApplyResources(this.tvOKMessage, "tvOKMessage");
            this.tvOKMessage.BackColor = System.Drawing.SystemColors.Control;
            this.tvOKMessage.Name = "tvOKMessage";
            this.tvOKMessage.ShowLines = false;
            this.tvOKMessage.ShowPlusMinus = false;
            this.tvOKMessage.ShowRootLines = false;
            this.tvOKMessage.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvMessage_NodeMouseDoubleClick);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // tvMachine
            // 
            resources.ApplyResources(this.tvMachine, "tvMachine");
            this.tvMachine.ContextMenuStrip = this.cmsObserve;
            this.tvMachine.ImageList = this.imageList;
            this.tvMachine.Name = "tvMachine";
            this.tvMachine.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvMachine_NodeMouseDoubleClick);
            // 
            // cmsObserve
            // 
            this.cmsObserve.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmStart,
            this.tsmStop});
            this.cmsObserve.Name = "cmsObserve";
            resources.ApplyResources(this.cmsObserve, "cmsObserve");
            this.cmsObserve.Opening += new System.ComponentModel.CancelEventHandler(this.cmsObserve_Opening);
            this.cmsObserve.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsObserve_ItemClicked);
            // 
            // tsmStart
            // 
            this.tsmStart.Name = "tsmStart";
            resources.ApplyResources(this.tsmStart, "tsmStart");
            // 
            // tsmStop
            // 
            this.tsmStop.Name = "tsmStop";
            resources.ApplyResources(this.tsmStop, "tsmStop");
            // 
            // tmRemoteErrReceiver
            // 
            this.tmRemoteErrReceiver.Interval = 2000;
            this.tmRemoteErrReceiver.Tick += new System.EventHandler(this.tmRemoteErrReceiver_Tick);
            // 
            // TmTypeChanger
            // 
            this.TmTypeChanger.Interval = 5000;
            this.TmTypeChanger.Tick += new System.EventHandler(this.TmTypeChanger_Tick);
            // 
            // F01_MachineWatch
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.tvMachine);
            this.Controls.Add(this.tvOKMessage);
            this.Controls.Add(this.tvNGMessage);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.btnSilent);
            this.MaximizeBox = false;
            this.Name = "F01_MachineWatch";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.F01_MachineWatch_FormClosing);
            this.Load += new System.EventHandler(this.G001_MachineWatch_Load);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.fileSystemWatcher)).EndInit();
            this.cmsObserve.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton toolAllMachineStart;
        private System.Windows.Forms.ToolStripButton toolCheckDBConnectInfo;
		private System.Windows.Forms.ToolStripButton toolAllMachineStop;
        private System.Windows.Forms.ToolStripButton toolMachineList;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.Button btnSilent;
        private System.Windows.Forms.ToolStripButton toolLotChain;
        private System.Windows.Forms.TreeView tvNGMessage;
        private System.IO.FileSystemWatcher fileSystemWatcher;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TreeView tvOKMessage;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TreeView tvMachine;
		private System.Windows.Forms.ContextMenuStrip cmsObserve;
		private System.Windows.Forms.ToolStripMenuItem tsmStart;
		private System.Windows.Forms.ToolStripMenuItem tsmStop;
		private System.Windows.Forms.Timer tmRemoteErrReceiver;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
		private System.Windows.Forms.Timer TmTypeChanger;
        private System.Windows.Forms.ToolStripButton toolTrayEdit;
    }
}

