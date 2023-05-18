namespace Arms3PCD
{
    partial class FrmMachineSelect
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
            this.menuSave = new System.Windows.Forms.MenuItem();
            this.menuCancel = new System.Windows.Forms.MenuItem();
            this.txtUrl = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.lstMachine = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader3 = new System.Windows.Forms.ColumnHeader();
            this.SuspendLayout();
            // 
            // mainMenu1
            // 
            this.mainMenu1.MenuItems.Add(this.menuSave);
            this.mainMenu1.MenuItems.Add(this.menuCancel);
            // 
            // menuSave
            // 
            this.menuSave.Text = "保存";
            this.menuSave.Click += new System.EventHandler(this.menuSave_Click);
            // 
            // menuCancel
            // 
            this.menuCancel.Text = "キャンセル";
            this.menuCancel.Click += new System.EventHandler(this.menuCancel_Click);
            // 
            // txtUrl
            // 
            this.txtUrl.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                        | System.Windows.Forms.AnchorStyles.Right)));
            this.txtUrl.Location = new System.Drawing.Point(3, 54);
            this.txtUrl.Name = "txtUrl";
            this.txtUrl.Size = new System.Drawing.Size(232, 23);
            this.txtUrl.TabIndex = 1;
            this.txtUrl.Visible = false;
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular);
            this.label1.Location = new System.Drawing.Point(3, 31);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(153, 20);
            this.label1.Text = "中間サーバーUrl";
            this.label1.Visible = false;
            // 
            // lstMachine
            // 
            this.lstMachine.CheckBoxes = true;
            this.lstMachine.Columns.Add(this.columnHeader1);
            this.lstMachine.Columns.Add(this.columnHeader2);
            this.lstMachine.Columns.Add(this.columnHeader3);
            this.lstMachine.FullRowSelect = true;
            this.lstMachine.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.Nonclickable;
            this.lstMachine.Location = new System.Drawing.Point(3, 31);
            this.lstMachine.Name = "lstMachine";
            this.lstMachine.Size = new System.Drawing.Size(232, 261);
            this.lstMachine.TabIndex = 5;
            this.lstMachine.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "No";
            this.columnHeader1.Width = 30;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "設備名";
            this.columnHeader2.Width = 130;
            // 
            // columnHeader3
            // 
            this.columnHeader3.Text = "号機";
            this.columnHeader3.Width = 68;
            // 
            // FrmMachineSelect
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(238, 295);
            this.Controls.Add(this.lstMachine);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtUrl);
            this.Menu = this.mainMenu1;
            this.Name = "FrmMachineSelect";
            this.Text = "装置選択";
            this.Load += new System.EventHandler(this.FrmMachineSelect_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TextBox txtUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.MenuItem menuSave;
        private System.Windows.Forms.MenuItem menuCancel;
        private System.Windows.Forms.ListView lstMachine;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.ColumnHeader columnHeader3;
    }
}