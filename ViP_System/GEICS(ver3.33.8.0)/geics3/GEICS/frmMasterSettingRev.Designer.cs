namespace GEICS
{
    partial class frmMasterSettingRev
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmMasterSettingRev));
            this.gvMasterSetting = new System.Windows.Forms.DataGridView();
            this.tvPLMExBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.dsTmPLMEx1 = new GEICS.dsTmPLMEx();
            this.revDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qcParamNODataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.modelNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.materialCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.classNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.manageNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Info1_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Info2_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Info3_NM = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.timingNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterMAXDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterMINDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parameterVALDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qcLinePNTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qcLineMAXDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.qcLineMINDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.reasonNMDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.updUserCDDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lastUpdDTDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.editFGDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.dSFGDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.gvMasterSetting)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvPLMExBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTmPLMEx1)).BeginInit();
            this.SuspendLayout();
            // 
            // gvMasterSetting
            // 
            this.gvMasterSetting.AccessibleDescription = null;
            this.gvMasterSetting.AccessibleName = null;
            this.gvMasterSetting.AllowUserToAddRows = false;
            this.gvMasterSetting.AllowUserToDeleteRows = false;
            resources.ApplyResources(this.gvMasterSetting, "gvMasterSetting");
            this.gvMasterSetting.AutoGenerateColumns = false;
            this.gvMasterSetting.BackgroundImage = null;
            this.gvMasterSetting.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.gvMasterSetting.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.revDataGridViewTextBoxColumn,
            this.qcParamNODataGridViewTextBoxColumn,
            this.modelNMDataGridViewTextBoxColumn,
            this.materialCDDataGridViewTextBoxColumn,
            this.classNMDataGridViewTextBoxColumn,
            this.parameterNMDataGridViewTextBoxColumn,
            this.manageNMDataGridViewTextBoxColumn,
            this.Info1_NM,
            this.Info2_NM,
            this.Info3_NM,
            this.timingNMDataGridViewTextBoxColumn,
            this.parameterMAXDataGridViewTextBoxColumn,
            this.parameterMINDataGridViewTextBoxColumn,
            this.parameterVALDataGridViewTextBoxColumn,
            this.qcLinePNTDataGridViewTextBoxColumn,
            this.qcLineMAXDataGridViewTextBoxColumn,
            this.qcLineMINDataGridViewTextBoxColumn,
            this.reasonNMDataGridViewTextBoxColumn,
            this.updUserCDDataGridViewTextBoxColumn,
            this.lastUpdDTDataGridViewTextBoxColumn,
            this.editFGDataGridViewCheckBoxColumn,
            this.dSFGDataGridViewCheckBoxColumn});
            this.gvMasterSetting.DataSource = this.tvPLMExBindingSource;
            this.gvMasterSetting.Font = null;
            this.gvMasterSetting.Name = "gvMasterSetting";
            this.gvMasterSetting.ReadOnly = true;
            this.gvMasterSetting.RowTemplate.Height = 21;
            // 
            // tvPLMExBindingSource
            // 
            this.tvPLMExBindingSource.DataMember = "TvPLMEx";
            this.tvPLMExBindingSource.DataSource = this.dsTmPLMEx1;
            // 
            // dsTmPLMEx1
            // 
            this.dsTmPLMEx1.DataSetName = "dsTmPLMEx";
            this.dsTmPLMEx1.SchemaSerializationMode = System.Data.SchemaSerializationMode.IncludeSchema;
            // 
            // revDataGridViewTextBoxColumn
            // 
            this.revDataGridViewTextBoxColumn.DataPropertyName = "Rev";
            resources.ApplyResources(this.revDataGridViewTextBoxColumn, "revDataGridViewTextBoxColumn");
            this.revDataGridViewTextBoxColumn.Name = "revDataGridViewTextBoxColumn";
            this.revDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // qcParamNODataGridViewTextBoxColumn
            // 
            this.qcParamNODataGridViewTextBoxColumn.DataPropertyName = "QcParam_NO";
            resources.ApplyResources(this.qcParamNODataGridViewTextBoxColumn, "qcParamNODataGridViewTextBoxColumn");
            this.qcParamNODataGridViewTextBoxColumn.Name = "qcParamNODataGridViewTextBoxColumn";
            this.qcParamNODataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // modelNMDataGridViewTextBoxColumn
            // 
            this.modelNMDataGridViewTextBoxColumn.DataPropertyName = "Model_NM";
            resources.ApplyResources(this.modelNMDataGridViewTextBoxColumn, "modelNMDataGridViewTextBoxColumn");
            this.modelNMDataGridViewTextBoxColumn.Name = "modelNMDataGridViewTextBoxColumn";
            this.modelNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // materialCDDataGridViewTextBoxColumn
            // 
            this.materialCDDataGridViewTextBoxColumn.DataPropertyName = "Material_CD";
            resources.ApplyResources(this.materialCDDataGridViewTextBoxColumn, "materialCDDataGridViewTextBoxColumn");
            this.materialCDDataGridViewTextBoxColumn.Name = "materialCDDataGridViewTextBoxColumn";
            this.materialCDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // classNMDataGridViewTextBoxColumn
            // 
            this.classNMDataGridViewTextBoxColumn.DataPropertyName = "Class_NM";
            resources.ApplyResources(this.classNMDataGridViewTextBoxColumn, "classNMDataGridViewTextBoxColumn");
            this.classNMDataGridViewTextBoxColumn.Name = "classNMDataGridViewTextBoxColumn";
            this.classNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // parameterNMDataGridViewTextBoxColumn
            // 
            this.parameterNMDataGridViewTextBoxColumn.DataPropertyName = "Parameter_NM";
            resources.ApplyResources(this.parameterNMDataGridViewTextBoxColumn, "parameterNMDataGridViewTextBoxColumn");
            this.parameterNMDataGridViewTextBoxColumn.Name = "parameterNMDataGridViewTextBoxColumn";
            this.parameterNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // manageNMDataGridViewTextBoxColumn
            // 
            this.manageNMDataGridViewTextBoxColumn.DataPropertyName = "Manage_NM";
            resources.ApplyResources(this.manageNMDataGridViewTextBoxColumn, "manageNMDataGridViewTextBoxColumn");
            this.manageNMDataGridViewTextBoxColumn.Name = "manageNMDataGridViewTextBoxColumn";
            this.manageNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // Info1_NM
            // 
            this.Info1_NM.DataPropertyName = "Info1_NM";
            resources.ApplyResources(this.Info1_NM, "Info1_NM");
            this.Info1_NM.Name = "Info1_NM";
            this.Info1_NM.ReadOnly = true;
            // 
            // Info2_NM
            // 
            this.Info2_NM.DataPropertyName = "Info2_NM";
            resources.ApplyResources(this.Info2_NM, "Info2_NM");
            this.Info2_NM.Name = "Info2_NM";
            this.Info2_NM.ReadOnly = true;
            // 
            // Info3_NM
            // 
            this.Info3_NM.DataPropertyName = "Info3_NM";
            resources.ApplyResources(this.Info3_NM, "Info3_NM");
            this.Info3_NM.Name = "Info3_NM";
            this.Info3_NM.ReadOnly = true;
            // 
            // timingNMDataGridViewTextBoxColumn
            // 
            this.timingNMDataGridViewTextBoxColumn.DataPropertyName = "Timing_NM";
            resources.ApplyResources(this.timingNMDataGridViewTextBoxColumn, "timingNMDataGridViewTextBoxColumn");
            this.timingNMDataGridViewTextBoxColumn.Name = "timingNMDataGridViewTextBoxColumn";
            this.timingNMDataGridViewTextBoxColumn.ReadOnly = true;
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
            // qcLinePNTDataGridViewTextBoxColumn
            // 
            this.qcLinePNTDataGridViewTextBoxColumn.DataPropertyName = "QcLine_PNT";
            resources.ApplyResources(this.qcLinePNTDataGridViewTextBoxColumn, "qcLinePNTDataGridViewTextBoxColumn");
            this.qcLinePNTDataGridViewTextBoxColumn.Name = "qcLinePNTDataGridViewTextBoxColumn";
            this.qcLinePNTDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // qcLineMAXDataGridViewTextBoxColumn
            // 
            this.qcLineMAXDataGridViewTextBoxColumn.DataPropertyName = "QcLine_MAX";
            resources.ApplyResources(this.qcLineMAXDataGridViewTextBoxColumn, "qcLineMAXDataGridViewTextBoxColumn");
            this.qcLineMAXDataGridViewTextBoxColumn.Name = "qcLineMAXDataGridViewTextBoxColumn";
            this.qcLineMAXDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // qcLineMINDataGridViewTextBoxColumn
            // 
            this.qcLineMINDataGridViewTextBoxColumn.DataPropertyName = "QcLine_MIN";
            resources.ApplyResources(this.qcLineMINDataGridViewTextBoxColumn, "qcLineMINDataGridViewTextBoxColumn");
            this.qcLineMINDataGridViewTextBoxColumn.Name = "qcLineMINDataGridViewTextBoxColumn";
            this.qcLineMINDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // reasonNMDataGridViewTextBoxColumn
            // 
            this.reasonNMDataGridViewTextBoxColumn.DataPropertyName = "Reason_NM";
            resources.ApplyResources(this.reasonNMDataGridViewTextBoxColumn, "reasonNMDataGridViewTextBoxColumn");
            this.reasonNMDataGridViewTextBoxColumn.Name = "reasonNMDataGridViewTextBoxColumn";
            this.reasonNMDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // updUserCDDataGridViewTextBoxColumn
            // 
            this.updUserCDDataGridViewTextBoxColumn.DataPropertyName = "UpdUser_CD";
            resources.ApplyResources(this.updUserCDDataGridViewTextBoxColumn, "updUserCDDataGridViewTextBoxColumn");
            this.updUserCDDataGridViewTextBoxColumn.Name = "updUserCDDataGridViewTextBoxColumn";
            this.updUserCDDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // lastUpdDTDataGridViewTextBoxColumn
            // 
            this.lastUpdDTDataGridViewTextBoxColumn.DataPropertyName = "LastUpd_DT";
            resources.ApplyResources(this.lastUpdDTDataGridViewTextBoxColumn, "lastUpdDTDataGridViewTextBoxColumn");
            this.lastUpdDTDataGridViewTextBoxColumn.Name = "lastUpdDTDataGridViewTextBoxColumn";
            this.lastUpdDTDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // editFGDataGridViewCheckBoxColumn
            // 
            this.editFGDataGridViewCheckBoxColumn.DataPropertyName = "Edit_FG";
            resources.ApplyResources(this.editFGDataGridViewCheckBoxColumn, "editFGDataGridViewCheckBoxColumn");
            this.editFGDataGridViewCheckBoxColumn.Name = "editFGDataGridViewCheckBoxColumn";
            this.editFGDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // dSFGDataGridViewCheckBoxColumn
            // 
            this.dSFGDataGridViewCheckBoxColumn.DataPropertyName = "DS_FG";
            resources.ApplyResources(this.dSFGDataGridViewCheckBoxColumn, "dSFGDataGridViewCheckBoxColumn");
            this.dSFGDataGridViewCheckBoxColumn.Name = "dSFGDataGridViewCheckBoxColumn";
            this.dSFGDataGridViewCheckBoxColumn.ReadOnly = true;
            // 
            // frmMasterSettingRev
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.gvMasterSetting);
            this.Font = null;
            this.Icon = null;
            this.Name = "frmMasterSettingRev";
            this.Load += new System.EventHandler(this.frmMasterSettingRev_Load);
            ((System.ComponentModel.ISupportInitialize)(this.gvMasterSetting)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.tvPLMExBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsTmPLMEx1)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.DataGridView gvMasterSetting;
        private dsTmPLMEx dsTmPLMEx1;
        private System.Windows.Forms.BindingSource tvPLMExBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn revDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn qcParamNODataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn modelNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn materialCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn classNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn manageNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info1_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info2_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn Info3_NM;
        private System.Windows.Forms.DataGridViewTextBoxColumn timingNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterMAXDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterMINDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn parameterVALDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn qcLinePNTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn qcLineMAXDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn qcLineMINDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn reasonNMDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn updUserCDDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn lastUpdDTDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn editFGDataGridViewCheckBoxColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn dSFGDataGridViewCheckBoxColumn;

    }
}