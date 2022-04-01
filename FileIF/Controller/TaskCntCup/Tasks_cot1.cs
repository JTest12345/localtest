using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{
    class Tasks_cot1 : Tasks_base
    {
        //ファイルクラス
        TaskFile_cot1 cot1;

        //本タスクにて使用するDB
        resincup_info rcinfo;

        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_cot1()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            cot1 = new TaskFile_cot1();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // データベース操作タスク関数
        public string[] InFileTasks(Mcfilesys fs)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Cup";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[1]; // iniファイルのDatabase2を選択


            //<taskid=min1101>【macconf.ini】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=cot1102>【検査】cot1ファイル読み込み
            try
            {
                taskid += 1;

                string[] rco1f = cot1.ReadCot1FileTask(taskid, fs, ref Dbgmsg);
                if (rco1f[0] == "NG")
                {
                    return rco1f;
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=cot1103>【DB登録】(resincup_info)/(resincup_lots)テーブルにcot1情報をInsertする
            try
            {
                taskid += 1;
                List<string> QueryScr = new List<string>();

                //resincup_infoテーブルInsert
                DateTime dt = DateTime.Now;
                string datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                rcinfo = new resincup_info()
                {
                    Cupno = fs.MagCupNo,
                    Cstmproduct = cot1.product_out,
                    Dierank = cot1.dierank_out,
                    Bdvol = cot1.bdvol_out,
                    Dt_haigo = datetime,
                    Macno_haigo = cot1.macno_out,
                    Kubun = cot1.kubun_out,
                    Seikeiki = cot1.seikeiki_out,
                    Henkaten = cot1.henkaten_out,
                    Recipe = cot1.recipe_out,
                    Result = cot1.result
                };

                rcinfo.updateResinCCupInfo();



                if (rcinfo.Cupno == "" || rcinfo.Cstmproduct == "" || rcinfo.Dierank == "" || rcinfo.Bdvol == "" || rcinfo.Dt_haigo == "" 
                    || rcinfo.Macno_haigo == "" || rcinfo.Kubun == "" || rcinfo.Seikeiki == "" || rcinfo.Henkaten == "" || rcinfo.Recipe == "")
                {
                    string mes = "CupデータのDB登録に必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }


                ///////////////////////////////////////////////////
                // DB先はPMMSとなったため下記コードはコメントアウト
                //
                //QueryScr.Add($"INSERT INTO resincup_info (cupno, custom_product, led_rank, board_vol, datetime_haigo, macno_haigo, kubun, seikeiki, henkaten, recipefile_name, result, create_at, create_by) " +
                //             $"VALUE ('{rcinfo.Cupno}','{rcinfo.Cstmproduct}','{rcinfo.Dierank}','{rcinfo.Bdvol}','{rcinfo.Dt_haigo}','{rcinfo.Macno_haigo}'" +
                //                   $",'{rcinfo.Kubun}','{rcinfo.Seikeiki}','{rcinfo.Henkaten}','{rcinfo.Recipe}','{rcinfo.Result}', '{datetime}', 'magcupsv')");
                //Dbgmsg += "Query1: " + QueryScr[0] + crlf;

                ////resincup_lotsテーブルInsert
                //int i = 2;
                //foreach(string lotno in cot1.lotno_out)
                //{
                //    rclot = new resincup_lots();
                //    rclot.Cstmproduct = cot1.product_out;
                //    rclot.Cstmlotno = lotno;
                //    rclot.Cupno = fs.MagCupNo;
                //    if (rclot.Cstmproduct == "" || rclot.Cstmlotno == "" || rclot.Cupno == "")
                //    {
                //        string mes = "resincup_lotsテーブルのINSERTに必要なデータに空があります";
                //        msg = tcommons.ErrorMessage(taskid, fs, mes);
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }
                //    QueryScr.Add($"INSERT INTO resincup_lots (custom_product, custom_lotno, cupno, create_at, create_by) VALUE ('{rclot.Cstmproduct}','{rclot.Cstmlotno}','{rclot.Cupno}, '{datetime}', 'magcupsv'')");
                //    Dbgmsg += $"Query{i}: " + QueryScr[i-1] + crlf;
                //    i++;
                //}

                //// QueryScrリストからSQL文を実行
                //foreach (string quer in QueryScr)
                //{
                //    if (!sql.SqlTask_Write(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, quer, ref Dbgmsg))
                //    {
                //        string mes = "Queryの実行が失敗しました";
                //        msg = tcommons.ErrorMessage(taskid, fs, mes);
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }
                //}
                ////////////////////////////////////////ここまで


                // PMMSデバッグ中の為、一時コメントアウト2022.03.15
                // JunkiSys.Dll
                if (!ResinPrg.WorkResin.CupMatCompleted(rcinfo.resincupInfo, rcinfo.Macno_haigo, dt, ref msg))
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


            //<taskid=cot1104> inフォルダからtempフォルダにINファイルを移動
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
        public string[] OutFileTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=cot1901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end1", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            return new string[] { "OK", msg, Dbgmsg, "0" };
        }

    }
}
