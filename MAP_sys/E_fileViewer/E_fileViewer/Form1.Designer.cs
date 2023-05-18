
namespace E_fileViewer
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
            this.GetEData_button = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.lot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machinetype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typecd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorcode1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorcount1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastupddt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label2 = new System.Windows.Forms.Label();
            this.datestart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dateend = new System.Windows.Forms.DateTimePicker();
            this.lot_textbox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.MN_textbox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.zdled_textbox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.typecd_textbox = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.ecode_textbox = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // GetEData_button
            // 
            this.GetEData_button.Location = new System.Drawing.Point(27, 242);
            this.GetEData_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.GetEData_button.Name = "GetEData_button";
            this.GetEData_button.Size = new System.Drawing.Size(227, 50);
            this.GetEData_button.TabIndex = 0;
            this.GetEData_button.Text = "Eファイルデータ取得";
            this.GetEData_button.UseVisualStyleBackColor = true;
            this.GetEData_button.Click += new System.EventHandler(this.GetEData_button_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lot,
            this.machineNo,
            this.machinetype,
            this.typecd,
            this.errorcode1,
            this.errorcount1,
            this.lastupddt});
            this.dataGridView1.Location = new System.Drawing.Point(27, 300);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(1412, 466);
            this.dataGridView1.TabIndex = 1;
            // 
            // lot
            // 
            this.lot.HeaderText = "lot";
            this.lot.MinimumWidth = 8;
            this.lot.Name = "lot";
            this.lot.ReadOnly = true;
            this.lot.Width = 150;
            // 
            // machineNo
            // 
            this.machineNo.HeaderText = "号機";
            this.machineNo.MinimumWidth = 8;
            this.machineNo.Name = "machineNo";
            this.machineNo.ReadOnly = true;
            this.machineNo.Width = 70;
            // 
            // machinetype
            // 
            this.machinetype.HeaderText = "ZDorLED";
            this.machinetype.MinimumWidth = 8;
            this.machinetype.Name = "machinetype";
            this.machinetype.ReadOnly = true;
            this.machinetype.Width = 80;
            // 
            // typecd
            // 
            this.typecd.HeaderText = "typecd";
            this.typecd.MinimumWidth = 8;
            this.typecd.Name = "typecd";
            this.typecd.ReadOnly = true;
            this.typecd.Width = 150;
            // 
            // errorcode1
            // 
            this.errorcode1.HeaderText = "エラーコード";
            this.errorcode1.MinimumWidth = 8;
            this.errorcode1.Name = "errorcode1";
            this.errorcode1.ReadOnly = true;
            this.errorcode1.Width = 150;
            // 
            // errorcount1
            // 
            this.errorcount1.HeaderText = "エラー数";
            this.errorcount1.MinimumWidth = 8;
            this.errorcount1.Name = "errorcount1";
            this.errorcount1.ReadOnly = true;
            this.errorcount1.Width = 150;
            // 
            // lastupddt
            // 
            this.lastupddt.HeaderText = "更新日時";
            this.lastupddt.MinimumWidth = 8;
            this.lastupddt.Name = "lastupddt";
            this.lastupddt.ReadOnly = true;
            this.lastupddt.Width = 130;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(94, 18);
            this.label2.TabIndex = 4;
            this.label2.Text = "期間を指定";
            // 
            // datestart
            // 
            this.datestart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datestart.Location = new System.Drawing.Point(177, 24);
            this.datestart.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.datestart.Name = "datestart";
            this.datestart.Size = new System.Drawing.Size(212, 25);
            this.datestart.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(427, 32);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(26, 18);
            this.label3.TabIndex = 6;
            this.label3.Text = "～";
            // 
            // dateend
            // 
            this.dateend.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateend.Location = new System.Drawing.Point(482, 24);
            this.dateend.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.dateend.Name = "dateend";
            this.dateend.Size = new System.Drawing.Size(217, 25);
            this.dateend.TabIndex = 7;
            // 
            // lot_textbox
            // 
            this.lot_textbox.Location = new System.Drawing.Point(177, 76);
            this.lot_textbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lot_textbox.Multiline = true;
            this.lot_textbox.Name = "lot_textbox";
            this.lot_textbox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.lot_textbox.Size = new System.Drawing.Size(289, 139);
            this.lot_textbox.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(77, 87);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(45, 18);
            this.label4.TabIndex = 9;
            this.label4.Text = "lotno";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(583, 80);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(62, 18);
            this.label5.TabIndex = 10;
            this.label5.Text = "号機名";
            // 
            // MN_textbox
            // 
            this.MN_textbox.Location = new System.Drawing.Point(717, 77);
            this.MN_textbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MN_textbox.Name = "MN_textbox";
            this.MN_textbox.Size = new System.Drawing.Size(289, 25);
            this.MN_textbox.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(583, 134);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 18);
            this.label6.TabIndex = 12;
            this.label6.Text = "ZD or LED";
            // 
            // zdled_textbox
            // 
            this.zdled_textbox.Location = new System.Drawing.Point(717, 131);
            this.zdled_textbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.zdled_textbox.Name = "zdled_textbox";
            this.zdled_textbox.Size = new System.Drawing.Size(184, 25);
            this.zdled_textbox.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(583, 201);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(62, 18);
            this.label7.TabIndex = 14;
            this.label7.Text = "品種名";
            // 
            // typecd_textbox
            // 
            this.typecd_textbox.Location = new System.Drawing.Point(717, 190);
            this.typecd_textbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.typecd_textbox.Name = "typecd_textbox";
            this.typecd_textbox.Size = new System.Drawing.Size(374, 25);
            this.typecd_textbox.TabIndex = 15;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(583, 258);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 18);
            this.label8.TabIndex = 16;
            this.label8.Text = "エラーコード";
            // 
            // ecode_textbox
            // 
            this.ecode_textbox.Location = new System.Drawing.Point(717, 249);
            this.ecode_textbox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.ecode_textbox.Name = "ecode_textbox";
            this.ecode_textbox.Size = new System.Drawing.Size(372, 25);
            this.ecode_textbox.TabIndex = 17;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1488, 796);
            this.Controls.Add(this.ecode_textbox);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.typecd_textbox);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.zdled_textbox);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.MN_textbox);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.lot_textbox);
            this.Controls.Add(this.dateend);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.datestart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.GetEData_button);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "Form1";
            this.Text = "Eファイル実績";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetEData_button;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker datestart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateend;
        private System.Windows.Forms.TextBox lot_textbox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox MN_textbox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox zdled_textbox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox typecd_textbox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox ecode_textbox;
        private System.Windows.Forms.DataGridViewTextBoxColumn lot;
        private System.Windows.Forms.DataGridViewTextBoxColumn machineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn machinetype;
        private System.Windows.Forms.DataGridViewTextBoxColumn typecd;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorcode1;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorcount1;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastupddt;
    }
}

