namespace GEICS
{
    partial class F02_Menu
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F02_Menu));
            this.tvMenu = new System.Windows.Forms.TreeView();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolbtnLogoff = new System.Windows.Forms.ToolStripButton();
            this.toolbtnMiddleServer = new System.Windows.Forms.ToolStripButton();
            this.toolStripButton1 = new System.Windows.Forms.ToolStripButton();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toollblServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel3 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toollblEmp = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // tvMenu
            // 
            resources.ApplyResources(this.tvMenu, "tvMenu");
            this.tvMenu.ImageList = this.imageList;
            this.tvMenu.Name = "tvMenu";
            this.tvMenu.Nodes.AddRange(new System.Windows.Forms.TreeNode[] {
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvMenu.Nodes"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvMenu.Nodes1"))),
            ((System.Windows.Forms.TreeNode)(resources.GetObject("tvMenu.Nodes2")))});
            this.tvMenu.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvMenu_NodeMouseDoubleClick);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "FLD_FOLDER_GREY.png");
            this.imageList.Images.SetKeyName(1, "FLD_FOLDER_YELLOW.png");
            this.imageList.Images.SetKeyName(2, "ACT_MULTISELECT_EP.png");
            this.imageList.Images.SetKeyName(3, "ACT_MULTISELECT_IN.png");
            this.imageList.Images.SetKeyName(4, "FLD_FOLDER_BLUE.png");
            this.imageList.Images.SetKeyName(5, "FLD_FOLDER_GREEN.png");
            this.imageList.Images.SetKeyName(6, "FLD_FOLDER_RED.png");
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbtnLogoff,
            this.toolbtnMiddleServer,
            this.toolStripButton1});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolbtnLogoff
            // 
            resources.ApplyResources(this.toolbtnLogoff, "toolbtnLogoff");
            this.toolbtnLogoff.Name = "toolbtnLogoff";
            this.toolbtnLogoff.Click += new System.EventHandler(this.toolbtnLogoff_Click);
            // 
            // toolbtnMiddleServer
            // 
            resources.ApplyResources(this.toolbtnMiddleServer, "toolbtnMiddleServer");
            this.toolbtnMiddleServer.Name = "toolbtnMiddleServer";
            this.toolbtnMiddleServer.Click += new System.EventHandler(this.toolbtnMiddleServer_Click);
            // 
            // toolStripButton1
            // 
            this.toolStripButton1.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            resources.ApplyResources(this.toolStripButton1, "toolStripButton1");
            this.toolStripButton1.Name = "toolStripButton1";
            this.toolStripButton1.Click += new System.EventHandler(this.toolStripButton1_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel2,
            this.toolStripStatusLabel1,
            this.toollblServer,
            this.toolStripStatusLabel3,
            this.toollblEmp});
            resources.ApplyResources(this.statusStrip1, "statusStrip1");
            this.statusStrip1.Name = "statusStrip1";
            // 
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            resources.ApplyResources(this.toolStripStatusLabel2, "toolStripStatusLabel2");
            this.toolStripStatusLabel2.Spring = true;
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            resources.ApplyResources(this.toolStripStatusLabel1, "toolStripStatusLabel1");
            // 
            // toollblServer
            // 
            this.toollblServer.Name = "toollblServer";
            resources.ApplyResources(this.toollblServer, "toollblServer");
            // 
            // toolStripStatusLabel3
            // 
            this.toolStripStatusLabel3.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.toolStripStatusLabel3.Name = "toolStripStatusLabel3";
            resources.ApplyResources(this.toolStripStatusLabel3, "toolStripStatusLabel3");
            // 
            // toollblEmp
            // 
            this.toollblEmp.Name = "toollblEmp";
            resources.ApplyResources(this.toollblEmp, "toollblEmp");
            // 
            // F02_Menu
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.tvMenu);
            this.MaximizeBox = false;
            this.Name = "F02_Menu";
            this.Load += new System.EventHandler(this.frmMenu_Load);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView tvMenu;
        private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolbtnLogoff;
        private System.Windows.Forms.ToolStripButton toolbtnMiddleServer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.ToolStripStatusLabel toollblServer;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel3;
        private System.Windows.Forms.ToolStripStatusLabel toollblEmp;
		private System.Windows.Forms.ToolStripButton toolStripButton1;
    }
}