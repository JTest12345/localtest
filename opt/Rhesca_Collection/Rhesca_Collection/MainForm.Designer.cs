namespace Rhesca_Collection {
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea1 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend1 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.serialPort1 = new System.IO.Ports.SerialPort(this.components);
            this.comport_comboBox = new System.Windows.Forms.ComboBox();
            this.connect_button = new System.Windows.Forms.Button();
            this.connect_label = new System.Windows.Forms.Label();
            this.FormClose_button = new System.Windows.Forms.Button();
            this.WB_item_groupBox = new System.Windows.Forms.GroupBox();
            this.PC_radioButton = new System.Windows.Forms.RadioButton();
            this.BS_radioButton = new System.Windows.Forms.RadioButton();
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
            this.mode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.measure_datetime = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.judge = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allclear_button = new System.Windows.Forms.Button();
            this.lotInfo_clear_button = new System.Windows.Forms.Button();
            this.check_standard_label = new System.Windows.Forms.Label();
            this.ok_ng_label = new System.Windows.Forms.Label();
            this.debug_label = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.machine_label = new System.Windows.Forms.Label();
            this.machine_input_textBox = new System.Windows.Forms.TextBox();
            this.measure_num_checkBox = new System.Windows.Forms.CheckBox();
            this.measure_num_check_label = new System.Windows.Forms.Label();
            this.standard_label = new System.Windows.Forms.Label();
            this.item_cnt_textBox = new System.Windows.Forms.TextBox();
            this.num_ng_textBox = new System.Windows.Forms.TextBox();
            this.MeasureSerial_label = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.cpk_value_label = new System.Windows.Forms.Label();
            this.cpk_judge_label = new System.Windows.Forms.Label();
            this.cpk_measurename_label = new System.Windows.Forms.Label();
            this.Chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.WB_item_groupBox.SuspendLayout();
            this.WB_place_groupBox.SuspendLayout();
            this.WB_measure_panel.SuspendLayout();
            this.WB_chip_groupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.Chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // serialPort1
            // 
            this.serialPort1.Handshake = System.IO.Ports.Handshake.RequestToSend;
            this.serialPort1.StopBits = System.IO.Ports.StopBits.Two;
            this.serialPort1.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.serialPort1_DataReceived);
            // 
            // comport_comboBox
            // 
            this.comport_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comport_comboBox.FormattingEnabled = true;
            this.comport_comboBox.Location = new System.Drawing.Point(8, 46);
            this.comport_comboBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.comport_comboBox.Name = "comport_comboBox";
            this.comport_comboBox.Size = new System.Drawing.Size(199, 26);
            this.comport_comboBox.TabIndex = 0;
            this.comport_comboBox.SelectedIndexChanged += new System.EventHandler(this.comport_comboBox_SelectedIndexChanged);
            // 
            // connect_button
            // 
            this.connect_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.connect_button.Location = new System.Drawing.Point(217, 45);
            this.connect_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.connect_button.Name = "connect_button";
            this.connect_button.Size = new System.Drawing.Size(117, 75);
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
            this.connect_label.Location = new System.Drawing.Point(342, 46);
            this.connect_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.connect_label.Name = "connect_label";
            this.connect_label.Size = new System.Drawing.Size(165, 74);
            this.connect_label.TabIndex = 2;
            this.connect_label.Text = "切断中";
            this.connect_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FormClose_button
            // 
            this.FormClose_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.FormClose_button.Location = new System.Drawing.Point(1525, 18);
            this.FormClose_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.FormClose_button.Name = "FormClose_button";
            this.FormClose_button.Size = new System.Drawing.Size(100, 74);
            this.FormClose_button.TabIndex = 3;
            this.FormClose_button.Text = "ソフト\r\n終了";
            this.FormClose_button.UseVisualStyleBackColor = true;
            this.FormClose_button.Click += new System.EventHandler(this.FormClose_button_Click);
            // 
            // WB_item_groupBox
            // 
            this.WB_item_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.WB_item_groupBox.Controls.Add(this.PC_radioButton);
            this.WB_item_groupBox.Controls.Add(this.BS_radioButton);
            this.WB_item_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WB_item_groupBox.Location = new System.Drawing.Point(267, 93);
            this.WB_item_groupBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_item_groupBox.Name = "WB_item_groupBox";
            this.WB_item_groupBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_item_groupBox.Size = new System.Drawing.Size(233, 315);
            this.WB_item_groupBox.TabIndex = 5;
            this.WB_item_groupBox.TabStop = false;
            this.WB_item_groupBox.Text = "測定項目";
            // 
            // PC_radioButton
            // 
            this.PC_radioButton.AutoSize = true;
            this.PC_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.PC_radioButton.Location = new System.Drawing.Point(8, 90);
            this.PC_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.PC_radioButton.Name = "PC_radioButton";
            this.PC_radioButton.Size = new System.Drawing.Size(75, 33);
            this.PC_radioButton.TabIndex = 2;
            this.PC_radioButton.TabStop = true;
            this.PC_radioButton.Text = "PC";
            this.PC_radioButton.UseVisualStyleBackColor = true;
            this.PC_radioButton.CheckedChanged += new System.EventHandler(this.measure_item_radioButton_CheckedChanged);
            // 
            // BS_radioButton
            // 
            this.BS_radioButton.AutoSize = true;
            this.BS_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.BS_radioButton.Location = new System.Drawing.Point(8, 45);
            this.BS_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.BS_radioButton.Name = "BS_radioButton";
            this.BS_radioButton.Size = new System.Drawing.Size(73, 33);
            this.BS_radioButton.TabIndex = 1;
            this.BS_radioButton.TabStop = true;
            this.BS_radioButton.Text = "BS";
            this.BS_radioButton.UseVisualStyleBackColor = true;
            this.BS_radioButton.CheckedChanged += new System.EventHandler(this.measure_item_radioButton_CheckedChanged);
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
            this.WB_place_groupBox.Location = new System.Drawing.Point(8, 93);
            this.WB_place_groupBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_place_groupBox.Name = "WB_place_groupBox";
            this.WB_place_groupBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_place_groupBox.Size = new System.Drawing.Size(250, 315);
            this.WB_place_groupBox.TabIndex = 6;
            this.WB_place_groupBox.TabStop = false;
            this.WB_place_groupBox.Text = "測定箇所";
            // 
            // zdbump_radioButton
            // 
            this.zdbump_radioButton.AutoSize = true;
            this.zdbump_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.zdbump_radioButton.Location = new System.Drawing.Point(8, 270);
            this.zdbump_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.zdbump_radioButton.Name = "zdbump_radioButton";
            this.zdbump_radioButton.Size = new System.Drawing.Size(215, 33);
            this.zdbump_radioButton.TabIndex = 5;
            this.zdbump_radioButton.TabStop = true;
            this.zdbump_radioButton.Tag = "NotRGB";
            this.zdbump_radioButton.Text = "ZDBump (Z-Z)";
            this.zdbump_radioButton.UseVisualStyleBackColor = true;
            this.zdbump_radioButton.CheckedChanged += new System.EventHandler(this.measure_place_radioButton_CheckedChanged);
            // 
            // zd_radioButton
            // 
            this.zd_radioButton.AutoSize = true;
            this.zd_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.zd_radioButton.Location = new System.Drawing.Point(8, 225);
            this.zd_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.zd_radioButton.Name = "zd_radioButton";
            this.zd_radioButton.Size = new System.Drawing.Size(73, 33);
            this.zd_radioButton.TabIndex = 4;
            this.zd_radioButton.TabStop = true;
            this.zd_radioButton.Tag = "NotRGB";
            this.zd_radioButton.Text = "ZD";
            this.zd_radioButton.UseVisualStyleBackColor = true;
            this.zd_radioButton.CheckedChanged += new System.EventHandler(this.measure_place_radioButton_CheckedChanged);
            // 
            // lead_radioButton
            // 
            this.lead_radioButton.AutoSize = true;
            this.lead_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lead_radioButton.Location = new System.Drawing.Point(8, 180);
            this.lead_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lead_radioButton.Name = "lead_radioButton";
            this.lead_radioButton.Size = new System.Drawing.Size(180, 33);
            this.lead_radioButton.TabIndex = 3;
            this.lead_radioButton.TabStop = true;
            this.lead_radioButton.Tag = "NotRGB";
            this.lead_radioButton.Text = "Lead-Bump";
            this.lead_radioButton.UseVisualStyleBackColor = true;
            this.lead_radioButton.CheckedChanged += new System.EventHandler(this.measure_place_radioButton_CheckedChanged);
            // 
            // bump_radioButton
            // 
            this.bump_radioButton.AutoSize = true;
            this.bump_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.bump_radioButton.Location = new System.Drawing.Point(8, 135);
            this.bump_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.bump_radioButton.Name = "bump_radioButton";
            this.bump_radioButton.Size = new System.Drawing.Size(184, 33);
            this.bump_radioButton.TabIndex = 2;
            this.bump_radioButton.TabStop = true;
            this.bump_radioButton.Text = "Bump (P-P)";
            this.bump_radioButton.UseVisualStyleBackColor = true;
            this.bump_radioButton.CheckedChanged += new System.EventHandler(this.measure_place_radioButton_CheckedChanged);
            // 
            // cathode_radioButton
            // 
            this.cathode_radioButton.AutoSize = true;
            this.cathode_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cathode_radioButton.Location = new System.Drawing.Point(8, 90);
            this.cathode_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.cathode_radioButton.Name = "cathode_radioButton";
            this.cathode_radioButton.Size = new System.Drawing.Size(140, 33);
            this.cathode_radioButton.TabIndex = 1;
            this.cathode_radioButton.TabStop = true;
            this.cathode_radioButton.Text = "Cathode";
            this.cathode_radioButton.UseVisualStyleBackColor = true;
            this.cathode_radioButton.CheckedChanged += new System.EventHandler(this.measure_place_radioButton_CheckedChanged);
            // 
            // anode_radioButton
            // 
            this.anode_radioButton.AutoSize = true;
            this.anode_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.anode_radioButton.Location = new System.Drawing.Point(8, 45);
            this.anode_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.anode_radioButton.Name = "anode_radioButton";
            this.anode_radioButton.Size = new System.Drawing.Size(115, 33);
            this.anode_radioButton.TabIndex = 0;
            this.anode_radioButton.TabStop = true;
            this.anode_radioButton.Text = "Anode";
            this.anode_radioButton.UseVisualStyleBackColor = true;
            this.anode_radioButton.CheckedChanged += new System.EventHandler(this.measure_place_radioButton_CheckedChanged);
            // 
            // WB_measure_panel
            // 
            this.WB_measure_panel.Controls.Add(this.WB_chip_groupBox);
            this.WB_measure_panel.Controls.Add(this.WB_place_groupBox);
            this.WB_measure_panel.Controls.Add(this.WB_item_groupBox);
            this.WB_measure_panel.Enabled = false;
            this.WB_measure_panel.Location = new System.Drawing.Point(8, 248);
            this.WB_measure_panel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_measure_panel.Name = "WB_measure_panel";
            this.WB_measure_panel.Size = new System.Drawing.Size(517, 412);
            this.WB_measure_panel.TabIndex = 7;
            // 
            // WB_chip_groupBox
            // 
            this.WB_chip_groupBox.BackColor = System.Drawing.SystemColors.Control;
            this.WB_chip_groupBox.Controls.Add(this.green_chip_radioButton);
            this.WB_chip_groupBox.Controls.Add(this.red_chip_radioButton);
            this.WB_chip_groupBox.Controls.Add(this.blue_chip_radioButton);
            this.WB_chip_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.WB_chip_groupBox.Location = new System.Drawing.Point(8, 8);
            this.WB_chip_groupBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_chip_groupBox.Name = "WB_chip_groupBox";
            this.WB_chip_groupBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.WB_chip_groupBox.Size = new System.Drawing.Size(500, 82);
            this.WB_chip_groupBox.TabIndex = 36;
            this.WB_chip_groupBox.TabStop = false;
            this.WB_chip_groupBox.Text = "測定素子";
            // 
            // green_chip_radioButton
            // 
            this.green_chip_radioButton.AutoSize = true;
            this.green_chip_radioButton.BackColor = System.Drawing.Color.Green;
            this.green_chip_radioButton.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.green_chip_radioButton.ForeColor = System.Drawing.Color.Black;
            this.green_chip_radioButton.Location = new System.Drawing.Point(308, 38);
            this.green_chip_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.green_chip_radioButton.Name = "green_chip_radioButton";
            this.green_chip_radioButton.Size = new System.Drawing.Size(113, 33);
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
            this.red_chip_radioButton.Location = new System.Drawing.Point(158, 38);
            this.red_chip_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.red_chip_radioButton.Name = "red_chip_radioButton";
            this.red_chip_radioButton.Size = new System.Drawing.Size(85, 33);
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
            this.blue_chip_radioButton.Location = new System.Drawing.Point(8, 38);
            this.blue_chip_radioButton.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.blue_chip_radioButton.Name = "blue_chip_radioButton";
            this.blue_chip_radioButton.Size = new System.Drawing.Size(92, 33);
            this.blue_chip_radioButton.TabIndex = 1;
            this.blue_chip_radioButton.TabStop = true;
            this.blue_chip_radioButton.Text = "Blue";
            this.blue_chip_radioButton.UseVisualStyleBackColor = false;
            this.blue_chip_radioButton.CheckedChanged += new System.EventHandler(this.chip_radioButton_CheckedChanged);
            // 
            // save_button
            // 
            this.save_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.save_button.Location = new System.Drawing.Point(8, 668);
            this.save_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.save_button.Name = "save_button";
            this.save_button.Size = new System.Drawing.Size(133, 75);
            this.save_button.TabIndex = 10;
            this.save_button.Text = "保存";
            this.save_button.UseVisualStyleBackColor = true;
            this.save_button.Click += new System.EventHandler(this.save_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(290, 23);
            this.label1.TabIndex = 8;
            this.label1.Text = "COMポートを選択してください。";
            // 
            // SelectSaveFolder_button
            // 
            this.SelectSaveFolder_button.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.SelectSaveFolder_button.Location = new System.Drawing.Point(8, 158);
            this.SelectSaveFolder_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.SelectSaveFolder_button.Name = "SelectSaveFolder_button";
            this.SelectSaveFolder_button.Size = new System.Drawing.Size(167, 75);
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
            this.SaveFolder_label.Location = new System.Drawing.Point(8, 130);
            this.SaveFolder_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.SaveFolder_label.Name = "SaveFolder_label";
            this.SaveFolder_label.Size = new System.Drawing.Size(52, 18);
            this.SaveFolder_label.TabIndex = 11;
            this.SaveFolder_label.Text = "label2";
            // 
            // ReadQR_textBox
            // 
            this.ReadQR_textBox.BackColor = System.Drawing.SystemColors.Window;
            this.ReadQR_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ReadQR_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.ReadQR_textBox.Location = new System.Drawing.Point(535, 27);
            this.ReadQR_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.ReadQR_textBox.Name = "ReadQR_textBox";
            this.ReadQR_textBox.Size = new System.Drawing.Size(164, 31);
            this.ReadQR_textBox.TabIndex = 12;
            this.ReadQR_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.ReadQR_textBox_KeyPress);
            // 
            // product_label
            // 
            this.product_label.AutoSize = true;
            this.product_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.product_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.product_label.Location = new System.Drawing.Point(535, 68);
            this.product_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.product_label.Name = "product_label";
            this.product_label.Size = new System.Drawing.Size(89, 24);
            this.product_label.TabIndex = 13;
            this.product_label.Text = "Product";
            // 
            // lotno_label
            // 
            this.lotno_label.AutoSize = true;
            this.lotno_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.lotno_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.lotno_label.Location = new System.Drawing.Point(537, 98);
            this.lotno_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lotno_label.Name = "lotno_label";
            this.lotno_label.Size = new System.Drawing.Size(77, 24);
            this.lotno_label.TabIndex = 14;
            this.lotno_label.Text = "Lot No";
            // 
            // operator_input_textBox
            // 
            this.operator_input_textBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.operator_input_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.operator_input_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.operator_input_textBox.Location = new System.Drawing.Point(900, 27);
            this.operator_input_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.operator_input_textBox.Name = "operator_input_textBox";
            this.operator_input_textBox.Size = new System.Drawing.Size(164, 31);
            this.operator_input_textBox.TabIndex = 16;
            this.operator_input_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.operator_input_textBox_KeyPress);
            // 
            // lotQR_label
            // 
            this.lotQR_label.AutoSize = true;
            this.lotQR_label.Location = new System.Drawing.Point(533, 4);
            this.lotQR_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.lotQR_label.Name = "lotQR_label";
            this.lotQR_label.Size = new System.Drawing.Size(162, 18);
            this.lotQR_label.TabIndex = 17;
            this.lotQR_label.Text = "ロット管理表QRコード";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(900, 4);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(102, 18);
            this.label3.TabIndex = 18;
            this.label3.Text = "測定者コード";
            // 
            // operator_label
            // 
            this.operator_label.AutoSize = true;
            this.operator_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.operator_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.operator_label.Location = new System.Drawing.Point(1075, 33);
            this.operator_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.operator_label.Name = "operator_label";
            this.operator_label.Size = new System.Drawing.Size(98, 24);
            this.operator_label.TabIndex = 19;
            this.operator_label.Text = "Operator";
            // 
            // delete_row_button
            // 
            this.delete_row_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.delete_row_button.Location = new System.Drawing.Point(1532, 158);
            this.delete_row_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.delete_row_button.Name = "delete_row_button";
            this.delete_row_button.Size = new System.Drawing.Size(133, 75);
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
            this.mode,
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
            this.dataGridView1.Location = new System.Drawing.Point(531, 202);
            this.dataGridView1.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
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
            this.dataGridView1.Size = new System.Drawing.Size(991, 507);
            this.dataGridView1.TabIndex = 21;
            // 
            // place
            // 
            this.place.HeaderText = "測定箇所";
            this.place.MinimumWidth = 8;
            this.place.Name = "place";
            this.place.ReadOnly = true;
            this.place.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.place.Width = 112;
            // 
            // item
            // 
            this.item.HeaderText = "測定項目";
            this.item.MinimumWidth = 8;
            this.item.Name = "item";
            this.item.ReadOnly = true;
            this.item.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.item.Width = 112;
            // 
            // data
            // 
            this.data.HeaderText = "測定値[gf]";
            this.data.MinimumWidth = 8;
            this.data.Name = "data";
            this.data.ReadOnly = true;
            this.data.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.data.Width = 122;
            // 
            // mode
            // 
            this.mode.HeaderText = "破壊モード";
            this.mode.MinimumWidth = 8;
            this.mode.Name = "mode";
            this.mode.ReadOnly = true;
            this.mode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.mode.Width = 119;
            // 
            // measure_datetime
            // 
            this.measure_datetime.HeaderText = "測定日時";
            this.measure_datetime.MinimumWidth = 8;
            this.measure_datetime.Name = "measure_datetime";
            this.measure_datetime.ReadOnly = true;
            this.measure_datetime.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.measure_datetime.Width = 112;
            // 
            // judge
            // 
            this.judge.HeaderText = "判定";
            this.judge.MinimumWidth = 8;
            this.judge.Name = "judge";
            this.judge.ReadOnly = true;
            this.judge.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.judge.Width = 64;
            // 
            // allclear_button
            // 
            this.allclear_button.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.allclear_button.Location = new System.Drawing.Point(1733, 158);
            this.allclear_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.allclear_button.Name = "allclear_button";
            this.allclear_button.Size = new System.Drawing.Size(133, 75);
            this.allclear_button.TabIndex = 22;
            this.allclear_button.Text = "全消去";
            this.allclear_button.UseVisualStyleBackColor = true;
            this.allclear_button.Click += new System.EventHandler(this.allclear_button_Click);
            // 
            // lotInfo_clear_button
            // 
            this.lotInfo_clear_button.AutoSize = true;
            this.lotInfo_clear_button.Location = new System.Drawing.Point(708, 26);
            this.lotInfo_clear_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.lotInfo_clear_button.Name = "lotInfo_clear_button";
            this.lotInfo_clear_button.Size = new System.Drawing.Size(145, 39);
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
            this.check_standard_label.Location = new System.Drawing.Point(183, 172);
            this.check_standard_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.check_standard_label.Name = "check_standard_label";
            this.check_standard_label.Size = new System.Drawing.Size(314, 60);
            this.check_standard_label.TabIndex = 25;
            this.check_standard_label.Text = "この機種には\r\n下限値マスタがありません。\r\n規格値以上かしっかり確認して下さい。";
            // 
            // ok_ng_label
            // 
            this.ok_ng_label.AutoSize = true;
            this.ok_ng_label.BackColor = System.Drawing.Color.Red;
            this.ok_ng_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ok_ng_label.ForeColor = System.Drawing.Color.White;
            this.ok_ng_label.Location = new System.Drawing.Point(167, 675);
            this.ok_ng_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.ok_ng_label.Name = "ok_ng_label";
            this.ok_ng_label.Size = new System.Drawing.Size(203, 29);
            this.ok_ng_label.TabIndex = 26;
            this.ok_ng_label.Text = "NGがありました。";
            this.ok_ng_label.Visible = false;
            // 
            // debug_label
            // 
            this.debug_label.AutoSize = true;
            this.debug_label.Location = new System.Drawing.Point(8, 750);
            this.debug_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.debug_label.Name = "debug_label";
            this.debug_label.Size = new System.Drawing.Size(183, 18);
            this.debug_label.TabIndex = 27;
            this.debug_label.Text = "受信メッセージ(debug用)\r\n";
            this.debug_label.Visible = false;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(1233, 4);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(65, 18);
            this.label4.TabIndex = 28;
            this.label4.Text = "設備No";
            // 
            // machine_label
            // 
            this.machine_label.AutoSize = true;
            this.machine_label.BackColor = System.Drawing.Color.SkyBlue;
            this.machine_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.machine_label.Location = new System.Drawing.Point(1400, 33);
            this.machine_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.machine_label.Name = "machine_label";
            this.machine_label.Size = new System.Drawing.Size(92, 24);
            this.machine_label.TabIndex = 30;
            this.machine_label.Text = "Machine";
            // 
            // machine_input_textBox
            // 
            this.machine_input_textBox.BackColor = System.Drawing.Color.SkyBlue;
            this.machine_input_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.machine_input_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.machine_input_textBox.Location = new System.Drawing.Point(1225, 27);
            this.machine_input_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.machine_input_textBox.Name = "machine_input_textBox";
            this.machine_input_textBox.Size = new System.Drawing.Size(164, 31);
            this.machine_input_textBox.TabIndex = 29;
            this.machine_input_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.machine_input_textBox_KeyPress);
            // 
            // measure_num_checkBox
            // 
            this.measure_num_checkBox.AutoSize = true;
            this.measure_num_checkBox.Checked = true;
            this.measure_num_checkBox.CheckState = System.Windows.Forms.CheckState.Checked;
            this.measure_num_checkBox.Location = new System.Drawing.Point(1525, 810);
            this.measure_num_checkBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.measure_num_checkBox.Name = "measure_num_checkBox";
            this.measure_num_checkBox.Size = new System.Drawing.Size(164, 22);
            this.measure_num_checkBox.TabIndex = 31;
            this.measure_num_checkBox.Text = "測定数チェックする";
            this.measure_num_checkBox.UseVisualStyleBackColor = true;
            this.measure_num_checkBox.CheckedChanged += new System.EventHandler(this.measure_num_checkBox_CheckedChanged);
            // 
            // measure_num_check_label
            // 
            this.measure_num_check_label.AutoSize = true;
            this.measure_num_check_label.BackColor = System.Drawing.Color.Red;
            this.measure_num_check_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.measure_num_check_label.ForeColor = System.Drawing.Color.White;
            this.measure_num_check_label.Location = new System.Drawing.Point(167, 712);
            this.measure_num_check_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.measure_num_check_label.Name = "measure_num_check_label";
            this.measure_num_check_label.Size = new System.Drawing.Size(306, 29);
            this.measure_num_check_label.TabIndex = 32;
            this.measure_num_check_label.Text = "まだ測定数が足りません。";
            this.measure_num_check_label.Visible = false;
            // 
            // standard_label
            // 
            this.standard_label.AutoSize = true;
            this.standard_label.BackColor = System.Drawing.Color.Yellow;
            this.standard_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.standard_label.Location = new System.Drawing.Point(542, 165);
            this.standard_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.standard_label.Name = "standard_label";
            this.standard_label.Size = new System.Drawing.Size(181, 29);
            this.standard_label.TabIndex = 34;
            this.standard_label.Text = "standard_label";
            // 
            // item_cnt_textBox
            // 
            this.item_cnt_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.item_cnt_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.item_cnt_textBox.Location = new System.Drawing.Point(1525, 248);
            this.item_cnt_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.item_cnt_textBox.Multiline = true;
            this.item_cnt_textBox.Name = "item_cnt_textBox";
            this.item_cnt_textBox.ReadOnly = true;
            this.item_cnt_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.item_cnt_textBox.Size = new System.Drawing.Size(447, 523);
            this.item_cnt_textBox.TabIndex = 38;
            this.item_cnt_textBox.WordWrap = false;
            // 
            // num_ng_textBox
            // 
            this.num_ng_textBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.num_ng_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.num_ng_textBox.Location = new System.Drawing.Point(1525, 840);
            this.num_ng_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.num_ng_textBox.Multiline = true;
            this.num_ng_textBox.Name = "num_ng_textBox";
            this.num_ng_textBox.ReadOnly = true;
            this.num_ng_textBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.num_ng_textBox.Size = new System.Drawing.Size(447, 174);
            this.num_ng_textBox.TabIndex = 39;
            this.num_ng_textBox.WordWrap = false;
            // 
            // MeasureSerial_label
            // 
            this.MeasureSerial_label.AutoSize = true;
            this.MeasureSerial_label.Location = new System.Drawing.Point(1225, 75);
            this.MeasureSerial_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.MeasureSerial_label.Name = "MeasureSerial_label";
            this.MeasureSerial_label.Size = new System.Drawing.Size(62, 18);
            this.MeasureSerial_label.TabIndex = 40;
            this.MeasureSerial_label.Text = "測定器";
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(161, 750);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(204, 34);
            this.label2.TabIndex = 41;
            this.label2.Text = "管理値判定";
            // 
            // cpk_value_label
            // 
            this.cpk_value_label.Font = new System.Drawing.Font("MS UI Gothic", 28F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cpk_value_label.Location = new System.Drawing.Point(162, 827);
            this.cpk_value_label.Name = "cpk_value_label";
            this.cpk_value_label.Size = new System.Drawing.Size(244, 60);
            this.cpk_value_label.TabIndex = 42;
            this.cpk_value_label.Text = "-";
            // 
            // cpk_judge_label
            // 
            this.cpk_judge_label.Font = new System.Drawing.Font("MS UI Gothic", 36F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cpk_judge_label.Location = new System.Drawing.Point(154, 887);
            this.cpk_judge_label.Name = "cpk_judge_label";
            this.cpk_judge_label.Size = new System.Drawing.Size(204, 79);
            this.cpk_judge_label.TabIndex = 43;
            this.cpk_judge_label.Text = "判定";
            // 
            // cpk_measurename_label
            // 
            this.cpk_measurename_label.Font = new System.Drawing.Font("MS UI Gothic", 14F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.cpk_measurename_label.Location = new System.Drawing.Point(161, 784);
            this.cpk_measurename_label.Name = "cpk_measurename_label";
            this.cpk_measurename_label.Size = new System.Drawing.Size(325, 34);
            this.cpk_measurename_label.TabIndex = 44;
            this.cpk_measurename_label.Text = "測定項目";
            // 
            // Chart1
            // 
            this.Chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            chartArea1.Name = "ChartArea1";
            this.Chart1.ChartAreas.Add(chartArea1);
            legend1.Name = "Legend1";
            this.Chart1.Legends.Add(legend1);
            this.Chart1.Location = new System.Drawing.Point(531, 727);
            this.Chart1.Name = "Chart1";
            this.Chart1.Size = new System.Drawing.Size(986, 306);
            this.Chart1.TabIndex = 45;
            this.Chart1.Text = "chart1";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.SystemColors.Control;
            this.ClientSize = new System.Drawing.Size(1924, 1036);
            this.Controls.Add(this.Chart1);
            this.Controls.Add(this.cpk_measurename_label);
            this.Controls.Add(this.cpk_judge_label);
            this.Controls.Add(this.cpk_value_label);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.MeasureSerial_label);
            this.Controls.Add(this.num_ng_textBox);
            this.Controls.Add(this.item_cnt_textBox);
            this.Controls.Add(this.standard_label);
            this.Controls.Add(this.measure_num_check_label);
            this.Controls.Add(this.measure_num_checkBox);
            this.Controls.Add(this.machine_label);
            this.Controls.Add(this.machine_input_textBox);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.debug_label);
            this.Controls.Add(this.ok_ng_label);
            this.Controls.Add(this.check_standard_label);
            this.Controls.Add(this.lotInfo_clear_button);
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
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "MainForm";
            this.Text = "Rhesca_Collection(C#) version=1.2.0  2023/03/23";
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.Click += new System.EventHandler(this.MainForm_Click);
            this.WB_item_groupBox.ResumeLayout(false);
            this.WB_item_groupBox.PerformLayout();
            this.WB_place_groupBox.ResumeLayout(false);
            this.WB_place_groupBox.PerformLayout();
            this.WB_measure_panel.ResumeLayout(false);
            this.WB_chip_groupBox.ResumeLayout(false);
            this.WB_chip_groupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.Chart1)).EndInit();
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
        private System.Windows.Forms.Button lotInfo_clear_button;
        private System.Windows.Forms.Label check_standard_label;
        private System.Windows.Forms.Label ok_ng_label;
        private System.Windows.Forms.RadioButton PC_radioButton;
        private System.Windows.Forms.RadioButton BS_radioButton;
        private System.Windows.Forms.Label debug_label;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label machine_label;
        private System.Windows.Forms.TextBox machine_input_textBox;
        private System.Windows.Forms.DataGridViewTextBoxColumn place;
        private System.Windows.Forms.DataGridViewTextBoxColumn item;
        private System.Windows.Forms.DataGridViewTextBoxColumn data;
        private System.Windows.Forms.DataGridViewTextBoxColumn mode;
        private System.Windows.Forms.DataGridViewTextBoxColumn measure_datetime;
        private System.Windows.Forms.DataGridViewTextBoxColumn judge;
        private System.Windows.Forms.CheckBox measure_num_checkBox;
        private System.Windows.Forms.Label measure_num_check_label;
        private System.Windows.Forms.Label standard_label;
        private System.Windows.Forms.GroupBox WB_chip_groupBox;
        private System.Windows.Forms.RadioButton green_chip_radioButton;
        private System.Windows.Forms.RadioButton red_chip_radioButton;
        private System.Windows.Forms.RadioButton blue_chip_radioButton;
        private System.Windows.Forms.TextBox item_cnt_textBox;
        private System.Windows.Forms.TextBox num_ng_textBox;
        private System.Windows.Forms.Label MeasureSerial_label;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label cpk_value_label;
        private System.Windows.Forms.Label cpk_judge_label;
        private System.Windows.Forms.Label cpk_measurename_label;
        private System.Windows.Forms.DataVisualization.Charting.Chart Chart1;
    }
}

