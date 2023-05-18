namespace LENS2
{
	partial class F91_MachineLogOutputCompletionMainte
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
            this.components = new System.ComponentModel.Container();
            this.label1 = new System.Windows.Forms.Label();
            this.tvMainteMachine = new System.Windows.Forms.TreeView();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbMainteAddr = new System.Windows.Forms.ComboBox();
            this.tbMainteAddr = new System.Windows.Forms.TextBox();
            this.tbWriteVal = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tbCurrentVal = new System.Windows.Forms.TextBox();
            this.btnGetVal = new System.Windows.Forms.Button();
            this.btnSendVal = new System.Windows.Forms.Button();
            this.cbWriteUnlock = new System.Windows.Forms.CheckBox();
            this.tmWriteLock = new System.Windows.Forms.Timer(this.components);
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 4;
            this.label1.Text = "設備一覧";
            // 
            // tvMainteMachine
            // 
            this.tvMainteMachine.Location = new System.Drawing.Point(12, 24);
            this.tvMainteMachine.Name = "tvMainteMachine";
            this.tvMainteMachine.Size = new System.Drawing.Size(209, 251);
            this.tvMainteMachine.TabIndex = 3;
            this.tvMainteMachine.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.tvMainteMachine_NodeMouseClick);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(227, 24);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(107, 12);
            this.label3.TabIndex = 6;
            this.label3.Text = "メンテナンスアドレス名";
            // 
            // cmbMainteAddr
            // 
            this.cmbMainteAddr.FormattingEnabled = true;
            this.cmbMainteAddr.Location = new System.Drawing.Point(229, 39);
            this.cmbMainteAddr.Name = "cmbMainteAddr";
            this.cmbMainteAddr.Size = new System.Drawing.Size(187, 20);
            this.cmbMainteAddr.TabIndex = 5;
            this.cmbMainteAddr.TextChanged += new System.EventHandler(this.cmbMainteAddr_TextChanged);
            // 
            // tbMainteAddr
            // 
            this.tbMainteAddr.Enabled = false;
            this.tbMainteAddr.Location = new System.Drawing.Point(229, 65);
            this.tbMainteAddr.Name = "tbMainteAddr";
            this.tbMainteAddr.Size = new System.Drawing.Size(187, 19);
            this.tbMainteAddr.TabIndex = 7;
            // 
            // tbWriteVal
            // 
            this.tbWriteVal.Location = new System.Drawing.Point(229, 115);
            this.tbWriteVal.Name = "tbWriteVal";
            this.tbWriteVal.Size = new System.Drawing.Size(187, 19);
            this.tbWriteVal.TabIndex = 8;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(227, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 12);
            this.label2.TabIndex = 9;
            this.label2.Text = "書き込み値";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(420, 50);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(51, 12);
            this.label4.TabIndex = 11;
            this.label4.Text = "現在の値";
            // 
            // tbCurrentVal
            // 
            this.tbCurrentVal.Enabled = false;
            this.tbCurrentVal.Location = new System.Drawing.Point(422, 65);
            this.tbCurrentVal.Name = "tbCurrentVal";
            this.tbCurrentVal.Size = new System.Drawing.Size(170, 19);
            this.tbCurrentVal.TabIndex = 10;
            this.tbCurrentVal.TextChanged += new System.EventHandler(this.tbCurrentVal_TextChanged);
            // 
            // btnGetVal
            // 
            this.btnGetVal.Location = new System.Drawing.Point(493, 37);
            this.btnGetVal.Name = "btnGetVal";
            this.btnGetVal.Size = new System.Drawing.Size(99, 23);
            this.btnGetVal.TabIndex = 12;
            this.btnGetVal.Text = "値の再取得";
            this.btnGetVal.UseVisualStyleBackColor = true;
            this.btnGetVal.Click += new System.EventHandler(this.btnGetVal_Click);
            // 
            // btnSendVal
            // 
            this.btnSendVal.Location = new System.Drawing.Point(422, 113);
            this.btnSendVal.Name = "btnSendVal";
            this.btnSendVal.Size = new System.Drawing.Size(170, 23);
            this.btnSendVal.TabIndex = 13;
            this.btnSendVal.Text = "メンテナンスアドレスへ値を書込";
            this.btnSendVal.UseVisualStyleBackColor = true;
            this.btnSendVal.Click += new System.EventHandler(this.btnSendVal_Click);
            // 
            // cbWriteUnlock
            // 
            this.cbWriteUnlock.AutoSize = true;
            this.cbWriteUnlock.Location = new System.Drawing.Point(229, 259);
            this.cbWriteUnlock.Name = "cbWriteUnlock";
            this.cbWriteUnlock.Size = new System.Drawing.Size(138, 16);
            this.cbWriteUnlock.TabIndex = 14;
            this.cbWriteUnlock.Text = "値の書き込みロック解除";
            this.cbWriteUnlock.UseVisualStyleBackColor = true;
            this.cbWriteUnlock.CheckedChanged += new System.EventHandler(this.cbWriteUnlock_CheckedChanged);
            // 
            // tmWriteLock
            // 
            this.tmWriteLock.Interval = 5000;
            this.tmWriteLock.Tick += new System.EventHandler(this.tmWriteLock_Tick);
            // 
            // F91_MachineLogOutputCompletionMainte
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(604, 287);
            this.Controls.Add(this.cbWriteUnlock);
            this.Controls.Add(this.btnSendVal);
            this.Controls.Add(this.btnGetVal);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.tbCurrentVal);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.tbWriteVal);
            this.Controls.Add(this.tbMainteAddr);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.cmbMainteAddr);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.tvMainteMachine);
            this.Name = "F91_MachineLogOutputCompletionMainte";
            this.Text = "F91_MachineLogOutputCompletionMainte";
            this.Load += new System.EventHandler(this.F91_MachineLogOutputCompletionMainte_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TreeView tvMainteMachine;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox cmbMainteAddr;
		private System.Windows.Forms.TextBox tbMainteAddr;
		private System.Windows.Forms.TextBox tbWriteVal;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.TextBox tbCurrentVal;
		private System.Windows.Forms.Button btnGetVal;
		private System.Windows.Forms.Button btnSendVal;
		private System.Windows.Forms.CheckBox cbWriteUnlock;
		private System.Windows.Forms.Timer tmWriteLock;
	}
}