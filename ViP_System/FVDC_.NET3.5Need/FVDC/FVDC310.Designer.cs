namespace FVDC
{
    partial class FVDC310
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FVDC310));
            this.dsFVDC310 = new FVDC.DsFree();
            this.btnOK = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.dataGridView = new System.Windows.Forms.DataGridView();
            this.chkLine = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.DeviceID = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.btnFullON = new System.Windows.Forms.Button();
            this.btnFullOFF = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC310)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // dsFVDC310
            // 
            this.dsFVDC310.DataSetName = "DsFree";
            this.dsFVDC310.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // btnOK
            // 
            this.btnOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.btnOK.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnOK.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOK.Location = new System.Drawing.Point(8, 281);
            this.btnOK.Name = "btnOK";
            this.btnOK.Size = new System.Drawing.Size(89, 24);
            this.btnOK.TabIndex = 3;
            this.btnOK.Text = "OK";
            this.btnOK.UseVisualStyleBackColor = true;
            this.btnOK.Click += new System.EventHandler(this.btnOK_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCancel.Location = new System.Drawing.Point(119, 281);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(89, 24);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // dataGridView
            // 
            this.dataGridView.AllowDrop = true;
            this.dataGridView.AllowUserToResizeColumns = false;
            this.dataGridView.AllowUserToResizeRows = false;
            this.dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView.AutoGenerateColumns = false;
            this.dataGridView.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView.ColumnHeadersVisible = false;
            this.dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.chkLine,
            this.DeviceID});
            this.dataGridView.DataMember = "List";
            this.dataGridView.DataSource = this.dsFVDC310;
            this.dataGridView.Location = new System.Drawing.Point(8, 53);
            this.dataGridView.Name = "dataGridView";
            this.dataGridView.RowHeadersVisible = false;
            this.dataGridView.RowHeadersWidth = 20;
            this.dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.dataGridView.RowTemplate.Height = 21;
            this.dataGridView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView.ShowCellErrors = false;
            this.dataGridView.ShowCellToolTips = false;
            this.dataGridView.ShowEditingIcon = false;
            this.dataGridView.ShowRowErrors = false;
            this.dataGridView.Size = new System.Drawing.Size(200, 212);
            this.dataGridView.TabIndex = 2;
            this.dataGridView.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.dataGridView_RowsAdded);
            this.dataGridView.KeyDown += new System.Windows.Forms.KeyEventHandler(this.dataGridView_KeyDown);
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
            // DeviceID
            // 
            this.DeviceID.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.DeviceID.DataPropertyName = "Name";
            this.DeviceID.HeaderText = "Name";
            this.DeviceID.Name = "DeviceID";
            // 
            // btnFullON
            // 
            this.btnFullON.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFullON.Image = ((System.Drawing.Image)(resources.GetObject("btnFullON.Image")));
            this.btnFullON.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFullON.Location = new System.Drawing.Point(8, 12);
            this.btnFullON.Name = "btnFullON";
            this.btnFullON.Size = new System.Drawing.Size(89, 24);
            this.btnFullON.TabIndex = 0;
            this.btnFullON.Text = "　　全選択";
            this.btnFullON.UseVisualStyleBackColor = true;
            this.btnFullON.Click += new System.EventHandler(this.btnFullON_Click);
            // 
            // btnFullOFF
            // 
            this.btnFullOFF.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnFullOFF.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnFullOFF.Image = ((System.Drawing.Image)(resources.GetObject("btnFullOFF.Image")));
            this.btnFullOFF.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnFullOFF.Location = new System.Drawing.Point(119, 12);
            this.btnFullOFF.Name = "btnFullOFF";
            this.btnFullOFF.Size = new System.Drawing.Size(89, 24);
            this.btnFullOFF.TabIndex = 1;
            this.btnFullOFF.Text = "　　全解除";
            this.btnFullOFF.UseVisualStyleBackColor = true;
            this.btnFullOFF.Click += new System.EventHandler(this.btnFullOFF_Click);
            // 
            // FVDC310
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(216, 317);
            this.Controls.Add(this.btnOK);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.dataGridView);
            this.Controls.Add(this.btnFullON);
            this.Controls.Add(this.btnFullOFF);
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(232, 800);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(232, 171);
            this.Name = "FVDC310";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FVDC310 複数選択";
            this.Load += new System.EventHandler(this.FVDC310_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dsFVDC310)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private DsFree dsFVDC310;
        private System.Windows.Forms.Button btnOK;
        private System.Windows.Forms.Button btnCancel;
        private System.Windows.Forms.DataGridView dataGridView;
        private System.Windows.Forms.Button btnFullON;
        private System.Windows.Forms.Button btnFullOFF;
        private System.Windows.Forms.DataGridViewCheckBoxColumn chkLine;
        private System.Windows.Forms.DataGridViewTextBoxColumn DeviceID;
    }
}