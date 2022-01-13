namespace Ovens
{
    partial class FmOvens
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
            this.txt_dev = new System.Windows.Forms.TextBox();
            this.button1 = new System.Windows.Forms.Button();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.MnSettei = new System.Windows.Forms.ToolStripMenuItem();
            this.MnSettei_OpenFile = new System.Windows.Forms.ToolStripMenuItem();
            this.MnHyouji = new System.Windows.Forms.ToolStripMenuItem();
            this.MnHyoujiSetubi = new System.Windows.Forms.ToolStripMenuItem();
            this.bt_StopHub = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TaskTimer
            // 
            this.TaskTimer.Interval = 5000;
            this.TaskTimer.Tick += new System.EventHandler(this.TaskTimer_Tick);
            // 
            // consoleBox
            // 
            this.consoleBox.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.consoleBox.Size = new System.Drawing.Size(760, 349);
            // 
            // btServerStart
            // 
            this.btServerStart.Location = new System.Drawing.Point(681, 534);
            this.btServerStart.Size = new System.Drawing.Size(91, 31);
            this.btServerStart.Text = "Start Hub";
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // txt_dev
            // 
            this.txt_dev.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txt_dev.Location = new System.Drawing.Point(207, 538);
            this.txt_dev.Name = "txt_dev";
            this.txt_dev.Size = new System.Drawing.Size(81, 25);
            this.txt_dev.TabIndex = 5;
            this.txt_dev.Visible = false;
            this.txt_dev.WordWrap = false;
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button1.Location = new System.Drawing.Point(294, 535);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(99, 31);
            this.button1.TabIndex = 6;
            this.button1.Text = "TestGetData";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnSettei,
            this.MnHyouji});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(784, 24);
            this.menuStrip1.TabIndex = 8;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // MnSettei
            // 
            this.MnSettei.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnSettei_OpenFile});
            this.MnSettei.Name = "MnSettei";
            this.MnSettei.Size = new System.Drawing.Size(43, 20);
            this.MnSettei.Text = "設定";
            // 
            // MnSettei_OpenFile
            // 
            this.MnSettei_OpenFile.Name = "MnSettei_OpenFile";
            this.MnSettei_OpenFile.Size = new System.Drawing.Size(180, 22);
            this.MnSettei_OpenFile.Text = "設定ファイル";
            this.MnSettei_OpenFile.Click += new System.EventHandler(this.MnSettei_OpenFile_Click);
            // 
            // MnHyouji
            // 
            this.MnHyouji.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MnHyoujiSetubi});
            this.MnHyouji.Name = "MnHyouji";
            this.MnHyouji.Size = new System.Drawing.Size(43, 20);
            this.MnHyouji.Text = "表示";
            // 
            // MnHyoujiSetubi
            // 
            this.MnHyoujiSetubi.Name = "MnHyoujiSetubi";
            this.MnHyoujiSetubi.Size = new System.Drawing.Size(122, 22);
            this.MnHyoujiSetubi.Text = "設備表示";
            this.MnHyoujiSetubi.Click += new System.EventHandler(this.MnHyoujiSetubi_Click);
            // 
            // bt_StopHub
            // 
            this.bt_StopHub.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.bt_StopHub.Location = new System.Drawing.Point(585, 534);
            this.bt_StopHub.Name = "bt_StopHub";
            this.bt_StopHub.Size = new System.Drawing.Size(90, 31);
            this.bt_StopHub.TabIndex = 9;
            this.bt_StopHub.Text = "Stop Hub";
            this.bt_StopHub.UseVisualStyleBackColor = true;
            this.bt_StopHub.Click += new System.EventHandler(this.bt_StopHub_Click);
            // 
            // button2
            // 
            this.button2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.button2.Location = new System.Drawing.Point(399, 534);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(85, 31);
            this.button2.TabIndex = 10;
            this.button2.Text = "NlogTest";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // FmOvens
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.bt_StopHub);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.txt_dev);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.Name = "FmOvens";
            this.Text = "Oven Data Hub";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FmOvens_FormClosing);
            this.Controls.SetChildIndex(this.ErrorLogComsole, 0);
            this.Controls.SetChildIndex(this.menuStrip1, 0);
            this.Controls.SetChildIndex(this.consoleBox, 0);
            this.Controls.SetChildIndex(this.btServerStart, 0);
            this.Controls.SetChildIndex(this.txt_dev, 0);
            this.Controls.SetChildIndex(this.button1, 0);
            this.Controls.SetChildIndex(this.bt_StopHub, 0);
            this.Controls.SetChildIndex(this.button2, 0);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_dev;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem MnSettei;
        private System.Windows.Forms.ToolStripMenuItem MnHyouji;
        private System.Windows.Forms.ToolStripMenuItem MnHyoujiSetubi;
        private System.Windows.Forms.Button bt_StopHub;
        private System.Windows.Forms.ToolStripMenuItem MnSettei_OpenFile;
        private System.Windows.Forms.Button button2;
    }
}

