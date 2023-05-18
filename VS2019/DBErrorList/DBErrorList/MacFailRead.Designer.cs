
namespace DBErrorList
{
    partial class MacFailRead
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
            this.dataread = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.macno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.cause = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.worker = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.faildate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.inputdate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label1 = new System.Windows.Forms.Label();
            this.textBox1 = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataread
            // 
            this.dataread.Location = new System.Drawing.Point(559, 37);
            this.dataread.Name = "dataread";
            this.dataread.Size = new System.Drawing.Size(103, 31);
            this.dataread.TabIndex = 0;
            this.dataread.Text = "データ読込";
            this.dataread.UseVisualStyleBackColor = true;
            this.dataread.Click += new System.EventHandler(this.dataread_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.macno,
            this.cause,
            this.worker,
            this.faildate,
            this.inputdate});
            this.dataGridView1.Location = new System.Drawing.Point(28, 90);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(634, 270);
            this.dataGridView1.TabIndex = 1;
            // 
            // macno
            // 
            this.macno.HeaderText = "macno";
            this.macno.Name = "macno";
            this.macno.ReadOnly = true;
            this.macno.Width = 70;
            // 
            // cause
            // 
            this.cause.HeaderText = "cause";
            this.cause.Name = "cause";
            this.cause.ReadOnly = true;
            // 
            // worker
            // 
            this.worker.HeaderText = "worker";
            this.worker.Name = "worker";
            this.worker.ReadOnly = true;
            // 
            // faildate
            // 
            this.faildate.HeaderText = "faildate";
            this.faildate.Name = "faildate";
            this.faildate.ReadOnly = true;
            this.faildate.Width = 130;
            // 
            // inputdate
            // 
            this.inputdate.HeaderText = "inputdate";
            this.inputdate.Name = "inputdate";
            this.inputdate.ReadOnly = true;
            this.inputdate.Width = 130;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 46);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(64, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "設備絞込み";
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(108, 43);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(134, 19);
            this.textBox1.TabIndex = 3;
            // 
            // MacFailRead
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(726, 394);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.dataread);
            this.Name = "MacFailRead";
            this.Text = "MacFailRead";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button dataread;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.DataGridViewTextBoxColumn macno;
        private System.Windows.Forms.DataGridViewTextBoxColumn cause;
        private System.Windows.Forms.DataGridViewTextBoxColumn worker;
        private System.Windows.Forms.DataGridViewTextBoxColumn faildate;
        private System.Windows.Forms.DataGridViewTextBoxColumn inputdate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox textBox1;
    }
}