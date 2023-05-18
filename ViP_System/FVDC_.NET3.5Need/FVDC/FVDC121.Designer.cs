namespace FVDC
{
    partial class FVDC121
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVDC121));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lotNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnFullON = new System.Windows.Forms.Button();
            this.btnFullOFF = new System.Windows.Forms.Button();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblLotSelect = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.dgvLot = new System.Windows.Forms.DataGridView();
            this.chkLine = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Lot_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dsFVDC120 = new FVDC.dsFVDC120();
            this.dgvProfile = new System.Windows.Forms.DataGridView();
            this.dgvButton = new System.Windows.Forms.DataGridViewButtonColumn();
            this.keyCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dsProfile = new FVDC.DsName();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLot)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC120)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfile)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProfile)).BeginInit();
            this.SuspendLayout();
            // 
            // lotNODataGridViewTextBoxColumn
            // 
            this.lotNODataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.lotNODataGridViewTextBoxColumn.DataPropertyName = "Lot_NO";
            this.lotNODataGridViewTextBoxColumn.HeaderText = "Lot_NO";
            this.lotNODataGridViewTextBoxColumn.Name = "lotNODataGridViewTextBoxColumn";
            // 
            // btnFullON
            // 
            this.btnFullON.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFullON.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFullON.Image = ((System.Drawing.Image)(resources.GetObject("btnFullON.Image")));
            this.btnFullON.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFullON.Location = new System.Drawing.Point(241, 322);
            this.btnFullON.Name = "btnFullON";
            this.btnFullON.Size = new System.Drawing.Size(89, 24);
            this.btnFullON.TabIndex = 7;
            this.btnFullON.Text = "　　全選択";
            this.btnFullON.UseVisualStyleBackColor = true;
            this.btnFullON.Click += new System.EventHandler(this.btnFullON_Click);
            // 
            // btnFullOFF
            // 
            this.btnFullOFF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFullOFF.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFullOFF.Image = ((System.Drawing.Image)(resources.GetObject("btnFullOFF.Image")));
            this.btnFullOFF.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFullOFF.Location = new System.Drawing.Point(344, 322);
            this.btnFullOFF.Name = "btnFullOFF";
            this.btnFullOFF.Size = new System.Drawing.Size(89, 24);
            this.btnFullOFF.TabIndex = 8;
            this.btnFullOFF.Text = "　　全解除";
            this.btnFullOFF.UseVisualStyleBackColor = true;
            this.btnFullOFF.Click += new System.EventHandler(this.btnFullOFF_Click);
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(241, 358);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 24);
            this.btnOK.TabIndex = 9;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(344, 358);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 24);
            this.btnCancel.TabIndex = 10;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblLotSelect
            // 
            this.lblLotSelect.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblLotSelect.AutoSize = true;
            this.lblLotSelect.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLotSelect.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblLotSelect.Location = new System.Drawing.Point(217, 259);
            this.lblLotSelect.Name = "lblLotSelect";
            this.lblLotSelect.Size = new System.Drawing.Size(200, 16);
            this.lblLotSelect.TabIndex = 5;
            this.lblLotSelect.Text = "検証ロット一覧へ追加したい";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.label1.Location = new System.Drawing.Point(217, 284);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(220, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "ロットを左記から選択して下さい";
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.label2.Location = new System.Drawing.Point(217, 184);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(202, 16);
            this.label2.TabIndex = 2;
            this.label2.Text = "上記検証用プロファイル一覧";
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.label3.Location = new System.Drawing.Point(217, 209);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(182, 16);
            this.label3.TabIndex = 3;
            this.label3.Text = "の列左側のボタンを押すと";
            // 
            // label4
            // 
            this.label4.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.label4.Location = new System.Drawing.Point(217, 234);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(223, 16);
            this.label4.TabIndex = 4;
            this.label4.Text = "有効なロットが表示されますので";
            // 
            // dgvLot
            // 
            this.dgvLot.AllowDrop = true;
            this.dgvLot.AllowUserToResizeColumns = false;
            this.dgvLot.AllowUserToResizeRows = false;
            this.dgvLot.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLot.AutoGenerateColumns = false;
            this.dgvLot.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLot.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLot.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLot.ColumnHeadersVisible = false;
            this.dgvLot.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkLine,
            this.Lot_NO});
            this.dgvLot.DataMember = "FVDC120";
            this.dgvLot.DataSource = this.dsFVDC120;
            this.dgvLot.Location = new System.Drawing.Point(8, 180);
            this.dgvLot.Name = "dgvLot";
            this.dgvLot.RowHeadersVisible = false;
            this.dgvLot.RowHeadersWidth = 20;
            this.dgvLot.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvLot.RowTemplate.Height = 21;
            this.dgvLot.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLot.ShowCellErrors = false;
            this.dgvLot.ShowCellToolTips = false;
            this.dgvLot.ShowEditingIcon = false;
            this.dgvLot.ShowRowErrors = false;
            this.dgvLot.Size = new System.Drawing.Size(200, 202);
            this.dgvLot.TabIndex = 1;
            // 
            // chkLine
            // 
            dataGridViewCellStyle2.NullValue = false;
            this.chkLine.DefaultCellStyle = dataGridViewCellStyle2;
            this.chkLine.Frozen = true;
            this.chkLine.HeaderText = "";
            this.chkLine.Name = "chkLine";
            this.chkLine.Width = 20;
            // 
            // Lot_NO
            // 
            this.Lot_NO.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Lot_NO.DataPropertyName = "Lot_NO";
            this.Lot_NO.HeaderText = "Lot_NO";
            this.Lot_NO.Name = "Lot_NO";
            // 
            // dsFVDC120
            // 
            this.dsFVDC120.DataSetName = "dsFVDC120";
            this.dsFVDC120.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dgvProfile
            // 
            this.dgvProfile.AllowDrop = true;
            this.dgvProfile.AllowUserToResizeColumns = false;
            this.dgvProfile.AllowUserToResizeRows = false;
            this.dgvProfile.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvProfile.AutoGenerateColumns = false;
            this.dgvProfile.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvProfile.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dgvProfile.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvProfile.ColumnHeadersVisible = false;
            this.dgvProfile.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dgvButton,
            this.keyCDDataGridViewTextBoxColumn,
            this.dataNMDataGridViewTextBoxColumn});
            this.dgvProfile.DataMember = "Name";
            this.dgvProfile.DataSource = this.dsProfile;
            this.dgvProfile.Location = new System.Drawing.Point(8, 8);
            this.dgvProfile.Name = "dgvProfile";
            this.dgvProfile.RowHeadersVisible = false;
            this.dgvProfile.RowHeadersWidth = 20;
            this.dgvProfile.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dgvProfile.RowTemplate.Height = 21;
            this.dgvProfile.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvProfile.ShowCellErrors = false;
            this.dgvProfile.ShowCellToolTips = false;
            this.dgvProfile.ShowEditingIcon = false;
            this.dgvProfile.ShowRowErrors = false;
            this.dgvProfile.Size = new System.Drawing.Size(425, 158);
            this.dgvProfile.TabIndex = 0;
            this.dgvProfile.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellContentClick);
            // 
            // dgvButton
            // 
            this.dgvButton.FillWeight = 20F;
            this.dgvButton.Frozen = true;
            this.dgvButton.HeaderText = "";
            this.dgvButton.MinimumWidth = 20;
            this.dgvButton.Name = "dgvButton";
            this.dgvButton.ReadOnly = true;
            this.dgvButton.Width = 20;
            // 
            // keyCDDataGridViewTextBoxColumn
            // 
            this.keyCDDataGridViewTextBoxColumn.DataPropertyName = "Key_CD";
            this.keyCDDataGridViewTextBoxColumn.FillWeight = 50F;
            this.keyCDDataGridViewTextBoxColumn.Frozen = true;
            this.keyCDDataGridViewTextBoxColumn.HeaderText = "Key_CD";
            this.keyCDDataGridViewTextBoxColumn.MinimumWidth = 50;
            this.keyCDDataGridViewTextBoxColumn.Name = "keyCDDataGridViewTextBoxColumn";
            this.keyCDDataGridViewTextBoxColumn.Width = 50;
            // 
            // dataNMDataGridViewTextBoxColumn
            // 
            this.dataNMDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.dataNMDataGridViewTextBoxColumn.DataPropertyName = "Data_NM";
            this.dataNMDataGridViewTextBoxColumn.HeaderText = "Data_NM";
            this.dataNMDataGridViewTextBoxColumn.Name = "dataNMDataGridViewTextBoxColumn";
            // 
            // dsProfile
            // 
            this.dsProfile.DataSetName = "DsName";
            this.dsProfile.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // FVDC121
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(441, 394);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.lblLotSelect);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvLot);
            this.Controls.Add(this.btnFullON);
            this.Controls.Add(this.btnFullOFF);
            this.Controls.Add(this.dgvProfile);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(457, 433);
            this.Name = "FVDC121";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "テストプロファイル検索";
            this.Load += new System.EventHandler(this.FVDC121_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLot)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC120)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvProfile)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsProfile)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvProfile;
        private System.Windows.Forms.DataGridViewTextBoxColumn lotNODataGridViewTextBoxColumn;
        private dsFVDC120 dsFVDC120;
        private System.Windows.Forms.Button btnFullON;
        private System.Windows.Forms.Button btnFullOFF;
        private System.Windows.Forms.DataGridView dgvLot;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.Label lblLotSelect;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private DsName dsProfile;
        private System.Windows.Forms.DataGridViewButtonColumn dgvButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn keyCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lot_NO;
    }
}