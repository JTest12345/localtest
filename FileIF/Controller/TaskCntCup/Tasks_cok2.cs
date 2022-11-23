﻿using System;
using System.Collections.Generic;
using Oskas;



namespace FileIf
{
    class Tasks_cok2: Tasks_base
    {
        //ファイルクラス
        TaskFile_cok2 cok2;

        //本タスクにて使用するDBモデル
        resincup_info rcinfo;

        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_cok2()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            cok2 = new TaskFile_cok2();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // データベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string cupno, string fpath, string[] lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Cok2処理";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=cok2101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=cok2102>【SQL】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;

            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.Dvtype1;
            //fs.plcdvno = minfo.Devno1;

            
            //<taskid=cok2103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
                {
                    return cpa;
                }
            }
            

            //<taskid=cok2104>【検査】cok2ファイル読み込み、またはキャンセル
            try
            {
                taskid += 1;

                Task_Ret rco2f = cok2.ReadCok2FileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rco2f.Result == retkey.ng || rco2f.Result == retkey.cancel)
                {
                    return rco2f;
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=cok2105>【DB登録】(resincup_info)テーブルにcok2情報をUpdateする
            try
            {
                taskid += 1;
                List<string> QueryScr = new List<string>();

                //CUP情報登録
                DateTime dt = DateTime.Now;
                string datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                rcinfo = new resincup_info()
                {
                    Dt_kakuhan = datetime,
                    Macno_kakuhan = minfo.Macno,
                    Cupno = fs.MagCupNo,
                    Cstmproduct = "",
                    Dierank = "",
                    Bdvol = "",
                    Dt_haigo = "",
                    Macno_haigo = "",
                    Kubun = "",
                    Seikeiki = "",
                    Henkaten = "",
                    Recipe = "",
                    Result = 1,
                    Lotnoout = new string[] { "" }
                };

                rcinfo.updateResinCCupInfo();

                if (rcinfo.Dt_kakuhan == "" || rcinfo.Macno_kakuhan == "" || rcinfo.Cupno == "")
                {
                    string mes = "CupデータのDB登録に必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                // JunkiSys.Dll
                if (!ResinPrg.WorkResin.CupMatCompleted(rcinfo.resincupInfo, rcinfo.Macno_kakuhan, dt, ref msg))
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


            //<taskid=cok2106> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }



        // outのEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret) //(string pcat, string macno, string magno, string fpath, int errorcode, string[] lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=cok2901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            
            //<taskid=cok2902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret fgr = tcommons.FileGetRequest_Plc(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
                if (fgr.Result == retkey.ng)
                {
                    return fgr;
                }
            }

            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }
    }
}