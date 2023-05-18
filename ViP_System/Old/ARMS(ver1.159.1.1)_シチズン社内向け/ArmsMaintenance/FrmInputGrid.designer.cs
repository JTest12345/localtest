namespace ArmsMaintenance
{
    partial class FrmInputGrid
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
            this.grdItems = new System.Windows.Forms.DataGridView();
            this.button1 = new System.Windows.Forms.Button();
            this.btnApply = new System.Windows.Forms.Button();
            this.LotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnClear = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
            this.SuspendLayout();
            // 
            // grdItems
            // 
            this.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.LotNo});
            this.grdItems.Location = new System.Drawing.Point(14, 48);
            this.grdItems.Name = "grdItems";
            this.grdItems.RowTemplate.Height = 21;
            this.grdItems.Size = new System.Drawing.Size(378, 404);
            this.grdItems.TabIndex = 0;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(14, 14);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(135, 27);
            this.button1.TabIndex = 1;
            this.button1.Text = "クリップボード貼付";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnApply
            // 
            this.btnApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnApply.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnApply.Location = new System.Drawing.Point(288, 458);
            this.btnApply.Name = "btnApply";
            this.btnApply.Size = new System.Drawing.Size(104, 28);
            this.btnApply.TabIndex = 9;
            this.btnApply.Text = "反映";
            this.btnApply.UseVisualStyleBackColor = true;
            this.btnApply.Click += new System.EventHandler(this.btnApply_Click);
            // 
            // LotNo
            // 
            this.LotNo.HeaderText = "ロットNo";
            this.LotNo.Name = "LotNo";
            this.LotNo.Width = 200;
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(171, 14);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(90, 27);
            this.btnClear.TabIndex = 10;
            this.btnClear.Text = "クリア";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // FrmInputGrid
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(406, 498);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btnApply);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.grdItems);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmInputGrid";
            this.Text = "ロット入力";
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdItems;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button btnApply;
        private System.Windows.Forms.DataGridViewTextBoxColumn LotNo;
        private System.Windows.Forms.Button btnClear;
    }
}