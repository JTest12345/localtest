namespace GEICS
{
    partial class frmDrawGraphCatalog
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
			this.sbChartWidth = new System.Windows.Forms.HScrollBar();
			this.sbChartHeight = new System.Windows.Forms.HScrollBar();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.btnViewGraph = new System.Windows.Forms.Button();
			this.tbChartWidth = new System.Windows.Forms.TextBox();
			this.tbChartHeight = new System.Windows.Forms.TextBox();
			this.btnOutputXls = new System.Windows.Forms.Button();
			this.chkbResultLogOnly = new System.Windows.Forms.CheckBox();
			this.tbUpLim = new System.Windows.Forms.TextBox();
			this.tbLowLim = new System.Windows.Forms.TextBox();
			this.chkbUpLim = new System.Windows.Forms.CheckBox();
			this.chkbLowLim = new System.Windows.Forms.CheckBox();
			this.SuspendLayout();
			// 
			// sbChartWidth
			// 
			this.sbChartWidth.Location = new System.Drawing.Point(60, 9);
			this.sbChartWidth.Name = "sbChartWidth";
			this.sbChartWidth.Size = new System.Drawing.Size(173, 12);
			this.sbChartWidth.TabIndex = 0;
			this.sbChartWidth.ValueChanged += new System.EventHandler(this.sbChartWidth_ValueChanged);
			// 
			// sbChartHeight
			// 
			this.sbChartHeight.Location = new System.Drawing.Point(365, 9);
			this.sbChartHeight.Name = "sbChartHeight";
			this.sbChartHeight.Size = new System.Drawing.Size(173, 12);
			this.sbChartHeight.TabIndex = 1;
			this.sbChartHeight.ValueChanged += new System.EventHandler(this.sbChartHeight_ValueChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(42, 12);
			this.label1.TabIndex = 2;
			this.label1.Text = "グラフ幅";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Location = new System.Drawing.Point(312, 9);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(50, 12);
			this.label2.TabIndex = 3;
			this.label2.Text = "グラフ高さ";
			// 
			// btnViewGraph
			// 
			this.btnViewGraph.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnViewGraph.Location = new System.Drawing.Point(651, 6);
			this.btnViewGraph.Name = "btnViewGraph";
			this.btnViewGraph.Size = new System.Drawing.Size(167, 41);
			this.btnViewGraph.TabIndex = 4;
			this.btnViewGraph.Text = "グラフ描画";
			this.btnViewGraph.UseVisualStyleBackColor = true;
			this.btnViewGraph.Click += new System.EventHandler(this.btnViewGraph_Click);
			// 
			// tbChartWidth
			// 
			this.tbChartWidth.Location = new System.Drawing.Point(236, 6);
			this.tbChartWidth.Name = "tbChartWidth";
			this.tbChartWidth.ReadOnly = true;
			this.tbChartWidth.Size = new System.Drawing.Size(70, 19);
			this.tbChartWidth.TabIndex = 5;
			// 
			// tbChartHeight
			// 
			this.tbChartHeight.Location = new System.Drawing.Point(541, 6);
			this.tbChartHeight.Name = "tbChartHeight";
			this.tbChartHeight.ReadOnly = true;
			this.tbChartHeight.Size = new System.Drawing.Size(73, 19);
			this.tbChartHeight.TabIndex = 6;
			// 
			// btnOutputXls
			// 
			this.btnOutputXls.Anchor = System.Windows.Forms.AnchorStyles.Top;
			this.btnOutputXls.Location = new System.Drawing.Point(824, 6);
			this.btnOutputXls.Name = "btnOutputXls";
			this.btnOutputXls.Size = new System.Drawing.Size(167, 41);
			this.btnOutputXls.TabIndex = 7;
			this.btnOutputXls.Text = "Excel出力";
			this.btnOutputXls.UseVisualStyleBackColor = true;
			this.btnOutputXls.Click += new System.EventHandler(this.btnOutputXls_Click);
			// 
			// chkbResultLogOnly
			// 
			this.chkbResultLogOnly.AutoSize = true;
			this.chkbResultLogOnly.Location = new System.Drawing.Point(14, 31);
			this.chkbResultLogOnly.Name = "chkbResultLogOnly";
			this.chkbResultLogOnly.Size = new System.Drawing.Size(121, 16);
			this.chkbResultLogOnly.TabIndex = 8;
			this.chkbResultLogOnly.Text = "出来栄えデータ限定";
			this.chkbResultLogOnly.UseVisualStyleBackColor = true;
			// 
			// tbUpLim
			// 
			this.tbUpLim.Location = new System.Drawing.Point(297, 28);
			this.tbUpLim.Name = "tbUpLim";
			this.tbUpLim.Size = new System.Drawing.Size(100, 19);
			this.tbUpLim.TabIndex = 9;
			this.tbUpLim.Text = "999999";
			this.tbUpLim.Visible = false;
			// 
			// tbLowLim
			// 
			this.tbLowLim.Location = new System.Drawing.Point(545, 28);
			this.tbLowLim.Name = "tbLowLim";
			this.tbLowLim.Size = new System.Drawing.Size(100, 19);
			this.tbLowLim.TabIndex = 11;
			this.tbLowLim.Text = "-999999";
			this.tbLowLim.Visible = false;
			// 
			// chkbUpLim
			// 
			this.chkbUpLim.AutoSize = true;
			this.chkbUpLim.Location = new System.Drawing.Point(156, 30);
			this.chkbUpLim.Name = "chkbUpLim";
			this.chkbUpLim.Size = new System.Drawing.Size(135, 16);
			this.chkbUpLim.TabIndex = 12;
			this.chkbUpLim.Text = "除外する規格上限(>=)";
			this.chkbUpLim.UseVisualStyleBackColor = true;
			this.chkbUpLim.Visible = false;
			// 
			// chkbLowLim
			// 
			this.chkbLowLim.AutoSize = true;
			this.chkbLowLim.Location = new System.Drawing.Point(404, 30);
			this.chkbLowLim.Name = "chkbLowLim";
			this.chkbLowLim.Size = new System.Drawing.Size(135, 16);
			this.chkbLowLim.TabIndex = 13;
			this.chkbLowLim.Text = "除外する規格下限(<=)";
			this.chkbLowLim.UseVisualStyleBackColor = true;
			this.chkbLowLim.Visible = false;
			// 
			// frmDrawGraphCatalog
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(1016, 1009);
			this.Controls.Add(this.chkbLowLim);
			this.Controls.Add(this.chkbUpLim);
			this.Controls.Add(this.tbLowLim);
			this.Controls.Add(this.tbUpLim);
			this.Controls.Add(this.chkbResultLogOnly);
			this.Controls.Add(this.btnOutputXls);
			this.Controls.Add(this.tbChartHeight);
			this.Controls.Add(this.tbChartWidth);
			this.Controls.Add(this.btnViewGraph);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.sbChartHeight);
			this.Controls.Add(this.sbChartWidth);
			this.Name = "frmDrawGraphCatalog";
			this.Text = "frmDrawGraphCatalog";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Load += new System.EventHandler(this.frmDrawGraphCatalog_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDrawGraphCatalog_FormClosed);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.HScrollBar sbChartWidth;
		private System.Windows.Forms.HScrollBar sbChartHeight;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button btnViewGraph;
		private System.Windows.Forms.TextBox tbChartWidth;
		private System.Windows.Forms.TextBox tbChartHeight;
		private System.Windows.Forms.Button btnOutputXls;
		private System.Windows.Forms.CheckBox chkbResultLogOnly;
		private System.Windows.Forms.TextBox tbUpLim;
		private System.Windows.Forms.TextBox tbLowLim;
		private System.Windows.Forms.CheckBox chkbUpLim;
		private System.Windows.Forms.CheckBox chkbLowLim;
    }
}