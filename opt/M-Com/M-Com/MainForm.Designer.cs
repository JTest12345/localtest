namespace M_Com {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comport_comboBox = new System.Windows.Forms.ComboBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.connect_label = new System.Windows.Forms.Label();
            this.FormClose_button = new System.Windows.Forms.Button();
            this.WB_item_groupBox = new System.Windows.Forms.GroupBox();
            this.clearance_Z_button = new System.Windows.Forms.Button();
            this.reverse_Y_button = new System.Windows.Forms.Button();
            this.loop_Z_button = new System.Windows.Forms.Button();
            this.ball_Z_button = new System.Windows.Forms.Button();
            this.ball_Y_button = new System.Windows.Forms.Button();
            this.ball_X_button = new System.Windows.Forms.Button();
            this.WB_place_groupBox = new System.Windows.Forms.GroupBox();
            this.zdbump_radioButton = new System.Windows.Forms.RadioButton();
            this.zd_radioButton = new System.Windows.Forms.RadioButton();
            this.lead_radioButton = new System.Windows.Forms.RadioButton();
            this.bump_radioButton = new System.Windows.Forms.RadioButton();
            this.cathode_radioButton = new System.Windows.Forms.RadioButton();
            this.anode_radioButton = new System.Windows.Forms.RadioButton();
            this.WB_measure_panel = new System.Windows.Forms.Panel();
            this.WB_chip_groupBox = new System.Windows.Forms.GroupBox();
            this.green_chip_radioButton = new System.Windows.Forms.RadioButton();
            this.red_chip_radioButton = new System.Windows.Forms.RadioButton();
            this.blue_chip_radioButton = new System.Windows.Forms.RadioButton();
            this.save_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.SelectSaveFolder_button = new System.Windows.Forms.Button();
            this.SelectSaveFolder_fBD = new System.Windows.Forms.FolderBrowserDialog();
            this.SaveFolder_label = new System.Windows.Forms.Label();
            this.ReadQR_textBox = new System.Windows.Forms.TextBox();
            this.product_label = new System.Windows.Forms.Label();
            this.lotno_label = new System.Windows.Forms.Label();
            this.operator_input_textBox = new System.Windows.Forms.TextBox();
            this.lotQR_label = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.operator_label = new System.Windows.Forms.Label();
            this.delete_row_button = new System.Windows.Forms.Button();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.place = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.item = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.data = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.measure_datetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.judge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allclear_button = new System.Windows.Forms.Button();
            this.SDR_measure_panel = new System.Windows.Forms.Panel();
            this.SDR_place_groupBox = new System.Windows.Forms.GroupBox();
            this.minus_radioButton = new System.Windows.Forms.RadioButton();
            this.plus_radioButton = new System.Windows.Forms.RadioButton();
            this.SDR_item_groupBox = new System.Windows.Forms.GroupBox();
            this.clearance_Y_button = new System.Windows.Forms.Button();
            this.inner_Y_button = new System.Windows.Forms.Button();
            this.outer_Y_button = new System.Windows.Forms.Button();
            this.height_Z_button = new System.Windows.Forms.Button();
            this.width_Y_button = new System.Windows.Forms.Button();
            this.lotInfo_clear_button = new System.Windows.Forms.Button();
            this.check_standard_label = new System.Windows.Forms.Label();
            this.ok_ng_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.machine_input_textBox = new System.Windows.Forms.TextBox();
            this.machine_label = new System.Windows.Forms.Label();
            this.measure_mode_groupBox = new System.Windows.Forms.GroupBox();
            this.SDR_mode_radioButton = new System.Windows.Forms.RadioButton();
            this.WB_mode_radioButton = new System.Windows.Forms.RadioButton();
            this.standard_label = new System.Windows.Forms.Label();
            this.measure_num_checkBox = new System.Windows.Forms.CheckBox();
            this.measure_num_check_label = new System.Windows.Forms.Label();
            this.num_ng_textBox = new System.Windows.Forms.TextBox();
            this.MeasureSerial_label = new System.Windows.Forms.Label();
            this.item_cnt_textBox = new System.Windows.Forms.TextBox();
            this.WB_item_groupBox.SuspendLayout();
            this.WB_place_groupBox.SuspendLayout();
            this.WB_measure_panel.SuspendLayout();
            this.WB_chip_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SDR_measure_panel.SuspendLayout();
            this.SDR_place_groupBox.SuspendLayout();
            this.SDR_item_groupBox.SuspendLayout();
            this.measure_mode_groupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.ReadTimeout = 5000;
            this.serialPort1.StopBits = System.IO.Ports.StopBits.Two;
            this.serialPort1.WriteTimeout = 5000;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // comport_comboBox
            // 
            this.comport_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comport_comboBox.FormattingEnabled = true;
            this.comport_comboBox.Location = new System.Drawing.Point(5, 31);
            this.comport_comboBox.Name = "comport_comboBox";
            this.comport_comboBox.Size = new System.Drawing.Size(121, 20);
            this.comport_comboBox.TabIndex = 0;
            this.comport_comboBox.SelectedIndexChanged += new System.EventHandler(this.comport_comboBox_SelectedIndexChanged);
            // 
            // connect_button
            // 
            this.connect_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.connect_button.Location = new System.Drawing.Point(130, 30);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(70, 50);
            this.connect_button.TabIndex = 1;
            this.connect_button.Text = "接続";
            this.connect_button.UseVisualStyleBackColor = true;
            this.connect_button.Click += new System.EventHandler(this.connect_button_Click);
            // 
            // connect_label
            // 
            this.connect_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(128)))), ((int)(((byte)(0)))));
            this.connect_label.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.connect_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.connect_label.Location = new System.Drawing.Point(205, 31);
            this.connect_label.Name = "connect_label";
            this.connect_label.Size = new System.Drawing.Size(100, 50);
            this.connect_label.TabIndex = 2;
            this.connect_label.Text = "切断中";
            this.connect_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormClose_button
            // 
            this.FormClose_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.FormClose_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormClose_button.Location = new System.Drawing.Point(1120, 3);
            this.FormClose_button.Name = "FormClose_button";
            this.FormClose_button.Size = new System.Drawing.Size(60, 50);
            this.FormClose_button.TabIndex = 3;
            this.FormClose_button.Text = "ソフト\r\n終了";
            this.FormClose_button.UseVisualStyleBackColor = true;
            this.FormClose_button.Click += new System.EventHandler(this.FormClose_button_Click);
            // 
            // WB_item_groupBox
            // 
            this.WB_item_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.WB_item_groupBox.Controls.Add(this.clearance_Z_button);
            this.WB_item_groupBox.Controls.Add(this.reverse_Y_button);
            this.WB_item_groupBox.Controls.Add(this.loop_Z_button);
            this.WB_item_groupBox.Controls.Add(this.ball_Z_button);
            this.WB_item_groupBox.Controls.Add(this.ball_Y_button);
            this.WB_item_groupBox.Controls.Add(this.ball_X_button);
            this.WB_item_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WB_item_groupBox.Location = new System.Drawing.Point(160, 62);
            this.WB_item_groupBox.Name = "WB_item_groupBox";
            this.WB_item_groupBox.Size = new System.Drawing.Size(140, 320);
            this.WB_item_groupBox.TabIndex = 5;
            this.WB_item_groupBox.TabStop = false;
            this.WB_item_groupBox.Text = "測定項目";
            // 
            // clearance_Z_button
            // 
            this.clearance_Z_button.Location = new System.Drawing.Point(5, 280);
            this.clearance_Z_button.Name = "clearance_Z_button";
            this.clearance_Z_button.Size = new System.Drawing.Size(120, 36);
            this.clearance_Z_button.TabIndex = 5;
            this.clearance_Z_button.Tag = "QZ";
            this.clearance_Z_button.Text = "クリアランス Z";
            this.clearance_Z_button.UseVisualStyleBackColor = true;
            this.clearance_Z_button.Click += new System.EventHandler(this.GetData);
            // 
            // reverse_Y_button
            // 
            this.reverse_Y_button.Location = new System.Drawing.Point(5, 230);
            this.reverse_Y_button.Name = "reverse_Y_button";
            this.reverse_Y_button.Size = new System.Drawing.Size(120, 36);
            this.reverse_Y_button.TabIndex = 4;
            this.reverse_Y_button.Tag = "QY";
            this.reverse_Y_button.Text = "リバース量 Y";
            this.reverse_Y_button.UseVisualStyleBackColor = true;
            this.reverse_Y_button.Click += new System.EventHandler(this.GetData);
            // 
            // loop_Z_button
            // 
            this.loop_Z_button.Location = new System.Drawing.Point(5, 180);
            this.loop_Z_button.Name = "loop_Z_button";
            this.loop_Z_button.Size = new System.Drawing.Size(120, 36);
            this.loop_Z_button.TabIndex = 3;
            this.loop_Z_button.Tag = "QZ";
            this.loop_Z_button.Text = "ループ高さ Z";
            this.loop_Z_button.UseVisualStyleBackColor = true;
            this.loop_Z_button.Click += new System.EventHandler(this.GetData);
            // 
            // ball_Z_button
            // 
            this.ball_Z_button.Location = new System.Drawing.Point(5, 130);
            this.ball_Z_button.Name = "ball_Z_button";
            this.ball_Z_button.Size = new System.Drawing.Size(120, 36);
            this.ball_Z_button.TabIndex = 2;
            this.ball_Z_button.Tag = "QZ";
            this.ball_Z_button.Text = "ボール厚 Z";
            this.ball_Z_button.UseVisualStyleBackColor = true;
            this.ball_Z_button.Click += new System.EventHandler(this.GetData);
            // 
            // ball_Y_button
            // 
            this.ball_Y_button.Location = new System.Drawing.Point(6, 80);
            this.ball_Y_button.Name = "ball_Y_button";
            this.ball_Y_button.Size = new System.Drawing.Size(120, 36);
            this.ball_Y_button.TabIndex = 1;
            this.ball_Y_button.Tag = "QY";
            this.ball_Y_button.Text = "ボール径 Y";
            this.ball_Y_button.UseVisualStyleBackColor = true;
            this.ball_Y_button.Click += new System.EventHandler(this.GetData);
            // 
            // ball_X_button
            // 
            this.ball_X_button.Location = new System.Drawing.Point(5, 30);
            this.ball_X_button.Name = "ball_X_button";
            this.ball_X_button.Size = new System.Drawing.Size(120, 36);
            this.ball_X_button.TabIndex = 0;
            this.ball_X_button.Tag = "QX";
            this.ball_X_button.Text = "ボール径 X";
            this.ball_X_button.UseVisualStyleBackColor = true;
            this.ball_X_button.Click += new System.EventHandler(this.GetData);
            // 
            // WB_place_groupBox
            // 
            this.WB_place_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.WB_place_groupBox.Controls.Add(this.zdbump_radioButton);
            this.WB_place_groupBox.Controls.Add(this.zd_radioButton);
            this.WB_place_groupBox.Controls.Add(this.lead_radioButton);
            this.WB_place_groupBox.Controls.Add(this.bump_radioButton);
            this.WB_place_groupBox.Controls.Add(this.cathode_radioButton);
            this.WB_place_groupBox.Controls.Add(this.anode_radioButton);
            this.WB_place_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WB_place_groupBox.Location = new System.Drawing.Point(5, 62);
            this.WB_place_groupBox.Name = "WB_place_groupBox";
            this.WB_place_groupBox.Size = new System.Drawing.Size(150, 210);
            this.WB_place_groupBox.TabIndex = 6;
            this.WB_place_groupBox.TabStop = false;
            this.WB_place_groupBox.Text = "測定箇所";
            // 
            // zdbump_radioButton
            // 
            this.zdbump_radioButton.AutoSize = true;
            this.zdbump_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.zdbump_radioButton.Location = new System.Drawing.Point(5, 180);
            this.zdbump_radioButton.Name = "zdbump_radioButton";
            this.zdbump_radioButton.Size = new System.Drawing.Size(145, 23);
            this.zdbump_radioButton.TabIndex = 5;
            this.zdbump_radioButton.TabStop = true;
            this.zdbump_radioButton.Tag = "NotRGB";
            this.zdbump_radioButton.Text = "ZDBump (Z-Z)";
            this.zdbump_radioButton.UseVisualStyleBackColor = true;
            this.zdbump_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // zd_radioButton
            // 
            this.zd_radioButton.AutoSize = true;
            this.zd_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.zd_radioButton.Location = new System.Drawing.Point(5, 150);
            this.zd_radioButton.Name = "zd_radioButton";
            this.zd_radioButton.Size = new System.Drawing.Size(50, 23);
            this.zd_radioButton.TabIndex = 4;
            this.zd_radioButton.TabStop = true;
            this.zd_radioButton.Tag = "NotRGB";
            this.zd_radioButton.Text = "ZD";
            this.zd_radioButton.UseVisualStyleBackColor = true;
            this.zd_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // lead_radioButton
            // 
            this.lead_radioButton.AutoSize = true;
            this.lead_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lead_radioButton.Location = new System.Drawing.Point(5, 120);
            this.lead_radioButton.Name = "lead_radioButton";
            this.lead_radioButton.Size = new System.Drawing.Size(120, 23);
            this.lead_radioButton.TabIndex = 3;
            this.lead_radioButton.TabStop = true;
            this.lead_radioButton.Tag = "NotRGB";
            this.lead_radioButton.Text = "Lead-Bump";
            this.lead_radioButton.UseVisualStyleBackColor = true;
            this.lead_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // bump_radioButton
            // 
            this.bump_radioButton.AutoSize = true;
            this.bump_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bump_radioButton.Location = new System.Drawing.Point(5, 90);
            this.bump_radioButton.Name = "bump_radioButton";
            this.bump_radioButton.Size = new System.Drawing.Size(124, 23);
            this.bump_radioButton.TabIndex = 2;
            this.bump_radioButton.TabStop = true;
            this.bump_radioButton.Text = "Bump (P-P)";
            this.bump_radioButton.UseVisualStyleBackColor = true;
            this.bump_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // cathode_radioButton
            // 
            this.cathode_radioButton.AutoSize = true;
            this.cathode_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cathode_radioButton.Location = new System.Drawing.Point(5, 60);
            this.cathode_radioButton.Name = "cathode_radioButton";
            this.cathode_radioButton.Size = new System.Drawing.Size(95, 23);
            this.cathode_radioButton.TabIndex = 1;
            this.cathode_radioButton.TabStop = true;
            this.cathode_radioButton.Text = "Cathode";
            this.cathode_radioButton.UseVisualStyleBackColor = true;
            this.cathode_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // anode_radioButton
            // 
            this.anode_radioButton.AutoSize = true;
            this.anode_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.anode_radioButton.Location = new System.Drawing.Point(5, 30);
            this.anode_radioButton.Name = "anode_radioButton";
            this.anode_radioButton.Size = new System.Drawing.Size(78, 23);
            this.anode_radioButton.TabIndex = 0;
            this.anode_radioButton.TabStop = true;
            this.anode_radioButton.Text = "Anode";
            this.anode_radioButton.UseVisualStyleBackColor = true;
            this.anode_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // WB_measure_panel
            // 
            this.WB_measure_panel.Controls.Add(this.WB_chip_groupBox);
            this.WB_measure_panel.Controls.Add(this.WB_place_groupBox);
            this.WB_measure_panel.Controls.Add(this.WB_item_groupBox);
            this.WB_measure_panel.Enabled = false;
            this.WB_measure_panel.Location = new System.Drawing.Point(5, 165);
            this.WB_measure_panel.Name = "WB_measure_panel";
            this.WB_measure_panel.Size = new System.Drawing.Size(310, 385);
            this.WB_measure_panel.TabIndex = 7;
            this.WB_measure_panel.Visible = false;
            // 
            // WB_chip_groupBox
            // 
            this.WB_chip_groupBox.BackColor = System.Drawing.SystemColors.Control;
            this.WB_chip_groupBox.Controls.Add(this.green_chip_radioButton);
            this.WB_chip_groupBox.Controls.Add(this.red_chip_radioButton);
            this.WB_chip_groupBox.Controls.Add(this.blue_chip_radioButton);
            this.WB_chip_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WB_chip_groupBox.Location = new System.Drawing.Point(5, 5);
            this.WB_chip_groupBox.Name = "WB_chip_groupBox";
            this.WB_chip_groupBox.Size = new System.Drawing.Size(300, 55);
            this.WB_chip_groupBox.TabIndex = 7;
            this.WB_chip_groupBox.TabStop = false;
            this.WB_chip_groupBox.Text = "測定素子";
            // 
            // green_chip_radioButton
            // 
            this.green_chip_radioButton.AutoSize = true;
            this.green_chip_radioButton.BackColor = System.Drawing.Color.Green;
            this.green_chip_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.green_chip_radioButton.ForeColor = System.Drawing.Color.Black;
            this.green_chip_radioButton.Location = new System.Drawing.Point(185, 25);
            this.green_chip_radioButton.Name = "green_chip_radioButton";
            this.green_chip_radioButton.Size = new System.Drawing.Size(77, 23);
            this.green_chip_radioButton.TabIndex = 3;
            this.green_chip_radioButton.TabStop = true;
            this.green_chip_radioButton.Text = "Green";
            this.green_chip_radioButton.UseVisualStyleBackColor = false;
            this.green_chip_radioButton.CheckedChanged += new System.EventHandler(this.chip_radioButton_CheckedChanged);
            // 
            // red_chip_radioButton
            // 
            this.red_chip_radioButton.AutoSize = true;
            this.red_chip_radioButton.BackColor = System.Drawing.Color.Red;
            this.red_chip_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.red_chip_radioButton.ForeColor = System.Drawing.Color.Black;
            this.red_chip_radioButton.Location = new System.Drawing.Point(95, 25);
            this.red_chip_radioButton.Name = "red_chip_radioButton";
            this.red_chip_radioButton.Size = new System.Drawing.Size(58, 23);
            this.red_chip_radioButton.TabIndex = 2;
            this.red_chip_radioButton.TabStop = true;
            this.red_chip_radioButton.Text = "Red";
            this.red_chip_radioButton.UseVisualStyleBackColor = false;
            this.red_chip_radioButton.CheckedChanged += new System.EventHandler(this.chip_radioButton_CheckedChanged);
            // 
            // blue_chip_radioButton
            // 
            this.blue_chip_radioButton.AutoSize = true;
            this.blue_chip_radioButton.BackColor = System.Drawing.Color.Blue;
            this.blue_chip_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.blue_chip_radioButton.ForeColor = System.Drawing.Color.Black;
            this.blue_chip_radioButton.Location = new System.Drawing.Point(5, 25);
            this.blue_chip_radioButton.Name = "blue_chip_radioButton";
            this.blue_chip_radioButton.Size = new System.Drawing.Size(63, 23);
            this.blue_chip_radioButton.TabIndex = 1;
            this.blue_chip_radioButton.TabStop = true;
            this.blue_chip_radioButton.Text = "Blue";
            this.blue_chip_radioButton.UseVisualStyleBackColor = false;
            this.blue_chip_radioButton.CheckedChanged += new System.EventHandler(this.chip_radioButton_CheckedChanged);
            // 
            // save_button
            // 
            this.save_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.save_button.Location = new System.Drawing.Point(5, 555);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(80, 50);
            this.save_button.TabIndex = 10;
            this.save_button.Text = "保存";
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(5, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(190, 15);
            this.label1.TabIndex = 8;
            this.label1.Text = "COMポートを選択してください。";
            // 
            // SelectSaveFolder_button
            // 
            this.SelectSaveFolder_button.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SelectSaveFolder_button.Location = new System.Drawing.Point(5, 105);
            this.SelectSaveFolder_button.Name = "SelectSaveFolder_button";
            this.SelectSaveFolder_button.Size = new System.Drawing.Size(100, 50);
            this.SelectSaveFolder_button.TabIndex = 10;
            this.SelectSaveFolder_button.Text = "データ保存\r\nフォルダ選択";
            this.SelectSaveFolder_button.UseVisualStyleBackColor = true;
            this.SelectSaveFolder_button.Click += new System.EventHandler(this.SelectSaveFolder_button_Click);
            // 
            // SelectSaveFolder_fBD
            // 
            this.SelectSaveFolder_fBD.Description = "データを保存するフォルダを選択してください。";
            // 
            // SaveFolder_label
            // 
            this.SaveFolder_label.AutoSize = true;
            this.SaveFolder_label.Location = new System.Drawing.Point(5, 87);
            this.SaveFolder_label.Name = "SaveFolder_label";
            this.SaveFolder_label.Size = new System.Drawing.Size(35, 12);
            this.SaveFolder_label.TabIndex = 11;
            this.SaveFolder_label.Text = "label2";
            // 
            // ReadQR_textBox
            // 
            this.ReadQR_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReadQR_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ReadQR_textBox.Location = new System.Drawing.Point(321, 18);
            this.ReadQR_textBox.Name = "ReadQR_textBox";
            this.ReadQR_textBox.Size = new System.Drawing.Size(100, 23);
            this.ReadQR_textBox.TabIndex = 12;
            this.ReadQR_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReadQR_textBox_KeyPress);
            // 
            // product_label
            // 
            this.product_label.AutoSize = true;
            this.product_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.product_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.product_label.Location = new System.Drawing.Point(321, 45);
            this.product_label.Name = "product_label";
            this.product_label.Size = new System.Drawing.Size(62, 16);
            this.product_label.TabIndex = 13;
            this.product_label.Text = "Product";
            // 
            // lotno_label
            // 
            this.lotno_label.AutoSize = true;
            this.lotno_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lotno_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lotno_label.Location = new System.Drawing.Point(322, 65);
            this.lotno_label.Name = "lotno_label";
            this.lotno_label.Size = new System.Drawing.Size(54, 16);
            this.lotno_label.TabIndex = 14;
            this.lotno_label.Text = "Lot No";
            // 
            // operator_input_textBox
            // 
            this.operator_input_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.operator_input_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.operator_input_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.operator_input_textBox.Location = new System.Drawing.Point(540, 18);
            this.operator_input_textBox.Name = "operator_input_textBox";
            this.operator_input_textBox.Size = new System.Drawing.Size(100, 23);
            this.operator_input_textBox.TabIndex = 16;
            this.operator_input_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.operator_input_textBox_KeyPress);
            // 
            // lotQR_label
            // 
            this.lotQR_label.AutoSize = true;
            this.lotQR_label.Location = new System.Drawing.Point(320, 3);
            this.lotQR_label.Name = "lotQR_label";
            this.lotQR_label.Size = new System.Drawing.Size(108, 12);
            this.lotQR_label.TabIndex = 17;
            this.lotQR_label.Text = "ロット管理表QRコード";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(540, 3);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(68, 12);
            this.label3.TabIndex = 18;
            this.label3.Text = "測定者コード";
            // 
            // operator_label
            // 
            this.operator_label.AutoSize = true;
            this.operator_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.operator_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.operator_label.Location = new System.Drawing.Point(646, 22);
            this.operator_label.Name = "operator_label";
            this.operator_label.Size = new System.Drawing.Size(69, 16);
            this.operator_label.TabIndex = 19;
            this.operator_label.Text = "Operator";
            // 
            // delete_row_button
            // 
            this.delete_row_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.delete_row_button.Location = new System.Drawing.Point(905, 105);
            this.delete_row_button.Name = "delete_row_button";
            this.delete_row_button.Size = new System.Drawing.Size(80, 50);
            this.delete_row_button.TabIndex = 20;
            this.delete_row_button.Text = "行削除";
            this.delete_row_button.UseVisualStyleBackColor = true;
            this.delete_row_button.Click += new System.EventHandler(this.delete_row_button_Click);
            // 
            // dataGridView1
            // 
            this.dataGridView1.AllowUserToAddRows = false;
            this.dataGridView1.AllowUserToDeleteRows = false;
            this.dataGridView1.AllowUserToResizeRows = false;
            this.dataGridView1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.dataGridView1.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.place,
            this.item,
            this.data,
            this.measure_datetime,
            this.judge});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(128)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridView1.DefaultCellStyle = dataGridViewCellStyle2;
            this.dataGridView1.Location = new System.Drawing.Point(320, 135);
            this.dataGridView1.Name = "dataGridView1";
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle3.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle3.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle3.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.dataGridView1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.AutoSizeToAllHeaders;
            this.dataGridView1.RowTemplate.Height = 21;
            this.dataGridView1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridView1.Size = new System.Drawing.Size(580, 542);
            this.dataGridView1.TabIndex = 21;
            // 
            // place
            // 
            this.place.HeaderText = "測定箇所";
            this.place.Name = "place";
            this.place.ReadOnly = true;
            this.place.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.place.Width = 78;
            // 
            // item
            // 
            this.item.HeaderText = "測定項目";
            this.item.Name = "item";
            this.item.ReadOnly = true;
            this.item.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.item.Width = 78;
            // 
            // data
            // 
            this.data.HeaderText = "測定値[μm]";
            this.data.Name = "data";
            this.data.ReadOnly = true;
            this.data.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            // 
            // measure_datetime
            // 
            this.measure_datetime.HeaderText = "測定日時";
            this.measure_datetime.Name = "measure_datetime";
            this.measure_datetime.ReadOnly = true;
            this.measure_datetime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.measure_datetime.Width = 78;
            // 
            // judge
            // 
            this.judge.HeaderText = "判定";
            this.judge.Name = "judge";
            this.judge.ReadOnly = true;
            this.judge.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.judge.Width = 46;
            // 
            // allclear_button
            // 
            this.allclear_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.allclear_button.Location = new System.Drawing.Point(1040, 105);
            this.allclear_button.Name = "allclear_button";
            this.allclear_button.Size = new System.Drawing.Size(80, 50);
            this.allclear_button.TabIndex = 22;
            this.allclear_button.Text = "全消去";
            this.allclear_button.UseVisualStyleBackColor = true;
            this.allclear_button.Click += new System.EventHandler(this.allclear_button_Click);
            // 
            // SDR_measure_panel
            // 
            this.SDR_measure_panel.Controls.Add(this.SDR_place_groupBox);
            this.SDR_measure_panel.Controls.Add(this.SDR_item_groupBox);
            this.SDR_measure_panel.Enabled = false;
            this.SDR_measure_panel.Location = new System.Drawing.Point(372, 165);
            this.SDR_measure_panel.Name = "SDR_measure_panel";
            this.SDR_measure_panel.Size = new System.Drawing.Size(310, 285);
            this.SDR_measure_panel.TabIndex = 23;
            this.SDR_measure_panel.Visible = false;
            // 
            // SDR_place_groupBox
            // 
            this.SDR_place_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.SDR_place_groupBox.Controls.Add(this.minus_radioButton);
            this.SDR_place_groupBox.Controls.Add(this.plus_radioButton);
            this.SDR_place_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SDR_place_groupBox.Location = new System.Drawing.Point(5, 10);
            this.SDR_place_groupBox.Name = "SDR_place_groupBox";
            this.SDR_place_groupBox.Size = new System.Drawing.Size(150, 210);
            this.SDR_place_groupBox.TabIndex = 6;
            this.SDR_place_groupBox.TabStop = false;
            this.SDR_place_groupBox.Text = "測定箇所";
            // 
            // minus_radioButton
            // 
            this.minus_radioButton.AutoSize = true;
            this.minus_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.minus_radioButton.Location = new System.Drawing.Point(5, 60);
            this.minus_radioButton.Name = "minus_radioButton";
            this.minus_radioButton.Size = new System.Drawing.Size(104, 23);
            this.minus_radioButton.TabIndex = 1;
            this.minus_radioButton.TabStop = true;
            this.minus_radioButton.Text = "マイナス側";
            this.minus_radioButton.UseVisualStyleBackColor = true;
            this.minus_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // plus_radioButton
            // 
            this.plus_radioButton.AutoSize = true;
            this.plus_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.plus_radioButton.Location = new System.Drawing.Point(5, 30);
            this.plus_radioButton.Name = "plus_radioButton";
            this.plus_radioButton.Size = new System.Drawing.Size(88, 23);
            this.plus_radioButton.TabIndex = 0;
            this.plus_radioButton.TabStop = true;
            this.plus_radioButton.Text = "プラス側";
            this.plus_radioButton.UseVisualStyleBackColor = true;
            this.plus_radioButton.CheckedChanged += new System.EventHandler(this.place_radioButton_CheckedChanged);
            // 
            // SDR_item_groupBox
            // 
            this.SDR_item_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.SDR_item_groupBox.Controls.Add(this.clearance_Y_button);
            this.SDR_item_groupBox.Controls.Add(this.inner_Y_button);
            this.SDR_item_groupBox.Controls.Add(this.outer_Y_button);
            this.SDR_item_groupBox.Controls.Add(this.height_Z_button);
            this.SDR_item_groupBox.Controls.Add(this.width_Y_button);
            this.SDR_item_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SDR_item_groupBox.Location = new System.Drawing.Point(160, 10);
            this.SDR_item_groupBox.Name = "SDR_item_groupBox";
            this.SDR_item_groupBox.Size = new System.Drawing.Size(140, 270);
            this.SDR_item_groupBox.TabIndex = 5;
            this.SDR_item_groupBox.TabStop = false;
            this.SDR_item_groupBox.Text = "測定項目";
            // 
            // clearance_Y_button
            // 
            this.clearance_Y_button.Location = new System.Drawing.Point(6, 230);
            this.clearance_Y_button.Name = "clearance_Y_button";
            this.clearance_Y_button.Size = new System.Drawing.Size(120, 36);
            this.clearance_Y_button.TabIndex = 4;
            this.clearance_Y_button.Tag = "QY";
            this.clearance_Y_button.Text = "クリアランス Y";
            this.clearance_Y_button.UseVisualStyleBackColor = true;
            this.clearance_Y_button.Click += new System.EventHandler(this.GetData);
            // 
            // inner_Y_button
            // 
            this.inner_Y_button.Location = new System.Drawing.Point(6, 180);
            this.inner_Y_button.Name = "inner_Y_button";
            this.inner_Y_button.Size = new System.Drawing.Size(120, 36);
            this.inner_Y_button.TabIndex = 3;
            this.inner_Y_button.Tag = "QY";
            this.inner_Y_button.Text = "内径 Y";
            this.inner_Y_button.UseVisualStyleBackColor = true;
            this.inner_Y_button.Click += new System.EventHandler(this.GetData);
            // 
            // outer_Y_button
            // 
            this.outer_Y_button.Location = new System.Drawing.Point(6, 130);
            this.outer_Y_button.Name = "outer_Y_button";
            this.outer_Y_button.Size = new System.Drawing.Size(120, 36);
            this.outer_Y_button.TabIndex = 2;
            this.outer_Y_button.Tag = "QY";
            this.outer_Y_button.Text = "外径 Y";
            this.outer_Y_button.UseVisualStyleBackColor = true;
            this.outer_Y_button.Click += new System.EventHandler(this.GetData);
            // 
            // height_Z_button
            // 
            this.height_Z_button.Location = new System.Drawing.Point(6, 80);
            this.height_Z_button.Name = "height_Z_button";
            this.height_Z_button.Size = new System.Drawing.Size(120, 36);
            this.height_Z_button.TabIndex = 1;
            this.height_Z_button.Tag = "QZ";
            this.height_Z_button.Text = "高さ Z";
            this.height_Z_button.UseVisualStyleBackColor = true;
            this.height_Z_button.Click += new System.EventHandler(this.GetData);
            // 
            // width_Y_button
            // 
            this.width_Y_button.Location = new System.Drawing.Point(5, 30);
            this.width_Y_button.Name = "width_Y_button";
            this.width_Y_button.Size = new System.Drawing.Size(120, 36);
            this.width_Y_button.TabIndex = 0;
            this.width_Y_button.Tag = "QY";
            this.width_Y_button.Text = "幅 Y";
            this.width_Y_button.UseVisualStyleBackColor = true;
            this.width_Y_button.Click += new System.EventHandler(this.GetData);
            // 
            // lotInfo_clear_button
            // 
            this.lotInfo_clear_button.AutoSize = true;
            this.lotInfo_clear_button.Location = new System.Drawing.Point(425, 17);
            this.lotInfo_clear_button.Name = "lotInfo_clear_button";
            this.lotInfo_clear_button.Size = new System.Drawing.Size(87, 26);
            this.lotInfo_clear_button.TabIndex = 24;
            this.lotInfo_clear_button.Text = "ロット情報消去";
            this.lotInfo_clear_button.UseVisualStyleBackColor = true;
            this.lotInfo_clear_button.Click += new System.EventHandler(this.lotInfo_clear_button_Click);
            // 
            // check_standard_label
            // 
            this.check_standard_label.AutoSize = true;
            this.check_standard_label.BackColor = System.Drawing.Color.Red;
            this.check_standard_label.Font = new System.Drawing.Font("MS UI Gothic", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.check_standard_label.ForeColor = System.Drawing.Color.White;
            this.check_standard_label.Location = new System.Drawing.Point(115, 115);
            this.check_standard_label.Name = "check_standard_label";
            this.check_standard_label.Size = new System.Drawing.Size(193, 39);
            this.check_standard_label.TabIndex = 25;
            this.check_standard_label.Text = "この機種には\r\n上下限値マスタがありません。\r\n規格値内かしっかり確認して下さい。";
            // 
            // ok_ng_label
            // 
            this.ok_ng_label.AutoSize = true;
            this.ok_ng_label.BackColor = System.Drawing.Color.Red;
            this.ok_ng_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ok_ng_label.ForeColor = System.Drawing.Color.White;
            this.ok_ng_label.Location = new System.Drawing.Point(100, 560);
            this.ok_ng_label.Name = "ok_ng_label";
            this.ok_ng_label.Size = new System.Drawing.Size(121, 19);
            this.ok_ng_label.TabIndex = 26;
            this.ok_ng_label.Text = "NGがあります。";
            this.ok_ng_label.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(735, 3);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(43, 12);
            this.label4.TabIndex = 27;
            this.label4.Text = "設備No";
            // 
            // machine_input_textBox
            // 
            this.machine_input_textBox.BackColor = System.Drawing.Color.SkyBlue;
            this.machine_input_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.machine_input_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.machine_input_textBox.Location = new System.Drawing.Point(735, 18);
            this.machine_input_textBox.Name = "machine_input_textBox";
            this.machine_input_textBox.Size = new System.Drawing.Size(100, 23);
            this.machine_input_textBox.TabIndex = 28;
            this.machine_input_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.machine_input_textBox_KeyPress);
            // 
            // machine_label
            // 
            this.machine_label.AutoSize = true;
            this.machine_label.BackColor = System.Drawing.Color.SkyBlue;
            this.machine_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.machine_label.Location = new System.Drawing.Point(840, 22);
            this.machine_label.Name = "machine_label";
            this.machine_label.Size = new System.Drawing.Size(63, 16);
            this.machine_label.TabIndex = 29;
            this.machine_label.Text = "Machine";
            // 
            // measure_mode_groupBox
            // 
            this.measure_mode_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.measure_mode_groupBox.Controls.Add(this.SDR_mode_radioButton);
            this.measure_mode_groupBox.Controls.Add(this.WB_mode_radioButton);
            this.measure_mode_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.measure_mode_groupBox.Location = new System.Drawing.Point(5, 620);
            this.measure_mode_groupBox.Name = "measure_mode_groupBox";
            this.measure_mode_groupBox.Size = new System.Drawing.Size(310, 60);
            this.measure_mode_groupBox.TabIndex = 30;
            this.measure_mode_groupBox.TabStop = false;
            this.measure_mode_groupBox.Text = "測定モード選択";
            // 
            // SDR_mode_radioButton
            // 
            this.SDR_mode_radioButton.AutoSize = true;
            this.SDR_mode_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SDR_mode_radioButton.Location = new System.Drawing.Point(80, 25);
            this.SDR_mode_radioButton.Name = "SDR_mode_radioButton";
            this.SDR_mode_radioButton.Size = new System.Drawing.Size(62, 23);
            this.SDR_mode_radioButton.TabIndex = 1;
            this.SDR_mode_radioButton.TabStop = true;
            this.SDR_mode_radioButton.Text = "SDR";
            this.SDR_mode_radioButton.UseVisualStyleBackColor = true;
            this.SDR_mode_radioButton.CheckedChanged += new System.EventHandler(this.mode_radioButton_CheckedChanged);
            // 
            // WB_mode_radioButton
            // 
            this.WB_mode_radioButton.AutoSize = true;
            this.WB_mode_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WB_mode_radioButton.Location = new System.Drawing.Point(10, 25);
            this.WB_mode_radioButton.Name = "WB_mode_radioButton";
            this.WB_mode_radioButton.Size = new System.Drawing.Size(53, 23);
            this.WB_mode_radioButton.TabIndex = 0;
            this.WB_mode_radioButton.TabStop = true;
            this.WB_mode_radioButton.Text = "WB";
            this.WB_mode_radioButton.UseVisualStyleBackColor = true;
            this.WB_mode_radioButton.CheckedChanged += new System.EventHandler(this.mode_radioButton_CheckedChanged);
            // 
            // standard_label
            // 
            this.standard_label.AutoSize = true;
            this.standard_label.BackColor = System.Drawing.Color.Yellow;
            this.standard_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.standard_label.Location = new System.Drawing.Point(325, 110);
            this.standard_label.Name = "standard_label";
            this.standard_label.Size = new System.Drawing.Size(120, 19);
            this.standard_label.TabIndex = 31;
            this.standard_label.Text = "standard_label";
            // 
            // measure_num_checkBox
            // 
            this.measure_num_checkBox.AutoSize = true;
            this.measure_num_checkBox.Checked = true;
            this.measure_num_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.measure_num_checkBox.Location = new System.Drawing.Point(905, 540);
            this.measure_num_checkBox.Name = "measure_num_checkBox";
            this.measure_num_checkBox.Size = new System.Drawing.Size(119, 16);
            this.measure_num_checkBox.TabIndex = 32;
            this.measure_num_checkBox.Text = "測定数をチェックする";
            this.measure_num_checkBox.UseVisualStyleBackColor = true;
            this.measure_num_checkBox.CheckedChanged += new System.EventHandler(this.measure_num_checkBox_CheckedChanged);
            // 
            // measure_num_check_label
            // 
            this.measure_num_check_label.AutoSize = true;
            this.measure_num_check_label.BackColor = System.Drawing.Color.Red;
            this.measure_num_check_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.measure_num_check_label.ForeColor = System.Drawing.Color.White;
            this.measure_num_check_label.Location = new System.Drawing.Point(100, 585);
            this.measure_num_check_label.Name = "measure_num_check_label";
            this.measure_num_check_label.Size = new System.Drawing.Size(201, 19);
            this.measure_num_check_label.TabIndex = 33;
            this.measure_num_check_label.Text = "まだ測定数が足りません。";
            this.measure_num_check_label.Visible = false;
            // 
            // num_ng_textBox
            // 
            this.num_ng_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.num_ng_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.num_ng_textBox.Location = new System.Drawing.Point(905, 560);
            this.num_ng_textBox.Multiline = true;
            this.num_ng_textBox.Name = "num_ng_textBox";
            this.num_ng_textBox.ReadOnly = true;
            this.num_ng_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.num_ng_textBox.Size = new System.Drawing.Size(270, 117);
            this.num_ng_textBox.TabIndex = 35;
            this.num_ng_textBox.WordWrap = false;
            // 
            // MeasureSerial_label
            // 
            this.MeasureSerial_label.AutoSize = true;
            this.MeasureSerial_label.Location = new System.Drawing.Point(735, 50);
            this.MeasureSerial_label.Name = "MeasureSerial_label";
            this.MeasureSerial_label.Size = new System.Drawing.Size(92, 12);
            this.MeasureSerial_label.TabIndex = 36;
            this.MeasureSerial_label.Text = "メジャリング測定器";
            // 
            // item_cnt_textBox
            // 
            this.item_cnt_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.item_cnt_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.item_cnt_textBox.Location = new System.Drawing.Point(905, 165);
            this.item_cnt_textBox.Multiline = true;
            this.item_cnt_textBox.Name = "item_cnt_textBox";
            this.item_cnt_textBox.ReadOnly = true;
            this.item_cnt_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.item_cnt_textBox.Size = new System.Drawing.Size(270, 350);
            this.item_cnt_textBox.TabIndex = 37;
            this.item_cnt_textBox.WordWrap = false;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1184, 691);
            this.Controls.Add(this.item_cnt_textBox);
            this.Controls.Add(this.MeasureSerial_label);
            this.Controls.Add(this.num_ng_textBox);
            this.Controls.Add(this.measure_num_check_label);
            this.Controls.Add(this.measure_num_checkBox);
            this.Controls.Add(this.standard_label);
            this.Controls.Add(this.measure_mode_groupBox);
            this.Controls.Add(this.machine_label);
            this.Controls.Add(this.machine_input_textBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.ok_ng_label);
            this.Controls.Add(this.check_standard_label);
            this.Controls.Add(this.lotInfo_clear_button);
            this.Controls.Add(this.SDR_measure_panel);
            this.Controls.Add(this.save_button);
            this.Controls.Add(this.allclear_button);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.delete_row_button);
            this.Controls.Add(this.operator_label);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.lotQR_label);
            this.Controls.Add(this.operator_input_textBox);
            this.Controls.Add(this.lotno_label);
            this.Controls.Add(this.product_label);
            this.Controls.Add(this.ReadQR_textBox);
            this.Controls.Add(this.SaveFolder_label);
            this.Controls.Add(this.SelectSaveFolder_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.WB_measure_panel);
            this.Controls.Add(this.FormClose_button);
            this.Controls.Add(this.connect_label);
            this.Controls.Add(this.connect_button);
            this.Controls.Add(this.comport_comboBox);
            this.Cursor = System.Windows.Forms.Cursors.Default;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "MainForm";
            this.Text = "M-Com(C#) version=1.6.0  2021/10/26";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Click += new System.EventHandler(this.MainForm_Click);
            this.WB_item_groupBox.ResumeLayout(false);
            this.WB_place_groupBox.ResumeLayout(false);
            this.WB_place_groupBox.PerformLayout();
            this.WB_measure_panel.ResumeLayout(false);
            this.WB_chip_groupBox.ResumeLayout(false);
            this.WB_chip_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.SDR_measure_panel.ResumeLayout(false);
            this.SDR_place_groupBox.ResumeLayout(false);
            this.SDR_place_groupBox.PerformLayout();
            this.SDR_item_groupBox.ResumeLayout(false);
            this.measure_mode_groupBox.ResumeLayout(false);
            this.measure_mode_groupBox.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.IO.Ports.SerialPort serialPort1;
        private System.Windows.Forms.ComboBox comport_comboBox;
        private System.Windows.Forms.Button connect_button;
        private System.Windows.Forms.Label connect_label;
        private System.Windows.Forms.Button FormClose_button;
        private System.Windows.Forms.GroupBox WB_item_groupBox;
        private System.Windows.Forms.Button ball_Y_button;
        private System.Windows.Forms.Button ball_X_button;
        private System.Windows.Forms.Button reverse_Y_button;
        private System.Windows.Forms.Button loop_Z_button;
        private System.Windows.Forms.Button ball_Z_button;
        private System.Windows.Forms.GroupBox WB_place_groupBox;
        private System.Windows.Forms.RadioButton zd_radioButton;
        private System.Windows.Forms.RadioButton lead_radioButton;
        private System.Windows.Forms.RadioButton bump_radioButton;
        private System.Windows.Forms.RadioButton cathode_radioButton;
        private System.Windows.Forms.RadioButton anode_radioButton;
        private System.Windows.Forms.Panel WB_measure_panel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button save_button;
        private System.Windows.Forms.RadioButton zdbump_radioButton;
        private System.Windows.Forms.Button SelectSaveFolder_button;
        private System.Windows.Forms.FolderBrowserDialog SelectSaveFolder_fBD;
        private System.Windows.Forms.Label SaveFolder_label;
        private System.Windows.Forms.TextBox ReadQR_textBox;
        private System.Windows.Forms.Label product_label;
        private System.Windows.Forms.Label lotno_label;
        private System.Windows.Forms.TextBox operator_input_textBox;
        private System.Windows.Forms.Label lotQR_label;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label operator_label;
        private System.Windows.Forms.Button delete_row_button;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button allclear_button;
        private System.Windows.Forms.Panel SDR_measure_panel;
        private System.Windows.Forms.GroupBox SDR_place_groupBox;
        private System.Windows.Forms.RadioButton minus_radioButton;
        private System.Windows.Forms.RadioButton plus_radioButton;
        private System.Windows.Forms.GroupBox SDR_item_groupBox;
        private System.Windows.Forms.Button height_Z_button;
        private System.Windows.Forms.Button width_Y_button;
        private System.Windows.Forms.Button outer_Y_button;
        private System.Windows.Forms.Button clearance_Y_button;
        private System.Windows.Forms.Button inner_Y_button;
        private System.Windows.Forms.Button lotInfo_clear_button;
        private System.Windows.Forms.Label check_standard_label;
        private System.Windows.Forms.Label ok_ng_label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox machine_input_textBox;
        private System.Windows.Forms.Label machine_label;
        private System.Windows.Forms.DataGridViewTextBoxColumn place;
        private System.Windows.Forms.DataGridViewTextBoxColumn item;
        private System.Windows.Forms.DataGridViewTextBoxColumn data;
        private System.Windows.Forms.DataGridViewTextBoxColumn measure_datetime;
        private System.Windows.Forms.DataGridViewTextBoxColumn judge;
        private System.Windows.Forms.GroupBox measure_mode_groupBox;
        private System.Windows.Forms.RadioButton SDR_mode_radioButton;
        private System.Windows.Forms.RadioButton WB_mode_radioButton;
        private System.Windows.Forms.GroupBox WB_chip_groupBox;
        private System.Windows.Forms.RadioButton blue_chip_radioButton;
        private System.Windows.Forms.RadioButton green_chip_radioButton;
        private System.Windows.Forms.RadioButton red_chip_radioButton;
        private System.Windows.Forms.Label standard_label;
        private System.Windows.Forms.CheckBox measure_num_checkBox;
        private System.Windows.Forms.Label measure_num_check_label;
        private System.Windows.Forms.TextBox num_ng_textBox;
        private System.Windows.Forms.Label MeasureSerial_label;
        private System.Windows.Forms.TextBox item_cnt_textBox;
        private System.Windows.Forms.Button clearance_Z_button;
    }
}

