using System;
using System.Collections.Generic;
using Oskas;
//using ArmsApi;
//using ArmsApi.Model;
//using ArmsApi.Model.NASCA;
//using ArmsWeb.Models;


namespace FileIf
{
    class Tasks_min1 : Tasks_base
    {
        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_min1()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // in1のデータベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "MagNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            WipFuncs wip = new WipFuncs(fs.mci.WipDir);

            Dict.Add("{magno}", fs.MagCupNo);


            //<taskid=min1101>【FileSys】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=min1102>【FileSys】PLCの接続条件取得
            taskid += 1;
            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.plctrgdevtype;
            //fs.plcdvno = minfo.plctrgdevno;

            
            //<taskid=min1103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
                {
                    return cpa;
                }
            }


            //<taskid=min1104> ARMS開始前チェック
            try
            {
                taskid += 1;

                ws = new ArmsWebApi.WorkStart(fs.Macno, "FIF", fs.MagCupNo);
                
                if (!ws.CheckBeforeStart(out msg))
                {

                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                Dict.Add("{product}", ws.lot.TypeCd.PadRight(25, ' '));
                Dict.Add("{lotno}", ws.lot.NascaLotNo);
                Dict.Add("{bdqty}", ws.mag.FrameQty.ToString());

                // マガジンのステータス確認
                var magstatus = tcommons.GetMagazineState(Dict["{lotno}"], fs.Pcat, Dict["{magno}"], ref msg);
                //Dict.Add("{retcode}", magstatus.ToString());

                //for Debug
                Dbgmsg += "マガジンNo.：" + Dict["{magno}"] + crlf;
                Dbgmsg += "製品名　　 ：" + Dict["{product}"] + crlf;
                Dbgmsg += "ロットNo.　：" + Dict["{lotno}"] + crlf;
                Dbgmsg += "基板数   　：" + Dict["{bdqty}"] + crlf;
                Dbgmsg += "基板数   　：" + Dict["{bdqty}"] + crlf;
                Dbgmsg += "RetCode ：" + magstatus.ToString() + crlf;

                //◆マガジンステータスが開始前でなければWARNで終了する
                //  RetCodeはmagstatusを渡す
                if (magstatus != 0)
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet(retkey.warn, msg, Dbgmsg, magstatus);
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                //完了時の出力
                //return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min1105>【ファイル生成】WIPファイルの発行
            try
            {
                taskid += 1;

                DateTime dt = DateTime.Now;
                string datein = dt.ToString("yyyy-MM-dd HH:mm:ss");
                List<string> wiplist = new List<string>() {
                    "{date}," + datein,
                    "{magno}," + Dict["{magno}"],
                    "{product}," + Dict["{product}"],
                    "{lotno}," + Dict["{lotno}"],
                    "{bdqty}," + Dict["{bdqty}"]
                };

                Dbgmsg = "" + crlf;

                string WipFilePath = fs.fpath + @"\wip\" + fs.MagCupNo + "_" + ws.mag.NowCompProcess + "_wipin1.dat";
                string contents = "";

                if (wip.MakeWipFile("dat", fs.lbl[1], wiplist, WipFilePath, ref contents, ref Dbgmsg))
                {
                    Dbgmsg += contents;
                }
                else
                {
                    string mes = "WIPファイル作成時に問題が発生しました";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min1106> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs,ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }


            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }



        // in1のEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=min1901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end1", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            
            //<taskid=min1902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret fgr = tcommons.FileGetRequest_Plc(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (fgr.Result == retkey.ng)
                {
                    return fgr;
                }
            }

            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }

    }
}
