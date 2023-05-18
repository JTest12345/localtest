
namespace ST_DataView
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
            this.label1 = new System.Windows.Forms.Label();
            this.ST_Result = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.lotnum = new System.Windows.Forms.TextBox();
            this.typecd = new System.Windows.Forms.TextBox();
            this.STkikaku = new System.Windows.Forms.TextBox();
            this.numkisei = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.podnum = new System.Windows.Forms.TextBox();
            this.GetSTdata = new System.Windows.Forms.Button();
            this.Upper_checkBox = new System.Windows.Forms.CheckBox();
            this.Lower_checkBox = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(42, 131);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(24, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "BIN";
            // 
            // ST_Result
            // 
            this.ST_Result.Location = new System.Drawing.Point(44, 213);
            this.ST_Result.Multiline = true;
            this.ST_Result.Name = "ST_Result";
            this.ST_Result.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.ST_Result.Size = new System.Drawing.Size(439, 210);
            this.ST_Result.TabIndex = 2;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(42, 27);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(29, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "ロット";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(42, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(29, 12);
            this.label3.TabIndex = 4;
            this.label3.Text = "型番";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(42, 96);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(53, 12);
            this.label4.TabIndex = 5;
            this.label4.Text = "選別規格";
            // 
            // lotnum
            // 
            this.lotnum.Location = new System.Drawing.Point(107, 27);
            this.lotnum.Name = "lotnum";
            this.lotnum.Size = new System.Drawing.Size(177, 19);
            this.lotnum.TabIndex = 6;
            // 
            // typecd
            // 
            this.typecd.Location = new System.Drawing.Point(107, 61);
            this.typecd.Name = "typecd";
            this.typecd.Size = new System.Drawing.Size(177, 19);
            this.typecd.TabIndex = 7;
            // 
            // STkikaku
            // 
            this.STkikaku.Location = new System.Drawing.Point(107, 93);
            this.STkikaku.Name = "STkikaku";
            this.STkikaku.Size = new System.Drawing.Size(177, 19);
            this.STkikaku.TabIndex = 8;
            // 
            // numkisei
            // 
            this.numkisei.Location = new System.Drawing.Point(107, 164);
            this.numkisei.Name = "numkisei";
            this.numkisei.Size = new System.Drawing.Size(70, 19);
            this.numkisei.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(42, 167);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(53, 12);
            this.label5.TabIndex = 10;
            this.label5.Text = "数量規制";
            // 
            // podnum
            // 
            this.podnum.Location = new System.Drawing.Point(107, 128);
            this.podnum.Name = "podnum";
            this.podnum.Size = new System.Drawing.Size(177, 19);
            this.podnum.TabIndex = 11;
            // 
            // GetSTdata
            // 
            this.GetSTdata.Location = new System.Drawing.Point(343, 27);
            this.GetSTdata.Name = "GetSTdata";
            this.GetSTdata.Size = new System.Drawing.Size(140, 31);
            this.GetSTdata.TabIndex = 12;
            this.GetSTdata.Text = "STデータ抽出";
            this.GetSTdata.UseVisualStyleBackColor = true;
            this.GetSTdata.Click += new System.EventHandler(this.GetSTdata_Click);
            // 
            // Upper_checkBox
            // 
            this.Upper_checkBox.AutoSize = true;
            this.Upper_checkBox.Location = new System.Drawing.Point(183, 166);
            this.Upper_checkBox.Name = "Upper_checkBox";
            this.Upper_checkBox.Size = new System.Drawing.Size(48, 16);
            this.Upper_checkBox.TabIndex = 13;
            this.Upper_checkBox.Text = "以上";
            this.Upper_checkBox.UseVisualStyleBackColor = true;
            // 
            // Lower_checkBox
            // 
            this.Lower_checkBox.AutoSize = true;
            this.Lower_checkBox.Location = new System.Drawing.Point(236, 166);
            this.Lower_checkBox.Name = "Lower_checkBox";
            this.Lower_checkBox.Size = new System.Drawing.Size(48, 16);
            this.Lower_checkBox.TabIndex = 14;
            this.Lower_checkBox.Text = "以下";
            this.Lower_checkBox.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(544, 450);
            this.Controls.Add(this.Lower_checkBox);
            this.Controls.Add(this.Upper_checkBox);
            this.Controls.Add(this.GetSTdata);
            this.Controls.Add(this.podnum);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.numkisei);
            this.Controls.Add(this.STkikaku);
            this.Controls.Add(this.typecd);
            this.Controls.Add(this.lotnum);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.ST_Result);
            this.Controls.Add(this.label1);
            this.Name = "Form1";
            this.Text = "ST結果閲覧";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox ST_Result;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox lotnum;
        private System.Windows.Forms.TextBox typecd;
        private System.Windows.Forms.TextBox STkikaku;
        private System.Windows.Forms.TextBox numkisei;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox podnum;
        private System.Windows.Forms.Button GetSTdata;
        private System.Windows.Forms.CheckBox Upper_checkBox;
        private System.Windows.Forms.CheckBox Lower_checkBox;
    }
}

