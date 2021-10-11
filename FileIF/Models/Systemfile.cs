﻿using System;
using System.Collections.Generic;
using Oskas;


namespace FileIf
{

    ////////////////////////////////////
    ///
    /// MagCupタスククラス
    ///
    ////////////////////////////////////

    class Tasks_MagCup
    {
        public Tasks_MIn1_v2 Min1;
        public Tasks_MIn2_v2 Min2;
        public Tasks_MOut_v2 Mout;
        public Tasks_MIO Mio;
        public Tasks_Crcp_v1_1 Crcp;
        public Tasks_Csta_v1_1 Csta;
        public Tasks_Cot1_v1_1 Cot1;
        public Tasks_Cot2_v1_1 Cot2;

        public Tasks_MagCup()
        {
            Min1 = new Tasks_MIn1_v2();
            Min2 = new Tasks_MIn2_v2();
            Mout = new Tasks_MOut_v2();
            Mio = new Tasks_MIO();
            Crcp = new Tasks_Crcp_v1_1();
            Csta = new Tasks_Csta_v1_1();
            Cot1 = new Tasks_Cot1_v1_1();
            Cot2 = new Tasks_Cot2_v1_1();
        }
    }


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
        public string RecipeFile { get; set; }
        public string fpath { get; set; }
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
        public int outfiletimeout { get; set; }
        //ディレクトリ情報[fileconf]
        public string MCDir { get; set; }
        public string MsglogDir { get; set; }
        public string WipDir { get; set; }
        public string RecipUpLoadDir { get; set; }
        //ファイル検索キー[fileconf]
        public string[] infilekey { get; set; }
        public string[] wipfilekey { get; set; }
        public string[] endfilekey { get; set; }
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

        // DB接続文字列
        public string ConnectionString1 { get; set; }
        public string ConnectionString2 { get; set; }
        public string[] ConnectionStrings { get; set; }
        // サーバー開始時刻
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
            //CommonFuncs commons = new CommonFuncs();
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
                outfiletimeout = int.Parse(CommonFuncs.GetIniValue(filepath, "systemconf", "outfile_timeout"));

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