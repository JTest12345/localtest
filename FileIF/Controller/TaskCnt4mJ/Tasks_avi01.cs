using System;
using System.Collections.Generic;
using Oskas;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;


namespace FileIf
{
    class Tasks_avi01 : Tasks_base
    {
        //COJ格納変数
        string cojstr = string.Empty;

        //ファイルクラス
        TaskFile_avi01 avi01;

        //本タスクにて使用するDBテーブル
        recipe_info rrinfo;

        //Endファイル用変数格納用辞書
        //Dictionary<string, string> Dict;

        // 初期化
        public Tasks_avi01()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            avi01 = new TaskFile_avi01();
            rrinfo = new recipe_info();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // INファイルタスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "AVI01";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=avi0101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=avi0102> INファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                Task_Ret rsf = avi01.ReadInFile(taskid, fs, ref Dict, ref Dbgmsg);
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


            //<taskid=avi010x> 読込内容表示
            try
            {
                taskid += 1;

                Dbgmsg += "機種ロット：" + avi01.kishulot + crlf;
                var cnt = 1;
                foreach (var item in avi01.furyomeisai)
                {
                    Dbgmsg += $"不良明細{cnt}：";
                    foreach (var value in item)
                    {
                        Dbgmsg += value + " /";
                    }
                    Dbgmsg += crlf;
                    cnt++;
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=avi010x> COJを作成
            try
            {
                taskid += 1;

                var cojref = string.Empty;

                if (!CommonFuncs.JsonFileReader(fs.MacFld + "\\conf\\" + fs.keylbl + ".json", ref cojref, ref Dbgmsg))
                {
                    msg = "COJ元ファイルが読み込めません";
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                var coj = JsonConvert.DeserializeObject<CojIF.AVI01_01>(cojref);

                var kihonjouho = new CojIF.AVI01_01.KihonJyouho()
                {
                    Kishulot = avi01.kishulot,
                    Tanto = avi01.tanto,
                    Jyoukenmei = avi01.jyoken,
                    Gouki = int.Parse(avi01.setubi)
                };

                coj.cejObject.coList[0].formData = kihonjouho;

                var jissekijyouho = new CojIF.AVI01_01.JissekiJouho()
                {
                    Kaishijikan = avi01.kaisijikan,
                    Shuryojikan = avi01.shuryoujikan,
                    Kensajikan = avi01.kensajikan,
                    Hpk = avi01.hpk,
                    Budomari = avi01.budomari,
                    Tounyusu = long.Parse(avi01.tounyusu),
                    Ryouhinkei = long.Parse(avi01.ryouhinsu),
                    Furyoukei = long.Parse(avi01.furyousu)
                };

                coj.cejObject.coList[1].formData = jissekijyouho;

                var furyoMeisai = new List<CojIF.AVI01_01.FuryoJouho>();
                foreach (var item in avi01.furyomeisai)
                {
                    var fm = new CojIF.AVI01_01.FuryoJouho();
                    fm.Furyocode = item[0];
                    fm.Furyomei = item[1];
                    fm.Haishutusu = long.Parse(item[2]);
                    fm.Kenshutusu = long.Parse(item[3]);

                    furyoMeisai.Add(fm);
                }

                coj.cejObject.coList[2].formData = furyoMeisai;

                cojstr = JsonConvert.SerializeObject(coj, Formatting.Indented);

                //COJをデバッグ表示
                Dbgmsg += cojstr;

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=sta104>【COJ生成】COJIFを生成したCOJでキック
            try
            {
                taskid += 1;

                var coji = new CojIF.CojIf(cojstr);

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


            //<taskid=avi010x> inフォルダからtempフォルダにINファイルを移動
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
