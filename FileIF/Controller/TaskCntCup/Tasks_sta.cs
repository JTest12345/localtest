using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{
    class Tasks_sta : Tasks_base
    {
        //ファイルクラス
        TaskFile_sta sta;

        //本タスクにて使用するDBテーブル
        recipe_info rrinfo;

        //Endファイル用変数格納用辞書
        //Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_sta()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            sta = new TaskFile_sta();
            rrinfo = new recipe_info();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
            //retDict = new Dictionary<string, string>(Dict);
        }

        // データベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Sta処理";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=sta101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=sta102> staファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                Task_Ret rsf =sta.ReadStaFileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rsf.Result == retkey.ng || rsf.Result == retkey.cancel)
                {
                    return rsf;
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=sta103>【DB登録】recipe_infoテーブルにレシピ開始情報追加
            try
            {
                taskid += 1;
                List<string> QueryScr = new List<string>();

                //DBにレシピ情報追加
                DateTime dt = DateTime.Now;
                string datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                rrinfo.Getsta = 1;
                rrinfo.Dtsta = datetime;

                if (rrinfo.Getsta == 0 || rrinfo.Dtsta == "")
                {
                    string mes = "レシピのDB登録に必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }


                ///////////////////////////////////////////////////
                // DB先はPMMSとなったため下記コードはコメントアウト
                //
                //QueryScr.Add($"UPDATE recipe_info SET get_sta={rrinfo.Getsta}, datetime_sta='{rrinfo.Dtsta}' WHERE recipefile_name='{fs.RecipeFile}'");
                //Dbgmsg += "Query1: " + QueryScr[0] + crlf;

                //// QueryScrリストからSQL文を実行
                //foreach (string quer in QueryScr)
                //{
                //    if (!sql.SqlTask_Write(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, quer, ref Dbgmsg))
                //    {
                //        string mes = "Queryの実行が失敗しました";
                //        msg = tcommons.ErrorMessage(taskid, fs, mes);
                //        return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //    }
                //}
                ////////////////////////////////////////ここまで


                // PMMSデバッグ中の為、一時コメントアウト2022.03.15
                // JunkiSys.Dll
                if (!ResinPrg.WorkResin.RecipeStarted(fs.RecipeFile, dt, ref msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }


                Dbgmsg += "DB登録が完了しました" + crlf;
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=sta104> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.RecipeFile} タスク終了";
            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        //// outのEND出力タスク関数
        //public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        //{
        //    string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
        //    //返信用result,message,retcode追加
        //    tcommons.AddItems2DictOutputData(taskret, ref Dict);


        //    //<taskid=sta901>【ファイル生成】ENDファイルの発行
        //    taskid = 901;
        //    Task_Ret oef = tcommons.OutputEndFile(taskid, fs, Dict, "end", ref msg, ref Dbgmsg);
        //    if (oef.Result == retkey.ng)
        //    {
        //        return oef;
        //    }

        //    return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        //}
    }
}
