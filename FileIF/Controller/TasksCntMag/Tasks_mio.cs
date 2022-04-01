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
    class Tasks_mio : Tasks_base
    {
        //ファイルクラス
        TaskFile_mio mio;

        // ArmsAPI
        Magazine jcm; //TnMag
        AsmLot lot; //TnLot

        // ArmsWebApi
        ArmsWebApi.WorkStartEnd wse;

        //全リターン格納用辞書（Endファイルにデータが必要な場合に使用する）
        Dictionary<string, string> Dict; 

        // 初期化
        public Tasks_mio()
        {
            tcommons = new Tasks_Common();
            mio = new TaskFile_mio();
            minfo = new Macconinfo();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // outのデータベース操作タスク関数
        public string[] InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fs.fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "MagNo";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            Dict.Add("magno", fs.MagCupNo);


            //<taskid=mio101>【FileSys】設備情報取得
            taskid = 101;
            string[] gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic[0] == "NG")
            {
                return gmic;
            }


            //<taskid=mio102>【FileSys】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;
            string[] gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc[0] == "NG")
            {
                return gpcc;
            }

            //fs.plcdvtyp = minfo.Dvtype3;
            //fs.plcdvno = minfo.Devno3;

            
            //<taskid=mio103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                string[] cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref msg, ref Dbgmsg);
                if (cpa[0] == "NG")
                {
                    return cpa;
                }
            }


            //<taskid=mio104> マガジン・ロット情報取得
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


            //<taskid=mio105>【検査】outファイルの読み込み～処理の開始またはキャンセル～内容検査
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


            //<taskid=mio106> ARMS開始完了一括処理
            try
            {
                taskid += 1;

                //WorkStartEndAltModel wsem = new WorkStartEndAltModel(fs.Macno);

                //string magnoIn = mio.magno_in;
                //string magnoOut = mio.magno_out;

                ////開始完了の場合、マガジン入れ替えに伴う1マガジン毎の読み込み判定はスキップする
                //if (!string.IsNullOrEmpty(magnoIn))
                //{
                //    ArmsApi.Model.Magazine mag = ArmsApi.Model.Magazine.GetCurrent(magnoIn);

                //    if (mag == null)
                //    {
                //        string mes = "ロット情報が存在しません(Inマガジン)";
                //        msg = tcommons.ErrorMessage(taskid, fs, mes);
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }

                //    bool isOk = wsem.CheckBeforeStart(mag, out msg);

                //    if (!isOk)
                //    {
                //        msg = tcommons.ErrorMessage(taskid, fs, msg);
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }

                //    ///////////////////////////////////
                //    // 2021.10.12 Junichi Watanabe
                //    // 開始完了一括の基板数はアウト分の基板数が操作しにくいので見合わせ
                //    //
                //    //wsem.LastReadMag = mag;
                //    //if (string.IsNullOrEmpty(mio.val_in))
                //    //{
                //    //    mag.FrameQty = int.Parse(mio.val_in);
                //    //}
                //    ///////////////////////////////////

                //    //開始完了の場合は仮想マガジンも処理しない
                //    wsem.AddMagazine(mag);

                //}
                //else
                //{
                //    string mes = "IOファイルのINマガジンNoが不正です";
                //    msg = tcommons.ErrorMessage(taskid, fs, mes);
                //    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //}

                //// In/Outのマガジンが違う場合
                //if (magnoIn != magnoOut)
                //{

                //    if (string.IsNullOrEmpty(magnoOut))
                //    {
                //        string mes = "IOファイルのOUTマガジンNoが不正です";
                //        msg = tcommons.ErrorMessage(taskid, fs, mes);
                //        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                //    }

                //    wsem.UnloaderMagNo = magnoOut;

                //}

                wse = new ArmsWebApi.WorkStartEnd(fs.Macno, "FILEIF", mio.magno_in, mio.magno_out);

                bool success = wse.StartEnd(out msg);

                if (!success)
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }



            //<taskid=mio107> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            string[] mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref msg, ref Dbgmsg);
            if (mitf[0] == "NG")
            {
                return mitf;
            }


            Dbgmsg += "Queryの実行は全て終了しました" + crlf;
            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
            return new string[] { "OK", msg, Dbgmsg, "0" };
        }


        // outのEND出力タスク関数
        public string[] OutFileTasks(Mcfilesys fs, int errorcode)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=mio901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            string[] oef = tcommons.OutputEndFile(taskid, fs, errorcode, Dict, "end", ref msg, ref Dbgmsg);
            if (oef[0] == "NG")
            {
                return oef;
            }

            
            //<taskid=mio902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
