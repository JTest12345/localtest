using System;
using System.Collections.Generic;
using Oskas;
using YamlDotNet.RepresentationModel;


namespace FileIf
{
    class Tasks_vip1 : Tasks_base
    {
        //◇評価実験用定数・変数
        //CSV移動先
        string csvDirMoveTo = string.Empty;
        //COJ格納変数
        string coj = string.Empty;

        //Endファイル用変数格納用辞書
        Dictionary<string, string> Dict;

        // 初期化
        public Tasks_vip1()
        {
            tcommons = new Tasks_Common();
            minfo = new Macconinfo();
            // 返信ファイル用辞書の初期化
            Dict = tcommons.InitRetFileDict();
        }

        // メインタスク関数
        public Task_Ret InFileTasks(Mcfilesys fs) //(string pcat, string macno, string rcpname, string fpath, string[] fs.lbl)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）
            fs.mclbl = "Vip";
            fs.lbl = new string[] { fs.mclbl, fs.keylbl };


            //<taskid=vip101>【macconf.ini】設備情報取得
            taskid = 101;
            Task_Ret gmic = tcommons.GetMacInfoConf(taskid, fs, minfo, ref Dict, ref msg, ref Dbgmsg);
            if (gmic.Result == retkey.ng)
            {
                return gmic;
            }


            //<taskid=vip102>【fileConfig】fileconfigを読込
            try
            {
                taskid += 1;
                var filepath = fs.MacFld + "\\conf\\fileconf.yaml";
                var rootNode = CommonFuncs.YamlFileReader(filepath, ref msg);

                if (rootNode != null)
                {
                    var movedir = (YamlMappingNode)rootNode["movedir"];
                    csvDirMoveTo = movedir[fs.keylbl].ToString();
                }
                else
                {
                    msg = tcommons.ErrorMessage(taskid, fs, "fileconfig.yamlが読み込めませんでした");
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=vip103>【CSV読込】データを読込
            var vipcontents = new Dictionary<string, string>();
            try
            {
                taskid += 1;

                var vipfile = new TaskFile_vip1();
                vipcontents = TaskFile_vip1.FileContents(fs.filepath, fs.mcfc.encoding, ref msg);
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=vip104>【実績】フォーマットに変換
            var contents = string.Empty;
            try
            {
                taskid += 1;

                var date = DateTime.Now.ToString("yyyy/MM/dd");
                var time = DateTime.Now.ToString("HH:mm:ss");

                contents += "TREND ID,0,,DATA1-250" + crlf;
                contents += $"DATE,{date},{time}" + crlf;
                contents += "" + crlf;
                contents += "No,DATE,TIME,DATA1,DATA2,DATA3,DATA4,DATA5,DATA6,DATA7,DATA8,DATA9,DATA10,DATA11,DATA12,DATA13,DATA14,DATA15,DATA16,DATA17,DATA18,DATA19,DATA20,DATA21,DATA22,DATA23,DATA24,DATA25,DATA26,DATA27,DATA28,DATA29,DATA30,DATA31,DATA32,DATA33,DATA34,DATA35,DATA36,DATA37,DATA38,DATA39,DATA40,DATA41,DATA42,DATA43,DATA44,DATA45,DATA46,DATA47,DATA48,DATA49,DATA50,DATA51,DATA52,DATA53,DATA54,DATA55,DATA56,DATA57,DATA58,DATA59,DATA60,DATA61,DATA62,DATA63,DATA64,DATA65,DATA66,DATA67,DATA68,DATA69,DATA70,DATA71,DATA72,DATA73,DATA74,DATA75,DATA76,DATA77,DATA78,DATA79,DATA80,DATA81,DATA82,DATA83,DATA84,DATA85,DATA86,DATA87,DATA88,DATA89,DATA90,DATA91,DATA92,DATA93,DATA94,DATA95,DATA96,DATA97,DATA98,DATA99,DATA100,DATA101,DATA102,DATA103,DATA104,DATA105,DATA106,DATA107,DATA108,DATA109,DATA110,DATA111,DATA112,DATA113,DATA114,DATA115,DATA116,DATA117,DATA118,DATA119,DATA120,DATA121,DATA122,DATA123,DATA124,DATA125,DATA126,DATA127,DATA128,DATA129,DATA130,DATA131,DATA132,DATA133,DATA134,DATA135,DATA136,DATA137,DATA138,DATA139,DATA140,DATA141,DATA142,DATA143,DATA144,DATA145,DATA146,DATA147,DATA148,DATA149,DATA150,DATA151,DATA152,DATA153,DATA154,DATA155,DATA156,DATA157,DATA158,DATA159,DATA160,DATA161,DATA162,DATA163,DATA164,DATA165,DATA166,DATA167,DATA168,DATA169,DATA170,DATA171,DATA172,DATA173,DATA174,DATA175,DATA176,DATA177,DATA178,DATA179,DATA180,DATA181,DATA182,DATA183,DATA184,DATA185,DATA186,DATA187,DATA188,DATA189,DATA190,DATA191,DATA192,DATA193,DATA194,DATA195,DATA196,DATA197,DATA198,DATA199,DATA200,DATA201,DATA202,DATA203,DATA204,DATA205,DATA206,DATA207,DATA208,DATA209,DATA210,DATA211,DATA212,DATA213,DATA214,DATA215,DATA216,DATA217,DATA218,DATA219,DATA220,DATA221,DATA222,DATA223,DATA224,DATA225,DATA226,DATA227,DATA228,DATA229,DATA230,DATA231,DATA232,DATA233,DATA234,DATA235,DATA236,DATA237,DATA238,DATA239,DATA240,DATA241,DATA242,DATA243,DATA244,DATA245,DATA246,DATA247,DATA248,DATA249,DATA250" + crlf;
                if (vipcontents.ContainsKey("機種名抜出し"))
                {
                    contents += $"0,{date},{time},{vipcontents["機種名抜出し"]},0,0,0,0,0,0,0,0,0,0,0,0,{vipcontents["Lot名抜出し"]},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,";
                }
                else if (vipcontents.ContainsKey("W1100"))
                {
                    contents += $"0,{date},{time},{vipcontents["W1100"]},0,0,0,0,0,0,0,0,0,0,0,0,{vipcontents["W1200"]},0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,";
                }
                else
                {
                    throw new Exception("機種名のキーがみつかりません");
                }
                
                // Bin1-150 data行
                for (int i = 1; i < 151; i++)
                {
                    if (vipcontents.ContainsKey($"BIN{i}"))
                    {
                        //PLCコメントがインデックスになっている生データの場合
                        contents += vipcontents[$"BIN{i}"];
                    }
                    if (vipcontents.ContainsKey($"ZR{11512 + (i - 1) * 2}"))
                    {
                        //PLCメモリアドレスがインデックスになっている生データの場合
                        contents += vipcontents[$"ZR{11512 + (i - 1) * 2}"];
                    }
                    else
                    {
                        contents += "0";
                    }
                    contents += ",";
                }

                contents += crlf;
                contents += "" + crlf;
                contents += "" + crlf;
                contents += "TREND ID,0,,DATA251-356" + crlf;
                contents += $"DATE,{date},{time}" + crlf;
                contents += "" + crlf;
                contents += "No,DATE,TIME,DATA251,DATA252,DATA253,DATA254,DATA255,DATA256,DATA257,DATA258,DATA259,DATA260,DATA261,DATA262,DATA263,DATA264,DATA265,DATA266,DATA267,DATA268,DATA269,DATA270,DATA271,DATA272,DATA273,DATA274,DATA275,DATA276,DATA277,DATA278,DATA279,DATA280,DATA281,DATA282,DATA283,DATA284,DATA285,DATA286,DATA287,DATA288,DATA289,DATA290,DATA291,DATA292,DATA293,DATA294,DATA295,DATA296,DATA297,DATA298,DATA299,DATA300,DATA301,DATA302,DATA303,DATA304,DATA305,DATA306,DATA307,DATA308,DATA309,DATA310,DATA311,DATA312,DATA313,DATA314,DATA315,DATA316,DATA317,DATA318,DATA319,DATA320,DATA321,DATA322,DATA323,DATA324,DATA325,DATA326,DATA327,DATA328,DATA329,DATA330,DATA331,DATA332,DATA333,DATA334,DATA335,DATA336,DATA337,DATA338,DATA339,DATA340,DATA341,DATA342,DATA343,DATA344,DATA345,DATA346,DATA347,DATA348,DATA349,DATA350,DATA351,DATA352,DATA353,DATA354,DATA355,DATA356,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,,," + crlf;
                contents += $"0,{date},{time},";
                // Bin151-256 data行
                //
                //PLCコメントがインデックスになっている生データの場合
                for (int i = 151; i < 257; i++)
                {
                    if (vipcontents.ContainsKey($"BIN{i}"))
                    {
                        contents += vipcontents[$"BIN{i}"];
                    }
                    if (vipcontents.ContainsKey($"ZR{11512 + (i - 1) * 2}"))
                    {
                        //PLCメモリアドレスがインデックスになっている生データの場合
                        contents += vipcontents[$"ZR{11512 + (i - 1) * 2}"];
                    }
                    else
                    {
                        contents += "0";
                    }
                    contents += ",";
                }

                contents += crlf;
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }


            //<taskid=vip105>【実績】フォルダに変換したファイルを転送
            try
            {
                taskid += 1;

                var filepath = csvDirMoveTo + "\\jisseki.csv";
                if (!CommonFuncs.CreateFile(filepath, contents, ref msg))
                {
                    msg = tcommons.ErrorMessage(taskid, fs, msg);;
                    return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
            }
            catch (Exception ex)
            {
                msg = tcommons.ErrorMessage(taskid, fs, ex.Message);
                return tcommons.MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }



            //<taskid=vip106>【完了処理】inフォルダからtempフォルダにINファイルを移動
            taskid += 1;
            Task_Ret mitf = tcommons.MoveIn2TempFolder(taskid, fs, ref Dict, ref msg, ref Dbgmsg);
            if (mitf.Result == retkey.ng)
            {
                return mitf;
            }
            else
            {
                Dbgmsg += "タスクの実行は全て終了しました" + crlf;
                msg = $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} タスク終了";
                return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
            }
        }


        // outのEND出力タスク関数
        public Task_Ret OutFileTasks(Mcfilesys fs, Task_Ret taskret)
        {
            string msg = "", Dbgmsg = ""; // メッセージ（通常, デバック）

            //<taskid=vip901>【ファイル生成】ENDファイルの発行
            taskid = 901;
            if (!fs.mcfc.disableEndfile)
            {
                Task_Ret oef = tcommons.OutputEndFile(taskid, fs, taskret, Dict, "end", ref msg, ref Dbgmsg);
                if (oef.Result == retkey.ng)
                {
                    return oef;
                }
            }

            return tcommons.MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }
    }
}
