namespace Oven_Control
{
    partial class MainForm
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle5 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle6 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle4 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series1 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.Start_btn = new System.Windows.Forms.Button();
            this.MachineNo_comboBox = new System.Windows.Forms.ComboBox();
            this.MachineInfo_label = new System.Windows.Forms.Label();
            this.Step1_groupBox = new System.Windows.Forms.GroupBox();
            this.Process_comboBox = new System.Windows.Forms.ComboBox();
            this.Step2_groupBox = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.operator_label = new System.Windows.Forms.Label();
            this.operator_textBox = new System.Windows.Forms.TextBox();
            this.Retry_btn = new System.Windows.Forms.Button();
            this.ReadQREnd_btn = new System.Windows.Forms.Button();
            this.ReadQR_textBox = new System.Windows.Forms.TextBox();
            this.product_info_dgv = new System.Windows.Forms.DataGridView();
            this.Product = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.LotNo = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Recipe = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ShowTestForm_btn = new System.Windows.Forms.Button();
            this.Info_textBox = new System.Windows.Forms.TextBox();
            this.RunStatus_label = new System.Windows.Forms.Label();
            this.Standby_btn = new System.Windows.Forms.Button();
            this.Temp_chart = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.SoftEnd_btn = new System.Windows.Forms.Button();
            this.CheckTemp_label = new System.Windows.Forms.Label();
            this.SelectRecipeFolder_fBD = new System.Windows.Forms.FolderBrowserDialog();
            this.SelectRecipeFolder_btn = new System.Windows.Forms.Button();
            this.RecipeFolder_label = new System.Windows.Forms.Label();
            this.UseFTP_checkBox = new System.Windows.Forms.CheckBox();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.tabPage1 = new System.Windows.Forms.TabPage();
            this.falcon_ng_label = new System.Windows.Forms.Label();
            this.Explanation_label = new System.Windows.Forms.Label();
            this.tabPage2 = new System.Windows.Forms.TabPage();
            this.tabPage3 = new System.Windows.Forms.TabPage();
            this.UseMail_checkBox = new System.Windows.Forms.CheckBox();
            this.tabPage4 = new System.Windows.Forms.TabPage();
            this.const_rb_gB = new System.Windows.Forms.GroupBox();
            this.const25_radioButton = new System.Windows.Forms.RadioButton();
            this.const200_radioButton = new System.Windows.Forms.RadioButton();
            this.const170_radioButton = new System.Windows.Forms.RadioButton();
            this.const120_radioButton = new System.Windows.Forms.RadioButton();
            this.const60_radioButton = new System.Windows.Forms.RadioButton();
            this.const150_radioButton = new System.Windows.Forms.RadioButton();
            this.const_start_button = new System.Windows.Forms.Button();
            this.Zoom_btn = new System.Windows.Forms.Button();
            this.CheckStart_label = new System.Windows.Forms.Label();
            this.pm2_label = new System.Windows.Forms.Label();
            this.Step1_groupBox.SuspendLayout();
            this.Step2_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.product_info_dgv)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Temp_chart)).BeginInit();
            this.tabControl1.SuspendLayout();
            this.tabPage1.SuspendLayout();
            this.tabPage2.SuspendLayout();
            this.tabPage3.SuspendLayout();
            this.tabPage4.SuspendLayout();
            this.const_rb_gB.SuspendLayout();
            this.SuspendLayout();
            // 
            // Start_btn
            // 
            this.Start_btn.BackColor = System.Drawing.SystemColors.Control;
            this.Start_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Start_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Start_btn.Enabled = false;
            this.Start_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Start_btn.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Start_btn.ForeColor = System.Drawing.Color.Black;
            this.Start_btn.Location = new System.Drawing.Point(5, 82);
            this.Start_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Start_btn.Name = "Start_btn";
            this.Start_btn.Size = new System.Drawing.Size(133, 75);
            this.Start_btn.TabIndex = 8;
            this.Start_btn.Text = "Start";
            this.Start_btn.UseVisualStyleBackColor = false;
            this.Start_btn.Click += new System.EventHandler(this.Start_btn_Click);
            // 
            // MachineNo_comboBox
            // 
            this.MachineNo_comboBox.BackColor = System.Drawing.SystemColors.Window;
            this.MachineNo_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.MachineNo_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.MachineNo_comboBox.FormattingEnabled = true;
            this.MachineNo_comboBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.MachineNo_comboBox.Location = new System.Drawing.Point(8, 38);
            this.MachineNo_comboBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.MachineNo_comboBox.Name = "MachineNo_comboBox";
            this.MachineNo_comboBox.Size = new System.Drawing.Size(131, 37);
            this.MachineNo_comboBox.Sorted = true;
            this.MachineNo_comboBox.TabIndex = 9;
            this.MachineNo_comboBox.SelectedIndexChanged += new System.EventHandler(this.MachineNo_comboBox_SelectedIndexChanged);
            // 
            // MachineInfo_label
            // 
            this.MachineInfo_label.AutoSize = true;
            this.MachineInfo_label.Location = new System.Drawing.Point(8, 82);
            this.MachineInfo_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.MachineInfo_label.Name = "MachineInfo_label";
            this.MachineInfo_label.Size = new System.Drawing.Size(263, 66);
            this.MachineInfo_label.TabIndex = 13;
            this.MachineInfo_label.Text = "MachineInfo_label\r\nIP Address";
            // 
            // Step1_groupBox
            // 
            this.Step1_groupBox.BackColor = System.Drawing.SystemColors.Control;
            this.Step1_groupBox.Controls.Add(this.Process_comboBox);
            this.Step1_groupBox.Controls.Add(this.MachineNo_comboBox);
            this.Step1_groupBox.Controls.Add(this.MachineInfo_label);
            this.Step1_groupBox.Enabled = false;
            this.Step1_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Step1_groupBox.Location = new System.Drawing.Point(8, 8);
            this.Step1_groupBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Step1_groupBox.Name = "Step1_groupBox";
            this.Step1_groupBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Step1_groupBox.Size = new System.Drawing.Size(370, 210);
            this.Step1_groupBox.TabIndex = 16;
            this.Step1_groupBox.TabStop = false;
            this.Step1_groupBox.Text = "1.キュア炉/工程選択";
            // 
            // Process_comboBox
            // 
            this.Process_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.Process_comboBox.FormattingEnabled = true;
            this.Process_comboBox.Location = new System.Drawing.Point(8, 158);
            this.Process_comboBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Process_comboBox.Name = "Process_comboBox";
            this.Process_comboBox.Size = new System.Drawing.Size(131, 41);
            this.Process_comboBox.TabIndex = 14;
            this.Process_comboBox.SelectedIndexChanged += new System.EventHandler(this.Process_comboBox_SelectedIndexChanged);
            // 
            // Step2_groupBox
            // 
            this.Step2_groupBox.Controls.Add(this.label1);
            this.Step2_groupBox.Controls.Add(this.operator_label);
            this.Step2_groupBox.Controls.Add(this.operator_textBox);
            this.Step2_groupBox.Controls.Add(this.Retry_btn);
            this.Step2_groupBox.Controls.Add(this.ReadQREnd_btn);
            this.Step2_groupBox.Controls.Add(this.ReadQR_textBox);
            this.Step2_groupBox.Enabled = false;
            this.Step2_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Step2_groupBox.Location = new System.Drawing.Point(8, 252);
            this.Step2_groupBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Step2_groupBox.Name = "Step2_groupBox";
            this.Step2_groupBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Step2_groupBox.Size = new System.Drawing.Size(692, 135);
            this.Step2_groupBox.TabIndex = 17;
            this.Step2_groupBox.TabStop = false;
            this.Step2_groupBox.Text = "2.ロット管理表QRコード読み取り";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.label1.Location = new System.Drawing.Point(258, 90);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(131, 33);
            this.label1.TabIndex = 6;
            this.label1.Text = "作業者：";
            // 
            // operator_label
            // 
            this.operator_label.AutoSize = true;
            this.operator_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.operator_label.Location = new System.Drawing.Point(400, 90);
            this.operator_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.operator_label.Name = "operator_label";
            this.operator_label.Size = new System.Drawing.Size(32, 33);
            this.operator_label.TabIndex = 5;
            this.operator_label.Text = "-";
            // 
            // operator_textBox
            // 
            this.operator_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.operator_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.operator_textBox.Location = new System.Drawing.Point(8, 86);
            this.operator_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.operator_textBox.Name = "operator_textBox";
            this.operator_textBox.Size = new System.Drawing.Size(231, 39);
            this.operator_textBox.TabIndex = 4;
            this.operator_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.operator_textBox_KeyPress);
            // 
            // Retry_btn
            // 
            this.Retry_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Retry_btn.Location = new System.Drawing.Point(500, 38);
            this.Retry_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Retry_btn.Name = "Retry_btn";
            this.Retry_btn.Size = new System.Drawing.Size(167, 42);
            this.Retry_btn.TabIndex = 3;
            this.Retry_btn.Text = "やり直す";
            this.Retry_btn.UseVisualStyleBackColor = true;
            this.Retry_btn.Click += new System.EventHandler(this.Retry_btn_Click);
            // 
            // ReadQREnd_btn
            // 
            this.ReadQREnd_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.ReadQREnd_btn.Enabled = false;
            this.ReadQREnd_btn.Location = new System.Drawing.Point(250, 38);
            this.ReadQREnd_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.ReadQREnd_btn.Name = "ReadQREnd_btn";
            this.ReadQREnd_btn.Size = new System.Drawing.Size(233, 42);
            this.ReadQREnd_btn.TabIndex = 2;
            this.ReadQREnd_btn.Text = "読み取り終了";
            this.ReadQREnd_btn.UseVisualStyleBackColor = true;
            this.ReadQREnd_btn.Click += new System.EventHandler(this.ReadQREnd_btn_Click);
            // 
            // ReadQR_textBox
            // 
            this.ReadQR_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ReadQR_textBox.Location = new System.Drawing.Point(8, 38);
            this.ReadQR_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.ReadQR_textBox.Name = "ReadQR_textBox";
            this.ReadQR_textBox.Size = new System.Drawing.Size(231, 39);
            this.ReadQR_textBox.TabIndex = 0;
            this.ReadQR_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReadQR_textBox_KeyPress);
            // 
            // product_info_dgv
            // 
            this.product_info_dgv.AllowUserToAddRows = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.product_info_dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.product_info_dgv.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.product_info_dgv.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Product,
            this.LotNo,
            this.Recipe});
            dataGridViewCellStyle5.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle5.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle5.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle5.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.product_info_dgv.DefaultCellStyle = dataGridViewCellStyle5;
            this.product_info_dgv.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.product_info_dgv.Location = new System.Drawing.Point(7, 398);
            this.product_info_dgv.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.product_info_dgv.Name = "product_info_dgv";
            dataGridViewCellStyle6.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle6.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle6.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle6.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle6.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle6.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle6.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.product_info_dgv.RowHeadersDefaultCellStyle = dataGridViewCellStyle6;
            this.product_info_dgv.RowHeadersWidth = 51;
            this.product_info_dgv.RowTemplate.Height = 21;
            this.product_info_dgv.Size = new System.Drawing.Size(877, 288);
            this.product_info_dgv.TabIndex = 18;
            this.product_info_dgv.RowsAdded += new System.Windows.Forms.DataGridViewRowsAddedEventHandler(this.product_info_dgv_RowsAdded);
            // 
            // Product
            // 
            this.Product.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.Product.DefaultCellStyle = dataGridViewCellStyle2;
            this.Product.HeaderText = "Product";
            this.Product.MinimumWidth = 6;
            this.Product.Name = "Product";
            this.Product.ReadOnly = true;
            this.Product.Width = 146;
            // 
            // LotNo
            // 
            this.LotNo.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.LotNo.DefaultCellStyle = dataGridViewCellStyle3;
            this.LotNo.HeaderText = "LotNo";
            this.LotNo.MinimumWidth = 6;
            this.LotNo.Name = "LotNo";
            this.LotNo.ReadOnly = true;
            this.LotNo.Width = 124;
            // 
            // Recipe
            // 
            this.Recipe.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.AllCells;
            dataGridViewCellStyle4.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.Recipe.DefaultCellStyle = dataGridViewCellStyle4;
            this.Recipe.HeaderText = "Recipe";
            this.Recipe.MinimumWidth = 6;
            this.Recipe.Name = "Recipe";
            this.Recipe.Width = 132;
            // 
            // ShowTestForm_btn
            // 
            this.ShowTestForm_btn.Location = new System.Drawing.Point(1238, 126);
            this.ShowTestForm_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.ShowTestForm_btn.Name = "ShowTestForm_btn";
            this.ShowTestForm_btn.Size = new System.Drawing.Size(178, 90);
            this.ShowTestForm_btn.TabIndex = 19;
            this.ShowTestForm_btn.Text = "テストフォーム表示";
            this.ShowTestForm_btn.UseVisualStyleBackColor = true;
            this.ShowTestForm_btn.Visible = false;
            this.ShowTestForm_btn.Click += new System.EventHandler(this.ShowTestForm_btn_Click);
            // 
            // Info_textBox
            // 
            this.Info_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.Info_textBox.BackColor = System.Drawing.Color.Black;
            this.Info_textBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Info_textBox.ForeColor = System.Drawing.Color.Yellow;
            this.Info_textBox.Location = new System.Drawing.Point(894, 398);
            this.Info_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Info_textBox.Multiline = true;
            this.Info_textBox.Name = "Info_textBox";
            this.Info_textBox.ReadOnly = true;
            this.Info_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.Info_textBox.Size = new System.Drawing.Size(531, 349);
            this.Info_textBox.TabIndex = 20;
            this.Info_textBox.WordWrap = false;
            // 
            // RunStatus_label
            // 
            this.RunStatus_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.RunStatus_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.RunStatus_label.Font = new System.Drawing.Font("MS UI Gothic", 24F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RunStatus_label.Location = new System.Drawing.Point(5, 4);
            this.RunStatus_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.RunStatus_label.Name = "RunStatus_label";
            this.RunStatus_label.Size = new System.Drawing.Size(250, 75);
            this.RunStatus_label.TabIndex = 21;
            this.RunStatus_label.Text = "停止中";
            this.RunStatus_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.RunStatus_label.TextChanged += new System.EventHandler(this.RunStatus_label_TextChanged);
            // 
            // Standby_btn
            // 
            this.Standby_btn.BackColor = System.Drawing.SystemColors.Control;
            this.Standby_btn.BackgroundImageLayout = System.Windows.Forms.ImageLayout.None;
            this.Standby_btn.Cursor = System.Windows.Forms.Cursors.Hand;
            this.Standby_btn.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.Standby_btn.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Standby_btn.ForeColor = System.Drawing.Color.Black;
            this.Standby_btn.Location = new System.Drawing.Point(142, 82);
            this.Standby_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Standby_btn.Name = "Standby_btn";
            this.Standby_btn.Size = new System.Drawing.Size(133, 75);
            this.Standby_btn.TabIndex = 22;
            this.Standby_btn.Text = "Stop";
            this.Standby_btn.UseVisualStyleBackColor = false;
            this.Standby_btn.Click += new System.EventHandler(this.Standby_btn_Click);
            // 
            // Temp_chart
            // 
            this.Temp_chart.BackColor = System.Drawing.Color.Black;
            this.Temp_chart.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.Temp_chart.BorderlineWidth = 2;
            chartArea1.AxisX.IsLabelAutoFit = false;
            chartArea1.AxisX.LabelStyle.Angle = 60;
            chartArea1.AxisX.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisX.LabelStyle.Format = "HH:mm:ss";
            chartArea1.AxisX.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisX.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.NotSet;
            chartArea1.AxisX.Title = "Date Time";
            chartArea1.AxisX.TitleForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LabelStyle.ForeColor = System.Drawing.Color.White;
            chartArea1.AxisY.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineColor = System.Drawing.Color.White;
            chartArea1.AxisY.MajorGrid.LineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Dash;
            chartArea1.AxisY.Title = "Temperature[℃]";
            chartArea1.AxisY.TitleForeColor = System.Drawing.Color.White;
            chartArea1.BackColor = System.Drawing.Color.Black;
            chartArea1.BorderColor = System.Drawing.Color.White;
            chartArea1.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea1.InnerPlotPosition.Auto = false;
            chartArea1.InnerPlotPosition.Height = 70F;
            chartArea1.InnerPlotPosition.Width = 90F;
            chartArea1.InnerPlotPosition.X = 8F;
            chartArea1.InnerPlotPosition.Y = 5F;
            chartArea1.Name = "ChartArea1";
            this.Temp_chart.ChartAreas.Add(chartArea1);
            legend1.Enabled = false;
            legend1.Name = "Legend1";
            this.Temp_chart.Legends.Add(legend1);
            this.Temp_chart.Location = new System.Drawing.Point(8, 690);
            this.Temp_chart.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Temp_chart.Name = "Temp_chart";
            series1.BorderWidth = 3;
            series1.ChartArea = "ChartArea1";
            series1.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series1.Color = System.Drawing.Color.Yellow;
            series1.Legend = "Legend1";
            series1.Name = "temp";
            series1.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series2.BorderWidth = 2;
            series2.ChartArea = "ChartArea1";
            series2.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series2.Color = System.Drawing.Color.Red;
            series2.Legend = "Legend1";
            series2.Name = "upper";
            series2.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series3.BorderWidth = 2;
            series3.ChartArea = "ChartArea1";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Line;
            series3.Color = System.Drawing.Color.Red;
            series3.Legend = "Legend1";
            series3.Name = "lower";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            this.Temp_chart.Series.Add(series1);
            this.Temp_chart.Series.Add(series2);
            this.Temp_chart.Series.Add(series3);
            this.Temp_chart.Size = new System.Drawing.Size(1300, 390);
            this.Temp_chart.TabIndex = 23;
            this.Temp_chart.Text = "chart1";
            // 
            // SoftEnd_btn
            // 
            this.SoftEnd_btn.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SoftEnd_btn.Location = new System.Drawing.Point(1277, 4);
            this.SoftEnd_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.SoftEnd_btn.Name = "SoftEnd_btn";
            this.SoftEnd_btn.Size = new System.Drawing.Size(133, 105);
            this.SoftEnd_btn.TabIndex = 24;
            this.SoftEnd_btn.Text = "ソフト\r\n終了";
            this.SoftEnd_btn.UseVisualStyleBackColor = true;
            this.SoftEnd_btn.Click += new System.EventHandler(this.SoftEnd_btn_Click);
            // 
            // CheckTemp_label
            // 
            this.CheckTemp_label.AutoSize = true;
            this.CheckTemp_label.BackColor = System.Drawing.Color.Red;
            this.CheckTemp_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CheckTemp_label.ForeColor = System.Drawing.Color.White;
            this.CheckTemp_label.Location = new System.Drawing.Point(355, 83);
            this.CheckTemp_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.CheckTemp_label.Name = "CheckTemp_label";
            this.CheckTemp_label.Size = new System.Drawing.Size(312, 82);
            this.CheckTemp_label.TabIndex = 25;
            this.CheckTemp_label.Text = "温度上下限異常\r\n発生しました";
            this.CheckTemp_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckTemp_label.Visible = false;
            // 
            // SelectRecipeFolder_btn
            // 
            this.SelectRecipeFolder_btn.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SelectRecipeFolder_btn.Location = new System.Drawing.Point(5, 4);
            this.SelectRecipeFolder_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.SelectRecipeFolder_btn.Name = "SelectRecipeFolder_btn";
            this.SelectRecipeFolder_btn.Size = new System.Drawing.Size(247, 58);
            this.SelectRecipeFolder_btn.TabIndex = 26;
            this.SelectRecipeFolder_btn.Text = "条件フォルダ選択";
            this.SelectRecipeFolder_btn.UseVisualStyleBackColor = true;
            this.SelectRecipeFolder_btn.Click += new System.EventHandler(this.SelectRecipeFolder_btn_Click);
            // 
            // RecipeFolder_label
            // 
            this.RecipeFolder_label.AutoSize = true;
            this.RecipeFolder_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.RecipeFolder_label.Location = new System.Drawing.Point(5, 68);
            this.RecipeFolder_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.RecipeFolder_label.Name = "RecipeFolder_label";
            this.RecipeFolder_label.Size = new System.Drawing.Size(192, 24);
            this.RecipeFolder_label.TabIndex = 27;
            this.RecipeFolder_label.Text = "RecipeFolder_label";
            // 
            // UseFTP_checkBox
            // 
            this.UseFTP_checkBox.AutoSize = true;
            this.UseFTP_checkBox.Checked = true;
            this.UseFTP_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseFTP_checkBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.UseFTP_checkBox.Location = new System.Drawing.Point(5, 4);
            this.UseFTP_checkBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.UseFTP_checkBox.Name = "UseFTP_checkBox";
            this.UseFTP_checkBox.Size = new System.Drawing.Size(408, 33);
            this.UseFTP_checkBox.TabIndex = 28;
            this.UseFTP_checkBox.Text = "FTPで上位システムへ送信する";
            this.UseFTP_checkBox.UseVisualStyleBackColor = true;
            this.UseFTP_checkBox.CheckedChanged += new System.EventHandler(this.UseFTP_checkBox_CheckedChanged);
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.tabPage1);
            this.tabControl1.Controls.Add(this.tabPage2);
            this.tabControl1.Controls.Add(this.tabPage3);
            this.tabControl1.Controls.Add(this.tabPage4);
            this.tabControl1.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.tabControl1.ItemSize = new System.Drawing.Size(65, 30);
            this.tabControl1.Location = new System.Drawing.Point(383, 8);
            this.tabControl1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(700, 243);
            this.tabControl1.TabIndex = 29;
            // 
            // tabPage1
            // 
            this.tabPage1.AutoScroll = true;
            this.tabPage1.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage1.Controls.Add(this.falcon_ng_label);
            this.tabPage1.Controls.Add(this.Explanation_label);
            this.tabPage1.Controls.Add(this.RunStatus_label);
            this.tabPage1.Controls.Add(this.Start_btn);
            this.tabPage1.Controls.Add(this.Standby_btn);
            this.tabPage1.Controls.Add(this.CheckTemp_label);
            this.tabPage1.Location = new System.Drawing.Point(4, 34);
            this.tabPage1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage1.Name = "tabPage1";
            this.tabPage1.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage1.Size = new System.Drawing.Size(692, 205);
            this.tabPage1.TabIndex = 0;
            this.tabPage1.Text = "RunStatus";
            // 
            // falcon_ng_label
            // 
            this.falcon_ng_label.BackColor = System.Drawing.Color.Red;
            this.falcon_ng_label.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.falcon_ng_label.ForeColor = System.Drawing.Color.White;
            this.falcon_ng_label.Location = new System.Drawing.Point(363, 99);
            this.falcon_ng_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.falcon_ng_label.Name = "falcon_ng_label";
            this.falcon_ng_label.Size = new System.Drawing.Size(301, 92);
            this.falcon_ng_label.TabIndex = 27;
            this.falcon_ng_label.Text = "±1度管理異常が\r\n発生しました";
            this.falcon_ng_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.falcon_ng_label.Visible = false;
            // 
            // Explanation_label
            // 
            this.Explanation_label.AutoSize = true;
            this.Explanation_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.Explanation_label.Location = new System.Drawing.Point(292, 4);
            this.Explanation_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.Explanation_label.Name = "Explanation_label";
            this.Explanation_label.Size = new System.Drawing.Size(28, 29);
            this.Explanation_label.TabIndex = 26;
            this.Explanation_label.Text = "-";
            // 
            // tabPage2
            // 
            this.tabPage2.AutoScroll = true;
            this.tabPage2.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage2.Controls.Add(this.SelectRecipeFolder_btn);
            this.tabPage2.Controls.Add(this.RecipeFolder_label);
            this.tabPage2.Location = new System.Drawing.Point(4, 34);
            this.tabPage2.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage2.Name = "tabPage2";
            this.tabPage2.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage2.Size = new System.Drawing.Size(692, 205);
            this.tabPage2.TabIndex = 1;
            this.tabPage2.Text = "Select Folder";
            // 
            // tabPage3
            // 
            this.tabPage3.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage3.Controls.Add(this.UseMail_checkBox);
            this.tabPage3.Controls.Add(this.UseFTP_checkBox);
            this.tabPage3.Location = new System.Drawing.Point(4, 34);
            this.tabPage3.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage3.Name = "tabPage3";
            this.tabPage3.Size = new System.Drawing.Size(692, 205);
            this.tabPage3.TabIndex = 2;
            this.tabPage3.Text = "Option";
            // 
            // UseMail_checkBox
            // 
            this.UseMail_checkBox.AutoSize = true;
            this.UseMail_checkBox.Checked = true;
            this.UseMail_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.UseMail_checkBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.UseMail_checkBox.Location = new System.Drawing.Point(5, 50);
            this.UseMail_checkBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.UseMail_checkBox.Name = "UseMail_checkBox";
            this.UseMail_checkBox.Size = new System.Drawing.Size(423, 33);
            this.UseMail_checkBox.TabIndex = 29;
            this.UseMail_checkBox.Text = "上下限異常時にメール送信する";
            this.UseMail_checkBox.UseVisualStyleBackColor = true;
            this.UseMail_checkBox.CheckedChanged += new System.EventHandler(this.UseMail_checkBox_CheckedChanged);
            // 
            // tabPage4
            // 
            this.tabPage4.BackColor = System.Drawing.SystemColors.Control;
            this.tabPage4.Controls.Add(this.const_rb_gB);
            this.tabPage4.Controls.Add(this.const_start_button);
            this.tabPage4.Location = new System.Drawing.Point(4, 34);
            this.tabPage4.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage4.Name = "tabPage4";
            this.tabPage4.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.tabPage4.Size = new System.Drawing.Size(692, 205);
            this.tabPage4.TabIndex = 3;
            this.tabPage4.Text = "Maintenance";
            // 
            // const_rb_gB
            // 
            this.const_rb_gB.Controls.Add(this.const25_radioButton);
            this.const_rb_gB.Controls.Add(this.const200_radioButton);
            this.const_rb_gB.Controls.Add(this.const170_radioButton);
            this.const_rb_gB.Controls.Add(this.const120_radioButton);
            this.const_rb_gB.Controls.Add(this.const60_radioButton);
            this.const_rb_gB.Controls.Add(this.const150_radioButton);
            this.const_rb_gB.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.const_rb_gB.Location = new System.Drawing.Point(5, 4);
            this.const_rb_gB.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const_rb_gB.Name = "const_rb_gB";
            this.const_rb_gB.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const_rb_gB.Size = new System.Drawing.Size(467, 172);
            this.const_rb_gB.TabIndex = 3;
            this.const_rb_gB.TabStop = false;
            this.const_rb_gB.Text = "定値運転温度";
            // 
            // const25_radioButton
            // 
            this.const25_radioButton.AutoSize = true;
            this.const25_radioButton.Location = new System.Drawing.Point(8, 38);
            this.const25_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const25_radioButton.Name = "const25_radioButton";
            this.const25_radioButton.Size = new System.Drawing.Size(83, 28);
            this.const25_radioButton.TabIndex = 6;
            this.const25_radioButton.Tag = "";
            this.const25_radioButton.Text = "25℃";
            this.const25_radioButton.UseVisualStyleBackColor = true;
            // 
            // const200_radioButton
            // 
            this.const200_radioButton.AutoSize = true;
            this.const200_radioButton.Location = new System.Drawing.Point(8, 128);
            this.const200_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const200_radioButton.Name = "const200_radioButton";
            this.const200_radioButton.Size = new System.Drawing.Size(95, 28);
            this.const200_radioButton.TabIndex = 5;
            this.const200_radioButton.Tag = "";
            this.const200_radioButton.Text = "200℃";
            this.const200_radioButton.UseVisualStyleBackColor = true;
            // 
            // const170_radioButton
            // 
            this.const170_radioButton.AutoSize = true;
            this.const170_radioButton.Location = new System.Drawing.Point(308, 82);
            this.const170_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const170_radioButton.Name = "const170_radioButton";
            this.const170_radioButton.Size = new System.Drawing.Size(95, 28);
            this.const170_radioButton.TabIndex = 4;
            this.const170_radioButton.Tag = "";
            this.const170_radioButton.Text = "170℃";
            this.const170_radioButton.UseVisualStyleBackColor = true;
            // 
            // const120_radioButton
            // 
            this.const120_radioButton.AutoSize = true;
            this.const120_radioButton.Location = new System.Drawing.Point(8, 82);
            this.const120_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const120_radioButton.Name = "const120_radioButton";
            this.const120_radioButton.Size = new System.Drawing.Size(95, 28);
            this.const120_radioButton.TabIndex = 3;
            this.const120_radioButton.Tag = "";
            this.const120_radioButton.Text = "120℃";
            this.const120_radioButton.UseVisualStyleBackColor = true;
            // 
            // const60_radioButton
            // 
            this.const60_radioButton.AutoSize = true;
            this.const60_radioButton.Checked = true;
            this.const60_radioButton.Location = new System.Drawing.Point(158, 38);
            this.const60_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const60_radioButton.Name = "const60_radioButton";
            this.const60_radioButton.Size = new System.Drawing.Size(83, 28);
            this.const60_radioButton.TabIndex = 1;
            this.const60_radioButton.TabStop = true;
            this.const60_radioButton.Tag = "";
            this.const60_radioButton.Text = "60℃";
            this.const60_radioButton.UseVisualStyleBackColor = true;
            // 
            // const150_radioButton
            // 
            this.const150_radioButton.AutoSize = true;
            this.const150_radioButton.Location = new System.Drawing.Point(158, 82);
            this.const150_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const150_radioButton.Name = "const150_radioButton";
            this.const150_radioButton.Size = new System.Drawing.Size(95, 28);
            this.const150_radioButton.TabIndex = 2;
            this.const150_radioButton.Tag = "";
            this.const150_radioButton.Text = "150℃";
            this.const150_radioButton.UseVisualStyleBackColor = true;
            // 
            // const_start_button
            // 
            this.const_start_button.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.const_start_button.Font = new System.Drawing.Font("MS UI Gothic", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.const_start_button.Location = new System.Drawing.Point(477, 38);
            this.const_start_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.const_start_button.Name = "const_start_button";
            this.const_start_button.Size = new System.Drawing.Size(200, 120);
            this.const_start_button.TabIndex = 0;
            this.const_start_button.Text = "定値運転\r\n開始";
            this.const_start_button.UseVisualStyleBackColor = true;
            this.const_start_button.Click += new System.EventHandler(this.const_start_button_Click);
            // 
            // Zoom_btn
            // 
            this.Zoom_btn.Location = new System.Drawing.Point(1317, 690);
            this.Zoom_btn.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Zoom_btn.Name = "Zoom_btn";
            this.Zoom_btn.Size = new System.Drawing.Size(67, 60);
            this.Zoom_btn.TabIndex = 30;
            this.Zoom_btn.Text = "拡大";
            this.Zoom_btn.UseVisualStyleBackColor = true;
            this.Zoom_btn.Click += new System.EventHandler(this.Zoom_btn_Click);
            // 
            // CheckStart_label
            // 
            this.CheckStart_label.BackColor = System.Drawing.Color.Red;
            this.CheckStart_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.CheckStart_label.ForeColor = System.Drawing.Color.White;
            this.CheckStart_label.Location = new System.Drawing.Point(1158, 252);
            this.CheckStart_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.CheckStart_label.Name = "CheckStart_label";
            this.CheckStart_label.Size = new System.Drawing.Size(202, 62);
            this.CheckStart_label.TabIndex = 31;
            this.CheckStart_label.Text = "CheckStart_label";
            this.CheckStart_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.CheckStart_label.Visible = false;
            // 
            // pm2_label
            // 
            this.pm2_label.BackColor = System.Drawing.Color.Red;
            this.pm2_label.Font = new System.Drawing.Font("MS UI Gothic", 11F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.pm2_label.ForeColor = System.Drawing.Color.White;
            this.pm2_label.Location = new System.Drawing.Point(710, 255);
            this.pm2_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.pm2_label.Name = "pm2_label";
            this.pm2_label.Size = new System.Drawing.Size(438, 122);
            this.pm2_label.TabIndex = 32;
            this.pm2_label.Text = "FALCON55℃ステップ専用,NG回数:4回";
            this.pm2_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.pm2_label.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.BackgroundImage = global::Oven_Control.Properties.Resources.BackgroundImage;
            this.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.ClientSize = new System.Drawing.Size(1415, 1014);
            this.Controls.Add(this.pm2_label);
            this.Controls.Add(this.CheckStart_label);
            this.Controls.Add(this.Zoom_btn);
            this.Controls.Add(this.tabControl1);
            this.Controls.Add(this.SoftEnd_btn);
            this.Controls.Add(this.Temp_chart);
            this.Controls.Add(this.Info_textBox);
            this.Controls.Add(this.ShowTestForm_btn);
            this.Controls.Add(this.product_info_dgv);
            this.Controls.Add(this.Step2_groupBox);
            this.Controls.Add(this.Step1_groupBox);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
            this.Text = "Oven Control(C#)  version=2.1.0 2023/3/21";
            this.Step1_groupBox.ResumeLayout(false);
            this.Step1_groupBox.PerformLayout();
            this.Step2_groupBox.ResumeLayout(false);
            this.Step2_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.product_info_dgv)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Temp_chart)).EndInit();
            this.tabControl1.ResumeLayout(false);
            this.tabPage1.ResumeLayout(false);
            this.tabPage1.PerformLayout();
            this.tabPage2.ResumeLayout(false);
            this.tabPage2.PerformLayout();
            this.tabPage3.ResumeLayout(false);
            this.tabPage3.PerformLayout();
            this.tabPage4.ResumeLayout(false);
            this.const_rb_gB.ResumeLayout(false);
            this.const_rb_gB.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button Start_btn;
        private System.Windows.Forms.ComboBox MachineNo_comboBox;
        private System.Windows.Forms.Label MachineInfo_label;
        private System.Windows.Forms.GroupBox Step1_groupBox;
        private System.Windows.Forms.GroupBox Step2_groupBox;
        private System.Windows.Forms.TextBox ReadQR_textBox;
        private System.Windows.Forms.DataGridView product_info_dgv;
        private System.Windows.Forms.Button ReadQREnd_btn;
        private System.Windows.Forms.Button Retry_btn;
        private System.Windows.Forms.Button ShowTestForm_btn;
        private System.Windows.Forms.Label RunStatus_label;
        public System.Windows.Forms.TextBox Info_textBox;
        private System.Windows.Forms.Button Standby_btn;
        private System.Windows.Forms.DataVisualization.Charting.Chart Temp_chart;
        private System.Windows.Forms.ComboBox Process_comboBox;
        private System.Windows.Forms.Button SoftEnd_btn;
        private System.Windows.Forms.Label CheckTemp_label;
        private System.Windows.Forms.FolderBrowserDialog SelectRecipeFolder_fBD;
        private System.Windows.Forms.Button SelectRecipeFolder_btn;
        private System.Windows.Forms.Label RecipeFolder_label;
        private System.Windows.Forms.CheckBox UseFTP_checkBox;
        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage tabPage2;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button Zoom_btn;
        private System.Windows.Forms.Label Explanation_label;
        private System.Windows.Forms.DataGridViewTextBoxColumn Product;
        private System.Windows.Forms.DataGridViewTextBoxColumn LotNo;
        private System.Windows.Forms.DataGridViewTextBoxColumn Recipe;
        private System.Windows.Forms.Label operator_label;
        private System.Windows.Forms.TextBox operator_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabPage tabPage4;
        private System.Windows.Forms.GroupBox const_rb_gB;
        private System.Windows.Forms.RadioButton const60_radioButton;
        private System.Windows.Forms.RadioButton const150_radioButton;
        private System.Windows.Forms.Button const_start_button;
        private System.Windows.Forms.RadioButton const200_radioButton;
        private System.Windows.Forms.RadioButton const170_radioButton;
        private System.Windows.Forms.RadioButton const120_radioButton;
        private System.Windows.Forms.CheckBox UseMail_checkBox;
        private System.Windows.Forms.RadioButton const25_radioButton;
        private System.Windows.Forms.Label CheckStart_label;
        private System.Windows.Forms.Label pm2_label;
        private System.Windows.Forms.Label falcon_ng_label;
    }
}

