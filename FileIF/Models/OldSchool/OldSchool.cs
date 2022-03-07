using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIf
{
    ////////////////////////////////////
    ///
    /// OldSchoolTaskFile クラス
    ///
    ////////////////////////////////////


    class TaskFile_vip1 : IFile // avi01ファイル
    {
        public string fileId { get; set; } = "_vip1.csv";
        public string kishulot { get; set; }
        public string lotno { get; set; }
        public string logdatetime { get; set; }
        public Dictionary<string, string> bin { get; set; }

        public string[] ReadInFile(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg;
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                // ファイル読取
                var contents = new List<string>();
                bin = new Dictionary<string, string>();
                if (!Oskas.CommonFuncs.ReadTextFileLine(fs.filepath, ref contents, fs.mcfc.encoding))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "ファイル読取が失敗しました");
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                var index = 0;
                foreach (var item in contents)
                {
                    if (item.Contains("年"))
                    {
                        break;
                    }
                    index++;
                }

                if (index == contents.Count())
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "ファイル内容が不正です");
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                var keys = contents[index].Split(',');
                var values = contents[index].Split(',');

                index = 0;
                foreach (var key in keys)
                {
                    bin.Add(key, values[index]);
                    index++;
                }

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

        }

        public static Dictionary<string, string> FileContents(string filepath, string enc, ref string msg)
        {
            try
            {
                // ファイル読取
                var contents = new List<string>();
                var bin = new Dictionary<string, string>();
                if (!Oskas.CommonFuncs.ReadTextFileLine(filepath, ref contents, enc))
                {
                    msg = "ファイル読取が失敗しました";
                    return bin;
                }

                var index = 0;
                foreach (var item in contents)
                {
                    if (item.Contains("年"))
                    {
                        break;
                    }
                    index++;
                }

                if (index == contents.Count())
                {
                    msg = "ファイル内容が不正です";
                    return bin;
                }

                var keys = contents[index].Split(',');
                var values = contents[index+1].Split(',');

                index = 0;
                foreach (var key in keys)
                {
                    bin.Add(key, values[index]);
                    index++;
                }

                return bin;
            }
            catch (Exception ex)
            {
                msg = ex.Message;
                return new Dictionary<string, string>();
            }
        }
    }


}
