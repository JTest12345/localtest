namespace ArmsMaintenance
{
    partial class FrmImportLot
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
            this.txtLog = new System.Windows.Forms.TextBox();
            this.txtLotno = new System.Windows.Forms.TextBox();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.取り込みToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rdoLot = new System.Windows.Forms.RadioButton();
            this.rdoAllInfo = new System.Windows.Forms.RadioButton();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Location = new System.Drawing.Point(207, 40);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(390, 262);
            this.txtLog.TabIndex = 10;
            // 
            // txtLotno
            // 
            this.txtLotno.Location = new System.Drawing.Point(12, 89);
            this.txtLotno.Multiline = true;
            this.txtLotno.Name = "txtLotno";
            this.txtLotno.Size = new System.Drawing.Size(189, 213);
            this.txtLotno.TabIndex = 11;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.取り込みToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(609, 24);
            this.menuStrip1.TabIndex = 13;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 取り込みToolStripMenuItem
            // 
            this.取り込みToolStripMenuItem.Name = "取り込みToolStripMenuItem";
            this.取り込みToolStripMenuItem.Size = new System.Drawing.Size(62, 20);
            this.取り込みToolStripMenuItem.Text = "取り込み";
            this.取り込みToolStripMenuItem.Click += new System.EventHandler(this.取り込みToolStripMenuItem_Click);
            // 
            // rdoLot
            // 
            this.rdoLot.AutoSize = true;
            this.rdoLot.Location = new System.Drawing.Point(12, 39);
            this.rdoLot.Name = "rdoLot";
            this.rdoLot.Size = new System.Drawing.Size(94, 18);
            this.rdoLot.TabIndex = 14;
            this.rdoLot.TabStop = true;
            this.rdoLot.Text = "指図情報のみ";
            this.rdoLot.UseVisualStyleBackColor = true;
            // 
            // rdoAllInfo
            // 
            this.rdoAllInfo.AutoSize = true;
            this.rdoAllInfo.Location = new System.Drawing.Point(12, 63);
            this.rdoAllInfo.Name = "rdoAllInfo";
            this.rdoAllInfo.Size = new System.Drawing.Size(147, 18);
            this.rdoAllInfo.TabIndex = 15;
            this.rdoAllInfo.TabStop = true;
            this.rdoAllInfo.Text = "指図・実績・マガジン情報";
            this.rdoAllInfo.UseVisualStyleBackColor = true;
            // 
            // FrmImportLot
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(609, 310);
            this.Controls.Add(this.rdoAllInfo);
            this.Controls.Add(this.rdoLot);
            this.Controls.Add(this.txtLotno);
            this.Controls.Add(this.txtLog);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.MainMenuStrip = this.menuStrip1;
            this.Name = "FrmImportLot";
            this.Text = "FrmImportLot";
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.TextBox txtLotno;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 取り込みToolStripMenuItem;
        private System.Windows.Forms.RadioButton rdoLot;
        private System.Windows.Forms.RadioButton rdoAllInfo;
    }
}