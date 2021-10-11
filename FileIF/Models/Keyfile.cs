using System;
using System.Linq;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{
    ////////////////////////////////////
    ///
    /// KeyFile interface
    ///
    ////////////////////////////////////
    
    public interface IFile
    {
        string fileId { get; set; }
    }

    ////////////////////////////////////
    ///
    /// KeyFiles クラス
    ///
    ////////////////////////////////////

    class Contents_min1: IFile // in1ファイル
    {
        public string fileId { get; set; } = "_min1.csv";
    }

    class Contents_min2: IFile // in2ファイル
    {
        public string fileId { get; set; } = "_min2.csv";

        public string Mparam { get; set; }
        public string ErrorCode { get; set; }

        public string[] ReadMin2FileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            //CommonFuncs commons = new CommonFuncs();
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                string[] contents;
                string content = "";
                

                if (!CommonFuncs.ReadTextFile(fs.filepath, ref content))
                {
                    string mes = content;
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                string results = new string(content.Where(c => !char.IsControl(c)).ToArray());
                results = results.Replace("\"", "");
                contents = results.Split(',');

                Mparam = contents[0];

                if (Mparam == "ERROR")
                {
                    ErrorCode = contents[1];
                    string mes = $"エラー通知(No.{ErrorCode}受信、全タスクを中止します";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    Dbgmsg += "IN2処理で設備側からタスクのキャンセル要求がありました";
                    return new string[] { "Cancel", msg, Dbgmsg, taskid.ToString() };
                }
                else
                {
                    //for Debug
                    Dbgmsg += "設備:パラメータ：" + Mparam + "\r\n";
                    //
                }

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

        }
    }

    class Contents_mio : Contents_mot //mio ファイル
    {

    }

    class Contents_mot // motファイル
    {
        public string fileId { get; set; } = "_mot.csv";

        public string product_out { get; set; }
        public string lotno_out { get; set; }
        public string magno_in { get; set; }
        public string val_in { get; set; }
        public string magno_out { get; set; }
        public string val_out { get; set; }

        public string[] ReadMotFileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            //CommonFuncs commons = new CommonFuncs();
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                string[] contents;
                string content = "";
                if (!CommonFuncs.ReadTextFile(fs.filepath, ref content))
                {
                    string mes = content;
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                contents = content.Split(',');

                if (contents[0] == "ERROR")
                {
                    string errcode = contents[1];
                    string mes = $"エラー通知(No.{contents[1]}受信、全タスクを中止します";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    Dbgmsg += "MOT処理で設備側からタスクのキャンセル要求がありました";
                    return new string[] { "Cancel", msg, Dbgmsg, taskid.ToString() };
                }

                product_out = contents[0];
                lotno_out = contents[1];
                magno_in = contents[2];
                val_in = contents[3];
                magno_out = contents[4];
                val_out = contents[5];

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

        }
    }

    class Contents_recipe // recipeファイル
    {
        public string fileId { get; set; } = "_rcp.txt";

        public string mname { get; set; }

        public string[] ReadRcpFileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            //CommonFuncs commons = new CommonFuncs();
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                List<string> Recipelines = new List<string>();

                if (!CommonFuncs.ReadTextFileLine(fs.filepath, ref Recipelines))
                {
                    string mes = "レシピファイルの読み込みに失敗しました";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                mname = Recipelines[0];

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

        }
    }

    class Contents_csta // staファイル
    {
        public string fileId { get; set; } = "_sta.csv";

        public string Result { get; set; }
        public string ErrorCode { get; set; }

        public string[] ReadStaFileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            //CommonFuncs commons = new CommonFuncs();
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                string[] contents;
                string content = "";
                if (!CommonFuncs.ReadTextFile(fs.filepath, ref content))
                {
                    string mes = content;
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                contents = content.Split(',');

                Result = contents[0];
                if (contents.Length == 2)
                    ErrorCode = contents[1];

                if (Result == "ERROR")
                {
                    string mes = $"エラー通知(No.{ErrorCode}受信、全タスクを中止します";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    Dbgmsg += "STA処理で設備側からタスクのキャンセル要求がありました";
                    return new string[] { "Cancel", msg, Dbgmsg, taskid.ToString() };
                }

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }
        }
    }

    class Contents_cot1 // cot1ファイル
    {
        public string fileId { get; set; } = "_cot1.csv";

        public string product_out { get; set; }
        public string dierank_out { get; set; }
        public string bdvol_out { get; set; }
        public string macno_out { get; set; }
        public string kubun_out { get; set; }
        public string seikeiki_out { get; set; }
        public string henkaten_out { get; set; }
        public string recipe_out { get; set; }
        public string[] lotno_out { get; set; }
        public int result { get; set; }

        public string[] ReadCot1FileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            //CommonFuncs commons = new CommonFuncs();
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                List<string> Cupdataline = new List<string>();

                if (!CommonFuncs.ReadTextFileLine(fs.filepath, ref Cupdataline))
                {
                    string mes = "ファイルの読み込みに失敗しました";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                string[] cupdata = Cupdataline[0].Split(',');
                lotno_out = Cupdataline[1].Split(',');
                string[] resultstr = Cupdataline[2].Split(',');

                product_out = cupdata[0];
                dierank_out = cupdata[1];
                bdvol_out = cupdata[2];
                macno_out = cupdata[3];
                kubun_out = cupdata[4];
                seikeiki_out = cupdata[5];
                henkaten_out = cupdata[6];
                recipe_out = cupdata[7];

                result = int.Parse(resultstr[0]);

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }
        }
    }

    class Contents_cot2 // cot2ファイル
    {
        public string fileId { get; set; } = "_cot2.csv";

        public string product_out { get; set; }

        public string[] ReadCot2FileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            //CommonFuncs commons = new CommonFuncs();
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                List<string> Cupdataline = new List<string>();

                if (!CommonFuncs.ReadTextFileLine(fs.filepath, ref Cupdataline))
                {
                    string mes = "ファイルの読み込みに失敗しました";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                string[] cupdata = Cupdataline[0].Split(',');

                product_out = cupdata[0];

                return new string[] { "OK" };
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }
        }
    }

}
