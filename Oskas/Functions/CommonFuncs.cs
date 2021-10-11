using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace Oskas
{
    public static class CommonFuncs
    {
        //
        /* ini file 読み込み用
         * 
         */
        //
        [DllImport("kernel32.dll")]
        private static extern int GetPrivateProfileString(
        string lpApplicationName,
        string lpKeyName,
        string lpDefault,
        StringBuilder lpReturnedstring,
        int nSize,
        string lpFileName);

        public static string GetIniValue(string FilePath, string section, string key)
        {
            StringBuilder sb = new StringBuilder(256);
            GetPrivateProfileString(section, key, string.Empty, sb, sb.Capacity, FilePath);
            return sb.ToString();
        }


        //
        /* INT確認関数
         * OK: true
         * NG: false
         */
        //
        public static bool IntValueChk(string txtValue)
        {
            if (int.TryParse(txtValue, out int i))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        //
        /* テキストファイル全読み込み関数
         * OK: true
         * NG: false
         * 読込内容：contents 
         */
        //
        public static bool ReadTextFile(string filepath, ref string contents, string enccode = "UTF-8")
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding(enccode));
                contents = sr.ReadToEnd();
                contents = new string(contents.Where(c => !char.IsControl(c)).ToArray());
                sr.Close();

                return true;
            }
            catch (Exception ex)
            {
                contents = ex.Message;
                return false;
            }
        }

        //
        /* テキストファイル一行づつ読み込み関数
         * OK: true
         * NG: false
         * 読込内容(配列)：contents 
         */
        //
        public static bool ReadTextFileLine(string filepath, ref List<string> contents, string enccode = "UTF-8")
        {
            try
            {
                StreamReader sr = new StreamReader(filepath, Encoding.GetEncoding(enccode));
                while (sr.EndOfStream == false)
                {
                    string line = sr.ReadLine();
                    line = new string(line.Where(c => !char.IsControl(c)).ToArray());
                    contents.Add(line);
                }
                sr.Close();

                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        //
        /* ファイル存在確認関数
         * OK: true
         * NG: false
         */
        //
        public static bool FileExists(string filepath)
        {
            try
            {
                if (!File.Exists(filepath))
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        //
        /* ファイル強制移動処理確認関数
         * OK: true
         * NG: false
         */
        //
        public static bool MoveFile(string FilePath, string MoveToPath)
        {
            try
            {
                File.Copy(FilePath, MoveToPath, true);
                File.Delete(FilePath);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        //
        /* ファイル強制コピー処理確認関数
         * OK: true
         * NG: false
         */
        //
        public static bool CopyFile(string FilePath, string MoveToPath)
        {
            try
            {
                File.Copy(FilePath, MoveToPath, true);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }


        /////////////////////////////
        // json file 書き込み用
        /////////////////////////////
        public static void JsonFileWriter(string FilePath, string json, string mes = "")
        {
            DialogResult result = MessageBox.Show(mes + "設定ファイルを保存します", "確認",
                                                    MessageBoxButtons.YesNo,
                                                    MessageBoxIcon.Exclamation,
                                                    MessageBoxDefaultButton.Button1
                                                 );

            if (result == DialogResult.Yes)
            {
                Encoding enc = Encoding.GetEncoding("utf-8");
                StreamWriter writer = new StreamWriter(FilePath, false, enc);
                writer.WriteLine(json);
                writer.Close();
                MessageBox.Show("設定を保存しました");
            }
            else if (result == DialogResult.No)
            {
                MessageBox.Show("保存はキャンセルされました");
            }
        }

        /////////////////////////////
        // json file 読み込み用
        /////////////////////////////
        public static string JsonFileReader(string FilePath)
        {
            StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding("utf-8"));
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        ///////////////////////////////////
        // 汎用ファイル作成保存用
        ///////////////////////////////////
        public static bool CreateSimFile(string FilePath, string Contents, ref string msg)
        {
            try
            {
                // Create the file, or overwrite if the file exists.
                using (FileStream fs = File.Create(FilePath))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(Contents);
                    // Add some information to the file.
                    fs.Write(info, 0, info.Length);
                }

                return true;
            }

            catch (Exception ex)
            {
                msg = ex.ToString();
                return false;
            }
        }

    }
}
