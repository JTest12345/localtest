namespace GEICS
{
    partial class F01_Login
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F01_Login));
			this.cmbServer = new SLCommonLib.UCComboBox();
			this.txtEmployeeID = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.label6 = new System.Windows.Forms.Label();
			this.label3 = new System.Windows.Forms.Label();
			this.label4 = new System.Windows.Forms.Label();
			this.cmbServerList = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.btnLogin = new System.Windows.Forms.Button();
			this.btnExit = new System.Windows.Forms.Button();
			this.pictureBox1 = new System.Windows.Forms.PictureBox();
			this.pictureBox2 = new System.Windows.Forms.PictureBox();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
			this.SuspendLayout();
			// 
			// cmbServer
			// 
			this.cmbServer.FormattingEnabled = true;
			resources.ApplyResources(this.cmbServer, "cmbServer");
			this.cmbServer.Name = "cmbServer";
			this.cmbServer.SourceNVC = null;
			// 
			// txtEmployeeID
			// 
			resources.ApplyResources(this.txtEmployeeID, "txtEmployeeID");
			this.txtEmployeeID.Name = "txtEmployeeID";
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// label2
			// 
			resources.ApplyResources(this.label2, "label2");
			this.label2.Name = "label2";
			// 
			// label6
			// 
			resources.ApplyResources(this.label6, "label6");
			this.label6.Name = "label6";
			// 
			// label3
			// 
			resources.ApplyResources(this.label3, "label3");
			this.label3.Name = "label3";
			// 
			// label4
			// 
			resources.ApplyResources(this.label4, "label4");
			this.label4.Name = "label4";
			// 
			// cmbServerList
			// 
			this.cmbServerList.FormattingEnabled = true;
			resources.ApplyResources(this.cmbServerList, "cmbServerList");
			this.cmbServerList.Name = "cmbServerList";
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// btnLogin
			// 
			resources.ApplyResources(this.btnLogin, "btnLogin");
			this.btnLogin.Name = "btnLogin";
			this.btnLogin.UseVisualStyleBackColor = true;
			this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
			// 
			// btnExit
			// 
			resources.ApplyResources(this.btnExit, "btnExit");
			this.btnExit.Name = "btnExit";
			this.btnExit.UseVisualStyleBackColor = true;
			this.btnExit.Click += new System.EventHandler(this.btnExit_Click);
			// 
			// pictureBox1
			// 
			this.pictureBox1.BackColor = System.Drawing.SystemColors.Control;
			this.pictureBox1.Image = global::GEICS.Properties.Resources.LOGO_NICHIA;
			resources.ApplyResources(this.pictureBox1, "pictureBox1");
			this.pictureBox1.Name = "pictureBox1";
			this.pictureBox1.TabStop = false;
			// 
			// pictureBox2
			// 
			resources.ApplyResources(this.pictureBox2, "pictureBox2");
			this.pictureBox2.Name = "pictureBox2";
			this.pictureBox2.TabStop = false;
			// 
			// F01_Login
			// 
			this.AcceptButton = this.btnLogin;
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.label5);
			this.Controls.Add(this.cmbServerList);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.btnExit);
			this.Controls.Add(this.pictureBox1);
			this.Controls.Add(this.label6);
			this.Controls.Add(this.pictureBox2);
			this.Controls.Add(this.btnLogin);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.txtEmployeeID);
			this.Controls.Add(this.cmbServer);
			this.MaximizeBox = false;
			this.Name = "F01_Login";
			this.Load += new System.EventHandler(this.frmLogin_Load);
			((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private SLCommonLib.UCComboBox cmbServer;
        private System.Windows.Forms.TextBox txtEmployeeID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnExit;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox cmbServerList;
		private System.Windows.Forms.Label label5;
    }
}