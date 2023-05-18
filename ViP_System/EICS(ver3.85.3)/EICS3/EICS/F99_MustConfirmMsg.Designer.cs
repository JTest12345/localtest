namespace EICS
{
	partial class F99_MustConfirmMsg
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
			this.textBox = new System.Windows.Forms.TextBox();
			this.btnConfirmed = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// textBox
			// 
			this.textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
			this.textBox.BackColor = System.Drawing.SystemColors.Window;
			this.textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
			this.textBox.ForeColor = System.Drawing.SystemColors.WindowText;
			this.textBox.Location = new System.Drawing.Point(0, 3);
			this.textBox.Multiline = true;
			this.textBox.Name = "textBox";
			this.textBox.ReadOnly = true;
			this.textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textBox.Size = new System.Drawing.Size(638, 280);
			this.textBox.TabIndex = 0;
			// 
			// btnConfirmed
			// 
			this.btnConfirmed.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.btnConfirmed.Location = new System.Drawing.Point(0, 289);
			this.btnConfirmed.Name = "btnConfirmed";
			this.btnConfirmed.Size = new System.Drawing.Size(257, 23);
			this.btnConfirmed.TabIndex = 1;
			this.btnConfirmed.TabStop = false;
			this.btnConfirmed.Text = "OK";
			this.btnConfirmed.UseVisualStyleBackColor = true;
			this.btnConfirmed.Click += new System.EventHandler(this.btnConfirmed_Click);
			// 
			// btnCancel
			// 
			this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.btnCancel.Location = new System.Drawing.Point(381, 289);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(257, 23);
			this.btnCancel.TabIndex = 2;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "キャンセル";
			this.btnCancel.UseVisualStyleBackColor = true;
			this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
			// 
			// F99_MustConfirmMsg
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(639, 314);
			this.ControlBox = false;
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnConfirmed);
			this.Controls.Add(this.textBox);
			this.Name = "F99_MustConfirmMsg";
			this.Text = "F99_MustConfirmMsg";
			this.Load += new System.EventHandler(this.F99_MustConfirmMsg_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox textBox;
		private System.Windows.Forms.Button btnConfirmed;
		private System.Windows.Forms.Button btnCancel;
	}
}