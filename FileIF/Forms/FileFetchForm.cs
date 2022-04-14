using System;
using System.IO;
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

namespace FileIf
{
    public partial class FileFetchForm : Form
    {
        Magcupini mcini;
        Mcfilesys fs;
        macconfjson mconf;

        string crlf = "\r\n";
 
        //選択PLC情報
        string SelectedPlcName;
        PLCconf macplcConf;

        public FileFetchForm(Magcupini mci)
        {
            mcini = mci;
            fs = new Mcfilesys();
            macplcConf = new PLCconf();

            InitializeComponent();

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
                mcini.FFetchPcat = GetPcatDirName(mcini.MCDir);
            }

            if (mcini.FFetchPcat.Length != 0)
            {
                cmb_pcat.Enabled = true;
                cmb_pcat.Items.AddRange(mcini.FFetchPcat);
                // 初期表示の設定
                cmb_pcat.Text = "選択してください";
            }
            else
            {
                fetchConsole.Text += "工程カテゴリの読込ができません" + "\r\n";
                return;
            }

            // 設備名
            if (mcini.FFetchMacName[0] != "NA")
            {
                if (mcini.FFetchPcat.Length != 0)
                {
                    cmb_macname.Enabled = true;
                    cmb_macname.Items.AddRange(mcini.FFetchMacName);
                }
                else
                {
                    fetchConsole.Text += "設備名の読込ができません" + "\r\n";
                    return;
                }
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

        public void DownloadFile()
        {
            try
            {
                if (macplcConf.ipa == null)
                {
                    FetchConsoleShow("PLCの設定が正しく行われていないようです", 5);
                    return;
                }

                var ipaddress = macplcConf.ipa;
                var ftpuser = macplcConf.ftps[0].id;
                var ftpupassword = macplcConf.ftps[0].password;
                var ftpport = int.Parse(macplcConf.ftps[0].port);
                var ftphomefolder = macplcConf.ftps[0].homedir;
                var magcupdir = mcini.MCDir;
                var maccat = cmb_pcat.Text;
                var macname = cmb_macname.Text;

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
                    var lastTSFile = new FtpListItem();

                    foreach (FtpListItem item in client.GetListing(ftphomefolder))
                    {
                        if (item.Type == FtpFileSystemObjectType.File)
                        {

                            long size = client.GetFileSize(item.FullName);

                            DateTime time = client.GetModifiedTime(item.FullName);
                            if (time > lastTimeStamp)
                            {
                                lastTimeStamp = time;
                                lastTSFile = item;
                            }

                            // 指定ファイルダウンロード処理
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

                    // 最新ファイルダウンロード処理
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

                    // FTPサーバー側ワーキングフォルダのクリーン
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
            catch (Exception e)
            {
                FetchConsoleShow(e.Message, 5);
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

                contents += "◆機種名：" + vipcontents["機種名抜出し"] + crlf;
                contents += "◆ロットNO：" + vipcontents["Lot名抜出し"] + crlf;
                contents += "◆最終記録時間：" + vipcontents["年"] + "/"
                                                + vipcontents["月"] + "/"
                                                + vipcontents["日"] + " "
                                                + vipcontents["時"] + ":"
                                                + vipcontents["分"] + ":"
                                                + vipcontents["秒"] + crlf;
                contents += "◆BIN1：" + vipcontents["BIN1"] + crlf;
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

    }

}
