
namespace VSPMasterMaintenance
{
    partial class Top
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.MakeNewType = new System.Windows.Forms.Button();
            this.MakeNewWork = new System.Windows.Forms.Button();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.MakeNewMachine = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // MakeNewType
            // 
            this.MakeNewType.Location = new System.Drawing.Point(37, 45);
            this.MakeNewType.Name = "MakeNewType";
            this.MakeNewType.Size = new System.Drawing.Size(181, 38);
            this.MakeNewType.TabIndex = 0;
            this.MakeNewType.Text = "機種チェック";
            this.MakeNewType.UseVisualStyleBackColor = true;
            this.MakeNewType.Click += new System.EventHandler(this.MakeNewType_Click);
            // 
            // MakeNewWork
            // 
            this.MakeNewWork.Location = new System.Drawing.Point(37, 119);
            this.MakeNewWork.Name = "MakeNewWork";
            this.MakeNewWork.Size = new System.Drawing.Size(181, 43);
            this.MakeNewWork.TabIndex = 1;
            this.MakeNewWork.Text = "作業チェック";
            this.MakeNewWork.UseVisualStyleBackColor = true;
            this.MakeNewWork.Click += new System.EventHandler(this.MakeNewWork_Click);
            // 
            // MakeNewMachine
            // 
            this.MakeNewMachine.Location = new System.Drawing.Point(37, 195);
            this.MakeNewMachine.Name = "MakeNewMachine";
            this.MakeNewMachine.Size = new System.Drawing.Size(180, 42);
            this.MakeNewMachine.TabIndex = 2;
            this.MakeNewMachine.Text = "設備チェック";
            this.MakeNewMachine.UseVisualStyleBackColor = true;
            this.MakeNewMachine.Click += new System.EventHandler(this.MakeNewMachine_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(78, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 15);
            this.label1.TabIndex = 3;
            this.label1.Text = "マスターチェックツール";
            // 
            // Top
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(262, 284);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.MakeNewMachine);
            this.Controls.Add(this.MakeNewWork);
            this.Controls.Add(this.MakeNewType);
            this.Name = "Top";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button MakeNewType;
        private System.Windows.Forms.Button MakeNewWork;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.Button MakeNewMachine;
        private System.Windows.Forms.Label label1;
    }
}

