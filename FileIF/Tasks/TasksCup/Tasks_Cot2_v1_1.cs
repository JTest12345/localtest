using System;
using System.Collections.Generic;
using Oskas;



namespace FileIf
{
    class Tasks_Cot2_v1_1
    {
        //CommonFuncs commons;
        Tasks_Common tcommons;
        PlcCom plc;
        MySQL sql;

        //ファイルクラス
        Contents_cot2 cot2;
        Macconinfo minfo;

        //本タスクにて使用するDBテーブル
        resincup_info rcinfo;

        Dictionary<string, string> Dict; //Endファイル用変数格納用辞書

        static string crlf = "\r\n"; // 改行コード
        int taskid = 0; //タスクID

        // 初期化
        public Tasks_Cot2_v1_1()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();
            plc = new PlcCom();
            sql = new MySQL();

            minfo = new Macconinfo();
            cot2 = new Contents_cot2();

            rcinfo = new resincup_info();

            Dict = new Dictionary<string, string>();
            Dict.Add("ok", "OK");
            Dict.Add("0", "0");
        }

        // rcpのデータベース操作タスク関数
        public string[] DBTasks(Mcfilesys fs) //(string pcat, string macno, string cupno, string fpath, string[] lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
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

                //resincup_infoテーブルUPDATE
                DateTime dt = DateTime.Now;
                string datetime = dt.ToString("yyyy-MM-dd HH:mm:ss");
                rcinfo.Dt_kakuhan = datetime;
                rcinfo.Macno_kakuhan = minfo.Macno;
                rcinfo.Cupno = fs.MagCupNo;
                rcinfo.Cstmproduct = cot2.product_out;

                if (rcinfo.Dt_kakuhan == "" || rcinfo.Macno_kakuhan == "" || rcinfo.Cupno == "" || rcinfo.Cstmproduct == "")
                {
                    string mes = "resincup_infoテーブルのUPDATEに必要なデータに空があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                QueryScr.Add($"UPDATE resincup_info SET datetime_kakuhan='{rcinfo.Dt_kakuhan}', macno_kakuhan='{rcinfo.Macno_kakuhan}' WHERE cupno='{rcinfo.Cupno}'");
                Dbgmsg += "Query1: " + QueryScr[0] + crlf;

                // QueryScrリストからSQL文を実行
                foreach (string quer in QueryScr)
                {
                    if (!sql.SqlTask_Write(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, quer, ref Dbgmsg))
                    {
                        Dbgmsg = msg;
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


            //<taskid=cot2106> inフォルダからtempフォルダにINファイルを移動
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
        public string[] FOutTasks(Mcfilesys fs, int errorcode) //(string pcat, string macno, string magno, string fpath, int errorcode, string[] lbl)
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
