namespace ArmsMaintenance
{
    partial class FrmMacMaster
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
            this.grdMachines = new System.Windows.Forms.DataGridView();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.ColAvailable = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.grdMachines)).BeginInit();
            this.SuspendLayout();
            // 
            // grdMachines
            // 
            this.grdMachines.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdMachines.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.grdMachines.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMachines.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ColAvailable});
            this.grdMachines.Location = new System.Drawing.Point(14, 14);
            this.grdMachines.Name = "grdMachines";
            this.grdMachines.RowTemplate.Height = 21;
            this.grdMachines.Size = new System.Drawing.Size(675, 395);
            this.grdMachines.TabIndex = 0;
            // 
            // btnUpdate
            // 
            this.btnUpdate.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUpdate.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnUpdate.Image = global::ArmsMaintenance.Properties.Resources.edit;
            this.btnUpdate.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdate.Location = new System.Drawing.Point(580, 415);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(109, 27);
            this.btnUpdate.TabIndex = 1;
            this.btnUpdate.Text = "更新";
            this.btnUpdate.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // ColAvailable
            // 
            this.ColAvailable.DataPropertyName = "ColAvailable";
            this.ColAvailable.Frozen = true;
            this.ColAvailable.HeaderText = "現在有効";
            this.ColAvailable.Name = "ColAvailable";
            this.ColAvailable.Width = 61;
            // 
            // FrmMacMaster
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(703, 454);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.grdMachines);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmMacMaster";
            this.Text = "設備有効フラグ設定";
            this.Load += new System.EventHandler(this.FrmMacMaster_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdMachines)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView grdMachines;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn ColAvailable;
    }
}