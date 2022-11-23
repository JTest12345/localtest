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
    public partial class FmVipFetchMac : Form
    {
        string crlf = "\r\n";

        List<VipFetchMac> macList = new List<VipFetchMac>();

        Magcupini mcini;
        Mcfilesys fs;
        macconfjson mconf;

        object autofetchlock = new object();
        bool autofetch = false;

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


        public FmVipFetchMac(Magcupini mci)
        {
            InitializeComponent();

            mcini = mci;
            fs = new Mcfilesys();
            macplcConf = new PLCconf();

            //autofetch timer

            // カテゴリ
            //if (mcini.FFetchPcat[0] == "NA")
            //{
            //    // iniがNAならMagCupFolderを読込
            //    mcini.FFetchPcat = Tasks_Common.GetPcatDirName(mcini.MCDir);
            //}

            var macinfolist = new List<MacInfo>();
            foreach (var plantcd in mcini.FFetchMacName)
            {
                var macinfo = new MacInfo()
                {
                    plantcd = plantcd,
                    ftpenable = false
                };
                macinfolist.Add(macinfo);
            }

            var macs = new VipFetchMac()
            {
                MacCat = mcini.FFetchPcat[0],
                Macinfo = macinfolist
            };

            macList.Add(macs);

            var procNum = macList.Count;
            var treeNode = new TreeNode[procNum];

            for (int i = 0; i < procNum; i++)
            {
                var macNum = macList[i].Macinfo.Count;
                var treeNode_mac = new TreeNode[macNum];
                for (int l = 0; l < macNum; l++)
                {
                    treeNode_mac[l] = new TreeNode(macList[i].Macinfo[l].plantcd);
                    treeNode_mac[l].ForeColor = System.Drawing.Color.Red;
                }

                treeNode[i] = new TreeNode(macList[i].MacCat, treeNode_mac);
                treeNode[i].ForeColor = System.Drawing.Color.Black;
            }
            macTreeList.Nodes.Clear();
            macTreeList.Nodes.AddRange(treeNode);
            macTreeList.ExpandAll();

            //cbx_autofetchInterval.SelectedIndex = 2;
            cbx_autofetchInterval.Text = mcini.AutoTimerSpan.ToString();
        }


        bool _bDoubleClicked;

        private void treeView1_Click(object sender, TreeNodeMouseClickEventArgs e)
        {
            // 必要であれば下記をフォームに変更して情報表示する
            //MessageBox.Show(macTreeList.SelectedNode.Text);
            foreach (var macinfo in macList[0].Macinfo)
            {
                if (macinfo.plantcd == macTreeList.SelectedNode.Text)
                {
                    if (macinfo.ftpenable)
                    {
                        macinfo.ftpenable = false;
                        macTreeList.SelectedNode.ForeColor = System.Drawing.Color.Red;
                        MessageBox.Show(macTreeList.SelectedNode.Text + "を無効にします");
                    }
                    else
                    {
                        macinfo.ftpenable = true;
                        macTreeList.SelectedNode.ForeColor = System.Drawing.Color.Black;
                        MessageBox.Show(macTreeList.SelectedNode.Text + "を有効にします");
                    }
                }
            }
        }

        private void treeView1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Clicks == 2)
            {
                _bDoubleClicked = true;
            }
            else
            {
                _bDoubleClicked = false;
            }
        }
        private void treeView1_BeforeExpand(object sender, TreeViewCancelEventArgs e)
        {
            if (_bDoubleClicked == true)
            {
                e.Cancel = true;
                _bDoubleClicked = false;
            }
        }

        private void treeView1_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
        {
            if (_bDoubleClicked == true)
            {
                e.Cancel = true;
                _bDoubleClicked = false;
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

        public void DownloadFile(string pcat, string macname)
        {
            lock (this.autofetchlock)
            {
                
                SelectGetFileConditions(pcat, macname);
                getPlcInfo("default");

                ipaddress = macplcConf.ipa;
                ftpuser = macplcConf.ftps[0].id;
                ftpupassword = macplcConf.ftps[0].password;
                ftpport = int.Parse(macplcConf.ftps[0].port);
                ftphomefolder = macplcConf.ftps[0].homedir;
                maccat = pcat;
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

                    downloadWithScript(macplcConf);
                    //FetchConsoleShow("ftpuser: " + ftpuser, 1);
                    //FetchConsoleShow("password: " + ftpupassword, 1);
                    //FetchConsoleShow("", 1);

                }
                catch (Exception e)
                {
                    FetchConsoleShow(e.Message, 5);
                }
            }
        }

        public async void downloadWithScript(PLCconf macplcConf)
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
                if (!CommonFuncs.ReadTextFileLine(wippath + @"\log.txt", ref loglst, enc))
                {
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
                    }
                }
            }
            catch (Exception ex)
            {
                FetchConsoleShow(ex.Message, 5);
            }
        }


        private void SelectGetFileConditions(string pcat, string macname)
        {
            try
            {
                btn_fetchfile.Enabled = false;

                //ConfigJson読込関数
                string MacFld = mcini.MCDir + "\\" + pcat + "\\" + macname;
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
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
            AutoFecthTimer.Interval = int.Parse(cbx_autofetchInterval.Text) * 1000;
        }


        private void btn_AutoFetchFile_Click(object sender, EventArgs e)
        {
            if (!autofetch)
            {
                DialogResult dr = MessageBox.Show("ファイル自動取得を開始します", "確認", MessageBoxButtons.YesNo);

                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    AutoFecthTimer.Enabled = true;
                    toolStripStatusLabel1.Text = "ファイル自動取得実行中";
                    btn_AutoFetchFile.Text = "自動ファイル取得中止";
                    autofetch = true;
                }
            }
            else
            {
                DialogResult dr = MessageBox.Show("ファイル自動取得を中止します", "確認", MessageBoxButtons.YesNo);

                if (dr == System.Windows.Forms.DialogResult.Yes)
                {
                    AutoFecthTimer.Enabled = false;
                    toolStripStatusLabel1.Text = "ファイル自動取得停止中";
                    btn_AutoFetchFile.Text = "自動ファイル取得開始";
                    autofetch = false;
                }
            }
        }

        private void btn_fetchfile_Click(object sender, EventArgs e)
        {
            btn_fetchfile.Enabled = false;
            foreach (var macInfo in macList[0].Macinfo)
            {
                if (macInfo.ftpenable)
                {
                    DownloadFile(macList[0].MacCat, macInfo.plantcd);
                }
            }

            btn_fetchfile.Enabled = true;
        }

        private void AutoFecthTimer_Tick(object sender, EventArgs e)
        {
            FetchConsoleShow("自動取得タイマー稼働中", 1);
            foreach (var macInfo in macList[0].Macinfo)
            {
                if (macInfo.ftpenable)
                {
                    DownloadFile(macList[0].MacCat, macInfo.plantcd);
                }
            }
        }

        private void cbx_autofetchInterval_SelectedIndexChanged(object sender, EventArgs e)
        {
            AutoFecthTimer.Interval = int.Parse(cbx_autofetchInterval.Text) * 1000;
        }
    }

}