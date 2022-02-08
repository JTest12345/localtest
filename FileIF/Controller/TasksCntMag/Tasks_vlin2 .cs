using System;
using System.IO;
using System.Collections.Generic;
using Oskas;
using ArmsApi;
using ArmsApi.Model;


namespace FileIf
{
    class Tasks_vlin2 : Tasks_base
    {
        //ファイルクラス
        TaskFile_vlin2 vlin2;

        // Endファイル用変数格納用辞書
        Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_vlin2()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();

            vlin2 = new TaskFile_vlin2();

            Dict = new Dictionary<string, string>();
            Dict.Add("ok", "OK");
            Dict.Add("0", "0");
        }


        // vlin2のデータベース操作タスク関数
        public string[] InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fs.fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "VlotNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            //fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            MySQL sql = new MySQL();
            WipFuncs wip = new WipFuncs(fs.mci.WipDir);

            Dict.Add("vlotno", fs.MagCupNo);


            //<taskid=vlin2101>【FileSys】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);

            if (gmic[0] == "NG")
            {
                return gmic;
            }



            //<taskid=vlin2102>【FileSys】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;
            string[] gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc[0] == "NG")
            {
                return gpcc;
            }

            
            //<taskid=vlin2103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (cpa[0] == "NG")
                {
                    return cpa;
                }
            }
            

            //<taskid=vlin2104> vlin2ファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                string[] rl2f = vlin2.Readvlin2FileTask(taskid, fs, ref Dbgmsg);
                if (rl2f[0] == "NG" || rl2f[0] == "Cancel")
                {
                    return rl2f;
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=vlin2105>【WIP】WIPファイルの存在確認 ⇒ 確認
            try
            {
                taskid += 1;
                List<string> CallbackSql = new List<string>();

                string WipFilePath = fs.fpath + @"\wip\" + fs.MagCupNo + "_wipin1.dat";
                var contents = new List<string>();

                // ファイルがない
                if (!CommonFuncs.FileExists(WipFilePath))
                {
                    string mes = "対象のWIPファイルが見つかりません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                // 読めない
                if (!CommonFuncs.ReadTextFileLine(WipFilePath, ref contents))
                {
                    string mes = "対象のWIPファイルが読み込めません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                // 空
                if (contents[0]=="")
                {
                    string mes = "対象のWIPファイルの内容が空です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                // WIPファイルの確認
                //VlotNo
                var wipvlot = contents[1].Split(',')[1];
                if (wipvlot != fs.MagCupNo)
                {
                    msg = "WIPファイルの内容: VLOTNOが不正です";
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                //FMlotNo
                foreach (string fmCode in vlin2.fmCodeList)
                {
                    if (!contents.Contains("fmlotno," + fmCode))
                    {
                        msg = $"WIPファイルの内容: 4MLOT[{fmCode}]は開始可能確認ができていません";
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                }

                // WIPファイルの削除
                File.Delete(WipFilePath);

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=vlin2106> ARMS開始処理
            try
            {
                taskid += 1;

                //// 一度CheckBeforeStartを全てのロットで処理しておく
                //foreach (string fmCode in vlin2.fmCodeList)
                //{
                //    ws = new ArmsWebApi.WorkStart(fs.Macno, "FileIF", fmCode);

                //    if (!ws.CheckBeforeStart(out msg))
                //    {
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }
                //}

                // 全てのロットを開始処理
                foreach (string fmCode in vlin2.fmCodeList)
                {
                    ws = new ArmsWebApi.WorkStart(fs.Macno, "FileIF", fmCode);

                    // 開始前にチェックを入れないと開始できない
                    ws.CheckBeforeStart(out msg);

                    if (!ws.Start(out msg))
                    {
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=min2108> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} VLOT内の全てのロット開始が成功しました";
            return new string[] { "OK", msg, Dbgmsg, "0" };

        }



        // vlin2のEND出力タスク関数
        public string[] OutFileTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=vlin2901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end2", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            
            //<taskid=vlin2902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
