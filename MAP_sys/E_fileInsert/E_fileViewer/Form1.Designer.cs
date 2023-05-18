
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
            this.EFilePass = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.datestart = new System.Windows.Forms.DateTimePicker();
            this.label3 = new System.Windows.Forms.Label();
            this.dateend = new System.Windows.Forms.DateTimePicker();
            this.lot = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machineNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machinetype = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typecd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorcode1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorcount1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // GetEData_button
            // 
            this.GetEData_button.Location = new System.Drawing.Point(16, 107);
            this.GetEData_button.Name = "GetEData_button";
            this.GetEData_button.Size = new System.Drawing.Size(136, 33);
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
            this.errorcount1});
            this.dataGridView1.Location = new System.Drawing.Point(16, 146);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowHeadersWidth = 62;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(716, 268);
            this.dataGridView1.TabIndex = 1;
            // 
            // EFilePass
            // 
            this.EFilePass.Location = new System.Drawing.Point(106, 21);
            this.EFilePass.Margin = new System.Windows.Forms.Padding(2);
            this.EFilePass.Name = "EFilePass";
            this.EFilePass.Size = new System.Drawing.Size(375, 19);
            this.EFilePass.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(14, 23);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(89, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "Eファイル保存パス";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 58);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "期間を指定";
            // 
            // datestart
            // 
            this.datestart.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.datestart.Location = new System.Drawing.Point(106, 53);
            this.datestart.Name = "datestart";
            this.datestart.Size = new System.Drawing.Size(129, 19);
            this.datestart.TabIndex = 5;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(256, 58);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "～";
            // 
            // dateend
            // 
            this.dateend.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.dateend.Location = new System.Drawing.Point(289, 53);
            this.dateend.Name = "dateend";
            this.dateend.Size = new System.Drawing.Size(132, 19);
            this.dateend.TabIndex = 7;
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
            this.typecd.Name = "typecd";
            this.typecd.ReadOnly = true;
            this.typecd.Width = 150;
            // 
            // errorcode1
            // 
            this.errorcode1.HeaderText = "エラーコード1";
            this.errorcode1.MinimumWidth = 8;
            this.errorcode1.Name = "errorcode1";
            this.errorcode1.ReadOnly = true;
            // 
            // errorcount1
            // 
            this.errorcount1.HeaderText = "エラー数1";
            this.errorcount1.MinimumWidth = 8;
            this.errorcount1.Name = "errorcount1";
            this.errorcount1.ReadOnly = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(786, 450);
            this.Controls.Add(this.dateend);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.datestart);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.EFilePass);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.GetEData_button);
            this.Name = "Form1";
            this.Text = "Eファイル実績";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button GetEData_button;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.TextBox EFilePass;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DateTimePicker datestart;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.DateTimePicker dateend;
        private System.Windows.Forms.DataGridViewTextBoxColumn lot;
        private System.Windows.Forms.DataGridViewTextBoxColumn machineNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn machinetype;
        private System.Windows.Forms.DataGridViewTextBoxColumn typecd;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorcode1;
        private System.Windows.Forms.DataGridViewTextBoxColumn errorcount1;
    }
}

