namespace ProcMasterIF
{
    partial class FmProcMasterIF
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
            this.txt_shinkishutenkai = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txt_buhinhyou = new System.Windows.Forms.TextBox();
            this.txt_typecd_kansei_start = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_typecd_kansei_end = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.consoleBox = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.toolStripStatusLabel1 = new System.Windows.Forms.ToolStripStatusLabel();
            this.txt_procjson_hankan = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txt_procjson_kansei = new System.Windows.Forms.TextBox();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            this.statusStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txt_shinkishutenkai
            // 
            this.txt_shinkishutenkai.Location = new System.Drawing.Point(15, 28);
            this.txt_shinkishutenkai.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_shinkishutenkai.Name = "txt_shinkishutenkai";
            this.txt_shinkishutenkai.Size = new System.Drawing.Size(758, 25);
            this.txt_shinkishutenkai.TabIndex = 0;
            this.txt_shinkishutenkai.Text = "C:\\Users\\jn-wtnb\\Desktop\\procmaster\\211210@Ver.9 新機種展開 仕様一覧【3版】.xlsm";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 6);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(80, 18);
            this.label1.TabIndex = 2;
            this.label1.Text = "新機種展開表";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 185);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 18);
            this.label2.TabIndex = 5;
            this.label2.Text = "部品表フォルダ";
            // 
            // txt_buhinhyou
            // 
            this.txt_buhinhyou.Location = new System.Drawing.Point(15, 204);
            this.txt_buhinhyou.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_buhinhyou.Name = "txt_buhinhyou";
            this.txt_buhinhyou.Size = new System.Drawing.Size(758, 25);
            this.txt_buhinhyou.TabIndex = 3;
            this.txt_buhinhyou.Text = "C:\\Users\\jn-wtnb\\Desktop\\procmaster\\システム化部品表";
            // 
            // txt_typecd_kansei_start
            // 
            this.txt_typecd_kansei_start.Location = new System.Drawing.Point(134, 89);
            this.txt_typecd_kansei_start.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_typecd_kansei_start.Name = "txt_typecd_kansei_start";
            this.txt_typecd_kansei_start.Size = new System.Drawing.Size(462, 25);
            this.txt_typecd_kansei_start.TabIndex = 0;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 68);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(102, 18);
            this.label4.TabIndex = 2;
            this.label4.Text = "製品行指定(開始)";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Location = new System.Drawing.Point(15, 89);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(104, 25);
            this.numericUpDown1.TabIndex = 6;
            this.numericUpDown1.ValueChanged += new System.EventHandler(this.numericUpDown1_ValueChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(131, 68);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(66, 18);
            this.label5.TabIndex = 2;
            this.label5.Text = "製品(開始)";
            // 
            // txt_typecd_kansei_end
            // 
            this.txt_typecd_kansei_end.Location = new System.Drawing.Point(134, 147);
            this.txt_typecd_kansei_end.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_typecd_kansei_end.Name = "txt_typecd_kansei_end";
            this.txt_typecd_kansei_end.Size = new System.Drawing.Size(462, 25);
            this.txt_typecd_kansei_end.TabIndex = 0;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(12, 126);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(102, 18);
            this.label6.TabIndex = 2;
            this.label6.Text = "製品行指定(終了)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(131, 126);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(66, 18);
            this.label7.TabIndex = 2;
            this.label7.Text = "製品(終了)";
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Location = new System.Drawing.Point(15, 147);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(104, 25);
            this.numericUpDown2.TabIndex = 6;
            this.numericUpDown2.ValueChanged += new System.EventHandler(this.numericUpDown2_ValueChanged);
            // 
            // consoleBox
            // 
            this.consoleBox.Location = new System.Drawing.Point(15, 337);
            this.consoleBox.Multiline = true;
            this.consoleBox.Name = "consoleBox";
            this.consoleBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.consoleBox.Size = new System.Drawing.Size(758, 244);
            this.consoleBox.TabIndex = 8;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(619, 89);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(144, 83);
            this.button3.TabIndex = 9;
            this.button3.Text = "Make Process Json";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripStatusLabel1});
            this.statusStrip1.Location = new System.Drawing.Point(0, 589);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(784, 22);
            this.statusStrip1.TabIndex = 10;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // toolStripStatusLabel1
            // 
            this.toolStripStatusLabel1.Name = "toolStripStatusLabel1";
            this.toolStripStatusLabel1.Size = new System.Drawing.Size(118, 17);
            this.toolStripStatusLabel1.Text = "toolStripStatusLabel1";
            // 
            // txt_procjson_hankan
            // 
            this.txt_procjson_hankan.Location = new System.Drawing.Point(89, 262);
            this.txt_procjson_hankan.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_procjson_hankan.Name = "txt_procjson_hankan";
            this.txt_procjson_hankan.Size = new System.Drawing.Size(683, 25);
            this.txt_procjson_hankan.TabIndex = 3;
            this.txt_procjson_hankan.Text = "C:\\Users\\jn-wtnb\\Desktop\\procmaster\\json";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(11, 243);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(119, 18);
            this.label10.TabIndex = 5;
            this.label10.Text = "PROCJSONフォルダ";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(27, 265);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(56, 18);
            this.label11.TabIndex = 11;
            this.label11.Text = "半完成品";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(27, 297);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(44, 18);
            this.label12.TabIndex = 11;
            this.label12.Text = "完成品";
            // 
            // txt_procjson_kansei
            // 
            this.txt_procjson_kansei.Location = new System.Drawing.Point(89, 294);
            this.txt_procjson_kansei.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txt_procjson_kansei.Name = "txt_procjson_kansei";
            this.txt_procjson_kansei.Size = new System.Drawing.Size(683, 25);
            this.txt_procjson_kansei.TabIndex = 3;
            this.txt_procjson_kansei.Text = "C:\\Users\\jn-wtnb\\Desktop\\procmaster\\json";
            // 
            // FmProcMasterIF
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 611);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.consoleBox);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txt_procjson_kansei);
            this.Controls.Add(this.txt_procjson_hankan);
            this.Controls.Add(this.txt_buhinhyou);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txt_typecd_kansei_end);
            this.Controls.Add(this.txt_typecd_kansei_start);
            this.Controls.Add(this.txt_shinkishutenkai);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(800, 650);
            this.MinimumSize = new System.Drawing.Size(800, 650);
            this.Name = "FmProcMasterIF";
            this.Text = "PMIF";
            this.Shown += new System.EventHandler(this.FmProcMasterIF_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txt_shinkishutenkai;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txt_buhinhyou;
        private System.Windows.Forms.TextBox txt_typecd_kansei_start;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_typecd_kansei_end;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.TextBox consoleBox;
        public System.Windows.Forms.Button button3;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private System.Windows.Forms.ToolStripStatusLabel toolStripStatusLabel1;
        private System.Windows.Forms.TextBox txt_procjson_hankan;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txt_procjson_kansei;
    }
}

