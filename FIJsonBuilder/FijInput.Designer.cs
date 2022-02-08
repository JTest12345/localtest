namespace FIFJsonBuilder
{
    partial class fm_input
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
            this.tb_sharestring = new System.Windows.Forms.TextBox();
            this.bt_input = new System.Windows.Forms.Button();
            this.bt_cancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // tb_sharestring
            // 
            this.tb_sharestring.Location = new System.Drawing.Point(12, 13);
            this.tb_sharestring.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.tb_sharestring.Name = "tb_sharestring";
            this.tb_sharestring.Size = new System.Drawing.Size(314, 24);
            this.tb_sharestring.TabIndex = 0;
            // 
            // bt_input
            // 
            this.bt_input.Location = new System.Drawing.Point(168, 44);
            this.bt_input.Name = "bt_input";
            this.bt_input.Size = new System.Drawing.Size(75, 23);
            this.bt_input.TabIndex = 2;
            this.bt_input.Text = "入力";
            this.bt_input.UseVisualStyleBackColor = true;
            this.bt_input.Click += new System.EventHandler(this.button1_Click);
            // 
            // bt_cancel
            // 
            this.bt_cancel.Location = new System.Drawing.Point(249, 44);
            this.bt_cancel.Name = "bt_cancel";
            this.bt_cancel.Size = new System.Drawing.Size(75, 23);
            this.bt_cancel.TabIndex = 3;
            this.bt_cancel.Text = "キャンセル";
            this.bt_cancel.UseVisualStyleBackColor = true;
            this.bt_cancel.Click += new System.EventHandler(this.button2_Click);
            // 
            // fm_input
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(334, 74);
            this.ControlBox = false;
            this.Controls.Add(this.bt_cancel);
            this.Controls.Add(this.bt_input);
            this.Controls.Add(this.tb_sharestring);
            this.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(350, 113);
            this.MinimumSize = new System.Drawing.Size(350, 113);
            this.Name = "fm_input";
            this.Text = "を入力してください";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox tb_sharestring;
        private System.Windows.Forms.Button bt_input;
        private System.Windows.Forms.Button bt_cancel;
    }
}