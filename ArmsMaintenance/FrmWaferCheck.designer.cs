namespace ArmsMaintenance
{
    partial class FrmWaferCheck
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
            this.btnSearch = new System.Windows.Forms.Button();
            this.cmbMachineList = new System.Windows.Forms.ComboBox();
            this.dateTimePicker1 = new System.Windows.Forms.DateTimePicker();
            this.label1 = new System.Windows.Forms.Label();
            this.dateTimePicker2 = new System.Windows.Forms.DateTimePicker();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.label2 = new System.Windows.Forms.Label();
            this.rdoLotSearch = new System.Windows.Forms.RadioButton();
            this.rdoTimeSearch = new System.Windows.Forms.RadioButton();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // btnSearch
            // 
            this.btnSearch.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnSearch.Location = new System.Drawing.Point(733, 64);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(122, 27);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // cmbMachineList
            // 
            this.cmbMachineList.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbMachineList.Location = new System.Drawing.Point(132, 7);
            this.cmbMachineList.Name = "cmbMachineList";
            this.cmbMachineList.Size = new System.Drawing.Size(407, 22);
            this.cmbMachineList.TabIndex = 1;
            // 
            // dateTimePicker1
            // 
            this.dateTimePicker1.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dateTimePicker1.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker1.Location = new System.Drawing.Point(132, 66);
            this.dateTimePicker1.Name = "dateTimePicker1";
            this.dateTimePicker1.Size = new System.Drawing.Size(186, 22);
            this.dateTimePicker1.TabIndex = 2;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(327, 70);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(19, 14);
            this.label1.TabIndex = 3;
            this.label1.Text = "～";
            // 
            // dateTimePicker2
            // 
            this.dateTimePicker2.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dateTimePicker2.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePicker2.Location = new System.Drawing.Point(353, 66);
            this.dateTimePicker2.Name = "dateTimePicker2";
            this.dateTimePicker2.Size = new System.Drawing.Size(186, 22);
            this.dateTimePicker2.TabIndex = 4;
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
                        | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(14, 96);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(841, 559);
            this.dataGridView1.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(14, 10);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 14);
            this.label2.TabIndex = 6;
            this.label2.Text = "対象装置";
            // 
            // rdoLotSearch
            // 
            this.rdoLotSearch.AutoSize = true;
            this.rdoLotSearch.Location = new System.Drawing.Point(14, 38);
            this.rdoLotSearch.Name = "rdoLotSearch";
            this.rdoLotSearch.Size = new System.Drawing.Size(97, 18);
            this.rdoLotSearch.TabIndex = 8;
            this.rdoLotSearch.Text = "対象ロット検索";
            this.rdoLotSearch.UseVisualStyleBackColor = true;
            // 
            // rdoTimeSearch
            // 
            this.rdoTimeSearch.AutoSize = true;
            this.rdoTimeSearch.Checked = true;
            this.rdoTimeSearch.Location = new System.Drawing.Point(14, 68);
            this.rdoTimeSearch.Name = "rdoTimeSearch";
            this.rdoTimeSearch.Size = new System.Drawing.Size(97, 18);
            this.rdoTimeSearch.TabIndex = 9;
            this.rdoTimeSearch.TabStop = true;
            this.rdoTimeSearch.Text = "対象期間検索";
            this.rdoTimeSearch.UseVisualStyleBackColor = true;
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(132, 37);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(186, 22);
            this.txtLotNo.TabIndex = 10;
            this.txtLotNo.TextChanged += new System.EventHandler(this.txtLotNo_TextChanged);
            // 
            // FrmWaferCheck
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(869, 668);
            this.Controls.Add(this.txtLotNo);
            this.Controls.Add(this.rdoTimeSearch);
            this.Controls.Add(this.rdoLotSearch);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.dateTimePicker2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.dateTimePicker1);
            this.Controls.Add(this.cmbMachineList);
            this.Controls.Add(this.btnSearch);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FrmWaferCheck";
            this.Text = "FrmWaferCheck";
            this.Load += new System.EventHandler(this.FrmWaferCheck_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.ComboBox cmbMachineList;
        private System.Windows.Forms.DateTimePicker dateTimePicker1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DateTimePicker dateTimePicker2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.RadioButton rdoLotSearch;
        private System.Windows.Forms.RadioButton rdoTimeSearch;
        private System.Windows.Forms.TextBox txtLotNo;
    }
}