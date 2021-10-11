namespace ArmsMaintenance
{
    partial class FrmPreDBMonitor
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
            this.btnCheck = new System.Windows.Forms.Button();
            this.grdLimit = new System.Windows.Forms.DataGridView();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btnStopAlert = new System.Windows.Forms.Button();
            this.grdDBLoader = new System.Windows.Forms.DataGridView();
            this.btnIgnoreDBLoader = new System.Windows.Forms.Button();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.colChk2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.colChk = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.btnReload = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.grdLimit)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDBLoader)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // btnCheck
            // 
            this.btnCheck.Location = new System.Drawing.Point(7, 21);
            this.btnCheck.Name = "btnCheck";
            this.btnCheck.Size = new System.Drawing.Size(219, 31);
            this.btnCheck.TabIndex = 3;
            this.btnCheck.Text = "前回ベーキング記録無効設定";
            this.btnCheck.UseVisualStyleBackColor = true;
            this.btnCheck.Click += new System.EventHandler(this.btnCheck_Click);
            // 
            // grdLimit
            // 
            this.grdLimit.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdLimit.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdLimit.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChk});
            this.grdLimit.Location = new System.Drawing.Point(7, 59);
            this.grdLimit.Name = "grdLimit";
            this.grdLimit.RowTemplate.Height = 21;
            this.grdLimit.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdLimit.Size = new System.Drawing.Size(772, 193);
            this.grdLimit.TabIndex = 2;
            this.grdLimit.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdLimit_CellContentClick);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.grdLimit);
            this.groupBox1.Controls.Add(this.btnCheck);
            this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox1.Location = new System.Drawing.Point(0, 0);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(786, 260);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "ベーキング時間監視";
            this.groupBox1.Enter += new System.EventHandler(this.groupBox1_Enter);
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.btnStopAlert);
            this.groupBox2.Controls.Add(this.grdDBLoader);
            this.groupBox2.Controls.Add(this.btnIgnoreDBLoader);
            this.groupBox2.Dock = System.Windows.Forms.DockStyle.Fill;
            this.groupBox2.Location = new System.Drawing.Point(0, 0);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(786, 255);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "DB投入時間監視";
            // 
            // btnStopAlert
            // 
            this.btnStopAlert.Location = new System.Drawing.Point(164, 21);
            this.btnStopAlert.Name = "btnStopAlert";
            this.btnStopAlert.Size = new System.Drawing.Size(152, 31);
            this.btnStopAlert.TabIndex = 4;
            this.btnStopAlert.Text = "警報無効";
            this.btnStopAlert.UseVisualStyleBackColor = true;
            this.btnStopAlert.Click += new System.EventHandler(this.btnStopAlert_Click);
            // 
            // grdDBLoader
            // 
            this.grdDBLoader.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDBLoader.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDBLoader.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colChk2});
            this.grdDBLoader.Location = new System.Drawing.Point(7, 59);
            this.grdDBLoader.Name = "grdDBLoader";
            this.grdDBLoader.RowTemplate.Height = 21;
            this.grdDBLoader.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDBLoader.Size = new System.Drawing.Size(772, 189);
            this.grdDBLoader.TabIndex = 2;
            // 
            // btnIgnoreDBLoader
            // 
            this.btnIgnoreDBLoader.Location = new System.Drawing.Point(7, 21);
            this.btnIgnoreDBLoader.Name = "btnIgnoreDBLoader";
            this.btnIgnoreDBLoader.Size = new System.Drawing.Size(150, 31);
            this.btnIgnoreDBLoader.TabIndex = 3;
            this.btnIgnoreDBLoader.Text = "レコード削除";
            this.btnIgnoreDBLoader.UseVisualStyleBackColor = true;
            this.btnIgnoreDBLoader.Click += new System.EventHandler(this.btnIgnoreDBLoader_Click);
            // 
            // timer1
            // 
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.Location = new System.Drawing.Point(14, 44);
            this.splitContainer1.Name = "splitContainer1";
            this.splitContainer1.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.groupBox1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.groupBox2);
            this.splitContainer1.Size = new System.Drawing.Size(786, 520);
            this.splitContainer1.SplitterDistance = 260;
            this.splitContainer1.SplitterWidth = 5;
            this.splitContainer1.TabIndex = 6;
            // 
            // colChk2
            // 
            this.colChk2.Frozen = true;
            this.colChk2.HeaderText = "選択";
            this.colChk2.Name = "colChk2";
            this.colChk2.Width = 50;
            // 
            // colChk
            // 
            this.colChk.Frozen = true;
            this.colChk.HeaderText = "選択";
            this.colChk.Name = "colChk";
            this.colChk.Width = 50;
            // 
            // btnReload
            // 
            this.btnReload.FlatAppearance.BorderColor = System.Drawing.SystemColors.ControlDark;
            this.btnReload.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnReload.Image = global::ArmsMaintenance.Properties.Resources.recur;
            this.btnReload.Location = new System.Drawing.Point(14, 8);
            this.btnReload.Name = "btnReload";
            this.btnReload.Size = new System.Drawing.Size(137, 30);
            this.btnReload.TabIndex = 7;
            this.btnReload.Text = "最新の情報に更新";
            this.btnReload.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnReload.UseVisualStyleBackColor = true;
            this.btnReload.Click += new System.EventHandler(this.btnReload_Click);
            // 
            // FrmPreDBMonitor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(814, 578);
            this.Controls.Add(this.btnReload);
            this.Controls.Add(this.splitContainer1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmPreDBMonitor";
            this.Text = "MAP　DB前ベーキング時間監視";
            this.Load += new System.EventHandler(this.FrmPreDBMonitor_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmPreDBMonitor_FormClosing);
            ((System.ComponentModel.ISupportInitialize)(this.grdLimit)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdDBLoader)).EndInit();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button btnCheck;
        private System.Windows.Forms.DataGridView grdLimit;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.DataGridView grdDBLoader;
        private System.Windows.Forms.Button btnIgnoreDBLoader;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Button btnStopAlert;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChk;
        private System.Windows.Forms.DataGridViewCheckBoxColumn colChk2;
        private System.Windows.Forms.Button btnReload;
    }
}