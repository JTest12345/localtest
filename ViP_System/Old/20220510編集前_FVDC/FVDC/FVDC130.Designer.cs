namespace FVDC
{
    partial class FVDC130
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dgvLotType = new System.Windows.Forms.DataGridView();
            this.dsFVDC130 = new FVDC.dsFVDC130();
            this.dsType = new FVDC.DsName();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.lblMessage = new System.Windows.Forms.Label();
            this.cmsDataGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmDetail = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCause = new System.Windows.Forms.ToolStripMenuItem();
            this.TexAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Lot_No = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type_New = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type_Old = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotType)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC130)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsType)).BeginInit();
            this.cmsDataGridView.SuspendLayout();
            this.SuspendLayout();
            // 
            // dgvLotType
            // 
            this.dgvLotType.AllowUserToAddRows = false;
            this.dgvLotType.AllowUserToDeleteRows = false;
            this.dgvLotType.AllowUserToResizeColumns = false;
            this.dgvLotType.AllowUserToResizeRows = false;
            this.dgvLotType.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvLotType.AutoGenerateColumns = false;
            this.dgvLotType.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLotType.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvLotType.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvLotType.ColumnHeadersVisible = false;
            this.dgvLotType.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TexAction,
            this.Lot_No,
            this.Type_New,
            this.Type_Old});
            this.dgvLotType.ContextMenuStrip = this.cmsDataGridView;
            this.dgvLotType.DataMember = "LotType";
            this.dgvLotType.DataSource = this.dsFVDC130;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvLotType.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvLotType.Location = new System.Drawing.Point(12, 29);
            this.dgvLotType.Name = "dgvLotType";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvLotType.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.dgvLotType.RowHeadersVisible = false;
            this.dgvLotType.RowHeadersWidth = 10;
            this.dgvLotType.RowTemplate.Height = 21;
            this.dgvLotType.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvLotType.ShowCellErrors = false;
            this.dgvLotType.ShowCellToolTips = false;
            this.dgvLotType.ShowEditingIcon = false;
            this.dgvLotType.ShowRowErrors = false;
            this.dgvLotType.Size = new System.Drawing.Size(338, 204);
            this.dgvLotType.TabIndex = 1;
            this.dgvLotType.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dgvLotType.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dgvLotType.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dataGridView_CellContextMenuStripNeeded);
            // 
            // dsFVDC130
            // 
            this.dsFVDC130.DataSetName = "dsFVDC130";
            this.dsFVDC130.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // dsType
            // 
            this.dsType.DataSetName = "DsName";
            this.dsType.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(158, 246);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 24);
            this.btnOK.TabIndex = 2;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(261, 246);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 24);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // lblMessage
            // 
            this.lblMessage.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.lblMessage.AutoSize = true;
            this.lblMessage.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblMessage.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblMessage.Location = new System.Drawing.Point(12, 9);
            this.lblMessage.Name = "lblMessage";
            this.lblMessage.Size = new System.Drawing.Size(338, 16);
            this.lblMessage.TabIndex = 0;
            this.lblMessage.Text = "対象ロットのタイプを変更してOKを押して下さい。";
            // 
            // cmsDataGridView
            // 
            this.cmsDataGridView.AutoSize = false;
            this.cmsDataGridView.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.cmsDataGridView.Font = new System.Drawing.Font("Yu Gothic UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmsDataGridView.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.cmsDataGridView.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.tsmDetail,
            this.tsmDevice,
            this.tsmCause});
            this.cmsDataGridView.Name = "contextMenuStrip";
            this.cmsDataGridView.ShowImageMargin = false;
            this.cmsDataGridView.ShowItemToolTips = false;
            this.cmsDataGridView.Size = new System.Drawing.Size(120, 70);
            this.cmsDataGridView.Opening += new System.ComponentModel.CancelEventHandler(this.cmsDataGridView_Opening);
            this.cmsDataGridView.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.cmsDataGridView_ItemClicked);
            // 
            // tsmDetail
            // 
            this.tsmDetail.Name = "tsmDetail";
            this.tsmDetail.Size = new System.Drawing.Size(160, 22);
            this.tsmDetail.Text = "詳細画面表示";
            // 
            // tsmDevice
            // 
            this.tsmDevice.Name = "tsmDevice";
            this.tsmDevice.Size = new System.Drawing.Size(160, 22);
            this.tsmDevice.Text = "機器情報表示";
            // 
            // tsmCause
            // 
            this.tsmCause.Name = "tsmCause";
            this.tsmCause.Size = new System.Drawing.Size(160, 22);
            this.tsmCause.Text = "原因/対策画面表示";
            // 
            // TexAction
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Red;
            this.TexAction.DefaultCellStyle = dataGridViewCellStyle2;
            this.TexAction.FillWeight = 40F;
            this.TexAction.Frozen = true;
            this.TexAction.HeaderText = "";
            this.TexAction.MinimumWidth = 40;
            this.TexAction.Name = "TexAction";
            this.TexAction.ReadOnly = true;
            this.TexAction.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.TexAction.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.TexAction.Width = 40;
            // 
            // Lot_No
            // 
            this.Lot_No.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.Lot_No.DataPropertyName = "Lot_No";
            dataGridViewCellStyle3.Padding = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Lot_No.DefaultCellStyle = dataGridViewCellStyle3;
            this.Lot_No.HeaderText = "Lot_No";
            this.Lot_No.Name = "Lot_No";
            this.Lot_No.ReadOnly = true;
            this.Lot_No.Width = 5;
            // 
            // Type_New
            // 
            this.Type_New.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.Type_New.DataPropertyName = "Type_New";
            dataGridViewCellStyle4.Padding = new System.Windows.Forms.Padding(5, 0, 0, 0);
            this.Type_New.DefaultCellStyle = dataGridViewCellStyle4;
            this.Type_New.HeaderText = "Type_New";
            this.Type_New.Name = "Type_New";
            this.Type_New.ReadOnly = true;
            // 
            // Type_Old
            // 
            this.Type_Old.DataPropertyName = "Type_Old";
            this.Type_Old.HeaderText = "Type_Old";
            this.Type_Old.Name = "Type_Old";
            this.Type_Old.ReadOnly = true;
            this.Type_Old.Visible = false;
            // 
            // FVDC130
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(362, 282);
            this.Controls.Add(this.lblMessage);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dgvLotType);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FVDC130";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "タイプ切替";
            this.Load += new System.EventHandler(this.FVDC130_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dgvLotType)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC130)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsType)).EndInit();
            this.cmsDataGridView.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dgvLotType;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private DsName dsType;
        private System.Windows.Forms.Label lblMessage;
        private System.Windows.Forms.ContextMenuStrip cmsDataGridView;
        private System.Windows.Forms.ToolStripMenuItem tsmDetail;
        private System.Windows.Forms.ToolStripMenuItem tsmDevice;
        private System.Windows.Forms.ToolStripMenuItem tsmCause;
        private dsFVDC130 dsFVDC130;
        private System.Windows.Forms.DataGridViewTextBoxColumn TexAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn Lot_No;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type_New;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type_Old;
    }
}