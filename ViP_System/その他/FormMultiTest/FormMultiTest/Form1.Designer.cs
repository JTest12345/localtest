
namespace FormMultiTest
{
    partial class Form1
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
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea2 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series2 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.tabControl1 = new System.Windows.Forms.TabControl();
            this.test1 = new System.Windows.Forms.TabPage();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.test2 = new System.Windows.Forms.TabPage();
            this.get_usbdevice = new System.Windows.Forms.Button();
            this.device_list = new System.Windows.Forms.TextBox();
            this.tabControl1.SuspendLayout();
            this.test1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.SuspendLayout();
            // 
            // tabControl1
            // 
            this.tabControl1.Controls.Add(this.test1);
            this.tabControl1.Controls.Add(this.test2);
            this.tabControl1.Location = new System.Drawing.Point(444, 220);
            this.tabControl1.Name = "tabControl1";
            this.tabControl1.SelectedIndex = 0;
            this.tabControl1.Size = new System.Drawing.Size(329, 195);
            this.tabControl1.TabIndex = 0;
            this.tabControl1.Visible = false;
            // 
            // test1
            // 
            this.test1.Controls.Add(this.chart1);
            this.test1.Location = new System.Drawing.Point(4, 22);
            this.test1.Name = "test1";
            this.test1.Padding = new System.Windows.Forms.Padding(3);
            this.test1.Size = new System.Drawing.Size(321, 169);
            this.test1.TabIndex = 0;
            this.test1.Text = "test1";
            this.test1.UseVisualStyleBackColor = true;
            // 
            // chart1
            // 
            chartArea2.Name = "ChartArea1";
            this.chart1.ChartAreas.Add(chartArea2);
            legend2.Name = "Legend1";
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(72, 26);
            this.chart1.Name = "chart1";
            series2.ChartArea = "ChartArea1";
            series2.Legend = "Legend1";
            series2.Name = "Series1";
            this.chart1.Series.Add(series2);
            this.chart1.Size = new System.Drawing.Size(199, 111);
            this.chart1.TabIndex = 0;
            this.chart1.Text = "chart1";
            // 
            // test2
            // 
            this.test2.Location = new System.Drawing.Point(4, 22);
            this.test2.Name = "test2";
            this.test2.Padding = new System.Windows.Forms.Padding(3);
            this.test2.Size = new System.Drawing.Size(321, 169);
            this.test2.TabIndex = 1;
            this.test2.Text = "test2";
            this.test2.UseVisualStyleBackColor = true;
            // 
            // get_usbdevice
            // 
            this.get_usbdevice.Location = new System.Drawing.Point(47, 41);
            this.get_usbdevice.Name = "get_usbdevice";
            this.get_usbdevice.Size = new System.Drawing.Size(104, 36);
            this.get_usbdevice.TabIndex = 1;
            this.get_usbdevice.Text = "USBデバイス取得";
            this.get_usbdevice.UseVisualStyleBackColor = true;
            this.get_usbdevice.Click += new System.EventHandler(this.get_usbdevice_Click);
            // 
            // device_list
            // 
            this.device_list.Location = new System.Drawing.Point(47, 83);
            this.device_list.Multiline = true;
            this.device_list.Name = "device_list";
            this.device_list.ScrollBars = System.Windows.Forms.ScrollBars.Both;
            this.device_list.Size = new System.Drawing.Size(377, 282);
            this.device_list.TabIndex = 2;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.device_list);
            this.Controls.Add(this.get_usbdevice);
            this.Controls.Add(this.tabControl1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.tabControl1.ResumeLayout(false);
            this.test1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl tabControl1;
        private System.Windows.Forms.TabPage test1;
        private System.Windows.Forms.TabPage test2;
        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
        private System.Windows.Forms.Button get_usbdevice;
        private System.Windows.Forms.TextBox device_list;
    }
}

