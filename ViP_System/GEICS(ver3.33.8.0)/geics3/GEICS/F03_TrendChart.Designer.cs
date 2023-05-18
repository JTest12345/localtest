namespace GEICS
{
    using System.Windows.Forms.DataVisualization.Charting;
    using System.Drawing;
    partial class F03_TrendChart
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(F03_TrendChart));
			System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
			System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
			this.splitContainer2 = new System.Windows.Forms.SplitContainer();
			this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
			this.btnCatalog = new System.Windows.Forms.Button();
			this.splitContainer1 = new System.Windows.Forms.SplitContainer();
			this.grdLotStb = new System.Windows.Forms.DataGridView();
			this.dgvNascaList = new System.Windows.Forms.DataGridView();
			this.lotNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MateGroup_JA = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mtralitemcdDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.materialjaDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mateLotNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.mateChangeDTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.materialCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.materialcd2DataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.compltdtDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.plantCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.MixResult_ID = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.dsMaterialBindingSource = new System.Windows.Forms.BindingSource(this.components);
			this.dsMaterial = new GEICS.dsMaterial();
			this.gbEquipment = new System.Windows.Forms.GroupBox();
			this.clbMachineList = new System.Windows.Forms.CheckedListBox();
			this.cmbbAssetsNM = new System.Windows.Forms.ComboBox();
			this.label5 = new System.Windows.Forms.Label();
			this.label23 = new System.Windows.Forms.Label();
			this.cmbbLineNo = new System.Windows.Forms.ComboBox();
			this.label7 = new System.Windows.Forms.Label();
			this.btnAllGraph = new System.Windows.Forms.Button();
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
			this.lblresult = new System.Windows.Forms.Label();
			this.cbAnalysis = new System.Windows.Forms.CheckBox();
			this.gbQc = new System.Windows.Forms.GroupBox();
			this.cmbbQcParamNM = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			this.cmbbDefectNM = new System.Windows.Forms.ComboBox();
			this.lblDefectNM = new System.Windows.Forms.Label();
			this.txtbProcessNO = new System.Windows.Forms.TextBox();
			this.txtbInspectionNO = new System.Windows.Forms.TextBox();
			this.lblNinja = new System.Windows.Forms.Label();
			this.gbCompornent = new System.Windows.Forms.GroupBox();
			this.cmbbType = new System.Windows.Forms.ComboBox();
			this.label17 = new System.Windows.Forms.Label();
			this.btnNext = new System.Windows.Forms.Button();
			this.lblDefectNO = new System.Windows.Forms.Label();
			this.btnBack = new System.Windows.Forms.Button();
			this.btnRedraw = new System.Windows.Forms.Button();
			this.lblLotNO = new System.Windows.Forms.Label();
			this.splitContainer2.Panel1.SuspendLayout();
			this.splitContainer2.Panel2.SuspendLayout();
			this.splitContainer2.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
			this.splitContainer1.Panel1.SuspendLayout();
			this.splitContainer1.Panel2.SuspendLayout();
			this.splitContainer1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.grdLotStb)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvNascaList)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsMaterialBindingSource)).BeginInit();
			((System.ComponentModel.ISupportInitialize)(this.dsMaterial)).BeginInit();
			this.gbEquipment.SuspendLayout();
			this.gbGraph.SuspendLayout();
			this.gbQc.SuspendLayout();
			this.gbCompornent.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitContainer2
			// 
			resources.ApplyResources(this.splitContainer2, "splitContainer2");
			this.splitContainer2.Name = "splitContainer2";
			// 
			// splitContainer2.Panel1
			// 
			this.splitContainer2.Panel1.Controls.Add(this.chart1);
			// 
			// splitContainer2.Panel2
			// 
			resources.ApplyResources(this.splitContainer2.Panel2, "splitContainer2.Panel2");
			this.splitContainer2.Panel2.Controls.Add(this.btnCatalog);
			this.splitContainer2.Panel2.Controls.Add(this.splitContainer1);
			this.splitContainer2.Panel2.Controls.Add(this.gbEquipment);
			this.splitContainer2.Panel2.Controls.Add(this.btnAllGraph);
			this.splitContainer2.Panel2.Controls.Add(this.gbGraph);
			this.splitContainer2.Panel2.Controls.Add(this.lblresult);
			this.splitContainer2.Panel2.Controls.Add(this.cbAnalysis);
			this.splitContainer2.Panel2.Controls.Add(this.gbQc);
			this.splitContainer2.Panel2.Controls.Add(this.lblNinja);
			this.splitContainer2.Panel2.Controls.Add(this.gbCompornent);
			this.splitContainer2.Panel2.Controls.Add(this.btnNext);
			this.splitContainer2.Panel2.Controls.Add(this.lblDefectNO);
			this.splitContainer2.Panel2.Controls.Add(this.btnBack);
			this.splitContainer2.Panel2.Controls.Add(this.btnRedraw);
			this.splitContainer2.Panel2.Controls.Add(this.lblLotNO);
			// 
			// chart1
			// 
			this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(243)))), ((int)(((byte)(223)))), ((int)(((byte)(193)))));
			this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			this.chart1.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(181)))), ((int)(((byte)(64)))), ((int)(((byte)(1)))));
			this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			this.chart1.BorderlineWidth = 2;
			this.chart1.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
			chartArea1.Area3DStyle.Inclination = 40;
			chartArea1.Area3DStyle.IsClustered = true;
			chartArea1.Area3DStyle.IsRightAngleAxes = false;
			chartArea1.Area3DStyle.LightStyle = System.Windows.Forms.DataVisualization.Charting.LightStyle.Realistic;
			chartArea1.Area3DStyle.Perspective = 9;
			chartArea1.Area3DStyle.Rotation = 25;
			chartArea1.Area3DStyle.WallWidth = 3;
			chartArea1.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			chartArea1.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			chartArea1.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.BackColor = System.Drawing.Color.OldLace;
			chartArea1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
			chartArea1.BackSecondaryColor = System.Drawing.Color.White;
			chartArea1.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
			chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
			chartArea1.CursorX.IsUserEnabled = true;
			chartArea1.CursorX.IsUserSelectionEnabled = true;
			chartArea1.CursorY.IsUserEnabled = true;
			chartArea1.CursorY.IsUserSelectionEnabled = true;
			chartArea1.Name = "GRAPH";
			chartArea1.ShadowColor = System.Drawing.Color.Transparent;
			this.chart1.ChartAreas.Add(chartArea1);
			resources.ApplyResources(this.chart1, "chart1");
			legend1.BackColor = System.Drawing.Color.Transparent;
			legend1.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
			legend1.IsTextAutoFit = false;
			legend1.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Column;
			legend1.MaximumAutoSize = 100F;
			legend1.Name = "Default";
			legend1.TableStyle = System.Windows.Forms.DataVisualization.Charting.LegendTableStyle.Tall;
			this.chart1.Legends.Add(legend1);
			this.chart1.Name = "chart1";
			// 
			// btnCatalog
			// 
			resources.ApplyResources(this.btnCatalog, "btnCatalog");
			this.btnCatalog.Name = "btnCatalog";
			this.btnCatalog.UseVisualStyleBackColor = true;
			this.btnCatalog.Click += new System.EventHandler(this.btnCatalog_Click);
			// 
			// splitContainer1
			// 
			resources.ApplyResources(this.splitContainer1, "splitContainer1");
			this.splitContainer1.MinimumSize = new System.Drawing.Size(0, 413);
			this.splitContainer1.Name = "splitContainer1";
			// 
			// splitContainer1.Panel1
			// 
			this.splitContainer1.Panel1.Controls.Add(this.grdLotStb);
			// 
			// splitContainer1.Panel2
			// 
			this.splitContainer1.Panel2.Controls.Add(this.dgvNascaList);
			// 
			// grdLotStb
			// 
			this.grdLotStb.AllowUserToAddRows = false;
			this.grdLotStb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			resources.ApplyResources(this.grdLotStb, "grdLotStb");
			this.grdLotStb.Name = "grdLotStb";
			this.grdLotStb.RowTemplate.Height = 21;
			this.grdLotStb.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.grdLotStb_CellMouseClick);
			// 
			// dgvNascaList
			// 
			this.dgvNascaList.AllowUserToAddRows = false;
			this.dgvNascaList.AutoGenerateColumns = false;
			this.dgvNascaList.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.dgvNascaList.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.lotNODataGridViewTextBoxColumn,
            this.MateGroup_JA,
            this.mtralitemcdDataGridViewTextBoxColumn,
            this.materialjaDataGridViewTextBoxColumn,
            this.mateLotNODataGridViewTextBoxColumn,
            this.mateChangeDTDataGridViewTextBoxColumn,
            this.materialCDDataGridViewTextBoxColumn,
            this.materialcd2DataGridViewTextBoxColumn,
            this.compltdtDataGridViewTextBoxColumn,
            this.plantCDDataGridViewTextBoxColumn,
            this.MixResult_ID});
			this.dgvNascaList.DataSource = this.dsMaterialBindingSource;
			resources.ApplyResources(this.dgvNascaList, "dgvNascaList");
			this.dgvNascaList.Name = "dgvNascaList";
			this.dgvNascaList.ReadOnly = true;
			this.dgvNascaList.RowTemplate.Height = 21;
			this.dgvNascaList.CellMouseClick += new System.Windows.Forms.DataGridViewCellMouseEventHandler(this.dgvNascaList_CellMouseClick);
			// 
			// lotNODataGridViewTextBoxColumn
			// 
			this.lotNODataGridViewTextBoxColumn.DataPropertyName = "Lot_NO";
			this.lotNODataGridViewTextBoxColumn.FillWeight = 90F;
			resources.ApplyResources(this.lotNODataGridViewTextBoxColumn, "lotNODataGridViewTextBoxColumn");
			this.lotNODataGridViewTextBoxColumn.Name = "lotNODataGridViewTextBoxColumn";
			this.lotNODataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// MateGroup_JA
			// 
			this.MateGroup_JA.DataPropertyName = "MateGroup_JA";
			resources.ApplyResources(this.MateGroup_JA, "MateGroup_JA");
			this.MateGroup_JA.Name = "MateGroup_JA";
			this.MateGroup_JA.ReadOnly = true;
			// 
			// mtralitemcdDataGridViewTextBoxColumn
			// 
			this.mtralitemcdDataGridViewTextBoxColumn.DataPropertyName = "mtralitem_cd";
			resources.ApplyResources(this.mtralitemcdDataGridViewTextBoxColumn, "mtralitemcdDataGridViewTextBoxColumn");
			this.mtralitemcdDataGridViewTextBoxColumn.Name = "mtralitemcdDataGridViewTextBoxColumn";
			this.mtralitemcdDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// materialjaDataGridViewTextBoxColumn
			// 
			this.materialjaDataGridViewTextBoxColumn.DataPropertyName = "material_ja";
			resources.ApplyResources(this.materialjaDataGridViewTextBoxColumn, "materialjaDataGridViewTextBoxColumn");
			this.materialjaDataGridViewTextBoxColumn.Name = "materialjaDataGridViewTextBoxColumn";
			this.materialjaDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// mateLotNODataGridViewTextBoxColumn
			// 
			this.mateLotNODataGridViewTextBoxColumn.DataPropertyName = "MateLot_NO";
			resources.ApplyResources(this.mateLotNODataGridViewTextBoxColumn, "mateLotNODataGridViewTextBoxColumn");
			this.mateLotNODataGridViewTextBoxColumn.Name = "mateLotNODataGridViewTextBoxColumn";
			this.mateLotNODataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// mateChangeDTDataGridViewTextBoxColumn
			// 
			this.mateChangeDTDataGridViewTextBoxColumn.DataPropertyName = "MateChange_DT";
			resources.ApplyResources(this.mateChangeDTDataGridViewTextBoxColumn, "mateChangeDTDataGridViewTextBoxColumn");
			this.mateChangeDTDataGridViewTextBoxColumn.Name = "mateChangeDTDataGridViewTextBoxColumn";
			this.mateChangeDTDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// materialCDDataGridViewTextBoxColumn
			// 
			this.materialCDDataGridViewTextBoxColumn.DataPropertyName = "Material_CD";
			resources.ApplyResources(this.materialCDDataGridViewTextBoxColumn, "materialCDDataGridViewTextBoxColumn");
			this.materialCDDataGridViewTextBoxColumn.Name = "materialCDDataGridViewTextBoxColumn";
			this.materialCDDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// materialcd2DataGridViewTextBoxColumn
			// 
			this.materialcd2DataGridViewTextBoxColumn.DataPropertyName = "material_cd2";
			resources.ApplyResources(this.materialcd2DataGridViewTextBoxColumn, "materialcd2DataGridViewTextBoxColumn");
			this.materialcd2DataGridViewTextBoxColumn.Name = "materialcd2DataGridViewTextBoxColumn";
			this.materialcd2DataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// compltdtDataGridViewTextBoxColumn
			// 
			this.compltdtDataGridViewTextBoxColumn.DataPropertyName = "complt_dt";
			resources.ApplyResources(this.compltdtDataGridViewTextBoxColumn, "compltdtDataGridViewTextBoxColumn");
			this.compltdtDataGridViewTextBoxColumn.Name = "compltdtDataGridViewTextBoxColumn";
			this.compltdtDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// plantCDDataGridViewTextBoxColumn
			// 
			this.plantCDDataGridViewTextBoxColumn.DataPropertyName = "Plant_CD";
			resources.ApplyResources(this.plantCDDataGridViewTextBoxColumn, "plantCDDataGridViewTextBoxColumn");
			this.plantCDDataGridViewTextBoxColumn.Name = "plantCDDataGridViewTextBoxColumn";
			this.plantCDDataGridViewTextBoxColumn.ReadOnly = true;
			// 
			// MixResult_ID
			// 
			this.MixResult_ID.DataPropertyName = "MixResult_ID";
			resources.ApplyResources(this.MixResult_ID, "MixResult_ID");
			this.MixResult_ID.Name = "MixResult_ID";
			this.MixResult_ID.ReadOnly = true;
			// 
			// dsMaterialBindingSource
			// 
			this.dsMaterialBindingSource.DataMember = "dTblMaterial";
			this.dsMaterialBindingSource.DataSource = this.dsMaterial;
			// 
			// dsMaterial
			// 
			this.dsMaterial.DataSetName = "dsMaterial";
			this.dsMaterial.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
			// 
			// gbEquipment
			// 
			resources.ApplyResources(this.gbEquipment, "gbEquipment");
			this.gbEquipment.Controls.Add(this.clbMachineList);
			this.gbEquipment.Controls.Add(this.cmbbAssetsNM);
			this.gbEquipment.Controls.Add(this.label5);
			this.gbEquipment.Controls.Add(this.label23);
			this.gbEquipment.Controls.Add(this.cmbbLineNo);
			this.gbEquipment.Controls.Add(this.label7);
			this.gbEquipment.Name = "gbEquipment";
			this.gbEquipment.TabStop = false;
			// 
			// clbMachineList
			// 
			this.clbMachineList.FormattingEnabled = true;
			resources.ApplyResources(this.clbMachineList, "clbMachineList");
			this.clbMachineList.Name = "clbMachineList";
			// 
			// cmbbAssetsNM
			// 
			this.cmbbAssetsNM.FormattingEnabled = true;
			resources.ApplyResources(this.cmbbAssetsNM, "cmbbAssetsNM");
			this.cmbbAssetsNM.Name = "cmbbAssetsNM";
			this.cmbbAssetsNM.SelectedIndexChanged += new System.EventHandler(this.cmbbAssetsNM_SelectedIndexChanged);
			// 
			// label5
			// 
			resources.ApplyResources(this.label5, "label5");
			this.label5.Name = "label5";
			// 
			// label23
			// 
			resources.ApplyResources(this.label23, "label23");
			this.label23.Name = "label23";
			// 
			// cmbbLineNo
			// 
			resources.ApplyResources(this.cmbbLineNo, "cmbbLineNo");
			this.cmbbLineNo.FormattingEnabled = true;
			this.cmbbLineNo.Name = "cmbbLineNo";
			// 
			// label7
			// 
			resources.ApplyResources(this.label7, "label7");
			this.label7.Name = "label7";
			// 
			// btnAllGraph
			// 
			resources.ApplyResources(this.btnAllGraph, "btnAllGraph");
			this.btnAllGraph.Name = "btnAllGraph";
			this.btnAllGraph.UseVisualStyleBackColor = true;
			this.btnAllGraph.Click += new System.EventHandler(this.btnAllGraph_Click);
			// 
			// gbGraph
			// 
			resources.ApplyResources(this.gbGraph, "gbGraph");
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
			this.label10.Click += new System.EventHandler(this.label10_Click);
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
			// lblresult
			// 
			resources.ApplyResources(this.lblresult, "lblresult");
			this.lblresult.Name = "lblresult";
			// 
			// cbAnalysis
			// 
			resources.ApplyResources(this.cbAnalysis, "cbAnalysis");
			this.cbAnalysis.Name = "cbAnalysis";
			this.cbAnalysis.UseVisualStyleBackColor = true;
			this.cbAnalysis.CheckedChanged += new System.EventHandler(this.cbAnalysis_CheckedChanged);
			// 
			// gbQc
			// 
			resources.ApplyResources(this.gbQc, "gbQc");
			this.gbQc.Controls.Add(this.cmbbQcParamNM);
			this.gbQc.Controls.Add(this.label1);
			this.gbQc.Controls.Add(this.cmbbDefectNM);
			this.gbQc.Controls.Add(this.lblDefectNM);
			this.gbQc.Controls.Add(this.txtbProcessNO);
			this.gbQc.Controls.Add(this.txtbInspectionNO);
			this.gbQc.Name = "gbQc";
			this.gbQc.TabStop = false;
			// 
			// cmbbQcParamNM
			// 
			resources.ApplyResources(this.cmbbQcParamNM, "cmbbQcParamNM");
			this.cmbbQcParamNM.FormattingEnabled = true;
			this.cmbbQcParamNM.Name = "cmbbQcParamNM";
			this.cmbbQcParamNM.SelectedIndexChanged += new System.EventHandler(this.cmbbQcParamNM_SelectedIndexChanged);
			// 
			// label1
			// 
			resources.ApplyResources(this.label1, "label1");
			this.label1.Name = "label1";
			// 
			// cmbbDefectNM
			// 
			this.cmbbDefectNM.FormattingEnabled = true;
			resources.ApplyResources(this.cmbbDefectNM, "cmbbDefectNM");
			this.cmbbDefectNM.Name = "cmbbDefectNM";
			// 
			// lblDefectNM
			// 
			resources.ApplyResources(this.lblDefectNM, "lblDefectNM");
			this.lblDefectNM.Name = "lblDefectNM";
			// 
			// txtbProcessNO
			// 
			resources.ApplyResources(this.txtbProcessNO, "txtbProcessNO");
			this.txtbProcessNO.Name = "txtbProcessNO";
			this.txtbProcessNO.ReadOnly = true;
			// 
			// txtbInspectionNO
			// 
			resources.ApplyResources(this.txtbInspectionNO, "txtbInspectionNO");
			this.txtbInspectionNO.Name = "txtbInspectionNO";
			this.txtbInspectionNO.ReadOnly = true;
			this.txtbInspectionNO.TextChanged += new System.EventHandler(this.txtbInspectionNO_TextChanged);
			// 
			// lblNinja
			// 
			resources.ApplyResources(this.lblNinja, "lblNinja");
			this.lblNinja.Name = "lblNinja";
			// 
			// gbCompornent
			// 
			resources.ApplyResources(this.gbCompornent, "gbCompornent");
			this.gbCompornent.Controls.Add(this.cmbbType);
			this.gbCompornent.Controls.Add(this.label17);
			this.gbCompornent.Name = "gbCompornent";
			this.gbCompornent.TabStop = false;
			// 
			// cmbbType
			// 
			this.cmbbType.FormattingEnabled = true;
			resources.ApplyResources(this.cmbbType, "cmbbType");
			this.cmbbType.Name = "cmbbType";
			// 
			// label17
			// 
			resources.ApplyResources(this.label17, "label17");
			this.label17.Name = "label17";
			// 
			// btnNext
			// 
			resources.ApplyResources(this.btnNext, "btnNext");
			this.btnNext.Name = "btnNext";
			this.btnNext.UseVisualStyleBackColor = true;
			this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
			// 
			// lblDefectNO
			// 
			resources.ApplyResources(this.lblDefectNO, "lblDefectNO");
			this.lblDefectNO.Name = "lblDefectNO";
			// 
			// btnBack
			// 
			resources.ApplyResources(this.btnBack, "btnBack");
			this.btnBack.Name = "btnBack";
			this.btnBack.UseVisualStyleBackColor = true;
			this.btnBack.Click += new System.EventHandler(this.btnBack_Click);
			// 
			// btnRedraw
			// 
			resources.ApplyResources(this.btnRedraw, "btnRedraw");
			this.btnRedraw.Name = "btnRedraw";
			this.btnRedraw.UseVisualStyleBackColor = true;
			this.btnRedraw.Click += new System.EventHandler(this.btnRedraw_Click);
			// 
			// lblLotNO
			// 
			resources.ApplyResources(this.lblLotNO, "lblLotNO");
			this.lblLotNO.Name = "lblLotNO";
			// 
			// F03_TrendChart
			// 
			resources.ApplyResources(this, "$this");
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitContainer2);
			this.Name = "F03_TrendChart";
			this.Load += new System.EventHandler(this.frmDrawGraphAndList_Load);
			this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.frmDrawGraphAndList_FormClosed);
			this.splitContainer2.Panel1.ResumeLayout(false);
			this.splitContainer2.Panel2.ResumeLayout(false);
			this.splitContainer2.Panel2.PerformLayout();
			this.splitContainer2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
			this.splitContainer1.Panel1.ResumeLayout(false);
			this.splitContainer1.Panel2.ResumeLayout(false);
			this.splitContainer1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.grdLotStb)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dgvNascaList)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsMaterialBindingSource)).EndInit();
			((System.ComponentModel.ISupportInitialize)(this.dsMaterial)).EndInit();
			this.gbEquipment.ResumeLayout(false);
			this.gbEquipment.PerformLayout();
			this.gbGraph.ResumeLayout(false);
			this.gbGraph.PerformLayout();
			this.gbQc.ResumeLayout(false);
			this.gbQc.PerformLayout();
			this.gbCompornent.ResumeLayout(false);
			this.gbCompornent.PerformLayout();
			this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox gbGraph;
        private System.Windows.Forms.Label label22;
        private System.Windows.Forms.TextBox txtbEndMinute;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtbStartMinute;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtbEndHour;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtbStartHour;
        private System.Windows.Forms.Button btnRedraw;
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
        private System.Windows.Forms.ComboBox cmbbLineNo;
        private System.Windows.Forms.Label label23;
        private System.Windows.Forms.DataGridView dgvNascaList;
        private System.Windows.Forms.Button btnNext;
        private System.Windows.Forms.Button btnBack;
        private System.Windows.Forms.TextBox txtbInspectionNO;
        private System.Windows.Forms.Label lblresult;
        private System.Windows.Forms.Label lblDefectNM;
        private System.Windows.Forms.ComboBox cmbbDefectNM;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.GroupBox gbEquipment;
        private System.Windows.Forms.GroupBox gbQc;
        private System.Windows.Forms.GroupBox gbCompornent;
        private System.Windows.Forms.ComboBox cmbbType;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.Label lblDefectNO;
        private System.Windows.Forms.Label lblLotNO;
        private System.Windows.Forms.ComboBox cmbbAssetsNM;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblNinja;
        private System.Windows.Forms.TextBox txtbProcessNO;
        private System.Windows.Forms.BindingSource dsMaterialBindingSource;
        private dsMaterial dsMaterial;
        private System.Windows.Forms.CheckBox cbAnalysis;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.CheckedListBox clbMachineList;
        private System.Windows.Forms.Button btnAllGraph;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private System.Windows.Forms.SplitContainer splitContainer2;
        private System.Windows.Forms.DataGridView grdLotStb;
        private System.Windows.Forms.Button btnCatalog;
        private System.Windows.Forms.ComboBox cmbbQcParamNM;
        private System.Windows.Forms.DataGridViewTextBoxColumn lotNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MateGroup_JA;
        private System.Windows.Forms.DataGridViewTextBoxColumn mtralitemcdDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialjaDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mateLotNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mateChangeDTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialcd2DataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn compltdtDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn plantCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn MixResult_ID;
    }
}