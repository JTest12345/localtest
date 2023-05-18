namespace GEICS
{
    partial class F05_MachineErrorRecord
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F05_MachineErrorRecord));
            this.gvQCError = new System.Windows.Forms.DataGridView();
            this.Check_FG = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.ConfirmNM = new System.Windows.Forms.DataGridViewButtonColumn();
            this.NascaLot_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Measure_DT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.messageNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TargetConfirm_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.classNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timingNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Manage_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterMAXDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterMINDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterVALDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dParameterVALDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.sParameterVALDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.assetsNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.machinSeqNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Inline_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastUpd_DT = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Inline_CD = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Equipment_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Seq_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.QcParam_NO = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.tvQCILBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsQCIL = new GEICS.dsQCIL();
            this.label8 = new System.Windows.Forms.Label();
            this.txtbStartYear = new System.Windows.Forms.TextBox();
            this.txtbStartMonth = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtbStartDay = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.gbGraph = new System.Windows.Forms.GroupBox();
            this.label22 = new System.Windows.Forms.Label();
            this.txtbEndMinute = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtbStartMinute = new System.Windows.Forms.TextBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtbEndHour = new System.Windows.Forms.TextBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtbStartHour = new System.Windows.Forms.TextBox();
            this.btnRedraw = new System.Windows.Forms.Button();
            this.label15 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.txtbEndDay = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtbEndYear = new System.Windows.Forms.TextBox();
            this.txtbEndMonth = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.timer = new System.Windows.Forms.Timer(this.components);
            this.rbAllData = new System.Windows.Forms.RadioButton();
            this.rbUnConfirmedData = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.rbNoneData = new System.Windows.Forms.RadioButton();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.label18 = new System.Windows.Forms.Label();
            this.label24 = new System.Windows.Forms.Label();
            this.label25 = new System.Windows.Forms.Label();
            this.chklbLine = new System.Windows.Forms.CheckedListBox();
            this.chklbEqui = new System.Windows.Forms.CheckedListBox();
            this.chklbType = new System.Windows.Forms.CheckedListBox();
            this.設備名 = new System.Windows.Forms.Label();
            this.chklbAssetsNM = new System.Windows.Forms.CheckedListBox();
            this.btnLineAllSelect = new System.Windows.Forms.Button();
            this.btnAssetsAllSelect = new System.Windows.Forms.Button();
            this.btnEquiAllSelect = new System.Windows.Forms.Button();
            this.btnTypeAllSelect = new System.Windows.Forms.Button();
            this.btnSetAssets = new System.Windows.Forms.Button();
            this.btnSetEqui = new System.Windows.Forms.Button();
            this.btnSetType = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.gvQCError)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvQCILBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQCIL)).BeginInit();
            this.gbGraph.SuspendLayout();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // gvQCError
            // 
            this.gvQCError.AllowUserToAddRows = false;
            this.gvQCError.AllowUserToDeleteRows = false;
            this.gvQCError.AutoGenerateColumns = false;
            this.gvQCError.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.gvQCError.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvQCError.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Check_FG,
            this.ConfirmNM,
            this.NascaLot_NO,
            this.Measure_DT,
            this.messageNMDataGridViewTextBoxColumn,
            this.parameterNMDataGridViewTextBoxColumn,
            this.TargetConfirm_NM,
            this.classNMDataGridViewTextBoxColumn,
            this.timingNMDataGridViewTextBoxColumn,
            this.Manage_NM,
            this.parameterMAXDataGridViewTextBoxColumn,
            this.parameterMINDataGridViewTextBoxColumn,
            this.parameterVALDataGridViewTextBoxColumn,
            this.dParameterVALDataGridViewTextBoxColumn,
            this.sParameterVALDataGridViewTextBoxColumn,
            this.materialCDDataGridViewTextBoxColumn,
            this.assetsNMDataGridViewTextBoxColumn,
            this.machinSeqNODataGridViewTextBoxColumn,
            this.Inline_NM,
            this.LastUpd_DT,
            this.Inline_CD,
            this.Equipment_NO,
            this.Seq_NO,
            this.QcParam_NO});
            this.gvQCError.DataSource = this.tvQCILBindingSource;
            resources.ApplyResources(this.gvQCError, "gvQCError");
            this.gvQCError.Name = "gvQCError";
            this.gvQCError.RowTemplate.Height = 21;
            this.gvQCError.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvQCError_CellValueChanged);
            this.gvQCError.CellFormatting += new System.Windows.Forms.DataGridViewCellFormattingEventHandler(this.gvQCError_CellFormatting);
            this.gvQCError.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gvQCError_CellContentClick);
            // 
            // Check_FG
            // 
            this.Check_FG.DataPropertyName = "Check_FG";
            this.Check_FG.FalseValue = "False";
            resources.ApplyResources(this.Check_FG, "Check_FG");
            this.Check_FG.Name = "Check_FG";
            this.Check_FG.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Check_FG.TrueValue = "True";
            // 
            // ConfirmNM
            // 
            this.ConfirmNM.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            resources.ApplyResources(this.ConfirmNM, "ConfirmNM");
            this.ConfirmNM.Name = "ConfirmNM";
            this.ConfirmNM.Text = "入力";
            this.ConfirmNM.UseColumnTextForButtonValue = true;
            // 
            // NascaLot_NO
            // 
            this.NascaLot_NO.DataPropertyName = "NascaLot_NO";
            resources.ApplyResources(this.NascaLot_NO, "NascaLot_NO");
            this.NascaLot_NO.Name = "NascaLot_NO";
            this.NascaLot_NO.ReadOnly = true;
            // 
            // Measure_DT
            // 
            this.Measure_DT.DataPropertyName = "Measure_DT";
            resources.ApplyResources(this.Measure_DT, "Measure_DT");
            this.Measure_DT.Name = "Measure_DT";
            this.Measure_DT.ReadOnly = true;
            // 
            // messageNMDataGridViewTextBoxColumn
            // 
            this.messageNMDataGridViewTextBoxColumn.DataPropertyName = "Message_NM";
            resources.ApplyResources(this.messageNMDataGridViewTextBoxColumn, "messageNMDataGridViewTextBoxColumn");
            this.messageNMDataGridViewTextBoxColumn.Name = "messageNMDataGridViewTextBoxColumn";
            this.messageNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // parameterNMDataGridViewTextBoxColumn
            // 
            this.parameterNMDataGridViewTextBoxColumn.DataPropertyName = "Parameter_NM";
            resources.ApplyResources(this.parameterNMDataGridViewTextBoxColumn, "parameterNMDataGridViewTextBoxColumn");
            this.parameterNMDataGridViewTextBoxColumn.Name = "parameterNMDataGridViewTextBoxColumn";
            this.parameterNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // TargetConfirm_NM
            // 
            this.TargetConfirm_NM.DataPropertyName = "TargetConfirm_NM";
            resources.ApplyResources(this.TargetConfirm_NM, "TargetConfirm_NM");
            this.TargetConfirm_NM.Name = "TargetConfirm_NM";
            // 
            // classNMDataGridViewTextBoxColumn
            // 
            this.classNMDataGridViewTextBoxColumn.DataPropertyName = "Class_NM";
            resources.ApplyResources(this.classNMDataGridViewTextBoxColumn, "classNMDataGridViewTextBoxColumn");
            this.classNMDataGridViewTextBoxColumn.Name = "classNMDataGridViewTextBoxColumn";
            this.classNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // timingNMDataGridViewTextBoxColumn
            // 
            this.timingNMDataGridViewTextBoxColumn.DataPropertyName = "Timing_NM";
            resources.ApplyResources(this.timingNMDataGridViewTextBoxColumn, "timingNMDataGridViewTextBoxColumn");
            this.timingNMDataGridViewTextBoxColumn.Name = "timingNMDataGridViewTextBoxColumn";
            this.timingNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Manage_NM
            // 
            this.Manage_NM.DataPropertyName = "Manage_NM";
            resources.ApplyResources(this.Manage_NM, "Manage_NM");
            this.Manage_NM.Name = "Manage_NM";
            this.Manage_NM.ReadOnly = true;
            // 
            // parameterMAXDataGridViewTextBoxColumn
            // 
            this.parameterMAXDataGridViewTextBoxColumn.DataPropertyName = "Parameter_MAX";
            resources.ApplyResources(this.parameterMAXDataGridViewTextBoxColumn, "parameterMAXDataGridViewTextBoxColumn");
            this.parameterMAXDataGridViewTextBoxColumn.Name = "parameterMAXDataGridViewTextBoxColumn";
            this.parameterMAXDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // parameterMINDataGridViewTextBoxColumn
            // 
            this.parameterMINDataGridViewTextBoxColumn.DataPropertyName = "Parameter_MIN";
            resources.ApplyResources(this.parameterMINDataGridViewTextBoxColumn, "parameterMINDataGridViewTextBoxColumn");
            this.parameterMINDataGridViewTextBoxColumn.Name = "parameterMINDataGridViewTextBoxColumn";
            this.parameterMINDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // parameterVALDataGridViewTextBoxColumn
            // 
            this.parameterVALDataGridViewTextBoxColumn.DataPropertyName = "Parameter_VAL";
            resources.ApplyResources(this.parameterVALDataGridViewTextBoxColumn, "parameterVALDataGridViewTextBoxColumn");
            this.parameterVALDataGridViewTextBoxColumn.Name = "parameterVALDataGridViewTextBoxColumn";
            this.parameterVALDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // dParameterVALDataGridViewTextBoxColumn
            // 
            this.dParameterVALDataGridViewTextBoxColumn.DataPropertyName = "DParameter_VAL";
            resources.ApplyResources(this.dParameterVALDataGridViewTextBoxColumn, "dParameterVALDataGridViewTextBoxColumn");
            this.dParameterVALDataGridViewTextBoxColumn.Name = "dParameterVALDataGridViewTextBoxColumn";
            this.dParameterVALDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // sParameterVALDataGridViewTextBoxColumn
            // 
            this.sParameterVALDataGridViewTextBoxColumn.DataPropertyName = "SParameter_VAL";
            resources.ApplyResources(this.sParameterVALDataGridViewTextBoxColumn, "sParameterVALDataGridViewTextBoxColumn");
            this.sParameterVALDataGridViewTextBoxColumn.Name = "sParameterVALDataGridViewTextBoxColumn";
            this.sParameterVALDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // materialCDDataGridViewTextBoxColumn
            // 
            this.materialCDDataGridViewTextBoxColumn.DataPropertyName = "Material_CD";
            resources.ApplyResources(this.materialCDDataGridViewTextBoxColumn, "materialCDDataGridViewTextBoxColumn");
            this.materialCDDataGridViewTextBoxColumn.Name = "materialCDDataGridViewTextBoxColumn";
            this.materialCDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // assetsNMDataGridViewTextBoxColumn
            // 
            this.assetsNMDataGridViewTextBoxColumn.DataPropertyName = "Assets_NM";
            resources.ApplyResources(this.assetsNMDataGridViewTextBoxColumn, "assetsNMDataGridViewTextBoxColumn");
            this.assetsNMDataGridViewTextBoxColumn.Name = "assetsNMDataGridViewTextBoxColumn";
            this.assetsNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // machinSeqNODataGridViewTextBoxColumn
            // 
            this.machinSeqNODataGridViewTextBoxColumn.DataPropertyName = "MachinSeq_NO";
            resources.ApplyResources(this.machinSeqNODataGridViewTextBoxColumn, "machinSeqNODataGridViewTextBoxColumn");
            this.machinSeqNODataGridViewTextBoxColumn.Name = "machinSeqNODataGridViewTextBoxColumn";
            this.machinSeqNODataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Inline_NM
            // 
            this.Inline_NM.DataPropertyName = "Inline_NM";
            resources.ApplyResources(this.Inline_NM, "Inline_NM");
            this.Inline_NM.Name = "Inline_NM";
            // 
            // LastUpd_DT
            // 
            this.LastUpd_DT.DataPropertyName = "LastUpd_DT";
            resources.ApplyResources(this.LastUpd_DT, "LastUpd_DT");
            this.LastUpd_DT.Name = "LastUpd_DT";
            this.LastUpd_DT.ReadOnly = true;
            // 
            // Inline_CD
            // 
            this.Inline_CD.DataPropertyName = "Inline_CD";
            resources.ApplyResources(this.Inline_CD, "Inline_CD");
            this.Inline_CD.Name = "Inline_CD";
            // 
            // Equipment_NO
            // 
            this.Equipment_NO.DataPropertyName = "Equipment_NO";
            resources.ApplyResources(this.Equipment_NO, "Equipment_NO");
            this.Equipment_NO.Name = "Equipment_NO";
            // 
            // Seq_NO
            // 
            this.Seq_NO.DataPropertyName = "Seq_NO";
            resources.ApplyResources(this.Seq_NO, "Seq_NO");
            this.Seq_NO.Name = "Seq_NO";
            // 
            // QcParam_NO
            // 
            this.QcParam_NO.DataPropertyName = "QcParam_NO";
            resources.ApplyResources(this.QcParam_NO, "QcParam_NO");
            this.QcParam_NO.Name = "QcParam_NO";
            // 
            // tvQCILBindingSource
            // 
            this.tvQCILBindingSource.DataMember = "TvQCIL";
            this.tvQCILBindingSource.DataSource = this.dsQCIL;
            // 
            // dsQCIL
            // 
            this.dsQCIL.DataSetName = "dsQCIL";
            this.dsQCIL.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // label8
            // 
            resources.ApplyResources(this.label8, "label8");
            this.label8.Name = "label8";
            // 
            // txtbStartYear
            // 
            resources.ApplyResources(this.txtbStartYear, "txtbStartYear");
            this.txtbStartYear.Name = "txtbStartYear";
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
            // txtbStartDay
            // 
            resources.ApplyResources(this.txtbStartDay, "txtbStartDay");
            this.txtbStartDay.Name = "txtbStartDay";
            // 
            // label10
            // 
            resources.ApplyResources(this.label10, "label10");
            this.label10.Name = "label10";
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
            this.gbGraph.Controls.Add(this.btnRedraw);
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
            // btnRedraw
            // 
            resources.ApplyResources(this.btnRedraw, "btnRedraw");
            this.btnRedraw.Name = "btnRedraw";
            this.btnRedraw.UseVisualStyleBackColor = true;
            this.btnRedraw.Click += new System.EventHandler(this.btnRedraw_Click);
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
            // label12
            // 
            resources.ApplyResources(this.label12, "label12");
            this.label12.Name = "label12";
            // 
            // txtbEndYear
            // 
            resources.ApplyResources(this.txtbEndYear, "txtbEndYear");
            this.txtbEndYear.Name = "txtbEndYear";
            // 
            // txtbEndMonth
            // 
            resources.ApplyResources(this.txtbEndMonth, "txtbEndMonth");
            this.txtbEndMonth.Name = "txtbEndMonth";
            // 
            // label13
            // 
            resources.ApplyResources(this.label13, "label13");
            this.label13.Name = "label13";
            // 
            // timer
            // 
            this.timer.Enabled = true;
            this.timer.Interval = 60000;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
            // 
            // rbAllData
            // 
            resources.ApplyResources(this.rbAllData, "rbAllData");
            this.rbAllData.Name = "rbAllData";
            this.rbAllData.TabStop = true;
            this.rbAllData.UseVisualStyleBackColor = true;
            // 
            // rbUnConfirmedData
            // 
            resources.ApplyResources(this.rbUnConfirmedData, "rbUnConfirmedData");
            this.rbUnConfirmedData.Name = "rbUnConfirmedData";
            this.rbUnConfirmedData.TabStop = true;
            this.rbUnConfirmedData.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.rbNoneData);
            this.groupBox1.Controls.Add(this.rbUnConfirmedData);
            this.groupBox1.Controls.Add(this.rbAllData);
            resources.ApplyResources(this.groupBox1, "groupBox1");
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.TabStop = false;
            // 
            // rbNoneData
            // 
            resources.ApplyResources(this.rbNoneData, "rbNoneData");
            this.rbNoneData.Name = "rbNoneData";
            this.rbNoneData.TabStop = true;
            this.rbNoneData.UseVisualStyleBackColor = true;
            // 
            // timer1
            // 
            this.timer1.Enabled = true;
            this.timer1.Interval = 3600000;
            this.timer1.Tick += new System.EventHandler(this.timer1_Tick);
            // 
            // label18
            // 
            resources.ApplyResources(this.label18, "label18");
            this.label18.Name = "label18";
            // 
            // label24
            // 
            resources.ApplyResources(this.label24, "label24");
            this.label24.Name = "label24";
            // 
            // label25
            // 
            resources.ApplyResources(this.label25, "label25");
            this.label25.Name = "label25";
            // 
            // chklbLine
            // 
            this.chklbLine.FormattingEnabled = true;
            resources.ApplyResources(this.chklbLine, "chklbLine");
            this.chklbLine.Name = "chklbLine";
            // 
            // chklbEqui
            // 
            this.chklbEqui.FormattingEnabled = true;
            resources.ApplyResources(this.chklbEqui, "chklbEqui");
            this.chklbEqui.Name = "chklbEqui";
            // 
            // chklbType
            // 
            this.chklbType.FormattingEnabled = true;
            resources.ApplyResources(this.chklbType, "chklbType");
            this.chklbType.Name = "chklbType";
            // 
            // 設備名
            // 
            resources.ApplyResources(this.設備名, "設備名");
            this.設備名.Name = "設備名";
            // 
            // chklbAssetsNM
            // 
            this.chklbAssetsNM.FormattingEnabled = true;
            resources.ApplyResources(this.chklbAssetsNM, "chklbAssetsNM");
            this.chklbAssetsNM.Name = "chklbAssetsNM";
            // 
            // btnLineAllSelect
            // 
            resources.ApplyResources(this.btnLineAllSelect, "btnLineAllSelect");
            this.btnLineAllSelect.Name = "btnLineAllSelect";
            this.btnLineAllSelect.UseVisualStyleBackColor = true;
            this.btnLineAllSelect.Click += new System.EventHandler(this.btnLineAllSelect_Click);
            // 
            // btnAssetsAllSelect
            // 
            resources.ApplyResources(this.btnAssetsAllSelect, "btnAssetsAllSelect");
            this.btnAssetsAllSelect.Name = "btnAssetsAllSelect";
            this.btnAssetsAllSelect.UseVisualStyleBackColor = true;
            this.btnAssetsAllSelect.Click += new System.EventHandler(this.btnAssetsAllSelect_Click);
            // 
            // btnEquiAllSelect
            // 
            resources.ApplyResources(this.btnEquiAllSelect, "btnEquiAllSelect");
            this.btnEquiAllSelect.Name = "btnEquiAllSelect";
            this.btnEquiAllSelect.UseVisualStyleBackColor = true;
            this.btnEquiAllSelect.Click += new System.EventHandler(this.btnEquiAllSelect_Click);
            // 
            // btnTypeAllSelect
            // 
            resources.ApplyResources(this.btnTypeAllSelect, "btnTypeAllSelect");
            this.btnTypeAllSelect.Name = "btnTypeAllSelect";
            this.btnTypeAllSelect.UseVisualStyleBackColor = true;
            this.btnTypeAllSelect.Click += new System.EventHandler(this.btnTypeAllSelect_Click);
            // 
            // btnSetAssets
            // 
            resources.ApplyResources(this.btnSetAssets, "btnSetAssets");
            this.btnSetAssets.Name = "btnSetAssets";
            this.btnSetAssets.UseVisualStyleBackColor = true;
            this.btnSetAssets.Click += new System.EventHandler(this.btnSetAssets_Click);
            // 
            // btnSetEqui
            // 
            resources.ApplyResources(this.btnSetEqui, "btnSetEqui");
            this.btnSetEqui.Name = "btnSetEqui";
            this.btnSetEqui.UseVisualStyleBackColor = true;
            this.btnSetEqui.Click += new System.EventHandler(this.btnSetEqui_Click);
            // 
            // btnSetType
            // 
            resources.ApplyResources(this.btnSetType, "btnSetType");
            this.btnSetType.Name = "btnSetType";
            this.btnSetType.UseVisualStyleBackColor = true;
            this.btnSetType.Click += new System.EventHandler(this.btnSetType_Click);
            // 
            // F05_MachineErrorRecord
            // 
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CausesValidation = false;
            this.Controls.Add(this.chklbType);
            this.Controls.Add(this.chklbAssetsNM);
            this.Controls.Add(this.chklbEqui);
            this.Controls.Add(this.chklbLine);
            this.Controls.Add(this.設備名);
            this.Controls.Add(this.label25);
            this.Controls.Add(this.label24);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnTypeAllSelect);
            this.Controls.Add(this.btnEquiAllSelect);
            this.Controls.Add(this.btnAssetsAllSelect);
            this.Controls.Add(this.btnSetType);
            this.Controls.Add(this.btnSetEqui);
            this.Controls.Add(this.btnSetAssets);
            this.Controls.Add(this.btnLineAllSelect);
            this.Controls.Add(this.label18);
            this.Controls.Add(this.gbGraph);
            this.Controls.Add(this.gvQCError);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Fixed3D;
            this.MaximizeBox = false;
            this.Name = "F05_MachineErrorRecord";
            this.Load += new System.EventHandler(this.Form2_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvQCError)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvQCILBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsQCIL)).EndInit();
            this.gbGraph.ResumeLayout(false);
            this.gbGraph.PerformLayout();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }
        
        #endregion

        private System.Windows.Forms.DataGridView gvQCError;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox txtbStartYear;
        private System.Windows.Forms.TextBox txtbStartMonth;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtbStartDay;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.GroupBox gbGraph;
        private System.Windows.Forms.TextBox txtbEndDay;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtbEndYear;
        private System.Windows.Forms.TextBox txtbEndMonth;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button btnRedraw;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Timer timer;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtbEndHour;
        private System.Windows.Forms.TextBox txtbStartHour;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtbStartMinute;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtbEndMinute;
        /*private System.Windows.Forms.DataGridViewCheckBoxColumn Check_FG;
        private System.Windows.Forms.DataGridViewTextBoxColumn NascaLot_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Message_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn DParameter_VAL;
        private System.Windows.Forms.DataGridViewTextBoxColumn SParameter_VAL;
        private System.Windows.Forms.DataGridViewCheckBoxColumn delFGDataGridViewCheckBoxColumn;*/
        //private NPlot.Windows.PlotSurface2D plotSurface2D2;
        private System.Windows.Forms.BindingSource tvQCILBindingSource;
        private dsQCIL dsQCIL;
        private System.Windows.Forms.RadioButton rbUnConfirmedData;
        private System.Windows.Forms.RadioButton rbAllData;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.RadioButton rbNoneData;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.Label label24;
        private System.Windows.Forms.Label label25;
        private System.Windows.Forms.CheckedListBox chklbLine;
        private System.Windows.Forms.CheckedListBox chklbEqui;
        private System.Windows.Forms.CheckedListBox chklbType;
        private System.Windows.Forms.Label 設備名;
        private System.Windows.Forms.CheckedListBox chklbAssetsNM;
        private System.Windows.Forms.Button btnLineAllSelect;
        private System.Windows.Forms.Button btnAssetsAllSelect;
        private System.Windows.Forms.Button btnEquiAllSelect;
        private System.Windows.Forms.Button btnTypeAllSelect;
        private System.Windows.Forms.Button btnSetAssets;
        private System.Windows.Forms.Button btnSetEqui;
        private System.Windows.Forms.Button btnSetType;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Check_FG;
        private System.Windows.Forms.DataGridViewButtonColumn ConfirmNM;
        private System.Windows.Forms.DataGridViewTextBoxColumn NascaLot_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Measure_DT;
        private System.Windows.Forms.DataGridViewTextBoxColumn messageNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TargetConfirm_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn classNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn timingNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Manage_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterMAXDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterMINDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterVALDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dParameterVALDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn sParameterVALDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn assetsNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn machinSeqNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Inline_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn LastUpd_DT;
        private System.Windows.Forms.DataGridViewTextBoxColumn Inline_CD;
        private System.Windows.Forms.DataGridViewTextBoxColumn Equipment_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn Seq_NO;
        private System.Windows.Forms.DataGridViewTextBoxColumn QcParam_NO;
    }
}

