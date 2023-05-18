namespace ArmsNascaBridge3
{
    partial class FrmBridgeMain
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
			this.txtLog = new System.Windows.Forms.TextBox();
			this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
			this.更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStrip1 = new System.Windows.Forms.ToolStrip();
			this.lblVersion = new System.Windows.Forms.ToolStripLabel();
			this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
			this.contextMenuStrip1.SuspendLayout();
			this.toolStrip1.SuspendLayout();
			this.SuspendLayout();
			// 
			// txtLog
			// 
			this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.txtLog.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.txtLog.Location = new System.Drawing.Point(12, 28);
			this.txtLog.Multiline = true;
			this.txtLog.Name = "txtLog";
			this.txtLog.ReadOnly = true;
			this.txtLog.Size = new System.Drawing.Size(433, 228);
			this.txtLog.TabIndex = 0;
			// 
			// contextMenuStrip1
			// 
			this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.更新ToolStripMenuItem,
            this.終了ToolStripMenuItem});
			this.contextMenuStrip1.Name = "contextMenuStrip1";
			this.contextMenuStrip1.Size = new System.Drawing.Size(101, 48);
			// 
			// 更新ToolStripMenuItem
			// 
			this.更新ToolStripMenuItem.Name = "更新ToolStripMenuItem";
			this.更新ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
			this.更新ToolStripMenuItem.Text = "更新";
			// 
			// 終了ToolStripMenuItem
			// 
			this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
			this.終了ToolStripMenuItem.Size = new System.Drawing.Size(100, 22);
			this.終了ToolStripMenuItem.Text = "終了";
			this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem1_Click);
			// 
			// toolStrip1
			// 
			this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblVersion,
            this.toolStripLabel1});
			this.toolStrip1.Location = new System.Drawing.Point(0, 0);
			this.toolStrip1.Name = "toolStrip1";
			this.toolStrip1.Size = new System.Drawing.Size(457, 25);
			this.toolStrip1.TabIndex = 1;
			this.toolStrip1.Text = "toolStrip1";
			// 
			// lblVersion
			// 
			this.lblVersion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.lblVersion.Name = "lblVersion";
			this.lblVersion.Size = new System.Drawing.Size(0, 22);
			// 
			// toolStripLabel1
			// 
			this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
			this.toolStripLabel1.Name = "toolStripLabel1";
			this.toolStripLabel1.Size = new System.Drawing.Size(29, 22);
			this.toolStripLabel1.Text = "ver.";
			// 
			// FrmBridgeMain
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(457, 268);
			this.Controls.Add(this.txtLog);
			this.Controls.Add(this.toolStrip1);
			this.Name = "FrmBridgeMain";
			this.ShowInTaskbar = false;
			this.Text = "NASCABridge3";
			this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBridgeMain_FormClosing);
			this.Load += new System.EventHandler(this.FrmBridgeMain_Load);
			this.Shown += new System.EventHandler(this.FrmBridgeMain_Shown);
			this.contextMenuStrip1.ResumeLayout(false);
			this.toolStrip1.ResumeLayout(false);
			this.toolStrip1.PerformLayout();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 更新ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblVersion;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
    }
}