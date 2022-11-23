using System;
using System.Collections.Generic;
using Oskas;



namespace FileIf
{
    class Tasks_cot2: Tasks_base
    {
        //ファイルクラス
        TaskFile_cot2 cot2;

        //本タスクにて使用するDBモデル
        resincup_info rcinfo;

        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_cot2()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            cot2 = new TaskFile_cot2();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // データベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string cupno, string fpath, string[] lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Cot2処理";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=cot2101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=cot2102>【SQL】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;

            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.Dvtype1;
            //fs.plcdvno = minfo.Devno1;

            
            //<taskid=cot2103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
                {
                    return cpa;
                }
            }
            

            //<taskid=cot2104>【検査】cot2ファイル読み込み
            try
            {
                taskid += 1;

                Task_Ret rco2f = cot2.ReadCot2FileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rco2f.Result == retkey.ng)
                {
                    return rco2f;
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=cot2105>【DB登録】(resincup_info)テーブルにcot2情報をUpdateする
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
                    Cstmproduct = cot2.product_out
                };

                rcinfo.updateResinCCupInfo();

                if (rcinfo.Dt_kakuhan == "" || rcinfo.Macno_kakuhan == "" || rcinfo.Cupno == "" || rcinfo.Cstmproduct == "")
                {
                    string mes = "CupデータのDB登録に必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                ///////////////////////////////////////////////////
                // DB先はPMMSとなったため下記コードはコメントアウト
                //
                //QueryScr.Add($"UPDATE resincup_info SET datetime_kakuhan='{rcinfo.Dt_kakuhan}', macno_kakuhan='{rcinfo.Macno_kakuhan}' WHERE cupno='{rcinfo.Cupno}'");
                //Dbgmsg += "Query1: " + QueryScr[0] + crlf;

                //// QueryScrリストからSQL文を実行
                //foreach (string quer in QueryScr)
                //{
                //    if (!sql.SqlTask_Write(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, quer, ref Dbgmsg))
                //    {
                //        Dbgmsg = msg;
                //        string mes = "Queryの実行が失敗しました";
                //        msg = tcommons.ErrorMessage(taskid, fs, mes);
                //        return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //    }
                //}
                ////////////////////////////////////////ここまで


                // PMMSデバッグ中の為、一時コメントアウト2022.03.15
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


            //<taskid=cot2106> inフォルダからtempフォルダにINファイルを移動
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

            //<taskid=cot2901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            
            //<taskid=cot2902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
