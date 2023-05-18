
namespace DatabaseView
{
    partial class DBView
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
            this.DBname_combo = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.chooseDB = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tablename_combo = new System.Windows.Forms.ComboBox();
            this.colname_combo = new System.Windows.Forms.ComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.search_val = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // DBname_combo
            // 
            this.DBname_combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.DBname_combo.FormattingEnabled = true;
            this.DBname_combo.Location = new System.Drawing.Point(120, 24);
            this.DBname_combo.Name = "DBname_combo";
            this.DBname_combo.Size = new System.Drawing.Size(199, 20);
            this.DBname_combo.TabIndex = 0;
            this.DBname_combo.TextChanged += new System.EventHandler(this.DBname_combo_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 27);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(74, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "データベース名";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToOrderColumns = true;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(24, 94);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.ReadOnly = true;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(799, 415);
            this.dataGridView1.TabIndex = 2;
            // 
            // chooseDB
            // 
            this.chooseDB.Location = new System.Drawing.Point(358, 53);
            this.chooseDB.Name = "chooseDB";
            this.chooseDB.Size = new System.Drawing.Size(98, 19);
            this.chooseDB.TabIndex = 3;
            this.chooseDB.Text = "データベース抽出";
            this.chooseDB.UseVisualStyleBackColor = true;
            this.chooseDB.Click += new System.EventHandler(this.chooseDB_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(59, 60);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(55, 12);
            this.label2.TabIndex = 4;
            this.label2.Text = "テーブル名";
            // 
            // tablename_combo
            // 
            this.tablename_combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.tablename_combo.FormattingEnabled = true;
            this.tablename_combo.Location = new System.Drawing.Point(120, 53);
            this.tablename_combo.Name = "tablename_combo";
            this.tablename_combo.Size = new System.Drawing.Size(199, 20);
            this.tablename_combo.TabIndex = 5;
            this.tablename_combo.TextChanged += new System.EventHandler(this.tablename_combo_TextChanged);
            // 
            // colname_combo
            // 
            this.colname_combo.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.colname_combo.FormattingEnabled = true;
            this.colname_combo.Location = new System.Drawing.Point(595, 24);
            this.colname_combo.Name = "colname_combo";
            this.colname_combo.Size = new System.Drawing.Size(173, 20);
            this.colname_combo.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(473, 27);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(104, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "項目名(並べ替え用)";
            // 
            // search_val
            // 
            this.search_val.Location = new System.Drawing.Point(595, 54);
            this.search_val.Name = "search_val";
            this.search_val.Size = new System.Drawing.Size(173, 19);
            this.search_val.TabIndex = 8;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(473, 56);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(77, 12);
            this.label4.TabIndex = 9;
            this.label4.Text = "項目名検索値";
            // 
            // DBView
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(854, 542);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.search_val);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.colname_combo);
            this.Controls.Add(this.tablename_combo);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.chooseDB);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.DBname_combo);
            this.Name = "DBView";
            this.Text = "DatabaseView";
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox DBname_combo;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button chooseDB;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox tablename_combo;
        private System.Windows.Forms.ComboBox colname_combo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox search_val;
        private System.Windows.Forms.Label label4;
    }
}

