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
        public string[] InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "MagNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            WipFuncs wip = new WipFuncs(fs.mci.WipDir);

            Dict.Add("magno", fs.MagCupNo);


            //<taskid=min1101>【FileSys】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=min1102>【FileSys】PLCの接続条件取得
            taskid += 1;
            string[] gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc[0] == "NG")
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.plctrgdevtype;
            //fs.plcdvno = minfo.plctrgdevno;

            
            //<taskid=min1103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (cpa[0] == "NG")
                {
                    return cpa;
                }
            }


            //<taskid=min1104> ARMS開始前チェック
            try
            {
                taskid += 1;

                //var wsm = new WorkStartAltModel(fs.Macno);

                ////Console.WriteLine(wsm.Mac);

                //jcm = Magazine.GetCurrent(fs.MagCupNo);
                //lot = AsmLot.GetAsmLot(jcm.NascaLotNO);

                //bool isOk = wsm.CheckBeforeStart(jcm, out msg);

                ws = new ArmsWebApi.WorkStart(fs.Macno, "FIF", fs.MagCupNo);
                
                if (!ws.CheckBeforeStart(out msg))
                {
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                Dict.Add("product", ws.lot.TypeCd.PadRight(25, ' '));
                Dict.Add("lotno", ws.lot.NascaLotNo);
                Dict.Add("valout", ws.mag.FrameQty.ToString()); //ARMS要改修

                //for Debug
                Dbgmsg += "マガジンNo.：" + Dict["magno"] + crlf;
                Dbgmsg += "製品名　　 ：" + Dict["product"] + crlf;
                Dbgmsg += "ロットNo.　：" + Dict["lotno"] + crlf;
                Dbgmsg += "基板数   　：" + Dict["valout"] + crlf;
                //
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=min1105>【ファイル生成】WIPファイルの発行
            try
            {
                taskid += 1;

                DateTime dt = DateTime.Now;
                string datein = dt.ToString("yyyy-MM-dd HH:mm:ss");
                List<string> wiplist = new List<string>() {
                    "date," + datein,
                    "magno," + Dict["magno"],
                    "product," + Dict["product"],
                    "lotno," + Dict["lotno"],
                    "valout," + Dict["valout"]
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
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=min1106> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }

            return new string[] { "OK", msg, Dbgmsg, "0" };
        }



        // in1のEND出力タスク関数
        public string[] OutFileTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=min1901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end1", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            
            //<taskid=min1902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
