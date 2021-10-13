using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Oskas;
using uPLibrary.Networking.M2Mqtt;

namespace FileIf

{
    public class FileIfFlame: Oskas.fmMain
    {
        Tasks_Common tcommons = new Tasks_Common();
        MySQL db = new MySQL();

        // mqttインスタンス
        MqttClient mqttClient;
        OskasMqttClient mq = new OskasMqttClient();

        Magcupini mci = new Magcupini();

        // サーバー稼働許可
        bool serverOn = false;

        // 参照渡し用メッセージ変数
        string globalmsg;

        // タイムアウト監視用カウンタ
        int TofCnt = 1;
        private MenuStrip menuStrip;
        private ToolStripMenuItem Mn_Files;
        private ToolStripMenuItem Mn_Files_OpenFif;

        // TaskHander
        TaskHandler tskhdl;

        // インターロック設備リスト
        List<string> IntLokMac = new List<string>();

        public FileIfFlame()
        {
            InitializeComponent();
            TaskTimer.Enabled = false;
            toolStripStatusLabel1.Text = string.Format("サーバーは停止中です");

            // NLogのパス設定
            OskNLog.setFolderName("magcup");

            // mugcup.iniを読み込み
            if (!mci.GetMugCupIniValues(ref globalmsg))
            {
                btServerStart.Enabled = false;
                OskNLog.Log(globalmsg, Cnslcnf.msg_error);
            }

            if (!db.PingUpdatesState(mci.ConnectionStrings, ref globalmsg))
            {
                btServerStart.Enabled = false;
                OskNLog.Log(globalmsg, Cnslcnf.msg_error);
            }
            else
                OskNLog.Log(globalmsg, Cnslcnf.msg_info);

            if (mci.isUseMqtt)
            {
                mqttClient = new MqttClient(mci.mosquittoHost);
                var ret = mqttClient.Connect(Guid.NewGuid().ToString());
            }

            if (btServerStart.Enabled)
                OskNLog.Log("File InterFace(aka:MagCup)が起動しました", 0);

        }

        private void InitializeComponent()
        {
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.Mn_Files = new System.Windows.Forms.ToolStripMenuItem();
            this.Mn_Files_OpenFif = new System.Windows.Forms.ToolStripMenuItem();
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
            this.btServerStart.Location = new System.Drawing.Point(645, 504);
            this.btServerStart.Size = new System.Drawing.Size(127, 32);
            this.btServerStart.Click += new System.EventHandler(this.btServerStart_Click);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.Mn_Files});
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
            // FileIfFlame
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 18F);
            this.ClientSize = new System.Drawing.Size(784, 561);
            this.Controls.Add(this.menuStrip);
            this.MainMenuStrip = this.menuStrip;
            this.Name = "FileIfFlame";
            this.Text = "VSP://FileInterFace";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MagCupFlame_FormClosing);
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
            if (serverOn == false)
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
                        if (!db.PingUpdatesState(mci.ConnectionStrings, ref globalmsg))
                        {
                            btServerStart.Enabled = false;
                            OskNLog.Log(globalmsg, Cnslcnf.msg_error);
                        }
                        else
                        {
                            serverOn = true;
                            TaskTimer.Enabled = true;
                            btServerStart.Text = "Stop Server";
                            if (mci.DebugMode)
                            {
                                OskNLog.Log("デバックモードでサーバーを開始しました", Cnslcnf.msg_info);
                            }
                            else
                            {
                                OskNLog.Log("サーバーを開始しました", Cnslcnf.msg_info);
                            }

                            toolStripStatusLabel1.Text = string.Format("サーバーは稼働中です");
                        }
                    }
                    else
                    {
                        MessageBox.Show("ログの書き出しができないためサーバー稼働開始できません");
                    }
                }
            }
            else
            {
                serverOn = false;
                TaskTimer.Enabled = false;
                btServerStart.Text = "Start Server";
                OskNLog.Log("サーバーを停止しました", Cnslcnf.msg_info);
                toolStripStatusLabel1.Text = string.Format("サーバーは停止中です");
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
                tskhdl = new TaskHandler(tcommons, mci, IntLokMac);

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


        ////
        ///* タスク関数【Task0】
        // * 【Task1】
        // * 　登録したファイル検出キー毎に検索してList化
        // * 【Task2】
        // * 　検出されたファイルは検出キーに沿って処理
        // * 
        // */
        ////
        //public void Tasks(string TaskCat, string[] filekey)
        //{
        //    try
        //    {
        //        List<string> Files = new List<string>();
        //        foreach (string key in filekey)
        //        {
        //            Task1(ref Files, key);
        //        }
        //        if (Files.Count() != 0)
        //        {
        //            OskNLog.Log(Files.Count() + "件の" + TaskCat + "フォルダ関連のファイルを検出しました", Cnslcnf.msg_info);
        //            foreach (string file in Files)
        //            {
        //                /////////////////////////////////
        //                // filesysクラスの実体化
        //                //  
        //                //
        //                ////////////////////////////////
        //                Mcfilesys fs = new Mcfilesys();

        //                fs.filepath = file; //設備タスク対象ファイル名
        //                fs.lowerfilepath = file.ToLower(); //対象ファイル小文字
        //                fs.Upperfilepath = file.ToUpper(); //対象ファイル大文字
        //                fs.tmpfilepath = fs.filepath.Replace(@"\in\", @"\temp\"); //TMPフォルダパス
        //                fs.key = Tasks_Common.FindKey(mci, fs.lowerfilepath); //ファイル名から抽出したタスクキー(_min1.csvなど)
        //                fs.ff = fs.filepath.Split(System.IO.Path.DirectorySeparatorChar); //ファイル名のディレクトリ分割
        //                int indexofmagcup = Array.IndexOf(fs.ff, "magcupdir", 0);
        //                fs.Pcat = fs.ff[indexofmagcup + 1]; //設備カテゴリ
        //                fs.Macno = fs.ff[indexofmagcup + 2]; //設備No
        //                fs.FindFold = fs.ff[indexofmagcup + 3]; //ファイルが検出されたワークフォルダ
        //                fs.MagCupNo = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), ""); //ファイル名（拡張子なし）
        //                fs.RecipeFile = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), "");  //レシピ名（拡張子なし）
        //                fs.fpath = $"{mci.MCDir}\\{fs.Pcat}\\{fs.Macno}"; //設備フォルダパス（ルート）
        //                fs.keylbl = Tasks_Common.FindKeyAlt(mci, fs.lowerfilepath); // タスクキーラベル(min1など)
        //                fs.mclbl = ""; //タスク種別のラベル（タスク内で代入）
        //                string[] extstr = fs.lowerfilepath.Split('.');
        //                fs.ext = extstr[1];

        //                //fs.ConnectionStrings = mci.ConnectionStrings; //DB接続文字配列
        //                //fs.WipDir = mci.WipDir; // WIPひな形フォルダ（現在未使用）
        //                //fs.RecipUpLoadDir = mci.RecipUpLoadDir; //レシピバックアップをアップロード先
        //                //fs.UsePlcTrig = mci.UsePlcTrig; //【デバック用設定】PLCを使用しない
        //                fs.mci = mci; //iniクラスの取り込み

        //                if (!IntLokMac.Contains(fs.Macno))
        //                {
        //                    if (TaskCat == "IN") // INファイルはTask2を実施する
        //                    {
        //                        /*////////////////////////////////////////////////////
        //                        // 非同期処理はコメントアウト
        //                        // 設備のインターロック処理
        //                        //IntLokMac.Add(fs.Macno);
        //                        */////////////////////////////////////////////////////

        //                        //ConfigJson読込関数
        //                        string MacConfPath = mci.MCDir + @"\\" + fs.Pcat + @"\\" + fs.Macno + @"\\" + @"conf\macconf.json";
        //                        if (!Macconfjson2fs(fs, MacConfPath))
        //                        {
        //                            OskNLog.Log("設備設定ファイル(macconf.json)の条件またはkeyに異常があります", Cnslcnf.msg_error);
        //                            string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
        //                            string[] mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
        //                            if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
        //                            break; //foreachを抜ける
        //                        }

        //                        Task<bool> task2 = Task.Run(() =>
        //                        {
        //                            return Task2(fs);
        //                        });

        //                        // タスク開始間の間隔を100msおいてみる
        //                        Thread.Sleep(100);

        //                        /*////////////////////////////////////////////////////
        //                        // 非同期処理はコメントアウト
        //                        if (task2.Result)
        //                        {
        //                            // 設備のインターロック解除
        //                            IntLokMac.RemoveAll(s => s.Contains(fs.Macno));
        //                            TaskTimer.Enabled = true;
        //                        }
        //                        else
        //                        {
        //                            // 設備のインターロック解除
        //                            IntLokMac.RemoveAll(s => s.Contains(fs.Macno));
        //                            TaskTimer.Enabled = true;
        //                            OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")/ MgNo:" + MagCupNo + "のタスクが異常終了しました", msg_error);
        //                        }
        //                        */////////////////////////////////////////////////////
        //                    }
        //                    else // INファイル以外はタイムアウト監視：タイムアウトファイルはエラーフォルダに移動
        //                    {
        //                        Task<bool> tmout = Task.Run(() =>
        //                        {
        //                            return TimeOutTask(file, fs);
        //                        });
        //                    }
        //                }
        //            } 
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【Task0】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //    }
            

        //}


        ////
        ///* Task1関数
        // * 登録したファイル検出キー毎に検索してList化
        // * 
        // */
        ////
        //private void Task1(ref List<string> Files, string KeySearc)
        //{
        //    try
        //    {
        //        DirSearch(mci.MCDir, KeySearc, ref Files);
        //    }
        //    catch (Exception e)
        //    {
        //        OskNLog.Log("【Task1】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(e.Message, Cnslcnf.msg_error);
        //    }
        //}


        ////
        ///* Task2関数
        // * 設備ごとのインターロック管理を実施
        // * 
        // */
        ////
        //private bool Task2(Mcfilesys fs)
        //{
        //    string[] FOTaskRslt = new string[3];

        //    ////////////////////////////////
        //    // 設備のインターロック処理  //
        //    IntLokMac.Add(fs.Macno);    //
        //    /////////////////////////////
        //    OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを設置しました", Cnslcnf.msg_debug);

        //    try
        //    {
        //        if (!InFileTaskCont(fs))
        //        {
        //            throw new Exception();
        //        }

        //        // Task終了時ファイル消去・移動の遅れ対策ディレイ
        //        Thread.Sleep(500);
        //        ///////////////////////////////////////////////////////////
        //        // 設備のインターロック解除（同期の場合コメントアウト） //
        //        IntLokMac.RemoveAll(s => s.Contains(fs.Macno));        //
        //        //TaskTimer.Enabled = true; //同期の場合は必要        //
        //        ///////////////////////////////////////////////////////
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを解除しました", Cnslcnf.msg_debug);
        //        //
        //        return true;
        //    }
        //    catch
        //    {
        //        // Task終了時ファイル消去・移動の遅れ対策ディレイ
        //        Thread.Sleep(500);
        //        ///////////////////////////////////////////////////////////
        //        // 設備のインターロック解除（同期の場合コメントアウト） //
        //        IntLokMac.RemoveAll(s => s.Contains(fs.Macno));        //
        //        //TaskTimer.Enabled = true; //同期の場合は必要        //
        //        ///////////////////////////////////////////////////////
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")/ MgNo:" + fs.MagCupNo + "のタスクが異常終了しました", Cnslcnf.msg_error);
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを解除しました", Cnslcnf.msg_debug);
        //        //
        //        return false;
        //    }
        //}

        ////
        ///* Task2の実作業関数
        // * 検出されたファイルは検出キーに沿って処理
        // * 
        // */
        ////
        //private bool InFileTaskCont(Mcfilesys fs)
        //{
        //    try
        //    {
        //        string[] DBTaskRslt = new string[4];
        //        string[] FOTaskRslt = new string[4];

        //        Tasks_MagCup Tsk = new Tasks_MagCup();

        //        if (fs.FindFold == "in") //【ファイル検出場所】がINフォルダの場合（正常）
        //        {
        //            ///////////////////////////////////////////////
        //            // タスク処理ルーター
        //            ///////////////////////////////////////////////
        //            if (!DBTaskRsltRouter(fs, Tsk, ref DBTaskRslt))
        //            {
        //                return false;
        //            }

        //            ///////////////////////////////////////////////
        //            // タスク結果処理
        //            ///////////////////////////////////////////////
        //            if (DBTaskRslt[0] == "OK") //【DBタスク】が正常完了の場合
        //            {
        //                if (!DBTaskRsltIsOK(fs, Tsk, DBTaskRslt, ref FOTaskRslt))
        //                {
        //                    return false;
        //                }
        //            }
        //            else if (DBTaskRslt[0] == "NG") //【DBタスク】が正常でない場合
        //            {
        //                if (!DBTaskRsltIsNG(fs, Tsk, DBTaskRslt, ref FOTaskRslt))
        //                {
        //                    return false;
        //                }
        //            }
        //            else if (DBTaskRslt[0] == "Cancel") //【DBタスク】中止する場合
        //            {
        //                if (!DBTaskRsltIsCancel(fs, Tsk, DBTaskRslt, ref FOTaskRslt))
        //                {
        //                    return false;
        //                }
        //            }

        //            ///////////////////////////////////////////////
        //            // 異常終了時処理
        //            // return falseなし（でよいの？）
        //            ///////////////////////////////////////////////
        //            // 【OUTフォルダタスク】が異常終了の場合
        //            if (FOTaskRslt[0] == "NG")
        //            {
        //                OskNLog.Log(DBTaskRslt[2], Cnslcnf.msg_debug);
        //                OskNLog.Log(FOTaskRslt[1], Cnslcnf.msg_error);
        //                OskNLog.Log("サーバー動作異常：エラーが発生しています、管理者に報告してください", Cnslcnf.msg_error);
        //                string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
        //                //
        //                //inファイルをエラーフォルダに移動
        //                //
        //                //[temp]=>[error]
        //                string[] mef = tcommons.MoveErrorFile(fs, fs.tmpfilepath, ErrorPath);
        //                if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
        //                //[in]=>[error]
        //                mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
        //                if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
        //            }
        //        }
        //        else //【ファイル検出場所】がINフォルダ以外（異常）
        //        {
        //            OskNLog.Log(fs.MagCupNo + "のINフォルダ関連のファイルがINフォルダ以外で検出：" + fs.filepath, Cnslcnf.msg_alarm);
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【Task2】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        //private bool DBTaskRsltRouter(Mcfilesys fs, Tasks_MagCup Tsk, ref string[] DBTaskRslt)
        //{
        //    try
        //    {
        //        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} のタスク({fs.keylbl})処理開始", Cnslcnf.msg_info);

        //        switch (fs.key)
        //        {
        //            case "_min1.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Min1.DBTasks(fs);
        //                break;
        //            case "_min2.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Min2.DBTasks(fs);
        //                break;
        //            case "_mot.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Mout.DBTasks(fs);
        //                break;
        //            case "_rcp.txt":
        //                fs.mclbl = "Recipe";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Crcp.DBTasks(fs);
        //                break;
        //            case "_sta.csv":
        //                fs.mclbl = "Recipe";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Csta.DBTasks(fs);
        //                break;
        //            case "_cot1.csv":
        //                fs.mclbl = "Cup";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Cot1.DBTasks(fs);
        //                break;
        //            case "_cot2.csv":
        //                fs.mclbl = "Cup";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                DBTaskRslt = Tsk.Cot2.DBTasks(fs);
        //                break;
        //        }

        //        return true;
        //    }
        //    catch(Exception ex)
        //    {
        //        OskNLog.Log("【DBTaskRsltRouter】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        //private bool DBTaskRsltIsOK(Mcfilesys fs, Tasks_MagCup Tsk, string[] DBTaskRslt, ref string[] FOTaskRslt)
        //{
        //    try
        //    {
        //        switch (fs.key)
        //        {
        //            case "_min1.csv":
        //                FOTaskRslt = Tsk.Min1.FOutTasks(fs, 0);
        //                break;
        //            case "_min2.csv":
        //                FOTaskRslt = Tsk.Min2.FOutTasks(fs, 0);
        //                break;
        //            case "_mot.csv":
        //                FOTaskRslt = Tsk.Mout.FOutTasks(fs, 0);
        //                break;
        //            case "_rcp.txt":
        //                FOTaskRslt = Tsk.Crcp.FOutTasks(fs, 0);
        //                break;
        //            case "_sta.csv":
        //                FOTaskRslt = Tsk.Csta.FOutTasks(fs, 0);
        //                break;
        //            case "_cot1.csv":
        //                FOTaskRslt = Tsk.Cot1.FOutTasks(fs, 0);
        //                break;
        //            case "_cot2.csv":
        //                FOTaskRslt = Tsk.Cot2.FOutTasks(fs, 0);
        //                break;
        //        }

        //        if (FOTaskRslt[0] == "OK") //【OUTフォルダタスク】が正常終了の場合
        //        {
        //            OskNLog.Log(DBTaskRslt[2], Cnslcnf.msg_debug);
        //            if (DBTaskRslt[1] != "") OskNLog.Log(DBTaskRslt[1], Cnslcnf.msg_info);
        //            //
        //            //inファイルの消去
        //            //
        //            File.Delete(fs.tmpfilepath);

        //            OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} のタスクは正常に完了しました", Cnslcnf.msg_info);
        //            OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルを削除しました", Cnslcnf.msg_info);
        //        }
        //        // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【DBTaskRsltIsOK】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        //private bool DBTaskRsltIsNG(Mcfilesys fs, Tasks_MagCup Tsk, string[] DBTaskRslt, ref string[] FOTaskRslt)
        //{
        //    try
        //    {
        //        //【OUTフォルダタスク】マガジン情報出力
        //        switch (fs.key)
        //        {
        //            case "_min1.csv":
        //                FOTaskRslt = Tsk.Min1.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //            case "_min2.csv":
        //                FOTaskRslt = Tsk.Min2.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //            case "_mot.csv":
        //                FOTaskRslt = Tsk.Mout.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //            case "_rcp.txt":
        //                FOTaskRslt = Tsk.Crcp.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //            case "_sta.csv":
        //                FOTaskRslt = Tsk.Csta.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //            case "_cot1.csv":
        //                FOTaskRslt = Tsk.Cot1.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //            case "_cot2.csv":
        //                FOTaskRslt = Tsk.Cot2.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
        //                break;
        //        }

        //        if (FOTaskRslt[0] == "OK") //【OUTフォルダタスク】が正常終了の場合
        //        {
        //            OskNLog.Log(DBTaskRslt[2], Cnslcnf.msg_debug);
        //            OskNLog.Log(DBTaskRslt[1], Cnslcnf.msg_error);
        //            string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
        //            //
        //            //inファイルをエラーフォルダに移動
        //            //
        //            //[temp]=>[error]
        //            string[] mef = tcommons.MoveErrorFile(fs, fs.tmpfilepath, ErrorPath);
        //            if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
        //            //[in]=>[error]
        //            mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
        //            if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
        //        }
        //        // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【DBTaskRsltIsNG】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        //private bool DBTaskRsltIsCancel(Mcfilesys fs, Tasks_MagCup Tsk, string[] DBTaskRslt, ref string[] FOTaskRslt)
        //{
        //    try
        //    {
        //        //////////////////////////////////////////////////////////////////////////////////////
        //        // 設備側からタスクのキャンセル要求(ERROR返信)があった場合はMagCupDirをクリーンします
        //        // 但し、レシピについては
        //        //  　①キャンセル返信が必要 ②INが同時に複数あり得る
        //        // ことからクリーンはIN/OUTを除外します。
        //        //
        //        // =>クリーンの方法は別途検討したほうがいいと思われます！！！
        //        //
        //        //////////////////////////////////////////////////////////////////////////////////////
        //        //【OUTフォルダタスク】マガジン情報出力

        //        switch (fs.key)
        //        {
        //            case "_sta.csv":
        //                FOTaskRslt = Tsk.Csta.FOutTasks(fs, 999);
        //                break;
        //        }
        //        var fldn = new List<string> { "wip", "temp", "in", "out" };
        //        cleanfiles(fs, fldn);
        //        OskNLog.Log(DBTaskRslt[2], Cnslcnf.msg_debug);
        //        if (DBTaskRslt[1] != "") OskNLog.Log(DBTaskRslt[1], Cnslcnf.msg_info);

        //        if (FOTaskRslt[0] == "OK") FOTaskRslt[0] = "Cancel";
        //        // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【DBTaskRsltIsCancel】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        //private bool TimeOutTask(string filepath, Mcfilesys fs)
        //{
        //    try
        //    {
        //        string[] DBTaskRslt = new string[4];
        //        string[] FOTaskRslt = new string[4];

        //        TimeSpan timeout = new TimeSpan(0, 0, mci.outfiletimeout);
        //        DateTime ts = System.IO.File.GetLastWriteTime(filepath); //削除後すぐに同名ファイルが書き込まれると上書きになる場合があるみたい
        //        DateTime dt = DateTime.Now;

        //        if ((dt - ts - timeout) > new TimeSpan(0, 0, 0))
        //        {
        //            OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}はタイムスタンプ[" + ts.ToString("yyyy-MM-dd HH:mm:ss") + "]にてタイムアウトとしました", Cnslcnf.msg_alarm);
        //            string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
        //            //
        //            //inファイルをエラーフォルダに移動
        //            //
        //            if (CommonFuncs.MoveFile(filepath, ErrorPath))
        //            {
        //                OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}はエラーフォルダに移動しました", Cnslcnf.msg_info);
        //            }
        //            else
        //            {
        //                OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}ファイルはエラーフォルダに移動できません", Cnslcnf.msg_alarm);
        //            }
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【TimeOutTask】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        ////
        ///* 設備フォルダのクリーン
        //*/
        ////
        //private void cleanflders(Mcfilesys fs, List<string> fldn)
        //{
        //    if (tcommons.CleanMagCupDir(fs, fldn))
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のワークスペースをクリーンしました", Cnslcnf.msg_alarm);
        //    else
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のワークスペースをクリーンできません", Cnslcnf.msg_error);
        //}

        ////
        ///* 設備フォルダのクリーン
        //*/
        ////
        //private void cleanfiles(Mcfilesys fs, List<string> fldn)
        //{
        //    if (tcommons.CleanMagCupfiles(fs, fldn))
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")の対象ファイルをクリーンしました", Cnslcnf.msg_alarm);
        //    else
        //        OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")の対象ファイルをクリーンできません", Cnslcnf.msg_error);
        //}

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
            //サーバーステータスのパブリッシュ
            if (mci.isUseMqtt)
            {
                mq.Mqtt_ServerStatus(mqttClient, "magcup", serverOn);
            }
        }

       
        //
        /* 検出ファイル文字列から検出されたキー(String)を返します
         * 検出されない場合は "None"を返します
        */
        //
        /*
        private string FindKey(string File)
        {
            foreach (string key in mci.infilekey)
            {
                if (File.Contains(key))
                {
                    return key;
                }
            }
            foreach (string key in mci.wipfilekey)
            {
                if (File.Contains(key))
                {
                    return key;
                }
            }
            foreach (string key in mci.endfilekey)
            {
                if (File.Contains(key))
                {
                    return key;
                }
            }
            return "None";
        }
        */

        //
        /* 検出ファイル文字列から検出されたキー(String)を返します
         * 検出されない場合は "None"を返します
        */
        //
        /*
        private string FindKeyAlt(string File)
        {
            foreach (string key in mci.infilekey)
            {
                if (File.Contains(key))
                {
                    string keyAlt = key.Replace(".csv", "");
                    keyAlt = keyAlt.Replace(".txt", "");
                    keyAlt = keyAlt.Replace("_", "");
                    return keyAlt;
                }
            }
            foreach (string key in mci.wipfilekey)
            {
                if (File.Contains(key))
                {
                    string keyAlt = key.Replace(".csv", "");
                    keyAlt = keyAlt.Replace("_", "");
                    return keyAlt;
                }
            }
            foreach (string key in mci.endfilekey)
            {
                if (File.Contains(key))
                {
                    string keyAlt = key.Replace(".csv", "");
                    keyAlt = keyAlt.Replace("_", "");
                    return keyAlt;
                }
            }
            return "None";
        }
        */

        //private bool Macconfjson2fs(Mcfilesys fs, string MacConfPath)
        //{
        //    try
        //    {
        //        bool findmcplc = false, findmcpc = false;
        //        fs.mconf = JsonConvert.DeserializeObject<macconfjson>(CommonFuncs.JsonFileReader(MacConfPath));
        //        // 
        //        // mconfから検出されたmcfileの設定を抜く
        //        //
        //        for (int i = 0; i < fs.mconf.Mcfs.mcfconfs.Count; i++)
        //        {
        //            if (fs.key.Contains(fs.mconf.Mcfs[i].mcfilekey))
        //            {
        //                fs.mcfc = fs.mconf.Mcfs[i];
        //                OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のファイル設定確認", Cnslcnf.msg_debug);
        //            }
        //        }
        //        //
        //        // mcfile設定からコントローラ情報を抜く
        //        //
        //        if (fs.mcfc.foi.cnttype == "PLC")
        //        {
        //            for (int i = 0; i < fs.mconf.Plcs.plcconfs.Count; i++)
        //            {
        //                if (fs.mconf.Plcs[i].name == fs.mcfc.foi.cntid) // MagCupで使用するPLCの検出
        //                {
        //                    fs.plcc = fs.mconf.Plcs[i];
        //                    OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPLCを確認しました", Cnslcnf.msg_debug);
        //                    findmcplc = true;
        //                    break;
        //                }
        //            }
        //            if (!findmcplc)
        //            {
        //                OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPLC設定が不正です", Cnslcnf.msg_error);
        //                return false;
        //            }

        //        }
        //        else if (fs.mcfc.foi.cnttype == "PC")
        //        {
        //            for (int i = 0; i < fs.mconf.Pcs.pcconfs.Count; i++)
        //            {
        //                if (fs.mconf.Pcs[i].name == fs.mcfc.foi.cntid) // MagCupで使用するPCの検出
        //                {
        //                    fs.pcc = fs.mconf.Pcs[i];
        //                    OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPCを確認しました", Cnslcnf.msg_debug);
        //                    findmcpc = true;
        //                    break;
        //                }
        //            }
        //            if (!findmcpc)
        //            {
        //                OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPC設定が不正です", Cnslcnf.msg_error);
        //                return false;
        //            }
        //        }
        //        else
        //        {
        //            OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のコントローラ設定、またはkeyが不正です", Cnslcnf.msg_error);
        //            return false;
        //        }
        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【Macconfjson2fs】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>" + ex.ToString(), Cnslcnf.msg_debug);
        //        return false;
        //    }

        //}

        private void MagCupFlame_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (mci.isUseMqtt && mqttClient.IsConnected)
            {
                mqttClient.Disconnect();
            }
        }

    }

}
