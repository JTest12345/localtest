
namespace ManualWeighing {
    partial class SettingForm {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent() {
            this.panel1 = new System.Windows.Forms.Panel();
            this.setting_manual_folderpath_button = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.manual_folderpath_label = new System.Windows.Forms.Label();
            this.printer_groupBox = new System.Windows.Forms.GroupBox();
            this.zpl_radioButton = new System.Windows.Forms.RadioButton();
            this.bpac_radioButton = new System.Windows.Forms.RadioButton();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.panel2 = new System.Windows.Forms.Panel();
            this.balance_serialPortName_comboBox = new System.Windows.Forms.ComboBox();
            this.balance_plantcd_textBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.setting_balance_plantcd_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.label_format_comboBox = new System.Windows.Forms.ComboBox();
            this.label4 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.doorCheck_bitno_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.dio_deviceName_textBox = new System.Windows.Forms.TextBox();
            this.setting_dio_deviceName_tbutton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.panel7 = new System.Windows.Forms.Panel();
            this.exhaust_second_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label9 = new System.Windows.Forms.Label();
            this.manualMix_second_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.stable_second_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.system_fldpath_label = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.pmms_constr_label = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.panel6 = new System.Windows.Forms.Panel();
            this.gross_weight_checkBox = new System.Windows.Forms.CheckBox();
            this.gross_weight_allowableErrorGram_numericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label14 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.kanomax_serialPortName_comboBox = new System.Windows.Forms.ComboBox();
            this.panel1.SuspendLayout();
            this.printer_groupBox.SuspendLayout();
            this.panel2.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doorCheck_bitno_numericUpDown)).BeginInit();
            this.panel7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exhaust_second_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualMix_second_numericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.stable_second_numericUpDown)).BeginInit();
            this.panel3.SuspendLayout();
            this.panel6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gross_weight_allowableErrorGram_numericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.AutoSize = true;
            this.panel1.Controls.Add(this.setting_manual_folderpath_button);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Controls.Add(this.manual_folderpath_label);
            this.panel1.Location = new System.Drawing.Point(5, 5);
            this.panel1.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(790, 67);
            this.panel1.TabIndex = 13;
            // 
            // setting_manual_folderpath_button
            // 
            this.setting_manual_folderpath_button.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.setting_manual_folderpath_button.Location = new System.Drawing.Point(5, 11);
            this.setting_manual_folderpath_button.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.setting_manual_folderpath_button.Name = "setting_manual_folderpath_button";
            this.setting_manual_folderpath_button.Size = new System.Drawing.Size(84, 46);
            this.setting_manual_folderpath_button.TabIndex = 2;
            this.setting_manual_folderpath_button.Text = "設定";
            this.setting_manual_folderpath_button.UseVisualStyleBackColor = true;
            this.setting_manual_folderpath_button.Click += new System.EventHandler(this.setting_manual_folderpath_button_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(100, 7);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(611, 24);
            this.label1.TabIndex = 1;
            this.label1.Text = "Manualフォルダ(手配合に関するデータがあるベースとなるフォルダ)";
            // 
            // manual_folderpath_label
            // 
            this.manual_folderpath_label.AutoSize = true;
            this.manual_folderpath_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.manual_folderpath_label.Location = new System.Drawing.Point(100, 37);
            this.manual_folderpath_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.manual_folderpath_label.Name = "manual_folderpath_label";
            this.manual_folderpath_label.Size = new System.Drawing.Size(67, 24);
            this.manual_folderpath_label.TabIndex = 0;
            this.manual_folderpath_label.Text = "label1";
            // 
            // printer_groupBox
            // 
            this.printer_groupBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.printer_groupBox.Controls.Add(this.zpl_radioButton);
            this.printer_groupBox.Controls.Add(this.bpac_radioButton);
            this.printer_groupBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.printer_groupBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.printer_groupBox.Location = new System.Drawing.Point(5, 5);
            this.printer_groupBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.printer_groupBox.Name = "printer_groupBox";
            this.printer_groupBox.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.printer_groupBox.Size = new System.Drawing.Size(250, 60);
            this.printer_groupBox.TabIndex = 0;
            this.printer_groupBox.TabStop = false;
            this.printer_groupBox.Text = "プリンター種類";
            // 
            // zpl_radioButton
            // 
            this.zpl_radioButton.AutoSize = true;
            this.zpl_radioButton.Location = new System.Drawing.Point(150, 30);
            this.zpl_radioButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.zpl_radioButton.Name = "zpl_radioButton";
            this.zpl_radioButton.Size = new System.Drawing.Size(90, 28);
            this.zpl_radioButton.TabIndex = 1;
            this.zpl_radioButton.TabStop = true;
            this.zpl_radioButton.Tag = "zpl";
            this.zpl_radioButton.Text = "ゼブラ";
            this.zpl_radioButton.UseVisualStyleBackColor = true;
            this.zpl_radioButton.CheckedChanged += new System.EventHandler(this.printer_radioButton_CheckedChanged);
            // 
            // bpac_radioButton
            // 
            this.bpac_radioButton.AutoSize = true;
            this.bpac_radioButton.Location = new System.Drawing.Point(16, 30);
            this.bpac_radioButton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.bpac_radioButton.Name = "bpac_radioButton";
            this.bpac_radioButton.Size = new System.Drawing.Size(110, 28);
            this.bpac_radioButton.TabIndex = 0;
            this.bpac_radioButton.TabStop = true;
            this.bpac_radioButton.Tag = "bpac";
            this.bpac_radioButton.Text = "ブラザー";
            this.bpac_radioButton.UseVisualStyleBackColor = true;
            this.bpac_radioButton.CheckedChanged += new System.EventHandler(this.printer_radioButton_CheckedChanged);
            // 
            // panel2
            // 
            this.panel2.AutoSize = true;
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel2.Controls.Add(this.kanomax_serialPortName_comboBox);
            this.panel2.Controls.Add(this.label13);
            this.panel2.Controls.Add(this.balance_serialPortName_comboBox);
            this.panel2.Controls.Add(this.balance_plantcd_textBox);
            this.panel2.Controls.Add(this.label3);
            this.panel2.Controls.Add(this.setting_balance_plantcd_button);
            this.panel2.Controls.Add(this.label2);
            this.panel2.Location = new System.Drawing.Point(5, 79);
            this.panel2.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(864, 78);
            this.panel2.TabIndex = 15;
            // 
            // balance_serialPortName_comboBox
            // 
            this.balance_serialPortName_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.balance_serialPortName_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.balance_serialPortName_comboBox.FormattingEnabled = true;
            this.balance_serialPortName_comboBox.Location = new System.Drawing.Point(484, 37);
            this.balance_serialPortName_comboBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.balance_serialPortName_comboBox.Name = "balance_serialPortName_comboBox";
            this.balance_serialPortName_comboBox.Size = new System.Drawing.Size(164, 32);
            this.balance_serialPortName_comboBox.TabIndex = 22;
            this.balance_serialPortName_comboBox.SelectedIndexChanged += new System.EventHandler(this.balance_serialPortName_comboBox_SelectedIndexChanged);
            // 
            // balance_plantcd_textBox
            // 
            this.balance_plantcd_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.balance_plantcd_textBox.Location = new System.Drawing.Point(100, 37);
            this.balance_plantcd_textBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.balance_plantcd_textBox.Name = "balance_plantcd_textBox";
            this.balance_plantcd_textBox.Size = new System.Drawing.Size(164, 31);
            this.balance_plantcd_textBox.TabIndex = 16;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(485, 7);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(245, 36);
            this.label3.TabIndex = 1;
            this.label3.Text = "電子天秤ポート";
            // 
            // setting_balance_plantcd_button
            // 
            this.setting_balance_plantcd_button.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.setting_balance_plantcd_button.Location = new System.Drawing.Point(5, 11);
            this.setting_balance_plantcd_button.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.setting_balance_plantcd_button.Name = "setting_balance_plantcd_button";
            this.setting_balance_plantcd_button.Size = new System.Drawing.Size(84, 46);
            this.setting_balance_plantcd_button.TabIndex = 2;
            this.setting_balance_plantcd_button.Text = "設定";
            this.setting_balance_plantcd_button.UseVisualStyleBackColor = true;
            this.setting_balance_plantcd_button.Click += new System.EventHandler(this.setting_balance_plantcd_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(100, 7);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(365, 24);
            this.label2.TabIndex = 1;
            this.label2.Text = "電子天秤設備番号(例：TCSMR001)";
            // 
            // panel4
            // 
            this.panel4.AutoSize = true;
            this.panel4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(224)))), ((int)(((byte)(192)))));
            this.panel4.Controls.Add(this.label_format_comboBox);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.printer_groupBox);
            this.panel4.Location = new System.Drawing.Point(5, 166);
            this.panel4.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(630, 78);
            this.panel4.TabIndex = 17;
            // 
            // label_format_comboBox
            // 
            this.label_format_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.label_format_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label_format_comboBox.FormattingEnabled = true;
            this.label_format_comboBox.Location = new System.Drawing.Point(291, 37);
            this.label_format_comboBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.label_format_comboBox.Name = "label_format_comboBox";
            this.label_format_comboBox.Size = new System.Drawing.Size(330, 32);
            this.label_format_comboBox.TabIndex = 18;
            this.label_format_comboBox.SelectedIndexChanged += new System.EventHandler(this.label_format_comboBox_SelectedIndexChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(291, 5);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(168, 24);
            this.label4.TabIndex = 1;
            this.label4.Text = "ラベルフォーマット";
            // 
            // panel5
            // 
            this.panel5.AutoSize = true;
            this.panel5.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel5.Controls.Add(this.doorCheck_bitno_numericUpDown);
            this.panel5.Controls.Add(this.dio_deviceName_textBox);
            this.panel5.Controls.Add(this.setting_dio_deviceName_tbutton);
            this.panel5.Controls.Add(this.label6);
            this.panel5.Controls.Add(this.label5);
            this.panel5.Location = new System.Drawing.Point(5, 251);
            this.panel5.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(600, 118);
            this.panel5.TabIndex = 18;
            // 
            // doorCheck_bitno_numericUpDown
            // 
            this.doorCheck_bitno_numericUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.doorCheck_bitno_numericUpDown.Location = new System.Drawing.Point(400, 78);
            this.doorCheck_bitno_numericUpDown.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.doorCheck_bitno_numericUpDown.Maximum = new decimal(new int[] {
            15,
            0,
            0,
            0});
            this.doorCheck_bitno_numericUpDown.Name = "doorCheck_bitno_numericUpDown";
            this.doorCheck_bitno_numericUpDown.ReadOnly = true;
            this.doorCheck_bitno_numericUpDown.Size = new System.Drawing.Size(59, 31);
            this.doorCheck_bitno_numericUpDown.TabIndex = 3;
            this.doorCheck_bitno_numericUpDown.ValueChanged += new System.EventHandler(this.doorCheck_bitno_numericUpDown_ValueChanged);
            // 
            // dio_deviceName_textBox
            // 
            this.dio_deviceName_textBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.dio_deviceName_textBox.Location = new System.Drawing.Point(100, 37);
            this.dio_deviceName_textBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.dio_deviceName_textBox.Name = "dio_deviceName_textBox";
            this.dio_deviceName_textBox.Size = new System.Drawing.Size(164, 31);
            this.dio_deviceName_textBox.TabIndex = 16;
            // 
            // setting_dio_deviceName_tbutton
            // 
            this.setting_dio_deviceName_tbutton.Font = new System.Drawing.Font("MS UI Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.setting_dio_deviceName_tbutton.Location = new System.Drawing.Point(5, 17);
            this.setting_dio_deviceName_tbutton.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.setting_dio_deviceName_tbutton.Name = "setting_dio_deviceName_tbutton";
            this.setting_dio_deviceName_tbutton.Size = new System.Drawing.Size(84, 46);
            this.setting_dio_deviceName_tbutton.TabIndex = 2;
            this.setting_dio_deviceName_tbutton.Text = "設定";
            this.setting_dio_deviceName_tbutton.UseVisualStyleBackColor = true;
            this.setting_dio_deviceName_tbutton.Click += new System.EventHandler(this.setting_dio_deviceName_tbutton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(100, 83);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(256, 24);
            this.label6.TabIndex = 1;
            this.label6.Text = "ドアチェックbit番号(0～15)";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(100, 7);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(314, 24);
            this.label5.TabIndex = 1;
            this.label5.Text = "IOボードデバイス名(例：DIO000)";
            // 
            // panel7
            // 
            this.panel7.AutoSize = true;
            this.panel7.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel7.Controls.Add(this.exhaust_second_numericUpDown);
            this.panel7.Controls.Add(this.label9);
            this.panel7.Controls.Add(this.manualMix_second_numericUpDown);
            this.panel7.Controls.Add(this.stable_second_numericUpDown);
            this.panel7.Controls.Add(this.label8);
            this.panel7.Controls.Add(this.label7);
            this.panel7.Location = new System.Drawing.Point(5, 374);
            this.panel7.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel7.Name = "panel7";
            this.panel7.Size = new System.Drawing.Size(666, 77);
            this.panel7.TabIndex = 20;
            // 
            // exhaust_second_numericUpDown
            // 
            this.exhaust_second_numericUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.exhaust_second_numericUpDown.Location = new System.Drawing.Point(509, 37);
            this.exhaust_second_numericUpDown.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.exhaust_second_numericUpDown.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.exhaust_second_numericUpDown.Name = "exhaust_second_numericUpDown";
            this.exhaust_second_numericUpDown.Size = new System.Drawing.Size(84, 31);
            this.exhaust_second_numericUpDown.TabIndex = 5;
            this.exhaust_second_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.exhaust_second_numericUpDown.ValueChanged += new System.EventHandler(this.exhaust_second_numericUpDown_ValueChanged);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.Location = new System.Drawing.Point(466, 5);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(144, 24);
            this.label9.TabIndex = 4;
            this.label9.Text = "排気時間(秒)";
            // 
            // manualMix_second_numericUpDown
            // 
            this.manualMix_second_numericUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.manualMix_second_numericUpDown.Location = new System.Drawing.Point(291, 37);
            this.manualMix_second_numericUpDown.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.manualMix_second_numericUpDown.Maximum = new decimal(new int[] {
            1200,
            0,
            0,
            0});
            this.manualMix_second_numericUpDown.Name = "manualMix_second_numericUpDown";
            this.manualMix_second_numericUpDown.Size = new System.Drawing.Size(84, 31);
            this.manualMix_second_numericUpDown.TabIndex = 3;
            this.manualMix_second_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.manualMix_second_numericUpDown.ValueChanged += new System.EventHandler(this.manualMix_second_numericUpDown_ValueChanged);
            // 
            // stable_second_numericUpDown
            // 
            this.stable_second_numericUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stable_second_numericUpDown.Location = new System.Drawing.Point(50, 37);
            this.stable_second_numericUpDown.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.stable_second_numericUpDown.Maximum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.stable_second_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.stable_second_numericUpDown.Name = "stable_second_numericUpDown";
            this.stable_second_numericUpDown.Size = new System.Drawing.Size(84, 31);
            this.stable_second_numericUpDown.TabIndex = 3;
            this.stable_second_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.stable_second_numericUpDown.ValueChanged += new System.EventHandler(this.stable_second_numericUpDown_ValueChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(241, 5);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(168, 24);
            this.label8.TabIndex = 1;
            this.label8.Text = "手攪拌時間(秒)";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(5, 5);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(192, 24);
            this.label7.TabIndex = 1;
            this.label7.Text = "計量安定時間(秒)";
            // 
            // panel3
            // 
            this.panel3.AutoSize = true;
            this.panel3.Controls.Add(this.system_fldpath_label);
            this.panel3.Controls.Add(this.label12);
            this.panel3.Controls.Add(this.pmms_constr_label);
            this.panel3.Controls.Add(this.label10);
            this.panel3.Location = new System.Drawing.Point(5, 517);
            this.panel3.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(986, 132);
            this.panel3.TabIndex = 22;
            // 
            // system_fldpath_label
            // 
            this.system_fldpath_label.AutoSize = true;
            this.system_fldpath_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.system_fldpath_label.Location = new System.Drawing.Point(5, 97);
            this.system_fldpath_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.system_fldpath_label.Name = "system_fldpath_label";
            this.system_fldpath_label.Size = new System.Drawing.Size(161, 24);
            this.system_fldpath_label.TabIndex = 26;
            this.system_fldpath_label.Text = "SystemFolder...";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label12.Location = new System.Drawing.Point(5, 67);
            this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(280, 24);
            this.label12.TabIndex = 25;
            this.label12.Text = "システムファイルがあるフォルダ";
            // 
            // pmms_constr_label
            // 
            this.pmms_constr_label.AutoSize = true;
            this.pmms_constr_label.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.pmms_constr_label.Location = new System.Drawing.Point(5, 37);
            this.pmms_constr_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.pmms_constr_label.Name = "pmms_constr_label";
            this.pmms_constr_label.Size = new System.Drawing.Size(148, 24);
            this.pmms_constr_label.TabIndex = 22;
            this.pmms_constr_label.Text = "Data Source...";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(5, 7);
            this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(123, 24);
            this.label10.TabIndex = 21;
            this.label10.Text = "PMMS連携";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(640, 193);
            this.label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(191, 36);
            this.label11.TabIndex = 23;
            this.label11.Text = "※今はゼブラを選択しても\r\n　　印刷できません";
            // 
            // panel6
            // 
            this.panel6.AutoSize = true;
            this.panel6.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.panel6.Controls.Add(this.gross_weight_checkBox);
            this.panel6.Controls.Add(this.gross_weight_allowableErrorGram_numericUpDown);
            this.panel6.Controls.Add(this.label14);
            this.panel6.Location = new System.Drawing.Point(5, 456);
            this.panel6.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.panel6.Name = "panel6";
            this.panel6.Size = new System.Drawing.Size(666, 46);
            this.panel6.TabIndex = 24;
            // 
            // gross_weight_checkBox
            // 
            this.gross_weight_checkBox.AutoSize = true;
            this.gross_weight_checkBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.gross_weight_checkBox.Location = new System.Drawing.Point(9, 7);
            this.gross_weight_checkBox.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.gross_weight_checkBox.Name = "gross_weight_checkBox";
            this.gross_weight_checkBox.Size = new System.Drawing.Size(194, 28);
            this.gross_weight_checkBox.TabIndex = 4;
            this.gross_weight_checkBox.Text = "総重量判定する";
            this.gross_weight_checkBox.UseVisualStyleBackColor = true;
            this.gross_weight_checkBox.CheckedChanged += new System.EventHandler(this.gross_weight_checkBox_CheckedChanged);
            // 
            // gross_weight_allowableErrorGram_numericUpDown
            // 
            this.gross_weight_allowableErrorGram_numericUpDown.DecimalPlaces = 3;
            this.gross_weight_allowableErrorGram_numericUpDown.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.gross_weight_allowableErrorGram_numericUpDown.Increment = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.gross_weight_allowableErrorGram_numericUpDown.Location = new System.Drawing.Point(484, 5);
            this.gross_weight_allowableErrorGram_numericUpDown.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.gross_weight_allowableErrorGram_numericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            196608});
            this.gross_weight_allowableErrorGram_numericUpDown.Name = "gross_weight_allowableErrorGram_numericUpDown";
            this.gross_weight_allowableErrorGram_numericUpDown.Size = new System.Drawing.Size(100, 31);
            this.gross_weight_allowableErrorGram_numericUpDown.TabIndex = 3;
            this.gross_weight_allowableErrorGram_numericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            131072});
            this.gross_weight_allowableErrorGram_numericUpDown.ValueChanged += new System.EventHandler(this.gross_weight_allowableErrorGram_numericUpDown_ValueChanged);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label14.Location = new System.Drawing.Point(316, 10);
            this.label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(131, 24);
            this.label14.TabIndex = 1;
            this.label14.Text = "許容誤差(g)";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label13.Location = new System.Drawing.Point(672, 7);
            this.label13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(187, 24);
            this.label13.TabIndex = 23;
            this.label13.Text = "風速計通信ポート";
            // 
            // kanomax_serialPortName_comboBox
            // 
            this.kanomax_serialPortName_comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.kanomax_serialPortName_comboBox.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.kanomax_serialPortName_comboBox.FormattingEnabled = true;
            this.kanomax_serialPortName_comboBox.Location = new System.Drawing.Point(685, 37);
            this.kanomax_serialPortName_comboBox.Margin = new System.Windows.Forms.Padding(5);
            this.kanomax_serialPortName_comboBox.Name = "kanomax_serialPortName_comboBox";
            this.kanomax_serialPortName_comboBox.Size = new System.Drawing.Size(164, 32);
            this.kanomax_serialPortName_comboBox.TabIndex = 25;
            this.kanomax_serialPortName_comboBox.SelectedIndexChanged += new System.EventHandler(this.kanomax_serialPortName_comboBox_SelectedIndexChanged);
            // 
            // SettingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(891, 667);
            this.Controls.Add(this.panel6);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel7);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.panel1);
            this.Margin = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.Name = "SettingForm";
            this.Text = "設定";
            this.Load += new System.EventHandler(this.SettingForm_Load);
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.printer_groupBox.ResumeLayout(false);
            this.printer_groupBox.PerformLayout();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.doorCheck_bitno_numericUpDown)).EndInit();
            this.panel7.ResumeLayout(false);
            this.panel7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.exhaust_second_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.manualMix_second_numericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.stable_second_numericUpDown)).EndInit();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel6.ResumeLayout(false);
            this.panel6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gross_weight_allowableErrorGram_numericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button setting_manual_folderpath_button;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label manual_folderpath_label;
        private System.Windows.Forms.GroupBox printer_groupBox;
        private System.Windows.Forms.RadioButton zpl_radioButton;
        private System.Windows.Forms.RadioButton bpac_radioButton;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.TextBox balance_plantcd_textBox;
        private System.Windows.Forms.Button setting_balance_plantcd_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.ComboBox label_format_comboBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox dio_deviceName_textBox;
        private System.Windows.Forms.Button setting_dio_deviceName_tbutton;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown doorCheck_bitno_numericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Panel panel7;
        private System.Windows.Forms.NumericUpDown stable_second_numericUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown manualMix_second_numericUpDown;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ComboBox balance_serialPortName_comboBox;
        private System.Windows.Forms.NumericUpDown exhaust_second_numericUpDown;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Panel panel6;
        private System.Windows.Forms.CheckBox gross_weight_checkBox;
        private System.Windows.Forms.NumericUpDown gross_weight_allowableErrorGram_numericUpDown;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label system_fldpath_label;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label pmms_constr_label;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.ComboBox kanomax_serialPortName_comboBox;
        private System.Windows.Forms.Label label13;
    }
}