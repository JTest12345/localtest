
namespace ManualWeighing {
    partial class WeighForm {
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WeighForm));
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.use_buzai_groupBox = new System.Windows.Forms.GroupBox();
            this.buzai_fcode_textBox = new System.Windows.Forms.TextBox();
            this.buzai_lotno_textBox = new System.Windows.Forms.TextBox();
            this.buzai_name_textBox = new System.Windows.Forms.TextBox();
            this.input_buzaiQR_textBox = new System.Windows.Forms.TextBox();
            this.result_value_textBox = new System.Windows.Forms.TextBox();
            this.target_value_textBox = new System.Windows.Forms.TextBox();
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
            this.next_button = new System.Windows.Forms.Button();
            this.label12 = new System.Windows.Forms.Label();
            this.label14 = new System.Windows.Forms.Label();
            this.gross_weight_label = new System.Windows.Forms.Label();
            this.cup_weight_label = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label13 = new System.Windows.Forms.Label();
            this.use_buzai_dataGridView = new System.Windows.Forms.DataGridView();
            this.ok = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.name = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fcode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.lotno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.target = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.allowableErrorGram = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.result = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.recipe_info_label = new System.Windows.Forms.Label();
            this.electronicBalance_serialPort = new System.IO.Ports.SerialPort(this.components);
            this.door_check_timer = new System.Windows.Forms.Timer(this.components);
            this.door_check_label = new System.Windows.Forms.Label();
            this.door_check_error_label = new System.Windows.Forms.Label();
            this.end_button = new System.Windows.Forms.Button();
            this.hand_mix_time_label = new System.Windows.Forms.Label();
            this.status_label = new System.Windows.Forms.Label();
            this.status_label_timer = new System.Windows.Forms.Timer(this.components);
            this.fusoku_label = new System.Windows.Forms.Label();
            this.fusoku_value_textBox = new System.Windows.Forms.TextBox();
            this.kanomax_realtime_value = new System.Windows.Forms.Timer(this.components);
            this.kanomax_fusoku_serialPort = new System.IO.Ports.SerialPort(this.components);
            this.kanomaxNG_label = new System.Windows.Forms.Label();
            this.use_buzai_groupBox.SuspendLayout();
            this.weigh_panel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.use_buzai_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(8, 60);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(86, 61);
            this.label1.TabIndex = 0;
            this.label1.Text = "QR";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(8, 150);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(109, 61);
            this.label2.TabIndex = 1;
            this.label2.Text = "型番";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(8, 240);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(173, 61);
            this.label3.TabIndex = 2;
            this.label3.Text = "Fコード";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(8, 330);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(150, 61);
            this.label4.TabIndex = 3;
            this.label4.Text = "ロット";
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
            this.use_buzai_groupBox.Location = new System.Drawing.Point(5, 615);
            this.use_buzai_groupBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.use_buzai_groupBox.Name = "use_buzai_groupBox";
            this.use_buzai_groupBox.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.use_buzai_groupBox.Size = new System.Drawing.Size(625, 405);
            this.use_buzai_groupBox.TabIndex = 4;
            this.use_buzai_groupBox.TabStop = false;
            this.use_buzai_groupBox.Text = "使用部材QR";
            // 
            // buzai_fcode_textBox
            // 
            this.buzai_fcode_textBox.BackColor = System.Drawing.SystemColors.Control;
            this.buzai_fcode_textBox.Enabled = false;
            this.buzai_fcode_textBox.Location = new System.Drawing.Point(200, 236);
            this.buzai_fcode_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buzai_fcode_textBox.Name = "buzai_fcode_textBox";
            this.buzai_fcode_textBox.Size = new System.Drawing.Size(414, 68);
            this.buzai_fcode_textBox.TabIndex = 7;
            // 
            // buzai_lotno_textBox
            // 
            this.buzai_lotno_textBox.BackColor = System.Drawing.SystemColors.Control;
            this.buzai_lotno_textBox.Enabled = false;
            this.buzai_lotno_textBox.Location = new System.Drawing.Point(200, 326);
            this.buzai_lotno_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buzai_lotno_textBox.Name = "buzai_lotno_textBox";
            this.buzai_lotno_textBox.Size = new System.Drawing.Size(414, 68);
            this.buzai_lotno_textBox.TabIndex = 6;
            // 
            // buzai_name_textBox
            // 
            this.buzai_name_textBox.BackColor = System.Drawing.SystemColors.Control;
            this.buzai_name_textBox.Enabled = false;
            this.buzai_name_textBox.Location = new System.Drawing.Point(200, 146);
            this.buzai_name_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.buzai_name_textBox.Name = "buzai_name_textBox";
            this.buzai_name_textBox.Size = new System.Drawing.Size(414, 68);
            this.buzai_name_textBox.TabIndex = 5;
            // 
            // input_buzaiQR_textBox
            // 
            this.input_buzaiQR_textBox.Enabled = false;
            this.input_buzaiQR_textBox.Location = new System.Drawing.Point(200, 56);
            this.input_buzaiQR_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.input_buzaiQR_textBox.Name = "input_buzaiQR_textBox";
            this.input_buzaiQR_textBox.Size = new System.Drawing.Size(414, 68);
            this.input_buzaiQR_textBox.TabIndex = 1;
            this.input_buzaiQR_textBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.input_buzaiQR_textBox_KeyPress);
            // 
            // result_value_textBox
            // 
            this.result_value_textBox.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.result_value_textBox.Location = new System.Drawing.Point(183, 86);
            this.result_value_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.result_value_textBox.Name = "result_value_textBox";
            this.result_value_textBox.ReadOnly = true;
            this.result_value_textBox.Size = new System.Drawing.Size(414, 68);
            this.result_value_textBox.TabIndex = 7;
            // 
            // target_value_textBox
            // 
            this.target_value_textBox.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.target_value_textBox.Location = new System.Drawing.Point(183, 3);
            this.target_value_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.target_value_textBox.Name = "target_value_textBox";
            this.target_value_textBox.ReadOnly = true;
            this.target_value_textBox.Size = new System.Drawing.Size(414, 68);
            this.target_value_textBox.TabIndex = 8;
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
            this.weigh_panel.Location = new System.Drawing.Point(642, 645);
            this.weigh_panel.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.weigh_panel.Name = "weigh_panel";
            this.weigh_panel.Size = new System.Drawing.Size(1000, 270);
            this.weigh_panel.TabIndex = 9;
            // 
            // minus_allow_label
            // 
            this.minus_allow_label.AutoSize = true;
            this.minus_allow_label.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.minus_allow_label.Location = new System.Drawing.Point(675, 45);
            this.minus_allow_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.minus_allow_label.Name = "minus_allow_label";
            this.minus_allow_label.Size = new System.Drawing.Size(132, 48);
            this.minus_allow_label.TabIndex = 19;
            this.minus_allow_label.Text = "label16";
            // 
            // plus_allow_label
            // 
            this.plus_allow_label.AutoSize = true;
            this.plus_allow_label.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.plus_allow_label.Location = new System.Drawing.Point(675, 2);
            this.plus_allow_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.plus_allow_label.Name = "plus_allow_label";
            this.plus_allow_label.Size = new System.Drawing.Size(132, 48);
            this.plus_allow_label.TabIndex = 18;
            this.plus_allow_label.Text = "label16";
            // 
            // stable_time_label
            // 
            this.stable_time_label.AutoSize = true;
            this.stable_time_label.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stable_time_label.ForeColor = System.Drawing.Color.Black;
            this.stable_time_label.Location = new System.Drawing.Point(708, 192);
            this.stable_time_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.stable_time_label.Name = "stable_time_label";
            this.stable_time_label.Size = new System.Drawing.Size(166, 61);
            this.stable_time_label.TabIndex = 17;
            this.stable_time_label.Text = "10.789";
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label15.Location = new System.Drawing.Point(600, 180);
            this.label15.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(100, 82);
            this.label15.TabIndex = 16;
            this.label15.Text = "安定\r\n時間";
            // 
            // stable_mark_label
            // 
            this.stable_mark_label.AutoSize = true;
            this.stable_mark_label.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.stable_mark_label.ForeColor = System.Drawing.Color.LightGray;
            this.stable_mark_label.Location = new System.Drawing.Point(433, 172);
            this.stable_mark_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.stable_mark_label.Name = "stable_mark_label";
            this.stable_mark_label.Size = new System.Drawing.Size(138, 97);
            this.stable_mark_label.TabIndex = 15;
            this.stable_mark_label.Text = "●";
            // 
            // within_label
            // 
            this.within_label.AutoSize = true;
            this.within_label.Font = new System.Drawing.Font("MS UI Gothic", 48F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.within_label.ForeColor = System.Drawing.Color.LightGray;
            this.within_label.Location = new System.Drawing.Point(117, 172);
            this.within_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.within_label.Name = "within_label";
            this.within_label.Size = new System.Drawing.Size(138, 97);
            this.within_label.TabIndex = 14;
            this.within_label.Text = "●";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(317, 180);
            this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(100, 82);
            this.label10.TabIndex = 13;
            this.label10.Text = "天秤\r\n安定";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.Location = new System.Drawing.Point(8, 180);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(100, 82);
            this.label9.TabIndex = 11;
            this.label9.Text = "秤量\r\n結果";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(608, 90);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(52, 61);
            this.label8.TabIndex = 12;
            this.label8.Text = "g";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(608, 8);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(52, 61);
            this.label7.TabIndex = 11;
            this.label7.Text = "g";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(8, 90);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(150, 61);
            this.label6.TabIndex = 10;
            this.label6.Text = "秤量値";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(8, 8);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(150, 61);
            this.label5.TabIndex = 9;
            this.label5.Text = "目標値";
            // 
            // next_button
            // 
            this.next_button.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.next_button.Location = new System.Drawing.Point(1153, 939);
            this.next_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.next_button.Name = "next_button";
            this.next_button.Size = new System.Drawing.Size(167, 74);
            this.next_button.TabIndex = 0;
            this.next_button.Text = "Next";
            this.next_button.UseVisualStyleBackColor = true;
            this.next_button.Visible = false;
            this.next_button.Click += new System.EventHandler(this.next_button_Click);
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label12.ForeColor = System.Drawing.Color.Orange;
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(983, 922);
            this.label12.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(37, 40);
            this.label12.TabIndex = 81;
            this.label12.Text = "g";
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label14.ForeColor = System.Drawing.Color.Blue;
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(983, 975);
            this.label14.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(37, 40);
            this.label14.TabIndex = 82;
            this.label14.Text = "g";
            // 
            // gross_weight_label
            // 
            this.gross_weight_label.AutoSize = true;
            this.gross_weight_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.gross_weight_label.ForeColor = System.Drawing.Color.Orange;
            this.gross_weight_label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.gross_weight_label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.gross_weight_label.Location = new System.Drawing.Point(825, 922);
            this.gross_weight_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.gross_weight_label.Name = "gross_weight_label";
            this.gross_weight_label.Size = new System.Drawing.Size(111, 40);
            this.gross_weight_label.TabIndex = 79;
            this.gross_weight_label.Text = "---.----";
            // 
            // cup_weight_label
            // 
            this.cup_weight_label.AutoSize = true;
            this.cup_weight_label.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.cup_weight_label.ForeColor = System.Drawing.Color.Blue;
            this.cup_weight_label.ImageAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.cup_weight_label.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.cup_weight_label.Location = new System.Drawing.Point(825, 975);
            this.cup_weight_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.cup_weight_label.Name = "cup_weight_label";
            this.cup_weight_label.Size = new System.Drawing.Size(111, 40);
            this.cup_weight_label.TabIndex = 80;
            this.cup_weight_label.Text = "---.----";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label11.ForeColor = System.Drawing.Color.Orange;
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(642, 922);
            this.label11.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(125, 40);
            this.label11.TabIndex = 77;
            this.label11.Text = "総重量";
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Microsoft Sans Serif", 18F);
            this.label13.ForeColor = System.Drawing.Color.Blue;
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(642, 975);
            this.label13.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(91, 40);
            this.label13.TabIndex = 78;
            this.label13.Text = "カップ";
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
            this.use_buzai_dataGridView.Location = new System.Drawing.Point(783, 4);
            this.use_buzai_dataGridView.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.use_buzai_dataGridView.Name = "use_buzai_dataGridView";
            this.use_buzai_dataGridView.ReadOnly = true;
            this.use_buzai_dataGridView.RowHeadersWidth = 51;
            this.use_buzai_dataGridView.RowTemplate.Height = 21;
            this.use_buzai_dataGridView.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.use_buzai_dataGridView.Size = new System.Drawing.Size(1560, 300);
            this.use_buzai_dataGridView.TabIndex = 83;
            // 
            // ok
            // 
            this.ok.HeaderText = "OK";
            this.ok.MinimumWidth = 6;
            this.ok.Name = "ok";
            this.ok.ReadOnly = true;
            this.ok.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.ok.Width = 69;
            // 
            // name
            // 
            this.name.HeaderText = "部材型番";
            this.name.MinimumWidth = 6;
            this.name.Name = "name";
            this.name.ReadOnly = true;
            this.name.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.name.Width = 152;
            // 
            // fcode
            // 
            this.fcode.HeaderText = "Fコード";
            this.fcode.MinimumWidth = 6;
            this.fcode.Name = "fcode";
            this.fcode.ReadOnly = true;
            this.fcode.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.fcode.Width = 113;
            // 
            // lotno
            // 
            this.lotno.HeaderText = "部材ロット";
            this.lotno.MinimumWidth = 6;
            this.lotno.Name = "lotno";
            this.lotno.ReadOnly = true;
            this.lotno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.lotno.Width = 153;
            // 
            // target
            // 
            this.target.HeaderText = "目標値";
            this.target.MinimumWidth = 6;
            this.target.Name = "target";
            this.target.ReadOnly = true;
            this.target.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.target.Width = 120;
            // 
            // allowableErrorGram
            // 
            this.allowableErrorGram.HeaderText = "許容誤差";
            this.allowableErrorGram.MinimumWidth = 6;
            this.allowableErrorGram.Name = "allowableErrorGram";
            this.allowableErrorGram.ReadOnly = true;
            this.allowableErrorGram.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.allowableErrorGram.Width = 152;
            // 
            // result
            // 
            this.result.HeaderText = "計量結果";
            this.result.MinimumWidth = 6;
            this.result.Name = "result";
            this.result.ReadOnly = true;
            this.result.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.result.Width = 152;
            // 
            // recipe_info_label
            // 
            this.recipe_info_label.AutoSize = true;
            this.recipe_info_label.BackColor = System.Drawing.SystemColors.Control;
            this.recipe_info_label.Font = new System.Drawing.Font("メイリオ", 18F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.recipe_info_label.Location = new System.Drawing.Point(5, 4);
            this.recipe_info_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.recipe_info_label.Name = "recipe_info_label";
            this.recipe_info_label.Size = new System.Drawing.Size(660, 540);
            this.recipe_info_label.TabIndex = 84;
            this.recipe_info_label.Text = resources.GetString("recipe_info_label.Text");
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
            // door_check_timer
            // 
            this.door_check_timer.Interval = 500;
            this.door_check_timer.Tick += new System.EventHandler(this.door_check_timer_Tick);
            // 
            // door_check_label
            // 
            this.door_check_label.BackColor = System.Drawing.Color.Yellow;
            this.door_check_label.Font = new System.Drawing.Font("HGP創英角ｺﾞｼｯｸUB", 36F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.door_check_label.ForeColor = System.Drawing.Color.Red;
            this.door_check_label.Location = new System.Drawing.Point(1820, 368);
            this.door_check_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.door_check_label.Name = "door_check_label";
            this.door_check_label.Size = new System.Drawing.Size(1000, 150);
            this.door_check_label.TabIndex = 85;
            this.door_check_label.Text = "！KSF調合中！\r\n！グローブ着用→ドア閉！";
            this.door_check_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.door_check_label.Visible = false;
            // 
            // door_check_error_label
            // 
            this.door_check_error_label.AutoSize = true;
            this.door_check_error_label.BackColor = System.Drawing.Color.Yellow;
            this.door_check_error_label.Font = new System.Drawing.Font("HGP創英角ｺﾞｼｯｸUB", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.door_check_error_label.ForeColor = System.Drawing.Color.Red;
            this.door_check_error_label.Location = new System.Drawing.Point(1828, 562);
            this.door_check_error_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.door_check_error_label.Name = "door_check_error_label";
            this.door_check_error_label.Size = new System.Drawing.Size(254, 24);
            this.door_check_error_label.TabIndex = 86;
            this.door_check_error_label.Text = "door check error msg";
            this.door_check_error_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.door_check_error_label.Visible = false;
            // 
            // end_button
            // 
            this.end_button.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.end_button.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.end_button.Location = new System.Drawing.Point(1507, 939);
            this.end_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.end_button.Name = "end_button";
            this.end_button.Size = new System.Drawing.Size(133, 74);
            this.end_button.TabIndex = 87;
            this.end_button.Text = "Exit";
            this.end_button.UseVisualStyleBackColor = true;
            this.end_button.MouseDown += new System.Windows.Forms.MouseEventHandler(this.end_button_MouseDown);
            // 
            // hand_mix_time_label
            // 
            this.hand_mix_time_label.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(192)))), ((int)(((byte)(255)))));
            this.hand_mix_time_label.Font = new System.Drawing.Font("メイリオ", 21.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.hand_mix_time_label.ForeColor = System.Drawing.Color.Black;
            this.hand_mix_time_label.Location = new System.Drawing.Point(1768, 595);
            this.hand_mix_time_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.hand_mix_time_label.Name = "hand_mix_time_label";
            this.hand_mix_time_label.Size = new System.Drawing.Size(435, 141);
            this.hand_mix_time_label.TabIndex = 88;
            this.hand_mix_time_label.Text = "手攪拌して下さい";
            this.hand_mix_time_label.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.hand_mix_time_label.Visible = false;
            // 
            // status_label
            // 
            this.status_label.AutoSize = true;
            this.status_label.BackColor = System.Drawing.Color.LightGreen;
            this.status_label.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.status_label.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.status_label.Font = new System.Drawing.Font("メイリオ", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.status_label.Location = new System.Drawing.Point(980, 495);
            this.status_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.status_label.Name = "status_label";
            this.status_label.Size = new System.Drawing.Size(662, 146);
            this.status_label.TabIndex = 89;
            this.status_label.Text = "status_label\r\nあいうえおかきくけこさしすせそたちつてと\r\nあいうえおかきくけこさしすせそたちつてと";
            // 
            // status_label_timer
            // 
            this.status_label_timer.Enabled = true;
            this.status_label_timer.Interval = 1000;
            this.status_label_timer.Tick += new System.EventHandler(this.status_label_timer_Tick);
            // 
            // fusoku_label
            // 
            this.fusoku_label.AutoSize = true;
            this.fusoku_label.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fusoku_label.Location = new System.Drawing.Point(1652, 837);
            this.fusoku_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.fusoku_label.Name = "fusoku_label";
            this.fusoku_label.Size = new System.Drawing.Size(265, 61);
            this.fusoku_label.TabIndex = 90;
            this.fusoku_label.Text = "風速計(m/s)";
            this.fusoku_label.Visible = false;
            // 
            // fusoku_value_textBox
            // 
            this.fusoku_value_textBox.Font = new System.Drawing.Font("メイリオ", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fusoku_value_textBox.Location = new System.Drawing.Point(1663, 909);
            this.fusoku_value_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.fusoku_value_textBox.Name = "fusoku_value_textBox";
            this.fusoku_value_textBox.ReadOnly = true;
            this.fusoku_value_textBox.Size = new System.Drawing.Size(252, 68);
            this.fusoku_value_textBox.TabIndex = 91;
            this.fusoku_value_textBox.Visible = false;
            // 
            // kanomax_realtime_value
            // 
            this.kanomax_realtime_value.Interval = 500;
            this.kanomax_realtime_value.Tick += new System.EventHandler(this.kanomax_realtime_value_Tick);
            // 
            // kanomax_fusoku_serialPort
            // 
            this.kanomax_fusoku_serialPort.BaudRate = 115200;
            this.kanomax_fusoku_serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.kanomax_fusoku_serialPort_DataReceived);
            // 
            // kanomaxNG_label
            // 
            this.kanomaxNG_label.AutoSize = true;
            this.kanomaxNG_label.BackColor = System.Drawing.Color.Red;
            this.kanomaxNG_label.Font = new System.Drawing.Font("Meiryo UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.kanomaxNG_label.ForeColor = System.Drawing.Color.Black;
            this.kanomaxNG_label.Location = new System.Drawing.Point(1668, 981);
            this.kanomaxNG_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.kanomaxNG_label.Name = "kanomaxNG_label";
            this.kanomaxNG_label.Size = new System.Drawing.Size(242, 61);
            this.kanomaxNG_label.TabIndex = 92;
            this.kanomaxNG_label.Text = "風速計NG";
            this.kanomaxNG_label.Visible = false;
            // 
            // WeighForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.ClientSize = new System.Drawing.Size(1924, 1050);
            this.Controls.Add(this.kanomaxNG_label);
            this.Controls.Add(this.fusoku_value_textBox);
            this.Controls.Add(this.fusoku_label);
            this.Controls.Add(this.door_check_error_label);
            this.Controls.Add(this.door_check_label);
            this.Controls.Add(this.hand_mix_time_label);
            this.Controls.Add(this.status_label);
            this.Controls.Add(this.end_button);
            this.Controls.Add(this.recipe_info_label);
            this.Controls.Add(this.use_buzai_dataGridView);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.label14);
            this.Controls.Add(this.gross_weight_label);
            this.Controls.Add(this.cup_weight_label);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label13);
            this.Controls.Add(this.next_button);
            this.Controls.Add(this.weigh_panel);
            this.Controls.Add(this.use_buzai_groupBox);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "WeighForm";
            this.Text = "秤量";
            this.Load += new System.EventHandler(this.WeighForm_Load);
            this.use_buzai_groupBox.ResumeLayout(false);
            this.use_buzai_groupBox.PerformLayout();
            this.weigh_panel.ResumeLayout(false);
            this.weigh_panel.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.use_buzai_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox use_buzai_groupBox;
        private System.Windows.Forms.TextBox buzai_fcode_textBox;
        private System.Windows.Forms.TextBox buzai_lotno_textBox;
        private System.Windows.Forms.TextBox buzai_name_textBox;
        private System.Windows.Forms.TextBox input_buzaiQR_textBox;
        private System.Windows.Forms.TextBox result_value_textBox;
        private System.Windows.Forms.TextBox target_value_textBox;
        private System.Windows.Forms.Panel weigh_panel;
        private System.Windows.Forms.Label stable_mark_label;
        private System.Windows.Forms.Label within_label;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button next_button;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.Label gross_weight_label;
        private System.Windows.Forms.Label cup_weight_label;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataGridView use_buzai_dataGridView;
        private System.Windows.Forms.Label recipe_info_label;
        private System.IO.Ports.SerialPort electronicBalance_serialPort;
        private System.Windows.Forms.Timer door_check_timer;
        private System.Windows.Forms.Label door_check_label;
        private System.Windows.Forms.Label door_check_error_label;
        private System.Windows.Forms.Button end_button;
        private System.Windows.Forms.Label hand_mix_time_label;
        private System.Windows.Forms.Label status_label;
        private System.Windows.Forms.DataGridViewTextBoxColumn ok;
        private System.Windows.Forms.DataGridViewTextBoxColumn name;
        private System.Windows.Forms.DataGridViewTextBoxColumn fcode;
        private System.Windows.Forms.DataGridViewTextBoxColumn lotno;
        private System.Windows.Forms.DataGridViewTextBoxColumn target;
        private System.Windows.Forms.DataGridViewTextBoxColumn allowableErrorGram;
        private System.Windows.Forms.DataGridViewTextBoxColumn result;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.Label stable_time_label;
        private System.Windows.Forms.Label minus_allow_label;
        private System.Windows.Forms.Label plus_allow_label;
        private System.Windows.Forms.Timer status_label_timer;
        private System.Windows.Forms.Label fusoku_label;
        private System.Windows.Forms.TextBox fusoku_value_textBox;
        private System.Windows.Forms.Timer kanomax_realtime_value;
        private System.IO.Ports.SerialPort kanomax_fusoku_serialPort;
        private System.Windows.Forms.Label kanomaxNG_label;
    }
}