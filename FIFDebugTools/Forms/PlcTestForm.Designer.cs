namespace FIFDebugTools
{
    partial class PlcTestForm
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
            this.label2 = new System.Windows.Forms.Label();
            this.txtMno = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.btQueryIpPt = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.txtIpaddress = new System.Windows.Forms.TextBox();
            this.cmbCmd = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.btComTest = new System.Windows.Forms.Button();
            this.btSendCmd = new System.Windows.Forms.Button();
            this.consoleBox_plc = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.cmbPcat = new System.Windows.Forms.ComboBox();
            this.txtDeviceNo = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.txtCmdData = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtDevType = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(45, 49);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(62, 18);
            this.label2.TabIndex = 13;
            this.label2.Text = "設備NO：";
            // 
            // txtMno
            // 
            this.txtMno.Enabled = false;
            this.txtMno.Location = new System.Drawing.Point(105, 46);
            this.txtMno.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtMno.Name = "txtMno";
            this.txtMno.Size = new System.Drawing.Size(108, 25);
            this.txtMno.TabIndex = 1;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 16);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(67, 18);
            this.label1.TabIndex = 11;
            this.label1.Text = "工程CAT：";
            // 
            // btQueryIpPt
            // 
            this.btQueryIpPt.Enabled = false;
            this.btQueryIpPt.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btQueryIpPt.Location = new System.Drawing.Point(282, 44);
            this.btQueryIpPt.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btQueryIpPt.Name = "btQueryIpPt";
            this.btQueryIpPt.Size = new System.Drawing.Size(164, 27);
            this.btQueryIpPt.TabIndex = 2;
            this.btQueryIpPt.Text = "PLCの情報をDB問い合わせ";
            this.btQueryIpPt.UseVisualStyleBackColor = true;
            this.btQueryIpPt.Click += new System.EventHandler(this.btQueryIpPt_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(27, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(80, 18);
            this.label3.TabIndex = 13;
            this.label3.Text = "IPアドレス：";
            // 
            // txtIpaddress
            // 
            this.txtIpaddress.Location = new System.Drawing.Point(105, 81);
            this.txtIpaddress.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtIpaddress.Name = "txtIpaddress";
            this.txtIpaddress.Size = new System.Drawing.Size(175, 25);
            this.txtIpaddress.TabIndex = 3;
            // 
            // cmbCmd
            // 
            this.cmbCmd.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCmd.FormattingEnabled = true;
            this.cmbCmd.Items.AddRange(new object[] {
            "デバイスデータ読み込み[RD]",
            "デバイスデータ読み込み[WD]"});
            this.cmbCmd.Location = new System.Drawing.Point(105, 155);
            this.cmbCmd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbCmd.Name = "cmbCmd";
            this.cmbCmd.Size = new System.Drawing.Size(341, 26);
            this.cmbCmd.TabIndex = 6;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(51, 119);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(56, 18);
            this.label4.TabIndex = 13;
            this.label4.Text = "ポート：";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(105, 116);
            this.txtPort.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(108, 25);
            this.txtPort.TabIndex = 4;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(15, 158);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(92, 18);
            this.label5.TabIndex = 13;
            this.label5.Text = "コマンド種別：";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(15, 197);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(92, 18);
            this.label6.TabIndex = 13;
            this.label6.Text = "デバイス種別：";
            // 
            // btComTest
            // 
            this.btComTest.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btComTest.Location = new System.Drawing.Point(282, 116);
            this.btComTest.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btComTest.Name = "btComTest";
            this.btComTest.Size = new System.Drawing.Size(164, 25);
            this.btComTest.TabIndex = 5;
            this.btComTest.Text = "Open/Close接続テスト";
            this.btComTest.UseVisualStyleBackColor = true;
            this.btComTest.Click += new System.EventHandler(this.btComTest_Click);
            // 
            // btSendCmd
            // 
            this.btSendCmd.Font = new System.Drawing.Font("メイリオ", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.btSendCmd.Location = new System.Drawing.Point(282, 231);
            this.btSendCmd.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btSendCmd.Name = "btSendCmd";
            this.btSendCmd.Size = new System.Drawing.Size(164, 25);
            this.btSendCmd.TabIndex = 10;
            this.btSendCmd.Text = "コマンド送信";
            this.btSendCmd.UseVisualStyleBackColor = true;
            this.btSendCmd.Click += new System.EventHandler(this.btSendCmd_Click);
            // 
            // consoleBox_plc
            // 
            this.consoleBox_plc.Location = new System.Drawing.Point(18, 288);
            this.consoleBox_plc.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.consoleBox_plc.Multiline = true;
            this.consoleBox_plc.Name = "consoleBox_plc";
            this.consoleBox_plc.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.consoleBox_plc.Size = new System.Drawing.Size(429, 83);
            this.consoleBox_plc.TabIndex = 16;
            this.consoleBox_plc.WordWrap = false;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(16, 269);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(80, 18);
            this.label7.TabIndex = 13;
            this.label7.Text = "コンソール：";
            // 
            // cmbPcat
            // 
            this.cmbPcat.Enabled = false;
            this.cmbPcat.FormattingEnabled = true;
            this.cmbPcat.Items.AddRange(new object[] {
            "DB",
            "WB"});
            this.cmbPcat.Location = new System.Drawing.Point(105, 10);
            this.cmbPcat.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.cmbPcat.Name = "cmbPcat";
            this.cmbPcat.Size = new System.Drawing.Size(108, 26);
            this.cmbPcat.TabIndex = 0;
            this.cmbPcat.Tag = "";
            // 
            // txtDeviceNo
            // 
            this.txtDeviceNo.Location = new System.Drawing.Point(320, 194);
            this.txtDeviceNo.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDeviceNo.Name = "txtDeviceNo";
            this.txtDeviceNo.Size = new System.Drawing.Size(108, 25);
            this.txtDeviceNo.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(229, 197);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(92, 18);
            this.label8.TabIndex = 13;
            this.label8.Text = "デバイス番号：";
            // 
            // txtCmdData
            // 
            this.txtCmdData.Location = new System.Drawing.Point(105, 231);
            this.txtCmdData.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtCmdData.Name = "txtCmdData";
            this.txtCmdData.Size = new System.Drawing.Size(108, 25);
            this.txtCmdData.TabIndex = 9;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(51, 234);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(56, 18);
            this.label9.TabIndex = 13;
            this.label9.Text = "データ：";
            // 
            // txtDevType
            // 
            this.txtDevType.Location = new System.Drawing.Point(105, 194);
            this.txtDevType.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.txtDevType.Name = "txtDevType";
            this.txtDevType.Size = new System.Drawing.Size(108, 25);
            this.txtDevType.TabIndex = 17;
            // 
            // PlcTestForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(464, 381);
            this.Controls.Add(this.txtDevType);
            this.Controls.Add(this.cmbPcat);
            this.Controls.Add(this.cmbCmd);
            this.Controls.Add(this.txtCmdData);
            this.Controls.Add(this.txtDeviceNo);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.txtIpaddress);
            this.Controls.Add(this.txtMno);
            this.Controls.Add(this.consoleBox_plc);
            this.Controls.Add(this.btSendCmd);
            this.Controls.Add(this.btComTest);
            this.Controls.Add(this.btQueryIpPt);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("メイリオ", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.MaximumSize = new System.Drawing.Size(480, 420);
            this.MinimumSize = new System.Drawing.Size(480, 420);
            this.Name = "PlcTestForm";
            this.Text = "PLC接続テスト";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox txtMno;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button btQueryIpPt;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtIpaddress;
        private System.Windows.Forms.ComboBox cmbCmd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btComTest;
        private System.Windows.Forms.Button btSendCmd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox cmbPcat;
        private System.Windows.Forms.TextBox txtDeviceNo;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtCmdData;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtDevType;
        private System.Windows.Forms.TextBox consoleBox_plc;
    }
}