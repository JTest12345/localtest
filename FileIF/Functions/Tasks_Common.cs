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

        public string[] GetMacInfoConf(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
        {
            try
            {
                //var mcc = new macconfini();

                /*
                if (!mcc.GetMacInfoProfile(fs, ref Dbgmsg))
                {
                    string mes = "設備情報がiniから取得ができません";
                    msg = ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
                */

                if (fs.mconf.Mchd.mcat.ToLower() == fs.Pcat.ToLower() && fs.mconf.Mchd.macno.ToLower() == fs.Macno.ToLower())
                {
                    minfo.Pcat = fs.mconf.Mchd.mcat;
                    minfo.Macno = fs.mconf.Mchd.macno;
                    Dict.Add("pcat", fs.mconf.Mchd.mcat);
                    Dict.Add("macno", fs.mconf.Mchd.mcat);
                }
                else
                {
                    string mes = "設備情報(confとfilesystem)が一致していません";
                    msg = ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { "OK" };
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
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
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
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { "OK" };
        }
        */

        // macconfフォルダ内json使用版　⇒　現在使用中
        public string[] GetPlcConnectConf(int taskid, Mcfilesys fs, Macconinfo minfo, ref Dictionary<string, string> Dict, ref string msg, ref string Dbgmsg)
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
                

                Dict.Add("ipaddress", minfo.Ipaddress);
                Dict.Add("port", minfo.Port);
                Dict.Add("plctrgdevid", minfo.plctrgdevid);
                Dict.Add("plctrgdevtype", minfo.plctrgdevtype);
                Dict.Add("plctrgdevno", minfo.plctrgdevno);
            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { "OK" };
        }

        public string[] ChkPlcAccess(int taskid, Mcfilesys fs, Macconinfo minfo, ref string msg, ref string Dbgmsg)
        {
            try
            {
                PlcCom plc = new PlcCom();

                if (!plc.CheckSocketConnect(minfo.Ipaddress, Int32.Parse(minfo.Port), ref Dbgmsg))
                {
                    string mes = Dbgmsg;
                    msg = ErrorMessage(taskid, fs, mes);
                    return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }
            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] {"OK"};
        }


        public string[] FileGetRequest_Plc(int taskid, Mcfilesys fs, Macconinfo minfo, ref string msg, ref string Dbgmsg)
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
                            return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                        }

                        Dbgmsg += $"設備:{fs.Pcat}({fs.Macno})/{fs.lbl[0]}:{fs.MagCupNo} " + minfo.plctrgdevtype + "-" + int.Parse(minfo.plctrgdevno) + $"に1を書き込みました: taskid={fs.lbl[1]}{taskid.ToString()}";
                    }
                    else
                    {
                        string mes = Dbgmsg;
                        msg = ErrorMessage(taskid, fs, mes);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                }
                else
                {
                    return new string[] { "OK", "", "PLCデバイストリガはmacconfにて使用しない設定になっています", taskid.ToString() };
                }

            }
            catch (Exception ex)
            {
                msg = ErrorMessage(taskid, fs, ex.Message);
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { "OK", "", "", taskid.ToString() };
        }


        public string[] MoveIn2TempFolder(int taskid, Mcfilesys fs, ref string msg, ref string Dbgmsg)
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
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { "OK" };
        }


        public string[] OutputEndFile(int taskid, Mcfilesys fs, int errorcode, Dictionary<string,string> Dict, string endkey, ref string msg, ref string Dbgmsg)
        {
            try
            {
                var edcf = new Endfileconf();
                edcf.endfcontents = new List<string>(); // endファイル内容格納配列
                string contents = "";
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
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                }

                contents += edcf.stacode;

                if (errorcode == 0)
                {
                    if (GetEndContentItems(fs, ref edcf))
                    {

                        foreach (string content in edcf.endfcontents)
                        {
                            string[] contentdata = content.Split(':');
                            contents += Dict[contentdata[0]].PadRight(int.Parse(contentdata[1]), ' ') + ",";
                        }
                        
                    }
                    else
                    {
                        string mes = "エンドファイル項目設定が読み込めません";
                        msg = ErrorMessage(taskid, fs, mes);
                        return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
                    }
                }
                else if (errorcode == 999)
                {
                    contents = $"CANCEL,0,";
                }
                else
                {
                    contents = $"ERROR,{fs.lbl[1]}{errorcode},";
                }

                //contents += "fin";
                //contents = contents.Replace(",fin", "");
                contents = contents.Substring(0, contents.Length - 1);
                contents += edcf.stpcode;
                //if (edcf.needdbq == 1)
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
                return new string[] { "NG", msg, Dbgmsg, taskid.ToString() };
            }

            return new string[] { "OK" };
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

                // ダブルクォーテーション要否
                //edcf.needdbq = int.Parse(commons.GetIniValue(MacFilePath, "Endfileconf", "needdbq"));
                edcf.needdbq = fs.mcfc.spfnc1;


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
                    if (ret.Contains(":"))
                    {
                        edcf.endfcontents.Add(ret);
                    }
                    else
                    {
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
    }
}
