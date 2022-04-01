namespace ArmsMonitor
{
    partial class FrmMonitor
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
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.chkMacList = new System.Windows.Forms.CheckedListBox();
            this.btnChangeMac = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label3 = new System.Windows.Forms.Label();
            this.grdResinLimit = new System.Windows.Forms.DataGridView();
            this.btnDBPreOvnPlasma = new System.Windows.Forms.Button();
            this.btnCheck = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.grdLimit = new System.Windows.Forms.DataGridView();
            this.colChk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.grdMaterialLimit = new System.Windows.Forms.DataGridView();
            this.splitter1 = new System.Windows.Forms.Splitter();
            this.button1 = new System.Windows.Forms.Button();
            this.panel1.SuspendLayout();
            this.panel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResinLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdLimit)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterialLimit)).BeginInit();
            this.SuspendLayout();
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // chkMacList
            // 
            this.chkMacList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chkMacList.Enabled = false;
            this.chkMacList.FormattingEnabled = true;
            this.chkMacList.Location = new System.Drawing.Point(6, 60);
            this.chkMacList.Name = "chkMacList";
            this.chkMacList.Size = new System.Drawing.Size(188, 650);
            this.chkMacList.TabIndex = 6;
            // 
            // btnChangeMac
            // 
            this.btnChangeMac.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnChangeMac.Location = new System.Drawing.Point(119, 30);
            this.btnChangeMac.Name = "btnChangeMac";
            this.btnChangeMac.Size = new System.Drawing.Size(75, 27);
            this.btnChangeMac.TabIndex = 7;
            this.btnChangeMac.Text = "変更";
            this.btnChangeMac.UseVisualStyleBackColor = true;
            this.btnChangeMac.Click += new System.EventHandler(this.btnChangeMac_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 14);
            this.label2.TabIndex = 8;
            this.label2.Text = "監視設備";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.chkMacList);
            this.panel1.Controls.Add(this.btnChangeMac);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.panel1.Location = new System.Drawing.Point(741, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(200, 734);
            this.panel1.TabIndex = 9;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.grdResinLimit);
            this.panel2.Controls.Add(this.button1);
            this.panel2.Controls.Add(this.btnDBPreOvnPlasma);
            this.panel2.Controls.Add(this.btnCheck);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Controls.Add(this.grdLimit);
            this.panel2.Controls.Add(this.grdMaterialLimit);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel2.Location = new System.Drawing.Point(0, 0);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(731, 734);
            this.panel2.TabIndex = 10;
            // 
            // label3
            // 
            this.label3.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 551);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(127, 14);
            this.label3.TabIndex = 9;
            this.label3.Text = "調合樹脂有効期限監視";
            // 
            // grdResinLimit
            // 
            this.grdResinLimit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdResinLimit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResinLimit.Location = new System.Drawing.Point(15, 568);
            this.grdResinLimit.Name = "grdResinLimit";
            this.grdResinLimit.ReadOnly = true;
            this.grdResinLimit.RowTemplate.Height = 21;
            this.grdResinLimit.Size = new System.Drawing.Size(710, 146);
            this.grdResinLimit.TabIndex = 8;
            // 
            // btnDBPreOvnPlasma
            // 
            this.btnDBPreOvnPlasma.Location = new System.Drawing.Point(173, 12);
            this.btnDBPreOvnPlasma.Name = "btnDBPreOvnPlasma";
            this.btnDBPreOvnPlasma.Size = new System.Drawing.Size(152, 27);
            this.btnDBPreOvnPlasma.TabIndex = 7;
            this.btnDBPreOvnPlasma.Text = "樹脂カップ時間監視";
            this.btnDBPreOvnPlasma.UseVisualStyleBackColor = true;
            this.btnDBPreOvnPlasma.Click += new System.EventHandler(this.btnFrmSRTimeKeeper_Click);
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(15, 12);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(152, 27);
            this.btnCheck.TabIndex = 1;
            this.btnCheck.Text = "警告無効フラグ設定";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 385);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(103, 14);
            this.label1.TabIndex = 5;
            this.label1.Text = "資材有効期限監視";
            // 
            // grdLimit
            // 
            this.grdLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdLimit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdLimit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChk});
            this.grdLimit.Location = new System.Drawing.Point(14, 42);
            this.grdLimit.Name = "grdLimit";
            this.grdLimit.RowTemplate.Height = 21;
            this.grdLimit.Size = new System.Drawing.Size(711, 340);
            this.grdLimit.TabIndex = 0;
            // 
            // colChk
            // 
            this.colChk.Frozen = true;
            this.colChk.HeaderText = "無効";
            this.colChk.Name = "colChk";
            this.colChk.Width = 50;
            // 
            // grdMaterialLimit
            // 
            this.grdMaterialLimit.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdMaterialLimit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMaterialLimit.Location = new System.Drawing.Point(15, 402);
            this.grdMaterialLimit.Name = "grdMaterialLimit";
            this.grdMaterialLimit.ReadOnly = true;
            this.grdMaterialLimit.RowTemplate.Height = 21;
            this.grdMaterialLimit.Size = new System.Drawing.Size(710, 146);
            this.grdMaterialLimit.TabIndex = 4;
            // 
            // splitter1
            // 
            this.splitter1.Dock = System.Windows.Forms.DockStyle.Right;
            this.splitter1.Location = new System.Drawing.Point(731, 0);
            this.splitter1.Name = "splitter1";
            this.splitter1.Size = new System.Drawing.Size(10, 734);
            this.splitter1.TabIndex = 11;
            this.splitter1.TabStop = false;
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(573, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(152, 27);
            this.button1.TabIndex = 7;
            this.button1.Text = "DB硬化前プラズマ監視";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.btnDBPreOvnPlasma_Click);
            // 
            // FrmMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(941, 734);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.splitter1);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmMonitor";
            this.Text = "ARMS時間監視";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmMonitor_FormClosing);
            this.Load += new System.EventHandler(this.FrmMonitor_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResinLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdLimit)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterialLimit)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.CheckedListBox chkMacList;
        private System.Windows.Forms.Button btnChangeMac;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChk;
        private System.Windows.Forms.DataGridView grdLimit;
        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.DataGridView grdMaterialLimit;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Splitter splitter1;
        private System.Windows.Forms.Button btnDBPreOvnPlasma;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DataGridView grdResinLimit;
        private System.Windows.Forms.Button button1;
    }
}