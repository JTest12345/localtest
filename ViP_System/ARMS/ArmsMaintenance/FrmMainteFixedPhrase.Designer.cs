namespace ArmsMaintenance
{
	partial class FrmMainteFixedPhrase
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmMainteFixedPhrase));
            this.grdItems = new System.Windows.Forms.DataGridView();
            this.TextId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.TextKb = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ChangeFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.FixedPhraseText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.OldText = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.DelFg = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.UpdUserCd = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LastUpdDt = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsItems = new System.Windows.Forms.BindingSource(this.components);
            this.groupBox1 = new System.Windows.Forms.GroupBox();
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
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.bindingNavigatorSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).BeginInit();
            this.groupBox1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).BeginInit();
            this.bnItems.SuspendLayout();
            this.SuspendLayout();
            // 
            // grdItems
            // 
            this.grdItems.AllowUserToAddRows = false;
            this.grdItems.AllowUserToDeleteRows = false;
            this.grdItems.AllowUserToResizeRows = false;
            this.grdItems.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.grdItems.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.grdItems.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.grdItems.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.grdItems.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.TextId,
            this.TextKb,
            this.ChangeFg,
            this.FixedPhraseText,
            this.OldText,
            this.DelFg,
            this.UpdUserCd,
            this.LastUpdDt});
            this.grdItems.DataSource = this.bsItems;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.grdItems.DefaultCellStyle = dataGridViewCellStyle2;
            this.grdItems.Location = new System.Drawing.Point(6, 18);
            this.grdItems.Name = "grdItems";
            this.grdItems.RowHeadersVisible = false;
            this.grdItems.RowTemplate.Height = 21;
            this.grdItems.Size = new System.Drawing.Size(604, 313);
            this.grdItems.TabIndex = 0;
            this.grdItems.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.grdItems_CellValueChanged);
            // 
            // TextId
            // 
            this.TextId.DataPropertyName = "TextId";
            this.TextId.HeaderText = "ID";
            this.TextId.Name = "TextId";
            this.TextId.Visible = false;
            this.TextId.Width = 40;
            // 
            // TextKb
            // 
            this.TextKb.DataPropertyName = "TextKb";
            this.TextKb.HeaderText = "区分";
            this.TextKb.Name = "TextKb";
            this.TextKb.Visible = false;
            // 
            // ChangeFg
            // 
            this.ChangeFg.DataPropertyName = "ChangeFg";
            this.ChangeFg.HeaderText = "変更";
            this.ChangeFg.Name = "ChangeFg";
            this.ChangeFg.Width = 40;
            // 
            // FixedPhraseText
            // 
            this.FixedPhraseText.DataPropertyName = "Text";
            this.FixedPhraseText.HeaderText = "定型文";
            this.FixedPhraseText.Name = "FixedPhraseText";
            this.FixedPhraseText.Width = 300;
            // 
            // OldText
            // 
            this.OldText.DataPropertyName = "OldText";
            this.OldText.HeaderText = "Old定型文";
            this.OldText.Name = "OldText";
            this.OldText.Visible = false;
            // 
            // DelFg
            // 
            this.DelFg.DataPropertyName = "DelFg";
            this.DelFg.HeaderText = "削除";
            this.DelFg.Name = "DelFg";
            this.DelFg.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.DelFg.Width = 40;
            // 
            // UpdUserCd
            // 
            this.UpdUserCd.DataPropertyName = "UpdUserCd";
            this.UpdUserCd.HeaderText = "更新者";
            this.UpdUserCd.Name = "UpdUserCd";
            // 
            // LastUpdDt
            // 
            this.LastUpdDt.DataPropertyName = "LastUpdDt";
            this.LastUpdDt.HeaderText = "更新日時";
            this.LastUpdDt.Name = "LastUpdDt";
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.bnItems);
            this.groupBox1.Controls.Add(this.grdItems);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(616, 360);
            this.groupBox1.TabIndex = 1;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "一覧";
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
            this.btnAdd,
            this.bindingNavigatorSeparator2,
            this.bindingNavigatorDeleteItem,
            this.btnSave});
            this.bnItems.Location = new System.Drawing.Point(3, 332);
            this.bnItems.MoveFirstItem = this.bindingNavigatorMoveFirstItem;
            this.bnItems.MoveLastItem = this.bindingNavigatorMoveLastItem;
            this.bnItems.MoveNextItem = this.bindingNavigatorMoveNextItem;
            this.bnItems.MovePreviousItem = this.bindingNavigatorMovePreviousItem;
            this.bnItems.Name = "bnItems";
            this.bnItems.PositionItem = this.bindingNavigatorPositionItem;
            this.bnItems.Size = new System.Drawing.Size(610, 25);
            this.bnItems.TabIndex = 1;
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
            // bindingNavigatorSeparator2
            // 
            this.bindingNavigatorSeparator2.Name = "bindingNavigatorSeparator2";
            this.bindingNavigatorSeparator2.Size = new System.Drawing.Size(6, 25);
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
            // FrmMainteFixedPhrase
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(640, 384);
            this.Controls.Add(this.groupBox1);
            this.Name = "FrmMainteFixedPhrase";
            this.Text = "データメンテナンス定型文設定";
            this.Load += new System.EventHandler(this.FrmMainteFixedPhrase_Load);
            ((System.ComponentModel.ISupportInitialize)(this.grdItems)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsItems)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.bnItems)).EndInit();
            this.bnItems.ResumeLayout(false);
            this.bnItems.PerformLayout();
            this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.DataGridView grdItems;
		private System.Windows.Forms.GroupBox groupBox1;
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
		private System.Windows.Forms.ToolStripButton btnSave;
		private System.Windows.Forms.BindingSource bsItems;
		private System.Windows.Forms.DataGridViewTextBoxColumn TextId;
		private System.Windows.Forms.DataGridViewTextBoxColumn TextKb;
		private System.Windows.Forms.DataGridViewCheckBoxColumn ChangeFg;
		private System.Windows.Forms.DataGridViewTextBoxColumn FixedPhraseText;
		private System.Windows.Forms.DataGridViewTextBoxColumn OldText;
		private System.Windows.Forms.DataGridViewCheckBoxColumn DelFg;
		private System.Windows.Forms.DataGridViewTextBoxColumn UpdUserCd;
		private System.Windows.Forms.DataGridViewTextBoxColumn LastUpdDt;
	}
}