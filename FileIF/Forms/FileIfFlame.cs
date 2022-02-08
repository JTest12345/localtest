using Oskas;
using System;
using System.Collections.Generic;
using System.IO;
using System.Windows.Forms;
using uPLibrary.Networking.M2Mqtt;

namespace FileIf

{
    public class FileIfFlame : Oskas.fmMain
    {
        FileFetchForm fileFetchForm;

        Tasks_Common tcommons = new Tasks_Common();
        //MySQL db = new MySQL();

        // mqttインスタンス
        MqttClient mqttClient;
        OskasMqttClient mq = new OskasMqttClient();

        Magcupini mci = new Magcupini();

        // FILEIF稼働許可
        bool serverOn = false;

        // 参照渡し用メッセージ変数
        string globalmsg;

        // タイムアウト監視用カウンタ
        int TofCnt = 1;
        private MenuStrip menuStrip;
        private ToolStripMenuItem Mn_Files;
        private ToolStripMenuItem Mn_Files_OpenFif;

        // TaskHander
        TaskControlHandler tskhdl;
        private ToolStripMenuItem オプションToolStripMenuItem;
        private ToolStripMenuItem Mn_OpenFileFetch;

        // インターロック設備リスト
        List<string> IntLokMac = new List<string>();

        public FileIfFlame()
        {
            InitializeComponent();
            TaskTimer.Enabled = false;
            toolStripStatusLabel1.Text = string.Format("FILEIFは停止中です");
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_red;

            // NLogのパス設定
            OskNLog.setFolderName("FILEIF");

            // mugcup.iniを読み込み
            if (!mci.GetMugCupIniValues(ref globalmsg))
            {
                btServerStart.Enabled = false;
                OskNLog.Log(globalmsg, Cnslcnf.msg_error);
            }

            // FileFetchコンストラクタ
            fileFetchForm = new FileFetchForm(mci);


            // MySQLを使用しない仕様となった為コメントアウト
            // 2021.12.27
            //if (!db.PingUpdatesState(mci.ConnectionStrings, ref globalmsg))
            //{
            //    btServerStart.Enabled = false;
            //    OskNLog.Log(globalmsg, Cnslcnf.msg_error);
            //}
            //else
            //    OskNLog.Log(globalmsg, Cnslcnf.msg_info);

            // Mqttを使う？
            if (mci.isUseMqtt)
            {
                mqttClient = new MqttClient(mci.mosquittoHost);
                var ret = mqttClient.Connect(Guid.NewGuid().ToString());
            }

            // FILEIF起動メッセージ
            if (btServerStart.Enabled)
                OskNLog.Log("File InterFace(aka:MagCup)が起動しました", 0);

        }

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.Mn_Files = new System.Windows.Forms.ToolStripMenuItem();
            this.Mn_Files_OpenFif = new System.Windows.Forms.ToolStripMenuItem();
            this.オプションToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.Mn_OpenFileFetch = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // TaskTimer
            // 
            this.TaskTimer.Tick += new System.EventHandler(this.TaskTimer_Tick);
            // 
            // MqttTimer
            // 
            this.MqttTimer.Enabled = true;
            this.MqttTimer.Interval = 10000;
            this.MqttTimer.Tick += new System.EventHandler(this.MqttTimer_Tick);
            // 
            // consoleBox
            // 
            this.consoleBox.Location = new System.Drawing.Point(12, 25);
            // 
            // btServerStart
            // 
            this.btServerStart.Location = new System.Drawing.Point(645, 534);
            this.btServerStart.Size = new System.Drawing.Size(127, 32);
            this.btServerStart.Text = "Start FILEIF";
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mn_Files,
            this.オプションToolStripMenuItem});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(784, 24);
            this.menuStrip.TabIndex = 8;
            this.menuStrip.Text = "menuStrip1";
            // 
            // Mn_Files
            // 
            this.Mn_Files.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mn_Files_OpenFif});
            this.Mn_Files.Name = "Mn_Files";
            this.Mn_Files.Size = new System.Drawing.Size(53, 20);
            this.Mn_Files.Text = "ファイル";
            // 
            // Mn_Files_OpenFif
            // 
            this.Mn_Files_OpenFif.Name = "Mn_Files_OpenFif";
            this.Mn_Files_OpenFif.Size = new System.Drawing.Size(152, 22);
            this.Mn_Files_OpenFif.Text = "FIFフォルダを開く";
            // 
            // オプションToolStripMenuItem
            // 
            this.オプションToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mn_OpenFileFetch});
            this.オプションToolStripMenuItem.Name = "オプションToolStripMenuItem";
            this.オプションToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.オプションToolStripMenuItem.Text = "オプション";
            // 
            // Mn_OpenFileFetch
            // 
            this.Mn_OpenFileFetch.Name = "Mn_OpenFileFetch";
            this.Mn_OpenFileFetch.Size = new System.Drawing.Size(168, 22);
            this.Mn_OpenFileFetch.Text = "設備内ファイル取得";
            this.Mn_OpenFileFetch.Click += new System.EventHandler(this.OpenFileFetchForm);
            // 
            // FileIfFlame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FileIfFlame";
            this.Text = "FIF";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MagCupFlame_FormClosing);
            this.Controls.SetChildIndex(this.ErrorLogComsole, 0);
            this.Controls.SetChildIndex(this.consoleBox, 0);
            this.Controls.SetChildIndex(this.btServerStart, 0);
            this.Controls.SetChildIndex(this.menuStrip, 0);
            this.menuStrip.ResumeLayout(false);
            this.menuStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        private void btServerStart_Click(object sender, EventArgs e)
        {
            if (!serverOn)
            {
                // mugcup.iniを読み込み
                if (!mci.GetMugCupIniValues(ref globalmsg))
                {
                    btServerStart.Enabled = false;
                    OskNLog.Log(globalmsg, Cnslcnf.msg_error);
                }
                else
                {
                    mci.SrvStatDT = DateTime.Now;
                    mci.MsglogPath = mci.MsglogDir + @"\" + mci.SrvStatDT.ToString("yyyyMMddHHmmss") + ".txt";
                    if (MakeMsgLog(mci.MsglogPath))
                    {
                        if (false) //(!db.PingUpdatesState(mci.ConnectionStrings, ref globalmsg))
                        {
                            btServerStart.Enabled = false;
                            OskNLog.Log(globalmsg, Cnslcnf.msg_error);
                        }
                        else
                        {
                            serverOn = true;
                            TaskTimer.Enabled = true;
                            btServerStart.Text = "Stop FILEIF";
                            if (mci.DebugMode)
                            {
                                OskNLog.Log("デバックモードでFILEIFを開始しました", Cnslcnf.msg_info);
                            }
                            else
                            {
                                OskNLog.Log("FILEIFを開始しました", Cnslcnf.msg_info);
                            }

                            toolStripStatusLabel1.Text = string.Format("FILEIFは稼働中です");
                            toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_blue;
                            toolStripStatusLabel2.Text = "";
                            toolStripStatusLabel2.Image = null;
                        }
                    }
                    else
                    {
                        MessageBox.Show("ログの書き出しができないためFILEIF稼働開始できません");
                    }
                }
            }
            else
            {
                serverOn = false;
                TaskTimer.Enabled = false;
                btServerStart.Text = "Start FILEIF";
                OskNLog.Log("FILEIFを停止しました", Cnslcnf.msg_info);
                toolStripStatusLabel1.Text = string.Format("FILEIFは停止中です");
                toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_red;
                toolStripStatusLabel2.Text = "";
                toolStripStatusLabel2.Image = null;
            }
        }


        void DirSearch(string sDir, string key, ref List<String> Files)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, "*" + key))
                    {

                        Files.Add(f);

                    }
                    DirSearch(d, key, ref Files);
                }
            }
            catch (System.Exception ex)
            {
                OskNLog.Log("ファイル取得時に問題が発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
            }
        }


        private void TaskTimer_Tick(object sender, EventArgs e)
        {
            if (serverOn)
            {
                TaskTimer.Enabled = false;

                // TaskHandler初期化
                tskhdl = new TaskControlHandler(tcommons, mci, IntLokMac);

                // INファイルの検索
                tskhdl.Tasks("IN", mci.infilekey);

                if (TofCnt == 5)
                {
                    // WIPファイルの検索（タイムアウト監視）
                    tskhdl.Tasks("WIP", mci.wipfilekey);

                    // ENDファイルの検索（タイムアウト監視）
                    tskhdl.Tasks("END", mci.endfilekey);

                    TofCnt = 1;
                }

                TofCnt++;

                TaskTimer.Enabled = true;
            }
        }



        //
        /* メニューからmagcup.iniを開く
        */
        //
        private void MagcupiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\magcup\magcup.ini");
        }


        //mqtt code
        //
        private void MqttTimer_Tick(object sender, EventArgs e)
        {
            //FILEIFステータスのパブリッシュ
            if (mci.isUseMqtt)
            {
                mq.Mqtt_ServerStatus(mqttClient, "magcup", serverOn);
            }
        }



        private void MagCupFlame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (serverOn)
            {
                MessageBox.Show("FILEIFを停止してからクローズしてください");
                e.Cancel = true;
                return;
            }

            if (mci.isUseMqtt && mqttClient.IsConnected)
            {
                mqttClient.Disconnect();
            }
        }

        private void OpenFileFetchForm(object sender, EventArgs e)
        {
            if (!fileFetchForm.Visible)
            {
                if (fileFetchForm.IsDisposed)
                {
                    fileFetchForm = new FileFetchForm(mci);
                }
                fileFetchForm.Show(this);
            }
            else
            {
                ConsoleShow("既に表示されています", Cnslcnf.msg_error);
            }
        }
    }

}
