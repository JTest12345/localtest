using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using Oskas;
using YamlDotNet.RepresentationModel;


namespace Oskas
{

    ////////////////////////////////////
    ///
    /// Oska Iniファイルクラス
    ///
    ////////////////////////////////////

    class oskaini //イニシャル用定数と変数(magcup.iniの読み込み)実体
    {
        
        public string IniFileDir = @"C:\Oskas";
        public string inifilename = @"\oskas.ini";
        public string yamlfilename = @"\oskas.yaml";

        /// magcup.ini
        //システム情報[systemconf]
        public bool DebugMode { get; set; }
        //ディレクトリ情報[fileconf]
        public string MsglogDir { get; set; }

        // サーバー開始時刻
        public DateTime SrvStatDT { get; set; }
        // コンソールログのパス
        public string MsglogPath { get; set; }
        // MQTT・Mosquitto
        public bool isUseMqtt { get; set; }
        public string mosquittoHost { get; set; }

        //
        /* oskas.iniの読み込み関数
        */
        //
        public bool GetOskasIniValues(ref string globalmsg)
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

                // [fileconf]
                MsglogDir = CommonFuncs.GetIniValue(filepath, "fileconf", "MsglogDir");

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
            }
            catch
            {
                globalmsg = "oska.ini読み込みに問題が発生しました";
                return false;
            }
            return true;
        }

        //
        /* oskas.yamlの読み込み関数
        */
        //
        public bool GetOskasYamlValues(ref string globalmsg)
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

                // [fileconf]
                var fileconf = (YamlMappingNode)rootNode["fileconf"];
                MsglogDir = fileconf["MsglogDir"].ToString();

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
                mosquittoHost = mqtt["mosquittoHost"].ToString();

            }
            catch (Exception e)
            {
                globalmsg = "oskas.yaml読み込みに問題が発生しました";
                return false;
            }
            return true;
        }

    }

}
