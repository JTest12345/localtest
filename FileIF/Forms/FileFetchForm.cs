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
            btn_submitdata.Enabled = false;
            txt_fetchresults.Enabled = false;

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
                txt_fetchresults.Text += "工程カテゴリの読込ができません" + "\r\n";
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
                    txt_fetchresults.Text += "設備名の読込ができません" + "\r\n";
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
                    txt_fetchresults.Text += "設備名の読込ができません" + "\r\n";
                    return;
                }
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
                var ipaddress = macplcConf.ipa;
                var ftpuser = macplcConf.ftps[0].id;
                var ftpupassword = macplcConf.ftps[0].password;
                var ftpport = int.Parse(macplcConf.ftps[0].port);

                using (var ftp = new FtpClient(ipaddress, ftpuser, ftpupassword))
                {
                    ftp.Port = ftpport;
                    ftp.ConnectTimeout = 5000;
                    ftp.Connect();

                    var filelist = ftp.GetListing("tmp");

                    // download a file and ensure the local directory is created
                    if (ftp.DownloadFile(@"C:\Oskas\debug\magcupresorces\mag\00002_bto_ftp.csv", @"tmp\00002_bto.csv") != FtpStatus.Success)
                    // download a file and ensure the local directory is created, verify the file after download
                    // ftp.DownloadFile(@"D:\Github\FluentFTP\README.md", "/public_html/temp/README.md", FtpLocalExists.Overwrite, FtpVerify.Retry);
                    {
                        MessageBox.Show("失敗してます");
                    }

                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }

        }

        private void btn_fetchfile_Click(object sender, EventArgs e)
        {
            DownloadFile();
        }

        private void cmb_macname_SelectionChangeCommitted(object sender, EventArgs e)
        {
            try
            {
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

                if (filekeylist.ToArray().Length != 0)
                {
                    cmb_fetchfile.Enabled = true;
                    cmb_fetchfile.Items.AddRange(filekeylist.ToArray());
                    cmb_fetchfile.Text = "選択してください";
                }
                else
                {
                    txt_fetchresults.Text += "設備名の読込ができません" + "\r\n";
                    return;
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            
            
        }

        private void cmb_pcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            var macnamearr = GetPcatDirName(mcini.MCDir + "\\" + cmb_pcat.Text);

            if (macnamearr.Length != 0)
            {
                cmb_macname.Enabled = true;
                cmb_macname.Items.AddRange(macnamearr);
                cmb_macname.Text = "選択してください";
            }
            else
            {
                txt_fetchresults.Text += "設備名の読込ができません" + "\r\n";
            }
        }

        private void cmb_fetchfile_SelectedIndexChanged(object sender, EventArgs e)
        {
            //該当PLCの名称(ID)抽出
            foreach (var item in mconf.Mcfs.mcfconfs)
            {
                if (item.foi.serverpull && item.foi.pulltype == "FTP")
                {
                    if (item.mcfilekey == cmb_fetchfile.SelectedItem.ToString())
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
    }

}
