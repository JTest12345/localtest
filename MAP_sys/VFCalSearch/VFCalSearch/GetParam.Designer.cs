
namespace VFCalSearch
{
    partial class GetParam
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
            this.label1 = new System.Windows.Forms.Label();
            this.paramtext = new System.Windows.Forms.TextBox();
            this.GetAllParam = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 39);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(65, 12);
            this.label1.TabIndex = 0;
            this.label1.Text = "光束_Ra_R9";
            // 
            // paramtext
            // 
            this.paramtext.Location = new System.Drawing.Point(42, 81);
            this.paramtext.Multiline = true;
            this.paramtext.Name = "paramtext";
            this.paramtext.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.paramtext.Size = new System.Drawing.Size(430, 268);
            this.paramtext.TabIndex = 1;
            // 
            // GetAllParam
            // 
            this.GetAllParam.Location = new System.Drawing.Point(151, 33);
            this.GetAllParam.Name = "GetAllParam";
            this.GetAllParam.Size = new System.Drawing.Size(121, 25);
            this.GetAllParam.TabIndex = 2;
            this.GetAllParam.Text = "データ取得";
            this.GetAllParam.UseVisualStyleBackColor = true;
            this.GetAllParam.Click += new System.EventHandler(this.GetAllParam_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(40, 367);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(53, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "進行状況";
            // 
            // progressBar1
            // 
            this.progressBar1.Location = new System.Drawing.Point(109, 363);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(196, 21);
            this.progressBar1.TabIndex = 4;
            // 
            // GetParam
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(525, 402);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.GetAllParam);
            this.Controls.Add(this.paramtext);
            this.Controls.Add(this.label1);
            this.Name = "GetParam";
            this.Text = "光束_Ra_R9上下限取得";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox paramtext;
        private System.Windows.Forms.Button GetAllParam;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ProgressBar progressBar1;
    }
}