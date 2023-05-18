namespace GEICS
{
    partial class F04_ErrorRecord
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F04_ErrorRecord));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.timerSD = new System.Windows.Forms.Timer(this.components);
            this.gvQCErrList = new System.Windows.Forms.DataGridView();
            this.確認 = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Assets_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LabelCheck_NO = new System.Windows.Forms.DataGridViewButtonColumn();
            this.Check_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Type_CD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.NascaLot_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Magazine_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DBMachine_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WBMachine_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Defect_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Inspection_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Confirm_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Measure_DT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Equipment_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Timing_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Inline_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Message = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.BackNum_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastUpd_DT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QCNR_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Defect_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Multi_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.UpdUser_CD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Inspection_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tvQCNRBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsTvQCNR = new GEICS.dsTvQCNR();
            this.label23 = new System.Windows.Forms.Label();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label6 = new System.Windows.Forms.Label();
            this.rbUnConfirmedData = new System.Windows.Forms.RadioButton();
            this.rbAllData = new System.Windows.Forms.RadioButton();
            this.gbGraph = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtbEndMinute = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtbStartMinute = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtbEndHour = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtbStartHour = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtbEndDay = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtbStartDay = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.txtbEndYear = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtbEndMonth = new System.Windows.Forms.TextBox();
            this.txtbStartYear = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtbStartMonth = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.timerED = new System.Windows.Forms.Timer(this.components);
            this.btnInlineDisp = new System.Windows.Forms.Button();
            this.btnReport = new System.Windows.Forms.Button();
            this.btnMasterSetting = new System.Windows.Forms.Button();
            this.cmbbLineNo = new SLCommonLib.UCComboBox();
            this.txtbGraphPlotNum = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.cmbbPlantNM = new SLCommonLib.UCComboBox();
            this.label3 = new System.Windows.Forms.Label();
            this.cmbbLineCategory = new SLCommonLib.UCComboBox();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.toolbtnErrorLog = new System.Windows.Forms.ToolStripButton();
            this.toolbtnReport = new System.Windows.Forms.ToolStripButton();
            this.toolbtnMasterSetting = new System.Windows.Forms.ToolStripButton();
            this.toolbtnPhosphorSheetMasterSetting = new System.Windows.Forms.ToolStripButton();
            this.toolbtnDefect = new System.Windows.Forms.ToolStripButton();
            this.dtpStart = new System.Windows.Forms.DateTimePicker();
            this.dtpEnd = new System.Windows.Forms.DateTimePicker();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.chkViewDBWBMachine = new System.Windows.Forms.CheckBox();
            ((System.ComponentModel.ISupportInitialize)(this.gvQCErrList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvQCNRBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTvQCNR)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.gbGraph.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // timerSD
            // 
            this.timerSD.Enabled = true;
            this.timerSD.Interval = 600000;
            this.timerSD.Tick += new System.EventHandler(this.timerSD_Tick);
            // 
            // gvQCErrList
            // 
            this.gvQCErrList.AllowUserToAddRows = false;
            this.gvQCErrList.AllowUserToOrderColumns = true;
            resources.ApplyResources(this.gvQCErrList, "gvQCErrList");
            this.gvQCErrList.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvQCErrList.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.gvQCErrList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvQCErrList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.確認,
            this.Assets_NM,
            this.LabelCheck_NO,
            this.Check_NO,
            this.Type_CD,
            this.NascaLot_NO,
            this.Magazine_NO,
            this.DBMachine_NM,
            this.WBMachine_NM,
            this.Defect_NM,
            this.Inspection_NM,
            this.Confirm_NM,
            this.Measure_DT,
            this.Equipment_NO,
            this.Timing_NM,
            this.Inline_NM,
            this.Message,
            this.BackNum_NO,
            this.LastUpd_DT,
            this.QCNR_NO,
            this.Defect_NO,
            this.Multi_NO,
            this.UpdUser_CD,
            this.Inspection_NO});
            this.gvQCErrList.DataSource = this.tvQCNRBindingSource;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle4.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle4.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle4.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.gvQCErrList.DefaultCellStyle = dataGridViewCellStyle4;
            this.gvQCErrList.Name = "gvQCErrList";
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.gvQCErrList.RowHeadersDefaultCellStyle = dataGridViewCellStyle5;
            this.gvQCErrList.RowTemplate.Height = 21;
            this.gvQCErrList.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvQCErrList_CellContentClick);
            this.gvQCErrList.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvQCErrList_CellFormatting);
            // 
            // 確認
            // 
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.確認.DefaultCellStyle = dataGridViewCellStyle2;
            resources.ApplyResources(this.確認, "確認");
            this.確認.Name = "確認";
            this.確認.Text = "確認";
            this.確認.UseColumnTextForButtonValue = true;
            // 
            // Assets_NM
            // 
            this.Assets_NM.DataPropertyName = "Assets_NM";
            resources.ApplyResources(this.Assets_NM, "Assets_NM");
            this.Assets_NM.Name = "Assets_NM";
            // 
            // LabelCheck_NO
            // 
            this.LabelCheck_NO.DataPropertyName = "Check_NO";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.LabelCheck_NO.DefaultCellStyle = dataGridViewCellStyle3;
            resources.ApplyResources(this.LabelCheck_NO, "LabelCheck_NO");
            this.LabelCheck_NO.Name = "LabelCheck_NO";
            this.LabelCheck_NO.Text = "入力";
            this.LabelCheck_NO.UseColumnTextForButtonValue = true;
            // 
            // Check_NO
            // 
            this.Check_NO.DataPropertyName = "Check_NO";
            resources.ApplyResources(this.Check_NO, "Check_NO");
            this.Check_NO.Name = "Check_NO";
            this.Check_NO.ReadOnly = true;
            // 
            // Type_CD
            // 
            this.Type_CD.DataPropertyName = "Type_CD";
            resources.ApplyResources(this.Type_CD, "Type_CD");
            this.Type_CD.Name = "Type_CD";
            this.Type_CD.ReadOnly = true;
            // 
            // NascaLot_NO
            // 
            this.NascaLot_NO.DataPropertyName = "NascaLot_NO";
            resources.ApplyResources(this.NascaLot_NO, "NascaLot_NO");
            this.NascaLot_NO.Name = "NascaLot_NO";
            this.NascaLot_NO.ReadOnly = true;
            // 
            // Magazine_NO
            // 
            this.Magazine_NO.DataPropertyName = "Magazine_NO";
            resources.ApplyResources(this.Magazine_NO, "Magazine_NO");
            this.Magazine_NO.Name = "Magazine_NO";
            this.Magazine_NO.ReadOnly = true;
            // 
            // DBMachine_NM
            // 
            this.DBMachine_NM.DataPropertyName = "DBMachine_NM";
            resources.ApplyResources(this.DBMachine_NM, "DBMachine_NM");
            this.DBMachine_NM.Name = "DBMachine_NM";
            // 
            // WBMachine_NM
            // 
            this.WBMachine_NM.DataPropertyName = "WBMachine_NM";
            resources.ApplyResources(this.WBMachine_NM, "WBMachine_NM");
            this.WBMachine_NM.Name = "WBMachine_NM";
            // 
            // Defect_NM
            // 
            this.Defect_NM.DataPropertyName = "Defect_NM";
            resources.ApplyResources(this.Defect_NM, "Defect_NM");
            this.Defect_NM.Name = "Defect_NM";
            this.Defect_NM.ReadOnly = true;
            // 
            // Inspection_NM
            // 
            this.Inspection_NM.DataPropertyName = "Inspection_NM";
            resources.ApplyResources(this.Inspection_NM, "Inspection_NM");
            this.Inspection_NM.Name = "Inspection_NM";
            this.Inspection_NM.ReadOnly = true;
            // 
            // Confirm_NM
            // 
            this.Confirm_NM.DataPropertyName = "Confirm_NM";
            resources.ApplyResources(this.Confirm_NM, "Confirm_NM");
            this.Confirm_NM.Name = "Confirm_NM";
            // 
            // Measure_DT
            // 
            this.Measure_DT.DataPropertyName = "Measure_DT";
            resources.ApplyResources(this.Measure_DT, "Measure_DT");
            this.Measure_DT.Name = "Measure_DT";
            this.Measure_DT.ReadOnly = true;
            // 
            // Equipment_NO
            // 
            this.Equipment_NO.DataPropertyName = "Equipment_NO";
            resources.ApplyResources(this.Equipment_NO, "Equipment_NO");
            this.Equipment_NO.Name = "Equipment_NO";
            this.Equipment_NO.ReadOnly = true;
            // 
            // Timing_NM
            // 
            this.Timing_NM.DataPropertyName = "Timing_NM";
            resources.ApplyResources(this.Timing_NM, "Timing_NM");
            this.Timing_NM.Name = "Timing_NM";
            this.Timing_NM.ReadOnly = true;
            // 
            // Inline_NM
            // 
            this.Inline_NM.DataPropertyName = "Inline_NM";
            resources.ApplyResources(this.Inline_NM, "Inline_NM");
            this.Inline_NM.Name = "Inline_NM";
            // 
            // Message
            // 
            this.Message.DataPropertyName = "Message";
            resources.ApplyResources(this.Message, "Message");
            this.Message.Name = "Message";
            this.Message.ReadOnly = true;
            // 
            // BackNum_NO
            // 
            this.BackNum_NO.DataPropertyName = "BackNum_NO";
            resources.ApplyResources(this.BackNum_NO, "BackNum_NO");
            this.BackNum_NO.Name = "BackNum_NO";
            this.BackNum_NO.ReadOnly = true;
            // 
            // LastUpd_DT
            // 
            this.LastUpd_DT.DataPropertyName = "LastUpd_DT";
            resources.ApplyResources(this.LastUpd_DT, "LastUpd_DT");
            this.LastUpd_DT.Name = "LastUpd_DT";
            this.LastUpd_DT.ReadOnly = true;
            // 
            // QCNR_NO
            // 
            this.QCNR_NO.DataPropertyName = "QCNR_NO";
            resources.ApplyResources(this.QCNR_NO, "QCNR_NO");
            this.QCNR_NO.Name = "QCNR_NO";
            this.QCNR_NO.ReadOnly = true;
            // 
            // Defect_NO
            // 
            this.Defect_NO.DataPropertyName = "Defect_NO";
            resources.ApplyResources(this.Defect_NO, "Defect_NO");
            this.Defect_NO.Name = "Defect_NO";
            this.Defect_NO.ReadOnly = true;
            // 
            // Multi_NO
            // 
            this.Multi_NO.DataPropertyName = "Multi_NO";
            resources.ApplyResources(this.Multi_NO, "Multi_NO");
            this.Multi_NO.Name = "Multi_NO";
            this.Multi_NO.ReadOnly = true;
            // 
            // UpdUser_CD
            // 
            this.UpdUser_CD.DataPropertyName = "UpdUser_CD";
            resources.ApplyResources(this.UpdUser_CD, "UpdUser_CD");
            this.UpdUser_CD.Name = "UpdUser_CD";
            this.UpdUser_CD.ReadOnly = true;
            // 
            // Inspection_NO
            // 
            this.Inspection_NO.DataPropertyName = "Inspection_NO";
            resources.ApplyResources(this.Inspection_NO, "Inspection_NO");
            this.Inspection_NO.Name = "Inspection_NO";
            this.Inspection_NO.ReadOnly = true;
            // 
            // tvQCNRBindingSource
            // 
            this.tvQCNRBindingSource.DataMember = "TvQCNR";
            this.tvQCNRBindingSource.DataSource = this.dsTvQCNR;
            // 
            // dsTvQCNR
            // 
            this.dsTvQCNR.DataSetName = "dsTvQCNR";
            this.dsTvQCNR.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label23
            // 
            resources.ApplyResources(this.label23, "label23");
            this.label23.Name = "label23";
            // 
            // btnUpdate
            // 
            resources.ApplyResources(this.btnUpdate, "btnUpdate");
            this.btnUpdate.Name = "btnUpdate";
            this.btnUpdate.UseVisualStyleBackColor = true;
            this.btnUpdate.Click += new System.EventHandler(this.btnUpdate_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label6);
            this.groupBox1.Controls.Add(this.rbUnConfirmedData);
            this.groupBox1.Controls.Add(this.rbAllData);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // label6
            // 
            resources.ApplyResources(this.label6, "label6");
            this.label6.Name = "label6";
            // 
            // rbUnConfirmedData
            // 
            resources.ApplyResources(this.rbUnConfirmedData, "rbUnConfirmedData");
            this.rbUnConfirmedData.Checked = true;
            this.rbUnConfirmedData.Name = "rbUnConfirmedData";
            this.rbUnConfirmedData.TabStop = true;
            this.rbUnConfirmedData.UseVisualStyleBackColor = true;
            // 
            // rbAllData
            // 
            resources.ApplyResources(this.rbAllData, "rbAllData");
            this.rbAllData.Name = "rbAllData";
            this.rbAllData.UseVisualStyleBackColor = true;
            // 
            // gbGraph
            // 
            this.gbGraph.Controls.Add(this.label22);
            this.gbGraph.Controls.Add(this.txtbEndMinute);
            this.gbGraph.Controls.Add(this.label21);
            this.gbGraph.Controls.Add(this.txtbStartMinute);
            this.gbGraph.Controls.Add(this.label20);
            this.gbGraph.Controls.Add(this.txtbEndHour);
            this.gbGraph.Controls.Add(this.label19);
            this.gbGraph.Controls.Add(this.txtbStartHour);
            this.gbGraph.Controls.Add(this.label15);
            this.gbGraph.Controls.Add(this.label14);
            this.gbGraph.Controls.Add(this.txtbEndDay);
            this.gbGraph.Controls.Add(this.label11);
            this.gbGraph.Controls.Add(this.txtbStartDay);
            this.gbGraph.Controls.Add(this.label12);
            this.gbGraph.Controls.Add(this.label8);
            this.gbGraph.Controls.Add(this.txtbEndYear);
            this.gbGraph.Controls.Add(this.label10);
            this.gbGraph.Controls.Add(this.txtbEndMonth);
            this.gbGraph.Controls.Add(this.txtbStartYear);
            this.gbGraph.Controls.Add(this.label13);
            this.gbGraph.Controls.Add(this.txtbStartMonth);
            this.gbGraph.Controls.Add(this.label9);
            resources.ApplyResources(this.gbGraph, "gbGraph");
            this.gbGraph.Name = "gbGraph";
            this.gbGraph.TabStop = false;
            // 
            // label22
            // 
            resources.ApplyResources(this.label22, "label22");
            this.label22.Name = "label22";
            // 
            // txtbEndMinute
            // 
            resources.ApplyResources(this.txtbEndMinute, "txtbEndMinute");
            this.txtbEndMinute.Name = "txtbEndMinute";
            // 
            // label21
            // 
            resources.ApplyResources(this.label21, "label21");
            this.label21.Name = "label21";
            // 
            // txtbStartMinute
            // 
            resources.ApplyResources(this.txtbStartMinute, "txtbStartMinute");
            this.txtbStartMinute.Name = "txtbStartMinute";
            // 
            // label20
            // 
            resources.ApplyResources(this.label20, "label20");
            this.label20.Name = "label20";
            // 
            // txtbEndHour
            // 
            resources.ApplyResources(this.txtbEndHour, "txtbEndHour");
            this.txtbEndHour.Name = "txtbEndHour";
            // 
            // label19
            // 
            resources.ApplyResources(this.label19, "label19");
            this.label19.Name = "label19";
            // 
            // txtbStartHour
            // 
            resources.ApplyResources(this.txtbStartHour, "txtbStartHour");
            this.txtbStartHour.Name = "txtbStartHour";
            // 
            // label15
            // 
            resources.ApplyResources(this.label15, "label15");
            this.label15.Name = "label15";
            // 
            // label14
            // 
            resources.ApplyResources(this.label14, "label14");
            this.label14.Name = "label14";
            // 
            // txtbEndDay
            // 
            resources.ApplyResources(this.txtbEndDay, "txtbEndDay");
            this.txtbEndDay.Name = "txtbEndDay";
            // 
            // label11
            // 
            resources.ApplyResources(this.label11, "label11");
            this.label11.Name = "label11";
            // 
            // txtbStartDay
            // 
            resources.ApplyResources(this.txtbStartDay, "txtbStartDay");
            this.txtbStartDay.Name = "txtbStartDay";
            // 
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtbEndYear
            // 
            resources.ApplyResources(this.txtbEndYear, "txtbEndYear");
            this.txtbEndYear.Name = "txtbEndYear";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
            // 
            // txtbEndMonth
            // 
            resources.ApplyResources(this.txtbEndMonth, "txtbEndMonth");
            this.txtbEndMonth.Name = "txtbEndMonth";
            // 
            // txtbStartYear
            // 
            resources.ApplyResources(this.txtbStartYear, "txtbStartYear");
            this.txtbStartYear.Name = "txtbStartYear";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // txtbStartMonth
            // 
            resources.ApplyResources(this.txtbStartMonth, "txtbStartMonth");
            this.txtbStartMonth.Name = "txtbStartMonth";
            // 
            // label9
            // 
            resources.ApplyResources(this.label9, "label9");
            this.label9.Name = "label9";
            // 
            // timerED
            // 
            this.timerED.Enabled = true;
            this.timerED.Interval = 3600000;
            this.timerED.Tick += new System.EventHandler(this.timerED_Tick);
            // 
            // btnInlineDisp
            // 
            resources.ApplyResources(this.btnInlineDisp, "btnInlineDisp");
            this.btnInlineDisp.Name = "btnInlineDisp";
            this.btnInlineDisp.UseVisualStyleBackColor = true;
            this.btnInlineDisp.Click += new System.EventHandler(this.btnInlineDisp_Click);
            // 
            // btnReport
            // 
            resources.ApplyResources(this.btnReport, "btnReport");
            this.btnReport.Name = "btnReport";
            this.btnReport.UseVisualStyleBackColor = true;
            this.btnReport.Click += new System.EventHandler(this.btnReport_Click);
            // 
            // btnMasterSetting
            // 
            resources.ApplyResources(this.btnMasterSetting, "btnMasterSetting");
            this.btnMasterSetting.Name = "btnMasterSetting";
            this.btnMasterSetting.UseVisualStyleBackColor = true;
            this.btnMasterSetting.Click += new System.EventHandler(this.btnMasterSetting_Click);
            // 
            // cmbbLineNo
            // 
            this.cmbbLineNo.FormattingEnabled = true;
            resources.ApplyResources(this.cmbbLineNo, "cmbbLineNo");
            this.cmbbLineNo.Name = "cmbbLineNo";
            this.cmbbLineNo.SourceNVC = null;
            this.cmbbLineNo.SelectedIndexChanged += new System.EventHandler(this.cmbbLineNo_SelectedIndexChanged);
            this.cmbbLineNo.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbbLineNo_MouseClick);
            // 
            // txtbGraphPlotNum
            // 
            resources.ApplyResources(this.txtbGraphPlotNum, "txtbGraphPlotNum");
            this.txtbGraphPlotNum.Name = "txtbGraphPlotNum";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // cmbbPlantNM
            // 
            this.cmbbPlantNM.FormattingEnabled = true;
            resources.ApplyResources(this.cmbbPlantNM, "cmbbPlantNM");
            this.cmbbPlantNM.Name = "cmbbPlantNM";
            this.cmbbPlantNM.SourceNVC = null;
            this.cmbbPlantNM.SelectedIndexChanged += new System.EventHandler(this.cmbbPlantNM_SelectedIndexChanged);
            // 
            // label3
            // 
            resources.ApplyResources(this.label3, "label3");
            this.label3.Name = "label3";
            // 
            // cmbbLineCategory
            // 
            this.cmbbLineCategory.FormattingEnabled = true;
            resources.ApplyResources(this.cmbbLineCategory, "cmbbLineCategory");
            this.cmbbLineCategory.Name = "cmbbLineCategory";
            this.cmbbLineCategory.SourceNVC = null;
            this.cmbbLineCategory.SelectedIndexChanged += new System.EventHandler(this.cmbbLineCategory_SelectedIndexChanged);
            this.cmbbLineCategory.MouseClick += new System.Windows.Forms.MouseEventHandler(this.cmbbLineCategory_MouseClick);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolbtnErrorLog,
            this.toolbtnReport,
            this.toolbtnMasterSetting,
            this.toolbtnPhosphorSheetMasterSetting,
            this.toolbtnDefect});
            resources.ApplyResources(this.toolStrip1, "toolStrip1");
            this.toolStrip1.Name = "toolStrip1";
            // 
            // toolbtnErrorLog
            // 
            resources.ApplyResources(this.toolbtnErrorLog, "toolbtnErrorLog");
            this.toolbtnErrorLog.Name = "toolbtnErrorLog";
            this.toolbtnErrorLog.Click += new System.EventHandler(this.toolbtnErrorLog_Click);
            // 
            // toolbtnReport
            // 
            resources.ApplyResources(this.toolbtnReport, "toolbtnReport");
            this.toolbtnReport.Name = "toolbtnReport";
            this.toolbtnReport.Click += new System.EventHandler(this.toolbtnReport_Click);
            // 
            // toolbtnMasterSetting
            // 
            resources.ApplyResources(this.toolbtnMasterSetting, "toolbtnMasterSetting");
            this.toolbtnMasterSetting.Name = "toolbtnMasterSetting";
            this.toolbtnMasterSetting.Click += new System.EventHandler(this.toolbtnMasterSetting_Click);
            // 
            // toolbtnPhosphorSheetMasterSetting
            // 
            resources.ApplyResources(this.toolbtnPhosphorSheetMasterSetting, "toolbtnPhosphorSheetMasterSetting");
            this.toolbtnPhosphorSheetMasterSetting.Name = "toolbtnPhosphorSheetMasterSetting";
            this.toolbtnPhosphorSheetMasterSetting.Click += new System.EventHandler(this.toolbtnPhosphorSheetMasterSetting_Click);
            // 
            // toolbtnDefect
            // 
            resources.ApplyResources(this.toolbtnDefect, "toolbtnDefect");
            this.toolbtnDefect.Name = "toolbtnDefect";
            this.toolbtnDefect.Click += new System.EventHandler(this.toolbtnDefect_Click);
            // 
            // dtpStart
            // 
            resources.ApplyResources(this.dtpStart, "dtpStart");
            this.dtpStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpStart.Name = "dtpStart";
            // 
            // dtpEnd
            // 
            resources.ApplyResources(this.dtpEnd, "dtpEnd");
            this.dtpEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dtpEnd.Name = "dtpEnd";
            // 
            // label4
            // 
            resources.ApplyResources(this.label4, "label4");
            this.label4.Name = "label4";
            // 
            // label5
            // 
            resources.ApplyResources(this.label5, "label5");
            this.label5.Name = "label5";
            // 
            // chkViewDBWBMachine
            // 
            resources.ApplyResources(this.chkViewDBWBMachine, "chkViewDBWBMachine");
            this.chkViewDBWBMachine.Name = "chkViewDBWBMachine";
            this.chkViewDBWBMachine.UseVisualStyleBackColor = true;
            // 
            // F04_ErrorRecord
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.Controls.Add(this.chkViewDBWBMachine);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.dtpEnd);
            this.Controls.Add(this.dtpStart);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.cmbbLineCategory);
            this.Controls.Add(this.cmbbPlantNM);
            this.Controls.Add(this.cmbbLineNo);
            this.Controls.Add(this.btnMasterSetting);
            this.Controls.Add(this.btnReport);
            this.Controls.Add(this.btnInlineDisp);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.gbGraph);
            this.Controls.Add(this.txtbGraphPlotNum);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.btnUpdate);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.label23);
            this.Controls.Add(this.gvQCErrList);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.Name = "F04_ErrorRecord";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvQCErrList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvQCNRBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTvQCNR)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.gbGraph.ResumeLayout(false);
            this.gbGraph.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.Timer timerSD;
        /*private System.Windows.Forms.DataGridViewCheckBoxColumn Check_FG;
        private System.Windows.Forms.DataGridViewTextBoxColumn NascaLot_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn DParameter_VAL;
        private System.Windows.Forms.DataGridViewTextBoxColumn SParameter_VAL;
        private System.Windows.Forms.DataGridViewCheckBoxColumn delFGDataGridViewCheckBoxColumn;*/
        //private NPlot.Windows.PlotSurface2D plotSurface2D2;
        private System.Windows.Forms.DataGridView gvQCErrList;
        private System.Windows.Forms.BindingSource tvQCNRBindingSource;
        private dsTvQCNR dsTvQCNR;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbUnConfirmedData;
        private System.Windows.Forms.RadioButton rbAllData;
        private System.Windows.Forms.GroupBox gbGraph;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtbEndMinute;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtbStartMinute;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtbEndHour;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtbStartHour;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtbEndDay;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtbStartDay;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtbEndYear;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtbEndMonth;
        private System.Windows.Forms.TextBox txtbStartYear;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtbStartMonth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Timer timerED;
        private System.Windows.Forms.Button btnInlineDisp;
        private System.Windows.Forms.Button btnReport;
        private System.Windows.Forms.Button btnMasterSetting;
        private System.Windows.Forms.DataGridViewTextBoxColumn nascaLotNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn typeCDDataGridViewTextBoxColumn;
        private SLCommonLib.UCComboBox cmbbLineNo;
        private System.Windows.Forms.TextBox txtbGraphPlotNum;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
        private SLCommonLib.UCComboBox cmbbPlantNM;
        private System.Windows.Forms.Label label3;
        private SLCommonLib.UCComboBox cmbbLineCategory;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripButton toolbtnMasterSetting;
        private System.Windows.Forms.ToolStripButton toolbtnErrorLog;
        private System.Windows.Forms.ToolStripButton toolbtnReport;
        private System.Windows.Forms.ToolStripButton toolbtnDefect;
        private System.Windows.Forms.DateTimePicker dtpStart;
        private System.Windows.Forms.DateTimePicker dtpEnd;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.DataGridViewButtonColumn 確認;
        private System.Windows.Forms.DataGridViewTextBoxColumn Assets_NM;
        private System.Windows.Forms.DataGridViewButtonColumn LabelCheck_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Check_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Type_CD;
        private System.Windows.Forms.DataGridViewTextBoxColumn NascaLot_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Magazine_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn DBMachine_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn WBMachine_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Defect_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Inspection_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Confirm_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Measure_DT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Equipment_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Timing_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Inline_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message;
        private System.Windows.Forms.DataGridViewTextBoxColumn BackNum_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpd_DT;
        private System.Windows.Forms.DataGridViewTextBoxColumn QCNR_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Defect_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Multi_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn UpdUser_CD;
        private System.Windows.Forms.DataGridViewTextBoxColumn Inspection_NO;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ToolStripButton toolbtnPhosphorSheetMasterSetting;
        private System.Windows.Forms.CheckBox chkViewDBWBMachine;
    }
}

