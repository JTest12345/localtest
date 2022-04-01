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
        public string[] InFileTasks(Mcfilesys fs) //(string pcat, string macno, string cupno, string fpath, string[] lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Cup";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=cot2101>【macconf.ini】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=cot2102>【SQL】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;

            string[] gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc[0] == "NG")
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.Dvtype1;
            //fs.plcdvno = minfo.Devno1;

            
            //<taskid=cot2103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (cpa[0] == "NG")
                {
                    return cpa;
                }
            }
            

            //<taskid=cot2104>【検査】cot2ファイル読み込み
            try
            {
                taskid += 1;

                string[] rco2f = cot2.ReadCot2FileTask(taskid, fs, ref Dbgmsg);
                if (rco2f[0] == "NG")
                {
                    return rco2f;
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
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
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
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
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }
                //}
                ////////////////////////////////////////ここまで


                // PMMSデバッグ中の為、一時コメントアウト2022.03.15
                // JunkiSys.Dll
                if (!ResinPrg.WorkResin.CupMatCompleted(rcinfo.resincupInfo, rcinfo.Macno_kakuhan, dt, ref msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                Dbgmsg += "DB登録が完了しました" + crlf;
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=cot2106> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
            return new string[] { "OK", msg, Dbgmsg, "0" };
        }



        // outのEND出力タスク関数
        public string[] OutFileTasks(Mcfilesys fs, int errorcode) //(string pcat, string macno, string magno, string fpath, int errorcode, string[] lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=cot2901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            
            //<taskid=cot2902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] fgr = tcommons.FileGetRequest_Plc(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (fgr[0] == "NG")
                {
                    return fgr;
                }
            }
            

            return new string[] { "OK", msg, Dbgmsg, "0" };
        }
    }
}
