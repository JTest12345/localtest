namespace GEICS
{
    partial class frmQCBackground
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmQCBackground));
            this.btnFix = new System.Windows.Forms.Button();
            this.txtbBackground = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtbUpdUser = new System.Windows.Forms.TextBox();
            this.rbOn = new System.Windows.Forms.RadioButton();
            this.rbOff = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.btnCancel = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.lblCnfmNo = new System.Windows.Forms.Label();
            this.btnDelete = new System.Windows.Forms.Button();
            this.lstbFixedNM = new System.Windows.Forms.ListBox();
            this.btnInsert = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // btnFix
            // 
            this.btnFix.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnFix, "btnFix");
            this.btnFix.Name = "btnFix";
            this.btnFix.UseVisualStyleBackColor = true;
            this.btnFix.Click += new System.EventHandler(this.btnFix_Click);
            // 
            // txtbBackground
            // 
            resources.ApplyResources(this.txtbBackground, "txtbBackground");
            this.txtbBackground.Name = "txtbBackground";
            this.txtbBackground.TextChanged += new System.EventHandler(this.txtbBackground_TextChanged);
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
            // txtbUpdUser
            // 
            resources.ApplyResources(this.txtbUpdUser, "txtbUpdUser");
            this.txtbUpdUser.Name = "txtbUpdUser";
            this.txtbUpdUser.TextChanged += new System.EventHandler(this.txtbUpdUser_TextChanged);
            // 
            // rbOn
            // 
            resources.ApplyResources(this.rbOn, "rbOn");
            this.rbOn.Name = "rbOn";
            this.rbOn.TabStop = true;
            this.rbOn.UseVisualStyleBackColor = true;
            this.rbOn.CheckedChanged += new System.EventHandler(this.rbOn_CheckedChanged);
            // 
            // rbOff
            // 
            resources.ApplyResources(this.rbOff, "rbOff");
            this.rbOff.Name = "rbOff";
            this.rbOff.TabStop = true;
            this.rbOff.UseVisualStyleBackColor = true;
            this.rbOff.CheckedChanged += new System.EventHandler(this.rbOff_CheckedChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnCancel, "btnCancel");
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // lblCnfmNo
            // 
            resources.ApplyResources(this.lblCnfmNo, "lblCnfmNo");
            this.lblCnfmNo.Name = "lblCnfmNo";
            // 
            // btnDelete
            // 
            this.btnDelete.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            resources.ApplyResources(this.btnDelete, "btnDelete");
            this.btnDelete.Name = "btnDelete";
            this.btnDelete.UseVisualStyleBackColor = true;
            this.btnDelete.Click += new System.EventHandler(this.btnDelete_Click);
            // 
            // lstbFixedNM
            // 
            this.lstbFixedNM.FormattingEnabled = true;
            resources.ApplyResources(this.lstbFixedNM, "lstbFixedNM");
            this.lstbFixedNM.Name = "lstbFixedNM";
            // 
            // btnInsert
            // 
            resources.ApplyResources(this.btnInsert, "btnInsert");
            this.btnInsert.Name = "btnInsert";
            this.btnInsert.UseVisualStyleBackColor = true;
            this.btnInsert.Click += new System.EventHandler(this.btnInsert_Click);
            // 
            // frmQCBackground
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.btnInsert);
            this.Controls.Add(this.lstbFixedNM);
            this.Controls.Add(this.lblCnfmNo);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.rbOff);
            this.Controls.Add(this.rbOn);
            this.Controls.Add(this.txtbUpdUser);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtbBackground);
            this.Controls.Add(this.btnDelete);
            this.Controls.Add(this.btnFix);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmQCBackground";
            this.Load += new System.EventHandler(this.frmQCBackground_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnFix;
        private System.Windows.Forms.TextBox txtbBackground;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtbUpdUser;
        private System.Windows.Forms.RadioButton rbOn;
        private System.Windows.Forms.RadioButton rbOff;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label lblCnfmNo;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.ListBox lstbFixedNM;
        private System.Windows.Forms.Button btnInsert;
    }
}