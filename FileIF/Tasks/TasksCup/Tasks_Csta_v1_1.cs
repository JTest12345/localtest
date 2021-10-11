using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{
    class Tasks_Csta_v1_1
    {
        //CommonFuncs commons;
        Tasks_Common tcommons;
        MySQL sql;

        //ファイルクラス
        Contents_csta csta;
        Macconinfo minfo;

        //本タスクにて使用するDBテーブル
        recipe_info rrinfo;

        Dictionary<string, string> Dict; //Endファイル用変数格納用辞書

        int taskid = 0; //タスクID
        static string crlf = "\r\n"; // 改行コード

        // 初期化
        public Tasks_Csta_v1_1()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();
            sql = new MySQL();

            minfo = new Macconinfo();
            csta = new Contents_csta();

            rrinfo = new recipe_info();

            Dict = new Dictionary<string, string>();
            Dict.Add("ok", "OK");
            Dict.Add("0", "0");
        }

        // rcpのデータベース操作タスク関数
        public string[] DBTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=sta101>【macconf.ini】設備情報取得
            taskid = 100;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=sta102> staファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                string[] rsf = csta.ReadStaFileTask(taskid, fs, ref Dbgmsg);
                if (rsf[0] == "NG" || rsf[0] == "Cancel")
                {
                    return rsf;
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=sta103>【DB登録】recipe_infoテーブルにレシピ開始情報追加
            try
            {
                taskid += 1;
                List<string> QueryScr = new List<string>();

                //recipe_infoテーブルにレシピ情報追加
                DateTime dt = DateTime.Now;
                string datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                rrinfo.Getsta = 1;
                rrinfo.Dtsta = datetime;

                if (rrinfo.Getsta == 0 || rrinfo.Dtsta == "")
                {
                    string mes = "recipe_infoテーブルのINSERTに必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                QueryScr.Add($"UPDATE recipe_info SET get_sta={rrinfo.Getsta}, datetime_sta='{rrinfo.Dtsta}' WHERE recipefile_name='{fs.RecipeFile}'");
                Dbgmsg += "Query1: " + QueryScr[0] + crlf;

                // QueryScrリストからSQL文を実行
                foreach (string quer in QueryScr)
                {
                    if (!sql.SqlTask_Write(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, quer, ref Dbgmsg))
                    {
                        string mes = "Queryの実行が失敗しました";
                        msg = tcommons.ErrorMessage(taskid, fs, mes);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                }
                Dbgmsg += "Queryの実行は全て終了しました" + crlf;
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=sta104> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.RecipeFile} DBタスク終了";
            return new string[] { "OK", msg, Dbgmsg, "0" };
        }


        // outのEND出力タスク関数
        public string[] FOutTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=out901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }


            return new string[] { "OK", msg, Dbgmsg, "0" };
        }
    }
}
