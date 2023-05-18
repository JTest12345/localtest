namespace GEICS
{
	partial class F08_ParameterReport
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F08_ParameterReport));
            this.clbServer = new System.Windows.Forms.CheckedListBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btnSelect = new System.Windows.Forms.Button();
            this.btnCansel = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tbPlantCD = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.tbParamNO = new System.Windows.Forms.TextBox();
            this.btnOutputReport = new System.Windows.Forms.Button();
            this.dtsTargetDT = new SLCommonLib.UCDateTimeSelector();
            this.label4 = new System.Windows.Forms.Label();
            this.btnPlantMultiSelect = new System.Windows.Forms.Button();
            this.imageList = new System.Windows.Forms.ImageList(this.components);
            this.btnParamMultiSelect = new System.Windows.Forms.Button();
            this.chkSingleMagazineData = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // clbServer
            // 
            this.clbServer.FormattingEnabled = true;
            this.clbServer.Location = new System.Drawing.Point(2, 30);
            this.clbServer.Name = "clbServer";
            this.clbServer.Size = new System.Drawing.Size(236, 172);
            this.clbServer.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(10, 14);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(59, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "対象サーバ";
            // 
            // btnSelect
            // 
            this.btnSelect.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSelect.Image = ((System.Drawing.Image)(resources.GetObject("btnSelect.Image")));
            this.btnSelect.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSelect.Location = new System.Drawing.Point(2, 208);
            this.btnSelect.Name = "btnSelect";
            this.btnSelect.Size = new System.Drawing.Size(115, 34);
            this.btnSelect.TabIndex = 2;
            this.btnSelect.Text = "全選択";
            this.btnSelect.UseVisualStyleBackColor = true;
            this.btnSelect.Click += new System.EventHandler(this.btnSelect_Click);
            // 
            // btnCansel
            // 
            this.btnCansel.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnCansel.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCansel.Image = ((System.Drawing.Image)(resources.GetObject("btnCansel.Image")));
            this.btnCansel.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnCansel.Location = new System.Drawing.Point(123, 208);
            this.btnCansel.Name = "btnCansel";
            this.btnCansel.Size = new System.Drawing.Size(115, 34);
            this.btnCansel.TabIndex = 3;
            this.btnCansel.Text = "全解除";
            this.btnCansel.UseVisualStyleBackColor = true;
            this.btnCansel.Click += new System.EventHandler(this.btnCansel_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(253, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "設備";
            // 
            // tbPlantCD
            // 
            this.tbPlantCD.Enabled = false;
            this.tbPlantCD.Location = new System.Drawing.Point(316, 30);
            this.tbPlantCD.Name = "tbPlantCD";
            this.tbPlantCD.Size = new System.Drawing.Size(153, 19);
            this.tbPlantCD.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(253, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(49, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "パラメータ";
            // 
            // tbParamNO
            // 
            this.tbParamNO.Enabled = false;
            this.tbParamNO.Location = new System.Drawing.Point(316, 55);
            this.tbParamNO.Name = "tbParamNO";
            this.tbParamNO.Size = new System.Drawing.Size(153, 19);
            this.tbParamNO.TabIndex = 7;
            // 
            // btnOutputReport
            // 
            this.btnOutputReport.Image = ((System.Drawing.Image)(resources.GetObject("btnOutputReport.Image")));
            this.btnOutputReport.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnOutputReport.Location = new System.Drawing.Point(571, 208);
            this.btnOutputReport.Name = "btnOutputReport";
            this.btnOutputReport.Size = new System.Drawing.Size(120, 34);
            this.btnOutputReport.TabIndex = 2;
            this.btnOutputReport.Text = "レポート出力";
            this.btnOutputReport.UseVisualStyleBackColor = true;
            this.btnOutputReport.Click += new System.EventHandler(this.btnOutputReport_Click);
            // 
            // dtsTargetDT
            // 
            this.dtsTargetDT.Checked = true;
            this.dtsTargetDT.Conjuction = SLCommonLib.DateConjuction.to;
            this.dtsTargetDT.Cursor = System.Windows.Forms.Cursors.Default;
            this.dtsTargetDT.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtsTargetDT.EndDate = "2015/02/25 23:59:59";
            this.dtsTargetDT.Location = new System.Drawing.Point(313, 80);
            this.dtsTargetDT.MaxDateTime = new System.DateTime(9990, 12, 31, 23, 59, 59, 0);
            this.dtsTargetDT.MinDateTime = new System.DateTime(1900, 1, 1, 0, 0, 0, 0);
            this.dtsTargetDT.Name = "dtsTargetDT";
            this.dtsTargetDT.Size = new System.Drawing.Size(378, 23);
            this.dtsTargetDT.StartDate = "2015/02/25 00:00:00";
            this.dtsTargetDT.TabIndex = 8;
            this.dtsTargetDT.Leave += new System.EventHandler(this.dtsTargetDT_Leave);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(253, 85);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "対象期間";
            // 
            // btnPlantMultiSelect
            // 
            this.btnPlantMultiSelect.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnPlantMultiSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnPlantMultiSelect.ImageIndex = 0;
            this.btnPlantMultiSelect.ImageList = this.imageList;
            this.btnPlantMultiSelect.Location = new System.Drawing.Point(472, 30);
            this.btnPlantMultiSelect.Name = "btnPlantMultiSelect";
            this.btnPlantMultiSelect.Size = new System.Drawing.Size(22, 19);
            this.btnPlantMultiSelect.TabIndex = 10;
            this.btnPlantMultiSelect.UseVisualStyleBackColor = true;
            this.btnPlantMultiSelect.Click += new System.EventHandler(this.btnPlantMultiSelect_Click);
            // 
            // imageList
            // 
            this.imageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList.ImageStream")));
            this.imageList.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList.Images.SetKeyName(0, "ACT_MULTISELECT_EP.png");
            this.imageList.Images.SetKeyName(1, "ACT_MULTISELECT_IN.png");
            // 
            // btnParamMultiSelect
            // 
            this.btnParamMultiSelect.FlatAppearance.BorderColor = System.Drawing.Color.Gray;
            this.btnParamMultiSelect.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnParamMultiSelect.ImageIndex = 0;
            this.btnParamMultiSelect.ImageList = this.imageList;
            this.btnParamMultiSelect.Location = new System.Drawing.Point(472, 55);
            this.btnParamMultiSelect.Name = "btnParamMultiSelect";
            this.btnParamMultiSelect.Size = new System.Drawing.Size(22, 19);
            this.btnParamMultiSelect.TabIndex = 10;
            this.btnParamMultiSelect.UseVisualStyleBackColor = true;
            this.btnParamMultiSelect.Click += new System.EventHandler(this.btnParamMultiSelect_Click);
            // 
            // chkSingleMagazineData
            // 
            this.chkSingleMagazineData.AutoSize = true;
            this.chkSingleMagazineData.Location = new System.Drawing.Point(18, 25);
            this.chkSingleMagazineData.Name = "chkSingleMagazineData";
            this.chkSingleMagazineData.Size = new System.Drawing.Size(231, 16);
            this.chkSingleMagazineData.TabIndex = 11;
            this.chkSingleMagazineData.Text = "分割マガジンは先頭マガジンデータのみ表示";
            this.chkSingleMagazineData.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.chkSingleMagazineData);
            this.groupBox1.Location = new System.Drawing.Point(255, 110);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(428, 81);
            this.groupBox1.TabIndex = 12;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "オプション";
            // 
            // F08_ParameterReport
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(695, 245);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnParamMultiSelect);
            this.Controls.Add(this.btnPlantMultiSelect);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtsTargetDT);
            this.Controls.Add(this.tbParamNO);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.tbPlantCD);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnCansel);
            this.Controls.Add(this.btnOutputReport);
            this.Controls.Add(this.btnSelect);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.clbServer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.Name = "F08_ParameterReport";
            this.Text = "F08_ParameterReport";
            this.Load += new System.EventHandler(this.F08_ParameterReport_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.CheckedListBox clbServer;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button btnSelect;
		private System.Windows.Forms.Button btnCansel;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.TextBox tbPlantCD;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.TextBox tbParamNO;
		private System.Windows.Forms.Button btnOutputReport;
		private SLCommonLib.UCDateTimeSelector dtsTargetDT;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.Button btnPlantMultiSelect;
		private System.Windows.Forms.Button btnParamMultiSelect;
		private System.Windows.Forms.ImageList imageList;
        private System.Windows.Forms.CheckBox chkSingleMagazineData;
        private System.Windows.Forms.GroupBox groupBox1;
	}
}