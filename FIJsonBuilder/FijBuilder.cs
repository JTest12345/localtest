using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace FIFJsonBuilder
{
    public partial class fm_main : Form
    {
        //読み込むJsonファイル格納用
        macconfjson mcconf;
        //入力補助フォーム
        private string sharestring = "";
        fm_input fmin;
        //json path
        //string filePath = "C:\\magcupdir\\DB\\M00001\\conf\\macconf.json";
        //mcdir
        string mcdir = "C:\\oskas\\magcupdir";
        //TextChangeEventLock
        bool TceLock = true;
        //MagCupDirの設定ファイル検索用LIST
        List<string> confpath;

        public fm_main()
        {
            InitializeComponent();
            StartPosition = FormStartPosition.CenterScreen;

            //入力補助用フォーム
            fmin = new fm_input();
            fmin.fmmain = this;

            //MagCupDirの設定ファイル検索
            confpath = new List<string>();
            DirSearch(mcdir, ref confpath);
            string chkaddedcat = "";
            foreach (var path in confpath)
            {
                string[] sppath = path.Split(Path.DirectorySeparatorChar);
                // 設備CatコンボボックスにADD
                int indexofmagcup = Array.IndexOf(sppath, "magcupdir", 0);
                if (!chkaddedcat.Contains(sppath[indexofmagcup + 1]))
                    cb_mcat.Items.Add(sppath[indexofmagcup + 1]);
                    chkaddedcat += sppath[indexofmagcup + 1];
                // 設備NoコンボボックスにADD
                if (sppath[indexofmagcup + 1] == cb_mcat.Items[0].ToString())
                {
                    cb_macno.Items.Add(sppath[indexofmagcup + 2]);
                }
            }

            //初期表示CONF
            cb_mcat.SelectedIndex = 0;
            cb_macno.SelectedIndex = 0;

            //ConfigJson読込
            string FilePath = mcdir + @"\" + cb_mcat.Items[0].ToString() + @"\" + cb_macno.Items[0].ToString() + @"\" + @"conf\macconf.json";
            mcconf = JsonConvert.DeserializeObject<macconfjson>(JsonFileReader(FilePath));

            ////////////////////////////////////////////////////////////////////////////
            /// UIメンバが増えた時にするべき以下のこと
            /// ① シグナルの設定
            /// ② スロットに処理追加
            /// ③ 表示処理****()にメンバ追加
            /// ④ 初期化処理****null()にメンバ追加
            /// ⑤ ShareStringに初期化処理追加
            /// ⑥ 数値入力ならchkints()に追加
            /// ⑦ NULLがだめならchknull()に追加
            ////////////////////////////////////////////////////////////////////////////

            ////////
            // ① //
            ////////
            // 個別のスロット②は当然書いてね。
            //****************************************************************************************
            //タブコントロール変更用シグナル・スロット
            tb_main.SelectedIndexChanged += new EventHandler(tabindexChangedRouter);

            //工程カテゴリコンボボックス変更シグナル・スロット
            cb_mcat.SelectedIndexChanged += new EventHandler(cb_mcat_SelectedIndexChanged);

            //リストボックスアイテム変更用シグナル・スロット
            //PLC
            lb_plc.SelectedIndexChanged += new EventHandler(lb_plc_SelectedIndexChanged);
            lb_plc_dev.SelectedIndexChanged += new EventHandler(lb_plc_dev_SelectedIndexChanged);
            lb_plc_sharefd.SelectedIndexChanged += new EventHandler(lb_plc_sharefolder_SelectedIndexChanged);

            //設備PC
            lb_pc.SelectedIndexChanged += new EventHandler(lb_pc_SelectedIndexChanged);
            lb_pc_sharefd.SelectedIndexChanged += new EventHandler(lb_pc_sharefolder_SelectedIndexChanged);

            //IOファイル
            lb_mcfilekey.SelectedIndexChanged += new EventHandler(lb_mcf_SelectedIndexChanged);

            //設備情報
            lb_mstsfkey.SelectedIndexChanged += new EventHandler(lb_mstsf_SelectedIndexChanged);

            //****************************************************************************************
            // 共通スロットsavedata2mcconfniを追記！
            //テキストボックス・コンボボックス変更用シグナル・スロット
            //共通
            cb_mcat.TextChanged += new EventHandler(savedata2mcconf);
            cb_macno.TextChanged += new EventHandler(readconffile);
            //chk_useplc.CheckedChanged += new EventHandler(savedata2mcconf);
            //chk_usepc.CheckedChanged += new EventHandler(savedata2mcconf);

            //PLC
            tb_plc_name.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_ipa.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_devport.TextChanged += new EventHandler(savedata2mcconf);
            //chk_plc_usemc.CheckedChanged += new EventHandler(savedata2mcconf);
            //chk_plc_usemm.CheckedChanged += new EventHandler(savedata2mcconf);
            chk_plc_useftpsv.CheckedChanged += new EventHandler(savedata2mcconf);
            tb_plc_ftpport.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_ftpid.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_ftppass.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_ftphomedir.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_devid.TextChanged += new EventHandler(savedata2mcconf);
            cb_plc_devtype.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_devno.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_sharefdname.TextChanged += new EventHandler(savedata2mcconf);
            tb_plc_sharefdpath.TextChanged += new EventHandler(savedata2mcconf);

            //設備PC
            tb_pc_name.TextChanged += new EventHandler(savedata2mcconf);
            tb_pc_ipa.TextChanged += new EventHandler(savedata2mcconf);
            //chk_pc_usemc.CheckedChanged += new EventHandler(savedata2mcconf);
            //chk_pc_usemm.CheckedChanged += new EventHandler(savedata2mcconf);
            chk_pc_useftpsv.CheckedChanged += new EventHandler(savedata2mcconf);
            tb_pc_ftpport.TextChanged += new EventHandler(savedata2mcconf);
            tb_pc_ftpid.TextChanged += new EventHandler(savedata2mcconf);
            tb_pc_ftppass.TextChanged += new EventHandler(savedata2mcconf);
            tb_pc_ftphomedir.TextChanged += new EventHandler(savedata2mcconf);
            tb_pc_sharefdname.TextChanged += new EventHandler(savedata2mcconf);
            tb_pc_sharefdpath.TextChanged += new EventHandler(savedata2mcconf);

            //IOファイル
            tb_mcfilekey.TextChanged += new EventHandler(savedata2mcconf);
            cb_mcfile_enc.TextChanged += new EventHandler(savedata2mcconf);
            tb_mcfile_return.TextChanged += new EventHandler(savedata2mcconf);
            chk_mcfile_disableEndfile.CheckedChanged += new EventHandler(savedata2mcconf);
            chk_mcfile_verifiparam.CheckedChanged += new EventHandler(savedata2mcconf);
            chk_mcfile_sp1.CheckedChanged += new EventHandler(savedata2mcconf);
            chk_mcfile_useplcdev.CheckedChanged += new EventHandler(mcfedited);
            cb_mcf_plcdev.TextChanged += new EventHandler(savedata2mcconf);
            chk_mcf_serverpull.CheckedChanged += new EventHandler(mcfedited);
            cb_mcf_cnttype.TextChanged += new EventHandler(mcfedited);
            cb_mcf_cntid.TextChanged += new EventHandler(mcfedited);
            cb_mcf_pulltype.TextChanged += new EventHandler(mcfedited);
            cb_mcf_shfld.TextChanged += new EventHandler(mcfedited);
            tb_mcf_path.TextChanged += new EventHandler(mcfedited);

            //設備情報ファイル
            tb_mstsfkey.TextChanged += new EventHandler(savedata2mcconf);
            cb_mstsfid.TextChanged += new EventHandler(savedata2mcconf);
            cb_mstsf_inttimfetch.TextChanged += new EventHandler(savedata2mcconf);
            chk_mstsf_serverpull.CheckedChanged += new EventHandler(mstsfedited);
            cb_mstsf_cnttype.TextChanged += new EventHandler(mstsfedited);
            cb_mstsf_cntid.TextChanged += new EventHandler(mstsfedited);
            cb_mstsf_pulltype.TextChanged += new EventHandler(mstsfedited);
            cb_mstsf_shfld.TextChanged += new EventHandler(mstsfedited);


            //保存先表示
            lb_confpath.Text = lb_confpath.Text + mcdir + @"\" + cb_mcat.Items[0].ToString() + @"\" + cb_macno.Items[0].ToString() + @"\" + @"conf\macconf.json";

            //mcconf表示
            dispMcconf(); // リストボックス表示

            //mcconf書き込みロック
            TceLock = false;
        }


        /////////////////////////////
        // 設定保存
        /////////////////////////////
        private void bt_svmcj_Click(object sender, EventArgs e)
        {
            TceLock = true;
            try
            {
                string json = JsonConvert.SerializeObject(mcconf, Formatting.Indented);
                string mes = "設備CAT：" + mcconf.Mchd.mcat + " " + "\r\n設備No：" + mcconf.Mchd.macno + "\r\n";
                string FilePath = mcdir + @"\\" + mcconf.Mchd.mcat + @"\\" + mcconf.Mchd.macno + @"\\" + @"conf\macconf.json";
                JsonFileWriter(FilePath, json, mes);
            }
            catch (Exception ex)
            {
                //処理追加！
            }
            TceLock = true;
        }


        /////////////////////////////
        // 設定読み込み
        /////////////////////////////
        //private void bt_rdmcj_Click(object sender, EventArgs e)
        private void readconffile(object sender, EventArgs e)
        {
            try
            {
                TceLock = true;
                string mes = "設備CAT：" + cb_mcat.Text + " " + "\r\n設備No：" + cb_macno.Text + "\r\n";
                string FilePath = mcdir + @"\\" + cb_mcat.Text + @"\\" + cb_macno.Text + @"\\" + @"conf\macconf.json";
                DialogResult result = MessageBox.Show(mes + "から設定ファイルを読込みます", "確認",
                                                        MessageBoxButtons.YesNo,
                                                        MessageBoxIcon.Exclamation,
                                                        MessageBoxDefaultButton.Button1
                                                     );
                if (result == DialogResult.Yes)
                {
                      //////////
                     // ③④ //
                    //////////
                    comconfnull();
                    plcconfnull();
                    devconfnull();
                    pcconfnull();
                    pcshfdconfnull();
                    Mcfconfnull();
                    Mstsfconfnull();
                    mcconf = JsonConvert.DeserializeObject<macconfjson>(JsonFileReader(FilePath));
                    //保存先表示
                    lb_confpath.Text = "設定ファイル保存先：" + FilePath;
                    //
                }
                TceLock = false;
                dispMcconf(); // mcconf表示
            }
            catch (Exception ex)
            {
                //処理追加！
            }
        }


        /////////////////////////////
        // フォームクローズ
        /////////////////////////////
        private void bt_closefm_Click(object sender, EventArgs e)
        {
            this.Close();
        }


        ////////
        // ⑤ //
        ////////

        /////////////////////////////////////////////////////////////////////////////////////
        /// インプットフォームとのクリップボード関数
        /// 
        ///  ◆◆◆◆◆◆このコードは本当に必要？？？？？◆◆◆◆◆◆
        /// 
        /// 
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 入力補助用フォームのデータ格納用
        /// </summary>
        public string ShareString
        {
            set
            {
                sharestring = value;
                string[] words = sharestring.Split(',');
                if (words[1] != "")
                {
                    if (words[0] == "plc")
                    {
                        mcconf.Plcs.plcconfs.Add(new PLCconf
                        {
                            bender = "",
                            model = "",
                            name = words[1],
                            ipa = "",
                            devport = "",
                            //usemc = false,
                            //usemm = false,
                            devs = new devs(),
                            ftps = new ftps()
                        });
                        mcconf.Plcs[mcconf.Plcs.plcconfs.Count - 1].ftps.ftpconfs.Add(new ftpconf { id = "", password = "", port = "" });
                        lb_plc.Items.Add(words[1]);
                        lb_plc.SelectedIndex = lb_plc.Items.Count - 1;
                    }
                    else if (words[0] == "plcdev")
                    {
                        mcconf.Plcs[lb_plc.SelectedIndex].devs.devconfs.Add(new devconf
                        {
                            devid = words[1],
                            devno = "",
                            devtype = ""
                        });
                        lb_plc_dev.Items.Add(words[1]);
                        lb_plc_dev.SelectedIndex = lb_plc_dev.Items.Count - 1;
                    }
                    else if (words[0] == "plcshfd")
                    {
                        mcconf.Plcs[lb_plc.SelectedIndex].shfld.sfconf.Add(new shfldconf
                        {
                            name = words[1],
                            path = ""
                        });
                        lb_plc_sharefd.Items.Add(words[1]);
                        lb_plc_sharefd.SelectedIndex = lb_plc_sharefd.Items.Count - 1;
                    }
                    else if (words[0] == "pc")
                    {
                        mcconf.Pcs.pcconfs.Add(new PCconf
                        {
                            name = words[1],
                            ipa = "",
                            //usemc = false,
                            //usemm = false,
                            shfld = new Shareflds(),
                            ftps = new ftps()
                        });
                        mcconf.Pcs[mcconf.Pcs.pcconfs.Count - 1].ftps.ftpconfs.Add(new ftpconf { id = "", password = "", port = "" });
                        lb_pc.Items.Add(words[1]);
                        lb_pc.SelectedIndex = lb_pc.Items.Count - 1;
                    }
                    else if (words[0] == "pcshfd")
                    {
                        mcconf.Pcs[lb_pc.SelectedIndex].shfld.sfconf.Add(new shfldconf
                        {
                            name = words[1],
                            path = ""
                        });
                        lb_pc_sharefd.Items.Add(words[1]);
                        lb_pc_sharefd.SelectedIndex = lb_pc_sharefd.Items.Count - 1;
                    }
                    else if (words[0] == "mcf")
                    {
                        mcconf.Mcfs.mcfconfs.Add(new MCFconf
                        {
                            mcfilekey = words[1],
                            encoding = "",
                            returns = "",
                            spfnc1 = false,
                            verifiparam = false,
                            foi = new fileOwnerinfo
                            {
                                serverpull = false,
                                cnttype = "",
                                cntid = "",
                                pulltype = "",
                                shfld = "",
                                inttimefetch = "",
                                devid = "",
                                useplcdev = false
                            }
                        });
                        lb_mcfilekey.Items.Add(words[1]);
                        lb_mcfilekey.SelectedIndex = lb_mcfilekey.Items.Count - 1;
                    }
                    else if (words[0] == "mstsf")
                    {
                        mcconf.Mstsfs.mstsfconfs.Add(new MSTSFconf
                        {
                            mstsfilekey = words[1],
                            mstsfileid = "",
                            foi = new fileOwnerinfo
                            {
                                serverpull = false,
                                cnttype = "",
                                cntid = "",
                                pulltype = "",
                                shfld = "",
                                inttimefetch = ""
                            }
                        });
                        lb_mstsfkey.Items.Add(words[1]);
                        lb_mstsfkey.SelectedIndex = lb_mstsfkey.Items.Count - 1;
                    }
                }
            }
            get
            {
                return sharestring;
            }
        }


          //////////
         // ③④ //
        //////////

        /////////////////////////////////////////////////////////////////////////////////////
        /// mcconf表示関数
        /////////////////////////////////////////////////////////////////////////////////////
        private void dispMcconf()
        {
            //Tab: 共通設定
            //cb_mcat.Text = mcconf.Mchd.mcat;
            //cb_macno.Text = mcconf.Mchd.macno;
            //chk_useplc.Checked = mcconf.Mchd.mcuseplc;
            //chk_usepc.Checked = mcconf.Mchd.mcusepc;

            //Tab: PLC設定
            lb_plc.Items.Clear();
            if (mcconf.Plcs != null)
            {
                if (mcconf.Plcs.plcconfs.Count > 0)
                {
                    for (int i = 0; i < mcconf.Plcs.plcconfs.Count; i++)
                    {
                        lb_plc.Items.Add(mcconf.Plcs.plcconfs[i].name);
                    }
                    lb_plc.SelectedIndex = 0;
                }
            }
            else
            {
                mcconf.Plcs = new PLCs();
            }

            //Tab: 設備PC設定
            lb_pc.Items.Clear();
            if (mcconf.Pcs != null)
            {
                if (mcconf.Pcs.pcconfs.Count > 0)
                {
                    for (int i = 0; i < mcconf.Pcs.pcconfs.Count; i++)
                    {
                        lb_pc.Items.Add(mcconf.Pcs.pcconfs[i].name);
                    }
                    lb_pc.SelectedIndex = 0;
                }
            }
            else
            {
                mcconf.Pcs = new PCs();
            }

            //Tab: MagCupファイル設定
            lb_mcfilekey.Items.Clear();
            if (mcconf.Mcfs != null)
            {
                if (mcconf.Mcfs.mcfconfs.Count > 0)
                {
                    for (int i = 0; i < mcconf.Mcfs.mcfconfs.Count; i++)
                    {
                        lb_mcfilekey.Items.Add(mcconf.Mcfs.mcfconfs[i].mcfilekey);
                    }
                    lb_mcfilekey.SelectedIndex = 0;
                }
            }
            else
            {
                mcconf.Mcfs = new MCFs();
            }

            //Tab: 設備情報ファイル設定
            lb_mstsfkey.Items.Clear();
            if (mcconf.Mstsfs != null)
            {
                if (mcconf.Mstsfs.mstsfconfs.Count > 0)
                {
                    for (int i = 0; i < mcconf.Mstsfs.mstsfconfs.Count; i++)
                    {
                        lb_mstsfkey.Items.Add(mcconf.Mstsfs.mstsfconfs[i].mstsfilekey);
                    }
                    lb_mstsfkey.SelectedIndex = 0;
                }
            }
            else
            {
                mcconf.Mstsfs = new MSTSFs();
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////
        /// タブ変更時のルーター(特殊なデータがあれば追加)
        /////////////////////////////////////////////////////////////////////////////////////
        private void tabindexChangedRouter(Object sender, EventArgs e)
        {
            /*
            if (tb_main.SelectedIndex == 3)
            {
                cb_mcf_plcdev_update();
            }
            */
        }



        /////////////////////////////////////////////////////////////////////////////////////
        /// Tab: 共通設定
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 工程カテゴリコンボボックス変更スロット
        /// </summary>
        private void cb_mcat_SelectedIndexChanged(object sender, EventArgs e)
        {
            cb_macno.Items.Clear();
            foreach (var path in confpath)
            {
                string[] sppath = path.Split(Path.DirectorySeparatorChar);
                // 設備NoコンボボックスにADD
                if (sppath[3] == cb_mcat.Items[cb_mcat.SelectedIndex].ToString())
                {
                    cb_macno.Items.Add(sppath[4]);
                }
            }
            //cb_macno.SelectedIndex = 0;
        }


        private void comconfnull()
        {
            //chk_useplc.Checked = false;
            //chk_usepc.Checked = false;
        }

        /////////////////////////////////////////////////////////////////////////////////////
        /// Tab: PLC設定
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PLCリストボックス内のアイテムが変更された時にタブ内の表示を変更します
        /// </summary>
        /// <param name="plcconf"></param>
        private void plcconf(PLCconf plcconf)
        {
            tb_plc_name.Text = plcconf.name;
            tb_plc_ipa.Text = plcconf.ipa;
            tb_plc_devport.Text = plcconf.devport;
            //chk_plc_usemc.Checked = plcconf.usemc;
            //chk_plc_usemm.Checked = plcconf.usemm;
            chk_plc_useftpsv.Checked = plcconf.ftps[0].useftpsv;
            tb_plc_ftpid.Text = plcconf.ftps[0].id;
            tb_plc_ftppass.Text = plcconf.ftps[0].password;
            tb_plc_ftpport.Text = plcconf.ftps[0].port;
            tb_plc_ftphomedir.Text = plcconf.ftps[0].homedir;
            for (int i = 0; i < plcconf.devs.devconfs.Count; i++)
            {
                lb_plc_dev.Items.Add(plcconf.devs.devconfs[i].devid);
            }
            if (lb_plc_dev.Items.Count > 0)
            {
                lb_plc_dev.SelectedIndex = 0;
            }
            else
            {
                devconfnull();
            }
            for (int i = 0; i < plcconf.shfld.sfconf.Count; i++)
            {
                lb_plc_sharefd.Items.Add(plcconf.shfld.sfconf[i].name);
            }
            if (lb_plc_sharefd.Items.Count > 0)
            {
                lb_plc_sharefd.SelectedIndex = 0;
            }
            else
            {
                plcshfdconfnull();
            }
        }

        /// <summary>
        /// PLCタブの表示をクリアします（PLCリストボックス以外）
        /// </summary>
        private void plcconfnull()
        {
            tb_plc_name.Text = "";
            tb_plc_ipa.Text = "";
            tb_plc_devport.Text = "";
            //chk_plc_usemc.Checked = false;
            //chk_plc_usemm.Checked = false;
            chk_plc_useftpsv.Checked = false;
            tb_plc_ftpid.Text = "";
            tb_plc_ftppass.Text = "";
            tb_plc_ftpport.Text = "";
            tb_plc_ftphomedir.Text = "";
            lb_plc_dev.Items.Clear();
            devconfnull();
            lb_plc_sharefd.Items.Clear();
            plcshfdconfnull();
        }

        /// <summary>
        /// PLCのデバイスリストのアイテムが変更になったときにデバイス内容を変更します
        /// </summary>
        /// <param name="dvconf"></param>
        private void devconf(devconf dvconf)
        {
            tb_plc_devid.Text = dvconf.devid;
            tb_plc_devno.Text = dvconf.devno;
            cb_plc_devtype.Text = dvconf.devtype;
            
        }

        /// <summary>
        /// PLCのデバイスリストのアイテムがない場合の空表示
        /// </summary>
        private void devconfnull()
        {
            tb_plc_devid.Text = "";
            tb_plc_devno.Text = "";
            cb_plc_devtype.Text = "";
        }

        /// <summary>
        /// PCのシェアフォルダのアイテムが変更になったときにデバイス内容を変更します
        /// </summary>
        /// <param name="sfconf"></param>
        private void plcshfdconf(shfldconf sfconf)
        {
            tb_plc_sharefdname.Text = sfconf.name;
            tb_plc_sharefdpath.Text = sfconf.path;
        }

        /// <summary>
        /// PCのシェアフォルダのアイテムがない場合の空表示
        /// </summary>
        private void plcshfdconfnull()
        {
            tb_plc_sharefdname.Text = "";
            tb_plc_sharefdpath.Text = "";
        }

        /// <summary>
        /// PLC追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_plc_Click(object sender, EventArgs e)
        {
            sharestring = "plc";
            fmin.Show();
            fmin.Text = "PLCの名称(ID)を入力してください";
        }

        /// <summary>
        /// PLCの削除[-]
        /// </summary>
        private void bt_rm_plc_Click(object sender, EventArgs e)
        {
            if (lb_plc.SelectedIndex>-1)
            {
                mcconf.Plcs.plcconfs.RemoveAt(lb_plc.SelectedIndex);
                lb_plc.Items.Remove(lb_plc.SelectedItem);
                plcconfnull();
            }
        }

        /// <summary>
        /// PLCデバイス追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_plcdev_Click(object sender, EventArgs e)
        {
            sharestring = "plcdev";
            fmin.Show();
            fmin.Text = "PLCデバイスの名称(ID)を入力してください";
        }

        /// <summary>
        /// PLCデバイス削除[-]
        /// </summary>
        private void bt_rm_plcdev_Click(object sender, EventArgs e)
        {
            if (lb_plc_dev.SelectedIndex > -1)
            {
                mcconf.Plcs[lb_plc.SelectedIndex].devs.devconfs.RemoveAt(lb_plc_dev.SelectedIndex);
                lb_plc_dev.Items.Remove(lb_plc_dev.SelectedItem);
                devconfnull();
            }
        }

        /// <summary>
        /// PLCリストアイテム変更スロット
        /// </summary>
        private void lb_plc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_plc.SelectedIndex > -1)
            {
                lb_plc_dev.Items.Clear();
                lb_plc_sharefd.Items.Clear();
                plcconf(mcconf.Plcs.plcconfs[lb_plc.SelectedIndex]);
            }
            else
            {
                tb_plc_name.Text = "";
            }
            TceLock = false;
        }

        /// <summary>
        /// PLC共有フォルダ追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_plcsharefd_Click(object sender, EventArgs e)
        {
            sharestring = "plcshfd";
            fmin.Show();
            fmin.Text = "設備PLCの共有フォルダ名称(ID)を入力してください";
        }

        /// <summary>
        /// PLC共有フォルダ削除[-]
        /// </summary>
        private void bt_rm_plcsharefd_Click(object sender, EventArgs e)
        {
            if (lb_plc_sharefd.SelectedIndex > -1)
            {
                mcconf.Plcs[lb_plc.SelectedIndex].shfld.sfconf.RemoveAt(lb_plc_sharefd.SelectedIndex);
                lb_plc_sharefd.Items.Remove(lb_plc_sharefd.SelectedItem);
                plcshfdconfnull();
            }
        }

        /// <summary>
        /// PLCデバイスリスト変更スロット
        /// </summary>
        private void lb_plc_dev_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_plc_dev.SelectedIndex > -1)
            {
                devconf(mcconf.Plcs.plcconfs[lb_plc.SelectedIndex].devs.devconfs[lb_plc_dev.SelectedIndex]);
            }
            TceLock = false;
        }

        /// <summary>
        /// PLC共有フォルダリスト変更スロット
        /// </summary>
        private void lb_plc_sharefolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_plc_sharefd.SelectedIndex > -1)
            {
                plcshfdconf(mcconf.Plcs.plcconfs[lb_plc.SelectedIndex].shfld.sfconf[lb_plc_sharefd.SelectedIndex]);
            }
            TceLock = false;
        }


        /////////////////////////////////////////////////////////////////////////////////////
        /// Tab: 設備PC設定
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// PCリストボックス内のアイテムが変更された時にタブ内の表示を変更します
        /// </summary>
        /// <param name="pcconf"></param>
        private void pcconf(PCconf pcconf)
        {
            tb_pc_name.Text = pcconf.name;
            tb_pc_ipa.Text = pcconf.ipa;
            //chk_pc_usemc.Checked = pcconf.usemc;
            //chk_pc_usemm.Checked = pcconf.usemm;
            chk_pc_useftpsv.Checked = pcconf.ftps[0].useftpsv;
            tb_pc_ftpid.Text = pcconf.ftps[0].id;
            tb_pc_ftppass.Text = pcconf.ftps[0].password;
            tb_pc_ftpport.Text = pcconf.ftps[0].port;
            tb_pc_ftphomedir.Text = pcconf.ftps[0].homedir;
            for (int i = 0; i < pcconf.shfld.sfconf.Count; i++)
            {
                lb_pc_sharefd.Items.Add(pcconf.shfld.sfconf[i].name);
            }
            if (lb_pc_sharefd.Items.Count > 0)
            {
                lb_pc_sharefd.SelectedIndex = 0;
            }
            else
            {
                pcshfdconfnull();
            }
        }

        /// <summary>
        /// PCタブの表示をクリアします
        /// </summary>
        private void pcconfnull()
        {
            tb_pc_name.Text = "";
            tb_pc_ipa.Text = "";
            //chk_pc_usemc.Checked = false;
            //chk_pc_usemm.Checked = false;
            chk_pc_useftpsv.Checked = false;
            tb_pc_ftpid.Text = "";
            tb_pc_ftppass.Text = "";
            tb_pc_ftpport.Text = "";
            tb_pc_ftphomedir.Text = "";
            lb_pc_sharefd.Items.Clear();
            pcshfdconfnull();
        }

        /// <summary>
        /// PCのシェアフォルダのアイテムが変更になったときにデバイス内容を変更します
        /// </summary>
        /// <param name="sfconf"></param>
        private void pcshfdconf(shfldconf sfconf)
        {
            tb_pc_sharefdname.Text = sfconf.name;
            tb_pc_sharefdpath.Text = sfconf.path;
        }

        /// <summary>
        /// PCのシェアフォルダのアイテムがない場合の空表示
        /// </summary>
        private void pcshfdconfnull()
        {
            tb_pc_sharefdname.Text = "";
            tb_pc_sharefdpath.Text = "";
        }
        
        /// <summary>
        /// PC追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_pc_Click(object sender, EventArgs e)
        {
            sharestring = "pc";
            fmin.Show();
            fmin.Text = "設備PCの名称(ID)を入力してください";
        }

        /// <summary>
        /// PCの削除[-]
        /// </summary>
        private void bt_rm_pc_Click(object sender, EventArgs e)
        {
            if (lb_pc.SelectedIndex > -1)
            {
                mcconf.Pcs.pcconfs.RemoveAt(lb_pc.SelectedIndex);
                lb_pc.Items.Remove(lb_pc.SelectedItem);
                pcconfnull();
            }
        }

        /// <summary>
        /// PC共有フォルダ追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_pcsharefd_Click(object sender, EventArgs e)
        {
            sharestring = "pcshfd";
            fmin.Show();
            fmin.Text = "設備PCの共有フォルダ名称(ID)を入力してください";
        }

        /// <summary>
        /// PC共有フォルダ削除[-]
        /// </summary>
        private void bt_rm_pcsharefd_Click(object sender, EventArgs e)
        {
            if (lb_pc_sharefd.SelectedIndex > -1)
            {
                mcconf.Pcs[lb_pc.SelectedIndex].shfld.sfconf.RemoveAt(lb_pc_sharefd.SelectedIndex);
                lb_pc_sharefd.Items.Remove(lb_pc_sharefd.SelectedItem);
                pcshfdconfnull();
            }
        }
        
        /// <summary>
        /// PCリストアイテム変更スロット
        /// </summary>
        private void lb_pc_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_pc.SelectedIndex > -1)
            {
                lb_pc_sharefd.Items.Clear();
                pcconf(mcconf.Pcs.pcconfs[lb_pc.SelectedIndex]);
            }
            else
            {
                tb_pc_name.Text = "";
            }
            TceLock = false;
        }
        
        /// <summary>
        /// PC共有フォルダリスト変更スロット
        /// </summary>
        private void lb_pc_sharefolder_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_pc_sharefd.SelectedIndex > -1)
            {
                pcshfdconf(mcconf.Pcs.pcconfs[lb_pc.SelectedIndex].shfld.sfconf[lb_pc_sharefd.SelectedIndex]);
            }
            TceLock = false;
        }


        /////////////////////////////////////////////////////////////////////////////////////
        /// Tab: MCファイル設定
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// MCファイルリストボックス内のアイテムが変更された時にタブ内の表示を変更します
        /// </summary>
        /// <param name="mcfconf"></param>
        private void Mcfconf(MCFconf mcfconf)
        {
            tb_mcfilekey.Text = mcfconf.mcfilekey;
            tb_mcfile_return.Text = mcfconf.returns;
            cb_mcfile_enc.Text = mcfconf.encoding;
            chk_mcfile_useplcdev.Checked = mcfconf.foi.useplcdev;
            cb_mcf_plcdev.Text = mcfconf.foi.devid;
            chk_mcfile_sp1.Checked = mcfconf.spfnc1;
            chk_mcfile_disableEndfile.Checked = mcfconf.disableEndfile;
            chk_mcfile_verifiparam.Checked = mcfconf.verifiparam;
            cb_mcf_inttimfetch.Text = mcfconf.foi.inttimefetch;
            cb_mcf_cnttype.Text = mcfconf.foi.cnttype;
            update_mcf_cbcntid();
            cb_mcf_cntid.Text = mcfconf.foi.cntid;
            update_mcf_cbpulltype();
            cb_mcf_pulltype.Text = mcfconf.foi.pulltype; 
            update_mcf_cbshfld();
            cb_mcf_shfld.Text = mcfconf.foi.shfld;
            tb_mcf_path.Text = mcfconf.foi.path;
            chk_mcf_serverpull.Checked = mcfconf.foi.serverpull;
        }

        /// <summary>
        /// MCFタブの表示をクリアします
        /// </summary>
        private void Mcfconfnull()
        {
            tb_mcfilekey.Text = "";
            tb_mcfile_return.Text = "";
            cb_mcfile_enc.Text = "";
            chk_mcfile_sp1.Checked = false;
            chk_mcfile_useplcdev.Checked = false;
            chk_mcfile_verifiparam.Checked = false;
            cb_mcf_plcdev.Items.Clear();
        }

        /// <summary>
        /// MCF追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_mcfilekey_Click(object sender, EventArgs e)
        {
            sharestring = "mcf";
            fmin.Show();
            fmin.Text = "IOファイルのキー(ID)を入力してください";
        }

        /// <summary>
        /// MCFの削除[-]
        /// </summary>
        private void bt_rm_mcfilekey_Click(object sender, EventArgs e)
        {
            if (lb_mcfilekey.SelectedIndex > -1)
            {
                mcconf.Mcfs.mcfconfs.RemoveAt(lb_mcfilekey.SelectedIndex);
                lb_mcfilekey.Items.Remove(lb_mcfilekey.SelectedItem);
                Mcfconfnull();
            }
        }

        /// <summary>
        /// MCFリストアイテム変更スロット
        /// </summary>
        private void lb_mcf_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_mcfilekey.SelectedIndex > -1)
            {
                Mcfconf(mcconf.Mcfs.mcfconfs[lb_mcfilekey.SelectedIndex]);
            }
            else
            {
                tb_mcfilekey.Text = "";
            }
            TceLock = false;
        }

        /// <summary>
        /// MCFで使用するPLCデバイスコンボボックスのアップデート関数
        /// </summary>
        private void cb_mcf_plcdev_update()
        {
            cb_mcf_plcdev.Items.Clear();
            for (int i = 0; i < mcconf.Plcs[lb_plc.SelectedIndex].devs.devconfs.Count; i++)
            {
                cb_mcf_plcdev.Items.Add(mcconf.Plcs[lb_plc.SelectedIndex].devs[i].devid);
                //if (mcconf.Mcfs.mcfconfs[lb_mcfilekey.SelectedIndex].devid == mcconf.Plcs[lb_plc.SelectedIndex].devs[i].devid)
                //    cb_mcf_plcdev.SelectedIndex = i;
                cb_mcf_plcdev.Text = mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.devid;
            }
        }

        /// <summary>(object sender, EventArgs e)
        /// MCFファイル取得条件変更スロット
        /// </summary>
        private void mcfedited(object sender, EventArgs e)
        {
            var type = sender.GetType();
            if (type.Name == "CheckBox")
            {
                //chk_mcfile_disableEndfile
                if (!chk_mcfile_disableEndfile.Checked)
                {
                    tb_mcfile_return.Enabled = true;
                    chk_mcfile_verifiparam.Enabled = true;
                    chk_mcfile_sp1.Enabled = true;
                }
                else
                {
                    tb_mcfile_return.Enabled = false;
                    chk_mcfile_verifiparam.Enabled = false;
                    chk_mcfile_sp1.Enabled = false;
                }
                //chk_mcfile_useplcdev
                if (chk_mcfile_useplcdev.Checked)
                {
                    cb_mcf_plcdev.Enabled = true;
                }
                else
                {
                    cb_mcf_plcdev.Enabled = false;
                }
                //chk_mcf_serverpull
                if (chk_mcf_serverpull.Checked)
                {
                    cb_mcf_inttimfetch.Enabled = true;
                    cb_mcf_pulltype.Enabled = true;
                    cb_mcf_shfld.Enabled = false;
                    tb_mcf_path.Enabled = true;
                }
                else
                {
                    cb_mcf_inttimfetch.Enabled = false;
                    cb_mcf_pulltype.Enabled = false;
                    cb_mcf_shfld.Enabled = false;
                    tb_mcf_path.Enabled = false;
                }
            }
            else if (type.Name == "ComboBox")
            {
                //cb_mstsf_cnttype
                if (((ComboBox)sender).Name == "cb_mcf_cnttype")
                {
                    update_mcf_cbcntid();
                }
                //cb_mstsf_cntid
                if (((ComboBox)sender).Name == "cb_mcf_cntid")
                {
                    if (cb_mcf_cnttype.Text == "PLC")
                        cb_mcf_plcdev_update();
                    update_mcf_cbpulltype();
                }
                //cb_mstsf_shfld
                if (((ComboBox)sender).Name == "cb_mcf_pulltype")
                {
                    update_mcf_cbshfld();
                }
            }

            savedata2mcconf(sender, e);

        }

        private void update_mcf_cbcntid()
        {
            cb_mcf_cntid.Items.Clear();

            if (cb_mcf_cnttype.Text == "PLC")
            {
                for (int i = 0; i < mcconf.Plcs.plcconfs.Count; i++)
                {
                    cb_mcf_cntid.Items.Add(mcconf.Plcs[i].name);
                }
            }
            else
            {
                for (int i = 0; i < mcconf.Pcs.pcconfs.Count; i++)
                {
                    cb_mcf_cntid.Items.Add(mcconf.Pcs[i].name);
                }
            }
        }

        private void update_mcf_cbpulltype()
        {
            cb_mcf_pulltype.Items.Clear();
            //tb_mcf_path.Text = "";
            if (cb_mcf_cntid.SelectedIndex > -1)
            {
                if (cb_mcf_cnttype.Text == "PLC")
                {
                    //if (mcconf.Plcs[cb_mcf_cntid.SelectedIndex].usemm)
                    if (mcconf.Plcs[cb_mcf_cntid.SelectedIndex].ftps[0].useftpsv)
                    {
                        cb_mcf_pulltype.Items.Add("FTP");
                        cb_mcf_pulltype.SelectedIndex = 0;
                    }
                    //cb_mcf_shfld.Enabled = false;
                }
                else
                {
                    if (mcconf.Pcs[cb_mcf_cntid.SelectedIndex].ftps[0].useftpsv)
                    {
                        cb_mcf_pulltype.Items.Add("FTP");
                    }
                    cb_mcf_pulltype.Items.Add("ShareFolder");
                    cb_mcf_pulltype.SelectedIndex = 0;
                }
            }
        }

        private void update_mcf_cbshfld()
        {
            cb_mcf_shfld.Items.Clear();
            if (cb_mcf_cntid.SelectedIndex > -1)
            {
                if (cb_mcf_pulltype.Text == "ShareFolder")
                {
                    for (int i = 0; i < mcconf.Pcs[cb_mcf_cntid.SelectedIndex].shfld.sfconf.Count; i++)
                    {
                        cb_mcf_shfld.Items.Add(mcconf.Pcs[cb_mcf_cntid.SelectedIndex].shfld[i].name);
                    }
                    cb_mcf_shfld.Enabled = true;
                }
                else
                {
                    cb_mcf_shfld.Enabled = false;
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////
        /// Tab: 設備情報モニタ設定
        /////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// 設備情報ファイルリストボックス内のアイテムが変更された時にタブ内の表示を変更します
        /// </summary>
        /// <param name="mcfconf"></param>
        private void Mstsfconf(MSTSFconf mstsconf)
        {
            tb_mstsfkey.Text = mstsconf.mstsfilekey;
            cb_mstsfid.Text = mstsconf.mstsfileid;
            cb_mstsf_inttimfetch.Text = mstsconf.foi.inttimefetch;
            cb_mstsf_cnttype.Text = mstsconf.foi.cnttype;
            update_mstsf_cbcntid();
            cb_mstsf_cntid.Text = mstsconf.foi.cntid;
            update_mstsf_cbpulltype();
            cb_mstsf_pulltype.Text = mstsconf.foi.pulltype;
            update_mstsf_cbshfld();
            cb_mstsf_shfld.Text = mstsconf.foi.shfld;
            chk_mstsf_serverpull.Checked = mstsconf.foi.serverpull;
        }

        /// <summary>
        /// MSTSFタブの表示をクリアします
        /// </summary>
        private void Mstsfconfnull()
        {
            tb_mstsfkey.Text = "";
            cb_mstsfid.Text = "";
            cb_mstsf_inttimfetch.Text = "";
            cb_mstsf_cntid.Text = "";
            cb_mstsf_cnttype.Text = "";
            cb_mstsf_pulltype.Text = "";
            cb_mstsf_shfld.Text = "";
            chk_mstsf_serverpull.Checked = false;
        }

        /// <summary>
        /// MSTSF追加時[+]の入力補助フォーム表示
        /// </summary>
        private void bt_add_mstsf_Click(object sender, EventArgs e)
        {
            sharestring = "mstsf";
            fmin.Show();
            fmin.Text = "設備情報ファイル名を入力してください";
        }

        /// <summary>
        /// MSTSFの削除[-]
        /// </summary>
        private void bt_rm_mstsf_Click(object sender, EventArgs e)
        {
            if (lb_mstsfkey.SelectedIndex > -1)
            {
                mcconf.Mstsfs.mstsfconfs.RemoveAt(lb_mstsfkey.SelectedIndex);
                lb_mstsfkey.Items.Remove(lb_mstsfkey.SelectedItem);
                Mstsfconfnull();
            }
        }

        /// <summary>
        /// MSTSFリストアイテム変更スロット
        /// </summary>
        private void lb_mstsf_SelectedIndexChanged(object sender, EventArgs e)
        {
            TceLock = true;
            if (lb_mstsfkey.SelectedIndex > -1)
            {
                Mstsfconf(mcconf.Mstsfs.mstsfconfs[lb_mstsfkey.SelectedIndex]);
            }
            else
            {
                lb_mstsfkey.Text = "";
            }
            TceLock = false;
        }

        /// <summary>(object sender, EventArgs e)
        /// MSTSFファイル取得条件変更スロット
        /// </summary>
        private void mstsfedited(object sender, EventArgs e)
        {
            var type = sender.GetType();
            if (type.Name == "CheckBox")
            {
                //chk_mstsf_serverpull
                if (chk_mstsf_serverpull.Checked)
                {
                    cb_mstsf_inttimfetch.Enabled = true;
                    cb_mstsf_pulltype.Enabled = true;
                    cb_mstsf_shfld.Enabled = false;
                }
                else
                {
                    cb_mstsf_inttimfetch.Enabled = false;
                    cb_mstsf_pulltype.Enabled = false;
                    cb_mstsf_shfld.Enabled = false;
                }
            }
            else if (type.Name == "ComboBox")
            {
                //cb_mstsf_cnttype
                if (((ComboBox)sender).Name == "cb_mstsf_cnttype")
                {
                    if (cb_mstsf_cnttype.Text != "")
                    {
                        update_mstsf_cbcntid();
                    }
                }
                //cb_mstsf_cntid
                if (((ComboBox)sender).Name == "cb_mstsf_cntid")
                {
                    if (cb_mstsf_cntid.Text != "")
                    {
                        update_mstsf_cbpulltype();
                    }
                }
                //cb_mstsf_shfld
                if (((ComboBox)sender).Name == "cb_mstsf_pulltype")
                {
                    if (cb_mstsf_pulltype.Text != "")
                    {
                        update_mstsf_cbshfld();
                    }  
                }
            }

            savedata2mcconf(sender, e);

        }

        private void update_mstsf_cbcntid()
        {
            cb_mstsf_cntid.Items.Clear();

            if (cb_mstsf_cnttype.Text == "PLC")
            {
                for (int i = 0; i < mcconf.Plcs.plcconfs.Count; i++)
                {
                    cb_mstsf_cntid.Items.Add(mcconf.Plcs[i].name);
                }
            }
            else
            {
                for (int i = 0; i < mcconf.Pcs.pcconfs.Count; i++)
                {
                    cb_mstsf_cntid.Items.Add(mcconf.Pcs[i].name);
                }

            }
        }

        private void update_mstsf_cbpulltype()
        {
            cb_mstsf_pulltype.Items.Clear();
            if (cb_mstsf_cntid.SelectedIndex > -1)
            {
                if (cb_mstsf_cnttype.Text == "PLC")
                {
                    if (mcconf.Plcs[cb_mstsf_cntid.SelectedIndex].ftps[0].useftpsv)
                    {
                        cb_mstsf_pulltype.Items.Add("FTP");
                    }
                    cb_mstsf_shfld.Enabled = false;
                }
                else
                {
                    if (mcconf.Pcs[cb_mstsf_cntid.SelectedIndex].ftps[0].useftpsv)
                    {
                        cb_mstsf_pulltype.Items.Add("FTP");
                    }
                    cb_mstsf_pulltype.Items.Add("ShareFolder");
                }
            }
        }

        private void update_mstsf_cbshfld()
        {
            cb_mstsf_shfld.Items.Clear();
            if (cb_mstsf_cntid.SelectedIndex > -1)
            {
                if (cb_mstsf_pulltype.Text == "ShareFolder")
                {
                    for (int i = 0; i < mcconf.Pcs[cb_mstsf_cntid.SelectedIndex].shfld.sfconf.Count; i++)
                    {
                        cb_mstsf_shfld.Items.Add(mcconf.Pcs[cb_mstsf_cntid.SelectedIndex].shfld[i].name);
                    }
                    cb_mstsf_shfld.Enabled = true;
                }
                else
                {
                    cb_mstsf_shfld.Enabled = false;
                }
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////
        /// Functions
        /////////////////////////////////////////////////////////////////////////////////////

        ////////
        // ② //
        ////////

        /////////////////////////////
        // エディットされたデータを
        // オブジェクトに書き込む
        /////////////////////////////
        public void savedata2mcconf(object sender, EventArgs e)
        {
            if (!TceLock && chkints() && chknull())
            {
                //保存先表示
                //lb_confpath.Text = "設定ファイル保存先：" + mcdir + @"\" +
                //    cb_mcat.Text + @"\" + cb_macno.Text + @"\" + @"conf\macconf.json";
                lb_confpath.Text = "設定ファイル保存先：";

                /////////////////////////////
                /// オブジェクト書き換え
                /////////////////////////////
                // 共通
                mcconf.Mchd.mcat = cb_mcat.Text;
                mcconf.Mchd.macno = cb_macno.Text;
                //mcconf.Mchd.mcuseplc = chk_useplc.Checked;
                //mcconf.Mchd.mcusepc = chk_usepc.Checked;

                // PLC
                if (lb_plc.SelectedIndex > -1)
                {
                    mcconf.Plcs[lb_plc.SelectedIndex].name = tb_plc_name.Text;
                    mcconf.Plcs[lb_plc.SelectedIndex].ipa = tb_plc_ipa.Text;
                    mcconf.Plcs[lb_plc.SelectedIndex].devport = tb_plc_devport.Text;
                    //mcconf.Plcs[lb_plc.SelectedIndex].usemc = chk_plc_usemc.Checked;
                    //mcconf.Plcs[lb_plc.SelectedIndex].usemm = chk_plc_usemm.Checked;
                    mcconf.Plcs[lb_plc.SelectedIndex].ftps[0].useftpsv = chk_plc_useftpsv.Checked;
                    mcconf.Plcs[lb_plc.SelectedIndex].ftps[0].id = tb_plc_ftpid.Text;
                    mcconf.Plcs[lb_plc.SelectedIndex].ftps[0].password = tb_plc_ftppass.Text;
                    mcconf.Plcs[lb_plc.SelectedIndex].ftps[0].port = tb_plc_ftpport.Text;
                    mcconf.Plcs[lb_plc.SelectedIndex].ftps[0].homedir = tb_plc_ftphomedir.Text;
                    if (lb_plc_dev.SelectedIndex > -1)
                    {
                        mcconf.Plcs[lb_plc.SelectedIndex].devs[lb_plc_dev.SelectedIndex].devid = tb_plc_devid.Text;
                        mcconf.Plcs[lb_plc.SelectedIndex].devs[lb_plc_dev.SelectedIndex].devtype = cb_plc_devtype.Text;
                        mcconf.Plcs[lb_plc.SelectedIndex].devs[lb_plc_dev.SelectedIndex].devno = tb_plc_devno.Text;
                    }
                    if (lb_plc_sharefd.SelectedIndex > -1)
                    {
                        mcconf.Plcs[lb_plc.SelectedIndex].shfld[lb_plc_sharefd.SelectedIndex].name = tb_plc_sharefdname.Text;
                        mcconf.Plcs[lb_plc.SelectedIndex].shfld[lb_plc_sharefd.SelectedIndex].path = tb_plc_sharefdpath.Text;
                    }
                }
                // 設備PC
                if (lb_pc.SelectedIndex > -1)
                {
                    mcconf.Pcs[lb_pc.SelectedIndex].name = tb_pc_name.Text;
                    mcconf.Pcs[lb_pc.SelectedIndex].ipa = tb_plc_ipa.Text;
                    //mcconf.Pcs[lb_pc.SelectedIndex].usemc = chk_pc_usemc.Checked;
                    //mcconf.Pcs[lb_pc.SelectedIndex].usemm = chk_pc_usemm.Checked;
                    mcconf.Pcs[lb_pc.SelectedIndex].ftps[0].useftpsv = chk_pc_useftpsv.Checked;
                    mcconf.Pcs[lb_pc.SelectedIndex].ftps[0].id = tb_pc_ftpid.Text;
                    mcconf.Pcs[lb_pc.SelectedIndex].ftps[0].password = tb_pc_ftppass.Text;
                    mcconf.Pcs[lb_pc.SelectedIndex].ftps[0].port = tb_pc_ftpport.Text;
                    mcconf.Pcs[lb_pc.SelectedIndex].ftps[0].homedir = tb_pc_ftphomedir.Text;
                    if (lb_pc_sharefd.SelectedIndex > -1)
                    {
                        mcconf.Pcs[lb_pc.SelectedIndex].shfld[lb_pc_sharefd.SelectedIndex].name = tb_pc_sharefdname.Text;
                        mcconf.Pcs[lb_pc.SelectedIndex].shfld[lb_pc_sharefd.SelectedIndex].path = tb_pc_sharefdpath.Text;
                    }
                }
                // IOファイル設定
                if (lb_mcfilekey.SelectedIndex > -1)
                {
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].mcfilekey = tb_mcfilekey.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].returns = tb_mcfile_return.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].encoding = cb_mcfile_enc.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].disableEndfile = chk_mcfile_disableEndfile.Checked;
                    if (!chk_mcfile_disableEndfile.Checked)
                    {
                        tb_mcfile_return.Enabled = true;
                        chk_mcfile_verifiparam.Enabled = true;
                        chk_mcfile_sp1.Enabled = true;
                    }
                    else
                    {
                        tb_mcfile_return.Enabled = false;
                        chk_mcfile_verifiparam.Enabled = false;
                        chk_mcfile_sp1.Enabled = false;
                    }
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].verifiparam = chk_mcfile_verifiparam.Checked;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].spfnc1 = chk_mcfile_sp1.Checked;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.useplcdev = chk_mcfile_useplcdev.Checked;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.devid = cb_mcf_plcdev.Text;
                    if (mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.useplcdev)
                    {
                        cb_mcf_plcdev.Enabled = true;
                    }
                    else
                    {
                        cb_mcf_plcdev.Enabled = false;
                    }
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.serverpull = chk_mcf_serverpull.Checked;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.cntid = cb_mcf_cntid.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.cnttype = cb_mcf_cnttype.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.pulltype = cb_mcf_pulltype.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.shfld = cb_mcf_shfld.Text;
                    mcconf.Mcfs[lb_mcfilekey.SelectedIndex].foi.path = tb_mcf_path.Text;
                }
                // 設備情報ファイル設定
                if (lb_mstsfkey.SelectedIndex > -1)
                {
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].mstsfilekey = tb_mstsfkey.Text;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].mstsfileid = cb_mstsfid.Text;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].foi.inttimefetch = cb_mstsf_inttimfetch.Text;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].foi.serverpull = chk_mstsf_serverpull.Checked;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].foi.cntid = cb_mstsf_cntid.Text;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].foi.cnttype = cb_mstsf_cnttype.Text;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].foi.pulltype = cb_mstsf_pulltype.Text;
                    mcconf.Mstsfs[lb_mstsfkey.SelectedIndex].foi.shfld = cb_mstsf_shfld.Text;

                }
            }
        }


        /////////////////////////////
        /// 数値入力チェック
        /////////////////////////////
        private bool chkints()
        {
            var NumTxtBoxlst = new TextBox[] { tb_plc_devport, tb_plc_ftpport, tb_plc_devno, tb_pc_ftpport };
            for (int i = 0; i < NumTxtBoxlst.Length; i++)
            {
                if (NumTxtBoxlst[i].Text!="" && !IntValueChk(NumTxtBoxlst[i].Text))
                {
                    MessageBox.Show("数値入力を確認してください");
                    return false;
                }
            }
            return true;
        }


        /////////////////////////////
        /// 空入力チェック
        /////////////////////////////
        private bool chknull()
        {
            /*
            // PLC
            var plcTxtBoxlst = new TextBox[] { tb_plc_ipa, tb_plc_devport, tb_plc_ftpport, tb_plc_ftpid, tb_plc_ftppass};
            if (mcconf.Plcs.plcconfs.Count>0)
            {
                for (int i = 0; i < plcTxtBoxlst.Length; i++)
                {
                    if (plcTxtBoxlst[i].Text == "")
                    {
                        //MessageBox.Show("空入力に注意してください");
                        return false;
                    }
                }
            }
            
            //PC
            var pcTxtBoxlst = new TextBox[] { tb_pc_name, tb_pc_ipa, tb_pc_ftpport, tb_pc_ftpid, tb_pc_ftppass};
            if (mcconf.Pcs.pcconfs.Count > 0)
            {
                for (int i = 0; i < pcTxtBoxlst.Length; i++)
                {
                    if (pcTxtBoxlst[i].Text == "")
                    {
                        //MessageBox.Show("空入力に注意してください");
                        return false;
                    }
                }
            }
            //MCfile
            var mcfTxtBoxlst = new TextBox[] { tb_mcfile_return };
            if (mcconf.Mcfs.mcfconfs.Count > 0)
            {
                for (int i = 0; i < mcfTxtBoxlst.Length; i++)
                {
                    if (mcfTxtBoxlst[i].Text == "")
                    {
                        //MessageBox.Show("空入力に注意してください");
                        return false;
                    }
                }
            }
            */
            return true;
        }

        /////////////////////////////
        // json file 書き込み用
        /////////////////////////////
        public void JsonFileWriter(string FilePath, string json, string mes="")
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
        public string JsonFileReader(string FilePath)
        {
            StreamReader sr = new StreamReader(FilePath, Encoding.GetEncoding("utf-8"));
            string str = sr.ReadToEnd();
            sr.Close();
            return str;
        }

        /////////////////////////////
        // float値の入力確認
        /////////////////////////////
        public bool FloatValueChk(string txtValue)
        {
            if (float.TryParse(txtValue, out float f))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /////////////////////////////
        // intの入力確認
        /////////////////////////////
        public bool IntValueChk(string txtValue)
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

        /////////////////////////////
        // macconf.jsonの検索
        /////////////////////////////
        void DirSearch(string sDir, ref List<String> Files)
        {
            try
            {
                foreach (string d in Directory.GetDirectories(sDir))
                {
                    foreach (string f in Directory.GetFiles(d, "macconf.json"))
                    {
                        Files.Add(f);
                    }
                    DirSearch(d, ref Files);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("ファイル取得時に問題が発生しました\r\n" + ex.ToString());
            }
        }

    }
}
