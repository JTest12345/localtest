namespace FVDC
{
    partial class FVDC110
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースが破棄される場合 true、破棄されない場合は false です。</param>
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
            this.txtServer = new System.Windows.Forms.TextBox();
            this.lblServer = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtServer
            // 
            this.txtServer.AllowDrop = true;
            this.txtServer.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.txtServer.Location = new System.Drawing.Point(14, 13);
            this.txtServer.Name = "txtServer";
            this.txtServer.Size = new System.Drawing.Size(258, 23);
            this.txtServer.TabIndex = 0;
            this.txtServer.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblServer_DragDrop);
            this.txtServer.DragOver += new System.Windows.Forms.DragEventHandler(this.lblServer_DragOver);
            // 
            // lblServer
            // 
            this.lblServer.AllowDrop = true;
            this.lblServer.AutoSize = true;
            this.lblServer.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lblServer.ForeColor = System.Drawing.Color.Navy;
            this.lblServer.Location = new System.Drawing.Point(10, 51);
            this.lblServer.Name = "lblServer";
            this.lblServer.Size = new System.Drawing.Size(266, 48);
            this.lblServer.TabIndex = 1;
            this.lblServer.Text = "メインサーバー情報を入力して下さい。\r\nArmsConfig.xmlをドラッグ/ドロップ\r\nすると自動的に取得します。";
            this.lblServer.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblServer_DragDrop);
            this.lblServer.DragOver += new System.Windows.Forms.DragEventHandler(this.lblServer_DragOver);
            // 
            // btnUpdate
            // 
            this.btnUpdate.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnUpdate.Location = new System.Drawing.Point(83, 114);
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.Size = new System.Drawing.Size(87, 25);
            this.btnUpdate.TabIndex = 2;
            this.btnUpdate.Text = "更　新";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btnCancel.Location = new System.Drawing.Point(185, 114);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(87, 25);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = "キャンセル";
            this.btnCancel.UseVisualStyleBackColor = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // FVDC110
            // 
            this.AllowDrop = true;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 151);
            this.ControlBox = false;
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.lblServer);
            this.Controls.Add(this.txtServer);
            this.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MaximumSize = new System.Drawing.Size(300, 190);
            this.MinimumSize = new System.Drawing.Size(300, 190);
            this.Name = "FVDC110";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "FVDC110　メインサーバーIP情報";
            this.Load += new System.EventHandler(this.FVDC110_Load);
            this.DragDrop += new System.Windows.Forms.DragEventHandler(this.lblServer_DragDrop);
            this.DragOver += new System.Windows.Forms.DragEventHandler(this.lblServer_DragOver);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtServer;
        private System.Windows.Forms.Label lblServer;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnCancel;
    }
}