namespace AoiMagBuilder
{
    partial class fm4MCode
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
            this.txt_4Mcode = new System.Windows.Forms.TextBox();
            this.bt_clear4mcd = new System.Windows.Forms.Button();
            this.bt_Ok4mcd = new System.Windows.Forms.Button();
            this.bt_Cancel4mcd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txt_4Mcode
            // 
            this.txt_4Mcode.Location = new System.Drawing.Point(12, 12);
            this.txt_4Mcode.Name = "txt_4Mcode";
            this.txt_4Mcode.Size = new System.Drawing.Size(424, 19);
            this.txt_4Mcode.TabIndex = 0;
            this.txt_4Mcode.KeyDown += new System.Windows.Forms.KeyEventHandler(this.txt_4Mcode_KeyDown);
            // 
            // bt_clear4mcd
            // 
            this.bt_clear4mcd.Location = new System.Drawing.Point(12, 37);
            this.bt_clear4mcd.Name = "bt_clear4mcd";
            this.bt_clear4mcd.Size = new System.Drawing.Size(75, 23);
            this.bt_clear4mcd.TabIndex = 1;
            this.bt_clear4mcd.Text = "クリア";
            this.bt_clear4mcd.UseVisualStyleBackColor = true;
            this.bt_clear4mcd.Click += new System.EventHandler(this.bt_clear4mcd_Click);
            // 
            // bt_Ok4mcd
            // 
            this.bt_Ok4mcd.Location = new System.Drawing.Point(361, 37);
            this.bt_Ok4mcd.Name = "bt_Ok4mcd";
            this.bt_Ok4mcd.Size = new System.Drawing.Size(75, 23);
            this.bt_Ok4mcd.TabIndex = 1;
            this.bt_Ok4mcd.Text = "OK";
            this.bt_Ok4mcd.UseVisualStyleBackColor = true;
            this.bt_Ok4mcd.Click += new System.EventHandler(this.bt_Ok4mcd_Click);
            // 
            // bt_Cancel4mcd
            // 
            this.bt_Cancel4mcd.Location = new System.Drawing.Point(269, 37);
            this.bt_Cancel4mcd.Name = "bt_Cancel4mcd";
            this.bt_Cancel4mcd.Size = new System.Drawing.Size(75, 23);
            this.bt_Cancel4mcd.TabIndex = 1;
            this.bt_Cancel4mcd.Text = "キャンセル";
            this.bt_Cancel4mcd.UseVisualStyleBackColor = true;
            this.bt_Cancel4mcd.Click += new System.EventHandler(this.bt_Cancel4mcd_Click);
            // 
            // fm4MCode
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(449, 69);
            this.ControlBox = false;
            this.Controls.Add(this.bt_Ok4mcd);
            this.Controls.Add(this.bt_Cancel4mcd);
            this.Controls.Add(this.bt_clear4mcd);
            this.Controls.Add(this.txt_4Mcode);
            this.MaximumSize = new System.Drawing.Size(465, 108);
            this.MinimumSize = new System.Drawing.Size(465, 108);
            this.Name = "fm4MCode";
            this.Text = "4MCode";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_4Mcode;
        private System.Windows.Forms.Button bt_clear4mcd;
        private System.Windows.Forms.Button bt_Ok4mcd;
        private System.Windows.Forms.Button bt_Cancel4mcd;
    }
}