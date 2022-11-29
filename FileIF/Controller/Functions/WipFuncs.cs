using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileIf
{
    class WipFuncs
    {
        // 改行コード
        static string crlf = "\r\n";
        // wipファイル出力先
        string WipDir;

        public WipFuncs(string wipdir)
        {
            WipDir = wipdir;
        }

        public bool MakeWipFile(string knd, string key, List<string> Query, string WipFilePath, ref string contents, ref string dbgmsg)
        {
            try
            {
                switch (knd)
                {
                    case "dat":
                        foreach (string q in Query)
                        {
                            contents += q + crlf;
                        }
                        break;
                    case "py":
                        ReadWipyFormat(key, ref contents);
                        string QueryList = string.Join("\",\"", Query);
                        contents = contents.Replace("$querylist", QueryList);
                        break;
                }

                if(!WriteWipFile(WipFilePath, contents))
                {
                    dbgmsg += "WIPファイルの書き出しに失敗しました";
                }

                return true;
            }
            catch(Exception ex)
            {
                dbgmsg += ex.Message;
                return false;
            }
        }

        private bool ReadWipyFormat(string key, ref string contents)
        {
            try
            {
                string filepath = WipDir + @"\" + key + ".py";
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding("UTF-8"));
                contents = sr.ReadToEnd();
                sr.Close();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        private bool WriteWipFile(string FilePath, string contents)
        {
            try
            {
                StreamWriter sw = new StreamWriter(FilePath, false, Encoding.GetEncoding("UTF-8"));
                sw.Write(contents);
                sw.Close();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
            
        }
    }
}
