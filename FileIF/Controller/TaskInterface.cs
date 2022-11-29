using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIf
{
    // KeyFileのタスククラス用インターフェース
    public interface ITask
    {
        // タスク実動作関数
        string[] InFileTasks(Mcfilesys fs);
        // 返信ファイル処理関数
        string[] OutFileTasks(Mcfilesys fs, int errorcode);
    }


    // KeyFileタスククラスの基底クラス
    public class Tasks_base
    {
        // タスク処理用共通関数
        public Tasks_Common tcommons { get; set; }

        // 改行コード
        public static string crlf = "\r\n";

        //タスクID
        public int taskid { get; set; } = 0;

        // ArmsWebApi
        public ArmsWebApi.WorkStart ws { get; set; }

        // 設備情報
        public Macconinfo minfo { get; set; }

        // 返信用変数格納用辞書
        public Dictionary<string, string> Dict;

        // 返信ファイル名
        public string endfilenm { get; set; } = "end";

        // 返信関数
        public virtual Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            //返信用result,message,retcode追加
            //返信用result,message,retcode追加
            Dict.Add("{result}", taskret.Result);
            if (taskret.Result == retkey.ok)
            {
                Dict.Add("{resultint}", "1");
            }
            else if (taskret.Result == retkey.ng)
            {
                Dict.Add("{resultint}", "0");
            }
            else if (taskret.Result == retkey.cancel)
            {
                Dict.Add("{resultint}", "2");
            }
            Dict.Add("{message}", taskret.Msg);
            Dict.Add("{retcode}", taskret.RetCode.ToString());
            Dict.Add("{debugmsg}", taskret.DebugMsg.Replace("\r", "").Replace("\n", ""));


            //<taskid=cok1901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            if (!fs.mcfc.disableEndfile)
            {
                Task_Ret oef = tcommons.OutputEndFile(taskid, fs, Dict, endfilenm, ref msg, ref Dbgmsg);
                if (oef.Result == retkey.ng)
                {
                    return oef;
                }
            }


            //<taskid=cok1902> PLCデバイスに直接データを書込みます
            //【デバッグ中コードです】 
            taskid += 1;
            if (fs.mcfc.useplcdevret)
            {
                Task_Ret dspd = tcommons.DataSet2PlcDevs(taskid, fs, minfo, Dict, ref msg, ref Dbgmsg);
                if (dspd.Result == retkey.ng)
                {
                    return dspd;
                }
            }


            //<taskid=cok1903>【PLC】設備にOUTファイル取得要求（PLCの内部リレー操作）
            taskid += 1;
            if (fs.mci.UsePlcTrig)
            {
                Task_Ret fgr = tcommons.FileGetRequest_Plc(taskid, fs, minfo, Dict, ref msg, ref Dbgmsg);
                if (fgr.Result == retkey.ng)
                {
                    return fgr;
                }
            }

            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }

    }


    // ????
    public interface IFile
    {
        string fileId { get; set; }
    }

    // タスク返信クラス
    public class Task_Ret
    {
        public string Result { get; set; }
        public string Msg { get; set; }
        public string DebugMsg { get; set; }
        public int RetCode { get; set; }
    }

    public enum retcode : int
    {
        Success = 0,
        BeforeStart = 0,
        Info = 1,
        Warn = 2,
        Started = 2,
        Closed = 3,
        AllClosed = 3,
        Failure = 9,
        Cancel = 999
    }

    public class retkey
    {
        public const string ok = "OK";
        public const string ng = "NG";
        public const string error = retkey.ng;
        public const string warn = "WARN";
        public const string cancel = "CANCEL";
    }
}
