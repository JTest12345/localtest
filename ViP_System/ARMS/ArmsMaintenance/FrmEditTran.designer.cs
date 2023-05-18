namespace ArmsMaintenance
{
    partial class FrmEditTran
    {
        /// <summary>
        /// 必要なデザイナ変数です。
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

        #region Windows フォーム デザイナで生成されたコード

        /// <summary>
        /// デザイナ サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディタで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmEditTran));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            this.treeWork = new System.Windows.Forms.TreeView();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.txtTypeCd = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.chkLoopCondChange = new System.Windows.Forms.CheckBox();
            this.chkChangePointLot = new System.Windows.Forms.CheckBox();
            this.chkFullInspection = new System.Windows.Forms.CheckBox();
            this.chkWBMapping = new System.Windows.Forms.CheckBox();
            this.txtCutBlendCd = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtResinGr = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.chkWarning = new System.Windows.Forms.CheckBox();
            this.txtScheduleSelectionStandard = new System.Windows.Forms.TextBox();
            this.txtBlendCd = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.chkLifeTest = new System.Windows.Forms.CheckBox();
            this.chkKHLTest = new System.Windows.Forms.CheckBox();
            this.chkColorTest = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWorkEndEmpCd = new System.Windows.Forms.TextBox();
            this.txtWorkStartEmpCd = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.label16 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.txtComment = new System.Windows.Forms.TextBox();
            this.txtProcNo = new System.Windows.Forms.TextBox();
            this.txtMacNo = new System.Windows.Forms.TextBox();
            this.chkWorkEnd = new System.Windows.Forms.CheckBox();
            this.txtProcess = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.txtInspectCt = new System.Windows.Forms.TextBox();
            this.rdoInspectSample = new System.Windows.Forms.RadioButton();
            this.rdoInspectAll = new System.Windows.Forms.RadioButton();
            this.btnUpdateDefect = new System.Windows.Forms.Button();
            this.txtInspectEmpCd = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.txtDefTotal = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.grdDefect = new System.Windows.Forms.DataGridView();
            this.OrderNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CauseCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.CauseName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClassCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ClassName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefectCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DefectCt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OrgDefectCt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.grdResin = new System.Windows.Forms.DataGridView();
            this.chkChangeStock = new System.Windows.Forms.CheckBox();
            this.txtStocker2 = new System.Windows.Forms.TextBox();
            this.grdMaterials = new System.Windows.Forms.DataGridView();
            this.label12 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.txtStocker1 = new System.Windows.Forms.TextBox();
            this.dtpWorkEnd = new System.Windows.Forms.DateTimePicker();
            this.label5 = new System.Windows.Forms.Label();
            this.dtpWorkStart = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.btnSelectMachine = new System.Windows.Forms.Button();
            this.txtMachine = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.grdInspections = new System.Windows.Forms.DataGridView();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.保存ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ロット情報編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.設備資材情報編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.設備樹脂情報編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.設備条件情報編集ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ロット資材追加ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.作業実績削除ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.メンテ履歴参照ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.chkNascaEnd = new System.Windows.Forms.CheckBox();
            this.chkNascaStart = new System.Windows.Forms.CheckBox();
            this.chkDefectEnd = new System.Windows.Forms.CheckBox();
            this.chkPreparationOrder = new System.Windows.Forms.CheckBox();
            this.groupBox3.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDefect)).BeginInit();
            this.groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResin)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterials)).BeginInit();
            this.groupBox5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdInspections)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // treeWork
            // 
            this.treeWork.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.treeWork.FullRowSelect = true;
            this.treeWork.HideSelection = false;
            this.treeWork.ItemHeight = 15;
            this.treeWork.Location = new System.Drawing.Point(14, 62);
            this.treeWork.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.treeWork.Name = "treeWork";
            this.treeWork.Size = new System.Drawing.Size(237, 213);
            this.treeWork.TabIndex = 0;
            this.treeWork.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.treeWork_AfterSelect);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(15, 33);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(40, 14);
            this.label1.TabIndex = 2;
            this.label1.Text = "LotNo";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(62, 30);
            this.txtLotNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.ReadOnly = true;
            this.txtLotNo.Size = new System.Drawing.Size(159, 22);
            this.txtLotNo.TabIndex = 3;
            // 
            // txtTypeCd
            // 
            this.txtTypeCd.Location = new System.Drawing.Point(277, 30);
            this.txtTypeCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtTypeCd.Name = "txtTypeCd";
            this.txtTypeCd.ReadOnly = true;
            this.txtTypeCd.Size = new System.Drawing.Size(189, 22);
            this.txtTypeCd.TabIndex = 5;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(236, 33);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 14);
            this.label2.TabIndex = 4;
            this.label2.Text = "Type";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.chkLoopCondChange);
            this.groupBox3.Controls.Add(this.chkChangePointLot);
            this.groupBox3.Controls.Add(this.chkFullInspection);
            this.groupBox3.Controls.Add(this.chkWBMapping);
            this.groupBox3.Controls.Add(this.txtCutBlendCd);
            this.groupBox3.Controls.Add(this.label7);
            this.groupBox3.Controls.Add(this.txtResinGr);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.chkWarning);
            this.groupBox3.Controls.Add(this.txtScheduleSelectionStandard);
            this.groupBox3.Controls.Add(this.txtBlendCd);
            this.groupBox3.Controls.Add(this.label14);
            this.groupBox3.Controls.Add(this.label9);
            this.groupBox3.Controls.Add(this.chkLifeTest);
            this.groupBox3.Controls.Add(this.chkKHLTest);
            this.groupBox3.Controls.Add(this.chkColorTest);
            this.groupBox3.Location = new System.Drawing.Point(14, 279);
            this.groupBox3.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox3.Size = new System.Drawing.Size(237, 271);
            this.groupBox3.TabIndex = 6;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "特性";
            // 
            // chkLoopCondChange
            // 
            this.chkLoopCondChange.AutoSize = true;
            this.chkLoopCondChange.Enabled = false;
            this.chkLoopCondChange.Location = new System.Drawing.Point(128, 247);
            this.chkLoopCondChange.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkLoopCondChange.Name = "chkLoopCondChange";
            this.chkLoopCondChange.Size = new System.Drawing.Size(103, 18);
            this.chkLoopCondChange.TabIndex = 17;
            this.chkLoopCondChange.Text = "ループ条件変更";
            this.chkLoopCondChange.UseVisualStyleBackColor = true;
            // 
            // chkChangePointLot
            // 
            this.chkChangePointLot.AutoSize = true;
            this.chkChangePointLot.Enabled = false;
            this.chkChangePointLot.Location = new System.Drawing.Point(128, 225);
            this.chkChangePointLot.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkChangePointLot.Name = "chkChangePointLot";
            this.chkChangePointLot.Size = new System.Drawing.Size(103, 18);
            this.chkChangePointLot.TabIndex = 16;
            this.chkChangePointLot.Text = "抜き取り検査有";
            this.chkChangePointLot.UseVisualStyleBackColor = true;
            // 
            // chkFullInspection
            // 
            this.chkFullInspection.AutoSize = true;
            this.chkFullInspection.Enabled = false;
            this.chkFullInspection.Location = new System.Drawing.Point(128, 203);
            this.chkFullInspection.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkFullInspection.Name = "chkFullInspection";
            this.chkFullInspection.Size = new System.Drawing.Size(98, 18);
            this.chkFullInspection.TabIndex = 15;
            this.chkFullInspection.Text = "全数自動検査";
            this.chkFullInspection.UseVisualStyleBackColor = true;
            // 
            // chkWBMapping
            // 
            this.chkWBMapping.AutoSize = true;
            this.chkWBMapping.Enabled = false;
            this.chkWBMapping.Location = new System.Drawing.Point(128, 181);
            this.chkWBMapping.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkWBMapping.Name = "chkWBMapping";
            this.chkWBMapping.Size = new System.Drawing.Size(88, 18);
            this.chkWBMapping.TabIndex = 14;
            this.chkWBMapping.Text = "WBマッピング";
            this.chkWBMapping.UseVisualStyleBackColor = true;
            // 
            // txtCutBlendCd
            // 
            this.txtCutBlendCd.Location = new System.Drawing.Point(9, 113);
            this.txtCutBlendCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCutBlendCd.Name = "txtCutBlendCd";
            this.txtCutBlendCd.ReadOnly = true;
            this.txtCutBlendCd.Size = new System.Drawing.Size(219, 22);
            this.txtCutBlendCd.TabIndex = 13;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(6, 97);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(105, 14);
            this.label7.TabIndex = 12;
            this.label7.Text = "カットブレンドグループ";
            // 
            // txtResinGr
            // 
            this.txtResinGr.Location = new System.Drawing.Point(9, 73);
            this.txtResinGr.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtResinGr.Name = "txtResinGr";
            this.txtResinGr.ReadOnly = true;
            this.txtResinGr.Size = new System.Drawing.Size(219, 22);
            this.txtResinGr.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(6, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 14);
            this.label6.TabIndex = 10;
            this.label6.Text = "樹脂Gr";
            // 
            // chkWarning
            // 
            this.chkWarning.AutoSize = true;
            this.chkWarning.Enabled = false;
            this.chkWarning.Location = new System.Drawing.Point(9, 181);
            this.chkWarning.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkWarning.Name = "chkWarning";
            this.chkWarning.Size = new System.Drawing.Size(50, 18);
            this.chkWarning.TabIndex = 9;
            this.chkWarning.Text = "警告";
            this.chkWarning.UseVisualStyleBackColor = true;
            // 
            // txtScheduleSelectionStandard
            // 
            this.txtScheduleSelectionStandard.Location = new System.Drawing.Point(9, 153);
            this.txtScheduleSelectionStandard.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtScheduleSelectionStandard.Name = "txtScheduleSelectionStandard";
            this.txtScheduleSelectionStandard.ReadOnly = true;
            this.txtScheduleSelectionStandard.Size = new System.Drawing.Size(219, 22);
            this.txtScheduleSelectionStandard.TabIndex = 8;
            // 
            // txtBlendCd
            // 
            this.txtBlendCd.Location = new System.Drawing.Point(8, 33);
            this.txtBlendCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtBlendCd.Name = "txtBlendCd";
            this.txtBlendCd.ReadOnly = true;
            this.txtBlendCd.Size = new System.Drawing.Size(219, 22);
            this.txtBlendCd.TabIndex = 8;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(6, 137);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(79, 14);
            this.label14.TabIndex = 7;
            this.label14.Text = "予定選別規格";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(6, 17);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(58, 14);
            this.label9.TabIndex = 7;
            this.label9.Text = "ブレンドCD";
            // 
            // chkLifeTest
            // 
            this.chkLifeTest.AutoSize = true;
            this.chkLifeTest.Enabled = false;
            this.chkLifeTest.Location = new System.Drawing.Point(9, 225);
            this.chkLifeTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkLifeTest.Name = "chkLifeTest";
            this.chkLifeTest.Size = new System.Drawing.Size(75, 18);
            this.chkLifeTest.TabIndex = 6;
            this.chkLifeTest.Text = "ライフ試験";
            this.chkLifeTest.UseVisualStyleBackColor = true;
            // 
            // chkKHLTest
            // 
            this.chkKHLTest.AutoSize = true;
            this.chkKHLTest.Enabled = false;
            this.chkKHLTest.Location = new System.Drawing.Point(9, 247);
            this.chkKHLTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkKHLTest.Name = "chkKHLTest";
            this.chkKHLTest.Size = new System.Drawing.Size(122, 18);
            this.chkKHLTest.TabIndex = 5;
            this.chkKHLTest.Text = "吸湿保管点灯試験";
            this.chkKHLTest.UseVisualStyleBackColor = true;
            // 
            // chkColorTest
            // 
            this.chkColorTest.AutoSize = true;
            this.chkColorTest.Enabled = false;
            this.chkColorTest.Location = new System.Drawing.Point(9, 203);
            this.chkColorTest.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkColorTest.Name = "chkColorTest";
            this.chkColorTest.Size = new System.Drawing.Size(98, 18);
            this.chkColorTest.TabIndex = 0;
            this.chkColorTest.Text = "先行色調確認";
            this.chkColorTest.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.txtWorkEndEmpCd);
            this.groupBox1.Controls.Add(this.txtWorkStartEmpCd);
            this.groupBox1.Controls.Add(this.label17);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtComment);
            this.groupBox1.Controls.Add(this.txtProcNo);
            this.groupBox1.Controls.Add(this.txtMacNo);
            this.groupBox1.Controls.Add(this.chkWorkEnd);
            this.groupBox1.Controls.Add(this.txtProcess);
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.groupBox2);
            this.groupBox1.Controls.Add(this.groupBox4);
            this.groupBox1.Controls.Add(this.dtpWorkEnd);
            this.groupBox1.Controls.Add(this.label5);
            this.groupBox1.Controls.Add(this.dtpWorkStart);
            this.groupBox1.Controls.Add(this.label4);
            this.groupBox1.Controls.Add(this.btnSelectMachine);
            this.groupBox1.Controls.Add(this.txtMachine);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Location = new System.Drawing.Point(257, 57);
            this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox1.Size = new System.Drawing.Size(664, 682);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "実績";
            // 
            // txtWorkEndEmpCd
            // 
            this.txtWorkEndEmpCd.Location = new System.Drawing.Point(499, 122);
            this.txtWorkEndEmpCd.Name = "txtWorkEndEmpCd";
            this.txtWorkEndEmpCd.Size = new System.Drawing.Size(141, 22);
            this.txtWorkEndEmpCd.TabIndex = 38;
            // 
            // txtWorkStartEmpCd
            // 
            this.txtWorkStartEmpCd.Location = new System.Drawing.Point(499, 94);
            this.txtWorkStartEmpCd.Name = "txtWorkStartEmpCd";
            this.txtWorkStartEmpCd.Size = new System.Drawing.Size(141, 22);
            this.txtWorkStartEmpCd.TabIndex = 36;
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(426, 125);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(67, 14);
            this.label17.TabIndex = 37;
            this.label17.Text = "完了作業者";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(426, 97);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(67, 14);
            this.label16.TabIndex = 35;
            this.label16.Text = "開始作業者";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(25, 92);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(40, 14);
            this.label13.TabIndex = 34;
            this.label13.Text = "コメント";
            // 
            // txtComment
            // 
            this.txtComment.Location = new System.Drawing.Point(71, 72);
            this.txtComment.Multiline = true;
            this.txtComment.Name = "txtComment";
            this.txtComment.Size = new System.Drawing.Size(310, 72);
            this.txtComment.TabIndex = 33;
            // 
            // txtProcNo
            // 
            this.txtProcNo.Location = new System.Drawing.Point(71, 45);
            this.txtProcNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtProcNo.Name = "txtProcNo";
            this.txtProcNo.ReadOnly = true;
            this.txtProcNo.Size = new System.Drawing.Size(77, 22);
            this.txtProcNo.TabIndex = 32;
            // 
            // txtMacNo
            // 
            this.txtMacNo.Location = new System.Drawing.Point(71, 19);
            this.txtMacNo.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMacNo.Name = "txtMacNo";
            this.txtMacNo.ReadOnly = true;
            this.txtMacNo.Size = new System.Drawing.Size(77, 22);
            this.txtMacNo.TabIndex = 31;
            // 
            // chkWorkEnd
            // 
            this.chkWorkEnd.AutoSize = true;
            this.chkWorkEnd.Location = new System.Drawing.Point(451, 71);
            this.chkWorkEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkWorkEnd.Name = "chkWorkEnd";
            this.chkWorkEnd.Size = new System.Drawing.Size(74, 18);
            this.chkWorkEnd.TabIndex = 30;
            this.chkWorkEnd.Text = "作業完了";
            this.chkWorkEnd.UseVisualStyleBackColor = true;
            this.chkWorkEnd.CheckedChanged += new System.EventHandler(this.chkWorkEnd_CheckedChanged);
            // 
            // txtProcess
            // 
            this.txtProcess.Location = new System.Drawing.Point(154, 45);
            this.txtProcess.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtProcess.Name = "txtProcess";
            this.txtProcess.ReadOnly = true;
            this.txtProcess.Size = new System.Drawing.Size(159, 22);
            this.txtProcess.TabIndex = 28;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(34, 49);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(31, 14);
            this.label10.TabIndex = 27;
            this.label10.Text = "工程";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.groupBox6);
            this.groupBox2.Controls.Add(this.btnUpdateDefect);
            this.groupBox2.Controls.Add(this.txtInspectEmpCd);
            this.groupBox2.Controls.Add(this.label15);
            this.groupBox2.Controls.Add(this.txtDefTotal);
            this.groupBox2.Controls.Add(this.label8);
            this.groupBox2.Controls.Add(this.grdDefect);
            this.groupBox2.Location = new System.Drawing.Point(14, 385);
            this.groupBox2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox2.Size = new System.Drawing.Size(638, 293);
            this.groupBox2.TabIndex = 26;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "不良";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.txtInspectCt);
            this.groupBox6.Controls.Add(this.rdoInspectSample);
            this.groupBox6.Controls.Add(this.rdoInspectAll);
            this.groupBox6.Location = new System.Drawing.Point(307, 10);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(204, 39);
            this.groupBox6.TabIndex = 15;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "検査数";
            // 
            // txtInspectCt
            // 
            this.txtInspectCt.Location = new System.Drawing.Point(130, 10);
            this.txtInspectCt.Name = "txtInspectCt";
            this.txtInspectCt.Size = new System.Drawing.Size(68, 22);
            this.txtInspectCt.TabIndex = 16;
            // 
            // rdoInspectSample
            // 
            this.rdoInspectSample.AutoSize = true;
            this.rdoInspectSample.Checked = true;
            this.rdoInspectSample.Location = new System.Drawing.Point(68, 13);
            this.rdoInspectSample.Name = "rdoInspectSample";
            this.rdoInspectSample.Size = new System.Drawing.Size(57, 18);
            this.rdoInspectSample.TabIndex = 1;
            this.rdoInspectSample.TabStop = true;
            this.rdoInspectSample.Text = "抜取り";
            this.rdoInspectSample.UseVisualStyleBackColor = true;
            this.rdoInspectSample.CheckedChanged += new System.EventHandler(this.rdoInspectSample_CheckedChanged);
            // 
            // rdoInspectAll
            // 
            this.rdoInspectAll.AutoSize = true;
            this.rdoInspectAll.Location = new System.Drawing.Point(6, 13);
            this.rdoInspectAll.Name = "rdoInspectAll";
            this.rdoInspectAll.Size = new System.Drawing.Size(49, 18);
            this.rdoInspectAll.TabIndex = 0;
            this.rdoInspectAll.Text = "全数";
            this.rdoInspectAll.UseVisualStyleBackColor = true;
            this.rdoInspectAll.CheckedChanged += new System.EventHandler(this.rdoInspectSample_CheckedChanged);
            // 
            // btnUpdateDefect
            // 
            this.btnUpdateDefect.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnUpdateDefect.Image = ((System.Drawing.Image)(resources.GetObject("btnUpdateDefect.Image")));
            this.btnUpdateDefect.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnUpdateDefect.Location = new System.Drawing.Point(522, 18);
            this.btnUpdateDefect.Name = "btnUpdateDefect";
            this.btnUpdateDefect.Size = new System.Drawing.Size(110, 29);
            this.btnUpdateDefect.TabIndex = 14;
            this.btnUpdateDefect.Text = "不良更新";
            this.btnUpdateDefect.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnUpdateDefect.UseVisualStyleBackColor = true;
            this.btnUpdateDefect.Click += new System.EventHandler(this.btnUpdateDefect_Click);
            // 
            // txtInspectEmpCd
            // 
            this.txtInspectEmpCd.Location = new System.Drawing.Point(193, 20);
            this.txtInspectEmpCd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtInspectEmpCd.Name = "txtInspectEmpCd";
            this.txtInspectEmpCd.Size = new System.Drawing.Size(102, 22);
            this.txtInspectEmpCd.TabIndex = 12;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(144, 23);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(43, 14);
            this.label15.TabIndex = 11;
            this.label15.Text = "検査者";
            // 
            // txtDefTotal
            // 
            this.txtDefTotal.Location = new System.Drawing.Point(43, 20);
            this.txtDefTotal.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtDefTotal.Name = "txtDefTotal";
            this.txtDefTotal.ReadOnly = true;
            this.txtDefTotal.Size = new System.Drawing.Size(86, 22);
            this.txtDefTotal.TabIndex = 8;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(6, 23);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(31, 14);
            this.label8.TabIndex = 7;
            this.label8.Text = "合計";
            // 
            // grdDefect
            // 
            this.grdDefect.AllowUserToAddRows = false;
            this.grdDefect.AllowUserToDeleteRows = false;
            this.grdDefect.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdDefect.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdDefect.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdDefect.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.OrderNo,
            this.CauseCd,
            this.CauseName,
            this.ClassCd,
            this.ClassName,
            this.DefectCd,
            this.DefectName,
            this.DefectCt,
            this.OrgDefectCt});
            this.grdDefect.Location = new System.Drawing.Point(3, 54);
            this.grdDefect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grdDefect.Name = "grdDefect";
            this.grdDefect.RowTemplate.Height = 21;
            this.grdDefect.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.grdDefect.Size = new System.Drawing.Size(628, 235);
            this.grdDefect.TabIndex = 1;
            this.grdDefect.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdDefect_CellValueChanged);
            this.grdDefect.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.grdDefect_DataError);
            // 
            // OrderNo
            // 
            this.OrderNo.HeaderText = "OrderNo";
            this.OrderNo.Name = "OrderNo";
            this.OrderNo.ReadOnly = true;
            this.OrderNo.Visible = false;
            // 
            // CauseCd
            // 
            this.CauseCd.HeaderText = "CauseCd";
            this.CauseCd.Name = "CauseCd";
            this.CauseCd.ReadOnly = true;
            this.CauseCd.Visible = false;
            // 
            // CauseName
            // 
            this.CauseName.DataPropertyName = "CauseName";
            this.CauseName.HeaderText = "起因";
            this.CauseName.Name = "CauseName";
            this.CauseName.ReadOnly = true;
            // 
            // ClassCd
            // 
            this.ClassCd.HeaderText = "ClassCd";
            this.ClassCd.Name = "ClassCd";
            this.ClassCd.ReadOnly = true;
            this.ClassCd.Visible = false;
            // 
            // ClassName
            // 
            this.ClassName.DataPropertyName = "ClassName";
            this.ClassName.HeaderText = "分類";
            this.ClassName.Name = "ClassName";
            this.ClassName.ReadOnly = true;
            // 
            // DefectCd
            // 
            this.DefectCd.HeaderText = "コード";
            this.DefectCd.Name = "DefectCd";
            this.DefectCd.ReadOnly = true;
            // 
            // DefectName
            // 
            this.DefectName.DataPropertyName = "DefectName";
            this.DefectName.HeaderText = "名称";
            this.DefectName.Name = "DefectName";
            this.DefectName.ReadOnly = true;
            // 
            // DefectCt
            // 
            this.DefectCt.DataPropertyName = "DefectCt";
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Yellow;
            this.DefectCt.DefaultCellStyle = dataGridViewCellStyle1;
            this.DefectCt.HeaderText = "数量";
            this.DefectCt.Name = "DefectCt";
            // 
            // OrgDefectCt
            // 
            this.OrgDefectCt.DataPropertyName = "OrgDefectCt";
            this.OrgDefectCt.HeaderText = "元数量";
            this.OrgDefectCt.Name = "OrgDefectCt";
            this.OrgDefectCt.Visible = false;
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.grdResin);
            this.groupBox4.Controls.Add(this.chkChangeStock);
            this.groupBox4.Controls.Add(this.txtStocker2);
            this.groupBox4.Controls.Add(this.grdMaterials);
            this.groupBox4.Controls.Add(this.label12);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.txtStocker1);
            this.groupBox4.Location = new System.Drawing.Point(13, 149);
            this.groupBox4.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox4.Size = new System.Drawing.Size(638, 232);
            this.groupBox4.TabIndex = 25;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "資材";
            // 
            // grdResin
            // 
            this.grdResin.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdResin.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdResin.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdResin.Location = new System.Drawing.Point(4, 146);
            this.grdResin.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grdResin.Name = "grdResin";
            this.grdResin.ReadOnly = true;
            this.grdResin.RowHeadersVisible = false;
            this.grdResin.RowTemplate.Height = 21;
            this.grdResin.Size = new System.Drawing.Size(628, 82);
            this.grdResin.TabIndex = 38;
            // 
            // chkChangeStock
            // 
            this.chkChangeStock.AutoSize = true;
            this.chkChangeStock.Location = new System.Drawing.Point(6, 22);
            this.chkChangeStock.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkChangeStock.Name = "chkChangeStock";
            this.chkChangeStock.Size = new System.Drawing.Size(117, 18);
            this.chkChangeStock.TabIndex = 37;
            this.chkChangeStock.Text = "ストッカー情報変更";
            this.chkChangeStock.UseVisualStyleBackColor = true;
            this.chkChangeStock.CheckedChanged += new System.EventHandler(this.chkChangeStock_CheckedChanged);
            // 
            // txtStocker2
            // 
            this.txtStocker2.Location = new System.Drawing.Point(415, 20);
            this.txtStocker2.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtStocker2.Name = "txtStocker2";
            this.txtStocker2.ReadOnly = true;
            this.txtStocker2.Size = new System.Drawing.Size(158, 22);
            this.txtStocker2.TabIndex = 36;
            // 
            // grdMaterials
            // 
            this.grdMaterials.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdMaterials.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdMaterials.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdMaterials.Location = new System.Drawing.Point(4, 46);
            this.grdMaterials.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grdMaterials.Name = "grdMaterials";
            this.grdMaterials.ReadOnly = true;
            this.grdMaterials.RowHeadersVisible = false;
            this.grdMaterials.RowTemplate.Height = 21;
            this.grdMaterials.Size = new System.Drawing.Size(628, 96);
            this.grdMaterials.TabIndex = 0;
            this.grdMaterials.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdMaterials_CellContentClick);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(352, 23);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(57, 14);
            this.label12.TabIndex = 35;
            this.label12.Text = "ストッカー2";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(130, 23);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(58, 14);
            this.label11.TabIndex = 33;
            this.label11.Text = "ストッカー１";
            // 
            // txtStocker1
            // 
            this.txtStocker1.Location = new System.Drawing.Point(194, 20);
            this.txtStocker1.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtStocker1.Name = "txtStocker1";
            this.txtStocker1.ReadOnly = true;
            this.txtStocker1.Size = new System.Drawing.Size(127, 22);
            this.txtStocker1.TabIndex = 34;
            // 
            // dtpWorkEnd
            // 
            this.dtpWorkEnd.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkEnd.Location = new System.Drawing.Point(451, 45);
            this.dtpWorkEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkEnd.Name = "dtpWorkEnd";
            this.dtpWorkEnd.Size = new System.Drawing.Size(189, 22);
            this.dtpWorkEnd.TabIndex = 20;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(390, 49);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(55, 14);
            this.label5.TabIndex = 19;
            this.label5.Text = "完了時間";
            // 
            // dtpWorkStart
            // 
            this.dtpWorkStart.CustomFormat = "yyyy/MM/dd HH:mm:ss";
            this.dtpWorkStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpWorkStart.Location = new System.Drawing.Point(451, 19);
            this.dtpWorkStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.dtpWorkStart.Name = "dtpWorkStart";
            this.dtpWorkStart.Size = new System.Drawing.Size(189, 22);
            this.dtpWorkStart.TabIndex = 18;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(390, 22);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 14);
            this.label4.TabIndex = 17;
            this.label4.Text = "開始時間";
            // 
            // btnSelectMachine
            // 
            this.btnSelectMachine.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnSelectMachine.Image = global::ArmsMaintenance.Properties.Resources.viewmag;
            this.btnSelectMachine.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.btnSelectMachine.Location = new System.Drawing.Point(319, 16);
            this.btnSelectMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnSelectMachine.Name = "btnSelectMachine";
            this.btnSelectMachine.Size = new System.Drawing.Size(62, 27);
            this.btnSelectMachine.TabIndex = 16;
            this.btnSelectMachine.Text = "選択";
            this.btnSelectMachine.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageBeforeText;
            this.btnSelectMachine.UseVisualStyleBackColor = true;
            this.btnSelectMachine.Click += new System.EventHandler(this.btnSelectMachine_Click);
            // 
            // txtMachine
            // 
            this.txtMachine.Location = new System.Drawing.Point(154, 19);
            this.txtMachine.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMachine.Name = "txtMachine";
            this.txtMachine.ReadOnly = true;
            this.txtMachine.Size = new System.Drawing.Size(159, 22);
            this.txtMachine.TabIndex = 15;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(10, 23);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(55, 14);
            this.label3.TabIndex = 14;
            this.label3.Text = "作業号機";
            // 
            // groupBox5
            // 
            this.groupBox5.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.groupBox5.Controls.Add(this.grdInspections);
            this.groupBox5.Location = new System.Drawing.Point(14, 554);
            this.groupBox5.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Padding = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.groupBox5.Size = new System.Drawing.Size(236, 185);
            this.groupBox5.TabIndex = 28;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "検査・抜き取り";
            // 
            // grdInspections
            // 
            this.grdInspections.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.grdInspections.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdInspections.Dock = System.Windows.Forms.DockStyle.Fill;
            this.grdInspections.Location = new System.Drawing.Point(3, 17);
            this.grdInspections.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.grdInspections.MultiSelect = false;
            this.grdInspections.Name = "grdInspections";
            this.grdInspections.ReadOnly = true;
            this.grdInspections.RowHeadersVisible = false;
            this.grdInspections.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToDisplayedHeaders;
            this.grdInspections.RowTemplate.Height = 21;
            this.grdInspections.Size = new System.Drawing.Size(230, 166);
            this.grdInspections.TabIndex = 0;
            // 
            // menuStrip1
            // 
            this.menuStrip1.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.保存ToolStripMenuItem,
            this.ロット情報編集ToolStripMenuItem,
            this.設備資材情報編集ToolStripMenuItem,
            this.設備樹脂情報編集ToolStripMenuItem,
            this.設備条件情報編集ToolStripMenuItem,
            this.ロット資材追加ToolStripMenuItem,
            this.作業実績削除ToolStripMenuItem,
            this.メンテ履歴参照ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(933, 24);
            this.menuStrip1.TabIndex = 29;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // 保存ToolStripMenuItem
            // 
            this.保存ToolStripMenuItem.Name = "保存ToolStripMenuItem";
            this.保存ToolStripMenuItem.Size = new System.Drawing.Size(43, 20);
            this.保存ToolStripMenuItem.Text = "保存";
            this.保存ToolStripMenuItem.Click += new System.EventHandler(this.保存ToolStripMenuItem_Click);
            // 
            // ロット情報編集ToolStripMenuItem
            // 
            this.ロット情報編集ToolStripMenuItem.Name = "ロット情報編集ToolStripMenuItem";
            this.ロット情報編集ToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.ロット情報編集ToolStripMenuItem.Text = "ロット情報編集";
            this.ロット情報編集ToolStripMenuItem.Click += new System.EventHandler(this.ロット情報編集ToolStripMenuItem_Click);
            // 
            // 設備資材情報編集ToolStripMenuItem
            // 
            this.設備資材情報編集ToolStripMenuItem.Name = "設備資材情報編集ToolStripMenuItem";
            this.設備資材情報編集ToolStripMenuItem.Size = new System.Drawing.Size(121, 20);
            this.設備資材情報編集ToolStripMenuItem.Text = "設備：資材情報編集";
            this.設備資材情報編集ToolStripMenuItem.Click += new System.EventHandler(this.設備資材情報編集ToolStripMenuItem_Click);
            // 
            // 設備樹脂情報編集ToolStripMenuItem
            // 
            this.設備樹脂情報編集ToolStripMenuItem.Name = "設備樹脂情報編集ToolStripMenuItem";
            this.設備樹脂情報編集ToolStripMenuItem.Size = new System.Drawing.Size(121, 20);
            this.設備樹脂情報編集ToolStripMenuItem.Text = "設備：樹脂情報編集";
            this.設備樹脂情報編集ToolStripMenuItem.Click += new System.EventHandler(this.設備樹脂情報編集ToolStripMenuItem_Click);
            // 
            // 設備条件情報編集ToolStripMenuItem
            // 
            this.設備条件情報編集ToolStripMenuItem.Name = "設備条件情報編集ToolStripMenuItem";
            this.設備条件情報編集ToolStripMenuItem.Size = new System.Drawing.Size(121, 20);
            this.設備条件情報編集ToolStripMenuItem.Text = "設備：条件情報編集";
            this.設備条件情報編集ToolStripMenuItem.Click += new System.EventHandler(this.設備条件情報編集ToolStripMenuItem_Click);
            // 
            // ロット資材追加ToolStripMenuItem
            // 
            this.ロット資材追加ToolStripMenuItem.Name = "ロット資材追加ToolStripMenuItem";
            this.ロット資材追加ToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.ロット資材追加ToolStripMenuItem.Text = "ロット資材追加";
            this.ロット資材追加ToolStripMenuItem.Click += new System.EventHandler(this.ロット資材追加ToolStripMenuItem_Click);
            // 
            // 作業実績削除ToolStripMenuItem
            // 
            this.作業実績削除ToolStripMenuItem.Name = "作業実績削除ToolStripMenuItem";
            this.作業実績削除ToolStripMenuItem.Size = new System.Drawing.Size(91, 20);
            this.作業実績削除ToolStripMenuItem.Text = "作業実績削除";
            this.作業実績削除ToolStripMenuItem.Click += new System.EventHandler(this.作業実績削除ToolStripMenuItem_Click);
            // 
            // メンテ履歴参照ToolStripMenuItem
            // 
            this.メンテ履歴参照ToolStripMenuItem.Name = "メンテ履歴参照ToolStripMenuItem";
            this.メンテ履歴参照ToolStripMenuItem.Size = new System.Drawing.Size(93, 20);
            this.メンテ履歴参照ToolStripMenuItem.Text = "メンテ履歴参照";
            this.メンテ履歴参照ToolStripMenuItem.Click += new System.EventHandler(this.メンテ履歴参照ToolStripMenuItem_Click);
            // 
            // chkNascaEnd
            // 
            this.chkNascaEnd.AutoSize = true;
            this.chkNascaEnd.Location = new System.Drawing.Point(803, 35);
            this.chkNascaEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkNascaEnd.Name = "chkNascaEnd";
            this.chkNascaEnd.Size = new System.Drawing.Size(112, 18);
            this.chkNascaEnd.TabIndex = 31;
            this.chkNascaEnd.Text = "NASCA完了連携";
            this.chkNascaEnd.UseVisualStyleBackColor = true;
            this.chkNascaEnd.CheckedChanged += new System.EventHandler(this.chkNascaEnd_CheckedChanged);
            // 
            // chkNascaStart
            // 
            this.chkNascaStart.AutoSize = true;
            this.chkNascaStart.Location = new System.Drawing.Point(686, 35);
            this.chkNascaStart.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkNascaStart.Name = "chkNascaStart";
            this.chkNascaStart.Size = new System.Drawing.Size(112, 18);
            this.chkNascaStart.TabIndex = 32;
            this.chkNascaStart.Text = "NASCA開始連携";
            this.chkNascaStart.UseVisualStyleBackColor = true;
            this.chkNascaStart.CheckedChanged += new System.EventHandler(this.chkNascaStart_CheckedChanged);
            // 
            // chkDefectEnd
            // 
            this.chkDefectEnd.AutoSize = true;
            this.chkDefectEnd.Enabled = false;
            this.chkDefectEnd.Location = new System.Drawing.Point(582, 35);
            this.chkDefectEnd.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkDefectEnd.Name = "chkDefectEnd";
            this.chkDefectEnd.Size = new System.Drawing.Size(98, 18);
            this.chkDefectEnd.TabIndex = 33;
            this.chkDefectEnd.Text = "不良登録完了";
            this.chkDefectEnd.UseVisualStyleBackColor = true;
            // 
            // chkPreparationOrder
            // 
            this.chkPreparationOrder.AutoSize = true;
            this.chkPreparationOrder.Location = new System.Drawing.Point(484, 34);
            this.chkPreparationOrder.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.chkPreparationOrder.Name = "chkPreparationOrder";
            this.chkPreparationOrder.Size = new System.Drawing.Size(86, 18);
            this.chkPreparationOrder.TabIndex = 39;
            this.chkPreparationOrder.Text = "調合依頼済";
            this.chkPreparationOrder.UseVisualStyleBackColor = true;
            // 
            // FrmEditTran
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 14F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(933, 746);
            this.Controls.Add(this.chkPreparationOrder);
            this.Controls.Add(this.chkDefectEnd);
            this.Controls.Add(this.chkNascaStart);
            this.Controls.Add(this.chkNascaEnd);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.txtTypeCd);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.txtLotNo);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.treeWork);
            this.Controls.Add(this.menuStrip1);
            this.Font = new System.Drawing.Font("Tahoma", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.MinimumSize = new System.Drawing.Size(874, 694);
            this.Name = "FrmEditTran";
            this.Text = "製造実績入力";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.FrmEditTran_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdDefect)).EndInit();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdResin)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.grdMaterials)).EndInit();
            this.groupBox5.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdInspections)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TreeView treeWork;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtLotNo;
        private System.Windows.Forms.TextBox txtTypeCd;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox chkColorTest;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TextBox txtDefTotal;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.DataGridView grdDefect;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.DataGridView grdMaterials;
        private System.Windows.Forms.DateTimePicker dtpWorkEnd;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DateTimePicker dtpWorkStart;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Button btnSelectMachine;
        private System.Windows.Forms.TextBox txtMachine;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.CheckBox chkLifeTest;
        private System.Windows.Forms.CheckBox chkKHLTest;
        private System.Windows.Forms.TextBox txtBlendCd;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox chkWarning;
        private System.Windows.Forms.TextBox txtProcess;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.CheckBox chkWorkEnd;
        private System.Windows.Forms.TextBox txtMacNo;
        private System.Windows.Forms.TextBox txtProcNo;
        private System.Windows.Forms.TextBox txtResinGr;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtCutBlendCd;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.DataGridView grdInspections;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem 保存ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 設備資材情報編集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 作業実績削除ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem 設備樹脂情報編集ToolStripMenuItem;
        private System.Windows.Forms.TextBox txtStocker2;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtStocker1;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtComment;
        private System.Windows.Forms.CheckBox chkChangeStock;
        private System.Windows.Forms.ToolStripMenuItem 設備条件情報編集ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem ロット情報編集ToolStripMenuItem;
        private System.Windows.Forms.TextBox txtInspectEmpCd;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtWorkStartEmpCd;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox txtWorkEndEmpCd;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.CheckBox chkNascaEnd;
        private System.Windows.Forms.CheckBox chkNascaStart;
        private System.Windows.Forms.ToolStripMenuItem ロット資材追加ToolStripMenuItem;
        private System.Windows.Forms.DataGridView grdResin;
        private System.Windows.Forms.Button btnUpdateDefect;
        private System.Windows.Forms.ToolStripMenuItem メンテ履歴参照ToolStripMenuItem;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.RadioButton rdoInspectSample;
        private System.Windows.Forms.RadioButton rdoInspectAll;
        private System.Windows.Forms.TextBox txtInspectCt;
        private System.Windows.Forms.CheckBox chkFullInspection;
        private System.Windows.Forms.CheckBox chkWBMapping;
        private System.Windows.Forms.CheckBox chkChangePointLot;
		private System.Windows.Forms.CheckBox chkDefectEnd;
        private System.Windows.Forms.CheckBox chkPreparationOrder;
        private System.Windows.Forms.CheckBox chkLoopCondChange;
        private System.Windows.Forms.TextBox txtScheduleSelectionStandard;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrderNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn CauseCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn CauseName;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClassCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn ClassName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefectCd;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefectName;
        private System.Windows.Forms.DataGridViewTextBoxColumn DefectCt;
        private System.Windows.Forms.DataGridViewTextBoxColumn OrgDefectCt;
    }
}

