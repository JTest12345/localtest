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
        //Dictionary<string, string> Dict;

        // 初期化
        public Tasks_vlin1()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            vlin1 = new TaskFile_vlin1();
            // 返信ファイル名変更
            endfilenm = "end1";
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
            //Dict.Add("{retcode_list}", "9");
            //Dict.Add("{bdqty_list}", "0");
            //retDict = new Dictionary<string, string>(Dict);
        }

        // vlin1のデータベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "VlotNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            //fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            WipFuncs wip = new WipFuncs(fs.mci.WipDir);

            // VlotはMagCupNoに入る
            Dict.Add("{vlotno}", fs.MagCupNo);


            //<taskid=vlin1101>【FileSys】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=vlin1102>【FileSys】PLCの接続条件取得
            taskid += 1;
            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            
            //<taskid=vlin1103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
                {
                    return cpa;
                }
            }

            //<taskid=vlin1104> vlin1ファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                Task_Ret rl2f = vlin1.Readvlin1FileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rl2f.Result == retkey.ng || rl2f.Result == retkey.cancel)
                {
                    return rl2f;
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
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
                    VlotAccess vl = VlotAccess.CheckVLotInfo(Dict["{vlotno}"], seilotnolst);
                    if (vl.IsOkOnly)
                    {
                        Dbgmsg += "Vlot整合チェック完了" + crlf;
                    }
                    else
                    {
                        Dbgmsg += "Vlot整合チェックで不正検出" + crlf;
                        msg = "Vlot整合チェックで不正検出";
                        msg = tcommons.ErrorMessage(taskid, fs, msg);
                        return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=vlin1106> ARMS開始前チェック：マガジン状態の確認
            try
            {
                taskid += 1;
                //LOTステータスArr文字列
                var magstateArr = string.Empty;
                //LOT数量Arr文字列
                string bdqtyArr = string.Empty;
                vlin1.beStaMagLst = new List<string>();

                //for Debug
                Dbgmsg += "VロットNo.：" + Dict["{vlotno}"] + crlf;


                foreach (string m4Code in vlin1.m4CodeList)
                {
                    //Vlotはmagno = lotnoで確認
                    var magstatus = tcommons.GetMagazineState(m4Code, fs.Pcat, m4Code, ref msg);
                    magstateArr += magstatus + ",";

                    if (magstatus == 0)
                    {
                        vlin1.beStaMagLst.Add(m4Code);
                    }

                    // マガジンの数量を取得
                    var bdqty = ArmsApi.Model.Magazine.GetMagazine(m4Code).FrameQty;
                    bdqtyArr += bdqty.ToString() + ",";

                    //for Debug
                    Dbgmsg += "4MロットNo.：" + m4Code + "[qty:" + bdqty.ToString() + ", status:" + magstatus.ToString() + "]" + crlf;
                }

                // LOTステータス返信用文字列辞書追加
                magstateArr = magstateArr.TrimEnd(',');
                //Dict["{retcode_list}"] = magstateArr;
                Dict.Add("{retcode_list}", magstateArr);

                // ロット構成基板数返信用文字列辞書追加
                bdqtyArr = bdqtyArr.TrimEnd(',');
                //Dict["{bdqty_list}"] =  bdqtyArr;
                Dict.Add("{bdqty_list}", bdqtyArr);

                //◆開始可能なLOTなし且つmagstateArrにretcode.Startedが含まれなければエラーで返信

                if (vlin1.beStaMagLst.Count == 0 && !magstateArr.Contains(((int)retcode.Started).ToString()))
                {
                    msg = "問合せのロットリストは全て完了済みです";
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.AllClosed);
                }

                //◆上記以外で開始可能なLOTがなければWARNで終了する
                if (vlin1.beStaMagLst.Count == 0)
                {
                    msg = "問合せのロットリストに開始可能なロットがありません";
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet(retkey.warn, msg, Dbgmsg, (int)retcode.Warn);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=vlin1106> ARMS開始前チェック：CheckBeforeStart
            try
            {
                taskid += 1;

                // CheckBeforeStartを全てのロットで処理
                foreach (string m4Code in vlin1.beStaMagLst)
                {
                    ws = new ArmsWebApi.WorkStart(fs.Macno, "FIF", m4Code);

                    if (!ws.CheckBeforeStart(out msg))
                    {
                        return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                    }
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
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
                    "vlotno," + Dict["{vlotno}"],
                };

                // wipに4Mロットを追加
                // ※vlotのwipはLOTNO指定の為マガジン照合内容の照合なしでLOTNOのみになります。
                foreach (var m4lot in vlin1.beStaMagLst)
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
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=vlin1106> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }

            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }



        //// vlin1のEND出力タスク関数
        //public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        //{
        //    string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
        //    //返信用result,message,retcode追加
        //    tcommons.AddItems2DictOutputData(taskret, ref Dict);


        //    //<taskid=vlin1901>【ファイル生成】ENDファイルの発行
        //    taskid = 901;
        //    Task_Ret oef = tcommons.OutputEndFile(taskid, fs, Dict, "end1", ref msg, ref Dbgmsg);
        //    if (oef.Result == retkey.ng)
        //    {
        //        return oef;
        //    }

            
        //    //<taskid=vlin1902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
        //    taskid += 1;
        //    if (fs.mci.UsePlcTrig)
        //    {
        //        Task_Ret fgr = tcommons.FileGetRequest_Plc(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
        //        if (fgr.Result == retkey.ng)
        //        {
        //            return fgr;
        //        }
        //    }

        //    return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        //}

    }
}
