
namespace DBErrorList
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
            this.MacMnt = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.MFI = new System.Windows.Forms.Button();
            this.MacFailRead = new System.Windows.Forms.Button();
            this.DBErrorList = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // MacMnt
            // 
            this.MacMnt.Location = new System.Drawing.Point(257, 161);
            this.MacMnt.Name = "MacMnt";
            this.MacMnt.Size = new System.Drawing.Size(122, 28);
            this.MacMnt.TabIndex = 2;
            this.MacMnt.Text = "設備メンテ管理";
            this.MacMnt.UseVisualStyleBackColor = true;
            this.MacMnt.Click += new System.EventHandler(this.MacMnt_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(33, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "DBエラー管理システム";
            // 
            // MFI
            // 
            this.MFI.Location = new System.Drawing.Point(257, 53);
            this.MFI.Name = "MFI";
            this.MFI.Size = new System.Drawing.Size(122, 30);
            this.MFI.TabIndex = 4;
            this.MFI.Text = "設備故障入力";
            this.MFI.UseVisualStyleBackColor = true;
            this.MFI.Click += new System.EventHandler(this.MFI_Click);
            // 
            // MacFailRead
            // 
            this.MacFailRead.Location = new System.Drawing.Point(257, 104);
            this.MacFailRead.Name = "MacFailRead";
            this.MacFailRead.Size = new System.Drawing.Size(122, 30);
            this.MacFailRead.TabIndex = 5;
            this.MacFailRead.Text = "設備故障_履歴閲覧";
            this.MacFailRead.UseVisualStyleBackColor = true;
            this.MacFailRead.Click += new System.EventHandler(this.MacFailRead_Click);
            // 
            // DBErrorList
            // 
            this.DBErrorList.Location = new System.Drawing.Point(129, 104);
            this.DBErrorList.Name = "DBErrorList";
            this.DBErrorList.Size = new System.Drawing.Size(86, 26);
            this.DBErrorList.TabIndex = 6;
            this.DBErrorList.Text = "DBエラーリスト";
            this.DBErrorList.UseVisualStyleBackColor = true;
            this.DBErrorList.Click += new System.EventHandler(this.DBErrorList_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(427, 236);
            this.Controls.Add(this.DBErrorList);
            this.Controls.Add(this.MacFailRead);
            this.Controls.Add(this.MFI);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MacMnt);
            this.Name = "Form1";
            this.Text = "MMS_Main";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button MacMnt;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button MFI;
        private System.Windows.Forms.Button MacFailRead;
        private System.Windows.Forms.Button DBErrorList;
    }
}

