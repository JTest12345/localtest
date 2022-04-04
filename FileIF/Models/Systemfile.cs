using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Oskas;
using YamlDotNet.RepresentationModel;


namespace FileIf
{

    ////////////////////////////////////
    ///
    /// MagCupタスククラス
    /// 20211209廃止
    ///
    ////////////////////////////////////

    //class Tasks_MagCup
    //{
    //    public Tasks_min1 Min1;
    //    public Tasks_min2 Min2;
    //    public Tasks_mot Mout;
    //    public Tasks_mio Mio;
    //    public Tasks_vlin1 Vlin1;
    //    public Tasks_vlin2 Vlin2;
    //    public Tasks_bto Bto;
    //    public Tasks_rcp Crcp;
    //    public Tasks_sta Csta;
    //    public Tasks_cot1 Cot1;
    //    public Tasks_cot2 Cot2;

    //    public Tasks_MagCup()
    //    {
    //        Min1 = new Tasks_min1();
    //        Min2 = new Tasks_min2();
    //        Mout = new Tasks_mot();
    //        Mio = new Tasks_mio();
    //        Vlin1 = new Tasks_vlin1();
    //        Vlin2 = new Tasks_vlin2();
    //        Bto = new Tasks_bto();
    //        Crcp = new Tasks_rcp();
    //        Csta = new Tasks_sta();
    //        Cot1 = new Tasks_cot1();
    //        Cot2 = new Tasks_cot2();
    //    }
    //}


    ////////////////////////////////////
    ///
    /// File_Systemクラス
    ///
    ////////////////////////////////////

    public class Mcfilesys //タスク処理に必要な情報（システムini/ 検出されたファイルから得られるシステム情報をまとめたクラス）
    {
        public string filepath { get; set; }
        public string lowerfilepath { get; set; }
        public string Upperfilepath { get; set; }
        public string tmpfilepath { get; set; }
        public string key { get; set; }
        public string[] ff { get; set; }
        public string Pcat { get; set; }
        public string Macno { get; set; }
        public string FindFold { get; set; }
        public string MagCupNo { get; set; }
        public string FileNameKey { get; set; }
        public string RecipeFile { get; set; }
        public string fpath { get; set; }
        public string MacFld { get; set; }
        public string MacConfPath { get; set; }
        public string keylbl { get; set; }
        public string mclbl { get; set; }
        public string[] lbl { get; set; }
        public string ext { get; set; }
        //public string plcdvtyp { get; set; }
        //public string plcdvno { get; set; }
        public string ConnectionString { get; set; }
        public Magcupini mci = new Magcupini();
        public macconfjson mconf = new macconfjson();
        public MacHeader mhd = new MacHeader();
        public PLCconf plcc = new PLCconf();
        public PCconf pcc = new PCconf();
        public MCFconf mcfc = new MCFconf();
    }


    ////////////////////////////////////
    ///
    /// MagCup Iniファイルクラス
    ///
    ////////////////////////////////////

    public class Magcupini //イニシャル用定数と変数(magcup.iniの読み込み)実体
    {
        /// magcup.ini
        //システム情報[systemconf]
        public bool DebugMode { get; set; }
        public bool UsePlcTrig { get; set; }
        public bool CheckVlot { get; set; }
        public int outfiletimeout { get; set; }
        public int historyoutofdate { get; set; }
        //ディレクトリ情報[fileconf]
        public string MCDir { get; set; }
        public string MsglogDir { get; set; }
        public string WipDir { get; set; }
        public string RecipUpLoadDir { get; set; }
        //ファイル検索キー[fileconf]
        public string[] infilekey { get; set; }
        public string[] wipfilekey { get; set; }
        public string[] endfilekey { get; set; }
        public string[] donefilekey { get; set; }
        public string[] errfilekey { get; set; }
        //FileFetch関連[]
        public string[] FFetchPcat { get; set; }
        public string[] FFetchMacName { get; set; }
        public string[] FFetchFileKey { get; set; }
        //DB接続情報[dbconf]
        public string Server { get; set; }// ホスト名
        public string Port { get; set; }// ポート番号
        public string Database1 { get; set; }// データベース名-1
        public string Uid1 { get; set; }// ユーザ名-1
        public string Pwd1 { get; set; }// パスワード-1
        public string Database2 { get; set; }// データベース名-2
        public string Uid2 { get; set; }// ユーザ名-2
        public string Pwd2 { get; set; }// パスワード-2
        //Mqttブローカーホスト情報[mqtt]
        public bool isUseMqtt { get; set; }
        public string mosquittoHost { get; set; }
        /// magcup.iniここまで

        public string IniFileDir = @"C:\oskas\magcup";
        public string inifilename = @"\magcup.ini";
        public string yamlfilename = @"\magcup.yaml";

        // DB接続文字列
        public string ConnectionString1 { get; set; }
        public string ConnectionString2 { get; set; }
        public string[] ConnectionStrings { get; set; }
        // FILEIF開始時刻
        public DateTime SrvStatDT { get; set; }
        // ログのパス
        public string MsglogPath { get; set; }

        public Magcupini()
        {
        }

        //
        /* mugcup.iniの読み込み関数
        */
        //
        public bool GetMugCupIniValues(ref string globalmsg)
        {
            try
            {
                string filepath = IniFileDir + inifilename;
                if (!CommonFuncs.FileExists(filepath))
                    return false;

                // [systemconf]
                string debugmode = CommonFuncs.GetIniValue(filepath, "systemconf", "debug");
                if (debugmode == "true")
                    DebugMode = true;
                else
                    DebugMode = false;
                string useplcrig = CommonFuncs.GetIniValue(filepath, "systemconf", "use_plctrig");
                if (useplcrig == "false")
                    UsePlcTrig = false;
                else
                    UsePlcTrig = true;
                string checkvlot = CommonFuncs.GetIniValue(filepath, "systemconf", "check_vlot");
                if (checkvlot == "false")
                    CheckVlot = false;
                else
                    CheckVlot = true;
                outfiletimeout = int.Parse(CommonFuncs.GetIniValue(filepath, "systemconf", "outfile_timeout"));
                historyoutofdate = int.Parse(CommonFuncs.GetIniValue(filepath, "systemconf", "history_outofdate"));

                // [fileconf]
                MCDir = CommonFuncs.GetIniValue(filepath, "fileconf", "DirSearc");
                MsglogDir = CommonFuncs.GetIniValue(filepath, "fileconf", "MsglogDir");
                WipDir = CommonFuncs.GetIniValue(filepath, "fileconf", "WipDir");
                RecipUpLoadDir = CommonFuncs.GetIniValue(filepath, "fileconf", "RecipUpLoadDir");
                string infilekey_ini = CommonFuncs.GetIniValue(filepath, "fileconf", "infilekey");
                infilekey = infilekey_ini.Split(',');
                string wipfilekey_ini = CommonFuncs.GetIniValue(filepath, "fileconf", "wipfilekey");
                wipfilekey = wipfilekey_ini.Split(',');
                string endfilekey_ini = CommonFuncs.GetIniValue(filepath, "fileconf", "endfilekey");
                endfilekey = endfilekey_ini.Split(',');
                string donefilekey_ini = CommonFuncs.GetIniValue(filepath, "fileconf", "donefilekey");
                donefilekey = donefilekey_ini.Split(',');
                string errfilekey_ini = CommonFuncs.GetIniValue(filepath, "fileconf", "errfilekey");
                errfilekey = errfilekey_ini.Split(',');

                // [ffetch]
                string proccat_ini = CommonFuncs.GetIniValue(filepath, "ffetch", "proccat");
                FFetchPcat = proccat_ini.Split(',');
                string macname_ini = CommonFuncs.GetIniValue(filepath, "ffetch", "macno");
                FFetchMacName = macname_ini.Split(',');
                string filekey_ini = CommonFuncs.GetIniValue(filepath, "ffetch", "filekey");
                FFetchFileKey = filekey_ini.Split(',');


                // [dbconf]
                Server = CommonFuncs.GetIniValue(filepath, "dbconf", "Server");
                Port = CommonFuncs.GetIniValue(filepath, "dbconf", "Port");
                Database1 = CommonFuncs.GetIniValue(filepath, "dbconf", "Database1");
                Uid1 = CommonFuncs.GetIniValue(filepath, "dbconf", "Uid1");
                Pwd1 = CommonFuncs.GetIniValue(filepath, "dbconf", "Pwd1");
                Database2 = CommonFuncs.GetIniValue(filepath, "dbconf", "Database2");
                Uid2 = CommonFuncs.GetIniValue(filepath, "dbconf", "Uid2");
                Pwd2 = CommonFuncs.GetIniValue(filepath, "dbconf", "Pwd2");

                // [mosquitto]
                if (CommonFuncs.GetIniValue(filepath, "mqtt", "isUseMqtt") == "true")
                {
                    isUseMqtt = true;
                }
                else
                {
                    isUseMqtt = false;
                }
                mosquittoHost = CommonFuncs.GetIniValue(filepath, "mqtt", "mosquittoHost");

                // [SQL Server Connection string]
                ConnectionString1 = $"Server={Server}; Port={Port}; Database={Database1}; Uid={Uid1}; Pwd={Pwd1}; SslMode=none";
                ConnectionString2 = $"Server={Server}; Port={Port}; Database={Database2}; Uid={Uid2}; Pwd={Pwd2}; SslMode=none";
                ConnectionStrings = new string[]{ConnectionString1, ConnectionString2};

            }
            catch
            {
                globalmsg = "magcup.ini読み込みに問題が発生しました";
                return false;
            }
            return true;
        }

        //
        /* mugcup.iniの読み込み関数
        */
        //
        public bool GetMugCupYamlValues(ref string globalmsg)
        {
            try
            {
                string filepath = IniFileDir + yamlfilename;
                if (!CommonFuncs.FileExists(filepath))
                    return false;

                var input = new StreamReader(filepath, Encoding.UTF8);
                var yaml = new YamlStream();
                yaml.Load(input);

                var rootNode = yaml.Documents[0].RootNode;

                // [systemconf]
                var systemconf = (YamlMappingNode)rootNode["systemconf"];

                if (systemconf["debug"].ToString() == "true")
                    DebugMode = true;
                else
                    DebugMode = false;
                if (systemconf["use_plctrig"].ToString() == "true")
                    UsePlcTrig = true;
                else
                    UsePlcTrig = false;
                if (systemconf["check_vlot"].ToString() == "true")
                    CheckVlot = true;
                else
                    CheckVlot = false;
                outfiletimeout = int.Parse(systemconf["outfile_timeout"].ToString()); // CommonFuncs.GetIniValue(filepath, "systemconf", "outfile_timeout"));
                historyoutofdate = int.Parse(systemconf["history_outofdate"].ToString()); // CommonFuncs.GetIniValue(filepath, "systemconf", "history_outofdate"));

                // [fileconf]
                var fileconf = (YamlMappingNode)rootNode["fileconf"];

                MCDir = fileconf["DirSearc"].ToString(); // CommonFuncs.GetIniValue(filepath, "fileconf", "DirSearc");
                MsglogDir = fileconf["MsglogDir"].ToString(); // CommonFuncs.GetIniValue(filepath, "fileconf", "MsglogDir");
                WipDir = fileconf["WipDir"].ToString(); // CommonFuncs.GetIniValue(filepath, "fileconf", "WipDir");
                RecipUpLoadDir = fileconf["RecipUpLoadDir"].ToString(); // CommonFuncs.GetIniValue(filepath, "fileconf", "RecipUpLoadDir");
                string infilekey_ini = fileconf["infilekey"].ToString().Replace("[","").Replace("]","").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "fileconf", "infilekey");
                infilekey = infilekey_ini.Split(',');
                string wipfilekey_ini = fileconf["wipfilekey"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "fileconf", "wipfilekey");
                wipfilekey = wipfilekey_ini.Split(',');
                string endfilekey_ini = fileconf["endfilekey"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "fileconf", "endfilekey");
                endfilekey = endfilekey_ini.Split(',');
                string donefilekey_ini = fileconf["donefilekey"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "fileconf", "donefilekey");
                donefilekey = donefilekey_ini.Split(',');
                string errfilekey_ini = fileconf["errfilekey"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "fileconf", "errfilekey");
                errfilekey = errfilekey_ini.Split(',');

                // [ffetch]
                var ffetch = (YamlMappingNode)rootNode["ffetch"];

                string proccat_ini = ffetch["proccat"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "ffetch", "proccat");
                FFetchPcat = proccat_ini.Split(',');
                string macname_ini = ffetch["macno"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "ffetch", "macno");
                FFetchMacName = macname_ini.Split(',');
                string filekey_ini = ffetch["filekey"].ToString().Replace("[", "").Replace("]", "").Replace(" ", ""); // CommonFuncs.GetIniValue(filepath, "ffetch", "filekey");
                FFetchFileKey = filekey_ini.Split(',');


                // [dbconf]
                var dbconf = (YamlMappingNode)rootNode["dbconf"];

                Server = dbconf["Server"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Server");
                Port = dbconf["Port"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Port");
                Database1 = dbconf["Database1"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Database1");
                Uid1 = dbconf["Uid1"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Uid1");
                Pwd1 = dbconf["Pwd1"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Pwd1");
                Database2 = dbconf["Database2"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Database2");
                Uid2 = dbconf["Uid2"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Uid2");
                Pwd2 = dbconf["Pwd2"].ToString(); // CommonFuncs.GetIniValue(filepath, "dbconf", "Pwd2");

                // [mosquitto]
                var mqtt = (YamlMappingNode)rootNode["mqtt"];

                if (mqtt["isUseMqtt"].ToString() == "true")
                {
                    isUseMqtt = true;
                }
                else
                {
                    isUseMqtt = false;
                }
                mosquittoHost = mqtt["mosquittoHost"].ToString(); // CommonFuncs.GetIniValue(filepath, "mqtt", "mosquittoHost");

                // [SQL Server Connection string]
                ConnectionString1 = $"Server={Server}; Port={Port}; Database={Database1}; Uid={Uid1}; Pwd={Pwd1}; SslMode=none";
                ConnectionString2 = $"Server={Server}; Port={Port}; Database={Database2}; Uid={Uid2}; Pwd={Pwd2}; SslMode=none";
                ConnectionStrings = new string[] { ConnectionString1, ConnectionString2 };

            }
            catch
            {
                globalmsg = "magcup.yaml読み込みに問題が発生しました";
                return false;
            }
            return true;
        }

    }

    ////////////////////////////////////
    ///
    /// MagCup Iniファイルクラス
    /// タスクで実際に使用する機器情報クラス
    /// ⇒macconf.jsonに流用
    ///
    ////////////////////////////////////

    public class Macconinfo 
    {
        public string Macno { get; set; }
        public string Pcat { get; set; }
        public string Ipaddress { get; set; }
        public string Port { get; set; }
        /*
        public string Dvtype1 { get; set; }
        public string Devno1 { get; set; }
        public string Dvtype2 { get; set; }
        public string Devno2 { get; set; }
        public string Dvtype3 { get; set; }
        public string Devno3 { get; set; }
        public string Magno { get; set; }
        */
        public devs devs = new devs();
        public string plctrgdevid { get; set; }
        public string plctrgdevtype { get; set; }
        public string plctrgdevno { get; set; }
    }


    ////////////////////////////////////
    ///
    /// macconf.iniの内、
    /// エンドファイルの条件を読み込むクラス
    /// ⇒macconf.jsonに流用
    ///
    ////////////////////////////////////
    class Endfileconf
    {
        public string encode { get; set; }
        public string stacode { get; set; }
        public string stpcode { get; set; }
        //public int needdbq { get; set; }
        public bool needdbq { get; set; } //for json
        public List<string> endfcontents { get; set; }
    }


    ////////////////////////////////////
    /// 20200907
    /// macconfini
    /// mactaskconf
    /// ◆設備設定iniファイル版
    /// ⇒SystemfilePlus.cs内の
    /// 　macconfjsonに機能移管し廃止
    ///
    ////////////////////////////////////

    /*
    // macconf.iniの内、機器情報を読み込むクラス
    class macconfini
    {
        public string pcat { get; set; }
        public string macno { get; set; }
        public string ipa { get; set; }
        public string port { get; set; }
        public string[] dev1 { get; set; }
        public string[] dev2 { get; set; }
        public string[] dev3 { get; set; }


        public bool GetMacInfoProfile(Mcfilesys fs, ref string Dbgmsg)
        {
            CommonFuncs commons = new CommonFuncs();
            try
            {
                string filepath = fs.fpath + "\\conf\\macconf.ini";
                if (!commons.FileExists(filepath))
                {
                    Dbgmsg += "対象設備のmacconf.iniが存在しないようです";
                    return false;
                }
                // [macprofile]
                pcat = commons.GetIniValue(filepath, "macprofile", "pcat");
                macno = commons.GetIniValue(filepath, "macprofile", "macno");
            }
            catch
            {
                Dbgmsg += "magcup.ini読み込みに問題が発生しました";
                return false;
            }
            return true;
        }

        public bool GetMacPlcProfile(Mcfilesys fs, ref string Dbgmsg)
        {
            CommonFuncs commons = new CommonFuncs();
            try
            {
                string filepath = fs.fpath + "\\conf\\macconf.ini";
                if (!commons.FileExists(filepath))
                {
                    Dbgmsg += "対象設備のmacconf.iniが存在しないようです";
                    return false;
                }
                // [macprofile]
                ipa = commons.GetIniValue(filepath, "macprofile", "ipa");
                port = commons.GetIniValue(filepath, "macprofile", "port");
                string dev1str = commons.GetIniValue(filepath, "macprofile", "dev1");
                dev1 = dev1str.Split(',');
                string dev2str = commons.GetIniValue(filepath, "macprofile", "dev2");
                dev2 = dev2str.Split(',');
                string dev3str = commons.GetIniValue(filepath, "macprofile", "dev3");
                dev3 = dev3str.Split(',');
            }
            catch
            {
                Dbgmsg += "magcup.ini読み込みに問題が発生しました";
                return false;
            }
            return true;
        }
    }


    // macconf.iniの内、タスクのベリファイ条件を読み込むクラス
    class mactaskconf
    {
        public bool verifiparam { get; set; }

        public bool GetMacTaskConf(Mcfilesys fs, ref string Dbgmsg)
        {
            CommonFuncs commons = new CommonFuncs();
            try
            {
                string filepath = fs.fpath + "\\conf\\macconf.ini";
                if (!commons.FileExists(filepath))
                {
                    Dbgmsg += "対象設備のmacconf.iniが存在しないようです";
                    return false;
                }
                // [taskconf]
                string Verifiparam = commons.GetIniValue(filepath, "taskconf", "verifiparam");
                if (Verifiparam == "true")
                    verifiparam = true;
                else
                    verifiparam = false;
            }
            catch
            {
                Dbgmsg += "magcup.ini読み込みに問題が発生しました";
                return false;
            }
            return true;
        }

    }
    */

}
