using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace VIP_DBDataCopy
{
    class Program
    {
        //タイムアウト秒
        public static int TimeoutSeconds { get; set; } = 1;

        //ディレクトリ有無
        public static bool DirectoryExists(string path)
        {
            return TimeoutCore(() => Directory.Exists(path));
        }

        //タイムアウト処理部分
        private static bool TimeoutCore(Func<bool> existFunction)
        {
            try
            {
                var source = new CancellationTokenSource();
                source.CancelAfter(TimeoutSeconds * 1000);
                var task = Task.Factory.StartNew(() => existFunction(), source.Token);
                task.Wait(source.Token);
                return task.Result;
            }
            catch (OperationCanceledException ex)
            {
                return false;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        static void Main(string[] args)
        {
            string ffrom;
            string path;
            string month;
            string machinenum;
            //ffrom = @"E:\DB_data\1142\2019";
            // #1A:1-6,#1B:11-16,#2A:81-86,#2B:91-97

            path = @"D:\test\Filemoto";

            


            Program obj = new Program();           

                ffrom = @"E:\WB_data";
            ffrom = @"E:\DB_data";
            //machinenum = m.ToString();
            //ffrom = ffrom + machinenum + "\\share\\" + yyyymm + "\\Bind";

            //ffrom = @"D:\test\Filemoto\" + yyyymm + "\\Bind";
            //パスがなかったら、1秒で次のアドレスを見に行くような仕様
            if (DirectoryExists(ffrom))
                    {
                        obj.CopyYesterday(ffrom);
                    }
                
        }

        public void CopyYesterday(string ffrom)
        {
            //string folderFrom = @"D:\test\Filemoto"; //コピー元のフォルダー
            string folderFrom = ffrom; //コピー元のフォルダー
            string folderTo = @"D:\test\DB2022"; //コピー先のフォルダー
            string yyyymmdd;
            yyyymmdd = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            //string folderTo = @"\\vautom1\data\DB\TrendMonitor\" + yyyymmdd;
            string filePattern = "N*";

            DateTime filetime;
            int namelength;
            DateTime d = DateTime.Now.AddDays(-1).Date;

            DirectoryInfo directory = new DirectoryInfo(folderTo);

            foreach (FileInfo file in directory.EnumerateFiles())
            {
                file.Delete();
            }

            foreach (DirectoryInfo dir in directory.EnumerateDirectories())
            {
                dir.Delete(true);
            }

            var sf = new NativeMethods.SHFILEOPSTRUCT();

            sf.wFunc = NativeMethods.FileFuncFlags.FO_COPY; //コピーを指示します。
            sf.fFlags = NativeMethods.FILEOP_FLAGS.FOF_MULTIDESTFILES;
            sf.fFlags = sf.fFlags | NativeMethods.FILEOP_FLAGS.FOF_NOCONFIRMATION; //上書き確認ダイアログを表示せず、上書きします。
            sf.fFlags = sf.fFlags | NativeMethods.FILEOP_FLAGS.FOF_NOCONFIRMMKDIR; //フォルダー作成の確認ダイアログを表示せずフォルダーを作成します。
            sf.fFlags = sf.fFlags | NativeMethods.FILEOP_FLAGS.FOF_NOERRORUI; //エラーダイアログを表示しません。

            //最初にIファイルの対象ファイルを見つける
            var foldersFrom = new System.Collections.Generic.List<string>();
            var foldersTo = new System.Collections.Generic.List<string>();

            //以下の最初の1回のコピー先を入れる処理は意味がないが、これを入れないとファイルがコピーできない
            foldersFrom.Add(folderFrom + "\\" + filePattern);
            foldersTo.Add(folderTo + "\\");

            foreach (string fileName in System.IO.Directory.GetFiles(folderFrom, filePattern, System.IO.SearchOption.AllDirectories))
            {
                string folder = System.IO.Path.GetDirectoryName(fileName);
                if (foldersFrom.Contains(folder + "\\" + filePattern))
                {
                    continue;
                }

                filetime = File.GetLastWriteTime(fileName);
                namelength = Path.GetFileName(fileName).Length;

                //昨日の日付で、名称の長さが30文字以上　※正常完了しなかったロットは12文字くらい、正常ロットは50文字くらい
                if (filetime.Date >= d && namelength >= 30)
                {
                    //ファイル名称入りのフルパスでアドレス指定
                    foldersFrom.Add(fileName);
                    foldersTo.Add(folderTo + "\\");
                }
            }

            sf.pFrom = String.Join("\0", foldersFrom) + "\0";           
            sf.pTo = String.Join("\0", foldersTo) + "\0";

            int result;

            //対象ファイルのコピー実行
            result = NativeMethods.SHFileOperation(ref sf);

            
        }

        //指定のバックアップフォルダへDBの前日の日付のI*,H*データのみをコピーする関数
        public void CopyTest(string ffrom)
        {
            //string folderFrom = @"D:\test\Filemoto"; //コピー元のフォルダー
            string folderFrom = ffrom; //コピー元のフォルダー
            string folderTo = @"D:\test\DB2022"; //コピー先のフォルダー
            string yyyymmdd;
            yyyymmdd = DateTime.Now.AddDays(-1).ToString("yyyyMMdd");
            //string folderTo = @"\\vautom1\data\DB\TrendMonitor\" + yyyymmdd;
            string filePattern = "N*";
            //GetFiles複数条件トライしたが失敗 20220407
            //Regex regex = new Regex(@"I*|H*");

            DateTime filetime;
            int namelength;
            DateTime d = DateTime.Now.AddDays(0).Date;
            DateTime d2 = DateTime.Now.AddDays(-251).Date;

            var sf = new NativeMethods.SHFILEOPSTRUCT();

            sf.wFunc = NativeMethods.FileFuncFlags.FO_COPY; //コピーを指示します。
            sf.fFlags = NativeMethods.FILEOP_FLAGS.FOF_MULTIDESTFILES;
            sf.fFlags = sf.fFlags | NativeMethods.FILEOP_FLAGS.FOF_NOCONFIRMATION; //上書き確認ダイアログを表示せず、上書きします。
            sf.fFlags = sf.fFlags | NativeMethods.FILEOP_FLAGS.FOF_NOCONFIRMMKDIR; //フォルダー作成の確認ダイアログを表示せずフォルダーを作成します。
            sf.fFlags = sf.fFlags | NativeMethods.FILEOP_FLAGS.FOF_NOERRORUI; //エラーダイアログを表示しません。

            //最初にIファイルの対象ファイルを見つける
            var foldersFrom = new System.Collections.Generic.List<string>();
            var foldersTo = new System.Collections.Generic.List<string>();

            //以下の最初の1回のコピー先を入れる処理は意味がないが、これを入れないとファイルがコピーできない
            foldersFrom.Add(folderFrom + "\\" + filePattern);
            foldersTo.Add(folderTo + "\\");

            foreach (string fileName in System.IO.Directory.GetFiles(folderFrom, filePattern, System.IO.SearchOption.AllDirectories))
            {
                string folder = System.IO.Path.GetDirectoryName(fileName);
                if (foldersFrom.Contains(folder + "\\" + filePattern))
                {
                    continue;
                }

                filetime = File.GetLastWriteTime(fileName);
                namelength = Path.GetFileName(fileName).Length;
                
                //昨日の日付で、名称の長さが30文字以上　※正常完了しなかったロットは12文字くらい、正常ロットは50文字くらい
                if (filetime.Date >= d2 && filetime.Date <= d && namelength >= 30)
                {
                    //ファイル名称入りのフルパスでアドレス指定
                    foldersFrom.Add(fileName);
                    //foldersFrom.Add(folder + "\\" + filePattern);

                    //foldersTo.Add(folder.Replace(folderFrom, folderTo) + "\\");
                    foldersTo.Add(folderTo + "\\");
                }
            }

            

            sf.pFrom = String.Join("\0", foldersFrom) + "\0";
            //sf.pFrom = String.Join("\0", foldersFrom);
            //sf.pTo = String.Join("\0", foldersTo);
            sf.pTo = String.Join("\0", foldersTo) + "\0";

            int result;
            
            //対象ファイルのコピー実行
            result = NativeMethods.SHFileOperation(ref sf);

            /*if (result == 0)
            {
                System.Diagnostics.Debug.WriteLine("成功");
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("失敗。" + result);
            }*/
        }

        public class NativeMethods
        {
            /// <summary>
            /// ファイルをコピー・移動・削除・名前変更します。
            /// </summary>
            /// <param name="lpFileOp"></param>
            /// <returns>正常時0。異常時の値の意味は https://docs.microsoft.com/ja-jp/windows/win32/api/shellapi/nf-shellapi-shfileoperationa を参照。</returns>
            [DllImport("shell32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
            public static extern int SHFileOperation([In] ref SHFILEOPSTRUCT lpFileOp);

            [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
            public struct SHFILEOPSTRUCT
            {
                public IntPtr hwnd;
                [MarshalAs(UnmanagedType.U4)] public FileFuncFlags wFunc;
                [MarshalAs(UnmanagedType.LPWStr)] public string pFrom;
                [MarshalAs(UnmanagedType.LPWStr)] public string pTo;
                [MarshalAs(UnmanagedType.U2)] public FILEOP_FLAGS fFlags;
                [MarshalAs(UnmanagedType.Bool)] public bool ffAnyOperationsAborted;
                public IntPtr hNameMappings; //FOF_WANTMAPPINGHANDLEフラグとともに使用します。
                [MarshalAs(UnmanagedType.LPWStr)] public string lplpszProgressTitle; //FOF_SIMPLEPROGRESSフラグとともに使用します。
            }


            public enum FileFuncFlags
            {
                /// <summary>pFrom から pTo にファイルを移動します。</summary>
                FO_MOVE = 0x1,
                /// <summary>pFrom から pTo にファイルをコピーします。</summary>
                FO_COPY = 0x2,
                /// <summary>pFrom からファイルを削除します。</summary>
                FO_DELETE = 0x3,
                /// <summary>pFrom のファイルの名前を変更します。複数ファイルを対象とする場合は FO_MOVE を使用します。</summary>
                FO_RENAME = 0x4
            }

            [Flags]
            public enum FILEOP_FLAGS : short
            {
                /// <summary>pToにはpFromに１対１で対応する複数のコピー先を指定します。</summary>
                FOF_MULTIDESTFILES = 0x1,
                /// <summary>このフラグは使用しません。</summary>
                FOF_CONFIRMMOUSE = 0x2,
                /// <summary>進捗状況のダイアログを表示しません。</summary>
                FOF_SILENT = 0x4,
                /// <summary>同名のファイルが既に存在する場合、新しい名前を付けます。</summary>
                FOF_RENAMEONCOLLISION = 0x8,
                /// <summary>確認ダイアログを表示せず、すべて「はい」を選択したものとします。</summary>
                FOF_NOCONFIRMATION = 0x10,
                /// <summary>FOF_RENAMEONCOLLISIONフラグによるファイル名の衝突回避が発生した場合、SHFILEOPSTRUCT.hNameMappingsに新旧ファイル名の情報を格納します。この情報はSHFreeNameMappingsを使って開放する必要があります。</summary>
                FOF_WANTMAPPINGHANDLE = 0x20,
                /// <summary>可能であれば、操作を元に戻せるようにします。</summary>
                FOF_ALLOWUNDO = 0x40,
                /// <summary>ワイルドカードが使用された場合、ファイルのみを対象とします。</summary>
                FOF_FILESONLY = 0x80,
                /// <summary>進捗状況のダイアログを表示しますが、個々のファイル名は表示しません。</summary>
                FOF_SIMPLEPROGRESS = 0x100,
                /// <summary>新しいフォルダーの作成する前にユーザーに確認しません。</summary>
                FOF_NOCONFIRMMKDIR = 0x200,
                /// <summary>エラーが発生してもダイアログを表示しません。</summary>
                FOF_NOERRORUI = 0x400,
                /// <summary>ファイルのセキュリティ属性はコピーしません。コピー後のファイルはコピー先のフォルダーのセキュリティ属性を引き継ぎます。</summary>
                FOF_NOCOPYSECURITYATTRIBS = 0x800,
                /// <summary>サブディレクトリーを再帰的に処理しません。これは既定の動作です。</summary>
                FOF_NORECURSION = 0x1000,
                /// <summary>グループとして連結しているファイルは移動しません。指定されたファイルだけを移動します。</summary>
                FOF_NO_CONNECTED_ELEMENTS = 0x2000,
                /// <summary>ファイルが恒久的に削除される場合、警告を表示します。このフラグはFOF_NOCONFIRMATIONより優先されます。 </summary>
                FOF_WANTNUKEWARNING = 0x4000,
                /// <summary>UIを表示しません。</summary>
                FOF_NO_UI = FOF_SILENT | FOF_NOCONFIRMATION | FOF_NOERRORUI | FOF_NOCONFIRMMKDIR
            }

        }
    }
}
