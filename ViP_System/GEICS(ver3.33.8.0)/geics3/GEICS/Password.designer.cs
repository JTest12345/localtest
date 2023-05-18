namespace GEICS
{
    partial class Password
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
			this.txtbPassword = new System.Windows.Forms.TextBox();
			this.btnEICSStop = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.label1 = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// txtbPassword
			// 
			this.txtbPassword.Location = new System.Drawing.Point(12, 43);
			this.txtbPassword.Name = "txtbPassword";
			this.txtbPassword.PasswordChar = '*';
			this.txtbPassword.Size = new System.Drawing.Size(230, 19);
			this.txtbPassword.TabIndex = 0;
			// 
			// btnEICSStop
			// 
			this.btnEICSStop.Location = new System.Drawing.Point(12, 78);
			this.btnEICSStop.Name = "btnEICSStop";
			this.btnEICSStop.Size = new System.Drawing.Size(75, 23);
			this.btnEICSStop.TabIndex = 1;
			this.btnEICSStop.Text = "OK";
			this.btnEICSStop.UseVisualStyleBackColor = true;
			this.btnEICSStop.Click += new System.EventHandler(this.btnEICSStop_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Location = new System.Drawing.Point(167, 78);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(75, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 18);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(204, 12);
			this.label1.TabIndex = 3;
			this.label1.Text = "終了する場合、CLOSEと入力して下さい。";
			// 
			// Password
			// 
			this.AcceptButton = this.btnEICSStop;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(254, 113);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnEICSStop);
			this.Controls.Add(this.txtbPassword);
			this.Name = "Password";
			this.Text = "Password";
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtbPassword;
        private System.Windows.Forms.Button btnEICSStop;
		private System.Windows.Forms.Button btnCancel;
		private System.Windows.Forms.Label label1;
    }
}