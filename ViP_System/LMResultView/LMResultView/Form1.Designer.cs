
namespace LMResultView
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.dataGridViewF = new System.Windows.Forms.DataGridView();
            this.aoi = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pcbno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.pcblotno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.markstr = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kibanlot = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.markF_read = new System.Windows.Forms.Button();
            this.magresult = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewF)).BeginInit();
            this.SuspendLayout();
            // 
            // dataGridViewF
            // 
            this.dataGridViewF.AllowUserToAddRows = false;
            this.dataGridViewF.AllowUserToDeleteRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewF.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridViewF.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewF.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.aoi,
            this.pcbno,
            this.pcblotno,
            this.markstr});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewF.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridViewF.Location = new System.Drawing.Point(31, 89);
            this.dataGridViewF.Name = "dataGridViewF";
            this.dataGridViewF.ReadOnly = true;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Yu Gothic UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewF.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridViewF.RowTemplate.Height = 25;
            this.dataGridViewF.Size = new System.Drawing.Size(465, 392);
            this.dataGridViewF.TabIndex = 0;
            // 
            // aoi
            // 
            this.aoi.HeaderText = "aoi";
            this.aoi.Name = "aoi";
            this.aoi.ReadOnly = true;
            this.aoi.Width = 70;
            // 
            // pcbno
            // 
            this.pcbno.HeaderText = "pcbno";
            this.pcbno.Name = "pcbno";
            this.pcbno.ReadOnly = true;
            this.pcbno.Width = 70;
            // 
            // pcblotno
            // 
            this.pcblotno.HeaderText = "pcblotno";
            this.pcblotno.Name = "pcblotno";
            this.pcblotno.ReadOnly = true;
            this.pcblotno.Width = 110;
            // 
            // markstr
            // 
            this.markstr.HeaderText = "markstr";
            this.markstr.Name = "markstr";
            this.markstr.ReadOnly = true;
            this.markstr.Width = 70;
            // 
            // kibanlot
            // 
            this.kibanlot.Location = new System.Drawing.Point(116, 32);
            this.kibanlot.Name = "kibanlot";
            this.kibanlot.Size = new System.Drawing.Size(197, 23);
            this.kibanlot.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(31, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(72, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "基板ロットNo";
            // 
            // markF_read
            // 
            this.markF_read.Location = new System.Drawing.Point(328, 32);
            this.markF_read.Name = "markF_read";
            this.markF_read.Size = new System.Drawing.Size(76, 22);
            this.markF_read.TabIndex = 3;
            this.markF_read.Text = "検索";
            this.markF_read.UseVisualStyleBackColor = true;
            this.markF_read.Click += new System.EventHandler(this.markF_read_Click);
            // 
            // magresult
            // 
            this.magresult.Location = new System.Drawing.Point(416, 32);
            this.magresult.Name = "magresult";
            this.magresult.Size = new System.Drawing.Size(80, 21);
            this.magresult.TabIndex = 4;
            this.magresult.Text = "実績確認";
            this.magresult.UseVisualStyleBackColor = true;
            this.magresult.Click += new System.EventHandler(this.magresult_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(555, 532);
            this.Controls.Add(this.magresult);
            this.Controls.Add(this.markF_read);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.kibanlot);
            this.Controls.Add(this.dataGridViewF);
            this.Name = "Form1";
            this.Text = "MarkF";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewF)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView dataGridViewF;
        private System.Windows.Forms.TextBox kibanlot;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button markF_read;
        private System.Windows.Forms.DataGridViewTextBoxColumn aoi;
        private System.Windows.Forms.DataGridViewTextBoxColumn pcbno;
        private System.Windows.Forms.DataGridViewTextBoxColumn pcblotno;
        private System.Windows.Forms.DataGridViewTextBoxColumn markstr;
        private System.Windows.Forms.Button magresult;
    }
}

