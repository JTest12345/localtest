using System;
using System.IO;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using Oskas;


namespace FileIf
{
    class TaskControlHandler
    {

        Tasks_Common tcommons;
        Magcupini mci;
        List<string> IntLokMac;

        public TaskControlHandler(Tasks_Common tc, Magcupini mc, List<string> IntL)
        {
            tcommons = tc;
            mci = mc;
            IntLokMac = IntL;
        }
        //
        /* タスク関数【Task0】
         * 【Task1】
         * 　登録したファイル検出キー毎に検索してList化
         * 【Task2】
         * 　検出されたファイルは検出キーに沿って処理
         * 
         */
        //
        public void Tasks(string TaskCat, string[] filekey)
        {
            try
            {
                List<string> Files = new List<string>();
                foreach (string key in filekey)
                {
                    Task1(ref Files, key);
                }
                if (Files.Count() != 0)
                {
                    OskNLog.Log(Files.Count() + "件の" + TaskCat + "フォルダ関連のファイルを検出しました", Cnslcnf.msg_info);
                    foreach (string file in Files)
                    {
                        /////////////////////////////////
                        // filesysクラスの実体化
                        //  
                        //
                        ////////////////////////////////
                        Mcfilesys fs = new Mcfilesys();

                        fs.filepath = file; //設備タスク対象ファイル名
                        fs.lowerfilepath = file.ToLower(); //対象ファイル小文字
                        fs.Upperfilepath = file.ToUpper(); //対象ファイル大文字
                        fs.tmpfilepath = fs.filepath.Replace(@"\in\", @"\temp\"); //TMPフォルダパス
                        fs.key = Tasks_Common.FindKey(mci, fs.lowerfilepath); //ファイル名から抽出したタスクキー(_min1.csvなど)
                        fs.ff = fs.filepath.Split(System.IO.Path.DirectorySeparatorChar); //ファイル名のディレクトリ分割
                        int indexofmagcup = Array.IndexOf(fs.ff, "magcupdir", 0);
                        fs.Pcat = fs.ff[indexofmagcup + 1]; //設備カテゴリ
                        fs.Macno = fs.ff[indexofmagcup + 2]; //設備No
                        fs.FindFold = fs.ff[indexofmagcup + 3]; //ファイルが検出されたワークフォルダ
                        fs.MagCupNo = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), ""); //ファイル名（拡張子なし:MagCup)
                        fs.FileNameKey = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), ""); //ファイル名（拡張子なし:FileIF)
                        fs.RecipeFile = Path.GetFileName(fs.Upperfilepath).Replace(fs.key.ToUpper(), "");  //レシピ名（拡張子なし）
                        fs.fpath = $"{mci.MCDir}\\{fs.Pcat}\\{fs.Macno}"; //設備フォルダパス（ルート）
                        fs.keylbl = Tasks_Common.FindKeyAlt(mci, fs.lowerfilepath); // タスクキーラベル(min1など)
                        fs.mclbl = ""; //タスク種別のラベル（タスク内で代入）
                        string[] extstr = fs.lowerfilepath.Split('.');
                        fs.ext = extstr[1];

                        //fs.ConnectionStrings = mci.ConnectionStrings; //DB接続文字配列
                        //fs.WipDir = mci.WipDir; // WIPひな形フォルダ（現在未使用）
                        //fs.RecipUpLoadDir = mci.RecipUpLoadDir; //レシピバックアップをアップロード先
                        //fs.UsePlcTrig = mci.UsePlcTrig; //【デバック用設定】PLCを使用しない
                        fs.mci = mci; //iniクラスの取り込み

                        if (!IntLokMac.Contains(fs.Macno))
                        {
                            if (TaskCat == "IN") // INファイルはTask2を実施する
                            {
                                /*////////////////////////////////////////////////////
                                // 非同期処理はコメントアウト
                                // 設備のインターロック処理
                                //IntLokMac.Add(fs.Macno);
                                */////////////////////////////////////////////////////

                                //ConfigJson読込関数
                                string MacFld = mci.MCDir + "\\" + fs.Pcat + "\\" + fs.Macno;
                                string MacConfPath = MacFld + "\\conf\\macconf.json";
                                if (!Macconfjson2fs(fs, MacConfPath))
                                {
                                    OskNLog.Log("設備設定ファイル(macconf.json)の条件またはkeyに異常があります", Cnslcnf.msg_error);
                                    string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                                    string[] mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
                                    if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
                                    break; //foreachを抜ける
                                }
                                fs.MacFld = MacFld;
                                fs.MacConfPath = MacConfPath;

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
                                    IntLokMac.RemoveAll(s => s.Contains(fs.Macno));
                                    TaskTimer.Enabled = true;
                                }
                                else
                                {
                                    // 設備のインターロック解除
                                    IntLokMac.RemoveAll(s => s.Contains(fs.Macno));
                                    TaskTimer.Enabled = true;
                                    OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")/ MgNo:" + MagCupNo + "のタスクが異常終了しました", msg_error);
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
                }
            }
            catch (Exception ex)
            {
                OskNLog.Log("【Task0】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
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
                DirSearch(mci.MCDir, KeySearc, ref Files);
            }
            catch (Exception e)
            {
                OskNLog.Log("【Task1】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(e.Message, Cnslcnf.msg_error);
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


        //
        /* Task2関数
         * 設備ごとのインターロック管理を実施
         * 
         */
        //
        private bool Task2(Mcfilesys fs)
        {
            string[] OutFileTask = new string[3];

            ////////////////////////////////
            // 設備のインターロック処理  //
            IntLokMac.Add(fs.Macno);    //
            /////////////////////////////
            OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを設置しました", Cnslcnf.msg_debug);

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
                OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを解除しました", Cnslcnf.msg_debug);
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
                OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")/ MgNo:" + fs.MagCupNo + "のタスクが異常終了しました", Cnslcnf.msg_error);
                OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")のインターロックを解除しました", Cnslcnf.msg_debug);
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
        private bool InFileTaskCont(Mcfilesys fs)
        {
            try
            {
                string[] InFileTaskRslt = new string[4];
                string[] OutFileTask = new string[4];

                //Tasks_MagCup Tsk = new Tasks_MagCup();

                if (fs.FindFold == "in") //【ファイル検出場所】がINフォルダの場合（正常）
                {
                    /////////////////////////////////////////////////
                    //// タスク処理(Router)
                    ///  旧処理：下記のDBタスク処理に移行
                    /////////////////////////////////////////////////
                    //if (!InFileTaskRsltRouter(fs, TaskClass, InFileTasks, ref InFileTaskRslt))
                    //{
                    //    return false;
                    //}

                    /////////////////////////////////////////////////
                    //// タスク処理
                    /////////////////////////////////////////////////
                    // ◆keyfile対象Classのコンストラクターを取得
                    Type iTaskType = Type.GetType("FileIf.Tasks_" + fs.keylbl);
                    //Type[] types = new Type[1];
                    //types[0] = typeof(string);
                    //classの用意がない場合
                    if (iTaskType == null)
                    {
                        var moveto = fs.filepath.Replace("in", "error") + DateTime.Now.ToString("yyyyMMddHHmmss") + ".err";
                        File.Move(fs.filepath, moveto);
                        OskNLog.Log("検出されたファイルのクラス定義がみつかりません。ファイルはエラーフォルダに移動します。", Cnslcnf.msg_error);
                    }

                    Type[] types = new Type[0];
                    ConstructorInfo magicConstructor = iTaskType.GetConstructor(types);

                    // ◆keyfile対象クラスを実体化
                    object TaskClass = magicConstructor.Invoke(new object[] { });

                    // ◆keyfile対象クラスのメソッドを抽出
                    MethodInfo InFileTasks = iTaskType.GetMethod("InFileTasks");
                    MethodInfo OutFileTasks = iTaskType.GetMethod("OutFileTasks");

                    // ◆InFileTask実行
                    object InFileTaskRsltArr = InFileTasks.Invoke(TaskClass, new object[] { fs });
                    InFileTaskRslt = (string[])InFileTaskRsltArr;
                    


                    ///////////////////////////////////////////////
                    // タスク結果処理
                    ///////////////////////////////////////////////
                    if (InFileTaskRslt[0] == "OK") //【DBタスク】が正常完了の場合
                    {
                        // ◆FOutTask実行
                        object FOutTaskRsltArr = OutFileTasks.Invoke(TaskClass, new object[] { fs, 0 });
                        OutFileTask = (string[])FOutTaskRsltArr;

                        //if (!InFileTaskRsltIsOK(fs, Tsk, InFileTaskRslt, ref OutFileTask))
                        if (!InFileTaskRsltIsOK(fs, InFileTaskRslt, ref OutFileTask))
                        {
                            return false;
                        }
                    }
                    else if (InFileTaskRslt[0] == "NG") //【DBタスク】が正常でない場合
                    {
                        // ◆FOutTask実行(エラーコード有)
                        object FOutTaskRsltArr = OutFileTasks.Invoke(TaskClass, new object[] { fs, int.Parse(InFileTaskRslt[3]) });
                        OutFileTask = (string[])FOutTaskRsltArr;

                        //if (!InFileTaskRsltIsNG(fs, Tsk, InFileTaskRslt, ref OutFileTask))
                        if (!InFileTaskRsltIsNG(fs, InFileTaskRslt, ref OutFileTask))
                        {
                            return false;
                        }
                    }
                    else if (InFileTaskRslt[0] == "Cancel") //【DBタスク】中止する場合
                    {
                        // ◆FOutTask実行(エラーコード999)
                        object FOutTaskRsltArr = OutFileTasks.Invoke(TaskClass, new object[] { fs, 999 });
                        OutFileTask = (string[])FOutTaskRsltArr;

                        //if (!InFileTaskRsltIsCancel(fs, Tsk, InFileTaskRslt, ref OutFileTask))
                        if (!InFileTaskRsltIsCancel(fs, InFileTaskRslt, ref OutFileTask))
                        {
                            return false;
                        }

                    }

                    ///////////////////////////////////////////////
                    // 異常終了時処理
                    // return falseなし（でよいの？）
                    ///////////////////////////////////////////////
                    // 【OUTフォルダタスク】が異常終了の場合
                    if (OutFileTask[0] == "NG")
                    {
                        OskNLog.Log(InFileTaskRslt[2], Cnslcnf.msg_debug);
                        OskNLog.Log(OutFileTask[1], Cnslcnf.msg_error);
                        OskNLog.Log("FILEIF動作異常：エラーが発生しています、管理者に報告してください", Cnslcnf.msg_error);
                        string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                        //
                        //inファイルをエラーフォルダに移動
                        //
                        //[temp]=>[error]
                        string[] mef = tcommons.MoveErrorFile(fs, fs.tmpfilepath, ErrorPath);
                        if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
                        //[in]=>[error]
                        mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
                        if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
                    }
                }
                else //【ファイル検出場所】がINフォルダ以外（異常）
                {
                    OskNLog.Log(fs.MagCupNo + "のINフォルダ関連のファイルがINフォルダ以外で検出：" + fs.filepath, Cnslcnf.msg_alarm);
                }

                return true;
            }
            catch (Exception ex)
            {
                OskNLog.Log("【Task2】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
                return false;
            }
        }


        //private bool InFileTaskRsltRouter(Mcfilesys fs, Tasks_MagCup Tsk, ref string[] InFileTaskRslt)
        //{
        //    try
        //    {
        //        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} のタスク({fs.keylbl})処理開始", Cnslcnf.msg_info);

        //        switch (fs.key)
        //        {
        //            case "_min1.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Min1.InFileTasks(fs);
        //                break;
        //            case "_min2.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Min2.InFileTasks(fs);
        //                break;
        //            case "_mot.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Mout.InFileTasks(fs);
        //                break;
        //            case "_mio.csv":
        //                fs.mclbl = "MagNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Mio.InFileTasks(fs);
        //                break;
        //            case "_vlin1.csv":
        //                fs.mclbl = "VlotNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Vlin1.InFileTasks(fs);
        //                break;
        //            case "_vlin2.csv":
        //                fs.mclbl = "VlotNo";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Vlin2.InFileTasks(fs);
        //                break;
        //            case "_bto.csv":
        //                fs.mclbl = "MagNo[1]";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Bto.InFileTasks(fs);
        //                break;
        //            case "_rcp.txt":
        //                fs.mclbl = "Recipe";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Crcp.InFileTasks(fs);
        //                break;
        //            case "_sta.csv":
        //                fs.mclbl = "Recipe";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Csta.InFileTasks(fs);
        //                break;
        //            case "_cot1.csv":
        //                fs.mclbl = "Cup";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Cot1.InFileTasks(fs);
        //                break;
        //            case "_cot2.csv":
        //                fs.mclbl = "Cup";
        //                fs.lbl = new string[] { fs.mclbl, fs.keylbl };
        //                InFileTaskRslt = Tsk.Cot2.InFileTasks(fs);
        //                break;
        //        }

        //        return true;
        //    }
        //    catch (Exception ex)
        //    {
        //        OskNLog.Log("【InFileTaskRsltRouter】実行中にExceptionが発生しました", Cnslcnf.msg_error);
        //        OskNLog.Log(ex.Message, Cnslcnf.msg_error);
        //        return false;
        //    }
        //}


        //private bool InFileTaskRsltIsOK(Mcfilesys fs, Tasks_MagCup Tsk, string[] InFileTaskRslt, ref string[] OutFileTask)
        private bool InFileTaskRsltIsOK(Mcfilesys fs, string[] InFileTaskRslt, ref string[] OutFileTask)
        {
            try
            {
                //switch (fs.key)
                //{
                //    case "_min1.csv":
                //        OutFileTask = Tsk.Min1.OutFileTasks(fs, 0);
                //        break;
                //    case "_min2.csv":
                //        OutFileTask = Tsk.Min2.OutFileTasks(fs, 0);
                //        break;
                //    case "_mot.csv":
                //        OutFileTask = Tsk.Mout.OutFileTasks(fs, 0);
                //        break;
                //    case "_mio.csv":
                //        OutFileTask = Tsk.Mio.OutFileTasks(fs, 0);
                //        break;
                //    case "_vlin1.csv":
                //        OutFileTask = Tsk.Vlin1.OutFileTasks(fs, 0);
                //        break;
                //    case "_vlin2.csv":
                //        OutFileTask = Tsk.Vlin2.OutFileTasks(fs, 0);
                //        break;
                //    case "_bto.csv":
                //        OutFileTask = Tsk.Bto.OutFileTasks(fs, 0);
                //        break;
                //    case "_rcp.txt":
                //        OutFileTask = Tsk.Crcp.OutFileTasks(fs, 0);
                //        break;
                //    case "_sta.csv":
                //        OutFileTask = Tsk.Csta.OutFileTasks(fs, 0);
                //        break;
                //    case "_cot1.csv":
                //        OutFileTask = Tsk.Cot1.OutFileTasks(fs, 0);
                //        break;
                //    case "_cot2.csv":
                //        OutFileTask = Tsk.Cot2.OutFileTasks(fs, 0);
                //        break;
                //}

                if (OutFileTask[0] == "OK") //【OUTフォルダタスク】が正常終了の場合
                {
                    OskNLog.Log(InFileTaskRslt[2], Cnslcnf.msg_debug);
                    if (InFileTaskRslt[1] != "") OskNLog.Log(InFileTaskRslt[1], Cnslcnf.msg_info);
                    //
                    //inファイルの消去
                    //
                    File.Delete(fs.tmpfilepath);

                    OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} のタスクは正常に完了しました", Cnslcnf.msg_info);
                    OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルを削除しました", Cnslcnf.msg_info);
                }
                // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

                return true;
            }
            catch (Exception ex)
            {
                OskNLog.Log("【InFileTaskRsltIsOK】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
                return false;
            }
        }


        //private bool InFileTaskRsltIsNG(Mcfilesys fs, Tasks_MagCup Tsk, string[] InFileTaskRslt, ref string[] OutFileTask)
        private bool InFileTaskRsltIsNG(Mcfilesys fs, string[] InFileTaskRslt, ref string[] OutFileTask)
        {
            try
            {
                ////【OUTフォルダタスク】マガジン情報出力
                //switch (fs.key)
                //{
                //    // MAGファイル：通常工程開始①
                //    case "_min1.csv":
                //        OutFileTask = Tsk.Min1.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // MAGファイル：通常工程開始②
                //    case "_min2.csv":
                //        OutFileTask = Tsk.Min2.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // MAGファイル：通常工程完了
                //    case "_mot.csv":
                //        OutFileTask = Tsk.Mout.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // MAGファイル：通常工程開始完了一括
                //    case "_mio.csv":
                //        OutFileTask = Tsk.Mio.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // MAGファイル：V溝バリ取り工程開始①
                //    case "_vlin1.csv":
                //        OutFileTask = Tsk.Vlin1.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // MAGファイル：V溝バリ取り工程開始②
                //    case "_vlin2.csv":
                //        OutFileTask = Tsk.Vlin2.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // MAGファイル：レーザーダイサーのブレンド完了処理
                //    case "_bto.csv":
                //        OutFileTask = Tsk.Bto.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // CUPファイル：樹脂配合レシピ受付
                //    case "_rcp.txt":
                //        OutFileTask = Tsk.Crcp.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // CUPファイル：樹脂配合レシピ開始
                //    case "_sta.csv":
                //        OutFileTask = Tsk.Csta.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // CUPファイル：樹脂配合材料配合完了
                //    case "_cot1.csv":
                //        OutFileTask = Tsk.Cot1.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //    // CUPファイル：樹脂配合撹拌完了
                //    case "_cot2.csv":
                //        OutFileTask = Tsk.Cot2.OutFileTasks(fs, int.Parse(InFileTaskRslt[3]));
                //        break;
                //}

                if (OutFileTask[0] == "OK") //【OUTフォルダタスク】が正常終了の場合
                {
                    OskNLog.Log(InFileTaskRslt[2], Cnslcnf.msg_debug);
                    OskNLog.Log(InFileTaskRslt[1], Cnslcnf.msg_error);
                    string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                    //
                    //inファイルをエラーフォルダに移動
                    //
                    //[temp]=>[error]
                    string[] mef = tcommons.MoveErrorFile(fs, fs.tmpfilepath, ErrorPath);
                    if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
                    //[in]=>[error]
                    mef = tcommons.MoveErrorFile(fs, fs.filepath, ErrorPath);
                    if (mef[0] != "NA") OskNLog.Log(mef[0], int.Parse(mef[1]));
                }
                // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

                return true;
            }
            catch (Exception ex)
            {
                OskNLog.Log("【InFileTaskRsltIsNG】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
                return false;
            }
        }


        //private bool InFileTaskRsltIsCancel(Mcfilesys fs, Tasks_MagCup Tsk, string[] InFileTaskRslt, ref string[] OutFileTask)
        private bool InFileTaskRsltIsCancel(Mcfilesys fs, string[] InFileTaskRslt, ref string[] OutFileTask)
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

                var fldn = new List<string> { "wip", "temp", "in", "out" };
                cleanfiles(fs, fldn);
                OskNLog.Log(InFileTaskRslt[2], Cnslcnf.msg_debug);
                if (InFileTaskRslt[1] != "") OskNLog.Log(InFileTaskRslt[1], Cnslcnf.msg_info);

                if (OutFileTask[0] == "OK") OutFileTask[0] = "Cancel";
                // OUTファイルNGの場合の処理は「【OUTフォルダタスク】が異常終了の場合」で共通処理

                return true;
            }
            catch (Exception ex)
            {
                OskNLog.Log("【InFileTaskRsltIsCancel】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
                return false;
            }
        }


        private bool TimeOutTask(string filepath, Mcfilesys fs)
        {
            try
            {
                string[] InFileTaskRslt = new string[4];
                string[] OutFileTask = new string[4];

                TimeSpan timeout = new TimeSpan(0, 0, mci.outfiletimeout);
                DateTime ts = System.IO.File.GetLastWriteTime(filepath); //削除後すぐに同名ファイルが書き込まれると上書きになる場合があるみたい
                DateTime dt = DateTime.Now;

                if ((dt - ts - timeout) > new TimeSpan(0, 0, 0))
                {
                    OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}はタイムスタンプ[" + ts.ToString("yyyy-MM-dd HH:mm:ss") + "]にてタイムアウトとしました", Cnslcnf.msg_alarm);
                    string ErrorPath = $"{fs.fpath}\\error\\{fs.MagCupNo}_{fs.keylbl}_{fs.Pcat}_{fs.Macno}_{DateTime.Now.ToString("yyyyMMddHHmmss")}.err";
                    //
                    //inファイルをエラーフォルダに移動
                    //
                    if (CommonFuncs.MoveFile(filepath, ErrorPath))
                    {
                        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}はエラーフォルダに移動しました", Cnslcnf.msg_info);
                    }
                    else
                    {
                        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}ファイルはエラーフォルダに移動できません", Cnslcnf.msg_alarm);
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                OskNLog.Log("【TimeOutTask】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
                return false;
            }
        }



        public static bool Macconfjson2fs(Mcfilesys fs, string MacConfPath)
        {
            try
            {
                bool findmcplc = false, findmcpc = false;
                fs.mconf = JsonConvert.DeserializeObject<macconfjson>(CommonFuncs.JsonFileReader(MacConfPath));
                // 
                // mconfから検出されたmcfileの設定を抜く
                //
                for (int i = 0; i < fs.mconf.Mcfs.mcfconfs.Count; i++)
                {
                    if (fs.key.Contains(fs.mconf.Mcfs[i].mcfilekey))
                    {
                        fs.mcfc = fs.mconf.Mcfs[i];
                        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のファイル設定確認", Cnslcnf.msg_debug);
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
                            OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPLCを確認しました", Cnslcnf.msg_debug);
                            findmcplc = true;
                            break;
                        }
                    }
                    if (!findmcplc)
                    {
                        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPLC設定が不正です", Cnslcnf.msg_error);
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
                            OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPCを確認しました", Cnslcnf.msg_debug);
                            findmcpc = true;
                            break;
                        }
                    }
                    if (!findmcpc)
                    {
                        OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のPC設定が不正です", Cnslcnf.msg_error);
                        return false;
                    }
                }
                else
                {
                    OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>MagCup用のコントローラ設定、またはkeyが不正です", Cnslcnf.msg_error);
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                OskNLog.Log("【Macconfjson2fs】実行中にExceptionが発生しました", Cnslcnf.msg_error);
                OskNLog.Log($"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo}{fs.keylbl}>>" + ex.ToString(), Cnslcnf.msg_debug);
                return false;
            }

        }

        private void cleanfiles(Mcfilesys fs, List<string> fldn)
        {
            if (tcommons.CleanMagCupfiles(fs, fldn))
                OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")の対象ファイルをクリーンしました", Cnslcnf.msg_alarm);
            else
                OskNLog.Log("設備:" + fs.Pcat + "(" + fs.Macno + ")の対象ファイルをクリーンできません", Cnslcnf.msg_error);
        }

    }
}
