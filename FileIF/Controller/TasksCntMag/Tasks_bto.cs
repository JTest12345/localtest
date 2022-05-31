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
    class Tasks_bto : Tasks_base
    {
        //ファイルクラス
        TaskFile_bto bto;

        // ArmsAPI
        Magazine jcm; //TnMag
        AsmLot lot; //TnLot

        //全リターン格納用辞書（Endファイルにデータが必要な場合に使用する）
        Dictionary<string, string> Dict; 


        // 初期化
        public Tasks_bto()
        {
            tcommons = new Tasks_Common();
            bto = new TaskFile_bto();
            minfo = new Macconinfo();;
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // outのデータベース操作タスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) // string pcat, string macno, string magno, string fs.fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.ConnectionString = fs.mci.ConnectionStrings[0]; // iniファイルのDatabase1を選択

            Dict.Add("{magno}", fs.MagCupNo);


            //<taskid=mot101>【FileSys】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=mot102>【FileSys】PLCの接続条件取得(Table: Macconinfo)
            taskid += 1;
            Task_Ret gpcc = tcommons.GetPlcConnectConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gpcc.Result == retkey.ng)
            {
                return gpcc;
            }

            
            //<taskid=mot103>【PLC】設備のPLCにアクセス可能か確認
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret cpa = tcommons.ChkPlcAccess(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
                if (cpa.Result == retkey.ng)
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
                    msg = tcommons.ErrorMessage(taskid, fs, "ファイル名に使われているマガジンは実マガジン登録がありません");
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
                lot = AsmLot.GetAsmLot(jcm.NascaLotNO);

                Dict.Add("{product}", lot.TypeCd.PadRight(25, ' '));
                Dict.Add("{lotno}", lot.NascaLotNo);
                Dict.Add("{bdqty}", jcm.FrameQty.ToString());

                //for Debug
                Dbgmsg += "◆ファイル名代表マガジン情報" + crlf;
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


            //<taskid=mot105>【検査】outファイルの読み込み～処理の開始またはキャンセル～内容検査
            try
            {
                taskid += 1;

                Task_Ret rmf = bto.ReadBtoFileTask(taskid, fs, ref Dict, ref Dbgmsg);
                if (rmf.Result == retkey.ng || rmf.Result == retkey.cancel)
                {
                    return rmf;
                }

                //for Debug
                Dbgmsg += "◆Btoファイル情報" + crlf;
                foreach (var maginfo in bto.LdMagInfo)
                {
                    Dbgmsg += "***********************************************" + crlf;
                    Dbgmsg += "マガジンNO.：" + maginfo.magNo + crlf;
                    Dbgmsg += "入力基板数：" + maginfo.bdInQty + crlf;
                    Dbgmsg += "不良基板数：" + maginfo.bdFailQty + crlf;
                    Dbgmsg += "個片化良品数：" + maginfo.chipPassQty + crlf;
                    Dbgmsg += "個片化不良数：" + maginfo.chipFailQty + crlf;
                }
                //

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=mot106> ARMSブレンド完了処理
            try
            {
                taskid += 1;

                //var bld = new ArmsWeb.Models.CutBlendModel(fs.Macno);


                //// Btoで指定のマガジンが設備内のブレンド対象であるか確認し、ブレンドリスト(CurrentBlend)に追加
                //var NoMag = true;
                //bld.CurrentBlend.Clear();
                //foreach (var maginfo in bto.LdMagInfo)
                //{
                //    foreach (var bldmag in bld.BlendList)
                //    {
                //        if (maginfo.magNo == bldmag.MagNo)
                //        {
                //            NoMag = false;
                //            bld.CurrentBlend.Add(bldmag);
                //            break;
                //        }
                //        NoMag = true;
                //    }
                //}

                //if (NoMag)
                //{
                //    msg = $"Btoファイルで指定されたマガジンNoは設備：{fs.Macno}のブレンド対象ではありません";
                //    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //}

                //// ブレンド実行
                //try
                //{
                //    bld.FinishBlend();
                //}
                //catch (Exception ex)
                //{
                //    msg = "ブレンドロット完了エラー：" + ex.Message;
                //    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                //}

                if (!ArmsWebApi.LdcBlend.mkblendlot(fs.Macno, bto.magnoList, ref msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }

            //<taskid=mot109> inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs,ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }

            Dbgmsg += "タスクの実行は全て終了しました" + crlf;
            msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        // outのEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=mot901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end", ref msg, ref Dbgmsg);
            if (oef.Result == retkey.ng)
            {
                return oef;
            }

            
            //<taskid=mot902>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
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
