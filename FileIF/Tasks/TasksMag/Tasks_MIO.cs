﻿using System;
using System.Collections.Generic;
using System.Linq;
using Oskas;
using ArmsApi;
using ArmsApi.Model;
using ArmsApi.Model.NASCA;
using ArmsWeb.Models;


namespace FileIf
{
    class Tasks_MIO
    {
        //CommonFuncs commons;
        Tasks_Common tcommons;

        //ファイルクラス
        Contents_mio mio;
        Macconinfo minfo;

        // ArmsAPI
        Magazine jcm; //TnMag
        AsmLot lot; //TnLot

        Dictionary<string, string> Dict; //全リターン格納用辞書（Endファイルにデータが必要な場合に使用する）

        static string crlf = "\r\n"; // 改行コード
        int taskid = 0; //タスクID

        // 初期化
        public Tasks_MIO()
        {
            //commons = new CommonFuncs();
            tcommons = new Tasks_Common();

            mio = new Contents_mio();

            minfo = new Macconinfo();
            //kcm = new Current_mag(); 
            //jcm = new Current_mag(); 
            //cpm = new Process_master(); 
            //cpv = new Process_results(); 

            Dict = new Dictionary<string, string>();
            Dict.Add("ok", "OK");
            Dict.Add("0", "0");
        }

        // outのデータベース操作タスク関数
        public string[] DBTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fs.fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            Dict.Add("magno", fs.MagCupNo);


            //<taskid=mot101>【FileSys】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=mot102>【FileSys】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;
            string[] gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc[0] == "NG")
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.Dvtype3;
            //fs.plcdvno = minfo.Devno3;

            
            //<taskid=mot103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (cpa[0] == "NG")
                {
                    return cpa;
                }
            }


            //<taskid=mot104> マガジン・ロット情報取得
            try
            {
                taskid += 1;

                jcm = Magazine.GetCurrent(fs.MagCupNo);
                lot = AsmLot.GetAsmLot(jcm.NascaLotNO);

                Dict.Add("product", lot.TypeCd.PadRight(25, ' '));
                Dict.Add("lotno", lot.NascaLotNo);
                Dict.Add("valout", jcm.FrameQty.ToString()); //ARMS要改修

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


            //<taskid=mot105>【検査】outファイルの読み込み～処理の開始またはキャンセル～内容検査
            try
            {
                taskid += 1;

                string[] rmf = mio.ReadMotFileTask(taskid, fs, ref Dbgmsg);
                if (rmf[0] == "NG" || rmf[0] == "Cancel")
                {
                    return rmf;
                }

                Dict.Add("product_out", mio.product_out);
                Dict.Add("lotno_out", mio.lotno_out);
                Dict.Add("magno_in", mio.magno_in);
                Dict.Add("val_in", mio.val_in);
                Dict.Add("magno_out", mio.magno_out);
                Dict.Add("val_out", mio.val_out);

                //for Debug
                Dbgmsg += "設備:製品名：" + mio.product_out + crlf;
                Dbgmsg += "設備:ロットNo：" + mio.lotno_out + crlf;
                Dbgmsg += "設備:入力マガジンNo：" + mio.magno_in + crlf;
                Dbgmsg += "設備:入力基板数：" + mio.val_in + crlf;
                Dbgmsg += "設備:出力マガジンNo：" + mio.magno_out + crlf;
                Dbgmsg += "設備:出力基板数：" + mio.val_out + crlf;
                //

                if (mio.product_out != lot.TypeCd)
                {
                    string mes = "IOファイル内の製品名が不正です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                if (mio.lotno_out != lot.NascaLotNo)
                {
                    string mes = "IOファイル内のロットNoが不正です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }


            //<taskid=min110X> ARMS開始完了一括処理
            try
            {
                taskid += 1;

                WorkStartEndAltModel wsem = new WorkStartEndAltModel(fs.Macno);

                string magnoIn = mio.magno_in;
                string magnoOut = mio.magno_out;

                //開始完了の場合、マガジン入れ替えに伴う1マガジン毎の読み込み判定はスキップする
                if (!string.IsNullOrEmpty(magnoIn))
                {
                    ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magnoIn);

                    if (mag == null)
                    {
                        string mes = "ロット情報が存在しませんInMagagin";
                        msg = tcommons.ErrorMessage(taskid, fs, mes);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }

                    bool isOk = wsem.CheckBeforeStart(mag, out msg);

                    if (!isOk)
                    {
                        msg = tcommons.ErrorMessage(taskid, fs, msg);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }

                    wsem.LastReadMag = mag;

                    //開始完了の場合は仮想マガジンも処理しない
                    wsem.AddMagazine(mag);

                }
                else
                {
                    string mes = "IOファイルのINマガジンNoが不正です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                // In/Outのマガジンが違う場合
                if (magnoIn != magnoOut)
                {

                    if (!string.IsNullOrEmpty(magnoOut))
                    {
                        ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magnoOut);
                        if (mag == null)
                        {
                            string mes = "ロット情報が存在しません(OutMagagin)";
                            msg = tcommons.ErrorMessage(taskid, fs, mes);
                            return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                        }

                        //if (WorkStartEndAltModel.IsDoubleToSingleProcess(mag) == true)
                        //{
                        //    magno = elms[1];
                        //}
                    }
                    else
                    {
                        string mes = "IOファイルのOUTマガジンNoが不正です";
                        msg = tcommons.ErrorMessage(taskid, fs, mes);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }

                    wsem.UnloaderMagNo = magnoOut;

                }

                List<string> msgs;
                bool success = wsem.WorkEnd(out msgs);

                if (!success)
                {
                    string rawmsg = string.Join(" ", msgs.ToArray());
                    msg = tcommons.ErrorMessage(taskid, fs, rawmsg);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }



            //<taskid=mot109> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }


            Dbgmsg += "Queryの実行は全て終了しました" + crlf;
            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} DBタスク終了";
            return new string[] { "OK", msg, Dbgmsg, "0" };
        }


        // outのEND出力タスク関数
        public string[] FOutTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=mot901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            
            //<taskid=in902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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