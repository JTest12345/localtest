namespace GEICS
{
	partial class F100_MsgBox
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
			this.tbMsgBox = new System.Windows.Forms.TextBox();
			this.btOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// tbMsgBox
			// 
			this.tbMsgBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tbMsgBox.Location = new System.Drawing.Point(12, 12);
			this.tbMsgBox.MaxLength = 65535;
			this.tbMsgBox.Multiline = true;
			this.tbMsgBox.Name = "tbMsgBox";
			this.tbMsgBox.ReadOnly = true;
			this.tbMsgBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.tbMsgBox.Size = new System.Drawing.Size(808, 472);
			this.tbMsgBox.TabIndex = 0;
			this.tbMsgBox.WordWrap = false;
			// 
			// btOK
			// 
			this.btOK.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.btOK.Location = new System.Drawing.Point(584, 490);
			this.btOK.Name = "btOK";
			this.btOK.Size = new System.Drawing.Size(236, 23);
			this.btOK.TabIndex = 1;
			this.btOK.Text = "閉じる";
			this.btOK.UseVisualStyleBackColor = true;
			this.btOK.Click += new System.EventHandler(this.btOK_Click);
			// 
			// F100_MsgBox
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(832, 524);
			this.Controls.Add(this.btOK);
			this.Controls.Add(this.tbMsgBox);
			this.Name = "F100_MsgBox";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "F100_MsgBox";
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.TextBox tbMsgBox;
		private System.Windows.Forms.Button btOK;
	}
}