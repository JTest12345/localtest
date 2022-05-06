using System;
using System.Collections.Generic;
using System.Linq;
using Oskas;
using ArmsApi;
using ArmsApi.Model;
using ArmsApi.Model.NASCA;
using ArmsWeb.Models;


namespace FileIf
{
    class Tasks_mot : Tasks_base
    {
        //ファイルクラス
        TaskFile_mot mot;

        // ArmsAPI
        Magazine jcm; //TnMag
        AsmLot lot; //TnLot

        // ArmsWebApi
        ArmsWebApi.WorkEnd we;
        // Arms不良登録用Dict
        Dictionary<string, int> Defectdict;

        //全リターン格納用辞書（Endファイルにデータが必要な場合に使用する）
        Dictionary<string, string> Dict; 


        // 初期化
        public Tasks_mot()
        {
            tcommons = new Tasks_Common();
            mot = new TaskFile_mot();
            minfo = new Macconinfo();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // outのデータベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fs.fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "MagNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            Dict.Add("{magno}", fs.MagCupNo);


            //<taskid=mot101>【FileSys】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == "NG")
            {
                return gmic;
            }


            //<taskid=mot102>【FileSys】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;
            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == "NG")
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.Dvtype3;
            //fs.plcdvno = minfo.Devno3;

            
            //<taskid=mot103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo,ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == "NG")
                {
                    return cpa;
                }
            }


            //<taskid=mot104> マガジン・ロット情報取得
            try
            {
                taskid += 1;

                jcm = Magazine.GetCurrent(fs.MagCupNo);
                if (jcm == null)
                {
                    msg = "マガジン情報が取得できませんでした。";
                    msg = tcommons.ErrorMessage(taskid, fs, msg);;
                    return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
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
                return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
            }

            ////<taskid=mot105>【SQL】工程コード(Pcode)を得る(Table: Process_master)
            //try
            //{
            //    taskid += 1;

            //    string[] Lproc = new string[] { "Pcode", kcm.Lpcode };
            //    //string[] sfpt = cpm.SelectFromProcess_masterTable(taskid, fs, Lproc, kcm.Product, kcm.Mno, ref Dbgmsg); //v1_2でMno廃止
            //    string[] sfpt = cpm.SelectFromProcess_masterTable(taskid, fs, Lproc, kcm.Product, ref Dbgmsg);
            //    if (sfpt[0] == "NG")
            //    {
            //        return sfpt;
            //    }

            //    Dict.Add("pno", cpm.Pno.ToString());

            //    //for Debug
            //    Dbgmsg += "工程No：" + cpm.Pno.ToString() + crlf;
            //    //
            //}
            //catch (Exception ex)
            //{
            //    msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
            //    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //}


            //<taskid=mot105>【検査】outファイルの読み込み～処理の開始またはキャンセル～内容検査
            try
            {
                taskid += 1;

                Task_Ret rmf = mot.ReadMotFileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rmf.Result == "NG" || rmf.Result == "CANCEL")
                {
                    return rmf;
                }

                Dict.Add("{product_out}", mot.product_out);
                Dict.Add("{lotno_out}", mot.lotno_out);
                Dict.Add("{magno_in}", mot.magno_in);
                Dict.Add("{bd_in}", mot.bd_in);
                Dict.Add("{magno_out}", mot.magno_out);
                Dict.Add("{bd_out}", mot.bd_out);

                //for Debug
                Dbgmsg += "設備:製品名：" + mot.product_out + crlf;
                Dbgmsg += "設備:ロットNo：" + mot.lotno_out + crlf;
                Dbgmsg += "設備:入力マガジンNo：" + mot.magno_in + crlf;
                Dbgmsg += "設備:入力基板数：" + mot.bd_in + crlf;
                Dbgmsg += "設備:出力マガジンNo：" + mot.magno_out + crlf;
                Dbgmsg += "設備:出力基板数：" + mot.bd_out + crlf;
                //

                if (mot.product_out != lot.TypeCd)
                {
                    string mes = "OUTファイル内の製品名が不正です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
                }
                if (mot.lotno_out != lot.NascaLotNo)
                {
                    string mes = "OUTファイル内のロットNoが不正です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
            }


            ////<taskid=mot107>【SQL/検査】①出力マガジンはCurrent_magテーブルが空マガジン登録の状態か？ ②入力マガジンはProcess_resultsテーブルでmagin登録されているか？
            //try
            //{
            //    taskid += 1;
            //    //①出力マガジンはCurrent_magテーブルが空マガジン登録の状態か？ 
            //    string[] sfct = jcm.SelectFromCurrent_magTable(taskid, fs, mot.magno_out, ref Dbgmsg);
            //    if (sfct[0] == "NG")
            //    {
            //        return sfct;
            //    }

            //    if (jcm.Lpcode != "NA") // 登録上はまだ空マガジンの状態
            //    {
            //        string mes = "出力マガジンのDB登録が不正です（既に実マガジンの状態）";
            //        msg = tcommons.ErrorMessage(taskid, fs, mes);
            //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //    }

            //    //②入力マガジンはProcess_resultsテーブルでmagin登録されているか？
            //    string[] sfpt = cpv.SelectFromProcess_resultsTable(taskid, fs, kcm.Product, kcm.Lotno, cpm.Pcode, ref Dbgmsg);
            //    if (sfpt[0] == "NG")
            //    {
            //        return sfpt;
            //    }

            //    if (cpv.Magin != mot.magno_in)
            //    {
            //        string mes = "入力マガジンのDB登録が不正です（マガジン実績登録されたMag[in]ではない）";
            //        msg = tcommons.ErrorMessage(taskid, fs, mes);
            //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };

            //    }
            //}
            //catch (Exception ex)
            //{
            //    msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
            //    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //}


            ////<taskid=mot108>【DB登録】①ロット実績テーブル(Process_results)更新／②入力マガジンを空マガジン登録／③出力マガジンを実マガジン登録
            //try
            //{
            //    taskid += 1;
            //    List<string> QueryScr = new List<string>();

            //    //①ロット実績テーブル(Process_results)更新
            //    DateTime dt = DateTime.Now;
            //    string dateout = dt.ToString("yyyy-MM-dd HH:mm:ss");
            //    Process_results udpv = new Process_results();
            //    // IN/OUT数量, アウト実マガジンは設備のアウトファイルから登録
            //    udpv.Valin = mot.val_in;
            //    udpv.Valout = mot.val_out;
            //    udpv.Magout = mot.magno_out;
            //    // OUT日時は現在時間で登録
            //    udpv.Dateout = dateout;
            //    if (udpv.Valin == "" || udpv.Valout == "" || udpv.Magout == "" || udpv.Dateout == "")
            //    {
            //        string mes = "Process_resultsテーブルのUPDATEに必要なデータに空があります";
            //        msg = tcommons.ErrorMessage(taskid, fs, mes);
            //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //    }
            //    QueryScr.Add($"UPDATE Process_results SET val_in='{udpv.Valin}', val_out='{udpv.Valout}', magno_out='{udpv.Magout}', date_mag_out='{udpv.Dateout}' WHERE product = '{kcm.Product}' AND lotno = '{kcm.Lotno}' AND pcode = '{cpm.Pcode}'");
            //    Dbgmsg += "Query1: " + QueryScr[0] + crlf;

            //    //②出力マガジンを実マガジン登録
            //    // 実マガジンのPcode,Product,LotnoはIN空マガジンの登録を引き継ぎ
            //    jcm.Lpcode = kcm.Lpcode; 
            //    jcm.Product = kcm.Product;
            //    jcm.Lotno = kcm.Lotno;
            //    // 設備番号はminfoから登録
            //    jcm.Macno = minfo.Macno;
            //    // マガジン番号は設備のアウトファイルから登録
            //    jcm.Magno = mot.magno_out;
            //    if (jcm.Product == "" || jcm.Lotno == "" || jcm.Macno == "" || jcm.Magno == "")
            //    {
            //        string mes = "Current_magテーブルのUPDATEに必要なデータに空があります";
            //        msg = tcommons.ErrorMessage(taskid, fs, mes);
            //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //    }
            //    QueryScr.Add($"UPDATE Current_mag SET last_pcode='{jcm.Lpcode}', product='{jcm.Product}', lotno='{jcm.Lotno}', macno='{jcm.Macno}', in_out='OUT' WHERE magno='{jcm.Magno}'");
            //    Dbgmsg += "Query2: " + QueryScr[1] + crlf;

            //    //③入力マガジンを空マガジン登録
            //    //空マガジン番号はのアウトファイルから登録
            //    kcm.Magno = mot.magno_in;
            //    //空マガジンのデータは'NA'登録
            //    kcm.Lpcode = "NA";
            //    kcm.Product = "NA";
            //    kcm.Lotno = "NA";
            //    kcm.Macno = "NA";
            //    kcm.Io = "NA";
            //    kcm.Valbs = 0;
            //    if (kcm.Magno == "")
            //    {
            //        string mes = "Current_magテーブルのUPDATEに必要なデータに空があります";
            //        msg = tcommons.ErrorMessage(taskid, fs, mes);
            //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //    }
            //    QueryScr.Add($"UPDATE Current_mag SET last_pcode='{kcm.Lpcode}', product='{kcm.Product}', lotno='{kcm.Lotno}', macno='{kcm.Macno}', in_out='{kcm.Io}', val_bs='{kcm.Valbs}' WHERE magno='{kcm.Magno}'");
            //    Dbgmsg += "Query3: " + QueryScr[2] + crlf;

            //    // QueryScrリストからSQL文を実行
            //    foreach (string quer in QueryScr)
            //    {
            //        if (!sql.SqlTask_Write(fs.lbl[1] + taskid.ToString(), fs.ConnectionString, quer, ref Dbgmsg))
            //        {
            //            Dbgmsg = msg;
            //            string mes = "Queryの実行が失敗しました";
            //            msg = tcommons.ErrorMessage(taskid, fs, mes);
            //            return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //        }
            //    }
            //}
            //catch (Exception ex)
            //{
            //    msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
            //    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            //}

            //<taskid=mot106> ARMS完了処理
            try
            {
                taskid += 1;

                //WorkEndAltModel wem = new WorkEndAltModel(fs.Macno);

                //wem.MagList = wem.getUnloaderMag(wem.PlantCd);

                //ArmsApi.Model.VirtualMag[] vmgzs =
                //    ArmsApi.Model.VirtualMag.GetVirtualMag(wem.Mac.MacNo.ToString(), ((int)ArmsApi.Model.Station.Unloader).ToString(), string.Empty);

                //var ulmagazine = new List<string> { jcm.MagazineNo };
                //List<ArmsApi.Model.Magazine> svrmags = new List<ArmsApi.Model.Magazine>();
                //foreach (string mgz in ulmagazine)
                //{
                //    ArmsApi.Model.Magazine svrmag = ArmsApi.Model.Magazine.GetCurrent(mgz);

                //    //ブレンドされているロット、かつ最終工程以降の工程の開始の場合
                //    CutBlend[] cbs = CutBlend.GetData(mgz);
                //    if (cbs.Length > 0)
                //    {
                //        AsmLot lot = AsmLot.GetAsmLot(mgz);
                //        int lastprocno = Order.GetLastProcNoFromLotNo(cbs.First().BlendLotNo);
                //        Process prevproc = Process.GetPrevProcess(lastprocno, lot.TypeCd);
                //        Process nextprocess = Process.GetNextProcess(prevproc.ProcNo, lot);

                //        if (Process.IsFinalStAfterProcess(nextprocess, lot.TypeCd) == true)
                //        {
                //            svrmag = new Magazine();
                //            svrmag.MagazineNo = mgz;
                //            svrmag.NascaLotNO = mgz;
                //            svrmag.NowCompProcess = prevproc.ProcNo;

                //            ArmsApi.Model.AsmLot blendlot = lot;
                //            blendlot.NascaLotNo = cbs.First().BlendLotNo;
                //            wem.BlendLotList.Add(mgz, blendlot);
                //        }
                //    }

                //    ArmsApi.Model.VirtualMag vmag = vmgzs.Where(vm => vm.LastMagazineNo == mgz).FirstOrDefault();
                //    if (vmag == null)
                //    {
                //        msg = "Unloader位置に一致する仮想マガジンが見つかりません lastMag:" + vmag.MagazineNo;
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }
                //    wem.AddMagazine(svrmag, vmag);
                //}

                //wem.NewMagFrameQty = int.Parse(mot.val_out);
                //wem.UnloaderMagNo = mot.magno_out;


                /////////////////////////////////////////////////////////////////////////////////
                //Arms Web完了実行
                //
                we = new ArmsWebApi.WorkEnd(fs.Macno, "FIF", mot.magno_in, mot.magno_out);

                Defectdict = mot.defectdict;

                //不良計上
                if (Defectdict.Count != 0)
                {
                    if (!we.RegisterDefects(out msg, Defectdict))
                    {
                        msg = tcommons.ErrorMessage(taskid, fs, msg);
                        return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
                    }
                }
                else
                {
                    Dbgmsg += "不良の計上はありませんでした" + crlf;
                }
                

                //完了処理
                if (!we.End(out msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet("NG", msg, Dbgmsg, (int)retcode.Failure);
            }

            //<taskid=mot109> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == "NG")
            {
                return mitf;
            }


            Dbgmsg += "Queryの実行は全て終了しました" + crlf;
            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
            return tcommons.MakeRet("OK", "", Dbgmsg, (int)retcode.Success);
        }


        // outのEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=mot901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end", ref msg, ref Dbgmsg);
            if (oef.Result == "NG")
            {
                return oef;
            }

            
            //<taskid=mot902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret fgr = tcommons.FileGetRequest_Plc(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
                if (fgr.Result == "NG")
                {
                    return fgr;
                }
            }

            return tcommons.MakeRet("OK", "", Dbgmsg, (int)retcode.Success);
        }
    }
}
