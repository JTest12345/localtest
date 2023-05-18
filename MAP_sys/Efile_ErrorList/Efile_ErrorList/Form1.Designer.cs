
namespace Efile_ErrorList
{
    partial class Form1
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
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
            this.Efile_date = new System.Windows.Forms.DateTimePicker();
            this.Efile_View = new System.Windows.Forms.DataGridView();
            this.lot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.type = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.workname = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machineno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enddt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.endtime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lot_exist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.get_efile = new System.Windows.Forms.Button();
            this.existtext = new System.Windows.Forms.TextBox();
            this.nonexisttext = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.Efile_View)).BeginInit();
            this.SuspendLayout();
            // 
            // Efile_date
            // 
            this.Efile_date.Location = new System.Drawing.Point(52, 56);
            this.Efile_date.Name = "Efile_date";
            this.Efile_date.Size = new System.Drawing.Size(200, 19);
            this.Efile_date.TabIndex = 0;
            // 
            // Efile_View
            // 
            this.Efile_View.AllowUserToAddRows = false;
            this.Efile_View.AllowUserToDeleteRows = false;
            this.Efile_View.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.Efile_View.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lot,
            this.type,
            this.workname,
            this.machineno,
            this.enddt,
            this.endtime,
            this.lot_exist});
            this.Efile_View.Location = new System.Drawing.Point(52, 111);
            this.Efile_View.Name = "Efile_View";
            this.Efile_View.ReadOnly = true;
            this.Efile_View.RowTemplate.Height = 21;
            this.Efile_View.Size = new System.Drawing.Size(812, 293);
            this.Efile_View.TabIndex = 1;
            // 
            // lot
            // 
            this.lot.HeaderText = "ロット";
            this.lot.Name = "lot";
            this.lot.ReadOnly = true;
            // 
            // type
            // 
            this.type.FillWeight = 150F;
            this.type.HeaderText = "機種";
            this.type.Name = "type";
            this.type.ReadOnly = true;
            this.type.Width = 150;
            // 
            // workname
            // 
            this.workname.HeaderText = "ZDorLED";
            this.workname.Name = "workname";
            this.workname.ReadOnly = true;
            // 
            // machineno
            // 
            this.machineno.HeaderText = "号機";
            this.machineno.Name = "machineno";
            this.machineno.ReadOnly = true;
            // 
            // enddt
            // 
            this.enddt.HeaderText = "完了日";
            this.enddt.Name = "enddt";
            this.enddt.ReadOnly = true;
            // 
            // endtime
            // 
            this.endtime.HeaderText = "完了時間";
            this.endtime.Name = "endtime";
            this.endtime.ReadOnly = true;
            // 
            // lot_exist
            // 
            this.lot_exist.HeaderText = "Eファイル有無";
            this.lot_exist.Name = "lot_exist";
            this.lot_exist.ReadOnly = true;
            // 
            // get_efile
            // 
            this.get_efile.Location = new System.Drawing.Point(275, 56);
            this.get_efile.Name = "get_efile";
            this.get_efile.Size = new System.Drawing.Size(96, 34);
            this.get_efile.TabIndex = 2;
            this.get_efile.Text = "Eファイル確認";
            this.get_efile.UseVisualStyleBackColor = true;
            this.get_efile.Click += new System.EventHandler(this.get_efile_Click);
            // 
            // existtext
            // 
            this.existtext.Location = new System.Drawing.Point(746, 36);
            this.existtext.Name = "existtext";
            this.existtext.ReadOnly = true;
            this.existtext.Size = new System.Drawing.Size(101, 19);
            this.existtext.TabIndex = 3;
            // 
            // nonexisttext
            // 
            this.nonexisttext.Location = new System.Drawing.Point(747, 71);
            this.nonexisttext.Name = "nonexisttext";
            this.nonexisttext.ReadOnly = true;
            this.nonexisttext.Size = new System.Drawing.Size(100, 19);
            this.nonexisttext.TabIndex = 4;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(673, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 12);
            this.label1.TabIndex = 5;
            this.label1.Text = "Eファイル有";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(673, 74);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "Eファイル無";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(50, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(402, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "指定日の生産実績とEファイルコピー先フォルダを比較してファイルの有無を検索します";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(995, 450);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.nonexisttext);
            this.Controls.Add(this.existtext);
            this.Controls.Add(this.get_efile);
            this.Controls.Add(this.Efile_View);
            this.Controls.Add(this.Efile_date);
            this.Name = "Form1";
            this.Text = "Eファイル有無確認";
            ((System.ComponentModel.ISupportInitialize)(this.Efile_View)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DateTimePicker Efile_date;
        private System.Windows.Forms.DataGridView Efile_View;
        private System.Windows.Forms.Button get_efile;
        private System.Windows.Forms.DataGridViewTextBoxColumn lot;
        private System.Windows.Forms.DataGridViewTextBoxColumn type;
        private System.Windows.Forms.DataGridViewTextBoxColumn workname;
        private System.Windows.Forms.DataGridViewTextBoxColumn machineno;
        private System.Windows.Forms.DataGridViewTextBoxColumn enddt;
        private System.Windows.Forms.DataGridViewTextBoxColumn endtime;
        private System.Windows.Forms.DataGridViewTextBoxColumn lot_exist;
        private System.Windows.Forms.TextBox existtext;
        private System.Windows.Forms.TextBox nonexisttext;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
    }
}

