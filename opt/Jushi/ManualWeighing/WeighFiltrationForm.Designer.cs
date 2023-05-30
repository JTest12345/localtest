
namespace ManualWeighing {
    partial class WeighFiltrationForm {
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
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.use_buzai_groupBox = new System.Windows.Forms.GroupBox();
            this.buzai_fcode_textBox = new System.Windows.Forms.TextBox();
            this.buzai_lotno_textBox = new System.Windows.Forms.TextBox();
            this.buzai_name_textBox = new System.Windows.Forms.TextBox();
            this.input_buzaiQR_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.weigh_panel = new System.Windows.Forms.Panel();
            this.minus_allow_label = new System.Windows.Forms.Label();
            this.plus_allow_label = new System.Windows.Forms.Label();
            this.stable_time_label = new System.Windows.Forms.Label();
            this.label15 = new System.Windows.Forms.Label();
            this.stable_mark_label = new System.Windows.Forms.Label();
            this.within_label = new System.Windows.Forms.Label();
            this.label10 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.target_value_textBox = new System.Windows.Forms.TextBox();
            this.result_value_textBox = new System.Windows.Forms.TextBox();
            this.recipe_info_label = new System.Windows.Forms.Label();
            this.electronicBalance_serialPort = new System.IO.Ports.SerialPort(this.components);
            this.status_label = new System.Windows.Forms.Label();
            this.next_button = new System.Windows.Forms.Button();
            this.label14 = new System.Windows.Forms.Label();
            this.before_cup_weight_label = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.end_button = new System.Windows.Forms.Button();
            this.status_label_timer = new System.Windows.Forms.Timer(this.components);
            this.after_cup_weight_label = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.use_buzai_dataGridView = new System.Windows.Forms.DataGridView();
            this.ok = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lotno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allowableErrorGram = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.use_buzai_groupBox.SuspendLayout();
            this.weigh_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.use_buzai_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // use_buzai_groupBox
            // 
            this.use_buzai_groupBox.Controls.Add(this.buzai_fcode_textBox);
            this.use_buzai_groupBox.Controls.Add(this.buzai_lotno_textBox);
            this.use_buzai_groupBox.Controls.Add(this.buzai_name_textBox);
            this.use_buzai_groupBox.Controls.Add(this.input_buzaiQR_textBox);
            this.use_buzai_groupBox.Controls.Add(this.label1);
            this.use_buzai_groupBox.Controls.Add(this.label4);
            this.use_buzai_groupBox.Controls.Add(this.label2);
            this.use_buzai_groupBox.Controls.Add(this.label3);
            this.use_buzai_groupBox.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.use_buzai_groupBox.Location = new System.Drawing.Point(3, 355);
            this.use_buzai_groupBox.Name = "use_buzai_groupBox";
            this.use_buzai_groupBox.Size = new System.Drawing.Size(375, 270);
            this.use_buzai_groupBox.TabIndex = 5;
            this.use_buzai_groupBox.TabStop = false;
            this.use_buzai_groupBox.Text = "使用部材QR";
            // 
            // buzai_fcode_textBox
            // 
            this.buzai_fcode_textBox.BackColor = System.Drawing.SystemColors.Control;
            this.buzai_fcode_textBox.Enabled = false;
            this.buzai_fcode_textBox.Location = new System.Drawing.Point(120, 157);
            this.buzai_fcode_textBox.Name = "buzai_fcode_textBox";
            this.buzai_fcode_textBox.Size = new System.Drawing.Size(250, 48);
            this.buzai_fcode_textBox.TabIndex = 7;
            // 
            // buzai_lotno_textBox
            // 
            this.buzai_lotno_textBox.BackColor = System.Drawing.SystemColors.Control;
            this.buzai_lotno_textBox.Enabled = false;
            this.buzai_lotno_textBox.Location = new System.Drawing.Point(120, 217);
            this.buzai_lotno_textBox.Name = "buzai_lotno_textBox";
            this.buzai_lotno_textBox.Size = new System.Drawing.Size(250, 48);
            this.buzai_lotno_textBox.TabIndex = 6;
            // 
            // buzai_name_textBox
            // 
            this.buzai_name_textBox.BackColor = System.Drawing.SystemColors.Control;
            this.buzai_name_textBox.Enabled = false;
            this.buzai_name_textBox.Location = new System.Drawing.Point(120, 97);
            this.buzai_name_textBox.Name = "buzai_name_textBox";
            this.buzai_name_textBox.Size = new System.Drawing.Size(250, 48);
            this.buzai_name_textBox.TabIndex = 5;
            // 
            // input_buzaiQR_textBox
            // 
            this.input_buzaiQR_textBox.Enabled = false;
            this.input_buzaiQR_textBox.Location = new System.Drawing.Point(120, 37);
            this.input_buzaiQR_textBox.Name = "input_buzaiQR_textBox";
            this.input_buzaiQR_textBox.Size = new System.Drawing.Size(250, 48);
            this.input_buzaiQR_textBox.TabIndex = 1;
            this.input_buzaiQR_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.input_buzaiQR_textBox_KeyPress);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(5, 40);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 41);
            this.label1.TabIndex = 0;
            this.label1.Text = "QR";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(5, 220);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(99, 41);
            this.label4.TabIndex = 3;
            this.label4.Text = "ロット";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(5, 100);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 41);
            this.label2.TabIndex = 1;
            this.label2.Text = "型番";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(5, 160);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(114, 41);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fコード";
            // 
            // weigh_panel
            // 
            this.weigh_panel.BackColor = System.Drawing.SystemColors.Control;
            this.weigh_panel.Controls.Add(this.minus_allow_label);
            this.weigh_panel.Controls.Add(this.plus_allow_label);
            this.weigh_panel.Controls.Add(this.stable_time_label);
            this.weigh_panel.Controls.Add(this.label15);
            this.weigh_panel.Controls.Add(this.stable_mark_label);
            this.weigh_panel.Controls.Add(this.within_label);
            this.weigh_panel.Controls.Add(this.label10);
            this.weigh_panel.Controls.Add(this.label9);
            this.weigh_panel.Controls.Add(this.label8);
            this.weigh_panel.Controls.Add(this.label7);
            this.weigh_panel.Controls.Add(this.label6);
            this.weigh_panel.Controls.Add(this.label5);
            this.weigh_panel.Controls.Add(this.target_value_textBox);
            this.weigh_panel.Controls.Add(this.result_value_textBox);
            this.weigh_panel.Location = new System.Drawing.Point(385, 375);
            this.weigh_panel.Name = "weigh_panel";
            this.weigh_panel.Size = new System.Drawing.Size(600, 180);
            this.weigh_panel.TabIndex = 10;
            // 
            // minus_allow_label
            // 
            this.minus_allow_label.AutoSize = true;
            this.minus_allow_label.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.minus_allow_label.Location = new System.Drawing.Point(405, 30);
            this.minus_allow_label.Name = "minus_allow_label";
            this.minus_allow_label.Size = new System.Drawing.Size(87, 31);
            this.minus_allow_label.TabIndex = 19;
            this.minus_allow_label.Text = "label16";
            // 
            // plus_allow_label
            // 
            this.plus_allow_label.AutoSize = true;
            this.plus_allow_label.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.plus_allow_label.Location = new System.Drawing.Point(405, 1);
            this.plus_allow_label.Name = "plus_allow_label";
            this.plus_allow_label.Size = new System.Drawing.Size(87, 31);
            this.plus_allow_label.TabIndex = 18;
            this.plus_allow_label.Text = "label16";
            // 
            // stable_time_label
            // 
            this.stable_time_label.AutoSize = true;
            this.stable_time_label.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stable_time_label.ForeColor = System.Drawing.Color.Black;
            this.stable_time_label.Location = new System.Drawing.Point(425, 128);
            this.stable_time_label.Name = "stable_time_label";
            this.stable_time_label.Size = new System.Drawing.Size(112, 41);
            this.stable_time_label.TabIndex = 17;
            this.stable_time_label.Text = "10.789";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label15.Location = new System.Drawing.Point(360, 120);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(66, 54);
            this.label15.TabIndex = 16;
            this.label15.Text = "安定\r\n時間";
            // 
            // stable_mark_label
            // 
            this.stable_mark_label.AutoSize = true;
            this.stable_mark_label.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stable_mark_label.ForeColor = System.Drawing.Color.LightGray;
            this.stable_mark_label.Location = new System.Drawing.Point(260, 115);
            this.stable_mark_label.Name = "stable_mark_label";
            this.stable_mark_label.Size = new System.Drawing.Size(92, 64);
            this.stable_mark_label.TabIndex = 15;
            this.stable_mark_label.Text = "●";
            // 
            // within_label
            // 
            this.within_label.AutoSize = true;
            this.within_label.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.within_label.ForeColor = System.Drawing.Color.LightGray;
            this.within_label.Location = new System.Drawing.Point(70, 115);
            this.within_label.Name = "within_label";
            this.within_label.Size = new System.Drawing.Size(92, 64);
            this.within_label.TabIndex = 14;
            this.within_label.Text = "●";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(190, 120);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(66, 54);
            this.label10.TabIndex = 13;
            this.label10.Text = "天秤\r\n安定";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.Location = new System.Drawing.Point(5, 120);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(66, 54);
            this.label9.TabIndex = 11;
            this.label9.Text = "秤量\r\n結果";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(365, 60);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(34, 41);
            this.label8.TabIndex = 12;
            this.label8.Text = "g";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(365, 5);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(34, 41);
            this.label7.TabIndex = 11;
            this.label7.Text = "g";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(5, 60);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(99, 41);
            this.label6.TabIndex = 10;
            this.label6.Text = "秤量値";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(5, 5);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(99, 41);
            this.label5.TabIndex = 9;
            this.label5.Text = "目標値";
            // 
            // target_value_textBox
            // 
            this.target_value_textBox.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.target_value_textBox.Location = new System.Drawing.Point(110, 2);
            this.target_value_textBox.Name = "target_value_textBox";
            this.target_value_textBox.ReadOnly = true;
            this.target_value_textBox.Size = new System.Drawing.Size(250, 48);
            this.target_value_textBox.TabIndex = 8;
            // 
            // result_value_textBox
            // 
            this.result_value_textBox.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.result_value_textBox.Location = new System.Drawing.Point(110, 57);
            this.result_value_textBox.Name = "result_value_textBox";
            this.result_value_textBox.ReadOnly = true;
            this.result_value_textBox.Size = new System.Drawing.Size(250, 48);
            this.result_value_textBox.TabIndex = 7;
            // 
            // recipe_info_label
            // 
            this.recipe_info_label.AutoSize = true;
            this.recipe_info_label.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.recipe_info_label.Location = new System.Drawing.Point(3, 3);
            this.recipe_info_label.Name = "recipe_info_label";
            this.recipe_info_label.Size = new System.Drawing.Size(462, 144);
            this.recipe_info_label.TabIndex = 85;
            this.recipe_info_label.Text = "recipe_info_label\r\n機種：1234567890123456789012345\r\nLotNo\r\n作業者";
            // 
            // electronicBalance_serialPort
            // 
            this.electronicBalance_serialPort.BaudRate = 2400;
            this.electronicBalance_serialPort.DataBits = 7;
            this.electronicBalance_serialPort.Parity = System.IO.Ports.Parity.Even;
            this.electronicBalance_serialPort.ReadTimeout = 1000;
            this.electronicBalance_serialPort.WriteTimeout = 3000;
            this.electronicBalance_serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.electronicBalance_serialPort_DataReceived);
            // 
            // status_label
            // 
            this.status_label.AutoSize = true;
            this.status_label.BackColor = System.Drawing.Color.LightGreen;
            this.status_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.status_label.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.status_label.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.status_label.Location = new System.Drawing.Point(5, 250);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(436, 95);
            this.status_label.TabIndex = 90;
            this.status_label.Text = "status_label\r\nあいうえおかきくけこさしすせそたちつてと\r\nあいうえおかきくけこさしすせそたちつてと";
            // 
            // next_button
            // 
            this.next_button.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.next_button.Location = new System.Drawing.Point(692, 571);
            this.next_button.Name = "next_button";
            this.next_button.Size = new System.Drawing.Size(100, 49);
            this.next_button.TabIndex = 91;
            this.next_button.Text = "Next";
            this.next_button.UseVisualStyleBackColor = true;
            this.next_button.Visible = false;
            this.next_button.Click += new System.EventHandler(this.next_button_Click);
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial Unicode MS", 18F);
            this.label14.ForeColor = System.Drawing.Color.Blue;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(635, 565);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(28, 33);
            this.label14.TabIndex = 94;
            this.label14.Text = "g";
            // 
            // before_cup_weight_label
            // 
            this.before_cup_weight_label.AutoSize = true;
            this.before_cup_weight_label.Font = new System.Drawing.Font("Arial Unicode MS", 18F);
            this.before_cup_weight_label.ForeColor = System.Drawing.Color.Blue;
            this.before_cup_weight_label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.before_cup_weight_label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.before_cup_weight_label.Location = new System.Drawing.Point(550, 565);
            this.before_cup_weight_label.Name = "before_cup_weight_label";
            this.before_cup_weight_label.Size = new System.Drawing.Size(78, 33);
            this.before_cup_weight_label.TabIndex = 93;
            this.before_cup_weight_label.Text = "---.----";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial Unicode MS", 18F);
            this.label13.ForeColor = System.Drawing.Color.Blue;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(385, 565);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(111, 33);
            this.label13.TabIndex = 92;
            this.label13.Text = "空カップ";
            // 
            // end_button
            // 
            this.end_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.end_button.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.end_button.Location = new System.Drawing.Point(1152, 600);
            this.end_button.Name = "end_button";
            this.end_button.Size = new System.Drawing.Size(80, 49);
            this.end_button.TabIndex = 95;
            this.end_button.Text = "Exit";
            this.end_button.UseVisualStyleBackColor = true;
            this.end_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.end_button_MouseDown);
            // 
            // status_label_timer
            // 
            this.status_label_timer.Enabled = true;
            this.status_label_timer.Interval = 1000;
            this.status_label_timer.Tick += new System.EventHandler(this.status_label_timer_Tick);
            // 
            // after_cup_weight_label
            // 
            this.after_cup_weight_label.AutoSize = true;
            this.after_cup_weight_label.Font = new System.Drawing.Font("Arial Unicode MS", 18F);
            this.after_cup_weight_label.ForeColor = System.Drawing.Color.Fuchsia;
            this.after_cup_weight_label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.after_cup_weight_label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.after_cup_weight_label.Location = new System.Drawing.Point(550, 605);
            this.after_cup_weight_label.Name = "after_cup_weight_label";
            this.after_cup_weight_label.Size = new System.Drawing.Size(78, 33);
            this.after_cup_weight_label.TabIndex = 96;
            this.after_cup_weight_label.Text = "---.----";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Arial Unicode MS", 18F);
            this.label11.ForeColor = System.Drawing.Color.Fuchsia;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(385, 605);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(159, 33);
            this.label11.TabIndex = 97;
            this.label11.Text = "ろ過後カップ";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial Unicode MS", 18F);
            this.label12.ForeColor = System.Drawing.Color.Fuchsia;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(635, 605);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(28, 33);
            this.label12.TabIndex = 98;
            this.label12.Text = "g";
            // 
            // use_buzai_dataGridView
            // 
            this.use_buzai_dataGridView.AllowUserToAddRows = false;
            this.use_buzai_dataGridView.AllowUserToDeleteRows = false;
            this.use_buzai_dataGridView.AllowUserToResizeColumns = false;
            this.use_buzai_dataGridView.AllowUserToResizeRows = false;
            this.use_buzai_dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.use_buzai_dataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.use_buzai_dataGridView.AutoSizeRowsMode = System.Windows.Forms.DataGridViewAutoSizeRowsMode.AllCells;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.use_buzai_dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.use_buzai_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.use_buzai_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ok,
            this.name,
            this.fcode,
            this.lotno,
            this.target,
            this.allowableErrorGram,
            this.result});
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle2.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.use_buzai_dataGridView.DefaultCellStyle = dataGridViewCellStyle2;
            this.use_buzai_dataGridView.Enabled = false;
            this.use_buzai_dataGridView.Location = new System.Drawing.Point(470, 3);
            this.use_buzai_dataGridView.Name = "use_buzai_dataGridView";
            this.use_buzai_dataGridView.ReadOnly = true;
            this.use_buzai_dataGridView.RowTemplate.Height = 21;
            this.use_buzai_dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.use_buzai_dataGridView.Size = new System.Drawing.Size(760, 200);
            this.use_buzai_dataGridView.TabIndex = 99;
            // 
            // ok
            // 
            this.ok.HeaderText = "OK";
            this.ok.Name = "ok";
            this.ok.ReadOnly = true;
            this.ok.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ok.Width = 48;
            // 
            // name
            // 
            this.name.HeaderText = "部材型番";
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.name.Width = 102;
            // 
            // fcode
            // 
            this.fcode.HeaderText = "Fコード";
            this.fcode.Name = "fcode";
            this.fcode.ReadOnly = true;
            this.fcode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.fcode.Width = 77;
            // 
            // lotno
            // 
            this.lotno.HeaderText = "部材ロット";
            this.lotno.Name = "lotno";
            this.lotno.ReadOnly = true;
            this.lotno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.lotno.Width = 102;
            // 
            // target
            // 
            this.target.HeaderText = "目標値";
            this.target.Name = "target";
            this.target.ReadOnly = true;
            this.target.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.target.Width = 81;
            // 
            // allowableErrorGram
            // 
            this.allowableErrorGram.HeaderText = "許容誤差";
            this.allowableErrorGram.Name = "allowableErrorGram";
            this.allowableErrorGram.ReadOnly = true;
            this.allowableErrorGram.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.allowableErrorGram.Width = 102;
            // 
            // result
            // 
            this.result.HeaderText = "計量結果";
            this.result.Name = "result";
            this.result.ReadOnly = true;
            this.result.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.result.Width = 102;
            // 
            // WeighFiltrationForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1234, 651);
            this.Controls.Add(this.use_buzai_dataGridView);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.after_cup_weight_label);
            this.Controls.Add(this.end_button);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.before_cup_weight_label);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.next_button);
            this.Controls.Add(this.status_label);
            this.Controls.Add(this.recipe_info_label);
            this.Controls.Add(this.weigh_panel);
            this.Controls.Add(this.use_buzai_groupBox);
            this.Name = "WeighFiltrationForm";
            this.Text = "ろ過後秤量";
            this.Load += new System.EventHandler(this.WeighFiltrationForm_Load);
            this.use_buzai_groupBox.ResumeLayout(false);
            this.use_buzai_groupBox.PerformLayout();
            this.weigh_panel.ResumeLayout(false);
            this.weigh_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.use_buzai_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox use_buzai_groupBox;
        private System.Windows.Forms.TextBox buzai_fcode_textBox;
        private System.Windows.Forms.TextBox buzai_lotno_textBox;
        private System.Windows.Forms.TextBox buzai_name_textBox;
        private System.Windows.Forms.TextBox input_buzaiQR_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Panel weigh_panel;
        private System.Windows.Forms.Label minus_allow_label;
        private System.Windows.Forms.Label plus_allow_label;
        private System.Windows.Forms.Label stable_time_label;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label stable_mark_label;
        private System.Windows.Forms.Label within_label;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox target_value_textBox;
        private System.Windows.Forms.TextBox result_value_textBox;
        private System.Windows.Forms.Label recipe_info_label;
        private System.IO.Ports.SerialPort electronicBalance_serialPort;
        private System.Windows.Forms.Label status_label;
        private System.Windows.Forms.Button next_button;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label before_cup_weight_label;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.Button end_button;
        private System.Windows.Forms.Timer status_label_timer;
        private System.Windows.Forms.Label after_cup_weight_label;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.DataGridView use_buzai_dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn ok;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn fcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn lotno;
        private System.Windows.Forms.DataGridViewTextBoxColumn target;
        private System.Windows.Forms.DataGridViewTextBoxColumn allowableErrorGram;
        private System.Windows.Forms.DataGridViewTextBoxColumn result;
    }
}