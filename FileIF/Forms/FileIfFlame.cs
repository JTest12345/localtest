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
        //FTP Client
        FileFetchForm fileFetchForm;
        //VIPFTP Client
        FmVipFetchMac fmVipFetchMac;
        //FIFJsonBuilder
        FIFJsonBuilder.fm_main jsonbuilder;

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
        int TofCnt1 = 1;
        int TofCnt2 = 1;
        private MenuStrip menuStrip;
        private ToolStripMenuItem Mn_Files;

        // TaskHander
        TaskControlHandler tskhdl;
        private ToolStripMenuItem オプションToolStripMenuItem;
        private ToolStripMenuItem OpenFifJsonBuilder;

        // インターロック設備リスト
        List<string> IntLokMac = new List<string>();
        private ToolStripMenuItem vIP分類設備ファイル取得ToolStripMenuItem;
        private ToolStripMenuItem debugFetchToolStripMenuItem;

        // 改行コード
        string CR = "\r\n";

        public FileIfFlame()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;
            TaskTimer.Enabled = false;
            toolStripStatusLabel1.Text = string.Format("FILEIFは停止中です");
            toolStripStatusLabel2.Text = "";
            toolStripStatusLabel1.Image = Oskas.Properties.Resources.button_red;

            // NLogのパス設定
            OskNLog.setFolderName("FILEIF");

            // mugcup.iniを読み込み
            // if (!mci.GetMugCupIniValues(ref globalmsg))
            if (!mci.GetMugCupYamlValues(ref globalmsg))
            {
                btServerStart.Enabled = false;
                OskNLog.Log(globalmsg, Cnslcnf.msg_error);
            }

            // FileFetchコンストラクタ
            fileFetchForm = new FileFetchForm(mci);
            // FmVipFetchMacコンストラクタ
            fmVipFetchMac = new FmVipFetchMac(mci);
            // FIFJsonBuilderコンストラクタ
            jsonbuilder = new FIFJsonBuilder.fm_main();


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
                try
                {
                    mqttClient = new MqttClient(mci.mosquittoHost);
                    var ret = mqttClient.Connect(Guid.NewGuid().ToString());
                }
                catch (Exception ex)
                {
                    ConsoleShow(ex.Message, Cnslcnf.msg_error);
                    ConsoleShow("MQTTブローカーに接続できませんでした。MQTTをOFFします。", Cnslcnf.msg_error);
                    mci.isUseMqtt = false;
                }
                
            }

            // FILEIF起動メッセージ
            if (btServerStart.Enabled)
            {
                OskNLog.Log("File InterFace(aka:MagCup)が起動しました", Cnslcnf.msg_info);

                // FIF動作条件表示
                var msg = CR;
                msg += "**********************************" + CR;
                msg += "【FIF動作条件】" + CR;
                msg += "◇デバッグ表示モード：" + mci.DebugMode + CR;
                msg += "◇PLC上位リンク有効：" + mci.UsePlcTrig + CR;
                msg += "◇Vlotリスト検査有効：" + mci.CheckVlot + CR;
                msg += "**********************************" + CR;
                OskNLog.Log(msg, Cnslcnf.msg_info);

                if (mci.AutoStart)
                {
                    StartFIF();
                }
            }
            else
            {
                OskNLog.Log("プログラムを起動できません", Cnslcnf.msg_info);
            }
        }

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.Mn_Files = new System.Windows.Forms.ToolStripMenuItem();
            this.OpenFifJsonBuilder = new System.Windows.Forms.ToolStripMenuItem();
            this.オプションToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.vIP分類設備ファイル取得ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.debugFetchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
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
            this.OpenFifJsonBuilder});
            this.Mn_Files.Name = "Mn_Files";
            this.Mn_Files.Size = new System.Drawing.Size(53, 20);
            this.Mn_Files.Text = "ファイル";
            // 
            // OpenFifJsonBuilder
            // 
            this.OpenFifJsonBuilder.Name = "OpenFifJsonBuilder";
            this.OpenFifJsonBuilder.Size = new System.Drawing.Size(122, 22);
            this.OpenFifJsonBuilder.Text = "設備設定";
            this.OpenFifJsonBuilder.Click += new System.EventHandler(this.OpenFifJsonBuilder_Click);
            // 
            // オプションToolStripMenuItem
            // 
            this.オプションToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.vIP分類設備ファイル取得ToolStripMenuItem,
            this.debugFetchToolStripMenuItem});
            this.オプションToolStripMenuItem.Name = "オプションToolStripMenuItem";
            this.オプションToolStripMenuItem.Size = new System.Drawing.Size(63, 20);
            this.オプションToolStripMenuItem.Text = "オプション";
            // 
            // vIP分類設備ファイル取得ToolStripMenuItem
            // 
            this.vIP分類設備ファイル取得ToolStripMenuItem.Name = "vIP分類設備ファイル取得ToolStripMenuItem";
            this.vIP分類設備ファイル取得ToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.vIP分類設備ファイル取得ToolStripMenuItem.Text = "VIP分類設備ファイル取得";
            this.vIP分類設備ファイル取得ToolStripMenuItem.Click += new System.EventHandler(this.OpenVipFetchMac);
            // 
            // debugFetchToolStripMenuItem
            // 
            this.debugFetchToolStripMenuItem.Name = "debugFetchToolStripMenuItem";
            this.debugFetchToolStripMenuItem.Size = new System.Drawing.Size(197, 22);
            this.debugFetchToolStripMenuItem.Text = "DebugFetch";
            this.debugFetchToolStripMenuItem.Click += new System.EventHandler(this.debugFetchToolStripMenuItem_Click);
            // 
            // FileIfFlame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.ClientSize = new System.Drawing.Size(784, 591);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FileIfFlame";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "FIF";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MagCupFlame_FormClosing);
            this.Controls.SetChildIndex(this.bt_ClearErrLogs, 0);
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
            StartFIF();
        }


        private void StartFIF()
        {
            if (!serverOn)
            {
                // mugcup.iniを読み込み
                //if (!mci.GetMugCupIniValues(ref globalmsg))
                if (!mci.GetMugCupYamlValues(ref globalmsg))
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
                                OskNLog.Log("デバッグ表示モードでFILEIFを開始しました", Cnslcnf.msg_info);
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

                // タイムアウトファイル監視
                if (TofCnt1 == 10)
                {
                    // WIPファイルの検索（タイムアウト監視）
                    tskhdl.Tasks("WIP", mci.wipfilekey);

                    // ENDファイルの検索（タイムアウト監視）
                    tskhdl.Tasks("END", mci.endfilekey);

                    TofCnt1 = 1;
                }
                TofCnt1++;

                // 期限切れファイル監視
                if (TofCnt2 == 180)
                {
                    // Doneファイルの検索（保管期限切れ監視）
                    tskhdl.Tasks("DONE", mci.donefilekey);

                    // errファイルの検索（保管期限切れ監視）
                    tskhdl.Tasks("ERROR", mci.errfilekey);

                    TofCnt2 = 1;
                }
                TofCnt2++;

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

        private void OpenFifJsonBuilder_Click(object sender, EventArgs e)
        {
            if (!jsonbuilder.Visible)
            {
                if (jsonbuilder.IsDisposed)
                {
                    jsonbuilder = new FIFJsonBuilder.fm_main();
                }
                jsonbuilder.Show(this);
            }
            else
            {
                ConsoleShow("表示されてますよ！", Cnslcnf.msg_alarm);
            }
        }

        private void OpenVipFetchMac(object sender, EventArgs e)
        {
            if (!fmVipFetchMac.Visible)
            {
                if (fmVipFetchMac.IsDisposed)
                {
                    fmVipFetchMac = new FmVipFetchMac(mci);
                }
                fmVipFetchMac.Show(this);
            }
            else
            {
                ConsoleShow("既に表示されています", Cnslcnf.msg_error);
            }
        }

        private void debugFetchToolStripMenuItem_Click(object sender, EventArgs e)
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
