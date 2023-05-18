
namespace VSPMasterMaintenance
{
    partial class MakeNewMachine
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
            this.CheckMachine = new System.Windows.Forms.Button();
            this.maccombo = new System.Windows.Forms.ComboBox();
            this.maccheckresult = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CheckMachine
            // 
            this.CheckMachine.Location = new System.Drawing.Point(325, 15);
            this.CheckMachine.Name = "CheckMachine";
            this.CheckMachine.Size = new System.Drawing.Size(144, 41);
            this.CheckMachine.TabIndex = 0;
            this.CheckMachine.Text = "設備登録チェック";
            this.CheckMachine.UseVisualStyleBackColor = true;
            this.CheckMachine.Click += new System.EventHandler(this.CheckMachine_Click);
            // 
            // maccombo
            // 
            this.maccombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.maccombo.FormattingEnabled = true;
            this.maccombo.Location = new System.Drawing.Point(22, 33);
            this.maccombo.Name = "maccombo";
            this.maccombo.Size = new System.Drawing.Size(267, 23);
            this.maccombo.TabIndex = 1;
            // 
            // maccheckresult
            // 
            this.maccheckresult.Location = new System.Drawing.Point(22, 94);
            this.maccheckresult.Multiline = true;
            this.maccheckresult.Name = "maccheckresult";
            this.maccheckresult.Size = new System.Drawing.Size(477, 280);
            this.maccheckresult.TabIndex = 2;
            // 
            // MakeNewMachine
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(511, 406);
            this.Controls.Add(this.maccheckresult);
            this.Controls.Add(this.maccombo);
            this.Controls.Add(this.CheckMachine);
            this.Name = "MakeNewMachine";
            this.Text = "MakeNewMachine";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button CheckMachine;
        private System.Windows.Forms.ComboBox maccombo;
        private System.Windows.Forms.TextBox maccheckresult;
    }
}