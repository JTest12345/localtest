using System;
using System.Collections.Generic;
using CejDataAccessCom;


namespace FileIf
{
    class Tasks_cok1: Tasks_base
    {
        //ファイルクラス
        TaskFile_cok1 cok1;

        // Endファイル用変数格納用辞書
        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_cok1()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            cok1 = new TaskFile_cok1();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // cok1のデータベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Cok1処理";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };


            //<taskid=cok1101>【FileSys】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=cok1102>【FileSys】PLCの接続条件取得
            taskid += 1;
            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            
            //<taskid=cok1103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
                {
                    return cpa;
                }
            }


            //<taskid=cok1104> 撹拌開始前チェック：CUPの状況確認をPMMSに問合せ
            try
            {
                taskid += 1;
                bool allowmix = false;

                // JunkiSys.Dll
                if (!ResinPrg.WorkResin.CheckBeforeStartResinMix(fs.MagCupNo, minfo.Macno, ref allowmix, ref msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    Dbgmsg += "PMMSからエラー返信がありました" + crlf;
                    Dbgmsg += "AllowMix: 0 で登録します" + crlf;
                    Dict.Add("{allowmix}", "0");
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                Dbgmsg += "PMMS問合せが完了しました" + crlf;
                Dbgmsg += "AllowMix: " + allowmix.ToString() + crlf;
                if (allowmix)
                {
                    Dict.Add("{allowmix}", "1");
                }
                else
                {
                    Dict.Add("{allowmix}", "0");
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }

            //<taskid=cok1105> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }

            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }



        // cok1のEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=cok1901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end1", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            
            //<taskid=cok1902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
