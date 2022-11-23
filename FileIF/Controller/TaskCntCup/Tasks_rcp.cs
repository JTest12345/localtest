using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{
    class Tasks_rcp : Tasks_base
    {
        //ファイルクラス
        TaskFile_recipe rcp;

        //本タスクにて使用するDBモデル
        recipe_info rrinfo;

        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_rcp()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            rcp = new TaskFile_recipe();
            rrinfo = new recipe_info();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // データベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Recipe処理";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=rcp101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=rcp102> レシピファイルを設備に転送
            try
            {
                taskid += 1;
                //recipeファイル読み込み
                Task_Ret rrf = rcp.ReadRcpFileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rrf.Result == retkey.ng)
                {
                    return rrf;
                }

                Dict.Add("mname", rcp.mname);

                // 配合設備にレシピ転送
                string Macfilepath = fs.filepath.Replace(fs.Macno, rcp.mname);
                Macfilepath = Macfilepath.Replace(@"\in\", @"\out\");
                Macfilepath = Macfilepath.Replace("_rcp", "");
                if (!CommonFuncs.CopyFile(fs.filepath, Macfilepath))
                {
                    string mes = "配合設備へのレシピ転送に失敗しました";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
                // アップロード先にレシピ転送
                // アップロード失敗ではタスク停止しない！
                string rulfilepath = fs.mci.RecipUpLoadDir + "\\" + fs.RecipeFile + ".txt";
                if (!CommonFuncs.CopyFile(fs.filepath, rulfilepath))
                {
                    Dbgmsg += "設備へのレシピ転送が失敗しました（このエラーでタスク停止はしません）";
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=rcp103>【DB登録】recipe_infoテーブルにレシピ情報追加
            try
            {
                taskid += 1;
                List<string> QueryScr = new List<string>();

                //recipe_infoテーブルにレシピ情報追加
                DateTime dt = DateTime.Now;
                string datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                rrinfo.Recipe = fs.RecipeFile;
                rrinfo.Mname = rcp.mname;
                rrinfo.Dtin = datetime;

                if (rrinfo.Recipe == "" || rrinfo.Mname == "" || rrinfo.Dtin == "")
                {
                    string mes = "レシピのDB登録に必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                ///////////////////////////////////////////////////
                // DB先はPMMSとなったため下記コードはコメントアウト
                //
                //QueryScr.Add($"INSERT INTO recipe_info (recipefile_name, mac_name, datetime_in, create_at, create_by) VALUE ('{rrinfo.Recipe}', '{rrinfo.Mname}', '{rrinfo.Dtin}', '{datetime}', 'magcupsv')");
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
                if (!ResinPrg.WorkResin.RecipeRecieved(rrinfo.Recipe, rrinfo.Mname, dt, ref msg))
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


            //<taskid=rcp104> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }

            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.RecipeFile} タスク終了";
            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        // outのEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=rcp901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }
    }
}
