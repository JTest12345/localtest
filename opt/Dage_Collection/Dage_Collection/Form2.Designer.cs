namespace Dage_Collection {
    partial class DataChangeForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.Label1 = new System.Windows.Forms.Label();
            this.Reason_ListBox = new System.Windows.Forms.ListBox();
            this.Label2 = new System.Windows.Forms.Label();
            this.TextBox1 = new System.Windows.Forms.TextBox();
            this.Label3 = new System.Windows.Forms.Label();
            this.Change_ListBox = new System.Windows.Forms.ListBox();
            this.Panel1 = new System.Windows.Forms.Panel();
            this.Label8 = new System.Windows.Forms.Label();
            this.TextBox2 = new System.Windows.Forms.TextBox();
            this.Label7 = new System.Windows.Forms.Label();
            this.Destruction_Mode = new System.Windows.Forms.TextBox();
            this.Label6 = new System.Windows.Forms.Label();
            this.Label5 = new System.Windows.Forms.Label();
            this.Label4 = new System.Windows.Forms.Label();
            this.Row_Num = new System.Windows.Forms.TextBox();
            this.Change_Go = new System.Windows.Forms.Button();
            this.Back_Button = new System.Windows.Forms.Button();
            this.Panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // Label1
            // 
            this.Label1.AutoSize = true;
            this.Label1.Location = new System.Drawing.Point(5, 5);
            this.Label1.Name = "Label1";
            this.Label1.Size = new System.Drawing.Size(142, 12);
            this.Label1.TabIndex = 4;
            this.Label1.Text = "修正理由を選択して下さい。";
            // 
            // Reason_ListBox
            // 
            this.Reason_ListBox.FormattingEnabled = true;
            this.Reason_ListBox.ItemHeight = 12;
            this.Reason_ListBox.Items.AddRange(new object[] {
            "異物による為",
            "吸着不足により基板が動いてしまった為",
            "入力ミス(破壊モード、コメント)",
            "その他"});
            this.Reason_ListBox.Location = new System.Drawing.Point(5, 20);
            this.Reason_ListBox.Name = "Reason_ListBox";
            this.Reason_ListBox.Size = new System.Drawing.Size(200, 52);
            this.Reason_ListBox.TabIndex = 3;
            this.Reason_ListBox.SelectedIndexChanged += new System.EventHandler(this.Reason_ListBox_SelectedIndexChanged);
            // 
            // Label2
            // 
            this.Label2.AutoSize = true;
            this.Label2.Location = new System.Drawing.Point(5, 80);
            this.Label2.Name = "Label2";
            this.Label2.Size = new System.Drawing.Size(238, 12);
            this.Label2.TabIndex = 6;
            this.Label2.Text = "「その他」を選択した場合は理由を書いてください。";
            // 
            // TextBox1
            // 
            this.TextBox1.Location = new System.Drawing.Point(5, 95);
            this.TextBox1.Name = "TextBox1";
            this.TextBox1.Size = new System.Drawing.Size(280, 19);
            this.TextBox1.TabIndex = 5;
            // 
            // Label3
            // 
            this.Label3.AutoSize = true;
            this.Label3.Location = new System.Drawing.Point(5, 130);
            this.Label3.Name = "Label3";
            this.Label3.Size = new System.Drawing.Size(142, 12);
            this.Label3.TabIndex = 7;
            this.Label3.Text = "修正内容を選択して下さい。";
            // 
            // Change_ListBox
            // 
            this.Change_ListBox.FormattingEnabled = true;
            this.Change_ListBox.ItemHeight = 12;
            this.Change_ListBox.Items.AddRange(new object[] {
            "データを消したい",
            "破壊モードを修正したい",
            "コメントを修正したい"});
            this.Change_ListBox.Location = new System.Drawing.Point(5, 145);
            this.Change_ListBox.Name = "Change_ListBox";
            this.Change_ListBox.Size = new System.Drawing.Size(120, 40);
            this.Change_ListBox.TabIndex = 8;
            this.Change_ListBox.SelectedValueChanged += new System.EventHandler(this.Change_ListBox_SelectedValueChanged);
            // 
            // Panel1
            // 
            this.Panel1.Controls.Add(this.Label8);
            this.Panel1.Controls.Add(this.TextBox2);
            this.Panel1.Controls.Add(this.Label7);
            this.Panel1.Controls.Add(this.Destruction_Mode);
            this.Panel1.Controls.Add(this.Label6);
            this.Panel1.Controls.Add(this.Label5);
            this.Panel1.Controls.Add(this.Label4);
            this.Panel1.Controls.Add(this.Row_Num);
            this.Panel1.Location = new System.Drawing.Point(5, 190);
            this.Panel1.Name = "Panel1";
            this.Panel1.Size = new System.Drawing.Size(285, 95);
            this.Panel1.TabIndex = 9;
            // 
            // Label8
            // 
            this.Label8.AutoSize = true;
            this.Label8.Location = new System.Drawing.Point(150, 30);
            this.Label8.Name = "Label8";
            this.Label8.Size = new System.Drawing.Size(37, 12);
            this.Label8.TabIndex = 14;
            this.Label8.Text = "(0～9)";
            // 
            // TextBox2
            // 
            this.TextBox2.Location = new System.Drawing.Point(5, 70);
            this.TextBox2.Name = "TextBox2";
            this.TextBox2.Size = new System.Drawing.Size(280, 19);
            this.TextBox2.TabIndex = 13;
            // 
            // Label7
            // 
            this.Label7.AutoSize = true;
            this.Label7.Location = new System.Drawing.Point(5, 55);
            this.Label7.Name = "Label7";
            this.Label7.Size = new System.Drawing.Size(90, 12);
            this.Label7.TabIndex = 12;
            this.Label7.Text = "修正後のコメント：";
            // 
            // Destruction_Mode
            // 
            this.Destruction_Mode.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Destruction_Mode.Location = new System.Drawing.Point(115, 26);
            this.Destruction_Mode.Name = "Destruction_Mode";
            this.Destruction_Mode.Size = new System.Drawing.Size(30, 19);
            this.Destruction_Mode.TabIndex = 11;
            // 
            // Label6
            // 
            this.Label6.AutoSize = true;
            this.Label6.Location = new System.Drawing.Point(5, 30);
            this.Label6.Name = "Label6";
            this.Label6.Size = new System.Drawing.Size(109, 12);
            this.Label6.TabIndex = 10;
            this.Label6.Text = "修正後の破壊モード：";
            // 
            // Label5
            // 
            this.Label5.AutoSize = true;
            this.Label5.Location = new System.Drawing.Point(125, 5);
            this.Label5.Name = "Label5";
            this.Label5.Size = new System.Drawing.Size(95, 12);
            this.Label5.TabIndex = 9;
            this.Label5.Text = "(1～30) 入力必須";
            // 
            // Label4
            // 
            this.Label4.AutoSize = true;
            this.Label4.Location = new System.Drawing.Point(5, 5);
            this.Label4.Name = "Label4";
            this.Label4.Size = new System.Drawing.Size(85, 12);
            this.Label4.TabIndex = 8;
            this.Label4.Text = "何行目のデータ：";
            // 
            // Row_Num
            // 
            this.Row_Num.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.Row_Num.Location = new System.Drawing.Point(90, 1);
            this.Row_Num.Name = "Row_Num";
            this.Row_Num.Size = new System.Drawing.Size(30, 19);
            this.Row_Num.TabIndex = 7;
            // 
            // Change_Go
            // 
            this.Change_Go.BackColor = System.Drawing.Color.Yellow;
            this.Change_Go.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Change_Go.Location = new System.Drawing.Point(5, 290);
            this.Change_Go.Name = "Change_Go";
            this.Change_Go.Size = new System.Drawing.Size(140, 50);
            this.Change_Go.TabIndex = 10;
            this.Change_Go.Text = "修正実施";
            this.Change_Go.UseVisualStyleBackColor = false;
            this.Change_Go.Click += new System.EventHandler(this.Change_Go_Click);
            // 
            // Back_Button
            // 
            this.Back_Button.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Back_Button.Location = new System.Drawing.Point(243, 310);
            this.Back_Button.Name = "Back_Button";
            this.Back_Button.Size = new System.Drawing.Size(50, 30);
            this.Back_Button.TabIndex = 11;
            this.Back_Button.Text = "戻る";
            this.Back_Button.UseVisualStyleBackColor = true;
            this.Back_Button.Click += new System.EventHandler(this.Back_Button_Click);
            // 
            // DataChangeForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(294, 341);
            this.Controls.Add(this.Back_Button);
            this.Controls.Add(this.Change_Go);
            this.Controls.Add(this.Panel1);
            this.Controls.Add(this.Change_ListBox);
            this.Controls.Add(this.Label3);
            this.Controls.Add(this.Label2);
            this.Controls.Add(this.TextBox1);
            this.Controls.Add(this.Label1);
            this.Controls.Add(this.Reason_ListBox);
            this.Name = "DataChangeForm";
            this.Text = "Form2";
            this.Load += new System.EventHandler(this.DataChangeForm_Load);
            this.Panel1.ResumeLayout(false);
            this.Panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        internal System.Windows.Forms.Label Label1;
        internal System.Windows.Forms.ListBox Reason_ListBox;
        internal System.Windows.Forms.Label Label2;
        internal System.Windows.Forms.TextBox TextBox1;
        internal System.Windows.Forms.Label Label3;
        internal System.Windows.Forms.ListBox Change_ListBox;
        internal System.Windows.Forms.Panel Panel1;
        internal System.Windows.Forms.Label Label8;
        internal System.Windows.Forms.TextBox TextBox2;
        internal System.Windows.Forms.Label Label7;
        internal System.Windows.Forms.TextBox Destruction_Mode;
        internal System.Windows.Forms.Label Label6;
        internal System.Windows.Forms.Label Label5;
        internal System.Windows.Forms.Label Label4;
        internal System.Windows.Forms.TextBox Row_Num;
        internal System.Windows.Forms.Button Change_Go;
        internal System.Windows.Forms.Button Back_Button;
    }
}