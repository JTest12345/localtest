using System;
using System.Linq;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{

    ////////////////////////////////////
    ///
    /// MagCupTaskFile クラス
    ///
    ////////////////////////////////////

    //
    // MAGファイル：通常工程開始①
    //
    class TaskFile_min1 : IFile // in1ファイル
    {
        public string fileId { get; set; } = "_min1.csv";
    }

    //
    // MAGファイル：通常工程開始②
    //
    class TaskFile_min2 : IFile // in2ファイル
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

    //
    // MAGファイル：通常工程開始完了一括
    //
    class TaskFile_mio : TaskFile_mot //mio ファイル
    {
        public override string fileId { get; set; } = "_mio.csv";
    }

    //
    // MAGファイル：通常工程完了
    //
    class TaskFile_mot // motファイル
    {
        public virtual string fileId { get; set; } = "_mot.csv";

        public string product_out { get; set; }
        public string lotno_out { get; set; }
        public string magno_in { get; set; }
        public string val_in { get; set; }
        public string magno_out { get; set; }
        public string val_out { get; set; }
        public Dictionary<string, int> defectdict { get; set; }

        public string[] ReadMotFileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                string[] contents;
                string content = "";
                List<string> contentList = new List<string>();
                // 不良項目対応の為複数行を読む関数に変更
                // if (!CommonFuncs.ReadTextFile(fs.filepath, ref content))
                if (!CommonFuncs.ReadTextFileLine(fs.filepath, ref contentList))
                {
                    string mes = contentList[0];
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                // ◇実績(1行目)
                content = contentList[0];
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

                // ◇不良(2行目以降)
                defectdict = new Dictionary<string, int>();
                int sumqty = 0;
                if (contentList.Count > 1)
                {
                    for (int i = 1; i < contentList.Count; i++)
                    {
                        contents = contentList[i].Split(',');
                        int qty = 0;
                        if (int.TryParse(contents[1], out qty))
                        {
                            defectdict.Add(contents[0], qty);
                        }
                        else
                        {
                            string mes = "MOTファイル内の不良数が数値ではない為、処理できません";
                            msg = tcommons.ErrorMessage(taskid, fs, mes);
                            return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                        }
                        sumqty += qty;
                    }
                }

                // 基板計上数と基板不良数の整合確認
                int qtyin = 0;
                if (!int.TryParse(val_in, out qtyin))
                {
                    string mes = "MOTファイル内の受入基板数量が数値ではない為、処理できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                int qtyout = 0;
                if (!int.TryParse(val_out, out qtyout))
                {
                    string mes = "MOTファイル内の払出基板数量が数値ではない為、処理できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                if (qtyin - qtyout != sumqty)
                {
                    string mes = "MOTファイル内の基板数量と不良数が不整合の為、処理できません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
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

    //
    // MAGファイル：V溝バリ取り工程開始①
    //
    class TaskFile_vlin1 : IFile // vlin1ファイル
    {
        public string fileId { get; set; } = "_vlin1.csv";

        //4Mロットコードリスト
        public List<string> m4CodeList { get; set; }
        public string ErrorCode { get; set; }

        public string[] Readvlin1FileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
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

                m4CodeList = new List<string>();

                foreach (var m4lot in contents)
                {
                    if (!string.IsNullOrEmpty(m4lot))
                    {
                        m4CodeList.Add(m4lot);

                        //for Debug
                        Dbgmsg += "設備:パラメータ：" + m4lot + "\r\n";
                    }
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

    //
    // MAGファイル：V溝バリ取り工程開始②
    //
    class TaskFile_vlin2 : IFile // vlin2ファイル
    {
        public string fileId { get; set; } = "_vlin2.csv";

        //4Mロットコードリスト
        public List<string> fmCodeList { get; set; }
        public string ErrorCode { get; set; }

        public string[] Readvlin2FileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
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


                if (contents[0] == "ERROR")
                {
                    ErrorCode = contents[1];
                    string mes = $"エラー通知(No.{ErrorCode}受信、全タスクを中止します";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    Dbgmsg += "Vlin2処理で設備側からタスクのキャンセル要求がありました";
                    return new string[] { "Cancel", msg, Dbgmsg, taskid.ToString() };
                }
                else
                {
                    fmCodeList = new List<string>();

                    foreach (var m4lot in contents)
                    {
                        if (!string.IsNullOrEmpty(m4lot))
                        {
                            fmCodeList.Add(m4lot);

                            //for Debug
                            Dbgmsg += "設備:パラメータ：" + m4lot + "\r\n";
                        }
                    }
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

    //
    // MAGファイル：ブレンドロットトレイ(LD工程)完了
    //
    class TaskFile_bto // btoファイル
    {
        public virtual string fileId { get; set; } = "_bto.csv";

        public List<LDMagInfo> LdMagInfo { get; set; }

        public List<string> magnoList { get; set; }

        public class LDMagInfo
        {
            public string magNo { get; set; }
            public string bdInQty { get; set; }
            public string bdFailQty { get; set; }
            public string chipPassQty { get; set; }
            public string chipFailQty { get; set; }
        }

        public TaskFile_bto()
        {
            LdMagInfo = new List<LDMagInfo>();
            magnoList = new List<string>();
        }

        public string[] ReadBtoFileTask(int taskid, Mcfilesys fs, ref string Dbgmsg)
        {
            string msg = "";
            Tasks_Common tcommons = new Tasks_Common();

            try
            {
                // Btoファイル読込
                var contents = new List<string>();
                if (!CommonFuncs.ReadTextFileLine(fs.filepath, ref contents))
                {
                    string mes = "btoファイルが読み込めません";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                // 空
                if (contents[0] == "")
                {
                    string mes = "btoファイルの内容が空です";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                //Errorメッセージか確認
                var err = contents[0].Split(',');

                if (err[0] == "ERROR")
                {
                    string errcode = err[1];
                    string mes = $"エラー通知(No.{err[1]}受信、全タスクを中止します";
                    msg = tcommons.ErrorMessage(taskid, fs, mes);
                    Dbgmsg += "BTO処理で設備側からタスクのキャンセル要求がありました";
                    return new string[] { "Cancel", msg, Dbgmsg, taskid.ToString() };
                }

                // LdMagInfoにデータ格納
                foreach (var maginfo in contents)
                {
                    var arrminfo = maginfo.Split(',');
                    var ldm = new LDMagInfo {
                        magNo = arrminfo[0],
                        bdInQty = arrminfo[1],
                        bdFailQty = arrminfo[2],
                        chipPassQty = arrminfo[3],
                        chipFailQty = arrminfo[4]
                    };
                    LdMagInfo.Add(ldm);
                    magnoList.Add(arrminfo[0]);
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

    //
    // CUPファイル：樹脂配合レシピ受付
    //
    class TaskFile_recipe // recipeファイル
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

    //
    // CUPファイル：樹脂配合レシピ開始
    //
    class TaskFile_sta // staファイル
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

    //
    // CUPファイル：樹脂配合材料配合完了
    //
    class TaskFile_cot1 // cot1ファイル
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

    //
    // CUPファイル：樹脂配合撹拌完了
    //
    class TaskFile_cot2 // cot2ファイル
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
