using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Oskas;

namespace MagCapServer
{
    public partial class fmMain : Form
    {
        CommonFuncs commons = new CommonFuncs();
        Tasks_Common tcommons = new Tasks_Common();
        MySQL db = new MySQL();
        //MqttClient mq = new MqttClient();
        magcupini mci = new magcupini();

        // 改行コード
        static string crlf = "\r\n";

        // サーバー稼働許可
        bool serverOn = false;
        // 参照渡し用メッセージ変数
        string msg;
        string globalmsg;
        // タイムアウト監視用カウンタ
        int TofCnt = 1;
        // インターロック設備リスト
        List<string> IntLokMac = new List<string>();
        //Console message Lebel(INT)
        const int msg_info = 0;
        const int msg_detect = 1;
        const int msg_task = 2;
        const int msg_alarm = 3;
        const int msg_error = 4;
        const int msg_debug = 10;
        const int mesMaxLen = 1000;
        int mesLen = 0;

        public fmMain()
        {
            InitializeComponent();
            TaskTimer.Enabled = false;
            toolStripStatusLabel1.Text = string.Format("サーバーは停止中です");

            // mugcup.iniを読み込み
            if (!mci.GetMugCupIniValues(ref globalmsg))
            {
                btServerStart.Enabled = false;
                ConsoleShow(globalmsg, msg_error);
            }
            // MagCupDBにPingを投げる
            if (!db.PingUpdatesState(mci.ConnectionStrings, ref globalmsg))
            {
                btServerStart.Enabled = false;
                ConsoleShow(globalmsg, msg_error);
            }
            else
                ConsoleShow(globalmsg, msg_info);

            if (btServerStart.Enabled)
                ConsoleShow("アプリケーションが起動されました", 0);

            //mqtt test
            //timer2.Enabled = true;
        }


        delegate void ConsoleDelegate(string text, int level);
        private void ConsoleShow(string text, int level)
        {
            if (consoleBox.InvokeRequired)
            {
                ConsoleDelegate d = new ConsoleDelegate(ConsoleShow);
                BeginInvoke(d, new object[] {text, level});
            }
            else
            {
                if (mesLen == 0)
                {
                    consoleBox.Clear();
                }
                mesLen += 1;
                string message = "";
                DateTime today = DateTime.Today;
                DateTime now = DateTime.Now;
                string dt = now.ToString("yyyy-MM-dd HH:mm:ss] ");
                switch (level)
                {
                    case msg_info:
                        message = "[INFO: " + dt;
                        break;
                    case msg_detect:
                        message = "[DETECT: " + dt;
                        break;
                    case msg_task:
                        message = "[TASK: " + dt;
                        break;
                    case msg_alarm:
                        message = "[ALARM: " + dt;
                        break;
                    case msg_error:
                        message = "[ERROR: " + dt;
                        break;
                    case msg_debug:
                        if (mci.DebugMode)
                        {
                            message = "[DEBUG: " + dt + crlf;
                            consoleBox.AppendText(message + text + crlf);
                        }
                        break;
                }
                if (level != msg_debug)
                    consoleBox.AppendText(message + text + crlf);


                ///////////////////////////////////////////////////////////////////////////////////////
                // メッセージ件数が1000を超えた場合、TextBox(consoleBox)をクリアする
                ///////////////////////////////////////////////////////////////////////////////////////
                if (mesLen > mesMaxLen)
                {
                    consoleBox.AppendText($"■■■メッセージ件数が{mesMaxLen}件を超えました。次のメッセージで表示ログはクリアされます。■■■" + crlf);
                    mesLen = 0;
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // 日付が変わった場合、ログファイル名を現在時刻に再設定する
                ///////////////////////////////////////////////////////////////////////////////////////
                if (now.Date > mci.SrvStatDT.Date)
                {
                    mci.MsglogPath = mci.MsglogDir + @"\" + now.ToString("yyyyMMddHHmmss") + ".txt";
                }

                ///////////////////////////////////////////////////////////////////////////////////////
                // ログ書き出し
                ///////////////////////////////////////////////////////////////////////////////////////
                if (serverOn && !AppendMsgLog(mci.MsglogPath, message + dt + text))
                {
                    // ここの処理は要検討！
                    MessageBox.Show("ログの書き出しができません。");
                }
            }
        }


        public bool MakeMsgLog(string FilePath)
        {
            Encoding enc = Encoding.GetEncoding("utf-8");

            try
            {
                StreamWriter writer = new StreamWriter(FilePath, false, enc);
                writer.Close();
            }
            catch
            {
                return false;
            }

            if (!File.Exists(FilePath))
            {
                return false;
            }

            return true;
        }


        private bool AppendMsgLog(string FilePath, string msg)
        {
            Encoding enc = Encoding.GetEncoding("utf-8");

            File.AppendAllText(FilePath, msg + crlf);

            if (!File.Exists(FilePath))
            {
                return false;
            }

            return true;
        }


        private void btServerStart_Click(object sender, EventArgs e)
        {
            if (serverOn == false)
            {
                // mugcup.iniを読み込み
                if (!mci.GetMugCupIniValues(ref globalmsg))
                {
                    btServerStart.Enabled = false;
                    ConsoleShow(globalmsg, msg_error);
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
                            ConsoleShow(globalmsg, msg_error);
                        }
                        else
                        {
                            serverOn = true;
                            TaskTimer.Enabled = true;
                            btServerStart.Text = "Stop Server";
                            if (mci.DebugMode)
                            {
                                ConsoleShow("デバックモードでサーバーを開始しました", msg_info);
                            }
                            else
                            {
                                ConsoleShow("サーバーを開始しました", msg_info);
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
                ConsoleShow("サーバーを停止しました", msg_info);
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
                ConsoleShow("ファイル取得時に問題が発生しました", msg_error);
                ConsoleShow(ex.Message, 4);
            }
        }

        
        private void TaskTimer_Tick(object sender, EventArgs e)
        {
            // INファイルの検索
            Tasks("IN", mci.infilekey);

            if (TofCnt==5)
            {
                // WIPファイルの検索（タイムアウト監視）
                Tasks("WIP", mci.wipfilekey);

                // ENDファイルの検索（タイムアウト監視）
                Tasks("END", mci.endfilekey);

                TofCnt = 1;
            }

            TofCnt++;
        }


        //
        /* タスク関数
         * 【Task1】
         * 　登録したファイル検出キー毎に検索してList化
         * 【Task2】
         * 　検出されたファイルは検出キーに沿って処理
         * 
         */
        //
        private void Tasks(string TaskCat, string[] filekey)
        {
            TaskTimer.Enabled = false;

            List<string> Files = new List<string>();
            foreach (string key in filekey)
            {
                Task1(ref Files, key);
            }
            if (Files.Count() != 0)
            {
                ConsoleShow(Files.Count() + "件の" + TaskCat + "フォルダ関連のファイルを検出しました", msg_debug);
                foreach (string file in Files)
                {
                    /////////////////////////////////
                    // magcupfilesysクラスの実体化
                    ////////////////////////////////
                    mcfilesys fs = new mcfilesys();

                    fs.filepath = file; //設備タスク対象ファイル名
                    fs.lowerfilepath = file.ToLower(); //対象ファイル小文字
                    fs.Upperfilepath = file.ToUpper(); //対象ファイル大文字
                    fs.tmpfilepath = fs.filepath.Replace(@"\in\", @"\temp\"); //TMPフォルダパス
                    fs.key = FindKey(fs.lowerfilepath); //ファイル名から抽出したタスクキー(_min1.csvなど)
                    fs.ff = fs.filepath.Split(System.IO.Path.DirectorySeparatorChar); //ファイル名のディレクトリ分割
                    fs.Pcat = fs.ff[2]; //設備カテゴリ
                    fs.Macno = fs.ff[3]; //設備No
                    fs.FindFold = fs.ff[4]; //ファイルが検出されたワークフォルダ
                    fs.MagCupNo = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), ""); //ファイル名（拡張子なし）
                    fs.RecipeFile = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), "");  //レシピ名（拡張子なし）
                    fs.fpath = $"{mci.MCDir}\\{fs.Pcat}\\{fs.Macno}"; //設備フォルダパス（ルート）
                    fs.keylbl = FindKeyAlt(fs.lowerfilepath); // タスクキーラベル(min1など)
                    fs.mclbl = ""; //タスク種別のラベル（タスク内で代入）
                    string[] extstr = fs.lowerfilepath.Split('.');
                    fs.ext = extstr[1];

                    //fs.ConnectionStrings = mci.ConnectionStrings; //DB接続文字配列
                    //fs.WipDir = mci.WipDir; // WIPひな形フォルダ（現在未使用）
                    //fs.RecipUpLoadDir = mci.RecipUpLoadDir; //レシピバックアップをアップロード先
                    //fs.UsePlcTrig = mci.UsePlcTrig; //【デバック用設定】PLCを使用しない
                    fs.mci = mci; //iniクラスの取り込み

                    if (!IntLokMac.Contains(fs.ff[3]))
                    {
                        if (TaskCat=="IN") // INファイルはTask2を実施する
                        {
                            /*////////////////////////////////////////////////////
                            // 非同期処理はコメントアウト
                            // 設備のインターロック処理
                            //IntLokMac.Add(ff[3]);
                            */////////////////////////////////////////////////////

                            //ConfigJson読込関数
                            string MacConfPath = mci.MCDir + @"\\" + fs.Pcat + @"\\" + fs.Macno + @"\\" + @"conf\macconf.json";
                            if (!Macconfjson2fs(fs, MacConfPath))
                            {
                                ConsoleShow("設備設定ファイル(macconf.json)の読込に異常があります", msg_error);
                                string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                                break; //foreachを抜ける
                            }

                            Task<bool> task2 = Task.Run(() =>
                            {
                                return Task2(fs);
                            });

                            // タスク開始間の間隔を100msおいてみる
                            Thread.Sleep(100);

                            /*////////////////////////////////////////////////////
                            // 非同期処理はコメントアウト
                            if (task2.Result)
                            {
                                // 設備のインターロック解除
                                IntLokMac.RemoveAll(s => s.Contains(ff[3]));
                                TaskTimer.Enabled = true;
                            }
                            else
                            {
                                // 設備のインターロック解除
                                IntLokMac.RemoveAll(s => s.Contains(ff[3]));
                                TaskTimer.Enabled = true;
                                ConsoleShow("設備:" + ff[2] + "(" + ff[3] + ")/ MgNo:" + MagCupNo + "のタスクが異常終了しました", msg_error);
                            }
                            */////////////////////////////////////////////////////
                        }
                        else // INファイル以外はタイムアウト監視：タイムアウトファイルはエラーフォルダに移動
                        {
                            Task<bool> tmout = Task.Run(() =>
                            {
                                return TimeOutTask(file, fs);
                            });
                        }
                    }
                }
                
                TaskTimer.Enabled = true;
            }
            else
            {
                TaskTimer.Enabled = true;
            }

        }


        //
        /* Task1関数
         * 登録したファイル検出キー毎に検索してList化
         * 
         */
        //
        private void Task1(ref List<string> Files, string KeySearc)
        {
            try
            {
                if (serverOn)
                {
                    DirSearch(mci.MCDir, KeySearc, ref Files);
                }
            }
            catch (Exception e)
            {
                ConsoleShow(e.Message, msg_error);
            } 
        }


        //
        /* Task2関数
         * 設備ごとのインターロック管理を実施
         * 
         */
        //
        private bool Task2(mcfilesys fs)
        {
            string[] FOTaskRslt = new string[3];

            ////////////////////////////////
            // 設備のインターロック処理  //
            IntLokMac.Add(fs.Macno);    //
            /////////////////////////////
            ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを設置しました", msg_debug);

            try
            {
                if (!InFileTaskCont(fs))
                {
                    throw new Exception();
                }

                // Task終了時ファイル消去・移動の遅れ対策ディレイ
                Thread.Sleep(500);
                ///////////////////////////////////////////////////////////
                // 設備のインターロック解除（同期の場合コメントアウト） //
                IntLokMac.RemoveAll(s => s.Contains(fs.Macno));        //
                //TaskTimer.Enabled = true; //同期の場合は必要        //
                ///////////////////////////////////////////////////////
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを解除しました", msg_debug);
                //
                return true;
            }
            catch
            {
                // Task終了時ファイル消去・移動の遅れ対策ディレイ
                Thread.Sleep(500);
                ///////////////////////////////////////////////////////////
                // 設備のインターロック解除（同期の場合コメントアウト） //
                IntLokMac.RemoveAll(s => s.Contains(fs.Macno));        //
                //TaskTimer.Enabled = true; //同期の場合は必要        //
                ///////////////////////////////////////////////////////
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")/ MgNo:" + fs.MagCupNo + "のタスクが異常終了しました", msg_error);
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを解除しました", msg_debug);
                //
                return false;
            }
        }

        //
        /* Task2の実作業関数
         * 検出されたファイルは検出キーに沿って処理
         * 
         */
        //
        private bool InFileTaskCont(mcfilesys fs)
        {
            try
            {
                string[] DBTaskRslt = new string[4];
                string[] FOTaskRslt = new string[4];

                Tasks_MagCup Tsk = new Tasks_MagCup();

                if (fs.FindFold == "in") //【ファイル検出場所】がINフォルダの場合（正常）
                {
                    ///////////////////////////////////////////////
                    // タスク処理ルーター
                    ///////////////////////////////////////////////
                    if (!DBTaskRsltRouter(fs, Tsk, ref DBTaskRslt))
                    {
                        return false;
                    }

                    ///////////////////////////////////////////////
                    // タスク結果処理
                    ///////////////////////////////////////////////
                    if (DBTaskRslt[0] == "OK") //【DBタスク】が正常完了の場合
                    {
                        if (!DBTaskRsltIsOK(fs, Tsk, DBTaskRslt, ref FOTaskRslt))
                        {
                            return false;
                        }
                    }
                    else if (DBTaskRslt[0] == "NG") //【DBタスク】が正常でない場合
                    {
                        if (!DBTaskRsltIsNG(fs, Tsk, DBTaskRslt, ref FOTaskRslt))
                        {
                            return false;
                        }
                    }
                    else if (DBTaskRslt[0] == "Cancel") //【DBタスク】中止する場合
                    {
                        if (!DBTaskRsltIsCancel(fs, Tsk, DBTaskRslt, ref FOTaskRslt))
                        {
                            return false;
                        }
                    }

                    ///////////////////////////////////////////////
                    // 異常終了時処理
                    // return falseなし（でよいの？）
                    ///////////////////////////////////////////////
                    // 【OUTフォルダタスク】が異常終了の場合
                    if (FOTaskRslt[0] == "NG")
                    {
                        ConsoleShow(DBTaskRslt[2], msg_debug);
                        ConsoleShow(FOTaskRslt[1], msg_error);
                        ConsoleShow("サーバー動作異常：エラーが発生しています、管理者に報告してください", msg_error);
                        string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                        //
                        //inファイルをエラーフォルダに移動
                        //
                        //[temp]=>[error]
                        string[] mef = tcommons.MoveErrorFile(fs, fs.tmpfilepath, ErrorPath);
                        if (mef[0] != "NA") ConsoleShow(mef[0], int.Parse(mef[1]));
                        //[in]=>[error]
                        mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
                        if (mef[0] != "NA") ConsoleShow(mef[0], int.Parse(mef[1]));
                    }
                }
                else //【ファイル検出場所】がINフォルダ以外（異常）
                {
                    ConsoleShow(fs.MagCupNo + "のINフォルダ関連のファイルがINフォルダ以外で検出：" + fs.filepath, msg_alarm);
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }


        private bool DBTaskRsltRouter(mcfilesys fs, Tasks_MagCup Tsk, ref string[] DBTaskRslt)
        {
            try
            {
                ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} のタスク({fs.keylbl})処理開始", msg_task);

                switch (fs.key)
                {
                    case "_min1.csv":
                        fs.mclbl = "MagNo";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Min1.DBTasks(fs);
                        break;
                    case "_min2.csv":
                        fs.mclbl = "MagNo";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Min2.DBTasks(fs);
                        break;
                    case "_mot.csv":
                        fs.mclbl = "MagNo";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Mout.DBTasks(fs);
                        break;
                    case "_rcp.txt":
                        fs.mclbl = "Recipe";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Crcp.DBTasks(fs);
                        break;
                    case "_sta.csv":
                        fs.mclbl = "Recipe";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Csta.DBTasks(fs);
                        break;
                    case "_cot1.csv":
                        fs.mclbl = "Cup";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Cot1.DBTasks(fs);
                        break;
                    case "_cot2.csv":
                        fs.mclbl = "Cup";
                        fs.lbl = new string[] { fs.mclbl, fs.keylbl };
                        DBTaskRslt = Tsk.Cot2.DBTasks(fs);
                        break;
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool DBTaskRsltIsOK(mcfilesys fs, Tasks_MagCup Tsk, string[] DBTaskRslt, ref string[] FOTaskRslt)
        {
            try
            {
                switch (fs.key)
                {
                    case "_min1.csv":
                        FOTaskRslt = Tsk.Min1.FOutTasks(fs, 0);
                        break;
                    case "_min2.csv":
                        FOTaskRslt = Tsk.Min2.FOutTasks(fs, 0);
                        break;
                    case "_mot.csv":
                        FOTaskRslt = Tsk.Mout.FOutTasks(fs, 0);
                        break;
                    case "_rcp.txt":
                        FOTaskRslt = Tsk.Crcp.FOutTasks(fs, 0);
                        break;
                    case "_sta.csv":
                        FOTaskRslt = Tsk.Csta.FOutTasks(fs, 0);
                        break;
                    case "_cot1.csv":
                        FOTaskRslt = Tsk.Cot1.FOutTasks(fs, 0);
                        break;
                    case "_cot2.csv":
                        FOTaskRslt = Tsk.Cot2.FOutTasks(fs, 0);
                        break;
                }

                if (FOTaskRslt[0] == "OK") //【OUTフォルダタスク】が正常終了の場合
                {
                    ConsoleShow(DBTaskRslt[2], msg_debug);
                    if (DBTaskRslt[1] != "") ConsoleShow(DBTaskRslt[1], msg_task);
                    //
                    //inファイルの消去
                    //
                    File.Delete(fs.tmpfilepath);

                    ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} のタスクは正常に完了しました", msg_task);
                    ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルを削除しました", msg_task);
                }
                // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool DBTaskRsltIsNG(mcfilesys fs, Tasks_MagCup Tsk, string[] DBTaskRslt, ref string[] FOTaskRslt)
        {
            try
            {
                //【OUTフォルダタスク】マガジン情報出力
                switch (fs.key)
                {
                    case "_min1.csv":
                        FOTaskRslt = Tsk.Min1.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                    case "_min2.csv":
                        FOTaskRslt = Tsk.Min2.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                    case "_mot.csv":
                        FOTaskRslt = Tsk.Mout.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                    case "_rcp.txt":
                        FOTaskRslt = Tsk.Crcp.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                    case "_sta.csv":
                        FOTaskRslt = Tsk.Csta.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                    case "_cot1.csv":
                        FOTaskRslt = Tsk.Cot1.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                    case "_cot2.csv":
                        FOTaskRslt = Tsk.Cot2.FOutTasks(fs, int.Parse(DBTaskRslt[3]));
                        break;
                }

                if (FOTaskRslt[0] == "OK") //【OUTフォルダタスク】が正常終了の場合
                {
                    ConsoleShow(DBTaskRslt[2], msg_debug);
                    ConsoleShow(DBTaskRslt[1], msg_error);
                    string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                    //
                    //inファイルをエラーフォルダに移動
                    //
                    //[temp]=>[error]
                    string[] mef = tcommons.MoveErrorFile(fs, fs.tmpfilepath, ErrorPath);
                    if (mef[0] != "NA") ConsoleShow(mef[0], int.Parse(mef[1]));
                    //[in]=>[error]
                    mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
                    if (mef[0] != "NA") ConsoleShow(mef[0], int.Parse(mef[1]));
                }
                // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool DBTaskRsltIsCancel(mcfilesys fs, Tasks_MagCup Tsk, string[] DBTaskRslt, ref string[] FOTaskRslt)
        {
            try
            {
                //////////////////////////////////////////////////////////////////////////////////////
                // 設備側からタスクのキャンセル要求(ERROR返信)があった場合はMagCupDirをクリーンします
                // 但し、レシピについては
                //  　①キャンセル返信が必要 ②INが同時に複数あり得る
                // ことからクリーンはIN/OUTを除外します。
                //
                // =>クリーンの方法は別途検討したほうがいいと思われます！！！
                //
                //////////////////////////////////////////////////////////////////////////////////////
                //【OUTフォルダタスク】マガジン情報出力

                switch (fs.key)
                {
                    case "_sta.csv":
                        FOTaskRslt = Tsk.Csta.FOutTasks(fs, 999);
                        break;
                }
                var fldn = new List<string> { "wip", "temp", "in", "out" };
                cleanfiles(fs, fldn);
                ConsoleShow(DBTaskRslt[2], msg_debug);
                if (DBTaskRslt[1] != "") ConsoleShow(DBTaskRslt[1], msg_task);

                if (FOTaskRslt[0] == "OK") FOTaskRslt[0] = "Cancel";
                // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

                return true;
            }
            catch
            {
                return false;
            }
        }


        private bool TimeOutTask(string filepath, mcfilesys fs)
        {
            try
            {
                string[] DBTaskRslt = new string[4];
                string[] FOTaskRslt = new string[4];

                TimeSpan timeout = new TimeSpan(0, 0, mci.outfiletimeout);
                DateTime ts = System.IO.File.GetLastWriteTime(filepath); //削除後すぐに同名ファイルが書き込まれると上書きになる場合があるみたい
                DateTime dt = DateTime.Now;
                
                if((dt - ts - timeout)> new TimeSpan(0, 0, 0))
                {
                    ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}はタイムスタンプ[" + ts.ToString("yyyy-MM-dd HH:mm:ss") + "]にてタイムアウトとしました", msg_alarm);
                    string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                    //
                    //inファイルをエラーフォルダに移動
                    //
                    if (commons.MoveFile(filepath, ErrorPath))
                    {
                        ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}はエラーフォルダに移動しました", msg_task);
                    }
                    else
                    {
                        ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}ファイルはエラーフォルダに移動できません", msg_alarm);
                    }
                }

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }


        //
        /* 設備フォルダのクリーン
        */
        //
        private void cleanflders(mcfilesys fs, List<string> fldn)
        {
            if (tcommons.CleanMagCupDir(fs, fldn))
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")のワークスペースをクリーンしました", msg_alarm);
            else
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")のワークスペースをクリーンできません", msg_error);
        }

        //
        /* 設備フォルダのクリーン
        */
        //
        private void cleanfiles(mcfilesys fs, List<string> fldn)
        {
            if (tcommons.CleanMagCupfiles(fs, fldn))
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")の対象ファイルをクリーンしました", msg_alarm);
            else
                ConsoleShow("設備:" + fs.Pcat + "(" + fs.Macno + ")の対象ファイルをクリーンできません", msg_error);
        }

        //
        /* メニューからmagcup.iniを開く
        */
        //
        private void magcupiniToolStripMenuItem_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(@"C:\magcup\magcup.ini");
        }


        //mqtt test code
        //
        private void MqttTimer_Tick(object sender, EventArgs e)
        {
            //サーバーステータスのパブリッシュ
            //mq.MqttServerStatus(mci.mosquittoHost, "test", serverOn);
        }


        private void pLC接続テストToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var ptf = new PlcTestForm();
            ptf.ShowDialog();
            ptf.Dispose();
        }


        //
        /* 検出ファイル文字列から検出されたキー(String)を返します
         * 検出されない場合は "None"を返します
        */
        //
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


        //
        /* 検出ファイル文字列から検出されたキー(String)を返します
         * 検出されない場合は "None"を返します
        */
        //
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

        private bool Macconfjson2fs(mcfilesys fs, string MacConfPath)
        {
            try
            {
                bool findmcplc = false, findmcpc = false;
                fs.mconf = JsonConvert.DeserializeObject<macconfjson>(commons.JsonFileReader(MacConfPath));
                // 
                // mconfから検出されたmcfileの設定を抜く
                //
                for (int i = 0; i < fs.mconf.Mcfs.mcfconfs.Count; i++)
                {
                    if (fs.key.Contains(fs.mconf.Mcfs[i].mcfilekey))
                    {
                        fs.mcfc = fs.mconf.Mcfs[i];
                        ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のファイル設定確認", msg_debug);
                    }
                }
                //
                // mcfile設定からコントローラ情報を抜く
                //
                if (fs.mcfc.foi.cnttype == "PLC")
                {
                    for (int i = 0; i < fs.mconf.Plcs.plcconfs.Count; i++)
                    {
                        if (fs.mconf.Plcs[i].name == fs.mcfc.foi.cntid) // MagCupで使用するPLCの検出
                        {
                            fs.plcc = fs.mconf.Plcs[i];
                            ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPLCを確認しました", msg_debug);
                            findmcplc = true;
                            break;
                        }
                    }
                    if (!findmcplc)
                    {
                        ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPLC設定が不正です", msg_error);
                        return false;
                    }

                }
                else if (fs.mcfc.foi.cnttype == "PC")
                {
                    for (int i = 0; i < fs.mconf.Pcs.pcconfs.Count; i++)
                    {
                        if (fs.mconf.Pcs[i].name == fs.mcfc.foi.cntid) // MagCupで使用するPCの検出
                        {
                            fs.pcc = fs.mconf.Pcs[i];
                            ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPCを確認しました", msg_debug);
                            findmcpc = true;
                            break;
                        }
                    }
                    if (!findmcpc)
                    {
                        ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPC設定が不正です", msg_error);
                        return false;
                    }
                }
                else
                {
                    ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のコントローラ設定が不正です", msg_error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                ConsoleShow($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>" + ex.ToString(), msg_debug);
                return false;
            }
            
        }

    }

}
