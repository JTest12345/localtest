
namespace DBErrorList
{
    partial class DBErrorList
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
            this.ErrorListRead = new System.Windows.Forms.Button();
            this.ErrorListView = new System.Windows.Forms.DataGridView();
            this.CK = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ymd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hms = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assenlot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.macno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.x = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.y = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.area = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kibanno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LorR = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.chipad = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filetype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.delfg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.macfilter = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.GetCheckBoxTF = new System.Windows.Forms.Button();
            this.OldDataCheckBox = new System.Windows.Forms.CheckBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label2 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.ErrorListView)).BeginInit();
            this.SuspendLayout();
            // 
            // ErrorListRead
            // 
            this.ErrorListRead.Location = new System.Drawing.Point(37, 24);
            this.ErrorListRead.Name = "ErrorListRead";
            this.ErrorListRead.Size = new System.Drawing.Size(110, 30);
            this.ErrorListRead.TabIndex = 0;
            this.ErrorListRead.Text = "データ読込";
            this.ErrorListRead.UseVisualStyleBackColor = true;
            this.ErrorListRead.Click += new System.EventHandler(this.ErrorListRead_Click);
            // 
            // ErrorListView
            // 
            this.ErrorListView.AllowUserToAddRows = false;
            this.ErrorListView.AllowUserToDeleteRows = false;
            this.ErrorListView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.ErrorListView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.CK,
            this.ymd,
            this.hms,
            this.assenlot,
            this.macno,
            this.x,
            this.y,
            this.area,
            this.kibanno,
            this.LorR,
            this.chipad,
            this.filetype,
            this.delfg});
            this.ErrorListView.Location = new System.Drawing.Point(37, 113);
            this.ErrorListView.Name = "ErrorListView";
            this.ErrorListView.RowTemplate.Height = 21;
            this.ErrorListView.Size = new System.Drawing.Size(851, 299);
            this.ErrorListView.TabIndex = 1;
            // 
            // CK
            // 
            this.CK.HeaderText = "CK";
            this.CK.Name = "CK";
            this.CK.Width = 30;
            // 
            // ymd
            // 
            this.ymd.HeaderText = "ymd";
            this.ymd.Name = "ymd";
            this.ymd.Width = 70;
            // 
            // hms
            // 
            this.hms.HeaderText = "hms";
            this.hms.Name = "hms";
            this.hms.Width = 50;
            // 
            // assenlot
            // 
            this.assenlot.HeaderText = "assenlot";
            this.assenlot.Name = "assenlot";
            // 
            // macno
            // 
            this.macno.HeaderText = "macno";
            this.macno.Name = "macno";
            this.macno.Width = 70;
            // 
            // x
            // 
            this.x.HeaderText = "x";
            this.x.Name = "x";
            this.x.Width = 80;
            // 
            // y
            // 
            this.y.HeaderText = "y";
            this.y.Name = "y";
            this.y.Width = 80;
            // 
            // area
            // 
            this.area.HeaderText = "area";
            this.area.Name = "area";
            this.area.Width = 80;
            // 
            // kibanno
            // 
            this.kibanno.HeaderText = "kibanno";
            this.kibanno.Name = "kibanno";
            this.kibanno.Width = 50;
            // 
            // LorR
            // 
            this.LorR.HeaderText = "LorR";
            this.LorR.Name = "LorR";
            this.LorR.Width = 50;
            // 
            // chipad
            // 
            this.chipad.HeaderText = "chipad";
            this.chipad.Name = "chipad";
            this.chipad.Width = 50;
            // 
            // filetype
            // 
            this.filetype.HeaderText = "filetype";
            this.filetype.Name = "filetype";
            this.filetype.Width = 50;
            // 
            // delfg
            // 
            this.delfg.HeaderText = "delfg";
            this.delfg.Name = "delfg";
            this.delfg.Width = 50;
            // 
            // macfilter
            // 
            this.macfilter.Location = new System.Drawing.Point(250, 30);
            this.macfilter.Name = "macfilter";
            this.macfilter.Size = new System.Drawing.Size(99, 19);
            this.macfilter.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(182, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(62, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "号機フィルタ";
            // 
            // GetCheckBoxTF
            // 
            this.GetCheckBoxTF.Location = new System.Drawing.Point(650, 28);
            this.GetCheckBoxTF.Name = "GetCheckBoxTF";
            this.GetCheckBoxTF.Size = new System.Drawing.Size(116, 23);
            this.GetCheckBoxTF.TabIndex = 4;
            this.GetCheckBoxTF.Text = "チェックデータ更新";
            this.GetCheckBoxTF.UseVisualStyleBackColor = true;
            this.GetCheckBoxTF.Click += new System.EventHandler(this.GetCheckBoxTF_Click);
            // 
            // OldDataCheckBox
            // 
            this.OldDataCheckBox.AutoSize = true;
            this.OldDataCheckBox.Location = new System.Drawing.Point(184, 71);
            this.OldDataCheckBox.Name = "OldDataCheckBox";
            this.OldDataCheckBox.Size = new System.Drawing.Size(98, 16);
            this.OldDataCheckBox.TabIndex = 5;
            this.OldDataCheckBox.Text = "過去データ含む";
            this.OldDataCheckBox.UseVisualStyleBackColor = true;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.Location = new System.Drawing.Point(471, 30);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(136, 19);
            this.dateTimePicker1.TabIndex = 6;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(373, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(85, 12);
            this.label2.TabIndex = 7;
            this.label2.Text = "日付指定(以降)";
            // 
            // DBErrorList
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(915, 444);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.OldDataCheckBox);
            this.Controls.Add(this.GetCheckBoxTF);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.macfilter);
            this.Controls.Add(this.ErrorListView);
            this.Controls.Add(this.ErrorListRead);
            this.Name = "DBErrorList";
            this.Text = "DBErrorList";
            ((System.ComponentModel.ISupportInitialize)(this.ErrorListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button ErrorListRead;
        private System.Windows.Forms.DataGridView ErrorListView;
        private System.Windows.Forms.TextBox macfilter;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button GetCheckBoxTF;
        private System.Windows.Forms.CheckBox OldDataCheckBox;
        private System.Windows.Forms.DataGridViewCheckBoxColumn CK;
        private System.Windows.Forms.DataGridViewTextBoxColumn ymd;
        private System.Windows.Forms.DataGridViewTextBoxColumn hms;
        private System.Windows.Forms.DataGridViewTextBoxColumn assenlot;
        private System.Windows.Forms.DataGridViewTextBoxColumn macno;
        private System.Windows.Forms.DataGridViewTextBoxColumn x;
        private System.Windows.Forms.DataGridViewTextBoxColumn y;
        private System.Windows.Forms.DataGridViewTextBoxColumn area;
        private System.Windows.Forms.DataGridViewTextBoxColumn kibanno;
        private System.Windows.Forms.DataGridViewTextBoxColumn LorR;
        private System.Windows.Forms.DataGridViewTextBoxColumn chipad;
        private System.Windows.Forms.DataGridViewTextBoxColumn filetype;
        private System.Windows.Forms.DataGridViewTextBoxColumn delfg;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label2;
    }
}