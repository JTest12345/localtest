namespace ArmsNascaBridge2
{
    partial class FrmBridgeMain
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
            this.txtLog = new System.Windows.Forms.TextBox();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.更新ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.終了ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.lblVersion = new System.Windows.Forms.ToolStripLabel();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.label1 = new System.Windows.Forms.Label();
            this.button_RelationStart = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.checkBox_defect = new System.Windows.Forms.CheckBox();
            this.checkBox_DieShearSampler = new System.Windows.Forms.CheckBox();
            this.checkBox_WorkMacCond = new System.Windows.Forms.CheckBox();
            this.checkBox_MatLabel = new System.Windows.Forms.CheckBox();
            this.checkBox_WorkFlow = new System.Windows.Forms.CheckBox();
            this.checkBox_General = new System.Windows.Forms.CheckBox();
            this.checkBox_TypeCondtion = new System.Windows.Forms.CheckBox();
            this.checkBox_DBWaferWashLife = new System.Windows.Forms.CheckBox();
            this.checkBox_DBResinLife = new System.Windows.Forms.CheckBox();
            this.checkBox_DefectCt = new System.Windows.Forms.CheckBox();
            this.checkBox_CutPressDie = new System.Windows.Forms.CheckBox();
            this.checkBox_TimeLimit = new System.Windows.Forms.CheckBox();
            this.splitContainer1 = new System.Windows.Forms.SplitContainer();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.contextMenuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtLog
            // 
            this.txtLog.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.txtLog.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.txtLog.Location = new System.Drawing.Point(3, 14);
            this.txtLog.Multiline = true;
            this.txtLog.Name = "txtLog";
            this.txtLog.ReadOnly = true;
            this.txtLog.Size = new System.Drawing.Size(396, 381);
            this.txtLog.TabIndex = 0;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.更新ToolStripMenuItem,
            this.終了ToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(95, 48);
            // 
            // 更新ToolStripMenuItem
            // 
            this.更新ToolStripMenuItem.Name = "更新ToolStripMenuItem";
            this.更新ToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.更新ToolStripMenuItem.Text = "更新";
            // 
            // 終了ToolStripMenuItem
            // 
            this.終了ToolStripMenuItem.Name = "終了ToolStripMenuItem";
            this.終了ToolStripMenuItem.Size = new System.Drawing.Size(94, 22);
            this.終了ToolStripMenuItem.Text = "終了";
            this.終了ToolStripMenuItem.Click += new System.EventHandler(this.終了ToolStripMenuItem1_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblVersion,
            this.toolStripLabel1});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(796, 25);
            this.toolStrip1.TabIndex = 1;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // lblVersion
            // 
            this.lblVersion.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.lblVersion.Name = "lblVersion";
            this.lblVersion.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(23, 22);
            this.toolStripLabel1.Text = "ver.";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(3, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 12);
            this.label1.TabIndex = 3;
            this.label1.Text = "操作ログ";
            // 
            // button_RelationStart
            // 
            this.button_RelationStart.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.button_RelationStart.Location = new System.Drawing.Point(288, 383);
            this.button_RelationStart.Name = "button_RelationStart";
            this.button_RelationStart.Size = new System.Drawing.Size(75, 23);
            this.button_RelationStart.TabIndex = 20;
            this.button_RelationStart.Text = "連携実行";
            this.button_RelationStart.UseVisualStyleBackColor = true;
            this.button_RelationStart.Click += new System.EventHandler(this.button_RelationStart_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(3, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(77, 12);
            this.label2.TabIndex = 6;
            this.label2.Text = "連携対象選択";
            // 
            // checkBox_defect
            // 
            this.checkBox_defect.AutoSize = true;
            this.checkBox_defect.Location = new System.Drawing.Point(23, 303);
            this.checkBox_defect.Name = "checkBox_defect";
            this.checkBox_defect.Size = new System.Drawing.Size(262, 16);
            this.checkBox_defect.TabIndex = 21;
            this.checkBox_defect.Text = "NPPG414：不良明細マスタ　対象データは過去4hr";
            this.checkBox_defect.UseVisualStyleBackColor = true;
            // 
            // checkBox_DieShearSampler
            // 
            this.checkBox_DieShearSampler.AutoSize = true;
            this.checkBox_DieShearSampler.Location = new System.Drawing.Point(23, 347);
            this.checkBox_DieShearSampler.Name = "checkBox_DieShearSampler";
            this.checkBox_DieShearSampler.Size = new System.Drawing.Size(216, 16);
            this.checkBox_DieShearSampler.TabIndex = 22;
            this.checkBox_DieShearSampler.Text = "PSTester：REDダイシェア抜取対象リスト";
            this.checkBox_DieShearSampler.UseVisualStyleBackColor = true;
            // 
            // checkBox_WorkMacCond
            // 
            this.checkBox_WorkMacCond.AutoSize = true;
            this.checkBox_WorkMacCond.Location = new System.Drawing.Point(23, 193);
            this.checkBox_WorkMacCond.Name = "checkBox_WorkMacCond";
            this.checkBox_WorkMacCond.Size = new System.Drawing.Size(72, 16);
            this.checkBox_WorkMacCond.TabIndex = 23;
            this.checkBox_WorkMacCond.Text = "作業設備";
            this.checkBox_WorkMacCond.UseVisualStyleBackColor = true;
            // 
            // checkBox_MatLabel
            // 
            this.checkBox_MatLabel.AutoSize = true;
            this.checkBox_MatLabel.Location = new System.Drawing.Point(23, 281);
            this.checkBox_MatLabel.Name = "checkBox_MatLabel";
            this.checkBox_MatLabel.Size = new System.Drawing.Size(254, 16);
            this.checkBox_MatLabel.TabIndex = 24;
            this.checkBox_MatLabel.Text = "NPPG426：資材ロットマスタ (資材ラべル用マスタ)";
            this.checkBox_MatLabel.UseVisualStyleBackColor = true;
            // 
            // checkBox_WorkFlow
            // 
            this.checkBox_WorkFlow.AutoSize = true;
            this.checkBox_WorkFlow.Location = new System.Drawing.Point(23, 325);
            this.checkBox_WorkFlow.Name = "checkBox_WorkFlow";
            this.checkBox_WorkFlow.Size = new System.Drawing.Size(307, 16);
            this.checkBox_WorkFlow.TabIndex = 25;
            this.checkBox_WorkFlow.Text = "RPPG409：作業順マスタ （※取込設定が有効のラインのみ）";
            this.checkBox_WorkFlow.UseVisualStyleBackColor = true;
            // 
            // checkBox_General
            // 
            this.checkBox_General.AutoSize = true;
            this.checkBox_General.Location = new System.Drawing.Point(23, 128);
            this.checkBox_General.Name = "checkBox_General";
            this.checkBox_General.Size = new System.Drawing.Size(184, 16);
            this.checkBox_General.TabIndex = 26;
            this.checkBox_General.Text = "ラベルチェック値 (DCブレード規制)";
            this.checkBox_General.UseVisualStyleBackColor = true;
            // 
            // checkBox_TypeCondtion
            // 
            this.checkBox_TypeCondtion.AutoSize = true;
            this.checkBox_TypeCondtion.Location = new System.Drawing.Point(23, 77);
            this.checkBox_TypeCondtion.Name = "checkBox_TypeCondtion";
            this.checkBox_TypeCondtion.Size = new System.Drawing.Size(277, 16);
            this.checkBox_TypeCondtion.TabIndex = 27;
            this.checkBox_TypeCondtion.Text = "8_プラズマ洗浄値/3_オーブン硬化/26_資材投入条件";
            this.checkBox_TypeCondtion.UseVisualStyleBackColor = true;
            // 
            // checkBox_DBWaferWashLife
            // 
            this.checkBox_DBWaferWashLife.AutoSize = true;
            this.checkBox_DBWaferWashLife.Location = new System.Drawing.Point(23, 33);
            this.checkBox_DBWaferWashLife.Name = "checkBox_DBWaferWashLife";
            this.checkBox_DBWaferWashLife.Size = new System.Drawing.Size(148, 16);
            this.checkBox_DBWaferWashLife.TabIndex = 28;
            this.checkBox_DBWaferWashLife.Text = "22_資材作業後有効期限";
            this.checkBox_DBWaferWashLife.UseVisualStyleBackColor = true;
            // 
            // checkBox_DBResinLife
            // 
            this.checkBox_DBResinLife.AutoSize = true;
            this.checkBox_DBResinLife.Location = new System.Drawing.Point(23, 55);
            this.checkBox_DBResinLife.Name = "checkBox_DBResinLife";
            this.checkBox_DBResinLife.Size = new System.Drawing.Size(170, 16);
            this.checkBox_DBResinLife.TabIndex = 29;
            this.checkBox_DBResinLife.Text = "5_開封後期限/4_投入後期限";
            this.checkBox_DBResinLife.UseVisualStyleBackColor = true;
            // 
            // checkBox_DefectCt
            // 
            this.checkBox_DefectCt.AutoSize = true;
            this.checkBox_DefectCt.Location = new System.Drawing.Point(23, 150);
            this.checkBox_DefectCt.Name = "checkBox_DefectCt";
            this.checkBox_DefectCt.Size = new System.Drawing.Size(60, 16);
            this.checkBox_DefectCt.TabIndex = 31;
            this.checkBox_DefectCt.Text = "検査数";
            this.checkBox_DefectCt.UseVisualStyleBackColor = true;
            // 
            // checkBox_CutPressDie
            // 
            this.checkBox_CutPressDie.AutoSize = true;
            this.checkBox_CutPressDie.Location = new System.Drawing.Point(23, 215);
            this.checkBox_CutPressDie.Name = "checkBox_CutPressDie";
            this.checkBox_CutPressDie.Size = new System.Drawing.Size(207, 16);
            this.checkBox_CutPressDie.TabIndex = 32;
            this.checkBox_CutPressDie.Text = "カット金型 (作業設備から金型を抜粋)";
            this.checkBox_CutPressDie.UseVisualStyleBackColor = true;
            // 
            // checkBox_TimeLimit
            // 
            this.checkBox_TimeLimit.AutoSize = true;
            this.checkBox_TimeLimit.Location = new System.Drawing.Point(23, 259);
            this.checkBox_TimeLimit.Name = "checkBox_TimeLimit";
            this.checkBox_TimeLimit.Size = new System.Drawing.Size(176, 16);
            this.checkBox_TimeLimit.TabIndex = 33;
            this.checkBox_TimeLimit.Text = "NPPG420：作業実績監視マスタ";
            this.checkBox_TimeLimit.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            this.splitContainer1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.splitContainer1.IsSplitterFixed = true;
            this.splitContainer1.Location = new System.Drawing.Point(12, 28);
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.Controls.Add(this.txtLog);
            this.splitContainer1.Panel1.Controls.Add(this.label1);
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.Controls.Add(this.label6);
            this.splitContainer1.Panel2.Controls.Add(this.label5);
            this.splitContainer1.Panel2.Controls.Add(this.label4);
            this.splitContainer1.Panel2.Controls.Add(this.label3);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_defect);
            this.splitContainer1.Panel2.Controls.Add(this.button_RelationStart);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_DieShearSampler);
            this.splitContainer1.Panel2.Controls.Add(this.label2);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_WorkMacCond);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_TimeLimit);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_MatLabel);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_CutPressDie);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_WorkFlow);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_DefectCt);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_General);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_TypeCondtion);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_DBResinLife);
            this.splitContainer1.Panel2.Controls.Add(this.checkBox_DBWaferWashLife);
            this.splitContainer1.Panel2MinSize = 0;
            this.splitContainer1.Size = new System.Drawing.Size(772, 409);
            this.splitContainer1.SplitterDistance = 402;
            this.splitContainer1.TabIndex = 22;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(2, 244);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(48, 12);
            this.label6.TabIndex = 34;
            this.label6.Text = "■その他";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(3, 178);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(145, 12);
            this.label5.TabIndex = 34;
            this.label5.Text = "■NPPG419：作業設備マスタ";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(3, 113);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(169, 12);
            this.label4.TabIndex = 34;
            this.label4.Text = "■NPPG415：品目基本属性マスタ";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(3, 18);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(144, 12);
            this.label3.TabIndex = 34;
            this.label3.Text = "■NPPG429：PDA条件マスタ";
            // 
            // FrmBridgeMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(796, 449);
            this.Controls.Add(this.splitContainer1);
            this.Controls.Add(this.toolStrip1);
            this.Name = "FrmBridgeMain";
            this.ShowInTaskbar = false;
            this.Text = "ArmsNascaBridge2";
            this.WindowState = System.Windows.Forms.FormWindowState.Minimized;
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmBridgeMain_FormClosing);
            this.Load += new System.EventHandler(this.FrmBridgeMain_Load);
            this.Shown += new System.EventHandler(this.FrmBridgeMain_Shown);
            this.contextMenuStrip1.ResumeLayout(false);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel1.PerformLayout();
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
            this.splitContainer1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtLog;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 更新ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 終了ToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel lblVersion;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button_RelationStart;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.CheckBox checkBox_defect;
        private System.Windows.Forms.CheckBox checkBox_DieShearSampler;
        private System.Windows.Forms.CheckBox checkBox_WorkMacCond;
        private System.Windows.Forms.CheckBox checkBox_MatLabel;
        private System.Windows.Forms.CheckBox checkBox_WorkFlow;
        private System.Windows.Forms.CheckBox checkBox_General;
        private System.Windows.Forms.CheckBox checkBox_TypeCondtion;
        private System.Windows.Forms.CheckBox checkBox_DBWaferWashLife;
        private System.Windows.Forms.CheckBox checkBox_DBResinLife;
        private System.Windows.Forms.CheckBox checkBox_DefectCt;
        private System.Windows.Forms.CheckBox checkBox_CutPressDie;
        private System.Windows.Forms.CheckBox checkBox_TimeLimit;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
    }
}