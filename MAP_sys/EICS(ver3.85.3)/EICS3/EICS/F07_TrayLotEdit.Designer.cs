namespace EICS
{
    partial class F07_TrayLotEdit
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtbNascaLot = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtbTrayID = new System.Windows.Forms.TextBox();
            this.chkbEffectFG = new System.Windows.Forms.CheckBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.dgvTnSortingTrayLot = new System.Windows.Forms.DataGridView();
            this.btnRegister = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dgvTnSortingTrayLot)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 12);
            this.label1.TabIndex = 7;
            this.label1.Text = "NASCAロット番号";
            // 
            // txtbNascaLot
            // 
            this.txtbNascaLot.Location = new System.Drawing.Point(125, 6);
            this.txtbNascaLot.Name = "txtbNascaLot";
            this.txtbNascaLot.Size = new System.Drawing.Size(143, 19);
            this.txtbNascaLot.TabIndex = 9;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(12, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(42, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "トレイID";
            // 
            // txtbTrayID
            // 
            this.txtbTrayID.Location = new System.Drawing.Point(125, 31);
            this.txtbTrayID.Name = "txtbTrayID";
            this.txtbTrayID.Size = new System.Drawing.Size(143, 19);
            this.txtbTrayID.TabIndex = 9;
            // 
            // chkbEffectFG
            // 
            this.chkbEffectFG.AutoSize = true;
            this.chkbEffectFG.Location = new System.Drawing.Point(285, 8);
            this.chkbEffectFG.Name = "chkbEffectFG";
            this.chkbEffectFG.Size = new System.Drawing.Size(127, 16);
            this.chkbEffectFG.TabIndex = 10;
            this.chkbEffectFG.Text = "無効コードも検索する";
            this.chkbEffectFG.UseVisualStyleBackColor = true;
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(581, 20);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(78, 30);
            this.btnSearch.TabIndex = 11;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // dgvTnSortingTrayLot
            // 
            this.dgvTnSortingTrayLot.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvTnSortingTrayLot.Location = new System.Drawing.Point(12, 56);
            this.dgvTnSortingTrayLot.Name = "dgvTnSortingTrayLot";
            this.dgvTnSortingTrayLot.RowTemplate.Height = 21;
            this.dgvTnSortingTrayLot.Size = new System.Drawing.Size(647, 472);
            this.dgvTnSortingTrayLot.TabIndex = 12;
            // 
            // btnRegister
            // 
            this.btnRegister.Location = new System.Drawing.Point(581, 534);
            this.btnRegister.Name = "btnRegister";
            this.btnRegister.Size = new System.Drawing.Size(78, 30);
            this.btnRegister.TabIndex = 11;
            this.btnRegister.Text = "登録";
            this.btnRegister.UseVisualStyleBackColor = true;
            this.btnRegister.Click += new System.EventHandler(this.btnRegister_Click);
            // 
            // F07_TrayLotEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(671, 569);
            this.Controls.Add(this.dgvTnSortingTrayLot);
            this.Controls.Add(this.btnRegister);
            this.Controls.Add(this.btnSearch);
            this.Controls.Add(this.chkbEffectFG);
            this.Controls.Add(this.txtbTrayID);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtbNascaLot);
            this.Controls.Add(this.label1);
            this.Name = "F07_TrayLotEdit";
            this.Text = "F07 トレイロット編集画面";
            ((System.ComponentModel.ISupportInitialize)(this.dgvTnSortingTrayLot)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtbNascaLot;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtbTrayID;
        private System.Windows.Forms.CheckBox chkbEffectFG;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView dgvTnSortingTrayLot;
        private System.Windows.Forms.Button btnRegister;
    }
}