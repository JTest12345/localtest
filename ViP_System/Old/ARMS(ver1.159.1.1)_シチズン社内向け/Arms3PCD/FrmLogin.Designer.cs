namespace Arms3PCD
{
    partial class FrmLogin
    {
        /// <summary>
        /// 必要なデザイナ変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        private System.Windows.Forms.MainMenu mainMenu1;

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

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            this.mainMenu1 = new System.Windows.Forms.MainMenu();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.txtEmpCd = new System.Windows.Forms.TextBox();
            this.btnGo = new System.Windows.Forms.Button();
            this.btnEnd = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Tahoma", 16F, System.Drawing.FontStyle.Bold);
            this.label1.Location = new System.Drawing.Point(3, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(232, 32);
            this.label1.Text = "ARMS3 Client";
            this.label1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.label2.Location = new System.Drawing.Point(3, 81);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(224, 20);
            this.label2.Text = "作業者コードを読み込んでください。";
            // 
            // txtEmpCd
            // 
            this.txtEmpCd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtEmpCd.Location = new System.Drawing.Point(3, 104);
            this.txtEmpCd.Name = "txtEmpCd";
            this.txtEmpCd.Size = new System.Drawing.Size(184, 23);
            this.txtEmpCd.TabIndex = 2;
            // 
            // btnGo
            // 
            this.btnGo.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.btnGo.Location = new System.Drawing.Point(189, 104);
            this.btnGo.Name = "btnGo";
            this.btnGo.Size = new System.Drawing.Size(46, 23);
            this.btnGo.TabIndex = 3;
            this.btnGo.Text = "GO";
            this.btnGo.Click += new System.EventHandler(this.btnGo_Click);
            // 
            // btnEnd
            // 
            this.btnEnd.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.btnEnd.Location = new System.Drawing.Point(70, 179);
            this.btnEnd.Name = "btnEnd";
            this.btnEnd.Size = new System.Drawing.Size(95, 23);
            this.btnEnd.TabIndex = 4;
            this.btnEnd.Text = "END";
            this.btnEnd.Click += new System.EventHandler(this.btnEnd_Click);
            // 
            // FrmLogin
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 295);
            this.Controls.Add(this.btnEnd);
            this.Controls.Add(this.btnGo);
            this.Controls.Add(this.txtEmpCd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.KeyPreview = true;
            this.Menu = this.mainMenu1;
            this.Name = "FrmLogin";
            this.Text = "ARMS3";
            this.Load += new System.EventHandler(this.FrmLogin_Load);
            this.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.FrmLogin_KeyPress);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtEmpCd;
        private System.Windows.Forms.Button btnGo;
        private System.Windows.Forms.Button btnEnd;
    }
}

