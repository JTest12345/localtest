namespace FVDC
{
    partial class FVDC300
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVDC300));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.lblLine = new System.Windows.Forms.Label();
            this.cmbLine = new System.Windows.Forms.ComboBox();
            this.dsLine = new FVDC.DsName();
            this.btnFind = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.TextAction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.plantcd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.plantnm = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clasnm_New = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.macno_New = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.clasnm_Old = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.macno_Old = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cmsDataGridView = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.tsmDetail = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmDevice = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmCause = new System.Windows.Forms.ToolStripMenuItem();
            this.dsFVDC300 = new FVDC.dsFVDC300();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.btnSelect = new System.Windows.Forms.Button();
            this.lblDeviceID = new System.Windows.Forms.Label();
            this.txtDeviceID = new System.Windows.Forms.TextBox();
            this.btnReset = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dsLine)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.cmsDataGridView.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC300)).BeginInit();
            this.SuspendLayout();
            // 
            // lblLine
            // 
            this.lblLine.BackColor = System.Drawing.SystemColors.Info;
            this.lblLine.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblLine.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblLine.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblLine.Location = new System.Drawing.Point(12, 18);
            this.lblLine.Name = "lblLine";
            this.lblLine.Size = new System.Drawing.Size(75, 23);
            this.lblLine.TabIndex = 0;
            this.lblLine.Text = "対象ライン";
            this.lblLine.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cmbLine
            // 
            this.cmbLine.CausesValidation = false;
            this.cmbLine.DataSource = this.dsLine;
            this.cmbLine.DisplayMember = "Name.Data_NM";
            this.cmbLine.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cmbLine.FormattingEnabled = true;
            this.cmbLine.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.cmbLine.Location = new System.Drawing.Point(86, 18);
            this.cmbLine.Name = "cmbLine";
            this.cmbLine.Size = new System.Drawing.Size(67, 23);
            this.cmbLine.TabIndex = 1;
            this.cmbLine.ValueMember = "Name.Key_CD";
            this.cmbLine.SelectedIndexChanged += new System.EventHandler(this.cmbLine_SelectedIndexChanged);
            // 
            // dsLine
            // 
            this.dsLine.DataSetName = "DsName";
            this.dsLine.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnFind
            // 
            this.btnFind.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFind.Image = ((System.Drawing.Image)(resources.GetObject("btnFind.Image")));
            this.btnFind.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFind.Location = new System.Drawing.Point(411, 17);
            this.btnFind.Name = "btnFind";
            this.btnFind.Size = new System.Drawing.Size(89, 24);
            this.btnFind.TabIndex = 5;
            this.btnFind.Text = "　検　索";
            this.btnFind.UseVisualStyleBackColor = true;
            this.btnFind.Click += new System.EventHandler(this.btnFind_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowUserToAddRows = false;
            this.dataGridView.AllowUserToDeleteRows = false;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.GradientActiveCaption;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TextAction,
            this.plantcd,
            this.plantnm,
            this.clasnm_New,
            this.macno_New,
            this.clasnm_Old,
            this.macno_Old});
            this.dataGridView.ContextMenuStrip = this.cmsDataGridView;
            this.dataGridView.DataMember = "Machine";
            this.dataGridView.DataSource = this.dsFVDC300;
            this.dataGridView.Location = new System.Drawing.Point(12, 58);
            this.dataGridView.MultiSelect = false;
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.RowTemplate.Height = 21;
            this.dataGridView.Size = new System.Drawing.Size(488, 341);
            this.dataGridView.TabIndex = 6;
            this.dataGridView.CellClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellClick);
            this.dataGridView.CellContextMenuStripNeeded += new System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventHandler(this.dataGridView_CellContextMenuStripNeeded);
            // 
            // TextAction
            // 
            this.TextAction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.TextAction.DefaultCellStyle = dataGridViewCellStyle2;
            this.TextAction.FillWeight = 40F;
            this.TextAction.HeaderText = "";
            this.TextAction.MinimumWidth = 40;
            this.TextAction.Name = "TextAction";
            this.TextAction.ReadOnly = true;
            this.TextAction.Width = 40;
            // 
            // plantcd
            // 
            this.plantcd.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.plantcd.DataPropertyName = "plantcd";
            this.plantcd.FillWeight = 80F;
            this.plantcd.HeaderText = "設備番号";
            this.plantcd.MinimumWidth = 80;
            this.plantcd.Name = "plantcd";
            this.plantcd.ReadOnly = true;
            this.plantcd.Width = 80;
            // 
            // plantnm
            // 
            this.plantnm.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            this.plantnm.DataPropertyName = "plantnm";
            this.plantnm.HeaderText = "号機";
            this.plantnm.Name = "plantnm";
            this.plantnm.ReadOnly = true;
            this.plantnm.Width = 54;
            // 
            // clasnm_New
            // 
            this.clasnm_New.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.clasnm_New.DataPropertyName = "clasnm_New";
            this.clasnm_New.HeaderText = "設備名称";
            this.clasnm_New.Name = "clasnm_New";
            this.clasnm_New.ReadOnly = true;
            this.clasnm_New.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            // 
            // macno_New
            // 
            this.macno_New.DataPropertyName = "macno_New";
            this.macno_New.HeaderText = "macno_New";
            this.macno_New.Name = "macno_New";
            this.macno_New.Visible = false;
            // 
            // clasnm_Old
            // 
            this.clasnm_Old.DataPropertyName = "clasnm_Old";
            this.clasnm_Old.HeaderText = "clasnm_Old";
            this.clasnm_Old.Name = "clasnm_Old";
            this.clasnm_Old.Visible = false;
            // 
            // macno_Old
            // 
            this.macno_Old.DataPropertyName = "macno_Old";
            this.macno_Old.HeaderText = "macno_Old";
            this.macno_Old.Name = "macno_Old";
            this.macno_Old.Visible = false;
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
            // dsFVDC300
            // 
            this.dsFVDC300.DataSetName = "dsFVDC300";
            this.dsFVDC300.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(308, 414);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 24);
            this.btnOK.TabIndex = 8;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(411, 414);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 24);
            this.btnCancel.TabIndex = 9;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSelect
            // 
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.Location = new System.Drawing.Point(371, 18);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(19, 23);
            this.btnSelect.TabIndex = 4;
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // lblDeviceID
            // 
            this.lblDeviceID.BackColor = System.Drawing.SystemColors.Info;
            this.lblDeviceID.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.lblDeviceID.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.lblDeviceID.Font = new System.Drawing.Font("MS UI Gothic", 9.5F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblDeviceID.ForeColor = System.Drawing.SystemColors.ControlText;
            this.lblDeviceID.Location = new System.Drawing.Point(175, 18);
            this.lblDeviceID.Name = "lblDeviceID";
            this.lblDeviceID.Size = new System.Drawing.Size(75, 23);
            this.lblDeviceID.TabIndex = 2;
            this.lblDeviceID.Text = "設備番号";
            this.lblDeviceID.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.lblDeviceID.DoubleClick += new System.EventHandler(this.lblDeviceID_DoubleClick);
            // 
            // txtDeviceID
            // 
            this.txtDeviceID.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtDeviceID.ImeMode = System.Windows.Forms.ImeMode.Off;
            this.txtDeviceID.Location = new System.Drawing.Point(249, 18);
            this.txtDeviceID.Name = "txtDeviceID";
            this.txtDeviceID.Size = new System.Drawing.Size(126, 23);
            this.txtDeviceID.TabIndex = 3;
            this.txtDeviceID.TextChanged += new System.EventHandler(this.txtDeviceID_TextChanged);
            // 
            // btnReset
            // 
            this.btnReset.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnReset.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnReset.Image = ((System.Drawing.Image)(resources.GetObject("btnReset.Image")));
            this.btnReset.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnReset.Location = new System.Drawing.Point(12, 414);
            this.btnReset.Name = "btnReset";
            this.btnReset.Size = new System.Drawing.Size(89, 24);
            this.btnReset.TabIndex = 7;
            this.btnReset.Text = "　クリア";
            this.btnReset.UseVisualStyleBackColor = true;
            this.btnReset.Click += new System.EventHandler(this.btnReset_Click);
            // 
            // FVDC300
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(512, 450);
            this.Controls.Add(this.btnReset);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.lblDeviceID);
            this.Controls.Add(this.txtDeviceID);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnFind);
            this.Controls.Add(this.lblLine);
            this.Controls.Add(this.cmbLine);
            this.MaximumSize = new System.Drawing.Size(528, 1098);
            this.MinimumSize = new System.Drawing.Size(528, 189);
            this.Name = "FVDC300";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FVDC300　設備切替";
            this.Load += new System.EventHandler(this.FVDC300_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsLine)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.cmsDataGridView.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC300)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label lblLine;
        private System.Windows.Forms.ComboBox cmbLine;
        private System.Windows.Forms.Button btnFind;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private dsFVDC300 dsFVDC300;
        private DsName dsLine;
        private System.Windows.Forms.ContextMenuStrip cmsDataGridView;
        private System.Windows.Forms.ToolStripMenuItem tsmDetail;
        private System.Windows.Forms.ToolStripMenuItem tsmDevice;
        private System.Windows.Forms.ToolStripMenuItem tsmCause;
        private System.Windows.Forms.Button btnSelect;
        private System.Windows.Forms.Label lblDeviceID;
        private System.Windows.Forms.TextBox txtDeviceID;
        private System.Windows.Forms.DataGridViewTextBoxColumn TextAction;
        private System.Windows.Forms.DataGridViewTextBoxColumn plantcd;
        private System.Windows.Forms.DataGridViewTextBoxColumn plantnm;
        private System.Windows.Forms.DataGridViewTextBoxColumn clasnm_New;
        private System.Windows.Forms.DataGridViewTextBoxColumn macno_New;
        private System.Windows.Forms.DataGridViewTextBoxColumn clasnm_Old;
        private System.Windows.Forms.DataGridViewTextBoxColumn macno_Old;
        private System.Windows.Forms.Button btnReset;
    }
}