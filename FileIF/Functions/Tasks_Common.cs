using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Oskas;


namespace FileIf
{
    public class Tasks_Common
    {
        //CommonFuncs commons = new CommonFuncs();

        //Console message Lebel(INT)
        //const int msg_info = 0;
        //const int msg_detect = 1;
        //const int msg_info = 2;
        //const int msg_alarm = 3;
        //const int msg_error = 4;
        //const int msg_debug = 10;


        public Dictionary<string, string> InitRetFileDict()
        {
            var dict = new Dictionary<string, string>()
            {
                { "OK" , retkey.ok },
                { "Ok", retkey.ok },
                { "ok", retkey.ok },
                { "NG" , retkey.ng },
                { "Ng", retkey.ng },
                { "ng", retkey.ng },
                { "CANCEL", retkey.cancel },
                { "Cancel", retkey.cancel },
                { "cancel", retkey.cancel },
                { "0", "0" },
                { "1", "1" },
                { "2", "2" },
                { "3", "3" },
                { "4", "4" },
                { "5", "5" },
                { "6", "6" },
                { "7", "7" },
                { "8", "8" },
                { "9", "9" },
                { "[CR]", "\r" },
                { "[LF]", "\n" },
                { "[CRLF]", "\r\n" },
            };

            return dict;
        }



        public int GetMagazineState(string lotno, string workcd, string magno, ref string msg)
        {
            try
            {
                // FIFがファイルを受けた工程(ワークスペース)のProcNoを取得
                var prclist = ArmsApi.Model.Process.GetProcessList(workcd);
                if (prclist.Length != 1)
                {
                    msg = "工程がマスタ登録されていない、もしくはマスタが不正です";
                    return 9;
                }

                // TnTranの状態確認
                var order = ArmsApi.Model.Order.GetOrder(lotno);
                if (order.Length != 0)
                {
                    // TnTranにレコードが確認できれば対象は初工程開始済以降となる
                    // FIFの対象工程ProcnoからTnTranを特定する
                    var curorder = ArmsApi.Model.Order.GetOrder(lotno, prclist[0].ProcNo);

                    // ◆TnTranに対象工程レコードがない
                    if (curorder.Length == 0)
                    {
                        // 開始前の可能性を探る
                        var mag = ArmsApi.Model.Magazine.GetCurrent(magno);
                        var lot = ArmsApi.Model.AsmLot.GetAsmLot(lotno);
                        var nextmagproc = ArmsApi.Model.Process.GetNextProcess(prclist[0].ProcNo, lot);
                        if (nextmagproc.ProcNo == prclist[0].ProcNo)
                        {
                            //◇マガジンの登録工程の次工程がFIF工程と一致すれば開始前確定
                            msg = "FIF問合せのマガジンは開始前のマガジンです";
                            return (int)retcode.BeforeStart;
                        }
                        else
                        {
                            //一致しない場合は何か不正な状態
                            msg = "FIF問合せのマガジンは本工程で処理不可能、もしくはデータベースが不正な状態です";
                            return (int)retcode.Failure;
                        }
                    }

                    // ◆TnTranに対象工程レコードあり
                    if (curorder[0].WorkEndDt != null)
                    {
                        // ◇WordEndDtがUpdate済
                        msg = "FIF問合せのマガジンは本工程完了済みの状態です";
                        return (int)retcode.Closed;
                    }
                    else
                    {
                        // ◇WordEndDtがnull
                        msg = "FIF問合せのマガジンは本工程開始済みの状態です";
                        return (int)retcode.Started;
                    }
                }
                else
                {
                    // ◆TnTranにレコードがなく、FIF工程が初工程である場合
                    var mag = ArmsApi.Model.Magazine.GetCurrent(lotno);
                    if (mag != null)
                    {
                        // ◇ロットNOでGetCurrentが取得可能であれば初工程開始前確定
                        msg = "FIF問合せのロットは初工程開始前の状態です";
                        return (int)retcode.BeforeStart;
                    }
                    else
                    {
                        // 取得できなければ不正
                        msg = "FIF問合せのマガジンは本工程で処理不可能、もしくはデータベースが不正な状態です";
                        return (int)retcode.Failure;
                    }
                }
            }
            catch (Exception ex)
            {
                msg =  ex.Message;
                return (int)retcode.Failure;
            }
        }

        //public string[] GetMacInfoConf(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        public Task_Ret GetMacInfoConf(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                if (fs.mconf.Mchd.mcat.ToLower() == fs.Pcat.ToLower() && fs.mconf.Mchd.macno.ToLower() == fs.Macno.ToLower())
                {
                    minfo.Pcat = fs.mconf.Mchd.mcat;
                    minfo.Macno = fs.mconf.Mchd.macno;
                    Dict.Add("{pcat}", fs.mconf.Mchd.mcat);
                    Dict.Add("{macno}", fs.mconf.Mchd.mcat);
                }
                else
                {
                    string mes = "設備情報(confとfilesystem)が一致していません";
                    msg = ErrorMessage(taskid, fs, mes);
                    return MakeRet( retkey.ng, msg, Dbgmsg,  (int)retcode.Failure );
                }

            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }

            return MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        /*
        // macconfフォルダ内ini使用版　⇒　廃止
        public string[] _GetPlcConnectConf(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                var mcc = new macconfini();

                if (!mcc.GetMacPlcProfile(fs, ref Dbgmsg))
                {
                    string mes = "PLCの接続情報取得ができません";
                    msg = ErrorMessage(taskid, fs, mes);
                    return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
                }

                minfo.Ipaddress = mcc.ipa;
                minfo.Port = mcc.port;
                minfo.Dvtype1 = mcc.dev1[0];
                minfo.Devno1 = mcc.dev1[1];
                minfo.Dvtype2 = mcc.dev2[0];
                minfo.Devno2 = mcc.dev2[1];
                minfo.Dvtype3 = mcc.dev3[0];
                minfo.Devno3 = mcc.dev3[1];

                Dict.Add("ipaddress", minfo.Ipaddress);
                Dict.Add("port", minfo.Port);
                Dict.Add("devtype1", minfo.Dvtype1);
                Dict.Add("devno1", minfo.Devno1);
                Dict.Add("devtype2", minfo.Dvtype2);
                Dict.Add("devno2", minfo.Devno2);
                Dict.Add("devtype3", minfo.Dvtype3);
                Dict.Add("devno3", minfo.Devno3);

            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return new string[] { retkey.ng, msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { retkey.ok };
        }
        */

        // macconfフォルダ内json使用版　⇒　現在使用中
        public Task_Ret GetPlcConnectConf(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                minfo.Ipaddress = fs.plcc.ipa;
                minfo.Port = fs.plcc.devport;
                if (fs.mcfc.foi.useplcdev)
                {
                    minfo.plctrgdevid = fs.mcfc.foi.devid;
                    for (int i = 0; i < fs.plcc.devs.devconfs.Count; i++)
                    {
                        if (fs.plcc.devs[i].devid == minfo.plctrgdevid)
                        {
                            minfo.plctrgdevtype = fs.plcc.devs[i].devtype;
                            minfo.plctrgdevno = fs.plcc.devs[i].devno;
                        }
                    }
                }
                else
                {
                    minfo.plctrgdevid = "";
                    minfo.plctrgdevtype = "";
                    minfo.plctrgdevno = "";
                }
                

                Dict.Add("{ipaddress}", minfo.Ipaddress);
                Dict.Add("{port}", minfo.Port);
                Dict.Add("{plctrgdevid}", minfo.plctrgdevid);
                Dict.Add("{plctrgdevtype}", minfo.plctrgdevtype);
                Dict.Add("{plctrgdevno}", minfo.plctrgdevno);
            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }

            return MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }

        public Task_Ret ChkPlcAccess(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                PlcCom plc = new PlcCom();

                if (!plc.CheckSocketConnect(minfo.Ipaddress, Int32.Parse(minfo.Port), ref Dbgmsg))
                {
                    string mes = Dbgmsg;
                    msg = ErrorMessage(taskid, fs, mes);
                    return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }
            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }

            return MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        public Task_Ret FileGetRequest_Plc(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                if (fs.mcfc.foi.useplcdev)
                {
                    PlcCom plc = new PlcCom();

                    if (plc.CheckSocketConnect(minfo.Ipaddress, Int32.Parse(minfo.Port), ref Dbgmsg))
                    {
                        //Command = "WD " + cmbDeviceType.Text + txtDeviceNo.Text + ".U " + txtCmdData.Text + "\r";
                        if (!plc.DeviceWrite(minfo.Ipaddress, int.Parse(minfo.Port), minfo.plctrgdevtype, int.Parse(minfo.plctrgdevno), 1, ref Dbgmsg))
                        {
                            string mes = Dbgmsg;
                            msg = ErrorMessage(taskid, fs, mes);
                            return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                        }

                        Dbgmsg += $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} " + minfo.plctrgdevtype + "-" + int.Parse(minfo.plctrgdevno) + $"に1を書き込みました: taskid={fs.lbl[1]}{taskid.ToString()}";
                    }
                    else
                    {
                        string mes = Dbgmsg;
                        msg = ErrorMessage(taskid, fs, mes);
                        return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                    }

                    // 成功
                    return MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
                }
                else
                {
                    return MakeRet(retkey.ok, "", "PLCデバイストリガはmacconfにて使用しない設定になっています", (int)retcode.Success);
                }

            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }
        }


        public Task_Ret MoveIn2TempFolder(int taskid, Mcfilesys fs, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                //CommonFuncs commons = new CommonFuncs();
                CommonFuncs.MoveFile(fs.filepath, fs.tmpfilepath);
                Thread.Sleep(1000);
            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure); ;
            }

            return MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        public Task_Ret OutputEndFile(int taskid, Mcfilesys fs, Task_Ret taskret, Dictionary<string,string> Dict, string endkey, ref string msg, ref string Dbgmsg)
        {
            try
            {
                var edcf = new Endfileconf();
                edcf.endfcontents = new List<string>(); // endファイル内容格納配列
                string contents = "";

                //返信用のresult,message,retcodeをここで追加
                Dict.Add("{result}", taskret.Result);
                Dict.Add("{message}", taskret.Msg);
                Dict.Add("{retcode}", taskret.RetCode.ToString());
                Dict.Add("{debugmsg}", taskret.DebugMsg.Replace("\r", "").Replace("\n", ""));

                //string MacFilePath = fs.fpath + @"\conf\macconf.ini";

                if (GeEndfileconfIniValues(fs, ref edcf))
                {
                    contents += edcf.stacode;
                    //if (edcf.needdbq == 1)
                    if (edcf.needdbq)
                    {
                        contents += '"';
                    }
                }
                else
                {
                    string mes = "エンドファイル設定が読み込めません";
                    msg = ErrorMessage(taskid, fs, mes);
                    return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                contents += edcf.stacode;

                //if (errorcode == 0)
                //{
                //    if (GetEndContentItems(fs, ref edcf))
                //    {
                //        //桁数を指定する対応（データ:桁数）
                //        foreach (string content in edcf.endfcontents)
                //        {
                //            //[0]: Key, [1]:文字数
                //            string[] contentdata = content.Split(':');

                //            //返信Dictにキーが存在しない場合は""をAdd
                //            if (!Dict.ContainsKey(contentdata[0]))
                //            {
                //                Dict.Add(contentdata[0], "");
                //            }

                //            //電文(contents)成形処理
                //            if (contentdata[0].Contains("[CR") || contentdata[0].Contains("LF]"))
                //            {
                //                //改行指定[CR][LF][CRLF]はカンマなし
                //                contents += Dict[contentdata[0]];
                //            }
                //            else if (contentdata.Length == 2)
                //            {
                //                contents += Dict[contentdata[0]].PadRight(int.Parse(contentdata[1]), ' ') + ",";
                //            }
                //            else
                //            {
                //                //現状ここには入らなくなっている模様
                //                contents += Dict[contentdata[0]] + ",";
                //            }
                //        }
                //    }
                //    else
                //    {
                //        string mes = "エンドファイル項目設定が読み込めません";
                //        msg = ErrorMessage(taskid, fs, mes);
                //        return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                //    }
                //}
                //else if (errorcode == 999)
                //{
                //    contents = $"CANCEL,0,";
                //}
                //else
                //{
                //    contents = $"ERROR,{fs.lbl[1]}{errorcode},";
                //}


                if (GetEndContentItems(fs, ref edcf))
                {
                    //桁数を指定する対応（データ:桁数）
                    foreach (string content in edcf.endfcontents)
                    {
                        //[0]: Key, [1]:文字数
                        string[] contentdata = content.Split(':');

                        //返信Dictにキーが存在しない場合は""をAdd
                        if (!Dict.ContainsKey(contentdata[0]))
                        {
                            Dict.Add(contentdata[0], "");
                        }

                        //電文(contents)成形処理
                        if (contentdata[0].Contains("[CR") || contentdata[0].Contains("LF]"))
                        {
                            //改行前のカンマを消去
                            contents = contents.TrimEnd(',');
                            //改行指定[CR][LF][CRLF]はカンマなし
                            contents += Dict[contentdata[0]];
                        }
                        else if (contentdata.Length == 2)
                        {
                            if (int.Parse(contentdata[1]) > 0)
                            {
                                contents += Dict[contentdata[0]].PadRight(int.Parse(contentdata[1]), ' ').Substring(0, int.Parse(contentdata[1])) + ",";
                            }
                            else
                            {
                                contents += Dict[contentdata[0]].PadRight(int.Parse(contentdata[1]), ' ') + ",";
                            }
                            
                        }
                        else
                        {
                            //現状ここには入らなくなっている模様
                            contents += Dict[contentdata[0]] + ",";
                        }
                    }
                }
                else
                {
                    string mes = "エンドファイル項目設定が読み込めません";
                    msg = ErrorMessage(taskid, fs, mes);
                    return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
                }

                //contents = contents.Substring(0, contents.Length - 1);
                contents = contents.TrimEnd(',');
                contents += edcf.stpcode;

                if (edcf.needdbq)
                {
                    contents += '"';
                }

                string EndFilePath = fs.fpath + @"\out\" + fs.MagCupNo + "_" + endkey + "." + fs.ext;
                StreamWriter sw = new StreamWriter(EndFilePath, false, Encoding.GetEncoding(edcf.encode));
                sw.Write(contents);
                sw.Close();
            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return MakeRet(retkey.ng, msg, Dbgmsg, (int)retcode.Failure);
            }

            return MakeRet(retkey.ok, "", Dbgmsg, (int)retcode.Success);
        }


        //private bool GeEndfileconfIniValues(string MacFilePath, ref Endfileconf edcf)
        private bool GeEndfileconfIniValues(Mcfilesys fs, ref Endfileconf edcf)
        {
            try
            {
                //CommonFuncs commons = new CommonFuncs();
                //
                // [Endfileconf]
                //
                // エンコード指定取得
                //edcf.encode = commons.GetIniValue(MacFilePath, "Endfileconf", "encode");
                edcf.encode = fs.mcfc.encoding;

                // データ開始コード
                //edcf.stacode = commons.GetIniValue(MacFilePath, "Endfileconf", "stacodekey");

                // データ終了コード
                //edcf.stpcode = commons.GetIniValue(MacFilePath, "Endfileconf", "stpcodekey");
                if (fs.mcfc.spfnc1)
                {
                    edcf.stpcode = "\r\n";
                }

                // ダブルクォーテーション要否
                //edcf.needdbq = int.Parse(commons.GetIniValue(MacFilePath, "Endfileconf", "needdbq"));
                //edcf.needdbq = fs.mcfc.spfnc1;
                edcf.needdbq = false;


                return true;
            }
            catch
            {
                return false;
            }
        }

        //private bool GetEndContentItems(string MacFilePath, string key, ref Endfileconf edcf)
        private bool GetEndContentItems(Mcfilesys fs, ref Endfileconf edcf)
        {
            try
            {
                /*
                CommonFuncs commons = new CommonFuncs();
                //
                // [Endfileconf]
                //
                // 出力項目及び文字数取得
                string endfilecontents_ini = commons.GetIniValue(MacFilePath, "Endfileconf", key);
                string[] contents = endfilecontents_ini.Split(',');
                foreach (string content in contents)
                {
                    if (content.Contains(":"))
                    {
                        edcf.endfcontents.Add(content);
                    }
                    else
                    {
                        edcf.endfcontents.Add(content + ":0");
                    }
                }
                */

                string[] rets = fs.mcfc.returns.Split(',');
                foreach (string ret in rets)
                {
                    // 桁数を指定する対応（データ:桁数）
                    if (ret.Contains(":"))
                    {
                        // 桁数指定あり
                        edcf.endfcontents.Add(ret);
                    }
                    else
                    {
                        // 桁数指定なし
                        edcf.endfcontents.Add(ret + ":0");
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        public bool CleanMagCupDir(Mcfilesys fs, List<string> fldn)
        {
            try
            {
                foreach (string fld in fldn)
                {
                    string cleanfolder = fs.fpath + $"\\{fld}";
                    foreach (string file in Directory.GetFiles(cleanfolder, "*.*"))
                    {
                        if (file.Contains(fs.MagCupNo))
                            File.Delete(file);
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CleanMagCupfiles(Mcfilesys fs, List<string> fldn)
        {
            try
            {
                foreach (string fld in fldn)
                {
                    string cleanfolder = fs.fpath + $"\\{fld}";
                    foreach (string file in Directory.GetFiles(cleanfolder, "*.*"))
                    {

                        File.Delete(file);
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string ErrorMessage(int taskid, Mcfilesys fs, string mes)
        {
            return $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.RecipeFile} " + mes + $": taskid={fs.lbl[1]}{taskid.ToString()}";
        }

        //
        public string[] MoveErrorFile(Mcfilesys fs, string filepath, string ErrorPath)
        {
            if (CommonFuncs.FileExists(filepath))
            {
                if (CommonFuncs.MoveFile(filepath, ErrorPath))
                {
                    return new string[] { $"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルはエラーフォルダに移動しました", Cnslcnf.msg_info.ToString() };
                }
                else
                {
                    return new string[] { $"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルはエラーフォルダに移動できません", Cnslcnf.msg_warn.ToString() };
                }
            }

            return new string[] { "NA", "" };
        }

        //
        public string[] MoveDoneFile(Mcfilesys fs, string filepath, string DonerPath)
        {
            if (CommonFuncs.FileExists(filepath))
            {
                if (CommonFuncs.MoveFile(filepath, DonerPath))
                {
                    return new string[] { $"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルはDoneフォルダに移動しました", Cnslcnf.msg_info.ToString() };
                }
                else
                {
                    return new string[] { $"設備:{fs.Pcat} ({fs.Macno})/ {fs.mclbl}:{fs.MagCupNo} の{fs.keylbl}ファイルはDoneフォルダに移動できません", Cnslcnf.msg_warn.ToString() };
                }
            }

            return new string[] { "NA", "" };
        }

        //
        /* 検出ファイル文字列から検出されたキー(String)を返します
         * 検出されない場合は "None"を返します
        */
        //
        public static string FindKey(Magcupini mci,  string File)
        {
            foreach (string key in mci.infilekey)
            {
                if (File.Contains(key))
                {
                    return key;
                }
            }
            foreach (string key in mci.wipfilekey)
            {
                if (File.Contains(key))
                {
                    return key;
                }
            }
            foreach (string key in mci.endfilekey)
            {
                if (File.Contains(key))
                {
                    return key;
                }
            }
            return "None";
        }


        //
        /* 検出ファイル文字列から検出されたキー(String)を返します
         * 検出されない場合は "None"を返します
        */
        //
        public static string FindKeyAlt(Magcupini mci, string File)
        {
            foreach (string key in mci.infilekey)
            {
                if (File.Contains(key))
                {
                    string keyAlt = key.Replace(".csv", "");
                    keyAlt = keyAlt.Replace(".txt", "");
                    keyAlt = keyAlt.Replace("_", "");
                    return keyAlt;
                }
            }
            foreach (string key in mci.wipfilekey)
            {
                if (File.Contains(key))
                {
                    string keyAlt = key.Replace(".csv", "");
                    keyAlt = keyAlt.Replace("_", "");
                    return keyAlt;
                }
            }
            foreach (string key in mci.endfilekey)
            {
                if (File.Contains(key))
                {
                    string keyAlt = key.Replace(".csv", "");
                    keyAlt = keyAlt.Replace("_", "");
                    return keyAlt;
                }
            }
            return "None";
        }

        public Task_Ret MakeRet(string result, string msg, string debugmsg, int retcode)
        {
            return new Task_Ret
            {
                Result = result,
                Msg = msg,
                DebugMsg = debugmsg,
                RetCode = retcode
            };
        }
    }
}
