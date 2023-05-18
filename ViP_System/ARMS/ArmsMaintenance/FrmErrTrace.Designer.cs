namespace ArmsMaintenance
{
    partial class FrmErrTrace
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
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.grdItems = new System.Windows.Forms.DataGridView();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.txtBackSideMarking = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.txtSideMarking = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtSideMarking);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.txtBackSideMarking);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(646, 91);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // btnSearch
            // 
            this.btnSearch.Location = new System.Drawing.Point(532, 54);
            this.btnSearch.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(96, 27);
            this.btnSearch.TabIndex = 39;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // grdItems
            // 
            this.grdItems.AllowUserToAddRows = false;
            this.grdItems.AllowUserToDeleteRows = false;
            this.grdItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdItems.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.DisplayedCells;
            this.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdItems.Location = new System.Drawing.Point(12, 118);
            this.grdItems.Name = "grdItems";
            this.grdItems.ReadOnly = true;
            this.grdItems.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.grdItems.RowTemplate.Height = 21;
            this.grdItems.Size = new System.Drawing.Size(741, 321);
            this.grdItems.TabIndex = 38;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(72, 28);
            this.txtLotNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(159, 19);
            this.txtLotNo.TabIndex = 41;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(25, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(35, 12);
            this.label1.TabIndex = 40;
            this.label1.Text = "LotNo";
            // 
            // txtBackSideMarking
            // 
            this.txtBackSideMarking.Location = new System.Drawing.Point(337, 58);
            this.txtBackSideMarking.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBackSideMarking.Name = "txtBackSideMarking";
            this.txtBackSideMarking.Size = new System.Drawing.Size(159, 19);
            this.txtBackSideMarking.TabIndex = 43;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(255, 61);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(76, 12);
            this.label2.TabIndex = 42;
            this.label2.Text = "裏面マーキング";
            // 
            // txtSideMarking
            // 
            this.txtSideMarking.Location = new System.Drawing.Point(337, 28);
            this.txtSideMarking.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSideMarking.Name = "txtSideMarking";
            this.txtSideMarking.Size = new System.Drawing.Size(159, 19);
            this.txtSideMarking.TabIndex = 45;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(255, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(76, 12);
            this.label3.TabIndex = 44;
            this.label3.Text = "側面マーキング";
            // 
            // FrmErrTrace
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(765, 451);
            this.Controls.Add(this.grdItems);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmErrTrace";
            this.Text = "エラートレース";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.DataGridView grdItems;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtSideMarking;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBackSideMarking;
        private System.Windows.Forms.Label label2;
    }
}