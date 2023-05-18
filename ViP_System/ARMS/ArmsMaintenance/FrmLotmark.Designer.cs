namespace ArmsMaintenance
{
	partial class FrmLotmark
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmLotmark));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.txtWorkMagazineNo = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.txtLotNo = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.dgvItems = new System.Windows.Forms.DataGridView();
            this.ChangeFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.LotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MagazineStep = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StockerNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.MarkData = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.WorkDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ManualFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.bnItems = new System.Windows.Forms.BindingNavigator(this.components);
            this.bindingNavigatorCountItem = new System.Windows.Forms.ToolStripLabel();
            this.bindingNavigatorDeleteItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveFirstItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMovePreviousItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorPositionItem = new System.Windows.Forms.ToolStripTextBox();
            this.bindingNavigatorSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.bindingNavigatorMoveNextItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorMoveLastItem = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.txtWorkMagazineNo);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtLotNo);
            this.groupBox1.Controls.Add(this.btnSearch);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(599, 146);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "検索条件";
            // 
            // txtWorkMagazineNo
            // 
            this.txtWorkMagazineNo.Location = new System.Drawing.Point(143, 61);
            this.txtWorkMagazineNo.Name = "txtWorkMagazineNo";
            this.txtWorkMagazineNo.Size = new System.Drawing.Size(121, 19);
            this.txtWorkMagazineNo.TabIndex = 4;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(26, 64);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(111, 12);
            this.label2.TabIndex = 3;
            this.label2.Text = "マガジン番号(稼働中)";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(26, 34);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(53, 12);
            this.label1.TabIndex = 2;
            this.label1.Text = "ロット番号";
            // 
            // txtLotNo
            // 
            this.txtLotNo.Location = new System.Drawing.Point(143, 31);
            this.txtLotNo.Name = "txtLotNo";
            this.txtLotNo.Size = new System.Drawing.Size(121, 19);
            this.txtLotNo.TabIndex = 1;
            // 
            // btnSearch
            // 
            this.btnSearch.Image = ((System.Drawing.Image)(resources.GetObject("btnSearch.Image")));
            this.btnSearch.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.btnSearch.Location = new System.Drawing.Point(496, 107);
            this.btnSearch.Name = "btnSearch";
            this.btnSearch.Size = new System.Drawing.Size(83, 23);
            this.btnSearch.TabIndex = 0;
            this.btnSearch.Text = "検索";
            this.btnSearch.UseVisualStyleBackColor = true;
            this.btnSearch.Click += new System.EventHandler(this.btnSearch_Click);
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.dgvItems);
            this.groupBox2.Controls.Add(this.bnItems);
            this.groupBox2.Location = new System.Drawing.Point(12, 164);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(599, 361);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "一覧";
            // 
            // dgvItems
            // 
            this.dgvItems.AllowUserToAddRows = false;
            this.dgvItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.dgvItems.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dgvItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dgvItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgvItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ChangeFg,
            this.LotNo,
            this.MagazineStep,
            this.StockerNo,
            this.MarkData,
            this.WorkDt,
            this.ManualFg});
            this.dgvItems.DataSource = this.bsItems;
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dgvItems.DefaultCellStyle = dataGridViewCellStyle5;
            this.dgvItems.Location = new System.Drawing.Point(6, 18);
            this.dgvItems.Name = "dgvItems";
            this.dgvItems.RowHeadersVisible = false;
            this.dgvItems.RowTemplate.Height = 21;
            this.dgvItems.Size = new System.Drawing.Size(587, 312);
            this.dgvItems.TabIndex = 1;
            this.dgvItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dgvItems_CellValueChanged);
            // 
            // ChangeFg
            // 
            this.ChangeFg.DataPropertyName = "ChangeFg";
            this.ChangeFg.HeaderText = "変更";
            this.ChangeFg.Name = "ChangeFg";
            this.ChangeFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ChangeFg.Width = 40;
            // 
            // LotNo
            // 
            this.LotNo.DataPropertyName = "LotNo";
            this.LotNo.HeaderText = "ロット番号";
            this.LotNo.Name = "LotNo";
            this.LotNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.LotNo.Width = 120;
            // 
            // MagazineStep
            // 
            this.MagazineStep.DataPropertyName = "Row";
            this.MagazineStep.HeaderText = "マガジン段数";
            this.MagazineStep.Name = "MagazineStep";
            this.MagazineStep.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.MagazineStep.Width = 75;
            // 
            // StockerNo
            // 
            this.StockerNo.DataPropertyName = "StockerNo";
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Info;
            this.StockerNo.DefaultCellStyle = dataGridViewCellStyle2;
            this.StockerNo.HeaderText = "ストッカー";
            this.StockerNo.Name = "StockerNo";
            this.StockerNo.ReadOnly = true;
            this.StockerNo.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.StockerNo.Width = 80;
            // 
            // MarkData
            // 
            this.MarkData.DataPropertyName = "MarkData";
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Info;
            this.MarkData.DefaultCellStyle = dataGridViewCellStyle3;
            this.MarkData.HeaderText = "印字文字";
            this.MarkData.Name = "MarkData";
            this.MarkData.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // WorkDt
            // 
            this.WorkDt.DataPropertyName = "WorkDt";
            dataGridViewCellStyle4.BackColor = System.Drawing.SystemColors.Info;
            this.WorkDt.DefaultCellStyle = dataGridViewCellStyle4;
            this.WorkDt.HeaderText = "作業日時";
            this.WorkDt.Name = "WorkDt";
            this.WorkDt.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // ManualFg
            // 
            this.ManualFg.DataPropertyName = "ManualFg";
            this.ManualFg.HeaderText = "手動";
            this.ManualFg.Name = "ManualFg";
            this.ManualFg.ReadOnly = true;
            this.ManualFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.ManualFg.Width = 40;
            // 
            // bnItems
            // 
            this.bnItems.AddNewItem = null;
            this.bnItems.BindingSource = this.bsItems;
            this.bnItems.CountItem = this.bindingNavigatorCountItem;
            this.bnItems.DeleteItem = this.bindingNavigatorDeleteItem;
            this.bnItems.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.bnItems.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.bindingNavigatorMoveFirstItem,
            this.bindingNavigatorMovePreviousItem,
            this.bindingNavigatorSeparator,
            this.bindingNavigatorPositionItem,
            this.bindingNavigatorCountItem,
            this.bindingNavigatorSeparator1,
            this.bindingNavigatorMoveNextItem,
            this.bindingNavigatorMoveLastItem,
            this.bindingNavigatorSeparator2,
            this.btnAdd,
            this.bindingNavigatorDeleteItem,
            this.btnSave});
            this.bnItems.Location = new System.Drawing.Point(3, 333);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(593, 25);
            this.bnItems.TabIndex = 0;
            this.bnItems.Text = "bindingNavigator1";
            // 
            // bindingNavigatorCountItem
            // 
            this.bindingNavigatorCountItem.Name = "bindingNavigatorCountItem";
            this.bindingNavigatorCountItem.Size = new System.Drawing.Size(29, 22);
            this.bindingNavigatorCountItem.Text = "/ {0}";
            this.bindingNavigatorCountItem.ToolTipText = "項目の総数";
            // 
            // bindingNavigatorDeleteItem
            // 
            this.bindingNavigatorDeleteItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorDeleteItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorDeleteItem.Image")));
            this.bindingNavigatorDeleteItem.Name = "bindingNavigatorDeleteItem";
            this.bindingNavigatorDeleteItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorDeleteItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorDeleteItem.Text = "削除";
            this.bindingNavigatorDeleteItem.Visible = false;
            // 
            // bindingNavigatorMoveFirstItem
            // 
            this.bindingNavigatorMoveFirstItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveFirstItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveFirstItem.Image")));
            this.bindingNavigatorMoveFirstItem.Name = "bindingNavigatorMoveFirstItem";
            this.bindingNavigatorMoveFirstItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveFirstItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveFirstItem.Text = "最初に移動";
            // 
            // bindingNavigatorMovePreviousItem
            // 
            this.bindingNavigatorMovePreviousItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMovePreviousItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMovePreviousItem.Image")));
            this.bindingNavigatorMovePreviousItem.Name = "bindingNavigatorMovePreviousItem";
            this.bindingNavigatorMovePreviousItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMovePreviousItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMovePreviousItem.Text = "前に戻る";
            this.bindingNavigatorMovePreviousItem.Visible = false;
            // 
            // bindingNavigatorSeparator
            // 
            this.bindingNavigatorSeparator.Name = "bindingNavigatorSeparator";
            this.bindingNavigatorSeparator.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorPositionItem
            // 
            this.bindingNavigatorPositionItem.AccessibleName = "位置";
            this.bindingNavigatorPositionItem.AutoSize = false;
            this.bindingNavigatorPositionItem.Name = "bindingNavigatorPositionItem";
            this.bindingNavigatorPositionItem.Size = new System.Drawing.Size(50, 25);
            this.bindingNavigatorPositionItem.Text = "0";
            this.bindingNavigatorPositionItem.ToolTipText = "現在の場所";
            // 
            // bindingNavigatorSeparator1
            // 
            this.bindingNavigatorSeparator1.Name = "bindingNavigatorSeparator1";
            this.bindingNavigatorSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // bindingNavigatorMoveNextItem
            // 
            this.bindingNavigatorMoveNextItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveNextItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveNextItem.Image")));
            this.bindingNavigatorMoveNextItem.Name = "bindingNavigatorMoveNextItem";
            this.bindingNavigatorMoveNextItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveNextItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveNextItem.Text = "次に移動";
            this.bindingNavigatorMoveNextItem.Visible = false;
            // 
            // bindingNavigatorMoveLastItem
            // 
            this.bindingNavigatorMoveLastItem.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.bindingNavigatorMoveLastItem.Image = ((System.Drawing.Image)(resources.GetObject("bindingNavigatorMoveLastItem.Image")));
            this.bindingNavigatorMoveLastItem.Name = "bindingNavigatorMoveLastItem";
            this.bindingNavigatorMoveLastItem.RightToLeftAutoMirrorImage = true;
            this.bindingNavigatorMoveLastItem.Size = new System.Drawing.Size(23, 22);
            this.bindingNavigatorMoveLastItem.Text = "最後に移動";
            this.bindingNavigatorMoveLastItem.Click += new System.EventHandler(this.bindingNavigatorMoveLastItem_Click);
            // 
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // btnAdd
            // 
            this.btnAdd.Image = ((System.Drawing.Image)(resources.GetObject("btnAdd.Image")));
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.RightToLeftAutoMirrorImage = true;
            this.btnAdd.Size = new System.Drawing.Size(75, 22);
            this.btnAdd.Text = "新規追加";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnSave
            // 
            this.btnSave.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            this.btnSave.Image = ((System.Drawing.Image)(resources.GetObject("btnSave.Image")));
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(51, 22);
            this.btnSave.Text = "保存";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // FrmLotmark
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(621, 537);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmLotmark";
            this.Text = "FrmLotmark";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.GroupBox groupBox2;
		private System.Windows.Forms.BindingNavigator bnItems;
		private System.Windows.Forms.ToolStripButton btnAdd;
		private System.Windows.Forms.ToolStripLabel bindingNavigatorCountItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorDeleteItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveFirstItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMovePreviousItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator;
		private System.Windows.Forms.ToolStripTextBox bindingNavigatorPositionItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator1;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveNextItem;
		private System.Windows.Forms.ToolStripButton bindingNavigatorMoveLastItem;
		private System.Windows.Forms.ToolStripSeparator bindingNavigatorSeparator2;
		private System.Windows.Forms.DataGridView dgvItems;
		private System.Windows.Forms.ToolStripButton btnSave;
		private System.Windows.Forms.TextBox txtWorkMagazineNo;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox txtLotNo;
		private System.Windows.Forms.Button btnSearch;
		private System.Windows.Forms.BindingSource bsItems;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ChangeFg;
		private System.Windows.Forms.DataGridViewTextBoxColumn LotNo;
		private System.Windows.Forms.DataGridViewTextBoxColumn MagazineStep;
		private System.Windows.Forms.DataGridViewTextBoxColumn StockerNo;
		private System.Windows.Forms.DataGridViewTextBoxColumn MarkData;
		private System.Windows.Forms.DataGridViewTextBoxColumn WorkDt;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ManualFg;
	}
}