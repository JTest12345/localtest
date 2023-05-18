namespace FVDC
{
    partial class FVDC200
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVDC200));
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.tsbResident = new System.Windows.Forms.ToolStripButton();
            this.tsbReset = new System.Windows.Forms.ToolStripButton();
            this.tsSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbExcelOut = new System.Windows.Forms.ToolStripButton();
            this.tsSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.tsbReplace = new System.Windows.Forms.ToolStripButton();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.tsslProgressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.tsslMessage = new System.Windows.Forms.ToolStripStatusLabel();
            this.tsslServer = new System.Windows.Forms.ToolStripStatusLabel();
            this.btnFind = new System.Windows.Forms.Button();
            this.lblType = new System.Windows.Forms.Label();
            this.cmbType = new System.Windows.Forms.ComboBox();
            this.lblFrame = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.flowLayoutPanel = new System.Windows.Forms.FlowLayoutPanel();
            this.lblXPackage = new System.Windows.Forms.Label();
            this.lblYPackage = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.cmbTitle = new System.Windows.Forms.ComboBox();
            this.dsType = new FVDC.DsName();
            this.notifyIcon = new System.Windows.Forms.NotifyIcon(this.components);
            this.btnFileWrite = new System.Windows.Forms.Button();
            this.chkInvert = new System.Windows.Forms.CheckBox();
            this.toolStrip.SuspendLayout();
            this.statusStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsType)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip
            // 
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsbResident,
            this.tsbReset,
            this.tsSeparator1,
            this.tsbExcelOut,
            this.tsSeparator2,
            this.tsbReplace});
            this.toolStrip.Location = new System.Drawing.Point(0, 0);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Padding = new System.Windows.Forms.Padding(0, 0, 3, 0);
            this.toolStrip.Size = new System.Drawing.Size(1014, 25);
            this.toolStrip.TabIndex = 0;
            // 
            // tsbResident
            // 
            this.tsbResident.AccessibleRole = System.Windows.Forms.AccessibleRole.ColumnHeader;
            this.tsbResident.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.tsbResident.Image = ((System.Drawing.Image)(resources.GetObject("tsbResident.Image")));
            this.tsbResident.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbResident.Name = "tsbResident";
            this.tsbResident.Size = new System.Drawing.Size(23, 22);
            this.tsbResident.Text = "常駐";
            this.tsbResident.ToolTipText = "タスクバーに常駐します。";
            this.tsbResident.Click += new System.EventHandler(this.tsbResident_Click);
            // 
            // tsbReset
            // 
            this.tsbReset.Image = ((System.Drawing.Image)(resources.GetObject("tsbReset.Image")));
            this.tsbReset.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReset.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tsbReset.Name = "tsbReset";
            this.tsbReset.Size = new System.Drawing.Size(55, 22);
            this.tsbReset.Text = " ﾘｾｯﾄ";
            this.tsbReset.ToolTipText = "参照ロット未選択状態に戻します。";
            this.tsbReset.Click += new System.EventHandler(this.tsbReset_Click);
            // 
            // tsSeparator1
            // 
            this.tsSeparator1.Margin = new System.Windows.Forms.Padding(5, 0, 10, 0);
            this.tsSeparator1.Name = "tsSeparator1";
            this.tsSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbExcelOut
            // 
            this.tsbExcelOut.Image = ((System.Drawing.Image)(resources.GetObject("tsbExcelOut.Image")));
            this.tsbExcelOut.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbExcelOut.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tsbExcelOut.Name = "tsbExcelOut";
            this.tsbExcelOut.Size = new System.Drawing.Size(79, 22);
            this.tsbExcelOut.Text = " ｴｸｾﾙ出力";
            this.tsbExcelOut.ToolTipText = "表示している内容をExcelに出力します。";
            this.tsbExcelOut.Click += new System.EventHandler(this.tsbExcelOut_Click);
            // 
            // tsSeparator2
            // 
            this.tsSeparator2.Name = "tsSeparator2";
            this.tsSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // tsbReplace
            // 
            this.tsbReplace.Image = ((System.Drawing.Image)(resources.GetObject("tsbReplace.Image")));
            this.tsbReplace.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.tsbReplace.Margin = new System.Windows.Forms.Padding(10, 1, 0, 2);
            this.tsbReplace.Name = "tsbReplace";
            this.tsbReplace.Size = new System.Drawing.Size(55, 22);
            this.tsbReplace.Text = " 置換";
            this.tsbReplace.ToolTipText = "表示されているグリッドの文字を一括で置換出来ます。";
            this.tsbReplace.Click += new System.EventHandler(this.tsbReplace_Click);
            // 
            // statusStrip
            // 
            this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsslProgressBar,
            this.tsslMessage,
            this.tsslServer});
            this.statusStrip.Location = new System.Drawing.Point(0, 660);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.Padding = new System.Windows.Forms.Padding(1, 0, 16, 0);
            this.statusStrip.Size = new System.Drawing.Size(1014, 28);
            this.statusStrip.TabIndex = 12;
            // 
            // tsslProgressBar
            // 
            this.tsslProgressBar.Margin = new System.Windows.Forms.Padding(1, 3, 1, 1);
            this.tsslProgressBar.Name = "tsslProgressBar";
            this.tsslProgressBar.Size = new System.Drawing.Size(175, 24);
            // 
            // tsslMessage
            // 
            this.tsslMessage.AutoSize = false;
            this.tsslMessage.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsslMessage.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslMessage.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tsslMessage.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.tsslMessage.Name = "tsslMessage";
            this.tsslMessage.Size = new System.Drawing.Size(700, 23);
            this.tsslMessage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // tsslServer
            // 
            this.tsslServer.BorderSides = System.Windows.Forms.ToolStripStatusLabelBorderSides.Left;
            this.tsslServer.BorderStyle = System.Windows.Forms.Border3DStyle.Etched;
            this.tsslServer.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.tsslServer.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tsslServer.Margin = new System.Windows.Forms.Padding(0, 5, 0, 3);
            this.tsslServer.Name = "tsslServer";
            this.tsslServer.Size = new System.Drawing.Size(55, 20);
            this.tsslServer.Text = "サーバー";
            this.tsslServer.ToolTipText = "テストするサーバーです。メニューバー左上の設定より変更可能です。";
            // 
            // btnFind
            // 
            this.btnFind.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFind.Location = new System.Drawing.Point(301, 35);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(26, 23);
            this.btnFind.TabIndex = 3;
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // lblType
            // 
            this.lblType.BackColor = System.Drawing.SystemColors.Info;
            this.lblType.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblType.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblType.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblType.Location = new System.Drawing.Point(3, 35);
            this.lblType.Name = "lblType";
            this.lblType.Size = new System.Drawing.Size(75, 23);
            this.lblType.TabIndex = 1;
            this.lblType.Text = "対象タイプ";
            this.lblType.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbType
            // 
            this.cmbType.CausesValidation = false;
            this.cmbType.DisplayMember = "Name.Data_NM";
            this.cmbType.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbType.FormattingEnabled = true;
            this.cmbType.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbType.Location = new System.Drawing.Point(77, 35);
            this.cmbType.Name = "cmbType";
            this.cmbType.Size = new System.Drawing.Size(225, 23);
            this.cmbType.TabIndex = 2;
            this.cmbType.ValueMember = "Name.Key_CD";
            this.cmbType.SelectedIndexChanged += new System.EventHandler(this.cmbType_SelectedIndexChanged);
            this.cmbType.TextChanged += new System.EventHandler(this.cmbType_TextChanged);
            // 
            // lblFrame
            // 
            this.lblFrame.BackColor = System.Drawing.SystemColors.Info;
            this.lblFrame.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblFrame.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblFrame.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblFrame.Location = new System.Drawing.Point(342, 35);
            this.lblFrame.Name = "lblFrame";
            this.lblFrame.Size = new System.Drawing.Size(90, 23);
            this.lblFrame.TabIndex = 4;
            this.lblFrame.Text = "フレーム情報";
            this.lblFrame.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.Color.ForestGreen;
            this.label1.Location = new System.Drawing.Point(3, 66);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(953, 15);
            this.label1.TabIndex = 10;
            this.label1.Text = "※最初に対象タイプを選択してからwbm/mpd/MM/ME/SDファイルをドラッグドロップして下さい。自動解析してフレーム別に実際の配列で表示します。";
            // 
            // flowLayoutPanel
            // 
            this.flowLayoutPanel.AllowDrop = true;
            this.flowLayoutPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.flowLayoutPanel.AutoScroll = true;
            this.flowLayoutPanel.CausesValidation = false;
            this.flowLayoutPanel.ForeColor = System.Drawing.SystemColors.ControlText;
            this.flowLayoutPanel.Location = new System.Drawing.Point(0, 85);
            this.flowLayoutPanel.Name = "flowLayoutPanel";
            this.flowLayoutPanel.Size = new System.Drawing.Size(1014, 572);
            this.flowLayoutPanel.TabIndex = 11;
            this.flowLayoutPanel.DragDrop += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel_DragDrop);
            this.flowLayoutPanel.DragOver += new System.Windows.Forms.DragEventHandler(this.flowLayoutPanel_DragOver);
            // 
            // lblXPackage
            // 
            this.lblXPackage.BackColor = System.Drawing.SystemColors.HighlightText;
            this.lblXPackage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblXPackage.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblXPackage.Location = new System.Drawing.Point(431, 35);
            this.lblXPackage.Name = "lblXPackage";
            this.lblXPackage.Padding = new System.Windows.Forms.Padding(1);
            this.lblXPackage.Size = new System.Drawing.Size(67, 23);
            this.lblXPackage.TabIndex = 5;
            this.lblXPackage.Text = "X：";
            this.lblXPackage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblYPackage
            // 
            this.lblYPackage.BackColor = System.Drawing.SystemColors.HighlightText;
            this.lblYPackage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblYPackage.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblYPackage.Location = new System.Drawing.Point(497, 35);
            this.lblYPackage.Name = "lblYPackage";
            this.lblYPackage.Padding = new System.Windows.Forms.Padding(1);
            this.lblYPackage.Size = new System.Drawing.Size(67, 23);
            this.lblYPackage.TabIndex = 6;
            this.lblYPackage.Text = "Y：";
            this.lblYPackage.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // lblTitle
            // 
            this.lblTitle.BackColor = System.Drawing.SystemColors.Info;
            this.lblTitle.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblTitle.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblTitle.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblTitle.Location = new System.Drawing.Point(579, 36);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(75, 23);
            this.lblTitle.TabIndex = 7;
            this.lblTitle.Text = "表示項目";
            this.lblTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblTitle.Visible = false;
            // 
            // cmbTitle
            // 
            this.cmbTitle.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbTitle.CausesValidation = false;
            this.cmbTitle.DisplayMember = "Name.Data_NM";
            this.cmbTitle.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbTitle.FormattingEnabled = true;
            this.cmbTitle.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbTitle.Location = new System.Drawing.Point(653, 36);
            this.cmbTitle.Name = "cmbTitle";
            this.cmbTitle.Size = new System.Drawing.Size(349, 23);
            this.cmbTitle.TabIndex = 8;
            this.cmbTitle.ValueMember = "Name.Key_CD";
            this.cmbTitle.Visible = false;
            this.cmbTitle.SelectionChangeCommitted += new System.EventHandler(this.cmbTitle_SelectionChangeCommitted);
            // 
            // dsType
            // 
            this.dsType.DataSetName = "DsName";
            this.dsType.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // notifyIcon
            // 
            this.notifyIcon.BalloonTipText = "常駐して起動の手間を省きます。";
            this.notifyIcon.BalloonTipTitle = "マッピングファイル解析";
            this.notifyIcon.Icon = ((System.Drawing.Icon)(resources.GetObject("notifyIcon.Icon")));
            this.notifyIcon.Text = "マッピングファイル解析";
            this.notifyIcon.DoubleClick += new System.EventHandler(this.notifyIcon_DoubleClick);
            this.notifyIcon.MouseClick += new System.Windows.Forms.MouseEventHandler(this.notifyIcon_MouseClick);
            // 
            // btnFileWrite
            // 
            this.btnFileWrite.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFileWrite.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFileWrite.Image = ((System.Drawing.Image)(resources.GetObject("btnFileWrite.Image")));
            this.btnFileWrite.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFileWrite.Location = new System.Drawing.Point(871, 36);
            this.btnFileWrite.Name = "btnFileWrite";
            this.btnFileWrite.Size = new System.Drawing.Size(105, 24);
            this.btnFileWrite.TabIndex = 9;
            this.btnFileWrite.Text = "　ファイル更新";
            this.btnFileWrite.UseVisualStyleBackColor = true;
            this.btnFileWrite.Visible = false;
            this.btnFileWrite.Click += new System.EventHandler(this.btnFileWrite_Click);
            // 
            // chkInvert
            // 
            this.chkInvert.AutoSize = true;
            this.chkInvert.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.chkInvert.Location = new System.Drawing.Point(580, 38);
            this.chkInvert.Name = "chkInvert";
            this.chkInvert.Size = new System.Drawing.Size(108, 20);
            this.chkInvert.TabIndex = 13;
            this.chkInvert.Text = "フレーム反転";
            this.chkInvert.UseVisualStyleBackColor = true;
            this.chkInvert.Visible = false;
            // 
            // FVDC200
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1014, 688);
            this.Controls.Add(this.chkInvert);
            this.Controls.Add(this.btnFileWrite);
            this.Controls.Add(this.lblTitle);
            this.Controls.Add(this.cmbTitle);
            this.Controls.Add(this.lblYPackage);
            this.Controls.Add(this.lblXPackage);
            this.Controls.Add(this.flowLayoutPanel);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblFrame);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.lblType);
            this.Controls.Add(this.cmbType);
            this.Controls.Add(this.statusStrip);
            this.Controls.Add(this.toolStrip);
            this.Name = "FVDC200";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FVDC200　マッピングファイル解析";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.FVDC200_FormClosed);
            this.Load += new System.EventHandler(this.FVDC200_Load);
            this.Resize += new System.EventHandler(this.FVDC200_Resize);
            this.toolStrip.ResumeLayout(false);
            this.toolStrip.PerformLayout();
            this.statusStrip.ResumeLayout(false);
            this.statusStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsType)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.ToolStripButton tsbResident;
        private System.Windows.Forms.ToolStripButton tsbReset;
        private System.Windows.Forms.ToolStripSeparator tsSeparator1;
        private System.Windows.Forms.ToolStripButton tsbExcelOut;
        private System.Windows.Forms.ToolStripSeparator tsSeparator2;
        private System.Windows.Forms.StatusStrip statusStrip;
        private System.Windows.Forms.ToolStripProgressBar tsslProgressBar;
        private System.Windows.Forms.ToolStripStatusLabel tsslMessage;
        private System.Windows.Forms.ToolStripStatusLabel tsslServer;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.Label lblType;
        private System.Windows.Forms.ComboBox cmbType;
        private System.Windows.Forms.Label lblFrame;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.FlowLayoutPanel flowLayoutPanel;
        private System.Windows.Forms.Label lblXPackage;
        private System.Windows.Forms.Label lblYPackage;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.ComboBox cmbTitle;
        private DsName dsType;
        private System.Windows.Forms.NotifyIcon notifyIcon;
        private System.Windows.Forms.Button btnFileWrite;
        private System.Windows.Forms.ToolStripButton tsbReplace;
        private System.Windows.Forms.CheckBox chkInvert;
    }
}