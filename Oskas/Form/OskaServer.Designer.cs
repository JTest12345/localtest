namespace Oskas
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
            this.btServerStart = new System.Windows.Forms.Button();
            this.TaskTimer = new System.Windows.Forms.Timer(this.components);
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStripStatusLabel2 = new System.Windows.Forms.ToolStripStatusLabel();
            this.MqttTimer = new System.Windows.Forms.Timer(this.components);
            this.bt_ClearErrLogs = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.ErrorLogComsole = new System.Windows.Forms.TextBox();
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
            // btServerStart
            // 
            this.btServerStart.Location = new System.Drawing.Point(645, 505);
            this.btServerStart.Name = "btServerStart";
            this.btServerStart.Size = new System.Drawing.Size(127, 31);
            this.btServerStart.TabIndex = 3;
            this.btServerStart.Text = "Start Server";
            this.btServerStart.UseVisualStyleBackColor = true;
            // 
            // TaskTimer
            // 
            this.TaskTimer.Interval = 1000;
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1,
            this.toolStripStatusLabel2});
            this.statusStrip1.Location = new System.Drawing.Point(0, 539);
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
            // toolStripStatusLabel2
            // 
            this.toolStripStatusLabel2.Name = "toolStripStatusLabel2";
            this.toolStripStatusLabel2.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel2.Text = "toolStripStatusLabel2";
            // 
            // MqttTimer
            // 
            this.MqttTimer.Interval = 2000;
            // 
            // bt_ClearErrLogs
            // 
            this.bt_ClearErrLogs.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bt_ClearErrLogs.Location = new System.Drawing.Point(13, 505);
            this.bt_ClearErrLogs.Name = "bt_ClearErrLogs";
            this.bt_ClearErrLogs.Size = new System.Drawing.Size(166, 30);
            this.bt_ClearErrLogs.TabIndex = 10;
            this.bt_ClearErrLogs.Text = "アラーム・エラーログクリア";
            this.bt_ClearErrLogs.UseVisualStyleBackColor = true;
            this.bt_ClearErrLogs.Click += new System.EventHandler(this.bt_ClearErrLogs_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(14, 388);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 17);
            this.label1.TabIndex = 9;
            this.label1.Text = "アラーム・エラーモニタ";
            // 
            // ErrorLogComsole
            // 
            this.ErrorLogComsole.ForeColor = System.Drawing.Color.Red;
            this.ErrorLogComsole.Location = new System.Drawing.Point(13, 407);
            this.ErrorLogComsole.Multiline = true;
            this.ErrorLogComsole.Name = "ErrorLogComsole";
            this.ErrorLogComsole.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ErrorLogComsole.Size = new System.Drawing.Size(759, 92);
            this.ErrorLogComsole.TabIndex = 8;
            // 
            // fmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.bt_ClearErrLogs);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.ErrorLogComsole);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.btServerStart);
            this.Controls.Add(this.consoleBox);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(800, 600);
            this.MinimumSize = new System.Drawing.Size(800, 600);
            this.Name = "fmMain";
            this.Text = "Oskas Server Flame";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.fmMain_FormClosing);
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        protected System.Windows.Forms.Timer TaskTimer;
        protected System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        protected System.Windows.Forms.Timer MqttTimer;
        protected System.Windows.Forms.TextBox consoleBox;
        protected System.Windows.Forms.Button btServerStart;
        protected System.Windows.Forms.StatusStrip statusStrip1;
        protected System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel2;
        private System.Windows.Forms.Button bt_ClearErrLogs;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ErrorLogComsole;
    }
}

