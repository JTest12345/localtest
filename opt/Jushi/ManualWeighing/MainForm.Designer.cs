
namespace ManualWeighing {
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle7 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle8 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle9 = new System.Windows.Forms.DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle10 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle11 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle12 = new System.Windows.Forms.DataGridViewCellStyle();
            this.recipe_list_dataGridView = new System.Windows.Forms.DataGridView();
            this.cupno = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.filepath = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.input_cupno_textBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.update_recipeliist_button = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.print_button = new System.Windows.Forms.Button();
            this.start_weigh_button = new System.Windows.Forms.Button();
            this.label4 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.initialize_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setting_ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.ksf_ng_label = new System.Windows.Forms.Label();
            this.revival_button = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.filtration_weigh_start_button = new System.Windows.Forms.Button();
            this.label5 = new System.Windows.Forms.Label();
            this.recipe_list_tabControl = new System.Windows.Forms.TabControl();
            this.recipe_tabPage = new System.Windows.Forms.TabPage();
            this.label10 = new System.Windows.Forms.Label();
            this.start_manual_weigh_button = new System.Windows.Forms.Button();
            this.label7 = new System.Windows.Forms.Label();
            this.normal_recipe_num_label = new System.Windows.Forms.Label();
            this.filt_recipe_tabPage = new System.Windows.Forms.TabPage();
            this.label8 = new System.Windows.Forms.Label();
            this.filt_recipe_num_label = new System.Windows.Forms.Label();
            this.filtration_recipe_list_dataGridView = new System.Windows.Forms.DataGridView();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.label9 = new System.Windows.Forms.Label();
            this.latest_version_label = new System.Windows.Forms.Label();
            this.fusoku_value_timer = new System.Windows.Forms.Timer(this.components);
            this.fusoku_realtime_value = new System.Windows.Forms.TextBox();
            this.fusoku_realtime_label = new System.Windows.Forms.Label();
            this.fusokuNG_label = new System.Windows.Forms.Label();
            this.kanomax_serialPort = new System.IO.Ports.SerialPort(this.components);
            ((System.ComponentModel.ISupportInitialize)(this.recipe_list_dataGridView)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.recipe_list_tabControl.SuspendLayout();
            this.recipe_tabPage.SuspendLayout();
            this.filt_recipe_tabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filtration_recipe_list_dataGridView)).BeginInit();
            this.SuspendLayout();
            // 
            // recipe_list_dataGridView
            // 
            this.recipe_list_dataGridView.AllowUserToAddRows = false;
            this.recipe_list_dataGridView.AllowUserToDeleteRows = false;
            this.recipe_list_dataGridView.AllowUserToResizeColumns = false;
            this.recipe_list_dataGridView.AllowUserToResizeRows = false;
            this.recipe_list_dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle7.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle7.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle7.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle7.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle7.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle7.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle7.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.recipe_list_dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle7;
            this.recipe_list_dataGridView.ColumnHeadersHeight = 29;
            this.recipe_list_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.recipe_list_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.cupno,
            this.filepath});
            dataGridViewCellStyle8.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle8.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle8.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle8.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle8.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle8.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle8.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.recipe_list_dataGridView.DefaultCellStyle = dataGridViewCellStyle8;
            this.recipe_list_dataGridView.Location = new System.Drawing.Point(165, 4);
            this.recipe_list_dataGridView.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.recipe_list_dataGridView.Name = "recipe_list_dataGridView";
            this.recipe_list_dataGridView.ReadOnly = true;
            dataGridViewCellStyle9.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle9.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle9.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle9.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle9.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle9.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle9.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.recipe_list_dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle9;
            this.recipe_list_dataGridView.RowHeadersWidth = 51;
            this.recipe_list_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.recipe_list_dataGridView.RowTemplate.Height = 21;
            this.recipe_list_dataGridView.Size = new System.Drawing.Size(1517, 544);
            this.recipe_list_dataGridView.TabIndex = 48;
            // 
            // cupno
            // 
            this.cupno.HeaderText = "カップ番号";
            this.cupno.MinimumWidth = 6;
            this.cupno.Name = "cupno";
            this.cupno.ReadOnly = true;
            this.cupno.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.cupno.Width = 60;
            // 
            // filepath
            // 
            this.filepath.HeaderText = "ファイルパス";
            this.filepath.MinimumWidth = 6;
            this.filepath.Name = "filepath";
            this.filepath.ReadOnly = true;
            this.filepath.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.filepath.Width = 64;
            // 
            // input_cupno_textBox
            // 
            this.input_cupno_textBox.Font = new System.Drawing.Font("Meiryo UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.input_cupno_textBox.ImeMode = System.Windows.Forms.ImeMode.Disable;
            this.input_cupno_textBox.Location = new System.Drawing.Point(5, 105);
            this.input_cupno_textBox.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.input_cupno_textBox.Name = "input_cupno_textBox";
            this.input_cupno_textBox.ReadOnly = true;
            this.input_cupno_textBox.Size = new System.Drawing.Size(281, 48);
            this.input_cupno_textBox.TabIndex = 49;
            this.input_cupno_textBox.TextChanged += new System.EventHandler(this.input_cupno_textBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("MS UI Gothic", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label1.Location = new System.Drawing.Point(3, 70);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(209, 33);
            this.label1.TabIndex = 50;
            this.label1.Text = "カップ番号入力";
            // 
            // update_recipeliist_button
            // 
            this.update_recipeliist_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("update_recipeliist_button.BackgroundImage")));
            this.update_recipeliist_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.update_recipeliist_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.update_recipeliist_button.Location = new System.Drawing.Point(342, 72);
            this.update_recipeliist_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.update_recipeliist_button.Name = "update_recipeliist_button";
            this.update_recipeliist_button.Size = new System.Drawing.Size(117, 105);
            this.update_recipeliist_button.TabIndex = 51;
            this.update_recipeliist_button.UseVisualStyleBackColor = true;
            this.update_recipeliist_button.Click += new System.EventHandler(this.update_recipeliist_button_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label2.Location = new System.Drawing.Point(325, 180);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(140, 58);
            this.label2.TabIndex = 52;
            this.label2.Text = "レシピリスト\r\n更新";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label3.Location = new System.Drawing.Point(510, 180);
            this.label3.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(100, 58);
            this.label3.TabIndex = 53;
            this.label3.Text = "ラベル\r\n再印刷";
            this.label3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // print_button
            // 
            this.print_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("print_button.BackgroundImage")));
            this.print_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.print_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.print_button.Location = new System.Drawing.Point(508, 72);
            this.print_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.print_button.Name = "print_button";
            this.print_button.Size = new System.Drawing.Size(117, 105);
            this.print_button.TabIndex = 54;
            this.print_button.UseVisualStyleBackColor = true;
            this.print_button.Click += new System.EventHandler(this.print_button_Click);
            // 
            // start_weigh_button
            // 
            this.start_weigh_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("start_weigh_button.BackgroundImage")));
            this.start_weigh_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.start_weigh_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.start_weigh_button.Location = new System.Drawing.Point(25, 15);
            this.start_weigh_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.start_weigh_button.Name = "start_weigh_button";
            this.start_weigh_button.Size = new System.Drawing.Size(117, 105);
            this.start_weigh_button.TabIndex = 55;
            this.start_weigh_button.UseVisualStyleBackColor = true;
            this.start_weigh_button.Visible = false;
            this.start_weigh_button.Click += new System.EventHandler(this.start_weigh_button_Click);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label4.Location = new System.Drawing.Point(13, 128);
            this.label4.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(129, 29);
            this.label4.TabIndex = 56;
            this.label4.Text = "秤量開始";
            this.label4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // menuStrip1
            // 
            this.menuStrip1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.menuStrip1.GripMargin = new System.Windows.Forms.Padding(2, 2, 0, 2);
            this.menuStrip1.ImageScalingSize = new System.Drawing.Size(20, 20);
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initialize_ToolStripMenuItem,
            this.setting_ToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 3, 0, 3);
            this.menuStrip1.Size = new System.Drawing.Size(2573, 54);
            this.menuStrip1.TabIndex = 58;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // initialize_ToolStripMenuItem
            // 
            this.initialize_ToolStripMenuItem.Name = "initialize_ToolStripMenuItem";
            this.initialize_ToolStripMenuItem.Size = new System.Drawing.Size(82, 29);
            this.initialize_ToolStripMenuItem.Text = "初期化";
            this.initialize_ToolStripMenuItem.Click += new System.EventHandler(this.initialize_ToolStripMenuItem_Click);
            // 
            // setting_ToolStripMenuItem
            // 
            this.setting_ToolStripMenuItem.Name = "setting_ToolStripMenuItem";
            this.setting_ToolStripMenuItem.Size = new System.Drawing.Size(64, 29);
            this.setting_ToolStripMenuItem.Text = "設定";
            this.setting_ToolStripMenuItem.Click += new System.EventHandler(this.setting_ToolStripMenuItem_Click);
            // 
            // ksf_ng_label
            // 
            this.ksf_ng_label.AutoSize = true;
            this.ksf_ng_label.BackColor = System.Drawing.Color.Yellow;
            this.ksf_ng_label.Font = new System.Drawing.Font("Meiryo UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.ksf_ng_label.ForeColor = System.Drawing.Color.Red;
            this.ksf_ng_label.Location = new System.Drawing.Point(8, 165);
            this.ksf_ng_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.ksf_ng_label.Name = "ksf_ng_label";
            this.ksf_ng_label.Size = new System.Drawing.Size(251, 61);
            this.ksf_ng_label.TabIndex = 59;
            this.ksf_ng_label.Text = "KSF NG！";
            this.ksf_ng_label.Visible = false;
            // 
            // revival_button
            // 
            this.revival_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("revival_button.BackgroundImage")));
            this.revival_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.revival_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.revival_button.Location = new System.Drawing.Point(667, 72);
            this.revival_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.revival_button.Name = "revival_button";
            this.revival_button.Size = new System.Drawing.Size(117, 105);
            this.revival_button.TabIndex = 60;
            this.revival_button.UseVisualStyleBackColor = true;
            this.revival_button.Visible = false;
            this.revival_button.Click += new System.EventHandler(this.revival_button_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label6.Location = new System.Drawing.Point(652, 180);
            this.label6.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(138, 29);
            this.label6.TabIndex = 61;
            this.label6.Text = "レシピ復活";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.label6.Visible = false;
            // 
            // filtration_weigh_start_button
            // 
            this.filtration_weigh_start_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("filtration_weigh_start_button.BackgroundImage")));
            this.filtration_weigh_start_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.filtration_weigh_start_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.filtration_weigh_start_button.Location = new System.Drawing.Point(25, 15);
            this.filtration_weigh_start_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.filtration_weigh_start_button.Name = "filtration_weigh_start_button";
            this.filtration_weigh_start_button.Size = new System.Drawing.Size(117, 105);
            this.filtration_weigh_start_button.TabIndex = 62;
            this.filtration_weigh_start_button.UseVisualStyleBackColor = true;
            this.filtration_weigh_start_button.Visible = false;
            this.filtration_weigh_start_button.Click += new System.EventHandler(this.filtration_weigh_start_button_Click);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label5.Location = new System.Drawing.Point(12, 128);
            this.label5.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(129, 58);
            this.label5.TabIndex = 63;
            this.label5.Text = "ろ過後\r\n秤量開始";
            this.label5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // recipe_list_tabControl
            // 
            this.recipe_list_tabControl.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.recipe_list_tabControl.Controls.Add(this.recipe_tabPage);
            this.recipe_list_tabControl.Controls.Add(this.filt_recipe_tabPage);
            this.recipe_list_tabControl.Location = new System.Drawing.Point(8, 240);
            this.recipe_list_tabControl.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.recipe_list_tabControl.Name = "recipe_list_tabControl";
            this.recipe_list_tabControl.SelectedIndex = 0;
            this.recipe_list_tabControl.Size = new System.Drawing.Size(1700, 592);
            this.recipe_list_tabControl.TabIndex = 64;
            // 
            // recipe_tabPage
            // 
            this.recipe_tabPage.BackColor = System.Drawing.SystemColors.Control;
            this.recipe_tabPage.Controls.Add(this.label10);
            this.recipe_tabPage.Controls.Add(this.start_manual_weigh_button);
            this.recipe_tabPage.Controls.Add(this.label7);
            this.recipe_tabPage.Controls.Add(this.normal_recipe_num_label);
            this.recipe_tabPage.Controls.Add(this.recipe_list_dataGridView);
            this.recipe_tabPage.Controls.Add(this.start_weigh_button);
            this.recipe_tabPage.Controls.Add(this.label4);
            this.recipe_tabPage.Location = new System.Drawing.Point(4, 28);
            this.recipe_tabPage.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.recipe_tabPage.Name = "recipe_tabPage";
            this.recipe_tabPage.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.recipe_tabPage.Size = new System.Drawing.Size(1692, 560);
            this.recipe_tabPage.TabIndex = 0;
            this.recipe_tabPage.Text = "レシピリスト";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label10.Location = new System.Drawing.Point(13, 442);
            this.label10.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(129, 58);
            this.label10.TabIndex = 60;
            this.label10.Text = "MANUAL\r\n秤量開始";
            this.label10.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // start_manual_weigh_button
            // 
            this.start_manual_weigh_button.BackgroundImage = ((System.Drawing.Image)(resources.GetObject("start_manual_weigh_button.BackgroundImage")));
            this.start_manual_weigh_button.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.start_manual_weigh_button.Cursor = System.Windows.Forms.Cursors.Hand;
            this.start_manual_weigh_button.Location = new System.Drawing.Point(25, 330);
            this.start_manual_weigh_button.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.start_manual_weigh_button.Name = "start_manual_weigh_button";
            this.start_manual_weigh_button.Size = new System.Drawing.Size(117, 105);
            this.start_manual_weigh_button.TabIndex = 56;
            this.start_manual_weigh_button.Tag = "MANUAL";
            this.start_manual_weigh_button.UseVisualStyleBackColor = true;
            this.start_manual_weigh_button.Visible = false;
            this.start_manual_weigh_button.Click += new System.EventHandler(this.start_weigh_button_Click);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label7.Location = new System.Drawing.Point(25, 195);
            this.label7.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(109, 58);
            this.label7.TabIndex = 58;
            this.label7.Text = "未作成\r\nレシピ数";
            // 
            // normal_recipe_num_label
            // 
            this.normal_recipe_num_label.AutoSize = true;
            this.normal_recipe_num_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.normal_recipe_num_label.Location = new System.Drawing.Point(50, 262);
            this.normal_recipe_num_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.normal_recipe_num_label.Name = "normal_recipe_num_label";
            this.normal_recipe_num_label.Size = new System.Drawing.Size(58, 29);
            this.normal_recipe_num_label.TabIndex = 57;
            this.normal_recipe_num_label.Text = "---";
            // 
            // filt_recipe_tabPage
            // 
            this.filt_recipe_tabPage.BackColor = System.Drawing.SystemColors.Control;
            this.filt_recipe_tabPage.Controls.Add(this.label8);
            this.filt_recipe_tabPage.Controls.Add(this.filt_recipe_num_label);
            this.filt_recipe_tabPage.Controls.Add(this.filtration_recipe_list_dataGridView);
            this.filt_recipe_tabPage.Controls.Add(this.label5);
            this.filt_recipe_tabPage.Controls.Add(this.filtration_weigh_start_button);
            this.filt_recipe_tabPage.Location = new System.Drawing.Point(4, 28);
            this.filt_recipe_tabPage.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.filt_recipe_tabPage.Name = "filt_recipe_tabPage";
            this.filt_recipe_tabPage.Padding = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.filt_recipe_tabPage.Size = new System.Drawing.Size(1692, 560);
            this.filt_recipe_tabPage.TabIndex = 1;
            this.filt_recipe_tabPage.Text = "ろ過後レシピリスト";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label8.Location = new System.Drawing.Point(25, 210);
            this.label8.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(109, 58);
            this.label8.TabIndex = 65;
            this.label8.Text = "未作成\r\nレシピ数";
            // 
            // filt_recipe_num_label
            // 
            this.filt_recipe_num_label.AutoSize = true;
            this.filt_recipe_num_label.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.filt_recipe_num_label.Location = new System.Drawing.Point(50, 278);
            this.filt_recipe_num_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.filt_recipe_num_label.Name = "filt_recipe_num_label";
            this.filt_recipe_num_label.Size = new System.Drawing.Size(58, 29);
            this.filt_recipe_num_label.TabIndex = 64;
            this.filt_recipe_num_label.Text = "---";
            // 
            // filtration_recipe_list_dataGridView
            // 
            this.filtration_recipe_list_dataGridView.AllowUserToAddRows = false;
            this.filtration_recipe_list_dataGridView.AllowUserToDeleteRows = false;
            this.filtration_recipe_list_dataGridView.AllowUserToResizeColumns = false;
            this.filtration_recipe_list_dataGridView.AllowUserToResizeRows = false;
            this.filtration_recipe_list_dataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            dataGridViewCellStyle10.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle10.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle10.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle10.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle10.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle10.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle10.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.filtration_recipe_list_dataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle10;
            this.filtration_recipe_list_dataGridView.ColumnHeadersHeight = 29;
            this.filtration_recipe_list_dataGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            this.filtration_recipe_list_dataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewTextBoxColumn1,
            this.dataGridViewTextBoxColumn2});
            dataGridViewCellStyle11.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle11.BackColor = System.Drawing.SystemColors.Window;
            dataGridViewCellStyle11.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle11.ForeColor = System.Drawing.SystemColors.ControlText;
            dataGridViewCellStyle11.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle11.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle11.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.filtration_recipe_list_dataGridView.DefaultCellStyle = dataGridViewCellStyle11;
            this.filtration_recipe_list_dataGridView.Location = new System.Drawing.Point(165, 4);
            this.filtration_recipe_list_dataGridView.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.filtration_recipe_list_dataGridView.Name = "filtration_recipe_list_dataGridView";
            this.filtration_recipe_list_dataGridView.ReadOnly = true;
            dataGridViewCellStyle12.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle12.BackColor = System.Drawing.SystemColors.Control;
            dataGridViewCellStyle12.Font = new System.Drawing.Font("MS UI Gothic", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            dataGridViewCellStyle12.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle12.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle12.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle12.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.filtration_recipe_list_dataGridView.RowHeadersDefaultCellStyle = dataGridViewCellStyle12;
            this.filtration_recipe_list_dataGridView.RowHeadersWidth = 51;
            this.filtration_recipe_list_dataGridView.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.filtration_recipe_list_dataGridView.RowTemplate.Height = 21;
            this.filtration_recipe_list_dataGridView.Size = new System.Drawing.Size(1108, 334);
            this.filtration_recipe_list_dataGridView.TabIndex = 49;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "カップ番号";
            this.dataGridViewTextBoxColumn1.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn1.Width = 60;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.HeaderText = "ファイルパス";
            this.dataGridViewTextBoxColumn2.MinimumWidth = 6;
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.ReadOnly = true;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 64;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("MS UI Gothic", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.label9.ForeColor = System.Drawing.Color.Blue;
            this.label9.Location = new System.Drawing.Point(3, 40);
            this.label9.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(521, 24);
            this.label9.TabIndex = 65;
            this.label9.Text = "※初期化はグローブボックスの扉を閉じた状態で行う";
            // 
            // latest_version_label
            // 
            this.latest_version_label.AutoSize = true;
            this.latest_version_label.BackColor = System.Drawing.Color.PowderBlue;
            this.latest_version_label.Font = new System.Drawing.Font("Meiryo UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.latest_version_label.ForeColor = System.Drawing.Color.Black;
            this.latest_version_label.Location = new System.Drawing.Point(812, 40);
            this.latest_version_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.latest_version_label.Name = "latest_version_label";
            this.latest_version_label.Size = new System.Drawing.Size(355, 61);
            this.latest_version_label.TabIndex = 66;
            this.latest_version_label.Text = "latest_version";
            // 
            // fusoku_value_timer
            // 
            this.fusoku_value_timer.Tick += new System.EventHandler(this.fusoku_value_timer_Tick);
            // 
            // fusoku_realtime_value
            // 
            this.fusoku_realtime_value.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fusoku_realtime_value.Location = new System.Drawing.Point(1440, 194);
            this.fusoku_realtime_value.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.fusoku_realtime_value.Name = "fusoku_realtime_value";
            this.fusoku_realtime_value.Size = new System.Drawing.Size(252, 48);
            this.fusoku_realtime_value.TabIndex = 67;
            this.fusoku_realtime_value.Visible = false;
            // 
            // fusoku_realtime_label
            // 
            this.fusoku_realtime_label.AutoSize = true;
            this.fusoku_realtime_label.Font = new System.Drawing.Font("MS UI Gothic", 20.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fusoku_realtime_label.Location = new System.Drawing.Point(1125, 116);
            this.fusoku_realtime_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.fusoku_realtime_label.Name = "fusoku_realtime_label";
            this.fusoku_realtime_label.Size = new System.Drawing.Size(249, 41);
            this.fusoku_realtime_label.TabIndex = 68;
            this.fusoku_realtime_label.Text = "排気残り時間";
            this.fusoku_realtime_label.Visible = false;
            // 
            // fusokuNG_label
            // 
            this.fusokuNG_label.AutoSize = true;
            this.fusokuNG_label.BackColor = System.Drawing.Color.Red;
            this.fusokuNG_label.Font = new System.Drawing.Font("Meiryo UI", 24F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.fusokuNG_label.ForeColor = System.Drawing.Color.Black;
            this.fusokuNG_label.Location = new System.Drawing.Point(1433, 105);
            this.fusokuNG_label.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.fusokuNG_label.Name = "fusokuNG_label";
            this.fusokuNG_label.Size = new System.Drawing.Size(242, 61);
            this.fusokuNG_label.TabIndex = 69;
            this.fusokuNG_label.Text = "風速計NG";
            this.fusokuNG_label.Visible = false;
            // 
            // kanomax_serialPort
            // 
            this.kanomax_serialPort.BaudRate = 115200;
            this.kanomax_serialPort.DataReceived += new System.IO.Ports.SerialDataReceivedEventHandler(this.kanomax_serialPort_DataReceived);
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 18F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1715, 842);
            this.Controls.Add(this.fusokuNG_label);
            this.Controls.Add(this.fusoku_realtime_label);
            this.Controls.Add(this.fusoku_realtime_value);
            this.Controls.Add(this.latest_version_label);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.recipe_list_tabControl);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.revival_button);
            this.Controls.Add(this.ksf_ng_label);
            this.Controls.Add(this.menuStrip1);
            this.Controls.Add(this.print_button);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.update_recipeliist_button);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.input_cupno_textBox);
            this.Margin = new System.Windows.Forms.Padding(5, 4, 5, 4);
            this.Name = "MainForm";
            this.Text = "電子天秤樹脂秤量(C#)";
            ((System.ComponentModel.ISupportInitialize)(this.recipe_list_dataGridView)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.recipe_list_tabControl.ResumeLayout(false);
            this.recipe_tabPage.ResumeLayout(false);
            this.recipe_tabPage.PerformLayout();
            this.filt_recipe_tabPage.ResumeLayout(false);
            this.filt_recipe_tabPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.filtration_recipe_list_dataGridView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.DataGridView recipe_list_dataGridView;
        private System.Windows.Forms.TextBox input_cupno_textBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button update_recipeliist_button;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Button print_button;
        private System.Windows.Forms.Button start_weigh_button;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem setting_ToolStripMenuItem;
        private System.Windows.Forms.DataGridViewTextBoxColumn cupno;
        private System.Windows.Forms.DataGridViewTextBoxColumn filepath;
        private System.Windows.Forms.ToolStripMenuItem initialize_ToolStripMenuItem;
        private System.Windows.Forms.Label ksf_ng_label;
        private System.Windows.Forms.Button revival_button;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button filtration_weigh_start_button;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TabControl recipe_list_tabControl;
        private System.Windows.Forms.TabPage recipe_tabPage;
        private System.Windows.Forms.TabPage filt_recipe_tabPage;
        private System.Windows.Forms.DataGridView filtration_recipe_list_dataGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.Label normal_recipe_num_label;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label filt_recipe_num_label;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Button start_manual_weigh_button;
        private System.Windows.Forms.Label latest_version_label;
        private System.Windows.Forms.Timer fusoku_value_timer;
        private System.Windows.Forms.TextBox fusoku_realtime_value;
        private System.Windows.Forms.Label fusoku_realtime_label;
        private System.Windows.Forms.Label fusokuNG_label;
        private System.IO.Ports.SerialPort kanomax_serialPort;
    }
}

