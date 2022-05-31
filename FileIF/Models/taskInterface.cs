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

        // 
        public Macconinfo minfo { get; set; }

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
