using System;
using System.IO;
using System.Collections.Generic;
using Oskas;
using ArmsApi;
using ArmsApi.Model;


namespace FileIf
{
    class Tasks_min2 : Tasks_base
    {
        //ファイルクラス
        TaskFile_min2 min2;

        // ArmsAPI
        Magazine jcm; //TnMag
        AsmLot lot; //TnLot

        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_min2()
        {
            tcommons = new Tasks_Common();
            min2 = new TaskFile_min2();
            minfo = new Macconinfo();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }


        // in2のデータベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fs.fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "MagNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            MySQL sql = new MySQL();
            WipFuncs wip = new WipFuncs(fs.mci.WipDir);

            Dict.Add("{magno}", fs.MagCupNo);


            //<taskid=min2101>【FileSys】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);

            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }

            /*
            if (!mtc.GetMacTaskConf(fs, ref Dbgmsg))
            {
                return new string[] { retkey.ng, Dbgmsg, Dbgmsg, taskid.ToString() };
            }
            */


            //<taskid=min2102>【FileSys】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;
            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.plctrgdevtype;
            //fs.plcdvno = minfo.plctrgdevno;

            
            //<taskid=min2103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
                {
                    return cpa;
                }
            }
            

            //<taskid=min2104> マガジン・ロット情報取得
            try
            {
                taskid += 1;

                jcm = Magazine.GetCurrent(fs.MagCupNo);
                if(jcm == null)
                {
                    msg = "マガジン情報が取得できませんでした。";
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
                lot = AsmLot.GetAsmLot(jcm.NascaLotNO);

                Dict.Add("{product}", lot.TypeCd.PadRight(25, ' '));
                Dict.Add("{lotno}", lot.NascaLotNo);
                Dict.Add("{bdqty}", jcm.FrameQty.ToString()); 

                //for Debug
                Dbgmsg += "マガジンNo.：" + Dict["{magno}"] + crlf;
                Dbgmsg += "製品名　　 ：" + Dict["{product}"] + crlf;
                Dbgmsg += "ロットNo.　：" + Dict["{lotno}"] + crlf;
                Dbgmsg += "基板数   　：" + Dict["{bdqty}"] + crlf;
                //
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min2105> in2ファイルの読み込み～処理の開始またはキャンセル
            try
            {
                taskid += 1;

                Task_Ret rm2f = min2.ReadMin2FileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rm2f.Result == retkey.ng || rm2f.Result == retkey.cancel)
                {
                    return rm2f;
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min2106>【SQL】工程マスタの確認（設備設定によりパラメータ比較）
            try
            {
                taskid += 1;
                var pramnm = Process.GetCurrentParam(jcm.NowCompProcess, lot.TypeCd);

                if (pramnm == null)
                {
                    pramnm = "";
                    Dbgmsg += "現工程マシンパラメータは設定されていません";
                }else
                {
                    Dbgmsg += "現工程マシンパラメータ：" + pramnm + crlf;
                }

                //if (min2.Mparam != npm.Mparam)
                if (min2.Mparam != pramnm)
                {
                    string mes = "登録済マシンパラメータと差異があります";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    if (fs.mcfc.verifiparam)
                    {
                        return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                    }
                    else
                    {
                        Dbgmsg += "登録済マシンパラメータと差異あり\r\n但し設備設定(macconfini)によりパラメータ検査をしていません。" + crlf;
                    }
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min2107>【WIP】WIPファイルの存在確認 ⇒ 確認
            try
            {
                taskid += 1;
                List<string> CallbackSql = new List<string>();

                string WipFilePath = fs.fpath + @"\wip\" + fs.MagCupNo + "_" + jcm.NowCompProcess + "_wipin1.dat";
                var contents = new List<string>();

                // ファイルがない
                if (!CommonFuncs.FileExists(WipFilePath))
                {
                    string mes = "対象のWIPファイルが見つかりません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
                // 読めない
                if (!CommonFuncs.ReadTextFileLine(WipFilePath, ref contents))
                {
                    string mes = "対象のWIPファイルが読み込めません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
                // 空
                if (contents[0]=="")
                {
                    string mes = "対象のWIPファイルの内容が空です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                // WIPファイルの確認
                foreach (string content in contents)
                {
                    var ct = content.Split(',');
                    if (ct[0] != "{date}")
                    {
                        if (Dict[ct[0]] != ct[1])
                        {
                            msg = "WIPファイルの内容が不正です";
                            return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                        }
                    }
                }

                // WIPファイルの削除
                File.Delete(WipFilePath);

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min2108> ARMS開始処理
            try
            {
                taskid += 1;

                //var wsm = new WorkStartAltModel(fs.Macno);

                //bool isOk = wsm.CheckBeforeStart(jcm, out msg);
                //if (!isOk)
                //{
                //    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //}

                //wsm.MagList.Add(jcm);


                //if (!wsm.WorkStart(out msg))
                //{
                //    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //}

                ws = new ArmsWebApi.WorkStart(fs.Macno, "FIF", fs.MagCupNo);

                // 開始前にチェックを入れないと開始できない
                if (!ws.CheckBeforeStart(out msg))
                {
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                // 開始処理
                if (!ws.Start(out msg))
                {
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=min2109> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs,ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }


            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} 対象ロットは開始処理が成功しました";
            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);

        }



        // in2のEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=min2901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end2", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            
            //<taskid=min2902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
