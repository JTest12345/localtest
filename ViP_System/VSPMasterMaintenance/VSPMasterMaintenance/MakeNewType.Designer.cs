
namespace VSPMasterMaintenance
{
    partial class MakeNewType
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
            this.typecheckresult = new System.Windows.Forms.TextBox();
            this.CheckType = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.CopyType_CB = new System.Windows.Forms.ComboBox();
            this.SuspendLayout();
            // 
            // typecheckresult
            // 
            this.typecheckresult.Location = new System.Drawing.Point(38, 69);
            this.typecheckresult.Multiline = true;
            this.typecheckresult.Name = "typecheckresult";
            this.typecheckresult.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.typecheckresult.Size = new System.Drawing.Size(443, 453);
            this.typecheckresult.TabIndex = 1;
            // 
            // CheckType
            // 
            this.CheckType.Location = new System.Drawing.Point(391, 26);
            this.CheckType.Name = "CheckType";
            this.CheckType.Size = new System.Drawing.Size(90, 26);
            this.CheckType.TabIndex = 2;
            this.CheckType.Text = "品種チェック";
            this.CheckType.UseVisualStyleBackColor = true;
            this.CheckType.Click += new System.EventHandler(this.CheckType_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(38, 32);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "登録品種名";
            // 
            // CopyType_CB
            // 
            this.CopyType_CB.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.CopyType_CB.FormattingEnabled = true;
            this.CopyType_CB.Location = new System.Drawing.Point(133, 29);
            this.CopyType_CB.Name = "CopyType_CB";
            this.CopyType_CB.Size = new System.Drawing.Size(228, 23);
            this.CopyType_CB.TabIndex = 4;
            // 
            // MakeNewType
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(500, 559);
            this.Controls.Add(this.CopyType_CB);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.CheckType);
            this.Controls.Add(this.typecheckresult);
            this.Name = "MakeNewType";
            this.Text = "登録品種名";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox typecheckresult;
        private System.Windows.Forms.Button CheckType;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox CopyType_CB;
    }
}