namespace FVDC
{
    partial class FVDC140
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
            this.lblDeviceID = new System.Windows.Forms.Label();
            this.txtDeviceID = new System.Windows.Forms.TextBox();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblDeviceID
            // 
            this.lblDeviceID.BackColor = System.Drawing.SystemColors.Info;
            this.lblDeviceID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDeviceID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblDeviceID.Font = new System.Drawing.Font("MS UI Gothic", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblDeviceID.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDeviceID.Location = new System.Drawing.Point(12, 12);
            this.lblDeviceID.Name = "lblDeviceID";
            this.lblDeviceID.Size = new System.Drawing.Size(75, 23);
            this.lblDeviceID.TabIndex = 4;
            this.lblDeviceID.Text = "設備番号";
            this.lblDeviceID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtDeviceID
            // 
            this.txtDeviceID.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtDeviceID.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtDeviceID.Location = new System.Drawing.Point(86, 12);
            this.txtDeviceID.Name = "txtDeviceID";
            this.txtDeviceID.Size = new System.Drawing.Size(118, 23);
            this.txtDeviceID.TabIndex = 5;
            this.txtDeviceID.TextChanged += new System.EventHandler(this.txtDeviceID_TextChanged);
            // 
            // btnOK
            // 
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(12, 46);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 24);
            this.btnOK.TabIndex = 6;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(115, 46);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 24);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FVDC140
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(215, 82);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.lblDeviceID);
            this.Controls.Add(this.txtDeviceID);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(231, 121);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(231, 121);
            this.Name = "FVDC140";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FVDC140　設備番号入力";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.FVDC140_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblDeviceID;
        private System.Windows.Forms.TextBox txtDeviceID;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
    }
}