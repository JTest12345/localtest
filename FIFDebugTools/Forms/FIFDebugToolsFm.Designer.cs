namespace FIFDebugTools
{
    partial class FIFDebugToolsFm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.toolsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.データベース操作ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pLCデバイス操作ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btServerStart
            // 
            this.btServerStart.Text = "OptionButton";
            this.btServerStart.Visible = false;
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 11;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // toolsToolStripMenuItem
            // 
            this.toolsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.データベース操作ToolStripMenuItem,
            this.pLCデバイス操作ToolStripMenuItem});
            this.toolsToolStripMenuItem.Name = "toolsToolStripMenuItem";
            this.toolsToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.toolsToolStripMenuItem.Text = "Tools";
            // 
            // データベース操作ToolStripMenuItem
            // 
            this.データベース操作ToolStripMenuItem.Name = "データベース操作ToolStripMenuItem";
            this.データベース操作ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.データベース操作ToolStripMenuItem.Text = "データベース操作";
            this.データベース操作ToolStripMenuItem.Click += new System.EventHandler(this.OpenArmsTranController);
            // 
            // pLCデバイス操作ToolStripMenuItem
            // 
            this.pLCデバイス操作ToolStripMenuItem.Name = "pLCデバイス操作ToolStripMenuItem";
            this.pLCデバイス操作ToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.pLCデバイス操作ToolStripMenuItem.Text = "PLC接続テスト";
            this.pLCデバイス操作ToolStripMenuItem.Click += new System.EventHandler(this.OpenPlcTestForm);
            // 
            // FIFDebugToolsFm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FIFDebugToolsFm";
            this.Text = "FIFDebugTools";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FIFDebugToolsFm_FormClosing);
            this.Controls.SetChildIndex(this.consoleBox, 0);
            this.Controls.SetChildIndex(this.btServerStart, 0);
            this.Controls.SetChildIndex(this.menuStrip1, 0);
            this.Controls.SetChildIndex(this.ErrorLogComsole, 0);
            this.Controls.SetChildIndex(this.bt_ClearErrLogs, 0);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem toolsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem データベース操作ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pLCデバイス操作ToolStripMenuItem;
    }
}

