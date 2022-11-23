using System;
using System.IO;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Oskas;
using FluentFTP;
using Newtonsoft.Json;
using YamlDotNet.RepresentationModel;
using System.Threading;


namespace FileIf
{
    public partial class FileFetchForm : Form
    {
        Magcupini mcini;
        Mcfilesys fs;
        macconfjson mconf;

        object autofetchlock = new object();

        string crlf = "\r\n";
 
        //選択PLC情報
        string SelectedPlcName;
        PLCconf macplcConf;

        string msg = string.Empty;
        string ipaddress;
        string ftpuser;
        string ftpupassword;
        int ftpport;
        string ftphomefolder;
        string maccat;
        string macname;
        string magcupdir;
        string workingDir;
        string scrspath;
        string wippath;
        string tmppath;
        string donepath;
        string fileconfpath;
        YamlNode fileconf;

        public FileFetchForm(Magcupini mci)
        {
            mcini = mci;
            fs = new Mcfilesys();
            macplcConf = new PLCconf();

            InitializeComponent();

            //autofetch timer
            cbx_autofetchInterval.SelectedIndex = 0;

            // form要素の初期化(イネーブル)
            cmb_fetchfile.Enabled = false;
            cmb_macname.Enabled = false;
            cmb_pcat.Enabled = false;
            btn_fetchfile.Enabled = false;
            //fetchConsole.Enabled = false;

            // iniファイルのコンボボックス条件を確認
            // カテゴリ
            if (mcini.FFetchPcat[0] == "NA")
            {
                // iniがNAならMagCupFolderを読込
                mcini.FFetchPcat = Tasks_Common.GetPcatDirName(mcini.MCDir);
            }
            else
            {
                cmb_pcat.Text = mcini.FFetchPcat[0];
            }

            if (mcini.FFetchPcat.Length != 0)
            {
                cmb_pcat.Enabled = true;
                cmb_pcat.Items.AddRange(mcini.FFetchPcat);
                // 初期表示の設定
                if (cmb_pcat.Items.Count > 1)
                {
                    cmb_pcat.Text = "選択してください";
                }
                else
                {
                    cmb_pcat.SelectedIndex = 0;
                }
                
            }
            else
            {
                fetchConsole.Text += "工程カテゴリの読込ができません" + "\r\n";
                return;
            }

            // 設備名
            if (mcini.FFetchMacName[0] != "NA")
            {
                if (mcini.FFetchMacName.Length != 0)
                {
                    cmb_macname.Enabled = true;
                    cmb_macname.Text = mcini.FFetchMacName[0];
                    btn_fetchfile.Enabled = true;
                    SelectGetFileConditions();
                    getPlcInfo("default");
                }
                else
                {
                    fetchConsole.Text += "設備名の読込ができません" + "\r\n";
                    return;
                }
            }
            else
            {

            }

            if (mcini.FFetchFileKey[0] != "NA")
            {
                if (mcini.FFetchFileKey.Length != 0)
                {
                    cmb_fetchfile.Enabled = true;
                    cmb_fetchfile.Items.AddRange(mcini.FFetchFileKey);
                }
                else
                {
                    fetchConsole.Text += "設備名の読込ができません" + "\r\n";
                    return;
                }

            }


        }

        delegate void FetchConsoleDelegate(string text, int level);
        private void FetchConsoleShow(string text, int level)
        {
            if (fetchConsole.InvokeRequired)
            {
                FetchConsoleDelegate d = new FetchConsoleDelegate(FetchConsoleShow);
                BeginInvoke(d, new object[] { text, level });
            }
            else
            {
                string message = "";
                switch (level)
                {
                    case 1:
                        message = "[info] ";
                        break;
                    case 2:
                        message = "[Send] ";
                        break;
                    case 3:
                        message = "[Recieve] ";
                        break;
                    case 4:
                        message = "[Warn] ";
                        break;
                    case 5:
                        message = "[ERROR] ";
                        break;
                }
                fetchConsole.AppendText(message + text + crlf);
            }
        }

        // mciを元にDir検索 ⇒ カテゴリ抽出
        // カテゴリが選択 ⇒ 設備選択
        // 設備選択 ⇒ ファイル選択
        // 実行 ⇒ FTP、など？
        // ファイル読取 ／ 後処理
        // Submit ⇒ 後処理形式データの転送

        private string[] GetPcatDirName(string sDir)
        {
            var pcatarr = new List<string>();

            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    var foldername = d.Replace(sDir + "\\", "");
                    if (!foldername.Contains("_"))
                    {
                        pcatarr.Add(foldername);
                    }
                }
            }
            catch (System.Exception ex)
            {
                OskNLog.Log("ファイル取得時に問題が発生しました", Cnslcnf.msg_error);
                OskNLog.Log(ex.Message, Cnslcnf.msg_error);
            }

            return pcatarr.ToArray();
        }

        public async void DownloadFile()
        {
            lock (this.autofetchlock)
            {
                ipaddress = macplcConf.ipa;
                ftpuser = macplcConf.ftps[0].id;
                ftpupassword = macplcConf.ftps[0].password;
                ftpport = int.Parse(macplcConf.ftps[0].port);
                ftphomefolder = macplcConf.ftps[0].homedir;
                maccat = cmb_pcat.Text;
                macname = cmb_macname.Text;
                msg = string.Empty;
                magcupdir = mcini.MCDir;
                workingDir = magcupdir + @"\" + maccat + @"\" + macname;
                scrspath = workingDir + @"\conf\scripts";
                wippath = workingDir + @"\wip";
                tmppath = workingDir + @"\temp";
                donepath = workingDir + @"\done";
                fileconfpath = workingDir + @"\conf\fileconf.yaml";
                fileconf = CommonFuncs.YamlFileReader(fileconfpath, ref msg);

                try
                {
                    if (macplcConf.ipa == null)
                    {
                        FetchConsoleShow("PLCの設定が正しく行われていないようです", 5);
                        return;
                    }

                    if (!cbx_usebat.Checked)
                    {
                        downloadWithFluentFTP(macplcConf);
                    }
                    else
                    {
                        downloadWithVbScript(macplcConf);

                    }

                }
                catch (Exception e)
                {
                    FetchConsoleShow(e.Message, 5);
                }
            }
        }

        public async void downloadWithVbScript(PLCconf macplcConf)
        {
            btn_fetchfile.Enabled = false;

            //スクリプト内の変数書き換え用辞書
            var scrvardct = new Dictionary<string, string>()
            {
                {"{ipaddress}", ipaddress},
                {"{user}", ftpuser},
                {"{password}", ftpupassword},
                {"{port}", ftpport.ToString()},
                {"{ftphomefolder}", ftphomefolder},
                {"{maccat}", ftphomefolder},
                {"{macname}", ftphomefolder},
                {"{magcupdir}", ftphomefolder},
                {"{workingDir}", ftphomefolder},
                {"{scrspath}", ftphomefolder},
                {"{wippath}", ftphomefolder},
                {"{tmppath}", ftphomefolder}
            };

            var scrs = new YamlSequenceNode();
            if (fileconf != null)
            {
                scrs = (YamlSequenceNode)fileconf["scripts"];
            }
            else
            {
                FetchConsoleShow("fileconfig.yamlが読み込めませんでした", 5);
                return;
            }

            foreach (var scr in scrs)
            {
                Process scriptProc = new Process();
                if (scr.ToString().Contains(".bat"))
                {
                    scriptProc.StartInfo.FileName = scr.ToString();
                    scriptProc.StartInfo.Arguments = "";
                }
                else if (scr.ToString().Contains(".vbs"))
                {
                    scriptProc.StartInfo.FileName = "CScript";
                    scriptProc.StartInfo.Arguments = scr.ToString();
                }
                else if (scr.ToString().Contains(".py"))
                {
                    scriptProc.StartInfo.FileName = "python";
                    scriptProc.StartInfo.Arguments = scr.ToString();
                }
                scriptProc.StartInfo.WindowStyle = ProcessWindowStyle.Hidden; // 非表示で実行したい場合
                scriptProc.StartInfo.WorkingDirectory = wippath;

                // ConfからWipにスクリプトをコピー
                if (!CommonFuncs.CopyFile(scrspath + @"\" + scr, scriptProc.StartInfo.WorkingDirectory + @"\" + scr))
                {
                    FetchConsoleShow("スクリプトコピーが失敗しています", 5);
                    break;
                }

                // WIP内のスクリプトの変数を辞書で書き換え
                var contentslist = new List<string>();
                var contentslst_replaced = new List<string>();
                if (!CommonFuncs.ReadTextFileLine(wippath + @"\" + scr, ref contentslist))
                {
                    FetchConsoleShow("スクリプトコピーが読み込めません", 5);
                    break;
                }
                foreach (var text in contentslist) 
                {
                    var text_replaced = text;
                    foreach (KeyValuePair<string, string> scrvar in scrvardct)
                    {
                        text_replaced = text_replaced.Replace(scrvar.Key, scrvar.Value);
                    }
                    contentslst_replaced.Add(text_replaced);
                }

                var contents = string.Empty;
                foreach (var text in contentslst_replaced)
                {
                    contents += text + crlf;
                }
                if (!CommonFuncs.CreateFile(wippath + @"\" + scr, contents, ref msg))
                {
                    FetchConsoleShow("スクリプトの書き換えができません", 5);
                    break;
                }

                // ロギング用ファイル作成
                if (!CommonFuncs.CreateFile(wippath + @"\log.txt", "", ref msg))
                {
                    FetchConsoleShow("ロギングファイル作成が失敗しています", 5);
                    break;
                }
                // 実行結果ファイル作成
                if (!CommonFuncs.CreateFile(wippath + @"\result.txt", "", ref msg))
                {
                    FetchConsoleShow("実行結果ファイル作成が失敗しています", 5);
                    break;
                }
                await Task.Delay(500);

                scriptProc.Start();
                scriptProc.WaitForExit();
                scriptProc.Close();
                // Log.txt読込
                var loglst = new List<string>();
                var enc = "shift-jis";
                if (!CommonFuncs.ReadTextFileLine(wippath + @"\log.txt", ref loglst, enc)){
                    FetchConsoleShow("ログの読込が失敗しています", 5);
                    break;
                }
                // log表示
                foreach (var log in loglst)
                {
                    FetchConsoleShow(log, 1);
                }
                // result.txt参照
                var result = string.Empty;
                if (!CommonFuncs.ReadTextFile(wippath + @"\result.txt", ref result))
                {
                    FetchConsoleShow("結果ファイルの読込が失敗しています", 5);
                    break;
                }
                if (!(result.ToLower().Contains("pass")))
                {
                    FetchConsoleShow("スクリプトの実行が失敗しています", 5);
                    break;
                }
            }

            // WIP内のファイルを全移動
            var dt = DateTime.Now.ToString("yyyyMMddHHmmss");
            if (!CommonFuncs.MoveFiles(wippath, donepath, "_" + dt + "_wip"))
            {
                FetchConsoleShow("WIPフォルダ内のファイル削除が失敗しています", 5);
            }

            // TEMP内のファイルを全削除
            if (!CommonFuncs.RemoveFiles(tmppath))
            {
                FetchConsoleShow("TEMPフォルダ内のファイル削除が失敗しています", 5);
            }

            btn_fetchfile.Enabled = true;
        }

        public void downloadWithFluentFTP(PLCconf macplcConf)
        {
            using (var client = new FtpClient(ipaddress, ftpuser, ftpupassword))
            {
                client.Port = ftpport;
                client.ConnectTimeout = 5000;
                client.Connect();

                //// download a file and ensure the local directory is created
                //if (client.DownloadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto_ftp.csv", @"tmp\00002_bto.csv") != FtpStatus.Success)
                //// download a file and ensure the local directory is created, verify the file after download
                //// client.DownloadFile(@"D:\Github\FluentFTP\README.md", "/public_html/temp/README.md", FtpLocalExists.Overwrite, FtpVerify.Retry);
                //{
                //    FetchConsoleShow("失敗してます", 5);
                //    return;
                //}

                var lastTimeStamp = new DateTime(2000, 1, 1, 0, 0, 0);
                var lastFileNo = 0;
                var lastTSFile = new FtpListItem();

                foreach (FtpListItem item in client.GetListing(ftphomefolder))
                {
                    if (item.Type == FtpFileSystemObjectType.File)
                    {
                        //◇FTPサーバーがタイムスタンプに対応している場合
                        DateTime time = client.GetModifiedTime(item.FullName);
                        if (time > lastTimeStamp)
                        {
                            lastTimeStamp = time;
                            lastTSFile = item;
                        }

                        //◆指定ファイルダウンロード処理
                        if (rb_getSelectedFile.Checked)
                        {
                            if (item.Name.Contains(cmb_fetchfile.Text))
                            {
                                FetchConsoleShow("検出したファイル：" + item.FullName + " " + time.ToString("G"), 1);
                                if (downloadFile(client, $"{magcupdir}\\{maccat}\\{macname}\\ftp\\" + item.Name, item.FullName))
                                {
                                    FetchConsoleShow("ファイルダウンロードが失敗しています", 5);
                                    return;
                                }
                            }
                            else
                            {
                                FetchConsoleShow("指定のファイルは検出されませんでした", 1);
                                return;
                            }
                        }
                    }
                }

                //◆最新ファイルダウンロード処理
                if (rb_getLatestFile.Checked)
                {
                    FetchConsoleShow("検出したファイル：" + lastTSFile.FullName + " " + lastTimeStamp.ToString("G"), 1);
                    if (!downloadFile(client, $"{magcupdir}\\{maccat}\\{macname}\\ftp\\" + lastTSFile.Name, lastTSFile.FullName))
                    {
                        FetchConsoleShow("ファイルダウンロードが失敗しています", 5);
                        return;
                    }
                }

                FetchConsoleShow("ファイルダウンロードが完了しました", 1);

                //◆FTPサーバー側ワーキングフォルダのクリーン
                if (cbx_cleanFtpFold.Checked)
                {
                    FetchConsoleShow("FTPフォルダ内のファイルを全て削除します", 1);

                    foreach (FtpListItem item in client.GetListing(ftphomefolder))
                    {
                        if (item.Type == FtpFileSystemObjectType.File)
                        {
                            client.DeleteFile(item.FullName);
                            FetchConsoleShow(item.Name + "を削除しました", 1);
                        }
                    }
                }
            }

        }

        private bool downloadFile(FtpClient client, string localpath, string remotepath)
        {
            FetchConsoleShow("ファイル取得を開始します", 1);

            if (client.DownloadFile(localpath, remotepath) != FtpStatus.Success)
            {
                FetchConsoleShow("ファイル取得が失敗してます", 5);
                return false;
            }
            FetchConsoleShow(localpath + "にファイル転送完了", 1);
            return true;
        }

        private void btn_fetchfile_Click(object sender, EventArgs e)
        {
            btn_fetchfile.Enabled = false;
            DownloadFile();
            btn_fetchfile.Enabled = true;
        }

        private void cmb_macname_SelectionChangeCommitted(object sender, EventArgs e)
        {
            SelectGetFileConditions();

            // 最新ファイルを取得するにはFIFJsonBuilderで
            // FIF IOファイル設定に "default"を設定する
            // 最新ファイル取得の場合、これで設定されているPLCのコンフィグで動作する

            getPlcInfo("default");
        }

        private void SelectGetFileConditions()
        {
            try
            {
                btn_fetchfile.Enabled = false;
                tmpFileWatchTimer.Enabled = false;

                //ConfigJson読込関数
                string MacFld = mcini.MCDir + "\\" + cmb_pcat.SelectedItem + "\\" + cmb_macname.SelectedItem;
                string MacConfPath = MacFld + "\\conf\\macconf.json";

                mconf = JsonConvert.DeserializeObject<macconfjson>(CommonFuncs.JsonFileReader(MacConfPath));
                var filekeylist = new List<string>();

                foreach (var item in mconf.Mcfs.mcfconfs)
                {
                    if (item.foi.serverpull && item.foi.pulltype == "FTP")
                    {
                        filekeylist.Add(item.mcfilekey);
                    }
                }
                if (rb_getSelectedFile.Checked)
                {
                    if (filekeylist.ToArray().Length != 0)
                    {
                        cmb_fetchfile_Enabled(filekeylist);
                    }
                    else
                    {
                        fetchConsole.Text += "設備名の読込ができません" + "\r\n";
                        return;
                    }
                }
                else
                {
                    btn_fetchfile.Enabled = true;
                    cmb_fetchfile_Disabled();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void cmb_fetchfile_Enabled(List<string> filekeylist)
        {
            cmb_fetchfile.Enabled = true;
            cmb_fetchfile.Items.Clear();
            cmb_fetchfile.Items.AddRange(filekeylist.ToArray());
            cmb_fetchfile.Text = "選択してください";
        }

        private void cmb_fetchfile_Disabled()
        {
            cmb_fetchfile.Enabled = false;
            cmb_fetchfile.Items.Clear();
            cmb_fetchfile.Text = "";
        }

        private void cmb_pcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_fetchfile.Enabled = false;
            tmpFileWatchTimer.Enabled = false;

            var macnamearr = GetPcatDirName(mcini.MCDir + "\\" + cmb_pcat.Text);

            if (macnamearr.Length != 0)
            {
                cmb_macname.Enabled = true;
                cmb_macname.Items.Clear();
                cmb_macname.Items.AddRange(macnamearr);
                cmb_macname.Text = "選択してください";
                cmb_fetchfile.Items.Clear();
                cmb_fetchfile.Text = "";
                cmb_fetchfile.Enabled = false;
            }
            else
            {
                fetchConsole.Text += "設備名の読込ができません" + "\r\n";
                return;
            }
        }

        private void cmb_fetchfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            btn_fetchfile.Enabled = false;
            tmpFileWatchTimer.Enabled = false;

            getPlcInfo(cmb_fetchfile.SelectedItem.ToString());
        }

        private void getPlcInfo(string targetFileName)
        {
            try
            {
                //該当PLCの名称(ID)抽出
                foreach (var item in mconf.Mcfs.mcfconfs)
                {
                    if (item.foi.serverpull && item.foi.pulltype == "FTP")
                    {
                        if (item.mcfilekey == targetFileName)
                        {
                            SelectedPlcName = item.foi.cntid;
                        }
                    }
                }

                //macplcinfoに該当情報をいれる
                foreach (var item in mconf.Plcs.plcconfs)
                {
                    if (item.name == SelectedPlcName)
                    {
                        macplcConf = item;
                        btn_fetchfile.Enabled = true;
                        tmpFileWatchTimer.Enabled = true;
                    }
                }
            }
            catch(Exception ex)
            {
                FetchConsoleShow(ex.Message, 5);
            }
        }

        private void tmpFileWatchTimer_Tick(object sender, EventArgs e)
        {
            tmpFileWatchTimer.Enabled = false;

            var magcupdir = mcini.MCDir;
            var maccat = cmb_pcat.Text;
            var macname = cmb_macname.Text;
            var indir = $"{magcupdir}\\{maccat}\\{macname}\\in\\";
            var tmpdir = $"{magcupdir}\\{maccat}\\{macname}\\ftp\\";
            var errdir = $"{magcupdir}\\{maccat}\\{macname}\\error\\";
            var tmpkey = string.Empty;
            var inkey = string.Empty;
            var enc = string.Empty;

            foreach (string filepath in Directory.EnumerateFiles(tmpdir))
            {
                var filename = Path.GetFileName(filepath);

                foreach (var mcfconf in mconf.Mcfs.mcfconfs)
                {
                    if (filename.Contains(mcfconf.mcfilekey) || mcfconf.mcfilekey=="default")
                    {
                        tmpkey = mcfconf.mcfilekey;
                        inkey = mcfconf.foi.transinfilekey;
                        enc = mcfconf.encoding;
                    }
                }
                if (rb_getSelectedFile.Checked)
                {
                    if (tmpkey != cmb_fetchfile.Text || string.IsNullOrEmpty(inkey))
                    {
                        FetchConsoleShow("検出ファイルは設定ファイルに登録されていません", 5);
                        File.Move(filepath, errdir + filename + ".err");
                        FetchConsoleShow("検出ファイルはエラーフォルダに転送されました", 1);
                        return;
                    }
                }

                string message = string.Empty;
                string caption = string.Empty;

                if (!filename.Contains(cmb_fetchfile.Text))
                {
                    message = "検出されたファイルをエラーフォルダに転送しますか？";
                    caption = "検出ファイルをエラー処理";
                    var rslt = MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (rslt == DialogResult.Yes)
                    {
                        File.Move(filepath, errdir + filename + ".err");
                        FetchConsoleShow("検出ファイルはエラーフォルダに転送されました", 1);
                    }
                    return;
                }

                // コンソールに検出ファイル情報
                FetchConsoleShow(ShowDetectFileContents(filepath, enc), 1);

                message = "検出されたファイルを上位に転送しますか？";
                caption = "検出ファイルの転送";

                var result = MessageBox.Show(this, message, caption, MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);

                if (rb_getSelectedFile.Checked)
                {
                    filename = filename.Replace(tmpkey, inkey);
                }
                else
                {
                    filename = Path.GetFileNameWithoutExtension(filename) + "_" + inkey + Path.GetExtension(filename);
                }

                if (result == DialogResult.Yes)
                {
                    File.Move(filepath, indir + filename);
                    FetchConsoleShow("検出ファイルはINフォルダに転送されました", 1);
                }
                else if (result == DialogResult.No)
                {
                    message = "検出されたファイルを削除しますか？";
                    caption = "検出ファイルを削除";
                    result = MessageBox.Show(this, message, caption, MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                    if (result == DialogResult.Yes)
                    {
                        File.Delete(filepath);
                        FetchConsoleShow("検出ファイルは削除されました", 1);
                    }
                }
                    
            }

            tmpFileWatchTimer.Enabled = true;
        }

        private string ShowDetectFileContents(string filepath, string enc)
        {
            try
            {
                var msg = string.Empty;
                var contents = string.Empty;
                var filename = Path.GetFileName(filepath);
                var timestamp = File.GetCreationTime(filepath);

                contents += "■■■■■■検出されたファイル■■■■■■" + crlf;
                contents += "◆ファイル名：" + filename + crlf;
                contents += "◆タイムスタンプ：" + timestamp + crlf;

                // ファイル名により個別処理
                var vipfile = new TaskFile_vip1();
                var vipcontents = TaskFile_vip1.FileContents(filepath, enc, ref msg);
                if (vipcontents.Count == 0)
                {
                    FetchConsoleShow(msg, 5);
                    return "";
                }

                if (vipcontents.ContainsKey("年"))
                {
                    contents += "◆機種名：" + vipcontents["機種名抜出し"] + crlf;
                    contents += "◆ロットNO：" + vipcontents["Lot名抜出し"] + crlf;
                    contents += "◆最終記録時間：" + vipcontents["年"] + "/"
                                                + vipcontents["月"] + "/"
                                                + vipcontents["日"] + " "
                                                + vipcontents["時"] + ":"
                                                + vipcontents["分"] + ":"
                                                + vipcontents["秒"] + crlf;
                    contents += "◆BIN1：" + vipcontents["BIN1"] + crlf;
                }
                else if (vipcontents.ContainsKey("W1300"))
                {
                    contents += "◆機種名：" + vipcontents["W1100"] + crlf;
                    contents += "◆ロットNO：" + vipcontents["W1200"] + crlf;
                    contents += "◆最終記録時間：" + vipcontents["W1300"] + "/"
                                                + vipcontents["W1301"] + "/"
                                                + vipcontents["W1302"] + " "
                                                + vipcontents["W1303"] + ":"
                                                + vipcontents["W1304"] + ":"
                                                + vipcontents["W1305"] + crlf;
                    contents += "◆BIN1：" + vipcontents["ZR11512"] + crlf;
                }
                else
                {
                    throw new Exception("KEYががみつかりません");
                    
                }

                contents += "■■■■■■■■■■■■■■■■■■■■■■■[END]";

                return contents;
            }
            catch (Exception ex)
            {
                return "このファイルの中身は検証不可能なファイルの為、表示できません：" + ex.Message;
            }
        }

        private void getfileradio_CheckedChanged(object sender, EventArgs e)
        {
            if (!cmb_macname.Enabled)
            {
                return;
            }
            else
            {
                SelectGetFileConditions();
            }

        }

        private void AutoFecthTimer_Tick(object sender, EventArgs e)
        {
            //DownloadFile();
            longtask();
            FetchConsoleShow("自動取得タイマー稼働中", 1);
        }

        private void longtask()
        {
            lock (this.autofetchlock)
            {
                Thread.Sleep(2000);
            }
            
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoFecthTimer.Interval = int.Parse(cbx_autofetchInterval.Text) * 60000;
        }

        private void cbx_autofetch_CheckedChanged(object sender, EventArgs e)
        {
            if (cbx_autofetch.Checked)
            {
                DialogResult dr = MessageBox.Show("ファイル自動取得を開始します", "確認", MessageBoxButtons.YesNo);

                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    AutoFecthTimer.Enabled = true;
                    toolStripStatusLabel1.Text = "ファイル自動取得実行中";
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("ファイル自動取得を中止します", "確認", MessageBoxButtons.YesNo);

                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    AutoFecthTimer.Enabled = false;
                    toolStripStatusLabel1.Text = "ファイル自動取得停止中";
                }
            }
        }
    }

}
