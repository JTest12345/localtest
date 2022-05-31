using System;
using System.Collections.Generic;
using Oskas;
using YamlDotNet.RepresentationModel;


namespace FileIf
{
    class Tasks_csvtest : Tasks_base
    {
        //◇評価実験用定数・変数
        //CSV移動先
        string csvDirMoveTo = string.Empty;
        //COJ格納変数
        string coj = string.Empty;

        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_csvtest()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // メインタスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Csv";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };


            //<taskid=csvtest101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=csvtest102>【CSV移動】fileconfigを読込
            try
            {
                taskid += 1;
                var filepath = fs.MacFld + "\\conf\\fileconf.yaml";
                var rootNode = CommonFuncs.YamlFileReader(filepath, ref msg);

                if (rootNode != null)
                {
                    var movedir = (YamlMappingNode)rootNode["movedir"];
                    csvDirMoveTo = movedir[fs.keylbl].ToString();
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=csvtest103>【CSV移動】生データを指定先フォルダにコピー / ファイルの存在確認
            try
            {
                taskid += 1;

                if (!CommonFuncs.CopyFile(fs.filepath, csvDirMoveTo + fs.FileNameKey + ".csv"))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "CSVファイルを指定先フォルダにコピーできませんでした");
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                if (CommonFuncs.FileExists(csvDirMoveTo))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "CSVファイルを指定先フォルダにコピーができませんでした（コピーファイルが確認できない）");
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=csvtest104>【COJ生成】FS及び設備情報からCOJ生成
            try
            {
                taskid += 1;
                
                if (!CommonFuncs.JsonFileReader(fs.MacFld + "\\conf\\" + fs.keylbl + ".json", ref coj, ref msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "COJ元ファイルが正常に読込できませんでした。" + msg);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                var dt = DateTime.Now.ToString();

                var cojValueDict = new Dictionary<string, string>
                {
                    { "@datetime", dt },
                    { "@macno", fs.Macno },
                    { "@lotno", fs.FileNameKey },
                    { "@filepath", (csvDirMoveTo + fs.FileNameKey + ".csv").Replace(@"\", @"\\")}
                };

                foreach (KeyValuePair<string, string> cvd in cojValueDict)
                {
                    coj = coj.Replace(cvd.Key, cvd.Value);
                }

                Dbgmsg += coj;

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=csvtest105>【COJ生成】COJIFを生成したCOJでキック
            try
            {
                taskid += 1;

                var coji = new CojIF.CojIf(coj);

                Dbgmsg += coji.TestCojMethod();

                //if (!CommonFuncs.CreateFile(fs.MacFld + "\\temp\\temp.json", coj, ref msg))
                //{
                //    msg = tcommons.ErrorMessage(taskid, fs, "エラー発生：COJIFの処理が失敗しています");
                //    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //}
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=csvtest106> inフォルダからtempフォルダにCSVファイルを移動
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

            //<taskid=sta901>【ファイル生成】ENDファイルの発行
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
