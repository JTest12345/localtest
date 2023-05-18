
namespace LMResultView
{
    partial class MarkM
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
            this.magnotext = new System.Windows.Forms.TextBox();
            this.SearchM = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.materialcombo = new System.Windows.Forms.ComboBox();
            this.dataGridViewM = new System.Windows.Forms.DataGridView();
            this.aoino = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.magno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.typecd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialcd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.workstdt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.workenddt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewM)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 20);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(61, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "マガジンNo";
            // 
            // magnotext
            // 
            this.magnotext.Location = new System.Drawing.Point(127, 17);
            this.magnotext.Name = "magnotext";
            this.magnotext.Size = new System.Drawing.Size(136, 23);
            this.magnotext.TabIndex = 1;
            // 
            // SearchM
            // 
            this.SearchM.Location = new System.Drawing.Point(288, 17);
            this.SearchM.Name = "SearchM";
            this.SearchM.Size = new System.Drawing.Size(77, 23);
            this.SearchM.TabIndex = 2;
            this.SearchM.Text = "検索";
            this.SearchM.UseVisualStyleBackColor = true;
            this.SearchM.Click += new System.EventHandler(this.SearchM_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 57);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "部材コード";
            // 
            // materialcombo
            // 
            this.materialcombo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.materialcombo.FormattingEnabled = true;
            this.materialcombo.Location = new System.Drawing.Point(127, 54);
            this.materialcombo.Name = "materialcombo";
            this.materialcombo.Size = new System.Drawing.Size(136, 23);
            this.materialcombo.TabIndex = 4;
            // 
            // dataGridViewM
            // 
            this.dataGridViewM.AllowUserToAddRows = false;
            this.dataGridViewM.AllowUserToDeleteRows = false;
            this.dataGridViewM.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewM.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.aoino,
            this.magno,
            this.typecd,
            this.materialcd,
            this.workstdt,
            this.workenddt});
            this.dataGridViewM.Location = new System.Drawing.Point(30, 108);
            this.dataGridViewM.Name = "dataGridViewM";
            this.dataGridViewM.ReadOnly = true;
            this.dataGridViewM.RowTemplate.Height = 25;
            this.dataGridViewM.Size = new System.Drawing.Size(641, 247);
            this.dataGridViewM.TabIndex = 5;
            // 
            // aoino
            // 
            this.aoino.HeaderText = "aoino";
            this.aoino.Name = "aoino";
            this.aoino.ReadOnly = true;
            this.aoino.Width = 40;
            // 
            // magno
            // 
            this.magno.HeaderText = "magno";
            this.magno.Name = "magno";
            this.magno.ReadOnly = true;
            this.magno.Width = 60;
            // 
            // typecd
            // 
            this.typecd.HeaderText = "typecd";
            this.typecd.Name = "typecd";
            this.typecd.ReadOnly = true;
            // 
            // materialcd
            // 
            this.materialcd.HeaderText = "materialcd";
            this.materialcd.Name = "materialcd";
            this.materialcd.ReadOnly = true;
            // 
            // workstdt
            // 
            this.workstdt.HeaderText = "workstdt";
            this.workstdt.Name = "workstdt";
            this.workstdt.ReadOnly = true;
            this.workstdt.Width = 140;
            // 
            // workenddt
            // 
            this.workenddt.HeaderText = "workenddt";
            this.workenddt.Name = "workenddt";
            this.workenddt.ReadOnly = true;
            this.workenddt.Width = 140;
            // 
            // MarkM
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(705, 389);
            this.Controls.Add(this.dataGridViewM);
            this.Controls.Add(this.materialcombo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.SearchM);
            this.Controls.Add(this.magnotext);
            this.Controls.Add(this.label1);
            this.Name = "MarkM";
            this.Text = "MarkM";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewM)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox magnotext;
        private System.Windows.Forms.Button SearchM;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox materialcombo;
        private System.Windows.Forms.DataGridView dataGridViewM;
        private System.Windows.Forms.DataGridViewTextBoxColumn aoino;
        private System.Windows.Forms.DataGridViewTextBoxColumn magno;
        private System.Windows.Forms.DataGridViewTextBoxColumn typecd;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialcd;
        private System.Windows.Forms.DataGridViewTextBoxColumn workstdt;
        private System.Windows.Forms.DataGridViewTextBoxColumn workenddt;
    }
}