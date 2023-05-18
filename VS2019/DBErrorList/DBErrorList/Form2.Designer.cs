
namespace DBErrorList
{
    partial class Form2
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
            this.label1 = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.Alert = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.macno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.plantcd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Mntdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.delfg = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.GetListBtn = new System.Windows.Forms.Button();
            this.DBUpdate = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(70, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "設備(macno)";
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(394, 305);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(87, 22);
            this.button1.TabIndex = 1;
            this.button1.Text = "UPD,INS";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Visible = false;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(102, 28);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(100, 19);
            this.textBox1.TabIndex = 2;
            // 
            // Alert
            // 
            this.Alert.Location = new System.Drawing.Point(383, 25);
            this.Alert.Name = "Alert";
            this.Alert.Size = new System.Drawing.Size(87, 27);
            this.Alert.TabIndex = 3;
            this.Alert.Text = "期限管理";
            this.Alert.UseVisualStyleBackColor = true;
            this.Alert.Click += new System.EventHandler(this.Alert_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.macno,
            this.plantcd,
            this.Mntdate,
            this.delfg});
            this.dataGridView1.Location = new System.Drawing.Point(28, 73);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(384, 226);
            this.dataGridView1.TabIndex = 4;
            this.dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView1_CellContentClick_1);
            // 
            // macno
            // 
            this.macno.HeaderText = "macno";
            this.macno.Name = "macno";
            this.macno.Width = 80;
            // 
            // plantcd
            // 
            this.plantcd.HeaderText = "plantcd";
            this.plantcd.Name = "plantcd";
            this.plantcd.Width = 80;
            // 
            // Mntdate
            // 
            this.Mntdate.HeaderText = "Mntdate";
            this.Mntdate.Name = "Mntdate";
            this.Mntdate.Width = 130;
            // 
            // delfg
            // 
            this.delfg.HeaderText = "delfg";
            this.delfg.Name = "delfg";
            this.delfg.Width = 50;
            // 
            // GetListBtn
            // 
            this.GetListBtn.Location = new System.Drawing.Point(208, 25);
            this.GetListBtn.Name = "GetListBtn";
            this.GetListBtn.Size = new System.Drawing.Size(88, 24);
            this.GetListBtn.TabIndex = 5;
            this.GetListBtn.Text = "設備一覧取得";
            this.GetListBtn.UseVisualStyleBackColor = true;
            this.GetListBtn.Click += new System.EventHandler(this.GetListBtn_Click);
            // 
            // DBUpdate
            // 
            this.DBUpdate.Location = new System.Drawing.Point(302, 26);
            this.DBUpdate.Name = "DBUpdate";
            this.DBUpdate.Size = new System.Drawing.Size(75, 23);
            this.DBUpdate.TabIndex = 6;
            this.DBUpdate.Text = "DB更新";
            this.DBUpdate.UseVisualStyleBackColor = true;
            this.DBUpdate.Click += new System.EventHandler(this.DBUpdate_Click);
            // 
            // Form2
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(493, 331);
            this.Controls.Add(this.DBUpdate);
            this.Controls.Add(this.GetListBtn);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.Alert);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.label1);
            this.Name = "Form2";
            this.Text = "設備メンテナンス時期一覧";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button Alert;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button GetListBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn macno;
        private System.Windows.Forms.DataGridViewTextBoxColumn plantcd;
        private System.Windows.Forms.DataGridViewTextBoxColumn Mntdate;
        private System.Windows.Forms.DataGridViewTextBoxColumn delfg;
        private System.Windows.Forms.Button DBUpdate;
    }
}