namespace GEICS
{
    partial class frmLotInfo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frmLotInfo));
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
            this.splitContainer1.Panel1.SuspendLayout();
            this.splitContainer1.Panel2.SuspendLayout();
            this.splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.grdLotStb)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNascaList)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsMaterialBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsMaterial)).BeginInit();
            this.SuspendLayout();
            // 
            // splitContainer1
            // 
            this.splitContainer1.AccessibleDescription = null;
            this.splitContainer1.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1, "splitContainer1");
            this.splitContainer1.BackgroundImage = null;
            this.splitContainer1.Font = null;
            this.splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            this.splitContainer1.Panel1.AccessibleDescription = null;
            this.splitContainer1.Panel1.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1.Panel1, "splitContainer1.Panel1");
            this.splitContainer1.Panel1.BackgroundImage = null;
            this.splitContainer1.Panel1.Controls.Add(this.grdLotStb);
            this.splitContainer1.Panel1.Font = null;
            // 
            // splitContainer1.Panel2
            // 
            this.splitContainer1.Panel2.AccessibleDescription = null;
            this.splitContainer1.Panel2.AccessibleName = null;
            resources.ApplyResources(this.splitContainer1.Panel2, "splitContainer1.Panel2");
            this.splitContainer1.Panel2.BackgroundImage = null;
            this.splitContainer1.Panel2.Controls.Add(this.dgvNascaList);
            this.splitContainer1.Panel2.Font = null;
            // 
            // grdLotStb
            // 
            this.grdLotStb.AccessibleDescription = null;
            this.grdLotStb.AccessibleName = null;
            this.grdLotStb.AllowUserToAddRows = false;
            resources.ApplyResources(this.grdLotStb, "grdLotStb");
            this.grdLotStb.BackgroundImage = null;
            this.grdLotStb.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdLotStb.Font = null;
            this.grdLotStb.Name = "grdLotStb";
            this.grdLotStb.RowTemplate.Height = 21;
            // 
            // dgvNascaList
            // 
            this.dgvNascaList.AccessibleDescription = null;
            this.dgvNascaList.AccessibleName = null;
            this.dgvNascaList.AllowUserToAddRows = false;
            resources.ApplyResources(this.dgvNascaList, "dgvNascaList");
            this.dgvNascaList.AutoGenerateColumns = false;
            this.dgvNascaList.BackgroundImage = null;
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
            this.dgvNascaList.Font = null;
            this.dgvNascaList.Name = "dgvNascaList";
            this.dgvNascaList.ReadOnly = true;
            this.dgvNascaList.RowTemplate.Height = 21;
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
            // frmLotInfo
            // 
            this.AccessibleDescription = null;
            this.AccessibleName = null;
            resources.ApplyResources(this, "$this");
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackgroundImage = null;
            this.Controls.Add(this.splitContainer1);
            this.Font = null;
            this.Icon = null;
            this.Name = "frmLotInfo";
            this.TopMost = true;
            this.Load += new System.EventHandler(this.frmLotInfo_Load);
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.frmLotInfo_FormClosing);
            this.splitContainer1.Panel1.ResumeLayout(false);
            this.splitContainer1.Panel2.ResumeLayout(false);
            this.splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.grdLotStb)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dgvNascaList)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsMaterialBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.dsMaterial)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer splitContainer1;
        private dsMaterial dsMaterial;
        private System.Windows.Forms.BindingSource dsMaterialBindingSource;
        private System.Windows.Forms.DataGridView dgvNascaList;
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
        private System.Windows.Forms.DataGridView grdLotStb;


    }
}