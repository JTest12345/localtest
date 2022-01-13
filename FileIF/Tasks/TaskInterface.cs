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
}
