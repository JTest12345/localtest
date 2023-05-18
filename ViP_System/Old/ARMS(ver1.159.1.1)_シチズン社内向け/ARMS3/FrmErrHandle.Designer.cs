namespace ARMS3
{
    partial class FrmErrHandle
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
            this.btnExit = new System.Windows.Forms.Button();
            this.btnStopWav = new System.Windows.Forms.Button();
            this.btnRetry = new System.Windows.Forms.Button();
            this.btnDischarge = new System.Windows.Forms.Button();
            this.txtMagazine = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.lblMachine = new System.Windows.Forms.Label();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // btnExit
            // 
            this.btnExit.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnExit.Location = new System.Drawing.Point(128, 182);
            this.btnExit.Name = "btnExit";
            this.btnExit.Size = new System.Drawing.Size(75, 23);
            this.btnExit.TabIndex = 17;
            this.btnExit.Text = "終了";
            this.btnExit.UseVisualStyleBackColor = true;
            this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
            // 
            // btnStopWav
            // 
            this.btnStopWav.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnStopWav.Location = new System.Drawing.Point(12, 182);
            this.btnStopWav.Name = "btnStopWav";
            this.btnStopWav.Size = new System.Drawing.Size(75, 23);
            this.btnStopWav.TabIndex = 16;
            this.btnStopWav.Text = "警報停止";
            this.btnStopWav.UseVisualStyleBackColor = true;
            this.btnStopWav.Click += new System.EventHandler(this.btnStopWav_Click);
            // 
            // btnRetry
            // 
            this.btnRetry.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnRetry.Enabled = false;
            this.btnRetry.Location = new System.Drawing.Point(209, 182);
            this.btnRetry.Name = "btnRetry";
            this.btnRetry.Size = new System.Drawing.Size(75, 23);
            this.btnRetry.TabIndex = 15;
            this.btnRetry.Text = "リトライ";
            this.btnRetry.UseVisualStyleBackColor = true;
            this.btnRetry.Visible = false;
            this.btnRetry.Click += new System.EventHandler(this.btnRetry_Click);
            // 
            // btnDischarge
            // 
            this.btnDischarge.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnDischarge.Location = new System.Drawing.Point(290, 182);
            this.btnDischarge.Name = "btnDischarge";
            this.btnDischarge.Size = new System.Drawing.Size(75, 23);
            this.btnDischarge.TabIndex = 14;
            this.btnDischarge.Text = "排出";
            this.btnDischarge.UseVisualStyleBackColor = true;
            this.btnDischarge.Click += new System.EventHandler(this.btnDischarge_Click);
            // 
            // txtMagazine
            // 
            this.txtMagazine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtMagazine.Location = new System.Drawing.Point(62, 150);
            this.txtMagazine.Name = "txtMagazine";
            this.txtMagazine.ReadOnly = true;
            this.txtMagazine.Size = new System.Drawing.Size(167, 19);
            this.txtMagazine.TabIndex = 13;
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(13, 153);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 12);
            this.label1.TabIndex = 12;
            this.label1.Text = "マガジン";
            // 
            // txtMachine
            // 
            this.txtMachine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.txtMachine.Location = new System.Drawing.Point(62, 125);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(167, 19);
            this.txtMachine.TabIndex = 11;
            // 
            // lblMachine
            // 
            this.lblMachine.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblMachine.AutoSize = true;
            this.lblMachine.Location = new System.Drawing.Point(13, 128);
            this.lblMachine.Name = "lblMachine";
            this.lblMachine.Size = new System.Drawing.Size(29, 12);
            this.lblMachine.TabIndex = 10;
            this.lblMachine.Text = "装置";
            // 
            // txtMessage
            // 
            this.txtMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtMessage.Location = new System.Drawing.Point(12, 12);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ReadOnly = true;
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(353, 106);
            this.txtMessage.TabIndex = 9;
            // 
            // FrmErrHandle
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(380, 215);
            this.Controls.Add(this.btnExit);
            this.Controls.Add(this.btnStopWav);
            this.Controls.Add(this.btnRetry);
            this.Controls.Add(this.btnDischarge);
            this.Controls.Add(this.txtMagazine);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtMachine);
            this.Controls.Add(this.lblMachine);
            this.Controls.Add(this.txtMessage);
            this.Name = "FrmErrHandle";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FrmErrHandle";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmErrHandle_FormClosing);
            this.Load += new System.EventHandler(this.FrmErrHandle_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Button btnStopWav;
        private System.Windows.Forms.Button btnRetry;
        private System.Windows.Forms.Button btnDischarge;
        private System.Windows.Forms.TextBox txtMagazine;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.Label lblMachine;
        private System.Windows.Forms.TextBox txtMessage;
    }
}