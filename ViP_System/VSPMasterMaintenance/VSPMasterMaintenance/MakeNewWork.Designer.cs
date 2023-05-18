
namespace VSPMasterMaintenance
{
    partial class MakeNewWork
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
            this.CheckWork = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.workcombo = new System.Windows.Forms.ComboBox();
            this.workcheckresult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CheckWork
            // 
            this.CheckWork.Location = new System.Drawing.Point(318, 21);
            this.CheckWork.Name = "CheckWork";
            this.CheckWork.Size = new System.Drawing.Size(119, 36);
            this.CheckWork.TabIndex = 0;
            this.CheckWork.Text = "作業チェック";
            this.CheckWork.UseVisualStyleBackColor = true;
            this.CheckWork.Click += new System.EventHandler(this.CheckWork_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(22, 32);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 1;
            this.label1.Text = "作業名";
            // 
            // workcombo
            // 
            this.workcombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.workcombo.FormattingEnabled = true;
            this.workcombo.Location = new System.Drawing.Point(82, 29);
            this.workcombo.Name = "workcombo";
            this.workcombo.Size = new System.Drawing.Size(212, 23);
            this.workcombo.TabIndex = 2;
            // 
            // workcheckresult
            // 
            this.workcheckresult.Location = new System.Drawing.Point(22, 83);
            this.workcheckresult.Multiline = true;
            this.workcheckresult.Name = "workcheckresult";
            this.workcheckresult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.workcheckresult.Size = new System.Drawing.Size(415, 309);
            this.workcheckresult.TabIndex = 3;
            // 
            // MakeNewWork
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(459, 450);
            this.Controls.Add(this.workcheckresult);
            this.Controls.Add(this.workcombo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CheckWork);
            this.Name = "MakeNewWork";
            this.Text = "MakeNewWork";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CheckWork;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox workcombo;
        private System.Windows.Forms.TextBox workcheckresult;
    }
}