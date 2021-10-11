﻿using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{
    class Tasks_Cot1_v1_1
    {
        //CommonFuncs commons;
        Tasks_Common tcommons;
        MySQL sql;

        //ファイルクラス
        Contents_cot1 cot1;
        Macconinfo minfo;

        //本タスクにて使用するDBテーブル
        resincup_info rcinfo;
        resincup_lots rclot;

        Dictionary<string, string> Dict;

        int taskid = 0;
        static string crlf = "\r\n"; // 改行コード

        // 初期化
        public Tasks_Cot1_v1_1()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();
            sql = new MySQL();

            minfo = new Macconinfo();
            cot1 = new Contents_cot1();

            rcinfo = new resincup_info();
            rclot = new resincup_lots();

            Dict = new Dictionary<string, string>(); //Endファイル用変数格納用辞書
            Dict.Add("ok", "OK");
            Dict.Add("0", "0");
        }

        // rcpのデータベース操作タスク関数
        public string[] DBTasks(Mcfilesys fs)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
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
                rcinfo.Cupno = fs.MagCupNo;
                rcinfo.Cstmproduct = cot1.product_out;
                rcinfo.Dierank = cot1.dierank_out;
                rcinfo.Bdvol = cot1.bdvol_out;
                rcinfo.Dt_haigo = datetime;
                rcinfo.Macno_haigo = cot1.macno_out;
                rcinfo.Kubun = cot1.kubun_out;
                rcinfo.Seikeiki = cot1.seikeiki_out;
                rcinfo.Henkaten = cot1.henkaten_out;
                rcinfo.Recipe = cot1.recipe_out;
                rcinfo.Result = cot1.result;

                if (rcinfo.Cupno == "" || rcinfo.Cstmproduct == "" || rcinfo.Dierank == "" || rcinfo.Bdvol == "" || rcinfo.Dt_haigo == "" 
                    || rcinfo.Macno_haigo == "" || rcinfo.Kubun == "" || rcinfo.Seikeiki == "" || rcinfo.Henkaten == "" || rcinfo.Recipe == "")
                {
                    string mes = "resincup_infoテーブルのINSERTに必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                QueryScr.Add($"INSERT INTO resincup_info (cupno, custom_product, led_rank, board_vol, datetime_haigo, macno_haigo, kubun, seikeiki, henkaten, recipefile_name, result, create_at, create_by) " +
                             $"VALUE ('{rcinfo.Cupno}','{rcinfo.Cstmproduct}','{rcinfo.Dierank}','{rcinfo.Bdvol}','{rcinfo.Dt_haigo}','{rcinfo.Macno_haigo}'" +
                                   $",'{rcinfo.Kubun}','{rcinfo.Seikeiki}','{rcinfo.Henkaten}','{rcinfo.Recipe}','{rcinfo.Result}', '{datetime}', 'magcupsv')");
                Dbgmsg += "Query1: " + QueryScr[0] + crlf;
                
                //resincup_lotsテーブルInsert
                int i = 2;
                foreach(string lotno in cot1.lotno_out)
                {
                    rclot = new resincup_lots();
                    rclot.Cstmproduct = cot1.product_out;
                    rclot.Cstmlotno = lotno;
                    rclot.Cupno = fs.MagCupNo;
                    if (rclot.Cstmproduct == "" || rclot.Cstmlotno == "" || rclot.Cupno == "")
                    {
                        string mes = "resincup_lotsテーブルのINSERTに必要なデータに空があります";
                        msg = tcommons.ErrorMessage(taskid, fs, mes);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                    QueryScr.Add($"INSERT INTO resincup_lots (custom_product, custom_lotno, cupno, create_at, create_by) VALUE ('{rclot.Cstmproduct}','{rclot.Cstmlotno}','{rclot.Cupno}, '{datetime}', 'magcupsv'')");
                    Dbgmsg += $"Query{i}: " + QueryScr[i-1] + crlf;
                    i++;
                }

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


            //<taskid=cot1104> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }

            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} DBタスク終了";
            return new string[] { "OK", msg, Dbgmsg, "0" };
        }



        // outのEND出力タスク関数
        public string[] FOutTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=cot1901>【ファイル生成】ENDファイルの発行
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