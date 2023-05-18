namespace FVDC
{
    partial class FVDC210
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
            this.lblReplace = new System.Windows.Forms.Label();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.txtReplace = new System.Windows.Forms.TextBox();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // lblReplace
            // 
            this.lblReplace.BackColor = System.Drawing.SystemColors.Info;
            this.lblReplace.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblReplace.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblReplace.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblReplace.Location = new System.Drawing.Point(12, 55);
            this.lblReplace.Name = "lblReplace";
            this.lblReplace.Size = new System.Drawing.Size(116, 23);
            this.lblReplace.TabIndex = 2;
            this.lblReplace.Text = "置換後の文字列";
            this.lblReplace.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // lblSearch
            // 
            this.lblSearch.BackColor = System.Drawing.SystemColors.Info;
            this.lblSearch.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblSearch.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblSearch.Location = new System.Drawing.Point(12, 18);
            this.lblSearch.Name = "lblSearch";
            this.lblSearch.Size = new System.Drawing.Size(116, 23);
            this.lblSearch.TabIndex = 0;
            this.lblSearch.Text = "検索する文字列";
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // txtSearch
            // 
            this.txtSearch.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtSearch.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtSearch.Location = new System.Drawing.Point(127, 18);
            this.txtSearch.MaxLength = 3;
            this.txtSearch.Name = "txtSearch";
            this.txtSearch.Size = new System.Drawing.Size(116, 23);
            this.txtSearch.TabIndex = 1;
            this.txtSearch.WordWrap = false;
            // 
            // txtReplace
            // 
            this.txtReplace.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtReplace.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtReplace.Location = new System.Drawing.Point(127, 55);
            this.txtReplace.MaxLength = 3;
            this.txtReplace.Name = "txtReplace";
            this.txtReplace.Size = new System.Drawing.Size(116, 23);
            this.txtReplace.TabIndex = 3;
            this.txtReplace.WordWrap = false;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(156, 92);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 25);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUpdate.Location = new System.Drawing.Point(54, 92);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(87, 25);
            this.btnUpdate.TabIndex = 4;
            this.btnUpdate.Text = "置　換";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // FVDC210
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(257, 132);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblSearch);
            this.Controls.Add(this.lblReplace);
            this.Controls.Add(this.txtReplace);
            this.Controls.Add(this.txtSearch);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(273, 171);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(273, 171);
            this.Name = "FVDC210";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FVDC210　置換";
            this.TopMost = true;
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblReplace;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.TextBox txtReplace;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Button btnUpdate;
    }
}