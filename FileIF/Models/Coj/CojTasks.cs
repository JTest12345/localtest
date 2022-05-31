using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Oskas;

namespace FileIf
{
    ////////////////////////////////////
    ///
    /// CojTaskFile クラス
    ///
    ////////////////////////////////////


    class TaskFile_avi01 : IFile // avi01ファイル
    {
        public string fileId { get; set; } = "_avi01.csv";
        public string kishulot { get; set; }
        public string tanto { get; set; }
        public string jyoken { get; set; }
        public string setubi { get; set; }
        public string kaisijikan { get; set; }
        public string shuryoujikan { get; set; }
        public string kensajikan { get; set; }
        public string hpk { get; set; }
        public string budomari { get; set; }
        public string tounyusu { get; set; }
        public string ryouhinsu { get; set; }
        public string furyousu { get; set; }
        public List<string[]> furyomeisai { get; set; }


        public Task_Ret ReadInFile(int taskid, Mcfilesys fs,ref Dictionary<string, string> Dict, ref string Dbgmsg)
        {
            furyomeisai = new List<string[]>();
            Tasks_Common tcommons = new Tasks_Common();
            string msg;
            try
            {
                // ヘッダー情報読取
                kishulot = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "機種LOT");
                tanto = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "担当");
                jyoken = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "条件");
                setubi = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "設備");
                kaisijikan = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "開始時間");
                shuryoujikan = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "終了時間");
                kensajikan = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "検査時間");
                hpk = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "h/k");
                budomari = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "歩留");
                tounyusu = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "投入数");
                ryouhinsu = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "良品数");
                furyousu = CommonFuncs.GetIniValue(fs.filepath, "Hedder", "不良数");

                // 不良項目読取
                var contents = new List<string>();
                if (!CommonFuncs.ReadTextFileLine(fs.filepath, ref contents, "UTF-16"))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "不良項目の読取が失敗しました");
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                foreach (var item in contents)
                {
                    if (item.Contains("不良明細="))
                    {
                        var fmeisai = item.Replace("不良明細=", "").Split(',');
                        furyomeisai.Add(fmeisai);
                    }
                }

                return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
            }
            catch(Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }
            
        }
    }
}
