namespace ARMS3
{
    partial class FrmCVWaitInput
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
            this.label1 = new System.Windows.Forms.Label();
            this.btnApply = new System.Windows.Forms.Button();
            this.nudCVWaitMinutes = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.nudCVWaitMinutes)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(18, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(194, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "常温CV内の待機時間を分単位で設定";
            // 
            // btnApply
            // 
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(146, 28);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(75, 23);
            this.btnApply.TabIndex = 4;
            this.btnApply.Text = "設定反映";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // nudCVWaitMinutes
            // 
            this.nudCVWaitMinutes.Location = new System.Drawing.Point(20, 32);
            this.nudCVWaitMinutes.Maximum = new decimal(new int[] {
            43200,
            0,
            0,
            0});
            this.nudCVWaitMinutes.Name = "nudCVWaitMinutes";
            this.nudCVWaitMinutes.Size = new System.Drawing.Size(120, 19);
            this.nudCVWaitMinutes.TabIndex = 3;
            this.nudCVWaitMinutes.Value = new decimal(new int[] {
            840,
            0,
            0,
            0});
            // 
            // FrmCVWaitInput
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(242, 60);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.nudCVWaitMinutes);
            this.MaximizeBox = false;
            this.Name = "FrmCVWaitInput";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "常温CV待機時間設定";
            this.Load += new System.EventHandler(this.FrmCVWaitInput_Load);
            ((System.ComponentModel.ISupportInitialize)(this.nudCVWaitMinutes)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.NumericUpDown nudCVWaitMinutes;
    }
}