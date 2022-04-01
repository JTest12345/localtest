using System;
using System.Collections.Generic;
using CejDataAccessCom;


namespace FileIf
{
    class Tasks_vlin1: Tasks_base
    {
        //ファイルクラス
        TaskFile_vlin1 vlin1;

        // Endファイル用変数格納用辞書
        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_vlin1()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            vlin1 = new TaskFile_vlin1();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // vlin1のデータベース操作タスク関数
        public string[] InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "VlotNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            //fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            WipFuncs wip = new WipFuncs(fs.mci.WipDir);

            // VlotはMagCupNoに入る
            Dict.Add("vlotno", fs.MagCupNo);


            //<taskid=vlin1101>【FileSys】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=vlin1102>【FileSys】PLCの接続条件取得
            taskid += 1;
            string[] gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc[0] == "NG")
            {
                return gpcc;
            }

            
            //<taskid=vlin1103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (cpa[0] == "NG")
                {
                    return cpa;
                }
            }

            //<taskid=vlin1104> vlin1ファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                string[] rl2f = vlin1.Readvlin1FileTask(taskid, fs, ref Dbgmsg);
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


            //<taskid=vlin1105> ARMS開始前チェック：VロットのDB状況確認
            try
            {
                taskid += 1;

                if (fs.mci.CheckVlot)
                {
                    //VLotの登録状態と受け取った4Mlotのリストの整合性を検証
                    List<string> seilotnolst = new List<string>();
                    foreach (var m4Code in vlin1.m4CodeList)
                    {
                        seilotnolst.Add(m4Code);
                    }
                    //VロットNo、4MロットNo整合性チェック
                    VlotAccess vl = VlotAccess.CheckVLotInfo(Dict["vlotno"], seilotnolst);
                    if (vl.IsOkOnly)
                    {
                        Dbgmsg += "Vlot整合チェック完了" + crlf;
                    }
                    else
                    {
                        Dbgmsg += "Vlot整合チェックで不正検出" + crlf;
                        //return new string[] { "NG", vl.ErrDescription, Dbgmsg, taskid.ToString() };
                    }
                }

                // CheckBeforeStartを全てのロットで処理
                foreach (string m4Code in vlin1.m4CodeList)
                {
                    ws = new ArmsWebApi.WorkStart(fs.Macno, "FIF", m4Code);

                    if (!ws.CheckBeforeStart(out msg))
                    {
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                }

                //for Debug
                Dbgmsg += "VロットNo.：" + Dict["vlotno"] + crlf;
                foreach (var m4Code in vlin1.m4CodeList)
                {
                    Dbgmsg += "4MロットNo.　：" + m4Code + crlf;
                }
                //
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=vlin1105>【ファイル生成】WIPファイルの発行
            try
            {
                taskid += 1;

                DateTime dt = DateTime.Now;
                string datein = dt.ToString("yyyy-MM-dd HH:mm:ss");

                // wipの内容を初期化
                List<string> wiplist = new List<string>() {
                    "date," + datein,
                    "vlotno," + Dict["vlotno"],
                };

                // wipに4Mロットを追加
                foreach (var m4lot in vlin1.m4CodeList)
                {
                    wiplist.Add("m4lotno," + m4lot);
                }

                Dbgmsg += "" + crlf;

                string WipFilePath = fs.fpath + @"\wip\" + fs.MagCupNo + "_wipin1.dat";
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


            //<taskid=vlin1106> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }

            return new string[] { "OK", msg, Dbgmsg, "0" };
        }



        // vlin1のEND出力タスク関数
        public string[] OutFileTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=vlin1901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end1", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            
            //<taskid=vlin1902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
