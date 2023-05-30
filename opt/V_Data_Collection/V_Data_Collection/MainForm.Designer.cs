namespace V_Data_Collection {
    partial class MainForm {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージド リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent() {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comport_comboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.connect_button = new System.Windows.Forms.Button();
            this.connect_label = new System.Windows.Forms.Label();
            this.timer1 = new System.Windows.Forms.Timer(this.components);
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.csv_Fld_label = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.ST1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ST2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ST3 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.zan_groupBox = new System.Windows.Forms.GroupBox();
            this.LL_radioButton = new System.Windows.Forms.RadioButton();
            this.UL_radioButton = new System.Windows.Forms.RadioButton();
            this.LR_radioButton = new System.Windows.Forms.RadioButton();
            this.UR_radioButton = new System.Windows.Forms.RadioButton();
            this.st_groupBox = new System.Windows.Forms.GroupBox();
            this.st3_radioButton = new System.Windows.Forms.RadioButton();
            this.st2_radioButton = new System.Windows.Forms.RadioButton();
            this.st1_radioButton = new System.Windows.Forms.RadioButton();
            this.input_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.save_panel = new System.Windows.Forms.Panel();
            this.depth_center_offset_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.depth_center_offset_label = new System.Windows.Forms.Label();
            this.depth_lower_offset_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.depth_lower_offset_label = new System.Windows.Forms.Label();
            this.depth_upper_offset_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.depth_upper_offset_label = new System.Windows.Forms.Label();
            this.cutpos_offset_label = new System.Windows.Forms.Label();
            this.cutpos_offset_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.remeasurement_label = new System.Windows.Forms.Label();
            this.remeasurement_check_panel = new System.Windows.Forms.Panel();
            this.ST3_remeasurement_checkBox = new System.Windows.Forms.CheckBox();
            this.ST1_remeasurement_checkBox = new System.Windows.Forms.CheckBox();
            this.ST2_remeasurement_checkBox = new System.Windows.Forms.CheckBox();
            this.lower_bradecut_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.lower_bradecut_label = new System.Windows.Forms.Label();
            this.upper_bradecut_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.save_button = new System.Windows.Forms.Button();
            this.upper_bradecut_label = new System.Windows.Forms.Label();
            this.g_ng_radioButton = new System.Windows.Forms.RadioButton();
            this.gaikan_label = new System.Windows.Forms.Label();
            this.g_ok_radioButton = new System.Windows.Forms.RadioButton();
            this.output_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.output_num_label = new System.Windows.Forms.Label();
            this.input_num_label = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.menuToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setting_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.csvfld_select_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.savefld_select_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.exit_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.save_fld_label = new System.Windows.Forms.Label();
            this.rec_start_button = new System.Windows.Forms.Button();
            this.rec_cancel_button = new System.Windows.Forms.Button();
            this.check_start_button = new System.Windows.Forms.Button();
            this.zan_delete_button = new System.Windows.Forms.Button();
            this.now_rec_label = new System.Windows.Forms.Label();
            this.empty_check_timer = new System.Windows.Forms.Timer(this.components);
            this.not_empty_label = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.zan_groupBox.SuspendLayout();
            this.st_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.input_numericUpDown)).BeginInit();
            this.save_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.depth_center_offset_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depth_lower_offset_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.depth_upper_offset_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cutpos_offset_numericUpDown)).BeginInit();
            this.remeasurement_check_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lower_bradecut_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.upper_bradecut_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.output_numericUpDown)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.BaudRate = 2400;
            this.serialPort1.Handshake = System.IO.Ports.Handshake.RequestToSend;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // comport_comboBox
            // 
            this.comport_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comport_comboBox.FormattingEnabled = true;
            this.comport_comboBox.Location = new System.Drawing.Point(5, 65);
            this.comport_comboBox.Name = "comport_comboBox";
            this.comport_comboBox.Size = new System.Drawing.Size(174, 20);
            this.comport_comboBox.TabIndex = 0;
            this.comport_comboBox.SelectedIndexChanged += new System.EventHandler(this.comport_comboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(5, 35);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(242, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "残厚測定器と接続しているシリアル通信ケーブルの\r\nCOMポートを選択して下さい。";
            // 
            // connect_button
            // 
            this.connect_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.connect_button.Location = new System.Drawing.Point(255, 35);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(70, 50);
            this.connect_button.TabIndex = 2;
            this.connect_button.Text = "接続";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // connect_label
            // 
            this.connect_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.connect_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.connect_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.connect_label.Location = new System.Drawing.Point(240, 90);
            this.connect_label.Name = "connect_label";
            this.connect_label.Size = new System.Drawing.Size(100, 50);
            this.connect_label.TabIndex = 3;
            this.connect_label.Text = "切断中";
            this.connect_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // timer1
            // 
            this.timer1.Interval = 3000;
            this.timer1.Tick += new System.EventHandler(this.timer1_tick_method);
            // 
            // csv_Fld_label
            // 
            this.csv_Fld_label.AutoSize = true;
            this.csv_Fld_label.Location = new System.Drawing.Point(345, 35);
            this.csv_Fld_label.Name = "csv_Fld_label";
            this.csv_Fld_label.Size = new System.Drawing.Size(71, 12);
            this.csv_Fld_label.TabIndex = 5;
            this.csv_Fld_label.Text = "csv_Fld_label";
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dataGridView1.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ST1,
            this.ST2,
            this.ST3});
            this.dataGridView1.Location = new System.Drawing.Point(5, 197);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.Size = new System.Drawing.Size(630, 445);
            this.dataGridView1.TabIndex = 6;
            // 
            // ST1
            // 
            this.ST1.HeaderText = "ST1";
            this.ST1.Name = "ST1";
            this.ST1.ReadOnly = true;
            this.ST1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ST1.Width = 31;
            // 
            // ST2
            // 
            this.ST2.HeaderText = "ST2";
            this.ST2.Name = "ST2";
            this.ST2.ReadOnly = true;
            this.ST2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ST2.Width = 31;
            // 
            // ST3
            // 
            this.ST3.HeaderText = "ST3";
            this.ST3.Name = "ST3";
            this.ST3.ReadOnly = true;
            this.ST3.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ST3.Width = 31;
            // 
            // zan_groupBox
            // 
            this.zan_groupBox.Controls.Add(this.LL_radioButton);
            this.zan_groupBox.Controls.Add(this.UL_radioButton);
            this.zan_groupBox.Controls.Add(this.LR_radioButton);
            this.zan_groupBox.Controls.Add(this.UR_radioButton);
            this.zan_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.zan_groupBox.Location = new System.Drawing.Point(90, 95);
            this.zan_groupBox.Name = "zan_groupBox";
            this.zan_groupBox.Size = new System.Drawing.Size(145, 75);
            this.zan_groupBox.TabIndex = 8;
            this.zan_groupBox.TabStop = false;
            this.zan_groupBox.Text = "残厚測定箇所";
            // 
            // LL_radioButton
            // 
            this.LL_radioButton.AutoSize = true;
            this.LL_radioButton.Location = new System.Drawing.Point(10, 50);
            this.LL_radioButton.Name = "LL_radioButton";
            this.LL_radioButton.Size = new System.Drawing.Size(58, 20);
            this.LL_radioButton.TabIndex = 3;
            this.LL_radioButton.TabStop = true;
            this.LL_radioButton.Text = "左下";
            this.LL_radioButton.UseVisualStyleBackColor = true;
            this.LL_radioButton.CheckedChanged += new System.EventHandler(this.zan_posi_radioButton_CheckedChanged);
            // 
            // UL_radioButton
            // 
            this.UL_radioButton.AutoSize = true;
            this.UL_radioButton.Location = new System.Drawing.Point(10, 20);
            this.UL_radioButton.Name = "UL_radioButton";
            this.UL_radioButton.Size = new System.Drawing.Size(58, 20);
            this.UL_radioButton.TabIndex = 2;
            this.UL_radioButton.TabStop = true;
            this.UL_radioButton.Text = "左上";
            this.UL_radioButton.UseVisualStyleBackColor = true;
            this.UL_radioButton.CheckedChanged += new System.EventHandler(this.zan_posi_radioButton_CheckedChanged);
            // 
            // LR_radioButton
            // 
            this.LR_radioButton.AutoSize = true;
            this.LR_radioButton.Location = new System.Drawing.Point(80, 50);
            this.LR_radioButton.Name = "LR_radioButton";
            this.LR_radioButton.Size = new System.Drawing.Size(58, 20);
            this.LR_radioButton.TabIndex = 1;
            this.LR_radioButton.TabStop = true;
            this.LR_radioButton.Text = "右下";
            this.LR_radioButton.UseVisualStyleBackColor = true;
            this.LR_radioButton.CheckedChanged += new System.EventHandler(this.zan_posi_radioButton_CheckedChanged);
            // 
            // UR_radioButton
            // 
            this.UR_radioButton.AutoSize = true;
            this.UR_radioButton.Location = new System.Drawing.Point(80, 20);
            this.UR_radioButton.Name = "UR_radioButton";
            this.UR_radioButton.Size = new System.Drawing.Size(58, 20);
            this.UR_radioButton.TabIndex = 0;
            this.UR_radioButton.TabStop = true;
            this.UR_radioButton.Text = "右上";
            this.UR_radioButton.UseVisualStyleBackColor = true;
            this.UR_radioButton.CheckedChanged += new System.EventHandler(this.zan_posi_radioButton_CheckedChanged);
            // 
            // st_groupBox
            // 
            this.st_groupBox.Controls.Add(this.st3_radioButton);
            this.st_groupBox.Controls.Add(this.st2_radioButton);
            this.st_groupBox.Controls.Add(this.st1_radioButton);
            this.st_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.st_groupBox.Location = new System.Drawing.Point(5, 95);
            this.st_groupBox.Name = "st_groupBox";
            this.st_groupBox.Size = new System.Drawing.Size(75, 100);
            this.st_groupBox.TabIndex = 10;
            this.st_groupBox.TabStop = false;
            this.st_groupBox.Text = "ST選択";
            // 
            // st3_radioButton
            // 
            this.st3_radioButton.AutoSize = true;
            this.st3_radioButton.Location = new System.Drawing.Point(5, 70);
            this.st3_radioButton.Name = "st3_radioButton";
            this.st3_radioButton.Size = new System.Drawing.Size(53, 20);
            this.st3_radioButton.TabIndex = 2;
            this.st3_radioButton.TabStop = true;
            this.st3_radioButton.Text = "ST3";
            this.st3_radioButton.UseVisualStyleBackColor = true;
            this.st3_radioButton.CheckedChanged += new System.EventHandler(this.st_radioButton_CheckedChanged);
            // 
            // st2_radioButton
            // 
            this.st2_radioButton.AutoSize = true;
            this.st2_radioButton.Location = new System.Drawing.Point(5, 45);
            this.st2_radioButton.Name = "st2_radioButton";
            this.st2_radioButton.Size = new System.Drawing.Size(53, 20);
            this.st2_radioButton.TabIndex = 1;
            this.st2_radioButton.TabStop = true;
            this.st2_radioButton.Text = "ST2";
            this.st2_radioButton.UseVisualStyleBackColor = true;
            this.st2_radioButton.CheckedChanged += new System.EventHandler(this.st_radioButton_CheckedChanged);
            // 
            // st1_radioButton
            // 
            this.st1_radioButton.AutoSize = true;
            this.st1_radioButton.Location = new System.Drawing.Point(5, 20);
            this.st1_radioButton.Name = "st1_radioButton";
            this.st1_radioButton.Size = new System.Drawing.Size(53, 20);
            this.st1_radioButton.TabIndex = 0;
            this.st1_radioButton.TabStop = true;
            this.st1_radioButton.Text = "ST1";
            this.st1_radioButton.UseVisualStyleBackColor = true;
            this.st1_radioButton.CheckedChanged += new System.EventHandler(this.st_radioButton_CheckedChanged);
            // 
            // input_numericUpDown
            // 
            this.input_numericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.input_numericUpDown.Location = new System.Drawing.Point(70, 2);
            this.input_numericUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.input_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.input_numericUpDown.Name = "input_numericUpDown";
            this.input_numericUpDown.Size = new System.Drawing.Size(48, 19);
            this.input_numericUpDown.TabIndex = 12;
            this.input_numericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // save_panel
            // 
            this.save_panel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.save_panel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.save_panel.Controls.Add(this.depth_center_offset_numericUpDown);
            this.save_panel.Controls.Add(this.depth_center_offset_label);
            this.save_panel.Controls.Add(this.depth_lower_offset_numericUpDown);
            this.save_panel.Controls.Add(this.depth_lower_offset_label);
            this.save_panel.Controls.Add(this.depth_upper_offset_numericUpDown);
            this.save_panel.Controls.Add(this.depth_upper_offset_label);
            this.save_panel.Controls.Add(this.cutpos_offset_label);
            this.save_panel.Controls.Add(this.cutpos_offset_numericUpDown);
            this.save_panel.Controls.Add(this.remeasurement_label);
            this.save_panel.Controls.Add(this.remeasurement_check_panel);
            this.save_panel.Controls.Add(this.lower_bradecut_numericUpDown);
            this.save_panel.Controls.Add(this.lower_bradecut_label);
            this.save_panel.Controls.Add(this.upper_bradecut_numericUpDown);
            this.save_panel.Controls.Add(this.save_button);
            this.save_panel.Controls.Add(this.upper_bradecut_label);
            this.save_panel.Controls.Add(this.g_ng_radioButton);
            this.save_panel.Controls.Add(this.gaikan_label);
            this.save_panel.Controls.Add(this.g_ok_radioButton);
            this.save_panel.Controls.Add(this.output_numericUpDown);
            this.save_panel.Controls.Add(this.output_num_label);
            this.save_panel.Controls.Add(this.input_num_label);
            this.save_panel.Controls.Add(this.input_numericUpDown);
            this.save_panel.Location = new System.Drawing.Point(640, 197);
            this.save_panel.Name = "save_panel";
            this.save_panel.Size = new System.Drawing.Size(200, 405);
            this.save_panel.TabIndex = 13;
            // 
            // depth_center_offset_numericUpDown
            // 
            this.depth_center_offset_numericUpDown.DecimalPlaces = 3;
            this.depth_center_offset_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.depth_center_offset_numericUpDown.Location = new System.Drawing.Point(110, 340);
            this.depth_center_offset_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.depth_center_offset_numericUpDown.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.depth_center_offset_numericUpDown.Name = "depth_center_offset_numericUpDown";
            this.depth_center_offset_numericUpDown.Size = new System.Drawing.Size(70, 19);
            this.depth_center_offset_numericUpDown.TabIndex = 33;
            // 
            // depth_center_offset_label
            // 
            this.depth_center_offset_label.AutoSize = true;
            this.depth_center_offset_label.Location = new System.Drawing.Point(5, 344);
            this.depth_center_offset_label.Name = "depth_center_offset_label";
            this.depth_center_offset_label.Size = new System.Drawing.Size(90, 12);
            this.depth_center_offset_label.TabIndex = 32;
            this.depth_center_offset_label.Text = "残厚センター補正";
            // 
            // depth_lower_offset_numericUpDown
            // 
            this.depth_lower_offset_numericUpDown.DecimalPlaces = 3;
            this.depth_lower_offset_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.depth_lower_offset_numericUpDown.Location = new System.Drawing.Point(110, 315);
            this.depth_lower_offset_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.depth_lower_offset_numericUpDown.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.depth_lower_offset_numericUpDown.Name = "depth_lower_offset_numericUpDown";
            this.depth_lower_offset_numericUpDown.Size = new System.Drawing.Size(70, 19);
            this.depth_lower_offset_numericUpDown.TabIndex = 31;
            // 
            // depth_lower_offset_label
            // 
            this.depth_lower_offset_label.AutoSize = true;
            this.depth_lower_offset_label.Location = new System.Drawing.Point(5, 320);
            this.depth_lower_offset_label.Name = "depth_lower_offset_label";
            this.depth_lower_offset_label.Size = new System.Drawing.Size(85, 12);
            this.depth_lower_offset_label.TabIndex = 30;
            this.depth_lower_offset_label.Text = "残厚補正(下刃)";
            // 
            // depth_upper_offset_numericUpDown
            // 
            this.depth_upper_offset_numericUpDown.DecimalPlaces = 3;
            this.depth_upper_offset_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.depth_upper_offset_numericUpDown.Location = new System.Drawing.Point(110, 290);
            this.depth_upper_offset_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.depth_upper_offset_numericUpDown.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.depth_upper_offset_numericUpDown.Name = "depth_upper_offset_numericUpDown";
            this.depth_upper_offset_numericUpDown.Size = new System.Drawing.Size(70, 19);
            this.depth_upper_offset_numericUpDown.TabIndex = 29;
            // 
            // depth_upper_offset_label
            // 
            this.depth_upper_offset_label.AutoSize = true;
            this.depth_upper_offset_label.Location = new System.Drawing.Point(5, 295);
            this.depth_upper_offset_label.Name = "depth_upper_offset_label";
            this.depth_upper_offset_label.Size = new System.Drawing.Size(85, 12);
            this.depth_upper_offset_label.TabIndex = 28;
            this.depth_upper_offset_label.Text = "残厚補正(上刃)";
            // 
            // cutpos_offset_label
            // 
            this.cutpos_offset_label.AutoSize = true;
            this.cutpos_offset_label.Location = new System.Drawing.Point(5, 270);
            this.cutpos_offset_label.Name = "cutpos_offset_label";
            this.cutpos_offset_label.Size = new System.Drawing.Size(95, 12);
            this.cutpos_offset_label.TabIndex = 27;
            this.cutpos_offset_label.Text = "切削位置オフセット";
            // 
            // cutpos_offset_numericUpDown
            // 
            this.cutpos_offset_numericUpDown.DecimalPlaces = 3;
            this.cutpos_offset_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.cutpos_offset_numericUpDown.Location = new System.Drawing.Point(110, 265);
            this.cutpos_offset_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.cutpos_offset_numericUpDown.Minimum = new decimal(new int[] {
            5000,
            0,
            0,
            -2147483648});
            this.cutpos_offset_numericUpDown.Name = "cutpos_offset_numericUpDown";
            this.cutpos_offset_numericUpDown.Size = new System.Drawing.Size(70, 19);
            this.cutpos_offset_numericUpDown.TabIndex = 26;
            // 
            // remeasurement_label
            // 
            this.remeasurement_label.AutoSize = true;
            this.remeasurement_label.Location = new System.Drawing.Point(5, 165);
            this.remeasurement_label.Name = "remeasurement_label";
            this.remeasurement_label.Size = new System.Drawing.Size(110, 24);
            this.remeasurement_label.TabIndex = 25;
            this.remeasurement_label.Text = "手動再測定確認\r\n(画像測定NG時のみ)\r\n";
            // 
            // remeasurement_check_panel
            // 
            this.remeasurement_check_panel.Controls.Add(this.ST3_remeasurement_checkBox);
            this.remeasurement_check_panel.Controls.Add(this.ST1_remeasurement_checkBox);
            this.remeasurement_check_panel.Controls.Add(this.ST2_remeasurement_checkBox);
            this.remeasurement_check_panel.Location = new System.Drawing.Point(1, 190);
            this.remeasurement_check_panel.Name = "remeasurement_check_panel";
            this.remeasurement_check_panel.Size = new System.Drawing.Size(125, 65);
            this.remeasurement_check_panel.TabIndex = 21;
            // 
            // ST3_remeasurement_checkBox
            // 
            this.ST3_remeasurement_checkBox.AutoSize = true;
            this.ST3_remeasurement_checkBox.Location = new System.Drawing.Point(3, 45);
            this.ST3_remeasurement_checkBox.Name = "ST3_remeasurement_checkBox";
            this.ST3_remeasurement_checkBox.Size = new System.Drawing.Size(63, 16);
            this.ST3_remeasurement_checkBox.TabIndex = 26;
            this.ST3_remeasurement_checkBox.Tag = "ST3";
            this.ST3_remeasurement_checkBox.Text = "ST3 OK";
            this.ST3_remeasurement_checkBox.UseVisualStyleBackColor = true;
            this.ST3_remeasurement_checkBox.CheckedChanged += new System.EventHandler(this.remeasurement_checkBox_CheckedChanged);
            // 
            // ST1_remeasurement_checkBox
            // 
            this.ST1_remeasurement_checkBox.AutoSize = true;
            this.ST1_remeasurement_checkBox.Location = new System.Drawing.Point(3, 5);
            this.ST1_remeasurement_checkBox.Name = "ST1_remeasurement_checkBox";
            this.ST1_remeasurement_checkBox.Size = new System.Drawing.Size(63, 16);
            this.ST1_remeasurement_checkBox.TabIndex = 24;
            this.ST1_remeasurement_checkBox.Tag = "ST1";
            this.ST1_remeasurement_checkBox.Text = "ST1 OK";
            this.ST1_remeasurement_checkBox.UseVisualStyleBackColor = true;
            this.ST1_remeasurement_checkBox.CheckedChanged += new System.EventHandler(this.remeasurement_checkBox_CheckedChanged);
            // 
            // ST2_remeasurement_checkBox
            // 
            this.ST2_remeasurement_checkBox.AutoSize = true;
            this.ST2_remeasurement_checkBox.Location = new System.Drawing.Point(3, 25);
            this.ST2_remeasurement_checkBox.Name = "ST2_remeasurement_checkBox";
            this.ST2_remeasurement_checkBox.Size = new System.Drawing.Size(63, 16);
            this.ST2_remeasurement_checkBox.TabIndex = 21;
            this.ST2_remeasurement_checkBox.Tag = "ST2";
            this.ST2_remeasurement_checkBox.Text = "ST2 OK";
            this.ST2_remeasurement_checkBox.UseVisualStyleBackColor = true;
            this.ST2_remeasurement_checkBox.CheckedChanged += new System.EventHandler(this.remeasurement_checkBox_CheckedChanged);
            // 
            // lower_bradecut_numericUpDown
            // 
            this.lower_bradecut_numericUpDown.DecimalPlaces = 3;
            this.lower_bradecut_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.lower_bradecut_numericUpDown.Location = new System.Drawing.Point(125, 135);
            this.lower_bradecut_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.lower_bradecut_numericUpDown.Name = "lower_bradecut_numericUpDown";
            this.lower_bradecut_numericUpDown.Size = new System.Drawing.Size(70, 19);
            this.lower_bradecut_numericUpDown.TabIndex = 23;
            // 
            // lower_bradecut_label
            // 
            this.lower_bradecut_label.AutoSize = true;
            this.lower_bradecut_label.Location = new System.Drawing.Point(5, 130);
            this.lower_bradecut_label.Name = "lower_bradecut_label";
            this.lower_bradecut_label.Size = new System.Drawing.Size(114, 24);
            this.lower_bradecut_label.TabIndex = 22;
            this.lower_bradecut_label.Text = "下刃ブレードカット距離\r\n(5000m以内)";
            // 
            // upper_bradecut_numericUpDown
            // 
            this.upper_bradecut_numericUpDown.DecimalPlaces = 3;
            this.upper_bradecut_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.upper_bradecut_numericUpDown.Location = new System.Drawing.Point(125, 100);
            this.upper_bradecut_numericUpDown.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.upper_bradecut_numericUpDown.Name = "upper_bradecut_numericUpDown";
            this.upper_bradecut_numericUpDown.Size = new System.Drawing.Size(70, 19);
            this.upper_bradecut_numericUpDown.TabIndex = 21;
            // 
            // save_button
            // 
            this.save_button.Enabled = false;
            this.save_button.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.save_button.Location = new System.Drawing.Point(25, 370);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(75, 30);
            this.save_button.TabIndex = 16;
            this.save_button.Text = "保存";
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // upper_bradecut_label
            // 
            this.upper_bradecut_label.AutoSize = true;
            this.upper_bradecut_label.Location = new System.Drawing.Point(5, 100);
            this.upper_bradecut_label.Name = "upper_bradecut_label";
            this.upper_bradecut_label.Size = new System.Drawing.Size(114, 24);
            this.upper_bradecut_label.TabIndex = 20;
            this.upper_bradecut_label.Text = "上刃ブレードカット距離\r\n(5000m以内)";
            // 
            // g_ng_radioButton
            // 
            this.g_ng_radioButton.AutoSize = true;
            this.g_ng_radioButton.Location = new System.Drawing.Point(65, 75);
            this.g_ng_radioButton.Name = "g_ng_radioButton";
            this.g_ng_radioButton.Size = new System.Drawing.Size(39, 16);
            this.g_ng_radioButton.TabIndex = 19;
            this.g_ng_radioButton.TabStop = true;
            this.g_ng_radioButton.Text = "NG";
            this.g_ng_radioButton.UseVisualStyleBackColor = true;
            this.g_ng_radioButton.CheckedChanged += new System.EventHandler(this.gaikan_radioButton_CheckedChanged);
            // 
            // gaikan_label
            // 
            this.gaikan_label.AutoSize = true;
            this.gaikan_label.Location = new System.Drawing.Point(5, 55);
            this.gaikan_label.Name = "gaikan_label";
            this.gaikan_label.Size = new System.Drawing.Size(101, 12);
            this.gaikan_label.TabIndex = 18;
            this.gaikan_label.Text = "外観状態確認結果";
            // 
            // g_ok_radioButton
            // 
            this.g_ok_radioButton.AutoSize = true;
            this.g_ok_radioButton.Location = new System.Drawing.Point(15, 75);
            this.g_ok_radioButton.Name = "g_ok_radioButton";
            this.g_ok_radioButton.Size = new System.Drawing.Size(38, 16);
            this.g_ok_radioButton.TabIndex = 17;
            this.g_ok_radioButton.TabStop = true;
            this.g_ok_radioButton.Text = "OK";
            this.g_ok_radioButton.UseVisualStyleBackColor = true;
            this.g_ok_radioButton.CheckedChanged += new System.EventHandler(this.gaikan_radioButton_CheckedChanged);
            // 
            // output_numericUpDown
            // 
            this.output_numericUpDown.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.output_numericUpDown.Location = new System.Drawing.Point(70, 27);
            this.output_numericUpDown.Maximum = new decimal(new int[] {
            200,
            0,
            0,
            0});
            this.output_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.output_numericUpDown.Name = "output_numericUpDown";
            this.output_numericUpDown.Size = new System.Drawing.Size(48, 19);
            this.output_numericUpDown.TabIndex = 15;
            this.output_numericUpDown.Value = new decimal(new int[] {
            25,
            0,
            0,
            0});
            // 
            // output_num_label
            // 
            this.output_num_label.AutoSize = true;
            this.output_num_label.Location = new System.Drawing.Point(5, 30);
            this.output_num_label.Name = "output_num_label";
            this.output_num_label.Size = new System.Drawing.Size(53, 12);
            this.output_num_label.TabIndex = 14;
            this.output_num_label.Text = "回収枚数";
            // 
            // input_num_label
            // 
            this.input_num_label.AutoSize = true;
            this.input_num_label.Location = new System.Drawing.Point(5, 5);
            this.input_num_label.Name = "input_num_label";
            this.input_num_label.Size = new System.Drawing.Size(53, 12);
            this.input_num_label.TabIndex = 13;
            this.input_num_label.Text = "投入枚数";
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(844, 24);
            this.menuStrip1.TabIndex = 14;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // menuToolStripMenuItem
            // 
            this.menuToolStripMenuItem.BackColor = System.Drawing.Color.Transparent;
            this.menuToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.setting_ToolStripMenuItem,
            this.exit_ToolStripMenuItem});
            this.menuToolStripMenuItem.Name = "menuToolStripMenuItem";
            this.menuToolStripMenuItem.Size = new System.Drawing.Size(50, 20);
            this.menuToolStripMenuItem.Text = "Menu";
            // 
            // setting_ToolStripMenuItem
            // 
            this.setting_ToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.csvfld_select_ToolStripMenuItem,
            this.savefld_select_ToolStripMenuItem});
            this.setting_ToolStripMenuItem.Name = "setting_ToolStripMenuItem";
            this.setting_ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.setting_ToolStripMenuItem.Text = "設定";
            // 
            // csvfld_select_ToolStripMenuItem
            // 
            this.csvfld_select_ToolStripMenuItem.Name = "csvfld_select_ToolStripMenuItem";
            this.csvfld_select_ToolStripMenuItem.Size = new System.Drawing.Size(292, 22);
            this.csvfld_select_ToolStripMenuItem.Text = "LM-1000からのcsvファイル受信フォルダ設定...";
            this.csvfld_select_ToolStripMenuItem.Click += new System.EventHandler(this.csv_fld_select_Click);
            // 
            // savefld_select_ToolStripMenuItem
            // 
            this.savefld_select_ToolStripMenuItem.Name = "savefld_select_ToolStripMenuItem";
            this.savefld_select_ToolStripMenuItem.Size = new System.Drawing.Size(292, 22);
            this.savefld_select_ToolStripMenuItem.Text = "データ保存フォルダ設定...";
            this.savefld_select_ToolStripMenuItem.Click += new System.EventHandler(this.save_fld_select_Click);
            // 
            // exit_ToolStripMenuItem
            // 
            this.exit_ToolStripMenuItem.Name = "exit_ToolStripMenuItem";
            this.exit_ToolStripMenuItem.Size = new System.Drawing.Size(98, 22);
            this.exit_ToolStripMenuItem.Text = "終了";
            this.exit_ToolStripMenuItem.Click += new System.EventHandler(this.FormClose_Click);
            // 
            // save_fld_label
            // 
            this.save_fld_label.AutoSize = true;
            this.save_fld_label.Location = new System.Drawing.Point(345, 70);
            this.save_fld_label.Name = "save_fld_label";
            this.save_fld_label.Size = new System.Drawing.Size(74, 12);
            this.save_fld_label.TabIndex = 15;
            this.save_fld_label.Text = "save_fld_label";
            // 
            // rec_start_button
            // 
            this.rec_start_button.BackColor = System.Drawing.SystemColors.Control;
            this.rec_start_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rec_start_button.Location = new System.Drawing.Point(370, 120);
            this.rec_start_button.Name = "rec_start_button";
            this.rec_start_button.Size = new System.Drawing.Size(100, 50);
            this.rec_start_button.TabIndex = 16;
            this.rec_start_button.Text = "記録開始";
            this.rec_start_button.UseVisualStyleBackColor = true;
            this.rec_start_button.Click += new System.EventHandler(this.rec_start_button_Click);
            // 
            // rec_cancel_button
            // 
            this.rec_cancel_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.rec_cancel_button.Location = new System.Drawing.Point(635, 115);
            this.rec_cancel_button.Name = "rec_cancel_button";
            this.rec_cancel_button.Size = new System.Drawing.Size(140, 60);
            this.rec_cancel_button.TabIndex = 17;
            this.rec_cancel_button.Text = "記録中止\r\n(全データ消去)";
            this.rec_cancel_button.UseVisualStyleBackColor = true;
            this.rec_cancel_button.Click += new System.EventHandler(this.rec_cancel_button_Click);
            // 
            // check_start_button
            // 
            this.check_start_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.check_start_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.check_start_button.Location = new System.Drawing.Point(743, 605);
            this.check_start_button.Name = "check_start_button";
            this.check_start_button.Size = new System.Drawing.Size(100, 40);
            this.check_start_button.TabIndex = 18;
            this.check_start_button.Text = "点検開始";
            this.check_start_button.UseVisualStyleBackColor = true;
            this.check_start_button.Click += new System.EventHandler(this.rec_start_button_Click);
            // 
            // zan_delete_button
            // 
            this.zan_delete_button.Location = new System.Drawing.Point(240, 150);
            this.zan_delete_button.Name = "zan_delete_button";
            this.zan_delete_button.Size = new System.Drawing.Size(100, 40);
            this.zan_delete_button.TabIndex = 19;
            this.zan_delete_button.Text = "指定箇所の\r\n残厚データ削除";
            this.zan_delete_button.UseVisualStyleBackColor = true;
            this.zan_delete_button.Click += new System.EventHandler(this.zan_delete_button_Click);
            // 
            // now_rec_label
            // 
            this.now_rec_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.now_rec_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.now_rec_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.now_rec_label.Location = new System.Drawing.Point(476, 120);
            this.now_rec_label.Name = "now_rec_label";
            this.now_rec_label.Size = new System.Drawing.Size(100, 50);
            this.now_rec_label.TabIndex = 20;
            this.now_rec_label.Text = "記録中";
            this.now_rec_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.now_rec_label.Visible = false;
            // 
            // empty_check_timer
            // 
            this.empty_check_timer.Interval = 3000;
            this.empty_check_timer.Tick += new System.EventHandler(this.empty_check_timer_Tick);
            // 
            // not_empty_label
            // 
            this.not_empty_label.BackColor = System.Drawing.Color.Red;
            this.not_empty_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.not_empty_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.not_empty_label.ForeColor = System.Drawing.Color.White;
            this.not_empty_label.Location = new System.Drawing.Point(302, 216);
            this.not_empty_label.Name = "not_empty_label";
            this.not_empty_label.Size = new System.Drawing.Size(320, 120);
            this.not_empty_label.TabIndex = 21;
            this.not_empty_label.Text = "csvデータ受信フォルダに\r\n既にデータファイルがあるため\r\n開始出来ません。";
            this.not_empty_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.not_empty_label.Visible = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(844, 646);
            this.ControlBox = false;
            this.Controls.Add(this.not_empty_label);
            this.Controls.Add(this.now_rec_label);
            this.Controls.Add(this.zan_delete_button);
            this.Controls.Add(this.check_start_button);
            this.Controls.Add(this.rec_cancel_button);
            this.Controls.Add(this.rec_start_button);
            this.Controls.Add(this.save_fld_label);
            this.Controls.Add(this.save_panel);
            this.Controls.Add(this.st_groupBox);
            this.Controls.Add(this.zan_groupBox);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.csv_Fld_label);
            this.Controls.Add(this.connect_label);
            this.Controls.Add(this.connect_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.comport_comboBox);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(800, 630);
            this.Name = "MainForm";
            this.Text = "V_Data_Collection(C#) version=1.3.0  2022/6/14";
            this.Load += new System.EventHandler(this.MainForm_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.zan_groupBox.ResumeLayout(false);
            this.zan_groupBox.PerformLayout();
            this.st_groupBox.ResumeLayout(false);
            this.st_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.input_numericUpDown)).EndInit();
            this.save_panel.ResumeLayout(false);
            this.save_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.depth_center_offset_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depth_lower_offset_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.depth_upper_offset_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cutpos_offset_numericUpDown)).EndInit();
            this.remeasurement_check_panel.ResumeLayout(false);
            this.remeasurement_check_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.lower_bradecut_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.upper_bradecut_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.output_numericUpDown)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comport_comboBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Label connect_label;
        private System.Windows.Forms.Timer timer1;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Label csv_Fld_label;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.GroupBox zan_groupBox;
        private System.Windows.Forms.RadioButton LL_radioButton;
        private System.Windows.Forms.RadioButton UL_radioButton;
        private System.Windows.Forms.RadioButton LR_radioButton;
        private System.Windows.Forms.RadioButton UR_radioButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST1;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST2;
        private System.Windows.Forms.DataGridViewTextBoxColumn ST3;
        private System.Windows.Forms.GroupBox st_groupBox;
        private System.Windows.Forms.RadioButton st3_radioButton;
        private System.Windows.Forms.RadioButton st2_radioButton;
        private System.Windows.Forms.RadioButton st1_radioButton;
        private System.Windows.Forms.NumericUpDown input_numericUpDown;
        private System.Windows.Forms.Panel save_panel;
        private System.Windows.Forms.NumericUpDown output_numericUpDown;
        private System.Windows.Forms.Label output_num_label;
        private System.Windows.Forms.Label input_num_label;
        private System.Windows.Forms.Button save_button;
        private System.Windows.Forms.RadioButton g_ok_radioButton;
        private System.Windows.Forms.Label gaikan_label;
        private System.Windows.Forms.RadioButton g_ng_radioButton;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem menuToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem setting_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exit_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem csvfld_select_ToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem savefld_select_ToolStripMenuItem;
        private System.Windows.Forms.Label save_fld_label;
        private System.Windows.Forms.Button rec_start_button;
        private System.Windows.Forms.Button rec_cancel_button;
        private System.Windows.Forms.Label upper_bradecut_label;
        private System.Windows.Forms.NumericUpDown upper_bradecut_numericUpDown;
        private System.Windows.Forms.Button check_start_button;
        private System.Windows.Forms.Button zan_delete_button;
        private System.Windows.Forms.Label now_rec_label;
        private System.Windows.Forms.NumericUpDown lower_bradecut_numericUpDown;
        private System.Windows.Forms.Label lower_bradecut_label;
        private System.Windows.Forms.CheckBox ST1_remeasurement_checkBox;
        private System.Windows.Forms.Label remeasurement_label;
        private System.Windows.Forms.CheckBox ST2_remeasurement_checkBox;
        private System.Windows.Forms.CheckBox ST3_remeasurement_checkBox;
        private System.Windows.Forms.Panel remeasurement_check_panel;
        private System.Windows.Forms.Timer empty_check_timer;
        private System.Windows.Forms.Label not_empty_label;
        private System.Windows.Forms.Label cutpos_offset_label;
        private System.Windows.Forms.NumericUpDown cutpos_offset_numericUpDown;
        private System.Windows.Forms.NumericUpDown depth_center_offset_numericUpDown;
        private System.Windows.Forms.Label depth_center_offset_label;
        private System.Windows.Forms.NumericUpDown depth_lower_offset_numericUpDown;
        private System.Windows.Forms.Label depth_lower_offset_label;
        private System.Windows.Forms.NumericUpDown depth_upper_offset_numericUpDown;
        private System.Windows.Forms.Label depth_upper_offset_label;
    }
}

