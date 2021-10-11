namespace MagCapServer
{
    partial class fmMain
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
            this.components = new System.ComponentModel.Container();
            this.consoleBox = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.設定ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.magcupiniToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ツールToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pLC接続テストToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.btServerStart = new System.Windows.Forms.Button();
            this.TaskTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.MqttTimer = new System.Windows.Forms.Timer(this.components);
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // consoleBox
            // 
            this.consoleBox.Location = new System.Drawing.Point(12, 28);
            this.consoleBox.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.consoleBox.MaxLength = 0;
            this.consoleBox.Multiline = true;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.consoleBox.Size = new System.Drawing.Size(760, 360);
            this.consoleBox.TabIndex = 0;
            this.consoleBox.WordWrap = false;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.設定ToolStripMenuItem,
            this.ツールToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 2;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 設定ToolStripMenuItem
            // 
            this.設定ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.magcupiniToolStripMenuItem});
            this.設定ToolStripMenuItem.Name = "設定ToolStripMenuItem";
            this.設定ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.設定ToolStripMenuItem.Text = "設定";
            // 
            // magcupiniToolStripMenuItem
            // 
            this.magcupiniToolStripMenuItem.Name = "magcupiniToolStripMenuItem";
            this.magcupiniToolStripMenuItem.Size = new System.Drawing.Size(133, 22);
            this.magcupiniToolStripMenuItem.Text = "magcup.ini";
            this.magcupiniToolStripMenuItem.Click += new System.EventHandler(this.magcupiniToolStripMenuItem_Click);
            // 
            // ツールToolStripMenuItem
            // 
            this.ツールToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pLC接続テストToolStripMenuItem});
            this.ツールToolStripMenuItem.Name = "ツールToolStripMenuItem";
            this.ツールToolStripMenuItem.Size = new System.Drawing.Size(46, 20);
            this.ツールToolStripMenuItem.Text = "ツール";
            // 
            // pLC接続テストToolStripMenuItem
            // 
            this.pLC接続テストToolStripMenuItem.Name = "pLC接続テストToolStripMenuItem";
            this.pLC接続テストToolStripMenuItem.Size = new System.Drawing.Size(144, 22);
            this.pLC接続テストToolStripMenuItem.Text = "PLC接続テスト";
            this.pLC接続テストToolStripMenuItem.Click += new System.EventHandler(this.pLC接続テストToolStripMenuItem_Click);
            // 
            // btServerStart
            // 
            this.btServerStart.Location = new System.Drawing.Point(645, 395);
            this.btServerStart.Name = "btServerStart";
            this.btServerStart.Size = new System.Drawing.Size(127, 31);
            this.btServerStart.TabIndex = 3;
            this.btServerStart.Text = "Start Server";
            this.btServerStart.UseVisualStyleBackColor = true;
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // TaskTimer
            // 
            this.TaskTimer.Interval = 1000;
            this.TaskTimer.Tick += new System.EventHandler(this.TaskTimer_Tick);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 429);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.SizingGrip = false;
            this.statusStrip1.TabIndex = 4;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // MqttTimer
            // 
            this.MqttTimer.Interval = 2000;
            this.MqttTimer.Tick += new System.EventHandler(this.MqttTimer_Tick);
            // 
            // fmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 451);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btServerStart);
            this.Controls.Add(this.consoleBox);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(800, 490);
            this.MinimumSize = new System.Drawing.Size(800, 490);
            this.Name = "fmMain";
            this.Text = "MagCupServer ";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox consoleBox;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 設定ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem magcupiniToolStripMenuItem;
        private System.Windows.Forms.Button btServerStart;
        private System.Windows.Forms.Timer TaskTimer;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.Timer MqttTimer;
        private System.Windows.Forms.ToolStripMenuItem ツールToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pLC接続テストToolStripMenuItem;
    }
}

