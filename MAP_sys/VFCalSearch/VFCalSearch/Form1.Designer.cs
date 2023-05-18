
namespace VFCalSearch
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
            this.VfCal_NGList = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.STPath = new System.Windows.Forms.TextBox();
            this.VfCal_Check = new System.Windows.Forms.Button();
            this.GetParamOpen = new System.Windows.Forms.Button();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.label3 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // VfCal_NGList
            // 
            this.VfCal_NGList.Location = new System.Drawing.Point(25, 127);
            this.VfCal_NGList.MaxLength = 327670;
            this.VfCal_NGList.Multiline = true;
            this.VfCal_NGList.Name = "VfCal_NGList";
            this.VfCal_NGList.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.VfCal_NGList.Size = new System.Drawing.Size(478, 180);
            this.VfCal_NGList.TabIndex = 0;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(23, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(98, 12);
            this.label1.TabIndex = 1;
            this.label1.Text = "Vf Cal未登録リスト";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(23, 28);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(89, 12);
            this.label2.TabIndex = 2;
            this.label2.Text = "選別規格保存先";
            // 
            // STPath
            // 
            this.STPath.Location = new System.Drawing.Point(25, 59);
            this.STPath.Name = "STPath";
            this.STPath.Size = new System.Drawing.Size(478, 19);
            this.STPath.TabIndex = 3;
            // 
            // VfCal_Check
            // 
            this.VfCal_Check.Location = new System.Drawing.Point(134, 22);
            this.VfCal_Check.Name = "VfCal_Check";
            this.VfCal_Check.Size = new System.Drawing.Size(77, 25);
            this.VfCal_Check.TabIndex = 4;
            this.VfCal_Check.Text = "VfCalチェック";
            this.VfCal_Check.UseVisualStyleBackColor = true;
            this.VfCal_Check.Click += new System.EventHandler(this.VfCal_Check_Click);
            // 
            // GetParamOpen
            // 
            this.GetParamOpen.Location = new System.Drawing.Point(386, 22);
            this.GetParamOpen.Name = "GetParamOpen";
            this.GetParamOpen.Size = new System.Drawing.Size(89, 25);
            this.GetParamOpen.TabIndex = 5;
            this.GetParamOpen.Text = "パラメータ抽出";
            this.GetParamOpen.UseVisualStyleBackColor = true;
            this.GetParamOpen.Click += new System.EventHandler(this.GetParamOpen_Click);
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(82, 313);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(186, 21);
            this.progressBar1.TabIndex = 6;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(23, 319);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(53, 12);
            this.label3.TabIndex = 7;
            this.label3.Text = "進行状況";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(515, 352);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.GetParamOpen);
            this.Controls.Add(this.VfCal_Check);
            this.Controls.Add(this.STPath);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.VfCal_NGList);
            this.Name = "Form1";
            this.Text = "選別規格チェック";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox VfCal_NGList;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox STPath;
        private System.Windows.Forms.Button VfCal_Check;
        private System.Windows.Forms.Button GetParamOpen;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.Label label3;
    }
}

