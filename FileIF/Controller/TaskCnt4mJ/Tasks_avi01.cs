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
        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_avi01()
        {
            tcommons = new Tasks_Common();

            minfo = new Macconinfo();
            avi01 = new TaskFile_avi01();

            rrinfo = new recipe_info();

            Dict = new Dictionary<string, string>();
            Dict.Add("ok", "OK");
            Dict.Add("0", "0");
        }

        // INファイルタスク関数
        public string[] InFileTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "AVI01";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=avi0101>【macconf.ini】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=avi0102> INファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                string[] rsf = avi01.ReadInFile(taskid, fs, ref Dbgmsg);
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
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=avi010x> COJを作成
            try
            {
                taskid += 1;

                var cojref = string.Empty;

                if (!CommonFuncs.JsonFileReader(fs.MacFld + "\\conf\\" + fs.keylbl + ".json", ref cojref, ref Dbgmsg))
                {
                    msg = "COJ元ファイルが読み込めません";
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
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
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
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
                //    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //}
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=avi010x> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.RecipeFile} タスク終了";
            return new string[] { "OK", msg, Dbgmsg, "0" };
        }


        // outのEND出力タスク関数
        public string[] OutFileTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=sta901>【ファイル生成】ENDファイルの発行
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
