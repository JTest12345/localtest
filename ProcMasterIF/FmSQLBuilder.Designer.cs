namespace ProcMasterIF
{
    partial class FmSQLBuilder
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
            this.cmb_root = new System.Windows.Forms.ComboBox();
            this.cmb_subcat = new System.Windows.Forms.ComboBox();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.cmb_sqltype = new System.Windows.Forms.ComboBox();
            this.consoleBox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            this.SuspendLayout();
            // 
            // cmb_root
            // 
            this.cmb_root.FormattingEnabled = true;
            this.cmb_root.Items.AddRange(new object[] {
            "共通台帳",
            "機種プロファイル"});
            this.cmb_root.Location = new System.Drawing.Point(12, 12);
            this.cmb_root.Name = "cmb_root";
            this.cmb_root.Size = new System.Drawing.Size(254, 20);
            this.cmb_root.TabIndex = 0;
            // 
            // cmb_subcat
            // 
            this.cmb_subcat.FormattingEnabled = true;
            this.cmb_subcat.Items.AddRange(new object[] {
            "工程定義台帳",
            "設備定義台帳"});
            this.cmb_subcat.Location = new System.Drawing.Point(12, 50);
            this.cmb_subcat.Name = "cmb_subcat";
            this.cmb_subcat.Size = new System.Drawing.Size(254, 20);
            this.cmb_subcat.TabIndex = 0;
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(12, 94);
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(87, 19);
            this.numericUpDown1.TabIndex = 1;
            // 
            // cmb_sqltype
            // 
            this.cmb_sqltype.FormattingEnabled = true;
            this.cmb_sqltype.Items.AddRange(new object[] {
            "select",
            "insert",
            "update",
            "delete"});
            this.cmb_sqltype.Location = new System.Drawing.Point(105, 94);
            this.cmb_sqltype.Name = "cmb_sqltype";
            this.cmb_sqltype.Size = new System.Drawing.Size(161, 20);
            this.cmb_sqltype.TabIndex = 2;
            // 
            // consoleBox
            // 
            this.consoleBox.Location = new System.Drawing.Point(12, 136);
            this.consoleBox.Multiline = true;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.consoleBox.Size = new System.Drawing.Size(726, 100);
            this.consoleBox.TabIndex = 9;
            // 
            // FmSQLBuilder
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(751, 252);
            this.Controls.Add(this.consoleBox);
            this.Controls.Add(this.cmb_sqltype);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.cmb_subcat);
            this.Controls.Add(this.cmb_root);
            this.Name = "FmSQLBuilder";
            this.Text = "FmSQLBuilder";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox cmb_root;
        private System.Windows.Forms.ComboBox cmb_subcat;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.ComboBox cmb_sqltype;
        private System.Windows.Forms.TextBox consoleBox;
    }
}