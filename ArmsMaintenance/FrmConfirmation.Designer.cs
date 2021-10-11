namespace ArmsMaintenance
{
    partial class FrmConfirmation
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
            this.grdDefect = new System.Windows.Forms.DataGridView();
            this.btnok = new System.Windows.Forms.Button();
            this.btncancel = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.grdDefect)).BeginInit();
            this.SuspendLayout();
            // 
            // grdDefect
            // 
            this.grdDefect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdDefect.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.grdDefect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDefect.Location = new System.Drawing.Point(12, 31);
            this.grdDefect.Name = "grdDefect";
            this.grdDefect.RowTemplate.Height = 21;
            this.grdDefect.Size = new System.Drawing.Size(383, 190);
            this.grdDefect.TabIndex = 0;
            // 
            // btnok
            // 
            this.btnok.Location = new System.Drawing.Point(239, 227);
            this.btnok.Name = "btnok";
            this.btnok.Size = new System.Drawing.Size(75, 23);
            this.btnok.TabIndex = 1;
            this.btnok.Text = "OK";
            this.btnok.UseVisualStyleBackColor = true;
            this.btnok.Click += new System.EventHandler(this.btnok_Click);
            // 
            // btncancel
            // 
            this.btncancel.Location = new System.Drawing.Point(320, 227);
            this.btncancel.Name = "btncancel";
            this.btncancel.Size = new System.Drawing.Size(75, 23);
            this.btncancel.TabIndex = 2;
            this.btncancel.Text = "キャンセル";
            this.btncancel.UseVisualStyleBackColor = true;
            this.btncancel.Click += new System.EventHandler(this.btncancel_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 9F);
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(129, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "不良情報を保存しますか?";
            // 
            // FrmConfirmation
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(407, 262);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.btncancel);
            this.Controls.Add(this.btnok);
            this.Controls.Add(this.grdDefect);
            this.Name = "FrmConfirmation";
            this.Text = "確認";
            this.Load += new System.EventHandler(this.FrmConfirmation_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdDefect)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView grdDefect;
        private System.Windows.Forms.Button btnok;
        private System.Windows.Forms.Button btncancel;
        private System.Windows.Forms.Label label1;
    }
}